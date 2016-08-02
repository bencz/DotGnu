/*
 * monitor.c - Syncronization Monitor routines.
 *
 * Copyright (C) 2009, 2010  Southern Storm Software, Pty Ltd.
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
 * This is the generic code for monitor support for all pattforms.
 * The patform dependent code in in the *_defs files.
 */

#include <il_utils.h>
#include "thr_defs.h"
#include "interlocked.h"
#ifdef IL_THREAD_DEBUG
#include <stdio.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Number of monitors that should be kept in the thread local freelist.
 */
#define MIN_FREELIST_MONITORS	2

/*
 * Number of monitors in the thread local freelist that triggers a move of
 * monitors from the thread local freelist to the global freelist.
 */
#define MAX_FREELIST_MONITORS	5

struct _tagILMonitor
{
	ILMonitor		   *nextUsed;	/* The next monitor in the used list */
	ILMonitor		   *nextFree;	/* The next monitor in the free list */
	ILThread * volatile	owner;		/* The current owner of the monotor */
	ILInt32				enterCount;	/* The number of enters without corresponding leave by the owner */
	ILUInt32			users;		/* The number of threads using this monitor */
	_ILMonitor			monitor;	/* The platform dependent monitor */
};

/*
 * Pool used by the monitor.
 */
typedef struct _tagILMonitorPool _ILMonitorPool;
struct _tagILMonitorPool
{
	_ILCriticalSection	lock;		/* critical section to synchronize the access to the pool */
	ILMonitor		   *freeList;	/* List of unused monitors */
	ILMonitor		   *usedList;	/* List of monitors in use */
	ILMemPool			pool;		/* Pool to allocate the monitors from */
#ifdef IL_THREAD_DEBUG
	ILUInt32		numReclaimed;	/* Number of reclaimed monitors */
	ILUInt32		numAbandoned;	/* Number of abandoned monitors */
#endif
};

/*
 * The pool to hold all monitors used.
 */
static _ILMonitorPool _MonitorPool;

static int ILMonitorInit(ILMonitor *monitor, ILThread *thread)
{
	int result;

	monitor->nextFree = 0;
	monitor->owner = thread;
	monitor->enterCount = 1;
	monitor->users = 1;
	_ILMonitorCreate(&(monitor->monitor), result);
	return result;
}

static void ILMonitorDestroy(ILMonitor *monitor)
{
	int result;

	monitor->nextUsed = 0;
	monitor->nextFree = 0;
	monitor->owner = 0;
	monitor->enterCount = 0;
	monitor->users = 0;
	_ILMonitorDestroy(&(monitor->monitor), result);
}

static void DestroyMonitorUsedList(ILMonitor *monitor)
{
	while(monitor)
	{
		ILMonitor *next;

		next = monitor->nextUsed;
		ILMonitorDestroy(monitor);
		monitor = next;
	}
}

#ifdef IL_THREAD_DEBUG
static int FreeListCount(ILMonitor *monitor)
{
	int count;

	count = 0;
	while(monitor)
	{
		++count;
		monitor = monitor->nextFree;
	}
	return count;
}

static int UsedListCount(ILMonitor *monitor)
{
	int count;

	count = 0;
	while(monitor)
	{
		++count;
		monitor = monitor->nextUsed;
	}
	return count;
}

void ILMonitorPrintStats()
{
	fprintf(stderr, "Number of monitors in the used list: %i\n",
		    UsedListCount(_MonitorPool.usedList));
	fprintf(stderr, "Number of monitors in the free list: %i\n",
		    FreeListCount(_MonitorPool.freeList));
	fprintf(stderr, "Number of reclaimed monitors: %i\n",
					_MonitorPool.numReclaimed);
	fprintf(stderr, "Number of abandoned monitors: %i\n",
					_MonitorPool.numAbandoned);
}
#endif

