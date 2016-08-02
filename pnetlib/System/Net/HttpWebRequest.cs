/*
 * HttpWebRequest.cs - Implementation of the "System.Net.HttpWebRequest" class.
 *
 * Copyright (C) 2002 FSF India
 * 
 * Author: Gopal.V
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


/*
 * Please refer RFC-2616 for more info on HTTP/1.1 Protocol.
 *
 */
namespace System.Net
{

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using DotGNU.SSL;
using System.Security.Cryptography.X509Certificates;

public class HttpWebRequest : WebRequest
{
	private String accept="*/*";
	private Uri address=null;
	private Uri originalUri=null;
	private bool allowAutoRedirect=true;
	private bool allowWriteStreamBuffering=true;
	private String connectionGroupName=null;
	private long contentLength=-1;
	private HttpContinueDelegate continueDelegate=null;
	private ICredentials credentials=null;
	private bool haveResponse=false;
	private WebHeaderCollection headers=new WebHeaderCollection();
	private DateTime ifModifiedSince=DateTime.Now;
	private bool keepAlive=true;
	private int maximumAutomaticRedirections=5;
	private string method="GET";
	private bool pipelined=true;
	private bool preAuthenticate=false;
	private Version protocolVersion=System.Net.HttpVersion.Version11;
	private IWebProxy proxy;
	private Uri requestUri;
	private bool sendChunked=false;
	private ServicePoint servicePoint=null;
	private int timeout;
	private string mediaType=null;
	private bool isSecured=false;

// other useful variables
	protected bool headerSent=false; 
	// so that it is accessible to the nested classes
	private Stream outStream=null;
	private WebResponse response=null;
	private const String format="ddd, dd MMM yyyy HH:mm:ss 'GMT'zz";//HTTP

	private HttpController controller=null;

	internal HttpWebRequest(Uri uri)
	{
		this.originalUri=uri;
		this.method="GET";
		this.headers.SetInternal ("User-Agent","DotGNU Portable.net");
		SetAddress(uri);
	}

	internal void SetAddress(Uri uri)
	{
		CheckHeadersSent(); /* I know this never gets called , but better safe than sorry */
		this.address=uri;
		this.isSecured=String.Equals(uri.Scheme,Uri.UriSchemeHttps);
		this.headers.SetInternal ("Host", uri.Authority);
		this.headers.SetInternal ("Date", DateTime.Now.ToUniversalTime().ToString(format));
	}

	[TODO]
	public override void Abort()
	{
		if(outStream!=null) 
		{
			outStream.Close();
			outStream=null;
		}
	}
	
	public void AddRange(int from, int to)
	{
		AddRange("bytes",from,to);
	}

	public void AddRange(int range)
	{
		AddRange("bytes",range);
	}

	//need internationalisation
	public void AddRange(string rangeSpecifier,int from,int to)
	{
		if(from <0) 
			throw new ArgumentOutOfRangeException("from",S._("Arg_OutOfRange"));
		if(from > to)
			throw new ArgumentOutOfRangeException("from > to ",
				S._("Arg_OutOfRange"));
		if(rangeSpecifier == null)
			throw new ArgumentNullException("rangeSpecifier");

		if(this.Method != null && ! this.Method.Equals("GET"))
			throw new InvalidOperationException("Invalid Method");
		this.Headers.Set("Range",rangeSpecifier+"="+from+"-"+to);
	}

	public void AddRange(string rangeSpecifier, int from)
	{
		if(from <0) 
			throw new ArgumentOutOfRangeException("from",S._("Arg_OutOfRange"));
		if(rangeSpecifier == null)
			throw new ArgumentNullException("rangeSpecifier");
		if(this.Method != null && ! this.Method.Equals("GET"))
			throw new InvalidOperationException("Invalid Method");
		this.Headers.Set("Range",rangeSpecifier+"="+from+"-");
	}
	
