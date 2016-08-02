/*
 * jitc_array.c - Jit coder array handling routines.
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
 * The simple array header size.
 */
#define _IL_JIT_SARRAY_HEADERSIZE	sizeof(System_Array)

/*
 * Check if the elementtype e needs to be scanned by the GC.
 *
 * Use the following conservative check if you encounter any problems with
 * objects prematurely collected.
 *
 * 		((ILType_IsPrimitive(e) &&
 *		   (e) != ILType_TypedRef))
 */
#define _IL_JIT_ARRAY_TYPE_NEEDS_GC(e, c) \
	(!(ILType_IsPrimitive((e)) && ((e) != ILType_TypedRef)) || \
	 (ILTypeIsReference(e)) || \
	 (ILType_IsValueType((e)) && (c)->managedInstance))

/*
 * Validate the array index.
 */
#define JITC_START_CHECK_ARRAY_INDEX(jitCoder, length, index) \
	{ \
		jit_label_t __label = jit_label_undefined; \
		jit_label_t __okLabel = jit_label_undefined; \
		ILJitValue  __length = (length); \
		ILJitValue  __index = (index); \
		ILJitValue  __temp; \
		\
		AdjustSign(jitCoder->jitFunction, &(__length), 1, 0); \
		AdjustSign(jitCoder->jitFunction, &(__index), 1, 0); \
		\
		__temp = jit_insn_lt(jitCoder->jitFunction, __index, __length); \
		jit_insn_branch_if_not(jitCoder->jitFunction, __temp, &__label);

#define JITC_END_CHECK_ARRAY_INDEX(jitCoder) \
		jit_insn_branch(jitCoder->jitFunction, &__okLabel); \
		jit_insn_label(jitCoder->jitFunction, &__label); \
		_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_INDEX_OUT_OF_RANGE); \
		jit_insn_label(jitCoder->jitFunction, &__okLabel); \
	}

/*
 * Get the base pointer to the array data of a simple array.
 */
static ILJitValue _ILJitSArrayGetBase(ILJitFunction jitFunction,
									  ILJitValue array);

/*
 * Get the array length of a simple array.
 */
static ILJitValue _ILJitSArrayGetLength(ILJitFunction jitFunction,
										ILJitValue array);

/*
 * Create a simple Object array with the given size.
 */
static ILJitValue _ILJitSObjectArrayCreate(ILJitFunction jitFunction,
										   ILExecThread *_thread,
										   ILJitValue thread,
										   ILUInt32 numElements);

/*
 * Inline function to get an array element at the specified indexes from a
 * complex array.
 */
static int _ILJitMArrayGet(ILJITCoder *jitCoder,
						   ILMethod *method,
						   ILCoderMethodInfo *methodInfo,
						   ILJitStackItem *args,
						   ILInt32 numArgs);

/*
 * Inline function to get the address of an array element at the specified
 * indexes in a complex array.
 */
static int _ILJitMArrayAddress(ILJITCoder *jitCoder,
									  ILMethod *method,
									  ILCoderMethodInfo *methodInfo,
									  ILJitStackItem *args,
									  ILInt32 numArgs);

/*
 * Inline function to set an array element at the specified indexes in a
 * complex array.
 */
static int _ILJitMArraySet(ILJITCoder *jitCoder,
						   ILMethod *method,
						   ILCoderMethodInfo *methodInfo,
						   ILJitStackItem *args,
						   ILInt32 numArgs);

#endif	/* IL_JITC_DECLARATIONS */

#ifdef	IL_JITC_FUNCTIONS

/*
 * Allocate a simple array which elements comntain object references.
 */
