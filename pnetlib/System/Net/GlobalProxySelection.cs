/*
 * GlobalProxySelection.cs - Implementation of the
 *			"System.Net.GlobalProxySelection" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Net
{

public class GlobalProxySelection 
{
	// Internal state.
	private static IWebProxy select;

	// Create a new empty proxy.
	public static IWebProxy GetEmptyWebProxy()
			{
				return new EmptyWebProxy();
			}

	// Get or set the selected global proxy object.
	[TODO]
	public static IWebProxy Select
			{
				get
				{
					lock(typeof(GlobalProxySelection))
					{
						if(select == null)
						{
							// TODO: read the proxy information from
							// the configuration settings.
							String proxy = Environment.GetEnvironmentVariable("PNET_PROXY");
							try
							{	
								if(proxy != null)
								{
									Uri proxyUri = new Uri(proxy);
									select = new WebProxy(proxyUri);
									String userinfo=proxyUri.UserInfo;
									if(userinfo != null && userinfo != "" &&
													userinfo.IndexOf(":")!=-1)
									{
										String [] auth=userinfo.Split(':');
										select.Credentials = 
												new NetworkCredential(auth[0],auth[1]);
									}
									
								}
							}
							catch(UriFormatException)
							{
								//System.Diagnostics.Debug.WriteLine("Invalid proxy");
							}

							if(select == null)
							{
								select = new EmptyWebProxy();
							}
						}
						return select;
					}
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					lock(typeof(GlobalProxySelection))
					{
						select = value;
					}
				}
			}

	// Dummy class that represents empty proxy settings.
	private class EmptyWebProxy : IWebProxy
	{
		// Internal state.
		private ICredentials credentials;

		// Constructor.
		public EmptyWebProxy() {}

		// Get proxy information for a destination.
		public Uri GetProxy(Uri destination)
				{
					return destination;
				}

		// Determine if the proxy is bypassed for a host
		public bool IsBypassed(Uri host)
				{
					return true;
				}
		
		// Get or set the proxy credentials.
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

	}; // class EmptyWebProxy
	
}; // class GlobalProxySelection

}; // namespace System.Net
