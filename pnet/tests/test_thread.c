/*
 * test_thread.c - Test the thread routines in "support".
 *
 * Copyright (C) 2002, 2009, 2010  Southern Storm Software, Pty Ltd.
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

#include "ilunit.h"
#include "../support/thr_defs.h"
#include "../support/interlocked.h"
#include "../support/interlocked_slist.h"
#include "il_thread.h"
#include "il_gc.h"
#if HAVE_UNISTD_H
	#include <unistd.h>
#endif
#ifdef IL_WIN32_NATIVE
	#include <windows.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Put the current thread to sleep for a number of "time steps".
 */
static void sleepFor(int steps)
{
#ifdef HAVE_USLEEP
	/* Time steps are 100ms in length */
	usleep(steps * 100000);
#define	STEPS_TO_MS(steps)	(steps * 100)
#else
#ifdef IL_WIN32_NATIVE
	/* Time steps are 100ms in length */
	Sleep(steps * 100);
#define	STEPS_TO_MS(steps)	(steps * 100)
#else
	/* Time steps are 1s in length */
	sleep(steps);
#define	STEPS_TO_MS(steps)	(steps * 1000)
#endif
#endif
}

/*
 * Test that the descriptor for the main thread is not NULL.
 */
static void thread_main_nonnull(void *arg)
{
	if(!ILThreadSelf())
	{
		ILUnitFailed("main thread is null");
	}
}

/*
 * Test setting and getting the object on the main thread.
 */
static void thread_main_object(void *arg)
{
	ILThread *thread = ILThreadSelf();

	/* The value should be NULL initially */
	if(ILThreadGetObject(thread))
	{
		ILUnitFailed("object for main thread not initially NULL");
	}

	/* Change the value to something else and then check it */
	ILThreadSetObject(thread, (void *)0xBADBEEF);
	if(ILThreadGetObject(thread) != (void *)0xBADBEEF)
	{
		ILUnitFailed("object for main thread could not be changed");
	}
}

/*
 * Test that the "main" thread is initially in the running state.
 */
static void thread_main_running(void *arg)
{
	if(ILThreadGetState(ILThreadSelf()) != IL_TS_RUNNING)
	{
		ILUnitFailed("main thread is not running");
	}
}

/*
 * Test that the "main" thread is initially a foreground thread.
 */
static void thread_main_foreground(void *arg)
{
	if(ILThreadGetBackground(ILThreadSelf()))
	{
		ILUnitFailed("main thread is not a foreground thread");
	}
	if((ILThreadGetState(ILThreadSelf()) & IL_TS_BACKGROUND) != 0)
	{
		ILUnitFailed("main thread state is not consistent "
					 "with background flag");
	}
}

/*
 * Global flag that may be modified by started threads.
 */
static int volatile globalFlag;

/*
 * Thread start function that checks that its argument is set correctly.
 */
static void checkValue(void *arg)
{
	globalFlag = (arg == (void *)0xBADBEEF);
}

/*
 * Thread start function that sets a global flag.
 */
static void setFlag(void *arg)
{
	globalFlag = 1;
}

/*
 * Test that when a thread is created, it has the correct argument.
 */
static void thread_create_arg(void *arg)
{
	ILThread *thread;

	/* Set the global flag to "not modified" */
	globalFlag = -1;

	/* Create the thread */
	thread = ILThreadCreate(checkValue, (void *)0xBADBEEF);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Start the thread running */
	ILThreadStart(thread);

	/* Wait for the thread to finish */
	ILThreadJoin(thread, 10000);

	/* Destroy the thread object */
	ILThreadDestroy(thread);
	
	/* Determine if the test was successful or not */
	if(globalFlag == -1)
	{
		ILUnitFailed("thread start function was never called");
	}
	else if(!globalFlag)
	{
		ILUnitFailed("wrong value passed to thread start function");
	}
}

/*
 * Test that when a thread is created, it is initially suspended.
 */
static void thread_create_suspended(void *arg)
{
	ILThread *thread;
	int savedFlag1;
	int savedFlag2;

	/* Clear the global flag */
	globalFlag = 0;

	/* Create the thread */
	thread = ILThreadCreate(setFlag, 0);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Wait for the thread to get settled */
	sleepFor(1);

	/* Save the current state of the flag */
	savedFlag1 = globalFlag;

	/* Start the thread running */
	ILThreadStart(thread);

	/* Wait for the thread to exit */
	sleepFor(1);

	/* Get the new flag state */
	savedFlag2 = globalFlag;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Determine if the test was successful or not */
	if(savedFlag1)
	{
		ILUnitFailed("thread did not suspend on creation");
	}
	if(!savedFlag2)
	{
		ILUnitFailed("thread did not unsuspend after creation");
	}
}

/*
 * A thread procedure that sleeps for a number of time steps.
 */
static void sleepThread(void *arg)
{
	sleepFor((int)(ILNativeInt)arg);
}

/*
 * Test that when a thread is created, it is initially in
 * the "unstarted" state, and then transitions to the
 * "running" state, and finally to the "stopped" state.
 */
static void thread_create_state(void *arg)
{
	ILThread *thread;
	int state1;
	int state2;
	int state3;

	/* Create the thread */
	thread = ILThreadCreate(sleepThread, (void *)2);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Get the thread's state (should be "unstarted") */
	state1 = ILThreadGetState(thread);

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 1 time step and then get the state again */
	sleepFor(1);
	state2 = ILThreadGetState(thread);

	/* Wait for the thread to exit */
	ILThreadJoin(thread, 10000);
	state3 = ILThreadGetState(thread);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(state1 != IL_TS_UNSTARTED)
	{
		ILUnitFailed("thread did not begin in the `unstarted' state");
	}
	if(state2 != IL_TS_RUNNING)
	{
		ILUnitFailed("thread did not change to the `running' state");
	}
	if(state3 != IL_TS_STOPPED)
	{
		ILUnitFailed("thread did not end in the `stopped' state");
	}
}

/*
 * Test that we can destroy a newly created thread that isn't started yet.
 */
static void thread_create_destroy(void *arg)
{
	ILThread *thread;

	/* Create the thread */
	thread = ILThreadCreate(sleepThread, (void *)4);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Wait 1 time step to let the system settle */
	sleepFor(1);

	/* Destroy the thread */
	ILThreadDestroy(thread);
}

/*
 * Test that new threads are created as "foreground" threads.
 */
static void thread_create_foreground(void *arg)
{
	ILThread *thread;
	int isbg;
	int state;

	/* Create the thread */
	thread = ILThreadCreate(sleepThread, (void *)4);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Check the background flag and the state for the thread */
	isbg = ILThreadGetBackground(thread);
	state = ILThreadGetState(thread);

	/* Destroy the thread */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(isbg)
	{
		ILUnitFailed("new thread was not created as `foreground'");
	}
	if((state & IL_TS_BACKGROUND) != 0)
	{
		ILUnitFailed("new thread state is not consistent with background flag");
	}
}

/*
 * Test that we can suspend a running thread.
 */
static void thread_suspend(void *arg)
{
	ILThread *thread;
	int state1;
	int state2;

	/* Create the thread */
	thread = ILThreadCreate(sleepThread, (void *)4);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 1 time step and then suspend it */
	sleepFor(1);
	if(!ILThreadSuspend(thread))
	{
		ILUnitFailed("ILThreadSuspend returned zero");
	}

	/* Wait 4 more time steps - the thread will exit if
	   it was not properly suspended */
	sleepFor(4);

	/* Get the thread's current state (which should be "suspended") */
	state1 = ILThreadGetState(thread);

	/* Resume the thread to allow it to exit normally */
	ILThreadResume(thread);

	/* Wait another time step: the "sleepFor" in the thread
	   has now expired and should terminate the thread */
	sleepFor(1);

	/* Get the current state (which should be "stopped") */
	state2 = ILThreadGetState(thread);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(state1 != IL_TS_SUSPENDED)
	{
		ILUnitFailed("thread did not suspend when requested");
	}
	if(state2 != IL_TS_STOPPED)
	{
		ILUnitFailed("thread did not end in the `stopped' state");
	}
}

/*
 * A thread that suspends itself.
 */
static void suspendThread(void *arg)
{
	ILThreadSuspend(ILThreadSelf());
	sleepFor(2);
}

/*
 * Test that we can resume a thread that has suspended itself.
 */
