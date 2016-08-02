/*
 * jitc_call.c - Coder implementation for JIT method calls.
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

#ifdef	IL_JITC_DECLARATIONS

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Get the call signature from the methodInfo or the ILCoderMethoInfo.
 * If the value returned is != 0 then the signature must be destroyed after
 * the call is done.
 */
static ILInt32 _ILJitGetCallSignature(ILJITCoder *coder,
									  ILMethod *method,
									  ILCoderMethodInfo *info,
									  ILJitType *signature)
{
	if(method)
	{
		ILJitFunction func = ILJitFunctionFromILMethod(method);

		if(!func)
		{
			*signature = _ILJitCreateMethodSignature(coder, method, 0, 0);
			return 1;
		}
		else
		{
			*signature = jit_function_get_signature(func);
			return 0;
		}
	}
	else
	{
		*signature = _ILJitCreateMethodSignature(coder,
												 method,
												 info->signature,
												 0);
		return 1;
	}
	return 0;
}

/*
 * Fill the argument array for the methodcall with the args on the stack.
 * This function pops the arguments off the stack too.
 * The signature is filled with the call signature.
 * If the value returned is != 0 then the signature must be destroyed after
 * the call is done.
 */
static ILInt32 _ILJitFillArguments(ILJITCoder *coder,
								   ILMethod *method,
								   ILCoderMethodInfo *info,
								   ILJitValue *args,
								   ILInt32 startParam,
								   ILJitType *signature)
{
	int argCount = _ILJitStackNumArgs(info);
	ILJitStackItem *stackItems = _ILJitStackItemGetAndPop(coder, argCount);
	ILInt32 returnValue = _ILJitGetCallSignature(coder,
												 method,
												 info,
												 signature);
	int current = 0;
	int numJitParams = jit_type_num_params(*signature);
	ILJitType paramType;
	ILJitValue value;

	if(numJitParams != (startParam + argCount))
	{
		printf("Argument count mismatch!\n");
	}	
	for(current = 0; current < argCount; current++)
	{
		_ILJitStackHandleCallByRefArg(coder, stackItems[current]);
		paramType = jit_type_get_param(*signature, startParam + current);
		value = _ILJitValueConvertImplicit(coder->jitFunction,
										   _ILJitStackItemValue(stackItems[current]),
										   paramType);
		args[current] = value;
	}
	return returnValue;
}

/*
 * Get the vtable pointer for an interface function from an object.
 */
static ILJitValue _ILJitGetInterfaceFunction(ILJITCoder *jitCoder,
											 ILJitStackItem *object,
											 ILClass *interface,
											 int index)
{
	ILJitValue classPrivate;
	ILJitValue interfaceClass;
	ILJitValue methodIndex;
	ILJitValue args[3];
	ILJitValue jitFunction;
	jit_label_t label = jit_label_undefined;

	_ILJitStackItemCheckNull(jitCoder, *object);
	classPrivate = _ILJitGetObjectClassPrivate(jitCoder->jitFunction,
											   _ILJitStackItemValue(*object));
	interfaceClass = jit_value_create_nint_constant(jitCoder->jitFunction,
													_IL_JIT_TYPE_VPTR,
													(jit_nint)interface);
	methodIndex = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_UINT32,
												 (jit_nint)index);
	args[0] = classPrivate;
	args[1] = interfaceClass;
	args[2] = methodIndex;
	jitFunction = jit_insn_call_native(jitCoder->jitFunction,
									   "_ILRuntimeLookupInterfaceMethod",
									   _ILRuntimeLookupInterfaceMethod,
									   _ILJitSignature_ILRuntimeLookupInterfaceMethod,
									   args, 3, 0);

	jit_insn_branch_if(jitCoder->jitFunction, jitFunction, &label);
	_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_MISSING_METHOD);
	jit_insn_label(jitCoder->jitFunction, &label);

	return jitFunction;
}

/*
 * Get the vtable pointer for a virtual function from an object.
 */
static ILJitValue _ILJitGetVirtualFunction(ILJITCoder *jitCoder,
										   ILJitStackItem *object,
										   int index)
{
	ILJitValue classPrivate;
	ILJitValue vtable;
	ILJitValue vtableIndex;
	ILJitValue jitFunction;
	jit_label_t label = jit_label_undefined;

	_ILJitStackItemCheckNull(jitCoder, *object);
	classPrivate = _ILJitGetObjectClassPrivate(jitCoder->jitFunction,
											   _ILJitStackItemValue(*object));
	vtable = jit_insn_load_relative(jitCoder->jitFunction, classPrivate, 
									offsetof(ILClassPrivate, jitVtable), 
									_IL_JIT_TYPE_VPTR);
	vtableIndex = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_INT32,
												 (jit_nint)index);
	jitFunction = jit_insn_load_elem(jitCoder->jitFunction,
							  vtable, vtableIndex, _IL_JIT_TYPE_VPTR);

	jit_insn_branch_if(jitCoder->jitFunction, jitFunction, &label);
	_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_MISSING_METHOD);
	jit_insn_label(jitCoder->jitFunction, &label);

	return jitFunction;
}

/*
 * Create a new object and push it on the stack.
 */
static void _ILJitNewObj(ILJITCoder *coder, ILClass *info, ILJitValue *newArg)
{
	*newArg = _ILJitAllocObjectGen(coder->jitFunction, info);
}

/*
 * Pack a set of arguments into a params "Object[]" array and replaces
 * them with the new array on the stack.
 * This is not included in VarArgs as this is needed by non-vararg operations 
 * like BeginInvoke.
 */
