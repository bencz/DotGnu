/*
 * PerformanceCounterPermission.cs - Implementation of the
 *			"System.Diagnostics.PerformanceCounterPermission" class.
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
public sealed class PerformanceCounterPermission : ResourcePermissionBase
{
	// Constructors.
	public PerformanceCounterPermission() : this(PermissionState.None) {}
	public PerformanceCounterPermission(PermissionState state)
			: base(state)
			{
				PermissionAccessType =
					typeof(PerformanceCounterPermissionAccess);
				TagNames = new String [] {"Machine", "Category"};
			}
	public PerformanceCounterPermission
				(PerformanceCounterPermissionEntry[] permissionAccessEntries)
			: this(PermissionState.None)
			{
				foreach(PerformanceCounterPermissionEntry entry in
							permissionAccessEntries)
				{
					AddPermissionAccess(entry.ToResourceEntry());
				}
			}
	public PerformanceCounterPermission
				(PerformanceCounterPermissionAccess permissionAccess,
				 String machineName, String categoryName)
			: this(PermissionState.None)
			{
				AddPermissionAccess
					(new PerformanceCounterPermissionEntry
							(permissionAccess, machineName, categoryName)
						.ToResourceEntry());
			}

	// Get the permission entries in this collection.
	public PerformanceCounterPermissionEntryCollection PermissionEntries
			{
				get
				{
					PerformanceCounterPermissionEntryCollection coll;
					coll = new PerformanceCounterPermissionEntryCollection
						(this);
					ResourcePermissionBaseEntry[] entries;
					entries = GetPermissionEntries();
					foreach(PerformanceCounterPermissionEntry
								.PerformanceCounterPermissionResourceEntry
									entry in entries)
					{
						coll.AddDirect(entry.ToEntry());
					}
					return coll;
				}
			}

	// Helper methods for "PerformanceCounterPermissionEntryCollection".
	internal new void Clear()
			{
				base.Clear();
			}
	internal void Add(PerformanceCounterPermissionEntry entry)
			{
				AddPermissionAccess(entry.ToResourceEntry());
			}
	internal void Remove(PerformanceCounterPermissionEntry entry)
			{
				RemovePermissionAccess(entry.ToResourceEntry());
			}

}; // class PerformanceCounterPermission

#endif // CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
