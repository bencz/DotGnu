/*
 * lib_attrs.c - Builtin library attributes with special meanings.
 *
 * Copyright (C) 2002, 2008, 2009  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include "program.h"
#include "il_serialize.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Process a "Serializable" attribute.
 */
static int SerializableAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILClass *classInfo;

	/* We must use this on a class */
	classInfo = _ILProgramItem_ToClass(item);
	if(!classInfo)
	{
		return 0;
	}

	/* Mark the class as serialized */
	ILClassSetAttrs(classInfo, IL_META_TYPEDEF_SERIALIZABLE,
							   IL_META_TYPEDEF_SERIALIZABLE);
	return 1;
}

/*
 * Process a "NonSerialized" attribute.
 */
static int NonSerializedAttribute(ILProgramItem *item,
								  ILSerializeReader *reader)
{
	ILField *fieldInfo;

	/* We must use this on a field */
	fieldInfo = _ILProgramItem_ToFieldDef(item);
	if(!fieldInfo)
	{
		return 0;
	}

	/* Mark the field as non-serialized */
	ILMemberSetAttrs((ILMember *)fieldInfo,
					 IL_META_FIELDDEF_NOT_SERIALIZED,
				     IL_META_FIELDDEF_NOT_SERIALIZED);
	return 1;
}

/*
 * Match an attribute parameter name and type.
 */
#define	IsParam(name,ptype)		\
			(paramNameLen == strlen((name)) && \
			 !strncmp(paramName, (name), paramNameLen) && \
			 type == (ptype))

/*
 * Process a DLL importing attribute, to define PInvoke information.
 */
static int DllImportAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILMethod *method;
	ILField *field;
	const char *name;
	int nameLen;
	ILUInt32 attrs;
	const char *entryPoint;
	int entryPointLen;
	int numExtra;
	const char *paramName;
	int paramNameLen;
	int type;
	ILMember *member;
	char *dllName;
	char *aliasName;
	ILModule *module;
	int result;

	/* According to the ECMA spec, we must use this on a method.
	   We have added an extension to also support PInvoke'ed fields.
	   The metadata supports PInvoke information on fields, and it
	   is necessary for importing variables from shared objects */
	method = _ILProgramItem_ToMethodDef(item);
	field = _ILProgramItem_ToFieldDef(item);
	if(!method && !field)
	{
		return 0;
	}

	/* If it is a field, then it must be static and non-literal */
	if(field && (!ILField_IsStatic(field) || ILField_IsLiteral(field)))
	{
		return 0;
	}

	/* Get the name of the DLL, which is the only attribute parameter */
	if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
	{
		return 0;
	}
	nameLen = ILSerializeReaderGetString(reader, &name);
	if(nameLen < 0)
	{
		return 0;
	}
	if(ILSerializeReaderGetParamType(reader) != 0)
	{
		return 0;
	}

	/* Collect up extra information from the attribute blob */
	attrs = IL_META_PINVOKE_CALL_CONV_CDECL;
	entryPoint = 0;
	entryPointLen = -1;
	numExtra = ILSerializeReaderGetNumExtra(reader);
	if(numExtra < 0)
	{
		return 0;
	}
	while(numExtra > 0)
	{
		type = ILSerializeReaderGetExtra(reader, &member, &paramName,
										 &paramNameLen);
		if(type == -1)
		{
			return 0;
		}
		if(IsParam("CallingConvention", IL_META_SERIALTYPE_I4))
		{
			attrs &= ~IL_META_PINVOKE_CALL_CONV_MASK;
			attrs |= (ILUInt32)(ILSerializeReaderGetInt32(reader, type) << 8);
		}
		else if(IsParam("CharSet", IL_META_SERIALTYPE_I4))
		{
			attrs &= ~IL_META_PINVOKE_CHAR_SET_MASK;
			attrs |=
				(ILUInt32)((ILSerializeReaderGetInt32(reader, type) - 1) << 1);
		}
		else if(IsParam("EntryPoint", IL_META_SERIALTYPE_STRING))
		{
			entryPointLen = ILSerializeReaderGetString(reader, &entryPoint);
			if(entryPointLen < 0)
			{
				return 0;
			}
		}
		else if(IsParam("ExactSpelling", IL_META_SERIALTYPE_BOOLEAN))
		{
			if(ILSerializeReaderGetInt32(reader, type))
			{
				attrs |= IL_META_PINVOKE_NO_MANGLE;
			}
			else
			{
				attrs &= ~IL_META_PINVOKE_NO_MANGLE;
			}
		}
		else if(IsParam("PreserveSig", IL_META_SERIALTYPE_BOOLEAN))
		{
			if(ILSerializeReaderGetInt32(reader, type))
			{
				attrs |= IL_META_PINVOKE_OLE;
			}
			else
			{
				attrs &= ~IL_META_PINVOKE_OLE;
			}
		}
		else if(IsParam("SetLastError", IL_META_SERIALTYPE_BOOLEAN))
		{
			if(ILSerializeReaderGetInt32(reader, type))
			{
				attrs |= IL_META_PINVOKE_SUPPORTS_LAST_ERROR;
			}
			else
			{
				attrs &= ~IL_META_PINVOKE_SUPPORTS_LAST_ERROR;
			}
		}
		else
		{
			return 0;
		}
		--numExtra;
	}

	/* Create the PInvoke record for the method */
	dllName = ILDupNString(name, nameLen);
	if(!dllName)
	{
		return -1;
	}
	if(entryPoint)
	{
		aliasName = ILDupNString(entryPoint, entryPointLen);
		if(!aliasName)
		{
			ILFree(dllName);
			return -1;
		}
	}
	else
	{
		aliasName = 0;
	}
	module = ILModuleRefCreateUnique(ILProgramItem_Image(item), dllName);
	if(!module)
	{
		ILFree(dllName);
		if(aliasName)
		{
			ILFree(aliasName);
		}
		return -1;
	}
	if(method)
	{
		result = (ILPInvokeCreate(method, 0, attrs, module, aliasName) != 0);
		if(result)
		{
			/* Mark the method with the "pinvokeimpl" flag */
			ILMemberSetAttrs((ILMember *)method,
							 IL_META_METHODDEF_PINVOKE_IMPL,
							 IL_META_METHODDEF_PINVOKE_IMPL);
		}
	}
	else
	{
		result = (ILPInvokeFieldCreate
						(field, 0, attrs, module, aliasName) != 0);
		if(result)
		{
			/* Mark the field with the "pinvokeimpl" flag */
			ILMemberSetAttrs((ILMember *)field,
							 IL_META_FIELDDEF_PINVOKE_IMPL,
							 IL_META_FIELDDEF_PINVOKE_IMPL);
		}
	}
	ILFree(dllName);
	if(aliasName)
	{
		ILFree(aliasName);
	}
	return (result ? 1 : -1);
}

