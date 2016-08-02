/*
 * hb_gc.c - Interface to the Hans-Boehm garbage collector.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 *
 * Contributions by Thong Nguyen <tum@veridicus.com>
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

#include "il_gc.h"
#include "il_thread.h"
#include "thr_defs.h"
#include "stdio.h"

#ifdef HAVE_LIBGC

#include "../libgc/include/gc.h"
#include "../libgc/include/gc_typed.h"

#ifdef	__cplusplus
extern	"C" {
#endif

struct _ILGCRunArgs
{
	void *(* threadFunc)(void *);
	void *args;
};

/*#define GC_TRACE_ENABLE*/

#define GC_TRY_INVOKE_SYNCHRONOUSLY 1

/*
 * Set to non-zero if finalization has been temporarily disabled.
 */
static int volatile _FinalizersDisabled = 0;

/*
 *	Lock used by the finalizer.
 */
static _ILMutex _FinalizerLock;

/*
 *	Tells the finalizer to stop spinning.
 * Note:  You need to signal the finalizer first.
 */
static volatile int _FinalizerStopFlag = 0;

/*
 *	Flag that is set if finalizers are running.
 */
static volatile int _FinalizersRunning = 0;

/*
 *	Number that is incremented each time finalizers are invoked.
 */
static volatile int _FinalizingCount = 0;

/*
 *	The finalizer thread.
 */
static ILThread * volatile _FinalizerThread = 0;

/*
 *	Flag that determines whether the finalizer has started or not.
 */
static volatile int _FinalizerThreadStarted = 0;

/*
 *	WaitEvent that wakes up the finalizer thread.
 */
static ILWaitHandle *_FinalizerSignal = 0;

/*
 *	WaitEvent that wakes up threads waiting on finalizers.
 */
static ILWaitHandle *_FinalizerResponse = 0;

/*
 * Set while finalizers are running synchronously.
 */
static volatile int _FinalizersRunningSynchronously = 0;

/*
 *	Tracing macros for the GC.
 */
#ifdef GC_TRACE_ENABLE
	#define GC_TRACE(a, b)		printf(a, b)
#else
	#define GC_TRACE(a, b)
#endif

/*
 * This is a internal global variable with the number of reclaimed bytes
 * after a garbage collection.
 */
extern GC_signed_word GC_bytes_found;

/*
 *	Main entry point for the finalizer thread.
 */
static void _FinalizerThreadFunc(void *data)
{
	GC_TRACE("GC:_FinalizerThread: Finalizer thread started [thread:%p]\n", _ILThreadSelf());

	for (;;)
	{
		ILWaitOne(_FinalizerSignal, -1);

		GC_TRACE("GC:_FinalizerThread: Signal [thread:%p]\n", _ILThreadSelf());

		/* This *must* to be set before checking for !_FinalizersDisabled to prevent
		    a race with ILGCDisableFinalizers */

		_FinalizersRunning = 1;

		ILThreadMemoryBarrier();

		if (GC_should_invoke_finalizers() && !_FinalizersDisabled)
		{
			GC_TRACE("GC:_FinalizerThread: Finalizers running [thread:%p]\n", _ILThreadSelf());
			
			++_FinalizingCount;

			GC_invoke_finalizers();
			
			GC_TRACE("GC:_FinalizerThread: Finalizers finished [thread:%p]\n", _ILThreadSelf());
		}

		_FinalizersRunning = 0;

		ILThreadMemoryBarrier();
		
		if (_FinalizerStopFlag)
		{
			/* Exit finalizer thread after having invoked finalizers one last time */

			GC_TRACE("GC:_FinalizerThread: Finalizer thread finished [thread:%p]\n", _ILThreadSelf());

			ILWaitEventReset(_FinalizerSignal);
			/* Wake all waiting threads */
			ILWaitEventPulse(_FinalizerResponse);
			
			return;
		}

		GC_TRACE("GC:_FinalizerThread: Response [thread:%p]\n", _ILThreadSelf());

		ILThreadClearStack(4000);

		ILWaitEventReset(_FinalizerSignal);

		/* Wake all waiting threads */
		ILWaitEventPulse(_FinalizerResponse);
	}
}

