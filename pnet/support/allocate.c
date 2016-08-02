/*
 * allocate.c - System memory allocation routines.
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

#include "thr_defs.h"
#include "il_system.h"
#include "il_align.h"
#include "mem_debug.h"
#ifdef HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#if !(defined(_WIN32) || defined(WIN32) || defined(__CYGWIN__))
#ifdef HAVE_SYS_MMAN_H
	#include <sys/mman.h>
#endif
#endif
#ifdef HAVE_FCNTL_H
	#include <fcntl.h>
#endif
#ifdef _WIN32
	#include <windows.h>
	#include <io.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * If memory usage debugging is enabled, then track how many
 * bytes are malloc'ed by the system.  This doesn't count the
 * heap overhead due to rounding of block sizes.
 */
#ifdef IL_MEMUSAGE_DEBUG
static unsigned long mallocBytes = 0;
static unsigned long mallocMax = 0;
#define	MEM_TO_DATA(ptr)	\
			((void *)(((unsigned char *)ptr) + IL_BEST_ALIGNMENT))
#define	MEM_FROM_DATA(ptr)	\
			((void *)(((unsigned char *)ptr) - IL_BEST_ALIGNMENT))
#endif

long _ILMallocMaxUsage(void)
{
#ifdef IL_MEMUSAGE_DEBUG
	return (long)mallocMax;
#else
	return -1;
#endif
}

void *ILMalloc(unsigned long size)
{
#ifdef IL_MEMUSAGE_DEBUG
	void *ptr = malloc(size + IL_BEST_ALIGNMENT);
	if(ptr)
	{
		mallocBytes += size;
		if(mallocBytes > mallocMax)
		{
			mallocMax = mallocBytes;
		}
		*((unsigned long *)ptr) = size;
		return MEM_TO_DATA(ptr);
	}
	else
	{
		return 0;
	}
#else
	return malloc(size);
#endif
}

void *ILRealloc(void *ptr, unsigned long size)
{
#ifdef IL_MEMUSAGE_DEBUG
	unsigned long prevSize;
	if(!ptr)
	{
		return ILMalloc(size);
	}
	prevSize = *((unsigned long *)(MEM_FROM_DATA(ptr)));
	ptr = realloc(MEM_FROM_DATA(ptr), size + IL_BEST_ALIGNMENT);
	if(ptr)
	{
		mallocBytes = mallocBytes - prevSize + size;
		if(mallocBytes > mallocMax)
		{
			mallocMax = mallocBytes;
		}
		*((unsigned long *)ptr) = size;
		return MEM_TO_DATA(ptr);
	}
	else
	{
		return 0;
	}
#else
	return realloc(ptr, size);
#endif
}

void *ILCalloc(unsigned long nelems, unsigned long size)
{
#ifdef IL_MEMUSAGE_DEBUG
	void *ptr;
	size *= nelems;
	ptr = calloc(size + IL_BEST_ALIGNMENT, 1);
	if(ptr)
	{
		mallocBytes += size;
		if(mallocBytes > mallocMax)
		{
			mallocMax = mallocBytes;
		}
		*((unsigned long *)ptr) = size;
		return MEM_TO_DATA(ptr);
	}
	else
	{
		return 0;
	}
#else
	return calloc(nelems, size);
#endif
}

void ILFree(void *ptr)
{
#ifdef IL_MEMUSAGE_DEBUG
	if(ptr)
	{
		unsigned long size = *((unsigned long *)(MEM_FROM_DATA(ptr)));
		mallocBytes -= size;
		free(MEM_FROM_DATA(ptr));
	}
#else
	free(ptr);
#endif
}

char *ILDupString(const char *str)
{
	if(str)
	{
		char *newstr = (char *)ILMalloc(strlen(str) + 1);
		if(newstr)
		{
			strcpy(newstr, str);
		}
		return newstr;
	}
	else
	{
		return 0;
	}
}

char *ILDupNString(const char *str, int len)
{
	char *newstr = (char *)ILMalloc(len + 1);
	if(newstr)
	{
		if(len > 0)
		{
			ILMemCpy(newstr, str, len);
		}
		newstr[len] = '\0';
	}
	return newstr;
}

