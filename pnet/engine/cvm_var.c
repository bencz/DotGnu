/*
 * cvm_var.c - Opcodes for accessing variables.
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

/*
 * Macros that help to profile which variables are most commonly used
 * during execution of applications, and their depth on the stack.
 */
#ifdef IL_PROFILE_CVM_VAR_USAGE

extern int _ILCVMVarLoadCounts[256];
extern int _ILCVMVarLoadDepths[256];
extern int _ILCVMVarStoreCounts[256];
extern int _ILCVMVarStoreDepths[256];

#define	CVM_VAR_LOADED(n)	\
			do { \
				++(_ILCVMVarLoadCounts[(n)]); \
				++(_ILCVMVarLoadDepths[stacktop - (frame + (n))]); \
			} while (0)

#define	CVM_VAR_STORED(n)	\
			do { \
				++(_ILCVMVarStoreCounts[(n)]); \
				++(_ILCVMVarStoreDepths[stacktop - (frame + (n))]); \
			} while (0)

#else /* !CVM_PROFILE_VAR_USAGE */

#define	CVM_VAR_LOADED(n)
#define	CVM_VAR_STORED(n)

#endif /* !CVM_PROFILE_VAR_USAGE */

#elif defined(IL_CVM_LOCALS)

ILUInt32 tempNum;

#elif defined(IL_CVM_MAIN)

#define	COP_LOAD_N(n)	\
VMCASE(COP_ILOAD_##n): \
{ \
	CVM_VAR_LOADED(n); \
	stacktop[0].intValue = frame[(n)].intValue; \
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1); \
} \
VMBREAK(COP_ILOAD_##n); \
VMCASE(COP_PLOAD_##n): \
{ \
	CVM_VAR_LOADED(n); \
	stacktop[0].ptrValue = frame[(n)].ptrValue; \
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1); \
} \
VMBREAK(COP_PLOAD_##n)

/**
 * <opcode name="iload_&lt;n&gt;" group="Local variable handling">
 *   <operation>Load <code>int32</code> variable <i>n</i>
 *              onto the stack</operation>
 *
 *   <format>iload_&lt;n&gt;</format>
 *   <dformat>{iload_&lt;n&gt;}</dformat>
 *
 *   <form name="iload_0" code="COP_ILOAD_0"/>
 *   <form name="iload_1" code="COP_ILOAD_1"/>
 *   <form name="iload_2" code="COP_ILOAD_2"/>
 *   <form name="iload_3" code="COP_ILOAD_3"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>Load the <code>int32</code> variable from position
 *   <i>n</i> in the local variable frame and push its <i>value</i>
 *   onto the stack.</description>
 *
 *   <notes>These instructions can also be used to load variables
 *   of type <code>uint32</code> onto the stack.</notes>
 * </opcode>
 */
/**
 * <opcode name="pload_&lt;n&gt;" group="Local variable handling">
 *   <operation>Load <code>ptr</code> variable <i>n</i>
 *              onto the stack</operation>
 *
 *   <format>pload_&lt;n&gt;</format>
 *   <dformat>{pload_&lt;n&gt;}</dformat>
 *
 *   <form name="pload_0" code="COP_PLOAD_0"/>
 *   <form name="pload_1" code="COP_PLOAD_1"/>
 *   <form name="pload_2" code="COP_PLOAD_2"/>
 *   <form name="pload_3" code="COP_PLOAD_3"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>Load the <code>ptr</code> variable from position
 *   <i>n</i> in the local variable frame and push its <i>value</i>
 *   onto the stack.</description>
 *
 *   <notes>These instructions must not be confused with the
 *   <i>iload_&lt;n&gt;</i> instructions.  Values of type
 *   <code>int32</code> and <code>ptr</code> do not necessarily
 *   occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
/* Load integer or pointer values from the frame onto the stack */
COP_LOAD_N(0);
COP_LOAD_N(1);
COP_LOAD_N(2);
COP_LOAD_N(3);

/**
 * <opcode name="iload" group="Local variable handling">
 *   <operation>Load <code>int32</code> variable
 *              onto the stack</operation>
 *
 *   <format>iload<fsep/>N[1]</format>
 *   <format>wide<fsep/>iload<fsep/>N[4]</format>
 *   <dformat>{iload}<fsep/>N</dformat>
 *
 *   <form name="iload" code="COP_ILOAD"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>Load the <code>int32</code> variable from position
 *   <i>N</i> in the local variable frame and push its <i>value</i>
 *   onto the stack.</description>
 *
 *   <notes>This instruction can also be used to load variables
 *   of type <code>uint32</code> onto the stack.</notes>
 * </opcode>
 */
VMCASE(COP_ILOAD):
{
	/* Load an integer value from the frame */
	CVM_VAR_LOADED(CVM_ARG_WIDE_SMALL);
	stacktop[0].intValue = frame[CVM_ARG_WIDE_SMALL].intValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 1);
}
VMBREAK(COP_ILOAD);

/**
 * <opcode name="pload" group="Local variable handling">
 *   <operation>Load <code>ptr</code> variable
 *              onto the stack</operation>
 *
 *   <format>pload<fsep/>N[1]</format>
 *   <format>wide<fsep/>pload<fsep/>N[4]</format>
 *   <dformat>{pload}<fsep/>N</dformat>
 *
 *   <form name="pload" code="COP_PLOAD"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>Load the <code>ptr</code> variable from position
 *   <i>N</i> in the local variable frame and push its <i>value</i>
 *   onto the stack.</description>
 *
 *   <notes>This instruction must not be confused with <i>iload</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_PLOAD):
{
	/* Load a pointer value from the frame */
	CVM_VAR_LOADED(CVM_ARG_WIDE_SMALL);
	stacktop[0].ptrValue = frame[CVM_ARG_WIDE_SMALL].ptrValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 1);
}
VMBREAK(COP_PLOAD);

#define	COP_STORE_N(n)	\
VMCASE(COP_ISTORE_##n): \
{ \
	CVM_VAR_STORED(n); \
	frame[(n)].intValue = stacktop[-1].intValue; \
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -1); \
} \
VMBREAK(COP_ISTORE_##n); \
VMCASE(COP_PSTORE_##n): \
{ \
	CVM_VAR_STORED(n); \
	frame[(n)].ptrValue = stacktop[-1].ptrValue; \
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -1); \
} \
VMBREAK(COP_PSTORE_##n)

/**
 * <opcode name="istore_&lt;n&gt;" group="Local variable handling">
 *   <operation>Store the top of stack into <code>int32</code>
 *              variable <i>n</i></operation>
 *
 *   <format>istore_&lt;n&gt;</format>
 *   <dformat>{istore_&lt;n&gt;}</dformat>
 *
 *   <form name="istore_0" code="COP_ISTORE_0"/>
 *   <form name="istore_1" code="COP_ISTORE_1"/>
 *   <form name="istore_2" code="COP_ISTORE_2"/>
 *   <form name="istore_3" code="COP_ISTORE_3"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>Pop <i>value</i> from the stack as type <code>int32</code>
 *   and store it at position <i>n</i> in the local variable frame.
 *   </description>
 *
 *   <notes>These instructions can also be used to store to variables
 *   of type <code>uint32</code>.</notes>
 * </opcode>
 */
/**
 * <opcode name="pstore_&lt;n&gt;" group="Local variable handling">
 *   <operation>Store the top of stack into <code>ptr</code>
 *              variable <i>n</i></operation>
 *
 *   <format>pstore_&lt;n&gt;</format>
 *   <dformat>{pstore_&lt;n&gt;}</dformat>
 *
 *   <form name="pstore_0" code="COP_PSTORE_0"/>
 *   <form name="pstore_1" code="COP_PSTORE_1"/>
 *   <form name="pstore_2" code="COP_PSTORE_2"/>
 *   <form name="pstore_3" code="COP_PSTORE_3"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>Pop <i>value</i> from the stack as type <code>ptr</code>
 *   and store it at position <i>n</i> in the local variable frame.
 *   </description>
 *
 *   <notes>These instructions must not be confused with the
 *   <i>istore_&lt;n&gt;</i> instructions.  Values of type
 *   <code>int32</code> and <code>ptr</code> do not necessarily
 *   occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
/* Store integer or pointer values from the stack to the frame */
COP_STORE_N(0);
COP_STORE_N(1);
COP_STORE_N(2);
COP_STORE_N(3);

/**
 * <opcode name="istore" group="Local variable handling">
 *   <operation>Store the top of stack into <code>int32</code>
 *              variable</operation>
 *
 *   <format>istore<fsep/>N[1]</format>
 *   <format>wide<fsep/>istore<fsep/>N[4]</format>
 *   <dformat>{istore}<fsep/>N</dformat>
 *
 *   <form name="istore" code="COP_ISTORE"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>Pop <i>value</i> from the stack as type <code>int32</code>
 *   and store it at position <i>N</i> in the local variable frame.
 *   </description>
 *
 *   <notes>This instruction can also be used to store to variables
 *   of type <code>uint32</code>.</notes>
 * </opcode>
 */
VMCASE(COP_ISTORE):
{
	/* Store an integer value to the frame */
	CVM_VAR_STORED(CVM_ARG_WIDE_SMALL);
	frame[CVM_ARG_WIDE_SMALL].intValue = stacktop[-1].intValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, -1);
}
VMBREAK(COP_ISTORE);

/**
 * <opcode name="pstore" group="Local variable handling">
 *   <operation>Store the top of stack into <code>ptr</code>
 *              variable</operation>
 *
 *   <format>pstore<fsep/>N[1]</format>
 *   <format>wide<fsep/>pstore<fsep/>N[4]</format>
 *   <dformat>{pstore}<fsep/>N</dformat>
 *
 *   <form name="pstore" code="COP_PSTORE"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>Pop <i>value</i> from the stack as type <code>ptr</code>
 *   and store it at position <i>N</i> in the local variable frame.
 *   </description>
 *
 *   <notes>This instructions must not be confused with <i>istore</i>.
 *   Values of type <code>int32</code> and <code>ptr</code> do not
 *   necessarily occupy the same amount of space in a stack word on
 *   all platforms.</notes>
 * </opcode>
 */
VMCASE(COP_PSTORE):
{
	/* Store a pointer value to the frame */
	CVM_VAR_STORED(CVM_ARG_WIDE_SMALL);
	frame[CVM_ARG_WIDE_SMALL].ptrValue = stacktop[-1].ptrValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, -1);
}
VMBREAK(COP_PSTORE);