/*
 * Tries to invoke finalizers synchronously.
 * Returns 0 if successful or -1 if it would be unsafe to do so.
 */
static int _InvokeFinalizersSynchronously()
{
	unsigned long fg, bg;

	ILThreadGetCounts(&fg, &bg);

	if (!GC_should_invoke_finalizers())
	{
		return 0;
	}
	
	/* Prevent recursive finalization */
	if (_FinalizersRunningSynchronously)
	{
		return 0;
	}

	if (fg + bg > 1)
	{
		/* Threads are supported and there are other threads active so it isn't
		   safe to invoke finalizers synchronously. */

		return -1;
	}
	else
	{
		/* Because there is only one thread active (this thread), it is safe to
		   invoke finalizers synchronously */

		_FinalizersRunning = 1;
		_FinalizersRunningSynchronously = 1;

		++_FinalizingCount;

		GC_invoke_finalizers();

		_FinalizersRunning = 0;
		_FinalizersRunningSynchronously = 0;

		return 0;
	}
}

/*
 * Notify the finalization thread that there is work to do.
 */
static int PrivateGCNotifyFinalize(int timeout, int ignoreDisabled)
{
	int result;
	
	if (_FinalizersDisabled && !ignoreDisabled)
	{
		return 0;
	}

	/* Prevent recursive finalization */
	if (_FinalizersRunningSynchronously || _ILThreadSelf() == _FinalizerThread)
	{	
		return 0;
	}

#ifdef GC_TRY_INVOKE_SYNCHRONOUSLY

	/* Try to invoke synchronously (for performance & single threaded systems) */
	if (_InvokeFinalizersSynchronously() == 0)
	{
		return 0;
	}
	
#endif

	/* There is no finalizer thread!
	   We've already attempted to invoke synchronously (above) so just exit. */
	if (_FinalizerThread == 0)
	{
		return 0;
	}
	
	/* Finalizers need to be run on a seperate thread.
	   Start the finalizer thread if it hasn't been started */
	if (!_FinalizerThreadStarted)
	{
		_ILMutexLock(&_FinalizerLock);
		
		if (!_FinalizerThreadStarted)
		{
			if (ILThreadStart(_FinalizerThread) == 0)
			{
				/* Couldn't create the finalizer thread */

				GC_TRACE("PrivateGCInvokeFinalizers: Couldn't " \
						 "start finalizer thread [thread: %p]\n",
						 _ILThreadSelf());
				
				_ILMutexUnlock(&_FinalizerLock);

				return 0;
			}

			_FinalizerThreadStarted = 1;
		}

		_ILMutexUnlock(&_FinalizerLock);
	}

	/* Signal the finalizer thread */

	GC_TRACE("PrivateGCInvokeFinalizers: Invoking finalizers " \
			"and waiting [thread: %p]\n", _ILThreadSelf());

	result = ILSignalAndWait(_FinalizerSignal, _FinalizerResponse, timeout);
	
	GC_TRACE("PrivateGCInvokeFinalizers: Finalizers finished[thread: %p]\n", _ILThreadSelf());

	return result;
}

/*
 *	Called by the GC when it needs to run finalizers.
 */
static void GCNotifyFinalize(void)
{
	/*
	 * This is called by an allocating thread.  We pass in a timeout
	 * value of 0 because the allocating thread should not be blocked
	 * by finalizers otherwise finalizers may deadlock waiting for
	 * any locks that the (blocked) allocating thread might own.
	 */
	PrivateGCNotifyFinalize(0, 0);
}

void ILGCInit(unsigned long maxSize)
{
	GC_INIT();		/* For shared library initialization on sparc */	
	GC_set_max_heap_size((size_t)maxSize);
	
	/* Set up the finalization system the way we want it */
	GC_no_dls = 1;
	GC_finalize_on_demand = 1;
	GC_java_finalization = 1;
	GC_finalizer_notifier = GCNotifyFinalize;
	_FinalizersDisabled = 0;

	_ILMutexCreate(&_FinalizerLock);

	/* Create the finalizer thread */
	_FinalizerStopFlag = 0;	
	_FinalizerThread = ILThreadCreate(_FinalizerThreadFunc, 0);
	
	if (_FinalizerThread)
	{
		_FinalizerSignal = ILWaitEventCreate(1, 0);
		_FinalizerResponse = ILWaitEventCreate(1, 0);

		/* Make the finalizer thread a background thread */
		ILThreadSetBackground(_FinalizerThread, 1);

		/* To speed up simple command line apps, the finalizer thread doesn't start
		    until it is first needed */
	}
}

void ILGCDeinit()
{
	_FinalizerStopFlag = 1;

	GC_TRACE("ILGCDeinit: Performing final GC [thread:%p]\n", _ILThreadSelf());

	/* Do a final GC */
	ILGCFullCollection(1000);

	/* Cleanup the finalizer thread */
	if (_FinalizerThread && _FinalizerThreadStarted)
	{
		GC_TRACE("ILGCDeinit: Peforming last finalizer run [thread:%p]\n", _ILThreadSelf());

		ILWaitEventSet(_FinalizerSignal);
		
		GC_TRACE("ILGCDeinit: Waiting for finalizer thread to end [thread:%p]\n", _ILThreadSelf());

		/* Wait for the finalizer thread */
		if (ILThreadJoin(_FinalizerThread, 15000))
		{
			GC_TRACE("ILGCDeinit: Finalizer thread finished [thread:%p]\n", _ILThreadSelf());
		}
		else
		{
			GC_TRACE("ILGCDeinit: Finalizer thread not responding [thread:%p]\n", _ILThreadSelf());
		}

		/* Destroy the finalizer thread */
		ILThreadDestroy(_FinalizerThread);			
	}

	_ILMutexDestroy(&_FinalizerLock);
}

void *ILGCAlloc(unsigned long size)
{
	/* The Hans-Boehm routines guarantee to zero the block */
	return GC_MALLOC((size_t)size);
}

void *ILGCAllocAtomic(unsigned long size)
{
	void *block = GC_MALLOC_ATOMIC((size_t)size);
	if(block)
	{
		/* The Hans-Boehm routines don't guarantee to zero the block */
		ILMemZero(block, size);
	}
	return block;
}

void *ILGCAllocPersistent(unsigned long size)
{
	/* The Hans-Boehm routines guarantee to zero the block */
	return GC_MALLOC_UNCOLLECTABLE((size_t)size);
}

ILNativeInt ILGCCreateTypeDescriptor(ILNativeUInt bitmap[], ILNativeUInt len)
{
	return (ILNativeInt)GC_make_descriptor((GC_bitmap)bitmap, (size_t)len);
}

void *ILGCAllocExplicitlyTyped(unsigned long size, ILNativeInt descriptor)
{
	return GC_malloc_explicitly_typed(size, (GC_descr)descriptor);
}

void ILGCFreePersistent(void *block)
{
	if(block)
	{
		GC_FREE(block);
	}
}

void ILGCRegisterFinalizer(void *block, ILGCFinalizer func, void *data)
{
	/* We use the Java-style finalization algorithm, which
	   ignores cycles in the object structure */
	GC_REGISTER_FINALIZER_NO_ORDER(block, func, data, 0, 0);	
}

void ILGCMarkNoPointers(void *start, unsigned long size)
{
	GC_exclude_static_roots(start, (void *)(((unsigned char *)start) + size));
}

void ILGCCollect(void)
{
	GC_gcollect();
}

int ILGCFullCollection(int timeout)
{
	int lastFinalizingCount;
	int hasThreads;

	hasThreads = _ILHasThreads();

	if (timeout < 0)
	{
		ILInt32 bytesCollected;

		/* No timeout */
		if (_FinalizersRunning && hasThreads)
		{
			/* Wait for the finalizers to finish */
			if(PrivateGCNotifyFinalize(timeout, 1))
			{
				/* timeout expired */
				/* Then do at least one collection */
				GC_gcollect();

				return 0;
			}
		}

		ILThreadClearStack(4000);

		do
		{
			lastFinalizingCount = _FinalizingCount;

			GC_TRACE("Last finalizingCount = %i\n", lastFinalizingCount);

			GC_gcollect();
			bytesCollected = GC_bytes_found;

			GC_TRACE("GC: bytes collected =  %i\n", bytesCollected);

			if(!PrivateGCNotifyFinalize(-1, 0))
			{
				/* something went wrong or finalizers are disabled */

				return 0;
			}
		} while ((lastFinalizingCount < _FinalizingCount) || 
				 (bytesCollected != 0));
	}
	else
	{
		/* With timeout */
		ILCurrTime startTime;
		ILUInt64 startMs;
		int remainingTimeout;
		ILInt32 bytesCollected;

		if(!ILGetSinceRebootTime(&startTime))
		{
			return 0;
		}
		startMs = (startTime.secs * 1000) +
				  (ILUInt64)(startTime.nsecs / 1000000);
		remainingTimeout = timeout;

		if (_FinalizersRunning && hasThreads)
		{
			/* Wait for the finalizers to finish */
			if(PrivateGCNotifyFinalize(timeout, 1))
			{
				/* timeout expired */
				/* Then do at least one collection */
				GC_gcollect();

				return 0;
			}
		}

		ILThreadClearStack(4000);

		do
		{
			ILCurrTime currTime;
			ILUInt64 currMs;

			lastFinalizingCount = _FinalizingCount;

			GC_TRACE("Last finalizingCount = %i\n", lastFinalizingCount);

			GC_gcollect();
			bytesCollected = GC_bytes_found;

			GC_TRACE("GC: bytes collected =  %i\n", bytesCollected);

			if(!ILGetSinceRebootTime(&currTime))
			{
				return 0;
			}
			currMs = (currTime.secs * 1000) +
					 (ILUInt64)(currTime.nsecs / 1000000);
			remainingTimeout = timeout - (int)(currMs - startMs);

			if(remainingTimeout <= 0)
			{
				return 0;
			}
			if(PrivateGCNotifyFinalize(remainingTimeout, 1))
			{
				/* timeout expired */
				return 0;
			}

		} while ((lastFinalizingCount < _FinalizingCount) ||
				 (bytesCollected != 0));
	}

	return 1;
}

int ILGCCollectionCount(void)
{
	return GC_gc_no;
}

int ILGCInvokeFinalizers(int timeout)
{
	int retval = 1;
	
	if (GC_should_invoke_finalizers())
	{
		retval = (PrivateGCNotifyFinalize(timeout, 0) ? 0 : 1);
	}
	
	return retval;
}

int ILGCDisableFinalizers(int timeout)
{
	_ILMutexLock(&_FinalizerLock);

	if (_FinalizersDisabled)
	{
		_ILMutexUnlock(&_FinalizerLock);

		return 1;
	}

	_FinalizersDisabled = 1;

	_ILMutexUnlock(&_FinalizerLock);

	if (_FinalizersRunning && _ILHasThreads())
	{
		/* Invoke and wait so we can guarantee no finalizers will run after this method ends */
		return (PrivateGCNotifyFinalize(timeout, 1) ? 0 : 1);
	}
	
	return 1;
}

void ILGCEnableFinalizers(void)
{
	_FinalizersDisabled = 0;

	ILThreadMemoryBarrier();
}

long ILGCGetHeapSize(void)
{
	return (long)GC_get_heap_size();
}

void ILGCRegisterWeak(void *ptr)
{
	GC_register_disappearing_link(ptr);
}

void ILGCUnregisterWeak(void *ptr)
{
	GC_unregister_disappearing_link(ptr);
}

void ILGCRegisterGeneralWeak(void *ptr, void *obj)
{
	GC_general_register_disappearing_link(ptr, obj);
}

static void *RunFunc(struct GC_stack_base *stackBase, void *args)
{
	struct _ILGCRunArgs *runArgs = (struct _ILGCRunArgs *)args;

	return (runArgs->threadFunc)(runArgs->args);
}

void *ILGCRunFunc(void *(* threadFunc)(void *), void *args)
{
	struct _ILGCRunArgs runArgs;

	runArgs.threadFunc = threadFunc;
	runArgs.args = args;

	return GC_call_with_stack_base(RunFunc, (void *)&runArgs);
}

#ifdef	__cplusplus
};
#endif

#endif /* HAVE_LIBGC */
