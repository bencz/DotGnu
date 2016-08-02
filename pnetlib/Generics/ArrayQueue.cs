/*
 * ArrayQueue.cs - Generic queue class, implemented as an array.
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

public sealed class ArrayQueue<T> : IQueue<T>, ICapacity, ICloneable
{
	// Internal state.
	private T[]   items;
	private int   add, remove, size;

	// The default capacity for queues.
	private const int DefaultCapacity = 32;

	// Constructors.
	public ArrayQueue()
			{
				items = new Object [DefaultCapacity];
				add = 0;
				remove = 0;
				size = 0;
			}
	public ArrayQueue(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", S._("ArgRange_NonNegative"));
				}
				items = new T [capacity];
				add = 0;
				remove = 0;
				size = 0;
			}

	// Implement the ICollection<T> interface.
	public void CopyTo(T[] array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				else if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				else if((array.Length - index) < size)
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
				else if(size > 0)
				{
					if((remove + size) <= items.Length)
					{
						Array.Copy(items, remove, array, index, size);
					}
					else
					{
						Array.Copy(items, remove, array, index,
								   items.Length - remove);
						Array.Copy(items, 0, array,
								   index + items.Length - remove, add);
					}
				}
			}
	public int Count
			{
				get
				{
					return size;
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

	// Implement the ICapacity interface.
	public int Capacity
			{
				get
				{
					return items.Length;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					if(value < size)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("Arg_CannotReduceCapacity"));
					}
					if(value != size)
					{
						T[] newItems = new T [value];
						if(remove < size)
						{
							Array.Copy(items, remove, newItems,
									   0, size - remove);
						}
						if(remove > 0)
						{
							Array.Copy(items, 0, newItems,
									   size - remove, remove);
						}
						items = newItems;
						add = size;
						remove = 0;
					}
				}
			}

	// Implement the ICloneable<T> interface.
	public Object Clone()
			{
				ArrayQueue<T> queue = (ArrayQueue<T>)MemberwiseClone();
				queue.items = (T[])items.Clone();
				return queue;
			}

	// Implement the IIterable<T> interface.
	public IIterable<T> GetIterator()
			{
				return new QueueIterator<T>(this);
			}

	// Implement the IQueue<T> interface.
	public void Clear()
			{
				add = 0;
				remove = 0;
				size = 0;
			}
	public bool Contains(T obj)
			{
				int index = remove;
				int capacity = items.Length;
				int count = size;
				if(typeof(T).IsValueType)
				{
					while(count > 0)
					{
						if(obj.Equals(items[index]))
						{
							return true;
						}
						index = (index + 1) % capacity;
						--count;
					}
				}
				else
				{
					while(count > 0)
					{
						if(items[index] != null && obj != null)
						{
							if(obj.Equals(items[index]))
							{
								return true;
							}
						}
						else if(items[index] == null && obj == null)
						{
							return true;
						}
						index = (index + 1) % capacity;
						--count;
					}
				}
				return false;
			}
	public void Enqueue(T value)
			{
				if(size < items.Length)
				{
					// The queue is big enough to hold the new item.
					items[add] = value;
					add = (add + 1) % items.Length;
					++size;
				}
				else
				{
					// We need to increase the size of the queue.
					int newCapacity = (int)(items.Length * 2);
					if(newCapacity <= items.Length)
					{
						newCapacity = items.Length + 1;
					}
					T[] newItems = new T [newCapacity];
					if(remove < size)
					{
						Array.Copy(items, remove, newItems, 0, size - remove);
					}
					if(remove > 0)
					{
						Array.Copy(items, 0, newItems, size - remove, remove);
					}
					items = newItems;
					add = size;
					remove = 0;
					items[add] = value;
					add = (add + 1) % items.Length;
					++size;
				}
			}
	public T Dequeue()
			{
				if(size > 0)
				{
					T value = items[remove];
					remove = (remove + 1) % items.Length;
					--size;
					return value;
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_EmptyQueue"));
				}
			}
	public T Peek()
			{
				if(size > 0)
				{
					return items[remove];
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_EmptyQueue"));
				}
			}
	public T[] ToArray()
			{
				T[] array = new T [size];
				if(size > 0)
				{
					if((remove + size) <= items.Length)
					{
						Array.Copy(items, remove, array, 0, size);
					}
					else
					{
						Array.Copy(items, remove, array, 0,
								   items.Length - remove);
						Array.Copy(items, 0, array,
								   items.Length - remove, add);
					}
				}
				return array;
			}

	// Private class for implementing queue enumeration.
	private class QueueIterator<T> : IIterator<T>
	{
		// Internal state.
		private ArrayQueue<T> queue;
		private int position;

		// Constructor.
		public QueueIterator(ArrayQueue<T> queue)
				{
					this.queue = queue;
					position   = -1;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					++position;
					if(position < queue.size)
					{
						return true;
					}
					position = queue.size;
					return false;
				}
		public void Reset()
				{
					position = -1;
				}
		public void Remove()
				{
					throw new InvalidOperationException(S._("NotSupp_Remove"));
				}
		public T Current
				{
					get
					{
						if(position < 0 || position >= queue.size)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return queue.items
							[(queue.remove + position) % queue.size];
					}
				}

	}; // class QueueIterator<T>

}; // class ArrayQueue<T>

}; // namespace Generics
