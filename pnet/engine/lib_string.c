/*
 * lib_string.c - Internalcall methods for "System.String".
 *
 * Copyright (C) 2001, 2002, 2003, 2009  Southern Storm Software, Pty Ltd.
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

#include "engine.h"
#include "lib_defs.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Allocate space for a new string object.
 */
static System_String *AllocString(ILExecThread *thread, ILInt32 length)
{
	System_String *str;
	ILInt32 roundLen = ((length + 7) & ~7);	/* Round to a multiple of 8 */
	str = (System_String *)_ILEngineAllocAtomic(thread,
												thread->process->stringClass,
						 				  		sizeof(System_String) +
						 				  		roundLen * sizeof(ILUInt16));
	if(str)
	{
		str->capacity = roundLen;
		str->length = length;
		return str;
	}
	else
	{
		return 0;
	}
}

/*
 * public String(char[] value, int startIndex, int length);
 */
System_String *_IL_String_ctor_acii(ILExecThread *thread,
								    System_Array *value,
								    ILInt32 startIndex,
								    ILInt32 length)
{
	System_String *_this;

	/* Validate the parameters */
	if(!value)
	{
		ILExecThreadThrowArgNull(thread, "value");
		return 0;
	}
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_Array");
		return 0;
	}
	if(length < 0 || (ArrayLength(value) - startIndex) < length)
	{
		ILExecThreadThrowArgRange(thread, "length",
								  "ArgRange_Array");
		return 0;
	}

	/* Allocate space for the string object */
	_this = AllocString(thread, length);
	if(!_this)
	{
		return 0;
	}

	/* Copy the contents of the character array into the string */
	if(length > 0)
	{
		ILMemCpy(StringToBuffer(_this),
				 ((ILUInt16 *)(ArrayToBuffer(value))) + startIndex,
				 sizeof(ILUInt16) * length);
	}

	/* The string has been initialized */
	return _this;
}

/*
 * public String(char[] value);
 */
System_String *_IL_String_ctor_ac(ILExecThread *thread, System_Array *value)
{
	System_String *_this;
	ILInt32 length;

	/* Determine the length of the final string.  A null argument
	   indicates that the empty string should be constructed */
	if(value)
	{
		length = ArrayLength(value);
	}
	else
	{
		length = 0;
	}

	/* Allocate space for the string object */
	_this = AllocString(thread, length);
	if(!_this)
	{
		return 0;
	}

	/* Copy the contents of the character array into the string */
	if(length > 0)
	{
		ILMemCpy(StringToBuffer(_this), ArrayToBuffer(value),
				 sizeof(ILUInt16) * length);
	}

	/* The string has been initialized */
	return _this;
}

/*
 * public String(char c, int count);
 */
System_String *_IL_String_ctor_ci(ILExecThread *thread,
								  ILUInt16 c, ILInt32 count)
{
	System_String *_this;
	ILUInt16 *buffer;

	/* Validate the parameters */
	if(count < 0)
	{
		ILExecThreadThrowArgRange(thread, "count",
								  "ArgRange_NonNegative");
		return 0;
	}

	/* Allocate space for the string object */
	_this = AllocString(thread, count);
	if(!_this)
	{
		return 0;
	}

	/* Copy the character into the string buffer */
	buffer = StringToBuffer(_this);
	while(count > 0)
	{
		*buffer++ = c;
		--count;
	}

	/* The string has been initialized */
	return _this;
}

/*
 * unsafe public String(char *value, int startIndex, int length);
 */
System_String *_IL_String_ctor_pcii(ILExecThread *thread,
								    ILUInt16 *value,
								    ILInt32 startIndex,
								    ILInt32 length)
{
	System_String *_this;

	/* Validate the parameters */
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_Array");
		return 0;
	}
	if(length < 0 || (!value && length != 0))
	{
		ILExecThreadThrowArgRange(thread, "length",
								  "ArgRange_Array");
		return 0;
	}

	/* Allocate space for the string object */
	_this = AllocString(thread, length);
	if(!_this)
	{
		return 0;
	}

	/* Copy the contents of the character buffer into the string */
	if(length > 0)
	{
		ILMemCpy(StringToBuffer(_this), value + startIndex,
				 sizeof(ILUInt16) * length);
	}

	/* The string has been initialized */
	return _this;
}

/*
 * unsafe public String(char *value);
 */
System_String *_IL_String_ctor_pc(ILExecThread *thread, ILUInt16 *value)
{
	System_String *_this;
	ILInt32 length;

	/* Determine the length of the input.  A null pointer is valid
	   and indicates that an empty string should be created */
	length = 0;
	if(value != 0)
	{
		while(value[length] != 0)
		{
			++length;
		}
	}

	/* Allocate space for the string object */
	_this = AllocString(thread, length);
	if(!_this)
	{
		return 0;
	}

	/* Copy the contents of the character buffer into the string */
	if(length > 0)
	{
		ILMemCpy(StringToBuffer(_this), value, sizeof(ILUInt16) * length);
	}

	/* The string has been initialized */
	return _this;
}

/*
 * unsafe public String(sbyte *value, int startIndex, int length,
 *                      Encoding enc);
 */
System_String *_IL_String_ctor_pbiiEncoding(ILExecThread *thread,
								            ILInt8 *value,
								            ILInt32 startIndex,
								            ILInt32 length,
										    ILObject *encoding)
{
	System_String *_this;
	System_Array *array;
	ILInt32 posn;

	/* Validate the parameters */
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_Array");
		return 0;
	}
	if(length < 0 || (!value && length != 0))
	{
		ILExecThreadThrowArgRange(thread, "length",
								  "ArgRange_Array");
		return 0;
	}

	/* Should we use the default built-in encoding? */
	if(!encoding)
	{
		posn = (ILInt32)ILAnsiGetCharCount
			((unsigned char *)(value + startIndex),
			 (unsigned long)length);
		_this = AllocString(thread, posn);
		if(!_this)
		{
			return 0;
		}
		ILAnsiGetChars((unsigned char *)(value + startIndex),
					   (unsigned long)length,
					   StringToBuffer(_this),
					   (unsigned long )posn);
		return _this;
	}

	/* Construct an array that contains the bytes */
	array = (System_Array *)ILExecThreadNew(thread, "[B", "(Ti)V",
											(ILVaInt)length);
	if(!array)
	{
		return 0;
	}
	ILMemCpy(ArrayToBuffer(array), value + startIndex, length);

	/* Call "String System.Text.Encoding.GetChars(byte[])"
	   to construct the final string that we need */
	_this = 0;
	if(ILExecThreadCallNamedVirtual(thread, "System.Text.Encoding",
							        "GetString", "(T[B)oSystem.String;",
							        &_this, encoding, array))
	{
		return 0;
	}
	else
	{
		return _this;
	}
}

