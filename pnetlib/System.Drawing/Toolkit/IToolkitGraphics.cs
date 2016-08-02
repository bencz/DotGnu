/*
 * IToolkitGraphics.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitGraphics" class.
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

using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

[NonStandardExtra]
public interface IToolkitGraphics : IDisposable
{
	// Get or set the graphics object's properties.
	IToolkit Toolkit { get; }
	CompositingMode CompositingMode { get; set; }
	CompositingQuality CompositingQuality { get; set; }
	float DpiX { get; }
	float DpiY { get; }
	InterpolationMode InterpolationMode { get; set; }
	PixelOffsetMode PixelOffsetMode { get; set; }
	Point RenderingOrigin { get; set; }
	SmoothingMode SmoothingMode { get; set; }
	int TextContrast { get; set; }
	TextRenderingHint TextRenderingHint { get; set; }

	// Clear the entire drawing surface.
	void Clear(Color color);

	// Draw a line between two points using the current pen.
	void DrawLine(int x1, int y1, int x2, int y2);

	// Draw a set of connected line seguments using the current pen.
	void DrawLines(Point[] points);

	// Draw a polygon using the current pen.
	void DrawPolygon(Point[] points);

	// Fill a polygon using the current brush.
	void FillPolygon(Point[] points, FillMode fillMode);

	// Draw a bezier curve using the current pen.
	void DrawBezier(int x1, int y1, int x2, int y2,
					int x3, int y3, int x4, int y4);

	// Draw a bezier curve using the current pen.
	void FillBezier(int x1, int y1, int x2, int y2,
									int x3, int y3, int x4, int y4, FillMode fillMode);
	
	// Draw an arc within a rectangle defined by four points.
	void DrawArc(Point[] rect, float startAngle, float sweepAngle);

	// Draw a pie slice within a rectangle defined by four points.
	void DrawPie(Point[] rect, float startAngle, float sweepAngle);

	// Fill a pie slice within a rectangle defined by four points.
	void FillPie(Point[] rect, float startAngle, float sweepAngle);

	// Draw a closed cardinal curve using the current pen.
	void DrawClosedCurve(Point[] points, float tension);

	// Fill a closed cardinal curve using the current brush.
	void FillClosedCurve(Point[] points, float tension, FillMode fillMode);

	// Draw a cardinal curve using the current pen.
	void DrawCurve(Point[] points, int offset,
				   int numberOfSegments, float tension);

	// Draw a string using the current font and brush.
	void DrawString(String s, int x, int y, StringFormat format);

	// Draw a string using the current font and brush within a
	// layout rectangle that is defined by four points.
	void DrawString(String s, Point[] layoutRectangle, StringFormat format);

	// Measure a string using the current font and a given layout rectangle.
	Size MeasureString(String s, Point[] layoutRectangle,
					   StringFormat format, out int charactersFitted,
					   out int linesFilled, bool ascentOnly);

	// Flush the graphics subsystem
	void Flush(FlushIntention intention);

	// Get the nearest color to a specified one.
	Color GetNearestColor(Color color);

	// Add a metafile comment.
	void AddMetafileComment(byte[] data);

	// Get the HDC associated with this graphics object.
	IntPtr GetHdc();

	// Release a HDC that was obtained using "GetHdc()".
	void ReleaseHdc(IntPtr hdc);

	// Set the clipping region to empty.
	void SetClipEmpty();

	// Set the clipping region to infinite (i.e. disable clipping).
	void SetClipInfinite();

	// Set the clipping region to a single rectangle.
	void SetClipRect(int x, int y, int width, int height);

	// Set the clipping region to a list of rectangles.
	void SetClipRects(Rectangle[] rects);

	// Set the clipping region to a complex mask.  TODO
	void SetClipMask(Object mask, int topx, int topy);

	// Get the line spacing for the font selected into this graphics object.
	int GetLineSpacing();

	// Draw the image at point x, y.
	void DrawImage(IToolkitImage image, int x, int y);

	// Draw the source parallelogram of an image into the parallelogram
	// defined by dest. Point[] has 3 Points, Top-Left, Top-Right and Bottom-Left.
	// The remaining point is inferred.
	void DrawImage(IToolkitImage image,Point[] src, Point[] dest);

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	void DrawGlyph(int x, int y,
				   byte[] bits, int bitsWidth, int bitsHeight,
				   Color color);

}; // interface IToolkitGraphics

}; // namespace System.Drawing.Toolkit
