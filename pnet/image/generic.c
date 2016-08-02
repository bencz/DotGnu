/*
 * generic.c - Process generic information from an image file.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

ILGenericPar *ILGenericParCreate(ILImage *image, ILToken token,
								 ILProgramItem *owner, ILUInt32 number)
{
	ILGenericPar *genPar;
	ILClass *classInfo;

	/* Allocate space for the GenericPar block from the memory stack */
	genPar = ILMemStackAlloc(&(image->memStack), ILGenericPar);
	if(!genPar)
	{
		return 0;
	}

	/* Set the GenericPar information fields */
	genPar->ownedItem.programItem.image = image;
	genPar->ownedItem.owner = owner;
	genPar->number = (ILUInt16)number;
	genPar->flags = 0;
	genPar->name = 0;
	genPar->firstConstraint = 0;

	/* Assign a token code to the GenericPar information block */
	if(!_ILImageSetToken(image, &(genPar->ownedItem.programItem), token,
						 IL_META_TOKEN_GENERIC_PAR))
	{
		return 0;
	}

	/* Mark the class as having generic parameters */
	classInfo = ILProgramItemToClass(owner);
	if(classInfo)
	{
		classInfo->attributes |= IL_META_TYPEDEF_GENERIC_PARS;
	}

	/* Return the GenericPar information block to the caller */
	return genPar;
}

ILUInt32 ILGenericParGetNumber(ILGenericPar *genPar)
{
	return genPar->number;
}

ILUInt32 ILGenericParGetFlags(ILGenericPar *genPar)
{
	return genPar->flags;
}

void ILGenericParSetFlags(ILGenericPar *genPar, ILUInt32 mask, ILUInt32 value)
{
	genPar->flags = (ILUInt16)((genPar->flags & ~mask) | value);
}

ILProgramItem *ILGenericParGetOwner(ILGenericPar *genPar)
{
	return genPar->ownedItem.owner;
}

const char *ILGenericParGetName(ILGenericPar *genPar)
{
	return genPar->name;
}

int ILGenericParSetName(ILGenericPar *genPar, const char *name)
{
	genPar->name = _ILContextPersistString
			(genPar->ownedItem.programItem.image, name);
	return (genPar->name != 0);
}

ILGenericConstraint *ILGenericParNextConstraint(ILGenericPar *genPar, ILGenericConstraint *last)
{
	if(last)
	{
		return last->nextConstraint;
	}
	else if(genPar)
	{
		return genPar->firstConstraint;
	}
	else
	{
		return 0;
	}
}

ILGenericConstraint *ILConstraintCreate(ILImage *image, ILToken token,
										ILProgramItem *owner,
										ILProgramItem *classInfo)
{
	ILGenericConstraint *constraint;


	constraint = ILMemStackAlloc(&(image->memStack), ILGenericConstraint);
	if(!constraint)
	{
		return 0;
	}
	constraint->ownedItem.programItem.image = owner->image;
	constraint->ownedItem.owner = owner;
	constraint->parameter = owner;
	constraint->constraint = classInfo;
	constraint->nextConstraint = 0;

	/* Assign a token code to the GenericConstraint information block */
	if(token != 0 || image->type == IL_IMAGETYPE_BUILDING)
	{
		if(!_ILImageSetToken(image,
						 &(constraint->ownedItem.programItem), token,
						 IL_META_TOKEN_GENERIC_CONSTRAINT))
		{
			return 0;
		}
	}

	return constraint;
}

ILGenericConstraint *ILGenericParAddConstraint(ILGenericPar *genPar,
											   ILToken token,
											   ILProgramItem *constraint)
{
	ILGenericConstraint **last = &(genPar->firstConstraint);

	while (*last != 0)
	{
		last = &((*last)->nextConstraint);
	}
	(*last) = ILConstraintCreate(genPar->ownedItem.programItem.image,
								 token,
								 ILToProgramItem(genPar),
								 constraint);
	if(!(*last))
	{
		return 0;
	}


	return (*last);
}

ILGenericPar *ILConstraintGetParam(ILGenericConstraint *constraint)
{
	return (ILGenericPar *)constraint->parameter;
}

ILProgramItem *ILConstraintGetType(ILGenericConstraint *constraint)
{
	return constraint->constraint;
}

/*
 * Search key for "GenericParCompare".
 */
typedef struct
{
	ILProgramItem *owner;
	ILUInt32	   number;

} ILGenericParSearchKey;

/*
 * Compare GenericPar tokens looking for a match.
 */
