/*
 * lib_stdio.c - Internalcall methods for "Platform.Stdio".
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

/*

Note: it is possible to access stdin/stdout/stderr through regular
file stream operations using descriptors 0, 1, or 2.  However, those
versions don't go through the regular C stdio routines, and won't
work on platforms that don't have Unix-style file descriptors.

This also provides a convenient place where "System.Console" can be
redirected to somewhere other than the C stdio routines, which may
be useful for debugging in embedded environments.

*/

#include "engine.h"
#include "lib_defs.h"
#include "il_utils.h"
#include "il_console.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static void StdClose(int fd);
 */
void _IL_Stdio_StdClose(ILExecThread *thread, ILInt32 fd)
{
	/* We don't currently do anything as allowing programs
	   to close stdin/stdout/stderr is fraught with danger */
}

/*
 * public static void StdFlush(int fd);
 */
void _IL_Stdio_StdFlush(ILExecThread *thread, ILInt32 fd)
{
#ifndef REDUCED_STDIO
	switch(fd)
	{
		case 0:		fflush(stdin); break;
		case 1:		fflush(stdout); break;
		case 2:		fflush(stderr); break;
	}
#endif
}

/*
 * public static void StdWrite(int fd, char value);
 */
void _IL_Stdio_StdWrite_ic(ILExecThread *thread, ILInt32 fd, ILUInt16 value)
{
#ifndef REDUCED_STDIO
	switch(fd)
	{
		case 1:
		{
			if(ILConsoleGetMode() == IL_CONSOLE_NORMAL)
			{
				putc((value & 0xFF), stdout);
			}
			else
			{
				ILConsoleWriteChar(value);
				fflush(stdout);
			}
		}
		break;

		case 2:		putc((value & 0xFF), stderr); break;
	}
#else
	putchar(value);
#endif
}

/*
 * Write a 16-bit character buffer to a file descriptor.
 */
static void StdWrite(ILInt32 fd, ILUInt16 *buf, ILInt32 length)
{
#ifndef REDUCED_STDIO
	if(fd == 1)
	{
		if(ILConsoleGetMode() == IL_CONSOLE_NORMAL)
		{
			while(length > 0)
			{
				putc((*buf & 0xFF), stdout);
				++buf;
				--length;
			}
		}
		else
		{
			while(length > 0)
			{
				ILConsoleWriteChar(*buf);
				++buf;
				--length;
			}
			fflush(stdout);
		}
	}
	else if(fd == 2)
	{
		while(length > 0)
		{
			putc((*buf & 0xFF), stderr);
			++buf;
			--length;
		}
	}
#else
	while(length > 0)
	{
		putchar((*buf & 0xFF));
		++buf;
		--length;
	}
#endif
}

/*
 * public static void StdWrite(int fd, char[] value, int index, int count);
 */
void _IL_Stdio_StdWrite_iacii(ILExecThread *thread, ILInt32 fd,
							  System_Array *value, ILInt32 index,
							  ILInt32 count)
{
	StdWrite(fd, ((ILUInt16 *)ArrayToBuffer(value)) + index, count);
}

/*
 * public static void StdWrite(int fd, byte[] value, int index, int count);
 */
void _IL_Stdio_StdWrite_iaBii(ILExecThread *thread, ILInt32 fd,
							  System_Array *value, ILInt32 index,
							  ILInt32 count)
{
#ifndef REDUCED_STDIO
	if(fd == 1)
	{
		if(ILConsoleGetMode() == IL_CONSOLE_NORMAL)
		{
			fwrite(((ILUInt8 *)ArrayToBuffer(value)) + index, 1, count, stdout);
		}
		else
		{
			ILUInt8 *buf = ((ILUInt8 *)ArrayToBuffer(value)) + index;
			while(count > 0)
			{
				ILConsoleWriteChar(*buf);
				++buf;
				--count;
			}
			fflush(stdout);
		}
	}
	else if(fd == 2)
	{
		fwrite(((ILUInt8 *)ArrayToBuffer(value)) + index, 1, count, stderr);
	}
#else
	unsigned char *buf;
	buf = ((unsigned char *)(ArrayToBuffer(value))) + index;
	while(count > 0)
	{
		putchar((*buf & 0xFF));
		++buf;
		--count;
	}
#endif
}

