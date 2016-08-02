/*
 * cvm_call.c - Opcodes for performing calls to other methods.
 *
 * Copyright (C) 2001, 2008, 2011  Southern Storm Software, Pty Ltd.
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

#include "il_dumpasm.h"
#include "interlocked.h"

#if defined(IL_CVM_GLOBALS)

/*
 * A little bit of assembler magic to stop gcc doing tail-end combination
 * on some of the switch cases related to method calls.
 */
#if defined(CVM_X86) && defined(__GNUC__) && !defined(IL_NO_ASM)
#define	CVM_OPTIMIZE_BLOCK()		__asm__ ("")
#else
#define	CVM_OPTIMIZE_BLOCK()
#endif

//#define INDENT_TRACE

/*
 * Call a function via the FFI interface.  If we have "libffi",
 * then use the "ffi_call" and "ffi_raw_call" functions.
 * Otherwise, we assume that "cif" is the marshalling stub
 * that we need to use.
 */
#if defined(HAVE_LIBFFI)
#define	FFI_CALL(cif,fn,rvalue,avalue)	\
			ffi_call((ffi_cif *)(cif), (void (*)())(fn), \
			         (void *)(rvalue), (void **)(avalue))
#if !FFI_NO_RAW_API && FFI_NATIVE_RAW_API
#define	FFI_RAW_CALL(cif,fn,rvalue,avalue)	\
			ffi_raw_call((ffi_cif *)(cif), (void (*)())(fn), \
			             (void *)(rvalue), (ffi_raw *)(avalue))
#else
#define	FFI_RAW_CALL(cif,fn,rvalue,avalue)
#endif
#else
#define	FFI_CALL(cif,fn,rvalue,avalue)	\
			(*((void (*)(void (*)(), void *, void **))(cif)))	\
				((void (*)())(fn), (void *)(rvalue), (void **)(avalue))
#define	FFI_RAW_CALL(cif,fn,rvalue,avalue)
#endif

/*
 * Allocate a new call frame.
 */
ILCallFrame *_ILAllocCallFrame(ILExecThread *thread)
{
#ifdef IL_CONFIG_GROW_FRAMES
	ILUInt32 newsize;
	ILCallFrame *newframe;

	/* Calculate target frame size */
	newsize = thread->maxFrames * 2;

	/* Allocate the new frame and copy the old contents into it */
	if((newframe = (ILCallFrame *)ILGCAllocPersistent
				(sizeof(ILCallFrame) * newsize)) == 0)
	{
	    _ILExecThreadSetOutOfMemoryException(thread);
		return 0;
	}
	ILMemCpy(newframe, thread->frameStack,
			 sizeof(ILCallFrame) * thread->maxFrames);

	/* Free the old frame stack and copy the new one into place */
	ILGCFreePersistent(thread->frameStack);
	thread->frameStack = newframe;
	thread->maxFrames = newsize;

	/* Return the new frame to the caller */
	return &(thread->frameStack[(thread->numFrames)++]);
#else
	/* We are not allowed to grow the frame stack */
    _ILExecThreadSetOutOfMemoryException(thread);
	return 0;
#endif
}


#define CHECK_MANAGED_BARRIER()	\
	if (IL_EXPECT(thread->managedSafePointFlags, 0))	\
	{	\
		if  ((thread->managedSafePointFlags & _IL_MANAGED_SAFEPOINT_THREAD_ABORT) && ILThreadIsAbortRequested()) \
		{ \
			if (_ILExecThreadSelfAborting(thread) == 0) \
			{ \
				goto throwThreadAbortException; \
			} \
		} \
		else if (thread->managedSafePointFlags & _IL_MANAGED_SAFEPOINT_THREAD_SUSPEND) \
		{ \
			ILInterlockedAndU4(&(thread->managedSafePointFlags), \
							   ~_IL_MANAGED_SAFEPOINT_THREAD_SUSPEND); \
			if (ILThreadGetState(thread->supportThread) & IL_TS_SUSPEND_REQUESTED) \
			{ \
				_ILExecThreadSuspendThread(thread, thread->supportThread); \
			} \
		} \
	}

#define BEGIN_NATIVE_CALL()	\
	thread->runningManagedCode = 0;

#define END_NATIVE_CALL() \
	CHECK_MANAGED_BARRIER(); \
	thread->runningManagedCode = 1;

#ifdef IL_DUMP_CVM
#define	DUMP_STACK()	\
			do { \
				int posn; \
				BEGIN_NATIVE_CALL(); \
				fprintf(IL_DUMP_CVM_STREAM, "Stack:"); \
				for(posn = 1; posn <= 16; ++posn) \
				{ \
					if(posn <= (int)(stacktop - thread->stackBase)) \
					{ \
						fprintf(IL_DUMP_CVM_STREAM, " 0x%lX", \
							    (unsigned long)(stacktop[-posn].uintValue)); \
					} \
				} \
				putc('\n', IL_DUMP_CVM_STREAM); \
				fflush(IL_DUMP_CVM_STREAM); \
				END_NATIVE_CALL(); \
			} while (0)
#define	REPORT_METHOD_CALL()	\
			do { \
				BEGIN_NATIVE_CALL(); \
				fprintf(IL_DUMP_CVM_STREAM, "Entering %s::%s (%ld)\n", \
					    methodToCall->member.owner->className->name, \
					    methodToCall->member.name, \
					    (long)(stacktop - thread->stackBase)); \
				DUMP_STACK(); \
				END_NATIVE_CALL(); \
			} while (0)
#else
#define	REPORT_METHOD_CALL()
#endif
#define	ALLOC_CALL_FRAME()	\
			do { \
				REPORT_METHOD_CALL(); \
				if(IL_EXPECT(thread->numFrames < thread->maxFrames, 1)) \
				{ \
					callFrame = &(thread->frameStack[(thread->numFrames)++]); \
				} \
				else \
				{ \
					BEGIN_NATIVE_CALL(); \
					callFrame = _ILAllocCallFrame(thread); \
					END_NATIVE_CALL(); \
					if(IL_EXPECT(!callFrame, 0)) \
					{ \
						STACK_OVERFLOW_EXCEPTION(); \
					} \
				} \
			} while (0)

/*
 * Restore state information from the thread, except the pc.
 */
#ifdef IL_DUMP_CVM
#define	RESTORE_STATE_FROM_THREAD()	\
			do { \
				stacktop = thread->stackTop; \
				frame = thread->frame; \
				stackmax = thread->stackLimit; \
				if(IL_EXPECT(_ILExecThreadGetException(thread) != 0, 0)) \
				{ \
					/* An exception occurred, which we now must handle */ \
					goto throwCurrentException; \
				} \
			} while (0)
#else
#define	RESTORE_STATE_FROM_THREAD()	\
			do { \
				if(IL_EXPECT(_ILExecThreadGetException(thread) != 0, 0)) \
				{ \
					/* An exception occurred, which we now must handle */ \
					goto throwCurrentException; \
				} \
				stacktop = thread->stackTop; \
				frame = thread->frame; \
				stackmax = thread->stackLimit; \
			} while (0)
#endif

/*
 * Determine the number of stack words that are occupied
 * by a specific type.
 */
ILUInt32 _ILStackWordsForType(ILExecThread *thread, ILType *type)
{
	if(type == ILType_Float32 || type == ILType_Float64)
	{
		return CVM_WORDS_PER_NATIVE_FLOAT;
	}
	else
	{
		return ((ILSizeOfType(thread, type) + sizeof(CVMWord) - 1)
					/ sizeof(CVMWord));
	}
}

/*
 * Pack a set of arguments into a params "Object[]" array.
 * Returns the number of stack words to pop from the function,
 * and the new array in "*array". This is not included in VarArgs
 * as this is needed by non-vararg operations like BeginInvoke
 */
