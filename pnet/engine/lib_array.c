/*
 * lib_array.c - Internalcall methods for "System.Array" and subclasses.
 *
 * Copyright (C) 2001, 2008, 2009  Southern Storm Software, Pty Ltd.
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
#include <il_varargs.h>

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_USE_JIT
#define ArgWalker void *
#else
/*
 * Support for walking the argument stack in methods that
 * require a variable number of arguments.  This is CVM
 * specific and will need to be updated if we change the
 * engine architecture in the future.  We have to do this
 * because "libffi" will pack the arguments onto the
 * stack in a way which cannot reliably be extracted
 * with "va_arg" on all platforms.
 */
typedef struct
{
	CVMWord	*posn;

} ArgWalker;
#define	ArgWalkerInit(args)	\
			do { \
				(args)->posn = thread->frame; \
			} while (0)
#define	ArgWalkerInitThis(args)	\
			do { \
				(args)->posn = thread->frame + 1; \
			} while (0)
#define	ArgWalkerGetShortInt(args,type) \
			(*((type *)(((args)->posn)++)))
#define	ArgWalkerGetInt(args) \
			((((args)->posn)++)->intValue)
#define	ArgWalkerGetUInt(args) \
			((((args)->posn)++)->uintValue)
#define	ArgWalkerGetPtr(args) \
			((((args)->posn)++)->ptrValue)
#define	ArgWalkerGetAddr(args) \
			((args)->posn)
#define	ArgWalkerAdvance(args,nwords) \
			((args)->posn += (nwords))
#endif

/*
 * Allocation constructor for single-dimensional arrays.
 *
 * public T[](uint size);
 */
static System_Array *System_SArray_ctor(ILExecThread *thread,
										ILUInt32 length)
{
	ILClass *classInfo;
	ILType *type;
	ILUInt32 elemSize;
	ILUInt64 totalSize;
	System_Array *array;

	/* Get the synthetic class and type for the array */
	classInfo = ILMethod_Owner(thread->method);
	type = ILClassGetSynType(classInfo);

	/* Compute the element size */
	elemSize = ILSizeOfType(thread, ILType_ElemType(type));

	/* Determine the total size of the array in bytes */
	totalSize = ((ILUInt64)elemSize) * ((ILUInt64)length);
	if(totalSize > (ILUInt64)IL_MAX_INT32)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}

	/* Allocate the array, initialize, and return it */
	if(ILType_IsPrimitive(ILType_ElemType(type)) &&
	   ILType_ElemType(type) != ILType_TypedRef)
	{
		/* The array will never contain pointers, so use atomic allocation */
		array = (System_Array *)_ILEngineAllocAtomic
				(thread, classInfo, sizeof(System_Array) + (ILUInt32)totalSize);
	}
	else
	{
		/* The array might contain pointers, so play it safe */
		array = (System_Array *)_ILEngineAlloc
				(thread, classInfo, sizeof(System_Array) + (ILUInt32)totalSize);
	}
	if(array)
	{
		ArrayLength(array) = (ILInt32)length;
	}
	return array;
}

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS

/*
 * Construct the header part of a multi-dimensional array.
 */
static System_MArray *ConstructMArrayHeader(ILExecThread *thread,
											ILClass *classInfo,
											int *elemIsPrimitive)
{
	System_MArray *_this;
	ILType *type;
	ILType *elemType;
	ILInt32 rank;

	/* Get the type descriptor that underlies the class */
	type = ILClassGetSynType(classInfo);

	/* Determine the rank and get the element type */
	rank = 1;
	elemType = type;
	while(elemType != 0 && ILType_IsComplex(type) &&
		  ILType_Kind(elemType) == IL_TYPE_COMPLEX_ARRAY_CONTINUE)
	{
		++rank;
		elemType = ILType_ElemType(elemType);
	}
	if(elemType != 0 && ILType_IsComplex(elemType) &&
	   ILType_Kind(elemType) == IL_TYPE_COMPLEX_ARRAY)
	{
		elemType = ILType_ElemType(elemType);
	}
	else
	{
		/* Shouldn't happen, but do something sane anyway */
		elemType = ILType_Int32;
	}
	*elemIsPrimitive = (ILType_IsPrimitive(elemType) &&
						elemType != ILType_TypedRef);

	/* Allocate space for the array header */
	_this = (System_MArray *)_ILEngineAlloc
					(thread, classInfo,
					 sizeof(System_MArray) +
					 (rank - 1) * sizeof(MArrayBounds));
	if(!_this)
	{
		return 0;
	}

	/* Fill in the array header with the rank and element size values */
	_this->rank = rank;
	_this->elemSize = (ILInt32)ILSizeOfType(thread, elemType);
	return _this;
}

/*
 * Construct the data part of a multi-dimensional array.
 */
static System_MArray *ConstructMArrayData(ILExecThread *thread,
									      System_MArray *array,
										  int elemIsPrimitive)
{
	ILUInt64 sizeInBytes;
	int dim;
	char name[64];

	/* Compute the size of the array in bytes, and also
	   validate the size values to ensure non-negative */
	sizeInBytes = 1;
	for(dim = 0; dim < array->rank; ++dim)
	{
		if(array->bounds[dim].size < 0)
		{
			sprintf(name, "length%d", dim);
			ILExecThreadThrowArgRange(thread, name, "ArgRange_NonNegative");
			return 0;
		}
		sizeInBytes *= (ILUInt64)(array->bounds[dim].size);
		if(sizeInBytes > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
	}
	sizeInBytes *= (ILUInt64)(array->elemSize);
	if(sizeInBytes > (ILUInt64)IL_MAX_INT32)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}

	/* Allocate the data part of the array */
	if(elemIsPrimitive)
	{
		/* The array will never contain pointers, so use atomic allocation */
		array->data = _ILEngineAllocAtomic(thread, (ILClass *)0,
									 	   (ILUInt32)sizeInBytes);
	}
	else
	{
		/* The array might contain pointers, so play it safe */
		array->data = _ILEngineAlloc(thread, (ILClass *)0,
									 (ILUInt32)sizeInBytes);
	}
	if(!(array->data))
	{
		return 0;
	}

	/* The array is ready to go */
	return array;
}

/*
 * Get the address of a particular array element.
 * Returns NULL if an exception was thrown.
 */
static void *GetElemAddress(ILExecThread *thread,
							System_MArray *_this,
#ifdef IL_USE_JIT
							void *args)
#else
							ArgWalker *args)
#endif
{
	ILInt32 offset;
	ILInt32 dim;
	ILInt32 index;
#ifdef IL_USE_JIT
	IL_VA_LIST va;
	
	/* Copy the incoming "va_list" value */
	ILMemCpy(&va, args, sizeof(IL_VA_LIST));
#endif

	/* Find the offset of the element */
	offset = 0;
	for(dim = 0; dim < _this->rank; ++dim)
	{
	#ifdef IL_USE_JIT
		index = IL_VA_ARG(va, ILVaInt) - _this->bounds[dim].lower;
	#else
		index = ArgWalkerGetInt(args) - _this->bounds[dim].lower;
	#endif
		if(((ILUInt32)index) >= ((ILUInt32)(_this->bounds[dim].size)))
		{
			ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
		#ifdef IL_USE_JIT
			IL_VA_END;
		#endif
			return 0;
		}
		offset += index * _this->bounds[dim].multiplier;
	}

	/* Compute the element address */
	offset *= _this->elemSize;
	return (void *)(((unsigned char *)_this->data) + offset);
}

/*
 * Get the address of a particular array element in a two-dimensional array.
 * Returns NULL if an exception was thrown.
 */
static void *Get2ElemAddress(ILExecThread *thread,
							 System_MArray *_this,
							 ILInt32 index1, ILInt32 index2)
{
	ILInt32 offset;

	/* Find the offset of the element */
	index1 -= _this->bounds[0].lower;
	if(((ILUInt32)index1) >= ((ILUInt32)(_this->bounds[0].size)))
	{
		ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
								"Arg_InvalidArrayIndex");
		return 0;
	}
	offset = index1 * _this->bounds[0].multiplier;
	index2 -= _this->bounds[1].lower;
	if(((ILUInt32)index2) >= ((ILUInt32)(_this->bounds[1].size)))
	{
		ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
								"Arg_InvalidArrayIndex");
		return 0;
	}
	offset += index2;

	/* Compute the element address */
	offset *= _this->elemSize;
	return (void *)(((unsigned char *)_this->data) + offset);
}

/*
 * Set a multi-dimensional array element to a value that
 * appears after a list of indicies.  "valueType" indicates
 * the type of value.
 */
static void SetElement(ILExecThread *thread, System_MArray *_this,
#ifdef IL_USE_JIT
					   int valueType, void *args)
#else
					   int valueType, ArgWalker *args)
