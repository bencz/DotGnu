/*
 * w32_defs.h - Thread definitions for using Win32 threads.
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

#ifndef	_W32_DEFS_H
#define	_W32_DEFS_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * We need the thread identifier in this implementation.
 */
#define IL_THREAD_NEED_IDENTIFIER 1

/*
 * Types that are needed elsewhere.
 */
typedef CRITICAL_SECTION	_ILCriticalSection;
typedef CRITICAL_SECTION	_ILMutex;
typedef HANDLE				_ILCondMutex;
typedef HANDLE				_ILCondVar;
typedef HANDLE				_ILThreadHandle;
typedef DWORD				_ILThreadIdentifier;
typedef HANDLE				_ILSemaphore;
typedef CRITICAL_SECTION	_ILRWLock;

/*
 * Semaphore which allows to release multiple or all waiters.
 */
typedef struct
{
	_ILCriticalSection	_lock;
	_ILSemaphore		_sem;
	ILInt32 volatile	_waiting;
} _ILCountSemaphore;

/*
 * Structure of a monitor.
 */
typedef struct
{
	ILInt32				_waitValue;
	_ILSemaphore		_sem;
	_ILCondMutex		_mutex;
} _ILMonitor;

/*
 * Determine if we are using compiler thread local storage
 */
#if defined(USE_COMPILER_TLS)
#if !defined(__GNUC__)
#undef USE_COMPILER_TLS
#else
#define _THREAD_ __thread
#endif
#endif

/*
 * This is a real thread package.
 */
#define	_ILThreadIsReal		1

/*
 * Determine if a thread corresponds to "self".
 */
#define	_ILThreadIsSelf(thread)	\
			((thread)->identifier == GetCurrentThreadId())

/*
 * Send an interrupt request to a thread.
 */
void _ILThreadInterrupt(ILThread *thread);

/*
 * Some helper macros for wait handles.
 */

/*
 * Close (destroy) a WaitHandle.
 */
#define _ILWaitHandleClose(handle) (CloseHandle(handle) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN)

/*
 * Wait for a WaitHandle for the given amount of time.
 */
int	_ILWaitHandleTimedWait(HANDLE handle, ILUInt32 ms);

/*
 * Wait for a WaitHandle infinitely untit the WaitHandle is signaled.
 */
#define _ILWaitHandleWait(handle)	_ILWaitHandleTimedWait((handle), IL_MAX_UINT32)

/*
 * Suspend and resume threads.  Note: these are the primitive
 * versions, which are not "suspend-safe".
 */
#define	_ILThreadSuspendOther(thread)	\
			do { \
				SuspendThread((thread)->handle); \
			} while (0)
#define	_ILThreadSuspendSelf(thread)	\
			do { \
				WaitForSingleObject(thread->resumeAck, INFINITE); \
			} while (0)
#define	_ILThreadResumeOther(thread)	\
			do { \
				ResumeThread((thread)->handle); \
			} while (0)
#define	_ILThreadResumeSelf(thread)		\
			do { \
				ReleaseSemaphore(thread->resumeAck, 1, NULL); \
			} while (0)

/*
 * Terminate a running thread.
 */
#define	_ILThreadTerminate(thread)	\
			do { \
				TerminateThread((thread)->handle, 0); \
			} while (0)

/*
 * Destroy a thread handle that is no longer required.
 */
#define	_ILThreadDestroy(thread)	_ILWaitHandleClose((thread)->handle)

/*
 * Put the current thread to sleep for a number of milliseconds.
 * The sleep may be interrupted by a call to _ILThreadInterrupt.
 */
int _ILThreadSleep(ILUInt32 ms);

/*
 * Primitive critical section operations.
 * NOTE:: the "EnterUnsafe" and "LeaveUnsafe" operations are not "suspend-safe"
 */
#define	_ILCriticalSectionCreate(critsect)	\
			do { \
				InitializeCriticalSection((critsect)); \
			} while (0)
#define	_ILCriticalSectionDestroy(critsect)	\
			do { \
				DeleteCriticalSection((critsect)); \
			} while (0)
#define	_ILCriticalSectionEnterUnsafe(critsect)	\
			do { \
				EnterCriticalSection((critsect)); \
			} while (0)
#define	_ILCriticalSectionLeaveUnsafe(critsect)	\
			do { \
				LeaveCriticalSection((critsect)); \
			} while (0)

