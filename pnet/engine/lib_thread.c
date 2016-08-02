/*
 * lib_thread.c - Internalcall methods for "System.Threading.*".
 *
 * Copyright (C) 2001, 2002, 2003, 2011  Southern Storm Software, Pty Ltd.
 *
 * Contributions from Thong Nguyen <tum@veridicus.com>
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
#include "interlocked.h"

#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#if HAVE_UNISTD_H
	#include <unistd.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Comments on terminology:
 *
 * Support Thread:	Refers to an ILThread instance.
 * Engine Thread:		Refers to an ILExecThread instance.
 * CLR Thread:		Refers to a System.Threading.Thread instance. 
 */

/*
 * Timeout value the managed WaitHandle uses.
 */
#define IL_CLR_WAIT_TIMEOUT (258L)


/*************************************/
/* Interlocked functions for ILInt32 */
/*************************************/

/*
 * public static int CompareExchange(ref int location1, int value,
 *									 int comparand);
 */
ILInt32 _IL_Interlocked_CompareExchange_Riii
						(ILExecThread *thread,
						 ILInt32 *location1,
						 ILInt32 value,
						 ILInt32 comparand)
{
	return ILInterlockedCompareAndExchangeI4_Full(location1, value, comparand);
}

/*
* public static int Increment(ref int location);
*/
ILInt32 _IL_Interlocked_Increment_Ri(ILExecThread *thread, ILInt32 *location)
{
	return ILInterlockedIncrementI4_Full(location);
}


/*
* public static int Decrement(ref int location);
*/
ILInt32 _IL_Interlocked_Decrement_Ri(ILExecThread *thread, ILInt32 *location)
{
	return ILInterlockedDecrementI4_Full(location);
}

/*
 * public static int Exchange(ref int location, int value);
 */
ILInt32 _IL_Interlocked_Exchange_Rii(ILExecThread *thread,
									 ILInt32 *location, ILInt32 value)
{
	return ILInterlockedExchangeI4_Full(location, value);
}

/*************************************/
/* Interlocked functions for ILInt64 */
/*************************************/

/*
 * public static long Increment(ref long location);
 */
ILInt64 _IL_Interlocked_Increment_Rl(ILExecThread *thread, ILInt64 *location)
{
	return ILInterlockedIncrementI8_Full(location);
}

/*
 * public static long Decrement(ref long location);
 */
ILInt64 _IL_Interlocked_Decrement_Rl(ILExecThread *thread, ILInt64 *location)
{
	return ILInterlockedDecrementI8_Full(location);
}

/*************************************/
/* Interlocked functions for ILFloat */
/*************************************/

/*
 * public static float Exchange(ref float location, float value);
 */
ILFloat _IL_Interlocked_Exchange_Rff(ILExecThread *thread,
								     ILFloat *location, ILFloat value)
{
	return ILInterlockedExchangeR4_Full(location, value);
}

/*
 * public static float CompareExchange(ref float location1, float value,
 *									   float comparand);
 */
ILFloat _IL_Interlocked_CompareExchange_Rfff(ILExecThread *thread, ILFloat *location1,
						ILFloat value, ILFloat comparand)
{
	return ILInterlockedCompareAndExchangeR4_Full(location1, value, comparand);
}

/***************************************/
/* Interlocked functions for ILObject* */
/***************************************/

/*
 * public static Object Exchange(ref Object location, Object value);
 */
ILObject *_IL_Interlocked_Exchange_RObjectObject
(ILExecThread *thread, ILObject **location, ILObject *value)
{
	return (ILObject *)ILInterlockedExchangeP_Full((void **)location,
												   (void *)value);
}

/*
 * public static Object CompareExchange(ref Object location1, Object value,
 *									    Object comparand);
 */
ILObject *_IL_Interlocked_CompareExchange_RObjectObjectObject
						(ILExecThread *thread,
						 ILObject **location1,
						 ILObject *value,
						 ILObject *comparand)
{
	return (ILObject *)ILInterlockedCompareAndExchangeP_Full((void **)location1, (void *)value, (void *)comparand);
}

