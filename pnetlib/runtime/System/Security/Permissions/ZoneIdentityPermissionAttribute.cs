/*
 * ZoneIdentityPermissionAttribute.cs - Implementation of the
 *		"System.Security.Permissions.ZoneIdentityPermissionAttribute" class.
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

namespace System.Security.Permissions
{

#if CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

using System;
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class ZoneIdentityPermissionAttribute
	: CodeAccessSecurityAttribute
{
	// Internal state.
	private SecurityZone zone;

	// Constructors.
	public ZoneIdentityPermissionAttribute(SecurityAction action)
			: base(action)
			{
				zone = SecurityZone.NoZone;
			}

	// Get or set the zone value.
	public SecurityZone Zone
			{
				get
				{
					return zone;
				}
				set
				{
					zone = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				else
				{
					return new ZoneIdentityPermission(zone);
				}
			}

}; // class ZoneIdentityPermissionAttribute

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
