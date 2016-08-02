/*
 * EndPoint.cs - Implementation of the "System.Net.EndPoint" class.
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

public abstract class EndPoint
{
	// Constructor.
	protected EndPoint() {}
	
	// Create a new end point with a particular socket address.
	public virtual EndPoint Create(SocketAddress socketAddress) 
			{
				throw new NotSupportedException();
			}
	
	// Serialize this end point into a socket address array.
	public virtual SocketAddress Serialize()
			{
				throw new NotSupportedException();
			}

	// Get the address family of this end point.
	public virtual AddressFamily AddressFamily 
			{ 
				get
				{
					throw new NotSupportedException(); 
				}	
			}
	
}; // class EndPoint

}; // namespace System.Net
