/*
 * md_ia64.h - Machine-dependent definitions of IA-64
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Contributed by :CH Gowri Kumar <gkumar@csa.iisc.ernet.in>
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

#ifndef	_ENGINE_MD_IA_64_H
#define	_ENGINE_MD_IA_64_H

#include "md_ia64_macros.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#define dmesg(str) \
	IA64_DebugMessageBox(str,__FILE__,__LINE__,__FUNCTION__)

/*
 * Register numbers in the standard register allocation order.
 * -1 is the list terminator.
 */
#define	MD_REG_0	IA64_R16		
#define	MD_REG_1	IA64_R17	
#define	MD_REG_2	IA64_R18	
#define	MD_REG_3	IA64_R19	
#define	MD_REG_4	IA64_R20		
#define	MD_REG_5	IA64_R21		
#define	MD_REG_6	IA64_R22		
#define	MD_REG_7	IA64_R23	
#define	MD_REG_8	IA64_R24		
#define	MD_REG_9	IA64_R25		
#define	MD_REG_10	IA64_R26		
#define	MD_REG_11	IA64_R27		
#define	MD_REG_12	IA64_R28		
#define	MD_REG_13	IA64_R29		
#define	MD_REG_14	IA64_R30		
#define	MD_REG_15	IA64_R31		

/*
 * Mask that indicates a floating-point register.
 */
#define	MD_FREG_MASK	0x0000

/*
 * Floating point register numbers in the standard allocation order.
 * -1 is the list terminator.  The floating point register numbers
 * must include the MD_FREG_MASK value.
 */
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
#define	MD_REG_PC		IA64_R4	

/*
 * The register that contains the CVM stacktop pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_STACK	IA64_R5

/*
 * The register that contains the CVM frame pointer.  This must not
 * be present in the standard register allocation order.
 */
#define	MD_REG_FRAME	IA64_R6

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
#define	MD_REGS_TO_BE_SAVED	0	

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
#define	MD_LITTLE_ENDIAN_LONGS	0

/*
 * Type of the instruction pointer for outputting code.
 */
typedef ia64_inst_ptr	md_inst_ptr;

/*
 * Push a word register onto the system stack.
 */
#define	md_push_reg(inst,reg)	dmesg("md_push_reg")

/*
 * Pop a word register from the system stack.
 */
#define	md_pop_reg(inst,reg) dmesg("md_pop_reg")	

/*
 * Discard the contents of a floating-point register.
 */
#define	md_discard_freg(inst,reg)	dmesg("md_discard_freg")

/*
 * Load a 32-bit integer constant into a register.  This will sign-extend
 * if the native word size is larger.
 */
