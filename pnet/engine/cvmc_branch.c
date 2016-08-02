/*
 * cvmc_branch.c - Coder implementation for CVM branches.
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
 * Look for a label for a specific IL address.  Create
 * a new label if necessary.
 */
static ILCVMLabel *GetLabel(ILCVMCoder *coder, ILUInt32 address)
{
	ILCVMLabel *label;
	label = coder->labelList;
	while(label != 0)
	{
		if(label->address == address)
		{
			return label;
		}
		label = label->next;
	}
	label = ILMemPoolAlloc(&(coder->labelPool), ILCVMLabel);
	if(label)
	{
		label->address = address;
		label->offset = ILCVM_LABEL_UNDEF;
		label->next = coder->labelList;
		coder->labelList = label;
		label->nextRef = 0;
	}
	else
	{
		coder->labelOutOfMemory = 1;
	}
	return label;
}

/*
 * Handle a label.
 */
static void CVMCoder_Label(ILCoder *_coder, ILUInt32 offset)
{
	ILCVMCoder *coder = (ILCVMCoder *)_coder;
	ILCVMLabel *label;
	ILCVMLabel *ref;
	ILCVMLabel *nextRef;
	ILInt32 dest;

	/* Reset the stack height: the caller will notify us of
	   the new stack contents by calling "StackRefresh" */
	coder->height = coder->minHeight;

	/* If we might be unrolling the code later, then mark the label */
	if(_ILCVMUnrollPossible())
	{
		CVM_OUT_NONE(COP_NOP);
	}

	/* Search for a label with this address */
	label = GetLabel(coder, offset);
	if(!label)
	{
		return;
	}

	/* Set the label offset to the current code position */
	label->offset = (ILUInt32)(CVM_POSN() - coder->start);

	/* Back-patch any references that are attached to the label */
	ref = label->nextRef;
	while(ref != 0)
	{
		if(!ILCacheIsFull(coder->cache, &(coder->codePosn)))
		{
			dest = (ILInt32)(label->offset - ref->offset);
			if(ref->address == ILCVM_LABEL_UNDEF)
			{
				/* Ordinary branch instruction */
				if(dest >= (ILInt32)(-128) && dest <= (ILInt32)(127))
				{
					/* Short jump */
					CVM_BACKPATCH_BRANCH(coder->start + ref->offset, dest);
				}
				else
				{
					/* Long jump */
					CVM_BACKPATCH_BRANCH_LONG(coder->start + ref->offset, dest);
				}
			}
			else
			{
				/* Switch table entry */
				CVM_BACKPATCH_SWENTRY(coder->start + ref->offset, dest,
									  coder->start + ref->address);
			}
		}
		nextRef = ref->nextRef;
		ILMemPoolFree(&(coder->labelPool), ref);
		ref = nextRef;
	}
	label->nextRef = 0;
}

/*
 * Output an actual branch opcode.
 */
static void OutputBranch(ILCoder *_coder, int opcode, ILUInt32 dest)
{
	ILCVMCoder *coder = (ILCVMCoder *)_coder;
	ILCVMLabel *label;
	ILCVMLabel *ref;
	ILInt32 relative;

	/* Search for an existing label for this destination */
	label = GetLabel(coder, dest);
	if(!label)
	{
		return;
	}

	/* Have we already seen the definition for the label? */
	if(label->offset != ILCVM_LABEL_UNDEF)
	{
		/* Yes we have, so output the final branch instruction */
		relative = (ILInt32)(label->offset - (CVM_POSN() - coder->start));
		if(relative >= (ILInt32)(-128) && relative <= (ILInt32)(127))
		{
			CVM_OUT_BRANCH(opcode, relative);
		}
		else
		{
			CVM_OUT_BRANCH_LONG(opcode, relative);
		}
	}
	else
	{
		/* Record a reference to the label for later back-patching */
		ref = ILMemPoolAlloc(&(coder->labelPool), ILCVMLabel);
		if(ref)
		{
			ref->address = ILCVM_LABEL_UNDEF;
			ref->offset = (ILUInt32)(CVM_POSN() - coder->start);
			ref->next = 0;
			ref->nextRef = label->nextRef;
			label->nextRef = ref;
		}
		else
		{
			coder->labelOutOfMemory = 1;
		}

		/* Output a place-holder into the code stream */
		CVM_OUT_BRANCH_PLACEHOLDER(opcode);
	}
}

/*
 * Output a conditional branch opcode.
 */
