/*
 * unroll_arith.c - Arithmetic handling for generic CVM unrolling.
 *
 * Copyright (C) 2003, 2010  Southern Storm Software, Pty Ltd.
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

#ifdef CVM_X86

/*
 * Perform an integer division or remainder.
 */
static void Divide(MDUnroll *unroll, int isSigned, int wantRemainder,
				      unsigned char *pc, unsigned char *label)
{
#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	#define IL_NEED_DIVIDE_REEXECUTE 1
	unsigned char *patch1;
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	#define IL_NEED_DIVIDE_REEXECUTE 1
	unsigned char  *patch2, *patch3;
#endif

	/* Get the arguments into EAX and ECX so we know where they are */
	if(unroll->pseudoStackSize != 2 ||
	   unroll->pseudoStack[0] != X86_EAX ||
	   unroll->pseudoStack[1] != X86_ECX)
	{
		FlushRegisterStack(unroll);
		unroll->stackHeight -= 8;
		x86_mov_reg_membase(unroll->out, X86_EAX, MD_REG_STACK,
							unroll->stackHeight, 4);
		x86_mov_reg_membase(unroll->out, X86_ECX, MD_REG_STACK,
							unroll->stackHeight + 4, 4);
		unroll->pseudoStack[0] = X86_EAX;
		unroll->pseudoStack[1] = X86_ECX;
		unroll->pseudoStackSize = 2;
		unroll->regsUsed |= ((1 << X86_EAX) | (1 << X86_ECX));
	}

	/* Check for conditions that may cause an exception */
#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	x86_alu_reg_imm(unroll->out, X86_CMP, X86_ECX, 0);
	patch1 = unroll->out;
	x86_branch8(unroll->out, X86_CC_EQ, 0, 0);
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	x86_alu_reg_imm(unroll->out, X86_CMP, X86_ECX, -1);
	patch2 = unroll->out;
	x86_branch32(unroll->out, X86_CC_NE, 0, 0);

	x86_alu_reg_imm(unroll->out, X86_CMP, X86_EAX, (int)0x80000000);
	patch3 = unroll->out;
	x86_branch32(unroll->out, X86_CC_NE, 0, 0);
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	x86_patch(patch1, unroll->out);
#endif

#if defined(IL_NEED_DIVIDE_REEXECUTE)
	/* Re-execute the division instruction to throw the exception */
	ReExecute(unroll, pc, label);
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	x86_patch(patch2, unroll->out);
	x86_patch(patch3, unroll->out);
#endif

	/* Perform the division */
	if(isSigned)
	{
		x86_cdq(unroll->out);
	}
	else
	{
		x86_clear_reg(unroll->out, X86_EDX);
	}
	x86_div_reg(unroll->out, X86_ECX, isSigned);

	/* Pop ECX from the pseudo stack */
	FreeTopRegister(unroll, -1);

	/* If we want the remainder, then replace EAX with EDX on the stack */
	if(wantRemainder)
	{
		unroll->pseudoStack[0] = X86_EDX;
		unroll->regsUsed = (1 << X86_EDX);
	}
}

#endif /* CVM_X86 */

#ifdef CVM_X86_64

/*
 * Perform an integer division or remainder (basically the x86 
 * version modded for 8-byte words).
 */
