/*
 * SoapNegativeInteger.cs - Implementation of the
 *	"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapNegativeInteger" class.
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
public sealed class SoapNegativeInteger : ISoapXsd
{
	// Internal state.
	private Decimal value;

	// Constructors.
	public SoapNegativeInteger() {}
	public SoapNegativeInteger(Decimal value)
			{
				Value = value;
			}

	// Get or set this object's value.
	public Decimal Value
			{
				get
				{
					return value;
				}
				set
				{
					this.value = Decimal.Truncate(value);
					if(value > Decimal.MinusOne)
					{
						throw new RemotingException
							(_("ArgRange_Negative"));
					}
				}
			}

	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "negativeInteger";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Parse a value into an instance of this class.
	public static SoapNegativeInteger Parse(String value)
			{
				return new SoapNegativeInteger
					(Decimal.Parse(value, NumberStyles.Integer,
								   CultureInfo.InvariantCulture));
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return value.ToString(CultureInfo.InvariantCulture);
			}

}; // class SoapNegativeInteger

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
