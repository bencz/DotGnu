/*
 * RangeList.cs - Wrap an IList to access a sub-range.
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

public sealed class RangeList<T> : IList<T>, ICloneable
{
	// Internal state.
	private IList<T> list;
	private int index, count;

	// Constructor.
	public RangeList(IList<T> list, int index, int count)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				if(index < 0 || index > list.Count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("Arg_InvalidArrayIndex"));
				}
				else if(count < 0 || (list.Count - index) < count)
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
				this.list = list;
				this.index = index;
				this.count = count;
			}

	// Implement the IList<T> interface.
	public int Add(T value)
			{
				list.Insert(index + count, value);
				++count;
				return index + count - 1;
			}
	public void Clear()
			{
				if(index == 0 && count == list.Count)
				{
					list.Clear();
					count = 0;
				}
				else
				{
					int posn = index + count - 1;
					while(count > 0)
					{
						list.RemoveAt(posn--);
						--count;
					}
				}
			}
	public bool Contains(T item)
			{
				int posn;
				if(typeof(T).IsValueType)
				{
					for(posn = 0; posn < count; ++posn)
					{
						if(item.Equals(list[index + posn]))
						{
							return true;
						}
					}
					return false;
				}
				else
				{
					if(((Object)item) != null)
					{
						for(posn = 0; posn < count; ++posn)
						{
							if(item.Equals(list[index + posn]))
							{
								return true;
							}
						}
						return false;
					}
					else
					{
						for(posn = 0; posn < count; ++posn)
						{
							if(((Object)(list[index + posn])) == null)
							{
								return true;
							}
						}
						return false;
					}
				}
			}
	public IListIterator<T> GetIterator()
			{
				return new RangeListIterator<T>(this);
			}
	public int IndexOf(T value)
			{
				int posn;
				if(typeof(T).IsValueType)
				{
					for(posn = 0; posn < count; ++posn)
					{
						if(item.Equals(list[index + posn]))
						{
							return posn;
						}
					}
					return -1;
				}
				else
				{
					if(((Object)item) != null)
					{
						for(posn = 0; posn < count; ++posn)
						{
							if(item.Equals(list[index + posn]))
							{
								return posn;
							}
						}
						return -1;
					}
					else
					{
						for(posn = 0; posn < count; ++posn)
						{
							if(((Object)(list[index + posn])) == null)
							{
								return posn;
							}
						}
						return -1;
					}
				}
			}
	public void Insert(int index, T value)
			{
				if(index < 0 || index > count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				list.Insert(this.index + index, value);
				++count;
			}
	public void Remove(T value)
			{
				int posn;
				if(typeof(T).IsValueType)
				{
					for(posn = 0; posn < count; ++posn)
					{
						if(item.Equals(list[index + posn]))
						{
							list.RemoveAt(index + posn);
							return;
						}
					}
				}
				else
				{
					if(((Object)item) != null)
					{
						for(posn = 0; posn < count; ++posn)
						{
							if(item.Equals(list[index + posn]))
							{
								list.RemoveAt(index + posn);
								return;
							}
						}
					}
					else
					{
						for(posn = 0; posn < count; ++posn)
						{
							if(((Object)(list[index + posn])) == null)
							{
								list.RemoveAt(index + posn);
								return;
							}
						}
					}
				}
			}
	public void RemoveAt(int index)
			{
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				list.Remove(this.index + index);
				--count;
			}
	public bool IsRandomAccess
			{
				get
				{
					return list.IsRandomAccess;
				}
			}
	public T this[int index]
			{
				get
				{
					if(index < 0 || index >= count)
					{
						throw new ArgumentOutOfRangeException
							("index", S._("ArgRange_Array"));
					}
					return list[this.index + index];
				}
				set
				{
					if(index < 0 || index >= count)
					{
						throw new ArgumentOutOfRangeException
							("index", S._("ArgRange_Array"));
					}
					list[this.index + index] = value;
				}
			}

	// Implement the ICollection<T> interface.
	public void CopyTo(T[] array, int arrayIndex)
			{
				int posn;
				for(posn = 0; posn < count; ++posn)
				{
					array[arrayIndex++] = list[index + posn];
				}
			}
	public int Count
			{
				get
				{
					return count;
				}
			}
	public bool IsFixedSize
			{
				get
				{
					return list.IsFixedSize;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return list.IsReadOnly;
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return list.IsSynchronized;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return list.SyncRoot;
				}
			}

	// Implement the IIterable<T> interface.
	IIterator<T> IIterable<T>.GetIterator()
			{
				return new RangeListIterator<T>(this);
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				if(list is ICloneable)
				{
					return new RangeList<T>
						((IList<T>)(((ICloneable)list).Clone()),
						 index, count);
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

	// Range list iterator class.
	private sealed class RangeListIterator<T> : IListIterator<T>
	{
		// Internal state.
		private RangeList<T> list;
		private int position;
		private int removed;
		private bool reset;

		// Constructor.
		public RangeListIterator(RangeList<T> list)
				{
					this.list = list;
					position = -1;
					removed = -1;
					reset = true;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					if(reset)
					{
						// Start at the beginning of the range.
						position = 0;
						reset = false;
					}
					else if(removed != -1)
					{
						// An item was removed, so re-visit this position.
						position = removed;
						removed = -1;
					}
					else
					{
						++position;
					}
					return (position < list.count);
				}
		public void Reset()
				{
					reset = true;
					position = -1;
					removed = -1;
				}
		public void Remove()
				{
					if(position < 0 || position >= list.count || removed != -1)
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
					list.RemoveAt(position);
					removed = position;
				}
		T IIterator<T>.Current
				{
					get
					{
						if(position < 0 || position >= list.count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return list.list[position + list.index];
					}
				}

		// Implement the IListIterator<T> interface.
		public bool MovePrev()
				{
					if(reset)
					{
						// Start at the end of the range.
						position = list.count - 1;
						reset = false;
					}
					else if(removed != -1)
					{
						// An item was removed, so move to just before it.
						position = removed - 1;
						removed = -1;
					}
					else
					{
						--position;
					}
					return (position >= 0);
				}
		public int Position
				{
					get
					{
						if(position < 0 || position >= list.count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return position;
					}
				}
		public T Current
				{
					get
					{
						if(position < 0 || position >= list.count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return list.list[position + list.index];
					}
					set
					{
						if(position < 0 || position >= list.Count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						list.list[position + list.index] = value;
					}
				}

	}; // class RangeListIterator<T>

}; // class RangeList<T>

}; // namespace Generics
