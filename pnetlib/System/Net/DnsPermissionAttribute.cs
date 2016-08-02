/*
 * DnsPermissionAttribute.cs - Implementation of the "System.Net.DnsPermissionAttribute" class.
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 *
 * Contributed by Jason Lee <jason.lee@mac.com>
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
	
public sealed class DnsPermissionAttribute : CodeAccessSecurityAttribute
{

	// Constructor

	public DnsPermissionAttribute(SecurityAction action) : base (action)
	{
	}
	
	// Methods

	public override IPermission CreatePermission()
	{

		if(this.Unrestricted)
		{
			return new DnsPermission(PermissionState.Unrestricted);
		}
		
		return new DnsPermission(PermissionState.None);
			
	}
	
} // class DnsPermissionAttribute

#endif // CONFIG_PERMISSIONS

} //namespace System.Net
