/*
 * il_thread.h - Thread support routines.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

#ifndef	_IL_THREAD_H
#define	_IL_THREAD_H

#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * States that a thread can be in.  Based on the values
 * in the C# "ThreadState" enumeration.
 */
#define	IL_TS_RUNNING			0x0000
#define	IL_TS_STOP_REQUESTED	0x0001
#define	IL_TS_SUSPEND_REQUESTED	0x0002
#define	IL_TS_BACKGROUND		0x0004
#define	IL_TS_UNSTARTED			0x0008
#define	IL_TS_STOPPED			0x0010
#define	IL_TS_WAIT_SLEEP_JOIN	0x0020
#define	IL_TS_SUSPENDED			0x0040
#define	IL_TS_ABORT_REQUESTED	0x0080
#define	IL_TS_ABORTED			0x0100

/*
 * Threading error codes.
 */
#define IL_THREAD_OK						0x00000000
#define IL_THREAD_ERR_ABANDONED				0x00000080
#define IL_THREAD_ERR_INTERRUPT				0x000000C0
#define IL_THREAD_BUSY						0x00000102
#define IL_THREAD_ERR_INVALID_TIMEOUT		0x80131502
#define IL_THREAD_ERR_SYNCLOCK				0x80131518
#define IL_THREAD_ERR_ABORTED				0x80131530
#define IL_THREAD_ERR_INVALID_RELEASECOUNT	0xFFFFFFFD
#define IL_THREAD_ERR_OUTOFMEMORY			0xFFFFFFFE
#define IL_THREAD_ERR_UNKNOWN				0xFFFFFFFF

/*
 * Opaque types for thread-related objects.
 */
typedef struct _tagILThread     ILThread;
typedef struct _tagILMutex      ILMutex;
typedef struct _tagILRWLock     ILRWLock;
typedef struct _tagILSemaphore  ILSemaphore;
typedef struct _tagILWaitHandle ILWaitHandle;

/*
 * Type for a thread start function.
 */
typedef void (*ILThreadStartFunc)(void *startArg);

#define IL_INTERRUPT_TYPE_ILLEGAL_MEMORY_ACCESS (1)
#define IL_INTERRUPT_TYPE_INT_DIVIDE_BY_ZERO	(2)
#define IL_INTERRUPT_TYPE_INT_OVERFLOW			(4)

/*
 * Cleanup function for ILThread.
 */
typedef void (*ILThreadCleanupFunc)(ILThread *thread);

/*
 * Interrupt context
 * (see support/interrupt.h for platform specific definitioons)
 */
typedef struct _tagILInterruptContext ILInterruptContext;

/*
 * Function that handles an interrupt.
 */
typedef void (*ILInterruptHandler)(ILInterruptContext *context);

/*
 * Set the interrupt handler for the given thread.
 */
int ILThreadRegisterInterruptHandler(ILThread *thread, ILInterruptHandler handler);

/*
 * Unset the interrupt handler for the given thread.
 */
int ILThreadUnregisterInterruptHandler(ILThread *thread, ILInterruptHandler handler);

/*
 * Get the monitor for the thread.
 */
ILWaitHandle *ILThreadGetMonitor(ILThread *thread);

/*
 * Clear (length) amount of bytes from the stack.
 */
void ILThreadClearStack(int length);

/*
 * Determine if the system has thread support.  This can
 * be called either before or after "ILThreadInit".
 */
int ILHasThreads(void);

/*
 * Initialize the thread support routines.  Only needs to be
 * called once but it is safe to call multiple times.
 */
void ILThreadInit(void);

/*
 * Cleans up the threading subsystem.  Safe to call multiple times.  The thread
 * subsystem can't be reinitialized after it has been deinitialized.
 */
void ILThreadDeinit(void);

/*
 * Create a new thread, initially in the "unstarted" state.
 * When the thread is started, it will call "startFunc" with
 * the supplied "startArg".  Returns NULL if there are
 * insufficient resources to start the thread.
 */
ILThread *ILThreadCreate(ILThreadStartFunc startFunc, void *startArg);

/*
 * Start a thread running.  Returns zero if not in the
 * correct state to start.
 */
int ILThreadStart(ILThread *thread);

/*
 * Destroy a thread.  This cannot be used to destroy
 * the current thread.
 */
void ILThreadDestroy(ILThread *thread);

/*
 * Get the thread descriptor for the current thread.
 */
ILThread *ILThreadSelf(void);

/*
 * Get the object reference that is associated with a thread.
 */
void *ILThreadGetObject(ILThread *thread);

/*
 * Run a function in a thread not created by pnet.
 */
void *ILThreadRunSelf(void *(* thread_func)(void *), void *arg);

/*
 * Set the object reference that is associated with a thread.
 * This is used by the engine to store a pointer to an ILExecThread.
 */
