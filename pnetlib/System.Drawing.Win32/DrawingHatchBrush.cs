/*
 * DrawingHatchBrush.cs - Implementation of hatch brushes for System.Drawing.Win32.
 * Copyright (C) 2003  Neil Cawse.
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
using System.Drawing.Drawing2D;

internal class DrawingHatchBrush : DrawingBrush, IToolkitBrush
{
	// Internal state.
	private int foreColor;
	private int backColor;
	private static IntPtr[] hatchBitmaps;

	public DrawingHatchBrush(IToolkit toolkit, HatchStyle style,
							 System.Drawing.Color foreColor,
							 System.Drawing.Color backColor) : base (toolkit, backColor)
			{
				this.foreColor = DrawingGraphics.ColorToWin32(foreColor);
				this.backColor = DrawingGraphics.ColorToWin32(backColor);
				IntPtr hBi;
				lock(typeof(DrawingHatchBrush))
				{
					hBi = GetBitmap(style);
				}
				if (hBi != IntPtr.Zero)
				{
					hBrush = Win32.Api.CreatePatternBrush(hBi);
				}
				else
					//not one of the types we recognize, so make it a solid brush
					hBrush = DrawingSolidBrush.CreateSolidBrush(foreColor);
				
			}

	// Select this brush into a graphics object.
	public override void Select(IToolkitGraphics graphics)
			{
				Win32.Api.SetTextColor( (graphics as DrawingGraphics).hdc, backColor);
				Win32.Api.SetBkColor( (graphics as DrawingGraphics).hdc,foreColor);
			
				base.Select(graphics);
			}

	// Get the bitmap corresponding to a particular hatch style.
	private static IntPtr GetBitmap(HatchStyle style)
	{
		IntPtr bitmap;

		// See if we have a cached bitmap for this style.
		if(((int)style) >= 0 && ((int)style) <= 52)
		{
			if (hatchBitmaps == null)
			{
				hatchBitmaps = new IntPtr[53];
			}
			bitmap = hatchBitmaps[(int)style];
			if (bitmap != IntPtr.Zero)
			{
				return bitmap;
			}
		}
		else
		{
			return IntPtr.Zero;
		}

		// Get the raw bits for the hatch bitmap.
		byte[] bits = GetBits(style);
		if(bits == null)
		{
			return IntPtr.Zero;
		}

		// Create the bitmap, cache it for later, and then return it.
		Array.Reverse(bits);
		bitmap = Win32.Api.CreateBitmap(16, 16, 1, 1, bits);
		hatchBitmaps[(int)style] = bitmap;
		return bitmap;
				
	}

}; // class DrawingHatchBrush

}; // namespace System.Drawing.Toolkit
