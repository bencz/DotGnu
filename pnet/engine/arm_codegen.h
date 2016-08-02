/*
 * arm_codegen.h - Code generation macros for the ARM processor.
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

#ifndef	_ARM_CODEGEN_H
#define	_ARM_CODEGEN_H

#ifdef __cplusplus
extern "C" {
#endif

/*
 * Register numbers.
 */
typedef enum
{
	ARM_R0   = 0,
	ARM_R1   = 1,
	ARM_R2   = 2,
	ARM_R3   = 3,
	ARM_R4   = 4,
	ARM_R5   = 5,
	ARM_R6   = 6,
	ARM_R7   = 7,
	ARM_R8   = 8,
	ARM_R9   = 9,
	ARM_R10  = 10,
	ARM_R11  = 11,
	ARM_R12  = 12,
	ARM_R13  = 13,
	ARM_R14  = 14,
	ARM_R15  = 15,
	ARM_FP   = ARM_R11,			/* Frame pointer */
	ARM_LINK = ARM_R14,			/* Link register */
	ARM_PC   = ARM_R15,			/* Program counter */
	ARM_WORK = ARM_R12,			/* Work register that we can destroy */
	ARM_SP   = ARM_R13,			/* Stack pointer */

} ARM_REG;

#ifdef __VFP_FP__

#define ARM_HAS_FLOAT 1

/*
 * Register definitions for the arm vfp coprocessor.
 *
 * NOTE: Double precision operations are available only in D variants.
 * Arm: Variant VFPv1D supports double precision while VFP1xD doesn't.
 *      By default double precision support is present.
 */

/*
 * VFP Coprocessor numbers.
 */
#define ARMVFP_CSINGLE		10
#define ARMVFP_CDOUBLE		11

/*
 * VFP Register numbers.
 */
typedef enum
{
	/*
	 * Single precision registers.
	 */
	ARMVFP_S0	= 0,
	ARMVFP_S1	= 1,
	ARMVFP_S2	= 2,
	ARMVFP_S3	= 3,
	ARMVFP_S4	= 4,
	ARMVFP_S5	= 5,
	ARMVFP_S6	= 6,
	ARMVFP_S7	= 7,
	ARMVFP_S8	= 8,
	ARMVFP_S9	= 9,
	ARMVFP_S10	= 10,
	ARMVFP_S11	= 11,
	ARMVFP_S12	= 12,
	ARMVFP_S13	= 13,
	ARMVFP_S14	= 14,
	ARMVFP_S15	= 15,
	ARMVFP_S16	= 16,
	ARMVFP_S17	= 17,
	ARMVFP_S18	= 18,
	ARMVFP_S19	= 19,
	ARMVFP_S20	= 20,
	ARMVFP_S21	= 21,
	ARMVFP_S22	= 22,
	ARMVFP_S23	= 23,
	ARMVFP_S24	= 24,
	ARMVFP_S25	= 25,
	ARMVFP_S26	= 26,
	ARMVFP_S27	= 27,
	ARMVFP_S28	= 28,
	ARMVFP_S29	= 29,
	ARMVFP_S30	= 30,
	ARMVFP_S31	= 31,
	/*
	 * Double precision registers.
	 * NOTE: The double precision registers overlap with the single
	 * precision registers.
	 * For example ARMVFP_D0 overlaps with ARMVFP_S0 and ARMVFP_S1,
	 * ARMVFP_D1 overlaps with ARMVFP_S2 and ARMVFP_S3, ...
	 * ARMVFP_Dn overlaps with ARMVFP_S(2n) and ARMVFP_S(2n+1)
	 */
	ARMVFP_D0	= 0,
	ARMVFP_D1	= 1,
	ARMVFP_D2	= 2,
	ARMVFP_D3	= 3,
	ARMVFP_D4	= 4,
	ARMVFP_D5	= 5,
	ARMVFP_D6	= 6,
	ARMVFP_D7	= 7,
	ARMVFP_D8	= 8,
	ARMVFP_D9	= 9,
	ARMVFP_D10	= 10,
	ARMVFP_D11	= 11,
	ARMVFP_D12	= 12,
	ARMVFP_D13	= 13,
	ARMVFP_D14	= 14,
	ARMVFP_D15	= 15,
	/*
	 * VFP system registers for moving a vfp system register to one
	 * of the regular arm registers.
	 */
	ARMVFP_FPSID	= 0,
	ARMVFP_FPSCR	= 1,
	ARMVFP_FPEXC	= 8

} ARMVFP_REG;

#define ARM_D0	ARMVFP_D0
#define ARM_D1	ARMVFP_D1
#define ARM_D2	ARMVFP_D2
#define ARM_D3	ARMVFP_D3
#define ARM_D4	ARMVFP_D4
#define ARM_D5	ARMVFP_D5
#define ARM_D6	ARMVFP_D6
#define ARM_D7	ARMVFP_D7
#define ARM_D8	ARMVFP_D8
#define ARM_D9	ARMVFP_D9
#define ARM_D10	ARMVFP_D10
#define ARM_D11	ARMVFP_D11
#define ARM_D12	ARMVFP_D12
#define ARM_D13	ARMVFP_D13
#define ARM_D14	ARMVFP_D14
#define ARM_D15	ARMVFP_D15

#endif /* __VFP_FP__ */

/*
 * Condition codes.
 */
typedef enum
{
	ARM_CC_EQ    = 0,			/* Equal */
	ARM_CC_NE    = 1,			/* Not equal */
	ARM_CC_CS    = 2,			/* Carry set */
	ARM_CC_CC    = 3,			/* Carry clear */
	ARM_CC_MI    = 4,			/* Negative */
	ARM_CC_PL    = 5,			/* Positive */
	ARM_CC_VS    = 6,			/* Overflow set */
	ARM_CC_VC    = 7,			/* Overflow clear */
	ARM_CC_HI    = 8,			/* Higher */
	ARM_CC_LS    = 9,			/* Lower or same */
	ARM_CC_GE    = 10,			/* Signed greater than or equal */
	ARM_CC_LT    = 11,			/* Signed less than */
	ARM_CC_GT    = 12,			/* Signed greater than */
	ARM_CC_LE    = 13,			/* Signed less than or equal */
	ARM_CC_AL    = 14,			/* Always */
	ARM_CC_NV    = 15,			/* Never */
	ARM_CC_GE_UN = ARM_CC_CS,	/* Unsigned greater than or equal */
	ARM_CC_LT_UN = ARM_CC_CC,	/* Unsigned less than */
	ARM_CC_GT_UN = ARM_CC_HI,	/* Unsigned greater than */
	ARM_CC_LE_UN = ARM_CC_LS,	/* Unsigned less than or equal */

} ARM_CC;

/*
 * Arithmetic and logical operations.
 */
