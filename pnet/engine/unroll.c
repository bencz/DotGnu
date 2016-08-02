/*
 * unroll.c - CVM unrolling module.
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

#include "engine.h"
#include "cvm.h"
#include "method_cache.h"
#include "cvm_config.h"
#include "cvm_format.h"
#include "il_dumpasm.h"
#include "lib_defs.h"

#ifdef IL_USE_CVM

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_CVM_DIRECT_UNROLLED

/*
 * Include the machine-dependent definitions.
 */
#if defined(CVM_X86)
	#include "md_x86.h"
#elif defined(CVM_X86_64)
	#include "md_amd64.h"
#elif defined(CVM_ARM)
	#include "md_arm.h"
#elif defined(CVM_PPC)
	#include "md_ppc.h"
#elif defined(CVM_IA64)
	#include "md_ia64.h"
#else
	#error "Unknown unroller platform"
#endif
#include "md_default.h"

/*
 * Determine if the machine-dependent macros support floating-point.
 */
#if (MD_FP_STACK_SIZE != 0)
#define	MD_HAS_FP		1
#define	MD_HAS_FP_STACK	1
#elif (MD_FREG_0 != -1)
#define	MD_HAS_FP		1
#endif

/*
 * Unrolled code generation state.
 */
typedef struct
{
	md_inst_ptr	out;			/* Code output buffer */
	int		regsUsed;			/* General registers currently in use */
	int		regsSaved;			/* Fixed registers that were saved */
	int		pseudoStack[32];	/* Registers that make up the pseudo stack */
	int		pseudoStackSize;	/* Size of the pseudo stack */
	int		stackHeight;		/* Current virtual height of CVM stack */
	long	cachedLocal;		/* Local variable that was just stored */
	int		cachedReg;			/* Register for variable that was stored */
	int		thisValidated;		/* "this" has been checked for NULL */
#if !MD_STATE_ALREADY_IN_REGS
	int		pcOffset;			/* interpreter's pc variable offset */
	int		stackOffset;		/* interpreter's stack variable offset */
	int		frameOffset;		/* interpreter's frame variable offset */
#endif
#if MD_HAS_FP
#if MD_HAS_FP_STACK
	int		fpStackSize;		/* Size of the floating-point stack */
#else /* !MD_HAS_FP_STACK */
	int		fpRegsUsed;			/* Floatingpoint registers currently in use */
#endif /* !MD_HAS_FP_STACK */
#endif
} MDUnroll;

/*
 * Register allocation order for the word registers.
 */
static signed char const regAllocOrder[] =
		{MD_REG_0, MD_REG_1, MD_REG_2, MD_REG_3,
		 MD_REG_4, MD_REG_5, MD_REG_6, MD_REG_7,
		 MD_REG_8, MD_REG_9, MD_REG_10, MD_REG_11,
		 MD_REG_12, MD_REG_13, MD_REG_14, MD_REG_15};

/*
 * Register allocation order for the floating-point registers.
 */
#if MD_HAS_FP && !MD_HAS_FP_STACK
static signed char const regAllocFPOrder[] =
		{MD_FREG_0, MD_FREG_1, MD_FREG_2, MD_FREG_3,
		 MD_FREG_4, MD_FREG_5, MD_FREG_6, MD_FREG_7,
		 MD_FREG_8, MD_FREG_9, MD_FREG_10, MD_FREG_11,
		 MD_FREG_12, MD_FREG_13, MD_FREG_14, MD_FREG_15};
#endif

/*
 * Determine if a register number corresponds to an FPU register.
 */
#define	MD_IS_FREG(reg)			(((reg) & MD_FREG_MASK) != 0)

/*
 * Flag that indicates if a pseudo-stack word register is assumed
 * to contain a native word-sized value instead of a 32-bit value.
 */
#define	MD_NATIVE_REG_MASK		0x0100

/*
 * Determine if a pseudo-stack word register is native-sized.
 */
#define	MD_IS_NATIVE_REG(reg)	(((reg) & MD_NATIVE_REG_MASK) != 0)

/*
 * Flush all registers to the CVM operand stack, without updating
 * the height information in "unroll".
 */
static int FlushRegisterStackNoUpdate(MDUnroll *unroll)
{
	int index, reg;
	int stackHeight;
#if MD_HAS_FP_STACK
	int height;
#endif

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Flush the registers from bottom-most to top-most */
	stackHeight = unroll->stackHeight;
	for(index = 0; index < unroll->pseudoStackSize; ++index)
	{
		reg = unroll->pseudoStack[index];
		if(MD_IS_NATIVE_REG(reg))
		{
			/* Flush a native word register */
			reg &= ~MD_NATIVE_REG_MASK;
			md_store_membase_word_native(unroll->out, reg, MD_REG_STACK,
								         stackHeight);
			stackHeight += sizeof(CVMWord);
		}
#if MD_HAS_FP
		else if(MD_IS_FREG(reg))
		{
#if !MD_HAS_FP_STACK
			/* Flush an FP register */
			md_store_membase_float_native
				(unroll->out, reg, MD_REG_STACK, stackHeight);
#else /* MD_HAS_FP_STACK */
			/* Skip FP registers on the stack and handle them later */
#endif /* MD_HAS_FP_STACK */
			stackHeight += CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord);
		}
#endif /* MD_HAS_FP */
		else
		{
			/* Flush a 32-bit word register */
			md_store_membase_word_32(unroll->out, reg, MD_REG_STACK,
								     stackHeight);
			stackHeight += sizeof(CVMWord);
		}
	}

#if MD_HAS_FP_STACK
	/* Flush the FPU stack from top-most to bottom-most */
	height = stackHeight;
	for(index = unroll->pseudoStackSize - 1; index >= 0; --index)
	{
		reg = unroll->pseudoStack[index];
		if(!MD_IS_FREG(reg))
		{
			/* Skip a word register */
			height -= sizeof(CVMWord);
		}
		else
		{
			/* Flush an FPU register */
			height -= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord);
			md_store_membase_float_native
				(unroll->out, MD_FREG_0, MD_REG_STACK, height);
		}
	}
#endif

	/* Return the final stack height to the caller */
	return stackHeight;
}

/*
 * Flush all registers to the CVM operand stack.
 */
static void FlushRegisterStack(MDUnroll *unroll)
{
	unroll->stackHeight = FlushRegisterStackNoUpdate(unroll);
	unroll->pseudoStackSize = 0;
	unroll->regsUsed = 0;
#if MD_HAS_FP
#if MD_HAS_FP_STACK
	unroll->fpStackSize = 0;
#else /* !MD_HAS_FP_STACK */
	unroll->fpRegsUsed = 0;
#endif /* !MD_HAS_FP_STACK */
#endif /* MD_HAS_FP */
}

/*
 * Fix the height of the CVM stack to match the cached location.
 */
static void FixStackHeight(MDUnroll *unroll)
{
	if(unroll->stackHeight > 0)
	{
		md_add_reg_imm(unroll->out, MD_REG_STACK, unroll->stackHeight);
	}
	else if(unroll->stackHeight < 0)
	{
		md_sub_reg_imm(unroll->out, MD_REG_STACK, -(unroll->stackHeight));
	}
}

/*
 * Restore special registers that were saved during execution.
 */
static void RestoreSpecialRegisters(MDUnroll *unroll)
{
	int index, reg;
	for(index = 15; index >= 0; --index)
	{
		reg = regAllocOrder[index];
		if(reg != -1 && (unroll->regsSaved & (1 << reg)) != 0)
		{
			md_pop_reg(unroll->out, reg);
		}
	}
}

#if !MD_STATE_ALREADY_IN_REGS

/*
 * Load the machine state into the required registers.
 */
