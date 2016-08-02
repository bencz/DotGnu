/*
 * cg_coerce.c - Coercions and casts.
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

#include "cg_nodes.h"
#include "cg_resolve.h"
#include "cg_coerce.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Conversion rules that result from looking for a coercion or cast.
 */
typedef struct
{
	/* Class to use to box or unbox the value */
	ILClass		*boxClass;
	ILClass		*unboxClass;
	int			 boxIsEnum;

	/* Method to call to perform the conversion */
	ILMethod	*method;

	/* Explicit type to cast to using a checked cast instruction */
	ILType	    *castType;

	/* Builtin conversion */
	const ILConversion *builtin;

	/* Coerce "null" to a pointer type */
	int			pointerNull;

	/* Cast from System.String to a pointer to System.Char */
	int			stringCharPtr;

	/* Cast from a managed array to a pointer to the element type */
	int			arrayElementPtr;
} ConvertRules;

/*
 * Find a user-defined conversion operator.
 */
static int FindConversion(ILGenInfo *info, ILClass *classInfo,
						  ILType *fromType, ILType *toType,
						  int explicit, ConvertRules *rules)
{
	ILMethod *method;

	/* Look for an implicit operator definition */
	method = ILResolveConversionOperator(info, classInfo, "op_Implicit",
										 fromType, toType);
	if(method)
	{
		rules->method = method;
		return 1;
	}

	/* Look for an explicit operator definition */
	if(explicit)
	{
		method = ILResolveConversionOperator(info, classInfo, "op_Explicit",
											 fromType, toType);
		if(method)
		{
			rules->method = method;
			return 1;
		}
	}

	/* No user-defined operator */
	return 0;
}

/*
 * Forward declaration.
 */
static int GetConvertRules(ILGenInfo *info, ILType *fromType,
						   ILType *toType, int explicit,
						   int kinds, ConvertRules *rules);

/*
 * Get the rules to be used to convert from one reference type to another.
 */
static int GetReferenceConvertRules(ILGenInfo *info, ILType *fromType,
						   		    ILType *toType, int explicit,
						   		    ConvertRules *rules)
{
	ILClass *classFrom;
	ILClass *classTo;

	/* Both types must be reference types */
	if(!ILTypeIsReference(fromType) || !ILTypeIsReference(toType))
	{
		return 0;
	}

	/* Any reference type can be implicitly coerced to "Object" */
	if(ILTypeIsObjectClass(toType))
	{
		return 1;
	}

	/* "Object" can be explicitly converted into any reference type */
	if(ILTypeIsObjectClass(fromType) && explicit)
	{
		rules->castType = toType;
		return 1;
	}

	/* Convert "fromType" and "toType" into their class versions */
	classFrom = ILClassResolve(ILTypeToClass(info, fromType));
	classTo = ILClassResolve(ILTypeToClass(info, toType));
	if(!classFrom || !classTo)
	{
		return 0;
	}

	/* See if "fromType" inherits from "toType", or vice versa */
	if(ILClassInheritsFrom(classFrom, classTo))
	{
		/* Implicit conversion to a base type */
		return 1;
	}
	if(explicit)
	{
		if(ILClassInheritsFrom(classTo, classFrom))
		{
			/* Explicit conversion to a descendent type */
			rules->castType = toType;
			return 1;
		}
	}

	/* See if "fromType" implements "toType", or is an interface
	   that is derived from "toType" */
	if(ILClass_IsInterface(classTo))
	{
		if(ILClassImplements(classFrom, classTo))
		{
			/* Implicit conversion to an interface */
			return 1;
		}
		else if(explicit && !ILClass_IsSealed(classFrom))
		{
			/* Explicit conversion to an interface */
			rules->castType = toType;
			return 1;
		}
	}

	/* Check for explicit interface conversions */
	if(explicit && ILClass_IsInterface(classFrom))
	{
		if(!ILClass_IsInterface(classTo))
		{
			if(!ILClass_IsSealed(classTo) ||
		       ILClassImplements(classTo, classFrom))
			{
				/* Explicit conversion from an interface to a
				   class that may implement it */
				rules->castType = toType;
				return 1;
			}
		}
		else if(!ILClassImplements(classFrom, classTo))
		{
			/* Explicit conversion between unrelated interfaces */
			rules->castType = toType;
			return 1;
		}
	}

	/* Check array type compatibility */
	if(ILType_IsArray(fromType) && ILType_IsArray(toType))
	{
		/* The ranks must be equal, the element types must
		   be references, and there must be a valid conversion
		   between the element types */
		if(ILTypeGetRank(fromType) == ILTypeGetRank(toType))
		{
			ILType *elemFrom = ILTypeGetElemType(fromType);
			ILType *elemTo = ILTypeGetElemType(toType);
			if(ILTypeIsReference(elemFrom) && ILTypeIsReference(elemTo))
			{
				if(GetConvertRules(info, elemFrom, elemTo,
								   explicit, IL_CONVERT_REFERENCE, rules))
				{
					if(rules->castType)
					{
						/* Move the explicit cast up to the array level */
						rules->castType = toType;
					}
					return 1;
				}
			}
		}
	}

	/* This is not a valid reference conversion */
	return 0;
}

/*
 * Get the rules to be used to box or unbox a value type.
 */
static int GetBoxingConvertRules(ILGenInfo *info, ILType *fromType,
						   		 ILType *toType, int explicit,
						   		 ConvertRules *rules)
{
	ILClass *classFrom;
	ILClass *classTo;

	if(ILTypeIsValue(fromType))
	{
		/* Convert the source type into a class */
		classFrom = ILTypeToClass(info, fromType);
		if(!classFrom)
		{
			return 0;
		}

		/* Value types can always be boxed as "Object" */
		if(ILTypeIsObjectClass(toType))
		{
			rules->boxClass = classFrom;
			rules->boxIsEnum = ILTypeIsEnum(fromType);
			return 1;
		}

		/* If "toType" is not a reference type, then boxing is impossible */
		if(!ILTypeIsReference(toType))
		{
			return 0;
		}

		/* Convert the destination type into a class */
		classTo = ILTypeToClass(info, toType);
		if(!classTo)
		{
			return 0;
		}

		/* We can box the value if its class inherits from "toType",
		   or "toType" is an interface that the value type implements */
		if(ILClass_IsInterface(classTo))
		{
			if(!ILClassImplements(classFrom, classTo))
			{
				return 0;
			}
		}
		else
		{
			if(!ILClassInheritsFrom(classFrom, classTo))
			{
				return 0;
			}
		}

		/* Box the value */
		rules->boxClass = classFrom;
		rules->boxIsEnum = ILTypeIsEnum(fromType);
		return 1;
	}
	else if(explicit && ILTypeIsReference(fromType) && ILTypeIsValue(toType))
	{
		/* Convert the two types into classes */
		classFrom = ILTypeToClass(info, fromType);
		if(!classFrom)
		{
			return 0;
		}
		classTo = ILTypeToClass(info, toType);
		if(!classTo)
		{
			return 0;
		}

		/* If the source type is "Object", then unboxing is always possible */
		if(ILTypeIsObjectClass(fromType))
		{
			rules->unboxClass = classTo;
			rules->boxIsEnum = ILTypeIsEnum(toType);
			return 1;
		}

		/* We can unbox the object if the value type inherits from the
		   object's type, or if the object is an interface that the
		   value type implements */
		if(ILClass_IsInterface(classFrom))
		{
			if(!ILClassImplements(classTo, classFrom))
			{
				return 0;
			}
		}
		else
		{
			if(!ILClassInheritsFrom(classTo, classFrom))
			{
				return 0;
			}
		}

		/* Unbox the value */
		rules->unboxClass = classTo;
		rules->boxIsEnum = ILTypeIsEnum(toType);
		return 1;
	}
	else
	{
		/* No relevant boxing or unboxing conversion */
		return 0;
	}
}

/*
 * Get the rules to be used inside of an unsafe block.
 */
static int GetUnsafeConvertRules(ILGenInfo *info, ILType *fromType,
		ILType *toType, int explicit, int kinds, ConvertRules *rules)
{
	const ILConversion *conv;

	/* Handle pointer/pointer casts */
	if(ILType_IsPointer(toType) && ILType_IsPointer(fromType))
	{
		/* Can always cast between pointer types if explicit */
		if(explicit)
		{
			return 1;
		}

		/* Otherwise can only cast implicitly to "void *" */
		return (ILType_Ref(toType) == ILType_Void);
	}
	else if(ILType_IsPointer(toType) && fromType == ILType_Int)
	{
		/* Can cast explicitly from IntPtr to any pointer type */
		if(explicit)
		{
			return 1;
		}
	}
	else if(ILType_IsPointer(fromType) && toType == ILType_Int)
	{
		/* Can cast explicitly from any pointer type to IntPtr */
		if(explicit)
		{
			return 1;
		}
	}

	/* Can cast implicitly from "null" to any pointer type */
	if(ILType_IsPointer(toType) && fromType == ILType_Null)
	{
		rules->pointerNull = 1;
		return 1;
	}

	/* Handle explicit numeric/pointer casts */
	if(explicit)
	{
		/* Pointer to numeric conversion */
		if(ILIsBuiltinNumeric(toType) && ILType_IsPointer(fromType))
		{
			conv = ILFindConversion(fromType, toType, explicit, 1);
			if(conv)
			{
				rules->builtin = conv;
				return 1;
			}
		}

		/* Numeric to pointer conversion */
		if(ILType_IsPointer(toType) && ILIsBuiltinNumeric(fromType))
		{
			conv = ILFindConversion(fromType, toType, explicit, 1);
			if(conv)
			{
				rules->builtin = conv;
				return 1;
			}
		}
	}
	else if(info->inFixed)
	{
		/* String to char * or void * conversion */
		if((ILTypeToMachineType(fromType) == ILMachineType_String) &&
		   ILType_IsPointer(toType))
		{
			ILMachineType machineType;

			machineType = ILTypeToMachineType(ILType_Ref(toType));
			if((machineType == ILMachineType_Char) ||
			   (machineType == ILMachineType_Void))
			{
				rules->stringCharPtr = 1;
				return 1;
			}
		}

		/* Array to element pointer conversion */
		if(ILType_IsArray(fromType) && ILType_IsPointer(toType))
		{
			ILType *elementType;

			/* We can implicitely cast every pointer to void * */
			if(ILTypeToMachineType(ILType_Ref(toType)) == ILMachineType_Void)
			{
				rules->arrayElementPtr = 1;
				return 1;
			}

			elementType = ILTypeGetElemType(fromType);
			if(elementType)
			{
				if(ILTypeIdentical(elementType, ILType_Ref(toType)))
				{
					rules->arrayElementPtr = 1;
					return 1;
				}
			}
		}

		/* Managed Pointer to pointer conversion */
		if(ILType_IsPointer(toType) && ILType_IsRef(fromType))
		{
			if(ILType_Ref(toType) == ILType_Void)
			{
				/* We can allways cast to void * */
				return 1;
			}

			if(ILTypeIdentical(ILType_Ref(fromType), ILType_Ref(toType)))
			{
				return 1;
			}
		}		
	}

	/* Could not find an appropriate conversion */
	return 0;
}