ILInt32 _ILJitPackVarArgs(ILJITCoder *jitCoder,
						  ILUInt32 firstParam,
						  ILUInt32 numArgs,
						  ILType *callSiteSig)
{
	ILExecThread *_thread = ILExecThreadCurrent();
	ILUInt32 param;
	ILType *paramType;
	ILType *enumType;
	ILClass *info;
	ILUInt32 typeSize;
	ILJitValue thread = _ILJitCoderGetThread(jitCoder);
	ILJitValue array;
	ILInt32 arrayBase;
	ILJitValue boxObject;
	ILJitValue boxValue;
	ILJitValue boxObjectSize;
	ILJitValue ptr;
	ILJitStackItem *stackItems;


	/* Allocate an array to hold all of the arguments */
	array = _ILJitSObjectArrayCreate(jitCoder->jitFunction, _thread, thread, numArgs);
	if(!array)
	{
		return 0;
	}

	arrayBase = _IL_JIT_SARRAY_HEADERSIZE;

	/* Adjust the stack just to the first vararg. */
	stackItems = _ILJitStackItemGetAndPop(jitCoder, numArgs);

	/* Convert the arguments into objects in the array */
	for(param = 0; param < numArgs; ++param)
	{
		paramType = ILTypeGetParam(callSiteSig, firstParam + param);
		boxValue = 0;
		ptr = 0;

		if (ILType_IsComplex(paramType)
			&& ILType_Kind(paramType) == IL_TYPE_COMPLEX_BYREF)
		{
			paramType = ILType_Ref(paramType);

			enumType = ILTypeGetEnumType(paramType);

			if(ILType_IsPrimitive(enumType))
			{
				ILJitType jitType = _ILJitGetReturnType(enumType,
														jitCoder->process);

				if(!jitType)
				{
					return 0;
				}

				boxValue = jit_insn_load_relative(jitCoder->jitFunction,
												  _ILJitStackItemValue(stackItems[param]),
												  0, jitType);
			}
			else if(ILType_IsValueType(paramType))
			{
				ptr = _ILJitStackItemValue(stackItems[param]);
			}
			else if(ILTypeIsReference(paramType))
			{
				/* Ref to an object reference type: pass the object reference */
				boxValue = jit_insn_load_relative(jitCoder->jitFunction,
												  _ILJitStackItemValue(stackItems[param]),
												  0, _IL_JIT_TYPE_VPTR);
				jit_insn_store_relative(jitCoder->jitFunction,
										array,
										arrayBase,
										boxValue);
				boxValue = 0;
			}
			else
			{
				/* Assume that everything else is a pointer, and wrap
				it up within a "System.IntPtr" object */
				boxValue = jit_insn_load_relative(jitCoder->jitFunction,
												  _ILJitStackItemValue(stackItems[param]),
												  0,
												  _IL_JIT_TYPE_VPTR);
				paramType = ILType_Int;
			}
		}
		else
		{
			enumType = ILTypeGetEnumType(paramType);

			if(ILType_IsPrimitive(enumType))
			{
				ILJitType jitType = _ILJitGetReturnType(enumType,
														jitCoder->process);

				if(!jitType)
				{
					return 0;
				}

				boxValue = _ILJitValueConvertImplicit(jitCoder->jitFunction,
													  _ILJitStackItemValue(stackItems[param]),
													  jitType);
			}
			else if(ILType_IsValueType(paramType))
			{
				ptr = jit_insn_address_of(jitCoder->jitFunction,
										  _ILJitStackItemValue(stackItems[param]));
			}
			else if(ILTypeIsReference(paramType))
			{
				/* Object reference type: pass it directly */
				jit_insn_store_relative(jitCoder->jitFunction,
										array,
										arrayBase,
										_ILJitStackItemValue(stackItems[param]));
			}
			else
			{
				/* Assume that everything else is a pointer, and wrap
				it up within a "System.IntPtr" object */
				boxValue = _ILJitValueConvertImplicit(jitCoder->jitFunction, 
													  _ILJitStackItemValue(stackItems[param]),
													  _IL_JIT_TYPE_VPTR);
				paramType = ILType_Int;
			}
		}
		if(boxValue || ptr)
		{
			/* We have to box the argument. */
			info = ILClassFromType
							(ILContextNextImage(jitCoder->process->context, 0),
			 				0, paramType, 0);
			info = ILClassResolve(info);
			typeSize = _ILSizeOfTypeLocked(jitCoder->process, paramType);

			boxObject = _ILJitAllocObjectGen(jitCoder->jitFunction, info);
			if(boxValue)
			{
				jit_insn_store_relative(jitCoder->jitFunction,
										boxObject,
										0,
										boxValue);
			}
			else
			{
				boxObjectSize =
					jit_value_create_nint_constant(jitCoder->jitFunction,
												   _IL_JIT_TYPE_UINT32,
												   typeSize);

				jit_insn_memcpy(jitCoder->jitFunction,
								boxObject,
								ptr,
								boxObjectSize);
			}
			jit_insn_store_relative(jitCoder->jitFunction,
									array,
									arrayBase,
									boxObject);
		}
		/* Advance to the next slot in the array. */
		arrayBase += sizeof(ILObject *);
	}

	/* push the array on the stack */
	_ILJitStackPushNotNullValue(jitCoder, array);

	return 1;
}

#endif	/* IL_JITC_FUNCTIONS */

#ifdef IL_JITC_CODE

static void JITCoder_UpConvertArg(ILCoder *coder, ILEngineStackItem *args,
						          ILUInt32 numArgs, ILUInt32 param,
						          ILType *paramType)
{
}

static void JITCoder_DownConvertArg(ILCoder *coder, ILEngineStackItem *args,
						            ILUInt32 numArgs, ILUInt32 param,
						            ILType *paramType)
{
}

static void JITCoder_PackVarArgs(ILCoder *coder, ILType *callSiteSig,
					             ILUInt32 firstParam, ILEngineStackItem *args,
						         ILUInt32 numArgs)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"PackVarArgs: firstParam: %i, numArgs %i\n", 
			firstParam,
			numArgs);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	_ILJitPackVarArgs(jitCoder, firstParam, numArgs, callSiteSig);
}

static void JITCoder_ValueCtorArgs(ILCoder *coder, ILClass *classInfo,
								   ILEngineStackItem *args, ILUInt32 numArgs)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitStackItem *stackItems;
	ILJitType valueType = _ILJitGetTypeFromClass(classInfo);
	ILJitValue value = jit_value_create(jitCoder->jitFunction, valueType);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"ValueCtorArgs: %s %i\n", 
			ILClass_Name(classInfo),
			numArgs);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	stackItems = _ILJitStackMakeFreeSlots(jitCoder, 2, numArgs);
	_ILJitStackItemInitWithValue(stackItems[0], value);
	value = jit_insn_address_of(jitCoder->jitFunction, value);
	_ILJitStackItemInitWithValue(stackItems[1], value);
}

