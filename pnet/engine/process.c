/*
 * process.c - Manage processes within the runtime engine.
 *
 * Copyright (C) 2001, 2008, 2011  Southern Storm Software, Pty Ltd.
 *
 * Contributions by Thong Nguyen (tum@veridicus.com)
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
#include "il_utils.h"
#include "il_console.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Enable this to print some debug messages.
 */
/*#define PROCESS_DEBUG 1*/

/*
 * Add an application domain to the list of application domains.
 * param:	process = application domain to join to the linked list
 *			(must be not null)
 *			engine  = ILExecEngine to join.
 * Returns: void
 */
static void ILExecProcessJoinEngine(ILExecProcess *process,
									ILExecEngine *engine)
{
	if(!process || !engine)
	{
		return;
	}

	ILMutexLock(engine->processLock);

	process->engine = engine;

	/* The first ILExecProcess attached will be the default process */
	if (!engine->defaultProcess)
	{
		engine->defaultProcess = process;
		ILMutexUnlock(engine->processLock);
		return;
	}

#ifdef IL_CONFIG_APPDOMAINS
	process->id = ++(engine->lastId);
	process->nextProcess = engine->firstProcess;
	process->prevProcess = 0;
	if(engine->firstProcess)
	{
		engine->firstProcess->prevProcess = process;
	}
	engine->firstProcess = process;
#endif

	ILMutexUnlock(engine->processLock);
}

/*
 * Remove an application domain from the list of application domains.
 * param:	process = application domain to remove from the linked list
 *			(must be not null)
 * Returns: void
 */
static void ILExecProcessDetachFromEngine(ILExecProcess *process)
{
	ILExecEngine *engine = process->engine;
	
	if(!process)
	{
		return;
	}

	engine = process->engine;
	
	if(!engine)
	{
		return;
	}

	ILMutexLock(engine->processLock);

	if(engine->defaultProcess == process)
	{
		engine->defaultProcess = 0;
		process->engine = 0,
		ILMutexUnlock(engine->processLock);
		return;
	}

#ifdef IL_CONFIG_APPDOMAINS
	/* Detach the application domain from its process */
	if(process->nextProcess)
	{
		process->nextProcess->prevProcess = process->prevProcess;
	}
	if(process->prevProcess)
	{
		process->prevProcess->nextProcess = process->nextProcess;
	}
	else
	{
		engine->firstProcess = process->nextProcess;
	}
	ILMutexUnlock(engine->processLock);

	/* reset the links */
	process->engine = 0,
	process->prevProcess = 0;
	process->nextProcess = 0;
#else /* !IL_CONFIG_APPDOMAINS */
	/* We should never get here but to be sure release the lock. */
	ILMutexUnlock(engine->processLock);
#endif
}

/*
 * The internal function that does the whole unloading of the process.
 * It simply sets the flag accordingly and aborts all threads that are
 * currently in this process except the finalizer thread.
 * After this is done the collector gets a chance to collect and finalize
 * some stuff.
 * Destruction of the management stuff is up to ILExecProcessDestroyInternal.
 */
static void _ILExecProcessUnloadInternal(ILExecProcess *process)
{
	ILThread *self = ILThreadSelf();
	ILExecThread *thread;
	ILQueueEntry *joinQueue;
	int count = 0;

#ifdef PROCESS_DEBUG
#ifndef REDUCED_STDIO
	fprintf(stderr, "Start unloading process : %p\n", (void *)process);	
#else
	printf("Start unloading process : %p\n", (void *)process);	
#endif
#endif

	joinQueue = ILQueueCreate();

	/* Lock down the process */
	ILMutexLock(process->lock);

	/* Check if the process is already unloading or is unloaded. */
	if(process->state >= _IL_PROCESS_STATE_UNLOADING)
	{
		/* Unlock the process */
		ILMutexUnlock(process->lock);

		return;
	}

	/* and flag the process unloading */
	process->state = _IL_PROCESS_STATE_UNLOADING;

	/* From this point on, no threads can be created inside or enter the AppDomain */

	/* Walk all the threads, collecting CLR thread pointers since they are GC
	   managed and stable */

	thread = process->firstThread;

	while(thread)
	{
		if(thread != process->finalizerThread &&
		   thread->supportThread &&
		   thread->supportThread != self)
		{
			ILQueueAdd(&joinQueue, thread->supportThread);

			/* Cancel possible blocking in kernel call on background threads */
			if(ILThreadGetBackground(thread->supportThread))
			{
				ILThreadSigAbort(thread->supportThread);
			}

			_ILExecThreadAbortThread(ILExecThreadCurrent(), thread->supportThread);

			thread = thread->nextThread;

			count++;
		}
		else
		{
			/* Move onto the next thread */
			thread = thread->nextThread;
		}
	}

	/* Unlock the process */
	ILMutexUnlock(process->lock);

	/* Wait for all threads */
	while(joinQueue)
	{
		ILThread *target;

		target = (ILThread *)ILQueueRemove(&joinQueue);

		ILThreadJoin(target, -1);
	}
	
	process->state = _IL_PROCESS_STATE_RUNNING_FINALIZERS;

	/* Clear 4K of stack space in case it contains some stray pointers 
	   that may still be picked up by not so accurate collectors */
	ILThreadClearStack(4096);

	/* Invoke the finalizers -- hopefully finalizes all objects left in the
	   process being destroyed.  Objects left lingering are orphans */
	ILGCFullCollection(1000);

	if (process->engine)
	{
		ILExecProcessDetachFromEngine(process);
	}

	process->state = _IL_PROCESS_STATE_UNLOADED;
}

