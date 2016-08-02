/*
 * jitc.c - Coder implementation for JIT output.
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


#include "engine_private.h"
#include "il_coder.h"

#ifdef IL_USE_JIT

#include "cctormgr.h"
#include "il_opcodes.h"
#include "il_utils.h"
#ifdef IL_DEBUGGER
#include "debugger.h"
#endif
#ifndef IL_WITHOUT_TOOLS
#include "il_dumpasm.h"
#endif
#include "lib_defs.h"
#include "jitc_gen.h"
#include "interlocked.h"
/* Include header for backtrace support */
#ifdef HAVE_EXECINFO_H
#include <execinfo.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * For enabling extended debugging output uncomment the following define.
 */
/* #define _IL_JIT_ENABLE_DEBUG 1 */

/*
 * For dumping the jitted function before it is compiled uncomment the
 * following define.
 */
/* #define _IL_JIT_DUMP_FUNCTION 1 */

/*
 * For dumping the disassembled jitted function before after  it is compiled
 *  uncomment the following define.
 */
/* #define _IL_JIT_DISASSEMBLE_FUNCTION 1 */

/*
 * To enable parameter / locals optimizations uncomment the following define.
 */
#define _IL_JIT_OPTIMIZE_LOCALS 1

/*
 * To defer the initialization of the locals uncomment the following define.
 */
/* #define _IL_JIT_OPTIMIZE_INIT_LOCALS 1 */

/*
 * To enable method inlining uncomment the following define.
 */
/* #define _IL_JIT_ENABLE_INLINE 1 */

#ifdef _IL_JIT_DUMP_FUNCTION
#ifndef _IL_JIT_ENABLE_DEBUG
#define _IL_JIT_ENABLE_DEBUG 1
#endif
#endif

#ifdef _IL_JIT_DISASSEMBLE_FUNCTION
#ifndef _IL_JIT_ENABLE_DEBUG
#define _IL_JIT_ENABLE_DEBUG 1
#endif
#endif

#ifdef _IL_JIT_ENABLE_DEBUG
#include "jit/jit-dump.h"
#endif

/*
 * Mapping of the native clr types to the corresponing jit types.
 */
static struct _tagILJitTypes _ILJitType_VOID;
static struct _tagILJitTypes _ILJitType_BOOLEAN;
static struct _tagILJitTypes _ILJitType_BYTE;
static struct _tagILJitTypes _ILJitType_CHAR;
static struct _tagILJitTypes _ILJitType_I;
static struct _tagILJitTypes _ILJitType_I2;
static struct _tagILJitTypes _ILJitType_I4;
static struct _tagILJitTypes _ILJitType_I8;
static struct _tagILJitTypes _ILJitType_NFLOAT;
static struct _tagILJitTypes _ILJitType_R4;
static struct _tagILJitTypes _ILJitType_R8;
static struct _tagILJitTypes _ILJitType_SBYTE;
static struct _tagILJitTypes _ILJitType_U;
static struct _tagILJitTypes _ILJitType_U2;
static struct _tagILJitTypes _ILJitType_U4;
static struct _tagILJitTypes _ILJitType_U8;
static struct _tagILJitTypes _ILJitType_VPTR;
static struct _tagILJitTypes _ILJitType_TYPEDREF;

/*
 * Jit type for typed references.
 */
static ILJitType _ILJitTypedRef = 0;

/*
 * Allocate memory for an object that contains object references.
 */
static ILObject *_ILJitAlloc(ILClass *classInfo, ILUInt32 size);

/*
 * Allocate memory for an object that does not contain any object references.
 */
static ILObject *_ILJitAllocAtomic(ILClass *classInfo, ILUInt32 size);

#ifdef	IL_USE_TYPED_ALLOCATION
/*
 * Allocate memory for an object with a gc typedescriptor..
 */
static ILObject *_ILJitAllocTyped(ILClass *classInfo);
#endif	/* IL_USE_TYPED_ALLOCATION */

/*
 * Definition of signatures of internal functions used by jitted code.
 * They have to be kept in sync with the corresponding engine funcions.
 */

/*
 * ILExecThread *ILExecThreadCurrent()
 */
static ILJitType _ILJitSignature_ILExecThreadCurrent = 0;

/*
 * ILObject *_ILJitAlloc(ILClass *classInfo, ILUInt32 size)
 */
static ILJitType _ILJitSignature_ILJitAlloc = 0;

#ifdef	IL_USE_TYPED_ALLOCATION
/*
 * ILObject *_ILJitAllocTyped(ILClass *classInfo);
 */
static ILJitType _ILJitSignature_ILJitAllocTyped = 0;
#endif	/* IL_USE_TYPED_ALLOCATION */

/*
 * System_Array *_ILJitSArrayAlloc(ILClass *arrayClass,
 *									   ILUInt32 numElements,
 *									   ILUInt32 elementSize)
 */
static ILJitType _ILJitSignature_ILJitSArrayAlloc = 0;

/*
 * System_String *_ILJitStringAlloc(ILClass *stringClass,
 *									ILUInt32 numChars)
 */
static ILJitType _ILJitSignature_ILJitStringAlloc = 0;

/*
 * System_Array *_ILJitGetExceptionStackTrace(ILExecThread *thread)
 */
static ILJitType _ILJitSignature_ILJitGetExceptionStackTrace = 0;

/*
 * void ILRuntimeExceptionRethrow(ILObject *exception)
 */
static ILJitType _ILJitSignature_ILRuntimeExceptionRethrow = 0;

/*
 * void ILRuntimeExceptionThrow(ILObject *exception)
 */
static ILJitType _ILJitSignature_ILRuntimeExceptionThrow = 0;

/*
 * void ILRuntimeExceptionThrowClass(ILClass *classInfo)
 */
static ILJitType _ILJitSignature_ILRuntimeExceptionThrowClass = 0;

/*
 * void ILRuntimeExceptionThrowOutOfMemory()
 */
static ILJitType _ILJitSignature_ILRuntimeExceptionThrowOutOfMemory = 0;


/*
 * static void *_ILRuntimeLookupInterfaceMethod(ILClassPrivate *objectClassPrivate,
 *												ILClass *interfaceClass,
 *												ILUInt32 index)
 */
static ILJitType _ILJitSignature_ILRuntimeLookupInterfaceMethod = 0;

/*
 * ILInt32 ILRuntimeCanCastClass(ILMethod *method, ILObject *object, ILClass *toClass)
 *
 */
static ILJitType _ILJitSignature_ILRuntimeCanCastClass = 0;

/*
 * ILInt32 ILRuntimeClassImplements(ILObject *object, ILClass *toClass)
 */
static ILJitType _ILJitSignature_ILRuntimeClassImplements = 0;

/*
 * void *ILRuntimeGetThreadStatic(ILExecThread *thread,
 *							   ILUInt32 slot, ILUInt32 size)
 */
static ILJitType _ILJitSignature_ILRuntimeGetThreadStatic = 0;

/*
 * void jit_exception_clear_last()
 */
static ILJitType _ILJitSignature_JitExceptionClearLast = 0;

/*
 * void *malloc(size_t nbytes)
 * This signature is used for GCAlloc and GCAllocAtomic too.
 */
static ILJitType _ILJitSignature_malloc = 0;

#ifdef IL_JIT_FNPTR_ILMETHOD
/*
 * void *ILRuntimeMethodToVtablePointer(ILMethod *method)
 */
static ILJitType _ILJitSignature_ILRuntimeMethodToVtablePointer = 0;
#endif

/*
 * Definition of the signatures for inlined calls of native runtime functions.
 */

/*
 * void _IL_Monitor_Enter(ILExecThread *thread, ILObject *obj)
 */
static ILJitType _ILJitSignature_ILMonitorEnter = 0;

/*
 * void _IL_Monitor_Exit(ILExecThread *thread, ILObject *obj)
 */
static ILJitType _ILJitSignature_ILMonitorExit = 0;

/*
 * void _ILGetClrType(ILExecThread *thread, ILClass *info)
 */
static ILJitType _ILJitSignature_ILGetClrType = 0;

/*
 * void ILRuntimeHandleManagedSafePointFlags(ILExecThread *thread)
 */
static ILJitType _ILJitSignature_ILRuntimeHandleManagedSafePointFlags = 0;

/*
 * char *ILStringToAnsi(ILExecThread *thread, ILString *str)
 */
static ILJitType _ILJitSignature_ILStringToAnsi = 0;

/*
 * char *ILStringToUTF8(ILExecThread *thread, ILString *str)
 */
static ILJitType _ILJitSignature_ILStringToUTF8 = 0;

/*
 * char *ILStringToUTF16(ILExecThread *thread, ILString *str)
 */
static ILJitType _ILJitSignature_ILStringToUTF16 = 0;

/* 
 * ILString *ILStringCreate(ILExecThread *thread, const char *str)
 */
static ILJitType _ILJitSignature_ILStringCreate = 0;

/* 
 * ILString *ILStringCreateUTF8(ILExecThread *thread, const char *str)
 */
static ILJitType _ILJitSignature_ILStringCreateUTF8 = 0;

/* 
 * ILString *ILStringWCreate(ILExecThread *thread, const ILUInt16 *str)
 */
static ILJitType _ILJitSignature_ILStringWCreate = 0;

/*
 * void *ILJitDelegateGetClosure(ILObject *delegate, ILType *delType)
 */
static ILJitType _ILJitSignature_ILJitDelegateGetClosure = 0;

/*
 * void* MarshalObjectToCustom(void *value, ILType *type, ILMethod *method, ILExecThread *thread)
 */
static ILJitType _ILJitSignature_MarshalObjectToCustom = 0;

/*
 * void* MarshalCustomToObject(void *value, ILType *type, ILMethod *method, ILExecThread *thread)
 */
static ILJitType _ILJitSignature_MarshalCustomToObject = 0;

/*
 * ILInt32 ILInterlockedIncrement(ILInt32 *destination)
 */
static ILJitType _ILJitSignature_ILInterlockedIncrement = 0;

/*
 * ILInt32 ILSArrayCopy_AAI4(ILExecThread *thread, System_Array *array1,
 *							 System_Array *array2, ILInt32 length,
 *							 ILInt32 elementSize)
 */
static ILJitType _ILJitSignature_ILSArrayCopy_AAI4 = 0;

/*
 * ILInt32 ILSArrayCopy_AI4AI4I4(ILExecThread *thread,
 *								 System_Array *array1, ILInt32 index1,
 *								 System_Array *array2, ILInt32 index2,
 *								 ILInt32 length,
 *								 ILInt32 elementSize)
 */
static ILJitType _ILJitSignature_ILSArrayCopy_AI4AI4I4 = 0;

/*
 * ILInt32 ILSArrayClear_AI4I4(ILExecThread *thread, System_Array *array,
 *							   ILInt32 index, ILInt32 length,
 *							   ILInt32 elementSize)
 */
static ILJitType _ILJitSignature_ILSArrayClear_AI4I4 = 0;

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#ifdef ENHANCED_PROFILER
/*
 * void _ILProfilingStart(ILCurrTime *timestamp)
 */
static ILJitType _ILJitSignature_ILProfilingStart = 0;

/*
 * void _ILProfilingEnd(ILMethod *method, ILCurrTime *startTimestamp)
 */
static ILJitType _ILJitSignature_ILProfilingEnd = 0;
#endif /* ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */

/*
 * void ILJitTraceIn(ILExecThread *thread, ILMethod *method)
 * void ILJitTraceOut(ILExecThread *thread, ILMethod *method)
 */
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
static ILJitType _ILJitSignature_ILJitTraceInOut = 0;
#endif

/*
 * Define offsetof macro if not present.
 */
#ifndef offsetof
#define offsetof(struct_type, member) \
          (size_t) &(((struct_type *)0)->member)
#endif

/*
 * Handle locked methods in the jit coder.
 */
static void *JITCoder_HandleLockedMethod(ILCoder *coder, ILMethod *method);

#define	IL_JITC_DECLARATIONS
#include "jitc_locals.c"
#include "jitc_stack.c"
#include "jitc_labels.c"
#include "jitc_inline.c"
#include "jitc_except.c"
#include "jitc_alloc.c"
#include "jitc_array.c"
#include "jitc_string.c"
#include "jitc_call.c"
#include "jitc_delegate.c"
#include "jitc_math.c"
#include "jitc_profile.c"
#undef	IL_JITC_DECLARATIONS

#define _IL_JIT_IMPL_DEFAULT		0x000
#define _IL_JIT_IMPL_INTERNAL		0x001
#define _IL_JIT_IMPL_INTERNALALLOC	0x002
#define _IL_JIT_IMPL_INTERNALMASK	0x003
#define _IL_JIT_IMPL_PINVOKE		0x004
#define _IL_JIT_IMPL_MASK			0x0FF
#define _IL_JIT_IMPL_NOINLINE		0x100
#define _IL_JIT_IMPL_INLINE			0x200
#define _IL_JIT_IMPL_INLINE_MASK	0x300

/*
 * Error codes stored in fnInfo.func in case a library or method was not
 * found
 */
#define _IL_JIT_PINVOKE_DLLNOTFOUND			((void *)0x01)
#define _IL_JIT_PINVOKE_ENTRYPOINTNOTFOUND	((void *)0x02)
#define _IL_JIT_PINVOKE_ERRORMASK			((void *)0x03)

#ifdef IL_NATIVE_INT64
#define _ILJitPinvokeError(fnInfo)	\
	((((ILInt64)(fnInfo).un.func) & ~((ILInt64)_IL_JIT_PINVOKE_ERRORMASK)) == 0)
#endif
#ifdef IL_NATIVE_INT32
#define _ILJitPinvokeError(fnInfo)	\
	((((ILInt32)(fnInfo).un.func) & ~((ILInt32)_IL_JIT_PINVOKE_ERRORMASK)) == 0)
#endif
	
/*
 * Define the structure of a JIT coder's instance block.
 */
struct _tagILJITCoder
{
	ILCoder			coder;
	ILExecProcess  *process;
	jit_context_t   context;

	int				debugEnabled;
	int				flags;

	/* Pool for the method infos. */
	ILMemPool		methodPool;

#define	IL_JITC_CODER_INSTANCE
#include "jitc_inline.c"
#include "jitc_locals.c"
#include "jitc_stack.c"
#include "jitc_labels.c"
#include "jitc_profile.c"
#include "jitc_except.c"
#undef	IL_JITC_CODER_INSTANCE

	/* The current jitted function. */
	ILJitFunction	jitFunction;

	/* The manager for running the required cctors. */
	ILCCtorMgr		cctorMgr;

	/* The optimization level used by the coder */
	ILUInt32		optimizationLevel;

#ifndef IL_JIT_THREAD_IN_SIGNATURE
	/* cache for the current thread. */
	ILJitValue		thread;
#endif

#ifdef IL_DEBUGGER
	/* Flag if current method can be debugged */
	int markBreakpoints;
#endif
};

/*
 * Convert a pointer to an ILCoder to a pointer to the ILJITCoder instance
 */
#define _ILCoderToILJITCoder(coder) ((ILJITCoder *)coder)

/*
 * Generate the code to allocate the memory for an object.
 * Returns the ILJitValue with the pointer to the new object.
 */
static ILJitValue _ILJitAllocGen(ILJitFunction jitFunction, ILClass *classInfo,
								 ILUInt32 size);

/*
 * Generate the code to allocate the memory for an object.
 * Returns the ILJitValue with the pointer to the new object.
 */
static ILJitValue _ILJitAllocObjectGen(ILJitFunction jitFunction, ILClass *classInfo);

#ifdef IL_JIT_THREAD_IN_SIGNATURE
#define _ILJitCoderGetThread(coder)		jit_value_get_param((coder)->jitFunction, 0)
#define _ILJitFunctionGetThread(func)	jit_value_get_param((func), 0)
#else
static ILJitValue _ILJitFunctionGetThread(ILJitFunction func)
{
	return jit_insn_call_native(func, "ILExecThreadCurrent",
										ILExecThreadCurrent,
										_ILJitSignature_ILExecThreadCurrent,
										0, 0, JIT_CALL_NOTHROW);
}

static ILJitValue _ILJitCoderGetThread(ILJITCoder *jitCoder)
{
	if(!(jitCoder->thread))
	{
		ILJitValue thread;

		if(!(jitCoder->isInCatcher))
		{
			jit_label_t startLabel = jit_label_undefined;
			jit_label_t endLabel = jit_label_undefined;
			
			jit_insn_label(jitCoder->jitFunction, &startLabel);
			thread = _ILJitFunctionGetThread(jitCoder->jitFunction);
			jit_insn_label(jitCoder->jitFunction, &endLabel);
			jit_insn_move_blocks_to_start(jitCoder->jitFunction, startLabel,
																 endLabel);
		}
		else
		{
			thread = _ILJitFunctionGetThread(jitCoder->jitFunction);
		}
		jitCoder->thread = thread;
	}
	return jitCoder->thread;
}
#endif

/*
 * Initialize a ILJitTypes base structure 
 */
#define _ILJitTypesInitBase(jitTypes, jitType) \
	{ \
		(jitTypes)->jitTypeBase = (jitType); \
		(jitTypes)->jitTypeKind = 0; \
	}

/*
 * Check if the typeKind is a floating point number.
 */
#define _JIT_TYPEKIND_IS_FLOAT(typeKind) \
((typeKind == JIT_TYPE_FLOAT32)	|| \
 (typeKind == JIT_TYPE_FLOAT64)	|| \
 (typeKind == JIT_TYPE_NFLOAT))

/*
 * Check if the typeKind is an int (<=32 bit value).
 */
#ifdef IL_NATIVE_INT64
#define _JIT_TYPEKIND_IS_INT(typeKind) \
((typeKind == JIT_TYPE_INT)		|| \
 (typeKind == JIT_TYPE_UINT)	|| \
 (typeKind == JIT_TYPE_SHORT)	|| \
 (typeKind == JIT_TYPE_USHORT)	|| \
 (typeKind == JIT_TYPE_SBYTE)	|| \
 (typeKind == JIT_TYPE_UBYTE))
