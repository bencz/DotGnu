/*
 * CombinedDictionary.cs - Implementation of the
 *	"System.Runtime.Remoting.Channels.CombinedDictionary" class.
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

namespace System.Runtime.Remoting.Channels
{

#if CONFIG_REMOTING

using System.Collections;

// Class that combines multiple dictionaries into one.

internal class CombinedDictionary : IDictionary, ICollection, IEnumerable
{
	// Internal state.
	private ArrayList members;

	// Constructor.
	public CombinedDictionary(ArrayList members)
			{
				this.members = members;
			}

	// Implement the IDictionary interface.
	public void Add(Object key, Object value)
			{
				throw new NotSupportedException();
			}
	public void Clear()
			{
				throw new NotSupportedException();
			}
	public bool Contains(Object key)
			{
				foreach(IDictionary dict in members)
				{
					if(dict.Contains(key))
					{
						return true;
					}
				}
				return false;
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				return new CombinedEnumerator(this, Keys);
			}
	public void Remove(Object key)
			{
				throw new NotSupportedException();
			}
	public bool IsFixedSize
			{
				get
				{
					return true;
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
					foreach(IDictionary dict in members)
					{
						if(dict.Contains(key))
						{
							return dict[key];
						}
					}
					return null;
				}
				set
				{
					foreach(IDictionary dict in members)
					{
						if(dict.Contains(key))
						{
							dict[key] = value;
						}
					}
				}
			}
	public ICollection Keys
			{
				get
				{
					ArrayList keys = new ArrayList();
					foreach(IDictionary dict in members)
					{
						IDictionaryEnumerator e = dict.GetEnumerator();
						while(e.MoveNext())
						{
							keys.Add(e.Key);
						}
					}
					return keys;
				}
			}
	public ICollection Values
			{
				get
				{
					ArrayList values = new ArrayList();
					foreach(IDictionary dict in members)
					{
						IDictionaryEnumerator e = dict.GetEnumerator();
						while(e.MoveNext())
						{
							values.Add(e.Value);
						}
					}
					return values;
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				throw new NotSupportedException();
			}
	public int Count
			{
				get
				{
					int count = 0;
					foreach(IDictionary dict in members)
					{
						count += dict.Count;
					}
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
				return GetEnumerator();
			}

	// Enumerator class for "CombinedDictionary".
	private sealed class CombinedEnumerator : IDictionaryEnumerator
	{
		// Internal state.
		private CombinedDictionary dict;
		private ArrayList keys;
		private int index;

		// Constructor.
		public CombinedEnumerator(CombinedDictionary dict,
								  ICollection keys)
				{
					this.dict = dict;
					this.keys = (keys as ArrayList);
					this.index = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					++index;
					return (index < keys.Count);
				}
		public void Reset()
				{
					index = -1;
				}
		public Object Current
				{
					get
					{
						return new DictionaryEntry(Key, Value);
					}
				}

		// Implement the IDictionaryEnumerator interface.
		public DictionaryEntry Entry
				{
					get
					{
						return new DictionaryEntry(Key, Value);
					}
				}
		public Object Key
				{
					get
					{
						if(index < 0 || index >= keys.Count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return keys[index];
					}
				}
		public Object Value
				{
					get
					{
						if(index < 0 || index >= keys.Count)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return dict[keys[index]];
					}
				}

	}; // class CombinedEnumerator

}; // class CombinedDictionary

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