/*
 *  Thread function for ILThread.
 *
 *  @param objectArg	The ILExecThread for the new thread.
 */
static void __PrivateThreadStart(void *objectArg)
{
	ILExecValue result;
	ILExecThread *thread;

	thread = ILExecThreadCurrent();

#ifdef IL_USE_JIT
	/* Set the exception handler which converts builtin
	   libjit exceptions into clr exceptions */
	jit_exception_set_handler(_ILJitExceptionHandler);
#endif

	_ILCallMethod
		(
			thread,
			((System_Thread *)thread->clrThread)->start->methodInfo,
			_ILCallUnpackDirectResult,
			&result,
			0,
			((System_Thread *)thread->clrThread)->start->target,
			_ILCallPackVParams,
			0
		);

	/* Print out any uncaught exceptions */
	if (_ILExecThreadHasException(thread)
		&& !ILExecThreadIsThreadAbortException(thread, _ILExecThreadGetException(thread)))
	{				
		ILExecThreadPrintException(thread);
	}

	ILThreadClearStack(4096);

	/* Clear the thread data needed only during execution */
	_ILExecThreadClearExecutionState(thread);
}

/*
 * private void InitializeThread();
 */
void _IL_Thread_InitializeThread(ILExecThread *thread, ILObject *_this)
{
	ILThread *supportThread;

	if (((System_Thread *)_this)->privateData != NULL)
	{
		/* InitializeThread can't be called more than once */

		ILExecThreadThrowSystem(thread, "System.Threading.ThreadStateException", (const char *)0);

		return;
	}
	
	if (!ILHasThreads())
	{
		/* Threading isn't supported */

		ILExecThreadThrowSystem(thread, "System.NotSupportedException", 
			"Exception_ThreadsNotSupported");

		return;
	}

	/* Create a new support thread */

	if ((supportThread = ILThreadCreate(__PrivateThreadStart, 0)) == 0)
	{
		/* Threading is supported but creation failed.
			ThrowOutOfMemoryException */
			
		ILExecThreadThrowOutOfMemory(thread);
		
		return;
	}

	/* Associate the support thread with the CLR thread */
	((System_Thread *)_this)->privateData = supportThread;
}

/*
 * private void FinalizeThread();
 */
void _IL_Thread_FinalizeThread(ILExecThread *thread, ILObject *_this)
{
	ILThread *supportThread;
	System_Thread *clrThread;

	clrThread = (System_Thread *)_this;
	supportThread = clrThread->privateData;
	
	/* Only threads created from managed code should destroy the underlying
	   support thread.  Threads created outside of managed code but which
	   enter managed code are the reponsibility of the unmanaged code that
	   created them. */
	if (supportThread && clrThread->createdFromManagedCode)
	{
		clrThread->privateData = 0;

		ILThreadDestroy(supportThread);
	}
}

/*
 * public void Abort();
 */
void _IL_Thread_Abort(ILExecThread *thread, ILObject *_this)
{
	_ILExecThreadAbortThread(thread, ((System_Thread *)_this)->privateData);
}

/*
 * public void Interrupt();
 */
void _IL_Thread_Interrupt(ILExecThread *thread, ILObject *_this)
{
	ILThreadInterrupt(((System_Thread *)_this)->privateData);

	/* If the current thread is interrupting itself then it will
	   interrupt next time it enters a wait/sleep/join state */
}

/*
 * public void Suspend();
 */
void _IL_Thread_Suspend(ILExecThread *thread, ILObject *_this)
{
	_ILExecThreadSuspendThread(thread, ((System_Thread *)_this)->privateData);
}

/*
 * public void Resume();
 */
void _IL_Thread_Resume(ILExecThread *thread, ILObject *_this)
{	
	_ILExecThreadResumeThread(thread, ((System_Thread *)_this)->privateData);
}

/*
 * public static void SpinWait(int iterations);
 */
