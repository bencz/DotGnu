/*
 * cvm_branch.c - Opcodes for branching within a method.
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
 * <opcode name="br" group="Branch instructions">
 *   <operation>Branch unconditionally</operation>
 *
 *   <format>br<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>br
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{br}<fsep/>dest</dformat>
 *
 *   <form name="br" code="COP_BR"/>
 *
 *   <before>...</before>
 *   <after>...</after>
 *
 *   <description>In the first form, the program branches to the
 *   address <i>pc + offset</i>, where <i>pc</i> is the address of
 *   the first byte of the <i>br</i> instruction, and <i>offset</i>
 *   is a signed 8-bit quantity.<p/>
 *
 *   In the second form, <i>offset</i> is constructed by interpreting
 *   <i>offset1</i>, ..., <i>offset4</i> as a 32-bit signed quantity
 *   in little-endian order.</description>
 *
 *   <notes>Branch instructions are always 6 bytes long, but may
 *   contain either short or long forms of a branch.  The short form
 *   is always "<i>opcode offset pad</i>" where <i>offset</i> is an
 *   8-bit offset and <i>pad</i> is 4 bytes of padding.  The long form
 *   is always "<i>br.long opcode loffset</i>" where <i>loffset</i>
 *   is a 32-bit offset.  This allows the code generator to output
 *   branch instructions with a uniform length, while the interpreter
 *   runs faster on short branches.</notes>
 * </opcode>
 */
VMCASE(COP_BR):
{
	/* Unconditional branch */
	pc = CVM_ARG_BRANCH_SHORT;
}
VMBREAK(COP_BR);

/**
 * <opcode name="beq" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> values
 *   are equal</operation>
 *
 *   <format>beq<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>beq
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{beq}<fsep/>dest</dformat>
 *
 *   <form name="beq" code="COP_BEQ"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>int32</code>.  If the values are equal,
 *   then the program branches to <i>pc + offset</i>.  Otherwise,
 *   the program continues with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <i>br_peq</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BEQ):
{
	/* Branch if the top two integers are equal */
	if(stacktop[-2].intValue == stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BEQ);

/**
 * <opcode name="bne" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> values
 *   are not equal</operation>
 *
 *   <format>bne<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>bne
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{bne}<fsep/>dest</dformat>
 *
 *   <form name="bne" code="COP_BNE"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>int32</code>.  If the values are not equal,
 *   then the program branches to <i>pc + offset</i>.  Otherwise,
 *   the program continues with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <i>br_pne</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BNE):
{
	/* Branch if the top two integers are not equal */
	if(stacktop[-2].intValue != stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BNE);

/**
 * <opcode name="blt" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> values
 *   are less than</operation>
 *
 *   <format>blt<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>blt
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{blt}<fsep/>dest</dformat>
 *
 *   <form name="blt" code="COP_BLT"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>int32</code>.  If <i>value1</i> is less
 *   than <i>value2</i>, then the program branches to <i>pc + offset</i>.
 *   Otherwise, the program continues with the next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BLT):
{
	/* Branch if the top two integers are less than */
	if(stacktop[-2].intValue < stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BLT);

/**
 * <opcode name="blt_un" group="Branch instructions">
 *   <operation>Branch conditionally if <code>uint32</code> values
 *   are less than</operation>
 *
 *   <format>blt_un<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>blt_un
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{blt_un}<fsep/>dest</dformat>
 *
 *   <form name="blt_un" code="COP_BLT_UN"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <i>uint32</i>.  If <i>value1</i> is less
 *   than <i>value2</i>, then the program branches to <i>pc + offset</i>.
 *   Otherwise, the program continues with the next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BLT_UN):
{
	/* Branch if the top two unsigned integers are less than */
	if(stacktop[-2].uintValue < stacktop[-1].uintValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BLT_UN);

/**
 * <opcode name="ble" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> values
 *   are less than or equal</operation>
 *
 *   <format>ble<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>ble
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{ble}<fsep/>dest</dformat>
 *
 *   <form name="ble" code="COP_BLE"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>int32</code>.  If <i>value1</i> is less
 *   than or equal to <i>value2</i>, then the program branches to
 *   <i>pc + offset</i>.  Otherwise, the program continues with the
 *   next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BLE):
{
	/* Branch if the top two integers are less than or equal */
	if(stacktop[-2].intValue <= stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BLE);

