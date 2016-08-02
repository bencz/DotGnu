/*
 * cvm_const.c - Opcodes for loading constants onto the stack.
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

#if defined(IL_CVM_GLOBALS)

/* No globals required */

#elif defined(IL_CVM_LOCALS)

/* No locals required */

#elif defined(IL_CVM_MAIN)

/**
 * <opcode name="ldnull" group="Constant loading">
 *   <operation>Load <code>null</code> onto the stack</operation>
 *
 *   <format>ldnull</format>
 *   <dformat>{ldnull}</dformat>
 *
 *   <form name="ldnull" code="COP_LDNULL"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The <i>value</i> <code>null</code> is pushed onto
 *   the stack as type <code>ptr</code>.</description>
 *
 *   <notes>This instruction must not be confused with <i>ldc_i4_0</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_LDNULL):
{
	/* Load the "null" pointer value to the stack top */
	stacktop[0].ptrValue = 0;
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1);
}
VMBREAK(COP_LDNULL);

#define	COP_LDC_I4_VALUE(name,value)	\
VMCASE(COP_LDC_I4_##name): \
{ \
	stacktop[0].intValue = (ILInt32)(value); \
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1); \
} \
VMBREAK(COP_LDC_I4_##name) \

/**
 * <opcode name="ldc_i4_&lt;n&gt;" group="Constant loading">
 *   <operation>Load <i>n</i> onto the stack</operation>
 *
 *   <format>ldc_i4_&lt;n&gt;</format>
 *   <dformat>{ldc_i4_&lt;n&gt;}</dformat>
 *
 *   <form name="ldc_i4_m1" code="COP_LDC_I4_M1"/>
 *   <form name="ldc_i4_0" code="COP_LDC_I4_0"/>
 *   <form name="ldc_i4_1" code="COP_LDC_I4_1"/>
 *   <form name="ldc_i4_2" code="COP_LDC_I4_2"/>
 *   <form name="ldc_i4_3" code="COP_LDC_I4_3"/>
 *   <form name="ldc_i4_4" code="COP_LDC_I4_4"/>
 *   <form name="ldc_i4_5" code="COP_LDC_I4_5"/>
 *   <form name="ldc_i4_6" code="COP_LDC_I4_6"/>
 *   <form name="ldc_i4_7" code="COP_LDC_I4_7"/>
 *   <form name="ldc_i4_8" code="COP_LDC_I4_8"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The <i>value</i> <i>n</i> is pushed onto
 *   the stack as type <code>int32</code>.</description>
 *
 *   <notes>These instructions can also be used to load constants
 *   of type <code>uint32</code> onto the stack.</notes>
 * </opcode>
 */
/* Load simple integer constants onto the stack */
COP_LDC_I4_VALUE(M1, -1);
COP_LDC_I4_VALUE(0, 0);
COP_LDC_I4_VALUE(1, 1);
COP_LDC_I4_VALUE(2, 2);
COP_LDC_I4_VALUE(3, 3);
COP_LDC_I4_VALUE(4, 4);
COP_LDC_I4_VALUE(5, 5);
COP_LDC_I4_VALUE(6, 6);
COP_LDC_I4_VALUE(7, 7);
COP_LDC_I4_VALUE(8, 8);

/**
 * <opcode name="ldc_i4_s" group="Constant loading">
 *   <operation>Load small <code>int32</code> constant
 *				onto the stack</operation>
 *
 *   <format>ldc_i4_s<fsep/>n</format>
 *   <dformat>{ldc_i4_s}<fsep/>n</dformat>
 *
 *   <form name="ldc_i4_s" code="COP_LDC_I4_S"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The signed 8-bit <i>value</i> <i>n</i>
 *   is pushed onto the stack as type <code>int32</code>.</description>
 *
 *   <notes>This instruction can also be used to load constants
 *   of type <code>uint32</code> onto the stack.</notes>
 * </opcode>
 */
VMCASE(COP_LDC_I4_S):
{
	/* Load an 8-bit integer constant onto the stack */
	stacktop[0].intValue = CVM_ARG_SBYTE;
	MODIFY_PC_AND_STACK(CVM_LEN_BYTE, 1);
}
VMBREAK(COP_LDC_I4_S);

/**
 * <opcode name="ldc_i4" group="Constant loading">
 *   <operation>Load <code>int32</code> constant onto the stack</operation>
 *
 *   <format>ldc_i4<fsep/>n[4]</format>
 *   <dformat>{ldc_i4}<fsep/>n</dformat>
 *
 *   <form name="ldc_i4" code="COP_LDC_I4"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The 32-bit <i>value</i> <i>n</i>
 *   is pushed onto the stack as type <code>int32</code>.</description>
 *
 *   <notes>This instruction can also be used to load constants
 *   of type <code>uint32</code> onto the stack.</notes>
 * </opcode>
 */
VMCASE(COP_LDC_I4):
{
	/* Load a 32-bit integer constant onto the stack */
	stacktop[0].intValue = (ILInt32)CVM_ARG_WORD;
	MODIFY_PC_AND_STACK(CVM_LEN_WORD, 1);
}
VMBREAK(COP_LDC_I4);

/**
 * <opcode name="ldc_i8" group="Constant loading">
 *   <operation>Load <code>int64</code> constant onto the stack</operation>
 *
 *   <format>ldc_i8<fsep/>n[8]</format>
 *   <dformat>{ldc_i8}<fsep/>n[8]</dformat>
 *
 *   <form name="ldc_i8" code="COP_LDC_I8"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The 64-bit <i>value</i> <i>n</i>
 *   is pushed onto the stack as type <code>int64</code>.</description>
 *
 *   <notes>This instruction can also be used to load constants
 *   of type <code>uint64</code> onto the stack.</notes>
 * </opcode>
 */
VMCASE(COP_LDC_I8):
{
	/* Load a 64-bit integer constant onto the stack */
	WriteLong(&(stacktop[0]), CVM_ARG_LONG);
	MODIFY_PC_AND_STACK(CVM_LEN_LONG, CVM_WORDS_PER_LONG);
}
VMBREAK(COP_LDC_I8);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="ldc_r4" group="Constant loading">
 *   <operation>Load 32-bit floating point constant onto the stack</operation>
 *
 *   <format>ldc_r4<fsep/>n[4]</format>
 *   <dformat>{ldc_r4}<fsep/>n[4]</dformat>
 *
 *   <form name="ldc_r4" code="COP_LDC_R4"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The 32-bit floating point value <i>n</i> is fetched,
 *   converted to <code>native float</code>, and then pushed onto the
 *   stack as <i>value</i>.</description>
 * </opcode>
 */
VMCASE(COP_LDC_R4):
{
	/* Load a 32-bit floating point value onto the stack */
	WriteFloat(&(stacktop[0]), (ILNativeFloat)CVM_ARG_FLOAT);
	MODIFY_PC_AND_STACK(CVM_LEN_FLOAT, CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_LDC_R4);

/**
 * <opcode name="ldc_r8" group="Constant loading">
 *   <operation>Load 64-bit floating point constant onto the stack</operation>
 *
 *   <format>ldc_r8<fsep/>n[8]</format>
 *
 *   <form name="ldc_r8" code="COP_LDC_R8"/>
 *   <dformat>{ldc_r8}<fsep/>n[8]</dformat>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>The 64-bit floating point value <i>n</i> is fetched,
 *   converted to <code>native float</code>, and then pushed onto the
 *   stack as <i>value</i>.</description>
 * </opcode>
 */
VMCASE(COP_LDC_R8):
{
	/* Load a 64-bit floating point value onto the stack */
	WriteFloat(&(stacktop[0]), (ILNativeFloat)CVM_ARG_DOUBLE);
	MODIFY_PC_AND_STACK(CVM_LEN_DOUBLE, CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_LDC_R8);

#endif /* !IL_CONFIG_FP_SUPPORTED */

#endif /* IL_CVM_MAIN */
