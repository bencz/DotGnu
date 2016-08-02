/*
 * md_x86.h - Machine-dependent definitions for x86.
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

#ifndef	_ENGINE_MD_X86_H
#define	_ENGINE_MD_X86_H

#include "x86_codegen.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Register numbers in the standard register allocation order.
 * -1 is the list terminator.
 */
#define	MD_REG_0		X86_EAX
#define	MD_REG_1		X86_ECX
#define	MD_REG_2		X86_EDX
#define	MD_REG_3		X86_ESI
#define	MD_REG_4		X86_EBP
#define	MD_REG_5		-1
#define	MD_REG_6		-1
#define	MD_REG_7		-1
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
#define	MD_FREG_0		(MD_FREG_MASK | 0)
#define	MD_FREG_1		(MD_FREG_MASK | 1)
#define	MD_FREG_2		(MD_FREG_MASK | 2)
#define	MD_FREG_3		(MD_FREG_MASK | 3)
#define	MD_FREG_4		(MD_FREG_MASK | 4)
#define	MD_FREG_5		(MD_FREG_MASK | 5)
#define	MD_FREG_6		(MD_FREG_MASK | 6)
#define	MD_FREG_7		(MD_FREG_MASK | 7)
#define	MD_FREG_8		-1
#define	MD_FREG_9		-1
#define	MD_FREG_10		-1
#define	MD_FREG_11		-1
#define	MD_FREG_12		-1
#define	MD_FREG_13		-1
#define	MD_FREG_14		-1
#define	MD_FREG_15		-1

/*
 * Set this to a non-zero value if floating-point registers are organised
 * in a stack (e.g. the x87 FPU).
 */
#define	MD_FP_STACK_SIZE	8

/*
 * The register that contains the CVM program counter.  This may be
 * present in the standard register allocation order.  This can be
 * set to -1 if MD_STATE_ALREADY_IN_REGS is 0.
 */
#define	MD_REG_PC		X86_ESI

/*
 * The register that contains the CVM stacktop pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_STACK	X86_EDI

/*
 * The register that contains the CVM frame pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_FRAME	X86_EBX

/*
 * The registers to be used to save pc before jumping to cvm.  These
 * can be set to -1 if MD_STATE_ALREADY_IN_REGS is not 0.
 */
#define MD_REG_TEMP_PC			X86_EAX
#define MD_REG_TEMP_PC_PTR		X86_ECX

/*
 * The register that contains native stack pointer.  This can be set to
 * -1 if MD_STATE_ALREADY_IN_REGS is not 0.
 */
#define MD_REG_NATIVE_STACK		X86_ESP

/*
 * Set this to 1 if "pc", "stacktop", and "frame" are already in
 * the above registers when unrolled code is entered.  i.e. the
 * CVM interpreter has manual assignments of registers to variables
 * in the file "cvm.c".  If the state is not already in registers,
 * then set this value to zero.
 */
#ifdef IL_NO_REGISTERS_USED
#define	MD_STATE_ALREADY_IN_REGS	0
#else
#define	MD_STATE_ALREADY_IN_REGS	1
#endif

/*
 * Registers that must be saved on the system stack prior to their use
 * in unrolled code for temporary stack values.
 */
#define	MD_REGS_TO_BE_SAVED			(1 << X86_EBP)

/*
 * Registers with special meanings (pc, stacktop, frame) that must
 * be saved if MD_STATE_ALREADY_IN_REGS is 0.
 */
#ifdef IL_VMCASE_BARRIER
#define MD_SPECIAL_REGS_TO_BE_SAVED	0
#else
#define MD_SPECIAL_REGS_TO_BE_SAVED \
	((1 << MD_REG_PC) | (1 << MD_REG_STACK) | (1 << MD_REG_FRAME))
#endif

/*
 * The number of bytes the special registers occupy on the stack.
 */
#ifdef IL_VMCASE_BARRIER
#define MD_SPECIAL_REGS_STACK_OFFSET	0
#else
#define MD_SPECIAL_REGS_STACK_OFFSET	(3 * 4)
#endif

/*
 * Floating-point register numbers that must be saved.
 */
