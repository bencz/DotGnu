/*
 * ser_parse.c - Parse serialized attribute values.
 *
 * Copyright (C) 2001, 2008, 2009  Southern Storm Software, Pty Ltd.
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

#include "il_serialize.h"
#include "program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Internal structure of a serialization reader.
 */
struct _tagILSerializeReader
{
	ILMetaDataRead		meta;
	ILClass			   *info;
	ILType			   *signature;
	ILUInt32			param;

};

ILSerializeReader *ILSerializeReaderInit(ILMethod *method,
									     const void *blob,
										 ILUInt32 len)
{
	ILSerializeReader *reader;

	/* Resolve the method to it final class, so that we can
	   obtain the necessary field and property information */
	method = (ILMethod *)ILMemberResolve((ILMember *)method);

	/* Initialize the reader */
	if((reader = (ILSerializeReader *)ILMalloc(sizeof(ILSerializeReader))) == 0)
	{
		return 0;
	}
	reader->meta.data = (const unsigned char *)blob;
	reader->meta.len = len;
	reader->meta.error = 0;
	reader->info = method->member.owner;
	reader->signature = method->member.signature;
	reader->param = 0;

	/* Check the blob header */
	if(reader->meta.len < 2 || IL_READ_UINT16(reader->meta.data) != 1)
	{
		return 0;
	}
	reader->meta.data += 2;
	reader->meta.len -= 2;

	/* Ready to go */
	return reader;
}

void ILSerializeReaderDestroy(ILSerializeReader *reader)
{
	ILFree(reader);
}

int ILSerializeGetType(ILType *type)
{
	ILClass *classInfo;
	int elemType;

	/* Resolve enumerated type references to get the underlying type */
	type = ILTypeGetEnumType(type);

	/* If the type is an unresolved reference value type, then
	   assume that it is an integer-based enumerated type.  This
	   is needed to parse attributes across assembly boundaries
	   when "IL_LOADFLAG_NO_RESOLVE" is set */
	if(ILType_IsValueType(type) &&
	   ILClassIsRef(ILClassResolve(ILType_ToValueType(type))))
	{
		return IL_META_SERIALTYPE_I4;
	}

	/* Determine how to serialize the value */
	if(ILType_IsPrimitive(type))
	{
		/* Determine the primitive serialization type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_BOOLEAN:	return IL_META_SERIALTYPE_BOOLEAN;
			case IL_META_ELEMTYPE_I1:		return IL_META_SERIALTYPE_I1;
			case IL_META_ELEMTYPE_U1:		return IL_META_SERIALTYPE_U1;
			case IL_META_ELEMTYPE_I2:		return IL_META_SERIALTYPE_I2;
			case IL_META_ELEMTYPE_U2:		return IL_META_SERIALTYPE_U2;
			case IL_META_ELEMTYPE_CHAR:		return IL_META_SERIALTYPE_CHAR;
			case IL_META_ELEMTYPE_I4:		return IL_META_SERIALTYPE_I4;
			case IL_META_ELEMTYPE_U4:		return IL_META_SERIALTYPE_U4;
			case IL_META_ELEMTYPE_I8:		return IL_META_SERIALTYPE_I8;
			case IL_META_ELEMTYPE_U8:		return IL_META_SERIALTYPE_U8;
			case IL_META_ELEMTYPE_R4:		return IL_META_SERIALTYPE_R4;
			case IL_META_ELEMTYPE_R8:		return IL_META_SERIALTYPE_R8;
			/* this one is for deserializing array elements of type string */
			case IL_META_ELEMTYPE_STRING:	return IL_META_SERIALTYPE_STRING;
			default:						break;
		}
	}
	else if(ILType_IsClass(type))
	{
		/* Check for "System.String" and "System.Type" */
		classInfo = ILType_ToClass(type);
		if(!strcmp(classInfo->className->name, "String"))
		{
			if(classInfo->className->namespace &&
			   !strcmp(classInfo->className->namespace, "System") &&
			   ILClassGetNestedParent(classInfo) == 0)
			{
				return IL_META_SERIALTYPE_STRING;
			}
		}
		else if(!strcmp(classInfo->className->name, "Type"))
		{
			if(classInfo->className->namespace &&
			   !strcmp(classInfo->className->namespace, "System") &&
			   ILClassGetNestedParent(classInfo) == 0)
			{
				return IL_META_SERIALTYPE_TYPE;
			}
		}
		else if(!strcmp(classInfo->className->name, "Object"))
		{
			if(classInfo->className->namespace &&
			   !strcmp(classInfo->className->namespace, "System") &&
			   ILClassGetNestedParent(classInfo) == 0)
			{
				return IL_META_SERIALTYPE_VARIANT;
			}
		}
	}
	else if(ILType_IsSimpleArray(type))
	{
		/* Determine if this is an array of simple types */
		elemType = ILSerializeGetType(ILTypeGetElemType(type));
		if(elemType != -1 && (elemType & IL_META_SERIALTYPE_ARRAYOF) == 0)
		{
			return (IL_META_SERIALTYPE_ARRAYOF | elemType);
		}
	}

	/* The type is not serializable */
	return -1;
}

