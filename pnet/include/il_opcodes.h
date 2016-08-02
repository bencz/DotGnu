/*
 * il_opcodes.h - Opcodes for the "Intermediate Language" instruction set.
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

#ifndef	_IL_OPCODES_H
#define	_IL_OPCODES_H

/*
 * Main instruction set.
 */

#define	IL_OP_NOP							0x00
#define	IL_OP_BREAK							0x01

#define	IL_OP_LDARG_0						0x02
#define	IL_OP_LDARG_1						0x03
#define	IL_OP_LDARG_2						0x04
#define	IL_OP_LDARG_3						0x05
#define	IL_OP_LDLOC_0						0x06
#define	IL_OP_LDLOC_1						0x07
#define	IL_OP_LDLOC_2						0x08
#define	IL_OP_LDLOC_3						0x09
#define	IL_OP_STLOC_0						0x0A
#define	IL_OP_STLOC_1						0x0B
#define	IL_OP_STLOC_2						0x0C
#define	IL_OP_STLOC_3						0x0D

#define	IL_OP_LDARG_S						0x0E
#define	IL_OP_LDARGA_S						0x0F
#define	IL_OP_STARG_S						0x10
#define	IL_OP_LDLOC_S						0x11
#define	IL_OP_LDLOCA_S						0x12
#define	IL_OP_STLOC_S						0x13

#define	IL_OP_LDNULL						0x14
#define	IL_OP_LDC_I4_M1						0x15
#define	IL_OP_LDC_I4_0						0x16
#define	IL_OP_LDC_I4_1						0x17
#define	IL_OP_LDC_I4_2						0x18
#define	IL_OP_LDC_I4_3						0x19
#define	IL_OP_LDC_I4_4						0x1A
#define	IL_OP_LDC_I4_5						0x1B
#define	IL_OP_LDC_I4_6						0x1C
#define	IL_OP_LDC_I4_7						0x1D
#define	IL_OP_LDC_I4_8						0x1E
#define	IL_OP_LDC_I4_S						0x1F
#define	IL_OP_LDC_I4						0x20
#define	IL_OP_LDC_I8						0x21
#define	IL_OP_LDC_R4						0x22
#define	IL_OP_LDC_R8						0x23

#define	IL_OP_LDPTR							0x24

#define	IL_OP_DUP							0x25
#define	IL_OP_POP							0x26

#define	IL_OP_JMP							0x27
#define	IL_OP_CALL							0x28
#define	IL_OP_CALLI							0x29
#define	IL_OP_RET							0x2A

#define	IL_OP_BR_S							0x2B
#define	IL_OP_BRFALSE_S						0x2C
#define	IL_OP_BRTRUE_S						0x2D
#define	IL_OP_BEQ_S							0x2E
#define	IL_OP_BGE_S							0x2F
#define	IL_OP_BGT_S							0x30
#define	IL_OP_BLE_S							0x31
#define	IL_OP_BLT_S							0x32
#define	IL_OP_BNE_UN_S						0x33
#define	IL_OP_BGE_UN_S						0x34
#define	IL_OP_BGT_UN_S						0x35
#define	IL_OP_BLE_UN_S						0x36
#define	IL_OP_BLT_UN_S						0x37
#define	IL_OP_BR							0x38
#define	IL_OP_BRFALSE						0x39
#define	IL_OP_BRTRUE						0x3A
#define	IL_OP_BEQ							0x3B
#define	IL_OP_BGE							0x3C
#define	IL_OP_BGT							0x3D
#define	IL_OP_BLE							0x3E
#define	IL_OP_BLT							0x3F
#define	IL_OP_BNE_UN						0x40
#define	IL_OP_BGE_UN						0x41
#define	IL_OP_BGT_UN						0x42
#define	IL_OP_BLE_UN						0x43
#define	IL_OP_BLT_UN						0x44

#define	IL_OP_SWITCH						0x45

#define	IL_OP_LDIND_I1						0x46
#define	IL_OP_LDIND_U1						0x47
#define	IL_OP_LDIND_I2						0x48
#define	IL_OP_LDIND_U2						0x49
#define	IL_OP_LDIND_I4						0x4A
#define	IL_OP_LDIND_U4						0x4B
#define	IL_OP_LDIND_I8						0x4C
#define	IL_OP_LDIND_I						0x4D
#define	IL_OP_LDIND_R4						0x4E
#define	IL_OP_LDIND_R8						0x4F
#define	IL_OP_LDIND_REF						0x50
#define	IL_OP_STIND_REF						0x51
#define	IL_OP_STIND_I1						0x52
#define	IL_OP_STIND_I2						0x53
#define	IL_OP_STIND_I4						0x54
#define	IL_OP_STIND_I8						0x55
#define	IL_OP_STIND_R4						0x56
#define	IL_OP_STIND_R8						0x57

#define	IL_OP_ADD							0x58
#define	IL_OP_SUB							0x59
#define	IL_OP_MUL							0x5A
#define	IL_OP_DIV							0x5B
#define	IL_OP_DIV_UN						0x5C
#define	IL_OP_REM							0x5D
#define	IL_OP_REM_UN						0x5E
#define	IL_OP_AND							0x5F
#define	IL_OP_OR							0x60
#define	IL_OP_XOR							0x61
#define	IL_OP_SHL							0x62
#define	IL_OP_SHR							0x63
#define	IL_OP_SHR_UN						0x64
#define	IL_OP_NEG							0x65
#define	IL_OP_NOT							0x66

#define	IL_OP_CONV_I1						0x67
#define	IL_OP_CONV_I2						0x68
#define	IL_OP_CONV_I4						0x69
#define	IL_OP_CONV_I8						0x6A
#define	IL_OP_CONV_R4						0x6B
#define	IL_OP_CONV_R8						0x6C
#define	IL_OP_CONV_U4						0x6D
#define	IL_OP_CONV_U8						0x6E

#define	IL_OP_CALLVIRT						0x6F
#define	IL_OP_CPOBJ							0x70
#define	IL_OP_LDOBJ							0x71
#define	IL_OP_LDSTR							0x72

#define	IL_OP_NEWOBJ						0x73
#define	IL_OP_CASTCLASS						0x74
#define	IL_OP_ISINST						0x75

#define	IL_OP_CONV_R_UN						0x76
#define	IL_OP_ANN_DATA_S					0x77

#define	IL_OP_UNBOX							0x79

#define	IL_OP_THROW							0x7A

#define	IL_OP_LDFLD							0x7B
#define	IL_OP_LDFLDA						0x7C
#define	IL_OP_STFLD							0x7D
#define	IL_OP_LDSFLD						0x7E
#define	IL_OP_LDSFLDA						0x7F
#define	IL_OP_STSFLD						0x80
#define	IL_OP_STOBJ							0x81

#define	IL_OP_CONV_OVF_I1_UN				0x82
#define	IL_OP_CONV_OVF_I2_UN				0x83
#define	IL_OP_CONV_OVF_I4_UN				0x84
#define	IL_OP_CONV_OVF_I8_UN				0x85
#define	IL_OP_CONV_OVF_U1_UN				0x86
#define	IL_OP_CONV_OVF_U2_UN				0x87
#define	IL_OP_CONV_OVF_U4_UN				0x88
#define	IL_OP_CONV_OVF_U8_UN				0x89
#define	IL_OP_CONV_OVF_I_UN					0x8A
#define	IL_OP_CONV_OVF_U_UN					0x8B

#define	IL_OP_BOX							0x8C

#define	IL_OP_NEWARR						0x8D
#define	IL_OP_LDLEN							0x8E
#define	IL_OP_LDELEMA						0x8F
#define	IL_OP_LDELEM_I1						0x90
#define	IL_OP_LDELEM_U1						0x91
#define	IL_OP_LDELEM_I2						0x92
#define	IL_OP_LDELEM_U2						0x93
#define	IL_OP_LDELEM_I4						0x94
#define	IL_OP_LDELEM_U4						0x95
#define	IL_OP_LDELEM_I8						0x96
#define	IL_OP_LDELEM_I						0x97
#define	IL_OP_LDELEM_R4						0x98
#define	IL_OP_LDELEM_R8						0x99
#define	IL_OP_LDELEM_REF					0x9A

#define	IL_OP_STELEM_I						0x9B
#define	IL_OP_STELEM_I1						0x9C
#define	IL_OP_STELEM_I2						0x9D
#define	IL_OP_STELEM_I4						0x9E
#define	IL_OP_STELEM_I8						0x9F
#define	IL_OP_STELEM_R4						0xA0
#define	IL_OP_STELEM_R8						0xA1
#define	IL_OP_STELEM_REF					0xA2

#define	IL_OP_LDELEM						0xA3
#define	IL_OP_STELEM						0xA4
#define	IL_OP_UNBOX_ANY						0xA5

#define	IL_OP_UNUSED_A6						0xA6		/* !! */
#define	IL_OP_UNUSED_A7						0xA7
#define	IL_OP_UNUSED_A8						0xA8
#define	IL_OP_UNUSED_A9						0xA9
#define	IL_OP_UNUSED_AA						0xAA
#define	IL_OP_UNUSED_AB						0xAB
#define	IL_OP_UNUSED_AC						0xAC
#define	IL_OP_UNUSED_AD						0xAD
#define	IL_OP_UNUSED_AE						0xAE
#define	IL_OP_UNUSED_AF						0xAF
#define	IL_OP_UNUSED_B0						0xB0
#define	IL_OP_UNUSED_B1						0xB1
#define	IL_OP_UNUSED_B2						0xB2

#define	IL_OP_CONV_OVF_I1					0xB3
#define	IL_OP_CONV_OVF_U1					0xB4
#define	IL_OP_CONV_OVF_I2					0xB5
#define	IL_OP_CONV_OVF_U2					0xB6
#define	IL_OP_CONV_OVF_I4					0xB7
#define	IL_OP_CONV_OVF_U4					0xB8
#define	IL_OP_CONV_OVF_I8					0xB9
#define	IL_OP_CONV_OVF_U8					0xBA

#define	IL_OP_UNUSED_BB						0xBB		/* !! */
#define	IL_OP_UNUSED_BC						0xBC
#define	IL_OP_UNUSED_BD						0xBD
#define	IL_OP_UNUSED_BE						0xBE
#define	IL_OP_UNUSED_BF						0xBF
#define	IL_OP_UNUSED_C0						0xC0
#define	IL_OP_UNUSED_C1						0xC1

#define	IL_OP_REFANYVAL						0xC2
#define	IL_OP_CKFINITE						0xC3

#define	IL_OP_UNUSED_C4						0xC4		/* !! */
#define	IL_OP_UNUSED_C5						0xC5

#define	IL_OP_MKREFANY						0xC6

#define	IL_OP_ANN_CALL						0xC7
#define	IL_OP_ANN_CATCH						0xC8
#define	IL_OP_ANN_DEAD						0xC9
#define	IL_OP_ANN_HOISTED					0xCA
#define	IL_OP_ANN_HOISTED_CALL				0xCB
#define	IL_OP_ANN_LAB						0xCC		/* !! - not defined */
#define	IL_OP_ANN_DEF						0xCD		/* !! - not defined */
#define	IL_OP_ANN_REF_S						0xCE
#define	IL_OP_ANN_PHI						0xCF

#define	IL_OP_LDTOKEN						0xD0

#define	IL_OP_CONV_U2						0xD1
#define	IL_OP_CONV_U1						0xD2
#define	IL_OP_CONV_I						0xD3
#define	IL_OP_CONV_OVF_I					0xD4
#define	IL_OP_CONV_OVF_U					0xD5

#define	IL_OP_ADD_OVF						0xD6
#define	IL_OP_ADD_OVF_UN					0xD7
#define	IL_OP_MUL_OVF						0xD8
#define	IL_OP_MUL_OVF_UN					0xD9
#define	IL_OP_SUB_OVF						0xDA
#define	IL_OP_SUB_OVF_UN					0xDB

#define	IL_OP_ENDFINALLY					0xDC

#define	IL_OP_LEAVE							0xDD
#define	IL_OP_LEAVE_S						0xDE

#define	IL_OP_STIND_I						0xDF

#define	IL_OP_CONV_U						0xE0

#define	IL_OP_PREFIX						0xFE

/*
 * Prefixed instruction set.
 */

#define	IL_PREFIX_OP_ARGLIST				0x00

#define	IL_PREFIX_OP_CEQ					0x01
#define	IL_PREFIX_OP_CGT					0x02
#define	IL_PREFIX_OP_CGT_UN					0x03
#define	IL_PREFIX_OP_CLT					0x04
#define	IL_PREFIX_OP_CLT_UN					0x05

#define	IL_PREFIX_OP_LDFTN					0x06
#define	IL_PREFIX_OP_LDVIRTFTN				0x07

#define	IL_PREFIX_OP_JMPI					0x08

#define	IL_PREFIX_OP_LDARG					0x09
#define	IL_PREFIX_OP_LDARGA					0x0A
#define	IL_PREFIX_OP_STARG					0x0B
#define	IL_PREFIX_OP_LDLOC					0x0C
#define	IL_PREFIX_OP_LDLOCA					0x0D
#define	IL_PREFIX_OP_STLOC					0x0E

#define	IL_PREFIX_OP_LOCALLOC				0x0F

#define	IL_PREFIX_OP_UNUSED_PREFIX_10		0x10		/* !! */