/**
 * <opcode name="mload" group="Local variable handling">
 *   <operation>Load multiple stack words from a variable
 *              onto the stack</operation>
 *
 *   <format>mload<fsep/>N[1]<fsep/>M[1]</format>
 *   <format>wide<fsep/>mload<fsep/>N[4]<fsep/>M[4]</format>
 *   <dformat>{mload}<fsep/>N<fsep/>M</dformat>
 *
 *   <form name="mload" code="COP_MLOAD"/>
 *
 *   <before>...</before>
 *   <after>..., value1, ..., valueM</after>
 *
 *   <description>Load the <i>M</i> stack words from position
 *   <i>N</i> in the local variable frame and push them
 *   onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_MLOAD):
{
	/* Load a value consisting of multiple stack words */
	CVM_VAR_LOADED(CVM_ARG_DWIDE1_SMALL);
	IL_MEMCPY(stacktop, &(frame[CVM_ARG_DWIDE1_SMALL]),
	          sizeof(CVMWord) * CVM_ARG_DWIDE2_SMALL);
	MODIFY_PC_AND_STACK_REVERSE(CVM_LEN_DWIDE_SMALL, CVM_ARG_DWIDE2_SMALL);
}
VMBREAK(COP_MLOAD);

/**
 * <opcode name="mstore" group="Local variable handling">
 *   <operation>Store multiple stack words from the stack
 *              to a variable</operation>
 *
 *   <format>mstore<fsep/>N[1]<fsep/>M[1]</format>
 *   <format>wide<fsep/>mstore<fsep/>N[4]<fsep/>M[4]</format>
 *   <dformat>{mstore}<fsep/>N<fsep/>M</dformat>
 *
 *   <form name="mstore" code="COP_MSTORE"/>
 *
 *   <before>..., value1, ..., valueM</before>
 *   <after>...</after>
 *
 *   <description>Pop the <i>M</i> stack words from the top of
 *   the stack and store them at position <i>N</i> in the local
 *   variable frame.</description>
 * </opcode>
 */
VMCASE(COP_MSTORE):
{
	/* Store a value consisting of multiple stack words */
	CVM_VAR_STORED(CVM_ARG_DWIDE1_SMALL);
	stacktop -= CVM_ARG_DWIDE2_SMALL;
	IL_MEMCPY(&(frame[CVM_ARG_DWIDE1_SMALL]), stacktop,
			  sizeof(CVMWord) * CVM_ARG_DWIDE2_SMALL);
	MODIFY_PC_AND_STACK(CVM_LEN_DWIDE_SMALL, 0);
}
VMBREAK(COP_MSTORE);

/**
 * <opcode name="waddr" group="Local variable handling">
 *   <operation>Load the address of a variable onto the stack</operation>
 *
 *   <format>waddr<fsep/>N[1]</format>
 *   <format>wide<fsep/>waddr<fsep/>N[4]</format>
 *   <dformat>{waddr}<fsep/>N</dformat>
 *
 *   <form name="waddr" code="COP_WADDR"/>
 *
 *   <before>...</before>
 *   <after>..., pointer</after>
 *
 *   <description>Set <i>pointer</i> to the address of the word at
 *   position <i>N</i> in the local variable frame.  Push <i>pointer</i>
 *   onto the stack as type <code>ptr</code>.</description>
 * </opcode>
 */
VMCASE(COP_WADDR):
{
	/* Get the address of the value starting at a frame word */
	CVM_VAR_LOADED(CVM_ARG_WIDE_SMALL);
	stacktop[0].ptrValue = (void *)(&(frame[CVM_ARG_WIDE_SMALL]));
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 1);
}
VMBREAK(COP_WADDR);

/**
 * <opcode name="maddr" group="Local variable handling">
 *   <operation>Load the address of a stack word onto the stack</operation>
 *
 *   <format>maddr<fsep/>N[1]</format>
 *   <format>wide<fsep/>maddr<fsep/>N[4]</format>
 *   <dformat>{maddr}<fsep/>N</dformat>
 *
 *   <form name="maddr" code="COP_MADDR"/>
 *
 *   <before>...</before>
 *   <after>..., pointer</after>
 *
 *   <description>Set <i>pointer</i> to the address of the word at
 *   <i>N</i> positions down the stack.  Push <i>pointer</i>
 *   onto the stack as type <code>ptr</code>.  <i>N == 1</i> indicates
 *   the address of the top-most stack word prior to the operation.
 *   </description>
 *
 *   <notes>This instruction is typically used to get the address of
 *   a managed value on the stack, so that the value can be manipulated
 *   with pointer operations.</notes>
 * </opcode>
 */
VMCASE(COP_MADDR):
{
	/* Get the address of a managed value N words down the stack */
	stacktop[0].ptrValue = (void *)(stacktop - CVM_ARG_WIDE_SMALL);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 1);
}
VMBREAK(COP_MADDR);

/**
 * <opcode name="bload" group="Local variable handling">
 *   <operation>Load <code>uint8</code> variable
 *              onto the stack</operation>
 *
 *   <format>bload<fsep/>N[1]</format>
 *   <dformat>{bload}<fsep/>N</dformat>
 *
 *   <form name="bload" code="COP_BLOAD"/>
 *
 *   <before>...</before>
 *   <after>..., value</after>
 *
 *   <description>Load the <code>uint8</code> variable from position
 *   <i>N</i> in the local variable frame and push its <i>value</i>
 *   onto the stack.</description>
 *
 *   <notes>This instruction is a quicker variant of
 *   <i>waddr N, bread</i>.<p/>
 *
 *   This instruction can also be used to load <code>bool</code>
 *   values onto the stack.</notes>
 * </opcode>
 */
VMCASE(COP_BLOAD):
{
	/* Load a byte value from the frame */
	CVM_VAR_LOADED(CVM_ARG_BYTE);
	stacktop[0].intValue = *((ILUInt8 *)&(frame[CVM_ARG_BYTE]));
	MODIFY_PC_AND_STACK(CVM_LEN_BYTE, 1);
}
VMBREAK(COP_BLOAD);

