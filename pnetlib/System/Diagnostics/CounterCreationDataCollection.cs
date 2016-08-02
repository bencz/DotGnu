/*
 * CounterCreationDataCollection.cs - Implementation of the
 *			"System.Diagnostics.CounterCreationDataCollection" class.
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

[Serializable]
public class CounterCreationDataCollection : CollectionBase
{
	// Constructor.
	public CounterCreationDataCollection()
			{
				// Nothing to do here.
			}
	public CounterCreationDataCollection(CounterCreationData[] value)
			{
				AddRange(value);
			}
	public CounterCreationDataCollection(CounterCreationDataCollection value)
			{
				AddRange(value);
			}

	// Get or set a collection member.
	public CounterCreationData this[int index]
			{
				get
				{
					return (CounterCreationData)(((IList)this)[index]);
				}
				set
				{
					((IList)this)[index] = value;
				}
			}

	// Add an element to this collection.
	public int Add(CounterCreationData value)
			{
				return ((IList)this).Add(value);
			}

	// Add a range of elements to this collection.
	public void AddRange(CounterCreationData[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(CounterCreationData val in value)
				{
					Add(val);
				}
			}
	public void AddRange(CounterCreationDataCollection value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(CounterCreationData val in value)
				{
					Add(val);
				}
			}

	// Determine if an item exists in this collection.
	public bool Contains(CounterCreationData value)
			{
				return ((IList)this).Contains(value);
			}

	// Copy the elements in this collection to an array.
	public void CopyTo(CounterCreationData[] array, int index)
			{
				((IList)this).CopyTo(array, index);
			}

	// Get the index of a specific element in this collection.
	public int IndexOf(CounterCreationData value)
			{
				return ((IList)this).IndexOf(value);
			}

	// Insert an element into this collection.
	public void Insert(int index, CounterCreationData value)
			{
				((IList)this).Insert(index, value);
			}

	// Remove an element from this collection.
	public virtual void Remove(CounterCreationData value)
			{
				((IList)this).Remove(value);
			}

	// Detect when an item is inserted.
	protected override void OnInsert(int index, Object value)
			{
				// Nothing to do here: other implementations check
				// for a maximum collection size.  We support unlimited
				// sized collections in this implementation.
			}

}; // class CounterCreationDataCollection

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
