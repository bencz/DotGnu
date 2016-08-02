/*
 * box.c - Box and unbox operations.
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

ILObject *ILExecThreadBoxNoValue(ILExecThread *thread, ILType *type)
{
	ILClass *classInfo;
	ILObject *object;
	ILUInt32 typeSize;
	if(ILType_IsPrimitive(type) || ILType_IsValueType(type))
	{
		classInfo = ILClassFromType
			(ILContextNextImage(thread->process->context, 0),
			 0, type, 0);
		if(!classInfo)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		classInfo = ILClassResolve(classInfo);
		typeSize = ILSizeOfType(thread, type);
		object = (ILObject *)_ILEngineAlloc(thread, classInfo, typeSize);
		if(!object)
		{
			return 0;
		}
		return object;
	}
	else
	{
		return 0;
	}
}

ILObject *ILExecThreadBox(ILExecThread *thread, ILType *type, void *ptr)
{
	ILClass *classInfo;
	ILObject *object;
	ILUInt32 typeSize;
	if(ILType_IsPrimitive(type) || ILType_IsValueType(type))
	{
		classInfo = ILClassFromType
			(ILContextNextImage(thread->process->context, 0),
			 0, type, 0);
		if(!classInfo)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		classInfo = ILClassResolve(classInfo);
		typeSize = ILSizeOfType(thread, type);
		object = (ILObject *)_ILEngineAlloc(thread, classInfo, typeSize);
		if(object)
		{
			ILMemCpy(object, ptr, typeSize);
		}
		else
		{
			return 0;
		}
		return object;
	}
	else
	{
		return 0;
	}
}

ILObject *ILExecThreadBoxFloat(ILExecThread *thread, ILType *type, void *ptr)
{
	ILClass *classInfo;
	ILObject *object;
	ILUInt32 typeSize;
	if(ILType_IsPrimitive(type) || ILType_IsValueType(type))
	{
		classInfo = ILClassFromType
			(ILContextNextImage(thread->process->context, 0),
			 0, type, 0);
		if(!classInfo)
		{
			ILExecThreadThrowOutOfMemory(thread);
			return 0;
		}
		classInfo = ILClassResolve(classInfo);
		typeSize = ILSizeOfType(thread, type);
		object = (ILObject *)_ILEngineAlloc(thread, classInfo, typeSize);
		if(object)
		{
			if(type == ILType_Float32)
			{
				*((ILFloat *)object) = (ILFloat)(*((ILNativeFloat *)ptr));
			}
			else if(type == ILType_Float64)
			{
				*((ILDouble *)object) = (ILDouble)(*((ILNativeFloat *)ptr));
			}
			else
			{
				ILMemCpy(object, ptr, typeSize);
			}
		}
		return object;
	}
	else
	{
		return 0;
	}
}

int ILExecThreadUnbox(ILExecThread *thread, ILType *type,
					  ILObject *object, void *ptr)
{
	if(object && ptr &&
	   (ILType_IsPrimitive(type) || ILType_IsValueType(type)) &&
	   ILTypeIdentical(type, ILClassToType(GetObjectClass(object))))
	{
		ILMemCpy(ptr, object, ILSizeOfType(thread, type));
		return 1;
	}
	else
	{
		return 0;
	}
}

int ILExecThreadUnboxFloat(ILExecThread *thread, ILType *type,
					       ILObject *object, void *ptr)
{
	if(object && ptr &&
	   (ILType_IsPrimitive(type) || ILType_IsValueType(type)) &&
	   ILTypeIdentical(type, ILClassToType(GetObjectClass(object))))
	{
		if(type == ILType_Float32)
		{
			*((ILNativeFloat *)ptr) = (ILNativeFloat)(*((ILFloat *)object));
		}
		else if(type == ILType_Float64)
		{
			*((ILNativeFloat *)ptr) = (ILNativeFloat)(*((ILDouble *)object));
		}
		else
		{
			ILMemCpy(ptr, object, ILSizeOfType(thread, type));
		}
		return 1;
	}
	else
	{
		return 0;
	}
}

#define CHECK_WIDTH(value) \
	do\
	{\
		if(elemSize < valueSize + (value))\
		{\
			return 0;\
		}\
	}\
	while(0)
	
#define PROMOTE_UNSIGNED(cast) \
		switch(ILType_ToElement(objtype)) \
		{\
			case IL_META_ELEMTYPE_U1:\
			case IL_META_ELEMTYPE_U2:\
			case IL_META_ELEMTYPE_U4:\
			case IL_META_ELEMTYPE_U8: \
			{\
				CHECK_WIDTH(0);\
				*((cast*)ptr)=(cast) u8;\
				return 1;\
			}\
			break;\
			default:\
			{\
				return 0;\
			}\
			break;\
		}

#define PROMOTE_SIGNED(cast)\
		switch(ILType_ToElement(objtype)) \
		{\
			case IL_META_ELEMTYPE_U1:\
			case IL_META_ELEMTYPE_U2:\
			case IL_META_ELEMTYPE_U4:\
			case IL_META_ELEMTYPE_U8: \
			{\
				CHECK_WIDTH(1);\
				*((cast*)ptr)=(cast) u8;\
				return 1;\
			}\
			break;\
			case IL_META_ELEMTYPE_I1:\
			case IL_META_ELEMTYPE_I2:\
			case IL_META_ELEMTYPE_I4:\
			case IL_META_ELEMTYPE_I8: \
			{\
				CHECK_WIDTH(0);\
				*((cast*)ptr)=(cast) i8;\
				return 1;\
			}\
			break;\
			default:\
			{\
				return 0;\
			}\
			break;\
		}

#define PROMOTE_REAL(cast)\
		switch(ILType_ToElement(objtype)) \
		{\
			case IL_META_ELEMTYPE_U1:\
			case IL_META_ELEMTYPE_U2:\
			case IL_META_ELEMTYPE_U4:\
			case IL_META_ELEMTYPE_U8: \
			{\
				*((cast*)ptr)= (cast) u8;\
				return 1;\
			}\
			break;\
			case IL_META_ELEMTYPE_I1:\
			case IL_META_ELEMTYPE_I2:\
			case IL_META_ELEMTYPE_I4:\
			case IL_META_ELEMTYPE_I8: \
			{\
				*((cast*)ptr)= (cast) i8;\
				return 1;\
			}\
			break;\
			case IL_META_ELEMTYPE_R4:\
			case IL_META_ELEMTYPE_R8:\
			{\
				*((cast*)ptr)=(cast)r8;\
			}\
			default:\
			{\
				return 0;\
			}\
			break;\
		}


int ILExecThreadPromoteAndUnbox(ILExecThread *thread, ILType *type,
					       ILObject *object, void *ptr)
{
	ILInt64 i8=0;
	ILUInt64 u8=0;
	ILDouble r8=0;

	ILType *objtype;
	int elemSize, valueSize;

	if(!ptr || !object) return 0;
	/* because we're never dealing with null stuff or invalid data */
	
	/* handle all the regular stuff this way */
	if(ILExecThreadUnbox(thread, type, object, ptr))
	{
		return 1;
	}

	objtype= ILClassToType(GetObjectClass(object));

	elemSize=ILSizeOfType(thread,type);
	valueSize=ILSizeOfType(thread, objtype);

	/* try promoting this thing */

	switch(ILType_ToElement(objtype))
	{
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			return 0; /* boolean cannot be promoted */
		}
		break;

		case IL_META_ELEMTYPE_I1:
		{
			i8 = (ILInt64)(*(ILInt8*)object);
		}
		break;
		
		case IL_META_ELEMTYPE_U1:
		{
			u8 = (ILUInt64)(*(ILUInt8*)object);
		}
		break;
		
		case IL_META_ELEMTYPE_I2:
		{
			i8 = (ILInt64)(*(ILInt16*)object);
		}
		break;
		
		case IL_META_ELEMTYPE_CHAR:
		case IL_META_ELEMTYPE_U2:
		{
			u8 = (ILUInt64)(*(ILUInt16*)object);
		}
		break;
		
		case IL_META_ELEMTYPE_I4:
		{
			i8 = (ILInt64)(*(ILInt32*)object);
		}
		break;
		
		case IL_META_ELEMTYPE_U4:
		{
			u8 = (ILUInt64)(*(ILUInt32*)object);
		}
		break;
		
		case IL_META_ELEMTYPE_I8:
		{
			i8 = *(ILInt64*)object;
		}
		break;
		
		case IL_META_ELEMTYPE_U8:
		{
			u8 = *(ILUInt64*)object;
		}
		break;

		case IL_META_ELEMTYPE_R4:
		{
			r8 = (ILDouble)(*(ILFloat*)object);
		}
		break;

		case IL_META_ELEMTYPE_R8:
		{
			r8 = *(ILDouble*)object;
		}
		
		default:
		{
			return 0;
		}
		break;
	}

	/* downpromote based on conversion */
	switch(ILType_ToElement(type))
	{
		case IL_META_ELEMTYPE_BOOLEAN:
		{
			return 0; /* boolean cannot be promoted */
		}
		break;

		case IL_META_ELEMTYPE_I1:
		{
			PROMOTE_SIGNED(ILInt8);
		}
		break;
		
		case IL_META_ELEMTYPE_U1:
		{
			PROMOTE_UNSIGNED(ILUInt8);
		}
		break;
		
		case IL_META_ELEMTYPE_I2:
		{
			PROMOTE_SIGNED(ILInt16);
		}
		break;
		
		case IL_META_ELEMTYPE_CHAR:
		case IL_META_ELEMTYPE_U2:
		{
			PROMOTE_UNSIGNED(ILUInt16);
		}
		break;
		
		case IL_META_ELEMTYPE_I4:
		{
			PROMOTE_SIGNED(ILInt32);
		}
		break;
		
		case IL_META_ELEMTYPE_U4:
		{
			PROMOTE_UNSIGNED(ILUInt32);
		}
		break;
		
		case IL_META_ELEMTYPE_I8:
		{
			PROMOTE_SIGNED(ILInt64);
		}
		break;
		
		case IL_META_ELEMTYPE_U8:
		{
			PROMOTE_UNSIGNED(ILUInt64);
		}
		break;

		case IL_META_ELEMTYPE_R4:
		{
			PROMOTE_REAL(ILFloat);
		}
		break;

		case IL_META_ELEMTYPE_R8:
		{
			PROMOTE_REAL(ILDouble);
		}
		break;
		
		default:
		{
			return 0;
		}
		break;
	}
}

#ifdef	__cplusplus
};
#endif