#else
#define _JIT_TYPEKIND_IS_INT(typeKind) \
((typeKind == JIT_TYPE_NINT)	|| \
 (typeKind == JIT_TYPE_NUINT)	|| \
 (typeKind == JIT_TYPE_INT)		|| \
 (typeKind == JIT_TYPE_UINT)	|| \
 (typeKind == JIT_TYPE_SHORT)	|| \
 (typeKind == JIT_TYPE_USHORT)	|| \
 (typeKind == JIT_TYPE_SBYTE)	|| \
 (typeKind == JIT_TYPE_UBYTE)	|| \
 (typeKind == JIT_TYPE_PTR))
#endif

/*
 * Check if the typeKind is a long (64 bit value).
 */
#ifdef IL_NATIVE_INT64
#define _JIT_TYPEKIND_IS_LONG(typeKind) \
((typeKind == JIT_TYPE_LONG)	|| \
 (typeKind == JIT_TYPE_ULONG)	|| \
 (typeKind == JIT_TYPE_NINT)	|| \
 (typeKind == JIT_TYPE_NUINT)	|| \
 (typeKind == JIT_TYPE_PTR))
#else
#define _JIT_TYPEKIND_IS_LONG(typeKind) \
((typeKind == JIT_TYPE_LONG)	|| \
 (typeKind == JIT_TYPE_ULONG))
#endif

/*
 * Check if the typeKind is unsigned.
 */
#define _JIT_TYPEKIND_IS_UNSIGNED(typeKind) \
((typeKind == JIT_TYPE_ULONG)	|| \
 (typeKind == JIT_TYPE_NUINT)	|| \
 (typeKind == JIT_TYPE_UINT)	|| \
 (typeKind == JIT_TYPE_USHORT)	|| \
 (typeKind == JIT_TYPE_UBYTE))

/*
 * Check if the typeKind is signed.
 */
#define _JIT_TYPEKIND_IS_SIGNED(typeKind) \
((typeKind == JIT_TYPE_LONG)	|| \
 (typeKind == JIT_TYPE_NINT)	|| \
 (typeKind == JIT_TYPE_INT)	|| \
 (typeKind == JIT_TYPE_SHORT)	|| \
 (typeKind == JIT_TYPE_SBYTE))

/*
 * Check if the typeKind is a pointer type.
 */
#define _JIT_TYPEKIND_IS_POINTER(typeKind) \
(typeKind == JIT_TYPE_PTR)

/*
 * Get the evaluation stack type for the given ILJitType.
 */
static ILJitType _ILJitTypeToStackType(ILJitType type)
{
	ILJitType jitType = jit_type_promote_int(type);
	int typeKind = jit_type_get_kind(jitType);


	if(_JIT_TYPEKIND_IS_UNSIGNED(typeKind))
	{
		if(_JIT_TYPEKIND_IS_LONG(typeKind))
		{
			jitType = _IL_JIT_TYPE_INT64;
		}
		else if(typeKind == JIT_TYPE_NUINT)
		{
			jitType = _IL_JIT_TYPE_NINT;
		}
		else
		{
			jitType = _IL_JIT_TYPE_INT32;
		}
	}
	return jitType;
}

/*
 * Convert a value to the corresponding signed/unsigned type.
 * Returns 1 if the value is converted 0 otherwise.
 */
static int AdjustSign(ILJitFunction func, ILJitValue *value, int toUnsigned,
															  int checkOverflow)
{
	ILJitType type = jit_value_get_type(*value);
	int typeKind = jit_type_get_kind(type);
	
	if(_JIT_TYPEKIND_IS_SIGNED(typeKind) && toUnsigned)
	{
		switch(typeKind)
		{
			case JIT_TYPE_SBYTE:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_BYTE,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_SHORT:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_UINT16,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_INT:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_UINT32,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_NINT:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_NUINT,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_LONG:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_UINT64,
														checkOverflow);
				return 1;
			}
			break;
		}
	}
	else if(_JIT_TYPEKIND_IS_UNSIGNED(typeKind) && !toUnsigned)
	{
		switch(typeKind)
		{
			case JIT_TYPE_UBYTE:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_SBYTE,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_USHORT:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_INT16,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_UINT:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_INT32,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_NUINT:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_NINT,
														checkOverflow);
				return 1;
			}
			break;

			case JIT_TYPE_ULONG:
			{
				*value = jit_insn_convert(func, *value, _IL_JIT_TYPE_INT64,
														checkOverflow);
				return 1;
			}
			break;
		}
	}
	return 0;
}

/*
 * Do the implicit conversion of an ILJitValue to the given target type.
 * The value to convert is on the stack in it's source type. This means that no
 * implicit conversion to the stacktype (either INT32, INT64 or a pointer type
 * was done.
 * Because of this we have to take into account that unsigned values with a
 * length < 4 should have been zero extended to a size of an INT32 and signed
 * values would have been sign extended.
 * This means we have the following versions:
 * 1. Target size is <= 4 bytes:
 *    We can convert directly to the target type because either the source
 *    value is truncated if it's long or the sign is changed if it's int or
 *    unsigned int or it would have been extended to 4 byted depending on it's
 *    own sign.
 * 2. Both values are signed or both values are unsigned:
 *    we can convert directly to the target type.
 */
static ILJitValue _ILJitValueConvertImplicit(ILJitFunction func,
											 ILJitValue value,
											 ILJitType targetType)
{
	ILJitType sourceType = jit_value_get_type(value);
	int sourceTypeKind;
	int targetTypeKind;

	if(sourceType == targetType)
	{
		/* Nothing to do here. */
		return value;
	}
	if(jit_type_is_struct(sourceType) || jit_type_is_union(sourceType))
	{
		/* something is wrong here. */
		return value;
	}
	sourceTypeKind = jit_type_get_kind(sourceType);
	targetTypeKind = jit_type_get_kind(targetType);
	if(_JIT_TYPEKIND_IS_FLOAT(sourceTypeKind))
	{
		/* We can convert these values directly */
		return jit_insn_convert(func, value, targetType, 0);
	}
	else if(_JIT_TYPEKIND_IS_FLOAT(targetTypeKind))
	{
		if(_JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind))
		{
			int sourceSize = jit_type_get_size(sourceType);

			if(sourceSize >= 4)
			{
				/* We have to convert the value to signed first because we */
				/* don't know the value of the sign bit. */
				AdjustSign(func, &value, 0, 0);
			}
		}
		return jit_insn_convert(func, value, targetType, 0);
	}
	else if(_JIT_TYPEKIND_IS_INT(targetTypeKind))
	{
		/* We can convert this directly directly because the source value is */
		/* either truncated or expanded to the target type depending on the */
		/* sign of the source value. */
		return jit_insn_convert(func, value, targetType, 0);
	}
	else if(_JIT_TYPEKIND_IS_LONG(targetTypeKind))
	{
		if(_JIT_TYPEKIND_IS_LONG(sourceTypeKind))
		{
			/* we can convert this directly because only the sign is changed. */
			return jit_insn_convert(func, value, targetType, 0);
		}
		if(_JIT_TYPEKIND_IS_INT(sourceTypeKind))
		{
			int sourceIsSigned = _JIT_TYPEKIND_IS_SIGNED(sourceTypeKind);
			int targetIsSigned = _JIT_TYPEKIND_IS_SIGNED(targetTypeKind);
			int sourceIsUnsigned = _JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind);
			int targetIsUnsigned = _JIT_TYPEKIND_IS_UNSIGNED(targetTypeKind);

			if((sourceIsSigned && targetIsSigned) ||
			  (sourceIsUnsigned && targetIsUnsigned))
			{
				/* The signs match so we can convert directly. */
				return jit_insn_convert(func, value, targetType, 0);
			}
			else if(sourceIsUnsigned && targetIsSigned)
			{
				int sourceSize = jit_type_get_size(sourceType);

				if(sourceSize < 4)
				{
					/* The source value is 0 extended to INT32 and then */
					/* extended to INT64. */
					/* Because the source value is less than 4 bytes we */
					/* know that the sign will be 0 on the 32 bit value. */
					return jit_insn_convert(func, value, targetType, 0);
				}
				else
				{
					/* Because we don't know the sign of the 32 bit value we */
					/* have to convert it to INT32 first. */
					value = jit_insn_convert(func, value, _IL_JIT_TYPE_INT32, 0);
					return jit_insn_convert(func, value, targetType, 0);
				}
			}
			else if(sourceIsSigned && targetIsUnsigned)
			{
				/* In this case we'll allways have to do two conversions. */
				/* We have to convert it to UINT32 first. */
				value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, 0);
				return jit_insn_convert(func, value, targetType, 0);
			}
		}
	}
	/* All other values can be converted directly. */
	return jit_insn_convert(func, value, targetType, 0);
}

/*
 * Do the explicit conversion of an ILJitValue to the given target type.
 * The value to convert is on the stack in it's source type. This means that no
 * implicit conversion to the stacktype (either INT32, INT64 or a pointer type
 * was done.
 * Because of this we have to take into account that unsigned values with a
 * length < 4 should have been zero extended to a size of an INT32 and signed
 * values would have been sign extended.
 */
static ILJitValue _ILJitValueConvertExplicit(ILJitFunction func,
											 ILJitValue value,
											 ILJitType targetType,
											 int isUnsigned,
											 int overflowCheck)
{
	ILJitType sourceType = jit_value_get_type(value);
	int sourceTypeKind;
	int targetTypeKind;

	if(sourceType == targetType)
	{
		/* Nothing to do here. */
		return value;
	}
	if(jit_type_is_struct(sourceType) || jit_type_is_union(sourceType))
	{
		/* something is wrong here. */
		return value;
	}
	if(!isUnsigned && !overflowCheck)
	{
		/* This is just like the implicit type conversion. */
		return _ILJitValueConvertImplicit(func,
										  value,
										  targetType);
	}
	else if(!isUnsigned)
	{
		/* Do a signed type conversion with overflow check. */
		/* This is just like the implicit type conversion with overflow check. */
		sourceTypeKind = jit_type_get_kind(sourceType);
		targetTypeKind = jit_type_get_kind(targetType);
		if(_JIT_TYPEKIND_IS_FLOAT(sourceTypeKind))
		{
			/* We can convert these values directly */
			return jit_insn_convert(func, value, targetType, overflowCheck);
		}
		else if(_JIT_TYPEKIND_IS_FLOAT(targetTypeKind))
		{
			if(_JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind))
			{
				int sourceSize = jit_type_get_size(sourceType);

				if(sourceSize >= 4)
				{
					/* We have to convert the value to signed first because we */
					/* don't know the value of the sign bit. */
					AdjustSign(func, &value, 0, 0);
				}
			}
			return jit_insn_convert(func, value, targetType, 1);
		}
		else if(_JIT_TYPEKIND_IS_INT(targetTypeKind))
		{
			/* We have to check if the sign has to be adjusted. */
			if(_JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind))
			{
				int sourceSize = jit_type_get_size(sourceType);

				if(sourceSize >= 4)
				{
					/* We have to convert the value to signed first because we */
					/* don't know the value of the sign bit. */
					AdjustSign(func, &value, 0, 0);
				}
			}
			return jit_insn_convert(func, value, targetType, overflowCheck);
		}
		else if(_JIT_TYPEKIND_IS_LONG(targetTypeKind))
		{
			if(_JIT_TYPEKIND_IS_LONG(sourceTypeKind))
			{
				/* If the source value is unsigned we have to convert to */
				/* signed first. */
				if(_JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind))
				{
					AdjustSign(func, &value, 0, 0);
				}
				return jit_insn_convert(func, value, targetType, overflowCheck);
			}
			if(_JIT_TYPEKIND_IS_INT(sourceTypeKind))
			{
				int sourceIsSigned = _JIT_TYPEKIND_IS_SIGNED(sourceTypeKind);
				int targetIsSigned = _JIT_TYPEKIND_IS_SIGNED(targetTypeKind);
				int sourceIsUnsigned = _JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind);
				int targetIsUnsigned = _JIT_TYPEKIND_IS_UNSIGNED(targetTypeKind);

				if(sourceIsSigned && targetIsSigned)
				{
					/* Both values are signed and the source type is smaller */
					/* than the target type. So there won't be an overflow. */
					return jit_insn_convert(func, value, targetType, 0);
				}
				else if(sourceIsUnsigned && targetIsUnsigned)
				{
					int sourceSize = jit_type_get_size(sourceType);

					if(sourceSize < 4)
					{
						/* We know that the sign bit is 0 on the stack. */
						/* So we can convert directly without overflow check. */
						/* don't know the value of the sign bit. */
						return jit_insn_convert(func, value, targetType, 0);
					}

					/* TODO */
					/* This case is problematic right now because this type */
					/* of conversion with overflow check can't be handled  */
					/* correctly with libjit atm. */
					/* We should make the source value signed first before */
					/* the conversion with overflow check is done. */
					/* AdjustSign(func, &value, 0, 0); */
					return jit_insn_convert(func, value, targetType, overflowCheck);
				}
				else if(sourceIsUnsigned && targetIsSigned)
				{
					int sourceSize = jit_type_get_size(sourceType);

					if(sourceSize < 4)
					{
						/* The source value is 0 extended to INT32 and then */
						/* extended to INT64. */
						/* Because the source value is less than 4 bytes we */
						/* know that the sign will be 0 on the 32 bit value */
						/* and no overflow is possible. */
						return jit_insn_convert(func, value, targetType, 0);
					}
					else
					{
						/* Because we don't know the sign of the 32 bit value we */
						/* have to convert it to INT32 first. */
						value = jit_insn_convert(func, value, _IL_JIT_TYPE_INT32, 0);
						return jit_insn_convert(func, value, targetType, overflowCheck);
					}
				}
				else if(sourceIsSigned && targetIsUnsigned)
				{
					/* TODO */
					/* This case is problematic right now because this type */
					/* of conversion with overflow check can't be handled  */
					/* correctly with libjit atm. */
					/* We should make the source value signed first before */
					/* the conversion with overflow check is done. */
					/* value = jit_insn_convert(func, value, _IL_JIT_TYPE_INT32, 0); */
					/* to get the correct result we convert to UINT32 with */
					/* overflowCheck instead. */
					value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, overflowCheck);
					return jit_insn_convert(func, value, targetType, overflowCheck);
				}
			}
		}
		/* We have to check if the sign has to be adjusted before the  */
		/* conversion is done. */
		if(_JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind))
		{
			int sourceSize = jit_type_get_size(sourceType);

			if(sourceSize >= 4)
			{
				/* We have to convert the value to signed first because we */
				/* don't know the value of the sign bit. */
				AdjustSign(func, &value, 0, 0);
			}
		}
		return jit_insn_convert(func, value, targetType, overflowCheck);
	}
	else
	{
		/* We do an unsigned conversion. */
		sourceTypeKind = jit_type_get_kind(sourceType);
		targetTypeKind = jit_type_get_kind(targetType);
		if(_JIT_TYPEKIND_IS_FLOAT(sourceTypeKind))
		{
			/* We can convert these values directly */
			return jit_insn_convert(func, value, targetType, overflowCheck);
		}
		else if(_JIT_TYPEKIND_IS_FLOAT(targetTypeKind))
		{
			/* We have to adjust the sign first and sign extend the source */
			/* value to at least 4 bytes without overflow check. */
			if(_JIT_TYPEKIND_IS_SIGNED(sourceTypeKind))
			{
				int sourceSize = jit_type_get_size(sourceType);

				if(sourceSize <= 4)
				{
					/* We have to convert the value to UInt32 first. */
					value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, 0);
				}
				else if(sourceSize == 8)
				{
					/* We have to convert the value to UInt64 first. */
					value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT64, 0);
				}
			}
			return jit_insn_convert(func, value, targetType, overflowCheck);
		}
		else if(_JIT_TYPEKIND_IS_INT(targetTypeKind))
		{
			if(!overflowCheck)
			{
				/* We can convert this directly directly because the source value is */
				/* either truncated or expanded to the target type depending on the */
				/* sign of the source value. */
				return jit_insn_convert(func, value, targetType, 0);
			}
			else
			{
				/* We have to adjust the sign first and sign extend the source */
				/* value to at least 4 bytes without overflow check. */
				if(_JIT_TYPEKIND_IS_SIGNED(sourceTypeKind))
				{
					int sourceSize = jit_type_get_size(sourceType);

					if(sourceSize <= 4)
					{
						/* We have to convert the value to UInt32 first. */
						value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, 0);
					}
					else if(sourceSize == 8)
					{
						/* We have to convert the value to UInt64 first. */
						value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT64, 0);
					}
				}
				return jit_insn_convert(func, value, targetType, overflowCheck);
			}
		}
		else if(_JIT_TYPEKIND_IS_LONG(targetTypeKind))
		{
			if(_JIT_TYPEKIND_IS_LONG(sourceTypeKind))
			{
				if(!overflowCheck)
				{
					/* we can convert this directly because only the sign is changed. */
					return jit_insn_convert(func, value, targetType, 0);
				}
				else
				{
					/* We have to adjust the sign of the source value first. */
					if(_JIT_TYPEKIND_IS_SIGNED(sourceTypeKind))
					{
						/* We have to convert the value to UInt64 first. */
						value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT64, 0);
					}
					return jit_insn_convert(func, value, targetType, overflowCheck);
				}
			}
			if(_JIT_TYPEKIND_IS_INT(sourceTypeKind))
			{
				int sourceIsSigned = _JIT_TYPEKIND_IS_SIGNED(sourceTypeKind);
				int targetIsSigned = _JIT_TYPEKIND_IS_SIGNED(targetTypeKind);
				int sourceIsUnsigned = _JIT_TYPEKIND_IS_UNSIGNED(sourceTypeKind);
				int targetIsUnsigned = _JIT_TYPEKIND_IS_UNSIGNED(targetTypeKind);

				if(sourceIsSigned && targetIsSigned)
				{
					if(!overflowCheck)
					{
						/* The signs match so we can convert directly. */
						return jit_insn_convert(func, value, targetType, 0);
					}
					else
					{
						/* We have to adjust the sign and sign extend the */
						/* source value first to 4 bytes. */
						value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, overflowCheck);
						return jit_insn_convert(func, value, targetType, overflowCheck);
					}
				}
				else if (sourceIsUnsigned && targetIsUnsigned)
				{
					/* The signs match so we can convert directly without overflow check. */
					return jit_insn_convert(func, value, targetType, 0);
				}
				else if(sourceIsUnsigned && targetIsSigned)
				{
					int sourceSize = jit_type_get_size(sourceType);

					if(sourceSize < 4)
					{
						/* The source value is 0 extended to INT32 and then */
						/* extended to INT64. */
						/* Because the source value is less than 4 bytes we */
						/* know that the sign will be 0 on the 32 bit value. */
						return jit_insn_convert(func, value, targetType, 0);
					}
					else
					{
						/* Because we don't know the sign of the 32 bit */
						/* value we have to convert it to INT32 first. */
						value = jit_insn_convert(func, value, _IL_JIT_TYPE_INT32, overflowCheck);
						/* Because the signed value will be extended we */
						/* don't have to check for overflow now. */
						return jit_insn_convert(func, value, targetType, 0);
					}
				}
				else if(sourceIsSigned && targetIsUnsigned)
				{
					/* In this case we'll allways have to do two conversions. */
					/* We have to convert it to UINT32 without overflow check */
					/* first. */
					value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, 0);
					return jit_insn_convert(func, value, targetType, 0);
				}
			}
		}
		/* We have to adjust the sign first and sign extend the source */
		/* value to at least 4 bytes without overflow check. */
		if(_JIT_TYPEKIND_IS_SIGNED(sourceTypeKind))
		{
			int sourceSize = jit_type_get_size(sourceType);

			if(sourceSize <= 4)
			{
				/* We have to convert the value to UInt32 first. */
				value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT32, 0);
			}
			else if(sourceSize == 8)
			{
				/* We have to convert the value to UInt64 first. */
				value = jit_insn_convert(func, value, _IL_JIT_TYPE_UINT64, 0);
			}
		}
		return jit_insn_convert(func, value, targetType, 0);
	}
	return value;
}