ILUInt32 _ILPackCVMStackArgs(ILExecThread *thread, CVMWord *stacktop,
							ILUInt32 firstParam, ILUInt32 numArgs,
							ILType *callSiteSig, void **array)
{
	ILUInt32 height;
	ILUInt32 param;
	ILObject **posn;
	ILType *paramType;
	ILType *enumType;
	ILInt8 boxByte;
	ILInt16 boxShort;
#ifdef IL_CONFIG_FP_SUPPORTED
	ILFloat boxFloat;
	ILDouble boxDouble;
#else
	void *boxPtr;
#endif
	void *ptr;

	/* Allocate an array to hold all of the arguments */
	*array = ILExecThreadNew(thread, "[oSystem.Object;", "(Ti)V",
							 (ILVaUInt)numArgs);
	if(!(*array))
	{
		return 0;
	}

	/* Find the base and height of the varargs arguments */
	height = 0;
	for(param = 0; param < numArgs; ++param)
	{
		height += _ILStackWordsForType
			(thread, ILTypeGetParam(callSiteSig, firstParam + param));
	}
	stacktop -= height;

	/* Convert the arguments into objects in the array */
	posn = (ILObject **)ArrayToBuffer(*array);
	for(param = 0; param < numArgs; ++param)
	{
		paramType = ILTypeGetParam(callSiteSig, firstParam + param);

		if (ILType_IsComplex(paramType)
			&& ILType_Kind(paramType) == IL_TYPE_COMPLEX_BYREF)
		{
			ILType *targetParamType = ILType_Ref(paramType);

			if(ILType_IsPrimitive(targetParamType))
			{
				/* Box a primitive value after aligning it properly */
				switch(ILType_ToElement(targetParamType))
				{
					case IL_META_ELEMTYPE_I1:
					case IL_META_ELEMTYPE_U1:
					case IL_META_ELEMTYPE_BOOLEAN:
					{
						boxByte = (ILInt8)(*(ILInt32 *)(stacktop->ptrValue));
						ptr = &boxByte;
					}
					break;

					case IL_META_ELEMTYPE_I2:
					case IL_META_ELEMTYPE_U2:
					case IL_META_ELEMTYPE_CHAR:
					{
						boxShort = (ILInt16)(*(ILInt32 *)(stacktop->ptrValue));
						ptr = &boxShort;
					}
					break;

				#ifdef IL_CONFIG_FP_SUPPORTED
					case IL_META_ELEMTYPE_R4:
					{
						boxFloat = (ILFloat)(ReadFloat(stacktop->ptrValue));
						ptr = &boxFloat;
					}
					break;

					case IL_META_ELEMTYPE_R8:
					case IL_META_ELEMTYPE_R:
					{
						boxDouble = (ILDouble)(ReadDouble(stacktop->ptrValue));
						ptr = &boxDouble;
					}
					break;
				#else
					case IL_META_ELEMTYPE_R4:
					case IL_META_ELEMTYPE_R8:
					case IL_META_ELEMTYPE_R:
					{
						/* No FP support, so pass a "null" instead */
						boxPtr = 0;
						ptr = &boxPtr;
					}
					break;
				#endif

					default:
					{
						ptr = stacktop->ptrValue;
					}
					break;
				}
				*posn = ILExecThreadBox(thread, targetParamType, ptr);
				if(!(*posn))
				{
					return 0;
				}
			}
			else if(ILType_IsValueType(targetParamType))
			{
				/* Box value types after aligning small enumerated types */
				ptr = stacktop->ptrValue;
				if(ILTypeIsEnum(targetParamType))
				{
					enumType = ILTypeGetEnumType(targetParamType);
					if(ILType_IsPrimitive(enumType))
					{
						switch(ILType_ToElement(enumType))
						{
							case IL_META_ELEMTYPE_I1:
							case IL_META_ELEMTYPE_U1:
							case IL_META_ELEMTYPE_BOOLEAN:
							{
								boxByte = (ILInt8)(*(ILInt32 *)(stacktop->ptrValue));
								ptr = &boxByte;
							}
							break;

							case IL_META_ELEMTYPE_I2:
							case IL_META_ELEMTYPE_U2:
							case IL_META_ELEMTYPE_CHAR:
							{
								boxShort = (ILInt16)(*(ILInt32 *)(stacktop->ptrValue));
								ptr = &boxShort;
							}
							break;
						}
					}
				}
				*posn = ILExecThreadBox(thread, targetParamType, ptr);
				if(!(*posn))
				{
					return 0;
				}
			}
			else if(ILTypeIsReference(targetParamType))
			{
				/* Ref to an object reference type: pass the object reference */
				*posn = *((ILObject **)(stacktop->ptrValue));
			}
			else
			{
				/* Assume that everything else is a pointer, and wrap
				it up within a "System.IntPtr" object */
				*posn = ILExecThreadBox(thread, ILType_Int, stacktop->ptrValue);
				if(!(*posn))
				{
					return 0;
				}
			}
		}
		else
		{
			if(ILType_IsPrimitive(paramType))
			{
				/* Box a primitive value after aligning it properly */
				switch(ILType_ToElement(paramType))
				{
					case IL_META_ELEMTYPE_I1:
					case IL_META_ELEMTYPE_U1:
					case IL_META_ELEMTYPE_BOOLEAN:
					{
						boxByte = (ILInt8)(stacktop->intValue);
						ptr = &boxByte;
					}
					break;

					case IL_META_ELEMTYPE_I2:
					case IL_META_ELEMTYPE_U2:
					case IL_META_ELEMTYPE_CHAR:
					{
						boxShort = (ILInt16)(stacktop->intValue);
						ptr = &boxShort;
					}
					break;

				#ifdef IL_CONFIG_FP_SUPPORTED
					case IL_META_ELEMTYPE_R4:
					{
						boxFloat = (ILFloat)(ReadFloat(stacktop));
						ptr = &boxFloat;
					}
					break;

					case IL_META_ELEMTYPE_R8:
					case IL_META_ELEMTYPE_R:
					{
						boxDouble = (ILDouble)(ReadDouble(stacktop));
						ptr = &boxDouble;
					}
					break;
				#else
					case IL_META_ELEMTYPE_R4:
					case IL_META_ELEMTYPE_R8:
					case IL_META_ELEMTYPE_R:
					{
						/* No FP support, so pass a "null" instead */
						boxPtr = 0;
						ptr = &boxPtr;
					}
					break;
				#endif

					default:
					{
						ptr = stacktop;
					}
					break;
				}
				*posn = ILExecThreadBox(thread, paramType, ptr);
				if(!(*posn))
				{
					return 0;
				}
			}
			else if(ILType_IsValueType(paramType))
			{
				/* Box value types after aligning small enumerated types */
				ptr = stacktop;
				if(ILTypeIsEnum(paramType))
				{
					enumType = ILTypeGetEnumType(paramType);
					if(ILType_IsPrimitive(enumType))
					{
						switch(ILType_ToElement(enumType))
						{
							case IL_META_ELEMTYPE_I1:
							case IL_META_ELEMTYPE_U1:
							case IL_META_ELEMTYPE_BOOLEAN:
							{
								boxByte = (ILInt8)(stacktop->intValue);
								ptr = &boxByte;
							}
							break;

							case IL_META_ELEMTYPE_I2:
							case IL_META_ELEMTYPE_U2:
							case IL_META_ELEMTYPE_CHAR:
							{
								boxShort = (ILInt16)(stacktop->intValue);
								ptr = &boxShort;
							}
							break;
						}
					}
				}
				*posn = ILExecThreadBox(thread, paramType, ptr);
				if(!(*posn))
				{
					return 0;
				}
			}
			else if(ILTypeIsReference(paramType))
			{
				/* Object reference type: pass it directly */
				*posn = (ILObject *)(stacktop->ptrValue);
			}
			else
			{
				/* Assume that everything else is a pointer, and wrap
				it up within a "System.IntPtr" object */
				*posn = ILExecThreadBox(thread, ILType_Int, stacktop);
				if(!(*posn))
				{
					return 0;
				}
			}
		}

		stacktop += _ILStackWordsForType(thread, paramType);
		++posn;
	}

	/* Return the height of the varargs arguments to the caller */
	return height;
}

/*
 * Get the number of parameter words for a method.
 */
ILUInt32 _ILGetMethodParamCount(ILExecThread *thread, ILMethod *method,
								int suppressThis)
{
	ILType *signature = ILMethod_Signature(method);
	ILUInt32 num = 0;
	ILUInt32 numParams;
	ILUInt32 param;

	/* Account for the "this" argument */
	if(ILType_HasThis(signature) && !suppressThis)
	{
		++num;
	}

	/* Account for the size of the regular arguments */
	numParams = ILTypeNumParams(signature);
	for(param = 1; param <= numParams; ++param)
	{
		num += _ILStackWordsForType(thread, ILTypeGetParam(signature, param));
	}

	/* Account for the vararg array parameter */
	if((ILType_CallConv(signature) & IL_META_CALLCONV_MASK) ==
				IL_META_CALLCONV_VARARG)
	{
		++num;
	}

	/* Return the word count to the caller */
	return num;
}

#elif defined(IL_CVM_LOCALS)

ILMethod *IL_METHODTOCALL_VOLATILE methodToCall;
ILCallFrame *IL_CALLFRAME_VOLATILE callFrame = 0;

#elif defined(IL_CVM_MAIN)

