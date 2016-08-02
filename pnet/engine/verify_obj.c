/*
 * verify_obj.c - Verify instructions related to objects.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if defined(IL_VERIFY_GLOBALS)

/*
 * Get a field token from within a method's code.
 */
static ILField *GetFieldToken(ILExecProcess *process, ILMethod *method, unsigned char *pc)
{
	ILUInt32 token;
	ILField *fieldInfo;

	/* Fetch the token from the instruction's arguments */
	if(pc[0] != IL_OP_PREFIX)
	{
		token = IL_READ_UINT32(pc + 1);
	}
	else
	{
		token = IL_READ_UINT32(pc + 2);
	}

	/* Get the token and resolve it */
	fieldInfo = ILProgramItemToField((ILProgramItem *)
						ILImageTokenInfo(ILProgramItem_Image(method), token));
	if(!fieldInfo)
	{
		return 0;
	}
	fieldInfo = (ILField *)ILMemberResolveToInstance((ILMember *)fieldInfo, method);
	if(!fieldInfo)
	{
		return 0;
	}

	/* Check the accessibility of the field */
	if(!ILMemberAccessible((ILMember *)fieldInfo, ILMethod_Owner(method)))
	{
		return 0;
	}

	/* Literal fields can never be used with IL instructions because
	   they don't occupy any physical space at runtime.  Their values
	   are supposed to have been expanded by the compiler */
	if(ILField_IsLiteral(fieldInfo))
	{
		return 0;
	}

	/* Make sure that the field's class has been laid out */
	if(!_ILLayoutClass(process, ILField_Owner(fieldInfo)))
	{
		return 0;
	}

	/* We have the requested field */
	return fieldInfo;
}

/*
 * Get a particular system value type.
 */
static ILType *GetSystemValueType(ILMethod *method, const char *name)
{
	ILClass *classInfo;
	classInfo = ILClassResolveSystem(ILProgramItem_Image(method), 0,
									 name, "System");
	if(classInfo)
	{
		return ILType_FromValueType(classInfo);
	}
	else
	{
		return 0;
	}
}

#elif defined(IL_VERIFY_LOCALS)

ILField *fieldInfo;
ILMethod *methodInfo;
ILProgramItem *item;
ILClass *stackItemClass;

#else /* IL_VERIFY_CODE */

#define	IsCPPointer(type,typeInfo,classInfo)	\
			((((type) == ILEngineType_M || (type) == ILEngineType_T) && \
			  ILTypeIdentical(typeInfo, ILClassToType(classInfo))) || \
			 (unsafeAllowed && \
			  ((type) == ILEngineType_I || (type) == ILEngineType_I4)))

#define	IsCPSrcPointer(type,typeInfo,classInfo)	\
			((((type) == ILEngineType_M || \
			   (type) == ILEngineType_CM || \
			   (type) == ILEngineType_T) && \
			  ILTypeIdentical(typeInfo, ILClassToType(classInfo))) || \
			 (unsafeAllowed && \
			  ((type) == ILEngineType_I || (type) == ILEngineType_I4)))

case IL_OP_CPOBJ:
{
	/* Copy a value type */
	classInfo = GetValueTypeToken(method, pc);
	if(classInfo &&
	   IsCPPointer(STK_BINARY_1, stack[stackSize - 2].typeInfo, classInfo) &&
	   IsCPSrcPointer(STK_BINARY_2, stack[stackSize - 1].typeInfo, classInfo))
	{
		ILCoderCopyObject(coder, STK_BINARY_1, STK_BINARY_2, classInfo);
		stackSize -= 2;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_CASTCLASS:
{
	/* Cast an object to a specific class */
	classInfo = GetClassToken(method, pc);
	if(STK_UNARY == ILEngineType_O)
	{
		if(classInfo != 0)
		{
			ILCoderCastClass(coder, classInfo, 1, &prefixInfo);
			stack[stackSize - 1].typeInfo = ILClassToType(classInfo);
		}
		else
		{
			ThrowSystem("System", "TypeLoadException");
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_CASTCLASS_PREFIX,
						 VALID_NO_CASTCLASS);
}
break;

case IL_OP_ISINST:
{
	/* Determine if an object belongs to a specific class */
	classInfo = GetClassToken(method, pc);
	if(STK_UNARY == ILEngineType_O)
	{
		if(classInfo != 0)
		{
			ILCoderCastClass(coder, classInfo, 0, &prefixInfo);
			stack[stackSize - 1].typeInfo = ILClassToType(classInfo);
		}
		else
		{
			ThrowSystem("System", "TypeLoadException");
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_BOX:
{
	/* Box a value into an object */
	classInfo = GetClassToken(method, pc);
	if(classInfo && ILClassIsValueType(classInfo))
	{
		if(_ILCoderBoxValue(_ILExecThreadProcess(thread),
							stack[stackSize - 1].engineType,
							stack[stackSize - 1].typeInfo, classInfo))
		{
			stack[stackSize - 1].engineType = ILEngineType_O;
			stack[stackSize - 1].typeInfo = ILType_FromClass(classInfo);
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
	}
	else if(!classInfo)
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_UNBOX:
{
	/* Unbox a value from an object */
	classInfo = GetValueTypeToken(method, pc);
	if(classInfo && STK_UNARY == ILEngineType_O)
	{
		/* Cast the value to the required type first.  This takes
		   care of throwing the "InvalidCastException" if the value
		   is not of the correct class */
		if(!IsSubClass(stack[stackSize - 1].typeInfo, classInfo))
		{
			ILCoderCastClass(coder, classInfo, 1, &prefixInfo);
		}

		/* Unbox the object to produce a managed pointer */
		ILCoderUnbox(coder, classInfo, &prefixInfo);
		stack[stackSize - 1].engineType = ILEngineType_M;
		stack[stackSize - 1].typeInfo = ILClassToType(classInfo);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_UNBOX_PREFIX, VALID_NO_UNBOX);
}
break;

case IL_OP_UNBOX_ANY:
{
	classInfo = GetClassToken(method, pc);

	/* Unbox a value from an object */
	if(classInfo && STK_UNARY == ILEngineType_O)
	{
		classType = ILClassToType(classInfo);
		if(ILType_IsClass(classType))
		{
			/* Cast the value to the required type first.  This takes
			   care of throwing the "InvalidCastException" if the value
			   is not of the correct class */
			if(!IsSubClass(stack[stackSize - 1].typeInfo, classInfo))
			{
				ILCoderCastClass(coder, classInfo, 1, &prefixInfo);
			}
			stack[stackSize - 1].typeInfo = classType;
		}
		else
		{
			if(!(AssignCompatible(method, &(stack[stackSize - 1]),
								  classType,
								  unsafeAllowed)))
			{
				/* To throw the InvalitCastException in this case. */
				ILCoderCastClass(coder, classInfo, 1, &prefixInfo);
			}
			/* Unbox the object to produce a managed pointer */
			stackItemClass = ILClassFromType(ILProgramItem_Image(method),
											 0,
											 stack[stackSize - 1].typeInfo, 0);
			if(!stackItemClass)
			{
				ThrowSystem("System", "TypeLoadException");
			}
			/* First get the pointer to the value */
			ILCoderUnbox(coder, stackItemClass, &prefixInfo);
			stack[stackSize - 1].engineType = ILEngineType_M;
			/* We have to dereference the value in this case. */
			ILCoderPtrAccessManaged(coder, IL_OP_LDOBJ, stackItemClass,
									&prefixInfo);
			stack[stackSize - 1].engineType = TypeToEngineType(classType);
			stack[stackSize - 1].typeInfo = classType;
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_LDFLD:
{
	/* Load the contents of an object field.  Note: according to the
	   ECMA spec, it is possible to access a static field by way of
	   "ldfld", "stfld", and "ldflda", even though there are other
	   instructions that are normally used for that.  The only difference
	   is that the type of the object is verified to ensure that it
	   is consistent with the field's type */
	fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
	if(fieldInfo)
	{
		classType = ILField_Type(fieldInfo);
		if(STK_UNARY == ILEngineType_O)
		{
			/* Accessing a field within an object reference */
			if(IsSubClass(stack[stackSize - 1].typeInfo,
						  ILField_Owner(fieldInfo)))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderLoadField(coder, ILEngineType_O,
									 stack[stackSize - 1].typeInfo,
									 fieldInfo, classType, &prefixInfo);
				}
				else
				{
					ILCoderPop(coder, ILEngineType_O, ILType_Invalid);
					ILCoderLoadStaticField(coder, fieldInfo, classType,
										   &prefixInfo);
				}
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(!unsafeAllowed &&
				(STK_UNARY == ILEngineType_M ||
				 STK_UNARY == ILEngineType_CM ||
				 STK_UNARY == ILEngineType_T))
		{
			/* Accessing a field within a pointer to a managed value */
			if(IsSubClass(stack[stackSize - 1].typeInfo,
						  ILField_Owner(fieldInfo)))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderLoadField(coder, STK_UNARY,
									 stack[stackSize - 1].typeInfo,
									 fieldInfo, classType, &prefixInfo);
				}
				else
				{
					ILCoderPop(coder, STK_UNARY, ILType_Invalid);
					ILCoderLoadStaticField(coder, fieldInfo, classType,
										   &prefixInfo);
				}
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(STK_UNARY == ILEngineType_MV)
		{
			/* Accessing a field within a managed value */
			if(IsSubClass(stack[stackSize - 1].typeInfo,
						  ILField_Owner(fieldInfo)))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderLoadField(coder, ILEngineType_MV,
									 stack[stackSize - 1].typeInfo,
									 fieldInfo, classType, &prefixInfo);
				}
				else
				{
					ILCoderPop(coder, ILEngineType_MV,
							   stack[stackSize - 1].typeInfo);
					ILCoderLoadStaticField(coder, fieldInfo, classType,
										   &prefixInfo);
				}
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(unsafeAllowed &&
				(STK_UNARY == ILEngineType_I ||
				 STK_UNARY == ILEngineType_I4 ||
				 STK_UNARY == ILEngineType_M ||
				 STK_UNARY == ILEngineType_CM ||
				 STK_UNARY == ILEngineType_T))
		{
			/* Accessing a field within an unmanaged pointer.
			   We assume that the types are consistent */
			if(!ILField_IsStatic(fieldInfo))
			{
				ILCoderLoadField(coder, STK_UNARY,
								 stack[stackSize - 1].typeInfo,
								 fieldInfo, classType, &prefixInfo);
			}
			else
			{
				ILCoderPop(coder, STK_UNARY, ILType_Invalid);
				ILCoderLoadStaticField(coder, fieldInfo, classType,
									   &prefixInfo);
			}
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
		stack[stackSize - 1].engineType = TypeToEngineType(classType);
		stack[stackSize - 1].typeInfo = classType;
	}
	else
	{
		/* The ECMA spec specifies that an exception should be thrown
		   if the field cannot be found */
		ThrowSystem("System", "MissingFieldException");
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_LDFLD_PREFIX, VALID_NO_LDFLD);
}
break;

case IL_OP_LDFLDA:
{
	/* Load the address of an object field */
	fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
	if(fieldInfo)
	{
		classType = ILField_Type(fieldInfo);
		if(STK_UNARY == ILEngineType_O)
		{
			/* Accessing a field within an object reference */
			if(IsSubClass(stack[stackSize - 1].typeInfo,
						  ILField_Owner(fieldInfo)))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderLoadFieldAddr(coder, ILEngineType_O,
									     stack[stackSize - 1].typeInfo,
									     fieldInfo, classType);
				}
				else
				{
					ILCoderPop(coder, ILEngineType_O, ILType_Invalid);
					ILCoderLoadStaticFieldAddr(coder, fieldInfo, classType);
				}
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(STK_UNARY == ILEngineType_M ||
				STK_UNARY == ILEngineType_CM ||
				STK_UNARY == ILEngineType_T)
		{
			/* Accessing a field within a pointer to a managed value */
			if(IsSubClass(stack[stackSize - 1].typeInfo,
						  ILField_Owner(fieldInfo)))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderLoadFieldAddr(coder, STK_UNARY,
									     stack[stackSize - 1].typeInfo,
									     fieldInfo, classType);
				}
				else
				{
					ILCoderPop(coder, STK_UNARY, ILType_Invalid);
					ILCoderLoadStaticFieldAddr(coder, fieldInfo, classType);
				}
			}
			else if(unsafeAllowed)
			{
				/* Treat the access as unmanaged, because the program
				   has changed pointer types on us */
				goto unmanagedAddr;
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(unsafeAllowed &&
				(STK_UNARY == ILEngineType_I ||
				 STK_UNARY == ILEngineType_I4))
		{
			/* Accessing a field within an unmanaged pointer.
			   We assume that the types are consistent */
		unmanagedAddr:
			if(!ILField_IsStatic(fieldInfo))
			{
				ILCoderLoadFieldAddr(coder, STK_UNARY,
								     stack[stackSize - 1].typeInfo,
								     fieldInfo, classType);
			}
			else
			{
				ILCoderPop(coder, STK_UNARY, ILType_Invalid);
				ILCoderLoadStaticFieldAddr(coder, fieldInfo, classType);
			}

			/* Taking the address of a field within an unmanaged
			   pointer always returns an unmanaged pointer */
			stack[stackSize - 1].engineType = ILEngineType_I;
			stack[stackSize - 1].typeInfo = 0;
			break;
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
		stack[stackSize - 1].engineType = ILEngineType_M;
		stack[stackSize - 1].typeInfo = classType;
	}
	else
	{
		/* The ECMA spec specifies that an exception should be thrown
		   if the field cannot be found */
		ThrowSystem("System", "MissingFieldException");
	}
}
break;

case IL_OP_STFLD:
{
	/* Store a value into an object field */
	fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
	if(fieldInfo)
	{
		classType = ILField_Type(fieldInfo);
		if(STK_BINARY_1 == ILEngineType_O)
		{
			/* Accessing a field within an object reference */
			if(IsSubClass(stack[stackSize - 2].typeInfo,
						  ILField_Owner(fieldInfo)) &&
			   AssignCompatible(method, &(stack[stackSize - 1]),
								classType, unsafeAllowed))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderStoreField(coder, ILEngineType_O,
									  stack[stackSize - 2].typeInfo,
									  fieldInfo, classType,
									  STK_BINARY_2, &prefixInfo);
				}
				else
				{
					ILCoderStoreStaticField(coder, fieldInfo, classType,
											STK_BINARY_2, &prefixInfo);
					ILCoderPop(coder, ILEngineType_O, ILType_Invalid);
				}
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(!unsafeAllowed &&
		        (STK_BINARY_1 == ILEngineType_M ||
				 STK_BINARY_1 == ILEngineType_CM ||
				 STK_BINARY_1 == ILEngineType_T))
		{
			/* Accessing a field within a pointer to a managed value */
			if(IsSubClass(stack[stackSize - 2].typeInfo,
						  ILField_Owner(fieldInfo)) &&
			   AssignCompatible(method, &(stack[stackSize - 1]),
			   					classType, unsafeAllowed))
			{
				if(!ILField_IsStatic(fieldInfo))
				{
					ILCoderStoreField(coder, STK_BINARY_1,
									  stack[stackSize - 2].typeInfo,
									  fieldInfo, classType, STK_BINARY_2,
									  &prefixInfo);
				}
				else
				{
					ILCoderStoreStaticField(coder, fieldInfo, classType,
											STK_BINARY_2, &prefixInfo);
					ILCoderPop(coder, STK_BINARY_2, ILType_Invalid);
				}
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if(unsafeAllowed &&
				(STK_BINARY_1 == ILEngineType_I ||
				 STK_BINARY_1 == ILEngineType_I4 ||
				 STK_BINARY_1 == ILEngineType_M ||
				 STK_BINARY_1 == ILEngineType_CM ||
				 STK_BINARY_1 == ILEngineType_T))
		{
			/* Accessing a field within an unmanaged pointer.
			   We assume that the types are consistent */
			if(!ILField_IsStatic(fieldInfo))
			{
				ILCoderStoreField(coder, STK_BINARY_1,
								  stack[stackSize - 2].typeInfo,
								  fieldInfo, classType, STK_BINARY_2,
								  &prefixInfo);
			}
			else
			{
				ILCoderStoreStaticField(coder, fieldInfo, classType,
										STK_BINARY_2, &prefixInfo);
				ILCoderPop(coder, STK_BINARY_1, ILType_Invalid);
			}
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
		stackSize -= 2;
	}
	else
	{
		/* The ECMA spec specifies that an exception should be thrown
		   if the field cannot be found */
		ThrowSystem("System", "MissingFieldException");
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_STFLD_PREFIX, VALID_NO_STFLD);
}
break;

case IL_OP_LDSFLD:
{
	/* Load the contents of a static field */
	fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
	if(fieldInfo)
	{
		classType = ILField_Type(fieldInfo);
		ILCoderLoadStaticField(coder, fieldInfo, classType, &prefixInfo);
		stack[stackSize].engineType = TypeToEngineType(classType);
		stack[stackSize].typeInfo = classType;
		++stackSize;
	}
	else
	{
		/* The ECMA spec specifies that an exception should be thrown
		   if the field cannot be found */
		ThrowSystem("System", "MissingFieldException");
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_LDSFLD_PREFIX, VALID_NO_NONE);
}
break;

case IL_OP_LDSFLDA:
{
	/* Load the address of a static field */
	fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
	if(fieldInfo)
	{
		classType = ILField_Type(fieldInfo);
		ILCoderLoadStaticFieldAddr(coder, fieldInfo, classType);
		stack[stackSize].engineType = ILEngineType_M;
		stack[stackSize].typeInfo = classType;
		++stackSize;
	}
	else
	{
		/* The ECMA spec specifies that an exception should be thrown
		   if the field cannot be found */
		ThrowSystem("System", "MissingFieldException");
	}
}
break;

case IL_OP_STSFLD:
{
	/* Store a value into a static field */
	fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
	if(fieldInfo)
	{
		classType = ILField_Type(fieldInfo);
		if(AssignCompatible(method, &(stack[stackSize - 1]),
							classType, unsafeAllowed))
		{
			ILCoderStoreStaticField(coder, fieldInfo, classType, STK_UNARY,
									&prefixInfo);
			--stackSize;
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
	}
	else
	{
		/* The ECMA spec specifies that an exception should be thrown
		   if the field cannot be found */
		ThrowSystem("System", "MissingFieldException");
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_STSFLD_PREFIX, VALID_NO_NONE);
}
break;

case IL_OP_MKREFANY:
{
	/* Make a typed reference from a pointer */
	classInfo = GetClassToken(method, pc);
	if(classInfo)
	{
		classType = ILClassToType(classInfo);
		if((STK_UNARY == ILEngineType_M || STK_UNARY == ILEngineType_T) &&
		   ILTypeIdentical(classType, stack[stackSize - 1].typeInfo))
		{
			ILCoderMakeTypedRef(coder, classInfo);
			stack[stackSize - 1].engineType = ILEngineType_TypedRef;
			stack[stackSize - 1].typeInfo = 0;
		}
		else if(unsafeAllowed &&
		        (STK_UNARY == ILEngineType_I || STK_UNARY == ILEngineType_I4))
		{
			ILCoderToPointer(coder, STK_UNARY, (ILEngineStackItem *)0);
			ILCoderMakeTypedRef(coder, classInfo);
			stack[stackSize - 1].engineType = ILEngineType_TypedRef;
			stack[stackSize - 1].typeInfo = 0;
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_REFANYVAL:
{
	/* Extract the address from a typed reference */
	classInfo = GetClassToken(method, pc);
	if(classInfo && STK_UNARY == ILEngineType_TypedRef)
	{
		ILCoderRefAnyVal(coder, classInfo);
		stack[stackSize - 1].engineType = ILEngineType_M;
		stack[stackSize - 1].typeInfo = ILClassToType(classInfo);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_REFANYTYPE:
{
	/* Extract the type from a typed reference */
	if(STK_UNARY == ILEngineType_TypedRef)
	{
		ILCoderRefAnyType(coder);
		stack[stackSize - 1].engineType = ILEngineType_MV;
		stack[stackSize - 1].typeInfo =
				GetSystemValueType(method, "RuntimeTypeHandle");
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_LDTOKEN:
{
	/* Load a token onto the stack */
	item = (ILProgramItem *)ILImageTokenInfo(ILProgramItem_Image(method),
											 IL_READ_UINT32(pc + 1));
	if(item)
	{
		if((classInfo = ILProgramItemToClass(item)) != 0)
		{
			classInfo = GetClassToken(method, pc);
			if(classInfo)
			{
				ILCoderPushToken(coder, (ILProgramItem *)classInfo);
				stack[stackSize].engineType = ILEngineType_MV;
				stack[stackSize].typeInfo =
						GetSystemValueType(method, "RuntimeTypeHandle");
				++stackSize;
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if((methodInfo = ILProgramItemToMethod(item)) != 0)
		{
			methodInfo = GetMethodToken(_ILExecThreadProcess(thread), method,
										pc, (ILType **)0);
			if(methodInfo &&
			   ILMemberAccessible((ILMember *)methodInfo,
								  ILMethod_Owner(method)))
			{
				ILCoderPushToken(coder, (ILProgramItem *)methodInfo);
				stack[stackSize].engineType = ILEngineType_MV;
				stack[stackSize].typeInfo =
						GetSystemValueType(method, "RuntimeMethodHandle");
				++stackSize;
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else if((fieldInfo = ILProgramItemToField(item)) != 0)
		{
			fieldInfo = GetFieldToken(_ILExecThreadProcess(thread), method, pc);
			if(fieldInfo)
			{
				ILCoderPushToken(coder, (ILProgramItem *)fieldInfo);
				stack[stackSize].engineType = ILEngineType_MV;
				stack[stackSize].typeInfo =
						GetSystemValueType(method, "RuntimeFieldHandle");
				++stackSize;
			}
			else
			{
				VERIFY_TYPE_ERROR();
			}
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_ARGLIST:
{
	/* Get a pointer to the variable argument list */
	if((ILType_CallConv(ILMethod_Signature(method))
				& IL_META_CALLCONV_VARARG) != 0)
	{
		ILCoderArgList(coder);
		stack[stackSize].engineType = ILEngineType_MV;
		stack[stackSize].typeInfo =
				GetSystemValueType(method, "RuntimeArgumentHandle");
		++stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_INITOBJ:
{
	/* Initialize a value type */
	classInfo = GetClassToken(method, pc);
	if(classInfo)
	{
		if (ILClassIsValueType(classInfo) &&
		    IsCPPointer(STK_UNARY, stack[stackSize - 1].typeInfo, classInfo))
		{
			ILCoderInitObject(coder, STK_UNARY, classInfo);
			--stackSize;
		}
		else if (STK_UNARY == ILEngineType_M || STK_UNARY == ILEngineType_T)
		{
			/* Let's do a ldnull followed by stind.ref */
			ILCoderConstant(coder, IL_OP_LDNULL, pc + 1);
			ILCoderPtrAccess(coder, IL_OP_STIND_REF, &prefixInfo);
			--stackSize;
		}
		else
		{
			VERIFY_TYPE_ERROR();
		}
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

#define	IsBlkPointer(type)	\
			((type) == ILEngineType_I || \
			 (type) == ILEngineType_I4 || \
			 (type) == ILEngineType_M || \
			 (type) == ILEngineType_T)

case IL_OP_PREFIX + IL_PREFIX_OP_CPBLK:
{
	/* Copy a memory block */
	if(unsafeAllowed &&
	   IsBlkPointer(STK_TERNARY_1) &&
	   IsBlkPointer(STK_TERNARY_2) &&
	   STK_TERNARY_3 == ILEngineType_I4)
	{
		ILCoderCopyBlock(coder, STK_TERNARY_1, STK_TERNARY_2, &prefixInfo);
		stackSize -= 3;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_CPBLK_PREFIX, VALID_NO_NONE);
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_INITBLK:
{
	/* Initialize a memory block to a value */
	if(unsafeAllowed &&
	   IsBlkPointer(STK_TERNARY_1) &&
	   STK_TERNARY_2 == ILEngineType_I4 &&
	   STK_TERNARY_3 == ILEngineType_I4)
	{
		ILCoderInitBlock(coder, STK_TERNARY_1, &prefixInfo);
		stackSize -= 3;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
	CLEAR_VALID_PREFIXES(prefixInfo, VALID_INITBLK_PREFIX, VALID_NO_NONE);
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_SIZEOF:
{
	/* Compute the size of a value type */
	classInfo = GetValueTypeToken(method, pc);
	if(classInfo)
	{
		ILCoderSizeOf(coder, ILType_FromValueType(classInfo));
		stack[stackSize].engineType = ILEngineType_I4;
		stack[stackSize].typeInfo = 0;
		++stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

#endif /* IL_VERIFY_CODE */
