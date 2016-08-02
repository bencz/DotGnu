/*
 * assert.h - Debug assertion support.
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

#ifndef _ASSERT_H
#define _ASSERT_H

#include <features.h>

__BEGIN_DECLS

extern void __assert_fail(const char *__assertion, const char *__file,
                          unsigned int __line, const char *__function);
extern void __assert(const char *__assertion, const char *__file,
                     unsigned int __line);

#ifdef NDEBUG
#define	assert(expr)    ((void)0)
#else
#define	assert(expr)    \
    ((void)((expr) ? 0 : __assert_fail(#expr, __FILE__, __LINE__, __func__)))
#endif

__END_DECLS

#endif  /* !_ASSERT_H */
