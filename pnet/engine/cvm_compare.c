/*
 * cvm_compare.c - Opcodes for comparing values on the stack.
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
 * Compare two long values.
 */
static IL_INLINE ILInt32 LCmp(CVMWord *a, CVMWord *b)
{
	ILInt64 tempa = ReadLong(a);
	ILInt64 tempb = ReadLong(b);
	if(tempa < tempb)
	{
		return -1;
	}
	else if(tempa > tempb)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Compare two unsigned long values.
 */
static IL_INLINE ILInt32 LUCmp(CVMWord *a, CVMWord *b)
{
	ILUInt64 tempa = ReadULong(a);
	ILUInt64 tempb = ReadULong(b);
	if(tempa < tempb)
	{
		return -1;
	}
	else if(tempa > tempb)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

#ifdef IL_CONFIG_FP_SUPPORTED

/*
 * Compare two native float values.
 */
static IL_INLINE ILInt32 FCmp(CVMWord *a, CVMWord *b, ILInt32 nanResult)
{
	ILNativeFloat tempa = ReadFloat(a);
	ILNativeFloat tempb = ReadFloat(b);
	if(ILNativeFloatIsNaN(tempa) || ILNativeFloatIsNaN(tempb))
	{
		return nanResult;
	}
	else if(tempa < tempb)
	{
		return -1;
	}
	else if(tempa > tempb)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

#endif /* IL_CONFIG_FP_SUPPORTED */

#elif defined(IL_CVM_LOCALS)

/* No locals required */

#elif defined(IL_CVM_PREFIX)

/**
 * <opcode name="icmp" group="Comparison operators">
 *   <operation>Compare <code>int32</code></operation>
 *
 *   <format>prefix<fsep/>icmp</format>
 *   <dformat>{icmp}</dformat>
 *
 *   <form name="icmp" code="COP_PREFIX_ICMP"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>int32</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_ICMP):
{
	/* Compare integer values */
	if(stacktop[-2].intValue < stacktop[-1].intValue)
	{
		stacktop[-2].intValue = -1;
	}
	else if(stacktop[-2].intValue > stacktop[-1].intValue)
	{
		stacktop[-2].intValue = 1;
	}
	else
	{
		stacktop[-2].intValue = 0;
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_ICMP);

/**
 * <opcode name="icmp_un" group="Comparison operators">
 *   <operation>Compare <code>uint32</code></operation>
 *
 *   <format>prefix<fsep/>icmp_un</format>
 *   <dformat>{icmp_un}</dformat>
 *
 *   <form name="icmp_un" code="COP_PREFIX_ICMP_UN"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>uint32</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_ICMP_UN):
{
	/* Compare unsigned integer values */
	if(stacktop[-2].uintValue < stacktop[-1].uintValue)
	{
		stacktop[-2].intValue = -1;
	}
	else if(stacktop[-2].uintValue > stacktop[-1].uintValue)
	{
		stacktop[-2].intValue = 1;
	}
	else
	{
		stacktop[-2].intValue = 0;
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_ICMP_UN);

/**
 * <opcode name="lcmp" group="Comparison operators">
 *   <operation>Compare <code>int64</code></operation>
 *
 *   <format>prefix<fsep/>lcmp</format>
 *   <dformat>{lcmp}</dformat>
 *
 *   <form name="lcmp" code="COP_PREFIX_LCMP"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>int64</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_LCMP):
{
	/* Compare long values */
	stacktop[-(CVM_WORDS_PER_LONG * 2)].intValue =
		LCmp(&(stacktop[-(CVM_WORDS_PER_LONG * 2)]),
		     &(stacktop[-CVM_WORDS_PER_LONG]));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -(CVM_WORDS_PER_LONG * 2) + 1);
}
VMBREAK(COP_PREFIX_LCMP);

/**
 * <opcode name="lcmp_un" group="Comparison operators">
 *   <operation>Compare <code>uint64</code></operation>
 *
 *   <format>prefix<fsep/>lcmp_un</format>
 *   <dformat>{lcmp_un}</dformat>
 *
 *   <form name="lcmp_un" code="COP_PREFIX_LCMP_UN"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>uint64</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_LCMP_UN):
{
	/* Compare unsigned long values */
	stacktop[-(CVM_WORDS_PER_LONG * 2)].intValue =
		LUCmp(&(stacktop[-(CVM_WORDS_PER_LONG * 2)]),
		      &(stacktop[-CVM_WORDS_PER_LONG]));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -(CVM_WORDS_PER_LONG * 2) + 1);
}
VMBREAK(COP_PREFIX_LCMP_UN);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="fcmpl" group="Comparison operators">
 *   <operation>Compare <code>native float</code> with lower result</operation>
 *
 *   <format>prefix<fsep/>fcmpl</format>
 *   <dformat>{fcmpl}</dformat>
 *
 *   <form name="fcmpl" code="COP_PREFIX_FCMPL"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>native float</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If either <i>value1</i> or <i>value2</i> is NaN, then
 *         <i>result</i> is -1.</li>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_FCMPL):
{
	/* Compare float values */
	stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT * 2)].intValue =
		FCmp(&(stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT * 2)]),
		     &(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]), -1);
	MODIFY_PC_AND_STACK
		(CVMP_LEN_NONE, -(CVM_WORDS_PER_NATIVE_FLOAT * 2) + 1);
}
VMBREAK(COP_PREFIX_FCMPL);

