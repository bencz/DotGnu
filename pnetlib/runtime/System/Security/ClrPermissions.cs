/*
 * ClrPermissions.cs - Implementation of the
 *		"System.Security.ClrPermissions" class.
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

namespace System.Security
{

using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

// The "ClrPermission" class represents a group of permission set
// values for a particular stack frame with the thread's call stack.

internal sealed class ClrPermissions
{

#if CONFIG_PERMISSIONS

	// Accessible internal state.
	public PermissionSet granted;
	public PermissionSet denied;
	public PermissionSet permitOnly;

	// Constructor.
	public ClrPermissions(PermissionSet granted, PermissionSet denied,
						  PermissionSet permitOnly)
			{
				this.granted = granted;
				this.denied = denied;
				this.permitOnly = permitOnly;
			}

	// Set the granted permissions and return a new object.
	public ClrPermissions SetGranted(PermissionSet set)
			{
				return new ClrPermissions(set, denied, permitOnly);
			}

	// Set the denied permissions and return a new object.
	public ClrPermissions SetDenied(PermissionSet set)
			{
				return new ClrPermissions(granted, set, permitOnly);
			}

	// Set the permit-only permissions and return a new object.
	public ClrPermissions SetPermitOnly(PermissionSet set)
			{
				return new ClrPermissions(granted, denied, set);
			}

#else

	// Accessible internal state.
	public Object granted;
	public Object denied;
	public Object permitOnly;

#endif

}; // class ClrPermissions

}; // namespace System.Security