static System_Array *_ILJitSArrayAlloc(ILClass *arrayClass,
									   ILUInt32 numElements,
									   ILUInt32 elementSize)
{
	ILUInt64 totalSize;
	System_Array *ptr;

	totalSize = (((ILUInt64)elementSize) * ((ILUInt64)numElements)) + _IL_JIT_SARRAY_HEADERSIZE;
	if(totalSize > (ILUInt64)IL_MAX_INT32)
	{
		/* The array is too large. */
		/* Throw an "OutOfMemoryException" */
		ILRuntimeExceptionThrowOutOfMemory();
		return (System_Array *)0;
	}
	ptr = (System_Array *)_ILJitAlloc(arrayClass, (ILUInt32)totalSize);
	ArrayLength(ptr) = numElements;
	return ptr;
}

/*
 * Allocate a simple array which elements do not comntain object references.
 */
static System_Array *_ILJitSArrayAllocAtomic(ILClass *arrayClass,
											 ILUInt32 numElements,
											 ILUInt32 elementSize)
{
	ILUInt64 totalSize;
	System_Array *ptr;

	totalSize = (((ILUInt64)elementSize) * ((ILUInt64)numElements)) + _IL_JIT_SARRAY_HEADERSIZE;
	if(totalSize > (ILUInt64)IL_MAX_INT32)
	{
		/* The array is too large. */
		/* Throw an "OutOfMemoryException" */
		ILRuntimeExceptionThrowOutOfMemory();
		return (System_Array *)0;
	}
	ptr = (System_Array *)_ILJitAllocAtomic(arrayClass, (ILUInt32)totalSize);
	ArrayLength(ptr) = numElements;
	return ptr;
}

/*
 * Create a new simple array (1 dimension and zero based) with a constant size.
 */
static ILJitValue _ILJitSArrayNewWithConstantSize(ILJitFunction jitFunction,
												  ILExecThread *thread,
												  ILClass *arrayClass,
												  ILUInt32 length)
{
	/* The array element type. */
	ILType *elementType;
	/* The size of one array element. */
	ILUInt32 elementSize;
	ILUInt64 totalSize;
	/* Number of elements in the array. */
	ILJitValue arrayLength;
	/* Total amount of memory needed for the array. */
	ILJitValue arraySize;
	ILJitValue newArray;
	ILClass *elementClass;
	ILClassPrivate *classPrivate;
	ILJitValue args[2];

	/* Make sure the synthetic array class is layouted. */
	if(!(arrayClass->userData))
	{
		/* We have to layout the class first. */
		if(!_LayoutClass(thread, arrayClass))
		{
			printf("Failed to layout class: %s\n", ILClass_Name(arrayClass));
			return (ILJitValue)0;
		}
	}

	elementType = ILType_ElemType(ILClassGetSynType(arrayClass));

	elementClass = ILClassFromType(ILProgramItem_Image(arrayClass),
								   0, elementType, 0);
	elementClass = ILClassResolve(elementClass);
	classPrivate = (ILClassPrivate *)(elementClass->userData);

	if(!classPrivate)
	{
		if(!_LayoutClass(thread, elementClass))
		{
			printf("Failed to layout class: %s\n", ILClass_Name(elementClass));
			return (ILJitValue)0;
		}
		classPrivate = (ILClassPrivate *)(elementClass->userData);
	}

	/* Get the size of one array element. */
	elementSize = _ILSizeOfTypeLocked(_ILExecThreadProcess(thread), elementType);

	totalSize = (((ILUInt64)elementSize) * ((ILUInt64)length)) + _IL_JIT_SARRAY_HEADERSIZE;
	if(totalSize > (ILUInt64)IL_MAX_INT32)
	{
		/* The array is too large. */
		ILJitValue array = jit_value_create_nint_constant(jitFunction,
														  _IL_JIT_TYPE_VPTR, 0);

		_ILJitThrowSystem(jitFunction, _IL_JIT_OUT_OF_MEMORY);
		return array;
	}
	arraySize = jit_value_create_nint_constant(jitFunction,
											   _IL_JIT_TYPE_UINT32,
											   (ILUInt32)totalSize);

	arrayLength = jit_value_create_nint_constant(jitFunction,
												_IL_JIT_TYPE_UINT32,
												length);

	/* We call the alloc functions. */
	/* They thow an out of memory exception so we don't need to care. */
	args[0] = jit_value_create_nint_constant(jitFunction,
											 _IL_JIT_TYPE_VPTR,
											 (jit_nint)arrayClass);
	args[1] = arraySize;

	if(!_IL_JIT_ARRAY_TYPE_NEEDS_GC(elementType, classPrivate))
	{
		newArray = jit_insn_call_native(jitFunction,
										"_ILJitAllocAtomic",
										_ILJitAllocAtomic,
										_ILJitSignature_ILJitAlloc,
			 							args, 2, 0);
	}
	else
	{
		/* The element contains object references. */
		newArray = jit_insn_call_native(jitFunction,
										"_ILJitAlloc",
										_ILJitAlloc,
										_ILJitSignature_ILJitAlloc,
			 							args, 2, 0);
	}
	/* Set the length in the array. */
	jit_insn_store_relative(jitFunction,
							newArray, 
							offsetof(SArrayHeader, length),
							arrayLength);
	return newArray;
}

