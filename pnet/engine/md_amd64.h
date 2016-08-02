/*
 * md_amd64.h - Machine-dependent definitions for x86_64 (AMD64/EM64T).
 *
 * Copyright (C) 2003, 2005, 2010  Southern Storm Software, Pty Ltd.
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

#ifndef	_ENGINE_MD_AMD64_H
#define	_ENGINE_MD_AMD64_H

#include "amd64_codegen.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Register numbers in the standard register allocation order.
 * -1 is the list terminator.
 */
#define	MD_REG_0		AMD64_RAX
#define	MD_REG_1		AMD64_RCX
#define	MD_REG_2		AMD64_RBP
#define	MD_REG_3		AMD64_RBX
#define	MD_REG_4		AMD64_RDX
#define	MD_REG_5		AMD64_RSI
#define	MD_REG_6		AMD64_RDI
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
//#define	MD_FREG_0		-1 /* TODO: implement FPU support */
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
#define	MD_REG_PC		AMD64_R12

/*
 * The register that contains the CVM stacktop pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_STACK	AMD64_R14

/*
 * The register that contains the CVM frame pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_FRAME	AMD64_R15

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
#define	MD_REGS_TO_BE_SAVED			(1 << AMD64_RBP)

/*
 * Registers with special meanings (pc, stacktop, frame) that must
 * be saved if MD_STATE_ALREADY_IN_REGS is 0.
 */
#define	MD_SPECIAL_REGS_TO_BE_SAVED	0

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
#define	MD_LITTLE_ENDIAN_LONGS		0

/*
 * Type of the instruction pointer for outputting code.
 */
typedef unsigned char *md_inst_ptr;

/* 
 * Trap TODO code while executing
 */

#define TODO_trap(inst) \
	do { \
		amd64_breakpoint_size((inst), 8); \
		}while(0)

/*
 * Push a word register onto the system stack.
 */
#define	md_push_reg(inst,reg)	amd64_push_reg((inst), (reg))

/*
 * Pop a word register from the system stack.
 */
#define	md_pop_reg(inst,reg)	amd64_pop_reg((inst), (reg))

/*
 * Discard the contents of a floating-point register.
 */
#define	md_discard_freg(inst,reg)	do { ; } while (0)

/*
 * Load a 32-bit integer constant into a register.  This will sign-extend
 * if the native word size is larger.
 */
#define	md_load_const_32(inst,reg,value)	\
			amd64_mov_reg_imm_size((inst), (reg), (value), 4)

/*
 * Load a native integer constant into a register.
 */
#define	md_load_const_native(inst,reg,value)	\
			amd64_mov_reg_imm_size((inst), (reg), (value), 8)

/*
 * Load a 32-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_32(inst,reg,mem)	\
			do { \
				amd64_push_reg((inst), MD_REG_PC); \
				amd64_mov_reg_imm_size((inst), MD_REG_PC, (mem), 8); \
				amd64_fld_membase((inst), (MD_REG_PC), (0), 0); \
				amd64_pop_reg((inst), MD_REG_PC);\
			}while(0)

/*
 * Load a 64-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 * TODO: fix all pushes and pops
 */
#define	md_load_const_float_64(inst,reg,mem)	\
			do { \
				amd64_push_reg((inst), MD_REG_PC); \
				amd64_mov_reg_imm_size((inst), MD_REG_PC, (mem), 8); \
				amd64_fld_membase((inst), (MD_REG_PC), (0), 1); \
				amd64_pop_reg((inst), MD_REG_PC);\
			}while(0)

/*
 * Load the 32-bit constant zero into a register.  This will zero-extend
 * if the native word size is larger.
 */
#define	md_load_zero_32(inst,reg)	\
			amd64_clear_reg_size((inst), (reg), 4)

/*
 * Load the native constant zero into a register.
 */
#define	md_load_zero_native(inst,reg)	\
			amd64_clear_reg((inst), (reg))

/*
 * Load a 32-bit word register from an offset from a pointer register.
 * This will sign-extend if the native word size is larger.
 */
#define	md_load_membase_word_32(inst,reg,basereg,offset)	\
			amd64_mov_reg_membase((inst), (reg), (basereg), (offset), 4)

/*
 * Load a native-sized word register from an offset from a pointer register.
 */
#define	md_load_membase_word_native(inst,reg,basereg,offset)	\
			amd64_mov_reg_membase((inst), (reg), (basereg), (offset), 8)

/*
 * Load a 64-bit word register from an offset from a pointer register
 * into a pair of 32-bit registers.  Only used on 32-bit systems.
 */
#define	md_load_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				TODO_trap(inst);\
			} while (0)

/*
 * Load a byte value from an offset from a pointer register.
 */
#define	md_load_membase_byte(inst,reg,basereg,offset)	\
			amd64_widen_membase((inst), (reg), (basereg), (offset), 0, 0)

/*
 * Load a signed byte value from an offset from a pointer register.
 */
#define	md_load_membase_sbyte(inst,reg,basereg,offset)	\
			amd64_widen_membase((inst), (reg), (basereg), (offset), 1, 0)

/*
 * Load a short value from an offset from a pointer register.
 */
#define	md_load_membase_short(inst,reg,basereg,offset)	\
			amd64_widen_membase((inst), (reg), (basereg), (offset), 1, 1)

/*
 * Load an unsigned short value from an offset from a pointer register.
 */
#define	md_load_membase_ushort(inst,reg,basereg,offset)	\
			amd64_widen_membase((inst), (reg), (basereg), (offset), 0, 1)

/*
 * Load a floating-point value from an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * loaded into "reg".  Otherwise it is loaded onto the top of the
 * floating-point stack.
 */
#define	md_load_membase_float_32(inst,reg,basereg,offset)	\
			amd64_fld_membase((inst), (basereg), (offset), 0)
#define	md_load_membase_float_64(inst,reg,basereg,offset)	\
			amd64_fld_membase((inst), (basereg), (offset), 1)
#define	md_load_membase_float_native(inst,reg,basereg,offset)	\
			amd64_fld80_membase((inst), (basereg), (offset))

/*
 * Store a 32-bit word register to an offset from a pointer register.
 */
#define	md_store_membase_word_32(inst,reg,basereg,offset)	\
			amd64_mov_membase_reg((inst), (basereg), (offset), (reg), 4)

/*
 * Store a native-sized word register to an offset from a pointer register.
 */
#define	md_store_membase_word_native(inst,reg,basereg,offset)	\
			amd64_mov_membase_reg((inst), (basereg), (offset), (reg), 8)

/*
 * Store a pair of 32-bit word registers to an offset from a pointer
 * register as a 64-bit value.  Only used on 32-bit systems.
 */
#define	md_store_membase_word_64(inst,lreg,hreg,basereg,offset)	\
			do { \
				TODO_trap(inst);\
			} while (0)

/*
 * Store a byte value to an offset from a pointer register.
 */
md_inst_ptr _md_amd64_mov_membase_reg_byte
			(md_inst_ptr inst, int basereg, int offset, int srcreg);
