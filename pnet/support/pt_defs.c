/*
 * pt_defs.c - Thread definitions for using pthreads.
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

#include "il_thread.h"
#include "thr_defs.h"
#include "interlocked.h"
#if TIME_WITH_SYS_TIME
	#include <sys/time.h>
    #include <time.h>
#else
    #if HAVE_SYS_TIME_H
		#include <sys/time.h>
    #elif !defined(__palmos__)
        #include <time.h>
    #endif
#endif
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#include <errno.h>

#ifdef IL_USE_PTHREADS

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Signal mask that has to be unblocked if thread interrupts have to be
 * enabled.
 */
static sigset_t		_interruptSet;

/*
 * Signal mask to wait on if a thtead is suspended.
 */
static sigset_t		_suspendSet;

/*
 * Choose the version to use for modifying the counter in a count semaphore
 */
#define _SemAdd(dest, value)		ILInterlockedAddI4_Acquire((dest), (value))
#define _SemSub(dest, value)		ILInterlockedSubI4_Acquire((dest), (value))
#define _SemIncrement(dest)			ILInterlockedIncrementI4_Acquire((dest))
#define _SemDecrement(dest)			ILInterlockedDecrementI4_Acquire((dest))
#define _SemExchange(dest, value)	ILInterlockedExchangeI4_Acquire((dest), (value))

/*
 * Macros to run code in the current thread with the signal 
 * IL_SIG_INTERRUPT enabled.
 */

/*
 * This macro checks if an interrupt orrured after enabling the
 * IL_SIG_INTERRUPT signal and sets __result in this case without
 * entering the code that has to be executed.
 * NOTE: There is a possibility that the interrupt is missed if it happens
 * after enabling the signal but before the function that should be
 * interrupted is able to catch the signal.
 * Maybe we have to change this so that the signel handler is left with a
 * siglongjmp later. This would remove the need for the IL_TS_INTERRUPTED
 * flag too.
 */
#ifdef _IL_PT_INTERRUPT_JMP
/*
 * Special flag in the private threadstate to indicate that the interrupt
 * should be noticed in the signal handler but the longjmp should not be taken.
 */
#define IL_TS_UNINTERRUPTIBLE 0x1000

#define IL_START_INTERRUPTIBLE(__result) \
		{ \
			ILThread *__myThread; \
			__myThread = _ILThreadSelf(); \
			if(sigsetjmp(__myThread->interruptJmpBuf, 1) != 0) \
			{ \
				__result = IL_THREAD_ERR_INTERRUPT; \
			} \
			else \
			{ \
				sigset_t __oldset; \
				pthread_sigmask(SIG_UNBLOCK, &_interruptSet, &__oldset); \
				{

#define IL_END_INTERRUPTIBLE(__result) \
				} \
				pthread_sigmask(SIG_SETMASK, &__oldset, 0); \
			} \
		}

/*
 * This macro enables the IL_SIG_INTERRUPT signal and doesn't check if an
 * interrupt occured.
 */
#define IL_START_UNINTERRUPTIBLE() \
		{ \
			ILThread *__myThread; \
			ILUInt16 __threadState; \
			sigset_t __oldset; \
			__myThread = _ILThreadSelf(); \
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv)); \
			__threadState |= IL_TS_UNINTERRUPTIBLE; \
			ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState); \
			pthread_sigmask(SIG_UNBLOCK, &_interruptSet, &__oldset); \
			{ 

#define IL_END_UNINTERRUPTIBLE(__result) \
			} \
			pthread_sigmask(SIG_SETMASK, &__oldset, 0); \
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv)); \
			if((__threadState & IL_TS_INTERRUPTED) != 0) \
			{ \
				__result = IL_THREAD_ERR_INTERRUPT; \
			} \
			__threadState &= ~(IL_TS_UNINTERRUPTIBLE | IL_TS_INTERRUPTED); \
			ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState); \
		}

#else /* !_IL_PT_INTERRUPT_JMP */

#define IL_START_INTERRUPTIBLE(__result) \
		{ \
			sigset_t __oldset; \
			ILThread *__myThread; \
			ILUInt16 __threadState; \
			__myThread = _ILThreadSelf(); \
			pthread_sigmask(SIG_UNBLOCK, &_interruptSet, &__oldset); \
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv)); \
			if((__threadState & IL_TS_INTERRUPTED) != 0) \
			{ \
				__result = IL_THREAD_ERR_INTERRUPT; \
			}  \
			else \
			{

#define IL_END_INTERRUPTIBLE(__result) \
			} \
			pthread_sigmask(SIG_SETMASK, &__oldset, 0); \
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv)); \
			if((__threadState & IL_TS_INTERRUPTED) != 0) \
			{ \
				__threadState &= ~IL_TS_INTERRUPTED; \
				ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState); \
				__result = IL_THREAD_ERR_INTERRUPT; \
			} \
		}

/*
 * This macro enables the IL_SIG_INTERRUPT signal and doesn't check if an
 * interrupt occured.
 */
#define IL_START_UNINTERRUPTIBLE() \
		{ \
			sigset_t __oldset; \
			ILThread *__myThread; \
			ILUInt16 __threadState; \
			__myThread = _ILThreadSelf(); \
			pthread_sigmask(SIG_UNBLOCK, &_interruptSet, &__oldset); \
			{ 

#define IL_END_UNINTERRUPTIBLE(__result) \
			} \
			pthread_sigmask(SIG_SETMASK, &__oldset, 0); \
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv)); \
			if((__threadState & IL_TS_INTERRUPTED) != 0) \
			{ \
				__threadState &= ~IL_TS_INTERRUPTED; \
				ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState); \
				__result = IL_THREAD_ERR_INTERRUPT; \
			} \
		}

#endif /* !_IL_PT_INTERRUPT_JMP */

#if defined(USE_COMPILER_TLS)
/*
 * Define the thread local variable to hold the ILThread value for the
 * current thread.
 */
_THREAD_ ILThread *_myThread = 0;
#else
/*
 * Thread-specific key that is used to store and retrieve thread objects.
 */
pthread_key_t _ILThreadObjectKey;
#endif

/*
 * Default mutex attribute.
 */