static void _ILMonitorPoolInit(void)
{
	ILMemPoolInitType(&(_MonitorPool.pool), ILMonitor, 20);
	_MonitorPool.freeList = 0;
	_MonitorPool.usedList = 0;
#ifdef IL_THREAD_DEBUG
	_MonitorPool.numAbandoned = 0;
#endif
	_ILCriticalSectionCreate(&(_MonitorPool.lock));
}

static void _ILMonitorPoolDestroy(void)
{
#ifdef IL_THREAD_DEBUG
	ILMonitorPrintStats();
#endif
	/*
	 * NOTE: If the usedList is not 0 we will be in very big trouble afterwards
	 * if references to monitors in the pool are used later.
	 */
	DestroyMonitorUsedList(_MonitorPool.usedList);
	_MonitorPool.usedList = 0;
	_MonitorPool.freeList = 0;
	_ILCriticalSectionDestroy(&(_MonitorPool.lock));
	ILMemPoolDestroy(&(_MonitorPool.pool));
}

/*
 * Get a new monitor from the monitor pool.
 * The thread parameter *MUST* be the current thread. 
 * The monitor is owned by the current if the function returns success.
 * This function must be called with the monitorpool lock held.
 */
static int _ILMonitorPoolAllocMonitor(ILThread *thread, ILMonitor **monitorLocation)
{
	ILMonitor *monitor;
	int result;

	if(_MonitorPool.freeList)
	{
		/* We have a monitor on the global freelist so reuse this one */
		monitor = _MonitorPool.freeList;
		_MonitorPool.freeList = monitor->nextFree;
		monitor->nextFree = (ILMonitor *)0;
		if((result = _ILMonitorAcquire(&(monitor->monitor))) == IL_THREAD_OK)
		{
			/* Initialize the monitor state */
			monitor->owner = thread;
			monitor->enterCount = 1;
			monitor->users = 1;
			/* Return the monitor */
			*monitorLocation = monitor;
		}
		else
		{
			/* Add this monitor to the freelist */
			monitor->nextFree = _MonitorPool.freeList;
			_MonitorPool.freeList = monitor;
		}
		return result;
	}
	else
	{
		monitor = ILMemPoolAllocItem(&(_MonitorPool.pool));
		if(monitor)
		{
			/* Add the monitor to the list of monitors*/
			monitor->nextUsed = _MonitorPool.usedList;
			_MonitorPool.usedList = monitor;

			if((result = ILMonitorInit(monitor, thread)) == IL_THREAD_OK)
			{
				/* Return the monitor */
				*monitorLocation = monitor;
			}
			else
			{
				/* Add this monitor to the freelist */
				monitor->nextFree = _MonitorPool.freeList;
				_MonitorPool.freeList = monitor;
			}
			return result;
		}
		return IL_THREAD_ERR_OUTOFMEMORY;
	}
	return IL_THREAD_ERR_UNKNOWN;
}

/*
 * Reclaim a monitor that's no longer needed.
 * If the monitor is in the used list of the monitor pool it will be
 * destroyed and moved to the memory pool freelist.
 * This function must be called with the monitorpool lock held.
 */
static void _ILMonitorPoolReclaimMonitor(_ILMonitorPool *pool,
										 ILMonitor *monitor)
{
	if(pool->usedList == monitor)
	{
		pool->usedList = monitor->nextUsed;
		ILMonitorDestroy(monitor);
		ILMemPoolFree(&(pool->pool), monitor);
#ifdef IL_THREAD_DEBUG
		++(pool->numReclaimed);
#endif
	}
	else
	{
		ILMonitor *prevMonitor;

		prevMonitor = pool->usedList;
		while(prevMonitor)
		{
			if(prevMonitor->nextUsed == monitor)
			{
				prevMonitor->nextUsed = monitor->nextUsed;
				ILMonitorDestroy(monitor);
				ILMemPoolFree(&(pool->pool), monitor);
#ifdef IL_THREAD_DEBUG
				++(pool->numReclaimed);
#endif
				return;
			}
			prevMonitor = prevMonitor->nextUsed;
		}
	}
}