static void thread_suspend_self(void *arg)
{
	ILThread *thread;
	int state1;
	int state2;
	int state3;

	/* Create the thread */
	thread = ILThreadCreate(suspendThread, 0);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Start the thread, which should immediately suspend */
	ILThreadStart(thread);

	/* Wait 4 time steps - if the suspend is ignored, the
	   thread will run to completion */
	sleepFor(4);

	/* Get the thread's current state (which should be "suspended") */
	state1 = ILThreadGetState(thread);

	/* Resume the thread to allow it to exit normally */
	ILThreadResume(thread);

	/* Wait another time step to allow the thread to resume */
	sleepFor(1);

	/* Get the current state (which should be "running") */
	state2 = ILThreadGetState(thread);

	/* Wait two more time steps to allow the thread to terminate */
	sleepFor(2);

	/* Get the current state (which should be "stopped") */
	state3 = ILThreadGetState(thread);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(state1 != IL_TS_SUSPENDED)
	{
		ILUnitFailed("thread did not suspend itself");
	}
	if(state2 != IL_TS_RUNNING)
	{
		ILUnitFailed("thread did not resume when requested");
	}
	if(state3 != IL_TS_STOPPED)
	{
		ILUnitFailed("thread did not end in the `stopped' state");
	}
}

/*
 * Test that we can destroy a suspended thread.
 */
static void thread_suspend_destroy(void *arg)
{
	ILThread *thread;

	/* Create the thread */
	thread = ILThreadCreate(suspendThread, 0);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Start the thread, which should immediately suspend */
	ILThreadStart(thread);

	/* Wait 1 time step to let the system settle */
	sleepFor(1);

	/* Abort the thread */
	ILThreadAbort(thread);
	
	/* Destroy the thread */
	ILThreadDestroy(thread);
}

/*
 * Thread start function that holds a mutex for a period of time.
 */
static void mutexHold(void *arg)
{
	ILMutex *mutex = ILMutexCreate();
	ILMutexLock(mutex);
	sleepFor(2);
	globalFlag = 1;
	ILMutexUnlock(mutex);
	ILMutexDestroy(mutex);
	sleepFor(2);
}

/*
 * Test that a thread cannot be suspended while it holds
 * a mutex, but that it will suspend as soon as it gives
 * up the mutex.
 */
static void thread_suspend_mutex(void *arg)
{
	ILThread *thread;
	int savedFlag;

	/* Create the thread */
	thread = ILThreadCreate(mutexHold, 0);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flag */
	globalFlag = 0;

	/* Start the thread, which should immediately suspend */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Attempt to suspend the thread: this should block for 1 time step */
	ILThreadSuspend(thread);

	/* Save the global flag at this point */
	savedFlag = globalFlag;

	/* Resume the thread */
	ILThreadResume(thread);

	/* Wait 4 more time steps for the thread to exit */
	sleepFor(4);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors: the flag must have been set */
	if(!savedFlag)
	{
		ILUnitFailed("thread suspended while holding a mutex");
	}
}

/*
 * Thread start function that holds a read lock for a period of time.
 */
static void rwlockHold(void *arg)
{
	ILRWLock *rwlock = ILRWLockCreate();
	if(arg)
	{
		ILRWLockReadLock(rwlock);
	}
	else
	{
		ILRWLockWriteLock(rwlock);
	}
	sleepFor(2);
	globalFlag = 1;
	ILRWLockUnlock(rwlock);
	ILRWLockDestroy(rwlock);
	sleepFor(2);
}

/*
 * Test that a thread cannot be suspended while it holds
 * a read/write lock, but that it will suspend as soon as
 * it gives up the lock.  Read lock if arg != 0, and write
 * lock otherwise.
 */
static void thread_suspend_rwlock(void *arg)
{
	ILThread *thread;
	int savedFlag;

	/* Create the thread */
	thread = ILThreadCreate(rwlockHold, arg);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flag */
	globalFlag = 0;

	/* Start the thread, which should immediately suspend */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Attempt to suspend the thread: this should block for 1 time step */
	ILThreadSuspend(thread);

	/* Save the global flag at this point */
	savedFlag = globalFlag;

	/* Resume the thread */
	ILThreadResume(thread);

	/* Wait 4 more time steps for the thread to exit */
	sleepFor(4);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors: the flag must have been set */
	if(!savedFlag)
	{
		ILUnitFailed("thread suspended while holding the lock");
	}
}

static int volatile mainWasSuspended;

/*
 * Thread start function that suspends the main thread.
 */
static void suspendMainThread(void *arg)
{
	ILThread *thread = (ILThread *)arg;

	/* Wait 1 time step and then suspend the main thread */
	sleepFor(1);
	ILThreadSuspend(thread);

	/* Wait 4 more time steps - the main thread will continue if
	   it was not properly suspended */
	sleepFor(4);

	/* Determine if the main thread was suspended */
	mainWasSuspended = (ILThreadGetState(thread) == IL_TS_SUSPENDED);

	/* Resume the main thread to allow it to continue normally */
	ILThreadResume(thread);
}

/*
 * Test that the main thread can be suspended by another thread.
 */
static void thread_suspend_main(void *arg)
{
	ILThread *thread;
	int flag;
	int state;

	/* Create the thread that will attempt to suspend us */
	thread = ILThreadCreate(suspendMainThread, ILThreadSelf());
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the "mainWasSuspended" flag */
	mainWasSuspended = 0;

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 2 time steps - main should be suspended during this time */
	sleepFor(2);

	/* Save the value of "mainWasSuspended".  This will be zero if
	   the main thread was not properly suspended */
	flag = mainWasSuspended;

	/* Wait 5 extra time steps for the system to settle */
	sleepFor(5);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* The main thread should now be in the "running" state */
	state = ILThreadGetState(ILThreadSelf());

	/* Check for errors */
	if(!flag)
	{
		ILUnitFailed("the main thread was not suspended");
	}
	if(state != IL_TS_RUNNING)
	{
		ILUnitFailed("the main thread did not return to the `running' state");
	}
}

/*
 * Thread start function that resumes the main thread.
 */
static void resumeMainThread(void *arg)
{
	ILThread *thread = (ILThread *)arg;

	/* Wait 4 time steps - if the suspend is ignored, the
	   main thread will continue running */
	sleepFor(4);

	/* Determine if the main thread is suspended */
	mainWasSuspended = (ILThreadGetState(thread) == IL_TS_SUSPENDED);

	/* Resume the main thread to allow it to continue execution */
	ILThreadResume(thread);
}

/*
 * Test that we can resume the main thread after it has suspended itself.
 */
static void thread_suspend_main_self(void *arg)
{
	ILThread *thread;
	int flag;
	int state;

	/* Create the thread */
	thread = ILThreadCreate(resumeMainThread, ILThreadSelf());
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the "mainWasSuspended" flag */
	mainWasSuspended = 0;

	/* Start the thread */
	ILThreadStart(thread);

	/* Suspend the main thread and sleep for 2 time steps */
	ILThreadSuspend(ILThreadSelf());
	sleepFor(2);

	/* Save the "mainWasSuspended" flag, which will be zero if
	   the main thread didn't really suspend */
	flag = mainWasSuspended;

	/* Wait 3 extra time steps for the resumption thread to exit */
	sleepFor(3);

	/* Get the main thread's current state (which should be "running") */
	state = ILThreadGetState(ILThreadSelf());

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(!flag)
	{
		ILUnitFailed("the main thread did not suspend itself");
	}
	if(state != IL_TS_RUNNING)
	{
		ILUnitFailed("the main thread did not return to the `running' state");
	}
}

/*
 * Flags that are used by "sleepILThread".
 */
static int volatile sleepResult;
static int volatile sleepDone;

/*
 * A thread procedure that sleeps for a number of time steps
 * using the "ILThreadSleep" function.
 */
static void sleepILThread(void *arg)
{
	sleepResult = ILThreadSleep((ILUInt32)STEPS_TO_MS((int)(ILNativeInt)arg));
	sleepDone = 1;
}

/*
 * Test thread sleep functionality.
 */
static void thread_sleep(void *arg)
{
	ILThread *thread;
	int done1;

	/* Create the thread */
	thread = ILThreadCreate(sleepILThread, (void *)2);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flags */
	sleepResult = 0;
	sleepDone = 0;

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* The sleep should not be done yet */
	done1 = sleepDone;

	/* Wait 2 more time steps for the thread to exit */
	sleepFor(2);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(done1)
	{
		ILUnitFailed("thread did not sleep for the required amount of time");
	}
	if(!sleepDone)
	{
		ILUnitFailed("sleep did not end when expected");
	}
	if(sleepResult != IL_THREAD_OK)
	{
		if(sleepResult == IL_THREAD_ERR_INTERRUPT)
		{
			ILUnitFailed("sleep was interrupted");
		}
		else if(sleepResult == IL_THREAD_ERR_ABORTED)
		{
			ILUnitFailed("thread was aborted");
		}
		else
		{
			ILUnitFailed("sleep failed");
		}
	}
}

/*
 * Test thread sleep interrupt functionality.
 */
