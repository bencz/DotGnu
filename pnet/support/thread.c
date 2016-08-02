/*
 * thread.c - Thread management routines.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * Contributions from Thong Nguyen (tum) [tum@veridicus.com]
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
the correct CLI threading semantics based on the primitives in "*_defs.h".
You normally won't need to modify or replace this file when porting.

*/

#include "thr_defs.h"
#include "interlocked.h"
#include "interrupt.h"

#ifdef HAVE_ALLOCA_H
#include <alloca.h>
#endif
#ifdef HAVE_MALLOC_H
#include <malloc.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Global state that is mutexed between all threads.
 */
static _ILCriticalSection threadLockAll;
/* Number of threads that have started and not finished */
static long volatile numThreads;
/* Number of threads that have started and not finished and are background threads */
static long volatile numBackgroundThreads;
/* Event that is set when there are no foreground threads left */
static ILWaitHandle *noFgThreadsEvent;

/*
 * Global critical section for atomic operations.
 */
static _ILCriticalSection atomicLock;

/*
 * The "main" thread's object.
 */
static ILThread mainThread;

int ILHasThreads(void)
{
	return _ILHasThreads();
}

/*
 * Thread library initialization routines that are called once only.
 */
static void _ILThreadInit(void)
{
	/* Initialize the main thread to all 0s */
	ILMemZero(&mainThread, sizeof(ILThread));

	/* Perform system-specific initialization */
	_ILThreadInitSystem(&mainThread);

	/* Initialize synchronization objects that we need */
	_ILCriticalSectionCreate(&threadLockAll);
	_ILCriticalSectionCreate(&atomicLock);

	/* Set up the "main" thread.  "_ILThreadInitSystem" has already
	   set the "handle" and "identifier" fields for us */
	_ILCriticalSectionCreate(&(mainThread.lock));
	mainThread.state.split.priv = IL_TS_RUNNING;
	mainThread.useCount			= 2;	/* 2 here because the thread is already started */
	_ILSemaphoreCreate(&(mainThread.resumeAck));
	_ILSemaphoreCreate(&(mainThread.suspendAck));
	mainThread.monitor = ILWaitMonitorCreate();

	_ILWakeupCreate(&(mainThread.wakeup));
	_ILWakeupQueueCreate(&(mainThread.joinQueue));

	noFgThreadsEvent = ILWaitEventCreate(1, 1);

	/* Set the thread object for the "main" thread */
	_ILThreadSetSelf(&mainThread);

	/* Initialize the implementation specific thread members */
	#ifdef _ILTHREAD_EXT_INIT
		_IL_THREAD_EXT_INIT(&mainThread);
	#endif

	/* We have 1 foreground thread in the system at present */
	numThreads = 1;
	numBackgroundThreads = 0;

	_ILInterruptInit();

	/* Initialize the monitor subsystem */
	_ILMonitorSystemInit();
}

static void _ILThreadDeinit(void)
{
	/* Cleanup the monitor subsystem */
	_ILMonitorDestroyThread(&mainThread);
	_ILMonitorSystemDeinit();

	_ILInterruptDeinit();

	if(noFgThreadsEvent != 0)
	{
		ILWaitHandleClose(noFgThreadsEvent);
	}

	/* Destroy the main thread's resources */
	_ILWakeupQueueDestroy(&(mainThread.joinQueue));
	/* TODO: Find and fix the wakeup not removed
	_ILWakeupDestroy(&(mainThread.wakeup));
	*/
	if(mainThread.monitor)
	{
		ILWaitHandleClose(mainThread.monitor);
	}
	_ILSemaphoreDestroy(&(mainThread.suspendAck));
	_ILSemaphoreDestroy(&(mainThread.resumeAck));

	/* Deinitialize the implementation specific thread members */
	#ifdef _ILTHREAD_EXT_DEINIT
		_IL_THREAD_EXT_DEINIT(&mainThread);
	#endif

	_ILCriticalSectionDestroy(&(mainThread.lock));

	/* Destroy the synchronization objects */
	_ILCriticalSectionDestroy(&threadLockAll);
	_ILCriticalSectionDestroy(&atomicLock);
}

/*
 * Changes the counters for the number of threads.
 */
static void _ILThreadAdjustCount(int numThreadsAdjust, int numBackgroundThreadsAdjust)
{
	_ILCriticalSectionEnter(&threadLockAll);
	{
		numThreads += numThreadsAdjust;		
		numBackgroundThreads += numBackgroundThreadsAdjust;
		
		/* If there is only the main thread left then signal the
		   noFgThreads event */
		if (numThreads - numBackgroundThreads == 1)
		{
			ILWaitEventSet(noFgThreadsEvent);
		}
		else
		{
			ILWaitEventReset(noFgThreadsEvent);
		}
	}
	_ILCriticalSectionLeave(&threadLockAll);
}

void ILThreadInit(void)
{
	_ILCallOnce(_ILThreadInit);
}

void ILThreadDeinit(void)
{
	_ILCallOnce(_ILThreadDeinit);
}

ILWaitHandle *ILThreadGetMonitor(ILThread *thread)
{
	return thread->monitor;
}

