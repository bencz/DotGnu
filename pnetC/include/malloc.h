/*
 * malloc.h - Interface to the C memory allocation routines.
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

#ifndef _MALLOC_H
#define _MALLOC_H

#include <features.h>
#include <stddef.h>

__BEGIN_DECLS

extern void *malloc(size_t __size);
extern void *calloc(size_t __nelem, size_t __size);
extern void *realloc(void *__ptr, size_t __size);
extern void free(void *__ptr);
extern void cfree(void *__ptr);

__END_DECLS

#endif  /* !_MALLOC_H */
