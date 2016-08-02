/*
 * DrawingGraphics.cs - Implementation of graphics drawing for System.Drawing.
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
using System.Runtime.InteropServices;
using Xsharp;

internal sealed class DrawingGraphics : ToolkitGraphicsBase
{
	// Internal state.
	internal Xsharp.Graphics graphics;
	private bool gaveHdc;

	// Constructor.
	public DrawingGraphics(IToolkit toolkit, Xsharp.Graphics graphics)
			: base(toolkit)
			{
				this.graphics = graphics;
				this.font = null;
				this.gaveHdc = false;
			}


	// Get the dpi of the screen for the x axis.
	public override float DpiX
			{
				get { return graphics.DpiX; }
			}

	// Get the dpi of the screen for the y axis.
	public override float DpiY
			{
				get { return graphics.DpiY; }
			}


	// Dispose of this object.
	protected override void Dispose(bool disposing)
			{
				if(graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
			}

	[TODO]
	// Clear the entire drawing surface.
	public override void Clear(System.Drawing.Color color)
			{
				graphics.Clear();
			}

	// Draw a line between two points using the current pen.
	public override void DrawLine(int x1, int y1, int x2, int y2)
			{
				graphics.DrawLine(x1, y1, x2, y2);
				graphics.DrawPoint(x2, y2);
			}

	// Convert an array of "System.Drawing.Point" objects into an array of
	// "Xsharp.Point" objects.
	private static Xsharp.Point[] ConvertPoints
				(System.Drawing.Point[] points, bool dupFirst)
			{
				Xsharp.Point[] newPoints;
				if(dupFirst)
				{
					newPoints = new Xsharp.Point [points.Length + 1];
				}
				else
				{
					newPoints = new Xsharp.Point [points.Length];
				}
				int posn;
				for(posn = 0; posn < points.Length; ++posn)
				{
					newPoints[posn].x = RestrictXY(points[posn].X);
					newPoints[posn].y = RestrictXY(points[posn].Y);
				}
				if(dupFirst)
				{
					newPoints[points.Length] = newPoints[0];
				}
				return newPoints;
			}

	// Draw a set of connected line seguments using the current pen.
	public override void DrawLines(System.Drawing.Point[] points)
			{
				graphics.DrawLines(ConvertPoints(points, false));
			}

	// Draw a polygon using the current pen.
	public override void DrawPolygon(System.Drawing.Point[] points)
			{
				graphics.DrawLines(ConvertPoints(points, true));
			}

	// Fill a polygon using the current brush.
	public override void FillPolygon
				(System.Drawing.Point[] points, FillMode fillMode)
			{
				if(fillMode == FillMode.Alternate)
				{
					graphics.FillRule = FillRule.EvenOddRule;
				}
				else
				{
					graphics.FillRule = FillRule.WindingRule;
				}
				graphics.FillPolygon(ConvertPoints(points, false));
			}

	// Draw an arc within a rectangle defined by four points.
	public override void DrawArc
				(System.Drawing.Point[] rect,
				 float startAngle, float sweepAngle)
			{
				// Slight bug: this won't work for rotated arcs.
				int x = RestrictXY(rect[0].X);
				int y = RestrictXY(rect[0].Y);
				int width = RestrictXY(rect[1].X) - x;
				int height = RestrictXY(rect[2].Y) - y;
				graphics.DrawArc(x, y, width, height,
								 -startAngle, -sweepAngle);
			}

	// Draw a pie slice within a rectangle defined by four points.
	public override void DrawPie
				(System.Drawing.Point[] rect,
				 float startAngle, float sweepAngle)
			{
				// Slight bug: this won't work for rotated arcs.
				int x = RestrictXY(rect[0].X);
				int y = RestrictXY(rect[0].Y);
				int width = RestrictXY(rect[1].X) - x;
				int height = RestrictXY(rect[2].Y) - y;
				graphics.DrawPie(x, y, width, height,
								 -startAngle, -sweepAngle);
			}

	// Fill a pie slice within a rectangle defined by four points.
	public override void FillPie
				(System.Drawing.Point[] rect,
				 float startAngle, float sweepAngle)
			{
				// Slight bug: this won't work for rotated arcs.
				int x = RestrictXY(rect[0].X);
				int y = RestrictXY(rect[0].Y);
				int width = RestrictXY(rect[1].X) - x;
				int height = RestrictXY(rect[2].Y) - y;
				graphics.ArcMode = ArcMode.ArcPieSlice;
				graphics.FillArc(x, y, width, height,
								 -startAngle, -sweepAngle);
			}

	// Draw a string using the current font and brush.
	public override void DrawString
				(String s, int x, int y, StringFormat format)
			{
				Xsharp.Font xfont = (font as DrawingFont).xfont;
				FontExtents extents = xfont.GetFontExtents(graphics);
				xfont.DrawString(graphics, RestrictXY(x),
								 RestrictXY(y) + extents.Ascent,
								 s, 0, s.Length);
			}

	// Measure a string using the current font and a given layout rectangle.
	public override Size MeasureString
				(String s, System.Drawing.Point[] layoutRectangle,
				 StringFormat format, out int charactersFitted,
				 out int linesFilled, bool ascentOnly)
			{
				// TODO: line wrapping, etc
				int width, ascent, descent;
				charactersFitted = 0;
				linesFilled = 0;
				Xsharp.Font xfont = (font as DrawingFont).xfont;
				xfont.MeasureString
					(graphics, s, 0, s.Length,
					 out width, out ascent, out descent);
				if(!ascentOnly)
				{
					return new Size(width, ascent + descent);
				}
				else
				{
					return new Size(width, ascent);
				}
			}

	// Flush the graphics subsystem
	public override void Flush(FlushIntention intention)
			{
				if(intention == FlushIntention.Flush)
				{
					((DrawingToolkit)Toolkit).app.Display.Flush();
				}
				else
				{
					((DrawingToolkit)Toolkit).app.Display.Sync();
				}
			}

	/// Get the HDC associated with this graphics object.
	/// The HDC is a GCHandle to the Xsharp.Graphics object.
	/// To unwrap, just cast the IntPtr to a GCHandle and then
	/// cast the Target of the GCHandle to Xsharp.Graphics.	
	///
	/// e.g.
	///
	/// GCHandle gcHandle;
	///
	/// // Convert the Hdc into a GCHandle.
	/// gcHandle = (GCHandle)graphics.GetHdc();
	///
	/// // Convert the GCHandle into an Xsharp.Graphics object.
	/// xsharpGraphics = (Xsharp.Graphics)(gcHandle.Target);
	/// 
	/// // Always release the Hdc after you've gotten the Xsharp.Graphics object.
	/// // Make sure you release the gcHandle returned by the previous call
	/// // to GetHdc().  Calling GetHdc again will return a completely different
	/// // handle.
	/// graphics.ReleaseHdc((IntPtr)gcHandle);
	///
	/// // Now you can use the Xsharp.Graphics object to do anything you like.
	///
	public override IntPtr GetHdc()
			{
				if(gaveHdc)
				{
					throw new InvalidOperationException(/* TODO */);
				}
				gaveHdc = true;

				GCHandle handle = GCHandle.Alloc(graphics);

				return (IntPtr)handle;
			}

	// Release a HDC that was obtained using "GetHdc()".
	public override void ReleaseHdc(IntPtr hdc)
			{
				if(!gaveHdc)
				{
					throw new InvalidOperationException(/* TODO */);
				}
				gaveHdc = false;

				GCHandle handle = (GCHandle)hdc;

				handle.Free();				
			}

	// Set the clipping region to empty.
	public override void SetClipEmpty()
			{
				using (Xsharp.Region region = new Xsharp.Region())
				{
					graphics.SetClipRegion( region, 0, 0);
				}
			}

	// Set the clipping region to infinite (i.e. disable clipping).
	public override void SetClipInfinite()
			{
				using (Xsharp.Region region = new Xsharp.Region(short.MinValue, short.MinValue, ushort.MaxValue, ushort.MaxValue))
				{
					graphics.SetClipRegion( region, 0, 0);
				}
			}

	// Set the clipping region to a single rectangle.
	public override void SetClipRect(int x, int y, int width, int height)
			{
				using (Xsharp.Region region = new Xsharp.Region(x, y, width, height))
				{
					graphics.SetClipRegion( region, 0, 0);
				}
			}

	// Set the clipping region to a list of rectangles.
	public override void SetClipRects(System.Drawing.Rectangle[] rects)
			{
				using (Xsharp.Region region = rectsToRegion(rects))
				{
					graphics.SetClipRegion( region);
				}
			}

	[TODO]
	// Set the clipping region to a complex mask.
	public override void SetClipMask(Object mask, int topx, int topy)
			{
				return;
			}

	// Get the line spacing for the font selected into this graphics object.
	public override int GetLineSpacing()
			{
				Xsharp.Font xfont = (font as DrawingFont).xfont;
				FontExtents extents = xfont.GetFontExtents(graphics);
				return extents.Ascent + extents.Descent;
			}

	// Convert a System.Drawing.Region to Xsharp.Region
	private static Xsharp.Region rectsToRegion( System.Drawing.Rectangle[] rectangles)
		{
			Xsharp.Region newRegion =  new Xsharp.Region();
			for( int i = 0; i < rectangles.Length; i++)
			{
				System.Drawing.Rectangle rect = rectangles[i];
				// This implementation has a region size restriction.
				int left = RestrictXY(rect.Left);
				int top = RestrictXY(rect.Top);
				int right = RestrictXY(rect.Right);
				int bottom = RestrictXY(rect.Bottom);
				newRegion.Union( left, top, right - left, bottom - top);
			}
			return newRegion;
		}

	// Make sure a x or y fits within the X drawing position restriction
	// Because internally coordinates are represented by shorts, the x, y
	// position plus the window coordinates cant exceed this. So we make
	// the |min|, |max| doesnt exceed short/2.
	internal static int RestrictXY(int value)
	{
		if (value < short.MinValue/2)
			value = short.MinValue/2;
		else if (value > short.MaxValue/2)
			value = short.MaxValue/2;
		return value;
	}

	// Draw an image.
	public override void DrawImage
				(IToolkitImage image, System.Drawing.Point[] src,
				 System.Drawing.Point[] dest)
			{
				if((src[1].X - src[0].X) == (dest[1].X - dest[0].X) &&
				   (src[2].Y - src[0].Y) == (dest[2].Y - dest[0].Y))
				{
					// Draw a sub-image, without stretching the pixels.
					Xsharp.Image ximage =
						(image as DrawingImage).GetNativeImage();
					if(ximage != null)
					{
						graphics.DrawImage(dest[0].X, dest[0].Y, ximage,
										   src[0].X, src[0].Y,
										   src[1].X - src[0].X,
										   src[2].Y - src[0].Y);
					}
				}
				else
				{
					// Hard case: stretch the image before drawing it.
					base.DrawImage(image, src, dest);
				}
			}
	public override void DrawImage(IToolkitImage image, int x, int y)
			{
				Xsharp.Image ximage = (image as DrawingImage).GetNativeImage();
				if(ximage != null)
				{
					graphics.DrawImage(x, y, ximage);
				}
			}

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	public override void DrawGlyph(int x, int y,
				   				   byte[] bits, int bitsWidth, int bitsHeight,
				   				   System.Drawing.Color color)
			{
				Xsharp.Bitmap bitmap;
				bitmap = new Xsharp.Bitmap(bitsWidth, bitsHeight, bits);
				try
				{
					graphics.Foreground = DrawingToolkit.DrawingToXColor(color);
					graphics.SetFillStippled(bitmap, x, y);
					graphics.FillRectangle(x, y, bitsWidth, bitsHeight);
					graphics.SetFillSolid();
				}
				finally
				{
					bitmap.Destroy();
				}
			}

}; // class DrawingGraphics

}; // namespace System.Drawing.Toolkit