#define	md_load_const_32(inst,reg,value)	\
	do{\
		if(INRANGE((value),14))\
		{\
			template = MMI_;\
			instr0 = ADDS_rimm14r(0,(reg),(value),IA64_R0);\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			*(inst) = MOVL_rimm64(0,IA64_R2,(value));\
			(inst)++;\
			template = MMI_;\
			instr0 = ADD_rrr(0,(reg),IA64_R2,IA64_R0);\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
	}while(0);

/*
 * Load a native integer constant into a register.
 */
#define	md_load_const_native(inst,reg,value)	\
	md_load_const_32((inst),(reg),(value))

/*
 * Load a 32-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_32(inst,reg,mem)	\
			dmesg("md_load_const_float_32")

/*
 * Load a 64-bit floating-point constant into a register.  The constant
 * is at a particular memory location.  If the system does not use
 * floating-point registers, then load onto the top of the stack.
 */
#define	md_load_const_float_64(inst,reg,mem)	\
	dmesg("md_load_const_float_64")
	
/*
 * Load the 32-bit constant zero into a register.  This will zero-extend
 * if the native word size is larger.
 */
#define	md_load_zero_32(inst,reg)	\
	do {\
		instr0 = ADD_rrr(0,(reg),IA64_R0,IA64_R0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		template = MMI_;\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);
		

/*
 * Load the native constant zero into a register.
 */
#define	md_load_zero_native(inst,reg)	\
	do {\
		instr0 = ADD_rrr(0,(reg),IA64_R0,IA64_R0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		template = MMI_;\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

/*
 * Load a 32-bit word register from an offset from a pointer register.
 * This will sign-extend if the native word size is larger.
 */
#define	md_load_membase_word_32(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = M_MI_;\
			instr0 = LD4_rr(0,(reg),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = SXT4_rr(0,(reg),(reg));\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = LD4_rr(0,(reg),IA64_R2);\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
				template = MMI_;\
				instr0 = NOP_M_imm21(0,0);\
				instr1 = NOP_M_imm21(0,0);\
				instr2 = SXT4_rr(0,(reg),(reg));\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				printf("offset : 0x%x\n",offset);\
				dmesg("md_load_membase_word_32 offset >imm14");\
			}\
		}\
	}while(0);

/*
 * Load a native-sized word register from an offset from a pointer register.
 */
#define	md_load_membase_word_native(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = MMI_;\
			instr0 = LD8_rr(0,(reg),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = LD8_rr(0,(reg),IA64_R2);\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_load_membase_word_native offset >imm14");\
			}\
		}\
	}while(0);

/*
 * Load a 64-bit word value from an offset from a pointer register
 * as a pair of 32-bit registers.  Only used on 32-bit platforms.
 */
#define	md_load_membase_word_64(inst,lreg,hreg,basereg,offset)	\
		dmesg("md_load_membase_word_64")	

/*
 * Load a byte value from an offset from a pointer register.
 */
#define	md_load_membase_byte(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = MMI_;\
			instr0 = LD1_rr(0,(reg),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = LD1_rr(0,(reg),IA64_R2);\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_load_membase_byte offset >imm14");\
			}\
		}\
	}while(0);

/*
 * Load a signed byte value from an offset from a pointer register.
 */
#define	md_load_membase_sbyte(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = M_MI_;\
			instr0 = LD1_rr(0,(reg),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = SXT1_rr(0,(reg),(reg));\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = LD1_rr(0,(reg),IA64_R2);\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
				template = MMI_;\
				instr0 = NOP_M_imm21(0,0);\
				instr1 = NOP_M_imm21(0,0);\
				instr2 = SXT1_rr(0,(reg),(reg));\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				printf("offset : 0x%x\n",offset);\
				dmesg("md_load_membase_sbyte offset >imm14");\
			}\
		}\
	}while(0);

/*
 * Load a short value from an offset from a pointer register.
 */
#define	md_load_membase_short(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = M_MI_;\
			instr0 = LD2_rr(0,(reg),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = SXT2_rr(0,(reg),(reg));\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = LD2_rr(0,(reg),IA64_R2);\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
				template = MMI_;\
				instr0 = NOP_M_imm21(0,0);\
				instr1 = NOP_M_imm21(0,0);\
				instr2 = SXT2_rr(0,(reg),(reg));\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				printf("offset : 0x%x\n",offset);\
				dmesg("md_load_membase_short offset >imm14");\
			}\
		}\
	}while(0);

/*
 * Load an unsigned short value from an offset from a pointer register.
 */
