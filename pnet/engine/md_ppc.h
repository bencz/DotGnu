/*
 * md_ppc.h - Machine-dependent definitions for PowerPC.
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

#ifndef	_ENGINE_MD_PPC_H
#define	_ENGINE_MD_PPC_H

#include "ppc_codegen.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Register numbers in the standard register allocation order.
 * -1 is the list terminator.
 *
 * The register allocation order is the same as that used by gcc
 * for general purpose registers that aren't saved.
 *
 * Note: "r12" is designated as a work register by "ppc_codegen.h"
 * so we cannot use it for general allocation here.
 */
#define	MD_REG_0		PPC_R9
#define	MD_REG_1		PPC_R11
#define	MD_REG_2		PPC_R10
#define	MD_REG_3		PPC_R8
#define	MD_REG_4		PPC_R7
#define	MD_REG_5		PPC_R6
#define	MD_REG_6		PPC_R5
#define	MD_REG_7		PPC_R4
#define	MD_REG_8		PPC_R3
#define	MD_REG_9		-1
#define	MD_REG_10		-1
#define	MD_REG_11		-1
#define	MD_REG_12		-1
#define	MD_REG_13		-1
#define	MD_REG_14		-1
#define	MD_REG_15		-1

/*
 * Mask that indicates a floating-point register.
 */
#define	MD_FREG_MASK	0x0010

/*
 * Floating point register numbers in the standard allocation order.
 * -1 is the list terminator.  The floating point register numbers
 * must include the MD_FREG_MASK value.
 */
#define	MD_FREG_0		-1 /* TODO: enable FP */
//#define	MD_FREG_0		(MD_FREG_MASK | PPC_F0)
#define	MD_FREG_1		(MD_FREG_MASK | PPC_F1)
#define	MD_FREG_2		(MD_FREG_MASK | PPC_F2)
#define	MD_FREG_3		(MD_FREG_MASK | PPC_F3)
#define	MD_FREG_4		(MD_FREG_MASK | PPC_F4)
#define	MD_FREG_5		(MD_FREG_MASK | PPC_F5)
#define	MD_FREG_6		(MD_FREG_MASK | PPC_F6)
#define	MD_FREG_7		(MD_FREG_MASK | PPC_F7)
#define	MD_FREG_8		(MD_FREG_MASK | PPC_F8)
#define	MD_FREG_9		(MD_FREG_MASK | PPC_F9)
#define	MD_FREG_10		(MD_FREG_MASK | PPC_F10)
#define	MD_FREG_11		(MD_FREG_MASK | PPC_F11)
#define	MD_FREG_12		(MD_FREG_MASK | PPC_F12)
#define	MD_FREG_13		(MD_FREG_MASK | PPC_F13)
#define	MD_FREG_14		(MD_FREG_MASK | PPC_F14)
#define	MD_FREG_15		(MD_FREG_MASK | PPC_F15)

/*
 * Set this to a non-zero value if floating-point registers are organised
 * in a stack (e.g. the x87 FPU).
 */
#define	MD_FP_STACK_SIZE	0

/*
 * The register that contains the CVM program counter.  This may be
 * present in the standard register allocation order.  This can be
 * set to -1 if MD_STATE_ALREADY_IN_REGS is 0.
 */
#define	MD_REG_PC		PPC_R18

/*
 * The register that contains the CVM stacktop pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_STACK	PPC_R19

/*
 * The register that contains the CVM frame pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_FRAME	PPC_R20

/*
 * Set this to 1 if "pc", "stacktop", and "frame" are already in
 * the above registers when unrolled code is entered.  i.e. the
 * CVM interpreter has manual assignments of registers to variables
 * in the file "cvm.c".  If the state is not already in registers,
 * then set this value to zero.
 */
#define	MD_STATE_ALREADY_IN_REGS	1

/*
 * Registers that must be saved on the system stack prior to their use
 * in unrolled code for temporary stack values.
 */
#define	MD_REGS_TO_BE_SAVED		0

/*
 * Registers with special meanings (pc, stacktop, frame) that must
 * be saved if MD_STATE_ALREADY_IN_REGS is 0.
 */
#define	MD_SPECIAL_REGS_TO_BE_SAVED	0

/*
 * Set this to 1 if the CPU has integer division operations.
 * Set it to zero if integer division is too hard to be performed
 * inline using a simple opcode.
 */