static void JITCoder_CheckCallNull(ILCoder *coder, ILCoderMethodInfo *info)
{
	/* We don't check the this pointer for non virtual instance method */
	/* calls because it degrades the the performance on these calls. */
	/* The check for null in this case is not in the ECMA specs. */
}

static void JITCoder_CallMethod(ILCoder *coder, ILCoderMethodInfo *info,
								ILEngineStackItem *returnItem,
								ILMethod *methodInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(methodInfo);
	int argCount = _ILJitStackNumArgs(info);
	ILJitValue jitParams[argCount + 2];
	ILJitValue returnValue;
	ILJitType callSignature = 0;
	int destroyCallSignature = 0;
	char *methodName = 0;
	ILInternalInfo fnInfo;
	ILJitInlineFunc inlineFunc = 0;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallMethod: %s.%s\n", 
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo));
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	
	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return;
		}
		jitFunction = ILJitFunctionFromILMethod(methodInfo);
		if(!jitFunction)
		{
			/* This can be a generic method instance. */
			if(!ILJitFunctionCreate(ILExecThreadCurrent()->process->coder, methodInfo))
			{
				return;
			}
			jitFunction = ILJitFunctionFromILMethod(methodInfo);
		}
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILJitType  signature  = jit_function_get_signature(jitFunction);

		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallInfos: StackTop: %i, ArgCount: %i, Signature argCount: %i\n",
			jitCoder->stackTop,
			argCount,
			jit_type_num_params(signature));
		ILMutexUnlock(globalTraceMutex);
	}
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	methodName = _ILJitFunctionGetMethodName(jitFunction);
#endif

	/* Check if the function can be inlined. */
	if((inlineFunc = ((ILJitMethodInfo*)(methodInfo->userData))->inlineFunc))
	{
		ILJitStackItem *args = _ILJitStackItemGetAndPop(jitCoder, argCount);

		if(!((*inlineFunc)(jitCoder, methodInfo, info, args, argCount)))
		{
			/* Failure on inlining the function. */
			/* TODO: we have to handle this somehow. */
		}
		return;
	}

	/* Check if the function is implemented in the engine. */
	if(_ILJitFunctionIsInternal(jitCoder, methodInfo, &fnInfo, 0))
	{
		ILJitValue thread = _ILJitCoderGetThread(jitCoder);

		/* Call the engine function directly with the supplied args. */
		/* Queue the cctor to run. */
		ILCCtorMgr_OnCallMethod(&(jitCoder->cctorMgr), methodInfo);
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		destroyCallSignature = _ILJitFillArguments(jitCoder,
												   methodInfo,
												   info,
												   jitParams,
												   1,
												   &callSignature);
	#else
		destroyCallSignature = _ILJitFillArguments(jitCoder,
												   methodInfo,
												   info,
												   jitParams,
												   0,
												   &callSignature);
	#endif
		returnValue = _ILJitCallInternal(jitCoder, thread,
										 methodInfo,
										 fnInfo.un.func, methodName,
										 jitParams,
										 argCount);
		if(returnItem && returnItem->engineType != ILEngineType_Invalid)
		{
			_ILJitStackPushValue(jitCoder, returnValue);
		}
		if(destroyCallSignature && callSignature)
		{
			jit_type_free(callSignature);
		}
		return;
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	/* Set the ILExecThread argument. */
	jitParams[0] = _ILJitCoderGetThread(jitCoder);

	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   methodInfo,
											   info,
											   &(jitParams[1]),
											   1,
											   &callSignature);
#else
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   methodInfo,
											   info,
											   &(jitParams[0]),
											   0,
											   &callSignature);
#endif

	if(info->tailCall == 1)
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call(jitCoder->jitFunction, methodName,
									jitFunction, 0,
									jitParams, argCount + 1, JIT_CALL_TAIL);
	#else
		returnValue = jit_insn_call(jitCoder->jitFunction, methodName,
									jitFunction, 0,
									jitParams, argCount, JIT_CALL_TAIL);
	#endif
		if(returnItem->engineType != ILEngineType_Invalid)
		{
			_ILJitStackPushValue(jitCoder, returnValue);
		}
	}
	else
	{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(_IL_JIT_ENABLE_DEBUG)
		ILPInvoke *pinv = ILPInvokeFind(methodInfo);
		if(pinv && ((ILJitMethodInfo*)(methodInfo->userData))->fnInfo.un.func)
		{
			returnValue = _ILJitInlinePinvoke(jitCoder, methodInfo, jitParams);
		}
		else
#endif
		{
		#ifdef _IL_JIT_ENABLE_INLINE
			if(_ILJitMethodIsInlineable(jitCoder, methodInfo))
			{
				/* Get the pointer to the args on the stack. */
				ILJitStackItem *args = _ILJitStackItemGetTop(jitCoder, -1);

			#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
				if (jitCoder->flags & IL_CODER_FLAG_STATS)
				{
					ILMutexLock(globalTraceMutex);
					fprintf(stdout,
							"Inline Method: %s.%s\n", 
							ILClass_Name(ILMethod_Owner(methodInfo)),
							ILMethod_Name(methodInfo));
					ILMutexUnlock(globalTraceMutex);
				}
			#endif
				if(!_ILJitCoderInlineMethod(jitCoder,
											methodInfo,
											0,
											args,
											argCount))
				{
					printf("Inlining failed!\n");
				}
				if(destroyCallSignature && callSignature)
				{
					jit_type_free(callSignature);
				}
				return;
			}
		#endif /* _IL_JIT_ENABLE_INLINE */
		#ifdef IL_JIT_THREAD_IN_SIGNATURE
			returnValue = jit_insn_call(jitCoder->jitFunction, methodName,
										jitFunction, 0,
										jitParams, argCount + 1, 0);
		#else
			returnValue = jit_insn_call(jitCoder->jitFunction, methodName,
										jitFunction, 0,
										jitParams, argCount, 0);
		#endif
		}
		if(returnItem && returnItem->engineType != ILEngineType_Invalid)
		{
			_ILJitStackPushValue(jitCoder, returnValue);
		}
	}
	if(destroyCallSignature && callSignature)
	{
		jit_type_free(callSignature);
	}
}

