/*
 * meta_types.c - Type handling for IL images.
 *
 * Copyright (C) 2001, 2008  Southern Storm Software, Pty Ltd.
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

#ifdef	__cplusplus
extern	"C" {
#endif

ILType *ILTypeCreateRef(ILContext *context, int kind, ILType *refType)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = kind;
		type->un.refType__ = refType;
	}
	return type;
}

ILType *ILTypeCreateArray(ILContext *context, unsigned long rank,
						  ILType *elem)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = IL_TYPE_COMPLEX_ARRAY;
		type->un.array__.elemType__ = elem;
		type->un.array__.size__ = 0;
		type->un.array__.lowBound__ = 0;
		while(rank > 1)
		{
			elem = type;
			type = ILMemPoolCalloc(&(context->typePool), ILType);
			if(!type)
			{
				return 0;
			}
			type->kind__ = IL_TYPE_COMPLEX_ARRAY_CONTINUE;
			type->un.array__.elemType__ = elem;
			type->un.array__.size__ = 0;
			type->un.array__.lowBound__ = 0;
			--rank;
		}
	}
	return type;
}

ILType *ILTypeFindOrCreateArray(ILContext *context, unsigned long rank,
						        ILType *elem)
{
	ILType *type;
	ILType dims[8];
	unsigned long index;
	ILClass *info;

	/* Bail out if too many dimensions to be worth checking */
	if(rank > 8)
	{
		return ILTypeCreateArray(context, rank, elem);
	}

	/* Create a pseudo type on the stack, for performing hash lookups */
	ILMemZero(dims, sizeof(ILType) * rank);
	for(index = 0; index < (rank - 1); ++index)
	{
		dims[index].kind__ = IL_TYPE_COMPLEX_ARRAY_CONTINUE;
		dims[index].un.array__.elemType__ = &(dims[index + 1]);
		dims[index].un.array__.size__ = 0;
		dims[index].un.array__.lowBound__ = 0;
	}
	dims[index].kind__ = IL_TYPE_COMPLEX_ARRAY;
	dims[index].un.array__.elemType__ = elem;
	dims[index].un.array__.size__ = 0;
	dims[index].un.array__.lowBound__ = 0;
	type = &(dims[0]);

	/* Look for the type in the synthetic hash */
	info = ILHashFindType(context->syntheticHash, type, ILClass);
	if(info)
	{
		return info->synthetic;
	}

	/* Create the type for the first time */
	return ILTypeCreateArray(context, rank, elem);
}

void ILTypeSetSize(ILType *array, unsigned long dimension, long value)
{
	while(dimension > 0)
	{
		array = array->un.array__.elemType__;
		--dimension;
	}
	array->un.array__.size__ = value;
}

void ILTypeSetLowBound(ILType *array, unsigned long dimension, long value)
{
	while(dimension > 0)
	{
		array = array->un.array__.elemType__;
		--dimension;
	}
	array->un.array__.lowBound__ = value;
}

ILType *ILTypeCreateModifier(ILContext *context, ILType *list,
							 int kind, ILClass *info)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = kind;
		type->un.modifier__.info__ = info;
		type->un.modifier__.type__ = 0;
		if(!list)
		{
			/* The list was empty, so create a new list */
			return type;
		}
		else
		{
			/* Append the modifier to the list */
			ILType *temp = list;
			while(temp->un.modifier__.type__ != 0)
			{
				temp = temp->un.modifier__.type__;
			}
			temp->un.modifier__.type__ = type;
			return list;
		}
	}
	else
	{
		return 0;
	}
}

ILType *ILTypeAddModifiers(ILContext *context, ILType *modifiers, ILType *type)
{
	ILType *temp = modifiers;
	while(temp->un.modifier__.type__ != 0)
	{
		temp = temp->un.modifier__.type__;
	}
	temp->un.modifier__.type__ = type;
	return modifiers;
}

ILType *ILTypeCreateLocalList(ILContext *context)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = IL_TYPE_COMPLEX_LOCALS;
		type->num__ = 0;
		return type;
	}
	else
	{
		return 0;
	}
}

int ILTypeAddLocal(ILContext *context, ILType *locals, ILType *type)
{
	unsigned short num;
	ILType **end;
	ILType *start;

	/* Find the end of the current local list */
	num = locals->num__;
	end = 0;
	start = locals;
	while(num >= 4)
	{
		end = &(locals->un.locals__.next__);
		locals = locals->un.locals__.next__;
		num -= 4;
	}

	/* Try to convert the type to a primitive type */
	/* only value types are primitive types in dotGNU */
	if(ILType_IsValueType(type))
	{
		ILType *tempType = ILClassToPrimitiveType(ILType_ToValueType(type));

		if(tempType)
		{
			type = tempType;
		}
	}

	/* Will it fit in the current block? */
	if(locals)
	{
		locals->un.locals__.local__[num] = type;
		++(start->num__);
		return 1;
	}

	/* Allocate and initialize a new overflow block */
	*end = ILMemPoolCalloc(&(context->typePool), ILType);
	if(!(*end))
	{
		return 0;
	}
	(*end)->un.locals__.local__[0] = type;
	++(start->num__);
	return 1;
}

unsigned long ILTypeNumLocals(ILType *locals)
{
	if(locals->kind__ == IL_TYPE_COMPLEX_LOCALS)
	{
		return (unsigned long)(locals->num__);
	}
	else
	{
		return 0;
	}
}

ILType *ILTypeGetLocal(ILType *locals, unsigned long index)
{
	return ILTypeStripPrefixes(ILTypeGetLocalWithPrefixes(locals, index));
}

ILType *ILTypeGetLocalWithPrefixes(ILType *locals, unsigned long index)
{
	while(locals != 0 && index >= 4)
	{
		index -= 4;
		locals = locals->un.locals__.next__;
	}
	if(locals)
	{
		return locals->un.locals__.local__[index];
	}
	else
	{
		return 0;
	}
}