/*
 * Determine if there is sufficient data for a serialized value.
 */
static int HasSufficientSpace(ILSerializeReader *reader, int type)
{
	ILInt32 length;

	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		{
			return (reader->meta.len >= 1);
		}
		/* Not reached */

		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		{
			return (reader->meta.len >= 2);
		}
		/* Not reached */

		case IL_META_SERIALTYPE_I4:
		case IL_META_SERIALTYPE_U4:
		case IL_META_SERIALTYPE_R4:
		{
			return (reader->meta.len >= 4);
		}
		/* Not reached */

		case IL_META_SERIALTYPE_I8:
		case IL_META_SERIALTYPE_U8:
		case IL_META_SERIALTYPE_R8:
		{
			return (reader->meta.len >= 8);
		}
		/* Not reached */

		case IL_META_SERIALTYPE_STRING:
		case IL_META_SERIALTYPE_TYPE:
		case IL_META_SERIALTYPE_VARIANT:
		{
			/* Assume that space is sufficient, and check for real later */
			return 1;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_U1:
		{
			if(reader->meta.len < 4)
			{
				return 0;
			}
			length = IL_READ_INT32(reader->meta.data);
			if(length < 0 ||
			   ((ILUInt32)length) > (reader->meta.len - 4))
			{
				return 0;
			}
			return 1;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_CHAR:
		{
			if(reader->meta.len < 4)
			{
				return 0;
			}
			length = IL_READ_INT32(reader->meta.data);
			if(length < 0 ||
			   ((ILUInt32)length) > ((reader->meta.len - 4) / 2))
			{
				return 0;
			}
			return 1;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_I4:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_U4:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_R4:
		{
			if(reader->meta.len < 4)
			{
				return 0;
			}
			length = IL_READ_INT32(reader->meta.data);
			if(length < 0 ||
			   ((ILUInt32)length) > ((reader->meta.len - 4) / 4))
			{
				return 0;
			}
			return 1;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_I8:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_U8:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_R8:
		{
			if(reader->meta.len < 4)
			{
				return 0;
			}
			length = IL_READ_INT32(reader->meta.data);
			if(length < 0 ||
			   ((ILUInt32)length) > ((reader->meta.len - 4) / 8))
			{
				return 0;
			}
			return 1;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_STRING:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_TYPE:
		case IL_META_SERIALTYPE_ARRAYOF | IL_META_SERIALTYPE_VARIANT:
		{
			/* Assume that space is sufficient if we have a positive
			   length value, and check for real later */
			if(reader->meta.len < 4)
			{
				return 0;
			}
			length = IL_READ_INT32(reader->meta.data);
			return (length >= 0);
		}
		/* Not reached */
	}

	return 0;
}

int ILSerializeReaderGetParamType(ILSerializeReader *reader)
{
	int type;
	if(reader->param < ILTypeNumParams(reader->signature))
	{
		++(reader->param);
		type = ILSerializeGetType(ILTypeGetParam(reader->signature,
												 reader->param));
		if(HasSufficientSpace(reader, type))
		{
			return type;
		}
		else
		{
			return -1;
		}
	}
	else
	{
		return 0;
	}
}

ILInt32 ILSerializeReaderGetInt32(ILSerializeReader *reader, int type)
{
	ILInt32 value;
	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_I1:
		{
			value = (ILInt32)(*((ILInt8 *)(reader->meta.data)));
			++(reader->meta.data);
			--(reader->meta.len);
			return value;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_U1:
		{
			value = (ILInt32)(*((ILUInt8 *)(reader->meta.data)));
			++(reader->meta.data);
			--(reader->meta.len);
			return value;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_I2:
		{
			value = (ILInt32)(IL_READ_INT16(reader->meta.data));
			reader->meta.data += 2;
			reader->meta.len -= 2;
			return value;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		{
			value = (ILInt32)(IL_READ_UINT16(reader->meta.data));
			reader->meta.data += 2;
			reader->meta.len -= 2;
			return value;
		}
		/* Not reached */

		default: break;
	}
	value = IL_READ_INT32(reader->meta.data);
	reader->meta.data += 4;
	reader->meta.len -= 4;
	return value;
}

ILUInt32 ILSerializeReaderGetUInt32(ILSerializeReader *reader, int type)
{
	ILInt32 value;
	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_I1:
		{
			value = (ILUInt32)(ILInt32)(*((ILInt8 *)(reader->meta.data)));
			++(reader->meta.data);
			--(reader->meta.len);
			return value;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_U1:
		{
			value = (ILUInt32)(*((ILUInt8 *)(reader->meta.data)));
			++(reader->meta.data);
			--(reader->meta.len);
			return value;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_I2:
		{
			value = (ILUInt32)(ILInt32)(IL_READ_INT16(reader->meta.data));
			reader->meta.data += 2;
			reader->meta.len -= 2;
			return value;
		}
		/* Not reached */

		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		{
			value = (ILUInt32)(IL_READ_UINT16(reader->meta.data));
			reader->meta.data += 2;
			reader->meta.len -= 2;
			return value;
		}
		/* Not reached */

		default: break;
	}
	value = IL_READ_UINT32(reader->meta.data);
	reader->meta.data += 4;
	reader->meta.len -= 4;
	return value;
}

ILInt64 ILSerializeReaderGetInt64(ILSerializeReader *reader)
{
	ILInt64 value = IL_READ_INT64(reader->meta.data);
	reader->meta.data += 8;
	reader->meta.len -= 8;
	return value;
}

ILUInt64 ILSerializeReaderGetUInt64(ILSerializeReader *reader)
{
	ILUInt64 value = IL_READ_UINT64(reader->meta.data);
	reader->meta.data += 8;
	reader->meta.len -= 8;
	return value;
}

ILFloat ILSerializeReaderGetFloat32(ILSerializeReader *reader)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	ILFloat value = IL_READ_FLOAT(reader->meta.data);
	reader->meta.data += 4;
	reader->meta.len -= 4;
	return value;
#else
	return (ILFloat)(0.0);
#endif
}

ILDouble ILSerializeReaderGetFloat64(ILSerializeReader *reader)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	ILDouble value = IL_READ_DOUBLE(reader->meta.data);
	reader->meta.data += 8;
	reader->meta.len -= 8;
	return value;
#else
	return (ILDouble)(0.0);
#endif
}

int ILSerializeReaderGetString(ILSerializeReader *reader, const char **str)
{
	ILUInt32 length;
	if(reader->meta.len > 0 && reader->meta.data[0] == (unsigned char)0xFF)
	{
		/* Encoding of the null string */
		++(reader->meta.data);
		--(reader->meta.len);
		*str = 0;
		return 0;
	}
	length = ILMetaUncompressData(&(reader->meta));
	if(reader->meta.error || length > reader->meta.len)
	{
		return -1;
	}
	*str = (const char *)(reader->meta.data);
	reader->meta.data += length;
	reader->meta.len -= length;
	return (int)length;
}

ILInt32 ILSerializeReaderGetArrayLen(ILSerializeReader *reader)
{
	ILInt32 value = IL_READ_INT32(reader->meta.data);
	reader->meta.data += 4;
	reader->meta.len -= 4;
	return value;
}

int ILSerializeReaderGetNumExtra(ILSerializeReader *reader)
{
	int value;
	if(reader->meta.len < 2)
	{
		return -1;
	}
	value = (int)(IL_READ_UINT16(reader->meta.data));
	reader->meta.data += 2;
	reader->meta.len -= 2;
	return value;
}

int ILSerializeReaderGetExtra(ILSerializeReader *reader,
							  ILMember **memberReturn,
							  const char **nameReturn,
							  int *nameLenReturn)
{
	int type, extraType;
	ILClass *info;
	ILMember *member;
	const char *name;
	int nameLen;
	ILMethod *setter;
	ILType *memberType;

	/* Get the type of extra data (field or property) */
	if(reader->meta.len < 1)
	{
		return -1;
	}
	extraType = (int)(*(reader->meta.data));
	reader->meta.data += 1;
	reader->meta.len -= 1;
	if(extraType != IL_META_SERIALTYPE_FIELD &&
	   extraType != IL_META_SERIALTYPE_PROPERTY)
	{
		return -1;
	}

	/* Read the serialization type from the blob */
	if(reader->meta.len < 1)
	{
		return -1;
	}
	type = ((int)(*(reader->meta.data)));
	switch(type)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		case IL_META_SERIALTYPE_I4:
		case IL_META_SERIALTYPE_U4:
		case IL_META_SERIALTYPE_I8:
		case IL_META_SERIALTYPE_U8:
		case IL_META_SERIALTYPE_R4:
		case IL_META_SERIALTYPE_R8:
		case IL_META_SERIALTYPE_STRING:
		case IL_META_SERIALTYPE_TYPE:		break;

		/* The blob specified an invalid serialization type */
		default: return -1;
	}
	reader->meta.data += 1;
	reader->meta.len -= 1;

	/* Get the member's name */
	nameLen = ILSerializeReaderGetString(reader, &name);
	if(nameLen == -1 || !name)
	{
		return -1;
	}

	/* Search for the field or property within the class and its ancestors */
	if(extraType == IL_META_SERIALTYPE_FIELD)
	{
		extraType = IL_META_MEMBERKIND_FIELD;
	}
	else
	{
		extraType = IL_META_MEMBERKIND_PROPERTY;
	}
	info = ILClassResolve(reader->info);
	member = 0;
	while(info != 0)
	{
		while((member = ILClassNextMemberByKind(info, member, extraType)) != 0)
		{
			if(!strncmp(member->name, name, nameLen) &&
			   member->name[nameLen] == '\0')
			{
				break;
			}
		}
		if(member != 0)
		{
			break;
		}
		/* TODO */
		info = ILClass_ParentClass(info);
	}

	/* The member must have public access and be an instance member.
	   If it is a property, then it must also have a setter.  If we
	   did not find a member, then believe the data as to the type */
	if(member != 0)
	{
		if(extraType == IL_META_MEMBERKIND_FIELD)
		{
			if((member->attributes & IL_META_FIELDDEF_FIELD_ACCESS_MASK)
					!= IL_META_FIELDDEF_PUBLIC ||
			   (member->attributes & IL_META_FIELDDEF_STATIC) != 0)
			{
				return -1;
			}
			memberType = member->signature;
		}
		else
		{
			setter = ILPropertyGetSetter((ILProperty *)member);
			if(!setter)
			{
				return -1;
			}
			if((setter->member.attributes &
							IL_META_METHODDEF_MEMBER_ACCESS_MASK)
					!= IL_META_METHODDEF_PUBLIC ||
			   (setter->member.attributes & IL_META_METHODDEF_STATIC) != 0)
			{
				return -1;
			}
			memberType = ILTypeGetReturn(member->signature);
		}

		/* Convert the member type into a serialization type */
		extraType = ILSerializeGetType(memberType);
		if(extraType == -1 || (extraType & IL_META_SERIALTYPE_ARRAYOF) != 0)
		{
			return -1;
		}

		/* Validate the specified type against the actual type */
		if(type != extraType)
		{
			return -1;
		}
	}

	/* Check that we have sufficient space in the blob for the value */
	if(!HasSufficientSpace(reader, type))
	{
		return -1;
	}

	/* Return the member and type details to the caller */
	*memberReturn = member;
	*nameReturn = name;
	*nameLenReturn = nameLen;
	return type;
}

int ILSerializeReaderGetBoxedPrefix(ILSerializeReader *reader)
{
	int boxedType;
	if(reader->meta.len < 1)
	{
		return -1;
	}
	boxedType = (int)(*(reader->meta.data));
	reader->meta.data += 1;
	reader->meta.len -= 1;
	return boxedType;
}

#ifdef	__cplusplus
};
#endif