void _IL_Thread_SpinWait(ILExecThread *_thread, ILInt32 iterations)
{
	/*
	 * Spin wait is used to yield CPU time from one logical
	 * CPU to another logical CPU in a hyper-threaded CPU.
	 * see: http://blogs.msdn.com/cbrumme/archive/2003/04/15/51321.aspx
	 */

	if(iterations > 0)
	{
		/*
		 * Can't use ILThreadSleep because it will catch interrupts
		 * and we don't want it to do that.
		 *
		 * Also, usleep() isn't thread safe on some platforms.
		 */
		while ((iterations--) > 0)
		{
			ILThreadYield();
		}
	}
}

/*
 * private bool InternalJoin(int timeout);
 */
ILBool _IL_Thread_InternalJoin(ILExecThread *thread, ILObject *_this,
							   ILInt32 timeout)
{	
	ILThread *supportThread;

	supportThread = ((System_Thread *)_this)->privateData;
	
	switch (ILThreadJoin(supportThread, timeout))
	{	
	case IL_JOIN_OK:
	case IL_JOIN_SELF:		
		return 1;
	case IL_JOIN_TIMEOUT:		
		return 0;
	case IL_JOIN_MEMORY:		
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	case IL_JOIN_UNSTARTED:
		ILExecThreadThrowSystem
			(
				thread,
				"System.Threading.ThreadStateException",
				(const char *)0
			);
		return 0;
	case IL_JOIN_INTERRUPTED:
		
		ILExecThreadThrowSystem
			(
				thread,
				"System.Threading.ThreadInterruptedException",
				(const char *)0
			);
		
		return 0;

	case IL_JOIN_ABORTED:

		_ILExecThreadSelfAborting(thread);

		return 0;

	default:	

		ILExecThreadThrowSystem
			(
				thread,
				"System.Threading.ThreadStateException",
				(const char *)0
			);

		return 0;
	}
}

/*
 * public static void MemoryBarrier();
 */
void _IL_Thread_MemoryBarrier(ILExecThread *thread)
{
	ILInterlockedMemoryBarrier();
}

/*
 * public static void ResetAbort();
 */
void _IL_Thread_ResetAbort(ILExecThread *thread)
{
	if (!ILThreadIsAborting())
	{
		/* No abort has been requested */

		ILExecThreadThrowSystem(thread,
			"System.Threading.ThreadStateException", (const char *)0);

		return;
	}

	if (ILThreadAbortReset())
	{
		ILInterlockedAndU4(&(thread->managedSafePointFlags),
						   ~_IL_MANAGED_SAFEPOINT_THREAD_ABORT);
		thread->aborting = 0;
	}
}

/*
 * public static void InternalSleep(int timeout);
 */
void _IL_Thread_InternalSleep(ILExecThread *thread, ILInt32 timeout)
{
	int result;
	
	if (timeout < -1)
	{
		ILExecThreadThrowSystem(thread,
			"System.ArgumentOutOfRangeException", (const char *)0);
	}
	
	result = ILThreadSleep((ILUInt32)timeout);
	_ILExecThreadHandleError(thread, result);
}

/*
 * public void Start();
 */