#define	md_store_membase_byte(inst,reg,basereg,offset)	\
			do { \
				(inst) = _md_amd64_mov_membase_reg_byte	\
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
			amd64_mov_membase_reg((inst), (basereg), (offset), (reg), 2)

/*
 * Store an unsigned short value to an offset from a pointer register.
 */
#define	md_store_membase_ushort(inst,reg,basereg,offset)	\
			amd64_mov_membase_reg((inst), (basereg), (offset), (reg), 2)

/*
 * Store a floating-point value to an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * stored from "reg".  Otherwise it is stored from the top of the
 * floating-point stack.
 */
#define	md_store_membase_float_32(inst,reg,basereg,offset)	\
			amd64_fst_membase((inst), (basereg), (offset), 0, 1)
#define	md_store_membase_float_64(inst,reg,basereg,offset)	\
			amd64_fst_membase((inst), (basereg), (offset), 1, 1)
#define	md_store_membase_float_native(inst,reg,basereg,offset)	\
			amd64_fst80_membase((inst), (basereg), (offset))

/*
 * Add an immediate value to a register.
 */
#define	md_add_reg_imm(inst,reg,imm)	\
			amd64_alu_reg_imm_size((inst), X86_ADD, (reg), (imm), 8)

/*
 * Subtract an immediate value from a register.
 */
#define	md_sub_reg_imm(inst,reg,imm)	\
			amd64_alu_reg_imm_size((inst), X86_SUB, (reg), (imm), 8)

/*
 * Perform arithmetic and logical operations on 32-bit word registers.
 *
 * Division is tricky, so it is handled elsewhere for AMD64.
 */
#define	md_add_reg_reg_word_32(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_ADD, (reg1), (reg2), 4)
#define	md_sub_reg_reg_word_32(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_SUB, (reg1), (reg2), 4)
#define	md_mul_reg_reg_word_32(inst,reg1,reg2)	\
			amd64_imul_reg_reg_size((inst), (reg1), (reg2), 4)
#define	md_div_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_udiv_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_rem_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_urem_reg_reg_word_32(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_neg_reg_word_32(inst,reg)	\
			amd64_neg_reg_size((inst), (reg), 4)
#define	md_and_reg_reg_word_32(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_AND, (reg1), (reg2), 4)
#define	md_xor_reg_reg_word_32(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_XOR, (reg1), (reg2), 4)
#define	md_or_reg_reg_word_32(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_OR, (reg1), (reg2), 4)
#define	md_not_reg_word_32(inst,reg)	\
			amd64_not_reg_size((inst), (reg), 4)
extern md_inst_ptr _md_amd64_shift(md_inst_ptr inst, int opc, int reg1, int reg2);
#define	md_shl_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_amd64_shift \
					((inst), X86_SHL, (reg1), (reg2)); } while (0)
#define	md_shr_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_amd64_shift \
					((inst), X86_SAR, (reg1), (reg2)); } while (0)
#define	md_ushr_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_amd64_shift \
					((inst), X86_SHR, (reg1), (reg2)); } while (0)
/* 
 * Perform arithmetic on native word values 
 */

#define	md_add_reg_reg_word_native(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_ADD, (reg1), (reg2), 8)
#define	md_sub_reg_reg_word_native(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_SUB, (reg1), (reg2), 8)
#define	md_neg_reg_word_native(inst,reg)	\
			amd64_neg_reg((inst), (reg))
#define	md_and_reg_reg_word_native(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_AND, (reg1), (reg2), 8)
#define	md_xor_reg_reg_word_native(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_XOR, (reg1), (reg2), 8)
#define	md_or_reg_reg_word_native(inst,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_OR, (reg1), (reg2), 8)
#define	md_not_reg_word_native(inst,reg)	\
			amd64_not_reg((inst), (reg))
			
/*
 * Perform arithmetic on 64-bit values represented as 32-bit word pairs.
 */
#define	md_add_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				TODO_trap(inst); \
			} while (0)
#define	md_sub_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
			do { \
				TODO_trap(inst); \
			} while (0)
#define	md_neg_reg_word_64(inst,lreg,hreg)	\
			do { \
				TODO_trap(inst); \
			} while (0)

/*
 * Perform arithmetic operations on native float values.  If the system
 * uses a floating-point stack, then the register arguments are ignored.
 *
 * Note: remainder is handled elsewhere because it is complicated.
 */
#define	md_add_reg_reg_float(inst,reg1,reg2)	\
			amd64_fp_op_reg((inst), X86_FADD, 1, 1)
#define	md_sub_reg_reg_float(inst,reg1,reg2)	\
			amd64_fp_op_reg((inst), X86_FSUB, 1, 1)
#define	md_mul_reg_reg_float(inst,reg1,reg2)	\
			amd64_fp_op_reg((inst), X86_FMUL, 1, 1)
#define	md_div_reg_reg_float(inst,reg1,reg2)	\
			amd64_fp_op_reg((inst), X86_FDIV, 1, 1)
extern md_inst_ptr _md_amd64_rem_float
			(md_inst_ptr inst, int reg1, int reg2, int used);
