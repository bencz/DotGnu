/*
 * string.h - String manipulation functions.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2002, 2004  Southern Storm Software, Pty Ltd.
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

#ifndef _STRING_H
#define _STRING_H

#include <features.h>
#include <stddef.h>

__BEGIN_DECLS

extern void *memccpy(void * __restrict __dest, const void * __restrict __src,
                     int __c, size_t __n);
extern void *memchr(const void *__s, int __c, size_t __n);
extern int   memcmp(const void *__s1, const void *__s2, size_t __n);
extern void *memcpy(void * __restrict __dest, const void * __restrict __src,
                    size_t __n);
extern void *memmove(void *__dest, const void *__src, size_t __n);
extern void *memset(void *__s, int __c, size_t __n);

extern char *strcat(char * __restrict __dest, const char * __restrict __src);
extern char *strchr(const char * __restrict __s, int __c);
extern int strcmp(const char *__s1, const char *__s2);
extern int strcoll(const char *__s1, const char *__s2);
extern char *strcpy(char * __restrict __dest, const char * __restrict __src);
extern size_t strcspn(const char *__s, const char *__reject);
extern char *strdup(const char *__s);
extern char *strerror(int __errnum);
extern char *strerror_r(int __errnum, char *__buf, size_t __buflen);
extern size_t strlen(const char *__s);
extern char *strncat(char * __restrict __dest, const char * __restrict __src,
                     size_t __n);
extern int strncmp(const char *__s1, const char *__s2, size_t __n);
extern char *strncpy(char * __restrict __dest, const char * __restrict __src,
                     size_t __n);
extern char *strpbrk(const char *__s, const char *__accept);
extern char *strrchr(const char * __restrict __s, int __c);
extern char *strsignal(int __sig);
extern size_t strspn(const char *__s, const char *__accept);
extern char *strstr(const char *__haystack, const char *__needle);
extern char *strtok(char * __restrict __s, const char * __restrict __delim);
extern char *strtok_r(char *__s, const char *__delim, char **__ptrptr);
extern size_t strxfrm(char * __restrict __dest, const char * __restrict __src,
                      size_t n);

__END_DECLS

#include <strings.h>

#endif  /* !_STRING_H */
