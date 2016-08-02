/*
 * jitc_delegate.c - Jit coder delegates support.
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

/*
 * Build the Invoke method for the delegate.
 */
static int _ILJitCompileDelegateInvoke(ILJitFunction func);

/*
 * Build the BeginInvoke method for a delegate.
 */
static int _ILJitCompileDelegateBeginInvoke(ILJitFunction func);

/*
 * Build the EndInvoke method for a delegate.
 */
static int _ILJitCompileDelegateEndInvoke(ILJitFunction func);

/*
 * Build the Invoke method for the multicast delegate.
 */
static int _ILJitCompileMultiCastDelegateInvoke(ILJitFunction func);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_FUNCTIONS

/*
 * The class name for the AsyncResult.
 */
static const char *asyncResultClassName = "System.Runtime.Remoting.Messaging.AsyncResult";

/*
 * Pack the delegate invoke arguments into an "Object[]" array and return the
 * new array.
 */
static ILJitValue _ILJitPackDelegateArgs(ILJitFunction jitFunction,
										 ILExecThread *_thread,
										 ILType *invokeSignature,
										 ILJitValue *invokeArgs,
										 ILUInt32 firstArg,
										 ILUInt32 numArgs)
{
	ILUInt32 param;
	ILType *paramType;
	ILType *enumType;
	ILClass *info;
	ILUInt32 typeSize;
	ILJitValue thread = _ILJitFunctionGetThread(jitFunction);
	ILJitValue array;
	ILJitValue arrayBase;
	ILJitValue boxObject;
	ILJitValue boxValue;
	ILJitValue boxObjectSize;
	ILJitValue ptr;

	/* Allocate an array to hold all of the arguments */
	array = _ILJitSObjectArrayCreate(jitFunction, _thread, thread, numArgs);
	if(!array)
	{
		return 0;
	}

	arrayBase = jit_insn_add_relative(jitFunction, array, sizeof(System_Array));

	/* Convert the arguments into objects in the array */
	for(param = 0; param < numArgs; ++param)
	{
		paramType = ILTypeGetParam(invokeSignature, firstArg + param);
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
														_ILExecThreadProcess(_thread));

				if(!jitType)
				{
					return 0;
				}

				boxValue = jit_insn_load_relative(jitFunction,
												  invokeArgs[param],
												  0, jitType);
			}
			else if(ILType_IsValueType(paramType))
			{
				ptr = invokeArgs[param];
			}
			else if(ILTypeIsReference(paramType))
			{
				/* Ref to an object reference type: pass the object reference */
				boxValue = jit_insn_load_relative(jitFunction,
												  invokeArgs[param],
												  0, _IL_JIT_TYPE_VPTR);
				jit_insn_store_relative(jitFunction,
										arrayBase,
										param * sizeof(ILObject *),
										boxValue);
				boxValue = 0;
			}
			else
			{
				/* Assume that everything else is a pointer, and wrap
				it up within a "System.IntPtr" object */
				boxValue = jit_insn_load_relative(jitFunction,
												  invokeArgs[param],
												  0, _IL_JIT_TYPE_VPTR);
				paramType = ILType_Int;
			}
		}
		else
		{
			enumType = ILTypeGetEnumType(paramType);
			if(ILType_IsPrimitive(enumType))
			{
				ILJitType jitType = _ILJitGetReturnType(enumType,
														_ILExecThreadProcess(_thread));

				if(!jitType)
				{
					return 0;
				}

				boxValue = _ILJitValueConvertImplicit(jitFunction,
													  invokeArgs[param],
													  jitType);
			}
			else if(ILType_IsValueType(paramType))
			{
				ptr = jit_insn_address_of(jitFunction,
										  invokeArgs[param]);
			}
			else if(ILTypeIsReference(paramType))
			{
				/* Object reference type: pass it directly */
				jit_insn_store_relative(jitFunction,
										arrayBase,
										param * sizeof(ILObject *),
										invokeArgs[param]);
			}
			else
			{
				/* Assume that everything else is a pointer, and wrap
				it up within a "System.IntPtr" object */
				boxValue = _ILJitValueConvertImplicit(jitFunction, 
													  invokeArgs[param],
													  _IL_JIT_TYPE_VPTR);
				paramType = ILType_Int;
			}
		}
		if(boxValue || ptr)
		{
			/* We have to box the argument. */
			info = ILClassFromType
							(ILContextNextImage(_thread->process->context, 0),
			 				0, paramType, 0);
			info = ILClassResolve(info);
			typeSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(_thread), paramType);

			boxObject = _ILJitAllocObjectGen(jitFunction, info);
			if(boxValue)
			{
				jit_insn_store_relative(jitFunction, boxObject, 0, boxValue);
			}
			else
			{
				boxObjectSize =
					jit_value_create_nint_constant(jitFunction,
												   _IL_JIT_TYPE_UINT32,
												   typeSize);

				jit_insn_memcpy(jitFunction, boxObject, ptr, boxObjectSize);
			}
			jit_insn_store_relative(jitFunction,
									arrayBase,
									param * sizeof(ILObject *),
									boxObject);
		}
	}

	return array;
}

/*
 * Unpack the delegate out arguments from the "Object[]" array to the given
 * locations.
 */
static int _ILJitUnpackDelegateArgs(ILJitFunction jitFunction,
									 ILExecThread *_thread,
									 ILType *endInvokeSignature,
									 ILJitValue *endInvokeArgs,
									 ILJitValue array,
									 ILUInt32 firstArg,
									 ILUInt32 numArgs)
{
	ILJitValue arrayBaseOffset = jit_value_create_nint_constant(jitFunction,
																_IL_JIT_TYPE_UINT32,
																(jit_nint)(sizeof(System_Array)));
	ILJitValue arrayBase = jit_insn_add(jitFunction, array, arrayBaseOffset);
	ILInt32 i;
	
	for (i = 0; i < numArgs; i++)
	{
		ILType *paramType;

		paramType = ILTypeGetParam(endInvokeSignature, firstArg + i);
		paramType = ILType_Ref(paramType);
		paramType = ILTypeGetEnumType(paramType);
		
		if(ILType_IsPrimitive(paramType))
		{
			ILJitType jitType = _ILJitGetReturnType(paramType, _ILExecThreadProcess(_thread));
			ILJitValue srcPtr;
			ILJitValue value;

			if(!jitType)
			{
				return 0;
			}

			srcPtr = jit_insn_load_relative(jitFunction,
											arrayBase,
											i * sizeof(void *),
											_IL_JIT_TYPE_VPTR);
			value = jit_insn_load_relative(jitFunction, srcPtr, 0, jitType);
			jit_insn_store_relative(jitFunction, endInvokeArgs[i], 0, value);
		}
		else if(ILType_IsValueType(paramType))
		{
			ILJitType jitType = _ILJitGetReturnType(paramType, _ILExecThreadProcess(_thread));

			ILUInt32 typeSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(_thread), paramType);
			ILJitValue boxObjectSize = jit_value_create_nint_constant(jitFunction,
																	  _IL_JIT_TYPE_UINT32,
																	  typeSize);
			ILJitValue srcPtr = jit_insn_load_relative(jitFunction,
													   arrayBase,
													   i * sizeof(void *),
													   _IL_JIT_TYPE_VPTR);

			if(!jitType)
			{
				return 0;
			}

			jit_insn_memcpy(jitFunction, endInvokeArgs[i], srcPtr, boxObjectSize);
		}
		else if (ILType_IsClass(paramType))
		{
			/* If the param is a class reference then set the new class reference */
			ILJitValue value;

			value = jit_insn_load_relative(jitFunction,
											arrayBase,
											i * sizeof(void *),
											_IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(jitFunction, endInvokeArgs[i], 0, value);
		}
		else
		{
			/* Don't know how to return this type of out param */
		}
	}
	return 1;
}

/*
 * Unbox the object returned by EndInvoke and generate the return statement.
 */
