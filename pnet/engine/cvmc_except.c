/*
 * cvmc_except.c - Coder implementation for CVM exceptions.
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
#include "cvm_format.h"

#ifdef IL_CVMC_FUNCTIONS

/*
 * Allocate extra local variables needed for exception handling.
 */
static void CVMEntrySetupExceptions(CVMEntryContext *ctx, ILCVMCoder *coder,
									ILCoderExceptions *exceptions)
{
	if(exceptions->numBlocks > 0)
	{
		ILCoderExceptionBlock *exception;
		ILUInt32 index;
		ILUInt32 offset;

		offset = ctx->numLocalWords + ctx->numArgWords;
		index = 0;
		while(index < exceptions->numBlocks)
		{
			exception = &(exceptions->blocks[index]);
			exception->userData = IL_MAX_UINT32;
			if((exception->flags & IL_CODER_HANDLER_TYPE_CATCH) != 0)
			{
				if(exception->un.handlerBlock.tryBlock->userData == IL_MAX_UINT32)
				{
					exception->un.handlerBlock.tryBlock->userData = offset;
					++offset;
				}
				exception->userData = exception->un.handlerBlock.tryBlock->userData;
			}
			++index;
		}
		ctx->numLocalWords = offset - ctx->numArgWords;
	}
}

#endif

#ifdef IL_CVMC_CODE

/*
 * Output a throw instruction.
 */
static void CVMCoder_Throw(ILCoder *coder, int inCurrentMethod)
{
	CVMP_OUT_NONE(COP_PREFIX_THROW);
	CVM_ADJUST(-1);
}

/*
 * Output a stacktrace instruction.
 */
static void CVMCoder_SetStackTrace(ILCoder *coder)
{
	CVMP_OUT_NONE(COP_PREFIX_SET_STACK_TRACE);
}

/*
 * Output a rethrow instruction.
 */
static void CVMCoder_Rethrow(ILCoder *coder, ILCoderExceptionBlock *exception)
{
	/* Push the saved exception object back onto the stack */
	CVM_OUT_WIDE(COP_PLOAD, exception->userData);
	CVM_ADJUST(1);

	/* Throw the object to this method's exception handler table */
	CVMP_OUT_NONE(COP_PREFIX_THROW);
	CVM_ADJUST(-1);
}

/*
 * Output a "call to a finally ot fault subroutine" instruction.
 */
static void CVMCoder_CallFinally(ILCoder *coder, ILCoderExceptionBlock *exception,
								 ILUInt32 dest)
{
	OutputBranch(coder, COP_JSR, dest);
}

/*
 * Output a "return from subroutine" instruction.
 */
static void CVMCoder_RetFromFinally(ILCoder *coder)
{
	CVM_OUT_NONE(COP_RET_JSR);
}

static void CVMCoder_LeaveCatch(ILCoder *coder,
								ILCoderExceptionBlock *exception)
{
	CVMP_OUT_NONE(COP_PREFIX_LEAVE_CATCH);
}

static void CVMCoder_RetFromFilter(ILCoder *coder)
{
	CVMP_OUT_NONE(COP_PREFIX_RET_FROM_FILTER);
}

