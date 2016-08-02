/*
 * cctormgr.c - Queue and execute class initializers (static constructors).
 *
 * Copyright (C) 2001, 2011  Southern Storm Software, Pty Ltd.
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

#include "cctormgr.h"
#include "lib_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Forward declaration.
 */
static int _ILCCtorMgr_RunCCtors(ILCCtorMgr *cctorMgr,
								 ILExecThread *thread,
								 ILClass *classes[],
								 ILInt32 numClasses);

/*
 * Free an ILMethodLockEntry.
 */
static void _ILMethodLockEntry_Destroy(ILMethodLockPool *lockPool,
									   ILMethodLockEntry *lockEntry)
{
	if(lockPool && lockEntry)
	{
		/* Destroy the semaphore object. */
		ILSemaphoreDestroy(lockEntry->sem);

		/* And free the memory. */
		ILFree(lockEntry->sem);

		/* Free this lock entry. */
		ILMemPoolFree(&(lockPool->methodPool), lockEntry);
	}
}

/*
 * Initialize a mathod lock pool.
 */
static int _ILMethodLockPool_Init(ILMethodLockPool *lockPool)
{
	if(lockPool)
	{
		/* Initialize the lock mutex. */
		if(!(lockPool->lock = ILMutexCreate()))
		{
			return 0;
		}

		/* Intialize the pool for the class infos. */
		ILMemPoolInit(&(lockPool->methodPool),
					  sizeof(ILMethodLockEntry),
					  5);

		lockPool->lastLockedMethod = 0;

		return 1;
	}
	return 0;
}

/*
 * Destroy a mathod lock pool.
 */
static int _ILMethodLockPool_Destroy(ILMethodLockPool *lockPool)
{
	if(lockPool)
	{
		ILMemPoolDestroy(&(lockPool->methodPool));

		if(lockPool->lock)
		{
			/* Destroy the lock mutex. */
			ILMutexDestroy(lockPool->lock);

			ILFree(lockPool->lock);
		}
	}
	return 0;
}

/*
 * Add a method to the lock pool.
 * Returns 0 on failure.
 */
static int _ILMethodLockPool_LockMethod(ILMethodLockPool *lockPool,
										ILMethod *method,
										void *userData,
										ILClass *classesToInitialize[],
										ILInt32 numClassesToInitialize)
{
	if(lockPool)
	{
		ILExecThread *thread = ILExecThreadCurrent();
		ILMethodLockEntry *currentLockEntry;

		ILMutexLock(lockPool->lock);

		if(!(currentLockEntry = ILMemPoolAlloc(&(lockPool->methodPool),
											   ILMethodLockEntry)))
		{
			ILMutexUnlock(lockPool->lock);
			return 0;
		}

		if(!(currentLockEntry->sem = ILSemaphoreCreate()))
		{
			ILMemPoolFree(&(lockPool->methodPool), currentLockEntry);
			ILMutexUnlock(lockPool->lock);
			return 0;
		}
		/* We are using the support thread (ILThread) here to avoid */
		/* blocking during execution of finalizers where the ILExecThread */
		/* for the same ILThread might change. */
		currentLockEntry->thread = thread->supportThread;

		currentLockEntry->method = method;
		currentLockEntry->numWaitingThreads = 0;
		currentLockEntry->userData = userData;
		currentLockEntry->classesToInitialize = classesToInitialize;
		currentLockEntry->numClassesToInitialize = numClassesToInitialize;
		currentLockEntry->nextEntry = lockPool->lastLockedMethod;
		lockPool->lastLockedMethod = currentLockEntry;

		ILMutexUnlock(lockPool->lock);

		/* Return with success. */
		return 1;
	}
	return 0;
}

/*
 * Unlock a locked method.
 */
