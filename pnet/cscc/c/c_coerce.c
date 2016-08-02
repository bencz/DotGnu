/*
 * c_coerce.c - Test for casts and coercions between C types.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Coercion rules that may be returned from "GetCoerceRules".
 */
#define	C_COERCE_INVALID			0
#define	C_COERCE_OK					(1<<0)
#define	C_COERCE_LOSES_CONST		(1<<1)
#define	C_COERCE_PTR_TO_INT			(1<<2)
#define	C_COERCE_INT_TO_PTR			(1<<3)
#define	C_COERCE_PTR_TO_PTR			(1<<4)
#define	C_COERCE_NULL_PTR			(1<<5)
#define	C_COERCE_SIMPLE				(1<<6)
#define	C_COERCE_C_TO_CS_STRING		(1<<7)
#define	C_COERCE_CSHARP				(1<<8)
#define	C_COERCE_CSHARP_EXPLICIT	(1<<9)
#define	C_COERCE_NULL_REF			(1<<10)

/*
 * C# conversion rules that we can use within C.  Basically, everything
 * except numeric and user-defined.
 */
#define	IL_CONVERT_C_RULES_IMPLICIT	(IL_CONVERT_ENUM | \
							 		 IL_CONVERT_REFERENCE | \
							 		 IL_CONVERT_CONSTANT)
#define	IL_CONVERT_C_RULES_EXPLICIT	(IL_CONVERT_ENUM | \
							 		 IL_CONVERT_REFERENCE | \
							 		 IL_CONVERT_UNBOXING | \
							 		 IL_CONVERT_CONSTANT)

/*
 * Determine if a primitive type is numeric or boolean.
 */
static int TypeIsNumericOrBoolean(int elemType)
{
	switch(elemType)
	{
		case IL_META_ELEMTYPE_BOOLEAN:
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
		case IL_META_ELEMTYPE_U:
		case IL_META_ELEMTYPE_R4:
		case IL_META_ELEMTYPE_R8:
		case IL_META_ELEMTYPE_R:		return 1;
	}
	return 0;
}

/*
 * Determine if a primitive type is integer.
 */
static int TypeIsInteger(int elemType)
{
	switch(elemType)
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
		case IL_META_ELEMTYPE_U:		return 1;
	}
	return 0;
}

/*
 * Determine if a constant value is integer zero, which we can
 * implicitly coerce into the NULL pointer value.
 */
static int IsZero(ILEvalValue *constValue)
{
	switch(constValue->valueType)
	{
		case ILMachineType_Int8:
		case ILMachineType_UInt8:
		case ILMachineType_Int16:
		case ILMachineType_UInt16:
		case ILMachineType_Char:
		case ILMachineType_Int32:
		case ILMachineType_UInt32:
		case ILMachineType_NativeInt:
		case ILMachineType_NativeUInt:
				return (constValue->un.i4Value == 0);

		case ILMachineType_Int64:
		case ILMachineType_UInt64:
				return (constValue->un.i8Value == 0);

		default: break;
	}
	return 0;
}

/*
 * Determine if a type is "void *".
 */
static int IsVoidPtr(ILType *type)
{
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
	{
		return (ILTypeStripPrefixes(ILType_Ref(type)) == ILType_Void);
	}
	return 0;
}

/*
 * Determine if a pointer type is pointing at a "const" element type.
 */
static int IsConstPtr(ILType *type)
{
	if(type != 0 && ILType_IsComplex(type) &&
	   ILType_Kind(type) == IL_TYPE_COMPLEX_PTR)
	{
		return CTypeIsConst(ILType_Ref(type));
	}
	else
	{
		return 0;
	}
}

/*
 * Determine if an integer type is unsigned.
 */
static int IsUnsigned(ILType *type)
{
	type = ILTypeGetEnumType(ILTypeStripPrefixes(type));
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_U1:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
			case IL_META_ELEMTYPE_U4:
			case IL_META_ELEMTYPE_U8:
			case IL_META_ELEMTYPE_U:		return 1;
		}
	}
	return 0;
}

/*
 * Determine if two function signatures have the same "shape".
 * This is needed to handle cases like "qsort", where the prototype
 * of the comparison function may not be identical to that of the
 * parameter prototype, but it is compatible enough to use in an
 * indirect function call.
 */
