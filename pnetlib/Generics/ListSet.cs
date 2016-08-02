/*
 * ListSet.cs - Set implemented on top of a list.
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

public sealed class ListSet<T> : ISet<T>
{
	// Internal state.
	private IList<T> list;

	// Constructors.
	public ListSet()
			{
				list = new LinkedList<T>();
			}
	public ListSet(IList<T> list)
			{
				if(list == null)
				{
					throw new ArgumentNullException("list");
				}
				this.list = list;
			}

	// Implement the ISet<T> interface.
	public void Add(T value)
			{
				if(!list.Contains(value))
				{
					list.Add(value);
				}
			}
	public void Clear()
			{
				list.Clear();
			}
	public bool Contains(T value)
			{
				return list.Contains(value);
			}
	public void Remove(T value)
			{
				list.Remove(value);
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
					return false;
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
	public IIterator<T> GetIterator()
			{
				return list.GetIterator();
			}

}; // class ListSet<T>

}; // namespace Generics