#endif
{
	ILInt32 offset;
	ILInt32 dim;
	ILInt32 index;
	void *address;
#ifdef IL_USE_JIT
	IL_VA_LIST va;
	
	/* Copy the incoming "va_list" value */
	ILMemCpy(&va, args, sizeof(IL_VA_LIST));
#endif

	/* Find the offset of the element */
	offset = 0;
	for(dim = 0; dim < _this->rank; ++dim)
	{
	#ifdef IL_USE_JIT
		index = IL_VA_ARG(va, ILVaInt) - _this->bounds[dim].lower;
	#else
		index = ArgWalkerGetInt(args) - _this->bounds[dim].lower;
	#endif
		if(((ILUInt32)index) >= ((ILUInt32)(_this->bounds[dim].size)))
		{
			ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return;
		}
		offset += index * _this->bounds[dim].multiplier;
	}

	/* Compute the element address */
	offset *= _this->elemSize;
	address = (void *)(((unsigned char *)_this->data) + offset);

	/* Extract the value from the parameters and set it.  Values
	   greater than 4 bytes in length are copied using "ILMemCpy"
	   because we cannot guarantee their alignment on the stack */
	switch(valueType)
	{
		case IL_META_ELEMTYPE_I1:
		{
		#ifdef IL_USE_JIT
			*((ILInt8 *)address) = IL_VA_ARG(va, ILVaInt);
		#else
			*((ILInt8 *)address) = ArgWalkerGetShortInt(args, ILInt8);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_I2:
		{
		#ifdef IL_USE_JIT
			*((ILInt16 *)address) = IL_VA_ARG(va, ILVaInt);
		#else
			*((ILInt16 *)address) = ArgWalkerGetShortInt(args, ILInt16);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_I4:
		{
		#ifdef IL_USE_JIT
			*((ILInt32 *)address) = IL_VA_ARG(va, ILVaInt);
		#else
			*((ILInt32 *)address) = ArgWalkerGetInt(args);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_I8:
		{
		#ifdef IL_USE_JIT
			ILInt64 longValue = IL_VA_ARG(va, ILInt64);
			ILMemCpy(address, &longValue, sizeof(ILInt64));
		#else
			ILMemCpy(address, ArgWalkerGetAddr(args), sizeof(ILInt64));
			ArgWalkerAdvance(args, CVM_WORDS_PER_LONG);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_R4:
		{
		#ifdef IL_USE_JIT
			ILFloat floatValue = IL_VA_ARG(va, ILDouble);
			ILMemCpy(address, &floatValue, sizeof(ILFloat));
		#else
			ILMemCpy(address, ArgWalkerGetAddr(args), sizeof(ILFloat));
			ArgWalkerAdvance(args, CVM_WORDS_PER_NATIVE_FLOAT);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_R8:
		{
		#ifdef IL_USE_JIT
			ILDouble floatValue = IL_VA_ARG(va, ILVaDouble);
			ILMemCpy(address, &floatValue, sizeof(ILDouble));
		#else
			ILMemCpy(address, ArgWalkerGetAddr(args), sizeof(ILDouble));
			ArgWalkerAdvance(args, CVM_WORDS_PER_NATIVE_FLOAT);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_R:
		{
		#ifdef IL_USE_JIT
			ILNativeFloat floatValue = IL_VA_ARG(va, ILNativeFloat);
			ILMemCpy(address, &floatValue, sizeof(ILNativeFloat));
		#else
			ILMemCpy(address, ArgWalkerGetAddr(args), sizeof(ILNativeFloat));
			ArgWalkerAdvance(args, CVM_WORDS_PER_NATIVE_FLOAT);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_OBJECT:
		{
		#ifdef IL_USE_JIT
			*((ILObject **)address) = (ILObject *)IL_VA_ARG(va, void *);
		#else
			*((ILObject **)address) = (ILObject *)ArgWalkerGetPtr(args);
		#endif
		}
		break;

		case IL_META_ELEMTYPE_VALUETYPE:
		{
		#ifdef IL_USE_JIT
			void *ptrValue = IL_VA_ARG(va, void *);
			ILMemCpy(address, ptrValue, _this->elemSize);
		#else
			ILMemCpy(address, ArgWalkerGetAddr(args), _this->elemSize);
		#endif
		}
		break;
	}
}

/*
 * Constructor for multi-dimensional arrays that takes sizes.
 *
 * public T[,,,](int size1, int size2, ..., int sizeN)
 */
#ifdef IL_USE_JIT
static System_MArray *System_MArray_ctor_1(ILExecThread *thread, ...)
#else
static System_MArray *System_MArray_ctor_1(ILExecThread *thread)
#endif
{
	System_MArray *_this;
	ILInt32 dim;
	ILInt32 multiplier;
#ifdef IL_USE_JIT
	IL_VA_START(thread);
#else
	ArgWalker args;
#endif
	int elemIsPrimitive;

	/* Construct the header part of the array */
	_this = ConstructMArrayHeader(thread, ILMethod_Owner(thread->method),
								  &elemIsPrimitive);
	if(!_this)
	{
		return 0;
	}

	/* Fill in the array header with the size values */
#ifndef IL_USE_JIT
	ArgWalkerInit(&args);
#endif
	for(dim = 0; dim < _this->rank; ++dim)
	{
		_this->bounds[dim].lower = 0;
#ifdef IL_USE_JIT
		_this->bounds[dim].size  = IL_VA_ARG(va, ILVaInt);
#else
		_this->bounds[dim].size  = ArgWalkerGetInt(&args);
#endif
	}
#ifdef IL_USE_JIT
	IL_VA_END;
#endif

	/* Fill in the array header with the multiplier values */
	multiplier = 1;
	for(dim = _this->rank - 1; dim >= 0; --dim)
	{
		_this->bounds[dim].multiplier = multiplier;
		multiplier *= _this->bounds[dim].size;
	}

	/* Construct the data part of the array */
	return ConstructMArrayData(thread, _this, elemIsPrimitive);
}

/*
 * Constructor for multi-dimensional arrays that takes lower bounds and sizes.
 *
 * public T[,,,](int low1, int size1, ..., int lowN, int sizeN)
 */
#ifdef IL_USE_JIT
static System_MArray *System_MArray_ctor_2(ILExecThread *thread, ...)
#else
static System_MArray *System_MArray_ctor_2(ILExecThread *thread)
#endif
{
	System_MArray *_this;
	ILInt32 dim;
	ILInt32 multiplier;
#ifdef IL_USE_JIT
	IL_VA_START(thread);
#else
	ArgWalker args;
#endif
	int elemIsPrimitive;

	/* Construct the header part of the array */
	_this = ConstructMArrayHeader(thread, ILMethod_Owner(thread->method),
								  &elemIsPrimitive);
	if(!_this)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}

	/* Fill in the array header with the lower bound and size values */
#ifndef IL_USE_JIT
	ArgWalkerInit(&args);
#endif
	for(dim = 0; dim < _this->rank; ++dim)
	{
	#ifdef IL_USE_JIT
		_this->bounds[dim].lower = IL_VA_ARG(va, ILVaInt);
		_this->bounds[dim].size  = IL_VA_ARG(va, ILVaInt);
	#else
		_this->bounds[dim].lower = ArgWalkerGetInt(&args);
		_this->bounds[dim].size  = ArgWalkerGetInt(&args);
	#endif
	}
#ifdef IL_USE_JIT
	IL_VA_END;
#endif

	/* Fill in the array header with the multiplier values */
	multiplier = 1;
	for(dim = _this->rank - 1; dim >= 0; --dim)
	{
		_this->bounds[dim].multiplier = multiplier;
		multiplier *= _this->bounds[dim].size;
	}

	/* Construct the data part of the array */
	return ConstructMArrayData(thread, _this, elemIsPrimitive);
}

/*
 * Get a signed byte value from a multi-dimensional array.
 *
 * public sbyte Get(int index1, ..., int indexN)
 */
static ILInt8 System_MArray_Get_sbyte(ILExecThread *thread,
#ifdef IL_USE_JIT
							          System_MArray *_this, ...)
#else
							          System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);

	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);

	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILInt8 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a signed byte value from a two-dimensional array.
 *
 * public sbyte Get(int index1, int index2)
 */
static ILInt8 System_MArray_Get2_sbyte(ILExecThread *thread,
							           System_MArray *_this,
									   ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILInt8 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get an unsigned byte value from a multi-dimensional array.
 *
 * public byte Get(int index1, ..., int indexN)
 */
static ILUInt8 System_MArray_Get_byte(ILExecThread *thread,
#ifdef IL_USE_JIT
							          System_MArray *_this, ...)
#else
							          System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILUInt8 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get an unsigned byte value from a two-dimensional array.
 *
 * public sbyte Get(int index1, int index2)
 */
static ILUInt8 System_MArray_Get2_byte(ILExecThread *thread,
							           System_MArray *_this,
									   ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILUInt8 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a signed short value from a multi-dimensional array.
 *
 * public short Get(int index1, ..., int indexN)
 */
static ILInt16 System_MArray_Get_short(ILExecThread *thread,
#ifdef IL_USE_JIT
							           System_MArray *_this, ...)
#else
							           System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILInt16 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a signed short value from a two-dimensional array.
 *
 * public short Get(int index1, int index2)
 */
static ILInt16 System_MArray_Get2_short(ILExecThread *thread,
							            System_MArray *_this,
									    ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILInt16 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get an unsigned short value from a multi-dimensional array.
 *
 * public ushort Get(int index1, ..., int indexN)
 */
static ILUInt16 System_MArray_Get_ushort(ILExecThread *thread,
#ifdef IL_USE_JIT
							             System_MArray *_this, ...)
#else
							             System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILUInt16 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get an unsigned short value from a two-dimensional array.
 *
 * public ushort Get(int index1, int index2)
 */
static ILUInt16 System_MArray_Get2_ushort(ILExecThread *thread,
							              System_MArray *_this,
									      ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILUInt16 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a signed int value from a multi-dimensional array.
 *
 * public int Get(int index1, ..., int indexN)
 */
static ILInt32 System_MArray_Get_int(ILExecThread *thread,
#ifdef IL_USE_JIT
							         System_MArray *_this, ...)
#else
							         System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILInt32 *)address);
	}
	else
	{
		return 0;
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
	}
}

/*
 * Get a signed int value from a two-dimensional array.
 *
 * public int Get(int index1, int index2)
 */
static ILInt32 System_MArray_Get2_int(ILExecThread *thread,
							          System_MArray *_this,
									  ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILInt32 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get an unsigned int value from a multi-dimensional array.
 *
 * public uint Get(int index1, ..., int indexN)
 */
static ILUInt32 System_MArray_Get_uint(ILExecThread *thread,
#ifdef IL_USE_JIT
							           System_MArray *_this, ...)
#else
							           System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILUInt32 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get an unsigned int value from a two-dimensional array.
 *
 * public uint Get(int index1, int index2)
 */
static ILUInt32 System_MArray_Get2_uint(ILExecThread *thread,
							            System_MArray *_this,
									    ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILUInt32 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a signed long value from a multi-dimensional array.
 *
 * public long Get(int index1, ..., int indexN)
 */
static ILInt64 System_MArray_Get_long(ILExecThread *thread,
#ifdef IL_USE_JIT
							          System_MArray *_this, ...)
#else
							          System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILInt64 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a signed long value from a two-dimensional array.
 *
 * public long Get(int index1, int index2)
 */
static ILInt64 System_MArray_Get2_long(ILExecThread *thread,
							           System_MArray *_this,
									   ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILInt64 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get an unsigned long value from a multi-dimensional array.
 *
 * public ulong Get(int index1, ..., int indexN)
 */
static ILUInt64 System_MArray_Get_ulong(ILExecThread *thread,
#ifdef IL_USE_JIT
							            System_MArray *_this, ...)
#else
							            System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILUInt64 *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get an unsigned long value from a two-dimensional array.
 *
 * public ulong Get(int index1, int index2)
 */
static ILUInt64 System_MArray_Get2_ulong(ILExecThread *thread,
							             System_MArray *_this,
									     ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILUInt64 *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a float value from a multi-dimensional array.
 *
 * public float Get(int index1, ..., int indexN)
 */
static ILFloat System_MArray_Get_float(ILExecThread *thread,
#ifdef IL_USE_JIT
							           System_MArray *_this, ...)
#else
							           System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILFloat *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a float value from a two-dimensional array.
 *
 * public float Get(int index1, int index2)
 */
static ILFloat System_MArray_Get2_float(ILExecThread *thread,
							            System_MArray *_this,
									    ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILFloat *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a double value from a multi-dimensional array.
 *
 * public double Get(int index1, ..., int indexN)
 */
static ILDouble System_MArray_Get_double(ILExecThread *thread,
#ifdef IL_USE_JIT
							             System_MArray *_this, ...)
#else
							             System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILDouble *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a double value from a two-dimensional array.
 *
 * public double Get(int index1, int index2)
 */
static ILDouble System_MArray_Get2_double(ILExecThread *thread,
							              System_MArray *_this,
									      ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILDouble *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a native float value from a multi-dimensional array.
 *
 * public native float Get(int index1, ..., int indexN)
 */
static ILNativeFloat System_MArray_Get_nativeFloat(ILExecThread *thread,
#ifdef IL_USE_JIT
							             		   System_MArray *_this, ...)
#else
							             		   System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILNativeFloat *)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a native float value from a two-dimensional array.
 *
 * public native float Get(int index1, int index2)
 */
static ILNativeFloat System_MArray_Get2_nativeFloat(ILExecThread *thread,
							                        System_MArray *_this,
									                ILInt32 index1,
													ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILNativeFloat *)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get an object reference value from a multi-dimensional array.
 *
 * public Object Get(int index1, ..., int indexN)
 */
static ILObject *System_MArray_Get_ref(ILExecThread *thread,
#ifdef IL_USE_JIT
							           System_MArray *_this, ...)
#else
							           System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return *((ILObject **)address);
	}
	else
	{
	#ifdef IL_USE_JIT
		IL_VA_END;
	#endif
		return 0;
	}
}

/*
 * Get a native reference value from a two-dimensional array.
 *
 * public Object Get(int index1, int index2)
 */
static ILObject *System_MArray_Get2_ref(ILExecThread *thread,
							            System_MArray *_this,
									    ILInt32 index1, ILInt32 index2)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		return *((ILObject **)address);
	}
	else
	{
		return 0;
	}
}

/*
 * Get a managed value from a multi-dimensional array.
 *
 * public type Get(int index1, ..., int indexN)
 */
static void System_MArray_Get_managedValue(ILExecThread *thread,
										   void *result,
#ifdef IL_USE_JIT
							       		   System_MArray *_this, ...)
#else
							               System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	if(address)
	{
		ILMemCpy(result, address, _this->elemSize);
	}
#ifdef IL_USE_JIT
	IL_VA_END;
#endif
}

/*
 * Get a signed native int value from a multi-dimensional array.
 *
 * public native int Get(int index1, ..., int indexN)
 */
#ifdef IL_NATIVE_INT32
#define	System_MArray_Get_nativeInt		System_MArray_Get_int
#define	System_MArray_Get2_nativeInt	System_MArray_Get2_int
#else
#define	System_MArray_Get_nativeInt		System_MArray_Get_long
#define	System_MArray_Get2_nativeInt	System_MArray_Get2_long
#endif

/*
 * Get a unsigned native int value from a multi-dimensional array.
 *
 * public native uint Get(int index1, ..., int indexN)
 */
#ifdef IL_NATIVE_INT32
#define	System_MArray_Get_nativeUInt	System_MArray_Get_uint
#define	System_MArray_Get2_nativeUInt	System_MArray_Get2_uint
#else
#define	System_MArray_Get_nativeUInt	System_MArray_Get_ulong
#define	System_MArray_Get2_nativeUInt	System_MArray_Get2_ulong
#endif

/*
 * Get the address of an element within a multi-dimensional array.
 *
 * public type & Address(int index1, ..., int indexN)
 */
static void *System_MArray_Address(ILExecThread *thread,
#ifdef IL_USE_JIT
							       System_MArray *_this, ...)
#else
							       System_MArray *_this)
#endif
{
	void *address;
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	address = GetElemAddress(thread, _this, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	address = GetElemAddress(thread, _this, &args);
#endif
	return address;
}

/*
 * Set a byte value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, sbyte value)
 * public void Set(int index1, ..., int indexN, byte value)
 */
static void System_MArray_Set_byte(ILExecThread *thread,
#ifdef IL_USE_JIT
							       System_MArray *_this, ...)
#else
							       System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_I1, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_I1, &args);
#endif
}

/*
 * Set a byte value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, sbyte value)
 * public void Set(int index1, int index2, byte value)
 */
static void System_MArray_Set2_byte(ILExecThread *thread,
							        System_MArray *_this,
									ILInt32 index1, ILInt32 index2,
									ILInt8 value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILInt8 *)address) = value;
	}
}

