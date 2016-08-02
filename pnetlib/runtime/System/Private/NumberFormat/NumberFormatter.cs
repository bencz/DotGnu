/*
 * NumberFormatter.cs - Implementation of the
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
using System.Globalization;
using System.Text;

//
//  Number Formatter
internal class NumberFormatter : Formatter
{
	private static String FormatPrefixAndSuffix(String value, bool isneg,
												IFormatProvider provider)
	{
		NumberFormatInfo nfi = NumberFormatInfo(provider);
		string ret;

		if(isneg)
		{
			switch(nfi.NumberNegativePattern)
			{
				case 0:
					ret = "(" + value + ")";
					break;

				case 1:
				default:
					ret = nfi.NegativeSign + value;
					break;

				case 2:
					ret = nfi.NegativeSign + " " + value;
					break;

				case 3:
					ret = value + nfi.NegativeSign;
					break;

				case 4:
					ret = value + " " + nfi.NegativeSign;
					break;
			}
		}
		else
		{
			ret =  value;
		}
		return ret;
	}

	public NumberFormatter(int precision)
	{
		this.precision = precision;
	}

	public override string Format(Object o, IFormatProvider provider)
	{
		//
		//  Create working string
		//
		int precision = (this.precision == -1) ?
			NumberFormatInfo(provider).NumberDecimalDigits : this.precision;
		string rawnumber = FormatAnyRound(o, precision, provider);

		//
		//  Test for negative numbers
		//
		bool isNegative = false;
		if (rawnumber[0] == '-')
		{
			isNegative = true;
			rawnumber = rawnumber.Substring(1);
		}

		// 
		//  Build out the integral portion of the number
		//
		StringBuilder ret = new StringBuilder(
				GroupInteger(rawnumber.Substring(0,rawnumber.IndexOf('.')),
					NumberFormatInfo(provider).NumberGroupSizes,
					NumberFormatInfo(provider).NumberGroupSeparator));

		//
		//  Add the post-decimal portion.
		//
		if (precision > 0)
		{
			ret.Append(NumberFormatInfo(provider).NumberDecimalSeparator)
				.Append(rawnumber
					.PadRight(rawnumber.IndexOf('.')+1+precision,'0')
					.Substring(rawnumber.IndexOf('.')+1, precision));
		}

		//
		//  Final formatting
		//
		return FormatPrefixAndSuffix(ret.ToString(), isNegative, provider);
	}		
} // class NumberFormatter

} // namespace System.Private.Format

