/*
 * thread.c - Manage threads within the runtime engine.
 *
 * Copyright (C) 2001, 2011  Southern Storm Software, Pty Ltd.
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

#include "engine_private.h"
#include "lib_defs.h"
#include "thr_defs.h"
#include "interrupt.h"
#include "interlocked.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Gets the currently ILExecThread.
 */
ILExecThread *ILExecThreadCurrent()
{
	ILThread *thread = ILThreadSelf();

	if(thread)
	{
		return _ILExecThreadFromThread(thread);
	}
	else
	{
		return 0;
	}
}

/*
 * Gets the managed thread object from an engine thread.
 */
ILObject *ILExecThreadCurrentClrThread()
{
	ILMethod* method;
	ILClass *classInfo;
	ILExecValue result, arg;
	System_Thread *clrThread;
	ILExecThread *thread = ILExecThreadCurrent();

	if (thread == 0)
	{
		return 0;
	}

	if (thread->clrThread == 0)
	{
		/* Main thread or another thread created from inside the engine. */
	
		/* Get the CLR thread class */

		classInfo = ILExecThreadLookupClass(thread, "System.Threading.Thread");

		/* Allocate a new CLR thread object */

		clrThread = (System_Thread *)_ILEngineAllocObject(thread, classInfo);

		/* Associate the executing thread with the CLR thread */
		thread->clrThread = (ILObject *)clrThread;
		
		/* Execute the private constructor */

		method = ILExecThreadLookupMethod(thread, "System.Threading.Thread", ".ctor", "(Tj)V");

		if (method == 0)
		{
			ILExecThreadThrowSystem(thread, "System.NotImplementedException", "System.Threading.Thread..ctor()");

			return 0;
		}
		
		/* Pass the OS thread as an argument to the CLR thread's constructor */
		arg.ptrValue = ILThreadSelf();

		_ILCallMethod
		(
			thread,
			method,
			_ILCallUnpackDirectResult,
			&result,
			0,
			clrThread,
			_ILCallPackVParams,
			&arg
		);
	}

	return thread->clrThread;
}


#if defined(IL_USE_INTERRUPT_BASED_X)

static void _ILInterruptHandler(ILInterruptContext *context)
{
	ILExecThread *execThread;

	execThread = ILExecThreadCurrent();
	
	if (execThread == 0)
	{
		return;
	}

	if (execThread->runningManagedCode)
	{
		switch (context->type)
		{
		#if defined(IL_USE_INTERRUPT_BASED_NULL_POINTER_CHECKS)
			case IL_INTERRUPT_TYPE_ILLEGAL_MEMORY_ACCESS:
				execThread->interruptContext = *context;
				IL_LONGJMP(execThread->exceptionJumpBuffer, _IL_INTERRUPT_NULL_POINTER);
		#endif

		#if defined(IL_INTERRUPT_SUPPORTS_INT_DIVIDE_BY_ZERO)
			case IL_INTERRUPT_TYPE_INT_DIVIDE_BY_ZERO:
				execThread->interruptContext = *context;
				IL_LONGJMP(execThread->exceptionJumpBuffer, _IL_INTERRUPT_INT_DIVIDE_BY_ZERO);
		#endif

		#if defined(IL_USE_INTERRUPT_BASED_INT_OVERFLOW_CHECKS)
			case IL_INTERRUPT_TYPE_INT_OVERFLOW:
				execThread->interruptContext = *context;
				IL_LONGJMP(execThread->exceptionJumpBuffer, _IL_INTERRUPT_INT_OVERFLOW);
		#endif
		}
	}
}

#endif

void _ILThreadSetExecContext(ILThread *thread, ILThreadExecContext *context, ILThreadExecContext *saveContext)
{
	if (saveContext)
	{
		_ILThreadSaveExecContext(thread, saveContext);
	}

	ILThreadSetObject(thread, context->execThread);

	#if defined(IL_USE_INTERRUPT_BASED_X)
		if (context->execThread)
		{
			ILThreadRegisterInterruptHandler(thread, _ILInterruptHandler);
		}
	#endif

#ifdef IL_USE_JIT
	/* Set the exception handler which converts builtin
	   libjit exceptions into clr exceptions */
	jit_exception_set_handler(_ILJitExceptionHandler);
#endif

	context->execThread->supportThread = thread;
}

