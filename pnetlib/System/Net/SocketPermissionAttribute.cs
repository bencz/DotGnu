/*
 * SocketPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.SocketPermissionAttribute" class.
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

using System;
using System.Security;
using System.Security.Permissions;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class SocketPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private String access;
	private String transport;
	private String hostName;
	private String portNumber;

	// Constructors.
	public SocketPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the properties.
	public String Access
			{
				get
				{
					return access;
				}
				set
				{
					if(access != null)
					{
						throw new ArgumentException
							(S._("Arg_AccessAlreadySet"));
					}
					access = value;
				}
			}
	public String Transport
			{
				get
				{
					return transport;
				}
				set
				{
					if(transport != null)
					{
						throw new ArgumentException
							(S._("Arg_TransportAlreadySet"));
					}
					transport = value;
				}
			}
	public String Host
			{
				get
				{
					return hostName;
				}
				set
				{
					if(hostName != null)
					{
						throw new ArgumentException
							(S._("Arg_HostAlreadySet"));
					}
					hostName = value;
				}
			}
	public String Port
			{
				get
				{
					return portNumber;
				}
				set
				{
					if(portNumber != null)
					{
						throw new ArgumentException
							(S._("Arg_PortAlreadySet"));
					}
					portNumber = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new SocketPermission
						(PermissionState.Unrestricted);
				}
				else if(access == null || transport == null ||
						hostName == null || portNumber == null)
				{
					throw new ArgumentException
						(S._("Arg_SocketPermission"));
				}
				else
				{
					return new SocketPermission
						((NetworkAccess)Enum.Parse
							(typeof(NetworkAccess), access, true),
						 (TransportType)Enum.Parse
						 	(typeof(TransportType), transport, true),
						 hostName,
						 (portNumber == "All" ? SocketPermission.AllPorts
						 				      : Int32.Parse(portNumber)));
				}
			}

}; // class SocketPermissionAttribute

#endif // CONFIG_PERMISSIONS

}; // namespace System.Net
