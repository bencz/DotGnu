/*
 * FileWebResponse.cs - Implementation of "System.Net.FileWebResponse" class
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

public class FileWebResponse : WebResponse, IDisposable
#if CONFIG_SERIALIZATION
, ISerializable
#endif
{
	private FileWebRequest request;
	private WebHeaderCollection headers;
	private Stream stream;
	private long contentLength;
	private bool disposed;

	internal FileWebResponse(FileWebRequest request, Stream stream)
	{
		this.request = request;
		this.stream = stream;
		this.disposed = false;
		this.headers = new WebHeaderCollection();
		try
		{
			FileInfo fileInfo = new FileInfo(request.RequestUri.LocalPath);
			contentLength = fileInfo.Length;
			headers["Content-Length"] = contentLength.ToString();
		}
		catch(Exception e)
		{
		}
	}

#if CONFIG_SERIALIZATION
	[TODO]
	protected FileWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
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

	void IDisposable.Dispose()
	{
		Dispose(true);
	}

	protected virtual void Dispose(bool disposing)
	{
		Close();
		headers = null;
		request = null;
		contentLength = 0;
		disposed = true;
	}

	public override void Close()
	{
		if(stream != null)
		{
			stream.Close();
			stream = null;
		}
		disposed = true;
	}

	public override Stream GetResponseStream()
	{
		return stream;
	}

	public override long ContentLength 
	{
		get
		{
			return contentLength;
		}
	}

	public override String ContentType 
	{
		get
		{
			// Implement a bit of extension based detection at least
			return null;
		}
	}

	public override WebHeaderCollection Headers 
	{
		get
		{
			return this.headers;
		}
	}

	public override Uri ResponseUri 
	{
		get
		{
			// Symlinks could be this, but forget that
			return null;
		}
	}
}

}//namespace