unsigned long ILPageAllocSize(void)
{
#ifndef _WIN32
	/* Get the page size using a Unix-like sequence */
	#ifdef HAVE_GETPAGESIZE
		return (unsigned long)getpagesize();
	#else
		#ifdef NBPG
			return NBPG;
		#else
			#ifdef PAGE_SIZE
				return PAGE_SIZE;
			#else
				return 4096;
			#endif
		#endif
	#endif
#else
	/* Get the page size from a Windows-specific API */
	SYSTEM_INFO sysInfo;
	GetSystemInfo(&sysInfo);
	return (unsigned long)(sysInfo.dwPageSize);
#endif
}

unsigned long ILPageMapSize(void)
{
#ifndef _WIN32
	/* Get the page size using a Unix-like sequence */
	#ifdef HAVE_GETPAGESIZE
		return (unsigned long)getpagesize();
	#else
		#ifdef NBPG
			return NBPG;
		#else
			#ifdef PAGE_SIZE
				return PAGE_SIZE;
			#else
				return 4096;
			#endif
		#endif
	#endif
#else
	/* Get the page size from a Windows-specific API */
	SYSTEM_INFO sysInfo;
	GetSystemInfo(&sysInfo);
	return (unsigned long)(sysInfo.dwAllocationGranularity);
#endif
}

/*
 * Determine if we should use the system's malloc heap.
 */
#if defined(_WIN32) || defined(WIN32) || defined(__CYGWIN__) || \
    !(defined(HAVE_MMAP) && defined(HAVE_MUNMAP) && defined(HAVE_OPEN))
    #define IL_USE_MALLOC_FOR_PAGES
#endif

#ifndef IL_USE_MALLOC_FOR_PAGES

/*
 * Make sure that "MAP_ANON" is correctly defined, because it
 * may not exist on some variants of Unix.
 */
#ifndef MAP_ANON
    #ifdef MAP_ANONYMOUS
        #define MAP_ANON        MAP_ANONYMOUS
    #endif
#endif

/*
 * Control variables for page allocation.
 */
static int zero_fd = -1;

/*
 * Initialize the page allocation subsystem.  Only called once.
 */
static void PageInit(void)
{
#ifndef IL_MEMUSAGE_DEBUG
	void *addr;
#ifdef MAP_ANON
	addr = mmap((void *)0, ILPageAllocSize(),
				PROT_READ | PROT_WRITE | PROT_EXEC,
				MAP_PRIVATE | MAP_ANON, -1, 0);
	if(addr == (void *)(-1))
	{
		zero_fd = -1;
	}
	else
	{
		munmap(addr, ILPageAllocSize());
		zero_fd = 0;
	}
#else /* !MAP_ANON */
	zero_fd = open("/dev/zero", O_RDWR, 0);
	if(zero_fd != -1)
	{
		/* Set the descriptor to "no-inherit" */
		fcntl(zero_fd, F_SETFD, 1);

		/* Try to allocate a page, which should tell us if "/dev/zero"
		   is actually live and working on this platform */
		addr = mmap((void *)0, ILPageAllocSize(),
				    PROT_READ | PROT_WRITE | PROT_EXEC,
				    MAP_SHARED, zero_fd, 0);
		if(addr == (void *)(-1))
		{
			close(zero_fd);
			zero_fd = -1;
		}
		else
		{
			munmap(addr, ILPageAllocSize());
		}
	}
#endif /* !MAP_ANON */
#endif
}

#endif

void *ILPageAlloc(unsigned long size)
{
#ifdef IL_USE_MALLOC_FOR_PAGES
	return ILCalloc(size, 1);
#else
	/* Initialize the page allocation routines */
	_ILCallOnce(PageInit);

	/* Determine if we can should mmap or calloc */
	if(zero_fd != -1)
	{
	#ifdef MAP_ANON
		void *addr = mmap((void *)0, size,
						  PROT_READ | PROT_WRITE | PROT_EXEC,
						  MAP_PRIVATE | MAP_ANON, -1, 0);
	#else
		void *addr = mmap((void *)0, size,
						  PROT_READ | PROT_WRITE | PROT_EXEC,
						  MAP_SHARED, zero_fd, 0);
	#endif
		if(addr == (void *)(-1))
		{
			/* The allocation failed */
			return 0;
		}
		else
		{
			return addr;
		}
	}
	else
	{
		return ILCalloc(size, 1);
	}
#endif
}

void  ILPageFree(void *ptr, unsigned long size)
{
#ifdef IL_USE_MALLOC_FOR_PAGES
	ILFree(ptr);
#else
	if(zero_fd != -1)
	{
		munmap(ptr, size);
	}
	else
	{
		ILFree(ptr);
	}
#endif
}

#ifdef	__cplusplus
};
#endif
