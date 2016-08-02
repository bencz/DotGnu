/*
 * TreeSet.cs - Generic tree class, implementing a set.
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

public sealed class TreeSet<T> : TreeBase<T, bool>, ISet<T>, ICloneable
{
	// Constructors.
	public TreeSet() : base(null) {}
	public TreeSet(IComparer<T> cmp) : base(cmp) {}

	// Implement the ISet<T> interface.
	public void Add(T value)
			{
				AddItem(value, true, true);
			}
	public void Clear()
			{
				ClearAllItems();
			}
	public bool Contains(T value)
			{
				return tree.ContainsItem(value);
			}
	public void Remove(T value)
			{
				tree.RemoveItem(value);
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

	// Implement the IEnumerable<T> interface.
	public IIterator<T> GetIterator()
			{
				return new TreeSetIterator<T>(GetInOrderIterator());
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				TreeSet<T> tree = new TreeSet<T>(cmp);
				TreeBaseIterator<T, bool> iterator = GetInOrderIterator();
				while(iterator.MoveNext())
				{
					tree.AddItem(iterator.Key, iterator.Value);
				}
				return tree;
			}

	// Iterator class that implements in-order traversal of a tree set.
	private sealed class TreeSetIterator<T> : IIterator<T>
	{
		// Internal state.
		private TreeBase.TreeBaseIterator<T, bool> iterator;

		// Constructor.
		public TreeSetIterator(TreeBase.TreeBaseIterator<KeyT, bool> iterator)
				{
					this.iterator = iterator;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					return iterator.MoveNext();
				}
		public void Reset()
				{
					iterator.Reset();
				}
		public void Remove()
				{
					iterator.Remove();
				}
		public T Current
				{
					get
					{
						return iterator.Key;
					}
				}

	}; // class TreeSetIterator<T>

}; // class TreeSet<T>

}; // namespace Generics