/*
 * unsafe public String(sbyte *value, int startIndex, int length)
 */
System_String *_IL_String_ctor_pbii(ILExecThread *thread,
								    ILInt8 *value,
								    ILInt32 startIndex,
								    ILInt32 length)
{
	/* Construct the string using the default encoding */
	return _IL_String_ctor_pbiiEncoding(thread, value, startIndex, length, 0);
}

/*
 * unsafe public String(sbyte *value)
 */
System_String *_IL_String_ctor_pb(ILExecThread *thread, ILInt8 *value)
{
	/* Determine the length of the input buffer */
	ILInt32 length = 0;
	if(value)
	{
		while(value[length] != 0)
		{
			++length;
		}
	}

	/* Construct the string using the default encoding */
	return _IL_String_ctor_pbiiEncoding(thread, value, 0, length, 0);
}

/*
 * Compare two Unicode strings.
 */
static int ILStrCmpUnicode(const ILUInt16 *str1, ILInt32 length1,
						   const ILUInt16 *str2, ILInt32 length2)
{
	while(length1 > 0 && length2 > 0)
	{
		if(*str1 < *str2)
		{
			return -1;
		}
		else if(*str1 > *str2)
		{
			return 1;
		}
		++str1;
		++str2;
		--length1;
		--length2;
	}
	if(length1 > 0)
	{
		return 1;
	}
	else if(length2 > 0)
	{
		return -1;
	}
	else
	{
		return 0;
	}
}

/*
 * public static int Compare(String strA, String strB);
 */
ILInt32 _IL_String_Compare(ILExecThread *thread,
						   System_String *strA,
						   System_String *strB)
{
	int cmp;

	/* Handle the easy cases first */
	if(!strA)
	{
		if(!strB)
		{
			return 0;
		}
		else
		{
			return -1;
		}
	}
	else if(!strB)
	{
		return 1;
	}
	else if(strA == strB)
	{
		return 0;
	}

	/* Compare the two strings */
	if(strA->length >= strB->length)
	{
		cmp = ILUnicodeStringCompareNoIgnoreCase(StringToBuffer(strA),
						StringToBuffer(strB),
						strB->length);
		if(cmp != 0)
		{
			return cmp;
		}
		if(strA->length > strB->length)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	else
	{
		cmp = ILUnicodeStringCompareNoIgnoreCase(StringToBuffer(strA),
						StringToBuffer(strB),
						strA->length); 
		if(cmp != 0)
		{
			return cmp;
		}
		return -1;
	}
}

/*
 * public static int CompareInternal(String strA, int indexA, int lengthA,
 *									 String strB, int indexB, int lengthB,
 *							 		 bool ignoreCase);
 */
ILInt32 _IL_String_CompareInternal(ILExecThread *thread,
						   		   System_String *strA,
								   ILInt32 indexA, ILInt32 lengthA,
						   		   System_String *strB,
								   ILInt32 indexB, ILInt32 lengthB,
						   		   ILBool ignoreCase)
{
	int cmp;

	/* Handle the easy cases first */
	if(!strA)
	{
		if(!strB)
		{
			return 0;
		}
		else
		{
			return -1;
		}
	}
	else if(!strB)
	{
		return 1;
	}

	/* Compare the two strings */
	if(lengthA >= lengthB)
	{
		if(ignoreCase)
		{
			cmp = ILUnicodeStringCompareIgnoreCase
					(StringToBuffer(strA) + indexA,
					 StringToBuffer(strB) + indexB,
					 (unsigned long)(long)lengthB);
		}
		else
		{
			cmp = ILUnicodeStringCompareNoIgnoreCase
					(StringToBuffer(strA) + indexA,
					 StringToBuffer(strB) + indexB,
					 (unsigned long)(long)lengthB);
		}
		if(cmp != 0)
		{
			return cmp;
		}
		if(lengthA > lengthB)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	else
	{
		if(ignoreCase)
		{
			cmp = ILUnicodeStringCompareIgnoreCase
					(StringToBuffer(strA) + indexA,
					 StringToBuffer(strB) + indexB,
					 (unsigned long)(long)lengthA);
		}
		else
		{
			cmp = ILUnicodeStringCompareNoIgnoreCase
					(StringToBuffer(strA) + indexA,
					 StringToBuffer(strB) + indexB,
					 (unsigned long)(long)lengthA);
		}
		if(cmp != 0)
		{
			return cmp;
		}
		return -1;
	}
}

/*
 * public static int InternalOrdinal(String strA, int indexA, int lengthA,
 *									 String strB, int indexB, int lengthB);
 */
ILInt32 _IL_String_InternalOrdinal(ILExecThread *thread,
						   		   System_String *strA,
								   ILInt32 indexA, ILInt32 lengthA,
						   		   System_String *strB,
								   ILInt32 indexB, ILInt32 lengthB)
{
	ILUInt16 *bufA;
	ILUInt16 *bufB;

	/* Handle the easy cases first */
	if(!strA)
	{
		if(!strB)
		{
			return 0;
		}
		else
		{
			return -1;
		}
	}
	else if(!strB)
	{
		return 1;
	}

	/* Compare the two strings */
	bufA = StringToBuffer(strA) + indexA;
	bufB = StringToBuffer(strB) + indexB;
	while(lengthA > 0 && lengthB > 0)
	{
		if(*bufA < *bufB)
		{
			return -1;
		}
		else if(*bufA > *bufB)
		{
			return 1;
		}
		++bufA;
		++bufB;
		--lengthA;
		--lengthB;
	}

	/* Determine the ordering based on the tail sections */
	if(lengthA > 0)
	{
		return 1;
	}
	else if(lengthB > 0)
	{
		return -1;
	}
	else
	{
		return 0;
	}
}

/*
 * public static bool Equals(String strA, String strB);
 */
ILBool _IL_String_Equals(ILExecThread *thread,
					     System_String *strA,
					     System_String *strB)
{
	if(!strA)
	{
		if(!strB)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	else if(!strB)
	{
		return 0;
	}
	else if(strA == strB)
	{
		return 1;
	}
	else if(strA->length != strB->length)
	{
		return 0;
	}
	else if(!(strA->length))
	{
		return 1;
	}
	else
	{
		return !ILMemCmp(StringToBuffer(strA),
						 StringToBuffer(strB),
						 strA->length * sizeof(ILUInt16));
	}
}

/*
 * internal static String NewString(int length);
 */
System_String *_IL_String_NewString(ILExecThread *thread, ILInt32 length)
{
	return AllocString(thread, length);
}

/*
 * internal static String NewBuilder(String value, int length);
 */
System_String *_IL_String_NewBuilder(ILExecThread *thread,
							         System_String *value,
							         ILInt32 length)
{
	System_String *str;
	ILInt32 roundLen;
	if(length == -1)
	{
		roundLen = value->length;
	}
	else
	{
		roundLen = length;
	}
	/* Check if we have an overflow */
	if((roundLen < 0) || (roundLen > ((IL_MAX_INT32 >> 1) - sizeof(System_String))))
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	roundLen = ((length + 7) & ~7);	/* Round to a multiple of 8 */
	str = (System_String *)_ILEngineAllocAtomic(thread,
												thread->process->stringClass,
						 				  		sizeof(System_String) +
						 				  		roundLen * sizeof(ILUInt16));
	if(str)
	{
		str->capacity = roundLen;
		if(value != 0)
		{
			if(value->length <= roundLen)
			{
				ILMemCpy(StringToBuffer(str), StringToBuffer(value),
						 value->length * sizeof(ILUInt16));
				str->length = value->length;
			}
			else
			{
				ILMemCpy(StringToBuffer(str), StringToBuffer(value),
						 roundLen * sizeof(ILUInt16));
				str->length = roundLen;
			}
		}
		else
		{
			str->length = 0;
		}
		return str;
	}
	else
	{
		return 0;
	}
}

/*
 * public static String Concat(String str1, String str2);
 */
System_String *_IL_String_Concat_StringString(ILExecThread *thread,
					                          System_String *str1,
					                          System_String *str2)
{
	return (System_String *)ILStringConcat(thread,
										   (ILString *)str1,
										   (ILString *)str2);
}

/*
 * public static String Concat(String str1, String str2, String str3);
 */
System_String *_IL_String_Concat_StringStringString(ILExecThread *thread,
					                                System_String *str1,
					                                System_String *str2,
					                                System_String *str3)
{
	return (System_String *)ILStringConcat3(thread,
										    (ILString *)str1,
										    (ILString *)str2,
										    (ILString *)str3);
}

/*
 * internal static void Copy(String dest, int destPos, String src);
 */
void _IL_String_Copy_StringiString(ILExecThread *thread, System_String *dest,
				     			   ILInt32 destPos, System_String *src)
{
	ILMemCpy(StringToBuffer(dest) + destPos,
			 StringToBuffer(src), src->length * sizeof(ILUInt16));
}

/*
 * internal static void Copy(String dest, int destPos,
 *						     String src, int srcPos, int length);
 */
void _IL_String_Copy_StringiStringii(ILExecThread *thread,
						             System_String *dest,
					 	             ILInt32 destPos,
					 	             System_String *src,
					 	             ILInt32 srcPos,
					 	             ILInt32 length)
{
	ILMemCpy(StringToBuffer(dest) + destPos,
			 StringToBuffer(src) + srcPos, length * sizeof(ILUInt16));
}

/*
 * internal void InsertSpace(String dest, int srcPos, int destPos)
 */
void _IL_String_InsertSpace(ILExecThread *thread,
						    System_String *dest,
						    ILInt32 srcPos, ILInt32 destPos)
{
	ILMemMove(StringToBuffer(dest) + destPos,
			  StringToBuffer(dest) + srcPos, 
			  (dest->length - srcPos) * sizeof(ILUInt16));
	dest->length += (destPos - srcPos);
}

/*
 * internal void RemoveSpace(String dest, int index, int length)
 */
void _IL_String_RemoveSpace(ILExecThread *thread,
						    System_String *dest,
						    ILInt32 index, ILInt32 length)
{
	ILMemMove(StringToBuffer(dest) + index,
			  StringToBuffer(dest) + index + length,
			  (dest->length - (index + length)) * sizeof(ILUInt16));
	dest->length -= length;
}

/*
 * private void CopyToChecked(int sourceIndex, char[] destination,
 *							   int destinationIndex, int count);
 */
void _IL_String_CopyToChecked(ILExecThread *thread,
						 	  System_String *_this,
						 	  ILInt32 sourceIndex,
						 	  System_Array *destination,
						 	  ILInt32 destinationIndex,
						 	  ILInt32 count)
{
	ILMemCpy(((ILUInt16 *)(ArrayToBuffer(destination))) + destinationIndex,
			 StringToBuffer(_this) + sourceIndex, count * sizeof(ILUInt16));
}

/*
 * public override int GetHashCode();
 */
ILInt32 _IL_String_GetHashCode(ILExecThread *thread, System_String *_this)
{
	ILInt32 hash = 0;
	ILUInt16 *buf = StringToBuffer(_this);
	ILInt32 len = _this->length;
	ILInt32 posn;
	for(posn = 0; posn < len; ++posn)
	{
		hash = (hash << 5) + hash + (ILInt32)(*buf++);
	}
	return hash;
}

/*
 * public int IndexOf(char value, int startIndex, int count);
 */
ILInt32 _IL_String_IndexOf(ILExecThread *thread,
			 	           System_String *_this,
					       ILUInt16 value,
					       ILInt32 startIndex,
					       ILInt32 count)
{
	ILUInt16 *buf;

	/* Validate the parameters */
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_StringIndex");
		return -1;
	}
	if(count < 0 || (_this->length - startIndex) < count)
	{
		ILExecThreadThrowArgRange(thread, "count",
								  "ArgRange_StringRange");
		return -1;
	}

	/* Search for the value */
	buf = StringToBuffer(_this) + startIndex;
	while(count > 0)
	{
		if(*buf++ == value)
		{
			return startIndex;
		}
		++startIndex;
		--count;
	}
	return -1;
}

/*
 * public int IndexOfAny(char[] anyOf, int startIndex, int count);
 */
ILInt32 _IL_String_IndexOfAny(ILExecThread *thread,
				 	          System_String *_this,
					          System_Array *anyOf,
					          ILInt32 startIndex,
					          ILInt32 count)
{
	ILUInt16 *buf;
	ILUInt16 *anyBuf;
	ILInt32 anyLength;
	ILInt32 anyPosn;

	/* Validate the parameters */
	if(!anyOf)
	{
		ILExecThreadThrowArgNull(thread, "anyOf");
		return -1;
	}
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_StringIndex");
		return -1;
	}
	if(count < 0 || (_this->length - startIndex) < count)
	{
		ILExecThreadThrowArgRange(thread, "count",
								  "ArgRange_StringRange");
		return -1;
	}

	/* Get the start and extent of the "anyOf" array */
	anyBuf = ArrayToBuffer(anyOf);
	anyLength = ArrayLength(anyOf);
	if(!anyLength)
	{
		/* Bail out because there is nothing to find */
		return -1;
	}

	/* Search for the value */
	buf = StringToBuffer(_this) + startIndex;
	while(count > 0)
	{
		for(anyPosn = 0; anyPosn < anyLength; ++anyPosn)
		{
			if(*buf == anyBuf[anyPosn])
			{
				return startIndex;
			}
		}
		++buf;
		++startIndex;
		--count;
	}
	return -1;
}

/*
 * public int LastIndexOf(char value, int startIndex, int count);
 */
ILInt32 _IL_String_LastIndexOf(ILExecThread *thread,
				 	 	       System_String *_this,
					 	       ILUInt16 value,
					 	       ILInt32 startIndex,
					 	       ILInt32 count)
{
	ILUInt16 *buf;

	/* Validate the parameters */
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_StringIndex");
		return -1;
	}
	if(count < 0 || (startIndex - count) < -1)
	{
		ILExecThreadThrowArgRange(thread, "count",
								  "ArgRange_StringRange");
		return -1;
	}

	/* Adjust for overflow */
	if (startIndex >= _this->length)
	{
		count -= startIndex - (_this->length - 1);
		startIndex = _this->length - 1;
	}

	/* Search for the value */
	buf = StringToBuffer(_this) + startIndex;
	while(count > 0)
	{
		if(*buf-- == value)
		{
			return startIndex;
		}
		--startIndex;
		--count;
	}
	return -1;
}