pthread_mutexattr_t _ILMutexAttr;

/*
 * Suspend until we receive IL_SIG_RESUME from another thread
 * in this process.
 *
 * This implementation currently assumes a Linux-like threads
 * implementation that uses one process per thread, and may not
 * work on other pthreads implementations.
 *
 * The "SIG_SUSPEND" signal is used by the garbage collector to stop
 * the world for garbage collection, and so we must not block it.
 *
 * The "PTHREAD_SIG_CANCEL" signal is used by pthreads itself to
 * cancel running threads, and must not be blocked by us.
 */
void _ILThreadSuspendUntilResumed(ILThread *thread)
{
	/* Suspend until we receive IL_SIG_RESUME from something in this process */
	do
	{
		sigsuspend(&_suspendSet);
	}
	while(!(thread->resumeRequested));
	thread->resumeRequested = 0;
}

/*
 * Signal handler for IL_SIG_SUSPEND.
 */
static void SuspendSignal(int sig)
{
	ILThread *thread = _ILThreadGetSelf();

	/* Tell the "ILThreadSuspend" function that we are now suspended */
	_ILSemaphorePost(&(thread->suspendAck));

	/* Suspend until we are resumed by some other thread */
	_ILThreadSuspendUntilResumed(thread);

	/* Tell the "ILThreadResume" function that we have now resumed */
	_ILSemaphorePost(&(thread->resumeAck));
}

/*
 * Signal handler for IL_SIG_RESUME.
 */
static void ResumeSignal(int sig)
{
	/* There is nothing to do here - "_ILThreadSuspendUntilResumed"
	   will be interrupted in the call to "sigsuspend", and then
	   will test the "resumeRequested" flag to detect the request */
}

/*
 * Signal handler for IL_SIG_INTERRUPT
 */
static void InterruptSignal(int sig)
{
	ILThread *thread = _ILThreadGetSelf();
	ILUInt16 threadState;

	threadState = ILInterlockedLoadU2(&(thread->state.split.priv));
#ifdef _IL_PT_INTERRUPT_JMP
	if((threadState & IL_TS_UNINTERRUPTIBLE) == 0)
	{
		siglongjmp(thread->interruptJmpBuf, IL_THREAD_ERR_INTERRUPT);
	}
#endif
	/* Set the interrupted flag */
	threadState |= IL_TS_INTERRUPTED;
	ILInterlockedStoreU2(&(thread->state.split.priv), threadState);
}

/*
 * Signal handler for IL_SIG_ABORT.
 */
static void AbortSignal(int sig)
{
	/* There is nothing to do here - this signal exists purely
	   to force system calls to exit with EINTR */
}

/*
 * Define some functions from the Timeouts options that need not be provided
 * in all implementations.
 */

#ifndef HAVE_PTHREAD_MUTEX_TIMEDLOCK
int pthread_mutex_timedlock (pthread_mutex_t *mutex,
			    const struct timespec *abs_timeout)
{
	int retcode;
#ifdef HAVE_NANOSLEEP
	struct timespec ts;
	
	/* To avoid a completely busy wait we sleep for 10 ms between two tries. */
	/* As a consequence the resolution is at most 10 ms instead of 1 ms. */
	ts.tv_sec = 0;
	ts.tv_nsec = 10000000;	/* 10ms */
#endif /* HAVE_NANOSLEEP */
	
	while((retcode = pthread_mutex_trylock(mutex)) == EBUSY)
	{
		struct timeval tv;

		gettimeofday(&tv, NULL);
		
		if((tv.tv_sec > abs_timeout->tv_sec) ||
		   (tv.tv_sec == abs_timeout->tv_sec &&
		    (tv.tv_usec * 1000) >= abs_timeout->tv_nsec))
		{
			return ETIMEDOUT;
		}
#ifdef HAVE_NANOSLEEP
		nanosleep(&ts, NULL);
#else /* !HAVE_NANOSLEEP */
#ifdef HAVE_USLEEP
			usleep(10000);	/* 10 ms */
#else /* !HAVE_USLEEP */
#error "neither nanosleep nor usleep are available on this system"
#endif /* !HAVE_USLEEP */
#endif /* !HAVE_NANOSLEEP */
	}
	return retcode;
}
#endif /* !HAVE_PTHREAD_MUTEX_TIMEDLOCK */

#ifndef HAVE_SEM_TIMEDWAIT
int sem_timedwait(sem_t *sem,
				  const struct timespec *abs_timeout)
{
#ifdef HAVE_NANOSLEEP
	struct timespec ts;
	
	/* To avoid a completely busy wait we sleep for 10 ms between two tries. */
	/* As a consequence the resolution is at most 10 ms instead of 1 ms. */
	ts.tv_sec = 0;
	ts.tv_nsec = 10000000;	/* 10ms */
#endif /* HAVE_NANOSLEEP */

	while(sem_trywait(sem) == -1)
	{
		if(errno == EAGAIN)
		{
			struct timeval tv;

			gettimeofday(&tv, NULL);
		
			if((tv.tv_sec > abs_timeout->tv_sec) ||
			   (tv.tv_sec == abs_timeout->tv_sec &&
			    (tv.tv_usec * 1000) >= abs_timeout->tv_nsec))
			{
				errno = ETIMEDOUT;
				return -1;
			}
#ifdef HAVE_NANOSLEEP
			if(nanosleep(&ts, NULL) != 0)
			{
				/* Looks like we got interrupted */
				return -1;
			}
#else /* !HAVE_NANOSLEEP */
#error "nanosleep is needed for implementing an interruptible sleep"
#endif /* !HAVE_NANOSLEEP */
		}
		else
		{
			return -1;
		}
	}
	return 0;
}
#endif	/* !HAVE_SEM_TIMEDWAIT */

#ifdef _IL_PT_LOCK_SEM
static int LockSignal(_ILLock *lock)
{
	return (sem_post(&(lock)->_sem) == 0 ? IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN);
}

static int LockSignalCount(_ILLock *lock, int count)
{
	while(count > 0)
	{
		sem_post(&(lock)->_sem);
		--count;
	}
	return IL_THREAD_OK;
}

