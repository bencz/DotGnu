/*
 * lib_marshal.c - Internalcall methods for the Marshal class.
 *
 * Copyright (C) 2002, 2008  Southern Storm Software, Pty Ltd.
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

#ifdef IL_CONFIG_PINVOKE

/*
 * Determine if the caller is authorised to perform unmanaged operations,
 * and throw an exception if it isn't.
 */
static int UnmanagedOK(ILExecThread *thread)
{
	/* Check that the caller is secure */
	if(ILImageIsSecure(_ILClrCallerImage(thread)))
	{
		return 1;
	}

	/* Throw a SecurityException within the current thread */
	ILExecThreadThrowSystem(thread, "System.Security.SecurityException", 0);
	return 0;
}

/*
 * public static IntPtr AllocHGlobal(IntPtr cb);
 */
ILNativeInt _IL_Marshal_AllocHGlobal(ILExecThread *_thread, ILNativeInt cb)
{
	if(UnmanagedOK(_thread))
	{
		/* Use the underlying system "calloc", because "ILCalloc"
		   may have been redirected elsewhere */
		void *ptr = (void *)calloc((unsigned)cb, 1);
		if(ptr)
		{
			return (ILNativeInt)ptr;
		}
		ILExecThreadThrowOutOfMemory(_thread);
	}
	return 0;
}

/*
 * private static void CopyMU(Array source, int startOffset,
 *							  IntPtr destination, int numBytes);
 */
void _IL_Marshal_CopyMU(ILExecThread *_thread, ILObject *source,
						ILInt32 startOffset, ILNativeInt destination,
						ILInt32 numBytes)
{
	if(UnmanagedOK(_thread) && source && destination && numBytes > 0)
	{
		ILMemMove((void *)destination,
				  ((unsigned char *)(ArrayToBuffer(source))) + startOffset,
				  (unsigned)numBytes);
	}
}

/*
 * private static void CopyUM(IntPtr source, Array destination,
 *							  int startOffset, int numBytes);
 */
void _IL_Marshal_CopyUM(ILExecThread *_thread, ILNativeInt source,
					    ILObject *destination, ILInt32 startOffset,
						ILInt32 numBytes)
{
	if(UnmanagedOK(_thread) && source && destination && numBytes > 0)
	{
		ILMemMove(((unsigned char *)(ArrayToBuffer(destination))) +
					startOffset, (void *)source, (unsigned)numBytes);
	}
}

/*
 * public static void FreeHGlobal(IntPtr hglobal);
 */
void _IL_Marshal_FreeHGlobal(ILExecThread *_thread, ILNativeInt hglobal)
{
	if(UnmanagedOK(_thread) && hglobal)
	{
		/* Use the underlying system "free", because "ILFree"
		   may have been redirected elsewhere */
		free((void *)hglobal);
	}
}

/*
 * private static IntPtr OffsetOfInternal(Type t, String fieldName);
 */
ILNativeInt _IL_Marshal_OffsetOfInternal(ILExecThread *_thread, ILObject *t,
								 		 ILString *fieldName)
{
	char *name;
	ILClass *classInfo;
	ILField *field;
	ILNativeInt offset;

	if(UnmanagedOK(_thread) && t && fieldName)
	{
		/* Convert the "Type" into an "ILClass *" structure */
		classInfo = _ILGetClrClass(_thread, t);
		if(!classInfo)
		{
			return -1;
		}

		/* Get the field name in UTF-8 */
		name = ILStringToUTF8(_thread, fieldName);
		if(!name)
		{
			return -1;
		}

		/* Make sure that the class has been laid out */
		IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
		if(!_ILLayoutClass(_ILExecThreadProcess(_thread), classInfo))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			return -1;
		}

		/* Look for the field within the class */
		while(classInfo != 0)
		{
			field = 0;
			while((field = (ILField *)ILClassNextMemberByKind
						(classInfo, (ILMember *)field,
						 IL_META_MEMBERKIND_FIELD)) != 0)
			{
				if(!ILField_IsStatic(field) &&
				   !strcmp(ILField_Name(field), name))
				{
					offset = (ILNativeInt)(ILUInt32)(field->offset);
					IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
					return offset;
				}
			}
			classInfo = ILClass_ParentClass(classInfo);
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	}
	return -1;
}

/*
 * private static String PtrToStringAnsiInternal(IntPtr ptr, int len);
 */
ILString *_IL_Marshal_PtrToStringAnsiInternal(ILExecThread *_thread,
											  ILNativeInt ptr, ILInt32 len)
{
	if(UnmanagedOK(_thread))
	{
		if(ptr)
		{
			if(len < 0)
			{
				return ILStringCreate(_thread, (const char *)ptr);
			}
			else
			{
				return ILStringCreateLen(_thread, (const char *)ptr, len);
			}
		}
	}
	return 0;
}

/*
 * private static String PtrToStringAutoInternal(IntPtr ptr, int len);
 */
ILString *_IL_Marshal_PtrToStringAutoInternal(ILExecThread *_thread,
											  ILNativeInt ptr, ILInt32 len)
{
	if(UnmanagedOK(_thread))
	{
		if(ptr)
		{
			if(len < 0)
			{
				return ILStringCreateUTF8(_thread, (const char *)ptr);
			}
			else
			{
				return ILStringCreateUTF8Len(_thread, (const char *)ptr, len);
			}
		}
	}
	return 0;
}

/*
 * private static String PtrToStringUniInternal(IntPtr ptr, int len);
 */
ILString *_IL_Marshal_PtrToStringUniInternal(ILExecThread *_thread,
											 ILNativeInt ptr, ILInt32 len)
{
	if(UnmanagedOK(_thread))
	{
		if(ptr)
		{
			if(len < 0)
			{
				return ILStringWCreate(_thread, (const ILUInt16 *)ptr);
			}
			else
			{
				return ILStringWCreateLen(_thread, (const ILUInt16 *)ptr, len);
			}
		}
	}
	return 0;
}

/*
 * private static IntPtr ObjectToPtr(Object obj);
 */
ILNativeInt _IL_Marshal_ObjectToPtr(ILExecThread *_thread, ILObject *obj)
{
	/* In this implementation, the object handle points at the object data */
	return (ILNativeInt)obj;
}

/*
 * public static byte ReadByte(IntPtr ptr, int ofs);
 */
ILUInt8 _IL_Marshal_ReadByte(ILExecThread *_thread, ILNativeInt ptr,
							 ILInt32 ofs)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		return ((ILUInt8 *)ptr)[ofs];
	}
	return 0;
}

/*
 * public static short ReadInt16(IntPtr ptr, int ofs);
 */
ILInt16 _IL_Marshal_ReadInt16(ILExecThread *_thread, ILNativeInt ptr,
							  ILInt32 ofs)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILInt16 temp;
		ILMemCpy(&temp, ((unsigned char *)ptr) + ofs, sizeof(ILInt16));
		return temp;
	}
	return 0;
}

/*
 * public static int ReadInt32(IntPtr ptr, int ofs);
 */
ILInt32 _IL_Marshal_ReadInt32(ILExecThread *_thread, ILNativeInt ptr,
							  ILInt32 ofs)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILInt32 temp;
		ILMemCpy(&temp, ((unsigned char *)ptr) + ofs, sizeof(ILInt32));
		return temp;
	}
	return 0;
}

/*
 * public static long ReadInt64(IntPtr ptr, int ofs);
 */
ILInt64 _IL_Marshal_ReadInt64(ILExecThread *_thread, ILNativeInt ptr,
							  ILInt32 ofs)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILInt64 temp;
		ILMemCpy(&temp, ((unsigned char *)ptr) + ofs, sizeof(ILInt64));
		return temp;
	}
	return 0;
}

/*
 * public static IntPtr ReadIntPtr(IntPtr ptr, int ofs);
 */
ILNativeInt _IL_Marshal_ReadIntPtr(ILExecThread *_thread, ILNativeInt ptr,
								   ILInt32 ofs)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILNativeInt temp;
		ILMemCpy(&temp, ((unsigned char *)ptr) + ofs, sizeof(ILNativeInt));
		return temp;
	}
	return 0;
}

/*
 * public static IntPtr ReAllocHGlobal(IntPtr pv, IntPtr cb);
 */
ILNativeInt _IL_Marshal_ReAllocHGlobal(ILExecThread *_thread,
									   ILNativeInt pv, ILNativeInt cb)
{
	if(UnmanagedOK(_thread))
	{
		/* Use the underlying system "realloc", because "ILRealloc"
		   may have been redirected elsewhere */
		void *ptr = (void *)realloc((void *)pv, (unsigned)cb);
		if(ptr)
		{
			return (ILNativeInt)ptr;
		}
		ILExecThreadThrowOutOfMemory(_thread);
	}
	return 0;
}

/*
 * private static int SizeOfInternal(Type t);
 */