#define	MD_HAS_INT_DIVISION			1

/*
 * Set to 1 if 64-bit register pairs are stored in little-endian order.
 */
#define	MD_LITTLE_ENDIAN_LONGS		0

/*
 * Type of the instruction pointer for outputting code.
 */
typedef ppc_inst_ptr	md_inst_ptr;

/*
 * Push a word register onto the system stack.
 */
#define	md_push_reg(inst,reg)	ppc_push_reg((inst), (reg))

/*
 * Pop a word register from the system stack.
 */
#define	md_pop_reg(inst,reg)	ppc_pop_reg((inst), (reg))

/*
 * Discard the contents of a floating-point register.
 */
#define	md_discard_freg(inst,reg)	do { ; } while (0)

/*
 * Load a 32-bit integer constant into a register.  This will sign-extend
 * if the native word size is larger.
 */
#define	md_load_const_32(inst,reg,value)	\
			ppc_mov_reg_imm((inst), (reg), (value))

/*
 * Load a native integer constant into a register.
 */
#define	md_load_const_native(inst,reg,value)	\
			ppc_mov_reg_imm((inst), (reg), (value))

/*
 * Load a 32-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_32(inst,freg,mem)	\
			do { \
				ppc_mov_reg_imm((inst), PPC_WORK, mem);\
				ppc_load_membase_float_32((inst), (freg) & (~MD_FREG_MASK), PPC_WORK, 0);\
			}while(0)

/*
 * Load a 64-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_64(inst,freg,mem)   \
			do { \
				ppc_mov_reg_imm((inst), PPC_WORK, mem);\
				ppc_load_membase_float_64((inst), (freg) & (~MD_FREG_MASK), PPC_WORK, 0);\
			}while(0)

/*
 * Load the 32-bit constant zero into a register.  This will zero-extend
 * if the native word size is larger.
 */
#define	md_load_zero_32(inst,reg)	\
			ppc_mov_reg_imm((inst), (reg), 0)

/*
 * Load the native constant zero into a register.
 */
#define	md_load_zero_native(inst,reg)	\
			ppc_mov_reg_imm((inst), (reg), 0)

/*
 * Load a 32-bit word register from an offset from a pointer register.
 * This will sign-extend if the native word size is larger.
 */
#define	md_load_membase_word_32(inst,reg,basereg,offset)	\
			ppc_load_membase((inst), (reg), (basereg), (offset))

/*
 * Load a native-sized word register from an offset from a pointer register.
 */
#define	md_load_membase_word_native(inst,reg,basereg,offset)	\
			ppc_load_membase((inst), (reg), (basereg), (offset))

/*
 * Load a 64-bit word value from an offset from a pointer register
 * as a pair of 32-bit registers.  Only used on 32-bit platforms.
 */
#define	md_load_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				ppc_load_membase((inst), (lreg), (basereg), (offset) + 4); \
				ppc_load_membase((inst), (hreg), (basereg), (offset)); \
			} while (0)

/*
 * Load a byte value from an offset from a pointer register.
 */
#define	md_load_membase_byte(inst,reg,basereg,offset)	\
			ppc_load_membase_byte((inst), (reg), (basereg), (offset))

/*
 * Load a signed byte value from an offset from a pointer register.
 */
#define	md_load_membase_sbyte(inst,reg,basereg,offset)	\
			ppc_load_membase_sbyte((inst), (reg), (basereg), (offset))

/*
 * Load a short value from an offset from a pointer register.
 */
#define	md_load_membase_short(inst,reg,basereg,offset)	\
			ppc_load_membase_short((inst), (reg), (basereg), (offset))

/*
 * Load an unsigned short value from an offset from a pointer register.
 */
#define	md_load_membase_ushort(inst,reg,basereg,offset)	\
			ppc_load_membase_ushort((inst), (reg), (basereg), (offset))

/*
 * Load a floating-point value from an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * loaded into "freg".  Otherwise it is loaded onto the top of the
 * floating-point stack.
 */
#define	md_load_membase_float_32(inst,freg,basereg,offset)	\
			ppc_load_membase_float_32(inst, (freg) & (~MD_FREG_MASK), basereg, offset)
#define	md_load_membase_float_64(inst,freg,basereg,offset)	\
			ppc_load_membase_float_64(inst, (freg) & (~MD_FREG_MASK), basereg, offset)
