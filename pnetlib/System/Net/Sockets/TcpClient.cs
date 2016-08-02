/*
 * TcpClient.cs - Implementation of the
 *			"System.Net.Sockets.TcpClient" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Net.Sockets
{

#if !ECMA_COMPAT

using System;
using System.IO;

public class TcpClient : IDisposable
{
	// Internal state.
	private Socket client;
	private NetworkStream stream;
	private bool active;

	// Constructors.
	public TcpClient()
			{
				Initialize(null, null, AddressFamily.InterNetwork);
			}
	public TcpClient(AddressFamily family)
			{
				Initialize(null, null, family);
			}
	public TcpClient(IPEndPoint localEP)
			{
				if(localEP == null)
				{
					throw new ArgumentNullException("localEP");
				}
				Initialize(localEP, null, localEP.AddressFamily);
			}
	public TcpClient(String hostname, int port)
			{
				IPEndPoint remoteEP = Lookup(hostname, port);
				Initialize(null, remoteEP, remoteEP.AddressFamily);
			}
	internal TcpClient(Socket client)
			{
				this.client = client;
				this.stream = null;
				this.active = true;
			}

	// Destructor.
	~TcpClient()
			{
				Dispose(false);
			}

	// Initialize this object with a new TCP socket, optionally bind
	// to a local end-point, and optionally connect to a remote
	// end-point.  If anything fails, the object will be left in a
	// clean state, with the socket handle closed.
	private void Initialize(IPEndPoint localEP, IPEndPoint remoteEP,
							AddressFamily family)
			{
				client = new Socket
					(family, SocketType.Stream, ProtocolType.Tcp);
				stream = null;
				active = false;
				try
				{
					if(localEP != null)
					{
						client.Bind(localEP);
					}
					if(remoteEP != null)
					{
						client.Connect(remoteEP);
						active = true;
					}
				}
				catch(SocketException)
				{
					// We weren't able to bind or connect, so clean up the
					// socket on our way back up the stack.
					client.Close();
					client = null;
					throw;
				}
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Close this TCP client object.
	public void Close()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Connect to a remote end-point.
	public void Connect(IPEndPoint remoteEP)
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				client.Connect(remoteEP);
				active = true;
			}
	public void Connect(String hostname, int port)
			{
				Connect(Lookup(hostname, port));
			}
	public void Connect(IPAddress address, int port)
			{
				Connect(new IPEndPoint(address, port));
			}

	// Dispose of this object.
	protected virtual void Dispose(bool disposing)
			{
				// Close the client socket if we haven't yet attached a
				// "NetworkStream" object to it.
				if(stream == null && client != null)
				{
					client.Close();
				}

				// Clear the internal state.
				client = null;
				stream = null;
				active = false;
			}

	// Get a network stream for accessing the client socket.
	public NetworkStream GetStream()
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				if(!(client.Connected))
				{
					throw new InvalidOperationException
						(S._("IO_SocketNotConnected"));
				}
				if(stream == null)
				{
					stream = new NetworkStream
						(client, FileAccess.ReadWrite, true);
				}
				return stream;
			}

	// Get or set a value that indicates if there is an active connection.
	protected bool Active
			{
				get
				{
					return active;
				}
				set
				{
					active = value;
				}
			}

	// Get or set the socket used by this client object.
	protected Socket Client
			{
				get
				{
					return client;
				}
				set
				{
					client = value;
				}
			}

	// Get or set the linger state.
	public LingerOption LingerState
			{
				get
				{
					return (LingerOption)(client.GetSocketOption
						(SocketOptionLevel.Socket,
						 SocketOptionName.Linger));
				}
				set
				{
					client.SetSocketOption(SocketOptionLevel.Socket,
										   SocketOptionName.Linger,
										   value);
				}
			}

	// Get or set the no-delay flag.
	public bool NoDelay
			{
				get
				{
					return (((int)(client.GetSocketOption
						(SocketOptionLevel.Tcp,
						 SocketOptionName.NoDelay))) != 0);
				}
				set
				{
					client.SetSocketOption(SocketOptionLevel.Tcp,
										   SocketOptionName.NoDelay,
										   value ? 1 : 0);
				}
			}

	// Get or set the receive buffer size.
	public int ReceiveBufferSize
			{
				get
				{
					return (int)(client.GetSocketOption
						(SocketOptionLevel.Socket,
						 SocketOptionName.ReceiveBuffer));
				}
				set
				{
					client.SetSocketOption(SocketOptionLevel.Socket,
										   SocketOptionName.ReceiveBuffer,
										   value);
				}
			}

	// Get or set the receive timeout value.
	public int ReceiveTimeout
			{
				get
				{
					return (int)(client.GetSocketOption
						(SocketOptionLevel.Socket,
						 SocketOptionName.ReceiveTimeout));
				}
				set
				{
					client.SetSocketOption(SocketOptionLevel.Socket,
										   SocketOptionName.ReceiveTimeout,
										   value);
				}
			}

	// Get or set the send buffer size.
	public int SendBufferSize
			{
				get
				{
					return (int)(client.GetSocketOption
						(SocketOptionLevel.Socket,
						 SocketOptionName.SendBuffer));
				}
				set
				{
					client.SetSocketOption(SocketOptionLevel.Socket,
										   SocketOptionName.SendBuffer,
										   value);
				}
			}

	// Get or set the send timeout value.
	public int SendTimeout
			{
				get
				{
					return (int)(client.GetSocketOption
						(SocketOptionLevel.Socket,
						 SocketOptionName.SendTimeout));
				}
				set
				{
					client.SetSocketOption(SocketOptionLevel.Socket,
										   SocketOptionName.SendTimeout,
										   value);
				}
			}

	// Perform a lookup on a hostname and port number to
	// get a remote end-point.
	internal static IPEndPoint Lookup(String hostname, int port)
			{
				if(hostname == null)
				{
					throw new ArgumentNullException("hostname");
				}
				if(port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
				{
					throw new ArgumentOutOfRangeException
						("port", S._("ArgRange_Port"));
				}
				IPAddress address = Dns.Resolve(hostname).AddressList[0];
				return new IPEndPoint(address, port);
			}

}; // class TcpClient

#endif // !ECMA_COMPAT

}; // namespace System.Net.Sockets
