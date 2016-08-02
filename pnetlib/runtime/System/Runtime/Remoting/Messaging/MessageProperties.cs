/*
 * MessageProperties.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.MessageProperties" class.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_SERIALIZATION

using System.Collections;

internal class MessageProperties : IDictionary
{
	// Internal state.
	private IMessageDictionary special;
	private IDictionary dict;
	private String[] properties;

	// Constructor.
	public MessageProperties(IMessageDictionary special, IDictionary dict)
			{
				this.special = special;
				this.dict = dict;
				this.properties = special.SpecialProperties;
			}

	// Implement the IDictionary interface.
	void IDictionary.Add(Object key, Object value)
			{
				if(Array.IndexOf(properties, key) != -1)
				{
					throw new ArgumentException
						(_("Remoting_InvalidKey"));
				}
				dict.Add(key, value);
			}
	void IDictionary.Clear()
			{
				dict.Clear();
			}
	bool IDictionary.Contains(Object key)
			{
				if(Array.IndexOf(properties, key) != -1)
				{
					return true;
				}
				return dict.Contains(key);
			}
	IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return new Enumerator(this);
			}
	void IDictionary.Remove(Object key)
			{
				if(Array.IndexOf(properties, key) != -1)
				{
					throw new ArgumentException
						(_("Remoting_InvalidKey"));
				}
				dict.Remove(key);
			}
	bool IDictionary.IsFixedSize
			{
				get
				{
					return false;
				}
			}
	bool IDictionary.IsReadOnly
			{
				get
				{
					return false;
				}
			}
	Object IDictionary.this[Object key]
			{
				get
				{
					if(Array.IndexOf(properties, key) != -1)
					{
						return special.GetSpecialProperty((String)key);
					}
					else
					{
						return dict[key];
					}
				}
				set
				{
					if(Array.IndexOf(properties, key) != -1)
					{
						special.SetSpecialProperty((String)key, value);
					}
					else
					{
						dict[key] = value;
					}
				}
			}
	ICollection IDictionary.Keys
			{
				get
				{
					ArrayList list = new ArrayList();
					foreach(String name in properties)
					{
						list.Add(name);
					}
					list.AddRange(dict.Keys);
					return list;
				}
			}
	ICollection IDictionary.Values
			{
				get
				{
					ArrayList list = new ArrayList();
					foreach(String name in properties)
					{
						list.Add(special.GetSpecialProperty(name));
					}
					list.AddRange(dict.Keys);
					return list;
				}
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				foreach(String name in properties)
				{
					array.SetValue(special.GetSpecialProperty(name), index);
					++index;
				}
				dict.CopyTo(array, index);
			}
	int ICollection.Count
			{
				get
				{
					return properties.Length + dict.Count;
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
	IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(this);
			}

	// Enumerator for a "MessageProperties" dictionary.
	private class Enumerator : IDictionaryEnumerator
	{
		// Internal state.
		private MessageProperties properties;
		private int index;
		private IDictionaryEnumerator e;

		// Constructor.
		public Enumerator(MessageProperties properties)
				{
					this.properties = properties;
					this.index = -1;
					this.e = null;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(e != null)
					{
						return e.MoveNext();
					}
					else
					{
						++index;
						if(index < properties.properties.Length)
						{
							return true;
						}
						e = properties.dict.GetEnumerator();
						return e.MoveNext();
					}
				}
		public void Reset()
				{
					index = -1;
					e = null;
				}
		public Object Current
				{
					get
					{
						return Value;
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
						if(e != null)
						{
							return e.Key;
						}
						else if(index >= 0 &&
							    index < properties.properties.Length)
						{
							return properties.properties[index];
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}
		public Object Value
				{
					get
					{
						if(e != null)
						{
							return e.Value;
						}
						else if(index >= 0 &&
							    index < properties.properties.Length)
						{
							return properties.special.GetSpecialProperty
								(properties.properties[index]);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

	}; // class Enumerator

}; // class MessageProperties

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Messaging
