/*
 * SynchronizedDictionary.cs - Wrap a dictionary to make it synchronized.
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

public class SynchronizedDictionary<KeyT, ValueT>
	: SynchronizedCollection< DictionaryEntry<KeyT, ValueT> >,
	  IDictionary<KeyT, ValueT>
{
	// Internal state.
	protected IDictionary<KeyT, ValueT> dict;

	// Constructor.
	public SynchronizedDictionary(IDictionary<KeyT, ValueT> dict) : base(dict)
			{
				this.dict = dict;
			}

	// Implement the IDictionary<KeyT, ValueT> interface.
	public void Add(KeyT key, ValueT value)
			{
				lock(SyncRoot)
				{
					dict.Add(key, value);
				}
			}
	public void Clear()
			{
				lock(SyncRoot)
				{
					dict.Clear();
				}
			}
	public bool Contains(KeyT key)
			{
				lock(SyncRoot)
				{
					return dict.Contains(key);
				}
			}
	public new IDictionaryIterator<KeyT, ValueT> GetIterator()
			{
				lock(SyncRoot)
				{
					return new SynchronizedDictIterator<KeyT, ValueT>
							(dict, dict.GetIterator());
				}
			}
	public void Remove(KeyT key)
			{
				lock(SyncRoot)
				{
					dict.Remove(key);
				}
			}
	public ValueT this[KeyT key]
			{
				get
				{
					lock(SyncRoot)
					{
						return dict[key];
					}
				}
				set
				{
					lock(SyncRoot)
					{
						dict[key] = value;
					}
				}
			}
	public ICollection<KeyT> Keys
			{
				get
				{
					lock(SyncRoot)
					{
						return new SynchronizedCollection<KeyT>(dict.Keys);
					}
				}
			}
	public ICollection<ValueT> Values
			{
				get
				{
					lock(SyncRoot)
					{
						return new SynchronizedCollection<ValueT>(dict.Values);
					}
				}
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				lock(SyncRoot)
				{
					if(dict is ICloneable)
					{
						return new SynchronizedDictionary<T>
							((IDictionary<T>)(((ICloneable)dict).Clone()));
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_NotCloneable"));
					}
				}
			}

	// Synchronized dictionary iterator.
	private sealed class SynchronizedDictIterator<KeyT, ValueT>
		: IDictionaryIterator<KeyT, ValueT>
	{
		// Internal state.
		protected IDictionary<KeyT, ValueT> dict;
		protected IDictionaryIterator<KeyT, ValueT> iterator;

		// Constructor.
		public SynchronizedDictIterator
					(IDictionary<KeyT, ValueT> dict,
					 IDictionaryIterator<KeyT, ValueT> iterator)
				{
					this.dict = dict;
					this.iterator = iterator;
				}

		// Implement the IIterator<ValueT> interface.
		public bool MoveNext()
				{
					lock(dict.SyncRoot)
					{
						return iterator.MoveNext();
					}
				}
		public void Reset()
				{
					lock(dict.SyncRoot)
					{
						iterator.Reset();
					}
				}
		public void Remove()
				{
					lock(dict.SyncRoot)
					{
						iterator.Remove();
					}
				}
		public DictionaryEntry<KeyT, ValueT> Current
				{
					get
					{
						lock(dict.SyncRoot)
						{
							return iterator.Current;
						}
					}
				}

		// Implement the IDictionaryIterator<KeyT, ValueT> interface.
		public KeyT Key
				{
					get
					{
						lock(dict.SyncRoot)
						{
							return iterator.Key;
						}
					}
				}
		public ValueT Value
				{
					get
					{
						lock(dict.SyncRoot)
						{
							return iterator.Value;
						}
					}
					set
					{
						lock(dict.SyncRoot)
						{
							iterator.Value = value;
						}
					}
				}

	}; // class SynchronizedDictIterator<KeyT, ValueT>

}; // class SynchronizedDictionary<KeyT, ValueT>

}; // namespace Generics
