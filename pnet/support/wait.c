/*
 * wait.c - Wait handle objects for the threading sub-system.
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

/*

Note: the code in this module is generic to all platforms.  It implements
the correct CLI wait handle semantics based on the primitives in "*_defs.h".
You normally won't need to modify or replace this file when porting.

*/

#include "thr_defs.h"
#include "interlocked.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int ILWaitHandleClose(ILWaitHandle *handle)
{
	int result = (*_ILWaitHandle_closeFunc(handle))(handle);
	if(result == IL_WAITCLOSE_FREE)
	{
		ILFree(handle);
	}
	return (result != IL_WAITCLOSE_OWNED);
}

/*
 * Enter the "wait/sleep/join" state on the current thread.
 * NOTE: _ILWakeupCancelInterrupt doesn't read or change the thread's state.
 */
int _ILEnterWait(ILThread *thread)
{
	int result = 0;
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
	if ((threadState.split.pub & (IL_TS_INTERRUPTED_OR_ABORT_REQUESTED)) != 0)
	{	
		_ILCriticalSectionEnter(&(thread->lock));

		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
		if((threadState.split.pub & (IL_TS_ABORT_REQUESTED)) != 0)
		{
			/* Clear the wait/sleep/join state and interrupted flag */
			threadState.split.priv |= IL_TS_WAIT_SLEEP_JOIN;
			threadState.split.pub &= ~(IL_TS_INTERRUPTED);
			ILInterlockedStoreU4(&(thread->state.comb), threadState.comb);

			result = IL_WAIT_ABORTED;

			_ILWakeupCancelInterrupt(&(thread->wakeup));
		}

		_ILCriticalSectionLeave(&(thread->lock));
	}

	return result;
}

/*
 * Leave the "wait/sleep/join" state on the current thread.
 * NOTE: _ILWakeupCancelInterrupt doesn't read or change the thread's state.
 */
int _ILLeaveWait(ILThread *thread, int result)
{
	_ILThreadState threadState;

	/* The double checks for result == IL_WAIT_* are needed to bubble down
	   results even if the threadstate has been reset which may happen
	   if enter/leavewait are called recursively */

	threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
	if ((threadState.split.pub & (IL_TS_ABORT_REQUESTED | IL_TS_INTERRUPTED | IL_TS_SUSPEND_REQUESTED)) != 0)
	{
		_ILCriticalSectionEnter(&(thread->lock));

		threadState.comb = ILInterlockedLoadU4(&(thread->state.comb));

		/* Abort has more priority over interrupt */
		if(((threadState.split.pub & IL_TS_ABORT_REQUESTED) != 0) ||
		   result == IL_WAIT_ABORTED)
		{
			result = IL_WAIT_ABORTED;
		}
		else if(((threadState.split.pub & IL_TS_INTERRUPTED) != 0) ||
				result == IL_WAIT_INTERRUPTED)
		{
			result = IL_WAIT_INTERRUPTED;
		}

		_ILWakeupCancelInterrupt(&(thread->wakeup));
	
		if((threadState.split.pub & IL_TS_SUSPEND_REQUESTED) != 0)
		{
			threadState.split.pub &= ~(IL_TS_SUSPEND_REQUESTED | IL_TS_INTERRUPTED);
			threadState.split.pub |= (IL_TS_SUSPENDED | IL_TS_SUSPENDED_SELF);
			threadState.split.priv &= ~IL_TS_WAIT_SLEEP_JOIN;

			ILInterlockedStoreU4(&(thread->state.comb), threadState.comb);

			/* Unlock the thread object prior to suspending */
			_ILCriticalSectionLeave(&(thread->lock));

			/* Suspend until we receive notification from another thread */
			_ILThreadSuspendSelf(thread);

			return result;
		}
		threadState.split.priv &= ~IL_TS_WAIT_SLEEP_JOIN;
		
		ILInterlockedStoreU4(&(thread->state.comb), threadState.comb);

		_ILCriticalSectionLeave(&(thread->lock));
	}
	else
	{
		threadState.split.priv &= ~IL_TS_WAIT_SLEEP_JOIN;
		ILInterlockedStoreU2(&(thread->state.split.priv), threadState.split.priv);
	}
	return result;
}

