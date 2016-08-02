/*
 * Collection.cs - Implementation of the
 *			"Microsoft.VisualBasic.Collection" class.
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

namespace Microsoft.VisualBasic
{

using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;

public sealed class Collection : ICollection, IList
{
	// Key mapping information.
	private sealed class KeyMap
	{
		public String key;
		public int index;
		public KeyMap next;

		public KeyMap(String key, int index, KeyMap next)
				{
					this.key = key;
					this.index = index;
					this.next = next;
				}

	}; // class KeyMap

	// Internal state.
	private ArrayList list;
	private KeyMap keys;

	// Constructor.
	public Collection()
			{
				list = new ArrayList();
				keys = null;
			}

	// Adjust all key mappings after a specific position.
	private void AdjustAfter(int index, int change)
			{
				KeyMap map = keys;
				while(map != null)
				{
					if(map.index >= index)
					{
						map.index += change;
					}
					map = map.next;
				}
			}

	// Add an item to the collection.
	public void Add(Object Item,
					[Optional] [DefaultValue(null)] String Key,
					[Optional] [DefaultValue(null)] Object Before,
					[Optional] [DefaultValue(null)] Object After)
			{
				int index;

				// Determine the index to insert at.
				if(Before != null)
				{
					if(After != null)
					{
						throw new ArgumentException
							(S._("VB_BothBeforeAndAfter"));
					}
					index = (int)Before;
					if(index < 1 || index > list.Count)
					{
						throw new ArgumentException
							(S._("VB_InvalidCollectionIndex"));
					}
					--index;
				}
				else if(After != null)
				{
					index = (int)After;
					if(index < 1 || index > list.Count)
					{
						throw new ArgumentException
							(S._("VB_InvalidCollectionIndex"));
					}
				}
				else
				{
					index = list.Count;
				}

				// Validate the key.
				if(Key != null)
				{
					KeyMap map = keys;
					while(map != null)
					{
						if(map.key == Key)
						{
							throw new ArgumentException
								(S._("VB_KeyAlreadyExists"));
						}
						map = map.next;
					}
				}

				// Insert the item into the list.
				list.Insert(index, Item);

				// Adjust all key pointers after this one.
				AdjustAfter(index, 1);

				// Add the key mapping if necessary.
				if(Key != null)
				{
					keys = new KeyMap(Key, index, keys);
				}
			}

	// Remove an item from this collection.
	public void Remove(int index)
			{
				if(index < 1 || index > list.Count)
				{
					throw new ArgumentException
						(S._("VB_InvalidCollectionIndex"));
				}
				--index;
				list.RemoveAt(index);
				AdjustAfter(index, -1);
			}
	public void Remove(String key)
			{
				if(key == null || keys == null)
				{
					throw new ArgumentException
						(S._("VB_InvalidCollectionIndex"));
				}
				KeyMap map = keys;
				KeyMap prev = null;
				while(map != null)
				{
					if(map.key == key)
					{
						if(prev != null)
						{
							prev.next = map.next;
						}
						else
						{
							keys = map.next;
						}
						int index = map.index;
						list.RemoveAt(index);
						AdjustAfter(index, -1);
						return;
					}
					prev = map;
					map = map.next;
				}
				throw new ArgumentException
					(S._("VB_InvalidCollectionIndex"));
			}

	// Get an item from the collection.
	public Object this[int index]
			{
				get
				{
					if(index < 1 || index > list.Count)
					{
						throw new ArgumentException
							(S._("VB_InvalidCollectionIndex"));
					}
					return list[index - 1];
				}
			}
	public Object this[Object index]
			{
				get
				{
					if(index is int)
					{
						return this[(int)index];
					}
					else if(index is String)
					{
						String key = (String)index;
						KeyMap map = keys;
						while(map != null)
						{
							if(map.key == key)
							{
								return list[map.index];
							}
							map = map.next;
						}
						throw new ArgumentException
							(S._("VB_InvalidCollectionIndex"));
					}
					else
					{
						throw new ArgumentException
							(S._("VB_InvalidCollectionIndex"));
					}
				}
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				IEnumerator e = GetEnumerator();
				while(e.MoveNext())
				{
					array.SetValue(e.Current, index++);
				}
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

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				return list.Add(value);
			}
	void IList.Clear()
			{
				list.Clear();
				keys = null;
			}
	bool IList.Contains(Object value)
			{
				return list.Contains(value);
			}
	int  IList.IndexOf(Object value)
			{
				return list.IndexOf(value);
			}
	void IList.Insert(int index, Object value)
			{
				list.Insert(index, value);
				AdjustAfter(index, 1);
			}
	void IList.Remove(Object value)
			{
				int index = list.IndexOf(value);
				if(index != -1)
				{
					Remove(index + 1);
				}
			}
	void IList.RemoveAt(int index)
			{
				Remove(index + 1);
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
					list[index] = value;
				}
			}

}; // class Collection

}; // namespace Microsoft.VisualBasic