/*
 * Process a field offset attribute.
 */
static int FieldOffsetAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILField *field;
	ILFieldLayout *layout;
	int type;
	ILUInt32 offset;

	/* We must use this on a field */
	field = _ILProgramItem_ToFieldDef(item);
	if(!field)
	{
		return 0;
	}

	/* Get the first parameter, which should be int32 */
	type = ILSerializeReaderGetParamType(reader);
	if(type != IL_META_SERIALTYPE_I4)
	{
		return 0;
	}
	offset = (ILUInt32)(ILSerializeReaderGetInt32(reader, type));
	if(ILSerializeReaderGetParamType(reader) != 0 ||
	   ILSerializeReaderGetNumExtra(reader) != 0)
	{
		return 0;
	}

	/* Set the field offset */
	layout = ILFieldLayoutGetFromOwner(field);
	if(layout)
	{
		layout->offset = offset;
	}
	else if((layout = ILFieldLayoutCreate(ILProgramItem_Image(field), 0,
										  field, offset)) == 0)
	{
		return -1;
	}
	return 1;
}

/*
 * Process an "in" attribute on a parameter.
 */
static int InAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILParameter *param;

	/* We must use this on a parameter */
	param = _ILProgramItem_ToParamDef(item);
	if(!param)
	{
		return 0;
	}

	/* There should be no parameters or extra information */
	if(ILSerializeReaderGetParamType(reader) != 0 ||
	   ILSerializeReaderGetNumExtra(reader) != 0)
	{
		return 0;
	}

	/* Set the "in" flag on the parameter */
	ILParameterSetAttrs(param, IL_META_PARAMDEF_IN, IL_META_PARAMDEF_IN);
	return 1;
}

/*
 * Process an "out" attribute on a parameter.
 */
static int OutAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILParameter *param;

	/* We must use this on a parameter */
	param = _ILProgramItem_ToParamDef(item);
	if(!param)
	{
		return 0;
	}

	/* There should be no parameters or extra information */
	if(ILSerializeReaderGetParamType(reader) != 0 ||
	   ILSerializeReaderGetNumExtra(reader) != 0)
	{
		return 0;
	}

	/* Set the "out" flag on the parameter */
	ILParameterSetAttrs(param, IL_META_PARAMDEF_OUT, IL_META_PARAMDEF_OUT);
	return 1;
}

/*
 * Process an "optional" attribute on a parameter.
 */
static int OptionalAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILParameter *param;

	/* We must use this on a parameter */
	param = _ILProgramItem_ToParamDef(item);
	if(!param)
	{
		return 0;
	}

	/* There should be no parameters or extra information */
	if(ILSerializeReaderGetParamType(reader) != 0 ||
	   ILSerializeReaderGetNumExtra(reader) != 0)
	{
		return 0;
	}

	/* Set the "out" flag on the parameter */
	ILParameterSetAttrs(param, IL_META_PARAMDEF_OPTIONAL,
						IL_META_PARAMDEF_OPTIONAL);
	return 1;
}

/*
 * Process a structure layout attribute.
 */
