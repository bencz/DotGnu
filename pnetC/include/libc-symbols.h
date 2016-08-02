/*
 * libc-symbols.h - Macros that help with compiling imported glibc code.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

#ifndef _LIBC_SYMBOLS_H
#define	_LIBC_SYMBOLS_H

#define _LIBC		1
#define _GNU_SOURCE	1
#define	_REENTRANT	1

#define	_strong_alias(name,aliasname)	\
    extern __typeof__ (name) aliasname __attribute__ ((alias (#name)));
#define	strong_alias(name,aliasname)	_strong_alias(name, aliasname)
#define	weak_alias(name,aliasname)		_strong_alias(name, aliasname)
#if 0	/* TODO: aliases aren't supported in the compiler yet */
#define	_weak_alias(name,aliasname)	\
    extern __typeof__ (name) aliasname __attribute__ ((weak, alias (#name)));
#define	weak_alias(name,aliasname)	_weak_alias(name, aliasname)
#endif

#define internal_function
#define	__builtin_expect(expr, val)	(expr)

#define	__set_errno(e)	(errno = (e))

#define	link_warning(a,b)
#define	__libc_fatal(a)

#endif /* _LIBC_SYMBOLS_H */
