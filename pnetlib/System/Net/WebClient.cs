/*
 * WebClient.cs - Implementation of "WebClient" class 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V 
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
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.Net
{
#if !ECMA_COMPAT
	[ComVisible(true)]
#endif
	public sealed class WebClient
#if CONFIG_COMPONENT_MODEL
		: Component
#endif
	{
		private WebRequest webrequest=null;
		private WebResponse webresponse=null;
		private String baseAddress=null;
		private ICredentials credentials = null;

		const String defaultContentType = "application/octet-stream";
		const String fileContentType = "multipart/form-data";
		const String formContentType = "application/x-www-form-urlencoded";

		private void CreateWebrequest(String str)
		{
			if(str == null)
			{
				throw new ArgumentNullException("str");
			}
			if(webrequest!=null)
			{
				throw new WebException("Multiple connection attempts");
			}
			Uri uri = new Uri(str);
			webrequest = WebRequest.Create(uri);
			if(webrequest is HttpWebRequest)
			{
				(webrequest as HttpWebRequest).KeepAlive = false;
			}
			
			if(credentials != null)
			{
				webrequest.Credentials = credentials;
			}
			
			this.baseAddress = str;
		}

		private void WriteToStream(Stream inStream, Stream outStream,
									long maxBytes) 
		{
			byte[] buf = new byte [4096];
			int read;
			bool checkMax = (maxBytes != -1);
			while((read=inStream.Read(buf, 0 , 4096)) > 0)
			{
				outStream.Write(buf,0,read);
				maxBytes -= read;
				if(checkMax && maxBytes <= 0)
				{
					break;	
				}
			}
		}

		public byte[] DownloadData(String address)
		{
			Stream stream = OpenRead(address);
			MemoryStream memory = new MemoryStream();
			WriteToStream(stream, memory,webresponse.ContentLength);
			byte[] retval = new byte[memory.Length];
			Array.Copy(memory.GetBuffer(),retval,retval.Length);
			memory.Close();
			return retval;
		}

		public void DownloadFile(String address, String fileName)
		{
			Stream stream = OpenRead(address);
			FileStream file = File.OpenWrite(fileName);
			WriteToStream(stream, file, webresponse.ContentLength);
			file.Close();
		}


		public Stream OpenRead(String address)
		{
			CreateWebrequest(address);
			webresponse = webrequest.GetResponse();
			return webresponse.GetResponseStream();
		}

		public Stream OpenWrite(String address)
		{
			return OpenWrite(address, "POST");
		}

		public Stream OpenWrite(String address, String method)
		{
			CreateWebrequest(address);
			webrequest.Method = method;
			return webrequest.GetRequestStream();
		}
	
		public byte[] UploadData(String address, byte[] data)
		{
			return UploadData(address, "POST", data);
		}

		public byte[] UploadData(String address, String method, byte[] data)
		{
			CreateWebrequest(address);
			webrequest.Method = method;
			webrequest.ContentLength = data.Length;
			webrequest.ContentType = defaultContentType;
			Stream stream = webrequest.GetRequestStream();
			stream.Write(data,0,data.Length);
			webresponse = webrequest.GetResponse();
			MemoryStream memory = new MemoryStream(1024);
			WriteToStream(webresponse.GetResponseStream(),memory, 
								webresponse.ContentLength);
			byte [] retval = new byte[memory.Length];
			Array.Copy(memory.GetBuffer(), retval, retval.Length);
			memory.Close();
			return retval;
		}

		public byte[] UploadFile(String address, String fileName)
		{
			return UploadFile(address, "POST", fileName);
		}
		/* Note: refer rfc2068.txt for more info */
		public byte[] UploadFile(String address, String method, String fileName)
		{
			String name=Path.GetFileName(fileName);
			/* Some inane boundary not likely in a data stream generally */
			String mimeBoundary = 
							"--DotGNU--Portable--message--"+
							DateTime.Now.Ticks.ToString("X");
			byte [] boundary = 
					Encoding.ASCII.GetBytes("\r\n--" + mimeBoundary + "\r\n");

			String headerString = 
			"--"+boundary + "\r\n" +
			"Content-Disposition: form-data; name=\"file\";filename\""+name+"\""+"\r\n"
			+ "\r\n";

			byte [] header = Encoding.UTF8.GetBytes(headerString);

			CreateWebrequest(address);
			webrequest.Method = method;
			webrequest.ContentType = fileContentType + "; " +
												 "boundary="+mimeBoundary;
			Stream file = File.OpenRead(fileName);
			try
			{
				webrequest.ContentLength = file.Length;
				webrequest.ContentLength += header.Length+boundary.Length; 
			}
			catch
			{
			}
			Stream stream = webrequest.GetRequestStream();
			
			// Send in the mime header
			stream.Write(header,0,header.Length);
			WriteToStream(file,stream,-1);
			// Send in the boundary
			stream.Write(boundary,0,boundary.Length);
			
			webresponse = webrequest.GetResponse();
			MemoryStream memory = new MemoryStream(1024);
			WriteToStream(webresponse.GetResponseStream(),memory, 
							webresponse.ContentLength);
			byte [] retval = new byte[memory.Length];
			Array.Copy(memory.GetBuffer(), retval, retval.Length);
			memory.Close();
			return retval;
		}

		[TODO]
		public byte[] UploadValues(String address, NameValueCollection data)
		{
			throw new NotImplementedException("UploadValues");
		}

		[TODO]
		public byte[] UploadValues(String address, String method, NameValueCollection data)
		{
			throw new NotImplementedException("UploadValues");
		}

		public String BaseAddress 
		{
 			get
			{
				return this.baseAddress;
			}
			set
			{
				this.baseAddress=value;
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

		public WebHeaderCollection Headers 
		{
 			get
			{
				if(webrequest!=null)
				{
					return webrequest.Headers;
				}
				throw new WebException("Headers not available");
			}
			set
			{
				if(webrequest!=null)
				{
					webrequest.Headers = value;
				}
				throw new WebException("Headers not available");
			}
		}

		[TODO]
		public NameValueCollection QueryString 
		{
 			get
			{
				throw new NotImplementedException("QueryString");
			}
			set
			{
				throw new NotImplementedException("QueryString");
			}
		}

		public WebHeaderCollection ResponseHeaders 
		{
 			get
			{
				if(webresponse!=null)
				{
					return webresponse.Headers;
				}
				throw new WebException("Headers not available");
			}
		}
	}
}//namespace