void _ILThreadSaveExecContext(ILThread *thread, ILThreadExecContext *saveContext)
{
	saveContext->execThread = ILThreadGetObject(thread);
}

void _ILThreadRestoreExecContext(ILThread *thread, ILThreadExecContext *context)
{
	ILThreadSetObject(thread, context->execThread);

	if (context->execThread)
	{
		context->execThread->supportThread = thread;

		#if defined(IL_USE_INTERRUPT_BASED_X)
			ILThreadRegisterInterruptHandler(thread, _ILInterruptHandler);
		#endif
	}
	else
	{
		#if defined(IL_USE_INTERRUPT_BASED_X)
			ILThreadUnregisterInterruptHandler(thread, _ILInterruptHandler);
		#endif
	}
}

void _ILThreadClearExecContext(ILThread *thread)
{
	ILExecThread *prev;

	prev = ILThreadGetObject(thread);

	if (prev)
	{
		prev->supportThread = 0;
	}

	#if defined(IL_USE_INTERRUPT_BASED_X)
		ILThreadUnregisterInterruptHandler(thread, _ILInterruptHandler);
	#endif

	ILThreadSetObject(thread, 0);
}

/*
 *	Cleanup handler for threads that have been registered for managed execution.
 */
static void ILExecThreadCleanup(ILThread *thread)
{
	ILThreadUnregisterForManagedExecution(thread);
}

/*
 * Registers a thread for managed execution
 */
ILExecThread *ILThreadRegisterForManagedExecution(ILExecProcess *process, ILThread *thread)
{
	ILExecThread *execThread;
	ILThreadExecContext context;

	if(process && (process->state >= _IL_PROCESS_STATE_UNLOADING))
	{
		return 0;
	}

	/* If the thread has already been registerered then return the existing engine thread */
	if ((execThread = ILThreadGetObject(thread)) != 0)
	{
		return execThread;
	}

	/* Create a new engine-level thread */
	if ((execThread = _ILExecThreadCreate(process, 0)) == 0)
	{
		return 0;
	}

	/* TODO: Notify the GC that we possibly have a new thread to be scanned */

	context.execThread = execThread;

	_ILThreadSetExecContext(thread, &context, 0);

	/* Register a cleanup handler for the thread */
	ILThreadRegisterCleanup(thread, ILExecThreadCleanup);

	return execThread;
}

/*
 *	Unregisters a thread for managed execution.
 */
void ILThreadUnregisterForManagedExecution(ILThread *thread)
{
	ILWaitHandle *monitor;
	ILExecThread *execThread;

	/* Get the engine thread from the support thread */
	execThread = ILThreadGetObject(thread);

	if (execThread == 0)
	{
		/* Already unregistered */
		return;
	}

	/* Unregister the cleanup handler */
	ILThreadUnregisterCleanup(thread, ILExecThreadCleanup);

	monitor = ILThreadGetMonitor(thread);

	ILWaitMonitorEnter(monitor);
	{
		/* Disassociate the engine thread with the support thread */
		_ILThreadClearExecContext(thread);

		/* Destroy the engine thread */
		_ILExecThreadDestroy(execThread);
	}
	ILWaitMonitorLeave(monitor);
}

/*
 * Called by the current thread when it was to begin its abort sequence.
 * Returns 0 if the thread has successfully self aborted.
 */
int _ILExecThreadSelfAborting(ILExecThread *thread)
{
	ILObject *exception;

	/* Determine if we currently have an abort in progress,
	   or if we need to throw a new abort exception */
	if(!ILThreadIsAborting())
	{
		if(ILThreadSelfAborting())
		{
			/* Allocate an instance of "ThreadAbortException" and throw */
			
			exception = _ILExecThreadNewThreadAbortException(thread, 0);

			ILInterlockedAndU4(&(thread->managedSafePointFlags),
							   ~_IL_MANAGED_SAFEPOINT_THREAD_ABORT);
			thread->aborting = 1;
			_ILExecThreadSetException(thread, exception);

			return 0;
		}
	}

	return -1;
}

ILInt32 _ILExecThreadGetState(ILExecThread *thread, ILThread* supportThread)
{
	return ILThreadGetState(supportThread);
}

void _ILExecThreadResumeThread(ILExecThread *thread, ILThread *supportThread)
{
	ILWaitHandle *monitor;
	ILExecThread *execThread;

	monitor = ILThreadGetMonitor(supportThread);

	/* Locking the monitor prevents the associated engine thread from being destroyed */
	ILWaitMonitorEnter(monitor);

	execThread = _ILExecThreadFromThread(supportThread);

	if (execThread == 0)
	{
		/* The thread has finished running */

		ILWaitMonitorLeave(monitor);

		ILExecThreadThrowSystem
			(
				thread,
				"System.Threading.ThreadStateException",
				(const char *)0
			);

		return;
	}

	/* Remove the _IL_MANAGED_SAFEPOINT_THREAD_SUSPEND flag */
	ILInterlockedAndU4(&(execThread->managedSafePointFlags),
					   ~_IL_MANAGED_SAFEPOINT_THREAD_SUSPEND);

	/* Call the threading subsystem's resume */
	ILThreadResume(supportThread);

	ILWaitMonitorLeave(monitor);
}

/*
 * Suspend the given thread.
 * If the thread is a in wait/sleep/join state or the thread is executing non-managed code
 * then a suspend request will be made and the method will exit immediately.  The suspend
 * request will be processed either by the thread subsystem (when the thread next enters
 * and exits a wait/sleep/join state) or by the engine (when the thread next enters a 
 * managed safepoint).
 */
void _ILExecThreadSuspendThread(ILExecThread *thread, ILThread *supportThread)
{
	int result;
	ILWaitHandle *monitor;
	ILExecThread *execThread;

	monitor = ILThreadGetMonitor(supportThread);

	/* If the thread being suspended is the current thread then suspend now */
	if (thread->supportThread == supportThread)
	{
		/* Suspend the current thread */
		result = ILThreadSuspend(supportThread);

		if (result == 0)
		{
			ILExecThreadThrowSystem
				(
					thread,
					"System.Threading.ThreadStateException",
					(const char *)0
				);

			return;
		}
		else if (result < 0)
		{
			if (_ILExecThreadHandleWaitResult(thread, result) != 0)
			{
				return;
			}
		}

		return;
	}

	/* Entering the monitor keeps the execThread from being destroyed */
	ILWaitMonitorEnter(monitor);

	execThread = _ILExecThreadFromThread(supportThread);

	if (execThread == 0)
	{
		/* The thread is dead */
		result = IL_SUSPEND_FAILED;
	}
	else
	{
		result = ILThreadSuspendRequest(supportThread, !execThread->runningManagedCode);
	}

	if (result == IL_SUSPEND_FAILED)
	{
		ILExecThreadThrowSystem
			(
				thread,
				"System.Threading.ThreadStateException",
				(const char *)0
			);

		ILWaitMonitorLeave(monitor);

		return;
	}
	else if (result == IL_SUSPEND_REQUESTED)
	{		
		/* In order to prevent a suspend_request from being processed twice (once by
		    the threading subsystem and twice by the engine when it detects the safepoint
		    flags, the engine needs to check the ThreadState after checking the safepoint
		    flags (see cvm_call.c) */

		ILInterlockedOrU4(&(execThread->managedSafePointFlags),
						  _IL_MANAGED_SAFEPOINT_THREAD_SUSPEND);

		ILWaitMonitorLeave(monitor);

		return;
	}
	else if (result < 0)
	{
		if (_ILExecThreadHandleWaitResult(thread, result) != 0)
		{
			ILWaitMonitorLeave(monitor);

			return;
		}
	}
}

/*
 * Abort the given thread.
 */
void _ILExecThreadAbortThread(ILExecThread *thread, ILThread *supportThread)
{
	ILWaitHandle *monitor;
	ILExecThread *execThread;

	monitor = ILThreadGetMonitor(supportThread);

	/* Prevent the ILExecThread from being destroyed while
	   we are accessing it */
	ILWaitMonitorEnter(monitor);

	execThread = _ILExecThreadFromThread(supportThread);

	if (execThread == 0)
	{	
		/* The thread has already finished */
		ILWaitMonitorLeave(monitor);

		return;
	}
	else
	{
		/* Mark the flag so the thread self aborts when it next returns
		   to managed code */
		ILInterlockedOrU4(&(execThread->managedSafePointFlags),
						  _IL_MANAGED_SAFEPOINT_THREAD_ABORT);
	}

	ILWaitMonitorLeave(monitor);

	/* Abort the thread if its in or when it next enters a wait/sleep/join
	   state */
	ILThreadAbort(supportThread);

	/* If the current thread is aborting itself then abort immediately.
	   We have to check the thread here because the thread can be aborted
	   from an unmanaged thread. Then thread will be 0. */
	if (thread && (supportThread == thread->supportThread))
	{
		_ILExecThreadSelfAborting(execThread);		
	}
}

/*
 * Handle the result from an "ILWait*" function call.
 */
int _ILExecThreadHandleWaitResult(ILExecThread *thread, int result)
{
	switch (result)
	{
	case IL_WAIT_INTERRUPTED:
		{
			ILExecThreadThrowSystem
				(
				thread,
				"System.Threading.ThreadInterruptedException",
				(const char *)0
				);
		}
		break;
	case IL_WAIT_ABORTED:
		{
			_ILExecThreadSelfAborting(thread);
		}
		break;
	case IL_WAIT_FAILED:
		{
			ILExecThreadThrowSystem
				(
					thread,
					"System.Threading.SystemException",
					(const char *)0
				);
		}
	}

	return result;
}

/*
 * Throw the exception for the thread error code.
 */
void _ILExecThreadHandleError(ILExecThread *thread, int error)
{
	switch(error)
	{
		case IL_THREAD_ERR_SYNCLOCK:
		{
			ILExecThreadThrowSystem(thread,
									"System.Threading.SynchronizationLockException",
									(const char *)0);
		}
		break;

		case IL_THREAD_ERR_INTERRUPT:
		{
			ILExecThreadThrowSystem(thread,
									"System.Threading.ThreadInterruptedException",
									(const char *)0);
		}
		break;

		case IL_THREAD_ERR_ABORTED:
		{
			_ILExecThreadSelfAborting(thread);
		}
		break;

		case IL_THREAD_ERR_OUTOFMEMORY:
		{
			
		}
		break;
		
		case IL_THREAD_ERR_UNKNOWN:
		{
			ILExecThreadThrowSystem(thread,
									"System.SystemException",
									(const char *)0);
		}
		break;
	}
}

/*
 * Join an ILExecProcess for normal execution.
 * The caller has to handle the thread safety and to make sure
 * that the process is not unloaded.
 */
static IL_INLINE void ILExecThreadJoinProcess(ILExecThread *thread, ILExecProcess *process)
{
	_ILExecThreadProcess(thread) = process;
	thread->nextThread = process->firstThread;
	thread->prevThread = 0;
	if(process->firstThread)
	{
		process->firstThread->prevThread = thread;
	}
	process->firstThread = thread;

}

/*
 * Detach from the ILExecProcess.
 * The caller has to handle the thread safety.
 */
static IL_INLINE void ILExecThreadDetachFromProcess(ILExecThread *thread)
{
	if(thread->nextThread)
	{
		thread->nextThread->prevThread = thread->prevThread;
	}
	if(thread->prevThread)
	{
		thread->prevThread->nextThread = thread->nextThread;
	}
	else
	{
		_ILExecThreadProcess(thread)->firstThread = thread->nextThread;
	}
	_ILExecThreadProcess(thread) = 0;
	thread->nextThread = thread->prevThread = 0;
}

