/*
 * cvm.c - Implementation of the "Converted Virtual Machine".
 *
 * Copyright (C) 2001, 2011  Southern Storm Software, Pty Ltd.
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
#include "lib_defs.h"
#include "cvm.h"
#include "cvm_config.h"
#include "cvm_format.h"
#if defined(HAVE_LIBFFI)
#include "ffi.h"
#endif
#ifdef IL_DEBUGGER
#include "debugger.h"
#endif

#ifdef IL_USE_CVM

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Note: the configuration macros that used to be here
 * are now in "cvm_config.h".
 */

/*
 * Return values for ILCVMInterpreter.
 */

/*
 * Return value for normal exit
 */
#define _CVM_EXIT_OK						0
/*
 * Return value for exit with an exception pending
 */
#define _CVM_EXIT_EXCEPT					1

/*
 * Additional return values for the interpreter function.
 */
/*
 * Return value if an exception is thrown.
 * Exception handling has to be performed.
 */
#define _CVM_EXIT_THROW						2
/*
 * Return value for propagating a ThreadAbortException.
 */
#define _CVM_EXIT_PROPAGATE_ABORT			3
/*
 * Return value for normal returning from a finally, fault or filter block.
 */
#define _CVM_EXIT_RETURN					4
/*
 * Return value from unwind to execute a handler by the caller.
 */
#define _CVM_EXIT_EXECUTE_HANDLER			5
/*
 * Return value for continuing to unwind in the caller.
 * This is for keeping the call nesting level as small as possible.
 */
#define _CVM_EXIT_CONTINUE_UNWIND			6

/*
 * Return values from a filter call as specified in ECMA
 */
#define _CVM_EXCEPTION_CONTINUE_SEARCH		0
#define _CVM_EXCEPTION_EXECUTE_HANDLER		1

#if !defined(IL_NO_REGISTERS_USED)
#if defined(CVM_X86) && defined(__GNUC__) && !defined(IL_NO_ASM)
	/*
	 * Memory copies on x86 use esi and edi, but we want them
	 * for something else.  So we force the copy to go through
	 * a function to prevent register spills in gcc.  Similarly
	 * for memory set operations.
	 */
	#define	IL_MEMCPY(dst,src,len)			LocalMemCpy((dst), (src), (len))
	#define	IL_MEMMOVE(dst,src,len)			LocalMemMove((dst), (src), (len))
	#define	IL_MEMZERO(dst,len)				LocalMemZero((dst), (len))
	#define	IL_MEMSET(dst,ch,len)			LocalMemSet((dst), (ch), (len))
	#define	IL_MEMCMP(dst,src,len)			LocalMemCmp((dst), (src), (len))
	static void LocalMemCpy(void *dst, const void *src, unsigned len)
	{
		ILMemCpy(dst, src, len);
	}
	static void LocalMemMove(void *dst, const void *src, unsigned len)
	{
		ILMemMove(dst, src, len);
	}
	static void LocalMemZero(void *dst, unsigned len)
	{
		ILMemZero(dst, len);
	}
	static void LocalMemSet(void *dst, int ch, unsigned len)
	{
		ILMemSet(dst, ch, len);
	}
	static int LocalMemCmp(void *dst, const void *src, unsigned len)
	{
		return ILMemCmp(dst, src, len);
	}
#endif
#endif

/*
 * Define the memory macros if not defined.
 */
#ifndef IL_MEMCPY
#define	IL_MEMCPY(dst,src,len)			(ILMemCpy((dst), (src), (len)))
#endif
#ifndef IL_MEMMOVE
#define	IL_MEMMOVE(dst,src,len)			(ILMemMove((dst), (src), (len)))
#endif
#ifndef IL_MEMZERO
#define	IL_MEMZERO(dst,len)				(ILMemZero((dst), (len)))
#endif
#ifndef IL_MEMSET
#define	IL_MEMSET(dst,ch,len)			(ILMemSet((dst), (ch), (len)))
#endif
#ifndef IL_MEMCMP
#define	IL_MEMCMP(dst,src,len)			(ILMemCmp((dst), (src), (len)))
#endif

/*
 * Defining macros to declare variables in the interpreter loop volatile.
 * This is done to fix problems with gcc's optimized code.
 */
