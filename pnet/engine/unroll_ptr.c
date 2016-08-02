/*
 * unroll_ptr.c - Pointer handling for generic CVM unrolling.
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

#ifdef HAVE_STDDEF_H
#include <stddef.h>
#endif
/* 
This should work in case offsetof is not preset, but I fear
the segfaults. 

#ifndef offsetof
#define offsetof(type, field) \
	   		((int)(&(((type *)(NULL))->field)))
#endif
*/

#include <lib_defs.h>

/*
 * Check the contents of a register for NULL and re-execute
 * the current instruction in the interpreter if it is.
 * This function generates a null check even if null check 
 * elimination is enabled.
 */
static void ForceCheckForNull(MDUnroll *unroll, int reg, unsigned char *pc,
						 unsigned char *label, int popReg)
{
	md_inst_ptr patch;

	/* Check the register's contents against NULL */
	md_reg_is_null(unroll->out, reg);
	patch = unroll->out;
#ifdef CVM_X86
	x86_branch8(unroll->out, X86_CC_NE, 0, 0);
#else
	md_branch_ne(unroll->out);
#endif

	/* Re-execute the current instruction in the interpreter */
	if(popReg)
	{
		--(unroll->pseudoStackSize);
		ReExecute(unroll, pc, label);
		++(unroll->pseudoStackSize);
	}
	else
	{
		ReExecute(unroll, pc, label);
	}

	/* Continue with real execution here */
	md_patch(patch, unroll->out);
}

/*
 * Check the contents of a register for NULL and re-execute
 * the current instruction in the interpreter if it is.
 * If null check elmination is enabled, this function does nothing.
 */
static void CheckForNull(MDUnroll *unroll, int reg, unsigned char *pc,
						 unsigned char *label, int popReg)
{
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	md_inst_ptr patch;

	/* Check the register's contents against NULL */
	md_reg_is_null(unroll->out, reg);
	patch = unroll->out;
#ifdef CVM_X86
	x86_branch8(unroll->out, X86_CC_NE, 0, 0);
#else
	md_branch_ne(unroll->out);
#endif

	/* Re-execute the current instruction in the interpreter */
	if(popReg)
	{
		--(unroll->pseudoStackSize);
		ReExecute(unroll, pc, label);
		++(unroll->pseudoStackSize);
	}
	else
	{
		ReExecute(unroll, pc, label);
	}

	/* Continue with real execution here */
	md_patch(patch, unroll->out);
#endif
}

/*
 * Check an array access operation for exception conditions.
 */
static void CheckArrayAccess(MDUnroll *unroll, int reg, int reg2,
							 unsigned char *pc, unsigned char *label)
{
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	md_inst_ptr patch1;
#endif
	md_inst_ptr patch2;

#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	/* Check the array reference against NULL */
	md_reg_is_null(unroll->out, reg);
	patch1 = unroll->out;
#ifdef CVM_X86
	x86_branch8(unroll->out, X86_CC_EQ, 0, 0);
#else
	md_branch_eq(unroll->out);
#endif
#endif
	/* Check the array bounds */
	md_bounds_check(unroll->out, reg, reg2);
	patch2 = unroll->out;
	md_branch_lt_un(unroll->out);

#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	/* Re-execute the current instruction in the interpreter */
	md_patch(patch1, unroll->out);
#endif
	ReExecute(unroll, pc, label);

	/* Continue with real execution here */
	md_patch(patch2, unroll->out);
}

/*
 * Bounds checking for 2D arrays is a bit complicated, so we have
 * to do it specially for each CPU type, unfortunately.
 */

#ifdef CVM_X86

#define	MD_HAS_2D_ARRAYS	1

/*
 * Check a 2D array access operation for exception conditions.
 */