/*
 * Set a short value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, short value)
 * public void Set(int index1, ..., int indexN, ushort value)
 */
static void System_MArray_Set_short(ILExecThread *thread,
#ifdef IL_USE_JIT
							        System_MArray *_this, ...)
#else
							        System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_I2, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_I2, &args);
#endif
}

/*
 * Set a short value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, short value)
 * public void Set(int index1, int index2, ushort value)
 */
static void System_MArray_Set2_short(ILExecThread *thread,
							         System_MArray *_this,
									 ILInt32 index1, ILInt32 index2,
									 ILInt16 value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILInt16 *)address) = value;
	}
}

/*
 * Set an int value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, int value)
 * public void Set(int index1, ..., int indexN, uint value)
 */
static void System_MArray_Set_int(ILExecThread *thread,
#ifdef IL_USE_JIT
							      System_MArray *_this, ...)
#else
							      System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_I4, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_I4, &args);
#endif
}

/*
 * Set an int value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, int value)
 * public void Set(int index1, int index2, uint value)
 */
static void System_MArray_Set2_int(ILExecThread *thread,
							       System_MArray *_this,
								   ILInt32 index1, ILInt32 index2,
								   ILInt32 value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILInt32 *)address) = value;
	}
}

/*
 * Set a long value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, long value)
 * public void Set(int index1, ..., int indexN, ulong value)
 */
static void System_MArray_Set_long(ILExecThread *thread,
#ifdef IL_USE_JIT
							       System_MArray *_this, ...)
#else
							       System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_I8, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_I8, &args);
#endif
}

/*
 * Set a long value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, long value)
 * public void Set(int index1, int index2, ulong value)
 */
static void System_MArray_Set2_long(ILExecThread *thread,
							        System_MArray *_this,
								    ILInt32 index1, ILInt32 index2,
								    ILInt64 value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILInt64 *)address) = value;
	}
}

/*
 * Set a float value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, float value)
 */
static void System_MArray_Set_float(ILExecThread *thread,
#ifdef IL_USE_JIT
							        System_MArray *_this, ...)
#else
							        System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_R4, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_R4, &args);
#endif
}

/*
 * Set a float value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, float value)
 */
static void System_MArray_Set2_float(ILExecThread *thread,
							         System_MArray *_this,
								     ILInt32 index1, ILInt32 index2,
								     ILFloat value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILFloat *)address) = value;
	}
}

/*
 * Set a double value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, double value)
 */
static void System_MArray_Set_double(ILExecThread *thread,
#ifdef IL_USE_JIT
							         System_MArray *_this, ...)
#else
							         System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_R8, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_R8, &args);
#endif
}

/*
 * Set a double value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, double value)
 */
static void System_MArray_Set2_double(ILExecThread *thread,
							          System_MArray *_this,
								      ILInt32 index1, ILInt32 index2,
								      ILDouble value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILDouble *)address) = value;
	}
}

/*
 * Set a native float value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, native float value)
 */
static void System_MArray_Set_nativeFloat(ILExecThread *thread,
#ifdef IL_USE_JIT
							              System_MArray *_this, ...)
#else
							              System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_R, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_R, &args);
#endif
}

/*
 * Set a native float value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, native float value)
 */
static void System_MArray_Set2_nativeFloat(ILExecThread *thread,
							               System_MArray *_this,
								           ILInt32 index1, ILInt32 index2,
								           ILNativeFloat value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILNativeFloat *)address) = value;
	}
}

/*
 * Set an object reference value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, Object value)
 */
static void System_MArray_Set_ref(ILExecThread *thread,
#ifdef IL_USE_JIT
							      System_MArray *_this, ...)
#else
							      System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_OBJECT, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_OBJECT, &args);
#endif
}

/*
 * Set an object reference value within a two-dimensional array.
 *
 * public void Set(int index1, int index2, Object value)
 */
static void System_MArray_Set2_ref(ILExecThread *thread,
							       System_MArray *_this,
								   ILInt32 index1, ILInt32 index2,
								   ILObject *value)
{
	void *address;
	address = Get2ElemAddress(thread, _this, index1, index2);
	if(address)
	{
		*((ILObject **)address) = value;
	}
}

/*
 * Set a managed value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, type value)
 */
static void System_MArray_Set_managedValue(ILExecThread *thread,
#ifdef IL_USE_JIT
							               System_MArray *_this, ...)
#else
							               System_MArray *_this)
#endif
{
#ifdef IL_USE_JIT
	IL_VA_START(_this);
	SetElement(thread, _this, IL_META_ELEMTYPE_VALUETYPE, &va);
	IL_VA_END;
#else
	ArgWalker args;
	ArgWalkerInitThis(&args);
	SetElement(thread, _this, IL_META_ELEMTYPE_VALUETYPE, &args);
#endif
}

/*
 * Set a native int value within a multi-dimensional array.
 *
 * public void Set(int index1, ..., int indexN, native int value)
 * public void Set(int index1, ..., int indexN, native uint value)
 */
#ifdef IL_NATIVE_INT32
#define	System_MArray_Set_nativeInt		System_MArray_Set_int
#define	System_MArray_Set2_nativeInt	System_MArray_Set2_int
#else
#define	System_MArray_Set_nativeInt		System_MArray_Set_long
#define	System_MArray_Set2_nativeInt	System_MArray_Set2_long
#endif

/*
 * Determine if a class inherits from "$Synthetic.MArray".
 */
static int IsMArrayClass(ILClass *classInfo)
{
	const char *name;
	classInfo = ILClass_ParentClass(classInfo);
	if(!classInfo)
	{
		return 0;
	}
	name = ILClass_Name(classInfo);
	if(strcmp(name, "MArray") != 0)
	{
		return 0;
	}
	name = ILClass_Namespace(classInfo);
	return (name != 0 && !strcmp(name, "$Synthetic"));
}

#if !defined(HAVE_LIBFFI)

/*
 * Marshal a single-dimensional array constructor for non-libffi systems.
 */
static void System_SArray_ctor_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	*((System_Array **)rvalue) =
		System_SArray_ctor(*((ILExecThread **)(avalue[0])),
		                   *((ILUInt32 *)(avalue[1])));
}

/*
 * Marshal a type 1 multi-dimensional array constructor for non-libffi systems.
 */
static void System_MArray_ctor_1_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	*((System_MArray **)rvalue) =
		System_MArray_ctor_1(*((ILExecThread **)(avalue[0])));
}

/*
 * Marshal a type 2 multi-dimensional array constructor for non-libffi systems.
 */
static void System_MArray_ctor_2_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	*((System_MArray **)rvalue) =
		System_MArray_ctor_2(*((ILExecThread **)(avalue[0])));
}

/*
 * Marshal a multi-dimensional "Get" method for non-libffi systems.
 */
#define	MarshalGet(type,name)	\
static void System_MArray_Get_##name##_marshal	\
	(void (*fn)(), void *rvalue, void **avalue)	\
{	\
	*((type *)rvalue) =		\
		System_MArray_Get_##name	\
				(*((ILExecThread **)(avalue[0])),	\
				 *((System_MArray **)(avalue[1])));	\
} \
static void System_MArray_Get2_##name##_marshal	\
	(void (*fn)(), void *rvalue, void **avalue)	\
{	\
	*((type *)rvalue) =		\
		System_MArray_Get2_##name	\
				(*((ILExecThread **)(avalue[0])),	\
				 *((System_MArray **)(avalue[1])),	\
				 *((ILInt32 *)(avalue[2])),			\
				 *((ILInt32 *)(avalue[3])));		\
}
MarshalGet(ILInt8, sbyte)
MarshalGet(ILUInt8, byte)
MarshalGet(ILInt16, short)
MarshalGet(ILUInt16, ushort)
MarshalGet(ILInt32, int)
MarshalGet(ILUInt32, uint)
MarshalGet(ILInt64, long)
MarshalGet(ILUInt64, ulong)
MarshalGet(ILNativeInt, nativeInt)
MarshalGet(ILNativeUInt, nativeUInt)
MarshalGet(ILFloat, float)
MarshalGet(ILDouble, double)
MarshalGet(ILNativeFloat, nativeFloat)
MarshalGet(void *, ref)
static void System_MArray_Get_managedValue_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	System_MArray_Get_managedValue
		(*((ILExecThread **)(avalue[0])),
		 *((void **)(avalue[1])),
		 *((System_MArray **)(avalue[2])));
}

/*
 * Marshal a multi-dimensional "Set" method for non-libffi systems.
 */
