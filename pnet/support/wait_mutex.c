/*
 * wait_mutex.c - Wait mutex objects for the threading sub-system.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * Contributions:  Thong Nguyen (tum@veridicus.com)
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
the correct CLI wait mutex semantics based on the primitives in "*_defs.h".
You normally won't need to modify or replace this file when porting.

Wait mutexes differ from ordinary mutexes in that they can be used
from C# code and can be held by a suspended thread.

*/

#include "thr_defs.h"
#include "wait_mutex.h"
#include "interlocked.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * List of mutex names that are in use.
 */
typedef struct _tagILMutexName ILMutexName;
struct _tagILMutexName
{
	ILMutexName	   *next;
	ILWaitHandle   *handle;
	char			name[1];

};
static ILMutexName *nameList;
static _ILCriticalSection nameListLock;

/*
 * Close a regular mutex.
 */
static int MutexClose(ILWaitMutex *mutex)
{
	/* Lock down the mutex and determine if it is currently owned */
	_ILCriticalSectionEnter(&(mutex->parent.lock));

	if(mutex->owner != 0 || !_ILWakeupQueueIsEmpty(&(mutex->queue)))
	{
		_ILCriticalSectionLeave(&(mutex->parent.lock));
		return IL_WAITCLOSE_OWNED;
	}

	/* Clean up the mutex */
	_ILCriticalSectionLeave(&(mutex->parent.lock));
	_ILWakeupQueueDestroy(&(mutex->queue));
	_ILMutexDestroy(&(mutex->parent.lock));
	
	return IL_WAITCLOSE_FREE;
}

/*
 * Close a named mutex.
 */
static int MutexCloseNamed(ILWaitMutexNamed *mutex)
{
	ILMutexName *temp, *prev;

	/* We need the name list lock */
	_ILCriticalSectionEnter(&nameListLock);

	/* Lock down the mutex and determine if it is currently owned */
	_ILCriticalSectionEnter(&(mutex->parent.parent.lock));
	if(mutex->parent.owner != 0 ||
	   !_ILWakeupQueueIsEmpty(&(mutex->parent.queue)))
	{
		_ILCriticalSectionLeave(&(mutex->parent.parent.lock));
		_ILCriticalSectionLeave(&nameListLock);
		return IL_WAITCLOSE_OWNED;
	}

	/* Decrease the number of users and bail out if still non-zero */
	if(--(mutex->numUsers) != 0)
	{
		_ILCriticalSectionLeave(&(mutex->parent.parent.lock));
		_ILCriticalSectionLeave(&nameListLock);
		return IL_WAITCLOSE_DONT_FREE;
	}

	/* Clean up the mutex */
	_ILCriticalSectionLeave(&(mutex->parent.parent.lock));
	_ILWakeupQueueDestroy(&(mutex->parent.queue));
	_ILMutexDestroy(&(mutex->parent.parent.lock));

	/* Remove the mutex from the name list */
	prev = 0;
	temp = nameList;
	while(temp != 0 && temp->handle != (ILWaitHandle *)mutex)
	{
		prev = temp;
		temp = temp->next;
	}
	if(temp != 0)
	{
		if(prev)
		{
			prev->next = temp->next;
		}
		else
		{
			nameList = temp->next;
		}
		ILFree(temp);
	}
	_ILCriticalSectionLeave(&nameListLock);

	/* The wait handle object is now completely free */
	return IL_WAITCLOSE_FREE;
}

/*
 * Set of prime numbers used for hashtable allocation.
 */
static int __PrimeNumbers[] =
{
	13, 29, 61, 127, 257, 521
};

static int __numberOfPrimes = sizeof(__PrimeNumbers) / sizeof(int);