/**
 * <opcode name="bstore" group="Local variable handling">
 *   <operation>Store the top of stack into <code>uint8</code>
 *              variable</operation>
 *
 *   <format>bstore<fsep/>N[1]</format>
 *   <dformat>{bstore}<fsep/>N</dformat>
 *
 *   <form name="bstore" code="COP_BSTORE"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>Pop <i>value</i> from the stack as type <code>int32</code>
 *   and store it at position <i>N</i> in the local variable frame,
 *   truncated to the type <code>uint8</code>.
 *   </description>
 *
 *   <notes>This instruction is a quicker variant of
 *   <i>waddr N, bwrite_r</i>.<p/>
 *
 *   This instruction can also be used to store <code>bool</code>
 *   values from the stack.</notes>
 * </opcode>
 */
VMCASE(COP_BSTORE):
{
	/* Store a byte value to the frame */
	CVM_VAR_STORED(CVM_ARG_BYTE);
	*((ILUInt8 *)&(frame[CVM_ARG_BYTE])) = (ILUInt8)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_BYTE, -1);
}
VMBREAK(COP_BSTORE);

/**
 * <opcode name="bfixup" group="Local variable handling">
 *   <operation>Fix up <code>int8</code> variable</operation>
 *
 *   <format>bfixup<fsep/>N[1]</format>
 *   <format>wide<fsep/>bfixup<fsep/>N[4]</format>
 *   <dformat>{bfixup}<fsep/>N</dformat>
 *
 *   <form name="bfixup" code="COP_BFIXUP"/>
 *
 *   <description>Retrieve the contents of position <i>N</i> in the
 *   local variable frame, truncate the value to 8 bits and write it
 *   back to the same variable.  The destination is aligned at the
 *   start of the stack word that contains the variable.</description>
 *
 *   <notes>This instruction is used to align <code>int8</code> and
 *   <code>uint8</code> values that were passed as arguments to the
 *   current method.<p/>
 *
 *   The result is guaranteed to be aligned on the start of a stack
 *   word so that <i>waddr M</i> will push the correct address of
 *   the byte.<p/>
 *
 *   This instruction is not normally required on little-endian platforms,
 *   but it is definitely required on big-endian platforms.<p/>
 *
 *   The contents of an <code>int8</code> argument can be fetched
 *   using <i>waddr N, bread</i> once <i>bfixup</i> has been used to
 *   align its contents.</notes>
 * </opcode>
 */
VMCASE(COP_BFIXUP):
{
	/* Perform a byte fixup on a frame offset that corresponds
	   to an argument.  Byte arguments are passed from the caller
	   as int parameters, but inside the method we need to access
	   them by an address that is always aligned on the start
	   of a stack word.  This operation fixes address mismatches
	   on CPU's that aren't little-endian */
	*((ILInt8 *)(&(frame[CVM_ARG_WIDE_SMALL]))) =
			(ILInt8)(frame[CVM_ARG_WIDE_SMALL].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0);
}
VMBREAK(COP_BFIXUP);

/**
 * <opcode name="sfixup" group="Local variable handling">
 *   <operation>Fix up <code>int16</code> variable</operation>
 *
 *   <format>sfixup<fsep/>N[1]</format>
 *   <format>wide<fsep/>sfixup<fsep/>N[4]</format>
 *   <dformat>{sfixup}<fsep/>N</dformat>
 *
 *   <form name="sfixup" code="COP_SFIXUP"/>
 *
 *   <description>Retrieve the contents of position <i>N</i> in the
 *   local variable frame, truncate the value to 16 bits and write it
 *   back to the same variable.  The destination is aligned at the
 *   start of the stack word that contains the variable.</description>
 *
 *   <notes>This instruction is used to align <code>int16</code> and
 *   <code>uint16</code> values that were passed as arguments to the
 *   current method.<p/>
 *
 *   The result is guaranteed to be aligned on the start of a stack
 *   word so that <i>waddr M</i> will push the correct address of
 *   the 16 bit value.<p/>
 *
 *   This instruction is not normally required on little-endian platforms,
 *   but it is definitely required on big-endian platforms.<p/>
 *
 *   The contents of an <code>int16</code> argument can be fetched
 *   using <i>waddr N, sread</i> once <i>sfixup</i> has been used to
 *   align its contents.</notes>
 * </opcode>
 */
VMCASE(COP_SFIXUP):
{
	/* Perform a short fixup on a frame offset that corresponds
	   to an argument.  See above for the rationale */
	*((ILInt16 *)(&(frame[CVM_ARG_WIDE_SMALL]))) =
			(ILInt16)(frame[CVM_ARG_WIDE_SMALL].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0);
}
VMBREAK(COP_SFIXUP);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="ffixup" group="Local variable handling">
 *   <operation>Fix up <code>float32</code> variable</operation>
 *
 *   <format>ffixup<fsep/>N[1]</format>
 *   <format>wide<fsep/>ffixup<fsep/>N[4]</format>
 *   <dformat>{ffixup}<fsep/>N</dformat>
 *
 *   <form name="ffixup" code="COP_FFIXUP"/>
 *
 *   <description>Retrieve the contents of position <i>N</i> in the
 *   local variable frame as type <code>native float</code>, truncate
 *   the value to <code>float32</code> and write it back to the same
 *   variable.  The destination is aligned at the start of the stack
 *   word that contains the variable.</description>
 *
 *   <notes>This instruction is used to convert <code>native float</code>
 *   values that were passed as arguments to the current method into
 *   the <code>float32</code> for internal local variable access.<p/>
 *
 *   The result is guaranteed to be aligned on the start of a stack
 *   word so that <i>waddr M</i> will push the correct address of
 *   the <code>float32</code> value.<p/>
 *
 *   The contents of a <code>float32</code> argument can be fetched
 *   using <i>waddr N, fread</i> once <i>ffixup</i> has been used to
 *   convert its contents.</notes>
 * </opcode>
 */
VMCASE(COP_FFIXUP):
{
	/* Perform a float fixup on a frame offset that corresponds
	   to an argument.  Float arguments are passed from the caller
	   as "native float" values, but inside the method we need to
	   access them by an address that is a pointer to "float" */
	*((ILFloat *)(&(frame[CVM_ARG_WIDE_SMALL]))) =
			(ILFloat)ReadFloat(&(frame[CVM_ARG_WIDE_SMALL]));
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0);
}
VMBREAK(COP_FFIXUP);

/**
 * <opcode name="dfixup" group="Local variable handling">
 *   <operation>Fix up <code>float64</code> variable</operation>
 *
 *   <format>dfixup<fsep/>N[1]</format>
 *   <format>wide<fsep/>dfixup<fsep/>N[4]</format>
 *   <dformat>{dfixup}<fsep/>N</dformat>
 *
 *   <form name="dfixup" code="COP_DFIXUP"/>
 *
 *   <description>Retrieve the contents of position <i>N</i> in the
 *   local variable frame as type <code>native float</code>, truncate
 *   the value to <code>float64</code> and write it back to the same
 *   variable.  The destination is aligned at the start of the stack
 *   word that contains the variable.</description>
 *
 *   <notes>This instruction is used to convert <code>native float</code>
 *   values that were passed as arguments to the current method into
 *   the <code>float64</code> for internal local variable access.<p/>
 *
 *   The result is guaranteed to be aligned on the start of a stack
 *   word so that <i>waddr M</i> will push the correct address of
 *   the <code>float64</code> value.<p/>
 *
 *   The contents of a <code>float64</code> argument can be fetched
 *   using <i>waddr N, dread</i> once <i>dfixup</i> has been used to
 *   convert its contents.</notes>
 * </opcode>
 */
VMCASE(COP_DFIXUP):
{
	/* Perform a double fixup on a frame offset that corresponds
	   to an argument.  Double arguments are passed from the caller
	   as "native float" values, but inside the method we need to
	   access them by an address that is a pointer to "double" */
	WriteDouble(&(frame[CVM_ARG_WIDE_SMALL]),
			(ILDouble)ReadFloat(&(frame[CVM_ARG_WIDE_SMALL])));
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0);
}
VMBREAK(COP_DFIXUP);

#endif /* IL_CONFIG_FP_SUPPORTED */

/**
 * <opcode name="mk_local_1" group="Local variable handling">
 *   <operation>Make one local variable slot</operation>
 *
 *   <format>mk_local_1</format>
 *   <dformat>{mk_local_1}</dformat>
 *
 *   <form name="mk_local_1" code="COP_MK_LOCAL_1"/>
 *
 *   <before>...</before>
 *   <after>..., zero</after>
 *
 *   <description>Push a single zeroed word onto the stack.</description>
 *
 *   <notes>This instruction is used to allocate local variable space
 *   at the start of a method.</notes>
 * </opcode>
 */
VMCASE(COP_MK_LOCAL_1):
{
	/* Make a single local variable slot on the stack */
#ifdef CVM_WORDS_AND_PTRS_SAME_SIZE
	stacktop[0].ptrValue = 0;
#else
	IL_MEMZERO(&(stacktop[0]), sizeof(CVMWord));
#endif
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1);
}
VMNULLASM();	/* Prevent tail-end combination with ldc_i4_0 */
VMBREAK(COP_MK_LOCAL_1);

/**
 * <opcode name="mk_local_2" group="Local variable handling">
 *   <operation>Make two local variable slots</operation>
 *
 *   <format>mk_local_2</format>
 *   <dformat>{mk_local_2}</dformat>
 *
 *   <form name="mk_local_2" code="COP_MK_LOCAL_2"/>
 *
 *   <before>...</before>
 *   <after>..., zero1, zero2</after>
 *
 *   <description>Push two zeroed words onto the stack.</description>
 *
 *   <notes>This instruction is used to allocate local variable space
 *   at the start of a method.</notes>
 * </opcode>
 */
VMCASE(COP_MK_LOCAL_2):
{
	/* Make two local variable slots on the stack */
#ifdef CVM_WORDS_AND_PTRS_SAME_SIZE
	stacktop[0].ptrValue = 0;
	stacktop[1].ptrValue = 0;
#else
	IL_MEMZERO(&(stacktop[0]), sizeof(CVMWord) * 2);
#endif
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 2);
}
VMBREAK(COP_MK_LOCAL_2);

/**
 * <opcode name="mk_local_3" group="Local variable handling">
 *   <operation>Make three local variable slots</operation>
 *
 *   <format>mk_local_3</format>
 *   <dformat>{mk_local_3}</dformat>
 *
 *   <form name="mk_local_3" code="COP_MK_LOCAL_3"/>
 *
 *   <before>...</before>
 *   <after>..., zero1, zero2, zero3</after>
 *
 *   <description>Push three zeroed words onto the stack.</description>
 *
 *   <notes>This instruction is used to allocate local variable space
 *   at the start of a method.</notes>
 * </opcode>
 */
VMCASE(COP_MK_LOCAL_3):
{
	/* Make three local variable slots on the stack */
#ifdef CVM_WORDS_AND_PTRS_SAME_SIZE
	stacktop[0].ptrValue = 0;
	stacktop[1].ptrValue = 0;
	stacktop[2].ptrValue = 0;
#else
	IL_MEMZERO(&(stacktop[0]), sizeof(CVMWord) * 3);
#endif
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 3);
}
VMBREAK(COP_MK_LOCAL_3);

/**
 * <opcode name="mk_local_n" group="Local variable handling">
 *   <operation>Make <i>N</i> local variable slots</operation>
 *
 *   <format>mk_local_n<fsep/>N[1]</format>
 *   <format>wide<fsep/>mk_local_n<fsep/>N[4]</format>
 *   <dformat>{mk_local_n}<fsep/>N</dformat>
 *
 *   <form name="mk_local_n" code="COP_MK_LOCAL_N"/>
 *
 *   <before>...</before>
 *   <after>..., zero1, ..., zeroN</after>
 *
 *   <description>Push <i>N</i> zeroed words onto the stack.</description>
 *
 *   <notes>This instruction is used to allocate local variable space
 *   at the start of a method.</notes>
 * </opcode>
 */
VMCASE(COP_MK_LOCAL_N):
{
	/* Make an arbitrary number of local variable slots */
	IL_MEMZERO(&(stacktop[0]), sizeof(CVMWord) * CVM_ARG_WIDE_SMALL);
	MODIFY_PC_AND_STACK_REVERSE(CVM_LEN_WIDE_SMALL, CVM_ARG_WIDE_SMALL);
}
VMBREAK(COP_MK_LOCAL_N);

#elif defined(IL_CVM_WIDE)

case COP_ILOAD:
{
	/* Wide version of "iload" */
	stacktop[0].intValue = frame[CVM_ARG_WIDE_LARGE].intValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 1);
}
VMBREAKNOEND;

case COP_PLOAD:
{
	/* Wide version of "pload" */
	stacktop[0].ptrValue = frame[CVM_ARG_WIDE_LARGE].ptrValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 1);
}
VMBREAKNOEND;

case COP_ISTORE:
{
	/* Wide version of "istore" */
	frame[CVM_ARG_WIDE_LARGE].intValue = stacktop[-1].intValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, -1);
}
VMBREAKNOEND;

case COP_PSTORE:
{
	/* Wide version of "pstore" */
	frame[CVM_ARG_WIDE_LARGE].ptrValue = stacktop[-1].ptrValue;
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, -1);
}
VMBREAKNOEND;

case COP_MLOAD:
{
	/* Wide version of "mload" */
	tempNum = CVM_ARG_DWIDE2_LARGE;
	IL_MEMCPY(stacktop, &(frame[CVM_ARG_DWIDE1_LARGE]),
			  sizeof(CVMWord) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_DWIDE_LARGE, tempNum);
}
VMBREAKNOEND;

case COP_MSTORE:
{
	/* Wide version of "mstore" */
	tempNum = CVM_ARG_DWIDE2_LARGE;
	stacktop -= tempNum;
	IL_MEMCPY(&(frame[CVM_ARG_DWIDE1_LARGE]), stacktop,
			  sizeof(CVMWord) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_DWIDE_LARGE, 0);
}
VMBREAKNOEND;

case COP_WADDR:
{
	/* Wide version of "waddr" */
	stacktop[0].ptrValue = (void *)(&(frame[CVM_ARG_WIDE_LARGE]));
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 1);
}
VMBREAKNOEND;

case COP_MADDR:
{
	/* Wide version of "maddr" */
	stacktop[0].ptrValue = (void *)(stacktop - CVM_ARG_WIDE_LARGE);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 1);
}
VMBREAKNOEND;

case COP_BFIXUP:
{
	/* Wide version of "bfixup" */
	tempNum = CVM_ARG_WIDE_LARGE;
	*((ILInt8 *)(&(frame[tempNum]))) = (ILInt8)(frame[tempNum].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0);
}
VMBREAKNOEND;

case COP_SFIXUP:
{
	/* Wide version of "sfixup" */
	tempNum = CVM_ARG_WIDE_LARGE;
	*((ILInt16 *)(&(frame[tempNum]))) = (ILInt16)(frame[tempNum].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0);
}
VMBREAKNOEND;

#ifdef IL_CONFIG_FP_SUPPORTED

case COP_FFIXUP:
{
	/* Wide version of "ffixup" */
	tempNum = CVM_ARG_WIDE_LARGE;
	*((ILFloat *)(&(frame[tempNum]))) = (ILFloat)ReadFloat(&(frame[tempNum]));
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0);
}
VMBREAKNOEND;

case COP_DFIXUP:
{
	/* Wide version of "dfixup" */
	tempNum = CVM_ARG_WIDE_LARGE;
	WriteDouble(&(frame[tempNum]), (ILDouble)ReadFloat(&(frame[tempNum])));
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0);
}
VMBREAKNOEND;

#else /* !IL_CONFIG_FP_SUPPORTED */

case COP_FFIXUP:
case COP_DFIXUP:
{
	COPY_STATE_TO_THREAD();
	stacktop[0].ptrValue =
		_ILSystemException(thread, "System.NotImplementedException");
	stacktop += 1;
	goto throwException;
}
/* Not reached */

#endif /* !IL_CONFIG_FP_SUPPORTED */

case COP_MK_LOCAL_N:
{
	/* Wide version of "mk_local_n" */
	tempNum = CVM_ARG_WIDE_LARGE;
	IL_MEMZERO(&(stacktop[0]), sizeof(CVMWord) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, tempNum);
}
VMBREAKNOEND;

#endif /* IL_CVM_WIDE */