/**
 * <opcode name="ble_un" group="Branch instructions">
 *   <operation>Branch conditionally if <code>uint32</code> values
 *   are less than or equal</operation>
 *
 *   <format>ble_un<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>ble_un
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{ble_un}<fsep/>dest</dformat>
 *
 *   <form name="ble_un" code="COP_BLE_UN"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <i>uint32</i>.  If <i>value1</i> is less
 *   than or equal to <i>value2</i>, then the program branches to
 *   <i>pc + offset</i>.  Otherwise, the program continues with the
 *   next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BLE_UN):
{
	/* Branch if the top two unsigned integers
	   are less than or equal */
	if(stacktop[-2].uintValue <= stacktop[-1].uintValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BLE_UN);

/**
 * <opcode name="bgt" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> values
 *   are greater than</operation>
 *
 *   <format>bgt<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>bgt
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{bgt}<fsep/>dest</dformat>
 *
 *   <form name="bgt" code="COP_BGT"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>int32</code>.  If <i>value1</i> is greater
 *   than <i>value2</i>, then the program branches to <i>pc + offset</i>.
 *   Otherwise, the program continues with the next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BGT):
{
	/* Branch if the top two integers are greater than */
	if(stacktop[-2].intValue > stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BGT);

/**
 * <opcode name="bgt_un" group="Branch instructions">
 *   <operation>Branch conditionally if <code>uint32</code> values
 *   are greater than</operation>
 *
 *   <format>bgt_un<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>bgt_un
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{bgt_un}<fsep/>dest</dformat>
 *
 *   <form name="bgt_un" code="COP_BGT_UN"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <i>uint32</i>.  If <i>value1</i> is greater
 *   than <i>value2</i>, then the program branches to <i>pc + offset</i>.
 *   Otherwise, the program continues with the next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BGT_UN):
{
	/* Branch if the top two unsigned integers are greater than */
	if(stacktop[-2].uintValue > stacktop[-1].uintValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BGT_UN);

/**
 * <opcode name="bge" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> values
 *   are greater than or equal</operation>
 *
 *   <format>bge<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>bge
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{bge}<fsep/>dest</dformat>
 *
 *   <form name="bge" code="COP_BGE"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>int32</code>.  If <i>value1</i> is greater
 *   than or equal to <i>value2</i>, then the program branches to
 *   <i>pc + offset</i>.  Otherwise, the program continues with the
 *   next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BGE):
{
	/* Branch if the top two integers are greater than or equal */
	if(stacktop[-2].intValue >= stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BGE);

/**
 * <opcode name="bge_un" group="Branch instructions">
 *   <operation>Branch conditionally if <code>uint32</code> values
 *   are greater than or equal</operation>
 *
 *   <format>bge_un<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>bge_un
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{bge_un}<fsep/>dest</dformat>
 *
 *   <form name="bge_un" code="COP_BGE_UN"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <i>uint32</i>.  If <i>value1</i> is greater
 *   than or equal to <i>value2</i>, then the program branches to
 *   <i>pc + offset</i>.  Otherwise, the program continues with the
 *   next instruction.</description>
 * </opcode>
 */
VMCASE(COP_BGE_UN):
{
	/* Branch if the top two unsigned integers
	   are greater than or equal */
	if(stacktop[-2].uintValue >= stacktop[-1].uintValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BGE_UN);

/**
 * <opcode name="brtrue" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> value
 *   is non-zero</operation>
 *
 *   <format>brtrue<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>brtrue
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{brtrue}<fsep/>dest</dformat>
 *
 *   <form name="brtrue" code="COP_BRTRUE"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>The <i>value</i> is popped from the stack as type
 *   <code>int32</code>.  If <i>value</i> is non-zero, then the program
 *   branches to <i>pc + offset</i>.  Otherwise, the program continues
 *   with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <code>brnonnull</code>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BRTRUE):
{
	/* Branch if the top value is non-zero */
	if(stacktop[-1].intValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 1;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
	}
}
VMBREAK(COP_BRTRUE);

/**
 * <opcode name="brfalse" group="Branch instructions">
 *   <operation>Branch conditionally if <code>int32</code> value
 *   is zero</operation>
 *
 *   <format>brfalse<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>brfalse
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{brfalse}<fsep/>dest</dformat>
 *
 *   <form name="brfalse" code="COP_BRFALSE"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>The <i>value</i> is popped from the stack as type
 *   <code>int32</code>.  If <i>value</i> is zero, then the program branches
 *   to <i>pc + offset</i>.  Otherwise, the program continues with the
 *   next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <code>brnull</code>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BRFALSE):
{
	/* Branch if the top value is zero */
	if(!(stacktop[-1].intValue))
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 1;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
	}
}
VMBREAK(COP_BRFALSE);

/**
 * <opcode name="brnull" group="Branch instructions">
 *   <operation>Branch conditionally if <code>ptr</code> value
 *   is <code>null</code></operation>
 *
 *   <format>brnull<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>brnull
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{brnull}<fsep/>dest</dformat>
 *
 *   <form name="brnull" code="COP_BRNULL"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>The <i>value</i> is popped from the stack as type
 *   <i>ptr</i>.  If <i>value</i> is <code>null</code>, then the program
 *   branches to <i>pc + offset</i>.  Otherwise, the program continues
 *   with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <code>brfalse</code>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BRNULL):
{
	/* Branch if the top value is null */
	if(!(stacktop[-1].ptrValue))
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 1;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
	}
}
VMBREAK(COP_BRNULL);

/**
 * <opcode name="brnonnull" group="Branch instructions">
 *   <operation>Branch conditionally if <code>ptr</code> value
 *   is not <code>null</code></operation>
 *
 *   <format>brnonnull<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>brnonnull
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{brnonnull}<fsep/>dest</dformat>
 *
 *   <form name="brnonnull" code="COP_BRNONNULL"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>The <i>value</i> is popped from the stack as type
 *   <i>ptr</i>.  If <i>value</i> is not <code>null</code>, then the program
 *   branches to <i>pc + offset</i>.  Otherwise, the program continues
 *   with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <code>brtrue</code>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BRNONNULL):
{
	/* Branch if the top value is non-null */
	if(stacktop[-1].ptrValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 1;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
	}
}
VMBREAK(COP_BRNONNULL);

/**
 * <opcode name="br_peq" group="Branch instructions">
 *   <operation>Branch conditionally if <code>ptr</code> values
 *   are equal</operation>
 *
 *   <format>br_peq<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>br_peq
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{br_peq}<fsep/>dest</dformat>
 *
 *   <form name="br_peq" code="COP_BR_PEQ"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <i>ptr</i>.  If the values are equal,
 *   then the program branches to <i>pc + offset</i>.  Otherwise,
 *   the program continues with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <i>beq</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BR_PEQ):
{
	/* Branch if the top two pointers are equal */
	if(stacktop[-2].ptrValue == stacktop[-1].ptrValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BR_PEQ);

/**
 * <opcode name="br_pne" group="Branch instructions">
 *   <operation>Branch conditionally if <code>ptr</code> values
 *   are not equal</operation>
 *
 *   <format>br_pne<fsep/>offset<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <format>br_long<fsep/>br_pne
 *       <fsep/>offset1<fsep/>offset2<fsep/>offset3<fsep/>offset4</format>
 *   <dformat>{br_pne}<fsep/>dest</dformat>
 *
 *   <form name="br_pne" code="COP_BR_PNE"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <i>ptr</i>.  If the values are not equal,
 *   then the program branches to <i>pc + offset</i>.  Otherwise,
 *   the program continues with the next instruction.</description>
 *
 *   <notes>This instruction must not be confused with <i>bne</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_BR_PNE):
{
	/* Branch if the top two pointers are not equal */
	if(stacktop[-2].ptrValue != stacktop[-1].ptrValue)
	{
		pc = CVM_ARG_BRANCH_SHORT;
		stacktop -= 2;
	}
	else
	{
		MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
	}
}
VMBREAK(COP_BR_PNE);

/**
 * <opcode name="br_long" group="Branch instructions">
 *   <operation>Modify a branch instruction to its long form</operation>
 *
 *   <format>br_long<fsep/>opcode<fsep/>...</format>
 *
 *   <form name="br_long" code="COP_BR_LONG"/>
 *
 *   <description>The <i>br_long</i> instruction modifies a branch
 *   instruction to take longer operands.</description>
 *
 *   <notes>The documentation for other branch instructions includes
 *   information on their long forms.<p/>
 *
 *   There is no direct format for this instruction, because <i>br_long</i>
 *   is not required for the direct encoding.</notes>
 * </opcode>
 */
