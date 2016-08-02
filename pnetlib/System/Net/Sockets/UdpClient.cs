/*
 * UdpClient.cs - Implementation of the
 *			"System.Net.Sockets.UdpClient" class.
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

public class UdpClient : IDisposable
{
	// Internal state.
	private Socket client;
	private bool active;

	// Constructors.
	public UdpClient()
			{
				Initialize(null, null, AddressFamily.InterNetwork);
			}
	public UdpClient(AddressFamily family)
			{
				Initialize(null, null, family);
			}
	public UdpClient(IPEndPoint localEP)
			{
				if(localEP == null)
				{
					throw new ArgumentNullException("localEP");
				}
				Initialize(localEP, null, localEP.AddressFamily);
			}
	public UdpClient(int port)
			{
				if(port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
				{
					throw new ArgumentOutOfRangeException
						("port", S._("ArgRange_Port"));
				}
				Initialize(new IPEndPoint(IPAddress.Any, port),
						   null, AddressFamily.InterNetwork);
			}
	public UdpClient(int port, AddressFamily family)
			{
				if(port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
				{
					throw new ArgumentOutOfRangeException
						("port", S._("ArgRange_Port"));
				}
				if(family == AddressFamily.InterNetworkV6)
				{
					Initialize(new IPEndPoint(IPAddress.IPv6Any, port),
							   null, AddressFamily.InterNetworkV6);
				}
				else
				{
					Initialize(new IPEndPoint(IPAddress.Any, port),
							   null, AddressFamily.InterNetwork);
				}
			}
	public UdpClient(String hostname, int port)
			{
				IPEndPoint remoteEP = TcpClient.Lookup(hostname, port);
				Initialize(null, remoteEP, remoteEP.AddressFamily);
			}

	// Initialize this object with a new UDP socket, optionally bind
	// to a local end-point, and optionally connect to a remote
	// end-point.  If anything fails, the object will be left in a
	// clean state, with the socket handle closed.
	private void Initialize(IPEndPoint localEP, IPEndPoint remoteEP,
							AddressFamily family)
			{
				client = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
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

	// Close this UDP client object.
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
				Connect(TcpClient.Lookup(hostname, port));
			}
	public void Connect(IPAddress address, int port)
			{
				Connect(new IPEndPoint(address, port));
			}

	// Dispose of this object.
	private void Dispose(bool disposing)
			{
				if(client != null)
				{
					client.Close();
					client = null;
				}
				active = false;
			}

	// Join a multicast group.
	public void JoinMulticastGroup(IPAddress multicastAddr)
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				client.SetSocketOption(SocketOptionLevel.IP,
									   SocketOptionName.AddMembership,
									   new MulticastOption(multicastAddr));
			}
	public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
			{
				// Ignore the TTL because changing it isn't secure.
				JoinMulticastGroup(multicastAddr);
			}
	public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				client.SetSocketOption(SocketOptionLevel.IPv6,
									   SocketOptionName.AddMembership,
									   new IPv6MulticastOption
									   		(multicastAddr, ifindex));
			}

	// Drop a multicast group.
	public void DropMulticastGroup(IPAddress multicastAddr)
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				client.SetSocketOption(SocketOptionLevel.IP,
									   SocketOptionName.DropMembership,
									   new MulticastOption(multicastAddr));
			}
	public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				client.SetSocketOption(SocketOptionLevel.IPv6,
									   SocketOptionName.DropMembership,
									   new IPv6MulticastOption
									   		(multicastAddr, ifindex));
			}

	// Receive the next datagram using this client.
	public byte[] Receive(ref IPEndPoint remoteEP)
			{
				// Bail out if the socket has been closed.
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}

				// Poll for the next read available event.
				client.Poll(-1, SelectMode.SelectRead);

				// Create a buffer to hold the database.
				int size = client.Available;
				byte[] dgram = new byte [size];

				// Receive the datagram from the socket.
				EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
				client.ReceiveFrom(dgram, 0, size,
								   SocketFlags.None, ref endPoint);
				remoteEP = (IPEndPoint)endPoint;

				// Return the datagram to the caller.
				return dgram;
			}

	// Send a datagram using this client.
	public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
			{
				if(client == null)
				{
					throw new ObjectDisposedException
						(S._("Exception_Disposed"));
				}
				if(dgram == null)
				{
					throw new ArgumentNullException("dgram");
				}
				if(endPoint == null)
				{
					return client.Send(dgram, 0, bytes, SocketFlags.None);
				}
				else if(active)
				{
					throw new InvalidOperationException
						(S._("Invalid_SendToOnConnected"));
				}
				else
				{
					return client.SendTo(dgram, 0, bytes,
										 SocketFlags.None, endPoint);
				}
			}
	public int Send(byte[] dgram, int bytes, String hostname, int port)
			{
				if(hostname == null && port == 0)
				{
					return Send(dgram, bytes, null);
				}
				else
				{
					return Send(dgram, bytes,
								TcpClient.Lookup(hostname, port));
				}
			}
	public int Send(byte[] dgram, int bytes)
			{
				if(!active)
				{
					throw new InvalidOperationException
						(S._("Invalid_SendOnNonConnected"));
				}
				return Send(dgram, bytes, null);
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

}; // class UdpClient

#endif // !ECMA_COMPAT

}; // namespace System.Net.Sockets