ILExecThread *_ILExecThreadCreate(ILExecProcess *process, int ignoreProcessState)
{
	ILExecThread *thread;

	/* Check the process state early the first time */
	if (!ignoreProcessState && process &&
		(process->state >= _IL_PROCESS_STATE_UNLOADING))
	{
		return 0;
	}

	/* Create a new thread block */
	if((thread = (ILExecThread *)ILGCAllocPersistent
									(sizeof(ILExecThread))) == 0)
	{
		return 0;
	}

	/* Initialize the thread state */
	thread->supportThread = 0;
	thread->clrThread = 0;
	thread->aborting = 0;
	thread->freeMonitor = 0;
	thread->freeMonitorCount = 0;
	thread->isFinalizerThread = 0;
	thread->method = 0;
	thread->thrownException = 0;
	thread->threadStaticSlots = 0;
	thread->threadStaticSlotsUsed = 0;
	thread->managedSafePointFlags = 0;
	thread->runningManagedCode = 0;
	thread->process = 0;

#ifdef IL_CONFIG_APPDOMAINS
	thread->prevContext = 0;
#endif

#ifdef IL_USE_CVM
	thread->numFrames = 0;
	thread->maxFrames = 0;
	thread->pc = 0;
	thread->frame = 0;
	thread->stackTop = 0;
#endif

#ifdef IL_DEBUGGER
	thread->numWatches = 0;
	thread->maxWatches = 0;
#endif

	if(process)
	{
		/* Lock down the process */
		ILMutexLock(process->lock);

		if (!ignoreProcessState &&
			(process->state >= _IL_PROCESS_STATE_UNLOADING))
		{
			ILMutexUnlock(process->lock);
			ILGCFreePersistent(thread);

			return 0;
		}

#ifdef IL_USE_CVM
		/* Allocate space for the thread-specific value stack */
		if((thread->stackBase = (CVMWord *)ILGCAllocPersistent
						(sizeof(CVMWord) * process->stackSize)) == 0)
		{
			ILMutexUnlock(process->lock);
			ILGCFreePersistent(thread);
			return 0;
		}
		thread->stackLimit = thread->stackBase + process->stackSize;

		/* Allocate space for the initial frame stack */
		if((thread->frameStack = (ILCallFrame *)ILGCAllocPersistent
					(sizeof(ILCallFrame) * process->frameStackSize)) == 0)
		{
			ILMutexUnlock(process->lock);
			ILGCFreePersistent(thread->stackBase);
			ILGCFreePersistent(thread);
			return 0;
		}

		thread->maxFrames = process->frameStackSize;
		thread->frame = thread->stackBase;
		thread->stackTop = thread->stackBase;
#endif /* IL_USE_CVM */

		/* Attach the thread to the process */
		ILExecThreadJoinProcess(thread, process);

		ILMutexUnlock(process->lock);
	}

	/* Return the thread block to the caller */
	return thread;
}

void _ILExecThreadDestroy(ILExecThread *thread)
{
	ILExecProcess *process = _ILExecThreadProcess(thread);

	if(process)
	{
		/* Lock down the process */
		ILMutexLock(process->lock);

		/* If this is the finalizer thread then clear process->finalizerThread */
		if (process->finalizerThread == thread)
		{
			process->finalizerThread = 0;
		}

		/* Detach the thread from its process */
		ILExecThreadDetachFromProcess(thread);

		/* Unlock the process */
		ILMutexUnlock(process->lock);
	}
	
	/* Remove associations between ILExecThread and ILThread if they
	   haven't already been removed */
	if (thread->supportThread)
	{
		_ILThreadClearExecContext(thread->supportThread);
	}

#ifdef IL_USE_CVM
	/* Destroy the operand stack */
	if(thread->stackBase)
	{
		ILGCFreePersistent(thread->stackBase);
	}

	/* Destroy the call frame stack */
	if(thread->frameStack)
	{
		ILGCFreePersistent(thread->frameStack);
	}
#endif

#ifdef IL_DEBUGGER
	/* Destroy the watch stack */
	if(thread->watchStack)
	{
		ILGCFreePersistent(thread->watchStack);
	}
#endif

	/* Destroy the thread block */
	ILGCFreePersistent(thread);
}