void ILThreadSetObject(ILThread *thread, void *userObject);

#define IL_SUSPEND_FAILED				(0)
#define IL_SUSPEND_OK					(1)
#define IL_SUSPEND_REQUESTED		(2)

/*
 * Suspend a thread.  Does nothing if the thread is already
 * suspended.  Returns non-zero if OK, or zero if the thread
 * is not in an appropriate state.  Returns 2 if the thread
 * is in a wait/sleep/join state and has been flagged into a
 * a suspend_requested state.
 */
int ILThreadSuspend(ILThread *thread);

/*
 * Suspend or request a thread to suspend. If requestOnly is
 * 1 then the thread will only be flagged as suspend_requested
 * and will be suspended the next time it enters and comes out
 * of a wait/sleep/join state (even if it currently isn't in a 
 * wait/sleep/join state).
 */
int ILThreadSuspendRequest(ILThread *thread, int requestOnly);

/*
 * Resume a suspended thread.  Does nothing if the thread
 * is already resumed.
 */
void ILThreadResume(ILThread *thread);

/*
 * Interrupt a thread that is currently in the "wait/sleep/join"
 * thread state.  If the thread is not currently in that state,
 * the request will be queued.
 */
void ILThreadInterrupt(ILThread *thread);

/*
 * Send IL_SIG_ABORT to given thread. This will abort thread that
 * is blocking in system call.
 */
void ILThreadSigAbort(ILThread *thread);

/*
 * Request that a thread be aborted.  Returns zero if the thread
 * is already aborting or already has an abort request queued.
 */
int ILThreadAbort(ILThread *thread);

/*
 * Called by the current thread when it is ready to being the
 * abort process.  The thread moves from an IL_TS_ABORT_REQUESTED
 * state to an IL_TS_ABORTED state.
 */
int ILThreadSelfAborting();

/*
 * Determine if the current thread is in the process of
 * aborting.  If this function returns non-zero, the current
 * thread should call "ILThreadSelfAborting" on at some
 * future point to determine if we are processing an abort
 * request, or are in the middle of an abort.  i.e. the
 * correct way to check for aborts is as follows:
 *
 *		if(ILThreadIsAborting())
 *		{
 *			if(ILThreadSelfAborting)
 *			{
 *				// Abort request received: do initial processing.
 *				...
 *			}
 *			else
 *			{
 *				// We are processing an existing abort.
 *				...
 *			}
 *		}
 */
int ILThreadIsAborting(void);

/*
 * Returns 1 if an abort has been requested.
 */
int ILThreadIsAbortRequested(void);

/*
 * Reset a pending abort on the current thread.  Returns
 * zero if an abort is not pending.
 */
int ILThreadAbortReset(void);

/*
 *	Thread priorities.
 */
#define IL_TP_LOWEST					0x0
#define IL_TP_BELOW_NORMAL		0x1
#define IL_TP_NORMAL					0x2
#define IL_TP_ABOVE_NORMAL		0x3
#define IL_TP_HIGHEST					0x4

/*
 *	Sets the thread priority.
 */
void ILThreadSetPriority(ILThread *thread, int priority);

/*
 *	Gets the thread priority.
 */
int ILThreadGetPriority(ILThread *thread);

/*
 * Join result codes.
 */
#define	IL_JOIN_TIMEOUT		0	/* Join timed out */
#define	IL_JOIN_OK			1	/* Join was successful */
#define	IL_JOIN_INTERRUPTED	2	/* Join was interrupted */
#define	IL_JOIN_ABORTED		3	/* Thread doing the join was aborted */
#define	IL_JOIN_SELF		4	/* Tried to join with ourselves */
#define	IL_JOIN_UNSTARTED   5   /* Tried to join a thread that hasn't started */
#define	IL_JOIN_MEMORY		6	/* Out of memory */

/*
 * Join with another thread to wait for exit.
 */
int ILThreadJoin(ILThread *thread, ILUInt32 ms);

/*
 * Get the background state of a thread.
 */
int ILThreadGetBackground(ILThread *thread);

/*
 * Set the background state of a thread.
 */
void ILThreadSetBackground(ILThread *thread, int flag);

/*
 * Get the current thread state flags.
 */
int ILThreadGetState(ILThread *thread);

/*
 * Start a critical section for atomic operations.
 * This isn't highly efficient, but it should be
 * safe to use on all platforms.
 */
void ILThreadAtomicStart(void);

/*
 * End a critical section for atomic operations.
 */
void ILThreadAtomicEnd(void);

/*
 * Process a memory barrier within the current thread.
 */
void ILThreadMemoryBarrier(void);

/*
 * Get the number of foreground and background threads
 * that currently exist in the system.
 */
void ILThreadGetCounts(unsigned long *numForeground,
					   unsigned long *numBackground);

