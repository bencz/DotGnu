/*
 * PropertyDescriptorCollection.cs - Implementation of the
 *			"System.ComponentModel.PropertyDescriptorCollection" class.
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

public class PropertyDescriptorCollection
		: IList, ICollection, IEnumerable, IDictionary
{
	// Internal state.
	private ArrayList list;

	// Empty collection.
	public static readonly PropertyDescriptorCollection Empty =
			new PropertyDescriptorCollection(null);

	// Constructors.
	public PropertyDescriptorCollection(PropertyDescriptor[] properties)
			{
				list = new ArrayList();
				if(properties != null)
				{
					foreach(PropertyDescriptor descr in properties)
					{
						list.Add(descr);
					}
				}
			}
	private PropertyDescriptorCollection(PropertyDescriptorCollection copyFrom,
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

	// Get a specific property by index.
	public virtual PropertyDescriptor this[int index]
			{
				get
				{
					if(index < 0 || index >= list.Count)
					{
						throw new IndexOutOfRangeException
							(S._("Arg_InvalidArrayIndex"));
					}
					return (PropertyDescriptor)(list[index]);
				}
			}

	// Get a specific property by name.
	public virtual PropertyDescriptor this[String name]
			{
				get
				{
					return Find(name, false);
				}
			}

	// Add an descriptor to this collection.
	public int Add(PropertyDescriptor value)
			{
				return list.Add(value);
			}

	// Clear this collection.
	public void Clear()
			{
				list.Clear();
			}

	// Determine if this collection contains a particular descriptor.
	public bool Contains(PropertyDescriptor value)
			{
				return list.Contains(value);
			}

	// Find a descriptor with a specific name.
	public virtual PropertyDescriptor Find(String name, bool ignoreCase)
			{
				foreach(PropertyDescriptor descr in list)
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
	public virtual IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Get the index of a specific descriptor within the collection.
	public int IndexOf(PropertyDescriptor value)
			{
				return list.IndexOf(value);
			}

	// Insert a descriptor into this collection.
	public void Insert(int index, PropertyDescriptor value)
			{
				list.Insert(index, value);
			}

	// Remove a descriptor from this collection.
	public void Remove(PropertyDescriptor value)
			{
				list.Remove(value);
			}

	// Remove a descriptor at a particular index within this collection.
	public void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}

	// Sort the descriptor collection.
	public virtual PropertyDescriptorCollection Sort()
			{
				return new PropertyDescriptorCollection(this, null, null);
			}
	public virtual PropertyDescriptorCollection Sort(IComparer comparer)
			{
				return new PropertyDescriptorCollection(this, null, comparer);
			}
	public virtual PropertyDescriptorCollection Sort(String[] names)
			{
				return new PropertyDescriptorCollection(this, names, null);
			}
	public virtual PropertyDescriptorCollection Sort
				(String[] names, IComparer comparer)
			{
				return new PropertyDescriptorCollection(this, names, comparer);
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
						PropertyDescriptor descr = Find(name, false);
						if(descr != null)
						{
							newList.Add(descr);
							list.Remove(descr);
						}
					}
					list.Sort(comparer);
					foreach(PropertyDescriptor ed in list)
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
				return Add((PropertyDescriptor)value);
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
				Insert(index, (PropertyDescriptor)value);
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
					list[index] = (PropertyDescriptor)value;
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
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

	// Implement the IDictionary interface.
	void IDictionary.Add(Object key, Object value)
			{
				if(value is PropertyDescriptor)
				{
					Add((PropertyDescriptor)value);
				}
				else
				{
					throw new ArgumentException
						(S._("Arg_InvalidElement"));
				}
			}
	void IDictionary.Clear()
			{
				Clear();
			}
	bool IDictionary.Contains(Object key)
			{
				if(key is String)
				{
					return (Find((String)key, false) != null);
				}
				else
				{
					return false;
				}
			}
	IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return new Enumerator(GetEnumerator());
			}
	void IDictionary.Remove(Object key)
			{
				if(key is String)
				{
					PropertyDescriptor descr = Find((String)key, false);
					if(descr != null)
					{
						Remove(descr);
					}
				}
			}
	bool IDictionary.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool IDictionary.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	Object IDictionary.this[Object key]
			{
				get
				{
					if(key is String)
					{
						return Find((String)key, false);
					}
					else
					{
						return null;
					}
				}
				set
				{
					((IDictionary)this).Add(key, value);
				}
			}
	ICollection IDictionary.Keys
			{
				get
				{
					String[] keys = new String [list.Count];
					int posn = 0;
					foreach(PropertyDescriptor descr in list)
					{
						keys[posn++] = descr.Name;
					}
					return keys;
				}
			}
	ICollection IDictionary.Values
			{
				get
				{
					PropertyDescriptor[] values;
					values = new PropertyDescriptor [list.Count];
					int posn = 0;
					foreach(PropertyDescriptor descr in list)
					{
						values[posn++] = descr;
					}
					return values;
				}
			}

	// Dictionary enumerator for property descriptor collections.
	private sealed class Enumerator : IDictionaryEnumerator
	{
		// Internal state.
		private IEnumerator e;

		// Constructor.
		public Enumerator(IEnumerator e)
				{
					this.e = e;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					return e.MoveNext();
				}
		public void Reset()
				{
					e.Reset();
				}
		public Object Current
				{
					get
					{
						return Entry;
					}
				}

		// Implement the IDictionaryEnumerator interface.
		public DictionaryEntry Entry
				{
					get
					{
						PropertyDescriptor descr;
						descr = (PropertyDescriptor)(e.Current);
						return new DictionaryEntry(descr.Name, descr);
					}
				}
		public Object Key
				{
					get
					{
						return ((PropertyDescriptor)(e.Current)).Name;
					}
				}
		public Object Value
				{
					get
					{
						return (PropertyDescriptor)(e.Current);
					}
				}

	}; // class IDictionaryEnumerator

}; // class PropertyDescriptorCollection

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
