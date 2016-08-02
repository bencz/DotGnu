/*
 * md_arm.h - Machine-dependent definitions for ARM.
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

#ifndef	_ENGINE_MD_ARM_H
#define	_ENGINE_MD_ARM_H

#include "arm_codegen.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Register numbers in the standard register allocation order.
 * -1 is the list terminator.
 */
#define	MD_REG_0		ARM_R0
#define	MD_REG_1		ARM_R1
#define	MD_REG_2		ARM_R2
#define	MD_REG_3		ARM_R3
#define	MD_REG_4		ARM_R4
#define	MD_REG_5		ARM_R7
#define	MD_REG_6		ARM_R8
#define	MD_REG_7		ARM_R9
#define	MD_REG_8		-1
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
#ifdef ARM_HAS_FLOAT

#define	MD_FREG_0		(ARM_D0 | MD_FREG_MASK)
#define	MD_FREG_1		(ARM_D1 | MD_FREG_MASK)
#define	MD_FREG_2		(ARM_D2 | MD_FREG_MASK)
#define	MD_FREG_3		(ARM_D3 | MD_FREG_MASK)
#define	MD_FREG_4		(ARM_D4 | MD_FREG_MASK)
#define	MD_FREG_5		(ARM_D5 | MD_FREG_MASK)
#define	MD_FREG_6		(ARM_D6 | MD_FREG_MASK)
#define	MD_FREG_7		(ARM_D7 | MD_FREG_MASK)
#define	MD_FREG_8		-1
#define	MD_FREG_9		-1
#define	MD_FREG_10		-1
#define	MD_FREG_11		-1
#define	MD_FREG_12		-1
#define	MD_FREG_13		-1
#define	MD_FREG_14		-1
#define	MD_FREG_15		-1

#else /* !ARM_HAS_FLOAT */

#define	MD_FREG_0		-1
#define	MD_FREG_1		-1
#define	MD_FREG_2		-1
#define	MD_FREG_3		-1
#define	MD_FREG_4		-1
#define	MD_FREG_5		-1
#define	MD_FREG_6		-1
#define	MD_FREG_7		-1
#define	MD_FREG_8		-1
#define	MD_FREG_9		-1
#define	MD_FREG_10		-1
#define	MD_FREG_11		-1
#define	MD_FREG_12		-1
#define	MD_FREG_13		-1
#define	MD_FREG_14		-1
#define	MD_FREG_15		-1

#endif /* !ARM_HAS_FLOAT */

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
#define	MD_REG_PC		ARM_R4

/*
 * The register that contains the CVM stacktop pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_STACK	ARM_R5

/*
 * The register that contains the CVM frame pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_FRAME	ARM_R6

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
#define	MD_REGS_TO_BE_SAVED	\
			((1 << ARM_R7) | (1 << ARM_R8) | (1 << ARM_R9))

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
#define	MD_HAS_INT_DIVISION			0

/*
 * Set to 1 if 64-bit register pairs are stored in little-endian order.
 */
#define	MD_LITTLE_ENDIAN_LONGS		1

/*
 * Type of the instruction pointer for outputting code.
 */
typedef arm_inst_ptr	md_inst_ptr;

/*
 * Push a word register onto the system stack.
 */
#define	md_push_reg(inst,reg)	arm_push_reg((inst), (reg))

/*
 * Pop a word register from the system stack.
 */
#define	md_pop_reg(inst,reg)	arm_pop_reg((inst), (reg))

/*
 * Discard the contents of a floating-point register.
 */
#define	md_discard_freg(inst,reg)	do { ; } while (0)

/*
 * Load a 32-bit integer constant into a register.  This will sign-extend
 * if the native word size is larger.
 */
#define	md_load_const_32(inst,reg,value)	\
			arm_mov_reg_imm((inst), (reg), (value))

/*
 * Load a native integer constant into a register.
 */
#define	md_load_const_native(inst,reg,value)	\
			arm_mov_reg_imm((inst), (reg), (value))

/*
 * Load a 32-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#ifdef ARM_HAS_FLOAT

#define md_load_const_float_32(inst,reg,mem) \
			do { \
				int __lcs_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				int __lcs_sreg = (__lcs_dreg << 1); \
				arm_load_mem_single((inst), ARM_CC_AL, __lcs_sreg, (mem)); \
				arm_cvt_single_double_reg_reg((inst), ARM_CC_AL, __lcs_dreg, __lcs_sreg); \
			} while (0)

#else /* !ARM_HAS_FLOAT */

#define	md_load_const_float_32(inst,reg,mem)	do { ; } while (0)

#endif /* !ARM_HAS_FLOAT */