#define	md_load_membase_ushort(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = MMI_;\
			instr0 = LD1_rr(0,(reg),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = LD1_rr(0,(reg),IA64_R2);\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_load_membase_ushort offset >imm14");\
			}\
		}\
	}while(0);

/*
 * Load a floating-point value from an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * loaded into "reg".  Otherwise it is loaded onto the top of the
 * floating-point stack.
 */
#define	md_load_membase_float_32(inst,reg,basereg,offset)	\
			dmesg("md_load_membase_float_32")
#define	md_load_membase_float_64(inst,reg,basereg,offset)	\
		dmesg("md_load_membase_float_64")
#define	md_load_membase_float_native(inst,reg,basereg,offset)	\
	dmesg("md_load_membase_float_native")

/*
 * Store a 32-bit word register to an offset from a pointer register.
 */
#define	md_store_membase_word_32(inst,reg,basereg,offset)	\
	do {\
		if(!(offset))\
		{\
			template = MMI_;\
			instr0 = ST4_rr(0,(basereg),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = ST4_rr(0,IA64_R2,(reg));\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_store_membase_word_32 offset >imm14");\
			}\
		}\
	}while(0);
		

/*
 * Store a native-sized word register to an offset from a pointer register.
 */
#define	md_store_membase_word_native(inst,reg,basereg,offset)	\
	do {\
		if((offset))\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = ST8_rr(0,IA64_R2,(reg));\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_store_membase_word_native offset >imm14");\
			}\
		}\
		else\
		{\
			template = MMI_;\
			instr0 = ST8_rr(0,(basereg),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
	}while(0);

/*
 * Store a pair of 32-bit word registers to an offset from a pointer
 * register as a 64-bit value.  Only used on 32-bit platforms.
 */
#define	md_store_membase_word_64(inst,lreg,hreg,basereg,offset)	\
	dmesg("md_store_membase_word_64")




/*
 * Store a byte value to an offset from a pointer register.
 */
#define	md_store_membase_byte(inst,reg,basereg,offset)	\
	do {\
		if((offset))\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = ST1_rr(0,IA64_R2,(reg));\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_store_membase_byte offset >imm14");\
			}\
		}\
		else\
		{\
			template = MMI_;\
			instr0 = ST1_rr(0,(basereg),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
	}while(0);


/*
 * Store a signed byte value to an offset from a pointer register.
 */
#define	md_store_membase_sbyte(inst,reg,basereg,offset)	\
	do {\
		if((offset))\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = ST1_rr(0,IA64_R2,(reg));\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_store_membase_sbyte offset >imm14");\
			}\
		}\
		else\
		{\
			template = MMI_;\
			instr0 = ST1_rr(0,(basereg),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
	}while(0);

/*
 * Store a short value to an offset from a pointer register.
 */
#define	md_store_membase_short(inst,reg,basereg,offset)	\
	do {\
		if((offset))\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = ST1_rr(0,IA64_R2,(reg));\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_store_membase_short offset >imm14");\
			}\
		}\
		else\
		{\
			template = MMI_;\
			instr0 = ST1_rr(0,(basereg),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
	}while(0);

/*
 * Store an unsigned short value to an offset from a pointer register.
 */
#define	md_store_membase_ushort(inst,reg,basereg,offset)	\
	do {\
		if((offset))\
		{\
			if(INRANGE((offset),14))\
			{\
				template = M_MI_;\
				instr0 = ADDS_rimm14r(0,IA64_R2,(offset),(basereg));\
				instr1 = ST2_rr(0,IA64_R2,(reg));\
				instr2 = NOP_I_imm21(0,0);\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
			}\
			else\
			{\
				dmesg("md_store_membase_ushort offset >imm14");\
			}\
		}\
		else\
		{\
			template = MMI_;\
			instr0 = ST2_rr(0,(basereg),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
	}while(0);

/*
 * Store a floating-point value to an offset from a pointer register.
 * If the system uses floating-point registers, then the value is
 * stored from "reg".  Otherwise it is stored from the top of the
 * floating-point stack.
 */
#define	md_store_membase_float_32(inst,reg,basereg,offset)	\
	dmesg("md_store_membase_float_32")
#define	md_store_membase_float_64(inst,reg,basereg,offset)	\
	dmesg("md_store_membase_float_64")
#define	md_store_membase_float_native(inst,reg,basereg,offset)	\
	dmesg("md_store_membase_float_native")

/*
 * Add an immediate value to a register.
 */
#define	md_add_reg_imm(inst,reg,imm)	\
	do {\
		if(INRANGE((imm),14))\
		{\
			template = MMI_;\
			instr0 = ADDS_rimm14r(0,(reg),(imm),(reg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			dmesg("md_add_reg_imm imm > imm14");\
		}\
	}while(0);

/*
 * Subtract an immediate value from a register.
 */
#define	md_sub_reg_imm(inst,reg,imm)	\
	md_add_reg_imm((inst),(reg),-(imm))

/*
 * Perform arithmetic and logical operations on 32-bit word registers.
 */
#define	md_add_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = M_MI_;\
		instr0 = ADD_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg1),(reg1));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_sub_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = M_MI_;\
		instr0 = SUB_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg1),(reg1));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_mul_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg("md_mul_reg_reg_word_32")
#define	md_div_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg("md_div_reg_reg_word_32")
#define	md_udiv_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg("md_udiv_reg_reg_word_32")
#define	md_rem_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg("md_rem_reg_reg_word_32")
#define	md_urem_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg{"md_urem_reg_reg_word_32")

#define	md_neg_reg_word_32(inst,reg)	\
	do {\
		template = M_MI_;\
		instr0 = SUB_rrr(0,(reg),IA64_R0,(reg));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg),(reg));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_and_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = M_MI_;\
		instr0 = AND_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg1),(reg1));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_xor_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = M_MI_;\
		instr0 = XOR_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg1),(reg1));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_or_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = M_MI_;\
		instr0 = OR_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg1),(reg1));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_not_reg_word_32(inst,reg)	\
	do {\
		template = M_MI_;\
		instr0 = ANDCM_rimm8r(0,(reg),-1,(reg));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg),(reg));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_shl_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SHL_rrr(0,(reg1),(reg1),(reg2));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_shr_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SHR_rrr(0,(reg1),(reg1),(reg2));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_ushr_reg_reg_word_32(inst,reg1,reg2)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SHR_U_rrr(0,(reg1),(reg1),(reg2));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