/*
 * Get the rules to be used to convert from one type to another.
 */
static int GetConvertRules(ILGenInfo *info, ILType *fromType,
						   ILType *toType, int explicit,
						   int kinds, ConvertRules *rules)
{
	const ILConversion *conv;
	ILClass *fromClass;
	ILClass *toClass;

	/* Clear the rules */
	rules->boxClass = 0;
	rules->unboxClass = 0;
	rules->boxIsEnum = 0;
	rules->method = 0;
	rules->castType = 0;
	rules->builtin = 0;
	rules->pointerNull = 0;
	rules->stringCharPtr = 0;
	rules->arrayElementPtr = 0;

	/* Strip type prefixes before we start */
	fromType = ILTypeStripPrefixes(fromType);
	toType = ILTypeStripPrefixes(toType);

	/* If the types are identical at this point, then we are done */
	if(ILTypeIdentical(fromType, toType))
	{
		return 1;
	}

	/* We can never convert to the "null" type */
	if(toType == ILType_Null)
	{
		return 0;
	}

	/* If "fromType" is null, then "toType" must be a reference or pointer */
	if(fromType == ILType_Null)
	{
		if((kinds & IL_CONVERT_REFERENCE) != 0)
		{
			if(ILType_IsPointer(toType) && info->unsafeLevel > 0)
			{
				rules->pointerNull = 1;
				return 1;
			}
			return ILTypeIsReference(toType);
		}
		else
		{
			return 0;
		}
	}

	/* Look for a builtin numeric conversion */
	if((kinds & IL_CONVERT_NUMERIC) != 0)
	{
		conv = ILFindConversion(fromType, toType, explicit,0);
		if(conv)
		{
			rules->builtin = conv;
			return 1;
		}
	}

	/* Look for a user-defined conversion */
	if((kinds & IL_CONVERT_USER_DEFINED) != 0)
	{
		fromClass = ILTypeToClass(info, fromType);
		toClass = ILTypeToClass(info, toType);
		if(fromClass != 0)
		{
			if(FindConversion(info, fromClass, fromType, toType,
							  explicit, rules))
			{
				return 1;
			}
		}
		if(toClass != 0)
		{
			if(FindConversion(info, toClass, fromType, toType,
							  explicit, rules))
			{
				return 1;
			}
		}
	}

	/* Check for reference conversions */
	if((kinds & IL_CONVERT_REFERENCE) != 0)
	{
		if(GetReferenceConvertRules(info, fromType, toType,
									explicit, rules))
		{
			return 1;
		}
	}

	/* Check for boxing and unboxing conversions */
	if((kinds & IL_CONVERT_BOXING) != 0)
	{
		if(GetBoxingConvertRules(info, fromType, toType,
								 explicit, rules))
		{
			return 1;
		}
	}

	/* Check for explicit enumeration conversions */
	if((kinds & IL_CONVERT_ENUM) != 0 && explicit)
	{
		if(ILTypeIsEnum(fromType))
		{
			if(ILTypeIsEnum(toType))
			{
				/* Both are enumerated types */
				if(GetConvertRules(info, ILTypeGetEnumType(fromType),
								   ILTypeGetEnumType(toType), explicit,
								   IL_CONVERT_NUMERIC, rules))
				{
					return 1;
				}
			}
			else if(ILIsBuiltinNumeric(toType))
			{
				/* Converting from an enumerated type to a numeric type */
				if(GetConvertRules(info, ILTypeGetEnumType(fromType), toType,
								   explicit, IL_CONVERT_NUMERIC, rules))
				{
					return 1;
				}
			}
		}
		else if(ILTypeIsEnum(toType))
		{
			if(ILIsBuiltinNumeric(fromType))
			{
				/* Converting from a numeric type to an enumerated type */
				if(GetConvertRules(info, fromType, ILTypeGetEnumType(toType),
								   explicit, IL_CONVERT_NUMERIC, rules))
				{
					return 1;
				}
			}
		}
	}

	if (info->unsafeLevel > 0)
	{
		if(GetUnsafeConvertRules(info, fromType, toType, explicit, kinds, rules))
		{
			return 1;
		}
	}

	/* If we get here, then we do not know how to convert */
	return 0;
}

