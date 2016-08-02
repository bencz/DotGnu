/*
 * IrDAEndPoint.cs - Implementation of the
 *			"System.Net.IrDAEndPoint" class.
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

namespace System.Net
{

using System.Net.Sockets;
using System.Text;

public class IrDAEndPoint : EndPoint
{
	// Internal state.
	private byte[] deviceID;
	private String serviceName;

	// Constructor.
	public IrDAEndPoint(byte[] irdaDeviceID, String serviceName)
			{
				// Validate the parameters.
				ValidateDeviceID(irdaDeviceID);
				ValidateServiceName(serviceName);

				// Save the parameters for later.
				this.deviceID = new byte [4];
				Array.Copy(irdaDeviceID, 0, this.deviceID, 0, 4);
				this.serviceName = serviceName;
			}

	// Get the address family for this end point.
	public override AddressFamily AddressFamily
			{
				get
				{
					return AddressFamily.Irda;
				}
			}

	// Get or set the device identifier for this end point.
	public byte[] DeviceID
			{
				get
				{
					return deviceID;
				}
				set
				{
					ValidateDeviceID(value);
					Array.Copy(value, 0, deviceID, 0, 4);
				}
			}

	// Get or set the service name for this end point.
	public String ServiceName
			{
				get
				{
					return serviceName;
				}
				set
				{
					ValidateServiceName(value);
					serviceName = value;
				}
			}

	// Create an end point from a socket address.
	public override EndPoint Create(SocketAddress sockaddr)
			{
				// Extract the device ID [2..5] from the socket address.
				byte[] deviceID = new byte [4];
				deviceID[0] = sockaddr[2];
				deviceID[1] = sockaddr[3];
				deviceID[2] = sockaddr[4];
				deviceID[3] = sockaddr[5];

				// Extract the service name [6..] from the socket address.
				int posn = 6;
				StringBuilder builder = new StringBuilder();
				while(posn < sockaddr.Size && sockaddr[posn] != 0)
				{
					builder.Append((char)(sockaddr[posn]));
					++posn;
				}

				// Make service name non empty on UNIX.
				if(builder.Length == 0)
				{
					builder.Append("LSA");
				}

				// Build and return the new end point.
				return new IrDAEndPoint(deviceID, builder.ToString());
			}

	// Serialize the end point into a socket address.
	public override SocketAddress Serialize()
			{
				SocketAddress addr = new SocketAddress(AddressFamily.Irda, 32);
				addr[2] = deviceID[0];
				addr[3] = deviceID[1];
				addr[4] = deviceID[2];
				addr[5] = deviceID[3];
				int posn = 0;
				while(posn < 24 && posn < serviceName.Length)
				{
					addr[posn + 6] = (byte)(serviceName[posn]);
					++posn;
				}
				addr[posn + 6] = 0;
				return addr;
			}

	// Validate an IrDA device identifier.
	private static void ValidateDeviceID(byte[] irdaDeviceID)
			{
				if(irdaDeviceID == null)
				{
					throw new ArgumentNullException("irdaDeviceID");
				}
				if(irdaDeviceID.Length != 4)
				{
					throw new ArgumentException
						(S._("IrDA_InvalidDeviceID"), "irdaDeviceID");
				}
			}

	// Validate an IrDA service name.
	private static void ValidateServiceName(String serviceName)
			{
				// Must not be null, empty, or greater than 24 characters long.
				if(serviceName == null)
				{
					throw new ArgumentNullException("serviceName");
				}
				if(serviceName.Length <= 0 || serviceName.Length >= 25)
				{
					throw new ArgumentException
						(S._("IrDA_InvalidServiceName"), "serviceName");
				}

				// Must only contain Latin1 characters.
				foreach(char ch in serviceName)
				{
					if(ch >= '\u0100')
					{
						throw new ArgumentException
							(S._("IrDA_InvalidServiceName"), "serviceName");
					}
				}
			}

}; // class IrDAEndPoint

}; // namespace System.Net