static void thread_sleep_interrupt(void *arg)
{
	ILThread *thread;
	int done1;
	int result1;

	/* Create the thread */
	thread = ILThreadCreate(sleepILThread, (void *)3);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flags */
	sleepResult = 0;
	sleepDone = 0;

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Interrupt the thread */
	ILThreadInterrupt(thread);

	/* Wait 1 time step for the interrupt to be processed */
	sleepFor(1);

	/* Check that the sleep is done */
	done1 = sleepDone;
	result1 = sleepResult;

	/* Wait 2 more time steps for the thread to exit */
	sleepFor(2);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(!done1)
	{
		ILUnitFailed("sleep was not interrupted");
	}
	if(sleepResult != IL_THREAD_ERR_INTERRUPT)
	{
		ILUnitFailed("sleep should have returned IL_THREAD_ERR_INTERRUPT");
	}
}

/*
 * Test thread sleep suspend functionality.
 */
static void thread_sleep_suspend(void *arg)
{
	ILThread *thread;
	int done1;
	int done2;

	/* Create the thread */
	thread = ILThreadCreate(sleepILThread, (void *)2);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flags */
	sleepResult = 0;
	sleepDone = 0;

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Suspend the thread.  The suspend will wait until the sleep finishes */
	ILThreadSuspend(thread);

	/* Wait 2 time steps for the sleep to finish */
	sleepFor(2);

	/* The "sleepDone" flag should not be set yet, because the
	   thread has been suspended before it can be set */
	done1 = sleepDone;

	/* Resume the thread */
	ILThreadResume(thread);

	/* Wait 1 more time step for the thread to exit */
	sleepFor(1);

	/* The "sleepDone" flag should now be set */
	done2 = sleepDone;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(done1)
	{
		ILUnitFailed("thread did not suspend after the sleep");
	}
	if(!done2)
	{
		ILUnitFailed("thread did not resume");
	}
}

/*
 * Test thread sleep suspend functionality, when the suspend is
 * resumed before the sleep finishes.
 */
