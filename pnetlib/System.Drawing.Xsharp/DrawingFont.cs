/*
 * DrawingFont.cs - Implementation of fonts for System.Drawing.
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

internal sealed class DrawingFont : IToolkitFont
{
	// Internal state.
	private System.Drawing.Font properties;
	private float dpi;
	internal Xsharp.Font xfont;

	// Fudge factor for converting a Windows-style point size
	// into and an X-style point size. The Windows point sizes
	// are consistant across different DPI settings while X
	// treats point sizes, correctly, as dependent on DPI. The
	// faked Windows point sizes appear to be the same as those
	// of X on a 96 DPI setting with no conversions.
	private const float PointSizeConversion = 96.0f;

	// Constructor.
	public DrawingFont(System.Drawing.Font properties, float dpi)
			{
				this.properties = properties;
				this.dpi = dpi;
				this.xfont = null;
			}

	// Select this font into a graphics object.
	public void Select(IToolkitGraphics _graphics)
			{
				DrawingGraphics graphics = (_graphics as DrawingGraphics);
				if(graphics != null)
				{
					lock(this)
					{
						if(xfont == null)
						{
							xfont = Xsharp.Font.CreateFont
								(properties.Name,
								 (int)(properties.SizeInPoints *
								 	   (10.0f * (PointSizeConversion/dpi))),
								 (Xsharp.FontStyle)(properties.Style));
						}
						graphics.Font = this;
					}
				}
			}

	// Dispose of this pen.
	public void Dispose()
			{
				// Nothing to do here in this implementation.
			}

	// Get the raw HFONT for this toolkit font.  IntPtr.Zero if none.
	public IntPtr GetHfont()
			{
				// Nothing to do here in this implementation.
				return IntPtr.Zero;
			}

	// Get the LOGFONT information for this toolkit font.
	public void ToLogFont(Object lf, IToolkitGraphics graphics)
			{
				// Nothing to do here in this implementation.
			}

}; // class DrawingFont

}; // namespace System.Drawing.Toolkit
