/*
 * Hashtable.cs - Implementation of the
 *			"System.Collections.Hashtable" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System;
using System.Private;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public class Hashtable : ICloneable, ICollection, IDictionary, IEnumerable
#if CONFIG_SERIALIZATION
	, ISerializable, IDeserializationCallback
#endif
{

	// Contents of a hash bucket.
	private struct HashBucket
	{
		public Object key;
		public Object value;

	}; // struct HashBucket

	// Internal state.
	private IHashCodeProvider hcp__;
	private IComparer         comparer__;
	private int				  capacity;
	private int				  capacityLimit;
	private int				  num;
	private float			  loadFactor;
	private HashBucket[]	  table;
	private int				  generation;
#if CONFIG_SERIALIZATION
	private SerializationInfo info;
#endif

	// Place holder for a removed object.
	private static readonly Object removed = new Object();
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
				hcp__ = null;
				comparer__ = null;
				capacity = 0;
				capacityLimit = 0;
				num = 0;
				try
				{
					loadFactor = 1.0f;
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not support floating point,
					// but we still need hash tables when no FP.
				}
				table = null;
				generation = 0;
			}
	public Hashtable(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				hcp__ = null;
				comparer__ = null;
				this.capacity = capacity;
				capacityLimit = capacity;
				num = 0;
				try
				{
					loadFactor = 1.0f;
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not support floating point,
					// but we still need hash tables when no FP.
				}
				if(capacity != 0)
				{
					table = new HashBucket [capacity];
				}
				else
				{
					table = null;
				}
				generation = 0;
			}
	public Hashtable(IHashCodeProvider hcp, IComparer comparer)
			{
				hcp__ = hcp;
				comparer__ = comparer;
				capacity = 0;
				capacityLimit = 0;
				num = 0;
				try
				{
					loadFactor = 1.0f;
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not support floating point,
					// but we still need hash tables when no FP.
				}
				table = null;
				generation = 0;
			}
	public Hashtable(int capacity, IHashCodeProvider hcp, IComparer comparer)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				hcp__ = hcp;
				comparer__ = comparer;
				this.capacity = capacity;
				capacityLimit = capacity;
				num = 0;
				try
				{
					loadFactor = 1.0f;
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not support floating point,
					// but we still need hash tables when no FP.
				}
				if(capacity != 0)
				{
					table = new HashBucket [capacity];
				}
				else
				{
					table = null;
				}
				generation = 0;
			}
	public Hashtable(IDictionary d)
			{
				if(d == null)
				{
					throw new ArgumentNullException("d");
				}
				hcp__ = null;
				comparer__ = null;
				capacity = d.Count;
				capacityLimit = capacity;
				num = 0;
				try
				{
					loadFactor = 1.0f;
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not support floating point,
					// but we still need hash tables when no FP.
				}
				if(capacity != 0)
				{
					table = new HashBucket [capacity];
				}
				else
				{
					table = null;
				}
				generation = 0;
				AddDictionaryContents(d);
			}
	public Hashtable(IDictionary d, IHashCodeProvider hcp, IComparer comparer)
			{
				if(d == null)
				{
					throw new ArgumentNullException("d");
				}
				hcp__ = hcp;
				comparer__ = comparer;
				capacity = d.Count;
				capacityLimit = capacity;
				num = 0;
				try
				{
					loadFactor = 1.0f;
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not support floating point,
					// but we still need hash tables when no FP.
				}
				if(capacity != 0)
				{
					table = new HashBucket [capacity];
				}
				else
				{
					table = null;
				}
				generation = 0;
				AddDictionaryContents(d);
			}

#if !ECMA_COMPAT

	// Non-ECMA constructors.
	public Hashtable(int capacity, float loadFactor)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				if(loadFactor >= 0.1f && loadFactor <= 1.0f)
				{
					hcp__ = null;
					comparer__ = null;
					this.capacity = capacity;
					capacityLimit = (int)(capacity * loadFactor);
					num = 0;
					this.loadFactor = loadFactor;
					if(capacity != 0)
					{
						table = new HashBucket [capacity];
					}
					else
					{
						table = null;
					}
					generation = 0;
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("loadFactor", _("ArgRange_HashLoadFactor"));
				}
			}
	public Hashtable(int capacity, float loadFactor,
					 IHashCodeProvider hcp, IComparer comparer)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				if(loadFactor >= 0.1f && loadFactor <= 1.0f)
				{
					hcp__ = hcp;
					comparer__ = comparer;
					this.capacity = capacity;
					capacityLimit = (int)(capacity * loadFactor);
					num = 0;
					this.loadFactor = loadFactor;
					if(capacity != 0)
					{
						table = new HashBucket [capacity];
					}
					else
					{
						table = null;
					}
					generation = 0;
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("loadFactor", _("ArgRange_HashLoadFactor"));
				}
			}
	public Hashtable(IDictionary d, float loadFactor)
			{
				if(d == null)
				{
					throw new ArgumentNullException("d");
				}
				if(loadFactor >= 0.1f && loadFactor <= 1.0f)
				{
					hcp__ = null;
					comparer__ = null;
					capacity = d.Count;
					capacityLimit = (int)(capacity * loadFactor);
					num = 0;
					this.loadFactor = loadFactor;
					if(capacity != 0)
					{
						table = new HashBucket [capacity];
					}
					else
					{
						table = null;
					}
					generation = 0;
					AddDictionaryContents(d);
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("loadFactor", _("ArgRange_HashLoadFactor"));
				}
			}
	public Hashtable(IDictionary d, float loadFactor,
					 IHashCodeProvider hcp, IComparer comparer)
			{
				if(d == null)
				{
					throw new ArgumentNullException("d");
				}
				if(loadFactor >= 0.1f && loadFactor <= 1.0f)
				{
					hcp__ = hcp;
					comparer__ = comparer;
					capacity = d.Count;
					capacityLimit = (int)(capacity * loadFactor);
					num = 0;
					this.loadFactor = loadFactor;
					if(capacity != 0)
					{
						table = new HashBucket [capacity];
					}
					else
					{
						table = null;
					}
					generation = 0;
					AddDictionaryContents(d);
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("loadFactor", _("ArgRange_HashLoadFactor"));
				}
			}

#endif // !ECMA_COMPAT

#if CONFIG_SERIALIZATION

	protected Hashtable(SerializationInfo info, StreamingContext context)
			{
				// Save the serialization information for the later call
				// to "OnDeserialization".
				this.info = info;
			}

#endif // CONFIG_SERIALIZATION
	// Add the contents of a dictionary to this hash table.
	private void AddDictionaryContents(IDictionary d)
			{
				IDictionaryEnumerator enumerator = d.GetEnumerator();
				while(enumerator.MoveNext())
				{
					Add(enumerator.Key, enumerator.Value);
				}
			}

	// Expand the hash table and add a new entry.
	private void Expand()
			{
				HashBucket[] orig;
				int origSize;
				int newCapacity;

				// Copy the original table.
				orig = table;
				origSize = capacity;

				// Expand the size of the hash table.
				// Keep going until we meet the load factor constraint
				do
				{
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
					table = new HashBucket [newCapacity];
					capacity = newCapacity;

					// Determine the new capacity limit.
					try
					{
						capacityLimit = (int)(capacity * loadFactor);
					}
					catch(NotImplementedException)
					{
						// The runtime engine does not support floating point,
						// so assume a load factor of 1.
						capacityLimit = capacity;
					}
				} while (capacityLimit <= num);

				// Copy the original entries to the new table.
				while (origSize > 0)
				{
					--origSize;
					if(orig[origSize].key == null || orig[origSize].key == removed)
					  	continue;
					int insertionPoint;
					FindSlot(orig[origSize].key, out insertionPoint);
					table[insertionPoint].key = orig[origSize].key;
					table[insertionPoint].value = orig[origSize].value;
				}
			}

	// Implement the ICloneable interface.
	public virtual Object Clone()
			{
				Hashtable hashtab = (Hashtable)(MemberwiseClone());
				if(capacity > 0)
				{
					hashtab.table = new HashBucket [capacity];
					Array.Copy(table, hashtab.table, capacity);
				}
				return hashtab;
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
				else if(index > (array.GetLength(0) - num))
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				else
				{
					if(table != null)
					{
						int posn;
						for(posn = 0; posn < table.Length; ++posn)
						{
							if(table[posn].key != null && table[posn].key != removed)
							{
								array.SetValue
									(new DictionaryEntry(table[posn].key,
														 table[posn].value),
									 index++);
							}
						}
					}
				}
			}
	public virtual int Count
			{
				get
				{
					return num;
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

	// Implement the IDictionary interface.
	public virtual void Add(Object key, Object value)
			{
				int insertionPoint;
				int found = FindSlot(key, out insertionPoint);
				if(found != -1)
				{
					// There is already an entry with the key.
					throw new ArgumentException
						(_("Arg_ExistingEntry"));
				}
				if(insertionPoint == -1)
				{
					Expand();
					FindSlot(key, out insertionPoint);
				}
				table[insertionPoint].key = key;
				table[insertionPoint].value = value;
				++num;
			}
	public virtual void Clear()
			{
				if(table != null)
				{
					Array.Clear(table, 0, capacity);
				}
				num = 0;
			}
	public virtual bool Contains(Object key)
			{
				return ContainsKey(key);
			}
	public virtual IDictionaryEnumerator GetEnumerator()
			{
				return new HashtableEnum(this);
			}
	public virtual void Remove(Object key)
			{
				int insertionPoint;
				int found = FindSlot(key, out insertionPoint);
				if(found != -1)
				{
					table[found].value = null;
					--num;
					++generation;
					//
					// Get rid of unnecessary "removed" entries.  If we
					// don't purge them, then searching for an element
					// that isn't in the hash table will become an O(n)
					// operation as the hash table ages.
					//
					if (num != 0 && table[(found + 1) % capacity].key != null)
						table[found].key = removed;
					else
					{
						do
						{
							table[found].key = null;
							found = (found == 0 ? capacity : found) - 1;
						} while (table[found].key == removed);
					}
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
	[IndexerName("Item")]
	public virtual Object this[Object key]
			{
				get
				{
					int insertionPoint;
					int found = FindSlot(key, out insertionPoint);
					return found == -1 ?  null : table[found].value;
				}
				set
				{
					int insertionPoint;
					int found = FindSlot(key, out insertionPoint);
					if (insertionPoint == -1)
					{
						Expand();
						FindSlot(key, out insertionPoint);
					}
					table[insertionPoint].value = value;
					if (found == -1)
					{
						table[insertionPoint].key = key;
						++num;
					}
					++generation;
				}
			}
	public virtual ICollection Keys
			{
				get
				{
					return new HashtableKeyValueCollection(this, true);
				}
			}
	public virtual ICollection Values
			{
				get
				{
					return new HashtableKeyValueCollection(this, false);
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return new HashtableEnum(this);
			}

	// Determine if this hash table contains a specific key.
	public virtual bool ContainsKey(Object key)
			{
				int insertionPoint;
				return FindSlot(key, out insertionPoint) != -1;
			}

	// Determine if this hash table contains a specific value.
	public virtual bool ContainsValue(Object value)
			{
				int posn;
				for(posn = capacity - 1; posn >= 0; --posn)
				{
					if(table[posn].key != null && table[posn].key != removed &&
					   table[posn].value == value)
					{
						return true;
					}
				}
				return false;
			}

	// Get the hash value for a key.
	protected virtual int GetHash(Object key)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
			#if ECMA_COMPAT
				if(hcp__ != null)
				{
					return hcp__.GetHashCode(key);
				}
			#else
				IHashCodeProvider provider = hcp;
				if(provider != null)
				{
					return provider.GetHashCode(key);
				}
			#endif
				else
				{
					return key.GetHashCode();
				}
			}

	//
	// Find the position of the key in the hash table.
	//
	// Returns: the hash of the found key, or -1 if not found.
	//
	// out insertionPoint: the place the key can be inserted into the
	// hash table, or -1 if adding this key would cause the the hashtable 
	// to execeed its capacityLimit.
	//
	private int FindSlot(Object key, out int insertionPoint)
			{
				insertionPoint = -1;
				if(capacity == 0)
				{
					return -1;
				}
				int hash = (int)((uint)GetHash(key) % (uint)capacity);
				for (int count = capacity; count > 0; count -= 1)
				{
					if (table[hash].key == null)
					{
						if (insertionPoint == -1)
							insertionPoint = hash;
						break;
					}
					if(table[hash].key == removed)
					{
						if (insertionPoint == -1)
						{
							insertionPoint = hash;
						}
					}
					else if(KeyEquals(table[hash].key, key))
					{
						insertionPoint = hash;
						return hash;
					}
					hash = (hash + 1) % capacity;
				}
				if(num >= capacityLimit)
				{
					insertionPoint = -1;
				}
				return -1;
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this object.
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("LoadFactor", loadFactor);
				info.AddValue("Version", generation);
				info.AddValue("Comparer", comparer, typeof(IComparer));
				info.AddValue("HashCodeProvider", hcp,
							  typeof(IHashCodeProvider));
				info.AddValue("HashSize", (table == null ? 0 : table.Length));

				Object[] keyTemp = new Object [num];
				Keys.CopyTo(keyTemp, 0);
				info.AddValue("Keys", keyTemp, typeof(Object[]));

				Object[] valTemp = new Object [num];
				Values.CopyTo(valTemp, 0);
				info.AddValue("Values", valTemp, typeof(Object[]));
			}

	// Process a deserialization callback.
	public virtual void OnDeserialization(Object sender)
			{
				// If the table is non-null, then we've been re-entered.
				if(table != null)
				{
					return;
				}

				// Bail out if there is no de-serialization information.
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}

				// De-serialize the main parameter values.
				try
				{
					loadFactor = info.GetSingle("LoadFactor");
					if(loadFactor < 0.1f)
					{
						loadFactor = 0.1f;
					}
					else if(loadFactor > 1.0f)
					{
						loadFactor = 1.0f;
					}
				}
				catch(NotImplementedException)
				{
					// Floating point not supported by the runtime engine.
				}
				generation = info.GetInt32("Version");
				comparer = (IComparer)(info.GetValue
						("Comparer", typeof(IComparer)));
				hcp = (IHashCodeProvider)(info.GetValue
						("HashCodeProvider", typeof(IHashCodeProvider)));
				int hashSize = info.GetInt32("HashSize");
				if(hashSize > 0)
				{
					capacity = hashSize;
					try
					{
						capacityLimit = (int)(hashSize * loadFactor);
					}
					catch(NotImplementedException)
					{
						// Floating point not supported by the runtime engine.
						capacityLimit = capacity;
					}
					table = new HashBucket [hashSize];
				}

				// Get the key and value arrays from the serialization data.
				Object[] keys = (Object[])(info.GetValue
						("Keys", typeof(Object[])));
				Object[] values = (Object[])(info.GetValue
						("Values", typeof(Object[])));
				if(keys == null || values == null)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				if(keys.Length != values.Length)
				{
					throw new SerializationException
						(_("Serialize_KeyValueMismatch"));
				}

				// Add the (key, value) pairs to the hash table.
				int posn;
				for(posn = 0; posn < keys.Length; ++posn)
				{
					Add(keys[posn], values[posn]);
				}

				// De-serialization is finished.
				info = null;
			}

#endif // CONFIG_SERIALIZATION

	// Determine if an item is equal to a key value.
	protected virtual bool KeyEquals(Object item, Object key)
			{
				if(item == null)
				{
					throw new ArgumentNullException("item");
				}
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
			#if ECMA_COMPAT
				if(comparer__ != null)
				{
					return (comparer__.Compare(item, key) == 0);
				}
			#else
				IComparer cmp = comparer;
				if(cmp != null)
				{
					return (cmp.Compare(item, key) == 0);
				}
			#endif
				else
				{
					return item.Equals(key);
				}
			}

	// Wrap a Hashtable object to make it synchronized.
	public static Hashtable Synchronized(Hashtable table)
			{
				if(table == null)
				{
					throw new ArgumentNullException("table");
				}
				else if(table.IsSynchronized)
				{
					return table;
				}
				else
				{
					return new SynchronizedHashtable(table);
				}
			}

#if !ECMA_COMPAT

	// Get the hash code provider that is being used by this instance.
	protected IHashCodeProvider hcp
			{
				get
				{
					return hcp__;
				}
				set
				{
					hcp__ = value;
				}
			}

	// Get the comparer that is being used by this instance.
	protected IComparer comparer
			{
				get
				{
					return comparer__;
				}
				set
				{
					comparer__ = value;	
				}
			}

#if	false
	public void dump(System.IO.TextWriter writer)
			{
				if (table == null)
				{
					writer.WriteLine("HastTable: null");
					return;
				}
				for (int i = 0; i < table.Length; i += 1)
				{
					if (table[i].key == null)
						continue;
					if (table[i].key == removed)
					{
						writer.WriteLine("Hashtable[" + i + "] removed");
						continue;
					}
					writer.WriteLine("Hashtable[" + i + "] key=" + table[i].key);
				}
				writer.WriteLine("HastTable: END size=" + table.Length);
			}
#endif

#endif // !ECMA_COMPAT

	// Synchronized hash table implementation.
	//
	// Note: We lock every operation on the underlying hash table,
	// even if it is a read or enumerator operation.  This is because
	// we cannot guarantee correct behaviour in symmetric multi-processing
	// environments if we only lock write operations.
	private sealed class SynchronizedHashtable : Hashtable, IEnumerable
	{
		// Internal state.
		private new Hashtable table;

		// Constructor.
		public SynchronizedHashtable(Hashtable table)
				{
					this.table = table;
				}
#if CONFIG_SERIALIZATION
		internal SynchronizedHashtable(SerializationInfo info,
									   StreamingContext context)
				: base(info, context)
				{
					table = (Hashtable)
						(info.GetValue("ParentTable", typeof(Hashtable)));
					if(table == null)
					{
						throw new SerializationException
							(_("Serialize_StateMissing"));
					}
				}
#endif

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					lock(SyncRoot)
					{
						return new SynchronizedHashtable
							((Hashtable)(table.Clone()));
					}
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int index)
				{
					lock(SyncRoot)
					{
						table.CopyTo(array, index);
					}
				}
		public override int Count
				{
					get
					{
						lock(SyncRoot)
						{
							return table.Count;
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
						return table.SyncRoot;
					}
				}

		// Implement the IDictionary interface.
		public override void Add(Object key, Object value)
				{
					lock(SyncRoot)
					{
						table.Add(key, value);
					}
				}
		public override void Clear()
				{
					lock(SyncRoot)
					{
						table.Clear();
					}
				}
		public override bool Contains(Object key)
				{
					lock(SyncRoot)
					{
						return table.Contains(key);
					}
				}
		public override IDictionaryEnumerator GetEnumerator()
				{
					lock(SyncRoot)
					{
						return new SynchronizedDictEnumerator
							(SyncRoot, table.GetEnumerator());
					}
				}
		public override void Remove(Object key)
				{
					lock(SyncRoot)
					{
						table.Remove(key);
					}
				}
		public override bool IsFixedSize
				{
					get
					{
						lock(SyncRoot)
						{
							return table.IsFixedSize;
						}
					}
				}
		public override bool IsReadOnly
				{
					get
					{
						lock(SyncRoot)
						{
							return table.IsReadOnly;
						}
					}
				}
		public override Object this[Object key]
				{
					get
					{
						lock(SyncRoot)
						{
							return table[key];
						}
					}
					set
					{
						lock(SyncRoot)
						{
							table[key] = value;
						}
					}
				}
		public override ICollection Keys
				{
					get
					{
						lock(SyncRoot)
						{
							return table.Keys;
						}
					}
				}
		public override ICollection Values
				{
					get
					{
						lock(SyncRoot)
						{
							return table.Values;
						}
					}
				}
	
		// Implement the IEnumerable interface.
		IEnumerator IEnumerable.GetEnumerator()
				{
					lock(SyncRoot)
					{
						return new SynchronizedEnumerator
							(SyncRoot, ((IEnumerable)table).GetEnumerator());
					}
				}

		// Determine if this hash table contains a specific key.
		public override bool ContainsKey(Object key)
				{
					lock(SyncRoot)
					{
						return table.ContainsKey(key);
					}
				}

		// Determine if this hash table contains a specific value.
		public override bool ContainsValue(Object value)
				{
					lock(SyncRoot)
					{
						return table.ContainsValue(value);
					}
				}

		// Get the hash value for a key.
		protected override int GetHash(Object key)
				{
					// We don't lock this because it does not modify
					// the underlying hash table, or access fields
					// that may be modified by other threads.
					return table.GetHash(key);
				}

		// Determine if an item is equal to a key value.
		protected override bool KeyEquals(Object item, Object key)
				{
					// We don't lock this because it does not modify
					// the underlying hash table, or access fields
					// that may be modified by other threads.
					return table.KeyEquals(item, key);
				}

#if CONFIG_SERIALIZATION

		// Get the serialization data for this object.
		public override void GetObjectData(SerializationInfo info,
										   StreamingContext context)
				{
					if(info == null)
					{
						throw new ArgumentNullException("info");
					}
					info.AddValue("ParentTable", table, typeof(Hashtable));
				}

		// Process a deserialization callback.
		public override void OnDeserialization(Object sender)
				{
					// Nothing to do here for synchronized hash tables.
				}

#endif // CONFIG_SERIALIZATION

	}; // SynchronizedHashtable

	// Hashtable collection and dictionary enumerator.
	private class HashtableEnum : IDictionaryEnumerator
	{
		// Internal state.
		protected Hashtable table;
		protected int       generation;
		protected int		posn;

		// Constructor.
		public HashtableEnum(Hashtable table)
				{
					this.table = table;
					generation = table.generation;
					posn = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(table.generation != generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					while(++posn < table.capacity)
					{
						if(table.table[posn].key != null && table.table[posn].key != removed)
						{
							return true;
						}
					}
					posn = table.capacity;
					return false;
				}
		public void Reset()
				{
					if(table.generation != generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					posn = -1;
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
						if(table.generation != generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(posn < 0 || posn >= table.capacity)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return new DictionaryEntry(table.table[posn].key,
												   table.table[posn].value);
					}
				}
		public Object Key
				{
					get
					{
						if(table.generation != generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(posn < 0 || posn >= table.capacity)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return table.table[posn].key;
					}
				}
		public Object Value
				{
					get
					{
						if(table.generation != generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(posn < 0 || posn >= table.capacity)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return table.table[posn].value;
					}
				}

	}; // class HashtableEnum

	// Key/value enumerator class.
	private sealed class HashtableKeyValueEnumerator : IEnumerator
	{
		// Internal state.
		private IDictionaryEnumerator e;
		private bool keys;

		// Constructor.
		public HashtableKeyValueEnumerator(IDictionaryEnumerator e, bool keys)
				{
					this.e = e;
					this.keys = keys;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					return e.MoveNext();
				}
		public void Reset()
				{
					e.Reset();
				}
		public Object Current
				{
					get
					{
						if(keys)
						{
							return e.Key;
						}
						else
						{
							return e.Value;
						}
					}
				}

	}; // class HashtableKeyValueEnumerator

	// Collection access to the keys or values in a hash table.
	private sealed class HashtableKeyValueCollection : ICollection
	{
		// Internal state.
		private Hashtable table;
		private bool keys;

		// Constructor.
		public HashtableKeyValueCollection(Hashtable table, bool keys)
				{
					this.table = table;
					this.keys = keys;
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
					else if(index > (array.GetLength(0) - table.Count))
					{
						throw new ArgumentException(_("Arg_InvalidArrayRange"));
					}
					else
					{
						IDictionaryEnumerator e = table.GetEnumerator();
						while(e.MoveNext())
						{
							if(keys)
							{
								array.SetValue(e.Key, index++);
							}
							else
							{
								array.SetValue(e.Value, index++);
							}
						}
					}
				}
		public int Count
				{
					get
					{
						return table.Count;
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

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return new HashtableKeyValueEnumerator
						(table.GetEnumerator(), keys);
				}

	}; // class HashtableKeyCollection

}; // class Hashtable

}; // namespace System.Collections
