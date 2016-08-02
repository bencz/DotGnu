/*
 * PrincipalPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.PrincipalPermissionAttribute" class.
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

[AttributeUsage( AttributeTargets.Class |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class PrincipalPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private String name;
	private String role;
	private bool authenticated;

	// Constructors.
	public PrincipalPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the principal name.
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

	// Get or set the principal role.
	public String Role
			{
				get
				{
					return role;
				}
				set
				{
					role = value;
				}
			}

	// Get or set the principal authentication flag.
	public bool Authenticated
			{
				get
				{
					return authenticated;
				}
				set
				{
					authenticated = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new PrincipalPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new PrincipalPermission(name, role, authenticated);
				}
			}

}; // class PrincipalPermissionAttribute

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
