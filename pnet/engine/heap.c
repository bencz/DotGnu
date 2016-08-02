/*
 * heap.c - Heap routines for the runtime engine.
 *
 * Copyright (C) 2001, 2008, 2009, 2011  Southern Storm Software, Pty Ltd.
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

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Initialize a class.
 */
static int InitializeClass(ILExecThread *thread, ILClass *classInfo)
{
	/* Quick check to see if the class is already laid out,
	   to avoid acquiring the metadata lock if possible */
	if(_ILLayoutAlreadyDone(classInfo))
	{
		return 1;
	}

	/* Acquire the metadata write lock and disable finalizers */
	METADATA_WRLOCK(_ILExecThreadProcess(thread));

	/* Lay out the class's fields.  This will check for layout
	   again, to avoid race condition situations */
	if(!_ILLayoutClass(_ILExecThreadProcess(thread), classInfo))
	{
		/* Throw a "TypeInitializationException" */
		METADATA_UNLOCK(_ILExecThreadProcess(thread));
		_ILExecThreadSetException(thread, _ILSystemException
			(thread, "System.TypeInitializationException"));
		return 0;
	}

	/* Re-enable finalizers and unlock the metadata write lock */
	METADATA_UNLOCK(_ILExecThreadProcess(thread));
	return 1;
}

static IL_INLINE ILMethod *FindFinalizeMethod(ILClassPrivate *classPrivate)
{
	ILMethod *method;
	const char *methodName;
	ILType *signature;
	ILClass *classInfo;
	
	if (!classPrivate->hasFinalizer)
	{
		return 0;
	}

	classInfo = classPrivate->classInfo;

	while(classInfo != 0)
	{
		method = 0;
		while((method = (ILMethod *)ILClassNextMemberByKind
			(classInfo, (ILMember *)method, IL_META_MEMBERKIND_METHOD)) != 0)
		{
			methodName = ILMethod_Name(method);

			if(methodName[0] == 'F' && strcmp(methodName, "Finalize") == 0)
			{
				signature = ILMethod_Signature(method);

				if(ILTypeGetReturn(signature) == ILType_Void &&
				   ILTypeNumParams(signature) == 0)
				{
					return method;
				}
			}
		}
		classInfo = ILClass_ParentClass(classInfo);
	}

	return 0;
}

/* This method assumes there is only one finalizer thread for the entire os process */
void _ILFinalizeObject(void *block, void *data)
{
	ILObject *object;
	ILMethod *method;
	ILThread *thread;
	ILExecProcess *process;
	ILExecThread *execThread;
	ILFinalizationContext *finalizationContext;
	ILThreadExecContext newContext, saveContext;
	
	/* Get the finalization context */
	finalizationContext = (ILFinalizationContext *)data;

	/* Skip the object header within the block */
	object = GetObjectFromGcBase(block);

	/* Get the process that created the object */
	process = finalizationContext->process;

	/* Make sure the process is still alive */
	if (process == 0)
	{
		/* Our owner process died.  We're orphaned and can't finalize */
#ifndef IL_CONFIG_USE_THIN_LOCKS
		/*
		 * We'll look for a monitor that's still attached to the object
		 * anyways and reclaim it if needed.
		 */
		ILMonitorReclaim((void **)GetObjectLockWordPtr(execThread, object));
#endif
		return;
	}

	/* Get the engine thread to execute the finalizer on */
	execThread = process->finalizerThread;

	/* Get the finalizer thread instance */
	thread = ILThreadSelf();
	
	if (execThread == 0)
	{
		/* Create a new engine thread for the finalizers of this process to run on */

		execThread = _ILExecThreadCreate(process, 1);

		if (execThread == 0)
		{
			return;
		}

		process->finalizerThread = execThread;
	}

	newContext.execThread = execThread;

	/* Make the new thread execute on the finalizer ILExecThread
	   (could be the main ILExecThread on single threaded systems) */
	_ILThreadSetExecContext(thread, &newContext, &saveContext);

	method = FindFinalizeMethod(GetObjectClassPrivate(object));

	if (method != 0)
	{
		ILGCRegisterFinalizer(block, 0, 0);
		
		/* Invoke the "Finalize" method on the object */
		if(ILExecThreadCall(ILExecThreadCurrent(),
							method, (void *)0, object))
		{
			_ILExecThreadClearException(ILExecThreadCurrent());
		}
	}
	
#ifndef IL_CONFIG_USE_THIN_LOCKS
	/*
	 * We'll look for a monitor that's still attached to the object
	 * anyways and reclaim it if needed.
	 */
	ILMonitorReclaim((void **)GetObjectLockWordPtr(execThread, object));
#endif

	/* On multi-threaded systems, the finalizer ILExecThread and main
	   ILExecThreads are distinct *but* it is possible for the main
	   ILThread to execute finalizers (when synchronous finalization
	   is used) so the ILThread needs to be reassociated with the
	   original ILExecThread rather than the finalizer ILExecThread. */
	_ILThreadRestoreExecContext(thread, &saveContext);
}

ILObject *_ILEngineAlloc(ILExecThread *thread, ILClass *classInfo,
						 ILUInt32 size)
{
	void *ptr;
	ILObject *obj;

	if (classInfo == 0)
	{
		/* Allocating non-object memory so no need to make space for the header. */
		return ILGCAlloc(size);
	}
	else
	{
		/* Make sure the class has been initialized before we start */
		if (!InitializeClass(thread, classInfo))
		{
			return 0;
		}

		/* Allocate memory from the heap */
		ptr = ILGCAlloc(size + IL_OBJECT_HEADER_SIZE);
		
		if(!ptr)
		{
			/* Throw an "OutOfMemoryException" */
			_ILExecThreadSetOutOfMemoryException(thread);
			return 0;
		}

		obj = GetObjectFromGcBase(ptr);

		/* Set the class into the block */
		SetObjectClassPrivate(obj, (ILClassPrivate *)(classInfo->userData));
		

		/* Attach a finalizer to the object if the class has
		a non-trival finalizer method attached to it */
		if(((ILClassPrivate *)(classInfo->userData))->hasFinalizer)
		{
			ILGCRegisterFinalizer(ptr, _ILFinalizeObject, thread->process->finalizationContext);
		}

		/* Return a pointer to the object */
		return obj;
	}
}

