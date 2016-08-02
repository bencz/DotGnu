/*
 * ItemCheckEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.ItemCheckEventArgs" class.
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

using System.Runtime.InteropServices;

#if !ECMA_COMPAT && !CONFIG_COMPACT_FORMS
[ComVisible(true)]
#endif
public class ItemCheckEventArgs : EventArgs
{
	// Internal state.
	private int index;
	private CheckState newCheckValue;
	private CheckState currentValue;

	// Constructor.
	public ItemCheckEventArgs
				(int index, CheckState newCheckValue, CheckState currentValue)
			{
				this.index = index;
				this.newCheckValue = newCheckValue;
				this.currentValue = currentValue;
			}

	// Get the current state of the check box.
	public CheckState CurrentValue
			{
				get
				{
					return currentValue;
				}
			}

	// Get the index of the item to change.
	public int Index
			{
				get
				{
					return index;
				}
			}

	// Get or set the new value of the check box.
	public CheckState NewValue
			{
				get
				{
					return newCheckValue;
				}
				set
				{
					newCheckValue = value;
				}
			}

}; // class ItemCheckEventArgs

}; // namespace System.Windows.Forms
