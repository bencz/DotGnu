/*
 * cvmc_call.c - Coder implementation for CVM method calls.
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

#ifdef IL_CVMC_CODE

static void CVMCoder_UpConvertArg(ILCoder *coder, ILEngineStackItem *args,
						          ILUInt32 numArgs, ILUInt32 param,
						          ILType *paramType)
{
	/* TODO */
}

static void CVMCoder_DownConvertArg(ILCoder *coder, ILEngineStackItem *args,
						            ILUInt32 numArgs, ILUInt32 param,
						            ILType *paramType)
{
	/* TODO */
}

static void CVMCoder_PackVarArgs(ILCoder *coder, ILType *callSiteSig,
					             ILUInt32 firstParam, ILEngineStackItem *args,
						         ILUInt32 numArgs)
{
#ifdef IL_CONFIG_VARARGS
	CVMP_OUT_WORD2_PTR(COP_PREFIX_PACK_VARARGS,
					   firstParam, numArgs, callSiteSig);
	CVM_ADJUST(-(ILInt32)(ComputeStackSize(coder, args, numArgs)));
	CVM_ADJUST(1);
#endif
}

static void CVMCoder_ValueCtorArgs(ILCoder *coder, ILClass *classInfo,
								   ILEngineStackItem *args, ILUInt32 numArgs)
{
	ILUInt32 posn = ComputeStackSize(coder, args, numArgs);
	ILUInt32 size = GetTypeSize(_ILCoderToILCVMCoder(coder)->process,
								ILType_FromValueType(classInfo));
	CVM_OUT_DWIDE(COP_NEW_VALUE, posn, size);
	CVM_ADJUST(size + 1);
}

/*
 * Adjust the position of the stack for a call.
 */
static void AdjustForCall(ILCoder *coder, ILCoderMethodInfo *info,
						  ILEngineStackItem *returnItem)
{
	CVM_ADJUST(-(ILInt32)ComputeStackSize
					(coder, info->args, info->numBaseArgs));
	if(info->hasParamArray)
	{
		CVM_ADJUST(-1);
	}
	if(returnItem != 0 && returnItem->engineType != ILEngineType_Invalid)
	{
		CVM_ADJUST(ComputeStackSize(coder, returnItem, 1));
	}
}

static void CVMCoder_CheckCallNull(ILCoder *coder, ILCoderMethodInfo *info)
{
	ILUInt32 size = ComputeStackSize(coder, info->args, info->numBaseArgs);
	if(info->hasParamArray)
	{
		/* Account for the vararg parameter array on the stack */
		++size;
	}
	if(size == 1)
	{
		CVM_OUT_NONE(COP_CKNULL);
	}
	else
	{
		--size;
		CVM_OUT_WIDE(COP_CKNULL_N, size);
	}
}

static void CVMCoder_CallMethod(ILCoder *coder, ILCoderMethodInfo *info,
								ILEngineStackItem *returnItem,
								ILMethod *methodInfo)
{
	if(info->tailCall)
	{
		CVMP_OUT_PTR(COP_PREFIX_TAIL_CALL, methodInfo);
	}
	else
	{
		CVM_OUT_PTR(COP_CALL, methodInfo);
	}
	AdjustForCall(coder, info, returnItem);
}

static void CVMCoder_CallIndirect(ILCoder *coder, ILCoderMethodInfo *info,
								  ILEngineStackItem *returnItem)
{
	if(info->tailCall)
	{
		CVMP_OUT_NONE(COP_PREFIX_TAIL_CALLI);
	}
	else
	{
		CVM_OUT_NONE(COP_CALLI);
	}
	CVM_ADJUST(-1);	/* The function pointer was popped */
	AdjustForCall(coder, info, returnItem);
}

static void CVMCoder_CallCtor(ILCoder *coder, ILCoderMethodInfo *info,
					   		  ILMethod *methodInfo)
{
	CVM_OUT_PTR(COP_CALL_CTOR, methodInfo);
	AdjustForCall(coder, info, 0);
	CVM_ADJUST(1);
}

