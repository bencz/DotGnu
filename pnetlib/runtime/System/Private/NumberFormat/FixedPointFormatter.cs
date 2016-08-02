/*
 * FixedPointFormatter.cs - Implementation of the
 *          "System.Private.NumberFormat.FixedPointFormatter" class.
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

internal class FixedPointFormatter : Formatter
{
	public FixedPointFormatter(int precision)
	{
		this.precision = precision;
	}

	internal static string Format(Object o, int precision, IFormatProvider provider)
	{
#if CONFIG_EXTENDED_NUMERICS
		if( !IsDecimal(o) )
			return Formatter.FormatDouble( OToDouble(o), precision, true, provider );
#endif
		//  Get string
		string rawnumber = FormatAnyRound(o, precision, provider);
		StringBuilder ret = new StringBuilder();

		if (rawnumber[0] == '-')
		{
			ret.Append(NumberFormatInfo(provider).NegativeSign);
			rawnumber = rawnumber.Substring(1);
		}

		if (rawnumber[0] == '.')
		{
			ret.Append('0');
		}
		else
		{
			ret.Append(rawnumber.Substring(0, rawnumber.IndexOf('.')));
		}

		if (precision > 0)
		{
			ret.Append(NumberFormatInfo(provider).NumberDecimalSeparator)
				.Append(rawnumber
						.PadRight(rawnumber.IndexOf('.')+1+precision, '0') 
						.Substring(rawnumber.IndexOf('.')+1, precision));
		}

		return ret.ToString();
	}		


	public override string Format(Object o, IFormatProvider provider)
	{
		//  Calculate precision
		int precision = (this.precision == -1) ?
			NumberFormatInfo(provider).NumberDecimalDigits : this.precision;

		return Format( o, precision, provider );
	}		
} // class FixedPointFormatter

} // namespace System.Private.NumberFormat