static void _ILThreadRunAndFreeCleanups(ILThread *thread)
{
	ILThreadCleanupEntry *entry, *next;
	
	entry = thread->firstCleanupEntry;

	while (entry)
	{
		/* Call the cleanup function */
		entry->cleanup(thread);
		next = entry->next;

		/* Free the entry */
		ILFree(entry);

		entry = next;
	}

	thread->firstCleanupEntry = 0;
}

static void _ILPrivateThreadDestroy(ILThread *thread)
{
	_ILThreadState threadState;

	/* Bail out if this is the main thread */
	if(thread == &mainThread)
	{
		return;
	}

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	/* Decrement the usage counter */
	thread->useCount -= 1;
	
	if(thread->useCount > 0)
	{
		/* The thread is still in use */
		/* So unlock the thread object and return */
		_ILCriticalSectionLeave(&(thread->lock));
		return;
	}
	
	/* Get the complete threadstate */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));


	/* Don't destroy the thread if it's started and not yet stopped */
	if(((threadState.split.priv & IL_TS_STOPPED) == 0) &&
	   ((threadState.split.pub & IL_TS_UNSTARTED) == 0))
	{
		_ILCriticalSectionLeave(&(thread->lock));
		return;
	}
	else
	{
		/* Unlock the thread object and free it */
		_ILCriticalSectionLeave(&(thread->lock));
	}

	/* Run and free the cleanup handlers */
	_ILThreadRunAndFreeCleanups(thread);

	/* Only destroy the system thread if one was created */
	if((threadState.split.pub & IL_TS_UNSTARTED) == 0)
	{
		_ILThreadDestroy(thread);
	}

	ILWaitHandleClose(thread->monitor);
	_ILCriticalSectionDestroy(&(thread->lock));
	_ILSemaphoreDestroy(&(thread->suspendAck));
	_ILSemaphoreDestroy(&(thread->resumeAck));
	_ILWakeupQueueDestroy(&(thread->joinQueue));
#ifdef IL_THREAD_DEBUG
	fprintf(stderr, "ILThread destroyed: [%p]\n", thread);
#endif /* IL_THREAD_DEBUG */
	ILFree(thread);
}

void ILThreadDestroy(ILThread *thread)
{
	_ILPrivateThreadDestroy(thread);
}

void *ILThreadRunSelf(void *(* thread_func)(void *), void *arg)
{
	ILThread *thread_self;
	void *result;

	/*
	 * Create a new thread object and populate it.
	 * All members are guaranteed to be inititialized to 0 after this point.
	 */
	thread_self = (ILThread *)ILCalloc(1, sizeof(ILThread));
	if(!thread_self)
	{
		return 0;
	}

	_ILCriticalSectionCreate(&(thread_self->lock));
	thread_self->state.split.priv = IL_TS_RUNNING;
	thread_self->monitor = ILWaitMonitorCreate();
	_ILSemaphoreCreate(&(thread_self->resumeAck));
	_ILSemaphoreCreate(&(thread_self->suspendAck));
	_ILWakeupCreate(&(thread_self->wakeup));
	_ILWakeupQueueCreate(&(thread_self->joinQueue));
	thread_self->useCount = 2;	/* 2 here because the thread is already started */
	
	/* Initialize the handle and the identifier */
	_ILThreadInitHandleSelf(thread_self);

	/* Set the thread object for the thread */
	_ILThreadSetSelf(thread_self);

	/* Initialize the implementation specific thread members */
	#ifdef _ILTHREAD_EXT_INIT
		_IL_THREAD_EXT_INIT(thread_self);
	#endif

	result = ILGCRunFunc(thread_func, arg);

	_ILThreadRunAndFreeCleanups(thread_self);

	/* and now destroy the ILThread instance. */
	_ILMonitorDestroyThread(thread_self);
	ILWaitHandleClose(thread_self->monitor);
	_ILSemaphoreDestroy(&(thread_self->suspendAck));
	_ILSemaphoreDestroy(&(thread_self->resumeAck));
	_ILWakeupQueueDestroy(&(thread_self->joinQueue));
	/* Deinitialize the implementation specific thread members */
	#ifdef _ILTHREAD_EXT_DEINIT
		_IL_THREAD_EXT_DEINIT(thread_self);
	#endif
	_ILCriticalSectionDestroy(&(thread_self->lock));

	_ILThreadSetSelf(0);
	/* Release the handle */
	_ILThreadDestroy(thread_self);

	ILFree(thread_self);

	return result;
}

