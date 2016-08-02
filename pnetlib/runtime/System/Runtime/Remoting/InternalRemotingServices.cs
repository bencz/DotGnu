/*
 * InternalRemotingServices.cs - Implementation of the
 *			"System.Runtime.Remoting.InternalRemotingServices" class.
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

namespace System.Runtime.Remoting
{

#if CONFIG_SERIALIZATION

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;

public class InternalRemotingServices
{
	// Internal state.
	private static Hashtable attributeHash;

	// Output debug information.  Not used in this implementation.
	[Conditional("_LOGGING")]
	public static void DebugOutChnl(String s) {}
	[Conditional("_LOGGING")]
	public static void RemotingTrace(params Object[] messages) {}
	[Conditional("_DEBUG")]
	public static void RemotingAssert(bool condition, String message) {}

#if CONFIG_REMOTING

	// Set the server identity on a method call object.
	[CLSCompliant(false)]
	public static void SetServerIdentity(MethodCall m, Object srvID)
			{
				m.SetServerIdentity(srvID);
			}

#endif

	// Get the cached SOAP attribute data for an object.
	public static SoapAttribute GetCachedSoapAttribute(Object reflectionObject)
			{
				// Validate the paramter to ensure that it is a
				// legitimate reflection object.
				if(reflectionObject == null)
				{
					return null;
				}
				else if(!(reflectionObject is MemberInfo) &&
						!(reflectionObject is ParameterInfo))
				{
					return null;
				}
				lock(typeof(InternalRemotingServices))
				{
					Object attr;
					Object[] attrs;

					// Look for a cached value from last time.
					if(attributeHash == null)
					{
						attributeHash = new Hashtable();
					}
					else if((attr = attributeHash[reflectionObject]) != null)
					{
						return (attr as SoapAttribute);
					}

					// Get the attribute information from the type.
					if(reflectionObject is Type)
					{
						attrs = ((Type)reflectionObject).GetCustomAttributes
							(typeof(SoapTypeAttribute), true);
						if(attrs == null || attrs.Length < 1)
						{
							attr = new SoapTypeAttribute();
						}
						else
						{
							attr = attrs[0];
						}
					}
					else if(reflectionObject is MethodBase)
					{
						attrs = ((MethodBase)reflectionObject)
							.GetCustomAttributes
								(typeof(SoapMethodAttribute), true);
						if(attrs == null || attrs.Length < 1)
						{
							attr = new SoapMethodAttribute();
						}
						else
						{
							attr = attrs[0];
						}
					}
					else if(reflectionObject is FieldInfo)
					{
						attrs = ((FieldInfo)reflectionObject)
							.GetCustomAttributes
								(typeof(SoapFieldAttribute), true);
						if(attrs == null || attrs.Length < 1)
						{
							attr = new SoapFieldAttribute();
						}
						else
						{
							attr = attrs[0];
						}
					}
					else if(reflectionObject is ParameterInfo)
					{
						attrs = ((ParameterInfo)reflectionObject)
							.GetCustomAttributes
								(typeof(SoapParameterAttribute), true);
						if(attrs == null || attrs.Length < 1)
						{
							attr = new SoapParameterAttribute();
						}
						else
						{
							attr = attrs[0];
						}
					}
					else
					{
						attrs = ((MemberInfo)reflectionObject)
							.GetCustomAttributes(typeof(SoapAttribute), true);
						if(attrs == null || attrs.Length < 1)
						{
							attr = new SoapAttribute();
						}
						else
						{
							attr = attrs[0];
						}
					}
					((SoapAttribute)attr).SetReflectInfo(reflectionObject);
					return (SoapAttribute)attr;
				}
			}

}; // class InternalRemotingServices

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting
