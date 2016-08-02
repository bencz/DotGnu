/*
 * coder.c - Common helper methods for code generation.
 *
 * Copyright (C) 2011  Southern Storm Software, Pty Ltd.
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
#include "coder.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Get the type of a parameter to the current method.
 * Returns 0 if the parameter number is invalid.
 */
ILType *_ILCoderGetParamType(ILType *signature, ILMethod *method,
							 ILUInt32 num)
{
	if(ILType_HasThis(signature))
	{
		/* This method has a "this" parameter */
		if(!num)
		{
			ILClass *owner;
			ILType *synthetic;

			owner = ILMethod_Owner(method);
			if(ILClassIsValueType(owner))
			{
				/* The "this" parameter is a value type, which is
				   being passed as a managed pointer.  Return
				   ILType_Invalid to tell the caller that special
				   handling is required */
				return ILType_Invalid;
			}
			synthetic = ILClassGetSynType(owner);
			if(synthetic)
			{
				return synthetic;
			}
			else
			{
				return ILType_FromClass(owner);
			}
		}
		else
		{
			return ILTypeGetParam(signature, num);
		}
	}
	else
	{
		return ILTypeGetParam(signature, num + 1);
	}
}

/*
 * Load the arguments from fromArg to toArg on the verification and
 * coder stack.
 * fromArg ... toArg must be valid argument numbers for the given
 * method and signature.
 */
void _ILCoderLoadArgs(ILCoder *coder, ILEngineStackItem *stack,
					  ILMethod *method, ILType *signature,
					  ILUInt32 fromArg, ILUInt32 toArg)
{
	ILUInt32 current;
	ILUInt32 stackTop;

	stackTop = 0;
	current = fromArg;
	while(current <= toArg)
	{
		ILType *paramType;

		paramType = _ILCoderGetParamType(signature, method, current);
		stack[stackTop].typeInfo = paramType;
		stack[stackTop].engineType = _ILTypeToEngineType(paramType);
		ILCoderLoadArg(coder, current, paramType);
		++stackTop;
		++current;
	}
}

/*
 * Set return type information within a stack item.
 */
void _ILCoderSetReturnType(ILEngineStackItem *item, ILType *returnType)
{
	if(returnType != ILType_Void)
	{
		item->engineType = _ILTypeToEngineType(returnType);
		if(item->engineType != ILEngineType_M)
		{
			item->typeInfo = returnType;
		}
		else
		{
			item->typeInfo = ILType_Ref(ILTypeStripPrefixes(returnType));
		}
	}
	else
	{
		item->engineType = ILEngineType_Invalid;
	}
}

/*
 * Process a "box" operation on a value.  Returns zero if
 * invalid parameters.
 */
int _ILCoderBoxValue(ILExecProcess *process, ILEngineType valueType,
					 ILType *typeInfo, ILClass *boxClass)
{
	ILUInt32 size;
	ILType *rawType;

	/* Determine the raw version of the boxing type */
	rawType = ILTypeGetEnumType(ILClassToType(boxClass));

	/* Get the size of the value type */
	size = _ILSizeOfTypeLocked(process, rawType);

	/* Determine how to box the value */
	if(ILType_IsPrimitive(rawType))
	{
		if(valueType == ILEngineType_I4)
		{
			/* Determine if we are boxing a byte, short, or int
			   based on the raw type */
			switch(ILType_ToElement(rawType))
			{
				case IL_META_ELEMTYPE_BOOLEAN:
				case IL_META_ELEMTYPE_I1:
				case IL_META_ELEMTYPE_U1:
				{
					ILCoderBoxSmaller(process->coder, boxClass, valueType, ILType_Int8);
					return 1;
				}
				/* Not reached */
	
				case IL_META_ELEMTYPE_I2:
				case IL_META_ELEMTYPE_U2:
				case IL_META_ELEMTYPE_CHAR:
				{
					ILCoderBoxSmaller(process->coder, boxClass, valueType, ILType_Int16);
					return 1;
				}
				/* Not reached */
	
				case IL_META_ELEMTYPE_I4:
				case IL_META_ELEMTYPE_U4:
			#ifdef IL_NATIVE_INT32
				case IL_META_ELEMTYPE_I:
				case IL_META_ELEMTYPE_U:
			#endif
				{
					ILCoderBox(process->coder, boxClass, valueType, size);
					return 1;
				}
				/* Not reached */
			}
		}
		else if(valueType == ILEngineType_I)
		{
			/* Box a native integer */
			switch(ILType_ToElement(rawType))
			{
				case IL_META_ELEMTYPE_I:
				case IL_META_ELEMTYPE_U:
				{
					ILCoderBox(process->coder, boxClass, valueType, size);
					return 1;
				}
				/* Not reached */
			}
		}
		else if(valueType == ILEngineType_I8)
		{
			/* Box a 64-bit integer */
			switch(ILType_ToElement(rawType))
			{
				case IL_META_ELEMTYPE_I8:
				case IL_META_ELEMTYPE_U8:
				{
					ILCoderBox(process->coder, boxClass, valueType, size);
					return 1;
				}
				/* Not reached */
			}
		}
		else if(valueType == ILEngineType_F)
		{
			/* Determine if we are boxing a float or double
			   based on the size of the value type */
			if(rawType == ILType_Float32)
			{
				ILCoderBoxSmaller(process->coder, boxClass, valueType, ILType_Float32);
				return 1;
			}
			else if(rawType == ILType_Float64 ||
				    rawType == ILType_Float)
			{
				ILCoderBoxSmaller(process->coder, boxClass, valueType, ILType_Float64);
				return 1;
			}
		}
	}
	else if(valueType == ILEngineType_MV ||
			valueType == ILEngineType_TypedRef)
	{
		if(ILTypeIdentical(typeInfo, ILClassToType(boxClass)))
		{
			ILCoderBox(process->coder, boxClass, valueType, size);
			return 1;
		}
	}
	return 0;
}

/*
 * Process a "box" operation on a value.  Returns zero if
 * invalid parameters.
 */
int _ILCoderBoxPtr(ILExecProcess *process, ILType *typeInfo, 
				   ILClass *boxClass, ILUInt32 pos)
{
	ILUInt32 size;
	ILType *rawType;

	/* Determine the raw version of the boxing type */
	rawType = ILTypeGetEnumType(ILClassToType(boxClass));

	/* Get the size of the value type */
	size = _ILSizeOfTypeLocked(process, rawType);

	if(ILTypeIdentical(typeInfo, ILClassToType(boxClass)))
	{
		ILCoderBoxPtr(process->coder, boxClass, size, pos);
		return 1;
	}

	return 0;
}

static int NestExceptionBlocks(ILCoderExceptionBlock *block,
							   ILCoderExceptionBlock *prevEB,
							   ILCoderExceptionBlock *firstNestedEB,
							   ILCoderExceptionBlock **firstEB)
{
	ILCoderExceptionBlock *lastNestedEB;
	ILCoderExceptionBlock *nextEB;

	block->nested = firstNestedEB;
	block->parent = firstNestedEB->parent;
	firstNestedEB->parent = block;
	lastNestedEB = firstNestedEB;
	nextEB = firstNestedEB->nextNested;
	while(nextEB)
	{
		if(block->endOffset <= nextEB->startOffset)
		{
			/*
			 * The next block is not nested in the current block.
			 */
			break;
		}
		if(block->endOffset >= nextEB->endOffset)
		{
			/*
			 * The next block is nested in the current block too.
			 */
			nextEB->parent = block;
			lastNestedEB = nextEB;
		}
		else
		{
			/* Partially overlapping blocks are not allowed */
			return 0;
		}
		nextEB = nextEB->nextNested;
	}
	block->nextNested = lastNestedEB->nextNested;
	lastNestedEB->nextNested = 0;
	if(prevEB)
	{
		prevEB->nextNested = block;
	}
	else
	{
		if(block->parent)
		{
			nextEB = block->parent;
			nextEB->nested = block;
		}
		else
		{
			nextEB = *firstEB;
			*firstEB = block;
		}
	}
	return 1;
}

static int InsertExceptionBlock(ILCoderExceptions *coderExceptions,
								ILCoderExceptionBlock *block)
{
	ILCoderExceptionBlock *prevEB;
	ILCoderExceptionBlock *checkEB;

	prevEB = 0;
	checkEB = coderExceptions->firstBlock;
	while(checkEB)
	{
		if(block->endOffset <= checkEB->startOffset)
		{
			/* The current block is before the check block */
			if(prevEB)
			{
				block->nextNested = prevEB->nextNested;
				prevEB->nextNested = block;
				block->parent = prevEB->parent;
			}
			else
			{
				if(checkEB->parent)
				{
					checkEB = checkEB->parent;
					block->nextNested = checkEB->nested;
					checkEB->nested = block;
					block->parent = checkEB;
				}
				else
				{
					block->nextNested = coderExceptions->firstBlock;
					coderExceptions->firstBlock = block;
				}
			}
			break;
		}
		else if(block->startOffset >= checkEB->endOffset)
		{
			/*
			 * The current block starts after the check block.
			 */
			if(checkEB->nextNested)
			{
				prevEB = checkEB;
				checkEB = checkEB->nextNested;
			}
			else
			{
				checkEB->nextNested = block;
				block->parent = checkEB->parent;
				break;
			}
		}
		else if(block->startOffset <= checkEB->startOffset &&
				block->endOffset >= checkEB->endOffset)
		{
			/*
			 * The current block encloses the check block.
			 */
			if(!NestExceptionBlocks(block, prevEB, checkEB,
									&(coderExceptions->firstBlock)))
			{
				return 0;
			}
			break;
		}
		else if(block->startOffset >= checkEB->startOffset &&
				block->endOffset <= checkEB->endOffset)
		{
			/*
			 * The current try block is nested in the check try block.
			 */
			if(checkEB->nested)
			{
				prevEB = 0;
				checkEB = checkEB->nested;
			}
			else
			{
				checkEB->nested = block;
				block->parent = checkEB;
				break;
			}
		}
		else
		{
			/*
			 * Partially overlapping blocks are not allowed.
			 */
			return 0;
		}
	}
	return 1;
}

static ILCoderExceptionBlock *FindOrAddTryBlock(ILCoderExceptions *coderExceptions,
												ILUInt32 tryStart, ILUInt32 tryEnd)
{
	ILCoderExceptionBlock *block;
	ILUInt32 current;
	
	current = 0;
	while(current < coderExceptions->numBlocks)
	{
		block = &(coderExceptions->blocks[current]);
		if((block->startOffset == tryStart) && (block->endOffset == tryEnd) &&
			(block->flags == IL_CODER_HANDLER_TYPE_TRY))
		{
			return block;
		}
		++current;
	}
	/* If we get here no matching try block was found */
	block = &(coderExceptions->blocks[coderExceptions->numBlocks++]);
	block->flags = IL_CODER_HANDLER_TYPE_TRY;
	block->startOffset = tryStart;
	block->endOffset = tryEnd;
	block->un.tryBlock.handlerBlock = 0;
	block->parent = 0;
	block->nested = 0;
	block->nextNested = 0;

	/*
	 * Now insert the new try block at it's place in the exception block
	 * structure
	 */
	if(!coderExceptions->firstBlock)
	{
		coderExceptions->firstBlock = block;
	}
	else
	{
		if(!InsertExceptionBlock(coderExceptions, block))
		{
			--coderExceptions->numBlocks;
			return 0;
		}
	}
	return block;
}

static void AddHandlerBlock(ILCoderExceptionBlock *tryBlock,
							ILCoderExceptionBlock *handler)
{
	if(!tryBlock->un.tryBlock.handlerBlock)
	{
		tryBlock->un.tryBlock.handlerBlock = handler;
	}
	else
	{
		ILCoderExceptionBlock *nextHandler;

		nextHandler = tryBlock->un.tryBlock.handlerBlock;
		while(nextHandler)
		{
			if(!nextHandler->un.handlerBlock.nextHandler)
			{
				nextHandler->un.handlerBlock.nextHandler = handler;
				break;
			}
			nextHandler = nextHandler->un.handlerBlock.nextHandler;
		}
	}
}