/*
 * Load a 64-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#ifdef ARM_HAS_FLOAT

#define md_load_const_float_64(inst,reg,mem) \
			do { \
				int __lcd_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_load_mem_double((inst), ARM_CC_AL, __lcd_dreg, (mem)); \
			} while (0)

#else /* !ARM_HAS_FLOAT */

#define	md_load_const_float_64(inst,reg,mem)	do { ; } while (0)

#endif /* !ARM_HAS_FLOAT */

/*
 * Load the 32-bit constant zero into a register.  This will zero-extend
 * if the native word size is larger.
 */
#define	md_load_zero_32(inst,reg)	\
			arm_mov_reg_imm((inst), (reg), 0)

/*
 * Load the native constant zero into a register.
 */
#define	md_load_zero_native(inst,reg)	\
			arm_mov_reg_imm((inst), (reg), 0)

/*
 * Load a 32-bit word register from an offset from a pointer register.
 * This will sign-extend if the native word size is larger.
 */
#define	md_load_membase_word_32(inst,reg,basereg,offset)	\
			arm_load_membase((inst), (reg), (basereg), (offset))

/*
 * Load a native-sized word register from an offset from a pointer register.
 */
#define	md_load_membase_word_native(inst,reg,basereg,offset)	\
			arm_load_membase((inst), (reg), (basereg), (offset))

/*
 * Load a 64-bit word value from an offset from a pointer register
 * as a pair of 32-bit registers.  Only used on 32-bit platforms.
 */
#define	md_load_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				arm_load_membase((inst), (lreg), (basereg), (offset)); \
				arm_load_membase((inst), (hreg), (basereg), (offset) + 4); \
			} while (0)

/*
 * Load a byte value from an offset from a pointer register.
 */
#define	md_load_membase_byte(inst,reg,basereg,offset)	\
			arm_load_membase_byte((inst), (reg), (basereg), (offset))

/*
 * Load a signed byte value from an offset from a pointer register.
 */
#define	md_load_membase_sbyte(inst,reg,basereg,offset)	\
			arm_load_membase_sbyte((inst), (reg), (basereg), (offset))

/*
 * Load a short value from an offset from a pointer register.
 */
#define	md_load_membase_short(inst,reg,basereg,offset)	\
			arm_load_membase_short((inst), (reg), (basereg), (offset))

/*
 * Load an unsigned short value from an offset from a pointer register.
 */
#define	md_load_membase_ushort(inst,reg,basereg,offset)	\
			arm_load_membase_ushort((inst), (reg), (basereg), (offset))

/*
 * Load a floating-point value from an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * loaded into "reg".  Otherwise it is loaded onto the top of the
 * floating-point stack.
 */
#ifdef ARM_HAS_FLOAT

#define	md_load_membase_float_32(inst,reg,basereg,offset)	\
			do { \
				int __lmbs_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				int __lmbs_sreg = (__lmbs_dreg << 1); \
				arm_load_membase_single((inst), ARM_CC_AL, __lmbs_sreg, (basereg), (offset)); \
				arm_cvt_single_double_reg_reg((inst), ARM_CC_AL, __lmbs_dreg, __lmbs_sreg); \
			} while (0)

#define md_load_membase_float_64(inst,reg,basereg,offset)	\
			do { \
				int __lmbd_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_load_membase_double((inst), ARM_CC_AL, __lmbd_dreg, (basereg), (offset)); \
			} while (0)

#define	md_load_membase_float_native(inst,reg,basereg,offset) \
			md_load_membase_float_64(inst,reg,basereg,offset)

#else /* !ARM_HAS_FLOAT */

#define	md_load_membase_float_32(inst,reg,basereg,offset)	\
			do { ; } while (0)
#define	md_load_membase_float_64(inst,reg,basereg,offset)	\
			do { ; } while (0)
#define	md_load_membase_float_native(inst,reg,basereg,offset)	\
			do { ; } while (0)

#endif /* !ARM_HAS_FLOAT */

/*
 * Store a 32-bit word register to an offset from a pointer register.
 */
#define	md_store_membase_word_32(inst,reg,basereg,offset)	\
			arm_store_membase((inst), (reg), (basereg), (offset))

/*
 * Store a native-sized word register to an offset from a pointer register.
 */
#define	md_store_membase_word_native(inst,reg,basereg,offset)	\
			arm_store_membase((inst), (reg), (basereg), (offset))

/*
 * Store a pair of 32-bit word registers to an offset from a pointer
 * register as a 64-bit value.  Only used on 32-bit platforms.
 */
