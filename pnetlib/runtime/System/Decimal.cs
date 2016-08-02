/*
 * Decimal.cs - Implementation of the "System.Decimal" class.
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

#if CONFIG_EXTENDED_NUMERICS

using System.Private;
using System.Private.NumberFormat;
using System.Globalization;
using System.Runtime.CompilerServices;

public struct Decimal : IComparable, IFormattable
#if !ECMA_COMPAT
	, IConvertible
#endif
#if CONFIG_FRAMEWORK_2_0
	, IComparable<decimal>, IEquatable<decimal>
#endif
{
	private int flags, high, middle, low;

	public const decimal Zero = 0.0m;
	public const decimal One = 1.0m;
	public const decimal MinusOne = -1.0m;
	public const decimal MaxValue = 79228162514264337593543950335.0m;
	public const decimal MinValue = -79228162514264337593543950335.0m;
	private const int DecimalScalePosition = 16;

	// Public routines that are imported from the runtime engine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Add(decimal x, decimal y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Compare(decimal x, decimal y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Divide(decimal x, decimal y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Floor(decimal x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Remainder(decimal x, decimal y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Multiply(decimal x, decimal y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Negate(decimal x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Round(decimal x, int decimals);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Subtract(decimal x, decimal y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static decimal Truncate(decimal x);

	// Other routines that can be implemented in IL bytecode.
	internal static decimal Abs(decimal value)
			{
				return new Decimal(value.low, value.middle,
								   value.high, value.flags & 0x7FFFFFFF);
			}
	public static bool Equals(decimal x, decimal y)
			{
				return (Compare(x, y) == 0);
			}
	internal static decimal Max(decimal x, decimal y)
			{
				if(Compare(x, y) > 0)
					return x;
				else
					return y;
			}
	internal static decimal Min(decimal x, decimal y)
			{
				if(Compare(x, y) < 0)
					return x;
				else
					return y;
			}

	// Declared operators.
	public static decimal operator++(decimal x)
				{ return x + 1.0m; }
	public static decimal operator--(decimal x)
				{ return x - 1.0m; }
	public static decimal operator+(decimal x)
				{ return x; }
	public static decimal operator-(decimal x)
				{ return Negate(x); }
	public static decimal operator*(decimal x, decimal y)
				{ return Multiply(x, y); }
	public static decimal operator/(decimal x, decimal y)
				{ return Divide(x, y); }
	public static decimal operator%(decimal x, decimal y)
				{ return Remainder(x, y); }
	public static decimal operator+(decimal x, decimal y)
				{ return Add(x, y); }
	public static decimal operator-(decimal x, decimal y)
				{ return Subtract(x, y); }
	public static bool operator==(decimal x, decimal y)
				{ return (Compare(x, y) == 0); }
	public static bool operator!=(decimal x, decimal y)
				{ return (Compare(x, y) != 0); }
	public static bool operator<(decimal x, decimal y)
				{ return (Compare(x, y) < 0); }
	public static bool operator>(decimal x, decimal y)
				{ return (Compare(x, y) > 0); }
	public static bool operator<=(decimal x, decimal y)
				{ return (Compare(x, y) <= 0); }
	public static bool operator>=(decimal x, decimal y)
				{ return (Compare(x, y) >= 0); }
	[CLSCompliant(false)]
	public static implicit operator decimal(sbyte x)
				{ return new Decimal((int)x); }
	public static implicit operator decimal(byte x)
				{ return new Decimal((uint)x); }
	public static implicit operator decimal(short x)
				{ return new Decimal((int)x); }
	[CLSCompliant(false)]
	public static implicit operator decimal(ushort x)
				{ return new Decimal((uint)x); }
	public static implicit operator decimal(int x)
				{ return new Decimal(x); }
	[CLSCompliant(false)]
	public static implicit operator decimal(uint x)
				{ return new Decimal(x); }
	public static implicit operator decimal(long x)
				{ return new Decimal(x); }
	[CLSCompliant(false)]
	public static implicit operator decimal(ulong x)
				{ return new Decimal(x); }
	public static implicit operator decimal(char x)
				{ return new Decimal((uint)(ushort)x); }
	public static explicit operator decimal(float x)
				{ return new Decimal(x); }
	public static explicit operator decimal(double x)
				{ return new Decimal(x); }
	[CLSCompliant(false)]
	public static explicit operator sbyte(decimal x)
				{ return ToSByte(x); }
	public static explicit operator byte(decimal x)
				{ return ToByte(x); }
	public static explicit operator short(decimal x)
				{ return ToInt16(x); }
	[CLSCompliant(false)]
	public static explicit operator ushort(decimal x)
				{ return ToUInt16(x); }
	public static explicit operator int(decimal x)
				{ return ToInt32(x); }
	[CLSCompliant(false)]
	public static explicit operator uint(decimal x)
				{ return ToUInt32(x); }
	public static explicit operator long(decimal x)
				{ return ToInt64(x); }
	[CLSCompliant(false)]
	public static explicit operator ulong(decimal x)
				{ return ToUInt64(x); }
	public static explicit operator char(decimal x)
				{ return (char)(ToUInt16(x)); }
	public static explicit operator float(decimal x)
				{ return ToSingle(x); }
	public static explicit operator double(decimal x)
				{ return ToDouble(x); }

	// Constructors.
	public Decimal(int value)
			{
				if(value >= 0)
				{
					low = value;
					flags = 0;
				}
				else
				{
					low = unchecked((int)(uint)(-value));
					flags = unchecked((int)(-0x80000000));
				}
				middle = 0;
				high = 0;
			}
	[CLSCompliant(false)]
	public Decimal(uint value)
			{
				low = unchecked((int)value);
				middle = 0;
				high = 0;
				flags = 0;
			}
	public Decimal(long value)
			{
				if(value >= 0)
				{
					low = unchecked((int)value);
					middle = unchecked((int)(value >> 32));
					flags = 0;
				}
				else
				{
					value = -value;
					low = unchecked((int)value);
					middle = unchecked((int)(value >> 32));
					flags = unchecked((int)(-0x80000000));
				}
				high = 0;
			}
	[CLSCompliant(false)]
	public Decimal(ulong value)
			{
				low = unchecked((int)value);
				middle = unchecked((int)(value >> 32));
				high = 0;
				flags = 0;
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public Decimal(float value);
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public Decimal(double value);
	public Decimal(int[] bits)
			{
				if(bits == null)
				{
					throw new ArgumentNullException("bits");
				}
				if(bits.Length == 4 &&
				   (bits[3] & 0x7F00FFFF) == 0 &&
				   (bits[3] & 0x00FF0000) <= (28 << DecimalScalePosition))
				{
					low = bits[0];
					middle = bits[1];
					high = bits[2];
					flags = bits[3];
				}
				else
				{
					throw new ArgumentException(_("Arg_DecBitCtor"));
				}
			}

	// Note: this contructor is not ECMA-compliant, but Microsoft's Beta 2
	// C# compiler will explode if it isn't present.  So we have to include
	// this constructor even when compiling with ECMA_COMPAT.
	public Decimal(int _low, int _middle, int _high, bool _isneg, byte _scale)
			{
				if(_scale <= 28)
				{
					low = _low;
					middle = _middle;
					high = _high;
					flags = (((int)_scale) << DecimalScalePosition) |
							(_isneg ? unchecked((int)0x80000000) : 0);
				}
				else
				{
					throw new ArgumentOutOfRangeException
						(_("ArgRange_DecimalScale"));
				}
			}

	// Private constructor used only by this class.
	private Decimal(int _low, int _middle, int _high, int _flags)
			{
				low = _low;
				middle = _middle;
				high = _high;
				flags = _flags;
			}

	// Override inherited methods.
	public override int GetHashCode()
			{ return (low ^ middle ^ high ^ flags); }
	public override bool Equals(Object obj)
			{
				if(obj is Decimal)
				{
					return (Compare((decimal)this, (decimal)obj) == 0);
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
				if (format == null) format = "G";
				return 
					Formatter.CreateFormatter(format).Format(this, provider);
			}

	// Parsing methods.
	public static decimal Parse(String s, NumberStyles style,
							    IFormatProvider provider)
			{
				return NumberParser.ParseDecimal
						(s, style, NumberFormatInfo.GetInstance(provider));
			}
	public static decimal Parse(String s)
			{
				return Parse(s, NumberStyles.Currency, null);
			}
	public static decimal Parse(String s, NumberStyles style)
			{
				return Parse(s, style, null);
			}
	public static decimal Parse(String s, IFormatProvider provider)
			{
				return Parse(s, NumberStyles.Currency, provider);
			}

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(value is Decimal)
					{
						return Compare((decimal)this, (decimal)value);
					}
					else
					{
						throw new ArgumentException(_("Arg_MustBeDecimal"));
					}
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<decimal> interface.
	public int CompareTo(decimal value)
			{
				return Compare(this, value);
			}

	// Implementation of the IEquatable<decimal> interface.
	public bool Equals(decimal obj)
			{
				return (Compare(this, obj) == 0);
			}

#endif // CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT

	// Implementation of the IConvertible interface.
	public TypeCode GetTypeCode()
			{
				return TypeCode.Decimal;
			}
	bool IConvertible.ToBoolean(IFormatProvider provider)
			{
				return Convert.ToBoolean(this);
			}
	byte IConvertible.ToByte(IFormatProvider provider)
			{
				return Convert.ToByte(this);
			}
	sbyte IConvertible.ToSByte(IFormatProvider provider)
			{
				return Convert.ToSByte(this);
			}
	short IConvertible.ToInt16(IFormatProvider provider)
			{
				return Convert.ToInt16(this);
			}
	ushort IConvertible.ToUInt16(IFormatProvider provider)
			{
				return Convert.ToUInt16(this);
			}
	char IConvertible.ToChar(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Decimal", "Char"));
			}
	int IConvertible.ToInt32(IFormatProvider provider)
			{
				return Convert.ToInt32(this);
			}
	uint IConvertible.ToUInt32(IFormatProvider provider)
			{
				return Convert.ToUInt32(this);
			}
	long IConvertible.ToInt64(IFormatProvider provider)
			{
				return Convert.ToInt64(this);
			}
	ulong IConvertible.ToUInt64(IFormatProvider provider)
			{
				return Convert.ToUInt64(this);
			}
	float IConvertible.ToSingle(IFormatProvider provider)
			{
				return Convert.ToSingle(this);
			}
	double IConvertible.ToDouble(IFormatProvider provider)
			{
				return Convert.ToDouble(this);
			}
	Decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				return this;
			}
	DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Decimal", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

	// Static conversion methods.
	private static int TruncateToInt32(decimal value)
			{
				decimal temp = Truncate(value);
				if(temp.flags >= 0)
				{
					return temp.low;
				}
				else
				{
					return unchecked(-(temp.low));
				}
			}

#if !ECMA_COMPAT

	// Non-ECMA declares these as public.

	public static byte ToByte(decimal value)
			{
				if(value >= 0.0m && value <= 255.0m)
				{
					return unchecked((byte)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(decimal value)
			{
				if(value >= -128.0m && value <= 127.0m)
				{
					return unchecked((sbyte)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	public static short ToInt16(decimal value)
			{
				if(value >= -32768.0m && value <= 32767.0m)
				{
					return unchecked((short)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(decimal value)
			{
				if(value >= 0.0m && value <= 65535.0m)
				{
					return unchecked((ushort)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	public static int ToInt32(decimal value)
			{
				if(value >= -2147483648.0m && value <= 2147483647.0m)
				{
					return unchecked(TruncateToInt32(value));
				}
				else
				{
					throw new OverflowException(_("Overflow_Int32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(decimal value)
			{
				if(value >= 0.0m && value <= 4294967295.0m)
				{
					decimal temp = Truncate(value);
					return unchecked((uint)(temp.low));
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	public static long ToInt64(decimal value)
			{
				if(value >= -9223372036854775808.0m &&
				   value <= 9223372036854775807.0m)
				{
					decimal temp = Truncate(value);
					if(temp.flags >= 0)
					{
						return unchecked((((long)(temp.middle)) << 32) |
										  ((long)(ulong)(uint)(temp.low)));
					}
					else
					{
						return unchecked(-((((long)(temp.middle)) << 32) |
										    ((long)(ulong)(uint)(temp.low))));
					}
				}
				else
				{
					throw new OverflowException(_("Overflow_Int64"));
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(decimal value)
			{
				if(value >= 0.0m && value <= 18446744073709551615.0m)
				{
					decimal temp = Truncate(value);
					return unchecked((((ulong)(uint)(temp.middle)) << 32) |
									  ((ulong)(uint)(temp.low)));
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static float ToSingle(decimal value);
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double ToDouble(decimal value);

#else // ECMA_COMPAT

	// ECMA does not have these, but we need them to implement "Convert".

	internal static byte ToByte(decimal value)
			{
				if(value >= 0.0m && value <= 255.0m)
				{
					return unchecked((byte)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	internal static sbyte ToSByte(decimal value)
			{
				if(value >= -128.0m && value <= 127.0m)
				{
					return unchecked((sbyte)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	internal static short ToInt16(decimal value)
			{
				if(value >= -32768.0m && value <= 32767.0m)
				{
					return unchecked((short)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	internal static ushort ToUInt16(decimal value)
			{
				if(value >= 0.0m && value <= 65535.0m)
				{
					return unchecked((ushort)(TruncateToInt32(value)));
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	internal static int ToInt32(decimal value)
			{
				if(value >= -2147483648.0m && value <= 2147483647.0m)
				{
					return unchecked(TruncateToInt32(value));
				}
				else
				{
					throw new OverflowException(_("Overflow_Int32"));
				}
			}
	internal static uint ToUInt32(decimal value)
			{
				if(value >= 0.0m && value <= 4294967295.0m)
				{
					decimal temp = Truncate(value);
					return unchecked((uint)(temp.low));
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	internal static long ToInt64(decimal value)
			{
				if(value >= -9223372036854775808.0m &&
				   value <= 9223372036854775807.0m)
				{
					decimal temp = Truncate(value);
					if(temp.flags >= 0)
					{
						return unchecked((((long)(temp.middle)) << 32) |
										  ((long)(ulong)(uint)(temp.low)));
					}
					else
					{
						return unchecked(-((((long)(temp.middle)) << 32) |
										    ((long)(ulong)(uint)(temp.low))));
					}
				}
				else
				{
					throw new OverflowException(_("Overflow_Int64"));
				}
			}
	internal static ulong ToUInt64(decimal value)
			{
				if(value >= 0.0m && value <= 18446744073709551615.0m)
				{
					decimal temp = Truncate(value);
					return unchecked((((ulong)(uint)(temp.middle)) << 32) |
									  ((ulong)(uint)(temp.low)));
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static float ToSingle(decimal value);
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static double ToDouble(decimal value);

#endif // ECMA_COMPAT

	// Get the bits of a decimal value.
	public static int[] GetBits(decimal x)
			{
				int[] result = new int [4];
				result[0] = x.low;
				result[1] = x.middle;
				result[2] = x.high;
				result[3] = x.flags;
				return result;
			}

	// Throw an overflow exception, with the correct translated message.
	// This is called by the runtime engine.
	private static void ThrowOverflow()
			{
				throw new OverflowException(_("Overflow_Decimal"));
			}

	// Throw a division by zero exception, with the correct translated message.
	// This is called by the runtime engine.
	private static void ThrowDivZero()
			{
				throw new DivideByZeroException(_("DivZero_Decimal"));
			}

	// Throw a "decimal scale out of range" exception, with the correct
	// translated message.  This is called by the runtime engine.
	private static void ThrowDecimals()
			{
				throw new ArgumentOutOfRangeException
					(_("ArgRange_DecimalScale"));
			}

#if !ECMA_COMPAT

	// Convert an OA currency value into a Decimal value.
	// An OA currency value is a 64-bit fixed-point value with
	// four places after the decimal point.
	public static Decimal FromOACurrency(long cy)
			{
				return ((Decimal)cy) / 10000.0m;
			}

	// Convert a Decimal value into an OA currency value.
	public static long ToOACurrency(Decimal value)
			{
				try
				{
					return (long)(value * 10000.0m);
				}
				catch(OverflowException)
				{
					// Change the message string in the overflow exception.
					throw new OverflowException(_("Overflow_Currency"));
				}
			}

#endif // !ECMA_COMPAT

}; // class Decimal

#endif // CONFIG_EXTENDED_NUMERICS

}; // namespace System
