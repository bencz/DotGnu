/*
 * WebRequest.cs - Implementation of the "System.Net.WebRequest" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * With contributions from Jason Lee <jason.lee@mac.com>
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

using System;
using System.IO;
using System.Collections;

public abstract class WebRequest : MarshalByRefObject
{
	private String				connectionGroupName;
	private Int64			  	contentLength;
	private String				contentType;
	private ICredentials		credentials;
	private WebHeaderCollection headers;
	private String				method;
	private Boolean				preAuthenticate;
	private IWebProxy			proxy;
	private Uri					requestUri;
	private Int32				timeout;
	private static Hashtable	prefixes=new Hashtable();

	static WebRequest()
	{
		prefixes.Clear();

		// Register the prefix types here
		RegisterPrefix(Uri.UriSchemeHttp,  new WebRequestCreator());
		RegisterPrefix(Uri.UriSchemeHttps, new WebRequestCreator());
		RegisterPrefix(Uri.UriSchemeFile, new WebRequestCreator());

		// TODO: More prefixes, such as those contained in Uri should come later
	}
	
	protected WebRequest()
	{
		connectionGroupName = "";
		contentLength = 0;
		contentType = "";
		credentials = null;
		method = "";
		preAuthenticate = false;
		proxy = null;
		requestUri = null;
		timeout = 0;	
	}

	public virtual void Abort()
	{
		throw new NotSupportedException("Abort");
	}

	public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
	{
		throw new NotSupportedException("BeginGetRequestStream");
	}

	public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
	{
		throw new NotSupportedException("BeginGetResponse");
	}

	public static WebRequest Create(String requestUriString)
	{
		if (requestUriString == null)
		{
			throw new ArgumentNullException("requestUriString");
		}

		Uri requestUri = new Uri(requestUriString);		
		
		return Create(requestUri);
		
	}

	// This method will attempt to create a particular subclass of WebRequest
	// based on the scheme from the uri passed in. Currently on HttpWebRequest
	// is supported.
	public static WebRequest Create(Uri requestUri)
	{

		IWebRequestCreate theCreator = null;
		
		if(CheckUriValidity(requestUri, false))
		{

			// Check here to see if the Uri scheme exists in the
			// prefixes table and if so, then return back the
			// proper WebRequest for it
			theCreator = (prefixes[requestUri.Scheme] as IWebRequestCreate);
			
			if(theCreator!=null)
			{
				return theCreator.Create(requestUri);
			}

		// TODO: this client does not have the permission to connect to the URI or
		// the URI that the request is redirected to.
		// throw new SecurityException("requestUriString");
		}
		   
		return null;
	}

	// This method will attempt to create a 'default' WebRequest. In this case
	// we're assuming the default is a HttpWebRequest. Non-default requests
	// will need to be created with the Create method.
	public static WebRequest CreateDefault(Uri requestUri)
	{
		// TODO: this client does not have the permission to connect to the URI or
		// the URI that the request is redirected to.
		// throw new SecurityException("requestUriString");
		if(CheckUriValidity(requestUri, true))
		{
			if(String.Equals(requestUri.Scheme, Uri.UriSchemeHttp) ||
				String.Equals(requestUri.Scheme, Uri.UriSchemeHttps))
			{
				return new HttpWebRequest(requestUri);
			}
			if(requestUri.IsFile)
			{
				return new FileWebRequest(requestUri);
			}
			throw new NotSupportedException("CreateDefault");
		}

		return null;
		
	}

	public virtual Stream EndGetRequestStream(IAsyncResult asyncResult)
	{
		throw new NotSupportedException("EndGetRequestStream");
	}

	public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
	{
		throw new NotSupportedException("EndGetResponse");
	}

	public virtual Stream GetRequestStream()	
	{
		throw new NotSupportedException("GetRequestStream");
	}

	public virtual WebResponse GetResponse()
	{
		throw new NotSupportedException("GetResponse");
	}

	public static bool RegisterPrefix(String prefix, IWebRequestCreate creator)
	{
		if (prefix== null)
		{
			throw new ArgumentNullException("prefix", S._("Arg_NotNull"));
		}

		if (creator== null)
		{
			throw new ArgumentNullException("creator", S._("Arg_NotNull"));
		}

		if(prefixes.Contains( prefix.ToLower() ))
		{
			return false;
		}
		else
		{
			prefixes.Add(prefix.ToLower(), creator);
		}
			
		return true;
	}

	private static bool CheckUriValidity(Uri requestUri, bool defaultUri)
	{

		// defaultUri is provided so we can throw the proper exception
		// based on whether or not we're referred from the regular Create 
		// method or if we're actually using the DefaultCreate
		
		if (requestUri == null)
		{
			throw new ArgumentNullException ("requestUri");
		}

		if(!Uri.CheckSchemeName(requestUri.Scheme))
		{
			if(defaultUri)
			{
				throw new NotSupportedException("CreateDefault");
			}
			else
			{
				throw new NotSupportedException("Create");
			}
			
		}

		// TODO: There's probably additional checking for what constitutes the
		// query portion of a Uri, but I'm not sure what it is yet. Probably
		// look into this later.
		if(!(requestUri.IsFile) && 
			requestUri.HostNameType.Equals(UriHostNameType.Unknown))
		{
			throw new UriFormatException("requestUri");
		}

		return true;
	}

	// Internal classes needed to use with the RegisterPrefix method as per spec
	// These could have been done as 'helpers' but they're only needed in this class
	internal class WebRequestCreator : IWebRequestCreate
	{
		internal WebRequestCreator()
		{
		}
	
		public WebRequest Create(Uri uri)
		{
			return WebRequest.CreateDefault(uri);
		}
	}

	// properties

	public virtual string ConnectionGroupName 
	{ 
		get
		{
			throw new NotSupportedException("ConnectionGroupName");
		}
		
		set
		{
			throw new NotSupportedException("ConnectionGroupName");
		}
	}

	public virtual long ContentLength
	{
		get
		{
			throw new NotSupportedException("ContentLength");
		}
		
		set
		{
			throw new NotSupportedException("ContentLength");
		}
	}

	public virtual string ContentType
	{
		get
		{
			throw new NotSupportedException("ContentType");
		}
		
		set
		{
			throw new NotSupportedException("ContentType");
		}
	}

	public virtual ICredentials Credentials
	{
		get
		{
			throw new NotSupportedException("Credentials");
		}
		
		set
		{
			throw new NotSupportedException("Credentials");
		}
	}

	public virtual WebHeaderCollection Headers
	{
		get
		{
			throw new NotSupportedException("Headers");
		}
		
		set
		{
			throw new NotSupportedException("Headers");
		}
	}

	public virtual string Method
	{
		get
		{
			throw new NotSupportedException("Method");
		}
		
		set
		{
			throw new NotSupportedException("Method");
		}
	}

	public virtual bool PreAuthenticate
	{
		get
		{
			throw new NotSupportedException("PreAuthenticate");
		}
		
		set
		{
			throw new NotSupportedException("PreAuthenticate");
		} 
	}

	public virtual IWebProxy Proxy
	{
		get
		{
			throw new NotSupportedException("Proxy");
		}
		
		set
		{
			throw new NotSupportedException("Proxy");
		}
	}

	public virtual Uri RequestUri
	{
		get
		{
			throw new NotSupportedException("RequestUri");
		}
		
		set
		{
			throw new NotSupportedException("RequestUri");
		}
	}

	public virtual int Timeout
	{
		get
		{
			throw new NotSupportedException("Timeout");
		}
		
		set
		{
			throw new NotSupportedException("Timeout");
		} 
	}
	
}; // class WebRequest

}; // namespace System.Net