/*
 * Create a new simple array (1 dimension and zero based) 
 */
static ILJitValue _ILJitSArrayNew(ILJITCoder *jitCoder, ILClass *arrayClass, ILJitValue length)
{
	ILExecThread *thread = ILExecThreadCurrent();

	if(jit_value_is_constant(length))
	{
		ILUInt32 numElements = jit_value_get_nint_constant(length);

		return _ILJitSArrayNewWithConstantSize(jitCoder->jitFunction,
											   thread,
											   arrayClass,
											   numElements);
	}
	else
	{
		/* The array element type. */
		ILType *elementType;
		/* The size of one array element. */
		ILUInt32 elementSize;
		/* Total amount of memory needed for the array. */
		ILJitValue newArray;
		ILClass *elementClass;
		ILClassPrivate *classPrivate;
		ILJitValue args[3];

		/* Make sure the synthetic array class is layouted. */
		if(!(arrayClass->userData))
		{
			/* We have to layout the class first. */
			if(!_LayoutClass(thread, arrayClass))
			{
				printf("Failed to layout class: %s\n", ILClass_Name(arrayClass));
				return (ILJitValue)0;
			}
		}

		elementType = ILType_ElemType(ILClassGetSynType(arrayClass));

		/* Get the size of one array element. */
		elementSize = _ILSizeOfTypeLocked(jitCoder->process, elementType);

		elementClass = ILClassFromType(ILProgramItem_Image(arrayClass),
									   0, elementType, 0);
		elementClass = ILClassResolve(elementClass);
		classPrivate = (ILClassPrivate *)(elementClass->userData);

		if(!classPrivate)
		{
			if(!_LayoutClass(thread, elementClass))
			{
				printf("Failed to layout class: %s\n", ILClass_Name(elementClass));
				return (ILJitValue)0;
			}
			classPrivate = (ILClassPrivate *)(elementClass->userData);
		}

	#if !defined(IL_CONFIG_REDUCE_CODE) && !defined(IL_WITHOUT_TOOLS) && defined(_IL_JIT_ENABLE_DEBUG)
		if(jitCoder->flags & IL_CODER_FLAG_STATS)
		{
			ILMutexLock(globalTraceMutex);
			fprintf(stdout, "New Array of type: %s\n", ILClass_Name(elementClass));
			ILMutexUnlock(globalTraceMutex);
		}
	#endif

		/* We call the alloc functions. */
		/* They thow an out of memory exception so we don't need to care. */
		args[0] = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_VPTR,
												 (jit_nint)arrayClass);
		args[1] = _ILJitValueConvertImplicit(jitCoder->jitFunction,
											 length,
											 _IL_JIT_TYPE_UINT32);
		args[2] = jit_value_create_nint_constant(jitCoder->jitFunction,
												 _IL_JIT_TYPE_UINT32,
												 elementSize);

		if(!_IL_JIT_ARRAY_TYPE_NEEDS_GC(elementType, classPrivate))
		{
			newArray = jit_insn_call_native(jitCoder->jitFunction,
											"_ILJitSArrayAllocAtomic",
											_ILJitSArrayAllocAtomic,
											_ILJitSignature_ILJitSArrayAlloc,
				 							args, 3, 0);
		}
		else
		{
			/* The element contains object references. */
			newArray = jit_insn_call_native(jitCoder->jitFunction,
											"_ILJitSArrayAlloc",
											_ILJitSArrayAlloc,
											_ILJitSignature_ILJitSArrayAlloc,
				 							args, 3, 0);
		}
		return newArray;
	}
}