/**
 * <opcode name="fcmpg" group="Comparison operators">
 *   <operation>Compare <code>native float</code> with
 *              greater result</operation>
 *
 *   <format>prefix<fsep/>fcmpg</format>
 *   <dformat>{fcmpg}</dformat>
 *
 *   <form name="fcmpg" code="COP_PREFIX_FCMPG"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>native float</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If either <i>value1</i> or <i>value2</i> is NaN, then
 *         <i>result</i> is 1.</li>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_FCMPG):
{
	/* Compare float values */
	stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT * 2)].intValue =
		FCmp(&(stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT * 2)]),
		     &(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]), 1);
	MODIFY_PC_AND_STACK
		(CVMP_LEN_NONE, -(CVM_WORDS_PER_NATIVE_FLOAT * 2) + 1);
}
VMBREAK(COP_PREFIX_FCMPG);

#endif /* IL_CONFIG_FP_SUPPORTED */

/**
 * <opcode name="pcmp" group="Comparison operators">
 *   <operation>Compare <code>ptr</code></operation>
 *
 *   <format>prefix<fsep/>pcmp</format>
 *   <dformat>{pcmp}</dformat>
 *
 *   <form name="pcmp" code="COP_PREFIX_PCMP"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i>
 *   are popped from the stack as type <code>ptr</code>.  The
 *   <code>int32</code> <i>result</i> is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt; value2</i>, then <i>result</i> is -1.</li>
 *     <li>If <i>value1 &gt; value2</i>, then <i>result</i> is 1.</li>
 *     <li>Otherwise, <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_PCMP):
{
	/* Compare pointer values */
	if(stacktop[-2].ptrValue < stacktop[-1].ptrValue)
	{
		stacktop[-2].intValue = -1;
	}
	else if(stacktop[-2].ptrValue > stacktop[-1].ptrValue)
	{
		stacktop[-2].intValue = 1;
	}
	else
	{
		stacktop[-2].intValue = 0;
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_PCMP);

/**
 * <opcode name="seteq" group="Comparison operators">
 *   <operation>Set if equal to zero</operation>
 *
 *   <format>prefix<fsep/>seteq</format>
 *   <dformat>{seteq}</dformat>
 *
 *   <form name="seteq" code="COP_PREFIX_SETEQ"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 == 0</i>, then <i>result</i> is 1.</li>
 *     <li>If <i>value1 != 0</i>, then <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_SETEQ):
{
	/* Set true if the stack top is zero */
	stacktop[-1].intValue = (stacktop[-1].intValue == 0);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SETEQ);

/**
 * <opcode name="setne" group="Comparison operators">
 *   <operation>Set if not equal to zero</operation>
 *
 *   <format>prefix<fsep/>setne</format>
 *   <dformat>{setne}</dformat>
 *
 *   <form name="setne" code="COP_PREFIX_SETNE"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 != 0</i>, then <i>result</i> is 1.</li>
 *     <li>If <i>value1 == 0</i>, then <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_SETNE):
{
	/* Set true if the stack top is non-zero */
	stacktop[-1].intValue = (stacktop[-1].intValue != 0);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SETNE);

/**
 * <opcode name="setlt" group="Comparison operators">
 *   <operation>Set if less than zero</operation>
 *
 *   <format>prefix<fsep/>setlt</format>
 *   <dformat>{setlt}</dformat>
 *
 *   <form name="setlt" code="COP_PREFIX_SETLT"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt; 0</i>, then <i>result</i> is 1.</li>
 *     <li>If <i>value1 &gt;= 0</i>, then <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_SETLT):
{
	/* Set true if the stack top is less than zero */
	stacktop[-1].intValue = (stacktop[-1].intValue < 0);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SETLT);

/**
 * <opcode name="setle" group="Comparison operators">
 *   <operation>Set if less than or equal to zero</operation>
 *
 *   <format>prefix<fsep/>setle</format>
 *   <dformat>{setle}</dformat>
 *
 *   <form name="setle" code="COP_PREFIX_SETLE"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &lt;= 0</i>, then <i>result</i> is 1.</li>
 *     <li>If <i>value1 &gt; 0</i>, then <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_SETLE):
{
	/* Set true if the stack top is less or equal to zero */
	stacktop[-1].intValue = (stacktop[-1].intValue <= 0);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SETLE);

/**
 * <opcode name="setgt" group="Comparison operators">
 *   <operation>Set if greater than zero</operation>
 *
 *   <format>prefix<fsep/>setgt</format>
 *   <dformat>{setgt}</dformat>
 *
 *   <form name="setgt" code="COP_PREFIX_SETGT"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &gt; 0</i>, then <i>result</i> is 1.</li>
 *     <li>If <i>value1 &lt;= 0</i>, then <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_SETGT):
{
	/* Set true if the stack top is greater than zero */
	stacktop[-1].intValue = (stacktop[-1].intValue > 0);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SETGT);

/**
 * <opcode name="setge" group="Comparison operators">
 *   <operation>Set if greater than or equal to zero</operation>
 *
 *   <format>prefix<fsep/>setge</format>
 *   <dformat>{setge}</dformat>
 *
 *   <form name="setge" code="COP_PREFIX_SETGE"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is determined as follows:
 *
 *   <ul>
 *     <li>If <i>value1 &gt;= 0</i>, then <i>result</i> is 1.</li>
 *     <li>If <i>value1 &lt; 0</i>, then <i>result</i> is 0.</li>
 *   </ul>
 *
 *   The <i>result</i> is then pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_SETGE):
{
	/* Set true if the stack top greater or equal to zero */
	stacktop[-1].intValue = (stacktop[-1].intValue >= 0);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SETGE);

#endif /* IL_CVM_PREFIX */