/*
 * Leave the "wait/sleep/join" state, and release a wait
 * handle if the leave was interrupted or aborted.  This
 * is used when we thought we acquired the handle, but the
 * leave then fails.
 */
static int _ILLeaveWaitHandle(ILThread *thread, ILWaitHandle *handle, int ok)
{
	int result = _ILLeaveWait(thread, 0);
	if(result != 0)
	{
		(*_ILWaitHandle_unregisterFunc(handle))(handle, &(thread->wakeup), 1);
		return result;
	}
	else
	{
		return ok;
	}
}

int ILSignalAndWait(ILWaitHandle *signalHandle, ILWaitHandle *waitHandle, ILUInt32 timeout)
{
	ILThread *thread = _ILThreadGetSelf();
	_ILWakeup *wakeup = &(thread->wakeup);
	int result;

	/* Enter the "wait/sleep/join" state */
	result = _ILEnterWait(thread);
	if(result != 0)
	{
		return result;
	}

	if (_ILWaitHandle_signalFunc(signalHandle) == 0)
	{
		return IL_WAIT_FAILED;
	}

	/* Set the limit for the thread's wakeup object */
	if(!_ILWakeupSetLimit(wakeup, 1))
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}

	/* Register this thread with the handle */
	result = (*_ILWaitHandle_registerFunc(waitHandle))(waitHandle, wakeup);
		
	_ILWaitHandle_signalFunc(signalHandle)(signalHandle);
	
	if(result == IL_WAITREG_ACQUIRED)
	{
		/* We were able to acquire the wait handle immediately */
		_ILWakeupAdjustLimit(wakeup, 0);		
		return _ILLeaveWaitHandle(thread, waitHandle, 0);
	}
	else if(result == IL_WAITREG_FAILED)
	{
		/* Something serious happened which prevented registration */
		_ILWakeupAdjustLimit(wakeup, 0);
		return _ILLeaveWait(thread, IL_WAIT_FAILED);
	}

	/* Wait until we are signalled, timed out, or interrupted */
	result = _ILWakeupWait(wakeup, timeout, 0);

	/* Unregister the thread from the wait handle */
	(*_ILWaitHandle_unregisterFunc(waitHandle))(waitHandle, wakeup, (result <= 0));

	/* Tell the caller what happened */
	if(result > 0)
	{
		/* We have to account for "_ILLeaveWait" detecting interrupt
		or abort after we already acquired the wait handle */
		return _ILLeaveWaitHandle(thread, waitHandle, 0);
	}
	else if(result == 0)
	{
		return _ILLeaveWait(thread, IL_WAIT_TIMEOUT);
	}
	else
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}
}

int ILWaitOne(ILWaitHandle *handle, ILUInt32 timeout)
{
	ILThread *thread = _ILThreadGetSelf();
	_ILWakeup *wakeup = &(thread->wakeup);
	int result;

	/* Enter the "wait/sleep/join" state */
	result = _ILEnterWait(thread);
	if(result != 0)
	{
		return result;
	}

	/* Set the limit for the thread's wakeup object */
	if(!_ILWakeupSetLimit(wakeup, 1))
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}

	/* Register this thread with the handle */
	result = (*_ILWaitHandle_registerFunc(handle))(handle, wakeup);

	if(result == IL_WAITREG_ACQUIRED)
	{
		/* We were able to acquire the wait handle immediately */
		_ILWakeupAdjustLimit(wakeup, 0);
		return _ILLeaveWaitHandle(thread, handle, 0);
	}
	else if(result == IL_WAITREG_FAILED)
	{
		/* Something serious happened which prevented registration */
		_ILWakeupAdjustLimit(wakeup, 0);
		return _ILLeaveWait(thread, IL_WAIT_FAILED);
	}

	/* Wait until we are signalled, timed out, or interrupted */
	result = _ILWakeupWait(wakeup, timeout, 0);

	/* Unregister the thread from the wait handle */
	(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, (result <= 0));

	/* Tell the caller what happened */
	if(result > 0)
	{
		/* We have to account for "_ILLeaveWait" detecting interrupt
		   or abort after we already acquired the wait handle */
		return _ILLeaveWaitHandle(thread, handle, 0);
	}
	else if(result == 0)
	{
		return _ILLeaveWait(thread, IL_WAIT_TIMEOUT);
	}
	else
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}
}