void _IL_Thread_Start(ILExecThread *thread, ILObject *_this)
{
	ILThread *supportThread;
	ILExecThread *execThread;
	int result;
	
	/* Get the support thread stored inside the first field of the 
	CLR thread by _IL_Thread_InitializeThread */

	supportThread = ((System_Thread *)_this)->privateData;

	result = ILMonitorEnter((void **)GetObjectLockWordPtr(thread, _this));
	if(result != IL_THREAD_OK)
	{
		_ILExecThreadHandleError(thread, result);
		return;
	}

	if (supportThread == 0
		|| (ILThreadGetState(supportThread) & IL_TS_UNSTARTED) == 0)
	{
		/* Thread has already been started or has ended. */

		result = ILMonitorExit((void **)GetObjectLockWordPtr(thread, _this));
		if(result != IL_THREAD_OK)
		{
			_ILExecThreadHandleError(thread, result);
			return;
		}

		ILExecThreadThrowSystem(thread, "System.Threading.ThreadStateException", 
			"Exception_ThreadAlreadyStarted");

		return;
	}

	/* Register the support thread for managed code execution */

	if ((execThread = ILThreadRegisterForManagedExecution(thread->process, supportThread)) == 0)
	{
		result = ILMonitorExit((void **)GetObjectLockWordPtr(thread, _this));
		if(result != IL_THREAD_OK)
		{
			_ILExecThreadHandleError(thread, result);
			return;
		}

		if ((thread->process->state & (_IL_PROCESS_STATE_UNLOADED | _IL_PROCESS_STATE_UNLOADING)) == 0)
		{
			ILExecThreadThrowOutOfMemory(thread);
		}

		return;
	}

	/* Associate the CLR thread with the engine thread */

	execThread->clrThread = _this;

	/* Start the support thread */

	if (ILThreadStart(supportThread) == 0)
	{
		/* Start unsuccessul.  Do a GC then try again. */

		ILGCCollect();
		/* Wait a resonable amount of time (1 sec) for finalizers to run */
		ILGCInvokeFinalizers(1000);

		if (ILThreadStart(supportThread) == 0)
		{
			/* Start unsuccessful.  Destroy the engine thread */
			/* The support thread will linger as long as the CLR thread does */
			ILThreadUnregisterForManagedExecution(supportThread);

			/* Throw an OutOfMemoryException */
			ILExecThreadThrowOutOfMemory(thread);
		}
	}

	result = ILMonitorExit((void **)GetObjectLockWordPtr(thread, _this));
	if(result != IL_THREAD_OK)
	{
		_ILExecThreadHandleError(thread, result);
		return;
	}
}

/*
 * public static Thread InternalCurrentThread();
 */
ILObject *_IL_Thread_InternalCurrentThread(ILExecThread *thread)
{
	return ILExecThreadCurrentClrThread();
}

/*
 * public void InternalSetBackground(bool value);
 */
void _IL_Thread_InternalSetBackground(ILExecThread *thread,
									  ILObject *_this, ILBool value)
{
	if (ILThreadGetState(((System_Thread *)_this)->privateData) == IL_TS_STOPPED)
	{
		ILExecThreadThrowSystem(thread, "System.Threading.ThreadStateException", (const char *)0);

		return;
	}

	ILThreadSetBackground(((System_Thread *)_this)->privateData, value);
}

/*
 * public ThreadPriority InternalGetPriority();
 */
ILInt32 _IL_Thread_InternalGetPriority(ILExecThread *thread, ILObject *_this)
{
	ILThread *supportThread;

	supportThread = ((System_Thread *)_this)->privateData;

	if ((ILThreadGetState(supportThread) 
			& (IL_TS_ABORTED | IL_TS_ABORT_REQUESTED | IL_TS_STOPPED)) != 0)
	{
		ILExecThreadThrowSystem(thread, "System.Threading.ThreadStateException", (const char *)0);

		return 0;
	}

	return ILThreadGetPriority(supportThread);
}

/*
 * public void InternalSetPriority(ThreadPriority priority);
 */
void _IL_Thread_InternalSetPriority(ILExecThread *thread, ILObject *_this,
									ILInt32 priority)
{
	ILThread *supportThread;
	
	supportThread = ((System_Thread *)_this)->privateData;

	if ((ILThreadGetState(supportThread)
			& (IL_TS_ABORTED | IL_TS_ABORT_REQUESTED | IL_TS_STOPPED)) != 0)
	{
		ILExecThreadThrowSystem(thread, "System.Threading.ThreadStateException", (const char *)0);

		return;
	}

	ILThreadSetPriority(supportThread, priority);
}

/*
 * public ThreadState InternalGetState();
 */
ILInt32 _IL_Thread_InternalGetState(ILExecThread *thread, ILObject *_this)
{
	return _ILExecThreadGetState(thread, ((System_Thread *)_this)->privateData);
}

/*
 * internal static bool CanStartThreads();
 */
ILBool _IL_Thread_CanStartThreads(ILExecThread *_thread)
{
	return ILHasThreads();
}

/*
 * The Thread.Volatile methods guarantee not only ordering but
 * correct correct flused values on multiprocessor systems which 
 * is why memory barriers are required on each call.  The C volatile
 * keyword is far from sufficient and most likely has no effect
 * but it can't hurt to use them anyway :D.
 *
 * -Tum
 */

/*
 * public static sbyte VolatileRead(ref sbyte address);
 */
ILInt8 _IL_Thread_VolatileRead_Rb(ILExecThread *thread, ILInt8 *address)
{
	return ILInterlockedLoadI1(address);
}

/*
 * public static byte VolatileRead(ref byte address);
 */
ILUInt8 _IL_Thread_VolatileRead_RB(ILExecThread *thread, ILUInt8 *address)
{
	return ILInterlockedLoadU1(address);
}

/*
 * public static short VolatileRead(ref short address);
 */
ILInt16 _IL_Thread_VolatileRead_Rs(ILExecThread *thread, ILInt16 *address)
{
	return ILInterlockedLoadI2(address);
}

/*
 * public static ushort VolatileRead(ref ushort address);
 */
ILUInt16 _IL_Thread_VolatileRead_RS(ILExecThread *thread, ILUInt16 *address)
{
	return ILInterlockedLoadU2(address);
}

/*
 * public static int VolatileRead(ref int address);
 */
ILInt32 _IL_Thread_VolatileRead_Ri(ILExecThread *thread, ILInt32 *address)
{
	return ILInterlockedLoadI4(address);
}

/*
 * public static uint VolatileRead(ref uint address);
 */
ILUInt32 _IL_Thread_VolatileRead_RI(ILExecThread *thread, ILUInt32 *address)
{
	return ILInterlockedLoadU4(address);
}

/*
 * public static long VolatileRead(ref long address);
 */
ILInt64 _IL_Thread_VolatileRead_Rl(ILExecThread *thread, ILInt64 *address)
{
	return ILInterlockedLoadI8(address);
}

/*
 * public static ulong VolatileRead(ref ulong address);
 */
ILUInt64 _IL_Thread_VolatileRead_RL(ILExecThread *thread, ILUInt64 *address)
{
	return ILInterlockedLoadU8(address);
}

/*
 * public static IntPtr VolatileRead(ref IntPtr address);
 */
ILNativeInt _IL_Thread_VolatileRead_Rj(ILExecThread *thread,
									   ILNativeInt *address)
{
	return (ILNativeInt)ILInterlockedLoadP((void **)address);
}

/*
 * public static UIntPtr VolatileRead(ref UIntPtr address);
 */
ILNativeUInt _IL_Thread_VolatileRead_RJ(ILExecThread *thread,
										ILNativeUInt *address)
{
	return (ILNativeUInt)ILInterlockedLoadP((void **)address);
}

/*
 * public static float VolatileRead(ref float address);
 */
ILFloat _IL_Thread_VolatileRead_Rf(ILExecThread *thread, ILFloat *address)
{
	return ILInterlockedLoadR4(address);
}

/*
 * public static double VolatileRead(ref double address);
 */
ILDouble _IL_Thread_VolatileRead_Rd(ILExecThread *thread, ILDouble *address)
{
	return ILInterlockedLoadR8(address);
}

/*
 * public static Object VolatileRead(ref Object address);
 */
ILObject *_IL_Thread_VolatileRead_RObject(ILExecThread *thread,
										  ILObject **address)
{
	return (ILObject *)ILInterlockedLoadP((void **)address);
}

/*
 * public static void VolatileWrite(ref sbyte address, sbyte value);
 */
void _IL_Thread_VolatileWrite_Rbb(ILExecThread *thread,
								  ILInt8 *address, ILInt8 value)
{
	ILInterlockedStoreI1_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref byte address, byte value);
 */
void _IL_Thread_VolatileWrite_RBB(ILExecThread *thread,
								  ILUInt8 *address, ILUInt8 value)
{
	ILInterlockedStoreU1_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref short address, short value);
 */
void _IL_Thread_VolatileWrite_Rss(ILExecThread *thread,
								  ILInt16 *address, ILInt16 value)
{
	ILInterlockedStoreI2_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref ushort address, ushort value);
 */
void _IL_Thread_VolatileWrite_RSS(ILExecThread *thread,
								  ILUInt16 *address, ILUInt16 value)
{
	ILInterlockedStoreU2_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref int address, int value);
 */
