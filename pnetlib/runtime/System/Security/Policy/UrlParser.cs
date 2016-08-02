/*
 * UrlParser.cs - Implementation of the
 *		"System.Security.Policy.UrlParser" class.
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

namespace System.Security.Policy
{

#if CONFIG_POLICY_OBJECTS

internal sealed class UrlParser
{
	// Internal state.
	private String url;
	private String scheme;
	private String userInfo;
	private String host;
	private String port;
	private String rest;

	// Constructor.
	public UrlParser(String url)
			{
				if(url == null)
				{
					throw new ArgumentNullException("url");
				}
				this.url = url;
				if(!Parse(url))
				{
					throw new ArgumentException(_("Arg_InvalidUrl"));
				}
			}

	// Parse a URL into its constituent components.
	private bool Parse(String url)
			{
				int index;

				// Extract the scheme.
				index = url.IndexOf(':');
				if(index == -1)
				{
					return false;
				}
				scheme = url.Substring(0, index);
				if(scheme.IndexOf('/') != -1)
				{
					return false;
				}

				// Split the URL between the "host" and "rest" portions.
				if((index + 2) < url.Length &&
				   url[index + 1] == '/' && url[index + 2] == '/')
				{
					url = url.Substring(index + 3);
					index = url.IndexOf('/');
					if(index != -1)
					{
						rest = url.Substring(index);
						host = url.Substring(0, index);
					}
					else
					{
						host = url;
						rest = "/";
					}
				}
				else
				{
					// Probably something like "mailto:xxx".
					host = String.Empty;
					rest = url.Substring(index + 1);
				}

				// Pull out the port number and the user login information.
				index = host.LastIndexOf(':');
				if(index != -1)
				{
					port = host.Substring(index + 1);
					host = host.Substring(0, index);
				}
				else
				{
					port = String.Empty;
				}
				index = host.LastIndexOf('@');
				if(index != -1)
				{
					userInfo = host.Substring(0, index);
					host = host.Substring(index + 1);
				}
				else
				{
					userInfo = String.Empty;
				}
				return true;
			}

	// Get the URL components.
	public String URL
			{
				get
				{
					return url;
				}
			}
	public String Scheme
			{
				get
				{
					return scheme;
				}
			}
	public String UserInfo
			{
				get
				{
					return userInfo;
				}
			}
	public String Host
			{
				get
				{
					return host;
				}
			}
	public String Port
			{
				get
				{
					return port;
				}
			}
	public String Rest
			{
				get
				{
					return rest;
				}
			}

	// Determine if we have a host subset match.
	public static bool HostMatches(String pattern, String actual)
			{
				if(pattern.Length > actual.Length)
				{
					return false;
				}
				if(String.Compare(pattern, 0, actual,
								  actual.Length - pattern.Length,
								  pattern.Length, true) != 0)
				{
					return false;
				}
				if(pattern.Length == actual.Length)
				{
					return true;
				}
				else
				{
					return (actual[actual.Length - pattern.Length - 1] == '.');
				}
			}

	// Determine if we have a URL subset match.
	public bool Matches(UrlParser url)
			{
				// Compare the scheme and host information.
				if(String.Compare(scheme, url.Scheme, true) != 0)
				{
					return false;
				}
				if(String.Compare(userInfo, url.UserInfo, true) != 0)
				{
					return false;
				}
				if(!HostMatches(host, url.Host))
				{
					return false;
				}
				if(String.Compare(port, url.Port, true) != 0)
				{
					return false;
				}

				// Compare the "rest" fields.
				if(rest.EndsWith("*"))
				{
					String r = rest.Substring(0, rest.Length - 1);
					String ur = url.Rest;
					if(r.Length > ur.Length)
					{
						return false;
					}
					if(String.Compare(r, 0, ur, 0, r.Length, true) != 0)
					{
						return false;
					}
				}
				else
				{
					if(String.Compare(rest, url.Rest, true) != 0)
					{
						return false;
					}
				}
				return true;
			}

}; // class UrlParser

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
