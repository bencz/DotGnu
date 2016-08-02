/*
 * cvm_stack.c - Opcodes for accessing the stack.
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
 * <opcode name="dup" group="Stack manipulation">
 *   <operation>Duplicate the top of stack</operation>
 *
 *   <format>dup</format>
 *   <dformat>{dup}</dformat>
 *
 *   <form name="dup" code="COP_DUP"/>
 *
 *   <before>..., value</before>
 *   <after>..., value, value</after>
 *
 *   <description>Pop the single-word <i>value</i> from
 *   the stack, and then push it twice.</description>
 * </opcode>
 */
VMCASE(COP_DUP):
{
	/* Duplicate the top-most word on the stack */
	stacktop[0] = stacktop[-1];
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 1);
}
VMBREAK(COP_DUP);

/**
 * <opcode name="dup2" group="Stack manipulation">
 *   <operation>Duplicate the top two stack words</operation>
 *
 *   <format>dup2</format>
 *   <dformat>{dup2}</dformat>
 *
 *   <form name="dup2" code="COP_DUP2"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., value1, value2, value1, value2</after>
 *
 *   <description>Pop the words <i>value1</i> and <i>value2</i> from
 *   the stack and then push them twice.</description>
 * </opcode>
 */
VMCASE(COP_DUP2):
{
	/* Duplicate the top two words on the stack */
	stacktop[0] = stacktop[-2];
	stacktop[1] = stacktop[-1];
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 2);
}
VMBREAK(COP_DUP2);

/**
 * <opcode name="dup_n" group="Stack manipulation">
 *   <operation>Duplicate the top <i>N</i> stack words</operation>
 *
 *   <format>dup_n<fsep/>N[1]</format>
 *   <format>wide<fsep/>dup_n<fsep/>N[4]</format>
 *   <dformat>{dup_n}<fsep/>N</dformat>
 *
 *   <form name="dup_n" code="COP_DUP_N"/>
 *
 *   <before>..., value1, ..., valueN</before>
 *   <after>..., value1, ..., valueN, value1, ..., valueN</after>
 *
 *   <description>Pop the top <i>N</i> words from the stack and then
 *   push them twice.</description>
 *
 *   <notes>This is typically used for value type instances that are larger
 *   than 2 words in size.</notes>
 * </opcode>
 */
VMCASE(COP_DUP_N):
{
	/* Duplicate the top N words on the stack */
	tempNum = CVM_ARG_WIDE_SMALL;
	IL_MEMCPY(&(stacktop[0]), &(stacktop[-((ILInt32)tempNum)]),
			  sizeof(CVMWord) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, tempNum);
}
VMBREAK(COP_DUP_N);

/**
 * <opcode name="dup_word_n" group="Stack manipulation">
 *   <operation>Duplicate a stack word that is <i>N</i> words
 *				down the stack</operation>
 *
 *   <format>dup_word_n<fsep/>N[1]</format>
 *   <format>wide<fsep/>dup_word_n<fsep/>N[4]</format>
 *   <dformat>{dup_word_n}<fsep/>N</dformat>
 *
 *   <form name="dup_word_n" code="COP_DUP_WORD_N"/>
 *
 *   <before>..., value, word1, ..., wordN</before>
 *   <after>..., value, word1, ..., wordN, value</after>
 *
 *   <description>Retrieve the <i>value</i> that is <i>N</i> words
 *   down the stack and push it onto the top of the stack.</description>
 * </opcode>
 */
VMCASE(COP_DUP_WORD_N):
{
	/* Duplicate a word which is N words down the stack */
	stacktop[0] = stacktop[-(((ILInt32)CVM_ARG_WIDE_SMALL) + 1)];
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 1);
}
VMBREAK(COP_DUP_WORD_N);

/**
 * <opcode name="pop" group="Stack manipulation">
 *   <operation>Pop the top-most word from the stack</operation>
 *
 *   <format>pop</format>
 *   <dformat>{pop}</dformat>
 *
 *   <form name="pop" code="COP_POP"/>
 *
 *   <before>..., value</before>
 *   <after>...</after>
 *
 *   <description>Pop the single-word <i>value</i> from
 *   the stack.</description>
 * </opcode>
 */
VMCASE(COP_POP):
{
	/* Pop the top-most word from the stack */
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -1);
}
VMBREAK(COP_POP);

/**
 * <opcode name="pop2" group="Stack manipulation">
 *   <operation>Pop the top two words from the stack</operation>
 *
 *   <format>pop2</format>
 *   <dformat>{pop2}</dformat>
 *
 *   <form name="pop2" code="COP_POP2"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>...</after>
 *
 *   <description>Pop the two stack words <i>value1</i> and <i>value2</i>
 *   from the stack.</description>
 * </opcode>
 */
VMCASE(COP_POP2):
{
	/* Pop the top two words from the stack */
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -2);
}
VMBREAK(COP_POP2);

