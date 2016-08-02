/*
 * wait_mutex.h - Inline definitions for wait mutexes.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * Authors: Thong Nguyen (tum@veridicus.com)
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

#ifndef	_WAIT_MUTEX_H
#define	_WAIT_MUTEX_H

#include "thr_defs.h"
#include "interlocked.h"

/*
 * Returns 1 if the given thread owns the mutexd.
 */
#define ILWaitMutexThreadOwns(t, handle) \
	(((ILWaitMutex *)handle)->owner == &(t->wakeup))

static IL_INLINE int ILWaitMutexFastEnter(ILThread *thread, ILWaitHandle *handle)
{
	int result = 0;
	ILWaitMutex *mutex = (ILWaitMutex *)handle;
	_ILThreadState threadState;
	
	/*
	 * MS.NET behaviour is weird.  If a thread is interrupted and
	 * it tries to enter an unlocked monitor it won't throw a
	 * ThreadInterrupedException *unless* another thread owns a
	 * lock to *any* monitor.  The first thread will throw an
	 * interrupted exception even though it doesn't have to wait
	 * because the monitor it wants is immeidately available.
	 *
	 * I can only conclude that this is because MS.NET uses a
	 * global lock for *all* monitors and threads have to enter
	 * a wait state if any monitor is locked (not just the
	 * monitor the thread wants).
	 *
	 * To be more predicatable, PNET could either always throw
	 * ThreadInterruptedExceptions if a thread is interrupted
	 * even if the monitor is unlocked or PNET could only
	 * throw ThreadInterruptedExceptions if the monitor being
	 * entered (not just *any* monitor like with MS.NET) is
	 * locked.  Currently, PNET uses the first option.
	 */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if ((threadState.split.pub & (IL_TS_INTERRUPTED_OR_ABORT_REQUESTED)) != 0)
	{
		_ILMutexLock(&thread->lock);

		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
		if ((threadState.split.pub & (IL_TS_ABORT_REQUESTED)) != 0)
		{
			result = IL_WAIT_ABORTED;
		}
		else if ((threadState.split.pub & (IL_TS_INTERRUPTED)) != 0)
		{
			result = IL_WAIT_INTERRUPTED;
		}
		
		_ILWakeupCancelInterrupt(&(thread->wakeup));
		threadState.split.pub &= ~(IL_TS_INTERRUPTED);
		ILInterlockedStoreU2(&(thread->state.split.pub), threadState.split.pub);

		_ILMutexUnlock(&thread->lock);
	}

	if (result == 0)
	{
		if (mutex->owner == 0)
		{
			mutex->owner = &thread->wakeup;
			mutex->count = 1;
		}
		else
		{
			++mutex->count;
		}
	}
		
	return result;
}

static IL_INLINE int ILWaitMutexFastRelease(ILThread *thread, ILWaitHandle *handle)
{
	ILWaitMutex *mutex = (ILWaitMutex *)handle;
	int result;

	/* Determine what to do based on the mutex's state */
	if((_ILWaitHandle_kind(&(mutex->parent)) & IL_WAIT_MUTEX) == 0)
	{
		/* This isn't actually a mutex */
		result = IL_WAITMUTEX_RELEASE_FAIL;
	}
	else if(mutex->owner != &(thread->wakeup))
	{
		/* This thread doesn't currently own the mutex */
		result = IL_WAITMUTEX_RELEASE_FAIL;
	}
	else if(--(mutex->count) == 0)
	{
		mutex->owner = 0;
		
		/* Don't notify anyone cause no threads should be waiting */
		result = IL_WAITMUTEX_RELEASE_SUCCESS;
	}
	else
	{
		/* The current thread still owns the mutex */
		result = IL_WAITMUTEX_RELEASE_STILL_OWNS;
	}

	return result;
}

#ifdef	__cplusplus
};
#endif

#endif	/* _WAIT_MUTEX_H */
