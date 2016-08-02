/*
 * lib_monitor.c - Internalcall methods for "System.Threading.Monitor".
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Authors:  Thong Nguyen (tum@veridicus.com) 
 * Lots of tips from Russell Stuart (rstuart@stuart.id.au)
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
#include "wait_mutex.h"
#include "interlocked.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * There are two methods of associating monitors with objects.
 * The standard method stores a pointer to the monitor in the object header.
 * The "thin-lock" method uses a hashtable to associate the monitor with the
 * object.
 * The standard method is faster but requires an extra word for every object
 * even if the object is never used for synchronization.  The thin-lock method
 * is slower but doesn't require an extra word for every object so is better
 * on platforms with limited memory or with programs that create thousands
 * (or millions) of tiny objects or with programs that don't use make much
 * use of synchronization (remember: pnetlib uses synchronization).
 *
 * The standard algorithm is designed to eliminate context switches into kernel
 * mode by avoiding "fat" OS-locks (therefore avoiding system calls).
 * The algorithm stores a pointer to an object's monitor in the object header
 * This pointer is known as the object's lockword.
 * If an object's lockword is 0, the algorithm knows *for sure* that the object is
 * unlocked and will then use a compare-and-exchange to to attach a monitor
 * to the object.  When a thread completely exits a monitor, the monitor is
 * returned to a thread-local free list and the objects' lockword is reset to 0.
 * Because objects are always properly aligned on word boundaries, the first
 * (and second) bits of the pointer/object-lockword are always zero allowing us to 
 * utilize them to store other bits of information.  The first bit of the 
 * lockword is used as a spin lock so that the algorithm can, at necessary 
 * critical sections, prevent other thread from changing the monitor and/or
 * lockword.  There are critical (but extremely unlikely) points where a monitor 
 * may never be returned to the thread-local free list even if the monitor is fully 
 * released.  This isn't actually a problem because monitors are always allocated 
 * on the GC heap.  If the object is locked and unlocked again, the monitor may 
 * get another chance to be returned to the free list.  If the object becomes 
 * garbage and is collected then then monitor will eventually be collected as well.
 *
 * Both algorithms uses 3 main functions: GetObjectLockWord, SetObjectLockWord and 
 * CompareAndExchangeObjectLockWord.  The implementation of these functions is
 * trivial for the standard algorithm and is implemented as macros in
 * engine/lib_def.h.
 * On some platforms, CompareAndExchangeObjectLockWord may use a global lock
 * because it uses ILInterlockedCompareAndExchangePointers which may not be
 * ported to that platform (see support/interlocked.h).  
 * CompareAndExchangeObjectLockWord always uses a global lock if the thin-lock
 * algorithm is used.
 *
 * The algorithm ASSUMES ILInterlockedCompareAndExchangePointers acts as a
 * memory barrier.
 *
 * The thin-lock algorithm is designed to be easily "plugged in" so it uses
 * the concept of lockwords as well as locking of lockwords except that the
 * lockword is stored in a hashtable instead of the object header.  Instead of
 * being defined as macros, the implementation of GetObjectLockWord,
 * SetObjectLockWord and CompareAndExchangeObjectLockWord are defined as functions
 * and their definitions are in engine/monitor.c.
 *
 * This file includes support/wait_mutex.h so that it can have fast access
 * to certain data structures.  These structures should never be accessed
 * directly but should be accessed through MACROs or inline functions
 * defined in support/wait_mutex.h.
 *
 * You can enable thin-locks by configuring pnet with the full-tl profile (1).
 *
 * ./configure --with-profile=full-tl
 *
 * When thin locks are configured, IL_CONFIG_USE_THIN_LOCKS is defined.
 *
 * (1) See the profiles directory for more information about profiles.
 *
 * - Thong Nguyen (tum@veridicus.com) aka Tum
 */

/*
 * TODO: #define out bits of code that isn't needed if IL_NO_THREADS is defined.
 * It may not be possible to remove everything (if anything) because even on
 * single threaded systems, a thread should still be able to interrupt/abort
 * itself so Monitor.Enter and Monitor.Wait still needs to call the underlying
 * methods that check for aborts/interrupts.
 */

/*
 * public static void Enter(Object obj);
 */
void _IL_Monitor_Enter(ILExecThread *thread, ILObject *obj)
{
	_IL_Monitor_InternalTryEnter(thread, obj, IL_WAIT_INFINITE);
}

/*
 * public static bool InternalTryEnter(Object obj, int timeout);
 *
 * The acquisition algorithm used is designed to be only require
 * a compare-and-exchange when the monitor is uncontested
 * (the most likely case).
 */
ILBool _IL_Monitor_InternalTryEnter(ILExecThread *thread,
									ILObject *obj, ILInt32 timeout)
{
	int result;
	
	/* Make sure the object isn't null */
	if(obj == 0)
	{
		ILExecThreadThrowArgNull(thread, "obj");
		return 0;
	}

	/* Make sure the timeout is within range */
	if (timeout < -1)
	{
		ILExecThreadThrowArgRange(thread, "timeout", (const char *)0);
		return 0;
	}

	result = ILMonitorTimedTryEnter((void **)GetObjectLockWordPtr(thread, obj),
									timeout);

	if((result != IL_THREAD_OK) && (result != IL_THREAD_BUSY))
	{
		_ILExecThreadHandleError(thread, result);
	}
	
	return (result == IL_THREAD_OK);
}

/*
 * public static void Exit(Object obj);
 */
void _IL_Monitor_Exit(ILExecThread *thread, ILObject *obj)
{
	int result;

	/* Make sure obj isn't null */
	if(obj == 0)
	{
		ILExecThreadThrowArgNull(thread, "obj");

		return;
	}

	result = ILMonitorExit((void **)GetObjectLockWordPtr(thread, obj));

	if(result != IL_THREAD_OK)
	{
		_ILExecThreadHandleError(thread, result);
	}
}

/*
 * public static bool InternalWait(Object obj, int timeout);
 */
ILBool _IL_Monitor_InternalWait(ILExecThread *thread,
								ILObject *obj, ILInt32 timeout)
{
	int result;

	/* Make sure obj isn't null */
	if (obj == 0)
	{
		ILExecThreadThrowArgNull(thread, "obj");
		return 0;
	}

	result = ILMonitorTimedWait((void **)GetObjectLockWordPtr(thread, obj), timeout);

	if((result != IL_THREAD_OK) && (result != IL_THREAD_BUSY))
	{
		_ILExecThreadHandleError(thread, result);
	}

	return (result == IL_THREAD_OK);
}

/*
 * public static void Pulse(Object obj);
 */
void _IL_Monitor_Pulse(ILExecThread *thread, ILObject *obj)
{
	int result;

	if(obj == 0)
	{
		ILExecThreadThrowArgNull(thread, "obj");
		return;
	}

	result = ILMonitorPulse((void **)GetObjectLockWordPtr(thread, obj));
	
	if(result != IL_THREAD_OK)
	{
		_ILExecThreadHandleError(thread, result);
	}
}

/*
 * public static void PulseAll(Object obj);
 */
void _IL_Monitor_PulseAll(ILExecThread *thread, ILObject *obj)
{
	int result;

	if(obj == 0)
	{
		ILExecThreadThrowArgNull(thread, "obj");
		return;
	}

	result = ILMonitorPulseAll((void **)GetObjectLockWordPtr(thread, obj));

	if(result != IL_THREAD_OK)
	{
		_ILExecThreadHandleError(thread, result);
	}
}

#ifdef	__cplusplus
};
#endif