	[TODO]
	//unclear about how to go about this
	public override IAsyncResult BeginGetRequestStream(AsyncCallback callback,
													   object state)
	{
		return null;
	}

	[TODO]
	//same problem here
	public override IAsyncResult BeginGetResponse(AsyncCallback callback,
												  object state)
	{
		return null;
	}
	
	[TODO]
	//how can I use IAsyncResult.AsyncWaitHandle
	public override Stream EndGetRequestStream(IAsyncResult asyncResult)
	{
		object state=asyncResult.AsyncState;	
		return null;
	}
	
	[TODO]
	public override WebResponse EndGetResponse(IAsyncResult asyncResult)
	{
		object state=asyncResult.AsyncState;
		return null;
	}

	[TODO]
	public override int GetHashCode()
	{
		return 0;//just to make it compiling
	}
	
	// put in exception handling 
	// and proxy support 
	public override Stream GetRequestStream()
	{
		if(!canGetRequestStream())
			throw new WebException(" not allowed for " + this.Method);
		if(outStream==null)
		{
			if(preAuthenticate) AddHttpAuthHeaders(null);
			outStream=new HttpStream(this, contentLength < 0);
		}
		return outStream;
	}
	
	//need some doubts cleared
	public override WebResponse GetResponse()
	{
		HttpWebResponse httpResponse;
		WebRequest newRequest;

		if(response!=null) return response;
		
		if(outStream==null)
		{
			if(preAuthenticate) AddHttpAuthHeaders(null);
			outStream=new HttpStream(this, false);
			outStream.Flush();
			// which is the response stream as well 
		}

		httpResponse=new HttpWebResponse(this,this.outStream);
		this.response=httpResponse;

		/* here it always is a HttpWebResponse , need to use a Factory actually 
		 * When that happens, change the design */
		
		newRequest=Controller.Recurse(this, httpResponse);
		
		/* this is the tricky recursion thing */ 
		this.response=newRequest.GetResponse(); 
		
		this.haveResponse=true; // I hope this is correct
		if(outStream!=null)
		{
			(outStream as HttpStream).ContentLength=response.ContentLength;
		}
		return this.response; 
	}

/*
 * Implement the Checks for Setting values
 */
	public string Accept 
	{
		get
		{
			return headers["Accept"];
		}
		set
		{
			CheckHeadersSent();
			headers.SetInternal("Accept",value);
		}
	}
	
	public Uri Address 
	{
		get
		{
			return this.address;	
		}
	}

	public bool AllowAutoRedirect 
	{
		get
		{
			return this.allowAutoRedirect;
		}
		
		set
		{
			this.allowAutoRedirect=value;
		}
	}

	public bool AllowWriteStreamBuffering 
	{
		get 
		{
			return this.allowWriteStreamBuffering;	
		}
		set
		{
			this.allowWriteStreamBuffering=value;
		}
	}

	public string Connection 
	{
		get
		{
			return headers["Connection"];
		}
		set
		{
			CheckHeadersSent();
			String str=null;
			if(value!=null)str=value.ToLower().Trim();
			if(str==null || str.Length==0)
			{
				headers.RemoveInternal("Connection");
				return;
			}
			if(str=="keep-alive" || str=="close")
			{
				headers.SetInternal("Connecton",str);
			}
			else throw new ArgumentException("Value NOT keep-alive or close");
		}
	}

	public override string ConnectionGroupName 
	{
		get
		{
			return this.connectionGroupName;
		}
		set
		{
			this.connectionGroupName=value;
		}
	}

	public override long ContentLength 
	{
		get
		{
			return this.contentLength;
		}
		set
		{
			if(value<0) 
				throw new ArgumentOutOfRangeException("Content-Length < 0");
			CheckHeadersSent();
			this.contentLength=value;
			this.headers.SetInternal("Content-Length",value.ToString());
		}
	}

	public override string ContentType 
	{
		get
		{
			return headers["Content-Type"];
		}
		set
		{
			CheckHeadersSent();
			this.headers.SetInternal("Content-Type",value);
			// I should really check for <type>/<subtype> in it ;)
		}
	}

