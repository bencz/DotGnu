/*
 * ArrayList.cs - Generic array list class.
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

public sealed class ArrayList<T> : IList<T>, ICapacity, ICloneable
{
	// Internal state.
	private int count;
	private T[] store;

	// Simple constructors.
	public ArrayList()
			{
				count = 0;
				store = new T [16];
			}
	public ArrayList(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", S._("ArgRange_NonNegative"));
				}
				count = 0;
				store = new T [capacity];
			}

	// Reallocate the array to accomodate "n" new entries at "index".
	// This leaves "count" unchanged.
	private void Realloc(int n, int index)
			{
				if((count + n) <= store.Length)
				{
					// The current capacity is sufficient, so just
					// shift the contents of the array upwards.
					int posn = count - 1;
					while(posn >= index)
					{
						store[posn + n] = store[posn];
						--posn;
					}
				}
				else
				{
					// We need to allocate a new array.
					int newCapacity = (((count + n) + 31) & ~31);
					int newCapacity2 = count * 2;
					if(newCapacity2 > newCapacity)
					{
						newCapacity = newCapacity2;
					}
					T[] newStore = new T [newCapacity];
					if(index != 0)
					{
						Array.Copy(store, 0, newStore, 0, index);
					}
					if(count != index)
					{
						Array.Copy(store, index, newStore, index + n,
								   count - index);
					}
					store = newStore;
				}
			}

	// Delete "n" entries from the list at "index".
	// This modifies "count".
	private void Delete(int n, int index)
			{
				while((index + n) < count)
				{
					store[index] = store[index + n];
					++index;
				}
				count -= n;
			}

	// Implement the IList<T> interface.
	public int Add(T value)
			{
				if(count >= store.Length)
				{
					Realloc(1, count);
				}
				store[count] = value;
				return count++;
			}
	public void Clear()
			{
				Array.Clear(store, 0, count);
				count = 0;
			}
	public bool Contains(T item)
			{
				int index;
				if(typeof(T).IsValueType)
				{
					for(index = 0; index < count; ++index)
					{
						if(item.Equals(store[index]))
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
						for(index = 0; index < count; ++index)
						{
							if(item.Equals(store[index]))
							{
								return true;
							}
						}
						return false;
					}
					else
					{
						for(index = 0; index < count; ++index)
						{
							if(((Object)(store[index])) == null)
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
				return new ArrayListIterator<T>(this);
			}
	public int IndexOf(T value)
			{
				int index;
				if(typeof(T).IsValueType)
				{
					for(index = 0; index < count; ++index)
					{
						if(item.Equals(store[index]))
						{
							return index;
						}
					}
					return -1;
				}
				else
				{
					if(((Object)item) != null)
					{
						for(index = 0; index < count; ++index)
						{
							if(item.Equals(store[index]))
							{
								return index;
							}
						}
						return -1;
					}
					else
					{
						for(index = 0; index < count; ++index)
						{
							if(((Object)(store[index])) == null)
							{
								return index;
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
				Realloc(1, index);
				store[index] = value;
				++count;
			}
	public void Remove(T value)
			{
				int index = Array.IndexOf(store, T, 0, count);
				if(index != -1)
				{
					Delete(1, index);
				}
			}
	public void RemoveAt(int index)
			{
				if(index < 0 || index > count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				Delete(1, index);
			}
	public bool IsRandomAccess
			{
				get
				{
					return true;
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
					return store[index];
				}
				set
				{
					if(index < 0 || index >= count)
					{
						throw new ArgumentOutOfRangeException
							("index", S._("ArgRange_Array"));
					}
					store[index] = value;
				}
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				ArrayList<T> clone = new ArrayList<T>(count);
				clone.count = count;
				Array.Copy(store, 0, clone.store, 0, count);
				return clone;
			}

	// Implement the ICollection<T> interface.
	public void CopyTo(T[] array, int arrayIndex)
			{
				Array.Copy(store, 0, array, arrayIndex, count);
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

	// Implement the IIterable<T> interface.
	IIterator<T> IIterable<T>.GetIterator()
			{
				return new ArrayListIterator<T>(this);
			}

	// Implement the ICapacity interface.
	public int Capacity
			{
				get
				{
					return store.Length;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					if(value < count)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("Arg_CannotReduceCapacity"));
					}
					if(value != store.Length)
					{
						T[] newStore = new T[value];
						int index;
						for(index = 0; index < count; ++index)
						{
							newStore[index] = store[index];
						}
						store = newStore;
					}
				}
			}

	// Array list iterator class.
	private class ArrayListIterator<T> : IListIterator<T>
	{
		// Internal state.
		private ArrayList<T> list;
		private int position;
		private int removed;
		private bool reset;

		// Constructor.
		public ArrayListIterator(ArrayList<T> list)
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
					return (position < list.Count);
				}
		public void Reset()
				{
					reset = true;
					position = -1;
					removed = -1;
				}
		public void Remove()
				{
					if(position < 0 || position >= list.Count || removed != -1)
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
						if(position < 0 || position >= list.Count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return list[position];
					}
				}

		// Implement the IListIterator<T> interface.
		public bool MovePrev()
				{
					if(reset)
					{
						// Start at the end of the range.
						position = list.Count - 1;
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
						if(position < 0 || position >= list.Count ||
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
						if(position < 0 || position >= list.Count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return list[position];
					}
					set
					{
						if(position < 0 || position >= list.Count ||
						   removed != -1)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						list[position] = value;
					}
				}

	}; // class ArrayListIterator<T>

}; // class ArrayList<T>

}; // namespace Generics
