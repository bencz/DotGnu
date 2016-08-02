/*
 * unroll_branch.c - Branch handling for generic CVM unrolling.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#ifdef IL_UNROLL_GLOBAL

/*
 * Perform a conditional branch to one of two program locations.
 * It is assumed that "cond" refers to the inverse of the condition
 * that we are really testing.
 */
static void BranchOnCondition(MDUnroll *unroll, int cond,
						      unsigned char *truePC,
							  unsigned char *falsePC)
{
	md_inst_ptr patch;

	/* Flush the registers and restore special values.  Because this only
	   uses "mov" and "pop" operations, it will not affect the flags */
	FlushRegisterStack(unroll);
	RestoreSpecialRegisters(unroll);
	unroll->regsSaved = 0;

	/* Test the condition in such a way that we branch if false */
	patch = unroll->out;
	md_branch_cc(unroll->out, cond);

	/* Output the jump to the true PC */
	FixStackHeight(unroll);
	UnloadMachineState(unroll, truePC, 0, 0);

	/* Back-patch the branch instruction to point here */
	md_patch(patch, unroll->out);

	/* Output the jump to the false PC */
	FixStackHeight(unroll);
	unroll->stackHeight = 0;
	UnloadMachineState(unroll, falsePC, 0, 0);
}

#elif defined(IL_UNROLL_CASES)

case COP_BR:
{
	/* Branch unconditionally to a destination */
	UNROLL_BRANCH_START();
	BranchToPC(&unroll, CVM_ARG_BRANCH_SHORT);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BEQ:
{
	/* Branch if two words are equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_NE, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_NE,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BR_PEQ:
{
	/* Branch if two native words are equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_cmp_cc_reg_reg_word_native(unroll.out, MD_CC_NE, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_NE,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BNE:
{
	/* Branch if two words are not equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_EQ, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_EQ,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BR_PNE:
{
	/* Branch if two native words are not equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_cmp_cc_reg_reg_word_native(unroll.out, MD_CC_EQ, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_EQ,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BLT:
{
	/* Branch if less than */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_GE, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_GE,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BLT_UN:
{
	/* Branch if unsigned less than */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_GE_UN, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_GE_UN,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BLE:
{
	/* Branch if less than or equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_GT, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_GT,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BLE_UN:
{
	/* Branch if unsigned less than or equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_GT_UN, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_GT_UN,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BGT:
{
	/* Branch if greater than */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_LE, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_LE,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BGT_UN:
{
	/* Branch if unsigned greater than */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_LE_UN, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_LE_UN,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BGE:
{
	/* Branch if greater than or equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_LT, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_LT,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BGE_UN:
{
	/* Branch if unsigned greater than or equal */
	UNROLL_BRANCH_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_cc_reg_reg_word_32(unroll.out, MD_CC_LT_UN, reg, reg2);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_LT_UN,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BRTRUE:
{
	/* Branch if non-zero */
	UNROLL_BRANCH_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_reg_is_zero(unroll.out, reg);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_EQ,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BRNONNULL:
{
	/* Branch if non-null */
	UNROLL_BRANCH_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_reg_is_null(unroll.out, reg);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_EQ,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BRFALSE:
{
	/* Branch if zero */
	UNROLL_BRANCH_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_reg_is_zero(unroll.out, reg);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_NE,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_BRNULL:
{
	/* Branch if null */
	UNROLL_BRANCH_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_reg_is_null(unroll.out, reg);
	FreeTopRegister(&unroll, -1);
	BranchOnCondition(&unroll, MD_CC_NE,
					  CVM_ARG_BRANCH_SHORT, pc + CVM_LEN_BRANCH);
	MODIFY_UNROLL_PC(CVM_LEN_BRANCH);
	UNROLL_FLUSH();
}
break;

case COP_SWITCH:
{
	/* Switch statement */
	md_inst_ptr patch;
	UNROLL_BRANCH_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	FreeTopRegister(&unroll, -1);
	FlushRegisterStack(&unroll);
	if((MD_REGS_TO_BE_SAVED & (1 << reg)) != 0)
	{
		/* Shift the value into MD_REG_0 so that it won't
		   get clobbered by "RestoreSpecialRegisters" below */
		md_mov_reg_reg(unroll.out, MD_REG_0, reg);
		reg = MD_REG_0;
	}
	md_cmp_reg_imm_word_32(unroll.out, MD_CC_GE_UN, reg, CVM_ARG_SWITCH_LIMIT);
	patch = unroll.out;
	md_branch_ge_un(unroll.out);
	FixStackHeight(&unroll);
	RestoreSpecialRegisters(&unroll);
	md_switch(unroll.out, reg, pc + 3 * sizeof(void *));
	md_patch(patch, unroll.out);
	BranchToPC(&unroll, CVM_ARG_SWITCH_DEFAULT);
	pc = CVM_ARG_SWITCH_DEFAULT;
	UNROLL_FLUSH();
}
break;

#endif /* IL_UNROLL_CASES */