	public HttpContinueDelegate ContinueDelegate 
	{
		get
		{
			return this.continueDelegate;
		}
		set
		{
			this.continueDelegate=value;
		}
	}

	public override ICredentials Credentials 
	{
		get
		{
			return this.credentials;
		}
		set
		{
			this.credentials=value;
		}
	}
	
#if CONFIG_X509_CERTIFICATES
	[TODO]
	public X509CertificateCollection ClientCertificates 
	{
 		get
		{
			throw new NotImplementedException("ClientCertificates");
		}
	}
#endif // CONFIG_X509_CERTIFICATES
	
#if !ECMA_COMPAT
	[TODO]
	public System.Net.CookieContainer CookieContainer 
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

	public string Expect 
	{
		get
		{
			return headers["Expect"];
		} 
		set
		{
			CheckHeadersSent();
			if(value.ToLower()=="100-continue") 
				throw new ArgumentException("cannot set 100-continue");
			headers.SetInternal("Expect",value);
		}
	}

	public bool HaveResponse 
	{
		get
		{
			return this.haveResponse;
		}
	}

	public override WebHeaderCollection Headers 
	{
		get
		{
			return this.headers;
		} 
		set
		{
			CheckHeadersSent();
			this.headers=value;
		}
	}

	public DateTime IfModifiedSince 
	{
		/* avoid the thunk of Headers */
		get
		{
			return this.ifModifiedSince;
		} 
		set
		{
			CheckHeadersSent();
			headers.SetInternal("IfModifiedSince", value.ToString(format));
			this.ifModifiedSince=value;
		}
	}

	public bool KeepAlive 
	{
		get
		{
			return this.keepAlive;
		} 
		set
		{
			CheckHeadersSent();
			this.keepAlive=value;
		}
	}

	public int MaximumAutomaticRedirections 
	{
		get
		{
			return this.maximumAutomaticRedirections;
		} 
		set
		{
			if(value<=0)
				throw new ArgumentException("redirections <= 0");
			this.maximumAutomaticRedirections=value;
		}
	}

	public string MediaType 
	{
		get
		{
			return this.mediaType;
		} 
		set
		{
			CheckHeadersSent();
			this.mediaType=value;
		}
	}

	public override string Method 
	{
		get
		{
			return this.method;
		} 
		set
		{
			CheckHeadersSent();
			if(isHTTPMethod(value))
				this.method=value;
			else 
				throw new ArgumentException("invalid method");
		}
	}

	public bool Pipelined 
	{
		get
		{
			return this.pipelined;
		} 
		set
		{
			CheckHeadersSent();
			this.pipelined=value;
		}
	}

	public override bool PreAuthenticate
	{
		get
		{
			return this.preAuthenticate;
		} 
		set
		{
			this.preAuthenticate=value;
		}
	}

	public Version ProtocolVersion 
	{
		get
		{
			return this.protocolVersion;
		} 
		set
		{
			CheckHeadersSent();
			if(value.Major==1 && (value.Minor==0 || value.Minor==1))
			{
				this.protocolVersion=value;
			}
			else 
				throw new ArgumentException("Wrong args");
		}
	}

	public override IWebProxy Proxy 
	{
		get
		{
			if(this.proxy==null)
			{
				this.proxy=GlobalProxySelection.Select;
			}
			return this.proxy;
		} 
		set
		{
			if(value==null)throw new ArgumentNullException("Proxy null");
			CheckHeadersSent();
			//TODO: implement SecurityException
			this.proxy=value;
		}
	}

	public string Referer 
	{
		get
		{
			return headers["Referer"];
		} 
		set
		{
			CheckHeadersSent();
			headers.SetInternal("Referer",value);
		}
	}

	public override Uri RequestUri 
	{
		get
		{
			return this.requestUri;
		}
	}

