/*
 * mempool.c - Memory pool functions.
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

Note: this is a very simple pool allocator.  It could be replaced with
something fancier to improve allocation policy on particular systems.

*/

#include "il_system.h"
#include "il_utils.h"
#include "il_align.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Default memory pool block size.  This is a bit less
 * than a system page so that the "malloc" headers and
 * the data will hopefully fit in a single page.  This
 * calculation is very system-specific by its very nature,
 * so adjust as necessary.
 */
#ifndef	IL_POOL_BLOCK_SIZE
#define	IL_POOL_BLOCK_SIZE	4080
#endif

void ILMemPoolInit(ILMemPool *pool, unsigned size, unsigned nitems)
{
	unsigned maxitems;

	/* Round the size to the nearest alignment boundary so that
	   we always return aligned structures from the allocator */
	pool->size = ((size + IL_BEST_ALIGNMENT - 1) & ~(IL_BEST_ALIGNMENT - 1));

	/* Determine the number of items in each block */
	maxitems = (IL_POOL_BLOCK_SIZE - IL_BEST_ALIGNMENT) / pool->size;
	if(nitems != 0 && nitems < maxitems)
	{
		pool->nitems = nitems;
	}
	else
	{
		pool->nitems = maxitems;
	}

	/* Clear the block and free lists */
	pool->index = pool->nitems;
	pool->blocks = 0;
	pool->freeItems = 0;
}

void ILMemPoolDestroy(ILMemPool *pool)
{
	void *block, *next;
	block = pool->blocks;
	while(block != 0)
	{
		next = *((void **)block);
		ILFree(block);
		block = next;
	}
	pool->index = pool->nitems;
	pool->blocks = 0;
	pool->freeItems = 0;
}

void ILMemPoolClear(ILMemPool *pool)
{
	if(pool->blocks)
	{
		/* Destroy everything except the first block, which
		   we keep around so that the next allocation is fast */
		void *block, *next;
		block = *((void **)(pool->blocks));
		while(block != 0)
		{
			next = *((void **)block);
			ILFree(block);
			block = next;
		}
		*((void **)(pool->blocks)) = 0;
		pool->index = 0;
	}
	pool->freeItems = 0;
}

void *ILMemPoolAllocItem(ILMemPool *pool)
{
	void *item;
	void *block;

	/* Return the first free item if one is available */
	if(pool->freeItems)
	{
		item = pool->freeItems;
		pool->freeItems = *((void **)item);
		return item;
	}

	/* Get the next item in the current block */
	if(pool->index < pool->nitems)
	{
		item = (void *)(((char *)(pool->blocks)) + IL_BEST_ALIGNMENT +
						pool->index * pool->size);
		++(pool->index);
		return item;
	}

	/* Allocate a new block from the system */
	block = (void *)(ILMalloc(IL_BEST_ALIGNMENT + pool->size * pool->nitems));
	if(!block)
	{
		return 0;
	}
	*((void **)block) = pool->blocks;
	pool->blocks = block;
	pool->index = 1;

	/* Return the first item in the new block */
	return (void *)(((char *)block) + IL_BEST_ALIGNMENT);
}

void *ILMemPoolCallocItem(ILMemPool *pool)
{
	void *item = ILMemPoolAllocItem(pool);
	if(item)
	{
		ILMemZero(item, pool->size);
	}
	return item;
}

void ILMemPoolFree(ILMemPool *pool, void *item)
{
	*((void **)item) = pool->freeItems;
	pool->freeItems = item;
}

#ifdef	__cplusplus
};
#endif
