/*
 * InvalidateEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.InvalidateEventArgs" class.
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

public class InvalidateEventArgs : EventArgs
{
	// Internal state.
	private Rectangle invalidRect;

	// Constructor.
	public InvalidateEventArgs(Rectangle invalidRect)
			{
				this.invalidRect = invalidRect;
			}

	// Get the rectangle that this event applies to.
	public Rectangle InvalidRect
			{
				get
				{
					return invalidRect;
				}
			}

}; // class InvalidateEventArgs

}; // namespace System.Windows.Forms