/**
 * <opcode name="call" group="Call management instructions">
 *   <operation>Call a method</operation>
 *
 *   <format>call<fsep/>mptr</format>
 *   <dformat>{call}<fsep/>mptr</dformat>
 *
 *   <form name="call" code="COP_CALL"/>
 *
 *   <description>The <i>call</i> instruction effects a method
 *   call to <i>mptr</i>.  The call proceeds as follows:
 *
 *   <ul>
 *     <li>The method is converted into CVM bytecode.  If this is not
 *         possible, then <code>System.Security.VerificationException</code>
 *         will be thrown.</li>
 *     <li>A new call frame is allocated.</li>
 *     <li>The current method, program counter, frame pointer, and
 *         exception frame height are saved into the call frame.</li>
 *     <li>The program counter is set to the first instruction in
 *         the method <i>mptr</i>.</li>
 *     <li>The current method is set to <i>mptr</i>.</li>
 *   </ul>
 *   </description>
 *
 *   <notes>The <i>mptr</i> value is a 32-bit or 64-bit method
 *   pointer reference.<p/>
 *
 *   The <i>call</i> instruction does not set up a new frame pointer.
 *   The <i>set_num_args</i> instruction is used to set up local variable
 *   frames at the start of the new method's code.
 *
 *   The <i>return*</i> instructions are responsible for popping the
 *   method arguments from the stack at method exit.</notes>
 *
 *   <exceptions>
 *     <exception name="System.Security.VerificationException">Raised if
 *     the method could not be translated into CVM bytecode.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_CALL):
{
	/* Call a method */
	methodToCall = CVM_ARG_PTR(ILMethod *);
	if(methodToCall->userData)
	{
		/* It is converted: allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALL_RETURN(pc);
		callFrame->frame = frame;
		callFrame->permissions = 0;

		/* Pass control to the new method */
		pc = (unsigned char *)(methodToCall->userData);
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
	else
	{
		/* Copy the state back into the thread object */
		COPY_STATE_TO_THREAD();

		BEGIN_NATIVE_CALL();

		/* Convert the method */
		IL_CONVERT_METHOD(tempptr, thread, methodToCall);
		if(!tempptr)
		{
			END_NATIVE_CALL();

			CONVERT_FAILED_EXCEPTION();
		}

		END_NATIVE_CALL();

		/* Allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALL_RETURN(thread->pc);
		callFrame->frame = thread->frame;
		callFrame->permissions = 0;

		/* Restore the state information and jump to the new method */
		RESTORE_STATE_FROM_THREAD();
		pc = (unsigned char *)tempptr;
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
}
VMBREAK(COP_CALL);

/**
 * <opcode name="call_ctor" group="Call management instructions">
 *   <operation>Call a constructor</operation>
 *
 *   <format>call_ctor<fsep/>mptr</format>
 *   <dformat>{call_ctor}<fsep/>mptr</dformat>
 *
 *   <form name="call_ctor" code="COP_CALL_CTOR"/>
 *
 *   <description>The <i>call_ctor</i> instruction effects a method
 *   call to the constructor identified by <i>mptr</i>.</description>
 *
 *   <notes>Constructors in the CVM system have two entry points: one
 *   which creates a block of memory and then initializes it; and the
 *   other which initializes a pre-allocated block.  The particular
 *   entry point is chosen based on the constructor's usage in the
 *   original CIL bytecode:
 *
 *   <ul>
 *     <li>If the CIL bytecode invoked the constructor method using
 *         <i>newobj</i>, then <i>call_ctor</i> should be used.</li>
 *     <li>If the CIL bytecode invoked a parent class's constructor
 *         method directly using the IL <i>call</i> instruction,
 *         then <i>call</i> should be used.</li>
 *   </ul>
 *
 *   See the description of the <i>call</i> instruction for
 *   a full account of frame handling, argument handling, etc.</notes>
 * </opcode>
 */
VMCASE(COP_CALL_CTOR):
{
	/* Call a constructor that we don't know if it has been converted */
	methodToCall = CVM_ARG_PTR(ILMethod *);

	/* Determine if we have already converted the constructor */
	if(methodToCall->userData)
	{
		/* It is converted: allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALL_RETURN(pc);
		callFrame->frame = frame;
		callFrame->permissions = 0;

		/* Pass control to the new method */
		pc = ((unsigned char *)(methodToCall->userData)) - CVM_CTOR_OFFSET;
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
	else
	{
		/* Copy the state back into the thread object */
		COPY_STATE_TO_THREAD();

		/* Convert the method */
		BEGIN_NATIVE_CALL();

		IL_CONVERT_METHOD(tempptr, thread, methodToCall);
		if(!tempptr)
		{
			END_NATIVE_CALL();
			CONVERT_FAILED_EXCEPTION();
		}

		END_NATIVE_CALL();

		/* Allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALL_RETURN(thread->pc);
		callFrame->frame = thread->frame;
		callFrame->permissions = 0;

		/* Restore the state information and jump to the new method */
		RESTORE_STATE_FROM_THREAD();
		pc = ((unsigned char *)tempptr) - CVM_CTOR_OFFSET;
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
}
VMBREAK(COP_CALL_CTOR);

/**
 * <opcode name="call_native" group="Call management instructions">
 *   <operation>Call a native function that has a return value</operation>
 *
 *   <format>call_native<fsep/>function<fsep/>cif</format>
 *   <dformat>{call_native}<fsep/>function<fsep/>cif</dformat>
 *
 *   <form name="call_native" code="COP_CALL_NATIVE"/>
 *
 *   <before>..., address</before>
 *   <after>...</after>
 *
 *   <description>The <i>call_native</i> instruction effects a native
 *   function call to <i>function</i>, using <i>cif</i> to define the
 *   format of the function arguments and return value.  The return
 *   value is stored at <i>address</i>.  The arguments are assumed
 *   to have already been stored into the "native argument buffer"
 *   using the <i>waddr_native*</i> instructions.</description>
 *
 *   <notes>Both <i>function</i> and <i>cif</i> are native pointers,
 *   which may be either 32 or 64 bits in size, depending upon the
 *   platform.<p/>
 *
 *   Native function calls occur in CIL "InternalCall" and "PInvoke"
 *   methods.  For each such method, the CVM translation process
 *   creates a CVM stub method that transfers the arguments on
 *   the CVM stack to the native argument buffer, makes the native
 *   call, and then puts the function's return value back onto
 *   the CVM stack prior to exiting.</notes>
 * </opcode>
 */
VMCASE(COP_CALL_NATIVE):
{
	BEGIN_NATIVE_CALL();

	/* Call a native method */
	COPY_STATE_TO_THREAD();
	FFI_CALL(CVM_ARG_PTR2(void *), CVM_ARG_PTR(void *),
	         stacktop[-1].ptrValue, nativeArgs);
	RESTORE_STATE_FROM_THREAD();
	pc = thread->pc;
	MODIFY_PC_AND_STACK(CVM_LEN_PTR2, -1);

	END_NATIVE_CALL();
}
VMBREAK(COP_CALL_NATIVE);

/**
 * <opcode name="call_native_void" group="Call management instructions">
 *   <operation>Call a native function with no return value</operation>
 *
 *   <format>call_native_void<fsep/>function<fsep/>cif</format>
 *   <dformat>{call_native_void}<fsep/>function<fsep/>cif</dformat>
 *
 *   <form name="call_native_void" code="COP_CALL_NATIVE_VOID"/>
 *
 *   <description>The <i>call_native_void</i> instruction is identical
 *   to <i>call_native</i>, except that the native function is assumed
 *   not to have a return value.</description>
 * </opcode>
 */
VMCASE(COP_CALL_NATIVE_VOID):
{
	BEGIN_NATIVE_CALL();

	/* Call a native method that has no return value */
	COPY_STATE_TO_THREAD();
	FFI_CALL(CVM_ARG_PTR2(void *), CVM_ARG_PTR(void *), 0, nativeArgs);
	RESTORE_STATE_FROM_THREAD();
	pc = thread->pc;
	MODIFY_PC_AND_STACK(CVM_LEN_PTR2, 0);

	END_NATIVE_CALL();
}
VMBREAK(COP_CALL_NATIVE_VOID);

/**
 * <opcode name="call_native_raw" group="Call management instructions">
 *   <operation>Call a native function that has a return value,
 *              using a raw call</operation>
 *
 *   <format>call_native_raw<fsep/>function<fsep/>cif</format>
 *   <dformat>{call_native_raw}<fsep/>function<fsep/>cif</dformat>
 *
 *   <form name="call_native_raw" code="COP_CALL_NATIVE_RAW"/>
 *
 *   <before>..., avalue, rvalue</before>
 *   <after>...</after>
 *
 *   <description>The <i>call_native_raw</i> instruction effects a native
 *   function call to <i>function</i>, using <i>cif</i> to define the
 *   format of the function arguments and return value.  The arguments
 *   are stored on the stack beginning at <i>avalue</i>.  The return
 *   value is stored at <i>rvalue</i>.</description>
 *
 *   <notes>This instruction differs from <i>call_native</i> in the manner
 *   in which the call is performed.  This instruction uses a "raw" call,
 *   which is only applicable on some platforms.  The arguments are
 *   passed on the stack, instead of in a separate native argument
 *   buffer.</notes>
 * </opcode>
 */
VMCASE(COP_CALL_NATIVE_RAW):
{
	BEGIN_NATIVE_CALL();

	/* Call a native method using the raw API */
	COPY_STATE_TO_THREAD();
	FFI_RAW_CALL(CVM_ARG_PTR2(void *), CVM_ARG_PTR(void *),
	             stacktop[-1].ptrValue, stacktop[-2].ptrValue);
	RESTORE_STATE_FROM_THREAD();
	pc = thread->pc;
	MODIFY_PC_AND_STACK(CVM_LEN_PTR2, -2);

	END_NATIVE_CALL();
}
VMBREAK(COP_CALL_NATIVE_RAW);

/**
 * <opcode name="call_native_void_raw" group="Call management instructions">
 *   <operation>Call a native function with no return value
 *              using a raw call</operation>
 *
 *   <format>call_native_void_raw<fsep/>function<fsep/>cif</format>
 *   <dformat>{call_native_void_raw}<fsep/>function<fsep/>cif</dformat>
 *
 *   <form name="call_native_void_raw" code="COP_CALL_NATIVE_VOID_RAW"/>
 *
 *   <before>..., avalue</before>
 *   <after>...</after>
 *
 *   <description>The <i>call_native_void_raw</i> instruction is identical
 *   to <i>call_native_raw</i>, except that the native function is assumed
 *   not to have a return value.</description>
 * </opcode>
 */
VMCASE(COP_CALL_NATIVE_VOID_RAW):
{
	BEGIN_NATIVE_CALL();

	/* Call a native method that has no return value using the raw API */
	COPY_STATE_TO_THREAD();
	FFI_RAW_CALL(CVM_ARG_PTR2(void *), CVM_ARG_PTR(void *),
				 0, stacktop[-1].ptrValue);
	RESTORE_STATE_FROM_THREAD();
	pc = thread->pc;
	MODIFY_PC_AND_STACK(CVM_LEN_PTR2, -1);

	END_NATIVE_CALL();
}
VMBREAK(COP_CALL_NATIVE_VOID_RAW);

/**
 * <opcode name="call_virtual" group="Call management instructions">
 *   <operation>Call a virtual method</operation>
 *
 *   <format>call_virtual<fsep/>N[1]<fsep/>M[1]</format>
 *   <format>wide<fsep/>call_virtual<fsep/>N[4]<fsep/>M[4]</format>
 *   <dformat>{call_virtual}<fsep/>N<fsep/>M</dformat>
 *
 *   <form name="call_virtual" code="COP_CALL_VIRTUAL"/>
 *
 *   <description>The <i>call_virtual</i> instruction effects a
 *   virtual method call.  The value <i>N</i> indicates the
 *   position of the <code>this</code> pointer on the stack:
 *   1 indicates the top of stack, 2 indicates the stack word
 *   just below the top-most stack word, etc.  The value <i>M</i>
 *   is the offset into the object's vtable for the method.</description>
 *
 *   <notes>See the description of the <i>call</i> instruction for
 *   a full account of frame handling, argument handling, etc.</notes>
 *
 *   <exceptions>
 *     <exception name="System.NullReferenceException">Raised if
 *     the <code>this</code> pointer is <code>null</code>.</exception>
 *     <exception name="System.Security.VerificationException">Raised if
 *     the method could not be translated into CVM bytecode.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_CALL_VIRTUAL):
{
	/* Call a virtual method */
	tempptr = stacktop[-((ILInt32)CVM_ARG_DWIDE1_SMALL)].ptrValue;
	if(tempptr)
	{
		/* Locate the method to be called */
		methodToCall = (GetObjectClassPrivate(tempptr))
							->vtable[CVM_ARG_DWIDE2_SMALL];

		/* Has the method already been converted? */
		if(methodToCall->userData)
		{
			/* It is converted: allocate a new call frame */
			ALLOC_CALL_FRAME();

			/* Fill in the call frame details */
			callFrame->method = method;
			callFrame->pc = CVM_ARG_CALLV_RETURN_SMALL(pc);
			callFrame->frame = frame;
			callFrame->permissions = 0;

			/* Pass control to the new method */
			pc = (unsigned char *)(methodToCall->userData);
			method = methodToCall;
			CVM_OPTIMIZE_BLOCK();
		}
		else
		{
			/* Copy the state back into the thread object */
			COPY_STATE_TO_THREAD();

			/* Convert the method */
			BEGIN_NATIVE_CALL();

			IL_CONVERT_METHOD(tempptr, thread, methodToCall);
			if(!tempptr)
			{
				END_NATIVE_CALL();

				CONVERT_FAILED_EXCEPTION();
			}

			END_NATIVE_CALL();

			/* Allocate a new call frame */
			ALLOC_CALL_FRAME();

			/* Fill in the call frame details */
			callFrame->method = method;
			callFrame->pc = CVM_ARG_CALLV_RETURN_SMALL(thread->pc);
			callFrame->frame = thread->frame;
			callFrame->permissions = 0;

			/* Restore the state information and jump to the new method */
			RESTORE_STATE_FROM_THREAD();
			pc = (unsigned char *)tempptr;
			method = methodToCall;
			CVM_OPTIMIZE_BLOCK();
		}
	}
	else
	{
		NULL_POINTER_EXCEPTION();
	}
}
VMBREAK(COP_CALL_VIRTUAL);

/**
 * <opcode name="call_interface" group="Call management instructions">
 *   <operation>Call an interface method</operation>
 *
 *   <format>call_interface<fsep/>N[1]<fsep/>M[1]<fsep/>cptr</format>
 *   <format>wide<fsep/>call_interface<fsep/>N[4]<fsep/>M[4]<fsep/>cptr</format>
 *   <dformat>{call_interface}<fsep/>N<fsep/>M<fsep/>cptr</dformat>
 *
 *   <form name="call_interface" code="COP_CALL_INTERFACE"/>
 *
 *   <description>The <i>call_interface</i> instruction effects an
 *   interface method call.  The value <i>N</i> indicates the
 *   position of the <code>this</code> pointer on the stack:
 *   1 indicates the top of stack, 2 indicates the stack word
 *   just below the top-most stack word, etc.  The value <i>M</i>
 *   is the offset into the interface's vtable for the method.  The value
 *   <i>cptr</i> indicates the interface class pointer.</description>
 *
 *   <notes>See the description of the <i>call</i> instruction for
 *   a full account of frame handling, argument handling, etc.<p/>
 *
 *   The <i>cptr</i> value is a native pointer that may be either 32 or
 *   64 bits in size, depending upon the platform.</notes>
 *
 *   <exceptions>
 *     <exception name="System.NullReferenceException">Raised if
 *     the <code>this</code> pointer is <code>null</code>.</exception>
 *     <exception name="System.Security.VerificationException">Raised if
 *     the method could not be translated into CVM bytecode.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_CALL_INTERFACE):
{
	/* Call an interface method */
	tempptr = stacktop[-((ILInt32)CVM_ARG_DWIDE1_SMALL)].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		/* Locate the method to be called */
	#ifdef IL_USE_IMTS
		methodToCall = GetObjectClassPrivate(tempptr)
			->imt[CVM_ARG_DWIDE2_SMALL];
		if(!methodToCall)
		{
			methodToCall = CVM_ARG_DWIDE_PTR_SMALL(ILMethod *);
			methodToCall = _ILLookupInterfaceMethod
				(GetObjectClassPrivate(tempptr),
				 methodToCall->member.owner, methodToCall->index);
			if(!methodToCall)
			{
				MISSING_METHOD_EXCEPTION();
			}
		}
		#if IL_DEBUG_IMTS
		if(methodToCall)
		{
			fprintf(stderr, "%s:%d found <%s:%s> for <%s:%s> \n", 
				__FILE__, __LINE__,
				ILClass_Name(methodToCall->member.owner),
				ILMethod_Name(methodToCall),
				ILClass_Name(
					ILMethod_Owner(CVM_ARG_DWIDE_PTR_SMALL(ILMethod *))),
				ILMethod_Name(CVM_ARG_DWIDE_PTR_SMALL(ILMethod*)));
		}
		#endif
	#else
		methodToCall = _ILLookupInterfaceMethod
			(GetObjectClassPrivate(tempptr), CVM_ARG_DWIDE_PTR_SMALL(ILClass *),
			 CVM_ARG_DWIDE2_SMALL);
		if(!methodToCall)
		{
			MISSING_METHOD_EXCEPTION();
		}
	#endif

		/* Has the method already been converted? */
		if(methodToCall->userData)
		{
			/* It is converted: allocate a new call frame */
			ALLOC_CALL_FRAME();

			/* Fill in the call frame details */
			callFrame->method = method;
			callFrame->pc = CVM_ARG_CALLI_RETURN_SMALL(pc);
			callFrame->frame = frame;
			callFrame->permissions = 0;

			/* Pass control to the new method */
			pc = (unsigned char *)(methodToCall->userData);
			method = methodToCall;
			CVM_OPTIMIZE_BLOCK();
		}
		else
		{
			/* Copy the state back into the thread object */
			COPY_STATE_TO_THREAD();
	
			/* Convert the method */
			BEGIN_NATIVE_CALL();

			IL_CONVERT_METHOD(tempptr, thread, methodToCall);
			if(!tempptr)
			{
				END_NATIVE_CALL();

				CONVERT_FAILED_EXCEPTION();
			}
			
			END_NATIVE_CALL();

			/* Allocate a new call frame */
			ALLOC_CALL_FRAME();

			/* Fill in the call frame details */
			callFrame->method = method;
			callFrame->pc = CVM_ARG_CALLI_RETURN_SMALL(thread->pc);
			callFrame->frame = thread->frame;
			callFrame->permissions = 0;

			/* Restore the state information and jump to the new method */
			RESTORE_STATE_FROM_THREAD();
			pc = (unsigned char *)tempptr;
			method = methodToCall;
			CVM_OPTIMIZE_BLOCK();
		}
	}
	END_NULL_CHECK();
}
VMBREAK(COP_CALL_INTERFACE);

