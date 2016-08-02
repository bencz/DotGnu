/*
 * memory.c - System memory manipulation routines.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#ifndef HAVE_MEMSET

void *ILMemSet(void *dest, int ch, unsigned long len)
{
	unsigned char *d = (unsigned char *)dest;
	while(len > 0)
	{
		*d++ = (unsigned char)ch;
		--len;
	}
	return dest;
}

#ifndef HAVE_BZERO

void ILMemZero(void *dest, unsigned long len)
{
	unsigned char *d = (unsigned char *)dest;
	while(len > 0)
	{
		*d++ = (unsigned char)0;
		--len;
	}
}

#endif	/* !HAVE_BZERO */
#endif	/* !HAVE_MEMSET */

#if !defined(HAVE_MEMCPY) && !defined(HAVE_BCOPY)

void *ILMemCpy(void *dest, const void *src, unsigned long len)
{
	unsigned char *d = (unsigned char *)dest;
	const unsigned char *s = (const unsigned char *)src;
	while(len > 0)
	{
		*d++ = *s++;
		--len;
	}
	return dest;
}

#endif	/* !HAVE_MEMCPY && !HAVE_BCOPY */

#ifndef HAVE_MEMMOVE

void *ILMemMove(void *dest, const void *src, unsigned long len)
{
	unsigned char *d = (unsigned char *)dest;
	const unsigned char *s = (const unsigned char *)src;
	if(((const unsigned char *)d) < s)
	{
		while(len > 0)
		{
			*d++ = *s++;
			--len;
		}
	}
	else
	{
		d += len;
		s += len;
		while(len > 0)
		{
			*(--d) = *(--s);
			--len;
		}
	}
	return dest;
}

#endif	/* !HAVE_MEMMOVE */

#if !defined(HAVE_MEMCMP) && !defined(HAVE_BCMP)

int ILMemCmp(const void *s1, const void *s2, unsigned long len)
{
	const unsigned char *str1 = (const unsigned char *)s1;
	const unsigned char *str2 = (const unsigned char *)s2;
	while(len > 0)
	{
		if(*str1 < *str2)
			return -1;
		else if(*str1 > *str2)
			return 1;
		++str1;
		++str2;
		--len;
	}
	return 0;
}

#endif	/* !HAVE_MEMCMP && !HAVE_BCMP */

#ifndef HAVE_MEMCHR

void *ILMemChr(const void *str, int ch, unsigned long len)
{
	const unsigned char *s = (const unsigned char *)str;
	while(len > 0)
	{
		if(*s == (unsigned char)ch)
		{
			return (void *)s;
		}
		++s;
		--len;
	}
	return (void *)0;
}

#endif	/* !HAVE_MEMCHR */

#ifdef	__cplusplus
};
#endif
