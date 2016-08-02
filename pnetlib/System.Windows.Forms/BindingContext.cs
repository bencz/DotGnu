/*
 * BindingContext.cs - Implementation of the
 *			"System.Windows.Forms.BindingContext" class.
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

namespace System.Windows.Forms
{

using System.Collections;
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[DefaultEvent("CollectionChanged")]
#endif
public class BindingContext : ICollection, IEnumerable
{
	// Internal state.
	private Hashtable list;

	// Entry in the binding hash.
	private class BindingHashEntry
	{
		// Internal state.
		private Object dataSource;
		private String dataMember;

		// Constructor.
		public BindingHashEntry(Object dataSource, String dataMember)
				{
					this.dataSource = dataSource;
					this.dataMember = dataMember;
				}

		// Determine if two objects are equal
		public override bool Equals(Object obj)
				{
					BindingHashEntry other = (obj as BindingHashEntry);
					return (other.dataSource == dataSource &&
							other.dataMember == dataMember);
				}

		// Get the hash code for this entry.
		public override int GetHashCode()
				{
					int hash;
					if(dataSource != null)
					{
						hash = dataSource.GetHashCode();
					}
					else
					{
						hash = 0;
					}
					return hash + dataMember.GetHashCode();
				}

	}; // class BindingHashEntry

	// Constructor.
	public BindingContext() {}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				list.Values.CopyTo(array, index);
			}
	int ICollection.Count
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
					return false;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return list.Values.GetEnumerator();
			}

	// Get the binding manager associated with a particular data source.
	public BindingManagerBase this[Object dataSource]
			{
				get
				{
					return this[dataSource, String.Empty];
				}
			}
	[TODO]
	public BindingManagerBase this[Object dataSource, String dataMember]
			{
				get
				{
					// Set the default data member name if necessary.
					if(dataMember == null)
					{
						dataMember = String.Empty;
					}

					// See if we already have an entry for the data source.
					BindingHashEntry key = new BindingHashEntry
						(dataSource, dataMember);
					Object value = list[key];
					if(value != null)
					{
						return (BindingManagerBase)value;
					}

					// TODO: create a new binding manager.
					return null;
				}
			}

	// Add an entry to this collection.
	protected internal void Add
				(Object dataSource, BindingManagerBase listManager)
			{
				AddCore(dataSource, listManager);
			#if CONFIG_COMPONENT_MODEL
				OnCollectionChanged
					(new CollectionChangeEventArgs
						(CollectionChangeAction.Add, dataSource));
			#endif
			}
	protected virtual void AddCore
				(Object dataSource, BindingManagerBase listManager)
			{
				if(dataSource == null)
				{
					throw new ArgumentNullException("dataSource");
				}
				if(listManager == null)
				{
					throw new ArgumentNullException("listManager");
				}
				list[new BindingHashEntry(dataSource, String.Empty)] =
						 listManager;
			}

	// Clear this collection.
	protected internal void Clear()
			{
				ClearCore();
			#if CONFIG_COMPONENT_MODEL
				OnCollectionChanged
					(new CollectionChangeEventArgs
						(CollectionChangeAction.Refresh, null));
			#endif
			}
	protected virtual void ClearCore()
			{
				list.Clear();
			}

	// Remove an entry from this collection.
	protected internal void Remove(Object dataSource)
			{
				RemoveCore(dataSource);
			#if CONFIG_COMPONENT_MODEL
				OnCollectionChanged
					(new CollectionChangeEventArgs
						(CollectionChangeAction.Remove, dataSource));
			#endif
			}
	protected virtual void RemoveCore(Object dataSource)
			{
				list.Remove(new BindingHashEntry(dataSource, String.Empty));
			}

	// Determine if this collection contains a particular data source.
	public bool Contains(Object dataSource)
			{
				return Contains(dataSource, String.Empty);
			}
	public bool Contains(Object dataSource, String dataMember)
			{
				return list.Contains
					(new BindingHashEntry(dataSource, dataMember));
			}

	// Determine if this collection is read-only.
	public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

#if CONFIG_COMPONENT_MODEL

	// Event that is raised when the collection changes.
	public event CollectionChangeEventHandler CollectionChanged;

	// Raise the "CollectionChanged" event.
	protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
			{
				if(CollectionChanged != null)
				{
					CollectionChanged(this, e);
				}
			}

#endif // CONFIG_COMPONENT_MODEL

}; // class BindingContext

}; // namespace System.Windows.Forms
