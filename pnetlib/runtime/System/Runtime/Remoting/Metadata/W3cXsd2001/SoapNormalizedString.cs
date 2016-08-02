/*
 * SoapNormalizedString.cs - Implementation of the
 *	"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapNormalizedString" class.
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
public sealed class SoapNormalizedString : ISoapXsd
{
	// Internal state.
	private String value;

	// Constructors.
	public SoapNormalizedString() {}
	public SoapNormalizedString(String value)
			{
				this.Value = value;
			}

	// Get or set this object's value.
	public String Value
			{
				get
				{
					return value;
				}
				set
				{
					if(value != null)
					{
						if(value.IndexOfAny(new char [] {'\n', '\r', '\t'})
								!= -1)
						{
							throw new RemotingException
								(_("Arg_InvalidSoapValue"));
						}
					}
					this.value = value;
				}
			}

	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "normalizedString";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Parse a value into an instance of this class.
	public static SoapNormalizedString Parse(String value)
			{
				return new SoapNormalizedString(value);
			}

	// Escape problematic characters in a string.
	internal static String Escape(String str)
			{
				if(str == null)
				{
					return String.Empty;
				}
				StringBuilder builder = new StringBuilder();
				foreach(char ch in str)
				{
					switch(ch)
					{
						case '<':
						case '>':
						case '&':
						case '\'':
						case '"':
						case '\0':
						{
							builder.Append('&');
							builder.Append('#');
							builder.Append(((int)ch).ToString());
							builder.Append(';');
						}
						break;

						default:
						{
							builder.Append(ch);
						}
						break;
					}
				}
				return builder.ToString();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return Escape(value);
			}

}; // class SoapNormalizedString

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
