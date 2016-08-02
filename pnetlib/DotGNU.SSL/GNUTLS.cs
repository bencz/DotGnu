/*
 * GNUTLS.cs - Implementation of the "DotGNU.SSL.GNUTLS" class.
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

// Secure session provider object that uses the GNUTLS library to
// provide the underlying security functionality.

internal unsafe sealed class GNUTLS : ISecureSessionProvider
{
	// Internal state.
	private static bool initialized = false;

	// Constructor.
	public GNUTLS()
			{
				// Make sure that the GNUTLS library is initialized.
				lock(typeof(GNUTLS))
				{
					if(!initialized)
					{
						try
						{
							gnutls_global_init();
						}
						catch(Exception)
						{
							// Could not find the functions to execute,
							// so we probably don't have GNUTLS on this
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
				return new GNUTLSSession(protocol, true);
			}
	public ISecureSession CreateServerSession(Protocol protocol)
			{
				return new GNUTLSSession(protocol, false);
			}

	// Credentials object for GNUTLS.
	private abstract class GNUTLSCredentials : IDisposable
	{
		// Internal state.
		protected IntPtr cred;
		private bool isDisposed;

		// Constructor.
		protected GNUTLSCredentials() {}

		// Finalizer.
		~GNUTLSCredentials()
		{
			if(isDisposed == false)
			{
				Dispose();
			}
		}

		// Get the credential pointer.
		public IntPtr Cred
				{
					get
					{
						return cred;
					}
				}

		// Get the credential type.
		public abstract int CredType { get; }

		// Dispose of this object.
		public virtual void Dispose()
				{
					cred = IntPtr.Zero;
					isDisposed = true;
				}

	}; // class GNUTLSCredentials

	// Credentials object that is based on a certificate/private key pair.
	private sealed class GNUTLSCertificateCredentials : GNUTLSCredentials
	{
		private bool isDisposed;

		// Constructor.
		public GNUTLSCertificateCredentials(byte[] certificate, byte[] key)
				{
					gnutls_certificate_allocate_credentials(out cred);
					gnutls_datum c = new gnutls_datum(certificate);
					gnutls_datum k = new gnutls_datum(key);
					gnutls_certificate_set_x509_key_mem
						(cred, ref c, ref k, 0 /* GNUTLS_X509_FMT_DER */);
					c.Free();
					k.Free();
				}

		// Finalizer.
		~GNUTLSCertificateCredentials()
				{
					if(isDisposed == false)
					{
						Dispose();
					}
				}

		// Get the credential type.
		public override int CredType
				{
					get
					{
						return 1;	// GNUTLS_CRD_CERTIFICATE
					}
				}

		// Dispose of this object.
		public override void Dispose()
				{
					if(cred != IntPtr.Zero)
					{
						gnutls_certificate_free_credentials(cred);
						base.Dispose();
					}
				}

	}; // class GNUTLSCertificateCredentials

	// Credentials object for anonymous access.
	private sealed class GNUTLSAnonCredentials : GNUTLSCredentials
	{
		private bool isDisposed;

		// Constructor.
		public GNUTLSAnonCredentials()
				{
					gnutls_certificate_allocate_credentials(out cred);
				}

		// Finalizer.
		~GNUTLSAnonCredentials()
				{
					if(isDisposed == false)
					{
						Dispose();
					}
				}

		// Get the credential type.
		public override int CredType
				{
					get
					{
						return 1;	// GNUTLS_CRD_CERTIFICATE
					}
				}

		// Dispose of this object.
		public override void Dispose()
				{
					if(cred != IntPtr.Zero)
					{
						gnutls_certificate_free_credentials(cred);
						base.Dispose();
					}
					isDisposed = true;
				}

	}; // class GNUTLSAnonCredentials

	// Session handling object for GNUTLS.
	private sealed class GNUTLSSession : ISecureSession, IDisposable
	{
		// Internal state.
		private Protocol protocol;
		private bool isClient;
		private byte[] certificate;
		private byte[] privateKey;
		private byte[] remoteCertificate;
		private GNUTLSStream stream;

		// Constructor.
		public GNUTLSSession(Protocol protocol, bool isClient)
				{
					this.protocol = protocol;
					this.isClient = isClient;
				}

		// Destructor.
		~GNUTLSSession()
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
							certificate = value;
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
							privateKey = value;
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

						// Allocate and set the credentials object.
						GNUTLSCredentials cred;
						if(certificate != null && privateKey != null)
						{
							cred = new GNUTLSCertificateCredentials
								(certificate, privateKey);
						}
						else
						{
							cred = new GNUTLSAnonCredentials();
						}
						if(cred.Cred == IntPtr.Zero)
						{
							throw new NotSupportedException();
						}

						// Initialize the session object.
						IntPtr session = IntPtr.Zero;
						if(isClient)
						{
							gnutls_init(ref session,
										(Int)2 /* GNUTLS_CLIENT */);
						}
						else
						{
							gnutls_init(ref session,
									    (Int)1 /* GNUTLS_SERVER */);
						}
						if(session == IntPtr.Zero)
						{
							cred.Dispose();
							throw new NotSupportedException();
						}

						// Set the default priority values and then
						// override for the protocol specifics.
						gnutls_set_default_priority(session);
						gnutls_certificate_type_set_priority
							(session, new Int [] {(Int)1, (Int)2, (Int)0});
						switch(protocol)
						{
							case Protocol.AutoDetect:
							{
								// The defaults are already set up for this.
							}
							break;

							// Note: GNUTLS does not support SSLv2, so
							// we use SSLv3 in that case.
							case Protocol.SSLv2:
							case Protocol.SSLv3:
							{
								gnutls_protocol_set_priority
									(session, new Int [] {(Int)1, (Int)0});
							}
							break;

							case Protocol.TLSv1:
							{
								gnutls_protocol_set_priority
									(session, new Int [] {(Int)2, (Int)0});
							}
							break;

							default:
							{
								// Don't know what protocol to use.
								gnutls_deinit(session);
								cred.Dispose();
								throw new NotSupportedException();
							}
						}

						// Set the local X.509 credentials if necessary.
						gnutls_credentials_set
							(session, (Int)(cred.CredType), cred.Cred);

						// Associate the socket fd with the session.
						gnutls_transport_set_ptr(session, new IntPtr(fd));

						// Attempt to connect or accept.
						int result = (int)gnutls_handshake(session);
						if(result < 0)
						{
							gnutls_deinit(session);
							cred.Dispose();
							throw new SecurityException
								(gnutls_strerror((Int)result));
						}

						// Get the remote certificate and record it.
						UInt size;
						gnutls_datum *x509 = gnutls_certificate_get_peers
							(session, out size); 
						if(x509 != null)
						{
							int length = (int)(x509->size);
							if(length > 0)
							{
								remoteCertificate = new byte [length];
								Marshal.Copy(x509->data, remoteCertificate,
											 0, length);
							}
						}

						// Create the stream object and return it.
						stream = new GNUTLSStream(session, cred);
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
					if(stream != null)
						stream.Close();
				}

	}; // class GNUTLSSession

	// Stream object for managing a secure connection.
	private sealed class GNUTLSStream : Stream
	{
		// Internal state.
		private IntPtr session;
		private GNUTLSCredentials cred;

		// Constructor.
		public GNUTLSStream(IntPtr session, GNUTLSCredentials cred)
				{
					this.session = session;
					this.cred = cred;
				}

		// Destructor.
		~GNUTLSStream()
				{
					Close();
				}

		// Close the stream.
		public override void Close()
				{
					lock(this)
					{
						if(session != IntPtr.Zero)
						{
							gnutls_bye(session, 0 /* GNUTLS_SHUT_RDWR */);
							gnutls_deinit(session);
							session = IntPtr.Zero;
							cred.Dispose();
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
						if(session == IntPtr.Zero)
						{
							throw new ObjectDisposedException("stream");
						}
						if(offset == 0)
						{
							return (int)gnutls_record_recv
								(session, buffer, (Int)count);
						}
						else
						{
							byte[] temp = new byte [count];
							int result = (int)gnutls_record_recv
								(session, temp, (Int)count);
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

		// Write to the stream, with a guaranteed zero-based offset.
		private void Write(byte[] buffer, int count)
				{
					int result;
					for(;;)
					{
						result = (int)gnutls_record_send
							(session, buffer, (Int)count);
						if(result != -52 &&		// GNUTLS_E_INTERRUPTED
						   result != -28)		// GNUTLS_E_AGAIN
						{
							break;
						}
					}
				}

		// Write a buffer of bytes to this stream.
		public override void Write(byte[] buffer, int offset, int count)
				{
					Utils.ValidateBuffer(buffer, offset, count);
					lock(this)
					{
						if(session == IntPtr.Zero)
						{
							throw new ObjectDisposedException("stream");
						}
						if(offset == 0)
						{
							Write(buffer, count);
						}
						else if(count > 0)
						{
							byte[] temp = new byte [count];
							Array.Copy(buffer, offset, temp, 0, count);
							Write(temp, count);
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

	}; // class GNUTLSStream

	// Import the functions we need from the GNUTLS library.

	[DllImport("gnutls")]
	extern private static Int gnutls_global_init();

	[DllImport("gnutls")]
	extern private static Int gnutls_certificate_allocate_credentials
			(out IntPtr sc);

	[DllImport("gnutls")]
	extern private static void gnutls_certificate_free_credentials(IntPtr sc);

	[DllImport("gnutls")]
	extern private static Int gnutls_anon_allocate_client_credentials
			(out IntPtr sc);

	[DllImport("gnutls")]
	extern private static void gnutls_anon_free_client_credentials(IntPtr sc);

	[DllImport("gnutls")]
	extern private static Int gnutls_anon_allocate_server_credentials
			(out IntPtr sc);

	[DllImport("gnutls")]
	extern private static void gnutls_anon_free_server_credentials(IntPtr sc);

	[DllImport("gnutls")]
	extern private static Int gnutls_init(ref IntPtr session, Int con_end);

	[DllImport("gnutls")]
	extern private static void gnutls_deinit(IntPtr session);

	[DllImport("gnutls")]
	extern private static Int gnutls_set_default_priority(IntPtr session);

	[DllImport("gnutls")]
	extern private static Int gnutls_protocol_set_priority
			(IntPtr session, Int[] list);

	[DllImport("gnutls")]
	extern private static Int gnutls_certificate_type_set_priority
			(IntPtr session, Int[] list);

	[DllImport("gnutls")]
	extern private static void gnutls_transport_set_ptr
			(IntPtr session, IntPtr ptr);

	[DllImport("gnutls")]
	extern private static Int gnutls_handshake(IntPtr session);

	[DllImport("gnutls")]
	extern private static String gnutls_strerror(Int error);

	[DllImport("gnutls")]
	extern private static Int gnutls_credentials_set
			(IntPtr session, Int type, IntPtr cred);

	[DllImport("gnutls")]
	extern private static gnutls_datum *gnutls_certificate_get_peers
			(IntPtr session, out UInt size);

	[DllImport("gnutls")]
	extern private static Int gnutls_bye(IntPtr session, Int how);

	[DllImport("gnutls")]
	extern private static Int gnutls_record_send
			(IntPtr session, byte[] data, Int size);

	[DllImport("gnutls")]
	extern private static Int gnutls_record_recv
			(IntPtr session, byte[] data, Int size);

	[DllImport("gnutls")]
	extern private static Int gnutls_certificate_set_x509_key_mem
			(IntPtr cred, ref gnutls_datum cert,
			 ref gnutls_datum key, Int type);

	[StructLayout(LayoutKind.Sequential)]
	private struct gnutls_datum
	{
		public IntPtr data;
		public UInt size;

		// Constructor.
		public gnutls_datum(byte[] value)
				{
					data = Marshal.AllocHGlobal(value.Length);
					Marshal.Copy(value, 0, data, value.Length);
					size = (UInt)(value.Length);
				}

		// Free the data in this object.
		public void Free()
				{
					if(data != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(data);
						data = IntPtr.Zero;
						size = (UInt)0;
					}
				}

	}; // struct gnutls_datum

}; // class GNUTLS

#endif // CONFIG_RUNTIME_INFRA

}; // namespace DotGNU.SSL
