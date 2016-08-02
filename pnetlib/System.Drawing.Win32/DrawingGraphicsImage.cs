/*
 * DrawingGraphicsImage.cs - Implementation of "DrawingGraphicsImage" for System.Drawing.Win32.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using DotGNU.Images;

internal class DrawingGraphicsImage : ToolkitGraphicsImageBase, IDisposable
{

	public DrawingGraphicsImage(IToolkit toolkit, IToolkitImage image)
		: base(toolkit, image) {}

	[TODO]
	// Draw a string using the current font and brush.
	public override void DrawString
		(String s, int x, int y, StringFormat format)
	{
	}

	protected override void Dispose(bool Disposing)
	{
		// Nothing to do.
	}

	[TODO]
	// Measure a string using the current font and a given layout rectangle.
	public override Size MeasureString( String s, System.Drawing.Point[] layoutRectangle, StringFormat format, out int charactersFitted, out int linesFilled, bool ascentOnly )
	{
		charactersFitted = 0;
		linesFilled = 0;
		return new Size(0,0);
	}

	[TODO]
	// Get the line spacing for the font selected into this graphics object.
	public override int GetLineSpacing()
	{
		return 0;
	}

}; // class DrawingGraphicsImage

}; // namespace System.Drawing.Toolkit