static int LockTryWait(_ILLock *lock)
{
	int result;

	result = sem_trywait(&(lock->_sem));
	if(result == -1)
	{
		if(errno == EAGAIN)
		{
			return IL_THREAD_BUSY;
		}
		return IL_THREAD_ERR_UNKNOWN;
	}
	return IL_THREAD_OK;
}

/*
 * Interruptible wait on a semaphore.
 */
static int LockWaitInterruptible(_ILLock *lock)
{
	int result;

	IL_START_INTERRUPTIBLE(result)
	do
	{
		result = sem_wait(&(lock->_sem));
		if(result == 0)
		{
			break;
		}
		else if(errno == EINTR)
		{
#ifndef _IL_PT_INTERRUPT_JMP
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
			if((__threadState & IL_TS_INTERRUPTED) != 0)
			{
				break;
			}
#endif /* _IL_PT_INTERRUPT_JMP */
		}
		else
		{
			result = IL_THREAD_ERR_UNKNOWN;
			break;
		}
	} while(1);
	IL_END_INTERRUPTIBLE(result)
	return result;
}

/*
 * Interruptible timed wait on a semaphore.
 */
static int LockTimedWaitInterruptible(_ILLock *lock, const struct timespec *ts)
{
	int result;

	IL_START_INTERRUPTIBLE(result)
	do
	{
		result = sem_timedwait(&(lock->_sem), ts);
		if(result == 0)
		{
			break;
		}
		else if(errno == ETIMEDOUT)
		{
			result = IL_THREAD_BUSY;
			break;
		}
		else if(errno == EINTR)
		{
#ifndef _IL_PT_INTERRUPT_JMP
			__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
			if((__threadState & IL_TS_INTERRUPTED) != 0)
			{
				break;
			}
#endif /* _IL_PT_INTERRUPT_JMP */
		}
		else
		{
			result = IL_THREAD_ERR_UNKNOWN;
			break;
		}
	} while(1);
	IL_END_INTERRUPTIBLE(result)
	return result;
}

static int LockWaitUninterruptible(_ILLock *lock)
{
	int result = IL_THREAD_OK;

	IL_START_UNINTERRUPTIBLE()
	do
	{
		if(sem_wait(&(lock->_sem)) == 0)
		{
			break;
		}
		else if(errno == EINTR)
		{
			/* Ignore this one */
		}
		else
		{
			/* An unexpected error occured so bail out */
			result = IL_THREAD_ERR_UNKNOWN;
			break;
		}
	} while(1);
	IL_END_UNINTERRUPTIBLE(result)
	return result;
}
#elif defined(_IL_PT_LOCK_COND)
static int LockSignal(_ILLock *lock)
{
	_ILCondMutexLock(&(lock->_mutex));
	lock->_value += 1;
	if(lock->_value <= 0)
	{
		/* We have to release a waiter */
		if(lock->_interrupted)
		{
			/*
			 * See if there is a thread woken up by an interrupt of an other
			 * thread waiting on this lock
			 */
			ILThread *thread;

			thread = lock->_waiters;
			while(thread)
			{
				ILUInt16 threadState;

				threadState = ILInterlockedLoadU2(&(thread->state.split.priv));
				if((threadState & IL_TS_NOTINTERRUPTED) != 0)
				{
					/*
					 * Clear the not interrupted flag so that the thread
					 * will not continue waiting
					 */
					threadState &= ~IL_TS_NOTINTERRUPTED;
					ILInterlockedStoreU2(&(thread->state.split.priv), threadState);
					break;
				}
				thread = thread->nextWaiter;
			}
			if(!thread)
			{
				/*
				 * There was no thread waiting with the IL_TS_NOTINTERRUPTED
				 * flag set so clear the interropted flag of the lock.
				 */
				lock->_interrupted = 0;
				/* And signal a thread to wake up */
				_ILCondVarSignal(&(lock->_cond));
			}
		}
		else
		{
			_ILCondVarSignal(&(lock->_cond));
		}
	}
	_ILCondMutexUnlock(&(lock->_mutex));

	return IL_THREAD_OK;
}

static int LockSignalCount(_ILLock *lock, int count)
{
	_ILCondMutexLock(&(lock->_mutex));
	lock->_value += count;
	if(lock->_value > 0)
	{
		count -= lock->_value;
	}
	if(lock->_interrupted)
	{
		/*
		 * Process the threads woken up by interrupting an other thread
		 * and not being able to process the not interrupted first.
		 */
		ILThread *thread;

		thread = lock->_waiters;
		while(thread && (count > 0))
		{
			ILUInt16 threadState;

			threadState = ILInterlockedLoadU2(&(thread->state.split.priv));
			if((threadState & IL_TS_NOTINTERRUPTED) != 0)
			{
				/*
				 * Clear the not interrupted flag so that the thread
				 * will not continue waiting
				 */
				threadState &= ~IL_TS_NOTINTERRUPTED;
				ILInterlockedStoreU2(&(thread->state.split.priv), threadState);
				/* and decrement the number of waiters that should be awakened */
				--count;
				if(count <= 0)
				{
					break;
				}
			}
			thread = thread->nextWaiter;
		}
		if(!thread)
		{
			/*
			 * There are no more threads waiting with the IL_TS_NOTINTERRUPTED
			 * flag set so clear the interropted flag of the lock.
			 */
			lock->_interrupted = 0;
		}
	}
	if(lock->_value == 0)
	{
		/* Release all waiters */
		_ILCondVarSignalAll(&(lock->_cond));
	}
	else
	{
		/* Release the number of waiters */
		while(count > 0)
		{
			_ILCondVarSignal(&(lock->_cond));
			--count;
		}
	}
	_ILCondMutexUnlock(&(lock->_mutex));

	return IL_THREAD_OK;
}

static int LockTryWait(_ILLock *lock)
{
	int result;

	_ILCondMutexLock(&(lock->_mutex));
	if(lock->_value > 0)
	{
		lock->_value -= 1;
		result = IL_THREAD_OK;
	}
	else
	{
		result = IL_THREAD_BUSY;
	}
	_ILCondMutexUnlock(&(lock->_mutex));

	return result;
}

/*
 * Interruptible wait on a lock.
 */