#define	md_load_membase_float_native(inst,freg,basereg,offset)	\
			ppc_load_membase_float_64(inst, (freg) & (~MD_FREG_MASK), basereg, offset)

/*
 * Store a 32-bit word register to an offset from a pointer register.
 */
#define	md_store_membase_word_32(inst,reg,basereg,offset)	\
			ppc_store_membase((inst), (reg), (basereg), (offset))

/*
 * Store a native-sized word register to an offset from a pointer register.
 */
#define	md_store_membase_word_native(inst,reg,basereg,offset)	\
			ppc_store_membase((inst), (reg), (basereg), (offset))

/*
 * Store a pair of 32-bit word registers to an offset from a pointer
 * register as a 64-bit value.  Only used on 32-bit platforms.
 */
#define	md_store_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				ppc_store_membase((inst), (lreg), (basereg), (offset) + 4); \
				ppc_store_membase((inst), (hreg), (basereg), (offset)); \
			} while (0)

/*
 * Store a byte value to an offset from a pointer register.
 */
#define	md_store_membase_byte(inst,reg,basereg,offset)	\
			ppc_store_membase_byte((inst), (reg), (basereg), (offset))

/*
 * Store a signed byte value to an offset from a pointer register.
 */
#define	md_store_membase_sbyte(inst,reg,basereg,offset)	\
			ppc_store_membase_sbyte((inst), (reg), (basereg), (offset))

/*
 * Store a short value to an offset from a pointer register.
 */
#define	md_store_membase_short(inst,reg,basereg,offset)	\
			ppc_store_membase_short((inst), (reg), (basereg), (offset))

/*
 * Store an unsigned short value to an offset from a pointer register.
 */
#define	md_store_membase_ushort(inst,reg,basereg,offset)	\
			ppc_store_membase_ushort((inst), (reg), (basereg), (offset))

/*
 * Store a floating-point value to an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * stored from "reg".  Otherwise it is stored from the top of the
 * floating-point stack.
 */
#define	md_store_membase_float_32(inst,freg,basereg,offset)	\
			ppc_store_membase_float_32(inst, (freg) & (~MD_FREG_MASK), basereg, offset)
#define	md_store_membase_float_64(inst,freg,basereg,offset)	\
			ppc_store_membase_float_64(inst, (freg) & (~MD_FREG_MASK), basereg, offset)
#define	md_store_membase_float_native(inst,freg,basereg,offset)	\
			ppc_store_membase_float_64(inst, (freg) & (~MD_FREG_MASK), basereg, offset)

/*
 * Add an immediate value to a register.
 */
#define	md_add_reg_imm(inst,reg,imm)	\
			ppc_add_reg_imm((inst), (reg), (reg), (imm))

/*
 * Subtract an immediate value from a register.
 */
#define	md_sub_reg_imm(inst,reg,imm)	\
			ppc_sub_reg_imm((inst), (reg), (reg), (imm))

/*
 * Perform arithmetic and logical operations on 32-bit word registers.
 */
#define	md_add_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_dss((inst), PPC_ADD, (reg1), (reg1), (reg2))
#define	md_sub_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_dss((inst), PPC_SUBF, (reg1), (reg2), (reg1))
#define	md_mul_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_dss((inst), PPC_MUL, (reg1), (reg1), (reg2))
#define	md_div_reg_reg_word_32(inst,reg1,reg2)	\
				ppc_alu_reg_dss((inst), PPC_DIV, (reg1), (reg1), (reg2))
#define	md_udiv_reg_reg_word_32(inst,reg1,reg2)	\
				ppc_alu_reg_dss((inst), PPC_DIV_UN , (reg1), (reg1), (reg2))
#define	md_rem_reg_reg_word_32(inst,reg1,reg2)	\
			do {\
				ppc_alu_reg_dss((inst), PPC_DIV, PPC_WORK, (reg1), (reg2));\
				ppc_alu_reg_dss((inst), PPC_MUL, PPC_WORK, PPC_WORK, (reg2));\
				ppc_alu_reg_dss((inst), PPC_SUBF, (reg1), PPC_WORK, (reg1));\
			} while (0)
