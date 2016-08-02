/*
 * md_x86.c - Machine-dependent definitions for x86.
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

#include "cvm_config.h"
#include "md_x86.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef CVM_X86

md_inst_ptr _md_x86_shift(md_inst_ptr inst, int opc, int reg1, int reg2)
{
	if(reg2 == X86_ECX)
	{
		/* The shift value is already in ECX */
		x86_shift_reg(inst, opc, reg1);
	}
	else if(reg1 == X86_ECX)
	{
		/* The value to be shifted is in ECX, so swap the order */
		x86_xchg_reg_reg(inst, reg1, reg2, 4);
		x86_shift_reg(inst, opc, reg2);
		x86_mov_reg_reg(inst, reg1, reg2, 4);
	}
	else
	{
		/* Save ECX, perform the shift, and then restore ECX */
		x86_push_reg(inst, X86_ECX);
		x86_mov_reg_reg(inst, X86_ECX, reg2, 4);
		x86_shift_reg(inst, opc, reg1);
		x86_pop_reg(inst, X86_ECX);
	}
	return inst;
}

md_inst_ptr _md_x86_mov_membase_reg_byte
			(md_inst_ptr inst, int basereg, int offset, int srcreg)
{
	if(srcreg == X86_EAX || srcreg == X86_EBX ||
	   srcreg == X86_ECX || srcreg == X86_EDX)
	{
		x86_mov_membase_reg(inst, basereg, offset, srcreg, 1);
	}
	else if(basereg != X86_EAX)
	{
		x86_push_reg(inst, X86_EAX);
		x86_mov_reg_reg(inst, X86_EAX, srcreg, 4);
		x86_mov_membase_reg(inst, basereg, offset, X86_EAX, 1);
		x86_pop_reg(inst, X86_EAX);
	}
	else
	{
		x86_push_reg(inst, X86_EDX);
		x86_mov_reg_reg(inst, X86_EDX, srcreg, 4);
		x86_mov_membase_reg(inst, basereg, offset, X86_EDX, 1);
		x86_pop_reg(inst, X86_EDX);
	}
	return inst;
}

md_inst_ptr _md_x86_mov_memindex_reg_byte(md_inst_ptr inst, int basereg,
							   			  unsigned offset, int indexreg,
							   			  int srcreg)
{
	if(srcreg == X86_EAX || srcreg == X86_EBX ||
	   srcreg == X86_ECX || srcreg == X86_EDX)
	{
		x86_mov_memindex_reg(inst, basereg, offset, indexreg,
							 0, srcreg, 1);
	}
	else
	{
		int tempreg;
		if(basereg != X86_EAX && indexreg != X86_EAX)
		{
			tempreg = X86_EAX;
		}
		else if(basereg != X86_ECX && indexreg != X86_ECX)
		{
			tempreg = X86_ECX;
		}
		else
		{
			tempreg = X86_EDX;
		}
		x86_push_reg(inst, tempreg);
		x86_mov_reg_reg(inst, tempreg, srcreg, 4);
		x86_mov_memindex_reg(inst, basereg, offset, indexreg,
							 0, tempreg, 1);
		x86_pop_reg(inst, tempreg);
	}
	return inst;
}

md_inst_ptr _md_x86_rem_float
		(md_inst_ptr inst, int reg1, int reg2, int used)
{
	md_inst_ptr label;
	if((used & (1 << X86_EAX)) != 0)
	{
		x86_push_reg(inst, X86_EAX);
	}
	label = inst;
	x86_fprem(inst);
	x86_fnstsw(inst);
	x86_alu_reg_imm(inst, X86_AND, X86_EAX, 0x0400);
	x86_branch(inst, X86_CC_NZ, label, 0);
	x86_fstp(inst, 1);
	if((used & (1 << X86_EAX)) != 0)
	{
		x86_pop_reg(inst, X86_EAX);
	}
	return inst;
}

md_inst_ptr _md_x86_setcc(md_inst_ptr inst, int reg, int cond)
{
	if(cond == X86_CC_EQ || cond == X86_CC_NE)
	{
		x86_alu_reg_reg(inst, X86_OR, reg, reg);
	}
	else
	{
		x86_alu_reg_imm(inst, X86_CMP, reg, 0);
	}
	if(reg == X86_EAX || reg == X86_EBX || reg == X86_ECX || reg == X86_EDX)
	{
		/* Use a SETcc instruction if we have a basic register */
		x86_set_reg(inst, cond, reg, 1);
		x86_widen_reg(inst, reg, reg, 0, 0);
	}
	else
	{
		/* The register is not useable as an 8-bit destination */
		unsigned char *patch1, *patch2;
		patch1 = inst;
		x86_branch8(inst, cond, 0, 1);
		x86_clear_reg(inst, reg);
		patch2 = inst;
		x86_jump8(inst, 0);
		x86_patch(patch1, inst);
		x86_mov_reg_imm(inst, reg, 1);
		x86_patch(patch2, inst);
	}
	return inst;
}

md_inst_ptr _md_x86_compare(md_inst_ptr inst, int reg1, int reg2, int isSigned)
{
	unsigned char *patch1, *patch2, *patch3;
	x86_alu_reg_reg(inst, X86_CMP, reg1, reg2);
	patch1 = inst;
	x86_branch8(inst, X86_CC_GE, 0, isSigned);
	x86_mov_reg_imm(inst, reg1, -1);
	patch2 = inst;
	x86_jump8(inst, 0);
	x86_patch(patch1, inst);
	patch1 = inst;
	x86_branch8(inst, X86_CC_EQ, 0, 0);
	x86_mov_reg_imm(inst, reg1, 1);
	patch3 = inst;
	x86_jump8(inst, 0);
	x86_patch(patch1, inst);
	x86_clear_reg(inst, reg1);
	x86_patch(patch2, inst);
	x86_patch(patch3, inst);
	return inst;
}

md_inst_ptr _md_x86_widen_byte(md_inst_ptr inst, int reg, int isSigned)
{
	if(reg == X86_EAX || reg == X86_EBX || reg == X86_ECX || reg == X86_EDX)
	{
		x86_widen_reg(inst, reg, reg, isSigned, 0);
	}
	else
	{
		x86_push_reg(inst, X86_EAX);
		x86_mov_reg_reg(inst, X86_EAX, reg, 4);
		x86_widen_reg(inst, reg, X86_EAX, isSigned, 0);
		x86_pop_reg(inst, X86_EAX);
	}
	return inst;
}

md_inst_ptr _md_x86_cmp_float(md_inst_ptr inst, int dreg, int lessop)
{
	md_inst_ptr patch1, patch2, patch3;

	/* We need the EAX register to store the FPU status word */
	if(dreg != X86_EAX)
	{
		x86_push_reg(inst, X86_EAX);
	}

	/* Compare the values and get the FPU status word */
	x86_fcompp(inst);
	x86_fnstsw(inst);
	x86_alu_reg_imm(inst, X86_AND, X86_EAX, 0x4500);

	/* Decode the FPU status word to determine the result */
	x86_alu_reg_imm(inst, X86_CMP, X86_EAX, 0x4000);		/* eq */
	patch1 = inst;
	x86_branch8(inst, X86_CC_NE, 0, 0);
	x86_clear_reg(inst, X86_EAX);
	patch2 = inst;
	x86_jump8(inst, 0);
	x86_patch(patch1, inst);
	if(lessop)
	{
		x86_alu_reg_imm(inst, X86_CMP, X86_EAX, 0x0100);	/* gt */
		patch1 = inst;
		x86_branch8(inst, X86_CC_NE, 0, 0);
		x86_mov_reg_imm(inst, X86_EAX, 1);
		patch3 = inst;
		x86_jump8(inst, 0);
		x86_patch(patch1, inst);
		x86_mov_reg_imm(inst, X86_EAX, -1);
	}
	else
	{
		x86_alu_reg_imm(inst, X86_CMP, X86_EAX, 0x0000);	/* lt */
		patch1 = inst;
		x86_branch8(inst, X86_CC_NE, 0, 0);
		x86_mov_reg_imm(inst, X86_EAX, -1);
		patch3 = inst;
		x86_jump8(inst, 0);
		x86_patch(patch1, inst);
		x86_mov_reg_imm(inst, X86_EAX, 1);
	}
	x86_patch(patch2, inst);
	x86_patch(patch3, inst);

	/* Shift the result into the destination register */
	if(dreg != X86_EAX)
	{
		x86_mov_reg_reg(inst, dreg, X86_EAX, 4);
		x86_pop_reg(inst, X86_EAX);
	}
	return inst;
}

#endif /* CVM_X86 */

#ifdef	__cplusplus
};
#endif