/**
 * <opcode name="pop_n" group="Stack manipulation">
 *   <operation>Pop the top <i>N</i> words from the stack</operation>
 *
 *   <format>pop_n<fsep/>N[1]</format>
 *   <format>wide<fsep/>pop_n<fsep/>N[4]</format>
 *   <dformat>{pop_n}<fsep/>N</dformat>
 *
 *   <form name="pop_n" code="COP_POP_N"/>
 *
 *   <before>..., value1, ..., valueN</before>
 *   <after>...</after>
 *
 *   <description>Pop the top <i>N</i> stack words
 *   from the stack.</description>
 * </opcode>
 */
VMCASE(COP_POP_N):
{
	/* Pop the top N words from the stack */
	MODIFY_PC_AND_STACK_REVERSE(CVM_LEN_WIDE_SMALL,
								-((ILInt32)CVM_ARG_WIDE_SMALL));
}
VMBREAK(COP_POP_N);

/**
 * <opcode name="squash" group="Stack manipulation">
 *   <operation>Squash a number of words out of the stack</operation>
 *
 *   <format>squash<fsep/>N[1]<fsep/>M[1]</format>
 *   <format>wide<fsep/>squash<fsep/>N[4]<fsep/>M[4]</format>
 *   <dformat>{squash}<fsep/>N<fsep/>M</dformat>
 *
 *   <form name="squash" code="COP_SQUASH"/>
 *
 *   <before>..., word1, ..., wordM, value1, ..., valueN</before>
 *   <after>..., value1, ..., valueN</after>
 *
 *   <description>Remove the <i>M</i> words from the stack,
 *   <i>N</i> words down the stack.</description>
 * </opcode>
 */
VMCASE(COP_SQUASH):
{
	/* Squash M words out of the stack, N words down the stack */
	tempNum = CVM_ARG_DWIDE1_SMALL;
	tempSize = CVM_ARG_DWIDE2_SMALL;
	IL_MEMMOVE(&(stacktop[-(((ILInt32)tempNum) + ((ILInt32)tempSize))]),
			   &(stacktop[-((ILInt32)tempNum)]),
			   sizeof(CVMWord *) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_DWIDE_SMALL, -((ILInt32)tempSize));
}
VMBREAK(COP_SQUASH);

/**
 * <opcode name="ckheight" group="Stack manipulation">
 *   <operation>Check the height of the stack</operation>
 *
 *   <format>ckheight<fsep/>0<fsep/>0<fsep/>0<fsep/>0</format>
 *   <dformat>{ckheight}<fsep/>0</dformat>
 *
 *   <form name="ckheight" code="COP_CKHEIGHT"/>
 *
 *   <description>Check that the stack has at least 8 words of
 *   space available for pushing values.</description>
 *
 *   <notes>This instruction is used at the start of a method to
 *   check that that there is sufficient stack space to hold the
 *   local variables and temporary stack values used by the method.<p/>
 *
 *   The opcode is followed by 4 zero bytes so that this instruction
 *   has the same length as <i>ckheight_n</i>.  This makes it easier
 *   to back-patch the height after translating the method.</notes>
 *
 *   <exceptions>
 *     <exception name="System.StackOverflowException">Raised if
 *     there is insufficient space available on the stack.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_CKHEIGHT):
{
	/* Check the height of the stack to ensure that we have
	   at least 8 stack slots available.  This instruction
	   occupies 5 bytes because it is normally overlaid on
	   top of a longer CKHEIGHT_N instruction.  We allow one
	   extra slot to allow for engine exception objects to
	   be pushed onto the stack at the maximum height */
	if(((ILUInt32)(stackmax - stacktop)) > 8)
	{
		MODIFY_PC_AND_STACK(CVM_LEN_WORD, 0);
	}
	else
	{
		STACK_OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_CKHEIGHT);

/**
 * <opcode name="ckheight_n" group="Stack manipulation">
 *   <operation>Check the height of the stack for <i>N</i>
 *              words of available space</operation>
 *
 *   <format>ckheight_n<fsep/>N[4]</format>
 *   <dformat>{ckheight_n}<fsep/>N</dformat>
 *
 *   <form name="ckheight_n" code="COP_CKHEIGHT_N"/>
 *
 *   <description>Check that the stack has at least <i>N</i> words of
 *   space available for pushing values.</description>
 *
 *   <notes>This instruction is used at the start of a method to
 *   check that that there is sufficient stack space to hold the
 *   local variables and temporary stack values used by the method.</notes>
 *
 *   <exceptions>
 *     <exception name="System.StackOverflowException">Raised if
 *     there is insufficient space available on the stack.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_CKHEIGHT_N):
{
	/* Check the height of the stack to ensure that we have
	   at least N stack slots available, plus 1 extra to allow
	   for engine exception objects to be pushed */
	if(((ILUInt32)(stackmax - stacktop)) > CVM_ARG_WORD)
	{
		MODIFY_PC_AND_STACK(CVM_LEN_WORD, 0);
	}
	else
	{
		STACK_OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_CKHEIGHT_N);

/**
 * <opcode name="set_num_args" group="Call management instructions">
 *   <operation>Set the number of arguments for the current method</operation>
 *
 *   <format>set_num_args<fsep/>N[1]</format>
 *   <format>wide<fsep/>set_num_args<fsep/>N[4]</format>
 *   <dformat>{set_num_args}<fsep/>N</dformat>
 *
 *   <form name="set_num_args" code="COP_SET_NUM_ARGS"/>
 *
 *   <description>Set the frame pointer for the current method to
 *   the address of the <i>N</i>'th word down the stack.</description>
 *
 *   <notes>This is typically the first instruction in a method, which
 *   sets up the local variable frame.</notes>
 * </opcode>
 */
VMCASE(COP_SET_NUM_ARGS):
{
	/* Set the number of argument stack slots */
	frame = stacktop - CVM_ARG_WIDE_SMALL;

	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0);
}
VMBREAK(COP_SET_NUM_ARGS);

#elif defined(IL_CVM_WIDE)

case COP_DUP_N:
{
	/* Wide version of "dup_n" */
	tempNum = CVM_ARG_WIDE_LARGE;
	IL_MEMCPY(&(stacktop[0]), &(stacktop[-((int)tempNum)]),
			  sizeof(CVMWord) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, tempNum);
}
VMBREAKNOEND;

case COP_DUP_WORD_N:
{
	/* Wide version of "dup_word_n" */
	stacktop[0] = stacktop[-(((ILInt32)CVM_ARG_WIDE_LARGE) + 1)];
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 1);
}
VMBREAKNOEND;

case COP_POP_N:
{
	/* Wide version of "pop_n" */
	MODIFY_PC_AND_STACK_REVERSE(CVM_LEN_WIDE_LARGE,
								-((ILInt32)CVM_ARG_WIDE_LARGE));
}
VMBREAKNOEND;

case COP_SQUASH:
{
	/* Wide version of "squash" */
	tempNum = CVM_ARG_DWIDE1_LARGE;
	tempSize = CVM_ARG_DWIDE2_LARGE;
	tempptr = stacktop - tempNum - tempSize;
	IL_MEMMOVE(tempptr, stacktop - tempNum, sizeof(CVMWord *) * tempNum);
	MODIFY_PC_AND_STACK(CVM_LEN_DWIDE_LARGE, -((ILInt32)tempSize));
}
VMBREAKNOEND;

case COP_SET_NUM_ARGS:
{
	/* Wide version of "set_num_args" */
	frame = stacktop - CVM_ARG_WIDE_LARGE;

	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0);
}
VMBREAKNOEND;

#elif defined(IL_CVM_PREFIX)

/**
 * <opcode name="local_alloc" group="Stack manipulation">
 *   <operation>Allocate local stack space</operation>
 *
 *   <format>prefix<fsep/>local_alloc</format>
 *   <dformat>{local_alloc}</dformat>
 *
 *   <form name="local_alloc" code="COP_PREFIX_LOCAL_ALLOC"/>
 *
 *   <before>..., size</before>
 *   <after>..., pointer</after>
 *
 *   <description>Pop <i>size</i> from the stack as type <code>uint</code>,
 *   and then push a <i>pointer</i> to a block of memory of <i>size</i> bytes
 *   in size.</description>
 *
 *   <notes>The block is not expected to last beyond the lifetime of
 *   the current method, but implementations may allocate longer-term
 *   memory if it is difficult to do direct stack allocation.</notes>
 *
 *   <exceptions>
 *     <exception name="System.OutOfMemoryException">Raised if
 *     there is insufficient memory to allocate the block.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_LOCAL_ALLOC):
{
	/* Allocate local stack space.  We allocate it within the
	   garbage-collected heap because we don't have a traditional
	   C-style stack that we can use */
	if((stacktop[-1].ptrValue =
			ILGCAlloc((unsigned long)(stacktop[-1].uintValue))) != 0)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		STACK_OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_LOCAL_ALLOC);

/**
 * <opcode name="repl_word_n" group="Stack manipulation">
 *   <operation>Replace a stack word that is <i>N</i> words
 *				down the stack with the value at top of the stack</operation>
 *
 *   <format>prefix<fsep/>repl_word_n</format>
 *   <dformat>{repl_word_n}</dformat>
 *
 *   <form name="repl_word_n" code="COP_PREFIX_REPL_WORD_N"/>
 *
 *   <before>..., value, word1, ..., wordN-1, wordN</before>
 *   <after>..., wordN, word1, ..., wordN-1</after>
 *
 *   <description>Replace the <i>value</i> that is <i>N</i> words
 *   down the stack with the value on top of the stack and then pop stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_REPL_WORD_N):
{
	stacktop[-(((ILInt32)CVMP_ARG_WORD) + 1)] = stacktop[-1];
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD, -1);
}
VMBREAK(COP_PREFIX_REPL_WORD_N);

#endif /* IL_CVM_PREFIX */