static void *_ILMethodLockPool_UnlockMethod(ILMethodLockPool *lockPool,
										   ILMethod *method)
{
	if(lockPool)
	{
		ILMethodLockEntry *currentLockEntry = lockPool->lastLockedMethod;

		if(currentLockEntry)
		{
			/* Lock the lock pool. */
			ILMutexLock(lockPool->lock);

			currentLockEntry = lockPool->lastLockedMethod;

			if(currentLockEntry)
			{
				if(currentLockEntry->method == method)
				{
					lockPool->lastLockedMethod = currentLockEntry->nextEntry;
				}
				else
				{
					ILMethodLockEntry *tempLockEntry;

					while(currentLockEntry)
					{
						if((currentLockEntry->nextEntry) &&
						   (currentLockEntry->nextEntry->method == method))
						{
							tempLockEntry = currentLockEntry->nextEntry;
							currentLockEntry->nextEntry = currentLockEntry->nextEntry->nextEntry;
							currentLockEntry = tempLockEntry;
							break;
						}
						currentLockEntry = currentLockEntry->nextEntry;
					}
				}				
				if(currentLockEntry)
				{
					/* We found the lock entry for the given method and */
					/* unlinked it from the list. */
					/* So the locked method can't be found by any other */
					/* thread now. */

					void *userData = currentLockEntry->userData;

					if(currentLockEntry->numWaitingThreads == 0)
					{
						/* There are no threads waiting for thie method. */
						/* So we have to destroy the lock entry here. */

						_ILMethodLockEntry_Destroy(lockPool, currentLockEntry);
					}
					else
					{
						/* Release all threads waiting for this method. */
						ILSemaphorePostMultiple(currentLockEntry->sem,
												currentLockEntry->numWaitingThreads);
					}
					/* Unlock the lock pool. */
					ILMutexUnlock(lockPool->lock);

					return userData;
				}
			}

			/* Unlock the lock pool. */
			ILMutexUnlock(lockPool->lock);
		}
	}
	return 0;
}

/*
 * Check if a method is locked and block the calling thread if the method is
 * locked by an other thread.
 * If the method is locked by the current thread the userData entry of the
 * lock entry is returned.
 * Returns 0 if the method is not locked, 
 */
void *ILMethodLockPool_HandleLockedMethod(ILMethodLockPool *lockPool,
										  ILMethod *method,
										  ILCCtorMgr *cctorMgr)
{
	if(lockPool)
	{
		ILMethodLockEntry *currentLockEntry = lockPool->lastLockedMethod;

		if(currentLockEntry)
		{
			/* Lock the lock pool. */
			ILMutexLock(lockPool->lock);

			currentLockEntry = lockPool->lastLockedMethod;

			while(currentLockEntry)
			{
				if(currentLockEntry->method == method)
				{
					break;
				}
				currentLockEntry = currentLockEntry->nextEntry;
			}
			if(currentLockEntry)
			{
				/* The method is locked. */
				ILExecThread *thread = ILExecThreadCurrent();

				if(currentLockEntry->thread == thread->supportThread)
				{
					/* The current thread locked the method. */

					/* Unlock the lock pool. */
					ILMutexUnlock(lockPool->lock);

					/* And return the userData entry. */
					return currentLockEntry->userData;
				}
				else if(cctorMgr->thread == thread->supportThread)
				{
					/* We have to run the cctors queued for this method. */
					/* Unlock the lock pool. */
					ILMutexUnlock(lockPool->lock);

					/* and execute the cctors queued for this method. */
					_ILCCtorMgr_RunCCtors(cctorMgr,
										  thread,
										  currentLockEntry->classesToInitialize,
										  currentLockEntry->numClassesToInitialize);

					/* And return the userData entry. */
					return currentLockEntry->userData;
				}
				else
				{
					ILExecProcess *process = ((ILClassPrivate *)(ILMethod_Owner(method)->userData))->process;

					/* Increase the number of waiting threads. */
					++(currentLockEntry->numWaitingThreads);

					/* Unlock the lock pool. */
					ILMutexUnlock(lockPool->lock);

					/* Unlock the metadata. */
					METADATA_UNLOCK(process);

					/* And wait at the semaphore object. */
					ILSemaphoreWait(currentLockEntry->sem);

					/* Lock the metadata again. */
					METADATA_WRLOCK(process);

					/* Lock the lock pool again. */
					ILMutexLock(lockPool->lock);

					/* Decrease the number of waiting threads. */
					--(currentLockEntry->numWaitingThreads);

					if(currentLockEntry->numWaitingThreads == 0)
					{
						/* This is the last thread waiting for this method. */
						void *userData = currentLockEntry->userData;

						_ILMethodLockEntry_Destroy(lockPool, currentLockEntry);

						/* Unlock the lock pool. */
						ILMutexUnlock(lockPool->lock);

						return userData;
					}
					else
					{
						/* Unlock the lock pool. */
						ILMutexUnlock(lockPool->lock);

						return currentLockEntry->userData;
					}
				}
			}
			/* Unlock the lock pool. */
			ILMutexUnlock(lockPool->lock);
		}
	}
	return 0;
}

