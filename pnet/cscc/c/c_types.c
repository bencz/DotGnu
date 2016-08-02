/*
 * c_types.c - Type representation for the C programming language.
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

#include <cscc/c/c_internal.h>
/* This one is included only to be able to read all class attributes */
/* Non System flags are masked out by ILClass_Attrs (ILClassGetAttrs). */
/* So the flags for IL_META_TYPEDEF_IS_STRUCT, IL_META_TYPEDEF_IS_UNION */
/* and IL_META_TYPEDEF_IS_ENUM might get lost there. */
#include <image/program.h>
#include "il_serialize.h"
#include "il_crypt.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Forward declarations.
 */
static char *AppendThree(ILGenInfo *info, const char *prefix,
						 char *str, const char *suffix);

ILType *CTypeCreateStructOrUnion(ILGenInfo *info, const char *name,
								 int kind, const char *funcName)
{
	int funcNameLen;
	ILClass *classInfo;
	char *newName;

	/* Determine if we need to qualify the name using a function name */
	if(!funcName || *funcName == '\0')
	{
		funcNameLen = 0;
	}
	else
	{
		funcNameLen = strlen(funcName) + 1;
	}

	/* Create a new name by prepending the prefix and function to the name */
	newName = (char *)ILMalloc(strlen(name) + funcNameLen + 1);
	if(!newName)
	{
		ILGenOutOfMemory(info);
	}
	if(funcNameLen > 0)
	{
		strcpy(newName, funcName);
		newName[funcNameLen - 1] = '-';
	}
	strcpy(newName + funcNameLen, name);

	/* Search for a class information block with the name */
	classInfo = ILClassLookup(ILClassGlobalScope(info->image), newName, 0);
	if(classInfo)
	{
		ILFree(newName);
		return ILType_FromValueType(classInfo);
	}

	/* Create a new class reference with the specified name */
	classInfo = ILClassCreateRef(ILClassGlobalScope(info->image), 0,
							     newName, 0);
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}

	/* Mark the reference so that we know if it is a struct or union */
	if(kind == C_STKIND_STRUCT)
	{
		ILClassSetAttrs(classInfo,
						IL_META_TYPEDEF_TYPE_BITS,
						IL_META_TYPEDEF_IS_STRUCT);
	}
	else
	{
		ILClassSetAttrs(classInfo,
						IL_META_TYPEDEF_TYPE_BITS,
						IL_META_TYPEDEF_IS_UNION);
	}

	/* Clean up and exit */
	ILFree(newName);
	return ILType_FromValueType(classInfo);
}

ILType *CTypeCreateEnum(ILGenInfo *info, const char *name,
						const char *funcName)
{
	char *newName;
	ILClass *classInfo;
	int funcNameLen;

	/* Determine if we need to qualify the name using a function name */
	if(!funcName || *funcName == '\0')
	{
		funcNameLen = 0;
	}
	else
	{
		funcNameLen = strlen(funcName) + 1;
	}

	/* Create a new name by prepending the function name to the name */
	newName = (char *)ILMalloc(strlen(name) + funcNameLen + 6);
	if(!newName)
	{
		ILGenOutOfMemory(info);
	}
	if(funcNameLen > 0)
	{
		strcpy(newName, funcName);
		newName[funcNameLen - 1] = '-';
	}
	strcpy(newName + funcNameLen, name);

	/* Search for a class information block with the name */
	classInfo = ILClassLookup(ILClassGlobalScope(info->image), newName, 0);
	if(classInfo)
	{
		ILFree(newName);
		return ILType_FromValueType(classInfo);
	}

	/* Create a new class with the specified name */
	classInfo = ILClassCreateRef(ILClassGlobalScope(info->image), 0,
							     newName, 0);
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}

	/* Mark the reference so that we know if it is an enum */
	ILClassSetAttrs(classInfo,
					IL_META_TYPEDEF_TYPE_BITS,
					IL_META_TYPEDEF_IS_ENUM);

	/* Clean up and exit */
	ILFree(newName);
	return ILType_FromValueType(classInfo);
}

/*
 * Format the name of an array type.
 */
static char *FormatArrayName(ILGenInfo *info, ILType *elemType,
							 ILUInt32 size, ILNode *sizeNode, int isOpen)
{
	char *innerName;
	char sizeName[64];
	ILType *type;

	/* Find the innermost element type */
	type = elemType;
	while(CTypeIsArray(type))
	{
		type = CTypeGetElemType(type);
	}

	/* Format the innermost element type and the passed-in array size */
	innerName = CTypeToName(info, type);
	if(isOpen)
	{
		innerName = AppendThree(info, 0, innerName, "[]");
	}
	else if(sizeNode)
	{
		/* Convert the size node into a name */
		CName nameInfo = ILNode_CName(sizeNode);
		innerName = AppendThree(info, 0, innerName, "[");
		innerName = AppendThree(info, 0, innerName, nameInfo.name);
		innerName = AppendThree(info, 0, innerName, "]");
		ILFree(nameInfo.name);
	}
	else
	{
		sprintf(sizeName, "[%lu]", (unsigned long)size);
		innerName = AppendThree(info, 0, innerName, sizeName);
	}

	/* Format the dimension specifiers for the other dimensions */
	type = elemType;
	while(CTypeIsArray(type))
	{
		if(CTypeIsOpenArray(type))
		{
			innerName = AppendThree(info, 0, innerName, "[]");
		}
		else
		{
			sprintf(sizeName, "[%lu]", (unsigned long)(CTypeGetNumElems(type)));
			innerName = AppendThree(info, 0, innerName, sizeName);
		}
		type = CTypeGetElemType(type);
	}

	/* Return the formatted name to the caller */
	return innerName;
}

/*
 * Create an array type, with either a size or an open-ended definition.
 */
static ILType *CreateArray(ILGenInfo *info, ILType *elemType,
						   ILUInt32 size, ILNode *sizeNode, int isOpen)
{
	char *name;
	ILUInt32 attrs;
	ILClass *classInfo;
	ILField *field;
	CTypeLayoutInfo layout;
	int needClassLayout;

	/* Format the name of the array type */
	name = AppendThree(info, "array ",
				FormatArrayName(info, elemType, size, sizeNode, isOpen), 0);

	/* See if we already have a type with this name */
	classInfo = ILClassLookup(ILClassGlobalScope(info->image), name, 0);
	if(classInfo)
	{
		ILFree(name);
		return ILType_FromValueType(classInfo);
	}

	/* Get the size and alignment of the element type */
	CTypeGetLayoutInfo(elemType, &layout);
	if(layout.category == C_TYPECAT_NO_LAYOUT)
	{
		return 0;
	}

	/* Validate the array size: it must not overflow the ".size" field
	   within the class's metadata structure */
	if(layout.category == C_TYPECAT_FIXED && !sizeNode && !isOpen && size != 0)
	{
		if((((ILUInt64)(layout.size)) * ((ILUInt64)size)) >
				(ILUInt64)(ILInt64)IL_MAX_INT32)
		{
			return 0;
		}
		needClassLayout = 1;
	}
	else
	{
		needClassLayout = 0;
	}

	/* Determine the attributes for the array type */
	if(ILType_IsValueType(elemType) &&
	   !ILClass_IsPublic(ILType_ToValueType(elemType)))
	{
		/* The element type is not exported, so neither should the array.
		   This can happen when creating arrays of anonymous structs */
		attrs = IL_META_TYPEDEF_NOT_PUBLIC;
	}
	else
	{
		/* Export the array type to match the element type */
		attrs = IL_META_TYPEDEF_PUBLIC;
	}
	attrs |= IL_META_TYPEDEF_SEALED | IL_META_TYPEDEF_SERIALIZABLE;
	if(needClassLayout)
	{
		/* Use explicit layout for arrays with fixed layout */
		attrs |= IL_META_TYPEDEF_EXPLICIT_LAYOUT;
	}
	else
	{
		/* Use sequential layout for arrays with complex layout */
		attrs |= IL_META_TYPEDEF_LAYOUT_SEQUENTIAL;
	}

	/* Create the class that corresponds to the array type */
	classInfo = ILType_ToClass(ILFindSystemType(info, "ValueType"));
	classInfo = ILClassCreate(ILClassGlobalScope(info->image), 0,
							  name, 0, ILToProgramItem(classInfo));
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}
	ILClassSetAttrs(classInfo, ~((ILUInt32)0), attrs);

	/* Set the explicit size for the entire array type */
	if(needClassLayout)
	{
		if(!ILClassLayoutCreate(info->image, 0, classInfo,
							    0, layout.size * size))
		{
			ILGenOutOfMemory(info);
		}
	}

	/* If we have a size node, then add it as user data and inhibit
	   the parser from rolling back the nodes with "yynodepop()" */
	if(!needClassLayout && !sizeNode && !isOpen)
	{
		/* Create a node for the constant size, because we cannot store
		   the value in the ".size" directive on the class */
		sizeNode = ILNode_UInt32_create((ILUInt64)size, 0, 1);
	}
	if(sizeNode)
	{
		ILClassSetUserData(classInfo, sizeNode);
		CInhibitNodeRollback();
	}

	/* Create the "elem__" field which defines the type */
	if(isOpen)
	{
		/* Open arrays store the type as a "private static" field */
		field = ILFieldCreate(classInfo, 0, "elem__",
							  IL_META_FIELDDEF_PRIVATE |
							  IL_META_FIELDDEF_STATIC |
							  IL_META_FIELDDEF_SPECIAL_NAME);
	}
	else
	{
		/* Other arrays store the type in a regular field */
		field = ILFieldCreate(classInfo, 0, "elem__",
							  IL_META_FIELDDEF_PUBLIC |
							  IL_META_FIELDDEF_SPECIAL_NAME);
		if(needClassLayout)
		{
			if(!ILFieldLayoutCreate(info->image, 0, field, 0))
			{
				ILGenOutOfMemory(info);
			}
		}
	}
	if(!field)
	{
		ILGenOutOfMemory(info);
	}
	ILMemberSetSignature((ILMember *)field, elemType);

	/* Return the array type to the caller */
	return ILType_FromValueType(classInfo);
}

