/*
 * synthetic.c - Handle synthetic classes such as arrays, pointers, etc.
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

/*
 * Compute a hash value for a type.
 */
static unsigned long HashType(unsigned long start, ILType *type)
{
	ILUInt32 param;
	ILUInt32 numParams;

	if(ILType_IsPrimitive(type))
	{
		return (start << 5) + start + ILType_ToElement(type);
	}
	else if(ILType_IsValueType(type) || ILType_IsClass(type))
	{
		ILClass *classInfo = ILType_ToClass(type);
		if(classInfo->className->namespace)
		{
			return ILHashString(ILHashString(ILHashString
								  (start, classInfo->className->namespace, -1),
								   ".", 1),
								classInfo->className->name, -1);
		}
		else
		{
			return ILHashString(start, classInfo->className->name, -1);
		}
	}
	else if(type != 0 && ILType_IsComplex(type))
	{
		start = (start << 5) + start + (unsigned long)(type->kind__);
		switch(ILType_Kind(type) & 0xFF)
		{
			case IL_TYPE_COMPLEX_BYREF:
			case IL_TYPE_COMPLEX_PTR:
			case IL_TYPE_COMPLEX_PINNED:
			{
				return HashType(start, type->un.refType__);
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				return HashType(start, type->un.array__.elemType__);
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				return HashType(start, type->un.modifier__.type__);
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_WITH:
			case IL_TYPE_COMPLEX_PROPERTY:
			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
			{
				start = HashType(start, type->un.method__.retType__);
				numParams = type->num__;
				for(param = 1; param <= numParams; ++param)
				{
					start = HashType
						(start, ILTypeGetParamWithPrefixes(type, param));
				}
			}
			break;

			case IL_TYPE_COMPLEX_VAR:
			case IL_TYPE_COMPLEX_MVAR:
			{
				start = (start << 5) + start +
						(unsigned long)ILType_VarNum(type);
			}
			break;
		}
		return start;
	}
	else
	{
		return start;
	}
}

/*
 * Compute the hash value for a synthentic hash element.
 */
static unsigned long SynHash_Compute(ILClass *info)
{
	return HashType(0, info->synthetic);
}

/*
 * Compute the hash value for a synthentic type key.
 */
static unsigned long SynHash_KeyCompute(ILType *type)
{
	return HashType(0, type);
}

/*
 * Determine if we have a match in the synthetic types hash table.
 */
static int SynHash_Match(ILClass *info, ILType *type)
{
	return ILTypeIdentical(info->synthetic, type);
}

/*
 * Initialize the synthetic types hash table.
 */
int _ILContextSyntheticInit(ILContext *context)
{
	context->syntheticHash =
				ILHashCreate(0,
					 (ILHashComputeFunc)SynHash_Compute,
					 (ILHashKeyComputeFunc)SynHash_KeyCompute,
					 (ILHashMatchFunc)SynHash_Match,
					 (ILHashFreeFunc)0);
	return (context->syntheticHash != 0);
}

/*
 * Create a synthetic class if not already present.
 */
static ILClass *CreateSynthetic(ILImage *image, const char *name,
								ILClass *parent, int isSealed)
{
	ILProgramItem *scope = ILClassGlobalScope(image);
	ILClass *info;

	/* See if we already have the class in the image */
	info = ILClassLookup(scope, name, "$Synthetic");
	if(info)
	{
		return info;
	}

	/* Create a new class information block */
	info = ILClassCreate(scope, 0, name, "$Synthetic", ILToProgramItem(parent));
	if(!info)
	{
		return 0;
	}

	/* Set the correct attributes on the class */
	ILClassSetAttrs(info, ~0, IL_META_TYPEDEF_PUBLIC |
							  IL_META_TYPEDEF_LAYOUT_SEQUENTIAL |
							  (isSealed ? IL_META_TYPEDEF_SEALED
							  			: IL_META_TYPEDEF_ABSTRACT) |
							  IL_META_TYPEDEF_SPECIAL_NAME |
							  IL_META_TYPEDEF_RT_SPECIAL_NAME |
							  IL_META_TYPEDEF_BEFORE_FIELD_INIT);

	/* The class is ready to go */
	return info;
}

/*
 * Add methods to a synthetic class that corresponds
 * to a single-dimensional array.
 */
static int AddSArrayMethods(ILClass *info, ILType *type)
{
	ILContext *context = ILClassToContext(info);
	ILMethod *method;
	ILType *signature;

	/* Build the constructor, which specifies a single dimension */
	method = ILMethodCreate(info, 0, ".ctor",
							IL_META_METHODDEF_PUBLIC |
							IL_META_METHODDEF_HIDE_BY_SIG |
							IL_META_METHODDEF_SPECIAL_NAME |
							IL_META_METHODDEF_RT_SPECIAL_NAME);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(context, ILType_Void);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	if(!ILTypeAddParam(context, signature, ILType_Int32))
	{
		return 0;
	}
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~0, IL_META_METHODIMPL_RUNTIME);

	/* Done */
	return 1;
}