/*
 * Attributes that are indicate the type of method.
 */
#define	METHOD_TYPE_ATTRS	(IL_META_METHODDEF_STATIC | \
							 IL_META_METHODDEF_SPECIAL_NAME | \
							 IL_META_METHODDEF_RT_SPECIAL_NAME)

static int ILBetterIndirectConversion(ILGenInfo *info, ILType *from ,
						ILType *to ,
						ILType *conversion1,
						ILType *conversion2)
{
	switch(ILBetterConversion(info,from, ILTypeGetParam(conversion1,1), 
							  ILTypeGetParam(conversion2,1)))
	{
		case IL_BETTER_S1:
		{
			switch(ILBetterConversionFrom(info, ILTypeGetReturn(conversion1), 
										ILTypeGetReturn(conversion2),to))
			{
				case IL_BETTER_S1:
				case IL_BETTER_NEITHER:
				{
					return IL_BETTER_S1;
				}
				break;
				
				case IL_BETTER_S2:
				{
					return IL_BETTER_NEITHER;
				}
				break;
			}
		}
		break;
		
		case IL_BETTER_S2:
		{
			switch(ILBetterConversionFrom(info, ILTypeGetReturn(conversion1), 
										ILTypeGetReturn(conversion2),to))
			{
				case IL_BETTER_S1:
				{
					return IL_BETTER_NEITHER;
				}
				break;
				case IL_BETTER_S2:
				case IL_BETTER_NEITHER:
				{
					return IL_BETTER_S2;
				}
				break;
			}
		}
		break;
		
		case IL_BETTER_NEITHER:
		{
			switch(ILBetterConversionFrom(info, ILTypeGetReturn(conversion1), 
										ILTypeGetReturn(conversion2),to))
			{
				case IL_BETTER_S1:
				{
					return IL_BETTER_S1;
				}
				case IL_BETTER_S2:
				{
					return IL_BETTER_S2;
				}
			}
		}
		break;
		
	}
	return IL_BETTER_NEITHER;
}

/*
 * multiple convert rules are checked here . I have implemented only
 * a 2 step conversion fromType->itype1->itype2->toType , more levels
 * can be implemented  using an extra "indirect" nesting count . But
 * this stops at 2 .
 */
