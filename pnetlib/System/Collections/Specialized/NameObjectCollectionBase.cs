/*
 * NameObjectCollectionBase.cs - Implementation of
 *		"System.Collections.Specialized.NameObjectCollectionBase".
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

using System;
using System.Globalization;
using System.Collections;
using System.Runtime.Serialization;

#if !ECMA_COMPAT
public
#else
internal
#endif
abstract class NameObjectCollectionBase
			: ICollection, IEnumerable
#if CONFIG_SERIALIZATION
			, ISerializable, IDeserializationCallback
#endif
{

	// Internal state.  We implement our own hash table rather than
	// use the Hashtable class, because we have to handle multiple
	// values per key, which Hashtable cannot do.  Working around
	// Hashtable's foibles can give unreliable behaviour as entries
	// are added and removed.
	private Entry[] table;
	private IHashCodeProvider hcp;
	private IComparer cmp;
	private ArrayList entries;
	private bool readOnly;
	private const int HashTableSize = 61;
#if CONFIG_SERIALIZATION
	private SerializationInfo info;
#endif

	// Constructors.
	protected NameObjectCollectionBase()
			: this(0, null, null) {}
	protected NameObjectCollectionBase(int capacity)
			: this(capacity, null, null) {}
	protected NameObjectCollectionBase(IHashCodeProvider hashProvider,
									   IComparer comparer)
			: this(0, hashProvider, comparer) {}
	protected NameObjectCollectionBase(int capacity,
									   IHashCodeProvider hashProvider,
									   IComparer comparer)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", S._("ArgRange_NonNegative"));
				}
				if(hashProvider == null)
				{
					hashProvider = CaseInsensitiveHashCodeProvider.Default;
				}
				if(comparer == null)
				{
					comparer = CaseInsensitiveComparer.Default;
				}
				table = new Entry [HashTableSize];
				hcp = hashProvider;
				cmp = comparer;
				entries = new ArrayList(capacity);
				readOnly = false;
			}
#if CONFIG_SERIALIZATION
	protected NameObjectCollectionBase(SerializationInfo info,
									   StreamingContext context)
			: this(0, null, null)
			{
				this.info = info;
			}
#endif

	// Properties.
	public virtual KeysCollection Keys
			{
				get
				{
					return new KeysCollection(this);
				}
			}
	protected bool IsReadOnly
			{
				get
				{
					return readOnly;
				}
				set
				{
					readOnly = value;
				}
			}

	// Implement the ICollection interface.
	public virtual int Count
			{
				get
				{
					return entries.Count;
				}
			}
	void ICollection.CopyTo(Array array, int index)
			{
				foreach(Object obj in this)
				{
					array.SetValue(obj, index);
					++index;
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

	// Implement the IEnumerator interface.
	public IEnumerator GetEnumerator()
			{
				return new KeysEnumerator(this);
			}

#if CONFIG_SERIALIZATION

	// Implement the ISerializable interface.
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				// Validate the parameters.
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}

				// Add general information.
				info.AddValue("ReadOnly", readOnly);
				info.AddValue("HashProvider", hcp, typeof(IHashCodeProvider));
				info.AddValue("Comparer", cmp, typeof(IComparer));
				info.AddValue("Count", entries.Count);

				// Build arrays for the keys and values and serialize them.
				String[] keys = new String [entries.Count];
				Object[] values = new Object [entries.Count];
				int posn;
				Entry entry;
				for(posn = 0; posn < entries.Count; ++posn)
				{
					entry = (Entry)(entries[posn]);
					keys[posn] = entry.key;
					values[posn] = entry.value;
				}
				info.AddValue("Keys", keys, typeof(String[]));
				info.AddValue("Values", values, typeof(Object[]));
			}

	// Implement the IDeserializationCallback interface.
	public virtual void OnDeserialization(Object sender)
			{
				// Bail out if we've already been deserialized.
				if(hcp != null)
				{
					return;
				}

				// Validate the deserialization state.
				if(info == null)
				{
					throw new SerializationException
						(S._("Serialize_StateMissing"));
				}

				// De-serialize the hash provider and comparer.
				hcp = (IHashCodeProvider)(info.GetValue
							("HashProvider", typeof(IHashCodeProvider)));
				cmp = (IComparer)(info.GetValue
							("Comparer", typeof(IComparer)));
				if(hcp == null || cmp == null)
				{
					throw new SerializationException
						(S._("Serialize_StateMissing"));
				}

				// De-serialize the key/value arrays.
				String[] keys = (String[])(info.GetValue
						("Keys", typeof(String[])));
				Object[] values = (String[])(info.GetValue
						("Values", typeof(Object[])));
				if(keys == null || values == null)
				{
					throw new SerializationException
						(S._("Serialize_StateMissing"));
				}
				int count = info.GetInt32("Count");
				int posn;
				for(posn = 0; posn < count; ++posn)
				{
					BaseAdd(keys[posn], values[posn]);
				}

				// De-serialize the read-only flag last.
				readOnly = info.GetBoolean("ReadOnly");

				// De-serialization is complete.
				info = null;
			}

#endif // CONFIG_SERIALIZATION

	// Get the hash value for a string, restricted to the table size.
	private int GetHash(String name)
			{
				int hash = (name != null ? hcp.GetHashCode(name) : 0);
				return (int)(((uint)hash) % (uint)HashTableSize);
			}

	// Compare two keys for equality.
	private bool Compare(String key1, String key2)
			{
				if(key1 == null || key2 == null)
				{
					return (key1 == key2);
				}
				else
				{
					return (cmp.Compare(key1, key2) == 0);
				}
			}

	// Add a name/value pair to this collection.
	protected void BaseAdd(String name, Object value)
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				Entry entry = new Entry(name, value);
				int hash = GetHash(name);
				Entry last = table[hash];
				if(last == null)
				{
					table[hash] = entry;
				}
				else
				{
					while(last.next != null)
					{
						last = last.next;
					}
					last.next = entry;
				}
				entries.Add(entry);
			}

	// Clear this collection.
	protected void BaseClear()
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				((IList)table).Clear();
				entries.Clear();
			}

	// Get the item at a specific index within this collection.
	protected Object BaseGet(int index)
			{
				return ((Entry)(entries[index])).value;
			}

	// Get the item associated with a specific name within this collection.
	protected Object BaseGet(String name)
			{
				Entry entry = table[GetHash(name)];
				while(entry != null)
				{
					if(Compare(entry.key, name))
					{
						return entry.value;
					}
					entry = entry.next;
				}
				return null;
			}

	// Get a list of all keys in the collection.
	protected String[] BaseGetAllKeys()
			{
				String[] keys = new String [entries.Count];
				int index = 0;
				foreach(Entry entry in entries)
				{
					keys[index++] = (String)(entry.key);
				}
				return keys;
			}

	// Get a list of all values in the collection.
	protected Object[] BaseGetAllValues()
			{
				Object[] values = new String [entries.Count];
				int index = 0;
				foreach(Entry entry in entries)
				{
					values[index++] = entry.value;
				}
				return values;
			}

	// Get an array of a specific type of all values in the collection.
	protected Object[] BaseGetAllValues(Type type)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				Object[] values = (Object[])
					Array.CreateInstance(type, entries.Count);
				int index = 0;
				foreach(Entry entry in entries)
				{
					values[index++] = entry.value;
				}
				return values;
			}

	// Get the key at a specific index.
	protected String BaseGetKey(int index)
			{
				return ((Entry)(entries[index])).key;
			}

	// Determine if there a non-null keys in the collection.
	protected bool BaseHasKeys()
			{
				Entry entry = table[GetHash(null)];
				while(entry != null)
				{
					if(entry.key != null)
					{
						return true;
					}
					entry = entry.next;
				}
				return false;
			}

	// Remove all entries with a specific key.
	protected void BaseRemove(String name)
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				int hash = GetHash(name);
				Entry entry = table[hash];
				Entry prev = null;
				while(entry != null)
				{
					if(Compare(entry.key, name))
					{
						if(prev != null)
						{
							prev.next = entry.next;
						}
						else
						{
							table[hash] = entry.next;
						}
						entry = entry.next;
					}
					else
					{
						prev = entry;
						entry = entry.next;
					}
				}
				int count = entries.Count;
				int posn = 0;
				while(posn < count)
				{
					if(Compare(((Entry)(entries[posn])).key, name))
					{
						entries.RemoveAt(posn);
						--count;
					}
					else
					{
						++posn;
					}
				}
			}

	// Remove a specific entry by index.
	protected void BaseRemoveAt(int index)
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				Entry entry = (Entry)(entries[index]);
				entries.RemoveAt(index);
				int hash = GetHash(entry.key);
				Entry find = table[hash];
				Entry prev = null;
				while(find != null)
				{
					if(find == entry)
					{
						if(prev != null)
						{
							prev.next = find.next;
						}
						else
						{
							table[hash] = find.next;
						}
						return;
					}
					else
					{
						prev = find;
						find = find.next;
					}
				}
			}

	// Set the value of an entry at a particular index.
	protected void BaseSet(int index, Object value)
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				((Entry)(entries[index])).value = value;
			}

	// Set the value of the first entry with a particular name.
	protected void BaseSet(String name, Object value)
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				int hash = GetHash(name);
				Entry entry = table[hash];
				while(entry != null)
				{
					if(Compare(entry.key, name))
					{
						entry.value = value;
						return;
					}
					entry = entry.next;
				}
				entry = new Entry(name, value);
				entry.next = table[hash];
				table[hash] = entry;
				entries.Add(entry);
			}

	// Structure of an entry in the collection.
	private class Entry
	{
		public String key;
		public Object value;
		public Entry  next;

		// Constructor.
		public Entry(String key, Object value)
				{
					this.key = key;
					this.value = value;
					this.next = null;
				}

	}; // class Entry

	// Alternate interface to the keys in a name object collection.
	public class KeysCollection : ICollection, IEnumerable
	{
		// Internal state.
		private NameObjectCollectionBase c;

		// Constructor.
		internal KeysCollection(NameObjectCollectionBase c)
				{
					this.c = c;
				}

		// Implement the ICollection interface.
		public int Count
				{
					get
					{
						return c.Count;
					}
				}
		void ICollection.CopyTo(Array array, int index)
				{
					foreach(Object obj in this)
					{
						array.SetValue(obj, index);
						++index;
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
						return c;
					}
				}

		// Implement the IEnumerable interface.
		public IEnumerator GetEnumerator()
				{
					return new KeysEnumerator(c);
				}

		// Get the key at a specific index.
		public String Get(int index)
				{
					return c.BaseGetKey(index);
				}
		public String this[int index]
				{
					get
					{
						return Get(index);
					}
				}

	}; // class KeysCollection

	// Enumerator for the keys in a name object collection.
	private class KeysEnumerator : IEnumerator
	{
		// Internal state.
		private IEnumerator e;

		// Constructor.
		public KeysEnumerator(NameObjectCollectionBase c)
				{
					e = c.entries.GetEnumerator();
				}

		// Implement the IEnumerator interface.
		bool IEnumerator.MoveNext()
				{
					return e.MoveNext();
				}
		void IEnumerator.Reset()
				{
					e.Reset();
				}
		Object IEnumerator.Current
				{
					get
					{
						return ((Entry)(e.Current)).key;
					}
				}

	}; // class KeysEnumerator

#if ECMA_COMPAT

	// Local copy of "System.Collections.CaseInsenstiveHashCodeProvider"
	// for use in ECMA-compatbile systems.
	private class CaseInsensitiveHashCodeProvider : IHashCodeProvider
	{
		private static readonly CaseInsensitiveHashCodeProvider
			defaultProvider = new CaseInsensitiveHashCodeProvider();

		// Internal state.
		private TextInfo info;

		// Get the default comparer instance.
		public static CaseInsensitiveHashCodeProvider Default
				{
					get
					{
						return defaultProvider;
					}
				}

		// Constructors.
		public CaseInsensitiveHashCodeProvider()
				{
					info = CultureInfo.CurrentCulture.TextInfo;
				}
		public CaseInsensitiveHashCodeProvider(CultureInfo culture)
				{
					if(culture == null)
					{
						throw new ArgumentNullException("culture");
					}
					info = culture.TextInfo;
				}

		// Implement the IHashCodeProvider interface.
		public int GetHashCode(Object obj)
				{
					String str = (obj as String);
					if(str != null)
					{
						return info.ToLower(str).GetHashCode();
					}
					else if(obj != null)
					{
						return obj.GetHashCode();
					}
					else
					{
						throw new ArgumentNullException("obj");
					}
				}

	}; // class CaseInsensitiveHashCodeProvider

	// Local copy of "System.Collections.CaseInsenstiveComparer"
	// for use in ECMA-compatbile systems.
	private class CaseInsensitiveComparer : IComparer
	{
		// The default case insensitive comparer instance.
		private static readonly CaseInsensitiveComparer defaultComparer =
			new CaseInsensitiveComparer();
	
		// Internal state.
		private CompareInfo compare;

		// Get the default comparer instance.
		public static CaseInsensitiveComparer Default
				{
					get
					{
						return defaultComparer;
					}
				}

		// Constructors.
		public CaseInsensitiveComparer()
				{
					compare = CultureInfo.CurrentCulture.CompareInfo;
				}
		public CaseInsensitiveComparer(CultureInfo culture)
				{
					if(culture == null)
					{
						throw new ArgumentNullException("culture");
					}
					compare = culture.CompareInfo;
				}

		// Implement the IComparer interface.
		public int Compare(Object a, Object b)
				{
					String stra = (a as String);
					String strb = (b as String);
					if(stra != null && strb != null)
					{
						return compare.Compare
							(stra, strb, CompareOptions.IgnoreCase);
					}
					else
					{
						return Comparer.Default.Compare(a, b);
					}
				}

	}; // class CaseInsensitiveComparer

#endif // ECMA_COMPAT

}; // class NameObjectCollectionBase

}; // namespace System.Collections.Specialized