ILType *ILTypeCreateMethod(ILContext *context, ILType *returnType)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = IL_TYPE_COMPLEX_METHOD;
		type->num__ = 0;

		/* Try to convert the return type to a primitive type */
		/* only value types are primitive types in dotGNU */
		if(ILType_IsValueType(returnType))
		{
			ILType *tempType;

			tempType = ILClassToPrimitiveType(ILType_ToValueType(returnType));
			if(tempType)
			{
				returnType = tempType;
			}
		}

		type->un.method__.retType__ = returnType;
	}
	return type;
}

ILType *ILTypeCreateProperty(ILContext *context, ILType *propType)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = IL_TYPE_COMPLEX_PROPERTY;
		type->num__ = 0;

		/* Try to convert the property type to a primitive type */
		/* only value types are primitive types in dotGNU */
		if(ILType_IsValueType(propType))
		{
			ILType *tempType;

			tempType = ILClassToPrimitiveType(ILType_ToValueType(propType));
			if(tempType)
			{
				propType = tempType;
			}
		}

		type->un.method__.retType__ = propType;
	}
	return type;
}

int ILTypeAddParam(ILContext *context, ILType *method, ILType *paramType)
{
	unsigned short num;
	ILType **end;
	ILType *start;

	/* Try to convert the parameter type to a primitive type */
	/* only value types are primitive types in dotGNU */
	if(ILType_IsValueType(paramType))
	{
		ILType *tempType;

		tempType = ILClassToPrimitiveType(ILType_ToValueType(paramType));
		if(tempType)
		{
			paramType = tempType;
		}
	}

	/* Put the parameter in the method block if it will fit */
	if(method->num__ < 3)
	{
		method->un.method__.param__[(method->num__)++] = paramType;
		return 1;
	}

	/* Find the end of the overflow parameter list */
	num = method->num__ - 3;
	end = &(method->un.method__.next__);
	start = method;
	method = method->un.method__.next__;
	while(num >= 4)
	{
		end = &(method->un.params__.next__);
		method = method->un.params__.next__;
		num -= 4;
	}

	/* Will it fit in the current overflow block? */
	if(method)
	{
		method->un.params__.param__[num] = paramType;
		++(start->num__);
		return 1;
	}

	/* Allocate and initialize a new overflow block */
	*end = ILMemPoolCalloc(&(context->typePool), ILType);
	if(!(*end))
	{
		return 0;
	}
	(*end)->un.params__.param__[0] = paramType;
	++(start->num__);
	return 1;
}

int ILTypeAddSentinel(ILContext *context, ILType *method)
{
	ILType *paramType = ILMemPoolCalloc(&(context->typePool), ILType);
	if(paramType)
	{
		method->kind__ |= IL_TYPE_COMPLEX_METHOD_SENTINEL;
		paramType->kind__ = IL_TYPE_COMPLEX_SENTINEL;
		return ILTypeAddParam(context, method, paramType);
	}
	else
	{
		return 0;
	}
}

unsigned long ILTypeNumParams(ILType *method)
{
	if(ILType_IsMethod(method) || ILType_IsProperty(method) ||
	   ILType_IsWith(method))
	{
		return method->num__;
	}
	else
	{
		return 0;
	}
}

ILType *ILTypeGetParam(ILType *method, unsigned long index)
{
	return ILTypeStripPrefixes(ILTypeGetParamWithPrefixes(method, index));
}

ILType *ILTypeGetParamWithPrefixes(ILType *method, unsigned long index)
{
	if(!ILType_IsMethod(method) && !ILType_IsProperty(method) &&
	   !ILType_IsWith(method))
	{
		return 0;
	}
	else if(!index)
	{
		return method->un.method__.retType__;
	}
	else if(index > (unsigned long)(method->num__))
	{
		return 0;
	}
	else if(index <= 3)
	{
		return method->un.method__.param__[index - 1];
	}
	else
	{
		index -= 4;
		method = method->un.method__.next__;
		while(index >= 4)
		{
			index -= 4;
			method = method->un.params__.next__;
		}
		return method->un.params__.param__[index];
	}
}

void ILTypeSetCallConv(ILType *type, ILUInt32 callConv)
{
	if(ILType_IsMethod(type) || ILType_IsProperty(type))
	{
		type->kind__ = (short)((type->kind__ & 0xFF) | (callConv << 8));
	}
}

void ILTypeSetReturn(ILType *type, ILType *retType)
{
	if(ILType_IsMethod(type) || ILType_IsProperty(type))
	{
		/* Try to convert the return type to a primitive type */
		/* only value types are primitive types in dotGNU */
		if(ILType_IsValueType(retType))
		{
			ILType *tempType;

			tempType = ILClassToPrimitiveType(ILType_ToValueType(retType));
			if(tempType)
			{
				retType = tempType;
			}
		}

		type->un.method__.retType__ = retType;
	}
}

ILType *ILTypeGetReturn(ILType *type)
{
	if(ILType_IsMethod(type) || ILType_IsProperty(type) || ILType_IsWith(type))
	{
		return ILTypeStripPrefixes(type->un.method__.retType__);
	}
	else
	{
		return 0;
	}
}

ILType *ILTypeGetReturnWithPrefixes(ILType *type)
{
	if(ILType_IsMethod(type) || ILType_IsProperty(type) || ILType_IsWith(type))
	{
		return type->un.method__.retType__;
	}
	else
	{
		return 0;
	}
}

ILType *ILTypeStripPrefixes(ILType *type)
{
	while(type != 0 && ILType_IsComplex(type))
	{
		if(type->kind__ == IL_TYPE_COMPLEX_CMOD_REQD ||
		   type->kind__ == IL_TYPE_COMPLEX_CMOD_OPT)
		{
			type = type->un.modifier__.type__;
		}
		else if(type->kind__ == IL_TYPE_COMPLEX_PINNED)
		{
			type = type->un.refType__;
		}
		else
		{
			break;
		}
	}
	return type;
}

/*
 * Determine if two classes are identical.
 */
static int ClassIdentical(ILClass *classInfo1, ILClass *classInfo2)
{
	const char *namespace1;
	const char *namespace2;
	ILClass *parent1;
	ILClass *parent2;

	/* Resolve the classes as far as possible */
	classInfo1 = ILClassResolve(classInfo1);
	classInfo2 = ILClassResolve(classInfo2);
	if(classInfo1 == classInfo2)
	{
		return 1;
	}

	/* If there are no "redo" items, then there is no way to match */
	if(classInfo1->programItem.image->context->numRedoItems == 0)
	{
		return 0;
	}

	/* If neither is a reference, then they cannot be identical */
	if(!ILClassIsRef(classInfo1) && !ILClassIsRef(classInfo2))
	{
		return 0;
	}

	/* Check for name identity */
	if(strcmp(ILClass_Name(classInfo1), ILClass_Name(classInfo2)) != 0)
	{
		return 0;
	}
	namespace1 = ILClass_Namespace(classInfo1);
	namespace2 = ILClass_Namespace(classInfo2);
	if(namespace1 && namespace2)
	{
		if(strcmp(namespace1, namespace2) != 0)
		{
			return 0;
		}
	}
	else if(namespace1 != namespace2)
	{
		return 0;
	}

	/* Check the nesting scope levels */
	parent1 = ILClassGetNestedParent(classInfo1);
	parent2 = ILClassGetNestedParent(classInfo2);
	if(parent1 && parent2)
	{
		return ClassIdentical(parent1, parent2);
	}
	else if(parent1 != parent2)
	{
		return 0;
	}

	/* The two classes are identical, or will be after we perform
	   "redo" operations at the end of the loading process */
	return 1;
}

int ILTypeIdentical(ILType *type1, ILType *type2)
{
	unsigned long arg;

	/* Bail out early if the pointers are identical, to speed things up */
	if(type1 == type2)
	{
		return 1;
	}

	/* Strip the type prefixes */
	type1 = ILTypeStripPrefixes(type1);
	type2 = ILTypeStripPrefixes(type2);

	/* Check simple cases again */
	if(type1 == type2)
	{
		return 1;
	}
	if(!type1 || !type2)
	{
		return 0;
	}

	/* Primitive, class, and value types can be checked easily */
	if(ILType_IsPrimitive(type1))
	{
		return (type1 == type2);
	}
	else if(ILType_IsClass(type1))
	{
		if(ILType_IsClass(type2))
		{
			return ClassIdentical(ILType_ToClass(type1),
								  ILType_ToClass(type2));
		}
		else
		{
			return 0;
		}
	}
	else if(ILType_IsValueType(type1))
	{
		if(ILType_IsValueType(type2))
		{
			return ClassIdentical(ILType_ToValueType(type1),
								  ILType_ToValueType(type2));
		}
		else
		{
			return 0;
		}
	}

	/* Determine how to perform the test based on the complex type kind */
	if(!ILType_IsComplex(type1) || !ILType_IsComplex(type2) ||
	   type1->kind__ != type2->kind__)
	{
		return 0;
	}
	switch(type1->kind__)
	{
		case IL_TYPE_COMPLEX_BYREF:
		case IL_TYPE_COMPLEX_PTR:
		case IL_TYPE_COMPLEX_PINNED:
		{
			return ILTypeIdentical(type1->un.refType__, type2->un.refType__);
		}
		/* Not reached */

		case IL_TYPE_COMPLEX_ARRAY:
		case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
		{
			if(!ILTypeIdentical(type1->un.array__.elemType__,
								type2->un.array__.elemType__))
			{
				return 0;
			}
			return (type1->un.array__.size__ == type2->un.array__.size__ &&
					type1->un.array__.lowBound__ ==
							type2->un.array__.lowBound__);
		}
		/* Not reached */

		case IL_TYPE_COMPLEX_SENTINEL:
		{
			return 1;
		}
		/* Not reached */

		case IL_TYPE_COMPLEX_MVAR:
		case IL_TYPE_COMPLEX_VAR:
		{
			return (ILType_VarNum(type1) == ILType_VarNum(type2));
		}
		/* Not reached */

		default:
		{
			/* Probably a method, property, or generic type specification */
			if((type1->kind__ & 0xFF) == IL_TYPE_COMPLEX_PROPERTY ||
			   (type1->kind__ & IL_TYPE_COMPLEX_METHOD) != 0 ||
			   (type1->kind__ & 0xFF) == IL_TYPE_COMPLEX_WITH)
			{
				/* Check the property or method signature */
				if(type1->num__ != type2->num__)
				{
					return 0;
				}
#if IL_VERSION_MAJOR > 1
				/* Check if the number of generic parameters match */
				if((type1->kind__ & IL_TYPE_COMPLEX_METHOD) != 0)
				{
					if((ILType_CallConv(type1) & IL_META_CALLCONV_GENERIC) != 0)
					{
						if(ILType_NumGen(type1) != ILType_NumGen(type2))
						{
							return 0;
						}
					}
				}
#endif
				if(!ILTypeIdentical(type1->un.method__.retType__,
									type2->un.method__.retType__))
				{
					return 0;
				}
				for(arg = 1; arg <= type1->num__; ++arg)
				{
					if(!ILTypeIdentical(ILTypeGetParamWithPrefixes(type1, arg),
										ILTypeGetParamWithPrefixes(type2, arg)))
					{
						return 0;
					}
				}
				return 1;
			}
		}
		break;
	}

	/* Not identical */
	return 0;
}