typedef enum
{
	ARM_AND = 0,				/* Bitwise AND */
	ARM_EOR = 1,				/* Bitwise XOR */
	ARM_SUB = 2,				/* Subtract */
	ARM_RSB = 3,				/* Reverse subtract */
	ARM_ADD = 4,				/* Add */
	ARM_ADC = 5,				/* Add with carry */
	ARM_SBC = 6,				/* Subtract with carry */
	ARM_RSC = 7,				/* Reverse subtract with carry */
	ARM_TST = 8,				/* Test with AND */
	ARM_TEQ = 9,				/* Test with XOR */
	ARM_CMP = 10,				/* Test with SUB (compare) */
	ARM_CMN = 11,				/* Test with ADD */
	ARM_ORR = 12,				/* Bitwise OR */
	ARM_MOV = 13,				/* Move */
	ARM_BIC = 14,				/* Test with Op1 & ~Op2 */
	ARM_MVN = 15,				/* Bitwise NOT */

} ARM_OP;

/*
 * Shift operators.
 */
typedef enum
{
	ARM_SHL = 0,				/* Logical left */
	ARM_SHR = 1,				/* Logical right */
	ARM_SAR = 2,				/* Arithmetic right */
	ARM_ROR = 3,				/* Rotate right */

} ARM_SHIFT;

/*
 * Defines to enable individual instructions that are aveilable only in
 * specific processor verions.
 */

/*
 * MLA is available in arm versions 2 and above 
 */
#define ARM_HAS_MLA 1

/*
 * LDRSB and LDRSH are available in arm versions 4 and above.
 */
#define ARM_HAS_LDRSB 1
#define ARM_HAS_LDRSH 1

/*
 * Type for instruction pointers (word-based, not byte-based).
 */
typedef unsigned int *arm_inst_ptr;

/*
 * Build an instruction prefix from a condition code and a mask value.
 */
#define	arm_build_prefix(cond,mask)	\
			((((unsigned int)(cond)) << 28) | ((unsigned int)(mask)))

/*
 * Build an "always" instruction prefix for a regular instruction.
 */
#define arm_prefix(mask)	(arm_build_prefix(ARM_CC_AL, (mask)))

/*
 * Build special "always" prefixes.
 */
#define	arm_always			(arm_build_prefix(ARM_CC_AL, 0))
#define	arm_always_cc		(arm_build_prefix(ARM_CC_AL, (1 << 20)))
#define	arm_always_imm		(arm_build_prefix(ARM_CC_AL, (1 << 25)))

/*
 * Arithmetic or logical operation which doesn't set condition codes.
 */
#define	arm_alu_reg_reg(inst,opc,dreg,sreg1,sreg2)	\
			do { \
				*(inst)++ = arm_always | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg1)) << 16) | \
							 ((unsigned int)(sreg2)); \
			} while (0)
#define	arm_alu_reg_reg_lslimm(inst,opc,dreg,sreg1,sreg2,shift)	\
			do { \
				*(inst)++ = arm_always | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg1)) << 16) | \
							(((unsigned int)(shift) & 0x1f) << 7) | \
							 ((unsigned int)(sreg2)); \
			} while (0)
#define	arm_alu_reg_imm8(inst,opc,dreg,sreg,imm)	\
			do { \
				*(inst)++ = arm_always_imm | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg)) << 16) | \
							 ((unsigned int)((imm) & 0xFF)); \
			} while (0)
#define	arm_alu_reg_imm8_cond(inst,opc,dreg,sreg,imm,cond)	\
			do { \
				*(inst)++ = arm_build_prefix((cond), (1 << 25)) | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg)) << 16) | \
							 ((unsigned int)((imm) & 0xFF)); \
			} while (0)
#define	arm_alu_reg_imm8_rotate(inst,opc,dreg,sreg,imm,rotate)	\
			do { \
				*(inst)++ = arm_always_imm | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg)) << 16) | \
							(((unsigned int)(rotate)) << 8) | \
							 ((unsigned int)((imm) & 0xFF)); \
			} while (0)
extern arm_inst_ptr _arm_alu_reg_imm(arm_inst_ptr inst, int opc, int dreg,
							         int sreg, int imm, int saveWork);
#define	arm_alu_reg_imm(inst,opc,dreg,sreg,imm)	\
			do { \
				int __alu_imm = (int)(imm); \
				if(__alu_imm >= 0 && __alu_imm < 256) \
				{ \
					arm_alu_reg_imm8 \
						((inst), (opc), (dreg), (sreg), __alu_imm); \
				} \
				else \
				{ \
					(inst) = _arm_alu_reg_imm \
						((inst), (opc), (dreg), (sreg), __alu_imm, 0); \
				} \
			} while (0)
#define	arm_alu_reg_imm_save_work(inst,opc,dreg,sreg,imm)	\
			do { \
				int __alu_imm_save = (int)(imm); \
				if(__alu_imm_save >= 0 && __alu_imm_save < 256) \
				{ \
					arm_alu_reg_imm8 \
						((inst), (opc), (dreg), (sreg), __alu_imm_save); \
				} \
				else \
				{ \
					(inst) = _arm_alu_reg_imm \
						((inst), (opc), (dreg), (sreg), __alu_imm_save, 1); \
				} \
			} while (0)
#define arm_alu_reg(inst,opc,dreg,sreg)	\
			do { \
				*(inst)++ = arm_always | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							 ((unsigned int)(sreg)); \
			} while (0)
#define arm_alu_reg_cond(inst,opc,dreg,sreg,cond)	\
			do { \
				*(inst)++ = arm_build_prefix((cond), 0) | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							 ((unsigned int)(sreg)); \
			} while (0)

/*
 * Arithmetic or logical operation which sets condition codes.
 */
#define	arm_alu_cc_reg_reg(inst,opc,dreg,sreg1,sreg2)	\
			do { \
				*(inst)++ = arm_always_cc | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg1)) << 16) | \
							 ((unsigned int)(sreg2)); \
			} while (0)
#define	arm_alu_cc_reg_imm8(inst,opc,dreg,sreg,imm)	\
			do { \
				*(inst)++ = arm_always_imm | arm_always_cc | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg)) << 16) | \
							 ((unsigned int)((imm) & 0xFF)); \
			} while (0)
#define arm_alu_cc_reg(inst,opc,dreg,sreg)	\
			do { \
				*(inst)++ = arm_always_cc | \
							(((unsigned int)(opc)) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							 ((unsigned int)(sreg)); \
			} while (0)

/*
 * Test operation, which sets the condition codes but has no other result.
 */
#define arm_test_reg_reg(inst,opc,sreg1,sreg2)	\
			do { \
				arm_alu_cc_reg_reg((inst), (opc), 0, (sreg1), (sreg2)); \
			} while (0)
#define arm_test_reg_imm8(inst,opc,sreg,imm)	\
			do { \
				arm_alu_cc_reg_imm8((inst), (opc), 0, (sreg), (imm)); \
			} while (0)
#define arm_test_reg_imm(inst,opc,sreg,imm)	\
			do { \
				int __test_imm = (int)(imm); \
				if(__test_imm >= 0 && __test_imm < 256) \
				{ \
					arm_alu_cc_reg_imm8((inst), (opc), 0, (sreg), __test_imm); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __test_imm); \
					arm_test_reg_reg((inst), (opc), (sreg), ARM_WORK); \
				} \
			} while (0)

