/*
 * HybridDictionary.cs - Implementation of
 *		"System.Collections.Specialized.HybridDictionary".
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
using System.Globalization;

public class HybridDictionary : IDictionary, ICollection, IEnumerable
{

	// Internal state.  The object starts off as a list dictionary
	// and then switches to a hash table when the number of elements
	// exceeds "SwitchOverSize".
	private ListDictionary list;
	private Hashtable hash;
	private bool caseInsensitive;

	// The size at which we switch over.
	private const int SwitchOverSize = 8;

	// Constructors.
	public HybridDictionary() : this(0, false) {}
	public HybridDictionary(bool caseInsensitive)
			: this(0, caseInsensitive) {}
	public HybridDictionary(int initialSize)
			: this(initialSize, false) {}
	public HybridDictionary(int initialSize, bool caseInsensitive)
			{
				this.caseInsensitive = caseInsensitive;
				if(initialSize > SwitchOverSize)
				{
					SwitchOver(initialSize);
				}
				else
				{
					list = new ListDictionary();
				}
			}

	// Switch over to the hash table implementation.
	private void SwitchOver(int size)
			{
				if(caseInsensitive)
				{
					hash = new Hashtable
						(size, new CaseInsensitiveHashCodeProvider
								(CultureInfo.InvariantCulture),
						 new CaseInsensitiveComparer
								(CultureInfo.InvariantCulture));
				}
				else
				{
					hash = new Hashtable(size);
				}
				if(list != null)
				{
					IDictionaryEnumerator e = list.GetEnumerator();
					while(e.MoveNext())
					{
						hash.Add(e.Key, e.Value);
					}
					list = null;
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				if(hash != null)
				{
					hash.CopyTo(array, index);
				}
				else
				{
					list.CopyTo(array, index);
				}
			}
	public int Count
			{
				get
				{
					if(hash != null)
					{
						return hash.Count;
					}
					else
					{
						return list.Count;
					}
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

	// Implement the IDictionary interface.
	public void Add(Object key, Object value)
			{
				if(hash != null)
				{
					hash.Add(key, value);
				}
				else if(list.Count < SwitchOverSize)
				{
					list.Add(key, value);
				}
				else
				{
					SwitchOver(SwitchOverSize + 1);
					hash.Add(key, value);
				}
			}
	public void Clear()
			{
				if(hash != null)
				{
					hash.Clear();
					hash = null;
					list = new ListDictionary();
				}
				else if(list != null)
				{
					list.Clear();
				}
			}
	public bool Contains(Object key)
			{
				if(hash != null)
				{
					return hash.Contains(key);
				}
				else
				{
					return list.Contains(key);
				}
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				if(hash != null)
				{
					return hash.GetEnumerator();
				}
				else
				{
					return list.GetEnumerator();
				}
			}
	public void Remove(Object key)
			{
				if(hash != null)
				{
					hash.Remove(key);
				}
				else
				{
					list.Remove(key);
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
					if(hash != null)
					{
						return hash[key];
					}
					else
					{
						return list[key];
					}
				}
				set
				{
					if(hash != null)
					{
						hash[key] = value;
					}
					else
					{
						list[key] = value;
					}
				}
			}
	public ICollection Keys
			{
				get
				{
					if(hash != null)
					{
						return hash.Keys;
					}
					else
					{
						return list.Keys;
					}
				}
			}
	public ICollection Values
			{
				get
				{
					if(hash != null)
					{
						return hash.Values;
					}
					else
					{
						return list.Values;
					}
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				if(hash != null)
				{
					return ((IEnumerable)hash).GetEnumerator();
				}
				else
				{
					return ((IEnumerable)list).GetEnumerator();
				}
			}

}; // class HybridDictionary

#endif // !ECMA_COMPAT

}; // namespace System.Collections.Specialized
