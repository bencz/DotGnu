/*
 * SoapHexBinary.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary" class.
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
public sealed class SoapHexBinary : ISoapXsd
{
	// Internal state.
	private byte[] value;

	// Constructors.
	public SoapHexBinary() {}
	public SoapHexBinary(byte[] value)
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
					return "hexBinary";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Parse a value into an instance of this class.
	public static SoapHexBinary Parse(String value)
			{
				if(value == null)
				{
					return new SoapHexBinary(new byte [0]);
				}
				byte[] buf = new byte [value.Length];
				int posn = 0;
				int temp = -1;
				int digit;
				foreach(char ch in value)
				{
					if(ch >= '0' && ch <= '9')
					{
						digit = (int)(ch - '0');
					}
					else if(ch >= 'A' && ch <= 'F')
					{
						digit = (int)(ch - 'A' + 10);
					}
					else if(ch >= 'a' && ch <= 'f')
					{
						digit = (int)(ch - 'a' + 10);
					}
					else
					{
						throw new RemotingException(_("Arg_InvalidSoapValue"));
					}
					if(temp == -1)
					{
						temp = digit;
					}
					else
					{
						buf[posn++] = (byte)((temp << 4) + digit);
						temp = -1;
					}
				}
				if(temp != -1)
				{
					throw new RemotingException(_("Arg_InvalidSoapValue"));
				}
				byte[] result = new byte [posn];
				Array.Copy(buf, 0, result, 0, posn);
				return new SoapHexBinary(result);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(value == null)
				{
					return String.Empty;
				}
				StringBuilder builder = new StringBuilder(value.Length * 2);
				String hexchars = "0123456789ABCDEF";
				foreach(char ch in value)
				{
					builder.Append(hexchars[(ch >> 4) & 0x0F]);
					builder.Append(hexchars[ch & 0x0F]);
				}
				return builder.ToString();
			}

}; // class SoapHexBinary

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
