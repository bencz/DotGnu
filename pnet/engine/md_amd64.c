/*
 * md_amd64.c - Machine-dependent definitions for x86_64 (AMD64/EM64T).
 *
 * Copyright (C) 2003,2005  Southern Storm Software, Pty Ltd.
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
#include "md_amd64.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef CVM_X86_64

md_inst_ptr _md_amd64_shift(md_inst_ptr inst, int opc, int reg1, int reg2)
{
	if(reg2 == AMD64_RCX)
	{
		/* The shift value is already in ECX */
		amd64_shift_reg_size(inst, opc, reg1, 4);
	}
	else if(reg1 == AMD64_RCX)
	{
		/* The value to be shifted is in ECX, so swap the order */
		amd64_xchg_reg_reg(inst, reg1, reg2, 4);
		amd64_shift_reg_size(inst, opc, reg2, 4);
		amd64_mov_reg_reg(inst, reg1, reg2, 4);
	}
	else
	{
		/* Save ECX, perform the shift, and then restore ECX */
		amd64_push_reg(inst, AMD64_RCX);
		amd64_mov_reg_reg(inst, AMD64_RCX, reg2, 4);
		amd64_shift_reg_size(inst, opc, reg1, 4);
		amd64_pop_reg(inst, AMD64_RCX);
	}
	return inst;
}


md_inst_ptr _md_amd64_mov_membase_reg_byte
			(md_inst_ptr inst, int basereg, int offset, int srcreg)
{
	if(srcreg == AMD64_RAX || srcreg == AMD64_RBX ||
	   srcreg == AMD64_RCX || srcreg == AMD64_RDX)
	{
		amd64_mov_membase_reg(inst, basereg, offset, srcreg, 1);
	}
	else if(basereg != AMD64_RAX)
	{
		amd64_push_reg(inst, AMD64_RAX);
		amd64_mov_reg_reg(inst, AMD64_RAX, srcreg, 4);
		amd64_mov_membase_reg(inst, basereg, offset, AMD64_RAX, 1);
		amd64_pop_reg(inst, AMD64_RAX);
	}
	else
	{
		amd64_push_reg(inst, AMD64_RDX);
		amd64_mov_reg_reg(inst, AMD64_RDX, srcreg, 4);
		amd64_mov_membase_reg(inst, basereg, offset, AMD64_RDX, 1);
		amd64_pop_reg(inst, AMD64_RDX);
	}
	return inst;
}

md_inst_ptr _md_amd64_mov_memindex_reg_byte(md_inst_ptr inst, int basereg,
											unsigned offset, int indexreg,
											int srcreg)
{
	if(srcreg == AMD64_RAX || srcreg == AMD64_RBX ||
	   srcreg == AMD64_RCX || srcreg == AMD64_RDX)
	{
		amd64_mov_memindex_reg(inst, basereg, offset, indexreg,
							 0, srcreg, 1);
	}
	else
	{
		int tempreg;
		if(basereg != AMD64_RAX && indexreg != AMD64_RAX)
		{
			tempreg = AMD64_RAX;
		}
		else if(basereg != AMD64_RCX && indexreg != AMD64_RCX)
		{
			tempreg = AMD64_RCX;
		}
		else
		{
			tempreg = AMD64_RDX;
		}
		amd64_push_reg(inst, tempreg);
		amd64_mov_reg_reg(inst, tempreg, srcreg, 4);
		amd64_mov_memindex_reg(inst, basereg, offset, indexreg,
							 0, tempreg, 1);
		amd64_pop_reg(inst, tempreg);
	}
	return inst;
}

md_inst_ptr _md_amd64_switch(md_inst_ptr inst, int reg, void * table)
{
	/* we can clobber over MD_REG_PC */
	amd64_mov_reg_imm_size(inst, MD_REG_PC, table, 8);
	amd64_mov_reg_memindex(inst, MD_REG_PC, MD_REG_PC,
									 0, reg, 3, 8);
	amd64_jump_membase(inst, MD_REG_PC, 0);

	return inst;
}

md_inst_ptr _md_amd64_rem_float
		(md_inst_ptr inst, int reg1, int reg2, int used)
{
	md_inst_ptr label;
	if((used & (1 << AMD64_RAX)) != 0)
	{
		amd64_push_reg(inst, AMD64_RAX);
	}
	label = inst;
	amd64_fprem(inst);
	amd64_fnstsw(inst);
	amd64_alu_reg_imm(inst, X86_AND, AMD64_RAX, 0x0400);
	amd64_branch(inst, X86_CC_NZ, label, 0);
	amd64_fstp(inst, 1);
	if((used & (1 << AMD64_RAX)) != 0)
	{
		amd64_pop_reg(inst, AMD64_RAX);
	}
	return inst;
}

