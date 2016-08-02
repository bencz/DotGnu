/*
 * HatchBrush.cs - Implementation of the
 *			"System.Drawing.Drawing2D.HatchBrush" class.
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

namespace System.Drawing.Drawing2D
{

using System.Drawing.Toolkit;

public sealed class HatchBrush : Brush
{
	// Internal state.
	private HatchStyle style;
	private Color foreColor;
	private Color backColor;

	// Constructors.
	public HatchBrush(HatchStyle style, Color foreColor)
			{
				this.style = style;
				this.foreColor = foreColor;
			}
	public HatchBrush(HatchStyle style, Color foreColor, Color backColor)
			{
				this.style = style;
				this.foreColor = foreColor;
				this.backColor = backColor;
			}

	// Get the background color of this brush.
	public Color BackgroundColor
			{
				get
				{
					return backColor;
				}
			}

	// Get the foreground color of this brush.
	public Color ForegroundColor
			{
				get
				{
					return foreColor;
				}
			}

	// Get the style of this brush.
	public HatchStyle HatchStyle
			{
				get
				{
					return style;
				}
			}

	// Clone this brush.
	public override Object Clone()
			{
				return new HatchBrush(style, foreColor, backColor);
			}

	// Create this brush for a specific toolkit.  Inner part of "GetBrush()".
	internal override IToolkitBrush CreateBrush(IToolkit toolkit)
			{
				return toolkit.CreateHatchBrush(style, foreColor, backColor);
			}

}; // class HatchBrush

}; // namespace System.Drawing.Drawing2D