static int GetIndirectConvertRules(ILGenInfo *info, ILType *fromType,
						   ILType *toType, int explicit,
						   int kinds, ILType **itype1,ILType **itype2)
{
	ILMethod *method;
	ILMember *member;
	ILMember *bestMember = 0;
	ILType *signature;
	ILClass *arg1Class = ILTypeToClass(info, fromType);
	ILClass *arg2Class = ILTypeToClass(info, toType);	
	ILType *argType;
	ILType *returnType;
	
	if((kinds & IL_CONVERT_USER_DEFINED) == 0)
	{
		return 0;
		/* this is only used when all others have failed */
	}
	
	while(arg1Class != 0)
	{
		arg1Class= ILClassResolve(arg1Class);
		member=0;
		while((member = ILClassNextMemberMatch
					(arg1Class, member,
					 IL_META_MEMBERKIND_METHOD, "op_Implicit", 0)) != 0)
		{
			/* Filter out members that aren't interesting */
			if((ILMember_Attrs(member) & METHOD_TYPE_ATTRS) !=
   			(IL_META_METHODDEF_STATIC |IL_META_METHODDEF_SPECIAL_NAME))
			{
				continue;
			}
			method = (ILMethod *)member;
			/* Check that this is the signature we are interested in */
			signature = ILMethod_Signature(method);
			if(!ILType_IsMethod(signature))
			{
				continue;
			}
			returnType=ILTypeGetReturn(signature);
			if(ILTypeNumParams(signature)!=1)
			{
				continue;
			}
			argType=ILTypeGetParam(signature,1);
			if(((ILCanCoerceKind(info,returnType,toType,kinds,0) && 
				ILCanCoerceKind(info,fromType,argType,kinds,0))) 
				|| ((explicit && ILCanCastKind(info,returnType,toType,
				kinds,0) && ILCanCastKind(info,fromType,argType,kinds,0))))
			{
				if(bestMember == NULL || (ILBetterIndirectConversion(info,
					fromType, toType, ILMethod_Signature(member),									ILMethod_Signature(bestMember)) == IL_BETTER_S1))
				{
					bestMember = member;
				}
			}
		}
		arg1Class = ILClass_ParentClass(arg1Class);
	}

	while(arg2Class != 0)
	{
		arg2Class= ILClassResolve(arg2Class);
		member=0;
		while((member = ILClassNextMemberMatch
					(arg2Class, member,
					 IL_META_MEMBERKIND_METHOD, "op_Implicit", 0)) != 0)
		{
			/* Filter out members that aren't interesting */
			if((ILMember_Attrs(member) & METHOD_TYPE_ATTRS) !=
   			(IL_META_METHODDEF_STATIC |IL_META_METHODDEF_SPECIAL_NAME))
			{
				continue;
			}
			method = (ILMethod *)member;
			/* Check that this is the signature we are interested in */
			signature = ILMethod_Signature(method);
			if(!ILType_IsMethod(signature))
			{
				continue;
			}
			returnType=ILTypeGetReturn(signature);
			if(ILTypeNumParams(signature)!=1)
			{
				continue;
			}
			argType=ILTypeGetParam(signature,1);

			if(((ILCanCoerceKind(info,returnType,toType,kinds,0) && 
				ILCanCoerceKind(info,fromType,argType,kinds,0))) 
				|| ((explicit && ILCanCastKind(info,returnType,toType,
				kinds,0) && ILCanCastKind(info,fromType,argType,kinds,0))))
			{
				if(bestMember == NULL || (ILBetterIndirectConversion(info,
					fromType, toType, ILMethod_Signature(member),									ILMethod_Signature(bestMember)) == IL_BETTER_S1))
				{
					bestMember = member;
				}
			}
		}
		arg2Class = ILClass_ParentClass(arg2Class);
	}
	
	if(bestMember)
	{
		signature = ILMethod_Signature(bestMember);
		argType = ILTypeGetParam(signature, 1);
		returnType = ILTypeGetReturn(signature);
		if(itype1)(*itype1)=argType;
		if(itype2)(*itype2)=returnType;
		return 1;
	}

	if(!explicit)return 0;
	
	/* start explicit checks */
	
	arg1Class = ILTypeToClass(info, fromType);
	arg2Class = ILTypeToClass(info, toType);	

	while(arg1Class != 0)
	{
		arg1Class= ILClassResolve(arg1Class);
		member=0;
		while((member = ILClassNextMemberMatch
					(arg1Class, member,
					 IL_META_MEMBERKIND_METHOD, "op_Explicit", 0)) != 0)
		{
			/* Filter out members that aren't interesting */
			if((ILMember_Attrs(member) & METHOD_TYPE_ATTRS) !=
   			(IL_META_METHODDEF_STATIC |IL_META_METHODDEF_SPECIAL_NAME))
			{
				continue;
			}
			method = (ILMethod *)member;
			/* Check that this is the signature we are interested in */
			signature = ILMethod_Signature(method);
			if(!ILType_IsMethod(signature))
			{
				continue;
			}
			returnType=ILTypeGetReturn(signature);
			if(ILTypeNumParams(signature)!=1)
			{
				continue;
			}
			argType=ILTypeGetParam(signature,1);
			
			/* coercion is automatically tried by casting */
			if(ILCanCastKind(info,returnType,toType,kinds,0) && 
				ILCanCastKind(info,fromType,argType,kinds,0))
			{
				if(bestMember == NULL || (ILBetterIndirectConversion(info,
					fromType, toType, ILMethod_Signature(member),									ILMethod_Signature(bestMember)) == IL_BETTER_S1))
				{
					bestMember = member;
				}
			}
		}
		arg1Class = ILClass_ParentClass(arg1Class);
	}

	while(arg2Class != 0)
	{
		arg2Class= ILClassResolve(arg2Class);
		member=0;
		while((member = ILClassNextMemberMatch
					(arg2Class, member,
					 IL_META_MEMBERKIND_METHOD, "op_Explicit", 0)) != 0)
		{
			/* Filter out members that aren't interesting */
			if((ILMember_Attrs(member) & METHOD_TYPE_ATTRS) !=
   			(IL_META_METHODDEF_STATIC |IL_META_METHODDEF_SPECIAL_NAME))
			{
				continue;
			}
			method = (ILMethod *)member;
			/* Check that this is the signature we are interested in */
			signature = ILMethod_Signature(method);
			if(!ILType_IsMethod(signature))
			{
				continue;
			}
			returnType=ILTypeGetReturn(signature);
			if(ILTypeNumParams(signature)!=1)
			{
				continue;
			}
			argType=ILTypeGetParam(signature,1);

			if(ILCanCastKind(info,returnType,toType,kinds,0) && 
				ILCanCastKind(info,fromType,argType,kinds,0))
			{
				if(bestMember == NULL || (ILBetterIndirectConversion(info,
					fromType, toType, ILMethod_Signature(member),									ILMethod_Signature(bestMember)) == IL_BETTER_S1))
				{
					bestMember = member;
				}
			}
		}
		arg2Class = ILClass_ParentClass(arg2Class);
	}

	if(bestMember)
	{
		signature = ILMethod_Signature(bestMember);
		argType = ILTypeGetParam(signature, 1);
		returnType = ILTypeGetReturn(signature);
		if(itype1)(*itype1)=argType;
		if(itype2)(*itype2)=returnType;
		return 1;
	}

	return 0;
}

/*
 * Apply a set of conversion rules to a node.
 */
