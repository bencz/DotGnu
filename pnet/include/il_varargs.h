/*
 * il_varargs.h - variable argument handling declarations.
 *
 * Copyright (C) 2009  Free Software Foundation, Inc.
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

#ifndef	_IL_VARARGS_H
#define	_IL_VARARGS_H

#include "il_config.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef HAVE_STDARG_H
#include <stdarg.h>
#define	IL_VA_LIST			va_list
#define	IL_VA_START(arg)	va_list va; va_start(va, arg)
#define	IL_VA_END			va_end(va)
#define	IL_VA_ARG(va, type)	va_arg(va, type)
#define	IL_VA_GET_LIST		va
#else
#ifdef HAVE_VARARGS_H
#include <varargs.h>
#define	IL_VA_LIST			va_list
#define	IL_VA_START(arg)	va_list va; va_start(va)
#define	IL_VA_END			va_end(va)
#define	IL_VA_ARG(va, type)	va_arg(va, type)
#define	IL_VA_GET_LIST		va
#else
#define	IL_VA_LIST			int
#define	IL_VA_START(arg)
#define	IL_VA_END
#define	IL_VA_ARG(va, type)	((type)0)
#define	IL_VA_GET_LIST		0
#endif
#endif

/*
 * Some gcc magic to make it check for correct printf
 * arguments when using the error reporting functions.
 */
#if defined(__GNUC__)
	#define	IL_PRINTF(str,arg) \
		__attribute__((__format__ (__printf__, str, arg)))
#else
	#define	IL_PRINTF(str,arg)
#endif

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_VARARGS_H */