static char *AppendString(char *str1, const char *str2)
{
	if(str1)
	{
		char *temp = (char *)ILRealloc(str1, strlen(str1) + strlen(str2) + 1);
		if(temp)
		{
			strcat(temp, str2);
			return temp;
		}
		ILFree(str1);
		return 0;
	}
	else
	{
		return 0;
	}
}

char *ILTypeToName(ILType *type)
{
	char *name;
	char *elemName;
	ILClass *info;
	int len;
	const char *assemName;
	char numbuf[80];
	ILType *elemType;
	unsigned long numParams;
	unsigned long param;

	/* Strip unnecessary prefixes from the type */
	type = ILTypeStripPrefixes(type);

	/* Determine what form of name we need */
	if(!type)
	{
		return 0;
	}
	else if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		/* Resolve the class to the actual assembly it resides in */
		info = (ILClass *)(_ILProgramItemResolve
								(&(ILType_ToClass(type)->programItem)));

		/* Get the name of the assembly to qualify the class name */
		if(ILClassIsRef(info))
		{
			assemName = 0;
		}
		else
		{
			assemName = ILImageGetAssemblyName(info->programItem.image);
		}

		/* How many characters do we need for the class name? */
		if(assemName)
		{
			len = strlen(assemName) + 2;
		}
		else
		{
			len = 0;
		}
		if(info->className->namespace)
		{
			len += strlen(info->className->namespace) + 1;
		}
		len += strlen(info->className->name) + 1;

		/* Build the fully-qualified class name */
		name = (char *)ILMalloc(len);
		if(name)
		{
			if(assemName)
			{
				name[0] = '[';
				strcpy(name + 1, assemName);
				len = strlen(assemName) + 1;
				name[len++] = ']';
			}
			else
			{
				len = 0;
			}
			if(info->className->namespace)
			{
				strcpy(name + len, info->className->namespace);
				len += strlen(info->className->namespace);
				name[len++] = '.';
			}
			strcpy(name + len, info->className->name);
		}
		return name;
	}
	else if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_VOID:
					return ILDupString("void");

			case IL_META_ELEMTYPE_BOOLEAN:
					return ILDupString("bool");

			case IL_META_ELEMTYPE_CHAR:
					return ILDupString("char");

			case IL_META_ELEMTYPE_I1:
					return ILDupString("sbyte");

			case IL_META_ELEMTYPE_U1:
					return ILDupString("byte");

			case IL_META_ELEMTYPE_I2:
					return ILDupString("short");

			case IL_META_ELEMTYPE_U2:
					return ILDupString("ushort");

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_I:
					return ILDupString("int");

			case IL_META_ELEMTYPE_U4:
			case IL_META_ELEMTYPE_U:
					return ILDupString("uint");

			case IL_META_ELEMTYPE_I8:
					return ILDupString("long");

			case IL_META_ELEMTYPE_U8:
					return ILDupString("ulong");

			case IL_META_ELEMTYPE_R4:
					return ILDupString("float");

			case IL_META_ELEMTYPE_R8:
			case IL_META_ELEMTYPE_R:
					return ILDupString("double");

			case IL_META_ELEMTYPE_TYPEDBYREF:
					return ILDupString("typedbyref");

			default: break;
		}
		return 0;
	}
	else
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_PTR:
			{
				name = ILTypeToName(type->un.refType__);
				return AppendString(name, " *");
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_BYREF:
			{
				name = ILTypeToName(type->un.refType__);
				return AppendString(name, " &");
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				/* Find the element type */
				elemType = type->un.array__.elemType__;
				while(ILType_IsComplex(elemType) &&
				      (elemType->kind__ == IL_TYPE_COMPLEX_ARRAY ||
					   elemType->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE))
				{
					elemType = elemType->un.array__.elemType__;
				}

				/* Convert the element type into a name */
				name = AppendString(ILTypeToName(elemType), "[");

				/* Add the rank specifiers */
				while(type != elemType)
				{
					if(!(type->un.array__.size__) &&
					   type->un.array__.lowBound__)
					{
						sprintf(numbuf, "%ld...", type->un.array__.lowBound__);
						name = AppendString(name, numbuf);
					}
					else if(!(type->un.array__.lowBound__) &&
							type->un.array__.size__)
					{
						sprintf(numbuf, "%ld", type->un.array__.size__);
						name = AppendString(name, numbuf);
					}
					else if(type->un.array__.size__ &&
							type->un.array__.lowBound__)
					{
						sprintf(numbuf, "%ld...%ld",
								type->un.array__.lowBound__,
								type->un.array__.lowBound__ +
									type->un.array__.size__ - 1);
						name = AppendString(name, numbuf);
					}
					if(type->kind__ == IL_TYPE_COMPLEX_ARRAY)
					{
						type = type->un.array__.elemType__;
						if(type != elemType)
						{
							name = AppendString(name, "][");
						}
						else
						{
							name = AppendString(name, "]");
						}
					}
					else
					{
						name = AppendString(name, ",");
						type = type->un.array__.elemType__;
					}
				}

				/* Return the final name to the caller */
				return name;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_METHOD:
			{
				ILType *returnType = ILTypeGetReturn(type);

				if(returnType == ILType_Invalid)
				{
					name = ILDupString("");
				}
				else
				{
					name = ILTypeToName(returnType);
				}
				if(ILType_HasThis(type))
				{
					name = AppendString(name, " * instance (");
				}
				else
				{
					name = AppendString(name, " * (");
				}
				numParams = ILTypeNumWithParams(type);
				for(param = 1; param <= numParams; ++param)
				{
					if(param != 1)
					{
						name = AppendString(name, ", ");
					}
					elemType = ILTypeGetParam(type, param);
					elemName = ILTypeToName(elemType);
					if(!elemName)
					{
						if(name)
						{
							ILFree(name);
						}
						return 0;
					}
					name = AppendString(name, elemName);
					ILFree(elemName);
				}
				return AppendString(name, ")");
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_WITH:
			{
				name = ILTypeToName(ILTypeGetWithMain(type));
				name = AppendString(name, "<");
				numParams = ILTypeNumWithParams(type);
				for(param = 1; param <= numParams; ++param)
				{
					if(param != 1)
					{
						name = AppendString(name, ", ");
					}
					elemType = ILTypeGetWithParam(type, param);
					elemName = ILTypeToName(elemType);
					if(!elemName)
					{
						if(name)
						{
							ILFree(name);
						}
						return 0;
					}
					name = AppendString(name, elemName);
					ILFree(elemName);
				}
				return AppendString(name, ">");
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_MVAR:
			{
				sprintf(numbuf, "!!%d", ILType_VarNum(type));
				return ILDupString(numbuf);
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_VAR:
			{
				sprintf(numbuf, "!%d", ILType_VarNum(type));
				return ILDupString(numbuf);
			}
			/* Not reached */

			default: break;
		}
		return 0;
	}
}

ILType *ILTypeGetEnumType(ILType *type)
{
	ILClass *classInfo = 0;

	if(ILType_IsWith(type))
	{		
		ILType *valueType = ILTypeGetWithMain(type);

		if(ILType_IsValueType(valueType))
		{
			ILClass *genInfo = ILClassResolve(ILType_ToValueType(valueType));

			classInfo = ILClassInstantiate(ILClassToImage(genInfo), type, type, 0);
			if(!classInfo)
			{
				return 0;
			}
		}
	}
	else if(ILType_IsValueType(type))
	{
		classInfo = ILClassResolve(ILType_ToValueType(type));
	}
	if(classInfo)
	{
		ILClass *parent = ILClass_UnderlyingParentClass(classInfo);
		if(parent)
		{
			const char *namespace = ILClass_Namespace(parent);
			if(namespace && !strcmp(namespace, "System") &&
			   !strcmp(ILClass_Name(parent), "Enum"))
			{
				ILField *field = 0;
				while((field = (ILField *)ILClassNextMemberByKind
							(classInfo, (ILMember *)field,
							 IL_META_MEMBERKIND_FIELD)) != 0)
				{
					if(!ILField_IsStatic(field) &&
					   ILField_HasRTSpecialName(field))
					{
						return ILField_Type(field);
					}
				}
			}
		}
	}
	return type;
}

ILType *ILTypeGetElemType(ILType *type)
{
	while(type != 0 && ILType_IsComplex(type) &&
		  type->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
	{
		type = type->un.array__.elemType__;
	}
	if(type != 0 && ILType_IsComplex(type))
	{
		if(type->kind__ == IL_TYPE_COMPLEX_ARRAY)
		{
			return type->un.array__.elemType__;
		}
	}
	return 0;
}

int ILTypeGetRank(ILType *type)
{
	int rank = 0;
	while(type != 0 && ILType_IsComplex(type) &&
		  type->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
	{
		++rank;
		type = type->un.array__.elemType__;
	}
	if(type != 0 && ILType_IsComplex(type))
	{
		if(type->kind__ == IL_TYPE_COMPLEX_ARRAY)
		{
			++rank;
		}
	}
	return rank;
}

int ILTypeIsStringClass(ILType *type)
{
	ILClass *info;
	ILImage *systemImage;
	if(ILType_IsClass(type))
	{
		/* Check the name against "System.String" */
		info = ILClassResolve(ILType_ToClass(type));
		if(!strcmp(info->className->name, "String") &&
		   info->className->namespace &&
		   !strcmp(info->className->namespace, "System"))
		{
			/* Check that it is within the system image, to prevent
			   applications from fooling us into believing that their
			   own class is the system's string class */
			info = ILClassResolve(info);
			systemImage = info->programItem.image->context->systemImage;
			if(!systemImage || systemImage == info->programItem.image)
			{
				return 1;
			}
		}
	}
	return 0;
}

int ILTypeIsObjectClass(ILType *type)
{
	ILClass *info;
	ILImage *systemImage;
	if(ILType_IsClass(type))
	{
		/* Check the name against "System.Object" */
		info = ILClassResolve(ILType_ToClass(type));
		if(!strcmp(info->className->name, "Object") &&
		   info->className->namespace &&
		   !strcmp(info->className->namespace, "System"))
		{
			/* Check that it is within the system image, to prevent
			   applications from fooling us into believing that their
			   own class is the system's object class */
			info = ILClassResolve(info);
			systemImage = info->programItem.image->context->systemImage;
			if(!systemImage || systemImage == info->programItem.image)
			{
				return 1;
			}
		}
	}
	return 0;
}

int ILTypeIsSystemClass(ILType *type, const char *namespace, const char *name)
{
	ILClass *info;
	ILImage *systemImage;
	if(ILType_IsClass(type))
	{
		/* Check the name against "System.Object" */
		info = ILClassResolve(ILType_ToClass(type));
		if(!strcmp(info->className->name, name) &&
		   info->className->namespace &&
		   !strcmp(info->className->namespace, namespace))
		{
			/* Check that it is within the system image, to prevent
			   applications from fooling us into believing that their
			   own class is the system's object class */
			info = ILClassResolve(info);
			systemImage = info->programItem.image->context->systemImage;
			if(!systemImage || systemImage == info->programItem.image)
			{
				return 1;
			}
		}
	}
	return 0;
}

static int TypeAssignCompatible(ILImage *image, ILType *src,
								ILType *dest, int allowBoxing)
{
	ILClass *classInfo;
	ILClass *classInfo2;

	/* Strip unnecessary prefixes, and resolve enumerated
	   types to their underlying type */
	src = ILTypeGetEnumType(ILTypeStripPrefixes(src));
	dest = ILTypeGetEnumType(ILTypeStripPrefixes(dest));

	/* Determine how to compare the types based on their kind */
	if(ILType_IsPrimitive(src) && !allowBoxing)
	{
		/* Primitive type assignments must be identical */
		return (src == dest);
	}
	else if(src == 0 || src == ILType_Null)
	{
		/* A "null" constant is being assigned, which is
		   compatible with any object reference type */
		return ILTypeIsReference(dest);
	}
	else if(dest == 0 || dest == ILType_Null)
	{
		/* Cannot assign to "null" */
		return 0;
	}
	else if(ILTypeIsReference(src))
	{
		if(!ILTypeIsReference(dest))
		{
			/* Both types must be object references */
			return 0;
		}
	boxCheck:
		classInfo = ILClassResolve(ILClassFromType(image, 0, dest, 0));
		classInfo2 = ILClassResolve(ILClassFromType(image, 0, src, 0));
		if(classInfo && classInfo2)
		{
			/* Is the type a regular class or an interface? */
			if(!ILClass_IsInterface(classInfo))
			{
				/* Regular class: the value must inherit from the type */
				if(ILClassInheritsFrom(classInfo2, classInfo))
				{
					return 1;
				}

				/* If "classInfo2" is an interface, then the conversion
				   is OK if "dest" is "System.Object", because all
				   interfaces inherit from "System.Object", even though
				   the metadata doesn't explicitly say so */
				if(ILClass_IsInterface(classInfo2))
				{
					return ILTypeIsObjectClass(dest);
				}

				/* The conversion is not OK */
				return 0;
			}
			else
			{
				/* Interface which the value must implement or inherit from */
				return ILClassImplements(classInfo2, classInfo) ||
				       ILClassInheritsFrom(classInfo2, classInfo);
			}
		}
		else
		{
			return 0;
		}
	}
	else if(allowBoxing && ILTypeIsReference(dest))
	{
		/* Check for boxing conversions */
		goto boxCheck;
	}
	else
	{
		/* Everything else must have type identity to be assignable */
		return ILTypeIdentical(src, dest);
	}
}

int ILTypeAssignCompatible(ILImage *image, ILType *src, ILType *dest)
{
	return TypeAssignCompatible(image, src, dest, 1);
}

int ILTypeAssignCompatibleNonBoxing(ILImage *image, ILType *src, ILType *dest)
{
	return TypeAssignCompatible(image, src, dest, 0);
}

int ILTypeHasModifier(ILType *type, ILClass *classInfo)
{
	classInfo = ILClassResolve(classInfo);
	while(type != 0 && ILType_IsComplex(type))
	{
		if(type->kind__ == IL_TYPE_COMPLEX_CMOD_REQD ||
		   type->kind__ == IL_TYPE_COMPLEX_CMOD_OPT)
		{
			if(ILClassResolve(type->un.modifier__.info__) == classInfo)
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

int ILTypeIsReference(ILType *type)
{
	if(type == 0 || type == ILType_Null)
	{
		/* This is the "null" type, which is always an object reference */
		return 1;
	}
	else if(ILType_IsClass(type))
	{
		return 1;
	}
	else if(ILType_IsComplex(type) &&
	        (type->kind__ == IL_TYPE_COMPLEX_ARRAY ||
			 type->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILTypeIsEnum(ILType *type)
{
	if(ILType_IsValueType(type))
	{
		ILClass *classInfo = ILClassResolve(ILType_ToValueType(type));
		ILClass *parent;

		parent = ILClass_UnderlyingParentClass(classInfo);
		if(parent)
		{
			const char *namespace = ILClass_Namespace(parent);
			if(namespace && !strcmp(namespace, "System") &&
			   !strcmp(ILClass_Name(parent), "Enum"))
			{
				return 1;
			}
		}
	}
	return 0;
}

int ILTypeIsValue(ILType *type)
{
	if(ILType_IsValueType(type))
	{
		return 1;
	}
	else if(ILType_IsPrimitive(type) && type != ILType_Null)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILTypeIsDelegate(ILType *type)
{
	return (ILTypeGetDelegateMethod(type) != 0);
}

void *ILTypeGetDelegateMethod(ILType *type)
{
	if(ILType_IsClass(type))
	{
		ILClass *classInfo = ILClassResolve(ILType_ToClass(type));
		ILClass *parent;

		parent = ILClass_UnderlyingParentClass(classInfo);
		if(parent)
		{
			const char *namespace = ILClass_Namespace(parent);
			if(namespace && !strcmp(namespace, "System") &&
			   !strcmp(ILClass_Name(parent), "MulticastDelegate"))
			{
				ILMethod *method = 0;
				while((method = (ILMethod *)ILClassNextMemberByKind
							(classInfo, (ILMember *)method,
							 IL_META_MEMBERKIND_METHOD)) != 0)
				{
					if(!strcmp(ILMethod_Name(method), "Invoke"))
					{
						return method;
					}
				}
			}
		}
	}
	return 0;
}

void *ILTypeGetDelegateBeginInvokeMethod(ILType *type)
{
	if(ILType_IsClass(type))
	{
		ILType *invokeMethod = ILTypeGetDelegateMethod(type);
		ILClass *classInfo = ILClassResolve(ILType_ToClass(type));
		ILClass *parent;

		parent = ILClass_UnderlyingParentClass(classInfo);
		if (invokeMethod == 0)
		{
			return 0;
		}

		if(parent)
		{
			const char *namespace = ILClass_Namespace(parent);
			if(namespace && !strcmp(namespace, "System") &&
				!strcmp(ILClass_Name(parent), "MulticastDelegate"))
			{
				int i = 0, numInvokeParams;
				ILMethod *method = 0;
				ILType *paramType, *methodSignature;
				ILType *invokeMethodSignature;				

				invokeMethodSignature = ILMethod_Signature(invokeMethod);

				while((method = (ILMethod *)ILClassNextMemberByKind
					(classInfo, (ILMember *)method,
					IL_META_MEMBERKIND_METHOD)) != 0)
				{
					if(!strcmp(ILMethod_Name(method), "BeginInvoke"))
					{
						methodSignature = ILMethod_Signature(method);

						numInvokeParams = ILTypeNumParams(invokeMethodSignature);

						/* Make sure the number of params is correct */

						if (ILTypeNumParams(methodSignature) != numInvokeParams + 2)
						{
							continue;
						}

						/* Make sure all the params in the BeginInvoke method match the params in the Invoke method */

						for (i = 1; i <= numInvokeParams; i++)
						{
							if (!ILTypeIdentical
								(
									ILTypeGetParam(methodSignature, i), 
									ILTypeGetParam(invokeMethodSignature, i)
								))
							{
								break;
							}
						}

						/* Find next BeginInvoke method if params don't match Invoke params */

						if (i < numInvokeParams + 1)
						{
							continue;
						}

						paramType = (ILTypeGetParam(methodSignature, numInvokeParams + 1));

						/* Find next BeginInvoke method if IAsyncResult param isn't correct */

						if (!ILTypeIsSystemClass(paramType, "System", "AsyncCallback"))
						{
							continue;
						}
						
						paramType = (ILTypeGetParam(methodSignature, numInvokeParams + 2));

						/* Find next BeginInvoke method if AsyncState param isn't correct */

						if (!ILTypeIsObjectClass(paramType))
						{
							continue;
						}

						return method;
					}
				}
			}
		}
	}
	return 0;
}

void *ILTypeGetDelegateEndInvokeMethod(ILType *type)
{
	if(ILType_IsClass(type))
	{
		ILType *invokeMethod = ILTypeGetDelegateMethod(type);
		ILClass *classInfo = ILClassResolve(ILType_ToClass(type));
		ILClass *parent;

		parent = ILClass_UnderlyingParentClass(classInfo);
		if (invokeMethod == 0)
		{
			return 0;
		}

		if(parent)
		{
			const char *namespace = ILClass_Namespace(parent);
			if(namespace && !strcmp(namespace, "System") &&
				!strcmp(ILClass_Name(parent), "MulticastDelegate"))
			{
				int i = 0, j, ok, numInvokeParams;
				ILMethod *method = 0;
				ILType *paramType, *methodSignature;
				ILType *invokeMethodSignature;				

				invokeMethodSignature = ILMethod_Signature(invokeMethod);

				while((method = (ILMethod *)ILClassNextMemberByKind
					(classInfo, (ILMember *)method,
					IL_META_MEMBERKIND_METHOD)) != 0)
				{
					if(!strcmp(ILMethod_Name(method), "EndInvoke"))
					{
						methodSignature = ILMethod_Signature(method);

						numInvokeParams = ILTypeNumParams(invokeMethodSignature);

						/* Make sure all the params in the EndInvoke method match the byref params in the Invoke method */

						for (ok = 1, j = 1, i = 1; i <= numInvokeParams; i++)
						{
							paramType = ILTypeGetParam(invokeMethodSignature, i);

							if (ILType_IsComplex(paramType) 
								&& ILType_Kind(paramType) == IL_TYPE_COMPLEX_BYREF)
							{
								if (j > ILTypeNumParams(methodSignature))
								{
									ok = 0;

									break;
								}

								if (!ILTypeIdentical
									(
										paramType,
										ILTypeGetParam(methodSignature, j++)
									))
								{
									ok = 0;

									break;
								}
							}
						}

						/* Find next EndInvoke method if params don't match Invoke params */

						if (!ok)
						{
							continue;
						}

						paramType = (ILTypeGetParam(methodSignature, j));

						/* Find next EndInvoke method if IAsyncResult param isn't correct */

						if (!ILTypeIsSystemClass(paramType, "System", "IAsyncResult"))
						{
							continue;
						}

						return method;
					}
				}
			}
		}
	}
	return 0;
}

int ILTypeDelegateSignatureMatch(ILType *type, void *_method)
{
	ILMethod *method = (ILMethod *)_method;
	ILMethod *invoke;
	ILType *methodSignature;
	ILType *invokeSignature;
	unsigned long numParams;
	unsigned long param;

	/* Constructors can never be used in delegate invocation */
	if(ILMethod_IsConstructor(method) || ILMethod_IsStaticConstructor(method))
	{
		return 0;
	}

	/* Find the delegate invocation method.  This also checks
	   that the class is truly a delegate */
	invoke = ILTypeGetDelegateMethod(type);
	if(!invoke)
	{
		return 0;
	}

	/* Check that the delegate signatures match */
	methodSignature = ILMethod_Signature(method);
	invokeSignature = ILMethod_Signature(invoke);
	numParams = ILTypeNumParams(methodSignature);
	if(numParams != ILTypeNumParams(invokeSignature))
	{
		return 0;
	}
	if(!ILTypeIdentical(ILTypeGetReturn(methodSignature),
						ILTypeGetReturn(invokeSignature)))
	{
		return 0;
	}
	for(param = 1; param <= numParams; ++param)
	{
		if(!ILTypeIdentical(ILTypeGetParam(methodSignature, param),
							ILTypeGetParam(invokeSignature, param)))
		{
			return 0;
		}
	}

	/* Check that both signatures have identical vararg flags */
	if((ILType_CallConv(methodSignature) & IL_META_CALLCONV_MASK)
			== IL_META_CALLCONV_VARARG)
	{
		if((ILType_CallConv(invokeSignature) & IL_META_CALLCONV_MASK)
				!= IL_META_CALLCONV_VARARG)
		{
			return 0;
		}
	}
	else if((ILType_CallConv(invokeSignature) & IL_META_CALLCONV_MASK)
					== IL_META_CALLCONV_VARARG)
	{
		return 0;
	}

	/* We have a match */
	return 1;
}

int ILTypeIsDelegateSubClass(ILType *type)
{
	if(ILType_IsClass(type))
	{
		ILClass *classInfo = ILClassResolve(ILType_ToClass(type));
		const char *namespace;
		ILImage *systemImage;
		while(classInfo != 0)
		{
			namespace = ILClass_Namespace(classInfo);
			if(namespace && !strcmp(namespace, "System") &&
			   !strcmp(ILClass_Name(classInfo), "Delegate"))
			{
				/* Check that it is within the system image, to prevent
				   applications from fooling us into believing that their
				   own class is the system's Delegate class */
				systemImage = classInfo->programItem.image
						->context->systemImage;
				if(!systemImage || systemImage == classInfo->programItem.image)
				{
					return 1;
				}
			}
			classInfo = ILClass_UnderlyingParentClass(classInfo);
		}
	}
	return 0;
}

ILType *ILTypeCreateVarNum(ILContext *context, int kind, int num)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = kind;
		type->un.num__ = num;
	}
	return type;
}

ILType *ILTypeCreateWith(ILContext *context, ILType *mainType)
{
	ILType *type = ILMemPoolCalloc(&(context->typePool), ILType);
	if(type)
	{
		type->kind__ = IL_TYPE_COMPLEX_WITH;
		type->num__ = 0;
		type->un.method__.retType__ = mainType;
	}
	return type;
}

int ILTypeAddWithParam(ILContext *context, ILType *type, ILType *paramType)
{
	return ILTypeAddParam(context, type, paramType);
}

unsigned long ILTypeNumWithParams(ILType *type)
{
	return ILTypeNumParams(type);
}

ILType *ILTypeGetWithParam(ILType *type, unsigned long num)
{
	return ILTypeGetParam(type, num);
}

ILType *ILTypeGetWithParamWithPrefixes(ILType *type, unsigned long num)
{
	return ILTypeGetParamWithPrefixes(type, num);
}

ILType *ILTypeGetWithMain(ILType *type)
{
	return ILTypeGetReturn(type);
}

ILType *ILTypeGetWithMainWithPrefixes(ILType *type)
{
	return ILTypeGetReturnWithPrefixes(type);
}

void ILTypeSetWithMain(ILType *type, ILType *mainType)
{
	if(ILType_IsWith(type))
	{
		type->un.method__.retType__ = mainType;
	}
}

ILType *ILTypeImport(ILImage *image, ILType *type)
{
	if(!image || !type)
	{
		return 0;
	}
	if(ILType_IsPrimitive(type))
	{
		return type;
	}
	else if(ILType_IsClass(type) || ILType_IsValueType(type))
	{
		ILClass *class = ILType_ToClass(type);
		ILType *synType;

		if((synType = ILClassGetSynType(class)) != 0)
		{
			return ILTypeImport(image, synType);
		}
		if(class->programItem.image != image)
		{
			class = ILClassResolve(class);
			if(!class)
			{
				return 0;
			}
			class = ILClassImport(image, class);
			if(!class)
			{
				return 0;
			}
			if(ILType_IsClass(type))
			{
				return ILType_FromClass(class);
			}
			return ILType_FromValueType(class);
		}
		return type;
	}
	else if(ILType_IsComplex(type))
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_VAR:
			case IL_TYPE_COMPLEX_MVAR:
			{
				return type;
			}
			break;

			case IL_TYPE_COMPLEX_BYREF:
			case IL_TYPE_COMPLEX_PTR:
			case IL_TYPE_COMPLEX_PINNED:
			{
				ILType *refType;
				ILType *newRefType;

				refType = ILType_Ref(type);
				if((newRefType = ILTypeImport(image, refType)) == 0)
				{
					return 0;
				}
				if(newRefType != refType)
				{
					return ILTypeCreateRef(ILImageToContext(image),
										   ILType_Kind(type), newRefType);
				}
				return type;
			}
			break;

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				ILType *elemType;
				ILType *newElemType;

				elemType = ILType_ElemType(type);
				if(!elemType)
				{
					return 0;
				}
				newElemType = ILTypeImport(image, elemType);
				if(!newElemType)
				{
					return 0;
				}
				if(elemType != newElemType)
				{
					ILType *newType;

					newType = ILTypeCreateArray(ILImageToContext(image), 1,
												newElemType);
					if(!newType)
					{
						return 0;
					}
					newType->kind__ = ILType_Kind(type);
					ILType_Size(newType) = ILType_Size(type);
					ILType_LowBound(newType) = ILType_LowBound(type);
					return newType;
				}
				return type;
			}
			break;

			case IL_TYPE_COMPLEX_WITH:
			{
				ILType *withMain;
				ILType *newWithMain;
				ILType *newType;
				unsigned long numWithParams;
				unsigned long current;

				withMain = ILTypeGetWithMain(type);
				if((newWithMain = ILTypeImport(image, withMain)) == 0)
				{
					return 0;
				}
				if((newType = ILTypeCreateWith(ILImageToContext(image),
											   newWithMain)) == 0)
				{
					return 0;
				}
				numWithParams = ILTypeNumWithParams(type);
				for(current = 1; current <= numWithParams; ++current)
				{
					ILType *withParam;
					ILType *newWithParam;

					if((withParam = ILTypeGetWithParam(type, current)) == 0)
					{
						return 0;
					}
					if((newWithParam = ILTypeImport(image, withParam)) == 0)
					{
						return 0;
					}
					if(ILTypeAddWithParam(ILImageToContext(image), newType,
										  newWithParam) == 0)
					{
						return 0;
					}
				}
				return newType;
			}
			break;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