/*
 * Perform arithmetic on 64-bit values represented as 32-bit word pairs.
 */
#define	md_add_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
	dmesg("md_add_reg_reg_word_64")
#define	md_sub_reg_reg_word_64(inst,lreg1,hreg1,lreg2,hreg2)	\
	dmesg("md_sub_reg_reg_word_64")
#define	md_neg_reg_word_64(inst,lreg,hreg)	\
	dmesg("md_neg_reg_word_64")

#define md_add_reg_reg_word_native(inst,reg1,reg2) \
	do {\
		template = MMI_;\
		instr0 = ADD_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define md_sub_reg_reg_word_native(inst,reg1,reg2) \
	do {\
		template = MMI_;\
		instr0 = SUB_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define md_and_reg_reg_word_native(inst,reg1,reg2) \
	do {\
		template = MMI_;\
		instr0 = AND_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define md_or_reg_reg_word_native(inst,reg1,reg2) \
	do {\
		template = MMI_;\
		instr0 = OR_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);
	
#define md_xor_reg_reg_word_native(inst,reg1,reg2) \
	do {\
		template = MMI_;\
		instr0 = XOR_rrr(0,(reg1),(reg1),(reg2));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_neg_reg_word_native(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = SUB_rrr(0,(reg),IA64_R0,(reg));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define md_not_reg_word_native(inst,reg) \
	do {\
		template = MMI_;\
		instr0 = ANDCM_rimm8r(0,(reg),-1,(reg));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

/*
 * Convert word registers between various types.
 */
#define	md_reg_to_byte(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = EXTR_U_rrpos6len6(0,(reg),(reg),0,8);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);
		
#define	md_reg_to_sbyte(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = EXTR_rrpos6len6(0,(reg),(reg),0,8);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_reg_to_short(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = EXTR_rrpos6len6(0,(reg),(reg),0,16);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_reg_to_ushort(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = EXTR_U_rrpos6len6(0,(reg),(reg),0,16);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_reg_to_word_32(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = EXTR_rrpos6len6(0,(reg),(reg),0,32);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

#define	md_reg_to_word_native(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = SXT4_rr(0,(reg),(reg));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);
	
#define	md_reg_to_word_native_un(inst,reg)	\
	do {\
		template = MMI_;\
		instr0 = NOP_M_imm21(0,0);\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = ZXT4_rr(0,(reg),(reg));\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

/*
 * Truncate floating point values to 32-bit or 64-bit.
 */
#define	md_reg_to_float_32(inst,reg)	\
	dmesg("md_reg_to_float_32")
#define	md_reg_to_float_64(inst,reg)	\
	dmesg("md_reg_to_float_64")

/*
 * Swap the top two items on the floating-point stack.
 */
#define	md_freg_swap(inst)	\
	dmesg("md_freg_swap")
/*
 * Jump back into the CVM interpreter to execute the instruction
 * at "pc".  If "label" is non-NULL, then it indicates the address
 * of the CVM instruction handler to jump directly to.
 */
#define	md_jump_to_cvm(inst,pc,label)	\
	do{\
		*(inst) = MOVL_rimm64(IA64_P0,MD_REG_PC,(unsigned long)(pc));\
		(inst)++;\
		if(!(label)) \
		{\
				instr0 = LD8_rr(IA64_P0,IA64_R2,MD_REG_PC);\
				instr1 = NOP_M_imm21(0,0);\
				instr2 = MOV_brtag13(0,IA64_B6,IA64_R2,0);\
				template = M_MI_;\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
				instr0 = NOP_M_imm21(0,0);\
				instr1 = NOP_I_imm21(0,0);\
				instr2 = BR_COND_SPTK_MANY_b(0,IA64_B6);\
				template = MIB_;\
				MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
				(inst)++;\
		}\
		else\
		{\
			dmesg("md_jump_to_cvm for non-NULL label");\
		}\
	}while(0);

/*
 * Jump to a program counter that is defined by a switch table.
 */
#define	md_switch(inst,reg,table)	\
	dmesg("md_switch")


/*
 * Perform a clear operation at a memory base.
 */
#define	md_clear_membase_start(inst)	\
	do{;}while(0);

#define	md_clear_membase(inst,reg,offset)	\
	md_store_membase_word_native((inst),(IA64_R0),(reg),(offset))


/*
 * Load the effective address of a memory base into a register.
 */
#define	md_lea_membase(inst,reg,basereg,offset)	\
	do {\
		if(INRANGE((offset),14))\
		{\
			template = MMI_;\
			instr0 = ADDS_rimm14r(0,(reg),(offset),(basereg));\
			instr1 = NOP_M_imm21(0,0);\
			instr2 = NOP_I_imm21(0,0);\
			MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
			(inst)++;\
		}\
		else\
		{\
			printf("offset : 0x%x\n",offset);\
			dmesg("md_lea_membase offset > imm14");\
		}\
	}while(0);


/*
 * Move values between registers.
 */
#define	md_mov_reg_reg(inst,dreg,sreg)	\
	do {\
		template = MMI_;\
		instr0 = ADD_rrr(0,(dreg),IA64_R0,(sreg));\
		instr1 = NOP_M_imm21(0,0);\
		instr2 = NOP_I_imm21(0,0);\
		MAKE_BUNDLE((inst),template,instr0,instr1,instr2);\
		(inst)++;\
	}while(0);

/*
 * Set a register to a 0 or 1 value based on a condition.
 */
extern md_inst_ptr _md_arm_setcc(md_inst_ptr inst, int reg,
								 int cond, int invcond);
#define	md_seteq_reg(inst,reg)	\
	dmesg("md_seteq_reg")

#define	md_setne_reg(inst,reg)	\
	dmesg("md_setne_reg")

#define	md_setlt_reg(inst,reg)	\
	dmesg("md_setlt_reg")

#define	md_setle_reg(inst,reg)	\
	dmesg("md_setle_reg")

#define	md_setgt_reg(inst,reg)	\
	dmesg("md_setgt_reg")

#define	md_setge_reg(inst,reg)	\
	dmesg("md_setge_reg")


/*
 * Set a register to -1, 0, or 1 based on comparing two values.
 */
#define	md_cmp_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg("md_cmp_reg_reg_word_32")

#define	md_ucmp_reg_reg_word_32(inst,reg1,reg2)	\
	dmesg("md_ucmp_reg_reg_word_32")

/*
 * Set the condition codes based on comparing two values.
 * The "cond" value indicates the type of condition that we
 * want to check for.
 */
#define	md_cmp_cc_reg_reg_word_32(inst,cond,reg1,reg2)	\
	dmesg("md_cmp_cc_reg_reg_word_32")
#define	md_cmp_cc_reg_reg_word_native(inst,cond,reg1,reg2)	\
	dmesg("md_cmp_cc_reg_reg_word_native")

/*
 * Test the contents of a register against NULL and set the
 * condition codes based on the result.
 */
#define	md_reg_is_null(inst,reg)	\
	dmesg("md_reg_is_null")

/*
 * Test the contents of a register against 32-bit zero and set the
 * condition codes based on the result.
 */
#define	md_reg_is_zero(inst,reg)	\
	dmesg("md_reg_is_zero")

/*
 * Compare a 32-bit register against an immediate value and set
 * the condition codes based on the result.
 */
#define	md_cmp_reg_imm_word_32(inst,cond,reg,imm)	\
	dmesg("md_cmp_reg_imm_word_32")

/*
 * Output a branch to a location based on a condition.  The actual
 * jump offset will be filled in by a later "md_patch" call.
 */
#define	md_branch_eq(inst)	\
	dmesg("md_branch_eq")
#define	md_branch_ne(inst)	\
	dmesg("md_branch_ne")
#define	md_branch_lt(inst)	\
	dmesg("md_branch_lt")
#define	md_branch_le(inst)	\
	dmesg("md_branch_le")
#define	md_branch_gt(inst)	\
	dmesg("md_branch_gt")
#define	md_branch_ge(inst)	\
	dmesg("md_branch_ge")
#define	md_branch_lt_un(inst)	\
	dmesg("md_branch_lt_un")
#define	md_branch_le_un(inst)	\
	dmesg("md_branch_le_un")
#define	md_branch_gt_un(inst)	\
	dmesg("md_branch_gt_un")
#define	md_branch_ge_un(inst)	\
	dmesg("md_branch_ge_un")
#define	md_branch_cc(inst,cond)	\
	dmesg("md_branch_cc")

/*
 * Specific condition codes for "md_branch_cc".
 */
#define	MD_CC_EQ				-1	
#define	MD_CC_NE				-1
#define	MD_CC_LT				-1
#define	MD_CC_LE				-1
#define	MD_CC_GT				-1
#define	MD_CC_GE				-1
#define	MD_CC_LT_UN			-1
#define	MD_CC_LE_UN			-1
#define	MD_CC_GT_UN			-1
#define	MD_CC_GE_UN			-1

/*
 * Back-patch a branch instruction at "patch" to branch to "inst".
 */
#define	md_patch(patch,inst)	\
	dmesg("md_patch")

/*
 * Check an array bounds value.  "reg1" points to the array,
 * and "reg2" is the array index to check.  This will advance
 * the pointer in "reg1" past the array bounds value.
 */
#define	md_bounds_check(inst,reg1,reg2)	\
	dmesg("md_bounds_check")

/*
 * Load a 32-bit word value from an indexed array.  "disp" is the offset
 * to use to skip over the array bounds value.  Some platforms may ignore
 * "disp" if they advance the base pointer in "md_bounds_check".
 */
#define	md_load_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_load_memindex_word_32")

/*
 * Load a native word value from an indexed array.
 */
#define	md_load_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_load_memindex_word_native")

/*
 * Load a byte value from an indexed array.
 */
#define	md_load_memindex_byte(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_load_memindex_byte")

/*
 * Load a signed byte value from an indexed array.
 */
#define	md_load_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_load_memindex_sbyte")

/*
 * Load a short value from an indexed array.
 */
#define	md_load_memindex_short(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_load_memindex_short")

/*
 * Load an unsigned short value from an indexed array.
 */
#define	md_load_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_load_memindex_ushort")

/*
 * Store a 32-bit word value into an indexed array.
 */
#define	md_store_memindex_word_32(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_store_memindex_word_32")

/*
 * Store a native word value into an indexed array.
 */
#define	md_store_memindex_word_native(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_store_memindex_word_native")

/*
 * Store a byte value into an indexed array.
 */
#define	md_store_memindex_byte(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_store_memindex_byte")

/*
 * Store a signed byte value into an indexed array.
 */
#define	md_store_memindex_sbyte(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_store_memindex_sbyte")

/*
 * Store a short value into an indexed array.
 */
#define	md_store_memindex_short(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_store_memindex_short")

/*
 * Store an unsigned short value into an indexed array.
 */
#define	md_store_memindex_ushort(inst,reg,basereg,indexreg,disp)	\
	dmesg("md_store_memindex_ushort")

#ifdef	__cplusplus
};
#endif

#endif /* _ENGINE_MD_IA-64_H */