ILInt32 _IL_Marshal_SizeOfInternal(ILExecThread * _thread, ILObject *t)
{
	ILClass *classInfo;

	if(UnmanagedOK(_thread) && t)
	{
		/* Convert the "Type" into an "ILClass *" structure */
		classInfo = _ILGetClrClass(_thread, t);
		if(!classInfo)
		{
			return 0;
		}

		/* Get the size of the type and return it */
		/* TODO: this should return the native size, not the managed size */
		return (ILInt32)(ILSizeOfType
					(_thread, ILType_FromValueType(classInfo)));
	}
	else
	{
		return 0;
	}
}

/*
 * public static IntPtr StringToHGlobalAnsi(String s);
 */
ILNativeInt _IL_Marshal_StringToHGlobalAnsi(ILExecThread *_thread, ILString *s)
{
	if(UnmanagedOK(_thread) && s)
	{
		ILUInt16 *buf = StringToBuffer(s);
		ILInt32 len = ((System_String *)s)->length;
		unsigned long size = ILAnsiGetByteCount(buf, (unsigned long)len);
		char *newStr = (char *)malloc(size + 1);
		if(!newStr)
		{
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
		ILAnsiGetBytes(buf, (unsigned long)len,
					   (unsigned char *)newStr, size);
		newStr[size] = '\0';
		return (ILNativeInt)newStr;
	}
	else
	{
		return 0;
	}
}

/*
 * public static IntPtr StringToHGlobalAuto(String s);
 */
ILNativeInt _IL_Marshal_StringToHGlobalAuto(ILExecThread *_thread, ILString *s)
{
	ILUInt16 *buffer;
	ILInt32 length;
	ILInt32 utf8Len;
	char *newStr;
	char *temp;
	int posn;

	/* Bail out immediately if the string is NULL */
	if(!UnmanagedOK(_thread) || !s)
	{
		return 0;
	}

	/* Determine the length of the string in UTF-8 characters */
	buffer = StringToBuffer(s);
	length = ((System_String *)s)->length;
	posn = 0;
	utf8Len = 0;
	while(posn < (int)length)
	{
		utf8Len += ILUTF8WriteChar
			(0, ILUTF16ReadChar(buffer, (int)length, &posn));
	}

	/* Allocate space within the garbage-collected heap */
	newStr = (char *)malloc(utf8Len + 1);
	if(!newStr)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	/* Copy the characters into the allocated buffer */
	temp = newStr;
	posn = 0;
	while(posn < (int)length)
	{
		temp += ILUTF8WriteChar
			(temp, ILUTF16ReadChar(buffer, (int)length, &posn));
	}
	*temp = '\0';

	/* Done */
	return (ILNativeInt)newStr;
}

/*
 * public static IntPtr StringToHGlobalUni(String s);
 */
ILNativeInt _IL_Marshal_StringToHGlobalUni(ILExecThread *_thread, ILString *s)
{
	if(UnmanagedOK(_thread) && s)
	{
		ILUInt16 *buf = StringToBuffer(s);
		ILInt32 len = ((System_String *)s)->length;
		ILUInt16 *newStr = (ILUInt16 *)malloc((len + 1) * sizeof(ILUInt16));
		if(!newStr)
		{
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
		if(len > 0)
		{
			ILMemCpy(newStr, buf, len * sizeof(ILUInt16));
		}
		newStr[len] = (ILUInt16)0;
		return (ILNativeInt)newStr;
	}
	else
	{
		return 0;
	}
}

/*
 * public static IntPtr UnsafeAddrOfPinnedArrayElement(Array arr, int index);
 */
ILNativeInt _IL_Marshal_UnsafeAddrOfPinnedArrayElement(ILExecThread *_thread,
													   ILObject *arr,
													   ILInt32 index)
{
	ILType *type;
	ILInt32 elemSize;

	if(UnmanagedOK(_thread) && arr && index >= 0 &&
	   _ILIsSArray((System_Array *)arr) &&
	   index < ArrayLength(arr))
	{
		type = ILClassGetSynType(GetObjectClass(arr));
		type = ILTypeGetEnumType(ILType_ElemType(type));
		elemSize = (ILInt32)(ILSizeOfType(_thread, type));
		return (ILNativeInt)(((unsigned char *)(ArrayToBuffer(arr))) +
								elemSize * index);
	}
	else
	{
		return 0;
	}
}

/*
 * public static void WriteByte(IntPtr ptr, int ofs, byte val);
 */
void _IL_Marshal_WriteByte(ILExecThread *_thread, ILNativeInt ptr,
						   ILInt32 ofs, ILUInt8 val)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		((ILUInt8 *)ptr)[ofs] = val;
	}
}

/*
 * public static void WriteInt16(IntPtr ptr, int ofs, short val);
 */
void _IL_Marshal_WriteInt16(ILExecThread *_thread, ILNativeInt ptr,
							ILInt32 ofs, ILInt16 val)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILMemCpy(((unsigned char *)ptr) + ofs, &val, sizeof(val));
	}
}

/*
 * public static void WriteInt32(IntPtr ptr, int ofs, int val);
 */
void _IL_Marshal_WriteInt32(ILExecThread *_thread, ILNativeInt ptr,
						    ILInt32 ofs, ILInt32 val)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILMemCpy(((unsigned char *)ptr) + ofs, &val, sizeof(val));
	}
}

/*
 * public static void WriteInt64(IntPtr ptr, int ofs, long val);
 */
void _IL_Marshal_WriteInt64(ILExecThread *_thread, ILNativeInt ptr,
							ILInt32 ofs, ILInt64 val)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILMemCpy(((unsigned char *)ptr) + ofs, &val, sizeof(val));
	}
}

/*
 * public static void WriteIntPtr(IntPtr ptr, int ofs, IntPtr val);
 */
void _IL_Marshal_WriteIntPtr(ILExecThread *_thread, ILNativeInt ptr,
							 ILInt32 ofs, ILNativeInt val)
{
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Handle the possibility of misaligned accesses carefully */
		ILMemCpy(((unsigned char *)ptr) + ofs, &val, sizeof(val));
	}
}

ILBool _IL_Marshal_PtrToStructureInternal(ILExecThread *_thread,
										  ILNativeInt ptr,
										  ILObject *structure,
										  ILBool allowValueTypes)
{
	/* TODO: this isn't correct */
	ILClass *classInfo;
	ILInt32 size;
	if(UnmanagedOK(_thread) && ptr)
	{
		/* Convert the "Object" into an "ILClass *" structure */
		classInfo = GetObjectClass(structure);
		if(!classInfo)
		{
			return 0;
		}
		
		/* Get the size of the type */
		size = (ILInt32)(ILSizeOfType
				(_thread, ILType_FromValueType(classInfo)));
		ILMemCpy(structure,(void*)ptr,size);
		return 1;
	}
	return 0;
}

ILBool _IL_Marshal_DestroyStructureInternal(ILExecThread *_thread,
											ILNativeInt ptr,
											ILObject *structureType)
{
	/* TODO */
	return 0;
}

void _ILStructToNative(ILExecThread *thread, void *value, ILType *type)
{
	ILClass *classInfo;
	ILField *field;
	void *ptr;
	ILMethod *method;
	ILPInvoke *pinv;
	ILUInt32 marshalType;
	char *customName;
	int customNameLen;
	char *customCookie;
	int customCookieLen;

	/* Bail out if not a struct type */
	type = ILTypeStripPrefixes(type);
	if(!ILType_IsValueType(type))
	{
		return;
	}

	/* Get the current method and PInvoke information */
	method = thread->method;
	pinv = ILPInvokeFind(method);

	/* Process the fields within the type */
	classInfo = ILType_ToValueType(type);
	field = 0;
	while((field = (ILField *)ILClassNextMemberByKind
				(classInfo, (ILMember *)field,
				 IL_META_MEMBERKIND_FIELD)) != 0)
	{
		if(ILField_IsStatic(field))
		{
			continue;
		}
		type = ILField_Type(field);
		ptr = (void *)(((unsigned char *)value) + field->offset);
		_ILStructToNative(thread, ptr, type);
		marshalType = ILPInvokeGetMarshalType(pinv, method, 0,
	   										  &customName, &customNameLen,
											  &customCookie,
											  &customCookieLen, type);
		/* TODO: convert other kinds of fields, not just delegates */
		if(marshalType == IL_META_MARSHAL_FNPTR)
		{
	    #ifdef IL_USE_JIT
			*((void **)ptr) = ILJitDelegateGetClosure
				(*((ILObject **)ptr), type);
	    #else
			*((void **)ptr) = _ILDelegateGetClosure
				(thread, *((ILObject **)ptr));
	    #endif
		}
	}
}

ILBool _IL_Marshal_StructureToPtrInternal(ILExecThread *_thread,
										  ILObject *structure,
										  ILNativeInt ptr)
{
	if(UnmanagedOK(_thread) && structure && ptr)
	{
		ILClass *classInfo = GetObjectClass(structure);
		ILType *type = ILClassToType(classInfo);
		ILUInt32 size = ILSizeOfType(_thread, type);
		ILMemCpy((void *)ptr, structure, size);
		_ILStructToNative(_thread, (void *)ptr, type);
		return 1;
	}
	return 0;
}

#endif /* IL_CONFIG_PINVOKE */

#ifdef	__cplusplus
};
#endif
