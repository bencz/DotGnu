/*
 * CollectionWrapper.cs - Wrap a non-generic collection and make it generic.
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

public sealed class CollectionWrapper<T> : ICollection<T>
{
	// Internal state.
	private System.Collections.ICollection coll;

	// Constructor.
	public CollectionWrapper(System.Collections.ICollection coll)
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
					if(coll is System.Collections.IDictionary)
					{
						return ((System.Collections.IDictionary)coll)
							.IsFixedSize;
					}
					else if(coll is IList)
					{
						return ((System.Collections.IList)coll).IsFixedSize;
					}
					return false;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					if(coll is System.Collections.IDictionary)
					{
						return ((System.Collections.IDictionary)coll)
							.IsReadOnly;
					}
					else if(coll is IList)
					{
						return ((System.Collections.IList)coll).IsReadOnly;
					}
					return false;
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
				return new EnumeratorWrapper(coll.GetEnumerator());
			}

}; // class CollectionWrapper<T>

}; // namespace Generics
