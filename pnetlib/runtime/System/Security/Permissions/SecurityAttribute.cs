/*
 * SecurityAttribute.cs - Implementation of the
 *			"System.Security.Permissions.SecurityAttribute" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if CONFIG_PERMISSIONS

using System;
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public abstract class SecurityAttribute : Attribute
{
	// Internal state.
	private SecurityAction action;
	private bool unrestricted;

	// Constructors.
	internal SecurityAttribute()
			{
				action = SecurityAction.Demand;
				unrestricted = false;
			}
	public SecurityAttribute(SecurityAction action)
			{
				this.action = action;
				unrestricted = false;
			}

	// Create a permission object that corresponds to this attribute.
	public abstract IPermission CreatePermission();

	// Get or set the security action.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	SecurityAction Action
			{
				get
				{
					return action;
				}
				set
				{
					action = value;
				}
			}

	// Get or set the unrestricted state.
	public bool Unrestricted
			{
				get
				{
					return unrestricted;
				}
				set
				{
					unrestricted = value;
				}
			}

}; // class SecurityAttribute

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security.Permissions
