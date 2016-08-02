/*
 * wakeup.c - Implementation of "thread wakeup" objects and queues.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * Contributions fom Thong Nguyen (tum@veridicus.com)
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

Wakeup objects are similar to condition variables, except that they
also support "signal at N" and "interrupt" semantics.

*/

#include "thr_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

void _ILWakeupCreate(_ILWakeup *wakeup)
{
	_ILCondMutexCreate(&(wakeup->lock));
	_ILCondVarCreate(&(wakeup->condition));
	wakeup->count = 0;
	wakeup->limit = 0;
	wakeup->interrupted = 0;
	wakeup->object = 0;
	wakeup->ownedMutexesCount = 0;
	wakeup->ownedMutexesCapacity = 0;
	wakeup->ownedMutexes = 0;
}

void _ILWakeupDestroy(_ILWakeup *wakeup)
{
	int i;

	if (wakeup->ownedMutexes != 0)
	{
		/* Fully release all owned mutexes */
		for (i = 0; i < wakeup->ownedMutexesCapacity; i++)
		{
			if (wakeup->ownedMutexes[i] != 0)
			{
				while (ILWaitMutexRelease((ILWaitHandle *)wakeup->ownedMutexes[i]) == IL_WAITMUTEX_RELEASE_STILL_OWNS);
			}
		}

		ILFree(wakeup->ownedMutexes);
	}
	_ILCondVarDestroy(&(wakeup->condition));
	_ILCondMutexDestroy(&(wakeup->lock));
}

/*
 * A thread must set its wakeup limit before it adds itself to a
 * wakeup queue and before it waits on its wakeup.  Failing to do
 * so will mean missing signals that are sent after being added to
 * a wait queue but before calling wait.
 */
int _ILWakeupSetLimit(_ILWakeup *wakeup, ILUInt32 limit)
{
	int result;

	/* Lock down the wakeup object.  We use an "unsafe" mutex
	   because we will be in the "wait/sleep/join" state */
	_ILCondMutexLockUnsafe(&(wakeup->lock));

	/* Is there a pending interrupt? */
	if(!(wakeup->interrupted))
	{
		wakeup->count = 0;
		wakeup->limit = limit;
		wakeup->object = 0;
		result = 1;
	}
	else
	{
		wakeup->interrupted = 0;
		result = 0;
	}

	/* Unlock the wakeup object and return */
	_ILCondMutexUnlockUnsafe(&(wakeup->lock));
	return result;
}

void _ILWakeupAdjustLimit(_ILWakeup *wakeup, ILUInt32 limit)
{
	_ILCondMutexLockUnsafe(&(wakeup->lock));
	wakeup->limit = limit;
	_ILCondMutexUnlockUnsafe(&(wakeup->lock));
}

int _ILWakeupWait(_ILWakeup *wakeup, ILUInt32 ms, void **object)
{
	int result;

	/* Lock down the wakeup object.  We use an "unsafe" mutex lock
	   because we will be giving up the lock later on.  It is OK
	   to do this because the current thread will always be in the
	   "wait/sleep/join" state, which prevents thread suspension */
	_ILCondMutexLockUnsafe(&(wakeup->lock));

	/* Is there a pending interrupt? */
	if(!(wakeup->interrupted))
	{
		/* Give up the lock and wait for someone to signal us */

		result = 1;
		
		/*
		 *	Two subtle races to consider:
		 *
		 * 1: Signal arrives between adding a wakeup to the queue and waiting on the wakeup.
		 *     The signal will get missed and the thread will halt indefinitely.
		 *     The while loop predicate allows the thread to exit successfully if we missed 
		 *     the signal.
		 *
		 * 2: We get here, notice the wakeup->count < wakeup->limit, don't bother waiting
		 *     on the signal.  No context switch is made, we reset and register our wakeup again
		 *     and get the signal that was intended for the last time we registered the wakeup.
		 *     The thread will wakeup when it shouldn't.
		 *     The "continue" in the while loop makes us keep waiting
		 */

		while (wakeup->count < wakeup->limit)
		{
			if (_ILCondVarTimedWait(&(wakeup->condition), &(wakeup->lock), ms))
			{
				if (wakeup->interrupted)
				{
					wakeup->interrupted = 0;

					result = -1;

					break;
				}
				else
				{
					if (wakeup->count < wakeup->limit)
					{
						/*
						 * This signal was from a previous wakeup registration.
						 *
						 * No need to adjust "ms" because the time difference will be very
						 * small.
						 */
						   
						continue;
					}

					/* All signals that we were expecting have arrived */
					if(object)
					{
						/* Return the last object that was signalled */
						*object = wakeup->object;
					}
					result = 1;

					break;
				}
			}
			else
			{
				/* The wakeup object timed out.  We still check for interrupt
				because we may have been interrupted just after timeout,
				but before this thread was re-awoken */

				if (wakeup->interrupted)
				{
					result = -1;
					wakeup->interrupted = 0;
				}
				else
				{
					/* Timed out */
					result = 0;
				}
				
				break;
			}
		}

		wakeup->count = 0;
		wakeup->limit = 0;
	}
	else
	{
		/* The thread was already interrupted before we got this far */
		wakeup->interrupted = 0;
		result = -1;
	}

	/* Unlock the wakeup object and return */
	_ILCondMutexUnlockUnsafe(&(wakeup->lock));
	return result;

}