#define	MarshalSet(type,name)	\
static void System_MArray_Set_##name##_marshal	\
	(void (*fn)(), void *rvalue, void **avalue)	\
{	\
	System_MArray_Set_##name	\
		(*((ILExecThread **)(avalue[0])),	\
		 *((System_MArray **)(avalue[1])));	\
}	\
static void System_MArray_Set2_##name##_marshal	\
	(void (*fn)(), void *rvalue, void **avalue)	\
{	\
	System_MArray_Set2_##name	\
			(*((ILExecThread **)(avalue[0])),	\
			 *((System_MArray **)(avalue[1])),	\
			 *((ILInt32 *)(avalue[2])),			\
			 *((ILInt32 *)(avalue[3])),			\
			 *((type *)(avalue[4])));			\
}
MarshalSet(ILInt8, byte)
MarshalSet(ILInt16, short)
MarshalSet(ILInt32, int)
MarshalSet(ILInt64, long)
MarshalSet(ILNativeInt, nativeInt)
MarshalSet(ILFloat, float)
MarshalSet(ILDouble, double)
MarshalSet(ILNativeFloat, nativeFloat)
MarshalSet(void *, ref)
static void System_MArray_Set_managedValue_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	System_MArray_Set_managedValue
		(*((ILExecThread **)(avalue[0])),
		 *((System_MArray **)(avalue[1])));
}

/*
 * Marshal a multi-dimensional "Address" method for non-libffi systems.
 */
static void System_MArray_Address_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	*((void **)rvalue) = System_MArray_Address
		(*((ILExecThread **)(avalue[0])),
		 *((System_MArray **)(avalue[1])));
}

#else	/* HAVE_LIBFFI */

/*
 * We don't need marshalling functions if we have libffi.
 */
#define	System_SArray_ctor_marshal					0
#define	System_MArray_ctor_1_marshal				0
#define	System_MArray_ctor_2_marshal				0
#define	System_MArray_Get_sbyte_marshal				0
#define	System_MArray_Get2_sbyte_marshal			0
#define	System_MArray_Get_byte_marshal				0
#define	System_MArray_Get2_byte_marshal				0
#define	System_MArray_Get_short_marshal				0
#define	System_MArray_Get2_short_marshal			0
#define	System_MArray_Get_ushort_marshal			0
#define	System_MArray_Get2_ushort_marshal			0
#define	System_MArray_Get_int_marshal				0
#define	System_MArray_Get2_int_marshal				0
#define	System_MArray_Get_uint_marshal				0
#define	System_MArray_Get2_uint_marshal				0
#define	System_MArray_Get_long_marshal				0
#define	System_MArray_Get2_long_marshal				0
#define	System_MArray_Get_ulong_marshal				0
#define	System_MArray_Get2_ulong_marshal			0
#define	System_MArray_Get_nativeInt_marshal			0
#define	System_MArray_Get2_nativeInt_marshal		0
#define	System_MArray_Get_nativeUInt_marshal		0
#define	System_MArray_Get2_nativeUInt_marshal		0
#define	System_MArray_Get_float_marshal				0
#define	System_MArray_Get2_float_marshal			0
#define	System_MArray_Get_double_marshal			0
#define	System_MArray_Get2_double_marshal			0
#define	System_MArray_Get_nativeFloat_marshal		0
#define	System_MArray_Get2_nativeFloat_marshal		0
#define	System_MArray_Get_managedValue_marshal		0
#define	System_MArray_Get_ref_marshal				0
#define	System_MArray_Get2_ref_marshal				0
#define	System_MArray_Set_byte_marshal				0
#define	System_MArray_Set2_byte_marshal				0
#define	System_MArray_Set_short_marshal				0
#define	System_MArray_Set2_short_marshal			0
#define	System_MArray_Set_int_marshal				0
#define	System_MArray_Set2_int_marshal				0
#define	System_MArray_Set_long_marshal				0
#define	System_MArray_Set2_long_marshal				0
#define	System_MArray_Set_nativeInt_marshal			0
#define	System_MArray_Set2_nativeInt_marshal		0
#define	System_MArray_Set_float_marshal				0
#define	System_MArray_Set2_float_marshal			0
#define	System_MArray_Set_double_marshal			0
#define	System_MArray_Set2_double_marshal			0
#define	System_MArray_Set_nativeFloat_marshal		0
#define	System_MArray_Set2_nativeFloat_marshal		0
#define	System_MArray_Set_managedValue_marshal		0
#define	System_MArray_Set_ref_marshal				0
#define	System_MArray_Set2_ref_marshal				0
#define	System_MArray_Address_marshal				0

#endif	/* HAVE_LIBFFI */

#else /* !IL_CONFIG_NON_VECTOR_ARRAYS */

#if !defined(HAVE_LIBFFI)

/*
 * Marshal a single-dimensional array constructor for non-libffi systems.
 */
static void System_SArray_ctor_marshal
	(void (*fn)(), void *rvalue, void **avalue)
{
	*((System_Array **)rvalue) =
		System_SArray_ctor(*((ILExecThread **)(avalue[0])),
		                   *((ILUInt32 *)(avalue[1])));
}

#else	/* HAVE_LIBFFI */

/*
 * We don't need marshalling functions if we have libffi.
 */
#define	System_SArray_ctor_marshal					0

#endif	/* HAVE_LIBFFI */

#endif /* !IL_CONFIG_NON_VECTOR_ARRAYS */

/*
 * Get the internal version of a synthetic "SArray" or "MArray" method.
 */