/*
 * Create an Object array with the given size.
 */
static ILJitValue _ILJitSObjectArrayCreate(ILJitFunction jitFunction,
										   ILExecThread *_thread,
										   ILJitValue thread,
										   ILUInt32 numElements)
{
	ILClass *objectClass = _ILExecThreadProcess(_thread)->objectClass;
	ILType *objectType = ILType_FromClass(objectClass);
	ILType *arrayType = ILTypeFindOrCreateArray(_ILExecThreadProcess(_thread)->context,
												1, objectType);
	ILClass *arrayClass;

	/* Find the object array class. */
	if(!arrayType)
	{
		return 0;
	}
	arrayClass = ILClassFromType(ILContextNextImage(_ILExecThreadProcess(_thread)->context, 0),
								 0, arrayType, 0);
	if(!arrayClass)
	{
		return 0;
	}

	arrayClass = ILClassResolve(arrayClass);
	if(!arrayClass)
	{
		return 0;
	}

	return _ILJitSArrayNewWithConstantSize(jitFunction,
										   _thread,
										   arrayClass,
										   numElements);
}

/*
 * Calculate the absolute Index of an element in the array.
 * Throws an IndexOutOfRangeException if any of the indexes is out of bounds.
 */
static ILJitValue _ILJitMArrayCalcIndex(ILJITCoder *jitCoder,
										ILJitValue array,
										ILJitStackItem *index,
										ILInt32 rank)
{
	jit_label_t arrayOutOfBoundsLabel = jit_label_undefined;
	jit_label_t okLabel = jit_label_undefined;
	ILJitValue absoluteIndex = jit_value_create(jitCoder->jitFunction,
												_IL_JIT_TYPE_INT32);
	ILInt32 currentIndex = 0;

	for(currentIndex = 0; currentIndex < rank; currentIndex++)
	{
		ILJitValue temp;
		ILJitValue multiplier;
		ILJitValue pos = _ILJitStackItemValue(index[currentIndex]);
		ILJitValue base = jit_insn_load_relative(jitCoder->jitFunction, array,
												 offsetof(System_MArray, bounds) +
												 (sizeof(MArrayBounds) * currentIndex) +
												 offsetof(MArrayBounds, lower),
												 _IL_JIT_TYPE_UINT32);
		ILJitValue length = jit_insn_load_relative(jitCoder->jitFunction, array,
												 offsetof(System_MArray, bounds) +
												 (sizeof(MArrayBounds) * currentIndex) +
												 offsetof(MArrayBounds, size),
												 _IL_JIT_TYPE_UINT32);

		/* calculate the real index for this dimension. */
		pos = jit_insn_sub(jitCoder->jitFunction, pos, base);

		/* Make pos unsigned. We can save one compare this way. */
		temp = pos;
		AdjustSign(jitCoder->jitFunction, &temp, 1, 0);
		temp = jit_insn_lt(jitCoder->jitFunction, temp, length);
		jit_insn_branch_if_not(jitCoder->jitFunction, temp, &arrayOutOfBoundsLabel);

		/* the multilplier is 1 for the last dimension so we don't have to */
		/* use it then.*/
		if(currentIndex < (rank -1))
		{
			multiplier = jit_insn_load_relative(jitCoder->jitFunction, array,
												offsetof(System_MArray, bounds) +
												(sizeof(MArrayBounds) * currentIndex) +
												offsetof(MArrayBounds, multiplier),
												_IL_JIT_TYPE_INT32);
			pos = jit_insn_mul(jitCoder->jitFunction, pos, multiplier);
		}
		if(currentIndex == 0)
		{
			jit_insn_store(jitCoder->jitFunction, absoluteIndex, pos);
		}
		else
		{
			pos = jit_insn_add(jitCoder->jitFunction, absoluteIndex, pos);
			jit_insn_store(jitCoder->jitFunction, absoluteIndex, pos);
		}
	}
	jit_insn_branch(jitCoder->jitFunction, &okLabel);
	jit_insn_label(jitCoder->jitFunction, &arrayOutOfBoundsLabel);
	/* Throw a System.IndexOutOfRange exception. */
	_ILJitThrowSystem(jitCoder->jitFunction, _IL_JIT_INDEX_OUT_OF_RANGE);
	jit_insn_label(jitCoder->jitFunction, &okLabel);

	return absoluteIndex;
}