/*
 * Add a monitor to the global freelist.
 * The monitor pool lock must be held.
 */
static void _ILMonitorPoolAddToFreeList(_ILMonitorPool *pool,
										 ILMonitor *monitor)
{
	monitor->nextFree = pool->freeList;
	pool->freeList = monitor;
}

/*
 * Add a list of monitors to the global freelist.
 * The monitor pool lock must be held.
 */
static void _ILMonitorPoolAddListToFreeList(_ILMonitorPool *pool,
											 ILMonitor *firstMonitor,
											 ILMonitor *lastMonitor)
{
	lastMonitor->nextFree = pool->freeList;
	pool->freeList = firstMonitor;
}

void _ILMonitorSystemInit()
{
	_ILMonitorPoolInit();
}

void _ILMonitorSystemDeinit()
{
	_ILMonitorPoolDestroy();
}

void _ILMonitorDestroyThread(ILThread *thread)
{
	IL_THREAD_ASSERT(thread == _ILThreadGetSelf());
	if(thread->monitorFreeList)
	{
		ILMonitor *firstMonitor;
		ILMonitor *lastMonitor;

		firstMonitor = thread->monitorFreeList;
		thread->monitorFreeList = 0;
		lastMonitor = firstMonitor;
		while(lastMonitor)
		{
			IL_THREAD_ASSERT(lastMonitor->users == 1)
			lastMonitor->owner = 0;
			lastMonitor->users = 0;
			_ILMonitorRelease(&(lastMonitor->monitor));
			if(lastMonitor->nextFree == 0)
			{
				break;
			}
			lastMonitor = lastMonitor->nextFree;
		}

		/* Lock the monitor system */
		_ILCriticalSectionEnter(&(_MonitorPool.lock));

		_ILMonitorPoolAddListToFreeList(&_MonitorPool,
										firstMonitor, lastMonitor);

		/* Unlock the monitor system */
		_ILCriticalSectionLeave(&(_MonitorPool.lock));
	}
}