#define	md_rem_reg_reg_float(inst,reg1,reg2,used)	\
			do { (inst) = _md_amd64_rem_float \
					((inst), (reg1), (reg2), (used)); } while (0)
#define	md_neg_reg_float(inst,reg)	\
			amd64_fchs((inst))

/*
 * Compare two floating point values and produce a -1, 0, or 1 result.
 */
extern md_inst_ptr _md_amd64_cmp_float(md_inst_ptr inst, int dreg, int lessop);
#define	md_cmp_reg_reg_float(inst, dreg, sreg1, sreg2, lessop)	\
			do { \
				(inst) = _md_amd64_cmp_float((inst), (dreg), (lessop)); \
			} while (0)

/*
 * Convert word registers between various types.
 */
extern md_inst_ptr _md_amd64_widen_byte(md_inst_ptr inst, int reg, int isSigned);
#define	md_reg_to_byte(inst,reg)	\
			do { \
				(inst) = _md_amd64_widen_byte((inst), (reg), 0); \
			} while (0)
#define	md_reg_to_sbyte(inst,reg)	\
			do { \
				(inst) = _md_amd64_widen_byte((inst), (reg), 1); \
			} while (0)
#define	md_reg_to_short(inst,reg)	\
			amd64_widen_reg((inst), (reg), (reg), 1, 1)
#define	md_reg_to_ushort(inst,reg)	\
			amd64_widen_reg((inst), (reg), (reg), 0, 1)
#define	md_reg_to_word_32(inst,reg)	\
			amd64_mov_reg_reg((inst), (reg), (reg), 4)
#define	md_reg_to_word_native(inst,reg)	\
			amd64_movsxd_reg_reg((inst), (reg), (reg))
#define	md_reg_to_word_native_un(inst,reg)	\
			amd64_mov_reg_reg_size((inst), (reg), (reg), 4)

/*
 * Truncate floating point values to 32-bit or 64-bit.
 * TODO: use cvt2* opcodes here
 */
#define	md_reg_to_float_32(inst,reg)	\
			do { \
				amd64_alu_reg_imm((inst), X86_SUB, AMD64_RSP, 4); \
				amd64_fst_membase((inst), AMD64_RSP, 0, 0, 1); \
				amd64_fld_membase((inst), AMD64_RSP, 0, 0); \
				amd64_alu_reg_imm((inst), X86_ADD, AMD64_RSP, 4); \
			} while (0)
#define	md_reg_to_float_64(inst,reg)	\
			do { \
				amd64_alu_reg_imm((inst), X86_SUB, AMD64_RSP, 8); \
				amd64_fst_membase((inst), AMD64_RSP, 0, 1, 1); \
				amd64_fld_membase((inst), AMD64_RSP, 0, 1); \
				amd64_alu_reg_imm((inst), X86_ADD, AMD64_RSP, 8); \
			} while (0)

/*
 * Convert a signed 32 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_sword_32_float(inst,dreg,sreg)	\
			do { \
				amd64_alu_reg_imm((inst), X86_SUB, AMD64_RSP, 8); \
				amd64_mov_membase_reg((inst), AMD64_RSP, 0, (sreg), 4); \
				amd64_fild_membase((inst), AMD64_RSP, 0, 0); \
				amd64_alu_reg_imm((inst), X86_ADD, AMD64_RSP, 8); \
			}while(0)

/*
 * Convert an unsigned 32 bit value in the general register to a native
 * floating-point value an load it into the top fp register.
 */
#define	md_conv_uword_32_float(inst,dreg,sreg)	\
			do { \
				amd64_alu_reg_imm((inst), X86_SUB, AMD64_RSP, 8); \
				amd64_mov_membase_reg((inst), AMD64_RSP, 0, (sreg), 4); \
				amd64_mov_membase_imm((inst), AMD64_RSP, 4, 0, 4); \
				amd64_fild_membase((inst), AMD64_RSP, 0, 1); \
				amd64_alu_reg_imm((inst), X86_ADD, AMD64_RSP, 8); \
			}while(0)

/*
 * Convert a signed 64 bit value in the general register to a native
 * floating-point value and load it into the top fp register.
 */
