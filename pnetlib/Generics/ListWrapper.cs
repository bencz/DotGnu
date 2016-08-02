/*
 * ListWrapper.cs - Wrap a non-generic list to turn it into a generic one.
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

public sealed class ListWrapper<T> : IList<T>, ICollection<T>
{

	// Internal state.
	private System.Collections.IList list;

	// Constructor.
	public ListWrapper(System.Collections.IList list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				this.list = list;
			}

	// Implement the IList<T> interface.
	public int Add(T value)
			{
				return list.Add(value);
			}
	public void Clear()
			{
				list.Clear();
			}
	public bool Contains(T value)
			{
				return list.Contains(value);
			}
	public IListIterator<T> GetIterator()
			{
				return new ListIterator<T>(list);
			}
	public int IndexOf(T value)
			{
				return list.IndexOf(value);
			}
	public void Insert(int index, T value)
			{
				list.Insert(index, value);
			}
	public void Remove(T value)
			{
				list.Remove(value);
			}
	public void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}
	public bool IsRandomAccess
			{
				get
				{
					// Recognise ArrayList specially as random-access.
					return (list is System.Collections.ArrayList);
				}
			}
	public T this[int index]
			{
				get
				{
					return (T)(list[index]);
				}
				set
				{
					list[index] = value;
				}
			}

	// Implement the ICollection<T> interface.
	public void CopyTo(T[] array, int index)
			{
				list.CopyTo(array, index);
			}
	public int Count
			{
				get
				{
					return list.Count;
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
				return new ListIterator<T>(list);
			}

	// Private list iterator implementation.
	private sealed class ListIterator<T> : IListIterator<T>
	{
		// Internal state.
		private IList list;
		private int posn;
		private int removed;
		private bool reset;

		// Constructor.
		public ListIterator(IList list)
				{
					this.list = list;
					posn = -1;
					removed = -1;
					reset = true;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					if(reset)
					{
						// Start at the beginning of the list.
						posn = 0;
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
						++posn;
					}
					return (posn < list.Count);
				}
		public void Reset()
				{
					posn = -1;
					removed = -1;
					reset = true;
				}
		public void Remove()
				{
					if(posn >= 0 && posn < list.Count)
					{
						list.RemoveAt(posn);
						removed = posn;
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
				}
		T IIterator<T>.Current
				{
					get
					{
						if(posn >= 0 && posn < list.Count && removed == -1)
						{
							return (T)(list[posn]);
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
					}
				}

		// Implement the IListIterator<T> interface.
		public bool MovePrev()
				{
					if(reset)
					{
						// Start at the end of the list.
						posn = list.Count - 1;
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
						--posn;
					}
					return (posn >= 0);
				}
		public int Position
				{
					get
					{
						if(posn >= 0 && posn < list.Count && removed == -1)
						{
							return posn;
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
					}
				}
		public T Current
				{
					get
					{
						if(posn >= 0 && posn < list.Count && removed == -1)
						{
							return (T)(list[posn]);
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
					}
					set
					{
						if(posn >= 0 && posn < list.Count && removed == -1)
						{
							list[posn] = value;
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
					}
				}

	}; // class ListIterator<T>

}; // class ListWrapper<T>

}; // namespace Generics
