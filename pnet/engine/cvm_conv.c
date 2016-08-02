/*
 * cvm_conv.c - Opcodes for converting between data types.
 *
 * Copyright (C) 2001, 2011  Southern Storm Software, Pty Ltd.
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

#ifdef IL_CONFIG_FP_SUPPORTED

/*
 * Convert "ulong" into "native float".
 *
 * Some platforms cannot perform the conversion directly,
 * so we need to do it in stages.
 */
static IL_INLINE ILNativeFloat LU2F(ILUInt64 value)
{
	return ILUInt64ToFloat(value);
}

/*
 * Convert "native float" into "ulong".
 *
 * Some platforms cannot perform the conversion directly,
 * so we need to do it in stages.
 */
static IL_INLINE ILUInt64 F2LU(ILNativeFloat value)
{
	return ILFloatToUInt64(value);
}

#endif /* IL_CONFIG_FP_SUPPORTED */

/*
 * Convert "long" into "int" with overflow testing.
 */
static IL_INLINE int L2IOvf(CVMWord *posn)
{
	ILInt64 longValue = ReadLong(posn);
	if(longValue >= (ILInt64)IL_MIN_INT32 &&
	   longValue <= (ILInt64)IL_MAX_INT32)
	{
		posn->intValue = (ILInt32)longValue;
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Convert "long" into "uint" with overflow testing.
 */
static IL_INLINE int L2UIOvf(CVMWord *posn)
{
	ILInt64 longValue = ReadLong(posn);
	if(longValue >= 0 &&
	   longValue <= (ILInt64)IL_MAX_UINT32)
	{
		posn->uintValue = (ILUInt32)longValue;
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Convert "ulong" into "int" with overflow testing.
 */
static IL_INLINE int LU2IOvf(CVMWord *posn)
{
	ILUInt64 ulongValue = ReadULong(posn);
	if(ulongValue <= (ILUInt64)IL_MAX_INT32)
	{
		posn->intValue = (ILInt32)ulongValue;
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Convert "ulong" into "uint" with overflow testing.
 */
static IL_INLINE int LU2UIOvf(CVMWord *posn)
{
	ILUInt64 ulongValue = ReadULong(posn);
	if(ulongValue <= (ILUInt64)IL_MAX_UINT32)
	{
		posn->uintValue = (ILUInt32)ulongValue;
		return 1;
	}
	else
	{
		return 0;
	}
}

#ifdef IL_CONFIG_FP_SUPPORTED

/*
 * Convert "native float" into "int" with overflow testing.
 */
static IL_INLINE int F2IOvf(CVMWord *posn)
{
	ILNativeFloat value = ReadFloat(posn);
	if(ILNativeFloatIsFinite(value))
	{
		if(value > (ILNativeFloat)(-2147483649.0) &&
		   value < (ILNativeFloat)2147483648.0)
		{
			posn->intValue = (ILInt32)value;
			return 1;
		}
	}
	return 0;
}

/*
 * Convert "native float" into "uint" with overflow testing.
 */
static IL_INLINE int F2UIOvf(CVMWord *posn)
{
	ILNativeFloat value = ReadFloat(posn);
	if(ILNativeFloatIsFinite(value))
	{
		if(value >= (ILNativeFloat)0.0 &&
		   value < (ILNativeFloat)4294967296.0)
		{
			posn->uintValue = (ILUInt32)value;
			return 1;
		}
	}
	return 0;
}

/*
 * Convert "native float" into "long" with overflow testing.
 */
static IL_INLINE int F2LOvf(CVMWord *posn)
{
	ILInt64 result;
	if(ILFloatToInt64Ovf(&result, ReadFloat(posn)))
	{
		WriteLong(posn, result);
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Convert "native float" into "ulong" with overflow testing.
 */
static IL_INLINE int F2LUOvf(CVMWord *posn)
{
	ILUInt64 result;
	if(ILFloatToUInt64Ovf(&result, ReadFloat(posn)))
	{
		WriteULong(posn, result);
		return 1;
	}
	else
	{
		return 0;
	}
}

#endif /* IL_CONFIG_FP_SUPPORTED */

#ifdef IL_CONFIG_PINVOKE

/*
 * Convert a reference to a string array into a pointer to a C array.
 */
static void *RefArrayToC(ILExecThread *thread, void *ref,
						 char *(*conv)(ILExecThread *thread, ILString *str),
						 int needRefPtr)
{
	void *result;
	System_Array *array;
	void **newArray;
	ILInt32 index;

	/* Process the NULL pointer case */
	if(!ref || !(*((void **)ref)))
	{
		/* Return a pointer to a NULL pointer for the C array */
		result = ILGCAlloc(sizeof(void *));
		if(result)
		{
			*((void **)result) = 0;
		}
		return result;
	}

	/* Extract the string array and then create a new C array */
	if(needRefPtr)
	{
		array = *((System_Array **)ref);
	}
	else
	{
		array = (System_Array *)ref;
	}
	result = ILGCAlloc(sizeof(void *) * (ArrayLength(array) + 2));
	if(!result)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}

	/* We use the first element to store the reference to the array */
	((void **)result)[0] = (void *)(&(((void **)result)[1]));

	/* Copy the array elements */
	newArray = &(((void **)result)[1]);
	for(index = 0; index < ArrayLength(array); ++index)
	{
		*newArray = (void *)((*conv)
			(thread, ((ILString **)(ArrayToBuffer(array)))[index]));
		if(_ILExecThreadHasException(thread))
		{
			return 0;
		}
		++newArray;
	}
	*newArray = 0;

	/* Return the new reference to the caller */
	if(needRefPtr)
	{
		return result;
	}
	else
	{
		return (void *)(((void **)result) + 1);
	}
}

#endif /* IL_CONFIG_PINVOKE */

#elif defined(IL_CVM_LOCALS)

ILInt32 position;

#elif defined(IL_CVM_MAIN)

/**
 * <opcode name="i2b" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int8</code></operation>
 *
 *   <format>i2b</format>
 *   <dformat>{i2b}</dformat>
 *
 *   <form name="i2b" code="COP_I2B"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 8 bits and then
 *   sign-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_I2B):
{
	/* Convert from integer to signed byte */
	stacktop[-1].intValue = (ILInt32)(ILInt8)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_I2B);

/**
 * <opcode name="i2ub" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>uint8</code></operation>
 *
 *   <format>i2ub</format>
 *   <dformat>{i2ub}</dformat>
 *
 *   <form name="i2ub" code="COP_I2UB"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 8 bits and then
 *   zero-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_I2UB):
{
	/* Convert from integer to unsigned byte */
	stacktop[-1].intValue = (ILInt32)(ILUInt8)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_I2UB);

/**
 * <opcode name="i2s" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int16</code></operation>
 *
 *   <format>i2s</format>
 *   <dformat>{i2s}</dformat>
 *
 *   <form name="i2s" code="COP_I2S"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 16 bits and then
 *   sign-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_I2S):
{
	/* Convert from integer to signed short */
	stacktop[-1].intValue = (ILInt32)(ILInt16)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_I2S);

/**
 * <opcode name="i2us" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>uint16</code></operation>
 *
 *   <format>i2us</format>
 *   <dformat>{i2us}</dformat>
 *
 *   <form name="i2us" code="COP_I2US"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 16 bits and then
 *   zero-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_I2US):
{
	/* Convert from integer to unsigned short */
	stacktop[-1].intValue = (ILInt32)(ILUInt16)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_I2US);

/**
 * <opcode name="i2l" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int64</code></operation>
 *
 *   <format>i2l</format>
 *   <dformat>{i2l}</dformat>
 *
 *   <form name="i2l" code="COP_I2L"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int64</code> <i>result</i>
 *   is formed by sign-extending <i>value</i> to 64 bits.
 *   The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_I2L):
{
	/* Convert from integer to long */
	WriteLong(&(stacktop[-1]), (ILInt64)(stacktop[-1].intValue));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, CVM_WORDS_PER_LONG - 1);
}
VMBREAK(COP_I2L);

/**
 * <opcode name="iu2l" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>int64</code></operation>
 *
 *   <format>iu2l</format>
 *   <dformat>{iu2l}</dformat>
 *
 *   <form name="iu2l" code="COP_IU2L"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  The <code>int64</code> <i>result</i>
 *   is formed by zero-extending <i>value</i> to 64 bits.
 *   The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_IU2L):
{
	/* Convert from unsigned integer to long */
	WriteLong(&(stacktop[-1]), (ILInt64)(stacktop[-1].uintValue));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, CVM_WORDS_PER_LONG - 1);
}
VMBREAK(COP_IU2L);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="i2f" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to
 *				<code>native float</code></operation>
 *
 *   <format>i2f</format>
 *   <dformat>{i2f}</dformat>
 *
 *   <form name="i2f" code="COP_I2F"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>, and converted into a <code>native float</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_I2F):
{
	/* Convert from integer to "native float" */
	WriteFloat(&(stacktop[-1]), (ILNativeFloat)(stacktop[-1].intValue));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, CVM_WORDS_PER_NATIVE_FLOAT - 1);
}
VMBREAK(COP_I2F);

