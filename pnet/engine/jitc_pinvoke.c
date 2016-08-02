/*
 * jitc_pinvoke.c - Handle marshaling within the JIT.
 *
 * Copyright (C) 2006  Southern Storm Software, Pty Ltd.
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

/*
 * For enabling extended marshaling - of byref structures and arrays of structures uncomment
 * the following define.
 */
// #define USE_BYREF_MARSHALING 1

#define MARSHAL_FIRST_LEVEL_VALUE		0
#define MARSHAL_ITEM_OF_STRUCTURE		1
#define MARSHAL_ITEM_OF_ARRAY			2

static int NeedMarshalValue(ILType *type);
static int NeedMarshalStruct(ILType *structureILType);
static ILJitValue MarshalValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
				    unsigned int addressKind, jit_nint offset, ILJitValue outAddress);
static ILJitValue MarshalStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
				    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset, unsigned int addressKind);
static ILJitValue MarshalReturnValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
					unsigned int addressKind, jit_nint offset, ILJitValue outAddress);
static ILJitValue MarshalReturnStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
					ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
					unsigned int addressKind);
#ifdef USE_BYREF_MARSHALING
static void MarshalByRefValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
				unsigned int addressKind, jit_nint offset, ILJitValue outAddress);
static void MarshalByRefStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType, ILUInt32 marshalType,
				ILJitValue outAddress, jit_nint offset, unsigned int addressKind);
#endif
void *ILJitDelegateGetClosure(ILObject *del, ILType *delType);
static ILJitValue MarshalDelegateValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
					unsigned int addressKind, jit_nint offset, ILJitValue outAddress);
static ILJitValue MarshalDelegateStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
					    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
					    unsigned int addressKind);
static ILJitValue MarshalDelegateReturnValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
						unsigned int addressKind, jit_nint offset, ILJitValue outAddress);
static ILJitValue MarshalDelegateReturnStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
						ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
						unsigned int addressKind);
#ifdef USE_BYREF_MARSHALING
static void MarshalDelegateByRefValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
					unsigned int addressKind, jit_nint offset, ILJitValue outAddress);
static void MarshalDelegateByRefStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
					    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
					    unsigned int addressKind);
#endif
static void *MarshalObjectToCustom(void *value, ILType *type, ILMethod *method, ILExecThread *thread);
static void *MarshalCustomToObject(void *value, ILType *type, ILMethod *method, ILExecThread *thread);
/*
 * On demand code generator.for functions implemented in IL code.
 */
static int _ILJitCompilePinvoke(jit_function_t func)
{	
	ILMethod *method = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILClass *info = ILMethod_Owner(method);
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;
	ILJITCoder *jitCoder = (ILJITCoder*)(classPrivate->process->coder);
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	char *methodName = _ILJitFunctionGetMethodName(func);
#endif
	ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)(method->userData);
	ILType *ilSignature = ILMethod_Signature(method);
	ILType *type;
	ILJitType signature = jit_function_get_signature(func);
	ILPInvoke *pinv = ILPInvokeFind(method);
	jit_abi_t jitAbi;
	ILUInt32 numParams = jit_type_num_params(signature);
	ILJitType returnType = jit_type_get_return(signature);
	ILJitValue returnValue = jit_value_create(func, jit_type_void_ptr);
	unsigned int current;
	ILType *valueType;
	ILUInt32 marshalType;
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;
	
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "CompilePinvoke: %s\n", methodName);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	if(!pinv)
	{
		/* The pinvoke record could not be found. */
		return JIT_RESULT_COMPILE_ERROR;
	}

	/* Setup the needed stuff in the jitCoder. */
	jitCoder->jitFunction = func;

#ifndef IL_JIT_THREAD_IN_SIGNATURE
	/* Reset the cached thread. */
	jitCoder->thread = 0;
#endif

#ifdef ENHANCED_PROFILER
	/* Reset the timestamps */
	jitCoder->profileTimestamp = 0;
	jitCoder->inlineTimestamp = 0;
#endif /* ENHANCED_PROFILER */

	/* Check if the method to invoke was found on this system. */
	if(_ILJitPinvokeError(jitMethodInfo->fnInfo))
	{
		if(jitMethodInfo->fnInfo.un.func == _IL_JIT_PINVOKE_DLLNOTFOUND)
		{
			_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_DLL_NOT_FOUND);
		}
		else if(jitMethodInfo->fnInfo.un.func == _IL_JIT_PINVOKE_ENTRYPOINTNOTFOUND)
		{
			_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_ENTRYPOINT_NOT_FOUND);
		}
		return JIT_RESULT_OK;
	}

	/* determine which calling convention to use. */
	switch(pinv->member.attributes & IL_META_PINVOKE_CALL_CONV_MASK)
	{
		case IL_META_PINVOKE_CALL_CONV_WINAPI:
		{
			/* TODO: There is no winapi calling convention in libjit. */
			jitAbi = IL_JIT_CALLCONV_STDCALL;
		}
		break;

		case IL_META_PINVOKE_CALL_CONV_CDECL:
		{
			jitAbi = IL_JIT_CALLCONV_CDECL;
		}
		break;

		case IL_META_PINVOKE_CALL_CONV_STDCALL:
		{
			jitAbi = IL_JIT_CALLCONV_STDCALL;
		}
		break;

		case IL_META_PINVOKE_CALL_CONV_FASTCALL:
		{
			jitAbi = IL_JIT_CALLCONV_FASTCALL;
		}
		break;

		default:
		{
			/* There is an invalid calling convention in the metadata. */
			return JIT_RESULT_COMPILE_ERROR;
		}
		break;
	}

	ILJitType paramType;
	ILJitType jitParamTypes[numParams + 1];
	ILJitValue jitParams[numParams + 1];
	ILJitValue tempParams[numParams + 1];
	ILUInt32 param = 0;
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	for(current = 1; current < numParams; ++current)
	
#else
	for(current = 0; current < numParams; ++current)