/*
 * Move a value between registers.
 */
#define	arm_mov_reg_reg(inst,dreg,sreg)	\
			do { \
				arm_alu_reg((inst), ARM_MOV, (dreg), (sreg)); \
			} while (0)

/*
 * Move an immediate value into a register.  This is hard because
 * ARM lacks an instruction to load a 32-bit immediate value directly.
 * We handle the simple cases and then bail out to a function for the rest.
 */
#define	arm_mov_reg_imm8(inst,reg,imm)	\
			do { \
				arm_alu_reg_imm8((inst), ARM_MOV, (reg), 0, (imm)); \
			} while (0)
#define	arm_mov_reg_imm8_rotate(inst,reg,imm,rotate)	\
			do { \
				arm_alu_reg_imm8_rotate((inst), ARM_MOV, (reg), \
										0, (imm), (rotate)); \
			} while (0)
extern arm_inst_ptr _arm_mov_reg_imm(arm_inst_ptr inst, int reg, int value);
#define	arm_mov_reg_imm(inst,reg,imm)	\
			do { \
				int __imm = (int)(imm); \
				if(__imm >= 0 && __imm < 256) \
				{ \
					arm_mov_reg_imm8((inst), (reg), __imm); \
				} \
				else if((reg) == ARM_PC) \
				{ \
					(inst) = _arm_mov_reg_imm((inst), ARM_WORK, __imm); \
					arm_mov_reg_reg((inst), ARM_PC, ARM_WORK); \
				} \
				else if(__imm > -256 && __imm < 0) \
				{ \
					arm_mov_reg_imm8((inst), (reg), ~(__imm)); \
					arm_alu_reg((inst), ARM_MVN, (reg), (reg)); \
				} \
				else \
				{ \
					(inst) = _arm_mov_reg_imm((inst), (reg), __imm); \
				} \
			} while (0)

/*
 * Clear a register to zero.
 */
#define	arm_clear_reg(inst,reg)	\
			do { \
				arm_mov_reg_imm8((inst), (reg), 0); \
			} while (0)

/*
 * No-operation instruction.
 */
#define	arm_nop(inst)	arm_mov_reg_reg((inst), ARM_R0, ARM_R0)

/*
 * Perform a shift operation.
 */
#define	arm_shift_reg_reg(inst,opc,dreg,sreg1,sreg2) \
			do { \
				*(inst)++ = arm_always | \
							(((unsigned int)ARM_MOV) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(sreg2)) << 8) | \
							(((unsigned int)(opc)) << 5) | \
							 ((unsigned int)(1 << 4)) | \
							 ((unsigned int)(sreg1)); \
			} while (0)
#define	arm_shift_reg_imm8(inst,opc,dreg,sreg,imm) \
			do { \
				*(inst)++ = arm_always | \
							(((unsigned int)ARM_MOV) << 21) | \
							(((unsigned int)(dreg)) << 12) | \
							(((unsigned int)(opc)) << 5) | \
							(((unsigned int)(imm)) << 7) | \
							 ((unsigned int)(sreg)); \
			} while (0)

/*
 * Perform a multiplication instruction.  Note: ARM instruction rules
 * say that dreg should not be the same as sreg2, so we swap the order
 * of the arguments if that situation occurs.  We assume that sreg1
 * and sreg2 are distinct registers.
 */
#define arm_mul_reg_reg(inst,dreg,sreg1,sreg2)	\
			do { \
				if((dreg) != (sreg2)) \
				{ \
					*(inst)++ = arm_prefix(0x00000090) | \
								(((unsigned int)(dreg)) << 16) | \
								(((unsigned int)(sreg1)) << 8) | \
								 ((unsigned int)(sreg2)); \
				} \
				else \
				{ \
					*(inst)++ = arm_prefix(0x00000090) | \
								(((unsigned int)(dreg)) << 16) | \
								(((unsigned int)(sreg2)) << 8) | \
								 ((unsigned int)(sreg1)); \
				} \
			} while (0)

#if ARM_HAS_MLA

/*
 * Perform a multiply and accumulate operation.
 * dreg = sreg1 + sreg2 * sreg3
 */
#define arm_muladd_reg_reg_reg(inst,cond,dreg,sreg1,sreg2,sreg3)	\
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x00200090) | \
							((dreg) << 16) | \
							((sreg1) << 12) | \
							((sreg2) << 8) | \
							(sreg3)); \
			} while (0)

#endif /* ARM_HAS_MLA */

/*
 * Branch or jump immediate by a byte offset.  The offset is
 * assumed to be +/- 32 Mbytes.
 */
#define	arm_branch_imm(inst,cond,imm)	\
			do { \
				*(inst)++ = arm_build_prefix((cond), 0x0A000000) | \
							(((unsigned int)(((int)(imm)) >> 2)) & \
								0x00FFFFFF); \
			} while (0)
#define	arm_jump_imm(inst,imm)	arm_branch_imm((inst), ARM_CC_AL, (imm))

/*
 * Branch or jump to a specific target location.  The offset is
 * assumed to be +/- 32 Mbytes.
 */
#define	arm_branch(inst,cond,target)	\
			do { \
				int __br_offset = (int)(((unsigned char *)(target)) - \
							           (((unsigned char *)(inst)) + 8)); \
				arm_branch_imm((inst), (cond), __br_offset); \
			} while (0)
#define	arm_jump(inst,target)	arm_branch((inst), ARM_CC_AL, (target))

/*
 * Jump to a specific target location that may be greater than
 * 32 Mbytes away from the current location.
 */
#define	arm_jump_long(inst,target)	\
			do { \
				int __jmp_offset = (int)(((unsigned char *)(target)) - \
							            (((unsigned char *)(inst)) + 8)); \
				if(__jmp_offset >= -0x04000000 && __jmp_offset < 0x04000000) \
				{ \
					arm_jump_imm((inst), __jmp_offset); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_PC, (int)(target)); \
				} \
			} while (0)

/*
 * Back-patch a branch instruction.
 */
#define	arm_patch(inst,target)	\
			do { \
				int __p_offset = (int)(((unsigned char *)(target)) - \
							          (((unsigned char *)(inst)) + 8)); \
				__p_offset = (__p_offset >> 2) & 0x00FFFFFF; \
				*((int *)(inst)) = (*((int *)(inst)) & 0xFF000000) | \
					__p_offset; \
			} while (0)

/*
 * Call a subroutine immediate by a byte offset.
 */
