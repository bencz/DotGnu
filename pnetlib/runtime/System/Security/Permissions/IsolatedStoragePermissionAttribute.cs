/*
 * IsolatedStoragePermissionAttribute.cs - Implementation of the
 *		"System.Security.Permissions.IsolatedStoragePermissionAttribute" class.
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

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

using System;
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public abstract class IsolatedStoragePermissionAttribute
	: CodeAccessSecurityAttribute
{
	// Internal state.
	internal long userQuota;
	internal IsolatedStorageContainment usageAllowed;

	// Constructors.
	public IsolatedStoragePermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the user quota value.
	public long UserQuota
			{
				get
				{
					return userQuota;
				}
				set
				{
					userQuota = value;
				}
			}

	// Get or set the allowed containment usage value.
	public IsolatedStorageContainment UsageAllowed
			{
				get
				{
					return usageAllowed;
				}
				set
				{
					usageAllowed = value;
				}
			}

}; // class IsolatedStoragePermissionAttribute

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