/*
 * public static void StdWrite(int fd, String value)
 */
void _IL_Stdio_StdWrite_iString(ILExecThread *thread, ILInt32 fd,
								ILString *value)
{
	if(value != 0)
	{
		ILUInt16 *buf;
		ILInt32 len = _ILStringToBuffer(thread, value, &buf);
		StdWrite(fd, buf, len);
	}
}

/*
 * public static int StdRead(int fd);
 */
ILInt32 _IL_Stdio_StdRead_i(ILExecThread *thread, ILInt32 fd)
{
#ifndef REDUCED_STDIO
	if(fd == 0)
	{
		return (ILInt32)(getc(stdin));
	}
	else
#endif
	{
		return -1;
	}
}

/*
 * public static int StdRead(int fd, char[] value, int index, int count);
 */
ILInt32 _IL_Stdio_StdRead_iacii(ILExecThread *thread, ILInt32 fd,
								System_Array *value, ILInt32 index,
								ILInt32 count)
{
#ifndef REDUCED_STDIO
	ILUInt16 *buf = ((ILUInt16 *)(ArrayToBuffer(value))) + index;
	ILInt32 result = 0;
	int ch;
	if(fd != 0)
	{
		return -1;
	}
	while(count > 0)
	{
		ch = getc(stdin);
		if(ch == -1)
		{
			break;
		}
		*buf++ = (ILUInt16)(ch & 0xFF);
		++result;
		--count;
	}
	return result;
#else
	return -1;
#endif
}

/*
 * public static int StdRead(int fd, byte[] value, int index, int count);
 */
ILInt32 _IL_Stdio_StdRead_iaBii(ILExecThread *thread, ILInt32 fd,
								System_Array *value, ILInt32 index,
								ILInt32 count)
{
#ifndef REDUCED_STDIO
	if(fd != 0)
	{
		return -1;
	}
	return (ILInt32)(int)fread
		(((ILUInt8 *)ArrayToBuffer(value)) + index, 1, (int)count, stdin);
#else
	return -1;
#endif
}

/*
 * public static int StdPeek(int fd);
 */
ILInt32 _IL_Stdio_StdPeek(ILExecThread *thread, ILInt32 fd)
{
#ifndef REDUCED_STDIO
	if(fd == 0)
	{
		int ch = getc(stdin);
		if(ch != -1)
		{
			ungetc(ch, stdin);
		}
		return (ILInt32)ch;
	}
	else
#endif
	{
		return -1;
	}
}

/*
 * public static void SetConsoleMode(int mode);
 */
void _IL_Stdio_SetConsoleMode(ILExecThread *_thread, ILInt32 mode)
{
	ILConsoleSetMode(mode);
}

/*
 * public static void Beep(int frequency, int duration);
 */
void _IL_Stdio_Beep(ILExecThread *_thread, ILInt32 frequency, ILInt32 duration)
{
	ILConsoleBeep(frequency, duration);
}

/*
 * public static void Clear();
 */
void _IL_Stdio_Clear(ILExecThread *_thread)
{
	ILConsoleClear();
}

/*
 * public static void ReadKey(out char ch, out int key, out int modifiers);
 */
void _IL_Stdio_ReadKey(ILExecThread *_thread, ILUInt16 *ch,
					   ILInt32 *key, ILInt32 *modifiers)
{
	ILConsoleReadKey(ch, key, modifiers);
}

/*
 * public static void SetCursorPosition(int x, int y);
 */
void _IL_Stdio_SetCursorPosition(ILExecThread *_thread, ILInt32 x, ILInt32 y)
{
	ILConsoleSetPosition(x, y);
}

/*
 * public static int GetTextAttributes();
 */
ILInt32 _IL_Stdio_GetTextAttributes(ILExecThread *_thread)
{
	return ILConsoleGetAttributes();
}

/*
 * public static void SetTextAttributes(int attrs);
 */
void _IL_Stdio_SetTextAttributes(ILExecThread *_thread, ILInt32 attrs)
{
	ILConsoleSetAttributes(attrs);
}

/*
 * public static void GetBufferSize(out int width, out int height);
 */
void _IL_Stdio_GetBufferSize(ILExecThread *_thread, ILInt32 *width,
							 ILInt32 *height)
{
	ILConsoleGetBufferSize(width, height);
}

/*
 * public static void SetBufferSize(int width, int height);
 */
void _IL_Stdio_SetBufferSize(ILExecThread *_thread,
							 ILInt32 width, ILInt32 height)
{
	ILConsoleSetBufferSize(width, height);
}

/*
 * public static void GetCursorPosition(out int x, out int y);
 */
void _IL_Stdio_GetCursorPosition(ILExecThread *_thread, ILInt32 *x,
								 ILInt32 *y)
{
	ILConsoleGetPosition(x, y);
}

/*
 * public static bool KeyAvailable();
 */
ILBool _IL_Stdio_KeyAvailable(ILExecThread *_thread)
{
	return ILConsoleKeyAvailable();
}

/*
 * public static void SetConsoleTitle(String title);
 */
void _IL_Stdio_SetConsoleTitle(ILExecThread *_thread, ILString *title)
{
	char *str = ILStringToAnsi(_thread, title);
	if(str)
	{
		ILConsoleSetTitle(str);
	}
}

/*
 * public static void GetWindowSize(out int left, out int top,
 *									out int width, out int height);
 */
void _IL_Stdio_GetWindowSize(ILExecThread *_thread,
							 ILInt32 *left, ILInt32 *top,
							 ILInt32 *width, ILInt32 *height)
{
	ILConsoleGetWindowSize(left, top, width, height);
}

/*
 * public static void SetWindowSize(int left, int top,
 *									int width, int height);
 */
void _IL_Stdio_SetWindowSize(ILExecThread *_thread,
							 ILInt32 left, ILInt32 top,
							 ILInt32 width, ILInt32 height)
{
	ILConsoleSetWindowSize(left, top, width, height);
}

/*
 * public static void GetLargestWindowSize(out int width, out int height);
 */
void _IL_Stdio_GetLargestWindowSize(ILExecThread *_thread,
									ILInt32 *width, ILInt32 *height)
{
	ILConsoleGetLargestWindowSize(width, height);
}

/*
 * public static void MoveBufferArea(int sourceLeft, int sourceTop,
 *									 int sourceWidth, int sourceHeight,
 *									 int targetLeft, int targetTop,
 *									 char sourceChar, int attributes);
 */
void _IL_Stdio_MoveBufferArea(ILExecThread *_thread,
							  ILInt32 sourceLeft, ILInt32 sourceTop,
							  ILInt32 sourceWidth, ILInt32 sourceHeight,
							  ILInt32 targetLeft, ILInt32 targetTop,
							  ILUInt16 sourceChar, ILInt32 attributes)
{
	ILConsoleMoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight,
							targetLeft, targetTop, sourceChar, attributes);
}

/*
 * public static int GetLockState();
 */
ILInt32 _IL_Stdio_GetLockState(ILExecThread *_thread)
{
	return ILConsoleGetLockState();
}

/*
 * public static int GetCursorSize();
 */
ILInt32 _IL_Stdio_GetCursorSize(ILExecThread *_thread)
{
	return ILConsoleGetCursorSize();
}

/*
 * public static void SetCursorSize(int size);
 */
void _IL_Stdio_SetCursorSize(ILExecThread *_thread, ILInt32 size)
{
	ILConsoleSetCursorSize(size);
}

/*
 * public static bool GetCursorVisible();
 */
ILBool _IL_Stdio_GetCursorVisible(ILExecThread *_thread)
{
	return ILConsoleGetCursorVisible();
}

/*
 * public static void SetCursorVisible(bool flag);
 */
void _IL_Stdio_SetCursorVisible(ILExecThread *_thread, ILBool flag)
{
	ILConsoleSetCursorVisible(flag);
}

#ifdef	__cplusplus
};
#endif