static void JITCoder_CallIndirect(ILCoder *coder, ILCoderMethodInfo *info,
								  ILEngineStackItem *returnItem)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	int argCount = _ILJitStackNumArgs(info);
	ILJitValue ftnPtr;
	ILJitType callSignature = 0;
	int destroyCallSignature = 0;
	ILJitValue jitParams[argCount + 1];
	ILJitValue returnValue;
	_ILJitStackItemNew(stackItem);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallIndirect\n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPop(jitCoder, stackItem);
#ifdef IL_JIT_FNPTR_ILMETHOD
	/* We need to convert the method pointer to the vtable pointer first. */
	ftnPtr = jit_insn_call_native(jitCoder->jitFunction,
								  "ILRuntimeMethodToVtablePointer",
								  ILRuntimeMethodToVtablePointer,
								  _ILJitSignature_ILRuntimeMethodToVtablePointer,
								  &(_ILJitStackItemValue(stackItem)), 1, 0);
#else
	/* The function pointer on the stack is the vtable pointer. */
	ftnPtr = _ILJitStackItemValue(stackItem);
#endif

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	jitParams[0] = _ILJitCoderGetThread(jitCoder);
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   0,
											   info,
											   &(jitParams[1]),
											   1,
											   &callSignature);

#else
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   0,
											   info,
											   &(jitParams[0]),
											   0,
											   &callSignature);
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallInfos: StackTop: %i, ArgCount: %i, Signature argCount: %i\n",
			jitCoder->stackTop,
			argCount,
			jit_type_num_params(callSignature));
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	if(info->tailCall == 1)
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													ftnPtr,
													callSignature,
													jitParams,
													argCount + 1,
													JIT_CALL_TAIL);
	#else
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													ftnPtr,
													callSignature,
													jitParams,
													argCount,
													JIT_CALL_TAIL);
	#endif
	}
	else
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													ftnPtr,
													callSignature,
													jitParams,
													argCount + 1,
													0);
	#else
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													ftnPtr,
													callSignature,
													jitParams,
													argCount,
													0);
	#endif
	}
	if(returnItem->engineType != ILEngineType_Invalid)
	{
		_ILJitStackPushValue(jitCoder, returnValue);
	}
	if(destroyCallSignature && callSignature)
	{
		jit_type_free(callSignature);
	}
}

static void JITCoder_CallCtor(ILCoder *coder, ILCoderMethodInfo *info,
					   		  ILMethod *methodInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILClass *classInfo;
	ILType *type;
	ILType *synType;
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(methodInfo);
	int argCount = _ILJitStackNumArgs(info);
	ILJitType callSignature = 0;
	int destroyCallSignature = 0;
	ILJitValue jitParams[argCount + 2];
	ILJitValue returnValue;
	ILInternalInfo fnInfo;
	int internalType = _IL_JIT_IMPL_DEFAULT;
	char *methodName = 0;
	
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallCtor: %s.%s\n", 
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo));
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return;
		}
		jitFunction = ILJitFunctionFromILMethod(methodInfo);
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#if defined(_IL_JIT_ENABLE_DEBUG)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILJitType  signature  = jit_function_get_signature(jitFunction);

		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallInfos: StackTop: %i, ArgCount: %i, Signature argCount: %i\n",
			jitCoder->stackTop,
			argCount,
			jit_type_num_params(signature));
		ILMutexUnlock(globalTraceMutex);
	}
	methodName = _ILJitFunctionGetMethodName(jitFunction);
#endif
#endif
	classInfo = ILMethod_Owner(methodInfo);
	type = ILType_FromClass(classInfo);
	synType = ILClassGetSynType(classInfo);

	/* Check if the function is implemented in the engine. */
	if((internalType = _ILJitFunctionIsInternal(jitCoder, methodInfo, &fnInfo, 1)))
	{
		ILJitValue thread = _ILJitCoderGetThread(jitCoder);

		/* Queue the cctor to run. */
		ILCCtorMgr_OnCallMethod(&(jitCoder->cctorMgr), methodInfo);
	
		/* Call the engine function directly with the supplied args. */
		if(internalType == _IL_JIT_IMPL_INTERNALALLOC)
		{
			/* This is an allocating constructor. */
		#ifdef IL_JIT_THREAD_IN_SIGNATURE
			destroyCallSignature = _ILJitFillArguments(jitCoder,
													   methodInfo,
													   info,
													   jitParams,
													   1,
													   &callSignature);
		#else
			destroyCallSignature = _ILJitFillArguments(jitCoder,
													   methodInfo,
													   info,
													   jitParams,
													   0,
													   &callSignature);
		#endif
			returnValue = _ILJitCallInternal(jitCoder, thread,
											 methodInfo,
											 fnInfo.un.func, methodName,
											 jitParams,
											 argCount);
			_ILJitStackPushNotNullValue(jitCoder, returnValue);
		}
		else
		{
			/* create a newobj and add it to the jitParams[0]. */
			_ILJitNewObj(jitCoder, ILMethod_Owner(methodInfo), &jitParams[0]); 
			destroyCallSignature = _ILJitFillArguments(jitCoder,
													   methodInfo,
													   info,
													   &(jitParams[1]),
													   1,
													   &callSignature);

			returnValue = _ILJitCallInternal(jitCoder, thread,
											 methodInfo,
											 fnInfo.un.func, methodName,
											 jitParams, argCount + 1);
			_ILJitStackPushNotNullValue(jitCoder, jitParams[0]);
		}
		if(destroyCallSignature && callSignature)
		{
			jit_type_free(callSignature);
		}
		return;
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	/* Output a call to the constructor */
	jitParams[0] = _ILJitCoderGetThread(jitCoder); // we add the current function thread as the first param
#endif
	if((synType && ILType_IsArray(synType)) || ILTypeIsStringClass(type))
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		destroyCallSignature = _ILJitFillArguments(jitCoder,
												   methodInfo,
												   info,
												   &(jitParams[1]),
												   1,
												   &callSignature);
		// call the constructor with jitParams as input
		returnValue = jit_insn_call(jitCoder->jitFunction, 0, jitFunction, 0,
									jitParams, argCount + 1, 0);
	#else
		destroyCallSignature = _ILJitFillArguments(jitCoder,
												   methodInfo,
												   info,
												   &(jitParams[0]),
												   0,
												   &callSignature);
		// call the constructor with jitParams as input
		returnValue = jit_insn_call(jitCoder->jitFunction, 0, jitFunction, 0,
									jitParams, argCount, 0);
	#endif
		_ILJitStackPushNotNullValue(jitCoder, returnValue);
	}
	else
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		/* create a newobj and add it to the jitParams[1]. */
		_ILJitNewObj(jitCoder, ILMethod_Owner(methodInfo), &jitParams[1]);
		destroyCallSignature = _ILJitFillArguments(jitCoder,
												   methodInfo,
												   info,
												   &(jitParams[2]),
												   2,
												   &callSignature);

		// call the constructor with jitParams as input
		returnValue = jit_insn_call(jitCoder->jitFunction, methodName,
									jitFunction, 0,
									jitParams, argCount + 2, 0);

		_ILJitStackPushNotNullValue(jitCoder, jitParams[1]);
	#else
		/* create a newobj and add it to the jitParams[0]. */
		_ILJitNewObj(jitCoder, ILMethod_Owner(methodInfo), &jitParams[0]);
		destroyCallSignature = _ILJitFillArguments(jitCoder,
												   methodInfo,
												   info,
												   &(jitParams[1]),
												   1,
												   &callSignature);

		// call the constructor with jitParams as input
		returnValue = jit_insn_call(jitCoder->jitFunction, methodName,
									jitFunction, 0,
									jitParams, argCount + 1, 0);

		_ILJitStackPushNotNullValue(jitCoder, jitParams[0]);
	#endif
	}	
	if(destroyCallSignature && callSignature)
	{
		jit_type_free(callSignature);
	}
}

