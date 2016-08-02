/*
 * BindingManagerBase.cs - Implementation of the
 *			"System.Windows.Forms.BindingManagerBase" class.
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

using System.ComponentModel;
using System.Collections;

public abstract class BindingManagerBase
{
	// Internal state.
	protected EventHandler onCurrentChangedHandler;
	protected EventHandler onPositionChangedHandler;
	private BindingsCollection bindings;
	private bool inPull;

	// Constructor.
	public BindingManagerBase() {}

	// Get the bindings that are being managed.
	public BindingsCollection Bindings
			{
				get
				{
					if(bindings == null)
					{
						bindings =
							new BindingsCollection.RestrictedBindingsCollection
								(this);
					}
					return bindings;
				}
			}

	// Get the number of rows managed by the manager base.
	public abstract int Count { get; }

	// Get the current object.
	public abstract Object Current { get; }

	// Get or set the current position.
	public abstract int Position { get; set; }

	// Add a new item.
	public abstract void AddNew();

	// Cancel the current edit operation.
	public abstract void CancelCurrentEdit();

	// End the current edit operation.
	public abstract void EndCurrentEdit();

	// Remove an entry at a specific index.
	public abstract void RemoveAt(int index);

	// Resume data binding.
	public abstract void ResumeBinding();

	// Suspend data binding.
	public abstract void SuspendBinding();

	// Get the name of list that supplies data for the binding.
	protected internal abstract String GetListName(ArrayList listAccessors);

	// Pull data from the data bound control into the data source.
	protected void PullData()
			{
				inPull = true;
				try
				{
					UpdateIsBinding();
					foreach(Binding binding in Bindings)
					{
						binding.PullData();
					}
				}
				finally
				{
					inPull = false;
				}
			}

	// Push data from the data source into the data bound control.
	protected void PushData()
			{
				if(!inPull)
				{
					UpdateIsBinding();
					foreach(Binding binding in Bindings)
					{
						binding.PushData();
					}
				}
			}

	// Update the binding.
	protected abstract void UpdateIsBinding();

	// Event that is emitted when the current object changes.
	public event EventHandler CurrentChanged
			{
				add
				{
					onCurrentChangedHandler += value;
				}
				remove
				{
					onCurrentChangedHandler -= value;
				}
			}

	// Event that is emitted when the position changes.
	public event EventHandler PositionChanged
			{
				add
				{
					onPositionChangedHandler += value;
				}
				remove
				{
					onPositionChangedHandler -= value;
				}
			}

	// Raise the "CurrentChanged" event.
	protected internal abstract void OnCurrentChanged(EventArgs e);

#if CONFIG_COMPONENT_MODEL

	// Get the property descriptors for the binding.
	public abstract PropertyDescriptorCollection GetItemProperties();
	[TODO]
	protected internal virtual PropertyDescriptorCollection GetItemProperties
				(ArrayList dataSources, ArrayList listAccessors)
			{
				return null;
			}
	[TODO]
	protected virtual PropertyDescriptorCollection GetItemProperties
				(Type listType, int offset,
				 ArrayList dataSources, ArrayList listAccessors)
			{
				return null;
			}

#endif // CONFIG_COMPONENT_MODEL

}; // class BindingManangerBase

}; // namespace System.Windows.Forms