ILType *CTypeCreateArray(ILGenInfo *info, ILType *elemType, ILUInt32 size)
{
	return CreateArray(info, elemType, size, 0, 0);
}

ILType *CTypeCreateArrayNode(ILGenInfo *info, ILType *elemType, ILNode *size)
{
	ILEvalValue value;
	if(ILNode_EvalConst(size, info, &value))
	{
		return CreateArray(info, elemType, (ILUInt32)(value.un.i4Value), 0, 0);
	}
	else
	{
		return CreateArray(info, elemType, 0, size, 0);
	}
}

ILType *CTypeCreateOpenArray(ILGenInfo *info, ILType *elemType)
{
	return CreateArray(info, elemType, 0, 0, 1);
}

ILType *CTypeCreatePointer(ILGenInfo *info, ILType *refType)
{
	ILType *type = ILTypeCreateRef(info->context, IL_TYPE_COMPLEX_PTR, refType);
	if(!type)
	{
		ILGenOutOfMemory(info);
	}
	return type;
}

/*
 * Add a qualifier to a type.
 */
static ILType *AddQualifier(ILGenInfo *info, ILType *type,
							const char *name, int checkDups)
{
	ILClass *classInfo;
	ILType *modifiers;
	classInfo = ILType_ToClass(ILFindNonSystemType
			(info, name, "OpenSystem.C"));
	if(checkDups && ILTypeHasModifier(type, classInfo))
	{
		/* The type already has the specified modifier, so don't add again */
		return type;
	}
	else
	{
		/* Add a modifier prefix to the type */
		modifiers = ILTypeCreateModifier(info->context, 0,
										 IL_TYPE_COMPLEX_CMOD_OPT,
										 classInfo);
		if(!modifiers)
		{
			ILGenOutOfMemory(info);
		}
		return ILTypeAddModifiers(info->context, modifiers, type);
	}
}

ILType *CTypeCreateComplexPointer(ILGenInfo *info, ILType *refType)
{
	ILType *type = CTypeCreatePointer(info, refType);
	return AddQualifier(info, type, "IsComplexPointer", 1);
}

ILType *CTypeCreateByRef(ILGenInfo *info, ILType *refType)
{
	ILType *type = ILTypeCreateRef
		(info->context, IL_TYPE_COMPLEX_BYREF, refType);
	if(!type)
	{
		ILGenOutOfMemory(info);
	}
	return type;
}

ILType *CTypeCreateVaList(ILGenInfo *info)
{
	/* The base class library's "System.ArgIterator" class is the
	   underlying representation for "__builtin_va_list" */
	return ILFindSystemType(info, "ArgIterator");
}

ILType *CTypeCreateVoidPtr(ILGenInfo *info)
{
	static ILType *voidPtr = 0;
	if(!voidPtr)
	{
		voidPtr = CTypeCreatePointer(info, ILType_Void);
	}
	return voidPtr;
}

ILType *CTypeCreateCharPtr(ILGenInfo *info)
{
	static ILType *charPtr = 0;
	if(!charPtr)
	{
		charPtr = CTypeCreatePointer(info, ILType_Int8);
	}
	return charPtr;
}

ILType *CTypeCreateWCharPtr(ILGenInfo *info)
{
	static ILType *wcharPtr = 0;
	if(!wcharPtr)
	{
		wcharPtr = CTypeCreatePointer(info, ILType_Char);
	}
	return wcharPtr;
}

ILType *CTypeAddConst(ILGenInfo *info, ILType *type)
{
	return AddQualifier(info, type, "IsConst", 1);
}

ILType *CTypeAddVolatile(ILGenInfo *info, ILType *type)
{
	ILClass *classInfo;
	ILType *modifiers;
	classInfo = ILType_ToClass(ILFindNonSystemType
			(info, "IsVolatile", "System.Runtime.CompilerServices"));
	if(ILTypeHasModifier(type, classInfo))
	{
		/* The type already has the specified modifier, so don't add again */
		return type;
	}
	else
	{
		/* Add a modifier prefix to the type */
		modifiers = ILTypeCreateModifier(info->context, 0,
										 IL_TYPE_COMPLEX_CMOD_REQD,
										 classInfo);
		if(!modifiers)
		{
			ILGenOutOfMemory(info);
		}
		return ILTypeAddModifiers(info->context, modifiers, type);
	}
}

ILType *CTypeAddFunctionPtr(ILGenInfo *info, ILType *type)
{
	return AddQualifier(info, type, "IsFunctionPointer", 1);
}

ILType *CTypeAddManaged(ILGenInfo *info, ILType *type)
{
	return AddQualifier(info, type, "IsManaged", 0);
}

ILType *CTypeAddUnmanaged(ILGenInfo *info, ILType *type)
{
	return AddQualifier(info, type, "IsUnmanaged", 0);
}

ILType *CTypeStripGC(ILType *type)
{
	if(CTypeIsManaged(type) || CTypeIsUnmanaged(type))
	{
		return type->un.modifier__.type__;
	}
	else
	{
		return type;
	}
}

int CTypeAlreadyDefined(ILType *type)
{
	if(ILType_IsValueType(type))
	{
		return !(ILClassIsRef(ILType_ToValueType(type)));
	}
	else
	{
		return 0;
	}
}

/*
 * Set the correct class attributes for a struct or union.
 */
static void SetupStructAttrs(ILGenInfo *info, ILClass *classInfo, int kind)
{
	if(kind == C_STKIND_STRUCT)
	{
		ILClassSetAttrs(classInfo, ~((ILUInt32)0),
						IL_META_TYPEDEF_PUBLIC |
						IL_META_TYPEDEF_SERIALIZABLE |
						IL_META_TYPEDEF_LAYOUT_SEQUENTIAL |
						IL_META_TYPEDEF_SEALED |
						IL_META_TYPEDEF_IS_STRUCT);
	}
	else
	{
		ILClassSetAttrs(classInfo, ~((ILUInt32)0),
						IL_META_TYPEDEF_PUBLIC |
						IL_META_TYPEDEF_SERIALIZABLE |
						IL_META_TYPEDEF_EXPLICIT_LAYOUT |
						IL_META_TYPEDEF_SEALED |
						IL_META_TYPEDEF_IS_UNION);
	}
}

ILType *CTypeDefineStructOrUnion(ILGenInfo *info, const char *name,
								 int kind, const char *funcName)
{
	ILType *type;
	ILClass *classInfo;
	ILClass *parent;

	/* Create the type reference, and bail out if already defined */
	type = CTypeCreateStructOrUnion(info, name, kind, funcName);
	if(CTypeAlreadyDefined(type))
	{
		return 0;
	}

	/* Convert the reference into an actual class definition */
	classInfo = ILType_ToValueType(type);
	parent = ILType_ToClass(ILFindSystemType(info, "ValueType"));
	classInfo = ILClassCreate(ILClassGlobalScope(info->image), 0,
							  ILClass_Name(classInfo), 0, ILToProgramItem(parent));
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}
	SetupStructAttrs(info, classInfo, kind);

	/* The type definition is ready to go */
	return type;
}

ILType *CTypeDefineAnonStructOrUnion(ILGenInfo *info, ILType *parent,
							  		 const char *funcName, int kind)
{
	long number;
	ILNestedInfo *nested;
	ILClass *parentInfo;
	ILClass *classInfo;
	ILProgramItem *scope;
	char name[64];
	char *newName;
	ILUInt32 attrs;

	/* Get the number to assign to the anonymous type */
	if(parent)
	{
		/* Count the nested types to determine the number */
		parentInfo = ILType_ToValueType(parent);
		number = 1;
		nested = 0;
		while((nested = ILClassNextNested(parentInfo, nested)) != 0)
		{
			++number;
		}
		scope = ILToProgramItem(parentInfo);
		attrs = IL_META_TYPEDEF_NESTED_PUBLIC;
	}
	else
	{
		/* Use the size of the TypeDef table to determine the number */
		number = (long)(ILImageNumTokens(info->image,
										 IL_META_TOKEN_TYPE_DEF) + 1);
		scope = ILClassGlobalScope(info->image);
		attrs = IL_META_TYPEDEF_NOT_PUBLIC;
	}

	/* Format the name of the type */
	if(funcName && *funcName != '\0')
	{
		/* Format the name as "struct func(N)" */
		sprintf(name, "(%ld)", number);
		newName = ILDupString(funcName);
		if(!newName)
		{
			ILGenOutOfMemory(info);
		}
		if(kind == C_STKIND_STRUCT)
		{
			newName = AppendThree(info, "struct ", newName, name);
		}
		else
		{
			newName = AppendThree(info, "union ", newName, name);
		}
	}
	else
	{
		/* Format the name as "struct (N)" */
		if(kind == C_STKIND_STRUCT)
		{
			sprintf(name, "struct (%ld)", number);
		}
		else
		{
			sprintf(name, "union (%ld)", number);
		}
		newName = ILDupString(name);
		if(!newName)
		{
			ILGenOutOfMemory(info);
		}
	}

	/* Create the anonymous type */
	parentInfo = ILType_ToClass(ILFindSystemType(info, "ValueType"));
	classInfo = ILClassCreate(scope, 0, newName, 0, ILToProgramItem(parentInfo));
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}
	SetupStructAttrs(info, classInfo, kind);

	/* The type definition is ready to go */
	ILFree(newName);
	return ILType_FromValueType(classInfo);
}

