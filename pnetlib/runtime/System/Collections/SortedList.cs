/*
 * SortedList.cs - Implementation of the "System.Collections.SortedList" class.
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
using System.Private;

public class SortedList : IDictionary, ICollection, IEnumerable, ICloneable
{
	// Internal state.
	private IComparer comparer;
	private Object[]  keys;
	private Object[]  values;
	private int       count;
	private int       generation;
	private IList	  keyList;
	private IList	  valueList;

	// The default capacity of a sorted list.
	private const int DefaultCapacity = 16;

	// Constructors.
	public SortedList()
			{
				comparer = null;
				keys = new Object [DefaultCapacity];
				values = new Object [DefaultCapacity];
				count = 0;
				generation = 0;
				keyList = null;
				valueList = null;
			}
	public SortedList(IComparer comparer)
			{
				this.comparer = comparer;
				keys = new Object [DefaultCapacity];
				values = new Object [DefaultCapacity];
				count = 0;
				generation = 0;
				keyList = null;
				valueList = null;
			}
	public SortedList(IDictionary d)
			: this(d, null)
			{
				// Nothing to do here.
			}
	public SortedList(int initialCapacity)
			{
				if(initialCapacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("initialCapacity", _("ArgRange_NonNegative"));
				}
				comparer = null;
				keys = new Object [initialCapacity];
				values = new Object [initialCapacity];
				count = 0;
				generation = 0;
				keyList = null;
				valueList = null;
			}
	public SortedList(IComparer comparer, int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				this.comparer = comparer;
				keys = new Object [capacity];
				values = new Object [capacity];
				count = 0;
				generation = 0;
				keyList = null;
				valueList = null;
			}
	public SortedList(IDictionary d, IComparer comparer)
			{
				this.comparer = comparer;
				int capacity = d.Count;
				keys = new Object [capacity];
				values = new Object [capacity];
				count = 0;
				generation = 0;
				keyList = null;
				valueList = null;
				IDictionaryEnumerator e = d.GetEnumerator();
				while(e.MoveNext())
				{
					Add(e.Key, e.Value);
				}
			}

	// Find the location of a specific key.  Returns a negative
	// value if the key is not present.
	private int FindKey(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				return Array.BinarySearch(keys, 0, count, key, comparer);
			}

	// Insert a new entry at a particular index.
	private void InsertAt(int index, Object key, Object value)
			{
				if(count < keys.Length)
				{
					// We have enough room in the list for the entry.
					int posn = count;
					while(posn > index)
					{
						keys[posn]   = keys[posn - 1];
						values[posn] = values[posn - 1];
						--posn;
					}
				}
				else
				{
					// We need to expand the list capacity to make room.
					int newCapacity = keys.Length * 2;
					if(newCapacity < DefaultCapacity)
					{
						newCapacity = DefaultCapacity;
					}
					Object[] newKeys   = new Object [newCapacity];
					Object[] newValues = new Object [newCapacity];
					if(index > 0)
					{
						Array.Copy(keys, 0, newKeys, 0, index);
						Array.Copy(values, 0, newValues, 0, index);
					}
					if((count - index) > 0)
					{
						Array.Copy(keys, index, newKeys,
								   index + 1, count - index);
						Array.Copy(values, index, newValues,
								   index + 1, count - index);
					}
					keys   = newKeys;
					values = newValues;
				}
				keys[index]   = key;
				values[index] = value;
				++count;
				++generation;
			}

	// Remove an entry from a particular index.
	private void DoRemoveAt(int index)
			{
				int posn;
				for(posn = index; posn < (count - 1); ++posn)
				{
					keys[posn]   = keys[posn + 1];
					values[posn] = values[posn + 1];
				}
				// remove last reference to avoid memory leak
				keys[posn]   = null;
				values[posn] = null;
				
				--count;
				++generation;
			}

	// Implement the IDictionary interface.
	public virtual void Add(Object key, Object value)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				int index = FindKey(key);
				if(index >= 0)
				{
					throw new ArgumentException(_("Arg_ExistingEntry"));
				}
				InsertAt(~index, key, value);
			}
	public virtual void Clear()
			{
				Array.Clear( values, 0, values.Length ); // clear the array to release references
				Array.Clear( keys, 0, keys.Length ); // clear the array to release references
				count = 0;
				++generation;
			}
	public virtual bool Contains(Object key)
			{
				return (FindKey(key) >= 0);
			}
	public virtual IDictionaryEnumerator GetEnumerator()
			{
				return new SortedListEnumerator(this);
			}
	public virtual void Remove(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				int index = FindKey(key);
				if(index >= 0)
				{
					DoRemoveAt(index);
				}
			}
	public virtual bool IsFixedSize
			{
				get
				{
					return false;
				}
			}
	public virtual bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
	public virtual Object this[Object key]
			{
				get
				{
					int index = FindKey(key);
					if(index >= 0)
					{
						return values[index];
					}
					else
					{
						return null;
					}
				}
				set
				{
					if(key == null)
					{
						throw new ArgumentNullException("key");
					}
					int index = FindKey(key);
					if(index >= 0)
					{
						values[index] = value;
						++generation;
					}
					else
					{
						InsertAt(~index, key, value);
					}
				}
			}
	public virtual ICollection Keys
			{
				get
				{
					return GetKeyList();
				}
			}
	public virtual ICollection Values
			{
				get
				{
					return GetValueList();
				}
			}

	// Implement the ICollection interface.
	public virtual void CopyTo(Array array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				else if(array.Rank != 1)
				{
					throw new ArgumentException(_("Arg_RankMustBe1"));
				}
				else if(index < array.GetLowerBound(0))
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				else if(index > (array.GetLength(0) - count))
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				else
				{
					int posn;
					for(posn = 0; posn < count; ++posn)
					{
						array.SetValue
							(new DictionaryEntry(keys[posn], values[posn]),
							 index++);
					}
				}
			}
	public virtual int Count
			{
				get
				{
					return count;
				}
			}
	public virtual bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public virtual Object SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedListEnumerator(this);
			}

	// Implement the ICloneable interface.
	public virtual Object Clone()
			{
				SortedList list = (SortedList)MemberwiseClone();
				list.keys = (Object[])(keys.Clone());
				list.values = (Object[])(values.Clone());
				return list;
			}

	// Get or set the capacity of the sorted list.
	public virtual int Capacity
			{
				get
				{
					return keys.Length;
				}
				set
				{
					if(value < count)
					{
						throw new ArgumentOutOfRangeException
							("value", _("Arg_CannotReduceCapacity"));
					}
					if(value != keys.Length)
					{
						Object[] newKeys   = new Object [value];
						Object[] newValues = new Object [value];
						if(count > 0)
						{
							Array.Copy(keys, 0, newKeys, 0, count);
							Array.Copy(values, 0, newValues, 0, count);
						}
						keys   = newKeys;
						values = newValues;
					}
				}
			}

	// Determine if this sorted list contains a particular key.
	public virtual bool ContainsKey(Object key)
			{
				return (FindKey(key) >= 0);
			}

	// Determine if this sorted list contains a particular value.
	public virtual bool ContainsValue(Object value)
			{
				return (Array.IndexOf(values, value, 0, count) != -1);
			}

	// Get a value by index.
	public virtual Object GetByIndex(int index)
			{
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				return values[index];
			}

	// Get a key by index.
	public virtual Object GetKey(int index)
			{
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				return keys[index];
			}

	// Get the list of keys.
	public virtual IList GetKeyList()
			{
				if(keyList == null)
				{
					keyList = new KeyListWrapper(this);
				}
				return keyList;
			}

	// Get the list of values.
	public virtual IList GetValueList()
			{
				if(valueList == null)
				{
					valueList = new ValueListWrapper(this);
				}
				return valueList;
			}

	// Get the index of a specific key within the sorted list.
	public virtual int IndexOfKey(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				int index = FindKey(key);
				if(index >= 0)
				{
					return index;
				}
				else
				{
					return -1;
				}
			}

	// Get the index of a specific value within the sorted list.
	public virtual int IndexOfValue(Object value)
			{
				return Array.IndexOf(values, value, 0, count);
			}

	// Remove an entry from a position within the sorted list.
	public virtual void RemoveAt(int index)
			{
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				DoRemoveAt(index);
			}

	// Set a value within the sorted list by index.
	public virtual void SetByIndex(int index, Object value)
			{
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				values[index] = value;
				++generation;
			}

	// Trim the capacity of this sorted list to its actual size.
	public virtual void TrimToSize()
			{
				if(count != keys.Length && keys.Length > DefaultCapacity)
				{
					Object[] newKeys = new Object [count];
					Object[] newValues = new Object [count];
					Array.Copy(keys, 0, newKeys, 0, count);
					Array.Copy(values, 0, newValues, 0, count);
					keys = newKeys;
					values = newValues;
				}
			}

	// Wrap a sorted list to make it synchronized.
	public static SortedList Synchronized(SortedList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				else if(list.IsSynchronized)
				{
					return list;
				}
				else
				{
					return new SynchronizedSortedList(list);
				}
			}

	// Private class for implementing synchronized sorted lists.
	private class SynchronizedSortedList : SortedList, IEnumerable
	{

		// Internal state.
		private SortedList list;

		// Constructor.
		public SynchronizedSortedList(SortedList list)
				{
					this.list = list;
				}

		// Implement the IDictionary interface.
		public override void Add(Object key, Object value)
				{
					lock(SyncRoot)
					{
						list.Add(key, value);
					}
				}
		public override void Clear()
				{
					lock(SyncRoot)
					{
						list.Clear();
					}
				}
		public override bool Contains(Object key)
				{
					lock(SyncRoot)
					{
						return list.Contains(key);
					}
				}
		public override IDictionaryEnumerator GetEnumerator()
				{
					lock(SyncRoot)
					{
						return ((IDictionary)list).GetEnumerator();
					}
				}
		public override void Remove(Object key)
				{
					lock(SyncRoot)
					{
						list.Remove(key);
					}
				}
		public override bool IsFixedSize
				{
					get
					{
						lock(SyncRoot)
						{
							return list.IsFixedSize;
						}
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						lock(SyncRoot)
						{
							return list.IsReadOnly;
						}
					}
				}
		public override Object this[Object key]
				{
					get
					{
						lock(SyncRoot)
						{
							return list[key];
						}
					}
					set
					{
						lock(SyncRoot)
						{
							list[key] = value;
						}
					}
				}
	
		// Implement the ICollection interface.
		public override void CopyTo(Array array, int index)
				{
					lock(SyncRoot)
					{
						list.CopyTo(array, index);
					}
				}
		public override int Count
				{
					get
					{
						lock(SyncRoot)
						{
							return list.Count;
						}
					}
				}
		public override bool IsSynchronized
				{
					get
					{
						return true;
					}
				}
		public override Object SyncRoot
				{
					get
					{
						return list.SyncRoot;
					}
				}
	
		// Implement the IEnumerable interface.
		IEnumerator IEnumerable.GetEnumerator()
				{
					lock(SyncRoot)
					{
						return ((IEnumerable)list).GetEnumerator();
					}
				}
	
		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new SynchronizedSortedList
						((SortedList)(list.Clone()));
				}
	
		// Get or set the capacity of the sorted list.
		public override int Capacity
				{
					get
					{
						lock(SyncRoot)
						{
							return list.Capacity;
						}
					}
					set
					{
						lock(SyncRoot)
						{
							list.Capacity = value;
						}
					}
				}
	
		// Determine if this sorted list contains a particular key.
		public override bool ContainsKey(Object key)
				{
					lock(SyncRoot)
					{
						return list.ContainsKey(key);
					}
				}
	
		// Determine if this sorted list contains a particular value.
		public override bool ContainsValue(Object value)
				{
					lock(SyncRoot)
					{
						return list.ContainsValue(value);
					}
				}
	
		// Get a value by index.
		public override Object GetByIndex(int index)
				{
					lock(SyncRoot)
					{
						return list.GetByIndex(index);
					}
				}
	
		// Get a key by index.
		public override Object GetKey(int index)
				{
					lock(SyncRoot)
					{
						return list.GetKey(index);
					}
				}
	
		// Get the list of keys.
		public override IList GetKeyList()
				{
					lock(SyncRoot)
					{
						return new SynchronizedList(list.GetKeyList());
					}
				}

		// Get the list of values.
		public override IList GetValueList()
				{
					lock(SyncRoot)
					{
						return new SynchronizedList(list.GetValueList());
					}
				}
	
		// Get the index of a specific key within the sorted list.
		public override int IndexOfKey(Object key)
				{
					lock(SyncRoot)
					{
						return list.IndexOfKey(key);
					}
				}
	
		// Get the index of a specific value within the sorted list.
		public override int IndexOfValue(Object value)
				{
					lock(SyncRoot)
					{
						return list.IndexOfValue(value);
					}
				}
	
		// Remove an entry from a position within the sorted list.
		public override void RemoveAt(int index)
				{
					lock(SyncRoot)
					{
						list.RemoveAt(index);
					}
				}
	
		// Set a value within the sorted list by index.
		public override void SetByIndex(int index, Object value)
				{
					lock(SyncRoot)
					{
						list.SetByIndex(index, value);
					}
				}
	
		// Trim the capacity of this sorted list to its actual size.
		public override void TrimToSize()
				{
					lock(SyncRoot)
					{
						list.TrimToSize();
					}
				}

	}; // class SynchronizedSortedList

	// Read-only wrapper for the list of keys in this sorted list.
	private class KeyListWrapper : IList
	{
		// Internal state.
		private SortedList list;

		// Constructor.
		public KeyListWrapper(SortedList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public int Add(Object value)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public void Clear()
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public bool Contains(Object value)
				{
					return (list.FindKey(value) >= 0);
				}
		public int IndexOf(Object value)
				{
					int index = list.FindKey(value);
					if(index >= 0)
					{
						return index;
					}
					else
					{
						return -1;
					}
				}
		public void Insert(int index, Object value)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public void Remove(Object value)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public void RemoveAt(int index)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
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
						return true;
					}
				}
		public Object this[int index]
				{
					get
					{
						return list.GetKey(index);
					}
					set
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
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
						throw new ArgumentException(_("Arg_RankMustBe1"));
					}
					else if(index < array.GetLowerBound(0))
					{
						throw new ArgumentOutOfRangeException
							("index", _("Arg_InvalidArrayIndex"));
					}
					else if(index > (array.GetLength(0) - list.count))
					{
						throw new ArgumentException(_("Arg_InvalidArrayRange"));
					}
					else
					{
						int posn;
						for(posn = 0; posn < list.count; ++posn)
						{
							array.SetValue(list.keys[posn], index++);
						}
					}
				}
		public int Count
				{
					get
					{
						return list.count;
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
						return list.SyncRoot;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return new KeyListEnumerator(list);
				}

	}; // class KeyListWrapper

	// Read-only wrapper for the list of values in this sorted list.
	private class ValueListWrapper : IList
	{
		// Internal state.
		private SortedList list;

		// Constructor.
		public ValueListWrapper(SortedList list)
				{
					this.list = list;
				}

		// Implement the IList interface.
		public int Add(Object value)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public void Clear()
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public bool Contains(Object value)
				{
					return list.ContainsValue(value);
				}
		public int IndexOf(Object value)
				{
					return list.IndexOfValue(value);
				}
		public void Insert(int index, Object value)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public void Remove(Object value)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
				}
		public void RemoveAt(int index)
				{
					throw new InvalidOperationException(_("Invalid_ReadOnly"));
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
						return true;
					}
				}
		public Object this[int index]
				{
					get
					{
						return list.GetByIndex(index);
					}
					set
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
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
						throw new ArgumentException(_("Arg_RankMustBe1"));
					}
					else if(index < array.GetLowerBound(0))
					{
						throw new ArgumentOutOfRangeException
							("index", _("Arg_InvalidArrayIndex"));
					}
					else if(index > (array.GetLength(0) - list.count))
					{
						throw new ArgumentException(_("Arg_InvalidArrayRange"));
					}
					else
					{
						int posn;
						for(posn = 0; posn < list.count; ++posn)
						{
							array.SetValue(list.values[posn], index++);
						}
					}
				}
		public int Count
				{
					get
					{
						return list.count;
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
						return list.SyncRoot;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return new ValueListEnumerator(list);
				}

	}; // class ValueListWrapper

	// Private class for enumerating over a sorted list.
	private class SortedListEnumerator : IDictionaryEnumerator
	{
		// Internal state.
		private SortedList list;
		private int        generation;
		private int        position;

		// Constructor.
		public SortedListEnumerator(SortedList list)
				{
					this.list  = list;
					generation = list.generation;
					position   = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					++position;
					if(position < list.count)
					{
						return true;
					}
					position = list.count;
					return false;
				}
		public void Reset()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					position = -1;
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
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= list.count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return new DictionaryEntry(list.keys[position],
												   list.values[position]);
					}
				}
		public Object Key
				{
					get
					{
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= list.count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return list.keys[position];
					}
				}
		public Object Value
				{
					get
					{
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= list.count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return list.values[position];
					}
				}

	}; // class SortedListEnumerator

	// Private enumerator class for the key list.
	private class KeyListEnumerator : IEnumerator
	{
		// Internal state.
		private SortedList list;
		private int        generation;
		private int        position;

		// Constructor.
		public KeyListEnumerator(SortedList list)
				{
					this.list  = list;
					generation = list.generation;
					Reset();
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					++position;
					if(position < list.count)
					{
						return true;
					}
					position = list.count;
					return false;
				}
		public void Reset()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					position = -1;
				}
		public Object Current
				{
					get
					{
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= list.count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return list.keys[position];
					}
				}

	}; // class KeyListEnumerator

	// Private enumerator class for the value list.
	private class ValueListEnumerator : IEnumerator
	{
		// Internal state.
		private SortedList list;
		private int        generation;
		private int        position;

		// Constructor.
		public ValueListEnumerator(SortedList list)
				{
					this.list  = list;
					generation = list.generation;
					Reset();
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					++position;
					if(position < list.count)
					{
						return true;
					}
					position = list.count;
					return false;
				}
		public void Reset()
				{
					if(generation != list.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					position = -1;
				}
		public Object Current
				{
					get
					{
						if(generation != list.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= list.count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return list.values[position];
					}
				}

	}; // class ValueListEnumerator

}; // class SortedList

#endif // !ECMA_COMPAT

}; // namespace System.Collections