static void AddMutexToWakeup(ILWaitMutex *mutex, _ILWakeup *wakeup)
{
	int i, x;
	ILWaitMutex **ownedMutexes;

	if (wakeup->ownedMutexes == 0)
	{
		/* Allocate owned mutexes hashtable */
		ownedMutexes = wakeup->ownedMutexes = ILCalloc(__PrimeNumbers[0], sizeof(ILWaitMutex *));
		wakeup->ownedMutexesCapacity = __PrimeNumbers[0];
	}
	else
	{
		ownedMutexes = wakeup->ownedMutexes;
	}
	
	/* Check if the hashtable is large enough */
	if (wakeup->ownedMutexesCount >= wakeup->ownedMutexesCapacity)
	{
		int newCapacity;
		ILWaitMutex **newArray;

		/* Grow the new hashtable */
		
	#if SIZEOF_INT <= 4
		if (wakeup->ownedMutexesCapacity >= (IL_MAX_INT32) / 2)
	#else
		if (wakeup->ownedMutexesCapacity >= (IL_MAX_NATIVE_INT) / 2)
	#endif
		{
			return;
		}

		/* Find a nice efficient prime sized hashtable.  If it gets too large
		   then forget about using a prime */

		newCapacity = wakeup->ownedMutexesCapacity * 2;

		for (i = __numberOfPrimes - 1; i >= 0; i--)
		{
			if (__PrimeNumbers[i] < newCapacity)
			{
				if (i == __numberOfPrimes - 1)
				{
					break;					
				}
				else
				{
					newCapacity = __PrimeNumbers[i + 1];
				}
			}
		}

		newArray = ILCalloc(newCapacity, sizeof(ILWaitMutex *));

		if (!newArray)
		{
			return;
		}

		/* Rehash all the existing mutexes into the new table */

		for (i = 0; i < wakeup->ownedMutexesCapacity; i++)
		{
			if (ownedMutexes[i] == 0)
			{
				/* This bucket doesn't have an entry */

				continue;
			}
		
			/* Find the new available bucket in the new hashtable */

			#if SIZEOF_VOID_P <= 4
				x = (int)(((ILNativeInt)ownedMutexes[i]) >> 2) % newCapacity;
			#else
				x = (int)(((ILNativeInt)ownedMutexes[i]) >> 3) % newCapacity;
			#endif
			
			for (;;)
			{
				if (newArray[x] == 0)
				{
					newArray[x] = mutex;

					break;
				}

				x++;
				x %= newCapacity;
			}
			
		}

		ownedMutexes = wakeup->ownedMutexes = newArray;
		wakeup->ownedMutexesCapacity = newCapacity;
	}

	/* Get the initial bucket to try putting the mutex in */
	
	#if SIZEOF_VOID_P <= 4
		i = (int)(((ILNativeInt)mutex) >> 2) % wakeup->ownedMutexesCapacity;
	#else
		i = (int)(((ILNativeInt)mutex) >> 3) % wakeup->ownedMutexesCapacity;
	#endif

	/* Scan the hashtable and find an empty bucket for the mutex */

	for (;;)
	{
		if (ownedMutexes[i] == 0)
		{
			ownedMutexes[i] = mutex;
			wakeup->ownedMutexesCount++;

			break;
		}

		i++;
		i %= wakeup->ownedMutexesCapacity;
	}
}

static void RemoveMutexFromWakeup(ILWaitMutex *mutex, _ILWakeup *wakeup)
{
	int i, j;
	ILWaitMutex **ownedMutexes;

	ownedMutexes = wakeup->ownedMutexes;

	if (ownedMutexes == 0)
	{
		return;
	}

	/* Get the initial (and correct right) bucket */

	#if SIZEOF_VOID_P <= 4
		j = i = (int)(((ILNativeInt)mutex) >> 2) % wakeup->ownedMutexesCapacity;
	#else
		j = i = (int)(((ILNativeInt)mutex) >> 3) % wakeup->ownedMutexesCapacity;
	#endif

	/* Scan the hashtable and clear the entry for the mutex (if found) */

	for (;;)
	{
		if (ownedMutexes[i] == mutex)
		{
			ownedMutexes[i] = 0;
			wakeup->ownedMutexesCount--;

			break;
		}
		
		i++;
		i %= wakeup->ownedMutexesCapacity;
		
		if (i == j)
		{
			break;
		}
	}
}

/*
 * Register a wakeup object with a wait mutex.
 */