#define	arm_call_imm(inst,imm)	\
			do { \
				*(inst)++ = arm_prefix(0x0B000000) | \
							(((unsigned int)(((int)(imm)) >> 2)) & \
								0x00FFFFFF); \
			} while (0)

/*
 * Call a subroutine at a specific target location.
 */
#define	arm_call(inst,target)	\
			do { \
				int __call_offset = (int)(((unsigned char *)(target)) - \
							             (((unsigned char *)(inst)) + 8)); \
				if(__call_offset >= -0x04000000 && __call_offset < 0x04000000) \
				{ \
					arm_call_imm((inst), __call_offset); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, (int)(target)); \
					arm_mov_reg_reg((inst), ARM_LINK, ARM_PC); \
					arm_mov_reg_reg((inst), ARM_PC, ARM_WORK); \
				} \
			} while (0)

/*
 * Return from a subroutine, where the return address is in the link register.
 */
#define	arm_return(inst)	\
			do { \
				arm_mov_reg_reg((inst), ARM_PC, ARM_LINK); \
			} while (0)

/*
 * Push a register onto the system stack.
 */
#define	arm_push_reg(inst,reg)	\
			do { \
				*(inst)++ = arm_prefix(0x05200004) | \
							(((unsigned int)ARM_SP) << 16) | \
							(((unsigned int)(reg)) << 12); \
			} while (0)

/*
 * Pop a register from the system stack.
 */
#define	arm_pop_reg(inst,reg)	\
			do { \
				*(inst)++ = arm_prefix(0x04900004) | \
							(((unsigned int)ARM_SP) << 16) | \
							(((unsigned int)(reg)) << 12); \
			} while (0)

/*
 * Load a word value from a pointer and then advance the pointer.
 */
#define	arm_load_advance(inst,dreg,sreg)	\
			do { \
				*(inst)++ = arm_prefix(0x04900004) | \
							(((unsigned int)(sreg)) << 16) | \
							(((unsigned int)(dreg)) << 12); \
			} while (0)

#define arm_load_membase_extend(inst,cond,reg,basereg,imm,mask) \
			do { \
				int __lmbx_offset = (int)(imm); \
				if((__lmbx_offset > -256) && (__lmbx_offset < 256)) \
				{ \
					unsigned int __lmbx_u; \
					if(__lmbx_offset >= 0) \
					{ \
						__lmbx_u = 0x00800000; \
					} \
					else \
					{ \
						__lmbx_offset = -__lmbx_offset; \
						__lmbx_u = 0; \
					} \
					*(inst)++ = arm_build_prefix((cond), (mask)) | \
								(__lmbx_u) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								((((unsigned int)__lmbx_offset) & 0xf0) << 4) | \
								(((unsigned int)__lmbx_offset) & 0x0f); \
				} \
				else \
				{ \
					arm_alu_reg_imm((inst), ARM_ADD, ARM_WORK, (basereg), __lmbx_offset); \
					*(inst)++ = arm_build_prefix((cond), (mask)) | \
								(((unsigned int)(ARM_WORK)) << 16) | \
								(((unsigned int)(reg)) << 12); \
				} \
			} while (0)

/*
 * Load a value from an address into a register.
 */
#define arm_load_membase_either(inst,reg,basereg,imm,mask)	\
			do { \
				int __mb_offset = (int)(imm); \
				if(__mb_offset >= 0 && __mb_offset < (1 << 12)) \
				{ \
					*(inst)++ = arm_prefix(0x05900000 | (mask)) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								 ((unsigned int)__mb_offset); \
				} \
				else if(__mb_offset > -(1 << 12) && __mb_offset < 0) \
				{ \
					*(inst)++ = arm_prefix(0x05100000 | (mask)) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								 ((unsigned int)(-__mb_offset)); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __mb_offset); \
					*(inst)++ = arm_prefix(0x07900000 | (mask)) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								 ((unsigned int)ARM_WORK); \
				} \
			} while (0)
#define	arm_load_membase(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_either((inst), (reg), (basereg), (imm), 0); \
			} while (0)
#define	arm_load_membase_byte(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_either((inst), (reg), (basereg), (imm), \
										0x00400000); \
			} while (0)

#if ARM_HAS_LDRSB

#define	arm_load_membase_sbyte(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_extend((inst), ARM_CC_AL, (reg), (basereg), (imm), 0x015000d0); \
			} while (0)

#else /* !ARM_HAS_LDRSB */

#define	arm_load_membase_sbyte(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_either((inst), (reg), (basereg), (imm), \
										0x00400000); \
				arm_shift_reg_imm8((inst), ARM_SHL, (reg), (reg), 24); \
				arm_shift_reg_imm8((inst), ARM_SAR, (reg), (reg), 24); \
			} while (0)

#endif /* !ARM_HAS_LDRSB */

#define	arm_load_membase_ushort(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_extend((inst), ARM_CC_AL, (reg), (basereg), (imm), 0x015000b0); \
			} while (0)

#if ARM_HAS_LDRSH

#define	arm_load_membase_short(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_extend((inst), ARM_CC_AL, (reg), (basereg), (imm), 0x015000f0); \
			} while (0)

#else /* !ARM_HAS_LDRSH */

#define	arm_load_membase_short(inst,reg,basereg,imm)	\
			do { \
				arm_load_membase_byte((inst), ARM_WORK, (basereg), (imm)); \
				arm_load_membase_byte((inst), (reg), (basereg), (imm) + 1); \
				arm_shift_reg_imm8((inst), ARM_SHL, (reg), (reg), 24); \
				arm_shift_reg_imm8((inst), ARM_SAR, (reg), (reg), 16); \
				arm_alu_reg_reg((inst), ARM_ORR, (reg), (reg), ARM_WORK); \
			} while (0)

#endif /* !ARM_HAS_LDRSH */

/*
 * Store a value from a register into an address.
 *
 * Note: storing a 16-bit value destroys the value in the register.
 */
#define arm_store_membase_either(inst,reg,basereg,imm,mask)	\
			do { \
				int __sm_offset = (int)(imm); \
				if(__sm_offset >= 0 && __sm_offset < (1 << 12)) \
				{ \
					*(inst)++ = arm_prefix(0x05800000 | (mask)) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								 ((unsigned int)__sm_offset); \
				} \
				else if(__sm_offset > -(1 << 12) && __sm_offset < 0) \
				{ \
					*(inst)++ = arm_prefix(0x05000000 | (mask)) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								 ((unsigned int)(-__sm_offset)); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __sm_offset); \
					*(inst)++ = arm_prefix(0x07800000 | (mask)) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								 ((unsigned int)ARM_WORK); \
				} \
			} while (0)
#define	arm_store_membase(inst,reg,basereg,imm)	\
			do { \
				arm_store_membase_either((inst), (reg), (basereg), (imm), 0); \
			} while (0)
