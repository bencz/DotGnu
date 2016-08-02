/*
 * lib_misc.c - Internalcall methods for misc "System" classes.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static Guid NewGuid();
 */
void _IL_Guid_NewGuid(ILExecThread *thread, void *result)
{
	ILGUIDGenerate((unsigned char *)result);
}

/*
 * private static bool GetLittleEndian();
 */
ILBool _IL_BitConverter_GetLittleEndian(ILExecThread *thread)
{
#if defined(__i386) || defined(__i386__)
	return 1;
#else
	union
	{
		unsigned char bytes[4];
		ILUInt32      value;

	} convert;
	convert.value = (ILUInt32)0x01020304;
	return (convert.bytes[0] == (unsigned char)0x04);
#endif
}

/*
 * public static long DoubleToInt64Bits(double value);
 */
ILInt64 _IL_BitConverter_DoubleToInt64Bits(ILExecThread *thread,
										   ILDouble value)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	union
	{
		ILDouble input;
		ILInt64  output;

	} convert;
	convert.input = value;
	return convert.output;
#else
	ILExecThreadThrowSystem(thread, "System.NotImplementedException", 0);
	return 0;
#endif
}

/*
 * public static double Int64BitsToDouble(long value);
 */
ILDouble _IL_BitConverter_Int64BitsToDouble(ILExecThread *thread,
											ILInt64 value)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	union
	{
		ILInt64  input;
		ILDouble output;

	} convert;
	convert.input = value;
	return convert.output;
#else
	ILExecThreadThrowSystem(thread, "System.NotImplementedException", 0);
	return 0;
#endif
}

/*
 * public static int FloatToInt32Bits(float value);
 */
ILInt32 _IL_BitConverter_FloatToInt32Bits(ILExecThread *thread,
										  ILFloat value)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	union
	{
		ILFloat input;
		ILInt32 output;

	} convert;
	convert.input = value;
	return convert.output;
#else
	ILExecThreadThrowSystem(thread, "System.NotImplementedException", 0);
	return 0;
#endif
}

/*
 * public static float Int32BitsToFloat(int value);
 */
ILFloat _IL_BitConverter_Int32BitsToFloat(ILExecThread *thread,
										  ILInt32 value)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	union
	{
		ILInt32 input;
		ILFloat output;

	} convert;
	convert.input = value;
	return convert.output;
#else
	ILExecThreadThrowSystem(thread, "System.NotImplementedException", 0);
	return 0;
#endif
}

/*
 * internal static byte[] GetLittleEndianBytes(float value);
 */
System_Array *_IL_BitConverter_GetLittleEndianBytes_f
		(ILExecThread *_thread, ILFloat value)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	System_Array *array =
		(System_Array *)ILExecThreadNew(_thread, "[B", "(Ti)V", (ILVaInt)4);
	if(array)
	{
		IL_WRITE_FLOAT((unsigned char *)ArrayToBuffer(array), value);
	}
	return array;
#else	/* !IL_CONFIG_FP_SUPPORTED */
	ILExecThreadThrowSystem(_thread, "System.NotImplementedException", 0);
	return 0;
#endif	/* !IL_CONFIG_FP_SUPPORTED */
}

/*
 * internal static byte[] GetLittleEndianBytes(double value);
 */
System_Array *_IL_BitConverter_GetLittleEndianBytes_d
		(ILExecThread *_thread, ILDouble value)
{
#ifdef IL_CONFIG_FP_SUPPORTED
	System_Array *array =
		(System_Array *)ILExecThreadNew(_thread, "[B", "(Ti)V", (ILVaInt)8);
	if(array)
	{
		IL_WRITE_DOUBLE((unsigned char *)ArrayToBuffer(array), value);
	}
	return array;
#else	/* !IL_CONFIG_FP_SUPPORTED */
	ILExecThreadThrowSystem(_thread, "System.NotImplementedException", 0);
	return 0;
#endif	/* !IL_CONFIG_FP_SUPPORTED */
}