int _ILGetInternalArray(ILMethod *method, int *isCtor, ILInternalInfo *info)
{
	ILClass *classInfo;
	const char *name;
	ILType *type;
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	int rank;
#endif

	classInfo = ILMethod_Owner(method);
	if(!classInfo)
	{
		return 0;
	}
	name = ILMethod_Name(method);
	type = ILClassGetSynType(classInfo);
	if(!type)
	{
		return 0;
	}
	if(ILType_IsSimpleArray(type))
	{
		/* Single-dimensional arrays have a simple constructor only */
		if(!strcmp(name, ".ctor"))
		{
			*isCtor = 1;
			info->un.func = (void *)System_SArray_ctor;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_SArray_ctor_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else
		{
			return 0;
		}
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	rank = ILTypeGetRank(type);
	if(!strcmp(name, ".ctor"))
	{
		/* There are two constructors for multi-dimensional arrays.
		   The first specifies sizes, and the second specifies
		   lower bounds and sizes.  Determine which one this is */
		*isCtor = 1;
		if(ILClassNextMemberByKind(classInfo, 0, IL_META_MEMBERKIND_METHOD)
				== (ILMember *)method)
		{
			/* This is the first constructor */
			info->un.func = (void *)System_MArray_ctor_1;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_ctor_1_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else
		{
			/* This is the second constructor */
			info->un.func = (void *)System_MArray_ctor_2;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_ctor_2_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
	}
	*isCtor = 0;
	if(!strcmp(name, "Get"))
	{
		/* Determine which get function to use based on the element type */
		type = ILTypeGetEnumType(ILTypeGetElemType(type));
		if(ILType_IsPrimitive(type))
		{
			switch(ILType_ToElement(type))
			{
				case IL_META_ELEMTYPE_BOOLEAN:
				case IL_META_ELEMTYPE_I1:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_sbyte;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_sbyte_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_sbyte;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_sbyte_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_U1:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_byte;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_byte_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_byte;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_byte_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I2:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_short;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_short_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_short;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_short_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_U2:
				case IL_META_ELEMTYPE_CHAR:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_ushort;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_ushort_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_ushort;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_ushort_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I4:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_int;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_int_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_int;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_int_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_U4:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_uint;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_uint_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_uint;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_uint_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_nativeInt;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_nativeInt_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_nativeInt;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_nativeInt_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_U:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_nativeUInt;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_nativeUInt_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_nativeUInt;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_nativeUInt_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I8:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_long;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_long_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_long;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_long_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_U8:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_ulong;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_ulong_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_ulong;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_ulong_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_R4:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_float;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_float_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_float;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_float_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_R8:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_double;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_double_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_double;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_double_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_R:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Get2_nativeFloat;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get2_nativeFloat_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Get_nativeFloat;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Get_nativeFloat_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_TYPEDBYREF:
				{
					info->un.func = (void *)System_MArray_Get_managedValue;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
					info->marshal =
						(void *)System_MArray_Get_managedValue_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					return 1;
				}
				/* Not reached */
			}
			info->un.func = (void *)System_MArray_Get_int;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Get_int_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else if(ILType_IsValueType(type))
		{
			info->un.func = (void *)System_MArray_Get_managedValue;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Get_managedValue_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else if(rank == 2)
		{
			info->un.func = (void *)System_MArray_Get2_ref;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Get2_ref_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else
		{
			info->un.func = (void *)System_MArray_Get_ref;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Get_ref_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
	}
	else if(!strcmp(name, "Set"))
	{
		/* Determine which set function to use based on the element type */
		type = ILTypeGetEnumType(ILTypeGetElemType(type));
		if(ILType_IsPrimitive(type))
		{
			switch(ILType_ToElement(type))
			{
				case IL_META_ELEMTYPE_BOOLEAN:
				case IL_META_ELEMTYPE_I1:
				case IL_META_ELEMTYPE_U1:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_byte;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_byte_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_byte;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_byte_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I2:
				case IL_META_ELEMTYPE_U2:
				case IL_META_ELEMTYPE_CHAR:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_short;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_short_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_short;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_short_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I4:
				case IL_META_ELEMTYPE_U4:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_int;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_int_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_int;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_int_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I:
				case IL_META_ELEMTYPE_U:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_nativeInt;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_nativeInt_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_nativeInt;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_nativeInt_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_I8:
				case IL_META_ELEMTYPE_U8:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_long;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_long_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_long;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_long_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_R4:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_float;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_float_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_float;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_float_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_R8:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_double;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_double_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_double;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_double_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_R:
				{
					if(rank == 2)
					{
						info->un.func = (void *)System_MArray_Set2_nativeFloat;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set2_nativeFloat_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					else
					{
						info->un.func = (void *)System_MArray_Set_nativeFloat;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
						info->marshal =
							(void *)System_MArray_Set_nativeFloat_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					}
					return 1;
				}
				/* Not reached */

				case IL_META_ELEMTYPE_TYPEDBYREF:
				{
					info->un.func = (void *)System_MArray_Set_managedValue;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
					info->marshal =
						(void *)System_MArray_Set_managedValue_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
					return 1;
				}
				/* Not reached */
			}
			info->un.func = (void *)System_MArray_Set_int;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Set_int_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else if(ILType_IsValueType(type))
		{
			info->un.func = (void *)System_MArray_Set_managedValue;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Set_managedValue_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else if(rank == 2)
		{
			info->un.func = (void *)System_MArray_Set2_ref;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Set2_ref_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
		else
		{
			info->un.func = (void *)System_MArray_Set_ref;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
			info->marshal = (void *)System_MArray_Set_ref_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
			return 1;
		}
	}
	else if(!strcmp(name, "Address"))
	{
		info->un.func = (void *)System_MArray_Address;
#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
		info->marshal = (void *)System_MArray_Address_marshal;
#endif /* defined(IL_USE_CVM) && !defined(HAVE_LIBFFI) */
		return 1;
	}
	else
#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */
	{
		return 0;
	}
}

int _ILIsSArray(System_Array *array)
{
	if(array)
	{
		ILClass *classInfo;
		const char *name;
		classInfo = ILClass_ParentClass(GetObjectClass(array));
		if(!classInfo)
		{
			return 0;
		}
		name = ILClass_Name(classInfo);
		if(strcmp(name, "SArray") != 0)
		{
			return 0;
		}
		name = ILClass_Namespace(classInfo);
		return (name != 0 && !strcmp(name, "$Synthetic"));
	}
	else
	{
		return 0;
	}
}

/*
 * Validate the arguments for a copy with array, array, length arguments.
 */
static int ValidateCopyArgs_AAI4(ILExecThread *thread,
								 System_Array *array1,
								 System_Array *array2,
								 ILInt32 length)
{
	if(!array1)
	{
		ILExecThreadThrowArgNull(thread, "sourceArray");
		return 0;
	}
	if(!array2)
	{
		ILExecThreadThrowArgNull(thread, "destinationArray");
		return 0;
	}
	if(length < 0)
	{
		ILExecThreadThrowArgRange(thread, "length", "Arg_InvalidArrayIndex");
		return 0;
	}
	if(array1 == array2)
	{
		/* We have nothing to do here but check the length anyways */
		if(length > ArrayLength(array1))
		{
			ILExecThreadThrowSystem(thread, "System.ArgumentException",
									"Arg_InvalidArrayRange");
			return 0;
		}
	}
	else
	{
		if((length > ArrayLength(array1)) || (length > ArrayLength(array2)))
		{
			ILExecThreadThrowSystem(thread, "System.ArgumentException",
									"Arg_InvalidArrayRange");
			return 0;
		}
	}
	return 1;
}

/*
 * Validate the arguments for a copy with array, array, length arguments.
 */
static int ValidateCopyArgs_AI4AI4I4(ILExecThread *thread,
									 System_Array *array1,
									 ILInt32 index1,
									 System_Array *array2,
									 ILInt32 index2,
									 ILInt32 length)
{
	if(!array1)
	{
		ILExecThreadThrowArgNull(thread, "sourceArray");
		return 0;
	}
	if(!array2)
	{
		ILExecThreadThrowArgNull(thread, "destinationArray");
		return 0;
	}
	if(length < 0)
	{
		ILExecThreadThrowArgRange(thread, "length", "Arg_InvalidArrayIndex");
		return 0;
	}
	if(index1 < 0)
	{
		ILExecThreadThrowArgRange(thread, "sourceIndex", "Arg_InvalidArrayIndex");
		return 0;
	}
	if(index2 < 0)
	{
		ILExecThreadThrowArgRange(thread, "destinationIndex", "Arg_InvalidArrayIndex");
		return 0;
	}
	if(array1 == array2)
	{
		/* We have nothing to do here but check the length anyways */
		if((index1 + length) > ArrayLength(array1))
		{
			ILExecThreadThrowSystem(thread, "System.ArgumentException",
									"Arg_InvalidArrayRange");
			return 0;
		}
	}
	else
	{
		if(((index1 + length) > ArrayLength(array1)) ||
		   ((index2 + length) > ArrayLength(array2)))
		{
			ILExecThreadThrowSystem(thread, "System.ArgumentException",
									"Arg_InvalidArrayRange");
			return 0;
		}
	}
	return 1;
}

/*
 * Validate the arguments for a copy with array, array, length arguments.
 */
static int ValidateClearArgs_AI4I4(ILExecThread *thread,
								   System_Array *array,
								   ILInt32 index,
								   ILInt32 length)
{
	if(!array)
	{
		ILExecThreadThrowArgNull(thread, "array");
		return 0;
	}
	if(index < 0)
	{
		ILExecThreadThrowArgRange(thread, "index", "Arg_InvalidArrayIndex");
		return 0;
	}
	if(length < 0)
	{
		ILExecThreadThrowArgRange(thread, "length", "Arg_InvalidArrayIndex");
		return 0;
	}
	if((index + length) > ArrayLength(array))
	{
		ILExecThreadThrowSystem(thread, "System.ArgumentException",
								"Arg_InvalidArrayRange");
		return 0;
	}
	return 1;
}

/*
 * Internal versions for Array.Copy(System.Array, System.Array, int) where the
 * types of the arrays are known at verification time, the element types
 * are known to be assignment compatible and the sizes of the elements are the
 * same so that copying can be done with memcpy or memmove.
 */
ILInt32 ILSArrayCopy_AAI4(ILExecThread *thread,
						  System_Array *array1,
						  System_Array *array2,
						  ILInt32 length,
						  ILInt32 elementSize)
{
	if(ValidateCopyArgs_AAI4(thread, array1, array2, length))
	{
		if(array1 != array2)
		{
			void *base1 = ArrayToBuffer(array1);
			void *base2 = ArrayToBuffer(array2);

			ILMemCpy(base2, base1, length * elementSize);
		}
		return 1;
	}
	return 0;
}

/*
 * Function for Array.Copy(System.Array, int, System.Array, int, int) where the
 * types of the arrays are known at verification time, the element types
 * are known to be assignment compatible and the sizes of the elements are the
 * same so that copying can be done with memcpy or memmove.
 */
ILInt32 ILSArrayCopy_AI4AI4I4(ILExecThread *thread,
							  System_Array *array1,
							  ILInt32 index1,
							  System_Array *array2,
							  ILInt32 index2,
							  ILInt32 length,
							  ILInt32 elementSize)
{
	if(ValidateCopyArgs_AI4AI4I4(thread, array1, index1, array2, index2, length))
	{
		ILInt8 *base1 = ArrayToBuffer(array1);
		ILInt8 *base2 = ArrayToBuffer(array2);

		if((array1 != array2) || ((index1 + length) < index2) ||
								  (index1 > (index2 + length)))
		{
			/* Arrays are not the same or the ranges do not overlap */
			ILMemCpy(&(base2[index2 * elementSize]),
					 &(base1[index1 * elementSize]),
					 length * elementSize);
		}
		else
		{
			/* Ranges overlap */
			ILMemMove(&(base2[index2 * elementSize]),
					  &(base1[index1 * elementSize]),
					  length * elementSize);
		}
		return 1;
	}
	return 0;
}

/*
 * Function for Array.Clear(System.Array, int, int) where the type of the
 * array is known at verification time and it is zero based one dimensional.
 */
ILInt32 ILSArrayClear_AI4I4(ILExecThread *thread,
							System_Array *array,
							ILInt32 index,
							ILInt32 length,
							ILInt32 elementSize)
{
	if(ValidateClearArgs_AI4I4(thread, array, index, length))
	{
		ILInt8 *base = ArrayToBuffer(array);

		ILMemZero(&(base[index * elementSize]),
				  length * elementSize);
		return 1;
	}
	return 0;
}

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS

int _ILIsMArray(System_Array *array)
{
	if(array)
	{
		return IsMArrayClass(GetObjectClass(array));
	}
	else
	{
		return 0;
	}
}

#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */

/*
 * Get the element type for an array object.
 */
static ILType *GetArrayElemType(System_Array *array)
{
	ILType *type = ILClassGetSynType(GetObjectClass(array));
	return ILTypeGetElemType(type);
}

/*
 * public static void Clear(Array array, int index, int length);
 */
void _IL_Array_Clear(ILExecThread *thread, ILObject *_array,
				     ILInt32 index, ILInt32 length)
{
	System_Array *array = (System_Array *)_array;
	ILType *elemType;
	ILUInt32 elemSize;
	void *start;

	/* Bail out if the array is NULL */
	if(!array)
	{
		ILExecThreadThrowArgNull(thread, "array");
		return;
	}

	/* Get the element type and size */
	elemType = GetArrayElemType(array);
	elemSize = ILSizeOfType(thread, elemType);

	/* Determine the start address of the clear */
	if(_ILIsSArray(array))
	{
		if(index < 0)
		{
			ILExecThreadThrowArgRange(thread, "index", "Arg_InvalidArrayIndex");
			return;
		}
		else if(length < 0 ||
		        (ArrayLength(array) - index) < length)
		{
			ILExecThreadThrowArgRange(thread, "index", "Arg_InvalidArrayRange");
			return;
		}
		start = ((unsigned char *)ArrayToBuffer(array)) +
					((ILUInt32)index) * elemSize;
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(array))
	{
		System_MArray *marray = (System_MArray *)array;
		ILInt32 dim;
		ILInt32 totalLen;
		if(index < marray->bounds[0].lower)
		{
			ILExecThreadThrowArgRange(thread, "index", "Arg_InvalidArrayIndex");
			return;
		}
		index -= marray->bounds[0].lower;
		totalLen = 1;
		for(dim = 0; dim < marray->rank; ++dim)
		{
			totalLen *= marray->bounds[dim].size;
		}
		if(length < 0 || (totalLen - index) < length)
		{
			ILExecThreadThrowArgRange(thread, "index", "Arg_InvalidArrayRange");
			return;
		}
		start = ((unsigned char *)(marray->data)) +
					((ILUInt32)index) * elemSize;
	}
#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */
	else
	{
		return;
	}

	/* Clear the array contents within the specified range */
	if(length > 0)
	{
		ILMemZero(start, length * elemSize);
	}
}

/*
 * public static void Initialize();
 */
void _IL_Array_Initialize(ILExecThread *thread, ILObject *thisObj)
{
	System_Array *_this = (System_Array *)thisObj;
	ILType *elemType;
	ILUInt32 elemSize;
	ILInt32 totalLen;
	void *start;
	ILMethod *method;

	/* Get the element type and size */
	elemType = GetArrayElemType(_this);
	elemSize = ILSizeOfType(thread, elemType);

	/* Bail out if this is not a value type with a default constructor */
	if(!ILType_IsValueType(elemType))
	{
		return;
	}
	method = 0;
	while((method = (ILMethod *)ILClassNextMemberByKind
				(ILType_ToValueType(elemType), (ILMember *)method,
				 IL_META_MEMBERKIND_METHOD)) != 0)
	{
		if(ILMethod_IsConstructor(method) &&
		   ILTypeNumParams(ILMethod_Signature(method)) == 0)
		{
			break;
		}
	}
	if(method == 0)
	{
		return;
	}

	/* Determine the start address and length of the clear */
	if(_ILIsSArray(_this))
	{
		start = ArrayToBuffer(_this);
		totalLen = ArrayLength(_this);
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		System_MArray *marray = (System_MArray *)_this;
		ILInt32 dim;
		totalLen = 1;
		for(dim = 0; dim < marray->rank; ++dim)
		{
			totalLen *= marray->bounds[dim].size;
		}
		start = marray->data;
	}
#endif
	else
	{
		return;
	}

	/* Initialize the array contents within the specified range */
	while(totalLen > 0)
	{
		if(ILExecThreadCall(thread, method, (void *)0, start))
		{
			/* An exception occurred during the constructor */
			break;
		}
		start = (void *)(((unsigned char *)start) + elemSize);
		--totalLen;
	}
}

/*
 * public static void InternalCopy(Array sourceArray, int sourceIndex,
 *                                 Array destArray, int destIndex,
 *                                 int length);
 */
void _IL_Array_InternalCopy(ILExecThread *thread,
				            ILObject *sourceArray,
				            ILInt32 sourceIndex,
				            ILObject *destArray,
				            ILInt32 destIndex,
				            ILInt32 length)
{
	void *src, *dest;
	ILType *synType;
	ILInt32 size;

	/* Get the base pointers for the two arrays and the element size */
	if(_ILIsSArray((System_Array *)sourceArray))
	{
		src = ArrayToBuffer(sourceArray);
		synType = ILClassGetSynType(GetObjectClass(sourceArray));
		size = (ILInt32)(ILSizeOfType(thread, ILType_ElemType(synType)));
	}
	else
	{
	#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
		src = ((System_MArray *)sourceArray)->data;
		size = ((System_MArray *)sourceArray)->elemSize;
	#else
		return;
	#endif
	}
	if(_ILIsSArray((System_Array *)destArray))
	{
		dest = ArrayToBuffer(destArray);
	}
	else
	{
	#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
		dest = ((System_MArray *)destArray)->data;
	#else
		return;
	#endif
	}

	/* Copy the contents of the array */
	ILMemMove(((unsigned char *)dest) + destIndex * size,
			  ((unsigned char *)src) + sourceIndex * size,
			  length * size);
}

/*
 * private static Array CreateArray(IntPtr elementType,
 *                                  int rank, int length1,
 *                                  int length2, int length3);
 */
ILObject *_IL_Array_CreateArray_jiiii(ILExecThread *thread,
							          ILNativeInt elementType,
							          ILInt32 rank, ILInt32 length1,
							          ILInt32 length2, ILInt32 length3)
{
	ILClass *classInfo;
	ILType *type;
	ILType *elemType;
	ILUInt32 elemSize;
	ILUInt64 totalSize;
	System_Array *array;
	int isPrimitive;

	/* Create the array type and class structures */
	elemType = ILClassToType((ILClass *)elementType);
	type = ILTypeFindOrCreateArray
				(thread->process->context, (unsigned long)rank, elemType);
	if(!type)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	classInfo = ILClassFromType(ILProgramItem_Image(thread->method),
								0, type, 0);
	if(!classInfo)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	classInfo = ILClassResolve(classInfo);

	/* Compute the element size */
	elemSize = ILSizeOfType(thread, elemType);

	/* Determine if the element type is primitive */
	if(ILType_IsPrimitive(elemType) && elemType != ILType_TypedRef)
	{
		isPrimitive = 1;
	}
	else
	{
		isPrimitive = 0;
	}

	/* Determine the type of array to create */
	if(rank == 1)
	{
		/* Determine the total size of the array in bytes */
		totalSize = ((ILUInt64)elemSize) * ((ILUInt64)length1);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}

		/* Allocate the array, initialize, and return it */
		if(isPrimitive)
		{
			/* The array will never contain pointers,
			   so use atomic allocation */
			array = (System_Array *)_ILEngineAllocAtomic
				(thread, classInfo, sizeof(System_Array) + (ILUInt32)totalSize);
		}
		else
		{
			/* The array might contain pointers, so play it safe */
			array = (System_Array *)_ILEngineAlloc
				(thread, classInfo, sizeof(System_Array) + (ILUInt32)totalSize);
		}
		if(array)
		{
			ArrayLength(array) = (ILInt32)length1;
		}
		return (ILObject *)array;
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(rank == 2)
	{
		System_MArray *marray;

		/* Determine the total size of the array in bytes */
		totalSize = ((ILUInt64)elemSize) * ((ILUInt64)length1);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		totalSize *= ((ILUInt64)length2);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}

		/* Allocate the multi-dimensional array header */
		marray = (System_MArray *)_ILEngineAlloc
				(thread, classInfo, sizeof(System_MArray) +
				 sizeof(MArrayBounds));
		if(!marray)
		{
			return 0;
		}
		marray->rank = 2;
		marray->elemSize = elemSize;
		marray->bounds[0].lower      = 0;
		marray->bounds[0].size       = length1;
		marray->bounds[0].multiplier = length2;
		marray->bounds[1].lower      = 0;
		marray->bounds[1].size       = length2;
		marray->bounds[1].multiplier = 1;

		/* Allocate the data portion of the array */
		if(isPrimitive)
		{
			/* The array will never contain pointers,
			   so use atomic allocation */
			marray->data = _ILEngineAllocAtomic(thread, 0, (ILUInt32)totalSize);
		}
		else
		{
			/* The array might contain pointers, so play it safe */
			marray->data = _ILEngineAlloc(thread, 0, (ILUInt32)totalSize);
		}
		if(!(marray->data))
		{
			return 0;
		}

		/* Return the final array to the caller */
		return (ILObject *)marray;
	}
	else
	{
		System_MArray *marray;

		/* Determine the total size of the array in bytes */
		totalSize = ((ILUInt64)elemSize) * ((ILUInt64)length1);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		totalSize *= ((ILUInt64)length2);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		totalSize *= ((ILUInt64)length3);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}

		/* Allocate the multi-dimensional array header */
		marray = (System_MArray *)_ILEngineAlloc
				(thread, classInfo, sizeof(System_MArray) +
				 sizeof(MArrayBounds) * 2);
		if(!marray)
		{
			return 0;
		}
		marray->rank = 3;
		marray->elemSize = elemSize;
		marray->bounds[0].lower      = 0;
		marray->bounds[0].size       = length1;
		marray->bounds[0].multiplier = length2 * length3;
		marray->bounds[1].lower      = 0;
		marray->bounds[1].size       = length2;
		marray->bounds[1].multiplier = length3;
		marray->bounds[2].lower      = 0;
		marray->bounds[2].size       = length3;
		marray->bounds[2].multiplier = 1;

		/* Allocate the data portion of the array */
		if(isPrimitive)
		{
			/* The array will never contain pointers,
			   so use atomic allocation */
			marray->data = _ILEngineAllocAtomic(thread, 0, (ILUInt32)totalSize);
		}
		else
		{
			/* The array might contain pointers, so play it safe */
			marray->data = _ILEngineAlloc(thread, 0, (ILUInt32)totalSize);
		}
		if(!(marray->data))
		{
			return 0;
		}

		/* Return the final array to the caller */
		return (ILObject *)marray;
	}
#else /* !IL_CONFIG_NON_VECTOR_ARRAYS */
	return (ILObject *)0;
#endif /* !IL_CONFIG_NON_VECTOR_ARRAYS */
}

/*
 * private static Array CreateArray(IntPtr elementType,
 *                                  int[] lengths, int[] lowerBounds);
 */
ILObject *_IL_Array_CreateArray_jaiai(ILExecThread *thread,
									  ILNativeInt elementType,
									  System_Array *lengths,
									  System_Array *lowerBounds)
{
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	ILClass *classInfo;
	ILInt32 rank;
	ILInt32 multiplier;
	ILType *type;
	ILType *elemType;
	ILUInt32 elemSize;
	ILUInt64 totalSize;
	System_MArray *marray;
	ILInt32 dim;
#endif

	/* Handle the single-dimensional, zero lower bound, case specially */
	if(ArrayLength(lengths) == 1 &&
	   (!lowerBounds || ((ILInt32 *)ArrayToBuffer(lowerBounds))[0] == 0))
	{
		return _IL_Array_CreateArray_jiiii(thread, elementType, 1,
						((ILInt32 *)ArrayToBuffer(lengths))[0], 0, 0);
	}

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	/* Create the array type and class structures */
	rank = ArrayLength(lengths);
	elemType = ILClassToType((ILClass *)elementType);
	if(lowerBounds)
	{
		type = ILTypeCreateArray
				(thread->process->context, (unsigned long)rank, elemType);
	}
	else
	{
		type = ILTypeFindOrCreateArray
				(thread->process->context, (unsigned long)rank, elemType);
	}
	if(!type)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	if(lowerBounds)
	{
		for(dim = 0; dim < rank; ++dim)
		{
			ILTypeSetLowBound
				(type, (unsigned long)(long)dim,
				 (long)((ILInt32 *)ArrayToBuffer(lowerBounds))[dim]);
			ILTypeSetSize
				(type, (unsigned long)(long)dim,
				 (long)((ILInt32 *)ArrayToBuffer(lengths))[dim]);
		}
	}
	classInfo = ILClassFromType(ILProgramItem_Image(thread->method),
								0, type, 0);
	if(!classInfo)
	{
		ILExecThreadThrowOutOfMemory(thread);
		return 0;
	}
	classInfo = ILClassResolve(classInfo);

	/* Compute the element size */
	elemSize = ILSizeOfType(thread, elemType);

	/* Determine the total size of the array in bytes */
	totalSize = ((ILUInt64)elemSize);
	for(dim = 0; dim < rank; ++dim)
	{
		totalSize *= (ILUInt64)(((ILInt32 *)ArrayToBuffer(lengths))[dim]);
		if(totalSize > (ILUInt64)IL_MAX_INT32)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
	}

	/* Allocate the multi-dimensional array header */
	marray = (System_MArray *)_ILEngineAlloc
			(thread, classInfo, sizeof(System_MArray) +
			 sizeof(MArrayBounds) * (rank - 1));
	if(!marray)
	{
		return 0;
	}
	marray->rank = rank;
	marray->elemSize = elemSize;
	for(dim = 0; dim < rank; ++dim)
	{
		if(lowerBounds)
		{
			marray->bounds[dim].lower =
				((ILInt32 *)ArrayToBuffer(lowerBounds))[dim];
		}
		else
		{
			marray->bounds[dim].lower = 0;
		}
		marray->bounds[dim].size = ((ILInt32 *)ArrayToBuffer(lengths))[dim];
	}
	multiplier = 1;
	for(dim = rank - 1; dim >= 0; --dim)
	{
		marray->bounds[dim].multiplier = multiplier;
		multiplier *= marray->bounds[dim].size;
	}

	/* Allocate the data portion of the array */
	if(ILType_IsPrimitive(elemType) && elemType != ILType_TypedRef)
	{
		/* The array will never contain pointers,
		   so use atomic allocation */
		marray->data = _ILEngineAllocAtomic(thread, 0, (ILUInt32)totalSize);
	}
	else
	{
		/* The array might contain pointers, so play it safe */
		marray->data = _ILEngineAlloc(thread, 0, (ILUInt32)totalSize);
	}
	if(!(marray->data))
	{
		return 0;
	}

	/* Return the final array to the caller */
	return (ILObject *)marray;
#else /* !IL_CONFIG_NON_VECTOR_ARRAYS */
	return (ILObject *)0;
#endif /* !IL_CONFIG_NON_VECTOR_ARRAYS */
}

/*
 * private int GetLength();
 */
ILInt32 _IL_Array_GetLength_(ILExecThread *thread, ILObject *thisObj)
{
	System_Array *_this = (System_Array *)thisObj;
	if(_ILIsSArray(_this))
	{
		return ArrayLength(_this);
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		ILInt32 len = 1;
		ILInt32 dim;
		for(dim = 0; dim < ((System_MArray *)_this)->rank; ++dim)
		{
			len *= ((System_MArray *)_this)->bounds[dim].size;
		}
		return len;
	}
#endif
	else
	{
		return 0;
	}
}

/*
 * private int GetLength(int dimension);
 */
ILInt32 _IL_Array_GetLength_i(ILExecThread *thread,
							  ILObject *thisObj,
							  ILInt32 dimension)
{
	System_Array *_this = (System_Array *)thisObj;
	if(_ILIsSArray(_this))
	{
		if(dimension == 0)
		{
			return ArrayLength(_this);
		}
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		if(dimension >= 0 && dimension < ((System_MArray *)_this)->rank)
		{
			return ((System_MArray *)_this)->bounds[dimension].size;
		}
	}
#endif
	ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
							"Arg_InvalidArrayIndex");
	return 0;
}

/*
 * public int GetLowerBound(int dimension);
 */
ILInt32 _IL_Array_GetLowerBound(ILExecThread *thread,
							    ILObject *thisObj,
							    ILInt32 dimension)
{
	System_Array *_this = (System_Array *)thisObj;
	if(_ILIsSArray(_this))
	{
		if(dimension == 0)
		{
			return 0;
		}
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		if(dimension >= 0 && dimension < ((System_MArray *)_this)->rank)
		{
			return ((System_MArray *)_this)->bounds[dimension].lower;
		}
	}
#endif
	else
	{
		return 0;
	}
	ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
							"Arg_InvalidDimension");
	return 0;
}

/*
 * public int GetUpperBound(int dimension);
 */
ILInt32 _IL_Array_GetUpperBound(ILExecThread *thread,
							    ILObject *thisObj,
							    ILInt32 dimension)
{
	System_Array *_this = (System_Array *)thisObj;
	if(_ILIsSArray(_this))
	{
		if(dimension == 0)
		{
			return ArrayLength(_this) - 1;
		}
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		if(dimension >= 0 && dimension < ((System_MArray *)_this)->rank)
		{
			return ((System_MArray *)_this)->bounds[dimension].lower +
				   ((System_MArray *)_this)->bounds[dimension].size - 1;
		}
	}
#endif
	else
	{
		return 0;
	}
	ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
							"Arg_InvalidDimension");
	return 0;
}

/*
 * private int GetRank();
 */
ILInt32 _IL_Array_GetRank(ILExecThread *thread, ILObject *thisObj)
{
	System_Array *_this = (System_Array *)thisObj;
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	if(_ILIsMArray(_this))
	{
		return ((System_MArray *)_this)->rank;
	}
	else
#endif
	if(_ILIsSArray(_this))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * private Object Get(int index1, int index2, int index3);
 */
ILObject *_IL_Array_Get_iii(ILExecThread *thread, ILObject *thisObj,
					        ILInt32 index1, ILInt32 index2,
					        ILInt32 index3)
{
	System_Array *_this = (System_Array *)thisObj;
	ILType *elemType = GetArrayElemType(_this);
	ILUInt32 elemSize = ILSizeOfType(thread, elemType);
	if(_ILIsSArray(_this))
	{
		if(index1 < 0 || index1 >= ArrayLength(_this))
		{
			ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return 0;
		}
		if(ILType_IsPrimitive(elemType) || ILType_IsValueType(elemType))
		{
			return ILExecThreadBox(thread, elemType,
						((unsigned char *)ArrayToBuffer(_this)) +
						((ILUInt32)index1) * elemSize);
		}
		else
		{
			return ((ILObject **)ArrayToBuffer(_this))[index1];
		}
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		System_MArray *marray = (System_MArray *)_this;
		ILUInt32 offset;
		ILInt32 index;
		void *ptr;
		if(marray->rank == 1)
		{
			/* Single-dimensional array */
			index = index1 - marray->bounds[0].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[0].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return 0;
			}
			ptr = ((unsigned char *)(marray->data)) +
				  ((ILUInt32)index) * elemSize;
		}
		else if(marray->rank == 2)
		{
			/* Double-dimensional array */
			index = index1 - marray->bounds[0].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[0].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return 0;
			}
			offset = index * marray->bounds[0].multiplier;
			index = index2 - marray->bounds[1].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[1].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return 0;
			}
			offset += (ILUInt32)index;
			ptr = ((unsigned char *)(marray->data)) + offset * elemSize;
		}
		else
		{
			/* Triple-dimensional array */
			index = index1 - marray->bounds[0].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[0].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return 0;
			}
			offset = index * marray->bounds[0].multiplier;
			index = index2 - marray->bounds[1].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[1].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return 0;
			}
			offset += index * marray->bounds[1].multiplier;
			index = index3 - marray->bounds[2].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[2].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return 0;
			}
			offset += (ILUInt32)index;
			ptr = ((unsigned char *)(marray->data)) + offset * elemSize;
		}
		if(ILType_IsPrimitive(elemType) || ILType_IsValueType(elemType))
		{
			return ILExecThreadBox(thread, elemType, ptr);
		}
		else
		{
			return *((ILObject **)ptr);
		}
	}
#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */
	else
	{
		return 0;
	}
}

/*
 * private Object Get(int[] indices);
 */
ILObject *_IL_Array_Get_ai(ILExecThread *thread, ILObject *thisObj,
						   System_Array *indices)
{
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	System_Array *_this = (System_Array *)thisObj;
	System_MArray *marray;
	ILType *elemType;
	ILUInt32 elemSize;
	ILUInt32 offset;
	ILInt32 dim;
	ILInt32 index;
	void *ptr;
#endif
	ILInt32 *ind = (ILInt32 *)ArrayToBuffer(indices);

	/* Handle the single-dimensional case specially */
	if(ArrayLength(indices) == 1)
	{
		return _IL_Array_Get_iii(thread, thisObj, ind[0], 0, 0);
	}

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	/* Get the element type and its size */
	elemType = GetArrayElemType(_this);
	elemSize = ILSizeOfType(thread, elemType);

	/* Find the specific element position within the array */
	marray = (System_MArray *)_this;
	offset = 0;
	for(dim = 0; dim < marray->rank; ++dim)
	{
		index = ind[dim] - marray->bounds[dim].lower;
		if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[dim].size)))
		{
			ILExecThreadThrowSystem(thread,
									"System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return 0;
		}
		offset += (ILUInt32)(index * marray->bounds[dim].multiplier);
	}
						
	ptr = ((unsigned char *)(marray->data)) + offset * elemSize;

	/* Box and return the array element */
	if(ILType_IsPrimitive(elemType) || ILType_IsValueType(elemType))
	{
		return ILExecThreadBox(thread, elemType, ptr);
	}
	else
	{
		return *((ILObject **)ptr);
	}
#else /* !IL_CONFIG_NON_VECTOR_ARRAYS */
	return (ILObject *)0;
#endif /* !IL_CONFIG_NON_VECTOR_ARRAYS */
}

