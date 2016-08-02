/*
 * DrawingTextureBrush.cs - Implementation of texture brushes for System.Drawing.Win32.
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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

internal class DrawingTextureBrush : DrawingBrush, IToolkitBrush
{
	// Internal state.
	private TextureBrush properties;
	private DrawingImage image;
	private RectangleF dstRect;
	private ImageAttributes imageAttr;
	private IntPtr selectedHdc = IntPtr.Zero;

	// Constructor.
	public DrawingTextureBrush(IToolkit toolkit, TextureBrush properties,
		IToolkitImage image, RectangleF dstRect,
		ImageAttributes imageAttr) : base(toolkit, Color.Black)
	{
		this.properties = properties;
		this.image = image as DrawingImage;
		this.dstRect = dstRect;
		this.imageAttr = imageAttr;
	}

	public override void Select(IToolkitGraphics graphics)
	{
		// See if the cached brush is for this hdc.
		if (selectedHdc != (graphics as DrawingGraphics).hdc)
		{
			// If we have previously selected into a different hdc, then delete and recreate.
			if (selectedHdc != IntPtr.Zero)
			{
				Win32.Api.DeleteObject(hBrush);
			}

			hBrush = image.BrushFromBitmap((graphics as DrawingGraphics).hdc);
		}
		base.Select(graphics);
	}

}; // class DrawingTextureBrush

}; // namespace System.Drawing.Toolkit