#define	MD_FREGS_TO_BE_SAVED		0

/*
 * Set this to 1 if the CPU has integer division operations.
 * Set it to zero if integer division is too hard to be performed
 * inline using a simple opcode.
 */
#define	MD_HAS_INT_DIVISION			1

/*
 * Set to 1 if 64-bit register pairs are stored in little-endian order.
 */
#define	MD_LITTLE_ENDIAN_LONGS		1

/*
 * Type of the instruction pointer for outputting code.
 */
typedef unsigned char *md_inst_ptr;

/*
 * Push a word register onto the system stack.
 */
#define	md_push_reg(inst,reg)	x86_push_reg((inst), (reg))

/*
 * Pop a word register from the system stack.
 */
#define	md_pop_reg(inst,reg)	x86_pop_reg((inst), (reg))

/*
 * Discard the contents of a floating-point register.
 */
#define	md_discard_freg(inst,reg)	do { ; } while (0)

/*
 * Load a 32-bit integer constant into a register.  This will sign-extend
 * if the native word size is larger.
 */
#define	md_load_const_32(inst,reg,value)	\
			x86_mov_reg_imm((inst), (reg), (value))

/*
 * Load a native integer constant into a register.
 */
#define	md_load_const_native(inst,reg,value)	\
			x86_mov_reg_imm((inst), (reg), (value))

/*
 * Load a 32-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_32(inst,reg,mem)	\
			x86_fld((inst), (int)(mem), 0)

/*
 * Load a 64-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_64(inst,reg,mem)	\
			x86_fld((inst), (int)(mem), 1)

/*
 * Load the 32-bit constant zero into a register.  This will zero-extend
 * if the native word size is larger.
 */
#define	md_load_zero_32(inst,reg)	\
			x86_clear_reg((inst), (reg))

/*
 * Load the native constant zero into a register.
 */
#define	md_load_zero_native(inst,reg)	\
			x86_clear_reg((inst), (reg))

/*
 * Load a 32-bit word register from an offset from a pointer register.
 * This will sign-extend if the native word size is larger.
 */
#define	md_load_membase_word_32(inst,reg,basereg,offset)	\
			x86_mov_reg_membase((inst), (reg), (basereg), (offset), 4)

/*
 * Load a native-sized word register from an offset from a pointer register.
 */
#define	md_load_membase_word_native(inst,reg,basereg,offset)	\
			x86_mov_reg_membase((inst), (reg), (basereg), (offset), 4)

/*
 * Load a 64-bit word register from an offset from a pointer register
 * into a pair of 32-bit registers.  Only used on 32-bit systems.
 */
#define	md_load_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				x86_mov_reg_membase \
					((inst), (lreg), (basereg), (offset), 4); \
				x86_mov_reg_membase \
					((inst), (hreg), (basereg), (offset) + 4, 4); \
			} while (0)

/*
 * Load a byte value from an offset from a pointer register.
 */
#define	md_load_membase_byte(inst,reg,basereg,offset)	\
			x86_widen_membase((inst), (reg), (basereg), (offset), 0, 0)

/*
 * Load a signed byte value from an offset from a pointer register.
 */
#define	md_load_membase_sbyte(inst,reg,basereg,offset)	\
			x86_widen_membase((inst), (reg), (basereg), (offset), 1, 0)

/*
 * Load a short value from an offset from a pointer register.
 */
#define	md_load_membase_short(inst,reg,basereg,offset)	\
			x86_widen_membase((inst), (reg), (basereg), (offset), 1, 1)

/*
 * Load an unsigned short value from an offset from a pointer register.
 */
#define	md_load_membase_ushort(inst,reg,basereg,offset)	\
			x86_widen_membase((inst), (reg), (basereg), (offset), 0, 1)

/*
 * Load a floating-point value from an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * loaded into "reg".  Otherwise it is loaded onto the top of the
 * floating-point stack.
 */
#define	md_load_membase_float_32(inst,reg,basereg,offset)	\
			x86_fld_membase((inst), (basereg), (offset), 0)
