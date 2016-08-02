/*
 * UICuesEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.UICuesEventArgs" class.
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

public class UICuesEventArgs : EventArgs
{
	// Internal state.
	private UICues uicues;

	// Constructor.
	public UICuesEventArgs(UICues uicues)
			{
				this.uicues = uicues;
			}

	// Get this object's properties.
	public UICues Changed
			{
				get
				{
					return (uicues & UICues.Changed);
				}
			}
	public bool ChangeFocus
			{
				get
				{
					return ((uicues & UICues.ChangeFocus) != 0);
				}
			}
	public bool ChangeKeyboard
			{
				get
				{
					return ((uicues & UICues.ChangeKeyboard) != 0);
				}
			}
	public bool ShowFocus
			{
				get
				{
					return ((uicues & UICues.ShowFocus) != 0);
				}
			}
	public bool ShowKeyboard
			{
				get
				{
					return ((uicues & UICues.ShowKeyboard) != 0);
				}
			}

}; // class UICuesEventArgs

}; // namespace System.Windows.Forms