#if defined(__GNUC__) && !defined(IL_VMCASE_BARRIER)
#if (__GNUC__ == 4) && defined(CVM_X86)
#define IL_PC_VOLATILE
#define IL_STACKTOP_VOLATILE
#define IL_FRAME_VOLATILE
#define IL_STACKMAX_VOLATILE
#define IL_METHOD_VOLATILE
#define IL_METHODTOCALL_VOLATILE
#define IL_CALLFRAME_VOLATILE
#define IL_TEMPPTR_VOLATILE
#endif
#if (__GNUC__ == 4) && defined(CVM_X86_64)
#define IL_PC_VOLATILE
#define IL_STACKTOP_VOLATILE
#define IL_FRAME_VOLATILE
#define IL_STACKMAX_VOLATILE
#define IL_METHOD_VOLATILE
#define IL_METHODTOCALL_VOLATILE
#define IL_CALLFRAME_VOLATILE
#define IL_TEMPPTR_VOLATILE
#endif
#if (__GNUC__ == 4) && defined(CVM_ARM)
#define IL_PC_VOLATILE
#define IL_STACKTOP_VOLATILE
#define IL_FRAME_VOLATILE
#define IL_STACKMAX_VOLATILE
#define IL_METHOD_VOLATILE
#define IL_METHODTOCALL_VOLATILE
#define IL_CALLFRAME_VOLATILE
#define IL_TEMPPTR_VOLATILE
#endif
#if (__GNUC__ == 4) && defined(CVM_PPC)
#define IL_PC_VOLATILE
#define IL_STACKTOP_VOLATILE
#define IL_FRAME_VOLATILE
#define IL_STACKMAX_VOLATILE
#define IL_METHOD_VOLATILE
#define IL_METHODTOCALL_VOLATILE
#define IL_CALLFRAME_VOLATILE
#define IL_TEMPPTR_VOLATILE
#endif
#endif
#ifndef IL_PC_VOLATILE
#define IL_PC_VOLATILE
#endif
#ifndef IL_STACKTOP_VOLATILE
#define IL_STACKTOP_VOLATILE
#endif
#ifndef IL_FRAME_VOLATILE
#define IL_FRAME_VOLATILE
#endif
#ifndef IL_STACKMAX_VOLATILE
#define IL_STACKMAX_VOLATILE
#endif
#ifndef IL_METHOD_VOLATILE
#define IL_METHOD_VOLATILE
#endif
#ifndef IL_METHODTOCALL_VOLATILE
#define IL_METHODTOCALL_VOLATILE
#endif
#ifndef IL_CALLFRAME_VOLATILE
#define IL_CALLFRAME_VOLATILE
#endif
#ifndef IL_TEMPPTR_VOLATILE
#define IL_TEMPPTR_VOLATILE
#endif

#if defined(IL_USE_INTERRUPT_BASED_X)
	#if defined(IL_INTERRUPT_HAVE_X86_CONTEXT) && defined(REGISTER_ASM_X86)
		/* If the interrupt subsystem can provide us the x86 registers at the
		   time of interrupt then we don't need to save anything */
		#define INTERRUPT_BACKUP_FRAME()
		#define INTERRUPT_BACKUP_PC_STACKTOP_FRAME()

		/* We can restore locals directly from the register state
		   at the time of interrupt */
		#define INTERRUPT_RESTORE_FROM_THREAD() \
			do \
			{ \
				pc = (unsigned char *)thread->interruptContext.Esi; \
				stacktop = (CVMWord *)thread->interruptContext.Edi; \
				frame = (CVMWord *)thread->interruptContext.Ebx; \
			} \
			while (0);
	#elif defined(IL_INTERRUPT_HAVE_X86_64_CONTEXT) && defined(REGISTER_ASM_X86_64)
		/*
		 * If the interrupt subsystem can provide us the x86_64 registers
		 * at the time of interrupt then we don't need to save anything
		 */
		#define INTERRUPT_BACKUP_FRAME()
		#define INTERRUPT_BACKUP_PC_STACKTOP_FRAME()

		/* We can restore locals directly from the register state
		   at the time of interrupt */
		#define INTERRUPT_RESTORE_FROM_THREAD() \
			do \
			{ \
				pc = (unsigned char *)thread->interruptContext.R12; \
				stacktop = (CVMWord *)thread->interruptContext.R14; \
				frame = (CVMWord *)thread->interruptContext.R15; \
			} \
			while (0);
	#elif defined(IL_INTERRUPT_HAVE_ARM_CONTEXT) && defined(REGISTER_ASM_ARM)
		/*
		 * If the interrupt subsystem can provide us the arm registers
		 * at the time of interrupt then we don't need to save anything
		 */
		#define INTERRUPT_BACKUP_FRAME()
		#define INTERRUPT_BACKUP_PC_STACKTOP_FRAME()

		/* We can restore locals directly from the register state
		   at the time of interrupt */
		#define INTERRUPT_RESTORE_FROM_THREAD() \
			do \
			{ \
				pc = (unsigned char *)thread->interruptContext.R4; \
				stacktop = (CVMWord *)thread->interruptContext.R5; \
				frame = (CVMWord *)thread->interruptContext.R6; \
			} \
			while (0);
	#else
		#define INTERRUPT_RESTORE_FROM_THREAD()
	#endif
#endif

#if defined(IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS)
	#define BEGIN_NULL_CHECK(x)
	#define BEGIN_NULL_CHECK_STMT(x) x;
	#define END_NULL_CHECK()
#else
	#define BEGIN_NULL_CHECK(x) \
		if (IL_EXPECT((x) == 0, 0)) goto throwNullReferenceException; \
		{

	#define BEGIN_NULL_CHECK_STMT(x) BEGIN_NULL_CHECK(x)

	#define END_NULL_CHECK() \
		}
#endif

#if defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	#define BEGIN_INT_ZERO_DIV_CHECK(x)
	#define END_INT_ZERO_DIV_CHECK()