#define	arm_store_membase_byte(inst,reg,basereg,imm)	\
			do { \
				arm_store_membase_either((inst), (reg), (basereg), (imm), \
										 0x00400000); \
			} while (0)
#define	arm_store_membase_sbyte(inst,reg,basereg,imm)	\
			do { \
				arm_store_membase_byte((inst), (reg), (basereg), (imm)); \
			} while (0)
#define	arm_store_membase_short(inst,reg,basereg,imm)	\
			do { \
				int __smbi16_offset = (int)(imm); \
				if((__smbi16_offset > -256) && (__smbi16_offset < 256)) \
				{ \
					unsigned int __smbi16_u; \
					if(__smbi16_offset < 0) \
					{ \
						__smbi16_offset = -__smbi16_offset; \
						__smbi16_u = 0; \
					} \
					else \
					{ \
						__smbi16_u = 0x00800000; \
					} \
					*(inst)++ = arm_build_prefix(ARM_CC_AL, 0x014000b0) | \
								(__smbi16_u) | \
								(((unsigned int)(basereg)) << 16) | \
								(((unsigned int)(reg)) << 12) | \
								((((unsigned int)__smbi16_offset) & 0xf0) << 4) | \
								(((unsigned int)__smbi16_offset) & 0x0f); \
				} \
				else \
				{ \
					arm_store_membase_either((inst), (reg), (basereg), (imm), \
											 0x00400000); \
					arm_shift_reg_imm8((inst), ARM_SHR, (reg), (reg), 8); \
					arm_store_membase_either((inst), (reg), (basereg), \
											 (imm) + 1, 0x00400000); \
				} \
			} while (0)
#define	arm_store_membase_ushort(inst,reg,basereg,imm)	\
			do { \
				arm_store_membase_short((inst), (reg), (basereg), (imm)); \
			} while (0)

/*
 * Load a value from an indexed address into a register.
 */
#define arm_load_memiindex_extend(inst,cond,reg,basereg,indexreg,mask) \
			do { \
				*(inst)++ = arm_build_prefix((cond), (mask)) | \
							(((unsigned int)(basereg)) << 16) | \
							(((unsigned int)(reg)) << 12) | \
							(((unsigned int)(indexreg))); \
			} while (0)
#define arm_load_memindex_either(inst,reg,basereg,indexreg,shift,mask)	\
			do { \
				*(inst)++ = arm_prefix(0x07900000 | (mask)) | \
							(((unsigned int)(basereg)) << 16) | \
							(((unsigned int)(reg)) << 12) | \
							(((unsigned int)(shift)) << 7) | \
							 ((unsigned int)(indexreg)); \
			} while (0)
#define	arm_load_memindex(inst,reg,basereg,indexreg)	\
			do { \
				arm_load_memindex_either((inst), (reg), (basereg), \
										 (indexreg), 2, 0); \
			} while (0)
#define	arm_load_memindex_byte(inst,reg,basereg,indexreg)	\
			do { \
				arm_load_memindex_either((inst), (reg), (basereg), \
									     (indexreg), 0, 0x00400000); \
			} while (0)

#if ARM_HAS_LDRSB

#define	arm_load_memindex_sbyte(inst,reg,basereg,indexreg)	\
			do { \
				arm_load_memiindex_extend((inst), ARM_CC_AL, (reg), (basereg), (indexreg), 0x019000d0); \
			} while (0)

#else /* !ARM_HAS_LDRSB */

#define	arm_load_memindex_sbyte(inst,reg,basereg,indexreg)	\
			do { \
				arm_load_memindex_either((inst), (reg), (basereg), \
									     (indexreg), 0, 0x00400000); \
				arm_shift_reg_imm8((inst), ARM_SHL, (reg), (reg), 24); \
				arm_shift_reg_imm8((inst), ARM_SAR, (reg), (reg), 24); \
			} while (0)

#endif /* !ARM_HAS_LDRSB */

#define	arm_load_memindex_ushort(inst,reg,basereg,indexreg)	\
			do { \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, (indexreg), (indexreg)); \
				arm_load_memiindex_extend((inst), ARM_CC_AL, (reg), (basereg), ARM_WORK, 0x019000b0); \
			} while (0)

#if ARM_HAS_LDRSH

#define	arm_load_memindex_short(inst,reg,basereg,indexreg)	\
			do { \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, (indexreg), (indexreg)); \
				arm_load_memiindex_extend((inst), ARM_CC_AL, (reg), (basereg), ARM_WORK, 0x019000f0); \
			} while (0)

#else /* !ARM_HAS_LDRSH */

#define	arm_load_memindex_short(inst,reg,basereg,indexreg)	\
			do { \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, (basereg), \
								(indexreg)); \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
								(indexreg)); \
				arm_load_membase_byte((inst), (reg), ARM_WORK, 0); \
				arm_load_membase_byte((inst), ARM_WORK, ARM_WORK, 1); \
				arm_shift_reg_imm8((inst), ARM_SHL, ARM_WORK, ARM_WORK, 24); \
				arm_shift_reg_imm8((inst), ARM_SAR, ARM_WORK, ARM_WORK, 16); \
				arm_alu_reg_reg((inst), ARM_ORR, (reg), (reg), ARM_WORK); \
			} while (0)

#endif /* !ARM_HAS_LDRSH */

/*
 * Store a value from a register into an indexed address.
 */
#define arm_store_memindex_either(inst,reg,basereg,indexreg,shift,mask)	\
			do { \
				*(inst)++ = arm_prefix(0x07800000 | (mask)) | \
							(((unsigned int)(basereg)) << 16) | \
							(((unsigned int)(reg)) << 12) | \
							(((unsigned int)(shift)) << 7) | \
							 ((unsigned int)(indexreg)); \
			} while (0)
#define	arm_store_memindex(inst,reg,basereg,indexreg)	\
			do { \
				arm_store_memindex_either((inst), (reg), (basereg), \
										  (indexreg), 2, 0); \
			} while (0)
#define	arm_store_memindex_byte(inst,reg,basereg,indexreg)	\
			do { \
				arm_store_memindex_either((inst), (reg), (basereg), \
										  (indexreg), 0, 0x00400000); \
			} while (0)
#define	arm_store_memindex_sbyte(inst,reg,basereg,indexreg)	\
			do { \
				arm_store_memindex_byte((inst), (reg), (basereg), \
										(indexreg)); \
			} while (0)
#define	arm_store_memindex_short(inst,reg,basereg,indexreg)	\
			do { \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, (indexreg), (indexreg)); \
				*(inst)++ = arm_build_prefix(ARM_CC_AL, 0x018000b0) | \
							(((unsigned int)(basereg)) << 16) | \
							(((unsigned int)(reg)) << 12) | \
							ARM_WORK; \
			} while (0)
#define	arm_store_memindex_ushort(inst,reg,basereg,indexreg)	\
			do { \
				arm_store_memindex_short((inst), (reg), \
										 (basereg), (indexreg)); \
			} while (0)

