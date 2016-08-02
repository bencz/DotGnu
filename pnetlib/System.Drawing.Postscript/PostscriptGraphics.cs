/*
 * PostscriptGraphics.cs - Implementation of the
 *			"System.Drawing.Toolkit.PostscriptGraphics" class.
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

using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Text;

internal class PostscriptGraphics : ToolkitGraphicsBase
{
	// Internal state.
	internal TextWriter writer;
	private PostscriptPrintSession session;
	internal IToolkitSelectObject selectObject;

	private const uint Power85To1 = 85; // 85^1
	private const uint Power85To2 = 7225; // 85^2
	private const uint Power85To3 = 614125; // 85^3
	private const uint Power85To4 = 52200625; // 85^4

	// conversion table for reversing bit order in bytes
	private static byte[] reverseByte =
	{
		0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0,
		0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0,
		0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8,
		0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8,
		0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4,
		0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4,
		0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC,
		0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC,
		0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2,
		0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
		0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA,
		0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
		0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6,
		0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6,
		0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE,
		0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
		0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1,
		0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
		0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9,
		0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
		0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5,
		0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
		0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED,
		0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
		0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3,
		0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3,
		0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB,
		0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
		0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7,
		0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
		0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF,
		0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
	};

	// Constructor.
	public PostscriptGraphics(IToolkit toolkit, TextWriter writer,
							  PostscriptPrintSession session)
			: base(toolkit)
			{
				this.writer = writer;
				this.session = session;
				this.selectObject = null;
			}

	// Get or set the graphics object's properties.
	public override float DpiX
			{
				get
				{
					return 300.0f;
				}
			}
	public override float DpiY
			{
				get
				{
					return 300.0f;
				}
			}

	// Clear the entire drawing surface.
	public override void Clear(Color color)
			{
				// We assume that the page is already "clear" when
				// we start drawing it, so nothing to do here.
			}

	// Draw a line between two points using the current pen.
	public override void DrawLine(int x1, int y1, int x2, int y2)
			{
				writer.WriteLine("{0} {1} moveto {2} {3} lineto stroke",
								 x1, y1, x2, y2);
			}

	// Draw a set of connected line seguments using the current pen.
	public override void DrawLines(Point[] points)
			{
				int index;
				writer.Write("{0} {1} moveto ", points[0].X, points[0].Y);
				for(index = 1; index < points.Length; ++index)
				{
					writer.Write("{0} {1} lineto ",
								 points[index].X, points[index].Y);
				}
				writer.WriteLine("stroke");
			}

	// Draw a polygon using the current pen.
	public override void DrawPolygon(Point[] points)
			{
				int index;
				writer.Write("{0} {1} moveto ", points[0].X, points[0].Y);
				for(index = 1; index < points.Length; ++index)
				{
					writer.Write("{0} {1} lineto ",
								 points[index].X, points[index].Y);
				}
				writer.WriteLine("closepath stroke");
			}

	// Fill a polygon using the current brush.
	public override void FillPolygon(Point[] points, FillMode fillMode)
			{
				int index;
				writer.Write("{0} {1} moveto ", points[0].X, points[0].Y);
				for(index = 1; index < points.Length; ++index)
				{
					writer.Write("{0} {1} lineto ",
								 points[index].X, points[index].Y);
				}
				writer.WriteLine("closepath fill");
			}

	// Draw an arc within a rectangle defined by four points.
	public override void DrawArc
				(Point[] rect, float startAngle, float sweepAngle)
			{
				int x, y, width, height;
				ConvertPoints(rect, out x, out y, out width, out height);
				float halfWidth = (width / 2.0f);
				float halfHeight = (height / 2.0f);

				writer.WriteLine
					("gsave " +
					 "matrix currentmatrix " +
					 "newpath " +
					 "{0} {1} translate " +
					 "{2} {3} scale " +
					 "0 0 1 {4} {5} arcn " +
					 "setmatrix " +
					 "stroke " +
					 "grestore",
					 x+halfWidth, y+halfHeight,
					 halfWidth, halfHeight,
					 startAngle, sweepAngle);
			}

	// Draw a pie slice within a rectangle defined by four points.
	public override void DrawPie
				(Point[] rect, float startAngle, float sweepAngle)
			{
				int x, y, width, height;
				ConvertPoints(rect, out x, out y, out width, out height);
				float halfWidth = (width / 2.0f);
				float halfHeight = (height / 2.0f);

				writer.WriteLine
					("gsave " +
					 "matrix currentmatrix " +
					 "newpath " +
					 "{0} {1} translate " +
					 "{2} {3} scale " +
					 "0 0 moveto " +
					 "0 0 1 {4} {5} arcn " +
					 "closepath " +
					 "setmatrix " +
					 "stroke " +
					 "grestore",
					 x+halfWidth, y+halfHeight,
					 halfWidth, halfHeight,
					 startAngle, sweepAngle);
			}

	// Fill a pie slice within a rectangle defined by four points.
	public override void FillPie
				(Point[] rect, float startAngle, float sweepAngle)
			{
				int x, y, width, height;
				ConvertPoints(rect, out x, out y, out width, out height);
				float halfWidth = (width / 2.0f);
				float halfHeight = (height / 2.0f);

				writer.WriteLine
					("gsave " +
					 "matrix currentmatrix " +
					 "newpath " +
					 "{0} {1} translate " +
					 "{2} {3} scale " +
					 "0 0 moveto " +
					 "0 0 1 {4} {5} arcn " +
					 "closepath " +
					 "setmatrix " +
					 "fill " +
					 "grestore",
					 x+halfWidth, y+halfHeight,
					 halfWidth, halfHeight,
					 startAngle, sweepAngle);
			}

	[TODO]
	// Draw a string using the current font and brush.
	public override void DrawString
				(String s, int x, int y, StringFormat format)
			{
				return;
			}

	[TODO]
	// Draw a string using the current font and brush within a
	// layout rectangle that is defined by four points.
	public override void DrawString
				(String s, Point[] layoutRectangle, StringFormat format)
			{
				return;
				
			}

	[TODO]
	// Measure a string using the current font and a given layout rectangle.
	public override Size MeasureString
				(String s, Point[] layoutRectangle,
				 StringFormat format, out int charactersFitted,
				 out int linesFilled, bool ascentOnly)
			{
				charactersFitted = 0;
				linesFilled = 0;
				ascentOnly = false;
				return new Size(0, 0);
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

	[TODO]
	// Get the line spacing for the font selected into this graphics object.
	public override int GetLineSpacing()
			{
				return 0;
			}

	// Draw an image at the coordinates
	public override void DrawImage(IToolkitImage image, int x, int y)
			{
				PostscriptImage psi = (image as PostscriptImage);
				String psImage;
				if(PostscriptLevel == 3)
				{
					psImage = psi.LevelThree;
				}
				else
				{
					psImage = psi.LevelTwo;
				}
				writer.WriteLine
					("matrix currentmatrix " +
					 "gsave " +
					 "newpath " +
					 "{0} {1} translate " +
					 "0 0 moveto " +
					 "{2} " +
					 "grestore " +
					 "setmatrix",
					 x, y, psImage);
			}

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	public override void DrawGlyph
				(int x, int y, byte[] bits, int bitsWidth,
				 int bitsHeight, System.Drawing.Color color)
			{
				StringBuilder sb = new StringBuilder(bits.Length*2);
				byte b;
				for(int i = 0; i < bits.Length; ++i)
				{
					b = reverseByte[bits[i]];
					sb.Append(String.Format(b.ToString("x2")));
				}
				writer.WriteLine
					("matrix currentmatrix " +
					 "gsave " +
					 "{0} {1} {2} setrgbcolor " +
					 "newpath " +
					 "{3} {4} translate " +
					 "0 0 moveto " +
					 "<< " +
					 "/ImageType 1 " +
					 "/Width {5} " +
					 "/Height {6} " +
					 "/ImageMatrix [1 0 0 -1 0 {6}] " +
					 "/DataSource {{ currentfile 2 string readhexstring pop }} " +
					 "/BitsPerComponent 1 " +
					 "/Decode [1 0] " +
					 "/Interpolation true " +
					 ">> imagemask " +
					 "{7} " +
					 "grestore " +
					 "setmatrix",
					 ((double)(color.R)) / 255.0,
					 ((double)(color.G)) / 255.0,
					 ((double)(color.B)) / 255.0,
					 x, y, bitsWidth, bitsHeight,
					 sb.ToString());
			}

	protected override void Dispose(bool disposing)
			{
				// Nothing to do.
			}

	private static void ConvertPoints
				(Point[] rect,
				 out int x, out int y,
				 out int width, out int height)
			{
				x = rect[0].X;
				y = rect[0].Y;
				width = rect[2].X - x;
				height = rect[2].Y - y;
			}

	// Get the current LanguageLevel for postscript output.
	public static int PostscriptLevel
			{
				get
				{
					String s = Environment.GetEnvironmentVariable
						("PNET_POSTSCRIPT_LEVEL");
					if(s == null) { return 2; }
					try
					{
						int level = Int32.Parse(s);
						if(level == 3) { return 3; }
						else { return 2; }
					}
					catch(FormatException)
					{
						return 2;
					}
				}
			}

	// Get the ASCII85 encoding of an array of bytes.
	public static String ASCII85Encode(byte[] b)
			{
				int blen = b.Length;
				// 4:5 data ratio plus 4:5 newlines ratio plus 2 for '~>'
				int clen = (((blen+3)/4)*5) + ((blen+63)/64) + 2;
				char[] c = new char[clen];
				int bptr = 0;
				int cptr = 0;
				int lastline = 0;
				while(bptr < blen)
				{
					int trim = 0;
					uint bits = (b[bptr++]);
					for(int k = 0; k < 4; ++k)
					{
						bits = bits << 8;
						if(bptr < blen) { bits |= b[bptr++]; }
						else { ++trim; }
					}
					if(bits == 0 && trim == 0)
					{
						c[cptr++] = 'z';
					}
					else
					{
						uint tmp = bits / Power85To4;
						bits -= tmp * Power85To4;
						c[cptr++] = (char)(tmp + '!');

						tmp = bits / Power85To3;
						bits -= tmp * Power85To3;
						c[cptr++] = (char)(tmp + '!');

						tmp = bits / Power85To2;
						bits -= tmp * Power85To2;
						c[cptr++] = (char)(tmp + '!');

						tmp = bits / Power85To1;
						c[cptr++] = (char)(tmp + '!');

						tmp = bits % 85;
						c[cptr++] = (char)(tmp + '!');

						cptr -= trim;
					}
					if((cptr - lastline) > 76)
					{
						lastline = cptr;
						c[cptr++] = '\n';
					}
				}
				c[cptr++] = '~';
				c[cptr++] = '>';
				return new String(c, 0, cptr);
			}

}; // class PostscriptGraphics

}; // namespace System.Drawing.Toolkit