#define	md_load_membase_float_64(inst,reg,basereg,offset)	\
			x86_fld_membase((inst), (basereg), (offset), 1)
#define	md_load_membase_float_native(inst,reg,basereg,offset)	\
			x86_fld80_membase((inst), (basereg), (offset))

/*
 * Store a 32-bit word register to an offset from a pointer register.
 */
#define	md_store_membase_word_32(inst,reg,basereg,offset)	\
			x86_mov_membase_reg((inst), (basereg), (offset), (reg), 4)

/*
 * Store a native-sized word register to an offset from a pointer register.
 */
#define	md_store_membase_word_native(inst,reg,basereg,offset)	\
			x86_mov_membase_reg((inst), (basereg), (offset), (reg), 4)

/*
 * Store a pair of 32-bit word registers to an offset from a pointer
 * register as a 64-bit value.  Only used on 32-bit systems.
 */
#define	md_store_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				x86_mov_membase_reg \
					((inst), (basereg), (offset), (lreg), 4); \
				x86_mov_membase_reg \
					((inst), (basereg), (offset) + 4, (hreg), 4); \
			} while (0)

/*
 * Store a byte value to an offset from a pointer register.
 */
md_inst_ptr _md_x86_mov_membase_reg_byte
			(md_inst_ptr inst, int basereg, int offset, int srcreg);
#define	md_store_membase_byte(inst,reg,basereg,offset)	\
			do { \
				(inst) = _md_x86_mov_membase_reg_byte	\
					((inst), (basereg), (int)(offset), (reg)); \
			} while (0)

/*
 * Store a signed byte value to an offset from a pointer register.
 */
#define	md_store_membase_sbyte(inst,reg,basereg,offset)	\
			md_store_membase_byte((inst), (reg), (basereg), (offset))

/*
 * Store a short value to an offset from a pointer register.
 */
#define	md_store_membase_short(inst,reg,basereg,offset)	\
			x86_mov_membase_reg((inst), (basereg), (offset), (reg), 2)

/*
 * Store an unsigned short value to an offset from a pointer register.
 */
#define	md_store_membase_ushort(inst,reg,basereg,offset)	\
			x86_mov_membase_reg((inst), (basereg), (offset), (reg), 2)

/*
 * Store a floating-point value to an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * stored from "reg".  Otherwise it is stored from the top of the
 * floating-point stack.
 */
#define	md_store_membase_float_32(inst,reg,basereg,offset)	\
			x86_fst_membase((inst), (basereg), (offset), 0, 1)
#define	md_store_membase_float_64(inst,reg,basereg,offset)	\
			x86_fst_membase((inst), (basereg), (offset), 1, 1)
#define	md_store_membase_float_native(inst,reg,basereg,offset)	\
			x86_fst80_membase((inst), (basereg), (offset))

/*
 * Add an immediate value to a register.
 */
#define	md_add_reg_imm(inst,reg,imm)	\
			x86_alu_reg_imm((inst), X86_ADD, (reg), (imm))

/*
 * Subtract an immediate value from a register.
 */
#define	md_sub_reg_imm(inst,reg,imm)	\
			x86_alu_reg_imm((inst), X86_SUB, (reg), (imm))

/*
 * Perform arithmetic and logical operations on 32-bit word registers.
 *
 * Division is tricky, so it is handled elsewhere for x86.
 */
#define	md_add_reg_reg_word_32(inst,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_ADD, (reg1), (reg2))
#define	md_sub_reg_reg_word_32(inst,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_SUB, (reg1), (reg2))
#define	md_mul_reg_reg_word_32(inst,reg1,reg2)	\
			x86_imul_reg_reg((inst), (reg1), (reg2))
extern md_inst_ptr _md_x86_divide(md_inst_ptr inst, int reg1, int reg2,
								  int isSigned, int wantRemainder);
#define	md_div_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_udiv_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_rem_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_urem_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_neg_reg_word_32(inst,reg)	\
			x86_neg_reg((inst), (reg))
#define	md_and_reg_reg_word_32(inst,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_AND, (reg1), (reg2))
#define	md_xor_reg_reg_word_32(inst,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_XOR, (reg1), (reg2))
#define	md_or_reg_reg_word_32(inst,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_OR, (reg1), (reg2))
#define	md_not_reg_word_32(inst,reg)	\
			x86_not_reg((inst), (reg))
extern md_inst_ptr _md_x86_shift(md_inst_ptr inst, int opc, int reg1, int reg2);
#define	md_shl_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_x86_shift \
					((inst), X86_SHL, (reg1), (reg2)); } while (0)
#define	md_shr_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_x86_shift \
					((inst), X86_SAR, (reg1), (reg2)); } while (0)
#define	md_ushr_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_x86_shift \
					((inst), X86_SHR, (reg1), (reg2)); } while (0)

/*
 * Perform arithmetic on 64-bit values represented as 32-bit word pairs.
 */
#define	md_add_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				x86_alu_reg_reg((inst), X86_ADD, (lreg1), (lreg2)); \
				x86_alu_reg_reg((inst), X86_ADC, (hreg1), (hreg2)); \
			} while (0)
#define	md_sub_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				x86_alu_reg_reg((inst), X86_SUB, (lreg1), (lreg2)); \
				x86_alu_reg_reg((inst), X86_SBB, (hreg1), (hreg2)); \
			} while (0)
#define	md_neg_reg_word_64(inst,lreg,hreg)	\
			do { \
				x86_not_reg((inst), (lreg)); \
				x86_not_reg((inst), (hreg)); \
				x86_alu_reg_imm((inst), X86_ADD, (lreg), 1); \
				x86_alu_reg_imm((inst), X86_ADC, (hreg), 0); \
			} while (0)

/*
 * Perform arithmetic operations on native float values.  If the system
 * uses a floating-point stack, then the register arguments are ignored.
 *
 * Note: x86 remainder is handled elsewhere because it is complicated.
 */
#define	md_add_reg_reg_float(inst,reg1,reg2)	\
			x86_fp_op_reg((inst), X86_FADD, 1, 1)
#define	md_sub_reg_reg_float(inst,reg1,reg2)	\
			x86_fp_op_reg((inst), X86_FSUB, 1, 1)
#define	md_mul_reg_reg_float(inst,reg1,reg2)	\
			x86_fp_op_reg((inst), X86_FMUL, 1, 1)
#define	md_div_reg_reg_float(inst,reg1,reg2)	\
			x86_fp_op_reg((inst), X86_FDIV, 1, 1)
extern md_inst_ptr _md_x86_rem_float
			(md_inst_ptr inst, int reg1, int reg2, int used);
#define	md_rem_reg_reg_float(inst,reg1,reg2,used)	\
			do { (inst) = _md_x86_rem_float \
					((inst), (reg1), (reg2), (used)); } while (0)
#define	md_neg_reg_float(inst,reg)	\
			x86_fchs((inst))

/*
 * Compare two floating point values and produce a -1, 0, or 1 result.
 */
extern md_inst_ptr _md_x86_cmp_float(md_inst_ptr inst, int dreg, int lessop);
#define	md_cmp_reg_reg_float(inst, dreg, sreg1, sreg2, lessop)	\
			do { \
				(inst) = _md_x86_cmp_float((inst), (dreg), (lessop)); \
			} while (0)

/*
 * Convert word registers between various types.
 */
extern md_inst_ptr _md_x86_widen_byte(md_inst_ptr inst, int reg, int isSigned);
#define	md_reg_to_byte(inst,reg)	\
			do { \
				(inst) = _md_x86_widen_byte((inst), (reg), 0); \
			} while (0)
#define	md_reg_to_sbyte(inst,reg)	\
			do { \
				(inst) = _md_x86_widen_byte((inst), (reg), 1); \
			} while (0)
#define	md_reg_to_short(inst,reg)	\
			x86_widen_reg((inst), (reg), (reg), 1, 1)
#define	md_reg_to_ushort(inst,reg)	\
			x86_widen_reg((inst), (reg), (reg), 0, 1)
#define	md_reg_to_word_32(inst,reg)	\
			do { ; } while (0)
#define	md_reg_to_word_native(inst,reg)	\
			do { ; } while (0)
#define	md_reg_to_word_native_un(inst,reg)	\
			do { ; } while (0)

/*
 * Truncate floating point values to 32-bit or 64-bit.
 */
#define	md_reg_to_float_32(inst,reg)	\
			do { \
				x86_alu_reg_imm((inst), X86_SUB, X86_ESP, 4); \
				x86_fst_membase((inst), X86_ESP, 0, 0, 1); \
				x86_fld_membase((inst), X86_ESP, 0, 0); \
				x86_alu_reg_imm((inst), X86_ADD, X86_ESP, 4); \
			} while (0)
#define	md_reg_to_float_64(inst,reg)	\
			do { \
				x86_alu_reg_imm((inst), X86_SUB, X86_ESP, 8); \
				x86_fst_membase((inst), X86_ESP, 0, 1, 1); \
				x86_fld_membase((inst), X86_ESP, 0, 1); \
				x86_alu_reg_imm((inst), X86_ADD, X86_ESP, 8); \
			} while (0)

/*
 * Convert a signed 32 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_sword_32_float(inst,dreg,sreg)	\
			do { \
				x86_alu_reg_imm((inst), X86_SUB, X86_ESP, 4); \
				x86_mov_membase_reg((inst), X86_ESP, 0, (sreg), 4); \
				x86_fild_membase((inst), X86_ESP, 0, 0); \
				x86_alu_reg_imm((inst), X86_ADD, X86_ESP, 4); \
			} while (0)

/*
 * Convert an unsigned 32 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_uword_32_float(inst,dreg,sreg)	\
			do { \
				x86_alu_reg_imm((inst), X86_SUB, X86_ESP, 8); \
				x86_mov_membase_reg((inst), X86_ESP, 0, (sreg), 4); \
				x86_mov_membase_imm((inst), X86_ESP, 4, 0, 4); \
				x86_fild_membase((inst), X86_ESP, 0, 1); \
				x86_alu_reg_imm((inst), X86_ADD, X86_ESP, 8); \
			} while (0)

/*
 * Convert a signed 64 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_sword_64_float(inst,dreg,sregl,sregh)	\
			do { \
				x86_alu_reg_imm((inst), X86_SUB, X86_ESP, 8); \
				x86_mov_membase_reg((inst), X86_ESP, 0, (sregl), 4); \
				x86_mov_membase_reg((inst), X86_ESP, 4, (sregh), 4); \
				x86_fild_membase((inst), X86_ESP, 0, 1); \
				x86_alu_reg_imm((inst), X86_ADD, X86_ESP, 8); \
			} while (0)

/*
 * Swap the top two items on the floating-point stack.
 */
#define	md_freg_swap(inst)		x86_fxch((inst), 1)

/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".  If "label" is non-NULL, then it indicates the address
 * of the CVM instruction handler to jump directly to.
 *
 * This is used if MD_STATE_ALREADY_IN_REGS is 0.
 */
#define	md_jump_to_cvm(inst,pc,label)	\
			do { \
				x86_mov_reg_imm((inst), MD_REG_PC, (int)pc); \
				if((label)) \
				{ \
					x86_jump_code((inst), label); \
				} \
				else \
				{ \
					x86_jump_membase((inst), MD_REG_PC, 0); \
				} \
			} while (0)

/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".  If "label" is non-NULL, then it indicates the address
 * of the CVM instruction handler to jump directly to.
 *
 * This is used if MD_STATE_ALREADY_IN_REGS is not 0.
 */
#define	md_jump_to_cvm_with_pc_offset(inst,pc,pc_offset,label)		\
			do { \
				int __offset = (pc_offset) - MD_SPECIAL_REGS_STACK_OFFSET; \
				x86_mov_reg_imm((inst), MD_REG_TEMP_PC, (int)pc); \
				x86_mov_membase_reg((inst), MD_REG_NATIVE_STACK, __offset, MD_REG_TEMP_PC, 4); \
				if((label)) \
				{ \
					x86_jump_code((inst), label); \
				} \
				else \
				{ \
					x86_jump_membase((inst), MD_REG_TEMP_PC, 0); \
				} \
			} while (0)

/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".
 *
 * This is used if MD_STATE_ALREADY_IN_REGS is not 0 to bootstrap
 * the unroller.
 */
#define	md_jump_to_cvm_with_pc_ptr(inst,pc,pc_ptr)		\
			do { \
				x86_mov_reg_imm((inst), MD_REG_TEMP_PC, (int)pc); \
				x86_mov_reg_imm((inst), MD_REG_TEMP_PC_PTR, (int)pc_ptr); \
				x86_mov_membase_reg((inst), MD_REG_TEMP_PC_PTR, 0, MD_REG_TEMP_PC, 4); \
				x86_jump_membase((inst), MD_REG_TEMP_PC, 0); \
			} while (0)

/*
 * Jump to a program counter that is defined by a switch table.
 */
#define	md_switch(inst,reg,table)	\
			do { \
				x86_mov_reg_memindex((inst), MD_REG_PC, X86_NOBASEREG, \
									 (int)(table), (reg), 2, 4); \
				x86_jump_membase((inst), MD_REG_PC, 0); \
			} while (0)

/*
 * Perform a clear operation at a memory base.
 */
#define	md_clear_membase_start(inst)	do { ; } while (0)
#define	md_clear_membase(inst,reg,offset)	\
			do { \
				x86_mov_membase_imm((inst), (reg), (offset), 0, 4); \
			} while (0)

/*
 * Load the effective address of a memory base into a register.
 */
#define	md_lea_membase(inst,reg,basereg,offset)	\
			do { \
				int __value = (int)(offset); \
				if(!__value) \
				{ \
					x86_mov_reg_reg((inst), (reg), (basereg), 4); \
				} \
				else \
				{ \
					x86_lea_membase((inst), (reg), (basereg), __value); \
				} \
			} while (0)

/*
 * Load the effective address of a memory base + shifted index into
 * a register.
 * WARNING: indexreg might be destroyed by this operation.
 */
#define	md_lea_memindex_shift(inst,reg,basereg,indexreg,shift,offset)	\
			do { \
				if((shift) <= 3) \
				{ \
					x86_lea_memindex((inst), (reg), (basereg), (offset), (indexreg), (shift)); \
				} \
				else \
				{ \
					x86_shift_reg_imm((inst), X86_SHL, (indexreg), (shift)); \
					x86_lea_memindex((inst), (reg), (basereg), (offset), (indexreg), 0); \
				} \
			} while (0)

/*
 * Move values between registers.
 */
#define	md_mov_reg_reg(inst,dreg,sreg)	\
			x86_mov_reg_reg((inst), (dreg), (sreg), 4)

/*
 * Set a register to a 0 or 1 value based on a condition.
 */
extern md_inst_ptr _md_x86_setcc(md_inst_ptr inst, int reg, int cond);
#define	md_seteq_reg(inst,reg)	\
			do { (inst) = _md_x86_setcc((inst), (reg), X86_CC_EQ); } while (0)
#define	md_setne_reg(inst,reg)	\
			do { (inst) = _md_x86_setcc((inst), (reg), X86_CC_NE); } while (0)
#define	md_setlt_reg(inst,reg)	\
			do { (inst) = _md_x86_setcc((inst), (reg), X86_CC_LT); } while (0)
#define	md_setle_reg(inst,reg)	\
			do { (inst) = _md_x86_setcc((inst), (reg), X86_CC_LE); } while (0)
#define	md_setgt_reg(inst,reg)	\
			do { (inst) = _md_x86_setcc((inst), (reg), X86_CC_GT); } while (0)
#define	md_setge_reg(inst,reg)	\
			do { (inst) = _md_x86_setcc((inst), (reg), X86_CC_GE); } while (0)

/*
 * Set a register to -1, 0, or 1 based on comparing two values.
 */
extern md_inst_ptr _md_x86_compare
				(md_inst_ptr inst, int reg1, int reg2, int isSigned);
#define	md_cmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_x86_compare \
					((inst), (reg1), (reg2), 1); } while (0)