#define	md_urem_reg_reg_word_32(inst,reg1,reg2)	\
			do {\
				ppc_alu_reg_dss((inst), PPC_DIV_UN, PPC_WORK, (reg1), (reg2));\
				ppc_alu_reg_dss((inst), PPC_MUL, PPC_WORK, PPC_WORK, (reg2));\
				ppc_alu_reg_dss((inst), PPC_SUBF, (reg1), PPC_WORK, (reg1));\
			} while (0)
#define	md_neg_reg_word_32(inst,reg)	\
			ppc_alu_reg_ds((inst), PPC_NEG, (reg), (reg))
#define	md_and_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_sds((inst), PPC_AND, (reg1), (reg1), (reg2))
#define	md_xor_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_sds((inst), PPC_XOR, (reg1), (reg1), (reg2))
#define	md_or_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_sds((inst), PPC_OR, (reg1), (reg1), (reg2))
#define	md_not_reg_word_32(inst,reg)	\
			ppc_alu_reg_sds((inst), PPC_NOR, (reg), (reg), (reg))
#define	md_shl_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_sds((inst), PPC_SL, (reg1), (reg1), (reg2))
#define	md_shr_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_sds((inst), PPC_SRA, (reg1), (reg1), (reg2))
#define	md_ushr_reg_reg_word_32(inst,reg1,reg2)	\
			ppc_alu_reg_sds((inst), PPC_SR, (reg1), (reg1), (reg2))

/*
 * Perform arithmetic on 64-bit values represented as 32-bit word pairs.
 */
#define	md_add_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				ppc_alu_cc_reg_dss((inst), PPC_ADDC, \
								   (lreg1), (lreg1), (lreg2)); \
				ppc_alu_reg_dss((inst), PPC_ADDE, (hreg1), (hreg1), (hreg2)); \
			} while (0)
#define	md_sub_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				ppc_alu_cc_reg_dss((inst), PPC_SUBFC, \
								   (lreg1), (lreg2), (lreg1)); \
				ppc_alu_reg_dss((inst), PPC_SUBFE, \
								(hreg1), (hreg2), (hreg1)); \
			} while (0)
#define	md_neg_reg_word_64(inst,lreg,hreg)	\
			do { \
				ppc_alu_reg_sds((inst), PPC_NOR, (lreg), (lreg), (lreg)); \
				ppc_alu_reg_sds((inst), PPC_NOR, (hreg), (hreg), (hreg)); \
				ppc_mov_reg_imm((inst), PPC_WORK, 1); \
				ppc_mov_reg_imm((inst), PPC_R0, 0); \
				ppc_alu_cc_reg_dss((inst), PPC_ADDC, \
								   (lreg), (lreg), PPC_WORK); \
				ppc_alu_reg_dss((inst), PPC_ADDE, (hreg), (hreg), PPC_R0); \
			} while (0)

/*
 * Convert word registers between various types.
 */
#define	md_reg_to_byte(inst,reg)	\
			do { \
				ppc_and_reg_imm((inst), (reg), (reg), 0xFF); \
			} while (0)
#define	md_reg_to_sbyte(inst,reg)	\
			do { \
				ppc_alu_reg_sd((inst), PPC_EXTSB, (reg), (reg)); \
			} while (0)
#define	md_reg_to_short(inst,reg)	\
			do { \
				ppc_alu_reg_sd((inst), PPC_EXTSH, (reg), (reg)); \
			} while (0)
#define	md_reg_to_ushort(inst,reg)	\
			do { \
				ppc_and_reg_imm((inst), (reg), (reg), 0xFFFF); \
			} while (0)
#define	md_reg_to_word_32(inst,reg)	\
			do { ; } while (0)
#define	md_reg_to_word_native(inst,reg)	\
			do { ; } while (0)
#define	md_reg_to_word_native_un(inst,reg)	\
			do { ; } while (0)

/*
 * Truncate floating point values to 32-bit or 64-bit.
 */
#define	md_reg_to_float_32(inst, freg)	\
		do { \
			int __freg32 = (freg) & (~MD_FREG_MASK);\
			ppc_fpu_fsrp(inst, __freg32, __freg32);\
		}while(0)
#define	md_reg_to_float_64(inst,freg)	\
			do { /* it is f64 by default for all operations \
			        All f32 operations are rounded back after \
					operation */; } while (0)

/*
 * Swap the top two items on the floating-point stack.
 */
#define	md_freg_swap(inst)		do { ; } while (0)

