/*
 * cg_genattr.c - Helper routines for custom attributes.
 *
 * Copyright (C) 2009  Free Software Foundation, Inc.
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

#include "cg_genattr.h"
#include "cg_errors.h"
#include "cg_coerce.h"
#include "cg_intl.h"
#include "il_program.h"
#include "il_serialize.h"

#ifdef	__cplusplus
extern	"C" {
#endif

struct _tagCGPermissionSets
{
	ILProgramItem			   *owner;
	CGSecurityAttributeInfo	   *permissionSet[9];	/* For the values 2 ... 10 */
	CGPermissionSets		   *next;				/* Next entry for am other owner in the list */
};

typedef struct _tagCGAttrConvertInfo CGAttrConvertInfo;
struct _tagCGAttrConvertInfo
{
	const char *name;
	int (*func)(ILGenInfo *info, CGAttributeInfo *attributeInfo);

};

void CGAttributeInfosInit(ILGenInfo *info,
						  CGAttributeInfos *attributeInfos)
{
	if(attributeInfos == 0)
	{
		return;
	}
	ILMemStackInit(&(attributeInfos->memstack), 0);
	attributeInfos->info = info;
	attributeInfos->attributes = 0;
	attributeInfos->permissionSets = 0;
}

void CGAttributeInfosDestroy(CGAttributeInfos *attributeInfos)
{
	if(attributeInfos == 0)
	{
		return;
	}
	ILMemStackDestroy(&(attributeInfos->memstack));
}

CGAttrCtorArg *CGAttrCtorArgAlloc(CGAttributeInfos *attributeInfos,
								  ILUInt32 numArgs)
{
	CGAttrCtorArg *ctorArg;


	if(attributeInfos == 0)
	{
		return 0;
	}
	if(numArgs == 0)
	{
		return 0;
	}
	ctorArg = (CGAttrCtorArg *)ILMemStackAllocItem
						(&(attributeInfos->memstack),
						 numArgs * sizeof(CGAttrCtorArg));
	if(ctorArg == 0)
	{
		ILGenOutOfMemory(attributeInfos->info);
	}
	return ctorArg;
}

CGAttrNamedArg *CGAttrNamedArgAlloc(CGAttributeInfos *attributeInfos,
									ILUInt32 numNamed)
{
	CGAttrNamedArg *namedArg;

	if(attributeInfos == 0)
	{
		return 0;
	}
	if(numNamed == 0)
	{
		return 0;
	}
	namedArg = (CGAttrNamedArg *)ILMemStackAllocItem
						(&(attributeInfos->memstack),
						 numNamed * sizeof(CGAttrNamedArg));
	if(namedArg == 0)
	{
		ILGenOutOfMemory(attributeInfos->info);
	}
	return namedArg;
}

static CGPermissionSets *CGPermissionSetsAlloc(CGAttributeInfos *attributeInfos,
											   ILProgramItem *owner)
{
	CGPermissionSets *permissionSets;

	if(attributeInfos == 0)
	{
		return 0;
	}
	if(owner == 0)
	{
		return 0;
	}
	/* Look for a permissionset for that owner */
	permissionSets = attributeInfos->permissionSets;
	while(permissionSets != 0)
	{
		if(permissionSets->owner == owner)
		{
			return permissionSets;
		}
		permissionSets = permissionSets->next;
	}
	/* We need a new one */
	permissionSets = ILMemStackAlloc(&(attributeInfos->memstack),
									 CGPermissionSets);
	if(permissionSets == 0)
	{
		ILGenOutOfMemory(attributeInfos->info);
		return 0;
	}
	permissionSets->owner = owner;
	permissionSets->next = attributeInfos->permissionSets;
	attributeInfos->permissionSets = permissionSets;
	return permissionSets;
}	

/*
 * Get the target name for an attribute target.
 */
static const char *AttributeTargetToName(ILUInt32 target)
{
	switch(target)
	{
		case IL_ATTRIBUTE_TARGET_ASSEMBLY:
		{
			return "assemblies";
		}
		break;

		case IL_ATTRIBUTE_TARGET_MODULE:
		{
			return "modules";
		}
		break;

		case IL_ATTRIBUTE_TARGET_CLASS:
		{
			return "classes";
		}
		break;

		case IL_ATTRIBUTE_TARGET_STRUCT:
		{
			return "structs";
		}
		break;

		case IL_ATTRIBUTE_TARGET_ENUM:
		{
			return "enumerations";
		}
		break;

		case IL_ATTRIBUTE_TARGET_CONSTRUCTOR:
		{
			return "constructors";
		}
		break;

		case IL_ATTRIBUTE_TARGET_METHOD:
		{
			return "methods";
		}
		break;

		case IL_ATTRIBUTE_TARGET_PROPERTY:
		{
			return "properties";
		}
		break;

		case IL_ATTRIBUTE_TARGET_FIELD:
		{
			return "fields";
		}
		break;

		case IL_ATTRIBUTE_TARGET_EVENT:
		{
			return "events";
		}
		break;

		case IL_ATTRIBUTE_TARGET_INTERFACE:
		{
			return "interfaces";
		}
		break;

		case IL_ATTRIBUTE_TARGET_PARAMETER:
		{
			return "parameters";
		}
		break;

		case IL_ATTRIBUTE_TARGET_DELEGATE:
		{
			return "delegates";
		}
		break;

		case IL_ATTRIBUTE_TARGET_RETURNVALUE:
		{
			return "returnvalues";
		}
		break;

#if IL_VERSION_MAJOR > 1
		case IL_ATTRIBUTE_TARGET_GENERICPAR:
		{
			return "generic parameters";
		}
#endif
	}
	return "Unknown";
}

static const char *SecurityActionToName(ILUInt32 action)
{
	switch(action)
	{
		case IL_META_SECURITY_DEMAND:
		{
			return "Demand";
		}
		break;

		case IL_META_SECURITY_ASSERT:
		{
			return "Assert";
		}
		break;

		case IL_META_SECURITY_DENY:
		{
			return "Deny";
		}
		break;

		case IL_META_SECURITY_PERMIT_ONLY:
		{
			return "PermitOnly";
		}
		break;

		case IL_META_SECURITY_LINK_TIME_CHECK:
		{
			return "LinkDemand";
		}
		break;

		case IL_META_SECURITY_INHERITANCE_CHECK:
		{
			return "InheritanceDemand";
		}
		break;

		case IL_META_SECURITY_REQUEST_MINIMUM:
		{
			return "RequestMinimum";
		}
		break;

		case IL_META_SECURITY_REQUEST_OPTIONAL:
		{
			return "RequestOptional";
		}
		break;

		case IL_META_SECURITY_REQUEST_REFUSE:
		{
			return "RequestRefuse";
		}
		break;
	}
	return "Unknown";
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
 * Convert a class into a name used by error messages.
 */
static const char *CGClassToName(ILClass *classInfo)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	const char *finalName;
	if(namespace)
	{
		finalName = ILInternStringConcat3
						(ILInternString(namespace, -1),
						 ILInternString(".", 1),
						 ILInternString(name, -1)).string;
	}
	else
	{
		finalName = name;
	}
	if(ILClass_NestedParent(classInfo) != 0)
	{
		/* Prepend the name of the enclosing nesting class */
		const char *parentName;
		parentName = CGClassToName(ILClass_NestedParent(classInfo));
		finalName = ILInternStringConcat3
						(ILInternString(parentName, -1),
						 ILInternString(".", 1),
						 ILInternString(finalName, -1)).string;
	}
	return finalName;
}

/*
 * Convert a type into a name used by error messages.
 */
static const char *CGTypeToName(ILGenInfo *info, ILType *type)
{
	ILClass *classInfo;

	classInfo = ILTypeToClass(info, type);
	return CGClassToName(classInfo);
}

/*
 * Convert a class into a name, formatted for use in attribute values.
 */
static const char *CGClassToAttrName(ILGenInfo *info, ILClass *classInfo)
{
	const char *name = ILClass_Name(classInfo);
	const char *namespace = ILClass_Namespace(classInfo);
	const char *finalName;
	if(namespace)
	{
		finalName = ILInternStringConcat3
						(ILInternString(namespace, -1),
						 ILInternString(".", 1),
						 ILInternString(name, -1)).string;
	}
	else
	{
		finalName = name;
	}
	if(ILClass_NestedParent(classInfo) != 0)
	{
		/* Prepend the name of the enclosing nesting class */
		const char *parentName = CGClassToAttrName
			(info, ILClass_NestedParent(classInfo));
		finalName = ILInternStringConcat3
						(ILInternString(parentName, -1),
						 ILInternString("+", 1),
						 ILInternString(finalName, -1)).string;
	}
	return finalName;
}

/*
 * Convert a type into a name, formatted for use in attribute values.
 */
static const char *CGTypeToAttrName(ILGenInfo *info, ILType *type)
{
	ILClass *classInfo = ILTypeToClass(info, type);

	return CGClassToAttrName(info, classInfo);
}

/* 
 * write a fixed ctor argument entry into the serialized stream using the
 * provided paramType, argValue and serialType.
 */
