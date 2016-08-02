/*
 * XmlResolver.cs - Implementation of the "System.Xml.XmlResolver" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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

public abstract class XmlResolver
{
	// Schemes supported by the Uri class.
	private static readonly String[] schemes = new String[]
	{
		"file://",
		"ftp://",
		"gopher://",
		"http://",
		"https://",
		"mailto://",
		"news://",
		"nntp://"
	};


	// Constructor.
	protected XmlResolver() {}


	// Set the credentials to use to resolve Web requests.
	public abstract ICredentials Credentials { set; }


	// Map a URI to the entity it represents.
	public abstract Object GetEntity
				(Uri absoluteUri, String role, Type ofObjectToReturn);

	// Resolve a relative URI.
	public virtual Uri ResolveUri(Uri baseUri, String relativeUri)
			{
				if(baseUri == null && ((Object)relativeUri) == null)
				{
					throw new ArgumentNullException(S._("Xml_UnspecifiedUri"));
				}

				try
				{
					if(baseUri == null)
					{
						for(int i = 0; i < schemes.Length; ++i)
						{
							if(relativeUri.StartsWith(schemes[i]))
							{
								return new Uri(relativeUri);
							}
						}
						return new Uri(Path.GetFullPath(relativeUri));
					}
					else if(relativeUri == null)
					{
						return baseUri;
					}
					else
					{
						return new Uri(baseUri, relativeUri);
					}
				}
				catch(UriFormatException)
				{
					return null;
				}
			}

}; // class XmlResolver

}; // namespace System.Xml