static int StructLayoutAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILClass *classInfo;
	int type;
	ILUInt32 attrs;
	ILInt32 sizeValue = 0;
	ILInt32 packValue = 0;
	ILClassLayout *layout;
	int numExtra;
	const char *paramName;
	int paramNameLen;
	ILMember *member;

	/* We must use this on a class */
	classInfo = _ILProgramItem_ToClass(item);
	if(!classInfo)
	{
		return 0;
	}

	/* Get the layout kind, which must be either int32 or int16 */
	type = ILSerializeReaderGetParamType(reader);
	if(type != IL_META_SERIALTYPE_I4 && type != IL_META_SERIALTYPE_I2)
	{
		return 0;
	}
	type = (int)(ILSerializeReaderGetInt32(reader, type));
	if(ILSerializeReaderGetParamType(reader) != 0)
	{
		return 0;
	}

	/* Get the current state of the class's attributes */
	attrs = ILClass_Attrs(classInfo);

	/* Convert the kind value into a layout attribute value */
	switch(type)
	{
		case 0:
		{
			attrs = (attrs & ~IL_META_TYPEDEF_LAYOUT_MASK) |
					IL_META_TYPEDEF_LAYOUT_SEQUENTIAL;
		}
		break;

		case 2:
		{
			attrs = (attrs & ~IL_META_TYPEDEF_LAYOUT_MASK) |
					IL_META_TYPEDEF_EXPLICIT_LAYOUT;
		}
		break;

		case 3:
		{
			attrs = (attrs & ~IL_META_TYPEDEF_LAYOUT_MASK) |
					IL_META_TYPEDEF_AUTO_LAYOUT;
		}
		break;

		default:	break;
	}

	/* Collect up extra information from the attribute blob */
	numExtra = ILSerializeReaderGetNumExtra(reader);
	if(numExtra < 0)
	{
		return 0;
	}
	while(numExtra > 0)
	{
		type = ILSerializeReaderGetExtra(reader, &member, &paramName,
										 &paramNameLen);
		if(type == -1)
		{
			return 0;
		}
		if(IsParam("CharSet", IL_META_SERIALTYPE_I4))
		{
			attrs &= ~IL_META_TYPEDEF_STRING_FORMAT_MASK;
			type = (int)(ILSerializeReaderGetInt32(reader, type));
			if(type == 1 || type == 4)
			{
				attrs |= IL_META_TYPEDEF_AUTO_CLASS;
			}
			else if(type == 3)
			{
				attrs |= IL_META_TYPEDEF_UNICODE_CLASS;
			}
		}
		else if(IsParam("Size", IL_META_SERIALTYPE_I4))
		{
			sizeValue = ILSerializeReaderGetInt32(reader, type);
		}
		else if(IsParam("Pack", IL_META_SERIALTYPE_I4))
		{
			packValue = ILSerializeReaderGetInt32(reader, type);
		}
		else
		{
			return 0;
		}
		--numExtra;
	}

	/* Change the attributes for the class */
	ILClassSetAttrs(classInfo, ~((ILUInt32)0), attrs);

	/* Add size and packing information if necessary */
	layout = ILClassLayoutGetFromOwner(classInfo);
	if(layout)
	{
		/* Modify an existing layout information block */
		ILClassLayoutSetClassSize(layout, sizeValue);
		ILClassLayoutSetPackingSize(layout, packValue);
	}
	else
	{
		/* Create a new layout block if necessary */
		if(sizeValue != 0 || packValue != 0)
		{
			if(!ILClassLayoutCreate(ILProgramItem_Image(classInfo), 0,
									classInfo, packValue, sizeValue))
			{
				return -1;
			}
		}
	}

	/* Done */
	return 1;
}

/*
 * Process a field or parameter marshalling attribute.
 */
static int MarshalAsAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	int type;
	ILUInt32 unmanagedType;
	ILUInt32 arraySubType = IL_META_NATIVETYPE_END;
	ILUInt32 safeArraySubType = IL_META_VARIANTTYPE_EMPTY;
	int numExtra;
	const char *paramName;
	int paramNameLen;
	ILMember *member;
	const char *marshalCookie = 0;
	int marshalCookieLen = 0;
	const char *marshalType = 0;
	int marshalTypeLen = 0;
	ILInt32 sizeValue = 0;
	ILInt32 sizeParamIndex = 0;
	unsigned char buf[64];
	unsigned char *blob;
	int blobLen;
	ILFieldMarshal *marshal;

	/* We must use this on a field or parameter */
	if(!_ILProgramItem_ToFieldDef(item) && !_ILProgramItem_ToParamDef(item))
	{
		return 0;
	}

	/* Get the unmanaged type, which must be either int32 or int16 */
	type = ILSerializeReaderGetParamType(reader);
	if(type != IL_META_SERIALTYPE_I4 && type != IL_META_SERIALTYPE_I2)
	{
		return 0;
	}
	unmanagedType = (ILUInt32)(ILSerializeReaderGetInt32(reader, type));
	if(ILSerializeReaderGetParamType(reader) != 0)
	{
		return 0;
	}

	/* Collect up extra information from the attribute blob */
	numExtra = ILSerializeReaderGetNumExtra(reader);
	if(numExtra < 0)
	{
		return 0;
	}
	while(numExtra > 0)
	{
		type = ILSerializeReaderGetExtra(reader, &member, &paramName,
										 &paramNameLen);
		if(type == -1)
		{
			return 0;
		}
		if(IsParam("ArraySubType", IL_META_SERIALTYPE_I4))
		{
			arraySubType = (ILUInt32)(ILSerializeReaderGetInt32(reader, type));
		}
		else if(IsParam("SafeArraySubType", IL_META_SERIALTYPE_I4))
		{
			safeArraySubType =
				(ILUInt32)(ILSerializeReaderGetInt32(reader, type));
		}
		else if(IsParam("MarshalCookie", IL_META_SERIALTYPE_STRING))
		{
			marshalCookieLen = ILSerializeReaderGetString
					(reader, &marshalCookie);
			if(marshalCookieLen < 0)
			{
				return 0;
			}
		}
		else if(IsParam("MarshalType", IL_META_SERIALTYPE_STRING) ||
		        IsParam("MarshalTypeRef", IL_META_SERIALTYPE_TYPE))
		{
			marshalTypeLen = ILSerializeReaderGetString(reader, &marshalType);
			if(marshalTypeLen < 0)
			{
				return 0;
			}
		}
		else if(IsParam("SizeConst", IL_META_SERIALTYPE_I4))
		{
			sizeValue = ILSerializeReaderGetInt32(reader, type);
		}
		else if(IsParam("SizeParamIndex", IL_META_SERIALTYPE_I2))
		{
			sizeParamIndex = ILSerializeReaderGetInt32(reader, type);
		}
		else
		{
			return 0;
		}
		--numExtra;
	}

	/* Build the marshalling blob that corresponds to the supplied data */
	switch(unmanagedType)
	{
		case IL_META_NATIVETYPE_SAFEARRAY:
		{
			/* Safe variant array */
			buf[0] = IL_META_NATIVETYPE_SAFEARRAY;
			blobLen = 1 + ILMetaCompressData(buf + 1, safeArraySubType);
			blob = buf;
		}
		break;

		case IL_META_NATIVETYPE_FIXEDARRAY:
		{
			/* Fixed length array */
			buf[0] = IL_META_NATIVETYPE_FIXEDARRAY;
			blobLen = 1 + ILMetaCompressData(buf + 1, (ILUInt32)sizeValue);
			if(arraySubType == IL_META_NATIVETYPE_END)
			{
				buf[blobLen++] = IL_META_NATIVETYPE_MAX;
			}
			else
			{
				blobLen += ILMetaCompressData(buf + blobLen, arraySubType);
			}
			blob = buf;
		}
		break;

		case IL_META_NATIVETYPE_ARRAY:
		{
			/* An array that has a size value in a separate parameter */
			if(arraySubType == IL_META_NATIVETYPE_END)
			{
				/* Unspecified array element types should be "MAX" */
				arraySubType = IL_META_NATIVETYPE_MAX;
			}
			buf[0] = IL_META_NATIVETYPE_ARRAY;
			blobLen = 1 + ILMetaCompressData(buf + 1, arraySubType);
			blobLen += ILMetaCompressData
				(buf + blobLen, (ILUInt32)sizeParamIndex);
			buf[blobLen++] = 1;		/* Multiplier */
			buf[blobLen++] = 0;		/* Number of elements */
			blob = buf;
		}
		break;

		case IL_META_NATIVETYPE_CUSTOMMARSHALER:
		{
			/* Custom marshalling directive */
			blob = (unsigned char *)ILMalloc(IL_META_COMPRESS_MAX_SIZE * 2 +
								     3 + marshalTypeLen + marshalCookieLen);
			if(!blob)
			{
				return 0;
			}
			blob[0] = IL_META_NATIVETYPE_CUSTOMMARSHALER;
			blob[1] = 0;	/* Length of GUID string (unused) */
			blob[2] = 0;	/* Length of native type name string (unused) */
			blobLen = 3;
			blobLen += ILMetaCompressData
				(blob + blobLen, (ILUInt32)marshalTypeLen);
			if(marshalTypeLen > 0)
			{
				ILMemCpy(blob + blobLen, marshalType, marshalTypeLen);
				blobLen += marshalTypeLen;
			}
			blobLen += ILMetaCompressData
				(blob + blobLen, (ILUInt32)marshalCookieLen);
			if(marshalCookieLen > 0)
			{
				ILMemCpy(blob + blobLen, marshalCookie, marshalCookieLen);
				blobLen += marshalCookieLen;
			}
		}
		break;

		default:
		{
			/* This native type is represented as a simple value */
			blobLen = ILMetaCompressData(buf, unmanagedType);
			blob = buf;
			blobLen = 1;
		}
		break;
	}

	/* Add the marshalling information to the program item */
	marshal = ILFieldMarshalGetFromOwner(item);
	if(!marshal)
	{
		marshal = ILFieldMarshalCreate(ILProgramItem_Image(item), 0, item);
		if(!marshal)
		{
			if(blob != buf)
			{
				ILFree(blob);
			}
			return -1;
		}
	}
	if(!ILFieldMarshalSetType(marshal, blob, blobLen))
	{
		if(blob != buf)
		{
			ILFree(blob);
		}
		return -1;
	}

	/* Done */
	if(blob != buf)
	{
		ILFree(blob);
	}
	return 1;
}