md_inst_ptr _md_amd64_setcc(md_inst_ptr inst, int reg, int cond)
{
	if(cond == X86_CC_EQ || cond == X86_CC_NE)
	{
		amd64_alu_reg_reg_size(inst, X86_OR, reg, reg, 4);
	}
	else
	{
		amd64_alu_reg_imm_size(inst, X86_CMP, reg, 0, 4);
	}
	if(reg == AMD64_RAX || reg == AMD64_RBX || reg == AMD64_RCX || reg == AMD64_RDX)
	{
		/* Use a SETcc instruction if we have a basic register */
		amd64_set_reg_size(inst, cond, reg, 1, 4);
		amd64_widen_reg_size(inst, reg, reg, 0, 0, 4);
	}
	else
	{
		/* The register is not useable as an 8-bit destination */
		unsigned char *patch1, *patch2;
		patch1 = inst;
		amd64_branch8(inst, cond, 0, 1);
		amd64_clear_reg_size(inst, reg, 4);
		patch2 = inst;
		amd64_jump8(inst, 0);
		amd64_patch(patch1, inst);
		amd64_mov_reg_imm_size(inst, reg, 1, 4);
		amd64_patch(patch2, inst);
	}
	return inst;
}

md_inst_ptr _md_amd64_compare(md_inst_ptr inst, int reg1, int reg2, int isSigned, int size)
{
	unsigned char *patch1, *patch2, *patch3;
	amd64_alu_reg_reg_size(inst, X86_CMP, reg1, reg2, size);
	patch1 = inst;
	amd64_branch8(inst, X86_CC_GE, 0, isSigned);
	amd64_mov_reg_imm_size(inst, reg1, -1, size);
	patch2 = inst;
	amd64_jump8(inst, 0);
	amd64_patch(patch1, inst);
	patch1 = inst;
	amd64_branch8(inst, X86_CC_EQ, 0, 0);
	amd64_mov_reg_imm_size(inst, reg1, 1, size);
	patch3 = inst;
	amd64_jump8(inst, 0);
	amd64_patch(patch1, inst);
	amd64_clear_reg_size(inst, reg1, size);
	amd64_patch(patch2, inst);
	amd64_patch(patch3, inst);
	return inst;
}

md_inst_ptr _md_amd64_widen_byte(md_inst_ptr inst, int reg, int isSigned)
{
	if(reg == AMD64_RAX || reg == AMD64_RBX || reg == AMD64_RCX || reg == AMD64_RDX)
	{
		amd64_widen_reg(inst, reg, reg, isSigned, 0);
	}
	else
	{
		amd64_push_reg(inst, AMD64_RAX);
		amd64_mov_reg_reg(inst, AMD64_RAX, reg, 4);
		amd64_widen_reg(inst, reg, AMD64_RAX, isSigned, 0);
		amd64_pop_reg(inst, AMD64_RAX);
	}
	return inst;
}

md_inst_ptr _md_amd64_cmp_float(md_inst_ptr inst, int dreg, int lessop)
{
	md_inst_ptr patch1, patch2, patch3;

	/* We need the EAX register to store the FPU status word */
	if(dreg != AMD64_RAX)
	{
		amd64_push_reg(inst, AMD64_RAX);
	}

	/* Compare the values and get the FPU status word */
	amd64_fcompp(inst);
	amd64_fnstsw(inst);
	amd64_alu_reg_imm(inst, X86_AND, AMD64_RAX, 0x4500);

	/* Decode the FPU status word to determine the result */
	amd64_alu_reg_imm(inst, X86_CMP, AMD64_RAX, 0x4000);		/* eq */
	patch1 = inst;
	amd64_branch8(inst, X86_CC_NE, 0, 0);
	amd64_clear_reg(inst, AMD64_RAX);
	patch2 = inst;
	amd64_jump8(inst, 0);
	amd64_patch(patch1, inst);
	if(lessop)
	{
		amd64_alu_reg_imm(inst, X86_CMP, AMD64_RAX, 0x0100);	/* gt */
		patch1 = inst;
		amd64_branch8(inst, X86_CC_NE, 0, 0);
		amd64_mov_reg_imm(inst, AMD64_RAX, 1);
		patch3 = inst;
		amd64_jump8(inst, 0);
		amd64_patch(patch1, inst);
		amd64_mov_reg_imm(inst, AMD64_RAX, -1);
	}
	else
	{
		amd64_alu_reg_imm(inst, X86_CMP, AMD64_RAX, 0x0000);	/* lt */
		patch1 = inst;
		amd64_branch8(inst, X86_CC_NE, 0, 0);
		amd64_mov_reg_imm(inst, AMD64_RAX, -1);
		patch3 = inst;
		amd64_jump8(inst, 0);
		amd64_patch(patch1, inst);
		amd64_mov_reg_imm(inst, AMD64_RAX, 1);
	}
	amd64_patch(patch2, inst);
	amd64_patch(patch3, inst);

	/* Shift the result into the destination register */
	if(dreg != AMD64_RAX)
	{
		amd64_mov_reg_reg(inst, dreg, AMD64_RAX, 4);
		amd64_pop_reg(inst, AMD64_RAX);
	}
	return inst;
}

#endif /* CVM_X86_64 */

#ifdef	__cplusplus
};
#endif
