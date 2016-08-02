/*
 * md_arm.c - Machine-dependent definitions for ARM.
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

#include "cvm_config.h"
#include "md_arm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef CVM_ARM

arm_inst_ptr _arm_mov_reg_imm(arm_inst_ptr inst, int reg, int value)
{
	/* Handle bytes in various positions */
	if((value & 0x000000FF) == value)
	{
		arm_mov_reg_imm8(inst, reg, value);
		return inst;
	}
	else if((value & 0x0000FF00) == value)
	{
		arm_mov_reg_imm8_rotate(inst, reg, (value >> 8), 12);
		return inst;
	}
	else if((value & 0x00FF0000) == value)
	{
		arm_mov_reg_imm8_rotate(inst, reg, (value >> 16), 8);
		return inst;
	}
	else if((value & 0xFF000000) == value)
	{
		arm_mov_reg_imm8_rotate(inst, reg, ((value >> 24) & 0xFF), 4);
		return inst;
	}

	/* Handle inverted bytes in various positions */
	value = ~value;
	if((value & 0x000000FF) == value)
	{
		arm_mov_reg_imm8(inst, reg, value);
		arm_alu_reg(inst, ARM_MVN, reg, reg);
		return inst;
	}
	else if((value & 0x0000FF00) == value)
	{
		arm_mov_reg_imm8_rotate(inst, reg, (value >> 8), 12);
		arm_alu_reg(inst, ARM_MVN, reg, reg);
		return inst;
	}
	else if((value & 0x00FF0000) == value)
	{
		arm_mov_reg_imm8_rotate(inst, reg, (value >> 16), 8);
		arm_alu_reg(inst, ARM_MVN, reg, reg);
		return inst;
	}
	else if((value & 0xFF000000) == value)
	{
		arm_mov_reg_imm8_rotate(inst, reg, ((value >> 24) & 0xFF), 4);
		arm_alu_reg(inst, ARM_MVN, reg, reg);
		return inst;
	}

	/* Build the value the hard way, byte by byte */
	value = ~value;
	if((value & 0xFF000000) != 0)
	{
		arm_mov_reg_imm8_rotate(inst, reg, ((value >> 24) & 0xFF), 4);
		if((value & 0x00FF0000) != 0)
		{
			arm_alu_reg_imm8_rotate
				(inst, ARM_ADD, reg, reg, ((value >> 16) & 0xFF), 8);
		}
		if((value & 0x0000FF00) != 0)
		{
			arm_alu_reg_imm8_rotate
				(inst, ARM_ADD, reg, reg, ((value >> 8) & 0xFF), 12);
		}
		if((value & 0x000000FF) != 0)
		{
			arm_alu_reg_imm8(inst, ARM_ADD, reg, reg, (value & 0xFF));
		}
	}
	else if((value & 0x00FF0000) != 0)
	{
		arm_mov_reg_imm8_rotate(inst, reg, ((value >> 16) & 0xFF), 8);
		if((value & 0x0000FF00) != 0)
		{
			arm_alu_reg_imm8_rotate
				(inst, ARM_ADD, reg, reg, ((value >> 8) & 0xFF), 12);
		}
		if((value & 0x000000FF) != 0)
		{
			arm_alu_reg_imm8(inst, ARM_ADD, reg, reg, (value & 0xFF));
		}
	}
	else if((value & 0x0000FF00) != 0)
	{
		arm_mov_reg_imm8_rotate(inst, reg, ((value >> 8) & 0xFF), 12);
		if((value & 0x000000FF) != 0)
		{
			arm_alu_reg_imm8(inst, ARM_ADD, reg, reg, (value & 0xFF));
		}
	}
	else
	{
		arm_mov_reg_imm8(inst, reg, (value & 0xFF));
	}
	return inst;
}

arm_inst_ptr _arm_alu_reg_imm(arm_inst_ptr inst, int opc,
							  int dreg, int sreg, int imm,
							  int saveWork)
{
	int tempreg;
	if(saveWork)
	{
		if(dreg != ARM_R2 && sreg != ARM_R2)
		{
			tempreg = ARM_R2;
		}
		else if(dreg != ARM_R3 && sreg != ARM_R3)
		{
			tempreg = ARM_R3;
		}
		else
		{
			tempreg = ARM_R4;
		}
		arm_push_reg(inst, tempreg);
	}
	else
	{
		tempreg = ARM_WORK;
	}
	inst = _arm_mov_reg_imm(inst, tempreg, imm);
	arm_alu_reg_reg(inst, opc, dreg, sreg, tempreg);
	if(saveWork)
	{
		arm_pop_reg(inst, tempreg);
	}
	return inst;
}

md_inst_ptr _md_arm_setcc(md_inst_ptr inst, int reg, int cond, int invcond)
{
	arm_test_reg_imm8(inst, ARM_CMP, reg, 0);
	arm_alu_reg_imm8_cond(inst, ARM_MOV, reg, 0, 1, cond);
	arm_alu_reg_imm8_cond(inst, ARM_MOV, reg, 0, 0, invcond);
	return inst;
}

#ifdef ARM_HAS_FLOAT

/*
 * Comparision results:
 *  Flags:    | N | Z | C | V |
 *  ===================================
 *  ==        | 0 | 1 | 1 | 0 |
 *  <         | 1 | 0 | 0 | 0 |
 *  >         | 0 | 0 | 1 | 0 |
 *  unordered | 0 | 0 | 1 | 1 |
 */
md_inst_ptr _md_arm_cmp_float(md_inst_ptr inst, int dreg, int sreg1,
							  int sreg2, int lessop)
{
	/* Remove the MD_FP_REG flag from the source registers */
	sreg1 &= ~MD_FREG_MASK;
	sreg2 &= ~MD_FREG_MASK;
	/* perform a nonsignaling compare */
	arm_cmpn_double_reg_reg(inst, ARM_CC_AL, sreg1, sreg2);
	/* Initialize result with 0. This handles the equal result */
	arm_mov_reg_imm8(inst, dreg, 0);
	/* Move the float condition flags to arm */
	arm_mov_vfpstatus(inst);
	if(lessop)
	{
		/* Do it just how specified for now */
		/* Set the result to 1 if C is set and Z is not set */
		arm_alu_reg_imm8_cond(inst, ARM_MOV, dreg, dreg, 1, ARM_CC_HI);
		/* Set the result to -1 if either N or V are set */
		arm_alu_reg_imm8_cond(inst, ARM_MVN, dreg, dreg, 0, ARM_CC_LT);
	}
	else
	{
		/* Set the result to 1 if C is set and Z is not set */
		arm_alu_reg_imm8_cond(inst, ARM_MOV, dreg, dreg, 1, ARM_CC_HI);
		/* Set the result to -1 if N is set */
		arm_alu_reg_imm8_cond(inst, ARM_MVN, dreg, dreg, 0, ARM_CC_MI);
		/* Set the result to 1 if V is set */
		arm_alu_reg_imm8_cond(inst, ARM_MOV, dreg, dreg, 1, ARM_CC_VS);
	}
	return inst;
}

#endif /* ARM_HAS_FLOAT */

#endif /* CVM_ARM */

#ifdef	__cplusplus
};
#endif
