/*
 * cvm_inline.c - Opcodes for inlined methods.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 *
 * Contributions: Thong Nguyen (tum@veridicus.com)
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
 * Test two strings for equality.
 */
static IL_INLINE int StringEquals(System_String *str1,
								  System_String *str2)
{
	/* marginally faster */
	if (str2 == str1)
	{
		return 1;
	}
	else if(str1 && str2)
	{
		if(str1->length != str2->length) 
		{
			return 0;
		}
		if(str2->length == 0 ||
			!IL_MEMCMP(StringToBuffer(str1), 
					StringToBuffer(str2), str1->length * 2))
		{
			return 1;
		}
	}

	return 0;
}

#define COP_PREFIX_MATH_CASE(f1, f2) \
VMCASE(COP_PREFIX_##f1): \
{ \
	WriteFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT], (ILNativeFloat)(_IL_Math_##f2(thread, (ILDouble)ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])))); \
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0); \
} \
VMBREAK(COP_PREFIX_##f1)

#define COP_PREFIX_MATH_CASE_2(f1, f2) \
VMCASE(COP_PREFIX_##f1): \
{ \
	WriteFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)], (ILNativeFloat)(_IL_Math_##f2(thread, (ILDouble)ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)]), (ILDouble)ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT)])))); \
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT); \
} \
VMBREAK(COP_PREFIX_##f1)

#elif defined(IL_CVM_LOCALS)

ILInt32 tempI4;
#ifdef IL_CONFIG_FP_SUPPORTED
ILDouble tempR8;
#endif /* IL_CONFIG_FP_SUPPORTED */
System_Text_StringBuilder *builder;

#elif defined(IL_CVM_MAIN)

/* Nothing in the main table */

#elif defined(IL_CVM_PREFIX)

/**
 * <opcode name="string_concat_2" group="Inline methods">
 *   <operation>Concatenate two strings</operation>
 *
 *   <format>prefix<fsep/>string_concat_2</format>
 *   <dformat>{string_concat_2}</dformat>
 *
 *   <form name="string_concat_2" code="COP_PREFIX_STRING_CONCAT_2"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>string</code>.  The <i>result</i>
 *   is the <code>string</code> that results from concatenating
 *   <i>value1</i> and <i>value2</i>.  The <i>result</i> is pushed
 *   onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>String.Concat(String, String)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRING_CONCAT_2):
{
	/* Concatenate two strings */
	COPY_STATE_TO_THREAD();
	stacktop[-2].ptrValue =
		ILStringConcat(thread,
					   (ILString *)(stacktop[-2].ptrValue),
					   (ILString *)(stacktop[-1].ptrValue));
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_STRING_CONCAT_2);

/**
 * <opcode name="string_concat_3" group="Inline methods">
 *   <operation>Concatenate three strings</operation>
 *
 *   <format>prefix<fsep/>string_concat_3</format>
 *   <dformat>{string_concat_3}</dformat>
 *
 *   <form name="string_concat_3" code="COP_PREFIX_STRING_CONCAT_3"/>
 *
 *   <before>..., value1, value2, value3</before>
 *   <after>..., result</after>
 *
 *   <description>The values <i>value1</i>, <i>value2</i>, and <i>value3</i>
 *   are popped from the stack as type <code>string</code>.  The <i>result</i>
 *   is the <code>string</code> that results from concatenating
 *   <i>value1</i>, <i>value2</i>, and <i>value3</i>.  The <i>result</i>
 *   is pushed onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>String.Concat(String, String, String)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRING_CONCAT_3):
{
	/* Concatenate three strings */
	COPY_STATE_TO_THREAD();
	stacktop[-3].ptrValue =
		ILStringConcat3(thread,
						(ILString *)(stacktop[-3].ptrValue),
					    (ILString *)(stacktop[-2].ptrValue),
					    (ILString *)(stacktop[-1].ptrValue));
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -2);
}
VMBREAK(COP_PREFIX_STRING_CONCAT_3);