/*
 * Floating point arithmetic operations
 */
#define	md_add_reg_reg_float(inst,reg1,reg2)	\
		do { \
			int __freg1 = (reg1) & (~MD_FREG_MASK);\
			int __freg2 = (reg2) & (~MD_FREG_MASK);\
			ppc_fpu_reg_dab(inst, 1 /* f64 */ , PPC_FADD, \
								__freg1, __freg1, __freg2);\
		} while(0)
#define	md_sub_reg_reg_float(inst,reg1,reg2)	\
		do { \
			int __freg1 = (reg1) & (~MD_FREG_MASK);\
			int __freg2 = (reg2) & (~MD_FREG_MASK);\
			ppc_fpu_reg_dab(inst, 1 /* f64 */ , PPC_FSUB, \
								__freg1, __freg1, __freg2);\
		} while(0)
#define	md_mul_reg_reg_float(inst,reg1,reg2)	\
		do { \
			int __freg1 = (reg1) & (~MD_FREG_MASK);\
			int __freg2 = (reg2) & (~MD_FREG_MASK);\
			ppc_fpu_reg_dac(inst, 1 /* f64 */ , PPC_FMUL, \
								__freg1, __freg1, __freg2);\
		} while(0)
#define	md_div_reg_reg_float(inst,reg1,reg2)	\
		do { \
			int __freg1 = (reg1) & (~MD_FREG_MASK);\
			int __freg2 = (reg2) & (~MD_FREG_MASK);\
			ppc_fpu_reg_dab(inst, 1 /* f64 */ , PPC_FDIV, \
								__freg1, __freg1, __freg2);\
		} while(0)
#define	md_rem_reg_reg_float(inst,reg1,reg2,used)	\
		TODO_trap(inst)
#define	md_neg_reg_float(inst,reg)	\
		do { \
			int __freg1 = (reg) & (~MD_FREG_MASK);\
			ppc_fpu_reg_dab(inst, 1, PPC_FNEG, __freg1, 0, __freg1);\
		}while(0)
#define	md_cmp_reg_reg_float(inst,dreg,sreg1,sreg2,lessop)	\
		TODO_trap(inst)

/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".  If "label" is non-NULL, then it indicates the address
 * of the CVM instruction handler to jump directly to.
 */
#define	md_jump_to_cvm(inst,pc,label)	\
			do { \
				int offset; \
				ppc_mov_reg_imm(inst, MD_REG_PC, (int)pc); \
				if(!(label)) \
				{ \
					/* Jump to the contents of the specified PC */  \
					ppc_load_membase(inst, PPC_WORK, MD_REG_PC, 0); \
				} \
				else \
				{ \
					ppc_mov_reg_imm(inst, PPC_WORK, (label));\
				} \
				ppc_jump_reg(inst, PPC_WORK); \
			} while (0)

/*
 * Jump to a program counter that is defined by a switch table.
 */
#define	md_switch(inst,reg,table)	\
			do { \
					/* pc = table[reg] */\
					ppc_mov_reg_imm(inst, PPC_WORK, (int)(table));\
					/* cannot use ppc_load_memindex as that will clobber 
					PPC_WORK instead of using reg as the shift dest - so
					essentially same code but different clobbering
					*/\
					ppc_alu_rlwinm(inst, reg, reg, 2, 0, 29);\
					ppc_memindex_common(inst, MD_REG_PC, PPC_WORK, reg, 0, 23);\
					/* goto *pc; */\
					ppc_load_membase(inst, PPC_WORK, MD_REG_PC, 0);\
					ppc_jump_reg(inst, PPC_WORK);\
			} while (0)

/*
 * Perform a clear operation at a memory base.
 */
#define	md_clear_membase_start(inst)	\
			do { \
				ppc_mov_reg_imm((inst), PPC_WORK, 0); \
			} while (0)
#define	md_clear_membase(inst,reg,offset)	\
			do { \
				ppc_store_membase((inst), PPC_WORK, (reg), (offset)); \
			} while (0)

/*
 * Load the effective address of a memory base into a register.
 */
#define	md_lea_membase(inst,reg,basereg,offset)	\
			do { \
				int ___value = (int)(offset); \
				ppc_mov_reg_reg((inst), (reg), (basereg)); \
				if(___value != 0) \
				{ \
					ppc_add_reg_imm((inst), (reg), (reg), ___value); \
				} \
			} while (0)