#ifdef __VFP_FP__

/*
 * Instructions valid only with VFP support.
 */

/*
 * Move a vfp system register to an arm register
 */
#define arm_mov_vfpsreg_reg(inst,cond,reg,vfpsreg) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0ef00a10) | \
							 ((vfpsreg) << 16) | \
							 ((reg) << 12)); \
			} while (0)

/*
 * Move the vfp status register to the arm's cpsr
 */
#define arm_mov_vfpstatus(inst) \
			do { \
				arm_mov_vfpsreg_reg(inst, ARM_CC_AL, ARM_R15, ARMVFP_FPSCR); \
			} while (0)

/*
 * Load a floatingpoint value from one or two arm registers.
 */
#define arm_load_reg_single(inst,cond,vfpreg,reg) \
			do { \
				int __load_reg = (int)(vfpreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0e000010) | \
							((__load_reg) & 0x1e) << 15 | \
							(reg) << 12 | \
							((__load_reg) & 1) << 7 | \
							(ARMVFP_CSINGLE) << 8); \
			} while (0)

#define arm_load_reg_double(inst,cond,vfpreg,regl,regh) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0e000010) | \
							(vfpreg) << 16 | \
							(regl) << 12 | \
							(ARMVFP_CDOUBLE) << 8); \
				*(inst)++ = (arm_build_prefix((cond), 0x0e200010) | \
							(vfpreg) << 16 | \
							(regh) << 12 | \
							(ARMVFP_CDOUBLE) << 8); \
			} while (0)

/*
 * Load a floatingpoint value from an address in a register
 */
#define arm_load_regbase_single(inst, cond, reg, basereg) \
			do { \
				int __load_reg = (reg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0d100000) | \
							((__load_reg) & 1) << 22 | \
							(basereg) << 16 | \
							((__load_reg) & 0x1e) << 11 | \
							(ARMVFP_CSINGLE) << 8); \
			} while (0)

#define arm_load_regbase_double(inst, cond, reg, basereg) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0d100000) | \
							(basereg) << 16 | \
							(reg) << 12 | \
							(ARMVFP_CDOUBLE) << 8 | \
							0); \
			} while (0)

/*
 * Load a floatingpoint value from an absolute address.
 */
#define arm_load_mem_single(inst, cond, reg, mem) \
			do { \
				arm_mov_reg_imm((inst), ARM_WORK, (mem)); \
				arm_load_regbase_single((inst), (cond), (reg), ARM_WORK); \
			} while (0)

#define arm_load_mem_double(inst, cond, reg, mem) \
			do { \
				arm_mov_reg_imm((inst), ARM_WORK, (mem)); \
				arm_load_regbase_double((inst), (cond), (reg), ARM_WORK); \
			} while (0)

/*
 * Load a floatingpoint value from an address in a register and an offset.
 */
#define arm_load_membase_single(inst, cond, reg, basereg, imm) \
			do { \
				int __load_reg = (reg); \
				int __mb_imm = (int)(imm); \
				if(((__mb_imm & 3) == 0) && \
					(__mb_imm < (1 << 10)) && \
					(__mb_imm > -(1 << 10))) \
				{ \
					int __load_u = ((__mb_imm > 0) ? 1 : 0); \
					if(!__load_u) \
					{ \
						__mb_imm = -__mb_imm; \
					} \
					__mb_imm >>= 2; \
					*(inst)++ = (arm_build_prefix((cond), 0x0d100000) | \
								(__load_u << 23) | \
								((__load_reg) & 1) << 22 | \
								(basereg) << 16 | \
								((__load_reg) & 0x1e) << 11 | \
								(ARMVFP_CSINGLE) << 8 | \
								__mb_imm); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __mb_imm); \
					arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
									(basereg)); \
					arm_load_regbase_single((inst), (cond), (reg), ARM_WORK); \
				} \
			} while (0)

#define arm_load_membase_double(inst, cond, reg, basereg, imm) \
			do { \
				int __sm_imm = (int)(imm); \
				if(((__sm_imm & 3) == 0) && \
					(__sm_imm < (1 << 10)) && \
					(__sm_imm > -(1 << 10))) \
				{ \
					int __u = ((__sm_imm > 0) ? 1 : 0); \
					if(!__u) \
					{ \
						__sm_imm = -__sm_imm; \
					} \
					__sm_imm >>= 2; \
					*(inst)++ = (arm_build_prefix((cond), 0x0d100000) | \
								(__u << 23) | \
								(basereg) << 16 | \
								(reg) << 12 | \
								(ARMVFP_CDOUBLE) << 8 | \
								__sm_imm); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __sm_imm); \
					arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
									(basereg)); \
					arm_load_regbase_double((inst), (cond), (reg), ARM_WORK); \
				} \
			} while (0)

/*
 * Load a floatingpoint value from a register and an index.
 */
#define arm_load_memindex_single(inst, cond, reg, basereg, indexreg) \
			do { \
                arm_shift_reg_imm8((inst), ARM_SHL, ARM_WORK, (indexreg), 2); \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
								(basereg)); \
				arm_load_regbase_single((inst), (cond), (reg), ARM_WORK); \
			} while (0)

#define arm_load_memindex_double(inst, cond, reg, basereg, indexreg) \
			do { \
                arm_shift_reg_imm8((inst), ARM_SHL, ARM_WORK, (indexreg), 3); \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
								(basereg)); \
				arm_load_regbase_double((inst), (cond), (reg), ARM_WORK); \
			} while (0)


/*
 * Store a floatingpoint register to one or two arm registers.
 */
#define arm_store_reg_single(inst, cond, reg, vfpreg) \
			do { \
				int __reg = (int)(vfpreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0e100010) | \
							((__reg) & 0x1e) << 15 | \
							(reg) << 12 | \
							(ARMVFP_CSINGLE) << 8 | \
							((__reg) & 1) << 7); \
				} while (0)

#define arm_store_reg_double(inst, cond, regl, regh, vfpreg) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0e100010) | \
							(vfpreg) << 16 | \
							(regl) << 12 | \
							(ARMVFP_CDOUBLE) << 8); \
				*(inst)++ = (arm_build_prefix((cond), 0x0e300010) | \
							(vfpreg) << 16 | \
							(regh) << 12 | \
							(ARMVFP_CDOUBLE) << 8); \
				} while (0)


/*
 * Store a floatingpoint value to an address in a register.
 */
#define arm_store_regbase_single(inst, cond, reg, basereg) \
			do { \
				int __reg = (reg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0d000000) | \
							((__reg) & 1) << 22 | \
							(basereg) << 16 | \
							((__reg) & 0x1e) << 11 | \
							(ARMVFP_CSINGLE) << 8 | \
							0); \
			} while (0)

#define arm_store_regbase_double(inst, cond, reg, basereg) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0d000000) | \
							(basereg) << 16 | \
							(reg) << 12 | \
							(ARMVFP_CDOUBLE) << 8 | \
							0); \
			} while (0)