void _IL_Thread_VolatileWrite_Rii(ILExecThread *thread,
								  ILInt32 *address, ILInt32 value)
{
	ILInterlockedStoreI4_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref uint address, uint value);
 */
void _IL_Thread_VolatileWrite_RII(ILExecThread *thread,
								  ILUInt32 *address, ILUInt32 value)
{
	ILInterlockedStoreU4_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref long address, long value);
 */
void _IL_Thread_VolatileWrite_Rll(ILExecThread *thread,
								  ILInt64 *address, ILInt64 value)
{
	ILInterlockedStoreI8_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref ulong address, ulong value);
 */
void _IL_Thread_VolatileWrite_RLL(ILExecThread *thread,
								  ILUInt64 *address, ILUInt64 value)
{
	ILInterlockedStoreU8_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref IntPtr address, IntPtr value);
 */
void _IL_Thread_VolatileWrite_Rjj(ILExecThread *thread,
								  ILNativeInt *address, ILNativeInt value)
{
	ILInterlockedStoreP_Acquire((void **)address, (void *)value);
}

/*
 * public static void VolatileWrite(ref UIntPtr address, UIntPtr value);
 */
void _IL_Thread_VolatileWrite_RJJ(ILExecThread *thread,
								  ILNativeUInt *address, ILNativeUInt value)
{
	ILInterlockedStoreP_Acquire((void **)address, (void *)value);
}

/*
 * public static void VolatileWrite(ref float address, float value);
 */
void _IL_Thread_VolatileWrite_Rff(ILExecThread *thread,
								  ILFloat *address, ILFloat value)
{
	ILInterlockedStoreR4_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref double address, double value);
 */
void _IL_Thread_VolatileWrite_Rdd(ILExecThread *thread,
								  ILDouble *address, ILDouble value)
{
	ILInterlockedStoreR8_Acquire(address, value);
}

/*
 * public static void VolatileWrite(ref Object address, Object value);
 */
void _IL_Thread_VolatileWrite_RObjectObject(ILExecThread *thread,
											ILObject **address, ILObject *value)
{
	ILInterlockedStoreP_Acquire((void **)address, (void *)value);
}

/*
 * internal static int InternalGetThreadId();
 */
ILInt32 _IL_Thread_InternalGetThreadId(ILExecThread *_thread)
{
	return (ILInt32)(*((ILThread **)_thread));
}

/*
 * private static void InternalClose(IntPtr privateData);
 */
void _IL_WaitHandle_InternalClose(ILExecThread *_thread,
								  ILNativeInt privateData)
{
	if(privateData)
	{
		ILWaitHandleClose((ILWaitHandle *)privateData);
	}
}

/*
 * private static bool InternalWaitAll(WaitHandle[] waitHandles,
 *									   int timeout, bool exitContext);
 */
ILBool _IL_WaitHandle_InternalWaitAll(ILExecThread *_thread,
									  System_Array *waitHandles,
									  ILInt32 timeout,
									  ILBool exitContext)
{
	int result;
	ILWaitHandle **handles;
	
	if(waitHandles == 0)
	{
		ILExecThreadThrowArgNull(_thread, "waitHandles");

		return 0;
	}

	handles = (ILWaitHandle **)ArrayToBuffer(waitHandles);

	/* Perform the wait */
	result = ILWaitAll(handles, (ILUInt32)(ArrayLength(waitHandles)), timeout);

	if (result == IL_WAIT_TIMEOUT)
	{
		return exitContext;
	}
	else if (result < 0)
	{
		_ILExecThreadHandleWaitResult(_thread, result);

		return 0;
	}
	else
	{
		return exitContext;
	}
}

/*
 * private static int InternalWaitAny(WaitHandle[] waitHandles,
 *									  int timeout, bool exitContext);
 */
ILInt32 _IL_WaitHandle_InternalWaitAny(ILExecThread *_thread,
									   System_Array *waitHandles,
									   ILInt32 timeout, ILBool exitContext)
{
	int result;
	ILWaitHandle **handles;
	
	if(waitHandles == 0)
	{
		ILExecThreadThrowArgNull(_thread, "waitHandles");

		return 0;
	}

	handles = (ILWaitHandle **)ArrayToBuffer(waitHandles);

	/* Perform the wait */
	result = ILWaitAny(handles, (ILUInt32)(ArrayLength(waitHandles)), timeout);

	if (result == IL_WAIT_TIMEOUT)
	{
		result = IL_CLR_WAIT_TIMEOUT;
	}
	else if (result < 0)
	{
		_ILExecThreadHandleWaitResult(_thread, result);

		result = 0;
	}

	return result;
}

/*
 * private static bool InternalWaitOne(IntPtr privateData, int timeout);
 */
ILBool _IL_WaitHandle_InternalWaitOne(ILExecThread *_thread,
									  ILNativeInt privateData,
									  ILInt32 timeout)
{
	if(privateData)
	{
		int result = ILWaitOne((ILWaitHandle *)privateData, timeout);
		_ILExecThreadHandleWaitResult(_thread, result);
		return (result == 0);
	}
	return 0;
}

/*
 * private static IntPtr InternalCreateMutex(bool initiallyOwned,
 *											 String name,
 *                                           out bool gotOwnership);
 */
ILNativeInt _IL_Mutex_InternalCreateMutex(ILExecThread *_thread,
										  ILBool initiallyOwned,
										  ILString *name,
										  ILBool *gotOwnership)
{
	ILWaitHandle *handle;
	if(!name)
	{
		/* Create an ordinary mutex */
		handle = ILWaitMutexCreate(initiallyOwned);
		*gotOwnership = initiallyOwned;
	}
	else
	{
		/* Create a named mutex */
		char *nameStr = ILStringToUTF8(_thread, name);
		if(nameStr)
		{
			int gotOwn = 0;
			handle = ILWaitMutexNamedCreate(nameStr, initiallyOwned, &gotOwn);
			*gotOwnership = (ILBool)gotOwn;
		}
		else
		{
			handle = 0;
		}
	}
	if(handle)
	{
		return (ILNativeInt)handle;
	}
	else
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
}

/*
 * private static void InternalReleaseMutex(IntPtr mutex);
 */
void _IL_Mutex_InternalReleaseMutex(ILExecThread *_thread, ILNativeInt mutex)
{
	if(mutex != 0)
	{
		ILWaitMutexRelease((ILWaitHandle *)mutex);
	}
}

/*
 * Internal WaitEvent methods.
 */

/*
 * internal static extern IntPtr InternalCreateEvent(bool manualReset, bool initialState);
 */
ILNativeInt _IL_WaitEvent_InternalCreateEvent(ILExecThread *_thread, ILBool manualReset, ILBool initialState)
{
	ILWaitHandle *event;

	event = ILWaitEventCreate((int)manualReset, (int)initialState);

	if (event == 0)
	{
		ILExecThreadThrowOutOfMemory(_thread);
	}

	return (ILNativeInt)event;
}

/*
 * internal static extern bool InternalSetEvent(IntPtr handle);
 */
ILBool _IL_WaitEvent_InternalResetEvent(ILExecThread *_thread, ILNativeInt event)
{
	if( 0 == event )
	{
		ILExecThreadThrowSystem(_thread, "System.ArgumentException", 0);
	}
	
	return (ILBool)ILWaitEventReset((ILWaitHandle *)event);
}

/*
 * internal static extern bool InternalResetEvent(IntPtr handle);
 */
ILBool _IL_WaitEvent_InternalSetEvent(ILExecThread *_thread, ILNativeInt event)
{
	if( 0 == event )
	{
		ILExecThreadThrowSystem(_thread, "System.ArgumentException", 0);
	}
	
	return (ILBool)ILWaitEventSet((ILWaitHandle *)event);
}

/*
 * static void BlockingOperation.ThreadSigAbort(Thread thread);
 */
void _IL_BlockingOperation_ThreadSigAbort(ILExecThread * _thread, ILObject * thread)
{
	if( 0 != thread ) {
		ILThreadSigAbort(((System_Thread *)thread)->privateData);
	}
}

#ifdef	__cplusplus
};
#endif