static void WriteFixedArgument(ILGenInfo *info,
							   ILSerializeWriter *writer, 
							   ILType *paramType,
							   ILEvalValue *argValue,
							   ILType *argType,
							   int serialType)
{	
	ILType *systemType=ILFindSystemType(info,"Type");

	switch(serialType)
	{
		case IL_META_SERIALTYPE_BOOLEAN:
		case IL_META_SERIALTYPE_I1:
		case IL_META_SERIALTYPE_U1:
		case IL_META_SERIALTYPE_I2:
		case IL_META_SERIALTYPE_U2:
		case IL_META_SERIALTYPE_CHAR:
		case IL_META_SERIALTYPE_I4:
		case IL_META_SERIALTYPE_U4:
		{
			ILSerializeWriterSetInt32(writer, argValue->un.i4Value,
									  serialType);
		}
		break;

		case IL_META_SERIALTYPE_I8:
		case IL_META_SERIALTYPE_U8:
		{
			ILSerializeWriterSetInt64(writer, argValue->un.i8Value);
		}
		break;

		case IL_META_SERIALTYPE_R4:
		{
			ILSerializeWriterSetFloat32(writer, argValue->un.r4Value);
		}
		break;

		case IL_META_SERIALTYPE_R8:
		{
			ILSerializeWriterSetFloat64(writer, argValue->un.r8Value);
		}
		break;

		case IL_META_SERIALTYPE_STRING:
		{
			if(argValue->valueType == ILMachineType_String)
			{
				ILSerializeWriterSetString(writer, argValue->un.strValue.str,
										   argValue->un.strValue.len);
			}
			else
			{
				ILSerializeWriterSetString(writer, 0, 0);
			}
		}
		break;

		case IL_META_SERIALTYPE_TYPE:
		{
			const char *name = CGTypeToAttrName
				(info, (ILType *)(argValue->un.strValue.str));
			ILSerializeWriterSetString(writer, name, strlen(name));
		}
		break;

		case IL_META_SERIALTYPE_VARIANT:
		{
			/* Note : We assume the values are castable and
			 * do not provide any checks here */
			if(ILType_IsPrimitive(argType))
			{
				switch(argValue->valueType)
				{	
					case ILMachineType_Boolean:
					case ILMachineType_Int8:
					case ILMachineType_UInt8:
					case ILMachineType_Int16:
					case ILMachineType_UInt16:
					case ILMachineType_Char:
					case ILMachineType_Int32:
					case ILMachineType_UInt32:
					case ILMachineType_Int64:
					case ILMachineType_UInt64:
					case ILMachineType_Float32:
					case ILMachineType_Float64:
					case ILMachineType_Decimal:
					{
						serialType=ILSerializeGetType(argType);
						
						ILSerializeWriterSetBoxedPrefix(writer, 
														   serialType);

						WriteFixedArgument(info, writer, paramType, 
											 argValue, argType, serialType);
					}
					break;

					case ILMachineType_String:
					{
						/* TODO */
					}
					break;

					default:
					{
					}
					break;
				}
			}
			else if(ILTypeIdentical(argType, systemType))
			{ 
				ILSerializeWriterSetBoxedPrefix(writer,
										IL_META_SERIALTYPE_TYPE);

				WriteFixedArgument(info, writer, paramType, 
									 argValue, argType,
									 IL_META_SERIALTYPE_TYPE);
			}
			else if(ILTypeIsEnum(argType))
			{
				const char *name = CGTypeToAttrName(info, (ILType *)(argType));
				ILSerializeWriterSetBoxedPrefix(writer,
										IL_META_SERIALTYPE_ENUM);
				ILSerializeWriterSetString(writer, name, strlen(name));

				serialType=ILSerializeGetType(argType);

				WriteFixedArgument(info, writer, paramType,
										argValue, argType,
										serialType);
			}
		}
		break;

		default:
		{
			if(ILType_IsArray(paramType))
			{
				/* TODO: arrays */
			}
		}
		break;
	}
}

/*
 * Write fixed attribute ctor arguments into the serialized stream.
 */
static int WriteFixedAttributeArguments(ILGenInfo *info,
										ILSerializeWriter *writer,
										ILType *ctorSignature,
										CGAttrCtorArg *ctorArgs,
										ILUInt32 numArgs)
{
	ILUInt32 argNum;

	for(argNum = 0; argNum < numArgs; ++argNum)
	{
		ILType *argType;
		ILType *paramType;
		ILEvalValue *argValue;
		int serialType;

		paramType = ILTypeGetParam(ctorSignature, argNum + 1);
		argValue = &(ctorArgs[argNum].evalValue);
		argType = ctorArgs[argNum].type;
		serialType = ILSerializeGetType(paramType);
		WriteFixedArgument(info, writer, paramType, argValue, argType,
							 serialType);
	}
	return 1;
}

/* 
 * write a named argument entry into the serialized stream.
 */
static void WriteNamedArgument(ILGenInfo *info,
							   ILSerializeWriter *writer,
							   CGAttrNamedArg *namedArg)
{	
	ILType *argType;
	ILType *paramType;
	ILEvalValue *argValue;
	int serialType;

	argType = namedArg->type;
	if(ILType_IsArray(argType))
	{
		/* TODO: arrays */
		return;
	}
	else if(ILTypeIsEnum(argType))
	{
		const char *name = CGTypeToAttrName(info, argType);
		const char *memberName = ILMember_Name(namedArg->member);

		if(ILMember_IsField(namedArg->member))
		{
			ILSerializeWriterSetInt32(writer, IL_META_SERIALTYPE_FIELD,
									  IL_META_SERIALTYPE_U1);
		}
		else
		{
			ILSerializeWriterSetInt32(writer, IL_META_SERIALTYPE_PROPERTY,
									  IL_META_SERIALTYPE_U1);
		}
		ILSerializeWriterSetInt32(writer, IL_META_SERIALTYPE_ENUM,
								  IL_META_SERIALTYPE_U1);
		ILSerializeWriterSetString(writer, name, strlen(name));
		ILSerializeWriterSetString(writer, memberName, strlen(memberName));
		argType = ILTypeGetEnumType(argType);
		argValue = &(namedArg->evalValue);
		if(argValue->valueType == ILMachineType_Void)
		{
			serialType = IL_META_SERIALTYPE_TYPE;
			paramType = NULL;
		}
		else
		{
			paramType = ILValueTypeToType(info, argValue->valueType);
			serialType = ILSerializeGetType(paramType);
		}
	}
	else
	{
		argValue = &(namedArg->evalValue);
		if(argValue->valueType == ILMachineType_Void)
		{
			serialType = IL_META_SERIALTYPE_TYPE;
			paramType = NULL;
		}
		else
		{
			paramType = ILValueTypeToType(info, argValue->valueType);
			serialType = ILSerializeGetType(paramType);
		}
		if(ILMember_IsField(namedArg->member))
		{
			ILSerializeWriterSetField
				(writer, ILMember_Name(namedArg->member), serialType);
		}
		else
		{
			ILSerializeWriterSetProperty
				(writer, ILMember_Name(namedArg->member), serialType);
		}
	}
	WriteFixedArgument(info, writer, paramType, argValue, argType, serialType);
}

/*
 * Write named attribute arguments.
 */
static int WriteNamedAttributeArguments(ILGenInfo *info,
										ILSerializeWriter *writer,
										CGAttrNamedArg *namedArgs,
										ILUInt32 numNamed)
{
	ILUInt32 argNum;

	for(argNum = 0; argNum < numNamed; ++argNum)
	{
		WriteNamedArgument(info, writer, &(namedArgs[argNum]));
	}
	return 1;
}

/*
 * Write a custom attribute.
 */
static int WriteCustomAttribute(ILGenInfo *info,
								CGAttributeInfo *attributeInfo)
{
	int argNum;
	ILType *signature;
	ILType *paramType;
	ILAttribute *attribute;
	ILMethod *methodInfo;
	const void *blob;
	ILUInt32 blobLen;
	int haveErrors = 0;
	ILSerializeWriter *writer = 0;

	/* Import the constructor method into this image */
	methodInfo = (ILMethod *)ILMemberImport(info->image,
											(ILMember *)attributeInfo->ctor);
	if(!methodInfo)
	{
		ILGenOutOfMemory(info);
	}

	/* Check if all types can be serialized */
	signature = ILMethod_Signature(methodInfo);
	for(argNum = 0; argNum < attributeInfo->numArgs; ++argNum)
	{
		if(attributeInfo->ctorArgs[argNum].evalValue.valueType != ILMachineType_Void)
		{
			paramType = ILTypeGetParam(signature, argNum + 1);

			if(ILSerializeGetType(paramType) == -1)
			{
				CGErrorForNode(info, attributeInfo->ctorArgs[argNum].node,
							   _("attribute argument %d is not serializable"),
							   argNum + 1);
				haveErrors = 1;
			}
		}
	}

	for(argNum = 0; argNum < attributeInfo->numNamed; ++argNum)
	{
		if(attributeInfo->namedArgs[argNum].evalValue.valueType != ILMachineType_Void)
		{
			paramType = attributeInfo->namedArgs[argNum].type;

			if(ILSerializeGetType(paramType) == -1)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[argNum].node,
							  _("named attribute argument %d is not serializable"),
							  argNum + 1);
				haveErrors = 1;
			}
		}
	}

	if(haveErrors)
	{
		return 0;
	}

	/* Build the serialized attribute value */
	writer = ILSerializeWriterInit();
	if(!writer)
	{
		ILGenOutOfMemory(info);
	}
	/* Set the header for a custom attribute */
	ILSerializeWriterSetInt32(writer, 1, IL_META_SERIALTYPE_U2);

	if(!WriteFixedAttributeArguments(info, writer, signature,
									 attributeInfo->ctorArgs,
									 attributeInfo->numArgs))
	{
		ILSerializeWriterDestroy(writer);
		return 0;
	}
	ILSerializeWriterSetNumExtra(writer, attributeInfo->numNamed);
	if(!WriteNamedAttributeArguments(info, writer, attributeInfo->namedArgs,
									 attributeInfo->numNamed))
	{
		ILSerializeWriterDestroy(writer);
		return 0;
	}
	blob = ILSerializeWriterGetBlob(writer, &blobLen);
	if(!blob)
	{
		ILGenOutOfMemory(info);
	}

	/* Add the attribute value to the program item */
	attribute = ILAttributeCreate(info->image, 0);
	if(!attribute)
	{
		ILGenOutOfMemory(info);
	}
	ILAttributeSetType(attribute, ILToProgramItem(methodInfo));
	if(!ILAttributeSetValue(attribute, blob, blobLen))
	{
		ILGenOutOfMemory(info);
	}
	ILProgramItemAddAttribute(attributeInfo->owner, attribute);

	ILSerializeWriterDestroy(writer);

	return 1;
}

/*
 * Process a serializable attribute on a class, struct or enum.
 */
