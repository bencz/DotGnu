/*
 * Dictionary.cs - Implementation of the
 *		"System.Collections.Generic.Dictionary" class.
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

namespace System.Collections.Generic
{

#if CONFIG_GENERICS

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
[CLSCompliant(false)]
public class Dictionary<K,V>
	: System.Collections.ICollection, ICollection< KeyValuePair<K,V> >,
	  System.Collections.IDictionary, IDictionary<K,V>,
	  System.Collections.IEnumerable, IEnumerable< KeyValuePair<K,V> >
{
	// Constructors.
	[TODO]
	public Dictionary()
			{
				// TODO
			}
	[TODO]
	public Dictionary(int capacity)
			{
				// TODO
			}
	[TODO]
	public Dictionary(IKeyComparer comparer)
			{
				// TODO
			}
	[TODO]
	public Dictionary(int capacity, IKeyComparer comparer)
			{
				// TODO
			}
	[TODO]
	public Dictionary(IDictionary<K,V> dictionary)
			{
				// TODO
			}
	[TODO]
	public Dictionary(IDictionary<K,V> dictionary, IKeyComparer comparer)
			{
				// TODO
			}

	// Implement the non-generic ICollection interface.
	[TODO]
	void System.Collections.ICollection.CopyTo(Array array, int index)
			{
				// TODO
			}
	int System.Collections.ICollection.Count
			{
				get
				{
					return Count;
				}
			}
	bool System.Collections.ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}
	Object System.Collections.ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the generic ICollection<KeyValuePair<K,V>> interface.
	[TODO]
	void ICollection< KeyValuePair<K,V> >.CopyTo
				(KeyValuePair<K,V> array, int index)
			{
				// TODO
			}
	[TODO]
	public int Count
			{
				get
				{
					// TODO
					return 0;
				}
			}

	// Implement the non-generic IDictionary interface.
	[TODO]
	void System.Collections.IDictionary.Add(Object key, Object value)
			{
				// TODO
			}
	void System.Collections.IDictionary.Clear()
			{
				Clear();
			}
	bool System.Collections.IDictionary.Contains(Object key)
			{
				if(key == null)
				{
					return false;
				}
				else if(key is K)
				{
					return ContainsKey((K)key);
				}
				else
				{
					return false;
				}
			}
	[TODO]
	new IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
			{
				// TODO
				return null;
			}
	void System.Collections.IDictionary.Remove(Object key)
			{
				if(key != null && key is K)
				{
					Remove((K)key);
				}
			}
	bool System.Collections.IDictionary.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool System.Collections.IDictionary.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	Object System.Collections.IDictionary.this[Object key]
			{
				get
				{
					if(key == null)
					{
						throw new ArgumentNullException("key");
					}
					else if(key is K)
					{
						return this[(K)key];
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
					else
					{
						this[(K)key] = (V)value;
					}
				}
			}
	ICollection System.Collections.IDictionary.Keys
			{
				get
				{
					return (ICollection)Keys;
				}
			}
	ICollection System.Collections.IDictionary.Values
			{
				get
				{
					return (ICollection)Values;
				}
			}

	// Implement the generic IDictionary<K,V> interface.
	[TODO]
	public void Add(K key, V value)
			{
				// TODO
			}
	[TODO]
	public void Clear()
			{
				// TODO
			}
	[TODO]
	public bool ContainsKey(K key)
			{
				// TODO
				return false;
			}
	[TODO]
	public bool Remove(K key)
			{
				// TODO
				return false;
			}
	bool IDictionary<K,V>.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool IDictionary<K,V>.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	[TODO[
	V IDictionary.this[K key]
			{
				get
				{
					// TODO
					throw new NotImplementedException();
				}
				set
				{
					// TODO
				}
			}
	[TODO]
	public ICollection<K> Keys
			{
				get
				{
					// TODO
					return null;
				}
			}
	[TODO]
	public ICollection<V> Values
			{
				get
				{
					// TODO
					return null;
				}
			}

	// Implement the non-generic IEnumerable interface.
	System.Collections.IEnumerator
		System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

	// Implement the generic IEnumerable< KeyValuePair<K,V> > interface.
	IEnumerator< KeyValuePair<K,V> >
		IEnumerable< KeyValuePair<K,V> >.GetEnumerator()
			{
				return GetEnumerator();
			}

	// Determine if this dictionary contains a particular value.
	[TODO]
	public bool ContainsValue(V value)
			{
				// TODO
				return false;
			}

	// Get an enumerator for this dictionary.
	public Enumerator<K,V> GetEnumerator()
			{
				// TODO
				return new Enumerator(this);
			}

	// Enumerator class for generic dictionaries.
	public struct Enumerator<K,V>
		: System.Collections.IEnumerator, IEnumerator< KeyValuePair<K,V> >
	{
		// Constructor.
		[TODO]
		internal Enumerator(Dictionary<K,V> dictionary)
				{
					// TODO
				}

		// Dispose of this enumerator.
		public void Dispose()
				{
					// TODO
				}

		// Implement the non-generic IEnumerator interface.
		bool System.Collections.IEnumerator.MoveNext()
				{
					return MoveNext();
				}
		void System.Collections.IEnumerator.Reset()
				{
					throw new InvalidOperationException();
				}
		Object System.Collections.IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}

		// Implement the generic IEnumerator< KeyValuePair<K,V> > interface.
		public bool MoveNext()
				{
					// TODO
					return false;
				}
		public KeyValuePair<K,V> Current
				{
					get
					{
						// TODO
						throw new NotImplementedException();
					}
				}

	}; // struct Enumerator<K,V>

}; // class IDictionary<K,V>

#endif // CONFIG_GENERICS

}; // namespace System.Collections.Generic
