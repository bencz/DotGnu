/*
 * verify_except.c - Verify instructions related to exceptions.
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

#if defined(IL_VERIFY_GLOBALS)

/*
 * Determine the most nested catch block where the offset is in..
 */
static IL_INLINE ILCoderExceptionBlock *
InsideExceptionHandler(ILCoderExceptions *coderExceptions, ILUInt32 offset)
{
	ILCoderExceptionBlock *block;

	block = _ILCoderFindExceptionBlock(coderExceptions, offset);
	while(block)
	{
		if(block->flags & IL_CODER_HANDLER_TYPE_CATCH)
		{
			return block;
		}
		block = block->parent;
	}
	return 0;
}

#define	ThrowSystem(namespace,name)	\
			_ILCoderThrowSystem(coder, method, (name), (namespace))

#elif defined(IL_VERIFY_LOCALS)

/* No locals required */

#else /* IL_VERIFY_CODE */

case IL_OP_THROW:
{
	/* Throw an exception */
	if(stackSize >= 1 && stack[stackSize - 1].engineType == ILEngineType_O)
	{
		/* If the current method has exception handlers, then throw
		   the object to those handlers.  Otherwise throw directly
		   to the calling method */
		ILCoderSetStackTrace(coder);
		if(coderExceptions.numBlocks > 0)
		{
			/* Throw to the exception table */
			ILCoderThrow(coder, 1);
		}
		else
		{
			/* If throwing to the calling method then make sure we exit the sync lock */

			if (isSynchronized)
			{
				PUSH_SYNC_OBJECT();
				ILCoderCallInlineable(coder, IL_INLINEMETHOD_MONITOR_EXIT, 0, 0);
			}

			/* Notify the coder to emit profiling for method end */
			if((coderFlags & IL_CODER_FLAG_METHOD_PROFILE) != 0)
			{
				ILCoderProfileEnd(coder);
			}

			ILCoderThrow(coder, 0);
		}
		stackSize = 0;
		lastWasJump = 1;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_RETHROW:
{
	/* Re-throw the current exception */
	coderException = InsideExceptionHandler(&coderExceptions, offset);
	if(coderException)
	{
		ILCoderRethrow(coder, coderException);
		lastWasJump = 1;
	}
	else
	{
		VERIFY_INSN_ERROR();
	}
}
break;

case IL_OP_ENDFINALLY:
{
	/* End the current "finally" or "fault" clause */
	if(stackSize == 0)
	{
		coderException = _ILCoderFindExceptionBlock(&coderExceptions, offset);
		if(!coderException ||
		   ((coderException->flags & IL_CODER_HANDLER_TYPE_FINALLY) == 0))
		{
			VERIFY_BRANCH_ERROR();
		}
		/* We are in a finally or fault block */
		ILCoderRetFromFinally(coder);
		stackSize = 0;
		lastWasJump = 1;
	}
	else
	{
		VERIFY_STACK_ERROR();
	}
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_ENDFILTER:
{
	/*
	 * End the current "filter" clause.
	 * There must be exactly one element on the evaluation stack of
	 * type int32.
	 */
	if(stackSize == 1)
	{
		coderException = _ILCoderFindExceptionBlock(&coderExceptions, offset);
		if(!coderException ||
		   (coderException->flags != IL_CODER_HANDLER_TYPE_FILTER))
		{
			VERIFY_BRANCH_ERROR();
		}
		ILCoderRetFromFilter(coder);
		lastWasJump = 1;
	}
	else
	{
	   VERIFY_STACK_ERROR();
	}
}
break;

case IL_OP_LEAVE_S:
{
	/* Unconditional short branch out of an exception block */
	dest = GET_SHORT_DEST();
processLeave:
	
	currentCoderException = 0;
	
	/* The stack must be empty when we leave the block */
	while(stackSize)
	{
		/* Pop the current top of stack */
		ILCoderPop(coder, stack[stackSize -1].engineType,
			   stack[stackSize -1].typeInfo);
		stackSize--;
	}

	/* Call any applicable "finally" handlers, but not "fault" handlers */
	coderException = _ILCoderFindExceptionBlock(&coderExceptions, offset);
	while(coderException != 0)
	{
		/*
		 * If the leave target is inside this exception block then stop here.
		 */
		if((coderException->startOffset <= dest) &&
		   (coderException->endOffset > dest))
		{
			break;
		}
		switch(coderException->flags)
		{
			case IL_CODER_HANDLER_TYPE_TRY:
			{
				currentCoderException = coderException->un.tryBlock.handlerBlock;
				while(currentCoderException)
				{
					if(currentCoderException->flags == IL_CODER_HANDLER_TYPE_FINALLY)
					{
						/* Call the "finally" clause for exiting this level */
						ILCoderCallFinally(coder, currentCoderException,
										   currentCoderException->startOffset);
					}
					currentCoderException = currentCoderException->un.handlerBlock.nextHandler;
				}
			}
			break;

			case IL_CODER_HANDLER_TYPE_FINALLY:
			case IL_CODER_HANDLER_TYPE_FAULT:
			case IL_CODER_HANDLER_TYPE_FILTER:
			{
				/*
				 * Finally, fault and filters are not allowed to be left with
				 * a leave opcode.
				 */
				VERIFY_BRANCH_ERROR();
			}
			break;

			case IL_CODER_HANDLER_TYPE_CATCH:
			case IL_CODER_HANDLER_TYPE_FILTEREDCATCH:
			{
				ILCoderLeaveCatch(coder, coderException);
			}
			break;
		}
		coderException = coderException->parent;
	}

	/* Output the branch instruction */
	ILCoderBranch(coder, opcode, dest, ILEngineType_I4, ILEngineType_I4);
	VALIDATE_BRANCH_STACK(dest);
	lastWasJump = 1;
}
break;

case IL_OP_LEAVE:
{
	/* Unconditional long branch out of an exception block */
	dest = GET_LONG_DEST();
	goto processLeave;
}
/* Not reached */

#endif /* IL_VERIFY_CODE */