static void CVMCoder_OutputExceptionTable(ILCoder *_coder,
										  ILCoderExceptions *exceptions)
{
	ILCVMCoder *coder = (ILCVMCoder *)_coder;
	ILCoderExceptionBlock *exception;
	ILCVMUnwind *cvmUnwind;
	ILCVMUnwind *unwind;
	ILCVMLabel *startLabel;
	ILCVMLabel *endLabel;
	int index;

	if(exceptions->numBlocks == 0)
	{
		/* There is no exception handling needed for this method */
		return;
	}

	cvmUnwind = (ILCVMUnwind *)ILCacheAlloc(&(coder->codePosn),
											(exceptions->numBlocks + 1) * sizeof(ILCVMUnwind));
	if(!cvmUnwind)
	{
		return;
	}
	
	/* Clear the cvm unwind information */
	ILMemZero(cvmUnwind, (exceptions->numBlocks + 1) * sizeof(ILCVMUnwind));

	/*
	 * Set the cookie for the method's exception region
	 * to the unwind information.
	 */
	ILCacheSetCookie(&(coder->codePosn), cvmUnwind);

	/* Setup the unwind information for the method */
	cvmUnwind[0].start = coder->start;
	cvmUnwind[0].end = CVM_POSN();
	cvmUnwind[0].flags = _IL_CVM_UNWIND_TYPE_TRY;
	cvmUnwind[0].parent = -1;
	cvmUnwind[0].nested = -1;
	cvmUnwind[0].nextNested = -1;
	cvmUnwind[0].stackChange = coder->minHeight;
	cvmUnwind[0].exceptionSlot = -1;
	cvmUnwind[0].un.tryBlock.firstHandler = -1;

	index = 0;
	while(index < exceptions->numBlocks)
	{
		exception = &(exceptions->blocks[index]);
		startLabel = GetLabel(coder, exception->startOffset);
		endLabel = GetLabel(coder, exception->endOffset);
		if(!startLabel || !endLabel)
		{
			/*
			 * This is a bug in the coder.
			 * Both labels must be  defined at this point.
			 */
			return;
		}
		unwind = &(cvmUnwind[index + 1]);
		unwind->start = coder->start + startLabel->offset;
		unwind->end = coder->start + endLabel->offset;
		unwind->parent = -1;
		unwind->nested = -1;
		unwind->nextNested = -1;
		unwind->stackChange = 0;
		/*
		 * Copy the exception slot to the unwind information.
		 */
		unwind->exceptionSlot = exception->userData;
		/*
		 * And reuse the exception's userData for the index in the unwind information.
		 */
		exception->userData = index + 1;
		switch(exception->flags)
		{
			case IL_CODER_HANDLER_TYPE_TRY:
			{
				unwind->flags = _IL_CVM_UNWIND_TYPE_TRY;
				unwind->stackChange = 0;
				unwind->un.tryBlock.firstHandler = -1;
			}
			break;

			case IL_CODER_HANDLER_TYPE_CATCH:
			{
				unwind->flags = _IL_CVM_UNWIND_TYPE_CATCH;
				unwind->stackChange = 1; /* For the propagate abort flag */
				unwind->un.handlerBlock.nextHandler = -1;
				unwind->un.handlerBlock.un.exceptionClass =
					exception->un.handlerBlock.exceptionClass;
			}
			break;

			case IL_CODER_HANDLER_TYPE_FILTEREDCATCH:
			{
				unwind->flags = _IL_CVM_UNWIND_TYPE_FILTEREDCATCH;
				unwind->stackChange = 1; /* For the propagate abort flag */
				unwind->un.handlerBlock.nextHandler = -1;
			}
			break;

			case IL_CODER_HANDLER_TYPE_FINALLY:
			{
				unwind->flags = _IL_CVM_UNWIND_TYPE_FINALLY;
				unwind->stackChange = 1; /* For the return address */
				unwind->un.handlerBlock.nextHandler = -1;
			}
			break;

			case IL_CODER_HANDLER_TYPE_FAULT:
			{
				unwind->flags = _IL_CVM_UNWIND_TYPE_FAULT;
				unwind->stackChange = 1; /* For the return address */
				unwind->un.handlerBlock.nextHandler = -1;
			}
			break;

			case IL_CODER_HANDLER_TYPE_FILTER:
			{
				unwind->flags = _IL_CVM_UNWIND_TYPE_FILTER;
				unwind->stackChange = 0;
			}
			break;
		}
		++index;
	}

	/*
	 * Now set the indexes in the blocks.
	 */
	index = 0;
	while(index < exceptions->numBlocks)
	{
		exception = &(exceptions->blocks[index]);
		unwind = &(cvmUnwind[exception->userData]);
		if(exception->parent)
		{
			unwind->parent = exception->parent->userData;
		}
		else
		{
			/*
			 * Set the outermost block as parent.
			 */
			unwind->parent = 0;
		}
		if(exception->nested)
		{
			unwind->nested = exception->nested->userData;
		}
		if(exception->nextNested)
		{
			unwind->nextNested = exception->nextNested->userData;
		}
		switch(exception->flags)
		{
			case IL_CODER_HANDLER_TYPE_TRY:
			{
				if(exception->un.tryBlock.handlerBlock)
				{
					unwind->un.tryBlock.firstHandler =
						exception->un.tryBlock.handlerBlock->userData;
				}
			}
			break;

			case IL_CODER_HANDLER_TYPE_CATCH:
			case IL_CODER_HANDLER_TYPE_FILTEREDCATCH:
			case IL_CODER_HANDLER_TYPE_FINALLY:
			case IL_CODER_HANDLER_TYPE_FAULT:
			{
				if(exception->un.handlerBlock.nextHandler)
				{
					unwind->un.handlerBlock.nextHandler =
						exception->un.handlerBlock.nextHandler->userData;
				}
				if(exception->flags == IL_CODER_HANDLER_TYPE_FILTEREDCATCH)
				{
					if(exception->un.handlerBlock.filterBlock)
					{
						unwind->un.handlerBlock.un.filter =
							exception->un.handlerBlock.filterBlock->userData;
					}
				}
			}
		}
		++index;
	}
	/*
	 * Set the nested information for the function block.
	 */
	cvmUnwind[0].nested = exceptions->firstBlock->userData;
}

/*
 * Convert a program counter into an exception handler.
 */
static void *CVMCoder_PCToHandler(ILCoder *_coder, void *pc, int beyond)
{
	void *cookie;
	if(beyond)
	{
		pc = ILCacheRetAddrToPC(pc);
	}
	if(ILCacheGetMethod(((ILCVMCoder *)_coder)->cache, pc, &cookie))
	{
		return cookie;
	}
	else
	{
		return 0;
	}
}

/*
 * Convert a program counter into a method descriptor.
 */
static ILMethod *CVMCoder_PCToMethod(ILCoder *_coder, void *pc, int beyond)
{
	if(beyond)
	{
		pc = ILCacheRetAddrToPC(pc);
	}
	return ILCacheGetMethod(((ILCVMCoder *)_coder)->cache, pc, (void **)0);
}

#endif	/* IL_CVMC_CODE */