ILType *CTypeDefineEnum(ILGenInfo *info, const char *name,
						const char *funcName)
{
	ILType *type;
	ILClass *classInfo;
	ILClass *parent;
	ILField *field;

	/* Create the enum type reference, and bail out if already defined */
	type = CTypeCreateEnum(info, name, funcName);
	if(CTypeAlreadyDefined(type))
	{
		return 0;
	}

	/* Convert the reference into an actual class definition */
	classInfo = ILType_ToValueType(type);
	parent = ILType_ToClass(ILFindSystemType(info, "Enum"));
	classInfo = ILClassCreate(ILClassGlobalScope(info->image), 0,
							  ILClass_Name(classInfo), 0, ILToProgramItem(parent));
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}

	/* Set the attributes on the enumerated type correctly */
	ILClassSetAttrs(classInfo, ~((ILUInt32)0),
					IL_META_TYPEDEF_PUBLIC |
					IL_META_TYPEDEF_SERIALIZABLE |
					IL_META_TYPEDEF_SEALED);

	/* Add the "value__" field to hold the enum's value */
	field = ILFieldCreate(classInfo, 0, "value__",
						  IL_META_FIELDDEF_PUBLIC |
						  IL_META_FIELDDEF_SPECIAL_NAME |
						  IL_META_FIELDDEF_RT_SPECIAL_NAME);
	if(!field)
	{
		ILGenOutOfMemory(info);
	}
	ILMemberSetSignature((ILMember *)field, ILType_Int32);

	/* The enum definition is ready to go */
	return type;
}

ILType *CTypeDefineAnonEnum(ILGenInfo *info, const char *funcName)
{
	int funcNameLen;
	long number;
	char name[64];
	char *newName;
	ILClass *classInfo;
	ILClass *parent;
	ILField *field;

	/* Determine if we need to qualify the name using a function name */
	if(!funcName)
	{
		funcNameLen = 0;
	}
	else
	{
		funcNameLen = strlen(funcName);
	}

	/* Get a unique number for the enumeration based on the TypeDef table */
	number = (long)(ILImageNumTokens(info->image, IL_META_TOKEN_TYPE_DEF) + 1);
	sprintf(name, "(%ld)", number);

	/* Create a new name by prepending "enum " to the name */
	newName = (char *)ILMalloc(strlen(name) + funcNameLen + 6);
	if(!newName)
	{
		ILGenOutOfMemory(info);
	}
	strcpy(newName, "enum ");
	if(funcNameLen > 0)
	{
		strcpy(newName + 5, funcName);
	}
	strcpy(newName + funcNameLen + 5, name);

	/* Create the anonymous type */
	parent = ILType_ToClass(ILFindSystemType(info, "Enum"));
	classInfo = ILClassCreate(ILClassGlobalScope(info->image), 0,
							  newName, 0, ILToProgramItem(parent));
	if(!classInfo)
	{
		ILGenOutOfMemory(info);
	}

	/* Set the attributes on the anonymous type correctly */
	ILClassSetAttrs(classInfo, ~((ILUInt32)0),
					IL_META_TYPEDEF_NOT_PUBLIC |
					IL_META_TYPEDEF_SERIALIZABLE |
					IL_META_TYPEDEF_SEALED);

	/* Add the "value__" field to hold the enum's value */
	field = ILFieldCreate(classInfo, 0, "value__",
						  IL_META_FIELDDEF_PUBLIC |
						  IL_META_FIELDDEF_SPECIAL_NAME |
						  IL_META_FIELDDEF_RT_SPECIAL_NAME);
	if(!field)
	{
		ILGenOutOfMemory(info);
	}
	ILMemberSetSignature((ILMember *)field, ILType_Int32);

	/* The enum definition is ready to go */
	ILFree(newName);
	return ILType_FromValueType(classInfo);
}

ILType *CTypeResolveAnonEnum(ILType *type)
{
	if(!CTypeIsAnonEnum(type))
	{
		return type;
	}
	else
	{
		return ILTypeGetEnumType(ILTypeStripPrefixes(type));
	}
}

ILField *CTypeDefineField(ILGenInfo *info, ILType *structType,
					 	  const char *fieldName, ILType *fieldType)
{
	ILClass *classInfo = ILType_ToValueType(structType);
	CTypeLayoutInfo layout;
	ILField *field;
	ILFieldLayout *flayout;

	/* Convert open arrays into zero-length arrays, so that
	   "type[]" can be used in a struct to mean "type[0]" */
	if(CTypeIsOpenArray(fieldType))
	{
		fieldType = CTypeCreateArray(info, CTypeGetElemType(fieldType), 0);
	}

	/* Determine the size and alignment of the new field */
	CTypeGetLayoutInfo(fieldType, &layout);
	if(layout.category == C_TYPECAT_NO_LAYOUT)
	{
		return 0;
	}

	/* Create the new field */
	field = ILFieldCreate(classInfo, 0, fieldName, IL_META_FIELDDEF_PUBLIC);
	if(!field)
	{
		ILGenOutOfMemory(info);
	}
	ILMemberSetSignature((ILMember *)field, fieldType);

	/* If we are within a union, then set the starting offset to 0.
	   This will cause the runtime engine to make all fields overlap */
	if(CTypeIsUnion(structType))
	{
		flayout = ILFieldLayoutCreate(ILProgramItem_Image(classInfo),
									  0, field, 0);
		if(!flayout)
		{
			ILGenOutOfMemory(info);
		}
	}

	/* Return the final field to the caller */
	return field;
}

/*
 * Get the constructor for the "BitFieldAttribute" class.
 */
static ILMethod *BitFieldCtor(ILGenInfo *info)
{
	ILClass *classInfo;
	ILType *args[4];

	/* Find the "BitFieldAttribute" class */
	classInfo = ILType_ToClass(ILFindNonSystemType
			(info, "BitFieldAttribute", "OpenSystem.C"));

	/* Build the argument array to look for */
	args[0] = ILFindSystemType(info, "String");
	args[1] = args[0];
	args[2] = ILType_Int32;
	args[3] = ILType_Int32;

	/* Resolve the constructor */
	return ILResolveConstructor(info, classInfo,
								ILClassLookup(ILClassGlobalScope(info->image),
											  "<Module>", 0),
								args, 4);
}

/*
 * Get the left-over space in a bit field storage area.
 */
static ILUInt32 BitFieldLeftOver(ILGenInfo *info, ILClass *classInfo,
								 const char *name, ILUInt32 *start,
								 ILUInt32 maxBits)
{
	ILMethod *ctor = BitFieldCtor(info);
	ILAttribute *attr;
	const void *blob;
	ILUInt32 blobLen;
	ILSerializeReader *reader;
	const char *str;
	int slen;
	ILUInt32 fieldStart;
	ILUInt32 fieldSize;

	/* Initialize the "start" value */
	*start = 0;

	/* Search all "BitFieldAttribute" values for "name" */
	attr = 0;
	while((attr = ILProgramItemNextAttribute
				(ILToProgramItem(classInfo), attr)) != 0)
	{
		/* Skip this attribute if it is not "BitFieldAttribute" */
		if(ILAttributeTypeAsItem(attr) != ILToProgramItem(ctor))
		{
			continue;
		}

		/* Does this attribute value belong to the specified storage area? */
		blob = ILAttributeGetValue(attr, &blobLen);
		if(!blob)
		{
			continue;
		}
		reader = ILSerializeReaderInit(ctor, blob, blobLen);
		if(!reader)
		{
			continue;
		}
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if(ILSerializeReaderGetString(reader, &str) < 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if((slen = ILSerializeReaderGetString(reader, &str)) < 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if(slen != strlen(name) || strncmp(name, str, slen) != 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}

		/* Extract the start and size values for the bit field */
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_I4)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		fieldStart = (ILUInt32)(ILSerializeReaderGetInt32
			(reader, IL_META_SERIALTYPE_I4));
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_I4)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		fieldSize = (ILUInt32)(ILSerializeReaderGetInt32
			(reader, IL_META_SERIALTYPE_I4));
		ILSerializeReaderDestroy(reader);

		/* Adjust "start" for the field position */
		if(*start < (fieldStart + fieldSize))
		{
			*start = fieldStart + fieldSize;
		}
	}

	/* Return the size of the left-over area to the caller */
	return maxBits - *start;
}

/*
 * Add an attribute that defines a bit field name.
 */
static void BitFieldAdd(ILGenInfo *info, ILClass *classInfo,
						const char *fieldName,
						ILUInt32 fieldLen,
						const char *storageName,
						ILUInt32 storageLen,
						ILUInt32 posn, ILUInt32 numBits)
{
	unsigned char fieldHeader[IL_META_COMPRESS_MAX_SIZE];
	unsigned char storageHeader[IL_META_COMPRESS_MAX_SIZE];
	int fieldHeaderLen, storageHeaderLen;
	ILUInt32 totalLen;
	unsigned char *buf;
	ILAttribute *attr;

	/* Determine the total length of the attribute value */
	fieldHeaderLen = ILMetaCompressData(fieldHeader, fieldLen);
	storageHeaderLen = ILMetaCompressData(storageHeader, storageLen);
	totalLen = 12 + fieldHeaderLen + fieldLen + storageHeaderLen + storageLen;

	/* Allocate a block of memory to use to format the value */
	buf = (unsigned char *)ILMalloc(totalLen);
	if(!buf)
	{
		ILGenOutOfMemory(info);
	}

	/* Format the attribute value */
	buf[0] = 0x01;
	buf[1] = 0x00;
	ILMemCpy(buf + 2, fieldHeader, fieldHeaderLen);
	totalLen = 2 + fieldHeaderLen;
	ILMemCpy(buf + totalLen, fieldName, fieldLen);
	totalLen += fieldLen;
	ILMemCpy(buf + totalLen, storageHeader, storageHeaderLen);
	totalLen += storageHeaderLen;
	ILMemCpy(buf + totalLen, storageName, storageLen);
	totalLen += storageLen;
	IL_WRITE_UINT32(buf + totalLen, posn);
	totalLen += 4;
	IL_WRITE_UINT32(buf + totalLen, numBits);
	totalLen += 4;
	buf[totalLen++] = 0x00;
	buf[totalLen++] = 0x00;

	/* Create an attribute block and add it to the class */
	attr = ILAttributeCreate(info->image, 0);
	if(!attr || !ILAttributeSetValue(attr, buf, totalLen))
	{
		ILGenOutOfMemory(info);
	}
	ILAttributeSetType(attr, ILToProgramItem(BitFieldCtor(info)));
	ILProgramItemAddAttribute(ILToProgramItem(classInfo), attr);
	ILFree(buf);
}