static int SerializableAttribute(ILGenInfo *info,
								 CGAttributeInfo *attributeInfo)
{
	ILClass *classInfo;

	if((classInfo = ILProgramItemToClass(attributeInfo->owner)) != 0)
	{
		ILClassSetAttrs(classInfo, IL_META_TYPEDEF_SERIALIZABLE,
								   IL_META_TYPEDEF_SERIALIZABLE);
		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The serializable attribute can be used only on types"));
	return 0;
}

/*
 * Process a non serialized attribute on a field.
 */
static int NonSerializedAttribute(ILGenInfo *info,
								  CGAttributeInfo *attributeInfo)
{
	ILField *field;

	if((field = ILProgramItemToField(attributeInfo->owner)) != 0)
	{
		ILMemberSetAttrs((ILMember *)field, IL_META_FIELDDEF_NOT_SERIALIZED,
											IL_META_FIELDDEF_NOT_SERIALIZED);
		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The non serialize attribute can be used only on fields"));
	return 0;
}

/*
 * Process an "in" attribute on a parameter.
 */
static int InAttribute(ILGenInfo *info,
					   CGAttributeInfo *attributeInfo)
{
	ILParameter *param;

	/* We must use this on a parameter */
	if((param = ILProgramItemToParameter(attributeInfo->owner)) != 0)
	{
		ILParameterSetAttrs(param, IL_META_PARAMDEF_IN,
								   IL_META_PARAMDEF_IN);
		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The in attribute can be used only on parameters"));
	return 0;
}

/*
 * Process an "out" attribute on a parameter.
 */
static int OutAttribute(ILGenInfo *info,
						CGAttributeInfo *attributeInfo)
{
	ILParameter *param;

	/* We must use this on a parameter */
	if((param = ILProgramItemToParameter(attributeInfo->owner)) != 0)
	{
		ILParameterSetAttrs(param, IL_META_PARAMDEF_OUT,
								   IL_META_PARAMDEF_OUT);
		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The out attribute can be used only on parameters"));
	return 0;
}

/*
 * Process an "optional" attribute on a parameter.
 */
static int OptionalAttribute(ILGenInfo *info,
							 CGAttributeInfo *attributeInfo)
{
	ILParameter *param;

	/* We must use this on a parameter */
	if((param = ILProgramItemToParameter(attributeInfo->owner)) != 0)
	{
		ILParameterSetAttrs(param, IL_META_PARAMDEF_OPTIONAL,
								   IL_META_PARAMDEF_OPTIONAL);
		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The optional attribute can be used only on parameters"));
	return 0;
}

/*
 * Process a "comimport" attribute on a type.
 */
static int ComImportAttribute(ILGenInfo *info,
							  CGAttributeInfo *attributeInfo)
{
	ILClass *classInfo;

	/* We must use this on a type */
	if((classInfo = ILProgramItemToClass(attributeInfo->owner)) != 0)
	{
		ILClassSetAttrs(classInfo, IL_META_TYPEDEF_IMPORT,
								   IL_META_TYPEDEF_IMPORT);
		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The com import attribute can be used only on types"));
	return 0;
}

/*
 * Process a "StructLayout" attribute on a type.
 */
static int StructLayoutAttribute(ILGenInfo *info,
								 CGAttributeInfo *attributeInfo)
{
	ILClass *classInfo;

	/* We must use this on a type */
	if((classInfo = ILProgramItemToClass(attributeInfo->owner)) != 0)
	{
		ILClassLayout *layout;
		ILUInt32 attrMask = 0;
		ILUInt32 attrs = 0;
		ILInt32 sizeValue = 0;
		ILInt32 packValue = 0;
		int currentNamedArg;

		if(attributeInfo->numArgs != 1)
		{
			CGErrorForNode(info, attributeInfo->node,
				_("incorrect number of arguments for the struct layout attribute"));
			return 0;
		}

		/* Convert the kind value into a layout attribute value */
		attrMask |= IL_META_TYPEDEF_LAYOUT_MASK;
		switch(attributeInfo->ctorArgs[0].evalValue.un.i4Value)
		{
			case 0:
			{
				attrs |= IL_META_TYPEDEF_LAYOUT_SEQUENTIAL;
			}
			break;

			case 2:
			{
				attrs |= IL_META_TYPEDEF_EXPLICIT_LAYOUT;
			}
			break;

			case 3:
			{
				attrs |= IL_META_TYPEDEF_AUTO_LAYOUT;
			}
			break;

			default:
			{
				CGErrorForNode(info, attributeInfo->ctorArgs[0].node,
					_("invalid structure layout"));
			}
			break;
		}

		for(currentNamedArg = 0; currentNamedArg < attributeInfo->numNamed; ++currentNamedArg)
		{
			ILField *field;
			ILProgramItem *item;
			const char *fieldName;

			item = ILToProgramItem(attributeInfo->namedArgs[currentNamedArg].member);
			if((field = ILProgramItemToField(item)) == 0)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("Invalid named field for the struct layout attribute"));
				continue;
			}
			fieldName = ILField_Name(field);
			if(!fieldName)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("Invalid named field for the struct layout attribute"));
				continue;
			}
			if(!strcmp(fieldName, "CharSet"))
			{
				attrMask |= IL_META_TYPEDEF_STRING_FORMAT_MASK;
				switch(attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value)
				{
					case 1:
					case 2:
					{
						attrs |= IL_META_TYPEDEF_ANSI_CLASS;
					}
					break;

					case 3:
					{
						attrs |= IL_META_TYPEDEF_UNICODE_CLASS;
					}
					break;

					case 4:
					{
						attrs |= IL_META_TYPEDEF_AUTO_CLASS;
					}
					break;

					default:
					{
						CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
							_("Invalid CharSet specified for the struct layout attribute"));
					}
					break;
				}
			}
			else if(!strcmp(fieldName, "Size"))
			{
				sizeValue = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
			}
			else if(!strcmp(fieldName, "Pack"))
			{
				packValue = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
				switch(packValue)
				{
					case 1:
					case 2:
					case 4:
					case 8:
					case 16:
					case 32:
					case 64:
					case 128:
					{
						/* OK */
					}
					break;

					default:
					{
						CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
							_("Invalid Pack specified for the struct layout attribute"));
					}
					break;
				}
			}
			else
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("Invalid named field for the struct layout attribute"));
				return 0;
			}
		}

		/* Change the attributes for the class */
		ILClassSetAttrs(classInfo, attrMask, attrs);

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
					ILGenOutOfMemory(info);
				}
			}
		}

		return 1;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The struct layout attribute can be used only on types"));
	return 0;
}

/*
 * Process a "FieldOffset" attribute on a field.
 */
static int FieldOffsetAttribute(ILGenInfo *info,
								CGAttributeInfo *attributeInfo)
{
	ILField *field;

	if((field = ILProgramItemToField(attributeInfo->owner)) != 0)
	{
		if(attributeInfo->numArgs == 1)
		{
			ILFieldLayout *layout;
			ILUInt32 offset;

			/* Set the field offset */
			offset = attributeInfo->ctorArgs[0].evalValue.un.i4Value;
			layout = ILFieldLayoutGetFromOwner(field);
			if(layout)
			{
				ILFieldLayoutSetOffset(layout, offset);
			}
			else if((layout = ILFieldLayoutCreate(ILProgramItem_Image(field), 0,
												  field, offset)) == 0)
			{
				ILGenOutOfMemory(info);
			}
			return 1;
		}
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of arguments for the field offset attribute"));
		return 0;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The field offset attribute can be used only on fields"));
	return 0;
}

/*
 * Process a "DiiImport" attribute on a method (or field see notes).
 */
static int DllImportAttribute(ILGenInfo *info,
							  CGAttributeInfo *attributeInfo)
{
	ILMethod *method = 0;
	ILField *field = 0;
	ILModule *module;
	ILUInt32 attrMask = 0;
	ILUInt32 attrs;
	const char *dllName = 0;
	const char *entryPoint = 0;
	int currentNamedArg;

	/* Set the default value for the attributes */
	attrs = IL_META_PINVOKE_CHAR_SET_ANSI | IL_META_PINVOKE_CALL_CONV_STDCALL;
	if((method = ILProgramItemToMethod(attributeInfo->owner)) != 0)
	{
		/* Nothing to do here */
	}
	else if((field = ILProgramItemToField(attributeInfo->owner)) != 0)
	{
		if(!ILField_IsStatic(field) || ILField_IsLiteral(field))
		{
			CGErrorForNode(info, attributeInfo->node,
				_("The dllimport attribute can be used only on static non constant fields"));
			return 0;
		}
	}
	else
	{
		CGErrorForNode(info, attributeInfo->node,
			_("The dllimport attribute can be used only on methods and fields"));
		return 0;
	}

	if(attributeInfo->numArgs != 1)
	{
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of parameters passed to the constructor"));
		return 0;
	}
	dllName = attributeInfo->ctorArgs[0].evalValue.un.strValue.str;
	if(!dllName || attributeInfo->ctorArgs[0].evalValue.un.strValue.len <= 0)
	{
		CGErrorForNode(info, attributeInfo->node,
			_("the name of the shared library must not be null or empty"));
		return 0;
	}

	for(currentNamedArg = 0; currentNamedArg < attributeInfo->numNamed; ++currentNamedArg)
	{
		ILProgramItem *item;
		ILField *attrField;
		const char *attrFieldName;

		item = ILToProgramItem(attributeInfo->namedArgs[currentNamedArg].member);
		if((attrField = ILProgramItemToField(item)) == 0)
		{
			CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
				_("Invalid named field for the dllimport attribute"));
			continue;
		}
		attrFieldName = ILField_Name(attrField);
		if(!attrFieldName)
		{
			CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
				_("Invalid named field for the dllimport attribute"));
			continue;
		}
		if(!strcmp("CallingConvention", attrFieldName))
		{
			ILUInt32 callConv;

			callConv = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
			if(callConv < 1 || callConv > 5)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("Invalid calling convention specified for the dllimport attribute"));
				continue;
			}
			attrMask |= IL_META_PINVOKE_CALL_CONV_MASK;
			attrs &= ~IL_META_PINVOKE_CALL_CONV_MASK;
			attrs |= (callConv << 8);
		}
		else if(!strcmp(attrFieldName, "CharSet"))
		{
			ILUInt32 charSet;
	
			attrMask |= IL_META_PINVOKE_CHAR_SET_MASK;
			attrs &= ~IL_META_PINVOKE_CHAR_SET_MASK;
			charSet = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
			switch(charSet)
			{
				case 1:
				{
					/* None */
				}
				break;

				case 2:
				{
					/* Ansi */
					attrs |= IL_META_PINVOKE_CHAR_SET_ANSI;
				}
				break;

				case 3:
				{
					/* Unicode */
					attrs |= IL_META_PINVOKE_CHAR_SET_UNICODE;
				}
				break;

				case 4:
				{
					/* Auto */
					attrs |= IL_META_PINVOKE_CHAR_SET_AUTO;
				}
				break;

				default:
				{
					CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
						_("Invalid character set specified for the dllimport attribute"));
					continue;
				}
			}
		}	
		else if(!strcmp(attrFieldName, "EntryPoint"))
		{
			entryPoint = attributeInfo->namedArgs[currentNamedArg].evalValue.un.strValue.str;

			if(!entryPoint ||
			   attributeInfo->namedArgs[currentNamedArg].evalValue.un.strValue.len <= 0)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("the entrypoint in of the shared library must not be null or empty"));
				return 0;
			}		
		}
		else if(!strcmp(attrFieldName, "ExactSpelling"))
		{
			attrMask |= IL_META_PINVOKE_NO_MANGLE;

			if(attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value)
			{
				attrs |= IL_META_PINVOKE_NO_MANGLE;
			}
			else
			{
				attrs &= ~IL_META_PINVOKE_NO_MANGLE;
			}
		}
		else if(!strcmp(attrFieldName, "PreserveSig"))
		{
			attrMask |= IL_META_PINVOKE_OLE;

			if(attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value)
			{
				attrs |= IL_META_PINVOKE_OLE;
			}
			else
			{
				attrs &= ~IL_META_PINVOKE_OLE;
			}
		}
		else if(!strcmp(attrFieldName, "SetLastError"))
		{
			attrMask |= IL_META_PINVOKE_SUPPORTS_LAST_ERROR;

			if(attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value)
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
			CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
				_("Invalid named field for the dllimport attribute"));
			return 0;
		}
	}

	module = ILModuleRefCreateUnique(ILProgramItem_Image(attributeInfo->owner), dllName);
	if(!module)
	{
		ILGenOutOfMemory(info);
	}
	if(method)
	{
		if(!ILPInvokeCreate(method, 0, attrs, module, entryPoint))
		{
			ILGenOutOfMemory(info);
		}
		/* Mark the method with the "pinvokeimpl" flag */
		ILMemberSetAttrs((ILMember *)method,
						 IL_META_METHODDEF_PINVOKE_IMPL,
						 IL_META_METHODDEF_PINVOKE_IMPL);
	}
	else
	{
		if(!ILPInvokeFieldCreate(field, 0, attrs, module, entryPoint))
		{
			ILGenOutOfMemory(info);
		}
		/* Mark the field with the "pinvokeimpl" flag */
		ILMemberSetAttrs((ILMember *)field,
						 IL_META_FIELDDEF_PINVOKE_IMPL,
						 IL_META_FIELDDEF_PINVOKE_IMPL);
	}
	return 1;
}

/*
 * Add the marshalling information to the owner.
 */
static int AddMarshalling(ILGenInfo *info, ILProgramItem *owner,
						  const unsigned char *blob, ILUInt32 blobLen)
{
	ILFieldMarshal *marshal;

	marshal = ILFieldMarshalGetFromOwner(owner);
	if(!marshal)
	{
		marshal = ILFieldMarshalCreate(ILProgramItem_Image(owner), 0, owner);
		if(!marshal)
		{
			ILGenOutOfMemory(info);
			return 0;
		}
	}
	if(!ILFieldMarshalSetType(marshal, blob, blobLen))
	{
		ILGenOutOfMemory(info);
		return 0;
	}

	return 1;
}

/*
 * Process a marshal attribute on a field or parameter.
 */
static int MarshalAsAttribute(ILGenInfo *info,
							  CGAttributeInfo *attributeInfo)
{
	ILField *field = 0;
	ILParameter *param = 0;
	ILUInt32 unmanagedType;
	ILUInt32 arraySubType = IL_META_NATIVETYPE_MAX;
	ILUInt32 safeArraySubType = IL_META_VARIANTTYPE_EMPTY;
	const char *marshalCookie = 0;
	int marshalCookieLen = 0;
	int currentNamedArg;
	ILInt32 sizeValue = 0;
	ILInt32 sizeParamIndex = -1;
	const char *marshalType = 0;
	int marshalTypeLen = 0;

	if((field = ILProgramItemToField(attributeInfo->owner)) != 0)
	{
		/* Nothing to do here */
	}
	else if((param = ILProgramItemToParameter(attributeInfo->owner)) != 0)
	{
		/* Nothing to do here */
	}
	else
	{
		CGErrorForNode(info, attributeInfo->node,
			_("The marshal as attribute can be used only on fields and parameters"));
		return 0;
	}

	if(attributeInfo->numArgs != 1)
	{
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of parameters passed to the constructor"));
		return 0;
	}

	unmanagedType = attributeInfo->ctorArgs[0].evalValue.un.i4Value;
	/* vaidate the unmanaged type */
	switch(unmanagedType)
	{
		case 0x02:
		case 0x03:
		case 0x04:
		case 0x05:
		case 0x06:
		case 0x07:
		case 0x08:
		case 0x09:
		case 0x0A:
		case 0x0B:
		case 0x0C:
		case 0x14:
		case 0x15:
		case 0x1F:
		case 0x20:
		case 0x26:
		case 0x2A:
		/* Non ECMA types */
		case 0x0F:
		case 0x13:
		case 0x16:
		case 0x17:
		case 0x18:
		case 0x19:
		case 0x1A:
		case 0x1B:
		case 0x1C:
		case 0x1D:
		case 0x1E:
		case 0x22:
		case 0x23:
		case 0x24:
		case 0x25:
		case 0x28:
		case 0x2B:
		case 0x2C:
		case 0x2D:
		{
			/* OK */
		}
		break;

		default:
		{
			CGErrorForNode(info, attributeInfo->ctorArgs[0].node,
				_("invalid unmanaged type passed to the constructor"));
			return 0;
		}
		break;
	}

	for(currentNamedArg = 0; currentNamedArg < attributeInfo->numNamed; ++currentNamedArg)
	{
		ILProgramItem *item;
		ILField *attrField;
		const char *attrFieldName;

		item = ILToProgramItem(attributeInfo->namedArgs[currentNamedArg].member);
		if((attrField = ILProgramItemToField(item)) == 0)
		{
			CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
				_("Invalid named field for the marshal as attribute"));
			continue;
		}
		attrFieldName = ILField_Name(attrField);
		if(!attrFieldName)
		{
			CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
				_("Invalid named field for the marshal as attribute"));
			continue;
		}
		if(!strcmp("ArraySubType", attrFieldName))
		{
			arraySubType = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
		}
		else if(!strcmp("SizeConst", attrFieldName))
		{
			sizeValue = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
			if(sizeValue < 0)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("The array size must be >= 0"));
			}
		}
		else if(!strcmp("SizeParamIndex", attrFieldName))
		{
			if(!param)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("The size parameter index can only be set on marshal as attributes on parameters"));
			}
			else
			{
				sizeParamIndex = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
				if(sizeParamIndex <= 0)
				{
					CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
						_("The size parameter index must be >= 0"));
				}
			}
		}
		/* NON ECMA */
		else if(!strcmp("SafeArraySubType", attrFieldName))
		{
			safeArraySubType = attributeInfo->namedArgs[currentNamedArg].evalValue.un.i4Value;
		}
		else if(!strcmp("MarshalCookie", attrFieldName))
		{
			marshalCookie = attributeInfo->namedArgs[currentNamedArg].evalValue.un.strValue.str;
			marshalCookieLen = attributeInfo->namedArgs[currentNamedArg].evalValue.un.strValue.len;
			if(!marshalCookie || marshalCookieLen <= 0)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("marshal cookie must not be null or empty"));
			}
		}
		else if(!strcmp("MarshalType", attrFieldName))
		{
			marshalType = attributeInfo->namedArgs[currentNamedArg].evalValue.un.strValue.str;
			marshalTypeLen = attributeInfo->namedArgs[currentNamedArg].evalValue.un.strValue.len;
			if(!marshalType || marshalTypeLen <= 0)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("marshal type must not be null or empty"));
			}
		}
		else if(!strcmp("MarshalTypeRef", attrFieldName))
		{
			ILType *type;

			type = (ILType *)attributeInfo->namedArgs[currentNamedArg].evalValue.un.oValue;
			marshalType = CGTypeToAttrName(info, type);
			if(!marshalType)
			{
				CGErrorForNode(info, attributeInfo->namedArgs[currentNamedArg].node,
					_("invalid type"));
			}
			else
			{
				marshalTypeLen = strlen(marshalType);
			}
		}
	}

	/* Build the marshalling blob that corresponds to the supplied data */
	switch(unmanagedType)
	{
		case IL_META_NATIVETYPE_ARRAY:
		{
			/* An array that has a size value in a separate parameter */
			unsigned char buf[64];
			int blobLen = 0;

			buf[0] = IL_META_NATIVETYPE_ARRAY;
			blobLen = 1 + ILMetaCompressData(buf + 1, arraySubType);
			blobLen += ILMetaCompressData
				(buf + blobLen, (ILUInt32)sizeParamIndex);
			buf[blobLen++] = 1;		/* Multiplier */
			blobLen += ILMetaCompressData
				(buf + blobLen, (ILUInt32)sizeValue);

			if(!AddMarshalling(info, attributeInfo->owner, buf, blobLen))
			{
				return 0;
			}
		}
		break;

		case IL_META_NATIVETYPE_SAFEARRAY:
		{
			/* Safe variant array */
			unsigned char buf[64];
			int blobLen = 0;

			buf[0] = IL_META_NATIVETYPE_SAFEARRAY;
			blobLen = 1 + ILMetaCompressData(buf + 1, safeArraySubType);

			if(!AddMarshalling(info, attributeInfo->owner, buf, blobLen))
			{
				return 0;
			}
		}
		break;

		case IL_META_NATIVETYPE_FIXEDARRAY:
		{
			/* Fixed length array */
			unsigned char buf[64];
			int blobLen = 0;

			buf[0] = IL_META_NATIVETYPE_FIXEDARRAY;
			blobLen = 1 + ILMetaCompressData(buf + 1, (ILUInt32)sizeValue);
			blobLen += ILMetaCompressData(buf + blobLen, arraySubType);

			if(!AddMarshalling(info, attributeInfo->owner, buf, blobLen))
			{
				return 0;
			}
		}
		break;

		case IL_META_NATIVETYPE_CUSTOMMARSHALER:
		{
			/* Custom marshalling directive */
			unsigned char buf[IL_META_COMPRESS_MAX_SIZE * 2 +
							  3 + marshalTypeLen + marshalCookieLen];
			int blobLen = 0;

			buf[0] = IL_META_NATIVETYPE_CUSTOMMARSHALER;
			buf[1] = 0;	/* Length of GUID string (unused) */
			buf[2] = 0;	/* Length of native type name string (unused) */
			blobLen = 3;
			blobLen += ILMetaCompressData
				(buf + blobLen, (ILUInt32)marshalTypeLen);
			if(marshalTypeLen > 0)
			{
				ILMemCpy(buf + blobLen, marshalType, marshalTypeLen);
				blobLen += marshalTypeLen;
			}
			blobLen += ILMetaCompressData
				(buf + blobLen, (ILUInt32)marshalCookieLen);
			if(marshalCookieLen > 0)
			{
				ILMemCpy(buf + blobLen, marshalCookie, marshalCookieLen);
				blobLen += marshalCookieLen;
			}

			if(!AddMarshalling(info, attributeInfo->owner, buf, blobLen))
			{
				return 0;
			}
		}
		break;


		default:
		{
			unsigned char buf[64];
			int blobLen = 0;

			/* This native type is represented as a simple value */
			blobLen = ILMetaCompressData(buf, unmanagedType);

			if(!AddMarshalling(info, attributeInfo->owner, buf, blobLen))
			{
				return 0;
			}
		}
		break;
	}

	if(field)
	{
		ILMemberSetAttrs((ILMember *)field,
						 IL_META_FIELDDEF_HAS_FIELD_MARSHAL,
						 IL_META_FIELDDEF_HAS_FIELD_MARSHAL);
	}
	else if(param)
	{
		ILParameterSetAttrs(param,
							IL_META_PARAMDEF_HAS_FIELD_MARSHAL,
							IL_META_PARAMDEF_HAS_FIELD_MARSHAL);
	}

	return 1;
}