static int LockWaitInterruptible(_ILLock *lock)
{
	int result = IL_THREAD_OK;

	IL_START_INTERRUPTIBLE(result)
	_ILCondMutexLock(&(lock->_mutex));
	lock->_value -= 1;
	if(lock->_value < 0)
	{
		/*
		 * Record this lock in the ILThread structure and add the thread to
		 * the waiters on this lock.
		 */
		__myThread->lockWaitingOn = lock;
		__myThread->nextWaiter = lock->_waiters;
		lock->_waiters = __myThread;
		do
		{
			result = pthread_cond_wait(&(lock->_cond), &(lock->_mutex));
			if(result == 0)
			{
				/* Check if we are interrupted */
				__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
				if((__threadState & IL_TS_INTERRUPTED) != 0)
				{
					/* Leave the wait loop  */
					break;
				}
				else if((__threadState & IL_TS_NOTINTERRUPTED) != 0)
				{
					/* Clear the not interrupted flag */
					__threadState &= ~IL_TS_NOTINTERRUPTED;
					ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState);
					/* And continue waiting */
					continue;
				}
				result = IL_THREAD_OK;
				break;
			}
			else
			{
				/* Remove me from the number of waiters */
				lock->_value += 1;
				result = IL_THREAD_ERR_UNKNOWN;
				break;
			}
		} while(1);
		/*
		 * Clear the  lock in the ILThread structure and remove the thread from
		 * the waiters on this lock.
		 */
		__myThread->lockWaitingOn = 0;
		if(lock->_waiters == __myThread)
		{
			lock->_waiters = __myThread->nextWaiter;
			__myThread->nextWaiter = 0;
		}
		else
		{
			ILThread *nextThread;
			
			nextThread = lock->_waiters;
			while(nextThread)
			{
				if(nextThread->nextWaiter == __myThread)
				{
					nextThread->nextWaiter = __myThread->nextWaiter;
					__myThread->nextWaiter = 0;
					break;
				}
				nextThread = nextThread->nextWaiter;
			}
		}
	}
	_ILCondMutexUnlock(&(lock->_mutex));
	IL_END_INTERRUPTIBLE(result)
	return result;
}

/*
 * Interruptible timed wait on a lock.
 */
static int LockTimedWaitInterruptible(_ILLock *lock, const struct timespec *ts)
{
	int result = IL_THREAD_OK;

	IL_START_INTERRUPTIBLE(result)
	_ILCondMutexLock(&(lock->_mutex));
	lock->_value -= 1;
	if(lock->_value < 0)
	{
		/*
		 * Record this lock in the ILThread structure and add the thread to
		 * the waiters on this lock.
		 */
		__myThread->lockWaitingOn = lock;
		__myThread->nextWaiter = lock->_waiters;
		lock->_waiters = __myThread;
		do
		{
			result = pthread_cond_timedwait(&(lock->_cond), &(lock->_mutex), ts);
			if(result == 0)
			{
				/* Check if we are interrupted */
				__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
				if((__threadState & IL_TS_INTERRUPTED) != 0)
				{
					/* Leave the wait loop  */
					break;
				}
				else if((__threadState & IL_TS_NOTINTERRUPTED) != 0)
				{
					/* Clear the not interrupted flag */
					__threadState &= ~IL_TS_NOTINTERRUPTED;
					ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState);
					/* And continue waiting */
					continue;
				}
				result = IL_THREAD_OK;
				break;
			}
			else if(result == ETIMEDOUT)
			{
				/* Clear the not interrupted flag if present */
				__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
				if((__threadState & IL_TS_NOTINTERRUPTED) != 0)
				{
					__threadState &= ~IL_TS_NOTINTERRUPTED;
					ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState);
				}
				lock->_value += 1;
				result = IL_THREAD_BUSY;
				break;
			}
			else
			{
				lock->_value += 1;
				result = IL_THREAD_ERR_UNKNOWN;
				break;
			}
		} while(1);
		/*
		 * Clear the  lock in the ILThread structure and remove the thread from
		 * the waiters on this lock.
		 */
		__myThread->lockWaitingOn = 0;
		if(lock->_waiters == __myThread)
		{
			lock->_waiters = __myThread->nextWaiter;
			__myThread->nextWaiter = 0;
		}
		else
		{
			ILThread *nextThread;
			
			nextThread = lock->_waiters;
			while(nextThread)
			{
				if(nextThread->nextWaiter == __myThread)
				{
					nextThread->nextWaiter = __myThread->nextWaiter;
					__myThread->nextWaiter = 0;
					break;
				}
				nextThread = nextThread->nextWaiter;
			}
		}
	}
	_ILCondMutexUnlock(&(lock->_mutex));
	IL_END_INTERRUPTIBLE(result)
	return result;
}

static int LockWaitUninterruptible(_ILLock *lock)
{
	int result = IL_THREAD_OK;

	IL_START_UNINTERRUPTIBLE()
	_ILCondMutexLock(&(lock->_mutex));
	lock->_value -= 1;
	if(lock->_value < 0)
	{
		/*
		 * Record this lock in the ILThread structure and add the thread to
		 * the waiters on this lock.
		 */
		__myThread->lockWaitingOn = lock;
		__myThread->nextWaiter = lock->_waiters;
		lock->_waiters = __myThread;
		do
		{
			if(pthread_cond_wait(&(lock->_cond), &(lock->_mutex)) == 0)
			{
				/* Check if we are interrupted */
				__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
				if((__threadState & IL_TS_INTERRUPTED) != 0)
				{
					/* Clear the interrupted flag */
					__threadState &= ~IL_TS_INTERRUPTED;
					ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState);
					/* Readd me to the number of waiters. */
					lock->_value -= 1;
					/* Set the result to interrupted */
					result = IL_THREAD_ERR_INTERRUPT;
					/* and continue waiting */
					continue;
				}
				else if((__threadState & IL_TS_NOTINTERRUPTED) != 0)
				{
					/* Clear the not interrupted flag */
					__threadState &= ~IL_TS_NOTINTERRUPTED;
					ILInterlockedStoreU2(&(__myThread->state.split.priv), __threadState);
					/* And continue waiting */
					continue;
				}
				result = IL_THREAD_OK;
				break;
			}
			else
			{
				/* An unexpected error occured so bail out */
				lock->_value += 1;
				result = IL_THREAD_ERR_UNKNOWN;
				break;
			}
		} while(1);
		/*
		 * Clear the  lock in the ILThread structure and remove the thread from
		 * the waiters on this lock.
		 */
		__myThread->lockWaitingOn = 0;
		if(lock->_waiters == __myThread)
		{
			lock->_waiters = __myThread->nextWaiter;
			__myThread->nextWaiter = 0;
		}
		else
		{
			ILThread *nextThread;
			
			nextThread = lock->_waiters;
			while(nextThread)
			{
				if(nextThread->nextWaiter == __myThread)
				{
					nextThread->nextWaiter = __myThread->nextWaiter;
					__myThread->nextWaiter = 0;
					break;
				}
				nextThread = nextThread->nextWaiter;
			}
		}
	}
	_ILCondMutexUnlock(&(lock->_mutex));
	IL_END_UNINTERRUPTIBLE(result)
	return result;
}
#endif /* _IL_PT_LOCK_COND */