void _ILThreadRun(ILThread *thread)
{
	_ILThreadState threadState;
	
	/* When a thread starts, it blocks until the ILThreadStart function
	   has finished setup */
	/* Wait until the starting thread has released the lock */
	_ILCriticalSectionEnter(&(thread->lock));
	_ILCriticalSectionLeave(&(thread->lock));

	/* Initialize the implementation specific thread members */
	#ifdef _ILTHREAD_EXT_INIT
		_IL_THREAD_EXT_INIT(thread);
	#endif

	/* Mark the thread as running */
	ILInterlockedStoreU2(&(thread->state.split.priv), IL_TS_RUNNING);

	/* If we still have a startup function, then execute it.
	   The field may have been replaced with NULL if the thread
	   was aborted before it even got going */
	if(thread->startFunc)
	{
		(*(thread->startFunc))(thread->startArg);
	}

	thread->startArg = 0;

	/* Mark the thread as stopped */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	threadState.split.priv |= IL_TS_STOPPED;
	ILInterlockedStoreU2(&(thread->state.split.priv), threadState.split.priv);

	/* Change the thread count */
	_ILThreadAdjustCount(-1, ((threadState.split.pub & IL_TS_BACKGROUND) != 0) ? -1 : 0);

	/* Clean up the monitors used */
	_ILMonitorDestroyThread(thread);

	_ILCriticalSectionEnter(&(thread->lock));
	{
		/* Wakeup everyone waiting to join */
		_ILWakeupQueueWakeAll(&(thread->joinQueue));
	}
	_ILCriticalSectionLeave(&(thread->lock));

	/* The wakeup isn't needed anymore so destroy it now to allow
	   held mutexes to be released */
	_ILWakeupDestroy(&(thread->wakeup));

	/* Deinitialize the implementation specific thread members */
	#ifdef _ILTHREAD_EXT_DEINIT
		_IL_THREAD_EXT_DEINIT(thread);
	#endif

	_ILPrivateThreadDestroy(thread);
}

ILThread *ILThreadCreate(ILThreadStartFunc startFunc, void *startArg)
{
	ILThread *thread;

	/* We cannot create threads if the system doesn't really support them */
	if(!_ILHasThreads())
	{
		return 0;
	}
	/*
	 * Create a new thread object and populate it.
	 * All members are guaranteed to be initialized to 0 after this point.
	 */
	thread = (ILThread *)ILCalloc(1, sizeof(ILThread));
	if(!thread)
	{
		return 0;
	}

	_ILCriticalSectionCreate(&(thread->lock));
	thread->state.split.pub = IL_TS_UNSTARTED;
	thread->monitor = ILWaitMonitorCreate();
	_ILSemaphoreCreate(&(thread->resumeAck));
	_ILSemaphoreCreate(&(thread->suspendAck));
	thread->startFunc = startFunc;
	thread->startArg = startArg;
	_ILWakeupCreate(&(thread->wakeup));
	_ILWakeupQueueCreate(&(thread->joinQueue));
	thread->useCount = 1;	/* 1 here because the handle will be returned */

	/* Make sure everything setup is seen by all threads. */
	ILInterlockedMemoryBarrier();

#ifdef IL_THREAD_DEBUG
	fprintf(stderr, "ILThread created: [%p]\n", thread);
#endif /* IL_THREAD_DEBUG */

	return thread;
}

int ILThreadStart(ILThread *thread)
{
	_ILThreadState threadState;

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	/* Are we in the correct state to start? */
	if((threadState.split.pub & IL_TS_UNSTARTED) != 0)
	{
		int result;
		
		/* Lock down the thread object */
		_ILCriticalSectionEnter(&(thread->lock));

		/* reread the thread's state because of possible race conditions */
		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));

		/* And check again with the lock held */
		if((threadState.split.pub & IL_TS_UNSTARTED) != 0)
		{
			/* Create the new thread */
			if(!_ILThreadCreateSystem(thread))
			{
				result = 0;
			}
			else
			{
				int isBackground;

				/* Increment the use count because the thread is started now */
				thread->useCount += 1;
				
				/* Clear the thread's unstarted flag */
				threadState.split.pub &= ~IL_TS_UNSTARTED;
				
				ILInterlockedStoreU2(&(thread->state.split.pub),
									 threadState.split.pub);

				isBackground = ((threadState.split.pub & IL_TS_BACKGROUND) != 0);
				_ILThreadAdjustCount(1, isBackground ? 1 : 0);

				result = 1;
			}
		}
		else
		{
			result = 0;
		}
		/* Unlock the thread object and return */
		_ILCriticalSectionLeave(&(thread->lock));

		return result;
	}
	return 0;
}

ILThread *ILThreadSelf(void)
{
#ifdef IL_NO_THREADS
	return &mainThread;
#else
	return _ILThreadGetSelf();
#endif
}

void *ILThreadGetObject(ILThread *thread)
{
	return thread->userObject;
}

void ILThreadSetObject(ILThread *thread, void *userObject)
{
	thread->userObject = userObject;
}

int ILThreadSuspend(ILThread *thread)
{
	return ILThreadSuspendRequest(thread, 0);
}