/* Clear the thread data needed only during execution */
void _ILExecThreadClearExecutionState(ILExecThread *thread)
{
	/*
	 * Clear the clrThread so that it can be collected if it's no longer
	 * referenced from managed code.
	 */
	thread->clrThread = 0;
	
#ifdef IL_USE_CVM
	/*
	 * Clear the members in the thread that are no longer needed after
	 * execution finished.
	 */
	/* Destroy the operand stack */
	if(thread->stackBase)
	{
		ILGCFreePersistent(thread->stackBase);
		thread->stackBase = 0;
	}
	/* Destroy the call frame stack */
	if(thread->frameStack)
	{
		ILGCFreePersistent(thread->frameStack);
		thread->frameStack = 0;
	}
	thread->frame = 0;
	thread->stackTop = 0;
#endif
}

ILExecProcess *ILExecThreadGetProcess(ILExecThread *thread)
{
	return _ILExecThreadProcess(thread);
}

ILCallFrame *_ILGetCallFrame(ILExecThread *thread, ILInt32 n)
{
#ifdef IL_USE_CVM
	ILCallFrame *frame;
	ILUInt32 posn;
	if(n < 0)
	{
		return 0;
	}
	posn = thread->numFrames;
	while(posn > 0)
	{
		--posn;
		frame = &(thread->frameStack[posn]);
		if(!n)
		{
			return frame;
		}
		--n;
	}
#endif
	return 0;
}

ILCallFrame *_ILGetNextCallFrame(ILExecThread *thread, ILCallFrame *frame)
{
#ifdef IL_USE_CVM
	ILUInt32 posn;
	posn = frame - thread->frameStack;
	if(posn > 0)
	{
		--posn;
		return &(thread->frameStack[posn]);
	}
	else
	{
		return 0;
	}
#else
	return 0;
#endif
}

#ifdef IL_DEBUGGER

ILLocalWatch *_ILAllocLocalWatch(ILExecThread *thread)
{
	ILUInt32 newsize;
	ILLocalWatch *watches;

	/* Calculate watch stack size */
	if(thread->maxWatches)
	{
		newsize = thread->maxWatches * 2;
	}
	else
	{
		newsize = 32;		/* initial value */
	}

	/* Allocate new watches */
	if((watches = (ILLocalWatch *)ILGCAllocPersistent
				(sizeof(ILLocalWatch) * newsize)) == 0)
	{
		_ILExecThreadSetOutOfMemoryException(thread);
		return 0;
	}

	/* Copy the old contents (if any) */
	if(thread->maxWatches)
	{
		ILMemCpy(watches, thread->watchStack,
				sizeof(ILLocalWatch) * thread->maxWatches);

		/* Free the old watches */
		ILGCFreePersistent(thread->watchStack);
	}
	thread->watchStack = watches;
	thread->maxWatches = newsize;

	/* Return the new watch to the caller */
	return &(thread->watchStack[(thread->numWatches)]);
}

#endif	// IL_DEBUGGER

ILMethod *ILExecThreadStackMethod(ILExecThread *thread, unsigned long num)
{
	ILCallFrame *frame;
	if(!num)
	{
		return thread->method;
	}
	frame = _ILGetCallFrame(thread, (ILInt32)(num - 1));
	if(frame)
	{
		return frame->method;
	}
	else
	{
		return 0;
	}
}

#ifdef IL_CONFIG_APPDOMAINS
/* Copy the process dependant data to the ILExecContext */
static IL_INLINE void ILExecThreadSaveExecContext(ILExecThread *thread, ILExecContext *context)
{
	context->prevContext = thread->prevContext;
	context->clrThread = thread->clrThread;
	context->process = _ILExecThreadProcess(thread);
	context->threadStaticSlots = thread->threadStaticSlots;
	context->threadStaticSlotsUsed = thread->threadStaticSlotsUsed;
}

/* Clean up an ILExecContext */
static IL_INLINE void ILExecThreadClearExecContext(ILExecContext *context)
{
	context->prevContext = 0;
	context->process = 0;
	context->clrThread = 0;
	context->threadStaticSlots = 0;
	context->threadStaticSlotsUsed = 0;
}

/* Copy the process dependant data to the ILExecContext */
static IL_INLINE void ILExecThreadRestoreExecContext(ILExecThread *thread, ILExecContext *context)
{
	thread->prevContext = context->prevContext;
	thread->clrThread = context->clrThread;
	_ILExecThreadProcess(thread) = context->process;
	thread->threadStaticSlots = context->threadStaticSlots;
	thread->threadStaticSlotsUsed = context->threadStaticSlotsUsed;
}