int CTypeDefineBitField(ILGenInfo *info, ILType *structType,
				 	    const char *fieldName, ILType *fieldType,
						ILUInt32 numBits, ILUInt32 maxBits)
{
	ILClass *classInfo = ILType_ToValueType(structType);
	ILField *field;
	int num;
	char name[64];
	ILUInt32 posn;
	ILUInt32 leftOverBits;
	ILUInt32 leftOverStart;

	/* Initialize variables for the bit field number and default position */
	num = 1;
	posn = 0;

	/* Determine if the last field in the structure is a bit
	   field of the right type, with sufficient space available */
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field) &&
		   !strncmp(ILField_Name(field), ".bitfield-", 10))
		{
			if(ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD) == 0)
			{
				/* This is the last bit field: check the type and space */
				if(CTypeIsIdentical(fieldType, ILField_Type(field)))
				{
					leftOverBits = BitFieldLeftOver
						(info, classInfo, ILField_Name(field),
						 &leftOverStart, maxBits);
					if(leftOverBits >= numBits)
					{
						posn = leftOverStart;
						break;
					}
				}
			}
			++num;
		}
	}

	/* Create a new bit field storage structure if necessary */
	if(!field)
	{
		sprintf(name, ".bitfield-%d", num);
		field = CTypeDefineField(info, structType, name, fieldType);
		if(!field)
		{
			return 0;
		}
	}

	/* Add a new "BitFieldAttribute" instance to the class.
	   Don't do this if the field is anonymous */
	if(fieldName)
	{
		BitFieldAdd(info, classInfo, fieldName, strlen(fieldName),
					ILField_Name(field), strlen(ILField_Name(field)),
					posn, numBits);
	}

	/* Done */
	return 1;
}

/*
 * Add a character to an MD5 context.
 */
#define	MD5HashAddChar(md5,value)	\
			do { \
				unsigned char ch = (unsigned char)(value); \
				ILMD5Data((md5), &ch, 1); \
			} while (0)

/*
 * Add a 32-bit size value to an MD5 context.
 */
static void MD5HashAddSize(ILMD5Context *md5, ILUInt32 value)
{
	unsigned char buf[4];
	IL_WRITE_UINT32(buf, value);
	ILMD5Data(md5, buf, 4);
}

/*
 * Add a class name to an MD5 context.
 */
static void MD5HashAddName(ILMD5Context *md5, ILClass *classInfo)
{
	const char *namespace = ILClass_Namespace(classInfo);
	const char *name = ILClass_Name(classInfo);
	if(namespace)
	{
		ILMD5Data(md5, namespace, strlen(namespace));
		MD5HashAddChar(md5, '.');
	}
	ILMD5Data(md5, name, strlen(name) + 1);
}

/*
 * Forward reference.
 */
static void MD5HashFields(ILMD5Context *md5, ILClass *classInfo);

/*
 * Hash a type using a given MD5 context.
 */
static void MD5HashType(ILMD5Context *md5, ILType *type)
{
	if(ILType_IsPrimitive(type))
	{
		/* Hash a primitive type */
		MD5HashAddChar(md5, ILType_ToElement(type));
	}
	else if(ILType_IsValueType(type))
	{
		int structKind = CTypeGetStructKind(type);
		if(structKind != -1)
		{
			/* Hash a struct or union type */
			MD5HashAddChar(md5, structKind + 100);
			MD5HashAddName(md5, ILType_ToValueType(type));
		}
		else if(CTypeIsOpenArray(type))
		{
			/* Hash an open array type */
			MD5HashAddChar(md5, 110);
			MD5HashAddSize(md5, 0);
			MD5HashType(md5, CTypeGetElemType(type));
		}
		else if(CTypeIsArray(type))
		{
			MD5HashAddChar(md5, 110);
			MD5HashAddSize(md5, CTypeGetNumElems(type));
			MD5HashType(md5, CTypeGetElemType(type));
		}
		else
		{
			/* Hash an ordinary value type */
			MD5HashAddChar(md5, IL_META_ELEMTYPE_VALUETYPE);
			MD5HashAddName(md5, ILType_ToValueType(type));
		}
	}
	else if(ILType_IsClass(type))
	{
		/* Hash an object reference type */
		MD5HashAddChar(md5, IL_META_ELEMTYPE_CLASS);
		MD5HashAddName(md5, ILType_ToClass(type));
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		/* Hash a complex type */
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_BYREF);
				MD5HashType(md5, ILType_Ref(type));
			}
			break;

			case IL_TYPE_COMPLEX_PTR:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_PTR);
				MD5HashType(md5, ILType_Ref(type));
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_ARRAY);
				MD5HashAddSize(md5, (ILUInt32)(ILTypeGetRank(type)));
				MD5HashType(md5, ILTypeGetElemType(type));
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_REQD:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_CMOD_REQD);
				MD5HashAddName(md5, type->un.modifier__.info__);
				MD5HashType(md5, type->un.modifier__.type__);
			}
			break;

			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_CMOD_OPT);
				MD5HashAddName(md5, type->un.modifier__.info__);
				MD5HashType(md5, type->un.modifier__.type__);
			}
			break;

			case IL_TYPE_COMPLEX_SENTINEL:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_SENTINEL);
			}
			break;

			case IL_TYPE_COMPLEX_PINNED:
			{
				MD5HashAddChar(md5, IL_META_ELEMTYPE_PINNED);
				MD5HashType(md5, ILType_Ref(type));
			}
			break;

			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				unsigned long numParams;
				unsigned long param;
				MD5HashAddChar(md5, IL_META_ELEMTYPE_FNPTR);
				MD5HashType(md5, ILTypeGetReturnWithPrefixes(type));
				numParams = ILTypeNumParams(type);
				MD5HashAddSize(md5, (ILUInt32)numParams);
				for(param = 1; param <= numParams; ++param)
				{
					MD5HashType(md5, ILTypeGetParamWithPrefixes(type, param));
				}
			}
			break;
		}
	}
}

/*
 * Hash the fields within a struct or union type.
 */
static void MD5HashFields(ILMD5Context *md5, ILClass *classInfo)
{
	ILField *field = 0;
	const char *name;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field))
		{
			name = ILField_Name(field);
			ILMD5Data(md5, name, strlen(name) + 1);
			MD5HashType(md5, ILField_Type(field));
		}
	}
}

/*
 * Create a new name for a top-level anonymous struct or union.
 */
static char *CreateNewAnonName(ILGenInfo *info, ILType *type,
							   ILClass *classInfo)
{
	ILMD5Context md5;
	unsigned char hash[IL_MD5_HASH_SIZE];
	int structKind, posn;
	char name[64];
	int value, index, bits;
	static char const encode[] =
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_$";
	char *newName;

	/* Hash the fields in the structure */
	ILMD5Init(&md5);
	MD5HashFields(&md5, classInfo);
	ILMD5Finalize(&md5, hash);

	/* Format the name */
	structKind = CTypeGetStructKind(type);
	if(structKind == C_STKIND_STRUCT)
	{
		strcpy(name, "struct (");
		posn = 8;
	}
	else
	{
		strcpy(name, "union (");
		posn = 7;
	}
	value = 0;
	bits = 0;
	for(index = 0; index < IL_MD5_HASH_SIZE; ++index)
	{
		value = (value << 8) + hash[index];
		bits += 8;
		while(bits >= 6)
		{
			bits -= 6;
			name[posn++] = encode[value >> bits];
			value &= ((1 << bits) - 1);
		}
	}
	value <<= (6 - bits);
	name[posn++] = encode[value >> bits];
	name[posn++] = ')';
	name[posn++] = '\0';

	/* Duplicate the string and return */
	newName = ILDupString(name);
	if(!newName)
	{
		ILGenOutOfMemory(info);
	}
	return newName;
}

/*
 * Clone the contents of a structure type.
 */
static void CloneStruct(ILGenInfo *info, ILClass *dest, ILClass *src)
{
	ILField *field;
	ILField *newField;
	ILFieldLayout *flayout;
	ILClassLayout *clayout;
	ILMethod *ctor;
	ILAttribute *attr;
	const void *blob;
	ILUInt32 blobLen;
	ILSerializeReader *reader;
	const char *str;
	int slen;
	const char *fieldName;
	ILUInt32 fieldLen;
	const char *storageName;
	ILUInt32 storageLen;
	ILUInt32 bitFieldStart;
	ILUInt32 bitFieldSize;

	/* Clone the fields */
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(src, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		newField = ILFieldCreate(dest, 0, ILField_Name(field),
								 ILField_Attrs(field));
		if(!newField)
		{
			ILGenOutOfMemory(info);
		}
		ILMemberSetSignature
			((ILMember *)newField, ILFieldGetTypeWithPrefixes(field));
		flayout = ILFieldLayoutGetFromOwner(field);
		if(flayout)
		{
			if(!ILFieldLayoutCreate(info->image, 0, newField,
									ILFieldLayoutGetOffset(flayout)))
			{
				ILGenOutOfMemory(info);
			}
		}
	}

	/* Clone the bit field declarations */
	ctor = BitFieldCtor(info);
	attr = 0;
	while((attr = ILProgramItemNextAttribute(ILToProgramItem(src), attr)) != 0)
	{
		/* Skip this attribute if it is not "BitFieldAttribute" */
		if(ILAttributeTypeAsItem(attr) != ILToProgramItem(ctor))
		{
			continue;
		}

		/* Extract the bit field definition's parameters */
		blob = ILAttributeGetValue(attr, &blobLen);
		if(!blob)
		{
			continue;
		}
		reader = ILSerializeReaderInit(ctor, blob, blobLen);
		if(!reader)
		{
			continue;
		}
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if((slen = ILSerializeReaderGetString(reader, &str)) < 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		fieldName = str;
		fieldLen = (ILUInt32)slen;
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if((slen = ILSerializeReaderGetString(reader, &str)) < 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		storageName = str;
		storageLen = (ILUInt32)slen;
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_I4)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		bitFieldStart = (ILUInt32)(ILSerializeReaderGetInt32
			(reader, IL_META_SERIALTYPE_I4));
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_I4)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		bitFieldSize = (ILUInt32)(ILSerializeReaderGetInt32
			(reader, IL_META_SERIALTYPE_I4));
		ILSerializeReaderDestroy(reader);

		/* Create a new bit field declaration on the cloned copy */
		BitFieldAdd(info, dest, fieldName, fieldLen,
					storageName, storageLen, bitFieldStart, bitFieldSize);
	}

	/* Clone the class layout information */
	clayout = ILClassLayoutGetFromOwner(src);
	if(clayout)
	{
		if(!ILClassLayoutCreate(info->image, 0, dest,
								ILClassLayoutGetPackingSize(clayout),
								ILClassLayoutGetClassSize(clayout)))
		{
			ILGenOutOfMemory(info);
		}
	}
}