	public bool SendChunked 
	{
		get
		{
			return this.sendChunked;
		} 
		set
		{
			CheckHeadersSent();
			this.sendChunked=value;
		}
	}

	public ServicePoint ServicePoint 
	{
		get
		{
			return this.servicePoint;
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
			if(this.timeout<0 && this.timeout!=
			System.Threading.Timeout.Infinite)
				throw new ArgumentOutOfRangeException("timeout < 0");
			this.timeout=value;
		}
	}

	public string TransferEncoding 
	{
		get
		{
			return headers["Transfer-Encoding"];
		} 
		set
		{
			CheckHeadersSent();
			if(!this.sendChunked)throw new 
				InvalidOperationException(" send chunked set");
			if(String.Compare("Chunked",value,true) == 0)
				throw new ArgumentException(" cannot chunk it");
			headers.SetInternal("Transfer-Encoding",value);
		}
	}

	public string UserAgent 
	{
		get
		{
			return headers["User-Agent"];
		} 
		set
		{
			headers.SetInternal("User-Agent",value);
		}
	}
	

	private void CheckHeadersSent()
	{
		if(headerSent)
		{
			throw new InvalidOperationException("Headers already sent");
		}
	}
	private bool canGetRequestStream()
	{
		if((this.method.Equals("PUT")) || (this.method.Equals("POST")))
			return true;
		return false;
	}
	private bool isHTTPMethod(String val)
	{
		if(val==null)return false;
		if(val=="GET" || val=="HEAD" || val=="POST" || val=="PUT" ||
			val=="DELETE" || val=="TRACE" || val=="OPTIONS")
			{
				return true;
			}
		return false;
	}

	internal bool AddHttpAuthHeaders(String challenge)
	{
		ICredentials cred;
		String challengeType="Basic"; /* default */
		Authorization auth;
		
		if(credentials==null) return false;
		if(challenge==null && !preAuthenticate) 
		{
			return false; /* TODO : throw an exception here ? */
		}
		else if (challenge!=null)
		{
			int len=challenge.IndexOf(' ');
			challengeType=( len==-1) ? challenge : 
						challenge.Substring(0,len);
		}
		cred=credentials.GetCredential(this.Address,challengeType);
		if(cred==null)
		{
			return false; /* TODO : throw an exception here ? */
		}

		if(preAuthenticate)
		{
			auth=AuthenticationManager.PreAuthenticate(this, 
							cred);
		}
		else
		{
			auth=AuthenticationManager.Authenticate(challenge,
							this, cred);
		}

		if(auth==null)
		{
			return false; /* TODO : throw an exception here ? */
		}
		this.Headers["Authorization"]=auth.Message;
		return true;
	}

	internal bool AddProxyAuthHeaders(String challenge)
	{
		ICredentials cred;
		String challengeType="Basic"; /* default */
		Authorization auth;
		
		if(proxy==null) return false;
		
		if(proxy.Credentials==null) return false;

		if(challenge==null)
		{
			throw new ArgumentNullException("challenge");
		}
		else
		{
			int len=challenge.IndexOf(' ');
			challengeType=( len==-1) ? challenge : 
						challenge.Substring(0,len);
		}

		cred=proxy.Credentials.GetCredential(this.Address,challengeType);
		
		if(cred==null)
		{
			return false; /* TODO : throw an exception here ? */
		}

		/* I would have added a PreAuthenticate option for this as well 
		 * But MS or ECMA doesn't . So I won't -- Gopal */

		auth=AuthenticationManager.Authenticate(challenge,
							this, cred);
		if(auth==null)
		{
			return false; /* TODO : throw an exception here ? */
		}

		this.Headers["Proxy-Authorization"]=auth.Message;
		return true;
	}