#ifdef IL_CONFIG_VARARGS

/*
 * Structure of an "ArgIterator" object.  Variable arguments
 * are packed into "Object[]" arrays by the caller, from where
 * they can be accessed using this iterator type.
 */
typedef struct
{
	union
	{
		/* Fields of interest to us internally */
		struct
		{
			System_Array *args;
			ILInt32       posn;

		} argIter;

		/* Pad the structure to its declared length in the C# library */
		struct
		{
			ILInt32 cookie1;
			ILInt32 cookie2;
			ILInt32 cookie3;
			ILInt32 cookie4;

		} dummy;

	} un;

} ArgIterator;

/*
 * public ArgIterator(RuntimeArgumentHandle argList);
 */
void _IL_ArgIterator_ctor_RuntimeArgumentHandle(ILExecThread *_thread,
												void *_this, void *argList)
{
	ArgIterator *iter = (ArgIterator *)_this;
#ifdef IL_USE_JIT
	iter->un.argIter.args = (System_Array *)((ILObject *)argList);
#else
	iter->un.argIter.args = (System_Array *)(*((ILObject **)argList));
#endif
	iter->un.argIter.posn = 0;
}

/*
 * public ArgIterator(RuntimeArgumentHandle argList, void *ptr);
 */
void _IL_ArgIterator_ctor_RuntimeArgumentHandlepV(ILExecThread *_thread,
												  void *_this, void *argList,
												  void *ptr)
{
	/* We don't use the pointer form in this implementation,
	   because it is extremely dangerous */
	_IL_ArgIterator_ctor_RuntimeArgumentHandle(_thread, _this, argList);
}

/*
 * public void End();
 */
void _IL_ArgIterator_End(ILExecThread *_thread, void *_this)
{
	ArgIterator *iter = (ArgIterator *)_this;
	if(iter->un.argIter.args)
	{
		iter->un.argIter.posn = ArrayLength(iter->un.argIter.args);
	}
}

/*
 * public TypedReference GetNextArg();
 */
ILTypedRef _IL_ArgIterator_GetNextArg_(ILExecThread *_thread, void *_this)
{
	ArgIterator *iter = (ArgIterator *)_this;
	ILTypedRef ref;
	ILObject **object;
	ILClass *classInfo;

	if(iter->un.argIter.args &&
	   iter->un.argIter.posn < ArrayLength(iter->un.argIter.args))
	{
		/* Extract the next object and unpack it */
		object = &(((ILObject **)ArrayToBuffer(iter->un.argIter.args))
						[(iter->un.argIter.posn)++]);
		if(*object)
		{
			/* Determine if this is an object or a value type */
			classInfo = GetObjectClass(*object);
			if(!ILClassIsValueType(classInfo))
			{
				/* Object reference */
				ref.type = classInfo;
				ref.value = (void *)object;
			}
			else
			{
				/* Value type reference */
				ref.type = classInfo;
				ref.value = (void *)(*object);
			}
		}
		else
		{
			/* Points at a "null" object reference */
			ref.type = _thread->process->objectClass;
			ref.value = (void *)object;
		}
	}
	else
	{
		/* We've reached the end of the argument list */
		ILExecThreadThrowSystem(_thread, "System.InvalidOperationException",
								"Invalid_BadEnumeratorPosition");
		ref.type = 0;
		ref.value = 0;
	}
	return ref;
}

/*
 * Convert a type into its primitive form, ignoring slight differences
 * in type that don't matter because we can blindly cast between the
 * equivalents without losing type-safety.
 */