static int _ILJitDelegateUnboxReturn(ILJitFunction func,
									 ILExecThread *_thread,
									 ILType *endInvokeSignature,
									 ILJitValue returnObject)
{
	ILType *returnType = ILTypeGetParam(endInvokeSignature, 0); /* 0 == return type */

	returnType = ILTypeGetEnumType(returnType);

	if(ILType_IsPrimitive(returnType) || ILType_IsValueType(returnType))
	{
		ILJitType jitType = _ILJitGetReturnType(returnType,
												_ILExecThreadProcess(_thread));
		if(!jitType)
		{
			return 0;
		}

		if(jitType != _IL_JIT_TYPE_VOID)
		{
			jit_insn_return_ptr(func, returnObject, jitType);
		}
	}
	else if (ILType_IsClass(returnType))
	{
		jit_insn_return(func, returnObject);
	}
	else
	{
		/* don't know how to return this type. */
		return 0;
	}
	return 1;
}

/*
 * Emit the code to call a delegate.
 * args contains only the arguments passed to the method (excluding the this pointer).
 */
static ILJitValue _ILJitDelegateInvokeCodeGen(ILJitFunction func,
											  ILJitFunction invokeFunc,
											  ILJitValue delegate,
											  ILJitValue *args,
											  ILUInt32 argCount)
{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ILJitValue thread = _ILJitFunctionGetThread(func);
#endif
	ILJitType signature = jit_function_get_signature(invokeFunc);
	ILUInt32 numArgs = jit_type_num_params(signature);
	ILJitType returnType = jit_type_get_return(signature);
	jit_label_t noTarget = jit_label_undefined;
	jit_label_t endLabel = jit_label_undefined;
	ILJitValue returnValue = 0;
	ILJitValue temp;
	ILJitValue target = jit_insn_load_relative(func, delegate,
											   offsetof(System_Delegate, target),
											   _IL_JIT_TYPE_VPTR);
	ILJitValue function = jit_insn_load_relative(func, delegate,
												 offsetof(System_Delegate, methodInfo),
												 _IL_JIT_TYPE_VPTR);
	ILJitType types[numArgs];
	ILJitValue invokeArgs[numArgs];
	ILUInt32 current;

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(argCount != numArgs - 2)
#else
	if(argCount != numArgs - 1)
#endif
	{
		/* Something is wrong here. */
	}

	if(returnType != _IL_JIT_TYPE_VOID)
	{
		returnValue = jit_value_create(func, returnType);
	}
#ifdef IL_JIT_FNPTR_ILMETHOD
	/* We need to convert the methodInfo pointer to the vtable pointer first. */
	function = jit_insn_call_native(func,
									"ILRuntimeMethodToVtablePointer",
									ILRuntimeMethodToVtablePointer,
									_ILJitSignature_ILRuntimeMethodToVtablePointer,
									&function, 1, 0);
#endif

	jit_insn_branch_if_not(func, target, &noTarget);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	invokeArgs[0] = thread;
	invokeArgs[1] = target;
#else
	invokeArgs[0] = target;
#endif
	for(current = 0; current < argCount; ++current)
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		invokeArgs[current + 2] = args[current];
	#else
		invokeArgs[current + 1] = args[current];
	#endif
	}
	temp = jit_insn_call_indirect_vtable(func,
										 function, signature,
										 invokeArgs, numArgs, 0);
	if(returnType != _IL_JIT_TYPE_VOID)
	{
		jit_insn_store(func, returnValue, temp);
	}
	jit_insn_branch(func, &endLabel);
	jit_insn_label(func, &noTarget);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	types[0] = jit_type_get_param(signature, 0);
	invokeArgs[0] = thread;
#endif
	for(current = 0; current < argCount; ++current)
	{
	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		types[current + 1] = jit_type_get_param(signature, current + 2);
		invokeArgs[current + 1] = args[current];
	#else
		types[current] = jit_type_get_param(signature, current + 1);
		invokeArgs[current] = args[current];
	#endif
	}
	if(!(signature = jit_type_create_signature(IL_JIT_CALLCONV_DEFAULT,
											   returnType,
											   types, numArgs - 1, 1)))
	{
		return 0;
	}
	temp = jit_insn_call_indirect_vtable(func,
										 function, signature,
										 invokeArgs, numArgs - 1, 0);
	jit_type_free(signature);
	if(returnType != _IL_JIT_TYPE_VOID)
	{
		jit_insn_store(func, returnValue, temp);
	}
	jit_insn_label(func, &endLabel);
	return returnValue;
}

