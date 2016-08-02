/*
 * verify_conv.c - Verify instructions related to conversions.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if defined(IL_VERIFY_GLOBALS)

/* No globals required */

#elif defined(IL_VERIFY_LOCALS)

/* No locals required */

#else /* IL_VERIFY_CODE */

#define	VERIFY_CONV(name,resultType)	\
			case name: \
			{ \
				if(STK_UNARY >= ILEngineType_I4 && \
				   STK_UNARY <= ILEngineType_F) \
				{ \
					ILCoderConv(coder, opcode, STK_UNARY); \
					STK_UNARY = resultType; \
					STK_UNARY_TYPEINFO = 0; \
				} \
				else if(unsafeAllowed && \
						((STK_UNARY == ILEngineType_M || \
						 STK_UNARY == ILEngineType_O || \
						 STK_UNARY == ILEngineType_T) && \
						 (((resultType) == ILEngineType_I8) || \
						 ((resultType) == ILEngineType_I)))) \
				{ \
					ILCoderConv(coder, opcode, resultType); \
					STK_UNARY = resultType; \
					STK_UNARY_TYPEINFO = 0; \
				} \
				else \
				{ \
					VERIFY_TYPE_ERROR(); \
				} \
			} \
			break

VERIFY_CONV(IL_OP_CONV_I1, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_I2, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_I4, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_I8, ILEngineType_I8);
VERIFY_CONV(IL_OP_CONV_R4, ILEngineType_F);
VERIFY_CONV(IL_OP_CONV_R8, ILEngineType_F);
VERIFY_CONV(IL_OP_CONV_U4, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_U8, ILEngineType_I8);
VERIFY_CONV(IL_OP_CONV_R_UN, ILEngineType_F);
VERIFY_CONV(IL_OP_CONV_OVF_I1_UN, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_I2_UN, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_I4_UN, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_I8_UN, ILEngineType_I8);
VERIFY_CONV(IL_OP_CONV_OVF_U1_UN, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_U2_UN, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_U4_UN, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_U8_UN, ILEngineType_I8);
VERIFY_CONV(IL_OP_CONV_OVF_I_UN, ILEngineType_I);
VERIFY_CONV(IL_OP_CONV_OVF_U_UN, ILEngineType_I);
VERIFY_CONV(IL_OP_CONV_OVF_I1, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_U1, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_I2, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_U2, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_I4, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_U4, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_OVF_I8, ILEngineType_I8);
VERIFY_CONV(IL_OP_CONV_OVF_U8, ILEngineType_I8);
VERIFY_CONV(IL_OP_CONV_U2, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_U1, ILEngineType_I4);
VERIFY_CONV(IL_OP_CONV_I, ILEngineType_I);
VERIFY_CONV(IL_OP_CONV_OVF_I, ILEngineType_I);
VERIFY_CONV(IL_OP_CONV_OVF_U, ILEngineType_I);
VERIFY_CONV(IL_OP_CONV_U, ILEngineType_I);

#endif /* IL_VERIFY_CODE */