/*
 * Move values between registers.
 */
#define	md_mov_reg_reg(inst,dreg,sreg)	\
			ppc_mov_reg_reg((inst), (dreg), (sreg))

/*
 * Set a register to a 0 or 1 value based on a condition.
 */
extern md_inst_ptr _md_ppc_setcc(md_inst_ptr inst, int reg,
									int cond);
#define	md_seteq_reg(inst,reg)	\
			do { (inst) = _md_ppc_setcc \
					((inst), (reg), PPC_CC_EQ); } while (0)
#define	md_setne_reg(inst,reg)	\
			do { (inst) = _md_ppc_setcc \
					((inst), (reg), PPC_CC_NE); } while (0)
#define	md_setlt_reg(inst,reg)	\
			do { (inst) = _md_ppc_setcc \
					((inst), (reg), PPC_CC_LT); } while (0)
#define	md_setle_reg(inst,reg)	\
			do { (inst) = _md_ppc_setcc \
					((inst), (reg), PPC_CC_LE); } while (0)
#define	md_setgt_reg(inst,reg)	\
			do { (inst) = _md_ppc_setcc \
					((inst), (reg), PPC_CC_GT); } while (0)
#define	md_setge_reg(inst,reg)	\
			do { (inst) = _md_ppc_setcc \
					((inst), (reg), PPC_CC_GE); } while (0)

/*
 * Set a register to -1, 0, or 1 based on comparing two values.
 */
extern md_inst_ptr _md_ppc_setcmp(md_inst_ptr inst, int dreg);
#define	md_cmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { \
				ppc_cmp_reg_reg((inst), PPC_CMP, (reg1), (reg2)); \
				(inst) = _md_ppc_setcmp((inst), (reg1)); \
			} while (0)
#define	md_ucmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { \
				ppc_cmp_reg_reg((inst), PPC_CMPL, (reg1), (reg2)); \
				(inst) = _md_ppc_setcmp((inst), (reg1)); \
			} while (0)

/*
 * Set the condition codes based on comparing two values.
 * The "cond" value indicates the type of condition that we
 * want to check for.
 */
#define	md_cmp_cc_reg_reg_word_32(inst,cond,reg1,reg2)	\
			ppc_cmp_reg_reg((inst), ((cond) & 16) ? PPC_CMPL : PPC_CMP, \
							(reg1), (reg2))
#define	md_cmp_cc_reg_reg_word_native(inst,cond,reg1,reg2)	\
			ppc_cmp_reg_reg((inst), ((cond) & 16) ? PPC_CMPL : PPC_CMP, \
							(reg1), (reg2))

/*
 * Test the contents of a register against NULL and set the
 * condition codes based on the result.
 */
#define	md_reg_is_null(inst,reg)	\
			ppc_cmp_reg_imm((inst), PPC_CMP, (reg), 0)

/*
 * Test the contents of a register against 32-bit zero and set the
 * condition codes based on the result.
 */
#define	md_reg_is_zero(inst,reg)	\
			ppc_cmp_reg_imm((inst), PPC_CMP, (reg), 0)

/*
 * Compare a 32-bit register against an immediate value and set
 * the condition codes based on the result.
 */
#define	md_cmp_reg_imm_word_32(inst,cond,reg,imm)	\
			ppc_cmp_reg_imm((inst), ((cond) & 16) ? PPC_CMPL : PPC_CMP, \
							(reg), (int)(imm))

/*
 * Output a branch to a location based on a condition.  The actual
 * jump offset will be filled in by a later "md_patch" call.
 */
#define	md_branch_eq(inst)	\
			ppc_branch((inst), PPC_CC_EQ, 0)
#define	md_branch_ne(inst)	\
			ppc_branch((inst), PPC_CC_NE, 0)
#define	md_branch_lt(inst)	\
			ppc_branch((inst), PPC_CC_LT, 0)
#define	md_branch_le(inst)	\
			ppc_branch((inst), PPC_CC_LE, 0)
#define	md_branch_gt(inst)	\
			ppc_branch((inst), PPC_CC_GT, 0)
#define	md_branch_ge(inst)	\
			ppc_branch((inst), PPC_CC_GE, 0)
#define	md_branch_lt_un(inst)	\
			ppc_branch((inst), PPC_CC_LT, 0)
#define	md_branch_le_un(inst)	\
			ppc_branch((inst), PPC_CC_LE, 0)
#define	md_branch_gt_un(inst)	\
			ppc_branch((inst), PPC_CC_GT, 0)
#define	md_branch_ge_un(inst)	\
			ppc_branch((inst), PPC_CC_GE, 0)
#define	md_branch_cc(inst,cond)	\
			ppc_branch((inst), (cond) & ~16, 0)

/*
 * Specific condition codes for "md_branch_cc".
 *
 * Note: the unsigned forms are obtained by performing a different
 * kind of comparison just before the branch.
 */
#define	MD_CC_EQ				PPC_CC_EQ
#define	MD_CC_NE				PPC_CC_NE
#define	MD_CC_LT				PPC_CC_LT
#define	MD_CC_LE				PPC_CC_LE
#define	MD_CC_GT				PPC_CC_GT
#define	MD_CC_GE				PPC_CC_GE
#define	MD_CC_LT_UN				(PPC_CC_LT | 16)
#define	MD_CC_LE_UN				(PPC_CC_LE | 16)
#define	MD_CC_GT_UN				(PPC_CC_GT | 16)
#define	MD_CC_GE_UN				(PPC_CC_GE | 16)

/*
 * Back-patch a branch instruction at "patch" to branch to "inst".
 */
#define	md_patch(patch,inst)	\
			ppc_patch((patch), (inst))

/*
 * Check an array bounds value.  "reg1" points to the array,
 * and "reg2" is the array index to check.  This will advance
 * the pointer in "reg1" past the array bounds value.
 */
#define	md_bounds_check(inst,reg1,reg2)	\
			do { \
				/* faking load_advance is easier for memindex code  \
				   ppc_work = *(reg)++; (so to speak) \
				*/ \
				ppc_load_membase((inst), PPC_WORK, (reg1), 0);\
				ppc_add_reg_imm((inst), (reg1), (reg1), sizeof(System_Array));\
				ppc_cmp_reg_reg((inst), PPC_CMPL, (reg2), PPC_WORK); \
				/* branch compare needs a NOP, refresh cache instead */\
				ppc_cache_prefetch((inst), reg1, reg2);  \
			} while (0)

/*
 * Load a 32-bit word value from an indexed array.  "disp" is the offset
 * to use to skip over the array bounds value.  Some platforms may ignore
 * "disp" if they advance the base pointer in "md_bounds_check".
 */
#define	md_load_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			ppc_load_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Load a native word value from an indexed array.
 */
#define	md_load_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			ppc_load_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Load a byte value from an indexed array.
 */
#define	md_load_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			ppc_load_memindex_byte((inst), (reg), (basereg), (indexreg))

/*
 * Load a signed byte value from an indexed array.
 */
#define	md_load_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			ppc_load_memindex_sbyte((inst), (reg), (basereg), (indexreg))

/*
 * Load a short value from an indexed array.
 */
#define	md_load_memindex_short(inst,reg,basereg,indexreg,disp)	\
			ppc_load_memindex_short((inst), (reg), (basereg), (indexreg))

/*
 * Load an unsigned short value from an indexed array.
 */
#define	md_load_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			ppc_load_memindex_ushort((inst), (reg), (basereg), (indexreg))

/*
 * Store a 32-bit word value into an indexed array.
 */
#define	md_store_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			ppc_store_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Store a native word value into an indexed array.
 */
#define	md_store_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			ppc_store_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Store a byte value into an indexed array.
 */
#define	md_store_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			ppc_store_memindex_byte((inst), (reg), (basereg), (indexreg))

/*
 * Store a signed byte value into an indexed array.
 */
#define	md_store_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			ppc_store_memindex_sbyte((inst), (reg), (basereg), (indexreg))

/*
 * Store a short value into an indexed array.
 */
#define	md_store_memindex_short(inst,reg,basereg,indexreg,disp)	\
			ppc_store_memindex_short((inst), (reg), (basereg), (indexreg))

/*
 * Store an unsigned short value into an indexed array.
 */
#define	md_store_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			ppc_store_memindex_ushort((inst), (reg), (basereg), (indexreg))

#ifdef	__cplusplus
};
#endif

#endif /* _ENGINE_MD_PPC_H */
