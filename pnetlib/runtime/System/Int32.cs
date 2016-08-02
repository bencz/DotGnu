/*
 * Int32.cs - Implementation of the "System.Int32" class.
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

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
[Serializable]
#endif
public struct Int32 : IComparable, IFormattable
#if !ECMA_COMPAT
	, IConvertible
#endif
#if CONFIG_FRAMEWORK_2_0
	, IComparable<int>, IEquatable<int>
#endif
{
	private int value_;

	public const int MaxValue = 0x7FFFFFFF;
	public const int MinValue = unchecked((int)(-0x80000000));

	// Override inherited methods.
	public override int GetHashCode()
			{ return (value_); }
	public override bool Equals(Object value)
			{
				if(value is Int32)
				{
					return (value_ == ((Int32)value).value_);
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
				return Formatter.FormatInt32( value_, format, provider );
			}

	// Parsing methods.
	public static int Parse(String s, NumberStyles style,
							IFormatProvider provider)
			{
				NumberParser.ValidateIntegerStyle(style);
				return NumberParser.ParseInt32
					(s, style, NumberFormatInfo.GetInstance(provider), 0);
			}
	public static int Parse(String s)
			{
				return Parse(s, NumberStyles.Integer, null);
			}
	public static int Parse(String s, IFormatProvider provider)
			{
				return Parse(s, NumberStyles.Integer, provider);
			}
	public static int Parse(String s, NumberStyles style)
			{
				return Parse(s, style, null);
			}

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(!(value is Int32))
					{
						throw new ArgumentException(_("Arg_MustBeInt32"));
					}
					int temp = ((Int32)value).value_;
					if(value_ < temp)
					{
						return -1;
					}
					else if(value_ > temp)
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

	// Implementation of the IComparable<int> interface.
	public int CompareTo(int value)
			{
				return (value_ - value.value_);
			}

	// Implementation of the IEquatable<int> interface.
	public bool Equals(int obj)
			{
				return (value_ == obj.value_);
			}

#endif // CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT

	// Implementation of the IConvertible interface.
	public TypeCode GetTypeCode()
			{
				return TypeCode.Int32;
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
				return value_;
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
						(_("InvalidCast_FromTo"), "Int32", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

}; // class Int32

}; // namespace System
