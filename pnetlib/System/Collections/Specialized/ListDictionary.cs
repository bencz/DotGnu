/*
 * ListDictionary.cs - Implementation of
 *		"System.Collections.Specialized.ListDictionary".
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

public class ListDictionary : IDictionary, ICollection, IEnumerable
{
	// Internal state.
	private IComparer comparer;
	private ListStorage list;
	private int count;
	private int generation;

	// Constructors.
	public ListDictionary() : this(null) {}
	public ListDictionary(IComparer comparer)
			{
				if(comparer != null)
				{
					this.comparer = comparer;
				}
				else
				{
					this.comparer = Comparer.Default;
				}
				this.list = null;
				this.count = 0;
				this.generation = 0;
			}

	// Common function for add and set operations.
	private void SetOrAdd(Object key, Object value, bool set)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				ListStorage current = list;
				ListStorage prev = null;
				while(current != null)
				{
					if(comparer.Compare(current.key, key) == 0)
					{
						if(set)
						{
							return;
						}
						throw new ArgumentException(S._("Arg_ExistingEntry"));
					}
					prev = current;
					current = current.next;
				}
				if(prev != null)
				{
					prev.next = new ListStorage(key, value);
				}
				else
				{
					list = new ListStorage(key, value);
				}
				++count;
				++generation;
			}

	// Implement the IDictionary interface.
	public void Add(Object key, Object value)
			{
				SetOrAdd(key, value, false);
			}

	public void Clear()
			{
				list = null;
				count = 0;
				++generation;
			}
	public bool Contains(Object key)
			{
				ListStorage current = list;
				while(current != null)
				{
					if(comparer.Compare(current.key, key) == 0)
					{
						return true;
					}
					current = current.next;
				}
				return false;
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				return new ListEnumerator(this);
			}
	public void Remove(Object key)
			{
				ListStorage current = list;
				ListStorage prev = null;
				while(current != null)
				{
					if(comparer.Compare(current.key, key) == 0)
					{
						if(prev != null)
						{
							prev.next = current.next;
						}
						else
						{
							list = current.next;
						}
						--count;
						++generation;
						return;
					}
					prev = current;
					current = current.next;
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
	public Object this[Object key]
			{
				get
				{
					ListStorage current = list;
					while(current != null)
					{
						if(comparer.Compare(current.key, key) == 0)
						{
							return current.value;
						}
						current = current.next;
					}
					return null;
				}
				set
				{
					SetOrAdd(key, value, true);
				}
			}
	public ICollection Keys
			{
				get
				{
					return new ListMemberCollection(this, true);
				}
			}
	public ICollection Values
			{
				get
				{
					return new ListMemberCollection(this, false);
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				else if(array.Rank != 1)
				{
					throw new ArgumentException(S._("Arg_RankMustBe1"));
				}
				else if(index < array.GetLowerBound(0))
				{
					throw new ArgumentOutOfRangeException
						("index", S._("Arg_InvalidArrayIndex"));
				}
				else if(index > (array.GetLength(0) - count))
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
				else
				{
					ListStorage current = list;
					while(current != null)
					{
						array.SetValue
							(new DictionaryEntry(current.key,
												 current.value),
							 index++);
						current = current.next;
					}
				}
			}
	public int Count
			{
				get
				{
					return count;
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

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return new ListEnumerator(this);
			}

	// Storage node for the dictionary.
	private sealed class ListStorage
	{
		// Accessible state.
		public ListStorage next;
		public Object key;
		public Object value;

		// Constructor.
		public ListStorage(Object key, Object value)
				{
					this.next = null;
					this.key = key;
					this.value = value;
				}

	}; // class ListStorage

	// Enumerator for this list.
	private class ListEnumerator : IDictionaryEnumerator
	{
		// Internal state.
		private ListDictionary list;
		private int generation;
		private ListStorage current;
		private bool atEnd;

		// Constructor.
		public ListEnumerator(ListDictionary list)
				{
					this.list = list;
					this.generation = list.generation;
					this.current = null;
					this.atEnd = false;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(generation != this.generation)
					{
						throw new InvalidOperationException
							(S._("Invalid_CollectionModified"));
					}
					if(atEnd)
					{
						return false;
					}
					else if(current == null)
					{
						current = list.list;
					}
					else
					{
						current = current.next;
					}
					atEnd = (current == null);
					return !atEnd;
				}
		public void Reset()
				{
					if(generation != this.generation)
					{
						throw new InvalidOperationException
							(S._("Invalid_CollectionModified"));
					}
					current = null;
					atEnd = false;
				}
		public virtual Object Current
				{
					get
					{
						return (Object)Entry;
					}
				}

		// Implement the IDictionaryEnumerator interface.
		public DictionaryEntry Entry
				{
					get
					{
						if(generation != this.generation)
						{
							throw new InvalidOperationException
								(S._("Invalid_CollectionModified"));
						}
						if(current == null)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadEnumeratorPosition"));
						}
						return new DictionaryEntry(current.key, current.value);
					}
				}
		public Object Key
				{
					get
					{
						if(generation != this.generation)
						{
							throw new InvalidOperationException
								(S._("Invalid_CollectionModified"));
						}
						if(current == null)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadEnumeratorPosition"));
						}
						return current.key;
					}
				}
		public Object Value
				{
					get
					{
						if(generation != this.generation)
						{
							throw new InvalidOperationException
								(S._("Invalid_CollectionModified"));
						}
						if(current == null)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadEnumeratorPosition"));
						}
						return current.value;
					}
				}

	}; // ListEnumerator

	// Private collection class for accessing keys and values.
	private sealed class ListMemberCollection : ICollection, IEnumerable
	{
		// Internal state.
		private ListDictionary list;
		private bool wantKeys;

		// Constructor.
		public ListMemberCollection(ListDictionary list, bool wantKeys)
				{
					this.list = list;
					this.wantKeys = wantKeys;
				}

		// Implement the ICollection interface.
		public void CopyTo(Array array, int index)
				{
					if(array == null)
					{
						throw new ArgumentNullException("array");
					}
					else if(array.Rank != 1)
					{
						throw new ArgumentException(S._("Arg_RankMustBe1"));
					}
					else if(index < array.GetLowerBound(0))
					{
						throw new ArgumentOutOfRangeException
							("index", S._("Arg_InvalidArrayIndex"));
					}
					else if(index > (array.GetLength(0) - list.Count))
					{
						throw new ArgumentException
							(S._("Arg_InvalidArrayRange"));
					}
					else
					{
						ListStorage current = list.list;
						while(current != null)
						{
							if(wantKeys)
							{
								array.SetValue(current.key, index++);
							}
							else
							{
								array.SetValue(current.value, index++);
							}
							current = current.next;
						}
					}
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
						return list.IsSynchronized;
					}
				}
		public Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return new ListMemberEnumerator(list, wantKeys);
				}

	}; // ListMemberCollection

	// Enumerate over a ListMemberCollection.
	private sealed class ListMemberEnumerator : ListEnumerator
	{
		// Internal state.
		private bool wantKeys;

		// Constructor.
		public ListMemberEnumerator(ListDictionary list, bool wantKeys)
				: base(list)
				{
					this.wantKeys = wantKeys;
				}

		// Override enumerator methods from the base class.
		public override Object Current
				{
					get
					{
						if(wantKeys)
						{
							return Entry.Key;
						}
						else
						{
							return Entry.Value;
						}
					}
				}

	}; // ListMemberEnumerator

}; // class ListDictionary

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