static void CVMCoder_CallVirtual(ILCoder *coder, ILCoderMethodInfo *info,
								 ILEngineStackItem *returnItem,
								 ILMethod *methodInfo)
{
	ILUInt32 argSize = ComputeStackSize(coder, info->args, info->numBaseArgs);
	ILUInt32 index = methodInfo->index;
	int isVirtualGenericCall = 0;
	if(info->hasParamArray)
	{
		++argSize;
	}
	if(ILMember_IsGenericInstance(methodInfo))
	{
		if(ILMethod_IsVirtualGeneric(methodInfo))
		{
			ILMethodInstance *methodInst = (ILMethodInstance *)methodInfo;

			/* This is an instance of a virtual generic method. */
			index = (index << 16) | methodInst->genMethod->index;
			isVirtualGenericCall = 1;
		}
	}
	if(isVirtualGenericCall)
	{
		CVMP_OUT_WORD2(COP_PREFIX_CALL_VIRTGEN, argSize, index);
	}
	else
	{
		if(info->tailCall)
		{
			CVMP_OUT_WORD2(COP_PREFIX_TAIL_CALLVIRT, argSize, index);
		}
		else
		{
			CVM_OUT_DWIDE(COP_CALL_VIRTUAL, argSize, index);
		}
	}
	AdjustForCall(coder, info, returnItem);
}

static void CVMCoder_CallInterface(ILCoder *coder, ILCoderMethodInfo *info,
								   ILEngineStackItem *returnItem,
								   ILMethod *methodInfo)
{
	ILUInt32 argSize = ComputeStackSize(coder, info->args, info->numBaseArgs);
	if(info->hasParamArray)
	{
		++argSize;
	}
#ifdef IL_USE_IMTS
	{
		ILUInt32 index = methodInfo->index;
		ILClassPrivate *classPrivate;
		classPrivate = (ILClassPrivate *)(methodInfo->member.owner->userData);
		if(classPrivate)
		{
			index += classPrivate->imtBase;
		}
		index %= IL_IMT_SIZE;
		if(info->tailCall)
		{
			CVMP_OUT_WORD2_PTR(COP_PREFIX_TAIL_CALLINTF, argSize,
							   index, methodInfo);
		}
		else
		{
			CVM_OUT_DWIDE_PTR(COP_CALL_INTERFACE, argSize,
							  index, methodInfo);
		}
	}
#else
	{
		void *ptr = ILMethod_Owner(methodInfo);
		if(info->tailCall)
		{
			CVMP_OUT_WORD2_PTR(COP_PREFIX_TAIL_CALLINTF, argSize,
							   methodInfo->index, ptr);
		}
		else
		{
			CVM_OUT_DWIDE_PTR(COP_CALL_INTERFACE, argSize,
							  methodInfo->index, ptr);
		}
	}
#endif
	AdjustForCall(coder, info, returnItem);
}

/*
 * Macros inlining simple methods that don't change the stack height.
 */
