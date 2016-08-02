/*
 * jitc.h - Definitions for the JIT coder.
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

#ifndef	_ENGINE_JITC_H
#define	_ENGINE_JITC_H

#ifdef IL_USE_JIT

#include "jit/jit.h"
#include "lib_defs.h"

/*
 * Map the native IL types to JIT types.
 */
#define _IL_JIT_TYPE_VOID	jit_type_void
#define _IL_JIT_TYPE_BYTE	jit_type_ubyte
#define _IL_JIT_TYPE_CHAR	jit_type_ushort
#define _IL_JIT_TYPE_DOUBLE	jit_type_float64
#define _IL_JIT_TYPE_INT16	jit_type_short
#define _IL_JIT_TYPE_INT32	jit_type_int
#define _IL_JIT_TYPE_INT64	jit_type_long
#define _IL_JIT_TYPE_NINT	jit_type_nint
#define _IL_JIT_TYPE_INTPTR	jit_type_void_ptr
#define _IL_JIT_TYPE_NFLOAT	jit_type_nfloat
#define _IL_JIT_TYPE_SBYTE	jit_type_sbyte
#define _IL_JIT_TYPE_SINGLE	jit_type_float32
#define _IL_JIT_TYPE_UINT16	jit_type_ushort
#define _IL_JIT_TYPE_UINT32	jit_type_uint
#define _IL_JIT_TYPE_UINT64	jit_type_ulong
#define _IL_JIT_TYPE_NUINT	jit_type_nuint
#define _IL_JIT_TYPE_VPTR	jit_type_void_ptr
 
/*
 * Definition of method metadata indexes.
 */
#define IL_JIT_META_METHOD 1

/*
 * Definitions of method metadata only set when the coder is built with
 * extended debugging.
 */
#define IL_JIT_META_METHODNAME 1001

/*
 * Use the ILMethod * as function pointer.
 */
#define IL_JIT_FNPTR_ILMETHOD 1

/*
 * Include the current thread in the method's signature.
 */
#define IL_JIT_THREAD_IN_SIGNATURE 1

/*
 * Calling conventions for libjit.
 */
#define IL_JIT_CALLCONV_CDECL		jit_abi_cdecl
#define IL_JIT_CALLCONV_VARARG		jit_abi_vararg
#define IL_JIT_CALLCONV_STDCALL		jit_abi_stdcall
#define IL_JIT_CALLCONV_FASTCALL	jit_abi_fastcall

/*
 * Defaut calling convention for libjit.
 */
#define IL_JIT_CALLCONV_DEFAULT		IL_JIT_CALLCONV_CDECL

/*
 * Values in data1 in debug hook.
 */
#define	JIT_DEBUGGER_DATA1_METHOD_ENTER			0
#define	JIT_DEBUGGER_DATA1_METHOD_LEAVE			1
#define	JIT_DEBUGGER_DATA1_METHOD_OFFSET		2
#define	JIT_DEBUGGER_DATA1_THIS_ADDR			3
#define	JIT_DEBUGGER_DATA1_PARAM_ADDR			4
#define	JIT_DEBUGGER_DATA1_LOCAL_VAR_ADDR		5

/*
 * Representation of a type representation for libjit.
 */
typedef jit_type_t		ILJitType;

/*
 * Definition of a method representation for libjit.
 */
typedef jit_function_t	ILJitFunction;

/*
 * Representation of a jit value.
 */
typedef jit_value_t		ILJitValue;

/*
 * Structure to hold the common types needed for jitc for a clr type.
 */
typedef struct _tagILJitTypes ILJitTypes;
struct _tagILJitTypes
{
	ILJitType	jitTypeBase;	/* the base type (class or struct) */
	ILUInt32	jitTypeKind;	/* special class. */
};

/*
 * Definition of the class which need special handling by the jit coder.
 */
#define IL_JIT_TYPEKIND_DELEGATE			0x00000001
#define IL_JIT_TYPEKIND_MULTICASTDELEGATE	0x00000002
#define IL_JIT_TYPEKIND_ARRAY				0x00000010
#define IL_JIT_TYPEKIND_SYSTEM_ARRAY		0x00000020
#define IL_JIT_TYPEKIND_SYSTEM_MATH			0x00000040
#define IL_JIT_TYPEKIND_SYSTEM_STRING		0x00000080

/*
 * Initialize a ILJitTypes structure 
 */
#define _ILJitTypesInit(jitTypes) \
	{ \
		(jitTypes)->jitTypeBase = 0; \
		(jitTypes)->jitTypeKind = 0; \
	}

/*
 * Forward declaration of the JIT coder's instance block.
 */
typedef struct _tagILJITCoder ILJITCoder;