/**
 * <opcode name="return" group="Call management instructions">
 *   <operation>Return from the current method with no return value</operation>
 *
 *   <format>return</format>
 *   <dformat>{return}</dformat>
 *
 *   <form name="return" code="COP_RETURN"/>
 *
 *   <description>Return control to the method that called the current
 *   method, as follows:
 *
 *   <ul>
 *     <li>Set the top of stack pointer to the frame pointer.</li>
 *     <li>Pop the top-most call frame from the call frame stack.</li>
 *     <li>Retrieve the method pointer, progrm counter, exception frame
 *         height, and the frame pointer from the call frame.</li>
 *   </ul>
 *   </description>
 *
 *   <notes>The <i>set_num_args</i> instruction has previously set the
 *   frame pointer to the address of the first argument.  When <i>return</i>
 *   is executed, the first step above will pop all of the arguments.</notes>
 * </opcode>
 */
VMCASE(COP_RETURN):
{
	/* Return from a method with no return value */
	stacktop = frame;
popFrame:

	CHECK_MANAGED_BARRIER();
	
	callFrame = &(thread->frameStack[--(thread->numFrames)]);

	methodToCall = callFrame->method;
	pc = callFrame->pc;
	frame = callFrame->frame;
	method = methodToCall;

	/* Should we return to an external method? */
	if(pc == IL_INVALID_PC)
	{
#if defined(IL_USE_INTERRUPT_BASED_X)
		IL_MEMCPY(&thread->exceptionJumpBuffer, &backupJumpBuffer, sizeof(IL_JMP_BUFFER));
#endif
		COPY_STATE_TO_THREAD();
		return _CVM_EXIT_OK;
	}

#ifdef IL_DUMP_CVM
	/* Dump the name of the method we are returning to */
	fprintf(IL_DUMP_CVM_STREAM, "Returning to %s::%s (%ld)\n",
			method->member.owner->className->name,
		    method->member.name, (long)(stacktop - thread->stackBase));
	DUMP_STACK();
#endif
}
VMBREAK(COP_RETURN);

