/*
 * CurrencyFormatter.cs - Implementation of the
 *          "System.Private.Formatter" class.
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
//  Currency Formatter - "Cxx" - Valid for only integral types.
internal class CurrencyFormatter : Formatter
{
	private static String FormatPrefixAndSuffix(String value, bool isneg,
												IFormatProvider provider)
	{
		NumberFormatInfo nfi = NumberFormatInfo(provider);
		string ret = null;

		if(isneg)
		{
			switch(nfi.CurrencyNegativePattern)
			{
				case 0:
					ret = "(" + nfi.CurrencySymbol + value + ")";
					break;

				case 1:
					ret = nfi.NegativeSign + nfi.CurrencySymbol + value;
					break;

				case 2:
					ret = nfi.CurrencySymbol + nfi.NegativeSign + value;
					break;

				case 3:
					ret = nfi.CurrencySymbol + value + nfi.NegativeSign;
					break;

				case 4:
					ret = "(" + value + nfi.CurrencySymbol + ")";
					break;

				case 5:
					ret = nfi.NegativeSign + value + nfi.CurrencySymbol;
					break;

				case 6:
					ret = value + nfi.NegativeSign;
					break;

				case 7:
					ret = value + nfi.CurrencySymbol + nfi.NegativeSign;
					break;

				case 8:
					ret = nfi.NegativeSign + value + " " + nfi.CurrencySymbol;
					break;

				case 9:
					ret = nfi.NegativeSign + nfi.CurrencySymbol + " " + value;
					break;

				case 10:
					ret = value + " " + nfi.CurrencySymbol + nfi.NegativeSign;
					break;

				case 11:
					ret = nfi.CurrencySymbol + " " + value + nfi.NegativeSign;
					break;

				case 12:
					ret = nfi.CurrencySymbol + " " + nfi.NegativeSign + value;
					break;

				case 13:
					ret = value + nfi.NegativeSign + " " + nfi.CurrencySymbol;
					break;

				case 14:
					ret = "(" + nfi.CurrencySymbol + " " + value + ")";
					break;

				case 15:
					ret = "(" + value + " " + nfi.CurrencySymbol + ")";
					break;
			}
		}
		else
		{
			switch(nfi.CurrencyPositivePattern)
			{
				case 0:
					ret = nfi.CurrencySymbol + value;
					break;

				case 1:
					ret = value + nfi.CurrencySymbol;
					break;

				case 2:
					ret = nfi.CurrencySymbol + " " + value;
					break;

				case 3:
					ret = value + " " + nfi.CurrencySymbol;
					break;
			}
		}
		return ret;
	}

	public CurrencyFormatter(int precision)
	{
		this.precision = precision;
	}

	public override string Format(Object o, IFormatProvider provider)
	{
		//
		//  Create working string
		//
		int precision = (this.precision == -1) ?
			NumberFormatInfo(provider).CurrencyDecimalDigits : this.precision;
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
					NumberFormatInfo(provider).CurrencyGroupSizes,
					NumberFormatInfo(provider).CurrencyGroupSeparator));

		//
		//  Add the post-decimal portion.
		//
		if (precision > 0)
		{
			ret.Append(NumberFormatInfo(provider).CurrencyDecimalSeparator);
			ret.Append(
					rawnumber.PadRight(
						rawnumber.IndexOf('.')+1 + precision, '0')
					.Substring(rawnumber.IndexOf('.') + 1, precision));
		}

		//
		//  Final formatting
		//
		return FormatPrefixAndSuffix(ret.ToString(), isNegative, provider);
	}		
} // class CurrencyFormatter

} // namespace System.Private.Format

