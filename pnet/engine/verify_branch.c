/*
 * verify_branch.c - Verify instructions related to branching.
 *
 * Copyright (C) 2001, 2008  Southern Storm Software, Pty Ltd.
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
 * Type inference matrix for binary comparison operations.
 */
static char const binaryCompareMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_NO, T_NO, T_M,  T_NO, T_T,  T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_NO, T_NO, T_T,  T_NO, T_T,  T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for binary equality operations.
 */
static char const binaryEqualityMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_NO, T_NO, T_M,  T_NO, T_T,  T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_O,  T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_NO, T_NO, T_T,  T_NO, T_T,  T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for binary comparison operations,
 * when unsafe pointer comparisons are allowed.
 */
static char const unsafeCompareMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_M,  T_NO, T_T,  T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_M,  T_NO, T_M,  T_NO, T_T,  T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_T,  T_NO, T_T,  T_NO, T_T,  T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for binary equality operations,
 * when unsafe pointer comparisons are allowed.
 */
static char const unsafeEqualityMatrix
			[ILEngineType_ValidTypes][ILEngineType_ValidTypes] =
{
		    /* I4    I8    I     F     &     O     *     MV */
	/* I4: */ {T_I4, T_NO, T_I,  T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I8: */ {T_NO, T_I8, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
	/* I:  */ {T_I,  T_NO, T_I,  T_NO, T_M,  T_NO, T_T,  T_NO},
	/* F:  */ {T_NO, T_NO, T_NO, T_F,  T_NO, T_NO, T_NO, T_NO},
	/* &:  */ {T_NO, T_NO, T_M,  T_NO, T_M,  T_NO, T_T,  T_NO},
	/* O:  */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_O,  T_NO, T_NO},
	/* *:  */ {T_NO, T_NO, T_T,  T_NO, T_T,  T_NO, T_T,  T_NO},
	/* MV: */ {T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO, T_NO},
};

/*
 * Type inference matrix for unary branch operators.
 */
static char const unaryBranchMatrix[ILEngineType_ValidTypes] =
{
    /* I4    I8    I     F     &     O     *    MV */
     T_I4, T_I8, T_I,  T_F,  T_M,  T_O,  T_T, T_NO,
};

/*
 * Information that is stored for a label.
 */
typedef struct _tagBranchLabel BranchLabel;
struct _tagBranchLabel
{
	ILUInt32	 address;
	BranchLabel *next;
	ILUInt32	 stackSize;

};

/*
 * Get the destination for a short or long branch instruction.
 */
#define	GET_SHORT_DEST()	(offset + 2 + ((ILUInt32)(ILInt32)(ILInt8)(pc[1])))
#define	GET_LONG_DEST()		(offset + 5 + ((ILUInt32)IL_READ_INT32(pc + 1)))

/*
 * Find an existing branch label information block.
 */
static BranchLabel *FindLabel(BranchLabel *labelList, ILUInt32 address)
{
	while(labelList != 0)
	{
		if(labelList->address == address)
		{
			return labelList;
		}
		labelList = labelList->next;
	}
	return 0;
}

/* 
 * Find the nearest type in the type2 heirarchy that is assign 
 * compatible to type1.
 */
static ILType* TryCommonType(ILImage* image, ILType * type1, ILType *type2)
{
	ILClass *classInfo;

	if(type1==NULL) return type2;
	if(type2==NULL) return type1;

	/* Note: boxing conversions are not allowed because both
	 * types have to be reference types.
	 */ 
	if(ILTypeAssignCompatibleNonBoxing(image, type1,type2))
	{
		return type2;
	}

	classInfo=ILClassFromType(image, 0, type2, 0);
	
	if((classInfo = ILClass_ParentClass(classInfo)) != NULL)
	{
		return TryCommonType(image, type1, ILClassToType(classInfo));
	}
	
	return type2;
}

/* 
 * Obtain a common parent for 2 types 
 * TODO: handle interfaces
 */
static int CommonType(ILImage * image, ILType *type1, ILType *type2,
						ILType** commonType)
{
	ILType *ctype12=TryCommonType(image, type1, type2);
	ILType *ctype21=TryCommonType(image, type2, type1);
	
	if(ILTypeAssignCompatibleNonBoxing(image, ctype12, ctype21))
	{
		(*commonType) = ctype12;
		return 1;
	}
	else if(ILTypeAssignCompatibleNonBoxing(image, ctype21, ctype12))
	{
		(*commonType) = ctype21;
		return 1;
	}
	return 0;
}

/*
 * Validate the current contents of the stack against
 * information that was previously recorded.
 */
static int ValidateStack(ILImage *image, BranchLabel *label,
						 ILEngineStackItem *stack, ILUInt32 stackSize)
{
	ILUInt32 posn;
	ILEngineStackItem *labelStack;
	ILType *commonType;
	if(stackSize != label->stackSize)
	{
		return 0;
	}
	labelStack = (ILEngineStackItem *)(label + 1);
	for(posn = 0; posn < stackSize; ++posn)
	{
#ifdef IL_NATIVE_INT32
		if(stack[posn].engineType == ILEngineType_I4 ||
			stack[posn].engineType == ILEngineType_I)
		{
			if(labelStack[posn].engineType == ILEngineType_I4 ||
				labelStack[posn].engineType == ILEngineType_I)
			{
				continue;
			}
		}
#else
	#ifdef IL_NATIVE_INT64
		if(stack[posn].engineType == ILEngineType_I8 ||
			stack[posn].engineType == ILEngineType_I)
		{
			if(labelStack[posn].engineType == ILEngineType_I8 ||
				labelStack[posn].engineType == ILEngineType_I)
			{
				continue;
			}
		}
	#endif
#endif
		if(stack[posn].engineType != labelStack[posn].engineType)
		{
			return 0;
		}
		if(stack[posn].engineType == ILEngineType_M ||
		   stack[posn].engineType == ILEngineType_T ||
		   stack[posn].engineType == ILEngineType_MV)
		{
			if(!ILTypeIdentical(stack[posn].typeInfo,
								labelStack[posn].typeInfo))
			{
				return 0;
			}
		}
		else if(stack[posn].engineType == ILEngineType_O)
		{
			if(!CommonType(image, stack[posn].typeInfo,
						labelStack[posn].typeInfo, &commonType))
			{
				return 0;
			}
			labelStack[posn].typeInfo = commonType;
			stack[posn].typeInfo = commonType;
		}
	}
	return 1;
}

/*
 * Copy the current contents of the stack to a new label block.
 * Returns NULL if out of memory.
 */
static BranchLabel *CopyStack(TempAllocator *allocator, ILUInt32 address,
							  ILEngineStackItem *stack, ILUInt32 stackSize)
{
	BranchLabel *label;

	/* Allocate space for the label */
	label = (BranchLabel *)TempAllocate
				(allocator, sizeof(BranchLabel) +
							sizeof(ILEngineStackItem) * stackSize);
	if(!label)
	{
		return 0;
	}

	/* Copy the address and stack information to the label */
	label->address = address;
	label->stackSize = stackSize;
	if(stackSize > 0)
	{
		ILMemCpy(label + 1, stack, stackSize * sizeof(ILEngineStackItem));
	}

	/* The label has been initialized */
	return label;
}

/*
 * Copy exception information to a new label block.
 * Returns NULL if out of memory.
 */
static BranchLabel *CopyExceptionStack(TempAllocator *allocator,
									   ILUInt32 address, ILClass *classInfo,
									   ILUInt32 stackSize)
{
	BranchLabel *label;

	/* Allocate space for the label */
	label = (BranchLabel *)TempAllocate
				(allocator, sizeof(BranchLabel) +
							sizeof(ILEngineStackItem) * stackSize);
	if(!label)
	{
		return 0;
	}

	/* Copy the address and stack information to the label */
	label->address = address;
	label->stackSize = stackSize;
	if(stackSize > 0)
	{
		((ILEngineStackItem *)(label + 1))->engineType = ILEngineType_O;
		((ILEngineStackItem *)(label + 1))->typeInfo =
				ILType_FromClass(classInfo);
	}

	/* The label has been initialized */
	return label;
}

/*
 * Reload the contents of the stack from a pre-existing label.
 * Returns the new stack size.
 */
static ILUInt32 ReloadStack(BranchLabel *label, ILEngineStackItem *stack)
{
	if(label->stackSize > 0)
	{
		ILMemCpy(stack, label + 1,
				 label->stackSize * sizeof(ILEngineStackItem));
	}
	return label->stackSize;
}

/*
 * Validate or record the stack information for a destination label.
 */
#define	VALIDATE_BRANCH_STACK(dest)	\
			do { \
				currLabel = FindLabel(labelList, (dest)); \
				if(currLabel) \
				{ \
					if(!ValidateStack(ILProgramItem_Image(method), \
									  currLabel, stack, stackSize)) \
					{ \
						VERIFY_STACK_ERROR(); \
					} \
				} \
				else \
				{ \
					currLabel = CopyStack(&allocator, (dest), \
										  stack, stackSize); \
					if(!currLabel) \
					{ \
						VERIFY_MEMORY_ERROR(); \
					} \
					currLabel->next = labelList; \
					labelList = currLabel; \
				} \
			} while (0)

/*
 * Validate or record the stack information for a jump target.
 */
#define	VALIDATE_TARGET_STACK(address) \
			do { \
				currLabel = FindLabel(labelList, (address)); \
				if(currLabel) \
				{ \
					if(lastWasJump) \
					{ \
						/* Reload the stack contents from the label */ \
						stackSize = ReloadStack(currLabel, stack); \
					} \
					else if(!ValidateStack(ILProgramItem_Image(method), \
									       currLabel, stack, stackSize)) \
					{ \
						VERIFY_STACK_ERROR(); \
					} \
				} \
				else \
				{ \
					if(lastWasJump) \
					{ \
						/* This is probably the head of a while loop */ \
						/* which is assumed to always be zero-sized. */ \
						stackSize = 0; \
					} \
					currLabel = CopyStack(&allocator, (address), \
										  stack, stackSize); \
					if(!currLabel) \
					{ \
						VERIFY_MEMORY_ERROR(); \
					} \
					currLabel->next = labelList; \
					labelList = currLabel; \
				} \
			} while (0)

/*
 * Set the contents of the stack at a particular point
 * in the method to a given exception object type.
 */
#define	SET_TARGET_STACK(address,classInfo)	\
			do { \
				currLabel = FindLabel(labelList, (address)); \
				if(currLabel) \
				{ \
					/* Check the stack contents for equality */ \
					if(currLabel->stackSize != 1 || \
					   ((ILEngineStackItem *)(currLabel + 1))->engineType \
					   		!= ILEngineType_O || \
					   ((ILEngineStackItem *)(currLabel + 1))->typeInfo \
					   		!= ILType_FromClass((classInfo))) \
					{ \
						VERIFY_STACK_ERROR(); \
					} \
				} \
				else \
				{ \
					currLabel = CopyExceptionStack(&allocator, (address), \
										  		   (classInfo), 1); \
					if(!currLabel) \
					{ \
						VERIFY_MEMORY_ERROR(); \
					} \
					currLabel->next = labelList; \
					labelList = currLabel; \
				} \
			} while (0)

/*
 * Set the contents of the stack at a particular point
 * in the method to empty.
 */
#define	SET_TARGET_STACK_EMPTY(address)	\
			do { \
				currLabel = FindLabel(labelList, (address)); \
				if(currLabel) \
				{ \
					/* Check the stack contents for equality */ \
					if(currLabel->stackSize != 0) \
					{ \
						VERIFY_STACK_ERROR(); \
					} \
				} \
				else \
				{ \
					currLabel = CopyExceptionStack(&allocator, (address), \
										  		   0, 0); \
					if(!currLabel) \
					{ \
						VERIFY_MEMORY_ERROR(); \
					} \
					currLabel->next = labelList; \
					labelList = currLabel; \
				} \
			} while (0)

#elif defined(IL_VERIFY_LOCALS)

ILUInt32 dest;
ILUInt32 numEntries;
BranchLabel *labelList;
BranchLabel *currLabel;

#else /* IL_VERIFY_CODE */

case IL_OP_BR_S:
{
	/* Unconditional short branch */
	dest = GET_SHORT_DEST();
	ILCoderBranch(coder, opcode, dest, ILEngineType_I4, ILEngineType_I4);
	VALIDATE_BRANCH_STACK(dest);
	lastWasJump = 1;
}
break;

case IL_OP_BRFALSE_S:
case IL_OP_BRTRUE_S:
{
	/* Unary conditional short branch */
  	dest = GET_SHORT_DEST();
unaryBranch:
	commonType = unaryBranchMatrix[STK_UNARY];
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderBranch(coder, opcode, dest, commonType, commonType);
		--stackSize;
		VALIDATE_BRANCH_STACK(dest);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_BEQ_S:
case IL_OP_BNE_UN_S:
{
	/* Binary equality short branch */
	dest = GET_SHORT_DEST();
binaryEquality:
	if(unsafeAllowed)
	{
		commonType = unsafeEqualityMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	else
	{
		commonType = binaryEqualityMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderBranch(coder, opcode, dest, STK_BINARY_1, STK_BINARY_2);
		stackSize -= 2;
		VALIDATE_BRANCH_STACK(dest);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_BGT_UN_S:
{
	if(STK_BINARY_1 == ILEngineType_O && STK_BINARY_2 == ILEngineType_O)
	{
		/* "bgt.un" can be used on object references */
		dest = GET_SHORT_DEST();
		ILCoderBranch(coder, opcode, dest, STK_BINARY_1, STK_BINARY_2);
		stackSize -= 2;
		VALIDATE_BRANCH_STACK(dest);
		break;
	}
}
/* Fall through */

case IL_OP_BGE_S:
case IL_OP_BGT_S:
case IL_OP_BLE_S:
case IL_OP_BLT_S:
case IL_OP_BGE_UN_S:
case IL_OP_BLE_UN_S:
case IL_OP_BLT_UN_S:
{
	/* Binary conditional short branch */
	dest = GET_SHORT_DEST();
binaryBranch:
	if(unsafeAllowed)
	{
		commonType = unsafeCompareMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	else
	{
		commonType = binaryCompareMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	if(commonType != ILEngineType_Invalid)
	{
		ILCoderBranch(coder, opcode, dest, STK_BINARY_1, STK_BINARY_2);
		stackSize -= 2;
		VALIDATE_BRANCH_STACK(dest);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_BR:
{
	/* Unconditional long branch */
	dest = GET_LONG_DEST();
	ILCoderBranch(coder, opcode, dest, ILEngineType_I4, ILEngineType_I4);
	VALIDATE_BRANCH_STACK(dest);
	lastWasJump = 1;
}
break;

case IL_OP_BRFALSE:
case IL_OP_BRTRUE:
{
	/* Unary conditional long branch */
	dest = GET_LONG_DEST();
	goto unaryBranch;
}
/* Not reached */

case IL_OP_BEQ:
case IL_OP_BNE_UN:
{
	/* Binary equality long branch */
	dest = GET_LONG_DEST();
	goto binaryEquality;
}
/* Not reached */

case IL_OP_BGT_UN:
{
	if(STK_BINARY_1 == ILEngineType_O && STK_BINARY_2 == ILEngineType_O)
	{
		/* "bgt.un" can be used on object references */
		dest = GET_LONG_DEST();
		ILCoderBranch(coder, opcode, dest, STK_BINARY_1, STK_BINARY_2);
		stackSize -= 2;
		VALIDATE_BRANCH_STACK(dest);
		break;
	}
}
/* Fall through */

case IL_OP_BGE:
case IL_OP_BGT:
case IL_OP_BLE:
case IL_OP_BLT:
case IL_OP_BGE_UN:
case IL_OP_BLE_UN:
case IL_OP_BLT_UN:
{
	/* Binary conditional long branch */
	dest = GET_LONG_DEST();
	goto binaryBranch;
}
/* Not reached */

case IL_OP_SWITCH:
{
	/* Switch statement */
	if(STK_UNARY == ILEngineType_I4)
	{
		--stackSize;
		numEntries = IL_READ_UINT32(pc + 1);
		insnSize = 5 + numEntries * 4;
		ILCoderSwitchStart(coder, numEntries);
		for(argNum = 0; argNum < numEntries; ++argNum)
		{
			dest = (ILUInt32)((pc + insnSize) - (unsigned char *)(code->code)) +
				   (ILUInt32)(IL_READ_INT32(pc + 5 + argNum * 4));
			ILCoderSwitchEntry(coder, dest);
			VALIDATE_BRANCH_STACK(dest);
		}
		ILCoderSwitchEnd(coder);
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;


case IL_OP_PREFIX + IL_PREFIX_OP_CEQ:
{
	/* Binary equality comparison */
	if(unsafeAllowed)
	{
		commonType = unsafeEqualityMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	else
	{
		commonType = binaryEqualityMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	if(commonType != ILEngineType_Invalid)
	{
		/*
		 * Note: Look for logical not and give the coder the
		 * opportunity to optimize it away by inverting the test.
		 * First look for "ldc.i4.1;xor" because this is what's
		 * generated by cscc.
		 */
		if(pc[2] == IL_OP_LDC_I4_1 && pc[3] == IL_OP_XOR &&
		   !IsJumpTarget(jumpMask, offset + 2) &&
		   !IsJumpTarget(jumpMask, offset + 3))
		{
			ILCoderCompare(coder, opcode, STK_BINARY_1, STK_BINARY_2, 1);
			insnSize = 4;
		}
		/*
		 * Then look for "ldc.i4.0;ceq" because other compilers tend
		 * to generate this sequence for a logical not.
		 */
		else if(pc[2] == IL_OP_LDC_I4_0 && pc[3] == IL_OP_PREFIX &&
		   pc[4] == IL_PREFIX_OP_CEQ &&
		   !IsJumpTarget(jumpMask, offset + 2) &&
		   !IsJumpTarget(jumpMask, offset + 3))
		{
			ILCoderCompare(coder, opcode, STK_BINARY_1, STK_BINARY_2, 1);
			insnSize = 5;
		}
		else
		{
			ILCoderCompare(coder, opcode, STK_BINARY_1, STK_BINARY_2, 0);
		}
		STK_BINARY_1 = ILEngineType_I4;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_CGT:
case IL_OP_PREFIX + IL_PREFIX_OP_CGT_UN:
case IL_OP_PREFIX + IL_PREFIX_OP_CLT:
case IL_OP_PREFIX + IL_PREFIX_OP_CLT_UN:
{
	/* Binary conditional comparison */
	if(opcode == IL_OP_PREFIX + IL_PREFIX_OP_CGT_UN &&
	   STK_BINARY_1 == ILEngineType_O &&
	   STK_BINARY_2 == ILEngineType_O)
	{
		/* "cgt.un" is allowed on object references */
		commonType = ILEngineType_O;
	}
	else
	{
		commonType = binaryCompareMatrix[STK_BINARY_1][STK_BINARY_2];
	}
	if(commonType != ILEngineType_Invalid)
	{
		/*
		 * Note: there is an inconsistency when these comparison
		 * codes are used with floating point values and are
		 * followed by a "NOT" to invert the result of the test.
		 * The test for "not a number" will be inconsistent with
		 * the test performed by the branch opcodes.  e.g. "clt;not"
		 * will produce a different behaviour to "bge".  This is a
		 * deficiency in the IL instruction set itself.  We attempt
		 * to correct this problem by looking for "ldc.i4.1;xor"
		 * just after the comparison.  We pass this information to
		 * the coder so that it can process "not a number" correctly.
		 * This isn't foolproof, but should catch most cases.
		 */
		if(pc[2] == IL_OP_LDC_I4_1 && pc[3] == IL_OP_XOR &&
		   !IsJumpTarget(jumpMask, offset + 2) &&
		   !IsJumpTarget(jumpMask, offset + 3))
		{
			ILCoderCompare(coder, opcode, STK_BINARY_1, STK_BINARY_2, 1);
			insnSize = 4;
		}
		/*
		 * Then look for "ldc.i4.0;ceq" because other compilers tend
		 * to generate this sequence for a logical not.
		 */
		else if(pc[2] == IL_OP_LDC_I4_0 && pc[3] == IL_OP_PREFIX &&
		   pc[4] == IL_PREFIX_OP_CEQ &&
		   !IsJumpTarget(jumpMask, offset + 2) &&
		   !IsJumpTarget(jumpMask, offset + 3))
		{
			ILCoderCompare(coder, opcode, STK_BINARY_1, STK_BINARY_2, 1);
			insnSize = 5;
		}
		else
		{
			ILCoderCompare(coder, opcode, STK_BINARY_1, STK_BINARY_2, 0);
		}
		STK_BINARY_1 = ILEngineType_I4;
		STK_TYPEINFO_1 = 0;
		--stackSize;
	}
	else
	{
		VERIFY_TYPE_ERROR();
	}
}
break;

#endif /* IL_VERIFY_CODE */