/**
 * <opcode name="return_1" group="Call management instructions">
 *   <operation>Return from the current method with a single stack
 *              word as a return value</operation>
 *
 *   <format>return_1</format>
 *   <dformat>{return_1}</dformat>
 *
 *   <form name="return_1" code="COP_RETURN_1"/>
 *
 *   <description>Return control to the method that called the current
 *   method, as follows:
 *
 *   <ul>
 *     <li>Copy the top-most word on the stack to the position
 *         indicated by the frame pointer, and then set the top
 *         of stack pointer to point just after the copy.</li>
 *     <li>Pop the top-most call frame from the call frame stack.</li>
 *     <li>Retrieve the method pointer, progrm counter, exception frame
 *         height, and the frame pointer from the call frame.</li>
 *   </ul>
 *   </description>
 *
 *   <notes>The <i>set_num_args</i> instruction has previously set the
 *   frame pointer to the address of the first argument.  When <i>return_1</i>
 *   is executed, the first step above will pop all of the arguments,
 *   with the single-word return value left in their place.</notes>
 * </opcode>
 */
VMCASE(COP_RETURN_1):
{
	/* Return from a method with a single-word return value */
	frame[0] = stacktop[-1];
	stacktop = frame + 1;
	goto popFrame;
}
/* Not reached */

/**
 * <opcode name="return_2" group="Call management instructions">
 *   <operation>Return from the current method with two stack
 *              words as the return value</operation>
 *
 *   <format>return_2</format>
 *   <dformat>{return_2}</dformat>
 *
 *   <form name="return_2" code="COP_RETURN_2"/>
 *
 *   <description>Return control to the method that called the current
 *   method, as follows:
 *
 *   <ul>
 *     <li>Copy the two top-most words on the stack to the position
 *         indicated by the frame pointer, and then set the top
 *         of stack pointer to point just after the two copied words.</li>
 *     <li>Pop the top-most call frame from the call frame stack.</li>
 *     <li>Retrieve the method pointer, progrm counter, exception frame
 *         height, and the frame pointer from the call frame.</li>
 *   </ul>
 *   </description>
 *
 *   <notes>The <i>set_num_args</i> instruction has previously set the
 *   frame pointer to the address of the first argument.  When <i>return_2</i>
 *   is executed, the first step above will pop all of the arguments,
 *   with the double-word return value left in their place.</notes>
 * </opcode>
 */
VMCASE(COP_RETURN_2):
{
	/* Return from a method with a double-word return value */
	frame[0] = stacktop[-2];
	frame[1] = stacktop[-1];
	stacktop = frame + 2;
	goto popFrame;
}
/* Not reached */

/**
 * <opcode name="return_n" group="Call management instructions">
 *   <operation>Return from the current method with <i>n</i> stack
 *              words as the return value</operation>
 *
 *   <format>return_n<fsep/>N[4]</format>
 *   <dformat>{return_n}<fsep/>N</dformat>
 *
 *   <form name="return_n" code="COP_RETURN_N"/>
 *
 *   <description>Return control to the method that called the current
 *   method, as follows:
 *
 *   <ul>
 *     <li>Copy the <i>n</i> top-most words on the stack to the position
 *         indicated by the frame pointer, and then set the top of
 *         stack pointer to point just after the <i>n</i> copied words.</li>
 *     <li>Pop the top-most call frame from the call frame stack.</li>
 *     <li>Retrieve the method pointer, progrm counter, exception frame
 *         height, and the frame pointer from the call frame.</li>
 *   </ul>
 *   </description>
 *
 *   <notes>The <i>set_num_args</i> instruction has previously set the
 *   frame pointer to the address of the first argument.  When <i>return_n</i>
 *   is executed, the first step above will pop all of the arguments,
 *   with the <i>n</i>-word return value left in their place.</notes>
 * </opcode>
 */
VMCASE(COP_RETURN_N):
{
	/* Return from a method with an N-word return value */
	tempNum = CVM_ARG_WORD;
	IL_MEMMOVE(frame, stacktop - tempNum, sizeof(CVMWord) * tempNum);
	stacktop = frame + tempNum;
	goto popFrame;
}
/* Not reached */

/**
 * <opcode name="push_thread" group="Call management instructions">
 *   <operation>Push the thread identifier onto the native
 *              argument stack</operation>
 *
 *   <format>push_thread</format>
 *   <dformat>{push_thread}</dformat>
 *
 *   <form name="push_thread" code="COP_PUSH_THREAD"/>
 *
 *   <description>Pushes an identifier for the current thread onto
 *   the native argument stack.  This is only used for "InternalCall"
 *   methods.  "PInvoke" methods should use <i>waddr_native_m1</i> instead.
 *   </description>
 * </opcode>
 */
VMCASE(COP_PUSH_THREAD):
{
	/* Push a pointer to the thread value onto the native argument stack */
	nativeArgs[0] = (void *)&thread;
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_PUSH_THREAD);

/**
 * <opcode name="push_thread_raw" group="Call management instructions">
 *   <operation>Push the thread identifier onto the native
 *              argument stack as a raw value</operation>
 *
 *   <format>push_thread_raw</format>
 *   <dformat>{push_thread_raw}</dformat>
 *
 *   <form name="push_thread_raw" code="COP_PUSH_THREAD_RAW"/>
 *
 *   <description>Pushes an identifier for the current thread onto
 *   the native argument stack.  This is only used for "InternalCall"
 *   methods.  This instruction differs from <i>push_thread</i> in
 *   that it is intended for use with <i>call_native_raw</i> instead
 *   of <i>call_native</i>.</description>
 * </opcode>
 */
VMCASE(COP_PUSH_THREAD_RAW):
{
	/* Push the thread value onto the stack */
	stacktop[0].ptrValue = (void *)thread;
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1);
}
VMBREAK(COP_PUSH_THREAD_RAW);

/**
 * <opcode name="pushdown" group="Call management instructions">
 *   <operation>Push the <code>ptr</code> value at the top of
 *              stack down and duplicate it twice</operation>
 *
 *   <format>pushdown<fsep/>N[4]</format>
 *   <dformat>{pushdown}<fsep/>N</dformat>
 *
 *   <form name="pushdown" code="COP_PUSHDOWN"/>
 *
 *   <before>..., val1, ..., valN, value</before>
 *   <after>..., value, value, val1, ..., valN</after>
 *
 *   <description>The <i>value</i> at the top of the stack is popped,
 *   pushed down <i>N</i> stack words, and duplicated twice.</description>
 *
 *   <notes>This instruction is used in combination with <i>new</i>
 *   to construct a block of memory for a new object.  The block
 *   is allocated, and then pushed down.  The lowest duplicated
 *   <i>value</i> becomes the return value for the constructor method.
 *   The other duplicated <i>value</i> becomes the <code>this</code>
 *   argument for the constructor method.</notes>
 * </opcode>
 */
