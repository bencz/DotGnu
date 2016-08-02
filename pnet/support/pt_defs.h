/*
 * pt_defs.h - Thread definitions for using pthreads.
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

#ifndef	_PT_DEFS_H
#define	_PT_DEFS_H

#include <errno.h>
#include <semaphore.h>
#include <signal.h>
#ifdef HAVE_SETJMP_H
#include <setjmp.h>
#endif
#ifdef GC_DARWIN_THREADS
#include <mach/mach.h>
#include <mach/thread_act.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * This is a real thread package.
 */
#define	_ILThreadIsReal		1

/*
 * Define IL_USE_COND_LOCK if condition variable based locks should be used.
 * They are used by default if sem_timedwait is not available.
 * This has to be done if the semaphore implementation on the OS doesn't
 * return with errno set to EINTR if the waiting thread was interrupted by
 * a signal (like on various BSD implementations).
 */
/* #define IL_USE_COND_LOCK 1 */

/*
 * Determine the minimum and maximum real-time signal numbers.
 */
#if !defined(__SIGRTMIN) && defined(SIGRTMIN)
#define	__SIGRTMIN			SIGRTMIN
#endif
#if !defined(__SIGRTMAX) && defined(SIGRTMAX)
#define	__SIGRTMAX			SIGRTMAX
#endif

/*
 * Signals that are used to bang threads on the head and notify
 * them of various conditions.  Finding a free signal is a bit
 * of a pain as most of the obvious candidates are already in
 * use by pthreads or libgc.  Unix needs more signals!
 * NOTE: libgc is using either SIGUSR1 and SIGUSR2 or
 * SIGRTMIN + 5 and SIGRTMIN + 6 for SIG_INTERRUPT or SIG_THR_RESTART.
 * Linux NTPL is using the first two and LinuxThreads the first three
 * real time signals.
 */
#if defined(__sun)
#define	IL_SIG_SUSPEND		(__SIGRTMIN+0)
#define	IL_SIG_RESUME		(__SIGRTMIN+1)
#define	IL_SIG_ABORT		(__SIGRTMIN+2)
#define	IL_SIG_INTERRUPT	(__SIGRTMIN+3)
#elif !defined(__SIGRTMIN) || (__SIGRTMAX - __SIGRTMIN < 14)
#define	IL_SIG_SUSPEND		SIGALRM
#define	IL_SIG_RESUME		SIGVTALRM
#define	IL_SIG_ABORT		SIGFPE
#define	IL_SIG_INTERRUPT	SIGUSR1
#else
#define	IL_SIG_SUSPEND		(__SIGRTMIN+10)
#define	IL_SIG_RESUME		(__SIGRTMIN+11)
#define	IL_SIG_ABORT		(__SIGRTMIN+12)
#define	IL_SIG_INTERRUPT	(__SIGRTMIN+13)
#endif

/*
 * Signals that are used inside pthreads itself.  This is a bit of
 * a hack, but there isn't any easy way to get this information.
 */
#if defined(SIGCANCEL)
#define PTHREAD_SIG_CANCEL	SIGCANCEL
#elif !defined(__SIGRTMIN) || (__SIGRTMAX - __SIGRTMIN < 3)
#define PTHREAD_SIG_CANCEL	SIGUSR2
#else
#define PTHREAD_SIG_CANCEL	(__SIGRTMIN+1)
#endif

/*
 * Determine if we have read-write lock support in pthreads.
 */
#if defined(PTHREAD_RWLOCK_INITIALIZER) && defined(__USE_UNIX98)
	#define	IL_HAVE_RWLOCKS
#endif

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
 * Get or set the thread object that is associated with "self".
 */
#if defined(USE_COMPILER_TLS)
extern _THREAD_ ILThread *_myThread;
#define	_ILThreadGetSelf()			(_myThread)
#define	_ILThreadSetSelf(object)	(_myThread = (object))
#else
extern pthread_key_t _ILThreadObjectKey;
#define	_ILThreadGetSelf()	\
			((ILThread *)(pthread_getspecific(_ILThreadObjectKey)))
