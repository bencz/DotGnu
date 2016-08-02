/*
 * jitc_alloc.c - Jit coder memory management routines.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#ifdef	IL_JITC_DECLARATIONS

/*
 * Generate the code to allocate the memory for an object with the given size.
 * Returns the ILJitValue with the pointer to the new object.
 */
static ILJitValue _ILJitAllocGen(ILJitFunction jitFunction,
								 ILClass *classInfo,
								 ILUInt32 size);

/*
 * Generate the code to allocate the memory for an object.
 * Returns the ILJitValue with the pointer to the new object.
 */
static ILJitValue _ILJitAllocObjectGen(ILJitFunction jitFunction,
									   ILClass *classInfo);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Allocate memory for an object that contains object references.
 */
static ILObject *_ILJitAlloc(ILClass *classInfo, ILUInt32 size)
{
	ILClassPrivate *classPrivate = (ILClassPrivate *)(classInfo->userData);
	void *ptr;
	ILObject *obj;
	
	/* Allocate memory from the heap */
	ptr = ILGCAlloc(size + IL_OBJECT_HEADER_SIZE);

	if(!ptr)
	{
		/* Throw an "OutOfMemoryException" */
		ILRuntimeExceptionThrowOutOfMemory();
	}

	obj = GetObjectFromGcBase(ptr);

	/* Set the class into the block */
	SetObjectClassPrivate(obj, classPrivate);

	/* Attach a finalizer to the object if the class has
	a non-trival finalizer method attached to it */
	if(classPrivate->hasFinalizer)
	{
		ILGCRegisterFinalizer(ptr, _ILFinalizeObject,
							  classPrivate->process->finalizationContext);
	}

	/* Return a pointer to the object */
	return obj;
}

/*
 * Allocate memory for an object that does not contain any object references.
 */
static ILObject *_ILJitAllocAtomic(ILClass *classInfo, ILUInt32 size)
{
	ILClassPrivate *classPrivate = (ILClassPrivate *)(classInfo->userData);
	void *ptr;
	ILObject *obj;

	/* Allocate memory from the heap */
#ifdef IL_CONFIG_USE_THIN_LOCKS
	ptr = ILGCAllocAtomic(size + IL_OBJECT_HEADER_SIZE);
#else
	/* We need this because we have to make sure the ILLockWord in the ObjectHeader is scanned by the GC. */
	/* TODO: Move descriptor creation to layout.c */
	if(classPrivate->gcTypeDescriptor == IL_MAX_NATIVE_UINT)
	{
		ILNativeUInt bitmap = IL_OBJECT_HEADER_PTR_MAP;

		classPrivate->gcTypeDescriptor = ILGCCreateTypeDescriptor(&bitmap, IL_OBJECT_HEADER_SIZE / sizeof(ILNativeInt));		
	}

	ptr = ILGCAllocExplicitlyTyped(size + IL_OBJECT_HEADER_SIZE, classPrivate->gcTypeDescriptor);
#endif

	if(!ptr)
	{
		/* Throw an "OutOfMemoryException" */
		ILRuntimeExceptionThrowOutOfMemory();
	}

	obj = GetObjectFromGcBase(ptr);

	/* Set the class into the block */
	SetObjectClassPrivate(obj, classPrivate);

	/* Attach a finalizer to the object if the class has
	a non-trival finalizer method attached to it */
	if(classPrivate->hasFinalizer)
	{
		ILGCRegisterFinalizer(ptr, _ILFinalizeObject,
							  classPrivate->process->finalizationContext);
	}

	/* Return a pointer to the object */
	return obj;
}

#ifdef	IL_USE_TYPED_ALLOCATION
/*
 * Allocate memory for an object that contains object references.
 */