/*
 * Convert the given ILJitValue to the type needed on the evaluation stack.
 * When no conversion is needed the value is returned as it is.
 */
static ILJitValue _ILJitValueConvertToStackType(ILJitFunction func,
												ILJitValue value)
{
	ILJitValue temp = value;
	ILJitType type = jit_value_get_type(temp);
	ILJitType stackType = _ILJitTypeToStackType(type);

	if(type != stackType)
	{
		temp = jit_insn_convert(func, temp, stackType, 0);
	}

	/* We have only signed values on the stack. */
	AdjustSign(func, &temp, 0, 0);

	return temp;
}

/*
 * Readjust the stack to normalize binary operands when
 * I and I4 are mixed together.  Also determine which of
 * I4 or I8 to use if the operation involves I.
 * The verifier makes sure that no invalid combinations are on the stack.
 * That means: 
 * 1. If one value is a pointer then the second one is a pointer too.
 * 2. If one value is a float then the second one is a float too.
 */
static void AdjustMixedBinary(ILJITCoder *coder, int isUnsigned,
							  ILJitValue *value1, ILJitValue *value2)
{
	ILJitType type1 = jit_value_get_type(*value1);
	ILJitType type2 = jit_value_get_type(*value2);
	ILJitType newType = 0;
	int type1Kind = jit_type_get_kind(type1);
	int type2Kind = jit_type_get_kind(type2);
	int type1IsFloat = _JIT_TYPEKIND_IS_FLOAT(type1Kind);
	int type2IsFloat = _JIT_TYPEKIND_IS_FLOAT(type2Kind);
	int type1IsPointer = _JIT_TYPEKIND_IS_POINTER(type1Kind);
	int type2IsPointer = _JIT_TYPEKIND_IS_POINTER(type2Kind);
	int type1IsLong = _JIT_TYPEKIND_IS_LONG(type1Kind);
	int type2IsLong = _JIT_TYPEKIND_IS_LONG(type2Kind);

	if(type1IsFloat || type2IsFloat)
	{
		/* Nothing to do here. */
		return;
	}
	else if(type1IsLong || type2IsLong)
	{
		/* If the arguments mix I8 and I4, then cast the I4 value to I8 */
		if(isUnsigned)
		{
			newType = _IL_JIT_TYPE_UINT64;
		}
		else
		{
			newType = _IL_JIT_TYPE_INT64;
		}
	}
	else if(type1IsPointer || type2IsPointer)
	{
		if(isUnsigned)
		{
			newType = _IL_JIT_TYPE_NUINT;
		}
		else
		{
			newType = _IL_JIT_TYPE_NINT;
		}
	}
	else
	{
		/* We have only 32 bit values left. */
		if(isUnsigned)
		{
			newType = _IL_JIT_TYPE_UINT32;
		}
		else
		{
			newType = _IL_JIT_TYPE_INT32;
		}
	}
	
	/* now do the conversion if necessairy. */
	if(type1 != newType)
	{
		*value1 = _ILJitValueConvertImplicit(coder->jitFunction, *value1,
											 newType);
	}
	if(type2 != newType)
	{
		*value2 = _ILJitValueConvertImplicit(coder->jitFunction, *value2,
											 newType);
	}
}

/*
 * Perform a class layout.
 */
static int _LayoutClass(ILExecThread *thread, ILClass *info)
{
	/* Check if the class is allready layouted. */
	if((info->userData) && !(((ILClassPrivate *)(info->userData))->inLayout))
	{
		return 1;
	}
	return _ILLayoutClass(_ILExecThreadProcess(thread), info);
}

/*
 * Function to free the metadata attached to a jit_function_t.
 */
static void _ILJitMetaFreeFunc(void *data)
{
	if(data)
	{
		ILFree(data);
	}
}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
/*
 * Get a complete methodname.
 * The string returned must be freed by the caller.
 */
static char *_ILJitGetMethodName(ILMethod *method)
{
	const char *methodName = ILMethod_Name(method);
	const char *className = ILClass_Name(ILMethod_Owner(method));
	const char *namespaceName = ILClass_Namespace(ILMethod_Owner(method));
	int namespaceLen = 0;
	int classLen = 0;
	int methodLen = 0;
	int len = 0;
	char *fullName = 0;

	if(namespaceName)
	{
		namespaceLen = strlen(namespaceName);
		len += namespaceLen + 1;
	}
	if(className)
	{
		classLen = strlen(className);
		len += classLen + 1;
	}
	if(methodName)
	{
		methodLen = strlen(methodName);
		len += methodLen;
	}
	if(len)
	{
		int pos = 0;

		if(!(fullName = ILMalloc(len + 1)))
		{
			return 0;
		}
		if(namespaceName)
		{
			strcpy(fullName, namespaceName);
			pos = namespaceLen;
			fullName[pos] = '.';
			pos++;
		}
		if(className)
		{
			strcpy(fullName + pos, className);
			pos += classLen;
			fullName[pos] = '.';
			pos++;
		}
		if(methodName)
		{
			strcpy(fullName + pos, methodName);
			pos += methodLen;
		}
	}
	return fullName;
}

/*
 * Set the full methodname in the functions metadata.
 */
static void _ILJitFunctionSetMethodName(ILJitFunction func, ILMethod *method)
{
	char *methodName = _ILJitGetMethodName(method);

	if(methodName)
	{
		jit_function_set_meta(func, IL_JIT_META_METHODNAME, methodName,
							  _ILJitMetaFreeFunc, 0);
	}
}

/*
 * Get the full methodname from the jit functions metadata.
 */
static char *_ILJitFunctionGetMethodName(ILJitFunction func)
{
	return (char *)jit_function_get_meta(func, IL_JIT_META_METHODNAME);
}
#endif

/*
 * Destroy every ILJitType in a ILJitTypes  structure 
 */
void ILJitTypesDestroy(ILJitTypes *jitTypes)
{
	if(jitTypes->jitTypeBase)
	{
		ILJitTypeDestroy(jitTypes->jitTypeBase);
		jitTypes->jitTypeBase = 0;
	}
}

/*
 * Get the jit types for this ILType.
 */
static ILJitTypes *_ILJitGetTypes(ILType *type, ILExecProcess *process)
{

	type = ILTypeStripPrefixes(type);
	type = ILTypeGetEnumType(type);
	if(ILType_IsPrimitive(type))
	{
		return ILJitPrimitiveClrTypeToJitTypes(ILType_ToElement(type));
	}
	else
	{
		ILClass *classInfo;
		ILClassPrivate *classPrivate;

		if(ILType_IsValueType(type))
		{
			classInfo = ILClassResolve(ILType_ToValueType(type));
			ILType *synType = ILClassGetSynType(classInfo);

			if(synType != 0)
			{
				/* classInfo = ILClassResolve(ILType_ToClass(ILTypeStripPrefixes(synType))); */
				return _ILJitGetTypes(synType, process);
			}
		}
		else if(ILType_IsComplex(type) && type != 0)
		{
			switch(ILType_Kind(type))
			{
				case IL_TYPE_COMPLEX_PTR:
				{
					/* Unsafe pointers are represented as native integers */
					return &_ILJitType_I;
				}
				/* Not reached */

				case IL_TYPE_COMPLEX_BYREF:
				{
					/* Reference values are managed pointers */
					return &_ILJitType_VPTR;
				}
				/* Not reached */

				case IL_TYPE_COMPLEX_PINNED:
				{
					/* Pinned types are the same as their underlying type */
					return _ILJitGetTypes(ILType_Ref(type), process);
				}
				/* Not reached */

				case IL_TYPE_COMPLEX_CMOD_REQD:
				case IL_TYPE_COMPLEX_CMOD_OPT:
				{
					/* Strip the modifier and inspect the underlying type */
					return _ILJitGetTypes(type->un.modifier__.type__, process);
				}
				/* Not reached */

				case IL_TYPE_COMPLEX_METHOD:
				case IL_TYPE_COMPLEX_METHOD | IL_TYPE_COMPLEX_METHOD_SENTINEL:
				{
					/* Pass method pointers around the system as "I".  Higher
				   	   level code will also set the "typeInfo" field to reflect
					   the signature so that method pointers become verifiable */
					return &_ILJitType_I;
				}
				/* Not reached */

				default:
				{
					/* Everything else is a pointer type. */
					return &_ILJitType_VPTR;
				}
				/* Not reached */
			}
		} 
		else
		{
			classInfo = ILClassResolve(ILType_ToClass(type));
		}
		classPrivate = classInfo->userData;

		/* check if the class is layouted. */
		if(!classPrivate)
		{
			if(!_ILLayoutClass(process, classInfo))
			{
				return 0;
			}
			classPrivate = classInfo->userData;
		}
		return &(classPrivate->jitTypes);
	}
}

/*
 * Rethrow the given exception.
 */
void ILRuntimeExceptionRethrow(ILObject *object)
{
	if(object)
	{
		ILExecThread *thread = ILExecThreadCurrent();

		if(thread)
		{
			_ILExecThreadSetException(thread, object);
			jit_exception_throw(object);
		}
	}
}

/*
 * Throw the given exception.
 */
void ILRuntimeExceptionThrow(ILObject *object)
{
	System_Exception *exception = (System_Exception *)object;

	if(exception)
	{
		ILExecThread *thread = ILExecThreadCurrent();

		if(thread)
		{
			_ILExecThreadSetException(thread, (ILObject *)exception);
			exception->stackTrace = _ILJitGetExceptionStackTrace(thread);
			jit_exception_throw(exception);
		}
	}
}

/*
 * Throw an exception of the given type.
 * The caller has to make sure that the exception class is allready layouted.
 */
void ILRuntimeExceptionThrowClass(ILClass *classInfo)
{
	ILExecThread *thread = ILExecThreadCurrent();
	System_Exception *exception =
		(System_Exception *)_ILJitAlloc(classInfo,
							((ILClassPrivate *)(classInfo->userData))->size);

	/* We don't have to check exception for 0 because _ILJitAlloc would have */
	/* thrown an OutOfMenory exception then. */
	if(thread)
	{
		_ILExecThreadSetException(thread, (ILObject *)exception);
		exception->stackTrace = _ILJitGetExceptionStackTrace(thread);
		jit_exception_throw(exception);
	}
}

/*
 * Throw an OutOfMemoryException.
 */
void ILRuntimeExceptionThrowOutOfMemory()
{
	ILExecThread *thread = ILExecThreadCurrent();

	if(thread)
	{
		_ILExecThreadSetOutOfMemoryException(thread);
		jit_exception_throw(_ILExecThreadGetException(thread));
		return;
	}
	jit_exception_builtin(JIT_RESULT_OUT_OF_MEMORY);
}

/*
 * Handle the managed safepoint flags after a native call was made.
 */
void ILRuntimeHandleManagedSafePointFlags(ILExecThread *thread)
{
	if((thread->managedSafePointFlags & _IL_MANAGED_SAFEPOINT_THREAD_ABORT) && ILThreadIsAbortRequested())
	{
		if(_ILExecThreadSelfAborting(thread) == 0)
		{
			jit_exception_throw(_ILExecThreadGetException(thread));
		}
	}
	else if(thread->managedSafePointFlags & _IL_MANAGED_SAFEPOINT_THREAD_SUSPEND)
	{
		ILInterlockedAndU4(&(thread->managedSafePointFlags),
						   ~_IL_MANAGED_SAFEPOINT_THREAD_SUSPEND);
		if(ILThreadGetState(thread->supportThread) & IL_TS_SUSPEND_REQUESTED)
		{
			_ILExecThreadSuspendThread(thread, thread->supportThread);
		}
	}
}

/*
 * Handle an exception thrown in an internal call.
 */
static void _ILJitHandleThrownException(ILJitFunction func,
										ILJitValue thread)
{ 
	ILJitValue thrownException;
	jit_label_t label;

	label = jit_label_undefined;
	thrownException = jit_insn_load_relative(func, thread,
											 offsetof(ILExecThread, thrownException),
											 _IL_JIT_TYPE_VPTR);
	jit_insn_branch_if_not(func, thrownException, &label);
	jit_insn_throw(func, thrownException);
	jit_insn_label(func, &label);
}

/*
 * Perform a class cast, taking arrays into account.
 */
ILInt32 ILRuntimeCanCastClass(ILMethod *method, ILObject *object, ILClass *toClass)
{
	ILImage *image = ILProgramItem_Image(method);
	ILClass *fromClass = GetObjectClass(object);
	ILType *fromType = ILClassGetSynType(fromClass);
	ILType *toType = ILClassGetSynType(toClass);
	if(fromType && toType)
	{
		if(ILType_IsArray(fromType) && ILType_IsArray(toType) &&
		   ILTypeGetRank(fromType) == ILTypeGetRank(toType))
		{
			return ILTypeAssignCompatibleNonBoxing
			  (image, ILTypeGetElemType(fromType), ILTypeGetElemType(toType));
		}
		else if(ILType_IsWith(fromType) && ILType_IsWith(toType))
		{
			/* TODO: perform a real check of assignment compatibility as
			   described in ECMA 334 Version 4 Partition I 8.7 */
			return ILTypeIdentical(fromType, toType);
		}
		else
		{
			return 0;
		}
	}
	else
	{
		fromType=ILTypeGetEnumType(ILClassToType(fromClass));
		toType=ILTypeGetEnumType(ILClassToType(toClass));
		
		if(ILTypeIdentical(fromType,toType))
		{
			return 1;
		}

	   	return ILClassInheritsFrom(fromClass, toClass);
	}
}

/*
 * Perform a check if a class implements a given interface.
 */
ILInt32 ILRuntimeClassImplements(ILObject *object, ILClass *toClass)
{
	ILClass *info = GetObjectClass(object);

	return ILClassImplements(info, toClass);
}

/*
 * Get a thread-static value from the current thread.
 */
void *ILRuntimeGetThreadStatic(ILExecThread *thread,
							   ILUInt32 slot, ILUInt32 size)
{
	void **array;
	ILUInt32 used;
	void *ptr;

	/* Determine if we need to allocate space for a new slot */
	if(slot >= thread->threadStaticSlotsUsed)
	{
		used = (slot + 8) & ~7;
		array = (void **)ILGCAlloc(sizeof(void *) * used);
		if(!array)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		if(thread->threadStaticSlotsUsed > 0)
		{
			ILMemMove(array, thread->threadStaticSlots,
					  sizeof(void *) * thread->threadStaticSlotsUsed);
		}
		thread->threadStaticSlots = array;
		thread->threadStaticSlotsUsed = used;
	}

	/* Fetch the current value in the slot */
	ptr = thread->threadStaticSlots[slot];
	if(ptr)
	{
		return ptr;
	}

	/* Allocate a new value and write it to the slot */
	if(!size)
	{
		/* Sanity check, just in case */
		size = sizeof(unsigned long);
	}
	ptr = ILGCAlloc((unsigned long)size);
	if(!ptr)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	thread->threadStaticSlots[slot] = ptr;
	return ptr;
}

#ifdef IL_JIT_FNPTR_ILMETHOD
/*
 * Get the vtable pointer for an ILMethod.
 */
void *ILRuntimeMethodToVtablePointer(ILMethod *method)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);

	if(!jitFunction)
	{
		/* The method's owner class needs to be layouted first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(method)))
		{
			/* TODO: Throw an exception here. */
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			/* TODO: Throw an exception here. */
			return 0;
		}
	}
	return jit_function_to_vtable_pointer(jitFunction);
}
#endif