static void OutputCondBranch(ILCoder *coder, int iopcode, int lopcode,
							 int fopcode, int cmpopcode,
							 ILEngineType type, ILUInt32 dest)
{
	if(type == ILEngineType_I4)
	{
		OutputBranch(coder, iopcode, dest);
	}
	else if(type == ILEngineType_I8)
	{
		CVMP_OUT_NONE(lopcode);
		CVM_ADJUST(-(CVM_WORDS_PER_LONG * 2) + 1);
		CVM_OUT_NONE(COP_LDC_I4_0);
		CVM_ADJUST(1);
		OutputBranch(coder, cmpopcode, dest);
	}
	else if(type == ILEngineType_F)
	{
		CVMP_OUT_NONE(fopcode);
		CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT * 2) + 1);
		CVM_OUT_NONE(COP_LDC_I4_0);
		CVM_ADJUST(1);
		OutputBranch(coder, cmpopcode, dest);
	}
	else if(type == ILEngineType_M || type == ILEngineType_T ||
	        type == ILEngineType_O || type == ILEngineType_I)
	{
		if(cmpopcode == COP_BEQ)
		{
			OutputBranch(coder, COP_BR_PEQ, dest);
		}
		else if(cmpopcode == COP_BNE)
		{
			OutputBranch(coder, COP_BR_PNE, dest);
		}
		else
		{
			CVMP_OUT_NONE(COP_PREFIX_PCMP);
			CVM_ADJUST(-1);
			CVM_OUT_NONE(COP_LDC_I4_0);
			CVM_ADJUST(1);
			OutputBranch(coder, cmpopcode, dest);
		}
	}
	CVM_ADJUST(-2);
}

/*
 * Output a branch instruction using a CVM coder.
 */
