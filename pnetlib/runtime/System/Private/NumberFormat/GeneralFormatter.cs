/*
 * GeneralFormatter.cs - Implementation of the
 *          "System.Private.NumberFormat.GeneralFormatter" class.
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
using System.Text;

//
//  General Formatter 
//
internal class GeneralFormatter : Formatter
{
	char Ee;
	char Gg;

	public GeneralFormatter(int precision, char Gg)
	{
		this.precision = precision;
		this.Gg = Gg;
		switch(Gg)
		{
			case 'G':
				Ee = 'E';
				break;
			case 'g':
				Ee = 'e';
				break;
			default:
				throw new FormatException(_("Format_StringException"));
		}
	}

	private static int PrecisionOf(Object o) 
	{
		if (o is Int16 || o is UInt16) return 5;
		if (o is Int32 || o is UInt32) return 10;
		if (o is Int64 || o is UInt64) return 19;
		if (o is Byte || o is SByte) return 3;
#if CONFIG_EXTENDED_NUMERICS
		if (o is Single) return 7;
		if (o is Double) return 15;
		if (o is Decimal) return 29;
#endif
		throw new FormatException(_("Format_TypeException"));
	}

	private string StripTrail(string s, IFormatProvider provider)
	{
		String ds = NumberFormatInfo(provider).NumberDecimalSeparator;

		// Strip unnecessary trailing zeroes and point
		if ( s.IndexOf(ds) >= 0)
		{
			StringBuilder sb = new StringBuilder(s);
			while (sb[sb.Length - 1] == '0') sb.Remove(sb.Length-1,1);
			if (sb.ToString().IndexOf(ds) == sb.Length - ds.Length)
			{
				sb.Remove(sb.Length-ds.Length, ds.Length);
			}
			return sb.ToString();
		}
		return s;
	}

	public override string Format(Object o, IFormatProvider provider)
	{
		int precision, exponent;

		if (this.precision == -1)
		{
			precision = PrecisionOf(o);
		}
		else
		{
			precision = this.precision;
		}

#if CONFIG_EXTENDED_NUMERICS
		double val = OToDouble(o);
		if ( val == 0.0) 
		{
			exponent = 0;
		}
		else
		{
			//exponent = (int) Math.Floor(Math.Log10(Math.Abs(OToDouble(o))));
			exponent = Formatter.GetExponent( val );
		}
#else
		// Determine the exponent without using floating-point.
		if (IsSignedInt(o))
		{
			long lvalue = OToLong(o);
			if (lvalue < 0)
			{
				lvalue = -lvalue;
			}
			exponent = 0;
			while(lvalue >= 10)
			{
				++exponent;
				lvalue /= 10;
			}
		}
		else if (IsUnsignedInt(o))
		{
			ulong ulvalue = OToUlong(o);
			exponent = 0;
			while(ulvalue >= 10)
			{
				++exponent;
				ulvalue /= 10;
			}
		}
		else
		{
			exponent = 0;
		}
#endif

		if (IsSignedInt(o) || IsUnsignedInt(o))
		{
			if (exponent < precision)
			{
				return StripTrail(
							FixedPointFormatter.Format(o, 0, provider),
							provider);

			}
			else
			{
				return 
					new ScientificFormatter(-1, Ee).Format(o, provider);
			}
		}

		if (exponent >= -4 && exponent < precision)
		{
#if CONFIG_EXTENDED_NUMERICS
			if( !IsDecimal(o) )
				return Formatter.FormatDouble( val, exponent, precision, false, provider );
#endif
			return StripTrail(
					FixedPointFormatter.Format(o, precision, provider),
					provider);
		}
		else
		{
#if CONFIG_EXTENDED_NUMERICS
			return Formatter.FormatScientific( val, exponent, precision, this.Gg.ToString(), provider);
#else
			return new ScientificFormatter(precision, Ee).Format(o, provider);
#endif
		}
	}		

} // class GeneralFormatter

} // namespace System.Private.NumberFormat

