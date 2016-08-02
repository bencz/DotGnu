/*
 * EnvironmentPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.EnvironmentPermissionAttribute" class.
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

#if CONFIG_PERMISSIONS

using System;
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class EnvironmentPermissionAttribute
		: CodeAccessSecurityAttribute
{
	// Internal state.
	private String read;
	private String write;

	// Constructors.
	public EnvironmentPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the read permission value.
	public String Read
			{
				get
				{
					return read;
				}
				set
				{
					read = value;
				}
			}

	// Get or set the write permission value.
	public String Write
			{
				get
				{
					return write;
				}
				set
				{
					write = value;
				}
			}

	// Set both the read and write permission values.
	public String All
			{
#if !ECMA_COMPAT
				get
				{
					throw new NotSupportedException ("All");
				}
#endif
				set
				{
					read = value;
					write = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new EnvironmentPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new EnvironmentPermission
						(PermissionState.None,
						 EnvironmentPermission.SplitPath(read),
						 EnvironmentPermission.SplitPath(write));
				}
			}

}; // class EnvironmentPermissionAttribute

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security.Permissions