/*
 * Get the number of queued cctors.
 */
static ILInt32 _ILCCtorMgr_GetNumQueuedCCtors(ILCCtorMgr *cctorMgr)
{
	ILInt32 numQueuedCCtors = 0;
	ILClassEntry *classEntry = cctorMgr->lastClass;

	while(classEntry)
	{
		numQueuedCCtors++;
		classEntry = classEntry->prevClass;
	}
	return numQueuedCCtors;
}

/*
 * Execute the cctor of the given class.
 * This function must be executed with the current thread holding the
 * cctor manager's lock.
 */
static int _ILCCtorMgr_RunCCtor(ILCCtorMgr *cctorMgr,
								ILExecThread *thread,
								ILClass *classInfo)
{
	if((classInfo->attributes &
		(IL_META_TYPEDEF_CCTOR_RUNNING | IL_META_TYPEDEF_CCTOR_ONCE)) == 0)
	{
		/* The cctor was not allready executed and is not currently running. */
		/* So find the cctor now. */
		ILMethod *cctor = 0;

		IL_METADATA_RDLOCK(_ILExecThreadProcess(thread));
		while((cctor = (ILMethod *)ILClassNextMemberByKind(classInfo,
														  (ILMember *)cctor,
														  IL_META_MEMBERKIND_METHOD)) != 0)
		{
			if(ILMethod_IsStaticConstructor(cctor))
			{
				break;
			}
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(thread));
		if(cctor)
		{
			/* We found the cctor so we can execute it now. */
			classInfo->attributes |= IL_META_TYPEDEF_CCTOR_RUNNING;
			if(ILExecThreadCall(thread, cctor, 0))
			{
				classInfo->attributes &= ~IL_META_TYPEDEF_CCTOR_RUNNING;
				return 0;
			}
		}
		classInfo->attributes |= IL_META_TYPEDEF_CCTOR_ONCE;
	}
	return 1;
}

/*
 * Execute the queued cctors.
 * This function must be executed with the current thread holding the
 * cctor manager's lock.
 */
static int _ILCCtorMgr_RunCCtors(ILCCtorMgr *cctorMgr,
								 ILExecThread *thread,
								 ILClass *classes[],
								 ILInt32 numClasses)
{
	ILInt32 currentCCtor;

	/* And run the cctors now. */
	for(currentCCtor = 0; currentCCtor < numClasses; currentCCtor++)
	{
		if(!(_ILCCtorMgr_RunCCtor(cctorMgr,
								  thread,
								  classes[currentCCtor])))
		{
			return 0;
		}
	}
	return 1;
}

/*
 * Lookup the class entry for the given class in the cctor manager and return
 * the pointer to the class entry or add a new class entry and return the
 * pointer to the newly created entry.
 * Returns 0 on out of memory.
 */
static ILClassEntry *ILCCtorMgr_FindOrAddClass(ILCCtorMgr *cctorMgr,
											   ILClass *classInfo)
{
	ILClassEntry *currentClass = cctorMgr->lastClass;

	while(currentClass)
	{
		if(currentClass->classInfo == classInfo)
		{
			return currentClass;
		}
		currentClass = currentClass->prevClass;
	}

	/* Add the new class to the queue. */
	if(!(currentClass = ILMemPoolAlloc(&(cctorMgr->classPool),
									   ILClassEntry)))
	{
		return 0;
	}
	/* Now fill the class entry with the current class. */
	currentClass->classInfo = classInfo;
	currentClass->prevClass = cctorMgr->lastClass;
	cctorMgr->lastClass = currentClass;

	return currentClass;
}

/*
 * Add a class to the queue of the classes which have to be initialized.
 */
static int _ILCCtorMgr_QueueClass(ILCCtorMgr *cctorMgr,
						  ILClass *classInfo)
{
	ILMethod *cctor = 0;
	ILClassEntry *currentClass = 0;

	if(!cctorMgr)
	{
		return 0;
	}

	if(!classInfo)
	{
		return 0;
	}

	if((classInfo->attributes & IL_META_TYPEDEF_CCTOR_ONCE) != 0)
	{
		/* We already know that the static constructor has been executed, */
		return 1;
	}

	while((cctor = (ILMethod *)ILClassNextMemberByKind(classInfo,
													  (ILMember *)cctor,
													  IL_META_MEMBERKIND_METHOD)) != 0)
	{
		if(ILMethod_IsStaticConstructor(cctor))
		{
			break;
		}
	}
	if(cctor == 0)
	{
		/* We couldn't find a cctor. */
		/* So we flag the class as if the cctor was executed. */
		classInfo->attributes |= IL_META_TYPEDEF_CCTOR_ONCE;
		return 1;
	}

	/* Check if the class is allready in the queue or add it. */
	if(!(currentClass = ILCCtorMgr_FindOrAddClass(cctorMgr, classInfo)))
	{
		return 0;
	}

	/* And exit with success. */
	return 1;
}

