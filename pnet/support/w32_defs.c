/*
 * w32_defs.c - Thread definitions for using Win32 threads.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#include "thr_defs.h"
#include "interlocked.h"

#ifdef IL_USE_WIN32_THREADS

/* Don't include windows.h here otherwise it will override
   CreateThread macros from gc.h.  It should be included by thr_choose.h */
   
#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(USE_COMPILER_TLS)
/*
 * Define the thread local variable to hold the ILThread value for the
 * current thread.
 */
_THREAD_ ILThread *_myThread;
#else
/*
 * Thread-specific key that is used to store and retrieve the thread object.
 */
DWORD _ILThreadObjectKey;
#endif

/*
 *	Sets the thread priority.
 */
void _ILThreadSetPriority(ILThread *thread, int priority)
{
	switch (priority)
	{
	case IL_TP_LOWEST:
		SetThreadPriority(thread->handle, THREAD_PRIORITY_LOWEST);
		break;
	case IL_TP_BELOW_NORMAL:
		SetThreadPriority(thread->handle, THREAD_PRIORITY_BELOW_NORMAL);
		break;
	case IL_TP_NORMAL:
		SetThreadPriority(thread->handle, THREAD_PRIORITY_NORMAL);
		break;
	case IL_TP_ABOVE_NORMAL:
		SetThreadPriority(thread->handle, THREAD_PRIORITY_ABOVE_NORMAL);
		break;
	case IL_TP_HIGHEST:
		SetThreadPriority(thread->handle, THREAD_PRIORITY_HIGHEST);
		break;
	}	
}

/*
 *	Gets the thread priority.
 */
int _ILThreadGetPriority(ILThread *thread)
{
	switch (GetThreadPriority(thread->handle))
	{
	case THREAD_PRIORITY_IDLE:
	case THREAD_PRIORITY_LOWEST:		
		return IL_TP_LOWEST;

	case THREAD_PRIORITY_BELOW_NORMAL:		
		return IL_TP_BELOW_NORMAL;

	case THREAD_PRIORITY_NORMAL:		
		return IL_TP_NORMAL;

	case THREAD_PRIORITY_ABOVE_NORMAL:		
		return IL_TP_ABOVE_NORMAL;

	case THREAD_PRIORITY_HIGHEST:
	case THREAD_PRIORITY_TIME_CRITICAL:
		return IL_TP_HIGHEST;

	default:
		return IL_TP_NORMAL;
	}
}

/*
 * This function is only used for initializing an ILThread
 * structure for threads not created by pnet.
 * This Thread MUST NOT BE USED to run managed code or create
 * managed objects because this thread is not controled by the GC.
 */
void _ILThreadInitHandleSelf(ILThread *thread)
{
	/* Initialize the thread's handle and identifier.  We have
	   to duplicate the thread handle because "GetCurrentThread()" returns
	   a pseudo-handle and not a real one. We need the real one */
	DuplicateHandle(GetCurrentProcess(), GetCurrentThread(),
					GetCurrentProcess(), (HANDLE *)(&(thread->handle)),
					0, 0, DUPLICATE_SAME_ACCESS);
	thread->identifier = GetCurrentThreadId();
}

void _ILThreadInitSystem(ILThread *mainThread)
{
#if !defined(USE_COMPILER_TLS)
	/* Allocate a TLS key for storing thread objects */
	_ILThreadObjectKey = TlsAlloc();
#endif

	/* Initialize the "main" thread's handle and identifier.  We have
	   to duplicate the thread handle because "GetCurrentThread()" returns
	   a pseudo-handle and not a real one.  We need the real one */
	DuplicateHandle(GetCurrentProcess(), GetCurrentThread(),
					GetCurrentProcess(), (HANDLE *)(&(mainThread->handle)),
					0, 0, DUPLICATE_SAME_ACCESS);
	mainThread->identifier = GetCurrentThreadId();

	/* Initialize the atomic operations */
	ILInterlockedInit();
}

/*
 * Main Win32 entry point for a thread.
 */
static DWORD WINAPI ThreadStart(LPVOID arg)
{
	ILThread *thread = (ILThread *)arg;

#if defined(USE_COMPILER_TLS)
	/* Store the thread at the thread local storage */
	_myThread = thread;
#else
	/* Attach the thread object to the thread */
	TlsSetValue(_ILThreadObjectKey, thread);
#endif

	/* Run the thread */
	_ILThreadRun(thread);

	/* Exit from the thread */
	return 0;
}