VMCASE(COP_PUSHDOWN):
{
	/* Push a value on the stack top down and duplicate it twice */
	if(((ILUInt32)(stackmax - stacktop)) >= 1)
	{
		tempptr = stacktop[-1].ptrValue;
		tempNum = CVM_ARG_WORD;
		if(tempNum != 0)
		{
			IL_MEMMOVE(stacktop + 1 - tempNum, stacktop - 1 - tempNum,
					   sizeof(CVMWord) * tempNum);
		}
		(stacktop - tempNum - 1)->ptrValue = tempptr;
		(stacktop - tempNum)->ptrValue = tempptr;
		MODIFY_PC_AND_STACK(CVM_LEN_WORD, 1);
	}
	else
	{
		STACK_OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PUSHDOWN);

#define COP_WADDR_NATIVE(name,value)	\
VMCASE(COP_WADDR_NATIVE_##name): \
{ \
	/* Set a value within the native argument stack */ \
	nativeArgs[(value) + 1] = (void *)(&(frame[CVM_ARG_WIDE_SMALL])); \
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0); \
} \
VMBREAK(COP_WADDR_NATIVE_##name)

/**
 * <opcode name="waddr_native_&lt;n&gt;" group="Call management instructions">
 *   <operation>Set position <i>n</i> of the native argument buffer
 *              to the address of a local variable</operation>
 *
 *   <format>waddr_native_&lt;n&gt;<fsep/>V[1]</format>
 *   <format>wide<fsep/>waddr_native_&lt;n&gt;<fsep/>V[4]</format>
 *   <dformat>{waddr_native_&lt;n&gt;}<fsep/>V</dformat>
 *
 *   <form name="waddr_native_m1" code="COP_WADDR_NATIVE_M1"/>
 *   <form name="waddr_native_0" code="COP_WADDR_NATIVE_0"/>
 *   <form name="waddr_native_1" code="COP_WADDR_NATIVE_1"/>
 *   <form name="waddr_native_2" code="COP_WADDR_NATIVE_2"/>
 *   <form name="waddr_native_3" code="COP_WADDR_NATIVE_3"/>
 *   <form name="waddr_native_4" code="COP_WADDR_NATIVE_4"/>
 *   <form name="waddr_native_5" code="COP_WADDR_NATIVE_5"/>
 *   <form name="waddr_native_6" code="COP_WADDR_NATIVE_6"/>
 *   <form name="waddr_native_7" code="COP_WADDR_NATIVE_7"/>
 *
 *   <description>Set position <i>n</i> of the native argument buffer
 *   to the address of local variable <i>V</i>.  For an "InternalCall"
 *   method, 0 is the first argument.  For a "PInvoke" method,
 *   -1 (<i>m1</i>) is the first argument.</description>
 * </opcode>
 */
COP_WADDR_NATIVE(M1, -1);
COP_WADDR_NATIVE(0, 0);
COP_WADDR_NATIVE(1, 1);
COP_WADDR_NATIVE(2, 2);
COP_WADDR_NATIVE(3, 3);
COP_WADDR_NATIVE(4, 4);
COP_WADDR_NATIVE(5, 5);
COP_WADDR_NATIVE(6, 6);
COP_WADDR_NATIVE(7, 7);

VMCASE(COP_CALLI):
{
	/* Call a method by pointer */
	methodToCall = (ILMethod *)(stacktop[-1].ptrValue);
	--stacktop;
	if(methodToCall && methodToCall->userData)
	{
		/* It is converted: allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALLIND_RETURN(pc);
		callFrame->frame = frame;
		callFrame->permissions = 0;

		/* Pass control to the new method */
		pc = (unsigned char *)(methodToCall->userData);
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
	else if(methodToCall)
	{
		/* Copy the state back into the thread object */
		COPY_STATE_TO_THREAD();

		/* Convert the method */
		BEGIN_NATIVE_CALL();

		IL_CONVERT_METHOD(tempptr, thread, methodToCall);
		if(!tempptr)
		{
			END_NATIVE_CALL();

			CONVERT_FAILED_EXCEPTION();
		}

		END_NATIVE_CALL();

		/* Allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALLIND_RETURN(thread->pc);
		callFrame->frame = thread->frame;
		callFrame->permissions = 0;

		/* Restore the state information and jump to the new method */
		RESTORE_STATE_FROM_THREAD();
		pc = (unsigned char *)tempptr;
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
	else
	{
		NULL_POINTER_EXCEPTION();
	}
}
VMBREAK(COP_CALLI);

VMCASE(COP_JMPI):
{
	/* Jump to a method by pointer */
	/* TODO */
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_JMPI);

#elif defined(IL_CVM_WIDE)

case COP_CALL_VIRTUAL:
{
	/* Wide version of "call_virtual" */
	tempptr = stacktop[-((ILInt32)CVM_ARG_DWIDE1_LARGE)].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		/* Locate the method to be called */
		methodToCall = (GetObjectClassPrivate(tempptr))
							->vtable[CVM_ARG_DWIDE2_LARGE];

		/* Copy the state back into the thread object */
		COPY_STATE_TO_THREAD();

		/* Convert the method */
		BEGIN_NATIVE_CALL();

		IL_CONVERT_METHOD(tempptr, thread, methodToCall);
		if(!tempptr)
		{
			END_NATIVE_CALL();

			CONVERT_FAILED_EXCEPTION();
		}
		
		END_NATIVE_CALL();

		/* Allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALLV_RETURN_LARGE(thread->pc);
		callFrame->frame = thread->frame;
		callFrame->permissions = 0;

		/* Restore the state information and jump to the new method */
		RESTORE_STATE_FROM_THREAD();
		pc = (unsigned char *)tempptr;
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
	END_NULL_CHECK();
}
VMBREAKNOEND;

case COP_CALL_INTERFACE:
{
	/* Wide version of "call_interface" */
	tempptr = stacktop[-((ILInt32)CVM_ARG_DWIDE1_LARGE)].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		/* Locate the method to be called */
	#ifdef IL_USE_IMTS
		methodToCall = GetObjectClassPrivate(tempptr)
			->imt[CVM_ARG_DWIDE2_LARGE];
		if(!methodToCall)
		{
			methodToCall = CVM_ARG_DWIDE_PTR_LARGE(ILMethod *);
			methodToCall = _ILLookupInterfaceMethod
				(GetObjectClassPrivate(tempptr),
				 methodToCall->member.owner, methodToCall->index);
			if(!methodToCall)
			{
				MISSING_METHOD_EXCEPTION();
			}
		}
	#else
		methodToCall = _ILLookupInterfaceMethod
			(GetObjectClassPrivate(tempptr), CVM_ARG_DWIDE_PTR_LARGE(ILClass *),
			 CVM_ARG_DWIDE2_LARGE);
		if(!methodToCall)
		{
			MISSING_METHOD_EXCEPTION();
		}
	#endif

		/* Copy the state back into the thread object */
		COPY_STATE_TO_THREAD();

		/* Convert the method */
		BEGIN_NATIVE_CALL();

		IL_CONVERT_METHOD(tempptr, thread, methodToCall);
		if(!tempptr)
		{
			END_NATIVE_CALL();

			CONVERT_FAILED_EXCEPTION();
		}
		
		END_NATIVE_CALL();

		/* Allocate a new call frame */
		ALLOC_CALL_FRAME();

		/* Fill in the call frame details */
		callFrame->method = method;
		callFrame->pc = CVM_ARG_CALLI_RETURN_LARGE(thread->pc);
		callFrame->frame = thread->frame;
		callFrame->permissions = 0;

		/* Restore the state information and jump to the new method */
		RESTORE_STATE_FROM_THREAD();
		pc = (unsigned char *)tempptr;
		method = methodToCall;
		CVM_OPTIMIZE_BLOCK();
	}
	END_NULL_CHECK();
}
VMBREAKNOEND;

#define COP_WADDR_NATIVE_WIDE(name,value)	\
case COP_WADDR_NATIVE_##name: \
{ \
	/* Wide version of "waddr_native_*" */ \
	nativeArgs[(value) + 1] = (void *)(&(frame[CVM_ARG_WIDE_LARGE])); \
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0); \
} \
break

COP_WADDR_NATIVE_WIDE(M1, -1);
COP_WADDR_NATIVE_WIDE(0, 0);
COP_WADDR_NATIVE_WIDE(1, 1);
COP_WADDR_NATIVE_WIDE(2, 2);
COP_WADDR_NATIVE_WIDE(3, 3);
COP_WADDR_NATIVE_WIDE(4, 4);
COP_WADDR_NATIVE_WIDE(5, 5);
COP_WADDR_NATIVE_WIDE(6, 6);
COP_WADDR_NATIVE_WIDE(7, 7);

#elif defined(IL_CVM_PREFIX)

/**
 * <opcode name="tail_call" group="Call management instructions">
 *   <operation>Call a method using tail call semantics</operation>
 *
 *   <format>prefix<fsep/>tail_call<fsep/>mptr</format>
 *   <dformat>{tail_call}<fsep/>mptr</dformat>
 *
 *   <form name="tail_call" code="COP_PREFIX_TAIL_CALL"/>
 *
 *   <description>This instruction is identical to <i>call</i>, except
 *   that it performs a tail-optimized call to the method identified
 *   by <i>mptr</i>.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_TAIL_CALL):
{
	/* Retrieve the target method */
	methodToCall = CVM_ARG_TAIL_METHOD;

performTailCall:
	/* Convert the method if necessary */
	if(methodToCall->userData)
	{
		tempptr = methodToCall->userData;
	}
	else
	{
		COPY_STATE_TO_THREAD();
		BEGIN_NATIVE_CALL();

		IL_CONVERT_METHOD(tempptr, thread, methodToCall);
		if (!tempptr)
		{
			END_NATIVE_CALL();

			CONVERT_FAILED_EXCEPTION();
		}
		
		END_NATIVE_CALL();

		RESTORE_STATE_FROM_THREAD();
	}

	/* Copy the parameters down to the start of the frame */
	/* TODO: we should add an argument to the "tail" instruction
	   that contains "tempNum", so that we don't have to compute
	   the value dynamically */
	tempNum = _ILGetMethodParamCount(thread, methodToCall, 0);
	IL_MEMMOVE(frame, stacktop - tempNum, tempNum * sizeof(CVMWord));
	stacktop = frame + tempNum;

	/* Transfer control to the new method */
	REPORT_METHOD_CALL();
	pc = (unsigned char *)tempptr;
	method = methodToCall;
}
VMBREAK(COP_PREFIX_TAIL_CALL);

/**
 * <opcode name="tail_calli" group="Call management instructions">
 *   <operation>Call a method using indirect tail call semantics</operation>
 *
 *   <format>prefix<fsep/>tail_calli</format>
 *   <dformat>{tail_calli}</dformat>
 *
 *   <form name="tail_calli" code="COP_PREFIX_TAIL_CALLI"/>
 *
 *   <description>This instruction is identical to <i>calli</i>, except
 *   that it performs a tail-optimized call.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_TAIL_CALLI):
{
	/* Retrieve the target method */
	methodToCall = (ILMethod *)(stacktop[-1].ptrValue);
	--stacktop;
	if(methodToCall)
	{
		goto performTailCall;
	}
	else
	{
		NULL_POINTER_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_TAIL_CALLI);

/**
 * <opcode name="tail_callvirt" group="Call management instructions">
 *   <operation>Call a virtual method using tail call semantics</operation>
 *
 *   <format>prefix<fsep/>tail_callvirt<fsep/>N[1]<fsep/>M[1]</format>
 *   <dformat>{tail_callvirt}<fsep/>N<fsep/>M</dformat>
 *
 *   <form name="tail_callvirt" code="COP_PREFIX_TAIL_CALLVIRT"/>
 *
 *   <description>The <i>tail_callvirt</i> instruction is identical
 *   to <i>call_virtual</i>, except that it uses tail call semantics.
 *   </description>
 * </opcode>
 */
VMCASE(COP_PREFIX_TAIL_CALLVIRT):
{
	/* Call a virtual method */
	tempptr = stacktop[-((ILInt32)CVMP_ARG_WORD)].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		/* Locate the method to be called */
		methodToCall = (GetObjectClassPrivate(tempptr))->vtable[CVMP_ARG_WORD2];
		goto performTailCall;
	}
	END_NULL_CHECK();
}
VMBREAK(COP_PREFIX_TAIL_CALLVIRT);