/*
 * Get the pointer to the start of the data in a MArray.
 */
static ILJitValue _ILJitMarrayGetBase(ILJITCoder *jitCoder,
									  ILJitValue array)
{
	ILJitValue data = jit_insn_load_relative(jitCoder->jitFunction, array,
											 offsetof(System_MArray, data),
											 _IL_JIT_TYPE_VPTR);
	
	return data;
}

/*
 * Get an array element at the specified indexes from a complex array.
 */
static int _ILJitMArrayGet(ILJITCoder *jitCoder,
								  ILMethod *method,
								  ILCoderMethodInfo *methodInfo,
								  ILJitStackItem *args,
								  ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitValue array = _ILJitStackItemValue(args[0]);
	ILJitValue arrayBase;
	ILJitValue returnValue;
	ILJitType signature;
	ILJitType arrayType;
	ILJitValue index;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}

	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	arrayType = jit_type_get_return(signature);
	index = _ILJitMArrayCalcIndex(jitCoder, array, &(args[1]), numArgs - 1);
	arrayBase = _ILJitMarrayGetBase(jitCoder, array);
	returnValue = jit_insn_load_elem(jitCoder->jitFunction, arrayBase, index, arrayType);
	_ILJitStackPushValue(jitCoder, returnValue);
	return 1;
}

/*
 * Get the address of an array element at the specified indexes in a complex
 * array.
 */
static int _ILJitMArrayAddress(ILJITCoder *jitCoder,
							   ILMethod *method,
							   ILCoderMethodInfo *methodInfo,
							   ILJitStackItem *args,
							   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitValue array = _ILJitStackItemValue(args[0]);
	ILClass *arrayClass = ILMethod_Owner(method);
	ILType *elementType = ILTypeGetElemType(ILClassToType(arrayClass));
	ILJitValue arrayBase;
	ILJitType signature;
	ILJitType arrayType;
	ILJitValue index;
	ILJitValue returnValue;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}

	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
	arrayType = _ILJitGetReturnType(elementType, 
									((ILClassPrivate *)arrayClass->userData)->process);
	index = _ILJitMArrayCalcIndex(jitCoder, array, &(args[1]), numArgs - 1);
	arrayBase = _ILJitMarrayGetBase(jitCoder, array);
	returnValue = jit_insn_load_elem_address(jitCoder->jitFunction,
											 arrayBase,
											 index,
											 arrayType);
	_ILJitStackPushNotNullValue(jitCoder, returnValue);
	return 1;
}

/*
 * Set an array element at the specified indexes in a complex array.
 */