int ILWaitAny(ILWaitHandle **handles, ILUInt32 numHandles, ILUInt32 timeout)
{
	ILThread *thread = _ILThreadGetSelf();
	_ILWakeup *wakeup = &(thread->wakeup);
	int result;
	ILUInt32 index, index2;
	ILWaitHandle *handle;
	ILWaitHandle *resultHandle;

	/* Enter the "wait/sleep/join" state */
	result = _ILEnterWait(thread);
	if(result != 0)
	{
		return result;
	}

	/* Set the limit for the thread's wakeup object */
	if(!_ILWakeupSetLimit(wakeup, 1))
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}

	/* Register this thread with all of the wait handles */
	for(index = 0; index < numHandles; ++index)
	{
		handle = handles[index];
		result = (*_ILWaitHandle_registerFunc(handle))(handle, wakeup);
		if(result == IL_WAITREG_ACQUIRED)
		{
			/* We were able to acquire this wait handle immediately */
			_ILWakeupAdjustLimit(wakeup, 0);

			/* Unregister the handles we have registered so far */
			for(index2 = 0; index2 < index; ++index2)
			{
				handle = handles[index2];
				(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, 1);
			}
			return _ILLeaveWaitHandle(thread, handles[index], index);
		}
		else if(result == IL_WAITREG_FAILED)
		{
			/* Something serious happened which prevented registration */
			_ILWakeupAdjustLimit(wakeup, 0);

			/* Unregister the handles we have registered so far */
			for(index2 = 0; index2 < index; ++index2)
			{
				handle = handles[index2];
				(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, 1);
			}
			return _ILLeaveWait(thread, IL_WAIT_FAILED);
		}
	}

	/* Wait until we are signalled, timed out, or interrupted */
	resultHandle = 0;
	result = _ILWakeupWait(wakeup, timeout, (void *)&resultHandle);

	/* Unregister the thread from the wait handles */
	index2 = 0;
	for(index = 0; index < numHandles; ++index)
	{
		handle = handles[index];
		if(handle == resultHandle && result > 0)
		{
			index2 = index;
			(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, 0);
		}
		else
		{
			(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, 1);
		}
	}

	/* Tell the caller what happened */
	if(result > 0)
	{
		/* We have to account for "_ILLeaveWait" detecting interrupt
		   or abort after we already acquired the wait handle */
		return _ILLeaveWaitHandle(thread, resultHandle, (int)index2);
	}
	else if(result == 0)
	{
		return _ILLeaveWait(thread, IL_WAIT_TIMEOUT);
	}
	else
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}
}

