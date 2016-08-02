/*
 * OpenSSL.cs - Implementation of the "DotGNU.SSL.OpenSSL" class.
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

namespace DotGNU.SSL
{

#if CONFIG_RUNTIME_INFRA

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using OpenSystem.Platform;

// Secure session provider object that uses the OpenSSL library to
// provide the underlying security functionality.

internal sealed class OpenSSL : ISecureSessionProvider
{
	// Internal state.
	private static bool initialized = false;

	// Constructor.
	public OpenSSL()
			{
				// Make sure that the OpenSSL library is initialized.
				lock(typeof(OpenSSL))
				{
					if(!initialized)
					{
						try
						{
							SSL_load_error_strings();
							SSL_library_init();
						}
						catch(Exception)
						{
							// Could not find the functions to execute,
							// so we probably don't have OpenSSL on this
							// system or it isn't on the LD_LIBRARY_PATH.
							throw new NotSupportedException();
						}
						initialized = true;
					}
				}
			}

	// Implement the ISecureSessionProvider interface.
	public ISecureSession CreateClientSession(Protocol protocol)
			{
				IntPtr method;
				IntPtr ctx;

				// Get the method handler for the protocol.
				try
				{
					switch(protocol)
					{
						case Protocol.AutoDetect:
						{
							method = SSLv23_client_method();
						}
						break;
	
						case Protocol.SSLv2:
						{
							method = SSLv2_client_method();
						}
						break;
	
						case Protocol.SSLv3:
						{
							method = SSLv3_client_method();
						}
						break;
	
						case Protocol.TLSv1:
						{
							method = TLSv1_client_method();
						}
						break;

						default:
						{
							method = IntPtr.Zero;
						}
						break;
					}
				}
				catch(NotImplementedException)
				{
					// The entry point does not exist in the library.
					method = IntPtr.Zero;
				}
				if(method == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}

				// Create the OpenSSL context for the session.
				ctx = SSL_CTX_new(method);
				if(ctx == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}

				// Wrap the context object and return it.
				return new OpenSSLSession(ctx, true);
			}
	public ISecureSession CreateServerSession(Protocol protocol)
			{
				IntPtr method;
				IntPtr ctx;

				// Get the method handler for the protocol.
				try
				{
					switch(protocol)
					{
						case Protocol.AutoDetect:
						{
							method = SSLv23_server_method();
						}
						break;
	
						case Protocol.SSLv2:
						{
							method = SSLv2_server_method();
						}
						break;
	
						case Protocol.SSLv3:
						{
							method = SSLv3_server_method();
						}
						break;
	
						case Protocol.TLSv1:
						{
							method = TLSv1_server_method();
						}
						break;

						default:
						{
							method = IntPtr.Zero;
						}
						break;
					}
				}
				catch(NotImplementedException)
				{
					// The entry point does not exist in the library.
					method = IntPtr.Zero;
				}
				if(method == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}

				// Create the OpenSSL context for the session.
				ctx = SSL_CTX_new(method);
				if(ctx == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}

				// Wrap the context object and return it.
				return new OpenSSLSession(ctx, false);
			}

	// Session handling object for OpenSSL.
	private sealed class OpenSSLSession : ISecureSession, IDisposable
	{
		// Internal state.
		private IntPtr ctx;
		private bool isClient;
		private byte[] certificate;
		private byte[] privateKey;
		private byte[] remoteCertificate;
		private OpenSSLStream stream;

		// Constructor.
		public OpenSSLSession(IntPtr ctx, bool isClient)
				{
					this.ctx = ctx;
					this.isClient = isClient;
				}

		// Destructor.
		~OpenSSLSession()
				{
					Dispose();
				}

		// Implement the ISecureSession interface.
		public bool IsClient
				{
					get
					{
						return isClient;
					}
				}
		public byte[] Certificate
				{
					get
					{
						return certificate;
					}
					set
					{
						lock(this)
						{
							if(value == null)
							{
								throw new ArgumentNullException("value");
							}
							if(certificate != null)
							{
								throw new InvalidOperationException();
							}
							if(ctx == IntPtr.Zero)
							{
								throw new ObjectDisposedException("session");
							}
							certificate = value;
							if(SSL_CTX_use_certificate_ASN1
									(ctx, value, (Int)value.Length) == 0)
							{
								throw new ArgumentException();
							}
						}
					}
				}
		public byte[] PrivateKey
				{
					get
					{
						return privateKey;
					}
					set
					{
						lock(this)
						{
							if(value == null)
							{
								throw new ArgumentNullException("value");
							}
							if(privateKey != null)
							{
								throw new InvalidOperationException();
							}
							if(ctx == IntPtr.Zero)
							{
								throw new ObjectDisposedException("session");
							}
							privateKey = value;
							if(SSL_CTX_use_RSAPrivateKey_ASN1
									(ctx, value, (Int)value.Length) == 0)
							{
								throw new ArgumentException();
							}
						}
					}
				}
		public byte[] RemoteCertificate
				{
					get
					{
						return remoteCertificate;
					}
				}
		public Stream PerformHandshake(Object socket)
				{
					lock(this)
					{
						// Validate the parameter and state.
						if(socket == null)
						{
							throw new ArgumentNullException("socket");
						}
						int fd = Utils.GetSocketFd(socket);
						if(fd == -1)
						{
							throw new ArgumentException();
						}
						if(stream != null)
						{
							throw new InvalidOperationException();
						}
						if(ctx == IntPtr.Zero)
						{
							throw new ObjectDisposedException("session");
						}

						// Create the SSL session control object.
						IntPtr ssl = SSL_new(ctx);
						if(ssl == IntPtr.Zero)
						{
							throw new NotSupportedException();
						}

						// Create a socket BIO object and set it.
						IntPtr bio = BIO_new_socket((Int)fd, (Int)0);
						if(bio == IntPtr.Zero)
						{
							SSL_free(ssl);
							throw new NotSupportedException();
						}
						SSL_set_bio(ssl, bio, bio);

						// Attempt to connect or accept.
						int result;
						if(isClient)
						{
							result = (int)SSL_connect(ssl);
						}
						else
						{
							result = (int)SSL_accept(ssl);
						}
						if(result != 1)
						{
							SSL_free(ssl);
							throw new SecurityException();
						}

						// Get the remote certificate and record it.
						IntPtr x509 = SSL_get_peer_certificate(ssl); 
						if(x509 != IntPtr.Zero)
						{
							int length = (int)i2d_X509(x509, IntPtr.Zero);
							if(length > 0)
							{
								IntPtr data = Marshal.AllocHGlobal(length);
								if(data != IntPtr.Zero)
								{
									IntPtr temp = data;
									i2d_X509(x509, ref temp);
									remoteCertificate = new byte [length];
									Marshal.Copy(data, remoteCertificate,
												 0, length);
									Marshal.FreeHGlobal(data);
								}
							}
							X509_free(x509);
						}

						// Create the stream object and return it.
						stream = new OpenSSLStream(ssl);
						return stream;
					}
				}
		public Stream SecureStream
				{
					get
					{
						return stream;
					}
				}

		// Implement the IDisposable interface.
		public void Dispose()
				{
					lock(this)
					{
						if(ctx != IntPtr.Zero)
						{
							SSL_CTX_free(ctx);
							ctx = IntPtr.Zero;
						}
						if(stream != null)
							stream.Close();
					}
				}

	}; // class OpenSSLSession

	// Stream object for managing a secure connection.
	private sealed class OpenSSLStream : Stream
	{
		// Internal state.
		private IntPtr ssl;

		// Constructor.
		public OpenSSLStream(IntPtr ssl)
				{
					this.ssl = ssl;
				}

		// Destructor.
		~OpenSSLStream()
				{
					Close();
				}

		// Close the stream.
		public override void Close()
				{
					lock(this)
					{
						if(ssl != IntPtr.Zero)
						{
							int result = (int)SSL_shutdown(ssl);
							if(result == 0)
							{
								// Bi-directional shutdown is required.
								result = (int)SSL_shutdown(ssl);
							}
							SSL_free(ssl);
							ssl = IntPtr.Zero;
							if(result < 0)
							{
								// The shutdown failed, probably because
								// there is a security problem with the
								// data that was transmitted.
								throw new SecurityException();
							}
						}
					}
				}

		// Flush the pending contents in this stream.
		public override void Flush()
				{
					// Nothing to do here.
				}

		// Read data from this stream.
		public override int Read(byte[] buffer, int offset, int count)
				{
					Utils.ValidateBuffer(buffer, offset, count);
					lock(this)
					{
						if(ssl == IntPtr.Zero)
						{
							throw new ObjectDisposedException("stream");
						}
						if(offset == 0)
						{
							return (int)SSL_read(ssl, buffer, (Int)count);
						}
						else
						{
							byte[] temp = new byte [count];
							int result = (int)SSL_read(ssl, temp, (Int)count);
							if(result > 0)
							{
								Array.Copy(temp, 0, buffer, offset, result);
							}
							Array.Clear(temp, 0, count);
							return result;
						}
					}
				}

		// Seek to a new position within this stream.
		public override long Seek(long offset, SeekOrigin origin)
				{
					throw new NotSupportedException();
				}

		// Set the length of this stream.
		public override void SetLength(long value)
				{
					throw new NotSupportedException();
				}

		// Write a buffer of bytes to this stream.
		public override void Write(byte[] buffer, int offset, int count)
				{
					Utils.ValidateBuffer(buffer, offset, count);
					lock(this)
					{
						if(ssl == IntPtr.Zero)
						{
							throw new ObjectDisposedException("stream");
						}
						if(offset == 0)
						{
							SSL_write(ssl, buffer, (Int)count);
						}
						else if(count > 0)
						{
							byte[] temp = new byte [count];
							Array.Copy(buffer, offset, temp, 0, count);
							SSL_write(ssl, temp, (Int)count);
							Array.Clear(temp, 0, count);
						}
					}
				}

		// Determine if it is possible to read from this stream.
		public override bool CanRead
				{
					get
					{
						return true;
					}
				}

		// Determine if it is possible to seek within this stream.
		public override bool CanSeek
				{
					get
					{
						return false;
					}
				}

		// Determine if it is possible to write to this stream.
		public override bool CanWrite
				{
					get
					{
						return true;
					}
				}

		// Get the length of this stream.
		public override long Length
				{
					get
					{
						throw new NotSupportedException();
					}
				}

		// Get the current position within the stream.
		public override long Position
				{
					get
					{
						throw new NotSupportedException();
					}
					set
					{
						throw new NotSupportedException();
					}
				}

	}; // class OpenSSLStream

	// Import the functions we need from the OpenSSL library.

	[DllImport("ssl")]
	extern private static Int SSL_library_init();

	[DllImport("ssl")]
	extern private static void SSL_load_error_strings();

	[DllImport("ssl")]
	extern private static IntPtr SSLv2_client_method();

	[DllImport("ssl")]
	extern private static IntPtr SSLv2_server_method();

	[DllImport("ssl")]
	extern private static IntPtr SSLv3_client_method();

	[DllImport("ssl")]
	extern private static IntPtr SSLv3_server_method();

	[DllImport("ssl")]
	extern private static IntPtr TLSv1_client_method();

	[DllImport("ssl")]
	extern private static IntPtr TLSv1_server_method();

	[DllImport("ssl")]
	extern private static IntPtr SSLv23_client_method();

	[DllImport("ssl")]
	extern private static IntPtr SSLv23_server_method();

	[DllImport("ssl")]
	extern private static IntPtr SSL_CTX_new(IntPtr method);

	[DllImport("ssl")]
	extern private static void SSL_CTX_free(IntPtr ctx);

	[DllImport("ssl")]
	extern private static Int SSL_CTX_use_certificate_ASN1
			(IntPtr ctx, byte[] d, Int len);

	[DllImport("ssl")]
	extern private static Int SSL_CTX_use_RSAPrivateKey_ASN1
			(IntPtr ctx, byte[] d, Int len);

	[DllImport("ssl")]
	extern private static IntPtr SSL_new(IntPtr ctx);

	[DllImport("ssl")]
	extern private static void SSL_free(IntPtr ssl);

	[DllImport("ssl")]
	extern private static void SSL_set_bio
				(IntPtr ssl, IntPtr rbio, IntPtr wbio);

	[DllImport("ssl")]
	extern private static Int SSL_connect(IntPtr ssl);

	[DllImport("ssl")]
	extern private static Int SSL_accept(IntPtr ssl);

	[DllImport("ssl")]
	extern private static Int SSL_shutdown(IntPtr ssl);

	[DllImport("ssl")]
	extern private static Int SSL_read(IntPtr ssl, byte[] buf, Int num);

	[DllImport("ssl")]
	extern private static Int SSL_write(IntPtr ssl, byte[] buf, Int num);

	[DllImport("ssl")]
	extern private static IntPtr SSL_get_peer_certificate(IntPtr ssl);

	[DllImport("ssl")]
	extern private static IntPtr BIO_new_socket(Int sock, Int close_flag);

	[DllImport("ssl")]
	extern private static Int i2d_X509(IntPtr x509, IntPtr buf);

	[DllImport("ssl")]
	extern private static Int i2d_X509(IntPtr x509, ref IntPtr buf);

	[DllImport("ssl")]
	extern private static void X509_free(IntPtr x509);

}; // class OpenSSL

#endif // CONFIG_RUNTIME_INFRA

}; // namespace DotGNU.SSL