ILType *CTypeEndStruct(ILGenInfo *info, ILType *structType, int renameAnon)
{
	ILClass *classInfo = ILType_ToValueType(structType);
	if(renameAnon)
	{
		char *newName;
		ILClass *newClass;

		/* Create a new name for the anonymous struct or union */
		newName = CreateNewAnonName(info, structType, classInfo);

		/* See if we already have a type defined with this name */
		newClass = ILClassLookup(ILClassGlobalScope(info->image), newName, 0);
		if(newClass)
		{
			ILFree(newName);
			return ILType_FromValueType(newClass);
		}

		/* Create a new type and clone the original structure into it */
		newClass = ILClassCreate(ILClassGlobalScope(info->image), 0,
							     newName, 0, ILToProgramItem(ILClass_ParentRef(classInfo)));
		if(!newClass)
		{
			ILGenOutOfMemory(info);
		}
		ILFree(newName);
		ILClassSetAttrs(newClass, ~0, classInfo->attributes);
		CloneStruct(info, newClass, classInfo);
		return ILType_FromValueType(newClass);
	}
	return structType;
}

ILField *CTypeLookupField(ILGenInfo *info, ILType *structType,
						  const char *fieldName, ILUInt32 *bitFieldStart,
						  ILUInt32 *bitFieldSize)
{
	ILClass *classInfo = ILType_ToValueType(ILTypeStripPrefixes(structType));
	ILField *field;
	ILMethod *ctor;
	ILAttribute *attr;
	const void *blob;
	ILUInt32 blobLen;
	ILSerializeReader *reader;
	const char *str;
	int slen;

	/* Search for the field by name */
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field) && !strcmp(ILField_Name(field), fieldName))
		{
			*bitFieldStart = 0;
			*bitFieldSize = 0;
			return field;
		}
	}

	/* Search for a bit field definition with the name */
	ctor = BitFieldCtor(info);
	attr = 0;
	while((attr = ILProgramItemNextAttribute
				(ILToProgramItem(classInfo), attr)) != 0)
	{
		/* Skip this attribute if it is not "BitFieldAttribute" */
		if(ILAttributeTypeAsItem(attr) != ILToProgramItem(ctor))
		{
			continue;
		}

		/* Does this attribute value belong to the specified field? */
		blob = ILAttributeGetValue(attr, &blobLen);
		if(!blob)
		{
			continue;
		}
		reader = ILSerializeReaderInit(ctor, blob, blobLen);
		if(!reader)
		{
			continue;
		}
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if((slen = ILSerializeReaderGetString(reader, &str)) < 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if(slen != strlen(fieldName) || strncmp(fieldName, str, slen) != 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_STRING)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		if((slen = ILSerializeReaderGetString(reader, &str)) < 0)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}

		/* Extract the start and size values for the bit field */
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_I4)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		*bitFieldStart = (ILUInt32)(ILSerializeReaderGetInt32
			(reader, IL_META_SERIALTYPE_I4));
		if(ILSerializeReaderGetParamType(reader) != IL_META_SERIALTYPE_I4)
		{
			ILSerializeReaderDestroy(reader);
			continue;
		}
		*bitFieldSize = (ILUInt32)(ILSerializeReaderGetInt32
			(reader, IL_META_SERIALTYPE_I4));
		ILSerializeReaderDestroy(reader);

		/* Find the underlying field */
		field = 0;
		while((field = (ILField *)ILClassNextMemberByKind
					(classInfo, (ILMember *)field,
					 IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if(!ILField_IsStatic(field) &&
			   slen == strlen(ILField_Name(field)) &&
			   !strncmp(ILField_Name(field), str, slen))
			{
				return field;
			}
		}

		/* If we get here, then the bit field definition is invalid */
		break;
	}
	return 0;
}

void CTypeDefineEnumConst(ILGenInfo *info, ILType *enumType,
					 	  const char *constName, ILInt32 constValue)
{
	ILClass *classInfo = ILType_ToValueType(enumType);
	ILField *field;
	ILConstant *constant;
	unsigned char buf[4];

	/* Create the new literal constant field */
	field = ILFieldCreate(classInfo, 0, constName,
						  IL_META_FIELDDEF_PUBLIC |
						  IL_META_FIELDDEF_STATIC |
						  IL_META_FIELDDEF_LITERAL);
	if(!field)
	{
		ILGenOutOfMemory(info);
	}
	ILMemberSetSignature((ILMember *)field, enumType);

	/* Create a constant block and attach it to the field */
	constant = ILConstantCreate(info->image, 0, ILToProgramItem(field),
								IL_META_ELEMTYPE_I4);
	if(!constant)
	{
		ILGenOutOfMemory(info);
	}
	IL_WRITE_INT32(buf, constValue);
	if(!ILConstantSetValue(constant, buf, 4))
	{
		ILGenOutOfMemory(info);
	}
}

ILType *CTypeWithoutQuals(ILType *type)
{
	/* Qualifiers are stored in the IL type as custom modifiers */
	return ILTypeStripPrefixes(type);
}

static int CheckForModifier(ILType *type, const char *name,
						    const char *namespace)
{
	ILClass *classInfo;
	while(type != 0 && ILType_IsComplex(type))
	{
		if(ILType_Kind(type) == IL_TYPE_COMPLEX_CMOD_OPT ||
		   ILType_Kind(type) == IL_TYPE_COMPLEX_CMOD_REQD)
		{
			classInfo = type->un.modifier__.info__;
			if(!strcmp(ILClass_Name(classInfo), name) &&
			   ILClass_Namespace(classInfo) != 0 &&
			   !strcmp(ILClass_Namespace(classInfo), namespace))
			{
				return 1;
			}
			type = type->un.modifier__.type__;
		}
		else
		{
			break;
		}
	}
	return 0;
}

int CTypeIsConst(ILType *type)
{
	return CheckForModifier(type, "IsConst", "OpenSystem.C");
}

int CTypeIsVolatile(ILType *type)
{
	return CheckForModifier(type, "IsVolatile",
							"System.Runtime.CompilerServices");
}

int CTypeIsManaged(ILType *type)
{
	return CheckForModifier(type, "IsManaged", "OpenSystem.C");
}

int CTypeIsUnmanaged(ILType *type)
{
	return CheckForModifier(type, "IsUnmanaged", "OpenSystem.C");
}

int CTypeIsComplexPointer(ILType *type)
{
	return CheckForModifier(type, "IsComplexPointer", "OpenSystem.C");
}

int CTypeIsPrimitive(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsPrimitive(type) || ILTypeIsEnum(type))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int CTypeIsInteger(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:	return 1;
		}
		return 0;
	}
	else if(ILTypeIsEnum(type))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int CTypeIsStruct(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		ILClass *info = ILType_ToValueType(type);

		if((info->attributes & IL_META_TYPEDEF_IS_STRUCT) != 0)
		{
			return 1;
		}
	}
	return 0;
}

int CTypeIsUnion(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		ILClass *info = ILType_ToValueType(type);

		if((info->attributes & IL_META_TYPEDEF_IS_UNION) != 0)
		{
			return 1;
		}
	}
	return 0;
}

int CTypeGetStructKind(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		ILClass *info = ILType_ToValueType(type);

		if((info->attributes & IL_META_TYPEDEF_IS_STRUCT) != 0)
		{
			return C_STKIND_STRUCT;
		}
		else if((info->attributes & IL_META_TYPEDEF_IS_UNION) != 0)
		{
			return C_STKIND_UNION;
		}
	}
	return -1;
}

int CTypeIsEnum(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		ILClass *info = ILType_ToValueType(type);

		if((info->attributes & IL_META_TYPEDEF_IS_ENUM) != 0)
		{
			return 1;
		}
		if(ILTypeIsEnum(type))
		{
			return 1;
		}
	}
	return 0;
}

int CTypeIsAnonEnum(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		if(!strncmp(ILClass_Name(ILType_ToValueType(type)), "enum (", 6))
		{
			return 1;
		}
	}
	return 0;
}

int CTypeIsArray(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		if(!strncmp(ILClass_Name(ILType_ToValueType(type)), "array ", 6))
		{
			return 1;
		}
	}
	return 0;
}

/*
 * Find the "elem__" field within an array type.
 */
static ILField *FindArrayElemField(ILClass *classInfo)
{
	ILField *field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!strcmp(ILField_Name(field), "elem__"))
		{
			return field;
		}
	}
	return 0;
}

int CTypeIsOpenArray(ILType *type)
{
	ILField *field;
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		if(!strncmp(ILClass_Name(ILType_ToValueType(type)), "array ", 6))
		{
			field = FindArrayElemField(ILType_ToValueType(type));
			if(field)
			{
				return ILField_IsPrivate(field);
			}
			else
			{
				return 0;
			}
		}
	}
	return 0;
}

