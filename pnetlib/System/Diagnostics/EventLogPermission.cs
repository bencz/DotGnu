/*
 * EventLogPermission.cs - Implementation of the
 *			"System.Diagnostics.EventLogPermission" class.
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
public sealed class EventLogPermission : ResourcePermissionBase
{
	// Constructors.
	public EventLogPermission() : this(PermissionState.None) {}
	public EventLogPermission(PermissionState state)
			: base(state)
			{
				PermissionAccessType = typeof(EventLogPermissionAccess);
				TagNames = new String [] {"Machine"};
			}
	public EventLogPermission
				(EventLogPermissionEntry[] permissionAccessEntries)
			: this(PermissionState.None)
			{
				foreach(EventLogPermissionEntry entry in
							permissionAccessEntries)
				{
					AddPermissionAccess(entry.ToResourceEntry());
				}
			}
	public EventLogPermission
				(EventLogPermissionAccess access, String machineName)
			: this(PermissionState.None)
			{
				AddPermissionAccess
					(new EventLogPermissionEntry(access, machineName)
						.ToResourceEntry());
			}

	// Get the permission entries in this collection.
	public EventLogPermissionEntryCollection PermissionEntries
			{
				get
				{
					EventLogPermissionEntryCollection coll;
					coll = new EventLogPermissionEntryCollection(this);
					ResourcePermissionBaseEntry[] entries;
					entries = GetPermissionEntries();
					foreach(EventLogPermissionEntry.EventLogResourceEntry
								entry in entries)
					{
						coll.AddDirect(entry.ToEntry());
					}
					return coll;
				}
			}

	// Helper methods for "EventLogPermissionEntryCollection".
	internal new void Clear()
			{
				base.Clear();
			}
	internal void Add(EventLogPermissionEntry entry)
			{
				AddPermissionAccess(entry.ToResourceEntry());
			}
	internal void Remove(EventLogPermissionEntry entry)
			{
				RemovePermissionAccess(entry.ToResourceEntry());
			}

}; // class EventLogPermission

#endif // CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
