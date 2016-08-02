/*
 * lib_helpers.c - Internalcall methods for "System.Runtime.CompilerServices".
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

#include "engine.h"
#include "lib_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static void InitializeArray(Array array, RuntimeFieldHandle field);
 */
void _IL_RuntimeHelpers_InitializeArray(ILExecThread *thread,
										ILObject *_array, void *handle)
{
	System_Array *array = (System_Array *)_array;
	ILField *field;
	ILFieldRVA *fieldRVA;
	ILUInt32 rva;
#ifndef IL_USE_JIT
	ILMethod *caller;
#endif
	ILImage *image;
	void *addr;
	unsigned long maxLen;
	ILType *elemType;
	void *buffer;
	ILUInt32 maxSize;
	ILUInt32 elemSize;

	/* Validate the parameters */
	if(!array)
	{
		return;
	}
#ifdef IL_USE_JIT
	field = (ILField *)handle;
#else
	field = *((ILField **)handle);
#endif
	if(!field)
	{
		return;
	}

	/* Make sure that the field is static, RVA, and in the caller's image */
	if(!ILField_IsStatic(field))
	{
		return;
	}
	fieldRVA = ILFieldRVAGetFromOwner(field);
	if(!fieldRVA)
	{
		return;
	}
	rva = ILFieldRVAGetRVA(fieldRVA);
	if(!rva)
	{
		return;
	}
	image = ILProgramItem_Image(field);
#ifndef IL_USE_JIT
	caller = ILExecThreadStackMethod(thread, 1);
	if(!caller || ILProgramItem_Image(caller) != image)
	{
		return;
	}
#endif

	/* Map the RVA to find the array data */
	addr = ILImageMapRVA(image, rva, &maxLen);
	if(!addr)
	{
		return;
	}

	/* Get the element type and make sure that it is primitive and numeric */
	elemType = ILClassGetSynType(GetObjectClass(array));
	elemType = ILTypeGetElemType(elemType);
	switch((unsigned long)elemType)
	{
		case (unsigned long)ILType_Boolean:
		case (unsigned long)ILType_Int8:
		case (unsigned long)ILType_UInt8:
		{
			elemSize = 1;
		}
		break;

		case (unsigned long)ILType_Int16:
		case (unsigned long)ILType_UInt16:
		case (unsigned long)ILType_Char:
		{
			elemSize = 2;
		}
		break;

		case (unsigned long)ILType_Int32:
		case (unsigned long)ILType_UInt32:
		case (unsigned long)ILType_Float32:
		{
			elemSize = 4;
		}
		break;

		case (unsigned long)ILType_Int64:
		case (unsigned long)ILType_UInt64:
		case (unsigned long)ILType_Float64:
		{
			elemSize = 8;
		}
		break;

		default: return;
	}

	/* Determine where the array starts, and its extent */
	if(_ILIsSArray(array))
	{
		buffer = ArrayToBuffer(array);
		maxSize = (ILUInt32)(ArrayLength(array) * elemSize);
	}
#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
	else if(_ILIsMArray(array))
	{
		System_MArray *marray = (System_MArray *)array;
		ILInt32 dim;
		buffer = marray->data;
		maxSize = elemSize;
		for(dim = 0; dim < marray->rank; ++dim)
		{
			maxSize *= (ILUInt32)(marray->bounds[dim].size);
		}
	}
#endif
	else
	{
		return;
	}

	/* Make sure that the program section has sufficient bytes available */
	if(((unsigned long)maxSize) > maxLen)
	{
		return;
	}

	/* Copy the data from the program section to the array */
	switch((unsigned long)elemType)
	{
		case (unsigned long)ILType_Boolean:
		{
			unsigned char *dest = (unsigned char *)buffer;
			unsigned char *src = (unsigned char *)addr;
			while(maxSize > 0)
			{
				*dest++ = (*src++ != 0);
				--maxSize;
			}
		}
		break;

		case (unsigned long)ILType_Int8:
		case (unsigned long)ILType_UInt8:
		{
			ILMemCpy(buffer, addr, maxSize);
		}
		break;

		case (unsigned long)ILType_Int16:
		case (unsigned long)ILType_UInt16:
		case (unsigned long)ILType_Char:
		{
			ILUInt16 *dest = (ILUInt16 *)buffer;
			unsigned char *src = (unsigned char *)addr;
			while(maxSize > 0)
			{
				*dest++ = IL_READ_UINT16(src);
				src += 2;
				maxSize -= 2;
			}
		}
		break;

		case (unsigned long)ILType_Int32:
		case (unsigned long)ILType_UInt32:
		{
			ILUInt32 *dest = (ILUInt32 *)buffer;
			unsigned char *src = (unsigned char *)addr;
			while(maxSize > 0)
			{
				*dest++ = IL_READ_UINT32(src);
				src += 4;
				maxSize -= 4;
			}
		}
		break;

		case (unsigned long)ILType_Int64:
		case (unsigned long)ILType_UInt64:
		{
			unsigned char *dest = (unsigned char *)buffer;
			unsigned char *src = (unsigned char *)addr;
			ILUInt64 value;
			while(maxSize > 0)
			{
				value = IL_READ_UINT64(src);
				ILMemCpy(dest, &value, sizeof(ILUInt64));
				src += 8;
				dest += sizeof(ILUInt64);
				maxSize -= 8;
			}
		}
		break;

	#ifdef IL_CONFIG_FP_SUPPORTED
		case (unsigned long)ILType_Float32:
		{
			ILFloat *dest = (ILFloat *)buffer;
			unsigned char *src = (unsigned char *)addr;
			while(maxSize > 0)
			{
				*dest++ = IL_READ_FLOAT(src);
				src += 4;
				maxSize -= 4;
			}
		}
		break;

		case (unsigned long)ILType_Float64:
		{
			unsigned char *dest = (unsigned char *)buffer;
			unsigned char *src = (unsigned char *)addr;
			ILDouble value;
			while(maxSize > 0)
			{
				value = IL_READ_DOUBLE(src);
				ILMemCpy(dest, &value, sizeof(ILDouble));
				src += 8;
				dest += sizeof(ILDouble);
				maxSize -= 8;
			}
		}
		break;
	#endif /* IL_CONFIG_FP_SUPPORTED */
	}
}

void _IL_RuntimeHelpers_RunClassConstructor(ILExecThread *thread, void *type)
{
#ifdef IL_USE_JIT
	ILClass *classInfo = (ILClass *)type;
#else
	ILClass *classInfo = *((ILClass **)type);
#endif
	ILCoder *coder = _ILExecThreadProcess(thread)->coder;

	ILCoderRunCCtor(coder, classInfo);
}

ILInt32 _IL_RuntimeHelpers_InternalOffsetToStringData(ILExecThread *thread)
{
	return (ILInt32)(ILNativeInt)(StringToBuffer(0));
}

ILObject *_IL_RuntimeHelpers_GetObjectValue(ILExecThread *_thread,
										    ILObject *obj)
{
	ILType *type;
	if(obj)
	{
		/* If the object is a reference type or an immutable primitive
		   value type, then there is nothing to do.  Otherwise we need
		   to make a copy of the value type instance just in case the
		   original object is modified */
		type = ILClassToType(GetObjectClass(obj));
		if(ILType_IsValueType(type) && !ILTypeIsEnum(type))
		{
			return _IL_Object_MemberwiseClone(_thread, obj);
		}
	}
	return obj;
}

/*
 * public static bool Equals(Object o1, Object o2);
 */
ILBool _IL_RuntimeHelpers_Equals(ILExecThread *_thread,
								 ILObject *o1, ILObject *o2)
{
	if(o1 == o2)
	{
		return 1;
	}
	else if(!o1 || !o2)
	{
		return 0;
	}
	else
	{
		return _IL_Object_Equals(_thread, o1, o2);
	}
}

ILInt32 _IL_RuntimeHelpers_GetHashCode(ILExecThread *_thread, ILObject *o1)
{
	if(!o1)
	{
		return 0;
	}
	else
	{
		return _IL_Object_GetHashCode(_thread, o1);
	}
}

#ifdef	__cplusplus
};
#endif
