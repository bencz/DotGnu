/*
 * lib_defs.h - Definitions for the builtin class library.
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

#ifndef	_ENGINE_LIB_DEFS_H
#define	_ENGINE_LIB_DEFS_H

#include "il_decimal.h"
#include "engine.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Comment from Thong Nguyen (tum@veridicus.com)
 *
 * New object layout is like this:
 *
 * [Object Header][Managed Object Data]
 * ^                   ^
 * |                    |
 * |                    |____ILObject 
 * |
 * |_____GcBase & Start of ObjectHeader
 *
 *
 * Use GetMemPtr or GetObjectHeader to get the GcBase/ObjectHeader
 * from an (ILObject *).
 *
 * Use GetObjectFromGcBase from get an (ILObject *) from a (void *).
 */

/*
 *	The size of the object header in bytes.
 */
#define IL_OBJECT_HEADER_SIZE \
	((sizeof(ILObjectHeader) + IL_BEST_ALIGNMENT - 1) & ~(IL_BEST_ALIGNMENT - 1))

/*
 *	Gets a pointer to the object header from an object pointer.
 */
#define GetObjectHeader(obj) \
	(((ILObjectHeader *)(((unsigned char *)(obj)) - IL_OBJECT_HEADER_SIZE)))

/*
 *	Gets a pointer to the start of an object's memory.
 * (Same as GetObjectHeader since the header is the first thing in memory)
 */
#define GetObjectGcBase(obj) \
	((void *)GetObjectHeader(obj))

/*
 *	Gets an object pointer from a pointer to the object's first byte of memory.
 */
#define GetObjectFromGcBase(ptr) \
	((ILObject *)(((unsigned char *)ptr) + IL_OBJECT_HEADER_SIZE))

/*
 * Gets a pointer to the ILClassPrivate information that is associated with
 * a non-null object.
 */
#define	GetObjectClassPrivate(obj) 	\
	(GetObjectHeader(obj)->classPrivate)

#define	SetObjectClassPrivate(obj, value) 	\
	(GetObjectHeader(obj)->classPrivate) = value;

/*
 * Get the class that is associated with a non-null object.
 */
#define	GetObjectClass(obj)	((GetObjectClassPrivate((obj)))->classInfo)

/*
 * See engine/lib_monitor.c for notes on the monitor algorithm.
 */

/* The GC guarantees that blocks are allocated on 4 byteboundaries
    These MARK macros can be used to attach & query flags on each
	monitor pointer. */

#define IL_LW_MARKED(raw)	\
	((((ILNativeUInt)(raw)) & 1) == 1)

#define IL_LW_MARK(raw)	\
	((ILLockWord)((((ILNativeUInt)(raw)) | 1)))

#define IL_LW_UNMARK(raw)	\
	((ILLockWord)((((ILNativeUInt)(raw)) & ~((ILNativeUInt)1))))

#define GetObjectMonitor(thread, obj) \
	((ILExecMonitor *)(IL_LW_UNMARK(GetObjectLockWord(thread, obj))))

#ifdef IL_CONFIG_USE_THIN_LOCKS
	
	#define IL_OBJECT_HEADER_PTR_MAP		(0)

	/*
	 * Gets a pointer to the WaitHandle object used by the object.
	 */
	ILLockWord CompareAndExchangeObjectLockWord
		(ILExecThread *thread, ILObject *obj, ILLockWord value, ILLockWord comparand);

	/*
	 *	Gets the LockWord for the object.
	 */
	ILLockWord GetObjectLockWord(ILExecThread *thread, ILObject *obj);

	/*
	 * Sets the LockWord for the object.
	 */
	void SetObjectLockWord(ILExecThread *thread, ILObject *obj, ILLockWord value);

#else

	/* The second word in the object is a pointer so the second bit in the map is 1 */
	#define IL_OBJECT_HEADER_PTR_MAP (2)

	/*
	 *	Classic monitor tags are stored in the object header.
	 */

	#define GetObjectLockWord(thread, obj) \
		(GetObjectHeader(obj)->lockWord)

	#define SetObjectLockWord(thread, ob, value) \
		GetObjectHeader(obj)->lockWord = value;

	#define GetObjectLockWordPtr(thread, obj) \
		(&(GetObjectLockWord(thread, obj)))

	#define CompareAndExchangeObjectLockWord(thread, obj, value, comparand) \
		(ILLockWord)((ILInterlockedCompareAndExchangeP_Full \
			((void **)GetObjectLockWordPtr(thread, obj), (void *)value, (void *)comparand)))

#endif

ILObject *_ILGetCurrentClrThread(ILExecThread *thread);

/*
 * Internal structure of a string object header.
 */
typedef struct
{
	ILInt32		capacity;
	ILInt32		length;

} System_String;

/*
 * Convert a string object pointer into a pointer to the first character.
 */
#define	StringToBuffer(str)		((ILUInt16 *)(((System_String *)(str)) + 1))

/*
 * Internal structure of a string builder.
 */
typedef struct
{
	System_String *buildString;
	ILInt32		   maxCapacity;
	ILBool		   needsCopy;

} System_Text_StringBuilder;

/*
 * Internal structure of an array header, padded to the best alignment.
 * TODO: We can enable the rank once the changed structure can be fixed
 * in the unroller.
 */
typedef struct
{
	/* ILInt32			rank; */		/* Allways 0 for simple arrays. */
	ILInt32			length;
} SArrayHeader;

typedef union
{
	SArrayHeader	__header;
#if !defined(__i386) && !defined(__i386__)
	unsigned char	pad[(sizeof(SArrayHeader) & ~(IL_BEST_ALIGNMENT - 1)) + IL_BEST_ALIGNMENT];
#endif

} System_Array;

/*
 * Convert an array object pointer into a pointer to the first item.
 */
#define	ArrayToBuffer(array)	((void *)(((System_Array *)(array)) + 1))

/*
 * Get the length (number of elements) in a simple array.
 */
#define ArrayLength(array)		(((System_Array *)(array))->__header.length)

/*
 * Determine if an array inherits from "$Synthetic.SArray".
 */
int _ILIsSArray(System_Array *array);

/*
 * Clone a single-dimensional array.
 */
ILObject *_ILCloneSArray(ILExecThread *thread, System_Array *array);

/*
 * Fuction for the engine for Array.Copy(System.Array, System.Array, Int32)
 * where the arrays are known to be one dimensional zero based, elemment types
 * are assignment compatible and of the same size so that copying can be one
 * with memcpy.
 * Return 0 if an exception was thrown and 1 if successfull.
 */
ILInt32 ILSArrayCopy_AAI4(ILExecThread *thread,
						  System_Array *array1,
						  System_Array *array2,
						  ILInt32 length,
						  ILInt32 elementSize);


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
							  ILInt32 elementSize);

/*
 * Function for Array.Clear(System.Array, int, int) where the type of the
 * array is known at verification time and it is zero based one dimensional.
 */
ILInt32 ILSArrayClear_AI4I4(ILExecThread *thread,
							System_Array *array,
							ILInt32 index,
							ILInt32 length,
							ILInt32 elementSize);

#ifdef IL_CONFIG_NON_VECTOR_ARRAYS

/*
 * Internal structure of a multi-dimensional array header.
 */
typedef struct
{
	ILInt32		lower;
	ILInt32		size;
	ILInt32		multiplier;

} MArrayBounds;
typedef struct
{
	ILInt32			rank;
	ILInt32			elemSize;
	void	       *data;
	MArrayBounds	bounds[1];

} System_MArray;

/*
 * Determine if an array inherits from "$Synthetic.MArray".
 */
int _ILIsMArray(System_Array *array);

/*
 * Clone a multi-dimensional array.
 */
ILObject *_ILCloneMArray(ILExecThread *thread, System_MArray *array);

#endif /* IL_CONFIG_NON_VECTOR_ARRAYS */

/*
 * Internal structure of a reflection object.  Types, methods, fields, etc.
 */
typedef struct
{
	void	   *privateData;

} System_Reflection;

/*
 * Get the caller's image from the call frame stack of a method.
 */
ILImage *_ILClrCallerImage(ILExecThread *thread);

/*
 * Check that the caller has permission to access a specific class
 * or member via reflection.  Returns non-zero if access has been granted.
 */
int _ILClrCheckAccess(ILExecThread *thread, ILClass *classInfo,
					  ILMember *member);

/*
 * Check that the caller has permission to access a specific program item.
 */
int _ILClrCheckItemAccess(ILExecThread *thread, ILProgramItem *item);

/*
 * Convert an "ILClass" into a "ClrType" instance.
 */
ILObject *_ILGetClrType(ILExecThread *thread, ILClass *classInfo);

/*
 * Convert an "ILType" into a "ClrType" instance.
 */
ILObject *_ILGetClrTypeForILType(ILExecThread *thread, ILType *type);

/*
 * Get the "ILClass" value associated with a "ClrType" object.
 */
ILClass *_ILGetClrClass(ILExecThread *thread, ILObject *type);

/*
 * Convert a non-type program item pointer into a reflection object.
 * Do not use this for types.  Use "_ILGetClrType" instead.
 */
ILObject *_ILClrToObject(ILExecThread *thread, void *item, const char *name);

/*
 * Convert a reflection object into a non-type program item pointer.
 * Do not use this for types.  Use "_ILGetClrClass" instead.
 */
void *_ILClrFromObject(ILExecThread *thread, ILObject *object);

/*
 * Throw a "NotSupportedException" from within the reflection code.
 */
void _ILClrNotSupported(ILExecThread *thread);

/*
 * Look for a type, starting at a particular image.  If "assemblyImage"
 * is NULL, then use the name to determine the image to start at.
 */
ILObject *_ILGetTypeFromImage(ILExecThread *thread,
							  ILImage *assemblyImage,
							  ILString *name,
							  ILBool throwOnError,
							  ILBool ignoreCase);

/*
 * Internal structure of a delegate.  Must match the "System.MulticastDelegate"
 * definition in the C# class library.
 */
typedef struct
{
	ILObject   *target;
	ILMethod   *methodInfo;
	void       *closure;
	ILObject   *prev;

} System_Delegate;

/*
 * Internal structure of "System.Threading.Thread".
 */
typedef struct
{
	ILThread   *privateData;
	ILBool		createdFromManagedCode;
	ILObject   *stateInfo;
	System_Delegate *start;
	ILString   *name;

} System_Thread;

/*
 * Structure of the "System.Diagnostics.PackedStackFrame" class.
 */
typedef struct
{
	ILMethod	   *method;
	ILInt32			offset;
	ILInt32			nativeOffset;

} PackedStackFrame;

/*
 * Structure of the ECMA part of the "System.Exception" class.
 */
typedef struct _tagSystemException System_Exception;
struct _tagSystemException
{
	System_String	   *message;
	System_Exception   *innerException;
	System_Array	   *stackTrace;

};

/*
 * Prototype all of the "internalcall" methods in the engine.
 */
#include "int_proto.h"

#ifdef	__cplusplus
};
#endif

#endif	/* _ENGINE_LIB_DEFS_H */
