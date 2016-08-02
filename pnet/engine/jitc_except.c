/*
 * jitc_except.c - Coder implementation for JIT exceptions.
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
 * Declaration of the engine internal exceptions.
 */
#define _IL_JIT_OK						0
#define _IL_JIT_OUT_OF_MEMORY			1
#define _IL_JIT_INVALID_CAST			2
#define _IL_JIT_INDEX_OUT_OF_RANGE		3
#define _IL_JIT_MISSING_METHOD			4
#define _IL_JIT_DLL_NOT_FOUND			5
#define _IL_JIT_ENTRYPOINT_NOT_FOUND	6

/*
 * Emit the code to throw a system exception.
 */
static void _ILJitThrowSystem(ILJitFunction jitFunction,
							  ILUInt32 exception);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_CODER_INSTANCE

	/* Flag if the catcher is started. */
	int				isInCatcher;
	jit_label_t     nextBlock;
	jit_label_t     rethrowBlock;
	ILJitValue		aborting;
	ILJitValue		abortBlock;
	ILJitValue		threadAbortException;

#endif	/* IL_JITC_CODER_INSTANCE */

#ifdef	IL_JITC_CODER_INIT

	/* Initialize the exception stuff */
	coder->isInCatcher = 0;
	coder->nextBlock = 0;
	coder->rethrowBlock = 0;
	coder->aborting = 0;
	coder->abortBlock = 0;
	coder->threadAbortException = 0;

#endif	/* IL_JITC_CODER_INIT */

#ifdef	IL_JITC_CODER_DESTROY

	/* Nothing to do here */

#endif	/* IL_JITC_CODER_DESTROY */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Find the "stackTrace" field within "System.Exception" and then set.
 */
static void _ILJitFindAndSetStackTrace(ILJITCoder *jitCoder, ILJitValue exception)
{
	ILExecThread *_thread = ILExecThreadCurrent();
	ILJitValue thread = _ILJitCoderGetThread(jitCoder);
	ILJitValue trace;
	ILField *field;

	/* Find the "stackTrace" field within the "Exception" class */
	field = ILExecThreadLookupField
			(_thread, "System.Exception", "stackTrace",
			 "[vSystem.Diagnostics.PackedStackFrame;");
	if(field)
	{
		/* Get the stack trace and pop the frame */
		trace = jit_insn_call_native(jitCoder->jitFunction,
									 "_ILJitGetExceptionStackTrace",
									 _ILJitGetExceptionStackTrace,
									 _ILJitSignature_ILJitGetExceptionStackTrace,
									 &thread, 1, JIT_CALL_NOTHROW);

		/* Write the stack trace into the object */
		jit_insn_store_relative(jitCoder->jitFunction, exception,
								field->offset, trace);
	}
}

/*
 * Emit the code to throw a system exception.
 */
static void _ILJitThrowSystem(ILJitFunction jitFunction,
							  ILUInt32 exception)
{
	static const char * const exceptionClasses[] = {
		"Ok",
		"System.OutOfMemoryException",
		"System.InvalidCastException",
		"System.IndexOutOfRangeException",
		"System.MissingMethodException",
		"System.DllNotFoundException",
		"System.EntryPointNotFoundException"
	};
	#define	numExceptions	(sizeof(exceptionClasses) / sizeof(const char *))
	ILExecThread *_thread = ILExecThreadCurrent();

	if(exception == _IL_JIT_OUT_OF_MEMORY)
	{
		jit_insn_call_native(jitFunction,
							 "ILRuntimeExceptionThrowOutOfMemory",
							 ILRuntimeExceptionThrowOutOfMemory,
							 _ILJitSignature_ILRuntimeExceptionThrowOutOfMemory,
							 0, 0, JIT_CALL_NORETURN);
		return;
	}
	if(exception > 0)
	{
		ILClass *classInfo = _ILLookupClass(_ILExecThreadProcess(_thread),
											exceptionClasses[exception],
											strlen(exceptionClasses[exception]));
		ILJitValue info;
		if(!classInfo)
		{
		#ifndef REDUCED_STDIO
			/* Huh?  The required class doesn't exist.  This shouldn't happen */
			fprintf(stderr, "Fatal error: %s is missing from the system library\n",
					exceptionClasses[exception]);
			return;
		#endif
		}
		classInfo = ILClassResolve(classInfo);
		if(!(classInfo->userData) || 
			(((ILClassPrivate *)(classInfo->userData))->inLayout))
		{
			if(!_LayoutClass(_thread, classInfo))
			{
				return;
			}
		}
		info = jit_value_create_nint_constant(jitFunction,
											  _IL_JIT_TYPE_VPTR,
											  (jit_nint)classInfo);
		jit_insn_call_native(jitFunction,
							 "ILRuntimeExceptionThrowClass",
							 ILRuntimeExceptionThrowClass,
							 _ILJitSignature_ILRuntimeExceptionThrowClass,
							 &info, 1, JIT_CALL_NORETURN);
	}
}

/*
 * Output a table of exception matching directives.
 * Each table entry specifies a region of code for the
 * directive.  Whenever an exception occurs in this
 * region, the method will jump to the instructions
 * contained in the table entry.  These instructions
 * will typically call "finally" handlers, and then
 * attempt to match the exception against the rules.
 *
 * The code generated will be something like this
 *
 * if pc not in region1 goto check_for_region2
 * if pc not in nested_region1 goto check_for_nested_region2
 * 
 * check_for_nested_region2:
 * if pc not in nested_region2 goto call_handlers_for_region1
 *
 * call_handlers_for_region1:
 * call handlers for region 1
 * goto no_more_handlers
 * check_for_region2:
 * if pc not in region2 goto check_for_region3
 *
 * check_for_region3:
 */

static jit_label_t *GetNextParentHandlerLabel(ILCoderExceptions *coderExceptions,
											  ILCoderExceptionBlock *coderException)
{
	ILCoderExceptionBlock *currentCoderException;

	currentCoderException = coderException->parent;
	while(currentCoderException)
	{
		if(currentCoderException->flags == IL_CODER_HANDLER_TYPE_TRY)
		{
			/* This is a try block */
			if(!currentCoderException->handlerLabel)
			{
				currentCoderException->handlerLabel = (void *)jit_label_undefined;
			}
			return (jit_label_t *)&(currentCoderException->handlerLabel);
		}
		currentCoderException = currentCoderException->parent;
	}
	/*
	 * If we get here there is no surrounding try block so return the
	 * label for rethrowing the exception.
	 */
	if(!coderExceptions->rethrowLabel)
	{
		coderExceptions->rethrowLabel = (void *)jit_label_undefined;
	}
	return (jit_label_t *)&(coderExceptions->rethrowLabel);
}

static jit_label_t *GetNextHandlerBlockStartLabel(ILCoderExceptions *coderExceptions,
												  ILCoderExceptionBlock *handler)
{
	ILCoderExceptionBlock *currentHandler;

	currentHandler = handler->un.handlerBlock.nextHandler;
	if(currentHandler)
	{
		if(!currentHandler->startLabel)
		{
			currentHandler->startLabel = (void *)jit_label_undefined;
		}
		return (jit_label_t *)(&currentHandler->startLabel);
	}
	/*
	 * If we get here there is no other try block on the same level.
	 * So we return the label for calling the handlers of the first
	 * surrounding try block.
	 */
	return GetNextParentHandlerLabel(coderExceptions, handler);
}

static jit_label_t *GetNextTryBlockStartLabel(ILCoderExceptions *coderExceptions,
											  ILCoderExceptionBlock *coderException)
{
	ILCoderExceptionBlock *currentCoderException;

	currentCoderException = coderException->nextNested;
	while(currentCoderException)
	{
		if(currentCoderException->flags == IL_CODER_HANDLER_TYPE_TRY)
		{
			/* This is a try block */
			if(!currentCoderException->startLabel)
			{
				currentCoderException->startLabel = (void *)jit_label_undefined;
			}
			return (jit_label_t *)&(currentCoderException->startLabel);
		}
		else
		{
			/*
			 * See if there is a nested block.
			 */
			if(currentCoderException->nested)
			{
				/*
				 * The first nested block must be a try block.
				 */
				currentCoderException = currentCoderException->nested;
				if(!currentCoderException->startLabel)
				{
					currentCoderException->startLabel = (void *)jit_label_undefined;
				}
				return (jit_label_t *)&(currentCoderException->startLabel);
			}
		}
		currentCoderException = currentCoderException->nextNested;
	}
	/*
	 * If we get here there is no other try block on the same level.
	 * So we return the label for calling the handlers of the first
	 * surrounding try block.
	 */
	return GetNextParentHandlerLabel(coderExceptions, coderException);
}