static void *_ILRuntimeLookupInterfaceMethod(ILClassPrivate *objectClassPrivate,
											 ILClass *interfaceClass,
											 ILUInt32 index)
{
	ILImplPrivate *implements;
	ILClassPrivate *searchClass = objectClassPrivate;
	ILClass *parent;

	/* Locate the interface table within the class hierarchy for the object */
	while(searchClass != 0)
	{
		implements = searchClass->implements;
		while(implements != 0)
		{
			if(implements->interface == interfaceClass)
			{
				/* We've found the interface, so look in the interface
				   table to find the vtable slot, which is then used to
				   look in the class's vtable for the actual method */
				index = (ILUInt32)((ILImplPrivate_Table(implements))[index]);
				if(index != (ILUInt32)(ILUInt16)0xFFFF)
				{
					return objectClassPrivate->jitVtable[index];
				}
				else
				{
					/* The interface slot is abstract.  This shouldn't
					   happen in practice, but let's be paranoid anyway */
					return 0;
				}
			}
			implements = implements->next;
		}
		parent = ILClass_ParentClass(searchClass->classInfo);
		if(!parent)
		{
			break;
		}
		searchClass = (ILClassPrivate *)(parent->userData);
	}

	/* The interface implementation was not found */
	return 0;
}

#ifdef IL_JIT_FNPTR_ILMETHOD
/*
 * This is the same function as above but returns the ILMethod instead of the
 * vtable pointer of the interface method.
 */
static void *_ILRuntimeLookupInterfaceILMethod(ILClassPrivate *objectClassPrivate,
											   ILClass *interfaceClass,
											   ILUInt32 index)
{
	ILImplPrivate *implements;
	ILClassPrivate *searchClass = objectClassPrivate;
	ILClass *parent;

	/* Locate the interface table within the class hierarchy for the object */
	while(searchClass != 0)
	{
		implements = searchClass->implements;
		while(implements != 0)
		{
			if(implements->interface == interfaceClass)
			{
				/* We've found the interface, so look in the interface
				   table to find the vtable slot, which is then used to
				   look in the class's vtable for the actual method */
				index = (ILUInt32)((ILImplPrivate_Table(implements))[index]);
				if(index != (ILUInt32)(ILUInt16)0xFFFF)
				{
					return objectClassPrivate->vtable[index];
				}
				else
				{
					/* The interface slot is abstract.  This shouldn't
					   happen in practice, but let's be paranoid anyway */
					return 0;
				}
			}
			implements = implements->next;
		}
		parent = ILClass_ParentClass(searchClass->classInfo);
		if(!parent)
		{
			break;
		}
		searchClass = (ILClassPrivate *)(parent->userData);
	}

	/* The interface implementation was not found */
	return 0;
}
#endif

/*
 * Get the pointer to base type from the JitTypes.
 * The pointer type is created on demand if not allready present.
 * Returns 0 when out of memory.
 */
static ILJitType _ILJitGetPointerTypeFromJitTypes(ILJitTypes *types)
{
	return _IL_JIT_TYPE_VPTR;
}

/*
 * Get the jit type representing the this pointer for the given ILType.
 * Returns 0 whne the type could not be found or out of memory.
 */
static ILJitType _ILJitGetThisType(ILType *type, ILExecProcess *process)
{
	return _IL_JIT_TYPE_VPTR;
}

/*
 * Get the jit type representing the argument type for the given ILType.
 * Returns 0 whne the type could not be found or out of memory.
 * TODO: Handle ref and out args.
 */
static ILJitType _ILJitGetArgType(ILType *type, ILExecProcess *process)
{
	if(ILType_IsClass(type))
	{
		return _IL_JIT_TYPE_VPTR;
	}
	else
	{
		ILJitTypes *types = _ILJitGetTypes(type, process);

		if(!types)
		{
			return 0;
		}
		return types->jitTypeBase;
	}
}

/*
 * Get the jit type representing the local type for the given ILType.
 * Returns 0 when the type could not be found or out of memory.
 */
static ILJitType _ILJitGetLocalsType(ILType *type, ILExecProcess *process)
{
	if(ILType_IsClass(type))
	{
		return _IL_JIT_TYPE_VPTR;
	}
	else
	{
		ILJitTypes *types = _ILJitGetTypes(type, process);

		if(!types)
		{
			return 0;
		}
		return types->jitTypeBase;
	}
}

/*
 * Get the jit type representing the return value for the given ILType.
 */
static ILJitType _ILJitGetReturnType(ILType *type, ILExecProcess *process)
{
	if(ILType_IsClass(type))
	{
		return _IL_JIT_TYPE_VPTR;
	}
	else
	{
		ILJitTypes *types = _ILJitGetTypes(type, process);

		if(!types)
		{
			return 0;
		}
		return types->jitTypeBase;
	}
}

/*
 * Get the jit type from an ILClass.
 */
static ILJitType _ILJitGetTypeFromClass(ILClass *info)
{
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;

	if(!classPrivate)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), info))
		{
			return 0;
		}
		classPrivate = (ILClassPrivate *)info->userData;
	}
	return(classPrivate->jitTypes.jitTypeBase);
}

/*
 * Get the size of an object.
 */
static ILUInt32 _ILJitGetSizeOfClass(ILClass *info)
{
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;

	if(!classPrivate)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), info))
		{
			return 0;
		}
		classPrivate = (ILClassPrivate *)info->userData;
	}
	return((ILUInt32)jit_type_get_size(classPrivate->jitTypes.jitTypeBase));
}

/*
 * The exception handler which converts libjit inbuilt exceptions
 * into clr exceptions.
 */
void *_ILJitExceptionHandler(int exception_type)
{
    ILExecThread *thread = ILExecThreadCurrent();
    void *object = 0;

    switch(exception_type)
    {
		case(JIT_RESULT_OVERFLOW):
		{
			object = _ILSystemException(thread, "System.OverflowException");
		}
		break;

		case(JIT_RESULT_ARITHMETIC):
		{
			object = _ILSystemException(thread, "System.ArithmeticException");
		}
		break;

		case(JIT_RESULT_DIVISION_BY_ZERO):
		{
			object = _ILSystemException(thread, "System.DivideByZeroException");
		}
		break;

		case(JIT_RESULT_COMPILE_ERROR):
		{
			object = _ILSystemException(thread, "System.ExecutionEngineException");
		}
		break;

		case(JIT_RESULT_OUT_OF_MEMORY):
		{
			object = _ILSystemException(thread, "System.OutOfMemoryException");
		}    
		break;

		case(JIT_RESULT_NULL_REFERENCE):
		{
			object = _ILSystemException(thread, "System.NullReferenceException");
		}
		break;

		case(JIT_RESULT_NULL_FUNCTION):
		{
			object = _ILSystemException(thread, "System.MissingMethodException");
		}
		break;

		case(JIT_RESULT_CALLED_NESTED):
		{
			object = _ILSystemException(thread, "System.MissingMethodException");
		}
		break;

		case(JIT_RESULT_OUT_OF_BOUNDS):
		{
			object = _ILSystemException(thread, "System.IndexOutOfRangeException");
		}
		break;

		default:
		{
			object = _ILSystemException(thread, "System.Exception");
		}
		break;
	}
	_ILExecThreadSetException(thread, object);
	return object;
}

/*
 * Generate the code to throw the current exception in the thread in libjit.
 */
static void _ILJitThrowCurrentException(ILJITCoder *coder)
{
	ILJitValue thread;
	ILJitValue thrownException;

	thread = _ILJitCoderGetThread(coder);
	thrownException = jit_insn_load_relative(coder->jitFunction,
											 thread,
											 offsetof(ILExecThread, thrownException), 
											 _IL_JIT_TYPE_VPTR);
	jit_insn_throw(coder->jitFunction, thrownException);
}

static int _ILJitCompile(jit_function_t func);

/*
 * The on demand driver function for libjit.
 */
static void *_ILJitOnDemandDriver(ILJitFunction func)
{
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	char *methodName = _ILJitFunctionGetMethodName(func);
#endif
	ILMethod *method = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILClass *info = ILMethod_Owner(method);
	ILClassPrivate *classPrivate = (ILClassPrivate *)info->userData;
	ILExecProcess *process = classPrivate->process;
	ILJITCoder *jitCoder = (ILJITCoder *)process->coder;
	void *entry_point;
	jit_on_demand_func onDemandCompiler;
	int result = JIT_RESULT_OK;
	jit_context_t context = jit_function_get_context(func);

	if(!context)
	{
		result = JIT_RESULT_COMPILE_ERROR;
		jit_exception_builtin(result);
	}

	/* Lock the metadata. */
	METADATA_WRLOCK(process);

	/* Handle a locked method. */
	if((entry_point = JITCoder_HandleLockedMethod(process->coder, method)))
	{
		/* Unlock the metadata. */
		METADATA_UNLOCK(process);

		return entry_point;
	}

	/* Check if the function is compiled now */
	if(jit_function_is_compiled(func))
	{
		if(jit_function_compile_entry(func, &entry_point))
		{
			/* Unlock the metadata. */
			METADATA_UNLOCK(process);

			return entry_point;
		}
	}

	/* Set the function info in the jit coder. */
	jitCoder->jitFunction = func;
	ILCCtorMgr_SetCurrentMethod(&(jitCoder->cctorMgr), method);
	jitCoder->cctorMgr.currentJitFunction = func;

	/* Lock down the context. */
	jit_context_build_start(context);

	if(!(onDemandCompiler = jit_function_get_on_demand_compiler(func)))
	{
		result = JIT_RESULT_COMPILE_ERROR;

		/* Unlock the context. */
		jit_context_build_end(context);

		/* Unlock the metadata. */
		METADATA_UNLOCK(process);

		/* And throw an exception. */
		jit_exception_builtin(result);
	}

	if((result = (*onDemandCompiler)(func)) == JIT_RESULT_OK)
	{
	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	#ifdef _IL_JIT_DUMP_FUNCTION
		if(jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			jit_dump_function(stdout, func, methodName);
			ILMutexUnlock(globalTraceMutex);
		}
	#endif	/* _IL_JIT_DUMP_FUNCTION */
	#endif

		/* Now compile the function to it's native form. */
		if(!jit_function_compile_entry(func, &entry_point))
		{
			/* How are errors handled ? */

			/* Unlock the context. */
			jit_context_build_end(context);

			/* Unlock the metadata. */
			METADATA_UNLOCK(process);

			/* And throw an exception. */
			jit_exception_builtin(JIT_RESULT_OUT_OF_MEMORY);
		}

		/* Unlock the context. */
		jit_context_build_end(context);

		/* and run the queued class initializers. */
		ILCCtorMgr_RunCCtors(&(jitCoder->cctorMgr), entry_point);

	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	#ifdef _IL_JIT_DISASSEMBLE_FUNCTION
		if(jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			jit_dump_function(stdout, func, methodName);
			ILMutexUnlock(globalTraceMutex);
		}
	#endif	/* _IL_JIT_DISASSEMBLE_FUNCTION */
	#endif
	}
	else
	{
		/* Unlock the context. */
		jit_context_build_end(context);

		/* This is ugly but it's the only fast solution now */
		if(onDemandCompiler != _ILJitCompile)
		{
			/* Unlock the metadata. */
			METADATA_UNLOCK(process);
		}

		/* And throw an exception. */
		jit_exception_builtin(result);
	}
	return entry_point;
}

/*
 * Initialize the libjit coder.
 * Returns 1 on success and 0 on failure.
 */
int ILJitInit()
{
	ILJitType	returnType;
	ILJitType	args[7];

	/* Initialize libjit */
	jit_init();

	/* Initialize the nattive types. */
	_ILJitTypesInitBase(&_ILJitType_VOID, jit_type_void);
	_ILJitTypesInitBase(&_ILJitType_BOOLEAN, jit_type_ubyte);
	_ILJitTypesInitBase(&_ILJitType_BYTE, jit_type_ubyte);
	_ILJitTypesInitBase(&_ILJitType_CHAR, jit_type_ushort);
#ifdef IL_NATIVE_INT32
	_ILJitTypesInitBase(&_ILJitType_I, jit_type_int);
#else
#ifdef IL_NATIVE_INT64
	_ILJitTypesInitBase(&_ILJitType_I, jit_type_long);
#endif
#endif
	_ILJitTypesInitBase(&_ILJitType_I2, jit_type_short);
	_ILJitTypesInitBase(&_ILJitType_I4, jit_type_int);
	_ILJitTypesInitBase(&_ILJitType_I8, jit_type_long);
	_ILJitTypesInitBase(&_ILJitType_NFLOAT, jit_type_nfloat);
	_ILJitTypesInitBase(&_ILJitType_R4, jit_type_float32);
	_ILJitTypesInitBase(&_ILJitType_R8, jit_type_float64);
	_ILJitTypesInitBase(&_ILJitType_SBYTE, jit_type_sbyte);
#ifdef IL_NATIVE_INT32
	_ILJitTypesInitBase(&_ILJitType_U, jit_type_uint);
#else
#ifdef IL_NATIVE_INT64
	_ILJitTypesInitBase(&_ILJitType_U, jit_type_ulong);
#endif
#endif
	_ILJitTypesInitBase(&_ILJitType_U2, jit_type_ushort);
	_ILJitTypesInitBase(&_ILJitType_U4, jit_type_uint);
	_ILJitTypesInitBase(&_ILJitType_U8, jit_type_ulong);
	_ILJitTypesInitBase(&_ILJitType_VPTR, jit_type_void_ptr);


	// Initialize the TypedRef type in its jit representation.	
	if(!(_ILJitTypedRef = jit_type_create_struct(0, 0, 0)))
	{
		return 0;
	}
	jit_type_set_size_and_alignment(_ILJitTypedRef,
									sizeof(ILTypedRef),
									_IL_ALIGN_FOR_TYPE(void_p));
	_ILJitTypesInitBase(&_ILJitType_TYPEDREF, _ILJitTypedRef);

	/* Initialize the native method signatures. */
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILExecThreadCurrent = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, 0, 0, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_UINT32;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILJitAlloc = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

#ifdef	IL_USE_TYPED_ALLOCATION
	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILJitAllocTyped = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}
#endif	/* IL_USE_TYPED_ALLOCATION */

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_UINT32;
	args[2] = _IL_JIT_TYPE_UINT32;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILJitSArrayAlloc = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 3, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_UINT32;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILJitStringAlloc = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_INT32;
	if(!(_ILJitSignature_ILRuntimeCanCastClass = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 3, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_INT32;
	if(!(_ILJitSignature_ILRuntimeClassImplements = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILRuntimeExceptionRethrow =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILRuntimeExceptionThrow =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILRuntimeExceptionThrowClass =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILRuntimeExceptionThrowOutOfMemory =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, 0, 0, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_UINT32;
	args[2] = _IL_JIT_TYPE_UINT32;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILRuntimeGetThreadStatic = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 3, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_UINT32;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILRuntimeLookupInterfaceMethod = 
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 3, 1)))
	{
		return 0;
	}

	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_JitExceptionClearLast =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, 0, 0, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILJitGetExceptionStackTrace =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_NINT;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_malloc =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

#ifdef IL_JIT_FNPTR_ILMETHOD
	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILRuntimeMethodToVtablePointer =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}
