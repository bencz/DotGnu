/*
 * FixedSizeList.cs - Wrap a list to make it fixed-size.
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

public class FixedSizeList<T> : FixedSizeCollection<T>, IList<T>, IRandomAccess
{
	// Internal state.
	protected IList<T> list;

	// Constructor.
	public FixedSizeList(IList<T> list) : base(list)
			{
				this.list = list;
			}

	// Implement the IList<T> interface.
	public int Add(T value)
			{
				throw new InvalidOperationException
					(S._("NotSupp_FixedSizeCollection"));
			}
	public void Clear()
			{
				throw new InvalidOperationException
					(S._("NotSupp_FixedSizeCollection"));
			}
	public bool Contains(T value)
			{
				return list.Contains(value);
			}
	public new IListIterator<T> GetIterator()
			{
				return new FixedSizeListIterator<T>(list.GetIterator());
			}
	public int IndexOf(T value)
			{
				return list.IndexOf(value);
			}
	public void Insert(int index, T value)
			{
				throw new InvalidOperationException
					(S._("NotSupp_FixedSizeCollection"));
			}
	public void Remove(T value)
			{
				throw new InvalidOperationException
					(S._("NotSupp_FixedSizeCollection"));
			}
	public void RemoveAt(int index)
			{
				throw new InvalidOperationException
					(S._("NotSupp_FixedSizeCollection"));
			}
	public bool IsRandomAccess
			{
				get
				{
					return list.IsRandomAccess;
				}
			}
	public T this[int index]
			{
				get
				{
					return list[index];
				}
				set
				{
					list[index] = value;
				}
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				if(list is ICloneable)
				{
					return new FixedSizeList<T>
						((IList<T>)(((ICloneable)list).Clone()));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

	// Fixed-size list iterator.
	private sealed class FixedSizeListIterator<T> : IListIterator<T>
	{
		// Internal state.
		protected IListIterator<T> iterator;

		// Constructor.
		public FixedSizeListIterator(IListIterator<T> iterator)
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
					throw new InvalidOperationException
						(S._("NotSupp_FixedSizeCollection"));
				}
		T IIterator<T>.Current
				{
					get
					{
						return ((IIterator<T>)iterator).Current;
					}
				}

		// Implement the IListIterator<T> interface.
		public bool MovePrev()
				{
					return iterator.MovePrev();
				}
		public int Position
				{
					get
					{
						return iterator.Position;
					}
				}
		public T Current
				{
					get
					{
						return iterator.Current;
					}
					set
					{
						iterator.Current = value;
					}
				}

	}; // class FixedSizeListIterator<T>

}; // class FixedSizeList<T>

}; // namespace Generics