#else
	#define BEGIN_INT_ZERO_DIV_CHECK(x) \
		if (IL_EXPECT((x) != 0, 1)) \
		{

	#define END_INT_ZERO_DIV_CHECK() \
		} \
		else \
		{ \
			ARITHMETIC_EXCEPTION(); \
		}
#endif

/*
 * int overflow checks
 */
#if defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	#define BEGIN_INT_OVERFLOW_CHECK(x)
	#define END_INT_OVERFLOW_CHECK()
#else
	#define BEGIN_INT_OVERFLOW_CHECK(x) \
		if (IL_EXPECT((x), 1)) \
		{

	#define END_INT_OVERFLOW_CHECK() \
		} \
		else \
		{ \
			ARITHMETIC_EXCEPTION(); \
		}
#endif

/*
 * Modify the program counter and stack pointer.
 */
#define	MODIFY_PC_AND_STACK(pcmod,stkmod)	\
			do { \
				pc += (pcmod); \
				stacktop += (stkmod); \
			} while (0)

/*
 * Modify the program counter and stack pointer in reverse order
 * because the "stkmod" value is extracted from the program.
 */
#define	MODIFY_PC_AND_STACK_REVERSE(pcmod,stkmod)	\
			do { \
				stacktop += (stkmod); \
				pc += (pcmod); \
			} while (0)

/*
 * Throw an overflow exception.
 */
#define	OVERFLOW_EXCEPTION()	\
			do { \
				goto throwOverflowException; \
			} while (0)

/*
 * Throw an arithmetic exception.
 */
#define	ARITHMETIC_EXCEPTION()	\
			do { \
				goto throwArithmeticException; \
			} while (0)

/*
 * Throw a division by zero exception.
 */
#define	ZERO_DIV_EXCEPTION()	\
			do { \
				goto throwDivideByZeroException; \
			} while (0)

/*
 * Throw a null pointer exception.
 */
#define	NULL_POINTER_EXCEPTION()	\
			do { \
				goto throwNullReferenceException; \
			} while (0)

/*
 * Throw a stack overflow exception.
 */
#define	STACK_OVERFLOW_EXCEPTION()	\
			do { \
				goto throwStackOverflowException; \
			} while (0)

/*
 * Throw a missing method exception.
 */
#define	MISSING_METHOD_EXCEPTION()	\
			do { \
				goto throwMissingMethodException; \
			} while (0)

/*
 * Process an exception that was thrown by "_ILConvertMethod".
 * The state is already copied into the thread, with the
 * exception ready to be detected by "RESTORE_STATE_FROM_THREAD".
 */
#define	CONVERT_FAILED_EXCEPTION()	\
			do { \
				RESTORE_STATE_FROM_THREAD(); \
			} while (0)

/*
 * Throw an invalid cast exception.
 */
#define	INVALID_CAST_EXCEPTION()	\
			do { \
				goto throwInvalidCastException; \
			} while (0)

/*
 * Throw an array index out of range exception.
 */
#define	ARRAY_INDEX_EXCEPTION()	\
			do { \
				goto throwIndexOutOfRangeException; \
			} while (0)

/*
 * Read a long value from a stack position.
 */
static IL_INLINE ILInt64 ReadLong(CVMWord *stack)
{
#ifdef CVM_LONGS_ALIGNED_WORD
	return *((ILInt64 *)stack);
#else
	ILInt64 temp;
	IL_MEMCPY(&temp, stack, sizeof(ILInt64));
	return temp;
#endif
}

/*
 * Write a long value to a stack position.
 */
static IL_INLINE void WriteLong(CVMWord *stack, ILInt64 value)
{
#ifdef CVM_LONGS_ALIGNED_WORD
	*((ILInt64 *)stack) = value;
#else
	IL_MEMCPY(stack, &value, sizeof(ILInt64));
#endif
}

/*
 * Read an unsigned long value from a stack position.
 */
static IL_INLINE ILUInt64 ReadULong(CVMWord *stack)
{
#ifdef CVM_LONGS_ALIGNED_WORD
	return *((ILUInt64 *)stack);
#else
	ILUInt64 temp;
	IL_MEMCPY(&temp, stack, sizeof(ILUInt64));
	return temp;
#endif
}

/*
 * Write an unsigned long value to a stack position.
 */
static IL_INLINE void WriteULong(CVMWord *stack, ILUInt64 value)
{
#ifdef CVM_LONGS_ALIGNED_WORD
	*((ILUInt64 *)stack) = value;
#else
	IL_MEMCPY(stack, &value, sizeof(ILUInt64));
#endif
}

#ifdef IL_CONFIG_FP_SUPPORTED

/*
 * Read a native float value from a stack position.
 */
static IL_INLINE ILNativeFloat ReadFloat(CVMWord *stack)
{
#ifdef CVM_REALS_ALIGNED_WORD
	return *((ILNativeFloat *)stack);
#else
	ILNativeFloat temp;
	IL_MEMCPY(&temp, stack, sizeof(ILNativeFloat));
	return temp;
#endif
}

/*
 * Write a native float value to a stack position.
 */