static void JITCoder_CallVirtual(ILCoder *coder, ILCoderMethodInfo *info,
								 ILEngineStackItem *returnItem,
								 ILMethod *methodInfo)
{

	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	int argCount = _ILJitStackNumArgs(info);
	ILJitFunction func = ILJitFunctionFromILMethod(methodInfo);
	ILJitType  signature;
	int destroyCallSignature = 0;
	ILJitValue jitParams[argCount + 1];
	ILJitValue returnValue;
	ILJitValue jitFunction;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallVirtual: %s.%s at slot %i\n", 
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo),
			methodInfo->index);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	if(!func)
	{
		/* We might need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return;
		}
		func = ILJitFunctionFromILMethod(methodInfo);
		if(!func)
		{
			/* This can be a generic method instance. */
			if(!ILJitFunctionCreate(ILExecThreadCurrent()->process->coder, methodInfo))
			{
				return;
			}
			func = ILJitFunctionFromILMethod(methodInfo);
		}
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	jitParams[0] = _ILJitCoderGetThread(jitCoder);
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   methodInfo,
											   info,
											   &(jitParams[1]),
											   1,
											   &signature);

	jitFunction = _ILJitGetVirtualFunction(jitCoder,
										   _ILJitStackItemGetTop(jitCoder, -1),
										   methodInfo->index);
#else
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   methodInfo,
											   info,
											   &(jitParams[0]),
											   0,
											   &signature);

	jitFunction = _ILJitGetVirtualFunction(jitCoder,
										   _ILJitStackItemGetTop(jitCoder, -1),
										   methodInfo->index);
#endif
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallInfos: StackTop: %i, ArgCount: %i, Signature argCount: %i\n",
			jitCoder->stackTop,
			argCount,
			jit_type_num_params(signature));
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	if(info->tailCall == 1)
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount + 1,
													JIT_CALL_TAIL);
	#else
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount,
													JIT_CALL_TAIL);
	#endif
	}
	else
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount + 1,
													0);
	#else
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount,
													0);
	#endif
	}
	if(returnItem->engineType != ILEngineType_Invalid)
	{
		_ILJitStackPushValue(jitCoder, returnValue);
	}
	if(destroyCallSignature && signature)
	{
		jit_type_free(signature);
	}
}

static void JITCoder_CallInterface(ILCoder *coder, ILCoderMethodInfo *info,
								   ILEngineStackItem *returnItem,
								   ILMethod *methodInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	int argCount = _ILJitStackNumArgs(info);
	ILJitType  signature = 0;
	int destroyCallSignature = 0;
	ILJitValue jitParams[argCount + 1];
	ILJitValue returnValue;
	ILJitValue jitFunction;
	jit_label_t label = jit_label_undefined;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallInterface: %s.%s at slot %i\n", 
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo),
			methodInfo->index);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	jitParams[0] = _ILJitCoderGetThread(jitCoder);
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   methodInfo,
											   info,
											   &(jitParams[1]),
											   1,
											   &signature);

	jitFunction = _ILJitGetInterfaceFunction(jitCoder,
											 _ILJitStackItemGetTop(jitCoder, -1),
											 methodInfo->member.owner,
											 methodInfo->index);
#else
	destroyCallSignature = _ILJitFillArguments(jitCoder,
											   methodInfo,
											   info,
											   &(jitParams[0]),
											   0,
											   &signature);

	jitFunction = _ILJitGetInterfaceFunction(jitCoder,
											 _ILJitStackItemGetTop(jitCoder, -1),
											 methodInfo->member.owner,
											 methodInfo->index);
#endif
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallInfos: StackTop: %i, ArgCount: %i, Signature argCount: %i\n",
			jitCoder->stackTop,
			argCount,
			jit_type_num_params(signature));
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	jit_insn_branch_if(jitCoder->jitFunction, jitFunction, &label);
	/* TODO: raise a MissingMethodException here. */

	jit_insn_label(jitCoder->jitFunction, &label);
	if(info->tailCall == 1)
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount + 1,
													JIT_CALL_TAIL);
	#else
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount,
													JIT_CALL_TAIL);
	#endif
	}
	else
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount + 1,
													0);
	#else
		returnValue = jit_insn_call_indirect_vtable(jitCoder->jitFunction,
													jitFunction, signature,
													jitParams, argCount,
													0);
	#endif
	}
	if(returnItem->engineType != ILEngineType_Invalid)
	{
		_ILJitStackPushValue(jitCoder, returnValue);
	}
	if(destroyCallSignature && signature)
	{
		jit_type_free(signature);
	}
}