static void ApplyRules(ILGenInfo *info, ILNode *node,
					   ILNode **parent, ConvertRules *rules,
					   ILType *fromType, ILType *toType)
{
	/* Box or unbox the input value if necessary */
	if(rules->boxClass)
	{
		*parent = ILNode_Box_create(node, rules->boxClass, rules->boxIsEnum);
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
		node = *parent;
	}
	else if(rules->unboxClass)
	{
		*parent = ILNode_Unbox_create(node, rules->unboxClass,
									  rules->boxIsEnum,
									  ILTypeToMachineType(toType));
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
		node = *parent;
	}

	/* Apply a builtin conversion */
	if(rules->builtin)
	{
		ILApplyConversion(info, node, parent, rules->builtin);
		node = *parent;
	}

	/* Call a method to perform the conversion */
	if(rules->method)
	{
		ILMethod *method = (ILMethod *)ILMemberImport
								(info->image, (ILMember *)(rules->method));
		if(!method)
		{
			ILGenOutOfMemory(info);
		}
		if(ILCoerce(info, node, parent, 
					fromType,
					ILTypeGetParam(ILMethod_Signature(method),1),0))
		{
			/* should succeed always */
			node = *parent;
		}
		*parent = ILNode_UserConversion_create
						(node, ILTypeToMachineType(toType), method);
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
		node = *parent;
	}

	/* Cast to an explicit type */
	if(rules->castType)
	{
		*parent = ILNode_CastType_create(node, rules->castType);
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
	}

	/* Convert the object reference "null" into a pointer "null" */
	if(rules->pointerNull)
	{
		*parent = ILNode_NullPtr_create();
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
	}

	/* Convert from System.String to a pointer to System.Char */
	if(rules->stringCharPtr)
	{
		*parent = ILNode_CastStringToCharPtr_create(node);
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
	}

	/* Cast a managed array to a pointer to the element type */
	if(rules->arrayElementPtr)
	{
		*parent = ILNode_CastArrayToElementPtr_create(node,
											ILTypeStripPrefixes(fromType));
		yysetfilename(*parent, yygetfilename(node));
		yysetlinenum(*parent, yygetlinenum(node));
	}
}

/*
 * Determine if there is an implicit constant coercion
 * between two types for a particular node.  Returns the
 * new type, or ILMachineType_Void if there is no coercion.
 */
static ILMachineType CanCoerceConst(ILGenInfo *info, ILNode *node,
									ILType *fromType, ILType *toType)
{
	ILEvalValue value;
	if(ILType_IsPrimitive(fromType) && ILType_IsPrimitive(toType) &&
	   ILNode_EvalConst(node, info, &value))
	{
		/* We can implicitly down-convert some types of constants,
		   but only if the result remains the same */
		if(fromType == ILType_Int32)
		{
			if(toType == ILType_Int8)
			{
				if(value.un.i4Value >= ((ILInt32)(-128)) &&
				   value.un.i4Value <= ((ILInt32)127))
				{
					return ILMachineType_Int8;
				}
			}
			else if(toType == ILType_UInt8)
			{
				if(value.un.i4Value >= ((ILInt32)0) &&
				   value.un.i4Value <= ((ILInt32)255))
				{
					return ILMachineType_UInt8;
				}
			}
			else if(toType == ILType_Int16)
			{
				if(value.un.i4Value >= ((ILInt32)(-32768)) &&
				   value.un.i4Value <= ((ILInt32)32767))
				{
					return ILMachineType_Int16;
				}
			}
			else if(toType == ILType_UInt16)
			{
				if(value.un.i4Value >= ((ILInt32)0) &&
				   value.un.i4Value <= ((ILInt32)65535))
				{
					return ILMachineType_UInt16;
				}
			}
			else if(toType == ILType_UInt32)
			{
				if(value.un.i4Value >= 0)
				{
					return ILMachineType_UInt32;
				}
			}
			else if(toType == ILType_UInt64)
			{
				if(value.un.i4Value >= 0)
				{
					return ILMachineType_UInt64;
				}
			}
		}
		else if(fromType == ILType_Int64)
		{
			if(toType == ILType_UInt64)
			{
				if(value.un.i8Value >= 0)
				{
					return ILMachineType_UInt64;
				}
			}
		}
	}
	else if(ILType_IsPrimitive(fromType) && ILTypeIsEnum(toType) &&
	   	    ILNode_EvalConst(node, info, &value))
	{
		/* We can coerce the integer value zero to any enumerated type */
		if(fromType == ILType_Int8 || fromType == ILType_UInt8 ||
		   fromType == ILType_Int16 || fromType == ILType_UInt16 ||
		   fromType == ILType_Int32 || fromType == ILType_UInt32)
		{
			if(value.un.i4Value == 0)
			{
				return ILTypeToMachineType(toType);
			}
		}
		else if(fromType == ILType_Int64 || fromType == ILType_UInt64)
		{
			if(value.un.i8Value == 0)
			{
				return ILTypeToMachineType(toType);
			}
		}
	}
	return ILMachineType_Void;
}

int ILCanCoerce(ILGenInfo *info, ILType *fromType, ILType *toType,int indirect)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 0, IL_CONVERT_ALL, &rules))
	{
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType, 
												0, IL_CONVERT_ALL,NULL,NULL))
	{
		return 1;
	}
	return 0;
}

int ILCanCoerceKind(ILGenInfo *info, ILType *fromType,
					ILType *toType, int kinds,int indirect)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 0, kinds, &rules))
	{
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,0, 
												kinds,NULL,NULL))
	{
		return 1;
	}
	return 0;
}