static IL_INLINE void WriteFloat(CVMWord *stack, ILNativeFloat value)
{
#ifdef CVM_REALS_ALIGNED_WORD
	*((ILNativeFloat *)stack) = value;
#else
	IL_MEMCPY(stack, &value, sizeof(ILNativeFloat));
#endif
}

/*
 * Read a double value from a stack position.
 */
static IL_INLINE ILDouble ReadDouble(CVMWord *stack)
{
#ifdef CVM_DOUBLES_ALIGNED_WORD
	return *((ILDouble *)stack);
#else
	ILDouble temp;
	IL_MEMCPY(&temp, stack, sizeof(ILDouble));
	return temp;
#endif
}

/*
 * Write a double value to a stack position.
 */
static IL_INLINE void WriteDouble(CVMWord *stack, ILDouble value)
{
#ifdef CVM_DOUBLES_ALIGNED_WORD
	*((ILDouble *)stack) = value;
#else
	IL_MEMCPY(stack, &value, sizeof(ILDouble));
#endif
}

#endif /* !IL_CONFIG_FP_SUPPORTED */

/*
 * Read a pointer value from a program position.
 */
static IL_INLINE void *ReadPointer(unsigned char *pc)
{
#ifdef CVM_X86
	/* The x86 can read values from non-aligned addresses */
	return *((void **)pc);
#else
	/* We need to be careful about alignment on other platforms */
#if SIZEOF_VOID_P == 4
	return (void *)(IL_READ_UINT32(pc));
#else
	return (void *)(IL_READ_UINT64(pc));
#endif
#endif
}

/*
 * Copy the temporary state into the thread object.
 */
#define	COPY_STATE_TO_THREAD()	\
			do { \
				thread->pc = pc; \
				thread->frame = frame; \
				thread->stackTop = stacktop; \
				thread->method = method; \
			} while (0)

/* Define global variables that are used by the instruction categories */
#define IL_CVM_GLOBALS
#include "cvm_var.c"
#include "cvm_ptr.c"
#include "cvm_stack.c"
#include "cvm_arith.c"
#include "cvm_conv.c"
#include "cvm_const.c"
#include "cvm_branch.c"
#include "cvm_call.c"
#include "cvm_except.c"
#include "cvm_compare.c"
#include "cvm_inline.c"
#undef IL_CVM_GLOBALS

/*
 * Define instruction label tables, if necessary.
 */
CVM_DEFINE_TABLES();