#define	_ILThreadSetSelf(object)	\
			(pthread_setspecific(_ILThreadObjectKey, (object)))
#endif

/*
 * Types that are needed elsewhere.
 */
typedef pthread_mutex_t		_ILCriticalSection;
typedef pthread_mutex_t		_ILMutex;
typedef pthread_mutex_t		_ILCondMutex;
typedef pthread_cond_t		_ILCondVar;
typedef pthread_t			_ILThreadHandle;
#ifdef GC_DARWIN_THREADS
#define IL_THREAD_NEED_IDENTIFIER 1
typedef mach_port_t			_ILThreadIdentifier;
#endif
typedef sem_t				_ILSemaphore;
#ifdef IL_HAVE_RWLOCKS
typedef pthread_rwlock_t	_ILRWLock;
#else
typedef pthread_mutex_t		_ILRWLock;
#endif

/*
 * sigsetjmp and siglongjmp are not recognized by configure in all cases
 * because they might be implemented by a macro (like on Linux)
 * So we check for macros here if HAVE_SIGSETJMP or HAVE_SIGLONGJMP are
 * not defined.
 */
#ifndef HAVE_SIGSETJMP
#if defined(sigsetjmp) || defined(HAVE___SIGSETJMP)
#define HAVE_SIGSETJMP 1
#endif
#endif

#ifndef HAVE_SIGLONGJMP
#ifdef siglongjmp
#define HAVE_SIGLONGJMP 1
#endif
#endif

/*
 * Check if we can implement thread interrupts using sigsetjmp and siglongjmp
 */
#if defined(HAVE_SIGSETJMP) && defined(HAVE_SIGLONGJMP) && \
	!defined(__NetBSD__)
#define _IL_PT_INTERRUPT_JMP
#endif

/*
 * Check which type of lock to use on this architecture
 */
#if !defined(IL_USE_COND_LOCK) && defined(HAVE_SEM_TIMEDWAIT) && defined(_IL_PT_INTERRUPT_JMP)
#define _IL_PT_LOCK_SEM 1
#else
#ifdef _IL_PT_INTERRUPT_JMP
#undef _IL_PT_INTERRUPT_JMP
#endif
#define _IL_PT_LOCK_COND 1
/*
 * An additional flag in the private thread state part to indicate that the
 * thread was woken up because of an interruption of an other thread waiting
 * on the same lock.
 * NOTE: We extend the modifiability of the private thread state part by
 * foreign threads here if the following conditions are met:
 * 1. The thread whose private thread state shall be modified must be waiting
 *    on a lock. (-> the member lockWaitingOn is != 0).
 * 2. The thread wanting to modify the private state of the other thread
 *    *MUST* hold the lockWaitingOn lock.
 */
#define IL_TS_NOTINTERRUPTED	0x0800
#endif

/*
 * Define the type of lock to use.
 */
#if defined(_IL_PT_LOCK_SEM)
typedef struct
{
	_ILSemaphore		_sem;
} _ILLock;
#elif defined(_IL_PT_LOCK_COND)
typedef struct
{
	_ILCondVar			_cond;
	_ILCondMutex		_mutex;
	ILThread		   *_waiters;
	ILInt32				_value;
	ILInt32				_interrupted;
} _ILLock;
#else
#error "No lock type defined for this architecture"
#endif

/*
 * pthread specific extensions for an ILThread
 */
#if defined(_IL_PT_INTERRUPT_JMP)
#define _IL_THREAD_EXT \
	sigjmp_buf			interruptJmpBuf;
#elif defined(_IL_PT_LOCK_COND)
#define _IL_THREAD_EXT \
	_ILLock			   *lockWaitingOn; \
	ILThread		   *nextWaiter;
#endif

/*
 * Primitive semaphore operations.
 */
#define _ILSemaphoreCreate(sem)		(sem_init((sem), 0, 0))
#define _ILSemaphoreDestroy(sem)	(sem_destroy((sem)))
#define _ILSemaphoreWait(sem) \
	do { \
		while(sem_wait((sem)) == -1 && errno == EINTR) \
		{ \
			continue; \
		} \
	} while(0)
