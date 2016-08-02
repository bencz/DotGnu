/*
 * SynchronizedCollection.cs - Wrap a collection to make it synchronized.
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

public class SynchronizedCollection<T> : ICollection<T>, ICloneable
{
	// Internal state.
	protected ICollection<T> coll;

	// Constructors.
	public SynchronizedCollection(ICollection<T> coll)
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
				lock(SyncRoot)
				{
					coll.CopyTo(array, index);
				}
			}
	public int Count
			{
				get
				{
					lock(SyncRoot)
					{
						return coll.Count;
					}
				}
			}
	public bool IsFixedSize
			{
				get
				{
					lock(SyncRoot)
					{
						return coll.IsFixedSize;
					}
				}
			}
	public bool IsReadOnly
			{
				get
				{
					lock(SyncRoot)
					{
						return coll.IsReadOnly;
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
					return coll.SyncRoot;
				}
			}

	// Implement the IIterable<T> interface.
	public IIterator<T> GetIterator()
			{
				lock(SyncRoot)
				{
					return new SynchronizedIterator<T>
						(coll, coll.GetIterator());
				}
			}

	// Implement the ICloneable interface.
	public virtual Object Clone()
			{
				lock(SyncRoot)
				{
					if(coll is ICloneable)
					{
						return new SynchronizedCollection<T>
							((ICollection<T>)(((ICloneable)coll).Clone()));
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_NotCloneable"));
					}
				}
			}

	// Synchronized collection iterator.
	private sealed class SynchronizedIterator<T> : IIterator<T>
	{
		// Internal state.
		protected ICollection<T> coll;
		protected IIterator<T>   iterator;

		// Constructor.
		public SynchronizedIterator(ICollection<T> coll, IIterator<T> iterator)
			{
				this.coll = coll;
				this.iterator = iterator;
			}

		// Implement the IIterator<T> interface.
		public bool MoveNext()
				{
					lock(coll.SyncRoot)
					{
						return iterator.MoveNext();
					}
				}
		public void Reset()
				{
					lock(coll.SyncRoot)
					{
						iterator.Reset();
					}
				}
		public void Remove()
				{
					lock(coll.SyncRoot)
					{
						iterator.Remove();
					}
				}
		public T Current
				{
					get
					{
						lock(coll.SyncRoot)
						{
							return iterator.Current;
						}
					}
				}

	}; // class SynchronizedIterator<T>

}; // class SynchronizedCollection<T>

}; // namespace Generics
