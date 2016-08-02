/*
 * ScientificFormatter.cs - Implementation of the
 *          "System.Private.NumberFormat.ScientificFormatter" class.
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
using System.Globalization;
using System.Text;

//
//  Scientific Formatter - "Exx" or "exx"
internal class ScientificFormatter : Formatter
{
	private char Ee;

	public ScientificFormatter(int precision, char Ee)
	{
		this.precision = precision == -1 ? 6 : precision;
		this.Ee = Ee;
	}

	public override string Format(Object o, IFormatProvider provider)
	{
#if CONFIG_EXTENDED_NUMERICS
		double value = OToDouble(o);
		return Formatter.FormatScientific( value, Formatter.GetExponent( value ), precision, Ee.ToString(), provider);
		/*

		bool isNegative = false;

		double value;

		double mantissa;
		int exponent;

		string rawnumber;
		StringBuilder ret;

		value = OToDouble(o);
		if (Double.IsNaN(value))
		{
			return NumberFormatInfo(provider).NaNSymbol;
		}
		else if(Double.IsPositiveInfinity(value))
		{
			return NumberFormatInfo(provider).PositiveInfinitySymbol;
		}
		else if(Double.IsNegativeInfinity(value))
		{
			return NumberFormatInfo(provider).NegativeInfinitySymbol;
		}
		else if( value == double.Epsilon ) {
		 return "4" + NumberFormatInfo(provider).NumberDecimalSeparator + "94065645841247E-324";
		}

		
		if (value < 0)
		{
			isNegative = true;
			value = -value;
		}

		//  Calculate exponent and mantissa
		if (value == 0.0d)
		{
			exponent = 0;
			mantissa = 0.0d;
		}
		else
		{
			//exponent = (int) Math.Floor(Math.Log10(value));
			exponent = Formatter.GetExponent( value );
			mantissa = value / Math.Pow(10, exponent);
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
		}

		rawnumber = FormatFloat(mantissa, precision);
		if (rawnumber.Length == 0) rawnumber = "0";

		ret = new StringBuilder( 
				rawnumber.Substring(0, rawnumber.IndexOf('.')));

		if (precision > 0)
		{
			ret.Append(NumberFormatInfo(provider).NumberDecimalSeparator)
				.Append(rawnumber.Substring(rawnumber.IndexOf('.')+1));
			if (rawnumber.EndsWith("."))
			{
				ret.Append("0");
			}
		}

		if (isNegative)
		{
			ret.Insert(0, NumberFormatInfo(provider).NegativeSign);
		}

		ret.Append(this.Ee);
		if (exponent < 0)
		{
			ret.Append(NumberFormatInfo(provider).NegativeSign);
		}
		else
		{
			ret.Append(NumberFormatInfo(provider).PositiveSign);
		}
		rawnumber = FormatInteger((ulong)Math.Abs(exponent));
		ret.Append(rawnumber.Substring(0, rawnumber.IndexOf('.'))
													.PadLeft(3,'0'));

		return ret.ToString();
		*/
#else // !CONFIG_EXTENDED_NUMERICS
		return String.Empty;
#endif // !CONFIG_EXTENDED_NUMERICS
	}		
} // class ScientificFormatter

} // namespace System.Private.NumberFormat

