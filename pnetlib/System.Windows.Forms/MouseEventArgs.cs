/*
 * MouseEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.MouseEventArgs" class.
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
public class MouseEventArgs : EventArgs
{
	// Internal state.
	private MouseButtons button;
	private int x, y;
#if !CONFIG_COMPACT_FORMS
	private int clicks, delta;
#endif

	// Constructor.
	public MouseEventArgs(MouseButtons button, int clicks,
						  int x, int y, int delta)
			{
				this.button = button;
				this.x = x;
				this.y = y;
			#if !CONFIG_COMPACT_FORMS
				this.clicks = clicks;
				this.delta = delta;
			#endif
			}

	// Get this object's properties.
	public MouseButtons Button
			{
				get
				{
					return button;
				}
			}
	public int X
			{
				get
				{
					return x;
				}
			}
	public int Y
			{
				get
				{
					return y;
				}
			}
#if !CONFIG_COMPACT_FORMS
	public int Clicks
			{
				get
				{
					return clicks;
				}
			}
	public int Delta
			{
				get
				{
					return delta;
				}
			}
#endif

}; // class MouseEventArgs

}; // namespace System.Windows.Forms