/*
 * Add methods to a synthetic class that corresponds
 * to a multi-dimensional array.
 */
static int AddMArrayMethods(ILClass *info, ILType *type)
{
	ILContext *context = ILClassToContext(info);
	ILType *temp;
	ILUInt32 numDims;
	ILUInt32 dim;
	ILMethod *method;
	ILType *signature;

	/* Count the number of dimensions and get the element type */
	temp = type;
	numDims = 0;
	while(temp != 0 && ILType_IsComplex(temp) &&
		  temp->kind__ == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
	{
		++numDims;
		temp = temp->un.array__.elemType__;
	}
	if(temp != 0 && ILType_IsComplex(temp) &&
	   temp->kind__ == IL_TYPE_COMPLEX_ARRAY)
	{
		++numDims;
		temp = temp->un.array__.elemType__;
	}
	if(!temp)
	{
		return 0;
	}

	/* Build the first constructor, which only specifies dimensions */
	method = ILMethodCreate(info, 0, ".ctor",
							IL_META_METHODDEF_PUBLIC |
							IL_META_METHODDEF_HIDE_BY_SIG |
							IL_META_METHODDEF_SPECIAL_NAME |
							IL_META_METHODDEF_RT_SPECIAL_NAME);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(context, ILType_Void);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	for(dim = 0; dim < numDims; ++dim)
	{
		if(!ILTypeAddParam(context, signature, ILType_Int32))
		{
			return 0;
		}
	}
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~0, IL_META_METHODIMPL_RUNTIME);

	/* Build the second constructor, which specifies lower bounds and lengths */
	method = ILMethodCreate(info, 0, ".ctor",
							IL_META_METHODDEF_PUBLIC |
							IL_META_METHODDEF_HIDE_BY_SIG |
							IL_META_METHODDEF_SPECIAL_NAME |
							IL_META_METHODDEF_RT_SPECIAL_NAME);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(context, ILType_Void);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	for(dim = 0; dim < numDims; ++dim)
	{
		if(!ILTypeAddParam(context, signature, ILType_Int32))
		{
			return 0;
		}
		if(!ILTypeAddParam(context, signature, ILType_Int32))
		{
			return 0;
		}
	}
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~0, IL_META_METHODIMPL_RUNTIME);

	/* Build the "Get" method */
	method = ILMethodCreate(info, 0, "Get",
							IL_META_METHODDEF_PUBLIC |
							IL_META_METHODDEF_HIDE_BY_SIG);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(context, temp);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	for(dim = 0; dim < numDims; ++dim)
	{
		if(!ILTypeAddParam(context, signature, ILType_Int32))
		{
			return 0;
		}
	}
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~0, IL_META_METHODIMPL_RUNTIME);

	/* Build the "Set" method */
	method = ILMethodCreate(info, 0, "Set",
							IL_META_METHODDEF_PUBLIC |
							IL_META_METHODDEF_HIDE_BY_SIG);
	if(!method)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(context, ILType_Void);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	for(dim = 0; dim < numDims; ++dim)
	{
		if(!ILTypeAddParam(context, signature, ILType_Int32))
		{
			return 0;
		}
	}
	if(!ILTypeAddParam(context, signature, temp))
	{
		return 0;
	}
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~0, IL_META_METHODIMPL_RUNTIME);

	/* Build the "Address" method */
	method = ILMethodCreate(info, 0, "Address",
							IL_META_METHODDEF_PUBLIC |
							IL_META_METHODDEF_HIDE_BY_SIG);
	if(!method)
	{
		return 0;
	}
	temp = ILTypeCreateRef(context, IL_TYPE_COMPLEX_BYREF, temp);
	if(!temp)
	{
		return 0;
	}
	signature = ILTypeCreateMethod(context, temp);
	if(!signature)
	{
		return 0;
	}
	ILTypeSetCallConv(signature, IL_META_CALLCONV_HASTHIS);
	for(dim = 0; dim < numDims; ++dim)
	{
		if(!ILTypeAddParam(context, signature, ILType_Int32))
		{
			return 0;
		}
	}
	ILMemberSetSignature((ILMember *)method, signature);
	ILMethodSetImplAttrs(method, ~0, IL_META_METHODIMPL_RUNTIME);

	/* Done */
	return 1;
}