#define	md_ucmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_x86_compare \
					((inst), (reg1), (reg2), 0); } while (0)

/*
 * Set the condition codes based on comparing two values.
 * The "cond" value indicates the type of condition that we
 * want to check for.
 */
#define	md_cmp_cc_reg_reg_word_32(inst,cond,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_CMP, (reg1), (reg2))
#define	md_cmp_cc_reg_reg_word_native(inst,cond,reg1,reg2)	\
			x86_alu_reg_reg((inst), X86_CMP, (reg1), (reg2))

/*
 * Test the contents of a register against NULL and set the
 * condition codes based on the result.
 */
#define	md_reg_is_null(inst,reg)	\
			x86_alu_reg_reg((inst), X86_OR, (reg), (reg))

/*
 * Test the contents of a register against 32-bit zero and set the
 * condition codes based on the result.
 */
#define	md_reg_is_zero(inst,reg)	\
			x86_alu_reg_reg((inst), X86_OR, (reg), (reg))

/*
 * Compare a 32-bit register against an immediate value and set
 * the condition codes based on the result.
 */
#define	md_cmp_reg_imm_word_32(inst,cond,reg,imm)	\
			x86_alu_reg_imm((inst), X86_CMP, (reg), (int)(imm))

/*
 * Output a branch to a location based on a condition.  The actual
 * jump offset will be filled in by a later "md_patch" call.
 */
#define	md_branch_eq(inst)	\
			x86_branch32((inst), X86_CC_EQ, 0, 0)
#define	md_branch_ne(inst)	\
			x86_branch32((inst), X86_CC_NE, 0, 0)
#define	md_branch_lt(inst)	\
			x86_branch32((inst), X86_CC_LT, 0, 1)
#define	md_branch_le(inst)	\
			x86_branch32((inst), X86_CC_LE, 0, 1)
#define	md_branch_gt(inst)	\
			x86_branch32((inst), X86_CC_GT, 0, 1)
#define	md_branch_ge(inst)	\
			x86_branch32((inst), X86_CC_GE, 0, 1)
#define	md_branch_lt_un(inst)	\
			x86_branch32((inst), X86_CC_LT, 0, 0)
#define	md_branch_le_un(inst)	\
			x86_branch32((inst), X86_CC_LE, 0, 0)
#define	md_branch_gt_un(inst)	\
			x86_branch32((inst), X86_CC_GT, 0, 0)
#define	md_branch_ge_un(inst)	\
			x86_branch32((inst), X86_CC_GE, 0, 0)
#define	md_branch_cc(inst,cond)	\
			x86_branch32((inst), (cond) & 15, 0, ((cond) & 16) != 0)

/*
 * Specific condition codes for "md_branch_cc".
 */
#define	MD_CC_EQ				X86_CC_EQ
#define	MD_CC_NE				X86_CC_NE
#define	MD_CC_LT				(X86_CC_LT | 16)
#define	MD_CC_LE				(X86_CC_LE | 16)
#define	MD_CC_GT				(X86_CC_GT | 16)
#define	MD_CC_GE				(X86_CC_GE | 16)
#define	MD_CC_LT_UN				X86_CC_LT
#define	MD_CC_LE_UN				X86_CC_LE
#define	MD_CC_GT_UN				X86_CC_GT
#define	MD_CC_GE_UN				X86_CC_GE

/*
 * Back-patch a branch instruction at "patch" to branch to "inst".
 */
#define	md_patch(patch,inst)	\
			x86_patch((patch), (inst))

/*
 * Check an array bounds value.  "reg1" points to the array,
 * and "reg2" is the array index to check.
 */
#define	md_bounds_check(inst,reg1,reg2)	\
			x86_alu_reg_membase((inst), X86_CMP, (reg2), (reg1), 0)

/*
 * Load a 32-bit word value from an indexed array.  "disp" is the offset
 * to use to skip over the array bounds value.  Some platforms may ignore
 * "disp" if they advance the base pointer in "md_bounds_check".
 */
#define	md_load_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_mov_reg_memindex((inst), (reg), (basereg), \
									 (disp), (indexreg), 2, 4); \
			} while (0)