/*
 * Set up exception handling for the current method.
 */
static void SetupExceptions(ILJITCoder *jitCoder,
							ILCoderExceptions *exceptions,
							int hasRethrow)
{
	ILJitValue nullPointer;
	ILJitValue nullInt32;
	int currentException;
	int currentCatchBlock;
	jit_label_t startLabel;
	jit_label_t endLabel;

	if(exceptions->numBlocks == 0)
	{
		/* Nothing to do here. */
		return;
	}

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"SetupExceptions: hasRethrow: %i\n", 
			hasRethrow);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/*
	 * Initialize the values needed for exception handling.
	 */
	startLabel = jit_label_undefined;
	endLabel = jit_label_undefined;
	if(!jit_insn_label(jitCoder->jitFunction, &startLabel))
	{
		return;
	}

	nullPointer = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_VPTR,
												 (jit_nint)0);
	nullInt32 = jit_value_create_nint_constant(jitCoder->jitFunction,
											   _IL_JIT_TYPE_INT32, 0);

	jitCoder->threadAbortException = jit_value_create(jitCoder->jitFunction, _IL_JIT_TYPE_VPTR);
	jit_insn_store(jitCoder->jitFunction, jitCoder->threadAbortException, nullPointer);
	jitCoder->abortBlock = jit_value_create(jitCoder->jitFunction, _IL_JIT_TYPE_INT32);
	jit_insn_store(jitCoder->jitFunction, jitCoder->abortBlock, nullInt32);
	jitCoder->aborting = jit_value_create(jitCoder->jitFunction, _IL_JIT_TYPE_INT32);
	jit_insn_store(jitCoder->jitFunction, jitCoder->aborting, nullInt32);

	/*
	 * Create a new block for the end label. This should be done
	 * by jit_insn_label anyways but it doesn't hurt.
	 */
	if(!jit_insn_new_block(jitCoder->jitFunction))
	{
		return;
	}

	if(!jit_insn_label(jitCoder->jitFunction, &endLabel))
	{
		return;
	}

	if(!jit_insn_move_blocks_to_start(jitCoder->jitFunction, startLabel,
									  endLabel))
	{
		return;
	}

	/* Setup the jit function to handle exceptions. */
	jit_insn_uses_catcher(jitCoder->jitFunction);
	jitCoder->nextBlock = jit_label_undefined;
	jitCoder->rethrowBlock = jit_label_undefined;

	/*
	 * Setup the labels for the entries of the exception blocks.
	 */
	currentException = 0;
	currentCatchBlock = 0;
	while(currentException < exceptions->numBlocks)
	{
		ILCoderExceptionBlock *block;

		block = &(exceptions->blocks[currentException]);
		switch(block->flags)
		{
			case IL_CODER_HANDLER_TYPE_FINALLY:
			case IL_CODER_HANDLER_TYPE_FAULT:
			{
			#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
				if (jitCoder->flags & IL_CODER_FLAG_STATS)
				{
					ILMutexLock(globalTraceMutex);
					fprintf(stdout,
						"AddFinallyLabel for offset: %i\n", 
						block->startOffset);
					ILMutexUnlock(globalTraceMutex);
				}
			#endif
				jitCoder->stackTop = 0;
				_ILJitLabelGet(jitCoder, block->startOffset,
										 _IL_JIT_LABEL_STARTFINALLY);
			}
			break;

			case IL_CODER_HANDLER_TYPE_CATCH:
			case IL_CODER_HANDLER_TYPE_FILTEREDCATCH:
			{
				/* Create the label for a catch block */
				ILJitValue exception;
				ILCoderExceptionBlock *tryBlock;

			#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
				if (jitCoder->flags & IL_CODER_FLAG_STATS)
				{
					ILMutexLock(globalTraceMutex);
					fprintf(stdout,
						"AddCatchLabel for offset: %i\n", 
						block->startOffset);
					ILMutexUnlock(globalTraceMutex);
				}
			#endif
				/*
				 * We save the created value in the exception for
				 * rethrowing the exception.
				 */
				tryBlock = block->un.handlerBlock.tryBlock;
				if(!tryBlock->ptrUserData)
				{
					exception = jit_value_create(jitCoder->jitFunction,
												 _IL_JIT_TYPE_VPTR);
					tryBlock->ptrUserData = (void *)exception;
					tryBlock->userData = ++currentCatchBlock;
				}
				block->userData = tryBlock->userData;
				block->ptrUserData = tryBlock->ptrUserData;
				/*
				 * Create the exception value that is on the stack on entry
				 * of the catch block.
				 */
				exception = jit_value_create(jitCoder->jitFunction,
											 _IL_JIT_TYPE_VPTR);
				_ILJitStackPushValue(jitCoder, exception);
				_ILJitLabelGet(jitCoder, block->startOffset,
							   _IL_JIT_LABEL_STARTCATCH);
				/*
				 * and reset the stack top.
				 */
				jitCoder->stackTop = 0;
			}
			break;

			case IL_CODER_HANDLER_TYPE_FILTER:
			{
				/* Create the label for a filter block */
				/* We need one value on the stack for the exception object. */
				ILJitValue exception;

			#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
				if (jitCoder->flags & IL_CODER_FLAG_STATS)
				{
					ILMutexLock(globalTraceMutex);
					fprintf(stdout,
						"AddFilterLabel for offset: %i\n", 
						block->startOffset);
					ILMutexUnlock(globalTraceMutex);
				}
			#endif
				/*
				 * Create the exception value that is on the stack on entry
				 * of the filter.
				 */
				exception = jit_value_create(jitCoder->jitFunction,
											 _IL_JIT_TYPE_VPTR);
				_ILJitStackPushValue(jitCoder, exception);
				_ILJitLabelGet(jitCoder, block->startOffset,
							   _IL_JIT_LABEL_STARTFILTER);
				/*
				 * and reset the stack top.
				 */
				jitCoder->stackTop = 0;
			}
			break;
		}
		++currentException;
	}
}

