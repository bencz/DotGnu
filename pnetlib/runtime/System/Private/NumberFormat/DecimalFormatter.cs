
/*
 * DecimalFormatter.cs - Implementation of the
 *          "System.Private.NumberFormat.DecimalFormatter" class.
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
//  Decimal Formatter - "Dxx" - Valid for only integral types.
internal class DecimalFormatter : Formatter
{
	public DecimalFormatter(int precision)
	{
		this.precision = (precision == -1) ? 1 : precision;
	}

	public override string Format(Object o, IFormatProvider provider)
	{
		bool isNegative = false;
		ulong value;
		string rawnumber;
		StringBuilder buf;
		bool willOverflow=false;
		// willOverflow is set when negation overflows ie 
		// for Int64.MinValue

		//  Type validation
		if (IsSignedInt(o) && OToLong(o) < 0)
		{
			isNegative = true;
			value = (ulong) -OToLong(o);
			if(value==0)
			{
				// We somehow overflowed the data
				// this is definitely Int64.MinValue
				willOverflow=true;
			}
		}
		else if (IsSignedInt(o) || IsUnsignedInt(o))
		{
			isNegative = false;
			value = OToUlong(o);
		}
		else
		{
			//  This is a bad place to be.
			throw new FormatException(_("Format_TypeException"));	
		}

		if(!willOverflow)
		{
			//  Type conversion
			rawnumber=(FormatInteger(value));
		}
		else
		{
			rawnumber="9223372036854775808.";
		}
		
		buf = new StringBuilder(rawnumber.Substring(0, rawnumber.Length-1));

		//  Padding
		if (buf.Length < precision)
		{
			buf.Insert(0, "0", precision - buf.Length);
		}

		//  Negative sign
		if (isNegative)
		{
			buf.Insert(0, NumberFormatInfo(provider).NegativeSign);
		}

		return buf.ToString();
	}		
} // class DecimalFormatter

} // namespace System.Private.NumberFormat

