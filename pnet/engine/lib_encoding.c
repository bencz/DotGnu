/*
 * lib_encoding.c - Internalcall methods for "System.Text.DefaultEncoding".
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
 * private static int InternalGetByteCount(char[] chars, int index, int count);
 */
ILInt32 _IL_DefaultEncoding_InternalGetByteCount_acii
				(ILExecThread *_thread, System_Array *chars,
				 ILInt32 index, ILInt32 count)
{
	return (ILInt32)ILAnsiGetByteCount
		(((ILUInt16 *)ArrayToBuffer(chars)) + index, (unsigned long)count);
}

/*
 * private static int InternalGetByteCount(String s, int index, int count);
 */
ILInt32 _IL_DefaultEncoding_InternalGetByteCount_Stringii
				(ILExecThread *_thread, ILString *s,
				 ILInt32 index, ILInt32 count)
{
	return (ILInt32)ILAnsiGetByteCount
		(StringToBuffer(s) + index, (unsigned long)count);
}

/*
 * private static int InternalGetBytes(char[] chars, int charIndex,
 *									   int charCount, byte[] bytes,
 *									   int byteIndex);
 */
ILInt32 _IL_DefaultEncoding_InternalGetBytes_aciiaBi
				(ILExecThread *_thread, System_Array *chars,
				 ILInt32 charIndex, ILInt32 charCount,
				 System_Array * bytes, ILInt32 byteIndex)
{
	return (ILInt32)ILAnsiGetBytes
		(((ILUInt16 *)ArrayToBuffer(chars)) + charIndex,
		 (unsigned long)charCount,
		 ((ILUInt8 *)ArrayToBuffer(bytes)) + byteIndex,
		 (unsigned long)(ArrayLength(bytes) - byteIndex));
}

/*
 * private static int InternalGetBytes(String s, int charIndex,
 *									   int charCount, byte[] bytes,
 *									   int byteIndex);
 */
ILInt32 _IL_DefaultEncoding_InternalGetBytes_StringiiaBi
				(ILExecThread *_thread, ILString *s,
				 ILInt32 charIndex, ILInt32 charCount,
				 System_Array *bytes, ILInt32 byteIndex)
{
	return (ILInt32)ILAnsiGetBytes
		(StringToBuffer(s) + charIndex,
		 (unsigned long)charCount,
		 ((ILUInt8 *)ArrayToBuffer(bytes)) + byteIndex,
		 (unsigned long)(ArrayLength(bytes) - byteIndex));
}

/*
 * private static int InternalGetCharCount(byte[] bytes, int index, int count);
 */
ILInt32 _IL_DefaultEncoding_InternalGetCharCount
				(ILExecThread *_thread, System_Array *bytes,
				 ILInt32 index, ILInt32 count)
{
	return (ILInt32)ILAnsiGetCharCount
		(((ILUInt8 *)ArrayToBuffer(bytes)) + index, (unsigned long)count);
}

/*
 * private static int InternalGetChars(byte[] bytes, int byteIndex,
 *									   int byteCount, char[] chars,
 *									   int charIndex);
 */
ILInt32 _IL_DefaultEncoding_InternalGetChars
				(ILExecThread *_thread, System_Array *bytes,
				 ILInt32 byteIndex, ILInt32 byteCount,
				 System_Array *chars, ILInt32 charIndex)
{
	return (ILInt32)ILAnsiGetChars
		(((ILUInt8 *)ArrayToBuffer(bytes)) + byteIndex,
		 (unsigned long)byteCount,
		 ((ILUInt16 *)ArrayToBuffer(chars)) + charIndex,
		 (unsigned long)(ArrayLength(chars) - charIndex));
}

/*
 * private static int InternalGetMaxByteCount(int charCount);
 */
ILInt32 _IL_DefaultEncoding_InternalGetMaxByteCount
				(ILExecThread *_thread, ILInt32 charCount)
{
	return (ILInt32)ILAnsiGetMaxByteCount((unsigned long)charCount);
}

/*
 * private static int InternalGetMaxCharCount(int byteCount);
 */
ILInt32 _IL_DefaultEncoding_InternalGetMaxCharCount
				(ILExecThread *_thread, ILInt32 byteCount)
{
	return (ILInt32)ILAnsiGetMaxCharCount((unsigned long)byteCount);
}

/*
 * private static String InternalGetString(byte[] bytes, int index, int count);
 */
ILString *_IL_DefaultEncoding_InternalGetString
				(ILExecThread *_thread, System_Array *bytes,
				 ILInt32 index, ILInt32 count)
{
	System_String *str;
	ILInt32 charCount = (ILInt32)ILAnsiGetCharCount
			(((ILUInt8 *)ArrayToBuffer(bytes)) + index,
			 (unsigned long)count);
	if(charCount < 0)
	{
		return 0;
	}
	str = _IL_String_NewString(_thread, charCount);
	if(!str)
	{
		return 0;
	}
	ILAnsiGetChars(((ILUInt8 *)ArrayToBuffer(bytes)) + index,
				   (unsigned long)count,
			       StringToBuffer(str),
				   (unsigned long)(str->length));
	return (ILString *)str;
}

/*
 * internal static int InternalCodePage();
 */
ILInt32 _IL_DefaultEncoding_InternalCodePage(ILExecThread *_thread)
{
	return (ILInt32)ILGetCodePage();
}

/*
 * private static int InternalCultureID();
 */
ILInt32 _IL_CultureInfo_InternalCultureID(ILExecThread *_thread)
{
	return (ILInt32)ILGetCultureID();
}

/*
 * private static String InternalCultureName();
 */
ILString *_IL_CultureInfo_InternalCultureName(ILExecThread *_thread)
{
	char *name = ILGetCultureName();
	if(!name)
	{
		return ILStringCreate(_thread, "iv");
	}
	else
	{
		ILString *str = ILStringCreate(_thread, name);
		ILFree(name);
		return str;
	}
}

#ifdef	__cplusplus
};
#endif