#define	md_store_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				arm_store_membase((inst), (lreg), (basereg), (offset)); \
				arm_store_membase((inst), (hreg), (basereg), (offset) + 4); \
			} while (0)

/*
 * Store a byte value to an offset from a pointer register.
 */
#define	md_store_membase_byte(inst,reg,basereg,offset)	\
			arm_store_membase_byte((inst), (reg), (basereg), (offset))

/*
 * Store a signed byte value to an offset from a pointer register.
 */
#define	md_store_membase_sbyte(inst,reg,basereg,offset)	\
			arm_store_membase_sbyte((inst), (reg), (basereg), (offset))

/*
 * Store a short value to an offset from a pointer register.
 */
#define	md_store_membase_short(inst,reg,basereg,offset)	\
			arm_store_membase_short((inst), (reg), (basereg), (offset))

/*
 * Store an unsigned short value to an offset from a pointer register.
 */
#define	md_store_membase_ushort(inst,reg,basereg,offset)	\
			arm_store_membase_ushort((inst), (reg), (basereg), (offset))

/*
 * Store a floating-point value to an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * stored from "reg".  Otherwise it is stored from the top of the
 * floating-point stack.
 */
#ifdef ARM_HAS_FLOAT

#define	md_store_membase_float_32(inst,reg,basereg,offset)	\
			do { \
				int __smbs_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				int __smbs_sreg = (__smbs_dreg << 1); \
				arm_cvt_double_single_reg_reg((inst), ARM_CC_AL, __smbs_sreg, __smbs_dreg); \
				arm_store_membase_single((inst), ARM_CC_AL, __smbs_sreg, (basereg), (offset)); \
			} while (0)

#define md_store_membase_float_64(inst,reg,basereg,offset)	\
			do { \
				int __smbd_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_store_membase_double((inst), ARM_CC_AL, __smbd_dreg, (basereg), (offset)); \
			} while (0)

#define	md_store_membase_float_native(inst,reg,basereg,offset) \
			md_store_membase_float_64(inst,reg,basereg,offset)

#else /* !ARM_HAS_FLOAT */

#define	md_store_membase_float_32(inst,reg,basereg,offset)	\
			do { ; } while (0)
#define	md_store_membase_float_64(inst,reg,basereg,offset)	\
			do { ; } while (0)
#define	md_store_membase_float_native(inst,reg,basereg,offset)	\
			do { ; } while (0)

#endif /* !ARM_HAS_FLOAT */

/*
 * Add an immediate value to a register.
 */
#define	md_add_reg_imm(inst,reg,imm)	\
			arm_alu_reg_imm((inst), ARM_ADD, (reg), (reg), (imm))

/*
 * Subtract an immediate value from a register.
 */
#define	md_sub_reg_imm(inst,reg,imm)	\
			arm_alu_reg_imm((inst), ARM_SUB, (reg), (reg), (imm))

/*
 * Perform arithmetic and logical operations on 32-bit word registers.
 */
#define	md_add_reg_reg_word_32(inst,reg1,reg2)	\
			arm_alu_reg_reg((inst), ARM_ADD, (reg1), (reg1), (reg2))
#define	md_sub_reg_reg_word_32(inst,reg1,reg2)	\
			arm_alu_reg_reg((inst), ARM_SUB, (reg1), (reg1), (reg2))
#define	md_mul_reg_reg_word_32(inst,reg1,reg2)	\
			arm_mul_reg_reg((inst), (reg1), (reg1), (reg2))
#define	md_div_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_udiv_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_rem_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_urem_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_neg_reg_word_32(inst,reg)	\
			arm_alu_reg_imm((inst), ARM_RSB, (reg), (reg), 0)
#define	md_and_reg_reg_word_32(inst,reg1,reg2)	\
			arm_alu_reg_reg((inst), ARM_AND, (reg1), (reg1), (reg2))
#define	md_xor_reg_reg_word_32(inst,reg1,reg2)	\
			arm_alu_reg_reg((inst), ARM_EOR, (reg1), (reg1), (reg2))
#define	md_or_reg_reg_word_32(inst,reg1,reg2)	\
			arm_alu_reg_reg((inst), ARM_ORR, (reg1), (reg1), (reg2))
#define	md_not_reg_word_32(inst,reg)	\
			arm_alu_reg((inst), ARM_MVN, (reg), (reg))
#define	md_shl_reg_reg_word_32(inst,reg1,reg2)	\
			arm_shift_reg_reg((inst), ARM_SHL, (reg1), (reg1), (reg2))
#define	md_shr_reg_reg_word_32(inst,reg1,reg2)	\
			arm_shift_reg_reg((inst), ARM_SAR, (reg1), (reg1), (reg2))
