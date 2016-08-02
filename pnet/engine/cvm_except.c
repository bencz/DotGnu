/*
 * cvm_except.c - Opcodes for handling exceptions.
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

#if defined(IL_CVM_GLOBALS)

/*
 * Find the most nested unwind information block where the pc.
 * Returns NULL if no such exception block could be found.
 */
static const ILCVMUnwind *FindUnwindBlock(const ILCVMUnwind *unwind,
										  ILCVMContext *context)
{
	const unsigned char *pc;
	ILInt32 index ;
	CVMWord *stacktop;
	const ILCVMUnwind *prevEB;

	pc = context->pc;
	stacktop = context->frame;
	prevEB = 0;
	index = 0;
	while(index >= 0)
	{
		const ILCVMUnwind *block;

		block = &(unwind[index]);
		if(pc < block->start)
		{
			break;
		}
		else if(pc >= block->end)
		{
			index = block->nextNested;
		}
		else
		{
			prevEB = block;
			stacktop += block->stackChange;
			index = block->nested;
		}
	}
	/*
	 * Update the stacktop in the context structure.
	 */
	context->stackTop = stacktop;
	return prevEB;
}

#elif defined(IL_CVM_LOCALS)

/* No locals required */

#elif defined(IL_CVM_MAIN)

/**
 * <opcode name="jsr" group="Exception handling instructions">
 *   <operation>Jump to local subroutine</operation>
 *
 *   <format>jsr<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>jsr
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{jsr}<fsep/>dest</dformat>
 *
 *   <form name="jsr" code="COP_JSR"/>
 *
 *   <before>...</before>
 *   <after>..., address</after>
 *
 *   <description>The program counter for the next instruction (<i>pc + 6</i>)
 *   is pushed on the stack as type <code>ptr</code>.  Then the program
 *   branches to <i>pc + offset</i>.</description>
 *
 *   <notes>This instruction is used to implement <code>finally</code>
 *   blocks.</notes>
 * </opcode>
 */
VMCASE(COP_JSR):
{
	/* Jump to a subroutine within this method */
	stacktop[0].ptrValue = (void *)CVM_ARG_JSR_RETURN;
	pc = CVM_ARG_BRANCH_SHORT;
	stacktop += 1;
}
VMBREAK(COP_JSR);

/**
 * <opcode name="ret_jsr" group="Exception handling instructions">
 *   <operation>Return from local subroutine</operation>
 *
 *   <format>ret_jsr</format>
 *   <dformat>{ret_jsr}</dformat>
 *
 *   <form name="ret_jsr" code="COP_RET_JSR"/>
 *
 *   <before>..., address</before>
 *   <after>...</after>
 *
 *   <description>The <i>address</i> is popped from the stack as the
 *   type <code>ptr</code> and transferred into <i>pc</i>.
 *   If the resultint pc is an invalid pc the finally block was called
 *   during exception handling and the interpreter is left with an
 *   return code.</description>
 *
 *   <notes>This instruction is used to implement returning from
 *   <code>finally</code> blocks.</notes>
 * </opcode>
 */
VMCASE(COP_RET_JSR):
{
	/* Return from a subroutine within this method */
	if((unsigned char *)(stacktop[-1].ptrValue) == IL_INVALID_PC)
	{
		COPY_STATE_TO_THREAD();
#if defined(IL_USE_INTERRUPT_BASED_X)
		IL_MEMCPY(&thread->exceptionJumpBuffer, &backupJumpBuffer, sizeof(IL_JMP_BUFFER));
#endif
		return _CVM_EXIT_RETURN;
	}
	else
	{
		pc = (unsigned char *)(stacktop[-1].ptrValue);
		stacktop -= 1;
	}
}
VMBREAK(COP_RET_JSR);

#elif defined(IL_CVM_PREFIX)

/**
 * <opcode name="throw" group="Exception handling instructions">
 *   <operation>Throw an exception</operation>
 *
 *   <format>prefix<fsep/>throw</format>
 *   <dformat>{throw}</dformat>
 *
 *   <form name="throw" code="COP_PREFIX_THROW"/>
 *
 *   <before>..., working1, ..., workingN, object</before>
 *   <after>..., object</after>
 *
 *   <description>The <i>object</i> is popped from the stack as
 *   type <code>ptr</code>.  The stack is then reset to the same
 *   as the current method's exception frame height.  Then,
 *   <i>object</i> is re-pushed onto the stack and control is
 *   passed to the current method's exception matching code.</description>
 *
 *   <notes>This is used to throw exceptions within methods that
 *   have an <i>enter_try</i> instruction.  Use <i>throw_caller</i>
 *   if the method does not include <code>try</code> blocks.<p/>
 *
 *   Setting the stack height to the exception frame height ensures
 *   that all working values are removed from the stack prior to entering
 *   the exception matching code.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_THROW):
{
	/* Pop the exception object from the stack. */
	--stacktop;
	tempptr = stacktop[0].ptrValue;
	COPY_STATE_TO_THREAD();
	/*
	 * Label that we jump to when the engine throws an exception.
	 * The exception thrown is expected to be in tempptr.
	 */
throwException:
	/* Store the exception to the thread. */
	_ILExecThreadSetException(thread, tempptr);
	/*
	 * Label that we jump to when the exception thrown is allready
	 * stored in the thread's thrownException slot.
	 */
throwCurrentException:
#ifdef IL_DUMP_CVM
	fputs("Throw ", IL_DUMP_CVM_STREAM);
	DUMP_STACK();
#endif
#if defined(IL_USE_INTERRUPT_BASED_X)
	IL_MEMCPY(&thread->exceptionJumpBuffer, &backupJumpBuffer, sizeof(IL_JMP_BUFFER));
