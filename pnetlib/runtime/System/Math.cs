/*
 * Math.cs - Implementation of the "System.Math" class.
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

using System.Runtime.CompilerServices;

public
#if CONFIG_FRAMEWORK_2_0
static
#else
sealed
#endif
class Math
{
	// Constants.
	public const double E  = 2.7182818284590452354;
	public const double PI = 3.14159265358979323846;

#if !CONFIG_FRAMEWORK_2_0

	// This class cannot be instantiated.
	private Math() {}

#endif // !CONFIG_FRAMEWORK_2_0

	// Get the absolute value of a number.
	[CLSCompliant(false)]
	public static sbyte Abs(sbyte value)
			{
				if(value >= 0)
				{
					return value;
				}
				else if(value != SByte.MinValue)
				{
					return (sbyte)(-value);
				}
				else
				{
					throw new OverflowException
						(_("Overflow_NegateTwosCompNum"));
				}
			}
	public static short Abs(short value)
			{
				if(value >= 0)
				{
					return value;
				}
				else if(value != Int16.MinValue)
				{
					return (short)(-value);
				}
				else
				{
					throw new OverflowException
						(_("Overflow_NegateTwosCompNum"));
				}
			}
	public static int Abs(int value)
			{
				if(value >= 0)
				{
					return value;
				}
				else if(value != Int32.MinValue)
				{
					return -value;
				}
				else
				{
					throw new OverflowException
						(_("Overflow_NegateTwosCompNum"));
				}
			}
	public static long Abs(long value)
			{
				if(value >= 0)
				{
					return value;
				}
				else if(value != Int64.MinValue)
				{
					return -value;
				}
				else
				{
					throw new OverflowException
						(_("Overflow_NegateTwosCompNum"));
				}
			}
	public static float Abs(float value)
			{
				if(value >= 0.0f)
				{
					return value;
				}
				else
				{
					return -value;
				}
			}
	public static double Abs(double value)
			{
				if(value >= 0.0d)
				{
					return value;
				}
				else
				{
					return -value;
				}
			}
	public static Decimal Abs(Decimal value)
			{
				return Decimal.Abs(value);
			}

	// Multiply two 32-bit numbers to get a 64-bit result.
	public static long BigMul(int a, int b)
			{
				return ((long)a) * ((long)b);
			}

	// Divide two numbers and get both the quotient and the remainder.
	public static int DivRem(int a, int b, out int result)
			{
				result = (a % b);
				return (a / b);
			}
	public static long DivRem(long a, long b, out long result)
			{
				result = (a % b);
				return (a / b);
			}

	// Get the logarithm of a number in a specific base.
	public static double Log(double a, double newBase)
			{
				return Log(a) / Log(newBase);
			}

	// Get the maximum of two values.
	[CLSCompliant(false)]
	public static sbyte Max(sbyte val1, sbyte val2)
			{
				return (sbyte)((val1 > val2) ? val1 : val2);
			}
	public static byte Max(byte val1, byte val2)
			{
				return (byte)((val1 > val2) ? val1 : val2);
			}
	public static short Max(short val1, short val2)
			{
				return (short)((val1 > val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ushort Max(ushort val1, ushort val2)
			{
				return (ushort)((val1 > val2) ? val1 : val2);
			}
	public static int Max(int val1, int val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static uint Max(uint val1, uint val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	public static long Max(long val1, long val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ulong Max(ulong val1, ulong val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	public static float Max(float val1, float val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	public static double Max(double val1, double val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	public static Decimal Max(Decimal val1, Decimal val2)
			{
				return Decimal.Max(val1, val2);
			}

	// Get the minimum of two values.
	[CLSCompliant(false)]
	public static sbyte Min(sbyte val1, sbyte val2)
			{
				return (sbyte)((val1 < val2) ? val1 : val2);
			}
	public static byte Min(byte val1, byte val2)
			{
				return (byte)((val1 < val2) ? val1 : val2);
			}
	public static short Min(short val1, short val2)
			{
				return (short)((val1 < val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ushort Min(ushort val1, ushort val2)
			{
				return (ushort)((val1 < val2) ? val1 : val2);
			}
	public static int Min(int val1, int val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static uint Min(uint val1, uint val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	public static long Min(long val1, long val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ulong Min(ulong val1, ulong val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	public static float Min(float val1, float val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	public static double Min(double val1, double val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	public static Decimal Min(Decimal val1, Decimal val2)
			{
				return Decimal.Min(val1, val2);
			}

	// Round a value to a certain number of digits.
	public static double Round(double value, int digits)
			{
				if(digits < 0 || digits > 15)
				{
					throw new ArgumentOutOfRangeException
						("digits", _("ArgRange_RoundDigits"));
				}
				return RoundDouble(value, digits);
			}
#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
	public static double Round(double value, MidpointRounding mode)
			{
				if(mode == MidpointRounding.ToEven)
				{
					return Math.Round(value);
				}
				else if(mode == MidpointRounding.AwayFromZero)
				{
					return RoundDoubleAwayFromZero(value);
				}
				else
				{
					throw new ArgumentException
						(_("Arg_InvalidMidpointRounding"));
				}
			}

	public static double Round(double value, int digits, MidpointRounding mode)
			{
				if(digits < 0 || digits > 15)
				{
					throw new ArgumentOutOfRangeException
						("digits", _("ArgRange_RoundDigits"));
				}
				if(mode == MidpointRounding.ToEven)
				{
					return RoundDouble(value, digits);
				}
				else if(mode == MidpointRounding.AwayFromZero)
				{
					return RoundDoubleAwayFromZero(value, digits);
				}
				else
				{
					throw new ArgumentException
						(_("Arg_InvalidMidpointRounding"));
				}
			}
#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
	public static Decimal Round(Decimal value)
			{
				return Decimal.Round(value, 0);
			}
#if !ECMA_COMPAT
	public static Decimal Round(Decimal value, int decimals)
			{
				return Decimal.Round(value, decimals);
			}
#endif

	// Get the sign of a value.
	[CLSCompliant(false)]
	public static int Sign(sbyte value)
			{
				if(value > 0)
				{
					return 1;
				}
				else if(value < 0)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
	public static int Sign(short value)
			{
				if(value > 0)
				{
					return 1;
				}
				else if(value < 0)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
	public static int Sign(int value)
			{
				if(value > 0)
				{
					return 1;
				}
				else if(value < 0)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
	public static int Sign(long value)
			{
				if(value > 0)
				{
					return 1;
				}
				else if(value < 0)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
	public static int Sign(float value)
			{
				if(Single.IsNaN(value))
				{
					throw new ArithmeticException(_("Arg_NotANumber"));
				}
				if(value > 0)
				{
					return 1;
				}
				else if(value < 0)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
	public static int Sign(double value)
			{
				if(Double.IsNaN(value))
				{
					throw new ArithmeticException(_("Arg_NotANumber"));
				}
				if(value > 0)
				{
					return 1;
				}
				else if(value < 0)
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
	public static int Sign(Decimal value)
			{
				return Decimal.Compare(value, 0.0m);
			}

	// Math methods that are implemented in the runtime engine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Acos(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Asin(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Atan(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Atan2(double y, double x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Ceiling(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Cos(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Cosh(double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Exp(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Floor(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double IEEERemainder(double x, double y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Log(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Log10(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Pow(double x, double y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Round(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static double RoundDouble(double value, int digits);

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static double RoundDoubleAwayFromZero(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static double RoundDoubleAwayFromZero(double a, int digits);
#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Sin(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Sinh(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Sqrt(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Tan(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Tanh(double value);

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Truncate(double d);
#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // class Math

#endif // CONFIG_EXTENDED_NUMERICS

}; // namespace System
