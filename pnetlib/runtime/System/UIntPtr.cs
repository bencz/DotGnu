/*
 * UIntPtr.cs - Implementation of the "System.UIntPtr" class.
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

namespace System
{

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[CLSCompliant(false)]
public struct UIntPtr
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Public constants.
	public static readonly UIntPtr Zero = new UIntPtr(0);

	// Internal state.
	unsafe private void *value_;

	// Constructors.
	unsafe public UIntPtr(uint value)
			{
				value_ = (void *)value;
			}
	unsafe public UIntPtr(ulong value)
			{
				if(Size == 4 &&
				   value > ((ulong)(UInt32.MaxValue)))
				{
					throw new OverflowException(_("Overflow_Pointer"));
				}
				value_ = (void *)value;
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	unsafe public UIntPtr(void *value)
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
				if(value is UIntPtr)
				{
					return (value_ == ((UIntPtr)value).value_);
				}
				else
				{
					return false;
				}
			}

	// Numeric conversion.
	unsafe public uint ToUInt32()
			{
				ulong ptr = (ulong)value_;
				if(ptr <= (ulong)(UInt32.MaxValue))
				{
					return unchecked((uint)ptr);
				}
				else
				{
					throw new OverflowException(_("Overflow_Pointer"));
				}
			}
	unsafe public ulong ToUInt64()
			{
				return (ulong)value_;
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
					return ((uint)value_).ToString();
				}
				else
				{
					return ((ulong)value_).ToString();
				}
			}

	// Properties.
	public static int Size
			{
				get
				{
					unsafe
					{
						return sizeof(UIntPtr);
					}
				}
			}

	// Operators.
	unsafe public static bool operator==(UIntPtr x, UIntPtr y)
			{
				return (x.value_ == y.value_);
			}
	unsafe public static bool operator!=(UIntPtr x, UIntPtr y)
			{
				return (x.value_ != y.value_);
			}

#if !ECMA_COMPAT

	unsafe public static explicit operator UIntPtr(uint x)
			{
				return new UIntPtr(x);
			}
	unsafe public static explicit operator UIntPtr(ulong x)
			{
				return new UIntPtr(x);
			}
	[CLSCompliant(false)]
	unsafe public static explicit operator UIntPtr(void *x)
			{
				return new UIntPtr(x);
			}
	unsafe public static explicit operator uint(UIntPtr x)
			{
				return x.ToUInt32();
			}
	unsafe public static explicit operator ulong(UIntPtr x)
			{
				return x.ToUInt64();
			}
	[CLSCompliant(false)]
	unsafe public static explicit operator void *(UIntPtr x)
			{
				return x.ToPointer();
			}

#endif // !ECMA_COMPAT

#if CONFIG_SERIALIZATION

	// De-serialize an "UIntPtr" value.
	internal unsafe UIntPtr(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				ulong value = info.GetUInt64("value");
				if(Size == 4)
				{
					if(value > (ulong)(UInt32.MaxValue))
					{
						throw new ArgumentException(_("Overflow_Pointer"));
					}
				}
				value_ = (void *)value;
			}

	// Get the serialization data for an "UIntPtr" value.
	unsafe void ISerializable.GetObjectData(SerializationInfo info,
											StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("value", ToUInt64());
			}

#endif // CONFIG_SERIALIZATION

}; // struct UIntPtr

}; // namespace System