static ILObject *_ILJitAllocTyped(ILClass *classInfo)
{
	ILClassPrivate *classPrivate = (ILClassPrivate *)(classInfo->userData);
	void *ptr;
	ILObject *obj;
	
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
#else	/* !IL_CONFIG_USE_THIN_LOCKS */
	ptr = ILGCAllocExplicitlyTyped(classPrivate->size + IL_OBJECT_HEADER_SIZE,
								   classPrivate->gcTypeDescriptor);
#endif	/* !IL_CONFIG_USE_THIN_LOCKS */

	if(!ptr)
	{
		/* Throw an "OutOfMemoryException" */
		ILRuntimeExceptionThrowOutOfMemory();
	}

	obj = GetObjectFromGcBase(ptr);

	/* Set the class into the block */
	SetObjectClassPrivate(obj, classPrivate);

	/* Attach a finalizer to the object if the class has
	a non-trival finalizer method attached to it */
	if(classPrivate->hasFinalizer)
	{
		ILGCRegisterFinalizer(ptr, _ILFinalizeObject,
							  classPrivate->process->finalizationContext);
	}

	/* Return a pointer to the object */
	return obj;
}
#endif	/* IL_USE_TYPED_ALLOCATION */

/*
 * Generate the code to allocate the memory for an object with the given size.
 * Returns the ILJitValue with the pointer to the new object.
 */
static ILJitValue _ILJitAllocGen(ILJitFunction jitFunction,
								 ILClass *classInfo,
								 ILUInt32 size)
{
	ILJitValue newObj;
	ILJitValue args[2];

	/* Make sure the class has been layouted. */
	if(!(classInfo->userData) || (((ILClassPrivate *)(classInfo->userData))->inLayout))
	{
		if(!_LayoutClass(ILExecThreadCurrent(), classInfo))
		{
			return (ILJitValue)0;
		}
	}
	/* We call the alloc functions. */
	/* They thow an out of memory exception so we don't need to care. */
	args[0] = jit_value_create_nint_constant(jitFunction,
											 _IL_JIT_TYPE_VPTR,
											 (jit_nint)classInfo);
	args[1] = jit_value_create_nint_constant(jitFunction,
											 _IL_JIT_TYPE_UINT32, size);

	if(((ILClassPrivate *)(classInfo->userData))->managedInstance)
	{
		newObj = jit_insn_call_native(jitFunction, "_ILJitAlloc",
									  _ILJitAlloc,
									  _ILJitSignature_ILJitAlloc,
				 					  args, 2, 0);
	}
	else
	{
		newObj = jit_insn_call_native(jitFunction,
									  "_ILJitAllocAtomic",
									  _ILJitAllocAtomic,
									  _ILJitSignature_ILJitAlloc,
				 					  args, 2, 0);
	}
	return newObj;
}

/*
 * Generate the code to allocate the memory for an object.
 * Returns the ILJitValue with the pointer to the new object.
 */
static ILJitValue _ILJitAllocObjectGen(ILJitFunction jitFunction,
									   ILClass *classInfo)
{
#ifdef	IL_USE_TYPED_ALLOCATION
	ILJitValue args[1];
#endif

	/* Make sure the class has been layouted. */
	if(!(classInfo->userData) || 
	   (((ILClassPrivate *)(classInfo->userData))->inLayout))
	{
		if(!_LayoutClass(ILExecThreadCurrent(), classInfo))
		{
			return (ILJitValue)0;
		}
	}

#ifdef	IL_USE_TYPED_ALLOCATION
	/* We call the alloc function. */
	/* They thow an out of memory exception so we don't need to care. */
	args[0] = jit_value_create_nint_constant(jitFunction,
											 _IL_JIT_TYPE_VPTR,
											 (jit_nint)classInfo);
	return jit_insn_call_native(jitFunction,
								"_ILJitAllocTyped",
								_ILJitAllocTyped,
								_ILJitSignature_ILJitAllocTyped,
				 				args, 1, 0);
#else	/* !IL_USE_TYPED_ALLOCATION */
	return _ILJitAllocGen(jitFunction, classInfo,
						  ((ILClassPrivate *)(classInfo->userData))->size);
#endif	/* !IL_USE_TYPED_ALLOCATION */
}

#endif	/* IL_JITC_FUNCTIONS */