static int MutexRegister(ILWaitMutex *mutex, _ILWakeup *wakeup)
{
	int result;

	/* Lock down the mutex */
	_ILCriticalSectionEnter(&(mutex->parent.lock));

	/* Determine what to do based on the mutex's current state */
	if(mutex->owner == 0)
	{
		/* No one owns the mutex, so grab it for ourselves */
		mutex->owner = wakeup;
		mutex->count = 1;

		result = IL_WAITREG_ACQUIRED;

		if (_ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_MUTEX
			|| _ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_NAMED_MUTEX)
		{
			/* Newly aquired mutex so add it to the wakeup's owned mutex list */

			AddMutexToWakeup(mutex, wakeup);
		}
	}
	else if(mutex->owner == wakeup)
	{
		/* The mutex is already owned by this thread, so re-acquire it */
		++(mutex->count);		
		result = IL_WAITREG_ACQUIRED;
	}
	else
	{
		/* Someone else owns the mutex, so add ourselves to the wait queue */
		if(_ILWakeupQueueAdd(&(mutex->queue), wakeup, mutex))
		{
			result = IL_WAITREG_OK;
		}
		else
		{
			result = IL_WAITREG_FAILED;
		}
	}

	/* Unlock the mutex and return */
	_ILCriticalSectionLeave(&(mutex->parent.lock));
	return result;
}

/*
 * Unregister a thread from a wait mutex's queue.
 */
static void MutexUnregister(ILWaitMutex *mutex, _ILWakeup *wakeup, int release)
{
	/* Lock down the mutex */
	_ILCriticalSectionEnter(&(mutex->parent.lock));

	/* Remove ourselves from the wait queue if we are currently on it */
	_ILWakeupQueueRemove(&(mutex->queue), wakeup);

	/* Release the mutex if requested */
	if(release && mutex->owner == wakeup)
	{
		if(--(mutex->count) == 0)
		{
			if (_ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_MUTEX
				|| _ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_NAMED_MUTEX)
			{
				RemoveMutexFromWakeup(mutex, wakeup);
			}

			mutex->owner = _ILWakeupQueueWake(&(mutex->queue));

			if(mutex->owner != 0)
			{
				mutex->count = 1;
			}
		}
	}
	else if (mutex->owner == wakeup && mutex->count == 1)
	{
		if (_ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_MUTEX
			|| _ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_NAMED_MUTEX)
		{
			/* Newly aquired mutex so add it to the wakeup's owned mutex list */
			AddMutexToWakeup(mutex, wakeup);		
		}
	}

	/* Unlock the mutex and return */
	_ILCriticalSectionLeave(&(mutex->parent.lock));
}

static int MutexSignal(ILWaitHandle *waitHandle)
{
	return ILWaitMutexRelease(waitHandle) > 0;
}

/*
 * The WaitHandleVtable for mutexes
 */
static const _ILWaitHandleVtable _ILWaitMutexVtable =
{
	IL_WAIT_MUTEX,
	(ILWaitCloseFunc)MutexClose,
	(ILWaitRegisterFunc)MutexRegister,
	(ILWaitUnregisterFunc)MutexUnregister,
	(ILWaitSignalFunc)MutexSignal
};

/*
 * The WaitHandleVtable for named mutexes
 */
static const _ILWaitHandleVtable _ILWaitMutexNamedVtable =
{
	IL_WAIT_NAMED_MUTEX,
	(ILWaitCloseFunc)MutexCloseNamed,
	(ILWaitRegisterFunc)MutexRegister,
	(ILWaitUnregisterFunc)MutexUnregister,
	(ILWaitSignalFunc)MutexSignal
};

ILWaitHandle *ILWaitMutexCreate(int initiallyOwned)
{
	ILWaitMutex *mutex;

	/* Allocate memory for the mutex */
	if((mutex = (ILWaitMutex *)ILMalloc(sizeof(ILWaitMutex))) == 0)
	{
		return 0;
	}

	/* Initialize the mutex */
	mutex->parent.vtable = &_ILWaitMutexVtable;
	_ILMutexCreate(&(mutex->parent.lock));

	if(initiallyOwned)
	{
		mutex->owner = &((_ILThreadGetSelf())->wakeup);
		mutex->count = 1;		
		AddMutexToWakeup(mutex, mutex->owner);
	}
	else
	{
		mutex->owner = 0;
		mutex->count = 0;
	}
	_ILWakeupQueueCreate(&(mutex->queue));

	/* Ready to go */
	return &(mutex->parent);
}

/*
 * Initialize the mutex name list.
 */
static void MutexNameListInit(void)
{
	_ILCriticalSectionCreate(&nameListLock);
	nameList = 0;
}

ILWaitHandle *ILWaitMutexNamedCreate(const char *name, int initiallyOwned,
							 int *gotOwnership)
{
	ILWaitHandle *handle;
	ILWaitMutexNamed *mutex;
	ILMutexName *temp;
	int owned;

	/* If we don't have a name, then create a regular mutex */
	if(!name)
	{
		handle = ILWaitMutexCreate(initiallyOwned);
		if(gotOwnership)
		{
			*gotOwnership = initiallyOwned;
		}
		return handle;
	}

	/* Search for a mutex with the same name */
	_ILCallOnce(MutexNameListInit);
	_ILCriticalSectionEnter(&nameListLock);
	temp = nameList;
	while(temp != 0)
	{
		if(!strcmp(temp->name, name))
		{
			/* Increase the usage count on the mutex */
			mutex = (ILWaitMutexNamed *)(temp->handle);
			_ILCriticalSectionEnter(&(mutex->parent.parent.lock));
			++(mutex->numUsers);
			_ILCriticalSectionLeave(&(mutex->parent.parent.lock));

			/* Unlock the name list */
			_ILCriticalSectionLeave(&nameListLock);

			/* Attempt to acquire ownership of the mutex if requested */
			if(initiallyOwned)
			{
				owned = (ILWaitOne(&(mutex->parent.parent), 0) == 0);
				AddMutexToWakeup(&(mutex->parent), &(_ILThreadGetSelf()->wakeup));
			}
			else
			{
				owned = 0;
			}
			if(gotOwnership)
			{
				*gotOwnership = owned;
			}

			/* Return the mutex to the caller */
			return &(mutex->parent.parent);
		}
		temp = temp->next;
	}

	/* Allocate memory for the mutex */
	if((mutex = (ILWaitMutexNamed *)ILMalloc(sizeof(ILWaitMutexNamed))) == 0)
	{
		_ILCriticalSectionLeave(&nameListLock);
		return 0;
	}

	/* Allocate space for a name entry */
	temp = (ILMutexName *)ILMalloc(sizeof(ILMutexName) + strlen(name));
	if(!temp)
	{
		_ILCriticalSectionLeave(&nameListLock);
		ILFree(mutex);
		return 0;
	}

	/* Initialize the mutex */
	mutex->parent.parent.vtable = &_ILWaitMutexNamedVtable;
	_ILMutexCreate(&(mutex->parent.parent.lock));
	if(initiallyOwned)
	{
		mutex->parent.owner = &((_ILThreadGetSelf())->wakeup);
		mutex->parent.count = 1;
		AddMutexToWakeup(&(mutex->parent), mutex->parent.owner);
	}
	else
	{
		mutex->parent.owner = 0;
		mutex->parent.count = 0;
	}
	_ILWakeupQueueCreate(&(mutex->parent.queue));
	mutex->numUsers = 1;
	if(gotOwnership)
	{
		*gotOwnership = initiallyOwned;
	}

	/* Add the mutex to the name list */
	temp->next = nameList;
	temp->handle = &(mutex->parent.parent);
	strcpy(temp->name, name);
	nameList = temp;
	_ILCriticalSectionLeave(&nameListLock);

	/* Ready to go */
	return &(mutex->parent.parent);
}

int ILWaitMutexRelease(ILWaitHandle *handle)
{
	_ILWakeup *wakeup;
	ILWaitMutex *mutex = (ILWaitMutex *)handle;
	int result;

	/* Lock down the mutex */
	_ILCriticalSectionEnter(&(mutex->parent.lock));

	wakeup = &_ILThreadGetSelf()->wakeup;

	/* Determine what to do based on the mutex's state */
	if((_ILWaitHandle_kind(&(mutex->parent)) & IL_WAIT_MUTEX) == 0)
	{
		/* This isn't actually a mutex */
		result = IL_WAITMUTEX_RELEASE_FAIL;
	}
	else if(mutex->owner != wakeup)
	{
		/* This thread doesn't currently own the mutex */
		result = IL_WAITMUTEX_RELEASE_FAIL;
	}
	else if(--(mutex->count) == 0)
	{
		/* The count has returned to zero, so find something
		   else to give the ownership of the mutex to */

		if (_ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_MUTEX
			|| _ILWaitHandle_kind(&(mutex->parent)) == IL_WAIT_NAMED_MUTEX)
		{
			RemoveMutexFromWakeup(mutex, wakeup);
		}

		mutex->owner = _ILWakeupQueueWake(&(mutex->queue));
		if(mutex->owner != 0)
		{
			mutex->count = 1;
		}
		result = IL_WAITMUTEX_RELEASE_SUCCESS;
	}
	else
	{
		/* The current thread still owns the mutex */
		result = IL_WAITMUTEX_RELEASE_STILL_OWNS;
	}

	/* Unlock the mutex and return */
	_ILCriticalSectionLeave(&(mutex->parent.lock));
	return result;
}

/*
 * Close a monitor.
 */
static int MonitorClose(ILWaitMonitor *monitor)
{
	/* Lock down the monitor and determine if it is currently owned */
	_ILCriticalSectionEnter(&(monitor->parent.parent.lock));
	
	/* We we allow monitors to be closed even if they have
	   an owner.  It is valid for a program to lock an
	   object and never release it before it gets GC-ed.  */
	if(monitor->waiters > 0 || !_ILWakeupQueueIsEmpty(&(monitor->parent.queue)))
	{
		_ILCriticalSectionLeave(&(monitor->parent.parent.lock));
		return IL_WAITCLOSE_OWNED;
	}

	/* Clean up the monitor */
	_ILCriticalSectionLeave(&(monitor->parent.parent.lock));
	_ILWakeupQueueDestroy(&(monitor->parent.queue));
	_ILWakeupQueueDestroy(&(monitor->signalQueue));
	_ILMutexDestroy(&(monitor->parent.parent.lock));
	return IL_WAITCLOSE_FREE;
}

/*
 * The WaitHandleVtable for wait monitors
 */
static const _ILWaitHandleVtable _ILWaitMonitorVtable =
{
	IL_WAIT_MONITOR,
	(ILWaitCloseFunc)MonitorClose,
	(ILWaitRegisterFunc)MutexRegister,
	(ILWaitUnregisterFunc)MutexUnregister,
	(ILWaitSignalFunc)MutexSignal
};

ILWaitHandle *ILWaitMonitorCreate(void)
{
	ILWaitMonitor *monitor;

	/* Allocate memory for the monitor */
	if((monitor = (ILWaitMonitor *)ILMalloc(sizeof(ILWaitMonitor))) == 0)
	{
		return 0;
	}

	/* Initialize the monitor fields */
	monitor->parent.parent.vtable = &_ILWaitMonitorVtable;
	_ILMutexCreate(&(monitor->parent.parent.lock));
	monitor->parent.owner = 0;
	monitor->parent.count = 0;
	monitor->waiters = 0;
	_ILWakeupQueueCreate(&(monitor->parent.queue));
	_ILWakeupQueueCreate(&(monitor->signalQueue));

	/* Ready to go */
	return &(monitor->parent.parent);
}

/*
 * The specification for what happens when a wait is aborted is ambiguous at best.
 * MS.NET appears to allow interrupted and aborted threads to exit wait without
 * reaquire the monitor lock.  To facilitate this, MS.NET ignores calls to
 * Monitor.Exit by any thread that has or ever was aborted.
 *
 * See: http://dotgnu.org/pipermail/developers/2004-May/012214.html
 *      http://dotgnu.org/pipermail/developers/2004-May/012226.html
 *
 * In PNET's implementation a waiting thread *must* reaquire the lock on the
 * monitor it is waiting on before it can continue.  Like, MS.NET, the
 * interrupted/aborted thread does not need to wait for a pulse.
 * 
 * Allowing a waiting thread to exit Monitor.Wait without reaquiring the lock
 * could potentially lead to deadlocks and data corruption.
 */
int ILWaitMonitorWait(ILWaitHandle *handle, ILUInt32 timeout)
{
	ILThread *thread = _ILThreadGetSelf();
	ILWaitMonitor *monitor = (ILWaitMonitor *)handle;
	_ILWakeup *wakeup = &(thread->wakeup);
	int result, result2;
	unsigned long saveCount;

	result = _ILEnterWait(thread);
	
	if (result != 0)
	{
		return result;
	}
		
	/* Lock down the monitor */
	_ILCriticalSectionEnter(&(monitor->parent.parent.lock));

	++monitor->waiters;

	/* Determine what to do based on the monitor's state */
	if(_ILWaitHandle_kind(&(monitor->parent.parent)) != IL_WAIT_MONITOR)
	{
		/* This isn't actually a monitor */
		result = 0;
	}
	else if(monitor->parent.owner != wakeup)
	{
		/* This thread doesn't currently own the monitor */
		result = 0;
	}
	else
	{
		/* Save the count and reset the monitor to unowned */
		saveCount = monitor->parent.count;
		monitor->parent.owner = 0;
		monitor->parent.count = 0;

		/*
		 * Must set the limit before we add ourselves to the wakeup queue 't
		 * otherwise we might miss any signal between now and the call to 
		 * _ILWakeupWait.
		 */
		if (_ILWakeupSetLimit(wakeup, 1))
		{			
			/* Add ourselves to the signal wakeup queue */
			_ILWakeupQueueAdd(&(monitor->signalQueue), wakeup, monitor);

			/* Wakeup any threads waiting to enter */
			monitor->parent.owner = _ILWakeupQueueWake(&(monitor->parent.queue));

			if(monitor->parent.owner != 0)
			{			
				monitor->parent.count = 1;
			}

			/* Unlock the monitor */
			_ILCriticalSectionLeave(&(monitor->parent.parent.lock));

			/* Wait until we are signalled */			
			result = _ILWakeupWait(wakeup, timeout, 0);
						
			if(result < 0)
			{
				result = IL_WAIT_INTERRUPTED;
			}
			else if(result == 0)
			{
				result = IL_WAIT_TIMEOUT;
			}
		}
		else
		{
			/* Unlock the monitor */
			_ILCriticalSectionLeave(&(monitor->parent.parent.lock));

			result = IL_WAIT_INTERRUPTED;
		}

		/* Wait to reaquire the monitor */
		result2 = _ILWaitOneBackupInterruptsAndAborts(handle, -1);

		if (result2 < 0)
		{
			result = result2;
		}

		/* Lock down the monitor and set the count back to the right value */
		_ILCriticalSectionEnter(&(monitor->parent.parent.lock));

		if(monitor->parent.owner == 0)
		{
			monitor->parent.owner = wakeup;
			monitor->parent.count = saveCount;
		}
		else if(monitor->parent.owner == wakeup)
		{
			monitor->parent.count = saveCount;
		}
	}

	--monitor->waiters;
	
	/* Unlock the monitor and return */
	_ILCriticalSectionLeave(&(monitor->parent.parent.lock));

	return _ILLeaveWait(thread, result);
}

static IL_INLINE int PrivateWaitMonitorPulse(ILWaitHandle *handle, int all)
{
	ILWaitMonitor *monitor = (ILWaitMonitor *)handle;
	_ILWakeup *wakeup = &((_ILThreadGetSelf())->wakeup);
	int result;

	/* Lock down the monitor */
	_ILCriticalSectionEnter(&(monitor->parent.parent.lock));

	/* Determine what to do based on the monitor's state */
	if(_ILWaitHandle_kind(&(monitor->parent.parent)) != IL_WAIT_MONITOR)
	{
		/* This isn't actually a monitor */
		result = 0;
	}
	else if(monitor->parent.owner != wakeup)
	{
		/* This thread doesn't currently own the monitor */
		result = 0;
	}
	else
	{
		/* Wake up something on the signal queue */
		/* GCC should optimise out this if statement */
		
		if (all)
		{
			_ILWakeupQueueWakeAll(&(monitor->signalQueue));
		}
		else
		{
			_ILWakeupQueueWake(&(monitor->signalQueue));
		}

		result = 1;
	}
	/* Unlock the monitor and return */
	_ILCriticalSectionLeave(&(monitor->parent.parent.lock));
	return result;
}

int ILWaitMonitorPulse(ILWaitHandle *handle)
{
	return PrivateWaitMonitorPulse(handle, 0);
}

int ILWaitMonitorPulseAll(ILWaitHandle *handle)
{
	return PrivateWaitMonitorPulse(handle, 1);
}

#ifdef	__cplusplus
};
#endif
