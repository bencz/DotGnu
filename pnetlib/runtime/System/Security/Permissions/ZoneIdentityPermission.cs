/*
 * ZoneIdentityPermission.cs - Implementation of the
 *		"System.Security.Permissions.ZoneIdentityPermission" class.
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

public sealed class ZoneIdentityPermission : CodeAccessPermission
{
	// Internal state.
	private PermissionState state;
	private SecurityZone zone;

	// Constructor.
	public ZoneIdentityPermission(PermissionState state)
			{
				if(state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				zone = SecurityZone.NoZone;
			}
	public ZoneIdentityPermission(SecurityZone zone)
			{
				this.zone = zone;
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
				value = esd.Attribute("Zone");
				if(value != null)
				{
					zone = (SecurityZone)
						Enum.Parse(typeof(SecurityZone), value);
				}
				else
				{
					zone = SecurityZone.NoZone;
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(ZoneIdentityPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddAttribute("Zone", zone.ToString());
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new ZoneIdentityPermission(zone);
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is ZoneIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				SecurityZone otherZone = ((ZoneIdentityPermission)target).zone;
				if(zone != otherZone)
				{
					return null;
				}
				else
				{
					return Copy();
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (zone == SecurityZone.NoZone);
				}
				else if(!(target is ZoneIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(zone == SecurityZone.NoZone)
				{
					return true;
				}
				else
				{
					return (zone == ((ZoneIdentityPermission)target).zone);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is ZoneIdentityPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				SecurityZone otherZone = ((ZoneIdentityPermission)target).zone;
				if(zone == otherZone || otherZone == SecurityZone.NoZone)
				{
					return Copy();
				}
				else if(zone == SecurityZone.NoZone)
				{
					return target.Copy();
				}
				else
				{
					return null;
				}
			}

	// Get or set the security zone on this permissions object.
	public SecurityZone SecurityZone
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

}; // class ZoneIdentityPermission

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