#define _ILSemaphorePost(sem)		(sem_post((sem)))
int _ILSemaphorePostMultiple(_ILSemaphore *sem, ILUInt32 count);

/*
 * Semaphore which allows to release multiple or all waiters.
 * The semantic for posix and windows semaphores is the same and the
 * implementation follows this semantics.
 * This means:
 * The value is decremented each time a thread tries to wait on the
 * semaphore. If the count is positive at the time the thread calls
 * one of the wait functions the thread is not blocked.
 * The value is incremented by one or the number given if the one of the
 * Signal functions is called.
 * This means that if the member _value is negative the absolute value of
 * _value indicates the number of blocked threads.
 */
typedef struct
{
	_ILSemaphore		_sem;
	ILInt32 volatile	_value;
} _ILCountSemaphore;

typedef struct
{
	ILInt32			_waitValue;
	_ILLock			_waitLock;
	_ILLock			_enterLock;
} _ILMonitor;

/*
 * Determine if a thread corresponds to "self".
 */
#define	_ILThreadIsSelf(thread)	\
			(pthread_equal((thread)->handle, pthread_self()))

/*
 * Suspend and resume threads.  Note: these are the primitive
 * versions, which are not "suspend-safe".
 */
#ifdef GC_DARWIN_THREADS
#define	_ILThreadSuspendOther(thread)	\
			do { \
				thread_suspend((thread)->identifier); \
			} while (0)
#define	_ILThreadResumeOther(thread)	\
			do { \
				thread_resume((thread)->identifier); \
			} while (0)
#define	_ILThreadSuspendSelf(thread)	\
			do { \
				_ILThreadSuspendUntilResumed((thread)); \
				_ILSemaphorePost(&((thread)->resumeAck)); \
			} while (0)
#define	_ILThreadResumeSelf(thread)	\
			do { \
				(thread)->resumeRequested = 1; \
				pthread_kill((thread)->handle, IL_SIG_RESUME); \
				_ILSemaphoreWait(&((thread)->resumeAck)); \
			} while (0)
#else
#define	_ILThreadSuspendOther(thread)	\
			do { \
				pthread_kill((thread)->handle, IL_SIG_SUSPEND); \
				_ILSemaphoreWait(&((thread)->suspendAck)); \
			} while (0)
#define	_ILThreadResumeOther(thread)	\
			do { \
				(thread)->resumeRequested = 1; \
				pthread_kill((thread)->handle, IL_SIG_RESUME); \
				_ILSemaphoreWait(&((thread)->resumeAck)); \
			} while (0)
#endif
#define	_ILThreadSuspendSelf(thread)	\
			do { \
				_ILThreadSuspendUntilResumed((thread)); \
				_ILSemaphorePost(&((thread)->resumeAck)); \
			} while (0)
#define	_ILThreadResumeSelf(thread)	\
			do { \
				(thread)->resumeRequested = 1; \
				pthread_kill((thread)->handle, IL_SIG_RESUME); \
				_ILSemaphoreWait(&((thread)->resumeAck)); \
			} while (0)

/*
 * Suspend the current thread until it is resumed.
 */
void _ILThreadSuspendUntilResumed(ILThread *thread);

/*
 * Terminate a running thread.
 */
#define	_ILThreadTerminate(thread)	\
			do { \
				pthread_cancel((thread)->handle); \
			} while (0)

/*
 * Destroy a thread handle that is no longer required.
 */
#define	_ILThreadDestroy(thread)	do { ; } while (0)

/*
 * Interrupt a thread in the wait/sleep/join state or when it enters the
 * wait/sleep/join state the next time.
 */
void _ILThreadInterrupt(ILThread *thread);

/*
 * Put the current thread to sleep for a number of milliseconds.
 * The sleep may be interrupted by a call to _ILThreadInterrupt.
 */
int _ILThreadSleep(ILUInt32 ms);

/*
 * The default mutex attribute for critical sections and mutexes.
 * The attribute defaults to a fast mutex that doesn't do error checking
 * and doesn't allow recursive locking.
 */
extern pthread_mutexattr_t _ILMutexAttr;