#define arm_store_membase_single(inst, cond, reg, basereg, imm) \
			do { \
				int __reg = (reg); \
				int __sm_imm = (int)(imm); \
				if(((__sm_imm & 3) == 0) && \
					(__sm_imm < (1 << 10)) && \
					(__sm_imm > -(1 << 10))) \
				{ \
					int __u = ((__sm_imm > 0) ? 1 : 0); \
					if(!__u) \
					{ \
						__sm_imm = -__sm_imm; \
					} \
					__sm_imm >>= 2; \
					*(inst)++ = (arm_build_prefix((cond), 0x0d000000) | \
								(__u << 23) | \
								((__reg) & 1) << 22 | \
								(basereg) << 16 | \
								((__reg) & 0x1e) << 11 | \
								(ARMVFP_CSINGLE) << 8 | \
								__sm_imm); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __sm_imm); \
					arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
									(basereg)); \
					arm_store_regbase_single((inst), (cond), (reg), ARM_WORK); \
				} \
			} while (0)

#define arm_store_membase_double(inst, cond, reg, basereg, imm) \
			do { \
				int __sm_imm = (int)(imm); \
				if(((__sm_imm & 3) == 0) && \
					(__sm_imm < (1 << 10)) && \
					(__sm_imm > -(1 << 10))) \
				{ \
					int __u = ((__sm_imm > 0) ? 1 : 0); \
					if(!__u) \
					{ \
						__sm_imm = -__sm_imm; \
					} \
					__sm_imm >>= 2; \
					*(inst)++ = (arm_build_prefix((cond), 0x0d000000) | \
								(__u << 23) | \
								(basereg) << 16 | \
								(reg) << 12 | \
								(ARMVFP_CDOUBLE) << 8 | \
								__sm_imm); \
				} \
				else \
				{ \
					arm_mov_reg_imm((inst), ARM_WORK, __sm_imm); \
					arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
									(basereg)); \
					arm_store_regbase_double((inst), (cond), (reg), ARM_WORK); \
				} \
			} while (0)

#define arm_store_memindex_single(inst, cond, reg, basereg, indexreg) \
			do { \
                arm_shift_reg_imm8((inst), ARM_SHL, ARM_WORK, (indexreg), 2); \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
								(basereg)); \
				arm_store_regbase_single((inst), (cond), (reg), ARM_WORK); \
			} while (0)

#define arm_store_memindex_double(inst, cond, reg, basereg, indexreg) \
			do { \
                arm_shift_reg_imm8((inst), ARM_SHL, ARM_WORK, (indexreg), 3); \
				arm_alu_reg_reg((inst), ARM_ADD, ARM_WORK, ARM_WORK, \
								(basereg)); \
				arm_store_regbase_double((inst), (cond), (reg), ARM_WORK); \
			} while (0)

/*
 * Conversion macros
 */

/*
 * Convert from single precision to double precision
 */
#define arm_cvt_single_double_reg_reg(inst, cond, dreg, sreg) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb700c0) | \
							((dreg) << 12) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1) | \
							((ARMVFP_CSINGLE) << 8)); \
			} while (0)

/*
 * Convert from double precision to single precision
 */
#define arm_cvt_double_single_reg_reg(inst, cond, sreg, dreg) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb700c0) | \
							((__cvt_sreg & 1) << 22) | \
							((__cvt_sreg & 0x1e) << 11) | \
							((ARMVFP_CDOUBLE) << 8) | \
							(dreg)); \
			} while (0)

/*
 * Convert from signed integer in a single precision register to a
 * single precision floatingpoint value.  FSITOS
 */
#define arm_cvt_si_single(inst, cond, dreg, sreg) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				int __cvt_dreg = (int)(dreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb800c0) | \
							((__cvt_dreg & 1) << 22) | \
							((__cvt_dreg & 0x1e) << 11) | \
							((ARMVFP_CSINGLE) << 8) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Convert from signed integer in a single precision register to a
 * double precision floatingpoint value.  FSITOD
 */
#define arm_cvt_si_double(inst, cond, dreg, sreg) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb800c0) | \
							((dreg) << 12) | \
							((ARMVFP_CDOUBLE) << 8) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Convert from unsigned integer in a single precision register to a
 * single precision floatingpoint value.  FUITOS
 */
#define arm_cvt_ui_single(inst, cond, dreg, sreg) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				int __cvt_dreg = (int)(dreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb80040) | \
							((__cvt_dreg & 1) << 22) | \
							((__cvt_dreg & 0x1e) << 11) | \
							((ARMVFP_CSINGLE) << 8) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Convert from unsigned integer in a single precision register to a
 * double precision floatingpoint value.  FSITOD
 */
#define arm_cvt_ui_double(inst, cond, dreg, sreg) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb80040) | \
							((dreg) << 12) | \
							((ARMVFP_CDOUBLE) << 8) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Convert from single precision to a signed integer in a single
 * precision register. Set to_zero != 0 if the truncate rounding mode
 * has to be used regardless of the settings in the control register.
 * FTOSI(Z)S
 */
#define arm_cvt_single_si(inst, cond, dreg, sreg, to_zero) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				int __cvt_dreg = (int)(dreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0ebd0040) | \
							((__cvt_dreg & 1) << 22) | \
							((__cvt_dreg & 0x1e) << 11) | \
							((ARMVFP_CSINGLE) << 8) | \
							((((to_zero) != 0) ? 1 : 0) << 7) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Convert from double precision to a signed integer in a single
 * precision register. Set to_zero != 0 if the truncate rounding mode
 * has to be used regardless of the settings in the control register.
 * FTOSI(Z)D
 */
#define arm_cvt_double_si(inst, cond, dreg, sreg, to_zero) \
			do { \
				int __cvt_dreg = (int)(dreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0ebd0040) | \
							((__cvt_dreg & 1) << 22) | \
							((__cvt_dreg & 0x1e) << 11) | \
							((ARMVFP_CDOUBLE) << 8) | \
							((((to_zero) != 0) ? 1 : 0) << 7) | \
							(sreg)); \
			} while (0)

/*
 * Convert from single precision to an unsigned integer in a single
 * precision register. Set to_zero != 0 if the truncate rounding mode
 * has to be used regardless of the settings in the control register.
 * FTOUI(Z)S
 */
