/*
 * HandlerCollection.cs - Implementation of the
 *		"I18N.Common.HandlerCollection" class.
 *
 * Copyright (c) 2003  Southern Storm Software, Pty Ltd
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

namespace I18N.Common
{

using System;
using System.Collections;

// This class manages a collection of handler names and the
// assemblies that implement them.  Previously we used a hash
// table, but it incurred a large application startup cost.

public sealed class HandlerCollection : IDictionary
{

	// Internal state.
	private String[] names;
	private String[] namespaces;
	private int numHandlers;

	// Estimate of the number of handlers (should be >= the number
	// of lines in the "I18N-handlers.def" file).
	private const int HandlerCountEstimate = 600;

	// Constructor.
	public HandlerCollection()
			{
				names = new String [HandlerCountEstimate];
				namespaces = new String [HandlerCountEstimate];
				numHandlers = 0;
			}

	// Find the index of a specific name.
	public int IndexOf(String name)
			{
				int posn;
				for(posn = 0; posn < numHandlers; ++posn)
				{
					if(names[posn] == name)
					{
						return posn;
					}
				}
				return -1;
			}

	// Implement the IDictionary interface.
	public void Add(Object key, Object value)
			{
				if(numHandlers >= names.Length)
				{
					String[] newNames = new String [names.Length + 32];
					String[] newNamespaces = new String [names.Length + 32];
					Array.Copy(names, 0, newNames, 0, names.Length);
					Array.Copy(namespaces, 0, newNamespaces, 0, names.Length);
					names = newNames;
					namespaces = newNamespaces;
				}
				names[numHandlers] = (String)key;
				namespaces[numHandlers] = (String)value;
				++numHandlers;
			}
	public void Clear()
			{
				numHandlers = 0;
			}
	public bool Contains(Object key)
			{
				return (IndexOf((String)key) != -1);
			}
	public IDictionaryEnumerator GetEnumerator()
			{
				return new HandlerCollectionEnumerator(this);
			}
	public void Remove(Object key)
			{
				// Not used in this implementation.
				throw new InvalidOperationException();
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
					int index = IndexOf((String)key);
					if(index != -1)
					{
						return namespaces[index];
					}
					else
					{
						return null;
					}
				}
				set
				{
					Add(key, value);
				}
			}
	public ICollection Keys
			{
				get
				{
					// Not used in this implementation.
					throw new InvalidOperationException();
				}
			}
	public ICollection Values
			{
				get
				{
					// Not used in this implementation.
					throw new InvalidOperationException();
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				// Not used in this implementation.
				throw new InvalidOperationException();
			}
	public int Count
			{
				get
				{
					return numHandlers;
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
				return new HandlerCollectionEnumerator(this);
			}

	// Enumerator class for this collection.
	private sealed class HandlerCollectionEnumerator : IDictionaryEnumerator
	{
		// Internal state.
		private HandlerCollection coll;
		private int posn;

		// Constructor.
		public HandlerCollectionEnumerator(HandlerCollection coll)
				{
					this.coll = coll;
					this.posn = -1;
				}

		// Implement the IEnumerator class.
		public bool MoveNext()
				{
					++posn;
					return (posn < coll.numHandlers);
				}
		public void Reset()
				{
					posn = -1;
				}
		public Object Current
				{
					get
					{
						if(posn >= 0 && posn < coll.numHandlers)
						{
							return new DictionaryEntry
								(coll.names[posn], coll.namespaces[posn]);
						}
						throw new InvalidOperationException();
					}
				}

		// Implement the IDictionaryEnumerator class.
		public DictionaryEntry Entry
				{
					get
					{
						if(posn >= 0 && posn < coll.numHandlers)
						{
							return new DictionaryEntry
								(coll.names[posn], coll.namespaces[posn]);
						}
						throw new InvalidOperationException();
					}
				}
		public Object Key
				{
					get
					{
						if(posn >= 0 && posn < coll.numHandlers)
						{
							return coll.names[posn];
						}
						throw new InvalidOperationException();
					}
				}
		public Object Value
				{
					get
					{
						if(posn >= 0 && posn < coll.numHandlers)
						{
							return coll.namespaces[posn];
						}
						throw new InvalidOperationException();
					}
				}

	}; // class HandlerCollectionEnumerator

}; // class HandlerCollection

}; // namespace I18N.Common