/**
 * <opcode name="tail_callintf" group="Call management instructions">
 *   <operation>Call an interface method using tail call semantics</operation>
 *
 *   <format>prefix<fsep/>tail_callintf<fsep/>N[1]<fsep/>M[1]<fsep/>cptr</format>
 *   <dformat>{tail_callintf}<fsep/>N<fsep/>M<fsep/>cptr</dformat>
 *
 *   <form name="tail_callintf" code="COP_PREFIX_TAIL_CALLINTF"/>
 *
 *   <description>The <i>tail_callintf</i> instruction is identical
 *   to <i>call_interface</i>, except that it uses tail call semantics.
 *   </description>
 * </opcode>
 */
VMCASE(COP_PREFIX_TAIL_CALLINTF):
{
	/* Call an interface method */
	tempptr = stacktop[-((ILInt32)CVMP_ARG_WORD)].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		/* Locate the method to be called */
	#ifdef IL_USE_IMTS
		methodToCall = GetObjectClassPrivate(tempptr)->imt[CVMP_ARG_WORD2];
		if(!methodToCall)
		{
			methodToCall = CVMP_ARG_WORD2_PTR(ILMethod *);
			methodToCall = _ILLookupInterfaceMethod
				(GetObjectClassPrivate(tempptr),
				 methodToCall->member.owner, methodToCall->index);
			if(!methodToCall)
			{
				MISSING_METHOD_EXCEPTION();
			}
		}
	#else
		methodToCall = _ILLookupInterfaceMethod
			(GetObjectClassPrivate(tempptr), CVMP_ARG_WORD2_PTR(ILClass *),
			 CVMP_ARG_WORD2);
		if(!methodToCall)
		{
			MISSING_METHOD_EXCEPTION();
		}
	#endif
		goto performTailCall;
	}
	END_NULL_CHECK();
}
VMBREAK(COP_PREFIX_TAIL_CALLINTF);

/**
 * <opcode name="call_virtual_generic" group="Call management instructions">
 *   <operation>Call a virtual generic method instance</operation>
 *
 *   <format>call_virtual_generic<fsep/>N[1]<fsep/>M[1]</format>
 *   <format>wide<fsep/>call_virtual_generic<fsep/>N[4]<fsep/>M[4]</format>
 *   <dformat>{call_virtual_generic}<fsep/>N<fsep/>M</dformat>
 *
 *   <form name="call_virtual_generic" code="COP_PREFIX_CALL_VIRTGEN"/>
 *
 *   <description>The <i>call_virtual_generic</i> instruction effects a
 *   virtual generic method instance call.  The value <i>N</i> indicates the
 *   position of the <code>this</code> pointer on the stack:
 *   1 indicates the top of stack, 2 indicates the stack word
 *   just below the top-most stack word, etc.  The value <i>M</i>
 *   is the offset into the generic method vtable for the method instance.</description>
 *
 *   <notes>See the description of the <i>call</i> instruction for
 *   a full account of frame handling, argument handling, etc.</notes>
 *
 *   <exceptions>
 *     <exception name="System.NullReferenceException">Raised if
 *     the <code>this</code> pointer is <code>null</code>.</exception>
 *     <exception name="System.Security.VerificationException">Raised if
 *     the method could not be translated into CVM bytecode.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_CALL_VIRTGEN):
{
	/* Call a virtual method */
	tempptr = stacktop[-((ILInt32)CVM_ARG_DWIDE1_SMALL)].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		/* Locate the method to be called */
		tempNum = CVM_ARG_DWIDE2_SMALL;
		methodToCall = (GetObjectClassPrivate(tempptr))
							->vtable[tempNum & 0xFFFF];
		if(methodToCall)
		{
			methodToCall = ILMethodGetInstance(methodToCall, ((tempNum >> 16) & 0xFFFF));
			if(!methodToCall)
			{
				MISSING_METHOD_EXCEPTION();
			}
		}
		else
		{
			MISSING_METHOD_EXCEPTION();
		}

		/* Has the method already been converted? */
		if(methodToCall->userData)
		{
			/* It is converted: allocate a new call frame */
			ALLOC_CALL_FRAME();

			/* Fill in the call frame details */
			callFrame->method = method;
			callFrame->pc = CVM_ARG_CALLV_RETURN_SMALL(pc);
			callFrame->frame = frame;
			callFrame->permissions = 0;

			/* Pass control to the new method */
			pc = (unsigned char *)(methodToCall->userData);
			method = methodToCall;
			CVM_OPTIMIZE_BLOCK();
		}
		else
		{
			/* Copy the state back into the thread object */
			COPY_STATE_TO_THREAD();

			/* Convert the method */
			BEGIN_NATIVE_CALL();

			IL_CONVERT_METHOD(tempptr, thread, methodToCall);
			if(!tempptr)
			{
				END_NATIVE_CALL();

				CONVERT_FAILED_EXCEPTION();
			}

			END_NATIVE_CALL();

			/* Allocate a new call frame */
			ALLOC_CALL_FRAME();

			/* Fill in the call frame details */
			callFrame->method = method;
			callFrame->pc = CVM_ARG_CALLV_RETURN_SMALL(thread->pc);
			callFrame->frame = thread->frame;
			callFrame->permissions = 0;

			/* Restore the state information and jump to the new method */
			RESTORE_STATE_FROM_THREAD();
			pc = (unsigned char *)tempptr;
			method = methodToCall;
			CVM_OPTIMIZE_BLOCK();
		}
	}
	END_NULL_CHECK();
}
VMBREAK(COP_PREFIX_CALL_VIRTGEN);

/**
 * <opcode name="ldftn" group="Call management instructions">
 *   <operation>Load the address of a function method onto the stack</operation>
 *
 *   <format>prefix<fsep/>ldftn<fsep/>method</format>
 *   <dformat>{ltftn}<fsep/>method</dformat>
 *
 *   <form name="ldftn" code="COP_PREFIX_LDFTN"/>
 *
 *   <before>...</before>
 *   <after>..., method</after>
 *
 *   <description>Push <i>method</i> onto the stack as a <code>ptr</code>
 *   value.</description>
 *
 *   <notes>The <i>method</i> value may be either 32 or 64 bits in size,
 *   depending upon the platform.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_LDFTN):
{
	/* Load the address of a function onto the stack */
	stacktop[0].ptrValue = CVMP_ARG_PTR(void *);
	MODIFY_PC_AND_STACK(CVMP_LEN_PTR, 1);
}
VMBREAK(COP_PREFIX_LDFTN);

/**
 * <opcode name="ldvirtftn" group="Call management instructions">
 *   <operation>Load the address of a virtual function method
 *				onto the stack</operation>
 *
 *   <format>prefix<fsep/>ldvirtftn<fsep/>index[4]</format>
 *   <dformat>{ltvirtftn}<fsep/>index</dformat>
 *
 *   <form name="ldvirtftn" code="COP_PREFIX_LDVIRTFTN"/>
 *
 *   <before>..., object</before>
 *   <after>..., address</after>
 *
 *   <description>Pop <i>object</i> from the stack as type <code>ptr</code>
 *   and locate the virtual method at <i>index</i> within the object's
 *   vtable.  The address of this method is pushed onto the stack
 *   as type <code>ptr</code>.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_LDVIRTFTN):
{
	/* Load the address of a virtual function onto the stack */
	tempptr = stacktop[-1].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		stacktop[-1].ptrValue =
			(GetObjectClassPrivate(tempptr))->vtable[CVMP_ARG_WORD];
		MODIFY_PC_AND_STACK(CVMP_LEN_WORD, 0);
	}
	END_NULL_CHECK();
}
VMBREAK(COP_PREFIX_LDVIRTFTN);

/**
 * <opcode name="ldinterfftn" group="Call management instructions">
 *   <operation>Load the address of an interface function method
 *				onto the stack</operation>
 *
 *   <format>prefix<fsep/>ldinterfftn<fsep/>index[4]<fsep/>class</format>
 *   <dformat>{ltinterfftn}<fsep/>index<fsep/>class</dformat>
 *
 *   <form name="ldinterfftn" code="COP_PREFIX_LDINTERFFTN"/>
 *
 *   <before>..., object</before>
 *   <after>..., address</after>
 *
 *   <description>Pop <i>object</i> from the stack as type <code>ptr</code>
 *   and locate the virtual method at <i>index</i> within the object's
 *   interface vtable for the interface <i>class</i>.  The address of
 *   this method is pushed onto the stack as type
 *   <code>ptr</code>.</description>
 *
 *   <notes>The <i>class</i> value may be either 32 or 64 bits in size,
 *   depending upon the platform.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_LDINTERFFTN):
{
	/* Load the address of an interface function onto the stack */
	tempptr = stacktop[-1].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		stacktop[-1].ptrValue =
			_ILLookupInterfaceMethod(GetObjectClassPrivate(tempptr),
									 CVMP_ARG_WORD_PTR(ILClass *),
									 CVMP_ARG_WORD);
		MODIFY_PC_AND_STACK(CVMP_LEN_WORD_PTR, 0);
	}
	END_NULL_CHECK();
}
VMBREAK(COP_PREFIX_LDINTERFFTN);