/*
 * Primitive mutex operations.  Note: the "Lock" and "Unlock"
 * operations are not "suspend-safe".
 */
#define	_ILMutexCreate(mutex)	\
			do { \
				InitializeCriticalSection((mutex)); \
			} while (0)
#define	_ILMutexDestroy(mutex)	\
			do { \
				DeleteCriticalSection((mutex)); \
			} while (0)
#define	_ILMutexLockUnsafe(mutex)	\
			do { \
				EnterCriticalSection((mutex)); \
			} while (0)
#define	_ILMutexUnlockUnsafe(mutex)	\
			do { \
				LeaveCriticalSection((mutex)); \
			} while (0)

/*
 * Primitive condition mutex operations.  These are similar to
 * normal mutexes, except that they can be used with condition
 * variables to do an atomic "unlock and wait" operation.
 */
#define	_ILCondMutexCreate(mutex)	\
			({ \
				*(mutex) = CreateMutex(NULL, FALSE, NULL); \
				*(mutex) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN; \
			})
#define	_ILCondMutexCreateOwned(mutex)	\
			({ \
				*(mutex) = CreateMutex(NULL, TRUE, NULL); \
				*(mutex) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN; \
			})
#define	_ILCondMutexDestroy(mutex)	_ILWaitHandleClose(*(mutex))
#define	_ILCondMutexLockUnsafe(mutex)	_ILWaitHandleWait(*(mutex))
#define	_ILCondMutexTryLockUnsafe(mutex)	_ILWaitHandleTimedWait(*(mutex), 0)
#define	_ILCondMutexUnlockUnsafe(mutex)	(ReleaseMutex(*(mutex)) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_SYNCLOCK)

/*
 * Try to lock a condition mutex wor a given amount of time.
 * Returns IL_THREAD_OK if the mutex could be acquired, IL_THREAD_BUSY if the
 * operation timed out, IL_THREAD_ERR_INVALID_TIMEOUT if a negative timeout not
 * equal to IL_MAX_UINT32 was supplied or IL_THREAD_ERR_UNKNOWN on every other
 * error.
 */
#define _ILCondMutexTimedLockUnsafe(mutex, ms)	_ILWaitHandleTimedWait((mutex), (ms))

/*
 * Primitive read/write lock operations.  Note: the "Lock" and
 * "Unlock" operations are not "suspend-safe".
 */
#define	_ILRWLockCreate(rwlock)				_ILMutexCreate((rwlock))
#define	_ILRWLockDestroy(rwlock)			_ILMutexDestroy((rwlock))
#define	_ILRWLockReadLockUnsafe(rwlock)		_ILMutexLockUnsafe((rwlock))
#define	_ILRWLockWriteLockUnsafe(rwlock)	_ILMutexLockUnsafe((rwlock))
#define	_ILRWLockUnlockUnsafe(rwlock)		_ILMutexUnlockUnsafe((rwlock))

/*
 * Primitive semaphore operations.
 */
#define	_ILSemaphoreCreate(sem)	\
			({ \
				*(sem) = CreateSemaphore(NULL, 0, 0x7FFFFFFF, NULL); \
				*(sem) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN; \
			})
#define	_ILSemaphoreDestroy(sem)	_ILWaitHandleClose(*(sem))
#define	_ILSemaphoreWait(sem)	_ILWaitHandleWait(*(sem))
#define	_ILSemaphorePost(sem)	\
	(ReleaseSemaphore(*(sem), 1, NULL) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN)
#define	_ILSemaphorePostMultiple(sem, count)	\
	(ReleaseSemaphore(*(sem), (count), NULL) != 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN)

/*
 * Primitive condition variable operations.
 */
#define	_ILCondVarCreate(cond)		_ILSemaphoreCreate((cond))
#define	_ILCondVarDestroy(cond)		_ILSemaphoreDestroy((cond))
#define	_ILCondVarSignal(cond)		_ILSemaphorePost((cond))
int _ILCondVarTimedWait(_ILCondVar *cond, _ILCondMutex *mutex, ILUInt32 ms);

/*
 * Primitive counting semaphore operations.
 */
#define _ILCountSemaphoreCreate(sem)	\
		({ \
			(sem)->_waiting = 0; \
			_ILCriticalSectionCreate(&((sem)->_lock)); \
			_ILSemaphoreCreate(&(sem)->_sem);	\
		})