static int GenericParCompare(void *item, void *userData)
{
	ILGenericPar *genPar = (ILGenericPar *)item;
	ILGenericParSearchKey *key = (ILGenericParSearchKey *)userData;
	ILToken token1 =
		(genPar->ownedItem.owner ? genPar->ownedItem.owner->token : 0);
	ILToken token1Stripped = (token1 & ~IL_META_TOKEN_MASK);
	ILToken token2 = key->owner->token;
	ILToken token2Stripped = (token2 & ~IL_META_TOKEN_MASK);
	if(token1Stripped < token2Stripped)
	{
		return -1;
	}
	else if(token1Stripped > token2Stripped)
	{
		return 1;
	}
	else if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else if(genPar->number < key->number)
	{
		return -1;
	}
	else if(genPar->number > key->number)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

ILGenericPar *ILGenericParGetFromOwner(ILProgramItem *owner, ILUInt32 number)
{
	ILGenericParSearchKey key;

	/* Take a short-cut if we know that the item is not generic */
	if((owner->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_TYPE_DEF &&
	   (((ILClass *)owner)->attributes & IL_META_TYPEDEF_GENERIC_PARS) == 0)
	{
		return 0;
	}
	if((owner->token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_DEF &&
	   (ILMethod_CallConv((ILMethod *)owner) & IL_META_CALLCONV_GENERIC) == 0)
	{
		return 0;
	}

	/* Search for the generic parameter information */
	key.owner = owner;
	key.number = number;
	return (ILGenericPar *)ILImageSearchForToken
				(owner->image, IL_META_TOKEN_GENERIC_PAR,
				 GenericParCompare, (void *)&key);
}

ILUInt32 ILGenericParGetNumParams(ILProgramItem *owner)
{
	ILUInt32 number = 0;
	while(ILGenericParGetFromOwner(owner, number) != 0)
	{
		++number;
	}
	return number;
}

ILMethodSpec *ILMethodSpecCreate(ILImage *image, ILToken token,
								 ILMember *method, ILType *type)
{
	ILMethodSpec *spec;

	/* Allocate space for the MethodSpec block from the memory stack */
	spec = ILMemStackAlloc(&(image->memStack), ILMethodSpec);
	if(!spec)
	{
		return 0;
	}

	/* Set the MethodSpec information fields */
	spec->programItem.image = image;
	spec->method = method;
	spec->type = type;
	spec->typeBlob = 0;

	/* Assign a token code to the MethodSpec information block */
	if(!_ILImageSetToken(image, &(spec->programItem), token,
						 IL_META_TOKEN_METHOD_SPEC))
	{
		return 0;
	}

	/* Return the MethodSpec information block to the caller */
	return spec;
}

ILMember *ILMethodSpecGetMethod(ILMethodSpec *spec)
{
	return spec->method;
}

ILType *ILMethodSpecGetType(ILMethodSpec *spec)
{
	return spec->type;
}

void _ILMethodSpecSetTypeIndex(ILMethodSpec *spec, ILUInt32 index)
{
	spec->typeBlob = index;
}

int ILTypeNeedsInstantiation(ILType *type)
{
	unsigned long num;
	unsigned long posn;

	if(type != 0 && ILType_IsComplex(type))
	{
		switch(ILType_Kind(type))
		{
			case IL_TYPE_COMPLEX_BYREF:
			case IL_TYPE_COMPLEX_PTR:
			case IL_TYPE_COMPLEX_PINNED:
			{
				return ILTypeNeedsInstantiation(ILType_Ref(type));
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_ARRAY:
			case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
			{
				return ILTypeNeedsInstantiation(ILTypeGetElemType(type));
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_CMOD_REQD:
			case IL_TYPE_COMPLEX_CMOD_OPT:
			{
				return ILTypeNeedsInstantiation(type->un.modifier__.type__);
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_LOCALS:
			{
				num = ILTypeNumLocals(type);
				for(posn = 0; posn < num; ++posn)
				{
					if(ILTypeNeedsInstantiation(ILTypeGetLocal(type, posn)))
					{
						return 1;
					}
				}
			}
			break;

			case IL_TYPE_COMPLEX_MVAR:
			case IL_TYPE_COMPLEX_VAR:
			{
				return 1;
			}
			/* Not reached */

			case IL_TYPE_COMPLEX_WITH:
			case IL_TYPE_COMPLEX_PROPERTY:
			case IL_TYPE_COMPLEX_METHOD:
			case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_SENTINEL:
			{
				if(ILTypeNeedsInstantiation(ILTypeGetReturn(type)))
				{
					return 1;
				}
				num = ILTypeNumParams(type);
				for(posn = 1; posn <= num; ++posn)
				{
					if(ILTypeNeedsInstantiation(ILTypeGetParam(type, posn)))
					{
						return 1;
					}
				}
			}
			break;
		}
	}
	return 0;
}

ILType *ILTypeInstantiate(ILContext *context, ILType *type,
						  ILType *classParams, ILType *methodParams)
{
	ILType *inner;
	ILType *newType;
	unsigned long num;
	unsigned long posn;

	/* Bail out immediately if the type does not need to be instantiated */
	if(!ILTypeNeedsInstantiation(type))
	{
		return type;
	}

	/* Re-construct the type with the instantiations in place */
	switch(ILType_Kind(type))
	{
		case IL_TYPE_COMPLEX_BYREF:
		case IL_TYPE_COMPLEX_PTR:
		case IL_TYPE_COMPLEX_PINNED:
		{
			/* Instantiate a simple reference type */
			inner = ILTypeInstantiate(context, ILType_Ref(type),
									  classParams, methodParams);
			if(inner)
			{
				type = ILTypeCreateRef(context, ILType_Kind(type), inner);
			}
			else
			{
				type = ILType_Invalid;
			}
		}
		break;

		case IL_TYPE_COMPLEX_ARRAY:
		case IL_TYPE_COMPLEX_ARRAY_CONTINUE:
		{
			/* Instantiate an array type */
			inner = ILTypeInstantiate(context, ILTypeGetElemType(type),
									  classParams, methodParams);
			if(inner)
			{
				type = ILTypeCreateArray(context, ILTypeGetRank(type), inner);
			}
			else
			{
				type = ILType_Invalid;
			}
		}
		break;

		case IL_TYPE_COMPLEX_CMOD_REQD:
		case IL_TYPE_COMPLEX_CMOD_OPT:
		{
			/* Instantiate a custom modifier reference */
			inner = ILTypeInstantiate(context, type->un.modifier__.type__,
									  classParams, methodParams);
			type = ILTypeCreateModifier
				(context, 0, ILType_Kind(type), type->un.modifier__.info__);
			if(type)
			{
				type = ILTypeAddModifiers(context, type, inner);
			}
		}
		break;

		case IL_TYPE_COMPLEX_LOCALS:
		{
			/* Instantiate a local variable signature */
			newType = ILTypeCreateLocalList(context);
			if(!newType)
			{
				return ILType_Invalid;
			}
			num = ILTypeNumLocals(type);
			for(posn = 0; posn < num; ++posn)
			{
				inner = ILTypeInstantiate(context, ILTypeGetLocalWithPrefixes
														(type, posn),
										  classParams, methodParams);
				if(!inner || !ILTypeAddLocal(context, newType, inner))
				{
					return ILType_Invalid;
				}
			}
			return newType;
		}
		/* Not reached */

		case IL_TYPE_COMPLEX_MVAR:
		{
			/* Instantiate a generic method variable reference */
			if(methodParams)
			{
				return ILTypeGetParamWithPrefixes
					(methodParams, ILType_VarNum(type) + 1);
			}
			else
			{
				return type;
			}
		}
		/* Not reached */

		case IL_TYPE_COMPLEX_VAR:
		{
			/* Instantiate a generic class variable reference */
			if(classParams)
			{
				return ILTypeGetWithParamWithPrefixes
					(classParams, ILType_VarNum(type) + 1);
			}
			else
			{
				return 0;
			}
		}
		/* Not reached */

		case IL_TYPE_COMPLEX_WITH:
		case IL_TYPE_COMPLEX_PROPERTY:
		case IL_TYPE_COMPLEX_METHOD:
		case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_SENTINEL:
		{
			/* Instantiate a method signature */
			inner = ILTypeGetReturnWithPrefixes(type);
			if(inner)
			{
				inner = ILTypeInstantiate
					(context, inner, classParams, methodParams);
				if(!inner)
				{
					return ILType_Invalid;
				}
			}
			newType = ILTypeCreateMethod(context, inner);
			if(!newType)
			{
				return ILType_Invalid;
			}
			newType->kind__ = type->kind__;
			num = ILTypeNumParams(type);
			for(posn = 1; posn <= num; ++posn)
			{
				inner = ILTypeInstantiate(context,
										  ILTypeGetParamWithPrefixes(type, posn),
										  classParams, methodParams);
				if(!inner || !ILTypeAddParam(context, newType, inner))
				{
					return ILType_Invalid;
				}
			}
			return newType;
		}
		/* Not reached */
	}
	return type;
}


#ifdef	__cplusplus
};
#endif