static int JITCoder_CallInlineable(ILCoder *coder, int inlineType,
								   ILMethod *methodInfo, ILInt32 elementSize)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	/* Determine what to do for the inlineable method type */
	switch(inlineType)
	{
		case IL_INLINEMETHOD_MONITOR_ENTER:
		{
			/* Enter the monitor on the top-most object */
			_ILJitStackItemNew(stackItem);
			ILJitValue args[2];

			_ILJitStackPop(jitCoder, stackItem);
			args[0] = _ILJitCoderGetThread(jitCoder);
			args[1] = _ILJitStackItemValue(stackItem);

			_ILJitBeginNativeCall(jitCoder->jitFunction, args[0]);
			jit_insn_call_native(jitCoder->jitFunction,
								 "_IL_Monitor_Enter",
								 _IL_Monitor_Enter,
								 _ILJitSignature_ILMonitorEnter,
								 args, 2, 0);
			_ILJitEndNativeCall(jitCoder->jitFunction, args[0]);

			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MONITOR_EXIT:
		{
			/* Exit the monitor on the top-most object */
			_ILJitStackItemNew(stackItem);
			ILJitValue args[2];

			_ILJitStackPop(jitCoder, stackItem);
			args[0] = _ILJitCoderGetThread(jitCoder);
			args[1] = _ILJitStackItemValue(stackItem);

			_ILJitBeginNativeCall(jitCoder->jitFunction, args[0]);
			jit_insn_call_native(jitCoder->jitFunction,
								 "_IL_Monitor_Exit",
								 _IL_Monitor_Exit,
								 _ILJitSignature_ILMonitorExit,
								 args, 2, 0);
			_ILJitEndNativeCall(jitCoder->jitFunction, args[0]);

			return 1;
		}
		/* Not reached */
		case IL_INLINEMETHOD_TYPE_FROM_HANDLE:
		{
			/* Convert a RuntimeTypeHandle into a Type object */
			_ILJitStackItemNew(stackItem);
			ILJitValue returnValue = jit_value_create(jitCoder->jitFunction,
													  _IL_JIT_TYPE_VPTR);
			ILJitValue temp;
			ILJitValue args[2];
			jit_label_t label = jit_label_undefined;
;
			_ILJitStackPop(jitCoder, stackItem);
			jit_insn_store(jitCoder->jitFunction,
						   returnValue, 
						   _ILJitStackItemValue(stackItem));

			jit_insn_branch_if_not(jitCoder->jitFunction, returnValue, &label);

			args[0] = _ILJitCoderGetThread(jitCoder);
			args[1] = returnValue;
			_ILJitBeginNativeCall(jitCoder->jitFunction, args[0]);
			temp = jit_insn_call_native(jitCoder->jitFunction,
										"_ILGetClrType",
										_ILGetClrType,
										_ILJitSignature_ILGetClrType,
										args, 2, 0);
			_ILJitEndNativeCall(jitCoder->jitFunction, args[0]);
			jit_insn_store(jitCoder->jitFunction,
						   returnValue, 
						   temp);
			jit_insn_label(jitCoder->jitFunction, &label);
			_ILJitStackPushValue(jitCoder, returnValue);
			return 1;
		}
		/* Not reached */

		/*
		 * Cases for Math class inlines.
		 */
		case IL_INLINEMETHOD_ABS_I4:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_INT32);
			value = jit_insn_abs(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ABS_R4:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_SINGLE);
			value = jit_insn_abs(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ABS_R8:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_abs(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ASIN:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_asin(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ATAN:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_atan(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ATAN2:
		{
			return 0;
		}
		/* Not reached */

		case IL_INLINEMETHOD_CEILING:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_ceil(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_COS:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_cos(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_COSH:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_cosh(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_EXP:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_exp(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_FLOOR:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_floor(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_IEEEREMAINDER:
		{
			return 0;
		}
		/* Not reached */

		case IL_INLINEMETHOD_LOG:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_log(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_LOG10:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_log10(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MAX_I4:
		{
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem1);
			ILJitValue value2;
			ILJitValue value1;
			ILJitValue result;

			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem1);
			value1 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem1),
											   _IL_JIT_TYPE_INT32);
			value2 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem2),
											   _IL_JIT_TYPE_INT32);
			result = jit_insn_max(jitCoder->jitFunction, value1, value2);
			_ILJitStackPushValue(jitCoder, result);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MIN_I4:
		{
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem1);
			ILJitValue value2;
			ILJitValue value1;
			ILJitValue result;

			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem1);
			value1 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem1),
											   _IL_JIT_TYPE_INT32);
			value2 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem2),
											   _IL_JIT_TYPE_INT32);
			result = jit_insn_min(jitCoder->jitFunction, value1, value2);
			_ILJitStackPushValue(jitCoder, result);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MAX_R4:
		{
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem1);
			ILJitValue value2;
			ILJitValue value1;
			ILJitValue result;

			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem1);
			value1 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem1),
											   _IL_JIT_TYPE_SINGLE);
			value2 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem2),
											   _IL_JIT_TYPE_SINGLE);
			result = jit_insn_max(jitCoder->jitFunction, value1, value2);
			_ILJitStackPushValue(jitCoder, result);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MIN_R4:
		{
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem1);
			ILJitValue value2;
			ILJitValue value1;
			ILJitValue result;

			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem1);
			value1 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem1),
											   _IL_JIT_TYPE_SINGLE);
			value2 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem2),
											   _IL_JIT_TYPE_SINGLE);
			result = jit_insn_min(jitCoder->jitFunction, value1, value2);
			_ILJitStackPushValue(jitCoder, result);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MAX_R8:
		{
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem1);
			ILJitValue value2;
			ILJitValue value1;
			ILJitValue result;

			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem1);
			value1 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem1),
											   _IL_JIT_TYPE_DOUBLE);
			value2 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem2),
											   _IL_JIT_TYPE_DOUBLE);
			result = jit_insn_max(jitCoder->jitFunction, value1, value2);
			_ILJitStackPushValue(jitCoder, result);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MIN_R8:
		{
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem1);
			ILJitValue value2;
			ILJitValue value1;
			ILJitValue result;

			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem1);
			value1 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem1),
											   _IL_JIT_TYPE_DOUBLE);
			value2 = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem2),
											   _IL_JIT_TYPE_DOUBLE);
			result = jit_insn_min(jitCoder->jitFunction, value1, value2);
			_ILJitStackPushValue(jitCoder, result);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_POW:
		case IL_INLINEMETHOD_ROUND:
		{
			return 0;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SIN:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_sin(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SIGN_I4:
		case IL_INLINEMETHOD_SIGN_R4:
		case IL_INLINEMETHOD_SIGN_R8:
		{
			return 0;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SINH:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_sinh(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SQRT:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_sqrt(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_TAN:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_tan(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_TANH:
		{
			_ILJitStackItemNew(stackItem);
			ILJitValue value;

			_ILJitStackPop(jitCoder, stackItem);
			value = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											   _ILJitStackItemValue(stackItem),
											   _IL_JIT_TYPE_DOUBLE);
			value = jit_insn_tanh(jitCoder->jitFunction, value);
			_ILJitStackPushValue(jitCoder, value);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ARRAY_COPY_AAI4:
		{
			_ILJitStackItemNew(stackItem1);
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem3);
			ILJitValue args[5];

			_ILJitStackPop(jitCoder, stackItem1);
			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem3);
			args[0] = _ILJitCoderGetThread(jitCoder);
			args[1] = _ILJitStackItemValue(stackItem3);
			args[2] = _ILJitStackItemValue(stackItem2);
			args[3] = _ILJitStackItemValue(stackItem1);
			args[4] = jit_value_create_nint_constant(jitCoder->jitFunction,
													 _IL_JIT_TYPE_INT32,
													 elementSize);

			_ILJitBeginNativeCall(jitCoder->jitFunction, args[0]);
			jit_insn_call_native(jitCoder->jitFunction,
								 "ILSArrayCopy_AAI4",
								 ILSArrayCopy_AAI4,
								 _ILJitSignature_ILSArrayCopy_AAI4,
								 args, 5, 0);
			_ILJitEndNativeCall(jitCoder->jitFunction, args[0]);

			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ARRAY_COPY_AI4AI4I4:
		{
			_ILJitStackItemNew(stackItem1);
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem3);
			_ILJitStackItemNew(stackItem4);
			_ILJitStackItemNew(stackItem5);
			ILJitValue args[7];

			_ILJitStackPop(jitCoder, stackItem1);
			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem3);
			_ILJitStackPop(jitCoder, stackItem4);
			_ILJitStackPop(jitCoder, stackItem5);
			args[0] = _ILJitCoderGetThread(jitCoder);
			args[1] = _ILJitStackItemValue(stackItem5);
			args[2] = _ILJitStackItemValue(stackItem4);
			args[3] = _ILJitStackItemValue(stackItem3);
			args[4] = _ILJitStackItemValue(stackItem2);
			args[5] = _ILJitStackItemValue(stackItem1);
			args[6] = jit_value_create_nint_constant(jitCoder->jitFunction,
													 _IL_JIT_TYPE_INT32,
													 elementSize);

			_ILJitBeginNativeCall(jitCoder->jitFunction, args[0]);
			jit_insn_call_native(jitCoder->jitFunction,
								 "ILSArrayCopy_AI4AI4I4",
								 ILSArrayCopy_AI4AI4I4,
								 _ILJitSignature_ILSArrayCopy_AI4AI4I4,
								 args, 7, 0);
			_ILJitEndNativeCall(jitCoder->jitFunction, args[0]);

			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ARRAY_CLEAR_AI4I4:
		{
			_ILJitStackItemNew(stackItem1);
			_ILJitStackItemNew(stackItem2);
			_ILJitStackItemNew(stackItem3);
			ILJitValue args[5];

			_ILJitStackPop(jitCoder, stackItem1);
			_ILJitStackPop(jitCoder, stackItem2);
			_ILJitStackPop(jitCoder, stackItem3);

			args[0] = _ILJitCoderGetThread(jitCoder);
			args[1] = _ILJitStackItemValue(stackItem3);
			args[2] = _ILJitStackItemValue(stackItem2);
			args[3] = _ILJitStackItemValue(stackItem1);
			args[4] = jit_value_create_nint_constant(jitCoder->jitFunction,
													 _IL_JIT_TYPE_INT32,
													 elementSize);

			_ILJitBeginNativeCall(jitCoder->jitFunction, args[0]);
			jit_insn_call_native(jitCoder->jitFunction,
								 "ILSArrayClear_AI4I4",
								 ILSArrayClear_AI4I4,
								 _ILJitSignature_ILSArrayClear_AI4I4,
								 args, 5, 0);
			_ILJitEndNativeCall(jitCoder->jitFunction, args[0]);

			return 1;
		}
		/* Not reached */
	}
	/* If we get here, then we don't know how to inline the method */
	return 0;
}

static void JITCoder_JumpMethod(ILCoder *coder, ILMethod *methodInfo)
{
	/* TODO */
}

static void JITCoder_ReturnInsn(ILCoder *coder, ILEngineType engineType,
							    ILType *returnType)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Return\n");
		ILMutexUnlock(globalTraceMutex);
	}

	if (jitCoder->flags & IL_CODER_FLAG_METHOD_TRACE)
	{
		ILMethod *method = (ILMethod *)jit_function_get_meta(jitCoder->jitFunction, IL_JIT_META_METHOD);
		ILJitValue args[2];
		args[0] = _ILJitCoderGetThread(jitCoder);
		args[1] = jit_value_create_nint_constant(jitCoder->jitFunction, _IL_JIT_TYPE_VPTR, (jit_nint) method);
		jit_insn_call_native(jitCoder->jitFunction, "ILJitTraceOut", ILJitTraceOut, _ILJitSignature_ILJitTraceInOut, args, 2, JIT_CALL_NOTHROW);
	}
#endif

#ifdef IL_DEBUGGER
	/* Insert potential breakpoint with method in data2 */
	if(jitCoder->markBreakpoints)
	{
		jit_insn_mark_breakpoint(jitCoder->jitFunction,
								 JIT_DEBUGGER_DATA1_METHOD_LEAVE,
								 (jit_nint) ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr)));
	}