#endif

	/* Create the signatures for the inlined native function calls. */
	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILMonitorEnter =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILMonitorExit =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILGetClrType =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILRuntimeHandleManagedSafePointFlags =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILStringToAnsi =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}
	
	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILStringToUTF8 =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}
	
	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILStringCreate =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILStringToUTF16 =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}
	
	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILStringCreateUTF8 =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}
	
	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILStringWCreate =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_ILJitDelegateGetClosure =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_VPTR;
	args[3] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_MarshalObjectToCustom =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 4, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_VPTR;
	args[3] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VPTR;
	if(!(_ILJitSignature_MarshalCustomToObject =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 4, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_INT32;
	if(!(_ILJitSignature_ILInterlockedIncrement =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_VPTR;
	args[3] = _IL_JIT_TYPE_INT32;
	args[4] = _IL_JIT_TYPE_INT32;
	returnType = _IL_JIT_TYPE_INT32;
	if(!(_ILJitSignature_ILSArrayCopy_AAI4 =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 5, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_INT32;
	args[3] = _IL_JIT_TYPE_VPTR;
	args[4] = _IL_JIT_TYPE_INT32;
	args[5] = _IL_JIT_TYPE_INT32;
	args[6] = _IL_JIT_TYPE_INT32;
	returnType = _IL_JIT_TYPE_INT32;
	if(!(_ILJitSignature_ILSArrayCopy_AI4AI4I4 =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 7, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	args[2] = _IL_JIT_TYPE_INT32;
	args[3] = _IL_JIT_TYPE_INT32;
	args[4] = _IL_JIT_TYPE_INT32;
	returnType = _IL_JIT_TYPE_INT32;
	if(!(_ILJitSignature_ILSArrayClear_AI4I4 =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 5, 1)))
	{
		return 0;
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
#ifdef ENHANCED_PROFILER
	args[0] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILProfilingStart =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 1, 1)))
	{
		return 0;
	}

	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILProfilingEnd =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}
#endif /* ENHANCED_PROFILER */
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	args[0] = _IL_JIT_TYPE_VPTR;
	args[1] = _IL_JIT_TYPE_VPTR;
	returnType = _IL_JIT_TYPE_VOID;
	if(!(_ILJitSignature_ILJitTraceInOut =
		jit_type_create_signature(IL_JIT_CALLCONV_CDECL, returnType, args, 2, 1)))
	{
		return 0;
	}
#endif

	return 1;
}

/*
 * Create a new JIT coder instance.
 */
static ILCoder *JITCoder_Create(ILExecProcess *process, ILUInt32 size,
								unsigned long cachePageSize)
{
	ILJITCoder *coder;
	if((coder = (ILJITCoder *)ILMalloc(sizeof(ILJITCoder))) == 0)
	{
		return 0;
	}
	coder->coder.classInfo = &_ILJITCoderClass;
	coder->process = process;
	if(!(coder->context = jit_context_create()))
	{
		ILFree(coder);
		return 0;
	}
	coder->debugEnabled = 0;
	coder->flags = 0;
	coder->optimizationLevel = 1;

	/* Intialize the pool for the method infos. */
	ILMemPoolInit(&(coder->methodPool), sizeof(ILJitMethodInfo), 100);

	/* Init the current jitted function. */
	coder->jitFunction = 0;

	if(!ILCCtorMgr_Init(&(coder->cctorMgr), 10))
	{
		ILMemPoolDestroy(&(coder->methodPool));
		ILFree(coder);
		return 0;
	}

	/* Set the on demand compilation driver for this context. */
	jit_context_set_on_demand_driver(coder->context,
									 _ILJitOnDemandDriver);

#ifndef IL_JIT_THREAD_IN_SIGNATURE
	coder->thread = 0;
#endif

#define IL_JITC_CODER_INIT
#include "jitc_inline.c"
#include "jitc_locals.c"
#include "jitc_stack.c"
#include "jitc_labels.c"
#include "jitc_profile.c"
#include "jitc_except.c"
#undef IL_JITC_CODER_INIT

	/* Ready to go */
	return &(coder->coder);
}

jit_context_t ILJitGetContext(ILCoder *_coder)
{
	return _ILCoderToILJITCoder(_coder)->context;
}

#ifdef IL_DEBUGGER

/*
 * Temporary watch marking.
 */
#define ILLocalWatchIsInvalid(watch)		((watch)->type & 0x80000000)
#define ILLocalWatchMarkInvalid(watch)		((watch)->type |= 0x80000000)
#define ILLocalWatchMarkValid(watch)		((watch)->type &= ~0x80000000)

/*
 * Drop invalid watches from thread->watchStack.
 * Watches are normaly removed on return statement (see handler
 * for JIT_DEBUGGER_DATA1_METHOD_LEAVE). If function returns to caller by any
 * other means e.g. because of exception, this function is called to drop
 * watches in frames that are no longer valid.
 */
static void ILJitDropInvalidWatches(ILExecThread *thread)
{
	void *frame;
	ILUInt32 i;
	ILLocalWatch *watch;

	/* Mark all watches invalid */
	for(i = 0; i < thread->numWatches; i++)
	{
		watch = &(thread->watchStack[i]);
		ILLocalWatchMarkInvalid(watch);
	}

	/* Find and unmark watches with valid frame pointers */
	frame = thread->frame;
	while(frame)
	{
		for(i = 0; i < thread->numWatches; i++)
		{
			watch = &(thread->watchStack[i]);
			if(watch->frame == frame)
			{
				ILLocalWatchMarkValid(watch);
			}
		}
		frame = jit_get_next_frame_address(frame);
	}

	/* Remove all invalid watches */
	for(i = thread->numWatches - 1; ((ILInt32)(i)) >= 0; i--)
	{
		watch = &(thread->watchStack[i]);
		if(ILLocalWatchIsInvalid(watch))
		{
			/* Watch is invalid */
			thread->numWatches--;
			ILMemMove(watch, watch - 1, thread->numWatches - i);
		}
	}
}

/*
 * Debugger hook function.
 * This function keeps up thread data refreshed using values
 * from soft breakpoints. These breakpoints are placed as function
 * is compiled in following order and with following data:
 *
 * data1					data2					meaning
 *-----------------------------------------------------------------------------
 * METHOD_ENTER 			ILMethod *				method was called
 * PARAM_ADDR				void *					address of parameter
 * THIS_ADDR				void *					address of "this"
 * LOCAL_VAR_ADDR			void *					address of local variable
 * ILMethod * (method)		ILUInt32 (IL offset)	soft breakpoint (before every IL instruction)
 * METHOD_LEAVE				unused					return from method
 */
static void JitDebuggerHook(jit_function_t func, jit_nint data1, jit_nint data2)
{
	ILExecThread *thread;
	ILLocalWatch *watch;
	void *frame;

	thread = ILExecThreadCurrent();

	/* Prevent debug hook call for thread executing from debugger */
	if(ILDebuggerIsThreadUnbreakable(thread))
	{
		return;
	}

	switch(data1)
	{
		case JIT_DEBUGGER_DATA1_METHOD_ENTER:
		{
			thread->method = (ILMethod *) data2;
			return;
		}
		/* Not reached */

		case JIT_DEBUGGER_DATA1_THIS_ADDR:
		case JIT_DEBUGGER_DATA1_PARAM_ADDR:
		case JIT_DEBUGGER_DATA1_LOCAL_VAR_ADDR:
		{
			/* Allocate watch for this local variable */
			if(thread->numWatches < thread->maxWatches)
			{
				watch = &(thread->watchStack[(thread->numWatches)]);
			}
			else if((watch = _ILAllocLocalWatch(thread)) == 0)
			{
				/* We ran out of memory trying to push the watch */
				thread->numWatches = 0;
				ILRuntimeExceptionThrowOutOfMemory();
			}

			/* Two frames above us should be frame for current method */
			watch->frame = jit_get_frame_address(2);

			/* Address is in data2 */
			watch->addr = (void *) data2;

			if(data1 == JIT_DEBUGGER_DATA1_PARAM_ADDR)
			{
				watch->type = IL_LOCAL_WATCH_TYPE_PARAM;
			}
			else if(data1 == JIT_DEBUGGER_DATA1_LOCAL_VAR_ADDR)
			{
				watch->type = IL_LOCAL_WATCH_TYPE_LOCAL_VAR;
			}
			else if(data1 == JIT_DEBUGGER_DATA1_THIS_ADDR)
			{
				watch->type = IL_LOCAL_WATCH_TYPE_THIS;
			}

			thread->numWatches++;
			return;
		}
		/* Not reached */

		case JIT_DEBUGGER_DATA1_METHOD_LEAVE:
		{
			/* Remove all watches in current frame */
			frame = thread->frame = jit_get_frame_address(2);
			watch = &(thread->watchStack[(thread->numWatches - 1)]);
			while(thread->numWatches > 0 && watch->frame == frame)
			{
				thread->numWatches--;
				watch--;
			}
			return;
		}
		/* Not reached */

		default:
		{
			/* Set current method and IL offset */
			thread->method = (ILMethod *) data1;
			thread->offset = (ILUInt32) data2;

			/* Check if current frame has changed - e.g. after exception */
			frame = jit_get_frame_address(2);
			if(frame != thread->frame)
			{
				/* Call stack has changed, drop invalid watches */
				thread->frame = frame;
				ILJitDropInvalidWatches(thread);
			}
		}
		break;
	}

	/* Call engine's breakpoint handler */
	_ILBreak(thread, 0);
}
#endif

/*
 * Enable debug mode in a JIT coder instance.
 */
static void JITCoder_EnableDebug(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	jitCoder->debugEnabled = 1;
#ifdef IL_DEBUGGER
	jit_debugger_set_hook(jitCoder->context, JitDebuggerHook);
#endif
}

/*
 * Allocate memory within a JIT coder instance.
 */
static void *JITCoder_Alloc(ILCoder *_coder, ILUInt32 size)
{
	/* this might be a noop for the JIT coder */
	/* ILJITCoder *coder = _ILCoderToILJITCoder(_coder);
	return ILCacheAllocNoMethod(coder->cache, size); */
	return 0;
}

/*
 * Get the size of the method cache.
 */
static unsigned long JITCoder_GetCacheSize(ILCoder *_coder)
{
	/* this might be a noop for the JIT coder */
	/* return ILCacheGetSize((_ILCoderToILJITCoder(_coder))->cache); */
	return 0;
}

/*
 * Destroy a JIT coder instance.
 */
static void JITCoder_Destroy(ILCoder *_coder)
{
	ILJITCoder *coder = _ILCoderToILJITCoder(_coder);

#define IL_JITC_CODER_DESTROY
#include "jitc_inline.c"
#include "jitc_locals.c"
#include "jitc_stack.c"
#include "jitc_labels.c"
#include "jitc_profile.c"
#undef IL_JITC_CODER_DESTROY

	if(coder->context)
	{
		jit_context_destroy(coder->context);
		coder->context = 0;
	}

	ILMemPoolDestroy(&(coder->methodPool));

	ILCCtorMgr_Destroy(&(coder->cctorMgr));

	ILFree(coder);
}

/*
 * Get an IL offset from a native offset within a method.
 */
static ILUInt32 JITCoder_GetILOffset(ILCoder *_coder, void *start,
									 ILUInt32 offset, int exact)
{
	/* return ILCacheGetBytecode(((ILCVMCoder *)_coder)->cache, start,
							  offset, exact); */
	/* TODO */
	return 0;
}

/*
 * Get a native offset from an IL offset within a method.
 */
static ILUInt32 JITCoder_GetNativeOffset(ILCoder *_coder, void *start,
									     ILUInt32 offset, int exact)
{
	/* return ILCacheGetNative(((ILCVMCoder *)_coder)->cache, start,
							offset, exact); */
	/* TODO */
	return 0;
}

/*
 * Mark the current position with a bytecode offset.
 */
static void JITCoder_MarkBytecode(ILCoder *coder, ILUInt32 offset)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	if(offset != 0)
	{
		jit_insn_mark_offset(jitCoder->jitFunction, (jit_int)offset);
	}
#ifdef IL_DEBUGGER
	/* Insert breakpoint marks if needed */
	if(jitCoder->markBreakpoints)
	{
		/* Mark breakpoint that reports current ILMethod and IL offset */
		jit_insn_mark_breakpoint(jitCoder->jitFunction,
								 (jit_nint) ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr)),
								 (jit_nint) offset);
	}
#endif
}

/*
 * Run the class initializers queued during generation of the last method.
 */
static ILInt32 JITCoder_RunCCtors(ILCoder *coder, void *userData)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	return ILCCtorMgr_RunCCtors(&(jitCoder->cctorMgr), userData);;
}

/*
 * Run the class initializer for the given class.
 */
static ILInt32 JITCoder_RunCCtor(ILCoder *coder, ILClass *classInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	return ILCCtorMgr_RunCCtor(&(jitCoder->cctorMgr), classInfo);
}

/*
 * Handle the method lock while running class initializers.
 */
static void *JITCoder_HandleLockedMethod(ILCoder *coder, ILMethod *method)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	return ILCCtorMgr_HandleLockedMethod(&(jitCoder->cctorMgr), method);
}

static void	JITCoder_SetOptimizationLevel(ILCoder *coder,
										  ILUInt32 optimizationLevel)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	jitCoder->optimizationLevel = optimizationLevel;
}

static ILUInt32 JITCoder_GetOptimizationLevel(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	return jitCoder->optimizationLevel;
}

#ifdef IL_CONFIG_PINVOKE

/*
 * Locate or load an external module that is being referenced via "PInvoke".
 * Returns the system module pointer, or NULL if it could not be loaded.
 */
static void *LocateExternalModule(ILExecProcess *process, const char *name,
								  ILPInvoke *pinvoke)
{
	ILLoadedModule *loaded;
	char *pathname;

	/* Search for an already-loaded module with the same name */
	loaded = process->loadedModules;
	while(loaded != 0)
	{
		if(!ILStrICmp(loaded->name, name))
		{
			return loaded->handle;
		}
		loaded = loaded->next;
	}

	/* Create a new module structure.  We keep this structure even
	   if we cannot load the actual module.  This ensures that
	   future requests for the same module will be rejected without
	   re-trying the open */
	loaded = (ILLoadedModule *)ILMalloc(sizeof(ILLoadedModule) + strlen(name));
	if(!loaded)
	{
		return 0;
	}
	loaded->next = process->loadedModules;
	loaded->handle = 0;
	strcpy(loaded->name, name);
	process->loadedModules = loaded;

	/* Resolve the module name to a library name */
	pathname = ILPInvokeResolveModule(pinvoke);
	if(!pathname)
	{
		return 0;
	}

	/* Attempt to open the module */
	loaded->handle = ILDynLibraryOpen(pathname);
	ILFree(pathname);
	return loaded->handle;
}

#endif /* IL_CONFIG_PINVOKE */

/*
 * Set the method member in the ILExecThread instance.
 */
static void _ILJitSetMethodInThread(ILJitFunction func, ILJitValue thread, ILMethod *method)
{
	ILJitValue methodPtr = jit_value_create_nint_constant(func, _IL_JIT_TYPE_VPTR, (jit_nint)method);

	jit_insn_store_relative(func, thread, offsetof(ILExecThread, method), methodPtr);
}

/*
 * Get the classPrivate pointer from an object reference.
 */
static ILJitValue _ILJitGetObjectClassPrivate(ILJitFunction func, ILJitValue object)
{
	ILJitValue classPrivate = 
		jit_insn_load_relative(func, object, 
		    				   -IL_OBJECT_HEADER_SIZE +
							   offsetof(ILObjectHeader, classPrivate),
							   _IL_JIT_TYPE_VPTR);
	return classPrivate;	
}

/*
 * Get the ILClass pointer from an object reference.
 */
static ILJitValue _ILJitGetObjectClass(ILJitFunction func, ILJitValue object)
{
	ILJitValue classPrivate = _ILJitGetObjectClassPrivate(func, object);

	return jit_insn_load_relative(func, classPrivate, 
								  offsetof(ILClassPrivate, classInfo),
								  _IL_JIT_TYPE_VPTR);
}

/*
 * Emit the code that has to run before a native function is executed.
 */
static void _ILJitBeginNativeCall(ILJitFunction func, ILJitValue thread)
{
	ILJitValue zero = jit_value_create_nint_constant(func,
													 jit_type_sys_int,
													 (jit_nint)0);

	jit_insn_store_relative(func, thread,
							offsetof(ILExecThread, runningManagedCode),
							zero);
}

/*
 * Emit the code that has to run after a native function is executed.
 */
static void _ILJitEndNativeCall(ILJitFunction func, ILJitValue thread)
{
	ILJitValue one = jit_value_create_nint_constant(func,
													 jit_type_sys_int,
													 (jit_nint)1);
	ILJitValue temp = jit_insn_load_relative(func,
											thread,
											offsetof(ILExecThread, managedSafePointFlags),
											jit_type_sys_int);
	jit_label_t label = jit_label_undefined;

	jit_insn_store_relative(func, thread,
							offsetof(ILExecThread, runningManagedCode),
							one);
	jit_insn_branch_if_not(func, temp, &label);
	jit_insn_call_native(func,
						 "ILRuntimeHandleManagedSafePointFlags",
						 ILRuntimeHandleManagedSafePointFlags,
						 _ILJitSignature_ILRuntimeHandleManagedSafePointFlags,
						 &thread, 1, 0);

	jit_insn_label(func, &label);
	_ILJitHandleThrownException(func, thread);
}

/*
 * Check if a function is implemented by an internalcall.
 * Returns 0 if the function is not implemented by an internal call,
 * 1 if the function is implemented by an internal call and the function
 * is not an allocating constructor (it doesn't allocate the new object)
 * and 2 if the function is an allocating constructor.
 */
static int _ILJitFunctionIsInternal(ILJITCoder *coder, ILMethod *method,
									ILInternalInfo *fnInfo, int isConstructor)
{
	ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)(method->userData);

	if(jitMethodInfo)
	{
		if((jitMethodInfo->implementationType) & _IL_JIT_IMPL_INTERNALMASK)
		{
			*fnInfo = jitMethodInfo->fnInfo;
			return (jitMethodInfo->implementationType) & _IL_JIT_IMPL_INTERNALMASK;
		}
	}
	return 0;
}

/*
 * Generate the code to call an internal function.
 */
static ILJitValue _ILJitCallInternal(ILJITCoder *jitCoder,
									 ILJitValue thread,
									 ILMethod *method,
									 void *nativeFunction,
									 const char *methodName,
									 ILJitValue *args,
									 ILUInt32 numArgs)
{
	ILType *ilSignature = ILMethod_Signature(method);
	ILType *type = ILTypeGetEnumType(ILTypeGetParam(ilSignature, 0));
	/* Get the function to call. */
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitType signature = 0;
	ILJitType callSignature = 0;
	ILJitType returnType = 0;
	ILJitValue returnValue = 0;
	unsigned int numParams = 0;
	unsigned int totalParams = 0;
	int hasStructReturn = 0;
	ILUInt32 current = 0;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(method)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}

	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	numParams = jit_type_num_params(signature);
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	totalParams = numParams;
#else
	/* otherwise we need the thread as an additional argument. */
	totalParams = numParams + 1;
#endif
	returnType = jit_type_get_return(signature);

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	if(numParams != (numArgs + 1))
#else
	if(numParams != numArgs)
