/*
 * SoapDate.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapDate" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{

#if CONFIG_SERIALIZATION

using System.Globalization;

[Serializable]
public sealed class SoapDate : ISoapXsd
{
	// Internal state.
	private DateTime value;
	private int sign;

	// Constructors.
	public SoapDate()
			{
				this.value = DateTime.MinValue;
				this.sign = 0;
			}
	public SoapDate(DateTime value)
			{
				this.value = value;
				this.sign = 0;
			}
	public SoapDate(DateTime value, int sign)
			{
				this.value = value;
				this.sign = sign;
			}

	// Get or set this object's value.
	public DateTime Value
			{
				get
				{
					return value;
				}
				set
				{
					this.value = value;
				}
			}

	// Get or set this object's sign.
	public int Sign
			{
				get
				{
					return sign;
				}
				set
				{
					sign = value;
				}
			}

	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "date";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Format values for "Parse".
	private static String[] formats = {"yyyy-MM-dd", "yyyy-MM-ddzzz"};

	// Parse a value into an instance of this class.
	public static SoapDate Parse(String value)
			{
				int sign;
				if(value[0] == '-')
				{
					value = value.Substring(1);
					sign = -1;
				}
				else if(value[0] == '+')
				{
					value = value.Substring(1);
					sign = 0;
				}
				else
				{
					sign = 0;
				}
				DateTime time = DateTime.ParseExact
					(value, formats, CultureInfo.InvariantCulture,
					 DateTimeStyles.None);
				return new SoapDate(time, sign);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(sign >= 0)
				{
					return value.ToString("yyyy-MM-dd",
										  CultureInfo.InvariantCulture);
				}
				else
				{
					return value.ToString("'-'yyyy-MM-dd",
										  CultureInfo.InvariantCulture);
				}
			}

}; // class SoapDate

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
