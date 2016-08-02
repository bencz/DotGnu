/*
 * DrawingPen.cs - Implementation of pens for System.Drawing.Win32.
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

internal class DrawingPen : ToolkitPenBase
{
	// Internal state.
	internal Pen properties;
	internal IntPtr hPen;
	internal int win32Color;
	private IToolkit toolkit;

	// Constructor.
	public DrawingPen(IToolkit toolkit, Pen properties) : base(properties.Color, (int)properties.Width)
			{
				this.toolkit = toolkit;
				this.properties = properties;
				//TODO: Rest of the properties
				win32Color = DrawingGraphics.ColorToWin32(properties.Color);
				hPen = Win32.Api.CreatePen((int)properties.DashStyle, (int)properties.Width, win32Color);
			}

	// Select this pen into a graphics object.
	public override void Select(IToolkitGraphics graphics)
			{
				(graphics as DrawingGraphics).Pen = this;
			}

	// Select a brush-based pen into a graphics object.
	public override void Select(IToolkitGraphics graphics, IToolkitBrush brush)
			{
				Select(graphics);
			}

	// Dispose of this object.
	protected override void Dispose(bool disposing)
			{
				Win32.Api.DeleteObject(hPen);
				hPen = IntPtr.Zero;
			}

}; // class DrawingPen

}; // namespace System.Drawing.Toolkit