#endif
	{
		printf("Number of args doesn't match: signature: %i - numArgs: %i\n", numParams, numArgs);
	}

	{
		/* We need to set the method member in the ILExecThread == arg[0]. */
		ILJitType paramType;
		ILJitType jitParamTypes[totalParams + 1];
		ILJitValue jitParams[totalParams + 1];
		ILUInt32 param = 1;

		/* We need to create a new Signature for the native Call with */
		/* an additional argument when the return value is a value type */
		/* and pointers for structs. */
		/* Set the current thread as arg[0]. */
		jitParamTypes[0] = _IL_JIT_TYPE_VPTR;
		jitParams[0] = thread;

		/* Check if the return type is a value type. */
		if(ILType_IsValueType(type) && (returnType != _ILJitTypedRef))
		{
			++totalParams;
			jitParamTypes[1] = _IL_JIT_TYPE_VPTR;
			returnValue = jit_value_create(jitCoder->jitFunction, returnType);
			jitParams[1] = jit_insn_address_of(jitCoder->jitFunction,
											   returnValue);
			returnType = _IL_JIT_TYPE_VOID;
			hasStructReturn = 1;
			++param;
		}

	#ifdef IL_JIT_THREAD_IN_SIGNATURE
		for(current = 1; current < numParams; ++current)
	#else
		for(current = 0; current < numParams; ++current)
	#endif
		{
			paramType = jit_type_get_param(signature, current);

			if(jit_type_is_struct(paramType) && (paramType != _ILJitTypedRef))
			{
				jitParamTypes[param] = _IL_JIT_TYPE_VPTR;
			#ifdef IL_JIT_THREAD_IN_SIGNATURE
				jitParams[param] = jit_insn_address_of(jitCoder->jitFunction,
													   args[current - 1]);
			#else
				jitParams[param] = jit_insn_address_of(jitCoder->jitFunction,
													   args[current]);
			#endif
			}
			else
			{
				jitParamTypes[param] = paramType;
			#ifdef IL_JIT_THREAD_IN_SIGNATURE
				args[current - 1] = _ILJitValueConvertImplicit(jitCoder->jitFunction,
															   args[current - 1],
															   paramType);
				jitParams[param] = args[current - 1];
		
			#else
				args[current] = _ILJitValueConvertImplicit(jitCoder->jitFunction,
														   args[current],
														   paramType);
				jitParams[param] = args[current];
			#endif
			}
			++param;
		}
		callSignature = jit_type_create_signature(IL_JIT_CALLCONV_CDECL,
												  returnType,
												  jitParamTypes,
												  totalParams, 1);

		_ILJitSetMethodInThread(jitCoder->jitFunction, thread, method);
		_ILJitBeginNativeCall(jitCoder->jitFunction, thread);
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
		if(!hasStructReturn)
		{
			returnValue = jit_insn_call_native(jitCoder->jitFunction,
											   methodName, nativeFunction,
											   callSignature,
											   jitParams, totalParams, 0);
		}
		else
		{
			jit_insn_call_native(jitCoder->jitFunction, methodName,
								 nativeFunction, callSignature,
							 	 jitParams, totalParams, 0);
		}
		jit_type_free(callSignature);
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
		/*
		 * Emit the code to end profiling of the method if profiling
		 * is enabled
		 */
		if(jitCoder->flags & IL_CODER_FLAG_METHOD_PROFILE)
		{
			_ILJitProfileEnd(jitCoder, method);
		}
#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */
	}
	_ILJitEndNativeCall(jitCoder->jitFunction, thread);

	return returnValue;
}

/*
 * Generate the stub for calling an internal function.
 */
static int _ILJitCompileInternal(ILJitFunction func)
{
	ILMethod *method = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);
	ILClassPrivate *classPrivate = (ILClassPrivate *)(ILMethod_Owner(method)->userData);
	ILJITCoder *jitCoder = (ILJITCoder *)(classPrivate->process->coder);
	ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)(method->userData);
	ILJitType signature = jit_function_get_signature(func);
	unsigned int numParams = jit_type_num_params(signature);
	ILJitValue returnValue = 0;
	ILJitValue thread = _ILJitFunctionGetThread(func);
	char *methodName = 0;
	ILJitValue paramValue;
	ILUInt32 current;
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	ILJitValue jitParams[numParams - 1];
#else
	ILJitValue jitParams[numParams];
#endif

	/* Set the current function in the coder */
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

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
	methodName = _ILJitFunctionGetMethodName(func);
	if(jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout, "CompileInternalMethod: %s\n", methodName);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	for(current = 1; current < numParams; ++current)
	{
		if(!(paramValue = jit_value_get_param(jitCoder->jitFunction, current)))
		{
			return JIT_RESULT_OUT_OF_MEMORY;
		}
		jitParams[current - 1] = paramValue;
	}
	returnValue = _ILJitCallInternal(jitCoder, thread, method,
					 				 jitMethodInfo->fnInfo.un.func,
									 methodName,
									 jitParams, numParams - 1);
#else
	for(current = 0; current < numParams; ++current)
	{
		if(!(paramValue = jit_value_get_param(jitCoder->jitFunction, current)))
		{
			return JIT_RESULT_OUT_OF_MEMORY;
		}
		jitParams[current] = paramValue;
	}
	returnValue = _ILJitCallInternal(jitCoder, thread, method,
									 jitMethodInfo->fnInfo.un.func,
									 methodName,
									 jitParams, numParams);
#endif
	jit_insn_return(jitCoder->jitFunction, returnValue);

	return JIT_RESULT_OK;
}

/*
 * On demand code generator.for functions implemented in IL code.
 */
static int _ILJitCompile(jit_function_t func)
{
	ILExecThread *thread = ILExecThreadCurrent();
	ILMethod *method = (ILMethod *)jit_function_get_meta(func, IL_JIT_META_METHOD);

	if(!method)
	{
		METADATA_UNLOCK(thread->process);
		return JIT_RESULT_COMPILE_ERROR;
	}

	if(!_ILConvertMethod(thread, method))
	{
		return JIT_RESULT_COMPILE_ERROR;
	}
	return JIT_RESULT_OK;
}

/*
 * Check if the given method is abstract (should have no implementation).
 */
static int _ILJitMethodIsAbstract(ILMethod *method)
{
	if(!method)
	{
		/* This is obviously a bug and should not happen. */
		return 0;
	}

	if(ILMethod_IsAbstract(method) ||
	   ILClass_IsInterface(ILMethod_Owner(method)))
	{
		return 1;
	}
	return 0;
}

#include "jitc_pinvoke.c"

/*
 * Create the signature type for an ILMethod.
 */
static ILJitType _ILJitCreateMethodSignature(ILJITCoder *coder,
											 ILMethod *method,
											 ILType* signature,
											 ILUInt32 implementationType)
{
	ILType *type;
	/* number of args in the bytecode */
	/* Argument 0 is the type of the return value. */
	ILUInt32 num = 0;
	/* total number of args */
	ILUInt32 total;
	ILUInt32 jitArgc;
	ILUInt32 current;
	/* calling convention for this function. */
	/* Hold the abi to use for libjit. */
	jit_abi_t jitAbi;
	/* JitType to hold the return type */
	ILJitType jitReturnType;
	/* The type of the jit signature for this function. */
	ILJitType jitSignature;
	/* Flag if this is an array or string constructor. */
	int isArrayOrString = 0;
	/* Flag if the method is a ctor. */
	int isCtor = 0;
	/* Some infos that we'll need later. */
	ILClass *info = 0;
#ifdef IL_CONFIG_VARARGS
	/* Flag to check if this is an internal vararg method. */
	ILInt32 isInternalVararg = 0;
#endif

	/* Get the information needed from the method. */
	if(method)
	{
		signature = ILMethod_Signature(method);
		isCtor = ILMethodIsConstructor(method);
		info = ILMethod_Owner(method);
	}

	/* Get the number of args in the signature. */
	num = ILTypeNumParams(signature);
	/* Get the total number of args for the jit signature. */
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	/* because we pass the ILExecThread as first arg we have to add one */
	total = num + 1;
	/* We set jitArgc to 1 because we allways pass the current ILExecThread */
	/* in jitArgs[0]. */
	jitArgc = 1;
#else
	total = num;
	jitArgc = 0;
#endif

	/* Get the calling convention to use for this method/call. */
	switch((ILType_CallConv(signature) & IL_META_CALLCONV_MASK))
	{
		case IL_META_CALLCONV_DEFAULT:
		{
			jitAbi = IL_JIT_CALLCONV_DEFAULT;
		}
		break;

		case IL_META_CALLCONV_C:
		{
			jitAbi = IL_JIT_CALLCONV_CDECL;
		}
		break;

		case IL_META_CALLCONV_STDCALL:
		{
			jitAbi = IL_JIT_CALLCONV_STDCALL;
		}
		break;

		case IL_META_CALLCONV_THISCALL:
		{
			/* This calling convention is not yet supported with libjit. */
			/* So we use cdecl instead. */
			jitAbi = IL_JIT_CALLCONV_CDECL;
		}
		break;

		case IL_META_CALLCONV_FASTCALL:
		{
			jitAbi = IL_JIT_CALLCONV_FASTCALL;
		}
		break;

		case IL_META_CALLCONV_VARARG:
		{
		#ifdef IL_CONFIG_VARARGS
			if((implementationType & _IL_JIT_IMPL_PINVOKE) != 0)
			{
				/* Methods not implemented internal must have the vararg calling */
				/* convertion. */
				jitAbi = IL_JIT_CALLCONV_VARARG;
			}
			else
			{
				/* For internal vararg methods we add an additional arg and */
				/* use the default calling convention instead. */
				jitAbi = IL_JIT_CALLCONV_DEFAULT;
				total++;
				isInternalVararg = 1;
			}
		#else
			/* Vararg calls are not supported. */
			return 0;
		#endif
		}
		break;

		default:
		{
			/* Invalid calling convention. */
			return 0;
		}
		break;

	}

	/* Check if the method has a not explicit this argument. */
	/* This is true only for non function pointer signatures. */
	if(ILType_HasThis(signature))
	{
		if(!isCtor)
		{
			/* we need an other arg for this */
			total++;
		}
		else
		{
			if(implementationType != _IL_JIT_IMPL_INTERNALALLOC)
			{
				/* we need an other arg for this */
				total++;
			}
			else
			{
				isArrayOrString = 1;
			}
		}
	}

	/* Array to hold the parameter types. */
	ILJitType jitArgs[total];

	/* Get the return type for this function */
	if(isCtor && isArrayOrString)
	{
		type = ILType_FromClass(info);
	}
	else
	{
		type = ILTypeGetEnumType(ILTypeGetParam(signature, 0));
	}
	if(!(jitReturnType = _ILJitGetReturnType(type, coder->process)))
	{
		return 0;
	}

#ifdef IL_JIT_THREAD_IN_SIGNATURE
	/* arg 0 is allways the ILExecThread */
	jitArgs[0] = _IL_JIT_TYPE_VPTR;
#endif

	if(ILType_HasThis(signature))
	{
		if(!isCtor || !isArrayOrString)
		{
			/* We need to setup the this arg */
			/* determine the type of this arg */
			if(!(type = ILType_FromClass(info)))
			{
				return 0;
			}
			/* at this time the type must be layouted or at least partially layouted */
		#ifdef IL_JIT_THREAD_IN_SIGNATURE
			if(!(jitArgs[1] = _ILJitGetThisType(type, coder->process)))
		#else
			if(!(jitArgs[0] = _ILJitGetThisType(type, coder->process)))
		#endif
			{
				return 0;
			}
			jitArgc++;
		}
	}

	/* Get the jit types for the regular arguments */
	for(current = 1; current <= num; ++current)
	{
		type = ILTypeGetEnumType(ILTypeGetParam(signature, current));
		if(!(jitArgs[jitArgc] = _ILJitGetArgType(type, coder->process)))
		{
			return 0;
		}
		jitArgc++;
	}

#ifdef IL_CONFIG_VARARGS
	/* If it's an internal vararg method add the pointer to the vararg */
	/* information. */
	if(isInternalVararg)
	{
		jitArgs[jitArgc] = _IL_JIT_TYPE_VPTR;
		jitArgc++;
	}
#endif

	if(!(jitSignature = jit_type_create_signature(jitAbi, jitReturnType,
													jitArgs, jitArgc, 1)))
	{
		return 0;
	}
	return jitSignature;
}

/*
 * Fill the MethodInfo and set the corresponding on demand compiler.
 */
