/*
 * HttpWebResponse.cs - Implementation of the
 *		"System.Net.HttpWebResponse" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Text;
using System.Globalization;

public class HttpWebResponse : WebResponse
{
	private HttpWebRequest req=null;
	private Stream stream=null;
	private WebHeaderCollection headers=new WebHeaderCollection();
	private Version version=null;
	private HttpStatusCode code=0;
	private String desc=null;
	internal HttpWebResponse(HttpWebRequest request,Stream dataStream)
	{
		req=request;
		stream=dataStream;
		ProcessRequest();
		headers.SetStrict(false); /* none of the restrictions of Request */
		ProcessHeaders();
	}
	private void ProcessRequest()
	{
		String response=ReadLine();
		int i=response.IndexOfAny(new char[]{' ','\t'});
		if(i==-1) throw new WebException("Invalid Response");
		switch(response.Substring(0,i))
		{
			case "HTTP/1.0":
				version=HttpVersion.Version10;
				break;
			case "HTTP/1.1":
				version=HttpVersion.Version11;
				break;
			default:
				throw new WebException("Unknown Protocol version");
		}
		response=response.Substring(i+1);
		i=response.IndexOfAny(new char[]{' ','\t'});
		if(i==-1) throw new WebException("Invalid Response");
		code=(HttpStatusCode) Int32.Parse(response.Substring(0,i));
		response=response.Substring(i+1);
		desc=response;
	}
	private void ProcessHeaders()
	{
		/* sometimes servers tend to send plain "\n"s */
		for(String s=ReadLine();s.Trim()!="";s=ReadLine())
		{
			headers.Add(s); 
		}
	}
	private String ReadLine()
	{
		StringBuilder builder = new StringBuilder();
		int ch;
		for(;;)
		{
			// Process characters until we reach a line terminator.
			ch=stream.ReadByte();
			if(ch==-1)
			{
				break;
			}
			else if(ch == 13)
			{
				if((ch=stream.ReadByte())==10)
				{
					return builder.ToString();
				}
				else if(ch==-1)
				{
					break;
				}
				else
				{
					builder.Append("\r"+(byte)ch);
					/* that "\r" is added to the stuff */
				}
			}
			else if(ch == 10)
			{
				// This is an LF line terminator.
				return builder.ToString();
			}
			else
			{
				builder.Append((char)ch);
			}
		}
		if(builder.Length!=0) return builder.ToString(); 
		else return null;
	}
	public override void Close()
	{
		this.req.Abort();
	}

	[TODO]
	protected virtual void Dispose(bool disposing)
	{
		throw new NotImplementedException();
	}

	[TODO]
	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

	public string GetResponseHeader(string headerName)
	{
		return headers[headerName];
	}

	public override Stream GetResponseStream()
	{
		if(String.Compare(headers["Transfer-Encoding"],"chunked", true) == 0)
		{
			return new ChunkedDecoderStream(stream);
		}
		else
		{
			return stream;  
		}
	}

	public string CharacterSet 
	{ 
		get
		{
			return "ISO-8859-1";
			/*
				TODO: figure out how to get correct CharacterSet,
				all headers don't have charsets
			*/
		}
	}

	public string ContentEncoding 
	{ 
		get
		{
			return headers["Content-Encoding"];
		}
	}

	public override long ContentLength 
	{ 
		get
		{
			String contentLength=headers["Content-Length"];
			if(contentLength==null) return 0;
			else return Int64.Parse(contentLength);
		}
	}

	public override string ContentType 
	{ 
		get
		{
			return headers["Content-Type"];
		}
	}

	public override WebHeaderCollection Headers 
	{
		get
		{
			return headers;
		}
	}
	
#if !ECMA_COMPAT
	[TODO]
	public CookieCollection Cookies 
	{
 		get
		{
			return null;
		}
		set
		{
		}
	}