#define CASE_INLINEMETHOD(x) \
	case IL_INLINEMETHOD_##x: \
		CVMP_OUT_NONE(COP_PREFIX_##x);\
		return 1;

/*
 * Macros inlining simple methods that change the stack height by 1.
 */
#define CASE_INLINEMETHOD_1(x) \
	case IL_INLINEMETHOD_##x: \
		CVMP_OUT_NONE(COP_PREFIX_##x);\
		CVM_ADJUST(-1); \
		return 1;

static int CVMCoder_CallInlineable(ILCoder *coder, int inlineType,
								   ILMethod *methodInfo, ILInt32 elementSize)
{
	/* Determine what to do for the inlineable method type */
	switch(inlineType)
	{
		case IL_INLINEMETHOD_MONITOR_ENTER:
		{
			/* Enter the monitor on the top-most object */
			if(ILHasThreads())
			{
				CVMP_OUT_NONE(COP_PREFIX_MONITOR_ENTER);
			}
			else
			{
				CVM_OUT_NONE(COP_POP);
			}
			CVM_ADJUST(-1);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_MONITOR_EXIT:
		{
			/* Exit the monitor on the top-most object */
			if(ILHasThreads())
			{
				CVMP_OUT_NONE(COP_PREFIX_MONITOR_EXIT);
			}
			else
			{
				CVM_OUT_NONE(COP_POP);
			}
			CVM_ADJUST(-1);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_STRING_LENGTH:
		{
			/* The string length is at a fixed offset from the pointer */
			CVM_OUT_BYTE(COP_IREAD_FIELD,
						 (unsigned)(ILNativeUInt)&(((System_String *)0)->length));
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_STRING_CONCAT_2:
		{
			/* Concatenate two string objects */
			CVMP_OUT_NONE(COP_PREFIX_STRING_CONCAT_2);
			CVM_ADJUST(-1);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_STRING_CONCAT_3:
		{
			/* Concatenate three string objects */
			CVMP_OUT_NONE(COP_PREFIX_STRING_CONCAT_3);
			CVM_ADJUST(-2);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_STRING_CONCAT_4:
		{
			/* Concatenate four string objects */
			CVMP_OUT_NONE(COP_PREFIX_STRING_CONCAT_4);
			CVM_ADJUST(-3);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_STRING_EQUALS:
		{
			/* Compare two string objects for equality */
			CVMP_OUT_NONE(COP_PREFIX_STRING_EQ);
			CVM_ADJUST(-1);
			return 1;
		}
		
		/* Not reached */

		case IL_INLINEMETHOD_STRING_NOT_EQUALS:
		{
			/* Compare two string objects for inequality */
			CVMP_OUT_NONE(COP_PREFIX_STRING_NE);
			CVM_ADJUST(-1);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_STRING_GET_CHAR:
		{
			/* Compare two string objects for equality */
			CVMP_OUT_NONE(COP_PREFIX_STRING_GET_CHAR);
			CVM_ADJUST(-1);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_TYPE_FROM_HANDLE:
		{
			/* Convert a RuntimeTypeHandle into a Type object */
			CVMP_OUT_NONE(COP_PREFIX_TYPE_FROM_HANDLE);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_GET2D_INT:
		{
			CVMP_OUT_NONE(COP_PREFIX_GET2D);
			CVM_OUT_NONE(COP_IREAD);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_GET2D_OBJECT:
		{
			CVMP_OUT_NONE(COP_PREFIX_GET2D);
			CVM_OUT_NONE(COP_PREAD);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_GET2D_DOUBLE:
		{
			CVMP_OUT_NONE(COP_PREFIX_GET2D);
			CVM_OUT_NONE(COP_DREAD);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SET2D_INT:
		{
			CVMP_OUT_WORD(COP_PREFIX_SET2D, 1);
			CVM_OUT_NONE(COP_IWRITE);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SET2D_OBJECT:
		{
			CVMP_OUT_WORD(COP_PREFIX_SET2D, 1);
			CVM_OUT_NONE(COP_PWRITE);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_SET2D_DOUBLE:
		{
			CVMP_OUT_WORD(COP_PREFIX_SET2D, CVM_WORDS_PER_NATIVE_FLOAT);
			CVM_OUT_NONE(COP_DWRITE);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_BUILDER_APPEND_CHAR:
		{
			/* Concatenate a char */
			CVMP_OUT_PTR(COP_PREFIX_APPEND_CHAR, methodInfo);
			CVM_ADJUST(-1);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_IS_WHITE_SPACE:
		{
			/* Check a character to see if it is white space */
			CVMP_OUT_NONE(COP_PREFIX_IS_WHITE_SPACE);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ARRAY_COPY_AAI4:
		{
			CVMP_OUT_WORD(COP_PREFIX_SARRAY_COPY_AAI4, elementSize);
			CVM_ADJUST(-3);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ARRAY_COPY_AI4AI4I4:
		{
			CVMP_OUT_WORD(COP_PREFIX_SARRAY_COPY_AI4AI4I4, elementSize);
			CVM_ADJUST(-5);
			return 1;
		}
		/* Not reached */

		case IL_INLINEMETHOD_ARRAY_CLEAR_AI4I4:
		{
			CVMP_OUT_WORD(COP_PREFIX_SARRAY_CLEAR_AI4I4, elementSize);
			CVM_ADJUST(-3);
			return 1;
		}
		/* Not reached */

		/*
		 * Cases for Math class inlines.
		 */
		CASE_INLINEMETHOD(ABS_I4);
		CASE_INLINEMETHOD(ABS_R4);
		CASE_INLINEMETHOD(ABS_R8);
		CASE_INLINEMETHOD(ASIN);
		CASE_INLINEMETHOD(ATAN);
		CASE_INLINEMETHOD_1(ATAN2);
		CASE_INLINEMETHOD(CEILING);
		CASE_INLINEMETHOD(COS);
		CASE_INLINEMETHOD(COSH);
		CASE_INLINEMETHOD(EXP);
		CASE_INLINEMETHOD(FLOOR);
		CASE_INLINEMETHOD_1(IEEEREMAINDER);
		CASE_INLINEMETHOD(LOG);
		CASE_INLINEMETHOD(LOG10);
		CASE_INLINEMETHOD_1(MAX_I4);
		CASE_INLINEMETHOD_1(MIN_I4);
		CASE_INLINEMETHOD_1(MAX_R4);
		CASE_INLINEMETHOD_1(MIN_R4);
		CASE_INLINEMETHOD_1(MAX_R8);
		CASE_INLINEMETHOD_1(MIN_R8);
		CASE_INLINEMETHOD_1(POW);
		CASE_INLINEMETHOD(ROUND);
		CASE_INLINEMETHOD(SIN);
		CASE_INLINEMETHOD(SIGN_I4);
		CASE_INLINEMETHOD(SIGN_R4);
		CASE_INLINEMETHOD(SIGN_R8);
		CASE_INLINEMETHOD(SINH);
		CASE_INLINEMETHOD(SQRT);
		CASE_INLINEMETHOD(TAN);
		CASE_INLINEMETHOD(TANH);
	}

	/* If we get here, then we don't know how to inline the method */
	return 0;
}

static void CVMCoder_JumpMethod(ILCoder *coder, ILMethod *methodInfo)
{
	/* TODO */
}

static void CVMCoder_ReturnInsn(ILCoder *coder, ILEngineType engineType,
							    ILType *returnType)
{
	switch(engineType)
	{
		case ILEngineType_Invalid:
		{
			CVM_OUT_RETURN(0);
		}
		break;

		case ILEngineType_M:
		case ILEngineType_CM:
		case ILEngineType_O:
		case ILEngineType_T:
		case ILEngineType_I4:
	#ifdef IL_NATIVE_INT32
		case ILEngineType_I:
	#endif
		{
			CVM_OUT_RETURN(1);
			CVM_ADJUST(-1);
		}
		break;

		case ILEngineType_I8:
	#ifdef IL_NATIVE_INT64
		case ILEngineType_I:
	#endif
		{
			CVM_OUT_RETURN(CVM_WORDS_PER_LONG);
			CVM_ADJUST(-CVM_WORDS_PER_LONG);
		}
		break;

		case ILEngineType_F:
		{
			CVM_OUT_RETURN(CVM_WORDS_PER_NATIVE_FLOAT);
			CVM_ADJUST(-CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case ILEngineType_MV:
		{
			ILUInt32 size = GetTypeSize(_ILCoderToILCVMCoder(coder)->process,
										returnType);
			CVM_OUT_RETURN(size);
			CVM_ADJUST(-(ILInt32)size);
		}
		break;

		case ILEngineType_TypedRef:
		{
			CVM_OUT_RETURN(CVM_WORDS_PER_TYPED_REF);
			CVM_ADJUST(-CVM_WORDS_PER_TYPED_REF);
		}
		break;
	}
}

static void CVMCoder_LoadFuncAddr(ILCoder *coder, ILMethod *methodInfo)
{
	CVMP_OUT_PTR(COP_PREFIX_LDFTN, methodInfo);
	CVM_ADJUST(1);
}

static void CVMCoder_LoadVirtualAddr(ILCoder *coder, ILMethod *methodInfo)
{
	CVMP_OUT_WORD(COP_PREFIX_LDVIRTFTN, methodInfo->index);
}

static void CVMCoder_LoadInterfaceAddr(ILCoder *coder, ILMethod *methodInfo)
{
	CVMP_OUT_WORD_PTR(COP_PREFIX_LDINTERFFTN, methodInfo->index,
					  methodInfo->member.owner);
}

#endif	/* IL_CVMC_CODE */