/**
 * <opcode name="string_concat_4" group="Inline methods">
 *   <operation>Concatenate four strings</operation>
 *
 *   <format>prefix<fsep/>string_concat_4</format>
 *   <dformat>{string_concat_4}</dformat>
 *
 *   <form name="string_concat_4" code="COP_PREFIX_STRING_CONCAT_4"/>
 *
 *   <before>..., value1, value2, value3, value4</before>
 *   <after>..., result</after>
 *
 *   <description>The values <i>value1</i>, <i>value2</i>, <i>value3</i>,
 *   and <i>value4</i> are popped from the stack as type <code>string</code>.
 *   The <i>result</i> is the <code>string</code> that results from
 *   concatenating <i>value1</i>, <i>value2</i>, <i>value3</i>, and
 *   <i>value4</i>.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>String.Concat(String, String, String, String)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRING_CONCAT_4):
{
	/* Concatenate four strings */
	COPY_STATE_TO_THREAD();
	stacktop[-4].ptrValue =
		ILStringConcat4(thread,
						(ILString *)(stacktop[-4].ptrValue),
					    (ILString *)(stacktop[-3].ptrValue),
					    (ILString *)(stacktop[-2].ptrValue),
					    (ILString *)(stacktop[-1].ptrValue));
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -3);
}
VMBREAK(COP_PREFIX_STRING_CONCAT_4);

/**
 * <opcode name="string_eq" group="Inline methods">
 *   <operation>Test two strings for equality</operation>
 *
 *   <format>prefix<fsep/>string_eq</format>
 *   <dformat>{string_eq}</dformat>
 *
 *   <form name="string_eq" code="COP_PREFIX_STRING_EQ"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>string</code>.  The <i>result</i>
 *   is the <code>int32</code> that results from comparing
 *   <i>value1</i> and <i>value2</i>: 1 if they are equal, and 0
 *   if they are not equal.  The <i>result</i> is pushed
 *   onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>String.op_Equality(String, String)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRING_EQ):
{
	/* Test two strings for equality */
	stacktop[-2].intValue =
		StringEquals((System_String *)(stacktop[-2].ptrValue),
					 (System_String *)(stacktop[-1].ptrValue));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_STRING_EQ);

/**
 * <opcode name="string_ne" group="Inline methods">
 *   <operation>Test two strings for inequality</operation>
 *
 *   <format>prefix<fsep/>string_ne</format>
 *   <dformat>{string_ne}</dformat>
 *
 *   <form name="string_ne" code="COP_PREFIX_STRING_NE"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as type <code>string</code>.  The <i>result</i>
 *   is the <code>int32</code> that results from comparing
 *   <i>value1</i> and <i>value2</i>: 1 if they are not equal, and 0
 *   if they are equal.  The <i>result</i> is pushed
 *   onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>String.op_Inequality(String, String)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRING_NE):
{
	/* Test two strings for inequality */
	stacktop[-2].intValue =
		!StringEquals((System_String *)(stacktop[-2].ptrValue),
					  (System_String *)(stacktop[-1].ptrValue));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_STRING_NE);

/**
 * <opcode name="string_get_char" group="Inline methods">
 *   <operation>Get a particular character from a string</operation>
 *
 *   <format>prefix<fsep/>string_get_char</format>
 *   <dformat>{string_get_char}</dformat>
 *
 *   <form name="string_get_char" code="COP_PREFIX_STRING_GET_CHAR"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped
 *   from the stack as the types <code>string</code> and <code>int32</code>
 *   respectively.  The <i>result</i> is the <code>int32</code> that
 *   results from fetching the character at position <i>value2</i>
 *   within the string <i>value1</i>.
 *   <code>System.IndexOutOfRangeException</code> will be thrown if
 *   <i>value2</i> is an invalid index.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>String.get_Chars(int)</code> method.</notes>
 *
 *   <exceptions>
 *     <exception name="System.NullReferenceException">Raised if
 *     <i>value1</i> is <code>null</code>.</exception>
 *     <exception name="System.IndexOutOfRangeException">Raised if
 *     <i>value2</i> is not a valid character index for the string
 *     <i>value1</i>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRING_GET_CHAR):
{
	/* Get a character from a string */
	tempptr = stacktop[-2].ptrValue;
	BEGIN_NULL_CHECK(tempptr)
	{
		tempNum = stacktop[-1].uintValue;
		if(tempNum < ((System_String *)tempptr)->length)
		{
			stacktop[-2].uintValue =
				(ILUInt32)(StringToBuffer(tempptr))[tempNum];
			MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
		}
		else
		{
			ARRAY_INDEX_EXCEPTION();
		}
	}
	END_NULL_CHECK();
}
VMBREAK(COP_PREFIX_STRING_GET_CHAR);