static void Check2DArrayAccess(MDUnroll *unroll, int reg, int reg2, int reg3,
							   unsigned char *pc, unsigned char *label)
{
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	unsigned char *patch1;
#endif
	unsigned char *patch2;
	unsigned char *patch3;

#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	/* Check the array reference against NULL */
	x86_alu_reg_reg(unroll->out, X86_OR, reg, reg);
	patch1 = unroll->out;
	x86_branch8(unroll->out, X86_CC_EQ, 0, 0);
#endif

	/* Check the array bounds */
	x86_alu_reg_membase(unroll->out, X86_SUB, reg2, reg, 12);
	x86_alu_reg_membase(unroll->out, X86_CMP, reg2, reg, 16);
	patch2 = unroll->out;
	x86_branch32(unroll->out, X86_CC_LT, 0, 0);
	x86_alu_reg_membase(unroll->out, X86_ADD, reg2, reg, 12);
	patch3 = unroll->out;
	x86_jump8(unroll->out, 0);
	x86_patch(patch2, unroll->out);
	x86_alu_reg_membase(unroll->out, X86_SUB, reg3, reg, 24);
	x86_alu_reg_membase(unroll->out, X86_CMP, reg3, reg, 28);
	patch2 = unroll->out;
	x86_branch32(unroll->out, X86_CC_LT, 0, 0);
	x86_alu_reg_membase(unroll->out, X86_ADD, reg2, reg, 12);
	x86_alu_reg_membase(unroll->out, X86_ADD, reg3, reg, 28);

	/* Re-execute the current instruction in the interpreter */
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	x86_patch(patch1, unroll->out);
#endif
	x86_patch(patch3, unroll->out);
	ReExecute(unroll, pc, label);

	/* Compute the address of the array element */
	x86_patch(patch2, unroll->out);
	x86_imul_reg_membase(unroll->out, reg2, reg, 20);
	x86_imul_reg_membase(unroll->out, reg3, reg, 32);
	x86_alu_reg_reg(unroll->out, X86_ADD, reg2, reg3);
	x86_imul_reg_membase(unroll->out, reg2, reg, 4);
	x86_mov_reg_membase(unroll->out, reg, reg, 8, 4);
	x86_alu_reg_reg(unroll->out, X86_ADD, reg, reg2);
}

#endif /* CVM_X86 */

#ifdef CVM_X86_64

#define	MD_HAS_2D_ARRAYS	1 

/*
 * Check a 2D array access operation for exception conditions.
 */
