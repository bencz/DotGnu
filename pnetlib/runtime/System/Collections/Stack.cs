/*
 * Stack.cs - Implementation of the "System.Collections.Stack" class.
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

#if !ECMA_COMPAT

using System;
using System.Private;

public class Stack : ICollection, IEnumerable, ICloneable
{
	// Internal state.
	private Object[] items;
	private int		 size;
	private int		 generation;

	// The default capacity for stacks.
	private const int DefaultCapacity = 10;

	// The amount to grow the stack by each time.
	private const int GrowSize = 10;

	// Constructors.
	public Stack()
			{
				items = new Object [DefaultCapacity];
				size = 0;
				generation = 0;
			}
	public Stack(int initialCapacity)
			{
				if(initialCapacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("initialCapacity", _("ArgRange_NonNegative"));
				}
				items = new Object [initialCapacity];
				size = 0;
				generation = 0;
			}
	public Stack(ICollection col)
			{
				if(col == null)
				{
					throw new ArgumentNullException("col");
				}
				items = new Object [col.Count];
				col.CopyTo(items, 0);
				size = items.Length;
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
					Array.Copy(ToArray(), 0, array, index, size);
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
				Stack stack = (Stack)MemberwiseClone();
				stack.items = (Object[])items.Clone();
				return stack;
			}

	// Implement the IEnumerable interface.
	public virtual IEnumerator GetEnumerator()
			{
				return new StackEnumerator(this);
			}

	// Clear the contents of this stack.
	public virtual void Clear()
			{
				// brubbel
				// set all references to zero, to avoid memory leaks !!!
				int iCount = items.Length;
				for( int i = 0; i < iCount; i++ ) {
					items[i] = null;
				}
				
				size = 0;
				++generation;
			}

	// Determine if this stack contains a specific object.
	public virtual bool Contains(Object obj)
			{
				int index;
				for(index = 0; index < size; ++index)
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
				}
				return false;
			}

	// Pop an item.
	public virtual Object Pop()
			{
				if(size > 0)
				{
					++generation;
					--size;
					Object o = items[size];
					items[size] = null; // remove reference of object, to avoid memory leaks !!!
					return o;
				}
				else
				{
					throw new InvalidOperationException
						(_("Invalid_EmptyStack"));
				}
			}

	// Push an item.
	public virtual void Push(Object obj)
			{
				if(size < items.Length)
				{
					// The stack is big enough to hold the new item.
					items[size++] = obj;
				}
				else
				{
					// We need to increase the size of the stack.
					int newCapacity = items.Length + GrowSize;
					Object[] newItems = new Object [newCapacity];
					if(size > 0)
					{
						Array.Copy(items, newItems, size);
					}
					items = newItems;
					items[size++] = obj;
				}
				++generation;
			}

	// Peek at the top-most item without popping it.
	public virtual Object Peek()
			{
				if(size > 0)
				{
					return items[size - 1];
				}
				else
				{
					throw new InvalidOperationException
						(_("Invalid_EmptyStack"));
				}
			}

	// Convert the contents of this stack into an array.
	public virtual Object[] ToArray()
			{
				Object[] array = new Object [size];
				int index;
				for(index = 0; index < size; ++index)
				{
					array[index] = items[size - index - 1];
				}
				return array;
			}

	// Convert this stack into a synchronized stack.
	public static Stack Synchronized(Stack stack)
			{
				if(stack == null)
				{
					throw new ArgumentNullException("stack");
				}
				else if(stack.IsSynchronized)
				{
					return stack;
				}
				else
				{
					return new SynchronizedStack(stack);
				}
			}

	// Private class that implements synchronized stacks.
	private class SynchronizedStack : Stack
	{
		// Internal state.
		private Stack stack;

		// Constructor.
		public SynchronizedStack(Stack stack)
				{
					this.stack = stack;
				}

		// Implement the ICollection interface.
		public override void CopyTo(Array array, int index)
				{
					lock(SyncRoot)
					{
						stack.CopyTo(array, index);
					}
				}
		public override int Count
				{
					get
					{
						lock(SyncRoot)
						{
							return stack.Count;
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
						return stack.SyncRoot;
					}
				}

		// Implement the ICloneable interface.
		public override Object Clone()
				{
					return new SynchronizedStack((Stack)(stack.Clone()));
				}

		// Implement the IEnumerable interface.
		public override IEnumerator GetEnumerator()
				{
					lock(SyncRoot)
					{
						return new SynchronizedEnumerator
							(SyncRoot, stack.GetEnumerator());
					}
				}

		// Clear the contents of this stack.
		public override void Clear()
				{
					lock(SyncRoot)
					{
						stack.Clear();
					}
				}

		// Determine if this stack contains a specific object.
		public override bool Contains(Object obj)
				{
					lock(SyncRoot)
					{
						return stack.Contains(obj);
					}
				}

		// Pop an item.
		public override Object Pop()
				{
					lock(SyncRoot)
					{
						return stack.Pop();
					}
				}

		// Push an item.
		public override void Push(Object obj)
				{
					lock(SyncRoot)
					{
						stack.Push(obj);
					}
				}

		// Peek at the top-most item without popping it.
		public override Object Peek()
				{
					lock(SyncRoot)
					{
						return stack.Peek();
					}
				}

		// Convert the contents of this stack into an array.
		public override Object[] ToArray()
				{
					lock(SyncRoot)
					{
						return stack.ToArray();
					}
				}

	}; // class SynchronizedStack

	// Private class for implementing stack enumeration.
	private class StackEnumerator : IEnumerator
	{
		// Internal state.
		private Stack stack;
		private int   generation;
		private int   position;

		// Constructor.
		public StackEnumerator(Stack stack)
				{
					this.stack = stack;
					generation = stack.generation;
					position   = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(generation != stack.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					++position;
					if(position < stack.size)
					{
						return true;
					}
					position = stack.size;
					return false;
				}
		public void Reset()
				{
					if(generation != stack.generation)
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
						if(generation != stack.generation)
						{
							throw new InvalidOperationException
								(_("Invalid_CollectionModified"));
						}
						if(position < 0 || position >= stack.size)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return stack.items[stack.size - position - 1];
					}
				}

	}; // class StackEnumerator

}; // class Stack

#endif // !ECMA_COMPAT

}; // namespace System.Collections