#endif
	return _CVM_EXIT_THROW;

	/*
	 * Jump target to throw a ThreadAbortException from the
	 * managed barrier.
	 */
throwThreadAbortException:
	COPY_STATE_TO_THREAD();
	ILInterlockedAndU4(&(thread->managedSafePointFlags),
					   ~_IL_MANAGED_SAFEPOINT_THREAD_ABORT);
	goto throwCurrentException;

	/*
	 * Jump target to throw a NullReferenceException
	 */
throwNullReferenceException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.NullReferenceException");
	goto throwException;

	/*
	 * Jump target to throw an ArithmeticException
	 */
throwArithmeticException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.ArithmeticException");
	goto throwException;

	/*
	 * Jump target to throw an OverflowException
	 */
throwOverflowException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.OverflowException");
	goto throwException;

	/*
	 * Jump target to throw a DivideByZeroException
	 */
throwDivideByZeroException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.DivideByZeroException");
	goto throwException;

	/*
	 * Jump target to throw a StackOverflowException
	 */
throwStackOverflowException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.StackOverflowException");
	goto throwException;

	/*
	 * Jump target to throw a MissingMethodException
	 */
throwMissingMethodException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.MissingMethodException");
	goto throwException;

	/*
	 * Jump target to throw an InvalidCastException
	 */
throwInvalidCastException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.InvalidCastException");
	goto throwException;

	/*
	 * Jump target to throw an IndexOutOfRangeException
	 */
throwIndexOutOfRangeException:
	COPY_STATE_TO_THREAD();
	tempptr = _ILSystemException(thread, "System.IndexOutOfRangeException");
	goto throwException;
}
VMBREAK(COP_PREFIX_THROW);

/**
 * <opcode name="set_stack_trace" group="Exception handling instructions">
 *   <operation>Set the stack trace in an exception object at
 *              the throw point</operation>
 *
 *   <format>prefix<fsep/>set_stack_trace</format>
 *   <dformat>{set_stack_trace}</dformat>
 *
 *   <form name="set_stack_trace" code="COP_PREFIX_SET_STACK_TRACE"/>
 *
 *   <before>..., object</before>
 *   <after>..., object</after>
 *
 *   <description>The <i>object</i> is popped from the stack as
 *   type <code>ptr</code>; information about the current method's
 *   stack calling context is written into <i>object</i>; and then
 *   <i>object</i> is pushed back onto the stack.</description>
 *
 *   <notes>This opcode will have no effect if <i>object</i> is
 *   <code>null</code>, or if its class does not inherit from
 *   <code>System.Exception</code>.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_SET_STACK_TRACE):
{
	/* Set the stack trace within an exception object */
#if defined(IL_CONFIG_REFLECTION) && defined(IL_CONFIG_DEBUG_LINES)
	COPY_STATE_TO_THREAD();
	BEGIN_NATIVE_CALL();
	_ILSetExceptionStackTrace(thread, stacktop[-1].ptrValue);
	END_NATIVE_CALL();
	RESTORE_STATE_FROM_THREAD();
#endif
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SET_STACK_TRACE);

/**
 * <opcode name="leave_catch" group="Exception handling instructions">
 *   <operation>Leave a catch region and propagate a Thread.Abort exception</operation>
 *
 *   <format>prefix<fsep/>leave_catch</format>
 *   <dformat>{leave_catch}</dformat>
 *
 *   <form name="leave_catch" code="COP_PREFIX_LEAVE_CATCH"/>
 *
 *   <before>...pointer</before>
 *   <after>...</after>
 *
 *   <description>If the thread is aborting and a
 *   <code>ThreadAbortException</code> has been thrown then there is an
 *   invalid pc pushed on the stack by the exception handling mechanism
 *   prior to invoking the handler. Otherwise a 0 pointer was pushed.
 *   If the pointer on top of the stack is an invalid pc and the thread is 
 *   still aborting then return from the interpreter with a propagate abort
 *   returncode.
 *   </description>
 * </opcode>
 */
VMCASE(COP_PREFIX_LEAVE_CATCH):
{
	--stacktop;
	if((stacktop[0].ptrValue == IL_INVALID_PC) && (thread->aborting))
	{
		COPY_STATE_TO_THREAD();
#if defined(IL_USE_INTERRUPT_BASED_X)
		IL_MEMCPY(&thread->exceptionJumpBuffer, &backupJumpBuffer, sizeof(IL_JMP_BUFFER));
#endif
		return _CVM_EXIT_PROPAGATE_ABORT;
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_LEAVE_CATCH);

/**
 * <opcode name="ret_from_filter" group="Exception handling instructions">
 *   <operation>Return from an exception filter</operation>
 *
 *   <format>prefix<fsep/>ret_from_filter</format>
 *   <dformat>{ret_from_filter}</dformat>
 *
 *   <form name="ret_from_filter" code="COP_PREFIX_RET_FROM_FILTER"/>
 *
 *   <before>...int</before>
 *   <after>...</after>
 *
 *   <description>Return to the exception handling with a <code>return</code>
 *   returncode. The value on top of the stack will be examined there.
 *   If the value is not 0 the corresponding catch handler will be invoked.
 *   </description>
 * </opcode>
 */
VMCASE(COP_PREFIX_RET_FROM_FILTER):
{
	COPY_STATE_TO_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
#if defined(IL_USE_INTERRUPT_BASED_X)
	IL_MEMCPY(&thread->exceptionJumpBuffer, &backupJumpBuffer, sizeof(IL_JMP_BUFFER));
#endif
	return _CVM_EXIT_RETURN;
}
VMBREAK(COP_PREFIX_RET_FROM_FILTER);

#endif /* IL_CVM_PREFIX */
