/*
 * Formatter.cs - Implementation of the
 *          "System.Private.NumberFormat.Formatter" class.
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

namespace System.Private.NumberFormat
{

using System;
using System.Collections;
using System.Globalization;
using System.Text;

internal abstract class Formatter
{
	// ----------------------------------------------------------------------
	//  Private static variables for formatter
	//
	private const string validformats = "CcDdEeFfGgNnPpRrXx";
	static private IDictionary formats = new Hashtable();
	// We need this ULongMaxValue, because cscc has a bug converting ulong.MaxValue to double
	static private double ULongMaxValue = 18446744073709551615d;

	// ----------------------------------------------------------------------
	//  Protected data for other methods
	//
	static protected readonly char[] decimalDigits =
	{'0','1','2','3','4','5','6','7','8','9'};

	// ----------------------------------------------------------------------
	//  Protected state information
	//
	protected int precision;

	// ----------------------------------------------------------------------
	//  Protected utility methods
	//
	static protected NumberFormatInfo 
		NumberFormatInfo(IFormatProvider provider)
	{
		if(provider == null)
		{
			return System.Globalization.NumberFormatInfo.CurrentInfo;
		}
		else
		{
			NumberFormatInfo nfi =
				(NumberFormatInfo) provider.GetFormat(
								typeof(System.Globalization.NumberFormatInfo));
			if(nfi != null)
			{
				return nfi;
			}
			else
			{
				return System.Globalization.NumberFormatInfo.CurrentInfo;
			}
		}
	}

	static protected bool IsSignedInt(Object o)
	{
		return ( o is Int32 || o is Int16 || o is Int64 || o is SByte  );
	}
	
	static protected bool IsUnsignedInt(Object o)
	{
		return (o is UInt32 ||  o is UInt16 || o is UInt64 || o is Byte );
	}
	
#if CONFIG_EXTENDED_NUMERICS
	static protected bool IsFloat(Object o)
	{
		return (o is Double || o is Single );
	}
	
	static protected bool IsDecimal(Object o)
	{
		return (o is Decimal);
	}

	static protected double OToDouble(Object o)
	{
		double ret;

		if (o is Int32)
		{
			Int32 n = (Int32)o;
			ret = (double)n;
		}
		else if (o is UInt32)
		{
			UInt32 n = (UInt32)o;
			ret = (double)n;
		}
		else if (o is SByte)
		{
			SByte n = (SByte)o;
			ret = (double)n;
		}
		else if (o is Byte)
		{
			Byte n = (Byte)o;
			ret = (double)n;
		}
		else if (o is Int16)
		{
			Int16 n = (Int16)o;
			ret = (double)n;
		}
		else if (o is UInt16)
		{
			UInt16 n = (UInt16)o;
			ret = (double)n;
		}
		else if (o is Int64)
		{
			Int64 n = (Int64)o;
			ret = (double)n;
		}
		else if (o is UInt64)
		{
			UInt64 n = (UInt64)o;
			ret = (double)n;
		}
		else if (o is Single)
		{
			Single n = (Single)o;
			ret = (double)n;
		}
		else if (o is Double)
		{
			ret = (double)o;
		}
		else if (o is Decimal)
		{
			Decimal n = (Decimal)o;
			ret = (double)n;
		}
		else
		{
			throw new FormatException(_("Format_TypeException"));
		}
		return ret;
	}
#endif // CONFIG_EXTENDED_NUMERICS

	static protected ulong OToUlong(Object o)
	{
		ulong ret;

		if (o is Int32)
		{
			Int32 n = (Int32)o;
			ret = (ulong)n;
		}
		else if (o is UInt32)
		{
			UInt32 n = (UInt32)o;
			ret = (ulong)n;
		}
		else if (o is SByte)
		{
			SByte n = (SByte)o;
			ret = (ulong)n;
		}
		else if (o is Byte)
		{
			Byte n = (Byte)o;
			ret = (ulong)n;
		}
		else if (o is Int16)
		{
			Int16 n = (Int16)o;
			ret = (ulong)n;
		}
		else if (o is UInt16)
		{
			UInt16 n = (UInt16)o;
			ret = (ulong)n;
		}
		else if (o is Int64)
		{
			Int64 n = (Int64)o;
			ret = (ulong)n;
		}
		else if (o is UInt64)
		{
			ret = (ulong)o;
		}
#if CONFIG_EXTENDED_NUMERICS
		else if (o is Single)
		{
			Single n = (Single)o;
			ret = (ulong)n;
		}
		else if (o is Double)
		{
			Double n = (Double)o;
			ret = (ulong)n;
		}
		else if (o is Decimal)
		{
			Decimal n = (Decimal)o;
			ret = (ulong)n;
		}
#endif
		else
		{
			throw new FormatException(_("Format_TypeException"));
		}
		return ret;
	}

	static protected long OToLong(Object o)
	{
		long ret;

		if (o is Int32)
		{
			Int32 n = (Int32)o;
			ret = (long)n;
		}
		else if (o is UInt32)
		{
			UInt32 n = (UInt32)o;
			ret = (long)n;
		}
		else if (o is SByte)
		{
			SByte n = (SByte)o;
			ret = (long)n;
		}
		else if (o is Byte)
		{
			Byte n = (Byte)o;
			ret = (long)n;
		}
		else if (o is Int16)
		{
			Int16 n = (Int16)o;
			ret = (long)n;
		}
		else if (o is UInt16)
		{
			UInt16 n = (UInt16)o;
			ret = (long)n;
		}
		else if (o is Int64)
		{
			ret = (long)o;
		}
		else if (o is UInt64)
		{
			UInt64 n = (UInt64)o;
			ret = (long)n;
		}
#if CONFIG_EXTENDED_NUMERICS
		else if (o is Single)
		{
			Single n = (Single)o;
			ret = (long)n;
		}
		else if (o is Double)
		{
			Double n = (Double)o;
			ret = (long)n;
		}
		else if (o is Decimal)
		{
			Decimal n = (Decimal)o;
			ret = (long)n;
		}
#endif
		else
		{
			throw new FormatException(_("Format_TypeException"));
		}
		return ret;
	}

	static protected string FormatAnyRound(Object o, int precision,
										   IFormatProvider provider)
	{
		string ret = null;

		//  Type validation
		if (IsSignedInt(o) )
		{
			long  lvalue = OToLong(o);
			if( lvalue < 0 ) {
				ulong value=(ulong) -lvalue;
	
				if(value==0)
				{
					// because -(Int64.MinValue) does not exist
					ret = "-9223372036854775808.";
				}
				else
				{
					ret = "-" + Formatter.FormatInteger(value);
				}
			}
			else {
				ret = Formatter.FormatInteger(OToUlong(o));
			}
		}
		else if (IsUnsignedInt(o))
		{
			ret = Formatter.FormatInteger(OToUlong(o));
		}

#if CONFIG_EXTENDED_NUMERICS
		else if (IsDecimal(o))
		{
			// Rounding code
			decimal r;
			int i;

			for (i=0, r=0.5m; i < precision; i++) 
			{
				r *= 0.1m;
			}

			//  Pick the call based on the inputs negativity.
			if ((decimal)o < 0)
			{
				ret = "-" + Formatter.FormatDecimal(-((decimal)o)+r);
			}
			else
			{
				ret = Formatter.FormatDecimal((decimal)o+r);
			}

			if (ret.Length - ret.IndexOf('.') > precision + 1) 
			{
				ret = ret.Substring(0, ret.IndexOf('.')+precision+1);
			}
		}
		else if (IsFloat(o))
		{
			// Beware rounding code
			double val = OToDouble(o);
			if (Double.IsNaN(val))
			{
				return NumberFormatInfo(provider).NaNSymbol;
			}
			else if( Double.IsInfinity(val) ) {
				if (Double.IsPositiveInfinity(val))
				{
					return NumberFormatInfo(provider).PositiveInfinitySymbol;
				}
				else if (Double.IsNegativeInfinity(val))
				{
					return NumberFormatInfo(provider).NegativeInfinitySymbol;
				}
			}
			else {
				// do not round here, is done in Formatter.FormatFloat
				if (val < 0) ret = "-" + Formatter.FormatFloat( -val, precision );
				else         ret =       Formatter.FormatFloat(  val, precision );

			}
		}
#endif
		else
		{
			//  This is a bad place to be.
			throw new FormatException(_("Format_TypeException"));	
		}
		return ret;
	}

	static protected string FormatInteger(ulong value)
	{
		//  Note:  CustomFormatter counts on having the trailing decimal
		//  point.  If you're considering taking it out, think hard.
		
		if (value == 0) return ".";

		StringBuilder ret = new StringBuilder(".", 30 );
		ulong work = value;

		while (work > 0) {
			ret.Insert(0, decimalDigits[work % 10]);
			work /= 10;
		}

		return ret.ToString();
	}

#if CONFIG_EXTENDED_NUMERICS
	static protected string FormatDecimal(decimal value)
	{
		//  Guard clause(s)
		if (value == 0.0m) return ".";

		//  Variable declarations
		int [] bits = Decimal.GetBits(value);
	    int scale = (bits[3] >> 16) & 0xff;
		decimal work = value;
		StringBuilder ret = new StringBuilder();

		//  Scale the decimal
		for (int i = 0; i<scale; i++) work *= 10.0m;
	   
		//  Pick off one digit at a time
		while (work > 0.0m) 
		{
			ret.Insert(0, decimalDigits[
					(int)(work - Decimal.Truncate(work*0.1m)*10.0m)]);
			work = Decimal.Truncate(work * 0.1m);
		}

		//  Pad out significant digits
		if (ret.Length < scale) 
		{
			ret.Insert(0, "0", scale - ret.Length);
		}

		//  Insert a decimal point
		ret.Insert(ret.Length - scale, '.');

		return ret.ToString();
	}
	
	internal static int GetExponent( double value, int precision ) {
		// return (int)Math.Floor(Math.Log10(Math.Abs(value)));
		/*
			Note:
			if value is a value like 99.9999999999999999999999999999998
			Math.Log10(value) would return 2.0
			but it should return 1.0
			so the exponent would be one to big.
			So the Method below is now used to calculate the exponent.
		*/

		if( value == 0.0 ) return 0;
	
		int exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));

		if( exponent >= -4 && exponent < 15 ) {
	
			double work = Math.Abs(value);
			if( precision >=0 ) work += 5*Math.Pow(10, -precision-1 );
		
			if( work < ULongMaxValue ) {
				ulong  value1 = (ulong)work;
				exponent = ULog10( value1 );
		
				work -= value1;
				work *= Math.Pow(10, 15 - exponent -1 );
		
				ulong  value2 = (ulong) (work+.5);
		
				if( value2 != 0 ) {
					ulong  test   = (ulong) work;
					// check for rounding error
					// if floor Log10(test) is not equal then floor Log10(value2) then value2 is factor 10 too big
					// so increment value1 by 1 and set value2 to 0 -> return new value1
					if( (test != value2) && ( ULog10(test) != ULog10(value2) ) ) {
						exponent++;
					}
		
					if( exponent < 0 ) {
						exponent = ULog10( value2 ) - 15;
					}
				}
			}
		}
		return exponent;
	}

	internal static int GetExponent( double value ) {
		return GetExponent( value, -1 );
	}
	
	static protected string FormatFloat(double value, int precision)
	{
		if (value == 0.0) return ".";

		// Rounding code
		double r = 5 * Math.Pow(10, -precision - 1);
				
		if( value < 0 ) value -= r;
		else            value += r;
		
		//  
		//int exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
		int exponent = Formatter.GetExponent( value );
		double work = value * Math.Pow(10, 16 - exponent);
		
		//
		//  Build a numeric representation, sans decimal point.
		//
		StringBuilder sb = 
			new StringBuilder(FormatInteger((ulong)Math.Floor(work)), 30 );
		sb.Remove(sb.Length-1, 1);    // Ditch the trailing decimal point

		if (sb.Length > 0 && sb.Length > precision + exponent + 1)
		{
			sb.Remove(precision+exponent+1, sb.Length-(precision+exponent+1));
		}	

		//
		//  Cases for reinserting the decimal point.
		//
		if (exponent >= -1 && exponent < sb.Length)
		{
			sb.Insert(exponent+1,'.');
		}
		else if (exponent < -1)
		{
			sb.Insert(0,new String('0',-exponent - 1));
			sb.Insert(0,".");
		}
		else 
		{
			sb.Append(new String('0', exponent - sb.Length + 1));
			sb.Append('.');
		}

		//
		//  Remove trailing zeroes.
		//
		while (sb[sb.Length-1] == '0') {
			sb.Remove(sb.Length-1, 1);
		}

		return sb.ToString();
	}