static int SameShape(ILType *type1, ILType *type2)
{
	unsigned long numParams;
	unsigned long param;
	type1 = ILTypeStripPrefixes(type1);
	type2 = ILTypeStripPrefixes(type2);
	if(type1 && type2 && ILType_IsComplex(type1) && ILType_IsComplex(type2) &&
	   ILType_Kind(type1) == ILType_Kind(type2))
	{
		if(ILType_Kind(type1) == IL_TYPE_COMPLEX_PTR)
		{
			/* Pointer types always have the same shape */
			return 1;
		}
		else if((ILType_Kind(type1) & IL_TYPE_COMPLEX_METHOD) != 0)
		{
			/* Check the return type and parameters in method signatures */
			if(!SameShape(ILTypeGetReturn(type1), ILTypeGetReturn(type2)))
			{
				return 0;
			}
			if(ILType_CallConv(type1) != ILType_CallConv(type2))
			{
				return 0;
			}
			numParams = ILTypeNumParams(type1);
			if(ILTypeNumParams(type2) != numParams)
			{
				return 0;
			}
			for(param = 1; param <= numParams; ++param)
			{
				if(!SameShape(ILTypeGetParam(type1, param),
							  ILTypeGetParam(type2, param)))
				{
					return 0;
				}
			}
			return 1;
		}
	}
	return ILTypeIdentical(type1, type2);
}

/*
 * Determine the coercion rules for coercing "fromType" to "toType".
 */
static int GetCoerceRules(ILType *fromType, ILType *toType,
						  ILEvalValue *constValue)
{
	int constFlags;

	/* Strip the outer prefixes from the types, as they apply to
	   the l-value containing the type, not the type itself */
	if(!CTypeIsFunctionPtr(fromType))
	{
		fromType = ILTypeGetEnumType(ILTypeStripPrefixes(fromType));
	}
	if(!CTypeIsFunctionPtr(toType))
	{
		toType = ILTypeGetEnumType(ILTypeStripPrefixes(toType));
	}

	/* Determine how to perform the coercion, starting at "fromType" */
	if(ILType_IsPrimitive(fromType))
	{
		if(ILType_IsPrimitive(toType))
		{
			if(TypeIsNumericOrBoolean(ILType_ToElement(fromType)) &&
			   TypeIsNumericOrBoolean(ILType_ToElement(toType)))
			{
				/* Can coerce any numeric type or bool to any other
				   numeric type or bool */
				return C_COERCE_SIMPLE;
			}
		}
		else if(CTypeIsPointer(toType) || CTypeIsFunctionPtr(toType))
		{
			if(TypeIsInteger(ILType_ToElement(fromType)))
			{
				if(constValue != 0 && IsZero(constValue))
				{
					/* Implicit conversion of 0 into a NULL pointer value.
					   This is always OK, as ANSI C defines this behaviour */
					return C_COERCE_NULL_PTR;
				}
				else
				{
					/* Coercing an integer type to a pointer */
					return C_COERCE_INT_TO_PTR;
				}
			}
		}
	}
	else if(ILType_IsValueType(fromType))
	{
		/* Coercing a struct, union, or array type: must be identical */
		if(ILType_IsValueType(toType))
		{
			if(ILClassResolve(ILType_ToValueType(fromType)) ==
			   ILClassResolve(ILType_ToValueType(toType)))
			{
				return C_COERCE_OK;
			}
		}
	}
	else if(CTypeIsPointer(fromType) || CTypeIsFunctionPtr(fromType))
	{
		/* Coercing a pointer type */
		if(IsConstPtr(fromType) && !IsConstPtr(toType))
		{
			constFlags = C_COERCE_LOSES_CONST;
		}
		else
		{
			constFlags = 0;
		}
		if(CTypeIsIdentical(fromType, toType))
		{
			/* Coercing a pointer type to itself */
			return C_COERCE_OK | constFlags;
		}
		else if(CTypeIsPointer(toType) || CTypeIsFunctionPtr(toType))
		{
			if(IsVoidPtr(fromType) || IsVoidPtr(toType))
			{
				/* Coercing to or from "void *" is always ok */
				return C_COERCE_OK | constFlags;
			}
			else
			{
				/* Coercing between pointers of different types */
				return C_COERCE_OK | C_COERCE_PTR_TO_PTR | constFlags;
			}
		}
		else if(ILType_IsPrimitive(toType) &&
				TypeIsInteger(ILType_ToElement(toType)))
		{
			/* Coercing a pointer to an integer type */
			return C_COERCE_PTR_TO_INT | constFlags;
		}
		else if(toType == ILType_Boolean)
		{
			/* Coercing a pointer to "_Bool" */
			return C_COERCE_SIMPLE | constFlags;
		}
	}
	else if(CTypeIsFunction(fromType) && CTypeIsFunctionPtr(toType))
	{
		/* We are coecing a function reference to a pointer destination */
		if(CTypeIsIdentical(fromType, ILTypeStripPrefixes(toType)))
		{
			return C_COERCE_OK;
		}

		/* We can coerce with a warning if the two function pointers
		   have identical "shape" */
		if(SameShape(fromType, toType))
		{
			return C_COERCE_OK | C_COERCE_PTR_TO_PTR;
		}
	}

	/* Deal with coercions between C and C# strings */
	if(CTypeIsPointer(fromType) &&
	   ILTypeStripPrefixes(CTypeGetPtrRef(fromType)) == ILType_Int8 &&
	   ILTypeIsStringClass(toType))
	{
		return C_COERCE_C_TO_CS_STRING;
	}

	/* We allow identical C# types to be coerced */
	if(ILTypeIdentical(fromType, toType))
	{
		return C_COERCE_OK;
	}

	/* Test for implicit coercion of zero to a C# reference type.
	   In this case, we replace the zero with "null" */
	if(ILTypeIsReference(toType))
	{
		if(TypeIsInteger(ILType_ToElement(fromType)))
		{
			if(constValue != 0 && IsZero(constValue))
			{
				/* Implicit conversion of 0 into a NULL reference value */
				return C_COERCE_NULL_REF;
			}
		}
	}

	/* Determine if we can perform a C# type coercion */
	if(ILCanCoerceKind(&CCCodeGen, fromType, toType,
					   IL_CONVERT_C_RULES_IMPLICIT, 0))
	{
		return C_COERCE_CSHARP;
	}
	if(ILCanCastKind(&CCCodeGen, fromType, toType,
					 IL_CONVERT_C_RULES_EXPLICIT, 0))
	{
		return C_COERCE_CSHARP_EXPLICIT;
	}

	/* We will get here if we don't have a C type or we have
	   a completely invalid combination of C types */
	return C_COERCE_INVALID;
}

