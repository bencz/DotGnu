/*
 * EventDescriptorCollection.cs - Implementation of the
 *			"System.ComponentModel.EventDescriptorCollection" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

[ComVisible(true)]
public class EventDescriptorCollection : IList, ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Empty collection.
	public static readonly EventDescriptorCollection Empty =
			new EventDescriptorCollection(null);

	// Constructors.
	public EventDescriptorCollection(EventDescriptor[] events)
			{
				list = new ArrayList();
				if(events != null)
				{
					foreach(EventDescriptor descr in events)
					{
						list.Add(descr);
					}
				}
			}
	private EventDescriptorCollection(EventDescriptorCollection copyFrom,
									  String[] names, IComparer comparer)
			{
				list = (ArrayList)(copyFrom.list.Clone());
				InternalSort(names, comparer);
			}

	// Get the number of items in the collection.
	public int Count
			{
				get
				{
					return list.Count;
				}
			}

	// Get a specific event by index.
	public virtual EventDescriptor this[int index]
			{
				get
				{
					if(index < 0 || index >= list.Count)
					{
						throw new IndexOutOfRangeException
							(S._("Arg_InvalidArrayIndex"));
					}
					return (EventDescriptor)(list[index]);
				}
			}

	// Get a specific event by name.
	public virtual EventDescriptor this[String name]
			{
				get
				{
					return Find(name, false);
				}
			}

	// Add an descriptor to this collection.
	public int Add(EventDescriptor value)
			{
				return list.Add(value);
			}

	// Clear this collection.
	public void Clear()
			{
				list.Clear();
			}

	// Determine if this collection contains a particular descriptor.
	public bool Contains(EventDescriptor value)
			{
				return list.Contains(value);
			}

	// Find a descriptor with a specific name.
	public virtual EventDescriptor Find(String name, bool ignoreCase)
			{
				foreach(EventDescriptor descr in list)
				{
					if(String.Compare(descr.Name, name, ignoreCase,
									  CultureInfo.InvariantCulture) == 0)
					{
						return descr;
					}
				}
				return null;
			}

	// Get an enumerator for this collection.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Get the index of a specific descriptor within the collection.
	public int IndexOf(EventDescriptor value)
			{
				return list.IndexOf(value);
			}

	// Insert a descriptor into this collection.
	public void Insert(int index, EventDescriptor value)
			{
				list.Insert(index, value);
			}

	// Remove a descriptor from this collection.
	public void Remove(EventDescriptor value)
			{
				list.Remove(value);
			}

	// Remove a descriptor at a particular index within this collection.
	public void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}

	// Sort the descriptor collection.
	public virtual EventDescriptorCollection Sort()
			{
				return new EventDescriptorCollection(this, null, null);
			}
	public virtual EventDescriptorCollection Sort(IComparer comparer)
			{
				return new EventDescriptorCollection(this, null, comparer);
			}
	public virtual EventDescriptorCollection Sort(String[] names)
			{
				return new EventDescriptorCollection(this, names, null);
			}
	public virtual EventDescriptorCollection Sort
				(String[] names, IComparer comparer)
			{
				return new EventDescriptorCollection(this, names, comparer);
			}

	// Internal version of "Sort".
	private void InternalSort(String[] names, IComparer comparer)
			{
				if(comparer == null)
				{
					comparer = new TypeDescriptor.DescriptorComparer();
				}
				if(names != null && names.Length > 0)
				{
					// Copy across elements from "names" before
					// sorting the main list and appending it.
					ArrayList newList = new ArrayList(list.Count);
					foreach(String name in names)
					{
						EventDescriptor descr = Find(name, false);
						if(descr != null)
						{
							newList.Add(descr);
							list.Remove(descr);
						}
					}
					list.Sort(comparer);
					foreach(EventDescriptor ed in list)
					{
						newList.Add(ed);
					}
					list = newList;
				}
				else
				{
					// No names, so just sort the main list.
					list.Sort(comparer);
				}
			}
	protected void InternalSort(IComparer comparer)
			{
				InternalSort(null, comparer);
			}
	protected void InternalSort(String[] names)
			{
				InternalSort(names, null);
			}

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				return Add((EventDescriptor)value);
			}
	void IList.Clear()
			{
				Clear();
			}
	bool IList.Contains(Object value)
			{
				return list.Contains(value);
			}
	int IList.IndexOf(Object value)
			{
				return list.IndexOf(value);
			}
	void IList.Insert(int index, Object value)
			{
				Insert(index, (EventDescriptor)value);
			}
	void IList.Remove(Object value)
			{
				list.Remove(value);
			}
	void IList.RemoveAt(int index)
			{
				list.RemoveAt(index);
			}
	bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool IList.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	Object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					list[index] = (EventDescriptor)value;
				}
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				list.CopyTo(array, index);
			}
	int ICollection.Count
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
	IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

}; // class EventDescriptorCollection

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
