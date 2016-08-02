/*
 * EventLogPermissionEntryCollection.cs - Implementation of the
 *			"System.Diagnostics.EventLogPermissionEntryCollection" class.
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

using System.Collections;

[Serializable]
public class EventLogPermissionEntryCollection : CollectionBase
{
	// Internal state.
	private EventLogPermission perm;

	// Constructor.
	internal EventLogPermissionEntryCollection(EventLogPermission perm)
			{
				this.perm = perm;
			}

	// Get or set a collection member.
	public EventLogPermissionEntry this[int index]
			{
				get
				{
					return (EventLogPermissionEntry)(((IList)this)[index]);
				}
				set
				{
					((IList)this)[index] = value;
				}
			}

	// Add an element to this collection.
	public int Add(EventLogPermissionEntry value)
			{
				return ((IList)this).Add(value);
			}
	internal void AddDirect(EventLogPermissionEntry value)
			{
				InnerList.Add(value);
			}

	// Add a range of elements to this collection.
	public void AddRange(EventLogPermissionEntry[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(EventLogPermissionEntry val in value)
				{
					Add(val);
				}
			}
	public void AddRange(EventLogPermissionEntryCollection value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(EventLogPermissionEntry val in value)
				{
					Add(val);
				}
			}

	// Determine if an item exists in this collection.
	public bool Contains(EventLogPermissionEntry value)
			{
				return ((IList)this).Contains(value);
			}

	// Copy the elements in this collection to an array.
	public void CopyTo(EventLogPermissionEntry[] array, int index)
			{
				((IList)this).CopyTo(array, index);
			}

	// Get the index of a specific element in this collection.
	public int IndexOf(EventLogPermissionEntry value)
			{
				return ((IList)this).IndexOf(value);
			}

	// Insert an element into this collection.
	public void Insert(int index, EventLogPermissionEntry value)
			{
				((IList)this).Insert(index, value);
			}

	// Remove an element from this collection.
	public void Remove(EventLogPermissionEntry value)
			{
				((IList)this).Remove(value);
			}

	// Detect when the collection is cleared.
	protected override void OnClear()
			{
				perm.Clear();
			}

	// Detect when an item is inserted.
	protected override void OnInsert(int index, Object value)
			{
				perm.Add((EventLogPermissionEntry)value);
			}

	// Detect when an item is removed.
	protected override void OnRemove(int index, Object value)
			{
				perm.Remove((EventLogPermissionEntry)value);
			}

	// Detect when an item is changed.
	protected override void OnSet(int index, Object oldValue, Object newValue)
			{
				perm.Remove((EventLogPermissionEntry)oldValue);
				perm.Add((EventLogPermissionEntry)newValue);
			}

}; // class EventLogPermissionEntryCollection

#endif // CONFIG_PERMISSIONS && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
