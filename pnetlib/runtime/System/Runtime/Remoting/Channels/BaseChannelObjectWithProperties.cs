/*
 * BaseChannelObjectWithProperties.cs - Implementation of the
 *	"System.Runtime.Remoting.Channels.BaseChannelObjectWithProperties" class.
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

public abstract class BaseChannelObjectWithProperties
	: IDictionary, ICollection, IEnumerable
{

	// Constructor.
	public BaseChannelObjectWithProperties() {}

	// Implement the IDictionary interface.
	public virtual void Add(Object key, Object value)
			{
				// Normally overrridden by subclasses.
				throw new NotSupportedException();
			}
	public virtual void Clear()
			{
				// Normally overrridden by subclasses.
				throw new NotSupportedException();
			}
	public virtual bool Contains(Object key)
			{
				ICollection keys = Keys;
				if(keys != null && key != null)
				{
					foreach(Object value in keys)
					{
						if(key.Equals(value))
						{
							return true;
						}
					}
					return false;
				}
				else
				{
					return false;
				}
			}
	public virtual IDictionaryEnumerator GetEnumerator()
			{
				return new BaseObjectEnumerator(this);
			}
	public virtual void Remove(Object key)
			{
				// Normally overrridden by subclasses.
				throw new NotSupportedException();
			}
	public virtual bool IsFixedSize
			{
				get
				{
					return true;
				}
			}
	public virtual bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
	public virtual Object this[Object key]
			{
				get
				{
					return null;
				}
				set
				{
					throw new NotImplementedException();
				}
			}
	public virtual ICollection Keys
			{
				get
				{
					// Normally overridden by subclasses.
					return null;
				}
			}
	public virtual ICollection Values
			{
				get
				{
					ICollection keys = Keys;
					if(keys == null)
					{
						return null;
					}
					ArrayList list = new ArrayList();
					foreach(Object obj in keys)
					{
						list.Add(this[obj]);
					}
					return list;
				}
			}

	// Implement the ICollection interface.
	public virtual void CopyTo(Array array, int index)
			{
				// Normally overrridden by subclasses.
				throw new NotSupportedException();
			}
	public virtual int Count
			{
				get
				{
					ICollection keys = Keys;
					if(keys != null)
					{
						return keys.Count;
					}
					else
					{
						return 0;
					}
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

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return new BaseObjectEnumerator(this);
			}

	// Get the properties associated with this object.
	public virtual IDictionary Properties
			{
				get
				{
					return this;
				}
			}

	// Enumerator for this class.
	private sealed class BaseObjectEnumerator : IDictionaryEnumerator
	{
		// Internal state.
		private BaseChannelObjectWithProperties obj;
		private IEnumerator enumerator;

		// Constructor.
		public BaseObjectEnumerator(BaseChannelObjectWithProperties obj)
				{
					this.obj = obj;
					ICollection keys = obj.Keys;
					if(keys != null)
					{
						enumerator = keys.GetEnumerator();
					}
					else
					{
						enumerator = null;
					}
				}

		// Implement the IEnumerable interface.
		public bool MoveNext()
				{
					if(enumerator != null)
					{
						return enumerator.MoveNext();
					}
					else
					{
						return false;
					}
				}
		public void Reset()
				{
					if(enumerator != null)
					{
						enumerator.Reset();
					}
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
						return new DictionaryEntry(Key, Value);
					}
				}
		public Object Key
				{
					get
					{
						if(enumerator != null)
						{
							return enumerator.Current;
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
						return obj[Key];
					}
				}

	}; // class BaseObjectEnumerator

}; // class BaseChannelObjectWithProperties

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