int ILMonitorTimedTryEnter(void **monitorLocation, ILUInt32 ms)
{
	ILMonitor *monitor;
	ILThread *thread;
	int result;
	int waitStateResult;

	if(!monitorLocation)
	{
		return IL_THREAD_ERR_UNKNOWN;
	}
	/* Get my thread */
	thread = _ILThreadGetSelf();

	monitor = (ILMonitor *)ILInterlockedLoadP(monitorLocation);
	if(monitor != 0)
	{
		if(monitor->owner == thread)
		{
			/*
			 * I'm already the owner of this monitor.
			 * So simply increase the enter count.
			 */
			++(monitor->enterCount);
			return IL_THREAD_OK;
		}
	}
	else if(thread->monitorFreeList)
	{
		/*
		 * The monitor location is not occupied and there is a monitor in the
		 * thread's freelist so try to get the monitor without acquiring the
		 * monitor pool lock.
		 */
		monitor = thread->monitorFreeList;
		if(ILInterlockedCompareAndExchangeP_Acquire(monitorLocation,
													monitor, 0) == 0)
		{
			/*
			 * Remove the monitor from the thread's freelist.
			 */
			thread->monitorFreeList = monitor->nextFree;
			monitor->nextFree = 0;
			--thread->monitorFreeCount;
			return IL_THREAD_OK;
		}
	}
	/*
	 * If we get here we have to acquire the monitor the hard way.
	 */

	/* Lock the monitor system */
	_ILCriticalSectionEnter(&(_MonitorPool.lock));

	monitor = (ILMonitor *)ILInterlockedLoadP(monitorLocation);
	if(monitor == 0)
	{
		if(thread->monitorFreeList)
		{
			monitor = thread->monitorFreeList;
			if(ILInterlockedCompareAndExchangeP_Acquire(monitorLocation,
														monitor, 0) == 0)
			{
				/*
				 * Release the global monitor lock.
				 */
				_ILCriticalSectionLeave(&(_MonitorPool.lock));

				/*
				 * And remove the monitor from the thread's freelist.
				 */
				thread->monitorFreeList = monitor->nextFree;
				monitor->nextFree = 0;
				--thread->monitorFreeCount;
				return IL_THREAD_OK;
			}
			monitor = (ILMonitor *)ILInterlockedLoadP(monitorLocation);
		}
		else
		{
			/* We have to allocate a new monitor */
			result = _ILMonitorPoolAllocMonitor(thread, &monitor);

			if(result == IL_THREAD_OK)
			{
				if(ILInterlockedCompareAndExchangeP_Acquire(monitorLocation,
															monitor, 0) == 0)
				{
					/*
					 * Release the global monitor lock.
					 */
					_ILCriticalSectionLeave(&(_MonitorPool.lock));
					return result;
				}
				monitor = (ILMonitor *)ILInterlockedLoadP(monitorLocation);
			}
			else
			{
				/* Unlock the monitor system */
				_ILCriticalSectionLeave(&(_MonitorPool.lock));
				return result;
			}
		}
	}
	if(monitor->owner == 0)
	{
		/* Add me to the monitor users */
		++(monitor->users);

		/*
		 * We can acquire the monitor in the lock here because we are
		 * sure that it's not owned by someone else.
		 */
		result = _ILMonitorTryEnter(&(monitor->monitor));
		if(result == IL_THREAD_OK)
		{
			/* Set me as owner of the monitor */
			monitor->owner = thread;
			/* And initialize my enter count to 1 */
			monitor->enterCount = 1;

			/* Unlock the monitor system */
			_ILCriticalSectionLeave(&(_MonitorPool.lock));

			return result;
		}
		else if(result == IL_THREAD_BUSY)
		{
			if(ms == 0)
			{
				/* Rmove me from the monitor users */
				--(monitor->users);

				/* Unlock the monitor system */
				_ILCriticalSectionLeave(&(_MonitorPool.lock));

				return result;
			}
		}
		/* Rmove me from the monitor users */
		--(monitor->users);
	}
	if(ms == 0)
	{
		/*
		 * We have to acquire the monitor lock without waiting.
		 * This is not possible.
		 */

		/* Unlock the monitor system */
		_ILCriticalSectionLeave(&(_MonitorPool.lock));

		return IL_THREAD_BUSY;
	}
	/*
	 * Enter the Wait/Sleep/Join state.
	 */
	result = _ILThreadEnterWaitState(thread);
	if(result != IL_THREAD_OK)
	{
		/* Unlock the monitor pool */
		_ILCriticalSectionLeave(&(_MonitorPool.lock));

		return result;
	}
	/* Add me to the monitor users */
	++(monitor->users);

	/* Unlock the monitor pool */
	_ILCriticalSectionLeave(&(_MonitorPool.lock));

	/* Try to acquire the monitor */
	result = _ILMonitorTimedTryEnter(&(monitor->monitor), ms);

	waitStateResult = _ILThreadLeaveWaitState(thread, result);

	if((result == IL_THREAD_OK) && (waitStateResult == IL_THREAD_OK))
	{
		/*
		 * The owner should be 0 at this point.
		 */
		monitor->owner = thread;
		monitor->enterCount = 1;
	}
	else
	{
		/* Lock the monitor system again */
		_ILCriticalSectionEnter(&(_MonitorPool.lock));

		/* Remove me from the user list */
		--(monitor->users);

		/*
		 * If we acquired the monitor successfully but got interrupted
		 * we have to exit the monitor now.
		 */
		if(result == IL_THREAD_OK)
		{
			_ILMonitorExit(&(monitor->monitor));
		}

		if(monitor->users <= 0)
		{
			/* No other waiters on the monitor */
			monitor->users = 0;
			/* So move the monitor to the free list of the  pool */
			_ILMonitorPoolAddToFreeList(&_MonitorPool, monitor);
			/* And clear the monitor location */
			ILInterlockedStoreP(monitorLocation, 0);
		}

		/* Unlock the monitor pool */
		_ILCriticalSectionLeave(&(_MonitorPool.lock));

		if(waitStateResult != IL_THREAD_OK)
		{
			result = waitStateResult;
		}
	}
	return result;
}