static void LoadMachineState(MDUnroll *unroll)
{
	/* Save the values of the special registers */
	if((MD_SPECIAL_REGS_TO_BE_SAVED & (1 << MD_REG_PC)) != 0)
	{
		md_push_reg(unroll->out, MD_REG_PC);
	}
	if((MD_SPECIAL_REGS_TO_BE_SAVED & (1 << MD_REG_STACK)) != 0)
	{
		md_push_reg(unroll->out, MD_REG_STACK);
	}
	if((MD_SPECIAL_REGS_TO_BE_SAVED & (1 << MD_REG_FRAME)) != 0)
	{
		md_push_reg(unroll->out, MD_REG_FRAME);
	}

	/* Load stack and frame from the local variables in "_ILCVMInterpreter" */
	if(unroll->stackOffset)
	{
		md_load_membase_word_native(unroll->out, MD_REG_STACK, MD_REG_NATIVE_STACK, unroll->stackOffset);
	}
	if(unroll->frameOffset)
	{
		md_load_membase_word_native(unroll->out, MD_REG_FRAME, MD_REG_NATIVE_STACK, unroll->frameOffset);
	}
}

#else /* MD_STATE_ALREADY_IN_REGS */

#define	LoadMachineState(unroll)	do { ; } while (0)

#endif /* MD_STATE_ALREADY_IN_REGS */

/*
 * Unload the machine state and jump back into the CVM interpreter.
 */
static void UnloadMachineState(MDUnroll *unroll, unsigned char *pc, unsigned char **pcPtr,
							   unsigned char *label)
{
#if !MD_STATE_ALREADY_IN_REGS
	/* Store the stack and pc to the local variables in "_ILCVMInterpreter" */
	if(unroll->stackOffset)
	{
		md_store_membase_word_native(unroll->out, MD_REG_STACK, MD_REG_NATIVE_STACK, unroll->stackOffset);
	}

	/* Restore the previous values of the special registers */
	if((MD_SPECIAL_REGS_TO_BE_SAVED & (1 << MD_REG_FRAME)) != 0)
	{
		md_pop_reg(unroll->out, MD_REG_FRAME);
	}
	if((MD_SPECIAL_REGS_TO_BE_SAVED & (1 << MD_REG_STACK)) != 0)
	{
		md_pop_reg(unroll->out, MD_REG_STACK);
	}
	if((MD_SPECIAL_REGS_TO_BE_SAVED & (1 << MD_REG_PC)) != 0)
	{
		md_pop_reg(unroll->out, MD_REG_PC);
	}

	/* Jump back into the CVM interpreter */
	if(unroll->pcOffset)
	{
		md_jump_to_cvm_with_pc_offset(unroll->out, pc, unroll->pcOffset, label);
	}
	else if(pcPtr)
	{
		md_jump_to_cvm_with_pc_ptr(unroll->out, pc, pcPtr);
	}
	else
	{
		md_jump_to_cvm(unroll->out, pc, label);
	}
#else
	/* Jump back into the CVM interpreter */
	md_jump_to_cvm(unroll->out, pc, label);
#endif
}

/*
 * Perform an unconditional branch to a new location.  This is
 * also used when control falls out through the end of the block.
 * In that case, "pc" is the start of the following block.
 */
static void BranchToPC(MDUnroll *unroll, unsigned char *pc)
{
	/* Flush the register stack to the CVM stack */
	FlushRegisterStack(unroll);

	/* Update the REG_STACK register if necessary */
	FixStackHeight(unroll);
	unroll->stackHeight = 0;

	/* Restore the special registers that we used */
	RestoreSpecialRegisters(unroll);
	unroll->regsSaved = 0;

	/* Unload the machine state and jump back into the CVM interpreter */
	UnloadMachineState(unroll, pc, 0, 0);
}

/*
 * Re-execute the current instruction in the CVM interpreter,
 * to process exception conditions.
 */
static void ReExecute(MDUnroll *unroll, unsigned char *pc,
					  unsigned char *label)
{
	int finalHeight;

	/* Flush the register stack, but don't change it as we will
	   still need it further down the code */
	finalHeight = FlushRegisterStackNoUpdate(unroll);

	/* Fix up the stack height */
	if(finalHeight > 0)
	{
		md_add_reg_imm(unroll->out, MD_REG_STACK, finalHeight);
	}
	else if(finalHeight < 0)
	{
		md_sub_reg_imm(unroll->out, MD_REG_STACK, -finalHeight);
	}

	/* Restore the saved special registers */
	RestoreSpecialRegisters(unroll);

	/* Jump into the CVM interpreter to re-execute the instruction */
	UnloadMachineState(unroll, pc, 0, label);
}

/*
 * Flags for native vs 32-bit word register allocations.
 */
#define	MD_REG1_32BIT		0x0000
#define	MD_REG1_NATIVE		0x0001
#define	MD_REG2_32BIT		0x0000
#define	MD_REG2_NATIVE		0x0002
#define	MD_REG3_32BIT		0x0000
#define	MD_REG3_NATIVE		0x0004
#define	MD_REG4_32BIT		0x0000
#define	MD_REG4_NATIVE		0x0008
#define	MD_REGN_NATIVE		0x000F

/*
 * Get a register that can be used to store word values.
 */
static int GetWordRegister(MDUnroll *unroll, int flags)
{
	int index, reg, regmask, nativemask;

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Determine if we need a 32-bit or native word register */
	if((flags & MD_REGN_NATIVE) != 0)
	{
		nativemask = MD_NATIVE_REG_MASK;
	}
	else
	{
		nativemask = 0;
	}

	/* Search for a free register */
	for(index = 0; index < 16 && regAllocOrder[index] != -1; ++index)
	{
		reg = regAllocOrder[index];
		regmask = (1 << reg);
		if((unroll->regsUsed & regmask) == 0)
		{
			unroll->pseudoStack[(unroll->pseudoStackSize)++]
					= reg | nativemask;
			unroll->regsUsed |= regmask;
			if((regmask & MD_REGS_TO_BE_SAVED) != 0 &&
			   (unroll->regsSaved & regmask) == 0)
			{
				/* Save a special register on the system stack */
				md_push_reg(unroll->out, reg);
				unroll->regsSaved |= regmask;
			}
			return reg;
		}
	}

	/*
	 * Spill the bottom-most registers to the CVM stack until we find a word
	 * and reuse it.
	 */
	reg = unroll->pseudoStack[0];
#if MD_HAS_FP
#if MD_HAS_FP_STACK
	if(MD_IS_FREG(reg))
	{
		/* We have an FP register at the bottom, so flush everything */
		FlushRegisterStack(unroll);

		/* Restart with the first word register */
		reg = regAllocOrder[0];
		unroll->regsUsed |= (1 << reg);
		unroll->pseudoStack[(unroll->pseudoStackSize)++] = reg | nativemask;
		return reg;
	}
#else /* !MD_HAS_FP_STACK */
	while(MD_IS_FREG(reg))
	{
		md_store_membase_float_native
			(unroll->out, reg, MD_REG_STACK, unroll->stackHeight);
		unroll->stackHeight += (sizeof(CVMWord) * CVM_WORDS_PER_NATIVE_FLOAT);
		unroll->fpRegsUsed &= ~(1 << (reg & ~MD_FREG_MASK));
		for(index = 1; index < unroll->pseudoStackSize; ++index)
		{
			unroll->pseudoStack[index - 1] = unroll->pseudoStack[index];
		}
		--(unroll->pseudoStackSize);
		reg = unroll->pseudoStack[0];
	}
#endif /* !MD_HAS_FP_STACK */
#endif /* MD_HAS_FP */
	if(MD_IS_NATIVE_REG(reg))
	{
		reg &= ~MD_NATIVE_REG_MASK;
		md_store_membase_word_native
			(unroll->out, reg, MD_REG_STACK, unroll->stackHeight);
	}
	else
	{
		md_store_membase_word_32
			(unroll->out, reg, MD_REG_STACK, unroll->stackHeight);
	}
	unroll->stackHeight += sizeof(CVMWord);
	for(index = 1; index < unroll->pseudoStackSize; ++index)
	{
		unroll->pseudoStack[index - 1] = unroll->pseudoStack[index];
	}
	unroll->pseudoStack[unroll->pseudoStackSize - 1] = reg | nativemask;
	return reg;
}

/*
 * Get a 32-bit word register, but try to reuse the previously
 * popped register if the local variable offsets are identical.
 * This is used to optimise the common case of storing to a local
 * and then immediately reloading it.  Returns -1 if the register
 * was reused.
 */
static int GetCachedWordRegister(MDUnroll *unroll, long local, int flags)
{
	int nativemask;

	/* Determine if we need a 32-bit or native word register */
	if((flags & MD_REGN_NATIVE) != 0)
	{
		nativemask = MD_NATIVE_REG_MASK;
	}
	else
	{
		nativemask = 0;
	}

	/* Check for cached registers */
	if(unroll->cachedLocal == local)
	{
		/* Push the previous register back onto the stack */
		int reg = unroll->cachedReg;
		unroll->pseudoStack[(unroll->pseudoStackSize)++] = reg | nativemask;
		unroll->regsUsed |= (1 << reg);

		/* We can only do this once: use a new register if the
		   variable is loaded again */
		unroll->cachedLocal = -1;
		unroll->cachedReg = -1;

		/* Tell the caller that the top-most register was reused */
		return -1;
	}
	else
	{
		return GetWordRegister(unroll, flags);
	}
}

#ifdef MD_HAS_FP

/*
 * Check to see if the word register array/stack is full.
 * If it is, then flush the entire register stack.
 */
static void CheckWordFull(MDUnroll *unroll)
{
	int index, reg, regmask;

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Search for an unused word register */
	for(index = 0; index < 16 && regAllocOrder[index] != -1; ++index)
	{
		reg = regAllocOrder[index];
		regmask = (1 << reg);
		if((unroll->regsUsed & regmask) == 0)
		{
			return;
		}
	}

	/* Flush the entire register stack */
	FlushRegisterStack(unroll);
}

/*
 * Check to see if the floating-point register array/stack is full.
 * If it is, then flush the entire register stack.
 */
static void CheckFPFull(MDUnroll *unroll)
{
#if MD_HAS_FP_STACK
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* If the FPU stack is full, then flush and restart */
	if(unroll->fpStackSize >= MD_FP_STACK_SIZE)
	{
		FlushRegisterStack(unroll);
	}
#else /* !MD_HAS_FP_STACK */
	int index, reg, regmask;

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Search for an unused floating-point register */
	for(index = 0; index < 16 && regAllocFPOrder[index] != -1; ++index)
	{
		reg = regAllocFPOrder[index];
		regmask = (1 << (reg & ~MD_FREG_MASK));
		if((unroll->fpRegsUsed & regmask) == 0)
		{
			return;
		}
	}

	/* Flush the entire register stack */
	FlushRegisterStack(unroll);
#endif  /* !MD_HAS_FP_STACK */
}

/*
 * Get a register that can be used to store floating-point values.
 */
static int GetFPRegister(MDUnroll *unroll)
{
#if MD_HAS_FP_STACK
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* If the FPU stack is full, then flush and restart */
	if(unroll->fpStackSize >= MD_FP_STACK_SIZE)
	{
		FlushRegisterStack(unroll);
	}

	/* Allocate the top of stack for the caller.  The register number
	   is irrelevant in this case, so always return register 0 */
	++(unroll->fpStackSize);
	unroll->pseudoStack[(unroll->pseudoStackSize)++] = MD_FREG_0;
	return MD_FREG_0;
#else /* !MD_HAS_FP_STACK */
	int index, reg, regmask;

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Search for an unused floating-point register */
	for(index = 0; index < 16 && regAllocFPOrder[index] != -1; ++index)
	{
		reg = regAllocFPOrder[index];
		regmask = (1 << (reg & ~MD_FREG_MASK));
		if((unroll->fpRegsUsed & regmask) == 0)
		{
			unroll->fpRegsUsed |= regmask;
			unroll->pseudoStack[(unroll->pseudoStackSize)++] = reg;
			return reg;
		}
	}

	/*
	 * Spill the bottom-most registers to the CVM stack until we find a
	 * floatingpoint register and reuse it.
	 */
	reg = unroll->pseudoStack[0];
	while(!MD_IS_FREG(reg))
	{
		if(MD_IS_NATIVE_REG(reg))
		{
			reg &= ~MD_NATIVE_REG_MASK;
			md_store_membase_word_native
				(unroll->out, reg, MD_REG_STACK, unroll->stackHeight);
		}
		else
		{
			md_store_membase_word_32
				(unroll->out, reg, MD_REG_STACK, unroll->stackHeight);
		}
		unroll->stackHeight += sizeof(CVMWord);
		for(index = 1; index < unroll->pseudoStackSize; ++index)
		{
			unroll->pseudoStack[index - 1] = unroll->pseudoStack[index];
		}
		--(unroll->pseudoStackSize);
		unroll->regsUsed &= ~reg;
		reg = unroll->pseudoStack[0];
	}
	md_store_membase_float_native
			(unroll->out, reg, MD_REG_STACK, unroll->stackHeight);
	unroll->stackHeight += (sizeof(CVMWord) * CVM_WORDS_PER_NATIVE_FLOAT);
	for(index = 1; index < unroll->pseudoStackSize; ++index)
	{
		unroll->pseudoStack[index - 1] = unroll->pseudoStack[index];
	}
	unroll->pseudoStack[unroll->pseudoStackSize - 1] = reg;
	return reg;
#endif /* !MD_HAS_FP_STACK */
}

#endif /* MD_HAS_FP */

/*
 * Change the type of the top-most register on the stack.
 */
static void ChangeRegisterType(MDUnroll *unroll, int type)
{
	if((type & MD_REGN_NATIVE) != 0)
	{
		unroll->pseudoStack[unroll->pseudoStackSize - 1] |= MD_NATIVE_REG_MASK;
	}
	else
	{
		unroll->pseudoStack[unroll->pseudoStackSize - 1] &= ~MD_NATIVE_REG_MASK;
	}
}

/*
 * Get the top-most word value on the stack into a register.
 */
static int GetTopWordRegister(MDUnroll *unroll, int flags)
{
	int reg;

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Check for an existing word register on the stack */
	if(unroll->pseudoStackSize > 0)
	{
		reg = unroll->pseudoStack[unroll->pseudoStackSize - 1];
		if(!MD_IS_FREG(reg))
		{
			return (reg & ~MD_NATIVE_REG_MASK);
		}
		FlushRegisterStack(unroll);
	}

	/* Load the top of the CVM stack into the first register */
	reg = regAllocOrder[0];
	unroll->regsUsed |= (1 << reg);
	unroll->pseudoStackSize = 1;
	unroll->stackHeight -= sizeof(CVMWord);
	if((flags & MD_REGN_NATIVE) != 0)
	{
		unroll->pseudoStack[0] = reg | MD_NATIVE_REG_MASK;
		md_load_membase_word_native(unroll->out, reg, MD_REG_STACK,
									unroll->stackHeight);
	}
	else
	{
		unroll->pseudoStack[0] = reg;
		md_load_membase_word_32(unroll->out, reg, MD_REG_STACK,
								unroll->stackHeight);
	}
	return reg;
}

