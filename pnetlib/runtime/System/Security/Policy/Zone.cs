/*
 * Zone.cs - Implementation of the
 *		"System.Security.Policy.Zone" class.
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

namespace System.Security.Policy
{

#if CONFIG_POLICY_OBJECTS

using System.Security.Permissions;

[Serializable]
public sealed class Zone
#if CONFIG_PERMISSIONS
	: IIdentityPermissionFactory
#endif
{
	// Internal state.
	private SecurityZone zone;

	// Constructor.
	public Zone(SecurityZone zone)
			{
				if(((int)zone) < ((int)(SecurityZone.NoZone)) ||
				   ((int)zone) > ((int)(SecurityZone.Untrusted)))
				{
					throw new ArgumentException(_("Arg_SecurityZone"));
				}
				this.zone = zone;
			}

	// Get the security zone value.
	public SecurityZone SecurityZone
			{
				get
				{
					return zone;
				}
			}

	// Make a copy of this object.
	public Object Copy()
			{
				return new Zone(zone);
			}

	// Create a new zone from a URL.  We assume that everything is
	// in the "Internet" zone in this implementation.
	public static Zone CreateFromUrl(String url)
			{
				if(url == null)
				{
					throw new ArgumentNullException("url");
				}
				return new Zone(SecurityZone.Internet);
			}

#if CONFIG_PERMISSIONS

	// Implement the IIdentityPermissionFactory interface
	public IPermission CreateIdentityPermission(Evidence evidence)
			{
				return new ZoneIdentityPermission(zone);
			}

#endif

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				Zone other = (obj as Zone);
				if(other != null)
				{
					return (other.zone == zone);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return ((int)zone).GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return zone.ToString();
			}

}; // class Zone

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
