/*
 * Marshal.cs - Implementation of the
 *			"System.Runtime.InteropServices.Marshal" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_RUNTIME_INFRA

using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;
using System.Security;

// This class is not ECMA-compatible, strictly speaking.  But it is
// usually necessary for any application that uses PInvoke or C.

#if CONFIG_PERMISSIONS && !ECMA_COMPAT
[SuppressUnmanagedCodeSecurity]
#endif
public sealed class Marshal
{
	// This class cannot be instantiated.
	private Marshal() {}

	// Character size information.
	public static readonly int SystemDefaultCharSize = 1;
	public static readonly int SystemMaxDBCSCharSize = 6;

	// Allocate memory from the global (malloc) heap.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr AllocHGlobal(IntPtr cb);
	public static IntPtr AllocHGlobal(int cb)
			{
				return AllocHGlobal(new IntPtr(cb));
			}

	// Internal version of "Copy" from managed to unmanaged.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void CopyMU(Array source, int startOffset,
					           		  IntPtr destination, int numBytes);

	// Copy data from a managed array to an unmanaged memory pointer.
	public static void Copy(byte[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex, destination, length);
			}
	public static void Copy(char[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex * 2, destination, length * 2);
			}
	public static void Copy(double[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex * 8, destination, length * 8);
			}
	public static void Copy(float[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex * 4, destination, length * 4);
			}
	public static void Copy(int[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex * 4, destination, length * 4);
			}
	public static void Copy(long[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex * 8, destination, length * 8);
			}
	public static void Copy(short[] source, int startIndex,
					        IntPtr destination, int length)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == IntPtr.Zero)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > source.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (source.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyMU(source, startIndex * 2, destination, length * 2);
			}

	// Internal version of "Copy" from unmanaged to managed.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void CopyUM(IntPtr source, Array destination,
							   		  int startOffset, int numBytes);

	// Copy data from an unmanaged pointer to a managed array.
	public static void Copy(IntPtr source, byte[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex, length);
			}
	public static void Copy(IntPtr source, char[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex * 2, length * 2);
			}
	public static void Copy(IntPtr source, double[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex * 8, length * 8);
			}
	public static void Copy(IntPtr source, float[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex * 4, length * 4);
			}
	public static void Copy(IntPtr source, int[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex * 4, length * 4);
			}
	public static void Copy(IntPtr source, long[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex * 8, length * 8);
			}
	public static void Copy(IntPtr source, short[] destination,
							int startIndex, int length)
			{
				if(source == IntPtr.Zero)
				{
					throw new ArgumentNullException("source");
				}
				if(destination == null)
				{
					throw new ArgumentNullException("destination");
				}
				if(startIndex < 0 || startIndex > destination.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0 || (destination.Length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				CopyUM(source, destination, startIndex * 2, length * 2);
			}

	// Free memory that was allocated with AllocHGlobal.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void FreeHGlobal(IntPtr hglobal);

	// Get the offset of a field within a class.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr OffsetOfInternal(Type t, String fieldName);
	public static IntPtr OffsetOf(Type t, String fieldName)
			{
				if(t == null)
				{
					throw new ArgumentNullException("t");
				}
				else if(!(t is ClrType))
				{
					throw new ArgumentException(_("Arg_MustBeType"), "t");
				}
				if(fieldName == null)
				{
					throw new ArgumentNullException("fieldName");
				}
				IntPtr offset = OffsetOfInternal(t, fieldName);
				if(offset == new IntPtr(-1))
				{
					throw new ArgumentException
						(_("Reflection_UnknownField"), "fieldName");
				}
				return offset;
			}

	// Convert a pointer to an ANSI string into a string object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String PtrToStringAnsiInternal(IntPtr ptr, int len);
	public static String PtrToStringAnsi(IntPtr ptr)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else
				{
					return PtrToStringAnsiInternal(ptr, -1);
				}
			}
	public static String PtrToStringAnsi(IntPtr ptr, int len)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else if(len < 0)
				{
					throw new ArgumentException(_("ArgRange_NonNegative"));
				}
				else
				{
					return PtrToStringAnsiInternal(ptr, len);
				}
			}

	// Convert a pointer to an Auto string into a string object.
	// In this implementation, "Auto" is UTF-8.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String PtrToStringAutoInternal(IntPtr ptr, int len);
	public static String PtrToStringAuto(IntPtr ptr)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else
				{
					return PtrToStringAutoInternal(ptr, -1);
				}
			}
	public static String PtrToStringAuto(IntPtr ptr, int len)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else if(len < 0)
				{
					throw new ArgumentException(_("ArgRange_NonNegative"));
				}
				else
				{
					return PtrToStringAutoInternal(ptr, len);
				}
			}

	// Convert a pointer to a Unicode string into a string object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String PtrToStringUniInternal(IntPtr ptr, int len);
	public static String PtrToStringUni(IntPtr ptr)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else
				{
					return PtrToStringUniInternal(ptr, -1);
				}
			}
	public static String PtrToStringUni(IntPtr ptr, int len)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else if(len < 0)
				{
					throw new ArgumentException(_("ArgRange_NonNegative"));
				}
				else
				{
					return PtrToStringUniInternal(ptr, len);
				}
			}


	// Convert the data at an unmanaged pointer location into
	// an object by marshalling its fields one by one.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool PtrToStructureInternal
				(IntPtr ptr, Object structure, bool allowValueTypes);
	public static void PtrToStructure(IntPtr ptr, Object structure)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else if(structure == null)
				{
					throw new ArgumentNullException("structure");
				}
				if(!PtrToStructureInternal(ptr, structure, false))
				{
					throw new ArgumentException
						(_("Arg_CannotMarshalStruct"));
				}
			}
#if CONFIG_REFLECTION
	public static Object PtrToStructure(IntPtr ptr, Type structureType)
			{
				if(ptr == IntPtr.Zero)
				{
					return null;
				}
				else if(structureType == null)
				{
					throw new ArgumentNullException("structureType");
				}
				else if(!(structureType is ClrType))
				{
					throw new ArgumentException
						(_("Arg_MustBeType"), "structureType");
				}
				Object obj = Activator.CreateInstance(structureType);
				if(!PtrToStructureInternal(ptr, obj, true))
				{
					throw new ArgumentException
						(_("Arg_CannotMarshalStruct"));
				}
				return obj;
			}
#endif

	// Destroy the contents of an unmanaged structure.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool DestroyStructureInternal
				(IntPtr ptr, Type structureType);
	public static void DestroyStructure(IntPtr ptr, Type structureType)
			{
				if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				else if(structureType == null)
				{
					throw new ArgumentNullException("structureType");
				}
				else if(!(structureType is ClrType))
				{
					throw new ArgumentException
						(_("Arg_MustBeType"), "structureType");
				}
				if(!DestroyStructureInternal(ptr, structureType))
				{
					throw new ArgumentException
						(_("Arg_CannotMarshalStruct"));
				}
			}

	// Convert an object into an unmanaged structure.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool StructureToPtrInternal
				(Object structure, IntPtr ptr);
	public static void StructureToPtr(Object structure, IntPtr ptr,
									  bool fDeleteOld)
			{
				if(structure == null)
				{
					throw new ArgumentNullException("structure");
				}
				else if(ptr == IntPtr.Zero)
				{
					throw new ArgumentNullException("ptr");
				}
				if(!StructureToPtrInternal(structure, ptr))
				{
					throw new ArgumentException
						(_("Arg_CannotMarshalStruct"));
				}
				if(fDeleteOld)
				{
					DestroyStructure(ptr, structure.GetType());
				}
			}

	// Convert an object into a pointer to its first byte.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ObjectToPtr(Object obj);

	// Read a byte from an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte ReadByte(IntPtr ptr, int ofs);
	public static byte ReadByte(IntPtr ptr)
			{
				return ReadByte(ptr, 0);
			}
	public static byte ReadByte(Object ptr, int ofs)
			{
				return ReadByte(ObjectToPtr(ptr), ofs);
			}

	// Read a 16-bit integer from an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static short ReadInt16(IntPtr ptr, int ofs);
	public static short ReadInt16(IntPtr ptr)
			{
				return ReadInt16(ptr, 0);
			}
	public static short ReadInt16(Object ptr, int ofs)
			{
				return ReadInt16(ObjectToPtr(ptr), ofs);
			}

	// Read a 32-bit integer from an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int ReadInt32(IntPtr ptr, int ofs);
	public static int ReadInt32(IntPtr ptr)
			{
				return ReadInt32(ptr, 0);
			}
	public static int ReadInt32(Object ptr, int ofs)
			{
				return ReadInt32(ObjectToPtr(ptr), ofs);
			}

	// Read a 64-bit integer from an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long ReadInt64(IntPtr ptr, int ofs);
	public static long ReadInt64(IntPtr ptr)
			{
				return ReadInt64(ptr, 0);
			}
	public static long ReadInt64(Object ptr, int ofs)
			{
				return ReadInt64(ObjectToPtr(ptr), ofs);
			}

	// Read a native pointer from an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr ReadIntPtr(IntPtr ptr, int ofs);
	public static IntPtr ReadIntPtr(IntPtr ptr)
			{
				return ReadIntPtr(ptr, 0);
			}
	public static IntPtr ReadIntPtr(Object ptr, int ofs)
			{
				return ReadIntPtr(ObjectToPtr(ptr), ofs);
			}

	// Reallocate memory that was allocated with AllocHGlobal.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr ReAllocHGlobal(IntPtr pv, IntPtr cb);

	// Get the size of a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int SizeOfInternal(Type t);
	public static int SizeOf(Type t)
			{
				if(t == null)
				{
					throw new ArgumentNullException("t");
				}
				else if(!(t is ClrType))
				{
					throw new ArgumentException(_("Arg_MustBeType"), "t");
				}
				else
				{
					return SizeOfInternal(t);
				}
			}

	// Get the size of an object.
	public static int SizeOf(Object o)
			{
				if(o == null)
				{
					throw new ArgumentNullException("o");
				}
				return SizeOf(o.GetType());
			}

	// Convert a string into an ANSI character buffer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr StringToHGlobalAnsi(String s);

	// Convert a string into an Auto character buffer.
	// In this implementation, "Auto" is UTF-8.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr StringToHGlobalAuto(String s);

	// Convert a string into a Unicode character buffer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr StringToHGlobalUni(String s);

	// Get the address of a pinned array element.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr UnsafeAddrOfPinnedArrayElement
				(Array arr, int index);

	// Write a byte to an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void WriteByte(IntPtr ptr, int ofs, byte val);
	public static void WriteByte(IntPtr ptr, byte val)
			{
				WriteByte(ptr, 0, val);
			}
	public static void WriteByte(Object ptr, int ofs, byte val)
			{
				WriteByte(ObjectToPtr(ptr), ofs, val);
			}

	// Write a 16-bit integer to an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void WriteInt16(IntPtr ptr, int ofs, short val);
	public static void WriteInt16(IntPtr ptr, short val)
			{
				WriteInt16(ptr, 0, val);
			}
	public static void WriteInt16(Object ptr, int ofs, short val)
			{
				WriteInt16(ObjectToPtr(ptr), ofs, val);
			}

	// Write a 16-bit integer (as a char) to an unmanaged pointer.
	public static void WriteInt16(IntPtr ptr, int ofs, char val)
			{
				WriteInt16(ptr, ofs, (short)val);
			}
	public static void WriteInt16(IntPtr ptr, char val)
			{
				WriteInt16(ptr, 0, (short)val);
			}
	public static void WriteInt16(Object ptr, int ofs, char val)
			{
				WriteInt16(ObjectToPtr(ptr), ofs, (short)val);
			}

	// Write a 32-bit integer to an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void WriteInt32(IntPtr ptr, int ofs, int val);
	public static void WriteInt32(IntPtr ptr, int val)
			{
				WriteInt32(ptr, 0, val);
			}
	public static void WriteInt32(Object ptr, int ofs, int val)
			{
				WriteInt32(ObjectToPtr(ptr), ofs, val);
			}

	// Write a 64-bit integer to an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void WriteInt64(IntPtr ptr, int ofs, long val);
	public static void WriteInt64(IntPtr ptr, long val)
			{
				WriteInt64(ptr, 0, val);
			}
	public static void WriteInt64(Object ptr, int ofs, long val)
			{
				WriteInt64(ObjectToPtr(ptr), ofs, val);
			}

	// Write a native pointer to an unmanaged pointer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void WriteIntPtr(IntPtr ptr, int ofs, IntPtr val);
	public static void WriteIntPtr(IntPtr ptr, IntPtr val)
			{
				WriteIntPtr(ptr, 0, val);
			}
	public static void WriteIntPtr(Object ptr, int ofs, IntPtr val)
			{
				WriteIntPtr(ObjectToPtr(ptr), ofs, val);
			}

#if CONFIG_COM_INTEROP

	// Stub out COM-related methods, which are not supported
	// in this implementation.
	public static int AddRef(IntPtr pUnk)
			{
				throw new NotImplementedException();
			}
	public static IntPtr AllocCoTaskMem(int cb)
			{
				throw new NotImplementedException();
			}
	public static Object BindToMoniker(String monikerName)
			{
				throw new NotImplementedException();
			}
	public static void ChangeWrapperHandleStrength(Object otp, bool fIsWeak)
			{
				throw new NotImplementedException();
			}
	public static Object CreateWrapperOfType(Object o, Type t)
			{
				throw new NotImplementedException();
			}
	public static void FreeCoTaskMem(IntPtr ptr)
			{
				throw new NotImplementedException();
			}
	public static Guid GenerateGuidForType(Type type)
			{
				throw new NotImplementedException();
			}
	public static String GenerateProgIdForType(Type type)
			{
				throw new NotImplementedException();
			}
	public static Object GetActiveObject(String progID)
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetComInterfaceForObject(Object o, Type T)
			{
				throw new NotImplementedException();
			}
	public static Object GetComObjectData(Object obj, Object key)
			{
				throw new NotImplementedException();
			}
#if CONFIG_REFLECTION
	public static int GetComSlotForMethodInfo(MemberInfo m)
			{
				throw new NotImplementedException();
			}
#endif
	public static int GetEndComSlot(Type t)
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetIDispatchForObject(Object o)
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetITypeInfoForType(Type t)
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetIUnknownForObject(Object o)
			{
				throw new NotImplementedException();
			}
#if !ECMA_COMPAT && CONFIG_REFLECTION
	public static MemberInfo GetMethodInfoForComSlot
				(Type t, int slot, ref ComMemberType memberType)
			{
				throw new NotImplementedException();
			}
#endif
	public static Object GetObjectForIUnknown(IntPtr pUnk)
			{
				throw new NotImplementedException();
			}
	public static Object[] GetObjectsForNativeVariants
				(IntPtr aSrcNativeVariant, int cVars)
			{
				throw new NotImplementedException();
			}
	public static int GetStartComSlot(Type t)
			{
				throw new NotImplementedException();
			}
	public static Object GetTypedObjectForIUnknown(IntPtr pUnk, Type t)
			{
				throw new NotImplementedException();
			}
	public static Type GetTypeForITypeInfo(IntPtr piTypeInfo)
			{
				throw new NotImplementedException();
			}
	public static String GetTypeInfoName(UCOMITypeInfo pTI)
			{
				throw new NotImplementedException();
			}
	public static Guid GetTypeLibGuid(UCOMITypeLib pTLB)
			{
				throw new NotImplementedException();
			}
	public static Guid GetTypeLibGuidForAssembly(Assembly asm)
			{
				throw new NotImplementedException();
			}
	public static int GetTypeLibLcid(UCOMITypeLib pTLB)
			{
				throw new NotImplementedException();
			}
	public static String GetTypeLibName(UCOMITypeLib pTLB)
			{
				throw new NotImplementedException();
			}
	public static bool IsComObject(Object o)
			{
				return false;
			}
	public static bool IsTypeVisibleFromCom(Type t)
			{
				return false;
			}
	public static int QueryInterface(IntPtr pUnk, ref Guid iid, out IntPtr ppv)
			{
				throw new NotImplementedException();
			}
	public static IntPtr ReAllocCoTaskMem(IntPtr pv, int cb)
			{
				throw new NotImplementedException();
			}
	public static int Release(IntPtr pUnk)
			{
				throw new NotImplementedException();
			}
	public static int ReleaseComObject(Object o)
			{
				throw new NotImplementedException();
			}
	public static void ReleaseThreadCache()
			{
				throw new NotImplementedException();
			}
	public static bool SetComObjectData(Object obj, Object key, Object data)
			{
				throw new NotImplementedException();
			}
	public static IntPtr StringToCoTaskMemAnsi(String s)
			{
				throw new NotImplementedException();
			}
	public static IntPtr StringToCoTaskMemAuto(String s)
			{
				throw new NotImplementedException();
			}
	public static IntPtr StringToCoTaskMemUni(String s)
			{
				throw new NotImplementedException();
			}
	public static void ThrowExceptionForHR(int errorCode)
			{
				throw new NotImplementedException();
			}
	public static void ThrowExceptionForHR(int errorCode, IntPtr errorInfo)
			{
				throw new NotImplementedException();
			}

#endif // CONFIG_COM_INTEROP

	// Other methods that aren't relevant to this implementation.
	public static void FreeBSTR(IntPtr str)
			{
				throw new NotImplementedException();
			}
	public static int GetExceptionCode()
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetExceptionPointers()
			{
				throw new NotImplementedException();
			}
#if CONFIG_REFLECTION
	public static IntPtr GetHINSTANCE(Module m)
			{
				throw new NotImplementedException();
			}
#endif
	public static int GetHRForException(Exception e)
			{
				throw new NotImplementedException();
			}
	public static int GetHRForLastWin32Error()
			{
				throw new NotImplementedException();
			}
	public static int GetLastWin32Error()
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetManagedThunkForUnmanagedMethodPtr
				(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
			{
				throw new NotImplementedException();
			}
	public static void GetNativeVariantForObject
				(Object obj, IntPtr pDstNativeVariant)
			{
				throw new NotImplementedException();
			}
	public static Object GetObjectForNativeVariant(IntPtr pSrcNativeVariant)
			{
				throw new NotImplementedException();
			}
	public static Thread GetThreadFromFiberCookie(int cookie)
			{
				throw new NotImplementedException();
			}
	public static IntPtr GetUnmanagedThunkForManagedMethodPtr
				(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
			{
				throw new NotImplementedException();
			}
#if CONFIG_REFLECTION
	public static int NumParamBytes(MethodInfo m)
			{
				throw new NotImplementedException();
			}
	public static void Prelink(MethodInfo m)
			{
				throw new NotImplementedException();
			}
#endif
	public static void PrelinkAll(Type c)
			{
				throw new NotImplementedException();
			}
	public static String PtrToStringBSTR(IntPtr ptr)
			{
				throw new NotImplementedException();
			}
	public static IntPtr StringToBSTR(String s)
			{
				throw new NotImplementedException();
			}

}; // class Marshal

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Runtime.InteropServices