#endif

#ifdef	_IL_JIT_ENABLE_INLINE
	if(jitCoder->currentInlineContext)
	{
		if(jitCoder->currentInlineContext->returnValue)
		{
			/* Pop the return value ans store it in the returnvalue for the */
			/* inlined function. */
			ILJitType returnType = jit_value_get_type(jitCoder->currentInlineContext->returnValue);
			_ILJitStackItemNew(value);

			_ILJitStackPop(jitCoder, value);
			jit_insn_store(jitCoder->jitFunction,
						   jitCoder->currentInlineContext->returnValue,
						   _ILJitValueConvertImplicit(jitCoder->jitFunction,
													  _ILJitStackItemValue(value),
													  returnType));
		}
		/* And jump to the end of the inlined function. */
		jit_insn_branch(jitCoder->jitFunction, 
						&(jitCoder->currentInlineContext->returnLabel));

		return;
	}
#endif	/* _IL_JIT_ENABLE_INLINE */

	if(engineType == ILEngineType_Invalid)
	{
	       jit_insn_return(jitCoder->jitFunction, 0);
	}
	else
	{
		ILJitType signature = jit_function_get_signature(jitCoder->jitFunction);
		ILJitType returnType = jit_type_get_return(signature);
		_ILJitStackItemNew(value);
		ILJitValue returnValue;

		_ILJitStackPop(jitCoder, value);
		returnValue = _ILJitValueConvertImplicit(jitCoder->jitFunction,
												 _ILJitStackItemValue(value),
												 returnType);
		jit_insn_return(jitCoder->jitFunction,
						returnValue);
	}
}