#endif	/* IL_JITC_FUNCTIONS */

#ifdef IL_JITC_CODE

/*
 * Output a throw instruction.
 */
static void JITCoder_Throw(ILCoder *coder, int inCurrentMethod)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Throw: inCurrentMethod: %i\n", 
			inCurrentMethod);
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	_ILJitStackItemNew(exception);

	_ILJitStackPop(jitCoder, exception);

	jit_insn_call_native(jitCoder->jitFunction,
						 "ILRuntimeExceptionThrow",
						 ILRuntimeExceptionThrow,
						 _ILJitSignature_ILRuntimeExceptionThrow,
						 &(_ILJitStackItemValue(exception)), 1,
						 JIT_CALL_NORETURN);
}

/*
 * Output a stacktrace instruction.
 */
static void JITCoder_SetStackTrace(ILCoder *coder)
{
}

/*
 * Output a rethrow instruction.
 */
static void JITCoder_Rethrow(ILCoder *coder, ILCoderExceptionBlock *exception)
{
	ILJITCoder *jitCoder;
	ILJitValue exceptionObject;

	jitCoder = _ILCoderToILJITCoder(coder);
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Rethrow: \n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif

	/*
	 * Get the current exception object.
	 * We saved the exception in the ptrUserData of the coder exception block.
	 */
	exceptionObject = (ILJitValue)(exception->ptrUserData);

	jit_insn_call_native(jitCoder->jitFunction,
						 "ILRuntimeExceptionRethrow",
						 ILRuntimeExceptionRethrow,
						 _ILJitSignature_ILRuntimeExceptionRethrow,
						 &exceptionObject, 1, JIT_CALL_NORETURN);
}

/*
 * Output a "call a finally or fault subroutine" instruction.
 */
static void JITCoder_CallFinally(ILCoder *coder,
								 ILCoderExceptionBlock *exception,
								 ILUInt32 dest)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJITLabel *label = _ILJitLabelGet(jitCoder, dest,
									   _IL_JIT_LABEL_STARTFINALLY);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"CallFinally: dest: %i\n",
			dest);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	jit_insn_call_finally(jitCoder->jitFunction, &(label->label));
}

/*
 * Output a "return from finally" instruction.
 */
static void JITCoder_RetFromFinally(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"RetFromFinally: \n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	jit_insn_return_from_finally(jitCoder->jitFunction);
}

