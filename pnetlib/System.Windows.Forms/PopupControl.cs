/*
 * PopupControl.cs - Implementation of the
 *			"System.Windows.Forms.PopupControl" class.
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

using System.Drawing;
using System.Drawing.Toolkit;

// This class is used to parent top-level popup windows such as
// menus and combo box drop-down lists.  The mouse and keyboard
// are grabbed while a popup control is visible on-screen.

internal class PopupControl : Control
{
	// Occurs when the mouse is down outside the area of the control
	internal event EventHandler PopDown;
	// Constructor.
	public PopupControl()
			{
				// Popups are not visible by default.
				visible = false;
				SetStyle(ControlStyles.Selectable, false);
			}

	protected override Size DefaultSize
			{
				get
				{
					return new Size(1000,1000);
				}
			}


	internal override IToolkitWindow CreateToolkitWindow(IToolkitWindow parent)
	{
		CreateParams cp = CreateParams;

		// use ControlToolkitManager to create the window thread safe
		return ControlToolkitManager.CreatePopupWindow( this,
				cp.X + ToolkitDrawOrigin.X, cp.Y + ToolkitDrawOrigin.Y,
				 cp.Width - ToolkitDrawSize.Width, cp.Height - ToolkitDrawSize.Height);
	}

	// Trap the "OnMouseDown" event and pop down the window if a
	// mouse click occurs outside the control's bounds.
	protected override void OnMouseDown(MouseEventArgs e)
			{
				// Handle the event normally.
				base.OnMouseDown(e);

				// If the click was outside, then pop down the window.
				int x = e.X;
				int y = e.Y;
				if(x < 0 || x >= Width || y < 0 || y >= Height)
				{
					Hide();
					if (PopDown != null)
					{
						PopDown(this, EventArgs.Empty);
					}
				}
			}

}; // class PopupControl

}; // namespace System.Windows.Forms