static int Interpreter(ILExecThread *thread)
{
	CVM_REGISTER_ASM_PC(unsigned char *IL_PC_VOLATILE pc);
	CVM_REGISTER_ASM_STACK(CVMWord *IL_STACKTOP_VOLATILE stacktop);
	CVM_REGISTER_ASM_FRAME(CVMWord *IL_FRAME_VOLATILE frame);
	int divResult;
	CVMWord  *IL_STACKMAX_VOLATILE stackmax;
	ILMethod *IL_METHOD_VOLATILE method;
	void *nativeArgs[CVM_MAX_NATIVE_ARGS + 1];

	/* Define local variables that are used by the instruction categories */
	#define IL_CVM_LOCALS
	#include "cvm_var.c"
	#include "cvm_ptr.c"
	#include "cvm_stack.c"
	#include "cvm_arith.c"
	#include "cvm_conv.c"
	#include "cvm_const.c"
	#include "cvm_branch.c"
	#include "cvm_call.c"
	#include "cvm_except.c"
	#include "cvm_compare.c"
	#include "cvm_inline.c"
	#include "cvm_interrupt.c"
	#undef IL_CVM_LOCALS

	/* Include helper definitions and macros for the switch loop
	   that handle computed goto labels if necessary */
	#include "cvm_labels.h"

	/* Export the goto label tables from the interpreter if necessary */
	if(!thread)
	{
		CVM_EXPORT_TABLES();
		return 0;
	}

	/* Cache the engine state in local variables */
	pc = thread->pc;
	stacktop = thread->stackTop;
	frame = thread->frame;
	stackmax = thread->stackLimit;
	method = thread->method;
	thread->runningManagedCode = 1;

	#define IL_CVM_PRELUDE
	#include "cvm_interrupt.c"
	#undef IL_CVM_PRELUDE

	/* Enter the main instruction loop */
	for(;;)
	{
		CVM_DUMP();
		VMSWITCH(pc[0])
		{
			/**
			 * <opcode name="nop" group="Miscellaneous instructions">
			 *   <operation>Do nothing</operation>
			 *
			 *   <format>nop</format>
			 *   <dformat>{nop}</dformat>
			 *
			 *   <form name="nop" code="COP_NOP"/>
			 *
			 *   <description>Do nothing.</description>
			 * </opcode>
			 */
			VMCASE(COP_NOP):
			{
				/* The world's simplest instruction */
				MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
			}
			VMBREAK(COP_NOP);

			/* Include the instruction categories for the main switch */
			#define IL_CVM_MAIN
			#include "cvm_var.c"
			#include "cvm_ptr.c"
			#include "cvm_stack.c"
			#include "cvm_arith.c"
			#include "cvm_conv.c"
			#include "cvm_const.c"
			#include "cvm_branch.c"
			#include "cvm_call.c"
			#include "cvm_except.c"
			#include "cvm_compare.c"
			#include "cvm_inline.c"
			#undef IL_CVM_MAIN

			/**
			 * <opcode name="break" group="Miscellaneous instructions">
			 *   <operation>Mark the position of a breakpoint</operation>
			 *
			 *   <format>break<fsep/>subcode</format>
			 *
			 *   <dformat>{break}<fsep/>subcode</dformat>
			 *
			 *   <form name="break" code="COP_BREAK"/>
			 *
			 *   <description>This instruction marks a position in the
			 *   CVM bytecode that may be used as a breakpoint in
			 *   debug versions of the runtime engine.<p/>
			 *
			 *   Every potentional position for a breakpoint is marked,
			 *   even if those positions will never have active breakpoints
			 *   set on them.  The runtime engine keeps a list of active
			 *   breakpoints, which is inspected at each potentional
			 *   breakpoint.</description>
			 * </opcode>
			 */
			VMCASE(COP_BREAK):
			{
			#ifdef IL_CONFIG_DEBUG_LINES
				/* Check the breakpoint against the watch list */
				if(_ILIsBreak(thread, method)
#ifdef IL_DEBUGGER
				 && !ILDebuggerIsThreadUnbreakable(thread)
#endif
				)
				{
					COPY_STATE_TO_THREAD();
					_ILBreak(thread, (int)CVM_ARG_BREAK_SUBCODE);
					RESTORE_STATE_FROM_THREAD();
				}
			#endif
				MODIFY_PC_AND_STACK(CVM_LEN_BREAK, 0);
			}
			VMBREAK(COP_BREAK);

			/**
			 * <opcode name="wide" group="Miscellaneous instructions">
			 *   <operation>Modify an instruction to its wide form</operation>
			 *
			 *   <format>wide<fsep/>opcode<fsep/>...</format>
			 *
			 *   <form name="wide" code="COP_WIDE"/>
			 *
			 *   <description>The <i>wide</i> instruction modifies another
			 *   instruction to take longer operands.  The format of the
			 *   operands depends upon the <i>opcode</i>.</description>
			 *
			 *   <notes>The documentation for other instructions includes
			 *   information on their wide forms where appropriate.<p/>
			 *
			 *   There is no direct format for this instruction,
			 *   because <i>wide</i> is not required for the direct
			 *   encoding.</notes>
			 * </opcode>
			 */
#ifndef IL_CVM_DIRECT
			VMCASE(COP_WIDE):
			{
				CVM_WIDE_DUMP();
				switch(CVM_ARG_SUB_OPCODE)
				{
					/* Include the instruction categories for the wide switch */
					#define IL_CVM_WIDE
					#include "cvm_var.c"
					#include "cvm_ptr.c"
					#include "cvm_stack.c"
					#include "cvm_arith.c"
					#include "cvm_conv.c"
					#include "cvm_const.c"
					#include "cvm_branch.c"
					#include "cvm_call.c"
					#include "cvm_except.c"
					#include "cvm_compare.c"
					#include "cvm_inline.c"
					#undef IL_CVM_WIDE

					default:
					{
						/* Treat all other wide opcodes as NOP */
						MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
					}
					break;
				}
			}
			VMBREAK(COP_WIDE);
#else
			VMCASE(COP_WIDE):
			{
				/* We don't need "wide" in direct mode, so just stub it out */
				MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
			}
			VMBREAK(COP_WIDE);
#endif

			/**
			 * <opcode name="prefix" group="Miscellaneous instructions">
			 *   <operation>Prefix an alternative instruction</operation>
			 *
			 *   <format>prefix<fsep/>opcode<fsep/>...</format>
			 *
			 *   <form name="prefix" code="COP_PREFIX"/>
			 *
			 *   <description>The <i>prefix</i> instruction is used to
			 *   switch the runtime engine into an alternative instruction
			 *   set.  The alternative instruction is <i>opcode</i>.
			 *   Prefixing is necessary because the VM has more than
			 *   256 distinct instructions.</description>
			 *
			 *   <notes>There is no direct format for this instruction,
			 *   because <i>prefix</i> is not required for the direct
			 *   encoding.</notes>
			 * </opcode>
			 */
			VMCASE(COP_PREFIX):
			{
				/* Execute a prefixed opcode */
#ifndef IL_CVM_DIRECT
				CVM_PREFIX_DUMP();
				VMPREFIXSWITCH(CVM_ARG_SUB_OPCODE)
				{
#else
				/* We don't need "prefix" in direct mode, so just stub it out */
				switch(1)
				{
				case 0:
					VMPREFIXDEFAULT:
					{
						MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
					}
					break;
				}
			}
			VMBREAK(COP_PREFIX);
#endif
					/* Include instruction categories for the prefix switch */
					#define IL_CVM_PREFIX
					#include "cvm_var.c"
					#include "cvm_ptr.c"
					#include "cvm_stack.c"
					#include "cvm_arith.c"
					#include "cvm_conv.c"
					#include "cvm_const.c"
					#include "cvm_branch.c"
					#include "cvm_call.c"
					#include "cvm_except.c"
					#include "cvm_compare.c"
					#include "cvm_inline.c"
					#undef IL_CVM_PREFIX

					/**
					 * <opcode name="unroll_method"
					 *         group="Miscellaneous instructions">
					 *   <operation>Mark a method for unrolling</operation>
					 *
					 *   <dformat>{unroll_method}</dformat>
					 *
					 *   <form name="unroll_method"
					 *         code="COP_PREFIX_UNROLL_METHOD"/>
					 *
					 *   <description>The <i>unroll_method</i> instruction is
					 *   used in direct code to trigger native code unrolling.
					 *   <p/>
					 *
					 *   Unrolling converts fragments of the method into
					 *   native code for the underlying CPU, to speed up
					 *   execution.</description>
					 *
					 *   <notes>There is no bytecode format for this
					 *   instruction, because unrolling is not possible
					 *   with the bytecode encoding.<p/>
					 *
					 *   In a method that can support unrolling, the
					 *   <i>nop</i> instruction is used to mark
					 *   a label, so that the unroller can process labels
					 *   in a single translation pass.  The <i>prefix</i>
					 *   instruction is used to mark the end of the method's
					 *   code, so that the unroller knows where to stop.
					 *   </notes>
					 * </opcode>
					 */
					VMCASE(COP_PREFIX_UNROLL_METHOD):
					{
						/* Unroll the current method to native code */
					#ifdef IL_CVM_DIRECT_UNROLLED
						if(_ILUnrollMethod(thread, thread->process->coder,
										   pc, method))
						{
							VMSWITCH(0);
						}
						else
					#endif
						{
							MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
						}
					}
					VMBREAK(COP_PREFIX_UNROLL_METHOD);

					/**
					 * <opcode name="unroll_stack"
					 *         group="Miscellaneous instructions">
					 *
					 *   <operation>Start special stack variable
					 *   initialization for unroller</operation>
					 *
					 *   <dformat>{unroll_stack}</dformat>
					 *
					 *   <form name="unroll_stack"
					 *         code="COP_PREFIX_UNROLL_STACK"/>
					 *
					 *   <description>The <i>unroll_stack</i> instruction
					 *   is used only to bootstrap the unroller.<p/>
					 *
					 *   It generates the code that gets the native stack
					 *   pointer value and puts it on the interpreter
					 *   stack. This istruction must be followed by
					 *   <i>unroll_stack_return</i>.</description>
					 * </opcode>
					 */
					VMCASE(COP_PREFIX_UNROLL_STACK):
					{
					#if defined(IL_CVM_DIRECT_UNROLLED) && defined(IL_NO_REGISTERS_USED)
						_ILCVMUnrollGetNativeStack(thread->process->coder, &pc, &stacktop);
					#endif
						MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
					}
					VMBREAK(COP_PREFIX_UNROLL_STACK);

					/**
					 * <opcode name="unroll_stack_return"
					 *         group="Miscellaneous instructions">
					 *
					 *   <operation>Finish special stack variable
					 *   initialization for unroller</operation>
					 *
					 *   <dformat>{unroll_stack_return}</dformat>
					 *
					 *   <form name="unroll_stack_return"
					 *         code="COP_PREFIX_UNROLL_STACK_RETURN"/>
					 *
					 *   <description>The <i>unroll_stack_return</i>
					 *   instruction is used only to bootstrap the
					 *   unroller.<p/>
					 *
					 *   It gets the native stack pointer value obtained
					 *   through the "unroll_stack" instruction, computes
					 *   the offset of the interpreter variables against
					 *   it, and stores the offsets for internal use by
					 *   the unroller.</description>
					 * </opcode>
					 */
					VMCASE(COP_PREFIX_UNROLL_STACK_RETURN):
					{
					#ifdef IL_NO_REGISTERS_USED
						char *native_stack = stacktop[-1].ptrValue;
						_ILCVMSetPcOffset(
							thread->process->coder,
							((char *) &pc) - native_stack);
						_ILCVMSetStackOffset(
							thread->process->coder,
							((char *) &stacktop) - native_stack);
						_ILCVMSetFrameOffset(
							thread->process->coder,
							((char *) &frame) - native_stack);
						MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
					#else
						MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
					#endif
						return 0;
					}
					VMBREAK(COP_PREFIX_UNROLL_STACK_RETURN);

#ifndef IL_CVM_DIRECT
					VMPREFIXDEFAULT:
					{
						/* Treat all other prefixed opcodes as NOP */
						MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
					}
					VMBREAK(COP_PREFIX);
				}
			}
			VMOUTERBREAK;
#endif

			VMDEFAULT:
			{
				/* Treat all other opcodes as NOP */
				MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
			}
			VMBREAK(_DEFAULT_MAIN);
		}
	}

	/* We should never get here, but keep the compiler happy */
	return 0;
}