#define	md_ushr_reg_reg_word_32(inst,reg1,reg2)	\
			arm_shift_reg_reg((inst), ARM_SHR, (reg1), (reg1), (reg2))

/*
 * Perform arithmetic on 64-bit values represented as 32-bit word pairs.
 */
#define	md_add_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				arm_alu_cc_reg_reg((inst), ARM_ADD, \
								   (lreg1), (lreg1), (lreg2)); \
				arm_alu_reg_reg((inst), ARM_ADC, (hreg1), (hreg1), (hreg2)); \
			} while (0)
#define	md_sub_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				arm_alu_cc_reg_reg((inst), ARM_SUB, \
								   (lreg1), (lreg1), (lreg2)); \
				arm_alu_reg_reg((inst), ARM_SBC, (hreg1), (hreg1), (hreg2)); \
			} while (0)
#define	md_neg_reg_word_64(inst,lreg,hreg)	\
			do { \
				arm_alu_reg((inst), ARM_MVN, (lreg), (lreg)); \
				arm_alu_reg((inst), ARM_MVN, (hreg), (hreg)); \
				arm_alu_cc_reg_imm8((inst), ARM_ADD, (lreg), (lreg), 1); \
				arm_alu_reg_imm8((inst), ARM_ADC, (hreg), (hreg), 0); \
			} while (0)

/*
 * Convert word registers between various types.
 */
#define	md_reg_to_byte(inst,reg)	\
			arm_alu_reg_imm((inst), ARM_AND, (reg), (reg), 0xFF)
#define	md_reg_to_sbyte(inst,reg)	\
			do { \
				arm_shift_reg_imm8((inst), ARM_SHL, (reg), (reg), 24); \
				arm_shift_reg_imm8((inst), ARM_SAR, (reg), (reg), 24); \
			} while (0)
#define	md_reg_to_short(inst,reg)	\
			do { \
				arm_shift_reg_imm8((inst), ARM_SHL, (reg), (reg), 16); \
				arm_shift_reg_imm8((inst), ARM_SAR, (reg), (reg), 16); \
			} while (0)
#define	md_reg_to_ushort(inst,reg)	\
			do { \
				arm_shift_reg_imm8((inst), ARM_SHL, (reg), (reg), 16); \
				arm_shift_reg_imm8((inst), ARM_SHR, (reg), (reg), 16); \
			} while (0)
#define	md_reg_to_word_32(inst,reg)	\
			do { ; } while (0)
#define	md_reg_to_word_native(inst,reg)	\
			do { ; } while (0)
#define	md_reg_to_word_native_un(inst,reg)	\
			do { ; } while (0)

#ifdef ARM_HAS_FLOAT

/*
 * Convert a signed 32 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_sword_32_float(inst,dreg,sreg)	\
			do { \
				int __ci32s_dreg = ((int)(dreg) & ~MD_FREG_MASK); \
				int __ci32s_sreg = (__ci32s_dreg << 1); \
				arm_load_reg_single((inst), ARM_CC_AL, __ci32s_sreg, (sreg)); \
				arm_cvt_si_double((inst), ARM_CC_AL, __ci32s_dreg, __ci32s_sreg); \
			} while (0)

/*
 * Convert an unsigned 32 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_uword_32_float(inst,dreg,sreg)	\
			do { \
				int __cu32s_dreg = ((int)(dreg) & ~MD_FREG_MASK); \
				int __cu32s_sreg = (__cu32s_dreg << 1); \
				arm_load_reg_single((inst), ARM_CC_AL, __cu32s_sreg, (sreg)); \
				arm_cvt_ui_double((inst), ARM_CC_AL, __cu32s_dreg, __cu32s_sreg); \
			} while (0)

/*
 * Convert a native floating-point value to a signed 32 bit value using the
 * truncate (round to zero) rounding mode and store it in a general purpose
 * register.
 */
#define	md_conv_float_sword_32(inst,dreg,sreg)	\
			do { \
				int __cfi32_dreg = ((int)(sreg) & ~MD_FREG_MASK); \
				int __cfi32_sreg = (__cfi32_dreg << 1); \
				arm_cvt_double_si((inst), ARM_CC_AL, __cfi32_sreg, __cfi32_dreg, 1); \
				arm_store_reg_single((inst), ARM_CC_AL, (dreg), __cfi32_sreg); \
			} while (0)

/*
 * Convert a native floating-point value to an unsigned 32 bit value using
 * the truncate (round to zero) rounding mode and store it in a general
 * purpose register.
 */