int ILCanCoerceNode(ILGenInfo *info, ILNode *node,
				    ILType *fromType, ILType *toType, int indirect)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 0, IL_CONVERT_ALL, &rules))
	{
		return 1;
	}
	else if(CanCoerceConst(info, node, fromType, toType) != ILMachineType_Void)
	{
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,0, 
												IL_CONVERT_ALL,NULL,NULL))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILCanCoerceNodeKind(ILGenInfo *info, ILNode *node,
				        ILType *fromType, ILType *toType, int kinds,
						int indirect)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 0, kinds, &rules))
	{
		return 1;
	}
	else if((kinds & IL_CONVERT_CONSTANT) != 0 &&
	        CanCoerceConst(info, node, fromType, toType) != ILMachineType_Void)
	{
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,0, 
												kinds,NULL,NULL))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILCoerce(ILGenInfo *info, ILNode *node, ILNode **parent,
			 ILType *fromType, ILType *toType,int indirect)
{
	ConvertRules rules;
	ILType *t1,*t2;
	ILMachineType constType;
	if(GetConvertRules(info, fromType, toType, 0, IL_CONVERT_ALL, &rules))
	{
		ApplyRules(info, node, parent, &rules, fromType, toType);
		return 1;
	}
	else if((constType = CanCoerceConst(info, node, fromType, toType))
					!= ILMachineType_Void)
	{
		*parent = ILNode_CastSimple_create(node, constType);
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,0, 
												IL_CONVERT_ALL,&t1,&t2))
	{
		ILCoerce(info,*parent,parent,fromType,t1,0);
		ILCoerce(info,*parent,parent,t1,t2,0);
		ILCoerce(info,*parent,parent,t2,toType,0);
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILCoerceKind(ILGenInfo *info, ILNode *node, ILNode **parent,
			     ILType *fromType, ILType *toType, int kinds,int indirect)
{
	ConvertRules rules;
	ILMachineType constType;
	ILType *t1,*t2;
	if(GetConvertRules(info, fromType, toType, 0, kinds, &rules))
	{
		ApplyRules(info, node, parent, &rules, fromType, toType);
		return 1;
	}
	else if((kinds & IL_CONVERT_CONSTANT) != 0 &&
			(constType = CanCoerceConst(info, node, fromType, toType))
					!= ILMachineType_Void)
	{
		*parent = ILNode_CastSimple_create(node, constType);
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,0, 
												kinds,&t1,&t2))
	{
		ILCoerceKind(info,*parent,parent,fromType,t1,kinds,0);
		ILCoerceKind(info,*parent,parent,t1,t2,kinds,0);
		ILCoerceKind(info,*parent,parent,t2,toType,kinds,0);
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILCanCast(ILGenInfo *info, ILType *fromType, ILType *toType,int indirect)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 1, IL_CONVERT_ALL, &rules))
	{
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,1, 
												IL_CONVERT_ALL,NULL,NULL))
	{
		return 1;
	}
	return 0;
}

int ILCanCastKind(ILGenInfo *info, ILType *fromType,
				  ILType *toType, int kinds,int indirect)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 1, kinds, &rules))
	{
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,1, 
												kinds,NULL,NULL))
	{
		return 1;
	}
	return 0;
}