/*
 * Throw a System.TypeInitializationException.
 */
static void _ILCCtorMgr_ThrowTypeInitializationException(ILExecThread *thread)
{
	ILObject *exception;

	exception = _ILExecThreadGetException(thread);
	if(exception)
	{
		ILObject *typeInitializationException = 0;

		_ILExecThreadClearException(thread);

		ILExecThreadThrowSystem(thread,
								"System.TypeInitializationException",
								 0);

		typeInitializationException = _ILExecThreadGetException(thread);
		if(typeInitializationException)
		{
			/* Set the inner exception thrown by the class initializer. */
			((System_Exception *)typeInitializationException)->innerException = (System_Exception *)exception;
		}
	}
}

/*
 * Initialize a cctor manager instance.
 */
int ILCCtorMgr_Init(ILCCtorMgr *cctorMgr,
						   ILInt32 numCCtorEntries)
{
	if(cctorMgr)
	{
		/* Initialize the lock mutex. */
		if(!(cctorMgr->lock = ILMutexCreate()))
		{
			return 0;
		}

		if(!_ILMethodLockPool_Init(&(cctorMgr->lockPool)))
		{
			ILMutexDestroy(cctorMgr->lock);
			return 0;
		}

		cctorMgr->thread = (ILThread *)0;;
		cctorMgr->currentMethod = (ILMethod *)0;
	#ifdef IL_USE_JIT
		cctorMgr->currentJitFunction = 0;
	#endif	/* IL_USE_JIT */
		cctorMgr->isStaticConstructor = 0;
		cctorMgr->isConstructor = 0;

		cctorMgr->lastClass = (ILClassEntry *)0;

		/* Intialize the pool for the class infos. */
		ILMemPoolInit(&(cctorMgr->classPool),
					  sizeof(ILClassEntry),
					  numCCtorEntries);


		return 1;
	}
	return 0;
}

/*
 * Destroy a cctor manager instance.
 */
void ILCCtorMgr_Destroy(ILCCtorMgr *cctorMgr)
{
	if(cctorMgr)
	{
		ILMemPoolDestroy(&(cctorMgr->classPool));

		_ILMethodLockPool_Destroy(&(cctorMgr->lockPool));

		if(cctorMgr->lock)
		{
			/* Destroy the lock mutex. */
			ILMutexDestroy(cctorMgr->lock);
		}
	}
}

/*
 * Handle locked methods.
 * Returns the userData of the locked method if the method is locked and
 * 0 if the method isn't locked.
 */
void *ILCCtorMgr_HandleLockedMethod(ILCCtorMgr *cctorMgr,
									ILMethod *method)
{
	return ILMethodLockPool_HandleLockedMethod(&(cctorMgr->lockPool),
											   method,
											   cctorMgr);
}

/*
 * Set the current method to be compiled.
 * This checks if the class initializer of the class owning the method has
 * to be executed prior to executing the method.
 */
