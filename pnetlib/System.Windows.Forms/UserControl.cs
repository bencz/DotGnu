/*
 * UserControl.cs - Implementation of the
 *			"System.Windows.Forms.UserControl" class.
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

public class UserControl : ContainerControl
{
	// Constructor.
	public UserControl()
			{
			}

	// Event that is emitted before the control becomes
	// visible for the first time.
	public event EventHandler Load
			{
				add
				{
					AddHandler(EventId.Load, value);
				}
				remove
				{
					RemoveHandler(EventId.Load, value);
				}
			}

	// Get the default size of the user control
	protected override Size DefaultSize
			{
				get
				{
					return new Size(150, 150);
				}
			}

	// Override the control create event.
	protected override void OnCreateControl()
			{
				base.OnCreateControl();
				OnLoad(EventArgs.Empty);
			}

	// Raise the "Load" event.
	protected virtual void OnLoad(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Load));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	// Override the mouse down event.
	protected override void OnMouseDown(MouseEventArgs e)
			{
				if (ActiveControl == null)
				{
					SelectNextControl(null, true, true, true, false);
				}
				
				base.OnMouseDown(e);
			}

	// Override Select. Try to select the next available control,
	// preferably the first on in this UserControl.
	protected override void Select(bool directed, bool forward)
			{
				SelectNextControl(null, forward, true, true, false);
			}

#if !CONFIG_COMPACT_FORMS

	// Process a message.
	protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
			}

#endif // !CONFIG_COMPACT_FORMS

}; // class UserControl

}; // namespace System.Windows.Forms
