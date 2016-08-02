/*
 * stdarg.h - ANSI C header file for variable argument list support.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef _STDARG_H
#define _STDARG_H

#ifdef  __cplusplus
extern  "C" {
#endif

typedef __builtin_va_list   va_list;
#define va_start            __builtin_va_start
#define va_end(__va)        (__builtin_va_end((__va)))
#define va_arg(__va,__type) (__builtin_va_arg((__va),__type))

#undef __need___va_list

#ifdef  __cplusplus
};
#endif

#endif  /* !_STDARG_H */
