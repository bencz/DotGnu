/*
 * IrDAClient.cs - Implementation of the
 *			"System.Net.IrDAClient" class.
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

using System.IO;

public class IrDAClient
{
	// Internal state.
	private Socket socket;

	// Constructors.
	public IrDAClient()
			{
				socket = new Socket(AddressFamily.Irda,
									SocketType.Stream,
									ProtocolType.Unspecified);
			}
	public IrDAClient(IrDAEndPoint remoteEP) : this()
			{
				Connect(remoteEP);
			}
	public IrDAClient(String service) : this()
			{
				Connect(service);
			}
	internal IrDAClient(Socket socket)
			{
				this.socket = socket;
			}

	// Get the name of the device that we are connected to.
	public String RemoteMachineName
			{
				get
				{
					return GetRemoteMachineName(socket);
				}
			}

	// Close the connection.
	public void Close()
			{
				socket.Close();
			}

	// Connect to a remote end point.
	public void Connect(IrDAEndPoint remoteEP)
			{
				socket.Connect(remoteEP);
			}
	public void Connect(String service)
			{
				// Discover a device that we can connect to.
				IrDADeviceInfo[] devices = DiscoverDevices(1);
				if(devices.Length == 0)
				{
					throw new InvalidOperationException
						(S._("IrDA_NoDeviceAvailable"));
				}
				Connect(new IrDAEndPoint(devices[0].DeviceID, service));
			}

	// Discover information about the available IrDA devices.
	public IrDADeviceInfo[] DiscoverDevices(int maxDevices)
			{
				return DiscoverDevices(maxDevices, socket);
			}
	public static IrDADeviceInfo[] DiscoverDevices
				(int maxDevices, Socket irdaSocket)
			{
				// Fetch the option data from the socket.
				int maxSize = 4 + maxDevices * 32;
				byte[] data = irdaSocket.GetSocketOption
					((SocketOptionLevel)255, (SocketOptionName)16, maxSize);
				if(data == null || data.Length < 4)
				{
					return new IrDADeviceInfo [0];
				}

				// Construct the device array.
				uint num = BitConverter.ToUInt32(data, 0);
				if(num < 0)
				{
					num = 0;
				}
				else if(num > maxDevices)
				{
					num = (uint) maxDevices;
				}
				IrDADeviceInfo[] devs = new IrDADeviceInfo [num];
				int posn;
				for(posn = 0; posn < num; ++posn)
				{
					devs[posn] = new IrDADeviceInfo(data, 4 + posn * 32);
				}
				return devs;
			}

	// Get the name of a remote machine from an IrDA socket.
	public static String GetRemoteMachineName(Socket s)
			{
				// Find a device that has a matching device ID.
				byte[] remoteID = ((IrDAEndPoint)(s.RemoteEndPoint)).DeviceID;
				IrDADeviceInfo[] devices = DiscoverDevices(10, s);
				byte[] id;
				foreach(IrDADeviceInfo dev in devices)
				{
					id = dev.DeviceID;
					if(remoteID[0] == id[0] && remoteID[1] == id[1] &&
					   remoteID[2] == id[2] && remoteID[3] == id[3])
					{
						return dev.DeviceName;
					}
				}
				throw new InvalidOperationException
					(S._("IrDA_NoMatchingDevice"));
			}

	// Get the underlying data stream for the connection.
	public Stream GetStream()
			{
				return new NetworkStream(socket);
			}

}; // class IrDAClient

}; // namespace System.Net.Sockets