/*
 * Primitive critical section operations.
 * NOTE: The "EnterUnsafe" and "LeaveUnsafe" operations are not "suspend-safe"
 */
#define _ILCriticalSectionCreate(critsect)	\
			(pthread_mutex_init((critsect), &_ILMutexAttr))
#define _ILCriticalSectionDestroy(critsect)	\
			(pthread_mutex_destroy((critsect)))
#define _ILCriticalSectionEnterUnsafe(critsect)	\
			(pthread_mutex_lock((critsect)))
#define _ILCriticalSectionLeaveUnsafe(critsect)	\
			(pthread_mutex_unlock((critsect)))

/*
 * Primitive mutex operations.  Note: the "Lock" and "Unlock"
 * operations are not "suspend-safe".
 */
#define	_ILMutexCreate(mutex)	\
			(pthread_mutex_init((mutex), &_ILMutexAttr))
#define	_ILMutexDestroy(mutex)	\
			(pthread_mutex_destroy((mutex)))
#define	_ILMutexLockUnsafe(mutex)	\
			(pthread_mutex_lock((mutex)))
#define	_ILMutexUnlockUnsafe(mutex)	\
			(pthread_mutex_unlock((mutex)))

/*
 * Primitive condition mutex operations.  These are similar to
 * normal mutexes, except that they can be used with condition
 * variables to do an atomic "unlock and wait" operation.
 */
#define	_ILCondMutexCreate(mutex)		_ILMutexCreate((mutex))
#define	_ILCondMutexDestroy(mutex)		_ILMutexDestroy((mutex))
#define	_ILCondMutexLockUnsafe(mutex)	_ILMutexLockUnsafe((mutex))
#define	_ILCondMutexTryLockUnsafe(mutex)	\
			(pthread_mutex_trylock((mutex)))
#define	_ILCondMutexUnlockUnsafe(mutex)	_ILMutexUnlockUnsafe((mutex))

/*
 * Try to lock a condition mutex wor a given amount of time.
 * Returns IL_THREAD_OK if the mutex could be acquired, IL_THREAD_BUSY if the
 * operation timed out, IL_THREAD_ERR_INVALID_TIMEOUT if a negative timeout not
 * equal to IL_MAX_UINT32 was supplied or IL_THREAD_ERR_UNKNOWN on every other
 * error.
 */
int _ILCondMutexTimedLockUnsafe(_ILCondMutex *mutex, ILUInt32 ms);

/*
 * Primitive read/write lock operations.  Note: the "Lock" and
 * "Unlock" operations are not "suspend-safe".
 */
#ifdef IL_HAVE_RWLOCKS
#define	_ILRWLockCreate(rwlock)	\
			(pthread_rwlock_init((rwlock), (pthread_rwlockattr_t *)0))
#define	_ILRWLockDestroy(rwlock)	\
			(pthread_rwlock_destroy((rwlock)))
#define	_ILRWLockReadLockUnsafe(rwlock)	\
			(pthread_rwlock_rdlock((rwlock)))
#define	_ILRWLockWriteLockUnsafe(rwlock)	\
			(pthread_rwlock_wrlock((rwlock)))
#define	_ILRWLockUnlockUnsafe(rwlock)	\
			(pthread_rwlock_unlock((rwlock)))
#else
#define	_ILRWLockCreate(rwlock)				(_ILMutexCreate((rwlock)))
#define	_ILRWLockDestroy(rwlock)			(_ILMutexDestroy((rwlock)))
#define	_ILRWLockReadLockUnsafe(rwlock)		(_ILMutexLockUnsafe((rwlock)))
#define	_ILRWLockWriteLockUnsafe(rwlock)	(_ILMutexLockUnsafe((rwlock)))
#define	_ILRWLockUnlockUnsafe(rwlock)		(_ILMutexUnlockUnsafe((rwlock)))
#endif

/*
 * Primitive condition variable operations.
 */
#define	_ILCondVarCreate(cond)		\
			(pthread_cond_init((cond), (pthread_condattr_t *)0))
#define	_ILCondVarDestroy(cond)		\
			(pthread_cond_destroy((cond)))
