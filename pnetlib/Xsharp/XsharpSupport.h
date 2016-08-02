/*
 * XsharpSupport.h - C support code for Xsharp.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#ifndef _XSHARPSUPPORT_H
#define	_XSHARPSUPPORT_H

#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <X11/Xatom.h>
#ifdef WIN32
	#include <X11/Xwinsock.h>
#endif
#ifdef USE_XFT_EXTENSION
	#include <X11/Xft/Xft.h>
	#include <wchar.h>
#endif
#if TIME_WITH_SYS_TIME
	#include <sys/time.h>
    #include <time.h>
#else
    #if HAVE_SYS_TIME_H
		#include <sys/time.h>
    #else
        #include <time.h>
    #endif
#endif
#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#if HAVE_SYS_SELECT_H
	#include <sys/select.h>
#endif
#if HAVE_UNISTD_H
	#include <unistd.h>
#endif
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

/*
 * Font style flags.
 */
#define	FontStyle_Normal		0
#define	FontStyle_Bold			1
#define	FontStyle_Italic		2
#define	FontStyle_Underline		4
#define	FontStyle_StrikeOut		8
#define	FontStyle_NoDefault		0x40
#define	FontStyle_FontStruct	0x80

/*
 * Structure of Portable.NET's "System.String" object for direct
 * access to the Unicode contents, bypassing PInvoke string conversion.
 *
 * Use the "ILStringLength" and "ILStringToBuffer" macros, so that
 * 32-bit vs 64-bit system differences are properly dealt with.
 */
typedef unsigned short ILChar;
typedef struct _tagILString ILString;
typedef struct
{
	int		capacity;
	int		length;

} System_String_int;
typedef struct
{
	long	capacity;
	long	length;

} System_String_long;
#define	ILStringLength(str)	\
		(sizeof(int) == 4 \
			? ((System_String_int *)(str))->length \
			: ((System_String_long *)(str))->length)
#define	ILStringToBuffer(str)	\
		(sizeof(int) == 4 \
			? (ILChar *)(((System_String_int *)(str)) + 1) \
			: (ILChar *)(((System_String_long *)(str)) + 1))

/*
 * Import common declarations.
 */
void XSharpTextExtentsStruct(Display *dpy, void *fontSet,
					         ILString *str, long offset, long count,
							 XRectangle *overall_ink_return,
					         XRectangle *overall_logical_return);
void XSharpFontExtentsStruct(void *fontSet,
					         XRectangle *max_ink_return,
					         XRectangle *max_logical_return);

#endif /* _XSHARPSUPPORT_H */
