/*
 * Buffer.cs - Implementation of the "System.Buffer" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if !ECMA_COMPAT

using System.Runtime.CompilerServices;

public sealed class Buffer
{
	// Cannot instantiate this class.
	private Buffer() {}

	// Validate a primitive array and get its length in bytes.
	private static int ValidatePrimitive(Array array, String name)
			{
				if(array == null)
				{
					throw new ArgumentNullException(name);
				}
				else if(!array.GetType().GetElementType().IsPrimitive)
				{
					throw new ArgumentException
						(_("Arg_NonPrimitiveArray"), name);
				}
				return GetLength(array);
			}

	// Copy a block of bytes from one primitive array to another.
	public static void BlockCopy(Array src, int srcOffset,
								 Array dst, int dstOffset,
								 int count)
			{
				int srcLen = ValidatePrimitive(src, "src");
				int dstLen = ValidatePrimitive(dst, "dst");
				if(srcOffset < 0)
				{
					throw new ArgumentOutOfRangeException
						("srcOffset", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((srcLen - srcOffset) < count)
				{
					throw new ArgumentException
						("count", _("ArgRange_Array"));
				}
				if(dstOffset < 0 )
				{
					throw new ArgumentOutOfRangeException
						("dstOffset", _("ArgRange_Array"));
				}
				if((dstLen - dstOffset) < count)
				{
					throw new ArgumentException
						("count", _("ArgRange_Array"));
				}
				Copy(src, srcOffset, dst, dstOffset, count);
			}

	// Returns the number of bytes in the specified primitive array.
	public static int ByteLength(Array array)
			{
				return ValidatePrimitive(array, "array");
			}

	// Get a particular byte from a primitive array.
	public static byte GetByte(Array array, int index)
			{
				int len = ValidatePrimitive(array, "array");
				if(index < 0 || index >= len)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				return GetElement(array, index);
			}

	// Set a particular byte within a primitive array.
	public static void SetByte(Array array, int index, byte value)
			{
				int len = ValidatePrimitive(array, "array");
				if(index < 0 || index >= len)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				SetElement(array, index, value);
			}

	// Internal implementation provided by the runtime engine.

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void Copy(Array src, int srcOffset,
									Array dst, int dstOffset,
									int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int GetLength(Array array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static byte GetElement(Array array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetElement(Array array, int index, byte value);

}; // class Buffer

#endif // !ECMA_COMPAT

}; // namespace System