/*
 * Load a native word value from an indexed array.
 */
#define	md_load_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_mov_reg_memindex((inst), (reg), (basereg), \
									 (disp), (indexreg), 2, 4); \
			} while (0)

/*
 * Load a byte value from an indexed array.
 */
#define	md_load_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 0, 0, 0); \
			} while (0)

/*
 * Load a signed byte value from an indexed array.
 */
#define	md_load_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 0, 1, 0); \
			} while (0)

/*
 * Load a short value from an indexed array.
 */
#define	md_load_memindex_short(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 1, 1, 1); \
			} while (0)

/*
 * Load an unsigned short value from an indexed array.
 */
#define	md_load_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 1, 0, 1); \
			} while (0)

/*
 * Load a 32 bit floatingpoint value from an indexed array.
 */
#define	md_load_memindex_float_32(inst,reg,basereg,indexreg,disp) \
			do { \
				x86_fld_memindex((inst), (basereg), (disp), (indexreg), 0); \
			} while (0)

/*
 * Load a 64 bit floatingpoint value from an indexed array.
 */
#define	md_load_memindex_float_64(inst,reg,basereg,indexreg,disp) \
			do { \
				x86_fld_memindex((inst), (basereg), (disp), (indexreg), 1); \
			} while (0)

/*
 * Store a 32-bit word value into an indexed array.
 */
#define	md_store_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 2, (reg), 4); \
			} while (0)

/*
 * Store a native word value into an indexed array.
 */
#define	md_store_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 2, (reg), 4); \
			} while (0)

/*
 * Store a byte value into an indexed array.
 */
extern md_inst_ptr _md_x86_mov_memindex_reg_byte
			(md_inst_ptr inst, int basereg,
			 unsigned offset, int indexreg, int srcreg);
#define	md_store_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			do { \
				(inst) = _md_x86_mov_memindex_reg_byte \
					((inst), (basereg), (disp), (indexreg), (reg)); \
			} while (0)

/*
 * Store a signed byte value into an indexed array.
 */
#define	md_store_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			do { \
				(inst) = _md_x86_mov_memindex_reg_byte \
					((inst), (basereg), (disp), (indexreg), (reg)); \
			} while (0)

/*
 * Store a short value into an indexed array.
 */
#define	md_store_memindex_short(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 1, (reg), 2); \
			} while (0)

/*
 * Store an unsigned short value into an indexed array.
 */
#define	md_store_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			do { \
				x86_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 1, (reg), 2); \
			} while (0)

/*
 * Store a 32 bit floatingpoint value to an indexed array.
 */
#define	md_store_memindex_float_32(inst,reg,basereg,indexreg,disp) \
			do { \
				x86_fst_memindex((inst), (basereg), (disp), (indexreg), 0, 1); \
			} while (0)

/*
 * Store a 64 bit floatingpoint value to an indexed array.
 */
#define	md_store_memindex_float_64(inst,reg,basereg,indexreg,disp) \
			do { \
				x86_fst_memindex((inst), (basereg), (disp), (indexreg), 1, 1); \
			} while (0)

/*
 * Absolute of a floatinpoint value
 */
#define md_abs_reg_float(inst,reg) \
			do { \
				x86_fabs((inst)); \
			} while (0)

/*
 * Square root of a floatingpoint value
 */
#define md_sqrt_reg_float(inst,reg) \
			do { \
				x86_fsqrt((inst)); \
			} while (0)

/*
 * Sine of a floatingpoint value
 */
#define md_sin_reg_float(inst,reg) \
			do { \
				x86_fsin((inst)); \
			} while (0)

/*
 * Cosine of a floatingpoint value
 */
#define md_cos_reg_float(inst,reg) \
			do { \
				x86_fcos((inst)); \
			} while (0)

/*
 * Tangent of a floatingpoint value
 */
#define md_tan_reg_float(inst,reg) \
			do { \
				x86_fptan((inst)); \
				x86_fstp((inst), 0); \
			} while (0)

#ifdef	__cplusplus
};
#endif

#endif /* _ENGINE_MD_X86_H */
