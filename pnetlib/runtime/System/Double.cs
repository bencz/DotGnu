/*
 * Double.cs - Implementation of the "System.Double" class.
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
public struct Double : IComparable, IFormattable
#if !ECMA_COMPAT
	, IConvertible
#endif
#if CONFIG_FRAMEWORK_2_0
	, IComparable<double>, IEquatable<double>
#endif
{
	private double value_;

#if __CSCC__
	public const double MinValue         = __builtin_constant("double_min");
	public const double Epsilon          = __builtin_constant("double_epsilon");
	public const double MaxValue         = __builtin_constant("double_max");
#else
	public const double MinValue         = -1.7976931348623157E+308;
	public const double Epsilon          = 4.94065645841247e-324;
	public const double MaxValue         = 1.7976931348623157E+308;
#endif
	public const double PositiveInfinity = (1.0 / 0.0);
	public const double NegativeInfinity = (-1.0 / 0.0);
	public const double NaN              = (0.0 / 0.0);

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
				if(value is Double)
				{
					/* Note: ECMA spec says that "NaN!=NaN" but *
					 * NaN.Equals(NaN)==true , strange but true */

					double dvalue=((Double)value).value_;
					return ((value_ == dvalue) || 
							(IsNaN(value_) && IsNaN(dvalue)));
				}
				else
				{
					return false;
				}
			}

	// Value testing methods.
	public static bool IsNaN(double d)
			{
				// Comparing a NaN with any other value yields false.
				return (d != d);
			}

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int TestInfinity(double d);

	public static bool IsInfinity(double d)
				{
					return (TestInfinity(d) != 0);
				}
	public static bool IsPositiveInfinity(double d)
				{
					return (d == PositiveInfinity);
				}
	public static bool IsNegativeInfinity(double d)
				{
					return (d == NegativeInfinity);
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
				if( IsNaN( value_ ) ) {
					NumberFormatInfo nfi = NumberFormatInfo.GetInstance(provider);
					return nfi.NaNSymbol;
				}
				else if( IsInfinity( value_ ) ) {
					NumberFormatInfo nfi = NumberFormatInfo.GetInstance(provider);
					if( IsPositiveInfinity( value_ ) ) {
						return nfi.PositiveInfinitySymbol;
					}
					else {
						return nfi.NegativeInfinitySymbol;
					}
				}
				
				return Formatter.FormatDouble( value_, format, provider );
			}

	// Parsing methods.
	public static double Parse(String s, NumberStyles style,
							   IFormatProvider provider)
			{
				NumberFormatInfo nfi = NumberFormatInfo.GetInstance(provider);
				try
				{
					return NumberParser.ParseDouble(s, style, nfi);
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
	public static double Parse(String s)
			{
				return Parse(s, NumberStyles.Float |
							 NumberStyles.AllowThousands, null);
		 	}
	public static double Parse(String s, IFormatProvider provider)
			{
				return Parse(s, NumberStyles.Float |
							 NumberStyles.AllowThousands, provider);
		 	}
	public static double Parse(String s, NumberStyles style)
			{
				return Parse(s, style, null);
			}

#if !ECMA_COMPAT
	// Try to parse, and return a boolean on failure.
	public static bool TryParse(String s, NumberStyles style,
								IFormatProvider provider,
								out double result)
			{
				try
				{
					result = Parse(s, style, provider);
					return true;
				}
				catch(Exception)
				{
					result = 0.0;
					return false;
				}
			}
#endif

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(value is Double)
					{
						double val1 = value_;
			  			double val2 = ((Double)value).value_;

						/* Note: the order of these if statements are
						 * important as cscc often uses bge.s to simplify
						 * less than if statements. But because NaN < NaN
						 * and NaN > NaN are both false, this runs into some
						 * queer issues */

						if(val1 == val2)
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
						else if(val1 < val2)
						{
							return -1;
						}
						else if(val1 > val2)
						{
							return 1;
						}
						else
						{
							return 1;
						}
					}
					else
					{
						throw new ArgumentException(_("Arg_MustBeDouble"));
					}
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<double> interface.
	public int CompareTo(double value)
			{
				if(value_ == value)
				{
					return 0;
				}
				else if(IsNaN(value_))
				{
					if(IsNaN(value))
					{
						return 0;
					}
					else
					{
						return -1;
					}
				}
				else if(value_ < value)
				{
					return -1;
				}
				else if(value_ > value)
				{
					return 1;
				}
				else
				{
					return 1;
				}
			}

	// Implementation of the IEquatable<double> interface.
	public bool Equals(double obj)
			{
				// We have to handle NaN values especially
				return ((value_ == obj.value_) ||
						(IsNaN(this) && IsNaN(obj)));
			}

#endif // CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT

	// Implementation of the IConvertible interface.
	public TypeCode GetTypeCode()
			{
				return TypeCode.Double;
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
						(_("InvalidCast_FromTo"), "Double", "Char"));
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
				return value_;
			}
	Decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				return Convert.ToDecimal(value_);
			}
	DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Double", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

}; // class Double

#endif // CONFIG_EXTENDED_NUMERICS

}; // namespace System