/**
 * <opcode name="iu2f" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to
 *				<code>native float</code></operation>
 *
 *   <format>iu2f</format>
 *   <dformat>{iu2f}</dformat>
 *
 *   <form name="iu2f" code="COP_IU2F"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>, and converted into a <code>native float</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_IU2F):
{
	/* Convert from unsigned integer to "native float" */
	WriteFloat(&(stacktop[-1]), (ILNativeFloat)(stacktop[-1].uintValue));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, CVM_WORDS_PER_NATIVE_FLOAT - 1);
}
VMBREAK(COP_IU2F);

#endif /* IL_CONFIG_FP_SUPPORTED */

/**
 * <opcode name="l2i" group="Conversion operators">
 *   <operation>Convert <code>int64</code> to <code>int32</code></operation>
 *
 *   <format>l2i</format>
 *   <dformat>{l2i}</dformat>
 *
 *   <form name="l2i" code="COP_L2I"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int64</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 32 bits.
 *   The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_L2I):
{
	/* Convert from long to integer */
	stacktop[-CVM_WORDS_PER_LONG].intValue =
		(ILInt32)(ReadLong(&(stacktop[-CVM_WORDS_PER_LONG])));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -(CVM_WORDS_PER_LONG - 1));
}
VMBREAK(COP_L2I);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="l2f" group="Conversion operators">
 *   <operation>Convert <code>int64</code> to
 *				<code>native float</code></operation>
 *
 *   <format>l2f</format>
 *   <dformat>{l2f}</dformat>
 *
 *   <form name="l2f" code="COP_L2F"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int64</code>, and converted into a <code>native float</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <notes>The precision of the <code>native float</code> type is
 *   platform-dependent.  On some platforms, it may be sufficient to
 *   represent all <code>int64</code> values, and on other platforms it
 *   may round large values.  Programs should not rely upon precise
 *   conversions from <code>int64</code> to <code>native float</code>.</notes>
 * </opcode>
 */
VMCASE(COP_L2F):
{
	/* Convert from long to "native float" */
	WriteFloat(&(stacktop[-CVM_WORDS_PER_LONG]),
	   (ILNativeFloat)(ReadLong(&(stacktop[-CVM_WORDS_PER_LONG]))));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, CVM_WORDS_PER_NATIVE_FLOAT -
						   CVM_WORDS_PER_LONG);
}
VMBREAK(COP_L2F);

/**
 * <opcode name="lu2f" group="Conversion operators">
 *   <operation>Convert <code>uint64</code> to
 *				<code>native float</code></operation>
 *
 *   <format>lu2f</format>
 *   <dformat>{lu2f}</dformat>
 *
 *   <form name="lu2f" code="COP_LU2F"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint64</code>, and converted into a <code>native float</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <notes>The precision of the <code>native float</code> type is
 *   platform-dependent.  On some platforms, it may be sufficient to
 *   represent all <code>uint64</code> values, and on other platforms it
 *   may round large values.  Programs should not rely upon precise
 *   conversions from <code>uint64</code> to <code>native float</code>.</notes>
 * </opcode>
 */
VMCASE(COP_LU2F):
{
	/* Convert from unsigned long to "native float" */
	WriteFloat(&(stacktop[-CVM_WORDS_PER_LONG]),
			   LU2F(ReadULong(&(stacktop[-CVM_WORDS_PER_LONG]))));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, CVM_WORDS_PER_NATIVE_FLOAT -
						              CVM_WORDS_PER_LONG);
}
VMBREAK(COP_LU2F);

/**
 * <opcode name="f2i" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to
 *				<code>int32</code></operation>
 *
 *   <format>f2i</format>
 *   <dformat>{f2i}</dformat>
 *
 *   <form name="f2i" code="COP_F2I"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>, and converted into an <code>int32</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_F2I):
{
	/* Convert from "native float" to integer */
	stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].intValue = (ILInt32)
		(ReadFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -(CVM_WORDS_PER_NATIVE_FLOAT - 1));
}
VMBREAK(COP_F2I);

/**
 * <opcode name="f2iu" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to
 *				<code>uint32</code></operation>
 *
 *   <format>f2iu</format>
 *   <dformat>{f2iu}</dformat>
 *
 *   <form name="f2iu" code="COP_F2IU"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>, and converted into an <code>uint32</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_F2IU):
{
	/* Convert from "native float" to unsigned integer */
	stacktop[-CVM_WORDS_PER_NATIVE_FLOAT].uintValue = (ILUInt32)
		(ReadFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -(CVM_WORDS_PER_NATIVE_FLOAT - 1));
}
VMBREAK(COP_F2IU);

/**
 * <opcode name="f2l" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to
 *				<code>int64</code></operation>
 *
 *   <format>f2l</format>
 *   <dformat>{f2l}</dformat>
 *
 *   <form name="f2l" code="COP_F2L"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>, and converted into an <code>int64</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_F2L):
{
	/* Convert from "native float" to long */
	WriteLong(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]), (ILInt64)
		ReadFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -(CVM_WORDS_PER_NATIVE_FLOAT -
							 CVM_WORDS_PER_LONG));
}
VMBREAK(COP_F2L);

/**
 * <opcode name="f2lu" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to
 *				<code>uint64</code></operation>
 *
 *   <format>f2lu</format>
 *   <dformat>{f2lu}</dformat>
 *
 *   <form name="f2lu" code="COP_F2LU"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>, and converted into an <code>uint64</code>
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_F2LU):
{
	/* Convert from "native float" to unsigned long */
	WriteULong(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]),
			   F2LU(ReadFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]))));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, -(CVM_WORDS_PER_NATIVE_FLOAT -
							            CVM_WORDS_PER_LONG));
}
VMBREAK(COP_F2LU);

