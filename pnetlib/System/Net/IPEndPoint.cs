/*
 * IPEndPoint.cs - Implementation of the "System.Net.IPEndPoint" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Net
{

using System;
using System.Net.Sockets;

public class IPEndPoint : EndPoint
{
	// Internal state.
	private IPAddress address;
	private int port;
		
	// Minimum and maximum port numbers.
	public const int MinPort = 0;
	public const int MaxPort = 65535;

	// Constructors.
	public IPEndPoint(long address, int port) 
			{
				this.address = new IPAddress(address);
				if(port < MinPort || port > MaxPort)
				{
					throw new ArgumentOutOfRangeException("port");
				}
				this.port = port;
			}
	public IPEndPoint(IPAddress address, int port)
			{
				if(address == null)
				{
					throw new ArgumentNullException("address");
				}
				if(port < MinPort || port > MaxPort)
				{
					throw new ArgumentOutOfRangeException("port");
				}
				this.address = address;
				this.port = port;	
			}

	// Create a new end point with a particular socket address.
	public override EndPoint Create(SocketAddress socketAddress) 
			{
				// Validate the socket address.
				if(socketAddress.Family != address.AddressFamily)
				{
					throw new ArgumentException
						(S._("Arg_InvalidSockAddr"), "socketAddress");
				}

				// Different behaviour for IPv4 and IPv6.
				int port;
				int value;
				if(socketAddress.Family == AddressFamily.InterNetwork)
				{
					// Validate the address size.
					if(socketAddress.Size < 8)
					{
						throw new ArgumentException
							(S._("Arg_InvalidSockAddr"), "socketAddress");
					}

					// Extract the port and address.
					port = (socketAddress[2] << 8) | socketAddress[3];
					value = ((socketAddress[7]) |
							 (socketAddress[6] << 8) |
							 (socketAddress[5] << 16) |
							 (socketAddress[4] << 24));
					value = IPAddress.HostToNetworkOrder(value);

					// Construct the new end point.
					return new IPEndPoint(new IPAddress((uint)value), port);
				}
				else
				{
					// Validate the address size.
					if(socketAddress.Size < 28)
					{
						throw new ArgumentException
							(S._("Arg_InvalidSockAddr"), "socketAddress");
					}

					// Extract the port and scope.
					port = (socketAddress[2] << 8) | socketAddress[3];
					value = ((socketAddress[27]) |
							 (socketAddress[26] << 8) |
							 (socketAddress[25] << 16) |
							 (socketAddress[24] << 24));
					value = IPAddress.HostToNetworkOrder(value);

					// Extract the main part of the IPv6 address.
					byte[] buf = new byte [16];
					int posn;
					for(posn = 0; posn < 16; ++posn)
					{
						buf[posn] = socketAddress[posn + 8];
					}

					// Construct the new end point.
					return new IPEndPoint(new IPAddress(buf, (uint)value), port);
				}
			}

	// Serialize this end point into a socket address array.
	public override SocketAddress Serialize()
			{
				SocketAddress addr;
				if(address.AddressFamily == AddressFamily.InterNetwork)
				{
					// Serialize an ipv4 address.
					addr = new SocketAddress(AddressFamily.InterNetwork, 16);
					addr[2] = (byte)(port >> 8);
					addr[3] = (byte)port;
					int host = IPAddress.NetworkToHostOrder
						((int)(address.Address));
					addr[4] = (byte)(host >> 24);
					addr[5] = (byte)(host >> 16);
					addr[6] = (byte)(host >> 8);
					addr[7] = (byte)host;
					return addr;
				}
				else
				{
					// Serialize an ipv6 address.
					addr = new SocketAddress(AddressFamily.InterNetworkV6, 28);
					addr[2] = (byte)(port >> 8);
					addr[3] = (byte)port;
					ushort[] ipv6 = address.ipv6;
					int posn;
					for(posn = 0; posn < 8; ++posn)
					{
						addr[posn + 8] = (byte)(ipv6[posn] >> 8);
						addr[posn + 9] = (byte)(ipv6[posn]);
					}
					int scope = IPAddress.NetworkToHostOrder
						((int)(address.ScopeId));
					addr[24] = (byte)(scope >> 24);
					addr[25] = (byte)(scope >> 16);
					addr[26] = (byte)(scope >> 8);
					addr[27] = (byte)scope;
					return addr;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(object comparand)
			{
				IPEndPoint other = (comparand as IPEndPoint);
				if(other != null)
				{
					return (address.Equals(other.address) &&
							port == other.port);
				}
				else
				{
					return false;
				}
			}
			
	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return address.GetHashCode() + port.GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return address.ToString() + ":" + port.ToString();
			}

	// Get or set the IP address for this end point.
	public IPAddress Address
			{
				get
				{
					return address;
				}
				set
				{
					address = value;
				}
			}

	// Get the address family for this end point.
	public override AddressFamily AddressFamily 
			{ 
				get
				{
					return address.AddressFamily;
				}	
			}

	// Get the port for this end point.
	public int Port
			{
				get
				{
					return port;
				}
				set
				{	
					if(value < MinPort || value > MaxPort)
					{
						throw new ArgumentOutOfRangeException("port");
					}
					port = value;				
				}
			}
	
}; // class IPEndPoint

}; // namespace System.Net