	/* Re-use an HTTP Webrequest 
	 * Note: use with great care ,might be buggy -- Gopal 
	 * The basic assumption is that this WebRequest and the stream is of no further
	 * use. So beware , content will be lost for the last call .
	 * However the response will be retained if it is an incomplete Reset*/
	internal void ResetRequest(bool complete) 
	{
		if(outStream!=null) 
		{
			/* theoretically I should use the same KeepAlive stream for subsequent requests
			 * but the HttpStream needs a bit of hacking in that case */
			outStream.Close();
		}
		headerSent=false;
		if(complete)
		{
			response=null;
		}
		outStream=null;
	}

	private HttpController Controller
	{
			get
			{
					if(controller==null) controller=new HttpController(this);
					return this.controller;
			}
	}

	/* Note: the next cycle of refactoring should split this single
	 * stream class into two http streams . Current hack works, but
	 * only *just* */
	private class HttpStream : Stream
	{	
		private HttpWebRequest request;
		private Stream underlying=null;
		private long contentLength=Int64.MaxValue;
		private Stream netStream;

		public HttpStream(HttpWebRequest req, bool computeContentLength)
			: this(req, HttpStream.OpenStream(req), computeContentLength)
		{
		}

		public HttpStream(HttpWebRequest req, Stream underlying,
						  bool computeContentLength)
		{
			this.request=req;
			this.underlying=underlying;

			// If sendHeaderOnClose is set, then we use memory stream as
			// underlying stream so that we can compute content length.
			if(computeContentLength)
			{
				netStream = underlying;
				this.underlying = new MemoryStream();
			}
			else
			{
				SendHeaders();
			}
		}

		~HttpStream()
		{
			if(underlying!=null)
			{
				underlying.Close();
				underlying=null;
			}
		}

		// Stub out all stream functionality.
		public override void Flush() 
		{
			underlying.Flush();
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(contentLength<=0) return -1;
			int retval = underlying.Read(buffer, offset, count);
			contentLength-=retval;
			return retval;
		}
		
		public override int ReadByte() 
		{
			if(contentLength<=0) return -1;
			int retval = underlying.ReadByte();
			contentLength-=1;
			return retval;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return underlying.Seek(offset, origin);
		}
		
		public override void SetLength(long value)
		{
			underlying.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			underlying.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value) 
		{
			underlying.WriteByte(value);
		}

		public override void Close()
		{
			underlying.Flush();

			MemoryStream ms = underlying as MemoryStream;
			if(ms == null)
			{
				return;
			}

			// Update content length and send reqest over network.
			request.ContentLength = ms.Length;
			underlying = netStream;
			SendHeaders();
			ms.WriteTo(netStream);
		}

		public override bool CanRead 
		{
			get 
			{
				return underlying.CanRead;
			} 
		}

		public override bool CanSeek 
		{
			get 
			{
				return underlying.CanSeek; 
			} 
		}

		public override bool CanWrite 
		{
			get 
			{
				return underlying.CanWrite;
			}
		}
		
		public override long Length
		{
			get
			{
				return underlying.Length;
			}
		}

		public override long Position
		{
			get
			{
				return underlying.Position;
			}
			set
			{
				underlying.Position = value;
			}
		}

		public long ContentLength
		{
			set
			{
				if(value<=0)
				{
					contentLength=Int64.MaxValue;
				}
				else
				{
					contentLength=value;
				}
			}
		}

		private static Socket OpenSocket(HttpWebRequest req)
		{
			IPAddress ip=null;
			Uri nextHop=(req.Proxy!=null) ?
			       	req.Proxy.GetProxy(req.Address) : req.Address;
			
			if(nextHop.HostNameType == UriHostNameType.Dns)
			{
				ip=Dns.Resolve(nextHop.Host).AddressList[0];
			}
			else if(nextHop.HostNameType == UriHostNameType.IPv4 ||
					nextHop.HostNameType == UriHostNameType.IPv6)
			{
				ip=IPAddress.Parse(nextHop.Host);
			}
			IPEndPoint ep = new IPEndPoint(ip,nextHop.Port);
			Socket server=new 
					Socket(ip.AddressFamily, SocketType.Stream,
							ProtocolType.Tcp);
			server.Connect(ep);
			if(req.isSecured)
			{
				ProxyConnect(server, req);
			}
			return server;
		}

		private static void ProxyConnect(Socket sock, HttpWebRequest req)
		{
			// This should send an HTTP CONNECT + auth + KeepAlive 
			// read the response 200 header , wait for blank line
			// and jump back out
			SendConnect(sock, req);
			if(!GetConnection(sock, req))
			{
				throw new WebException("Proxy refused connection"); 
			}
		}

		private static void SendConnect(Socket sock, HttpWebRequest req)
		{
			// This stream does not own the socket
			Stream stream = new NetworkStream(sock, false);
			StreamWriter writer = new StreamWriter(stream);
			String connectString = null;
			String authHeader = null;
			
			if(req.Proxy.Credentials != null)
			{
				// TODO : remove this from the SSL req
				req.AddProxyAuthHeaders("Basic");
			}		
						
			connectString= 	"CONNECT "+	req.Address.Host + 
							":" + req.Address.Port +
							" HTTP/"+req.protocolVersion.Major+
							"."+req.protocolVersion.Minor+"\r\n";

			authHeader = 	"Proxy-Authorization: " + 
							req.Headers["Proxy-Authorization"] + "\r\n"; 

			writer.Write(connectString);
			writer.Write(authHeader);
			writer.Write("\r\n");// terminating CRLF
			writer.Flush();
			writer.Close();
			// stream is automatically closed , but the socket is
			// still alive
		}

		private static bool GetConnection(Socket sock, HttpWebRequest req)
		{
			NetworkStream stream = new NetworkStream(sock, false);
			String response = ReadLine(stream);
			if(response != null && 	
				response.Substring("HTTP/1.0 ".Length,3) == "200")
			{
				while((response = ReadLine(stream)) != null && 
						response != String.Empty)
				{
					// eat up any headers...
				}
				stream.Close(); // close stream, don't kill socket
				return true;
			}
			else
			{
				stream.Close(); // close stream, don't kill socket
				return false;
			}
		}

		
		private static String ReadLine(Stream stream)
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

		private static Stream OpenStream(HttpWebRequest req)
		{
			Socket sock=OpenSocket(req);
			if(req.isSecured)
			{
#if CONFIG_SSL
				SecureConnection secured=new SecureConnection();
				/*TODO: suspected memory hold for SecureConnection */
				Stream retval=secured.OpenStream(sock);
				return retval;
#else
				throw new NotSupportedException(S._("NotSupp_SSL"));
#endif
			}
			else
			{
				return new NetworkStream(sock,true);
			}
		}

		private void SendHeaders()
		{
			StreamWriter writer=new StreamWriter(this);
			request.headerSent=true; 
			/* fake it before sending to allow for atomicity */
			String requestString= null;
			if(request.Proxy!=null && !request.isSecured)
			{
				if(request.Proxy.Credentials != null &&
					request.Method != "GET" && 
					request.Method != "HEAD")
				{
					request.AddProxyAuthHeaders("Basic");
				}		
						
				requestString= request.Method+" "+
					request.Address+
					" HTTP/"+request.protocolVersion.Major+
					"."+request.protocolVersion.Minor+"\r\n";
			}
			else
			{
				requestString= request.Method+" "+
					request.Address.PathAndQuery+
					" HTTP/"+request.protocolVersion.Major+
					"."+request.protocolVersion.Minor+"\r\n";
			}
			writer.Write(requestString);
			writer.Write(request.Headers.ToString());
			writer.Write("\r\n");// terminating CRLF
			writer.Flush();
		}
	} //internal class

#if CONFIG_SSL
	private class SecureConnection: IDisposable
	{
		ISecureSession session;
		ISecureSessionProvider provider;

		public SecureConnection()
		{
			provider = SessionProviderFactory.GetProvider();
			session = provider.CreateClientSession(Protocol.AutoDetect);
		}
		
