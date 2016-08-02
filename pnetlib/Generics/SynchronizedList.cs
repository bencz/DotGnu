/*
 * SynchronizedList.cs - Wrap a list to make it synchronized.
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

public class SynchronizedList<T> : SynchronizedCollection<T>, IList<T>
{
	// Internal state.
	protected IList<T> list;

	// Constructor.
	public SynchronizedList(IList<T> list) : base(list)
			{
				this.list = list;
			}

	// Implement the IList<T> interface.
	public int Add(T value)
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
	public bool Contains(T value)
			{
				lock(SyncRoot)
				{
					return list.Contains(value);
				}
			}
	public new IListIterator<T> GetIterator()
			{
				lock(SyncRoot)
				{
					return new SynchronizedListIterator<T>
						(list, list.GetIterator());
				}
			}
	public int IndexOf(T value)
			{
				lock(SyncRoot)
				{
					return list.IndexOf(value);
				}
			}
	public void Insert(int index, T value)
			{
				lock(SyncRoot)
				{
					list.Insert(index, value);
				}
			}
	public void Remove(T value)
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
	public bool IsRandomAccess
			{
				get
				{
					lock(SyncRoot)
					{
						return list.IsRandomAccess;
					}
				}
			}
	public T this[int index]
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

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				lock(SyncRoot)
				{
					if(list is ICloneable)
					{
						return new SynchronizedList<T>
							((IList<T>)(((ICloneable)list).Clone()));
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_NotCloneable"));
					}
				}
			}

	// Synchronized list iterator.
	private sealed class SynchronizedListIterator<T> : IListIterator<T>
	{
		// Internal state.
		protected IList<T> list;
		protected IListIterator<T> iterator;

		// Constructor.
		public SynchronizedListIterator
					(IList<T> list, IListIterator<T> iterator)
				{
					this.list = list;
					this.iterator = iterator;
				}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					lock(list.SyncRoot)
					{
						return iterator.MoveNext();
					}
				}
		public void Reset()
				{
					lock(list.SyncRoot)
					{
						iterator.Reset();
					}
				}
		public void Remove()
				{
					lock(list.SyncRoot)
					{
						iterator.Remove();
					}
				}
		T IIterator<T>.Current
				{
					get
					{
						lock(list.SyncRoot)
						{
							return ((IIterator<T>)iterator).Current;
						}
					}
				}

		// Implement the IListIterator<T> interface.
		public bool MovePrev()
				{
					lock(list.SyncRoot)
					{
						return iterator.MovePrev();
					}
				}
		public int Position
				{
					get
					{
						lock(list.SyncRoot)
						{
							return iterator.Position;
						}
					}
				}
		public T Current
				{
					get
					{
						lock(list.SyncRoot)
						{
							return iterator.Current;
						}
					}
					set
					{
						lock(list.SyncRoot)
						{
							iterator.Current = value;
						}
					}
				}

	}; // class SynchronizedListIterator<T>

}; // class SynchronizedList<T>

}; // namespace Generics
