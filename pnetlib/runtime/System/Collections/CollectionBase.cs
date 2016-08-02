/*
 * CollectionBase.cs - Implementation of the
 *			"System.Collections.CollectionBase" class.
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

public abstract class CollectionBase : IList, ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	protected CollectionBase()
			{
				list = new ArrayList();
			}

	// Implement the IList interface.
	int IList.Add(Object value)
			{
				OnValidate(value);
				int index = list.Count;
				OnInsert(index, value);
				index = list.Add(value);
				try
				{
					OnInsertComplete(index, value);
				}
				catch(Exception)
				{
					list.RemoveAt(index);
					throw;
				}
				return index;
			}
	public void Clear()
			{
				OnClear();
				list.Clear();
				OnClearComplete();
			}
	bool IList.Contains(Object value)
			{
				return list.Contains(value);
			}
	int IList.IndexOf(Object value)
			{
				return list.IndexOf(value);
			}
	void IList.Insert(int index, Object value)
			{
				OnValidate(value);
				OnInsert(index, value);
				list.Insert(index, value);
				try
				{
					OnInsertComplete(index, value);
				}
				catch(Exception)
				{
					list.RemoveAt(index);
					throw;
				}
			}
	void IList.Remove(Object value)
			{
				OnValidate(value);
				int index = list.IndexOf(value);
				if(index != -1)
				{
					OnRemove(index, value);
					list.RemoveAt(index);
					try
					{
						OnRemoveComplete(index, value);
					}
					catch(Exception)
					{
						list.Insert(index, value);
						throw;
					}
				}
				else
				{
					throw new ArgumentException(_("Arg_NotFound"));
				}
			}
	public void RemoveAt(int index)
			{
				if(index < 0 || index >= list.Count)
				{
					throw new ArgumentOutOfRangeException
						("index", _("Arg_InvalidArrayIndex"));
				}
				Object value = list[index];
				OnRemove(index, value);
				list.RemoveAt(index);
				try
				{
					OnRemoveComplete(index, value);
				}
				catch(Exception)
				{
					list.Insert(index, value);
					throw;
				}
			}
	bool IList.IsFixedSize
			{
				get
				{
					return list.IsFixedSize;
				}
			}
	bool IList.IsReadOnly
			{
				get
				{
					return list.IsReadOnly;
				}
			}
	Object IList.this[int index]
			{
				get
				{
					return list[index];
				}
				set
				{
					if(index < 0 || index >= list.Count)
					{
						throw new ArgumentOutOfRangeException
							("index", _("Arg_InvalidArrayIndex"));
					}
					OnValidate(value);
					Object oldValue = list[index];
					OnSet(index, oldValue, value);
					list[index] = value;
					try
					{
						OnSetComplete(index, oldValue, value);
					}
					catch(Exception)
					{
						list[index] = oldValue;
						throw;
					}
				}
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

	// Get this collection base, represented as an IList.
	protected IList List
			{
				get
				{
					return this;
				}
			}

	// Collection control methods.
	protected virtual void OnClear() {}
	protected virtual void OnClearComplete() {}
	protected virtual void OnInsert(int index, Object value) {}
	protected virtual void OnInsertComplete(int index, Object value) {}
	protected virtual void OnRemove(int index, Object value) {}
	protected virtual void OnRemoveComplete(int index, Object value) {}
	protected virtual void OnSet(int index, Object oldValue, Object newValue) {}
	protected virtual void OnSetComplete
		(int index, Object oldValue, Object newValue) {}
	protected virtual void OnValidate(Object value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
			}

}; // class CollectionBase

#endif // !ECMA_COMPAT

}; // namespace System.Collections