		public Stream OpenStream(Socket sock)
		{
			return session.PerformHandshake(sock);
		}

		public void Dispose()
		{
			if(session!=null)
			{
				session.Dispose();
				session=null;
			}
		}
		~SecureConnection()
		{
			Dispose();
		}
	}
#endif

	/* this class forms the controller of the http life cycle
	 * of OK,Forbiddens and redirects */
	private class HttpController
	{
		/* I know it's a bad practice to have seperate states for each thing
		 * but I'll fix it later :-) */
		private enum HttpAuthState
		{
			NoAuth,
			Trying,
			OK,
			Failed
		}
		
		private HttpAuthState proxy;
		private HttpAuthState http;
		private int redirections;

		public HttpController(HttpWebRequest request)
		{
			if(request.PreAuthenticate)
			{
				this.http=HttpAuthState.Trying;
			}
			else
			{
				this.http=HttpAuthState.NoAuth;
			}
			this.proxy=HttpAuthState.NoAuth;
			redirections=0;
		}
		/* returns this WebRequest or a new one.
		 * If nothing can be done, it will return the same 'ol 
		 * request.
		 * Note: will change substantially soon*/
		public WebRequest Recurse(HttpWebRequest request,HttpWebResponse response)
		{
			HttpStatusCode code=response.StatusCode;
			switch(code)
			{
				case HttpStatusCode.Continue:
				{
					// Drop this response so that real one can be read.
					request.response = null;
					return request;
				}

				case HttpStatusCode.OK:
				{
					if(http==HttpAuthState.Trying) http=HttpAuthState.OK;
					if(proxy==HttpAuthState.Trying) proxy=HttpAuthState.OK;
					return request;
				}
				break; /* never reached */
				
				case HttpStatusCode.ProxyAuthenticationRequired:
				{
						String challenge=null;

						if(proxy==HttpAuthState.Trying || proxy==HttpAuthState.Failed) 
						{
							proxy=HttpAuthState.Failed;
							return request;
						}
						challenge=response.Headers["Proxy-Authenticate"];
						request.ResetRequest(false);
						if(request.AddProxyAuthHeaders(challenge))
						{
							request.ResetRequest(true);
							proxy=HttpAuthState.Trying;
							return request;
						}
						else
						{
							proxy=HttpAuthState.Failed;
							return request;
						}
				}
				break;

				case HttpStatusCode.Forbidden:
				{
						String challenge=null;

						if(http==HttpAuthState.Trying || http==HttpAuthState.Failed) 
						{
							http=HttpAuthState.Failed;
							return request;
						}
						challenge=response.Headers["WWW-Authenticate"];
						request.ResetRequest(false);
						if(request.AddHttpAuthHeaders(challenge))
						{
							request.ResetRequest(true);
							http=HttpAuthState.Trying;
							return request;
						}
						else
						{
							http=HttpAuthState.Failed;
							return request;
						}
				}
				break;

				case HttpStatusCode.Redirect:
				case HttpStatusCode.Moved :
				case HttpStatusCode.MultipleChoices :
				case HttpStatusCode.SeeOther :
				case HttpStatusCode.TemporaryRedirect:
				/* case HttpStatusCode.Found : // Duplicate */
				/* case HttpStatusCode.MovedPermanently : // Duplicate */
				{
						Uri newUri=response.ResponseUri;
						if(request.allowAutoRedirect &&	newUri!=null &&
							redirections<request.MaximumAutomaticRedirections &&
							(request.Method=="GET" || request.Method=="HEAD"))
						{
							/* ok redirect it */
							request.ResetRequest(true);
							request.SetAddress(newUri);
							redirections++;
							return request;
						}
				}
				break;

			}
			
			return request;
		}

		public override String ToString()
		{
				return "(Proxy="+proxy.ToString()+",Http="+http.ToString()+")";
		}
	}
}//class

}//namespace

