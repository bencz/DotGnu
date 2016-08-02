/*
 * DictionaryBase.cs - Implementation of the
 *			"System.Collections.DictionaryBase" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Collections
{

#if !ECMA_COMPAT

using System;

public abstract class DictionaryBase : IDictionary, ICollection, IEnumerable
{
	// Internal state.
	private Hashtable table;

	// Constructor.
	protected DictionaryBase()
			{
				table = new Hashtable();
			}

	// Implement the IDictionary interface.
	void IDictionary.Add(Object key, Object value)
			{
				OnValidate(key, value);
				OnInsert(key, value);
				table.Add(key, value);
				try
				{
					OnInsertComplete(key, value);
				}
				catch(Exception)
				{
					table.Remove(key);
					throw;
				}
			}
	public void Clear()
			{
				OnClear();
				table.Clear();
				OnClearComplete();
			}
	bool IDictionary.Contains(Object key)
			{
				return table.Contains(key);
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				return ((IDictionary)table).GetEnumerator();
			}
	void IDictionary.Remove(Object key)
			{
				Object value = table[key];
				OnValidate(key, value);
				OnRemove(key, value);
				table.Remove(key);
				try
				{
					OnRemoveComplete(key, value);
				}
				catch(Exception)
				{
					table[key] = value;
					throw;
				}
			}
	bool IDictionary.IsFixedSize
			{
				get
				{
					return table.IsFixedSize;
				}
			}
	bool IDictionary.IsReadOnly
			{
				get
				{
					return table.IsReadOnly;
				}
			}
	Object IDictionary.this[Object key]
			{
				get
				{
					Object value = table[key];
					OnGet(key, value);
					return value;
				}
				set
				{
					Object oldValue = table[key];
					OnSet(key, oldValue, value);
					table[key] = value;
					try
					{
						OnSetComplete(key, oldValue, value);
					}
					catch(Exception)
					{
						table[key] = oldValue;
						throw;
					}
				}
			}
	ICollection IDictionary.Keys
			{
				get
				{
					return table.Keys;
				}
			}
	ICollection IDictionary.Values
			{
				get
				{
					return table.Values;
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				table.CopyTo(array, index);
			}
	public int Count
			{
				get
				{
					return table.Count;
				}
			}
	bool ICollection.IsSynchronized
			{
				get
				{
					return table.IsSynchronized;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return table.SyncRoot;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)table).GetEnumerator();
			}

	// Get the inner hash table that is being used by this dictionary base.
	protected Hashtable InnerHashtable
			{
				get
				{
					return table;
				}
			}

	// Get this dictionary base, represented as an IDictionary.
	protected IDictionary Dictionary
			{
				get
				{
					return this;
				}
			}

	// Dictionary control methods.
	protected virtual void OnClear() {}
	protected virtual void OnClearComplete() {}
	protected virtual Object OnGet(Object key, Object currentValue)
			{
				return currentValue;
			}
	protected virtual void OnInsert(Object key, Object value) {}
	protected virtual void OnInsertComplete(Object key, Object value) {}
	protected virtual void OnRemove(Object key, Object value) {}
	protected virtual void OnRemoveComplete(Object key, Object value) {}
	protected virtual void OnSet
		(Object key, Object oldValue, Object newValue) {}
	protected virtual void OnSetComplete
		(Object key, Object oldValue, Object newValue) {}
	protected virtual void OnValidate(Object key, Object value) {}

}; // class DictionaryBase

#endif // !ECMA_COMPAT

}; // namespace System.Collections