#endif // CONFIG_EXTENDED_NUMERICS

	static protected string GroupInteger(string value, int[] groupSizes,
											string separator)
	{
		if (value == String.Empty) return "0";

		int vindex = value.Length;
		int i = 0;
		StringBuilder ret = new StringBuilder();

		while (vindex > 0)
		{
			if (vindex - groupSizes[i] <= 0 || groupSizes[i] == 0)
			{
				ret.Insert(0, value.Substring(0, vindex));
				vindex = 0;
			}
			else
			{
				vindex -= groupSizes[i];
				ret.Insert(0, value.Substring(vindex, groupSizes[i]));
				ret.Insert(0, separator);
			}
			if (i < groupSizes.Length-1) ++i;
		}

		return ret.ToString();
	}
	
	// ----------------------------------------------------------------------
	//  Public interface
	//

	//
	//  Factory/Singleton method
	//
	public static Formatter CreateFormatter(String format)
	{
		return CreateFormatter(format, null);
	}

	public static Formatter CreateFormatter(String format, IFormatProvider p)
	{
		int precision;
		Formatter ret;

		if (format == null)
		{
			throw new FormatException(_("Format_StringException"));
		}

		// handle empty format
		if(format.Length == 0)
		{
			ret = new GeneralFormatter(-1, 'G');
			formats[format] = ret;
			return ret;
		}

		//  Search for cached formats
		if (formats[format] != null)
		{
			return (Formatter) formats[format];
		}


		// Validate the format.  
		// It should be of the form 'X', 'X9', or 'X99'.
		// If it's not, return a CustomFormatter.
		if (validformats.IndexOf(format[0]) == -1 || format.Length > 3)
		{
			ret = new CustomFormatter(format);
			formats[format] = ret;
			return ret;
		}

		try 
		{
			precision = (format.Length == 1) ? 
				-1 : Byte.Parse(format.Substring(1));
		}
		catch (FormatException)
		{
			ret = new CustomFormatter(format);
			formats[format] = ret;
			return ret;
		}
		
		switch(format[0])	// There's always a yucky switch somewhere
		{
		case 'C':
		case 'c':
			ret = new CurrencyFormatter(precision);
			break;
		
		case 'D':
		case 'd':
			ret = new DecimalFormatter(precision);
			break;
		
		case 'E':
		case 'e':
			ret = new ScientificFormatter(precision, format[0]);
			break;
		
		case 'F':
		case 'f':
			ret = new FixedPointFormatter(precision);
			break;

		case 'G':
		case 'g':
			ret = new GeneralFormatter(precision, format[0]);
			break;

		case 'N':
		case 'n':
			ret = new System.Private.NumberFormat.NumberFormatter(precision);
			break;

		case 'P':
		case 'p':
			ret = new PercentFormatter(precision);
			break;
		
		case 'R':
		case 'r':
			ret = new RoundTripFormatter(precision);
			break;

		case 'X':
		case 'x':
			ret = new HexadecimalFormatter(precision, format[0]);
			break;

		default:
			ret = new CustomFormatter(format);
			break;
		}
		
		formats[format] = ret;
		return ret;
	}

	//
	//  Public access method.
	//
	public abstract string Format(Object o, IFormatProvider provider);

	static bool UseScientificFormatter( string format, out int precision ) {

		precision = -1;

		if( null != format && format.Length > 1 && format.Length <= 3 && (format[0] == 'E' || format[0] == 'e') ) {
			try 
			{
				precision = Byte.Parse(format.Substring(1));
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}
		return false;
	}

	static bool UseStandardFormatter( string format, out int precision ) {

		precision = -1;

		if ( null == format || format == "G" || format == "g" || format.Length == 0 || format == "G0" ) return true;

		if( format.Length > 1 && format.Length <= 3 && (format[0] == 'F' || format[0] == 'f') ) {
			try 
			{
				precision = Byte.Parse(format.Substring(1));
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}
		return false;
	}

	const string strNull = "0";

	static public string FormatDouble( double value, string format, IFormatProvider provider ) {
		int precision;
		if( UseStandardFormatter(format, out precision ) ) {
			if( value == 0.0 && precision <= 0 ) return strNull;

			int exponent  = GetExponent(value);

			if( precision >= 0 ) {
				return FormatDouble( value, exponent, precision, true, provider );
			}
			else {
				if (exponent >= -4 && exponent < 15) {
					return FormatDouble( value, exponent, 15, false, provider );
				}
				else {
					return FormatScientific(value, exponent, 15, format, provider);
					//return Formatter.CreateFormatter(format == null ? "G" : format).Format(value, provider);
				}
			}
		}
		else if( UseScientificFormatter(format, out precision ) ) {
			return FormatScientific(value, GetExponent(value), precision, format, provider);
		}
		
		return Formatter.CreateFormatter(format == null ? "G" : format).Format(value, provider);
	}

	static public string FormatSingle( float value, string format, IFormatProvider provider ) {
		int precision;
		if( UseStandardFormatter(format, out precision ) ) {
			if( value == 0.0 && precision <= 0 ) return strNull;

			int exponent  = GetExponent(value);
		
			if( precision >= 0 ) {
				return FormatDouble( value, exponent, precision, true, provider );
			}
			else {
				if (exponent >= -4 && exponent < 7) {
					return FormatDouble( value, exponent, 7, false, provider );
				}
				else {
					return FormatScientific(value, exponent, 7, format, provider);
					// return Formatter.CreateFormatter(format == null ? "G" : format).Format(value, provider);
				}
			}
		}
		else if( UseScientificFormatter(format, out precision ) ) {
			return FormatScientific(value, GetExponent(value), precision, format, provider);
		}
		return Formatter.CreateFormatter(format == null ? "G" : format).Format(value, provider);
	}

	static public string FormatInt64( long value, string format, IFormatProvider provider ) {
		int precision;
		if( UseStandardFormatter(format, out precision ) ) {
			NumberFormatInfo nfi = System.Globalization.NumberFormatInfo.GetInstance( provider );
			StringBuilder sb = new StringBuilder( FormatInt64( value, nfi.NegativeSign ) );
			if( precision > 0 ) {
				sb.Append( nfi.NumberDecimalSeparator );
				sb.Append( '0', precision );
			}
			return sb.ToString();
		}
		else if( UseScientificFormatter(format, out precision ) ) {
			return FormatScientific(value, GetExponent(value), precision, format, provider);
		}
		return Formatter.CreateFormatter(format).Format(value, provider);
	}

	static public string FormatUInt64( ulong value, string format, IFormatProvider provider ) {
		int precision;
		if( UseStandardFormatter(format, out precision ) ) {
			if( precision > 0 ) {
				NumberFormatInfo nfi = System.Globalization.NumberFormatInfo.GetInstance( provider );
				StringBuilder sb = new StringBuilder( FormatUInt64( value ) );
				sb.Append( nfi.NumberDecimalSeparator );
				sb.Append( '0', precision );
				return sb.ToString();
			}
			else {
				return FormatUInt64( value );
			}
		}
		else if( UseScientificFormatter(format, out precision ) ) {
			return FormatScientific(value, GetExponent(value), precision, format, provider);
		}
		return Formatter.CreateFormatter(format).Format(value, provider);
	}

	static public string FormatInt32( int value, string format, IFormatProvider provider ) {
		return FormatInt64( value, format, provider );
	}

	static public string FormatUInt32( uint value, string format, IFormatProvider provider ) {
		return FormatUInt64( value, format, provider );
	}

	static public string FormatInt16( short value, string format, IFormatProvider provider ) {
		return FormatInt64( value, format, provider );
	}

	static public string FormatUInt16( ushort value, string format, IFormatProvider provider ) {
		return FormatUInt64( value, format, provider );
	}

	static public string FormatSByte( sbyte value, string format, IFormatProvider provider ) {
		return FormatInt64( value, format, provider );
	}

	static public string FormatByte( byte value, string format, IFormatProvider provider ) {
		return FormatUInt64( value, format, provider );
	}

	// General formatting

	// return the absolut value of Log10
	static int ULog10( ulong val ) {
		if( val == 0 ) return -1;
		return (int) Math.Log10( val );
	}

	static protected string FormatDouble( double value, int exponent, int precision, bool FixedPoint, IFormatProvider provider ) {

		NumberFormatInfo nfi = System.Globalization.NumberFormatInfo.GetInstance( provider );

		bool negativ = value < 0;
		double work  = negativ ? -value : value;
		if( precision < 0 ) precision = 15;

		StringBuilder sb = new StringBuilder(30);

		do {
			if( work < ULongMaxValue ) {

				work += 5*Math.Pow(10,-precision-1 + (FixedPoint ? 0 : exponent+1) );
				ulong value1 = (ulong) work;

				if( negativ && value1 != 0 ) sb.Append( nfi.NegativeSign );
				sb.Append( FormatUInt64( value1 ) );

				if( precision == 0 ) break;

				work -= value1;
				work *= Math.Pow( 10, 15 );
				ulong value2 = (ulong) work;

				if( value2 == 0 && !FixedPoint ) break;

				int iCount = FixedPoint ? precision : precision-exponent-1;
				if( iCount <= 0 ) break;

				if( value2 == 0 ) {
					sb.Append( nfi.NumberDecimalSeparator );
					sb.Append( '0', iCount );
					break;
				}

				int iLen   = ULog10( value2 )+1;
				int iNull  = 15-iLen;
				if( iNull >= iCount ) {
					value2 = 0;
					if( FixedPoint ) {
						sb.Append( nfi.NumberDecimalSeparator );
						sb.Append( '0', iCount );
					}
				}
				else {
					iCount -= iNull;
					if( FixedPoint || iCount > 0 ) {
						sb.Append( nfi.NumberDecimalSeparator );
						if( iNull > 0 ) {
							sb.Append( '0', iNull );
						}
						if( iLen <= iCount ) {
							sb.Append( FormatUInt64( value2 ) );
							iCount -= iLen;
							if( iCount > 0 && FixedPoint ) sb.Append( '0', iCount );
						}
						else {
							sb.Append( FormatUInt64( value2 ).Substring( 0, iCount ) );
						}
						if( !FixedPoint ) {
							while( sb[sb.Length-1]=='0') sb.Remove(sb.Length-1,1);
						}
					}
				}

				if( negativ && value1 == 0 && value2 != 0 ) {
					sb.Insert( 0, nfi.NegativeSign );
				}
			}
			else {
				if( negativ  ) sb.Append( nfi.NegativeSign );
				int pow;
				if( exponent > precision && exponent > 15 ) {
					pow = exponent-15+1;
				}
				else {
					pow = exponent-precision+1;
				}
				work /= Math.Pow(10,pow);
				work += .5;
				sb.Append( FormatUInt64( (ulong)work ) );
				sb.Append( '0', pow );

				if( FixedPoint && precision > 0 ) {
					sb.Append( nfi.NumberDecimalSeparator );
					sb.Append( '0', precision );
				}
			}
		}while(false);

		return sb.ToString();
	}

	static protected string FormatDouble( double value, int precision, bool FixedPoint, IFormatProvider provider ) {
		return FormatDouble( value, GetExponent(value, precision ), precision, FixedPoint, provider );
	}
	
	static protected string FormatScientific(double value, int exponent, int precision, string format, IFormatProvider provider)
	{
		if (Double.IsNaN(value)) {
			return NumberFormatInfo(provider).NaNSymbol;
		}
		else if(Double.IsPositiveInfinity(value)) {
			return NumberFormatInfo(provider).PositiveInfinitySymbol;
		}
		else if(Double.IsNegativeInfinity(value)) {
			return NumberFormatInfo(provider).NegativeInfinitySymbol;
		}
		else if( value == double.Epsilon ) {
			return "4" + NumberFormatInfo(provider).NumberDecimalSeparator + "94065645841247E-324";
		}

		bool fixPrecision = false;
		bool isNegative = false;
		int  iPadLeft   = 2;
		char Ee         = 'E';
		if( null != format && format.Length > 0 ) {
			switch( format[0] ) {
				case 'g' : 
					Ee = 'e';
					iPadLeft = 2;
					break;
				case 'G' : 
					Ee = 'E';
					iPadLeft = 2;
					break;
				case 'e' : 
					Ee = 'e';
					iPadLeft = 3;
					fixPrecision = true;
					break;
				case 'E' : 
					Ee = 'E';
					iPadLeft = 3;
					fixPrecision = true;
					break;
			}
		}

		if (value < 0) {
			isNegative = true;
			value = -value;
		}

		double mantissa = value / Math.Pow(10, exponent);
		
		if (Double.IsNaN(mantissa))
		{
			return NumberFormatInfo(provider).NaNSymbol;
		}
		else if(Double.IsPositiveInfinity(mantissa))
		{
			return NumberFormatInfo(provider).PositiveInfinitySymbol;
		}
		else if(Double.IsNegativeInfinity(mantissa))
		{
			return NumberFormatInfo(provider).NegativeInfinitySymbol;
		}

		StringBuilder ret = new StringBuilder( FormatDouble( mantissa, precision, fixPrecision, provider ) );
		if (isNegative)
		{
			ret.Insert(0, NumberFormatInfo(provider).NegativeSign);
		}

		ret.Append(Ee);
		if (exponent < 0)
		{
			ret.Append(NumberFormatInfo(provider).NegativeSign);
		}
		else
		{
			ret.Append(NumberFormatInfo(provider).PositiveSign);
		}
		ret.Append( FormatUInt64( (ulong) Math.Abs(exponent) ).PadLeft(iPadLeft,'0') );
		return ret.ToString();
	}		
	

	static private string FormatInt64( long value, string sign ) {
		if( value == 0 ) return strNull;
		if( value == long.MinValue ) return "-9223372036854775808";

		StringBuilder ret = new StringBuilder(25);
		long work = value;
		if( work < 0 ) {
			work = -work;
		}
		while (work > 0) {
			ret.Insert(0, decimalDigits[work % 10]);
			work /= 10;
		}
		if( value < 0 ) ret.Insert(0, sign );
		return ret.ToString();
	}

	static private string FormatUInt64( ulong value ) {
		if( value == 0 ) return strNull;

		StringBuilder ret = new StringBuilder(25);
		ulong work = value;
		while (work > 0) {
			ret.Insert(0, decimalDigits[work % 10]);
			work /= 10;
		}
		return ret.ToString();
	}


} // class Formatter

} // namespace System.Private.Format

