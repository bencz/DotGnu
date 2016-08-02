/*
 * TcpListener.cs - Implementation of the
 *			"System.Net.Sockets.TcpListener" class.
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

public class TcpListener
{
	// Internal state.
	private Socket server;
	private IPEndPoint serverEP;

	// Constructors.
	public TcpListener(IPEndPoint localEP)
			{
				if(localEP == null)
				{
					throw new ArgumentNullException("localEP");
				}
				server = null;
				serverEP = localEP;
			}
	public TcpListener(IPAddress localaddr, int port)
			: this(new IPEndPoint(localaddr, port))
			{
				// Nothing to do here.
			}
	[Obsolete("Use TcpListener(IPAddress localaddr, int port).")]
	public TcpListener(int port)
			: this(new IPEndPoint(IPAddress.Any, port))
			{
				// Nothing to do here.
			}

	// Destructor.
	~TcpListener()
			{
				Stop();
			}

	// Accept the next incoming connection on a listener and
	// return it as a raw socket with no "TcpClient" wrapper.
	public Socket AcceptSocket()
			{
				if(server == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_ServerNotCreated"));
				}
				else
				{
					return server.Accept();
				}
			}

	// Accept the next incoming connection on a listener and
	// wrap it in a "TcpClient" object.
	public TcpClient AcceptTcpClient()
			{
				if(server == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_ServerNotCreated"));
				}
				else
				{
					return new TcpClient(server.Accept());
				}
			}

	// Determine if there is a pending connection on a listener.
	public bool Pending()
			{
				if(server == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_ServerNotCreated"));
				}
				else
				{
					return server.Poll(0, SelectMode.SelectRead);
				}
			}

	// Start the listener.
	public void Start()
			{
				if(server == null)
				{
					server = new Socket(AddressFamily.InterNetwork,
										SocketType.Stream,
										ProtocolType.Tcp);
					try
					{
						// Attempt to reuse an existing address, to prevent
						// delays in re-binding to the same port after a
						// server crash.
						server.SetSocketOption
							(SocketOptionLevel.Socket,
							 SocketOptionName.ReuseAddress, 1);
					}
					catch(SocketException)
					{
						// Don't get too concerned if we cannot reuse.
						// It isn't serious - just annoying.
					}
					try
					{
						server.Bind(serverEP);
						server.Listen(Int32.MaxValue);
					}
					catch(SocketException)
					{
						// Clean up the socket if the bind or listen failed.
						server.Close();
						server = null;
						throw;
					}
				}
			}

	// Stop the listener.
	public void Stop()
			{
				if(server != null)
				{
					server.Close();
					server = null;
				}
			}

	// Determine if the server socket is active.
	protected bool Active
			{
				get
				{
					return (server != null);
				}
			}

	// Get the local end-point for the server.
	public EndPoint LocalEndpoint
			{
				get
				{
					if(server != null)
					{
						return server.LocalEndPoint;
					}
					else
					{
						return serverEP;
					}
				}
			}

	// Get the socket that is acting as the server.
	protected Socket Server
			{
				get
				{
					return server;
				}
			}

}; // class TcpListener

#endif // !ECMA_COMPAT

}; // namespace System.Net.Sockets
