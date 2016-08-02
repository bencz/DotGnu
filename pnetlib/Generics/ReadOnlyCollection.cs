/*
 * ReadOnlyCollection.cs - Wrap a collection to make it read-only.
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

public class ReadOnlyCollection<T> : ICollection<T>, ICloneable
{
	// Internal state.
	protected ICollection<T> coll;

	// Constructor.
	public ReadOnlyCollection(ICollection<T> coll)
			{
				if(coll == null)
				{
					throw new ArgumentNullException("coll");
				}
				this.coll = coll;
			}

	// Implement the ICollection<T> interface.
	public void CopyTo(T[] array, int index)
			{
				coll.CopyTo(array, index);
			}
	public int Count
			{
				get
				{
					return coll.Count;
				}
			}
	public bool IsFixedSize
			{
				get
				{
					return coll.IsFixedSize;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return coll.IsSynchronized;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return coll.SyncRoot;
				}
			}

	// Implement the IIterable<T> interface.
	public IIterator<T> GetIterator()
			{
				return new ReadOnlyIterator<T>(coll.GetIterator());
			}

	// Implement the ICloneable interface.
	public virtual Object Clone()
			{
				if(coll is ICloneable)
				{
					return new ReadOnlyCollection<T>
						((ICollection<T>)(((ICloneable)coll).Clone()));
				}
				else
				{
					throw new InvalidOperationException
						(S._("Invalid_NotCloneable"));
				}
			}

	// Iterator class for read-only collections.
	private sealed class ReadOnlyIterator<T> : IIterator<T>
	{
		// Internal state.
		protected IIterator<T> iterator;

		// Constructor.
		public ReadOnlyIterator(IIterator<T> iterator)
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
						(S._("NotSupp_ReadOnly"));
				}
		public T Current
				{
					get
					{
						return iterator.Current;
					}
				}

	}; // class ReadOnlyIterator<T>

}; // class ReadOnlyCollection<T>

}; // namespace Generics
