/*
 * Hashtable.cs - Generic hash table class.
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

public sealed class Hashtable<KeyT, ValueT>
		: IDictionary<KeyT, ValueT>, ICapacity, ICloneable
{
	// Contents of a hash bucket.
	private struct HashBucket<KeyT, ValueT>
	{
	    public bool hasEntry;
		public KeyT key;
		public ValueT value;

	}; // struct HashBucket

	// Internal state.
	private IHashCodeProvider<KeyT> hcp;
	private IComparer<ValueT> comparer;
	private int capacity;
	private int num;
	private HashBucket<KeyT, ValueT>[] table;

	// Table of the first 400 prime numbers.
	private static readonly int[] primes = {
		2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47,
		53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107,
		109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167,
		173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
		233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283,
		293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359,
		367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431,
		433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491,
		499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571,
		577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641,
		643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709,
		719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787,
		797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859,
		863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
		947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013, 1019,
		1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087,
		1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153,
		1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223, 1229,
		1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
		1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381,
		1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453,
		1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511, 1523,
		1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
		1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663,
		1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733, 1741,
		1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811, 1823,
		1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889, 1901,
		1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993,
		1997, 1999, 2003, 2011, 2017, 2027, 2029, 2039, 2053, 2063,
		2069, 2081, 2083, 2087, 2089, 2099, 2111, 2113, 2129, 2131,
		2137, 2141, 2143, 2153, 2161, 2179, 2203, 2207, 2213, 2221,
		2237, 2239, 2243, 2251, 2267, 2269, 2273, 2281, 2287, 2293,
		2297, 2309, 2311, 2333, 2339, 2341, 2347, 2351, 2357, 2371,
		2377, 2381, 2383, 2389, 2393, 2399, 2411, 2417, 2423, 2437,
		2441, 2447, 2459, 2467, 2473, 2477, 2503, 2521, 2531, 2539,
		2543, 2549, 2551, 2557, 2579, 2591, 2593, 2609, 2617, 2621,
		2633, 2647, 2657, 2659, 2663, 2671, 2677, 2683, 2687, 2689,
		2693, 2699, 2707, 2711, 2713, 2719, 2729, 2731, 2741};

	// Constructors.
	public Hashtable()
			{
				hcp = null;
				comparer = null;
				capacity = 0;
				num = 0;
				table = null;
			}
	public Hashtable(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", S._("ArgRange_NonNegative"));
				}
				hcp = null;
				comparer = null;
				this.capacity = capacity;
				num = 0;
				if(capacity != 0)
				{
					table = new HashBucket<KeyT, ValueT> [capacity];
				}
				else
				{
					table = null;
				}
			}
	public Hashtable(IHashCodeProvider<KeyT> hcp, IComparer<KeyT> comparer)
			{
				this.hcp = hcp;
				this.comparer = comparer;
				capacity = 0;
				num = 0;
				table = null;
			}
	public Hashtable(int capacity, IHashCodeProvider<KeyT> hcp,
					 IComparer<KeyT> comparer)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", S._("ArgRange_NonNegative"));
				}
				this.hcp = hcp;
				this.comparer = comparer;
				this.capacity = capacity;
				num = 0;
				if(capacity != 0)
				{
					table = new HashBucket<KeyT, ValueT> [capacity];
				}
				else
				{
					table = null;
				}
			}

	// Add a hash entry to the table directly, with no error checking.
	// This assumes that there is at least one spare bucket available.
	private void AddDirect(KeyT key, ValueT value)
			{
				int hash = GetHash(key);
				hash = (int)(((uint)hash) % ((uint)capacity));
				for(;;)
				{
					if(!(table[hash].hasEntry))
					{
						// We've found an empty slot, so add the entry.
						table[hash].hasEntry = true;
						table[hash].key = key;
						table[hash].value = value;
						break;
					}
					hash = (hash + 1) % capacity;
				}
				++num;
			}

	// Expand the hash table and add a new entry.
	private void ExpandAndAdd(KeyT key, ValueT value)
			{
				HashBucket[] orig;
				int origSize;
				int newCapacity;

				// Copy the original table.
				orig = table;
				origSize = capacity;

				// Expand the size of the hash table.
				if(capacity == 0)
				{
					newCapacity = 2;
				}
				else
				{
					newCapacity = capacity * 2;
					if(newCapacity <= primes[primes.Length - 1])
					{
						// Search for the next value in the "primes" table.
						int left, right, middle;
						left = 0;
						right = primes.Length - 1;
						while(left <= right)
						{
							middle = (left + right) / 2;
							if(newCapacity < primes[middle])
							{
								right = middle - 1;
							}
							else if(newCapacity > primes[middle])
							{
								left = middle + 1;
							}
							else
							{
								right = middle;
								break;
							}
						}
						newCapacity = primes[right];
					}
					else
					{
						// We've run out of primes, so make the number odd
						// and assume that the result is close enough to
						// prime that it will be useful for our purposes.
						++newCapacity;
					}
				}
				table = new HashBucket<KeyT, ValueT> [newCapacity];
				capacity = newCapacity;
				num = 0;

				// Copy the original entries to the new table.
				while(origSize > 0)
				{
					--origSize;
					if(orig[origSize].hasEntry)
					{
						AddDirect(orig[origSize].key, orig[origSize].value);
					}
				}

				// Add the new entry to the hash table.
				AddDirect(key, value);
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				Hashtable<KeyT, ValueT> hashtab =
					(Hashtable<KeyT, ValueT>)(MemberwiseClone());
				if(capacity > 0)
				{
					hashtab.table = new HashBucket<KeyT, ValueT> [capacity];
					Array.Copy(table, hashtab.table, capacity);
				}
				return hashtab;
			}

	// Implement the ICollection< DictionaryEntry<KeyT, ValueT> > interface.
	public void CopyTo(DictionaryEntry<KeyT, ValueT>[] array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				else if(index < array.GetLowerBound(0))
				{
					throw new ArgumentOutOfRangeException
						("index", S._("Arg_InvalidArrayIndex"));
				}
				else if(index > (array.GetLength(0) - num))
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
				else
				{
					IDictionaryIterator<KeyT, ValueT> iterator;
					iterator = GetIterator();
					while(iterator.MoveNext())
					{
						array[index++] = iterator.Current;
					}
				}
			}
	public int Count
			{
				get
				{
					return num;
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

	// Implement the IDictionary<KeyT, ValueT> interface.
	public void Add(KeyT key, ValueT value)
			{
				// Find an empty slot to add the entry, or expand
				// the table if there are no free slots.
				if(capacity == 0)
				{
					ExpandAndAdd(key, value);
					return;
				}
				int hash = GetHash(key);
				hash = (int)(((uint)hash) % ((uint)capacity));
				int count = capacity;
				while(count > 0)
				{
					if(!(table[hash].hasEntry))
					{
						// We've found an empty slot.
						table[hash].hasEntry = true;
						table[hash].key = key;
						table[hash].value = value;
						++num;
						return;
					}
					else if(KeyEquals(table[hash].key, key))
					{
						// There is already an entry with the key.
						throw new ArgumentException(_("Arg_ExistingEntry"));
					}
					hash = (hash + 1) % capacity;
					--count;
				}
				ExpandAndAdd(key, value);
			}
	public void Clear()
			{
				if(table != null)
				{
					Array.Clear(table, 0, capacity);
				}
				num = 0;
			}
	public bool Contains(KeyT key)
			{
				if(capacity == 0)
				{
					return false;
				}
				int hash = GetHash(key);
				hash = (int)(((uint)hash) % ((uint)capacity));
				int count = capacity;
				while(count > 0)
				{
					if(!(table[hash].hasEntry))
					{
						break;
					}
					else if(KeyEquals(table[hash].key, key))
					{
						return true;
					}
					hash = (hash + 1) % capacity;
					--count;
				}
				return false;
			}
	public IDictionaryIterator<KeyT, ValueT> GetIterator()
			{
				return new HashtableIterator<KeyT, ValueT>(this);
			}
	public void Remove(KeyT key)
			{
				if(capacity == 0)
				{
					return;
				}
				int hash = GetHash(key);
				hash = (int)(((uint)hash) % ((uint)capacity));
				int count = capacity;
				while(count > 0)
				{
					if(!(table[hash].hasEntry))
					{
						break;
					}
					else if(KeyEquals(table[hash].key, key))
					{
						table[hash].hasEntry = false;
						--num;
						break;
					}
					hash = (hash + 1) % capacity;
					--count;
				}
			}
	public ValueT this[KeyT key]
			{
				get
				{
					// Find an existing entry with the specified key.
					if(capacity == 0)
					{
						throw new ArgumentException
							(S._("Arg_NotInDictionary"));
					}
					int hash = GetHash(key);
					hash = (int)(((uint)hash) % ((uint)capacity));
					int count = capacity;
					while(count > 0)
					{
						if(!(table[hash].hasEntry))
						{
							break;
						}
						else if(KeyEquals(table[hash].key, key))
						{
							return table[hash].value;
						}
						hash = (hash + 1) % capacity;
						--count;
					}
					throw new ArgumentException(S._("Arg_NotInDictionary"));
				}
				set
				{
					// Find an existing entry and replace it, or
					// add a new entry to the table if not found.
					if(capacity == 0)
					{
						ExpandAndAdd(key, value);
						return;
					}
					int hash = GetHash(key);
					hash = (int)(((uint)hash) % ((uint)capacity));
					int count = capacity;
					while(count > 0)
					{
						if(!(table[hash].hasEntry))
						{
							// We've found an empty slot.
							table[hash].hasEntry = true;
							table[hash].key = key;
							table[hash].value = value;
							++num;
							return;
						}
						else if(KeyEquals(table[hash].key, key))
						{
							// There is already an entry with the key,
							// so replace its value.
							table[hash].value = value;
							return;
						}
						hash = (hash + 1) % capacity;
						--count;
					}
					ExpandAndAdd(key, value);
				}
			}
	public ICollection<KeyT> Keys
			{
				get
				{
					return new HashtableKeyCollection<KeyT, ValueT>(this);
				}
			}
	public ICollection<ValueT> Values
			{
				get
				{
					return new HashtableValueCollection<KeyT, ValueT>(this);
				}
			}

	// Implement the IIterable< DictionaryEntry<KeyT, ValueT> > interface.
	IIterator IIterable< DictionaryEntry<KeyT, ValueT> >.GetIterator()
			{
				return GetIterator();
			}

	// Implement the ICapacity interface.
	public int Capacity
			{
				get
				{
					return capacity;
				}
				set
				{
					// Validate the argument.
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							(S._("ArgRange_NonNegative"));
					}
					if(value < num)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("Arg_CannotReduceCapacity"));
					}

					// Copy the original table.
					HashBucket[] orig = table;
					int origSize = capacity;

					// Create the new table.
					table = new HashBucket<KeyT, ValueT> [value];
					capacity = value;
					num = 0;

					// Copy the original entries to the new table.
					while(origSize > 0)
					{
						--origSize;
						if(orig[origSize].hasEntry)
						{
							AddDirect(orig[origSize].key, orig[origSize].value);
						}
					}
				}
			}

	// Get the hash value for a key.
	private int GetHash(KeyT key)
			{
				IHashCodeProvider<KeyT> provider = hcp;
				if(provider != null)
				{
					return provider.GetHashCode(key);
				}
				else
				{
					Object k = (Object)key;
					if(k != null)
					{
						return k.GetHashCode();
					}
					else
					{
						return 0;
					}
				}
			}

	// Determine if an item is equal to a key value.
	private bool KeyEquals(KeyT item, KeyT key)
			{
				IComparer<KeyT> cmp = comparer;
				if(cmp != null)
				{
					return (cmp.Compare(item, key) == 0);
				}
				else
				{
					Object i = (Object)item;
					if(i != null)
					{
						return i.Equals((Object)key);
					}
					else
					{
						return (((Object)key) == null);
					}
				}
			}

	// Hashtable collection and dictionary iterator.
	private class HashtableIterator<KeyT, ValueT>
			: IDictionaryIterator<KeyT, ValueT>
	{
		// Internal state.
		protected Hashtable<KeyT, ValueT> table;
		protected int		posn;

		// Constructor.
		public HashtableIterator(Hashtable<KeyT, ValueT> table)
				{
					this.table = table;
					posn = -1;
				}

		// Implement the IIterator<DictionaryEntry<KeyT, ValueT>> interface.
		public bool MoveNext()
				{
					while(++posn < table.capacity)
					{
						if(table.table[posn].hasEntry)
						{
							return true;
						}
					}
					posn = table.capacity;
					return false;
				}
		public void Reset()
				{
					posn = -1;
				}
		public void Remove()
				{
					if(posn < 0 || posn >= table.capacity ||
					   !(table.table[posn].hasEntry))
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
					table.table[posn].hasEntry = false;
					--(table.num);
				}
		public DictionaryEntry<KeyT, ValueT> Current
				{
					get
					{
						if(posn < 0 || posn >= table.capacity ||
					       !(table.table[posn].hasEntry))
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return new DictionaryEntry<KeyT, ValueT>
								(table.table[posn].key,
							     table.table[posn].value);
					}
				}

		// Implement the IDictionaryIterator<KeyT, ValueT> interface.
		public KeyT Key
				{
					get
					{
						if(posn < 0 || posn >= table.capacity ||
					       !(table.table[posn].hasEntry))
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return table.table[posn].key;
					}
				}
		public ValueT Value
				{
					get
					{
						if(posn < 0 || posn >= table.capacity ||
					       !(table.table[posn].hasEntry))
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return table.table[posn].value;
					}
					set
					{
						if(posn < 0 || posn >= table.capacity ||
					       !(table.table[posn].hasEntry))
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						table.table[posn].value = value;
					}
				}

	}; // class HashtableIterator<KeyT, ValueT>

	// Collection of hash table keys.
	private sealed class HashtableKeyCollection<KeyT, ValueT>
			: ICollection<KeyT>
	{
		// Internal state.
		private Hashtable<KeyT, ValueT> table;

		// Constructor.
		public HashtableKeyCollection(Hashtable<KeyT, ValueT> table)
				{
					this.table = table;
				}

		// Implement the ICollection<KeyT> interface.
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
						return table.Count;
					}
				}
		public bool IsFixedSize
				{
					get
					{
						return table.IsFixedSize;
					}
				}
		public bool IsReadOnly
				{
					get
					{
						return table.IsReadOnly;
					}
				}
		public bool IsSynchronized
				{
					get
					{
						return table.IsSynchronized;
					}
				}
		public Object SyncRoot
				{
					get
					{
						return table.SyncRoot;
					}
				}

		// Implement the IIterable<KeyT> interface.
		public IIterator<KeyT> GetIterator()
				{
					return new HashtableKeyIterator<KeyT, ValueT>(table);
				}

	}; // class HashtableKeyCollection<KeyT, ValueT>

	// Collection of hash table values.
	private sealed class HashtableValueCollection<KeyT, ValueT>
			: ICollection<ValueT>
	{
		// Internal state.
		private Hashtable<KeyT, ValueT> table;

		// Constructor.
		public HashtableValueCollection(Hashtable<KeyT, ValueT> table)
				{
					this.table = table;
				}

		// Implement the ICollection<ValueT> interface.
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
						return table.Count;
					}
				}
		public bool IsFixedSize
				{
					get
					{
						return table.IsFixedSize;
					}
				}
		public bool IsReadOnly
				{
					get
					{
						return table.IsReadOnly;
					}
				}
		public bool IsSynchronized
				{
					get
					{
						return table.IsSynchronized;
					}
				}
		public Object SyncRoot
				{
					get
					{
						return table.SyncRoot;
					}
				}

		// Implement the IIterable<KeyT> interface.
		public IIterator<ValueT> GetIterator()
				{
					return new HashtableValueIterator<KeyT, ValueT>(table);
				}

	}; // class HashtableValueCollection<KeyT, ValueT>

	// Hashtable key collection iterator.
	private class HashtableKeyIterator<KeyT, ValueT> : IIterator<KeyT>
	{
		// Internal state.
		protected Hashtable<KeyT, ValueT> table;
		protected int posn;

		// Constructor.
		public HashtableKeyIterator(Hashtable<KeyT, ValueT> table)
				{
					this.table = table;
					posn = -1;
				}

		// Implement the IIterator<KeyT> interface.
		public bool MoveNext()
				{
					while(++posn < table.capacity)
					{
						if(table.table[posn].hasEntry)
						{
							return true;
						}
					}
					posn = table.capacity;
					return false;
				}
		public void Reset()
				{
					posn = -1;
				}
		public void Remove()
				{
					if(posn < 0 || posn >= table.capacity ||
					   !(table.table[posn].hasEntry))
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
					table.table[posn].hasEntry = false;
					--(table.num);
				}
		public KeyT Current
				{
					get
					{
						if(posn < 0 || posn >= table.capacity ||
					       !(table.table[posn].hasEntry))
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return table.table[posn].key;
					}
				}

	}; // class HashtableKeyIterator<KeyT, ValueT>

	// Hashtable value collection iterator.
	private class HashtableValueIterator<KeyT, ValueT> : IIterator<KeyT>
	{
		// Internal state.
		protected Hashtable<KeyT, ValueT> table;
		protected int posn;

		// Constructor.
		public HashtableValueIterator(Hashtable<KeyT, ValueT> table)
				{
					this.table = table;
					posn = -1;
				}

		// Implement the IIterator<ValueT> interface.
		public bool MoveNext()
				{
					while(++posn < table.capacity)
					{
						if(table.table[posn].hasEntry)
						{
							return true;
						}
					}
					posn = table.capacity;
					return false;
				}
		public void Reset()
				{
					posn = -1;
				}
		public void Remove()
				{
					if(posn < 0 || posn >= table.capacity ||
					   !(table.table[posn].hasEntry))
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
					table.table[posn].hasEntry = false;
					--(table.num);
				}
		public ValueT Current
				{
					get
					{
						if(posn < 0 || posn >= table.capacity ||
					       !(table.table[posn].hasEntry))
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return table.table[posn].value;
					}
				}

	}; // class HashtableValueIterator<KeyT, ValueT>

}; // class Hashtable<KeyT, ValueT>

}; // namespace Generics