static void Check2DArrayAccess(MDUnroll *unroll, int reg, int reg2, int reg3,
							   unsigned char *pc, unsigned char *label)
{
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	unsigned char *patch1;
#endif
	unsigned char *patch2;
	unsigned char *patch3;

#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	/* Check the array reference against NULL */
	amd64_alu_reg_reg(unroll->out, X86_OR, reg, reg);
	patch1 = unroll->out;
	amd64_branch8(unroll->out, X86_CC_EQ, 0, 0);
#endif

	/*
	  a->data + (((i - a->bounds[0].lower) * a->bounds[0].multiplier) +
	  	(j - a->bounds[1].lower) * a->bounds[0].multiplier)) * a->elemSize)

	  as follows

	  i = i - a->bounds[0].lower 
	  j = j - a->bounds[1].lower
	  i = i * a->bounds[0].multiplier
	  j = j * a->bounds[1].multiplier
	  i = i + j
	  i = i * a->elemSize
	  a = a->data
	  a = a + i
	*/

	/* Check the array bounds (all of which are ILInt32) */
	amd64_alu_reg_membase_size(unroll->out, X86_SUB, reg2, reg, 
							offsetof(System_MArray, bounds)
							+ offsetof(MArrayBounds, lower),
							4); /* i = i - a->bounds[0].lower */
	
	amd64_alu_reg_membase_size(unroll->out, X86_CMP, reg2, reg,
							offsetof(System_MArray, bounds)
							+ offsetof(MArrayBounds, size),
							4);
	patch2 = unroll->out;
	amd64_branch32(unroll->out, X86_CC_LT, 0, 0);
	amd64_alu_reg_membase_size(unroll->out, X86_ADD, reg2, reg, 
							offsetof(System_MArray, bounds)
							+ offsetof(MArrayBounds, lower),
							4);
	patch3 = unroll->out;
	amd64_jump8(unroll->out, 0);
	amd64_patch(patch2, unroll->out);

	amd64_alu_reg_membase_size(unroll->out, X86_SUB, reg3, reg,
							offsetof(System_MArray, bounds)
							+ sizeof(MArrayBounds)
							+ offsetof(MArrayBounds, lower),
							4); /* j = j - a->bounds[1].lower */
	
	amd64_alu_reg_membase_size(unroll->out, X86_CMP, reg3, reg, 
							offsetof(System_MArray, bounds)
							+ sizeof(MArrayBounds)
							+ offsetof(MArrayBounds, size),
							4);
	patch2 = unroll->out;
	amd64_branch32(unroll->out, X86_CC_LT, 0, 0);
	amd64_alu_reg_membase_size(unroll->out, X86_ADD, reg2, reg,
							offsetof(System_MArray, bounds) +
							offsetof(MArrayBounds, lower),
							4);
	amd64_alu_reg_membase_size(unroll->out, X86_ADD, reg3, reg,
							offsetof(System_MArray, bounds) 
							+ sizeof(MArrayBounds)
							+ offsetof(MArrayBounds, lower),
							4);

	/* Re-execute the current instruction in the interpreter */
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	amd64_patch(patch1, unroll->out);
#endif
	amd64_patch(patch3, unroll->out);
	ReExecute(unroll, pc, label);

	/* Compute the address of the array element */
	amd64_patch(patch2, unroll->out);
	
	amd64_imul_reg_membase_size(unroll->out, reg2, reg, 
									offsetof(System_MArray, bounds)
									+ offsetof(MArrayBounds, multiplier),
									4); /* i = i * a->bounds[0].multiplier */
	amd64_imul_reg_membase_size(unroll->out, reg3, reg,
									offsetof(System_MArray, bounds)
									+ sizeof(MArrayBounds)
									+ offsetof(MArrayBounds, multiplier),
									4); /* j = j * a->bounds[1].multiplier */
	amd64_alu_reg_reg(unroll->out, X86_ADD, reg2, reg3); /* i = i + j */
	amd64_imul_reg_membase_size(unroll->out, reg2, reg,
									offsetof(System_MArray, elemSize),
									4); /* i = i * a->elemSize */
	amd64_mov_reg_membase(unroll->out, reg, reg, 
									offsetof(System_MArray, data), 
									8); /* a = a->data */
	
	/* up-convert to 32 bit */
	amd64_movsxd_reg_reg(unroll->out, reg2, reg2);
	amd64_alu_reg_reg(unroll->out, X86_ADD, reg, reg2); /* a = a + i */
}

#endif /* CVM_X86_64 */

#ifdef CVM_ARM

#define	MD_HAS_2D_ARRAYS	1

/*
 * Check a 2D array access operation for exception conditions.
 */