#define	md_conv_float_uword_32(inst,dreg,sreg)	\
			do { \
				int __cfu32_dreg = ((int)(sreg) & ~MD_FREG_MASK); \
				int __cfu32_sreg = (__cfu32_dreg << 1); \
				arm_cvt_double_ui((inst), ARM_CC_AL, __cfu32_sreg, __cfu32_dreg, 1); \
				arm_store_reg_single((inst), ARM_CC_AL, (dreg), __cfu32_sreg); \
			} while (0)

/*
 * Truncate floating point values to 32-bit or 64-bit.
 */
#define	md_reg_to_float_32(inst,reg)	\
			do { \
				int __cfs_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				int __cfs_sreg = (__cfs_dreg << 1); \
				arm_cvt_double_single_reg_reg((inst), ARM_CC_AL, __cfs_sreg, __cfs_dreg); \
				arm_cvt_single_double_reg_reg((inst), ARM_CC_AL, __cfs_dreg, __cfs_sreg); \
			} while (0)

#define	md_reg_to_float_64(inst,reg)	\
			do { ; } while (0)

#endif /* ARM_HAS_FLOAT */

/*
 * Swap the top two items on the floating-point stack.
 */
#define	md_freg_swap(inst)		do { ; } while (0)

/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".  If "label" is non-NULL, then it indicates the address
 * of the CVM instruction handler to jump directly to.
 */
#define	md_jump_to_cvm(inst,pc,label)	\
			do { \
				int offset; \
				if(!(label)) \
				{ \
					/* Jump to the contents of the specified PC */ \
					arm_load_membase((inst), MD_REG_PC, ARM_PC, 0); \
					arm_load_membase((inst), ARM_PC, MD_REG_PC, 0); \
					*((inst)++) = (unsigned int)(pc); \
				} \
				else \
				{ \
					/* Load "pc" back into the CVM interpreter's frame */ \
					/* and then jump directly to instruction handler at */ \
					/* "label".  This avoids the need for an indirect */ \
					/* jump instruction */ \
					arm_load_membase(unroll->out, MD_REG_PC, ARM_PC, 0); \
					offset = (int)(((unsigned char *)(label)) - \
				       			  (((unsigned char *)(inst)) + 8)); \
					if(offset >= -0x04000000 && offset < 0x04000000) \
					{ \
						arm_jump_imm((inst), offset); \
						*((inst)++) = (unsigned int)(pc); \
					} \
					else \
					{ \
						arm_load_membase(unroll->out, ARM_PC, ARM_PC, 0); \
						*((inst)++) = (unsigned int)(pc); \
						*((inst)++) = (unsigned int)(label); \
					} \
				} \
			} while (0)

/*
 * Jump to a program counter that is defined by a switch table.
 */
#define	md_switch(inst,reg,table)	\
			do { \
				arm_load_membase((inst), ARM_WORK, ARM_PC, 4); \
				arm_load_memindex((inst), MD_REG_PC, ARM_WORK, (reg)); \
				arm_load_membase((inst), ARM_PC, MD_REG_PC, 0); \
				*((inst)++) = (unsigned int)(table); \
			} while (0)

/*
 * Perform a clear operation at a memory base.
 */
#define	md_clear_membase_start(inst)	\
			do { \
				arm_mov_reg_imm((inst), ARM_WORK, 0); \
			} while (0)
#define	md_clear_membase(inst,reg,offset)	\
			do { \
				arm_store_membase((inst), ARM_WORK, (reg), (offset)); \
			} while (0)

/*
 * Load the effective address of a memory base into a register.
 */
#define	md_lea_membase(inst,reg,basereg,offset)	\
			do { \
				int ___value = (int)(offset); \
				if(___value == 0) \
				{ \
					arm_mov_reg_reg((inst), (reg), (basereg)); \
				} \
				else \
				{ \
					arm_alu_reg_imm((inst), ARM_ADD, (reg), (basereg), ___value); \
				} \
			} while (0)

/*
 * Load the effective address of a memory base + shifted index into
 * a register.
 */
#define	md_lea_memindex_shift(inst,reg,basereg,indexreg,shift,offset)	\
				arm_alu_reg_reg_lslimm((inst), ARM_ADD, (reg), (basereg), (indexreg), (shift))

/*
 * Load the effective address of a memory base + multiplied index into
 * a register.
 */
#define	md_lea_memindex_mul(inst,reg,basereg,indexreg,imm,offset)	\
			do { \
				arm_mov_reg_imm((inst), ARM_WORK, imm); \
				arm_muladd_reg_reg_reg((inst), ARM_CC_AL, (reg), (basereg), (indexreg), ARM_WORK); \
			} while (0)

/*
 * Move values between registers.
 */
