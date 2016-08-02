/*
 * ListChangedEventArgs.cs - Implementation of the
 *			"System.ComponentModel.ListChangedEventArgs" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

public class ListChangedEventArgs : EventArgs
{
	// Internal state.
	private ListChangedType listChangedType;
	private int oldIndex;
	private int newIndex;
	private PropertyDescriptor propDesc;

	// Constructors.
	public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
			{
				this.listChangedType = listChangedType;
				this.newIndex = newIndex;
			}
	public ListChangedEventArgs(ListChangedType listChangedType,
								int newIndex, int oldIndex)
			{
				this.listChangedType = listChangedType;
				this.newIndex = newIndex;
				this.oldIndex = oldIndex;
			}
	public ListChangedEventArgs(ListChangedType listChangedType,
								PropertyDescriptor propDesc)
			{
				this.listChangedType = listChangedType;
				this.propDesc = propDesc;
			}

	// Get the object's property values.
	public ListChangedType ListChangedType
			{
				get
				{
					return listChangedType;
				}
			}
	public int NewIndex
			{
				get
				{
					return newIndex;
				}
			}
	public int OldIndex
			{
				get
				{
					return oldIndex;
				}
			}

}; // class ListChangedEventArgs

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