/*
 * private Object GetRelative(int index);
 */
ILObject *_IL_Array_GetRelative(ILExecThread *thread, ILObject *_this,
								ILInt32 index)
{
	void *buf;
	ILType *synType;
	ILType *elemType;
	ILInt32 size;

	/* Get the base pointer, element type, and the size of the array */
	if(_ILIsSArray((System_Array *)_this))
	{
		buf = ArrayToBuffer(_this);
		synType = ILClassGetSynType(GetObjectClass(_this));
		elemType = ILType_ElemType(synType);
		size = (ILInt32)(ILSizeOfType(thread, elemType));
	}
	else
	{
	#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
		buf = ((System_MArray *)_this)->data;
		size = ((System_MArray *)_this)->elemSize;
		elemType = GetArrayElemType((System_Array *)_this);
	#else
		return (ILObject *)0;
	#endif
	}

	/* Retrieve the value and return it */
	if(ILType_IsPrimitive(elemType) || ILType_IsValueType(elemType))
	{
		return ILExecThreadBox
			(thread, elemType, ((unsigned char *)buf) + index * size);
	}
	else
	{
		return ((ILObject **)buf)[index];
	}
}

/* Helper function for all array stores */

static int StoreObjectToArray(ILExecThread *thread, ILType * elemType,
								ILObject *value, void* ptr,
								ILObject **location)
{
	/* Copy the value into position at "ptr" */
	if(ILType_IsPrimitive(elemType) || ILType_IsValueType(elemType))
	{
		if(ILExecThreadPromoteAndUnbox(thread, elemType, value, ptr))
		{
			return 1;
		}
	}
	else if(ILTypeAssignCompatible
					(ILProgramItem_Image(thread->method),
				     (value ? ILClassToType(GetObjectClass(value)) : 0),
				     elemType))
	{
		*(location) = value;
		return 1;
	}
	return 0;
}

/*
 * private void Set(Object value, int index1, int index2, int index3);
 */
void _IL_Array_Set_Objectiii(ILExecThread *thread, ILObject *thisObj,
						     ILObject *value, ILInt32 index1,
							 ILInt32 index2, ILInt32 index3)
{
	System_Array *_this = (System_Array *)thisObj;
	ILType *elemType = GetArrayElemType(_this);
	ILUInt32 elemSize = ILSizeOfType(thread, elemType);
	void *ptr;

	if(_ILIsSArray(_this))
	{
		if(index1 < 0 || index1 >= ArrayLength(_this))
		{
			ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return;
		}
		ptr = ((unsigned char *)ArrayToBuffer(_this)) +
			  ((ILUInt32)index1) * elemSize;
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(_this))
	{
		System_MArray *marray = (System_MArray *)_this;
		ILUInt32 offset;
		ILInt32 index;
		if(marray->rank == 1)
		{
			/* Single-dimensional array */
			index = index1 - marray->bounds[0].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[0].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return;
			}
			ptr = ((unsigned char *)(marray->data)) +
				  ((ILUInt32)index) * elemSize;
		}
		else if(marray->rank == 2)
		{
			/* Double-dimensional array */
			index = index1 - marray->bounds[0].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[0].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return;
			}
			offset = index * marray->bounds[0].multiplier;
			index = index2 - marray->bounds[1].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[1].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return;
			}
			offset += (ILUInt32)index;
			ptr = ((unsigned char *)(marray->data)) + offset * elemSize;
		}
		else
		{
			/* Triple-dimensional array */
			index = index1 - marray->bounds[0].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[0].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return;
			}
			offset = index * marray->bounds[0].multiplier;
			index = index2 - marray->bounds[1].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[1].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return;
			}
			offset += index * marray->bounds[1].multiplier;
			index = index3 - marray->bounds[2].lower;
			if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[2].size)))
			{
				ILExecThreadThrowSystem(thread,
										"System.IndexOutOfRangeException",
										"Arg_InvalidArrayIndex");
				return;
			}
			offset += (ILUInt32)index;
			ptr = ((unsigned char *)(marray->data)) + offset * elemSize;
		}
	}
#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */
	else
	{
		ILExecThreadThrowSystem(thread, "System.ArgumentException",
					 		    "Arg_ElementTypeMismatch");
		return;
	}

	/* store to location and be done with it */
	if(!StoreObjectToArray(thread, elemType, value, ptr, ptr))
	{
		ILExecThreadThrowSystem(thread, "System.ArgumentException",
					 		    "Arg_ElementTypeMismatch");
		return;
	}
}

/*
 * private void Set(Object value, int[] indices);
 */