#define	md_mov_reg_reg(inst,dreg,sreg)	\
			arm_mov_reg_reg((inst), (dreg), (sreg))

/*
 * Set a register to a 0 or 1 value based on a condition.
 */
extern md_inst_ptr _md_arm_setcc(md_inst_ptr inst, int reg,
								 int cond, int invcond);
#define	md_seteq_reg(inst,reg)	\
			do { (inst) = _md_arm_setcc \
					((inst), (reg), ARM_CC_EQ, ARM_CC_NE); } while (0)
#define	md_setne_reg(inst,reg)	\
			do { (inst) = _md_arm_setcc \
					((inst), (reg), ARM_CC_NE, ARM_CC_EQ); } while (0)
#define	md_setlt_reg(inst,reg)	\
			do { (inst) = _md_arm_setcc \
					((inst), (reg), ARM_CC_LT, ARM_CC_GE); } while (0)
#define	md_setle_reg(inst,reg)	\
			do { (inst) = _md_arm_setcc \
					((inst), (reg), ARM_CC_LE, ARM_CC_GT); } while (0)
#define	md_setgt_reg(inst,reg)	\
			do { (inst) = _md_arm_setcc \
					((inst), (reg), ARM_CC_GT, ARM_CC_LE); } while (0)
#define	md_setge_reg(inst,reg)	\
			do { (inst) = _md_arm_setcc \
					((inst), (reg), ARM_CC_GE, ARM_CC_LT); } while (0)

/*
 * Set a register to -1, 0, or 1 based on comparing two values.
 */
#define	md_cmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { \
				arm_test_reg_reg((inst), ARM_CMP, (reg1), (reg2)); \
				arm_alu_reg_imm8_cond((inst), ARM_MOV, (reg1), 0, 1, \
									  ARM_CC_GT); \
				arm_alu_reg_imm8_cond((inst), ARM_MOV, (reg1), 0, 0, \
									  ARM_CC_LE); \
				arm_alu_reg_cond((inst), ARM_MVN, (reg1), (reg1), ARM_CC_LT); \
			} while (0)
#define	md_ucmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { \
				arm_test_reg_reg((inst), ARM_CMP, (reg1), (reg2)); \
				arm_alu_reg_imm8_cond((inst), ARM_MOV, (reg1), 0, 1, \
									  ARM_CC_GT_UN); \
				arm_alu_reg_imm8_cond((inst), ARM_MOV, (reg1), 0, 0, \
									  ARM_CC_LE_UN); \
				arm_alu_reg_cond((inst), ARM_MVN, (reg1), (reg1), \
								 ARM_CC_LT_UN); \
			} while (0)

#ifdef ARM_HAS_FLOAT

md_inst_ptr _md_arm_cmp_float(md_inst_ptr inst, int dreg, int sreg1,
							  int sreg2, int lessop);
#define md_cmp_reg_reg_float(inst,dreg,sreg1,sreg2,lessop) \
			do { \
				inst = _md_arm_cmp_float((inst), (dreg), (sreg1), (sreg2), \
										 (lessop)); \
			} while (0)
#endif /* ARM_HAS_FLOAT */

/*
 * Set the condition codes based on comparing two values.
 * The "cond" value indicates the type of condition that we
 * want to check for.
 */
#define	md_cmp_cc_reg_reg_word_32(inst,cond,reg1,reg2)	\
			arm_test_reg_reg((inst), ARM_CMP, (reg1), (reg2))
#define	md_cmp_cc_reg_reg_word_native(inst,cond,reg1,reg2)	\
			arm_test_reg_reg((inst), ARM_CMP, (reg1), (reg2))

/*
 * Test the contents of a register against NULL and set the
 * condition codes based on the result.
 */
#define	md_reg_is_null(inst,reg)	\
			arm_test_reg_imm8((inst), ARM_CMP, (reg), 0)

/*
 * Test the contents of a register against 32-bit zero and set the
 * condition codes based on the result.
 */
#define	md_reg_is_zero(inst,reg)	\
			arm_test_reg_imm8((inst), ARM_CMP, (reg), 0)

/*
 * Compare a 32-bit register against an immediate value and set
 * the condition codes based on the result.
 */
#define	md_cmp_reg_imm_word_32(inst,cond,reg,imm)	\
			arm_test_reg_imm((inst), ARM_CMP, (reg), (int)(imm))

/*
 * Output a branch to a location based on a condition.  The actual
 * jump offset will be filled in by a later "md_patch" call.
 */
#define	md_branch_eq(inst)	\
			arm_branch_imm((inst), ARM_CC_EQ, 0)
#define	md_branch_ne(inst)	\
			arm_branch_imm((inst), ARM_CC_NE, 0)