static int _ILJitSetMethodInfo(ILJITCoder *jitCoder, ILMethod *method,
													 ILJitType jitSignature)
{
	ILClass *info;
	ILClassPrivate *classPrivate;
	ILUInt32 implementationType = 0;
	jit_on_demand_func onDemandCompiler = 0;
	ILJitInlineFunc inlineFunc = 0;
	ILInt32 setRecompilable = 0;
	ILInternalInfo fnInfo;

	if(!method)
	{
		return 0;
	}

	/* Get the method's owner class. */
	info = ILMethod_Owner(method);
	classPrivate = (ILClassPrivate *)(info->userData);
	ILMemZero(&fnInfo, sizeof(ILInternalInfo));

	if(classPrivate->jitTypes.jitTypeKind != 0)
	{
		switch(classPrivate->jitTypes.jitTypeKind)
		{
			case IL_JIT_TYPEKIND_ARRAY:
			{
				if(!strcmp(ILMethod_Name(method), "Get"))
				{
					inlineFunc = _ILJitMArrayGet;
				}
				else if(!strcmp(ILMethod_Name(method), "Set"))
				{
					inlineFunc = _ILJitMArraySet;
				}
				else if(!strcmp(ILMethod_Name(method), "Address"))
				{
					inlineFunc = _ILJitMArrayAddress;
				}
			}
			break;

			case IL_JIT_TYPEKIND_SYSTEM_MATH:
			{
				ILType *signature = ILMethod_Signature(method);
				ILType *returnType;

				if(!signature)
				{
					break;
				}
				if(!(returnType = ILTypeGetEnumType(ILTypeGetParam(signature, 0))))
				{
					break;
				}

				if(ILType_IsPrimitive(returnType))
				{
					if(!strcmp(ILMethod_Name(method), "Abs"))
					{
						inlineFunc = _ILJitSystemMathAbs;
					}
					else if(!strcmp(ILMethod_Name(method), "Acos"))
					{
						inlineFunc = _ILJitSystemMathAcos;
					}
					else if(!strcmp(ILMethod_Name(method), "Asin"))
					{
						inlineFunc = _ILJitSystemMathAsin;
					}
					else if(!strcmp(ILMethod_Name(method), "Atan"))
					{
						inlineFunc = _ILJitSystemMathAtan;
					}
					else if(!strcmp(ILMethod_Name(method), "Atan2"))
					{
						inlineFunc = _ILJitSystemMathAtan2;
					}
					else if(!strcmp(ILMethod_Name(method), "Ceiling"))
					{
						inlineFunc = _ILJitSystemMathCeiling;
					}
					else if(!strcmp(ILMethod_Name(method), "Cos"))
					{
						inlineFunc = _ILJitSystemMathCos;
					}
					else if(!strcmp(ILMethod_Name(method), "Cosh"))
					{
						inlineFunc = _ILJitSystemMathCosh;
					}
					else if(!strcmp(ILMethod_Name(method), "Exp"))
					{
						inlineFunc = _ILJitSystemMathExp;
					}
					else if(!strcmp(ILMethod_Name(method), "Floor"))
					{
						inlineFunc = _ILJitSystemMathFloor;
					}
					else if(!strcmp(ILMethod_Name(method), "IEEERemainder"))
					{
						inlineFunc = _ILJitSystemMathIEEERemainder;
					}
					else if(!strcmp(ILMethod_Name(method), "Log"))
					{
						ILUInt32 num = ILTypeNumParams(signature);

						if(num == 1)
						{
							inlineFunc = _ILJitSystemMathLog;
						}
					}
					else if(!strcmp(ILMethod_Name(method), "Log10"))
					{
						inlineFunc = _ILJitSystemMathLog10;
					}
					else if(!strcmp(ILMethod_Name(method), "Max"))
					{
						inlineFunc = _ILJitSystemMathMax;
					}
					else if(!strcmp(ILMethod_Name(method), "Min"))
					{
						inlineFunc = _ILJitSystemMathMin;
					}
					else if(!strcmp(ILMethod_Name(method), "Pow"))
					{
						inlineFunc = _ILJitSystemMathPow;
					}
					else if(!strcmp(ILMethod_Name(method), "Sign"))
					{
						ILUInt32 num = ILTypeNumParams(signature);

						if(num == 1)
						{
							ILType *argType;

							if(!(argType = ILTypeGetParam(signature, 1)))
							{
								break;
							}
							if(ILType_IsPrimitive(argType))
							{
								inlineFunc = _ILJitSystemMathSign;
							}
						}
					}
					else if(!strcmp(ILMethod_Name(method), "Sin"))
					{
						inlineFunc = _ILJitSystemMathSin;
					}
					else if(!strcmp(ILMethod_Name(method), "Sinh"))
					{
						inlineFunc = _ILJitSystemMathSinh;
					}
					else if(!strcmp(ILMethod_Name(method), "Sqrt"))
					{
						inlineFunc = _ILJitSystemMathSqrt;
					}
					else if(!strcmp(ILMethod_Name(method), "Tan"))
					{
						inlineFunc = _ILJitSystemMathTan;
					}
					else if(!strcmp(ILMethod_Name(method), "Tanh"))
					{
						inlineFunc = _ILJitSystemMathTanh;
					}
				}
			}
			break;

			case IL_JIT_TYPEKIND_SYSTEM_STRING:
			{
				ILType *signature = ILMethod_Signature(method);

				if(ILMethod_IsStatic(method) &&
				   !strcmp(ILMethod_Name(method), "NewString") &&
				   _ILLookupTypeMatch(signature, "(i)oSystem.String;"))
				{
						inlineFunc = _ILJitSystemStringNew;
				}
				else if(!ILMethod_IsStatic(method) &&
						!strcmp(ILMethod_Name(method), "get_Chars") &&
						_ILLookupTypeMatch(signature, "(Ti)c"))
				{
						inlineFunc = _ILJitSystemStringChars;
				}
			}
			break;
		}
	}

	if(!onDemandCompiler)
	{
		ILMethodCode code;

		/* Get the method code */
		if(!ILMethodGetCode(method, &code))
		{
			code.code = 0;
		}
	
		/* Check if the method is implemented in IL. */
		if(code.code)
		{
			/* Flag method implemented in IL.. */
			implementationType = _IL_JIT_IMPL_DEFAULT;

			/* set the function recompilable. */
			setRecompilable = 1;

			/* now set the on demand compiler function */
			onDemandCompiler = _ILJitCompile;
		}
		else
		{
			/* This is a "PInvoke", "internalcall", or "runtime" method */
			ILExecThread *thread = ILExecThreadCurrent();
			ILPInvoke *pinv = ILPInvokeFind(method);
			int isConstructor = ILMethod_IsConstructor(method);;
		#ifdef IL_CONFIG_PINVOKE
			ILModule *module;
			const char *name;
			void *moduleHandle;
		#endif

			switch(method->implementAttrs &
						(IL_META_METHODIMPL_CODE_TYPE_MASK |
						 IL_META_METHODIMPL_INTERNAL_CALL |
						 IL_META_METHODIMPL_JAVA))
			{
				case IL_META_METHODIMPL_IL:
				case IL_META_METHODIMPL_OPTIL:
				{
					/* If we don't have a PInvoke record, then we don't
					   know what to map this method call to */
					if(!pinv)
					{
						return 0;
					}

				#ifdef IL_CONFIG_PINVOKE
					/* Find the module for the PInvoke record */
					module = ILPInvoke_Module(pinv);
					if(!module)
					{
						return 0;
					}
					name = ILModule_Name(module);
					if(!name || *name == '\0')
					{
						return 0;
					}
					moduleHandle = LocateExternalModule
									(ILExecThreadGetProcess(thread), name, pinv);
					if(moduleHandle)
					{

						/* Get the name of the function within the module */
						name = ILPInvoke_Alias(pinv);
						if(!name || *name == '\0')
						{
							name = ILMethod_Name(method);
						}

					#ifdef IL_WIN32_PLATFORM

						if(!(pinv->member.attributes & IL_META_PINVOKE_NO_MANGLE))
						{
							/* We have to append an A or W to the function */
							/* name depending on the characterset used. */
							/* On Windows we have only either Ansi or Utf16 */
							int nameLength = strlen(name);
							ILUInt32 charSetUsed = ILPInvokeGetCharSet(pinv, method);
							char newName[nameLength + 2];

							strcpy(newName, name);
							if(charSetUsed == IL_META_MARSHAL_UTF16_STRING)
							{
								newName[nameLength] = 'W';
							}
							else
							{
								newName[nameLength] = 'A';
							}
							newName[nameLength + 1] = '\0';

							/* Look up the method within the module */
							fnInfo.un.func = ILDynLibraryGetSymbol(moduleHandle, newName);
						}
						if(!fnInfo.un.func)
						{
							/* Look up the method within the module */
							fnInfo.un.func = ILDynLibraryGetSymbol(moduleHandle, name);
						}

						if(!(fnInfo.un.func))
						{
							fnInfo.un.func = _IL_JIT_PINVOKE_ENTRYPOINTNOTFOUND;
						}

					#else	/* !IL_WIN32_PLATFORM */

						/* Look up the method within the module */
						fnInfo.un.func = ILDynLibraryGetSymbol(moduleHandle, name);

						if(!(fnInfo.un.func))
						{
							fnInfo.un.func = _IL_JIT_PINVOKE_ENTRYPOINTNOTFOUND;
						}
					#endif	/* !IL_WIN32_PLATFORM */
					}
					else
					{
						fnInfo.un.func = _IL_JIT_PINVOKE_DLLNOTFOUND;
					}

					/* Flag the method pinvoke. */
					implementationType = _IL_JIT_IMPL_PINVOKE;

					/* now set the on demand compiler function */
					onDemandCompiler = _ILJitCompilePinvoke;

				#else /* !IL_CONFIG_PINVOKE */
					return 0;
				#endif /* IL_CONFIG_PINVOKE */
				}
				break;

				case IL_META_METHODIMPL_RUNTIME:
				case IL_META_METHODIMPL_IL | IL_META_METHODIMPL_INTERNAL_CALL:
				{
					/* "internalcall" and "runtime" methods must not
				   	have PInvoke records associated with them */
					if(pinv)
					{
						return 0;
					}

					/* Look up the internalcall function details */
					if(isConstructor)
					{
						if(ILClassIsValueType(info))
						{
							_ILFindInternalCall(_ILExecThreadProcess(thread),
												method, 0, &fnInfo);
							if(fnInfo.un.func)
							{
								/* Flag the method internal. */
								implementationType = _IL_JIT_IMPL_INTERNAL;
							}
						}
						else
						{
							if(!_ILFindInternalCall(_ILExecThreadProcess(thread),
													method, 0, &fnInfo))
							{
								if(!_ILFindInternalCall(_ILExecThreadProcess(thread),
														method, 1, &fnInfo))
								{
									implementationType = _IL_JIT_IMPL_DEFAULT;
								}
								if(fnInfo.flags == _IL_INTERNAL_GENCODE)
								{
									implementationType = _IL_JIT_IMPL_DEFAULT;
								}
								else if(fnInfo.un.func)
								{
									/* Flag the method an allocating constructor. */
									implementationType = _IL_JIT_IMPL_INTERNALALLOC;
								}
							}
							else
							{
								if(fnInfo.flags == _IL_INTERNAL_GENCODE)
								{
									implementationType = _IL_JIT_IMPL_DEFAULT;
								}
								else if(fnInfo.un.func)
								{
									/* Flag the method internal. */
									implementationType = _IL_JIT_IMPL_INTERNAL;
								}
								else
								{
									if(!_ILFindInternalCall(_ILExecThreadProcess(thread),
															method, 1, &fnInfo))
									{
										implementationType = _IL_JIT_IMPL_DEFAULT;
									}
									if(fnInfo.flags == _IL_INTERNAL_GENCODE)
									{
										implementationType = _IL_JIT_IMPL_DEFAULT;
									}
									else if(fnInfo.un.func)
									{
										/* Flag the method an allocating constructor. */
										implementationType = _IL_JIT_IMPL_INTERNALALLOC;
									}
								}
							}
						}
					}
					else
					{
						if(!_ILFindInternalCall(_ILExecThreadProcess(thread),
											method, 0, &fnInfo))
						{
							implementationType = _IL_JIT_IMPL_DEFAULT;
						}
						else
						{
							if(fnInfo.flags == _IL_INTERNAL_GENCODE)
							{
								implementationType = _IL_JIT_IMPL_DEFAULT;
							}
							else if(fnInfo.un.func)
							{
								/* Flag the method internal. */
								implementationType = _IL_JIT_IMPL_INTERNAL;
							}
						}
					}

					/* Bail out if the native method could not be found. */
					if(fnInfo.flags == _IL_INTERNAL_NATIVE && !(fnInfo.un.func))
					{
						return 0;
					}

					/* now set the on demand compiler. */
					if(fnInfo.flags == _IL_INTERNAL_GENCODE)
					{
						onDemandCompiler = _ILJitCompile;
					}
					else
					{
						onDemandCompiler = _ILJitCompileInternal;
					}
				}
				break;

				default:
				{
					/* No idea how to invoke this method */
					return 0;
				}
				/* Not reached */
			}
		}
	}

	if(onDemandCompiler)
	{
		ILJitMethodInfo *jitMethodInfo = 0;
		ILInt32 signatureCreated = 0;
		ILJitFunction jitFunction = 0;

		if(!jitSignature)
		{
			if(!(jitSignature = _ILJitCreateMethodSignature(jitCoder,
															method,
															0,
															implementationType)))
			{
				return 0;
			}
			signatureCreated = 1;
		}
	
		/* Now we can create the jit function itself. */
		/* We must be able to create jit function prototypes while an other */
		/* function is on demand compiled. */
		if(!(jitFunction = jit_function_create(jitCoder->context, jitSignature)))
		{
			if(signatureCreated)
			{
				ILJitTypeDestroy(jitSignature);
			}
			return 0;
		}

		/* Set the ILMethod in the new functions metadata. */
		/* Because there has nothing to be freed we can use 0 for the free_func. */
		if(!jit_function_set_meta(jitFunction, IL_JIT_META_METHOD, method, 0, 0))
		{
			if(signatureCreated)
			{
				ILJitTypeDestroy(jitSignature);
			}
			return 0;
		}
	
		if(!(jitMethodInfo = ILMemPoolAlloc(&(jitCoder->methodPool),
											ILJitMethodInfo)))
		{
			if(signatureCreated)
			{
				ILJitTypeDestroy(jitSignature);
			}
			return 0;
		}

		/* Copy the infos to the MethodInfo structure. */
		jitMethodInfo->jitFunction = jitFunction;
		jitMethodInfo->implementationType = implementationType;
		jitMethodInfo->fnInfo = fnInfo;
		jitMethodInfo->inlineFunc = inlineFunc;

		/* and link the new jitFunction to the method. */
		method->userData = (void *)jitMethodInfo;

		if(setRecompilable)
		{
			/* set the function recompilable. */
			jit_function_set_recompilable(jitFunction);
		}
		/* now set the on demand compiler function */
		jit_function_set_on_demand_compiler(jitFunction, onDemandCompiler);
											
	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
		_ILJitFunctionSetMethodName(jitFunction, method);
	#endif

		return 1;
	}
	return 0;
}

/*
 * Create the jit function header for an ILMethod.
 * We allways pass the ILExecThread as arg 0.
 */
int ILJitFunctionCreate(ILCoder *_coder, ILMethod *method)
{
	ILJITCoder *coder = ((ILJITCoder *)_coder);

	/* Don't create the jit function twice. */
	if(method->userData)
	{
		return 1;
	}

	if(_ILJitMethodIsAbstract(method))
	{
		/* This Method is abstract so we do nothing. */
		return 1;
	}

	_ILJitSetMethodInfo(coder, method, 0);

	/* are we ready now ? */

	return 1;
}

/*
 * Create the jit function header for an ILMethod with the information from
 * a virtual ancestor.
 * We can reuse the signature in this case.
 */
int ILJitFunctionCreateFromAncestor(ILCoder *_coder, ILMethod *method,
													 ILMethod *virtualAncestor)
{
	ILJITCoder *jitCoder = ((ILJITCoder *)_coder);
	ILJitMethodInfo *ancestorInfo = (ILJitMethodInfo *)(virtualAncestor->userData);
	ILJitType jitSignature;

	/* Don't create the jit function twice. */
	if(method->userData)
	{
		return 1;
	}

	if(_ILJitMethodIsAbstract(method))
	{
		/* This Method is abstract so we do nothing. */
		return 1;
	}

	if(!ancestorInfo)
	{
		/* Tha ancestor has no jit function (might be abstract). */
		/* So we need to do it the hard way. */
		return ILJitFunctionCreate(_coder, method);
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CreateMethodFromAncestor: Ancestor: %s.%s at Slot %i New: %s.%s\n", 
			ILClass_Name(ILMethod_Owner(virtualAncestor)),
			ILMethod_Name(virtualAncestor),
			virtualAncestor->index,
			ILClass_Name(ILMethod_Owner(method)),
			ILMethod_Name(method));
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	jitSignature = jit_function_get_signature(ancestorInfo->jitFunction);

	_ILJitSetMethodInfo(jitCoder, method, jitSignature);

	/* are we ready now ? */

	return 1;
}

/*
 * Get a pointer for a method suitable for a vtable.
 * Returns 0 on error.
 */
void *ILJitGetVtablePointer(ILCoder *_coder, ILMethod *method)
{
	if(!ILJitFunctionCreate(_coder, method))
	{
		return  0;
	}
	return jit_function_to_vtable_pointer(ILJitFunctionFromILMethod(method));
}

/*
 * Create all jitMethods for the given class.
 * Returns 0 on error.
 */
int ILJitCreateFunctionsForClass(ILCoder *_coder, ILClass *info)
{
	int result = 1;

	/* we do not need to create functions for interfaces. */
	if((info->attributes & IL_META_TYPEDEF_CLASS_SEMANTICS_MASK) !=
				IL_META_TYPEDEF_INTERFACE)
	{
		ILMethod *method;

		method = 0;
		while((method = (ILMethod *)ILClassNextMemberByKind
				(info, (ILMember *)method, IL_META_MEMBERKIND_METHOD)) != 0)
		{
			if(!ILJitFunctionCreate(_coder, method))
			{
				return  0;
			}
		}
	}
	return result;
}

/*
 * Call the jit function for a ILMethod.
 * Returns 1 if an exception occured.
 */
int ILJitCallMethod(ILExecThread *thread, ILMethod *method, void **jitArgs, void *result)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);

	if(!jitFunction)
	{
		/* We have to layout the class. */
		if(!_LayoutClass(thread, ILMethod_Owner(method)))
		{
			return 0;
		}
		jitFunction = ILJitFunctionFromILMethod(method);
		if(!jitFunction)
		{
			/* This can be a generic method instance. */
			if(!ILJitFunctionCreate(thread->process->coder, method))
			{
				return 0;
			}
			jitFunction = ILJitFunctionFromILMethod(method);
		}
	}

	if(!jit_function_apply(jitFunction, jitArgs, result))
	{
		return 0;
	}
	return 1;
}

/*
 * Get the ILJitFunction for an ILMethod.
 * Returns 0 if the jit function stub isn't created yet.
 */
ILJitFunction ILJitFunctionFromILMethod(ILMethod *method)
{
	if(method)
	{
		ILJitMethodInfo *jitMethodInfo = (ILJitMethodInfo *)(method->userData);

		if(jitMethodInfo)
		{
			return jitMethodInfo->jitFunction;
		}
	}
	return 0;
}

/*
 * Handle special cases of the Type creation like handles, ...
 * Returns the jit type for the special case or 0 if is no special case.
 */
static ILJitType _ILJitTypeSpecials(ILClassName *className)
{
	if(className && className->namespace &&
	   !strcmp(className->namespace, "System"))
	{
		if(!strcmp(className->name, "IntPtr"))
		{
			return _IL_JIT_TYPE_NINT;
		}
		if(!strcmp(className->name, "UIntPtr"))
		{
			return _IL_JIT_TYPE_NUINT;
		}
		if(!strcmp(className->name, "RuntimeTypeHandle"))
		{
			return _IL_JIT_TYPE_NINT;
		}
		if(!strcmp(className->name, "RuntimeMethodHandle"))
		{
			return _IL_JIT_TYPE_NINT;
		}
		if(!strcmp(className->name, "RuntimeFieldHandle"))
		{
			return _IL_JIT_TYPE_NINT;
		}
		if(!strcmp(className->name, "RuntimeArgumentHandle"))
		{
			return _IL_JIT_TYPE_NINT;
		}
		if(!strcmp(className->name, "TypedRef"))
		{
			return _ILJitTypedRef;
		}
	}
	return 0;
}

/*
 * Create the type representation including fields for libjit.
 */
static ILJitType _ILJitTypeCreate(ILClass *info, ILExecProcess *process)
{
	ILJitType jitType = 0;
	ILField *field;
	ILUInt32 numFields = 0;

	/* The parent class is always the first field */
	if(info->parent)
	{
		++numFields;
	}

	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
			(info, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if((field->member.attributes & IL_META_FIELDDEF_STATIC) == 0)
		{
			++numFields;
		}
	}

	if(numFields > 0)
	{
		ILInt32 jitTypeIsUnion = 1;
		ILUInt32 currentField = 0;
		ILJitType fields[numFields];
		ILUInt32 offsets[numFields];

		if(info->parent)
		{
			/* The parent class must be laid out at this point */
			ILClass *parent = ILClass_ParentClass(info);

			if(parent && parent->userData)
			{
				fields[currentField] = ((ILClassPrivate *)(parent->userData))->jitTypes.jitTypeBase;
				/* Check if the size of the base type is > 0 */
				/* (it is not System.Object or System.ValueType for example */
				if(jit_type_get_size(fields[currentField]) > 0)
				{
					offsets[currentField] = 0;
					++currentField;
				}
			}
			else
			{
				return 0;
			}
		}

		field = 0;
		while((field = (ILField *)ILClassNextMemberByKind
				(info, (ILMember *)field, IL_META_MEMBERKIND_FIELD)) != 0)
		{
			if((field->member.attributes & IL_META_FIELDDEF_STATIC) == 0)
			{
				ILType *type = field->member.signature;
				ILJitTypes *jitTypes;

				type = ILTypeStripPrefixes(type);
				if(ILType_IsClass(type))
				{
					jitTypes = ILJitPrimitiveClrTypeToJitTypes(IL_META_ELEMTYPE_PTR);
				}
				else
				{
					jitTypes = _ILJitGetTypes(type, process);
				}
				if(!jitTypes)
				{
					return 0;
				}
				fields[currentField] = jitTypes->jitTypeBase;
				offsets[currentField] = field->offset;
				jitTypeIsUnion &= (field->offset == 0);
				++currentField;
			}
		}
		if(jitTypeIsUnion && (currentField > 1))
		{
			/* All fields have a 0 offset */
			jitType = jit_type_create_union(fields, currentField, 0);
		}
		else
		{
			jitType = jit_type_create_struct(fields, currentField, 0);
			if(!jitType)
			{
				return 0;
			}
			/* Set the offsets of the fields in the struct */
			for(currentField = 0; currentField < numFields; currentField++)
			{
				jit_type_set_offset(jitType, currentField, offsets[currentField]);
			}
		}
	}
	else
	{
		/* This must be the top level System.Object class. */
		jitType = jit_type_create_struct(0, 0, 0);
	}

	return jitType;
}

/*
 * Create the class/struct representation of a clr type for libjit.
 * and store the type in classPrivate.
 * Returns the 1 on success else 0
 */