/*
 * Internal function to destroy a process.
 * It cleans up all non managed ressources of the process.
 * This function must be called only after the process was unloaded by
 * calling ILExecProcessUnload.
 */
static void _ILExecProcessDestroyInternal(ILExecProcess *process,
										  int isFinalizing)
{
#ifdef PROCESS_DEBUG
#ifndef REDUCED_STDIO
	fprintf(stderr, "Start destroying process : %p\n", (void *)process);	
#else
	printf("Start destroying process : %p\n", (void *)process);	
#endif
#endif
	/* Mark the process as dead in the finalization context. */
	/* This prevents orphans from finalizing. */
	/* If multiple appdomains are supported this function is called  */
	/* during finalization. */
	process->finalizationContext->process = 0;

	/* Destroy the finalizer thread */
	if (process->finalizerThread)
	{
		/* Only destroy the engine thread.  The support thread is shared by other
		   engine processes and is destroyed when the engine is deinitialized */
		_ILExecThreadDestroy(process->finalizerThread);
	}

#ifdef IL_DEBUGGER
	/* Destroy the debugger */
	if(process->debugger)
	{
		ILDebuggerDestroy(process->debugger);
		process->debugger = 0;
	}
#endif

	/* Destroy the coder instance */
	if (process->coder)
	{
		ILCoderDestroy(process->coder);
		process->coder = 0;
	}

	/* Destroy the metadata lock */
	if(process->metadataLock)
	{
		ILRWLockDestroy(process->metadataLock);
		process->metadataLock = 0;
	}

	/* Destroy the image loading context */
	if(process->context)
	{
		/* and destroy the context */
		ILContextDestroy(process->context);
		process->context = 0;
	}

	if (process->internHash)
	{
		/* Destroy the main part of the intern'ed hash table.
		The rest will be cleaned up by the garbage collector */
		ILGCFreePersistent(process->internHash);
		process->internHash = 0;
	}

	if (process->reflectionHash)
	{
		/* Destroy the main part of the reflection hash table.
		The rest will be cleaned up by the garbage collector */
		ILGCFreePersistent(process->reflectionHash);
		process->reflectionHash = 0;
	}

#ifdef IL_CONFIG_PINVOKE
	/* Destroy the loaded module list */
	{
		ILLoadedModule *loaded, *nextLoaded;
		loaded = process->loadedModules;
		while(loaded != 0)
		{
			if(loaded->handle != 0)
			{
				ILDynLibraryClose(loaded->handle);
			}
			nextLoaded = loaded->next;
			ILFree(loaded);
			loaded = nextLoaded;
		}
		process->loadedModules = 0;
	}
#endif

#ifdef IL_CONFIG_RUNTIME_INFRA
	/* Destroy the GC handle table */
	if(process->gcHandles)
	{
		extern void _ILGCHandleTableFree(struct _tagILGCHandleTable *table);
		_ILGCHandleTableFree(process->gcHandles);
		process->gcHandles = 0;
	}
#endif

#ifdef IL_CONFIG_DEBUG_LINES
	/* Destroy the breakpoint watch list */
	{
		ILExecDebugWatch *watch, *nextWatch;
		watch = process->debugWatchList;
		while(watch != 0)
		{
			nextWatch = watch->next;
			ILFree(watch);
			watch = nextWatch;
		}
		process->debugWatchList = 0;
	}
#endif

	if (process->randomLock)
	{
		/* Destroy the random seed pool */
		ILMutexDestroy(process->randomLock);
		process->randomLock = 0;
	}

	if (process->randomPool)
	{
		ILMemZero(process->randomPool, sizeof(process->randomPool));
	}

	_ILExecMonitorProcessDestroy(process);

	if (process->lock)
	{
		/* Destroy the object lock */
		ILMutexDestroy(process->lock);
		process->lock = 0;
	}

	/* free the friendly name if available */
	if(process->friendlyName != 0)
	{
		ILFree(process->friendlyName);
		process->friendlyName = 0;
	}
}