/*
 * Build the Invoke method for the delegate.
 * The arguments to this function are:
 * arg 0 = thread
 * arg 1 = this pointer to the delegate
 * arg 2 ... n = arguments for the delegate function
 */
static int _ILJitCompileDelegateInvoke(ILJitFunction func)
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	ILMethod *method = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILClass *info = ILMethod_Owner(method);
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;
	ILJITCoder *jitCoder = (ILJITCoder *)(classPrivate->process->coder);
	char *methodName = _ILJitFunctionGetMethodName(func);
#endif
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ILJitValue thread = _ILJitFunctionGetThread(func);
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "CompileDelegateInvoke: %s\n", methodName);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/* TODO: Add the code generation here. */

	return JIT_RESULT_OK;
}

/*
 * Build the BeginInvoke method for a delegate.
 * The arguments to this function are:
 * arg 0 = thread
 * arg 1 = this pointer to the delegate
 * arg 2 ... n = arguments for the delegate function
 * arg n+1 = System.AsyncCallback
 * arg n+2 = IAsyncResult
 */
static int _ILJitCompileDelegateBeginInvoke(ILJitFunction func)
{
	ILExecThread *_thread = ILExecThreadCurrent();
	ILMethod *beginInvokeMethod = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILType *beginInvokeSignature = ILMethod_Signature(beginInvokeMethod);
	ILJitType jitSignature = jit_function_get_signature(func);
	unsigned int numBeginInvokeParams = jit_type_num_params(jitSignature);
	ILClass *asyncResultInfo = _ILLookupClass(_ILExecThreadProcess(_thread),
											  asyncResultClassName,
											  strlen(asyncResultClassName));
	ILMethod *asyncResultCtor = 0;
	ILJitFunction jitAsyncResultCtor = 0;
	
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	ILClass *info = ILMethod_Owner(beginInvokeMethod);
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;
	ILJITCoder *jitCoder = (ILJITCoder *)(classPrivate->process->coder);
	char *methodName = _ILJitFunctionGetMethodName(func);
#endif
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ILJitValue ctorArgs[6];
#else
	ILJitValue ctorArgs[5];
#endif
	ILJitValue args[numBeginInvokeParams];
	ILUInt32 current;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "CompileDelegateBeginInvoke: %s\n", methodName);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/* Check if the class AsyncResult was found. */
	if(!asyncResultInfo)
	{
		return JIT_RESULT_COMPILE_ERROR;
	}

	asyncResultCtor = ILExecThreadLookupMethodInClass(_thread,
								asyncResultInfo,
								".ctor",
								"(ToSystem.Delegate;[oSystem.Object;oSystem.AsyncCallback;oSystem.Object;)V");

	/* Check if the ctor for AsyncResult class was found. */
	if(!asyncResultCtor)
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
	asyncResultInfo = ILMethod_Owner(asyncResultCtor);

	jitAsyncResultCtor = ILJitFunctionFromILMethod(asyncResultCtor);
	/* Make sure the class is allready layouted. */
	if(!jitAsyncResultCtor)
	{
		/* We have to layout the class first. */
		if(!_LayoutClass(_thread, asyncResultInfo))
		{
			return JIT_RESULT_COMPILE_ERROR;
		}
		jitAsyncResultCtor = ILJitFunctionFromILMethod(asyncResultCtor);
	}

	/* get the arguments passed to this function. */
	for(current = 0; current < numBeginInvokeParams; ++current)
	{
		args[current] = jit_value_get_param(func, current);
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ctorArgs[0] = _ILJitFunctionGetThread(func);
	if(!(ctorArgs[1] = _ILJitAllocObjectGen(func, asyncResultInfo)))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
	ctorArgs[2] = args[1]; /* _this */
	if(!(ctorArgs[3] = _ILJitPackDelegateArgs(func, _thread,
											  beginInvokeSignature,
								  			  &(args[2]),
								  			  1, numBeginInvokeParams - 4)))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
	ctorArgs[4] = args[numBeginInvokeParams - 2];
	ctorArgs[5] = args[numBeginInvokeParams - 1];

	/* Call the ctor for the AsyncResult. */
	jit_insn_call(func, 0, jitAsyncResultCtor, 0, ctorArgs, 6, 0);

	jit_insn_return(func, ctorArgs[1]);
#else
	if(!(ctorArgs[0] = _ILJitAllocObjectGen(func, asyncResultInfo)))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
	ctorArgs[1] = args[0]; /* _this */
	if(!(ctorArgs[2] = _ILJitPackDelegateArgs(func, _thread,
											  beginInvokeSignature,
								  			  &(args[1]),
								  			  1, numBeginInvokeParams - 3)))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
	ctorArgs[3] = args[numBeginInvokeParams - 2];
	ctorArgs[4] = args[numBeginInvokeParams - 1];

	/* Call the ctor for the AsyncResult. */
	jit_insn_call(func, 0, jitAsyncResultCtor, 0, ctorArgs, 5, 0);

	jit_insn_return(func, ctorArgs[0]);
#endif

	return JIT_RESULT_OK;
}

