/*
 * ArrayStack.cs - Generic stack class, implemented as an array.
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

public sealed class ArrayStack<T> : IStack<T>, ICapacity, ICloneable
{
	// Internal state.
	private T[] items;
	private int size;

	// The default capacity for stacks.
	private const int DefaultCapacity = 10;

	// Constructors.
	public ArrayStack()
			{
				items = new T [DefaultCapacity];
				size = 0;
			}
	public ArrayStack(int initialCapacity)
			{
				if(initialCapacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("initialCapacity", S._("ArgRange_NonNegative"));
				}
				items = new T [initialCapacity];
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
					Array.Copy(items, 0, array, index, size);
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

	// Implement the ICloneable interface.
	public Object Clone()
			{
				ArrayStack<T> stack = (ArrayStack<T>)MemberwiseClone();
				stack.items = (T[])items.Clone();
				return stack;
			}

	// Implement the IIterable<T> interface.
	public IIterator<T> GetIterator()
			{
				return new StackIterator<T>(this);
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
						Array.Copy(items, 0, newItems, 0, size);
						items = newItems;
					}
				}
			}

	// Implement the IStack<T> interface.
	public void Clear()
			{
				size = 0;
			}
	public bool Contains(T obj)
			{
				int index;
				if(typeof(T).IsValueType)
				{
					for(index = 0; index < size; ++index)
					{
						if(obj.Equals(items[index]))
						{
							return true;
						}
					}
				}
				else
				{
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
				}
				return false;
			}
	public void Push(T value)
			{
				if(size < items.Length)
				{
					// The stack is big enough to hold the new item.
					items[size++] = value;
				}
				else
				{
					// We need to increase the size of the stack.
					int newCapacity = items.Length * 2;
					if(newCapacity <= items.Length)
					{
						newCapacity = items.Length + 1;
					}
					T[] newItems = new T [newCapacity];
					if(size > 0)
					{
						Array.Copy(items, newItems, size);
					}
					items = newItems;
					items[size++] = value;
				}
			}
	public T Pop()
			{
				if(size > 0)
				{
					return items[--size];
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_EmptyStack"));
				}
			}
	public T Peek()
			{
				if(size > 0)
				{
					return items[size - 1];
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_EmptyStack"));
				}
			}
	public T[] ToArray()
			{
				T[] array = new T [size];
				int index;
				for(index = 0; index < size; ++index)
				{
					array[index] = items[size - index - 1];
				}
				return array;
			}

	// Private class for implementing stack iteration.
	private class StackIterator<T> : IIterator<T>
	{
		// Internal state.
		private ArrayStack<T> stack;
		private int position;

		// Constructor.
		public StackIterator(ArrayStack<T> stack)
				{
					this.stack = stack;
					position   = -1;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
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
						if(position < 0 || position >= stack.size)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return stack.items[stack.size - position - 1];
					}
				}

	}; // class StackIterator<T>

}; // class ArrayStack<T>

}; // namespace Generics