/*
 * Put a thread to sleep for a given number of milliseconds.
 * Specifying "ms == 0" is the same as yielding the thread.
 * Specifying "ms == IL_MAX_UINT32" will sleep forever.
 * Returns 1 on sucess and IL_WAIT_TIMEOUT, IL_WAIT_ABORTED
 * or IL_WAIT_INTERRUPTED.
 */
int ILThreadSleep(ILUInt32 ms);

/*
 * Yield CPUs time to threads of equal or higher priority.
 */
void ILThreadYield();

/*
 *	Wait for a specified amount of time (in ms) for a all foreground threads
 * (except the main thread) to finish.  A timeout of -1 means infinite.
 */
void ILThreadWaitForForegroundThreads(int timeout);

/*
 *	Registers a function that will be called by the thread when it has thread finished.
 *
 * Returns 0 on success.
 */
int ILThreadRegisterCleanup(ILThread *thread, ILThreadCleanupFunc func);

/*
 *	Unregisters a thread cleanup function.
 *
 * Returns 0 on success.
 */
int ILThreadUnregisterCleanup(ILThread *thread, ILThreadCleanupFunc func);

/*
 * Create a mutex.  Note: this type of mutex will not
 * necessarily update the thread's "wait/sleep/join"
 * state, so it isn't directly suitable for emulating
 * Windows-like wait handle mutexes.  It is useful for
 * simple non-recursive mutual exclusion operations
 * that won't otherwise affect the thread's state.
 */
ILMutex *ILMutexCreate(void);

/*
 * Destroy a mutex.
 */
void ILMutexDestroy(ILMutex *mutex);

/*
 * Lock a mutex.
 */
void ILMutexLock(ILMutex *mutex);

/*
 * Unlock a mutex.
 */
void ILMutexUnlock(ILMutex *mutex);

/*
 * Create a read-write lock.  If the system does not
 * support read-write locks, this will act like a mutex.
 * Note: read-write locks, like mutexes, do not necessarily
 * update the thread's "wait/sleep/join" state.
 */
ILRWLock *ILRWLockCreate(void);

/*
 * Destroy a read-write lock.
 */
void ILRWLockDestroy(ILRWLock *rwlock);

/*
 * Lock a read-write lock for reading.
 */
void ILRWLockReadLock(ILRWLock *rwlock);

/*
 * Lock a read-write lock for writing.
 */
void ILRWLockWriteLock(ILRWLock *rwlock);

/*
 * Unlock a read-write lock.
 */
void ILRWLockUnlock(ILRWLock *rwlock);

/*
 * Create a semaphore. Note: this type of semaphore will not
 * necessarily update the thread's "wait/sleep/join"
 * state, so it isn't directly suitable for emulating
 * Windows-like wait handle semaphores.
 */
ILSemaphore *ILSemaphoreCreate(void);

/*
 * Destroy a semaphore.
 */
void ILSemaphoreDestroy(ILSemaphore *sem);

/*
 * Wait on a semaphore.
 */
void ILSemaphoreWait(ILSemaphore *sem);

/*
 * Increase the semaphore count by 1.
 */
void ILSemaphorePost(ILSemaphore *sem);

/*
 * Increase the semaphore count by count.
 */
void ILSemaphorePostMultiple(ILSemaphore *sem, ILUInt32 count);

/*
 * Close a wait handle.  Returns zero if the handle is
 * currently owned by a thread.
 */
int ILWaitHandleClose(ILWaitHandle *handle);

/*
 * Special timeout values.
 */
#define	IL_WAIT_INFINITE		((ILUInt32)IL_MAX_UINT32)

/*
 * Return values for "ILWaitOne", "ILWaitAny", and "ILWaitAll".
 */
#define	IL_WAIT_TIMEOUT			(-1)
#define	IL_WAIT_FAILED			(-2)
#define	IL_WAIT_INTERRUPTED		(-3)
#define	IL_WAIT_ABORTED			(-4)

/*
 * Wait for one wait handle to become available.  Returns
 * zero if the wait handle was acquired.
 */
int ILWaitOne(ILWaitHandle *handle, ILUInt32 timeout);

/*
 *	Signals and waits for a handle atomically.
 */
int ILSignalAndWait(ILWaitHandle *signalHandle, ILWaitHandle *waitHandle, ILUInt32 timeout);

/*
 * Wait for any wait handle in a set to become available.
 * Returns the index of the handle that was acquired.
 */
int ILWaitAny(ILWaitHandle **handles, ILUInt32 numHandles, ILUInt32 timeout);

/*
 * Wait for all handles in a set to become available.
 * Returns zero if all wait handles were acquired.
 */
int ILWaitAll(ILWaitHandle **handles, ILUInt32 numHandles, ILUInt32 timeout);

/*
 * Create a wait mutex, which differs from a regular
 * mutex in that it can be used from C# code, and can
 * be held for long periods of time.
 */
ILWaitHandle *ILWaitMutexCreate(int initiallyOwned);

/*
 * Create a named wait mutex, or return an existing named
 * mutex with the same name.
 */
ILWaitHandle *ILWaitMutexNamedCreate(const char *name, int initiallyOwned, int *gotOwnership);

/*
 * Release a wait mutex that is currently held by the
 * current thread.  Returns zero if not held.
 */
int ILWaitMutexRelease(ILWaitHandle *handle);

/*
 * Create a wait handle that corresponds to a monitor.
 */
ILWaitHandle *ILWaitMonitorCreate(void);

/*
 * Wait on a monitor for a "pulse" operation.  Returns zero if we
 * don't own the monitor, 1 if waiting, or an "IL_WAIT_*" error
 * code otherwise.
 */
int ILWaitMonitorWait(ILWaitHandle *handle, ILUInt32 timeout);

/*
 * Pulse a single waiting thread on a monitor.
 */
int ILWaitMonitorPulse(ILWaitHandle *handle);

/*
 * Pulse all waiting threads on a monitor.
 */
int ILWaitMonitorPulseAll(ILWaitHandle *handle);

#define IL_WAITMUTEX_RELEASE_SUCCESS	(1)
#define IL_WAITMUTEX_RELEASE_STILL_OWNS	(2)
#define IL_WAITMUTEX_RELEASE_FAIL	(0)

/*
 * Syntactic sugar.
 */
#define	ILWaitMonitorEnter(monitor)		\
			(ILWaitOne((monitor), IL_WAIT_INFINITE))
#define	ILWaitMonitorTryEnter(monitor,timeout)		\
			(ILWaitOne((monitor), (timeout)))
#define	ILWaitMonitorLeave(monitor)		\
			(ILWaitMutexRelease((monitor)))
/*
 * Wait Event definitions
 *
 * 12-DEC-2002  Thong Nguyen (tum@veridicus.com)
 */

/*
 * Creates an event.
 */
ILWaitHandle *ILWaitEventCreate(int manualReset, int initialState);

/*
 * Sets an event.
 */
int ILWaitEventSet(ILWaitHandle *event);

/*
 * Resets an event.
 */
int ILWaitEventReset(ILWaitHandle *event);

/*
 *	Pulses an event.
 *
 * If the wait event is a manual reset event, all waiting threads are signalled.
 * If the wait event is an auto reset event, a single waiting thread is signalled.
 * On return, the wait event is unsignalled.
 */
int ILWaitEventPulse(ILWaitHandle *event);

/*
 * Enter a monitor 
 * This function returns IL_THREAD_OK on success, IL_THREAD_BUSY on timeout
 * or any other of the threading return codes on error.
 * NOTE: The monitor is only entered if IL_THREAD_OK is returned.
 * On every other return value the lock is not obtained.
 * This is because the code used with monitors usually looks like:
 * Monitor.Enter(obj);
 * try
 * {
 *    do_something
 * }
 * finally
 * {
 *    Monitor.Exit(obj);
 * }
 * So the exception is thrown outside the try/finally handlier that ensures
 * propper monitor release.
 */
int ILMonitorTimedTryEnter(void **monitorLocation, ILUInt32 ms);
#define ILMonitorEnter(loc)		ILMonitorTimedTryEnter((loc), IL_MAX_UINT32)
#define ILMonitorTryEnter(loc)	ILMonitorTimedTryEnter((loc), 0)

/*
 * Leave the monitor stored at monitorLocation.
 * This function returns IL_THREAD_OK on success, IL_THREAD_BUSY on timeout
 * or any other of the threading return codes on error.
 */
int ILMonitorExit(void **monitorLocation);

/*
 * Enter the wait state on an owned monitor.
 */
int ILMonitorTimedWait(void **monitorLocation, ILUInt32 ms);
#define ILMonitorWait(loc)	ILMonitorTimedWait((loc), IL_MAX_UINT32)

/*
 * Move one thread in the waiting queue to the ready queue in the monitor
 * stored at monitorLocation.
 */
int ILMonitorPulse(void **monitorLocation);

/*
 * Move all threads in the waiting queue to the ready queue in the monitor
 * stored at monitorLocation.
 */
int ILMonitorPulseAll(void **monitorLocation);

/*
 * Reclaim the monitor stored at monitorLocation.
 * This function is used for example if a monitor is attached to an object
 * that is going to be reclaimed by a garbage collector.
 * The monitor will not be needed anymore but it's state is undefined.
 * Reclaiming a monitor is caused by a bug in the using application.
 */
void ILMonitorReclaim(void **monitorLocation);

#ifdef	__cplusplus 
};
#endif

#endif	/* _IL_THREAD_H */