/*
 * Roll a word register off the CVM operand stack.  This will only
 * be called if we know that the operand stack only contains word
 * registers and we need to reload some additional values.
 */
static int RollRegisterStack(MDUnroll *unroll, int flags, int preferred)
{
	int reg, nativemask, index;

	/* Determine if the register is native or regular */
	if((flags & MD_REGN_NATIVE) != 0)
	{
		nativemask = MD_NATIVE_REG_MASK;
	}
	else
	{
		nativemask = 0;
	}

	/* Allocate a word register, using the preferred one if possible */
	if(preferred != -1 && (unroll->regsUsed & (1 << preferred)) == 0)
	{
		reg = preferred;
		unroll->regsUsed |= (1 << reg);
		unroll->pseudoStack[(unroll->pseudoStackSize)++] = reg | nativemask;
	}
	else
	{
		reg = GetWordRegister(unroll, flags);
	}

	/* Roll the pseudo stack to shift the new register into the right place */
	for(index = unroll->pseudoStackSize - 1; index > 0; --index)
	{
		unroll->pseudoStack[index] = unroll->pseudoStack[index - 1];
	}
	unroll->pseudoStack[0] = reg | nativemask;

	/* Load the top of the CVM stack into the register */
	unroll->stackHeight -= sizeof(CVMWord);
	if((flags & MD_REGN_NATIVE) != 0)
	{
		md_load_membase_word_native(unroll->out, reg, MD_REG_STACK,
									unroll->stackHeight);
	}
	else
	{
		md_load_membase_word_32(unroll->out, reg, MD_REG_STACK,
								unroll->stackHeight);
	}
	return reg;
}

/*
 * Get the two top-most word values on the stack into registers.
 * "reg1" will be the lower of the two.
 */
static void GetTopTwoWordRegisters(MDUnroll *unroll, int *reg1,
								   int *reg2, int flags)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* See if we already have two word registers in play */
	if(unroll->pseudoStackSize > 1)
	{
		*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
		if(!MD_IS_FREG(*reg1))
		{
			*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
			if(!MD_IS_FREG(*reg2))
			{
				*reg1 &= ~MD_NATIVE_REG_MASK;
				*reg2 &= ~MD_NATIVE_REG_MASK;
				return;
			}
		}
	}

	/* See if we have one word register in play */
	if(unroll->pseudoStackSize == 1)
	{
		*reg2 = unroll->pseudoStack[0];
		if(!MD_IS_FREG(*reg2))
		{
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			*reg2 &= ~MD_NATIVE_REG_MASK;
			return;
		}
	}

	/* We may have an FP register in play, so flush it */
	FlushRegisterStack(unroll);

	/* Load the top of the CVM stack into the first two registers */
	RollRegisterStack(unroll, flags & MD_REG2_NATIVE, regAllocOrder[1]);
	RollRegisterStack(unroll, flags & MD_REG1_NATIVE, regAllocOrder[0]);
	*reg1 = (unroll->pseudoStack[0] & ~MD_NATIVE_REG_MASK);
	*reg2 = (unroll->pseudoStack[1] & ~MD_NATIVE_REG_MASK);
}

/*
 * Get the three top-most word values on the stack into registers.
 * "reg1" will be the lowest of the three.
 */
static void GetTopThreeWordRegisters(MDUnroll *unroll,
									 int *reg1, int *reg2,
									 int *reg3, int flags)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* See if we already have three word registers in play */
	if(unroll->pseudoStackSize > 2)
	{
		*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 3];
		if(!MD_IS_FREG(*reg1))
		{
			*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
			if(!MD_IS_FREG(*reg2))
			{
				*reg3 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
				if(!MD_IS_FREG(*reg3))
				{
					*reg1 &= ~MD_NATIVE_REG_MASK;
					*reg2 &= ~MD_NATIVE_REG_MASK;
					*reg3 &= ~MD_NATIVE_REG_MASK;
					return;
				}
			}
		}
	}

	/* See if we have two word registers in play */
	if(unroll->pseudoStackSize == 2)
	{
		*reg2 = unroll->pseudoStack[0];
		*reg3 = unroll->pseudoStack[1];
		if(!MD_IS_FREG(*reg2) && !MD_IS_FREG(*reg3))
		{
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			*reg2 &= ~MD_NATIVE_REG_MASK;
			*reg3 &= ~MD_NATIVE_REG_MASK;
			return;
		}
	}

	/* See if we have one word register in play */
	if(unroll->pseudoStackSize == 1)
	{
		*reg3 = unroll->pseudoStack[0];
		if(!MD_IS_FREG(*reg3))
		{
			*reg3 &= ~MD_NATIVE_REG_MASK;
			*reg2 = RollRegisterStack(unroll, flags & MD_REG2_NATIVE, -1);
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			return;
		}
	}

	/* We may have an FP register in play, so flush it */
	FlushRegisterStack(unroll);

	/* Load the top of the CVM stack into the first three registers */
	RollRegisterStack(unroll, flags & MD_REG3_NATIVE, regAllocOrder[2]);
	RollRegisterStack(unroll, flags & MD_REG2_NATIVE, regAllocOrder[1]);
	RollRegisterStack(unroll, flags & MD_REG1_NATIVE, regAllocOrder[0]);
	*reg1 = (unroll->pseudoStack[0] & ~MD_NATIVE_REG_MASK);
	*reg2 = (unroll->pseudoStack[1] & ~MD_NATIVE_REG_MASK);
	*reg3 = (unroll->pseudoStack[2] & ~MD_NATIVE_REG_MASK);
}

#ifdef IL_NATIVE_INT32

/*
 * Get the four top-most word values on the stack into registers.
 * "reg1" will be the lowest of the four.
 */
static void GetTopFourWordRegisters(MDUnroll *unroll,
									int *reg1, int *reg2,
									int *reg3, int *reg4,
									int flags)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* See if we already have four word registers in play */
	if(unroll->pseudoStackSize > 3)
	{
		*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 4];
		if(!MD_IS_FREG(*reg1))
		{
			*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 3];
			if(!MD_IS_FREG(*reg2))
			{
				*reg3 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
				if(!MD_IS_FREG(*reg3))
				{
					*reg4 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
					if(!MD_IS_FREG(*reg4))
					{
						*reg1 &= ~MD_NATIVE_REG_MASK;
						*reg2 &= ~MD_NATIVE_REG_MASK;
						*reg3 &= ~MD_NATIVE_REG_MASK;
						*reg4 &= ~MD_NATIVE_REG_MASK;
						return;
					}
				}
			}
		}
	}

	/* See if we have three word registers in play */
	if(unroll->pseudoStackSize == 3)
	{
		*reg2 = unroll->pseudoStack[0];
		*reg3 = unroll->pseudoStack[1];
		*reg4 = unroll->pseudoStack[2];
		if(!MD_IS_FREG(*reg2) && !MD_IS_FREG(*reg3) && !MD_IS_FREG(*reg4))
		{
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			*reg2 &= ~MD_NATIVE_REG_MASK;
			*reg3 &= ~MD_NATIVE_REG_MASK;
			*reg4 &= ~MD_NATIVE_REG_MASK;
			return;
		}
	}

	/* See if we have two word registers in play */
	if(unroll->pseudoStackSize == 2)
	{
		*reg3 = unroll->pseudoStack[0];
		*reg4 = unroll->pseudoStack[1];
		if(!MD_IS_FREG(*reg3) && !MD_IS_FREG(*reg4))
		{
			*reg4 &= ~MD_NATIVE_REG_MASK;
			*reg3 &= ~MD_NATIVE_REG_MASK;
			*reg2 = RollRegisterStack(unroll, flags & MD_REG2_NATIVE, -1);
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			return;
		}
	}

	/* See if we have one word register in play */
	if(unroll->pseudoStackSize == 1)
	{
		*reg4 = unroll->pseudoStack[0];
		if(!MD_IS_FREG(*reg4))
		{
			*reg4 &= ~MD_NATIVE_REG_MASK;
			*reg3 = RollRegisterStack(unroll, flags & MD_REG3_NATIVE, -1);
			*reg2 = RollRegisterStack(unroll, flags & MD_REG2_NATIVE, -1);
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			return;
		}
	}

	/* We may have an FP register in play, so flush it */
	FlushRegisterStack(unroll);

	/* Load the top of the CVM stack into the first four registers */
	RollRegisterStack(unroll, flags & MD_REG4_NATIVE, regAllocOrder[3]);
	RollRegisterStack(unroll, flags & MD_REG3_NATIVE, regAllocOrder[2]);
	RollRegisterStack(unroll, flags & MD_REG2_NATIVE, regAllocOrder[1]);
	RollRegisterStack(unroll, flags & MD_REG1_NATIVE, regAllocOrder[0]);
	*reg1 = (unroll->pseudoStack[0] & ~MD_NATIVE_REG_MASK);
	*reg2 = (unroll->pseudoStack[1] & ~MD_NATIVE_REG_MASK);
	*reg3 = (unroll->pseudoStack[2] & ~MD_NATIVE_REG_MASK);
	*reg4 = (unroll->pseudoStack[3] & ~MD_NATIVE_REG_MASK);
}

#endif /* IL_NATIVE_INT32 */

#ifdef MD_HAS_FP

/*
 * Get the top-most stack value into a floating-point register.
 */
static int GetTopFPRegister(MDUnroll *unroll)
{
	int reg;

	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Check for an existing FP register on the stack */
	if(unroll->pseudoStackSize > 0)
	{
		reg = unroll->pseudoStack[unroll->pseudoStackSize - 1];
		if(MD_IS_FREG(reg))
		{
			return reg;
		}
	}

	/* Flush the register stack and then reload the top into register 0.
	   If the FPU is stack-based, then the register number is ignored */
	FlushRegisterStack(unroll);
	unroll->pseudoStack[0] = MD_FREG_0;
	unroll->pseudoStackSize = 1;
	unroll->stackHeight -= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord);
	md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
								 unroll->stackHeight);
#if MD_HAS_FP_STACK
	++(unroll->fpStackSize);
#else
	unroll->fpRegsUsed |= (1 << (MD_FREG_0 & ~MD_FREG_MASK));
#endif
	return MD_FREG_0;
}

/*
 * Get the two top-most FP values on the stack into registers.
 * If "fprem" is non-zero, then reverse the order of the top
 * two registers on an FPU stack for a "COP_FREM" operation.
 * Used to work around an oddity in the FPU on x86 platforms.
 */
static void GetTopTwoFPRegisters(MDUnroll *unroll, int *reg1,
								 int *reg2, int fprem)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* See if we already have two FP registers in play */
	if(unroll->pseudoStackSize > 1)
	{
		if(MD_IS_FREG(unroll->pseudoStack[unroll->pseudoStackSize - 2]) &&
		   MD_IS_FREG(unroll->pseudoStack[unroll->pseudoStackSize - 1]))
		{
#if defined(CVM_X86) || defined(CVM_X86_64)
			if(fprem)
			{
				md_freg_swap(unroll->out);
			}
#endif
			*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
			*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
			return;
		}
	}

#if MD_HAS_FP_STACK

	/* See if we have one FP register in play */
	if(unroll->pseudoStackSize == 1)
	{
		if(MD_IS_FREG(unroll->pseudoStack[0]))
		{
			/* Load the top of the CVM stack onto the FP stack */
			unroll->stackHeight
				-= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord);
			md_load_membase_float_native
				(unroll->out, MD_FREG_0, MD_REG_STACK, unroll->stackHeight);

			/* Put the values into the correct order */
#if defined(CVM_X86) || defined(CVM_X86_64)
			if(!fprem)
#endif
			{
				md_freg_swap(unroll->out);
			}

			/* We now have an extra FP value in the register stack */
			unroll->pseudoStack[1] = MD_FREG_0;
			unroll->pseudoStackSize = 2;
			++(unroll->fpStackSize);
			*reg1 = MD_FREG_0;
			*reg2 = MD_FREG_0;
			return;
		}
	}

	/* We may have word registers in play, so flush them */
	FlushRegisterStack(unroll);

	/* Load the top of the CVM stack onto the FP stack */
	unroll->stackHeight -= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord) * 2;
#if defined(CVM_X86) || defined(CVM_X86_64)
	if(fprem)
	{
		md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
							  		 unroll->stackHeight +
							  			CVM_WORDS_PER_NATIVE_FLOAT *
											sizeof(CVMWord));
		md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
							  		 unroll->stackHeight);
	}
	else
#endif
	{
		md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
							  		 unroll->stackHeight);
		md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
							  		 unroll->stackHeight +
							  			CVM_WORDS_PER_NATIVE_FLOAT *
											sizeof(CVMWord));
	}
	unroll->pseudoStack[0] = MD_FREG_0;
	unroll->pseudoStack[1] = MD_FREG_0;
	unroll->pseudoStackSize = 2;
	unroll->fpStackSize = 2;
	*reg1 = MD_FREG_0;
	*reg2 = MD_FREG_0;

#else /* !MD_HAS_FP_STACK */

	/* See if we have one FP register in play */
	if(unroll->pseudoStackSize == 1)
	{
		if(MD_IS_FREG(unroll->pseudoStack[0]))
		{
			*reg2 = unroll->pseudoStack[0];
			*reg1 = GetFPRegister(unroll);

			/* Load the top of the CVM stack into reg1 */
			unroll->stackHeight
				-= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord);
			md_load_membase_float_native
				(unroll->out, *reg1, MD_REG_STACK, unroll->stackHeight);

			/* Swap the order of registers to match reality */
			unroll->pseudoStack[0] = *reg1;
			unroll->pseudoStack[1] = *reg2;
			return;
		}
	}

	/* We may have word registers in play, so flush them */
	FlushRegisterStack(unroll);

	/* Load the top of the CVM stack into the first two FP registers */
	*reg1 = GetFPRegister(unroll);
	*reg2 = GetFPRegister(unroll);
	unroll->stackHeight -= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord) * 2;
	md_load_membase_float_native(unroll->out, *reg1, MD_REG_STACK,
						  		 unroll->stackHeight);
	md_load_membase_float_native(unroll->out, *reg2, MD_REG_STACK,
						  		 unroll->stackHeight +
						  			CVM_WORDS_PER_NATIVE_FLOAT *
										sizeof(CVMWord));

#endif /* !MD_HAS_FP_STACK */
}

/*
 * Get the two top-most stack values in a word and an FP register.
 */
static void GetWordAndFPRegisters(MDUnroll *unroll, int *reg1, int *reg2)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Check for the expected information on the stack */
	if(unroll->pseudoStackSize > 1)
	{
		*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
		*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
		if(!MD_IS_FREG(*reg1) && MD_IS_FREG(*reg2))
		{
			*reg1 &= ~MD_NATIVE_REG_MASK;
			return;
		}
	}

	/* If we have 1 FP value on the stack, then load the word into MD_REG_0 */
	if(unroll->pseudoStackSize == 1 && MD_IS_FREG(unroll->pseudoStack[0]))
	{
		*reg1 = MD_REG_0;
		*reg2 = unroll->pseudoStack[0];
		unroll->pseudoStack[0] = *reg1 | MD_NATIVE_REG_MASK;
		unroll->pseudoStack[1] = *reg2;
		unroll->pseudoStackSize = 2;
		unroll->stackHeight -= sizeof(CVMWord);
		md_load_membase_word_native(unroll->out, MD_REG_0, MD_REG_STACK,
						            unroll->stackHeight);
		unroll->regsUsed |= (1 << MD_REG_0);
		return;
	}

	/* Flush the register stack and then reload into MD_REG_0 and MD_FREG_0 */
	FlushRegisterStack(unroll);
	*reg1 = MD_REG_0;
	*reg2 = MD_FREG_0;
	unroll->pseudoStack[0] = MD_REG_0 | MD_NATIVE_REG_MASK;
	unroll->pseudoStack[1] = MD_FREG_0;
	unroll->pseudoStackSize = 2;
	unroll->stackHeight -= (CVM_WORDS_PER_NATIVE_FLOAT + 1) * sizeof(CVMWord);
	md_load_membase_word_native(unroll->out, MD_REG_0, MD_REG_STACK,
					    		unroll->stackHeight);
	md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
								 unroll->stackHeight + sizeof(CVMWord));
	unroll->regsUsed |= (1 << MD_REG_0);
#if MD_HAS_FP_STACK
	++(unroll->fpStackSize);
#else
	unroll->fpRegsUsed |= (1 << (MD_FREG_0 & ~MD_FREG_MASK));
#endif
}

/*
 * Get the two top-most stack values in an FP and a word register.
 */
static void GetFPAndWordRegisters(MDUnroll *unroll, int *reg1, int *reg2)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* Check for the expected information on the stack */
	if(unroll->pseudoStackSize > 1)
	{
		*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
		*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
		if(MD_IS_FREG(*reg1) && !MD_IS_FREG(*reg2))
		{
			*reg2 &= ~MD_NATIVE_REG_MASK;
			return;
		}
	}

	/* If we have 1 word value on the stack, then load the FP into MD_FREG_0 */
	if(unroll->pseudoStackSize == 1 && !MD_IS_FREG(unroll->pseudoStack[0]))
	{
		*reg1 = MD_FREG_0;
		*reg2 = unroll->pseudoStack[0] & ~MD_NATIVE_REG_MASK;
		unroll->pseudoStack[0] = *reg1;
		unroll->pseudoStack[1] = *reg2 | MD_NATIVE_REG_MASK;
		unroll->pseudoStackSize = 2;
		unroll->stackHeight -= CVM_WORDS_PER_NATIVE_FLOAT * sizeof(CVMWord);
		md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
						             unroll->stackHeight);
	#if MD_HAS_FP_STACK
		++(unroll->fpStackSize);
	#else
		unroll->fpRegsUsed |= (1 << (MD_FREG_0 & ~MD_FREG_MASK));
	#endif
		return;
	}

	/* Flush the register stack and then reload into MD_FREG_0 and MD_REG_0 */
	FlushRegisterStack(unroll);
	*reg1 = MD_FREG_0;
	*reg2 = MD_REG_0;
	unroll->pseudoStack[0] = MD_FREG_0;
	unroll->pseudoStack[1] = MD_REG_0 | MD_NATIVE_REG_MASK;
	unroll->pseudoStackSize = 2;
	unroll->stackHeight -= (CVM_WORDS_PER_NATIVE_FLOAT + 1) * sizeof(CVMWord);
	md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
								 unroll->stackHeight);
	md_load_membase_word_native(unroll->out, MD_REG_0, MD_REG_STACK,
					    		unroll->stackHeight +
									CVM_WORDS_PER_NATIVE_FLOAT *
									sizeof(CVMWord));
	unroll->regsUsed |= (1 << MD_REG_0);
#if MD_HAS_FP_STACK
	++(unroll->fpStackSize);
#else
	unroll->fpRegsUsed |= (1 << (MD_FREG_0 & ~MD_FREG_MASK));
#endif
}

/*
 * Get the three top-most stack values in two word and a FP register.
 * "reg1" will be the lowest of the three.
 */
static void GetTopTwoWordAndFPRegisters(MDUnroll *unroll,
										int *reg1, int *reg2,
										int *reg3, int flags)
{
	/* Clear the cached local information */
	unroll->cachedLocal = -1;
	unroll->cachedReg = -1;

	/* See if we already have two word and one fp register in play */
	if(unroll->pseudoStackSize > 2)
	{
		*reg1 = unroll->pseudoStack[unroll->pseudoStackSize - 3];
		if(!MD_IS_FREG(*reg1))
		{
			*reg2 = unroll->pseudoStack[unroll->pseudoStackSize - 2];
			if(!MD_IS_FREG(*reg2))
			{
				*reg3 = unroll->pseudoStack[unroll->pseudoStackSize - 1];
				if(MD_IS_FREG(*reg3))
				{
					*reg1 &= ~MD_NATIVE_REG_MASK;
					*reg2 &= ~MD_NATIVE_REG_MASK;
					return;
				}
			}
		}
	}

	/* See if we have one FP and one word registers in play */
	if(unroll->pseudoStackSize == 2)
	{
		*reg2 = unroll->pseudoStack[0];
		*reg3 = unroll->pseudoStack[1];
		if(!MD_IS_FREG(*reg2) && MD_IS_FREG(*reg3))
		{
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			*reg2 &= ~MD_NATIVE_REG_MASK;
			return;
		}
	}

	/* See if we have one FP register in play */
	if(unroll->pseudoStackSize == 1)
	{
		*reg3 = unroll->pseudoStack[0];
		if(MD_IS_FREG(*reg3))
		{
			*reg2 = RollRegisterStack(unroll, flags & MD_REG2_NATIVE, -1);
			*reg1 = RollRegisterStack(unroll, flags & MD_REG1_NATIVE, -1);
			return;
		}
	}

	/* Flush the register stack and then reload into MD_REG_0, MD_REG_1 and MD_FREG_0 */
	FlushRegisterStack(unroll);
	*reg1 = MD_REG_0;
	*reg2 = MD_REG_1;
	*reg3 = MD_FREG_0;
	unroll->pseudoStack[0] = MD_REG_0 | MD_NATIVE_REG_MASK;
	unroll->pseudoStack[1] = MD_REG_1 | MD_NATIVE_REG_MASK;
	unroll->pseudoStack[2] = MD_FREG_0;
	unroll->pseudoStackSize = 3;
	unroll->stackHeight -= (CVM_WORDS_PER_NATIVE_FLOAT + 2) * sizeof(CVMWord);
	md_load_membase_word_native(unroll->out, MD_REG_0, MD_REG_STACK,
					    		unroll->stackHeight);
	md_load_membase_word_native(unroll->out, MD_REG_1, MD_REG_STACK,
					    		unroll->stackHeight + sizeof(CVMWord));
	md_load_membase_float_native(unroll->out, MD_FREG_0, MD_REG_STACK,
								 unroll->stackHeight + 2 * sizeof(CVMWord));
	unroll->regsUsed |= ((1 << MD_REG_0) | (1 << MD_REG_1));
#if MD_HAS_FP_STACK
	++(unroll->fpStackSize);
#else
	unroll->fpRegsUsed |= (1 << (MD_FREG_0 & ~MD_FREG_MASK));
#endif
}

#endif /* MD_HAS_FP */

/*
 * Free the top-most register on the pseudo stack, and record the
 * local variable that it was just stored in (-1 if no local).
 */
static void FreeTopRegister(MDUnroll *unroll, long local)
{
	int reg = unroll->pseudoStack[--(unroll->pseudoStackSize)];
	reg &= ~MD_NATIVE_REG_MASK;
	unroll->cachedLocal = local;
	unroll->cachedReg = reg;
	if(!MD_IS_FREG(reg))
	{
		unroll->regsUsed &= ~(1 << reg);
	}
	else
	{
#if MD_HAS_FP
#if MD_HAS_FP_STACK
		--(unroll->fpStackSize);
#else
		unroll->fpRegsUsed &= ~(1 << (reg & ~MD_FREG_MASK));
#endif
#endif
	}
}

/*
 * Peek at the top-most word register on the stack.  Returns
 * -1 if the top-most stack value is not in a word register.
 */
static int PeekTopWordRegister(MDUnroll *unroll)
{
	if(unroll->pseudoStackSize > 0)
	{
		int reg = unroll->pseudoStack[unroll->pseudoStackSize - 1];
		if(!MD_IS_FREG(reg))
		{
			unroll->cachedLocal = -1;
			unroll->cachedReg = -1;
			return reg & ~MD_NATIVE_REG_MASK;
		}
	}
	return -1;
}

/*
 * Peek at the top-most word register flags on the stack.
 */
static int GetTopRegisterFlags(MDUnroll *unroll)
{
	if(unroll->pseudoStackSize > 0)
	{
		int reg = unroll->pseudoStack[unroll->pseudoStackSize - 1];
		if(!MD_IS_FREG(reg))
		{
			if(MD_IS_NATIVE_REG(reg))
			{
				return MD_REG1_NATIVE;
			}
		}
	}
	return 0;
}

#ifdef MD_HAS_FP

/*
 * Push a register onto the stack directly.
 */
static void PushRegister(MDUnroll *unroll, int reg, int flags)
{
	if((flags & MD_REGN_NATIVE) != 0)
	{
		unroll->pseudoStack[(unroll->pseudoStackSize)++]
			= reg | MD_NATIVE_REG_MASK;
	}
	else
	{
		unroll->pseudoStack[(unroll->pseudoStackSize)++] = reg;
	}
	if(MD_IS_FREG(reg))
	{
#if MD_HAS_FP_STACK
		++(unroll->fpStackSize);
#else
		unroll->fpRegsUsed |= (1 << (reg & ~MD_FREG_MASK));
#endif
	}
	else
	{
		unroll->regsUsed |= (1 << reg);
	}
}

#endif /* MD_HAS_FP */

/*
 * Start an unrolled code section if necessary.
 */
#define	UNROLL_START()	\
			do { \
				if(!inUnrollBlock) \
				{ \
					overwritePC = pc; \
					unrollStart = unroll.out; \
					inUnrollBlock = 1; \
					LoadMachineState(&unroll); \
				} \
			} while (0)

/*
 * If an unrolled code section is in progress, then continue,
 * otherwise don't unroll the current instruction.  This is
 * typically used for branch instructions which should not be
 * unrolled at the start of a basic block.
 */
#define	UNROLL_BRANCH_START()	\
			do { \
				if(!inUnrollBlock) \
				{ \
					goto defaultCase; \
				} \
			} while (0)

/*
 * Flush the current unrolled code section.  Note: we assume
 * that the write to "overwritePC" will be more or less atomic
 * so that any other threads in the system that are executing
 * the method will instantly thread through to the new code
 * when execution returns to "overwritePC".
 */
#define	UNROLL_FLUSH()	\
			do { \
				*((void **)overwritePC) = unrollStart; \
				inUnrollBlock = 0; \
				unroll.cachedLocal = -1; \
				unroll.thisValidated = 0; \
			} while (0)

/*
 * Modify the "pc" variable to account for an instruction length.
 */
#define	MODIFY_UNROLL_PC(len)	\
			do { \
				pc += (len); \
			} while (0)

/*
 * Table that maps direct instruction pointers into opcodes.
 */
static unsigned short *ptrToOpcode;
static unsigned char *minPtrValue;
static unsigned char *maxPtrValue;

int _ILCVMUnrollInit(void)
{
	void *minPtr;
	void *maxPtr;
	void *tempPtr;
	int index;
	int size;

	/* Get the minimum and maximum label pointers from the CVM interpreter */
	minPtr = CVM_LABEL_FOR_OPCODE(0);
	maxPtr = minPtr;
	for(index = 1; index < 256; ++index)
	{
		tempPtr = CVM_LABEL_FOR_OPCODE(index);
		if(tempPtr < minPtr)
		{
			minPtr = tempPtr;
		}
		if(tempPtr > maxPtr)
		{
			maxPtr = tempPtr;
		}
	}
	for(index = 0; index < 256; ++index)
	{
		tempPtr = CVMP_LABEL_FOR_OPCODE(index);
		if(tempPtr < minPtr)
		{
			minPtr = tempPtr;
		}
		if(tempPtr > maxPtr)
		{
			maxPtr = tempPtr;
		}
	}

	/* Allocate space for the table and initialize to "nop" */
	size = (int)(((unsigned char *)maxPtr) - ((unsigned char *)minPtr) + 1);
	if((ptrToOpcode = (unsigned short *)ILMalloc(size * 2)) == 0)
	{
		return 0;
	}
	ILMemZero(ptrToOpcode, size * 2);

	/* Populate the mapping table */
	for(index = 0; index < 256; ++index)
	{
		tempPtr = CVM_LABEL_FOR_OPCODE(index);
		ptrToOpcode[((unsigned char *)tempPtr) - ((unsigned char *)minPtr)]
				= index;
	}
	for(index = 0; index < 256; ++index)
	{
		tempPtr = CVMP_LABEL_FOR_OPCODE(index);
		ptrToOpcode[((unsigned char *)tempPtr) - ((unsigned char *)minPtr)]
				= 0x100 + index;
	}
	minPtrValue = (unsigned char *)minPtr;
	maxPtrValue = (unsigned char *)maxPtr;

	/* Ready to go */
	return 1;
}

int _ILCVMUnrollPossible(void)
{
	return 1;
}

/*
 * Minimum buffer size.  If the amount of memory in the memory
 * cache falls below this value, then we stop the unroll process.
 * This value should be big enough to handle the largest block
 * of x86 code that may be output by an unroll case followed
 * by a full flush of the register stack.
 */
#define	UNROLL_BUFMIN		256

/*
 * Define this to enable debugging of translated code.
 */
/*#define UNROLL_DEBUG*/

#ifdef UNROLL_DEBUG

/*
 * Dump translated code to stdout.
 */
static void DumpCode(ILMethod *method, unsigned char *start, int len)
{
	char cmdline[BUFSIZ];
	FILE *file = fopen("/tmp/unroll.s", "w");
	unsigned char *ip = start;
	if(!file)
	{
		return;
	}
	ILDumpMethodType(stdout, ILProgramItem_Image(method),
					 ILMethod_Signature(method), 0,
					 ILMethod_Owner(method),
					 ILMethod_Name(method), method);
	fputs(" ->\n", stdout);
	fflush(stdout);
	while(len > 0)
	{
		fprintf(file, ".byte %d\n", (int)(*ip));
		++ip;
		--len;
	}
	fclose(file);
	sprintf(cmdline, "as /tmp/unroll.s -o /tmp/unroll.o;objdump --adjust-vma=%ld -d /tmp/unroll.o", (long)start);
	system(cmdline);
	unlink("/tmp/unroll.s");
	unlink("/tmp/unroll.o");
	putc('\n', stdout);
	fflush(stdout);
}

#endif /* UNROLL_DEBUG */

/*
 * Include the global definitions needed by the cases.
 */
#define	IL_UNROLL_GLOBAL
#include "unroll_arith.c"
#include "unroll_branch.c"
#include "unroll_const.c"
#include "unroll_conv.c"
#include "unroll_ptr.c"
#include "unroll_var.c"
#undef	IL_UNROLL_GLOBAL

/* Imported from "cvm_lengths.c" */
extern unsigned char const _ILCVMLengths[512];

/* Imported from "cvmc.c" */
int _ILCVMStartUnrollBlock(ILCoder *_coder, int align, ILCachePosn *posn);

int _ILCVMUnrollMethod(ILCoder *coder, unsigned char *pc, ILMethod *method)
{
	MDUnroll unroll;
	int inUnrollBlock;
	unsigned char *inst;
	int opcode;
	unsigned char *overwritePC;
	md_inst_ptr unrollStart;
	int reg, reg2, reg3;
#ifdef IL_NATIVE_INT32
	int reg4;
#endif
	ILCachePosn posn;

	/* Find some room in the cache */
	if(!_ILCVMStartUnrollBlock(coder, 32, &posn))
	{
		return 0;
	}
	if((posn.limit - posn.ptr) < UNROLL_BUFMIN)
	{
		/* Insufficient space to unroll the method */
		ILCacheEndMethod(&posn);
		return 0;
	}

	/* Initialize the local unroll state */
	unroll.out = (md_inst_ptr)(posn.ptr);
	unroll.regsUsed = 0;
	unroll.regsSaved = 0;
	unroll.pseudoStackSize = 0;
	unroll.stackHeight = 0;
	unroll.cachedLocal = -1;
	unroll.cachedReg = -1;
	unroll.thisValidated = 0;
#if !MD_STATE_ALREADY_IN_REGS
	unroll.pcOffset = _ILCVMGetPcOffset(coder);
	unroll.stackOffset = _ILCVMGetStackOffset(coder);
	unroll.frameOffset = _ILCVMGetFrameOffset(coder);
#endif
#if MD_HAS_FP
#if MD_HAS_FP_STACK
	unroll.fpStackSize = 0;
#else
	unroll.fpRegsUsed = 0;
#endif
#endif
	inUnrollBlock = 0;
	overwritePC = 0;
	unrollStart = 0;

	/* Unroll the code */
	for(;;)
	{
		/* Fetch the next instruction pointer, and convert into an opcode */
		inst = *((void **)pc);
		if(inst >= minPtrValue && inst <= maxPtrValue)
		{
			opcode = (int)(ptrToOpcode[inst - minPtrValue]);
		}
		else
		{
			opcode = COP_NOP;
		}

		/* Bail out if we've reached the end of the method */
		if(opcode == COP_PREFIX)
		{
			/* Special direct instruction that marks the end of the method */
			break;
		}

		/* Bail out if we are low on space */
		if((posn.limit - ((unsigned char *)(unroll.out))) < UNROLL_BUFMIN)
		{
			break;
		}

		/* Determine what to do based on the opcode */
		switch(opcode)
		{
			#define	IL_UNROLL_CASES
			#include "unroll_arith.c"
			#include "unroll_branch.c"
			#include "unroll_const.c"
			#include "unroll_conv.c"
			#include "unroll_ptr.c"
			#include "unroll_var.c"
			#undef	IL_UNROLL_CASES

			case COP_NOP:
			{
				/* The "nop" instruction is used to mark labels */
				if(inUnrollBlock)
				{
					/* Branch to just after the "nop", because there
					   is no point trying to execute it */
					BranchToPC(&unroll, pc + CVM_LEN_NONE);
					UNROLL_FLUSH();
				}
				pc += CVM_LEN_NONE;
			}
			break;

			case 0x100 + COP_PREFIX_UNROLL_METHOD:
			{
				/* This is usually the first instruction that is
				   replaced by unrolled code, so optimise it away */
				UNROLL_START();
				MODIFY_UNROLL_PC(CVMP_LEN_NONE);
			}
			break;

			default:
			{
			defaultCase:
				/* Everything else terminates an unroll block */
				if(inUnrollBlock)
				{
					BranchToPC(&unroll, pc);
					UNROLL_FLUSH();
				}

				/* Skip the instruction that could not be unrolled */
				if(opcode != COP_SWITCH)
				{
					pc += (int)_ILCVMLengths[opcode];
				}
				else
				{
					pc = CVM_ARG_SWITCH_DEFAULT;
				}
			}
			break;
		}
	}

	/* Flush the last block that was converted */
	if(inUnrollBlock)
	{
		BranchToPC(&unroll, pc);
		UNROLL_FLUSH();
	}

#ifdef UNROLL_DEBUG
	/* Dump the translated code */
	DumpCode(method, posn.ptr, (int)(((unsigned char *)unroll.out) - posn.ptr));
#endif

	/* Update the method cache to reflect the final position */
	ILCacheFlush(posn.ptr, (long)(((unsigned char *)(unroll.out)) - posn.ptr));
	posn.ptr = (unsigned char *)(unroll.out);
	ILCacheEndMethod(&posn);
	return 1;
}

int _ILCVMUnrollGetNativeStack(ILCoder *coder, unsigned char **pcPtr, CVMWord **stackPtr)
{
#if !MD_STATE_ALREADY_IN_REGS
	unsigned char *pc;
	unsigned char *stack;
	MDUnroll unroll;
	md_inst_ptr unrollStart;
	ILCachePosn posn;

	pc = *pcPtr;
	stack = (unsigned char *) *stackPtr;

	/* Find some room in the cache */
	if(!_ILCVMStartUnrollBlock(coder, 32, &posn))
	{
		return 0;
	}
	if((posn.limit - posn.ptr) < UNROLL_BUFMIN)
	{
		/* Insufficient space to unroll the method */
		ILCacheEndMethod(&posn);
		return 0;
	}

	/* Initialize the local unroll state */
	unroll.out = (md_inst_ptr)(posn.ptr);
	unroll.regsUsed = 0;
	unroll.regsSaved = 0;
	unroll.pseudoStackSize = 0;
	unroll.stackHeight = 0;
	unroll.cachedLocal = -1;
	unroll.cachedReg = -1;
	unroll.thisValidated = 0;
	unroll.pcOffset = 0;
	unroll.stackOffset = 0;
	unroll.frameOffset = 0;
#if MD_HAS_FP
#if MD_HAS_FP_STACK
	unroll.fpStackSize = 0;
#else
	unroll->fpRegsUsed = 0;
#endif
#endif

	/* The start position of the code */
	unrollStart = unroll.out;

	/* Load the machine state */
	LoadMachineState(&unroll);

	/* Store native stack pointer value on the interpreter stack */
	md_load_const_native(unroll.out, MD_REG_TEMP_PC, stack);
	md_load_const_native(unroll.out, MD_REG_TEMP_PC_PTR, stackPtr);
	md_store_membase_word_native(unroll.out, MD_REG_NATIVE_STACK, MD_REG_TEMP_PC, 0);
	md_add_reg_imm(unroll.out, MD_REG_TEMP_PC, sizeof(CVMWord));
	md_store_membase_word_native(unroll.out, MD_REG_TEMP_PC, MD_REG_TEMP_PC_PTR, 0);

	/* Store the start position next to the current op  */
	pc += CVM_LEN_NONE;
	*((void **)pc) = unrollStart;
	pc += CVM_LEN_NONE;

	/* Unload the machine state and jump back into the CVM interpreter */
	UnloadMachineState(&unroll, pc, pcPtr, 0);

	/* Update the method cache to reflect the final position */
	ILCacheFlush(posn.ptr, (long)(((unsigned char *)(unroll.out)) - posn.ptr));
	posn.ptr = (unsigned char *)(unroll.out);
	ILCacheEndMethod(&posn);
	return 1;
#else
	return 0;
#endif
}

#else /* !IL_CVM_DIRECT_UNROLLED */

/*
 * Stub out the unroll API on other platforms.
 */

int _ILCVMUnrollInit(void)
{
	return 1;
}

int _ILCVMUnrollPossible(void)
{
	return 0;
}

int _ILCVMUnrollMethod(ILCoder *coder, unsigned char *pc, ILMethod *method)
{
	return 0;
}

#endif /* !IL_CVM_DIRECT_UNROLLED */

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_CVM */