int ILWaitAll(ILWaitHandle **handles, ILUInt32 numHandles, ILUInt32 timeout)
{
	ILThread *thread = _ILThreadGetSelf();
	_ILWakeup *wakeup = &(thread->wakeup);
	int result;
	ILUInt32 index, index2;
	ILWaitHandle *handle;
	ILUInt32 limit;

	/* Enter the "wait/sleep/join" state */
	result = _ILEnterWait(thread);
	if(result != 0)
	{
		return result;
	}

	/* Set the limit for the thread's wakeup object.  This may
	   be reduced later if we were able to acquire some objects
	   during the registration step */
	limit = numHandles;
	if(!_ILWakeupSetLimit(wakeup, limit))
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}

	/* Register this thread with all of the wait handles */
	for(index = 0; index < numHandles; ++index)
	{
		handle = handles[index];
		result = (*_ILWaitHandle_registerFunc(handle))(handle, wakeup);
		if(result == IL_WAITREG_ACQUIRED)
		{
			/* We were able to acquire this wait handle immediately */
			--limit;
		}
		else if(result == IL_WAITREG_FAILED)
		{
			/* Something serious happened which prevented registration */
			_ILWakeupAdjustLimit(wakeup, 0);

			/* Unregister the handles we have registered so far */
			for(index2 = 0; index2 < index; ++index2)
			{
				handle = handles[index2];
				(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, 1);
			}
			return _ILLeaveWait(thread, IL_WAIT_FAILED);
		}
	}

	/* Adjust the wait limit to reflect handles we already acquired */
	_ILWakeupAdjustLimit(wakeup, limit);
	
	if (limit == 0)
	{
		/* No need to wait since we managed to aquire every handle immediately. */		
		result = 1;
	}
	else
	{
		/* Wait until we are signalled, timed out, or interrupted */
		result = _ILWakeupWait(wakeup, timeout, 0);
	}

	/* Unregister the thread from the wait handles */
	for(index = 0; index < numHandles; ++index)
	{
		handle = handles[index];
		(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, (result <= 0));
	}

	/* Tell the caller what happened */
	if(result > 0)
	{
		/* We have to account for "_ILLeaveWait" detecting interrupt
		   or abort after we already acquired the wait handles */
		result = _ILLeaveWait(thread, 0);
		if(result == 0)
		{
			return 0;
		}
		else
		{
			for(index = 0; index < numHandles; ++index)
			{
				handle = handles[index];
				(*_ILWaitHandle_unregisterFunc(handle))(handle, wakeup, 1);
			}
			return result;
		}
	}
	else if(result == 0)
	{
		return _ILLeaveWait(thread, IL_WAIT_TIMEOUT);
	}
	else
	{
		return _ILLeaveWait(thread, IL_WAIT_INTERRUPTED);
	}
}

/*
 * Waits for a handle and if an interrupt or abort is encountered during the
 * wait sequence the function will return the proper wait error code but only
 * after it has managed to aquire the handle (by continuously retrying).
 * If the wait times out, the handle will not be aquired and the function will
 * return with either IL_WAIT_TIMEOUT, IL_WAIT_ABORTED or IL_WAIT_INTERRUPTED.
 */
int _ILWaitOneBackupInterruptsAndAborts(ILWaitHandle *handle, int timeout)
{
	ILThread *thread = _ILThreadGetSelf();
	int result, retval = 0;
	_ILThreadState threadState;

	threadState.comb = 0;
	for (;;)
	{
		_ILThreadState newThreadState;

		/* Wait to re-acquire the monitor (add ourselves to the "ready queue") */
		result = ILWaitOne(handle, timeout);
		
		if (result < IL_WAIT_TIMEOUT)
		{
			/* We were aborted or interrupted.  Save the thread state
				and keep trying to reaquire the monitor */

			_ILCriticalSectionEnter(&thread->lock);

			newThreadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
			threadState.comb |= newThreadState.comb;
			
			if (result == IL_WAIT_INTERRUPTED)
			{
				/* Interrupted is cleared by ILWaitOne so save it manually */
				threadState.split.pub |= IL_TS_INTERRUPTED;
			}
			
			newThreadState.split.pub &= ~(IL_TS_INTERRUPTED_OR_ABORT_REQUESTED);

			if (result < retval)
			{
				retval = result;
			}

			ILInterlockedStoreU4(&(thread->state.comb), newThreadState.comb);

			_ILCriticalSectionLeave(&thread->lock);
			
			continue;
		}
		else
		{	
			if (threadState.comb != 0)
			{
				_ILCriticalSectionEnter(&thread->lock);

				/* Set the thread state to the thread state that was stored 
					and clear the interrupted flag */
				newThreadState.comb = ILInterlockedLoadU4(&(thread->state.comb));
				newThreadState.comb |= (threadState.comb & ~IL_TS_INTERRUPTED);
				ILInterlockedStoreU4(&(thread->state.comb), newThreadState.comb);
				
				_ILCriticalSectionLeave(&thread->lock);
			}	
			else
			{
				retval = result;
			}
			
			return retval;
		}
	}
}

#ifdef	__cplusplus
};
#endif