/*
 * Let the thread return from an other ILExecProcess and restore the saved state.
 * Returns 0 on failure.
 */
int ILExecThreadReturnToProcess(ILExecThread *thread, ILExecContext *context)
{
	if(context->process != _ILExecThreadProcess(thread))
	{
		int result = 1;
		ILExecProcess *returnProcess = context->process;
		ILExecProcess *leavingProcess = _ILExecThreadProcess(thread);
		/* perform a real return */

		/* do this so that the locks are always in the same order to avoid a deadlock */
		if(returnProcess < leavingProcess)
		{
			if(returnProcess)
			{
				ILMutexLock(returnProcess->lock);
			}
			if(leavingProcess)
			{
				ILMutexLock(leavingProcess->lock);
			}
		}
		else
		{
			if(leavingProcess)
			{
				ILMutexLock(leavingProcess->lock);
			}
			if(returnProcess)
			{
				ILMutexLock(returnProcess->lock);
			}
		}
		if(leavingProcess)
		{
			ILExecThreadDetachFromProcess(thread);
		}
		/* restore the previous state */
		thread->prevContext = context->prevContext;
		thread->clrThread = context->clrThread;
		_ILExecThreadProcess(thread) = context->process;
		thread->threadStaticSlots = context->threadStaticSlots;
		thread->threadStaticSlotsUsed = context->threadStaticSlotsUsed;
		/* check if the new process can't be entered */
		if(returnProcess->state >= _IL_PROCESS_STATE_UNLOADING)
		{
			/* process is unloaded */
			result = 0;
			/* i'm not sure what to do */
			/* abort the thread */
		}
		else
		{
			if(returnProcess)
			{
				ILExecThreadJoinProcess(thread, returnProcess);
			}
		}
		if(returnProcess)
		{
			ILMutexUnlock(returnProcess->lock);
		}
		if(leavingProcess)
		{
			ILMutexUnlock(leavingProcess->lock);
		}
		return result;
	}
	else
	{
		/* just clean up the context */
		ILExecThreadClearExecContext(context);
	}
	return 1;
}

/*
 * Let the thread join an other ILExecProcess and save the current state in context.
 * Returns 0 on failure.
 */
int ILExecThreadSwitchToProcess(ILExecThread *thread, ILExecProcess *process, ILExecContext *context)
{
	ILExecProcess *prevProcess = _ILExecThreadProcess(thread);

	if(prevProcess != process)
	{
		int result = 1;

		/* do this so that the locks are always in the same order to avoid a deadlock */
		if(prevProcess < process)
		{
			if(prevProcess)
			{
				ILMutexLock(prevProcess->lock);
			}
			if(process)
			{
				ILMutexLock(process->lock);
			}
		}
		else
		{
			if(process)
			{
				ILMutexLock(process->lock);
			}
			if(prevProcess)
			{
				ILMutexLock(prevProcess->lock);
			}
		}
		/* check if the new process can't be entered */
		if(process->state >= _IL_PROCESS_STATE_UNLOADING)
		{
			/* process is unloaded */
			result = 0;
			if(prevProcess)
			{
				ILExecThreadThrowSystem(thread, "System.AppDomainUnloadedException", 0);
			}
		}
		else
		{
			ILExecThreadSaveExecContext(thread, context);
			if(prevProcess)
			{
				ILExecThreadDetachFromProcess(thread);
			}
			if(process)
			{
				ILExecThreadJoinProcess(thread, process);
			}
			else
			{
				_ILExecThreadProcess(thread) = 0;
			}
			thread->prevContext = context;
			thread->clrThread = 0;
			thread->threadStaticSlots = 0;
			thread->threadStaticSlotsUsed = 0;
		}
		if(process)
		{
			ILMutexUnlock(process->lock);
		}
		if(prevProcess)
		{
			ILMutexUnlock(prevProcess->lock);
		}
		return result;
	}
	else
	{
		/* save the old context anyways */
		ILExecThreadSaveExecContext(thread, context);
		thread->prevContext = context;
	}
	return 1;	
}
#endif

#ifdef	__cplusplus
};
#endif