/*
 * This function is only used for initializing an ILThread
 * structure for threads not created by pnet.
 */
void _ILThreadInitHandleSelf(ILThread *thread)
{
	/* Set the thread handle and identifier for the thread */
	thread->handle = pthread_self();
#ifdef GC_DARWIN_THREADS
	thread->identifier = pthread_mach_thread_np(thread->handle);
#endif
}

void _ILThreadDestroyHandleSelf(ILThread *thread)
{
}

void _ILThreadInitSystem(ILThread *mainThread)
{
	struct sigaction action;
	sigset_t set;
#ifdef HAVE_LIBGC
	int suspendSignal;
#endif

	/* Set up the signal handlers that we require */
	ILMemZero(&action, sizeof(action));
	sigfillset(&(action.sa_mask));
	sigdelset(&(action.sa_mask), SIGINT);
	sigdelset(&(action.sa_mask), SIGQUIT);
	sigdelset(&(action.sa_mask), SIGTERM);
	sigdelset(&(action.sa_mask), SIGABRT);

	action.sa_flags = SA_RESTART;
	action.sa_handler = SuspendSignal;
	sigaction(IL_SIG_SUSPEND, &action, (struct sigaction *)0);

	/*
	 * Abort signal - used to signal a thread abort.
	 * SA_RESTART must not be set.
	 */
	action.sa_flags = 0;
	sigemptyset(&(action.sa_mask));
	action.sa_handler = AbortSignal;
	sigaction(IL_SIG_ABORT, &action, (struct sigaction *)0);

	/*
	 * Interrupt signal - used to interrupt a blocked thread.
	 * SA_RESTART must not be set.
	 */
	action.sa_handler = InterruptSignal;
	sigaction(IL_SIG_INTERRUPT, &action, (struct sigaction *)0);

	/*
	 * Resume signal - used to resume a suspended thread.
	 * SA_RESTART must not be set.
	 */
	action.sa_handler = ResumeSignal;
	sigaction(IL_SIG_RESUME, &action, (struct sigaction *)0);

	/*
	 * Initialize the signal mask for interrupting waits.
	 */
	sigemptyset(&_interruptSet);
	sigaddset(&_interruptSet, IL_SIG_INTERRUPT);

	/*
	 * Initialize the signal mask to resume a suspended thread.
	 */
	sigfillset(&_suspendSet);
	sigdelset(&_suspendSet, IL_SIG_RESUME);
#ifdef HAVE_LIBGC
	if((suspendSignal = GC_get_suspend_signal()) != -1)
	{
		sigdelset(&_suspendSet, suspendSignal);
	}
#endif
	sigdelset(&_suspendSet, PTHREAD_SIG_CANCEL);
	sigdelset(&_suspendSet, SIGINT);
	sigdelset(&_suspendSet, SIGQUIT);
	sigdelset(&_suspendSet, SIGTERM);
	sigdelset(&_suspendSet, SIGABRT);

#if !defined(USE_COMPILER_TLS)
	/* We need a thread-specific key for storing thread objects */
	pthread_key_create(&_ILThreadObjectKey, (void (*)(void *))0);
#endif

	/* Initialize the mutexattr used on this system. */
	pthread_mutexattr_init(&_ILMutexAttr);
#ifdef PTHREAD_MUTEX_NORMAL
	pthread_mutexattr_settype(&_ILMutexAttr, PTHREAD_MUTEX_NORMAL);
#endif

	/*
	 * Block the IL_SIG_RESUME and IL_SIG_INTERRUPT signals in the current
	 * thread. They will be unblocked by "SuspendUntilResumed" or if entering
	 * the wait/sleep/join state
	 */
	sigemptyset(&set);
	sigaddset(&set, IL_SIG_RESUME);
	sigaddset(&set, IL_SIG_INTERRUPT);
	pthread_sigmask(SIG_BLOCK, &set, (sigset_t *)0);

	/* Unblock the IL_SIG_SUSPEND and IL_SIG_ABORT signals */
	sigemptyset(&set);
	sigaddset(&set, IL_SIG_SUSPEND);
	sigaddset(&set, IL_SIG_ABORT);
	pthread_sigmask(SIG_UNBLOCK, &set, (sigset_t *)0);

	/* Set the thread handle and identifier for the main thread */
	mainThread->handle = pthread_self();
#ifdef GC_DARWIN_THREADS
	mainThread->identifier = pthread_mach_thread_np(mainThread->handle);
#endif
	/* Initialize the atomic operations */
	ILInterlockedInit();
}

void _ILThreadSetPriority(ILThread *thread, int priority)
{
}

int _ILThreadGetPriority(ILThread *thread)
{
	return IL_TP_NORMAL;
}

/*
 * Entry point for a pthread.  This initializes the thread
 * and then calls "_ILThreadRun" to do the real work.
 */