static int ArgTypeToPrimitive(ILType *type)
{
	if(ILType_IsPrimitive(type))
	{
		switch(ILType_ToElement(type))
		{
			case IL_META_ELEMTYPE_BOOLEAN:
			case IL_META_ELEMTYPE_I1:
			case IL_META_ELEMTYPE_U1:
				return IL_META_ELEMTYPE_I1;

			case IL_META_ELEMTYPE_I2:
			case IL_META_ELEMTYPE_U2:
			case IL_META_ELEMTYPE_CHAR:
				return IL_META_ELEMTYPE_I2;

			case IL_META_ELEMTYPE_I4:
			case IL_META_ELEMTYPE_U4:
		#ifdef IL_NATIVE_INT32
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
				return IL_META_ELEMTYPE_I4;

			case IL_META_ELEMTYPE_I8:
			case IL_META_ELEMTYPE_U8:
		#ifdef IL_NATIVE_INT64
			case IL_META_ELEMTYPE_I:
			case IL_META_ELEMTYPE_U:
		#endif
				return IL_META_ELEMTYPE_I8;

			case IL_META_ELEMTYPE_R4:
				return IL_META_ELEMTYPE_R4;

			case IL_META_ELEMTYPE_R8:
			case IL_META_ELEMTYPE_R:
				return IL_META_ELEMTYPE_R8;

			default: break;
		}
		return IL_META_ELEMTYPE_END;
	}
	else if(ILType_IsValueType(type))
	{
		if(ILTypeIsEnum(type))
		{
			return ArgTypeToPrimitive(ILTypeGetEnumType(type));
		}
		else
		{
			return IL_META_ELEMTYPE_VALUETYPE;
		}
	}
	else if(ILType_IsPointer(type))
	{
	#ifdef IL_NATIVE_INT32
		return IL_META_ELEMTYPE_I4;
	#else
		return IL_META_ELEMTYPE_I8;
	#endif
	}
	else
	{
		return IL_META_ELEMTYPE_END;
	}
}

/*
 * public TypedReference GetNextArg(RuntimeTypeHandle type);
 *
 * Note: this version is typically used by unmanaged C++ code, where
 * it is expected that the next argument is of a particular type.
 * We check the type and return the value.  This is a little stricter
 * than other implementations, but it is also a lot safer.
 */
ILTypedRef _IL_ArgIterator_GetNextArg_RuntimeTypeHandle(ILExecThread *_thread,
														void *_this,
														void *type)
{
	void *actualType = *((void **)type);
	int actualTypePrim;
	ILTypedRef ref;

	/* Convert the actual type into its primitive form */
	actualTypePrim = ArgTypeToPrimitive
		(ILClassToType((ILClass *)actualType));

	/* Get the next reference from the argument list */
	ref = _IL_ArgIterator_GetNextArg_(_thread, _this);
	if(!(ref.type))
	{
		/* An exception was thrown at the end of the list */
		return ref;
	}

	/* Convert the argument type into its primitive form and compare */
	if(ArgTypeToPrimitive(ILClassToType((ILClass *)(ref.type)))
			!= actualTypePrim || actualTypePrim == IL_META_ELEMTYPE_END)
	{
		ILExecThreadThrowSystem(_thread, "System.InvalidOperationException",
								"Invalid_BadEnumeratorPosition");
		ref.type = 0;
		ref.value = 0;
		return ref;
	}
	if(actualTypePrim == IL_META_ELEMTYPE_VALUETYPE && ref.type != actualType)
	{
		ILExecThreadThrowSystem(_thread, "System.InvalidOperationException",
								"Invalid_BadEnumeratorPosition");
		ref.type = 0;
		ref.value = 0;
		return ref;
	}

	/* Convert the typed reference into the requested type and return it */
	ref.type = actualType;
	return ref;
}

/*
 * public RuntimeTypeHandle GetNextArgType();
 */
void _IL_ArgIterator_GetNextArgType(ILExecThread *_thread,
									void *_result, void *_this)
{
	ArgIterator *iter = (ArgIterator *)_this;
	ILObject *object;

	if(iter->un.argIter.args &&
	   iter->un.argIter.posn < ArrayLength(iter->un.argIter.args))
	{
		/* Extract the next object and determine its type */
		object = ((ILObject **)ArrayToBuffer(iter->un.argIter.args))
						[iter->un.argIter.posn];
		if(object)
		{
			*((ILClass **)_result) = GetObjectClass(object);
		}
		else
		{
			/* Points at a "null" object reference */
			*((ILClass **)_result) = _thread->process->objectClass;
		}
	}
	else
	{
		/* We've reached the end of the argument list */
		ILExecThreadThrowSystem(_thread, "System.InvalidOperationException",
								"Invalid_BadEnumeratorPosition");
		*((ILClass **)_result) = _thread->process->objectClass;
	}
}

