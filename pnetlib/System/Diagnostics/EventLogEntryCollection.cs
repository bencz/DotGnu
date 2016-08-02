/*
 * EventLogEntryCollection.cs - Implementation of the
 *			"System.Diagnostics.EventLogEntryCollection" class.
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

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.Collections;

public class EventLogEntryCollection : ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	internal EventLogEntryCollection()
			{
				list = new ArrayList();
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				list.CopyTo(array, index);
			}
	public int Count
			{
				get
				{
					return list.Count;
				}
			}
	bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Get a particular entry from this collection.
	public virtual EventLogEntry this[int index]
			{
				get
				{
					return (EventLogEntry)(list[index]);
				}
			}

	// Copy the members of this collection to an array.
	public void CopyTo(EventLogEntry[] entries, int index)
			{
				list.CopyTo(entries, index);
			}

	// Add an entry to this collection.
	internal void Add(EventLogEntry entry)
			{
				list.Add(entry);
			}

	// Clear this collection.
	internal void Clear()
			{
				list.Clear();
			}

}; // class EventLogEntryCollection

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