/*
 * Process a "ComImport" attribute, which sets the "import" flag on the type.
 */
static int ComImportAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILClass *classInfo;

	/* We must use this on a class */
	classInfo = _ILProgramItem_ToClass(item);
	if(!classInfo)
	{
		return 0;
	}

	/* Mark the class as serialized */
	ILClassSetAttrs(classInfo, IL_META_TYPEDEF_IMPORT,
							   IL_META_TYPEDEF_IMPORT);
	return 1;
}

/*
 * Process a method implementation attribute.
 */
static int MethodImplAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	ILMethod *method;
	int type;
	ILUInt32 attrs;

	/* We must use this on a method */
	method = _ILProgramItem_ToMethodDef(item);
	if(!method)
	{
		return 0;
	}

	/* Get the first parameter, which should be int32 or int16 */
	type = ILSerializeReaderGetParamType(reader);
	if(type != IL_META_SERIALTYPE_I4 && type != IL_META_SERIALTYPE_I2)
	{
		return 0;
	}
	attrs = (ILUInt32)(ILSerializeReaderGetInt32(reader, type) & 0xFFFF);
	if(ILSerializeReaderGetParamType(reader) != 0 ||
	   ILSerializeReaderGetNumExtra(reader) != 0)
	{
		return 0;
	}

	/* Merge the attributes into the method's definition */
	ILMethodSetImplAttrs(method, ~IL_META_METHODIMPL_CODE_TYPE_MASK,
						 (attrs & ~IL_META_METHODIMPL_CODE_TYPE_MASK));
	return 1;
}

/*
 * Process an indexer name attribute.
 */
static int IndexerNameAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	/* We must use this on a property, and we just remove it when found */
	return (_ILProgramItem_ToPropertyDef(item) != 0);
}

/*
 * Convert a string from UTF-8 to UTF-16.
 */
static void *StringToUTF16(const char *str, ILUInt32 *len, int slen)
{
	int posn = 0;
	unsigned long ch;
	ILUInt32 index;
	char *utf16;

	/* Determine the length of the UTF-16 string in bytes */
	*len = 0;
	while(posn < slen)
	{
		ch = ILUTF8ReadChar(str, slen, &posn);
		*len += (ILUInt32)ILUTF16WriteCharAsBytes(0, ch);
	}

	/* Allocate space for the UTF-16 string */
	utf16 = (char *)ILMalloc(*len * 2);
	if(!utf16)
	{
		return 0;
	}

	/* Convert the string from UTF-8 to UTF-16 */
	index = 0;
	posn = 0;
	while(posn < slen)
	{
		ch = ILUTF8ReadChar(str, slen, &posn);
		index += (ILUInt32)ILUTF16WriteCharAsBytes(utf16 + index, ch);
	}
	return utf16;
}

