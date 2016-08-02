/*
 * IntPtr.cs - Implementation of the "System.IntPtr" class.
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

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
[Serializable]
#endif
public struct IntPtr
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Public constants.
	public static readonly IntPtr Zero = new IntPtr(0);

	// Internal state.
	unsafe private void *value_;

	// Constructors.
	unsafe public IntPtr(int value)
			{
				value_ = (void *)value;
			}
	unsafe public IntPtr(long value)
			{
				if(Size == 4 &&
				   (value < ((long)(Int32.MinValue)) ||
				    value > ((long)(Int32.MaxValue))))
				{
					throw new OverflowException(_("Overflow_Pointer"));
				}
				value_ = (void *)value;
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	unsafe public IntPtr(void *value)
			{
				value_ = value;
			}
#endif

	// Override inherited methods.
	unsafe public override int GetHashCode()
			{
				return unchecked((int)value_);
			}
	unsafe public override bool Equals(Object value)
			{
				if(value is IntPtr)
				{
					return (value_ == ((IntPtr)value).value_);
				}
				else
				{
					return false;
				}
			}

	// Numeric conversion.
	unsafe public int ToInt32()
			{
				long ptr = (long)value_;
				if(ptr >= (long)(Int32.MinValue) &&
				   ptr <= (long)(Int32.MaxValue))
				{
					return unchecked((int)ptr);
				}
				else
				{
					throw new OverflowException(_("Overflow_Pointer"));
				}
			}
	unsafe public long ToInt64()
			{
				return (long)value_;
			}

	// Get the pointer within this object.
	[CLSCompliant(false)]
	unsafe public void *ToPointer()
			{
				return value_;
			}

	// String conversion.
	unsafe public override String ToString()
			{
				if(Size == 4)
				{
					return ((int)value_).ToString();
				}
				else
				{
					return ((long)value_).ToString();
				}
			}

	// Properties.
	public static int Size
			{
				get
				{
					unsafe
					{
						return sizeof(IntPtr);
					}
				}
			}

	// Operators.
	unsafe public static bool operator==(IntPtr x, IntPtr y)
			{
				return (x.value_ == y.value_);
			}
	unsafe public static bool operator!=(IntPtr x, IntPtr y)
			{
				return (x.value_ != y.value_);
			}

#if !ECMA_COMPAT

	unsafe public static explicit operator IntPtr(int x)
			{
				return new IntPtr(x);
			}
	unsafe public static explicit operator IntPtr(long x)
			{
				return new IntPtr(x);
			}
	[CLSCompliant(false)]
	unsafe public static explicit operator IntPtr(void *x)
			{
				return new IntPtr(x);
			}
	unsafe public static explicit operator int(IntPtr x)
			{
				return x.ToInt32();
			}
	unsafe public static explicit operator long(IntPtr x)
			{
				return x.ToInt64();
			}
	[CLSCompliant(false)]
	unsafe public static explicit operator void *(IntPtr x)
			{
				return x.ToPointer();
			}

#endif // !ECMA_COMPAT

#if CONFIG_SERIALIZATION

	// De-serialize an "IntPtr" value.
	internal unsafe IntPtr(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				long value = info.GetInt64("value");
				if(Size == 4)
				{
					if(value < (long)(Int32.MinValue) ||
					   value > (long)(Int32.MaxValue))
					{
						throw new ArgumentException(_("Overflow_Pointer"));
					}
				}
				value_ = (void *)value;
			}

	// Get the serialization data for an "IntPtr" value.
	unsafe void ISerializable.GetObjectData(SerializationInfo info,
											StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("value", ToInt64());
			}

#endif // CONFIG_SERIALIZATION

}; // struct IntPtr

}; // namespace System
