/*
 * Queue.cs - Implementation of the "System.Collections.Queue" class.
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

#if !ECMA_COMPAT
public
#else
internal
#endif
class Queue : ICollection, IEnumerable, ICloneable
{
	// Internal state.
	private Object[] items;
	private int		 add, remove, size;
	private float    growFactor;
	private int		 generation;

	// The default capacity for queues.
	private const int DefaultCapacity = 32;

	// Constructors.
	public Queue()
			{
				items = new Object [DefaultCapacity];
				add = 0;
				remove = 0;
				size = 0;
				growFactor = 2.0f;
				generation = 0;
			}
	public Queue(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				items = new Object [capacity];
				add = 0;
				remove = 0;
				size = 0;
				growFactor = 2.0f;
				generation = 0;
			}
	public Queue(int capacity, float growFactor)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_NonNegative"));
				}
				if(growFactor < 1.0f || growFactor > 10.0f)
				{
					throw new ArgumentOutOfRangeException
						("growFactor", _("ArgRange_QueueGrowFactor"));
				}
				items = new Object [capacity];
				add = 0;
				remove = 0;
				size = 0;
				this.growFactor = growFactor;
				generation = 0;
			}
	public Queue(ICollection col)
			{
				if(col == null)
				{
					throw new ArgumentNullException("col");
				}
				items = new Object [col.Count];
				col.CopyTo(items, 0);
				add = 0;
				remove = 0;
				size = items.Length;
				growFactor = 2.0f;
				generation = 0;
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
				else if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				else if((array.GetLength(0) - index) < size)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
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
	public virtual int Count
			{
				get
				{
					return size;
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

	// Implement the ICloneable interface.
	public virtual Object Clone()
			{
				Queue queue = (Queue)MemberwiseClone();
				queue.items = (Object[])items.Clone();
				return queue;
			}

	// Implement the IEnumerable interface.
	public virtual IEnumerator GetEnumerator()
			{
				return new QueueEnumerator(this);
			}

	// Clear the contents of this queue.
	public virtual void Clear()
			{
				Array.Clear(items, 0, items.Length); // clear references of objects
				add = 0;
				remove = 0;
				size = 0;
				++generation;
			}

	// Determine if this queue contains a specific object.
	public virtual bool Contains(Object obj)
			{
				int index = remove;
				int capacity = items.Length;
				int count = size;
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
				return false;
			}

	// Dequeue an item.
	public virtual Object Dequeue()
			{
				if(size > 0)
				{
					Object value = items[remove];
					items[remove] = null;	// set to null to release the handle
					remove = (remove + 1) % items.Length;
					--size;
					++generation;
					return value;
				}
				else
				{
					throw new InvalidOperationException
						(_("Invalid_EmptyQueue"));
				}
			}

	// Enqueue an item.
	public virtual void Enqueue(Object obj)
			{
				if(size < items.Length)
				{
					// The queue is big enough to hold the new item.
					items[add] = obj;
					add = (add + 1) % items.Length;
					++size;
				}
				else
				{
					// We need to increase the size of the queue.
					int newCapacity = (int)(items.Length * growFactor);
					if(newCapacity <= items.Length)
					{
						newCapacity = items.Length + 1;
					}
					Object[] newItems = new Object [newCapacity];
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
					items[add] = obj;
					add = (add + 1) % items.Length;
					++size;
				}
				++generation;
			}

	// Peek at the first item without dequeuing it.
	public virtual Object Peek()
			{
				if(size > 0)
				{
					return items[remove];
				}
				else
				{
					throw new InvalidOperationException
						(_("Invalid_EmptyQueue"));
				}
			}

	// Convert the contents of this queue into an array.
	public virtual Object[] ToArray()
			{
				Object[] array = new Object [size];
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

	// Trim this queue to its actual size.
	public virtual void TrimToSize()
			{
				items = ToArray();
				add = items.Length;
				remove = 0;
				size = items.Length;
			}

	// Convert this queue into a synchronized queue.
	public static Queue Synchronized(Queue queue)
			{
				if(queue == null)
				{
					throw new ArgumentNullException("queue");
				}
				else if(queue.IsSynchronized)
				{
					return queue;
				}
				else
				{
					return new SynchronizedQueue(queue);
				}
			}

	// Private class that implements synchronized queues.
	private class SynchronizedQueue : Queue
	{
		// Internal state.
		private Queue queue;

		// Constructor.
		public SynchronizedQueue(Queue queue)
				{
					this.queue = queue;
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int index)
				{
					lock(SyncRoot)
					{
						queue.CopyTo(array, index);
					}
				}
		public override int Count
				{
					get
					{
						lock(SyncRoot)
						{
							return queue.Count;
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
						return queue.SyncRoot;
					}
				}

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new SynchronizedQueue((Queue)(queue.Clone()));
				}

		// Implement the IEnumerable interface.
		public override IEnumerator GetEnumerator()
				{
					lock(SyncRoot)
					{
						return new SynchronizedEnumerator
							(SyncRoot, queue.GetEnumerator());
					}
				}

		// Clear the contents of this queue.
		public override void Clear()
				{
					lock(SyncRoot)
					{
						queue.Clear();
					}
				}

		// Determine if this queue contains a specific object.
		public override bool Contains(Object obj)
				{
					lock(SyncRoot)
					{
						return queue.Contains(obj);
					}
				}

		// Dequeue an item.
		public override Object Dequeue()
				{
					lock(SyncRoot)
					{
						return queue.Dequeue();
					}
				}

		// Enqueue an item.
		public override void Enqueue(Object obj)
				{
					lock(SyncRoot)
					{
						queue.Enqueue(obj);
					}
				}

		// Peek at the first item without dequeuing it.
		public override Object Peek()
				{
					lock(SyncRoot)
					{
						return queue.Peek();
					}
				}

		// Convert the contents of this queue into an array.
		public override Object[] ToArray()
				{
					lock(SyncRoot)
					{
						return queue.ToArray();
					}
				}

		// Trim this queue to its actual size.
		public override void TrimToSize()
				{
					lock(SyncRoot)
					{
						queue.TrimToSize();
					}
				}

	}; // class SynchronizedQueue

	// Private class for implementing queue enumeration.
	private class QueueEnumerator : IEnumerator
	{
		// Internal state.
		private Queue queue;
		private int   generation;
		private int   position;

		// Constructor.
		public QueueEnumerator(Queue queue)
				{
					this.queue = queue;
					generation = queue.generation;
					position   = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(generation != queue.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
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
					if(generation != queue.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					position = -1;
				}
		public Object Current
				{
					get
					{
						if(generation != queue.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= queue.size)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return queue.items
							[(queue.remove + position) % queue.items.Length];
					}
				}

	}; // class QueueEnumerator

}; // class Queue

}; // namespace System.Collections
