/*
 * UInt64.cs - Implementation of the "System.UInt64" class.
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

using System.Private;
using System.Private.NumberFormat;
using System.Globalization;

[CLSCompliant(false)]
public struct UInt64 : IComparable, IFormattable
#if !ECMA_COMPAT
	, IConvertible
#endif
#if CONFIG_FRAMEWORK_2_0
	, IComparable<ulong>, IEquatable<ulong>
#endif
{
	private ulong value_;

	public const ulong MaxValue = 0xFFFFFFFFFFFFFFFF;
	public const ulong MinValue = 0;

	// Override inherited methods.
	public override int GetHashCode()
			{
				return unchecked((int)(value_ ^ (value_ >> 32)));
			}
	public override bool Equals(Object value)
			{
				if(value is UInt64)
				{
					return (value_ == ((UInt64)value).value_);
				}
				else
				{
					return false;
				}
			}

	// String conversion.
	public override String ToString()
			{
				return ToString(null, null);
			}
	public String ToString(String format)
			{
				return ToString(format, null);
			}

	public String ToString(IFormatProvider provider)
			{
				return ToString(null, provider);
			}

	public String ToString(String format, IFormatProvider provider)
			{
				return Formatter.FormatUInt64( value_, format, provider );
			}

	// Parsing methods.
	[CLSCompliant(false)]
	public static ulong Parse(String s, NumberStyles style,
							  IFormatProvider provider)
			{
				NumberParser.ValidateIntegerStyle(style);
				return NumberParser.ParseUInt64
					(s, style, NumberFormatInfo.GetInstance(provider), 0);
			}
	[CLSCompliant(false)]
	public static ulong Parse(String s)
			{
				return Parse(s, NumberStyles.Integer, null);
			}
	[CLSCompliant(false)]
	public static ulong Parse(String s, IFormatProvider provider)
			{
				return Parse(s, NumberStyles.Integer, provider);
			}
	[CLSCompliant(false)]
	public static ulong Parse(String s, NumberStyles style)
			{
				return Parse(s, style, null);
			}

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(!(value is UInt64))
					{
						throw new ArgumentException(_("Arg_MustBeUInt64"));
					}
					ulong value2 = ((UInt64)value).value_;
					if(value_ < value2)
					{
						return -1;
					}
					else if(value_ > value2)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<ulong> interface.
	public int CompareTo(ulong value)
			{
				if(value_ < value.value_)
				{
					return -1;
				}
				else if(value_ > value.value_)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

	// Implementation of the IEquatable<ulong> interface.
	public bool Equals(ulong obj)
			{
				return (value_ == obj.value_);
			}

#endif // CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT

	// Implementation of the IConvertible interface.
	public TypeCode GetTypeCode()
			{
				return TypeCode.UInt64;
			}
	bool IConvertible.ToBoolean(IFormatProvider provider)
			{
				return Convert.ToBoolean(value_);
			}
	byte IConvertible.ToByte(IFormatProvider provider)
			{
				return Convert.ToByte(value_);
			}
	sbyte IConvertible.ToSByte(IFormatProvider provider)
			{
				return Convert.ToSByte(value_);
			}
	short IConvertible.ToInt16(IFormatProvider provider)
			{
				return Convert.ToInt16(value_);
			}
	ushort IConvertible.ToUInt16(IFormatProvider provider)
			{
				return Convert.ToUInt16(value_);
			}
	char IConvertible.ToChar(IFormatProvider provider)
			{
				return Convert.ToChar(value_);
			}
	int IConvertible.ToInt32(IFormatProvider provider)
			{
				return Convert.ToInt32(value_);
			}
	uint IConvertible.ToUInt32(IFormatProvider provider)
			{
				return Convert.ToUInt32(value_);
			}
	long IConvertible.ToInt64(IFormatProvider provider)
			{
				return Convert.ToInt64(value_);
			}
	ulong IConvertible.ToUInt64(IFormatProvider provider)
			{
				return Convert.ToUInt64(value_);
			}
	float IConvertible.ToSingle(IFormatProvider provider)
			{
				return Convert.ToSingle(value_);
			}
	double IConvertible.ToDouble(IFormatProvider provider)
			{
				return Convert.ToDouble(value_);
			}
	Decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				return Convert.ToDecimal(value_);
			}
	DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "UInt64", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

}; // class UInt64

}; // namespace System
