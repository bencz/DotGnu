/*
 * md_ppc.c - Machine-dependent definitions for PPC
 *
 * Copyright (C) 2004-2005  Southern Storm Software, Pty Ltd.
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
#include "md_ppc.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef CVM_PPC

/*
	Please check the "Optimal Sequences" section in the
	PPC compiler writer's guide for a description of how
	the following code works.
 */
md_inst_ptr _md_ppc_setcc(md_inst_ptr inst, int reg, int cond)
{
	switch(cond)
	{
		case PPC_CC_EQ:
		{
			/*
				count zeros and check for bit no 5 
			*/
			ppc_alu_reg_ds(inst, PPC_CNTLZ, reg, reg);
			/* srwi reg, reg, 5 */
			ppc_alu_rlwinm(inst, reg, reg, 32-5, 5, 31);
		}
		break;
		case PPC_CC_NE:
		{
			ppc_alu_reg_imm(inst, PPC_ADDIC, PPC_WORK, reg, -1);
			ppc_alu_reg_dss(inst, PPC_SUBFE, reg, PPC_WORK, reg);
		}
		break;
		case PPC_CC_LT:
		{
			/* sign bit */
			ppc_alu_rlwinm(inst, reg, reg, 1, 31, 31);
		}
		break;
		case PPC_CC_GE:
		{
			/* sign bit */
			ppc_alu_rlwinm(inst, reg, reg, 1, 31, 31);
			/* invert */
			ppc_alu_reg_imm(inst, PPC_XORI,	reg, reg, 1);
		}
		break;
		case PPC_CC_GT:
		{
			ppc_alu_reg_ds(inst, PPC_NEG, PPC_WORK, reg);
			ppc_alu_reg_sds(inst, PPC_ANDC, reg, PPC_WORK,  reg);
			/* sign bit */
			ppc_alu_rlwinm(inst, reg, reg, 1, 31, 31);
		}
		break;
		case PPC_CC_LE:
		{
			ppc_alu_reg_ds(inst, PPC_NEG, PPC_WORK, reg);
			ppc_alu_reg_sds(inst, PPC_ORC, reg, reg, PPC_WORK);
			/* sign bit */
			ppc_alu_rlwinm(inst, reg, reg, 1, 31, 31);
		}
		break;
		default:
		{
			TODO_trap(inst);
		}
		break;
	}
	
	return inst;
}

/*
 *   li dreg, 1
 *   bgt L3
 *   blt L2
 *   li dreg, 0
 *   b L3
 * L2:
 *   li dreg, -1
 * L3:
 */

md_inst_ptr _md_ppc_setcmp(md_inst_ptr inst, int dreg)
{
	md_inst_ptr patch1, patch2, patch3;

	ppc_mov_reg_imm(inst, dreg, 1);
	
	patch1 = inst;
	ppc_branch(inst, PPC_CC_GT, 0); 

	patch2 = inst;
	ppc_branch(inst, PPC_CC_LT, 0);

	ppc_mov_reg_imm(inst, dreg, 0);

	patch3 = inst;
	ppc_jump(inst, 0);

	ppc_patch(patch2, inst); 
	ppc_mov_reg_imm(inst, dreg, -1);

	ppc_patch(patch1, inst);
	ppc_patch(patch3, inst);

	return inst;
}
#endif /* CVM_PPC */

#ifdef	__cplusplus
};
#endif