/*
 * public int LastIndexOfAny(char[] anyOf, int startIndex, int count);
 */
ILInt32 _IL_String_LastIndexOfAny(ILExecThread *thread,
							      System_String *_this,
							      System_Array *anyOf,
							      ILInt32 startIndex,
							      ILInt32 count)
{
	ILUInt16 *buf;
	ILUInt16 *anyBuf;
	ILInt32 anyLength;
	ILInt32 anyPosn;

	/* Validate the parameters */
	if(!anyOf)
	{
		ILExecThreadThrowArgNull(thread, "anyOf");
		return -1;
	}
	if(startIndex < 0)
	{
		ILExecThreadThrowArgRange(thread, "startIndex",
								  "ArgRange_StringIndex");
		return -1;
	}
	if(count < 0 || (startIndex - count) < -1)
	{
		ILExecThreadThrowArgRange(thread, "count",
								  "ArgRange_StringRange");
		return -1;
	}

	/* Get the start and extent of the "anyOf" array */
	anyBuf = ArrayToBuffer(anyOf);
	anyLength = ArrayLength(anyOf);
	if(!anyLength)
	{
		/* Bail out because there is nothing to find */
		return -1;
	}

	/* Adjust for overflow */
	if (startIndex >= _this->length)
	{
		count -= startIndex - (_this->length - 1);
		startIndex = _this->length - 1;
	}

	/* Search for the value */
	buf = StringToBuffer(_this) + startIndex;
	while(count > 0)
	{
		for(anyPosn = 0; anyPosn < anyLength; ++anyPosn)
		{
			if(*buf == anyBuf[anyPosn])
			{
				return startIndex;
			}
		}
		--buf;
		--startIndex;
		--count;
	}
	return -1;
}

/*
 * private int FindInRange(int srcFirst, int srcLast,
 *						   int step, String dest);
 */
ILInt32 _IL_String_FindInRange(ILExecThread *thread, System_String *_this,
							   ILInt32 srcFirst, ILInt32 srcLast,
							   ILInt32 step, System_String *dest)
{
	ILUInt16 *buf1;
	ILUInt16 *buf2;
	ILUInt32 size;

	/* Searches for zero length strings always match */
	if(dest->length == 0)
		return srcFirst;
	buf1 = StringToBuffer(_this) + srcFirst;
	buf2 = StringToBuffer(dest);
	size = (ILUInt32)(dest->length * sizeof(ILUInt16));
	if(step > 0)
	{
		/* Scan forwards for the string */
		if (dest->length == 1)
		{
			while(srcFirst <= srcLast)
			{
				if(*buf1 == *buf2)
				{
					return srcFirst;
				}
				++buf1;
				++srcFirst;
			}
		}
		else
		{
			while(srcFirst <= srcLast)
			{
				if((*buf1 == *buf2) && !ILMemCmp(buf1, buf2, size))
				{
					return srcFirst;
				}
				++buf1;
				++srcFirst;
			}
		}
		return -1;
	}
	else
	{
		/* Scan backwards for the string */
		if(dest->length == 1)
		{
			while(srcFirst >= srcLast)
			{
				if(*buf1 == *buf2)
				{
					return srcFirst;
				}
				--buf1;
				--srcFirst;
			}
		}
		else
		{
			while(srcFirst >= srcLast)
			{
				if((*buf1 == *buf2) && !ILMemCmp(buf1, buf2, size))
				{
					return srcFirst;
				}
				--buf1;
				--srcFirst;
			}
		}
		return -1;
	}
}

/*
 * Size of the intern'ed string hash table.
 */
#define	IL_INTERN_HASH_SIZE		509

/*
 * Structure of a intern'ed string hash table entry.
 */
typedef struct _tagILStrHash ILStrHash;
struct _tagILStrHash
{
	System_String	*value;
	ILStrHash		*next;

};

/*
 * Look up the intern'ed string hash table for a value.
 */
static System_String *InternString(ILExecThread *thread,
								   System_String *str, int add)
{
	ILStrHash *table;
	ILStrHash *entry;
	ILUInt32 hash;

	/* Allocate a new hash table, if required */
	table = (ILStrHash *)(thread->process->internHash);
	if(!table)
	{
		table = (ILStrHash *)ILGCAllocPersistent(sizeof(ILStrHash) *
												 IL_INTERN_HASH_SIZE);
		if(!table)
		{
			if(add)
			{
				ILExecThreadThrowOutOfMemory(thread);
			}
			return 0;
		}
		thread->process->internHash = (void *)table;
	}

	/* Compute the hash of the string */
	hash = ((ILUInt32)(_IL_String_GetHashCode(thread, str)))
				% IL_INTERN_HASH_SIZE;

	/* Look for an existing string with the same value */
	entry = &(table[hash]);
	while(entry != 0 && entry->value != 0)
	{
		if(entry->value->length == str->length &&
		   (entry->value->length == 0 ||
		    !ILMemCmp(StringToBuffer(entry->value), StringToBuffer(str),
					  entry->value->length * 2)))
		{
			return entry->value;
		}
		entry = entry->next;
	}
	if(!add)
	{
		return 0;
	}

	/* Add a new entry to the intern'ed string hash table */
	entry = &(table[hash]);
	if(entry->value == 0)
	{
		entry->value = str;
	}
	else
	{
		entry = (ILStrHash *)ILGCAlloc(sizeof(ILStrHash));
		if(!entry)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		entry->value = str;
		entry->next = table[hash].next;
		table[hash].next = entry;
	}
	return str;
}