#define arm_cvt_single_ui(inst, cond, dreg, sreg, to_zero) \
			do { \
				int __cvt_sreg = (int)(sreg); \
				int __cvt_dreg = (int)(dreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0ebc0040) | \
							((__cvt_dreg & 1) << 22) | \
							((__cvt_dreg & 0x1e) << 11) | \
							((ARMVFP_CSINGLE) << 8) | \
							((((to_zero) != 0) ? 1 : 0) << 7) | \
							((__cvt_sreg & 1) << 5) | \
							((__cvt_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Convert from double precision to an unsigned integer in a single
 * precision register. Set to_zero != 0 if the truncate rounding mode
 * has to be used regardless of the settings in the control register.
 * FTOUI(Z)D
 */
#define arm_cvt_double_ui(inst, cond, dreg, sreg, to_zero) \
			do { \
				int __cvt_dreg = (int)(dreg); \
				*(inst)++ = (arm_build_prefix((cond), 0x0ebc0040) | \
							((__cvt_dreg & 1) << 22) | \
							((__cvt_dreg & 0x1e) << 11) | \
							((ARMVFP_CDOUBLE) << 8) | \
							((((to_zero) != 0) ? 1 : 0) << 7) | \
							(sreg)); \
			} while (0)

/*
 * Compare two floatingpoint values (nonsignaling)  FCMPx.
 */
#define arm_cmpn_single_reg_reg(inst,cond,sreg1,sreg2) \
			do { \
				int __cmpn_sreg1 = (int)(sreg1); \
				int __cmpn_sreg2 = (int)(sreg2); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb40040) | \
							((__cmpn_sreg1 & 1) << 22) | \
							((__cmpn_sreg1 & 0x1e) << 11) | \
							((ARMVFP_CSINGLE) << 8) | \
							((__cmpn_sreg2 & 1) << 5) | \
							((__cmpn_sreg2 & 0x1e) >> 1)); \
			} while (0)

#define arm_cmpn_double_reg_reg(inst,cond,sreg1,sreg2) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb40040) | \
							((sreg1) << 12) | \
							((ARMVFP_CDOUBLE) << 8) | \
							(sreg2)); \
			} while (0)

/*
 * Compare two floatingpoint values (signaling)  FCMPEx.
 */
#define arm_cmps_single_reg_reg(inst,cond,sreg1,sreg2) \
			do { \
				int __cmps_sreg1 = (int)(sreg1); \
				int __cmps_sreg2 = (int)(sreg2); \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb400c0) | \
							((__cmps_sreg1 & 1) << 22) | \
							((__cmps_sreg1 & 0x1e) << 11) | \
							((ARMVFP_CSINGLE) << 8) | \
							((__cmps_sreg2 & 1) << 5) | \
							((__cmps_sreg2 & 0x1e) >> 1)); \
			} while (0)

#define arm_cmps_double_reg_reg(inst,cond,sreg1,sreg2) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), 0x0eb400c0) | \
							((sreg1) << 12) | \
							((ARMVFP_CDOUBLE) << 8) | \
							(sreg2)); \
			} while (0)

/*
 * Emit an unary operation for single precision floatingpoint values.
 */
#define arm_unary_single_reg(inst,cond,dreg,sreg,mask) \
			do { \
				int __uns_dreg = (int)(dreg); \
				int __uns_sreg = (int)(sreg); \
				*(inst)++ = (arm_build_prefix((cond), (mask)) | \
							((__uns_dreg & 1) << 22) | \
							((__uns_dreg & 0x1e) << 11) | \
							(ARMVFP_CSINGLE) << 8 | \
							((__uns_sreg & 1) << 5) | \
							((__uns_sreg & 0x1e) >> 1)); \
			} while (0)

/*
 * Emit an unary operation for double precision floatingpoint values.
 */
#define arm_unary_double_reg(inst,cond,dreg,sreg,mask) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), (mask)) | \
							((dreg) << 12) | \
							((ARMVFP_CDOUBLE) << 8) | \
							(sreg)); \
			} while (0)

/*
 * Emit a binary operation for single precision floatingpoint values.
 */
#define arm_binary_single_reg_reg(inst,cond,dreg,sreg1,sreg2,mask) \
			do { \
				int __bins_dreg = (int)(dreg); \
				int __bins_sreg1 = (int)(sreg1); \
				int __bins_sreg2 = (int)(sreg2); \
				*(inst)++ = (arm_build_prefix((cond), (mask)) | \
							((__bins_dreg & 1) << 22) | \
							((__bins_sreg1 & 0x1e) << 15) | \
							((__bins_dreg & 0x1e) << 11) | \
							(ARMVFP_CSINGLE) << 8 | \
							((__bins_sreg1 & 1) << 7) | \
							((__bins_sreg2 & 1) << 5) | \
							((__bins_sreg2 & 0x1e) >> 1)); \
			} while (0)

/*
 * Emit a binary operation for double precision floatingpoint values.
 */
#define arm_binary_double_reg_reg(inst,cond,dreg,sreg1,sreg2,mask) \
			do { \
				*(inst)++ = (arm_build_prefix((cond), (mask)) | \
							((sreg1) << 16) | \
							((dreg) << 12) | \
							(ARMVFP_CDOUBLE) << 8 | \
							(sreg2)); \
			} while (0)

/*
 * Add two floatingpoint values.
 */
#define arm_add_single_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_single_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e300000)

#define arm_add_double_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_double_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e300000)

/*
 * Subtract two floatingpoint values.
 */
#define arm_sub_single_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_single_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e300040)

#define arm_sub_double_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_double_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e300040)

/*
 * Multiply two floatingpoint values.
 */
#define arm_mul_single_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_single_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e200000)

#define arm_mul_double_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_double_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e200000)

/*
 * Divide two floatingpoint values.
 */
#define arm_div_single_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_single_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e800000)

#define arm_div_double_reg_reg(inst,cond,dreg,sreg1,sreg2) \
			arm_binary_double_reg_reg((inst), (cond), (dreg), (sreg1), (sreg2), 0x0e800000)

/*
 * Get the absolute of a floatingpoint value.
 */
#define arm_abs_single_reg(inst,cond,dreg,sreg) \
			arm_unary_single_reg((inst), (cond), (dreg), (sreg), 0x0eb000c0)

#define arm_abs_double_reg(inst,cond,dreg,sreg) \
			arm_unary_double_reg((inst), (cond), (dreg), (sreg), 0x0eb000c0)

/*
 * Negate a floatingpoint value.
 */
#define arm_neg_single_reg(inst,cond,dreg,sreg) \
			arm_unary_single_reg((inst), (cond), (dreg), (sreg), 0x0eb10040)

#define arm_neg_double_reg(inst,cond,dreg,sreg) \
			arm_unary_double_reg((inst), (cond), (dreg), (sreg), 0x0eb10040)

/*
 * Get the sqare root of a floatingpoint value.
 */
#define arm_sqrt_single_reg(inst,cond,dreg,sreg) \
			arm_unary_single_reg((inst), (cond), (dreg), (sreg), 0x0eb100c0)

#define arm_sqrt_double_reg(inst,cond,dreg,sreg) \
			arm_unary_double_reg((inst), (cond), (dreg), (sreg), 0x0eb100c0)

#endif /* __VFP_FP__ */

#ifdef __cplusplus
};
#endif

#endif /* _ARM_CODEGEN_H */
