/*
 * WebPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.WebPermissionAttribute" class.
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
using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class WebPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private Object accept;
	private Object connect;

	// Constructors.
	public WebPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the properties.
	public String Accept
			{
				get
				{
					return accept.ToString();
				}
				set
				{
					if(accept != null)
					{
						throw new ArgumentException
							(S._("Arg_WebAcceptAlreadySet"));
					}
					accept = value;
				}
			}
#if !ECMA_COMPAT
	public String AcceptPattern
			{
				get
				{
					return accept.ToString();
				}
				set
				{
					if(accept != null)
					{
						throw new ArgumentException
							(S._("Arg_WebAcceptAlreadySet"));
					}
					accept = new Regex(value);
				}
			}
#endif
	public String Connect
			{
				get
				{
					return connect.ToString();
				}
				set
				{
					if(connect != null)
					{
						throw new ArgumentException
							(S._("Arg_WebConnectAlreadySet"));
					}
					connect = value;
				}
			}
#if !ECMA_COMPAT
	public String ConnectPattern
			{
				get
				{
					return connect.ToString();
				}
				set
				{
					if(connect != null)
					{
						throw new ArgumentException
							(S._("Arg_WebConnectAlreadySet"));
					}
					connect = new Regex(value);
				}
			}
#endif

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new WebPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					WebPermission perm = new WebPermission();
					if(accept != null)
					{
						perm.AddPermission(NetworkAccess.Accept, accept);
					}
					if(connect != null)
					{
						perm.AddPermission(NetworkAccess.Connect, connect);
					}
					return perm;
				}
			}

}; // class WebPermissionAttribute

#endif // CONFIG_PERMISSIONS

}; // namespace System.Net