/**
 * <opcode name="type_from_handle" group="Inline methods">
 *   <operation>Get a type object from its runtime handle</operation>
 *
 *   <format>prefix<fsep/>type_from_handle</format>
 *   <dformat>{type_from_handle}</dformat>
 *
 *   <form name="type_from_handle" code="COP_PREFIX_TYPE_FROM_HANDLE"/>
 *
 *   <before>..., handle</before>
 *   <after>..., object</after>
 *
 *   <description>The <i>handle</i> is popped from the stack as the
 *   type <code>ptr</code>.  It is interpreted as an instance of
 *   the value type <code>System.RuntimeTypeHandle</code>.  The
 *   <i>handle</i> is converted into an <i>object</i> instance
 *   of the reference type <code>System.Type</code>.  The <i>object</i>
 *   is pushed onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Type.GetTypeFromHandle(RuntimeTypeHandle)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_TYPE_FROM_HANDLE):
{
	/* Get a character from a string */
	tempptr = stacktop[-1].ptrValue;
	if(tempptr != 0)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue =
			(void *)(_ILGetClrType(thread, (ILClass *)tempptr));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_TYPE_FROM_HANDLE);

/**
 * <opcode name="monitor_enter" group="Inline methods">
 *   <operation>Enter a monitor on an object</operation>
 *
 *   <format>prefix<fsep/>monitor_enter</format>
 *   <dformat>{monitor_enter}</dformat>
 *
 *   <form name="monitor_enter" code="COP_PREFIX_MONITOR_ENTER"/>
 *
 *   <before>..., object</before>
 *   <after>...</after>
 *
 *   <description>The <i>object</i> is popped from the stack as the
 *   type <code>ptr</code>.  The current thread is made to enter the
 *   synchronisation monitor on <i>object</i>.  Execution continues
 *   once the monitor has been acquired.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Monitor.Enter(Object)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_MONITOR_ENTER):
{
	/* Enter a monitor on an object */
	/* TODO: Actually make it fully inline :) */
	
	BEGIN_NATIVE_CALL();

	COPY_STATE_TO_THREAD();
	_IL_Monitor_Enter(thread, (ILObject *)stacktop[-1].ptrValue);	
	RESTORE_STATE_FROM_THREAD();

	END_NATIVE_CALL();

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_MONITOR_ENTER);

/**
 * <opcode name="monitor_exit" group="Inline methods">
 *   <operation>Exit a monitor on an object</operation>
 *
 *   <format>prefix<fsep/>monitor_exit</format>
 *   <dformat>{monitor_exit}</dformat>
 *
 *   <form name="monitor_exit" code="COP_PREFIX_MONITOR_EXIT"/>
 *
 *   <before>..., object</before>
 *   <after>...</after>
 *
 *   <description>The <i>object</i> is popped from the stack as the
 *   type <code>ptr</code>.  The current thread is made to exit the
 *   synchronisation monitor on <i>object</i>.  Execution continues
 *   once the monitor has been released.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Monitor.Exit(Object)</code> method.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_MONITOR_EXIT):
{
	/* Exit a monitor on an object */
	/* TODO: Actually make it fully inline :) */
	
	BEGIN_NATIVE_CALL();

	COPY_STATE_TO_THREAD();
	_IL_Monitor_Exit(thread, (ILObject *)stacktop[-1].ptrValue);	
	RESTORE_STATE_FROM_THREAD();

	END_NATIVE_CALL();

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_MONITOR_EXIT);

/**
 * <opcode name="append_char" group="Inline methods">
 *   <operation>Append a character to a string builder</operation>
 *
 *   <format>prefix<fsep/>append_char<fsep/>method</format>
 *   <dformat>{append_char}<fsep/>method</dformat>
 *
 *   <form name="append_char" code="COP_PREFIX_APPEND_CHAR"/>
 *
 *   <before>..., builder, ch</before>
 *   <after>..., builder</after>
 *
 *   <description>The <i>builder</i> and <i>ch</i> are popped from the
 *   stack as the types <code>ptr</code> and <code>int32</code> respectively.
 *   The character <i>ch</i> is appended to the end of the string builder
 *   indicated by <i>builder</i>.  The <i>builder</i> is then pushed
 *   back onto the stack.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>StringBuilder.Append(char)</code> method.  The <i>method</i>
 *   argument must be a pointer to this method, because the interpreter
 *   will "bail out" to the C# class library if the append is too
 *   difficult to perform (e.g. the string must be reallocated).</notes>
 *
 *   <exceptions>
 *     <exception name="System.NullReferenceException">Raised if
 *     <i>builder</i> is <code>null</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_APPEND_CHAR):
{
	/* Append a character to a string builder */
	BEGIN_NULL_CHECK(stacktop[-2].ptrValue)
	{
		builder = (System_Text_StringBuilder *)(stacktop[-2].ptrValue);
		if(!(builder->needsCopy) &&
		   builder->buildString->length < builder->buildString->capacity)
		{
			/* We can insert the character into the string directly */
			(StringToBuffer(builder->buildString))
				[(builder->buildString->length)++] =
					(ILUInt16)(stacktop[-1].intValue);
			MODIFY_PC_AND_STACK(CVMP_LEN_PTR, -1);
		}
		else
		{
			/* We need to reallocate the builder, so call the C# library */
			ILExecValue tempValue;
			tempValue.int32Value = stacktop[-1].intValue;
			COPY_STATE_TO_THREAD();
			ILExecThreadCallVirtualV(thread, CVMP_ARG_PTR(ILMethod *),
									 &tempValue, stacktop[-2].ptrValue,
									 &tempValue);
			RESTORE_STATE_FROM_THREAD();
			stacktop[-2].ptrValue = tempValue.ptrValue;
			MODIFY_PC_AND_STACK(CVMP_LEN_PTR, -1);
		}
	}
	END_NULL_CHECK()
}
VMBREAK(COP_PREFIX_APPEND_CHAR);

/**
 * <opcode name="is_white_space" group="Inline methods">
 *   <operation>Determine if a character is white space</operation>
 *
 *   <format>prefix<fsep/>is_white_space</format>
 *   <dformat>{is_white_space}</dformat>
 *
 *   <form name="is_white_space" code="COP_PREFIX_IS_WHITE_SPACE"/>
 *
 *   <before>..., ch</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>ch</i> is popped from the stack as the
 *   type <code>int32</code>.  If it is a white space character,
 *   then the <code>int32</code> <i>result</i> 1 is pushed onto
 *   the stack; otherwise 0 is pushed.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Char.IsWhiteSpace(char)</code> method, which is used
 *   heavily in text processing code.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_IS_WHITE_SPACE):
{
	/* Determine if a character is white space */
	position = stacktop[-1].intValue;
	if(position == 0x0009 || position == 0x0020 || position == 0x000a ||
	   position == 0x000b || position == 0x000c || position == 0x000d ||
	   position == 0x0085 || position == 0x2028 || position == 0x2029)
	{
		stacktop[-1].intValue = 1;
	}
	else if(position < 0x0080)
	{
		stacktop[-1].intValue = 0;
	}
	else
	{
		stacktop[-1].intValue =
			(ILGetUnicodeCategory((unsigned)position) ==
					ILUnicode_SpaceSeparator);
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_IS_WHITE_SPACE);

/**
 * <opcode name="copy_AAi4" group="Inline methods">
 *   <operation>copy elements from one vector to an other</operation>
 *
 *   <format>prefix<fsep/>copy_AAi4</format>
 *   <dformat>{copy_AAi4}</dformat>
 *
 *   <form name="copy_AAi4" code="COP_PREFIX_SARRAY_COPY_AAI4"/>
 *
 *   <before>..., src, dest, length</before>
 *   <after>..., </after>
 *
 *   <description>The <i>src</i>, <i>dest</i> and <i>length</i>are 
 *   popped from the stack as the types <code>ptr</code>, <code>ptr</code>
 *   and <code>int32</code> respectively. <i>Length</i> elements are copied
 *   from <i>dest</i> to <i>src</i> starting at index 0.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Array.Copy(src, dest, length)</code> method where the elements
 *   of the arrays can be determined at verification time.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_SARRAY_COPY_AAI4):
{
	COPY_STATE_TO_THREAD();
	ILSArrayCopy_AAI4(thread,
					  (System_Array *)stacktop[-3].ptrValue,
					  (System_Array *)stacktop[-2].ptrValue,
					  stacktop[-1].intValue, CVM_ARG_WORD);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD, -3);
}
VMBREAK(COP_PREFIX_SARRAY_COPY_AAI4);

/**
 * <opcode name="copy_Ai4Ai4i4" group="Inline methods">
 *   <operation>copy elements from one vector to an other</operation>
 *
 *   <format>prefix<fsep/>copy_Ai4Ai4i4</format>
 *   <dformat>{copy_Ai4Ai4i4}</dformat>
 *
 *   <form name="copy_Ai4Ai4i4" code="COP_PREFIX_SARRAY_COPY_AI4AI4I4"/>
 *
 *   <before>..., src, srcIndex, dest, destIndex, length</before>
 *   <after>..., </after>
 *
 *   <description>The <i>src</i>, <i>srcIndex</i>, <i>dest</i>,
 *   <i>destIndex</i> and <i>length</i> are popped from the stack as the
 *   types <code>ptr</code>, <code>int32</code>, <code>ptr</code>,
 *   <code>int32</code> and <code>int32</code> respectively.
 *   <i>Length</i> elements are copied from <i>dest</i> starting at
 *   <i>destIndex</i> to <i>src</i> starting at <i>srcIndex</i>.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Array.Copy(src, srcIndex, dest, destIndex, length)</code> method
 *   where the elements of the arrays can be determined at verification time.
 *   </notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_SARRAY_COPY_AI4AI4I4):
{
	COPY_STATE_TO_THREAD();
	ILSArrayCopy_AI4AI4I4(thread,
						  (System_Array *)stacktop[-5].ptrValue,
						  stacktop[-4].intValue,
						  (System_Array *)stacktop[-3].ptrValue,
						  stacktop[-2].intValue,
						  stacktop[-1].intValue, CVM_ARG_WORD);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD, -5);
}
VMBREAK(COP_PREFIX_SARRAY_COPY_AI4AI4I4);

/**
 * <opcode name="clear_Ai4I4" group="Inline methods">
 *   <operation>clear elements in a vector</operation>
 *
 *   <format>prefix<fsep/>clear_Ai4I4</format>
 *   <dformat>{clear_Ai4I4}</dformat>
 *
 *   <form name="clear_Ai4I4" code="COP_PREFIX_SARRAY_CLEAR_AI4I4"/>
 *
 *   <before>..., array, index, length</before>
 *   <after>..., </after>
 *
 *   <description>The <i>array</i>, <i>index</i> and <i>length</i>are 
 *   popped from the stack as the types <code>ptr</code>, <code>int32</code>
 *   and <code>int32</code> respectively. <i>Length</i> elements are cleared
 *   (set to 0) in <i>array</i> starting at <i>index</i>.</description>
 *
 *   <notes>This instruction is used to inline calls to the
 *   <code>Array.Clear(array, index, length)</code> method where the element
 *   of the array can be determined at verification time.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_SARRAY_CLEAR_AI4I4):
{
	COPY_STATE_TO_THREAD();
	ILSArrayClear_AI4I4(thread,
						(System_Array *)stacktop[-3].ptrValue,
						stacktop[-2].intValue,
						stacktop[-1].intValue, CVM_ARG_WORD);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD, -3);
}
VMBREAK(COP_PREFIX_SARRAY_CLEAR_AI4I4);

/**
 * <opcode name="abs_i4" group="Inline methods">
 *   <operation>Compute the absolute value of an int</operation>
 *
 *   <format>prefix<fsep/>abs_i4</format>
 *   <dformat>{abs_i4}</dformat>
 *
 *   <form name="abs_i4" code="COP_PREFIX_ABS_I4"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_ABS_I4):
{
	tempI4 = stacktop[-1].intValue;
	
	if (tempI4 >= 0)
	{
		/* Value is ok */
	}
	else if (tempI4 != IL_MIN_INT32)
	{
		stacktop[-1].intValue = -tempI4;
	}
	else
	{
		COPY_STATE_TO_THREAD();
		ILExecThreadThrowSystem(thread, "System.OverflowException", "Overflow_NegateTwosCompNum");
		RESTORE_STATE_FROM_THREAD();
	}

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ABS_I4);