static void Check2DArrayAccess(MDUnroll *unroll, int reg, int reg2, int reg3,
							   unsigned char *pc, unsigned char *label)
{
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	md_inst_ptr patch1;
#endif
	md_inst_ptr patch2;
	md_inst_ptr patch3;

#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	/* Check the array reference against NULL */
	arm_test_reg_imm8(unroll->out, ARM_CMP, reg, 0);
	patch1 = unroll->out;
	arm_branch_imm(unroll->out, ARM_CC_EQ, 0);
#endif

	/* Check the array bounds.  We assume that we can use the link
	   register as a work register because "lr" would have been
	   saved on entry to "_ILCVMInterpreter" */
	arm_load_membase(unroll->out, ARM_WORK, reg, 12);
	arm_alu_reg_reg(unroll->out, ARM_SUB, reg2, reg2, ARM_WORK);
	arm_load_membase(unroll->out, ARM_LINK, reg, 16);
	arm_test_reg_reg(unroll->out, ARM_CMP, reg2, ARM_LINK);
	patch2 = unroll->out;
	arm_branch_imm(unroll->out, ARM_CC_LT_UN, 0);
	arm_alu_reg_reg(unroll->out, ARM_ADD, reg2, reg2, ARM_WORK);
	patch3 = unroll->out;
	arm_jump_imm(unroll->out, 0);
	arm_patch(patch2, unroll->out);
	arm_load_membase(unroll->out, ARM_WORK, reg, 24);
	arm_alu_reg_reg(unroll->out, ARM_SUB, reg3, reg3, ARM_WORK);
	arm_load_membase(unroll->out, ARM_LINK, reg, 28);
	arm_test_reg_reg(unroll->out, ARM_CMP, reg3, ARM_LINK);
	patch2 = unroll->out;
	arm_branch_imm(unroll->out, ARM_CC_LT_UN, 0);
	arm_alu_reg_reg(unroll->out, ARM_ADD, reg3, reg3, ARM_WORK);
	arm_load_membase(unroll->out, ARM_WORK, reg, 12);
	arm_alu_reg_reg(unroll->out, ARM_ADD, reg2, reg2, ARM_WORK);

	/* Re-execute the current instruction in the interpreter */
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	arm_patch(patch1, unroll->out);
#endif
	arm_patch(patch3, unroll->out);
	ReExecute(unroll, pc, label);

	/* Compute the address of the array element */
	arm_patch(patch2, unroll->out);
	arm_load_membase(unroll->out, ARM_WORK, reg, 20);
	arm_mul_reg_reg(unroll->out, reg2, reg2, ARM_WORK);
	arm_load_membase(unroll->out, ARM_WORK, reg, 32);
	arm_mul_reg_reg(unroll->out, reg3, reg3, ARM_WORK);
	arm_alu_reg_reg(unroll->out, ARM_ADD, reg2, reg2, reg3);
	arm_load_membase(unroll->out, ARM_WORK, reg, 4);
	arm_mul_reg_reg(unroll->out, reg2, reg2, ARM_WORK);
	arm_load_membase(unroll->out, reg, reg, 8);
	arm_alu_reg_reg(unroll->out, ARM_ADD, reg, reg, reg2);
}

#endif /* CVM_ARM */

#ifdef CVM_PPC

#define	MD_HAS_2D_ARRAYS	1

static void Check2DArrayAccess(MDUnroll *unroll, int reg, int reg2, int reg3,
							   unsigned char *pc, unsigned char *label)
{
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	md_inst_ptr patch1;
#endif
	md_inst_ptr patch2;
	md_inst_ptr patch3;

	/* redefinitions to make it a lot clearer for me to debug */
	int array  = reg;
	int i = reg2;
	int j = reg3;
	int work = PPC_WORK;

	/*
	  Algorithm of calc_address:
		
	  a->data + (((i - a->bounds[0].lower) * a->bounds[0].multiplier) +
	  	(j - a->bounds[1].lower) * a->bounds[0].multiplier)) * a->elemSize)
	  
	  I've tried to pre-compute a->bounds into bounds (or MD_REG_PC)
	*/
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	md_reg_is_null(unroll->out, reg);
	patch1 = unroll->out;
	md_branch_eq(unroll->out);
#endif

	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds) 
									+ offsetof(MArrayBounds, lower));
	md_sub_reg_reg_word_32(unroll->out, i, work); 
	
	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ offsetof(MArrayBounds, size));

	md_cmp_cc_reg_reg_word_32(unroll->out, MD_CC_GE_UN, i, work);

	patch2 = unroll->out;
	md_branch_ge_un(unroll->out);

	
	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ sizeof(MArrayBounds)
									+ offsetof(MArrayBounds, lower));
									
	md_sub_reg_reg_word_32(unroll->out, j, work); 
	
	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ sizeof(MArrayBounds)
									+ offsetof(MArrayBounds, size));

	md_cmp_cc_reg_reg_word_32(unroll->out, MD_CC_GE_UN, j, work);

	patch3 = unroll->out;
	md_branch_lt_un(unroll->out);

	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ sizeof(MArrayBounds)
									+ offsetof(MArrayBounds, lower));
									
	md_add_reg_reg_word_32(unroll->out, j, work); 
	
	md_patch(patch2, unroll->out);	

	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ offsetof(MArrayBounds, lower));
									
	md_add_reg_reg_word_32(unroll->out, i, work); 
