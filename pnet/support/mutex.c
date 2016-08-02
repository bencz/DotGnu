/*
 * mutex.c - Mutex and read/write lock management routines.
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

ILMutex *ILMutexCreate(void)
{
	_ILMutex *mutex;
	mutex = (_ILMutex *)ILMalloc(sizeof(_ILMutex));
	if(mutex)
	{
		_ILMutexCreate(mutex);
	}
	return (ILMutex *)mutex;
}

void ILMutexDestroy(ILMutex *mutex)
{
	_ILMutexDestroy((_ILMutex *)mutex);
}

void ILMutexLock(ILMutex *mutex)
{
	_ILMutexLock((_ILMutex *)mutex);
}

void ILMutexUnlock(ILMutex *mutex)
{
	_ILMutexUnlock((_ILMutex *)mutex);
}

ILRWLock *ILRWLockCreate(void)
{
	_ILRWLock *rwlock;
	rwlock = (_ILRWLock *)ILMalloc(sizeof(_ILRWLock));
	if(rwlock)
	{
		_ILRWLockCreate(rwlock);
	}
	return (ILRWLock *)rwlock;
}

void ILRWLockDestroy(ILRWLock *rwlock)
{
	_ILRWLockDestroy((_ILRWLock *)rwlock);
}

void ILRWLockReadLock(ILRWLock *rwlock)
{
	_ILRWLockReadLock((_ILRWLock *)rwlock);
}

void ILRWLockWriteLock(ILRWLock *rwlock)
{
	_ILRWLockWriteLock((_ILRWLock *)rwlock);
}

void ILRWLockUnlock(ILRWLock *rwlock)
{
	_ILRWLockUnlock((_ILRWLock *)rwlock);
}

#ifdef	__cplusplus
};
#endif
