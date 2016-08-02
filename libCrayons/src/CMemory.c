/*
 * CMemory.c - Memory management and manipulation implementation.
 *
 * Copyright (C) 2005  Free Software Foundation, Inc.
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL void *
CMalloc(CUInt32 size)
{
	return malloc(size);
}

CINTERNAL void *
CRealloc(void     *ptr,
         CUInt32   size)
{
	return realloc(ptr, size);
}

CINTERNAL void *
CCalloc(CUInt32 count,
        CUInt32 size)
{
	return calloc(count, size);
}

CINTERNAL void
CFree(void *ptr)
{
	free(ptr);
}

CINTERNAL void *
CMemSet(void     *_dst,
        CByte     value,
        CUInt32   length)
{
#ifdef HAVE_MEMSET
	return memset(_dst, value, length);
#else
	/* get the data pointer */
	CByte *dst = (CByte *)_dst;

	/* set the memory */
	while(length > 0)
	{
		/* set the current data */
		*dst++ = value;

		/* move to the next position */
		--length;
	}

	/* return the destination */
	return _dst;
#endif
}

CINTERNAL void *
CMemCopy(void       *_dst,
         const void *_src,
         CUInt32     length)
{
#ifdef HAVE_MEMCPY
	return memcpy(_dst, _src, length);
#elif defined(HAVE_BCOPY)
	return bcopy((char *)_src, (char *)_dst, length);
#else
	/* get the source data pointer */
	const CByte *src = (const CByte *)_src;

	/* get the destination data pointer */
	CByte *dst = (CByte *)_dst;

	/* copy the memory */
	while(length > 0)
	{
		/* set the current data */
		*dst++ = *src++;

		/* move to the next position */
		--length;
	}

	/* return the destination */
	return _dst;
#endif
}

CINTERNAL void *
CMemMove(void       *_dst,
         const void *_src,
         CUInt32     length)
{
#ifdef HAVE_MEMMOVE
	return memmove(_dst, _src, length);
#else
	/* get the source data pointer */
	const CByte *src = (const CByte *)_src;

	/* get the destination data pointer */
	CByte *dst = (CByte *)_dst;

	/* move the memory, based on the direction */
	if(dst < src)
	{
		/* move the memory */
		while(length > 0)
		{
			/* set the current data */
			*dst++ = *src++;

			/* move to the next position */
			--length;
		}
	}
	else
	{
		/* set the data pointers to the end */
		src += length;
		dst += length;

		/* move the memory */
		while(length > 0)
		{
			/* set the current data */
			*(--dst) = *(--src);

			/* move to the next position */
			--length;
		}
	}

	/* return the destination */
	return _dst;
#endif
}

CINTERNAL int
CMemCmp(const void *_a,
        const void *_b,
        CUInt32     length)
{
#ifdef HAVE_MEMCMP
	return memcmp(_a, _b, length);
#elif defined(HAVE_BCMP)
	return bcmp((char *)_a, (char *)_b, length);
#else
	/* get the first data pointer */
	const CByte *a = (CByte *)_a;

	/* get the second data pointer */
	const CByte *b = (CByte *)_b;

	/* compare the memory */
	while(length > 0)
	{
		/* bail out now if we've found a difference */
		if(*a > *b) { return  1; }
		if(*a < *b) { return -1; }

		/* move to the next position */
		++a; ++b; --length;
	}

	/* return equality flag */
	return 0;
#endif
}


#ifdef __cplusplus
};
#endif
