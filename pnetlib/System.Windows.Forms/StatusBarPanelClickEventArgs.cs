/*
 * StatusBarPanelClickEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.StatusBarPanelClickEventArgs" class.
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

public class StatusBarPanelClickEventArgs : MouseEventArgs
{
	// Internal state.
	private StatusBarPanel statusBarPanel;

	// Constructor.
	public StatusBarPanelClickEventArgs
				(StatusBarPanel statusBarPanel, MouseButtons button,
				 int clicks, int x, int y)
			: base(button, clicks, x, y, 0)
			{
				this.statusBarPanel = statusBarPanel;
			}

	// Get this object's properties.
	public StatusBarPanel StatusBarPanel
			{
				get
				{
					return statusBarPanel;
				}
			}

}; // class StatusBarPanelClickEventArgs

}; // namespace System.Windows.Forms
