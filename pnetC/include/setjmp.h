/*
 * setjmp.h - ANSI C header file for setjmp/longjmp support.
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

#ifndef _SETARG_H
#define _SETARG_H

#include <features.h>

__BEGIN_DECLS

typedef int                     jmp_buf[1];
#define setjmp(__env)           (__builtin_setjmp((__env)))
extern void longjmp(jmp_buf __env, int __val);

__END_DECLS

#endif  /* !_SETARG_H */