/*
 * public static String Intern(String str);
 */
System_String *_IL_String_Intern(ILExecThread *thread, System_String *str)
{
	if(str)
	{
		return InternString(thread, str, 1);
	}
	else
	{
		ILExecThreadThrowArgNull(thread, "str");
		return 0;
	}
}

/*
 * public static String IsInterned(String str);
 */
System_String *_IL_String_IsInterned(ILExecThread *thread, System_String *str)
{
	if(str)
	{
		return InternString(thread, str, 0);
	}
	else
	{
		ILExecThreadThrowArgNull(thread, "str");
		return 0;
	}
}

/*
 * internal static void CharFill(String str, int start, int count, char ch);
 */
void _IL_String_CharFill_Stringiic(ILExecThread *thread, System_String *str,
					     		   ILInt32 start, ILInt32 count, ILUInt16 ch)
{
	while(count > 0)
	{
		StringToBuffer(str)[start] = ch;
		--count;
		++start;
	}
}

/*
 * internal static void CharFill(String str, int start,
 *								 char[] chars, int index,
 *								 int count);
 */
void _IL_String_CharFill_Stringiacii(ILExecThread *thread,
							 	   	 System_String *str,
								   	 ILInt32 start,
							 	   	 System_Array *chars,
								   	 ILInt32 index,
								   	 ILInt32 count)
{
	ILUInt16 *src = ((ILUInt16 *)(ArrayToBuffer(chars))) + index;
	ILUInt16 *dest = StringToBuffer(str) + start;
	while(count > 0)
	{
		*dest++ = *src++;
		--count;
	}
}

/*
 * public String Replace(char oldChar, char newChar)
 */
System_String *_IL_String_Replace_cc(ILExecThread *thread,
							 	     System_String *_this,
								     ILUInt16 oldChar,
								     ILUInt16 newChar)
{
	System_String *str;
	ILUInt16 *buf1;
	ILInt32 len;
	ILInt32 pos = 0;

	/* If nothing will happen, then return the current string as-is */
	len = _this->length;
	if(oldChar == newChar || len == 0)
	{
		return _this;
	}

	/* Scan the two strings, copying and replacing as we go */
	buf1 = StringToBuffer(_this);
	while(len > 0)
	{
		if(*buf1 != oldChar)
		{
			++buf1;
			++pos;
			--len;
		}
		else /* found one char to replace */
		{
			ILUInt16 *buf2;

			/* Allocate a new string */
			str = AllocString(thread, _this->length);
			if(!str)
			{
				return 0;
			}
			if(pos > 0)
			{
				/* copy the allready checked part */
				ILMemCpy(StringToBuffer(str), StringToBuffer(_this),
				 sizeof(ILUInt16) * pos);
			}
			buf2 = StringToBuffer(str) + pos;
			++buf1;
			*buf2++ = newChar;
			--len;
			while(len > 0)
			{
				if(*buf1 != oldChar)
				{
					*buf2++ = *buf1++;
				}
				else
				{
					*buf2++ = newChar;
					++buf1;
				}
				--len;
			}
			return str;
		}

	}
	return _this;
}

/*
 * Determine if a range of characters in two strings are equal.
 */
#define	EqualRange(str1,posn1,count,str2,posn2)	\
			(!ILMemCmp(StringToBuffer((str1)) + (posn1), \
					   StringToBuffer((str2)) + (posn2), \
					   (count) * sizeof(ILUInt16)))

/*
 * public String Replace(String oldValue, String newValue)
 */
System_String *_IL_String_Replace_StringString(ILExecThread *thread,
						 	    		       System_String *_this,
							    		       System_String *oldValue,
							    		       System_String *newValue)
{
	ILInt32 oldLen;
	ILInt32 newLen;
	ILInt32 finalLen;
	ILInt32 posn;
	System_String *str;
	ILUInt16 *buf;
	ILBool foundMatch = 0;

	/* Validate the parameters */
	if(!oldValue)
	{
		ILExecThreadThrowArgNull(thread, "oldValue");
		return 0;
	}

	/* If "oldValue" is an empty string, then the
	   string will not be changed */
	if(!(oldValue->length))
	{
		return _this;
	}

	/* Get the length of the old and new values */
	oldLen = oldValue->length;
	if(newValue)
	{
		newLen = newValue->length;
	}
	else
	{
		newLen = 0;
	}

	/* Determine the length of the final string */
	finalLen = 0;
	posn = 0;
	while((posn + oldLen) <= _this->length)
	{
		if(EqualRange(_this, posn, oldLen, oldValue, 0))
		{
			finalLen += newLen;
			posn += oldLen;
			foundMatch = 1;
		}
		else
		{
			++finalLen;
			++posn;
		}
		if(((ILUInt32)finalLen) > (ILUInt32)((IL_MAX_INT32 / 4) - 16))
		{
			/* The resulting string will be way too big */
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
	}
	if(!foundMatch) /* no match found */
	{
		return _this;
	}

	finalLen += _this->length - posn;

	/* Allocate a new string */
	str = AllocString(thread, finalLen);
	if(!str)
	{
		return 0;
	}

	/* Scan the input string again and perform the replacement */
	buf = StringToBuffer(str);
	finalLen = 0;
	posn = 0;
	while((posn) < _this->length)
	{
		if((posn + oldLen) <= _this->length && EqualRange(_this, posn, oldLen, oldValue, 0))
		{
			if(newLen > 0)
			{
				ILMemCpy(buf + finalLen, StringToBuffer(newValue),
						 newLen * sizeof(ILUInt16));
			}
			finalLen += newLen;
			posn += oldLen;
		}
		else
		{
			buf[finalLen++] = StringToBuffer(_this)[posn++];
		}
	}

	/* Return the final replaced string to the caller */
	return str;
}

#define	TrimFlag_Front		1
#define	TrimFlag_End		2

/*
 * Match a character against an array of characters.
 */
static IL_INLINE int IsCharMatch(System_Array *trimChars, ILUInt16 ch)
{
	if(trimChars)
	{
		ILInt32 len = ArrayLength(trimChars);
		ILUInt16 *buf = (ILUInt16 *)(ArrayToBuffer(trimChars));
		while(len > 0)
		{
			if(*buf++ == ch)
			{
				return 1;
			}
			--len;
		}
	}
	return 0;
}

/*
 * private String Trim(char[] trimChars, int trimFlags);
 */
System_String *_IL_String_Trim(ILExecThread *thread,
							   System_String *_this,
							   System_Array *trimChars,
							   ILInt32 trimFlags)
{
	ILInt32 start, end;
	ILUInt16 *buf = StringToBuffer(_this);
	System_String *str;
	start = 0;
	end = _this->length;
	if((trimFlags & TrimFlag_Front) != 0)
	{
		while(start < end && IsCharMatch(trimChars, buf[start]))
		{
			++start;
		}
	}
	if((trimFlags & TrimFlag_End) != 0)
	{
		while(start < end && IsCharMatch(trimChars, buf[end - 1]))
		{
			--end;
		}
	}
	str = AllocString(thread, end - start);
	if(str)
	{
		if(start < end)
		{
			ILMemCpy(StringToBuffer(str), buf + start,
					 (end - start) * sizeof(ILUInt16));
		}
		return str;
	}
	else
	{
		return 0;
	}
}

/*
 * internal char GetChar(int posn);
 */
ILUInt16 _IL_String_GetChar(ILExecThread *thread,
				  	        System_String *_this, ILInt32 posn)
{
	if(posn >= 0 && posn < _this->length)
	{
		return StringToBuffer(_this)[posn];
	}
	else
	{
		ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
								"ArgRange_StringIndex");
		return 0;
	}
}

