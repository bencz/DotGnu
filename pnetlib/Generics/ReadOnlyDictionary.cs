/*
 * ReadOnlyDictionary.cs - Wrap a dictionary to make it read-only.
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

public class ReadOnlyDictionary<KeyT, ValueT>
	: ReadOnlyCollection< DictionaryEntry<KeyT, ValueT> >,
	  IDictionary<KeyT, ValueT>
{
	// Internal state.
	protected IDictionary<KeyT, ValueT> dict;

	// Constructor.
	public ReadOnlyDictionary(IDictionary<KeyT, ValueT> dict) : base(dict)
			{
				this.dict = dict;
			}

	// Implement the IDictionary<KeyT, ValueT> interface.
	public void Add(KeyT key, ValueT value)
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public void Clear()
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public bool Contains(KeyT key)
			{
				return dict.Contains(key);
			}
	public new IDictionaryIterator<KeyT, ValueT> GetIterator()
			{
				return new ReadOnlyDictIterator<KeyT, ValueT>
					(dict.GetIterator());
			}
	public void Remove(KeyT key)
			{
				throw new InvalidOperationException(S._("NotSupp_ReadOnly"));
			}
	public ValueT this[KeyT key]
			{
				get
				{
					return dict[key];
				}
				set
				{
					throw new InvalidOperationException
						(S._("NotSupp_ReadOnly"));
				}
			}
	public ICollection<KeyT> Keys
			{
				get
				{
					return new ReadOnlyCollection<KeyT>(dict.Keys);
				}
			}
	public ICollection<ValueT> Values
			{
				get
				{
					return new ReadOnlyCollection<ValueT>(dict.Values);
				}
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				if(dict is ICloneable)
				{
					return new ReadOnlyDictionary<T>
						((IDictionary<T>)(((ICloneable)dict).Clone()));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

	// Read-only iterator class for dicionaries.
	private sealed class ReadOnlyDictIterator<KeyT, ValueT>
		: IDictionaryIterator<KeyT, ValueT>
	{
		// Internal state.
		protected IDictionaryIterator<KeyT, ValueT> iterator;

		// Constructor.
		public ReadOnlyDictIterator(IDictionaryIterator<KeyT, ValueT> iterator)
				{
					this.iterator = iterator;
				}

		// Implement the IIterator<DictionaryEntry<KeyT, ValueT>> interface.
		public bool MoveNext()
				{
					return iterator.MoveNext();
				}
		public void Reset()
				{
					iterator.Reset();
				}
		public void Remove()
				{
					throw new InvalidOperationException
						(S._("NotSupp_ReadOnly"));
				}
		public DictionaryEntry<KeyT, ValueT> Current
				{
					get
					{
						return iterator.Current;
					}
				}

		// Implement the IDictionaryIterator<KeyT, ValueT> interface.
		public KeyT Key
				{
					get
					{
						return iterator.Key;
					}
				}
		public ValueT Value
				{
					get
					{
						return iterator.Value;
					}
					set
					{
						throw new InvalidOperationException
							(S._("NotSupp_ReadOnly"));
					}
				}

	}; // class ReadOnlyDictIterator<KeyT, ValueT>

}; // class ReadOnlyDictionary<KeyT, ValueT>

}; // namespace Generics