#define	_ILCondVarSignal(cond)		\
			(pthread_cond_signal((cond)))
#define	_ILCondVarSignalAll(cond)		\
			(pthread_cond_broadcast((cond)))
int _ILCondVarTimedWait(_ILCondVar *cond, _ILCondMutex *mutex, ILUInt32 ms);

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

#if defined(_IL_PT_LOCK_SEM)
#define _ILLockCreate(lock, result) \
	do { \
		(result) = (sem_init(&((lock)->_sem), 0, 0) == 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN); \
	} while(0)
#define _ILLockDestroy(lock, result) \
	do { \
		(result) = (sem_destroy(&((lock)->_sem)) == 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN); \
	} while(0)
#elif defined(_IL_PT_LOCK_COND)
#define _ILLockCreate(lock, result) \
	do { \
		(lock)->_waiters = 0; \
		(lock)->_value = 0; \
		(lock)->_interrupted = 0; \
		if(!_ILCondMutexCreate(&((lock)->_mutex))) \
		{ \
			if(!_ILCondVarCreate((&((lock)->_cond)))) \
			{ \
				(result) = IL_THREAD_OK; \
			} \
			else \
			{ \
				(result) = IL_THREAD_ERR_UNKNOWN; \
			} \
		} \
		else \
		{ \
			(result) = IL_THREAD_ERR_UNKNOWN; \
		} \
	} while(0)
#define _ILLockDestroy(lock, result) \
	do { \
		if(!_ILCondMutexDestroy(&((lock)->_mutex))) \
		{ \
			if(!_ILCondVarDestroy((&((lock)->_cond)))) \
			{ \
				(result) = IL_THREAD_OK; \
			} \
			else \
			{ \
				(result) = IL_THREAD_ERR_UNKNOWN; \
			} \
		} \
		else \
		{ \
			(result) = IL_THREAD_ERR_UNKNOWN; \
		} \
	} while(0)
#endif /* _IL_PT_LOCK_COND) */

/*
 * Primitive monitor operations.
 */
#define	_ILMonitorCreate(mon, result) \
		do { \
			(mon)->_waitValue = 0; \
			_ILLockCreate(&((mon)->_waitLock), (result)); \
			if(!(result)) \
			{ \
				_ILLockCreate(&((mon)->_enterLock), (result)); \
			} \
		} while(0)
#define	_ILMonitorDestroy(mon, result) \
		do { \
			_ILLockDestroy(&((mon)->_waitLock),(result)); \
			if(!(result)) \
			{ \
				_ILLockDestroy(&((mon)->_enterLock), (result)); \
			} \
		} while(0)
int _ILMonitorPulse(_ILMonitor *mon);
int _ILMonitorPulseAll(_ILMonitor *mon);
int _ILMonitorTimedTryEnter(_ILMonitor *mon, ILUInt32 ms);
#define _ILMonitorTryEnter(mon) _ILMonitorTimedTryEnter((mon), 0)
#define _ILMonitorEnter(mon) _ILMonitorTimedTryEnter((mon), IL_MAX_UINT32)
int _ILMonitorExit(_ILMonitor *mon);
int _ILMonitorTimedWait(_ILMonitor *mon, ILUInt32 ms);
#define _ILMonitorWait(mon) _ILMonitorTimedWait((mon), IL_MAX_UINT32)
/*
 * Operations for acquiring and releasing a monitor where it is guaranteed
 * that no other thread is trying to enter or waiting on the monitor.
 * They are used for managing thread local freelists.
 */
#define _ILMonitorAcquire(mon)	IL_THREAD_OK
#define _ILMonitorRelease(mon)	IL_THREAD_OK

/*
 * Call a function "once".
 */
#define	_ILCallOnce(func)	\
			do { \
				static pthread_once_t __once = PTHREAD_ONCE_INIT; \
				pthread_once(&__once, (func)); \
			} while (0)


#ifdef _POSIX_PRIORITY_SCHEDULING
	#define _ILThreadYield() sched_yield()
#else
	#define _ILThreadYield()
#endif
	
		
#ifdef	__cplusplus
};
#endif

#endif	/* _PT_DEFS_H */