static void *ThreadStart(void *arg)
{
	ILThread *thread = (ILThread *)arg;
	sigset_t set;

	/* Store the thread identifier into the object */
	thread->handle = pthread_self();
#ifdef GC_DARWIN_THREADS
	thread->identifier = pthread_mach_thread_np(thread->handle);
#endif

#if defined(USE_COMPILER_TLS)
	/* Store the thread at the thread local storage */
	_myThread = thread;
#else
	/* Attach the thread object to the thread identifier */
	pthread_setspecific(_ILThreadObjectKey, (void *)thread);
#endif

	/* Detach ourselves because we won't be using "pthread_join"
	   to detect when the thread exits */
	pthread_detach(thread->handle);

	/* Set the cancel type to asynchronous */
	pthread_setcanceltype(PTHREAD_CANCEL_ASYNCHRONOUS, (int *)0);

	/* Block the IL_SIG_RESUME signal in the current thread.
	   It will be unblocked by "SuspendUntilResumed" */
	sigemptyset(&set);
	sigaddset(&set, IL_SIG_RESUME);
	pthread_sigmask(SIG_BLOCK, &set, (sigset_t *)0);

	/* Unblock the IL_SIG_SUSPEND and IL_SIG_ABORT signals */
	sigemptyset(&set);
	sigaddset(&set, IL_SIG_SUSPEND);
	sigaddset(&set, IL_SIG_ABORT);
	pthread_sigmask(SIG_UNBLOCK, &set, (sigset_t *)0);

	/* Run the thread */
	_ILThreadRun(thread);

#ifdef GC_DARWIN_THREADS
	thread->identifier = 0;
#endif
	
	/* The thread has exited back through its start function */
	return 0;
}

