/*
 * filemap.c - Memory-mapped file support routines.
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
#include "mem_debug.h"
#if defined(WIN32) || defined(_WIN32) || defined(__CYGWIN__)
	#include <windows.h>
	#include <io.h>
	#define	IL_USE_WIN32_MMAP
#else
#ifdef HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#ifdef HAVE_SYS_MMAN_H
	#include <sys/mman.h>
#endif
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Perform an mmap on a file and test whether it succeeded or not.
 */
#ifndef IL_USE_WIN32_MMAP

#if defined(HAVE_MMAP) && defined(HAVE_MUNMAP)

/* We have a Unix-style mmap available to us */
#ifndef IL_MEMUSAGE_DEBUG
#define	mmapPerform(fd,offset,len,end)	\
			(mmap((void *)0, (len), PROT_READ, MAP_SHARED, (fd), (offset)))
#else
#define	mmapPerform(fd,offset,len,end)	((void *)(-1))
#endif
#define	mmapInvalid(addr)	\
			((addr) == ((void *)0) || (addr) == ((void *)(-1)))

#else	/* !HAVE_MMAP || !HAVE_MUNMAP */

/* We don't have any way to map files */
#define	mmapPerform(fd,offset,len,end)	(0)
#define	mmapInvalid(addr)				(1)

#endif	/* !HAVE_MMAP || !HAVE_MUNMAP */

#else	/* IL_USE_WIN32_MMAP */

/* We are using Windows-specific API's to map files */
static void *mmapPerform(int fd, unsigned long offset,
						 unsigned long len, unsigned long end)
{
	HANDLE osHandle;
	HANDLE mapHandle;
	void *mapAddress;

	/* Get the underlying OS handle for the fd */
#ifdef IL_WIN32_CYGWIN
	osHandle = (HANDLE)get_osfhandle(fd);
#else
	osHandle = (HANDLE)_get_osfhandle(fd);
#endif
	if(osHandle == (HANDLE)INVALID_HANDLE_VALUE)
	{
		return 0;
	}

	/* Under Windows, we cannot map bytes beyond the end of the file,
	   so we need to clamp the length to stay within the file's extent */
	if((offset + len) > end)
	{
		len = end - offset;
	}

	/* Attempt to map the file */
	mapHandle = CreateFileMapping(osHandle, NULL, PAGE_READONLY,
								  0, 0, NULL);
	if(mapHandle == (HANDLE)NULL)
	{
		return 0;
	}
	mapAddress = MapViewOfFile(mapHandle, FILE_MAP_READ, 0, offset, len);

	/* Close the mapping object, which we no longer require */
	CloseHandle(mapHandle);

	/* Return the memory pointer to the caller */
	return mapAddress;
}

#define	mmapInvalid(addr)		((addr) == ((void *)0))

#endif	/* IL_USE_WIN32_MMAP */

/*
 * Map a region of a file to memory.  Returns zero if not possible.
 */
int ILMapFileToMemory(int fd, unsigned long start, unsigned long end,
					  void **mapAddress, unsigned long *mapLength,
					  char **addrOfStart)
{
	unsigned long pageSize;
	unsigned long mapStart;
	unsigned long mapEnd;

	/* Determine the size of a page */
	pageSize = ILPageMapSize();

	/* Round the start and end to page boundaries */
	mapStart = start & ~(pageSize - 1);
	mapEnd = (end + pageSize - 1) & ~(pageSize - 1);

	/* Attempt to map the file */
	*mapAddress = mmapPerform(fd, mapStart, mapEnd - mapStart, end);
	if(mmapInvalid(*mapAddress))
	{
		/* Could not mmap this type of file: it may be a pipe or socket */
		return 0;
	}

	/* Return the map length and the address of "start" to the caller */
	*mapLength = mapEnd - mapStart;
	*addrOfStart = ((char *)(*mapAddress)) + (start - mapStart);
	return 1;
}

/*
 * Unmap a block of memory.
 */
void ILUnmapFileFromMemory(void *addr, unsigned long len)
{
#ifndef IL_USE_WIN32_MMAP
#if defined(HAVE_MMAP) && defined(HAVE_MUNMAP)
	munmap(addr, len);
#endif
#else
	UnmapViewOfFile(addr);
#endif
}

#ifdef	__cplusplus
};
#endif
