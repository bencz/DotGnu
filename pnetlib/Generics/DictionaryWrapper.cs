/*
 * DictionaryWrapper.cs - Wrap a non-generic dictionary and make it generic.
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

public sealed class DictionaryWrapper<KeyT, ValueT> : IDictionary<KeyT, ValueT>
{
	// Internal state.
	private System.Collections.IDictionary dict;

	// Constructor.
	public DictionaryWrapper(System.Collections.IDictionary dict)
			{
				if(dict == null)
				{
					throw new ArgumentNullException("dict");
				}
				this.dict = dict;
			}

	// Implement the IDictionary<KeyT, ValueT> interface.
	public void Add(KeyT key, ValueT value)
			{
				dict.Add(key, value);
			}
	public void Clear()
			{
				dict.Clear();
			}
	public bool Contains(KeyT key)
			{
				return dict.Contains(key);
			}
	public IDictionaryIterator<KeyT, ValueT> GetIterator()
			{
				return new DictionaryEnumeratorWrapper<KeyT, ValueT>
						(dict.GetEnumerator());
			}
	public void Remove(KeyT key)
			{
				dict.Remove(key);
			}
	public ValueT this[KeyT key]
			{
				get
				{
					return (ValueT)(dict[key]);
				}
				set
				{
					dict[key] = value;
				}
			}
	public ICollection<KeyT> Keys
			{
				get
				{
					return new CollectionWrapper<KeyT>(dict.Keys);
				}
			}
	public ICollection<ValueT> Values
			{
				get
				{
					return new CollectionWrapper<ValueT>(dict.Values);
				}
			}

	// Implement the ICollection<DictionaryEntry<KeyT, ValueT>> interface.
	public void CopyTo(DictionaryEntry<KeyT, ValueT>[] array, int index)
			{
				dict.CopyTo(array, index);
			}
	public int Count
			{
				get
				{
					return dict.Count;
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

	// Implement the IIterable<DictionaryEntry<KeyT, ValueT>> interface.
	IIterator< DictionaryEntry<KeyT, ValueT> >
				IIterable< DictionaryEntry<KeyT, ValueT> >.GetIterator()
			{
				return new EnumeratorWrapper< DictionaryEntry<KeyT, ValueT> >
					(dict.GetEnumerator());
			}

}; // class DictionaryWrapper<KeyT, ValueT>

}; // namespace Generics
