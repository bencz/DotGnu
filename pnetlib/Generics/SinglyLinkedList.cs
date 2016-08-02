/*
 * SinglyLinkedList.cs - Generic singly-linked list class.
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

public sealed class SinglyLinkedList<T>
		: IQueue<T>, IStack<T>, IList<T>, ICloneable
{
	// Structure of a list node.
	private class Node<T>
	{
		public T       data;
		public Node<T> next;

		public Node(T data)
				{
					this.data = data;
					this.next = null;
				}

	}; // class Node

	// Internal state.
	private Node<T> first;
	private Node<T> last;
	private int     count;

	// Constructor.
	public SinglyLinkedList()
			{
				first = null;
				last = null;
				count = 0;
			}

	// Get a particular node in the list by index.
	private Node<T> Get(int index)
			{
				Node<T> current;
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				current = first;
				while(index > 0)
				{
					current = current.next;
					--index;
				}
				return current;
			}

	// Get the predecessor of a particular node in the list by index.
	private Node<T> GetPrev(int index)
			{
				Node<T> current;
				Node<T> prev;
				if(index < 0 || index >= count)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				current = first;
				prev = null;
				while(index > 0)
				{
					prev = current;
					current = current.next;
					--index;
				}
				return prev;
			}

	// Remove a node from this list.
	private void Remove(Node<T> prev, Node<T> node)
			{
				if(node.next == null)
				{
					last = prev;
				}
				if(prev != null)
				{
					prev.next = node.next;
				}
				else
				{
					first = node.next;
				}
				--count;
			}

	// Implement the IQueue<T> interface.
	void IQueue<T>.Clear()
			{
				Clear();
			}
	bool IQueue<T>.Contains(T value)
			{
				return Contains(value);
			}
	public void Enqueue(T value)
			{
				Add(value);
			}
	public T Dequeue()
			{
				if(first != null)
				{
					Node<T> node = first;
					if(node.next == null)
					{
						last = null;
					}
					first = node.next;
					--count;
					return node.data;
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_EmptyList"));
				}
			}
	public T Peek()
			{
				if(first != null)
				{
					return first.data;
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_EmptyList"));
				}
			}
	public T[] ToArray()
			{
				T[] array = new T [count];
				CopyTo(array, 0);
				return array;
			}

	// Implement the IStack<T> interface privately.
	void IStack<T>.Clear()
			{
				Clear();
			}
	bool IStack<T>.Contains(T value)
			{
				return Contains(value);
			}
	void IStack<T>.Push(T value)
			{
				Insert(0, value);
			}
	T IStack<T>.Pop()
			{
				return Dequeue();
			}
	T IStack<T>.Peek()
			{
				return Peek();
			}
	T[] IStack<T>.ToArray()
			{
				return ToArray();
			}

	// Implement the ICollection<T> interface.
	public void CopyTo(T[] array, int index)
			{
				IIterator<T> iterator = GetIterator();
				while(iterator.MoveNext())
				{
					array[index++] = iterator.Current;
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

	// Implement the IList<T> interface.
	public int Add(T value)
			{
				int index = count;
				Node<T> node = new Node<T>(value);
				if(last != null)
				{
					last.next = node;
				}
				else
				{
					first = node;
				}
				last = node;
				return index;
			}
	public void Clear()
			{
				first = null;
				last = null;
				count = 0;
			}
	public bool Contains(T value)
			{
				Node<T> current = first;
				if(typeof(T).IsValueType)
				{
					while(current != null)
					{
						if(value.Equals(current.data))
						{
							return true;
						}
						current = current.next;
					}
					return false;
				}
				else
				{
					if(((Object)value) != null)
					{
						while(current != null)
						{
							if(value.Equals(current.data))
							{
								return true;
							}
							current = current.next;
						}
						return false;
					}
					else
					{
						while(current != null)
						{
							if(((Object)(current.data)) == null)
							{
								return true;
							}
							current = current.next;
						}
						return false;
					}
				}
			}
	public IListIterator<T> GetIterator()
			{
				return new ListIterator<T>(this);
			}
	public int IndexOf(T value)
			{
				int index = 0;
				Node<T> current = first;
				if(typeof(T).IsValueType)
				{
					while(current != null)
					{
						if(value.Equals(current.data))
						{
							return index;
						}
						++index;
						current = current.next;
					}
					return -1;
				}
				else
				{
					if(((Object)value) != null)
					{
						while(current != null)
						{
							if(value.Equals(current.data))
							{
								return index;
							}
							++index;
							current = current.next;
						}
						return -1;
					}
					else
					{
						while(current != null)
						{
							if(((Object)(current.data)) == null)
							{
								return index;
							}
							++index;
							current = current.next;
						}
						return -1;
					}
				}
			}
	public void Insert(int index, T value)
			{
				if(index == count)
				{
					Add(value);
				}
				else
				{
					Node<T> prev = GetPrev(index);
					Node<T> node = new Node<T>(value);
					if(prev != null)
					{
						if(prev.next == null)
						{
							last = node;
						}
						node.next = prev.next;
						prev.next = node;
					}
					else if(first != null)
					{
						node.next = first;
						first = node;
					}
					else
					{
						first = node;
						last = node;
					}
				}
			}
	public void Remove(T value)
			{
				Node<T> current = first;
				Node<T> prev = null;
				if(typeof(T).IsValueType)
				{
					while(current != null)
					{
						if(value.Equals(current.data))
						{
							Remove(prev, current);
							return;
						}
						prev = current;
						current = current.next;
					}
				}
				else
				{
					if(((Object)value) != null)
					{
						while(current != null)
						{
							if(value.Equals(current.data))
							{
								Remove(prev, current);
								return;
							}
							prev = current;
							current = current.next;
						}
					}
					else
					{
						while(current != null)
						{
							if(((Object)(current.data)) == null)
							{
								Remove(prev, current);
								return;
							}
							prev = current;
							current = current.next;
						}
					}
				}
			}
	public void RemoveAt(int index)
			{
				Node<T> prev = GetPrev(index);
				if(prev != null)
				{
					Remove(prev, prev.next);
				}
				else
				{
					Remove(null, first);
				}
			}
	public bool IsRandomAccess
			{
				get
				{
					return false;
				}
			}
	public T this[int index]
			{
				get
				{
					return Get(index).data;
				}
				set
				{
					Get(index).data = value;
				}
			}

	// Implement the IIterable<T> interface.
	IIterator<T> IIterator<T>.GetIterator()
			{
				return GetIterator();
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				SinglyLinkedList<T> clone = new SinglyLinkedList<T>();
				IIterator<T> e = GetIterator();
				while(e.MoveNext())
				{
					clone.Add(e.Current);
				}
				return clone;
			}

	// Iterator class for lists.
	private class ListIterator<T> : IListIterator<T>
	{
		// Internal state, accessible to "LinkedList<T>".
		public LinkedList<T> list;
		public Node<T> posn;
		public Node<T> prev;
		public int     index;
		public bool    reset;
		public bool    removed;

		// Constructor.
		public ListIterator(LinkedList<T> list)
				{
					this.list = list;
					this.posn = null;
					this.prev = null;
					this.index = -1;
					this.reset = true;
					this.removed = false;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					if(reset)
					{
						posn = list.first;
						prev = null;
						index = 0;
						reset = false;
					}
					else if(removed)
					{
						if(prev != null)
						{
							posn = prev.next;
						}
						else
						{
							posn = list.first;
						}
					}
					else if(posn != null)
					{
						prev = posn;
						posn = posn.next;
						++index;
					}
					removed = false;
					return (posn != null);
				}
		public void Reset()
				{
					posn = null;
					prev = null;
					index = -1;
					reset = true;
					removed = false;
				}
		public void Remove()
				{
					if(posn == null || removed)
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
					list.Remove(prev, posn);
					removed = true;
				}
		T IIterator<T>.Current
				{
					get
					{
						if(posn == null || removed)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return posn.data;
					}
				}

		// Implement the IListIterator<T> interface.
		public bool MovePrev()
				{
					if(reset)
					{
						posn = list.last;
						index = list.count - 1;
						if(index >= 0)
						{
							prev = list.GetPrev(index);
						}
						else
						{
							prev = null;
						}
						reset = false;
					}
					else if(posn != null)
					{
						posn = prev;
						--index;
						if(index >= 0)
						{
							prev = list.GetPrev(index);
						}
						else
						{
							prev = null;
						}
					}
					removed = false;
					return (posn != null);
				}
		public int Position
				{
					get
					{
						if(posn == null || removed)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return index;
					}
				}
		public T Current
				{
					get
					{
						if(posn == null || removed)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						return posn.data;
					}
					set
					{
						if(posn == null || removed)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
						posn.data = value;
					}
				}

	}; // class ListIterator<T>

}; // class SinglyLinkedList<T>

}; // namespace Generics