#endif
	{
		paramType = jit_type_get_param(signature, current);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		valueType = ILTypeGetParam(ilSignature, current);
#else
		valueType = ILTypeGetParam(ilSignature, current + 1);
#endif
		valueType = ILTypeGetEnumType(valueType);
		tempParams[param] = jit_value_get_param(func, current);
		marshalType = ILPInvokeGetMarshalType(pinv, method, current, &customName, &customNameLen,
								    &customCookie, &customCookieLen, valueType);
		jitParams[param] = jit_value_get_param(func, current);
		if(marshalType==IL_META_MARSHAL_CUSTOM)
		{
			ILJitValue params[4];
			params[0] = tempParams[param];
			params[1] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)valueType);
			params[2] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)method);
			params[3] = _ILJitFunctionGetThread(func);
			returnValue = jit_insn_call_native(func, "MarshalObjectToCustom", MarshalObjectToCustom,
								_ILJitSignature_MarshalObjectToCustom, params, 4,
								JIT_CALL_NOTHROW);
		}
		else if(ILTypeIsStringClass(valueType))
		{
			 ILJitValue args[2];
			 args[0] = _ILJitFunctionGetThread(func);
			 args[1] = jit_value_get_param(func, current);
	  	 	 switch(marshalType)
			 {
				case IL_META_MARSHAL_ANSI_STRING:
				{
					jitParams[param] = jit_insn_call_native(func,
										    "ILStringToAnsi",
										    ILStringToAnsi,
										    _ILJitSignature_ILStringToAnsi,
										    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF8_STRING:
				{
					jitParams[param] = jit_insn_call_native(func,
										    "ILStringToUTF8",
										    ILStringToUTF8,
										    _ILJitSignature_ILStringToUTF8,
										    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF16_STRING:
				{
					jitParams[param] = jit_insn_call_native(func,
	    									    "ILStringToUTF16",
										    ILStringToUTF16,
										    _ILJitSignature_ILStringToUTF16,
										    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				default: break;
	    		}
		}
		else if(marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(valueType))
		{
			jitParams[param] = (ILJitValue)MarshalValue(func, tempParams[param], valueType, marshalType,
								    MARSHAL_FIRST_LEVEL_VALUE, 0, 0);
		}
		jitParamTypes[param] = paramType;
		++param;
	}
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(numParams==2 && jitParamTypes[1]==jit_type_void)
	{
		numParams = 1;
	}
#else
	if(numParams==1 && jitParamTypes[0]==jit_type_void)
	{
		numParams = 0;
	}
#endif
	ILJitType callSignature = jit_type_create_signature(jitAbi,
								   returnType,
								   jitParamTypes,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
								   numParams - 1, 1);
#else
								   numParams, 1);
#endif
	if(returnType==jit_type_void)
	{
		jit_insn_call_native(func, 0, jitMethodInfo->fnInfo.un.func,
								callSignature,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
								jitParams, numParams - 1, 0);
#else
								jitParams, numParams, 0);
#endif
	}
	else
	{
		type = ILTypeGetEnumType(ILTypeGetReturn(ilSignature));
		returnValue = jit_insn_call_native(func, 0, jitMethodInfo->fnInfo.un.func,
								 callSignature,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
								 jitParams, numParams - 1, 0);
#else
								 jitParams, numParams, 0);
#endif
		marshalType = ILPInvokeGetMarshalType
				(0, method, 0, &customName, &customNameLen,
				 &customCookie, &customCookieLen, type);
		if(marshalType==IL_META_MARSHAL_CUSTOM)
		{
			ILJitValue params[4];
			params[0] = returnValue;
			params[1] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)type);
			params[2] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)method);
			params[3] = _ILJitFunctionGetThread(func);
			returnValue = jit_insn_call_native(func, "MarshalCustomToObject",
								MarshalCustomToObject,
								_ILJitSignature_MarshalCustomToObject,
								params, 4, JIT_CALL_NOTHROW);
		}
		else if(ILTypeIsStringClass(type))
		{
			ILJitValue args[2];
			args[0] = _ILJitFunctionGetThread(func);
			args[1] = returnValue;
			switch(marshalType)
			{
				case IL_META_MARSHAL_ANSI_STRING:
				{
					returnValue = jit_insn_call_native(func,
									    "ILStringCreate",
									    ILStringCreate,
									    _ILJitSignature_ILStringCreate,
									    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF8_STRING:
				{
					returnValue = jit_insn_call_native(func,
									    "ILStringCreateUTF8",
									    ILStringCreateUTF8,
									    _ILJitSignature_ILStringCreateUTF8,
									    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF16_STRING:
				{
					returnValue = jit_insn_call_native(func,
									    "ILStringWCreate",
									    ILStringWCreate,
									    _ILJitSignature_ILStringWCreate,
									    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				default: break;
			}
		}
		else if(marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
		{
			returnValue = (ILJitValue)MarshalReturnValue(func, returnValue, type, marshalType,
									MARSHAL_FIRST_LEVEL_VALUE, 0, 0);
		}
	}

#ifdef USE_BYREF_MARSHALING
	param = 0;
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	for(current = 1; current < numParams; ++current)
#else
	for(current = 0; current < numParams; ++current)
#endif
	{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		type = ILTypeGetParam(ilSignature, current);
#else
		type = ILTypeGetParam(ilSignature, current + 1);
#endif
		type = ILTypeGetEnumType(type);

		// Does anyone use this?
		marshalType = ILPInvokeGetMarshalType
					(pinv, method, current, &customName, &customNameLen,
					 &customCookie, &customCookieLen, type);
		if((marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
			&& marshalType != IL_META_MARSHAL_CUSTOM && type != 0)
		{
			if(ILType_IsSimpleArray(type))
			{
				 MarshalByRefValue(func, jitParams[param], type, 
							    marshalType, MARSHAL_FIRST_LEVEL_VALUE, 
							    0, tempParams[param]);
			}
			else if(ILType_IsComplex(type))
			{
				typeFlag = ILType_Kind(type);
			    	if(typeFlag==IL_TYPE_COMPLEX_BYREF)
				{
					// Does anyone use this?
					MarshalByRefValue(func, jitParams[param], type,
								    marshalType, MARSHAL_FIRST_LEVEL_VALUE,
								    0, tempParams[param]);
				}
			}
		}
		++param;
	}
#endif
	if(returnType==jit_type_void)jit_insn_return(func, 0);
	else jit_insn_return(func, returnValue);
	jit_type_free(callSignature);

	return JIT_RESULT_OK;
}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(_IL_JIT_ENABLE_DEBUG)
static ILJitValue _ILJitInlinePinvoke(ILJITCoder *jitCoder, ILMethod *method, ILJitValue *tempParams)
{
	ILJitFunction func = jitCoder->jitFunction;
	ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)(method->userData);
	ILType *ilSignature = ILMethod_Signature(method);
	ILType *type;
	ILJitType signature = jit_function_get_signature(ILJitFunctionFromILMethod(method));
	ILPInvoke *pinv = ILPInvokeFind(method);
	jit_abi_t jitAbi;
	ILUInt32 numParams = jit_type_num_params(signature);
	ILJitType returnType = _ILJitGetReturnType(ILTypeGetEnumType(ILTypeGetReturn(ilSignature)), jitCoder->process);
	ILJitValue returnValue = jit_value_create(func, jit_type_void_ptr);
	unsigned int current;
	ILType *valueType;
	ILUInt32 marshalType;
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;

	if(!pinv)
	{
		/* The pinvoke record could not be found. */
		return 0;
	}

	/* Check if the method to invoke was found on this system. */
	if(_ILJitPinvokeError(jitMethodInfo->fnInfo))
	{
		if(jitMethodInfo->fnInfo.un.func == _IL_JIT_PINVOKE_DLLNOTFOUND)
		{
			_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_DLL_NOT_FOUND);
		}
		else if(jitMethodInfo->fnInfo.un.func == _IL_JIT_PINVOKE_ENTRYPOINTNOTFOUND)
		{
			_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_ENTRYPOINT_NOT_FOUND);
		}
		return 0;
	}

	/* determine which calling convention to use. */
	switch(pinv->member.attributes & IL_META_PINVOKE_CALL_CONV_MASK)
	{
		case IL_META_PINVOKE_CALL_CONV_WINAPI:
		{
			/* TODO: There is no winapi calling convention in libjit. */
			jitAbi = IL_JIT_CALLCONV_STDCALL;
		}
		break;

		case IL_META_PINVOKE_CALL_CONV_CDECL:
		{
			jitAbi = IL_JIT_CALLCONV_CDECL;
		}
		break;

		case IL_META_PINVOKE_CALL_CONV_STDCALL:
		{
			jitAbi = IL_JIT_CALLCONV_STDCALL;
		}
		break;

		case IL_META_PINVOKE_CALL_CONV_FASTCALL:
		{
			jitAbi = IL_JIT_CALLCONV_FASTCALL;
		}
		break;

		default:
		{
			/* There is an invalid calling convention in the metadata. */
			return 0;
		}
		break;
	}

	ILJitType paramType;
	ILJitType jitParamTypes[numParams + 1];
	ILJitValue jitParams[numParams + 1];
	ILUInt32 param = 0;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	/*
	 * Emit the code to start profiling of the method if profiling
	 * is enabled
	 */
	if(jitCoder->flags & IL_CODER_FLAG_METHOD_PROFILE)
	{
		_ILJitProfileStart(jitCoder, method);
	}
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	for(current = 1; current < numParams; ++current)
#else
	for(current = 0; current < numParams; ++current)
#endif
	{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		paramType = _ILJitGetArgType(ILTypeGetEnumType(ILTypeGetParam(ilSignature, current)), jitCoder->process);
		valueType = ILTypeGetParam(ilSignature, current);
#else
		paramType = _ILJitGetArgType(ILTypeGetEnumType(ILTypeGetParam(ilSignature, current + 1)), jitCoder->process);
		valueType = ILTypeGetParam(ilSignature, current + 1);
#endif	
		valueType = ILTypeGetEnumType(valueType);
		marshalType = ILPInvokeGetMarshalType(pinv, method, current, &customName, &customNameLen,
								    &customCookie, &customCookieLen, valueType);
		jitParams[param] = tempParams[current];
		if(marshalType==IL_META_MARSHAL_CUSTOM)
		{
			ILJitValue params[4];
			params[0] = tempParams[current];
			params[1] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)valueType);
			params[2] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)method);
			params[3] = _ILJitFunctionGetThread(func);
			returnValue = jit_insn_call_native(func, "MarshalObjectToCustom", MarshalObjectToCustom,
								_ILJitSignature_MarshalObjectToCustom, params, 4,
								JIT_CALL_NOTHROW);
		}
		else if(ILTypeIsStringClass(valueType))
		{
			 ILJitValue args[2];
			 args[0] = _ILJitCoderGetThread(jitCoder);
			 args[1] = tempParams[current];
	  	 	 switch(marshalType)
			 {
				case IL_META_MARSHAL_ANSI_STRING:
				{
					jitParams[param] = jit_insn_call_native(func,
										    "ILStringToAnsi",
										    ILStringToAnsi,
										    _ILJitSignature_ILStringToAnsi,
										    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF8_STRING:
				{
					jitParams[param] = jit_insn_call_native(func,
									    	    "ILStringToUTF8",
										    ILStringToUTF8,
									    	    _ILJitSignature_ILStringToUTF8,
									    	    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF16_STRING:
				{
					jitParams[param] = jit_insn_call_native(func,
	    									    "ILStringToUTF16",
										    ILStringToUTF16,
										    _ILJitSignature_ILStringToUTF16,
										    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				default: break;
	    		}
		}
		else if(marshalType!=IL_META_MARSHAL_DIRECT || NeedMarshalValue(valueType))
		{
			jitParams[param] = (ILJitValue)MarshalValue(func, tempParams[current], valueType, marshalType,
									MARSHAL_FIRST_LEVEL_VALUE, 0, 0);
		}
		jitParamTypes[param] = paramType;
		++param;
	}
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(numParams==2 && jitParamTypes[0]==jit_type_void)
	{
		numParams = 1;
	}
#else
	if(numParams==1 && jitParamTypes[0]==jit_type_void)
	{
		numParams = 0;
	}
#endif
	ILJitType callSignature = jit_type_create_signature(jitAbi,
								   returnType,
								   jitParamTypes,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
								   numParams - 1, 1);
#else
								   numParams, 1);
#endif
	if(returnType==jit_type_void)
	{
		 jit_insn_call_native(func, 0, jitMethodInfo->fnInfo.un.func,
								callSignature,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
								jitParams, numParams - 1, 0);
#else
								jitParams, numParams, 0);
#endif
	}
	else
	{
		type = ILTypeGetEnumType(ILTypeGetReturn(ilSignature));
		returnValue = jit_insn_call_native(func, 0, jitMethodInfo->fnInfo.un.func,
								 callSignature,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
								 jitParams, numParams - 1, 0);
#else
								 jitParams, numParams, 0);
#endif
		marshalType = ILPInvokeGetMarshalType
				(0, method, 0, &customName, &customNameLen,
				 &customCookie, &customCookieLen, type);
		if(marshalType==IL_META_MARSHAL_CUSTOM)
		{
			ILJitValue params[4];
			params[0] = returnValue;
			params[1] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)type);
			params[2] = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)method);
			params[3] = _ILJitFunctionGetThread(func);
			returnValue = jit_insn_call_native(func, "MarshalCustomToObject", MarshalCustomToObject,
								_ILJitSignature_MarshalCustomToObject, params, 4,
								JIT_CALL_NOTHROW);
		}
		else if(ILTypeIsStringClass(type))
		{
			ILJitValue args[2];
			args[0] = _ILJitFunctionGetThread(func);
			args[1] = returnValue;
			switch(marshalType)
			{
				case IL_META_MARSHAL_ANSI_STRING:
				{
					returnValue = jit_insn_call_native(func,
									    "ILStringCreate",
									    ILStringCreate,
									    _ILJitSignature_ILStringCreate,
									    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF8_STRING:
				{
					returnValue = jit_insn_call_native(func,
									    "ILStringCreateUTF8",
									    ILStringCreateUTF8,
									    _ILJitSignature_ILStringCreateUTF8,
									    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF16_STRING:
				{
					returnValue = jit_insn_call_native(func,
									    "ILStringWCreate",
									    ILStringWCreate,
									    _ILJitSignature_ILStringWCreate,
									    args, 2, JIT_CALL_NOTHROW);
				}
				break;
				default: break;
			}
		}
		else if(marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
		{
			returnValue = (ILJitValue)MarshalReturnValue(func, returnValue, type, marshalType,
									MARSHAL_FIRST_LEVEL_VALUE, 0, 0);
		}
	}

#ifdef USE_BYREF_MARSHALING
	param = 0;
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	for(current = 1; current < numParams; ++current)
#else
	for(current = 0; current < numParams; ++current)
#endif
	{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		type = ILTypeGetParam(ilSignature, current);
#else
		type = ILTypeGetParam(ilSignature, current + 1);
#endif
		type = ILTypeGetEnumType(type);

		marshalType = ILPInvokeGetMarshalType
					(pinv, method, current, &customName, &customNameLen,
						&customCookie, &customCookieLen, type);
		if((marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
		    && marshalType != IL_META_MARSHAL_CUSTOM && type != 0)
		{
			if(ILType_IsSimpleArray(type))
			{
				// Does anyone use this?
				MarshalByRefValue(func, jitParams[param], type, 
							marshalType, MARSHAL_FIRST_LEVEL_VALUE, 
							0, tempParams[current]);
			}
			else if(ILType_IsComplex(type))
			{
				typeFlag = ILType_Kind(type);
			    	if(typeFlag==IL_TYPE_COMPLEX_BYREF)
				{
					// Does anyone use this?
					MarshalByRefValue(func, jitParams[param], type,
								    marshalType, MARSHAL_FIRST_LEVEL_VALUE,
								    0, tempParams[current]);
				}
			}
		}
		++param;
	}
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	/*
	 * Emit the code to start profiling of the method if profiling
	 * is enabled
	 */
	if(jitCoder->flags & IL_CODER_FLAG_METHOD_PROFILE)
	{
		_ILJitProfileEnd(jitCoder, method);
	}
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */
#endif
	if(returnType==jit_type_void)return 0;
	else return(returnValue);
	jit_type_free(callSignature);	
}
#endif // _ILJitInlinePinvoke

static int NeedMarshalValue(ILType *type)
{
    type = ILTypeGetEnumType(type);
    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			return 0;
		}
		break;
		default: break;
	}			
    }
    else if(ILType_IsValueType(type))
    {	
	    return NeedMarshalStruct(type);
    }
    else if(ILTypeIsStringClass(type))
    {
	    return 1;
    }
    else if(ILTypeIsDelegateSubClass(type))
    {
	    return 1;
    }
    else if(type!=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    return 0;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    return 1;
		    }
		    break;
		    default:
		    {

		    }
		    break;
	    }
    }

    if(ILType_IsSimpleArray(type))
    {
	    return 1;
    }
    else if(ILType_IsArray(type))
    {
    	    return 1;
    }

    return 0;
}

static int NeedMarshalStruct(ILType *structureILType)
{
	ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
	ILField *field = 0;
	ILType *type;

	while((field = (ILField *)ILClassNextMemberByKind
			(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field))
		{
			type = ILTypeGetEnumType(ILField_Type(field));
			if(NeedMarshalValue(type))return 1;
		}
	}
	return 0;
}

static ILJitValue MarshalValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
				    unsigned int addressKind, jit_nint offset, ILJitValue outAddressValue)
{
    ILJitValue temp = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue ptr, valuePtr;
    ILType *refType;
    ILJitValue srcArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue args[3];
    ILJitValue srcElemSize;
    ILType *elemType;
    ILExecThread *thread;
    ILJitValue newElemSize, newArraySize;
    ILJitValue newArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    jit_label_t startLoop = jit_label_undefined;
    jit_label_t endLoop = jit_label_undefined;
    jit_label_t endLabel = jit_label_undefined;
    ILJitValue srcElemAddress;
    ILJitValue newElemAddress;
    ILJitValue counter;
    ILJitValue arraySize;
    type = ILTypeGetEnumType(type);
    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE && addressKind!=MARSHAL_ITEM_OF_STRUCTURE
						&& addressKind!=MARSHAL_ITEM_OF_ARRAY)
    {
	    fprintf(stderr, "addressKind has an invalid value of %d\n", addressKind);
	    return 0;
    }
    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SBYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_CHAR);				
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;			
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SINGLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_DOUBLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{			
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NUINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;			
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) temp = in;
			else temp = jit_insn_load_relative(function, in, offset, _ILJitTypedRef);
			ptr = jit_insn_address_of(function, temp);			
			valuePtr = jit_insn_load_relative(function, 
								ptr,
								offsetof(ILTypedRef, value),
								_IL_JIT_TYPE_VPTR);
			refType = ILType_Ref(type);
			switch(addressKind)
			{
				case MARSHAL_ITEM_OF_ARRAY:
				{
					return (ILJitValue)MarshalStruct(function, valuePtr, refType,
							    marshalType, outAddressValue, offset, MARSHAL_ITEM_OF_ARRAY);
				}
				break;
				case MARSHAL_ITEM_OF_STRUCTURE:
				{
					return (ILJitValue)MarshalStruct(function, valuePtr, refType,
							    marshalType, outAddressValue, offset, MARSHAL_ITEM_OF_STRUCTURE);
				}
				break;
				default:
				{
					return (ILJitValue)MarshalStruct(function, valuePtr, refType,
							    marshalType, 0, 0, MARSHAL_FIRST_LEVEL_VALUE);
				}
				break;
			}
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		default:  break;
	}			
    }
    else if(ILType_IsValueType(type))
    {
	    switch(addressKind)
	    {
		    case MARSHAL_ITEM_OF_STRUCTURE:
		    case MARSHAL_ITEM_OF_ARRAY:
		    {
			    return (ILJitValue)MarshalStruct(function, in, type, marshalType, outAddressValue, offset,
								MARSHAL_ITEM_OF_ARRAY);
		    }
		    break;
		    default:
		    {
			    return (ILJitValue)MarshalStruct(function, jit_insn_address_of(function, in), type, marshalType,
								0, 0, MARSHAL_FIRST_LEVEL_VALUE);
		    }
		    break;
	    }
    }
    else if(ILTypeIsStringClass(type))
    {
	    args[0] = _ILJitFunctionGetThread(function);
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) args[1] = in;
	    else args[1] = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    char *customName;
	    int customNameLen;
	    char *customCookie;
	    int customCookieLen;
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    ILMethod *method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		    marshalType = ILPInvokeGetMarshalType
					(0, method, 0, &customName, &customNameLen, &customCookie, &customCookieLen, type);
	    }
	    switch(marshalType)
	    {
			case IL_META_MARSHAL_ANSI_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToAnsi",
							    ILStringToAnsi,
							    _ILJitSignature_ILStringToAnsi,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF8_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToUTF8",
							    ILStringToUTF8,
							    _ILJitSignature_ILStringToUTF8,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF16_STRING:
			{
				temp = jit_insn_call_native(function,
	    						    "ILStringToUTF16",
							    ILStringToUTF16,
							    _ILJitSignature_ILStringToUTF16,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			default:
			{
				temp = args[1];
			}
			break;
	    }

	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
	    {
		    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    }
	    return temp;
    }
    else if(type!=0 && ILTypeIsDelegateSubClass(type))
    {
	    temp = jit_insn_dup(function, in);
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    }
	    ILJitValue closure = jit_insn_load_relative(function, temp, offsetof(System_Delegate, closure),
							    _IL_JIT_TYPE_VPTR);
	    jit_insn_branch_if(function, jit_insn_to_bool(function, closure), &endLabel);
	    args[0] = jit_insn_dup(function, temp);
	    args[1] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR, (jit_nint)type);
	    jit_insn_store(function, closure, jit_insn_call_native(function,
					    "ILJitDelegateGetClosure",
					    ILJitDelegateGetClosure,
					    _ILJitSignature_ILJitDelegateGetClosure,
					    args, 2, 0));
	    jit_insn_label(function, &endLabel);
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, closure);
	    return closure; 
    }
    else if(type!=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			    jit_insn_store_relative(function, outAddressValue, offset, temp);
			    return temp;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store(function, srcArray, jit_insn_load_relative(function, in, offset,
							_IL_JIT_TYPE_VPTR));
			    else jit_insn_store(function, srcArray, in);
			    elemType = ILType_Ref(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(NeedMarshalValue(elemType))
			    {
				        MarshalValue(function, jit_insn_dup(function, srcArray),
						    	    elemType, marshalType, 
							    MARSHAL_ITEM_OF_STRUCTURE,
							    0,
							    jit_insn_dup(function, srcArray));
			    }
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY) 
		    		jit_insn_store_relative(function, outAddressValue, offset, srcArray);
			    return srcArray;
		    }
		    break;
    		    default: break;
	    }

	    if(ILType_IsSimpleArray(type))
	    {
		    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
			    srcArray = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
		    else srcArray = in;
		    jit_insn_store(function, newArray, srcArray);
		    jit_insn_branch_if_not(function, jit_insn_to_bool(function, newArray),
						    &endLoop);
		    elemType = ILType_ElemType(type);
		    elemType = ILTypeGetEnumType(elemType);
		    if(!NeedMarshalValue(elemType))
		    {
			    jit_insn_store(function, newArray, srcArray);
			    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)
			    {
				    jit_insn_store(function, newArray,
								   _ILJitSArrayGetBase(function, newArray));
			    }
			    jit_insn_label(function, &endLoop);
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    	jit_insn_store_relative(function, outAddressValue, offset, newArray);
			    return newArray;
		    }
		    // The size of a simple array can be known only on run-time.
		    arraySize = _ILJitSArrayGetLength(function, srcArray);

		    thread = ILExecThreadCurrent();
		    srcElemSize = jit_value_create_nint_constant(function,
			    						_IL_JIT_TYPE_INT32,
									(jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elemType)));
		    newElemSize = jit_value_create_nint_constant(function,
									_IL_JIT_TYPE_INT32,
								    	(jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elemType)));
		    newArraySize = jit_insn_mul(function, arraySize, newElemSize);
		    args[0] = newArraySize;
		    jit_insn_store(function, newArray, jit_insn_call_native(function, "ILGCAllocAtomic",
			    						ILGCAllocAtomic,
			    				    		_ILJitSignature_malloc,
			    						args, 1, 0));
		    jit_insn_branch_if(function,
					jit_insn_eq(function, arraySize,
							    jit_value_create_nint_constant
									(function, _IL_JIT_TYPE_UINT32, 0)),
									&endLoop);
		    srcElemAddress = _ILJitSArrayGetBase(function, srcArray);
		    newElemAddress = jit_value_create(function, jit_type_int);
		    jit_insn_store(function, newElemAddress, newArray);
		    counter = jit_insn_dup(function, arraySize);
		    jit_insn_label(function, &startLoop);
		    jit_insn_store(function, counter, jit_insn_sub(function, counter,
		    								jit_value_create_nint_constant(function,
										_IL_JIT_TYPE_INT32,
										1)));
		    MarshalValue(function, srcElemAddress, elemType,
							    marshalType,
							    MARSHAL_ITEM_OF_ARRAY,
							    0,
							    newElemAddress);
		    jit_insn_store(function, srcElemAddress,
						    jit_insn_add(function,
						    srcElemAddress, srcElemSize));
		    jit_insn_store(function, newElemAddress,
						jit_insn_add(function,
						newElemAddress, newElemSize));
		    jit_insn_branch_if(function,
					    jit_insn_to_bool(function, counter),
					    &(startLoop));
		    jit_insn_label(function, &endLoop);
		    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				jit_insn_store_relative(function, outAddressValue, offset, newArray);
		    return newArray;
	    }
	    else if (ILType_IsArray(type))
	    {
		    fprintf(stderr, "Multidimential arrays are not supported for marshaling\n");
    		    return 0;
    	    }
    }
    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
    jit_insn_store_relative(function, outAddressValue, offset, temp);
    return temp;
}

