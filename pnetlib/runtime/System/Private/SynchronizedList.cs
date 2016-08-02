/*
 * SynchronizedList.cs - Implementation of the
 *			"System.Private.SynchronizedList" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Private
{

#if !ECMA_COMPAT

using System;
using System.Collections;

// This is a helper class for wrapping up lists to make them
// synchronized.  We synchronize all operations because it
// is safer in multi-processor environments.

internal class SynchronizedList : IList
{

	// Internal state.
	private IList  list;

	// Constructor.
	public SynchronizedList(IList list)
			{
				this.list = list;
			}

	// Implement the IList interface.
	public int Add(Object value)
			{
				lock(SyncRoot)
				{
					return list.Add(value);
				}
			}
	public void Clear()
			{
				lock(SyncRoot)
				{
					list.Clear();
				}
			}
	public bool Contains(Object value)
			{
				lock(SyncRoot)
				{
					return list.Contains(value);
				}
			}
	public int IndexOf(Object value)
			{
				lock(SyncRoot)
				{
					return list.IndexOf(value);
				}
			}
	public void Insert(int index, Object value)
			{
				lock(SyncRoot)
				{
					list.Insert(index, value);
				}
			}
	public void Remove(Object value)
			{
				lock(SyncRoot)
				{
					list.Remove(value);
				}
			}
	public void RemoveAt(int index)
			{
				lock(SyncRoot)
				{
					list.RemoveAt(index);
				}
			}
	public bool IsFixedSize
			{
				get
				{
					lock(SyncRoot)
					{
						return list.IsFixedSize;
					}
				}
			}
	public bool IsReadOnly
			{
				get
				{
					lock(SyncRoot)
					{
						return list.IsReadOnly;
					}
				}
			}
	public Object this[int index]
			{
				get
				{
					lock(SyncRoot)
					{
						return list[index];
					}
				}
				set
				{
					lock(SyncRoot)
					{
						list[index] = value;
					}
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				lock(SyncRoot)
				{
					list.CopyTo(array, index);
				}
			}
	public int Count
			{
				get
				{
					lock(SyncRoot)
					{
						return list.Count;
					}
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return true;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return list.SyncRoot;
				}
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				lock(SyncRoot)
				{
					return new SynchronizedEnumerator
						(SyncRoot, list.GetEnumerator());
				}
			}

}; // class SynchronizedList

#endif // !ECMA_COMPAT

}; // namespace System.Private