/**
 * <opcode name="min_i4" group="Inline methods">
 *   <operation>Compute the minimum of two numbers</operation>
 *
 *   <format>prefix<fsep/>min_i4</format>
 *   <dformat>{min_i4}</dformat>
 *
 *   <form name="min_i4" code="COP_PREFIX_MIN_I4"/>
 *
 *   <before>..., int</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_MIN_I4):
{
	stacktop[-2].intValue =
		stacktop[-1].intValue < stacktop[-2].intValue 
			?	stacktop[-1].intValue 
				: stacktop[-2].intValue;

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_MIN_I4);

/**
 * <opcode name="max_i4" group="Inline methods">
 *   <operation>Compute the maximum of two numbers</operation>
 *
 *   <format>prefix<fsep/>max_i4</format>
 *   <dformat>{min_i4}</dformat>
 *
 *   <form name="max_i4" code="COP_PREFIX_MAX_I4"/>
 *
 *   <before>...,  int</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_MAX_I4):
{
	stacktop[-2].intValue = 
		stacktop[-1].intValue > stacktop[-2].intValue 
			?	stacktop[-1].intValue 
				: stacktop[-2].intValue;

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -1);
}
VMBREAK(COP_PREFIX_MAX_I4);

/**
 * <opcode name="sign_i4" group="Inline methods">
 *   <operation>Compute the sign of an int</operation>
 *
 *   <format>prefix<fsep/>sign_i4</format>
 *   <dformat>{sign_i4}</dformat>
 *
 *   <form name="sign_i4" code="COP_PREFIX_SIGN_I4"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_SIGN_I4):
{
	tempI4 = stacktop[-1].intValue;

	if (tempI4 > 0)
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = 1;
	}
	else if (tempI4 < 0)
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = -1;
	}
	else
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = 0;
	}

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_SIGN_I4);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="asin" group="Inline methods">
 *   <operation>Compute the angle whose sine is the specified number</operation>
 *
 *   <format>prefix<fsep/>asin</format>
 *   <dformat>{asin}</dformat>
 *
 *   <form name="asin" code="COP_PREFIX_ASIN"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(ASIN, Asin);

/**
 * <opcode name="atan" group="Inline methods">
 *   <operation>Compute the angle whose tangent is the specified number</operation>
 *
 *   <format>prefix<fsep/>atan</format>
 *   <dformat>{atan}</dformat>
 *
 *   <form name="atan" code="COP_PREFIX_ATAN"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(ATAN, Atan);

/**
 * <opcode name="atan2" group="Inline methods">
 *   <operation>Compute the angle whose tangent is quotient of the two specified numbers</operation>
 *
 *   <format>prefix<fsep/>atan2</format>
 *   <dformat>{atan2}</dformat>
 *
 *   <form name="atan2" code="COP_PREFIX_ATAN"/>
 *
 *   <before>..., number1, number2</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE_2(ATAN2, Atan2);

/**
 * <opcode name="ceiling" group="Inline methods">
 *   <operation>Returns the smallest whole number greater than or equal to the specified number</operation>
 *
 *   <format>prefix<fsep/>ceiling</format>
 *   <dformat>{ceiling}</dformat>
 *
 *   <form name="ceiling" code="COP_PREFIX_CEILING"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(CEILING, Ceiling);

/**
 * <opcode name="cos" group="Inline methods">
 *   <operation>Compute the cosine of the specified angle</operation>
 *
 *   <format>prefix<fsep/>cos</format>
 *   <dformat>{cos}</dformat>
 *
 *   <form name="cos" code="COP_PREFIX_COS"/>
 *
 *   <before>..., angle</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(COS, Cos);

/**
 * <opcode name="cosh" group="Inline methods">
 *   <operation>Returns the hyperbolic cosine of the specified angle</operation>
 *
 *   <format>prefix<fsep/>cosh</format>
 *   <dformat>{cosh}</dformat>
 *
 *   <form name="cosh" code="COP_PREFIX_COSH"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(COSH, Cosh);

/**
 * <opcode name="exp" group="Inline methods">
 *   <operation>Computes e raised to the specified power</operation>
 *
 *   <format>prefix<fsep/>exp</format>
 *   <dformat>{exp}</dformat>
 *
 *   <form name="exp" code="COP_PREFIX_EXP"/>
 *
 *   <before>..., power</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(EXP, Exp);

/**
 * <opcode name="floor" group="Inline methods">
 *   <operation>Computes the largest whole number less than or equal to the specified number</operation>
 *
 *   <format>prefix<fsep/>floor</format>
 *   <dformat>{floor}</dformat>
 *
 *   <form name="floor" code="COP_PREFIX_FLOOR"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(FLOOR, Floor);

/**
 * <opcode name="ieeeremainder" group="Inline methods">
 *   <operation>Computes the remainder resulting from the division of 
 *   a specified number by another specified number.</operation>
 *
 *   <format>prefix<fsep/>ieeeremainder</format>
 *   <dformat>{ieeeremainder}</dformat>
 *
 *   <form name="ieeeremainder" code="COP_PREFIX_IEEEREMAINDER"/>
 *
 *   <before>..., number1, number2</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE_2(IEEEREMAINDER, IEEERemainder);

/**
 * <opcode name="log" group="Inline methods">
 *   <operation>Computes the natural (base e) logarithm of the specified number</operation>
 *
 *   <format>prefix<fsep/>log</format>
 *   <dformat>{log}</dformat>
 *
 *   <form name="log" code="COP_PREFIX_LOG"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(LOG, Log);

/**
 * <opcode name="log10" group="Inline methods">
 *   <operation>Computes the base 10 logarithm of the specified number</operation>
 *
 *   <format>prefix<fsep/>log10</format>
 *   <dformat>{log10}</dformat>
 *
 *   <form name="log10" code="COP_PREFIX_LOG10"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(LOG10, Log10);

/**
 * <opcode name="pow" group="Inline methods">
 *   <operation>Computes the given number raised to a specific power</operation>
 *
 *   <format>prefix<fsep/>pow</format>
 *   <dformat>{pow}</dformat>
 *
 *   <form name="pow" code="COP_PREFIX_POW"/>
 *
 *   <before>..., number, power</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE_2(POW, Pow);

/**
 * <opcode name="round" group="Inline methods">
 *   <operation>Computes the whole number nearest to the specified number</operation>
 *
 *   <format>prefix<fsep/>round</format>
 *   <dformat>{round}</dformat>
 *
 *   <form name="round" code="COP_PREFIX_ROUND"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(ROUND, Round);

/**
 * <opcode name="sin" group="Inline methods">
 *   <operation>Compute the sine of the specified angle</operation>
 *
 *   <format>prefix<fsep/>sin</format>
 *   <dformat>{sin}</dformat>
 *
 *   <form name="sin" code="COP_PREFIX_SIN"/>
 *
 *   <before>..., angle</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(SIN, Sin);

/**
 * <opcode name="sinh" group="Inline methods">
 *   <operation>Computes the hyperbolic sine of the specified angle</operation>
 *
 *   <format>prefix<fsep/>sinh</format>
 *   <dformat>{sinh}</dformat>
 *
 *   <form name="sinh" code="COP_PREFIX_SINH"/>
 *
 *   <before>..., angle</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(SINH, Sinh);

/**
 * <opcode name="sqrt" group="Inline methods">
 *   <operation>Computes the square root of the specified number</operation>
 *
 *   <format>prefix<fsep/>sqrt</format>
 *   <dformat>{sqrt}</dformat>
 *
 *   <form name="sqrt" code="COP_PREFIX_SQRT"/>
 *
 *   <before>..., number</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(SQRT, Sqrt);

/**
 * <opcode name="tan" group="Inline methods">
 *   <operation>Computes the tangent of the specified angle</operation>
 *
 *   <format>prefix<fsep/>tan</format>
 *   <dformat>{tan}</dformat>
 *
 *   <form name="tan" code="COP_PREFIX_TAN"/>
 *
 *   <before>..., angle</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(TAN, Tan);

/**
 * <opcode name="tanh" group="Inline methods">
 *   <operation>Computes the hyperbolic tangent of the specified angle</operation>
 *
 *   <format>prefix<fsep/>tanh</format>
 *   <dformat>{tanh}</dformat>
 *
 *   <form name="tanh" code="COP_PREFIX_TANH"/>
 *
 *   <before>..., angle</before>
 *   <after>..., result</after>
 * </opcode>
 */