int ILCast(ILGenInfo *info, ILNode *node, ILNode **parent,
		   ILType *fromType, ILType *toType,int indirect)
{
	ConvertRules rules;
	ILType *t1,*t2;
	if(GetConvertRules(info, fromType, toType, 1, IL_CONVERT_ALL, &rules))
	{
		ApplyRules(info, node, parent, &rules, fromType, toType);
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,1, 
												IL_CONVERT_ALL,&t1,&t2))
	{
		ILCast(info,*parent,parent,fromType,t1,0);
		ILCast(info,*parent,parent,t1,t2,0);
		ILCast(info,*parent,parent,t2,toType,0);
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILCastKind(ILGenInfo *info, ILNode *node, ILNode **parent,
		       ILType *fromType, ILType *toType, int kinds,
			   int indirect)
{
	ConvertRules rules;
	ILType *t1,*t2;
	if(GetConvertRules(info, fromType, toType, 1, kinds, &rules))
	{
		ApplyRules(info, node, parent, &rules, fromType, toType);
		return 1;
	}
	else if(indirect && GetIndirectConvertRules(info,fromType,toType,1, 
												kinds,&t1,&t2))
	{
		ILCastKind(info,*parent,parent,fromType,t1,kinds,0);
		ILCastKind(info,*parent,parent,t1,t2,kinds,0);
		ILCastKind(info,*parent,parent,t2,toType,kinds,0);
		return 1;
	}
	else
	{
		return 0;
	}
}

ILClass *ILGetExplicitConv(ILGenInfo *info, ILType *fromType,
						   ILType *toType, int kinds)
{
	ConvertRules rules;
	if(GetConvertRules(info, fromType, toType, 1, kinds, &rules))
	{
		if(rules.castType)
		{
			return ILTypeToClass(info, rules.castType);
		}
		else if(rules.unboxClass)
		{
			return rules.unboxClass;
		}
		else
		{
			return 0;
		}
	}
	else
	{
		return 0;
	}
}

int ILBetterConversion(ILGenInfo *info, ILType *sType,
					   ILType *t1Type, ILType *t2Type)
{
	if(ILTypeIdentical(t1Type, t2Type))
	{
		return IL_BETTER_NEITHER;
	}
	else if(ILTypeIdentical(sType, t1Type))
	{
		return IL_BETTER_T1;
	}
	else if(ILTypeIdentical(sType, t2Type))
	{
		return IL_BETTER_T2;
	}
	else if(ILCanCoerce(info, t1Type, t2Type,0) &&
	        !ILCanCoerce(info, t2Type, t1Type,0))
	{
		return IL_BETTER_T1;
	}
	else if(ILCanCoerce(info, t2Type, t1Type,0) &&
	        !ILCanCoerce(info, t1Type, t2Type,0))
	{
		return IL_BETTER_T2;
	}
	else if(t1Type == ILType_Int8 &&
	        (t2Type == ILType_UInt8 ||
			 t2Type == ILType_UInt16 ||
			 t2Type == ILType_UInt32 ||
			 t2Type == ILType_UInt64))
	{
		return IL_BETTER_T1;
	}
	else if(t2Type == ILType_Int8 &&
	        (t1Type == ILType_UInt8 ||
			 t1Type == ILType_UInt16 ||
			 t1Type == ILType_UInt32 ||
			 t1Type == ILType_UInt64))
	{
		return IL_BETTER_T2;
	}
	else if(t1Type == ILType_Int16 &&
			(t2Type == ILType_UInt16 ||
			 t2Type == ILType_UInt32 ||
			 t2Type == ILType_UInt64))
	{
		return IL_BETTER_T1;
	}
	else if(t2Type == ILType_Int16 &&
			(t1Type == ILType_UInt16 ||
			 t1Type == ILType_UInt32 ||
			 t1Type == ILType_UInt64))
	{
		return IL_BETTER_T2;
	}
	else if(t1Type == ILType_Int32 &&
			(t2Type == ILType_UInt32 ||
			 t2Type == ILType_UInt16))
	{
		return IL_BETTER_T1;
	}
	else if(t2Type == ILType_Int32 &&
			(t1Type == ILType_UInt32 ||
			 t1Type == ILType_UInt16))
	{
		return IL_BETTER_T2;
	}
	else if(t1Type == ILType_Int64 && t2Type == ILType_UInt64)
	{
		return IL_BETTER_T1;
	}
	else if(t2Type == ILType_Int64 && t1Type == ILType_UInt64)
	{
		return IL_BETTER_T2;
	}
	else
	{
		return IL_BETTER_NEITHER;
	}
}


/* TODO : Figure out the actual conversion rules before using
 * 		  this in the rest of the code . But it's a reasonably
 * 		  good approximation of what I understand about the spec
 */
int ILBetterConversionFrom(ILGenInfo *info, ILType *s1Type,
					   ILType *s2Type, ILType *tType)
{
	if(ILTypeIdentical(s1Type, s2Type))
	{
		return IL_BETTER_NEITHER;
	}
	else if(ILTypeIdentical(s1Type, tType))
	{
		return IL_BETTER_S1;
	}
	else if(ILTypeIdentical(s2Type, tType))
	{
		return IL_BETTER_S2;
	}
	else if(ILCanCoerce(info, s1Type, s2Type,0) &&
	        !ILCanCoerce(info, s2Type, s1Type,0))
	{
		return IL_BETTER_S2;
	}
	else if(ILCanCoerce(info, s2Type, s1Type,0) &&
	        !ILCanCoerce(info, s1Type, s2Type,0))
	{
		return IL_BETTER_S1;
	}
	else if(s1Type == ILType_Int8 &&
	        (s2Type == ILType_UInt8 ||
			 s2Type == ILType_UInt16 ||
			 s2Type == ILType_UInt32 ||
			 s2Type == ILType_UInt64))
	{
		return IL_BETTER_S1;
	}
	else if(s2Type == ILType_Int8 &&
	        (s1Type == ILType_UInt8 ||
			 s1Type == ILType_UInt16 ||
			 s1Type == ILType_UInt32 ||
			 s1Type == ILType_UInt64))
	{
		return IL_BETTER_S2;
	}
	else if(s1Type == ILType_Int16 &&
			(s2Type == ILType_UInt16 ||
			 s2Type == ILType_UInt32 ||
			 s2Type == ILType_UInt64))
	{
		return IL_BETTER_S1;
	}
	else if(s2Type == ILType_Int16 &&
			(s1Type == ILType_UInt16 ||
			 s1Type == ILType_UInt32 ||
			 s1Type == ILType_UInt64))
	{
		return IL_BETTER_S2;
	}
	else if(s1Type == ILType_Int32 &&
			(s2Type == ILType_UInt32 ||
			 s2Type == ILType_UInt16))
	{
		return IL_BETTER_S1;
	}
	else if(s2Type == ILType_Int32 &&
			(s1Type == ILType_UInt32 ||
			 s1Type == ILType_UInt16))
	{
		return IL_BETTER_S2;
	}
	else if(s1Type == ILType_Int64 && s2Type == ILType_UInt64)
	{
		return IL_BETTER_S1;
	}
	else if(s2Type == ILType_Int64 && s1Type == ILType_UInt64)
	{
		return IL_BETTER_S2;
	}
	else
	{
		return IL_BETTER_NEITHER;
	}
}

#ifdef	__cplusplus
};
#endif
