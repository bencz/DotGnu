/*
 * SoapQName.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapQName" class.
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

[Serializable]
public sealed class SoapQName : ISoapXsd
{
	// Internal state.
	private String key;
	private String name;
	private String namespaceValue;

	// Constructors.
	public SoapQName() {}
	public SoapQName(String value)
			{
				this.name = value;
			}
	public SoapQName(String key, String name)
			{
				this.key = key;
				this.name = name;
			}
	public SoapQName(String key, String name, String namespaceValue)
			{
				this.key = key;
				this.name = name;
				this.namespaceValue = namespaceValue;
			}

	// Get or set this object's value.
	public String Key
			{
				get
				{
					return key;
				}
				set
				{
					this.key = value;
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					this.name = value;
				}
			}
	public String Namespace
			{
				get
				{
					return namespaceValue;
				}
				set
				{
					this.namespaceValue = value;
				}
			}

	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "SoapQName";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Parse a value into an instance of this class.
	public static SoapQName Parse(String value)
			{
				if(value == null)
				{
					return new SoapQName();
				}
				else
				{
					int index = value.IndexOf(':');
					if(index != -1)
					{
						return new SoapQName
							(value.Substring(0, index),
							 value.Substring(index + 1));
					}
					else
					{
						return new SoapQName(String.Empty, value);
					}
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				if(key == null || key == String.Empty)
				{
					return name;
				}
				else
				{
					return key + ":" + name;
				}
			}

}; // class SoapQName

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
