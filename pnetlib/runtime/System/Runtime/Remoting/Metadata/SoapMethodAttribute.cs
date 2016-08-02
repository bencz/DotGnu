/*
 * SoapMethodAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Metadata.SoapMethodAttribute" class.
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

[AttributeUsage(AttributeTargets.Method)]
public sealed class SoapMethodAttribute : SoapAttribute
{
	// Internal state.
	private String responseXmlElementName;
	private String responseXmlNamespace;
	private String returnXmlElementName;
	private String soapAction;
	internal bool soapActionWasSet;

	// Constructor.
	public SoapMethodAttribute() {}

	// Get or set the attribute's properties.
	public String ResponseXmlElementName
			{
				get
				{
					if(responseXmlElementName == null)
					{
						if(ReflectInfo != null)
						{
							responseXmlElementName =
								((MemberInfo)ReflectInfo).Name + "Response";
						}
					}
					return responseXmlElementName;
				}
				set
				{
					responseXmlElementName = value;
				}
			}
	public String ResponseXmlNamespace
			{
				get
				{
					if(responseXmlNamespace != null)
					{
						return responseXmlNamespace;
					}
					else
					{
						return XmlNamespace;
					}
				}
				set
				{
					responseXmlNamespace = value;
				}
			}
	public String ReturnXmlElementName
			{
				get
				{
					if(returnXmlElementName == null)
					{
						return "return";
					}
					return returnXmlElementName;
				}
				set
				{
					returnXmlElementName = value;
				}
			}
	public String SoapAction
			{
				get
				{
					if(soapAction == null)
					{
						soapAction = GetReflectNamespace() + "#" +
									 ((MemberInfo)ReflectInfo).Name;
					}
					return soapAction;
				}
				set
				{
					soapAction = value;
					soapActionWasSet = true;
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
	public override String XmlNamespace
			{
				get
				{
					if(ProtXmlNamespace == null)
					{
						ProtXmlNamespace = GetReflectNamespace();
					}
					return ProtXmlNamespace;
				}
				set
				{
					ProtXmlNamespace = value;
				}
			}

	// Get the namespace corresponding to the reflected member.
	private String GetReflectNamespace()
			{
				if(ReflectInfo != null)
				{
					Type type = ((MemberInfo)ReflectInfo).DeclaringType;
					return SoapTypeAttribute.GetNamespaceForType(type);
				}
				return null;
			}

}; // class SoapMethodAttribute

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata
