/*
 * SoapBase64Binary.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapBase64Binary" class.
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

using System.Text;

[Serializable]
public sealed class SoapBase64Binary : ISoapXsd
{
	// Internal state.
	private byte[] value;

	// Constructors.
	public SoapBase64Binary() {}
	public SoapBase64Binary(byte[] value)
			{
				this.value = value;
			}

	// Get or set this object's value.
	public byte[] Value
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

	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "base64Binary";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Parse a value into an instance of this class.
	public static SoapBase64Binary Parse(String value)
			{
				if(value == null)
				{
					return new SoapBase64Binary();
				}
				else
				{
					return new SoapBase64Binary
						(Convert.FromBase64String(value));
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(value == null)
				{
					return null;
				}
				String result = Convert.ToBase64String(value);
				StringBuilder builder = new StringBuilder(result);
				int posn = 79;
				while(posn < builder.Length)
				{
					// Split the value into multiple lines.
					builder.Insert(posn, '\n');
					posn += 80;
				}
				return builder.ToString();
			}

}; // class SoapBase64Binary

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