#ifndef IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS
	md_patch(patch1, unroll->out);	
#endif
	ReExecute(unroll, pc, label);

	/* calculate address */
	md_patch(patch3, unroll->out);	
	
	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ offsetof(MArrayBounds, multiplier));
	md_mul_reg_reg_word_32(unroll->out, i, work);

	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, bounds)
									+ sizeof(MArrayBounds)
									+ offsetof(MArrayBounds, multiplier));
	md_mul_reg_reg_word_32(unroll->out, j, work);

	md_load_membase_word_32(unroll->out, work, array, 
									offsetof(System_MArray, elemSize));

	md_add_reg_reg_word_32(unroll->out, i, j);
	
	md_load_membase_word_native(unroll->out, reg, array, 
									offsetof(System_MArray,	data));
	
	md_mul_reg_reg_word_32(unroll->out, i, work);
	
	ppc_cache_prefetch(unroll->out, reg, i);

	md_reg_to_word_native(unroll->out, i);
	md_add_reg_reg_word_native(unroll->out, reg, i);
}

#endif

#elif defined(IL_UNROLL_CASES)

case COP_CKNULL:
{
	/* Check the stack top for null and throw an exception if it is */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	ForceCheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_BREAD:
{
	/* Read a signed byte value from a pointer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_sbyte(unroll.out, reg, reg, 0);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_UBREAD:
{
	/* Read an unsigned byte value from a pointer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_byte(unroll.out, reg, reg, 0);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_SREAD:
{
	/* Read a signed short value from a pointer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_short(unroll.out, reg, reg, 0);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_USREAD:
{
	/* Read an unsigned short value from a pointer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_ushort(unroll.out, reg, reg, 0);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IREAD:
{
	/* Read a 32-bit word value from a pointer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_word_32(unroll.out, reg, reg, 0);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PREAD:
{
	/* Read a native word value from a pointer */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_word_native(unroll.out, reg, reg, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef MD_HAS_FP

case COP_FREAD:
{
	/* Read a float32 value from a pointer */
	UNROLL_START();
	CheckFPFull(&unroll);
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	reg2 = GetFPRegister(&unroll);
	md_load_membase_float_32(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_DREAD:
{
	/* Read a float64 value from a pointer */
	UNROLL_START();
	CheckFPFull(&unroll);
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	reg2 = GetFPRegister(&unroll);
	md_load_membase_float_64(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* MD_HAS_FP */

case COP_BWRITE:
{
	/* Write a byte value to a pointer */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	md_store_membase_byte(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_SWRITE:
{
	/* Write a short value to a pointer */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	md_store_membase_short(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IWRITE:
{
	/* Write a word value to a pointer */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	md_store_membase_word_32(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PWRITE:
{
	/* Write a native word value to a pointer */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_store_membase_word_native(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef MD_HAS_FP

case COP_FWRITE:
{
	/* Write a float32 value to a pointer */
	UNROLL_START();
	GetWordAndFPRegisters(&unroll, &reg, &reg2);
	md_store_membase_float_32(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_DWRITE:
{
	/* Write a float64 value to a pointer */
	UNROLL_START();
	GetWordAndFPRegisters(&unroll, &reg, &reg2);
	md_store_membase_float_64(unroll.out, reg2, reg, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* MD_HAS_FP */

case COP_BWRITE_R:
{
	/* Write a byte value to a pointer with reversed arguments */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_NATIVE);
	md_store_membase_byte(unroll.out, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_SWRITE_R:
{
	/* Write a short value to a pointer with reversed arguments */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_NATIVE);
	md_store_membase_short(unroll.out, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IWRITE_R:
{
	/* Write a word value to a pointer with reversed arguments */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_NATIVE);
	md_store_membase_word_32(unroll.out, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PWRITE_R:
{
	/* Write a native word value to a pointer with reversed arguments */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_store_membase_word_native(unroll.out, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef MD_HAS_FP
#ifndef CVM_PPC /* TODO: bug in unroll stack handling */

case COP_FWRITE_R:
{
	/* Write a float32 value to a pointer with reversed arguments */
	UNROLL_START();
	GetFPAndWordRegisters(&unroll, &reg, &reg2);
	md_store_membase_float_32(unroll.out, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_DWRITE_R:
{
	/* Write a float64 value to a pointer with reversed arguments */
	UNROLL_START();
	GetFPAndWordRegisters(&unroll, &reg, &reg2);
	md_store_membase_float_64(unroll.out, reg, reg2, 0);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif
#endif /* MD_HAS_FP */

case COP_PADD_OFFSET:
{
	/* Add an offset value to a pointer */
	unsigned val = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	md_add_reg_imm(unroll.out, reg, val);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_PADD_I4:
{
	/* Add an integer value to a pointer */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	md_reg_to_word_native(unroll.out, reg2);
	md_add_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PADD_I4_R:
{
	/* Add an integer value to a pointer with reversed arguments */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_NATIVE);
	md_reg_to_word_native(unroll.out, reg);
	md_add_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PSUB:
{
	/* Subtract pointers */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	md_sub_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PSUB_I4:
{
	/* Subtract an integer from a pointer */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	md_reg_to_word_native(unroll.out, reg2);
	md_sub_reg_reg_word_native(unroll.out, reg, reg2);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#define	MD_ARRAY_HEADER		(sizeof(System_Array))

case COP_BREAD_ELEM:
{
	/* Read a byte from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_sbyte(unroll.out, reg, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_UBREAD_ELEM:
{
	/* Read an unsigned byte from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_byte(unroll.out, reg, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_SREAD_ELEM:
{
	/* Read a short from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_short(unroll.out, reg, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_USREAD_ELEM:
{
	/* Read an unsigned short from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_ushort(unroll.out, reg, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IREAD_ELEM:
{
	/* Read a word from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_word_32(unroll.out, reg, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PREAD_ELEM:
{
	/* Read a native word from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_word_native(unroll.out, reg, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_BWRITE_ELEM:
{
	/* Write a byte to an array */
	UNROLL_START();
	GetTopThreeWordRegisters(&unroll, &reg, &reg2, &reg3,
							 MD_REG1_NATIVE | MD_REG2_32BIT | MD_REG3_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_store_memindex_byte(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_SWRITE_ELEM:
{
	/* Write a short to an array */
	UNROLL_START();
	GetTopThreeWordRegisters(&unroll, &reg, &reg2, &reg3,
							 MD_REG1_NATIVE | MD_REG2_32BIT | MD_REG3_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_store_memindex_short(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_IWRITE_ELEM:
{
	/* Write a word to an array */
	UNROLL_START();
	GetTopThreeWordRegisters(&unroll, &reg, &reg2, &reg3,
							 MD_REG1_NATIVE | MD_REG2_32BIT | MD_REG3_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_store_memindex_word_32(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_PWRITE_ELEM:
{
	/* Write a word to an array */
	UNROLL_START();
	GetTopThreeWordRegisters(&unroll, &reg, &reg2, &reg3,
							 MD_REG1_NATIVE | MD_REG2_32BIT | MD_REG3_NATIVE);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_store_memindex_word_native(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_ARRAY_LEN:
{
	/* Get the length of an array */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_word_32(unroll.out, reg, reg, 0);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef md_lea_memindex_shift

case COP_ELEM_ADDR_SHIFT_I4:
{
	/* Load the effective address of an element from an array */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_lea_memindex_shift(unroll.out, reg, reg, reg2, temp, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

#endif /* md_lea_memindex_shift */

#ifdef md_lea_memindex_mul

case COP_ELEM_ADDR_MUL_I4:
{
	/* Load the effective address of an element from an array */
	ILUInt32 temp = (unsigned)CVM_ARG_WORD;
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_lea_memindex_mul(unroll.out, reg, reg, reg2, temp, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_WORD);
}
break;

#endif /* md_lea_memindex_mul */

case COP_BREAD_FIELD:
{
	/* Read a byte field from an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_sbyte(unroll.out, reg, reg, (int)temp);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_UBREAD_FIELD:
{
	/* Read an unsigned byte field from an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_byte(unroll.out, reg, reg, (int)temp);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_SREAD_FIELD:
{
	/* Read a short field from an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_short(unroll.out, reg, reg, (int)temp);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_USREAD_FIELD:
{
	/* Read an unsigned byte field from an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_ushort(unroll.out, reg, reg, (int)temp);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_IREAD_FIELD:
{
	/* Read a word field from an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_word_32(unroll.out, reg, reg, (int)temp);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_PREAD_FIELD:
{
	/* Read a word field from an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_load_membase_word_native(unroll.out, reg, reg, (int)temp);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_BWRITE_FIELD:
{
	/* Write a byte field to an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_store_membase_byte(unroll.out, reg2, reg, (int)temp);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_SWRITE_FIELD:
{
	/* Write a short field to an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_store_membase_short(unroll.out, reg2, reg, (int)temp);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_IWRITE_FIELD:
{
	/* Write a word field to an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_store_membase_word_32(unroll.out, reg2, reg, (int)temp);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_PWRITE_FIELD:
{
	/* Write a word field to an object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_NATIVE);
	CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 0);
	md_store_membase_word_native(unroll.out, reg2, reg, (int)temp);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_IREAD_THIS:
{
	/* Read a word field from the "this" object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();

	/* Load "this" into a new register */
	reg = GetWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_word_native(unroll.out, reg, MD_REG_FRAME, 0);

	/* Check "this" for NULL if we haven't done so already.
	   If "thisValidated" is -1, then it indicates that the
	   "this" variable has been addressed using "waddr" and
	   so we must always validate it just in case someone
	   writes to that address further down the code */
	if(unroll.thisValidated <= 0)
	{
		CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 1);
		if(!(unroll.thisValidated))
		{
			unroll.thisValidated = 1;
		}
	}

	/* Read the contents of the field */
	md_load_membase_word_32(unroll.out, reg, reg, (int)temp);
	ChangeRegisterType(&unroll, MD_REG1_32BIT);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

case COP_PREAD_THIS:
{
	/* Read a native word field from the "this" object */
	unsigned temp = (unsigned)CVM_ARG_BYTE;
	UNROLL_START();

	/* Load "this" into a new register */
	reg = GetWordRegister(&unroll, MD_REG1_NATIVE);
	md_load_membase_word_native(unroll.out, reg, MD_REG_FRAME, 0);

	/* Check "this" for NULL if we haven't done so already.
	   If "thisValidated" is -1, then it indicates that the
	   "this" variable has been addressed using "waddr" and
	   so we must always validate it just in case someone
	   writes to that address further down the code */
	if(unroll.thisValidated <= 0)
	{
		CheckForNull(&unroll, reg, pc, (unsigned char *)inst, 1);
		if(!(unroll.thisValidated))
		{
			unroll.thisValidated = 1;
		}
	}

	/* Read the contents of the field */
	md_load_membase_word_native(unroll.out, reg, reg, (int)temp);
	MODIFY_UNROLL_PC(CVM_LEN_BYTE);
}
break;

#ifdef MD_HAS_FP

#ifdef md_load_memindex_float_32

case 0x100 + COP_PREFIX_FREAD_ELEM:
{
	/* Read a word from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	reg3 = GetFPRegister(&unroll);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_float_32(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg3, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_load_memindex_float_32 */

#ifdef md_load_memindex_float_64

case 0x100 + COP_PREFIX_DREAD_ELEM:
{
	/* Read a word from an array */
	UNROLL_START();
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_NATIVE | MD_REG2_32BIT);
	reg3 = GetFPRegister(&unroll);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_load_memindex_float_64(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg3, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_load_memindex_float_64 */

#ifdef md_store_memindex_float_32

case 0x100 + COP_PREFIX_FWRITE_ELEM:
{
	/* Read a word from an array */
	UNROLL_START();
	GetTopTwoWordAndFPRegisters(&unroll, &reg, &reg2, &reg3,
								MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_store_memindex_float_32(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_store_memindex_float_32 */

#ifdef md_store_memindex_float_64

case 0x100 + COP_PREFIX_DWRITE_ELEM:
{
	/* Read a word from an array */
	UNROLL_START();
	GetTopTwoWordAndFPRegisters(&unroll, &reg, &reg2, &reg3,
								MD_REG1_NATIVE | MD_REG2_32BIT);
	CheckArrayAccess(&unroll, reg, reg2, pc, (unsigned char *)inst);
	md_store_memindex_float_64(unroll.out, reg3, reg, reg2, MD_ARRAY_HEADER);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_store_memindex_float_64 */

#endif /* MD_HAS_FP */

#ifdef MD_HAS_2D_ARRAYS

case 0x100 + COP_PREFIX_GET2D:
{
	/* Get the array object reference and indices into registers */
	UNROLL_START();
	GetTopThreeWordRegisters(&unroll, &reg, &reg2, &reg3,
							 MD_REG1_NATIVE | MD_REG2_32BIT |
							 MD_REG3_32BIT);

	/* Check the access and compute the element address */
	Check2DArrayAccess(&unroll, reg, reg2, reg3, pc, (unsigned char *)inst);

	/* Pop unnecessary registers: the address is now in "reg" */
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);

	/* Skip to the next instruction */
	MODIFY_UNROLL_PC(CVMP_LEN_NONE);
}
break;

case 0x100 + COP_PREFIX_SET2D:
{
	unsigned temp = (unsigned)CVMP_ARG_WORD;
	int offset;

	/* Flush the register stack onto the real stack so that we know
	   where everything is */
	UNROLL_START();
	FlushRegisterStack(&unroll);
	offset = unroll.stackHeight - ((int)((temp + 3) * sizeof(CVMWord)));

	/* Fetch the array object reference and indices */
	reg = regAllocOrder[0];
	reg2 = regAllocOrder[1];
	reg3 = regAllocOrder[2];
	md_load_membase_word_native(unroll.out, reg, MD_REG_STACK, offset);
	md_load_membase_word_32(unroll.out, reg2, MD_REG_STACK,
							offset + sizeof(CVMWord));
	md_load_membase_word_32(unroll.out, reg3, MD_REG_STACK,
							offset + sizeof(CVMWord) * 2);

	/* Check the access and compute the element address */
	Check2DArrayAccess(&unroll, reg, reg2, reg3, pc, (unsigned char *)inst);

	/* Shift everything down on the stack to squash out the indices */
	md_store_membase_word_native(unroll.out, reg, MD_REG_STACK, offset);
	temp = (temp * sizeof(CVMWord)) / sizeof(ILNativeInt);
	offset += sizeof(CVMWord);
	while(temp > 0)
	{
		md_load_membase_word_native(unroll.out, reg2, MD_REG_STACK,
									offset + sizeof(CVMWord) * 2);
		md_store_membase_word_native(unroll.out, reg2, MD_REG_STACK, offset);
		offset += sizeof(ILNativeInt);
		--temp;
	}

	/* Adjust the top of stack pointer */
	unroll.stackHeight -= sizeof(CVMWord) * 2;

	/* Skip to the next instruction */
	MODIFY_UNROLL_PC(CVMP_LEN_WORD);
}
break;

#endif /* MD_HAS_2D_ARRAYS */

#endif /* IL_UNROLL_CASES */