/*
 * Build the EndInvoke method for a delegate.
 * The arguments to this function are:
 * arg 0 = thread
 * arg 1 = this pointer to the delegate
 * arg 2 ... n = by ref arguments for the delegate function
 * arg n+1 = IAsyncResult
 */
static int _ILJitCompileDelegateEndInvoke(ILJitFunction func)
{
	ILExecThread *_thread = ILExecThreadCurrent();
	ILMethod *endInvokeMethod = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILType *endInvokeSignature = ILMethod_Signature(endInvokeMethod);
	ILJitType jitSignature = jit_function_get_signature(func);
	unsigned int numEndInvokeParams = jit_type_num_params(jitSignature);
	ILClass *asyncResultInfo = _ILLookupClass(_ILExecThreadProcess(_thread),
											  asyncResultClassName,
											  strlen(asyncResultClassName));
	ILMethod *asyncEndInvokeMethodInfo = 0;
	ILJitValue array;         /* Array to hold the out params. */
	ILJitValue returnObject;  /* returnvalue of the call to IAsyncResult.EndInvoke. */
	ILJitFunction jitEndInvoke;

	ILJitValue thread = _ILJitFunctionGetThread(func);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ILUInt32 numOutParams = numEndInvokeParams - 3;
	ILJitValue endInvokeArgs[3];
#else
	ILUInt32 numOutParams = numEndInvokeParams - 2;
	ILJitValue endInvokeArgs[2];
#endif
	ILJitValue args[numEndInvokeParams];
	ILUInt32 current;
	
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	ILClass *info = ILMethod_Owner(endInvokeMethod);
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;
	ILJITCoder *jitCoder = (ILJITCoder *)(classPrivate->process->coder);
	char *methodName = _ILJitFunctionGetMethodName(func);
#endif

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "CompileDelegateEndInvoke: %s\n", methodName);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/* Check if the class AsyncResult was found. */
	if(!asyncResultInfo)
	{
		return JIT_RESULT_COMPILE_ERROR;
	}

	asyncEndInvokeMethodInfo = ILExecThreadLookupMethodInClass(_thread,
										asyncResultInfo,
										"EndInvoke",
										"(T[oSystem.Object;)oSystem.Object;");

	/* Check if the EndInvoke Method for AsyncResult class was found. */
	if(!asyncEndInvokeMethodInfo)
	{
		return JIT_RESULT_COMPILE_ERROR;
	}

	jitEndInvoke = ILJitFunctionFromILMethod(asyncEndInvokeMethodInfo);
	if(!jitEndInvoke)
	{
		/* We have to layout the class first. */
		if(!_LayoutClass(_thread, ILMethod_Owner(asyncEndInvokeMethodInfo)))
		{
			return JIT_RESULT_COMPILE_ERROR;
		}
		jitEndInvoke = ILJitFunctionFromILMethod(asyncEndInvokeMethodInfo);
	}

	/* get the arguments passed to this function. */
	for(current = 0; current < numEndInvokeParams; ++current)
	{
		args[current] = jit_value_get_param(func, current);
	}

	/* Allocate an array to hold the out arguments */
	array = _ILJitSObjectArrayCreate(func, _thread, thread, numOutParams);
	if(!array)
	{
		return JIT_RESULT_COMPILE_ERROR;
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	endInvokeArgs[0] = thread;
	endInvokeArgs[1] = args[numEndInvokeParams - 1];
	endInvokeArgs[2] = array;
	returnObject = jit_insn_call(func, 0, jitEndInvoke,
						  0, endInvokeArgs, 3, 0);
	if(!(_ILJitUnpackDelegateArgs(func, _thread, endInvokeSignature,
								  &(args[2]), array, 1, numOutParams)))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
#else
	endInvokeArgs[0] = args[numEndInvokeParams - 1];
	endInvokeArgs[1] = array;
	returnObject = jit_insn_call(func, 0, jitEndInvoke,
						  0, endInvokeArgs, 2, 0);
	if(!(_ILJitUnpackDelegateArgs(func, _thread, endInvokeSignature,
								  &(args[1]), array, 1, numOutParams)))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
#endif

	if(!_ILJitDelegateUnboxReturn(func, _thread, endInvokeSignature,
												 returnObject))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}

	return JIT_RESULT_OK;
}