/**
 * <opcode name="pack_varargs" group="Call management instructions">
 *   <operation>Pack a set of arguments for a vararg method call</operation>
 *
 *   <format>prefix<fsep/>pack_varargs<fsep/>first[4]
 *           <fsep/>num[4]<fsep/>signature</format>
 *   <dformat>{pack_varargs}<fsep/>first<fsep/>num<fsep/>signature</dformat>
 *
 *   <form name="pack_varargs" code="COP_PREFIX_PACK_VARARGS"/>
 *
 *   <before>..., arg1, ..., argN</before>
 *   <after>..., array</after>
 *
 *   <description>Pop <i>N</i> words from the stack and pack them
 *   into an array of type <code>System.Object</code>.  The <i>first</i>
 *   value is the index of the first parameter in <i>signature</i>
 *   that corresponds to a word on the stack.  The <i>num</i> value is
 *   the number of logical arguments to be packed.  The final
 *   <i>array</i> is pushed onto the stack as type <code>ptr</code>.
 *   </description>
 *
 *   <notes>The <i>signature</i> value may be either 32 or 64 bits in size,
 *   depending upon the platform, and will usually include a sentinel
 *   marker at position <i>first - 1</i>.  The <i>signature</i> may not
 *   have a sentinel marker if <i>num</i> is zero.<p/>
 *
 *   This instruction is used to pack the arguments for a call to a
 *   <code>vararg</code> method.  The method itself will receive a
 *   single argument containing a pointer to the array.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_PACK_VARARGS):
{
	/* Pack a set of arguments for a vararg method call */
#ifdef IL_CONFIG_VARARGS	

	COPY_STATE_TO_THREAD();
	tempNum = _ILPackCVMStackArgs
		(thread, stacktop, CVMP_ARG_WORD, CVMP_ARG_WORD2,
		 CVMP_ARG_WORD2_PTR(ILType *), (void **)&tempptr);
	RESTORE_STATE_FROM_THREAD();

	stacktop -= tempNum;
	stacktop[0].ptrValue = tempptr;
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD2_PTR, 1);
#else
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD2_PTR, 0);
#endif
}
VMBREAK(COP_PREFIX_PACK_VARARGS);

/**
 *<opcode name="profile_count" group="Profiling Instructions">
 *	<operation>Count the number of times the current method is
 *             invoked</operation>
 *
 * 	<format>profile_count</format>
 * 	<dformat>{profile_count}</dformat>
 *
 * 	<form name="profile_count" code="COP_PREFIX_PROFILE_COUNT"/>
 *
 *  <description>This instruction adds 1 to the profiling count for
 *  the current method.  It is normally inserted at the beginning
 *  of a method's code by the CVM coder when the engine is running
 *  in the profiling mode.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_PROFILE_COUNT):
{
	BEGIN_NATIVE_CALL();	
	ILInterlockedIncrementI4((ILInt32 *)(&method->count));
	END_NATIVE_CALL();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE,0);
}
VMBREAK(COP_PREFIX_PROFILE_COUNT);

/**
 * <opcode name="waddr_native_n" group="Call management instructions">
 *   <operation>Set position <i>n</i> of the native argument buffer
 *              to the address of a local variable</operation>
 *
 *   <format>prefix<fsep/>waddr_native_n<fsep/>N[4]<fsep/>V[4]</format>
 *   <dformat>{waddr_native_n}<fsep/>N<fsep/>V</dformat>
 *
 *   <form name="waddr_native_n" code="COP_PREFIX_WADDR_NATIVE_N"/>
 *
 *   <description>Set position <i>N</i> of the native argument buffer
 *   to the address of local variable <i>V</i>.  For an "InternalCall"
 *   method, 0 is the first argument.  For a "PInvoke" method,
 *   -1 (<i>m1</i>) is the first argument.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_WADDR_NATIVE_N):
{
	/* Set a value within the native argument stack */
	nativeArgs[CVMP_ARG_WORD + 1] = (void *)(&(frame[CVMP_ARG_WORD2]));
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD2, 0);
}
VMBREAK(COP_PREFIX_WADDR_NATIVE_N);

/**
 *<opcode name="trace_in" group="Profiling Instructions">
 *	<operation>Print the name of the called method . This is injected
 *             at the top of every method in --trace mode</operation>
 *
 * 	<format>trace_in<fsep/>reason</format>
 * 	<dformat>{trace_in}<fsep/>reason</dformat>
 *
 * 	<form name="trace_in" code="COP_PREFIX_TRACE_IN"/>
 *
 *  <description>This instruction prints out the called method .  
 *  It is normally inserted immediately before a method call when 
 *  the engine is running in --trace mode. The reason parameter
 *  is provided for future additions , and is ignored in the current
 *  implementation.
 *  </description>
 * </opcode>
 */
VMCASE(COP_PREFIX_TRACE_IN):
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	BEGIN_NATIVE_CALL();

	ILMutexLock(globalTraceMutex);
#ifdef INDENT_TRACE
	int depth = thread->numFrames;
	while(depth--) 
	{
		putc(' ',stdout); 
	}
#endif
	fputs("Entering ", stdout);
	ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), 0,
					 ILMethod_Owner(method),
					 ILMethod_Name(method), method);
	putc('\n',stdout);
	fflush(stdout);
	ILMutexUnlock(globalTraceMutex);

	END_NATIVE_CALL();
#endif
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD, 0);
}
VMBREAK(COP_PREFIX_TRACE_IN);

/**
 *<opcode name="trace_out" group="Profiling Instructions">
 *	<operation>Print the name of the callee method while returning to it. 
 *             This is injected for every return in --trace mode</operation>
 *
 * 	<format>trace_out<fsep/>reason</format>
 * 	<dformat>{trace_out}<fsep/>reason</dformat>
 *
 * 	<form name="trace_out" code="COP_PREFIX_TRACE_OUT"/>
 *
 *  <description>This instruction prints out the callee method 
 *  on returning from the current method.  It is normally inserted 
 *  immediately before a return from a method when the engine is 
 *  running in --trace mode.
 *  </description>
 * </opcode>
 */

VMCASE(COP_PREFIX_TRACE_OUT):
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	// NOTE: at some point of time use the Reason parameter to
	// carry more information about this 
	ILMethod * methodToReturn;

	BEGIN_NATIVE_CALL();	

	methodToReturn = ILExecThreadStackMethod(thread, 1);
	ILMutexLock(globalTraceMutex);
#ifdef INDENT_TRACE
	int depth = thread->numFrames;
	while(depth--) 
	{
		putc(' ',stdout); 
	}
#endif
	fputs("Returning to ", stdout);
	if(methodToReturn)
	{
		ILDumpMethodType(stdout, ILProgramItem_Image(methodToReturn),
						 ILMethod_Signature(methodToReturn), 0,
						 ILMethod_Owner(methodToReturn),
						 ILMethod_Name(methodToReturn), methodToReturn);
	}
	else
	{
		fputs(" (ilrun_main) ",stdout);
	}
	putc('\n',stdout);
	fflush(stdout);
	ILMutexUnlock(globalTraceMutex);

	END_NATIVE_CALL();
#endif
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD, 0);
}
VMBREAK(COP_PREFIX_TRACE_OUT);

/**
 *<opcode name="profile_start" group="Profiling Instructions">
 *	<operation>Record the start performance counter for the current method
 *             invokation</operation>
 *
 * 	<format>profile_start</format>
 * 	<dformat>{profile_start}</dformat>
 *
 * 	<form name="profile_start" code="COP_PREFIX_PROFILE_START"/>
 *
 *  <description>This instruction records the start performance counter for
 *  the current method invokation. It is inserted at the beginning of the
 *  method's code by the CVM coder if the engine is built with enhanced
 *  profiling support and profiling is enabled.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_PROFILE_START):
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#ifdef ENHANCED_PROFILER
	if(callFrame)
	{
		BEGIN_NATIVE_CALL();
		_ILProfilingStart(&(callFrame->profileTime));
		END_NATIVE_CALL();
	}
#endif /* ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE,0);
}
VMBREAK(COP_PREFIX_PROFILE_START);

/**
 *<opcode name="profile_end" group="Profiling Instructions">
 *	<operation>Record the time spend in the method and increase the number of
 *             invokations for the method</operation>
 *
 * 	<format>profile_end</format>
 * 	<dformat>{profile_end}</dformat>
 *
 * 	<form name="profile_end" code="COP_PREFIX_PROFILE_END"/>
 *
 *  <description>This instruction retrieves the end performance counter for
 *  the current method invokation. It adds the difference between start and
 *  end to the total time spend in the method and increases the number of
 *  invokations of the method by one if the engine is built with enhanced
 *  profiling support and profiling is enabled for the current thread.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_PROFILE_END):
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#ifdef ENHANCED_PROFILER
	if(thread->profilingEnabled && callFrame)
	{
		BEGIN_NATIVE_CALL();
		_ILProfilingEnd(method, &(callFrame->profileTime));
		END_NATIVE_CALL();
	}
#endif /* ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE,0);
}
VMBREAK(COP_PREFIX_PROFILE_END);

#endif /* IL_CVM_PREFIX */