ILObject *_ILEngineAllocAtomic(ILExecThread *thread, ILClass *classInfo,
							   ILUInt32 size)
{
	void *ptr;
	ILObject *obj;
	ILClassPrivate *classPrivate;

	if (classInfo == 0)
	{
		/* Allocating non-object memory so no need to make space for the header. */
		return ILGCAllocAtomic(size);
	}
	else
	{
		classInfo = ILClassResolve(classInfo);

		/* Make sure the class has been initialized before we start */
		if(!InitializeClass(thread, classInfo))
		{
			return 0;
		}

#ifdef IL_CONFIG_USE_THIN_LOCKS
		/* Allocate memory from the heap */
		ptr = ILGCAllocAtomic(size + IL_OBJECT_HEADER_SIZE);
#else
		classPrivate = (ILClassPrivate *)(classInfo->userData);

		/* TODO: Move descriptor creation to layout.c */
		if (classPrivate->gcTypeDescriptor == IL_MAX_NATIVE_UINT)
		{
			ILNativeUInt bitmap = IL_OBJECT_HEADER_PTR_MAP;

			classPrivate->gcTypeDescriptor = ILGCCreateTypeDescriptor(&bitmap, IL_OBJECT_HEADER_SIZE / sizeof(ILNativeInt));		
		}

		ptr = ILGCAllocExplicitlyTyped(size + IL_OBJECT_HEADER_SIZE, classPrivate->gcTypeDescriptor);
#endif
		if(!ptr)
		{
			/* Throw an "OutOfMemoryException" */
			_ILExecThreadSetOutOfMemoryException(thread);
			return 0;
		}
		
		obj = GetObjectFromGcBase(ptr);

		SetObjectClassPrivate(obj, (ILClassPrivate *)(classInfo->userData));
		
		/* Attach a finalizer to the object if the class has
		   a non-trival finalizer method attached to it */
		if(((ILClassPrivate *)(classInfo->userData))->hasFinalizer)
		{
			ILGCRegisterFinalizer(ptr, _ILFinalizeObject, thread->process->finalizationContext);
		}

		/* Return a pointer to the object */
		return obj;
	}
}

#ifdef	IL_USE_TYPED_ALLOCATION
ILObject *_ILEngineAllocTyped(ILExecThread *thread, ILClass *classInfo)
{
	void *ptr;
	ILObject *obj;
	ILClassPrivate *classPrivate;

	classInfo = ILClassResolve(classInfo);

	/* Make sure the class has been initialized before we start */
	if(!InitializeClass(thread, classInfo))
	{
		return 0;
	}

	classPrivate = (ILClassPrivate *)(classInfo->userData);

#ifdef IL_CONFIG_USE_THIN_LOCKS
	/* Allocate memory from the heap */
	if(classPrivate->gcTypeDescriptor)
	{
		ptr = ILGCAllocExplicitlyTyped(classPrivate->size + IL_OBJECT_HEADER_SIZE,
									   classPrivate->gcTypeDescriptor);
	}
	else
	{
		/* In case we use thin locks we don't have a type descriptor for */
		/* classes not containing any managed fields. */
		ptr = ILGCAllocAtomic(classPrivate->size + IL_OBJECT_HEADER_SIZE);
	}
#else
	ptr = ILGCAllocExplicitlyTyped(classPrivate->size + IL_OBJECT_HEADER_SIZE,
								   classPrivate->gcTypeDescriptor);
#endif
	if(!ptr)
	{
		/* Throw an "OutOfMemoryException" */
		_ILExecThreadSetOutOfMemoryException(thread);
		return 0;
	}
		
	obj = GetObjectFromGcBase(ptr);

	SetObjectClassPrivate(obj, classPrivate);
		
	/* Attach a finalizer to the object if the class has
	   a non-trival finalizer method attached to it */
	if(classPrivate->hasFinalizer)
	{
		ILGCRegisterFinalizer(ptr, _ILFinalizeObject, thread->process->finalizationContext);
	}

	/* Return a pointer to the object */
	return obj;
}
#endif	/* IL_USE_TYPED_ALLOCATION */

ILObject *_ILEngineAllocObject(ILExecThread *thread, ILClass *classInfo)
{
#ifdef	IL_USE_TYPED_ALLOCATION
	return _ILEngineAllocTyped(thread, classInfo);
#else	/* !IL_USE_TYPED_ALLOCATION */
	classInfo = ILClassResolve(classInfo);

	if(!InitializeClass(thread, classInfo))
	{
		return 0;
	}
	if(((ILClassPrivate *)(classInfo->userData))->managedInstance)
	{
		return _ILEngineAlloc
				(thread, classInfo,
				 ((ILClassPrivate *)(classInfo->userData))->size);
	}
	else
	{
		/* There are no managed fields, so use atomic allocation */
		return _ILEngineAllocAtomic
				(thread, classInfo,
				 ((ILClassPrivate *)(classInfo->userData))->size);
	}
#endif	/* !IL_USE_TYPED_ALLOCATION */
}

#ifdef	__cplusplus
};
#endif