static void CVMCoder_Branch(ILCoder *coder, int opcode, ILUInt32 dest,
							ILEngineType type1, ILEngineType type2)
{
	/* Determine what form of branch to use */
	switch(opcode)
	{
		case IL_OP_BR:
		case IL_OP_BR_S:
		case IL_OP_LEAVE:
		case IL_OP_LEAVE_S:
		{
			/* Unconditional branch */
			OutputBranch(coder, COP_BR, dest);
		}
		break;

		case IL_OP_BRTRUE_S:
		case IL_OP_BRTRUE:
		{
			/* Branch if the top-most stack item is true */
		#ifdef IL_NATIVE_INT32
			if(type1 == ILEngineType_I4 || type1 == ILEngineType_I)
		#else
			if(type1 == ILEngineType_I4)
		#endif
			{
				OutputBranch(coder, COP_BRTRUE, dest);
				CVM_ADJUST(-1);
			}
		#ifdef IL_NATIVE_INT64
			else if(type1 == ILEngineType_I8 || type1 == ILEngineType_I)
		#else
			else if(type1 == ILEngineType_I8)
		#endif
			{
				CVM_OUT_NONE(COP_LDC_I4_0);
				CVM_OUT_NONE(COP_I2L);
				CVM_ADJUST(CVM_WORDS_PER_LONG);
				CVMP_OUT_NONE(COP_PREFIX_LCMP);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG * 2) + 1);
				OutputBranch(coder, COP_BRTRUE, dest);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_F)
			{
				CVM_OUT_NONE(COP_LDC_I4_0);
				CVM_OUT_NONE(COP_I2F);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT);
				CVMP_OUT_NONE(COP_PREFIX_FCMPG);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT * 2) + 1);
				OutputBranch(coder, COP_BRTRUE, dest);
				CVM_ADJUST(-1);
			}
			else
			{
				OutputBranch(coder, COP_BRNONNULL, dest);
				CVM_ADJUST(-1);
			}
		}
		break;

		case IL_OP_BRFALSE_S:
		case IL_OP_BRFALSE:
		{
			/* Branch if the top-most stack item is false */
		#ifdef IL_NATIVE_INT32
			if(type1 == ILEngineType_I4 || type1 == ILEngineType_I)
		#else
			if(type1 == ILEngineType_I4)
		#endif
			{
				OutputBranch(coder, COP_BRFALSE, dest);
				CVM_ADJUST(-1);
			}
		#ifdef IL_NATIVE_INT64
			else if(type1 == ILEngineType_I8 || type1 == ILEngineType_I)
		#else
			else if(type1 == ILEngineType_I8)
		#endif
			{
				CVM_OUT_NONE(COP_LDC_I4_0);
				CVM_OUT_NONE(COP_I2L);
				CVM_ADJUST(CVM_WORDS_PER_LONG);
				CVMP_OUT_NONE(COP_PREFIX_LCMP);
				CVM_ADJUST(-(CVM_WORDS_PER_LONG * 2) + 1);
				OutputBranch(coder, COP_BRFALSE, dest);
				CVM_ADJUST(-1);
			}
			else if(type1 == ILEngineType_F)
			{
				CVM_OUT_NONE(COP_LDC_I4_0);
				CVM_OUT_NONE(COP_I2F);
				CVM_ADJUST(CVM_WORDS_PER_NATIVE_FLOAT);
				CVMP_OUT_NONE(COP_PREFIX_FCMPG);
				CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT * 2) + 1);
				OutputBranch(coder, COP_BRFALSE, dest);
				CVM_ADJUST(-1);
			}
			else
			{
				OutputBranch(coder, COP_BRNULL, dest);
				CVM_ADJUST(-1);
			}
		}
		break;

		case IL_OP_BEQ:
		case IL_OP_BEQ_S:
		{
			/* Equality testing branch */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondBranch(coder, COP_BEQ, COP_PREFIX_LCMP,
							 COP_PREFIX_FCMPL, COP_BEQ, type1, dest);
		}
		break;

		case IL_OP_BNE_UN:
		case IL_OP_BNE_UN_S:
		{
			/* Inequality testing branch */
			AdjustMixedBinary(coder, 1, &type1, &type2);
			OutputCondBranch(coder, COP_BNE, COP_PREFIX_LCMP,
							 COP_PREFIX_FCMPL, COP_BNE, type1, dest);
		}
		break;

		case IL_OP_BGT:
		case IL_OP_BGT_S:
		{
			/* Signed greater than testing branch */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondBranch(coder, COP_BGT, COP_PREFIX_LCMP,
							 COP_PREFIX_FCMPL, COP_BGT, type1, dest);
		}
		break;

		case IL_OP_BGT_UN:
		case IL_OP_BGT_UN_S:
		{
			/* Unsigned/unordered greater than testing branch */
			AdjustMixedBinary(coder, 1, &type1, &type2);
			OutputCondBranch(coder, COP_BGT_UN, COP_PREFIX_LCMP_UN,
							 COP_PREFIX_FCMPG, COP_BGT, type1, dest);
		}
		break;

		case IL_OP_BGE:
		case IL_OP_BGE_S:
		{
			/* Signed greater than or equal testing branch */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondBranch(coder, COP_BGE, COP_PREFIX_LCMP,
							 COP_PREFIX_FCMPL, COP_BGE, type1, dest);
		}
		break;

		case IL_OP_BGE_UN:
		case IL_OP_BGE_UN_S:
		{
			/* Unsigned/unordered greater than or equal testing branch */
			AdjustMixedBinary(coder, 1, &type1, &type2);
			OutputCondBranch(coder, COP_BGE_UN, COP_PREFIX_LCMP_UN,
							 COP_PREFIX_FCMPG, COP_BGE, type1, dest);
		}
		break;

		case IL_OP_BLT:
		case IL_OP_BLT_S:
		{
			/* Signed less than testing branch */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondBranch(coder, COP_BLT, COP_PREFIX_LCMP,
							 COP_PREFIX_FCMPG, COP_BLT, type1, dest);
		}
		break;

		case IL_OP_BLT_UN:
		case IL_OP_BLT_UN_S:
		{
			/* Unsigned/unordered less than testing branch */
			AdjustMixedBinary(coder, 1, &type1, &type2);
			OutputCondBranch(coder, COP_BLT_UN, COP_PREFIX_LCMP_UN,
							 COP_PREFIX_FCMPL, COP_BLT, type1, dest);
		}
		break;

		case IL_OP_BLE:
		case IL_OP_BLE_S:
		{
			/* Signed less than or equal testing branch */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondBranch(coder, COP_BLE, COP_PREFIX_LCMP,
							 COP_PREFIX_FCMPG, COP_BLE, type1, dest);
		}
		break;

		case IL_OP_BLE_UN:
		case IL_OP_BLE_UN_S:
		{
			/* Unsigned/unordered less than or equal testing branch */
			AdjustMixedBinary(coder, 1, &type1, &type2);
			OutputCondBranch(coder, COP_BLE_UN, COP_PREFIX_LCMP_UN,
							 COP_PREFIX_FCMPL, COP_BLE, type1, dest);
		}
		break;
	}
}

/*
 * Output the start of a table-based switch statement.
 */
static void CVMCoder_SwitchStart(ILCoder *coder, ILUInt32 numEntries)
{
	ILInt32 defCase;

	/* Record the current position so that we know the
	   relative starting point for switch entry values */
	((ILCVMCoder *)coder)->switchStart = CVM_POSN();

	/* Determine the offset of the default case */
	defCase = CVM_DEFCASE_OFFSET(numEntries);

	/* Output the head of the switch statement */
	CVM_OUT_SWHEAD(numEntries, defCase);

	/* One less value on the stack */
	CVM_ADJUST(-1);
}

/*
 * Output an entry for a table-based switch statement.
 */
static void CVMCoder_SwitchEntry(ILCoder *_coder, ILUInt32 dest)
{
	ILCVMCoder *coder = (ILCVMCoder *)_coder;
	ILCVMLabel *label;
	ILCVMLabel *ref;
	ILInt32 relative;

	/* Get the label for the destination address */
	label = GetLabel(coder, dest);
	if(!label)
	{
		return;
	}

	/* Output the offset, or register a reference for later */
	if(label->offset != ILCVM_LABEL_UNDEF)
	{
		/* Output the relative address now */
		relative = (ILInt32)(label->offset - (coder->switchStart - CVM_POSN()));
		CVM_OUT_SWENTRY(coder->switchStart, relative);
	}
	else
	{
		/* Record a reference to the label for later back-patching */
		ref = ILMemPoolAlloc(&(coder->labelPool), ILCVMLabel);
		if(ref)
		{
			ref->address = (ILUInt32)(CVM_POSN() - coder->start);
			ref->offset = coder->switchStart - coder->start;
			ref->next = 0;
			ref->nextRef = label->nextRef;
			label->nextRef = ref;
		}
		else
		{
			coder->labelOutOfMemory = 1;
		}

		/* Output a place-holder to be back-patched when we find the label */
		CVM_OUT_SWENTRY_PLACEHOLDER();
	}
}

/*
 * Output the end of a table-based switch statement.
 */
static void CVMCoder_SwitchEnd(ILCoder *coder)
{
}

/*
 * Output a conditional compare opcode.
 */
static void OutputCondCompare(ILCoder *coder, int iopcode, int lopcode,
							  int fopcode, int finvopcode,
							  int cmpopcode, int cmpinvopcode,
							  ILEngineType type, int invertTest)
{
	if(type == ILEngineType_I4)
	{
		CVMP_OUT_NONE(iopcode);
		CVM_ADJUST(-1);
	}
	else if(type == ILEngineType_I8)
	{
		CVMP_OUT_NONE(lopcode);
		CVM_ADJUST(-(CVM_WORDS_PER_LONG * 2) + 1);
	}
	else if(type == ILEngineType_F)
	{
		if(invertTest)
		{
			CVMP_OUT_NONE(finvopcode);
		}
		else
		{
			CVMP_OUT_NONE(fopcode);
		}
		CVM_ADJUST(-(CVM_WORDS_PER_NATIVE_FLOAT * 2) + 1);
	}
	else if(type == ILEngineType_M || type == ILEngineType_T ||
			type == ILEngineType_O || type == ILEngineType_I)
	{
		CVMP_OUT_NONE(COP_PREFIX_PCMP);
		CVM_ADJUST(-1);
	}
	if(invertTest)
	{
		CVMP_OUT_NONE(cmpinvopcode);
	}
	else
	{
		CVMP_OUT_NONE(cmpopcode);
	}
}

/*
 * Output a comparison instruction.
 */
static void CVMCoder_Compare(ILCoder *coder, int opcode,
				   		     ILEngineType type1, ILEngineType type2,
							 int invertTest)
{
	switch(opcode)
	{
		case IL_OP_PREFIX + IL_PREFIX_OP_CEQ:
		{
			/* Test two values for equality */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondCompare(coder, COP_PREFIX_ICMP, COP_PREFIX_LCMP,
							  COP_PREFIX_FCMPL, COP_PREFIX_FCMPL,
							  COP_PREFIX_SETEQ, COP_PREFIX_SETNE,
							  type1, invertTest);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CGT:
		{
			/* Test two signed values for greater than */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondCompare(coder, COP_PREFIX_ICMP, COP_PREFIX_LCMP,
							  COP_PREFIX_FCMPL, COP_PREFIX_FCMPG,
							  COP_PREFIX_SETGT, COP_PREFIX_SETLE,
							  type1, invertTest);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CGT_UN:
		{
			/* Test two unsigned/unordered values for greater than */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondCompare(coder, COP_PREFIX_ICMP_UN, COP_PREFIX_LCMP_UN,
							  COP_PREFIX_FCMPG, COP_PREFIX_FCMPG,
							  COP_PREFIX_SETGT, COP_PREFIX_SETLE,
							  type1, invertTest);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CLT:
		{
			/* Test two signed values for less than */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondCompare(coder, COP_PREFIX_ICMP, COP_PREFIX_LCMP,
							  COP_PREFIX_FCMPG, COP_PREFIX_FCMPL,
							  COP_PREFIX_SETLT, COP_PREFIX_SETGE,
							  type1, invertTest);
		}
		break;

		case IL_OP_PREFIX + IL_PREFIX_OP_CLT_UN:
		{
			/* Test two unsigned/unordered values for less than */
			AdjustMixedBinary(coder, 0, &type1, &type2);
			OutputCondCompare(coder, COP_PREFIX_ICMP_UN, COP_PREFIX_LCMP_UN,
							  COP_PREFIX_FCMPL, COP_PREFIX_FCMPL,
							  COP_PREFIX_SETLT, COP_PREFIX_SETGE,
							  type1, invertTest);
		}
		break;
	}
}

#endif	/* IL_CVMC_CODE */