static int _ILJitMArraySet(ILJITCoder *jitCoder,
						   ILMethod *method,
						   ILCoderMethodInfo *methodInfo,
						   ILJitStackItem *args,
						   ILInt32 numArgs)
{
	ILJitFunction jitFunction = ILJitFunctionFromILMethod(method);
	ILJitValue array = _ILJitStackItemValue(args[0]);
	ILJitValue value = _ILJitStackItemValue(args[numArgs - 1]);
	ILJitValue arrayBase;
	ILJitType signature;
	ILJitType arrayType;
	ILJitValue index;

	if(!jitFunction)
	{
		/* We need to layout the class first. */
		if(!_LayoutClass(ILExecThreadCurrent(), ILMethod_Owner(methodInfo)))
		{
			return 0;
		}
		if(!(jitFunction = ILJitFunctionFromILMethod(method)))
		{
			return 0;
		}
	}

	if(!(signature = jit_function_get_signature(jitFunction)))
	{
		return 0;
	}
#ifdef IL_JIT_THREAD_IN_SIGNATURE
	arrayType = jit_type_get_param(signature, numArgs);
#else
	arrayType = jit_type_get_param(signature, numArgs - 1);
#endif
	index = _ILJitMArrayCalcIndex(jitCoder, array, &(args[1]), numArgs - 2);
	arrayBase = _ILJitMarrayGetBase(jitCoder, array);
	value = _ILJitValueConvertImplicit(jitCoder->jitFunction, value, arrayType);
	jit_insn_store_elem(jitCoder->jitFunction, arrayBase, index, value);
	/* We have no return value in this case. */
	return 1;
}

/*
 * Get the base pointer to the array data of a simple array.
 */
static ILJitValue _ILJitSArrayGetBase(ILJitFunction jitFunction,
									  ILJitValue array)
{
	return jit_insn_add_relative(jitFunction, array, sizeof(System_Array));
}

/*
 * Get the array length of a simple array.
 */
static ILJitValue _ILJitSArrayGetLength(ILJitFunction jitFunction,
										ILJitValue array)
{
	ILJitValue len;

	len = jit_insn_load_relative(jitFunction,
								 array,
								 offsetof(SArrayHeader, length),
								 _IL_JIT_TYPE_INT32);
	return len;
}

static void ValidateArrayIndex(ILJITCoder *coder, ILJitValue length,
												  ILJitValue index)
{
	jit_label_t label = jit_label_undefined;
	ILJitValue temp;

	/* Make both values unsigned. We can save one compare this way. */
	AdjustSign(coder->jitFunction, &length, 1, 0);
	AdjustSign(coder->jitFunction, &index, 1, 0);

	temp = jit_insn_lt(coder->jitFunction, index, length);
	jit_insn_branch_if(coder->jitFunction, temp, &label);
	/* Throw a System.IndexOutOfRange exception. */
	_ILJitThrowSystem(coder->jitFunction, _IL_JIT_INDEX_OUT_OF_RANGE);
	jit_insn_label(coder->jitFunction, &label);
}

/*
 * Handle the ldelem* instructions.
 */
static void LoadArrayElem(ILJITCoder *coder, ILJitType type)
{
	_ILJitStackItemNew(index);
	_ILJitStackItemNew(array);
	ILJitValue length;
	ILJitValue value;
	ILJitValue arrayBase;

	_ILJitStackPop(coder, index);
	_ILJitStackPop(coder, array);
	_ILJitStackItemCheckNull(coder, array);
	length = _ILJitSArrayGetLength(coder->jitFunction,
								   _ILJitStackItemValue(array));
	JITC_START_CHECK_ARRAY_INDEX(coder, length, _ILJitStackItemValue(index))
	arrayBase = _ILJitSArrayGetBase(coder->jitFunction,
									_ILJitStackItemValue(array));
	value = jit_insn_load_elem(coder->jitFunction,
							   arrayBase,
							   _ILJitStackItemValue(index),
							   type);
	_ILJitStackPushValue(coder, value);
	JITC_END_CHECK_ARRAY_INDEX(coder)
}

/*
 * Handle the stelem* instructions.
 */