/*
 * internal void SetChar(int posn, char value);
 */
void _IL_String_SetChar(ILExecThread *thread,
			  	        System_String *_this,
			  	 	    ILInt32 posn, ILUInt16 value)
{
	if(posn >= 0 && posn < _this->length)
	{
		StringToBuffer(_this)[posn] = value;
	}
	else
	{
		ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
								"ArgRange_StringIndex");
	}
}

ILString *ILStringCreate(ILExecThread *thread, const char *str)
{
	if(str)
	{
		/* Call the "String(sbyte *, int, int, Encoding)" constructor */
		return (ILString *)_IL_String_ctor_pbiiEncoding
					(thread, (ILInt8 *)str, 0,
					 (ILInt32)(strlen(str)), (void *)0);
	}
	else
	{
		return (ILString *)0;
	}
}

ILString *ILStringCreateLen(ILExecThread *thread, const char *str, int len)
{
	if(str)
	{
		/* Call the "String(sbyte *, int, int, Encoding)" constructor */
		return (ILString *)_IL_String_ctor_pbiiEncoding
					(thread, (ILInt8 *)str, 0, (ILInt32)len, (void *)0);
	}
	else
	{
		return (ILString *)0;
	}
}

ILString *ILStringCreateUTF8(ILExecThread *thread, const char *str)
{
	if(str)
	{
		return ILStringCreateUTF8Len(thread, str, strlen(str));
	}
	else
	{
		return (ILString *)0;
	}
}

ILString *ILStringCreateUTF8Len(ILExecThread *thread, const char *str, int len)
{
	ILInt32 newLen;
	int posn;
	unsigned long ch;
	System_String *newStr;
	unsigned short *buf;

	/* Bail out if the string is NULL */
	if(!str)
	{
		return (ILString *)0;
	}

	/* Determine the length of the UTF-8 string in UTF-16 characters */
	newLen = 0;
	posn = 0;
	while(posn < len)
	{
		ch = ILUTF8ReadChar(str, len, &posn);
		newLen += ILUTF16WriteChar((unsigned short *)0, ch);
	}

	/* Allocate a new string object */
	newStr = AllocString(thread, newLen);
	if(!newStr)
	{
		return (ILString *)0;
	}

	/* Copy the characters into the new string */
	buf = StringToBuffer(newStr);
	posn = 0;
	while(posn < len)
	{
		ch = ILUTF8ReadChar(str, len, &posn);
		buf += ILUTF16WriteChar(buf, ch);
	}

	/* Done */
	return (ILString *)newStr;
}

ILString *ILStringWCreate(ILExecThread *thread, const ILUInt16 *str)
{
	if(str)
	{
		int len = 0;
		while(str[len] != 0)
		{
			++len;
		}
		return ILStringWCreateLen(thread, str, len);
	}
	else
	{
		return (ILString *)0;
	}
}

ILString *ILStringWCreateLen(ILExecThread *thread,
							 const ILUInt16 *str, int len)
{
	if(str && len >= 0)
	{
		/* Call the "String(char *, int, int)" constructor */
		return (ILString *)_IL_String_ctor_pcii
				(thread, (ILUInt16 *)str, 0, (ILInt32)len);
	}
	else
	{
		return (ILString *)0;
	}
}

int ILStringCompare(ILExecThread *thread, ILString *strA, ILString *strB)
{
	return (int)(_IL_String_Compare(thread,
								    (System_String *)strA,
								    (System_String *)strB));
}

int ILStringCompareIgnoreCase(ILExecThread *thread, ILString *strA,
							  ILString *strB)
{
	return (int)(_IL_String_CompareInternal
						(thread,
						 (System_String *)strA, 0,
						 ((strA != 0) ? ((System_String *)strA)->length : 0),
						 (System_String *)strB, 0,
						 ((strB != 0) ? ((System_String *)strB)->length : 0),
						 (ILBool)1));
}

int ILStringCompareOrdinal(ILExecThread *thread, ILString *strA,
						   ILString *strB)
{
	return (int)(_IL_String_InternalOrdinal
						(thread,
						 (System_String *)strA, 0,
						 ((strA != 0) ? ((System_String *)strA)->length : 0),
						 (System_String *)strB, 0,
						 ((strB != 0) ? ((System_String *)strB)->length : 0)));
}

int ILStringEquals(ILExecThread *thread, ILString *strA, ILString *strB)
{
	return (int)(_IL_String_Equals(thread,
								   (System_String *)strA,
								   (System_String *)strB));
}