#define	md_branch_lt(inst)	\
			arm_branch_imm((inst), ARM_CC_LT, 0)
#define	md_branch_le(inst)	\
			arm_branch_imm((inst), ARM_CC_LE, 0)
#define	md_branch_gt(inst)	\
			arm_branch_imm((inst), ARM_CC_GT, 0)
#define	md_branch_ge(inst)	\
			arm_branch_imm((inst), ARM_CC_GE, 0)
#define	md_branch_lt_un(inst)	\
			arm_branch_imm((inst), ARM_CC_LT_UN, 0)
#define	md_branch_le_un(inst)	\
			arm_branch_imm((inst), ARM_CC_LE_UN, 0)
#define	md_branch_gt_un(inst)	\
			arm_branch_imm((inst), ARM_CC_GT_UN, 0)
#define	md_branch_ge_un(inst)	\
			arm_branch_imm((inst), ARM_CC_GE_UN, 0)
#define	md_branch_cc(inst,cond)	\
			arm_branch_imm((inst), (cond), 0)

/*
 * Specific condition codes for "md_branch_cc".
 */
#define	MD_CC_EQ				ARM_CC_EQ
#define	MD_CC_NE				ARM_CC_NE
#define	MD_CC_LT				ARM_CC_LT
#define	MD_CC_LE				ARM_CC_LE
#define	MD_CC_GT				ARM_CC_GT
#define	MD_CC_GE				ARM_CC_GE
#define	MD_CC_LT_UN				ARM_CC_LT_UN
#define	MD_CC_LE_UN				ARM_CC_LE_UN
#define	MD_CC_GT_UN				ARM_CC_GT_UN
#define	MD_CC_GE_UN				ARM_CC_GE_UN

/*
 * Back-patch a branch instruction at "patch" to branch to "inst".
 */
#define	md_patch(patch,inst)	\
			arm_patch((patch), (inst))

/*
 * Advance reg1 to the first array element. This means adding 4 on EABI
 * because sizeof(System_Array) is 8 here.
 */
#ifdef __ARM_EABI__
	#define arm_bounds_check_advance(inst, reg1) \
		do { \
			md_add_reg_imm((inst), reg1, 4); \
		} while (0)
#else
	#define arm_bounds_check_advance(inst, reg1)
#endif

/*
 * Check an array bounds value.  "reg1" points to the array,
 * and "reg2" is the array index to check.  This will advance
 * the pointer in "reg1" past the array bounds value.
 */
#define	md_bounds_check(inst,reg1,reg2)	\
			do { \
				arm_load_advance((inst), ARM_WORK, (reg1)); \
				arm_bounds_check_advance((inst), (reg1)); \
				arm_test_reg_reg((inst), ARM_CMP, (reg2), ARM_WORK); \
			} while (0)

/*
 * Load a 32-bit word value from an indexed array.  "disp" is the offset
 * to use to skip over the array bounds value.  Some platforms may ignore
 * "disp" if they advance the base pointer in "md_bounds_check".
 */
#define	md_load_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			arm_load_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Load a native word value from an indexed array.
 */
#define	md_load_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			arm_load_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Load a byte value from an indexed array.
 */
#define	md_load_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			arm_load_memindex_byte((inst), (reg), (basereg), (indexreg))

/*
 * Load a signed byte value from an indexed array.
 */
#define	md_load_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			arm_load_memindex_sbyte((inst), (reg), (basereg), (indexreg))

/*
 * Load a short value from an indexed array.
 */
#define	md_load_memindex_short(inst,reg,basereg,indexreg,disp)	\
			arm_load_memindex_short((inst), (reg), (basereg), (indexreg))

/*
 * Load an unsigned short value from an indexed array.
 */
#define	md_load_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			arm_load_memindex_ushort((inst), (reg), (basereg), (indexreg))

#ifdef ARM_HAS_FLOAT

/*
 * Load a 32 bit floatingpoint value from an indexed array.
 */
#define	md_load_memindex_float_32(inst,reg,basereg,indexreg,disp) \
			do { \
				int __lmis_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				int __lmis_sreg = (__lmis_dreg << 1); \
				arm_load_memindex_single((inst), ARM_CC_AL, __lmis_sreg, (basereg), (indexreg)); \
				arm_cvt_single_double_reg_reg((inst), ARM_CC_AL, __lmis_dreg, __lmis_sreg); \
			} while (0)

/*
 * Load a 64 bit floatingpoint value from an indexed array.
 */
#define	md_load_memindex_float_64(inst,reg,basereg,indexreg,disp) \
			do { \
				int __lmid_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_load_memindex_double((inst), ARM_CC_AL, __lmid_dreg, (basereg), (indexreg)); \
			} while (0)