int _ILThreadCreateSystem(ILThread *thread)
{
	if(pthread_create((pthread_t *)&(thread->handle), (pthread_attr_t *)0,
					  ThreadStart, (void *)thread) == 0)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Interrupt a thread in the wait/sleep/join state or when it enters the
 * wait/sleep/join state the next time.
 */
void _ILThreadInterrupt(ILThread *thread)
{
	if(thread)
	{
#ifdef _IL_PT_LOCK_COND
		_ILLock *lock;
		ILUInt16 threadState;

		threadState = ILInterlockedLoadU2(&(thread->state.split.priv));
		lock = (_ILLock *)ILInterlockedLoadP((void **)&(thread->lockWaitingOn));
#endif /* _IL_PT_LOCK_COND */
		/*
		 * Send the interrupt request to the thread.
		 */
		pthread_kill(thread->handle, IL_SIG_INTERRUPT);
#ifdef _IL_PT_LOCK_COND
		if(lock != 0)
		{
			/* Acquire the lock */
			_ILCondMutexLock(&(lock->_mutex));
			/* Recheck if the thread is still waiting on the same lock */
			if(lock == (_ILLock *)ILInterlockedLoadP((void **)&(thread->lockWaitingOn)))
			{
				/*
				 * First check if the thread to be interrupted should be awakened
				 * because of an other interrupt processing.
				 */
				if((threadState & IL_TS_INTERRUPTED) == 0)
				{
					/* Remove the thread from the number od waiters */
					lock->_value += 1;

					threadState |= IL_TS_INTERRUPTED;
					if((threadState & IL_TS_NOTINTERRUPTED) != 0)
					{
						threadState &= ~IL_TS_NOTINTERRUPTED;
						ILInterlockedStoreU2(&(thread->state.split.priv), threadState);
					}
					else
					{
						/*
						 * Mark all other threads waiting on the lock as not
						 * interrupted so that they will continue waiting at
						 * their next wakeup.
						 */
						ILThread *otherThread;
						ILInt32 value;
						int otherWaiter = 0;

						/*
						 * Set the interrupted flag here so that we don't depend
						 * on the signal processing timing to set the flag.
						 */
						ILInterlockedStoreU2(&(thread->state.split.priv), threadState);

						/*
						 * Process at most (-lock->_value) waiters.
						 */
						value = lock->_value;
						otherThread = lock->_waiters;
						while(otherThread && (value < 0))
						{
							if(otherThread != thread)
							{
								threadState = ILInterlockedLoadU2(&(otherThread->state.split.priv));
								if((threadState & IL_TS_INTERRUPTED) == 0)
								{
									threadState |= IL_TS_NOTINTERRUPTED;
									ILInterlockedStoreU2(&(otherThread->state.split.priv), threadState);
									/*
									 * Set the flag that there is an other thread waiting
									 * on the lock
									 */
									otherWaiter = 1;
								}
								++value;
							}
							otherThread = otherThread->nextWaiter;
						}
						if(otherWaiter)
						{
							/*
							 * Mark the lock that there is interrupt processing on
							 * the way
							 */
							lock->_interrupted = 1;
						}
						/* Wake up all threads waiting on the lock */
						_ILCondVarSignalAll(&(lock->_cond));
					}
				}
			}
			/* Release the lock */
			_ILCondMutexUnlock(&(lock->_mutex));
		}
#endif /* _IL_PT_LOCK_COND */
	}
}

/*
 * Put the current thread to sleep for a number of milliseconds.
 * The sleep may be interrupted by a call to _ILThreadInterrupt.
 */
int _ILThreadSleep(ILUInt32 ms)
{
	if(ms == 0)
	{
		/*
		 * This is like yielding the thread.
		 */
		_ILThreadYield();
		return IL_THREAD_OK;
	}
	else if((ms <= IL_MAX_INT32) || (ms == IL_MAX_UINT32))
	{
		int result;
#ifdef HAVE_NANOSLEEP
		struct timespec ts;
	
		ts.tv_sec = (time_t)(ms / 1000);
		ts.tv_nsec = (long int)(ms % 1000) * 1000000;

		IL_START_INTERRUPTIBLE(result)
		do
		{
			if((result = nanosleep(&ts, &ts)) != 0)
			{
				/* Looks like we got interrupted */
				if(errno == EINTR)
				{
#ifndef _IL_PT_INTERRUPT_JMP
					/* Check and clear the interrupted flag */
					__threadState = ILInterlockedLoadU2(&(__myThread->state.split.priv));
					if((__threadState & IL_TS_INTERRUPTED) != 0)
					{
						result = IL_THREAD_ERR_INTERRUPT;
						break;
					}
					else
					{
						continue;
					}
#endif /* !_IL_PT_INTERRUPT_JMP */
				}
				else
				{
					result = IL_THREAD_ERR_UNKNOWN;
					break;
				}
			}
			else if(ms == IL_MAX_UINT32)
			{
				/* Reinitialize the time to sleep */
				ts.tv_sec = (time_t)(ms / 1000);
				ts.tv_nsec = (long int)(ms % 1000) * 1000000;
			}
			else
			{
				break;
			}
		} while (1);
#else /* !HAVE_NANOSLEEP */
#error "nanosleep is needed for implementing an interruptible sleep"
#endif /* !HAVE_NANOSLEEP */
		IL_END_INTERRUPTIBLE(result)
		return result;
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

int _ILCondVarTimedWait(_ILCondVar *cond, _ILCondMutex *mutex, ILUInt32 ms)
{
	struct timeval tv;
	struct timespec ts;
	int result;

	if(ms != IL_MAX_UINT32)
	{
		/* Convert the milliseconds value into an absolute timeout */
		gettimeofday(&tv, 0);
		ts.tv_sec = tv.tv_sec + (long)(ms / 1000);
		ts.tv_nsec = (tv.tv_usec + (long)((ms % 1000) * 1000)) * 1000L;
		if(ts.tv_nsec >= 1000000000L)
		{
			++(ts.tv_sec);
			ts.tv_nsec -= 1000000000L;
		}

		/* Wait until we are signalled or the timeout expires */
		do
		{
			result = pthread_cond_timedwait(cond, mutex, &ts);
		}
		while(result == EINTR);
		return ((result == 0) ? 1 : 0);
	}
	else
	{
		/* Wait forever until the condition variable is signalled */
		pthread_cond_wait(cond, mutex);
		return 1;
	}
}

int _ILCondMutexTimedLockUnsafe(_ILCondMutex *mutex, ILUInt32 ms)
{
	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		int result;

		if(ms == IL_MAX_UINT32)
		{
			/* This is the same as the call to _ILCondMutexLockUnsafe. */
			result = pthread_mutex_lock(mutex);
		}
		else if(ms == 0)
		{
			/* This is the same as the call to _ILCondMutexTryLock. */
			result = pthread_mutex_trylock(mutex);
		}
		else
		{
			struct timeval tv;
			struct timespec ts;

			gettimeofday(&tv, 0);
			ts.tv_sec = tv.tv_sec + (long)(ms / 1000);
			ts.tv_nsec = (tv.tv_usec + (long)((ms % 1000) * 1000)) * 1000L;
			if(ts.tv_nsec >= 1000000000L)
			{
				++(ts.tv_sec);
				ts.tv_nsec -= 1000000000L;
			}
			result = pthread_mutex_timedlock(mutex, &ts);
		}
		switch(result)
		{
			case 0:
			{
				return IL_THREAD_OK;
			}
			break;

			case EBUSY:
			case ETIMEDOUT:
			{
				return IL_THREAD_BUSY;
			}
			break;
		}
		return IL_THREAD_ERR_UNKNOWN;
	}
	else
	{
		/* Invalid negative value for ms. */
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

int _ILSemaphorePostMultiple(_ILSemaphore *sem, ILUInt32 count)
{
	if((count > 0) && (count < IL_MAX_INT32))
	{
		ILUInt32 current;

		for(current = 0; current < count; current++)
		{
			if(sem_post(sem))
			{
				/* An error occured. */
				return IL_THREAD_ERR_UNKNOWN;
			}
		}
		return IL_THREAD_OK;
	}
	/* Invalid value for the release count. */
	return IL_THREAD_ERR_INVALID_RELEASECOUNT;
}

int _ILSemaphoreTimedWait(_ILSemaphore *sem, ILUInt32 ms, ILInt32 interruptable)
{
	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		int result;

		if(ms == IL_MAX_UINT32)
		{
			/* This is the same as the call to _ILSemaphoreWait. */
			do
			{
				result = sem_wait(sem);
				if(result == 0)
				{
					break;
				}
				else if(errno == EINTR)
				{
					if(interruptable)
					{
						result = IL_THREAD_ERR_INTERRUPT;
						break;
					}
				}
				else
				{
					result = IL_THREAD_ERR_UNKNOWN;
					break;					
				}
			} while(1);
		}
		else if(ms == 0)
		{
			/* This is the same as the call to _ILSemaphoreTryWait. */
			if((result = sem_trywait(sem)) != 0)
			{
				if(errno == EAGAIN)
				{
					result = IL_THREAD_BUSY;
				}
				else
				{
					result = IL_THREAD_ERR_UNKNOWN;
				}
			}
		}
		else
		{
			struct timeval tv;
			struct timespec ts;

			gettimeofday(&tv, 0);
			ts.tv_sec = tv.tv_sec + (long)(ms / 1000);
			ts.tv_nsec = (tv.tv_usec + (long)((ms % 1000) * 1000)) * 1000L;
			if(ts.tv_nsec >= 1000000000L)
			{
				++(ts.tv_sec);
				ts.tv_nsec -= 1000000000L;
			}
			do
			{
				result = sem_timedwait(sem, &ts);
			} while(!interruptable && (result == EINTR));
		}
		switch(result)
		{
			case 0:
			{
				return IL_THREAD_OK;
			}
			break;

			case EINTR:
			{
				return IL_THREAD_ERR_INTERRUPT;
			}
			break;

			case EAGAIN:
			case ETIMEDOUT:
			{
				return IL_THREAD_BUSY;
			}
			break;
		}
		return IL_THREAD_ERR_UNKNOWN;
	}
	else
	{
		/* Invalid negative value for ms. */
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

/*
 * Release a number of waiting threads from the count semaphore.
 */
int _ILCountSemaphoreSignalCount(_ILCountSemaphore *sem, ILUInt32 count)
{
	int result = IL_THREAD_OK;

	if((count > 0) && (count <= IL_MAX_INT32))
	{
		ILUInt32 current;

		_SemAdd(&(sem->_value), count);

		for(current = 0; current < count; current++)
		{
			if(sem_post(&(sem->_sem)))
			{
				/* An error occured. */
				return IL_THREAD_ERR_UNKNOWN;
			}
		}
	}
	else
	{
		/* Invalid value for the release count. */
		result = IL_THREAD_ERR_INVALID_RELEASECOUNT;
	}
	return result;
}

/*
 * Release all waiting threads from the count semaphore.
 */
int _ILCountSemaphoreSignalAll(_ILCountSemaphore *sem)
{
	int result = IL_THREAD_OK;
	ILInt32 count;
	ILInt32 current;

	count = _SemExchange(&(sem->_value), 0);

	for(current = 0; current < count; ++current)
	{
		if(sem_post(&(sem->_sem)))
		{
			/* An error occured. */
			return IL_THREAD_ERR_UNKNOWN;
		}
	}

	return result;
}

/*
 * Wait on a count semaphore.
 */
int _ILCountSemaphoreTimedWait(_ILCountSemaphore *sem, ILUInt32 ms)
{
	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		int result = IL_THREAD_OK;

		/* Increment the count semaphore's waiters. */		
		_SemIncrement(&(sem->_value));

		if((result = _ILSemaphoreTimedWait(&(sem->_sem), ms, 0)) != IL_THREAD_OK)
		{
			/*
			 * We have to decrement the counter again because the call failed.
			 * NOTE: The waiters can become negative here.
			 */
			_SemDecrement(&(sem->_value));
		}
		return result;
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

int _ILMonitorExit(_ILMonitor *mon)
{
	return LockSignal(&(mon->_enterLock));
}

int _ILMonitorTimedTryEnter(_ILMonitor *mon, ILUInt32 ms)
{
	if(ms == 0)
	{
		return LockTryWait(&(mon->_enterLock));
	}
	else if(ms == IL_MAX_UINT32)
	{
		return LockWaitInterruptible(&(mon->_enterLock));
	}
	else if(ms <= IL_MAX_INT32)
	{
		struct timeval tv;
		struct timespec ts;

		gettimeofday(&tv, 0);
		ts.tv_sec = tv.tv_sec + (long)(ms / 1000);
		ts.tv_nsec = (tv.tv_usec + (long)((ms % 1000) * 1000)) * 1000L;
		if(ts.tv_nsec >= 1000000000L)
		{
			++(ts.tv_sec);
			ts.tv_nsec -= 1000000000L;
		}
		return LockTimedWaitInterruptible(&(mon->_enterLock), &ts);
	}
	else
	{
		/* Invalid negative value for ms. */
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

int _ILMonitorTimedWait(_ILMonitor *mon, ILUInt32 ms)
{
	int result = IL_THREAD_OK;
	int enterResult;

	if(ms == 0)
	{
		/*
		 * In this case we don't have to enter the waiting queue but simply
		 * release and reenter the ready state.
		 */
		result = LockSignal(&(mon->_enterLock));
		if(result != IL_THREAD_OK)
		{
			return result;
		}
	}
	else if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		/*
		 * Enter the wait queue
		 */
		ILInterlockedDecrementI4_Acquire(&(mon->_waitValue));
		
		/*
		 * Release the ownership of the monitor.
		 */
		result = LockSignal(&(mon->_enterLock));
		if(result != IL_THREAD_OK)
		{
			/*
			 * Leave the wait queue and return with error
			 */
			ILInterlockedIncrementI4_Acquire(&(mon->_waitValue));
			return IL_THREAD_ERR_UNKNOWN;
		}

		if(ms == IL_MAX_UINT32)
		{
			result = LockWaitInterruptible(&(mon->_waitLock));
			if(result)
			{
				/*
				 * We have not been pulsed so leave the wait queue now
				 */
				ILInterlockedIncrementI4_Acquire(&(mon->_waitValue));
				
			}
		}
		else
		{
			struct timeval tv;
			struct timespec ts;

			gettimeofday(&tv, 0);
			ts.tv_sec = tv.tv_sec + (long)(ms / 1000);
			ts.tv_nsec = (tv.tv_usec + (long)((ms % 1000) * 1000)) * 1000L;
			if(ts.tv_nsec >= 1000000000L)
			{
				++(ts.tv_sec);
				ts.tv_nsec -= 1000000000L;
			}
			result = LockTimedWaitInterruptible(&(mon->_waitLock), &ts);
			if(result)
			{
				/*
				 * We have not been pulsed so leave the wait queue now
				 */
				ILInterlockedIncrementI4_Acquire(&(mon->_waitValue));
				
			}
		}
	}
	else
	{
		/* Invalid negative value for ms. */
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
	/*
	 * Now we have to regain the ownership of the monitor
	 */
	enterResult = LockWaitUninterruptible(&(mon->_enterLock));
	if(result == IL_THREAD_OK)
	{
		result = enterResult;
	}
	return result;
}

int _ILMonitorPulse(_ILMonitor *mon)
{
	int result;

	result = IL_THREAD_OK;
	if(mon)
	{
		ILInt32 waitValue;

		result = IL_THREAD_OK;
		waitValue = ILInterlockedLoadI4(&(mon->_waitValue));
		if(waitValue < 0)
		{
			waitValue = ILInterlockedIncrementI4_Acquire(&(mon->_waitValue));
			if(waitValue <= 0)
			{
				result = LockSignal(&(mon->_waitLock));
			}
			else
			{
				ILInterlockedDecrementI4_Acquire(&(mon->_waitValue));
			}
		}
	}
	return result;
}

int _ILMonitorPulseAll(_ILMonitor *mon)
{
	if(mon)
	{
		ILInt32 waitValue;
		
		waitValue = ILInterlockedLoadI4(&(mon->_waitValue));
		if(waitValue < 0)
		{
			waitValue = ILInterlockedExchangeI4_Acquire(&(mon->_waitValue), 0);

			if(waitValue < 0)
			{
				/* Release the waiters */
				LockSignalCount(&(mon->_waitLock), -waitValue);
			}
			else if(waitValue > 0)
			{
				/* Readjust the monitor's wait value */
				ILInterlockedAddI4_Acquire(&(mon->_waitValue), waitValue);
			}
		}
	}
	return IL_THREAD_OK;
}

#ifdef	__cplusplus
};
#endif

#endif	/* IL_USE_PTHREADS */
