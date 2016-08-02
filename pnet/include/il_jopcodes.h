/*
 * il_jopcodes.h - Opcodes for the JVM instruction set.
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

#ifndef	_IL_JOPCODES_H
#define	_IL_JOPCODES_H

/* No operation */
#define	JAVA_OP_NOP							0x00

/* Constants */
#define	JAVA_OP_ACONST_NULL					0x01
#define	JAVA_OP_ICONST_M1					0x02
#define	JAVA_OP_ICONST_0					0x03
#define	JAVA_OP_ICONST_1					0x04
#define	JAVA_OP_ICONST_2					0x05
#define	JAVA_OP_ICONST_3					0x06
#define	JAVA_OP_ICONST_4					0x07
#define	JAVA_OP_ICONST_5					0x08
#define	JAVA_OP_LCONST_0					0x09
#define	JAVA_OP_LCONST_1					0x0A
#define	JAVA_OP_FCONST_0					0x0B
#define	JAVA_OP_FCONST_1					0x0C
#define	JAVA_OP_FCONST_2					0x0D
#define	JAVA_OP_DCONST_0					0x0E
#define	JAVA_OP_DCONST_1					0x0F
#define	JAVA_OP_BIPUSH						0x10
#define	JAVA_OP_SIPUSH						0x11
#define	JAVA_OP_LDC							0x12
#define	JAVA_OP_LDC_W						0x13
#define	JAVA_OP_LDC2_W						0x14

/* Local variable loads */
#define	JAVA_OP_ILOAD						0x15
#define	JAVA_OP_LLOAD						0x16
#define	JAVA_OP_FLOAD						0x17
#define	JAVA_OP_DLOAD						0x18
#define	JAVA_OP_ALOAD						0x19
#define	JAVA_OP_ILOAD_0						0x1A
#define	JAVA_OP_ILOAD_1						0x1B
#define	JAVA_OP_ILOAD_2						0x1C
#define	JAVA_OP_ILOAD_3						0x1D
#define	JAVA_OP_LLOAD_0						0x1E
#define	JAVA_OP_LLOAD_1						0x1F
#define	JAVA_OP_LLOAD_2						0x20
#define	JAVA_OP_LLOAD_3						0x21
#define	JAVA_OP_FLOAD_0						0x22
#define	JAVA_OP_FLOAD_1						0x23
#define	JAVA_OP_FLOAD_2						0x24
#define	JAVA_OP_FLOAD_3						0x25
#define	JAVA_OP_DLOAD_0						0x26
#define	JAVA_OP_DLOAD_1						0x27
#define	JAVA_OP_DLOAD_2						0x28
#define	JAVA_OP_DLOAD_3						0x29
#define	JAVA_OP_ALOAD_0						0x2A
#define	JAVA_OP_ALOAD_1						0x2B
#define	JAVA_OP_ALOAD_2						0x2C
#define	JAVA_OP_ALOAD_3						0x2D

/* Array loads */
#define	JAVA_OP_IALOAD						0x2E
#define	JAVA_OP_LALOAD						0x2F
#define	JAVA_OP_FALOAD						0x30
#define	JAVA_OP_DALOAD						0x31
#define	JAVA_OP_AALOAD						0x32
#define	JAVA_OP_BALOAD						0x33
#define	JAVA_OP_CALOAD						0x34
#define	JAVA_OP_SALOAD						0x35

/* Local variable stores */
#define	JAVA_OP_ISTORE						0x36
#define	JAVA_OP_LSTORE						0x37
#define	JAVA_OP_FSTORE						0x38
#define	JAVA_OP_DSTORE						0x39
#define	JAVA_OP_ASTORE						0x3A
#define	JAVA_OP_ISTORE_0					0x3B
#define	JAVA_OP_ISTORE_1					0x3C
#define	JAVA_OP_ISTORE_2					0x3D
#define	JAVA_OP_ISTORE_3					0x3E
#define	JAVA_OP_LSTORE_0					0x3F
#define	JAVA_OP_LSTORE_1					0x40
#define	JAVA_OP_LSTORE_2					0x41
#define	JAVA_OP_LSTORE_3					0x42
#define	JAVA_OP_FSTORE_0					0x43
#define	JAVA_OP_FSTORE_1					0x44
#define	JAVA_OP_FSTORE_2					0x45
#define	JAVA_OP_FSTORE_3					0x46
#define	JAVA_OP_DSTORE_0					0x47
#define	JAVA_OP_DSTORE_1					0x48
#define	JAVA_OP_DSTORE_2					0x49
#define	JAVA_OP_DSTORE_3					0x4A
#define	JAVA_OP_ASTORE_0					0x4B
#define	JAVA_OP_ASTORE_1					0x4C
#define	JAVA_OP_ASTORE_2					0x4D
#define	JAVA_OP_ASTORE_3					0x4E

