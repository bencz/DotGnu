/*
 * DictionaryAdapter.cs - Adapt a generic dictionary into a non-generic one.
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

namespace Generics
{

using System;

public sealed class DictionaryAdapter<KeyT, ValueT>
	: System.Collections.IDictionary, System.Collections.ICollection,
	  System.Collections.IEnumerable
{

	// Internal state.
	private IDictionary<KeyT, ValueT> dict;

	// Constructor.
	public DictionaryAdapter(IDictionary<KeyT, ValueT> dict)
			{
				if(dict == null)
				{
					throw new ArgumentNullException("dict");
				}
				this.dict = dict;
			}

	// Implement the non-generic IDictionary interface.
	public void Add(Object key, Object value)
			{
				if(key == null)
				{
					// Cannot use null keys with non-generic dictionaries.
					throw new ArgumentNullException("key");
				}
				else if(!(key is KeyT))
				{
					// Wrong type of key to add to this dictionary.
					throw new InvalidOperationException
						(S._("Invalid_KeyType"));
				}
				if(typeof(ValueT).IsValueType)
				{
					if(value == null || !(value is ValueT))
					{
						// Wrong type of value to add to this dictionary.
						throw new InvalidOperationException
							(S._("Invalid_ValueType"));
					}
				}
				else
				{
					if(value != null && !(value is ValueT))
					{
						// Wrong type of value to add to this dictionary.
						throw new InvalidOperationException
							(S._("Invalid_ValueType"));
					}
				}
				dict.Add((KeyT)key, (ValueT)value);
			}
	public void Clear()
			{
				dict.Clear();
			}
	public bool Contains(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				else if(key is KeyT)
				{
					return dict.Contains((KeyT)key);
				}
				else
				{
					return false;
				}
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				return new DictionaryEnumeratorAdapter(dict.GetIterator());
			}
	public void Remove(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				else if(key is KeyT)
				{
					dict.Remove((KeyT)key);
				}
			}
	public bool IsFixedSize
			{
				get
				{
					return dict.IsFixedSize;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return dict.IsReadOnly;
				}
			}
	public Object this[Object key]
			{
				get
				{
					return dict[(KeyT)key];
				}
				set
				{
					if(key == null)
					{
						// Cannot use null keys with non-generic dictionaries.
						throw new ArgumentNullException("key");
					}
					else if(!(key is KeyT))
					{
						// Wrong type of key to add to this dictionary.
						throw new InvalidOperationException
							(S._("Invalid_KeyType"));
					}
					if(typeof(ValueT).IsValueType)
					{
						if(value == null || !(value is ValueT))
						{
							// Wrong type of value to add to this dictionary.
							throw new InvalidOperationException
								(S._("Invalid_ValueType"));
						}
					}
					else
					{
						if(value != null && !(value is ValueT))
						{
							// Wrong type of value to add to this dictionary.
							throw new InvalidOperationException
								(S._("Invalid_ValueType"));
						}
					}
					dict[(KeyT)key] = (ValueT)value;
				}
			}
	public ICollection Keys
			{
				get
				{
					return new CollectionAdapter<KeyT>(dict.Keys);
				}
			}
	public ICollection Values
			{
				get
				{
					return new CollectionAdapter<ValueT>(dict.Values);
				}
			}

	// Implement the non-generic ICollection interface.
	public void CopyTo(Array array, int index)
			{
				IDictionaryIterator<KeyT, ValueT> iterator = dict.GetIterator();
				while(iterator.MoveNext())
				{
					array.SetValue(iterator.Current, index++);
				}
			}
	public int Count
			{
				get
				{
					return dict.Count;
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return dict.IsSynchronized;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return dict.SyncRoot;
				}
			}

	// Implement the non-generic IEnumerable interface.
	System.Collections.IEnumerator
				System.Collections.IEnumerable.GetEnumerator()
			{
				return new EnumeratorAdapter< DictionaryEntry<KeyT, ValueT> >
					(dict.GetIterator());
			}

}; // class DictionaryAdapter<KeyT, ValueT>

}; // namespace Generics
