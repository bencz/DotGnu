/*
 * semaphore.c - Semaphore management routines.
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

/*

Note: the code in this module is generic to all platforms.  It implements
the correct CLI locking semantics based on the primitives in "*_defs.h".
You normally won't need to modify or replace this file when porting.

*/

#include "thr_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILSemaphore *ILSemaphoreCreate(void)
{
	_ILSemaphore *sem;
	sem = (_ILSemaphore *)ILMalloc(sizeof(_ILSemaphore));
	if(sem)
	{
		_ILSemaphoreCreate(sem);
	}
	return (ILSemaphore *)sem;
}

void ILSemaphoreDestroy(ILSemaphore *sem)
{
	_ILSemaphoreDestroy((_ILSemaphore *)sem);
}

void ILSemaphoreWait(ILSemaphore *sem)
{
	_ILSemaphoreWait((_ILSemaphore *)sem);
}

void ILSemaphorePost(ILSemaphore *sem)
{
	_ILSemaphorePost((_ILSemaphore *)sem);
}

void ILSemaphorePostMultiple(ILSemaphore *sem, ILUInt32 count)
{
	_ILSemaphorePostMultiple((_ILSemaphore *)sem, count);
}

#ifdef	__cplusplus
};
#endif
