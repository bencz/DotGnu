/*
 * jitc_branch.c - Coder implementation for JIT branches.
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

#ifdef IL_JITC_CODE

/*
 * Handle a label.
 */
static void JITCoder_Label(ILCoder *coder, ILUInt32 offset)
{
	ILJITCoder *jitCoder;
	ILJITLabel *label;

	jitCoder = _ILCoderToILJITCoder(coder);
	label = _ILJitLabelGet(jitCoder, offset, _IL_JIT_LABEL_NORMAL);
	if(label)
	{
		_ILJitValuesResetNullChecked(jitCoder);
		if(label->labelType == _IL_JIT_LABEL_STARTCATCH)
		{
		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"StartCatcher: %i\n", 
					offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitLabelRestoreStack(jitCoder, label);
			jit_insn_label(jitCoder->jitFunction, &(label->label));
		}
		else if(label->labelType == _IL_JIT_LABEL_STARTFINALLY)
		{
		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"StartFinally: %i\n", 
					offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			jit_insn_start_finally(jitCoder->jitFunction, &(label->label));
		}
		else if(label->labelType == _IL_JIT_LABEL_STARTFILTER)
		{
			
		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"StartFilter: %i\n", 
					offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			jit_insn_start_filter(jitCoder->jitFunction, &(label->label),
									_IL_JIT_TYPE_INT32);
		}
		else
		{
		#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
			if (jitCoder->flags & IL_CODER_FLAG_STATS)
			{
				ILMutexLock(globalTraceMutex);
				fprintf(stdout,
					"Label: %i\n", 
					offset);
				ILMutexUnlock(globalTraceMutex);
			}
		#endif
			_ILJitLabelRestoreStack(jitCoder, label);
			jit_insn_label(jitCoder->jitFunction, &(label->label));
		}
	}
}

/*
 * Output a comparision between the 2 top most values on the evaluation stack.
 * The result value is returned.
 */