/*
 * Output a "return from filter" instruction.
 */
static void JITCoder_RetFromFilter(ILCoder *coder)
{
	ILJITCoder *jitCoder;
	_ILJitStackItemNew(value);

	jitCoder = _ILCoderToILJITCoder(coder);
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"RetFromFilter: \n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	_ILJitStackPop(jitCoder, value);
	jit_insn_return_from_filter(jitCoder->jitFunction,
								_ILJitStackItemValue(value));
}

static void JITCoder_LeaveCatch(ILCoder *coder,
								ILCoderExceptionBlock *exception)
{
	ILJITCoder *jitCoder;
	ILJitValue thread;
	jit_label_t label;
	jit_label_t label1;
	ILJitValue currentExceptionBlock;
	ILJitValue temp;

	jitCoder = _ILCoderToILJITCoder(coder);
#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
				"LeaveCatch: \n");
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	thread = _ILJitCoderGetThread(jitCoder);
	label = jit_label_undefined;
	/* Check if we are handling a thread abort */
	jit_insn_branch_if_not(jitCoder->jitFunction, jitCoder->aborting, &label);
	/* Check if the abort was handled by the current catcher */
	currentExceptionBlock = jit_value_create_nint_constant(jitCoder->jitFunction,
														   _IL_JIT_TYPE_INT32,
														   exception->userData);
	temp = jit_insn_eq(jitCoder->jitFunction, jitCoder->abortBlock,
					   currentExceptionBlock);
	jit_insn_branch_if_not(jitCoder->jitFunction, temp, &label);
	/* Check if the thread is still aborting */
	temp = jit_insn_load_relative(jitCoder->jitFunction, thread,
								  offsetof(ILExecThread, aborting),
								  jit_type_sys_int);
	label1 = jit_label_undefined;
	jit_insn_branch_if_not(jitCoder->jitFunction, temp, &label1);
	/* If it still aborting then reset the aborting flag */
	temp = jit_value_create_nint_constant(jitCoder->jitFunction,
										  _IL_JIT_TYPE_INT32,
										 0);
	jit_insn_store(jitCoder->jitFunction, jitCoder->aborting, temp);
	/* and rethrow the thread abort exception. */
	jit_insn_call_native(jitCoder->jitFunction,
						 "ILRuntimeExceptionRethrow",
						 ILRuntimeExceptionRethrow,
						 _ILJitSignature_ILRuntimeExceptionRethrow,
						 &jitCoder->threadAbortException, 1,
						 JIT_CALL_NORETURN);
	jit_insn_label(jitCoder->jitFunction, &label1);
	/* Reset the aborting flag */
	temp = jit_value_create_nint_constant(jitCoder->jitFunction,
										  _IL_JIT_TYPE_INT32,
										 0);
	jit_insn_store(jitCoder->jitFunction, jitCoder->aborting, temp);
	jit_insn_label(jitCoder->jitFunction, &label);
}