static void JITCoder_LoadFuncAddr(ILCoder *coder, ILMethod *methodInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(methodInfo);
#ifndef IL_JIT_FNPTR_ILMETHOD
	void *function; /* vtable pointer for the function. */
	ILJitValue functionPtr; /* jit value containing the vtable pointer. */
#else
	ILJitValue functionPtr = jit_value_create_nint_constant(jitCoder->jitFunction,
															_IL_JIT_TYPE_VPTR,
															(jit_nint)methodInfo);
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadFuncAddr: %s.%s\n",
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo));
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return;
		}
		jitFunction = ILJitFunctionFromILMethod(methodInfo);
	}
#ifndef IL_JIT_FNPTR_ILMETHOD
	/* Get the vtable pointer for the function. */
	function = jit_function_to_vtable_pointer(jitFunction);
	functionPtr = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_VPTR,
												(jit_nint)function);
#endif
	/* Push the function pointer on the stack. */
	_ILJitStackPushValue(jitCoder, functionPtr);
}

static void JITCoder_LoadVirtualAddr(ILCoder *coder, ILMethod *methodInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(object);
	ILJitValue jitFunction;
#ifdef IL_JIT_FNPTR_ILMETHOD
	ILJitValue classPrivate;
	ILJitValue vtable;
	ILJitValue vtableIndex;
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadVirtualAddr: %s.%s at slot %i\n", 
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo),
			methodInfo->index);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPop(jitCoder, object);
#ifdef IL_JIT_FNPTR_ILMETHOD
	_ILJitStackItemCheckNull(jitCoder, object);
	classPrivate = _ILJitGetObjectClassPrivate(jitCoder->jitFunction, _ILJitStackItemValue(object));
	vtable = jit_insn_load_relative(jitCoder->jitFunction, classPrivate, 
									offsetof(ILClassPrivate, vtable),
									_IL_JIT_TYPE_VPTR);
	vtableIndex = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_INT32,
												 (jit_nint)methodInfo->index);
	jitFunction = jit_insn_load_elem(jitCoder->jitFunction,
									 vtable, vtableIndex, _IL_JIT_TYPE_VPTR);
#else
	jitFunction = _ILJitGetVirtualFunction(jitCoder,
										   &object,
										   methodInfo->index);
#endif
	/* Push the function pointer on the stack. */
	_ILJitStackPushValue(jitCoder, jitFunction);
}

static void JITCoder_LoadInterfaceAddr(ILCoder *coder, ILMethod *methodInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(object);
	ILJitValue jitFunction;
#ifdef IL_JIT_FNPTR_ILMETHOD
	ILJitValue args[3];
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"LoadInterfaceAddr: %s.%s at slot %i\n", 
			ILClass_Name(ILMethod_Owner(methodInfo)),
			ILMethod_Name(methodInfo),
			methodInfo->index);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackPop(jitCoder, object);
#ifdef IL_JIT_FNPTR_ILMETHOD
	_ILJitStackItemCheckNull(jitCoder, object);
	args[0] = _ILJitGetObjectClassPrivate(jitCoder->jitFunction,
										  _ILJitStackItemValue(object));
	args[1] = jit_value_create_nint_constant(jitCoder->jitFunction,
											 _IL_JIT_TYPE_VPTR,
											 (jit_nint)methodInfo->member.owner);
	args[2] = jit_value_create_nint_constant(jitCoder->jitFunction,
											 _IL_JIT_TYPE_UINT32,
											 (jit_nint)methodInfo->index);

	jitFunction = jit_insn_call_native(jitCoder->jitFunction,
									   "_ILRuntimeLookupInterfaceILMethod",
									   _ILRuntimeLookupInterfaceILMethod,
									   _ILJitSignature_ILRuntimeLookupInterfaceMethod,
									   args, 3, 0);

#else
	jitFunction = _ILJitGetInterfaceFunction(jitCoder,
											 &object,
											 methodInfo->member.owner,
											 methodInfo->index);
#endif
	/* Push the function pointer on the stack. */
	_ILJitStackPushValue(jitCoder, jitFunction);
}

#endif	/* IL_JITC_CODE */