int ILThreadSuspendRequest(ILThread *thread, int requestOnly)
{
	_ILThreadState threadState;
	int result = IL_SUSPEND_OK;

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	/* Determine what to do based on the thread's state */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.pub & (IL_TS_ABORT_REQUESTED)) != 0)
	{
		result = IL_WAIT_ABORTED;
	}
	else if((threadState.split.pub & IL_TS_SUSPENDED) != 0)
	{
		/* Nothing to do here - it is already suspended */
	}
	else if(((threadState.split.pub & IL_TS_UNSTARTED) != 0) ||
			 ((threadState.split.priv &  IL_TS_STOPPED) != 0))
	{
		/* We cannot suspend a thread that was never started
		   in the first place, or is stopped */
		result = IL_SUSPEND_FAILED;
	}
	else if(((threadState.split.priv & IL_TS_WAIT_SLEEP_JOIN) != 0) ||
			requestOnly)
	{
		/* Request a suspend, but otherwise ignore the request */
		threadState.split.pub |= IL_TS_SUSPEND_REQUESTED;
		ILInterlockedStoreU2(&(thread->state.split.pub),
							 threadState.split.pub);

		result = IL_SUSPEND_REQUESTED;
	}
	else if(_ILThreadIsSelf(thread))
	{
		/* Mark the thread as suspended */
		threadState.split.pub &= ~IL_TS_SUSPEND_REQUESTED;
		threadState.split.pub |= (IL_TS_SUSPENDED | IL_TS_SUSPENDED_SELF);
		ILInterlockedStoreU2(&(thread->state.split.pub),
							 threadState.split.pub);

		/* Unlock the thread object prior to suspending */
		_ILCriticalSectionLeave(&(thread->lock));

		/* Suspend until we receive notification from another thread */
		_ILThreadSuspendSelf(thread);

		/* We are resumed, and the thread object is already unlocked */
		return IL_SUSPEND_OK;
	}
	else
	{
		/* Mark the thread as waiting for a resume */
		threadState.split.pub |= IL_TS_SUSPENDED;
		ILInterlockedStoreU2(&(thread->state.split.pub),
							 threadState.split.pub);

		/* Put the thread to sleep temporarily */
		_ILThreadSuspendOther(thread);

		/* If the thread does not hold any locks, then everything is OK */
		if(!(thread->numLocksHeld))
		{
			_ILCriticalSectionLeave(&(thread->lock));
			return IL_SUSPEND_OK;
		}

		/* Mark the thread  suspend itself */
		threadState.split.pub &= ~IL_TS_SUSPENDED;
		threadState.split.pub |= IL_TS_SUSPEND_REQUESTED;
		ILInterlockedStoreU2(&(thread->state.split.pub),
							 threadState.split.pub);

		/* Notify the thread that we want it to suspend itself */
		thread->suspendRequested = 1;

		/* Resume the thread to allow it to give up all locks that it has */
		_ILThreadResumeOther(thread);

		/* Give up the lock on the thread, but don't reduce
		   "numLocksHeld" on the current thread just yet */
		_ILCriticalSectionLeaveUnsafe(&(thread->lock));

		/* Wait for the thread to signal us that it has put itself to sleep */
		_ILSemaphoreWait(&(thread->suspendAck));

		/* Re-acquire the lock on the thread object */
		_ILCriticalSectionEnterUnsafe(&(thread->lock));
	}

	/* Unlock the thread object and return */
	_ILCriticalSectionLeave(&(thread->lock));
	return result;
}

void ILThreadResume(ILThread *thread)
{
	_ILThreadState threadState;

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	/* Determine what to do based on the thread's state */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.pub & IL_TS_SUSPENDED) != 0)
	{
		if((threadState.split.pub & IL_TS_SUSPENDED_SELF) != 0)
		{
			/* The thread put itself to sleep */
			threadState.split.pub &= ~(IL_TS_SUSPENDED | IL_TS_SUSPENDED_SELF);
			threadState.split.pub |= IL_TS_RUNNING;
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);
			_ILThreadResumeSelf(thread);
		}
		else
		{
			/* Someone else suspended the thread */
			threadState.split.pub &= ~IL_TS_SUSPENDED;
			threadState.split.pub |= IL_TS_RUNNING;
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);
			_ILThreadResumeOther(thread);
		}
	}
	else if((threadState.split.pub & IL_TS_SUSPEND_REQUESTED) != 0)
	{
		/* A suspend was requested, but it hadn't started yet */
		threadState.split.pub &= ~IL_TS_SUSPEND_REQUESTED;
		ILInterlockedStoreU2(&(thread->state.split.pub),
							 threadState.split.pub);
		thread->suspendRequested = 0;
		/*
		 * TODO: Notify the thread that requested the suspend becaue it might be
		 * waiting for the ack.
		 */
	}

	/* Unlock the thread object */
	_ILCriticalSectionLeave(&(thread->lock));
}

void ILThreadInterrupt(ILThread *thread)
{
	_ILThreadState threadState;

	/* Determine what to do based on the thread's state */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.priv & IL_TS_STOPPED) == 0)
	{
		/* Lock down the thread object */
		_ILCriticalSectionEnter(&(thread->lock));

		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
		if(((threadState.split.pub & IL_TS_UNSTARTED) == 0) &&
		   ((threadState.split.priv & IL_TS_STOPPED) == 0))
		{
			/* Mark the thread as interrupted */
			threadState.split.pub |= IL_TS_INTERRUPTED;
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);

			/* Unlock the thread object: we never hold the thread
			   lock when updating the thread's wakeup object */
			_ILCriticalSectionLeave(&(thread->lock));

			/* Mark the thread as needing to be interrupted the next
			   time a "wait/sleep/join" occurs */
			_ILWakeupInterrupt(&(thread->wakeup));

			/* post the interrupt request to the thread */
			_ILThreadInterrupt(thread);
		}
		else
		{
			/* Unlock the thread object */
			_ILCriticalSectionLeave(&(thread->lock));
		}
	}
}