ILClass *_ILTypeToSyntheticArray(ILImage *image, ILType *type, int singleDim)
{
	ILContext *context = ILImageToContext(image);
	ILImage *synthetic;
	ILClass *parent, *info;
	char name[32];

	/* See if we already have a synthetic class for this type */
	info = ILHashFindType(context->syntheticHash, type, ILClass);
	if(info)
	{
		return info;
	}

	/* Bail out if not enough memory to create the synthetic image */
	synthetic = ILContextGetSynthetic(context);
	if(!synthetic)
	{
		return 0;
	}

	/* Create a unique name for the synthetic class */
	sprintf(name, "$%lu",
			(unsigned long)(synthetic->memStack.currSize -
							synthetic->memStack.size +
							synthetic->memStack.posn));

	/* What kind of array are we creating? */
	if(singleDim)
	{
		/* Create a single-dimensional array type,
		   which inherits from "$Synthetic.SArray" */
		parent = ILClassResolveSystem(image, 0, "Array", "System");
		if(!parent)
		{
			return 0;
		}
		parent = CreateSynthetic(synthetic, "SArray", parent, 0);
		if(!parent)
		{
			return 0;
		}
		info = CreateSynthetic(synthetic, name, parent, 1);
		if(!info)
		{
			return 0;
		}
		if(!AddSArrayMethods(info, type))
		{
			return 0;
		}
	}
	else
	{
		/* Create a multi-dimensional array type,
		   which inherits from "$Synthetic.MArray" */
		parent = ILClassResolveSystem(image, 0, "Array", "System");
		if(!parent)
		{
			return 0;
		}
		parent = CreateSynthetic(synthetic, "MArray", parent, 0);
		if(!parent)
		{
			return 0;
		}
		info = CreateSynthetic(synthetic, name, parent, 1);
		if(!info)
		{
			return 0;
		}
		if(!AddMArrayMethods(info, type))
		{
			return 0;
		}
	}

	/* Set the "synthetic" member for the class */
	info->synthetic = type;

	/* Add the synthetic class to the synthetic types hash table */
	if(!ILHashAdd(context->syntheticHash, info))
	{
		return 0;
	}
	return info;
}

ILClass *_ILTypeToSyntheticOther(ILImage *image, ILType *type)
{
	ILContext *context = ILImageToContext(image);
	ILImage *synthetic;
	ILClass *parent, *info;
	char name[32];

	/* See if we already have a synthetic class for this type */
	info = ILHashFindType(context->syntheticHash, type, ILClass);
	if(info)
	{
		return info;
	}

	/* Bail out if not enough memory to create the synthetic image */
	synthetic = ILContextGetSynthetic(context);
	if(!synthetic)
	{
		return 0;
	}

	/* Create a unique name for the synthetic class */
	sprintf(name, "$%lu",
			(unsigned long)(synthetic->memStack.currSize -
							synthetic->memStack.size +
							synthetic->memStack.posn));

	/* Inherit the class off "System.ValueType" unless it is a generic type */
	if(ILType_IsWith(type))
	{
		parent = 0;
	}
	else
	{
		parent = ILClassResolveSystem(image, 0, "ValueType", "System");
		if(!parent)
		{
			return 0;
		}
	}
	info = CreateSynthetic(synthetic, name, parent, 1);
	if(!info)
	{
		return 0;
	}

	/* Set the "synthetic" member for the class */
	info->synthetic = type;

	/* Add the synthetic class to the synthetic types hash table */
	if(!ILHashAdd(context->syntheticHash, info))
	{
		return 0;
	}
	return info;
}

ILClass *_ILTypeToSyntheticInstantiation
		(ILImage *image, ILType *type, ILType *classArgs, ILType *methodArgs)
{
	/* TODO: do this without calling ILTypeInstantiate */
	type = ILTypeInstantiate(image->context, type, classArgs, methodArgs);
	if(!type)
	{
		return 0;
	}
	return ILHashFindType(image->context->syntheticHash, type, ILClass);
}

unsigned long ILTypeHash(ILType *type)
{
	return HashType(0, type);
}

#ifdef	__cplusplus
};
#endif
