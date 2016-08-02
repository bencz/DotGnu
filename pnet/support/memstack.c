/*
 * memstack.c - Memory stack implementation.
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

#include "il_utils.h"
#include "il_system.h"
#include "il_align.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Tuning parameters.
 */
#ifndef	IL_MEMSTACK_BLOCK_SIZE
#define	IL_MEMSTACK_BLOCK_SIZE			16384
#endif
#ifndef	IL_MEMSTACK_MAX_OBJECT_SIZE
#define	IL_MEMSTACK_MAX_OBJECT_SIZE		1024
#endif

void ILMemStackInit(ILMemStack *stack, unsigned long maxSize)
{
	stack->size = IL_MEMSTACK_BLOCK_SIZE & ~(ILPageAllocSize() - 1);
	stack->posn = stack->size;
	stack->blocks = 0;
	stack->currSize = 0;
	stack->maxSize = maxSize;
	stack->extras = 0;
}

void ILMemStackDestroy(ILMemStack *stack)
{
	void *block, *next;
	block = stack->blocks;
	while(block != 0)
	{
		next = *((void **)block);
		ILPageFree(block, stack->size);
		block = next;
	}
	block = stack->extras;
	while(block != 0)
	{
		next = *((void **)block);
		ILFree(block);
		block = next;
	}
	stack->posn = stack->size;
	stack->blocks = 0;
	stack->currSize = 0;
	stack->extras = 0;
}

void *ILMemStackAllocItem(ILMemStack *stack, unsigned size)
{
	void *ptr;

	/* Round up the size to the nearest alignment boundary */
	size = (size + IL_BEST_ALIGNMENT - 1) & ~(IL_BEST_ALIGNMENT - 1);

	/* If the size is greater than the maximum object size,
	   then put the data into a separate malloc'ed block */
	if(size > IL_MEMSTACK_MAX_OBJECT_SIZE)
	{
		if(stack->maxSize && (stack->currSize + size + 64) > stack->maxSize)
		{
			/* We've reached the built-in limit for the stack */
			return 0;
		}
		ptr = ILCalloc(size + IL_BEST_ALIGNMENT, 1);
		if(!ptr)
		{
			/* The system itself is out of memory */
			return 0;
		}
		stack->currSize += size + 64;
		*((void **)ptr) = stack->extras;
		stack->extras = ptr;
		return (void *)(((unsigned char *)ptr) + IL_BEST_ALIGNMENT);
	}

	/* Do we need to allocate a new block? */
	if((stack->posn + size) > stack->size)
	{
		if(stack->maxSize && stack->currSize >= stack->maxSize)
		{
			/* We've reached the built-in limit for the stack */
			return 0;
		}
		ptr = ILPageAlloc(stack->size);
		if(!ptr)
		{
			/* The system itself is out of memory */
			return 0;
		}
		*((void **)ptr) = stack->blocks;
		stack->blocks = ptr;
		stack->posn = IL_BEST_ALIGNMENT;
		stack->currSize += stack->size;
	}

	/* Allocate the item */
	ptr = (void *)(((unsigned char *)(stack->blocks)) + stack->posn);
	stack->posn += size;
	return ptr;
}

#ifdef	__cplusplus
};
#endif