/**
 * <opcode name="f2f" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to
 *				<code>float32</code></operation>
 *
 *   <format>f2f</format>
 *   <dformat>{f2f}</dformat>
 *
 *   <form name="f2f" code="COP_F2F"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>, truncated to <code>float32</code>,
 *   and then converted into a <code>native float</code> <i>result</i>.
 *   The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_F2F):
{
	/* Convert from "native float" to "float" */
	WriteFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]),
		(ILNativeFloat)(ILFloat)
		ReadFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_F2F);

/**
 * <opcode name="f2d" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to
 *				<code>float64</code></operation>
 *
 *   <format>f2d</format>
 *   <dformat>{f2d}</dformat>
 *
 *   <form name="f2d" code="COP_F2D"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>, truncated to <code>float64</code>,
 *   and then converted into a <code>native float</code> <i>result</i>.
 *   The <i>result</i> is pushed onto the stack.</description>
 * </opcode>
 */
VMCASE(COP_F2D):
{
	/* Convert from "native float" to "double" */
	WriteFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT]),
		(ILNativeFloat)(ILDouble)
		ReadFloat(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])));
	MODIFY_PC_AND_STACK(CVM_LEN_NONE, 0);
}
VMBREAK(COP_F2D);

#endif /* IL_CONFIG_FP_SUPPORTED */

/**
 * <opcode name="i2p_lower" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>ptr</code>
 *				at some point lower down on the stack</operation>
 *
 *   <format>i2p_lower<fsep/>N[1]</format>
 *   <format>wide<fsep/>i2p_lower<fsep/>N[4]</format>
 *   <dformat>{i2p_lower}<fsep/>N</dformat>
 *
 *   <form name="f2d" code="COP_F2D"/>
 *
 *   <before>..., value, val1, ..., valN</before>
 *   <after>..., result, val1, ..., valN</after>
 *
 *   <description>The <i>value</i> at stack word <i>N</i> positions
 *   down from the top of the stack is converted from <code>uint32</code>
 *   into a <code>ptr</code> <i>result</i>.  <i>N == 0</i> indicates
 *   that <i>value</i> is on the top of the stack.</description>
 *
 *   <notes>This is typically used to convert CIL "I4" values into "I" values
 *   for use in unmanaged pointer operations.</notes>
 * </opcode>
 */
VMCASE(COP_I2P_LOWER):
{
	/* Convert an I4 value into a pointer value that
	   is lower down on the stack */
	position = -(((ILInt32)CVM_ARG_WIDE_SMALL) + 1);
	stacktop[position].ptrValue =
		(void *)(ILNativeUInt)(stacktop[position].uintValue);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_SMALL, 0);
}
VMBREAK(COP_I2P_LOWER);

#elif defined(IL_CVM_WIDE)

case COP_I2P_LOWER:
{
	/* Convert an I4 value into a pointer value that
	   is lower down on the stack */
	position = -(((ILInt32)CVM_ARG_WIDE_LARGE) + 1);
	stacktop[position].ptrValue =
		(void *)(ILNativeUInt)(stacktop[position].uintValue);
	MODIFY_PC_AND_STACK(CVM_LEN_WIDE_LARGE, 0);
}
VMBREAKNOEND;

#elif defined(IL_CVM_PREFIX)

/**
 * <opcode name="i2b_ovf" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int8</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>i2b_ovf</format>
 *   <dformat>{i2b_ovf}</dformat>
 *
 *   <form name="i2b_ovf" code="COP_PREFIX_I2B_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 8 bits and then
 *   sign-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.  If <i>result</i> does not have the same numeric
 *   value as <i>value</i>, then <code>System.OverflowException</code>
 *   is thrown.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int8</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2B_OVF):
{
	/* Convert "int" into "sbyte" with overflow testing */
	if(stacktop[-1].intValue >= -128 && stacktop[-1].intValue <= 127)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_I2B_OVF);

/**
 * <opcode name="i2ub_ovf" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>uint8</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>i2ub_ovf</format>
 *   <dformat>{i2ub_ovf}</dformat>
 *
 *   <form name="i2ub_ovf" code="COP_PREFIX_I2UB_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 8 bits and then
 *   zero-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.  If <i>result</i> does not have the same numeric
 *   value as <i>value</i>, then <code>System.OverflowException</code>
 *   is thrown.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint8</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2UB_OVF):
{
	/* Convert "int" into "byte" with overflow testing */
	if(stacktop[-1].intValue >= 0 && stacktop[-1].intValue <= 255)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_I2UB_OVF);

/**
 * <opcode name="iu2b_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>int8</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>iu2b_ovf</format>
 *   <dformat>{iu2b_ovf}</dformat>
 *
 *   <form name="iu2b_ovf" code="COP_PREFIX_IU2B_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  If <i>value</i> is greater than 127,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int8</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_IU2B_OVF):
{
	/* Convert "uint" into "sbyte" with overflow testing */
	if(stacktop[-1].intValue >= 0 && stacktop[-1].intValue <= 127)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_IU2B_OVF);

/**
 * <opcode name="iu2ub_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>uint8</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>iu2ub_ovf</format>
 *   <dformat>{iu2ub_ovf}</dformat>
 *
 *   <form name="iu2ub_ovf" code="COP_PREFIX_IU2UB_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  If <i>value</i> is greater than 255,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint8</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_IU2UB_OVF):
{
	/* Convert "uint" into "byte" with overflow testing */
	if(stacktop[-1].intValue >= 0 && stacktop[-1].intValue <= 255)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_IU2UB_OVF);

/**
 * <opcode name="i2s_ovf" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int16</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>i2s_ovf</format>
 *   <dformat>{i2s_ovf}</dformat>
 *
 *   <form name="i2s_ovf" code="COP_PREFIX_I2S_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 16 bits and then
 *   sign-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.  If <i>result</i> does not have the same numeric
 *   value as <i>value</i>, then <code>System.OverflowException</code>
 *   is thrown.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int16</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2S_OVF):
{
	/* Convert "int" into "short" with overflow testing */
	if(stacktop[-1].intValue >= -32768 && stacktop[-1].intValue <= 32767)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_I2S_OVF);

/**
 * <opcode name="i2us_ovf" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>uint16</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>i2us_ovf</format>
 *   <dformat>{i2us_ovf}</dformat>
 *
 *   <form name="i2us_ovf" code="COP_PREFIX_I2US_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 16 bits and then
 *   zero-extending it to 32 bits.  The <i>result</i> is pushed
 *   onto the stack.  If <i>result</i> does not have the same numeric
 *   value as <i>value</i>, then <code>System.OverflowException</code>
 *   is thrown.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint16</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2US_OVF):
{
	/* Convert "int" into "ushort" with overflow testing */
	if(stacktop[-1].intValue >= 0 && stacktop[-1].intValue <= 65535)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_I2US_OVF);

/**
 * <opcode name="iu2s_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>int16</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>iu2s_ovf</format>
 *   <dformat>{iu2s_ovf}</dformat>
 *
 *   <form name="iu2s_ovf" code="COP_PREFIX_IU2S_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  If <i>value</i> is greater than 32767,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int16</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_IU2S_OVF):
{
	/* Convert "uint" into "short" with overflow testing */
	if(stacktop[-1].intValue >= 0 && stacktop[-1].intValue <= 32767)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_IU2S_OVF);