/*
 * Process "System.ComponentModel.DefaultValueAttribute" values
 * that exist on parameters, turning them into metadata constants.
 * This is used to support interoperation with VB.NET.
 */
static int DefaultValueAttribute(ILProgramItem *item, ILSerializeReader *reader)
{
	int type, elemType;
	unsigned char blob[8];
	ILUInt32 blobLen;
	int len;
	const char *str;
	ILConstant *constant;
	void *utf16;

	/* We only convert the attribute if it is on a parameter */
	if(!_ILProgramItem_ToParamDef(item))
	{
		return 0;
	}

	/* Get the value type, to determine how to convert it */
	type = ILSerializeReaderGetParamType(reader);
resolveType:
	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		{
			elemType = IL_META_ELEMTYPE_BOOLEAN;
			blob[0] = (unsigned char)(ILSerializeReaderGetInt32(reader, type));
			blobLen = 1;
		}
		break;

		case IL_META_SERIALTYPE_CHAR:
		{
			ILUInt16 value = (ILUInt16)
				(ILSerializeReaderGetInt32(reader, type));
			elemType = IL_META_ELEMTYPE_CHAR;
			IL_WRITE_UINT16(blob, value);
			blobLen = 2;
		}
		break;

		case IL_META_SERIALTYPE_I1:
		{
			elemType = IL_META_ELEMTYPE_I1;
			blob[0] = (unsigned char)(ILSerializeReaderGetInt32(reader, type));
			blobLen = 1;
		}
		break;

		case IL_META_SERIALTYPE_U1:
		{
			elemType = IL_META_ELEMTYPE_U1;
			blob[0] = (unsigned char)(ILSerializeReaderGetInt32(reader, type));
			blobLen = 1;
		}
		break;

		case IL_META_SERIALTYPE_I2:
		{
			ILInt16 value = (ILInt16)
				(ILSerializeReaderGetInt32(reader, type));
			elemType = IL_META_ELEMTYPE_I2;
			IL_WRITE_INT16(blob, value);
			blobLen = 2;
		}
		break;

		case IL_META_SERIALTYPE_U2:
		{
			ILUInt16 value = (ILUInt16)
				(ILSerializeReaderGetInt32(reader, type));
			elemType = IL_META_ELEMTYPE_U2;
			IL_WRITE_UINT16(blob, value);
			blobLen = 2;
		}
		break;

		case IL_META_SERIALTYPE_I4:
		{
			ILInt32 value = (ILInt32)
				(ILSerializeReaderGetInt32(reader, type));
			elemType = IL_META_ELEMTYPE_I4;
			IL_WRITE_INT32(blob, value);
			blobLen = 4;
		}
		break;

		case IL_META_SERIALTYPE_U4:
		{
			ILUInt32 value = (ILUInt32)
				(ILSerializeReaderGetInt32(reader, type));
			elemType = IL_META_ELEMTYPE_U4;
			IL_WRITE_UINT32(blob, value);
			blobLen = 4;
		}
		break;

		case IL_META_SERIALTYPE_I8:
		{
			ILInt64 value = ILSerializeReaderGetInt64(reader);
			elemType = IL_META_ELEMTYPE_I8;
			IL_WRITE_INT64(blob, value);
			blobLen = 8;
		}
		break;

		case IL_META_SERIALTYPE_U8:
		{
			ILUInt64 value = ILSerializeReaderGetUInt64(reader);
			elemType = IL_META_ELEMTYPE_U8;
			IL_WRITE_UINT64(blob, value);
			blobLen = 8;
		}
		break;

		case IL_META_SERIALTYPE_R4:
		{
			ILFloat value = ILSerializeReaderGetFloat32(reader);
			elemType = IL_META_ELEMTYPE_R4;
			IL_WRITE_FLOAT(blob, value);
			blobLen = 4;
		}
		break;

		case IL_META_SERIALTYPE_R8:
		{
			ILDouble value = ILSerializeReaderGetFloat64(reader);
			elemType = IL_META_ELEMTYPE_R8;
			IL_WRITE_DOUBLE(blob, value);
			blobLen = 8;
		}
		break;

		case IL_META_SERIALTYPE_STRING:
		{
			len = ILSerializeReaderGetString(reader, &str);
			if(len < 0 || !str)
			{
				/* Encode a null value for the string */
				elemType = IL_META_ELEMTYPE_CLASS;
				blob[0] = 0;
				blob[1] = 0;
				blob[2] = 0;
				blob[3] = 0;
				blobLen = 4;
			}
			else
			{
				/* Convert the string into UTF16 and create a constant */
				utf16 = StringToUTF16(str, &blobLen, len);
				if(!utf16)
				{
					return 0;
				}
				constant = ILConstantCreate
					(ILProgramItem_Image(item), 0, item,
					 IL_META_ELEMTYPE_STRING);
				if(!constant)
				{
					ILFree(utf16);
					return 0;
				}
				if(!ILConstantSetValue(constant, utf16, blobLen))
				{
					ILFree(utf16);
					return 0;
				}
				ILFree(utf16);
				return 1;
			}
		}
		break;

		case IL_META_SERIALTYPE_VARIANT:
		{
			type = ILSerializeReaderGetBoxedPrefix(reader);
			goto resolveType;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_ENUM:
		{
			if(ILSerializeReaderGetString(reader, &str) < 0)
			{
				return 0;
			}
			type = IL_META_ELEMTYPE_I4;
			goto resolveType;
		}
		/* Not reached */

		default: return 0;
	}

	/* Create a constant node and attach it to the parameter */
	constant = ILConstantCreate(ILProgramItem_Image(item), 0, item, elemType);
	if(!constant)
	{
		return 0;
	}
	if(!ILConstantSetValue(constant, blob, blobLen))
	{
		return 0;
	}
	return 1;
}

/*
 * Concatenate two strings.
 */