COP_PREFIX_MATH_CASE(TANH, Tanh);

/**
 * <opcode name="min_r4" group="Inline methods">
 *   <operation>Compute the minimum of two numbers</operation>
 *
 *   <format>prefix<fsep/>min_r4</format>
 *   <dformat>{min_r4}</dformat>
 *
 *   <form name="min_r4" code="COP_PREFIX_MIN_R4"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_MIN_R4):
{
	WriteFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)], 
			ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) < ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)])
				?	ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) 
					: ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)]));

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_PREFIX_MIN_R4);

/**
 * <opcode name="max_r4" group="Inline methods">
 *   <operation>Compute the maximum of two floats</operation>
 *
 *   <format>prefix<fsep/>max_r4</format>
 *   <dformat>{max_r4}</dformat>
 *
 *   <form name="max_r4" code="COP_PREFIX_MAX_R4"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_MAX_R4):
{
	WriteFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)], 
			ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) > ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)])
				?	ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) 
					: ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)]));

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_PREFIX_MAX_R4);

/**
 * <opcode name="min_r8" group="Inline methods">
 *   <operation>Compute the minimum of two doubles</operation>
 *
 *   <format>prefix<fsep/>min_r8</format>
 *   <dformat>{min_r8}</dformat>
 *
 *   <form name="min_r8" code="COP_PREFIX_MIN_R8"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_MIN_R8):
{
	WriteFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)], 
			ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) < ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)])
				?	ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) 
					: ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)]));

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_PREFIX_MIN_R8);

/**
 * <opcode name="max_r8" group="Inline methods">
 *   <operation>Compute the maximum of two doubles</operation>
 *
 *   <format>prefix<fsep/>max_r8</format>
 *   <dformat>{max_r8}</dformat>
 *
 *   <form name="max_r8" code="COP_PREFIX_MAX_R8"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_MAX_R8):
{
	WriteFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)], 
			ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) > ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)])
				?	(ILNativeFloat)(ILDouble)ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]) 
					: (ILNativeFloat)(ILDouble)ReadFloat(&stacktop[-(CVM_WORDS_PER_NATIVE_FLOAT << 1)]));

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_PREFIX_MAX_R8);

/**
 * <opcode name="sign_r4" group="Inline methods">
 *   <operation>Compute the sign of a float</operation>
 *
 *   <format>prefix<fsep/>sign_r4</format>
 *   <dformat>{sign_r4}</dformat>
 *
 *   <form name="sign_r4" code="COP_PREFIX_SIGN_R4"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_SIGN_R4):
{
	tempR8 = ((ILDouble)ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]));

	if (tempR8 > 0.0)
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = 1;
	}
	else if (tempR8 < 0.0)
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = -1;
	}
	else
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = 0;
	}

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT + 1);
}
VMBREAK(COP_PREFIX_SIGN_R4);

/**
 * <opcode name="sign_r8" group="Inline methods">
 *   <operation>Compute the sign of a double</operation>
 *
 *   <format>prefix<fsep/>sign_r8</format>
 *   <dformat>{sign_r8}</dformat>
 *
 *   <form name="sign_r8" code="COP_PREFIX_SIGN_R8"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_SIGN_R8):
{
	tempR8 = ((ILDouble)ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]));

	if (tempR8 > 0.0)
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = 1;
	}
	else if (tempR8 < 0.0)
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = -1;
	}
	else
	{
		stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = 0;
	}

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, -CVM_WORDS_PER_NATIVE_FLOAT + 1);
}
VMBREAK(COP_PREFIX_SIGN_R8);

/**
 * <opcode name="abs_r4" group="Inline methods">
 *   <operation>Compute the absolute value of a float</operation>
 *
 *   <format>prefix<fsep/>abs_r4</format>
 *   <dformat>{abs_r4}</dformat>
 *
 *   <form name="abs_r4" code="COP_PREFIX_ABS_R4"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_ABS_R4):
{
	tempR8 = ((ILDouble)ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]));

	if (tempR8 < 0.0)
	{
		WriteFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT], (ILNativeFloat)-tempR8);
	}

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ABS_R4);

/**
 * <opcode name="abs_r8" group="Inline methods">
 *   <operation>Compute the absolute value of a double</operation>
 *
 *   <format>prefix<fsep/>abs_r8</format>
 *   <dformat>{abs_r8}</dformat>
 *
 *   <form name="abs_r8" code="COP_PREFIX_ABS_R8"/>
 *
 *   <before>..., db</before>
 *   <after>..., result</after>
 * </opcode>
 */
VMCASE(COP_PREFIX_ABS_R8):
{
	tempR8 = ((ILDouble)ReadFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]));

	if (tempR8 < 0.0)
	{		
		WriteFloat(&stacktop[-CVM_WORDS_PER_NATIVE_FLOAT], (ILNativeFloat)-tempR8);
	}

	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ABS_R8);

#else /* !IL_CONFIG_FP_SUPPORTED */
/*
 * Stub out floating-point instructions.
 */

VMCASE(COP_PREFIX_ABS_R4):
VMCASE(COP_PREFIX_ABS_R8):
VMCASE(COP_PREFIX_ASIN):
VMCASE(COP_PREFIX_ATAN):
VMCASE(COP_PREFIX_ATAN2):
VMCASE(COP_PREFIX_CEILING):
VMCASE(COP_PREFIX_COS):
VMCASE(COP_PREFIX_COSH):
VMCASE(COP_PREFIX_EXP):
VMCASE(COP_PREFIX_FLOOR):
VMCASE(COP_PREFIX_IEEEREMAINDER):
VMCASE(COP_PREFIX_LOG):
VMCASE(COP_PREFIX_LOG10):
VMCASE(COP_PREFIX_MIN_R4):
VMCASE(COP_PREFIX_MAX_R4):
VMCASE(COP_PREFIX_MIN_R8):
VMCASE(COP_PREFIX_MAX_R8):
VMCASE(COP_PREFIX_POW):
VMCASE(COP_PREFIX_ROUND):
VMCASE(COP_PREFIX_SIGN_R4):
VMCASE(COP_PREFIX_SIGN_R8):
VMCASE(COP_PREFIX_SIN):
VMCASE(COP_PREFIX_SINH):
VMCASE(COP_PREFIX_SQRT):
VMCASE(COP_PREFIX_TAN):
VMCASE(COP_PREFIX_TANH):
{
	COPY_STATE_TO_THREAD();
	stacktop[0].ptrValue =
		_ILSystemException(thread, "System.NotImplementedException");
	stacktop += 1;
	goto throwException;
}
/* Not reached */

#endif /* IL_CONFIG_FP_SUPPORTED */

#endif /* IL_CVM_PREFIX */