/*
 * public int GetRemainingCount();
 */
ILInt32 _IL_ArgIterator_GetRemainingCount(ILExecThread *_thread,
										  void *_this)
{
	ArgIterator *iter = (ArgIterator *)_this;
	if(iter->un.argIter.args)
	{
		return ArrayLength(iter->un.argIter.args) - iter->un.argIter.posn;
	}
	else
	{
		return 0;
	}
}

/*
 * private static TypedReference ClrMakeTypedReference(Object target,
 *													   FieldInfo[] flds);
 */
ILTypedRef _IL_TypedReference_ClrMakeTypedReference(ILExecThread *_thread,
													ILObject *target,
													System_Array *flds)
{
	ILClass *classInfo;
	ILInt32 index;
	ILUInt32 offset;
	ILTypedRef ref;
	ILField *field;

	/* Get the initial class and offset */
	classInfo = GetObjectClass(target);
	offset = 0;

	/* Resolve the fields within the object, level by level */
	for(index = 0; index < ArrayLength(flds); ++index)
	{
		field = *((ILField **)(((ILObject **)(ArrayToBuffer(flds)))[index]));
		if(!field)
		{
			ILExecThreadThrowSystem
				(_thread, "System.ArgumentException",
				 "Arg_MakeTypedRefFields");
			ref.type = 0;
			ref.value = 0;
			return ref;
		}
		offset += field->offset;
		classInfo = ILClassFromType
			(ILContextNextImage(_thread->process->context, 0),
			 0, ILField_Type(field), 0);
		classInfo = ILClassResolve(classInfo);
		if(!classInfo ||
		   (index < (ArrayLength(flds) - 1) && !ILClassIsValueType(classInfo)))
		{
			ILExecThreadThrowSystem
				(_thread, "System.ArgumentException",
				 "Arg_MakeTypedRefFields");
			ref.type = 0;
			ref.value = 0;
			return ref;
		}
	}

	/* Populate the typed reference */
	ref.type = (void *)classInfo;
	ref.value = (void *)(((unsigned char *)target) + offset);
	return ref;
}

/*
 * public static bool ClrSetTypedReference(TypedReference target,
 *										   Object value);
 */
ILBool _IL_TypedReference_ClrSetTypedReference(ILExecThread *_thread,
										       ILTypedRef target,
										       ILObject *value)
{
	ILType *type;
	if(target.type == 0 || target.value == 0)
	{
		/* This is an invalid typed reference */
		return 0;
	}
	type = ILClassToType(target.type);
	if(ILType_IsPrimitive(type) || ILType_IsValueType(type))
	{
		if(!ILExecThreadUnbox(_thread, type, value, target.value))
		{
			return 0;
		}
		return 1;
	}
	else if(ILTypeAssignCompatible
				(ILProgramItem_Image(_thread->method),
			     (value ? ILClassToType(GetObjectClass(value)) : 0),
			     type))
	{
		*((ILObject **)(target.value)) = value;
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * public static Object ToObject();
 */
ILObject *_IL_TypedReference_ToObject(ILExecThread *_thread, ILTypedRef value)
{
	if(value.type && value.value)
	{
		if(!ILClassIsValueType((ILClass *)(value.type)))
		{
			/* Refers to an object reference which is returned as-is */
			return *((ILObject **)(value.value));
		}
		else
		{
			/* Refers to a value type instance which should be boxed */
			return ILExecThreadBox
				(_thread, ILClassToType((ILClass *)(value.type)), value.value);
		}
	}
	else
	{
		return 0;
	}
}

#endif /* IL_CONFIG_VARARGS */

#ifdef	__cplusplus
};
#endif
