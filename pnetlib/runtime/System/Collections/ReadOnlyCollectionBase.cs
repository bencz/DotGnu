/*
 * ReadOnlyCollectionBase.cs - Implementation of the
 *			"System.Collections.ReadOnlyCollectionBase" class.
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

namespace System.Collections
{

#if !ECMA_COMPAT

using System;

public abstract class ReadOnlyCollectionBase : ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	protected ReadOnlyCollectionBase()
			{
				list = new ArrayList();
			}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
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
	bool ICollection.IsSynchronized
			{
				get
				{
					return list.IsSynchronized;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return list.SyncRoot;
				}
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Get the inner list that is being used by this collection base.
	protected ArrayList InnerList
			{
				get
				{
					return list;
				}
			}

}; // class ReadOnlyCollectionBase

#endif // !ECMA_COMPAT

}; // namespace System.Collections