/**
 * <opcode name="iu2us_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>uint16</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>iu2us_ovf</format>
 *   <dformat>{iu2us_ovf}</dformat>
 *
 *   <form name="iu2us_ovf" code="COP_PREFIX_IU2US_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  If <i>value</i> is greater than 65535,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint16</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_IU2US_OVF):
{
	/* Convert "uint" into "ushort" with overflow testing */
	if(stacktop[-1].intValue >= 0 && stacktop[-1].intValue <= 65535)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_IU2US_OVF);

/**
 * <opcode name="i2iu_ovf" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>uint32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>i2iu_ovf</format>
 *   <dformat>{i2iu_ovf}</dformat>
 *
 *   <form name="i2iu_ovf" code="COP_PREFIX_I2IU_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  If <i>value</i> is less than zero,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2IU_OVF):
{
	/* Convert "int" into "uint" with overflow testing */
	if(stacktop[-1].intValue >= 0)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_I2IU_OVF);

/**
 * <opcode name="iu2i_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint32</code> to <code>int32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>iu2i_ovf</format>
 *   <dformat>{iu2i_ovf}</dformat>
 *
 *   <form name="iu2i_ovf" code="COP_PREFIX_IU2I_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint32</code>.  If <i>value</i> is greater than 2147483647,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_IU2I_OVF):
{
	/* Convert "uint" into "int" with overflow testing */
	if(stacktop[-1].intValue >= 0)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_IU2I_OVF);

/**
 * <opcode name="i2ul_ovf" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>uint64</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>i2ul_ovf</format>
 *   <dformat>{i2ul_ovf}</dformat>
 *
 *   <form name="i2ul_ovf" code="COP_PREFIX_I2UL_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  If <i>value</i> is negative,
 *   then <code>System.OverflowException</code> is thrown.  Otherwise
 *   <i>result</i> is <i>value</i>, zero-extended to 64 bits.
 *   The <i>result</i> is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint64</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2UL_OVF):
{
	/* Convert "int" into "ulong" with overflow testing */
	if(stacktop[-1].intValue >= 0)
	{
		WriteLong(&(stacktop[-1]), (ILInt64)(stacktop[-1].intValue));
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, CVM_WORDS_PER_LONG - 1);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_I2UL_OVF);

/**
 * <opcode name="l2i_ovf" group="Conversion operators">
 *   <operation>Convert <code>int64</code> to <code>int32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>l2i_ovf</format>
 *   <dformat>{l2i_ovf}</dformat>
 *
 *   <form name="l2i_ovf" code="COP_PREFIX_L2I_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int64</code>.  If <i>value</i> is less than -2147483648,
 *   or greater than 2147483647, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, truncated
 *   to 32 bits.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_L2I_OVF):
{
	/* Convert "long" into "int" with overflow testing */
	if(L2IOvf(&(stacktop[-CVM_WORDS_PER_LONG])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 1 - CVM_WORDS_PER_LONG);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_L2I_OVF);

/**
 * <opcode name="l2ui_ovf" group="Conversion operators">
 *   <operation>Convert <code>int64</code> to <code>uint32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>l2ui_ovf</format>
 *   <dformat>{l2ui_ovf}</dformat>
 *
 *   <form name="l2ui_ovf" code="COP_PREFIX_L2UI_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int64</code>.  If <i>value</i> is less than zero or
 *   greater than 4294967295, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, truncated
 *   to 32 bits.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_L2UI_OVF):
{
	/* Convert "long" into "uint" with overflow testing */
	if(L2UIOvf(&(stacktop[-CVM_WORDS_PER_LONG])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 1 - CVM_WORDS_PER_LONG);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_L2UI_OVF);

/**
 * <opcode name="lu2i_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint64</code> to <code>int32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>lu2i_ovf</format>
 *   <dformat>{lu2i_ovf}</dformat>
 *
 *   <form name="lu2i_ovf" code="COP_PREFIX_LU2I_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint64</code>.  If <i>value</i> is greater than
 *   2147483647, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, truncated
 *   to 32 bits.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_LU2I_OVF):
{
	/* Convert "ulong" into "int" with overflow testing */
	if(LU2IOvf(&(stacktop[-CVM_WORDS_PER_LONG])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 1 - CVM_WORDS_PER_LONG);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_LU2I_OVF);

/**
 * <opcode name="lu2iu_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint64</code> to <code>uint32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>lu2iu_ovf</format>
 *   <dformat>{lu2iu_ovf}</dformat>
 *
 *   <form name="lu2iu_ovf" code="COP_PREFIX_LU2IU_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint64</code>.  If <i>value</i> is greater than
 *   4294967295, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, truncated
 *   to 32 bits.  The <i>result</i> is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_LU2IU_OVF):
{
	/* Convert "ulong" into "uint" with overflow testing */
	if(LU2UIOvf(&(stacktop[-CVM_WORDS_PER_LONG])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 1 - CVM_WORDS_PER_LONG);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_LU2IU_OVF);

/**
 * <opcode name="l2ul_ovf" group="Conversion operators">
 *   <operation>Convert <code>int64</code> to <code>uint64</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>l2ul_ovf</format>
 *   <dformat>{l2ul_ovf}</dformat>
 *
 *   <form name="l2ul_ovf" code="COP_PREFIX_L2UL_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int64</code>.  If <i>value</i> is less than zero,
 *   then <code>System.OverflowException</code> is thrown.
 *   Otherwise <i>result</i> is <i>value</i>.  The <i>result</i>
 *   is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint64</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_L2UL_OVF):
{
	/* Convert "long" into "ulong" with overflow testing */
	if(ReadLong(&(stacktop[-CVM_WORDS_PER_LONG])) >= 0)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_L2UL_OVF);

