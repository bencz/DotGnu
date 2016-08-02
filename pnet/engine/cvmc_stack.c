/*
 * cvmc_stack.c - Coder implementation for CVM stack operations.
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

/*
 * Compute the size of a list of stack items.  The return
 * value is the number of stack words occupied by the items.
 */
static ILUInt32 ComputeStackSize(ILCoder *coder, ILEngineStackItem *stack,
								 ILUInt32 stackSize)
{
	ILUInt32 total = 0;
	while(stackSize > 0)
	{
		--stackSize;
		switch(stack[stackSize].engineType)
		{
			case ILEngineType_I4:
			case ILEngineType_M:
			case ILEngineType_CM:
			case ILEngineType_T:
			case ILEngineType_O:
			{
				++total;
			}
			break;

			case ILEngineType_I8:
			{
				total += CVM_WORDS_PER_LONG;
			}
			break;

			case ILEngineType_I:
			{
			#ifdef IL_NATIVE_INT32
				++total;
			#else
				total += CVM_WORDS_PER_LONG;
			#endif
			}
			break;

			case ILEngineType_F:
			{
				total += CVM_WORDS_PER_NATIVE_FLOAT;
			}
			break;

			case ILEngineType_MV:
			{
				total += GetTypeSize(_ILCoderToILCVMCoder(coder)->process,
										stack[stackSize].typeInfo);
			}
			break;

			case ILEngineType_TypedRef:
			{
				total += CVM_WORDS_PER_TYPED_REF;
			}
			break;

			case ILEngineType_Invalid: break;
		}
	}
	return total;
}

/*
 * Refresh the coder's notion of the stack contents.
 */
static void CVMCoder_StackRefresh(ILCoder *_coder, ILEngineStackItem *stack,
							      ILUInt32 stackSize)
{
	ILCVMCoder *coder = (ILCVMCoder *)_coder;
	coder->height = coder->minHeight +
					ComputeStackSize(_coder, stack, stackSize);
	if(coder->height > coder->maxHeight)
	{
		coder->maxHeight = coder->height;
	}
}

/*
 * Duplicate a number of stack words.
 */
static void DupWords(ILCoder *coder, ILUInt32 numWords)
{
	if(numWords == 1)
	{
		CVM_OUT_NONE(COP_DUP);
	}
	else if(numWords == 2)
	{
		CVM_OUT_NONE(COP_DUP2);
	}
	else
	{
		CVM_OUT_WIDE(COP_DUP_N, numWords);
	}
	CVM_ADJUST(numWords);
}

/*
 * Duplicate the top element on the stack.
 */
static void CVMCoder_Dup(ILCoder *coder, ILEngineType engineType, ILType *type)
{
	switch(engineType)
	{
		case ILEngineType_I4:
		case ILEngineType_M:
		case ILEngineType_O:
		case ILEngineType_T:
		{
			DupWords(coder, 1);
		}
		break;

		case ILEngineType_I8:
		{
			DupWords(coder, CVM_WORDS_PER_LONG);
		}
		break;

		case ILEngineType_I:
		{
			DupWords(coder, CVM_WORDS_PER_NATIVE_INT);
		}
		break;

		case ILEngineType_F:
		{
			DupWords(coder, CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case ILEngineType_MV:
		{
			ILUInt32 size = GetTypeSize(((ILCVMCoder *)coder)->process, type);
			DupWords(coder, size);
		}
		break;

		case ILEngineType_TypedRef:
		{
			DupWords(coder, CVM_WORDS_PER_TYPED_REF);
		}
		break;

		default: break;
	}
}

/*
 * Pop a number of stack words.
 */
static void PopWords(ILCoder *coder, ILUInt32 numWords)
{
	if(numWords == 1)
	{
		CVM_OUT_NONE(COP_POP);
	}
	else if(numWords == 2)
	{
		CVM_OUT_NONE(COP_POP2);
	}
	else
	{
		CVM_OUT_WIDE(COP_POP_N, numWords);
	}
	CVM_ADJUST(-((ILInt32)(numWords)));
}

/*
 * Pop the top element on the stack.
 */
static void CVMCoder_Pop(ILCoder *coder, ILEngineType engineType, ILType *type)
{
	switch(engineType)
	{
		case ILEngineType_I4:
		case ILEngineType_M:
		case ILEngineType_O:
		case ILEngineType_T:
		{
			PopWords(coder, 1);
		}
		break;

		case ILEngineType_I8:
		{
			PopWords(coder, CVM_WORDS_PER_LONG);
		}
		break;

		case ILEngineType_I:
		{
			PopWords(coder, CVM_WORDS_PER_NATIVE_INT);
		}
		break;

		case ILEngineType_F:
		{
			PopWords(coder, CVM_WORDS_PER_NATIVE_FLOAT);
		}
		break;

		case ILEngineType_MV:
		{
			ILUInt32 size = GetTypeSize(_ILCoderToILCVMCoder(coder)->process,
										type);
			PopWords(coder, size);
		}
		break;

		case ILEngineType_TypedRef:
		{
			PopWords(coder, CVM_WORDS_PER_TYPED_REF);
		}
		break;

		default: break;
	}
}

#endif	/* IL_CVMC_CODE */
