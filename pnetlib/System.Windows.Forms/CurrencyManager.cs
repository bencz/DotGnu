/*
 * CurrencyManager.cs - Implementation of the
 *			"System.Windows.Forms.CurrencyManager" class.
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

public class CurrencyManager : BindingManagerBase
{
	// Internal state.
	private Object dataSource;
	private IList list;
	private int position;

	// Constructor.
	internal CurrencyManager(Object dataSource)
			{
				this.dataSource = dataSource;
				this.list = (dataSource as IList);
				if(list != null)
				{
					this.position = 0;
				}
				else
				{
					this.position = -1;
				}
			}

	// Get the number of rows managed by the manager base.
	public override int Count
			{
				get
				{
					if(list != null)
					{
						return list.Count;
					}
					return 0;
				}
			}

	// Get the current object.
	public override Object Current
			{
				get
				{
					return GetItem(Position);
				}
			}

	// Get the list for this currency manager.
	public IList List
			{
				get
				{
					return list;
				}
			}

	// Get or set the current position.
	public override int Position
			{
				get
				{
					return position;
				}
				set
				{
					if(list != null)
					{
						if(value < 0)
						{
							value = 0;
						}
						if(value >= list.Count)
						{
							value = list.Count - 1;
						}
						if(position != value)
						{
							position = value;
							if(onPositionChangedHandler != null)
							{
								onPositionChangedHandler
									(this, EventArgs.Empty);
							}
						}
					}
				}
			}

	// Get the item at a specific position.
	private Object GetItem(int index)
			{
				if(index >= 0 && index < list.Count)
				{
					return list[index];
				}
				else
				{
					throw new IndexOutOfRangeException
						(S._("SWF_Binding_IndexRange"));
				}
			}

	// Add a new item.
	public override void AddNew()
			{
			#if CONFIG_COMPONENT_MODEL
				if(!(list is IBindingList))
				{
					throw new NotSupportedException();
				}
				((IBindingList)list).AddNew();
				position = list.Count - 1;
				if(onPositionChangedHandler != null)
				{
					onPositionChangedHandler
						(this, EventArgs.Empty);
				}
			#else
				throw new NotSupportedException();
			#endif
			}

	// Cancel the current edit operation.
	public override void CancelCurrentEdit()
			{
				if(position >= 0 && position < Count)
				{
				#if CONFIG_COMPONENT_MODEL
					Object item = GetItem(position);
					if(item is IEditableObject)
					{
						((IEditableObject)item).CancelEdit();
					}
				#endif
					OnItemChanged(new ItemChangedEventArgs(position));
				}
			}

	// End the current edit operation.
	public override void EndCurrentEdit()
			{
			#if CONFIG_COMPONENT_MODEL
				if(position >= 0 && position < Count)
				{
					Object item = GetItem(position);
					if(item is IEditableObject)
					{
						((IEditableObject)item).EndEdit();
					}
				}
			#endif
			}

	// Refresh the bound controls.
	[TODO]
	public void Refresh()
			{
				return;
			}

	// Remove an entry at a specific index.
	public override void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}

	// Resume data binding.
	[TODO]
	public override void ResumeBinding()
			{
				return;
			}

	// Suspend data binding.
	[TODO]
	public override void SuspendBinding()
			{
				return;
			}

	// Check if the list is empty.
	protected void CheckEmpty()
			{
				if(dataSource == null || list == null || list.Count == 0)
				{
					throw new InvalidOperationException
						(S._("SWF_Binding_Empty"));
				}
			}

	// Get the name of list that supplies data for the binding.
	protected internal override String GetListName(ArrayList listAccessors)
			{
			#if CONFIG_COMPONENT_MODEL
				if(list is ITypedList)
				{
					PropertyDescriptor[] props;
					props = new PropertyDescriptor [listAccessors.Count];
					listAccessors.CopyTo(props, 0);
					return ((ITypedList)list).GetListName(props);
				}
			#endif
				return String.Empty;
			}

	[TODO]
	// Update the binding.
	protected override void UpdateIsBinding()
			{
				return;
			}

	// Raise the "CurrentChanged" event.
	protected internal override void OnCurrentChanged(EventArgs e)
			{
				if(onCurrentChangedHandler != null)
				{
					onCurrentChangedHandler(this, e);
				}
			}

	// Event that is emitted when an item changes.
	public event ItemChangedEventHandler ItemChanged;

	// Emit the "ItemChanged" event.
	protected virtual void OnItemChanged(ItemChangedEventArgs e)
			{
				if(ItemChanged != null)
				{
					ItemChanged(this, e);
				}
			}

	// Event that is emitted when the list metadata changes.
	public event EventHandler MetaDataChanged;

#if CONFIG_COMPONENT_MODEL

	[TODO]
	// Get the property descriptors for the binding.
	public override PropertyDescriptorCollection GetItemProperties()
			{
				return null;
			}

#endif // CONFIG_COMPONENT_MODEL

}; // class CurrencyManager

}; // namespace System.Windows.Forms
