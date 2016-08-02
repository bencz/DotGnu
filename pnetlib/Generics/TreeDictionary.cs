/*
 * TreeDictionary.cs - Generic dictionary class, implemented as a tree.
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

public sealed class TreeDictionary<KeyT, ValueT>
	: TreeBase<KeyT, ValueT>, IDictionary<KeyT, ValueT>, ICloneable
{
	// Constructor.
	public TreeDictionary() : base(null) {}
	public TreeDictionary(IComparer<KeyT> cmp) : base(cmp) {}

	// Implement the IDictionary<KeyT, ValueT> interface.
	public void Add(KeyT key, ValueT value)
			{
				AddItem(key, value, true);
			}
	public void Clear()
			{
				ClearAllItems();
			}
	public bool Contains(KeyT key)
			{
				return ContainsItem(key);
			}
	public IDictionaryIterator<KeyT, ValueT> GetIterator()
			{
				return new TreeDictionaryIterator<KeyT, ValueT>
					(GetInOrderIterator());
			}
	public void Remove(KeyT key)
			{
				RemoveItem(key);
			}
	public ValueT this[KeyT key]
			{
				get
				{
					return LookupItem(key);
				}
				set
				{
					AddItem(key, value, false);
				}
			}
	public ICollection<KeyT> Keys
			{
				get
				{
					return new TreeKeyCollection<KeyT, ValueT>(this);
				}
			}
	public ICollection<ValueT> Values
			{
				get
				{
					return new TreeValueCollection<KeyT, ValueT>(this);
				}
			}

	// Implement the ICollection<DictionaryEntry<KeyT, ValueT>> interface.
	public void CopyTo(DictionaryEntry<KeyT, ValueT>[] array, int index)
			{
				TreeBase.TreeIterator<KeyT, ValueT> iterator;
				iterator = GetInOrderIterator();
				while(iterator.MoveNext())
				{
					array[index++] = new DictionaryEntry<KeyT, ValueT>
							(iterator.Key, iterator.Value);
				}
			}
	public int Count
			{
				get
				{
					return count;
				}
			}
	public bool IsFixedSize
			{
				get
				{
					return false;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return false;
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
					return this;
				}
			}

	// Implement the IIterable<DictionaryEntry<KeyT, ValueT>> interface.
	IIterator< DictionaryEntry<KeyT, ValueT> >
				IIterable< DictionaryEntry<KeyT, ValueT> >.GetIterator()
			{
				return GetIterator();
			}

	// Get the in-order dictionary iterator for the key and value collections.
	// Needed to get around the "protected" permissions in "TreeBase".
	private TreeBase.TreeBaseIterator<KeyT, ValueT> GetDictIterator()
			{
				return GetInOrderIterator();
			}

	// Iterator class for tree dictionaries.
	private sealed class TreeDictionaryIterator<KeyT, ValueT>
			: IDictionaryIterator<KeyT, ValueT>
	{
		// Internal state.
		private TreeBase.TreeBaseIterator<KeyT, ValueT> iterator;

		// Constructor.
		public TreeDictionaryIterator
					(TreeBase.TreeBaseIterator<KeyT, ValueT> iterator)
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
					iterator.Remove();
				}
		public DictionaryEntry<KeyT, ValueT> Current
				{
					get
					{
						return new DictionaryEntry<KeyT, ValueT>
							(iterator.Key, iterator.Value);
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
						iterator.Value = value;
					}
				}

	}; // class TreeDictionaryIterator<KeyT, ValueT>

	// Key iterator class for tree dictionaries.
	private sealed class TreeKeyIterator<KeyT, ValueT> : IIterator<KeyT>
	{
		// Internal state.
		private TreeBase.TreeBaseIterator<KeyT, ValueT> iterator;

		// Constructor.
		public TreeKeyIterator
					(TreeBase.TreeBaseIterator<KeyT, ValueT> iterator)
				{
					this.iterator = iterator;
				}

		// Implement the IIterator<KeyT> interface.
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
					iterator.Remove();
				}
		public KeyT Current
				{
					get
					{
						return iterator.Key;
					}
				}

	}; // class TreeKeyIterator<KeyT, ValueT>

	// Value iterator class for tree dictionaries.
	private sealed class TreeValueIterator<KeyT, ValueT> : IIterator<ValueT>
	{
		// Internal state.
		private TreeBase.TreeBaseIterator<KeyT, ValueT> iterator;

		// Constructor.
		public TreeValueIterator
					(TreeBase.TreeBaseIterator<KeyT, ValueT> iterator)
				{
					this.iterator = iterator;
				}

		// Implement the IIterator<ValueT> interface.
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
					iterator.Remove();
				}
		public ValueT Current
				{
					get
					{
						return iterator.Value;
					}
				}

	}; // class TreeValueIterator<KeyT, ValueT>

	// Collection of keys in a tree dictionary.
	private sealed class TreeKeyCollection<KeyT, ValueT> : ICollection<KeyT>
	{
		// Internal state.
		private TreeDictionary<KeyT, ValueT> dict;

		// Constructor.
		public TreeKeyCollection(TreeDictionary<KeyT, ValueT> dict)
				{
					this.dict = dict;
				}

		// Implement the ICollection interface.
		public void CopyTo(KeyT[] array, int index)
				{
					IIterator<KeyT> iterator = GetIterator();
					while(iterator.MoveNext())
					{
						array[index++] = iterator.Current;
					}
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

		// Implement the IIterable<KeyT> interface.
		public IIterator<KeyT> GetIterator()
				{
					return new TreeKeyIterator<KeyT, ValueT>
						(dict.GetDictIterator());
				}

	}; // class TreeKeyCollection<KeyT, ValueT>

	// Collection of values in a tree dictionary.
	private sealed class TreeValueCollection<KeyT, ValueT>
			: ICollection<ValueT>
	{
		// Internal state.
		private TreeDictionary<KeyT, ValueT> dict;

		// Constructor.
		public TreeValueCollection(TreeDictionary<KeyT, ValueT> dict)
				{
					this.dict = dict;
				}

		// Implement the ICollection interface.
		public void CopyTo(ValueT[] array, int index)
				{
					IIterator<ValueT> iterator = GetIterator();
					while(iterator.MoveNext())
					{
						array[index++] = iterator.Current;
					}
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

		// Implement the IIterable<ValueT> interface.
		public IIterator<ValueT> GetIterator()
				{
					return new TreeValueIterator<KeyT, ValueT>
						(dict.GetDictIterator());
				}

	}; // class TreeValueCollection<KeyT, ValueT>

}; // class TreeDictionary<T>

}; // namespace Generics