int _ILThreadCreateSystem(ILThread *thread)
{
	thread->handle = CreateThread(NULL, 0, ThreadStart,
								  thread, 0, (DWORD *)&(thread->identifier));
	return (thread->handle != NULL);
}

/*
 * APC procedure for interrupting a thread
 */
static VOID CALLBACK _InterruptThread(ULONG_PTR args)
{
	/* Nothing to do here */
}

/*
 * Send an interrupt request to a thread.
 */
void _ILThreadInterrupt(ILThread *thread)
{
	QueueUserAPC(_InterruptThread, thread->handle, 0);
}

/*
 * Put the current thread to sleep for a number of milliseconds.
 * The sleep may be interrupted by a call to _ILThreadInterrupt.
 */
int _ILThreadSleep(ILUInt32 ms)
{
	if((ms <= IL_MAX_INT32) || (ms == IL_MAX_UINT32))
	{
		int result;

		result = SleepEx(ms, TRUE);
		if(result == 0)
		{
			return IL_THREAD_OK;
		}
		else if(result == WAIT_IO_COMPLETION)
		{
			return IL_THREAD_ERR_INTERRUPT;
		}
		else
		{
			return IL_THREAD_ERR_UNKNOWN;
		}
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

/*
 * This function is simply a wrapper around the WaitForSingleObject function
 * to handle the errors and convert them to the unified error codes.
 */
int	_ILWaitHandleTimedWait(HANDLE handle, ILUInt32 ms)
{
	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		DWORD result;

		if(ms == IL_MAX_UINT32)
		{
			result = WaitForSingleObject(handle, INFINITE);
		}
		else
		{
			result = WaitForSingleObject(handle, (DWORD)ms);
		}

		if(result == WAIT_OBJECT_0)
		{
			return IL_THREAD_OK;
		}
		else
		{
			switch(result)
			{
				case WAIT_ABANDONED:
				{
					return IL_THREAD_ERR_ABANDONED;
				}
				break;

				case WAIT_TIMEOUT:
				{
					return IL_THREAD_BUSY;
				}
				break;
			}
		}
		return IL_THREAD_ERR_UNKNOWN;
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

/*
 * This function is the same as _ILWaitHandleTimedWait except that this
 * version is interruptible by other threads calling _ILThreadInterrupt().
 */
int	_ILWaitHandleTimedWaitInterruptible(HANDLE handle, ILUInt32 ms)
{
	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		DWORD result;

		if(ms == IL_MAX_UINT32)
		{
			result = WaitForSingleObjectEx(handle, INFINITE, TRUE);
		}
		else
		{
			result = WaitForSingleObjectEx(handle, (DWORD)ms, TRUE);
		}

		if(result == WAIT_OBJECT_0)
		{
			return IL_THREAD_OK;
		}
		else
		{
			switch(result)
			{
				case WAIT_ABANDONED:
				{
					return IL_THREAD_ERR_ABANDONED;
				}
				break;

				case WAIT_IO_COMPLETION:
				{
					return IL_THREAD_ERR_INTERRUPT;
				}
				break;

				case WAIT_TIMEOUT:
				{
					return IL_THREAD_BUSY;
				}
				break;
			}
		}
		return IL_THREAD_ERR_UNKNOWN;
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

/*
 * Note: this implementation is not fully atomic.  There is a
 * window of opportunity between when the current thread notices
 * that the condition is signalled and when the mutex is regained.
 * The caller is expected to code around this.
 */
int _ILCondVarTimedWait(_ILCondVar *cond, _ILCondMutex *mutex, ILUInt32 ms)
{
	DWORD result;
	if(ms != IL_MAX_UINT32)
	{
		result = SignalObjectAndWait(*mutex, *cond, (DWORD)ms, FALSE);
	}
	else
	{
		result = SignalObjectAndWait(*mutex, *cond, INFINITE, FALSE);
	}
	WaitForSingleObject(*mutex, INFINITE);
	return (result == WAIT_OBJECT_0);
}

/*
 * Release a number of waiting threads from the count semaphore.
 */
int _ILCountSemaphoreSignalCount(_ILCountSemaphore *sem, ILUInt32 count)
{
	int result = IL_THREAD_OK;

	ILInterlockedSubI4_Acquire(&(sem->_waiting), count);

	result = (ReleaseSemaphore(sem->_sem, (LONG)count, 0) != 0) ? 
						IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN;

	return result;
}

/*
 * Release all waiting threads from the count semaphore.
 */
int _ILCountSemaphoreSignalAll(_ILCountSemaphore *sem)
{
	int result = IL_THREAD_OK;

	if(sem->_waiting > 0)
	{
		/* Lock the count semaphore object. */
		_ILCriticalSectionEnter(&(sem->_lock));

		/* We have to recheck because of possible race conditions. */
		if(sem->_waiting > 0)
		{
			result = (ReleaseSemaphore(sem->_sem, sem->_waiting, 0) != 0) ? 
						IL_THREAD_OK : IL_THREAD_ERR_UNKNOWN;

			sem->_waiting = 0;
		}

		/* Unlock the count semaphore object. */
		_ILCriticalSectionLeave(&(sem->_lock));
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

		/* Lock the count semaphore object. */
		_ILCriticalSectionEnter(&(sem->_lock));

		sem->_waiting += 1;

		/* Unlock the count semaphore object. */
		_ILCriticalSectionLeave(&(sem->_lock));

		if((result = _ILWaitHandleTimedWait(sem->_sem, ms)) != IL_THREAD_OK)
		{
			/* We have to decrement the counter again because the call failed. */

			/* Lock the count semaphore object. */
			_ILCriticalSectionEnter(&(sem->_lock));

			sem->_waiting -= 1;

			/* Unlock the count semaphore object. */
			_ILCriticalSectionLeave(&(sem->_lock));
		}
		return result;
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

int _ILMonitorTimedTryEnter(_ILMonitor *mon, ILUInt32 ms)
{
	DWORD result;

	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		result = _ILWaitHandleTimedWaitInterruptible(mon->_mutex, ms);
	}
	else
	{
		result = IL_THREAD_ERR_INVALID_TIMEOUT;
	}
	return result;
}

int _ILMonitorTimedWait(_ILMonitor *mon, ILUInt32 ms)
{
	if((ms == IL_MAX_UINT32) || (ms <= IL_MAX_INT32))
	{
		DWORD result;
		DWORD mutexResult;

		/* Decrement the wait count. */
		ILInterlockedDecrementI4_Acquire(&(mon->_waitValue));

		/* Unlock the mutex and wait on the semaphore. */
		result = SignalObjectAndWait(mon->_mutex, mon->_sem,
									 (DWORD)ms, TRUE);

		if(result != WAIT_OBJECT_0)
		{
			/* Remove us from the waiters. */
			ILInterlockedIncrementI4_Acquire(&(mon->_waitValue));

			if(result == WAIT_IO_COMPLETION)
			{
				/* We got interrupted */
				result = IL_THREAD_ERR_INTERRUPT;
			}
			else if(result == WAIT_TIMEOUT)
			{
				/* Timeout expired before we got signaled */
				result = IL_THREAD_BUSY;
			}
			else
			{
				/* An unexpected error occured */
				return IL_THREAD_ERR_UNKNOWN;
			}
		}
		else
		{
			result = IL_THREAD_OK;
		}

		/* Now wait until the mutex can be acquired. */
		mutexResult = WaitForSingleObject(mon->_mutex, INFINITE);

		if(mutexResult == WAIT_OBJECT_0)
		{
			return result;
		}
		return IL_THREAD_ERR_UNKNOWN;
	}
	else
	{
		return IL_THREAD_ERR_INVALID_TIMEOUT;
	}
}

int _ILMonitorPulse(_ILMonitor *mon)
{
	if(mon)
	{
		ILInt32 waitValue;

		waitValue = ILInterlockedLoadI4(&(mon->_waitValue));
		if(waitValue < 0)
		{
			waitValue = ILInterlockedIncrementI4_Acquire(&(mon->_waitValue));
			if(waitValue <= 0)
			{
				ReleaseSemaphore(mon->_sem, 1, NULL);
			}
			else
			{
				ILInterlockedDecrementI4_Acquire(&(mon->_waitValue));
			}
		}
		return IL_THREAD_OK;
	}
	return IL_THREAD_ERR_UNKNOWN;
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
				ReleaseSemaphore(mon->_sem, -waitValue, NULL);
			}
			else if(waitValue > 0)
			{
				ILInterlockedAddI4_Acquire(&(mon->_waitValue), waitValue);
			}
		}
		return IL_THREAD_OK;
	}
	return IL_THREAD_ERR_UNKNOWN;
}

#ifdef	__cplusplus
};
#endif

#endif	/* IL_USE_WIN32_THREADS */
