/*
 * Single.cs - Implementation of the "System.Single" class.
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

#if CONFIG_EXTENDED_NUMERICS

using System.Private;
using System.Private.NumberFormat;
using System.Globalization;
using System.Runtime.CompilerServices;

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
[Serializable]
#endif
public struct Single : IComparable, IFormattable
#if !ECMA_COMPAT
	, IConvertible
#endif
#if CONFIG_FRAMEWORK_2_0
	, IComparable<float>, IEquatable<float>
#endif
{
	private float value_;

#if __CSCC__
	public const float MinValue         = __builtin_constant("float_min");
	public const float Epsilon          = __builtin_constant("float_epsilon");
	public const float MaxValue         = __builtin_constant("float_max");
#else
	public const float MinValue         = -3.40282346638528859e38f;
	public const float Epsilon          = 1.4e-45f;
	public const float MaxValue         = 3.40282346638528859e38f;
#endif
	public const float PositiveInfinity = (1.0f / 0.0f);
	public const float NegativeInfinity = (-1.0f / 0.0f);
	public const float NaN              = (0.0f / 0.0f);

	// Override inherited methods.
	public override int GetHashCode()
			{
				if(value_ >= 0.0)
				{
					return unchecked((int)value_);
				}
				else
				{
					return unchecked(-(int)value_);
				}
			}
	public override bool Equals(Object value)
			{
				if(value is Single)
				{
					return (value_ == ((Single)value).value_);
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
				return Formatter.FormatSingle( value_, format, provider );
			}

	// Value testing methods.
	public static bool IsNaN(float f)
			{
				// Comparing a NaN with any other value yields false.
				return (f != f);
			}

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int TestInfinity(float f);

	public static bool IsInfinity(float f)
				{
					return (TestInfinity(f) != 0);
				}
	public static bool IsPositiveInfinity(float f)
				{
					return (f == PositiveInfinity);
				}
	public static bool IsNegativeInfinity(float f)
				{
					return (f == NegativeInfinity);
				}

	// Parsing methods.
	public static float Parse(String s, NumberStyles style,
							  IFormatProvider provider)
			{
				NumberFormatInfo nfi = NumberFormatInfo.GetInstance(provider);
				try
				{
					return NumberParser.ParseSingle(s, style, nfi);
				}
				catch(FormatException)
				{
					String temp = s.Trim();
					if(temp.Equals(nfi.PositiveInfinitySymbol))
					{
						return PositiveInfinity;
					}
					else if(temp.Equals(nfi.NegativeInfinitySymbol))
					{
						return NegativeInfinity;
					}
					else if(temp.Equals(nfi.NaNSymbol))
					{
						return NaN;
					}
					throw;
				}
			}
	public static float Parse(String s)
			{
				return Parse(s, NumberStyles.Float |
							 NumberStyles.AllowThousands, null);
		 	}
	public static float Parse(String s, IFormatProvider provider)
			{
				return Parse(s, NumberStyles.Float |
							 NumberStyles.AllowThousands, provider);
		 	}
	public static float Parse(String s, NumberStyles style)
			{
				return Parse(s, style, null);
			}

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(value is Single)
					{
						float val1 = value_;
			  			float val2 = ((Single)value).value_;
						if(val1 < val2)
						{
							return -1;
						}
						else if(val1 > val2)
						{
							return 1;
						}
						else if(val1 == val2)
						{
							return 0;
						}
						else if(IsNaN(val1))
						{
							if(IsNaN(val2))
							{
								return 0;
							}
							else
							{
								return -1;
							}
						}
						else
						{
							return 1;
						}
					}
					else
					{
						throw new ArgumentException(_("Arg_MustBeSingle"));
					}
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<float> interface.
	public int CompareTo(float value)
			{
				if(value_ < value.value_)
				{
					return -1;
				}
				else if(value_ > value.value_)
				{
					return 1;
				}
				else if(value_ == value.value_)
				{
					return 0;
				}
				else if(IsNaN(value_))
				{
					if(IsNaN(value.value_))
					{
						return 0;
					}
					else
					{
						return -1;
					}
				}
				else
				{
					return 1;
				}
			}

	// Implementation of the IEquatable<float> interface.
	public bool Equals(float obj)
			{
				return ((value_ == obj.value_) ||
						(IsNaN(value_) && IsNaN(obj.value_)));
			}

#endif // CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT

	// Implementation of the IConvertible interface.
	public TypeCode GetTypeCode()
			{
				return TypeCode.Single;
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
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Single", "Char"));
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
				return value_;
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
						(_("InvalidCast_FromTo"), "Single", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

}; // class Single

#endif // CONFIG_EXTENDED_NUMERICS

}; // namespace System
