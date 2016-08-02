/*
 * EndpointPermission.cs - Implementation of the
 *			"System.Net.EndpointPermission" class.
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

#if CONFIG_PERMISSIONS

using System.Globalization;

#if ECMA_COMPAT
internal
#else
public
#endif
class EndpointPermission
{
	// Accessible internal state.
	internal NetworkAccess access;
	internal TransportType transport;
	internal String hostName;
	internal int portNumber;

	// Constructor.
	internal EndpointPermission(NetworkAccess access,
								TransportType transport,
								String hostName,
								int portNumber)
			{
				this.access = access;
				this.transport = transport;
				this.hostName = hostName;
				this.portNumber = portNumber;
			}

	// Get the endpoint information.
	public TransportType Transport
			{
				get
				{
					return transport;
				}
			}
	public String Hostname
			{
				get
				{
					return hostName;
				}
			}
	public int Port
			{
				get
				{
					return portNumber;
				}
			}

	// Determine if two endpoints are equal (ignoring access).
	public override bool Equals(Object obj)
			{
				EndpointPermission info = (obj as EndpointPermission);
				if(info != null)
				{
					if(transport != info.transport)
					{
						return false;
					}
					if(portNumber != info.portNumber)
					{
						return false;
					}
				#if !ECMA_COMPAT
					return (String.Compare(hostName, info.hostName, true,
										   CultureInfo.InvariantCulture) == 0);
				#else
					return (String.Compare(hostName, info.hostName, true) == 0);
				#endif
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return hostName + "#" + portNumber.ToString() + "#" +
					   ((int)transport).ToString();
			}

}; // class EndpointPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Net