static void StoreArrayElem(ILJITCoder *coder, ILJitType type)
{
	_ILJitStackItemNew(value);
	_ILJitStackItemNew(index);
	_ILJitStackItemNew(array);
	ILJitValue length;
	ILJitValue arrayBase;
	ILJitValue temp;
	ILJitType valueType;

	_ILJitStackPop(coder, value);
	_ILJitStackPop(coder, index);
	_ILJitStackPop(coder, array);
	valueType = jit_value_get_type(_ILJitStackItemValue(value));
	_ILJitStackItemCheckNull(coder, array);
	length = _ILJitSArrayGetLength(coder->jitFunction,
								   _ILJitStackItemValue(array));
	ValidateArrayIndex(coder, length, _ILJitStackItemValue(index));
	arrayBase = _ILJitSArrayGetBase(coder->jitFunction,
									_ILJitStackItemValue(array));

	/* Convert the value to the array type when needed. */
	if(valueType != type)
	{
		int valueIsStruct = (jit_type_is_struct(valueType) || jit_type_is_union(valueType));
		int destIsStruct = (jit_type_is_struct(type) || jit_type_is_union(type));

		if(valueIsStruct || destIsStruct)
		{
			int valueSize = jit_type_get_size(valueType);
			int destSize = jit_type_get_size(type);

			if(destSize == valueSize)
			{
				/* The sizes match so we can safely use store element. */
				temp = _ILJitStackItemValue(value);
			}
			else
			{
				/* We assume that destSize is smaller than valueSize because */
				/* the values have to be assignment compatible. */
				/* But we have to use memcpy instead. */
				_ILJitStackItemNew(dest);
				ILJitValue destPtr = jit_insn_load_elem_address(coder->jitFunction,
																arrayBase,
																_ILJitStackItemValue(index),
																type);
				ILJitValue srcPtr = jit_insn_address_of(coder->jitFunction,
														_ILJitStackItemValue(value));
				ILJitValue size = jit_value_create_nint_constant(coder->jitFunction,
																 _IL_JIT_TYPE_NINT,
																 (jit_nint)destSize);

				_ILJitStackItemInitWithNotNullValue(dest, destPtr);
				_ILJitStackItemMemCpy(coder, dest, srcPtr, size);
				return;
			}
		}
		else
		{
			temp = _ILJitValueConvertImplicit(coder->jitFunction,
											  _ILJitStackItemValue(value),
											  type);
		}
	}
	else
	{
		temp = _ILJitStackItemValue(value);
	}
	jit_insn_store_elem(coder->jitFunction,
						arrayBase,
						_ILJitStackItemValue(index),
						temp);
}

#endif	/* IL_JITC_FUNCTIONS */

#ifdef IL_JITC_CODE

/*
 * Handle an array access opcode.
 */