int CTypeIsPointer(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int CTypeIsByRef(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_BYREF)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int CTypeIsFunctionPtr(ILType *type)
{
	if(!CheckForModifier(type, "IsFunctionPointer", "OpenSystem.C"))
	{
		return 0;
	}
	type = ILTypeStripPrefixes(type);
	return ILType_IsMethod(type);
}

int CTypeIsReference(ILType *type)
{
	return ILTypeIsReference(ILTypeStripPrefixes(type));
}

int CTypeIsFunction(ILType *type)
{
	if(CheckForModifier(type, "IsFunctionPointer", "OpenSystem.C"))
	{
		return 0;
	}
	type = ILTypeStripPrefixes(type);
	return ILType_IsMethod(type);
}

int CTypeToElementType(ILType *type)
{
	type = ILTypeGetEnumType(ILTypeStripPrefixes(type));
	if(ILType_IsPrimitive(type))
	{
		return ILType_ToElement(type);
	}
	else
	{
		return 0;
	}
}

ILUInt32 CTypeGetNumElems(ILType *type)
{
	ILClass *classInfo;
	ILField *field;
	ILType *elemType;
	ILClassLayout *clayout;
	CTypeLayoutInfo layout;
	ILNode *node;
	ILEvalValue value;

	/* Strip the prefixes and check that this is actually an array type */
	type = ILTypeStripPrefixes(type);
	if(!CTypeIsArray(type))
	{
		return 0;
	}

	/* Search for the "elem__" field within the array type */
	classInfo = ILType_ToValueType(type);
	field = FindArrayElemField(classInfo);
	if(!field)
	{
		return 0;
	}
	elemType = ILField_Type(field);

	/* Determine the size from the class layout information */
	clayout = ILClassLayoutGetFromOwner(classInfo);
	if(clayout != 0)
	{
		/* The size is determined from the class and element sizes */
		CTypeGetLayoutInfo(elemType, &layout);
		if(layout.size == 0)
		{
			/* Avoid divide by zero errors when the element type is empty */
			return 0;
		}
		else
		{
			return (ILClassLayoutGetClassSize(clayout) / layout.size);
		}
	}

	/* See if we have a constant size node associated with the type */
	node = CTypeGetComplexArraySize(type);
	if(node && ILNode_EvalConst(node, &CCCodeGen, &value))
	{
		return (ILUInt32)(value.un.i4Value);
	}

	/* The array has a complex type which cannot be computed at compile time */
	return IL_MAX_UINT32;
}

ILType *CTypeGetElemType(ILType *type)
{
	ILClass *classInfo;
	ILField *field;

	/* Strip the prefixes and check that this is actually an array type */
	type = ILTypeStripPrefixes(type);
	if(!CTypeIsArray(type))
	{
		return 0;
	}

	/* Search for the "elem__" field within the array type */
	classInfo = ILType_ToValueType(type);
	field = FindArrayElemField(classInfo);
	if(field)
	{
		type = ILFieldGetTypeWithPrefixes(field);
		if(CTypeIsFunctionPtr(type))
		{
			return type;
		}
		else
		{
			return ILField_Type(field);
		}
	}
	else
	{
		return 0;
	}
}

ILType *CTypeGetPtrRef(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
	{
		return ILType_Ref(type);
	}
	else
	{
		return 0;
	}
}

ILType *CTypeGetByRef(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_BYREF)
	{
		return ILType_Ref(type);
	}
	else
	{
		return 0;
	}
}

ILType *CTypeDecay(ILGenInfo *info, ILType *type)
{
	ILType *ptrType;

	/* Bail out if not an array type */
	if(!CTypeIsArray(type))
	{
		return type;
	}

	/* Build a pointer type from the array element type */
	ptrType = CTypeCreatePointer(info, CTypeGetElemType(type));

	/* Add back any "const" or "volatile" prefixes */
	if(CTypeIsConst(type))
	{
		ptrType = CTypeAddConst(info, ptrType);
	}
	if(CTypeIsVolatile(type))
	{
		ptrType = CTypeAddVolatile(info, ptrType);
	}

	/* Return the decayed type to the caller */
	return ptrType;
}

int CTypeIsIdentical(ILType *type1, ILType *type2)
{
	return ILTypeIdentical(type1, type2);
}

/*
 * Append three strings, where the middle one is realloc'able.
 */
static char *AppendThree(ILGenInfo *info, const char *prefix,
						 char *str, const char *suffix)
{
	int prefixLen = (prefix ? strlen(prefix) : 0);
	int suffixLen = (suffix ? strlen(suffix) : 0);
	int strLen = (str ? strlen(str) : 0);
	char *result;
	if(prefixLen)
	{
		result = (char *)ILMalloc(strLen + prefixLen + suffixLen + 1);
		if(!result)
		{
			ILGenOutOfMemory(info);
		}
		strcpy(result, prefix);
		if(strLen)
		{
			strcat(result, str);
		}
		if(suffixLen)
		{
			strcat(result, suffix);
		}
		ILFree(str);
	}
	else
	{
		result = (char *)ILRealloc
			(str, strLen + prefixLen + suffixLen + 1);
		if(!result)
		{
			ILGenOutOfMemory(info);
		}
		if(suffixLen)
		{
			strcat(result, suffix);
		}
	}
	return result;
}