typedef struct _tagILJitStackItem ILJitStackItem;

/*
 * Prototype for inlining functioncalls.
 *
 * On entry of the function the args are allready popped off the evaluation
 * stack. The args pointer points to the first arg (the one at the lowest
 * stack position).
 * The function is responsible to push the result value on the stack if the
 * return type is not void.
 *
 * The function has to return 0 on failure. Any other value will be treated as
 * success.
 *
 * int func(ILJITCoder *, ILMethod *, ILCoderMethodInfo *, ILJitStackItem *, ILInt32)
 */
typedef int (*ILJitInlineFunc)(ILJITCoder *jitCoder,
									  ILMethod *method,
									  ILCoderMethodInfo *methodInfo,
									  ILJitStackItem *args,
									  ILInt32 numArgs);

/*
 * Private method information for the jit coder.
 */
typedef struct _tagILJitMethodInfo ILJitMethodInfo;
struct _tagILJitMethodInfo
{
	ILJitFunction jitFunction;		/* Implementation of the method. */
	ILUInt32 implementationType;	/* Flag how the method is implemented. */
	ILInternalInfo fnInfo;			/* Information for internal calls or pinvokes. */
	ILJitInlineFunc inlineFunc;		/* Function for inlining. */
};

/*
 * Initialize the libjit coder.
 * Returns 1 on success and 0 on failure.
 */
int ILJitInit();

/*
 * Create the jit function header for an ILMethod.
 * We allways pass the ILExecThread as arg 0.
 * Returns 1 on success and 0 on error.
 */
int ILJitFunctionCreate(ILCoder *_coder, ILMethod *method);

/*
 * Get jit context.
 */
jit_context_t ILJitGetContext(ILCoder *_coder);

/*
 * Create the jit function header for an ILMethod with the information from
 * a virtual ancestor.
 * We can reuse the signature in this case.
 */
int ILJitFunctionCreateFromAncestor(ILCoder *_coder, ILMethod *method,
													 ILMethod *virtualAncestor);

/*
 * Create all jitMethods for the given class.
 * Returns 1 on success and 0 on error.
 */
int ILJitCreateFunctionsForClass(ILCoder *_coder, ILClass *info);

/*
 * Get a pointer for a method suitable for a vtable.
 * Returns 0 on error.
 */
void *ILJitGetVtablePointer(ILCoder *_coder, ILMethod *method);

/*
 * Get the ILJitFunction for an ILMethod.
 * Returns 0 if the jit function stub isn't created yet.
 */
ILJitFunction ILJitFunctionFromILMethod(ILMethod *method);

/*
 * Call the jit function for an ILMethod.
 * Returns 1 if an exception occured.
 */
int ILJitCallMethod(ILExecThread *thread, ILMethod *method,
					void**jitArgs, void *result);

/*
 * Get the ILMethod for the call frame up n slots.
 * Returns 0 if the function at that slot is not a jitted function.
 */
ILMethod *_ILJitGetCallingMethod(ILExecThread *thread, ILUInt32 frames);

/*
 * Get the number of stack frames associated with an ILMethod on the current
 * call stack.
 */
ILInt32 _ILJitDiagNumFrames(ILExecThread *thread);

/*
 * Get the current PackedStackFrame.
 */
System_Array *_ILJitGetExceptionStackTrace(ILExecThread *thread);

/*
 * Create the class/struct representation of a clr type for libjit.
 * and store the type in classPrivate.
 * Returns the jit type on success else 0
 */
int ILJitTypeCreate(ILClassPrivate *classPrivate, ILExecProcess *process);

/*
 * Destroy the given type in libjit.
 */
void ILJitTypeDestroy(ILJitType type);

/*
 * Destroy every ILJitType in a ILJitTypes  structure 
 */
void ILJitTypesDestroy(ILJitTypes *jitTypes);

/*
 * Get the jit type for a primitive clr type.
 * Returns 0 if primitiveClrType is not a primitive clr type.
 */
ILJitTypes *ILJitPrimitiveClrTypeToJitTypes(int primitiveClrType);

/*
 * Get the alignment for a jit type.
 */
#define ILJitTypeGetAlignment(jitType) jit_type_get_alignment((jitType))

/*
 * Get the zize of a jit type in bytes.
 */
#define ILJitTypeGetSize(jitType)      jit_type_get_size((jitType))

/*
 * The exception handler which converts libjit inbuilt exceptions
 * into clr exceptions.
 */
void *_ILJitExceptionHandler(int exception_type);


/*
 * Get the closure for a delegate.
 */
void *ILJitDelegateGetClosure(ILObject *delegate,
				ILType *delType);


#endif  /* IL_USE_JIT */

#endif	/* _ENGINE_JITC_H */