static void JITCoder_ArrayAccess(ILCoder *coder, int opcode,
								 ILEngineType indexType, ILType *elemType,
								 const ILCoderPrefixInfo *prefixInfo)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(array);
	_ILJitStackItemNew(index);
	ILJitValue len;
	ILJitValue value;
	ILJitValue arrayBase;

	switch(opcode)
	{
		case IL_OP_LDELEMA:
		{
			_ILJitStackPop(jitCoder, index);
			_ILJitStackPop(jitCoder, array);

			_ILJitStackItemCheckNull(jitCoder, array);
			len = _ILJitSArrayGetLength(jitCoder->jitFunction,
										_ILJitStackItemValue(array));
			ValidateArrayIndex(jitCoder, len, _ILJitStackItemValue(index));
			arrayBase = _ILJitSArrayGetBase(jitCoder->jitFunction,
											_ILJitStackItemValue(array));

			ILJitType type = _ILJitGetReturnType(elemType, jitCoder->process);

			value = jit_insn_load_elem_address(jitCoder->jitFunction,
											   arrayBase,
											   _ILJitStackItemValue(index),
											   type);
			_ILJitStackPushNotNullValue(jitCoder, value);
		}
		break;

		case IL_OP_LDELEM_I1:
		{
			/* Load a signed byte from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_SBYTE);
		}
		break;

		case IL_OP_LDELEM_I2:
		{
			/* Load a signed short from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_INT16);
		}
		break;

		case IL_OP_LDELEM_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_LDELEM_I:
	#endif
		{
			/* Load an integer from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_INT32);
		}
		break;

		case IL_OP_LDELEM_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_LDELEM_I:
	#endif
		{
			/* Load a long from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_INT64);
		}
		break;

		case IL_OP_LDELEM_U1:
		{
			/* Load an unsigned byte from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_BYTE);
		}
		break;

		case IL_OP_LDELEM_U2:
		{
			/* Load an unsigned short from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_UINT16);
		}
		break;

		case IL_OP_LDELEM_U4:
		{
			/* Load an unsigned integer from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_UINT32);
		}
		break;

		case IL_OP_LDELEM_R4:
		{
			/* Load a 32-bit float from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_SINGLE);
		}
		break;

		case IL_OP_LDELEM_R8:
		{
			/* Load a 64-bit float from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_DOUBLE);
		}
		break;

		case IL_OP_LDELEM_REF:
		{
			/* Load a pointer from an array */
			LoadArrayElem(jitCoder, _IL_JIT_TYPE_VPTR);
		}
		break;

		case IL_OP_LDELEM:
		{
			ILJitType type = _ILJitGetReturnType(elemType, jitCoder->process);
			LoadArrayElem(jitCoder, type);
		}
		break;

		case IL_OP_STELEM_I1:
		{
			/* Store a byte value to an array */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_SBYTE);
		}
		break;

		case IL_OP_STELEM_I2:
		{
			/* Store a short value to an array */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_INT16);
		}
		break;

		case IL_OP_STELEM_I4:
	#ifdef IL_NATIVE_INT32
		case IL_OP_STELEM_I:
	#endif
		{
			/* Store an integer value to an array */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_INT32);
		}
		break;

		case IL_OP_STELEM_I8:
	#ifdef IL_NATIVE_INT64
		case IL_OP_STELEM_I:
	#endif
		{
			/* Store a long value to an array */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_INT64);
		}
		break;

		case IL_OP_STELEM_R4:
		{
			/* Store a 32-bit floating point value to an array */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_SINGLE);
		}
		break;

		case IL_OP_STELEM_R8:
		{
			/* Store a 64-bit floating point value to an array */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_DOUBLE);
		}
		break;

		case IL_OP_STELEM_REF:
		{
			/* Store a pointer to an array */
			/* TODO: Check if the types are assignmentcompatible. */
			StoreArrayElem(jitCoder, _IL_JIT_TYPE_VPTR);
		}
		break;

		case IL_OP_STELEM:
		{
			/* Store a pointer to an array */
			/* TODO: Check if the types are assignmentcompatible. */
			ILJitType type = _ILJitGetReturnType(elemType, jitCoder->process);
			StoreArrayElem(jitCoder, type);
		}
		break;
	}
}

/*
 * Get the length of an array.
 */
static void JITCoder_ArrayLength(ILCoder *coder)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(array);
	ILJitValue length;

	/* Pop the array off the evaluation stack. */
	_ILJitStackPop(jitCoder, array);

	_ILJitStackItemCheckNull(jitCoder, array);
	length = _ILJitSArrayGetLength(jitCoder->jitFunction,
								   _ILJitStackItemValue(array));
	_ILJitStackPushValue(jitCoder, length);
}

/*
 * Construct a new array, given a type and length value.
 */
static void JITCoder_NewArray(ILCoder *coder, ILType *arrayType,
					 		  ILClass *arrayClass, ILEngineType lengthType)
{
	ILJITCoder *jitCoder = _ILCoderToILJITCoder(coder);
	_ILJitStackItemNew(length);
	ILJitValue returnValue;

	/* Pop the length off the evaluation stack. */
	_ILJitStackPop(jitCoder, length);

	returnValue = _ILJitSArrayNew(jitCoder,
								  arrayClass,
								  _ILJitStackItemValue(length));

	_ILJitStackPushNotNullValue(jitCoder, returnValue);
}

#endif /* IL_JITC_CODE */

