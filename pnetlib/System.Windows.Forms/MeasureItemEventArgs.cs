/*
 * MeasureItemEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.MeasureItemEventArgs" class.
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

public class MeasureItemEventArgs : EventArgs
{
	// Internal state.
	private Graphics graphics;
	private int index;
	private int itemHeight;
	private int itemWidth;

	// Constructors.
	public MeasureItemEventArgs(Graphics graphics, int index)
			{
				this.graphics = graphics;
				this.index = index;
			}
	public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight)
			{
				this.graphics = graphics;
				this.index = index;
				this.itemHeight = itemHeight;
			}

	// Get or set this object's properties.
	public Graphics Graphics
			{
				get
				{
					return graphics;
				}
			}
	public int Index
			{
				get
				{
					return index;
				}
			}
	public int ItemHeight
			{
				get
				{
					return itemHeight;
				}
				set
				{
					itemHeight = value;
				}
			}
	public int ItemWidth
			{
				get
				{
					return itemWidth;
				}
				set
				{
					itemWidth = value;
				}
			}

}; // class MeasureItemEventArgs

}; // namespace System.Windows.Forms
