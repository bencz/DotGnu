/*
 * StatusBarDrawItemEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.StatusBarDrawItemEventArgs" class.
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

public class StatusBarDrawItemEventArgs : DrawItemEventArgs
{
	// Internal state.
	private StatusBarPanel panel;

	// Constructors.
	public StatusBarDrawItemEventArgs
					(Graphics graphics, Font font, Rectangle rect,
					 int index, DrawItemState state, StatusBarPanel panel)
			: base(graphics, font, rect, index, state)
			{
				this.panel = panel;
			}
	public StatusBarDrawItemEventArgs
					(Graphics graphics, Font font, Rectangle rect,
					 int index, DrawItemState state, StatusBarPanel panel,
					 Color foreColor, Color backColor)
			: base(graphics, font, rect, index, state, foreColor, backColor)
			{
				this.panel = panel;
			}

	// Get this object's properties.
	public StatusBarPanel Panel
			{
				get
				{
					return panel;
				}
			}

}; // class StatusBarDrawItemEventArgs

}; // namespace System.Windows.Forms