void ILCCtorMgr_SetCurrentMethod(ILCCtorMgr *cctorMgr,
								 ILMethod *method)
{
	if(cctorMgr)
	{
	#ifdef IL_USE_JIT
		cctorMgr->currentJitFunction = 0;
	#endif	/* IL_USE_JIT */

		if(method)
		{
			ILClass *methodOwner = ILMethod_Owner(method);

			/* Setup the information for the current method. */
			cctorMgr->currentMethod = method;
			if(ILMethod_IsConstructor(method))
			{
				cctorMgr->isConstructor = 1;
				cctorMgr->isStaticConstructor = 0;
			}
			else
			{
				cctorMgr->isConstructor = 0;
				if(ILMethod_IsStaticConstructor(method))
				{
					cctorMgr->isStaticConstructor = 1;
				}
				else
				{
					cctorMgr->isStaticConstructor = 0;
				}
			}

			if((methodOwner->attributes & IL_META_TYPEDEF_CCTOR_ONCE) != 0)
			{
				/* We already know that the static constructor has been called, */
				return;
			}

			/* Now check if the class initializer of the method's owner */
			/* needs to be executed first and queue the class */
			/* if neccessairy. */
			if((methodOwner->attributes & IL_META_TYPEDEF_BEFORE_FIELD_INIT) == 0)
			{
				if(ILMethod_IsStatic(method))
				{
					/* We have to call the cctor before calling any static method */
					/* of this type */
					_ILCCtorMgr_QueueClass(cctorMgr, methodOwner);
					return;
				}
				if(cctorMgr->isConstructor)
				{
					/* We have to call the cctor before calling a */
					/* constructor of this type. */
					_ILCCtorMgr_QueueClass(cctorMgr, methodOwner);
					return;
				}
			}
		}
		else
		{
			cctorMgr->currentMethod = (ILMethod *)0;
			cctorMgr->isStaticConstructor = 0;
			cctorMgr->isConstructor = 0;
		}
	}
}

/*
 * Call this method before any non virtual method call is done.
 * It checks if the static constructor for the method owner has to be invoked
 * before the call is done.
 */
int ILCCtorMgr_OnCallMethod(ILCCtorMgr *cctorMgr,
							ILMethod *method)
{
	ILClass *methodOwner = ILMethod_Owner(method);

	if((methodOwner->attributes & IL_META_TYPEDEF_CCTOR_ONCE) != 0)
	{
		/* We already know that the static constructor has been called, */
		return 1;
	}

	if((methodOwner->attributes & IL_META_TYPEDEF_BEFORE_FIELD_INIT) == 0)
	{
		/* We have to call the cctor before calling any static method */
		/* or constructor of this type. */
		if(ILMethod_IsStatic(method) || ILMethod_IsConstructor(method))
		{
			if(cctorMgr->isStaticConstructor)
			{
				ILClass *cctorOwner = ILMethod_Owner(cctorMgr->currentMethod);

				if(cctorOwner == methodOwner)
				{
					/* We are in the static constructor of the class owning */
					/* the method. So we don't need to call the cctor again. */
					return 1;
				}
			}
			return _ILCCtorMgr_QueueClass(cctorMgr, methodOwner);
		}
	}
	return 1;
}

/*
 * Call this method before accessing any static field.
 * It checks if the static constructor for the field's owner has to be invoked
 * before the field is accessed.
 */
int ILCCtorMgr_OnStaticFieldAccess(ILCCtorMgr *cctorMgr,
								   ILField *field)
{
	ILClass *fieldOwner = ILField_Owner(field);

	if((fieldOwner->attributes & IL_META_TYPEDEF_CCTOR_ONCE) != 0)
	{
		/* We already know that the static constructor has been called, */
		return 1;
	}

	if(ILField_IsStatic(field))
	{
		/* We have to call the cctor before accessing any static field */
		/* of this type */
		if(cctorMgr->isStaticConstructor)
		{
			ILClass *methodOwner = ILMethod_Owner(cctorMgr->currentMethod);

			if(methodOwner == fieldOwner)
			{
				/* We are in the static constructor of the class owning */
				/* the field. So we don't need to call the cctor again. */
				return 1;
			}
		}
		return _ILCCtorMgr_QueueClass(cctorMgr, fieldOwner);
	}
	return 1;
}

/*
 * Run the cctor for the given class.
 */
int ILCCtorMgr_RunCCtor(ILCCtorMgr *cctorMgr, ILClass *classInfo)
{
	if((cctorMgr != 0) && (classInfo != 0))
	{
		if((classInfo->attributes & IL_META_TYPEDEF_CCTOR_ONCE) != 0)
		{
			/* We already know that the static constructor has been called, */
			return 1;
		}
		else
		{
			ILExecThread *thread = ILExecThreadCurrent();
			int result;

			if(cctorMgr->thread == thread->supportThread)
			{
				/* We allready hold the cctor lock. */
				result = _ILCCtorMgr_RunCCtor(cctorMgr,
									 		  thread,
									 		  classInfo);
			}
			else
			{
				/* Lock the cctor manager. */
				ILMutexLock(cctorMgr->lock);

				/* And set the thread holding the lock. */
				cctorMgr->thread = thread->supportThread;

				result = _ILCCtorMgr_RunCCtor(cctorMgr,
									 		  thread,
									 		  classInfo);

				/* reset the thread holding the lock. */
				cctorMgr->thread = (ILThread *)0;
				/* and unlock the cctor manager. */
				ILMutexUnlock(cctorMgr->lock);
			}
			if(!result)
			{
				_ILCCtorMgr_ThrowTypeInitializationException(thread);
			}
			return result;
		}
	}
	return 0;
}

