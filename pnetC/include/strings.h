/*
 * strings.h - Additional string manipulation functions.
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

#ifndef _STRINGS_H
#define _STRINGS_H

#include <features.h>
#include <stddef.h>

__BEGIN_DECLS

extern int bcmp(const void *__s1, const void *__s2, size_t __n);
extern void bcopy(const void *__src, void *__dest, size_t __n);
extern void bzero(void *__s, size_t __n);
extern int ffs(int __i);
extern char *index(const char *__s, int __c);
extern char *rindex(const char *__s, int __c);
extern int strcasecmp(const char *__s1, const char *__s2);
extern int strncasecmp(const char *__s1, const char *__s2, size_t __n);

__END_DECLS

#endif  /* !_STRINGS_H */
