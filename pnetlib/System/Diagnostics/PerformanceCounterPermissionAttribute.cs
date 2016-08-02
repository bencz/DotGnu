/*
 * PerformanceCounterPermissionAttribute.cs - Implementation of the
 *			"System.Diagnostics.PerformanceCounterPermissionAttribute" class.
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

namespace System.Diagnostics
{

#if CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

using System.Security;
using System.Security.Permissions;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly |
				AttributeTargets.Class |
				AttributeTargets.Struct |
				AttributeTargets.Constructor |
				AttributeTargets.Method |
				AttributeTargets.Event,
				AllowMultiple=true, Inherited=false)]
public class PerformanceCounterPermissionAttribute
		: CodeAccessSecurityAttribute
{
	// Internal state.
	private String machineName;
	private String categoryName;
	private PerformanceCounterPermissionAccess permissionAccess;

	// Constructors.
	public PerformanceCounterPermissionAttribute(SecurityAction action)
			: base(action)
			{
				machineName = ".";
				categoryName = "*";
				permissionAccess = PerformanceCounterPermissionAccess.Browse;
			}

	// Get or set the attribute's properties.
	public String CategoryName
			{
				get
				{
					return categoryName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					categoryName = value;
				}
			}
	public String MachineName
			{
				get
				{
					return machineName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					machineName = value;
				}
			}
	public PerformanceCounterPermissionAccess PermissionAccess
			{
				get
				{
					return permissionAccess;
				}
				set
				{
					permissionAccess = value;
				}
			}

	// Create a permission object from this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new PerformanceCounterPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new PerformanceCounterPermission
						(permissionAccess, machineName, categoryName);
				}
			}

}; // class PerformanceCounterPermissionAttribute

#endif // CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