static void Divide(MDUnroll *unroll, int isSigned, int wantRemainder,
				      unsigned char *pc, unsigned char *label)
{
#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	#define IL_NEED_DIVIDE_REEXECUTE 1
	unsigned char *patch1;
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	#define IL_NEED_DIVIDE_REEXECUTE 1
	unsigned char  *patch2, *patch3;
#endif

	/* Get the arguments into EAX and ECX so we know where they are */
	if(unroll->pseudoStackSize != 2 ||
	   unroll->pseudoStack[0] != AMD64_RAX ||
	   unroll->pseudoStack[1] != AMD64_RCX)
	{
		FlushRegisterStack(unroll);
		unroll->stackHeight -= 2 * sizeof(CVMWord)  ;
		amd64_mov_reg_membase(unroll->out, AMD64_RAX, MD_REG_STACK,
							unroll->stackHeight, 4);
		amd64_mov_reg_membase(unroll->out, AMD64_RCX, MD_REG_STACK,
							unroll->stackHeight + sizeof(CVMWord), 4);
		unroll->pseudoStack[0] = AMD64_RAX;
		unroll->pseudoStack[1] = AMD64_RCX;
		unroll->pseudoStackSize = 2;
		unroll->regsUsed |= ((1 << AMD64_RAX) | (1 << AMD64_RCX));
	}

	/* Check for conditions that may cause an exception */
#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	amd64_alu_reg_imm_size(unroll->out, X86_CMP, AMD64_RCX, 0, 4);
	patch1 = unroll->out;
	amd64_branch8(unroll->out, X86_CC_EQ, 0, 0);
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	amd64_alu_reg_imm_size(unroll->out, X86_CMP, AMD64_RCX, -1, 4);
	patch2 = unroll->out;
	amd64_branch32(unroll->out, X86_CC_NE, 0, 0);

	amd64_alu_reg_imm_size(unroll->out, X86_CMP, AMD64_RAX, (int)0x80000000, 4);
	patch3 = unroll->out;
	amd64_branch32(unroll->out, X86_CC_NE, 0, 0);
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	amd64_patch(patch1, unroll->out);
#endif

#if defined(IL_NEED_DIVIDE_REEXECUTE)
	/* Re-execute the division instruction to throw the exception */
	ReExecute(unroll, pc, label);
#endif

#if !defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
	amd64_patch(patch2, unroll->out);
	amd64_patch(patch3, unroll->out);
#endif

	/* Perform the division */
	if(isSigned)
	{
		amd64_cdq_size(unroll->out, 4);
	}
	else
	{
		amd64_clear_reg_size(unroll->out, AMD64_RDX, 4);
	}
	amd64_div_reg_size(unroll->out, AMD64_RCX, isSigned, 4);

	/* Pop ECX from the pseudo stack */
	FreeTopRegister(unroll, -1);

	/* If we want the remainder, then replace EAX with EDX on the stack */
	if(wantRemainder)
	{
		unroll->pseudoStack[0] = AMD64_RDX;
		unroll->regsUsed = (1 << AMD64_RDX);
	}
}

#endif /* CVM_X86_64 */

#if defined(CVM_PPC) && (MD_HAS_INT_DIVISION == 1)
static void Divide(MDUnroll *unroll, int isSigned, int wantRemainder,
				      unsigned char *pc, unsigned char *label)
{
#if !defined(IL_USE_INTERRUPT_BASED_INT_DIVIDE_BY_ZERO_CHECKS)
	#define IL_NEED_DIVIDE_REEXECUTE 1
#endif
	int reg, reg2;	
#ifdef IL_NEED_DIVIDE_REEXECUTE
	md_inst_ptr patch1 = NULL;
	md_inst_ptr patch2 = NULL;
	md_inst_ptr patch3 = NULL;
#endif
	GetTopTwoWordRegisters(unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);

#ifdef IL_NEED_DIVIDE_REEXECUTE
	md_reg_is_zero(unroll->out, reg2);
	patch1 = unroll->out;
	md_branch_eq(unroll->out);
	if(isSigned)
	{
		md_cmp_reg_imm_word_32(unroll->out, MD_CC_EQ, reg2, -1);
		patch2 = unroll->out;
		md_branch_ne(unroll->out);
		md_cmp_reg_imm_word_32(unroll->out, MD_CC_EQ, reg, IL_MIN_INT32);
		patch3 = unroll->out;
		md_branch_ne(unroll->out);
	}
	md_patch(patch1, unroll->out);	
	ReExecute(unroll, pc, label);
	if(isSigned)
	{
		md_patch(patch2, unroll->out);	
		md_patch(patch3, unroll->out);	
	}
#endif

	if(wantRemainder)
	{
		if(isSigned)
		{
			md_rem_reg_reg_word_32(unroll->out, reg, reg2);
		}
		else
		{
			md_urem_reg_reg_word_32(unroll->out, reg, reg2);
		}
	}
	else
	{
		if(isSigned)
		{
			md_div_reg_reg_word_32(unroll->out, reg, reg2);
		}
		else
		{
			md_udiv_reg_reg_word_32(unroll->out, reg, reg2);
		}
	}
	FreeTopRegister(unroll, -1);
}

#endif /* CVM_PPC */

#elif defined(IL_UNROLL_CASES)