#ifndef IL_CVM_DIRECT
VMCASE(COP_BR_LONG):
{
	/* Determine which type of long branch to use */
	switch(CVM_ARG_SUB_OPCODE)
	{
		case COP_BR:
		default:
		{
			/* Unconditional branch */
			pc = CVM_ARG_BRANCH_LONG;
		}
		VMBREAKNOEND;
		
		case COP_BEQ:
		{
			/* Branch if the top two integers are equal */
			if(stacktop[-2].intValue == stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BNE:
		{
			/* Branch if the top two integers are not equal */
			if(stacktop[-2].intValue != stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BLT:
		{
			/* Branch if the top two integers are less than */
			if(stacktop[-2].intValue < stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BLT_UN:
		{
			/* Branch if the top two unsigned integers
			   are less than */
			if(stacktop[-2].uintValue < stacktop[-1].uintValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BLE:
		{
			/* Branch if the top two integers are
			   less than or equal */
			if(stacktop[-2].intValue <= stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BLE_UN:
		{
			/* Branch if the top two unsigned integers
			   are less than or equal */
			if(stacktop[-2].uintValue <= stacktop[-1].uintValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BGT:
		{
			/* Branch if the top two integers are greater than */
			if(stacktop[-2].intValue > stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BGT_UN:
		{
			/* Branch if the top two unsigned integers
			   are greater than */
			if(stacktop[-2].uintValue > stacktop[-1].uintValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BGE:
		{
			/* Branch if the top two integers are
			   greater than or equal */
			if(stacktop[-2].intValue >= stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BGE_UN:
		{
			/* Branch if the top two unsigned integers
			   are greater than or equal */
			if(stacktop[-2].uintValue >= stacktop[-1].uintValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BRTRUE:
		{
			/* Branch if the top value is non-zero */
			if(stacktop[-1].intValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 1;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
			}
		}
		VMBREAKNOEND;
		
		case COP_BRFALSE:
		{
			/* Branch if the top value is zero */
			if(!(stacktop[-1].intValue))
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 1;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
			}
		}
		VMBREAKNOEND;
		
		case COP_BRNULL:
		{
			/* Branch if the top value is null */
			if(!(stacktop[-1].ptrValue))
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 1;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
			}
		}
		VMBREAKNOEND;
		
		case COP_BRNONNULL:
		{
			/* Branch if the top value is non-null */
			if(stacktop[-1].ptrValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 1;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -1);
			}
		}
		VMBREAKNOEND;

		case COP_BR_PEQ:
		{
			/* Branch if the top two pointers are equal */
			if(stacktop[-2].ptrValue == stacktop[-1].ptrValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;
		
		case COP_BR_PNE:
		{
			/* Branch if the top two pointers are not equal */
			if(stacktop[-2].ptrValue != stacktop[-1].ptrValue)
			{
				pc = CVM_ARG_BRANCH_LONG;
				stacktop -= 2;
			}
			else
			{
				MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, -2);
			}
		}
		VMBREAKNOEND;

		case COP_JSR:
		{
			/* Long form of the "jsr" opcode */
			stacktop[0].ptrValue = (void *)CVM_ARG_JSR_RETURN;
			pc = CVM_ARG_BRANCH_LONG;
			stacktop += 1;
		}
		VMBREAKNOEND;
	}
}
VMBREAK(COP_BR_LONG);
#else /* IL_CVM_DIRECT */
VMCASE(COP_BR_LONG):
{
	/* We don't need "br_long" in direct mode, so just stub it out */
	MODIFY_PC_AND_STACK(CVM_LEN_BRANCH, 0);
}
VMBREAK(COP_BR_LONG);
#endif /* IL_CVM_DIRECT */

/**
 * <opcode name="switch" group="Branch instructions">
 *   <operation>Switch on <code>uint32</code> value</operation>
 *
 *   <format>switch<fsep/>max<fsep/>defoffset<fsep/>...</format>
 *   <dformat>{switch}<fsep/>max<fsep/>defaddr<fsep/>...</dformat>
 *
 *   <form name="switch" code="COP_SWITCH"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  If it is greater than or equal to
 *   <i>max</i>, then the program continues at <i>pc + defoffset</i>.
 *   Otherwise, the 32-bit signed value at <i>pc + 9 + value * 4</i>
 *   is fetched and added to <i>pc</i>.</description>
 *
 *   <notes>The <i>max</i> value is an unsigned 32-bit value, and
 *   the <i>defoffset</i> is a signed 32-bit value.  All 32-bit
 *   values are in little-endian byte order.</notes>
 * </opcode>
 */
VMCASE(COP_SWITCH):
{
	/* Process a switch statement */
	if(stacktop[-1].uintValue < CVM_ARG_SWITCH_LIMIT)
	{
		/* Jump to a specific case */
		pc = CVM_ARG_SWITCH_DEST(stacktop[-1].uintValue);
		--stacktop;
	}
	else
	{
		/* Jump to the default case */
		pc = CVM_ARG_SWITCH_DEFAULT;
		--stacktop;
	}
}
VMBREAK(COP_SWITCH);

#endif /* IL_CVM_MAIN */