static void JITCoder_OutputExceptionTable(ILCoder *coder,
								 		  ILCoderExceptions *coderExceptions)
{
	ILJITCoder *jitCoder;
	ILJitValue thread;
	ILJitValue temp;
	ILJitValue newAbortException;
	ILJitValue threadAbortExceptionClass;
	ILJitValue exceptionClass;
	ILJitValue exceptionObject;
    ILJitValue nullException;
	jit_label_t label;
	jit_label_t label1;
	int allowWalkUp;
	ILCoderExceptionBlock *coderException;
	ILCoderExceptionBlock *currentCoderException;
	ILCoderExceptionBlock *prevCoderException;
	
	jitCoder = _ILCoderToILJITCoder(coder);

	/* Tell libjit that we are in the catcher. */
	jit_insn_start_catcher(jitCoder->jitFunction);
	jitCoder->isInCatcher = 1;

	thread = _ILJitCoderGetThread(jitCoder);
	/* Get the exception object. */
	exceptionObject = jit_insn_load_relative(jitCoder->jitFunction, thread,
											 offsetof(ILExecThread, thrownException),
											 _IL_JIT_TYPE_VPTR);
	/* And clear the exception */
	nullException = jit_value_create_nint_constant(jitCoder->jitFunction,
												   _IL_JIT_TYPE_VPTR,
												   (jit_nint)0);
	jit_insn_store_relative(jitCoder->jitFunction, thread,
							offsetof(ILExecThread, thrownException),
							nullException);

	temp = jit_value_create_nint_constant(jitCoder->jitFunction,
										  _IL_JIT_TYPE_INT32, 0);
	newAbortException = jit_value_create(jitCoder->jitFunction,
										 _IL_JIT_TYPE_INT32);
	jit_insn_store(jitCoder->jitFunction, newAbortException, temp);

	/* Check if we have to handle a thread abort. */
	label = jit_label_undefined;
	/*
	 * First check if we are not allready handling a thread abort.
	 */
	jit_insn_branch_if(jitCoder->jitFunction, jitCoder->aborting, &label);
	/*
	 * If we dont handle a thread abort check if the thread is aborting.
	 */
	temp = jit_insn_load_relative(jitCoder->jitFunction, thread,
								  offsetof(ILExecThread, aborting),
								  jit_type_sys_int);
	jit_insn_branch_if_not(jitCoder->jitFunction,
						   temp, &label);
	/*
	 * If the thread is aborting check if the exception is a threadAbortException.
	 */
	threadAbortExceptionClass = jit_value_create_nint_constant(jitCoder->jitFunction,
															   _IL_JIT_TYPE_VPTR,
															   (jit_nint)jitCoder->process->threadAbortClass);
	exceptionClass = _ILJitGetObjectClass(jitCoder->jitFunction, exceptionObject);
	temp = jit_insn_eq(jitCoder->jitFunction,
					   exceptionClass,
					   threadAbortExceptionClass);

	/* If it's not then this exception is thrown while handling an abort. */
	jit_insn_branch_if_not(jitCoder->jitFunction, temp, &label);

	/* Otherwise store the current exception object for later use */
	jit_insn_store(jitCoder->jitFunction, jitCoder->threadAbortException, exceptionObject);
	/* and set the flag that we are handling a thread abort. */
	temp = jit_value_create_nint_constant(jitCoder->jitFunction, _IL_JIT_TYPE_INT32, 1);
	jit_insn_store(jitCoder->jitFunction, jitCoder->aborting, temp);
	jit_insn_store(jitCoder->jitFunction, newAbortException, temp);

	jit_insn_label(jitCoder->jitFunction, &label);

	/* Process all regions in the method */
	prevCoderException = 0;
	allowWalkUp = 1;
	currentCoderException = coderExceptions->firstBlock;
	while(currentCoderException)
	{
		if(allowWalkUp)
		{
			if(currentCoderException->flags == IL_CODER_HANDLER_TYPE_TRY)
			{
				jit_label_t *label;
				ILJITLabel *startLabel;
				ILJITLabel *endLabel;

				/* This is a try block */
				if(currentCoderException->startLabel)
				{
					/* This is a branch target */
					jit_insn_label(jitCoder->jitFunction,
								   (jit_label_t *)&(currentCoderException->startLabel));
				}
				label = GetNextTryBlockStartLabel(coderExceptions,
												  currentCoderException);
				startLabel = _ILJitLabelFind(jitCoder,
											 currentCoderException->startOffset);
				endLabel = _ILJitLabelFind(jitCoder,
										   currentCoderException->endOffset);
				if(startLabel && endLabel)
				{
					jit_insn_branch_if_pc_not_in_range(jitCoder->jitFunction,
													   startLabel->label,
													   endLabel->label,
													   label);
				}
			}
			if(currentCoderException->nested)
			{
				/*
				 * There are nested exception blocks.
				 * So process them first.
				 */
				currentCoderException = currentCoderException->nested;
				continue;
			}
		}
		/*
		 * Emit the handler code for this block.
		 */
		if(currentCoderException->flags == IL_CODER_HANDLER_TYPE_TRY)
		{
			/* This is a try block */
			ILJitValue currentExceptionBlock;
			jit_label_t *notHandledLabel;
			jit_label_t tempLabel;
			jit_label_t *tryBlockEndLabel;

			if(currentCoderException->handlerLabel)
			{
				/* This is a branch target */
				jit_insn_label(jitCoder->jitFunction,
							   (jit_label_t *)&(currentCoderException->handlerLabel));
			}
			/*
			 * Emit the handler code.
			 */
			tempLabel = jit_label_undefined;
			tryBlockEndLabel = &tempLabel;
			notHandledLabel = 0;
			label = jit_label_undefined;
			/* Check if we are handling a thread abort */
			jit_insn_branch_if_not(jitCoder->jitFunction, newAbortException, &label);
			/* Store the current block that handles the abort */
			currentExceptionBlock = jit_value_create_nint_constant(jitCoder->jitFunction,
																   _IL_JIT_TYPE_INT32,
																   currentCoderException->userData);
			jit_insn_store(jitCoder->jitFunction, jitCoder->abortBlock,
						   currentExceptionBlock);
			jit_insn_label(jitCoder->jitFunction, &label);
			coderException = currentCoderException->un.tryBlock.handlerBlock;
			while(coderException)
			{
				if(coderException->startLabel)
				{
					/* This is a branch target */
					jit_insn_label(jitCoder->jitFunction,
								   (jit_label_t *)&(coderException->startLabel));
				}
				notHandledLabel = GetNextHandlerBlockStartLabel(coderExceptions,
																coderException);
				if(coderException->un.handlerBlock.nextHandler == 0)
				{
					tryBlockEndLabel = notHandledLabel;
					notHandledLabel = &tempLabel;
				}
				switch(coderException->flags & IL_CODER_HANDLER_TYPE_MASK)
				{
					case IL_CODER_HANDLER_TYPE_FINALLY:
					case IL_CODER_HANDLER_TYPE_FAULT:
					{
						/*
						 * This is a finally or fault block.
						 */
						JITCoder_CallFinally(coder, coderException,
											 coderException->startOffset);
					}
					break;

					case IL_CODER_HANDLER_TYPE_CATCH:
					{
						/*
						 * This is a typed catch block.
						 */
						ILJitValue method;
						ILJitValue classTo;
						ILJitValue args[3];
						ILJitValue returnValue;
						ILJITLabel *catchBlock = 0;

						classTo = jit_value_create_nint_constant(jitCoder->jitFunction,
																 _IL_JIT_TYPE_VPTR,
																 (jit_nint)coderException->un.handlerBlock.exceptionClass);
						method = jit_value_create_nint_constant(jitCoder->jitFunction,
																_IL_JIT_TYPE_VPTR,
																(jit_nint)ILCCtorMgr_GetCurrentMethod(&(jitCoder->cctorMgr)));
						/* 
						 * Look if the object can be casted to the caught exception type.
						 */
						args[0] = method;
						args[1] = exceptionObject;
						args[2] = classTo;
						returnValue = jit_insn_call_native(jitCoder->jitFunction,
														   "ILRuntimeCanCastClass",
														   ILRuntimeCanCastClass,
														   _ILJitSignature_ILRuntimeCanCastClass,
														   args, 3, JIT_CALL_NOTHROW);
						jit_insn_branch_if_not(jitCoder->jitFunction,
											   returnValue, notHandledLabel);
						/*
						 * Save the current exception for a possible rethrow.
						 */
						jit_insn_store(jitCoder->jitFunction,
									   (ILJitValue)coderException->ptrUserData,
									   exceptionObject);
						jitCoder->stackTop = 0;
						_ILJitStackPushValue(jitCoder, exceptionObject);
						catchBlock = _ILJitLabelGet(jitCoder,
													coderException->startOffset,
													_IL_JIT_LABEL_STARTCATCH);
						jit_insn_branch(jitCoder->jitFunction, &(catchBlock->label));
						jitCoder->stackTop = 0;
					}
					break;

					case IL_CODER_HANDLER_TYPE_FILTEREDCATCH:
					{
						/*
						 * This is a filtered catch block.
						 */
						/*
						 * TODO: Handle catch blocks with filters.
						 */
						
						
					}
					break;
				}
				coderException = coderException->un.handlerBlock.nextHandler;
			}
			/*
			 * Handle a thrown threadAbortException.
			 */
			jit_insn_label(jitCoder->jitFunction, notHandledLabel);
			label = jit_label_undefined;
			label1 = jit_label_undefined;
			jit_insn_branch_if_not(jitCoder->jitFunction,
								   jitCoder->aborting, &label);
			currentExceptionBlock = jit_value_create_nint_constant(jitCoder->jitFunction,
																   _IL_JIT_TYPE_INT32,
																   currentCoderException->userData);
			temp = jit_insn_eq(jitCoder->jitFunction, jitCoder->abortBlock,
							   currentExceptionBlock);
			jit_insn_branch_if_not(jitCoder->jitFunction, temp, &label);
			/*
			 * The thread abort was last handled here.
			 * Check if the thread is still aborting.
			 */
			temp = jit_insn_load_relative(jitCoder->jitFunction, thread,
										  offsetof(ILExecThread, aborting),
										  jit_type_sys_int);
			jit_insn_branch_if_not(jitCoder->jitFunction,
								   temp, &label1);
			/*
			 * The thread is still aborting.
			 * So replace the current exception with the first threadAbortException.
			 */
			jit_insn_store(jitCoder->jitFunction, exceptionObject,
						   jitCoder->threadAbortException);
			/*
			 * And set the new thread abort again.
			 */
			temp = jit_value_create_nint_constant(jitCoder->jitFunction,
												  _IL_JIT_TYPE_INT32, 1);
			jit_insn_store(jitCoder->jitFunction, newAbortException, temp);
			jit_insn_branch(jitCoder->jitFunction, &label);
			jit_insn_label(jitCoder->jitFunction, &label1);
			/*
			 * Reset the aborting flag.
			 */
			temp = jit_value_create_nint_constant(jitCoder->jitFunction, _IL_JIT_TYPE_INT32, 0);
			jit_insn_store(jitCoder->jitFunction, jitCoder->aborting, temp);
			jit_insn_label(jitCoder->jitFunction, &label);
			jit_insn_branch(jitCoder->jitFunction, tryBlockEndLabel);
		}
		/*
		 * Look for the next block to process.
		 */
		if(currentCoderException->nextNested)
		{
			/*
			 * An other exception block on the same level.
			 */
			currentCoderException = currentCoderException->nextNested;
			allowWalkUp = 1;
		}
		else
		{
			currentCoderException = currentCoderException->parent;
			allowWalkUp = 0;
		}
	}

	if(coderExceptions->rethrowLabel)
	{
		/* This is a branch target */
		jit_insn_label(jitCoder->jitFunction,
					   (jit_label_t *)&(coderExceptions->rethrowLabel));
	}

	/*
	 * If execution gets here, then there were no applicable catch blocks,
	 * so we always throw the exception to the calling method.
	 */

	/*
	 * Notify the coder to emit profiling for method end
	 */
	if((jitCoder->flags & IL_CODER_FLAG_METHOD_PROFILE) != 0)
	{
		JITCoder_ProfilingEnd(coder);
	}

	/*
	 * Restore the thrown exception and throw it to the caller.
	 */
	label = jit_label_undefined;
	label1 = jit_label_undefined;
	/*
	 * If the thread is aborting then rethrow the thread_abort_exception.
	 */
	jit_insn_branch_if_not(jitCoder->jitFunction, jitCoder->aborting, &label);
	jit_insn_store_relative(jitCoder->jitFunction, thread,
							offsetof(ILExecThread, thrownException),
							jitCoder->threadAbortException);
	jit_insn_branch(jitCoder->jitFunction, &label1);
	jit_insn_label(jitCoder->jitFunction, &label);
	jit_insn_store_relative(jitCoder->jitFunction, thread,
							offsetof(ILExecThread, thrownException),
							exceptionObject);
	jit_insn_label(jitCoder->jitFunction, &label1);
	jit_insn_rethrow_unhandled(jitCoder->jitFunction);
}

/*
 * Convert a program counter into an exception handler.
 */
static void *JITCoder_PCToHandler(ILCoder *_coder, void *pc, int beyond)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(_coder);
	void *handler;

	ILJitFunction jitFunction = jit_function_from_pc(jitCoder->context, pc,
													 &handler);
	if(jitFunction)
	{
		return handler;
	}
	return 0;
}

/*
 * Convert a program counter into a method descriptor.
 */
static ILMethod *JITCoder_PCToMethod(ILCoder *_coder, void *pc, int beyond)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(_coder);
	ILJitFunction jitFunction = jit_function_from_pc(jitCoder->context, pc,
													 (void **)0);

	if(jitFunction)
	{
		return (ILMethod *)jit_function_get_meta(jitFunction,
												 IL_JIT_META_METHOD);
	}
	return 0;
}

#endif	/* IL_JITC_CODE */
