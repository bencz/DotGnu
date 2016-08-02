/*
 * KeyedCollection.cs - Implementation of "System.Collections.KeyedCollection".
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

public class KeyedCollection : KeyedCollectionBase, ICloneable
{
	// Constructors.
	public KeyedCollection(IKeyComparer comparer, int capacity)
			: base(comparer, capacity) {}
	public KeyedCollection() : base() {}
	public KeyedCollection(IKeyComparer comparer) : base(comparer) {}
	public KeyedCollection(ComparisonType comparisonType)
			: base(comparisonType) {}
	public KeyedCollection(ComparisonType comparisonType, int capacity)
			: base(comparisonType, capacity) {}

	// Add an item to this collection.
	public virtual int Add(Object value)
			{
				return ((IList)this).Add(value);
			}
	public virtual int Add(Object value, String key)
			{
				return ((IKeyedCollection)this).Add(value, key);
			}

	// Add a range of items to this collection.
	public virtual void AddRange(ICollection c)
			{
				if(c != null)
				{
					foreach(Object value in c)
					{
						Add(value);
					}
				}
			}
	public virtual void AddRange(IDictionary c)
			{
				if(c != null)
				{
					IDictionaryEnumerator e = c.GetEnumerator();
					while(e.MoveNext())
					{
						((IKeyedCollection)this).Add(e.Value, e.Key);
					}
				}
			}

	// Clone this object.
	public virtual Object Clone()
			{
				KeyedCollection kc = (KeyedCollection)(MemberwiseClone());
				kc.values = (ArrayList)(values.Clone());
				kc.keys = (ArrayList)(keys.Clone());
				return kc;
			}

	// Determine if this collection contains a specific value.
	public virtual bool Contains(Object value)
			{
				return ((IList)this).Contains(value);
			}

	// Determine if this collection contains a specific key.
	public virtual bool ContainsKey(Object key)
			{
				return ((IKeyedCollection)this).ContainsKey(key);
			}

	// Copy the elements of this collection into an array.
	public virtual void CopyTo(Array array)
			{
				((ICollection)this).CopyTo(array, 0);
			}
	public virtual void CopyTo(Array array, int index)
			{
				((ICollection)this).CopyTo(array, index);
			}

	// Get or set an item from this collection by key.
	public virtual Object this[String key]
			{
				get
				{
					return ((IKeyedCollection)this).GetValue(key);
				}
				set
				{
					((IKeyedCollection)this).SetValue(value, key);
				}
			}

	// Get or set an item from this collection by index.
	public virtual Object this[int index]
			{
				get
				{
					return ((IList)this)[index];
				}
				set
				{
					((IList)this)[index] = value;
				}
			}

	// Get the index of a specific value in this collection.
	public virtual int IndexOf(Object value)
			{
				return ((IList)this).IndexOf(value);
			}

	// Get the index of a specific key in this collection.
	public virtual int IndexOfKey(String key)
			{
				return ((IKeyedCollection)this).IndexOfKey(key);
			}

	// Insert an object into this collection.
	public virtual void Insert(int index, Object value)
			{
				((IList)this).Insert(index, value);
			}
	public virtual void Insert(int index, Object value, String key)
			{
				((IKeyedCollection)this).Insert(index, value, key);
			}

	// Insert an object into this collection before or after a given key.
	public virtual void InsertBefore
				(String beforeKey, Object value, String key)
			{
				((IKeyedCollection)this).InsertBefore
						(beforeKey, value, key);
			}
	public virtual void InsertAfter
				(String afterKey, Object value, String key)
			{
				((IKeyedCollection)this).InsertAfter
						(afterKey, value, key);
			}

	// Remove a particular value from this collection.
	public virtual void Remove(Object value)
			{
				((IList)this).Remove(value);
			}

	// Remove a particular key from this collection.
	public virtual void RemoveByKey(String key)
			{
				((IKeyedCollection)this).RemoveByKey(key);
			}

	// Convert this collection into an array.
	public virtual Object[] ToArray()
			{
				return values.ToArray();
			}
	public virtual Array ToArray(Type type)
			{
				return values.ToArray(type);
			}

}; // class KeyedCollection

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Collections