/*
 * Process a default value attribute on a field, parameter or property.
 */
static int DefaultValueAttribute(ILGenInfo *info,
								 CGAttributeInfo *attributeInfo)
{
	ILField *field = 0;
	ILParameter *param = 0;
	ILProperty *property = 0;
	ILType *type;
	ILImage *image;
	ILConstant *constant;
	ILNativeUInt elementType;
	unsigned char blob[8];
	ILUInt32 blobLen;
	
	if((field = ILProgramItemToField(attributeInfo->owner)) != 0)
	{
		type = ILFieldGetType(field);
	}
	else if((param = ILProgramItemToParameter(attributeInfo->owner)) != 0)
	{
		ILNode_MethodDeclaration *methodNode;
		ILUInt32 paramNum;
		ILMethod *method;
		ILType *signature;

		paramNum = ILParameter_Num(param);
		if(info->currentMethod && yyisa(info->currentMethod,
										ILNode_MethodDeclaration))
		{
			methodNode = (ILNode_MethodDeclaration *)(info->currentMethod);
		}
		else
		{
			return 0;
		}
		method = methodNode->methodInfo;
		if(!method)
		{
			return 0;
		}
		signature = ILMethod_Signature(method);
		if(!signature)
		{
			return 0;
		}
		type = ILTypeGetParam(signature, paramNum);
	}
	else if((property = ILProgramItemToProperty(attributeInfo->owner)) != 0)
	{
		ILMethod *method;
		ILType *signature;

		if((method = ILPropertyGetGetter(property)) != 0)
		{
			/* The property type is the type of the return value */
			signature = ILMethod_Signature(method);
			if(!signature)
			{
				return 0;
			}
			type = ILTypeGetParam(signature, 0);
		}
		else if((method = ILPropertyGetSetter(property)) != 0)
		{
			unsigned long numParams;

			/* The property type is the type of the last parameter */
			signature = ILMethod_Signature(method);
			if(!signature)
			{
				return 0;
			}
			numParams = ILTypeNumParams(signature);
			type = ILTypeGetParam(signature, numParams);
		}
		else
		{
			/* No getter or setter so something must be wrong */
			return 0;
		}
	}
	else
	{
		/* The default value attribute can be applied to everything. */
		/* So make a real custom attribute out of this one */
		return WriteCustomAttribute(info, attributeInfo);
	}
	if(attributeInfo->numArgs != 1)
	{
		/* There are ctors with more than one parameter */
		/* Only those with one can be handled here */
		if(!WriteCustomAttribute(info, attributeInfo))
		{
			return 0;
		}
		goto setOwnerFlags;
	}
	if(!type)
	{
		return 0;
	}
	if(!ILGenCastConst(info, &(attributeInfo->ctorArgs[0].evalValue),
					   attributeInfo->ctorArgs[0].evalValue.valueType,
					   ILTypeToMachineType(type)) &&
	   !ILCanCastKind(info, attributeInfo->ctorArgs[0].type,
					  type, IL_CONVERT_STANDARD ,0))
	{
		/*
		 * could not cast to the desired type so simply use the type of the
		 * arg and hope that the application can handle this.
		 */
		type = attributeInfo->ctorArgs[0].type;
	}
	type = ILTypeGetEnumType(type);
	if(ILType_IsPrimitive(type))
	{
		elementType = ILType_ToElement(type);
		switch(elementType)
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			{
				blob[0] = (unsigned char)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				blobLen = 1;
			}
			break;

			case IL_META_ELEMTYPE_CHAR:
			{
				ILUInt16 value;

				value = (ILUInt16)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				IL_WRITE_UINT16(blob, value);
				blobLen = 2;
			}
			break;

			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			{
				blob[0] = (unsigned char)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				blobLen = 1;
			}
			break;

			case IL_META_ELEMTYPE_I2:
			{
				ILInt16 value;

				value = (ILInt16)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				IL_WRITE_INT16(blob, value);
				blobLen = 2;
			}
			break;

			case IL_META_ELEMTYPE_U2:
			{
				ILUInt16 value;

				value = (ILUInt16)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				IL_WRITE_UINT16(blob, value);
				blobLen = 2;
			}
			break;

			case IL_META_ELEMTYPE_I4:
			{
				ILInt32 value;

				value = (ILInt32)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				IL_WRITE_INT32(blob, value);
				blobLen = 4;
			}
			break;

			case IL_META_ELEMTYPE_U4:
			{
				ILUInt32 value;

				value = (ILUInt32)(attributeInfo->ctorArgs[0].evalValue.un.i4Value);
				IL_WRITE_UINT32(blob, value);
				blobLen = 4;
			}
			break;

			case IL_META_ELEMTYPE_I8:
			{
				ILInt64 value;

				value = (ILInt64)(attributeInfo->ctorArgs[0].evalValue.un.i8Value);
				IL_WRITE_INT64(blob, value);
				blobLen = 8;
			}
			break;

			case IL_META_ELEMTYPE_U8:
			{
				ILUInt64 value;

				value = (ILUInt64)(attributeInfo->ctorArgs[0].evalValue.un.i8Value);
				IL_WRITE_UINT64(blob, value);
				blobLen = 8;
			}
			break;

			case IL_META_ELEMTYPE_R4:
			{
				ILFloat value;

				value = attributeInfo->ctorArgs[0].evalValue.un.r4Value;
				IL_WRITE_FLOAT(blob, value);
				blobLen = 4;
			}
			break;

			case IL_META_ELEMTYPE_R8:
			{
				ILDouble value;

				value = attributeInfo->ctorArgs[0].evalValue.un.r8Value;
				IL_WRITE_DOUBLE(blob, value);
				blobLen = 8;
			}
			break;

			default:
			{
				/* This type cannot be converted to a constant */
				if(!WriteCustomAttribute(info, attributeInfo))
				{
					return 0;
				}
				goto setOwnerFlags;
			}
			break;
		}
	}
	else if(ILTypeIsStringClass(type))
	{
		if(attributeInfo->ctorArgs[0].evalValue.un.strValue.str == 0)
		{
			elementType = IL_META_ELEMTYPE_CLASS;
			blob[0] = 0;
			blob[1] = 0;
			blob[2] = 0;
			blob[3] = 0;
			blobLen = 4;
		}
		else
		{
			void *utf16;
			const char *str;

			elementType = IL_META_ELEMTYPE_STRING;
			str = attributeInfo->ctorArgs[0].evalValue.un.strValue.str;
			utf16 = StringToUTF16(str, &blobLen,
								  attributeInfo->ctorArgs[0].evalValue.un.strValue.len);
			if(!utf16)
			{
				ILGenOutOfMemory(info);
			}
			image = ILProgramItem_Image(attributeInfo->owner);
			constant = ILConstantCreate(image, 0, attributeInfo->owner,
										IL_META_ELEMTYPE_STRING);
			if(!constant)
			{
				ILFree(utf16);
				ILGenOutOfMemory(info);
			}
			if(!ILConstantSetValue(constant, utf16, blobLen))
			{
				ILFree(utf16);
				ILGenOutOfMemory(info);
			}
			ILFree(utf16);
			goto setOwnerFlags;
		}
	}
	else if(ILType_IsClass(type))
	{
		if(attributeInfo->ctorArgs[0].evalValue.un.oValue != 0)
		{
			/* Non Null object references cannot be converted to a constant */
			return -1;
		}
		elementType = IL_META_ELEMTYPE_CLASS;
		blob[0] = 0;
		blob[1] = 0;
		blob[2] = 0;
		blob[3] = 0;
		blobLen = 4;
	}
	else
	{
		/* This type cannot be converted to a constant */
		return -1;
	}

	/* Create a constant node and attach it to the parameter */
	image = ILProgramItem_Image(attributeInfo->owner);
	constant = ILConstantCreate(image, 0, attributeInfo->owner,
								elementType);
	if(!constant)
	{
		ILGenOutOfMemory(info);
	}
	if(!ILConstantSetValue(constant, blob, blobLen))
	{
		ILGenOutOfMemory(info);
	}

setOwnerFlags:
	if(field)
	{
		ILMemberSetAttrs((ILMember *)field, IL_META_FIELDDEF_HAS_DEFAULT,
											IL_META_FIELDDEF_HAS_DEFAULT);
	}
	else if(property)
	{
		ILMemberSetAttrs((ILMember *)property, IL_META_PROPDEF_HAS_DEFAULT,
											   IL_META_PROPDEF_HAS_DEFAULT);
	}
	else if(param)
	{
		ILParameterSetAttrs(param, IL_META_PARAMDEF_HAS_DEFAULT,
								   IL_META_PARAMDEF_HAS_DEFAULT);
	}
	return 1;
}

/*
 * Process an "IndexerName" attribute on a property (or better indexer).
 */
static int IndexerNameAttribute(ILGenInfo *info,
								CGAttributeInfo *attributeInfo)
{
	/*
	 * We do nothing here.
	 * The reason for this function is that no custom attribute has to be
	 * created because this attribute should have been processed before.
	 */
	return 1;
}

/*
 * Process a "MethodImpl" attribute on a method.
 */