int ILJitTypeCreate(ILClassPrivate *classPrivate, ILExecProcess *process)
{
	ILJitType jitType = 0;

	if(classPrivate->size >= 0)
	{
		ILType *type = ILClassToType(classPrivate->classInfo);

		/* If it's a runtime object check if it has to be handled special. */
		if(process->context->systemImage == classPrivate->classInfo->programItem.image)
		{
			jitType = _ILJitTypeSpecials(classPrivate->classInfo->className);
		}
		if(!jitType)
		{
			if(!(jitType = _ILJitTypeCreate(classPrivate->classInfo, process)))
			{
				return 0;
			}
			jit_type_set_size_and_alignment(jitType,
											classPrivate->size,
											classPrivate->alignment);
		}
		classPrivate->jitTypes.jitTypeBase = jitType;
		/* Check if it's one of the classes thet need special handling. */
		if(ILTypeIsDelegateSubClass(type))
		{
			if(ILTypeIsDelegate(type))
			{
				/* This is a subclass of System.MulticastDelegate. */
				classPrivate->jitTypes.jitTypeKind = IL_JIT_TYPEKIND_MULTICASTDELEGATE;
			}
			else
			{
				classPrivate->jitTypes.jitTypeKind = IL_JIT_TYPEKIND_DELEGATE;
			}
		}
		else if(ILType_IsArray(type))
		{
			classPrivate->jitTypes.jitTypeKind = IL_JIT_TYPEKIND_ARRAY;
		}
		else if(process->context->systemImage == classPrivate->classInfo->programItem.image)
		{
			if(ILClass_Namespace(classPrivate->classInfo))
			{
				/* Check for classes in the System namespace which have inline methods. */
				if(!strcmp(ILClass_Namespace(classPrivate->classInfo), "System"))
				{
					if(!strcmp(ILClass_Name(classPrivate->classInfo), "Array"))
					{
						classPrivate->jitTypes.jitTypeKind = IL_JIT_TYPEKIND_SYSTEM_ARRAY;
					}
					else if(!strcmp(ILClass_Name(classPrivate->classInfo), "Math"))
					{
						classPrivate->jitTypes.jitTypeKind = IL_JIT_TYPEKIND_SYSTEM_MATH;
					}
					else if(!strcmp(ILClass_Name(classPrivate->classInfo), "String"))
					{
						classPrivate->jitTypes.jitTypeKind = IL_JIT_TYPEKIND_SYSTEM_STRING;
					}
				}
			}
		}
	}
	else
	{
		return 0;
	}
	return 1;
}

/*
 * Destroy the given type in libjit.
 */
void ILJitTypeDestroy(ILJitType type)
{
	jit_type_free(type);	
}

/*
 * Get the jit type for a primitive clr type.
 * Returns 0 if the type is no primitive clr type.
 */
ILJitTypes *ILJitPrimitiveClrTypeToJitTypes(int primitiveClrType)
{
	switch(primitiveClrType)
	{
		case IL_META_ELEMTYPE_VOID:
		{
			return &_ILJitType_VOID;
		}
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			return &_ILJitType_BOOLEAN;
		}
		case IL_META_ELEMTYPE_I1:
		{
			return &_ILJitType_SBYTE;
		}
		case IL_META_ELEMTYPE_U1:
		{
			return &_ILJitType_BYTE;
		}
		case IL_META_ELEMTYPE_CHAR:
		{
			return &_ILJitType_CHAR;
		}
		case IL_META_ELEMTYPE_I2:
		{
			return &_ILJitType_I2;
		}
		case IL_META_ELEMTYPE_U2:
		{
			return &_ILJitType_U2;
		}
		case IL_META_ELEMTYPE_I4:
		{
			return &_ILJitType_I4;
		}
		case IL_META_ELEMTYPE_U4:
		{
			return &_ILJitType_U4;
		}
		case IL_META_ELEMTYPE_I8:
		{
			return &_ILJitType_I8;
		}
		case IL_META_ELEMTYPE_U8:
		{
			return &_ILJitType_U8;
		}
		case IL_META_ELEMTYPE_I:
		{
			return &_ILJitType_I;
		}
		case IL_META_ELEMTYPE_U:
		{
			return &_ILJitType_U;
		}
		case IL_META_ELEMTYPE_R4:
		{
			return &_ILJitType_R4;
		}
		case IL_META_ELEMTYPE_R8:
		{
			return &_ILJitType_R8;
		}
		case IL_META_ELEMTYPE_R:
		{
			return &_ILJitType_NFLOAT;
		}
		case IL_META_ELEMTYPE_TYPEDBYREF:
		{
			return &_ILJitType_TYPEDREF;
		}
		case IL_META_ELEMTYPE_PTR:
		case IL_META_ELEMTYPE_STRING:
		case IL_META_ELEMTYPE_BYREF:
		case IL_META_ELEMTYPE_ARRAY:
		case IL_META_ELEMTYPE_FNPTR:
		case IL_META_ELEMTYPE_OBJECT:
		case IL_META_ELEMTYPE_SZARRAY:
		{
			return &_ILJitType_VPTR;
		}
	}
	return 0;
}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)

/*
 * Dump method profile information.
 */
int _ILDumpMethodProfile(FILE *stream, ILExecProcess *process)
{
	ILJITCoder *coder = (ILJITCoder *)(process->coder);
	ILUInt32 count = 0;
	ILUInt32 current = 0;
	ILJitFunction function = 0;
	int haveCounts = 0;
	ILMethod *method;
	ILMethod **methods;

	/* Get the number of created and called functions */
	function = jit_function_next(coder->context, 0);
	while(function)
	{
		method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		if(method)
		{
			if(method->count > 0)
			{
				count++;
			}
		}
		function = jit_function_next(coder->context, function);
	}

	/* Allocate the array for the methods. */
	if(!(methods = (ILMethod **)ILMalloc((count + 1) * sizeof(ILMethod *))))
	{
		return 0;
	}

	/* Now fill the array. */
	function = jit_function_next(coder->context, 0);
	while(function)
	{
		method = (ILMethod *)jit_function_get_meta(function, IL_JIT_META_METHOD);
		if(method)
		{
			if((method->count > 0) && (current < count))
			{
				methods[current] = method;
				current++;
			}
		}
		function = jit_function_next(coder->context, function);
	}
	/* Mark the end of the list. */
	methods[current] = 0;

	/* Dump the profiling information */
	haveCounts = _ILProfilingDump(stream, methods);

	/* Clean up and exit */
	ILFree(methods);
	return haveCounts;
}

static void ILJitTraceIn(ILExecThread *thread, ILMethod *method)
{
	/* TODO: nesting level */
	ILMutexLock(globalTraceMutex);
	fputs("Entering ", stdout);
	ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), 0,
					 ILMethod_Owner(method),
					 ILMethod_Name(method), method);
	putc('\n',stdout);
	fflush(stdout);
	ILMutexUnlock(globalTraceMutex);
}

static void ILJitTraceOut(ILExecThread *thread, ILMethod *method)
{
	/* TODO: nesting level */
	ILMutexLock(globalTraceMutex);
	fputs("Leaving ", stdout);
	ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), 0,
					 ILMethod_Owner(method),
					 ILMethod_Name(method), method);
	putc('\n',stdout);
	fflush(stdout);
	ILMutexUnlock(globalTraceMutex);
}

#ifdef _IL_JIT_ENABLE_DEBUG

/*
 * Print ILMethod at given pc.
 * This function is used for printing stacktrace of jitted ILMethods in gdb.
 * Do not use otherwise!
 *
 * Usage:
 *
 * 1/ When gdb stops you can use: p ILJitPrintMethod (0x1234567)
 * where 0x1234567 is frame from bt command.
 *
 * 2/ add lines [1] to your ~/.gdbinit
 * to define new command for printing stacktrace.
 *
 * iljit_bt 5 in stopped gdb prints method names in first 5 frames.
 *
 * [1]:

 define iljit_bt
 select-frame 0
 set $i = 0
 while ($i < $arg0)
   set $foo = ILJitPrintMethod ($pc)
   printf "#%d %s\n", $i, $foo
   up-silently
   set $i = $i + 1
 end
end

 */
char *ILJitPrintMethod(void *pc)
{
	ILExecThread *thread;
	ILJITCoder *coder;
	jit_function_t fn;
	void *handler = 0;
	ILMethod *method;
	const char *methodName;
	const char *className;
	char *result;

	thread = ILExecThreadCurrent();
	if(thread == 0)
	{
		return "unable to get current thread";
	}
	coder = (ILJITCoder *)(thread->process->coder);
	fn = jit_function_from_pc(coder->context, pc, &handler);
	if(fn == 0)
	{
		return "function at given pc not found";
	}
	method = (ILMethod *)jit_function_get_meta(fn, IL_JIT_META_METHOD);
	methodName = ILMethod_Name(method);
	className = ILClass_Name(ILMethod_Owner(method));
	result = (char *) ILMalloc(strlen(methodName) + strlen(className) + 2);
	if(result)
	{
		sprintf(result, "%s.%s", className, methodName);
		return result;
	}
	else
	{
		return "out of memory";
	}
}

/*
 * This is a helper function for displaying backtraces in gdb.
 * gdb is using debug information for determining the current
 * frame address and return addresses.
 * Since libjit is not emitting debug information gdb doesn't
 * know what the frame looks like in jitted functions and shows
 * rubbish as soon as the first frame of a jitted function is
 * encounterd.
 * This is the reason for $fp (the current frame pointer in gdb)
 * being incorrect.
 * On some archs functions in foreign shared libraries might be
 * compiled so that their frame is different than the frame
 * emitted by libjit do its safest to do the following:
 * 1. show the gdb backtrace
 * 2. Select the first frame without function information with fr n
 *    where n is the number in front of that frame shown in gdb.
 * 3. enter call ILJitBacktrace($rbp, $pc, n)
 *    where n is the number of frames you want to see.
 *    $rbp is the frame pointer register on X86_64. On x86 replace
 *    $rbp with $ebp.
 *
 * This is for libjit in native mode. If you are using libjit in 
 * interpreter mode simply enter call ILJitBacktrace(0, 0, n)
 *
 * Sample gdb macro for x86
 *
   define iljit_bt
   call ILJitBacktrace($ebp, $pc, $arg0)
   end
 *
 * Sample gdb macro for x86_64
 *
   define iljit_bt
   call ILJitBacktrace($rbp, $pc, $arg0)
   end
 *
 * Sample gdb macro for libjit in interpreter mode
 *
   define iljit_bt
   call ILJitBacktrace(0, 0, $arg0)
   end
 */
void ILJitBacktrace(void *frame, void *pc, int numFrames)
{
	ILExecThread *thread;
	ILJITCoder *coder;
	int currentFrame = 0;
	jit_unwind_context_t unwindContext;
	void **returnAddresses;
#ifdef HAVE_BACKTRACE_SYMBOLS
	char **symbols;
#endif

	if(numFrames <= 0)
	{
		return;
	}
	thread = ILExecThreadCurrent();
	if(thread == 0)
	{
		return;
	}
	coder = (ILJITCoder *)(thread->process->coder);

	returnAddresses = (void **)alloca(sizeof(void *) * numFrames);

	/* Collect the return addresses */
	if(pc)
	{
		returnAddresses[0] = pc;
		++currentFrame;
	}
	if(jit_unwind_init(&unwindContext, coder->context))
	{
		/* I hate to do this but a backtrace is not possible without */
		/* setting the initial frame here */
		if(frame)
		{
			unwindContext.frame = frame;
		}
		while(currentFrame < numFrames)
		{
			returnAddresses[currentFrame] = jit_unwind_get_pc(&unwindContext);
			++currentFrame;
			if(!jit_unwind_next(&unwindContext))
			{
				break;
			}
		}
	}
	numFrames = currentFrame;
#ifdef HAVE_BACKTRACE_SYMBOLS
	/* Get the native symbols */
	symbols = backtrace_symbols(returnAddresses, numFrames);
	if(!symbols)
	{
		return;
	}
	/* Now set the method names for the jitted functions */
	for(currentFrame = 0; currentFrame < numFrames; ++currentFrame)
	{
		jit_function_t function;

		function = jit_function_from_pc(coder->context,
										returnAddresses[currentFrame], 0);
		if(function)
		{
			symbols[currentFrame] = _ILJitFunctionGetMethodName(function);
		}
	}
	/* Now print the information */
	for(currentFrame = 0; currentFrame < numFrames; ++currentFrame)
	{
		if(symbols[currentFrame])
		{
			printf("%i:\t%s\n", currentFrame, symbols[currentFrame]);
		}
		else
		{
			printf("%i:\t[%p]\n", currentFrame, returnAddresses[currentFrame]);
		}
	}
	ILFree(symbols);
#else
	/* Print the backtrace without having information about native functions */
	/* Now print the information */
	for(currentFrame = 0; currentFrame < numFrames; ++currentFrame)
	{
		jit_function_t function;

		function = jit_function_from_pc(coder->context,
										returnAddresses[currentFrame], 0);
		if(function)
		{
			char *methodName = _ILJitFunctionGetMethodName(function);
			if(methodName)
			{
				printf("%i:\t%s\n", currentFrame, methodName);
			}
			else
			{
				printf("%i:\t%s\n", currentFrame, "unnamed jitted function");
			}
		}
		else
		{
			printf("%i:\t[%p]\n", currentFrame, returnAddresses[currentFrame]);
		}
	}
#endif
}

#endif /* _IL_JIT_ENABLE_DEBUG */

#endif /* !IL_CONFIG_REDUCE_CODE && !IL_WITHOUT_TOOLS */

#define	IL_JITC_FUNCTIONS
#include "jitc_arith.c"
#include "jitc_diag.c"
#include "jitc_locals.c"
#include "jitc_stack.c"
#include "jitc_labels.c"
#include "jitc_inline.c"
#include "jitc_alloc.c"
#include "jitc_array.c"
#include "jitc_string.c"
#include "jitc_except.c"
#include "jitc_call.c"
#include "jitc_delegate.c"
#include "jitc_math.c"
#include "jitc_profile.c"
#undef	IL_JITC_FUNCTIONS

/*
 * Include the rest of the JIT conversion routines from other files.
 * We split the implementation to make it easier to maintain the code.
 */
#define	IL_JITC_CODE
#include "jitc_setup.c"
#include "jitc_const.c"
#include "jitc_arith.c"
#include "jitc_var.c"
#include "jitc_stack.c"
#include "jitc_ptr.c"
#include "jitc_array.c"
#include "jitc_branch.c"
#include "jitc_profile.c"
#include "jitc_except.c"
#include "jitc_conv.c"
#include "jitc_obj.c"
#include "jitc_call.c"
#undef	IL_JITC_CODE

/*
 * Define the JIT coder class.
 */
ILCoderClass const _ILJITCoderClass =
{
	JITCoder_Create,
	JITCoder_EnableDebug,
	JITCoder_Alloc,
	JITCoder_GetCacheSize,
	JITCoder_Setup,
	JITCoder_SetupExtern,
	JITCoder_SetupExternCtor,
	JITCoder_CtorOffset,
	JITCoder_Destroy,
	JITCoder_Finish,
	JITCoder_Label,
	JITCoder_StackRefresh,
	JITCoder_Constant,
	JITCoder_StringConstant,
	JITCoder_Binary,
	JITCoder_BinaryPtr,
	JITCoder_Shift,
	JITCoder_Unary,
	JITCoder_LoadArg,
	JITCoder_StoreArg,
	JITCoder_AddrOfArg,
	JITCoder_LoadLocal,
	JITCoder_StoreLocal,
	JITCoder_AddrOfLocal,
	JITCoder_Dup,
	JITCoder_Pop,
	JITCoder_ArrayAccess,
	JITCoder_PtrAccess,
	JITCoder_PtrDeref,
	JITCoder_PtrAccessManaged,
	JITCoder_Branch,
	JITCoder_SwitchStart,
	JITCoder_SwitchEntry,
	JITCoder_SwitchEnd,
	JITCoder_Compare,
	JITCoder_Conv,
	JITCoder_ToPointer,
	JITCoder_ArrayLength,
	JITCoder_NewArray,
	JITCoder_LocalAlloc,
	JITCoder_CastClass,
	JITCoder_LoadField,
	JITCoder_LoadStaticField,
	JITCoder_LoadThisField,
	JITCoder_LoadFieldAddr,
	JITCoder_LoadStaticFieldAddr,
	JITCoder_StoreField,
	JITCoder_StoreStaticField,
	JITCoder_CopyObject,
	JITCoder_CopyBlock,
	JITCoder_InitObject,
	JITCoder_InitBlock,
	JITCoder_Box,
	JITCoder_BoxPtr,
	JITCoder_BoxSmaller,
	JITCoder_Unbox,
	JITCoder_MakeTypedRef,
	JITCoder_RefAnyVal,
	JITCoder_RefAnyType,
	JITCoder_PushToken,
	JITCoder_SizeOf,
	JITCoder_ArgList,
	JITCoder_UpConvertArg,
	JITCoder_DownConvertArg,
	JITCoder_PackVarArgs,
	JITCoder_ValueCtorArgs,
	JITCoder_CheckCallNull,
	JITCoder_CallMethod,
	JITCoder_CallIndirect,
	JITCoder_CallCtor,
	JITCoder_CallVirtual,
	JITCoder_CallInterface,
	JITCoder_CallInlineable,
	JITCoder_JumpMethod,
	JITCoder_ReturnInsn,
	JITCoder_LoadFuncAddr,
	JITCoder_LoadVirtualAddr,
	JITCoder_LoadInterfaceAddr,
	JITCoder_Throw,
	JITCoder_SetStackTrace,
	JITCoder_Rethrow,
	JITCoder_CallFinally,
	JITCoder_RetFromFinally,
	JITCoder_LeaveCatch,
	JITCoder_RetFromFilter,
	JITCoder_OutputExceptionTable,
	JITCoder_PCToHandler,
	JITCoder_PCToMethod,
	JITCoder_GetILOffset,
	JITCoder_GetNativeOffset,
	JITCoder_MarkBytecode,
	JITCoder_MarkEnd,
	JITCoder_SetFlags,
	JITCoder_GetFlags,
	JITCoder_AllocExtraLocal,
	JITCoder_PushThread,
	JITCoder_LoadNativeArgAddr,
	JITCoder_LoadNativeLocalAddr,
	JITCoder_StartFfiArgs,
	JITCoder_PushRawArgPointer,
	JITCoder_CallFfi,
	JITCoder_CheckNull,
	JITCoder_Convert,
	JITCoder_ConvertCustom,
	JITCoder_RunCCtors,
	JITCoder_RunCCtor,
	JITCoder_HandleLockedMethod,
	JITCoder_ProfilingStart,
	JITCoder_ProfilingEnd,
	JITCoder_SetOptimizationLevel,
	JITCoder_GetOptimizationLevel,
	"sentinel"
};
#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_JIT */