/*
 * Forward declarations.
 */
static int unwind(ILExecThread *thread,
				  ILCVMExceptionContext *callerExceptionContext);

/*
 * Call an exception filter.
 * NOTE: Exceptions are caught at exit from a filter and interpreted
 * as a 0 return value (continue search) as specified by ECMA.
 */
static int CallFilter(ILExecThread *thread,
					  ILCVMExceptionContext *callerExceptionContext)
{
	int result;

	do
	{
		result = Interpreter(thread);
		if(result == _CVM_EXIT_THROW)
		{
			/*
			 * Perform exception handling.
			 */
			result = unwind(thread, callerExceptionContext);
		}
	} while(result == _CVM_EXIT_EXECUTE_HANDLER);
	if(result == _CVM_EXIT_RETURN)
	{
		CVMWord *stacktop;
		/*
		 * We returned from en ENDFILTER opcode.
		 */

		/*
		 * Pop the result of the filter call.
		 * The result is on top of the stack.
		 */
		stacktop = thread->stackTop;
		if(stacktop[-1].intValue)
		{
			return _CVM_EXCEPTION_EXECUTE_HANDLER;
		}
	}
	return _CVM_EXCEPTION_CONTINUE_SEARCH;
}

/*
 * Call a catch clause while thread abort is in effect.
 */