/**
 * <opcode name="lu2l_ovf" group="Conversion operators">
 *   <operation>Convert <code>uint64</code> to <code>int64</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>lu2l_ovf</format>
 *   <dformat>{lu2l_ovf}</dformat>
 *
 *   <form name="lu2l_ovf" code="COP_PREFIX_LU2L_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>uint64</code>.  If <i>value</i> is greater than
 *   9223372036854775807, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>.  The <i>result</i>
 *   is pushed onto the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int64</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_LU2L_OVF):
{
	/* Convert "ulong" into "long" with overflow testing */
	if(ReadULong(&(stacktop[-CVM_WORDS_PER_LONG])) <= (ILUInt64)IL_MAX_INT64)
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_LU2L_OVF);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="f2i_ovf" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to <code>int32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>f2i_ovf</format>
 *   <dformat>{f2i_ovf}</dformat>
 *
 *   <form name="f2i_ovf" code="COP_PREFIX_F2I_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>.  If <i>value</i> is not representable
 *   as a 32-bit integer, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, converted to
 *   <code>int32</code>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_F2I_OVF):
{
	/* Convert "native float" into "int" with overflow testing */
	if(F2IOvf(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 1 - CVM_WORDS_PER_NATIVE_FLOAT);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_F2I_OVF);

/**
 * <opcode name="f2iu_ovf" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to <code>uint32</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>f2iu_ovf</format>
 *   <dformat>{f2iu_ovf}</dformat>
 *
 *   <form name="f2iu_ovf" code="COP_PREFIX_F2IU_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>.  If <i>value</i> is not representable
 *   as an unsigned 32-bit integer, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, converted to
 *   <code>uint32</code>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint32</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_F2IU_OVF):
{
	/* Convert "native float" into "uint" with overflow testing */
	if(F2UIOvf(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 1 - CVM_WORDS_PER_NATIVE_FLOAT);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_F2IU_OVF);

/**
 * <opcode name="f2l_ovf" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to <code>int64</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>f2l_ovf</format>
 *   <dformat>{f2l_ovf}</dformat>
 *
 *   <form name="f2l_ovf" code="COP_PREFIX_F2L_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>.  If <i>value</i> is not representable
 *   as a 64-bit integer, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, converted to
 *   <code>int64</code>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>int64</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_F2L_OVF):
{
	/* Convert "native float" into "long" with overflow testing */
	if(F2LOvf(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE,
							CVM_WORDS_PER_LONG - CVM_WORDS_PER_NATIVE_FLOAT);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_F2L_OVF);

/**
 * <opcode name="f2lu_ovf" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to <code>uint64</code>
 *              with overflow detection</operation>
 *
 *   <format>prefix<fsep/>f2lu_ovf</format>
 *   <dformat>{f2lu_ovf}</dformat>
 *
 *   <form name="f2lu_ovf" code="COP_PREFIX_F2LU_OVF"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>.  If <i>value</i> is not representable
 *   as an unsigned 64-bit integer, then <code>System.OverflowException</code>
 *   is thrown.  Otherwise <i>result</i> is <i>value</i>, converted to
 *   <code>uint64</code>.  The <i>result</i> is pushed onto
 *   the stack.</description>
 *
 *   <exceptions>
 *     <exception name="System.OverflowException">Raised if <i>value</i>
 *     is out of range for <code>uint64</code>.</exception>
 *   </exceptions>
 * </opcode>
 */
VMCASE(COP_PREFIX_F2LU_OVF):
{
	/* Convert "native float" into "long" with overflow testing */
	if(F2LUOvf(&(stacktop[-CVM_WORDS_PER_NATIVE_FLOAT])))
	{
		MODIFY_PC_AND_STACK(CVMP_LEN_NONE,
							CVM_WORDS_PER_LONG - CVM_WORDS_PER_NATIVE_FLOAT);
	}
	else
	{
		OVERFLOW_EXCEPTION();
	}
}
VMBREAK(COP_PREFIX_F2LU_OVF);

#endif /* IL_CONFIG_FP_SUPPORTED */

/**
 * <opcode name="i2b_aligned" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int8</code>, aligned
 *              on a stack word boundary</operation>
 *
 *   <format>i2b_aligned</format>
 *   <dformat>{i2b_aligned}</dformat>
 *
 *   <form name="i2b_aligned" code="COP_PREFIX_I2B_ALIGNED"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 8 bits.  The <i>result</i>
 *   is stored in the top-most stack position so that it is aligned
 *   with the beginning of the stack word.</description>
 *
 *   <notes>This instruction is used to align a value prior to boxing
 *   it with the <i>box</i> instruction.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2B_ALIGNED):
{
	/* Convert a 32-bit value into a byte and align it on a word boundary */
	*((ILInt8 *)(stacktop - 1)) = (ILInt8)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_I2B_ALIGNED);

/**
 * <opcode name="i2s_aligned" group="Conversion operators">
 *   <operation>Convert <code>int32</code> to <code>int16</code>, aligned
 *              on a stack word boundary</operation>
 *
 *   <format>prefix<fsep/>i2s_aligned</format>
 *   <dformat>{i2s_aligned}</dformat>
 *
 *   <form name="i2s_aligned" code="COP_PREFIX_I2S_ALIGNED"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>int32</code>.  The <code>int32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 16 bits.  The <i>result</i>
 *   is stored in the top-most stack position so that it is aligned
 *   with the beginning of the stack word.</description>
 *
 *   <notes>This instruction is used to align a value prior to boxing
 *   it with the <i>box</i> instruction.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_I2S_ALIGNED):
{
	/* Convert a 32-bit value into a short and align it on a word boundary */
	*((ILInt16 *)(stacktop - 1)) = (ILInt16)(stacktop[-1].intValue);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_I2S_ALIGNED);

#ifdef IL_CONFIG_FP_SUPPORTED

/**
 * <opcode name="f2f_aligned" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to <code>float32</code>,
 *              aligned on a stack word boundary</operation>
 *
 *   <format>prefix<fsep/>f2f_aligned</format>
 *   <dformat>{f2f_aligned}</dformat>
 *
 *   <form name="f2f_aligned" code="COP_PREFIX_F2F_ALIGNED"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>.  The <code>float32</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 32 bits.  The <i>result</i>
 *   is stored in the top-most stack position so that it is aligned
 *   with the beginning of the stack word.</description>
 *
 *   <notes>This instruction is used to align a value prior to boxing
 *   it with the <i>box</i> instruction.<p/>
 *
 *   The <code>float32</code> type may occupy less stack words than
 *   the original <code>native float</code> value.  Excess stack words
 *   are popped from the stack.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_F2F_ALIGNED):
{
	/* Convert a native float into a float32 and align it on a word boundary */
	*((ILFloat *)(stacktop - CVM_WORDS_PER_NATIVE_FLOAT)) =
			   ReadFloat(stacktop - CVM_WORDS_PER_NATIVE_FLOAT);
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE,
						CVM_WORDS_PER_FLOAT - CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_PREFIX_F2F_ALIGNED);

/**
 * <opcode name="f2d_aligned" group="Conversion operators">
 *   <operation>Convert <code>native float</code> to <code>float64</code>,
 *              aligned on a stack word boundary</operation>
 *
 *   <format>prefix<fsep/>f2d_aligned</format>
 *   <dformat>{f2d_aligned}</dformat>
 *
 *   <form name="f2d_aligned" code="COP_PREFIX_F2D_ALIGNED"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>native float</code>.  The <code>float64</code> <i>result</i>
 *   is formed by truncating <i>value</i> to 64 bits.  The <i>result</i>
 *   is stored in the top-most stack position so that it is aligned
 *   with the beginning of the stack word.</description>
 *
 *   <notes>This instruction is used to align a value prior to boxing
 *   it with the <i>box</i> instruction.<p/>
 *
 *   The <code>float64</code> type may occupy less stack words than
 *   the original <code>native float</code> value.  Excess stack words
 *   are popped from the stack.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_F2D_ALIGNED):
{
	/* Convert a native float into a float64 and align it on a word boundary */
	WriteDouble(stacktop - CVM_WORDS_PER_NATIVE_FLOAT,
			    ReadFloat(stacktop - CVM_WORDS_PER_NATIVE_FLOAT));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE,
					    CVM_WORDS_PER_DOUBLE - CVM_WORDS_PER_NATIVE_FLOAT);
}
VMBREAK(COP_PREFIX_F2D_ALIGNED);

#endif /* IL_CONFIG_FP_SUPPORTED */

#ifdef IL_CONFIG_PINVOKE

/**
 * <opcode name="str2ansi" group="Conversion operators">
 *   <operation>Convert <code>string</code> to <code>ansi char *</code>
 *              </operation>
 *
 *   <format>prefix<fsep/>str2ansi</format>
 *   <dformat>{str2ansi}</dformat>
 *
 *   <form name="str2ansi" code="COP_PREFIX_STR2ANSI"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>string</code>.  The string is converted into a
 *   <i>result</i> character buffer using the underlying platform's
 *   current locale settings.  A pointer to the buffer is pushed onto
 *   the stack as type <code>ptr</code>.</description>
 *
 *   <notes>This instruction is used to convert C# strings into
 *   character buffers during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STR2ANSI):
{
	/* Convert a string object into an "ANSI" character buffer */
	if(stacktop[-1].ptrValue)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue =
			(void *)ILStringToAnsi(thread, (ILString *)(stacktop[-1].ptrValue));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_STR2ANSI);

/**
 * <opcode name="str2utf8" group="Conversion operators">
 *   <operation>Convert <code>string</code> to <code>utf8 char *</code>
 *              </operation>
 *
 *   <format>prefix<fsep/>str2utf8</format>
 *   <dformat>{str2utf8}</dformat>
 *
 *   <form name="str2utf8" code="COP_PREFIX_STR2UTF8"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>string</code>.  The string is converted into a
 *   <i>result</i> character buffer using the UTF-8 encoding.
 *   A pointer to the buffer is pushed onto the stack as type
 *   <code>ptr</code>.</description>
 *
 *   <notes>This instruction is used to convert C# strings into
 *   character buffers during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STR2UTF8):
{
	/* Convert a string object into a UTF-8 character buffer */
	if(stacktop[-1].ptrValue)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue =
			(void *)ILStringToUTF8(thread, (ILString *)(stacktop[-1].ptrValue));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_STR2UTF8);

/**
 * <opcode name="ansi2str" group="Conversion operators">
 *   <operation>Convert <code>ansi char *</code> to <code>string</code>
 *              </operation>
 *
 *   <format>prefix<fsep/>ansi2str</format>
 *   <dformat>{ansi2str}</dformat>
 *
 *   <form name="ansi2str" code="COP_PREFIX_ANSI2STR"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  This pointer is interpreted as a
 *   character buffer that uses the platform's current locale
 *   settings.  This buffer is converted into a string <i>result</i>,
 *   which is then pushed onto the stack as type <code>ptr</code>.
 *   </description>
 *
 *   <notes>This instruction is used to convert character buffers into
 *   C# strings during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_ANSI2STR):
{
	/* Convert an "ANSI" character buffer into a string */
	if(stacktop[-1].ptrValue)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue = (void *)ILStringCreate
			(thread, (const char *)(stacktop[-1].ptrValue));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ANSI2STR);

/**
 * <opcode name="utf82str" group="Conversion operators">
 *   <operation>Convert <code>utf8 char *</code> to <code>string</code>
 *              </operation>
 *
 *   <format>prefix<fsep/>utf82str</format>
 *   <dformat>{utf82str}</dformat>
 *
 *   <form name="utf82str" code="COP_PREFIX_UTF82STR"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>string</code>.  The string is converted into a
 *   <i>result</i>.  character buffer using the UTF-8 encoding.
 *   A pointer to the buffer is pushed onto the stack as type
 *   <code>ptr</code>.</description>
 *
 *   <notes>This instruction is used to convert C# strings into
 *   character buffers during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_UTF82STR):
{
	/* Convert a UTF-8 character buffer into a string */
	if(stacktop[-1].ptrValue)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue = (void *)ILStringCreateUTF8
			(thread, (const char *)(stacktop[-1].ptrValue));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_UTF82STR);

/**
 * <opcode name="str2utf16" group="Conversion operators">
 *   <operation>Convert <code>string</code> to <code>utf16 char *</code>
 *              </operation>
 *
 *   <format>prefix<fsep/>str2utf16</format>
 *   <dformat>{str2utf16}</dformat>
 *
 *   <form name="str2utf16" code="COP_PREFIX_STR2UTF16"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>string</code>.  The string is converted into a
 *   <i>result</i> wide character buffer using the UTF-16 encoding.
 *   A pointer to the buffer is pushed onto the stack as type
 *   <code>ptr</code>.</description>
 *
 *   <notes>This instruction is used to convert C# strings into wide
 *   character buffers during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_STR2UTF16):
{
	/* Convert a string object into a UTF-16 character buffer */
	if(stacktop[-1].ptrValue)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue =
			(void *)ILStringToUTF16
				(thread, (ILString *)(stacktop[-1].ptrValue));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_STR2UTF16);

/**
 * <opcode name="utf162str" group="Conversion operators">
 *   <operation>Convert <code>utf16 char *</code> to <code>string</code>
 *              </operation>
 *
 *   <format>prefix<fsep/>utf162str</format>
 *   <dformat>{utf162str}</dformat>
 *
 *   <form name="utf162str" code="COP_PREFIX_UTF162STR"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>utf16 char *</code>.  The wide character buffer
 *   is converted into a <i>result</i> of type <code>string</code>,
 *   which is pushed onto the stack.</description>
 *
 *   <notes>This instruction is used to convert wide character buffers
 *   into C# strings during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_UTF162STR):
{
	/* Convert a UTF-16 character buffer into a string */
	if(stacktop[-1].ptrValue)
	{
		COPY_STATE_TO_THREAD();
		stacktop[-1].ptrValue = (void *)ILStringWCreate
			(thread, (const ILUInt16 *)(stacktop[-1].ptrValue));
		RESTORE_STATE_FROM_THREAD();
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_UTF162STR);

/**
 * <opcode name="delegate2fnptr" group="Conversion operators">
 *   <operation>Convert a delegate into a function pointer</operation>
 *
 *   <format>prefix<fsep/>delegate2fnptr</format>
 *   <dformat>{delegate2fnptr}</dformat>
 *
 *   <form name="delegate2fnptr" code="COP_PREFIX_DELEGATE2FNPTR"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>delegate</code>.  The value is wrapped in a native
 *   closure to make it suitable for use as a C function pointer.
 *   The wrapped <i>result</i> is pushed onto the stack as type
 *   <code>ptr</code>.</description>
 *
 *   <notes>This instruction is used to convert C# delegates into
 *   C function pointers during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_DELEGATE2FNPTR):
{
	/* Convert a delegate into a function pointer */
	stacktop[-1].ptrValue = _ILDelegateGetClosure
		(thread, (ILObject *)(stacktop[-1].ptrValue));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_DELEGATE2FNPTR);

/**
 * <opcode name="array2ptr" group="Conversion operators">
 *   <operation>Convert an array into a pointer to its
 *				first element</operation>
 *
 *   <format>prefix<fsep/>array2ptr</format>
 *   <dformat>{array2ptr}</dformat>
 *
 *   <form name="array2ptr" code="COP_PREFIX_ARRAY2PTR"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  If <i>value</i> is not <code>null</code>,
 *   a pointer to the first element in the array is computed as
 *   <i>result</i>.  The <i>result</i> is pushed onto the stack
 *   as type <code>ptr</code>.</description>
 *
 *   <notes>This instruction is used to convert C# arrays into
 *   C pointers during "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_ARRAY2PTR):
{
	/* Convert an array into a pointer to its first element */
	if(stacktop[-1].ptrValue)
	{
		stacktop[-1].ptrValue = (void *)(ArrayToBuffer(stacktop[-1].ptrValue));
	}
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ARRAY2PTR);

/**
 * <opcode name="refarray2ansi" group="Conversion operators">
 *   <operation>Convert a reference to an array of strings into
 *              a pointer to an array of <code>ansi char *</code>
 *				values</operation>
 *
 *   <format>prefix<fsep/>refarray2ansi</format>
 *   <dformat>{refarray2ansi}</dformat>
 *
 *   <form name="refarray2ansi" code="COP_PREFIX_REFARRAY2ANSI"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  The <code>String[]</code> object at
 *   the address <i>value</i> is retrieved.  It is converted into
 *   a NULL-terminated C array of the same size, with all of the
 *   strings converted into the ANSI character encoding.  Then
 *   a pointer to this array's reference is pushed as <i>result</i>.
 *   </description>
 *
 *   <notes>This instruction is used to marshal parameters of type
 *   <code>ref String[]</code> "PInvoke" marshalling operations.
 *   It is primarily intended for use with Gtk#.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_REFARRAY2ANSI):
{
	/* Convert a reference to a string array into an ANSI array */
	COPY_STATE_TO_THREAD();
	stacktop[-1].ptrValue = RefArrayToC(thread, stacktop[-1].ptrValue,
										ILStringToAnsi, 1);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_REFARRAY2ANSI);

/**
 * <opcode name="refarray2utf8" group="Conversion operators">
 *   <operation>Convert a reference to an array of strings into
 *              a pointer to an array of <code>utf8 char *</code>
 *				values</operation>
 *
 *   <format>prefix<fsep/>refarray2utf8</format>
 *   <dformat>{refarray2utf8}</dformat>
 *
 *   <form name="refarray2utf8" code="COP_PREFIX_REFARRAY2UTF8"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  The <code>String[]</code> object at
 *   the address <i>value</i> is retrieved.  It is converted into
 *   a NULL-terminated C array of the same size, with all of the
 *   strings converted into the UTF-8 character encoding.  Then
 *   a pointer to this array's reference is pushed as <i>result</i>.
 *   </description>
 *
 *   <notes>This instruction is used to marshal parameters of type
 *   <code>ref String[]</code> "PInvoke" marshalling operations.
 *   It is primarily intended for use with Gtk#.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_REFARRAY2UTF8):
{
	/* Convert a reference to a string array into a UTF-8 array */
	COPY_STATE_TO_THREAD();
	stacktop[-1].ptrValue = RefArrayToC(thread, stacktop[-1].ptrValue,
										ILStringToUTF8, 1);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_REFARRAY2UTF8);

/**
 * <opcode name="array2ansi" group="Conversion operators">
 *   <operation>Convert an array of strings into an array of
 *              <code>ansi char *</code> values</operation>
 *
 *   <format>prefix<fsep/>array2ansi</format>
 *   <dformat>{array2ansi}</dformat>
 *
 *   <form name="array2ansi" code="COP_PREFIX_ARRAY2ANSI"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  The <code>String[]</code> object at
 *   the address <i>value</i> is retrieved.  It is converted into
 *   a NULL-terminated C array of the same size, with all of the
 *   strings converted into the ANSI character encoding.  Then
 *   a pointer to this array is pushed as <i>result</i>.
 *   </description>
 *
 *   <notes>This instruction is used to marshal parameters of type
 *   <code>String[]</code> "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_ARRAY2ANSI):
{
	/* Convert a reference to a string array into an ANSI array */
	COPY_STATE_TO_THREAD();
	stacktop[-1].ptrValue = RefArrayToC(thread, stacktop[-1].ptrValue,
									    ILStringToAnsi, 0);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ARRAY2ANSI);

/**
 * <opcode name="array2utf8" group="Conversion operators">
 *   <operation>Convert an array of strings into an array of
 *              <code>utf8 char *</code> values</operation>
 *
 *   <format>prefix<fsep/>array2utf8</format>
 *   <dformat>{array2utf8}</dformat>
 *
 *   <form name="array2utf8" code="COP_PREFIX_ARRAY2UTF8"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  The <code>String[]</code> object at
 *   the address <i>value</i> is retrieved.  It is converted into
 *   a NULL-terminated C array of the same size, with all of the
 *   strings converted into the UTF8 character encoding.  Then
 *   a pointer to this array is pushed as <i>result</i>.
 *   </description>
 *
 *   <notes>This instruction is used to marshal parameters of type
 *   <code>String[]</code> "PInvoke" marshalling operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_ARRAY2UTF8):
{
	/* Convert a reference to a string array into an ANSI array */
	COPY_STATE_TO_THREAD();
	stacktop[-1].ptrValue = RefArrayToC(thread, stacktop[-1].ptrValue,
									    ILStringToUTF8, 0);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_ARRAY2UTF8);

/**
 * <opcode name="tocustom" group="Conversion operators">
 *   <operation>Convert an object reference into a custom native
 *				pointer</operation>
 *
 *   <format>prefix<fsep/>tocustom<fsep/>len1[4]<fsep/>len2[4]<fsep/>name1<fsep/>name2</format>
 *   <dformat>{tocustom}<fsep/>len1<fsep/>len2<fsep/>name1<fsep/>name2</dformat>
 *
 *   <form name="tocustom" code="COP_PREFIX_TOCUSTOM"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  The custom marshaler called <i>name1</i>,
 *   with cookie <i>name2</i> is used to convert <i>value</i> into a
 *   native pointer <i>result</i>.  The values <i>len1</i> and <i>len2</i>
 *   are the lengths of <i>name1</i> and <i>name2</i>.
 *   </description>
 *
 *   <notes>This instruction is used to perform custom marshaling
 *   during "PInvoke" operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_TOCUSTOM):
{
	/* Convert an object reference into a native pointer */
	COPY_STATE_TO_THREAD();
	stacktop[-1].ptrValue = _ILObjectToCustom
		(thread, (ILObject *)(stacktop[-1].ptrValue),
		 CVMP_ARG_WORD2_PTR(const char *), CVMP_ARG_WORD,
		 CVMP_ARG_WORD2_PTR2(const char *), CVMP_ARG_WORD2);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD2_PTR2, 0);
}
VMBREAK(COP_PREFIX_TOCUSTOM);

/**
 * <opcode name="fromcustom" group="Conversion operators">
 *   <operation>Convert a custom native pointer into an object
 *				reference</operation>
 *
 *   <format>prefix<fsep/>fromcustom<fsep/>len1[4]<fsep/>len2[4]<fsep/>name1<fsep/>name2</format>
 *   <dformat>{fromcustom}<fsep/>len1<fsep/>len2<fsep/>name1<fsep/>name2</dformat>
 *
 *   <form name="fromcustom" code="COP_PREFIX_FROMCUSTOM"/>
 *
 *   <before>..., value</before>
 *   <after>..., result</after>
 *
 *   <description>The <i>value</i> is popped from the stack as
 *   type <code>ptr</code>.  The custom marshaler called <i>name1</i>,
 *   with cookie <i>name2</i>, is used to convert <i>value</i> into an
 *   object reference <i>result</i>.  The values <i>len1</i> and <i>len2</i>
 *   are the lengths of <i>name1</i> and <i>name2</i>.
 *   </description>
 *
 *   <notes>This instruction is used to perform custom marshaling
 *   during "PInvoke" operations.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_FROMCUSTOM):
{
	/* Convert a native pointer into an object reference */
	COPY_STATE_TO_THREAD();
	stacktop[-1].ptrValue = _ILCustomToObject
		(thread, stacktop[-1].ptrValue,
		 CVMP_ARG_WORD2_PTR(const char *), CVMP_ARG_WORD,
		 CVMP_ARG_WORD2_PTR2(const char *), CVMP_ARG_WORD2);
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD2_PTR2, 0);
}
VMBREAK(COP_PREFIX_FROMCUSTOM);

/**
 * <opcode name="struct2native" group="Conversion operators">
 *   <operation>Convert a struct into its native form</operation>
 *
 *   <format>prefix<fsep/>struct2native<fsep/>type</format>
 *   <dformat>{struct2native}<fsep/>type</dformat>
 *
 *   <form name="struct2native" code="COP_PREFIX_STRUCT2NATIVE"/>
 *
 *   <before>..., ptr</before>
 *   <after>...</after>
 *
 *   <description>The structured value at <i>ptr</i> is converted from
 *   its managed form into its native form, converting field values
 *   as appropriate.</description>
 * </opcode>
 */
VMCASE(COP_PREFIX_STRUCT2NATIVE):
{
	/* Convert a struct into its native form */
	extern void _ILStructToNative
		(ILExecThread *thread, void *value, ILType *type);
	COPY_STATE_TO_THREAD();
	_ILStructToNative(thread, stacktop[-1].ptrValue, CVMP_ARG_PTR(ILType *));
	RESTORE_STATE_FROM_THREAD();
	MODIFY_PC_AND_STACK(CVMP_LEN_PTR, -1);
}
VMBREAK(COP_PREFIX_STRUCT2NATIVE);

#else /* !IL_CONFIG_PINVOKE */

VMCASE(COP_PREFIX_STR2ANSI):
VMCASE(COP_PREFIX_STR2UTF8):
VMCASE(COP_PREFIX_ANSI2STR):
VMCASE(COP_PREFIX_UTF82STR):
VMCASE(COP_PREFIX_STR2UTF16):
VMCASE(COP_PREFIX_UTF162STR):
VMCASE(COP_PREFIX_DELEGATE2FNPTR):
VMCASE(COP_PREFIX_ARRAY2PTR):
VMCASE(COP_PREFIX_REFARRAY2ANSI):
VMCASE(COP_PREFIX_REFARRAY2UTF8):
VMCASE(COP_PREFIX_ARRAY2ANSI):
VMCASE(COP_PREFIX_ARRAY2UTF8):
{
	/* Stub out PInvoke-related CVM opcodes */
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
}
VMBREAK(COP_PREFIX_STR2ANSI);

VMCASE(COP_PREFIX_TOCUSTOM):
VMCASE(COP_PREFIX_FROMCUSTOM):
{
	/* Stub out PInvoke-related CVM opcodes */
	MODIFY_PC_AND_STACK(CVMP_LEN_WORD_PTR, 0);
}
VMBREAK(COP_PREFIX_TOCUSTOM);

VMCASE(COP_PREFIX_STRUCT2NATIVE):
{
	/* Stub out PInvoke-related CVM opcodes */
	MODIFY_PC_AND_STACK(CVMP_LEN_PTR, -1);
}
VMBREAK(COP_PREFIX_STRUCT2NATIVE);

#endif /* !IL_CONFIG_PINVOKE */

/**
 * <opcode name="fix_i4_i" group="Conversion operators">
 *   <operation>Convert a <code>int32</code>/<code>native int</code> pair
 *   of values into <code>native int</code>/<code>native int</code></operation>
 *
 *   <format>prefix<fsep/>fix_i4_i</format>
 *   <dformat>{fix_i4_i}</dformat>
 *
 *   <form name="fix_i4_i" code="COP_PREFIX_FIX_I4_I"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result, value2</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped from
 *   the stack as types <code>int32</code> and <code>native int</code>
 *   respectively.  The <code>native int</code> <i>result</i> is formed
 *   by sign-extending <i>value1</i>.  Then, <i>result</i> and <i>value2</i>
 *   are pushed onto the stack.</description>
 *
 *   <notes>This is typically used to promote CIL I4 values to I when
 *   used with a binary arithmetic operation.<p/>
 *
 *   On 32-bit platforms, this instruction will typically do nothing
 *   because the <code>int32</code> and <code>native int</code> types
 *   will be identical.<p/>
 *
 *   There is no <i>fix_i_i4</i> instruction because <i>i2l</i> can
 *   be used to acheive the same result on 64-bit platforms.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_FIX_I4_I):
{
	/* Fix a (I4, I) pair on the stack to be (I, I) */
#ifdef IL_NATIVE_INT64
	WriteLong(&(stacktop[-1]),
		ReadLong(&(stacktop[-CVM_WORDS_PER_LONG])));
	WriteLong(&(stacktop[-(CVM_WORDS_PER_LONG + 1)]),
		(ILInt64)(stacktop[-(CVM_WORDS_PER_LONG + 1)].intValue));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, CVM_WORDS_PER_LONG - 1);
#else
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
#endif
}
VMBREAK(COP_PREFIX_FIX_I4_I);

/**
 * <opcode name="fix_i4_u" group="Conversion operators">
 *   <operation>Convert a <code>uint32</code>/<code>native uint</code> pair of
 *   values into <code>native uint</code>/<code>native uint</code></operation>
 *
 *   <format>prefix<fsep/>fix_i4_u</format>
 *   <dformat>{fix_i4_u}</dformat>
 *
 *   <form name="fix_i4_u" code="COP_PREFIX_FIX_I4_U"/>
 *
 *   <before>..., value1, value2</before>
 *   <after>..., result, value2</after>
 *
 *   <description>Both <i>value1</i> and <i>value2</i> are popped from
 *   the stack as types <code>uint32</code> and <code>native uint</code>
 *   respectively.  The <code>native uint</code> <i>result</i> is formed
 *   by zero-extending <i>value1</i>.  Then, <i>result</i> and <i>value2</i>
 *   are pushed onto the stack.</description>
 *
 *   <notes>This is typically used to promote CIL I4 values to U when
 *   used with a binary arithmetic operation.<p/>
 *
 *   On 32-bit platforms, this instruction will typically do nothing
 *   because the <code>uint32</code> and <code>native uint</code> types
 *   will be identical.<p/>
 *
 *   There is no <i>fix_u_i4</i> instruction because <i>iu2l</i> can
 *   be used to acheive the same result on 64-bit platforms.</notes>
 * </opcode>
 */
VMCASE(COP_PREFIX_FIX_I4_U):
{
	/* Fix a (I4, U) pair on the stack to be (U, U) */
#ifdef IL_NATIVE_INT64
	WriteULong(&(stacktop[-1]),
		ReadULong(&(stacktop[-CVM_WORDS_PER_LONG])));
	WriteULong(&(stacktop[-(CVM_WORDS_PER_LONG + 1)]),
		(ILUInt64)(stacktop[-(CVM_WORDS_PER_LONG + 1)].uintValue));
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, CVM_WORDS_PER_LONG - 1);
#else
	MODIFY_PC_AND_STACK(CVMP_LEN_NONE, 0);
#endif
}
VMBREAK(COP_PREFIX_FIX_I4_U);

#endif /* IL_CVM_PREFIX */
