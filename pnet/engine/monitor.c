/*
 * monitor.c - Routines for monitors within the runtime.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Authors:  Thong Nguyen (tum@veridicus.com)
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

#include "engine.h"
#include "lib_defs.h"
#include "../support/interlocked.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * See engine/lib_monitor.c for notes on the monitor algorithm.
 */

#ifdef IL_CONFIG_USE_THIN_LOCKS

#define DEFAULT_HASHTABLE_SIZE		(523)

/*
 *	An entry in the monitor Hashtable.
 */
typedef struct _tagILMonitorEntry ILMonitorEntry;

struct _tagILMonitorEntry
{
	volatile ILObject *obj;
	volatile ILLockWord lockWord;
	volatile ILMonitorEntry *next;
};

#endif /* IL_CONFIG_USE_THIN_LOCKS */

/*
 * Intialize the monitor system for this process.
 */
int _ILExecMonitorProcessCreate(ILExecProcess *process)
{
#ifdef IL_CONFIG_USE_THIN_LOCKS	
	ILMonitorEntry **table;

	/* Initialize the monitor system lock */
	process->monitorSystemLock = ILMutexCreate();
	
	if(!(process->monitorSystemLock))
	{		
		return 0;
	}

	table = (ILMonitorEntry **)ILGCAlloc(sizeof(ILMonitorEntry *)
		* DEFAULT_HASHTABLE_SIZE);

	if (table == 0)
	{
		ILMutexDestroy(process->monitorSystemLock);

		return 0;
	}

	process->monitorTable = table;

	return 1;	
#else
	return 1;
#endif
}

/*
 * Clean up the monitor system for this process.
 */
int _ILExecMonitorProcessDestroy(ILExecProcess *process)
{
#ifdef IL_CONFIG_USE_THIN_LOCKS
	if (process->monitorSystemLock)
	{
		ILMutexDestroy(process->monitorSystemLock);
	}
	return 1;
#else
	return 1;
#endif
}

/*
 * Finalizer for monitors.
 */
static void ILExecMonitorFinalizer(void *block, void *data)
{
	ILExecMonitor *monitor = (ILExecMonitor *)block;

	ILWaitHandleClose(monitor->waitMutex);
}

/*
 * Create a new monitor.
 */
ILExecMonitor *_ILExecMonitorCreate(void)
{
	ILExecMonitor *monitor;

	/* Allocate memory for the ILExecMonitor */
	if((monitor = (ILExecMonitor *)ILGCAlloc(sizeof(ILExecMonitor))) == 0)
	{
		return 0;
	}

	/* Allocate memory for the wait monitor */
	if((monitor->waitMutex = ILWaitMonitorCreate()) == 0)
	{
		ILFree(monitor);

		return 0;
	}

	monitor->waiters = 0;
	monitor->next = 0;
	
	ILGCRegisterFinalizer(monitor, ILExecMonitorFinalizer, 0);

	return monitor;
}

#ifdef IL_CONFIG_USE_THIN_LOCKS

/*
 *	Gets a pointer to the WaitHandle object used by the object.
 */
static IL_INLINE ILLockWord *GetObjectLockWordPtr(ILExecThread *thread, ILObject *obj)
{
	ILNativeInt x;
	volatile ILMonitorEntry **table, *entry, *preventry, *ghost, *ghostparent;

	ILMutexLock(thread->process->monitorSystemLock);

	table = thread->process->monitorTable;

	/* Get the hashtable index */

	x = (ILNativeInt)obj;
	
	x = x ^ (x / sizeof(int));
	
	x %= DEFAULT_HASHTABLE_SIZE;
	
	entry = table[x];
	
	ghost = 0;
	ghostparent = 0;
	preventry = 0;

	for (;;)
	{
		if (entry == 0)
		{
			if (ghost == 0)
			{					
				/* No ghost found.  Create a whole new entry. */

				if ((entry = (ILMonitorEntry *)ILGCAlloc(sizeof(ILMonitorEntry))) == 0)
				{
					ILMutexUnlock(thread->process->monitorSystemLock);

					ILExecThreadThrowOutOfMemory(thread);

					return 0;
				}
			}
			else
			{
				/* Use the entry that no longer points
				to a live object */

				entry = ghost;

				/* Remove ghost from list */
				if (ghostparent == 0)
				{
					table[x] = ghost->next;
				}
				else
				{
					ghostparent->next = ghost->next;
				}

				ILGCUnregisterWeak((void *)&entry->obj);
			}

			/* Setup the new entry */

			entry->obj = obj;
			entry->next = table[x];
			entry->lockWord = 0;
			
			table[x] = entry;

			/* Tells the GC to zero entry->obj if obj is GC-ed */
			ILGCRegisterGeneralWeak((void *)&entry->obj, obj);

			ILMutexUnlock(thread->process->monitorSystemLock);

			return (ILLockWord *)&entry->lockWord;
		}
		else if (entry->obj == obj)
		{
			/* Entry was found.  Return it */

			ILMutexUnlock(thread->process->monitorSystemLock);

			return (ILLockWord *)&entry->lockWord;
		}
		else if (entry->obj == 0)
		{
			/* Found an entry pointing to a dead object */

			if (ghost == 0)
			{
				ghost = entry;

				/* Save the parent so we can remove the
					ghost from the list if it is used */
					   				
				ghostparent = preventry;
			}
		}

		preventry = entry;			
		entry = entry->next;
	}

	ILMutexUnlock(thread->process->monitorSystemLock);
	
	return 0;
}

/*
 *	Implementation of GetObjectLockWord using hashtables.
 */
ILLockWord GetObjectLockWord(ILExecThread *thread, ILObject *obj)
{
	return *GetObjectLockWordPtr(thread, obj);
}

/*
 *	Implementation of SetObjectLockWord using hashtables.
 */
void SetObjectLockWord(ILExecThread *thread, ILObject *obj, ILLockWord value)
{
	*GetObjectLockWordPtr(thread, obj) = value;
}

/*
 *	Implementation of CompareAndExchangeObjectLockWord using hashtables.
 */
ILLockWord CompareAndExchangeObjectLockWord(ILExecThread *thread, 
							ILObject *obj, ILLockWord value, ILLockWord comparand)
{
	ILLockWord oldValue;
	ILLockWord *lockWordPtr;

	/* Lockdown the monitor hashtable */
	ILMutexLock(thread->process->monitorSystemLock);
	
	lockWordPtr = GetObjectLockWordPtr(thread, obj);

	/* Do the compare & exchange */
	if ((oldValue = *lockWordPtr ) == comparand)
	{
		*lockWordPtr  = value;
	}

	/* Unlock the monitor hashtable */
	ILMutexUnlock(thread->process->monitorSystemLock);

	return oldValue;
}

#endif  /* IL_CONFIG_USE_THIN_LOCKS */

#ifdef	__cplusplus
};
#endif