int _ILCoderAddExceptionBlock(ILCoderExceptions *coderExceptions,
							  ILMethod *method, ILException *exception)
{
	ILCoderExceptionBlock *tryBlock;
	ILCoderExceptionBlock *handler;
	ILUInt32 startOffset;
	ILUInt32 endOffset;

	startOffset = exception->tryOffset;
	endOffset = exception->tryOffset + exception->tryLength;
	if(endOffset < startOffset)
	{
		return IL_CODER_BRANCH_ERR;
	}
	/*
	 * Find the try block for this exception handler.
	 */
	tryBlock = FindOrAddTryBlock(coderExceptions, startOffset, endOffset);
	if(!tryBlock)
	{
		return IL_CODER_BRANCH_ERR;
	}

	startOffset = exception->handlerOffset;
	endOffset = exception->handlerOffset + exception->handlerLength;
	if(endOffset < startOffset)
	{
		return IL_CODER_BRANCH_ERR;
	}

	/*
	 * Allocate a new handler block.
	 */
	handler = &(coderExceptions->blocks[coderExceptions->numBlocks++]);
	handler->startOffset = startOffset;
	handler->endOffset = endOffset;
	handler->parent = 0;
	handler->nested = 0;
	handler->nextNested = 0;
	handler->startLabel = 0;
	handler->handlerLabel = 0;
	if(exception->flags & (IL_META_EXCEPTION_FINALLY | IL_META_EXCEPTION_FAULT))
	{
		/*
		 * A finally or fault handler.
		 */
		if(exception->flags & IL_META_EXCEPTION_FINALLY)
		{
			handler->flags = IL_CODER_HANDLER_TYPE_FINALLY;
		}
		else
		{
			handler->flags = IL_CODER_HANDLER_TYPE_FAULT;
		}
		handler->un.handlerBlock.nextHandler = 0;
		handler->un.handlerBlock.filterBlock = 0;
		handler->un.handlerBlock.exceptionClass = 0;
		handler->un.handlerBlock.tryBlock = tryBlock;;
	}
	else
	{
		handler->un.handlerBlock.nextHandler = 0;
		handler->un.handlerBlock.tryBlock = tryBlock;;
		if(exception->flags & IL_META_EXCEPTION_FILTER)
		{
			/*
			 * A catch block with filter.
			 */
			ILCoderExceptionBlock *filterBlock;

			filterBlock = &(coderExceptions->blocks[coderExceptions->numBlocks++]);
			filterBlock->flags = IL_CODER_HANDLER_TYPE_FILTER;
			filterBlock->startOffset = exception->extraArg;
			filterBlock->endOffset = exception->handlerOffset;
			filterBlock->parent = 0;
			filterBlock->nested = 0;
			filterBlock->nextNested = 0;
			filterBlock->startLabel = 0;
			filterBlock->handlerLabel = 0;
			if(!InsertExceptionBlock(coderExceptions, filterBlock))
			{
				return IL_CODER_BRANCH_ERR;
			}
			handler->flags = IL_CODER_HANDLER_TYPE_FILTEREDCATCH;
			handler->un.handlerBlock.filterBlock = filterBlock;
			handler->un.handlerBlock.exceptionClass = 0;
		}
		else
		{
			/*
			 * A catch block.
			 */
			ILClass *classInfo;
			ILProgramItem *item;

			handler->flags = IL_CODER_HANDLER_TYPE_CATCH;
			handler->un.handlerBlock.filterBlock = 0;

			/* Validate the class token */
			item = ((ILProgramItem *)ILImageTokenInfo(ILProgramItem_Image(method),
													  exception->extraArg));
			classInfo = ILProgramItemToClass(item);
			if(!classInfo ||
			   !ILClassAccessible(classInfo, ILMethod_Owner(method)))
			{
				return IL_CODER_TYPE_ERR;
			}
			handler->un.handlerBlock.exceptionClass = classInfo;
		}
	}
	if(!InsertExceptionBlock(coderExceptions, handler))
	{
		return IL_CODER_BRANCH_ERR;
	}
	AddHandlerBlock(tryBlock, handler);
	return IL_CODER_OK;
}

ILCoderExceptionBlock *_ILCoderFindExceptionBlock(ILCoderExceptions *coderExceptions,
												  ILUInt32 offset)
{
	ILCoderExceptionBlock *block;
	ILCoderExceptionBlock *prevEB;

	prevEB = 0;
	block = coderExceptions->firstBlock;
	while(block)
	{
		if(offset < block->startOffset)
		{
			break;
		}
		else if(offset >= block->endOffset)
		{
			block = block->nextNested;
		}
		else
		{
			prevEB = block;
			block = block->nested;
		}
	}
	return prevEB;
}

/*
 * Emit code to throw a system-level exception.
 * Returns IL_CODER_OK on success or IL_CODER_TYPE_ERR if the exception
 * or it's constructor could not be resolved.
 */
int _ILCoderThrowSystem(ILCoder *coder, ILMethod *method,
						const char *name, const char *namespace)
{
	ILClass *classInfo;
	ILMethod *ctor;
	ILCoderMethodInfo callInfo;

	/* Find the no-argument constructor for the class */
	classInfo = ILClassResolveSystem(ILProgramItem_Image(method), 0,
								     name, namespace);
	if(!classInfo)
	{
		return IL_CODER_TYPE_ERR;
	}
	ctor = 0;
	while((ctor = (ILMethod *)ILClassNextMemberByKind
			(classInfo, (ILMember *)ctor, IL_META_MEMBERKIND_METHOD)) != 0)
	{
		if(ILMethod_IsConstructor(ctor) &&
		   ILTypeNumParams(ILMethod_Signature(ctor)) == 0)
		{
			break;
		}
	}
	if(!ctor)
	{
		return IL_CODER_TYPE_ERR;
	}

	/* Invoke the constructor */
	callInfo.args = 0;
	callInfo.numBaseArgs = 0;
	callInfo.numVarArgs = 0;
	callInfo.hasParamArray = 0;
	ILCoderCallCtor(coder, &callInfo, ctor);

	/* Set the stack trace & throw the object */
	ILCoderSetStackTrace(coder);
	ILCoderThrow(coder, 0);
	return IL_CODER_OK;
}

#ifdef	__cplusplus
};
#endif
