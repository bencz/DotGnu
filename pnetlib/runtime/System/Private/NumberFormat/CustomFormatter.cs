/*
 * CustomFormatter.cs - Implementation of the
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
using System.Collections;
using System.Globalization;
using System.Text;

internal class CustomFormatter : Formatter
{
	//
	//  Constants and read-only
	//
	private const char zeroPlaceholder = '0';
	private const char digitPlaceholder = '#';
	private const char decimalSeparator = '.';
	private const char groupSeparator = ',';
	private const char percentagePlaceholder = '%';

	private static readonly char[] engineeringFormat = {'E','e'};
	private static readonly char[] sectionSeparator = {';'};

	//
	//  Stored format information 
	//
	private string originalFormat;

	private string positiveFormat;
	private string negativeFormat;
	private string zeroFormat;
	bool includeNegSign;           //  Should a '-' be included?
	
	//
	//  Constructor - Take custom format string as input.
	//
	public CustomFormatter(string inFormat)
	{
		string[] fmts;

		this.originalFormat = inFormat;
		fmts = originalFormat.Split(sectionSeparator, 3);

		if (fmts.Length == 0)
		{
			this.positiveFormat = null;
			this.negativeFormat = null;
			this.zeroFormat = null;
			this.includeNegSign = true;
		} 
		else if (fmts.Length == 1)
		{
			this.positiveFormat = inFormat;
			this.negativeFormat = inFormat;
			this.zeroFormat = inFormat;
			this.includeNegSign = true;
		}
		else if (fmts.Length == 2)
		{
			this.positiveFormat = fmts[0];
			this.negativeFormat = fmts[1];
			this.zeroFormat = fmts[0];
			this.includeNegSign = false;
		}
		else
		{
			this.positiveFormat = fmts[0];
			if (this.negativeFormat == String.Empty)
			{
				this.negativeFormat = fmts[0];
				this.includeNegSign=true;
			}
			else
			{
				this.negativeFormat = fmts[1];
				this.includeNegSign=false;
			}
			this.zeroFormat = fmts[2];
		}
	}

	private int ScientificStart(string format)
	{
		int ret, pos;

		pos = format.IndexOfAny(engineeringFormat, 0);

		while (pos != -1)
		{
			ret = pos++;
			if (format[pos] == '+' || format[pos] == '-') 
			{
				pos++;
			}
			if (format[pos] == '0') 
			{
				return ret;
			}
			pos = format.IndexOfAny(engineeringFormat, pos);
		} 

		return -1;
	}

	private string FormatFromSign(int sign)
	{
		string ret = null;

		switch(sign)
		{
		case 1:
			ret = positiveFormat;
			break;
		case -1:
			ret = negativeFormat;
			break;
		case 0:
			ret = zeroFormat;
			break;
		}
		return ret;
	}

	//
	//  FormatRawNumber - Given a string of form 'n*.n*', format it based
	//  upon the custom format.
	//
	private string FormatRawNumber(string value, string format, int sign,
									               IFormatProvider provider)
	{
		if( value.StartsWith( "." ) ) value = "0" + value;

		string rawnumber;
		int firstzero, lastzero;
		int scale = 0;

		//  Locate the decimal point
		int decimalPos = format.IndexOf(decimalSeparator);
		if (decimalPos == -1) decimalPos = format.Length;

		//  Scaling
		int formatindex = decimalPos - 1;
		while (formatindex >=0 && format[formatindex] == groupSeparator)
		{
			scale += 3;
			--formatindex;
		}

		// Move the decimal point
		StringBuilder temp = new StringBuilder();
		if (scale <= value.IndexOf('.'))
		{
			temp.Append(value.Substring(0, value.IndexOf('.') - scale))
					.Append(".")
					.Append(value.Substring(value.IndexOf('.') - scale, scale));
		}					
		else
		{
			temp.Append(".")
				.Append(value
						.Substring(0, value.IndexOf('.'))
						.PadLeft(scale, '0'));
		}
		temp.Append(value.Substring(value.IndexOf('.') + 1));

		if (includeNegSign && sign == -1)
		{
			temp.Insert(0, NumberFormatInfo(provider).NegativeSign);
		}
		rawnumber = temp.ToString();

		//  Formatting
		int rawindex = rawnumber.IndexOf('.') - 1;
		StringBuilder ret = new StringBuilder();

		while (rawindex >= 0 && formatindex >= 0) {
			if (format[formatindex] == digitPlaceholder)
			{
				ret.Insert(0, rawnumber[rawindex--]);
			}
			else if (format[formatindex] == zeroPlaceholder )
			{
				// Don't put a negative if a zero should go there.
				if ( rawnumber[rawindex] < '0' || rawnumber[rawindex] > '9')
				{
					int i = formatindex + 1;
					while (format[i] == ',') ++i;

					if (format[i] == '.') 
					{
						ret.Insert(0,'0');
					}
					else
					{
						ret.Insert(0, rawnumber[rawindex--]);
					}
				}
				else
				{
					ret.Insert(0, rawnumber[rawindex--]);
				}
			}
			else if (format[formatindex] == groupSeparator)
			{
				ret.Insert(0, NumberFormatInfo(provider).NumberGroupSeparator);
			}
			else
			{
				ret.Insert(0, format[formatindex]);
			}
			--formatindex;
		}

		if (rawindex >= 0)
		{
			ret.Insert(0, rawnumber.Substring(0, rawindex+1));
		}

		//  Zero pad
		firstzero = format.IndexOf(zeroPlaceholder);
		if (firstzero != -1)
		{
			while (firstzero <= formatindex)
			{
				if (format[formatindex] == digitPlaceholder
						|| format[formatindex] == zeroPlaceholder)
				{
					ret.Insert(0, '0');
				}
				else if (format[formatindex] == groupSeparator)
				{
					ret.Insert(0,
						NumberFormatInfo(provider).NumberGroupSeparator);
				}
				else
				{
					ret.Insert(0, format[formatindex]);
				}
				--formatindex;
			}
		}

		//  Fill out literal portion
		while (formatindex >= 0)
		{
			if( !( format[formatindex] == digitPlaceholder
	                || format[formatindex] == zeroPlaceholder
					|| format[formatindex] == groupSeparator ))
			{
				ret.Insert(0, format[formatindex]);
			}
			--formatindex;
		}

		//  Decimal point dealings
		rawindex = rawnumber.IndexOf('.') + 1;
		formatindex = decimalPos + 1;

		if(decimalPos < format.Length)
		{
			ret.Append(NumberFormatInfo(provider).NumberDecimalSeparator);
		}

		while (rawindex < rawnumber.Length && formatindex < format.Length) {
			if (format[formatindex] == digitPlaceholder 
					|| format[formatindex] == zeroPlaceholder )
			{
				ret.Append(rawnumber[rawindex++]);
			}
			else
			{
				ret.Append(format[formatindex]);
			}
			++formatindex;
		}

		// More zero padding
		lastzero = format.LastIndexOf(zeroPlaceholder);

		while (lastzero >= formatindex)
		{
			if (format[formatindex] == digitPlaceholder
					|| format[formatindex] == zeroPlaceholder)
			{
				ret.Append('0');
			}
			else
			{
				ret.Append(format[formatindex]);
			}
			formatindex++;
		}

		while (formatindex < format.Length)
		{
			if ( !( format[formatindex] == digitPlaceholder
						|| format[formatindex] == zeroPlaceholder ))
			{
				ret.Append(format[formatindex]);
			}
			++formatindex;
		}

		return ret.ToString();
	}

#if CONFIG_EXTENDED_NUMERICS
	private string FormatScientific(double d, string format, int sign,
													IFormatProvider provider)
	{
		//int exponent = (int)Math.Floor(Math.Log10(d));
		int exponent = Formatter.GetExponent( d );
		double mantissa = d/Math.Pow(10,exponent);

		int i = ScientificStart(format);

		//  Format the mantissa
		StringBuilder ret = new StringBuilder(
				FormatFloat(mantissa, format.Substring(0, i), sign, provider))
			.Append(format[i++]);

		//  Sign logic
		if (format[i] == '+' && exponent >= 0)
		{
			ret.Append(NumberFormatInfo(provider).PositiveSign);
		}
		else if (exponent < 0)
		{
			ret.Append(NumberFormatInfo(provider).NegativeSign);
		}

		if (format[i] == '+' || format[i] == '-')
		{
			++i;
		}

		// Place exponent value into string.
		int exponentMin = 0;
		while (i<format.Length && format[i] == '0')
		{
			++i; 
			++exponentMin;
		}

		//  Format the exponent and append, 
		string exponentString = 
			Formatter.FormatInteger((ulong)Math.Abs(exponent));
		ret.Append(exponentString.Substring(0, exponentString.Length-1)
												.PadLeft(exponentMin,'0'));
		
		// Fill out the string
		ret.Append(format.Substring(i));

		return ret.ToString();
	}
#endif // CONFIG_EXTENDED_NUMERICS

	private string FormatInteger(ulong value, string format, int sign,
												IFormatProvider provider)
	{
		return FormatRawNumber(Formatter.FormatInteger(value)
												,format, sign, provider);
	}


#if CONFIG_EXTENDED_NUMERICS
	private string FormatFloat(double d, string format, int sign,
												IFormatProvider provider)
	{
		int formatindex, precision;

		formatindex = format.IndexOf('.');
		if (formatindex == -1) {
			precision = 0;
		}
		else
		{
			for (formatindex++, precision = 0; 
					formatindex < format.Length; formatindex++)
			{
				if (format[formatindex] == digitPlaceholder ||
						format[formatindex] == zeroPlaceholder)
				{
					++precision;
				}
			}
		}

		return FormatRawNumber( Formatter.FormatFloat(d, precision + 2), 
										format, sign, provider);
	}

	private string FormatDecimal(decimal value, string format, int sign,
												IFormatProvider provider)
	{
		if (format.IndexOf(percentagePlaceholder) != -1)
		{
			return FormatRawNumber(Formatter.FormatDecimal(value*100),
					format, sign, provider);
		}
		else
		{
			return FormatRawNumber(Formatter.FormatDecimal(value),
					format, sign, provider);
		}
	}
#endif // CONFIG_EXTENDED_NUMERICS

	//
	//  Public access method.
	//
	public override string Format(Object o, IFormatProvider provider)
	{
		if (IsSignedInt(o))
		{
			long val = OToLong(o);
			int sign;
			if(val < 0)
			{
				sign = -1;
				val = -val;
			}
			else if(val > 0)
			{
				sign = 1;
			}
			else
			{
				sign = 0;
			}
			string format = FormatFromSign(sign);
		#if CONFIG_EXTENDED_NUMERICS
			if (ScientificStart(format) == -1)
			{
				return FormatInteger((ulong)val, format, sign, provider);
			}
			else
			{
				return FormatScientific((double)val, format, sign, provider);
			}
		#else
			return FormatInteger((ulong)val, format, sign, provider);
		#endif
		}
		else if (IsUnsignedInt(o))
		{
			ulong val = OToUlong(o);
			string format = ( (val==0) ? zeroFormat : positiveFormat);

		#if CONFIG_EXTENDED_NUMERICS
			if (ScientificStart(format) == -1)
			{
				return FormatInteger(val, format, val == 0 ? 0 : 1, provider);
			}
			else
			{
				return FormatScientific( (double)val,
						format, val == 0 ? 0 : 1, provider);
			}
		#else
			return FormatInteger(val, format, val == 0 ? 0 : 1, provider);
		#endif
		}
#if CONFIG_EXTENDED_NUMERICS
		else if (IsFloat(o))
		{
			double val = OToDouble(o);
			string format = FormatFromSign(Math.Sign(val));
			if (ScientificStart(format) == -1)
			{
				return FormatFloat(Math.Abs(val),
						format, Math.Sign(val), provider);
			}
			else
			{
				return FormatScientific(Math.Abs(val),
						format, Math.Sign(val), provider);
			}
		}
		else if (IsDecimal(o))
		{
			decimal val = (decimal) o;
			string format = FormatFromSign(Math.Sign(val));
			if (ScientificStart(format) == -1)
			{
				return FormatDecimal(Math.Abs(val),
						format, Math.Sign(val), provider);
			}
			else
			{
				return FormatScientific((double)Math.Abs(val),
						format, Math.Sign(val), provider);
			}
		}
#endif // CONFIG_EXTENDED_NUMERICS
		else if (o is IFormattable)
		{
			return ((IFormattable)o).ToString(originalFormat, provider);
		}
		else
		{
			return o.ToString();
		}
	}

} // class CustomFormatter

} // namespace System.Private.NumberFormat