/*
 * Build the Invoke method for the multicast delegate.
 * The arguments to this function are:
 * arg 0 = thread
 * arg 1 = this pointer to the multicast delegate
 * arg 2 ... n = arguments for the delegate function
 */
static int _ILJitCompileMultiCastDelegateInvoke(ILJitFunction func)
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	ILMethod *method = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILClass *info = ILMethod_Owner(method);
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;
	ILJITCoder *jitCoder = (ILJITCoder *)(classPrivate->process->coder);
	char *methodName = _ILJitFunctionGetMethodName(func);
#endif
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ILJitValue thread = _ILJitFunctionGetThread(func);
	ILJitValue delegate = jit_value_get_param(func, 1);
#else
	ILJitValue delegate = jit_value_get_param(func, 0);
#endif
	ILJitType signature = jit_function_get_signature(func);
	ILUInt32 numArgs = jit_type_num_params(signature);
	jit_label_t invokeThis = jit_label_undefined;
	ILJitValue returnValue;
	ILJitValue prevDelegate = jit_insn_load_relative(func, delegate,
													 offsetof(System_Delegate, prev),
													 _IL_JIT_TYPE_VPTR);
	ILJitValue args[numArgs]; /* The array for the invokation args. */
	ILUInt32 current;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "CompileMulticastDelegateInvoke: %s\n", methodName);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(numArgs < 2)
#else
	if(numArgs < 1)
#endif
	{
		/* There is something wrong with this delegate. */
		return JIT_RESULT_COMPILE_ERROR;
	}
	/* If there is no previous delegate then invoke the delegate method. */
	jit_insn_branch_if_not(func, prevDelegate, &invokeThis);
	/* We need to invoke the previous delegate first. */
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	args[0] = thread;
	args[1] = prevDelegate;
	for(current = 2; current < numArgs; ++current)
#else
	args[0] = prevDelegate;
	for(current = 1; current < numArgs; ++current)
#endif
	{
		args[current] = jit_value_get_param(func, current);
	}
	returnValue = jit_insn_call(func, 0, func, 0, args, numArgs, 0);
	jit_insn_label(func, &invokeThis);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	returnValue = _ILJitDelegateInvokeCodeGen(func, func, delegate,
											  &(args[2]), numArgs - 2);
#else
	returnValue = _ILJitDelegateInvokeCodeGen(func, func, delegate,
											  &(args[1]), numArgs - 1);
#endif
	jit_insn_return(func, returnValue);

	return JIT_RESULT_OK;
}

#endif	/* IL_JITC_FUNCTIONS */

