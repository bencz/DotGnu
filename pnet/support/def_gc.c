/*
 * def_gc.c - Default garbage collector for small embedded systems.
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

/*

This "garbage collector" is intended for use on small embedded systems
where there is insufficient memory to use "libgc" or any other kind of
"real" garbage collector.

This collector doesn't actually collect anything.  It allocates a
fixed-sized heap and keeps allocating until the block runs out of
space.  This is suitable for embedded applications that strictly
control their memory usage.

*/

#include "thr_defs.h"
#include "il_align.h"

#ifndef HAVE_LIBGC

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * The default heap size.  The "IL_CONFIG_GC_HEAP_SIZE" variable can
 * be set to a non-zero value in the profile to override this value.
 */
#define	IL_DEFAULT_GC_HEAP_SIZE		(128L * 1024L)

/*
 * Global data for the GC heap.
 */
static _ILMutex gcLock;
static int initialized = 0;
static void *heapMemory = 0;
static unsigned long heapSize = 0;
static unsigned long heapPosn = 0;
static int collectionCount = 0;

/*
 * Initialize the thread locking objects needed to control
 * access to the garbage collection routines.
 */
static void GCLockInit(void)
{
	_ILMutexCreate(&gcLock);
}

void ILGCInit(unsigned long maxSize)
{
	unsigned long pageSize;

	/* Initialize the "gcLock" mutex */
	ILThreadInit();
	_ILCallOnce(GCLockInit);

	/* Bail out if we've already done this */
	_ILMutexLock(&gcLock);
	if(initialized)
	{
		_ILMutexUnlock(&gcLock);
		return;
	}
	initialized = 1;

	/* Use the default heap size if necessary */
	if(!maxSize)
	{
		maxSize = IL_DEFAULT_GC_HEAP_SIZE;
	}

	/* Round the size up to a multiple of the allocation page size */
	pageSize = ILPageAllocSize();
	maxSize = ((maxSize + pageSize - 1) & ~(pageSize - 1));
	if(!maxSize)
	{
		maxSize = pageSize;
	}

	/* Allocate page-based memory for the GC heap */
	heapMemory = ILPageAlloc(maxSize);
	if(heapMemory)
	{
		heapSize = maxSize;
	}
	else
	{
		heapSize = 0;
	}
	heapPosn = 0;

	/* We are ready to go */
	_ILMutexUnlock(&gcLock);
}

void ILGCDeinit()
{
}

void *ILGCAlloc(unsigned long size)
{
	void *ptr;

	/* Lock down the heap while we are trying to allocate from it */
	_ILMutexLock(&gcLock);

	/* Round the size to the best alignment for this platform */
	size = ((size + IL_BEST_ALIGNMENT - 1) & ~(IL_BEST_ALIGNMENT - 1));

	/* See if we have sufficient room for the requested block */
	if((heapSize - heapPosn) >= size)
	{
		/* Allocate the next "size" bytes of memory */
		ptr = (void *)(((unsigned char *)heapMemory) + heapPosn);
		heapPosn += size;
	}
	else
	{
		/* The heap is full and there is nothing that we can do about it */
		ptr = 0;
	}

	/* Unlock the heap and return */
	_ILMutexUnlock(&gcLock);
	return ptr;
}

void *ILGCAllocAtomic(unsigned long size)
{
	/* There's no difference between normal and atomic memory */
	return ILGCAlloc(size);
}

void *ILGCAllocPersistent(unsigned long size)
{
	/* There's no difference between normal and persistent memory */
	return ILGCAlloc(size);
}

void ILGCFreePersistent(void *block)
{
	/* Nothing to do here */
}

void ILGCRegisterFinalizer(void *block, ILGCFinalizer func, void *data)
{
	/* Since we never collect, the finalizers will never be called
	   and so we don't need to worry about registering them */
}


ILNativeInt ILGCCreateTypeDescriptor(ILNativeUInt bitmap[], ILNativeUInt len)
{
	/* Type descriptors aren't supported */
	return 0;
}

void *ILGCAllocExplicitlyTyped(unsigned long size, ILNativeInt descriptor)
{
	return ILGCAlloc(size);
}

void ILGCMarkNoPointers(void *start, unsigned long size)
{
	/* Nothing to do here */
}

void ILGCCollect(void)
{
	/* We don't care about threads here because it's a fake value anyways. */
	collectionCount++;
}

int ILGCFullCollection(int timeout)
{
	/* We don't care about threads here because it's a fake value anyways. */
	collectionCount++;

	return 1;
}

int ILGCCollectionCount(void)
{
	return collectionCount;
}

int ILGCInvokeFinalizers(int timeout)
{
	/* Nothing to do here because we don't do finalization */
	return 1;
}

int ILGCDisableFinalizers(int timeout)
{
	/* Nothing to do here because we don't do finalization */
	return 1;
}

void ILGCEnableFinalizers(void)
{
	/* Nothing to do here because we don't do finalization */
}

long ILGCGetHeapSize(void)
{
	return (long)heapSize;
}

void ILGCRegisterWeak(void *ptr)
{
	/* Nothing to do here because we don't do finalization */
}

void ILGCUnregisterWeak(void *ptr)
{
	/* Nothing to do here because we don't do finalization */
}

void ILGCRegisterGeneralWeak(void *ptr, void *obj)
{
	/* Nothing to do here because we don't do finalization */
}

void *ILGCRunFunc(void *(* thread_func)(void *), void *arg)
{
	return thread_func(arg);
}

#ifdef	__cplusplus
};
#endif

#endif /* !HAVE_LIBGC */
