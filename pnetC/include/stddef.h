/*
 * stddef.h - ANSI C common definitions.
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

/* Handle the glibc __need_XXXX macros */
#undef __need_something_stddef
#if defined(__need_ptrdiff_t) || defined(__need_wchar_t) || \
    defined(__need_wint_t) || defined(__need_size_t) || \
    defined(__need_NULL)
#define	__need_something_stddef
#endif
#if (!defined(_STDDEF_H) && !defined(_STDDEF_H_) && \
     !defined(_ANSI_STDDEF_H) && !defined(__STDDEF_H__)) || \
     defined(__need_something_stddef)

#if !defined(__need_something_stddef)
#define _STDDEF_H
#define _STDDEF_H_
#define _ANSI_STDDEF_H
#define	__STDDEF_H__
#endif

#if !defined(__need_something_stddef) || defined(__need_ptrdiff_t)
#ifndef ptrdiff_t
typedef __PTRDIFF_TYPE__        ptrdiff_t;
#define ptrdiff_t               ptrdiff_t
#endif
#undef __need_ptrdiff_t
#endif

#if !defined(__need_something_stddef) || defined(__need_wchar_t)
#ifndef wchar_t
typedef __WCHAR_TYPE__          wchar_t;
#define wchar_t                 wchar_t
#endif
#undef __need_wchar_t
#endif

#if !defined(__need_something_stddef) || defined(__need_wint_t)
#ifndef wint_t
typedef __WINT_TYPE__           wint_t;
#define wint_t                  wint_t
#endif
#undef __need_wint_t
#define _WINT_T
#endif

#if !defined(__need_something_stddef) || defined(__need_size_t)
#ifndef size_t
typedef __SIZE_TYPE__           size_t;
#define size_t                  size_t
#endif
#undef __need_size_t
#endif

#if !defined(__need_something_stddef) || defined(__need_NULL)
#ifndef NULL
#define NULL                    0
#endif
#undef __need_NULL
#endif

#if !defined(__need_something_stddef)
#ifndef offsetof
#define offsetof(type,member)   ((size_t)(&((type *)0)->member))
#endif
#endif

#endif /* !_STDDEF_H || __need_something_stddef */
#undef __need_something_stddef
