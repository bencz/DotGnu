/*
 * EventLogPermissionAttribute.cs - Implementation of the
 *			"System.Diagnostics.EventLogPermissionAttribute" class.
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
public class EventLogPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private String machineName;
	private EventLogPermissionAccess permissionAccess;

	// Constructors.
	public EventLogPermissionAttribute(SecurityAction action)
			: base(action)
			{
				machineName = ".";
				permissionAccess = EventLogPermissionAccess.Browse;
			}

	// Get or set the attribute's properties.
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
	public EventLogPermissionAccess PermissionAccess
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
					return new EventLogPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new EventLogPermission
						(permissionAccess, machineName);
				}
			}

}; // class EventLogPermissionAttribute

#endif // CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