static ILJitValue OutputCompare(ILJITCoder *coder, int opcode,
								ILJitValue *value1, ILJitValue *value2)
{
	switch(opcode)
	{
		case IL_OP_PREFIX + IL_PREFIX_OP_CEQ:
		case IL_OP_BEQ:
		{
			/* Test two values for equality */
			AdjustMixedBinary(coder, 0, value1, value2);
			return jit_insn_eq(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_BNE_UN:
		{
			/* Test two unsigned values for inequality */
			AdjustMixedBinary(coder, 1, value1, value2);
			return jit_insn_ne(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CGT:
		case IL_OP_BGT:
		{
			/* Test two signed values for greater than */
			AdjustMixedBinary(coder, 0, value1, value2);
			return jit_insn_gt(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CGT_UN:
		case IL_OP_BGT_UN:
		{
			/* Test two unsigned values for greater than */
			AdjustMixedBinary(coder, 1, value1, value2);
			return jit_insn_gt(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_BGE:
		{
			/* Test two signed values for greater than  or equal */
			AdjustMixedBinary(coder, 0, value1, value2);
			return jit_insn_ge(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_BGE_UN:
		{
			/* Test two unsigned values for greater than  or equal */
			AdjustMixedBinary(coder, 1, value1, value2);
			return jit_insn_ge(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CLT:
		case IL_OP_BLT:
		{
			/* Test two signed values for less than */
			AdjustMixedBinary(coder, 0, value1, value2);
			return jit_insn_lt(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CLT_UN:
		case IL_OP_BLT_UN:
		{
			/* Test two unsigned values for less than */
			AdjustMixedBinary(coder, 1, value1, value2);
			return jit_insn_lt(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_BLE:
		{
			/* Test two signed values for less than or equal */
			AdjustMixedBinary(coder, 0, value1, value2);
			return jit_insn_le(coder->jitFunction, *value1, *value2);
		}
		break;

		case IL_OP_BLE_UN:
		{
			/* Test two unsigned values for less than  or equal */
			AdjustMixedBinary(coder, 1, value1, value2);
			return jit_insn_le(coder->jitFunction, *value1, *value2);
		}
		break;
	}
	return 0;
}

/*
 * Output a branch instruction using a JIT coder.
 */
static void JITCoder_Branch(ILCoder *coder, int opcode, ILUInt32 dest,
				   		    ILEngineType type1, ILEngineType type2)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	ILJITLabel *label = 0;
	_ILJitStackItemNew(value2);
	_ILJitStackItemNew(value1);

#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS)
	if (jitCoder->flags & IL_CODER_FLAG_STATS)
	{
		ILMutexLock(globalTraceMutex);
		fprintf(stdout,
			"Branch: %i\n", 
			dest);
		ILMutexUnlock(globalTraceMutex);
	}
#endif
	/* Determine what form of branch to use */
	switch(opcode)
	{
		case IL_OP_BR:
		case IL_OP_BR_S:
		case IL_OP_LEAVE:
		case IL_OP_LEAVE_S:
		{
			/* Unconditional branch */
			label = _ILJitLabelGet(jitCoder, dest, _IL_JIT_LABEL_NORMAL);

			jit_insn_branch(jitCoder->jitFunction, &(label->label));
		}
		break;

		case IL_OP_BRTRUE_S:
		case IL_OP_BRTRUE:
		{
			/* Branch if the top-most stack item is true */
			_ILJitStackPop(jitCoder, value1);
			label = _ILJitLabelGet(jitCoder, dest, _IL_JIT_LABEL_NORMAL);

			jit_insn_branch_if(jitCoder->jitFunction,
							   _ILJitStackItemValue(value1),
							   &(label->label));
		}
		break;

		case IL_OP_BRFALSE_S:
		case IL_OP_BRFALSE:
		{
			/* Branch if the top-most stack item is false */
			_ILJitStackPop(jitCoder, value1);
			label = _ILJitLabelGet(jitCoder, dest, _IL_JIT_LABEL_NORMAL);

			jit_insn_branch_if_not(jitCoder->jitFunction,
								   _ILJitStackItemValue(value1),
								   &(label->label));
		}
		break;

		default:
		{
			int invertBranch = 0;
			ILJitValue temp = 0;
			_ILJitStackPop(jitCoder, value2);
			_ILJitStackPop(jitCoder, value1);
			label = _ILJitLabelGet(jitCoder, dest, _IL_JIT_LABEL_NORMAL);

			/*
			 * Note: Adjust the unsigned/unordered branch opcodes for float
			 * values so that NaN values are handled correctly.
			 */
			if(type1 == ILEngineType_F)
			{
				switch(opcode)
				{
					case IL_OP_BNE_UN:
					case IL_OP_BNE_UN_S:
					{
						opcode = IL_OP_BEQ;
						invertBranch = 1;
					}
					break;

					case IL_OP_BGT_UN:
					case IL_OP_BGT_UN_S:
					{
						opcode = IL_OP_BLE;
						invertBranch = 1;
					}
					break;

					case IL_OP_BGE_UN:
					case IL_OP_BGE_UN_S:
					{
						opcode = IL_OP_BLT;
						invertBranch = 1;
					}
					break;

					case IL_OP_BLT_UN:
					case IL_OP_BLT_UN_S:
					{
						opcode = IL_OP_BGE;
						invertBranch = 1;
					}
					break;

					case IL_OP_BLE_UN:
					case IL_OP_BLE_UN_S:
					{
						opcode = IL_OP_BGT;
						invertBranch = 1;
					}
					break;
				}
			}

			switch(opcode)
			{
				case IL_OP_BEQ:
				case IL_OP_BEQ_S:
				{
					/* Equality testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BEQ,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BNE_UN:
				case IL_OP_BNE_UN_S:
				{
					/* Unsigned inequality testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BNE_UN,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BGT:
				case IL_OP_BGT_S:
				{
					/* Signed greater than testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BGT,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BGT_UN:
				case IL_OP_BGT_UN_S:
				{
					/* Unsigned greater than testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BGT_UN,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BGE:
				case IL_OP_BGE_S:
				{
					/* Signed greater than or equal testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BGE,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BGE_UN:
				case IL_OP_BGE_UN_S:
				{
					/* Unsigned greater than or equal testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BGE_UN,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BLT:
				case IL_OP_BLT_S:
				{
					/* Signed less than testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BLT,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BLT_UN:
				case IL_OP_BLT_UN_S:
				{
					/* Unsigned less than testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BLT_UN,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BLE:
				case IL_OP_BLE_S:
				{
					/* Signed less than or equal testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BLE,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;

				case IL_OP_BLE_UN:
				case IL_OP_BLE_UN_S:
				{
					/* Unsigned less than or equal testing branch */
					temp = OutputCompare(jitCoder, IL_OP_BLE_UN,
										 &(_ILJitStackItemValue(value1)),
										 &(_ILJitStackItemValue(value2)));
				}
				break;
			}
			if(temp)
			{
				if(invertBranch)
				{
					jit_insn_branch_if_not(jitCoder->jitFunction, temp,
										   &(label->label));
				}
				else
				{
					jit_insn_branch_if(jitCoder->jitFunction, temp,
									   &(label->label));
				}
			}
		}
		break;
	}

}

/*
 * Output the start of a table-based switch statement.
 */
static void JITCoder_SwitchStart(ILCoder *coder, ILUInt32 numEntries)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(value);

	_ILJitStackPop(jitCoder, value);
	jitCoder->numSwitch = 0;
	jitCoder->maxSwitch = numEntries;
	jitCoder->switchValue = _ILJitStackItemValue(value);

	jitCoder->switchLabels = ILMalloc(numEntries * sizeof(jit_label_t));
	if(!jitCoder->switchLabels)
	{
		jitCoder->labelOutOfMemory = 1;
	}
}

/*
 * Output an entry for a table-based switch statement.
 */
static void JITCoder_SwitchEntry(ILCoder *_coder, ILUInt32 dest)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(_coder);
	ILJITLabel *label = _ILJitLabelGet(jitCoder, dest, _IL_JIT_LABEL_NORMAL);

	if (!label)
	{
		return;
	}

	if (label->label == jit_label_undefined)
	{
		label->label = jit_function_reserve_label(jitCoder->jitFunction);
	}

	jitCoder->switchLabels[jitCoder->numSwitch] = label->label;
	++jitCoder->numSwitch;
}

/*
 * Output the end of a table-based switch statement.
 */
static void JITCoder_SwitchEnd(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);

	if(!jitCoder->switchLabels)
	{
		return;
	}

	if(!jit_insn_jump_table(jitCoder->jitFunction,
				jitCoder->switchValue,
				jitCoder->switchLabels,
				jitCoder->numSwitch))
	{
		jitCoder->labelOutOfMemory = 1;
	}

	ILFree(jitCoder->switchLabels);
	jitCoder->switchLabels = NULL;
}

/*
 * Output a comparison instruction.
 */
static void JITCoder_Compare(ILCoder *coder, int opcode,
				   		     ILEngineType type1, ILEngineType type2,
							 int invertTest)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(value2);
	_ILJitStackItemNew(value1);
	ILJitValue temp;

	_ILJitStackPop(jitCoder, value2);
	_ILJitStackPop(jitCoder, value1);

	/*
	 * Note: Adjust the unsigned/unordered compare opcodes for float values
	 * so that NaN values are handled correctly.
	 */
	if(type1 == ILEngineType_F)
	{
		switch(opcode)
		{
			case IL_OP_PREFIX + IL_PREFIX_OP_CGT_UN:
			{
				opcode = IL_OP_BLE;
				invertTest = ((invertTest == 0) ? 1 : 0);
			}
			break;

			case IL_OP_PREFIX + IL_PREFIX_OP_CLT_UN:
			{
				opcode = IL_OP_BGE;
				invertTest = ((invertTest == 0) ? 1 : 0);
			}
			break;
		}
	}

	temp = OutputCompare(jitCoder, opcode,
						 &(_ILJitStackItemValue(value1)),
						 &(_ILJitStackItemValue(value2)));
	if(invertTest)
	{
		temp = jit_insn_to_not_bool(jitCoder->jitFunction, temp);
	}
	_ILJitStackPushValue(jitCoder, temp);
}

#endif	/* IL_JITC_CODE */
