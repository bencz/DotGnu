/*
 * XmlUrlResolver.cs - Implementation of the "System.Xml.XmlUrlResolver" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
 *
 * Contributions from Deryk Robosson <deryk@0x0a.com>
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
 
namespace System.Xml
{

using System;
using System.IO;
using System.Net;

public class XmlUrlResolver : XmlResolver
{
	// Internal state.
	private ICredentials credentials;


	// Constructor.
	public XmlUrlResolver()
			{
				credentials = null;
			}


	// Set the credentials to use to resolve Web requests.
	public override ICredentials Credentials
			{
				set { credentials = value; }
			}


	// Map a URI to the entity it represents.
	[TODO]
	public override Object GetEntity(Uri absoluteUri, String role,
									 Type ofObjectToReturn)
			{
				// Validate the parameters.
				if(absoluteUri == null)
				{
					throw new ArgumentNullException("absoluteUri");
				}
				else if(ofObjectToReturn != null &&
				        ofObjectToReturn != typeof(Stream))
				{
					throw new XmlException(S._("Xml_MustBeStream"));
				}

				// Bail out if "ofObjectToReturn" is null.
				if(ofObjectToReturn == null)
				{
					return null;
				}

				if(absoluteUri.IsFile == false)
				{
					WebClient client = new WebClient();
					client.Credentials = credentials;
					return client.OpenRead(absoluteUri.ToString());
				}
				else if(absoluteUri.IsFile == true || absoluteUri.IsUnc == true)
				{
					return new FileStream
						(absoluteUri.LocalPath, FileMode.Open, FileAccess.Read,
						 FileShare.Read);
				}
				return null;
			}

}; // class XmlUrlResolver

}; // namespace System.Xml