case COP_IADD:
{
	/* Add integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_add_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISUB:
{
	/* Subtract integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_sub_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IMUL:
{
	/* Multiply integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_mul_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#if (MD_HAS_INT_DIVISION == 1)

case COP_IDIV:
{
	/* Divide integers */
	UNROLL_START();
	Divide(&unroll, 1, 0, pc, (unsigned char *)inst);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IDIV_UN:
{
	/* Divide unsigned integers */
	UNROLL_START();
	Divide(&unroll, 0, 0, pc, (unsigned char *)inst);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IREM:
{
	/* Remainder integers */
	UNROLL_START();
	Divide(&unroll, 1, 1, pc, (unsigned char *)inst);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IREM_UN:
{
	/* Remainder unsigned integers */
	UNROLL_START();
	Divide(&unroll, 0, 1, pc, (unsigned char *)inst);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* MD_HAS_INT_DIVISION */

case COP_INEG:
{
	/* Negate integer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_neg_reg_word_32(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#if !defined(CVM_PPC) /* has 8 byte CVMWords */

case COP_LADD:
{
	/* Add 64-bit integers */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopFourWordRegisters(&unroll, &reg, &reg2, &reg3, &reg4,
						    MD_REG1_32BIT | MD_REG2_32BIT |
						    MD_REG3_32BIT | MD_REG4_32BIT);
#if MD_LITTLE_ENDIAN_LONGS
	md_add_reg_reg_word_64(unroll.out, reg, reg2, reg3, reg4);
#else
	md_add_reg_reg_word_64(unroll.out, reg2, reg, reg4, reg3);
#endif
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
#else
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_add_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_LSUB:
{
	/* Subtract 64-bit integers */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopFourWordRegisters(&unroll, &reg, &reg2, &reg3, &reg4,
						    MD_REG1_32BIT | MD_REG2_32BIT |
						    MD_REG3_32BIT | MD_REG4_32BIT);
#if MD_LITTLE_ENDIAN_LONGS
	md_sub_reg_reg_word_64(unroll.out, reg, reg2, reg3, reg4);
#else
	md_sub_reg_reg_word_64(unroll.out, reg2, reg, reg4, reg3);
#endif
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
#else
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_sub_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_LAND:
{
	/* Bitwise AND 64-bit integers */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopFourWordRegisters(&unroll, &reg, &reg2, &reg3, &reg4,
						    MD_REG1_32BIT | MD_REG2_32BIT |
						    MD_REG3_32BIT | MD_REG4_32BIT);
	md_and_reg_reg_word_32(unroll.out, reg, reg3);
	md_and_reg_reg_word_32(unroll.out, reg2, reg4);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
#else
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_and_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_LOR:
{
	/* Bitwise OR 64-bit integers */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopFourWordRegisters(&unroll, &reg, &reg2, &reg3, &reg4,
						    MD_REG1_32BIT | MD_REG2_32BIT |
						    MD_REG3_32BIT | MD_REG4_32BIT);
	md_or_reg_reg_word_32(unroll.out, reg, reg3);
	md_or_reg_reg_word_32(unroll.out, reg2, reg4);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
#else
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_or_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_LXOR:
{
	/* Bitwise XOR 64-bit integers */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopFourWordRegisters(&unroll, &reg, &reg2, &reg3, &reg4,
						    MD_REG1_32BIT | MD_REG2_32BIT |
						    MD_REG3_32BIT | MD_REG4_32BIT);
	md_xor_reg_reg_word_32(unroll.out, reg, reg3);
	md_xor_reg_reg_word_32(unroll.out, reg2, reg4);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
#else
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_xor_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_LNEG:
{
	/* Negate a 64-bit integer */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
#if MD_LITTLE_ENDIAN_LONGS
	md_neg_reg_word_64(unroll.out, reg, reg2);
#else
	md_neg_reg_word_64(unroll.out, reg2, reg);
#endif
#else
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_neg_reg_word_native(unroll.out, reg);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_LNOT:
{
	/* Bitwise NOT a 64-bit integer */
	UNROLL_START();
#ifdef IL_NATIVE_INT32
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	md_not_reg_word_32(unroll.out, reg);
	md_not_reg_word_32(unroll.out, reg2);
#else
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_not_reg_word_native(unroll.out, reg);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif

#ifdef MD_HAS_FP

case COP_FADD:
{
	/* Add floating point */
	UNROLL_START();
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 0);
	md_add_reg_reg_float(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_FSUB:
{
	/* Subtract floating point */
	UNROLL_START();
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 0);
	md_sub_reg_reg_float(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_FMUL:
{
	/* Multiply floating point */
	UNROLL_START();
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 0);
	md_mul_reg_reg_float(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_FDIV:
{
	/* Divide floating point */
	UNROLL_START();
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 0);
	md_div_reg_reg_float(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef md_rem_reg_reg_float

case COP_FREM:
{
	/* Remainder floating point */
	UNROLL_START();
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 1);
	md_rem_reg_reg_float(unroll.out, reg, reg2, unroll.regsUsed);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_rem_reg_reg_float */

case COP_FNEG:
{
	/* Negate floating point */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_neg_reg_float(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* MD_HAS_FP */

case COP_IAND:
{
	/* Bitwise and integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_and_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IOR:
{
	/* Bitwise or integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_or_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IXOR:
{
	/* Bitwise xor integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_xor_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_INOT:
{
	/* Bitwise not integer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_not_reg_word_32(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISHL:
{
	/* Left shift integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_shl_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISHR:
{
	/* Right shift integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_shr_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ISHR_UN:
{
	/* Unsigned right shift integers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_ushr_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef MD_HAS_FP

#ifdef md_cmp_reg_reg_float

case 0x100 + COP_PREFIX_FCMPL:
{
	/* Compare floating point values */
	UNROLL_START();
	CheckWordFull(&unroll);
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 0);
	reg3 = GetWordRegister(&unroll, MD_REG1_32BIT);
	md_cmp_reg_reg_float(unroll.out, reg3, reg, reg2, 1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg3, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_FCMPG:
{
	/* Compare floating point values */
	UNROLL_START();
	CheckWordFull(&unroll);
	GetTopTwoFPRegisters(&unroll, &reg, &reg2, 0);
	reg3 = GetWordRegister(&unroll, MD_REG1_32BIT);
	md_cmp_reg_reg_float(unroll.out, reg3, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg3, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

#endif /* md_cmp_reg_reg_float */

#endif /* MD_HAS_FP */

case 0x100 + COP_PREFIX_ICMP:
{
	/* Compare integer values with -1, 0, or 1 result */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_cmp_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_ICMP_UN:
{
	/* Compare unsigned integer values with -1, 0, or 1 result */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2, MD_REG1_32BIT | MD_REG2_32BIT);
	md_ucmp_reg_reg_word_32(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_PCMP:
{
	/* Compare native word values with -1, 0, or 1 result */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_ucmp_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SETEQ:
{
	/* Set if top of stack is equal to zero */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_seteq_reg(unroll.out, reg);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SETNE:
{
	/* Set if top of stack is not equal to zero */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_setne_reg(unroll.out, reg);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SETLT:
{
	/* Set if top of stack is less than zero */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_setlt_reg(unroll.out, reg);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SETLE:
{
	/* Set if top of stack is less than or equal to zero */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_setle_reg(unroll.out, reg);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SETGT:
{
	/* Set if top of stack is greater than zero */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_setgt_reg(unroll.out, reg);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SETGE:
{
	/* Set if top of stack is greater than or equal to zero */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_setge_reg(unroll.out, reg);
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

#ifdef MD_HAS_FP

#ifdef md_abs_reg_float

case 0x100 + COP_PREFIX_ABS_R4:
case 0x100 + COP_PREFIX_ABS_R8:
{
	/* Absolute value of a floating point */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_abs_reg_float(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_abs_reg_float */

#ifdef md_cos_reg_float

case 0x100 + COP_PREFIX_COS:
{
	/* Sqare root of a floating point */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_cos_reg_float(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_cos_reg_float */

#ifdef md_sin_reg_float

case 0x100 + COP_PREFIX_SIN:
{
	/* Sqare root of a floating point */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_sin_reg_float(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_sin_reg_float */

#ifdef md_sqrt_reg_float

case 0x100 + COP_PREFIX_SQRT:
{
	/* Sqare root of a floating point */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_sqrt_reg_float(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_sqrt_reg_float */

#ifdef md_tan_reg_float

case 0x100 + COP_PREFIX_TAN:
{
	/* Sqare root of a floating point */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_tan_reg_float(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_tan_reg_float */

#endif /* MD_HAS_FP */

#endif /* IL_UNROLL_CASES */
