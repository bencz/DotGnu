/*
 * ReflectionPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.ReflectionPermissionAttribute" class.
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

#if CONFIG_PERMISSIONS && CONFIG_REFLECTION

using System;
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class ReflectionPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private ReflectionPermissionFlag flags;

	// Constructors.
	public ReflectionPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the reflection permission flags.
	public ReflectionPermissionFlag Flags
			{
				get
				{
					return flags;
				}
				set
				{
					flags = value;
				}
			}

#if !ECMA_COMPAT

	// Get or set specific flags.
	public bool MemberAccess
			{
				get
				{
					return ((flags & ReflectionPermissionFlag.MemberAccess)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= ReflectionPermissionFlag.MemberAccess;
					}
					else
					{
						flags &= ~ReflectionPermissionFlag.MemberAccess;
					}
				}
			}
	public bool ReflectionEmit
			{
				get
				{
					return ((flags & ReflectionPermissionFlag.ReflectionEmit)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= ReflectionPermissionFlag.ReflectionEmit;
					}
					else
					{
						flags &= ~ReflectionPermissionFlag.ReflectionEmit;
					}
				}
			}
	public bool TypeInformation
			{
				get
				{
					return ((flags & ReflectionPermissionFlag.TypeInformation)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= ReflectionPermissionFlag.TypeInformation;
					}
					else
					{
						flags &= ~ReflectionPermissionFlag.TypeInformation;
					}
				}
			}

#endif // !ECMA_COMPAT

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new ReflectionPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new ReflectionPermission(flags);
				}
			}

}; // class ReflectionPermissionAttribute

#endif // CONFIG_PERMISSIONS && CONFIG_REFLECTION

}; // namespace System.Security.Permissions
