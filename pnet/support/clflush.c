/*
 * clflush.c - Flush CPU cache lines.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

/*
 * Note: if you port this function to another platform, you must
 * also update the #ifdef logic at the bottom of "il_system.h"
 * that detects if cache flushing is available.
 */

void ILCacheFlush(void *buf, long length)
{
	/* There's no code here for i386, because it isn't needed */
#if IL_HAVE_CACHE_FLUSH
#if defined(PPC)

	/* Flush the CPU cache on PPC platforms */
	register unsigned char *p;
	register int count;
	const int cache_line_size = 8; /* minimum is 8 - 32 in most boxen */

	/* find start of cache block */
	p = (unsigned char *)((long)buf & ~(cache_line_size - 1));
	/* find end of cache block */
	count = length + (((long)buf + cache_line_size) 
									& (cache_line_size - 1));
	while(count > 0)
	{
		/* Flush the data cache (coherence) */
		__asm__ __volatile__ ("dcbf 0,%0" :: "r"(p));
		/* Invalidate the cache lines in the instruction cache */
		__asm__ __volatile__ ("icbi 0,%0" :: "r"(p));
		p += cache_line_size;
		count -= cache_line_size;
	}
	__asm__ __volatile__ ("sync");
	__asm__ __volatile__ ("isync");

#elif defined(__sparc)

	/* Flush the CPU cache on sparc platforms */
	register unsigned char *p = (unsigned char *)buf;
	__asm__ __volatile__ ("stbar");
	while(length > 0)
	{
		__asm__ __volatile__ ("flush %0" :: "r"(p));
		p += 4;
		length -= 4;
	}
	__asm__ __volatile__ ("nop; nop; nop; nop; nop");

#elif (defined(__arm__) || defined(__arm)) && defined(linux)

	/* ARM Linux has a "cacheflush" system call */
	/* R0 = start of range, R1 = end of range, R3 = flags */
	/* flags = 0 indicates data cache, flags = 1 indicates both caches */
	__asm __volatile ("mov r0, %0\n"
	                  "mov r1, %1\n"
					  "mov r2, %2\n"
					  "swi 0x9f0002       @ sys_cacheflush"
					  : /* no outputs */
					  : "r" (buf),
					    "r" (((int)buf) + (int)length),
						"r" (0)
					  : "r0", "r1", "r3" );

#elif (defined(__ia64) || defined(__ia64__)) && defined(linux)
	void *end = (char*)buf + length;
	while(buf < end)
	{
		asm volatile("fc %0" :: "r"(buf));
		buf = (char*)buf + 32;
	}
	asm volatile(";;sync.i;;srlz.i;;");
#endif
#endif /* IL_HAVE_CACHE_FLUSH */
}

#ifdef	__cplusplus
};
#endif