#ifdef IL_CONFIG_APPDOMAINS
/*
 * Finalizer for processes.
 */
static void _ILExecProcessFinalizer(void *block, void *data)
{
	ILExecProcess *process = (ILExecProcess *)GetObjectFromGcBase(block);

	_ILExecProcessDestroyInternal(process, 1);
}
#endif

/*
 * Thread func for unloading a process from a new thread.
 */
static void _ILExecProcessUnloadFunc(void *process)
{
	_ILExecProcessUnloadInternal((ILExecProcess *)process);
}

/*
 * Create the ILExecProcess without creating the coder.
 * Initializing the coder to use is up to the caller.
 */
static ILExecProcess *_ILExecProcessCreateInternal(void)
{
	void *processBase;
	ILExecProcess *process;

	/* Create the process record */
#ifdef IL_CONFIG_APPDOMAINS
	if((processBase = ILGCAlloc
#else
	if((processBase = ILGCAllocPersistent
#endif
						(sizeof(ILExecProcess) + IL_OBJECT_HEADER_SIZE)) == 0)
	{
		return 0;
	}
	process = (ILExecProcess *)GetObjectFromGcBase(processBase);
	/* Initialize the fields */
	process->lock = 0;
	process->state = _IL_PROCESS_STATE_CREATED;
	process->engine = 0;
	process->firstThread = 0;
	process->finalizerThread = 0;
	process->context = 0;
	process->metadataLock = 0;
	process->exitStatus = 0;
	process->coder = 0;
	process->objectClass = 0;
	process->stringClass = 0;
	process->exceptionClass = 0;
	process->clrTypeClass = 0;
	process->outOfMemoryObject = 0;	
	process->commandLineObject = 0;
	process->threadAbortClass = 0;
	ILGetCurrTime(&(process->startTime));
	process->internHash = 0;
	process->reflectionHash = 0;
	process->loadedModules = 0;
	process->gcHandles = 0;
	process->entryImage = 0;
	process->internalClassTable = 0;
	process->friendlyName = 0;
	process->firstClassPrivate = 0;
	process->randomBytesDelivered = 1024;
	process->randomLastTime = 0;
	process->randomCount = 0;
	process->numThreadStaticSlots = 0;
	process->loadFlags = IL_LOADFLAG_FORCE_32BIT;
#if IL_CONFIG_DEBUG_LINES
	process->debugHookFunc = 0;
	process->debugHookData = 0;
	process->debugWatchList = 0;
	process->debugWatchAll = 0;
#endif
#ifdef IL_USE_IMTS
	process->imtBase = 1;
#endif

	/* Initialize the image loading context */
	if((process->context = ILContextCreate()) == 0)
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

	/* Associate the process with the context */
	ILContextSetUserData(process->context, process);

	/* Initialize the object lock */
	process->lock = ILMutexCreate();
	if(!(process->lock))
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

	/* Initialize the finalization context */
	process->finalizationContext = (ILFinalizationContext *)ILGCAlloc(sizeof(ILFinalizationContext));
	if (!process->finalizationContext)
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

	if (!_ILExecMonitorProcessCreate(process))
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

	process->finalizationContext->process = process;

	/* Initialize the metadata lock */
	process->metadataLock = ILRWLockCreate();
	if(!(process->metadataLock))
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

	/* Initialize the random seed pool lock */
	process->randomLock = ILMutexCreate();
	if(!(process->randomLock))
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

#ifdef IL_CONFIG_APPDOMAINS
	ILGCRegisterFinalizer(GetObjectGcBase(process), _ILExecProcessFinalizer, 0);
#endif

	/* Return the process record to the caller */
	return process;
}

/*
 * Create an ILExecProcess in which code can be executed.
 */
ILExecProcess *ILExecProcessCreate(unsigned long stackSize, unsigned long cachePageSize)
{
	ILExecProcess *process;
	ILExecEngine *engine;

	engine = ILExecEngineInstance();

	/* The engine must be initialized prior to creating any processes */
	if(!engine)
	{
		return 0;
	}

#ifndef	IL_CONFIG_APPDOMAINS
	/* Multiple processes are not supported */
	if(engine->defaultProcess)
	{
		return 0;
	}
#endif

	process = _ILExecProcessCreateInternal();
	if(!process)
	{
		return 0;
	}

#ifdef IL_USE_CVM
	process->stackSize = ((stackSize < IL_CONFIG_STACK_SIZE)
							? IL_CONFIG_STACK_SIZE : stackSize);
	process->frameStackSize = IL_CONFIG_FRAME_STACK_SIZE;
#endif

#ifdef IL_USE_JIT
	/* Initialize the JIT coder */
	process->coder = ILCoderCreate(&_ILJITCoderClass, process, 100000, cachePageSize);
#else
	/* Initialize the CVM coder */
	process->coder = ILCoderCreate(&_ILCVMCoderClass, process, 100000, cachePageSize);
#endif
	if(!(process->coder))
	{
		_ILExecProcessDestroyInternal(process, 0);
		return 0;
	}

	/* Attach the process to the engine */
	ILExecProcessJoinEngine(process, ILExecEngineInstance());

	/* Return the process record to the caller */
	return process;
}

/*
 * Import the null coder from "null_coder.c".
 */
extern ILCoder _ILNullCoder;

/*
 * Create an ILExecProcess associated with the null coder.
 * In this process no code can be executed.
 */
ILExecProcess *ILExecProcessCreateNull(void)
{
	ILExecProcess *process;
	ILExecEngine *engine;

	engine = ILExecEngineInstance();

	/* The engine must be initialized prior to creating any processes */
	if(!engine)
	{
		return 0;
	}

#ifndef	IL_CONFIG_APPDOMAINS
	/* Multiple processes are not supported */
	if(engine->defaultProcess)
	{
		return 0;
	}
#endif

	process = _ILExecProcessCreateInternal();
	if(!process)
	{
		return 0;
	}

#ifdef IL_USE_CVM
	process->stackSize = 0;
	process->frameStackSize = 0;
#endif

	process->coder = &_ILNullCoder;

	/* Attach the process to the engine */
	ILExecProcessJoinEngine(process, ILExecEngineInstance());

	/* Return the process record to the caller */
	return process;
}

/*
 * Unloads a process and all threads associated with it.
 * The process may not unload immediately (or even ever).
 * If the current thread exists inside the process, a
 * new thread will be created to unload & destroy the
 * process.  When this function exits, the thread that
 * calls this method will still be able to execute managed
 * code even if it resides within the process it tried to
 * destroy.  The process will eventually be destroyed
 * when the thread (and all other process-owned threads)
 * exit.
 */
void ILExecProcessUnload(ILExecProcess *process)
{
	ILExecThread *execThread = ILExecThreadCurrent();

	if(!process)
	{
		return;
	}

	if(process->state >= _IL_PROCESS_STATE_UNLOADING)
	{
		/* The process is already unloading or was unloaded previously */
		return;
	}

	if(!execThread || execThread->process != process)
	{
		/* Unload was called from an unmanaged thread or
		   a managed thread in a different domain */
		_ILExecProcessUnloadInternal(process);
		return;
	}

	/* We have to run the unload from different thread */
	if(ILHasThreads())
	{
		ILThread *thread;

		if(!(thread = ILThreadCreate(_ILExecProcessUnloadFunc, process)))
		{
			return;
		}
		ILThreadStart(thread);
	}
}

/*
 * Destroy a process.
 *
 * DO NOT call this function from a thread that expects to be
 * alive after the function returns.  This is *NOT* an implementation
 * of AppDomain.Unload.  A thread that calls this function but exists
 * inside the process will be *unusable* from managed code when this
 * function exits.
 *
 * In the implementation of AppDomain.Unload, this method should
 * not be called by a thread that runs inside the domain it is trying
 * to destroy otherwise the thread will return dead & to a dead domain.
 * A new domain-less thread should be created to destroy the domain.
 * On single threaded systems, this method can be called directly
 * by AppDomain.Unload unless the current thread is living in the
 * domain it is trying to unload in which case AppDomain.Unload *MUST*
 * just return without doing anything.
 *
 * Thong Nguyen (tum@veridicus.com)
 */
void ILExecProcessDestroy(ILExecProcess *process)
{
	ILExecThread *thread;

#ifdef IL_CONFIG_APPDOMAINS
	/* Remove the finalization function from this process */
	ILGCRegisterFinalizer(process, 0, 0);
#endif

	if(process->state < _IL_PROCESS_STATE_UNLOADING)
	{
		ILExecProcessUnload(process);
	}
	if(process->state < _IL_PROCESS_STATE_UNLOADED)
	{
		if(ILHasThreads())
		{
			/* something went wrong
			   Is there an other thread executing the unload currently? */
		}
		else
		{
			thread = ILExecThreadCurrent();
			if (thread &&
				thread != process->finalizerThread &&
				thread->process == process)
			{
				ILThreadUnregisterForManagedExecution(ILThreadSelf());
			}
			_ILExecProcessUnloadInternal(process);
		}
	}

	/* We must ensure that objects created and then orphaned by this process
	   won't finalize themselves from this point on (because the process will
	   no longer be valid).  Objects can be orphaned if the GC is conservative
	   (like the boehm GC) */

	/* Disable finalizers to ensure no finalizers are running until we 
	   reenable them */
	if (ILGCDisableFinalizers(10000) == 0)
	{
		/* Finalizers are taking too long.  Abandon unloading of this process */
		return;
	}

	/* Mark the process as dead in the finalization context.  This prevents
	   orphans from finalizing */
	process->finalizationContext->process = 0;

	/* Reenable finalizers */
	ILGCEnableFinalizers();

	/* Unregister (and destroy) the current thread if it 
	   wasn't destroyed above and if it belongs to this domain */
	thread = ILExecThreadCurrent();
	if (thread &&
		thread != process->finalizerThread &&
		thread->process == process)
	{
		ILThreadUnregisterForManagedExecution(ILThreadSelf());
	}

	_ILExecProcessDestroyInternal(process, 0);

#ifndef IL_CONFIG_APPDOMAINS
	/* Free the process block itself */
	ILGCFreePersistent(GetObjectGcBase(process));
#endif
}

void ILExecProcessSetLibraryDirs(ILExecProcess *process,
								 char **libraryDirs,
								 int numLibraryDirs)
{
	ILContextSetLibraryDirs(process->context, libraryDirs, numLibraryDirs);
}

ILContext *ILExecProcessGetContext(ILExecProcess *process)
{
	return process->context;
}

ILExecThread *ILExecProcessGetMain(ILExecProcess *process)
{
	ILExecThread *thread = ILExecThreadCurrent();

	if(!thread)
	{
		thread = ILThreadRegisterForManagedExecution(process, ILThreadSelf());
	}
	return thread;
}

/*
 * Load standard classes and objects.
 */
void _ILExecProcessLoadStandard(ILExecProcess *process,
								ILImage *image)
{
	ILClass *classInfo;

	if(process->state > _IL_PROCESS_STATE_CREATED)
	{
		return;
	}

	if(!(process->outOfMemoryObject))
	{
		/* If this image caused "OutOfMemoryException" to be
		loaded, then create an object based upon it.  We must
		allocate this object ahead of time because we won't be
		able to when the system actually runs out of memory */
		classInfo = ILClassLookupGlobal(ILImageToContext(image),
			"OutOfMemoryException", "System");
		if(classInfo)
		{
			/* Set the system image, for standard type resolutions */
			ILContextSetSystem(ILImageToContext(image),
						   	   ILProgramItem_Image(classInfo));

			/* We don't call the "OutOfMemoryException" constructor,
			to avoid various circularity problems at this stage
			of the loading process */
			process->outOfMemoryObject =
				_ILEngineAllocObject(ILExecProcessGetMain(process), classInfo);
		}
	}
	
	/* Look for "System.Object" */
	if(!(process->objectClass))
	{
		process->objectClass = ILClassLookupGlobal(ILImageToContext(image),
							        			   "Object", "System");
	}

	/* Look for "System.String" */
	if(!(process->stringClass))
	{
		process->stringClass = ILClassLookupGlobal(ILImageToContext(image),
							        			   "String", "System");
	}

	/* Look for "System.Exception" */
	if(!(process->exceptionClass))
	{
		process->exceptionClass = ILClassLookupGlobal(ILImageToContext(image),
							        			      "Exception", "System");
	}

	/* Look for "System.Reflection.ClrType" */
	if(!(process->clrTypeClass))
	{
		process->clrTypeClass = ILClassLookupGlobal(ILImageToContext(image),
								        "ClrType", "System.Reflection");
	}

	/* Look for "System.Threading.ThreadAbortException" */
	if(!(process->threadAbortClass))
	{
		process->threadAbortClass = ILClassLookupGlobal(ILImageToContext(image),
			"ThreadAbortException", "System.Threading");
	}

	process->state = _IL_PROCESS_STATE_LOADED;
}

/*
 * Perform the actions needed prior to executing any code in the process.
 */
static int ILExecProcessInitForExecute(ILExecProcess *process)
{
	if(process->state < _IL_PROCESS_STATE_LOADED)
	{
		return 0;
	}
	else if(process->state == _IL_PROCESS_STATE_LOADED)
	{
#ifdef IL_USE_CVM
		if(!_ILCVMUnrollInitStack(process))
		{
			return 0;
		}
#endif
		process->state = _IL_PROCESS_STATE_EXECUTABLE;
	}

	return 1;
}

#ifndef REDUCED_STDIO

int ILExecProcessLoadImage(ILExecProcess *process, FILE *file)
{
	ILImage *image;
	int loadError;
	ILRWLockWriteLock(process->metadataLock);
	loadError = ILImageLoad(file, 0, process->context, &image,
					   	    process->loadFlags);
	ILRWLockUnlock(process->metadataLock);
	if(loadError == 0)
	{
		_ILExecProcessLoadStandard(process, image);
	}
	return loadError;
}

#endif

int ILExecProcessLoadFile(ILExecProcess *process, const char *filename)
{
	int error;
	ILImage *image;
	ILRWLockWriteLock(process->metadataLock);
	error = ILImageLoadFromFile(filename, process->context, &image,
								process->loadFlags, 0);
	ILRWLockUnlock(process->metadataLock);
	if(error == 0)
	{
		_ILExecProcessLoadStandard(process, image);
	}
	return error;
}

/*
 * Get the last occurance of a directoryseparator in a path
 * up to len.
 *
 * Returns the length of the remaining path including the separator if found else -1.
 */
static int GetLastDirectorySeparator(const char *path, int len)
{

	if(path)
	{
		while(len > 0 && path[len - 1] != '/' &&
		      path[len - 1] != '\\')
		{
			--len;
		}
		return len;
	}
	return -1;
}

/*
 * Setup the application base dir and the friendly name in an ILContext
 * from a filename
 */
static void SetupApplication(ILExecProcess *process, const char *file)
{
	if(file)
	{
		int len = strlen(file);
		int pathLen = GetLastDirectorySeparator(file, len);

		if(pathLen > 0)
		{
			ILContextSetApplicationBaseDir(process->context, ILDupNString(file, pathLen));
			ILExecProcessSetFriendlyName(process, ILDupString(file + pathLen));
		}
		else
		{
			ILContextSetApplicationBaseDir(process->context, 0);
			ILExecProcessSetFriendlyName(process, ILDupString(file));
		}
	}
	else
	{
		ILContextSetApplicationBaseDir(process->context, 0);
		ILExecProcessSetFriendlyName(process, 0);
	}
}

static int ILExecProcessLoadFileInternal(ILExecProcess *process,
										 const char *filename,
										 ILImage **image)
{
	int error;
	ILRWLockWriteLock(process->metadataLock);
	error = ILImageLoadFromFile(filename, process->context, image,
								process->loadFlags, 0);
	ILRWLockUnlock(process->metadataLock);
	return error;
}

static ILMethod *ILExecProcessGetEntryInternal(ILExecProcess *process,
											   ILImage *image)
{
	ILMethod *method = 0;

	ILRWLockReadLock(process->metadataLock);

	if(ILImageType(image) == IL_IMAGETYPE_EXE)
	{
		ILToken token;

		token = ILImageGetEntryPoint(image);
		if(token && (token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_DEF)
		{
			process->entryImage = image;
			method = ILMethod_FromToken(image, token);
		}
	}

	ILRWLockUnlock(process->metadataLock);

	return method;
}

/*
 * Internal worker for ILExecProcessExecuteFile.
 * This function must be called from a managed thread that is in the
 * process where the file has to be executed.
 */
int ILExecProcessExecuteFileInternal(ILExecThread *thread,
									 const char *filename,
									 char *argv[],
									 int* retval)
{
	int error;
	ILMethod *method;
	ILObject *args;
	ILImage *entryImage;
	ILExecValue execValue;
	ILExecValue retValue;
	ILExecProcess *process;

	if(!thread)
	{
		return IL_EXECUTE_ERR_MEMORY;
	}

	if(!(process = _ILExecThreadProcess(thread)))
	{
		return IL_EXECUTE_ERR_MEMORY;
	}

	/* Attempt to load the program into the process */
	error = ILExecProcessLoadFileInternal(process, filename, &entryImage);
	if(error < 0)
	{
		return IL_EXECUTE_ERR_FILE_OPEN;
	}
	else
	{
		if(error > 0)
		{
			return error;
		}
	}
	if(process->state == _IL_PROCESS_STATE_CREATED)
	{
		_ILExecProcessLoadStandard(process, entryImage);
	}
	if(!ILExecProcessInitForExecute(process))
	{
		return IL_EXECUTE_ERR_MEMORY;
	}

	SetupApplication(process, ILImageGetFileName(entryImage));

	method = ILExecProcessGetEntryInternal(process, entryImage);
	if(!method)
	{
		return IL_EXECUTE_ERR_NO_ENTRYPOINT;
	}
	if(ILExecProcessEntryType(method) == IL_ENTRY_INVALID)
	{
		return IL_EXECUTE_ERR_INVALID_ENTRYPOINT;
	}
	args = ILExecProcessSetCommandLine(process, filename, argv);

	/* Call the entry point */
	if(args != 0 && !_ILExecThreadHasException(thread))
	{
		execValue.ptrValue = args;
		ILMemZero(&retValue, sizeof(retValue));
		if(ILExecThreadCallV(thread, method, &retValue, &execValue))
		{
			/* An exception was thrown while executing the program */
			return IL_EXECUTE_ERR_EXCEPTION;
		}
		else
		{
			*retval = retValue.int32Value;
		}
	}
	else
	{
		/* An exception was thrown while building the argument array */
		return IL_EXECUTE_ERR_EXCEPTION;
	}
	return IL_EXECUTE_OK;
}

int ILExecProcessExecuteFile(ILExecProcess *process,
							 const char *filename,
							 char *argv[],
							 int* retval)
{
	ILExecThread *thread;
	int error;

	thread = ILExecThreadCurrent();
	if(!thread)
	{
		ILThread *self = ILThreadSelf();

		/* Called from an unmanaged thread */
		thread = ILThreadRegisterForManagedExecution(process, self);
		if(!thread)
		{
			return IL_EXECUTE_ERR_MEMORY;
		}
		error = ILExecProcessExecuteFileInternal(thread, filename, argv, retval);
		ILThreadUnregisterForManagedExecution(self);
	}
	else
	{
		IL_BEGIN_EXECPROCESS_SWITCH(thread, process)
		error = ILExecProcessExecuteFileInternal(thread, filename, argv, retval);
		IL_END_EXECPROCESS_SWITCH(thread)
	}

	return error;
}

void ILExecProcessSetLoadFlags(ILExecProcess *process, int mask, int flags)
{
	process->loadFlags &= ~mask;
	process->loadFlags |= flags;
}

int ILExecProcessGetStatus(ILExecProcess *process)
{
	return process->exitStatus;
}

ILMethod *ILExecProcessGetEntry(ILExecProcess *process)
{
	ILImage *image = 0;
	ILToken token;
	ILMethod *method = 0;
	ILRWLockReadLock(process->metadataLock);
	while((image = ILContextNextImage(process->context, image)) != 0)
	{
		if(ILImageType(image) != IL_IMAGETYPE_EXE)
		{
			continue;
		}
		token = ILImageGetEntryPoint(image);
		if(token && (token & IL_META_TOKEN_MASK) == IL_META_TOKEN_METHOD_DEF)
		{
			process->entryImage = image;
			method = ILMethod_FromToken(image, token);
			break;
		}
	}
	ILRWLockUnlock(process->metadataLock);
	return method;
}

int ILExecProcessEntryType(ILMethod *method)
{
	ILType *signature;
	ILType *paramType;
	int entryType;

	/* The method must be static */
	if(!ILMethod_IsStatic(method))
	{
		return IL_ENTRY_INVALID;
	}

	/* The method must have either "void" or "int" as the return type */
	signature = ILMethod_Signature(method);
	if(ILType_HasThis(signature))
	{
		return IL_ENTRY_INVALID;
	}
	paramType = ILTypeGetReturn(signature);
	if(paramType == ILType_Void)
	{
		entryType = IL_ENTRY_NOARGS_VOID;
	}
	else if(paramType == ILType_Int32)
	{
		entryType = IL_ENTRY_NOARGS_INT;
	}
	else
	{
		return IL_ENTRY_INVALID;
	}

	/* The method must either have no args or a single "String[]" arg */
	if(ILTypeNumParams(signature) != 0)
	{
		if(ILTypeNumParams(signature) != 1)
		{
			return IL_ENTRY_INVALID;
		}
		paramType = ILTypeGetParam(signature, 1);
		if(!ILType_IsSimpleArray(paramType))
		{
			return IL_ENTRY_INVALID;
		}
		if(!ILTypeIsStringClass(ILTypeGetElemType(paramType)))
		{
			return IL_ENTRY_INVALID;
		}
		entryType += (IL_ENTRY_ARGS_VOID - IL_ENTRY_NOARGS_VOID);
	}

	/* Return the entry point type to the caller */
	return entryType;
}

long ILExecProcessGetParam(ILExecProcess *process, int type)
{
	switch(type)
	{
		case IL_EXEC_PARAM_GC_SIZE:
		{
			return ILGCGetHeapSize();
		}
		/* Not reached */

		case IL_EXEC_PARAM_MC_SIZE:
		{
			return (long)(ILCoderGetCacheSize(process->coder));
		}
		/* Not reached */

		case IL_EXEC_PARAM_MALLOC_MAX:
		{
			extern long _ILMallocMaxUsage(void);
			return _ILMallocMaxUsage();
		}
		/* Not reached */
	}
	return -1;
}

ILObject *ILExecProcessSetCommandLine(ILExecProcess *process,
									  const char *progName, char *args[])
{
	ILExecThread *thread;
	ILObject *mainArgs;
	ILObject *allArgs;
	ILString *argString;
	int opt;
	int argc;
	char *expanded;

	/* Cound the number of arguments in the "args" array */
	argc = 0;
	while(args != 0 && args[argc] != 0)
	{
		++argc;
	}

	/* Create two arrays: one for "Main" and the other for
	   "TaskMethods.GetCommandLineArgs".  The former does
	   not include "argv[0]", but the latter does */
	thread = ILExecProcessGetMain(process);
	mainArgs = ILExecThreadNew(thread, "[oSystem.String;",
						       "(Ti)V", (ILVaInt)argc);
	if(!mainArgs || _ILExecThreadHasException(thread))
	{
		return 0;
	}
	allArgs = ILExecThreadNew(thread, "[oSystem.String;",
						      "(Ti)V", (ILVaInt)(argc + 1));
	if(!allArgs || _ILExecThreadHasException(thread))
	{
		return 0;
	}

	/* Populate the argument arrays */
	expanded = ILExpandFilename(progName, (char *)0);
	if(!expanded)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	argString = ILStringCreate(thread, expanded);
	ILFree(expanded);
	if(!argString || _ILExecThreadHasException(thread))
	{
		return 0;
	}
	ILExecThreadSetElem(thread, allArgs, (ILInt32)0, argString);
	for(opt = 0; opt < argc; ++opt)
	{
		argString = ILStringCreate(thread, args[opt]);
		if(!argString || _ILExecThreadHasException(thread))
		{
			return 0;
		}
		ILExecThreadSetElem(thread, mainArgs, (ILInt32)opt, argString);
		ILExecThreadSetElem(thread, allArgs, (ILInt32)(opt + 1), argString);
	}

	/* Set the value for "TaskMethods.GetCommandLineArgs" */
	process->commandLineObject = allArgs;

	/* Return the "Main" arguments to the caller */
	return mainArgs;
}

void ILExecProcessSetFriendlyName(ILExecProcess *process, char *friendlyName)
{
	if(process->friendlyName && (process->friendlyName != friendlyName))
	{
		ILFree(process->friendlyName);
	}
	process->friendlyName = friendlyName;
}

char *ILExecProcessGetFriendlyName(ILExecProcess *process)
{
	return _ILExecProcessGetFriendlyName(process);
}

#ifndef IL_CONFIG_APPDOMAINS
int ILExecProcessAddInternalCallTable(ILExecProcess* process, 
					const ILEngineInternalClassInfo* internalClassTable,
					int tableSize)
{
	ILEngineInternalClassList* tmp;
	if((!internalClassTable) || (tableSize<=0))return 0;

	if(!(process->internalClassTable))
	{
		process->internalClassTable=(ILEngineInternalClassList*)ILMalloc(
									sizeof(ILEngineInternalClassList));
		process->internalClassTable->size=tableSize;
		process->internalClassTable->list=internalClassTable;
		process->internalClassTable->next=NULL;
		return 1;
	}
	for(tmp=process->internalClassTable;tmp->next!=NULL;tmp=tmp->next);
	tmp->next=(ILEngineInternalClassList*)ILMalloc(
								sizeof(ILEngineInternalClassList));
	tmp=tmp->next; /* advance */
	tmp->size=tableSize;
	tmp->list=internalClassTable;
	tmp->next=NULL;
	return 1;
}
#endif

void ILExecProcessSetCoderFlags(ILExecProcess *process,int flags)
{
	ILCoderSetFlags(process->coder,flags);
}

#ifdef	__cplusplus
};
#endif
