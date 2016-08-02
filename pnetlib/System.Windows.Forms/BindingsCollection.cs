/*
 * BindingsCollection.cs - Implementation of the
 *			"System.Windows.Forms.BindingsCollection" class.
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
public class BindingsCollection : BaseCollection
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	internal BindingsCollection() {}

	// Get the number of elements in this collection.
	public override int Count
			{
				get
				{
					return List.Count;
				}
			}

	// Get the binding at a particular position.
	public Binding this[int index]
			{
				get
				{
					return (Binding)(List[index]);
				}
			}

	// Get the array list that underlies this collection
	protected override ArrayList List
			{
				get
				{
					if(list == null)
					{
						list = new ArrayList();
					}
					return list;
				}
			}

	// Add an entry to this collection.
	protected internal void Add(Binding binding)
			{
				AddCore(binding);
			#if CONFIG_COMPONENT_MODEL
				OnCollectionChanged
					(new CollectionChangeEventArgs
						(CollectionChangeAction.Add, binding));
			#endif
			}
	protected virtual void AddCore(Binding binding)
			{
				if(binding == null)
				{
					throw new ArgumentNullException("binding");
				}
				List.Add(binding);
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
				List.Clear();
			}

	// Remove an entry from this collection.
	protected internal void Remove(Binding binding)
			{
				RemoveCore(binding);
			#if CONFIG_COMPONENT_MODEL
				OnCollectionChanged
					(new CollectionChangeEventArgs
						(CollectionChangeAction.Remove, binding));
			#endif
			}
	protected virtual void RemoveCore(Binding binding)
			{
				List.Remove(binding);
			}
	protected internal void RemoveAt(int index)
			{
				Remove(this[index]);
			}

	// Determine if this collection should be serialized.
	protected internal bool ShouldSerializeMyAll()
			{
				return (Count > 0);
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

	// Special class that checks all operations to ensure that they
	// apply to a particular BindingManagerBase object.
	internal class RestrictedBindingsCollection : BindingsCollection
	{
		// Internal state.
		private BindingManagerBase mgr;

		// Constructor.
		public RestrictedBindingsCollection(BindingManagerBase mgr)
				{
					this.mgr = mgr;
				}

		// Add an entry to this collection.
		protected override void AddCore(Binding binding)
				{
					if(binding == null)
					{
						throw new ArgumentNullException("binding");
					}
					if(binding.BindingManagerBase == mgr)
					{
						throw new ArgumentException
							(S._("SWF_Binding_AlreadyPresent"));
					}
					else if(binding.BindingManagerBase != null)
					{
						throw new ArgumentException
							(S._("SWF_Binding_AlreadyAdded"));
					}
					binding.bindingManagerBase = mgr;
					base.AddCore(binding);
				}
	
		// Clear this collection.
		protected override void ClearCore()
				{
					foreach(Binding binding in List)
					{
						binding.bindingManagerBase = null;
					}
					base.ClearCore();
				}
	
		// Remove an entry from this collection.
		protected override void RemoveCore(Binding binding)
				{
					if(binding.BindingManagerBase != mgr)
					{
						throw new ArgumentException
							(S._("SWF_Binding_NotPresent"));
					}
					binding.bindingManagerBase = null;
					base.RemoveCore(binding);
				}

	}; // class RestrictedBindingsCollection

}; // class BindingsCollection

}; // namespace System.Windows.Forms