/* Array stores */
#define	JAVA_OP_IASTORE						0x4F
#define	JAVA_OP_LASTORE						0x50
#define	JAVA_OP_FASTORE						0x51
#define	JAVA_OP_DASTORE						0x52
#define	JAVA_OP_AASTORE						0x53
#define	JAVA_OP_BASTORE						0x54
#define	JAVA_OP_CASTORE						0x55
#define	JAVA_OP_SASTORE						0x56

/* Stack manipulation */
#define	JAVA_OP_POP							0x57
#define	JAVA_OP_POP2						0x58
#define	JAVA_OP_DUP							0x59
#define	JAVA_OP_DUP_X1						0x5A
#define	JAVA_OP_DUP_X2						0x5B
#define	JAVA_OP_DUP2						0x5C
#define	JAVA_OP_DUP2_X1						0x5D
#define	JAVA_OP_DUP2_X2						0x5E
#define	JAVA_OP_SWAP						0x5F

/* Arithmetic operations */
#define	JAVA_OP_IADD						0x60
#define	JAVA_OP_LADD						0x61
#define	JAVA_OP_FADD						0x62
#define	JAVA_OP_DADD						0x63
#define	JAVA_OP_ISUB						0x64
#define	JAVA_OP_LSUB						0x65
#define	JAVA_OP_FSUB						0x66
#define	JAVA_OP_DSUB						0x67
#define	JAVA_OP_IMUL						0x68
#define	JAVA_OP_LMUL						0x69
#define	JAVA_OP_FMUL						0x6A
#define	JAVA_OP_DMUL						0x6B
#define	JAVA_OP_IDIV						0x6C
#define	JAVA_OP_LDIV						0x6D
#define	JAVA_OP_FDIV						0x6E
#define	JAVA_OP_DDIV						0x6F
#define	JAVA_OP_IREM						0x70
#define	JAVA_OP_LREM						0x71
#define	JAVA_OP_FREM						0x72
#define	JAVA_OP_DREM						0x73
#define	JAVA_OP_INEG						0x74
#define	JAVA_OP_LNEG						0x75
#define	JAVA_OP_FNEG						0x76
#define	JAVA_OP_DNEG						0x77
#define	JAVA_OP_ISHL						0x78
#define	JAVA_OP_LSHL						0x79
#define	JAVA_OP_ISHR						0x7A
#define	JAVA_OP_LSHR						0x7B
#define	JAVA_OP_IUSHR						0x7C
#define	JAVA_OP_LUSHR						0x7D
#define	JAVA_OP_IAND						0x7E
#define	JAVA_OP_LAND						0x7F
#define	JAVA_OP_IOR							0x80
#define	JAVA_OP_LOR							0x81
#define	JAVA_OP_IXOR						0x82
#define	JAVA_OP_LXOR						0x83

/* Increment */
#define	JAVA_OP_IINC						0x84

/* Conversion */
#define	JAVA_OP_I2L							0x85
#define	JAVA_OP_I2F							0x86
#define	JAVA_OP_I2D							0x87
#define	JAVA_OP_L2I							0x88
#define	JAVA_OP_L2F							0x89
#define	JAVA_OP_L2D							0x8A
#define	JAVA_OP_F2I							0x8B
#define	JAVA_OP_F2L							0x8C
#define	JAVA_OP_F2D							0x8D
#define	JAVA_OP_D2I							0x8E
#define	JAVA_OP_D2L							0x8F
#define	JAVA_OP_D2F							0x90
#define	JAVA_OP_I2B							0x91
#define	JAVA_OP_I2C							0x92
#define	JAVA_OP_I2S							0x93

/* Comparison */
#define	JAVA_OP_LCMP						0x94
#define	JAVA_OP_FCMPL						0x95
#define	JAVA_OP_FCMPG						0x96
#define	JAVA_OP_DCMPL						0x97
#define	JAVA_OP_DCMPG						0x98

/* Branching */
#define	JAVA_OP_IFEQ						0x99
#define	JAVA_OP_IFNE						0x9A
#define	JAVA_OP_IFLT						0x9B
#define	JAVA_OP_IFGE						0x9C
#define	JAVA_OP_IFGT						0x9D
#define	JAVA_OP_IFLE						0x9E
#define	JAVA_OP_IF_ICMPEQ					0x9F
#define	JAVA_OP_IF_ICMPNE					0xA0
#define	JAVA_OP_IF_ICMPLT					0xA1
#define	JAVA_OP_IF_ICMPGE					0xA2
#define	JAVA_OP_IF_ICMPGT					0xA3
#define	JAVA_OP_IF_ICMPLE					0xA4
#define	JAVA_OP_IF_ACMPEQ					0xA5
#define	JAVA_OP_IF_ACMPNE					0xA6
#define	JAVA_OP_GOTO						0xA7
#define	JAVA_OP_JSR							0xA8
#define	JAVA_OP_RET							0xA9

