/*
 * SocketAddress.cs - Implementation of the "System.Net.SocketAddress" class.
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
using System.Text;
using System.Net.Sockets;

public class SocketAddress
{
	// Internal state.
	private byte[] array;

	// Constructors.
	public SocketAddress(AddressFamily family) : this(family, 32) {}
	public SocketAddress(AddressFamily family, int size)
			{
				if(size < 2) 
				{
					throw new ArgumentOutOfRangeException();
				}
				array = new byte[size];
				array[0] = (byte)family;
				array[1] = (byte)((int)family >> 8);
			}
	internal SocketAddress(byte[] array)
			{
				this.array = array;
			}

	// Determine if two objects are equal.
	public override bool Equals(Object comparand) 
			{
				SocketAddress other = (comparand as SocketAddress);
				if(other != null)
				{
					if(array.Length != other.array.Length)
					{
						return false;
					}
					int posn = array.Length;
					while(posn > 0)
					{
						--posn;
						if(array[posn] != other.array[posn])
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				int hash = 0;
				int posn;
				for(posn = 0; posn < array.Length; ++posn)
				{
					hash = (hash << 5) + hash + array[posn];
				}
				return hash;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(Family.ToString());
				builder.Append(String.Format(":{0}:{", Size));
				int posn = 2;
				while(posn < array.Length)
				{
					if(posn > 2)
					{
						builder.Append(',');
					}
					builder.Append(array[posn].ToString());
					++posn;
				}
				builder.Append('}');
				return builder.ToString();
			}

	// Get the address family from the socket address.
	public AddressFamily Family
			{
				get
				{
					return (AddressFamily)(array[0] | (array[1] << 8));
				}
			}

	// Get or set a specific address element.
	public byte this[int offset]
			{
				get
				{
					return array[offset];
				}
				set
				{
					array[offset] = value;
				}
			}

	// Get the size of the socket address.
	public int Size
			{
				get
				{
					return array.Length;
				}
			}

	// Get the underlying array within this socket address.
	internal byte[] Array
			{
				get
				{
					return array;
				}
			}

	// Extract the IP address portion of this socket address.
	internal IPAddress IPAddress
			{
				get
				{
					IPEndPoint ep;
					if(Family == AddressFamily.InterNetwork)
					{
						ep = new IPEndPoint(IPAddress.Any, 0);
					}
					else
					{
						ep = new IPEndPoint(IPAddress.IPv6Any, 0);
					}
					ep = (IPEndPoint)(ep.Create(this));
					return ep.Address;
				}
			}

}; // class SocketAddress

}; // namespace System.Net
