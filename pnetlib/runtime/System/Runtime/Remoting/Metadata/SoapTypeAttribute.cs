/*
 * SoapTypeAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Metadata.SoapTypeAttribute" class.
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
using System.Text;

[AttributeUsage(AttributeTargets.Class |
				AttributeTargets.Struct |
				AttributeTargets.Enum |
				AttributeTargets.Interface)]
public sealed class SoapTypeAttribute : SoapAttribute
{
	// Internal state.
	private SoapOption soapOptions;
	private String xmlElementName;
	private XmlFieldOrderOption xmlFieldOrder;
	private String xmlTypeName;
	private String xmlTypeNamespace;
	internal bool xmlElementWasSet;
	internal bool xmlTypeWasSet;

	// Constructor.
	public SoapTypeAttribute() {}

	// Get or set the attribute's properties.
	public SoapOption SoapOptions
			{
				get
				{
					return soapOptions;
				}
				set
				{
					soapOptions = value;
				}
			}
	public override bool UseAttribute
			{
				get
				{
					return false;
				}
				set
				{
					throw new RemotingException
						(_("NotSupp_SetRemotingValue"));
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
							xmlElementName = ((Type)ReflectInfo).Name;
						}
					}
					return xmlElementName;
				}
				set
				{
					xmlElementName = value;
					xmlElementWasSet = true;
				}
			}
	public XmlFieldOrderOption XmlFieldOrder
			{
				get
				{
					return xmlFieldOrder;
				}
				set
				{
					xmlFieldOrder = value;
				}
			}
	public override String XmlNamespace
			{
				get
				{
					if(ProtXmlNamespace == null)
					{
						ProtXmlNamespace = XmlTypeNamespace;
					}
					return ProtXmlNamespace;
				}
				set
				{
					ProtXmlNamespace = value;
					xmlElementWasSet = true;
				}
			}
	public String XmlTypeName
			{
				get
				{
					if(xmlTypeName == null)
					{
						if(ReflectInfo != null)
						{
							xmlTypeName = ((Type)ReflectInfo).Name;
						}
					}
					return xmlTypeName;
				}
				set
				{
					xmlTypeName = value;
					xmlTypeWasSet = true;
				}
			}
	public String XmlTypeNamespace
			{
				get
				{
					if(xmlTypeNamespace == null)
					{
						if(ReflectInfo != null)
						{
							xmlTypeNamespace = GetNamespaceForType
								((Type)ReflectInfo);
						}
					}
					return xmlTypeNamespace;
				}
				set
				{
					xmlTypeNamespace = value;
					xmlTypeWasSet = true;
				}
			}

	// Get the namespace corresponding to a particular type.
	internal static String GetNamespaceForType(Type type)
			{
				StringBuilder builder = new StringBuilder();
				if(type.Assembly == Assembly.GetExecutingAssembly())
				{
					builder.Append(SoapServices.XmlNsForClrTypeWithNs);
					builder.Append(type.FullName);
				}
				else
				{
					builder.Append
						(SoapServices.XmlNsForClrTypeWithNsAndAssembly);
					builder.Append(type.FullName);
					builder.Append('/');
					String assembly = type.Assembly.FullName;
					int index = assembly.IndexOf(',');
					if(index != -1)
					{
						builder.Append(assembly, 0, index);
					}
					else
					{
						builder.Append(assembly);
					}
				}
				return builder.ToString();
			}

}; // class SoapTypeAttribute

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata
