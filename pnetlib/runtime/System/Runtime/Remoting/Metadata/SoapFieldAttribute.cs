/*
 * SoapFieldAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Metadata.SoapFieldAttribute" class.
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

namespace System.Runtime.Remoting.Metadata
{

#if CONFIG_SERIALIZATION

using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SoapFieldAttribute : SoapAttribute
{
	// Internal state.
	private int order;
	private String xmlElementName;

	// Constructor.
	public SoapFieldAttribute() {}

	// Get or set the attribute's properties.
	public int Order
			{
				get
				{
					return order;
				}
				set
				{
					order = value;
				}
			}
	public String XmlElementName
			{
				get
				{
					if(xmlElementName == null)
					{
						if(ReflectInfo != null)
						{
							xmlElementName = ((FieldInfo)ReflectInfo).Name;
						}
					}
					return xmlElementName;
				}
				set
				{
					xmlElementName = value;
					flag = true;
				}
			}

	// Determine if this attribute contains interop information.
	public bool IsInteropXmlElement()
			{
				return flag;
			}

}; // class SoapFieldAttribute

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata
