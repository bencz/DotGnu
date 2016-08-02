/*
 * CollectionAdapter.cs - Adapt a generic collection into a non-generic one.
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

public sealed class CollectionAdapter<T>
	: System.Collections.ICollection, System.Collections.IEnumerable
{

	// Internal state.
	private ICollection<T> coll;

	// Constructor.
	public CollectionAdapter(ICollection<T> coll)
			{
				if(coll == null)
				{
					throw new ArgumentNullException("coll");
				}
				this.coll = coll;
			}

	// Implement the non-generic ICollection interface.
	public void CopyTo(Array array, int index)
			{
				IIterator<T> iterator = coll.GetIterator();
				while(iterator.MoveNext())
				{
					array.SetValue(iterator.Current, index++);
				}
			}
	public int Count
			{
				get
				{
					return coll.Count;
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

	// Implement the non-generic IEnumerable interface.
	public System.Collections.IEnumerator GetEnumerator()
			{
				return new EnumeratorAdapter<T>(coll.GetIterator());
			}

}; // class CollectionAdapter<T>

}; // namespace Generics
