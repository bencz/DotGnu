/*
 * md_default.h - Define missing macros to generic defaults.
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

#ifndef	_ENGINE_MD_DEFAULT_H
#define	_ENGINE_MD_DEFAULT_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Perform arithmetic and logical operations on native word registers.
 */
#if defined(IL_NATIVE_INT32) && !defined(md_add_reg_reg_word_native)
#define	md_add_reg_reg_word_native(inst,reg1,reg2)	\
			md_add_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_sub_reg_reg_word_native(inst,reg1,reg2)	\
			md_sub_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_mul_reg_reg_word_native(inst,reg1,reg2)	\
			md_mul_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_div_reg_reg_word_native(inst,reg1,reg2)	\
			md_div_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_udiv_reg_reg_word_native(inst,reg1,reg2,regsused)	\
			md_udiv_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_rem_reg_reg_word_native(inst,reg1,reg2,regsused)	\
			md_rem_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_urem_reg_reg_word_native(inst,reg1,reg2,regsused)	\
			md_urem_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_neg_reg_word_native(inst,reg)	\
			md_neg_reg_word_32((inst), (reg))
#define	md_and_reg_reg_word_native(inst,reg1,reg2)	\
			md_and_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_xor_reg_reg_word_native(inst,reg1,reg2)	\
			md_xor_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_or_reg_reg_word_native(inst,reg1,reg2)	\
			md_or_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_not_reg_word_native(inst,reg)	\
			md_not_reg_word_32((inst), (reg))
#define	md_shl_reg_reg_word_native(inst,reg1,reg2)	\
			md_shl_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_shr_reg_reg_word_native(inst,reg1,reg2,regsused)	\
			md_shr_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_ushr_reg_reg_word_native(inst,reg1,reg2,regsused)	\
			md_ushr_reg_reg_word_32((inst), (reg1), (reg2))
#endif

/*
 * Perform arithmetic operations on native float values.  If the system
 * uses a floating-point stack, then the register arguments are ignored.
 */
#if !defined(md_add_reg_reg_float)
#define	md_add_reg_reg_float(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_sub_reg_reg_float(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_mul_reg_reg_float(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_div_reg_reg_float(inst,reg1,reg2)	\
			do { ; } while (0)
#define	md_rem_reg_reg_float(inst,reg1,reg2,used)	\
			do { ; } while (0)
#define	md_neg_reg_float(inst,reg)	\
			do { ; } while (0)
#define	md_cmp_reg_reg_float(inst,dreg,sreg1,sreg2,lessop)	\
			do { ; } while (0)
#endif

/*
 * Set a register to -1, 0, or 1 based on comparing two values.
 */
#if defined(IL_NATIVE_INT32) && !defined(md_cmp_reg_reg_word_native)
#define	md_cmp_reg_reg_word_native(inst,reg1,reg2)	\
			md_cmp_reg_reg_word_32((inst), (reg1), (reg2))
#define	md_ucmp_reg_reg_word_native(inst,reg1,reg2)	\
			md_ucmp_reg_reg_word_32((inst), (reg1), (reg2))
#endif

#ifdef	__cplusplus
};
#endif

#endif /* _ENGINE_MD_DEFAULT_H */
