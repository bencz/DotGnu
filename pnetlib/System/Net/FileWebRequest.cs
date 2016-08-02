/*
 * FileWebRequest.cs - Implementation of "System.Net.FileWebRequest" class
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace System.Net
{
public class FileWebRequest : WebRequest
#if CONFIG_SERIALIZATION
,ISerializable
#endif
{

	private long contentLength;
	private WebHeaderCollection headers;
	private string method;
	private Uri requestUri;
	private int timeout = Int32.MaxValue;
	private FileWebResponse response = null;
	
	internal FileWebRequest(Uri uri)
	{
		requestUri = uri;
		contentLength = -1;
		method = "GET";
		headers = new WebHeaderCollection();
	}

#if CONFIG_SERIALIZATION
	[TODO]
	protected FileWebRequest(SerializationInfo serializationInfo, 
							 StreamingContext streamingContext)
	{
		throw new NotImplementedException("ctor");
	}

	// Get the serialization data for this object.
	[TODO]
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
	{
	}
#endif

	[TODO]
	public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, Object state)
	{
		throw new NotImplementedException("BeginGetRequestStream");
	}

	[TODO]
	public override IAsyncResult BeginGetResponse(AsyncCallback callback, Object state)
	{
		throw new NotImplementedException("BeginGetResponse");
	}

	[TODO]
	public override Stream EndGetRequestStream(IAsyncResult asyncResult)
	{
		throw new NotImplementedException("EndGetRequestStream");
	}

	[TODO]
	public override WebResponse EndGetResponse(IAsyncResult asyncResult)
	{
		throw new NotImplementedException("EndGetResponse");
	}

	[TODO]
	public override Stream GetRequestStream()
	{
		throw new NotImplementedException("GetRequestStream");
	}

	public override WebResponse GetResponse()
	{
		lock(this)
		{
			if(response == null)
			{
				response = new FileWebResponse(this, 
								File.OpenRead(requestUri.LocalPath));
			}
		}
		return response;
	}

	public override String ConnectionGroupName 
	{
		get
		{
			return String.Empty;
		}
		set
		{
			/* Nothing ? */
		}
	}

	public override long ContentLength 
	{
		get
		{
			return Int64.Parse(headers["Content-Length"]);
		}
		set
		{
			headers["Content-Length"] = value.ToString();
		}
	}

	public override String ContentType 
	{
		get
		{
			return headers["Content-Type"];
		}
		set
		{
			headers["Content-Type"] = value;
		}
	}

	public override ICredentials Credentials 
	{
		get
		{
			return null;
		}
		set
		{
			/* Nothing */
		}
	}

	public override WebHeaderCollection Headers 
	{
		get
		{
			return headers;
		}
	}

	public override String Method 
	{
		get
		{
			return this.method;
		}
		set
		{
			if(value != "GET" && value != "POST")
			{
				/* TODO : resources */
				throw new ArgumentException("invalid method"); 
			}
			this.method = value;
		}
	}

	public override bool PreAuthenticate 
	{
		get
		{
			return false;
		}
		set
		{
			/* Nothing */
		}
	}

	public override IWebProxy Proxy 
	{
		get
		{
			return null;
		}
		set
		{
			/* Nothing */
		}
	}

	public override Uri RequestUri 
	{
		get
		{
			return requestUri;
		}
	}

	public override int Timeout 
	{
		get
		{
			return this.timeout;
		}
		set
		{
			this.timeout = value;
		}
	}

}
}//namespace