ILString *ILStringConcat(ILExecThread *thread, ILString *strA, ILString *strB)
{
	if(!strA || ((System_String *)strA)->length == 0)
	{
		if(!strB || ((System_String *)strB)->length == 0)
		{
			if(strB)
			{
				return strB;
			}
			else if(strA)
			{
				return strA;
			}
			else
			{
				return (ILString *)AllocString(thread, 0);
			}
		}
		else
		{
			return strB;
		}
	}
	else if(!strB || ((System_String *)strB)->length == 0)
	{
		return strA;
	}
	else
	{
		ILInt32 lenA = ((System_String *)strA)->length;
		ILInt32 lenB = ((System_String *)strB)->length;
		System_String *result;
		if(((ILUInt32)(lenA + lenB)) >= ((ILUInt32)(IL_MAX_INT32 / 2)))
		{
			/* The resulting string is too big */
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		result = AllocString(thread, lenA + lenB);
		if(result)
		{
			ILMemCpy(StringToBuffer(result), StringToBuffer(strA),
					 lenA * sizeof(ILUInt16));
			ILMemCpy(StringToBuffer(result) + lenA, StringToBuffer(strB),
					 lenB * sizeof(ILUInt16));
		}
		return (ILString *)result;
	}
}

ILString *ILStringConcat3(ILExecThread *thread, ILString *strA,
						  ILString *strB, ILString *strC)
{
	if(strA && strB && strC)
	{
		ILInt32 lenA = ((System_String *)strA)->length;
		ILInt32 lenB = ((System_String *)strB)->length;
		ILInt32 lenC = ((System_String *)strC)->length;
		System_String *result;
		if((((ILUInt64)lenA) + ((ILUInt64)lenB) + ((ILUInt64)lenC)) >=
					((ILUInt64)(IL_MAX_INT32 / 2)))
		{
			/* The resulting string is too big */
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		result = AllocString(thread, lenA + lenB + lenC);
		if(result)
		{
			ILMemCpy(StringToBuffer(result), StringToBuffer(strA),
					 lenA * sizeof(ILUInt16));
			ILMemCpy(StringToBuffer(result) + lenA, StringToBuffer(strB),
					 lenB * sizeof(ILUInt16));
			ILMemCpy(StringToBuffer(result) + lenA + lenB,
					 StringToBuffer(strC), lenC * sizeof(ILUInt16));
		}
		return (ILString *)result;
	}
	else if(!strA)
	{
		return ILStringConcat(thread, strB, strC);
	}
	else if(!strB)
	{
		return ILStringConcat(thread, strA, strC);
	}
	else
	{
		return ILStringConcat(thread, strA, strB);
	}
}

ILString *ILStringConcat4(ILExecThread *thread, ILString *strA,
						  ILString *strB, ILString *strC, ILString *strD)
{
	if(strA && strB && strC && strD)
	{
		ILInt32 lenA = ((System_String *)strA)->length;
		ILInt32 lenB = ((System_String *)strB)->length;
		ILInt32 lenC = ((System_String *)strC)->length;
		ILInt32 lenD = ((System_String *)strD)->length;
		System_String *result;
		if((((ILUInt64)lenA) + ((ILUInt64)lenB) +
		    ((ILUInt64)lenC) + ((ILUInt64)lenD)) >=
					((ILUInt64)(IL_MAX_INT32 / 2)))
		{
			/* The resulting string is too big */
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		result = AllocString(thread, lenA + lenB + lenC + lenD);
		if(result)
		{
			ILMemCpy(StringToBuffer(result), StringToBuffer(strA),
					 lenA * sizeof(ILUInt16));
			ILMemCpy(StringToBuffer(result) + lenA, StringToBuffer(strB),
					 lenB * sizeof(ILUInt16));
			ILMemCpy(StringToBuffer(result) + lenA + lenB,
					 StringToBuffer(strC), lenC * sizeof(ILUInt16));
			ILMemCpy(StringToBuffer(result) + lenA + lenB + lenC,
					 StringToBuffer(strD), lenD * sizeof(ILUInt16));
		}
		return (ILString *)result;
	}
	else if(!strA)
	{
		return ILStringConcat3(thread, strB, strC, strD);
	}
	else if(!strB)
	{
		return ILStringConcat3(thread, strA, strC, strD);
	}
	else if(!strC)
	{
		return ILStringConcat3(thread, strA, strB, strD);
	}
	else
	{
		return ILStringConcat3(thread, strA, strB, strC);
	}
}

ILString *ILObjectToString(ILExecThread *thread, ILObject *object)
{
	ILString *result = 0;
	if(object)
	{
		/* Call the virtual "ToString" method on the object */
		ILExecThreadCallNamedVirtual(thread, "System.Object", "ToString",
									 "(T)oSystem.String;",
									 &result, object);
	}
	return result;
}

ILString *ILStringIntern(ILExecThread *thread, ILString *str)
{
	if(str)
	{
		return (ILString *)InternString(thread, (System_String *)str, 1);
	}
	else
	{
		return 0;
	}
}

/*
 * Determine if the contents of a string buffer is the
 * same as a literal string value from an image.
 */
static int SameAsImage(ILUInt16 *buf, const char *str, ILInt32 len)
{
#if defined(__i386) || defined(__i386__)
	/* We can take a short-cut on x86 platforms which already
	   have the string in the correct format */
	if(len > 0)
	{
		return !ILMemCmp(buf, str, len * sizeof(ILUInt16));
	}
	else
	{
		return 1;
	}
#else
	while(len > 0)
	{
		if(*buf++ != IL_READ_UINT16(str))
		{
			return 0;
		}
		str += 2;
		--len;
	}
	return 1;
#endif
}

static ILString *InternFromBuffer(ILExecThread *thread,
								  const char *str, unsigned long len)
{
	unsigned long posn;
	System_String *newStr;
	ILStrHash *table;
	ILStrHash *entry;
	ILInt32 hashTemp;
	ILUInt32 hash;

	/* Allocate a new hash table, if required */
	table = (ILStrHash *)(thread->process->internHash);
	if(!table)
	{
		table = (ILStrHash *)ILGCAllocPersistent(sizeof(ILStrHash) *
												 IL_INTERN_HASH_SIZE);
		if(!table)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		thread->process->internHash = (void *)table;
	}

	/* Compute the hash of the string */
	hashTemp = 0;
	for(posn = 0; posn < len; ++posn)
	{
		hashTemp = (hashTemp << 5) + hashTemp +
				   (ILInt32)(IL_READ_UINT16(str + posn * 2));
	}
	hash = ((ILUInt32)hashTemp) % IL_INTERN_HASH_SIZE;

	/* Look for an existing string with the same value */
	entry = &(table[hash]);
	while(entry != 0 && entry->value != 0)
	{
		if(entry->value->length == (ILInt32)len &&
		   (entry->value->length == 0 ||
		    SameAsImage(StringToBuffer(entry->value), str,
					    entry->value->length)))
		{
			return (ILString *)(entry->value);
		}
		entry = entry->next;
	}

	/* Allocate space for the string */
	newStr = AllocString(thread, (ILInt32)len);
	if(!newStr)
	{
		return 0;
	}

	/* Copy the image data into the string */
#if defined(__i386) || defined(__i386__)
	/* We can take a short-cut on x86 platforms which already
	   have the string in the correct format */
	if(len > 0)
	{
		ILMemCpy(StringToBuffer(newStr), str, len * sizeof(ILUInt16));
	}
#else
	{
		ILUInt16 *dest = StringToBuffer(newStr);
		while(len > 0)
		{
			*dest++ = IL_READ_UINT16(str);
			str += 2;
			--len;
		}
	}
#endif

	/* Add a new entry to the intern'ed string hash table */
	entry = &(table[hash]);
	if(entry->value == 0)
	{
		entry->value = newStr;
	}
	else
	{
		entry = (ILStrHash *)ILGCAlloc(sizeof(ILStrHash));
		if(!entry)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		entry->value = newStr;
		entry->next = table[hash].next;
		table[hash].next = entry;
	}

	/* Return the final string to the caller */
	return (ILString *)newStr;
}

ILString *_ILStringInternFromImage(ILExecThread *thread, ILImage *image,
								   ILToken token)
{
	const char *str;
	ILUInt32 len;

	/* Get the string from the image's "#US" blob */
	str = ILImageGetUserString(image, token & ~IL_META_TOKEN_MASK, &len);
	if(!str)
	{
		/* Shouldn't happen, but intern an empty string anyway */
		len = 0;
	}

	/* Internalize the string buffer */
	return InternFromBuffer(thread, str, len);
}

ILString *_ILStringInternFromConstant(ILExecThread *thread, void *data,
									  unsigned long numChars)
{
	return InternFromBuffer(thread, (const char *)data, numChars);
}

ILInt32 _ILStringToBuffer(ILExecThread *thread, ILString *str, ILUInt16 **buf)
{
	if(str)
	{
		*buf = StringToBuffer(str);
		return ((System_String *)str)->length;
	}
	else
	{
		*buf = 0;
		return 0;
	}
}

char *ILStringToUTF8(ILExecThread *thread, ILString *str)
{
	ILUInt16 *buffer;
	ILInt32 length;
	ILInt32 utf8Len;
	char *newStr;
	char *temp;
	int posn;

	/* Bail out immediately if the string is NULL */
	if(!str)
	{
		return 0;
	}

	/* Determine the length of the string in UTF-8 characters */
	buffer = StringToBuffer(str);
	length = ((System_String *)str)->length;
	posn = 0;
	utf8Len = 0;
	while(posn < (int)length)
	{
		utf8Len += ILUTF8WriteChar
			(0, ILUTF16ReadChar(buffer, (int)length, &posn));
	}

	/* Allocate space within the garbage-collected heap */
	newStr = (char *)ILGCAllocAtomic(utf8Len + 1);
	if(!newStr)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}

	/* Copy the characters into the allocated buffer */
	temp = newStr;
	posn = 0;
	while(posn < (int)length)
	{
		temp += ILUTF8WriteChar
			(temp, ILUTF16ReadChar(buffer, (int)length, &posn));
	}
	*temp = '\0';

	/* Done */
	return newStr;
}

ILUInt16 *ILStringToUTF16(ILExecThread *thread, ILString *str)
{
	ILUInt16 *buffer;
	ILInt32 length;
	ILUInt16 *newStr;

	/* Bail out immediately if the string is NULL */
	if(!str)
	{
		return 0;
	}

	/* Determine the length of the string in UTF-16 characters */
	buffer = StringToBuffer(str);
	length = ((System_String *)str)->length;

	/* Allocate space within the garbage-collected heap */
	newStr = (ILUInt16 *)ILGCAllocAtomic((length + 1) * sizeof(ILUInt16));
	if(!newStr)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}

	/* Copy the characters into the allocated buffer */
	if(length > 0)
	{
		ILMemCpy(newStr, buffer, length * sizeof(ILUInt16));
	}
	newStr[length] = 0;
	return newStr;
}

char *ILStringToAnsi(ILExecThread *thread, ILString *str)
{
	if(str)
	{
		ILUInt16 *buf = StringToBuffer(str);
		ILInt32 len = ((System_String *)str)->length;
		unsigned long size = ILAnsiGetByteCount(buf, (unsigned long)len);
		char *newStr = (char *)ILGCAllocAtomic(size + 1);
		if(!newStr)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		ILAnsiGetBytes(buf, (unsigned long)len,
					   (unsigned char *)newStr, size);
		newStr[size] = '\0';
		return newStr;
	}
	else
	{
		return 0;
	}
}

/*
 * public virtual char ToLower(char c);
 */
ILUInt16 _IL_TextInfo_ToLower_c(ILExecThread *_thread, ILObject *_this,
								ILUInt16 c)
{
	if(c >= 'A' && c <= 'Z')
	{
		return (ILUInt16)(c - 'A' + 'a');
	}
	else if(c < 0x0080)
	{
		return c;
	}
	else
	{
		return ILUnicodeCharToLower(c);
	}
}

/*
 * public virtual char ToUpper(char c);
 */
ILUInt16 _IL_TextInfo_ToUpper_c(ILExecThread *_thread, ILObject *_this,
								ILUInt16 c)
{
	if(c >= 'a' && c <= 'z')
	{
		return (ILUInt16)(c - 'a' + 'A');
	}
	else if(c < 0x0080)
	{
		return c;
	}
	else
	{
		return ILUnicodeCharToUpper(c);
	}
}

/*
 * public virtual String ToLower(String str);
 */
ILString *_IL_TextInfo_ToLower_String(ILExecThread *_thread, ILObject *_this,
									  ILString *str)
{
	System_String *newStr;
	ILInt32 len;
	if(!str)
	{
		ILExecThreadThrowArgNull(_thread, "str");
		return 0;
	}
	len = ((System_String *)str)->length;
	newStr = AllocString(_thread, len);
	if(!newStr)
	{
		return 0;
	}
	ILUnicodeStringToLower(StringToBuffer(newStr),
						   StringToBuffer(str),
						   (unsigned long)(long)len);
	return (ILString *)newStr;
}

/*
 * public virtual String ToUpper(String str);
 */
ILString * _IL_TextInfo_ToUpper_String(ILExecThread *_thread, ILObject *_this,
									   ILString *str)
{
	System_String *newStr;
	ILInt32 len;
	if(!str)
	{
		ILExecThreadThrowArgNull(_thread, "str");
		return 0;
	}
	len = ((System_String *)str)->length;
	newStr = AllocString(_thread, len);
	if(!newStr)
	{
		return 0;
	}
	ILUnicodeStringToUpper(StringToBuffer(newStr),
						   StringToBuffer(str),
						   (unsigned long)(long)len);
	return (ILString *)newStr;
}

#ifdef	__cplusplus
};
#endif