int ILMonitorExit(void **monitorLocation)
{
	ILMonitor * volatile *monLoc;
	ILMonitor *monitor;
	ILThread *thread;

	if(!monitorLocation)
	{
		return IL_THREAD_ERR_UNKNOWN;
	}
	/* Store the location in a volatile variable */
	monLoc = (ILMonitor **)monitorLocation;
	if((monitor = *monLoc) == 0)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}
	thread = _ILThreadGetSelf();
	if(monitor->owner != thread)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}
	if(monitor->enterCount <= 1)
	{
		/* We really are leaving the monitor */
		_ILCriticalSectionEnter(&(_MonitorPool.lock));

		if(monitor->users <= 1)
		{
			/*
			 * I'm the only thread in this monitor.
			 * So move the monitor to the freelist.
			 */

			/* Clear the monitor location first */
			ILInterlockedStoreP(monitorLocation,  0);

			_ILCriticalSectionLeave(&(_MonitorPool.lock));

			if(thread->monitorFreeCount < MAX_FREELIST_MONITORS)
			{
				/* Add the monitor to the thread local freelist */
				monitor->nextFree = thread->monitorFreeList;
				thread->monitorFreeList = monitor;
				++thread->monitorFreeCount;
			}
			else
			{
				/*
				 * Move monitors from the thread local freelist to the global
				 * freelist.
				 */
				ILMonitor *firstMonitor;
				ILMonitor *lastMonitor;
				int current;

				/*
				 * First look for the last monitor that should stay on the
				 * local freelist.
				 */
				firstMonitor = thread->monitorFreeList;
				for(current = 0; current < (MIN_FREELIST_MONITORS - 1); ++current)
				{
					if(firstMonitor == 0)
					{
						break;
					}
					firstMonitor = firstMonitor->nextFree;
				}

				if(firstMonitor)
				{
					lastMonitor = firstMonitor->nextFree;
					firstMonitor->nextFree = 0;
					firstMonitor = lastMonitor;
					thread->monitorFreeCount = current + 1;

					while(lastMonitor)
					{
						IL_THREAD_ASSERT(lastMonitor->users == 1)
						lastMonitor->owner = 0;
						lastMonitor->users = 0;
						_ILMonitorRelease(&(lastMonitor->monitor));
						if(lastMonitor->nextFree == 0)
						{
							break;
						}
						lastMonitor = lastMonitor->nextFree;
					}
					/*
					 * Make the  current monitor the head of the monitor list
					 * to be moved to the global freelist.
					 */
					monitor->nextFree = firstMonitor;
					firstMonitor = monitor;
				}
				else
				{
					/*
					 * If this happens something is wrong or MIN and MAX
					 * FREELIST_MONITORS are not set correctly.
					 * So simply make monitor a single element list.
					 */
					firstMonitor = monitor;
					lastMonitor = monitor;
					thread->monitorFreeCount = current;
				}

				/* Release the current monitor */
				IL_THREAD_ASSERT(monitor->users == 1)
				monitor->owner = 0;
				monitor->users = 0;
				_ILMonitorRelease(&(monitor->monitor));

				/* Lock the monitor system */
				_ILCriticalSectionEnter(&(_MonitorPool.lock));

				_ILMonitorPoolAddListToFreeList(&_MonitorPool,
												firstMonitor,
												lastMonitor);

				/* Unlock the monitor system */
				_ILCriticalSectionLeave(&(_MonitorPool.lock));
			}
		}
		else
		{
			/* Remove me from the monitor users */
			--(monitor->users);
			
			/* Clear the owner and owner private data */
			monitor->owner = 0;
			monitor->enterCount = 0;

			_ILCriticalSectionLeave(&(_MonitorPool.lock));
			_ILMonitorExit(&(monitor->monitor));
		}
	}
	else
	{
		/* Simpy decrement the enter count */
		--(monitor->enterCount);
	}
	return IL_THREAD_OK;
}