char *CTypeToName(ILGenInfo *info, ILType *type)
{
	const char *cname;
	char *name;
	ILType *stripped;
	int modFlags;
	static const char * const beforeModifiers[] =
		{0, "const ", "volatile ", "const volatile "};
	static const char * const afterModifiers[] =
		{0, " const", " volatile", " const volatile"};

	/* Determine what kind of C type we have */
	if(ILType_IsPrimitive(type))
	{
		/* Recognise the primitive C types */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:		cname = "void"; break;
			case IL_META_ELEMTYPE_BOOLEAN:	cname = "_Bool"; break;
			case IL_META_ELEMTYPE_I1:		cname = "char"; break;
			case IL_META_ELEMTYPE_U1:		cname = "unsigned char"; break;
			case IL_META_ELEMTYPE_I2:		cname = "short"; break;
			case IL_META_ELEMTYPE_U2:		cname = "unsigned short"; break;
			case IL_META_ELEMTYPE_CHAR:		cname = "__wchar__"; break;
			case IL_META_ELEMTYPE_I4:		cname = "int"; break;
			case IL_META_ELEMTYPE_U4:		cname = "unsigned int"; break;
			case IL_META_ELEMTYPE_I:		cname = "long"; break;
			case IL_META_ELEMTYPE_U:		cname = "unsigned long"; break;
			case IL_META_ELEMTYPE_I8:		cname = "long long"; break;
			case IL_META_ELEMTYPE_U8:		cname = "unsigned long long"; break;
			case IL_META_ELEMTYPE_R4:		cname = "float"; break;
			case IL_META_ELEMTYPE_R8:		cname = "double"; break;
			default:						cname = 0; break;
		}
		if(cname)
		{
			name = ILDupString(cname);
			if(!name)
			{
				ILGenOutOfMemory(info);
			}
			return name;
		}
	}
	else if(ILType_IsValueType(type))
	{
		/* Recognise C value types */
		cname = ILClass_Name(ILType_ToValueType(type));
		if(CTypeIsStruct(type))
		{
			return AppendThree(info, "struct ", 0, cname);
		}
		else if(CTypeIsUnion(type))
		{
			return AppendThree(info, "union ", 0, cname);
		}
		else if(CTypeIsEnum(type))
		{
			return AppendThree(info, "enum ", 0, cname);
		}
		else if(!strncmp(cname, "array ", 6))
		{
			ILNode *sizeNode = (ILNode *)ILClassGetUserData
					(ILType_ToValueType(type));
			if(CTypeIsOpenArray(type))
			{
				return FormatArrayName
					(info, CTypeGetElemType(type), 0, sizeNode, 1);
			}
			else
			{
				return FormatArrayName
					(info, CTypeGetElemType(type), CTypeGetNumElems(type),
					 sizeNode, 0);
			}
		}
		else if(!strcmp(cname, "ArgIterator"))
		{
			cname = ILClass_Namespace(ILType_ToValueType(type));
			if(cname != 0 && !strcmp(cname, "System"))
			{
				/* "System.ArgIterator" is known to C as "__builtin_va_list" */
				name = ILDupString("__builtin_va_list");
				if(!name)
				{
					ILGenOutOfMemory(info);
				}
				return name;
			}
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		/* Recognise complex C types */
		if(ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
		{
			/* Convert a pointer type */
			return AppendThree
				(info, 0, CTypeToName(info, ILType_Ref(type)), " *");
		}
		else if(ILType_Kind(type) == IL_TYPE_COMPLEX_CMOD_OPT ||
		        ILType_Kind(type) == IL_TYPE_COMPLEX_CMOD_REQD)
		{
			/* Look for "const" and "volatile" qualifiers */
			stripped = ILTypeStripPrefixes(type);
			modFlags = 0;
			if(CheckForModifier(type, "IsConst",
							    "OpenSystem.C"))
			{
				modFlags |= 1;
			}
			if(CheckForModifier(type, "IsVolatile",
						        "System.Runtime.CompilerServices"))
			{
				modFlags |= 2;
			}
			name = CTypeToName(info, stripped);
			if(modFlags != 0)
			{
				if(CTypeIsPointer(stripped) || CTypeIsArray(stripped))
				{
					/* Put the qualifiers after the type, not before */
					name = AppendThree(info, 0, name,
									   afterModifiers[modFlags]);
				}
				else
				{
					/* Put the qualifiers before the type */
					return AppendThree(info, beforeModifiers[modFlags],
									   name, 0);
				}
			}
			return name;
		}
		else if(ILType_IsMethod(type))
		{
			/* Convert a method pointer type */
			/* TODO */
		}
	}

	/* If we get here, then the type is foreign to C */
	name = ILTypeToName(type);
	if(!name)
	{
		ILGenOutOfMemory(info);
	}
	return name;
}

ILType *CTypeFromCSharp(ILGenInfo *info, const char *assembly, ILNode *node)
{
	const char *name;
	const char *namespace;
	ILType *type;
	ILClass *classInfo;
	const char *assemblyName;

	/* Break the identifier into name and namespace */
	if(yyisa(node, ILNode_Identifier))
	{
		name = ILQualIdentName(node, 0);
		namespace = 0;

		/* Recognise C# builtin types that aren't C keywords */
		if(!strcmp(name, "bool"))
		{
			return ILType_Boolean;
		}
		else if(!strcmp(name, "byte"))
		{
			return ILType_UInt8;
		}
		else if(!strcmp(name, "sbyte"))
		{
			return ILType_Int8;
		}
		else if(!strcmp(name, "ushort"))
		{
			return ILType_UInt16;
		}
		else if(!strcmp(name, "uint"))
		{
			return ILType_UInt32;
		}
		else if(!strcmp(name, "ulong"))
		{
			return ILType_UInt64;
		}
	}
	else
	{
		name = ((ILNode_QualIdent *)node)->name;
		namespace = ILQualIdentName(((ILNode_QualIdent *)node)->left, 0);
	}

	/* Make sure that we have the specified assembly loaded */
	if(assembly)
	{
		if(!CCLoadLibrary(assembly))
		{
			return 0;
		}
	}

	/* Look up the type (returns NULL if not found) */
	type = ILFindNonSystemType(info, name, namespace);
	if(!type)
	{
		return 0;
	}
	classInfo = ILTypeToClass(info, type);
	if(!classInfo)
	{
		return 0;
	}

	/* Make sure that we found the right assembly */
	if(assembly)
	{
		assemblyName = ILImageGetAssemblyName
			(ILProgramItem_Image(ILClassResolve(classInfo)));
		if(!assemblyName || strcmp(assemblyName, assembly) != 0)
		{
			return 0;
		}
	}

	/* Convert builtin classes to their primitive forms */
	return ILClassToType(classInfo);
}

ILField *CTypeNextField(ILType *type, ILField *field)
{
	type = ILTypeStripPrefixes(type);
	if(ILType_IsValueType(type))
	{
		while((field = (ILField *)ILClassNextMemberByKind
				(ILType_ToValueType(type), (ILMember *)field,
				 IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if(!ILField_IsStatic(field))
			{
				return field;
			}
		}
	}
	return 0;
}

/*
 * Get the layout information for a "struct" or "union" type.
 */
static void StructOrUnionLayout(ILType *type, int isUnion,
								CTypeLayoutInfo *info)
{
	ILField *field;
	ILType *fieldType;
	CTypeLayoutInfo fieldInfo;
	int first, wasFirst;
	ILUInt32 maxAlign;

	/* If the type is a reference, then it has no layout */
	if(!CTypeAlreadyDefined(type))
	{
		info->category = C_TYPECAT_NO_LAYOUT;
		info->size = CTYPE_UNKNOWN;
		info->alignFlags = C_ALIGN_UNKNOWN;
		info->measureType = type;
		return;
	}

	/* Initialize the category information */
	info->category = C_TYPECAT_FIXED_BIT;
	info->size = 0;
	info->alignFlags = 0;
	info->measureType = type;

	/* Scan the fields and inspect their categories */
	first = 1;
	field = 0;
	while((field = CTypeNextField(type, field)) != 0)
	{
		/* Get the field's type layout information */
		fieldType = ILFieldGetTypeWithPrefixes(field);
		CTypeGetLayoutInfo(fieldType, &fieldInfo);
		wasFirst = first;
		first = 0;

		/* Merge the field's alignment with the overall type alignment */
		info->alignFlags |= fieldInfo.alignFlags;

		/* Handle the non-fixed categories */
		if(fieldInfo.category == C_TYPECAT_NO_LAYOUT)
		{
			info->category |= C_TYPECAT_NO_LAYOUT_BIT;
			continue;
		}
		else if(fieldInfo.category == C_TYPECAT_COMPLEX)
		{
			info->category |= C_TYPECAT_COMPLEX_BIT;
			continue;
		}
		else if(fieldInfo.category == C_TYPECAT_DYNAMIC)
		{
			info->category |= C_TYPECAT_DYNAMIC_BIT;
			continue;
		}

		/* Get the maximum alignment for this field */
		maxAlign = CTypeGetMaxAlign(fieldInfo.alignFlags);
		if(!maxAlign)
		{
			/* The alignment is unknown, so make the result dynamic */
			info->category |= C_TYPECAT_DYNAMIC_BIT;
			continue;
		}

		/* We handle unions and structures slightly differently */
		if(isUnion)
		{
			/* Update the size to reflect the maximum of all union arms */
			if(wasFirst || fieldInfo.size > info->size)
			{
				info->size = fieldInfo.size;
			}
		}
		else
		{
			if((info->size % maxAlign) != 0)
			{
				/* The structure is not aligned correctly */
				info->category |= C_TYPECAT_DYNAMIC_BIT;
				info->size += maxAlign - (info->size % maxAlign);
			}
			info->size += fieldInfo.size;
		}
	}
	if(first)
	{
		/* The CLI specs require empty structs to have a size of 1, not 0 */
		info->size = 1;
		info->alignFlags |= C_ALIGN_BYTE;
	}

	/* Align the entire structure on its maximal alignment */
	maxAlign = CTypeGetMaxAlign(info->alignFlags);
	if(!maxAlign || (info->size % maxAlign) != 0)
	{
		info->category |= C_TYPECAT_DYNAMIC_BIT;
	}

	/* Get the final type category */
	if((info->category & C_TYPECAT_NO_LAYOUT_BIT) != 0)
	{
		info->category = C_TYPECAT_NO_LAYOUT;
		info->size = CTYPE_UNKNOWN;
	}
	else if((info->category & C_TYPECAT_COMPLEX_BIT) != 0)
	{
		info->category = C_TYPECAT_COMPLEX;
		info->size = CTYPE_DYNAMIC;
	}
	else if((info->category & C_TYPECAT_DYNAMIC_BIT) != 0)
	{
		info->category = C_TYPECAT_DYNAMIC;
		info->size = CTYPE_DYNAMIC;
	}
	else
	{
		info->category = C_TYPECAT_FIXED;
	}
}

void CTypeGetLayoutInfo(ILType *type, CTypeLayoutInfo *info)
{
	ILClassLayout *clayout;

	/* Strip the prefixes from the type */
	type = ILTypeStripPrefixes(type);

	/* Determine what to do based on the type category */
	if(ILType_IsPrimitive(type))
	{
		/* Get the size and alignment information for a primitive type */
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
				info->category = C_TYPECAT_FIXED;
				info->size = 1;
				info->alignFlags = C_ALIGN_BYTE;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
				info->category = C_TYPECAT_FIXED;
				info->size = 2;
				info->alignFlags = C_ALIGN_SHORT;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
				info->category = C_TYPECAT_FIXED;
				info->size = 4;
				info->alignFlags = C_ALIGN_INT;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
				info->category = C_TYPECAT_FIXED;
				info->size = 8;
				info->alignFlags = C_ALIGN_LONG;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
				info->category = C_TYPECAT_DYNAMIC;
				info->size = CTYPE_DYNAMIC;
				info->alignFlags = C_ALIGN_POINTER;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_R4:
				info->category = C_TYPECAT_FIXED;
				info->size = 4;
				info->alignFlags = C_ALIGN_FLOAT;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_R8:
				info->category = C_TYPECAT_FIXED;
				info->size = 8;
				info->alignFlags = C_ALIGN_DOUBLE;
				info->measureType = type;
				return;

			case IL_META_ELEMTYPE_R:
			case IL_META_ELEMTYPE_TYPEDBYREF:
				info->category = C_TYPECAT_DYNAMIC;
				info->size = CTYPE_DYNAMIC;
				info->alignFlags = C_ALIGN_UNKNOWN;
				info->measureType = type;
				return;
		}
	}
	else if(CTypeIsEnum(type))
	{
		/* Get layout information for an enumerated type */
		if(CTypeAlreadyDefined(type))
		{
			/* Use the underlying type to get the actual layout */
			CTypeGetLayoutInfo(ILTypeGetEnumType(type), info);
		}
		else
		{
			/* The enum is not defined, so it has no layout */
			info->category = C_TYPECAT_NO_LAYOUT;
			info->size = CTYPE_UNKNOWN;
			info->alignFlags = C_ALIGN_UNKNOWN;
			info->measureType = type;
		}
		return;
	}
	else if(CTypeIsStruct(type))
	{
		/* Get layout information for a struct type */
		StructOrUnionLayout(type, 0, info);
		return;
	}
	else if(CTypeIsUnion(type))
	{
		/* Get layout information for a union type */
		StructOrUnionLayout(type, 1, info);
		return;
	}
	else if(CTypeIsOpenArray(type))
	{
		/* Get layout information for an open array type */
		CTypeGetLayoutInfo(CTypeGetElemType(type), info);
		info->category = C_TYPECAT_NO_LAYOUT;
		info->size = CTYPE_UNKNOWN;
		info->measureType = type;
		return;
	}
	else if(CTypeIsArray(type))
	{
		/* Get layout information for a regular array type */
		CTypeGetLayoutInfo(CTypeGetElemType(type), info);
		if(info->category == C_TYPECAT_FIXED)
		{
			/* Get the fixed size information from the array type */
			clayout = ILClassLayoutGetFromOwner(ILType_ToValueType(type));
			if(clayout)
			{
				info->size = ILClassLayoutGetClassSize(clayout);
			}
			else
			{
				/* If we don't have a class size, then the size of
				   the array is a complex expression */
				info->category = C_TYPECAT_COMPLEX;
				info->size = CTYPE_DYNAMIC;
			}
		}
		else if(info->category != C_TYPECAT_NO_LAYOUT)
		{
			/* Arrays with dynamic or complex element types are complex */
			info->category = C_TYPECAT_COMPLEX;
			info->size = CTYPE_DYNAMIC;
		}
		info->measureType = type;
		return;
	}
	else if(ILType_IsValueType(type))
	{
		/* Foreign value type that was imported from another language.
		   We have to assume that it is in another assembly and that
		   the definition may change in the future, or that the definition
		   differs from one CLR to another.  Therefore, both the size
		   and the alignment are unknown at compile time */
		info->category = C_TYPECAT_DYNAMIC;
		info->size = CTYPE_DYNAMIC;
		info->alignFlags = C_ALIGN_UNKNOWN;
		info->measureType = type;
		return;
	}

	/* If we get here, then assume that the type is a pointer or
	   object reference that has pointer alignment and dynamic layout.
	   We use "System.IntPtr" as the measuring type because the
	   "sizeof" IL opcode may not work on the actual type */
	info->category = C_TYPECAT_DYNAMIC;
	info->size = CTYPE_DYNAMIC;
	info->alignFlags = C_ALIGN_POINTER;
	info->measureType = ILType_Int;
}

ILUInt32 CTypeGetMaxAlign(ILUInt32 alignFlags)
{
	ILUInt32 size = 1;
	if((alignFlags & (C_ALIGN_SHORT | C_ALIGN_2)) != 0)
	{
		size = 2;
	}
	if((alignFlags & (C_ALIGN_INT | C_ALIGN_FLOAT | C_ALIGN_4)) != 0)
	{
		size = 4;
	}
	if((alignFlags & (C_ALIGN_LONG | C_ALIGN_DOUBLE |
					  C_ALIGN_POINTER | C_ALIGN_8)) != 0)
	{
		size = 8;
	}
	if((alignFlags & C_ALIGN_16) != 0)
	{
		size = 16;
	}
	if((alignFlags & C_ALIGN_UNKNOWN) != 0)
	{
		size = 0;
	}
	return size;
}

/*
 * Get the size node for a "struct" or "union" type.
 */
static ILNode *StructOrUnionSizeNode(ILType *type, int isUnion,
									 ILUInt32 overallAlignment,
									 const char *stopAt)
{
	ILNode *size = 0;
	ILField *field;
	ILType *fieldType;
	CTypeLayoutInfo fieldInfo;
	ILUInt32 alignFlags = 0;
	ILNode *fieldSize;
	int changedAlignment = 0;
	int first = 1;
	ILNode *alignVar = 0;

	/* If the type contains fields with unknown alignment,
	   then we will need a temporary variable to collect up
	   the actual alignment flags at runtime */
	if((overallAlignment & C_ALIGN_UNKNOWN) != 0 && !stopAt)
	{
		alignVar = ILNode_CSizeTempVar_create();
	}

	/* Scan the fields and collect up their sizes */
	field = 0;
	while((field = CTypeNextField(type, field)) != 0)
	{
		/* Get the layout information for the field */
		fieldType = ILFieldGetTypeWithPrefixes(field);
		CTypeGetLayoutInfo(fieldType, &fieldInfo);

		/* Merge the field's alignment flags with the total alignment */
		if(fieldInfo.alignFlags != alignFlags && !first)
		{
			alignFlags |= fieldInfo.alignFlags;
			changedAlignment = 1;
		}
		else
		{
			alignFlags |= fieldInfo.alignFlags;
		}
		first = 0;

		/* Is this the field that we need to stop at? */
		if(stopAt && !strcmp(ILField_Name(field), stopAt))
		{
			/* Align the field appropriately and then exit */
			if(size && changedAlignment)
			{
				size = ILNode_CSizeAlign_create
					(size, fieldInfo.alignFlags,
					 fieldInfo.measureType, alignVar);
			}
			return size;
		}

		/* Create the size node for the field type */
		fieldSize = CTypeCreateSizeNode(fieldType);

		/* Alignment is different for unions and structures */
		if(isUnion)
		{
			if(!size)
			{
				size = fieldSize;
			}
			else
			{
				size = ILNode_CSizeMax_create(size, fieldSize);
			}
		}
		else if(!size)
		{
			size = fieldSize;
		}
		else if(changedAlignment)
		{
			size = ILNode_Add_create
				(ILNode_CSizeAlign_create
					(size, fieldInfo.alignFlags,
					 fieldInfo.measureType, alignVar),
				 fieldSize);
		}
		else
		{
			size = ILNode_Add_create(size, fieldSize);
		}
	}
	if(!size)
	{
		/* If there are no fields, then the default size is 1 */
		ILNode_UInt32_create((ILUInt64)1, 0, 1);
	}

	/* Align the entire structure if the fields have differing alignments */
	if(changedAlignment)
	{
		size = ILNode_CSizeAlign_create
			(size, overallAlignment, 0, alignVar);
	}

	/* Release the temporary variable once we know the complete size */
	if(alignVar)
	{
		size = ILNode_CSizeReleaseTempVar_create(size, alignVar);
	}

	/* Return the final size node to the caller */
	return size;
}

ILNode *CTypeCreateSizeNode(ILType *type)
{
	CTypeLayoutInfo info;
	ILClassLayout *clayout;
	ILNode *arraySize;
	ILNode *elemSize;

	/* Get the layout information for the type */
	CTypeGetLayoutInfo(type, &info);

	/* If the type is not "complex", then getting the size is easy */
	if(info.category == C_TYPECAT_FIXED)
	{
		return ILNode_UInt32_create((ILUInt64)(info.size), 0, 1);
	}
	else if(info.category == C_TYPECAT_DYNAMIC)
	{
		return ILNode_CSizeOfRaw_create(info.measureType);
	}
	else if(info.category == C_TYPECAT_NO_LAYOUT)
	{
		return 0;
	}

	/* Determine the kind of complex layout to be performed */
	type = ILTypeStripPrefixes(type);
	if(CTypeIsStruct(type))
	{
		return StructOrUnionSizeNode(type, 0, info.alignFlags, 0);
	}
	else if(CTypeIsUnion(type))
	{
		return StructOrUnionSizeNode(type, 1, info.alignFlags, 0);
	}
	else if(CTypeIsArray(type))
	{
		clayout = ILClassLayoutGetFromOwner(ILType_ToValueType(type));
		if(clayout)
		{
			/* Use the pre-computed size on the array */
			return ILNode_UInt32_create
				((ILUInt64)ILClassLayoutGetClassSize(clayout), 0, 1);
		}
		arraySize = CTypeGetComplexArraySize(type);
		elemSize = CTypeCreateSizeNode(CTypeGetElemType(type));
		return ILNode_Mul_create(arraySize, elemSize);
	}

	/* If we get here, then we don't know how to compute the size */
	return 0;
}

ILNode *CTypeCreateOffsetNode(ILType *type, const char *name)
{
	type = ILTypeStripPrefixes(type);
	if(CTypeIsStruct(type))
	{
		CTypeLayoutInfo info;
		CTypeGetLayoutInfo(type, &info);
		return StructOrUnionSizeNode(type, 0, info.alignFlags, name);
	}
	return 0;
}

ILNode *CTypeGetComplexArraySize(ILType *type)
{
	type = ILTypeStripPrefixes(type);
	if(CTypeIsArray(type))
	{
		return (ILNode *)ILClassGetUserData(ILType_ToValueType(type));
	}
	return 0;
}

ILField *CTypeGetMeasureField(ILGenInfo *info, ILType *type)
{
	char *name;
	ILClass *classInfo;
	ILField *field;

	/* Get the name of the alignment type */
	type = ILTypeStripPrefixes(type);
	name = AppendThree(info, "align ", CTypeToName(info, type), 0);

	/* See if we already have the alignment type in the program */
	classInfo = ILClassLookup(ILClassGlobalScope(info->image), name, 0);
	if(!classInfo)
	{
		/* Create the alignment measuring type for the first time */
		classInfo = ILType_ToClass(ILFindSystemType(info, "ValueType"));
		classInfo = ILClassCreate(ILClassGlobalScope(info->image), 0,
							  	  name, 0, ILToProgramItem(classInfo));
		if(!classInfo)
		{
			ILGenOutOfMemory(info);
		}
		ILClassSetAttrs(classInfo, ~((ILUInt32)0),
						IL_META_TYPEDEF_PUBLIC |
						IL_META_TYPEDEF_SERIALIZABLE |
						IL_META_TYPEDEF_LAYOUT_SEQUENTIAL |
						IL_META_TYPEDEF_SEALED);

		/* Add the "pad" field, which has type "byte" */
		field = ILFieldCreate(classInfo, 0, "pad", IL_META_FIELDDEF_PUBLIC);
		if(!field)
		{
			ILGenOutOfMemory(info);
		}
		ILMemberSetSignature((ILMember *)field, ILType_UInt8);

		/* Add the "value" field, which has the type being measured */
		field = ILFieldCreate(classInfo, 0, "value", IL_META_FIELDDEF_PUBLIC);
		if(!field)
		{
			ILGenOutOfMemory(info);
		}
		ILMemberSetSignature((ILMember *)field, type);

		/* Mark the type for output */
		CTypeMarkForOutput(info, ILType_FromValueType(classInfo));

		/* Return the "value" field to the caller */
		return field;
	}

	/* Look up the "value" field and return it */
	type = ILType_FromValueType(classInfo);
	field = 0;
	while((field = CTypeNextField(type, field)) != 0)
	{
		if(!strcmp(ILField_Name(field), "value"))
		{
			break;
		}
	}
	return field;
}

int CTypeIsComplex(ILType *type)
{
	CTypeLayoutInfo layout;
	CTypeGetLayoutInfo(type, &layout);
	return (layout.category == C_TYPECAT_COMPLEX);
}

ILType *CTypeGetParam(ILType *signature, unsigned long param)
{
	ILType *type = ILTypeGetParamWithPrefixes(signature, param);
	if(CTypeIsComplexPointer(type))
	{
		return CTypeGetPtrRef(type);
	}
	else
	{
		return type;
	}
}

ILType *CTypeGetReturn(ILType *signature)
{
	ILType *type = ILTypeGetReturnWithPrefixes(signature);
	if(CTypeIsComplexPointer(type))
	{
		return CTypeGetPtrRef(type);
	}
	else
	{
		return type;
	}
}

#ifdef	__cplusplus
};
#endif