static char *ConcatStrings(char *str1, const char *str2)
{
	char *newStr;
	int len;

	/* Bail out if we ran out of memory last time */
	if(!str1)
	{
		return str1;
	}

	/* Allocate space for the new string */
	len = strlen(str1);
	newStr = (char *)ILRealloc(str1, len + strlen(str2) + 1);
	if(!newStr)
	{
		ILFree(str1);
		return 0;
	}

	/* Create the new string */
	strcpy(newStr + len, str2);
	return newStr;
}

/*
 * Concatenate a name to a string if a particular flag is set.
 */
static char *ConcatFlag(ILInt32 flags, ILInt32 flag, char *str,
						const char *name, int *comma)
{
	if((flag && (flags & flag) == flag) || (!flag && !flags))
	{
		if(*comma)
		{
			str = ConcatStrings(str, ", ");
		}
		else
		{
			*comma = 1;
		}
		str = ConcatStrings(str, name);
	}
	return str;
}

/*
 * Process a security permission.
 */
static int SecurityPermissionAttribute(ILProgramItem *item,
									   ILSerializeReader *reader)
{
	int type;
	ILInt32 action;
	int numExtra;
	const char *paramName;
	int paramNameLen;
	ILInt32 flags;
	ILMember *member;
	char *result;
	int comma;
	void *utf16;
	ILUInt32 utf16Len;
	ILDeclSecurity *decl;

	/* The item must be a class, method, or assembly */
	if(!_ILProgramItem_ToTypeDef(item) &&
	   !_ILProgramItem_ToMethodDef(item) &&
	   !_ILProgramItem_ToAssemblyDef(item))
	{
		return 0;
	}

	/* The first and only parameter should be a "SecurityAction" value */
	type = ILSerializeReaderGetParamType(reader);
	if(type != IL_META_SERIALTYPE_I4)
	{
		return 0;
	}
	action = ILSerializeReaderGetInt32(reader, type);
	if(ILSerializeReaderGetParamType(reader) != 0)
	{
		return 0;
	}

	/* Extract the flags */
	flags = 0;
	numExtra = ILSerializeReaderGetNumExtra(reader);
	if(numExtra < 0)
	{
		return 0;
	}
	while(numExtra > 0)
	{
		type = ILSerializeReaderGetExtra(reader, &member, &paramName,
										 &paramNameLen);
		if(type == -1)
		{
			return 0;
		}
		if(IsParam("Flags", IL_META_SERIALTYPE_I4))
		{
			flags |= ILSerializeReaderGetInt32(reader, type);
		}
		else if(IsParam("Assertion", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0001;
		}
		else if(IsParam("UnmanagedCode", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0002;
		}
		else if(IsParam("SkipVerification", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0004;
		}
		else if(IsParam("Execution", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0008;
		}
		else if(IsParam("ControlThread", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0010;
		}
		else if(IsParam("ControlEvidence", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0020;
		}
		else if(IsParam("ControlPolicy", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0040;
		}
		else if(IsParam("SerializationFormatter", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0080;
		}
		else if(IsParam("ControlDomainPolicy", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0100;
		}
		else if(IsParam("ControlPrincipal", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0200;
		}
		else if(IsParam("ControlAppDomain", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0400;
		}
		else if(IsParam("RemotingConfiguration", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x0800;
		}
		else if(IsParam("Infrastructure", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x1000;
		}
		else if(IsParam("BindingRedirects", IL_META_SERIALTYPE_BOOLEAN))
		{
			ILSerializeReaderGetInt32(reader, type);
			flags |= 0x2000;
		}
		else
		{
			return 0;
		}
		--numExtra;
	}

	/* Create the permission block */
	result = ILDupString
		("<PermissionSet class=\"PermissionSet\"\r\n"
		 "               version=\"1\">\r\n"
		 "   <IPermission class=\"System.Security.Permissions."
		 		"SecurityAttribute, mscorlib\"\r\n"
		 "                version=\"1\"\r\n"
		 "                Flags=\"");
	comma = 0;
	result = ConcatFlag(flags, 0x0000, result, "NoFlags", &comma);
	result = ConcatFlag(flags, 0x0001, result, "Assertion", &comma);
	result = ConcatFlag(flags, 0x0002, result, "UnmanagedCode", &comma);
	result = ConcatFlag(flags, 0x0004, result, "SkipVerification", &comma);
	result = ConcatFlag(flags, 0x0008, result, "Execution", &comma);
	result = ConcatFlag(flags, 0x0010, result, "ControlThread", &comma);
	result = ConcatFlag(flags, 0x0020, result, "ControlEvidence", &comma);
	result = ConcatFlag(flags, 0x0040, result, "ControlPolicy", &comma);
	result = ConcatFlag
		(flags, 0x0080, result, "SerializationFormatter", &comma);
	result = ConcatFlag(flags, 0x0100, result, "ControlDomainPolicy", &comma);
	result = ConcatFlag(flags, 0x0200, result, "ControlPrincipal", &comma);
	result = ConcatFlag(flags, 0x0400, result, "ControlAppDomain", &comma);
	result = ConcatFlag
		(flags, 0x0800, result, "RemotingConfiguration", &comma);
	result = ConcatFlag(flags, 0x1000, result, "Infrastructure", &comma);
	result = ConcatFlag(flags, 0x2000, result, "BindingRedirects", &comma);
	result = ConcatStrings(result, "\"/>\r\n</PermissionSet>\r\n");
	if(!result)
	{
		return 0;
	}

	/* Convert the string into UTF-16 */
	utf16 = StringToUTF16(result, &utf16Len, strlen(result));
	if(!utf16)
	{
		ILFree(result);
		return 0;
	}
	ILFree(result);

	/* Create a security declaration and attach it to the item */
	decl = ILDeclSecurityCreate(ILProgramItem_Image(item), 0, item, action);
	if(!decl)
	{
		ILFree(utf16);
		return 0;
	}
	if(!ILDeclSecuritySetBlob(decl, utf16, utf16Len))
	{
		ILFree(utf16);
		return 0;
	}
	ILFree(utf16);

	/* The attribute has been converted */
	return 1;
}

/*
 * Attribute lookup tables.
 */
typedef struct
{
	const char *name;
	int (*func)(ILProgramItem *item, ILSerializeReader *reader);

} AttrConvertInfo;
static AttrConvertInfo const systemAttrs[] = {
	{"SerializableAttribute", SerializableAttribute},
	{"NonSerializedAttribute", NonSerializedAttribute},
	{0, 0}
};
static AttrConvertInfo const interopAttrs[] = {
	{"DllImportAttribute",	DllImportAttribute},
	{"FieldOffsetAttribute", FieldOffsetAttribute},
	{"InAttribute", InAttribute},
	{"OutAttribute", OutAttribute},
	{"OptionalAttribute", OptionalAttribute},
	{"StructLayoutAttribute", StructLayoutAttribute},
	{"MarshalAsAttribute", MarshalAsAttribute},
	{"ComImportAttribute", ComImportAttribute},
	{0, 0}
};
static AttrConvertInfo const compilerAttrs[] = {
	{"MethodImplAttribute",	MethodImplAttribute},
	{"IndexerNameAttribute", IndexerNameAttribute},
	{0, 0}
};
static AttrConvertInfo const compilerCSharpAttrs[] = {
	{"IndexerNameAttribute", IndexerNameAttribute},
	{0, 0}
};
static AttrConvertInfo const securityAttrs[] = {
	{"SecurityPermissionAttribute", SecurityPermissionAttribute},
	{0, 0}
};
static AttrConvertInfo const componentModelAttrs[] = {
	{"DefaultValueAttribute", DefaultValueAttribute},
	{0, 0}
};
static AttrConvertInfo const basicAttrs[] = {
	{"DefaultValueAttribute", DefaultValueAttribute},
	{0, 0}
};

/*
 * Convert an attribute into metadata information.
 * Returns zero if not a builtin attribute, -1 if
 * out of memory, and 1 if OK.
 */
static int ConvertAttribute(ILProgramItem *item, ILAttribute *attr)
{
	ILMethod *ctor;
	const char *name;
	const char *namespace;
	const AttrConvertInfo *info;
	const void *blob;
	ILUInt32 blobLen;
	ILSerializeReader *reader;
	int result;

	/* Get the name of the class and the constructor signature */
	ctor = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
	if(!ctor)
	{
		return 0;
	}
	name = ILClass_Name(_ILMethod_Owner(ctor));
	namespace = ILClass_Namespace(_ILMethod_Owner(ctor));
	if(!namespace)
	{
		return 0;
	}

	/* Determine which attribute we are dealing with */
	if(!strcmp(namespace, "System.Runtime.InteropServices"))
	{
		info = interopAttrs;
	}
	else if(!strcmp(namespace, "System.Runtime.CompilerServices"))
	{
		info = compilerAttrs;
	}
	else if(!strcmp(namespace, "System.Runtime.CompilerServices.CSharp"))
	{
		info = compilerCSharpAttrs;
	}
	else if(!strcmp(namespace, "System.Security.Permissions"))
	{
		info = securityAttrs;
	}
	else if(!strcmp(namespace, "System.ComponentModel"))
	{
		info = componentModelAttrs;
	}
	else if(!strcmp(namespace, "System"))
	{
		info = systemAttrs;
	}
	else
	{
		return 0;
	}
	while(info->name != 0 && strcmp(info->name, name) != 0)
	{
		++info;
	}
	if(!(info->name))
	{
		return 0;
	}

	/* Get the data blob from the attribute and create a reader for it */
	blob = ILAttributeGetValue(attr, &blobLen);
	if(!blob)
	{
		return 0;
	}
	reader = ILSerializeReaderInit(ctor, blob, blobLen);
	if(!reader)
	{
		return 0;
	}

	/* Convert the attribute into metadata structures */
	result = (*(info->func))(item, reader);
	ILSerializeReaderDestroy(reader);
	return result;
}

int ILProgramItemConvertAttrs(ILProgramItem *item)
{
	ILAttribute *attr = ILProgramItemNextAttribute(item, 0);
	int result;
	while(attr != 0)
	{
		if((result = ConvertAttribute(item, attr)) < 0)
		{
			return 0;
		}
		else if(result > 0)
		{
			attr = ILProgramItemRemoveAttribute(item, attr);
		}
		else
		{
			attr = ILProgramItemNextAttribute(item, attr);
		}
	}
	return 1;
}

/*
 * Deserialize a System.AttributeUsageAttribute.
 * Returns 1 on success and 0 on error.
 */
static int DeSerializeAttributeUsage(ILAttributeUsageAttribute *usage,
									 ILSerializeReader *reader)
{
	int type;
	int numExtra;
	const char *paramName;
	int paramNameLen;
	ILMember *member;

	/* Get the first parameter, which should be int32 */
	type = ILSerializeReaderGetParamType(reader);
	if(type != IL_META_SERIALTYPE_I4)
	{
		return 0;
	}
	usage->validOn = (ILUInt32)(ILSerializeReaderGetInt32(reader, type));
	/* There MUST not be a second ctor argument */
	if(ILSerializeReaderGetParamType(reader) != 0)
	{
		return 0;
	}
	numExtra = ILSerializeReaderGetNumExtra(reader);
	if(numExtra < 0)
	{
		return 0;
	}
	while(numExtra > 0)
	{
		type = ILSerializeReaderGetExtra(reader, &member, &paramName,
										 &paramNameLen);
		if(type == -1)
		{
			return 0;
		}
		if(IsParam("AllowMultiple", IL_META_SERIALTYPE_BOOLEAN))
		{
			usage->allowMultiple = (ILSerializeReaderGetInt32(reader, type) != 0);
		}
		else if(IsParam("Inherited", IL_META_SERIALTYPE_BOOLEAN))
		{
			usage->inherited = (ILSerializeReaderGetInt32(reader, type) != 0);
		}
		else
		{
			/* Unknown property */
			return 0;
		}

		--numExtra;
	}
	return 1;
}

/*
 * Search for cached attribute information for the class.
 * Returns 0 if there is no cached attributeClass information is found.
 */
static ILCustomAttribute *FindCachedCustomAttribute(ILClass *info,
													ILClass *attributeClass)
{

	ILClassExt *ext;
	
	ext = _ILClassExtFind(info, _IL_EXT_CIL_CUSTOMATTR);
	while(ext)
	{
		if(ext->kind == _IL_EXT_CIL_CUSTOMATTR)
		{
			if(ext->un.customAttribute.attributeClass == attributeClass)
			{
				return &(ext->un.customAttribute);
			}
		}
		ext = ext->next;
	}
	/*
	 *  If we get here no cached information for the custom attribute was
	 * found
	 */
	return 0;
}

/*
 * Look for a custom attribute attached to the class.
 * Returns 0 if an attribute of this kind is not attached to the class.
 */
static ILAttribute *FindCustomAttribute(ILClass *info,
										ILClass *attributeClass)
{
	ILAttribute *attr;
	ILProgramItem *item;

	item = _ILToProgramItem(info);
	attr = 0;
	while((attr = ILProgramItemNextAttribute(item, attr)) != 0)
	{
		ILMethod *ctor;

		ctor = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
		if(ctor)
		{
			ILClass *currentAttrClass;
			
			currentAttrClass = _ILMethod_Owner(ctor);
			currentAttrClass = ILClassResolve(currentAttrClass);
			if(currentAttrClass == attributeClass)
			{
				return attr;
			}
		}
		attr = ILProgramItemNextAttribute(item, attr);
	}
	/*
	 * If we get here the class as no attributeClass custopm attribute.
	 */
	return 0;
}

static ILAttributeUsageAttribute *GetAttributeUsage(ILClass *attribute)
{
	ILClass *attributeUsage;

	attributeUsage = ILFindCustomAttribute(attribute->programItem.image->context,
										   "AttributeUsageAttribute", "System", 0);

	if(attributeUsage)
	{
		ILCustomAttribute *customAttr;
		
		/* First look for a cached attribute */
		customAttr = FindCachedCustomAttribute(attribute, attributeUsage);
		if(customAttr)
		{
			return &(customAttr->un.attributeUsage);
		}
		else
		{
			/* Try it the hard way */
			ILClassExt *ext;
			ILAttribute *attr;
			ILAttributeUsageAttribute *usage;

			ext = _ILClassExtCreate(attribute, _IL_EXT_CIL_CUSTOMATTR);
			if(!ext)
			{
				/* Out of memory */
				return 0;
			}
			ext->un.customAttribute.attributeClass = attributeUsage;
			usage = &(ext->un.customAttribute.un.attributeUsage);
			/* Set the defaults */
			/* See ECMA Version 4 paragraph 10.6 */
#if IL_VERSION_MAJOR > 1
			usage->validOn = 0X7FFF;
#else
			usage->validOn = 0X3FFF;
#endif
			usage->allowMultiple = (ILBool)0;
			usage->inherited = (ILBool)1;
			attr = 0;
			while(attribute && !attr)
			{
				/*
				 * The attribute usage is inherited from the parent attribute
				 * class so look for it in the parents too.
				 */
				attr = FindCustomAttribute(attribute, attributeUsage);
				if(!attr)
				{
					attribute = ILClassGetUnderlyingParentClass(attribute);
				}
			}
			if(attr)
			{
				ILMethod *ctor;

				ctor = ILProgramItemToMethod(ILAttributeTypeAsItem(attr));
				if(ctor)
				{
					const void *blob;
					ILUInt32 blobLen;
					ILSerializeReader *reader;
					int result;

					/*
					 * Get the data blob from the attribute and create a
					 * reader for it
					 */
					blob = ILAttributeGetValue(attr, &blobLen);
					if(!blob)
					{
						return 0;
					}
					reader = ILSerializeReaderInit(ctor, blob, blobLen);
					if(!reader)
					{
						return 0;
					}
					result = DeSerializeAttributeUsage(usage, reader);
					ILSerializeReaderDestroy(reader);
					if(!result)
					{
						return 0;
					}
				}
				else
				{
					return 0;
				}
			}
			return usage;
		}
	}
	return 0;
}

ILClass *ILFindCustomAttribute(ILContext *context, const char *name,
							   const char *namespace,
							   ILAttributeUsageAttribute **usage)
{
	ILClass *attribute;

	attribute = ILClassLookupGlobal(context, name, namespace);
	if(attribute)
	{
		attribute = ILClassResolve(attribute);
		if(attribute && usage)
		{
			/* Get the attribute usage information for the attribute */
			*usage = GetAttributeUsage(attribute);
		}
		return attribute;
	}
	return 0;
}

ILAttributeUsageAttribute *ILClassGetAttributeUsage(ILClass *attribute)
{
	if(attribute)
	{
		return GetAttributeUsage(attribute);
	}
	return 0;
}

ILUInt32 ILAttributeUsageAttributeGetValidOn(ILAttributeUsageAttribute *usage)
{
	return usage->validOn;
}

ILBool ILAttributeUsageAttributeGetAllowMultiple(ILAttributeUsageAttribute *usage)
{
	return usage->allowMultiple;
}

ILBool ILAttributeUsageAttributeGetInherited(ILAttributeUsageAttribute *usage)
{
	return usage->inherited;
}

#ifdef	__cplusplus
};
#endif
