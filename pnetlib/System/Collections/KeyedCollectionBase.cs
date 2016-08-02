/*
 * KeyedCollectionBase.cs - Implementation of
 *		"System.Collections.KeyedCollectionBase".
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

namespace System.Collections
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

public abstract class KeyedCollectionBase
	: ICollection, IEnumerable, IKeyedCollection, IList
{
	// Internal state.
	internal IKeyComparer comparer;
	internal ArrayList values;
	internal ArrayList keys;

	// Constructors.
	protected KeyedCollectionBase(IKeyComparer comparer, int capacity)
			{
				if(comparer == null)
				{
					throw new ArgumentNullException("comparer");
				}
				this.comparer = comparer;
				this.values = new ArrayList(capacity);
				this.keys = new ArrayList(capacity);
			}
	protected KeyedCollectionBase()
			: this(KeyComparer.CreateKeyComparer(ComparisonType.Ordinal), 16) {}
	protected KeyedCollectionBase(IKeyComparer comparer) : this(comparer, 16) {}
	protected KeyedCollectionBase(ComparisonType comparisonType)
			: this(KeyComparer.CreateKeyComparer(comparisonType), 16) {}
	protected KeyedCollectionBase(ComparisonType comparisonType, int capacity)
			: this(KeyComparer.CreateKeyComparer(comparisonType), capacity) {}

	// Find an item with a specific key.
	private int FindKey(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				int index;
				for(index = 0; index < keys.Count; ++index)
				{
					if(comparer.Equals(key, keys[index]))
					{
						return index;
					}
				}
				return -1;
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				values.CopyTo(array, index);
			}
	public virtual int Count
			{
				get
				{
					return values.Count;
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
	public virtual IEnumerator GetEnumerator()
			{
				return values.GetEnumerator();
			}

	// Implement the IKeyedCollection interface.
	int IKeyedCollection.Add(Object value, Object key)
			{
				OnValidate(value, key);
				if(FindKey(key) != -1)
				{
					throw new ArgumentException(S._("Arg_ExistingEntry"));
				}
				KeyedCollectionNotificationDetails details;
				int index = values.Count;
				details.AccessedByKey = true;
				details.Key = key;
				details.NewValue = value;
				OnInsert(details);
				try
				{
					try
					{
						keys.Add(key);
					}
					catch(Exception)
					{
						values.RemoveAt(index);
						throw;
					}
				}
				finally
				{
					OnInsertComplete(details);
				}
				return index;
			}
	bool IKeyedCollection.ContainsKey(Object key)
			{
				return (FindKey(key) != -1);
			}
	Object IKeyedCollection.GetValue(Object key)
			{
				int index = FindKey(key);
				if(index != -1)
				{
					return values[index];
				}
				else
				{
					return null;
				}
			}
	int IKeyedCollection.IndexOfKey(Object key)
			{
				return FindKey(key);
			}
	void IKeyedCollection.Insert(int index, Object value, Object key)
			{
				if(index < 0 || index > values.Count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				OnValidate(value, key);
				int existing = FindKey(key);
				if(existing != -1)
				{
					throw new ArgumentException(S._("Arg_ExistingEntry"));
				}
				KeyedCollectionNotificationDetails details;
				details.AccessedByKey = true;
				details.Key = key;
				details.NewValue = value;
				OnInsert(details);
				try
				{
					values.Insert(index, value);
					try
					{
						keys.Insert(index, key);
					}
					catch(Exception)
					{
						values.RemoveAt(index);
					}
				}
				finally
				{
					OnInsertComplete(details);
				}
			}
	void IKeyedCollection.InsertAfter
				(Object afterKey, Object value, Object key)
			{
				int index = FindKey(key);
				((IKeyedCollection)this).Insert(index + 1, value, key);
			}
	void IKeyedCollection.InsertBefore
				(Object beforeKey, Object value, Object key)
			{
				int index = FindKey(key);
				if(index == -1)
				{
					index = values.Count;
				}
				((IKeyedCollection)this).Insert(index, value, key);
			}
	void IKeyedCollection.RemoveByKey(Object key)
			{
				int index = FindKey(key);
				KeyedCollectionNotificationDetails details;
				details.AccessedByKey = true;
				details.Key = key;
				OnRemove(details);
				try
				{
					if(index != -1)
					{
						Object value = values[index];
						values.RemoveAt(index);
						try
						{
							keys.RemoveAt(index);
						}
						catch(Exception)
						{
							values.Insert(index, value);
							throw;
						}
					}
				}
				finally
				{
					OnRemoveComplete(details);
				}
			}
	void IKeyedCollection.SetValue(Object value, Object key)
			{
				OnValidate(value, key);
				KeyedCollectionNotificationDetails details;
				int index = FindKey(key);
				details.AccessedByKey = true;
				details.Key = key;
				details.NewValue = value;
				if(index != -1)
				{
					details.OldValue = values[index];
				}
				OnSet(details);
				try
				{
					if(index != -1)
					{
						values[index] = value;
					}
					else
					{
						index = values.Add(value);
						try
						{
							keys.Add(key);
						}
						catch(Exception)
						{
							values.RemoveAt(index);
							throw;
						}
					}
				}
				finally
				{
					OnSetComplete(details);
				}
			}

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				OnValidate(value);
				KeyedCollectionNotificationDetails details;
				int index = values.Count;
				details.AccessedByKey = true;
				details.Key = GenerateKeyForValue(value);
				details.NewValue = value;
				OnInsert(details);
				values.Add(value);
				try
				{
					try
					{
						keys.Add(details.Key);
					}
					catch(Exception)
					{
						values.RemoveAt(index);
						throw;
					}
				}
				finally
				{
					OnInsertComplete(details);
				}
				return index;
			}
	public virtual void Clear()
			{
				try
				{
					OnClear();
					values.Clear();
					keys.Clear();
				}
				finally
				{
					OnClearComplete();
				}
			}
	bool IList.Contains(Object value)
			{
				return values.Contains(value);
			}
	int IList.IndexOf(Object value)
			{
				return values.IndexOf(value);
			}
	void IList.Insert(int index, Object value)
			{
				((IKeyedCollection)this).Insert
					(index, value, GenerateKeyForValue(value));
			}
	void IList.Remove(Object value)
			{
				int index = values.IndexOf(value);
				if(index != -1)
				{
					KeyedCollectionNotificationDetails details;
					details.AccessedByKey = false;
					details.Index = index;
					OnRemove(details);
					try
					{
						values.RemoveAt(index);
						try
						{
							keys.RemoveAt(index);
						}
						catch(Exception)
						{
							values.Insert(index, value);
							throw;
						}
					}
					finally
					{
						OnRemoveComplete(details);
					}
				}
			}
	public virtual void RemoveAt(int index)
			{
				KeyedCollectionNotificationDetails details;
				details.AccessedByKey = false;
				details.Index = index;
				OnRemove(details);
				try
				{
					Object value = values[index];
					values.RemoveAt(index);
					try
					{
						keys.RemoveAt(index);
					}
					catch(Exception)
					{
						values.Insert(index, value);
						throw;
					}
				}
				finally
				{
					OnRemoveComplete(details);
				}
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
					KeyedCollectionNotificationDetails details;
					details.AccessedByKey = false;
					details.Index = index;
					OnGet(details);
					return values[index];
				}
				set
				{
					OnValidate(value);
					KeyedCollectionNotificationDetails details;
					details.AccessedByKey = false;
					details.Index = index;
					details.NewValue = value;
					details.OldValue = values[index];
					OnSet(details);
					try
					{
						values[index] = value;
					}
					finally
					{
						OnSetComplete(details);
					}
				}
			}

	// Generate a key for a specific value.
	protected virtual Object GenerateKeyForValue(Object value)
			{
				return value;
			}

	// Notify subclasses of various interesting properties.
	protected virtual void OnClear() {}
	protected virtual void OnClearComplete() {}
	protected virtual void OnGet(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnInsert
			(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnInsertComplete
			(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnRemove
			(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnRemoveComplete
			(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnSet
			(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnSetComplete
			(KeyedCollectionNotificationDetails details) {}
	protected virtual void OnValidate(Object value) {}
	protected virtual void OnValidate(Object value, Object key) {}

	// Get this object as a keyed collection.
	protected IKeyedCollection List
			{
				get
				{
					return this;
				}
			}
	protected IKeyedCollection InnerList
			{
				get
				{
					return this;
				}
			}

}; // class KeyedCollectionBase

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Collections