#define	md_conv_sword_64_float(inst,dreg,sreg)	\
			do { \
				amd64_alu_reg_imm((inst), X86_SUB, AMD64_RSP, 8); \
				amd64_mov_membase_reg((inst), AMD64_RSP, 0, (sreg), 8); \
				amd64_fild_membase((inst), AMD64_RSP, 0, 1); \
				amd64_alu_reg_imm((inst), X86_ADD, AMD64_RSP, 8); \
			}while(0)

/*
 * Swap the top two items on the floating-point stack.
 */
#define	md_freg_swap(inst)		amd64_fxch((inst), 1)

/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".  If "label" is non-NULL, then it indicates the address
 * of the CVM instruction handler to jump directly to.
 */
#define	md_jump_to_cvm(inst,pc,label)	\
			do { \
				amd64_mov_reg_imm_size((inst), MD_REG_PC, (void*)pc, 8); \
				if((label)) \
				{ \
					amd64_jump_code((inst), label); \
				} \
				else \
				{ \
					amd64_jump_membase((inst), MD_REG_PC, 0); \
				} \
			} while (0)

/*
 * Jump to a program counter that is defined by a switch table.
 */
extern md_inst_ptr _md_amd64_switch(md_inst_ptr inst, int reg, void * table);
#define	md_switch(inst,reg,table)	\
			do { \
				(inst) = _md_amd64_switch((inst), (reg), (table));\
			} while (0)

/*
 * Perform a clear operation at a memory base.
 */
#define	md_clear_membase_start(inst)	do { ; } while (0)
#define	md_clear_membase(inst,reg,offset)	\
			do { \
				amd64_mov_membase_imm((inst), (reg), (offset), 0, 8); \
			} while (0)

/*
 * Load the effective address of a memory base into a register.
 */
#define	md_lea_membase(inst,reg,basereg,offset)	\
			do { \
				int __value = (int)(offset); \
				if(!__value) \
				{ \
					amd64_mov_reg_reg((inst), (reg), (basereg), 8); \
				} \
				else \
				{ \
					amd64_lea_membase((inst), (reg), (basereg), __value); \
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
					amd64_lea_memindex((inst), (reg), (basereg), (offset), (indexreg), (shift)); \
				} \
				else \
				{ \
					amd64_shift_reg_imm((inst), X86_SHL, (indexreg), (shift)); \
					amd64_lea_memindex((inst), (reg), (basereg), (offset), (indexreg), 0); \
				} \
			} while (0)

/*
 * Move values between registers.
 */
#define	md_mov_reg_reg(inst,dreg,sreg)	\
			amd64_mov_reg_reg((inst), (dreg), (sreg), 4)

/*
 * Set a register to a 0 or 1 value based on a condition.
 */
extern md_inst_ptr _md_amd64_setcc(md_inst_ptr inst, int reg, int cond);
#define	md_seteq_reg(inst,reg)	\
			do { (inst) = _md_amd64_setcc((inst), (reg), X86_CC_EQ); } while (0)
#define	md_setne_reg(inst,reg)	\
			do { (inst) = _md_amd64_setcc((inst), (reg), X86_CC_NE); } while (0)
#define	md_setlt_reg(inst,reg)	\
			do { (inst) = _md_amd64_setcc((inst), (reg), X86_CC_LT); } while (0)
#define	md_setle_reg(inst,reg)	\
			do { (inst) = _md_amd64_setcc((inst), (reg), X86_CC_LE); } while (0)
#define	md_setgt_reg(inst,reg)	\
			do { (inst) = _md_amd64_setcc((inst), (reg), X86_CC_GT); } while (0)
#define	md_setge_reg(inst,reg)	\
			do { (inst) = _md_amd64_setcc((inst), (reg), X86_CC_GE); } while (0)

/*
 * Set a register to -1, 0, or 1 based on comparing two values.
 */
extern md_inst_ptr _md_amd64_compare
				(md_inst_ptr inst, int reg1, int reg2, int isSigned, int size);
#define	md_cmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_amd64_compare \
					((inst), (reg1), (reg2), 1, 4); } while (0)
#define	md_ucmp_reg_reg_word_32(inst,reg1,reg2)	\
			do { (inst) = _md_amd64_compare \
					((inst), (reg1), (reg2), 0, 4); } while (0)
#define	md_ucmp_reg_reg_word_native(inst,reg1,reg2)	\
			do { (inst) = _md_amd64_compare \
					((inst), (reg1), (reg2), 0, 8); } while (0)
/*
 * Set the condition codes based on comparing two values.
 * The "cond" value indicates the type of condition that we
 * want to check for.
 */
#define	md_cmp_cc_reg_reg_word_32(inst,cond,reg1,reg2)	\
			amd64_alu_reg_reg_size((inst), X86_CMP, (reg1), (reg2), 4)
#define	md_cmp_cc_reg_reg_word_native(inst,cond,reg1,reg2)	\
			amd64_alu_reg_reg((inst), X86_CMP, (reg1), (reg2))

/*
 * Test the contents of a register against NULL and set the
 * condition codes based on the result.
 */
#define	md_reg_is_null(inst,reg)	\
			amd64_alu_reg_reg((inst), X86_OR, (reg), (reg))

/*
 * Test the contents of a register against 32-bit zero and set the
 * condition codes based on the result.
 */
#define	md_reg_is_zero(inst,reg)	\
			amd64_alu_reg_reg((inst), X86_OR, (reg), (reg))

/*
 * Compare a 32-bit register against an immediate value and set
 * the condition codes based on the result.
 */
#define	md_cmp_reg_imm_word_32(inst,cond,reg,imm)	\
			amd64_alu_reg_imm_size((inst), X86_CMP, (reg), (int)(imm), 4)

/*
 * Output a branch to a location based on a condition.  The actual
 * jump offset will be filled in by a later "md_patch" call.
 */
#define	md_branch_eq(inst)	\
			amd64_branch32((inst), X86_CC_EQ, 0, 0)
#define	md_branch_ne(inst)	\
			amd64_branch32((inst), X86_CC_NE, 0, 0)
#define	md_branch_lt(inst)	\
			amd64_branch32((inst), X86_CC_LT, 0, 1)
#define	md_branch_le(inst)	\
			amd64_branch32((inst), X86_CC_LE, 0, 1)
#define	md_branch_gt(inst)	\
			amd64_branch32((inst), X86_CC_GT, 0, 1)
#define	md_branch_ge(inst)	\
			amd64_branch32((inst), X86_CC_GE, 0, 1)
#define	md_branch_lt_un(inst)	\
			amd64_branch32((inst), X86_CC_LT, 0, 0)
#define	md_branch_le_un(inst)	\
			amd64_branch32((inst), X86_CC_LE, 0, 0)
#define	md_branch_gt_un(inst)	\
			amd64_branch32((inst), X86_CC_GT, 0, 0)
#define	md_branch_ge_un(inst)	\
			amd64_branch32((inst), X86_CC_GE, 0, 0)
#define	md_branch_cc(inst,cond)	\
			amd64_branch32((inst), (cond) & 15, 0, ((cond) & 16) != 0)

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
			amd64_patch((patch), (inst))

/*
 * Check an array bounds value.  "reg1" points to the array,
 * and "reg2" is the array index to check.
 */
#define	md_bounds_check(inst,reg1,reg2)	\
			amd64_alu_reg_membase((inst), X86_CMP, (reg2), (reg1), 0)

/*
 * Load a 32-bit word value from an indexed array.  "disp" is the offset
 * to use to skip over the array bounds value.  Some platforms may ignore
 * "disp" if they advance the base pointer in "md_bounds_check".
 */
#define	md_load_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_mov_reg_memindex((inst), (reg), (basereg), \
									 (disp), (indexreg), 2, 4); \
			} while (0)

/*
 * Load a native word value from an indexed array.
 */
#define	md_load_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_mov_reg_memindex((inst), (reg), (basereg), \
									 (disp), (indexreg), 3, 8); \
			} while (0)

/*
 * Load a byte value from an indexed array.
 */
#define	md_load_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 0, 0, 0); \
			} while (0)

/*
 * Load a signed byte value from an indexed array.
 */
#define	md_load_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 0, 1, 0); \
			} while (0)

/*
 * Load a short value from an indexed array.
 */
#define	md_load_memindex_short(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 1, 1, 1); \
			} while (0)

/*
 * Load an unsigned short value from an indexed array.
 */
#define	md_load_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_widen_memindex((inst), (reg), (basereg), \
								   (disp), (indexreg), 1, 0, 1); \
			} while (0)

/*
 * Load a 32 bit floatingpoint value from an indexed array.
 */
#define	md_load_memindex_float_32(inst,reg,basereg,indexreg,disp) \
			do { \
				amd64_fld_memindex((inst), (basereg), (disp), (indexreg), 0); \
			} while (0)

/*
 * Load a 64 bit floatingpoint value from an indexed array.
 */
#define	md_load_memindex_float_64(inst,reg,basereg,indexreg,disp) \
			do { \
				amd64_fld_memindex((inst), (basereg), (disp), (indexreg), 1); \
			} while (0)

/*
 * Store a 32 bit floatingpoint value to an indexed array.
 */
#define	md_store_memindex_float_32(inst,reg,basereg,indexreg,disp) \
			do { \
				amd64_fst_memindex((inst), (basereg), (disp), (indexreg), 0, 1); \
			} while (0)

/*
 * Store a 64 bit floatingpoint value to an indexed array.
 */
#define	md_store_memindex_float_64(inst,reg,basereg,indexreg,disp) \
			do { \
				amd64_fst_memindex((inst), (basereg), (disp), (indexreg), 1, 1); \
			} while (0)

/*
 * Store a 32-bit word value into an indexed array.
 */
#define	md_store_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 2, (reg), 4); \
			} while (0)

/*
 * Store a native word value into an indexed array.
 */
#define	md_store_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 3, (reg), 8); \
			} while (0)

/*
 * Store a byte value into an indexed array.
 */
extern md_inst_ptr _md_amd64_mov_memindex_reg_byte
			(md_inst_ptr inst, int basereg,
			 unsigned offset, int indexreg, int srcreg);
#define	md_store_memindex_byte(inst,reg,basereg,indexreg,disp)	\
			do { \
				(inst) = _md_amd64_mov_memindex_reg_byte \
					((inst), (basereg), (disp), (indexreg), (reg)); \
			} while (0)

/*
 * Store a signed byte value into an indexed array.
 */
#define	md_store_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
			do { \
				(inst) = _md_amd64_mov_memindex_reg_byte \
					((inst), (basereg), (disp), (indexreg), (reg)); \
			} while (0)

/*
 * Store a short value into an indexed array.
 */
#define	md_store_memindex_short(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 1, (reg), 2); \
			} while (0)

/*
 * Store an unsigned short value into an indexed array.
 */
#define	md_store_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
			do { \
				amd64_mov_memindex_reg((inst), (basereg), (disp), (indexreg), \
									 1, (reg), 2); \
			} while (0)

/*
 * Absolute of a floatinpoint value
 */
#define md_abs_reg_float(inst,reg) \
			do { \
				amd64_fabs((inst)); \
			} while (0)

/*
 * Square root of a floatingpoint value
 */
#define md_sqrt_reg_float(inst,reg) \
			do { \
				amd64_fsqrt((inst)); \
			} while (0)

/*
 * Sine of a floatingpoint value
 */
#define md_sin_reg_float(inst,reg) \
			do { \
				amd64_fsin((inst)); \
			} while (0)

/*
 * Cosine of a floatingpoint value
 */
#define md_cos_reg_float(inst,reg) \
			do { \
				amd64_fcos((inst)); \
			} while (0)

/*
 * Tangent of a floatingpoint value
 */
#define md_tan_reg_float(inst,reg) \
			do { \
				amd64_fptan((inst)); \
				amd64_fstp((inst), 0); \
			} while (0)

#ifdef	__cplusplus
};
#endif

#endif /* _ENGINE_MD_X86_H */