/* Switching */
#define	JAVA_OP_TABLESWITCH					0xAA
#define	JAVA_OP_LOOKUPSWITCH				0xAB

/* Method return */
#define	JAVA_OP_IRETURN						0xAC
#define	JAVA_OP_LRETURN						0xAD
#define	JAVA_OP_FRETURN						0xAE
#define	JAVA_OP_DRETURN						0xAF
#define	JAVA_OP_ARETURN						0xB0
#define	JAVA_OP_RETURN						0xB1

/* Field access */
#define	JAVA_OP_GETSTATIC					0xB2
#define	JAVA_OP_PUTSTATIC					0xB3
#define	JAVA_OP_GETFIELD					0xB4
#define	JAVA_OP_PUTFIELD					0xB5

/* Method invocation */
#define	JAVA_OP_INVOKEVIRTUAL				0xB6
#define	JAVA_OP_INVOKESPECIAL				0xB7
#define	JAVA_OP_INVOKESTATIC				0xB8
#define	JAVA_OP_INVOKEINTERFACE				0xB9
#define	JAVA_OP_UNUSED						0xBA

/* Memory allocation */
#define	JAVA_OP_NEW							0xBB
#define	JAVA_OP_NEWARRAY					0xBC
#define	JAVA_OP_ANEWARRAY					0xBD

/* Misc object handling */
#define	JAVA_OP_ARRAYLENGTH					0xBE
#define	JAVA_OP_ATHROW						0xBF
#define	JAVA_OP_CHECKCAST					0xC0
#define	JAVA_OP_INSTANCEOF					0xC1
#define	JAVA_OP_MONITORENTER				0xC2
#define	JAVA_OP_MONITOREXIT					0xC3

/* Prefixes */
#define	JAVA_OP_WIDE						0xC4

/* Memory allocation */
#define	JAVA_OP_MULTIANEWARRAY				0xC5

/* Branching */
#define	JAVA_OP_IFNULL						0xC6
#define	JAVA_OP_IFNONNULL					0xC7
#define	JAVA_OP_GOTO_W						0xC8
#define	JAVA_OP_JSR_W						0xC9

/* Reserved */
#define	JAVA_OP_BREAKPOINT					0xCA
#define	JAVA_OP_IMPDEP1						0xFE
#define	JAVA_OP_IMPDEP2						0xFF

/* Constant pool tag values */
#define	JAVA_CONST_UTF8						1
#define	JAVA_CONST_INTEGER					3
#define	JAVA_CONST_FLOAT					4
#define	JAVA_CONST_LONG						5
#define	JAVA_CONST_DOUBLE					6
#define	JAVA_CONST_CLASS					7
#define	JAVA_CONST_STRING					8
#define	JAVA_CONST_FIELDREF					9
#define	JAVA_CONST_METHODREF				10
#define	JAVA_CONST_INTERFACEMETHODREF		11
#define	JAVA_CONST_NAMEANDTYPE				12

/* Access flags */
#define	JAVA_ACC_PUBLIC						0x0001
#define	JAVA_ACC_PRIVATE					0x0002
#define	JAVA_ACC_PROTECTED					0x0004
#define	JAVA_ACC_STATIC						0x0008
#define	JAVA_ACC_FINAL						0x0010
#define	JAVA_ACC_SUPER						0x0020
#define	JAVA_ACC_SYNCHRONIZED				0x0020
#define	JAVA_ACC_VOLATILE					0x0040
#define	JAVA_ACC_TRANSIENT					0x0080
#define	JAVA_ACC_NATIVE						0x0100
#define	JAVA_ACC_INTERFACE					0x0200
#define	JAVA_ACC_ABSTRACT					0x0400
#define	JAVA_ACC_STRICT						0x0800

/* Element types for "newarray" */
#define	JAVA_ARRAY_OF_BOOL					4
#define	JAVA_ARRAY_OF_CHAR					5
#define	JAVA_ARRAY_OF_FLOAT					6
#define	JAVA_ARRAY_OF_DOUBLE				7
#define	JAVA_ARRAY_OF_BYTE					8
#define	JAVA_ARRAY_OF_SHORT					9
#define	JAVA_ARRAY_OF_INT					10
#define	JAVA_ARRAY_OF_LONG					11

#endif	/* _IL_JOPCODES_H */