static int MethodImplAttribute(ILGenInfo *info,
							   CGAttributeInfo *attributeInfo)
{
	ILMethod *method;

	/* We must use this on a method */
	if((method = ILProgramItemToMethod(attributeInfo->owner)) != 0)
	{
		if(attributeInfo->numArgs == 1)
		{
			ILMethodSetImplAttrs(method, ~IL_META_METHODIMPL_CODE_TYPE_MASK,
				(attributeInfo->ctorArgs[0].evalValue.un.i4Value &
				 ~IL_META_METHODIMPL_CODE_TYPE_MASK));
			return 1;
		}
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of arguments for the method impl attribute"));
		return 0;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The method impl attribute can be used only on methods"));
	return 0;
}

/*
 * Process an "AssemblyAlgorithmId" attribute on an assembly.
 */
static int AssemblyAlgorithmIdAttribute(ILGenInfo *info,
										CGAttributeInfo *attributeInfo)
{
	ILAssembly *assembly;

	/* We must use this on an assembly */
	if((assembly = ILProgramItemToAssembly(attributeInfo->owner)) != 0)
	{
		if(attributeInfo->numArgs == 1)
		{
			ILAssemblySetHashAlgorithm(assembly, (ILUInt32)attributeInfo->ctorArgs[0].evalValue.un.i4Value);
			return 1;
		}
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of arguments for the assembly hash attribute"));
		return 0;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The assembly hash attribute can be used only on assemblies"));
	return 0;
}

/*
 * Process an "AssemblyCulture" attribute on an assembly.
 */
static int AssemblyCultureAttribute(ILGenInfo *info,
									CGAttributeInfo *attributeInfo)
{
	ILAssembly *assembly;

	/* We must use this on an assembly */
	if((assembly = ILProgramItemToAssembly(attributeInfo->owner)) != 0)
	{
		if(attributeInfo->numArgs == 1)
		{
			if(!ILAssemblySetLocale(assembly, attributeInfo->ctorArgs[0].evalValue.un.strValue.str))
			{
				ILGenOutOfMemory(info);
			}
			return 1;
		}
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of arguments for the assembly culture attribute"));
		return 0;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The assembly culture attribute can be used only on assemblies"));
	return 0;
}

static int AssemblyFlagsAttribute(ILGenInfo *info,
								  CGAttributeInfo *attributeInfo)
{
	ILAssembly *assembly;

	/* We must use this on an assembly */
	if((assembly = ILProgramItemToAssembly(attributeInfo->owner)) != 0)
	{
		if(attributeInfo->numArgs == 1)
		{
			ILAssemblySetAttrs(assembly, 0, attributeInfo->ctorArgs[0].evalValue.un.i4Value);
			return 1;
		}
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of arguments for the assembly flags attribute"));
		return 0;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The assembly flags attribute can be used only on assemblies"));
	return 0;
}

/*
 * Parse an assembly version
 */
static int ParseVersion(ILGenInfo *info, const char *versionString, int len,
						ILUInt16 *version, ILNode *attr)
{
	char currentChar;
	int index;
	int versionIndex;
	ILUInt32 versionPart;

	index = 0;
	versionIndex = 0;
	versionPart = 0;
	while(index < len)
	{
		currentChar = versionString[index];
		switch(currentChar)
		{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			{
				versionPart = versionPart * 10 + ((ILUInt32)currentChar - '0');
				if(versionPart > 0xFFFF)
				{
					/* Overflow */
					CGErrorForNode(info, attr,
							_("an overflow occured in an assembly version part"));
					return 0;
				}
			}
			break;

			case '.':
			{
				version[versionIndex] = (ILUInt16)versionPart;
				versionPart = 0;
				++versionIndex;
				if(versionIndex > 3)
				{
					/* To many parts in the version specifier */
					CGErrorForNode(info, attr,
							_("version must consist of 4 parts only"));
					return 0;
				}
			}
			break;

			case '*':
			{
				if(versionString[index + 1] != '\0')
				{
					/* Asterix must be the last part in the version string */
					return 0;
				}
				if(index > 0 && versionString[index - 1] != '.')
				{
					/* Asterix must follow immediate a period */
					CGErrorForNode(info, attr,
							_("the asterix must immediately follow after a period"));
					return 0;
				}
				/* So fill the rest of the version with some random values */
				while(versionIndex < 4)
				{
					version[versionIndex] = 0;
					++versionIndex;
				}
				return 1;
			}
			break;

			default:
			{
				/* Invalid character */
				CGErrorForNode(info, attr,
						_("invalid character in the assebbly version"));
				return 0;
			}
			break;
		}
		++index;
	}
	/* Handle the last part of the version */
	if(versionIndex == 3)
	{		
		version[versionIndex] = (ILUInt16)versionPart;
		return 1;
	}
	CGErrorForNode(info, attr, _("version must consist of 4 parts"));
	return 0;
}

static int AssemblyVersionAttribute(ILGenInfo *info,
									CGAttributeInfo *attributeInfo)
{
	ILAssembly *assembly;

	/* We must use this on an assembly */
	if((assembly = ILProgramItemToAssembly(attributeInfo->owner)) != 0)
	{
		if(attributeInfo->numArgs == 1)
		{
			ILUInt16 version[4];

			if(ParseVersion(info,
							attributeInfo->ctorArgs[0].evalValue.un.strValue.str,
							attributeInfo->ctorArgs[0].evalValue.un.strValue.len,
							version, attributeInfo->ctorArgs[0].node))
			{
				ILAssemblySetVersion(assembly, version);
				return 1;
			}
			return 0;
		}
		CGErrorForNode(info, attributeInfo->node,
			_("incorrect number of arguments for the assembly version attribute"));
		return 0;
	}
	CGErrorForNode(info, attributeInfo->node,
		_("The assembly version attribute can be used only on assemblies"));
	return 0;
}

static CGAttrConvertInfo const systemAttrs[] = {
	{"SerializableAttribute", SerializableAttribute},
	{"NonSerializedAttribute", NonSerializedAttribute},
	{0, 0}
};
static CGAttrConvertInfo const interopAttrs[] = {
	{"InAttribute", InAttribute},
	{"OutAttribute", OutAttribute},
	{"OptionalAttribute", OptionalAttribute},
	{"ComImportAttribute", ComImportAttribute},
	{"StructLayoutAttribute", StructLayoutAttribute},
	{"FieldOffsetAttribute", FieldOffsetAttribute},
	{"DllImportAttribute",	DllImportAttribute},
	{"MarshalAsAttribute", MarshalAsAttribute},
	{0, 0}
};
static CGAttrConvertInfo const reflectionAttrs[] = {
	{"AssemblyAlgorithmIdAttribute", AssemblyAlgorithmIdAttribute},
	{"AssemblyCultureAttribute", AssemblyCultureAttribute},
	{"AssemblyFlagsAttribute", AssemblyFlagsAttribute},
	{"AssemblyVersionAttribute", AssemblyVersionAttribute},
	{0, 0}
};
static CGAttrConvertInfo const compilerAttrs[] = {
	{"IndexerNameAttribute", IndexerNameAttribute},
	{"MethodImplAttribute", MethodImplAttribute},
	{0, 0}
};
static CGAttrConvertInfo const componentModelAttrs[] = {
	{"DefaultValueAttribute", DefaultValueAttribute},
	{0, 0}
};

/*
 * Handle a pseudo custom attribute that is stored in the metadata directly.
 * Returns	1 if the attribute was handled successfully.
 *			0 if it's not a pseudo custom attribute.
 *			-1 if there was an error in the pseudo custom attribute.
 */
static int HandlePseudoCustomAttribute(ILGenInfo *info,
									   CGAttributeInfo *attributeInfo)
{
	ILClass *attrClass;
	const char *namespace;
	const char *name;
	const CGAttrConvertInfo *convertInfo = 0;

	attrClass = ILMember_Owner(attributeInfo->ctor);
	if(!attrClass)
	{
		return 0;
	}
	namespace = ILClass_Namespace(attrClass);
	name = ILClass_Name(attrClass);

	if(namespace)
	{
		if(!strcmp(namespace, "System.Runtime.InteropServices"))
		{
			convertInfo = interopAttrs;
		}
		else if(!strcmp(namespace, "System.Runtime.CompilerServices"))
		{
			convertInfo = compilerAttrs;
		}
		else if(!strcmp(namespace, "System.Reflection"))
		{
			convertInfo = reflectionAttrs;
		}
		else if(!strcmp(namespace, "System"))
		{
			convertInfo = systemAttrs;
		}
		else if(!strcmp(namespace, "System.ComponentModel"))
		{
			convertInfo = componentModelAttrs;
		}
	}
	if(convertInfo)
	{
		while(convertInfo->name != 0 && strcmp(convertInfo->name, name) != 0)
		{
			++convertInfo;
		}
		if(convertInfo)
		{
			if(convertInfo->func)
			{
				int result;

				result = (*(convertInfo->func))(info, attributeInfo);
				if(result > 0)
				{
					return 1;
				}
				else if(result == 0)
				{
					return -1;
				}
			}
		}
	}
	return 0;
}

static int HandleSecurityPermissionAttribute(ILGenInfo *info,
											 CGAttrNamedArg *namedArgs,
											 ILUInt32 numNamed,
											 ILSerializeWriter *writer)
{
	ILUInt32 flags = 0;
	ILUInt32 arg;
	unsigned char buf[IL_META_COMPRESS_MAX_SIZE];
	int compressLen;
	int index;

	for(arg = 0; arg < numNamed; ++arg)
	{
		const char *attrFieldName;

		attrFieldName = ILMember_Name(namedArgs[arg].member);
		if(!strcmp(attrFieldName, "Flags"))
		{
			flags = (ILUInt32)namedArgs[arg].evalValue.un.i4Value;
		}
		else if(!strcmp(attrFieldName, "Assertion"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_ASSERTION;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_ASSERTION;
			}
		}
		else if(!strcmp(attrFieldName, "UnmanagedCode"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_UNMANAGEDCODE;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_UNMANAGEDCODE;
			}
		}
		else if(!strcmp(attrFieldName, "SkipVerification"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_SKIPVERIFICATION;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_SKIPVERIFICATION;
			}
		}
		else if(!strcmp(attrFieldName, "Execution"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_EXECUTION;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_EXECUTION;
			}
		}
		else if(!strcmp(attrFieldName, "ControlThread"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_CONTROLTHREAD;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_CONTROLTHREAD;
			}
		}
		else if(!strcmp(attrFieldName, "ControlEvidence"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_CONTROLEVIDENCE;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_CONTROLEVIDENCE;
			}
		}
		else if(!strcmp(attrFieldName, "ControlPolicy"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_CONTROLPOLICY;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_CONTROLPOLICY;
			}
		}
		else if(!strcmp(attrFieldName, "SerializationFormatter"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_SERIALIZATIONFORMATTER;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_SERIALIZATIONFORMATTER;
			}
		}
		else if(!strcmp(attrFieldName, "ControlDomainPolicy"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_CONTROLDOMAINPOLICY;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_CONTROLDOMAINPOLICY;
			}
		}
		else if(!strcmp(attrFieldName, "ControlPrincipal"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_CONTROLPRINCIPAL;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_CONTROLPRINCIPAL;
			}
		}
		else if(!strcmp(attrFieldName, "ControlAppDomain"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_CONTROLAPPDOMAIN;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_CONTROLAPPDOMAIN;
			}
		}
		else if(!strcmp(attrFieldName, "RemotingConfiguration"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_REMOTINGCONFIGURATION;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_REMOTINGCONFIGURATION;
			}
		}
		else if(!strcmp(attrFieldName, "Infrastructure"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_INFRASTRUCTURE;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_INFRASTRUCTURE;
			}
		}
		else if(!strcmp(attrFieldName, "BindingRedirects"))
		{
			ILInt32 value;

			value = namedArgs[arg].evalValue.un.i4Value;
			if(value)
			{
				flags |= IL_META_SECURITYPERMISSION_BINDINGREDIRECTS;
			}
			else
			{
				flags &= ~IL_META_SECURITYPERMISSION_BINDINGREDIRECTS;
			}
		}
		else
		{
			CGErrorForNode(info, namedArgs[arg].node,
				_("unknown named SecurityPermissionAttribute argument"));
		}
	}
	/*
	 * Now write the resulting named Flags argument.
	 */
	compressLen = ILMetaCompressData(buf, 1);
	for(index = 0; index < compressLen; ++index)
	{
		ILSerializeWriterSetInt32(writer, (ILInt32)buf[index],
								  IL_META_SERIALTYPE_I1);
	}
	ILSerializeWriterSetInt32(writer, IL_META_SERIALTYPE_PROPERTY,
							  IL_META_SERIALTYPE_U1);
	ILSerializeWriterSetInt32(writer, IL_META_SERIALTYPE_ENUM,
							  IL_META_SERIALTYPE_U1);
	ILSerializeWriterSetString(writer, "System.Security.Permissions.SecurityPermissionFlag", 50);
	ILSerializeWriterSetString(writer, "Flags", 5);
	ILSerializeWriterSetInt32(writer, flags,
							  IL_META_SERIALTYPE_I4);

	return 1;
}
/*
 * TODO: Check if the classes are defined in the core library too.
 */

/*
 * Determine if a class is a security attribute
 * (derives from System.Security.Permissions.SecurityAttribute)
 */
static int IsSecurityAttribute(ILGenInfo *info, ILClass *classInfo)
{
	ILClass *securityInfo;

	securityInfo = ILClassLookupGlobal(info->context, "SecurityAttribute",
									   "System.Security.Permissions");
	if(securityInfo)
	{
		return ILClassInheritsFrom(classInfo, securityInfo);
	}
	return 0;
}

/*
 * Determine if a class is a code access attribute
 * (derives from System.Security.Permissions.CodeAccessSecurityAttribute)
 */
static int IsCodeAccessSecurityAttribute(ILGenInfo *info, ILClass *classInfo)
{
	ILClass *securityInfo;

	securityInfo = ILClassLookupGlobal(info->context,
									   "CodeAccessSecurityAttribute",
									   "System.Security.Permissions");
	if(securityInfo)
	{
		return ILClassInheritsFrom(classInfo, securityInfo);
	}
	return 0;
}

/*
 * Determine if a class is a SecurityPermissionAttribute
 */
static int IsSecurityPermissionAttribute(ILGenInfo *info, ILClass *classInfo)
{
	const char *namespace;

	if(strcmp(ILClass_Name(classInfo), "SecurityPermissionAttribute") != 0)
	{
		return 0;
	}
	namespace = ILClass_Namespace(classInfo);
	if(!namespace || strcmp(namespace, "System.Security.Permissions") != 0)
	{
		return 0;
	}
	if(ILClass_NestedParent(classInfo) != 0)
	{
		return 0;
	}
	return 1;
}

/*
 * Determine if a class is "AttributeUsageAttribute".
 */
static int IsAttributeUsage(ILClass *classInfo)
{
	const char *namespace;

	if(strcmp(ILClass_Name(classInfo), "AttributeUsageAttribute") != 0)
	{
		return 0;
	}
	namespace = ILClass_Namespace(classInfo);
	if(!namespace || strcmp(namespace, "System") != 0)
	{
		return 0;
	}
	if(ILClass_NestedParent(classInfo) != 0)
	{
		return 0;
	}
	return 1;
}

/*
 * Determine if a class is "ConditionalAttribute".
 */
static int IsConditionalAttribute(ILClass *classInfo)
{
	const char *namespace;

	if(strcmp(ILClass_Name(classInfo), "ConditionalAttribute") != 0)
	{
		return 0;
	}
	namespace = ILClass_Namespace(classInfo);
	if(!namespace || strcmp(namespace, "System.Diagnostics") != 0)
	{
		return 0;
	}
	if(ILClass_NestedParent(classInfo) != 0)
	{
		return 0;
	}
	return 1;
}

/*
 * Determine if a type is a System.Security.Permissions.SecurityAction.
 */
static int IsSecurityAction(ILType *type)
{
	if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		const char *namespace;
		ILClass *classInfo = ILType_ToClass(type);

		if(strcmp(ILClass_Name(classInfo), "SecurityAction") != 0)
		{
			return 0;
		}
		namespace = ILClass_Namespace(classInfo);
		if(!namespace || strcmp(namespace, "System.Security.Permissions") != 0)
		{
			return 0;
		}
		if(ILClass_NestedParent(classInfo) != 0)
		{
			return 0;
		}
		return 1;
	}
	return 0;
}

/*
 * Get the number of security attributes
 */
static ILInt32 NumSecurityAttributes(CGSecurityAttributeInfo *securityAttribute)
{
	ILInt32 count = 0;

	while(securityAttribute)
	{
		++count;
		securityAttribute = securityAttribute->next;
	}
	return count;
}

/*
 * Write one permissionset
 */
static int WritePermissionSet(ILGenInfo *info, ILProgramItem *owner,
							  ILUInt32 action,
							  CGSecurityAttributeInfo *securityAttribute)
{
	ILInt32 numAttributes;

	numAttributes = NumSecurityAttributes(securityAttribute);
	if(numAttributes > 0)
	{
		ILSerializeWriter *writer = 0;
		ILDeclSecurity *decl;
		const void *blob;
		ILUInt32 blobLen;
		unsigned char buf[IL_META_COMPRESS_MAX_SIZE];
		int compressLen;
		int index;
		
		writer = ILSerializeWriterInit();
		if(!writer)
		{
			ILGenOutOfMemory(info);
		}
#if IL_VERSION_MAJOR > 1
		/* Set the leading '.' */
		ILSerializeWriterSetInt32(writer, (ILInt32)'.', IL_META_SERIALTYPE_I1);
		/* Set the compressed number of security attributes in the permissionset */
		compressLen = ILMetaCompressData(buf, numAttributes);
		for(index = 0; index < compressLen; ++index)
		{
			ILSerializeWriterSetInt32(writer, (ILInt32)buf[index],
									  IL_META_SERIALTYPE_I1);
		}
		/* Now add the security attributes */
		while(securityAttribute)
		{
			const char *attributeName;
			ILSerializeWriter *namedWriter = 0;
			const void *namedBlob;
			ILUInt32 namedLen;

			attributeName = CGClassToAttrName(info,
											  securityAttribute->securityAttribute);
			if(!attributeName)
			{
				ILGenOutOfMemory(info);
			}
			ILSerializeWriterSetString(writer, attributeName, strlen(attributeName));

			namedWriter = ILSerializeWriterInit();
			if(!namedWriter)
			{
				ILGenOutOfMemory(info);
			}
			if(IsSecurityPermissionAttribute(info, 
											 securityAttribute->securityAttribute))
			{
				/*
				 * Compress all properties so that we emit only the
				 * Flags property because the other properties might not
				 * be available.in other implementations.
				 */
				if(!HandleSecurityPermissionAttribute(info,
													  securityAttribute->namedArgs,
													  securityAttribute->numNamed,
													  namedWriter))
				{
					return 0;
				}
			}
			else
			{
				/* Here we need numExtra as a packed value */
				/* I couldn't find any doc about this */
				compressLen = ILMetaCompressData(buf, 
												 securityAttribute->numNamed);
				for(index = 0; index < compressLen; ++index)
				{
					ILSerializeWriterSetInt32(namedWriter,
											  (ILInt32)buf[index],
											  IL_META_SERIALTYPE_I1);
				}

				if(!WriteNamedAttributeArguments(info, namedWriter,
												 securityAttribute->namedArgs,
												 securityAttribute->numNamed))
				{
					ILSerializeWriterDestroy(namedWriter);
					ILSerializeWriterDestroy(writer);
					return 0;
				}
			}
			namedBlob = ILSerializeWriterGetBlob(namedWriter, &namedLen);
			if(!namedBlob)
			{
				ILGenOutOfMemory(info);
			}
			ILSerializeWriterSetString(writer, (const char *)namedBlob, namedLen);
			ILSerializeWriterDestroy(namedWriter);

			securityAttribute = securityAttribute->next;
		}

		blob = ILSerializeWriterGetBlob(writer, &blobLen);
		if(!blob)
		{
			ILGenOutOfMemory(info);
		}

		/* Create a security declaration and attach it to the item */
		decl = ILDeclSecurityCreate(ILProgramItem_Image(owner), 0, owner,
									action);
		if(!decl)
		{
			ILGenOutOfMemory(info);
		}
		if(!ILDeclSecuritySetBlob(decl, blob, blobLen))
		{
			ILGenOutOfMemory(info);
		}
#else
		/*
		 * TODO: Write the 1.x XML blob
		 */
#endif
		ILSerializeWriterDestroy(writer);
	}
	return 1;
}
/*
 * Write the permissionsets
 */
static int WritePermissionSets(CGAttributeInfos *attributeInfos)
{
	CGPermissionSets *permissionSets;

	permissionSets = attributeInfos->permissionSets;
	while(permissionSets)
	{
		int index;

		for(index = 0; index < 9; ++index)
		{
			if(permissionSets->permissionSet[index])
			{
				/* Write the permission set */
				if(!WritePermissionSet(attributeInfos->info,
									   permissionSets->owner,
									   index + 2,
									   permissionSets->permissionSet[index]))
				{
					return 0;
				}
			}
		}
		permissionSets = permissionSets->next;
	}
	return 1;
}

/*
 * Check if the attribute target is valid.
 */
static int AttributeTargetIsValid(ILGenInfo *info, ILProgramItem *owner,
								  ILClass *attribute,
								  ILAttributeUsageAttribute *attributeUsage,
								  ILNode *node,
								  ILUInt32 target)
{
	if(IsAttributeUsage(attribute))
	{
		ILClass *classInfo;
		/* We can only use "AttributeUsageAttribute" on classes
		   that inherit from "System.Attribute" */
		classInfo = ILProgramItemToClass(owner);
		if(!classInfo ||
		   !ILTypeAssignCompatible(info->image, ILClassToType(classInfo),
								   ILFindSystemType(info, "Attribute")))
		{
			CGErrorForNode(info, node,
			_("`System.AttributeUsageAttribute' is allowed only on attributes"));
			return 0;
		}
	}
	else if(IsConditionalAttribute(attribute))
	{
		if(target == IL_ATTRIBUTE_TARGET_CLASS)
		{
			ILClass *classInfo;

			classInfo = ILProgramItemToClass(owner);
			if(!classInfo ||
			   !ILTypeAssignCompatible(info->image, ILClassToType(classInfo),
									   ILFindSystemType(info, "Attribute")))
			{
				CGErrorForNode(info, node,
					    _("The attribute `%s' is not allowed on `%s'"),
						CGClassToName(attribute),
						AttributeTargetToName(target));
				return 0;
			}
		}
		else if(target != IL_ATTRIBUTE_TARGET_METHOD)
		{
			CGErrorForNode(info, node,
						   _("The attribute `%s' is not allowed on `%s'"),
						   CGClassToName(attribute),
						   AttributeTargetToName(target));
			return 0;
		}
	}
	else
	{
		ILUInt32 validOn;

		validOn = ILAttributeUsageAttributeGetValidOn(attributeUsage);
		if((validOn & target) == 0)
		{
			CGErrorForNode(info, node,
						   _("The attribute `%s' is not allowed on `%s'"),
						   CGClassToName(attribute),
						   AttributeTargetToName(target));
			return 0;
		}
	}

	return 1;
}

/*
 * Check if there is allready an instance of the given attribute class
 * collected for the owner. (Check for AllowMultiple).
 * Returns the attributeInfo of the instance found or 0 if none was found.
 */
static CGAttributeInfo *AttributeExistsForOwner(CGAttributeInfos *attributeInfos,
												ILProgramItem *owner,
												ILClass *attribute)
{
	CGAttributeInfo *attributeInfo;

	attributeInfo = attributeInfos->attributes;
	while(attributeInfo)
	{
		if(attributeInfo->owner == owner &&
		   ILMethod_Owner(attributeInfo->ctor) == attribute)
		{
			return attributeInfo;
		}
		attributeInfo = attributeInfo->next;
	}
	return 0;
}

static int CGPermissionSetsAddAttribute(CGAttributeInfos *attributeInfos,
										CGPermissionSets *permissionSets,
										ILInt32 action,
										ILNode *node,
										ILClass *securityAttribute,
										CGAttrNamedArg *namedArgs,
										ILUInt32 numNamed)
{
	ILInt32 index = action - 2;
	CGSecurityAttributeInfo *attributeInfo;

	attributeInfo = ILMemStackAlloc(&(attributeInfos->memstack),
									CGSecurityAttributeInfo);
	if(attributeInfo == 0)
	{
		ILGenOutOfMemory(attributeInfos->info);
		return 0;
	}
	attributeInfo->node = node;
	attributeInfo->securityAttribute = securityAttribute;
	attributeInfo->namedArgs = namedArgs;
	attributeInfo->numNamed = numNamed;
	attributeInfo->next = permissionSets->permissionSet[index];
	permissionSets->permissionSet[index] = attributeInfo;

	return 1;
}

static int AddSecurityAttribute(CGAttributeInfos *attributeInfos,
								ILNode *node,
								ILProgramItem *owner,
								ILMethod *ctor,
								CGAttrCtorArg *ctorArgs,
								ILUInt32 numArgs,
								CGAttrNamedArg *namedArgs,
								ILUInt32 numNamed,
								ILUInt32 target)
{
	ILInt32 action;
	ILClass *securityAttribute;
	CGPermissionSets *permissionSets;

	/*
	 * Constructors of attributes derieved from
	 * System.Security.Permissions.SecurityAttribute must have only one
	 * argument.
	 */
	if(numArgs != 1)
	{
		CGErrorForNode(attributeInfos->info, node,
			_("Constructors for security attributes must have one argument"));
		return 0;
	}
	/*
	 * The argument must be a System.Security.Permissions.SecurityAction.
	 */
	if(!IsSecurityAction(ctorArgs[0].type))
	{
		CGErrorForNode(attributeInfos->info, node,
		_("The argument for the constructor of a security attribute must be a `%s' and not a `%s'"),
					   "System.Security.Permissions.SecurityAction",
					   CGTypeToName(attributeInfos->info, ctorArgs[0].type));
		return 0;
	}
	action = ctorArgs[0].evalValue.un.i4Value;
	if(action < 2 || action > 10)
	{
		CGErrorForNode(attributeInfos->info, ctorArgs[0].node,
			_("Invalid SecurityAction %i"), action);
		return 0;
	}
	/* Check if the target is valid for the supplied security action */
	switch(action)
	{
		/* Actions allowed on methods and types */
		case IL_META_SECURITY_DEMAND:
		case IL_META_SECURITY_ASSERT:
		case IL_META_SECURITY_DENY:
		case IL_META_SECURITY_PERMIT_ONLY:
		case IL_META_SECURITY_LINK_TIME_CHECK:
		case IL_META_SECURITY_INHERITANCE_CHECK:
		{
			switch(target)
			{
				case IL_ATTRIBUTE_TARGET_CLASS:
				case IL_ATTRIBUTE_TARGET_STRUCT:
				case IL_ATTRIBUTE_TARGET_ENUM:
				case IL_ATTRIBUTE_TARGET_DELEGATE:
				case IL_ATTRIBUTE_TARGET_INTERFACE:
				case IL_ATTRIBUTE_TARGET_METHOD:
				case IL_ATTRIBUTE_TARGET_CONSTRUCTOR:
				{
					/* OK */
				}
				break;

				default:
				{
					CGErrorForNode(attributeInfos->info, ctorArgs[0].node,
									_("`%s' not allowed on `%s'"),
									SecurityActionToName(action),
									AttributeTargetToName(target));
				return 0;
				}
			}
		}
		break;

		/* Actions allowed only on assemblies */
		case IL_META_SECURITY_REQUEST_MINIMUM:
		case IL_META_SECURITY_REQUEST_OPTIONAL:
		case IL_META_SECURITY_REQUEST_REFUSE:
		{
			if(target != IL_ATTRIBUTE_TARGET_ASSEMBLY)
			{
				CGErrorForNode(attributeInfos->info, ctorArgs[0].node,
							   _("`%s' not allowed on `%s'"),
							   SecurityActionToName(action),
							   AttributeTargetToName(target));
				return 0;
			}
		}
		break;
	}

	/* Check if the security attribute is valid for the action */
	securityAttribute = ILMethod_Owner(ctor);
	switch(action)
	{
		/* Attribute must derive from CodeAccessSecurityAttribute */
		case IL_META_SECURITY_DEMAND:
		case IL_META_SECURITY_ASSERT:
		case IL_META_SECURITY_DENY:
		case IL_META_SECURITY_LINK_TIME_CHECK:
		case IL_META_SECURITY_INHERITANCE_CHECK:
		case IL_META_SECURITY_PERMIT_ONLY:
		{
			if(!IsCodeAccessSecurityAttribute(attributeInfos->info,
											  securityAttribute))
			{
				CGErrorForNode(attributeInfos->info, node,
							   _("`%s' not allowed for security action `%s'"),
							   CGClassToName(securityAttribute),
							   SecurityActionToName(action));
				return 0;
			}
		}
		break;
	}
	/* Get the permissionsets for the owner */
	permissionSets = CGPermissionSetsAlloc(attributeInfos, owner);
	if(permissionSets == 0)
	{
		/* The out of memory should have been raised before */
		return 0;
	}
	/* Now add the attribute to the permissionsets */
	return CGPermissionSetsAddAttribute(attributeInfos, permissionSets,
										action, node, securityAttribute,
										namedArgs, numNamed);
}

/*
 * Add an attribute to the attribute infos.
 */
int CGAttributeInfosAddAttribute(CGAttributeInfos *attributeInfos,
								 ILNode *node,
								 ILProgramItem *owner,
								 ILMethod *ctor,
								 CGAttrCtorArg *ctorArgs,
								 ILUInt32 numArgs,
								 CGAttrNamedArg *namedArgs,
								 ILUInt32 numNamed,
								 ILUInt32 target)
{
	CGAttributeInfo *attributeInfo;
	ILClass *attribute;
	ILAttributeUsageAttribute *attributeUsage;
	ILBool allowMultiple;

	if(attributeInfos == 0)
	{
		return 0;
	}
	if(ctor == 0)
	{
		return 0;
	}
	attribute = ILMethod_Owner(ctor);
	/* Get the usage information for the attribute */
	attributeUsage = ILClassGetAttributeUsage(attribute);
	if(!attributeUsage)
	{
		CGErrorForNode(attributeInfos->info, node,
					   _("Failed to retrieve AttibuteUsage information"));
		return 0;
	}
	/* Check if the attribute is valid for this target */
	if(!AttributeTargetIsValid(attributeInfos->info, owner, attribute,
							   attributeUsage, node, target))
	{
		return 1;
	}
	if(IsSecurityAttribute(attributeInfos->info, attribute))
	{
		return AddSecurityAttribute(attributeInfos, node, owner, ctor,
									ctorArgs, numArgs, namedArgs, numNamed,
									target);
	}
	allowMultiple = ILAttributeUsageAttributeGetAllowMultiple(attributeUsage);
	if(allowMultiple == 0)
	{
		CGAttributeInfo *checkInfo;

		checkInfo = AttributeExistsForOwner(attributeInfos, owner,
											ILMethod_Owner(ctor));
		if(checkInfo)
		{
			CGErrorForNode(attributeInfos->info, node,
				_("The `%s' must not be applied multiple times to an owner."
				  " The previous application was in line %lu"),
				CGClassToName(attribute),
				yygetlinenum(checkInfo->node));
			return 1;
		}
	}

	attributeInfo = ILMemStackAlloc(&(attributeInfos->memstack),
									CGAttributeInfo);
	if(attributeInfo == 0)
	{
		ILGenOutOfMemory(attributeInfos->info);
	}
	attributeInfo->node = node;
	attributeInfo->owner = owner;
	attributeInfo->ctor = ctor;
	attributeInfo->ctorArgs = ctorArgs;
	attributeInfo->numArgs = numArgs;
	attributeInfo->namedArgs = namedArgs;
	attributeInfo->numNamed = numNamed;
	attributeInfo->next = attributeInfos->attributes;
	attributeInfos->attributes = attributeInfo;

	return 1;
}

/*
 * Process the attributes.
 */
int CGAttributeInfosProcess(CGAttributeInfos *attributeInfos)
{
	CGAttributeInfo *attributeInfo;

	if(attributeInfos == 0)
	{
		return 0;
	}

	/* Write the custom attributes */
	attributeInfo = attributeInfos->attributes;
	while(attributeInfo)
	{
		/* Handle pseudo custom attributes stored in the metadata directly */
		if(!HandlePseudoCustomAttribute(attributeInfos->info, attributeInfo))
		{
			WriteCustomAttribute(attributeInfos->info, attributeInfo);
		}

		attributeInfo = attributeInfo->next;
	}
	/* Write the security attributes */
	return WritePermissionSets(attributeInfos);
}

#ifdef	__cplusplus
};
#endif