static int CallHandler(ILExecThread *thread,
					   ILCVMExceptionContext *callerExceptionContext)
{
	int result;

	do
	{
		result = Interpreter(thread);
		if(result == _CVM_EXIT_THROW)
		{
			/*
			 * Perform exception handling.
			 */
			result = unwind(thread, callerExceptionContext);
		}
	} while(result == _CVM_EXIT_EXECUTE_HANDLER);
	return result;
}

/*
 * Perform normal exception unwinding
 */
static int unwind(ILExecThread *thread,
				  ILCVMExceptionContext *callerExceptionContext)
{
	unsigned char *pc;
	CVMWord *stacktop;
	CVMWord *frame;
	ILCallFrame *callFrame;
	ILMethod *method;
	ILObject *exception;
	ILCVMExceptionContext exceptionContext;
	const ILCVMUnwind *unwind;
	int threadIsAborting;

	/*
	 * Get the state from the thread.
	 */
	pc = thread->pc;
	frame = thread->frame;
	stacktop = thread->stackTop;
	method = thread->method;
	exception = _ILExecThreadGetException(thread);
	_ILExecThreadClearException(thread);

	threadIsAborting = 0;
	/*
	 * Check if the thread is aborting
	 */
	if (thread->aborting)
	{
		if (exception && ILExecThreadIsThreadAbortException(thread, exception))
		{
			threadIsAborting = 1;
		}
	}

	unwind = (ILCVMUnwind *)ILCoderPCToHandler(thread->process->coder, pc, 0);
	do
	{
		ILCVMContext context;

		if(unwind)
		{
			const ILCVMUnwind *exceptUnwind;
			/*
			 * Locate the unwind block ehere the exceptionPc is located
			 * and adjust the stacktop accordingly.
			 */
			context.pc = pc;
			context.frame = frame;
			context.stackTop = frame;
			exceptUnwind = FindUnwindBlock(unwind, &context);
			while(exceptUnwind)
			{
				if(exceptUnwind->flags ==  _IL_CVM_UNWIND_TYPE_TRY)
				{
					const ILCVMUnwind *handler;
					const ILCVMUnwind *filter;

					if(exceptUnwind->un.tryBlock.firstHandler >= 0)
					{
						int stopExecuteHandlers;
						int result;

						/*
						 * Process the handlers for this try block.
						 */
						result = _CVM_EXIT_OK;
						stopExecuteHandlers = 0;
						handler = &(unwind[exceptUnwind->un.tryBlock.firstHandler]);
						while(handler)
						{
							switch(handler->flags)
							{
								case _IL_CVM_UNWIND_TYPE_CATCH:
								{
									/*
									 * A typed exception handler.
									 */
									if(stopExecuteHandlers)
									{
										break;
									}
									
									if(exception && CanCastClass(ILProgramItem_Image(method),
																 GetObjectClass(exception),
																 handler->un.handlerBlock.un.exceptionClass))
									{
									executeHandler:
										stacktop = context.stackTop;
										/*
										 * Push the flag for propagating
										 * a ThreadAbortException on the stack.
										 */
										stacktop[0].ptrValue = (threadIsAborting ? IL_INVALID_PC : 0);
										++stacktop;
										/*
										 * Push the exception object on the stack.
										 */
										stacktop[0].ptrValue = exception;
										++stacktop;
										/*
										 * Store the exception in the frame for
										 * rethrowing the exception.
										 */
										if(exceptUnwind->exceptionSlot != IL_MAX_UINT32)
										{
											frame[exceptUnwind->exceptionSlot].ptrValue = exception;
										}
										/*
										 * and jump to the handler.
										 */
										thread->pc = handler->start;
										thread->stackTop = stacktop;
										thread->frame = context.frame;
										thread->method = method;
										if(threadIsAborting)
										{
											exceptionContext.exception = exception;
											exceptionContext.context = &context;
											exceptionContext.unwind = handler;
											result = CallHandler(thread, &exceptionContext);
											if(result == _CVM_EXIT_PROPAGATE_ABORT)
											{
												/*
												 * Don't execute any handlers for this try block anymore.
												 * Remaining finally or fault blocks should be executed.
												 */
												stopExecuteHandlers = 1;
											}
										}
										else
										{
											return _CVM_EXIT_EXECUTE_HANDLER;
										}
									}
								}
								break;

								case _IL_CVM_UNWIND_TYPE_FILTEREDCATCH:
								{
									if(stopExecuteHandlers)
									{
										break;
									}

									filter = &(unwind[handler->un.handlerBlock.un.filter]);
									/*
									 * Push the exception object on the stack.
									 */
									stacktop = context.stackTop;
									stacktop[0].ptrValue = exception;
									++stacktop;
									/*
									 * and execute the filter.
									 */
									thread->pc = filter->start;
									thread->stackTop = stacktop;
									thread->frame = context.frame;
									thread->method = method;
									exceptionContext.exception = exception;
									exceptionContext.context = &context;
									exceptionContext.unwind = filter;
									if(CallFilter(thread, &exceptionContext) == _CVM_EXCEPTION_EXECUTE_HANDLER)
									{
										/*
										 * The handler should be called
										 */
										goto executeHandler;
									}
								}
								break;

								case _IL_CVM_UNWIND_TYPE_FINALLY:
								case _IL_CVM_UNWIND_TYPE_FAULT:
								{
									/*
									 * Call a cleanup handler.
									 */
									/*
									 * Push the return address on the stack.
									 * IL_INVALID_PC is pushed so that the
									 * interpreter knows that the finally block
									 * was called during exception handling and
									 * is left on returning from the finally block.
									 */
									stacktop = context.stackTop;
									stacktop[0].ptrValue = IL_INVALID_PC;
									++stacktop;
									/*
									 * and jump to the handler.
									 */
									thread->pc = handler->start;
									thread->stackTop = stacktop;
									thread->frame = context.frame;
									thread->method = method;
									exceptionContext.exception = exception;
									exceptionContext.context = &context;
									exceptionContext.unwind = handler;
									result = CallHandler(thread, &exceptionContext);
								}
								break;
							}
							/*
							 * Check if aborting the thread has been reset.
							 */
							if(threadIsAborting)
							{
								if(!thread->aborting)
								{
									/*
									 * The abort has been aborted.
									 * So reset the threadIsAborting flag.
									 */
									threadIsAborting = 0;
								}
							}
							if(result == _CVM_EXIT_CONTINUE_UNWIND)
							{
								/*
								 * An exception has been thrown while
								 * performing the unwinding.
								 * So continue unwinding using this
								 * exception if the thread is not aborted.
								 */
								if(!threadIsAborting)
								{
									exception = exceptionContext.exception;
									if(thread->aborting)
									{
										if(exception &&
										   ILExecThreadIsThreadAbortException(thread, exception))
										{
											threadIsAborting = 1;
										}
									}
								}
								break;
							}
							/*
							 * Get the next handler.
							 */
							if(handler->un.handlerBlock.nextHandler >= 0)
							{
								handler = &(unwind[handler->un.handlerBlock.nextHandler]);
							}
							else
							{
								break;
							}
						}
					}
				}
				else if(callerExceptionContext)
				{
					if((callerExceptionContext->context->frame == frame) &&
					   (callerExceptionContext->unwind == exceptUnwind))
					{
						/*
						 * We are trying to unwind below the caller's position.
						 * So update the caller's exceptioncontext and let him
						 * decide what to do.
						 */
						callerExceptionContext->exception = exception;
						callerExceptionContext->context->pc = pc;
						return _CVM_EXIT_CONTINUE_UNWIND;
					}
				}
				/*
				 * Walk up to the parent unwind block.
				 */
				if(exceptUnwind->parent >= 0)
				{
					context.stackTop -= exceptUnwind->stackChange;
					exceptUnwind = &(unwind[exceptUnwind->parent]);
				}
				else
				{
					break;
				}
			}
		}
		/*
		 * Walk the stack up to caller.
		 */
		stacktop = frame;
		callFrame = &(thread->frameStack[--(thread->numFrames)]);
		pc = callFrame->pc;
		frame = callFrame->frame;
		method = callFrame->method;

#ifdef IL_DUMP_CVM
		if(method)
		{
			fprintf(IL_DUMP_CVM_STREAM, "Throwing Back To %s::%s\n",
				    method->member.owner->className->name,
				    method->member.name);
		}
#endif

		/* Should we return to an external method? */
		if(callFrame->pc == IL_INVALID_PC)
		{
			_ILExecThreadSetException(thread, exception);
			COPY_STATE_TO_THREAD();
			return _CVM_EXIT_EXCEPT;
		}
		unwind = (ILCVMUnwind *)ILCoderPCToHandler(thread->process->coder, pc - 1, 1);
	} while(1);
}

int _ILCVMInterpreter(ILExecThread *thread)
{
	int result;

	if(!thread)
	{
		/*
		 * Export the goto label tables from the interpreter if necessary
		 */
		return Interpreter(thread);
	}

	do
	{
		result = Interpreter(thread);
		while(result == _CVM_EXIT_THROW)
		{
			/*
			 * Perform exception handling.
			 */
			result = unwind(thread, 0);
		}
		if(result != _CVM_EXIT_EXECUTE_HANDLER)
		{
			break;
		}
	} while(1);
	return result;
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_CVM */
