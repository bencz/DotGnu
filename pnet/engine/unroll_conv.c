/*
 * unroll_conv.c - Conversion handling for generic CVM unrolling.
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

#ifdef IL_UNROLL_CASES

case COP_I2B:
{
	/* Convert an integer into a byte */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_reg_to_sbyte(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_I2UB:
{
	/* Convert an integer into an unsigned byte */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_reg_to_byte(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_I2S:
{
	/* Convert an integer into a short */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_reg_to_short(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_I2US:
{
	/* Convert an integer into an unsigned short */
	UNROLL_START();
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	md_reg_to_ushort(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#ifdef MD_HAS_FP

#ifdef md_conv_sword_32_float

case COP_I2F:
{
	/* Read a float32 value from a pointer */
	UNROLL_START();
	CheckFPFull(&unroll);
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	reg2 = GetFPRegister(&unroll);
	md_conv_sword_32_float(unroll.out, reg2, reg);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_conv_sword_32_float */

#ifdef md_conv_uword_32_float

case COP_IU2F:
{
	/* Read a float32 value from a pointer */
	UNROLL_START();
	CheckFPFull(&unroll);
	reg = GetTopWordRegister(&unroll, MD_REG1_32BIT);
	reg2 = GetFPRegister(&unroll);
	md_conv_uword_32_float(unroll.out, reg2, reg);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_conv_uword_32_float */

#ifdef md_conv_sword_64_float

case COP_L2F:
{
	/* Convert a signed long value to float */
	UNROLL_START();
	CheckFPFull(&unroll);
#ifdef IL_NATIVE_INT32
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	reg3 = GetFPRegister(&unroll);
#if MD_LITTLE_ENDIAN_LONGS
	md_conv_sword_64_float(unroll.out, reg3, reg, reg2);
#else
	md_conv_sword_64_float(unroll.out, reg3, reg2, reg);
#endif
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1),
	PushRegister(&unroll, reg3, 0);
#else
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	reg2 = GetFPRegister(&unroll);
	md_conv_sword_64_float(unroll.out, reg2, reg);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_conv_sword_64_float */

#ifdef md_conv_uword_64_float

case COP_LU2F:
{
	/* Convert an unsigned long value to float */
	UNROLL_START();
	CheckFPFull(&unroll);
#ifdef IL_NATIVE_INT32
	GetTopTwoWordRegisters(&unroll, &reg, &reg2,
						   MD_REG1_32BIT | MD_REG2_32BIT);
	reg3 = GetFPRegister(&unroll);
#if MD_LITTLE_ENDIAN_LONGS
	md_conv_uword_64_float(unroll.out, reg3, reg, reg2);
#else
	md_conv_uword_64_float(unroll.out, reg3, reg2, reg);
#endif
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1),
	PushRegister(&unroll, reg3, 0);
#else
	reg = GetTopWordRegister(&unroll, MD_REG1_NATIVE);
	reg2 = GetFPRegister(&unroll);
	md_conv_uword_64_float(unroll.out, reg2, reg);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
#endif
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_conv_uword_64_float */

#ifdef md_conv_float_sword_32

case COP_F2I:
{
	/* Convert native float to signed 32 bit using truncate rounding mode */
	UNROLL_START();
	CheckFPFull(&unroll);
	reg = GetTopFPRegister(&unroll);
	reg2 = GetWordRegister(&unroll, 0);
	md_conv_float_sword_32(unroll.out, reg2, reg);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_conv_float_sword_32 */

#ifdef md_conv_float_uword_32

case COP_F2IU:
{
	/* Convert native float to unsigned 32 bit using truncate rounding mode */
	UNROLL_START();
	CheckFPFull(&unroll);
	reg = GetTopFPRegister(&unroll);
	reg2 = GetWordRegister(&unroll, 0);
	md_conv_float_uword_32(unroll.out, reg2, reg);
	FreeTopRegister(&unroll, -1);
	FreeTopRegister(&unroll, -1);
	PushRegister(&unroll, reg2, 0);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* md_conv_float_uword_32 */

case COP_F2F:
{
	/* Truncate a floating point value to float32 */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_reg_to_float_32(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

case COP_F2D:
{
	/* Truncate a floating point value to float64 */
	UNROLL_START();
	reg = GetTopFPRegister(&unroll);
	md_reg_to_float_64(unroll.out, reg);
	MODIFY_UNROLL_PC(CVM_LEN_NONE);
}
break;

#endif /* MD_HAS_FP */

#endif /* IL_UNROLL_CASES */
