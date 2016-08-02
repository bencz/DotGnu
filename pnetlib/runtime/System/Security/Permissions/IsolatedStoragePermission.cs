/*
 * IsolatedStoragePermission.cs - Implementation of the
 *		"System.Security.Permissions.IsolatedStoragePermission" class.
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

public abstract class IsolatedStoragePermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	internal PermissionState state;
	internal long userQuota;
	internal IsolatedStorageContainment usageAllowed;

	// Constructors.
	public IsolatedStoragePermission(PermissionState state)
			{
				if(state != PermissionState.Unrestricted &&
				   state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				this.state = state;
				if(state == PermissionState.Unrestricted)
				{
					userQuota = Int64.MaxValue;
					usageAllowed =
						IsolatedStorageContainment.UnrestrictedIsolatedStorage;
				}
				else
				{
					userQuota = 0;
					usageAllowed = IsolatedStorageContainment.None;
				}
			}
	internal IsolatedStoragePermission(IsolatedStoragePermission copyFrom)
			{
				this.state = copyFrom.state;
				this.userQuota = copyFrom.userQuota;
				this.usageAllowed = copyFrom.usageAllowed;
			}
	internal IsolatedStoragePermission
					(IsolatedStoragePermissionAttribute copyFrom)
			{
				this.state = PermissionState.None;
				this.userQuota = copyFrom.userQuota;
				this.usageAllowed = copyFrom.usageAllowed;
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
			{
				String value;
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}
				value = esd.Attribute("Unrestricted");
				if(value != null && Boolean.Parse(value))
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					state = PermissionState.None;
				}
				if(state != PermissionState.Unrestricted)
				{
					value = esd.Attribute("Allowed");
					if(value != null)
					{
						usageAllowed = (IsolatedStorageContainment)
								Enum.Parse(typeof(IsolatedStorageContainment),
										   value);
					}
					else
					{
						usageAllowed = IsolatedStorageContainment.None;
					}
				}
				else
				{
					usageAllowed =
						IsolatedStorageContainment.UnrestrictedIsolatedStorage;
				}
				if(usageAllowed !=
						IsolatedStorageContainment.UnrestrictedIsolatedStorage)
				{
					value = esd.Attribute("UserQuota");
					if(value != null)
					{
						userQuota = Int64.Parse(value);
					}
					else
					{
						userQuota = 0;
					}
				}
				else
				{
					userQuota = Int64.MaxValue;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(FileIOPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
					element.AddAttribute("Allowed", usageAllowed.ToString());
				}
				else
				{
					element.AddAttribute("Allowed", usageAllowed.ToString());
					if(userQuota > 0)
					{
						element.AddAttribute("UserQuota", userQuota.ToString());
					}
				}
				return element;
			}

	// Determine if this object has unrestricted permissions.
	public bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}

	// Get or set the user's quota value.
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

	// Get or set the user's isolated storage containment area.
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

}; // class IsolatedStoragePermission

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
