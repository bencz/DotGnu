/*
 * wait_event.c - Wait event objects for the threading sub-system.
 *
 * Copyright (C) 2002 Free Software Foundation
 *
 * Authors: Thong Nguyen
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
#include "stdio.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Handler for cleaning up the an event.with.
 */
static int EventClose(ILWaitEvent *event)
{
	/* Clean up the event */	
	_ILWakeupQueueDestroy(&(event->queue));
	_ILMutexDestroy(&(event->parent.lock));

	return IL_WAITCLOSE_FREE;
}

/*
 * Handler for registering a wakeup listener an with an event.
 */
static int EventRegister(ILWaitEvent *event, _ILWakeup *wakeup)
{
	int result;

	/* Lock down the event */
	_ILCriticalSectionEnter(&(event->parent.lock));

	/* Determine what to do based on the event's current state */
	if((event->data & EVENT_SET_MASK))
	{
		/* Event is set, aquire is successful */

		/* check if we should auto reset the event */
		if (!(event->data & EVENT_MANUALRESET_MASK))
		{
			/* event is an auto-reset event so reset it */
			event->data &= ~(EVENT_SET_MASK);
		}

		result = IL_WAITREG_ACQUIRED;
	}
	else
	{
		/* Event isn't set, so add the wakup to the queue */

		if(_ILWakeupQueueAdd(&(event->queue), wakeup, event))
		{
			result = IL_WAITREG_OK;
		}
		else
		{
			result = IL_WAITREG_FAILED;
		}
	}

	/* Unlock the event and return */
	_ILCriticalSectionLeave(&(event->parent.lock));

	return result;
}

/*
 * Handler for unregistering a wakeup listener an with an event.
 */
static void EventUnregister(ILWaitEvent *event, _ILWakeup *wakeup, int release)
{
	/* Lock down the event */
	_ILCriticalSectionEnter(&(event->parent.lock));

	/* Remove ourselves from the wait queue if we are currently on it */
	_ILWakeupQueueRemove(&(event->queue), wakeup);
	
	/* Unlock the event and return */
	_ILCriticalSectionLeave(&(event->parent.lock));
}

static int EventSignal(ILWaitHandle *waitHandle)
{
	return ILWaitEventSet(waitHandle);
}

/*
 * The WaitHandleVtable for events
 */
static const _ILWaitHandleVtable _ILWaitEventVtable =
{
	IL_WAIT_EVENT,
	(ILWaitCloseFunc)EventClose,
	(ILWaitRegisterFunc)EventRegister,
	(ILWaitUnregisterFunc)EventUnregister,
	(ILWaitSignalFunc)EventSignal
};

/*
 * Creates and returns a new wait event.
 *
 * @param manualReset  If false (0), the event automatically resets itself
 *                     after a single thread has been released.
 *
 * @param initialState The initial state of the event.  If true (1) The initial
 *                     state of the event is signalled.
 */
ILWaitHandle *ILWaitEventCreate(int manualReset, int initialState)
{
	ILWaitEvent *event;

	/* Allocate memory for the event */
	if((event = (ILWaitEvent *)ILMalloc(sizeof(ILWaitEvent))) == 0)
	{
		return 0;
	}

	_ILMutexCreate(&(event->parent.lock));

	/* setup event callbacks */
	event->parent.vtable = &_ILWaitEventVtable;

	_ILWakeupQueueCreate(&(event->queue));

	/* Set the event initialstate/manualreset flags */
	event->data = (initialState) ? 1 : 0;
	event->data |= (manualReset) ? 2 : 0;

	return &(event->parent);
}

/*
 * Sets the state of an event to signalled.
 *
 * @param handle  The pointer to the wait event.
 * @returns 1 if successful.
 */
int ILWaitEventSet(ILWaitHandle *handle)
{
	ILWaitEvent *event = (ILWaitEvent *)handle;

	if( event == 0 ) 
	{
		return 0;
	}
	
	/* Lock down the event */
	_ILCriticalSectionEnter(&(event->parent.lock));

	if (!(event->data & (EVENT_SET_MASK)))
	{		
		if (event->queue.first == 0)
		{
			/* No threads are waiting so just set the event */

			event->data |= (EVENT_SET_MASK);
		}
		else
		{
			/* One or more threads must be waiting */
			
			/* Check if this event is manually reset */
			if (event->data & (EVENT_MANUALRESET_MASK))
			{
				event->data |= (EVENT_SET_MASK);

				_ILWakeupQueueWakeAll(&event->queue);
			}
			else
			{
				/* No need to set the event mask since a single thread is going to be released */
				_ILWakeupQueueWake(&event->queue);
			}
		}
	}
	
	/* Unlock the event and return */
	_ILCriticalSectionLeave(&(event->parent.lock));

	return 1;
}

/*
* Pulses the event.
*
* @param handle  The pointer to the wait event.
* @returns 1 if successful.
*/
int ILWaitEventPulse(ILWaitHandle *handle)
{
	ILWaitEvent *event = (ILWaitEvent *)handle;

	if( event == 0 ) 
	{
		return 0;
	}
	
	/* Lock down the event */
	_ILCriticalSectionEnter(&(event->parent.lock));

	if (!(event->data & (EVENT_SET_MASK)))
	{		
		if (event->queue.first == 0)
		{
			/* No threads are waiting so just exit */
		}
		else
		{
			/* One or more threads must be waiting */

			/* Check if this event is manually reset */
			if (event->data & (EVENT_MANUALRESET_MASK))
			{
				_ILWakeupQueueWakeAll(&event->queue);
			}
			else
			{
				_ILWakeupQueueWake(&event->queue);
			}
		}
	}

	/* Unlock the event and return */
	_ILCriticalSectionLeave(&(event->parent.lock));

	return 1;
}

/*
 * Sets the state of the mutex to unsignalled.
 *
 * @param handle  The pointer to the wait event.
 * @returns 1 if successful.
 */
int ILWaitEventReset(ILWaitHandle *handle)
{
	ILWaitEvent *event = (ILWaitEvent *)handle;

	if( event == 0 ) 
	{
		return 0;
	}
	
	/* Lock down the event */
	_ILCriticalSectionEnter(&(event->parent.lock));

	/* Reset the event */
	event->data &= ~(EVENT_SET_MASK);
	
	/* Unlock the event and return */
	_ILCriticalSectionLeave(&(event->parent.lock));

	return 1;
}

#ifdef	__cplusplus
};
#endif