static void thread_sleep_suspend_ignore(void *arg)
{
	ILThread *thread;
	int done1;

	/* Create the thread */
	thread = ILThreadCreate(sleepILThread, (void *)3);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flags */
	sleepResult = 0;
	sleepDone = 0;

	/* Start the thread */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Suspend the thread.  The suspend will not be processed just yet */
	ILThreadSuspend(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Resume the thread */
	ILThreadResume(thread);

	/* Wait 2 more time steps for the thread to exit */
	sleepFor(2);

	/* The "sleepDone" flag should now be set */
	done1 = sleepDone;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(!done1)
	{
		ILUnitFailed("resume was not ignored");
	}
}

/*
 * Flags that are used by "checkGetSetValue".
 */
static int volatile correctFlag1;
static int volatile correctFlag2;
static int volatile correctFlag3;

/*
 * Thread start function that checks that its argument is set correctly,
 * and then changes the object to something else.
 */
static void checkGetSetValue(void *arg)
{
	ILThread *thread = ILThreadSelf();

	/* Check that the argument and thread object are 0xBADBEEF3 */
	correctFlag1 = (arg == (void *)0xBADBEEF1);
	correctFlag2 = (ILThreadGetObject(thread) == 0xBADBEEF3);

	/* Change the object to 0xBADBEEF4 and re-test */
	ILThreadSetObject(thread, (void *)0xBADBEEF4);
	correctFlag3 = (ILThreadGetObject(thread) == (void *)0xBADBEEF4);
}

/*
 * Test setting and getting the object on some other thread.
 */
static void thread_other_object(void *arg)
{
	ILThread *thread;
	int correct1;
	int correct2;
	int correct3;

	/* Create the thread */
	thread = ILThreadCreate(checkGetSetValue, (void *)0xBADBEEF1);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Get the current object, which should be 0 */
	correct1 = (ILThreadGetObject(thread) == 0);

	/* Change the object to 0xBADBEEF2 and check */
	ILThreadSetObject(thread, (void *)0xBADBEEF2);
	correct2 = (ILThreadGetObject(thread) == (void *)0xBADBEEF2);

	/* Change the object to 0xBADBEEF3 */
	ILThreadSetObject(thread, (void *)0xBADBEEF3);

	/* Start the thread, which checks to see if its argument is 0xBADBEEF3 */
	ILThreadStart(thread);

	/* Wait 1 time step for the thread to exit */
	sleepFor(1);

	/* Check that the final object value is 0xBADBEEF4 */
	correct3 = (ILThreadGetObject(thread) == (void *)0xBADBEEF4);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors */
	if(!correct1)
	{
		ILUnitFailed("initial object not set correctly");
	}
	if(!correct2)
	{
		ILUnitFailed("object could not be changed by main thread");
	}
	if(!correctFlag1)
	{
		ILUnitFailed("thread start function got wrong argument");
	}
	if(!correctFlag2)
	{
		ILUnitFailed("thread object not set properly");
	}
	if(!correctFlag3)
	{
		ILUnitFailed("could not change object in thread start function");
	}
	if(!correct3)
	{
		ILUnitFailed("final object value incorrect");
	}
}

/*
 * Make sure that the thread counts have returned to the correct values.
 * This indirectly validates that the "thread_create_destroy" and
 * "thread_suspend_destroy" tests updated the thread counts correctly.
 * It also validates that normal thread exits update the thread counts
 * correctly.
 */
static void thread_counts(void *arg)
{
	unsigned long numForeground;
	unsigned long numBackground;
	ILThreadGetCounts(&numForeground, &numBackground);
	if(numForeground != 1)
	{
		ILUnitFailed("foreground thread count has not returned to 1");
	}
	if(numBackground > 1)
	{
		/* The GC thread doesn't start until needed so numBackground can be 0 */
		/* Currently there is one background thread (the finalizer thread) */
		ILUnitFailed("background thread count has not returned to 0 or 1");
	}
}

static void interlocked_load(void *arg)
{
	volatile ILInt32 value;
	void * volatile ptrValue;
	int haveError;

	haveError = 0;
	value = 1;
	if(ILInterlockedLoadI4(&value) != 1)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedLoadI4");
	}
	if(ILInterlockedLoadI4_Acquire(&value) != 1)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedLoadI4_Acquire");
	}
	ptrValue = (void *)1;
	if(ILInterlockedLoadP(&ptrValue) != (void *)1)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedLoadP");
	}
	if(ILInterlockedLoadP_Acquire(&ptrValue) != (void *)1)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedLoadP_Acquire");
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_store(void *arg)
{
	volatile ILInt32 value;
	void * volatile ptrValue;
	int haveError;

	haveError = 0;
	value = 1;
	ILInterlockedStoreI4(&value, 2);
	if(value != 2)
	{
		haveError = 1;
		ILUnitFailMessage("ILInterlockedStoreI4 doesn't store the correct value");
	}
	ILInterlockedStoreI4_Release(&value, 3);
	if(value != 3)
	{
		haveError = 1;
		ILUnitFailMessage("ILInterlockedStoreI4_Release doesn't store the correct value");
	}
	ptrValue = (void *)1;
	ILInterlockedStoreP(&ptrValue, (void *)2);
	if(ptrValue != (void *)2)
	{
		haveError = 1;
		ILUnitFailMessage("ILInterlockedStoreP doesn't store the correct value");
	}
	ILInterlockedStoreP_Release(&ptrValue, (void *)3);
	if(ptrValue != (void *)3)
	{
		haveError = 1;
		ILUnitFailMessage("ILInterlockedStoreP_Release doesn't store the correct value");
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_exchange(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;
	int haveError;

	haveError = 0;
	value = 0;
	result = ILInterlockedExchangeI4(&value, 5);
	if(result != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedExchangeI4");
		ILUnitFailMessage("Expected 0 but was %i", result);
	}
	if(value != 5)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect value set in ILInterlockedExchangeI4");
		ILUnitFailMessage("Expected 5 but was %i", value);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_exchange_pointers(void *arg)
{
	void * volatile value;
	void * result;
	int haveError;

	haveError = 0;
	value = 0;
	result = ILInterlockedExchangeP(&value, (void *)5);
	if(result != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedExchangeP");
		ILUnitFailMessage("Expected 0 but was %p", result);
	}
	if(value != (void *)5)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect value set in ILInterlockedExchangeP");
		ILUnitFailMessage("Expected 5 but was %p", value);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_compare_and_exchange(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;
	int haveError;

	haveError = 0;
	value = 0;
	result = ILInterlockedCompareAndExchangeI4(&value, 5, 0);
	if(result != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedCompareAndExchangeI4");
		ILUnitFailMessage("Expected 0 but was %i", result);
	}
	if(value != 5)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect value set in ILInterlockedCompareAndExchangeI4");
		ILUnitFailMessage("Expected 5 but was %i", value);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_compare_and_exchange_fail(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;
	int haveError;

	haveError = 0;
	value = 0;
	result = ILInterlockedCompareAndExchangeI4(&value, 5, 1);
	if(result != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedCompareAndExchangeI4");
		ILUnitFailMessage("Expected 0 but was %i", result);
	}
	if(value != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect value set in ILInterlockedCompareAndExchangeI4");
		ILUnitFailMessage("Expected 0 but was %i", value);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_compare_and_exchange_pointers(void *arg)
{
	void * volatile value;
	void * result;
	int haveError;

	haveError = 0;
	value = 0;
	result = ILInterlockedCompareAndExchangeP(&value, (void *)5, (void *)0);
	if(result != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedCompareAndExchangeP");
		ILUnitFailMessage("Expected 0 but was %p", result);
	}
	if(value != (void *)5)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect value set in ILInterlockedCompareAndExchangeP");
		ILUnitFailMessage("Expected 5 but was %p", value);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_compare_and_exchange_pointers_fail(void *arg)
{
	void * volatile value;
	void * result;
	int haveError;

	haveError = 0;
	value = 0;
	result = ILInterlockedCompareAndExchangeP(&value, (void *)5, (void *)1);
	if(result != 0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect return value of ILInterlockedCompareAndExchangeP");
		ILUnitFailMessage("Expected 0 but was %p", result);
	}
	if(value != (void *)0)
	{
		haveError = 1;
		ILUnitFailMessage("Incorrect value set in ILInterlockedCompareAndExchangeP");
		ILUnitFailMessage("Expected 5 but was %p", value);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void interlocked_increment(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;

	value = 0;
	result = ILInterlockedIncrementI4(&value);
	if(result != 1)
	{
		ILUnitFailed("ILInterlockedIncrementI4 returned the wrong result.\n"
					 "Expected: 1 bus was %i\n", result);
	}
	ILUnitAssert(value == 1);
}

static void interlocked_decrement(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;

	value = 1;
	result = ILInterlockedDecrementI4(&value);
	if(result != 0)
	{
		ILUnitFailed("ILInterlockedDecrementI4 returned the wrong result.\n"
					 "Expected: 0 bus was %i\n", result);
	}
	ILUnitAssert(value == 0);
}

static void interlocked_add(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;

	value = 0;
	result = ILInterlockedAddI4(&value, 5);
	if(result != 5)
	{
		ILUnitFailed("ILInterlockedAddI4 returned the wrong result.\n"
					 "Expected: 5 bus was %i\n", result);
	}
	ILUnitAssert(value == 5);
}

static void interlocked_sub(void *arg)
{
	volatile ILInt32 value;
	ILInt32 result;

	value = 10;
	result = ILInterlockedSubI4(&value, 10);
	if(result != 0)
	{
		ILUnitFailed("ILInterlockedSubI4 returned the wrong result.\n"
					 "Expected: 0 bus was %i\n", result);
	}
	ILUnitAssert(value == 0);
}

#define _TEST_START_VALUE	0xAAAAAAAA
#define _TEST_AND_MASK		0x22222222
#define _TEST_OR_MASK		0x55555555
#define _TEST_AND_RESULT	(_TEST_START_VALUE & _TEST_AND_MASK)
#define _TEST_OR_RESULT		(_TEST_START_VALUE | _TEST_OR_MASK)

static void interlocked_and(void *arg)
{
	volatile ILUInt32 value;

	value = _TEST_START_VALUE;
	ILInterlockedAndU4(&value, _TEST_AND_MASK);
	ILUnitAssert(value == _TEST_AND_RESULT);
}

static void interlocked_or(void *arg)
{
	volatile ILUInt32 value;

	value = _TEST_START_VALUE;
	ILInterlockedOrU4(&value, _TEST_OR_MASK);
	ILUnitAssert(value == _TEST_OR_RESULT);
}

static void _interlocked_thrash_incdec(void *arg)
{
	volatile ILInt32 *ref;
	int counter;

	ref = (ILInt32 *)arg;
	for(counter = 0; counter < 1000000; ++counter)
	{
		ILInterlockedIncrementI4_Acquire(ref);
		ILInterlockedDecrementI4_Release(ref);
	}
}

static void _interlocked_thrash_addsub(void *arg)
{
	volatile ILInt32 *ref;
	int counter;

	ref = (ILInt32 *)arg;
	for(counter = 0; counter < 1000000; ++counter)
	{
		ILInterlockedAddI4_Acquire(ref, 10);
		ILInterlockedSubI4_Release(ref, 10);
	}
}

static ILInt32 testValue;

static void interlocked_thrash_incdec(void *arg)
{
	ILThread *thread1;
	ILThread *thread2;
	int rc;
	int haveError;

	haveError = 0;

	/* Create two threads */
	thread1 = ILThreadCreate(_interlocked_thrash_incdec, &testValue);
	if(!thread1)
	{
		ILUnitOutOfMemory();
	}
	thread2 = ILThreadCreate(_interlocked_thrash_incdec, &testValue);
	if(!thread2)
	{
		ILUnitOutOfMemory();
	}

	/* Initialize the test value */
	testValue = 0;

	/* Start the threads */
	ILThreadStart(thread1);
	ILThreadStart(thread2);

	if((rc = ILThreadJoin(thread1, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread1 with returncode %i", rc);
	}
	if((rc = ILThreadJoin(thread2, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread2 with returncode %i", rc);
	}

	ILThreadDestroy(thread1);
	ILThreadDestroy(thread2);

	if(haveError)
	{
		ILUnitFailEndMessages();
	}
	else
	{
		ILUnitAssert(testValue == 0);
	}
}

static void interlocked_thrash_addsub(void *arg)
{
	ILThread *thread1;
	ILThread *thread2;
	int rc;
	int haveError;

	haveError = 0;

	/* Create two threads */
	thread1 = ILThreadCreate(_interlocked_thrash_addsub, &testValue);
	if(!thread1)
	{
		ILUnitOutOfMemory();
	}
	thread2 = ILThreadCreate(_interlocked_thrash_addsub, &testValue);
	if(!thread2)
	{
		ILUnitOutOfMemory();
	}

	/* Initialize the test value */
	testValue = 0;

	/* Start the threads */
	ILThreadStart(thread1);
	ILThreadStart(thread2);

	if((rc = ILThreadJoin(thread1, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread1 with returncode %i", rc);
	}
	if((rc = ILThreadJoin(thread2, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread2 with returncode %i", rc);
	}

	ILThreadDestroy(thread1);
	ILThreadDestroy(thread2);

	if(haveError)
	{
		ILUnitFailEndMessages();
	}
	else
	{
		ILUnitAssert(testValue == 0);
	}
}

static void interlocked_slist_create(void *arg)
{
	ILInterlockedSListHead head;

	ILInterlockedSListHead_Init(&head);

	if(!ILInterlockedSList_IsClean(&head))
	{
		ILUnitFailed("List is not clean after creation.\n");
	}
}

static void interlocked_slist_add_rem_single(void *arg)
{
	ILInterlockedSListHead head;
	ILInterlockedSListElement elem;
	ILInterlockedSListElement *rem_elem;

	ILInterlockedSListHead_Init(&head);

	ILInterlockedSListAppend(&head, &elem);

	rem_elem = (ILInterlockedSListElement *)ILInterlockedSListGet(&head);

	if(rem_elem != &elem)
	{
		ILUnitFailed("The element removed from the list doesn't match the added element.\n");
	}

	if(!ILInterlockedSList_IsClean(&head))
	{
		ILUnitFailed("List is not clean after removing the element.\n");
	}
}

#define SLIST_ITERATIONS 1000000

static void _interlocked_slist_append_thrash(void *arg)
{
	ILInterlockedSListHead *head = (ILInterlockedSListHead *)arg;
	int i;

	for(i = 0; i < SLIST_ITERATIONS; ++i)
	{
		ILInterlockedSListElement *elem;

		elem = (ILInterlockedSListElement *)malloc(sizeof(ILInterlockedSListElement));
		if(elem == 0)
		{
			ILUnitOutOfMemory();
		}
		ILInterlockedSListAppend(head, elem);
	}
}

static void _interlocked_slist_get_thrash(void *arg)
{
	ILInterlockedSListHead *head = (ILInterlockedSListHead *)arg;
	int i;

	i = 0;
	while(i < SLIST_ITERATIONS)
	{
		ILInterlockedSListElement *elem;

		elem = (ILInterlockedSListElement *)ILInterlockedSListGet(head);
		if(elem)
		{
			++i;
			free(elem);
		}
	}
}

static void interlocked_slist_add_rem_thrash(void *arg)
{
	ILInterlockedSListHead head;
	ILThread *thread[8];
	int joinResults[8];
	int i;
	int haveError = 0;

	ILInterlockedSListHead_Init(&head);

	for(i = 0; i < 4; ++i)
	{
		thread[i] = ILThreadCreate(_interlocked_slist_append_thrash, &head);
		if(!thread[i])
		{
			ILUnitOutOfMemory();
		}
	}

	for(i = 4; i < 8; ++i)
	{
		thread[i] = ILThreadCreate(_interlocked_slist_get_thrash, &head);
		if(!thread[i])
		{
			ILUnitOutOfMemory();
		}
	}

	/* Start the threads */
	for(i = 0; i < 4; ++i)
	{
		ILThreadStart(thread[i]);
		ILThreadStart(thread[4 + i]);
	}

	/* Wait for the threads to finish */
	for(i = 0; i < 8; ++i)
	{
		joinResults[i] = ILThreadJoin(thread[i], 30000);
	}

	for(i = 0; i < 8; ++i)
	{
		if(joinResults[i] != IL_JOIN_OK)
		{
			haveError = 1;
			ILUnitFailMessage("Failed to join thread[%i] with returncode %i", i, joinResults[i]);
		}
		/* Destroy the thread object */
		ILThreadDestroy(thread[i]);
	}
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
	else if(!ILInterlockedSList_IsClean(&head))
	{
		ILUnitFailed("List is not clean after running the test.\n");
	}
}

/*
 * Test wait mutex creation.
 */
static void mutex_create(void *arg)
{
	ILWaitHandle *handle;
	ILWaitHandle *handle2;
	int gotOwn;

	/* Create simple mutexes */
	handle = ILWaitMutexCreate(0);
	if(!handle)
	{
		ILUnitFailed("could not create a simple mutex");
	}
	ILWaitHandleClose(handle);
	handle = ILWaitMutexCreate(1);
	if(!handle)
	{
		ILUnitFailed("could not create a simple mutex that is owned");
	}
	ILWaitHandleClose(handle);

	/* Create a named mutex */
	gotOwn = -100;
	handle = ILWaitMutexNamedCreate("aaa", 0, &gotOwn);
	if(!handle)
	{
		ILUnitFailed("could not create a named mutex");
	}
	if(gotOwn == -100)
	{
		ILUnitFailed("gotOwnership was not changed");
	}
	if(gotOwn)
	{
		ILUnitFailed("gotOwnership was set when it should have been cleared");
	}

	/* Create the same named mutex and acquire it */
	gotOwn = -100;
	handle2 = ILWaitMutexNamedCreate("aaa", 1, &gotOwn);
	if(!handle2)
	{
		ILUnitFailed("could not create a named mutex (2)");
	}
	if(handle != handle2)
	{
		ILUnitFailed("did not reacquire the same mutex");
	}
	if(gotOwn == -100)
	{
		ILUnitFailed("gotOwnership was not changed (2)");
	}
	if(!gotOwn)
	{
		ILUnitFailed("gotOwnership was cleared when it "
					 "should have been set (2)");
	}

	/* Close the second copy of the named mutex */
	ILWaitMutexRelease(handle2);
	ILWaitHandleClose(handle2);

	/* Create a named mutex with a different name */
	handle2 = ILWaitMutexNamedCreate("bbb", 0, 0);
	if(!handle2)
	{
		ILUnitFailed("could not create a named mutex (3)");
	}
	if(handle == handle2)
	{
		ILUnitFailed("mutexes with different names have the same handle");
	}

	/* Clean up */
	ILWaitHandleClose(handle);
	ILWaitHandleClose(handle2);
}

/*
 * Test monitor creation.
 */
static void wait_monitor_create(void *arg)
{
	ILWaitHandle *handle;
	handle = ILWaitMonitorCreate();
	if(!handle)
	{
		ILUnitFailed("could not create a monitor");
	}
	ILWaitHandleClose(handle);
}

/*
 * Test monitor acquire/release.
 */
static void wait_monitor_acquire(void *arg)
{
	ILWaitHandle *handle;

	/* Create the monitor */
	handle = ILWaitMonitorCreate();
	if(!handle)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Acquire it: zero timeout but we should get it immediately */
	if(ILWaitMonitorTryEnter(handle, 0) != 0)
	{
		ILUnitFailed("could not acquire (1)");
	}

	/* Acquire it again */
	if(ILWaitMonitorEnter(handle) != 0)
	{
		ILUnitFailed("could not acquire (2)");
	}

	/* Release twice */
	if(!ILWaitMonitorLeave(handle))
	{
		ILUnitFailed("could not release (1)");
	}
	if(!ILWaitMonitorLeave(handle))
	{
		ILUnitFailed("could not release (2)");
	}

	/* Try to release again, which should fail */
	if(ILWaitMonitorLeave(handle) != 0)
	{
		ILUnitFailed("released a monitor that we don't own");
	}

	/* Clean up */
	ILWaitHandleClose(handle);
}

/*
 * Thread start function that holds a monitor for a period of time.
 */
static void waitMonitorHold(void *arg)
{
	ILWaitHandle *monitor = ILWaitMonitorCreate();
	ILWaitMonitorEnter(monitor);
	sleepFor(2);
	globalFlag = 1;
	ILWaitMonitorLeave(monitor);
	ILWaitHandleClose(monitor);
	sleepFor(2);
}

/*
 * Test that a thread can be suspended while it holds a monitor.
 */
static void wait_monitor_suspend(void *arg)
{
	ILThread *thread;
	int savedFlag;

	/* Create the thread */
	thread = ILThreadCreate(waitMonitorHold, 0);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	/* Clear the global flag */
	globalFlag = 0;

	/* Start the thread, which should immediately suspend */
	ILThreadStart(thread);

	/* Wait 1 time step */
	sleepFor(1);

	/* Suspend the thread */
	ILThreadSuspend(thread);

	/* Wait for 4 time steps (enough for the thread to exit
	   if it wasn't suspended) */
	sleepFor(4);

	/* Save the global flag at this point */
	savedFlag = globalFlag;

	/* Resume the thread */
	ILThreadResume(thread);

	/* Wait 4 more time steps for the thread to exit */
	sleepFor(4);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	/* Check for errors: the flag must not have been set */
	if(savedFlag)
	{
		ILUnitFailed("thread holding the monitor did not suspend");
	}
	if(!globalFlag)
	{
		ILUnitFailed("thread holding the monitor did not finish");
	}
}

/*
 * Test monitor create primitive.
 */
static void primitive_monitor_create(void *arg)
{
	/* We are using the primitive versions for now. */
	_ILMonitor mon;
	int result;
	
	_ILMonitorCreate(&mon, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}
	if(_ILMonitorExit(&mon))
	{
		ILUnitFailed("could not exit a monitor");
	}
	_ILMonitorDestroy(&mon, result);
}

/*
 * Test monitor enter primitive.
 */
static void primitive_monitor_enter(void *arg)
{
	int result = 0;
	_ILMonitor mon;

	_ILMonitorCreate(&mon, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}
	if((result = _ILMonitorExit(&mon)))
	{
		_ILMonitorDestroy(&mon, result);
		ILUnitFailed("could not exit a monitor");
	}
	_ILMonitorDestroy(&mon, result);
}

static volatile int _result;
static _ILMonitor _mon1;

#ifndef IL_USE_PTHREADS
static void _primitive_monitor_exit_unowned(void *arg)
{
	_ILMonitor *mon1 = (_ILMonitor *)arg;

	_result = _ILMonitorExit(mon1);
}

/*
 * Test exiting an unowned monitor.
 */
static void primitive_monitor_exit_unowned(void *arg)
{
	int result = 0;
	ILThread *thread;

	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_exit_unowned, &_mon1);
	if(!thread)
	{
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	/* Set the result to an invalid value. */
	_result = -1;

	/* Start the thread, which should immediately try to exit the unowned monitor */
	ILThreadStart(thread);

	/* Wait 3 time steps */
	sleepFor(3);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(_result != IL_THREAD_ERR_SYNCLOCK)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitFailMessage("exiting an unowned monitor returned the wrong result");
		ILUnitFailMessage("the Wrong result is %i", _result);
		if(_result == IL_THREAD_OK)
		{
			ILUnitFailMessage("This is expected in pthread implementations");
		}
		ILUnitFailEndMessages();
	}

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILUnitFailed("could not exit a monitor");
	}
	_ILMonitorDestroy(&_mon1, result);
}
#endif /* !IL_USE_PTHREADS */

/*
 * Test monitor enter1.
 */
static void _primitive_monitor_enter_locked(void *arg)
{
	_ILMonitor *mon1 = (_ILMonitor *)arg;

	_result = 1;
	if(_ILMonitorEnter(mon1))
	{
		_result = 999;
		return;
	}
	_result = 2;
	if(_ILMonitorExit(mon1))
	{
		_result = 998;
	}
}

static void primitive_monitor_enter1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;
	int result2 = 0;

	/* We are using the primitive versions for now. */
	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_enter_locked, &_mon1);
	if(!thread)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 1 time steps */
	sleepFor(1);

	/* The result1 should be 1 now. */
	result1 = _result;

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	/* Wait 3 time steps */
	sleepFor(3);

	/* The result1 should be 2 now. */
	result2 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 1 || result2 != 2)
	{
		_ILMonitorDestroy(&_mon1, result);
		ILUnitFailed("lock on monitor enter doesn't work");
	}

	_ILMonitorDestroy(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not destroy a monitor");
	}
}

static void _primitive_monitor_tryenter_locked(void *arg)
{
	_ILMonitor *mon1 = (_ILMonitor *)arg;
	int result = _ILMonitorTryEnter(mon1);

	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 0;
			if(_ILMonitorExit(mon1))
			{
				_result = 998;
			}
		}
		break;

		case IL_THREAD_BUSY:
		{
			_result = 1;
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void primitive_monitor_tryenter1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;

	/* We are using the primitive versions for now. */
	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_tryenter_locked, &_mon1);
	if(!thread)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 1 time steps */
	sleepFor(2);

	/* The result1 should be 1 now. */
	result1 = _result;

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 1)
	{
		_ILMonitorDestroy(&_mon1, result);
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("tryenter on a locked monitor doesn't work");
	}

	_ILMonitorDestroy(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not destroy a monitor");
	}
}

static void primitive_monitor_tryenter2(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;

	/* We are using the primitive versions for now. */
	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_tryenter_locked, &_mon1);
	if(!thread)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps to let the thread enter the monitor. */
	sleepFor(2);

	result1 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 0)
	{
		_ILMonitorDestroy(&_mon1, result);
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("tryenter on a unlocked monitor doesn't work");
	}

	_ILMonitorDestroy(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not destroy a monitor");
	}
}

static void _primitive_monitor_timed_tryenter(void *arg)
{
	_ILMonitor *mon1 = (_ILMonitor *)arg;
	int result = _ILMonitorTimedTryEnter(mon1, 1);

	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 0;
			if(_ILMonitorExit(mon1))
			{
				_result = 998;
			}
		}
		break;

		case IL_THREAD_BUSY:
		{
			_result = 1;
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void primitive_monitor_timed_tryenter1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;

	/* We are using the primitive versions for now. */
	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_timed_tryenter, &_mon1);
	if(!thread)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps to let the thread try to enter the monitor. */
	sleepFor(2);

	result1 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	if(result1 != 1)
	{
		_ILMonitorDestroy(&_mon1, result);
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("timed tryenter on a locked monitor with timeout doesn't work");
	}

	_ILMonitorDestroy(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not destroy a monitor");
	}
}

static void _primitive_monitor_wait1(void *arg)
{
	_ILMonitor *mon1 = (_ILMonitor *)arg;
	int result;

	_result = 0;
	result = _ILMonitorEnter(mon1);
	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 1;
			if(_ILMonitorPulse(mon1))
			{
				result = 997;
				_ILMonitorExit(mon1);
			}
			else
			{
				if(_ILMonitorExit(mon1))
				{
					_result = 998;
				}
			}
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void primitive_monitor_wait1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;
	int result2 = 0;

	/* We are using the primitive versions for now. */
	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_wait1, &_mon1);
	if(!thread)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 0 now. */
	result1 = _result;

	if(_ILMonitorWait(&_mon1) != IL_THREAD_OK)
	{
		/* Clean up the thread object. */
		ILThreadDestroy(thread);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitFailed("wait on a monitor doesn't work");
	}

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 1 now. */
	result2 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 0)
	{
		_ILMonitorDestroy(&_mon1, result);
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("tryenter on a locked monitor doesn't work");
	}

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILUnitFailed("could not exit a monitor");
	}

	_ILMonitorDestroy(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not destroy a monitor");
	}

	if(result1 != 0 && result2 != 1)
	{
		ILUnitFailed("testcase failed");
	}
}

static void _primitive_monitor_timed_wait1(void *arg)
{
	_ILMonitor *mon1 = (_ILMonitor *)arg;
	int result;

	_result = 0;
	result = _ILMonitorEnter(mon1);
	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 1;
			/* Wait 3 time steps to let the calling thread timeout.*/
			sleepFor(3);
			if(_ILMonitorPulse(mon1))
			{
				result = 997;
				_ILMonitorExit(mon1);
			}
			else
			{
				if(_ILMonitorExit(mon1))
				{
					_result = 998;
				}
				else
				{
					sleepFor(1);
					if((result = _ILMonitorTryEnter(mon1)) == IL_THREAD_BUSY)
					{
						_result = 3;
					}
					else
					{
						if(result == IL_THREAD_OK)
						{
							_result = 996;
							_ILMonitorExit(mon1);
						}
						else
						{
							_result = 997;
						}
					}
				}
			}
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void primitive_monitor_timed_wait1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;
	int result2 = 0;

	/* We are using the primitive versions for now. */
	_ILMonitorCreate(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not create a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_primitive_monitor_timed_wait1, &_mon1);
	if(!thread)
	{
		_ILMonitorExit(&_mon1);
		_ILMonitorDestroy(&_mon1, result);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 0 now. */
	result1 = _result;

	result = _ILMonitorTimedWait(&_mon1, 2);
	if(result != IL_THREAD_BUSY)
	{
		/* Clean up the thread object. */
		ILThreadDestroy(thread);
		_ILMonitorDestroy(&_mon1, result1);
		printf("Wrong result is %i\n", result);
		ILUnitFailed("timed wait on a monitor doesn't work");
	}

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 3 now. */
	result2 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 0 || result2 != 3)
	{
		_ILMonitorDestroy(&_mon1, result);
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("tryenter on a locked monitor doesn't work");
	}

	if((result = _ILMonitorExit(&_mon1)))
	{
		_ILMonitorDestroy(&_mon1, result);
		ILUnitFailed("could not exit a monitor");
	}

	_ILMonitorDestroy(&_mon1, result);
	if(result)
	{
		ILUnitFailed("could not destroy a monitor");
	}

	if(result1 != 0 && result2 != 1)
	{
		ILUnitFailed("testcase failed");
	}
}

/*
 * Test monitor creation
 */
static void monitor_create(void *arg)
{
	void *monitorLocation;
	int result;

	/* initialize the test location */
	monitorLocation = 0;

	result = ILMonitorEnter(&monitorLocation);
	if(result != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a new monitor");
	}
	if(monitorLocation == 0)
	{
		ILUnitFailed("new monitor is not set at the monitor location");
	}

	result = ILMonitorExit(&monitorLocation);
	if(result != IL_THREAD_OK)
	{
		ILUnitFailed("could not exit a monitor");
	}
}

/*
 * Test entering and leaving a monitor multiple times
 */
static void monitor_enter_multiple(void *arg)
{
	void *monitorLocation;
	int result;
	int i;

	/* initialize the test location */
	monitorLocation = 0;

	for(i = 0; i < 10; ++i)
	{
		result = ILMonitorEnter(&monitorLocation);
		if(result != IL_THREAD_OK)
		{
			ILUnitFailed("could not enter a new monitor at i = %i", i);
		}
	}

	for(i = 0; i < 10; ++i)
	{
		result = ILMonitorExit(&monitorLocation);
		if(result != IL_THREAD_OK)
		{
			ILUnitFailed("could not exit the new monitor at i = %i", i);
		}
	}
}

void *_monitorLocation;

/*
 * Try to enter a locked monitor
 */
static void _monitor_enter_locked(void *arg)
{
	void **monitorLocation;

	monitorLocation = (void **)arg;
	_result = 1;
	if(ILMonitorEnter(monitorLocation) != IL_THREAD_OK)
	{
		_result = 999;
		return;
	}
	_result = 2;
	if(ILMonitorExit(monitorLocation) != IL_THREAD_OK)
	{
		_result = 998;
	}
}

static void monitor_enter_locked(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;
	int result2 = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	if((result = ILMonitorEnter(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a new monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_enter_locked, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 1 time steps */
	sleepFor(1);

	/* The result1 should be 1 now. */
	result1 = _result;

	if((result = ILMonitorExit(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	/* Wait 3 time steps */
	sleepFor(3);

	/* The result1 should be 2 now. */
	result2 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 1 || result2 != 2)
	{
		ILUnitFailed("lock on monitor enter doesn't work");
	}
}

static void _monitor_tryenter_locked(void *arg)
{
	void **monitorLocation;
	int result;

	monitorLocation = (void **)arg;
	result = ILMonitorTryEnter(monitorLocation);
	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 0;
			if(ILMonitorExit(monitorLocation) != IL_THREAD_OK)
			{
				_result = 998;
			}
		}
		break;

		case IL_THREAD_BUSY:
		{
			_result = 1;
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

/*
 * Test TryEnter on a locked monitor
 */
static void monitor_tryenter1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;

	/* initialize the test location */
	_monitorLocation = 0;		

	if((result = ILMonitorEnter(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_tryenter_locked, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 1 time steps */
	sleepFor(2);

	/* The result1 should be 1 now. */
	result1 = _result;

	if((result = ILMonitorExit(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 1)
	{
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("tryenter on a locked monitor doesn't work");
	}
}

/*
 * Test TryEnter on an unlocked monitor
 */
static void monitor_tryenter2(void *arg)
{
	ILThread *thread;
	int result = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	/* Create the thread */
	thread = ILThreadCreate(_monitor_tryenter_locked, (void *)&_monitorLocation);
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps to let the thread enter the monitor. */
	sleepFor(2);

	result = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result != 0)
	{
		printf("Wrong result is %i\n", result);
		ILUnitFailed("tryenter on a unlocked monitor doesn't work");
	}
}

static void _monitor_timed_tryenter(void *arg)
{
	void **monitorLocation;
	int result;

	monitorLocation = (void **)arg;
	result = ILMonitorTimedTryEnter(monitorLocation, 1);
	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 0;
			if(ILMonitorExit(monitorLocation) != IL_THREAD_OK)
			{
				_result = 998;
			}
		}
		break;

		case IL_THREAD_BUSY:
		{
			_result = 1;
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void monitor_timed_tryenter1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	if((result = ILMonitorEnter(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_timed_tryenter, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit((void *)&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps to let the thread try to enter the monitor. */
	sleepFor(2);

	result1 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if((result = ILMonitorExit((void *)&_monitorLocation)) != IL_THREAD_OK)
	{
		ILThreadDestroy(thread);
		ILUnitFailed("could not exit a monitor");
	}

	if(result1 != 1)
	{
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("timed tryenter on a locked monitor with timeout doesn't work");
	}
}

/*
 * Test for exiting an unowned monitor.
 */
static void _monitor_exit_unowned(void *arg)
{
	void **monitorLocation = (void **)arg;

	_result = ILMonitorExit(monitorLocation);
}

/*
 * Test exiting an unowned monitor.
 */
static void monitor_exit_unowned(void *arg)
{
	int result = 0;
	ILThread *thread;

	/* initialize the test location */
	_monitorLocation = 0;		

	/* Create the thread */
	thread = ILThreadCreate(_monitor_exit_unowned, (void *)(&_monitorLocation));
	if(!thread)
	{
		ILUnitOutOfMemory();
	}

	result = ILMonitorEnter(&_monitorLocation);
	if(result != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a new monitor");
	}

	/* Set the result to an invalid value. */
	_result = -1;

	/* Start the thread, which should immediately try to exit the unowned monitor */
	ILThreadStart(thread);

	/* Wait 3 time steps */
	sleepFor(3);

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(_result != IL_THREAD_ERR_SYNCLOCK)
	{
		ILMonitorExit(&_monitorLocation);
		printf("Wrong result is %i\n", _result);
		ILUnitFailed("exiting an unowned monitor returned the wrong result");
	}

	result = ILMonitorExit(&_monitorLocation);
	if(result)
	{
		ILUnitFailed("could not exit a monitor");
	}
}

static void _monitor_wait1(void *arg)
{
	void **monitorLocation;
	int result;

	monitorLocation = (void **)arg;
	_result = 0;
	result = ILMonitorEnter(monitorLocation);
	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 1;
			if(ILMonitorPulse(monitorLocation) == IL_THREAD_OK)
			{
				result = 997;
				ILMonitorExit(monitorLocation);
			}
			else
			{
				if(ILMonitorExit(monitorLocation) == IL_THREAD_OK)
				{
					_result = 998;
				}
			}
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void monitor_wait1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;
	int result2 = 0;

	/* initialize the test location */
	_monitorLocation = 0;		

	if(ILMonitorEnter(&_monitorLocation) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_wait1, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 0 now. */
	result1 = _result;

	if(ILMonitorWait(&_monitorLocation) != IL_THREAD_OK)
	{
		/* Clean up the thread object. */
		ILThreadDestroy(thread);
		ILMonitorExit(&_monitorLocation);
		ILUnitFailed("wait on a monitor doesn't work");
	}

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 1 now. */
	result2 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 0)
	{
		ILMonitorExit(&_monitorLocation);
		printf("Wrong result is %i\n", result1);
		ILUnitFailed("tryenter on a locked monitor doesn't work");
	}

	if((result = ILMonitorExit(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILUnitFailed("could not exit a monitor");
	}

	if(result1 != 0 && result2 != 1)
	{
		ILUnitFailed("testcase failed");
	}
}

static void _monitor_timed_wait1(void *arg)
{
	void **monitorLocation;
	int result;

	monitorLocation = (void **)arg;
	_result = 0;
	result = ILMonitorEnter(monitorLocation);
	switch(result)
	{
		case IL_THREAD_OK:
		{
			_result = 1;
			/* Wait 3 time steps to let the calling thread timeout.*/
			sleepFor(3);
			if(ILMonitorPulse(monitorLocation) != IL_THREAD_OK)
			{
				result = 997;
				ILMonitorExit(monitorLocation);
			}
			else
			{
				if(ILMonitorExit(monitorLocation) != IL_THREAD_OK)
				{
					_result = 998;
				}
				else
				{
					sleepFor(1);
					if((result = ILMonitorTryEnter(monitorLocation)) == IL_THREAD_BUSY)
					{
						_result = 3;
					}
					else
					{
						if(result == IL_THREAD_OK)
						{
							_result = 996;
							ILMonitorExit(monitorLocation);
						}
						else
						{
							_result = 997;
						}
					}
				}
			}
		}
		break;

		default:
		{
			_result = 999;
		}
	}
}

static void monitor_timed_wait1(void *arg)
{
	ILThread *thread;
	int result = 0;
	int result1 = 0;
	int result2 = 0;

	/* initialize the test location */
	_monitorLocation = 0;		

	if(ILMonitorEnter(&_monitorLocation) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_timed_wait1, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 0 now. */
	result1 = _result;

	result = ILMonitorTimedWait(&_monitorLocation, 2);
	if(result != IL_THREAD_BUSY)
	{
		/* Clean up the thread object. */
		ILThreadDestroy(thread);
		printf("Wrong result is %i\n", result);
		ILUnitFailed("timed wait on a monitor doesn't work");
	}

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 3 now. */
	result2 = _result;

	/* Clean up the thread object (the thread itself is now dead) */
	ILThreadDestroy(thread);

	if(result1 != 0 || result2 != 3)
	{
		printf("Wrong result1 is %i\n", result1);
		printf("Wrong result2 is %i\n", result2);
		ILUnitFailed("tryenter on a locked monitor doesn't work");
	}

	if((result = ILMonitorExit(&_monitorLocation)) != IL_THREAD_OK)
	{
		ILUnitFailed("could not exit a monitor");
	}

	if(result1 != 0 && result2 != 1)
	{
		ILUnitFailed("testcase failed");
	}
}

static void _monitor_enter(void *arg)
{
	void **monitorLocation;

	monitorLocation = (void **)arg;
	_result = 0;
	_result = ILMonitorEnter(monitorLocation);
}

static void monitor_interrupt_during_enter(void *arg)
{
	ILThread *thread;
	int haveError = 0;
	int result = 0;
	int result1 = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	if(ILMonitorEnter(&_monitorLocation) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_enter, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 0 now. */
	result1 = _result;

	/* interrupt the thread */
	ILThreadInterrupt(thread);
	
	if((result = ILThreadJoin(thread, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread with returncode %i", result);
	}

	ILThreadDestroy(thread);

	if(_result != IL_THREAD_ERR_INTERRUPT)
	{
		haveError = 1;
		ILUnitFailMessage("Wrong returncode for abort during enter was %i", result);
	}
	ILMonitorExit(&_monitorLocation);
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void monitor_abort_during_enter(void *arg)
{
	ILThread *thread;
	int haveError = 0;
	int result = 0;
	int result1 = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	if(ILMonitorEnter(&_monitorLocation) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_enter, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* Wait 2 time steps */
	sleepFor(2);

	/* The result1 should be 0 now. */
	result1 = _result;

	/* Abort the thread */
	result = ILThreadAbort(thread);
	
	if((result = ILThreadJoin(thread, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread with returncode %i", result);
	}

	ILThreadDestroy(thread);

	if(_result != IL_THREAD_ERR_ABORTED)
	{
		haveError = 1;
		ILUnitFailMessage("Wrong returncode for abort during enter was %i", result);
	}
	ILMonitorExit(&_monitorLocation);
	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void _monitor_wait(void *arg)
{
	void **monitorLocation = (void **)arg;

	/* First enter the monitor for this location */
	if(ILMonitorEnter(monitorLocation) != IL_THREAD_OK)
	{
		_result = 999;
		return;
	}
	/* Now pulse the monitor to releaase the main thread */
	if(ILMonitorPulse(monitorLocation)  != IL_THREAD_OK)
	{
		_result = 998;
		return;
	}
	/*
	 * Wait 2 time steps to give the main thread the possibility to
	 * run again and get the pulsed signal.
	 */
	sleepFor(2);

	/* Wait on the monitor */
	_result = ILMonitorWait(monitorLocation);
	
	/* And exit the monitor again */
	if(ILMonitorExit(monitorLocation) != IL_THREAD_OK)
	{
		_result = 996;
	}
}

static void monitor_interrupt_during_wait(void *arg)
{
	ILThread *thread;
	int haveError = 0;
	int result = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	if(ILMonitorEnter(&_monitorLocation) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_wait, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* And wait on the monitor */
	if((result = ILMonitorWait(&_monitorLocation)) != IL_THREAD_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Wait on the monitor failed with returncode %i", result);
	}
	
	/* interrupt the thread while it is waiting on the monitor*/
	ILThreadInterrupt(thread);
	
	/* And exit the monitor */
	if((result = ILMonitorExit(&_monitorLocation)) != IL_THREAD_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Exiting the monitor failed with returncode %i", result);
	}
	
	if((result = ILThreadJoin(thread, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread with returncode %i", result);
	}

	ILThreadDestroy(thread);

	if(_result != IL_THREAD_ERR_INTERRUPT)
	{
		haveError = 1;
		ILUnitFailMessage("Wrong returncode for interrupt during wait was %i", _result);
	}

	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

static void monitor_abort_during_wait(void *arg)
{
	ILThread *thread;
	int haveError = 0;
	int result = 0;

	/* initialize the test location */
	_monitorLocation = 0;

	if(ILMonitorEnter(&_monitorLocation) != IL_THREAD_OK)
	{
		ILUnitFailed("could not enter a monitor");
	}

	/* Create the thread */
	thread = ILThreadCreate(_monitor_wait, (void *)&_monitorLocation);
	if(!thread)
	{
		ILMonitorExit(&_monitorLocation);
		ILUnitOutOfMemory();
	}

	_result = -1;

	/* Start the thread, which should immediately try to enter the monitor. */
	ILThreadStart(thread);

	/* And wait on the monitor */
	if((result = ILMonitorWait(&_monitorLocation)) != IL_THREAD_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Wait on the monitor failed with returncode %i", result);
	}
	
	/* interrupt the thread while it is waiting on the monitor*/
	ILThreadAbort(thread);
	
	/* And exit the monitor */
	if((result = ILMonitorExit(&_monitorLocation)) != IL_THREAD_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Exiting the monitor failed with returncode %i", result);
	}
	
	if((result = ILThreadJoin(thread, 10000)) != IL_JOIN_OK)
	{
		haveError = 1;
		ILUnitFailMessage("Failed to join thread with returncode %i", result);
	}

	ILThreadDestroy(thread);

	if(_result != IL_THREAD_ERR_ABORTED)
	{
		haveError = 1;
		ILUnitFailMessage("Wrong returncode for abort during wait was %x", _result);
	}

	if(haveError)
	{
		ILUnitFailEndMessages();
	}
}

/*
 * Simple test registration macro.
 */
#define	RegisterSimple(name)	(ILUnitRegister(#name, name, 0))

/*
 * Register all unit tests.
 */
void ILUnitRegisterTests(void)
{
	/*
	 * Bail out if no thread support at all in the system.
	 */
	if(!ILHasThreads())
	{
		fputs("System does not support threads - skipping all tests\n", stdout);
		return;
	}

	/*
	 * Initialize the thread subsystem.
	 */
	ILThreadInit();

	/*
	 * Initialize the GC system (the GC is used to create threads).
	 */
	ILGCInit(0);	

	/*
	 * Test the properties of the "main" thread.
	 */
	ILUnitRegisterSuite("Main Thread Properties");
	RegisterSimple(thread_main_nonnull);
	RegisterSimple(thread_main_object);
	RegisterSimple(thread_main_running);
	RegisterSimple(thread_main_foreground);

	/*
	 * Test thread creation behaviours.
	 */
	ILUnitRegisterSuite("Thread Creation");
	RegisterSimple(thread_create_arg);
	RegisterSimple(thread_create_suspended);
	RegisterSimple(thread_create_state);
	RegisterSimple(thread_create_destroy);
	RegisterSimple(thread_create_foreground);

	/*
	 * Test thread suspend behaviours.
	 */
	ILUnitRegisterSuite("Thread Suspend");
	RegisterSimple(thread_suspend);
	RegisterSimple(thread_suspend_self);
	RegisterSimple(thread_suspend_destroy);
	RegisterSimple(thread_suspend_mutex);
	ILUnitRegister("thread_suspend_rdlock", thread_suspend_rwlock, (void *)1);
	ILUnitRegister("thread_suspend_wrlock", thread_suspend_rwlock, (void *)0);
	RegisterSimple(thread_suspend_main);
	RegisterSimple(thread_suspend_main_self);

	/*
	 * Test thread sleep and interrupt behaviours.
	 */
	ILUnitRegisterSuite("Thread Sleep");
	RegisterSimple(thread_sleep);
	RegisterSimple(thread_sleep_interrupt);
	RegisterSimple(thread_sleep_suspend);
	RegisterSimple(thread_sleep_suspend_ignore);

	/*
	 * Test miscellaneous thread behaviours.
	 */
	ILUnitRegisterSuite("Misc Thread Tests");
	RegisterSimple(thread_other_object);
	RegisterSimple(thread_counts);

	/*
	 * Test the interlocked operations.
	 */
	ILUnitRegisterSuite("Interlocked operations");
	RegisterSimple(interlocked_load);
	RegisterSimple(interlocked_store);
	RegisterSimple(interlocked_exchange);
	RegisterSimple(interlocked_exchange_pointers);
	RegisterSimple(interlocked_compare_and_exchange);
	RegisterSimple(interlocked_compare_and_exchange_fail);
	RegisterSimple(interlocked_compare_and_exchange_pointers);
	RegisterSimple(interlocked_compare_and_exchange_pointers_fail);
	RegisterSimple(interlocked_increment);
	RegisterSimple(interlocked_decrement);
	RegisterSimple(interlocked_add);
	RegisterSimple(interlocked_sub);
	RegisterSimple(interlocked_and);
	RegisterSimple(interlocked_or);
	RegisterSimple(interlocked_thrash_incdec);
	RegisterSimple(interlocked_thrash_addsub);

	/*
	 * Test the interlocked single linked list operations.
	 */
	ILUnitRegisterSuite("Interlocked single linked list operations");
	RegisterSimple(interlocked_slist_create);
	RegisterSimple(interlocked_slist_add_rem_single);
	RegisterSimple(interlocked_slist_add_rem_thrash);

	
	/*
	 * Test wait mutex behaviours.
	 */
	ILUnitRegisterSuite("Wait Mutex Tests");
	RegisterSimple(mutex_create);

	/*
	 * Test for the implementation of the monitor primitives.
	 */
	ILUnitRegisterSuite("Monitor primitives Tests");
	RegisterSimple(primitive_monitor_create);
	RegisterSimple(primitive_monitor_enter);
#ifndef IL_USE_PTHREADS
	/*
	 * Disable this tests for pthreads because the the behavior is
	 * unpredictable on this platform.
	 */
	RegisterSimple(primitive_monitor_exit_unowned);
#endif /* !IL_USE_PTHREADS */
	RegisterSimple(primitive_monitor_enter1);
	RegisterSimple(primitive_monitor_tryenter1);
	RegisterSimple(primitive_monitor_tryenter2);
	RegisterSimple(primitive_monitor_timed_tryenter1);
	RegisterSimple(primitive_monitor_wait1);
	RegisterSimple(primitive_monitor_timed_wait1);

	/*
	 * Test wait monitor behaviours.
	 */
	ILUnitRegisterSuite("Wait Monitor Tests");
	RegisterSimple(wait_monitor_create);
	RegisterSimple(wait_monitor_acquire);
	RegisterSimple(wait_monitor_suspend);

	/*
	 * Tests for the monitor implementation.
	 */
	ILUnitRegisterSuite("Monitor Tests");
	RegisterSimple(monitor_create);
	RegisterSimple(monitor_enter_multiple);
	RegisterSimple(monitor_enter_locked);
	RegisterSimple(monitor_tryenter1);
	RegisterSimple(monitor_tryenter2);	
	RegisterSimple(monitor_timed_tryenter1);
	RegisterSimple(monitor_exit_unowned);
	RegisterSimple(monitor_wait1);
	RegisterSimple(monitor_timed_wait1);
	RegisterSimple(monitor_interrupt_during_enter);
	RegisterSimple(monitor_abort_during_enter);
	RegisterSimple(monitor_interrupt_during_wait);
	RegisterSimple(monitor_abort_during_wait);
}

void ILUnitCleanupTests(void)
{
	/*
	 * Deinitialize the threading subsystem.
	 */
	ILThreadDeinit();
}

#ifdef	__cplusplus
};
#endif
