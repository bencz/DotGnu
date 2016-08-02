/*
 * DrawItemEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.DrawItemEventArgs" class.
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

public class DrawItemEventArgs : EventArgs
{
	// Internal state.
	private Graphics graphics;
	private Font font;
	private Rectangle rect;
	private int index;
	private DrawItemState state;
	private Color foreColor;
	private Color backColor;

	// Constructors.
	public DrawItemEventArgs(Graphics graphics, Font font,
		     				 Rectangle rect, int index, DrawItemState state)
			{
				this.graphics = graphics;
				this.font = font;
				this.rect = rect;
				this.index = index;
				this.state = state;
				this.foreColor = SystemColors.WindowText;
				this.backColor = SystemColors.Window;
			}
	public DrawItemEventArgs(Graphics graphics, Font font,
		     				 Rectangle rect, int index, DrawItemState state,
							 Color foreColor, Color backColor)
			{
				this.graphics = graphics;
				this.font = font;
				this.rect = rect;
				this.index = index;
				this.state = state;
				this.foreColor = foreColor;
				this.backColor = backColor;
			}

	// Get this object's properties.
	public Color BackColor
			{
				get
				{
					return backColor;
				}
			}
	public Rectangle Bounds
			{
				get
				{
					return rect;
				}
			}
	public Color ForeColor
			{
				get
				{
					return foreColor;
				}
			}
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
	public DrawItemState State
			{
				get
				{
					return state;
				}
			}
	public Font Font
			{
				get
				{
					return font;
				}
			}

	// Draw the background of the item, using BackColor.
	public virtual void DrawBackground()
			{
				if ((state & DrawItemState.Selected) > 0)
					graphics.FillRectangle(SystemBrushes.Highlight, rect);	
				else
				{
					using (SolidBrush brush = new SolidBrush(backColor))
						graphics.FillRectangle(brush, rect);
				}
			}

	// Draw the focus rectangle for the item.
	public virtual void DrawFocusRectangle()
			{
				if((state & (DrawItemState.Focus | DrawItemState.NoFocusRect))
						== DrawItemState.Focus)
				{
					ControlPaint.DrawFocusRectangle
						(graphics, rect, foreColor, backColor);
				}
			}

}; // class DrawItemEventArgs

}; // namespace System.Windows.Forms