#endif // !ECMA_COMPAT

	public DateTime LastModified 
	{ 
		get 
		{
			String []formats=new String[] { 	
						"ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", // RFC 1123
						"dddd, dd-MMM-yy HH:mm:ss \\G\\M\\T" , // RFC 850
						"ddd MMM dd HH:mm:ss yyyy"			   // asctime()
						};
			return DateTime.ParseExact(headers["Last-Modified"],formats,null,
						DateTimeStyles.None);
		}
	}

	public string Method 
	{ 
		get
		{
			return req.Method;
		}
	}

	public Version ProtocolVersion 
	{ 
		get
		{
			return version;
		}
	}

	public override Uri ResponseUri 
	{ 
		get
		{
			if(code==HttpStatusCode.Redirect || 
			   code==HttpStatusCode.MovedPermanently ||
			   code==HttpStatusCode.Moved ||
			   code==HttpStatusCode.MultipleChoices ||
			   code==HttpStatusCode.Found ||
			   code==HttpStatusCode.SeeOther ||
			   code==HttpStatusCode.TemporaryRedirect)
			{
				return new Uri(this.Headers["Location"]);
			}
			return null;
		}
	}

	public string Server 
	{
		get
		{
			return headers["Server"];
		}
	}

	public HttpStatusCode StatusCode 
	{ 
		get
		{
			return code;
		}
	}

	public string StatusDescription 
	{ 
		get
		{
			return desc;
		}
	}

	private class ChunkedDecoderStream : Stream
	{
		private Stream inputStream;
		private int chunkSize;

		public ChunkedDecoderStream ( Stream inputStream )
		{
			this.inputStream = inputStream;
			ReadChunkSize(false);
		}

		public override bool CanRead
		{
			get 
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get 
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("get_Length");
			}
		}

		public override long Position
		{
			get 
			{
				throw new NotSupportedException("get_Position");
			}
			set 
			{
				throw new NotSupportedException("set_Position");
			}
		}

		public override void Flush()
		{
		}

		public override void Close()
		{
			inputStream.Close();
		}
	
		public override int Read( byte[] buffer, int offset, int count )
		{
			/* Note: this reads only what is available in the next
			   chunk . This is consistent with most network streams
			   so as long as return of Read() is > 0 , keep reading */
			lock(this) // Lock needed to ensure that the chunkSize
			{
				if(chunkSize == 0)
				{
					return 0;
				}

				int toRead = Math.Min(count, chunkSize);
				int bytesRead = inputStream.Read ( buffer, offset, toRead );

				chunkSize = chunkSize - bytesRead;
				
				if(chunkSize == 0)
				{
					ReadChunkSize(true);
				}
				return bytesRead;
			}
		}

		public override long Seek ( long offset, SeekOrigin origin )
		{
			throw new NotSupportedException("Seek");
		}

		public override void SetLength ( long value )
		{
			throw new NotSupportedException("SetLength");
		}
		
		public override void Write ( byte[] buffer, int offset, int count )
		{
			throw new NotSupportedException("Write");
		}

		private void ReadChunkSize( bool readCRLFfirst )
		{
			int b;
			if (readCRLFfirst)
			{
				b = inputStream.ReadByte();
				if (b != 13)
				{
					throw new IOException("CR expected");
				}
				b = inputStream.ReadByte();
				if (b != 10)
				{
					throw new IOException("LF expected");
				}
			}

			bool append = true;
			chunkSize = 0;	


			/* TODO: rewrite this bit to be a lot clearer 
			         someday . Right now it stands as it is */
			
			while(true)
			{
				b = inputStream.ReadByte();
				if (b == 13)
				{
					b = inputStream.ReadByte();
					if (b != 10)
					{
						throw new IOException("LF expected");
					}
					break;
				}
				if (b == -1)
				{
					throw new IOException("unexpected end of stream");
				}
				if(b == 59 || b == 32 || b == 9) // chunk extension, we don't care
				{
					append = false;
				}
				if(append)
				{
					int digit;

					if (b>=48 && b<=57)  // 0..9
						digit = b-48;
					else if (b>=65 && b<=70) // A..F
						digit = b-65+10;
					else if (b>=97 && b<=102) // a..f
						digit = b-97+10;
					else
						throw new IOException("hex digit expected");

					chunkSize = (chunkSize*16)+digit;
				}
			}
		}
	} // class ChunkedDecoderStream
}; // class HttpWebResponse

}; //namespace System.Net
