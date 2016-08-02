/*
 * StringCollection.cs - Implementation of
 *		"System.Collections.Specialized.StringCollection".
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Specialized
{

#if !ECMA_COMPAT

using System;
using System.Collections;

public class StringCollection : IList, ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	public StringCollection()
			{
				list = new ArrayList();
			}

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				return list.Add((String)value);
			}
	public void Clear()
			{
				list.Clear();
			}
	bool IList.Contains(Object value)
			{
				if(value == null || value is String)
				{
					return list.Contains(value);
				}
				else
				{
					return false;
				}
			}
	int IList.IndexOf(Object value)
			{
				if(value == null || value is String)
				{
					return list.IndexOf(value);
				}
				else
				{
					return -1;
				}
			}
	void IList.Insert(int index, Object value)
			{
				list.Insert(index, (String)value);
			}
	void IList.Remove(Object value)
			{
				if(value == null || value is String)
				{
					list.Remove(value);
				}
			}
	public void RemoveAt(int index)
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
					return list[index];
				}
				set
				{
					list[index] = (String)value;
				}
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
	public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return list;
				}
			}

	// Determine if this collection is read-only.
	public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Access a specific item within this string collection.
	public String this[int index]
			{
				get
				{
					return (String)(list[index]);
				}
				set
				{
					list[index] = value;
				}
			}

	// Add a string to this collection.
	public int Add(String value)
			{
				return list.Add(value);
			}

	// Add a range of strings to this collection.
	public void AddRange(String[] value)
			{
				list.AddRange(value);
			}

	// Determine if this string collection contains a specific value.
	public bool Contains(String value)
			{
				return list.Contains(value);
			}

	// Copy the contents of this string collection to a string array.
	public void CopyTo(String[] array, int index)
			{
				list.CopyTo(array, index);
			}

	// Get an enumerator for this string collection.
	public StringEnumerator GetEnumerator()
			{
				return new StringEnumerator(list.GetEnumerator());
			}

	// Get the index of a specific value within this string collection.
	public int IndexOf(String value)
			{
				return list.IndexOf(value);
			}

	// Insert a string into this collection at a specific location.
	public void Insert(int index, String value)
			{
				list.Insert(index, value);
			}

	// Remove a string from this collection.
	public void Remove(String value)
			{
				list.Remove(value);
			}

}; // class StringCollection

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