/*
 * Run the queued cctors.
 */
int ILCCtorMgr_RunCCtors(ILCCtorMgr *cctorMgr, void *userData)
{
	if(cctorMgr)
	{
		ILExecThread *thread = ILExecThreadCurrent();

		if(!(cctorMgr->lastClass))
		{
			/* There are no cctors queued. */

			/* So store the userdata where the coder expects it to be. */
		#ifdef IL_USE_CVM
			cctorMgr->currentMethod->userData = userData;
		#endif	/* IL_USE_CVM */
		#ifdef IL_USE_JIT
			jit_function_setup_entry(cctorMgr->currentJitFunction, userData);
		#endif	/* IL_USE_JIT */

			/* Unlock the metadata. */
			METADATA_UNLOCK(_ILExecThreadProcess(thread));

			/* and return with success. */
			return 1;
		}
		else
		{
			int result;
			ILMethod *currentMethod = cctorMgr->currentMethod;
		#ifdef IL_USE_JIT
			ILJitFunction currentJitFunction = cctorMgr->currentJitFunction;
		#endif	/* IL_USE_JIT */
	
			ILInt32 numQueuedCCtors = _ILCCtorMgr_GetNumQueuedCCtors(cctorMgr);
			ILClass *classes[numQueuedCCtors];
			ILClassEntry *classEntry = cctorMgr->lastClass;
			ILInt32 currentCCtor;

			/* Copy the cctors to execute to the class array. */
			for(currentCCtor = 0; currentCCtor < numQueuedCCtors; currentCCtor++)
			{
				classes[currentCCtor] = classEntry->classInfo;
				classEntry = classEntry->prevClass;
			}

			/* and reset the queue in the cctor manager. */
			cctorMgr->lastClass = (ILClassEntry *)0;
			ILMemPoolClear(&(cctorMgr->classPool));

			/* Lock the method. */
			if(!_ILMethodLockPool_LockMethod(&(cctorMgr->lockPool),
											 currentMethod,
											 userData,
											 classes,
											 numQueuedCCtors))
			{
				/* Unlock the metadata. */
				METADATA_UNLOCK(_ILExecThreadProcess(thread));

				/* and return with failure. */
				return 0;
			}

			/* Now unlock the metadata. */
			METADATA_UNLOCK(_ILExecThreadProcess(thread));

			if(cctorMgr->thread == thread->supportThread)
			{
				/* We allready hold the cctor lock. */
				result = _ILCCtorMgr_RunCCtors(cctorMgr,
									 		   thread,
									 		   classes,
									 		   numQueuedCCtors);
			}
			else
			{
				/* Lock the cctor manager. */
				ILMutexLock(cctorMgr->lock);

				/* And set the thread holding the lock. */
				cctorMgr->thread = thread->supportThread;

				result = _ILCCtorMgr_RunCCtors(cctorMgr,
									 		   thread,
									 		   classes,
									 		   numQueuedCCtors);

				/* reset the thread holding the lock. */
				cctorMgr->thread = (ILThread *)0;
				/* and unlock the cctor manager. */
				ILMutexUnlock(cctorMgr->lock);
			}

			/* Lock the metadata again. */
			IL_METADATA_WRLOCK(_ILExecThreadProcess(thread));

			/* Store the userdata where the coder expects it to be. */
		#ifdef IL_USE_CVM
			currentMethod->userData = userData;
		#endif	/* IL_USE_CVM */
		#ifdef IL_USE_JIT
			jit_function_setup_entry(currentJitFunction, userData);
		#endif	/* IL_USE_JIT */

			/* Unlock the method. */
			_ILMethodLockPool_UnlockMethod(&(cctorMgr->lockPool),
										   currentMethod);


			/* Unlock the metadata. */
			IL_METADATA_UNLOCK(_ILExecThreadProcess(thread));

			if(!result)
			{
				_ILCCtorMgr_ThrowTypeInitializationException(thread);
			}
			return result;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
