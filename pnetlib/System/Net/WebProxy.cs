/*
 * WebProxy.cs - Implementation of the "System.Net.WebProxy" class.
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

namespace System.Net
{

using System.Collections;
using System.Runtime.Serialization;

public class WebProxy : IWebProxy
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Internal state.
	private Uri address;
	private bool bypassOnLocal;
	private ArrayList bypassList;
	private ICredentials credentials;

	// Constructors.
	public WebProxy() {}
	public WebProxy(String Address)
			{
				if(Address != null)
				{
					if(Address.IndexOf("://") == -1)
					{
						address = new Uri("http://" + Address);
					}
					else
					{
						address = new Uri(Address);
					}
				}
			}
	public WebProxy(Uri Address)
			{
				address = Address;
			}
	public WebProxy(String Address, bool BypassOnLocal)
			: this(Address)
			{
				bypassOnLocal = BypassOnLocal;
			}
	public WebProxy(String Host, int Port)
			{
				address = new Uri("http://" + Host + ":" + Port.ToString());
			}
	public WebProxy(Uri Address, bool BypassOnLocal)
			{
				address = Address;
				bypassOnLocal = BypassOnLocal;
			}
	public WebProxy(String Address, bool BypassOnLocal, String[] BypassList)
			: this(Address)
			{
				bypassOnLocal = BypassOnLocal;
				bypassList = MakeBypassList(BypassList);
			}
	public WebProxy(Uri Address, bool BypassOnLocal, String[] BypassList)
			{
				address = Address;
				bypassOnLocal = BypassOnLocal;
				bypassList = MakeBypassList(BypassList);
			}
	public WebProxy(String Address, bool BypassOnLocal,
					String[] BypassList, ICredentials Credentials)
			: this(Address)
			{
				bypassOnLocal = BypassOnLocal;
				bypassList = MakeBypassList(BypassList);
				credentials = Credentials;
			}
	public WebProxy(Uri Address, bool BypassOnLocal,
					String[] BypassList, ICredentials Credentials)
			{
				address = Address;
				bypassOnLocal = BypassOnLocal;
				bypassList = MakeBypassList(BypassList);
				credentials = Credentials;
			}
#if CONFIG_SERIALIZATION
	protected WebProxy(SerializationInfo serializationInfo,
					   StreamingContext streamingContext)
			{
				if(serializationInfo == null)
				{
					throw new ArgumentNullException("serializationInfo");
				}
				bypassOnLocal = serializationInfo.GetBoolean("_BypassOnLocal");
				address = (Uri)(serializationInfo.GetValue
						("_ProxyAddress", typeof(Uri)));
				bypassList = (ArrayList)(serializationInfo.GetValue
						("_BypassList", typeof(ArrayList)));
			}
#endif

	// Make a bypass list from a list of strings.
	private static ArrayList MakeBypassList(String[] list)
			{
				if(list == null)
				{
					return new ArrayList();
				}
				else
				{
					return new ArrayList(list);
				}
			}

	// Get or set this object's properties.
	public Uri Address
			{
				get
				{
					return address;
				}
				set
				{
					address = value;
				}
			}
	public ArrayList BypassArrayList
			{
				get
				{
					return bypassList;
				}
			}
	public String[] BypassList
			{
				get
				{
					return (String[])(bypassList.ToArray(typeof(String)));
				}
				set
				{
					bypassList = MakeBypassList(value);
				}
			}
	public bool BypassProxyOnLocal
			{
				get
				{
					return bypassOnLocal;
				}
				set
				{
					bypassOnLocal = value;
				}
			}
	public ICredentials Credentials
			{
				get
				{
					return credentials;
				}
				set
				{
					credentials = value;
				}
			}

	// Get the default proxy settings from the system configuration.
	[TODO]
	public static WebProxy GetDefaultProxy()
			{
				// TODO: read the system configuration
				return new WebProxy();
			}

	// Get the proxy to use for a particular URI destination.
	public Uri GetProxy(Uri destination)
			{
				if(IsBypassed(destination))
				{
					return destination;
				}
				else if(address != null)
				{
					return address;
				}
				else
				{
					return destination;
				}
			}

	// Dtermine if a URI destination has been bypassed.
	[TODO]
	public bool IsBypassed(Uri host)
			{
				if(host.IsLoopback)
				{
					return true;
				}
				if(bypassOnLocal && host.Host.IndexOf('.') == -1)
				{
					return true;
				}
				// TODO: scan the regexes in the bypass list for a match
				return false;
			}

#if CONFIG_SERIALIZATION

	// Serialize this object.
	void ISerializable.GetObjectData(SerializationInfo serializationInfo,
									 StreamingContext streamingContext)
			{
				if(serializationInfo == null)
				{
					throw new ArgumentNullException("serializationInfo");
				}
				serializationInfo.AddValue("_BypassOnLocal", bypassOnLocal);
				serializationInfo.AddValue("_ProxyAddress", address);
				serializationInfo.AddValue("_BypassList", bypassList);
			}

#endif

}; // class WebProxy

}; // namespace System.Net