static ILJitValue MarshalStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType, ILUInt32 marshalType,
					ILJitValue outAddress, jit_nint offset, unsigned int addressKind)
{
        ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
	ILExecThread *thread = ILExecThreadCurrent();
        unsigned int structSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(thread), structureILType);
	ILField *field = 0;
	ILType *type;

	switch(addressKind)
	{
		case MARSHAL_FIRST_LEVEL_VALUE:
		{
			ILJitValue sizeValue = jit_value_create_nint_constant(function, _IL_JIT_TYPE_UINT32, structSize);
			outAddress = jit_insn_call_native(function, "ILGCAlloc",
								    ILGCAlloc,
								    _ILJitSignature_malloc,
								    &sizeValue, 1, 0);
		}
		break;
		case MARSHAL_ITEM_OF_STRUCTURE:
		case MARSHAL_ITEM_OF_ARRAY:
		{	    
			
		}
		break;
		default:
		{
			fprintf(stderr, "addressKind is an invalid value of %d\n", addressKind);
			return 0;
		}
		break;
	}

	while((field = (ILField *)ILClassNextMemberByKind
			(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field))
		{
			type = ILTypeGetEnumType(ILField_Type(field));
			switch(addressKind)
			{
				case MARSHAL_FIRST_LEVEL_VALUE:
				{
					MarshalValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_STRUCTURE,
							field->offset + offset, outAddress);
				}
			        break;
				case MARSHAL_ITEM_OF_STRUCTURE:
				{
					MarshalValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_STRUCTURE,
							field->offset + offset, outAddress);
				}
				break;
				default:
				{
					MarshalValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_ARRAY,
							field->offset + offset, outAddress);
				}
				break;
			}
		}
	}
	return outAddress;
}

static ILJitValue MarshalReturnValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
					    unsigned int addressKind, jit_nint offset, ILJitValue outAddress)
{
    ILJitValue retValue;
    ILJitValue outAddressValue = outAddress;
    ILJitValue temp = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue args[3];
    ILJitValue srcArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    type = ILTypeGetEnumType(type);

    switch(addressKind)
    {    
	    case MARSHAL_FIRST_LEVEL_VALUE:
	    case MARSHAL_ITEM_OF_STRUCTURE:
	    case MARSHAL_ITEM_OF_ARRAY:
	    {
	    }
	    break;
	    default:
	    {
		    fprintf(stderr, "addressKind has an invalid %d value\n", addressKind);
		    return 0;
	    }
	    break;
    }

    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SBYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_CHAR);				
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SINGLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_DOUBLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NUINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{	
			fprintf(stderr, "Marshaling of TypedByRef as a return value is not supported\n");
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		default: break;
	}			
    }
    else if(ILType_IsValueType(type))
    {
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) 
			return (ILJitValue)MarshalReturnStruct(function, jit_insn_address_of(function, in),
								    type, marshalType, 0, 0, MARSHAL_FIRST_LEVEL_VALUE);
	    retValue = (ILJitValue)MarshalReturnStruct(function, in, type, marshalType, outAddressValue, offset,
							    MARSHAL_ITEM_OF_STRUCTURE);
	    jit_insn_store_relative(function, outAddressValue, offset, retValue);
	    return retValue;
    }
    else if(ILTypeIsStringClass(type))
    {
	    args[0] = _ILJitFunctionGetThread(function);
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) args[1] = jit_insn_dup(function, in);
	    else args[1] = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    char *customName;
	    int customNameLen;
	    char *customCookie;
	    int customCookieLen;
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    ILMethod *method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		    marshalType = ILPInvokeGetMarshalType
				    (0, method, 0, &customName, &customNameLen, &customCookie, &customCookieLen, type);
	    }
	    switch(marshalType)
	    {
			case IL_META_MARSHAL_ANSI_STRING:
			{
				temp = jit_insn_call_native(function,
						    "ILStringCreate",
						    ILStringCreate,
						    _ILJitSignature_ILStringCreate,
						    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF8_STRING:
			{
				temp = jit_insn_call_native(function,
						    "ILStringCreateUTF8",
						    ILStringCreateUTF8,
						    _ILJitSignature_ILStringCreateUTF8,
						    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF16_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringWCreate",
							    ILStringWCreate,
							    _ILJitSignature_ILStringWCreate,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			default:
			{
				temp = args[1];
			}
			break;
	    }
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    return temp;
    }
    else if(ILTypeIsDelegateSubClass(type))
    {
	    fprintf(stderr, "Marshaling of a delegate as a return value is not supported\n");
	    return 0;
    }
    else if(type!=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)return in;
			    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			    jit_insn_store_relative(function, outAddressValue, offset, temp);
			    return temp;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store(function, srcArray, jit_insn_load_relative(function, in, offset,
												_IL_JIT_TYPE_VPTR));
			    else jit_insn_store(function, srcArray, in);
			    ILType *elemType = ILType_Ref(type);
			    if(NeedMarshalValue(elemType))
			    {
				    MarshalReturnValue(function, jit_insn_dup(function, srcArray),
								    elemType, marshalType,
								    MARSHAL_ITEM_OF_STRUCTURE,
								    0,
								    jit_insn_dup(function, srcArray));
			    }
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store_relative(function, outAddressValue, offset, srcArray);
			    return srcArray;
		    }
		    break;
		    default: break;
	    }

	    if(ILType_IsSimpleArray(type))
	    {
			// The size of a simple array can be known only on run-time 
			// and we cannot use the same return memory as it is allocated in C code.
			// However, it is easy to be implemented using the MarshalValue case.
			fprintf(stderr, "Simple arrays not supported for marshaling as a return value");
			return 0;			
	    }
	    else if(ILType_IsArray(type))
	    {
		    fprintf(stderr, "Multidimential arrays are not supported for marshaling as a return value\n");
		    return 0;
	    }

    }

    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
    jit_insn_store_relative(function, outAddressValue, offset, temp);
    return temp;
}

static ILJitValue MarshalReturnStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
					    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
					    unsigned int addressKind)
{
    ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
    ILExecThread *thread = ILExecThreadCurrent();
    unsigned int structSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(thread), structureILType);
    ILField *field = 0;
    ILJitValue newStruct = 0;
    ILType *type;
    ILJitType structType;	

    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)
    {
	    // A returned structure should be a libjit value, so we create one.
	    // Although we could use ILGCAllocAtomic with structSize - it should
	    // work.
	    structType = jit_type_create_struct(0, 0, 0);
	    jit_type_set_size_and_alignment(structType, structSize, 0);
	    newStruct = jit_value_create(function, structType);
	    outAddress = jit_insn_address_of(function, newStruct);
    }

    while((field = (ILField *)ILClassNextMemberByKind
    		(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
    {
	    if(!ILField_IsStatic(field))
	    {
		    type = ILTypeGetEnumType(ILField_Type(field));
		    MarshalReturnValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_STRUCTURE,
					    offset + field->offset, outAddress);
	    }
    }
    return newStruct;
}

#ifdef USE_BYREF_MARSHALING
static void MarshalByRefValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
				    unsigned int addressKind, jit_nint offset, ILJitValue outAddress)
{
    ILJitValue outAddressValue = outAddress;
    ILJitValue temp = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue args[3];
    ILJitValue srcArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue srcElemSize;
    ILType *elemType;
    ILExecThread *thread;
    ILJitValue newElemSize;
    ILJitValue newArray;
    jit_label_t startLoop = jit_label_undefined;
    jit_label_t endLoop = jit_label_undefined;
    ILJitValue srcElemAddress;
    ILJitValue newElemAddress;
    ILJitValue counter;
    ILJitValue arraySize;
    type = ILTypeGetEnumType(type);
    if(!NeedMarshalValue(type)) return;

    switch(addressKind)
    {
	    case MARSHAL_FIRST_LEVEL_VALUE:
	    case MARSHAL_ITEM_OF_STRUCTURE:
	    case MARSHAL_ITEM_OF_ARRAY:
	    {
		    outAddressValue = outAddress;
	    }
	    break;
	    default:
	    {
		    fprintf(stderr, "addressKind has an invalid %d value\n", addressKind);
		    return;
	    }
	    break;
    }

    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{	
			fprintf(stderr, "Marshaling of TypedByRef as a byref is not supported\n");
			return;
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			return;
		}
		break;
		default: break;
	}			
    }
    else if(ILType_IsValueType(type))
    {
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    MarshalByRefStruct(function, jit_insn_address_of(function, in), type, marshalType, outAddressValue, 0,
					    MARSHAL_ITEM_OF_STRUCTURE);
		    return;
	    }
	    MarshalByRefStruct(function, in, type, marshalType, outAddressValue, offset, MARSHAL_ITEM_OF_STRUCTURE);
	    return;
    }
    else if(ILTypeIsStringClass(type))
    {
	    args[0] = _ILJitFunctionGetThread(function);
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) args[1] = in;
	    else args[1] = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    char *customName;
	    int customNameLen;
	    char *customCookie;
	    int customCookieLen;
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    ILMethod *method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		    marshalType = ILPInvokeGetMarshalType
				    (0, method, 0, &customName, &customNameLen, &customCookie, &customCookieLen, type);
	    }
	    switch(marshalType)
	    {
			case IL_META_MARSHAL_ANSI_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringCreate",
							    ILStringCreate,
							    _ILJitSignature_ILStringCreate,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF8_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringCreateUTF8",
							    ILStringCreateUTF8,
							    _ILJitSignature_ILStringCreateUTF8,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF16_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringWCreate",
							    ILStringWCreate,
							    _ILJitSignature_ILStringWCreate,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			default:
			{
				temp = args[1];
			}
			break;
	    }
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    return;
    }
    else if(ILTypeIsDelegateSubClass(type))
    {
	    return;
    }
    else if(type !=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    return;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
			    {
				    jit_insn_store(function, srcArray, jit_insn_load_relative(function, in, offset,
									    _IL_JIT_TYPE_VPTR));
			    }
			    else jit_insn_store(function, srcArray, in);
			    ILType *elemType = ILType_Ref(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(NeedMarshalValue(elemType))
			    {
				    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    {
				    
					    MarshalByRefValue(function, jit_insn_dup(function, srcArray),
										    elemType, marshalType,
										    MARSHAL_ITEM_OF_STRUCTURE,
										    0,
										    jit_insn_load_relative(function,
											outAddressValue, offset,
											_IL_JIT_TYPE_VPTR));
				    }
				    else MarshalByRefValue(function, jit_insn_dup(function, srcArray),
										    elemType, marshalType,
										    MARSHAL_ITEM_OF_STRUCTURE,
										    0,
										    srcArray);
			    }
			    return;
		    }
		    break;
		    default: break;
	    }

	    if(ILType_IsSimpleArray(type))
	    {
			    srcArray = in;
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
			    {
				    srcArray = jit_insn_load_relative(function, srcArray, offset, _IL_JIT_TYPE_VPTR);
			    }
			    jit_insn_branch_if_not(function, jit_insn_to_bool(function, srcArray),
							    &endLoop);
			    jit_insn_branch_if_not(function, jit_insn_to_bool(function, outAddressValue),
							    &endLoop);
			    elemType = ILType_ElemType(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(!NeedMarshalValue(elemType))
			    {
				    return;
			    }
			    // The size of a simple array can be known only on run-time.
			    arraySize = _ILJitSArrayGetLength(function, outAddressValue);

			    thread = ILExecThreadCurrent();
			    srcElemSize = jit_value_create_nint_constant(function,
					    					_IL_JIT_TYPE_INT32,
										(jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elemType)));
			    newElemSize = jit_value_create_nint_constant(function,
										_IL_JIT_TYPE_INT32,
									    	(jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elemType)));
			    newArray = _ILJitSArrayGetBase(function, outAddressValue);
			    jit_insn_branch_if(function,
						jit_insn_eq(function, arraySize,
									jit_value_create_nint_constant
										(function, _IL_JIT_TYPE_UINT32, 0)),
										&endLoop);
			    srcElemAddress = jit_insn_dup(function, srcArray);
			    newElemAddress = jit_insn_dup(function, newArray);
			    counter = jit_insn_dup(function, arraySize);
			    jit_insn_label(function, &startLoop);
			    jit_insn_store(function, counter, jit_insn_sub(function,
										counter,
		    								jit_value_create_nint_constant(function,
											_IL_JIT_TYPE_INT32,
											1)));
			    MarshalByRefValue(function, srcElemAddress,
								elemType, marshalType,
								MARSHAL_ITEM_OF_ARRAY,
								0,
								newElemAddress);
			    jit_insn_store(function, srcElemAddress,
							    jit_insn_add(function,
							    srcElemAddress, srcElemSize));
			    jit_insn_store(function, newElemAddress,
							jit_insn_add(function,
							newElemAddress, newElemSize));
			    jit_insn_branch_if(function,
					        jit_insn_to_bool(function, counter),
					        &(startLoop));
			    jit_insn_label(function, &endLoop);
			    return;
	    }
	    else if(ILType_IsArray(type))
	    {
		    fprintf(stderr, "Multidimential arrays are not supported for marshaling as a return value\n");
		    return;
	    }
    }
    // If we handle about all the cases above then we should not reach here,
    // however we make the compiler happy.
    return;
}

static void MarshalByRefStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType, ILUInt32 marshalType,
					ILJitValue outAddress, jit_nint offset, unsigned int addressKind)
{
    ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
    ILField *field = 0;
    ILType *type;

    while((field = (ILField *)ILClassNextMemberByKind
    		(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
    {
	    if(!ILField_IsStatic(field))
	    {
		    type = ILTypeGetEnumType(ILField_Type(field));
		    MarshalByRefValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_STRUCTURE,
					field->offset + offset, outAddress);
	    }
    }
    return;
}
#endif // USE_BYREF_MARSHALING

/*
 * Emit the code to call a delegate.
 * args contains only the arguments passed to the method (excluding the this pointer).
 */
static ILJitValue __ILJitDelegateInvokeCodeGen(ILJitFunction func,
											  ILJitFunction invokeFunc,
											  ILJitValue delegate,
											  ILJitValue *args,
											  ILUInt32 argCount)
{
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
	invokeArgs[0] = jit_insn_call_native(func, "ILExecThreadCurrent", ILExecThreadCurrent,
								_ILJitSignature_ILExecThreadCurrent,
								0, 0, JIT_CALL_NOTHROW);
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
	types[0] = jit_type_void_ptr;
	invokeArgs[0] = jit_insn_call_native(func, "ILExecThreadCurrent", ILExecThreadCurrent,
									    _ILJitSignature_ILExecThreadCurrent,
									    0, 0, JIT_CALL_NOTHROW);
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

void *ILJitDelegateGetClosure(ILObject *del, ILType *delType)
{
	int current;
	ILJitType newSignature;
	ILType *valueType;	
	ILJitType delInvokeSignature;
	ILJitFunction function;
	ILMethod *method;
	ILClass *info;
	ILClassPrivate *classPrivate;
	ILJITCoder *jitCoder;
	ILJitFunction delFunction;
	ILJitValue returnValue = 0;
	ILType *type;
	method = ILTypeGetDelegateMethod(delType);
	if(!method)
	{
		method = ((System_Delegate*)del)->methodInfo;
	}
	ILType *invokeSignature = ILMethod_Signature(method);
	int paramCount = ILTypeNumParams(invokeSignature);
	ILType *returnILType = ILTypeGetReturn(invokeSignature);
	ILJitType returnType = _ILJitGetReturnType(returnILType, ILExecThreadGetProcess(ILExecThreadCurrent()));
	ILJitType jitParamTypes[paramCount];
	ILJitType jitInvokeTypes[paramCount + 2];
	ILJitValue jitParams[paramCount + 2];
	ILJitValue tempParams[paramCount + 2];
	ILUInt32 marshalType;
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;
    	for(current = 0; current < paramCount; ++current)
	{
		valueType = ILTypeGetParam(invokeSignature, current + 1);
		jitParamTypes[current] = _ILJitGetArgType(valueType, ILExecThreadGetProcess(ILExecThreadCurrent()));
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		jitInvokeTypes[current + 2] = jitParamTypes[current];
#else
		jitInvokeTypes[current + 1] = jitParamTypes[current];
#endif
	}
	newSignature = jit_type_create_signature(jit_abi_cdecl,
						    returnType,
						    jitParamTypes,
						    paramCount, 1);
	jitInvokeTypes[0] = jit_type_void_ptr;
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	jitInvokeTypes[1] = jit_type_void_ptr;
#endif
	delInvokeSignature = jit_type_create_signature(IL_JIT_CALLCONV_DEFAULT,
							returnType,
							jitInvokeTypes,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
							paramCount + 2, 1);
#else
							paramCount + 1, 1);
#endif
	info = ILMethod_Owner(method);
	classPrivate = (ILClassPrivate *)info->userData;
	jitCoder = (ILJITCoder *)(classPrivate->process->coder);

	function = jit_function_create(jitCoder->context, newSignature);

	jit_function_set_meta(function, IL_JIT_META_METHOD, method, 0, 0);

	for(current = 0; current < paramCount; ++current)
	{
		tempParams[current] = jit_value_get_param(function, current);
		type = ILTypeGetParam(invokeSignature, current + 1);
		marshalType = ILPInvokeGetMarshalType(0, method, current + 1, &customName, &customNameLen,
									      &customCookie, &customCookieLen,
									      type);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		jitParams[current + 2] = jit_value_get_param(function, current);
#else
		jitParams[current + 1] = jit_value_get_param(function, current);
#endif
		if(marshalType==IL_META_MARSHAL_CUSTOM)
		{
			ILJitValue params[4];
			params[0] = jit_value_get_param(function, current);
			params[1] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR, (jit_nint)type);
			params[2] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR, (jit_nint)method);
			params[3] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
								    _ILJitSignature_ILExecThreadCurrent,
								    0, 0, JIT_CALL_NOTHROW);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
			jitParams[current + 2] =
#else
			jitParams[current + 1] =
#endif
						    jit_insn_call_native(function, "MarshalCustomToObject",
									    MarshalCustomToObject,
									    _ILJitSignature_MarshalCustomToObject,
									    params, 4, JIT_CALL_NOTHROW);
		}
		else if(ILTypeIsStringClass(type))
		{
			ILJitValue args[2];
			args[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
								    _ILJitSignature_ILExecThreadCurrent,
								    0, 0, JIT_CALL_NOTHROW);
			args[1] = jit_value_get_param(function, current);
			switch(marshalType)
			{
				case IL_META_MARSHAL_ANSI_STRING:
				{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
					jitParams[current + 2] =
#else
					jitParams[current + 1] =
#endif
							jit_insn_call_native(function,
										"ILStringCreate",
										ILStringCreate,
										_ILJitSignature_ILStringCreate,
										args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF8_STRING:
				{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
					jitParams[current + 2] =
#else
					jitParams[current + 1] =
#endif
							jit_insn_call_native(function,
									        "ILStringCreateUTF8",
										ILStringCreateUTF8,
										_ILJitSignature_ILStringCreateUTF8,
										args, 2, JIT_CALL_NOTHROW);
				}
				break;
				case IL_META_MARSHAL_UTF16_STRING:
				{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
					jitParams[current + 2] =
#else
					jitParams[current + 1] =
#endif
							jit_insn_call_native(function,
										"ILStringWCreate",
										ILStringWCreate,
										_ILJitSignature_ILStringWCreate,
										args, 2, JIT_CALL_NOTHROW);
				}
				break;
				default: break;
			}
		}
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		else if(marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
#else
		else if(marshalType != IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
#endif
		{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
			jitParams[current + 2] =
#else
			jitParams[current + 1] =
#endif
						    (ILJitValue)MarshalDelegateValue(function,
											jit_value_get_param(function, 
														current),
											type,
											marshalType,
											MARSHAL_FIRST_LEVEL_VALUE, 0, 0);
		}
	}
	marshalType = ILPInvokeGetMarshalType
			(0, method, 0, &customName, &customNameLen,
			 &customCookie, &customCookieLen, returnILType);
	if(!ILTypeIsDelegate(delType))
        {
		delFunction = jit_function_create(jitCoder->context, delInvokeSignature);
		returnValue = __ILJitDelegateInvokeCodeGen(function, delFunction,
									jit_value_create_nint_constant(function,
									_IL_JIT_TYPE_VPTR, (jit_nint)del),
#ifdef IL_JIT_THREAD_IN_SIGNATURE
									&jitParams[2], paramCount);
#else
									&jitParams[1], paramCount);
#endif
		if(returnType!=jit_type_void)
		{
			if(marshalType==IL_META_MARSHAL_CUSTOM)
			{
				ILJitValue params[4];
				params[0] = returnValue;
				params[1] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR,
											(jit_nint)returnILType);
				params[2] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR, (jit_nint)method);
				params[3] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
											_ILJitSignature_ILExecThreadCurrent,
											0, 0, JIT_CALL_NOTHROW);
				returnValue = jit_insn_call_native(function, "MarshalObjectToCustom",
										MarshalObjectToCustom,
										_ILJitSignature_MarshalObjectToCustom,
										params, 4, JIT_CALL_NOTHROW);
			}
			else if(ILTypeIsStringClass(returnILType))
			{
				ILJitValue args[2];
				args[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
									    _ILJitSignature_ILExecThreadCurrent,
									    0, 0, JIT_CALL_NOTHROW);
				args[1] = returnValue;
				switch(marshalType)
				{
					case IL_META_MARSHAL_ANSI_STRING:
					{
						returnValue = jit_insn_call_native(function,
							    			    "ILStringToAnsi",
										    ILStringToAnsi,
										    _ILJitSignature_ILStringToAnsi,
										    args, 2, JIT_CALL_NOTHROW);
					}
					break;
					case IL_META_MARSHAL_UTF8_STRING:
					{
						returnValue = jit_insn_call_native(function,
										    "ILStringToUTF8",
										    ILStringToUTF8,
										    _ILJitSignature_ILStringToUTF8,
										    args, 2, JIT_CALL_NOTHROW);
					}
					break;
					case IL_META_MARSHAL_UTF16_STRING:
					{
						returnValue = jit_insn_call_native(function,
	    									    "ILStringToUTF16",
										    ILStringToUTF16,
										    _ILJitSignature_ILStringToUTF16,
										    args, 2, JIT_CALL_NOTHROW);
					}
					break;
					default: break;
	    			}
			}
			else if(marshalType!=IL_META_MARSHAL_DIRECT || NeedMarshalValue(returnILType))
			{
				 returnValue = (ILJitValue)MarshalDelegateReturnValue(function, returnValue,
											returnILType, marshalType,
											MARSHAL_FIRST_LEVEL_VALUE, 0,
											0);
			}
		}
	}
	else
	{
		delFunction = ILJitFunctionFromILMethod(method);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		jitParams[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
										_ILJitSignature_ILExecThreadCurrent,
										0, 0, JIT_CALL_NOTHROW);
		jitParams[1] = jit_value_create_nint_constant(function, jit_type_void_ptr, (jit_nint)del);
#else
		jitParams[0] = jit_value_create_nint_constant(function, jit_type_void_ptr, (jit_nint)del);
#endif
		if(returnType==jit_type_void) jit_insn_call(function, "callDelegatePinvokeMethod",
							    delFunction, delInvokeSignature,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
							    jitParams, paramCount + 2,
#else
							    jitParams, paramCount + 1,
#endif
							    0);
		else
		{
			returnValue = jit_insn_call(function, "callDelegatePinvokeMethod",
						    delFunction, delInvokeSignature,
#ifdef IL_JIT_THREAD_IN_SIGNATURE
						    jitParams, paramCount + 2,
#else
						    jitParams, paramCount + 1,
#endif
						    0);
			if(marshalType==IL_META_MARSHAL_CUSTOM)
			{
				ILJitValue params[4];
				params[0] = returnValue;
				params[1] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR,
											(jit_nint)returnILType);
				params[2] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR, (jit_nint)method);
				params[3] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
									    _ILJitSignature_ILExecThreadCurrent,
									    0, 0, JIT_CALL_NOTHROW);
				returnValue = jit_insn_call_native(function, "MarshalObjectToCustom", MarshalObjectToCustom,
									    _ILJitSignature_MarshalObjectToCustom, params,
									    4, JIT_CALL_NOTHROW);
			}
			else if(ILTypeIsStringClass(returnILType))
			{
				ILJitValue args[2];
				args[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
									    _ILJitSignature_ILExecThreadCurrent,
									    0, 0, JIT_CALL_NOTHROW);
				args[1] = returnValue;
				switch(marshalType)
				{
					case IL_META_MARSHAL_ANSI_STRING:
					{
						returnValue = jit_insn_call_native(function,
							    			    "ILStringToAnsi",
										    ILStringToAnsi,
										    _ILJitSignature_ILStringToAnsi,
										    args, 2, JIT_CALL_NOTHROW);
					}
					break;
					case IL_META_MARSHAL_UTF8_STRING:
					{
						returnValue = jit_insn_call_native(function,
										    "ILStringToUTF8",
										    ILStringToUTF8,
										    _ILJitSignature_ILStringToUTF8,
										    args, 2, JIT_CALL_NOTHROW);
					}
					break;
					case IL_META_MARSHAL_UTF16_STRING:
					{
						returnValue = jit_insn_call_native(function,
	    									    "ILStringToUTF16",
										    ILStringToUTF16,
										    _ILJitSignature_ILStringToUTF16,
										    args, 2, JIT_CALL_NOTHROW);
					}
					break;
					default: break;
	    			}
			}
			else if(marshalType!=IL_META_MARSHAL_DIRECT || NeedMarshalValue(returnILType))
			{
				returnValue = (ILJitValue)MarshalDelegateReturnValue(function, returnValue,
											returnILType, marshalType,
											MARSHAL_FIRST_LEVEL_VALUE, 0,
											0);
			}
		}	
	}
#ifdef USE_BYREF_MARSHALING
	for(current = 0; current < paramCount; ++current)
	{
#ifdef IL_JIT_THREAD_IN_SIGNATURE
		newValue = jitParams[current + 2];
#else
		newValue = jitParams[current + 1];
#endif
		type = ILTypeGetParam(invokeSignature, current + 1);
		type = ILTypeGetEnumType(type);
		marshalType = ILPInvokeGetMarshalType
					(0, method, current, &customName, &customNameLen,
							&customCookie, &customCookieLen, type);
		if((type!=IL_META_MARSHAL_DIRECT || NeedMarshalValue(type))
			&& marshalType!=IL_META_MARSHAL_CUSTOM && type!=0)
		{
			if(ILType_IsSimpleArray(type))
			{
				// Does anyone use this?
				MarshalDelegateByRefValue(function, newValue, type, marshalType,
				 				    MARSHAL_FIRST_LEVEL_VALUE, 0,
			 					    tempParams[current]);
			}
			else if(ILType_IsComplex(type))
			{
				typeFlag = ILType_Kind(type);
				if(typeFlag==IL_TYPE_COMPLEX_BYREF || typeFlag==IL_TYPE_COMPLEX_PTR)
				{
					// Does anyone use this?
					MarshalDelegateByRefValue(function, newValue, type, marshalType, 
									    MARSHAL_FIRST_LEVEL_VALUE, 0, 
									    tempParams[current]);
				}
			}
		}
	}
#endif
	if(returnType==jit_type_void)jit_insn_return(function, 0);
	else jit_insn_return(function, returnValue);
	jit_function_compile(function);
	((System_Delegate*)del)->closure = jit_function_to_closure(function);
	return ((System_Delegate*)del)->closure;
}

static ILJitValue MarshalDelegateValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
					    unsigned int addressKind, jit_nint offset, ILJitValue outAddress)
{
    ILJitValue retValue;
    ILJitValue outAddressValue = outAddress;
    ILJitValue temp = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue args[3];
    ILJitValue srcArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue srcElemSize;
    ILType *elemType;
    ILJitValue newElemSize, newArraySize;
    ILJitValue newArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    jit_label_t startLoop = jit_label_undefined;
    jit_label_t endLoop = jit_label_undefined;
    ILJitValue srcElemAddress;
    ILJitValue newElemAddress;
    ILJitValue counter;
    ILJitValue arraySize;
    type = ILTypeGetEnumType(type);

    if(marshalType == IL_META_MARSHAL_CUSTOM)
    {
	    fprintf(stderr, "Custom marshaling not supported in delegate value\n");
	    return 0;
    }

    switch(addressKind)
    {    
	    case MARSHAL_FIRST_LEVEL_VALUE:
	    case MARSHAL_ITEM_OF_STRUCTURE:
	    case MARSHAL_ITEM_OF_ARRAY:
	    {
		    outAddressValue = outAddress;
	    }
	    break;
	    default:
	    {
		    fprintf(stderr, "addressKind has an invalid %d value\n", addressKind);
		    return 0;
	    }
	    break;
    }
    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SBYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_CHAR);				
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SINGLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_DOUBLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NUINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{	
			fprintf(stderr, "Marshaling of TypedByRef as a value is not supported");
			return 0;
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		default: break;
	}			
    }
    else if(ILType_IsValueType(type))
    {	
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) 
		    return (ILJitValue)MarshalDelegateStruct(function, jit_insn_address_of(function, in),
								type, marshalType, 0, 0, MARSHAL_FIRST_LEVEL_VALUE);
	    retValue = (ILJitValue)MarshalDelegateStruct(function, in, type, marshalType, outAddressValue, offset,
							    MARSHAL_ITEM_OF_STRUCTURE);
	    jit_insn_store_relative(function, outAddressValue, offset, retValue);
	    return retValue;
    }
    else if(ILTypeIsStringClass(type))
    {
	    args[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
							_ILJitSignature_ILExecThreadCurrent,
							0, 0, JIT_CALL_NOTHROW);
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) args[1] = in;
	    else args[1] = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    char *customName;
	    int customNameLen;
	    char *customCookie;
	    int customCookieLen;
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    ILMethod *method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		    marshalType = ILPInvokeGetMarshalType
					(0, method, 0, &customName, &customNameLen, &customCookie, &customCookieLen, type);
	    }
	    switch(marshalType)
	    {
			case IL_META_MARSHAL_ANSI_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringCreate",
							    ILStringCreate,
							    _ILJitSignature_ILStringCreate,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF8_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringCreateUTF8",
							    ILStringCreateUTF8,
							    _ILJitSignature_ILStringCreateUTF8,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF16_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringWCreate",
							    ILStringWCreate,
							    _ILJitSignature_ILStringWCreate,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			default:
			{
				temp = args[1];
			}
			break;
	    }
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    return temp;
    }
    else if(ILTypeIsDelegateSubClass(type))
    {
	    fprintf(stderr, "Marshaling in a delegate of a delegate as a value is not supported\n");
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
	    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    return temp;
    }
    else if(type!=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)return in;
			    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			    jit_insn_store_relative(function, outAddressValue, offset, temp);
			    return temp;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store(function, srcArray, jit_insn_load_relative(function, in, offset,
												_IL_JIT_TYPE_VPTR));
			    else jit_insn_store(function, srcArray, in);
			    ILType *elemType = ILType_Ref(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(NeedMarshalValue(elemType))
			    {
				    jit_insn_store(function, temp, MarshalDelegateStruct(function,
											    jit_insn_dup(function, srcArray),
											    elemType,
											    marshalType,
											    0,
											    0,
											    MARSHAL_FIRST_LEVEL_VALUE));
			    }
			    else jit_insn_store(function, temp, srcArray);
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store_relative(function, outAddressValue, offset, srcArray);
			    return srcArray;
		    }
		    break;
		    default: break;
	    }

	    if(ILType_IsSimpleArray(type))
	    {
				ILExecThread *thread = ILExecThreadCurrent();

			    srcArray = in;
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    srcArray = jit_insn_load_relative(function, srcArray, offset, _IL_JIT_TYPE_VPTR);
			    elemType = ILType_ElemType(type);
			    elemType = ILTypeGetEnumType(elemType);
			    jit_insn_store(function, newArray, srcArray);
			    jit_insn_branch_if_not(function, jit_insn_to_bool(function, srcArray),
							    &endLoop);
   			    // The size of a simple array can be known only on run-time.
			    arraySize = _ILJitSArrayGetLength(function, srcArray);

			    srcElemSize = jit_value_create_nint_constant(function,
				    					    _IL_JIT_TYPE_INT32,
									    (jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread),
												    elemType)));
			    newElemSize = jit_value_create_nint_constant(function,
									    _IL_JIT_TYPE_INT32,
									    (jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread),
												    elemType)));
			    newArraySize = jit_insn_mul(function, arraySize, newElemSize);
			    newArraySize = jit_insn_add_relative(function, newArraySize,
													 (jit_nint)_IL_JIT_SARRAY_HEADERSIZE);
			    args[0] = newArraySize;
			    newArray = jit_insn_call_native(function, "_ILGCAllocAtomic",
			    		    				ILGCAllocAtomic,
			    		    				_ILJitSignature_malloc,
									args, 1, 0);
			    jit_insn_store_relative(function, newArray,
										offsetof(SArrayHeader, length),
										arraySize);
			    newArray = _ILJitSArrayGetBase(function, newArray);
			    jit_insn_branch_if(function,
						jit_insn_eq(function, arraySize,
									jit_value_create_nint_constant
										(function, _IL_JIT_TYPE_UINT32, 0)),
										&endLoop);
			    srcElemAddress = jit_insn_add(function,
								srcArray,
								jit_value_create_nint_constant(function,
												_IL_JIT_TYPE_INT32,
												(jit_nint)_IL_JIT_SARRAY_HEADERSIZE));
			    newElemAddress = jit_insn_dup(function, newArray);
			    counter = jit_insn_dup(function, arraySize);
			    jit_insn_label(function, &startLoop);
			    jit_insn_store(function, counter, jit_insn_sub(function,
									    counter,
		    							    jit_value_create_nint_constant(function,
											_IL_JIT_TYPE_INT32,
											1)));
			    MarshalDelegateValue(function, srcElemAddress,
							elemType, marshalType,
							MARSHAL_ITEM_OF_ARRAY,
							0,
							newElemAddress);
			    jit_insn_store(function, srcElemAddress,
							    jit_insn_add(function,
							    srcElemAddress, srcElemSize));
			    jit_insn_store(function, newElemAddress,
							jit_insn_add(function,
							newElemAddress, newElemSize));
			    jit_insn_branch_if(function,
					        jit_insn_to_bool(function, counter),
					        &(startLoop));
			    jit_insn_label(function, &endLoop);
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
					jit_insn_store_relative(function, outAddressValue, offset, newArray);
			    return newArray;
	    }
	    else if(ILType_IsArray(type))
	    {
		    fprintf(stderr, "Multidimential arrays are not supported for marshaling in delegates as a value\n");
		    return 0;
	    }

    }

    // Should not reach here if every case is handled above,
    // however we make the compiler happy.
    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
    jit_insn_store_relative(function, outAddressValue, offset, temp);
    return temp;
}

ILJitValue MarshalDelegateStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
				    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset, unsigned int addressKind)
{
	ILExecThread *thread = ILExecThreadCurrent();
    ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
    unsigned int structSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(thread), structureILType);
    ILField *field = 0;
    ILType *type;
    ILJitType structType;
    ILJitValue newStruct = 0;
    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)
    {
	    // A structure passed to the delegate need to be a libjit value,
	    // although we could use ILGCAllocAtomic with the structSize.
	    structType = jit_type_create_struct(0, 0, 0);
	    jit_type_set_size_and_alignment(structType, structSize, 0);
	    newStruct = jit_value_create(function, structType);
	    outAddress = jit_insn_address_of(function, newStruct);
    }
    while((field = (ILField *)ILClassNextMemberByKind
    		(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
    {
	    if(!ILField_IsStatic(field))
	    {
		    type = ILTypeGetEnumType(ILField_Type(field));
		    MarshalDelegateValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_STRUCTURE,
					    field->offset, outAddress);
	    }
    }
    return newStruct;
}

static ILJitValue MarshalDelegateReturnValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
						unsigned int addressKind, jit_nint offset, ILJitValue outAddress)
{
    ILJitValue outAddressValue = outAddress;
    ILJitValue temp = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue ptr, valuePtr;
    ILType *refType;
    ILJitValue args[3];
    ILType *elemType;
    jit_label_t endLabel = jit_label_undefined;
    ILJitValue srcArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    type = ILTypeGetEnumType(type);    
    if(marshalType == IL_META_MARSHAL_CUSTOM)
    {
	    fprintf(stderr, "Custom marshaling not supported in return value\n");
	    return 0;
    }

    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE && addressKind!=MARSHAL_ITEM_OF_STRUCTURE
					    && addressKind!=MARSHAL_ITEM_OF_ARRAY)
    {
	    fprintf(stderr, "addressKind has an invalid value of %d\n", addressKind);
	    return 0;
    }

    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SBYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_BYTE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT16);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_CHAR);				
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT32);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_INT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_UINT64);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;			
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_SINGLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_DOUBLE);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{			
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_NUINT);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;			
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) temp = in;
			else temp = jit_insn_load_relative(function, in, offset, _ILJitTypedRef);
			ptr = jit_insn_address_of(function, temp);			
			valuePtr = jit_insn_load_relative(function, 
								ptr,
								offsetof(ILTypedRef, value),
								_IL_JIT_TYPE_VPTR);
			refType = ILType_Ref(type);
			switch(addressKind)
			{
				case MARSHAL_ITEM_OF_ARRAY:
				{
					return (ILJitValue)MarshalDelegateReturnStruct(function, valuePtr, refType,
							    marshalType, outAddressValue, offset, MARSHAL_ITEM_OF_ARRAY);
				}
				break;
				case MARSHAL_ITEM_OF_STRUCTURE:
				{
					return (ILJitValue)MarshalDelegateReturnStruct(function, valuePtr, refType,
							    marshalType, outAddressValue, offset, MARSHAL_ITEM_OF_STRUCTURE);
				}
				break;
				default:
				{
					return (ILJitValue)MarshalDelegateReturnStruct(function, valuePtr, refType,
							    marshalType, 0, 0, MARSHAL_FIRST_LEVEL_VALUE);
				}
				break;
			}
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
			temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			jit_insn_store_relative(function, outAddressValue, offset, temp);
			return temp;
		}
		break;
		default:  break;
	}			
    }
    else if(ILType_IsValueType(type))
    {
	    switch(addressKind)
	    {
		    case MARSHAL_ITEM_OF_STRUCTURE:
		    case MARSHAL_ITEM_OF_ARRAY:
		    {
			    return (ILJitValue)MarshalDelegateReturnStruct(function, in, type, marshalType, outAddressValue,
									    offset, MARSHAL_ITEM_OF_ARRAY);
		    }
		    break;
		    default:
		    {
			    return (ILJitValue)MarshalDelegateReturnStruct(function, jit_insn_address_of(function, in),
							type, marshalType, outAddressValue, 0, MARSHAL_FIRST_LEVEL_VALUE);
		    }
		    break;
	    }
    }
    else if(ILTypeIsStringClass(type))
    {
	    args[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
								_ILJitSignature_ILExecThreadCurrent,
								0, 0, JIT_CALL_NOTHROW);
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) args[1] = in;
	    else args[1] = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    char *customName;
	    int customNameLen;
	    char *customCookie;
	    int customCookieLen;
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    ILMethod *method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		    marshalType = ILPInvokeGetMarshalType
			    	    (0, method, 0, &customName, &customNameLen, &customCookie, &customCookieLen, type);
	    }
	    switch(marshalType)
	    {
			case IL_META_MARSHAL_ANSI_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToAnsi",
							    ILStringToAnsi,
							    _ILJitSignature_ILStringToAnsi,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF8_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToUTF8",
							    ILStringToUTF8,
							    _ILJitSignature_ILStringToUTF8,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF16_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToUTF16",
							    ILStringToUTF16,
							    _ILJitSignature_ILStringToUTF16,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			default:
			{
				temp = args[1];
			}
			break;
	    }
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    return temp;
    }
    else if(ILTypeIsDelegateSubClass(type))
    {
	    temp = jit_insn_dup(function, in);
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE) temp = jit_insn_load_relative(function, in, offset,
											_IL_JIT_TYPE_VPTR);
	    ILJitValue closure = jit_insn_load_relative(function, temp, offsetof(System_Delegate, closure),
											_IL_JIT_TYPE_VPTR);
	    jit_insn_branch_if(function, jit_insn_to_bool(function, closure), &endLabel);
	    args[0] = jit_insn_dup(function, temp);
	    args[1] = jit_value_create_nint_constant(function, _IL_JIT_TYPE_VPTR, (jit_nint)type);
	    jit_insn_store(function, closure, jit_insn_call_native(function,
					    "ILJitDelegateGetClosure",
					    ILJitDelegateGetClosure,
					    _ILJitSignature_ILJitDelegateGetClosure,
					    args, 2, JIT_CALL_NOTHROW));
	    jit_insn_label(function, &endLabel);
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, closure);
	    return(closure);

    }
    else if(type!=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)return in;
			    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
			    jit_insn_store_relative(function, outAddressValue, offset, temp);
			    return temp;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store(function, srcArray, jit_insn_load_relative(function, srcArray,
							offset, _IL_JIT_TYPE_VPTR));
			    else jit_insn_store(function, srcArray, in);
			    elemType = ILType_Ref(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(NeedMarshalValue(elemType))
			    {
				    MarshalDelegateReturnValue(function, jit_insn_dup(function, srcArray),
								elemType, marshalType,
								MARSHAL_ITEM_OF_ARRAY,
								0,
								jit_insn_dup(function, srcArray));
			    }
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store_relative(function, outAddressValue, offset, srcArray);
			    return srcArray;
		    }
		    break;
		    default: break;
		    }

		    if(ILType_IsSimpleArray(type))
		    {
			    // The size of a simple array can be known only on run-time but
			    // we should note that we cannot use the same memory as it is used in the C code.
			    fprintf(stderr, "Simple arrays are not supported for marshaling as a return value\n");
			    return 0;
		    }
		    else if (ILType_IsArray(type))
		    {
			    fprintf(stderr, "Multidimential arrays are not supported for marshaling as a return value\n");
    		    	    return 0;
    		    }
    }

    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) return in;
    temp = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
    jit_insn_store_relative(function, outAddressValue, offset, temp);
    return temp;
}

static ILJitValue MarshalDelegateReturnStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
						    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
						    unsigned int addressKind)
{
	ILExecThread *thread = ILExecThreadCurrent();
	ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
	unsigned int structSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(thread), structureILType);
	ILField *field = 0;
	ILType *type;

	switch(addressKind)
	{
		case MARSHAL_FIRST_LEVEL_VALUE:
		{
			ILJitValue sizeValue = jit_value_create_nint_constant(function, _IL_JIT_TYPE_UINT32, structSize);
			outAddress = jit_insn_call_native(function, "ILGCAlloc",
								    ILGCAlloc,
								    _ILJitSignature_malloc,
								    &sizeValue, 1, 0);
		}
		break;
		case MARSHAL_ITEM_OF_STRUCTURE:
		case MARSHAL_ITEM_OF_ARRAY:
		{	    
			
		}
		break;
		default:
		{
			fprintf(stderr, "addressKind is an invalid value of %d\n", addressKind);
			return 0;
		}
		break;
	}

	while((field = (ILField *)ILClassNextMemberByKind
			(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(!ILField_IsStatic(field))
		{
			type = ILTypeGetEnumType(ILField_Type(field));
			switch(addressKind)
			{
				case MARSHAL_FIRST_LEVEL_VALUE:
				{
					MarshalDelegateReturnValue(function, inAddress, type, marshalType,
									MARSHAL_ITEM_OF_STRUCTURE, field->offset + offset,
									outAddress);
				}
			        break;
				case MARSHAL_ITEM_OF_STRUCTURE:
				{
					MarshalDelegateReturnValue(function, inAddress, type, marshalType,
									MARSHAL_ITEM_OF_STRUCTURE, field->offset + offset,
									outAddress);
				}
				break;
				default:
				{
					MarshalDelegateReturnValue(function, inAddress, type, marshalType,
									MARSHAL_ITEM_OF_ARRAY, field->offset + offset,
									outAddress);
				}
				break;
			}
		}
	}
	return outAddress;
}

#ifdef USE_BYREF_MARSHALING
static void MarshalDelegateByRefValue(jit_function_t function, ILJitValue in, ILType *type, ILUInt32 marshalType,
					    unsigned int addressKind, jit_nint offset, ILJitValue outAddress,
					    ILExecThread *thread)
{
    ILJitValue outAddressValue;
    ILJitValue temp = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue args[3];
    ILJitValue srcArray = jit_value_create(function, _IL_JIT_TYPE_VPTR);
    ILJitValue srcElemSize;
    ILType *elemType;
    ILJitValue newElemSize;
    jit_label_t startLoop = jit_label_undefined;
    jit_label_t endLoop = jit_label_undefined;
    ILJitValue srcElemAddress;
    ILJitValue newElemAddress;
    ILJitValue counter;
    ILJitValue arraySize;

    if(marshalType == IL_META_MARSHAL_CUSTOM)
    {
	    fprintf(stderr, "Custom marshaling not supported in delegate by ref value\n");
	    return;
    }

    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)
    {
	    if(!NeedMarshalValue(type))return;
    }

    switch(addressKind)
    {    
	    case MARSHAL_FIRST_LEVEL_VALUE:
	    case MARSHAL_ITEM_OF_STRUCTURE:
	    case MARSHAL_ITEM_OF_ARRAY:
	    {
		    outAddressValue = outAddress;
	    }
	    break;
	    default:
	    {
		    fprintf(stderr, "addressKind has an invalid %d value\n", addressKind);
		    return;
	    }
	    break;
    }

    if(ILType_IsPrimitive(type))
    {
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_VOID:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I1:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U1:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I2:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U2:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_CHAR:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I4:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U4:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I8:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U8:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_R4:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_R8:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_I:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_U:
		{
			return;
		}
		break;
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{	
			fprintf(stderr, "Marshaling of TypedByRef as a ref value in delegate is not supported\n");
			return;
		}
		break;
		case IL_META_ELEMTYPE_OBJECT:
		{	
			return;
		}
		break;
		default: break;
	}			
    }
    else if(ILType_IsValueType(type))
    {
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    MarshalDelegateByRefStruct(function, jit_insn_address_of(function, in), type, marshalType,
						    jit_insn_address_of(function, in), 0, MARSHAL_ITEM_OF_STRUCTURE);
		    return;
	    }
	    MarshalDelegateByRefStruct(function, in, type, marshalType, outAddressValue, offset, MARSHAL_ITEM_OF_STRUCTURE);
	    return;
    }
    else if(ILTypeIsDelegateSubClass(type))
    {
	    return;
    }
    else if(ILTypeIsStringClass(type))
    {
	    args[0] = jit_insn_call_native(function, "ILExecThreadCurrent", ILExecThreadCurrent,
							    _ILJitSignature_ILExecThreadCurrent,
							    0, 0, JIT_CALL_NOTHROW);
	    if(addressKind==MARSHAL_FIRST_LEVEL_VALUE) args[1] = in;
	    else args[1] = jit_insn_load_relative(function, in, offset, _IL_JIT_TYPE_VPTR);
	    char *customName;
	    int customNameLen;
	    char *customCookie;
	    int customCookieLen;
	    if(addressKind!=MARSHAL_FIRST_LEVEL_VALUE)
	    {
		    ILMethod *method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		    marshalType = ILPInvokeGetMarshalType
				    (0, method, 0, &customName, &customNameLen, &customCookie, &customCookieLen, type);
	    }
	    switch(marshalType)
	    {
			case IL_META_MARSHAL_ANSI_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToAnsi",
							    ILStringToAnsi,
							    _ILJitSignature_ILStringToAnsi,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF8_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToUTF8",
							    ILStringToUTF8,
							    _ILJitSignature_ILStringToUTF8,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			case IL_META_MARSHAL_UTF16_STRING:
			{
				temp = jit_insn_call_native(function,
							    "ILStringToUTF16",
							    ILStringToUTF16,
							    _ILJitSignature_ILStringToUTF16,
							    args, 2, JIT_CALL_NOTHROW);
			}
			break;
			default:
			{
				temp = args[1];
			}
			break;
	    }
	    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
		    jit_insn_store_relative(function, outAddressValue, offset, temp);
	    return;
    }
    else if(type!=0 && ILType_IsComplex(type))
    {
	    switch(ILType_Kind(type))
	    {
		    case IL_TYPE_COMPLEX_PTR:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store_relative(function, in, offset, temp);
			    return;
		    }
		    break;
		    case IL_TYPE_COMPLEX_BYREF:
		    {
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    jit_insn_store(function, srcArray, jit_insn_load_relative(function, in,
										    offset, _IL_JIT_TYPE_VPTR));
			    else jit_insn_store(function, srcArray, in);
			    ILType *elemType = ILType_Ref(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(NeedMarshalValue(elemType))
			    {
				    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    {
					    MarshalDelegateByRefValue(function, jit_insn_dup(function, srcArray),
										elemType, marshalType,
										MARSHAL_ITEM_OF_STRUCTURE,
										0,
										jit_insn_load_relative(function,
													outAddressValue,
													offset,
													_IL_JIT_TYPE_VPTR));
				    }
				    else MarshalDelegateByRefValue(function, jit_insn_dup(function, srcArray),
										    elemType, marshalType,
										    MARSHAL_ITEM_OF_STRUCTURE,
										    0,
										    srcArray);
			    }
			    return;
		    }
		    break;
		    default: break;
	    }

	    if(ILType_IsSimpleArray(type))
	    {
			    srcArray = in;
			    if(addressKind==MARSHAL_ITEM_OF_STRUCTURE || addressKind==MARSHAL_ITEM_OF_ARRAY)
				    srcArray = jit_insn_load_relative(function, srcArray, offset, _IL_JIT_TYPE_VPTR);
			    jit_insn_branch_if_not(function, jit_insn_to_bool(function, srcArray),
								&endLoop);
			    elemType = ILType_ElemType(type);
			    elemType = ILTypeGetEnumType(elemType);
			    if(!NeedMarshalValue(elemType) || ILType_IsValueType(elemType))
			    {
				    return;
			    }

			    // The size of a simple array can be known only on run-time.
			    arraySize = _ILJitSArrayGetLength(function, srcArray);

			    srcElemSize = jit_value_create_nint_constant(function,
				    						_IL_JIT_TYPE_INT32,
										(jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elemType)));
			    newElemSize = jit_value_create_nint_constant(function,
										_IL_JIT_TYPE_INT32,
									    	(jit_nint)(_ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elemType)));

			    jit_insn_branch_if(function,
						jit_insn_eq(function, arraySize,
									jit_value_create_nint_constant
										(function, _IL_JIT_TYPE_UINT32, 0)),
										&endLoop);
			    newElemAddress = jit_insn_dup(function, outAddressValue);
			    srcElemAddress = _ILJitSArrayGetBase(function, srcArray);
			    counter = jit_insn_dup(function, arraySize);
			    jit_insn_label(function, &startLoop);
			    jit_insn_store(function, counter, jit_insn_sub(function,
										counter,
		    								jit_value_create_nint_constant(function,
										_IL_JIT_TYPE_INT32,
										1)));
			    MarshalDelegateByRefValue(function, srcElemAddress,
							elemType, marshalType,
							MARSHAL_ITEM_OF_ARRAY,
							0,
							newElemAddress);
			    jit_insn_store(function, srcElemAddress,
							    jit_insn_add(function,
							    srcElemAddress, srcElemSize));
			    jit_insn_store(function, newElemAddress,
							jit_insn_add(function,
							newElemAddress, newElemSize));
			    jit_insn_branch_if(function,
					        jit_insn_to_bool(function, counter),
					        &(startLoop));
			    jit_insn_label(function, &endLoop);
			    return;
	    }
	    else if(ILType_IsArray(type))
	    {
		    fprintf(stderr, "Multidimential arrays are not supported for marshaling as a return byref value\n");
		    return;
	    }

    }
    // If we handled every case above, we should not reach here,
    // but make the compiler happy.
    return;
}

static void MarshalDelegateByRefStruct(jit_function_t function, ILJitValue inAddress, ILType *structureILType,
					    ILUInt32 marshalType, ILJitValue outAddress, jit_nint offset,
					    unsigned int addressKind)
{
    ILClass *classInfo = ILClassResolve(ILType_ToValueType(structureILType));
    ILField *field = 0;
    ILType *type;

    while((field = (ILField *)ILClassNextMemberByKind
    		(classInfo, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
    {
	    if(!ILField_IsStatic(field))
	    {
		    type = ILTypeGetEnumType(ILField_Type(field));
		    MarshalDelegateByRefValue(function, inAddress, type, marshalType, MARSHAL_ITEM_OF_STRUCTURE,
						    field->offset + offset, outAddress);
	    }
    }
    return;
}
#endif // USE_BYREF_MARSHALING

/*
 * Marshal a CLR object to its native representation with custom marshaling support.
 */
static void* MarshalObjectToCustom(void *value, ILType *type, ILMethod *method, ILExecThread *thread)
{
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;

	ILPInvokeGetMarshalType
	    (0, method, 0, &customName, &customNameLen,
		&customCookie, &customCookieLen, type);
	void *ptr = _ILObjectToCustom (thread, value, customName, 
					    customNameLen, customCookie, customCookieLen);
	return ptr;
}

/*
 * Marshal a returned native value to a CLR object with custom marshaling support.
 */
static void* MarshalCustomToObject(void *value, ILType *type, ILMethod *method, ILExecThread *thread)
{
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;

	ILPInvokeGetMarshalType
	    (0, method, 0, &customName, &customNameLen,
		&customCookie, &customCookieLen, type);
	void *ptr = _ILCustomToObject (thread, value, customName,
					    customNameLen, customCookie, customCookieLen);
	return ptr;
}