void _IL_Array_Set_Objectai(ILExecThread *thread, ILObject *thisObj,
						    ILObject *value, System_Array *indices)
{
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	System_Array *_this = (System_Array *)thisObj;
	System_MArray *marray;
	ILType *elemType;
	ILUInt32 elemSize;
	ILUInt32 offset;
	ILInt32 dim;
	ILInt32 index;
	void *ptr;
#endif
	ILInt32 *ind = (ILInt32 *)ArrayToBuffer(indices);

	/* Handle the single-dimensional case specially */
	if(ArrayLength(indices) == 1)
	{
		_IL_Array_Set_Objectiii(thread, thisObj, value, ind[0], 0, 0);
		return;
	}

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	/* Get the element type and its size */
	elemType = GetArrayElemType(_this);
	elemSize = ILSizeOfType(thread, elemType);

	/* Find the specific element position within the array */
	marray = (System_MArray *)_this;
	offset = 0;
	for(dim = 0; dim < marray->rank; ++dim)
	{
		index = ind[dim] - marray->bounds[dim].lower;
		if(((ILUInt32)index) >= ((ILUInt32)(marray->bounds[dim].size)))
		{
			ILExecThreadThrowSystem(thread,
									"System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return;
		}
		offset += (ILUInt32)(index * marray->bounds[dim].multiplier);
	}
	ptr = ((unsigned char *)(marray->data)) + offset * elemSize;
	
	if(StoreObjectToArray(thread, elemType, value, ptr, ptr))
	{
		return;
	}
	else	
#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */
	{
		ILExecThreadThrowSystem
				(thread, "System.ArgumentException",
				 "Arg_ElementTypeMismatch");
	}
}

/*
 * private static void SetRelative(Object value, int index);
 */
void _IL_Array_SetRelative(ILExecThread *thread, ILObject *_this,
						   ILObject *value, ILInt32 index)
{
	void *buf;
	ILType *synType;
	ILType *elemType;
	ILInt32 size;

	/* Get the base pointer, element type, and the size of the array */
	if(_ILIsSArray((System_Array *)_this))
	{
		buf = ArrayToBuffer(_this);
		synType = ILClassGetSynType(GetObjectClass(_this));
		elemType = ILType_ElemType(synType);
		size = (ILInt32)(ILSizeOfType(thread, elemType));
	}
	else
	{
	#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
		buf = ((System_MArray *)_this)->data;
		size = ((System_MArray *)_this)->elemSize;
		elemType = GetArrayElemType((System_Array *)_this);
	#else
		return;
	#endif
	}

	if(StoreObjectToArray(thread, elemType, value, 
							((unsigned char *)buf) + index * size,
							&(((ILObject **)buf)[index])))
	{
		return;
	}		
	else
	{
		ILExecThreadThrowSystem
				(thread, "System.ArgumentException",
				 "Arg_ElementTypeMismatch");
	}
}

/*
 * private static void Buffer.Copy(Array src, int srcOffset,
 *								   Array dst, int dstOffset,
 *								   int count);
 */
void _IL_Buffer_Copy(ILExecThread *thread,
					 ILObject *_src, ILInt32 srcOffset,
					 ILObject *_dst, ILInt32 dstOffset,
					 ILInt32 count)
{
	System_Array *src = (System_Array *)_src;
	System_Array *dst = (System_Array *)_dst;
	unsigned char *srcBuffer;
	unsigned char *dstBuffer;
	if(_ILIsSArray(src))
	{
		srcBuffer = ((unsigned char *)(ArrayToBuffer(src))) + srcOffset;
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(src))
	{
		srcBuffer = ((unsigned char *)(((System_MArray *)src)->data)) +
					srcOffset;
	}
#endif
	else
	{
		return;
	}
	if(_ILIsSArray(dst))
	{
		dstBuffer = ((unsigned char *)(ArrayToBuffer(dst))) + dstOffset;
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(dst))
	{
		dstBuffer = ((unsigned char *)(((System_MArray *)dst)->data)) +
					dstOffset;
	}
#endif
	else
	{
		return;
	}
	if(count > 0)
	{
		/* Use "memmove", because the caller may be trying to move
		   an overlapping range of data within the same array */
		ILMemMove(dstBuffer, srcBuffer, count);
	}
}

/*
 * private static int Buffer.GetLength(Array array);
 */
ILInt32 _IL_Buffer_GetLength(ILExecThread *thread, ILObject *_array)
{
	System_Array *array = (System_Array *)_array;
	if(_ILIsSArray(array))
	{
		ILType *synType = ILClassGetSynType(GetObjectClass(array));
		return ArrayLength(array) * ILSizeOfType(thread, ILType_ElemType(synType));
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(array))
	{
		return _IL_Array_GetLength_(thread, _array) *
			   ((System_MArray *)array)->elemSize;
	}
#endif
	else
	{
		return 0;
	}
}

/*
 * private static byte Buffer.GetElement(Array array, int index);
 */
ILUInt8 _IL_Buffer_GetElement(ILExecThread *thread,
						      ILObject *_array, ILInt32 index)
{
	System_Array *array = (System_Array *)_array;
	if(_ILIsSArray(array))
	{
		return ((unsigned char *)(ArrayToBuffer(array)))[index];
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(array))
	{
		return ((unsigned char *)(((System_MArray *)array)->data))[index];
	}
#endif
	else
	{
		return 0;
	}
}

/*
 * private static void Buffer.SetElement(Array array, int index, byte value);
 */
void _IL_Buffer_SetElement(ILExecThread *thread,
						   ILObject *_array,
						   ILInt32 index, ILUInt8 value)
{
	System_Array *array = (System_Array *)_array;
	if(_ILIsSArray(array))
	{
		((unsigned char *)(ArrayToBuffer(array)))[index] = value;
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(array))
	{
		((unsigned char *)(((System_MArray *)array)->data))[index] = value;
	}
#endif
}

int ILExecThreadGetElem(ILExecThread *thread, void *value,
						ILObject *_array, ILInt32 index)
{
	if(_ILIsSArray((System_Array *)_array))
	{
		/* Validate the index */
		System_Array *array = (System_Array *)_array;
		ILType *type;
		ILInt32 elemSize;
		if(index >= 0 && index < ArrayLength(array))
		{
			/* Get the element size */
			type = ILClassGetSynType(GetObjectClass(array));
			elemSize = (ILInt32)(ILSizeOfType(thread, ILType_ElemType(type)));

			/* Copy the element to the "value" buffer */
			ILMemCpy(value, ((unsigned char *)(ArrayToBuffer(_array))) +
							elemSize * index, elemSize);
			return 0;
		}
		else
		{
			/* The array index is out of range */
			ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return 1;
		}
	}
	else
	{
		/* Not a single-dimensional array */
		ILExecThreadThrowSystem(thread, "System.ArrayTypeMismatchException",
							    (const char *)0);
		return 1;
	}
}

#define	ArrayElem(type)		\
			(*((type *)(((unsigned char *)(ArrayToBuffer(array))) + \
						elemSize * index)))

int ILExecThreadSetElem(ILExecThread *thread, ILObject *_array,
						ILInt32 index, ...)
{
	if(_ILIsSArray((System_Array *)_array))
	{
		/* Validate the index */
		System_Array *array = (System_Array *)_array;
		ILType *type;
		ILInt32 elemSize;
		if(index >= 0 && index < ArrayLength(array))
		{
			/* Get the element size */
			type = ILClassGetSynType(GetObjectClass(array));
			type = ILTypeGetEnumType(ILType_ElemType(type));
			elemSize = (ILInt32)(ILSizeOfType(thread, type));

			/* Copy the value to the element */
			{
				IL_VA_START(index);
				if(ILType_IsPrimitive(type))
				{
					/* Primitive type */
					switch(ILType_ToElement(type))
					{
						case IL_META_ELEMTYPE_BOOLEAN:
						case IL_META_ELEMTYPE_I1:
						case IL_META_ELEMTYPE_U1:
						{
							ArrayElem(ILInt8) = (ILInt8)(IL_VA_ARG(va, ILVaInt));
						}
						break;

						case IL_META_ELEMTYPE_I2:
						case IL_META_ELEMTYPE_U2:
						case IL_META_ELEMTYPE_CHAR:
						{
							ArrayElem(ILInt16) = (ILInt16)(IL_VA_ARG(va, ILVaInt));
						}
						break;

						case IL_META_ELEMTYPE_I4:
						case IL_META_ELEMTYPE_U4:
					#ifdef IL_NATIVE_INT32
						case IL_META_ELEMTYPE_I:
						case IL_META_ELEMTYPE_U:
					#endif
						{
							ArrayElem(ILInt32) = (ILInt32)(IL_VA_ARG(va, ILVaInt));
						}
						break;

						case IL_META_ELEMTYPE_I8:
						case IL_META_ELEMTYPE_U8:
					#ifdef IL_NATIVE_INT64
						case IL_META_ELEMTYPE_I:
						case IL_META_ELEMTYPE_U:
					#endif
						{
							ILInt64 int64Value = IL_VA_ARG(va, ILInt64);
							ILMemCpy(&(ArrayElem(ILInt64)), &int64Value,
									 sizeof(ILInt64));
						}
						break;

						case IL_META_ELEMTYPE_R4:
						{
							ArrayElem(ILFloat) =
									(ILFloat)(IL_VA_ARG(va, ILVaDouble));
						}
						break;

						case IL_META_ELEMTYPE_R8:
						{
							ILDouble fValue =
								(ILDouble)(IL_VA_ARG(va, ILVaDouble));
							ILMemCpy(&(ArrayElem(ILDouble)), &fValue,
									 sizeof(fValue));
						}
						break;

						case IL_META_ELEMTYPE_R:
						{
							ILNativeFloat fValue =
								(ILNativeFloat)(IL_VA_ARG(va, ILVaDouble));
							ILMemCpy(&(ArrayElem(ILNativeFloat)), &fValue,
									 sizeof(fValue));
						}
						break;
					}
				}
				else if(ILType_IsValueType(type))
				{
					/* Value type: the caller has passed us a pointer */
					ILMemCpy(&(ArrayElem(ILInt8)),
							 IL_VA_ARG(va, void *), elemSize);
				}
				else
				{
					/* Everything else is a pointer */
					ArrayElem(void *) = IL_VA_ARG(va, void *);
				}
				IL_VA_END;
			}
			return 0;
		}
		else
		{
			/* The array index is out of range */
			ILExecThreadThrowSystem(thread, "System.IndexOutOfRangeException",
									"Arg_InvalidArrayIndex");
			return 1;
		}
	}
	else
	{
		/* Not a single-dimensional array */
		ILExecThreadThrowSystem(thread, "System.ArrayTypeMismatchException",
							    (const char *)0);
		return 1;
	}
}

ILObject *_ILCloneSArray(ILExecThread *thread, System_Array *array)
{
	System_Array *newArray;
	ILType *elemType;
	ILUInt32 elemSize;
	ILUInt32 totalLen;

	/* Get the element type and size */
	elemType = GetArrayElemType(array);
	elemSize = ILSizeOfType(thread, elemType);

	/* Get the total length of the array object */
	totalLen = sizeof(System_Array) + elemSize * ((ILUInt32)(ArrayLength(array)));

	/* Allocate differently for primitive and non-primitive arrays */
	if(ILType_IsPrimitive(elemType) && elemType != ILType_TypedRef)
	{
		newArray = (System_Array *)_ILEngineAllocAtomic
				(thread, GetObjectClass(array), totalLen);
	}
	else
	{
		newArray = (System_Array *)_ILEngineAlloc
				(thread, GetObjectClass(array), totalLen);
	}

	/* Copy the contents of the original array */
	if(newArray)
	{
		ILMemCpy(newArray, array, totalLen);
	}
	return (ILObject *)newArray;
}

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS

ILObject *_ILCloneMArray(ILExecThread *thread, System_MArray *array)
{
	System_MArray *newArray;
	ILType *elemType;
	ILUInt32 elemSize;
	ILUInt32 headerLen;
	ILUInt32 totalLen;
	ILInt32 dim;

	/* Get the element type and size */
	elemType = GetArrayElemType((System_Array *)array);
	elemSize = ILSizeOfType(thread, elemType);

	/* Get the total length of the array header and data */
	headerLen = sizeof(System_MArray) + array->rank * sizeof(MArrayBounds);
	totalLen = elemSize;
	for(dim = 0; dim < array->rank; ++dim)
	{
		totalLen *= (ILUInt32)(array->bounds[dim].size);
	}

	/* Allocate a new array header */
	newArray = (System_MArray *)_ILEngineAlloc
					(thread, GetObjectClass(array), headerLen);
	if(!newArray)
	{
		return 0;
	}
	ILMemCpy(newArray, array, headerLen);

	/* Allocate the data differently for primitive and non-primitive arrays */
	if(ILType_IsPrimitive(elemType) && elemType != ILType_TypedRef)
	{
		newArray->data = (System_Array *)_ILEngineAllocAtomic
				(thread, 0, totalLen);
	}
	else
	{
		newArray->data = (System_Array *)_ILEngineAlloc(thread, 0, totalLen);
	}

	/* Copy the contents of the original array */
	if(newArray->data)
	{
		ILMemCpy(newArray->data, array->data, totalLen);
		return (ILObject *)newArray;
	}
	else
	{
		return 0;
	}
}

/*
 * public virtual Object Array.Clone();
 */
ILObject * _IL_Array_Clone(ILExecThread * _thread, ILObject * _this)
{
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	/* Test for arrays, which must be cloned differently */
	if(_ILIsSArray((System_Array *)_this))
	{
		return _ILCloneSArray(_thread, (System_Array *)_this);
	}
	return _ILCloneMArray(_thread, (System_MArray *)_this);
#else
	return _ILCloneSArray(_thread, (System_Array *)_this);
#endif

}

#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */

#ifdef	__cplusplus
};
#endif
