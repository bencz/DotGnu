/*
 * LabelEditEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.LabelEditEventArgs" class.
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

public class LabelEditEventArgs : EventArgs
{
	// Internal state.
	private int item;
	private String label;
	private bool cancelEdit;

	// Constructors.
	public LabelEditEventArgs(int item)
			{
				this.item = item;
			}
	public LabelEditEventArgs(int item, String label)
			{
				this.item = item;
				this.label = label;
			}

	// Get this object's properties.
	public bool CancelEdit
			{
				get
				{
					return cancelEdit;
				}
				set
				{
					cancelEdit = value;
				}
			}
	public int Item
			{
				get
				{
					return item;
				}
			}
	public String Label
			{
				get
				{
					return label;
				}
			}

}; // class LabelEditEventArgs

}; // namespace System.Windows.Forms