int ILMonitorPulse(void **monitorLocation)
{
	ILMonitor *monitor;
	ILThread *thread;

	if(!monitorLocation)
	{
		return IL_THREAD_ERR_UNKNOWN;
	}
	if((monitor = (ILMonitor *)(*monitorLocation)) == 0)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}
	thread = _ILThreadGetSelf();
	if(monitor->owner != thread)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}
	_ILMonitorPulse(&(monitor->monitor));
	return IL_THREAD_OK;
}

int ILMonitorPulseAll(void **monitorLocation)
{
	ILMonitor *monitor;
	ILThread *thread;

	if(!monitorLocation)
	{
		return IL_THREAD_ERR_UNKNOWN;
	}
	if((monitor = (ILMonitor *)(*monitorLocation)) == 0)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}
	thread = _ILThreadGetSelf();
	if(monitor->owner != thread)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}
	_ILMonitorPulseAll(&(monitor->monitor));
	return IL_THREAD_OK;
}

int ILMonitorTimedWait(void **monitorLocation, ILUInt32 ms)
{
	ILMonitor *monitor;
	ILThread *thread;
	int result;
	int waitStateResult;
	ILUInt32 enterCount;

	if(!monitorLocation)
	{
		return IL_THREAD_ERR_UNKNOWN;
	}
	thread = _ILThreadGetSelf();
	monitor = (ILMonitor *)(*monitorLocation);
	if(!monitor || monitor->owner != thread)
	{
		return IL_THREAD_ERR_SYNCLOCK;
	}

	/*
	 * Enter the Wait/Sleep/Join state.
	 */
	result = _ILThreadEnterWaitState(thread);
	if(result != IL_THREAD_OK)
	{
		return result;
	}
	/*
	 * Save the number of times this thread entered the monitor, reset
	 + the value in the monitor and clear the owner.
	 */
	enterCount = monitor->enterCount;
	monitor->enterCount = 0;
	monitor->owner = 0;
	result = _ILMonitorTimedWait(&(monitor->monitor), ms);
	monitor->enterCount = enterCount;
	monitor->owner = thread;
	
	waitStateResult = _ILThreadLeaveWaitState(thread, result);
	if(waitStateResult != IL_THREAD_OK)
	{
		result = waitStateResult;
	}
	return result;
}

/*
 * Reclaim a monitor that's no longer needed.
 * This function is usually called if an object is destroyed where
 * a monitor is attached,
 */
void ILMonitorReclaim(void **monitorLocation)
{
	ILMonitor * volatile *monLoc;
	ILMonitor *monitor;

	/*
	 * Validate the arguments.
	 */
	if(!monitorLocation)
	{
		return;
	}
	monLoc = (ILMonitor **)monitorLocation;
	if((monitor = *monLoc) == 0)
	{
		/* No Monitor attached to the location */
		return;
	}
	/* clear the monitor location */
	*monLoc = 0;

	/* Lock the pool */
	_ILCriticalSectionEnter(&(_MonitorPool.lock));

	/* Abandon the monitor in the pool */
	_ILMonitorPoolReclaimMonitor(&_MonitorPool, monitor);

	/* Unlock the pool */
	_ILCriticalSectionLeave(&(_MonitorPool.lock));
}

#ifdef	__cplusplus
};
#endif
