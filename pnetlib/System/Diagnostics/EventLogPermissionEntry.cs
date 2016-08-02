/*
 * EventLogPermissionEntry.cs - Implementation of the
 *			"System.Diagnostics.EventLogPermissionEntry" class.
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

using System.Security.Permissions;

[Serializable]
public class EventLogPermissionEntry
{
	// Internal state.
	private EventLogPermissionAccess access;
	private String machineName;
	private EventLogResourceEntry resourceEntry;

	// Constructor.
	public EventLogPermissionEntry(EventLogPermissionAccess access,
								   String machineName)
			{
				this.access = access;
				this.machineName = machineName;
				resourceEntry = new EventLogResourceEntry
					(this, (int)access, new String [] {machineName});
			}

	// Get this object's properties.
	public String MachineName
			{
				get
				{
					return machineName;
				}
			}
	public EventLogPermissionAccess PermissionAccess
			{
				get
				{
					return access;
				}
			}

	// Convert this object into a resource entry.
	internal ResourcePermissionBaseEntry ToResourceEntry()
			{
				return resourceEntry;
			}

	// Resource wrapper class.
	internal class EventLogResourceEntry : ResourcePermissionBaseEntry
	{
		// Internal state.
		private EventLogPermissionEntry entry;

		// Constructor.
		public EventLogResourceEntry(EventLogPermissionEntry entry,
									 int access, String[] path)
				: base(access, path)
				{
					this.entry = entry;
				}

		// Convert this object into an event log permission entry.
		public EventLogPermissionEntry ToEntry()
				{
					return entry;
				}

	}; // class EventLogResourceEntry

}; // class EventLogPermissionEntry

#endif // CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