int ILThreadSelfAborting()
{
	int result;
	_ILThreadState threadState;
	ILThread *thread = _ILThreadGetSelf();
	
	/* Determine if we've already seen the abort request or not */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.priv & IL_TS_ABORTED) != 0)
	{
		/* Already aborted */
		result = 0;
	}
	else if((threadState.split.pub & IL_TS_ABORT_REQUESTED) != 0)
	{
		/* Abort was requested */
		_ILCriticalSectionEnter(&(thread->lock));
	
		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
		if((threadState.split.pub & IL_TS_ABORT_REQUESTED) != 0)
		{
			threadState.split.pub &= ~IL_TS_ABORT_REQUESTED;
			threadState.split.priv |= IL_TS_ABORTED;
			ILInterlockedStoreU4(&(thread->state.comb), threadState.comb);
			result = 1;
		}
		else
		{
			/*
			 * This possibly can never happen because only the current thread
			 * can change the abort request state once it has been set by an
			 * other thread or the thread itself.
			 */
			result = 0;
		}
		_ILCriticalSectionLeave(&(thread->lock));
	}
	else
	{
		/* The thread is not aborting: we were called in error */
		result = 0;
	}
	return result;
}

void ILThreadSigAbort(ILThread *thread)
{
#ifdef IL_USE_PTHREADS
	if( 0 != thread && 0 != thread->handle )
	{
		pthread_kill(thread->handle, IL_SIG_ABORT);
	}
#endif
}

int ILThreadAbort(ILThread *thread)
{
	int result;
	_ILThreadState threadState;

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.priv & IL_TS_ABORTED) ||
	   (threadState.split.pub & IL_TS_ABORT_REQUESTED))
	{
		/* The thread is already processing an abort or an abort request */
		result = 0;
	}
	else
	{
		/* Mark the thread as needing to be aborted */
		threadState.split.pub |= IL_TS_ABORT_REQUESTED;
		ILInterlockedStoreU2_Acquire(&(thread->state.split.pub),
									 threadState.split.pub);

		/* And reload the thread state */
		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));

		if (((threadState.split.pub & (IL_TS_SUSPENDED_SELF | IL_TS_SUSPENDED))) != 0)
		{
			_ILCriticalSectionLeave(&(thread->lock));

			ILThreadResume(thread);

			return 0;
		}
		else if((threadState.split.priv & IL_TS_WAIT_SLEEP_JOIN) != 0)
		{
			/* Unlock the thread object: we never hold the thread
			   lock when updating the thread's wakeup object */
			_ILCriticalSectionLeave(&(thread->lock));

			_ILWakeupInterrupt(&(thread->wakeup));
			
			_ILThreadInterrupt(thread);

			return 0;
		}
		else
		{
			/* No need to abort the current thread ?? */
			/* Really ?
			threadState.split.pub &= ~IL_TS_ABORT_REQUESTED;
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);
			*/
		}
		/* No need to interrupt or resume the current thread */
		result = 0;
	}

	/* Unlock the thread object and return */
	_ILCriticalSectionLeave(&(thread->lock));
	return result;
}

int ILThreadIsAborting(void)
{
	ILThread *thread = _ILThreadGetSelf();
	ILUInt16 threadState;

	threadState = ILInterlockedLoadU2(&(thread->state.split.priv));
	/* Determine if an abort is in progress on this thread */
	return ((threadState & IL_TS_ABORTED) != 0);
}

int ILThreadIsAbortRequested(void)
{
	ILThread *thread = _ILThreadGetSelf();
	ILUInt16 threadState;

	threadState = ILInterlockedLoadU2(&(thread->state.split.pub));
	return ((threadState & IL_TS_ABORT_REQUESTED) != 0);
}

int ILThreadAbortReset(void)
{
	ILThread *thread = _ILThreadGetSelf();
	int result;
	_ILThreadState threadState;

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));

	/* Reset the "abort" and "abort requested" flags */
	if((threadState.split.priv & IL_TS_ABORTED) ||
	   (threadState.split.pub & IL_TS_ABORT_REQUESTED))
	{
		threadState.split.priv &= ~IL_TS_ABORTED;
		threadState.split.pub &= ~(IL_TS_ABORT_REQUESTED | IL_TS_INTERRUPTED);
		ILInterlockedStoreU4(&(thread->state.comb), threadState.comb);

		_ILWakeupCancelInterrupt(&thread->wakeup);

		result = 1;
	}
	else
	{
		result = 0;
	}

	/* Unlock the thread object and return */
	_ILCriticalSectionLeave(&(thread->lock));
	return result;
}

int ILThreadJoin(ILThread *thread, ILUInt32 ms)
{
	ILThread *self = _ILThreadGetSelf();
	int result;
	_ILThreadState threadState;

	/* Bail out if we are trying to join with ourselves */
	if(self == thread)
	{
		return IL_JOIN_SELF;
	}

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));

	/* Determine what to do based on the thread's state */
	if((threadState.split.priv & IL_TS_STOPPED) != 0)
	{
		/* The thread is already stopped, so return immediately */
		result = IL_JOIN_OK;
	}
	else if ((threadState.split.pub & IL_TS_UNSTARTED) != 0)
	{
		/* Can't join a thread that hasn't started */
		result = IL_JOIN_UNSTARTED;
	}
	else
	{
		/* Note: We must set our wait limit before adding ourselves to any wait queues.
		    Failure to do so may mean we may miss some signals because they will be 
			unset by the signal invoker (which reads us as having a 0 wait limit).
			In this specific case, the order doesn't actually matter because we have locked 
			the queue owner (the thread) but both operations must be performed before we
			unlock the thread - Tum */

		/* Set our wait limit to 1 */
		if(!_ILWakeupSetLimit(&(self->wakeup), 1))
		{
			result = -1;
		}
		else
		{
			/* Add ourselves to the foreign thread's join queue */
			if(!_ILWakeupQueueAdd(&(thread->joinQueue), &(self->wakeup), self))
			{
				result = IL_JOIN_MEMORY;
			}
			else
			{
				_ILThreadState selfState;

				/* Unlock the foreign thread */
				_ILCriticalSectionLeave(&(thread->lock));

				/* Put ourselves into the "wait/sleep/join" state */
				_ILCriticalSectionEnter(&(self->lock));
				selfState.comb = ILInterlockedLoadU4(&(self->state.comb));
				if((selfState.split.pub & IL_TS_ABORT_REQUESTED) != 0)
				{
					/* The current thread is aborted */
					_ILCriticalSectionLeave(&(self->lock));
					_ILCriticalSectionEnter(&(thread->lock));
					_ILWakeupQueueRemove(&(thread->joinQueue), &(self->wakeup));
					_ILCriticalSectionLeave(&(thread->lock));
					return IL_JOIN_ABORTED;
				}
				selfState.split.priv |= IL_TS_WAIT_SLEEP_JOIN;
				ILInterlockedStoreU2(&(self->state.split.priv), selfState.split.priv);
				_ILCriticalSectionLeave(&(self->lock));

				result = _ILWakeupWait(&(self->wakeup), ms, (void **)0);
				
				if(result < 0)
				{
					/* The wakeup was interrupted.  It may be either an
					"interrupt" or an "abort request".  We assume abort
					for now until we can inspect "self->state" below */
					result = IL_JOIN_ABORTED;
				}
				else if(result > 0)
				{
					result = IL_JOIN_OK;
				}
				else
				{
					result = IL_JOIN_TIMEOUT;
				}

				/* Remove ourselves from the "wait/sleep/join" state,
				and check for a pending interrupt */
				_ILCriticalSectionEnter(&(self->lock));

				selfState.comb = ILInterlockedLoadU4(&(self->state.comb));
				if((selfState.split.pub & IL_TS_INTERRUPTED) != 0)
				{
					result = IL_JOIN_INTERRUPTED;
				}
				selfState.split.priv &= ~IL_TS_WAIT_SLEEP_JOIN;
				selfState.split.pub &= ~IL_TS_INTERRUPTED;
				ILInterlockedStoreU4(&(self->state.comb), selfState.comb);
				/* Check and process any pending suspend request */
				threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
				if ((threadState.split.pub & IL_TS_SUSPEND_REQUESTED) != 0)
				{
					/* Unlock the thread object prior to suspending */
					_ILCriticalSectionLeave(&(self->lock));

					/* Lock down the foreign thread again */
					_ILCriticalSectionEnter(&(thread->lock));

					threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
					threadState.split.pub &= ~IL_TS_SUSPEND_REQUESTED;
					threadState.split.pub |= (IL_TS_SUSPENDED | IL_TS_SUSPENDED_SELF);
					ILInterlockedStoreU2(&(thread->state.split.pub),
										 threadState.split.pub);

					/* Remove ourselves from the foreign thread's join queue */
					_ILWakeupQueueRemove(&(thread->joinQueue), &(self->wakeup));

					/* Suspend until we receive notification from another thread */
					_ILThreadSuspendSelf(thread);
				}
				else
				{
					_ILCriticalSectionLeave(&(self->lock));

					/* Lock down the foreign thread again */
					_ILCriticalSectionEnter(&(thread->lock));

					/* Remove ourselves from the foreign thread's join queue */
					_ILWakeupQueueRemove(&(thread->joinQueue), &(self->wakeup));
				}
			}
		}
	}

	/* Unlock the thread object and return */
	_ILCriticalSectionLeave(&(thread->lock));
	return result;
}

int ILThreadGetBackground(ILThread *thread)
{
	ILUInt16 threadState;

	threadState = ILInterlockedLoadU2(&(thread->state.split.pub));
	/* Determine if this is a background thread */
	return ((threadState & IL_TS_BACKGROUND) != 0);
}

void ILThreadSetBackground(ILThread *thread, int flag)
{
	int change = 0;
	_ILThreadState threadState;

	/* Lock down the thread object */
	_ILCriticalSectionEnter(&(thread->lock));

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));

	/* Change the background state of the thread */
	if(flag)
	{
		if(!(threadState.split.pub & IL_TS_BACKGROUND))
		{
			threadState.split.pub |= IL_TS_BACKGROUND;
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);

			if(!(threadState.split.pub & IL_TS_UNSTARTED) && 
			   !(threadState.split.priv & IL_TS_STOPPED))
			{
				_ILThreadAdjustCount(0, 1);
			}
		}
	}
	else
	{
		if((threadState.split.pub & IL_TS_BACKGROUND))
		{
			threadState.split.pub &= ~IL_TS_BACKGROUND;
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);
			change = -1;

			if(!(threadState.split.pub & IL_TS_UNSTARTED) && 
			   !(threadState.split.priv & IL_TS_STOPPED))
			{
				_ILThreadAdjustCount(0, -1);
			}
		}
	}

	/* Unlock the thread object */
	_ILCriticalSectionLeave(&(thread->lock));
}

int ILThreadGetState(ILThread *thread)
{
	_ILThreadState threadState;

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	/* Return the publicly-interesting flags to the caller */
	return (int)((threadState.split.priv | threadState.split.pub) & IL_TS_PUBLIC_FLAGS);
}

void ILThreadAtomicStart(void)
{
	_ILCriticalSectionEnter(&atomicLock);
}

void ILThreadAtomicEnd(void)
{
	_ILCriticalSectionLeave(&atomicLock);
}

void ILThreadMemoryBarrier(void)
{
	ILInterlockedMemoryBarrier();
}

long _ILThreadGetNumThreads()
{
	return numThreads;
}

void ILThreadGetCounts(unsigned long *numForeground,
					   unsigned long *numBackground)
{
	_ILCriticalSectionEnter(&threadLockAll);
	*numForeground = (unsigned long)(numThreads - numBackgroundThreads);
	*numBackground = (unsigned long)(numBackgroundThreads);
	_ILCriticalSectionLeave(&threadLockAll);
}

void ILThreadYield()
{
	_ILThreadYield();
}

/*
 * Enter the "wait/sleep/join" state on the current thread.
 */
int _ILThreadEnterWaitState(ILThread *thread)
{
	int result = IL_THREAD_OK;
	_ILThreadState threadState;

	/*
	 * Set the wait/sleep/join state so that it will be seen by a possible
	 * call to ILThreadAbort by an other thread.
	 */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	threadState.split.priv |= IL_TS_WAIT_SLEEP_JOIN;
	ILInterlockedStoreU2_Acquire(&(thread->state.split.priv),
								 threadState.split.priv);
	/* Requery the thread state */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.pub & IL_TS_ABORT_REQUESTED) != 0)
	{
		_ILCriticalSectionEnter(&(thread->lock));

		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
		if((threadState.split.pub & (IL_TS_ABORT_REQUESTED | IL_TS_INTERRUPTED)) != 0)
		{
			/* Clear the wait/sleep/join state and interrupted flag */
			threadState.split.priv |= IL_TS_WAIT_SLEEP_JOIN;
			threadState.split.pub &= ~(IL_TS_INTERRUPTED);
			ILInterlockedStoreU4(&(thread->state.comb), threadState.comb);

			result = IL_THREAD_ERR_ABORTED;
		}
		_ILCriticalSectionLeave(&(thread->lock));
	}

	return result;
}

/*
 * Leave the "wait/sleep/join" state on the current thread.
 */
int _ILThreadLeaveWaitState(ILThread *thread, int result)
{
	_ILThreadState threadState;

	/*
	 * Clear the wait/sleep/join flag before checking for possible interrupt,
	 * abort or suspend requests.
	 */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	threadState.split.priv &= ~IL_TS_WAIT_SLEEP_JOIN;
	ILInterlockedStoreU2_Acquire(&(thread->state.split.priv),
								 threadState.split.priv);
	/* And requery the thread state */
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.pub & (IL_TS_ABORT_REQUESTED |
								 IL_TS_INTERRUPTED |
								 IL_TS_SUSPEND_REQUESTED)) != 0)
	{
		_ILCriticalSectionEnter(&(thread->lock));

		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	
		/* Abort has more priority over interrupt */
		if((threadState.split.pub & (IL_TS_ABORT_REQUESTED)) != 0)
		{
			threadState.split.pub &= ~(IL_TS_INTERRUPTED);
			_ILWakeupCancelInterrupt(&(thread->wakeup));
			result = IL_THREAD_ERR_ABORTED;
		}
		else if((threadState.split.pub & IL_TS_INTERRUPTED) != 0)
		{
			threadState.split.pub &= ~(IL_TS_INTERRUPTED);
			_ILWakeupCancelInterrupt(&(thread->wakeup));
			result = IL_THREAD_ERR_INTERRUPT;
		}

		if((threadState.split.pub & IL_TS_SUSPEND_REQUESTED) != 0)
		{
			threadState.split.pub &= ~IL_TS_SUSPEND_REQUESTED;
			threadState.split.pub |= (IL_TS_SUSPENDED | IL_TS_SUSPENDED_SELF);

			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);

			/* Unlock the thread object prior to suspending */
			_ILCriticalSectionLeave(&(thread->lock));

			/* Suspend until we receive notification from another thread */
			_ILThreadSuspendSelf(thread);
		}
		else
		{
			ILInterlockedStoreU2(&(thread->state.split.pub),
								 threadState.split.pub);
			_ILCriticalSectionLeave(&(thread->lock));
		}
		return result;
	}

	return result;
}

int ILThreadSleep(ILUInt32 ms)
{
	ILThread *thread = _ILThreadGetSelf();
	int result;
	int waitStateResult;

	if((result = _ILThreadEnterWaitState(thread)) != IL_THREAD_OK)
	{
		return result;
	}

	result = _ILThreadSleep(ms);

	waitStateResult = _ILThreadLeaveWaitState(thread, result);
	if(waitStateResult != IL_THREAD_OK)
	{
		result = waitStateResult;
	}
	return result;
}

void ILThreadWaitForForegroundThreads(int timeout)
{
#ifdef IL_NO_THREADS
	/* Nothing to do */
#else
	_ILWaitOneBackupInterruptsAndAborts(noFgThreadsEvent, timeout);
#endif
}

int ILThreadRegisterCleanup(ILThread *thread, ILThreadCleanupFunc func)
{
	ILThreadCleanupEntry *entry;
	_ILThreadState threadState;

	IL_THREAD_ASSERT((thread->state.split.pub & IL_TS_UNSTARTED) ||
					 (thread == _ILThreadGetSelf()));

	/* Lock down the thread */
	_ILCriticalSectionEnter(&(thread->lock));
	
	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.priv & IL_TS_STOPPED))
	{
		/* Thread has stopped */

		_ILCriticalSectionLeave(&(thread->lock));

		return -1;
	}

	entry = thread->firstCleanupEntry;

	while(entry)
	{
		if(entry->cleanup == func)
		{
			/* Function already registered */

			_ILCriticalSectionLeave(&(thread->lock));

			return -1;
		}
	}

	if((entry = (ILThreadCleanupEntry *)ILMalloc(sizeof(ILThreadCleanupEntry))) == 0)
	{
		/* Out of memory */

		_ILCriticalSectionLeave(&(thread->lock));

		return -1;
	}

	entry->cleanup = func;
	entry->next = 0;

	/* Add the entry to the end up the cleanup list */

	if(thread->lastCleanupEntry)
	{
		thread->lastCleanupEntry->next = entry;
		thread->lastCleanupEntry = entry;
	}
	else
	{
		thread->firstCleanupEntry = thread->lastCleanupEntry = entry;
	}

	_ILCriticalSectionLeave(&(thread->lock));
	
	return 0;
}

int ILThreadUnregisterCleanup(ILThread *thread, ILThreadCleanupFunc func)
{
	ILThreadCleanupEntry *entry, *prev;
	_ILThreadState threadState;

	/* Lock down the thread */
	_ILCriticalSectionEnter(&(thread->lock));

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if((threadState.split.priv & IL_TS_STOPPED))
	{
		/* Thread has stopped */

		_ILCriticalSectionLeave(&(thread->lock));

		return -1;
	}

	/* Walk the list and remove the cleanup function */

	prev = 0;
	entry = thread->firstCleanupEntry;

	while(entry)
	{
		if(entry->cleanup == func)
		{
			/* Remove the entry from the list */

			if(prev)
			{
				/* Entry is in the tail of the list */

				prev->next = entry->next;

				if(prev->next == 0)
				{
					thread->lastCleanupEntry = prev;
				}

				ILFree(entry);
			}
			else
			{
				/* Entry is in the head of the list */

				thread->firstCleanupEntry = entry->next;
				
				if(thread->firstCleanupEntry == 0)
				{
					thread->lastCleanupEntry = 0;
				}
			}

			_ILCriticalSectionLeave(&(thread->lock));

			/* Found and removed */

			return 0;
		}
	}

	_ILCriticalSectionLeave(&(thread->lock));

	/* Not found */

	return -1;
}

void ILThreadSetPriority(ILThread *thread, int priority)
{
	_ILThreadSetPriority(thread, priority);
}

int ILThreadGetPriority(ILThread *thread)
{
	return _ILThreadGetPriority(thread);
}

void _ILThreadSuspendRequest(ILThread *thread)
{
	ILUInt16 threadState;

	IL_THREAD_ASSERT(thread == _ILThreadGetSelf());

	_ILCriticalSectionEnter(&(thread->lock));

	threadState = ILInterlockedLoadU2(&(thread->state.split.pub));
	/* Clear the "suspendRequested" flag and set the suspended flags */
	threadState &= ~IL_TS_SUSPEND_REQUESTED;
	threadState |= (IL_TS_SUSPENDED | IL_TS_SUSPENDED_SELF);
	ILInterlockedStoreU2(&(thread->state.split.pub), threadState);
	thread->suspendRequested = 0;

	/* This implies a memory barrier */
	_ILCriticalSectionLeave(&(thread->lock));

	/* Signal the thread that wanted to make us suspend that we are */
	_ILSemaphorePost(&(thread->suspendAck));

	/* Suspend the current thread until we receive a resume signal */
	_ILThreadSuspendSelf(thread);
}

/*
 * Clear stack space.
 */
void ILThreadClearStack(int length)
{
	char *ptr;

#ifdef HAVE_ALLOCA
#ifdef _MSC_VER
	ptr = _alloca(length);
#else
	ptr = alloca(length);
#endif
#else
	char stackBuffer[4096];
	length = 4096;
	ptr = &stackBuffer[0];
#endif

	if (ptr)
	{
		ILMemZero(ptr, length);
	}
}

#ifdef	__cplusplus
};
#endif