int _ILWakeupSignal(_ILWakeup *wakeup, void *object)
{
	int result;

	/* Lock down the wakeup object */
	_ILCondMutexLock(&(wakeup->lock));

	/* Determine what to do based on the wakeup object's state */
	if(wakeup->interrupted || wakeup->count >= wakeup->limit)
	{
		/* The wakeup was interrupted or we have already reached the limit */
		result = 0;
	}
	else
	{
		/* Increase the wakeup count */
		++(wakeup->count);

		/* Record the object to be returned from "_ILWakeupWait" */
		wakeup->object = object;

		/* Signal the waiting thread if we have reached the limit */
		if(wakeup->count >= wakeup->limit)
		{
			_ILCondVarSignal(&(wakeup->condition));

			/* The signal operation has suceeded */
			result = 1;
		}
		else
		{
			/* The signal operation succeeded and the target is still waiting */
			result = 2;
		}
	}

	/* Unlock the wakeup object and return */
	_ILCondMutexUnlock(&(wakeup->lock));
	return result;
}

void _ILWakeupInterrupt(_ILWakeup *wakeup)
{
	/* Lock down the wakeup object */
	_ILCondMutexLock(&(wakeup->lock));

	/* Nothing to do if the thread is already marked for interruption */
	if(!(wakeup->interrupted))
	{
		/* Mark the thread for interruption */
		wakeup->interrupted = 1;

		/* Signal anyone who is waiting that interruption occurred */
		if(wakeup->count < wakeup->limit || wakeup->limit == 0)
		{
			_ILCondVarSignal(&(wakeup->condition));
		}
	}

	/* Unlock the wakeup object */
	_ILCondMutexUnlock(&(wakeup->lock));
}

void _ILWakeupCancelInterrupt(_ILWakeup *wakeup)
{
	/* Lock down the wakeup object */
	_ILCondMutexLock(&(wakeup->lock));
	
	if(wakeup->interrupted)
	{
		/* Mark the thread for interruption */
		wakeup->interrupted = 0;
	}

	/* Unlock the wakeup object */
	_ILCondMutexUnlock(&(wakeup->lock));
}

void _ILWakeupQueueCreate(_ILWakeupQueue *queue)
{
	queue->first = 0;
	queue->last = 0;
	queue->spaceUsed = 0;
}

void _ILWakeupQueueDestroy(_ILWakeupQueue *queue)
{
	_ILWakeupItem *item, *next;
	item = queue->first;
	while(item != 0)
	{
		next = item->next;
		if(item != &(queue->space))
		{
			ILFree(item);
		}
		item = next;
	}
	queue->first = 0;
	queue->last = 0;
	queue->spaceUsed = 0;
}

int _ILWakeupQueueAdd(_ILWakeupQueue *queue, _ILWakeup *wakeup, void *object)
{
	_ILWakeupItem *item;

	/* Allocate space for the item */
	if(!(queue->spaceUsed))
	{
		/* Reuse the pre-allocated "space" item */
		item = &(queue->space);
		queue->spaceUsed = 1;
	}
	else if((item = (_ILWakeupItem *)ILMalloc(sizeof(_ILWakeupItem))) == 0)
	{
		/* Out of memory */
		return 0;
	}

	/* Populate the item and add it to the queue */
	item->next = 0;
	item->wakeup = wakeup;
	item->object = object;
	if(queue->first)
	{
		queue->last->next = item;
	}
	else
	{
		queue->first = item;
	}
	queue->last = item;
	return 1;
}

void _ILWakeupQueueRemove(_ILWakeupQueue *queue, _ILWakeup *wakeup)
{
	_ILWakeupItem *item, *prev;
	item = queue->first;
	prev = 0;
	while(item != 0)
	{
		if(item->wakeup == wakeup)
		{
			if(prev)
			{
				prev->next = item->next;
			}
			else
			{
				queue->first = item->next;
			}
			if(!(item->next))
			{
				queue->last = prev;
			}
			if(item != &(queue->space))
			{
				ILFree(item);
			}
			else
			{
				queue->spaceUsed = 0;
			}
			break;
		}
		prev = item;
		item = item->next;
	}
}

_ILWakeup *_ILWakeupQueueWake(_ILWakeupQueue *queue)
{
	_ILWakeupItem *item, *next;
	_ILWakeup *woken = 0;
	item = queue->first;
	while(item != 0 && !woken)
	{
		next = item->next;
		if(_ILWakeupSignal(item->wakeup, item->object) == 1)
		{
			woken = item->wakeup;
		}
		if(item != &(queue->space))
		{
			ILFree(item);
		}
		else
		{
			queue->spaceUsed = 0;
		}
		item = next;
	}
	queue->first = item;
	if(!item)
	{
		queue->last = 0;
	}
	return woken;
}

void _ILWakeupQueueWakeAll(_ILWakeupQueue *queue)
{
	_ILWakeupItem *item, *next;
	item = queue->first;
	while(item != 0)
	{
		next = item->next;
		_ILWakeupSignal(item->wakeup, item->object);
		if(item != &(queue->space))
		{
			ILFree(item);
		}
		item = next;
	}
	queue->first = 0;
	queue->last = 0;
	queue->spaceUsed = 0;
}

#ifdef	__cplusplus
};
#endif