#define	IL_PREFIX_OP_ENDFILTER				0x11
#define	IL_PREFIX_OP_UNALIGNED				0x12
#define	IL_PREFIX_OP_VOLATILE				0x13
#define	IL_PREFIX_OP_TAIL					0x14
#define	IL_PREFIX_OP_INITOBJ				0x15
#define	IL_PREFIX_OP_CONSTRAINED			0x16
#define	IL_PREFIX_OP_CPBLK					0x17
#define	IL_PREFIX_OP_INITBLK				0x18
#define	IL_PREFIX_OP_NO						0x19
#define	IL_PREFIX_OP_RETHROW				0x1A

#define	IL_PREFIX_OP_UNUSED_PREFIX_1B		0x1B		/* !! */

#define	IL_PREFIX_OP_SIZEOF					0x1C
#define	IL_PREFIX_OP_REFANYTYPE				0x1D

#define	IL_PREFIX_OP_READONLY				0x1E
#define	IL_PREFIX_OP_UNUSED_PREFIX_1F		0x1F
#define	IL_PREFIX_OP_UNUSED_PREFIX_20		0x20
#define	IL_PREFIX_OP_UNUSED_PREFIX_21		0x21

#define	IL_PREFIX_OP_ANN_DATA				0x22
#define	IL_PREFIX_OP_ANN_ARG				0x23		/* !! - not defined */

/*
 * Valid values for tne IL_PREFIX_OP_NO opcode.
 * They may be ored together.
 */
#define IL_PREFIX_OP_NO_TYPECHECK			0x01
#define IL_PREFIX_OP_NO_RANGECHECK			0x02
#define IL_PREFIX_OP_NO_NULLCHECK			0x04

/*
 * Information about an opcode.
 */
typedef struct
{
	const char *name;			/* Name of the opcode */
	char		popped;			/* Number of values popped from the stack */
	char		pushed;			/* Number of values pushed on the stack */
	char		args;			/* Type of arguments to the opcode */
	char		size;			/* Size of the instruction */

} ILOpcodeInfo;

/*
 * Small information about an opcode.
 */
typedef struct
{
	char		popped;			/* Number of values popped from the stack */
	char		pushed;			/* Number of values pushed on the stack */
	char		args;			/* Type of arguments to the opcode */
	char		size;			/* Size of the instruction */

} ILOpcodeSmallInfo;

/*
 * Opcode argument types for "ILOpcodeInfo::args".
 */
#define	IL_OPCODE_ARGS_INVALID		0
#define	IL_OPCODE_ARGS_NONE			1
#define	IL_OPCODE_ARGS_INT8			2
#define	IL_OPCODE_ARGS_UINT8		3
#define	IL_OPCODE_ARGS_INT16		4
#define	IL_OPCODE_ARGS_UINT16		5
#define	IL_OPCODE_ARGS_INT32		6
#define	IL_OPCODE_ARGS_INT64		7
#define	IL_OPCODE_ARGS_FLOAT32		8
#define	IL_OPCODE_ARGS_FLOAT64		9
#define	IL_OPCODE_ARGS_TOKEN		10
#define	IL_OPCODE_ARGS_SHORT_VAR	11
#define	IL_OPCODE_ARGS_LONG_VAR		12
#define	IL_OPCODE_ARGS_SHORT_ARG	13
#define	IL_OPCODE_ARGS_LONG_ARG		14
#define	IL_OPCODE_ARGS_SHORT_JUMP	15
#define	IL_OPCODE_ARGS_LONG_JUMP	16
#define	IL_OPCODE_ARGS_CALL			17
#define	IL_OPCODE_ARGS_CALLI		18
#define	IL_OPCODE_ARGS_CALLVIRT		19
#define	IL_OPCODE_ARGS_SWITCH		20
#define	IL_OPCODE_ARGS_STRING		21
#define	IL_OPCODE_ARGS_NEW			22
#define	IL_OPCODE_ARGS_ANN_DATA		23
#define	IL_OPCODE_ARGS_ANN_DEAD		24
#define	IL_OPCODE_ARGS_ANN_REF		25
#define	IL_OPCODE_ARGS_ANN_PHI		26
#define	IL_OPCODE_ARGS_ANN_LIVE		27
#define	IL_OPCODE_ARGS_ANN_ARG		28
#define	IL_OPCODE_ARGS_LDTOKEN		29

/*
 * The opcode information tables.
 */
extern ILOpcodeInfo const ILMainOpcodeTable[];
extern ILOpcodeInfo const ILPrefixOpcodeTable[];
extern ILOpcodeSmallInfo const ILMainOpcodeSmallTable[];
extern ILOpcodeSmallInfo const ILPrefixOpcodeSmallTable[];

#endif	/* _IL_OPCODES_H */