#define _ILCountSemaphoreDestroy(sem)	\
		({ \
			_ILCriticalSectionDestroy(&((sem)->_lock)); \
			_ILSemaphoreDestroy(&((sem)->_sem)); \
		})

/*
 * Release a number of waiting threads from the count semaphore.
 */
int _ILCountSemaphoreSignalCount(_ILCountSemaphore *sem, ILUInt32 count);

/*
 * Release all waiting threads from the count semaphore.
 */
int _ILCountSemaphoreSignalAll(_ILCountSemaphore *sem);

#define _ILCountSemaphoreSignal(sem)	_ILCountSemaphoreSignalCount((sem), 1)

/*
 * Wait on a count semaphore.
 */
int _ILCountSemaphoreTimedWait(_ILCountSemaphore *sem, ILUInt32 ms);

#define _ILCountSemaphoreWait(sem)	_ILCountSemaphoreTimedWait((sem), IL_MAX_UINT32)
#define _ILCountSemaphoreTryWait(sem)	_ILCountSemaphoreTimedWait((sem), 0)

/*
 * Primitive monitor operations.
 */
#define	_ILMonitorCreate(mon, result)	\
		do { \
			(mon)->_waitValue = 0; \
			if(((result) = _ILCondMutexCreateOwned(&((mon)->_mutex))) == IL_THREAD_OK) \
			{ \
				(result) = _ILSemaphoreCreate(&((mon)->_sem)); \
			} \
			if((result) != IL_THREAD_OK) \
			{ \
				(result) = IL_THREAD_ERR_UNKNOWN; \
			} \
		} while(0)
#define	_ILMonitorDestroy(mon, result)	\
		do { \
			if(((result) = _ILSemaphoreDestroy(&((mon)->_sem))) == IL_THREAD_OK) \
			{ \
				(result) = _ILCondMutexDestroy(&((mon)->_mutex)); \
			} \
			if((result) != IL_THREAD_OK) \
			{ \
				(result) = IL_THREAD_ERR_UNKNOWN; \
			} \
		} while(0)

/*
 * Release one waiter from the waiting queue.
 */
int _ILMonitorPulse(_ILMonitor *mon);

/*
 * Release all waiters from the waiting queue.
 */
int	_ILMonitorPulseAll(_ILMonitor *mon);

int _ILMonitorTimedTryEnter(_ILMonitor *mon, ILUInt32 ms);
#define _ILMonitorTryEnter(mon)	_ILMonitorTimedTryEnter(mon, 0)
#define _ILMonitorEnter(mon)	_ILMonitorTimedTryEnter(mon, IL_MAX_UINT32)
#define	_ILMonitorExit(mon)		_ILCondMutexUnlockUnsafe(&((mon)->_mutex))
int _ILMonitorTimedWait(_ILMonitor *mon, ILUInt32 ms);
#define _ILMonitorWait(mon)	_ILMonitorTimedWait((mon), IL_MAX_UINT32)
/*
 * Operations for acquiring and releasing a monitor where it is guaranteed
 * that no other thread is trying to enter or waiting on the monitor.
 * They are used for managing thread local freelists.
 */
#define _ILMonitorAcquire(mon)		_ILMonitorTryEnter(mon)
#define _ILMonitorRelease(mon)		_ILMonitorExit(mon)

/*
 * Get or set the thread object that is associated with "self".
 */
#if defined(USE_COMPILER_TLS)
extern _THREAD_ ILThread *_myThread;
#define	_ILThreadGetSelf()			(_myThread)
#define	_ILThreadSetSelf(object)	(_myThread = (object))
#else
extern DWORD _ILThreadObjectKey;
#define	_ILThreadGetSelf()	\
			((ILThread *)(TlsGetValue(_ILThreadObjectKey)))
#define	_ILThreadSetSelf(object)	\
			(TlsSetValue(_ILThreadObjectKey, (object)))
#endif

/*
 * Call a function "once".
 */
#define	_ILCallOnce(func)	\
			do { \
				static LONG volatile __once = 0; \
				if(!InterlockedExchange((PLONG)&__once, 1)) \
				{ \
					(*(func))(); \
				} \
			} while (0)

#define _ILThreadYield() Sleep(0)

#ifdef	__cplusplus
};
#endif

#endif	/* _W32_DEFS_H */
