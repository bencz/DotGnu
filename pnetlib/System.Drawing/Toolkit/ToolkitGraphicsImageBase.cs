/*
 * ToolkitGraphicsImageBase.cs - Implementation of the
 *			"System.Drawing.Toolkit.ToolkitGraphicsImageBase" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Neil Cawse.
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
	using DotGNU.Images;

	[NonStandardExtra]
	public abstract class ToolkitGraphicsImageBase : ToolkitGraphicsBase
	{
		protected ToolkitImageBase image;
		protected IToolkitBrush selectedBrush;
		protected IToolkitFont selectedFont;

		protected ToolkitGraphicsImageBase(IToolkit toolkit, IToolkitImage image)
			: base(toolkit)
				{
					this.image = image as ToolkitImageBase;
				}

		protected override void Dispose(bool disposing)
		{
		}


		[TODO]
		// Clear the entire drawing surface.
		public override void Clear(Color color)
				{
					return;
				}
	
		// Draw a line between two points using the current pen.
		// Fast version of Bresenham's algorithm.
		public override void DrawLine(int x1, int y1, int x2, int y2)
				{
					IToolkitPen pen = Pen;
					ToolkitPenBase penBase;

					if (pen == null)
						return;
					penBase = pen as ToolkitPenBase;

					int color = penBase.Color.ToArgb();
					Frame frame = image.image.GetFrame(0);
					// TODO: Finish off
					int dy = y2 - y1;
					int dx = x2 - x1;
					int stepx, stepy;

					if (dy < 0)
					{
						dy = -dy;
						stepy = -frame.Stride;
					} 
					else
						stepy = frame.Stride;
					if (dx < 0)
					{
						dx = -dx;
						stepx = -1;
					} 
					else
					{
						stepx = 1;
					}
					dy <<= 1;
					dx <<= 1;

					y1 *= frame.Stride;
					y2 *= frame.Stride;
					SetPixelLine(frame, x1,y1, color);
					if (dx > dy) 
					{
						int fraction = dy - (dx >> 1);
						while (x1 != x2) 
						{
							if (fraction >= 0) 
							{
								y1 += stepy;
								fraction -= dx;
							}
							x1 += stepx;
							fraction += dy;
							SetPixelLine(frame, x1,y1, color);
						}
					} 
					else 
					{
						int fraction = dx - (dy >> 1);
						while (y1 != y2) 
						{
							if (fraction >= 0) 
							{
								x1 += stepx;
								fraction -= dy;
							}
							y1 += stepy;
							fraction += dx;
							SetPixelLine(frame, x1,y1, color);
						}
					}
					this.image.ImageChanged();
				}

		private void SetPixelLine(Frame frame, int x, int yPtr, int color)
				{
					switch (frame.PixelFormat)
					{
						case (PixelFormat.Format24bppRgb):
						{
							int ptr = yPtr + x * 3;
							if(ptr >= 0 && frame.Data.Length > ptr + 2)
							{
								frame.Data[ptr++] = (byte)color;
								frame.Data[ptr++] = (byte)(color>>8);
								frame.Data[ptr] = (byte)(color>>16);
							}
							break;
						}
						default:
							throw new NotSupportedException();
					}
				}

		[TODO]
		// Draw a polygon using the current pen.
		public override void DrawPolygon(Point[] points)
				{
					return;
				}

		[TODO]
		// Fill a polygon using the current brush.
		public override void FillPolygon(Point[] points, FillMode fillMode)
				{
					return;
				}

		[TODO]
		// Draw an arc within a rectangle defined by four points.
		public override void DrawArc (Point[] rect, float startAngle, float sweepAngle)
				{
					// f(x,y) = Ax^2 + Bxy + Cy^2 + Dx + Ey + F
					// Ellipse B^2 - 4AC<0, circle if A=C and B=0
						/*final static int OCTANTS = 0x12650374;
				public void conic(
					int x0, int y0,               // starting point
					int x1, int y1,               // ending point
					Color color,                  // color of curve
					float A, float B, float C,    // coefficients of conic
					float D, float E, float F)
			     
					int pix = color.getRGB();
					int dxDiag, dyDiag, dxLine, dyLine;
					int octant = 0;
					float d, u, v;

					F = 0;
					D = 2*A*x0 + B*y0 + D;
					E = B*x0 + 2*C*y0 + E;

					if (D >= 0) {
						dxDiag = dxLine = 1;
						octant += 1;
					} else {
						dxDiag = dxLine = -1;
					}

					if (E >= 0) {
						dyDiag = dyLine = 1;
						octant += 2;
					} else {
						dyDiag = dyLine = -1;
					}

					if (Math.abs(E) > Math.abs(D)) {
						dxLine = 0;
						u = dxDiag*dyDiag*B/2 + C + dyDiag*E;
						d = u + A/4 + dxDiag*D/2 + F;
						v = u + dxDiag*D;
						octant += 4;
					} else {
						dyLine = 0;
						u = dxDiag*dyDiag*B/2 + A + dxDiag*D;
						d = u + C/4 + dyDiag*E/2 + F;
						v = u + dyDiag*E;
					}
					octant = (OCTANTS >> (4*octant)) & 7;

					float k1 = 2*(A + dyLine*dyDiag*(C - A));
					float k2 = dxDiag*dyDiag*B;
					float k3 = 2*(A + C + k2);
					k2 += k1;

			loop:   do {
						if ((octant & 1) == 0) {
							while (2*v <= k2) {
								raster.setPixel(pix, x0, y0);
								if ((x0 == x1) & (y0 == y1)) break loop;
								if (d < 0) {
									x0 += dxLine;
									y0 += dyLine;
									u += k1;
									v += k2;
									d += u;
								} else {
									x0 += dxDiag;
									y0 += dyDiag;
									u += k2;
									v += k3;
									d += v;
								}
							}
							d = d - u + v/2 - k2/2 + 3*k3/8;
							u = -u + v - k2/2 + k3/2;
							v = v - k2 + k3/2;
							k1 = k1 - 2*k2 + k3;
							k2 = k3 - k2;
							int t = dxLine; dxLine = -dyLine; dyLine = t;
						} else {
							while (2*u < k2) {
								raster.setPixel(pix, x0, y0);
								if ((x0 == x1) & (y0 == y1)) break loop;
								if (d > 0) {
									x0 += dxLine;
									y0 += dyLine;
									u += k1;
									v += k2;
									d += u;
								} else {
									x0 += dxDiag;
									y0 += dyDiag;
									u += k2;
									v += k3;
									d += v;
								}
							}
							float dk = k1 - k2;
							d = d + u - v + dk;
							v = 2*u - v + dk;
							u = u + dk;
							k3 = k3 + 4*dk;
							k2 = k1 + dk;
							int t = dxDiag; dxDiag = -dyDiag; dyDiag = t;
						}
						octant = (octant + 1) & 7;
					} while (true);*/
				}

		[TODO]
		// Draw a pie slice within a rectangle defined by four points.
		public override void DrawPie (Point[] rect, float startAngle, float sweepAngle)
				{
					return;
				}

		[TODO]
		// Fill a pie slice within a rectangle defined by four points.
		public override void FillPie (Point[] rect, float startAngle, float sweepAngle)
				{
					return;
				}

		[TODO]
		// Set the clipping region to empty.
		public override void SetClipEmpty()
				{
					return;
				}

		[TODO]
		// Set the clipping region to infinite (i.e. disable clipping).
		public override void SetClipInfinite()
				{
					return;
				}

		[TODO]
		// Set the clipping region to a single rectangle.
		public override void SetClipRect(int x, int y, int width, int height)
				{
					return;
				}

		[TODO]
		// Set the clipping region to a list of rectangles.
		public override void SetClipRects(Rectangle[] rects)
				{
					return;
				}

		[TODO]
		// Set the clipping region to a complex mask.
		public override void SetClipMask(Object mask, int topx, int topy)
				{
					return;
				}

		// Copy image into this.image at x, y
		public override void DrawImage(IToolkitImage image, int x, int y)
				{
					DotGNU.Images.Image sourceImage = (image as ToolkitImageBase).image;
					Frame sourceFrame = sourceImage.GetFrame(0);
					DotGNU.Images.Image destImage = this.image.image;
					Frame destFrame =destImage.GetFrame(0);
					if (sourceFrame.PixelFormat != destFrame.PixelFormat)
						sourceFrame = sourceFrame.Reformat(destImage.PixelFormat);
					destFrame.Copy(sourceFrame, x, y);
					this.image.ImageChanged();
				}

		[TODO]
		// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
		// in the form of an xbm bitmap.
		public override void DrawGlyph
					(int x, int y,
					 byte[] bits, int bitsWidth, int bitsHeight, Color color)
				{
					return;
				}

	}
}
