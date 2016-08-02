/*
 * DrawingHatchBrush.cs - Implementation of hatch brushes for System.Drawing.
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

namespace System.Drawing.Toolkit
{

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;
using Xsharp;

internal sealed class DrawingHatchBrush : ToolkitBrushBase
{
	// Internal state.
	private HatchStyle style;
	private System.Drawing.Color foreColor;
	private System.Drawing.Color backColor;
	private static Xsharp.Bitmap[] hatchBitmaps;

	// Constructor.
	public DrawingHatchBrush(HatchStyle style,
							 System.Drawing.Color foreColor,
							 System.Drawing.Color backColor) : base (foreColor)
			{
				this.style = style;
				this.foreColor = foreColor;
				this.backColor = backColor;
			}

	// Select this brush into a graphics object.
	public override void Select(IToolkitGraphics _graphics)
			{
				DrawingGraphics graphics = (_graphics as DrawingGraphics);
				if(graphics != null)
				{
					Xsharp.Graphics g = graphics.graphics;
					Xsharp.Bitmap bitmap;
					lock(typeof(DrawingHatchBrush))
					{
						bitmap = GetBitmap(style);
					}
					g.Function = GCFunction.GXcopy;
					g.SubwindowMode = SubwindowMode.ClipByChildren;
					if(bitmap != null)
					{
						// Use an opaque stipple to fill the region.
						g.Foreground =
							DrawingToolkit.DrawingToXColor(foreColor);
						g.Background =
							DrawingToolkit.DrawingToXColor(backColor);
						g.SetFillOpaqueStippled(bitmap, 0, 0);
					}
					else
					{
						// We don't recognize this hatch style, so use a
						// solid brush with the foreground color.
						g.Foreground =
							DrawingToolkit.DrawingToXColor(foreColor);
						g.SetFillSolid();
					}
					graphics.Brush = this;
				}
			}

	// Dispose of this brush.
	protected override void Dispose(bool disposing)
			{
				// Nothing to do here in this implementation.
			}

	// Get the bitmap corresponding to a particular hatch style.
	private static Xsharp.Bitmap GetBitmap(HatchStyle style)
			{
				Xsharp.Bitmap bitmap;

				// See if we have a cached bitmap for this style.
				if(((int)style) >= 0 && ((int)style) <= 52)
				{
					if(hatchBitmaps == null)
					{
						hatchBitmaps = new Xsharp.Bitmap [53];
					}
					bitmap = hatchBitmaps[(int)style];
					if(bitmap != null)
					{
						return bitmap;
					}
				}
				else
				{
					return null;
				}

				// Get the raw bits for the hatch bitmap.
				byte[] bits = GetBits(style);
				if(bits == null)
				{
					return null;
				}

				// Create the bitmap, cache it for later, and then return it.
				bitmap = new Xsharp.Bitmap(16, 16, bits);
				hatchBitmaps[(int)style] = bitmap;
				return bitmap;
			}

}; // class DrawingHatchBrush

}; // namespace System.Drawing.Toolkit
