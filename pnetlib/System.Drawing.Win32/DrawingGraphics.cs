/*
 * DrawingGraphics.cs - Implementation of graphics drawing for System.Drawing.Win32.
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

internal class DrawingGraphics : ToolkitGraphicsBase, IDisposable
{
	// Internal state.
	//internal Xsharp.Font font;
	internal IntPtr hdc;
	private bool gaveHdc;

	public DrawingGraphics(IToolkit toolkit, IntPtr hdc)
		: base(toolkit)
	{
		this.hdc = hdc;
		this.gaveHdc = false;
	}


	// Dispose of this object.
	protected override void Dispose(bool disposing)
	{
		DeleteDC();
	}

	// Clear the entire drawing surface.
	public override void Clear(Color color)
	{
		//graphics.Clear();
	}

	// Draw a line between two points using the current pen.
	public override void DrawLine( int x1, int y1, int x2, int y2 )
	{
		Win32.Api.SetBkMode(hdc, Win32.Api.BackGroundModeType.TRANSPARENT);
		IntPtr prevhPen = Win32.Api.SelectObject( hdc, (pen as DrawingPen).hPen );
		Win32.Api.MoveToEx( hdc, x1, y1,IntPtr.Zero );
		Win32.Api.LineTo( hdc, x2, y2 );
		//set last point
		Win32.Api.SetPixel( hdc, x2, y2,(pen as DrawingPen).win32Color);
		Win32.Api.SelectObject( hdc, prevhPen );
			
	}

	private Win32.Api.POINT[] ConvertPoints( System.Drawing.Point[] points ) 
	{
		Win32.Api.POINT[] wPoints = new Win32.Api.POINT[points.Length];
		for( int i = 0; i < points.Length; i++ )
		{
			wPoints[i].x = points[i].X;
			wPoints[i].y = points[i].Y;
		}
		return wPoints;
	}

	// Draw a polygon using the current pen.
	public override void DrawPolygon( System.Drawing.Point[] points )
	{
		Polygon( points, (toolkit as DrawingToolkit).HollowBrushHandle, (pen as DrawingPen).hPen);
	}

	// Fill a polygon using the current brush.
	public override void FillPolygon( System.Drawing.Point[] points, FillMode fillMode )
	{
		Win32.Api.SetPolyFillMode(hdc, (int)fillMode);
		Polygon( points, (brush as DrawingBrush).hBrush, (toolkit as DrawingToolkit).NullPenHandle);
	}

	private void Polygon( System.Drawing.Point[] points, IntPtr hBrush, IntPtr hPen )
	{
		Win32.Api.SetBkMode(hdc, Win32.Api.BackGroundModeType.TRANSPARENT);
		IntPtr prevhPen = Win32.Api.SelectObject( hdc, hPen );
		IntPtr prevhBrush = Win32.Api.SelectObject( hdc, hBrush );
		Win32.Api.POINT[] wPoints = ConvertPoints(points);
		Win32.Api.Polygon(hdc, wPoints, wPoints.Length);
		Win32.Api.SelectObject( hdc, prevhBrush );
		Win32.Api.SelectObject( hdc, prevhPen );
		
	}

#if CONFIG_EXTENDED_NUMERICS

	// Draw an arc within a rectangle defined by four points.
	public override void DrawArc( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
	{
		Win32.Api.SetBkMode(hdc, Win32.Api.BackGroundModeType.TRANSPARENT);
		IntPtr prevhPen = Win32.Api.SelectObject( hdc, (pen as DrawingPen).hPen );
		if (sweepAngle == 360)
		{
			IntPtr prevBrush = Win32.Api.SelectObject(hdc, (toolkit as DrawingToolkit).HollowBrushHandle);
			Win32.Api.Ellipse(hdc, rect[0].X, rect[0].Y, rect[2].X + 1, rect[2].Y + 1);
			Win32.Api.SelectObject(hdc, prevBrush);
		}
		else
		{
			Rectangle intersect = EllipseIntersect( rect, startAngle, sweepAngle );
			Win32.Api.Arc( hdc, rect[0].X, rect[0].Y, rect[2].X + 1, rect[2].Y + 1, intersect.Left, intersect.Top, intersect.Right, intersect.Bottom );
		}
		Win32.Api.SelectObject( hdc, prevhPen );

	}

	// Draw a pie slice within a rectangle defined by four points.
	public override void DrawPie ( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
	{
		Pie( rect, startAngle, sweepAngle, (toolkit as DrawingToolkit).HollowBrushHandle, (pen as DrawingPen).hPen );
	}

	// Fill a pie slice within a rectangle defined by four points.
	public override void FillPie ( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
	{
		if (sweepAngle == 360)
		{
			IntPtr prevBrush = Win32.Api.SelectObject( hdc, (brush as DrawingBrush).hBrush );
			Win32.Api.SetBkMode(hdc, Win32.Api.BackGroundModeType.TRANSPARENT);
			IntPtr prevPen = Win32.Api.SelectObject(hdc, (toolkit as DrawingToolkit).NullPenHandle);
			Win32.Api.Ellipse(hdc, rect[0].X, rect[0].Y, rect[2].X + 2, rect[2].Y +2);
			Win32.Api.SelectObject( hdc, prevPen );
			Win32.Api.SelectObject( hdc, prevBrush );
		}
		else
			Pie( rect, startAngle, sweepAngle, (brush as DrawingBrush).hBrush, (toolkit as DrawingToolkit).NullPenHandle);
	}

	private void Pie( System.Drawing.Point[] rect, float startAngle, float sweepAngle, IntPtr hBrush, IntPtr hPen )
	{
		IntPtr prevBrush = Win32.Api.SelectObject( hdc, hBrush );
		Win32.Api.SetBkMode(hdc, Win32.Api.BackGroundModeType.TRANSPARENT);
		IntPtr prevPen = Win32.Api.SelectObject( hdc, hPen );
		Rectangle intersect = EllipseIntersect( rect, startAngle, sweepAngle );
					
		Win32.Api.Pie( hdc, rect[0].X, rect[0].Y, rect[2].X + 1, rect[2].Y + 1, intersect.Left, intersect.Top, intersect.Right, intersect.Bottom );
		Win32.Api.SelectObject( hdc, prevPen );
		Win32.Api.SelectObject( hdc, prevBrush );

	}

	//Top left of return rectangle is the one intersect, bottom right is the other.
	private Rectangle EllipseIntersect( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
	{
		double centerX = (rect[0].X+rect[2].X)/2;
		double centerY = (rect[0].Y+rect[2].Y)/2;
		double theta1 = (startAngle+sweepAngle)*Math.PI/180;
		double theta2 = startAngle*Math.PI/180;
		double a = (rect[2].X - rect[0].X)/2;
		double b = (rect[2].Y - rect[0].Y)/2;
		double r1 = a*b/Math.Sqrt(Math.Pow(b*Math.Cos(theta1), 2)+Math.Pow(a*Math.Sin(theta1), 2));
		double r2 = a*b/Math.Sqrt(Math.Pow(b*Math.Cos(theta2), 2)+Math.Pow(a*Math.Sin(theta2), 2));
		int p1X = (int)(r1 * Math.Cos(theta1) + 0.5 + centerX);
		int p1Y = (int)(r1 * Math.Sin(theta1) + 0.5 + centerY);
		int p2X = (int)(r2 * Math.Cos(theta2) + 0.5 + centerX);
		int p2Y = (int)(r2 * Math.Sin(theta2) + 0.5 + centerY);
		return new Rectangle(p1X, p1Y, p2X - p1X, p2Y - p1Y);
	}

#else // !CONFIG_EXTENDED_NUMERICS

	// TODO: arc routines that don't need to use the "Math" class.

	[TODO]
	// Draw an arc within a rectangle defined by four points.
	public override void DrawArc( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
		{
			return;
		}

	[TODO]
	// Draw a pie slice within a rectangle defined by four points.
	public override void DrawPie ( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
		{
			return;
		}

	[TODO]
	// Fill a pie slice within a rectangle defined by four points.
	public override void FillPie ( System.Drawing.Point[] rect, float startAngle, float sweepAngle )
		{
			return;
		}

#endif // CONFIG_EXTENDED_NUMERICS

	// Draw a string using the current font and brush.
	public override void DrawString
		(String s, int x, int y, StringFormat format)
	{
		//GDI does support writing text with a brush so we get the brush color - if available
		/*Could do but is it slow?:
				* 1) Select hatch brush
			2) BeginPath
			3) TextOut
			4) EndPath
			5) StrokeAndFillPath*/
		Win32.Api.SetTextColor(hdc, ColorToWin32((brush as ToolkitBrushBase).Color));
		Win32.Api.ExtTextOutA(hdc, x, y, 0, IntPtr.Zero, s, (uint)s.Length, IntPtr.Zero);
	}

	// Measure a string using the current font and a given layout rectangle.
	public override Size MeasureString( String s, System.Drawing.Point[] layoutRectangle, StringFormat format, out int charactersFitted, out int linesFilled, bool ascentOnly )
	{
		// TODO: line wrapping, etc
		//Win32.Api.SelectObject(hdc, selectedFont.hFont);
		Win32.Api.SIZE size;
		Win32.Api.GetTextExtentPoint32A(hdc, s, s.Length, out size);
		charactersFitted = 0;
		linesFilled = 0;
		return new Size(size.cx, size.cy); /*ascent + descent*/
	}

	// Not implementing Flush
	public override void Flush(FlushIntention intention)
	{
	}

	// Get the HDC associated with this graphics object.
	public override IntPtr GetHdc()
	{
		if(gaveHdc)
		{
			throw new InvalidOperationException(/* TODO */);
		}
		gaveHdc = true;
		return hdc;
	}

	// Release a HDC that was obtained using "GetHdc()".
	public override void ReleaseHdc(IntPtr hdc)
	{
		if(!gaveHdc)
		{
			throw new InvalidOperationException(/* TODO */);
		}
		gaveHdc = false;
	}

	// Set the clipping region to empty.
	public override void SetClipEmpty()
	{
		//Hope this is right!
		SetClipRect( 0, 0, 0, 0 );
	}

	// Set the clipping region to infinite (i.e. disable clipping).
	public override void SetClipInfinite()
	{
		Win32.Api.SelectClipRgn( hdc, IntPtr.Zero );
	}

	// Set the clipping region to a single rectangle.
	public override void SetClipRect( int x, int y, int width, int height )
	{
		IntPtr region = Win32.Api.CreateRectRgn( x, y, x + width, y + height );
		Win32.Api.SelectClipRgn( hdc, region );
		Win32.Api.DeleteObject( region );
	}

	// Set the clipping region to a list of rectangles.
	public override void SetClipRects(System.Drawing.Rectangle[] rects)
	{
		IntPtr region = DrawingToolkit.RectanglesToRegion(rects);
		Win32.Api.SelectClipRgn( hdc, region);
		Win32.Api.DeleteObject( region);
	}

	[TODO]
	// Set the clipping region to a complex mask.
	public override void SetClipMask(Object mask, int topx, int topy)
	{
	}

	// Get the line spacing for the font selected into this graphics object.
	public override int GetLineSpacing()
	{
		Win32.Api.TEXTMETRIC lptm;
		Win32.Api.GetTextMetricsA(hdc, out lptm);
		return lptm.tmHeight;
	}

	public static int ColorToWin32( Color color) 
	{
		return color.R | color.G<<8 | color.B<<16;
	}

	// Under 95/98 an object(fonts, pens or brushes) cant be disposed when it is select into a dc
	// This leads to a leak. So, if a dc is disposed, select out all the object types so when the objects are actually disposed, they will actually dispose.
	private void DeleteDC()
	{
		Win32.Api.DeleteDC(hdc);
		hdc = IntPtr.Zero;
	}

	// Write an image at x, y taking into account the mask
	public override void DrawImage(IToolkitImage image, int x, int y)
	{
		(image as DrawingImage).Draw(hdc, x, y);
	}

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	public override void DrawGlyph(int x, int y,
		byte[] xbmBits, int bitsWidth, int bitsHeight,
		System.Drawing.Color color)
	{
		DrawingImage.DrawGlyph(hdc, x, y, xbmBits, bitsWidth, bitsHeight, color);
	}

}; // class DrawingGraphics

}; // namespace System.Drawing.Toolkit