int CCanCoerce(ILType *fromType, ILType *toType)
{
	return (GetCoerceRules(fromType, toType, 0) != C_COERCE_INVALID);
}

int CCanCoerceValue(CSemValue fromValue, ILType *toType)
{
	if(!CSemIsRValue(fromValue))
	{
		return 0;
	}
	else
	{
		return (GetCoerceRules(CSemGetType(fromValue), toType,
							   CSemGetConstant(fromValue)) != C_COERCE_INVALID);
	}
}

/*
 * Create a simple cast node and fix up its line number information.
 */
static ILNode *SimpleCast(ILNode *node, ILMachineType type)
{
	ILNode *newNode = ILNode_CastSimple_create(node, type);
	CGenCloneLine(newNode, node);
	return newNode;
}

/*
 * Apply a coercion or cast to a specific node.
 */
static CSemValue ApplyCoercion(ILGenInfo *info, ILNode *node, ILNode **parent,
						  	   CSemValue fromValue, ILType *toType,
							   int explicit)
{
	int rules;
	ILEvalValue value;

	/* Get the coercion rules to apply */
	rules = GetCoerceRules(CSemGetType(fromValue), toType,
						   CSemGetConstant(fromValue));
	if(rules == C_COERCE_INVALID)
	{
		return fromValue;
	}

	/* Handle C# coercions and casts */
	if(rules == C_COERCE_CSHARP_EXPLICIT && !explicit)
	{
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  _("cannot cast from `%s' to `%s'"),
					  CTypeToName(info, CSemGetType(fromValue)),
					  CTypeToName(info, toType));
	}
	if(rules == C_COERCE_CSHARP)
	{
		ILEvalValue *constValue = CSemGetConstant(fromValue);
		if(constValue && constValue->valueType == ILMachineType_ObjectRef &&
		   constValue->un.oValue == 0)
		{
			/* We are coercing null to itself, so preserve the constant */
			value.valueType = ILMachineType_ObjectRef;
			value.un.oValue = 0;
			CSemSetConstant(fromValue, toType, value);
			return fromValue;
		}
		ILCoerceKind(info, node, parent, CSemGetType(fromValue), toType,
				     IL_CONVERT_C_RULES_IMPLICIT, 0);
		CSemSetRValue(fromValue, toType);
		return fromValue;
	}
	if(rules == C_COERCE_CSHARP_EXPLICIT)
	{
		ILCastKind(info, node, parent, CSemGetType(fromValue), toType,
			       IL_CONVERT_C_RULES_EXPLICIT, 0);
		CSemSetRValue(fromValue, toType);
		return fromValue;
	}

	/* Print warnings if necessary */
	if(!explicit)
	{
		if((rules & C_COERCE_LOSES_CONST) != 0)
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
				_("discarding `const' qualifier from pointer target type"));
		}
		if((rules & C_COERCE_PTR_TO_INT) != 0)
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
				_("makes integer from pointer without a cast"));
		}
		if((rules & C_COERCE_INT_TO_PTR) != 0)
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
				_("makes pointer from integer without a cast"));
		}
		if((rules & C_COERCE_PTR_TO_PTR) != 0)
		{
			CCWarningOnLine(yygetfilename(node), yygetlinenum(node),
				_("incompatible pointer types"));
		}
	}

	/* Determine the kind of cast to apply */
	if((rules & C_COERCE_PTR_TO_INT) != 0)
	{
		if(IsUnsigned(toType))
		{
			/* Coerce to "native unsigned int" first */
			node = *parent = SimpleCast(node, ILMachineType_NativeUInt);
		}
		else
		{
			/* Coerce to "native int" first */
			node = *parent = SimpleCast(node, ILMachineType_NativeInt);
		}
		*parent = SimpleCast(node, ILTypeToMachineType(toType));
		CSemSetRValue(fromValue, toType);
	}
	else if((rules & C_COERCE_INT_TO_PTR) != 0)
	{
		if(IsUnsigned(CSemGetType(fromValue)))
		{
			/* Coerce to "native unsigned int" first */
			node = *parent = SimpleCast(node, ILMachineType_NativeUInt);
		}
		else
		{
			/* Coerce to "native int" first */
			node = *parent = SimpleCast(node, ILMachineType_NativeInt);
		}
		*parent = SimpleCast(node, ILMachineType_UnmanagedPtr);
		CSemSetRValue(fromValue, toType);
	}
	else if((rules & C_COERCE_NULL_PTR) != 0)
	{
		/* Replace the current node with a constant "null" pointer value */
		*parent = ILNode_NullPtr_create();
		CGenCloneLine(*parent, node);
		value.valueType = ILMachineType_UnmanagedPtr;
		value.un.i4Value = 0;
		CSemSetConstant(fromValue, toType, value);
	}
	else if((rules & C_COERCE_NULL_REF) != 0)
	{
		/* Replace the current node with a constant "null" reference value */
		*parent = ILNode_Null_create();
		CGenCloneLine(*parent, node);
		value.valueType = ILMachineType_ObjectRef;
		value.un.i4Value = 0;
		CSemSetConstant(fromValue, toType, value);
	}
	else if((rules & C_COERCE_SIMPLE) != 0)
	{
		/* Apply a simple numeric cast if "from" and "to" are different */
		ILMachineType ftype = ILTypeToMachineType(CSemGetType(fromValue));
		ILMachineType ttype = ILTypeToMachineType(toType);
		if(ftype != ttype)
		{
			*parent = SimpleCast(node, ttype);
			if(CSemGetConstant(fromValue) != 0)
			{
				/* Cast the constant value within the compiler if possible */
				value = *(CSemGetConstant(fromValue));
				if(ILGenCastConst(info, &value, ftype, ttype))
				{
					/* Return the coerced constant as a new semantic value */
					CSemSetConstant(fromValue, toType, value);
				}
				else
				{
					/* We may get here if performing a cast that we cannot
					   know the value of until runtime.  e.g. converting
					   "int64" into "native int" may or may not lose bits */
					CSemSetRValue(fromValue, toType);
				}
			}
			else if(CSemIsDynConstant(fromValue))
			{
				/* Dynamic constant conversion: result is still a constant */
				CSemSetDynConstant(fromValue, toType);
			}
			else
			{
				CSemSetRValue(fromValue, toType);
			}
		}
		else if(CSemGetConstant(fromValue) != 0)
		{
			/* Constant conversion that doesn't change the machine type */
			CSemLToRValue(fromValue);
			fromValue.type__ = toType;
		}
		else if(CSemIsDynConstant(fromValue))
		{
			/* Dynamic constant conversion that doesn't change machine type */
			fromValue.type__ = toType;
		}
		else
		{
			CSemSetRValue(fromValue, toType);
		}
	}
	else if((rules & C_COERCE_C_TO_CS_STRING) != 0)
	{
		/* Convert a C string into a C# string */
		if(yyisa(node, ILNode_CString))
		{
			/* The argument is a constant, so merely change its node type */
			*parent = ILNode_String_create
				(((ILNode_CString *)node)->str, ((ILNode_CString *)node)->len);
			CGenCloneLine(*parent, node);
		}
		else
		{
			/* We need to call "PtrToStringAnsi" to convert the string */
			*parent = ILNode_CToCSharpString_create(node);
			CGenCloneLine(*parent, node);
		}
		CSemSetRValue(fromValue, toType);
	}
	else
	{
		CSemSetRValue(fromValue, toType);
	}

	/* Return the modified semantic value to the caller */
	return fromValue;
}

CSemValue CCoerceNode(ILGenInfo *info, ILNode *node, ILNode **parent,
				      CSemValue fromValue, ILType *toType)
{
	return ApplyCoercion(info, node, parent, fromValue, toType, 0);
}

CSemValue CCastNode(ILGenInfo *info, ILNode *node, ILNode **parent,
				 	CSemValue fromValue, ILType *toType)
{
	return ApplyCoercion(info, node, parent, fromValue, toType, 1);
}

#ifdef	__cplusplus
};
#endif