#endif /* ARM_HAS_FLOAT */

/*
 * Store a 32-bit word value into an indexed array.
 */
#define	md_store_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			arm_store_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Store a native word value into an indexed array.
 */
#define	md_store_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			arm_store_memindex((inst), (reg), (basereg), (indexreg))

/*
 * Store a byte value into an indexed array.
 */
#define	md_store_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			arm_store_memindex_byte((inst), (reg), (basereg), (indexreg))

/*
 * Store a signed byte value into an indexed array.
 */
#define	md_store_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			arm_store_memindex_sbyte((inst), (reg), (basereg), (indexreg))

/*
 * Store a short value into an indexed array.
 */
#define	md_store_memindex_short(inst,reg,basereg,indexreg,disp)	\
			arm_store_memindex_short((inst), (reg), (basereg), (indexreg))

/*
 * Store an unsigned short value into an indexed array.
 */
#define	md_store_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			arm_store_memindex_ushort((inst), (reg), (basereg), (indexreg))

#ifdef ARM_HAS_FLOAT

/*
 * Store a 32 bit floatingpoint value into an indexed array.
 */
#define	md_store_memindex_float_32(inst,reg,basereg,indexreg,disp) \
			do { \
				int __smis_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				int __smis_sreg = (__smis_dreg << 1); \
				arm_cvt_double_single_reg_reg((inst), ARM_CC_AL, __smis_sreg, __smis_dreg); \
				arm_store_memindex_single((inst), ARM_CC_AL, __smis_sreg, (basereg), (indexreg)); \
			} while (0)

/*
 * Store a 64 bit floatingpoint value into an indexed array.
 */
#define	md_store_memindex_float_64(inst,reg,basereg,indexreg,disp) \
			do { \
				int __smid_dreg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_store_memindex_double((inst), ARM_CC_AL, __smid_dreg, (basereg), (indexreg)); \
			} while (0)

#endif /* ARM_HAS_FLOAT */

#ifdef ARM_HAS_FLOAT

#define md_add_reg_reg_float(inst,reg,reg2) \
			do { \
				int __add_reg1 = ((int)(reg) & ~MD_FREG_MASK); \
				int __add_reg2 = ((int)(reg2) & ~MD_FREG_MASK); \
				arm_add_double_reg_reg(inst, ARM_CC_AL, __add_reg1, \
														__add_reg1, \
														__add_reg2); \
			} while (0)

#define md_sub_reg_reg_float(inst,reg,reg2) \
			do { \
				int __sub_reg1 = ((int)(reg) & ~MD_FREG_MASK); \
				int __sub_reg2 = ((int)(reg2) & ~MD_FREG_MASK); \
				arm_sub_double_reg_reg(inst, ARM_CC_AL, __sub_reg1, \
														__sub_reg1, \
														__sub_reg2); \
			} while (0)

#define md_mul_reg_reg_float(inst,reg,reg2) \
			do { \
				int __mul_reg1 = ((int)(reg) & ~MD_FREG_MASK); \
				int __mul_reg2 = ((int)(reg2) & ~MD_FREG_MASK); \
				arm_mul_double_reg_reg(inst, ARM_CC_AL, __mul_reg1, \
														__mul_reg1, \
														__mul_reg2); \
			} while (0)

#define md_div_reg_reg_float(inst,reg,reg2) \
			do { \
				int __div_reg1 = ((int)(reg) & ~MD_FREG_MASK); \
				int __div_reg2 = ((int)(reg2) & ~MD_FREG_MASK); \
				arm_div_double_reg_reg(inst, ARM_CC_AL, __div_reg1, \
														__div_reg1, \
														__div_reg2); \
			} while (0)

#define md_neg_reg_float(inst,reg) \
			do { \
				int __neg_reg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_neg_double_reg(inst, ARM_CC_AL, __neg_reg, __neg_reg); \
			} while (0)

#define md_abs_reg_float(inst,reg) \
			do { \
				int __abs_reg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_abs_double_reg(inst, ARM_CC_AL, __abs_reg, __abs_reg); \
			} while (0)

#define md_sqrt_reg_float(inst,reg) \
			do { \
				int __sqrt_reg = ((int)(reg) & ~MD_FREG_MASK); \
				arm_sqrt_double_reg(inst, ARM_CC_AL, __sqrt_reg, __sqrt_reg); \
			} while (0)

#endif /* ARM_HAS_FLOAT */

#ifdef	__cplusplus
};
#endif

#endif /* _ENGINE_MD_ARM_H */
