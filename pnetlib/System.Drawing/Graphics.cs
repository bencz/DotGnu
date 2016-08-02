/*
 * Graphics.cs - Implementation of the "System.Drawing.Graphics" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Free Software Foundation, Inc.
 * Contributions from Thomas Fritzsche <tf@noto.de>.
 * Contributions from Neil Cawse.
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

namespace System.Drawing
{

using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Toolkit;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class Graphics : MarshalByRefObject, IDisposable
{
	// Internal state.
	// static members
	internal static Graphics defaultGraphics;

	// instance members
	private IToolkitGraphics graphics;
	private TextLayoutManager textLayoutManager;
	private Region clip;
	internal Matrix transform;
	private float pageScale;
	private float dpiX;
	private float dpiY;
	private GraphicsUnit pageUnit;
	internal GraphicsContainer stackTop;
	// The window this graphics represents overlying the IToolkitGraphics
	internal Rectangle baseWindow;
	// The current clip in device coordinates - useful when deciding if something
	// is out of bounds, limiting the need to call the toolkit.
	private Rectangle deviceClipExtent;

	// Default DPI for the screen.
	internal const float DefaultScreenDpi = 96.0f;

	// Static constructor
	static Graphics()
			{
				defaultGraphics = new Graphics
							(ToolkitManager.Toolkit.GetDefaultGraphics());
				defaultGraphics.PageUnit = GraphicsUnit.Pixel;
			}

	// Constructor.
	internal Graphics(IToolkitGraphics graphics)
			{
				this.graphics = graphics;
				this.clip = null;
				this.transform = null;
				this.pageScale = 1.0f;
				this.pageUnit = GraphicsUnit.World;
				this.stackTop = null;
				this.baseWindow = Rectangle.Empty;
				if(graphics != null)
				{
					dpiX = graphics.DpiX;
					dpiY = graphics.DpiY;
				}
				else
				{
					dpiX = DefaultScreenDpi;
					dpiY = DefaultScreenDpi;
				}
			}

	// Window Constructor. Copies the existing Graphics and creates a new
	// Graphics that has an origin of baseWindow.Location and is always clipped
	// to baseWindow

	internal Graphics(IToolkitGraphics graphics, Rectangle baseWindow)
		: this(graphics)
			{
				this.baseWindow = baseWindow;
				clip = new Region(baseWindow);
				clip.Translate(-baseWindow.X, -baseWindow.Y);
				Clip = clip;
			}

	// Create a Graphics that is internally offset to baseWindow
	internal Graphics(Graphics graphics, Rectangle baseWindow)
			{
				// Use the same toolkit
				this.graphics = graphics.graphics;
				dpiX = graphics.DpiX;
				dpiY = graphics.DpiY;
				if (graphics.clip != null)
				{
					clip = graphics.clip.Clone();
					clip.Intersect(baseWindow);
				}
				else
					clip = new Region(baseWindow);
				// Find out what the clip is with our new Origin
				clip.Translate(-baseWindow.X, -baseWindow.Y);
				this.baseWindow = baseWindow;
				if (graphics.transform != null)
					this.transform = new Matrix(graphics.transform);
				this.pageScale = graphics.pageScale;
				this.pageUnit = graphics.pageUnit;
				this.stackTop = null;
				Clip = clip;
			}


	// Destructor.
	~Graphics()
			{
				Dispose(false);
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	private void Dispose(bool disposing)
			{
				lock(this)
				{
					if(graphics != null)
					{
						graphics.Dispose();
						graphics = null;
						dpiX = DefaultScreenDpi;
						dpiY = DefaultScreenDpi;
					}
					if (clip != null)
					{
						clip.Dispose();
						clip = null;
					}
				}
			}

	// Get or set this object's properties.
	public Region Clip
			{
				get
				{
					if(clip == null)
					{
						clip = new Region();
					}
					return clip;
				}
				set
				{
					SetClip(value, CombineMode.Replace);
				}
			}
	public RectangleF ClipBounds
			{
				get
				{
					return Clip.GetBounds(this);
				}
			}
	public CompositingMode CompositingMode
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.CompositingMode;
						}
						else
						{
							return CompositingMode.SourceOver;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.CompositingMode = value;
						}
					}
				}
			}
	public CompositingQuality CompositingQuality
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.CompositingQuality;
						}
						else
						{
							return CompositingQuality.Default;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.CompositingQuality = value;
						}
					}
				}
			}
	public float DpiX
			{
				get
				{
					return dpiX;
				}
			}
	public float DpiY
			{
				get
				{
					return dpiY;
				}
			}
	public InterpolationMode InterpolationMode
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.InterpolationMode;
						}
						else
						{
							return InterpolationMode.Default;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.InterpolationMode = value;
						}
					}
				}
			}
	public bool IsClipEmpty
			{
				get
				{
					return (clip != null && clip.IsEmpty(this));
				}
			}
	public bool IsVisibleClipEmpty
			{
				get
				{
					RectangleF clip = VisibleClipBounds;
					return (clip.Width <= 0.0f && clip.Height <= 0.0f);
				}
			}
	public float PageScale
			{
				get
				{
					return pageScale;
				}
				set
				{
					pageScale = value;
				}
			}
	public GraphicsUnit PageUnit
			{
				get
				{
					return pageUnit;
				}
				set
				{
					pageUnit = value;
				}
			}
	public PixelOffsetMode PixelOffsetMode
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.PixelOffsetMode;
						}
						else
						{
							return PixelOffsetMode.Default;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.PixelOffsetMode = value;
						}
					}
				}
			}
	public Point RenderingOrigin
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.RenderingOrigin;
						}
						else
						{
							return new Point(0, 0);
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.RenderingOrigin = value;
						}
					}
				}
			}
	public SmoothingMode SmoothingMode
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.SmoothingMode;
						}
						else
						{
							return SmoothingMode.Default;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.SmoothingMode = value;
						}
					}
				}
			}
	public int TextContrast
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.TextContrast;
						}
						else
						{
							return 4;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.TextContrast = value;
						}
					}
				}
			}
	public TextRenderingHint TextRenderingHint
			{
				get
				{
					lock(this)
					{
						if(graphics != null)
						{
							return graphics.TextRenderingHint;
						}
						else
						{
							return TextRenderingHint.SystemDefault;
						}
					}
				}
				set
				{
					lock(this)
					{
						if(graphics != null)
						{
							graphics.TextRenderingHint = value;
						}
					}
				}
			}
	public Matrix Transform
			{
				get
				{
					lock(this)
					{
						if(transform == null)
						{
							transform = new Matrix();
						}
						// return a copy instead of the original
						return new Matrix(transform);
					}
				}
				set
				{
					lock(this)
					{
						if(value == null || value.IsIdentity)
						{
							transform = null;
						}
						else
						{
							// Copy the given Matric, do not work on it directly
							transform = new Matrix(value);
						}
					}
				}
			}
	public RectangleF VisibleClipBounds
			{
				get
				{
					if(graphics != null)
					{
						PointF bottomRight = new PointF(baseWindow.Width, baseWindow.Height);
						PointF[] coords = new PointF[] {PointF.Empty, bottomRight };
						TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, coords);
						return RectangleF.FromLTRB(coords[0].X, coords[0].Y, coords[1].X, coords[1].Y);
					}
					else
					{
						return RectangleF.Empty;
					}
				}
			}

	// Add a metafile comment.
	public void AddMetafileComment(byte[] data)
			{
				lock(this)
				{
					if(graphics != null)
					{
						graphics.AddMetafileComment(data);
					}
				}
			}

	// Save the current contents of the graphics context in a container.
	public GraphicsContainer BeginContainer()
			{
				lock(this)
				{
					return new GraphicsContainer(this);
				}
			}
	public GraphicsContainer BeginContainer(Rectangle dstRect,
											Rectangle srcRect,
											GraphicsUnit unit)
			{
				return BeginContainer((RectangleF)dstRect,
									  (RectangleF)srcRect, unit);
			}
	public GraphicsContainer BeginContainer(RectangleF dstRect,
											RectangleF srcRect,
											GraphicsUnit unit)
			{
				// Save the current state of the context.
				GraphicsContainer container = BeginContainer();

				// Modify the context information appropriately.
				// TODO

				// Return the saved information to the caller.
				return container;
			}

	// Clear the entire drawing surface.
	public void Clear(Color color)
			{
				using (Brush brush = new SolidBrush(color))
				{
					FillRectangle(brush, ClipBounds);
				}
			}

	// Draw an arc.
	public void DrawArc(Pen pen, Rectangle rect,
						float startAngle, float sweepAngle)
			{
				DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height,
						startAngle, sweepAngle);
			}
	public void DrawArc(Pen pen, RectangleF rect,
						float startAngle, float sweepAngle)
			{
				DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height,
						startAngle, sweepAngle);
			}
	public void DrawArc(Pen pen, int x, int y, int width, int height,
						float startAngle, float sweepAngle)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}
				
				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddArc( x,y,width,height,startAngle,sweepAngle);
					DrawPath( pen, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectPen(pen);
						ToolkitGraphics.DrawArc(rect, startAngle, sweepAngle);
					}
				}
			}
	public void DrawArc(Pen pen, float x, float y, float width, float height,
						float startAngle, float sweepAngle)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddArc( x,y,width,height,startAngle,sweepAngle);
					DrawPath( pen, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectPen(pen);
						ToolkitGraphics.DrawArc(rect, startAngle, sweepAngle);
					}
				}
			}

	// Draw a Bezier spline.
	public void DrawBezier(Pen pen, Point pt1, Point pt2,
						   Point pt3, Point pt4)
			{
				DrawBezier(pen, (float)(pt1.X), (float)(pt1.Y),
						   (float)(pt2.X), (float)(pt2.Y),
						   (float)(pt3.X), (float)(pt3.Y),
						   (float)(pt4.X), (float)(pt4.Y));
			}
	public void DrawBezier(Pen pen, PointF pt1, PointF pt2,
						   PointF pt3, PointF pt4)
			{
				DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y,
						   pt3.X, pt3.Y, pt4.X, pt4.Y);
			}
	public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2,
						   float x3, float y3, float x4, float y4)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				int dx1, dy1, dx2, dy2;
				int dx3, dy3, dx4, dy4;
				ConvertPoint(x1 + baseWindow.X, y1 + baseWindow.Y, out dx1, out dy1, pageUnit);
				ConvertPoint(x2 + baseWindow.X, y2 + baseWindow.Y, out dx2, out dy2, pageUnit);
				ConvertPoint(x3 + baseWindow.X, y3 + baseWindow.Y, out dx3, out dy3, pageUnit);
				ConvertPoint(x4 + baseWindow.X, y4 + baseWindow.Y, out dx4, out dy4, pageUnit);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawBezier(dx1, dy1, dx2, dy2,
											   dx3, dy3, dx4, dy4);
				}
			}

	// Draw a Bezier spline.
	public void FillBezier(Brush brush, Point pt1, Point pt2,
	                       Point pt3, Point pt4, FillMode fillMode)
			{
				FillBezier(brush, (float)(pt1.X), (float)(pt1.Y),
									 (float)(pt2.X), (float)(pt2.Y),
									 (float)(pt3.X), (float)(pt3.Y),
									 (float)(pt4.X), (float)(pt4.Y), fillMode);
			}
	public void FillBezier(Brush brush, PointF pt1, PointF pt2,
	                       PointF pt3, PointF pt4, FillMode fillMode)
			{
				FillBezier(brush, pt1.X, pt1.Y, pt2.X, pt2.Y,
									 pt3.X, pt3.Y, pt4.X, pt4.Y, fillMode);
			}
	public void FillBezier(Brush brush, float x1, float y1, float x2, float y2,
	                       float x3, float y3, float x4, float y4, FillMode fillMode)
			{

				int dx1, dy1, dx2, dy2;
				int dx3, dy3, dx4, dy4;
				ConvertPoint(x1 + baseWindow.X, y1 + baseWindow.Y, out dx1, out dy1, pageUnit);
				ConvertPoint(x2 + baseWindow.X, y2 + baseWindow.Y, out dx2, out dy2, pageUnit);
				ConvertPoint(x3 + baseWindow.X, y3 + baseWindow.Y, out dx3, out dy3, pageUnit);
				ConvertPoint(x4 + baseWindow.X, y4 + baseWindow.Y, out dx4, out dy4, pageUnit);
				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.DrawBezier(dx1, dy1, dx2, dy2,
																		 dx3, dy3, dx4, dy4);
					ToolkitGraphics.FillBezier(dx1, dy1, dx2, dy2,
																		 dx3, dy3, dx4, dy4, fillMode);
				}
			}

			
	// Draw a series of Bezier splines.
	public void DrawBeziers(Pen pen, Point[] points)
			{
				if(pen == null)
				{
					throw new ArgumentNullException("pen");
				}
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				int posn = 0;
				while((posn + 4) <= points.Length)
				{
					DrawBezier(pen, points[posn], points[posn + 1],
							   points[posn + 2], points[posn + 3]);
					posn += 3;
				}
			}
	public void DrawBeziers(Pen pen, PointF[] points)
			{
				if(pen == null)
				{
					throw new ArgumentNullException("pen");
				}
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				int posn = 0;
				while((posn + 4) <= points.Length)
				{
					DrawBezier(pen, points[posn], points[posn + 1],
							   points[posn + 2], points[posn + 3]);
					posn += 3;
				}
			}

	// Draw a closed cardinal spline.
	public void DrawClosedCurve(Pen pen, Point[] points)
			{
				DrawClosedCurve(pen, points, 0.5f, FillMode.Alternate);
			}
	public void DrawClosedCurve(Pen pen, PointF[] points)
			{
				DrawClosedCurve(pen, points, 0.5f, FillMode.Alternate);
			}
	public void DrawClosedCurve(Pen pen, Point[] points,
								float tension, FillMode fillMode)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				points = ConvertPoints(points, 4, pageUnit);
				BaseOffsetPoints(points);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawClosedCurve(points, tension);
				}
			}
	public void DrawClosedCurve(Pen pen, PointF[] points,
								float tension, FillMode fillMode)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				Point[] dpoints = ConvertPoints(points, 4, pageUnit);
				BaseOffsetPoints(dpoints);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawClosedCurve(dpoints, tension);
				}
			}

	// Draw a cardinal spline.
	public void DrawCurve(Pen pen, Point[] points)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				DrawCurve(pen, points, 0, points.Length - 1, 0.5f);
			}
	public void DrawCurve(Pen pen, PointF[] points)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				DrawCurve(pen, points, 0, points.Length - 1, 0.5f);
			}
	public void DrawCurve(Pen pen, Point[] points, float tension)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				DrawCurve(pen, points, 0, points.Length - 1, tension);
			}
	public void DrawCurve(Pen pen, PointF[] points, float tension)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				DrawCurve(pen, points, 0, points.Length - 1, tension);
			}
	public void DrawCurve(Pen pen, Point[] points,
						  int offset, int numberOfSegments)
			{
				DrawCurve(pen, points, offset, numberOfSegments, 0.5f);
			}
	public void DrawCurve(Pen pen, PointF[] points,
						  int offset, int numberOfSegments)
			{
				DrawCurve(pen, points, offset, numberOfSegments, 0.5f);
			}
	public void DrawCurve(Pen pen, Point[] points,
						  int offset, int numberOfSegments, float tension)
			{
				points = ConvertPoints(points, 4, pageUnit);
				BaseOffsetPoints(points);
				if(offset < 0 || offset >= (points.Length - 1))
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("Arg_InvalidCurveOffset"));
				}
				if(numberOfSegments < 1 ||
				   (offset + numberOfSegments) >= points.Length)
				{
					throw new ArgumentOutOfRangeException
						("numberOfSegments", S._("Arg_InvalidCurveSegments"));
				}

				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}


				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawCurve
						(points, offset, numberOfSegments, tension);
				}
			}
	public void DrawCurve(Pen pen, PointF[] points,
						  int offset, int numberOfSegments, float tension)
			{
				Point[] dpoints = ConvertPoints(points, 4, pageUnit);
				BaseOffsetPoints(dpoints);
				if(offset < 0 || offset >= (points.Length - 1))
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("Arg_InvalidCurveOffset"));
				}
				if(numberOfSegments < 1 ||
				   (offset + numberOfSegments) >= points.Length)
				{
					throw new ArgumentOutOfRangeException
						("numberOfSegments", S._("Arg_InvalidCurveSegments"));
				}

				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawCurve
						(dpoints, offset, numberOfSegments, tension);
				}
			}

	// Draw an ellipse.
	public void DrawEllipse(Pen pen, Rectangle rect)
			{
				DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void DrawEllipse(Pen pen, RectangleF rect)
			{
				DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void DrawEllipse(Pen pen, int x, int y, int width, int height)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddEllipse( x,y,width,height);
					DrawPath( pen, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectPen(pen);
						ToolkitGraphics.DrawArc(rect, 0.0f, 360.0f);
					}
				}
			}
	public void DrawEllipse(Pen pen, float x, float y,
							float width, float height)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddEllipse( x,y,width,height);
					DrawPath( pen, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectPen(pen);
						ToolkitGraphics.DrawArc(rect, 0.0f, 360.0f);
					}
				}
			}

	// Draw an unstretched complete icon via the toolkit.
	private void ToolkitDrawIcon(Icon icon, int x, int y)
			{
				lock(this)
				{
					IToolkitImage toolkitImage = icon.GetToolkitImage(this);
					if(toolkitImage != null)
					{
						ConvertPoint(ref x, ref y, pageUnit);
						ToolkitGraphics.DrawImage
							(toolkitImage, x + baseWindow.X,
							 y + baseWindow.Y);
					}
				}
			}

	// Draw an icon.
	public void DrawIcon(Icon icon, Rectangle targetRect)
			{
				if(icon == null)
				{
					throw new ArgumentNullException("icon");
				}
				if(targetRect.Width == icon.Width &&
				   targetRect.Height == icon.Height)
				{
					// This is the easy case.
					ToolkitDrawIcon(icon, targetRect.X, targetRect.Y);
				}
				else
				{
					// Stretch and draw the image.
					Point[] src = ConvertUnits
						(0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel);
					Point[] dest = ConvertRectangle3
						(targetRect.X, targetRect.Y,
						 targetRect.Width, targetRect.Height, pageUnit);
					BaseOffsetPoints(dest);
					lock(this)
					{
						IToolkitImage toolkitImage = icon.GetToolkitImage(this);
						if(toolkitImage != null)
						{
							ToolkitGraphics.DrawImage
								(toolkitImage, src, dest);
						}
					}
				}
			}
	public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
			{
				if(icon == null)
				{
					throw new ArgumentNullException("icon");
				}
				int width = icon.Width;
				int height = icon.Height;
				if(width <= targetRect.Width && height <= targetRect.Height)
				{
					// No truncation is necessary, so use the easy case.
					ToolkitDrawIcon(icon, targetRect.X, targetRect.Y);
				}
				else
				{
					// Only draw as much as will fit in the target rectangle.
					if(width > targetRect.Width)
					{
						width = targetRect.Width;
					}
					if(height > targetRect.Height)
					{
						height = targetRect.Height;
					}
					Point[] src = ConvertUnits
						(0, 0, width, height, GraphicsUnit.Pixel);
					Point[] dest = ConvertRectangle3
						(targetRect.X, targetRect.Y, width, height, pageUnit);
					BaseOffsetPoints(dest);
					lock(this)
					{
						IToolkitImage toolkitImage = icon.GetToolkitImage(this);
						if(toolkitImage != null)
						{
							ToolkitGraphics.DrawImage
								(toolkitImage, src, dest);
						}
					}
				}
			}
	public void DrawIcon(Icon icon, int x, int y)
			{
				if(icon == null)
				{
					throw new ArgumentNullException("icon");
				}
				ToolkitDrawIcon(icon, x, y);
			}

	// Used to call the toolkit DrawImage methods
	private void ToolkitDrawImage(Image image, Point[] src, Point[] dest)
			{
				if (image.toolkitImage == null)
					image.toolkitImage = ToolkitGraphics.Toolkit.CreateImage(image.dgImage, 0);
				BaseOffsetPoints(dest);
				ToolkitGraphics.DrawImage(image.toolkitImage,src, dest);
			}

	private void ToolkitDrawImage(Image image, int x, int y)
			{
				if (image.toolkitImage == null)
					image.toolkitImage = ToolkitGraphics.Toolkit.CreateImage(image.dgImage, 0);
				ToolkitGraphics.DrawImage(image.toolkitImage, x + baseWindow.X, y + baseWindow.Y);
			}

	// Draw an image.
	public void DrawImage(Image image, Point point)
			{
				DrawImage(image, point.X, point.Y);
			}

	public void DrawImage(Image image, Point[] destPoints)
			{
				Point[] src = ConvertUnits(0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
				Point[] dest = ConvertPoints(destPoints, 3, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}

	public void DrawImage(Image image, PointF point)
			{
				DrawImage(image, point.X, point.Y);
			}

	public void DrawImage(Image image, PointF[] destPoints)
			{
				Point[] src = ConvertUnits(0, 0, image.Width,  image.Height, GraphicsUnit.Pixel);
				Point[] dest = ConvertPoints(destPoints, 3, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}

	public void DrawImage(Image image, Rectangle rect)
			{
				Point[] src = ConvertUnits(0, 0, image.Width,  image.Height, GraphicsUnit.Pixel);
				Point[] dest = ConvertRectangle3(rect.X, rect.Y, rect.Width, rect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}

	public void DrawImage(Image image, RectangleF rect)
			{
				Point[] src = ConvertUnits(0, 0, image.Width,  image.Height, GraphicsUnit.Pixel);
				Point[] dest = ConvertRectangle3(rect.X, rect.Y, rect.Width, rect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}

	public void DrawImage(Image image, int x, int y)
			{
				lock(this)
				{
					ConvertPoint(ref x, ref y, pageUnit);
					ToolkitDrawImage(image, x, y);
				}
			}

	public void DrawImage(Image image, float x, float y)
			{
				int dx, dy;
				ConvertPoint(x, y, out dx, out dy, pageUnit);
				ToolkitDrawImage(image, dx, dy);
			}

	public void DrawImage(Image image, Point[] destPoints,
						  Rectangle srcRect, GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
				Point[] dest = ConvertPoints(destPoints, 3, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	
	public void DrawImage(Image image, PointF[] destPoints,
						  RectangleF srcRect, GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
				Point[] dest = ConvertPoints(destPoints, 3, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	
	public void DrawImage(Image image, Rectangle destRect,
						  Rectangle srcRect, GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
				Point[] dest = ConvertRectangle3(destRect.X, destRect.Y, destRect.Width, destRect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	
	public void DrawImage(Image image, RectangleF destRect,
						  RectangleF srcRect, GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
				Point[] dest = ConvertRectangle3(destRect.X, destRect.Y, destRect.Width, destRect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	public void DrawImage(Image image, int x, int y, int width, int height)
			{
				Point[] src = ConvertUnits(0, 0, image.Width,  image.Height, GraphicsUnit.Pixel);
				Point[] dest = ConvertRectangle3(x, y, width, height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	public void DrawImage(Image image, int x, int y,
						  Rectangle srcRect, GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
				Point[] dest = ConvertRectangle3(x, y, 0, 0, pageUnit);
				dest[1].X = image.Width;
				dest[2].Y = image.Height;
				ToolkitDrawImage(image, src, dest);
			}
	public void DrawImage(Image image, float x, float y,
						  RectangleF srcRect, GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
				Point[] dest = ConvertRectangle3(x, y, srcRect.Width, srcRect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	public void DrawImage(Image image, float x, float y,
						  float width, float height)
			{
				Point[] src = ConvertUnits(0, 0, image.Width,  image.Height, GraphicsUnit.Pixel);
				Point[] dest = ConvertRectangle3(x, y, width, height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  int srcX, int srcY, int srcWidth, int srcHeight,
						  GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcX, srcY, srcWidth, srcHeight, srcUnit);
				Point[] dest = ConvertRectangle3(destRect.X, destRect.Y, destRect.Width, destRect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}
	
	public void DrawImage(Image image, Rectangle destRect,
						  float srcX, float srcY,
						  float srcWidth, float srcHeight,
						  GraphicsUnit srcUnit)
			{
				Point[] src = ConvertUnits(srcX, srcY, srcWidth, srcHeight, srcUnit);
				Point[] dest = ConvertRectangle3(destRect.X, destRect.Y, destRect.Width, destRect.Height, pageUnit);
				ToolkitDrawImage(image, src, dest);
			}

	// We don't use image attributes or image abort procedures in this
	// implementation, so stub out the remaining DrawImage methods.
	public void DrawImage(Image image, Point[] destPoints,
						  Rectangle srcRect, GraphicsUnit srcUnit,
						  ImageAttributes imageAttr)
			{
				DrawImage(image, destPoints, srcRect, srcUnit);
			}
	public void DrawImage(Image image, PointF[] destPoints,
						  RectangleF srcRect, GraphicsUnit srcUnit,
						  ImageAttributes imageAttr)
			{
				DrawImage(image, destPoints, srcRect, srcUnit);
			}
	public void DrawImage(Image image, Point[] destPoints,
						  Rectangle srcRect, GraphicsUnit srcUnit,
						  ImageAttributes imageAttr,
						  DrawImageAbort callback)
			{
				DrawImage(image, destPoints, srcRect, srcUnit);
			}
	public void DrawImage(Image image, PointF[] destPoints,
						  RectangleF srcRect, GraphicsUnit srcUnit,
						  ImageAttributes imageAttr,
						  DrawImageAbort callback)
			{
				DrawImage(image, destPoints, srcRect, srcUnit);
			}
	public void DrawImage(Image image, Point[] destPoints,
						  Rectangle srcRect, GraphicsUnit srcUnit,
						  ImageAttributes imageAttr,
						  DrawImageAbort callback, int callbackdata)
			{
				DrawImage(image, destPoints, srcRect, srcUnit);
			}
	public void DrawImage(Image image, PointF[] destPoints,
						  RectangleF srcRect, GraphicsUnit srcUnit,
						  ImageAttributes imageAttr,
						  DrawImageAbort callback, int callbackdata)
			{
				DrawImage(image, destPoints, srcRect, srcUnit);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  int srcX, int srcY, int srcWidth, int srcHeight,
						  GraphicsUnit srcUnit, ImageAttributes imageAttr)
			{
				DrawImage(image, destRect, srcX, srcY, srcWidth,
						  srcHeight, srcUnit);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  float srcX, float srcY,
						  float srcWidth, float srcHeight,
						  GraphicsUnit srcUnit, ImageAttributes imageAttr)
			{
				DrawImage(image, destRect, srcX, srcY, srcWidth,
						  srcHeight, srcUnit);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  int srcX, int srcY, int srcWidth, int srcHeight,
						  GraphicsUnit srcUnit, ImageAttributes imageAttr,
						  DrawImageAbort callback)
			{
				DrawImage(image, destRect, srcX, srcY, srcWidth,
						  srcHeight, srcUnit);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  float srcX, float srcY,
						  float srcWidth, float srcHeight,
						  GraphicsUnit srcUnit, ImageAttributes imageAttr,
						  DrawImageAbort callback)
			{
				DrawImage(image, destRect, srcX, srcY, srcWidth,
						  srcHeight, srcUnit);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  int srcX, int srcY, int srcWidth, int srcHeight,
						  GraphicsUnit srcUnit, ImageAttributes imageAttr,
						  DrawImageAbort callback, IntPtr callbackData)
			{
				DrawImage(image, destRect, srcX, srcY, srcWidth,
						  srcHeight, srcUnit);
			}
	public void DrawImage(Image image, Rectangle destRect,
						  float srcX, float srcY,
						  float srcWidth, float srcHeight,
						  GraphicsUnit srcUnit, ImageAttributes imageAttr,
						  DrawImageAbort callback, IntPtr callbackData)
			{
				DrawImage(image, destRect, srcX, srcY, srcWidth,
						  srcHeight, srcUnit);
			}

	// Draw an unscaled image.
	public void DrawImageUnscaled(Image image, Point point)
			{
				DrawImageUnscaled(image, point.X, point.Y);
			}
	public void DrawImageUnscaled(Image image, Rectangle rect)
			{
				DrawImageUnscaled(image, rect.X, rect.Y);
			}
	public void DrawImageUnscaled(Image image, int x, int y,
								  int width, int height)
			{
				DrawImageUnscaled(image, x, y);
			}
	public void DrawImageUnscaled(Image image, int x, int y)
			{
				DrawImage(image, x, y);
			}

	// Draw a line between two points.
	public void DrawLine(Pen pen, Point pt1, Point pt2)
			{
				DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
			}
	public void DrawLine(Pen pen, PointF pt1, PointF pt2)
			{
				DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
			}
	public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				ConvertPoint(ref x1, ref y1, pageUnit);
				ConvertPoint(ref x2, ref y2, pageUnit);
				if (x1 == x2 && y1 == y2)
					return;
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawLine(x1 + baseWindow.X, y1 + baseWindow.Y,
						x2 + baseWindow.X, y2+baseWindow.Y);
				}
			}
	public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				int dx1, dy1, dx2, dy2;
				ConvertPoint(x1, y1, out dx1, out dy1, pageUnit);
				ConvertPoint(x2, y2, out dx2, out dy2, pageUnit);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawLine(dx1 + baseWindow.X, dy1 + baseWindow.Y,
						dx2 + baseWindow.X, dy2 + baseWindow.Y);
				}
			}

	// Draw a series of connected line segments.
	public void DrawLines(Pen pen, Point[] points)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				points = ConvertPoints(points, 2, pageUnit);
				BaseOffsetPoints(points);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawLines(points);
				}
			}
	public void DrawLines(Pen pen, PointF[] points)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				Point[] dpoints = ConvertPoints(points, 2, pageUnit);
				BaseOffsetPoints(dpoints);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawLines(dpoints);
				}
			}

	// Draw a path object.
	public void DrawPath(Pen pen, GraphicsPath path)
			{
				if(pen == null)
				{
					throw new ArgumentNullException("pen");
				}
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}

				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				path.Draw(this, pen);
			}

	// Draw a pie shape.
	public void DrawPie(Pen pen, Rectangle rect,
						float startAngle, float sweepAngle)
			{
				if(((float)(int)startAngle) == startAngle &&
				   ((float)(int)sweepAngle) == sweepAngle)
				{
					DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height,
							(int)startAngle, (int)sweepAngle);
				}
				else
				{
					DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height,
							startAngle, sweepAngle);
				}
			}
	public void DrawPie(Pen pen, RectangleF rect,
						float startAngle, float sweepAngle)
			{
				DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height,
						startAngle, sweepAngle);
			}
	public void DrawPie(Pen pen, int x, int y, int width, int height,
						int startAngle, int sweepAngle)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawPie(rect, startAngle, sweepAngle);
				}
			}
	public void DrawPie(Pen pen, float x, float y, float width, float height,
						float startAngle, float sweepAngle)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawPie(rect, startAngle, sweepAngle);
				}
			}

	// Draw a polygon.
	public void DrawPolygon(Pen pen, Point[] points)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				points = ConvertPoints(points, 2, pageUnit);
				BaseOffsetPoints(points);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawPolygon(points);
				}
			}
	public void DrawPolygon(Pen pen, PointF[] points)
			{
				// Bail out now if there's nothing to draw.
				if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
				{
					return;
				}

				Point[] dpoints = ConvertPoints(points, 2, pageUnit);
				BaseOffsetPoints(dpoints);
				lock(this)
				{
					SelectPen(pen);
					ToolkitGraphics.DrawPolygon(dpoints);
				}
			}

	// Draw a rectangle.
	public void DrawRectangle(Pen pen, Rectangle rect)
			{
				DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void DrawRectangle(Pen pen, int x, int y, int width, int height)
			{
				if (width>0 && height>0)
				{
					// Bail out now if there's nothing to draw.
					if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
					{
						return;
					}

					lock(this)
					{
						SelectPen(pen);
						ToolkitGraphics.DrawPolygon(ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit));
					}
				}
			}
	public void DrawRectangle(Pen pen, float x, float y,
							  float width, float height)
			{
				if (width>0 && height>0)
				{
					// Bail out now if there's nothing to draw.
					if(pen.PenType == PenType.SolidColor && pen.Color.A == 0)
					{
						return;
					}

					lock(this)
					{
						SelectPen(pen);
						ToolkitGraphics.DrawPolygon(ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit));
					}
				}
			}

	// Draw a series of rectangles.
	public void DrawRectangles(Pen pen, Rectangle[] rects)
			{
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				int posn;
				for(posn = 0; posn < rects.Length; ++posn)
				{
					DrawRectangle(pen, rects[posn].X, rects[posn].Y,
								  rects[posn].Width, rects[posn].Height);
				}
			}
	public void DrawRectangles(Pen pen, RectangleF[] rects)
			{
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				int posn;
				for(posn = 0; posn < rects.Length; ++posn)
				{
					DrawRectangle(pen, rects[posn].X, rects[posn].Y,
								  rects[posn].Width, rects[posn].Height);
				}
			}

	// Draw a string.
	public void DrawString(String s, Font font, Brush brush, PointF point)
			{
				DrawString(s, font, brush, point.X, point.Y, null);
			}
	public void DrawString(String s, Font font, Brush brush,
						   RectangleF layoutRectangle)
			{
				DrawString(s, font, brush, layoutRectangle, null);
			}
	public void DrawString(String s, Font font, Brush brush,
						   PointF point, StringFormat format)
			{
				DrawString(s, font, brush, point.X, point.Y, format);
			}
	public void DrawString(String s, Font font, Brush brush,
						   RectangleF layoutRectangle, StringFormat format)
			{
				// bail out now if there's nothing to draw
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				// bail out now if there's nothing to draw
				if(((Object)s) == null || s.Length == 0) { return; }

				// make a little inset around the text dependent of the font size
				// (only for non-typographic format) 
				if(format == null || !format.IsTypographic)
				{
					layoutRectangle.Inflate(
								(int)(-font.SizeInPoints*DpiX/369.7184), 0);
				}

				// convert the layout into device coordinates
				Point[] rect = ConvertRectangle
					((layoutRectangle.X + baseWindow.X),
					 (layoutRectangle.Y + baseWindow.Y),
					 (layoutRectangle.Width - 1),
					 (layoutRectangle.Height - 1),
					 pageUnit);

				// create a layout rectangle from the device coordinates
				Rectangle deviceLayout = new Rectangle
					(rect[0].X, rect[0].Y,
					 (rect[1].X - rect[0].X + 1),
					 (rect[2].Y - rect[0].Y + 1));

				// bail out now if there's nothing to draw
				if(clip != null &&
				   !deviceClipExtent.IntersectsWith(deviceLayout))
				{
					return;
				}

				// ensure we have a text layout manager
				if(textLayoutManager == null)
				{
					textLayoutManager = new TextLayoutManager();
				}

				// set the default temporary clip
				Region clipTemp = null;

				// draw the text
				lock(this)
				{
					// get the clipping region, if needed
					if(format == null ||
					   ((format.FormatFlags & StringFormatFlags.NoClip) == 0))
					{
						// get the clipping region, if there is one
						if(clip != null)
						{
							// get a copy of the current clipping region
							clipTemp = clip.Clone();

							// interset the clipping region with the layout
							SetClip(layoutRectangle, CombineMode.Intersect);
						}
					}

					// attempt to draw the text
					try
					{
						// Workaround for calculation new font size, if a transformation is set
						// this does only work for scaling, not for rotation or multiply transformations
						
						// draw the text
						textLayoutManager.Draw
							(this, s, this.TransformFont(font), deviceLayout, format, brush);
					}
					finally
					{
						// reset the clip
						if(clipTemp != null) { Clip = clipTemp; }
					}
				}
			}
			
	// Workaround for calculation new font size, if a transformation is set
	// this does only work for scaling, not for rotation or multiply transformations
	// Normally we should stretch or shrink the font.
	Font TransformFont( Font font_in ) 
	{
		Font font = font_in;

		if( transform != null ) {	
			float sizeOld = font_in.Size;
			float sizeNew = transform.TransformFontSize( sizeOld );
			if( sizeOld != sizeNew ) {
				font = new Font( font_in.FontFamily, sizeNew, font_in.Style, font_in.Unit, font_in.GdiCharSet, font_in.GdiVerticalFont );
			}
		}
		return font;
	}
	
	public void DrawString(String s, Font font, Brush brush, float x, float y)
			{
				DrawString(s, font, brush, x, y, null);
			}

	public void DrawString(String s, Font font, Brush brush,
						   float x, float y, StringFormat format)
			{
				DrawString(s, font, brush, new RectangleF(x, y, 999999.0f, 999999.0f), format);
			}

	// Reset the graphics state back to a previous container level.
	public void EndContainer(GraphicsContainer container)
			{
				if(container != null)
				{
					lock(this)
					{
						container.Restore(this);
					}
				}
			}

	// Enumerate the contents of a metafile.
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point destPoint,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point destPoint,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point destPoint,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point destPoint,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point destPoint,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point destPoint,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF destPoint,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
								  Rectangle srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}
	[TODO]
	public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
								  RectangleF srcRect, GraphicsUnit srcUnit,
								  EnumerateMetafileProc callback,
								  IntPtr callbackData,
								  ImageAttributes imageAttr)
			{
				// TODO
			}

	// Update the clipping region to exclude a particular rectangle.
	public void ExcludeClip(Rectangle rect)
			{
				Clip.Exclude(rect);
				UpdateClip();
			}
	public void ExcludeClip(Region region)
			{
				Clip.Exclude(region);
				UpdateClip();
			}

	// Fill a closed cardinal spline.
	public void FillClosedCurve(Brush brush, Point[] points)
			{
				FillClosedCurve(brush, points, FillMode.Alternate, 0.5f);
			}
	public void FillClosedCurve(Brush brush, PointF[] points)
			{
				FillClosedCurve(brush, points, FillMode.Alternate, 0.5f);
			}
	public void FillClosedCurve(Brush brush, Point[] points,
								FillMode fillMode)
			{
				FillClosedCurve(brush, points, fillMode, 0.5f);
			}
	public void FillClosedCurve(Brush brush, PointF[] points,
								FillMode fillMode)
			{
				FillClosedCurve(brush, points, fillMode, 0.5f);
			}
	public void FillClosedCurve(Brush brush, Point[] points,
								FillMode fillMode, float tension)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				points = ConvertPoints(points, 4, pageUnit);
				BaseOffsetPoints(points);
				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.FillClosedCurve
						(points, tension, fillMode);
				}
			}
	public void FillClosedCurve(Brush brush, PointF[] points,
								FillMode fillMode, float tension)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				Point[] dpoints = ConvertPoints(points, 4, pageUnit);
				BaseOffsetPoints(dpoints);
				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.FillClosedCurve
						(dpoints, tension, fillMode);
				}
			}

	// Fill an ellipse.
	public void FillEllipse(Brush brush, Rectangle rect)
			{
				FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void FillEllipse(Brush brush, RectangleF rect)
			{
				FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void FillEllipse(Brush brush, int x, int y, int width, int height)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddEllipse( x,y,width,height);
					FillPath( brush, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectBrush(brush);
						ToolkitGraphics.FillPie(rect, 0.0f, 360.0f);
					}
				}
			}
	public void FillEllipse(Brush brush, float x, float y,
							float width, float height)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddEllipse( x,y,width,height);
					FillPath( brush, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectBrush(brush);
						ToolkitGraphics.FillPie(rect, 0.0f, 360.0f);
					}
				}
			}

	// Fill the interior of a path.
	public void FillPath(Brush brush, GraphicsPath path)
			{
				if(brush == null)
				{
					throw new ArgumentNullException("brush");
				}
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}

				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				path.Fill(this, brush);
			}

	// Fill a pie shape.
	public void FillPie(Brush brush, Rectangle rect,
						float startAngle, float sweepAngle)
			{
				if(((float)(int)startAngle) == startAngle &&
				   ((float)(int)sweepAngle) == sweepAngle)
				{
					FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height,
							(int)startAngle, (int)sweepAngle);
				}
				else
				{
					FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height,
							startAngle, sweepAngle);
				}
			}
	public void FillPie(Brush brush, int x, int y, int width, int height,
						int startAngle, int sweepAngle)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddPie( x,y,width,height,startAngle,sweepAngle);
					FillPath( brush, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectBrush(brush);
						ToolkitGraphics.FillPie(rect, startAngle, sweepAngle);
					}
				}
			}
	public void FillPie(Brush brush, float x, float y,
						float width, float height,
						float startAngle, float sweepAngle)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				// if transform is set use graphics path to draw or fill, since the transformation works for this then.
				if( null != transform ) {
					GraphicsPath path = new GraphicsPath();
					path.AddPie( x,y,width,height,startAngle,sweepAngle);
					FillPath( brush, path );
				}
				else {
					Point[] rect = ConvertRectangle(x + baseWindow.X, y + baseWindow.Y, width, height, pageUnit);
					lock(this)
					{
						SelectBrush(brush);
						ToolkitGraphics.FillPie(rect, startAngle, sweepAngle);
					}
				}
			}

	// Fill a polygon.
	public void FillPolygon(Brush brush, Point[] points)
			{
				FillPolygon(brush, points, FillMode.Alternate);
			}
	public void FillPolygon(Brush brush, PointF[] points)
			{
				FillPolygon(brush, points, FillMode.Alternate);
			}
	public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				points = ConvertPoints(points, 2, pageUnit);
				BaseOffsetPoints(points);
				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.FillPolygon(points, fillMode);
				}
			}
	public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				Point[] dpoints = ConvertPoints(points, 2, pageUnit);
				BaseOffsetPoints(dpoints);
				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.FillPolygon(dpoints, fillMode);
				}
			}

	// Fill a rectangle.
	public void FillRectangle(Brush brush, Rectangle rect)
			{
				FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void FillRectangle(Brush brush, RectangleF rect)
			{
				FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void FillRectangle(Brush brush, int x, int y, int width, int height)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.FillPolygon(ConvertRectangle(x + baseWindow.X,
						y + baseWindow.Y, width, height, pageUnit), FillMode.Alternate);
				}
			}
	public void FillRectangle(Brush brush, float x, float y,
							  float width, float height)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				lock(this)
				{
					SelectBrush(brush);
					ToolkitGraphics.FillPolygon(ConvertRectangle(x + baseWindow.X,
						y + baseWindow.Y, width, height, pageUnit), FillMode.Alternate);
				}
			}

	// Fill a series of rectangles.
	public void FillRectangles(Brush brush, Rectangle[] rects)
			{
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				int posn;
				for(posn = 0; posn < rects.Length; ++posn)
				{
					FillRectangle(brush, rects[posn].X, rects[posn].Y,
								  rects[posn].Width, rects[posn].Height);
				}
			}
	public void FillRectangles(Brush brush, RectangleF[] rects)
			{
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				int posn;
				for(posn = 0; posn < rects.Length; ++posn)
				{
					FillRectangle(brush, rects[posn].X, rects[posn].Y,
								  rects[posn].Width, rects[posn].Height);
				}
			}

	// Fill a region.
	public void FillRegion(Brush brush, Region region)
			{
				// Bail out now if there's nothing to draw.
				if((brush is SolidBrush) && ((SolidBrush)brush).Color.A == 0)
				{
					return;
				}

				RectangleF[] rs = region.GetRegionScans(new Drawing.Drawing2D.Matrix());
				for (int i = 0; i < rs.Length; i++)
				{
					Rectangle b = Rectangle.Truncate(rs[i]);
					FillRectangle(brush, rs[i]);
				}
			}

	// Flush graphics operations to the display device.
	public void Flush()
			{
				Flush(FlushIntention.Flush);
			}
	public void Flush(FlushIntention intention)
			{
				lock(this)
				{
					if(graphics != null)
					{
						graphics.Flush(intention);
					}
				}
			}

	// Create a Graphics object from a HDC.
	public static Graphics FromHdc(IntPtr hdc)
			{
				return new Graphics
					(ToolkitManager.Toolkit.CreateFromHdc(hdc, IntPtr.Zero));
			}
	public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
			{
				return new Graphics
					(ToolkitManager.Toolkit.CreateFromHdc(hdc, hdevice));
			}
	public static Graphics FromHdcInternal(IntPtr hdc)
			{
				return FromHdc(hdc);
			}

	// Create a Graphics object from a HWND.
	public static Graphics FromHwnd(IntPtr hwnd)
			{
				return new Graphics
					(ToolkitManager.Toolkit.CreateFromHwnd(hwnd));
			}
	public static Graphics FromHwndInternal(IntPtr hwnd)
			{
				return FromHwnd(hwnd);
			}

	// Create a graphics object for drawing into an image.
	public static Graphics FromImage(Image image)
			{
				if (image.toolkitImage == null)
					image.toolkitImage = ToolkitManager.Toolkit.CreateImage(image.dgImage, 0);
				Graphics g = new Graphics(ToolkitManager.Toolkit.CreateFromImage(image.toolkitImage));
				return g;
			}

	// Get the handle for the Windows halftone palette.  Not used here.
	public static IntPtr GetHalftonePalette()
			{
				return ToolkitManager.Toolkit.GetHalftonePalette();
			}

	// Get the HDC associated with this graphics object.
	public IntPtr GetHdc()
			{
				lock(this)
				{
					if(graphics != null)
					{
						return graphics.GetHdc();
					}
					else
					{
						return IntPtr.Zero;
					}
				}
			}

	// Get the nearest color that is supported by this graphics object.
	public Color GetNearestColor(Color color)
			{
				lock(this)
				{
					if(graphics != null)
					{
						return graphics.GetNearestColor(color);
					}
					else
					{
						return color;
					}
				}
			}

	// Intersect a region with the current clipping region.
	public void IntersectClip(Rectangle rect)
			{
				Clip.Intersect(rect);
				UpdateClip();
			}
	public void IntersectClip(RectangleF rect)
			{
				Clip.Intersect(rect);
				UpdateClip();
			}
	public void IntersectClip(Region region)
			{
				Clip.Intersect(region);
				UpdateClip();
			}

	// Determine if a point is within the visible clip region.
	public bool IsVisible(Point point)
			{
				return Clip.IsVisible(point, this);
			}
	public bool IsVisible(PointF point)
			{
				return Clip.IsVisible(point, this);
			}
	public bool IsVisible(Rectangle rect)
			{
				return Clip.IsVisible(rect, this);
			}
	public bool IsVisible(RectangleF rect)
			{
				return Clip.IsVisible(rect, this);
			}
	public bool IsVisible(int x, int y)
			{
				return Clip.IsVisible(x, y, this);
			}
	public bool IsVisible(float x, float y)
			{
				return Clip.IsVisible(x, y, this);
			}
	public bool IsVisible(int x, int y, int width, int height)
			{
				return Clip.IsVisible(x, y, width, height, this);
			}
	public bool IsVisible(float x, float y, float width, float height)
			{
				return Clip.IsVisible(x, y, width, height, this);
			}

	// First attempt as a way to measure strings and draw strings, taking into account a string format.
	// The string measuring happens a word at a time.
	private class StringDrawPositionCalculator
	{
		private SplitWord[] words;
		private int wordCount;
		private String text;
		private Point[] linePositions;
		private Graphics graphics;
		private Font font;
		private RectangleF layout;
		private StringFormat format;
		private int HotKeyIdx;

		// Details for each word
		private struct SplitWord
		{
			// Publicly-accessible state.
			public int start;
			public int length;
			public int width;
			public int line;

			// Constructor.
			public SplitWord(int start, int length)
					{
						this.start = start;
						this.length = length;
						this.width = 0;
						this.line = -1;
					}

		}; // struct SplitWord


		public StringDrawPositionCalculator(String text, Graphics graphics, Font font, RectangleF layout, StringFormat format)
				{
					this.text = text;
					this.graphics = graphics;
					this.font = font;
					this.layout = layout;
					this.format = format;
					this.HotKeyIdx = -1;
					
					if(format.HotkeyPrefix == System.Drawing.Text.HotkeyPrefix.Show)
					{
						HotKeyIdx = text.IndexOf('&');
						if( HotKeyIdx != -1)
						{
							if (HotKeyIdx >= text.Length-1 ||
						 		Char.IsControl(text[HotKeyIdx]))
							{
								HotKeyIdx = -1;
							}
							else
							{
								this.text = text.Substring(0,HotKeyIdx) +
											text.Substring(HotKeyIdx + 1);
							}
						}	
					}
				}

		public void LayoutByWords()
				{
					// Break the string into "words". Each word has a start pos, end pos and measured size.
					// Each new line group is treated as a "word" as is each whitespace.
					wordCount = 0;
					words = new SplitWord[16];
					int start = 0;
					int len = text.Length;
					for(int i = 0; i < len;)
					{
						start = i;
						char c = text[i];
						// Look for \r on its own, \n on its own or \r\n.
						if(c == '\r')
						{
							if(i < (len - 1) && text[i+1]=='\n')
							{
								i += 2;
							}
							else
							{
								i++;
							}
						}
						else if(c == '\n')
						{
							i++;
						}
						else
						{
							// Skip over the whitespace.
							while(i < len && Char.IsWhiteSpace(text[i]))
							{
								i++;
							}
							// We are at the start of text so skip over the text.
							if(i == start)
							{
								while(i < len)
								{
									c = text[i];
									if(Char.IsWhiteSpace(c) ||
									   c == '\n' || c == '\r')
									{
										break;
									}
									i++;
								}
							}
						}
						// Dynamically allocate the array if we need more space.
						if(wordCount >= words.Length)
						{
							SplitWord[] newWords = new SplitWord[words.Length * 2];
							Array.Copy(words, newWords, words.Length);
							words = newWords;
						}
						// Add the word.
						words[wordCount++] = new SplitWord(start, i - start);
					}
					Layout();
				}

		private void Layout()
				{
					MeasureStrings();
					WrapLines(layout.Width -1);
					linePositions = GetPositions(layout, format);
				}

		// Draw each line
		public void Draw(Brush brush)
				{
					graphics.SelectBrush(brush);
					if (linePositions.Length == 0)
						return;
					int currentLine = 0;
					int textStart = 0;
					int textLength = 0;
						
					SplitWord word = new SplitWord(0,0);
					for (int i = 0; i <= wordCount; i++)
					{
						if (i != wordCount)
						{
							word = words[i];
							if(word.line== -1)
								continue;
							if (word.line == currentLine)
							{
								textLength += word.length;
								continue;
							}
						}
						Point linePosition = linePositions[currentLine];
						// Draw if some of it is within layout.
						if(linePosition.X <= layout.Right && linePosition.Y <= layout.Bottom)
						{
							if (HotKeyIdx >= textStart && HotKeyIdx < textStart+textLength)
							{
								String startString = text.Substring(textStart, 
													 	HotKeyIdx -  textStart );
								String endString = text.Substring(HotKeyIdx+1, 
														textLength - (HotKeyIdx -textStart) -1);
								graphics.ToolkitGraphics.DrawString(startString, linePosition.X, linePosition.Y, null);  
								Font underlineFont = new Font (font, font.Style | FontStyle.Underline);
								float startWidth = graphics.MeasureString(startString,font).Width;
								float hotkeyWidth = graphics.MeasureString(text.Substring(HotKeyIdx,1),underlineFont).Width;
								graphics.SelectFont(font);
								graphics.ToolkitGraphics.DrawString(endString,
											linePosition.X+(int)(startWidth+hotkeyWidth), linePosition.Y, null);
								graphics.SelectFont(underlineFont);
								graphics.ToolkitGraphics.DrawString(text.Substring(HotKeyIdx,1),
											linePosition.X+(int)startWidth, linePosition.Y, null);
								graphics.SelectFont(font);
							} 
							else
							{
								String lineText = text.Substring(textStart, textLength);
								graphics.ToolkitGraphics.DrawString(lineText, linePosition.X, linePosition.Y, null);
							}
						}
						textStart = word.start;
						textLength = word.length;
						currentLine = word.line;
					}
				}

		// Calculate the bounds of the measured strings, the number of characters fitted and the number of lines.
		public Size GetBounds(out int charactersFitted, out int linesFilled)
				{
					linesFilled = 0;
					charactersFitted = 0;
					if (linePositions.Length == 0)
					{
						return Size.Empty;
					}
					bool noPartialLines = (format.FormatFlags & StringFormatFlags.LineLimit) != 0;
					int h = font.Height;
					// Find number of lines filled.
					for (int i = 0; i < linePositions.Length; i++)
					{
						if (noPartialLines)
						{
							if (linePositions[i].Y + h > layout.Height)
							{
								break;
							}
						}
						else if (linePositions[i].Y > layout.Height)
						{
							break;
						}
						linesFilled++;
					}

					int maxWidth = 0;
					int currentWidth = 0;
					int currentLine = 0;
					// Find the maximum width of a line and the number of characters fitted.
					for(int i=0; i < wordCount; i++)
					{
						SplitWord word = words[i];
						charactersFitted += word.length;
						if(word.line == currentLine)
						{
							currentWidth += word.width;
						}
						else
						{
							if (currentWidth > maxWidth)
							{
								maxWidth = currentWidth;
							}
							currentWidth = word.width;
							currentLine++;
						}
					}
					if (currentWidth > maxWidth)
					{
						maxWidth = currentWidth;
					}

					return new Size(maxWidth, h * linesFilled);
				}
		// Use the toolkit to measure all the words and spaces.
		private void  MeasureStrings() 
				{
					graphics.SelectFont(font);
					int spaceWidth = -1;
					Point[] rect = new Point[0];
					int charactersFitted;
					int linesFilled;
					for(int i=0;i< wordCount;i++)
					{
						SplitWord word = words[i];
						char c = text[word.start];
						if (c != '\n' && c != '\r')
						{
							if (char.IsWhiteSpace(c))
							{
								if(spaceWidth == -1)
								{
									spaceWidth = graphics.ToolkitGraphics.MeasureString(" ", rect, null, out charactersFitted, out linesFilled, false).Width;
								}
								word.width = spaceWidth;
								words[i] = word;
							}
							else
							{
								word.width = graphics.ToolkitGraphics.MeasureString(text.Substring(word.start, word.length), rect, null, out charactersFitted, out linesFilled, false).Width;
								words[i] = word;
							}
						}
					}
				}

		// Calculate the word wrap from the words.
		private void WrapLines(float lineSize)
				{
					float currSize=0;
					int currLine=0;

					for(int i=0; i < wordCount;i++)
					{
						SplitWord word = words[i];
						char c = text[word.start];
						// Wrap when \r\n
						if (c == '\r' || c == '\n') 
						{
							currLine++;
							currSize = 0;
							continue;
						}
						if (Char.IsWhiteSpace(c))
						{
							if( i< wordCount-1)
							{
								// Check that we are not at the end of the line.
								SplitWord nextWord = words[i+1];
								char c1 = text[nextWord.start];
								if (c1 != '\r' && c1 != '\n')
								{
									// If we have space for the next word in the line then set the lines.
									if (currSize + word.width + nextWord.width <= lineSize) 
									{
										currSize += word.width + nextWord.width;
										word.line = nextWord.line = currLine;
										words[i] = word;
										words[i+1] = nextWord;
										i+=1;
									} 
									else
									{
										// No space left in the line.
										currLine++;
										currSize=0;
									}
								}
							}
						}
						else  
						{
							// we have no whitespace in line -> append / wrap line
							if (currSize + word.width < lineSize || currLine == 0)
							{
								word.line = currLine;
								words[i] = word;
								currSize += word.width;
							} 
							else
							{
								word.line = ++currLine;					
								words[i] = word;
								currSize=0;
							}
						}
					}
				}
		// Calculate the Position of the wrapped lines
		private Point[] GetPositions(RectangleF layout, StringFormat format)
				{
					// Get the total number of lines by checking from the back.
					int numLines = 0;
					for(int i = wordCount-1; i >= 0; i--)
					{
						if(words[i].line!=-1)
						{
							numLines = words[i].line + 1;
							break;
						}
					}
					Point[] linePositions = new Point[numLines];

					int h = font.Height;

					int yOffset=0;
					// Set the offset based on the  LineAlignment.
					if (format.LineAlignment == StringAlignment.Far)
					{
						yOffset = (int)layout.Height - numLines * h;
					}
					else if (format.LineAlignment == StringAlignment.Center)
					{
						yOffset = ((int)layout.Height - numLines * h)/2;
					}

					for(int line=0; line < numLines; line++)
					{
						int xOffset = 0;
						if (format.Alignment != StringAlignment.Near)
						{
							for(int i=0; i < wordCount; i++)
							{
								SplitWord word = words[i];
								if(word.line == line)
								{
									xOffset += word.width;
								}
								if(word.line > line)
								{
									break;
								}
							}
							// Set the offset based on the Alignment.
							if (format.Alignment == StringAlignment.Far)
							{
								xOffset = (int)layout.Width - 1 - xOffset;
							}
							else if (format.Alignment == StringAlignment.Center)
							{
								xOffset = ((int)layout.Width - 1 - xOffset)/2;
							}
						}

						linePositions[line].Y=(int)layout.Y + yOffset + line*h;
						linePositions[line].X=(int)layout.X + xOffset;
					}
					return linePositions;
				}
	}; // class StringDrawPositionCalculator

	private class StringMeasurePositionCalculator
	{
		// Internal state.
		private Graphics graphics;
		private String text;
		private Font font;
		private Rectangle layout;
		private StringFormat format;
		private int[] lines = new int[16];
		private int count = 0;
		private int currentLine = 0;

		// NOTE: this needs to be rewritten as stateless in order to avoid
		//       excess allocations... this also needs to be fixed so that
		//       it actually handles horizontal text layout (currently only
		//       handles vertical) for character range measurements


		// Constructor.
		public StringMeasurePositionCalculator
					(Graphics graphics, String text, Font font,
					 Rectangle layout, StringFormat format)
				{
					this.graphics = graphics;
					this.text = text;
					this.font = font;
					this.layout = layout;
					this.format = format;
				}


		// Get the regions for the character ranges of the string format.
		public Region[] GetRegions()
				{
					Rectangle[] bounds = GetCharBounds();
					// Now consolidate positions based on character ranges
					Region[] regions = new Region[format.ranges.Length];
					for (int i = 0; i < format.ranges.Length; i++)
					{
						CharacterRange range = format.ranges[i];
						Region region = null;
						for (int j = range.First; j < range.First + range.Length; j++)
						{
							if (region == null)
								region = new Region(bounds[j]);
							else
								region.Union(bounds[j]);
						}
						regions[i] = region;
					}
					return regions;
				}

		// Get the rectangles bounding each character.
		public Rectangle[] GetCharBounds()
				{
					// create the bounds
					Rectangle[] bounds = new Rectangle[text.Length];

					// set the default maximum x
					int xMax = 0;

					// set the default maximum y
					int yMax = 0;

					// get the font height
					int fontHeight = (int)font.GetHeight(graphics);

					// get the vertical flag
					bool vertical = ((format.FormatFlags &
					                  StringFormatFlags.DirectionVertical) != 0);

					// set the maximum x and y based on the vertical flag
					if(vertical)
					{
						// set the maximum x to the maximum height
						xMax = (layout.Height - 1);

						// set the maximum y to the maximum width
						yMax = (layout.Width - 1);
					}
					else
					{
						// set the maximum x to the maximum width
						xMax = (layout.Width - 1);
						if(!format.IsTypographic)
						{
							xMax -= (int)(font.SizeInPoints*graphics.DpiX/369.7184);
						}

						// set the maximum y to the maximum height
						yMax = (layout.Height - 1);
					}

					// get the wrapping flag
					bool noWrap = ((format.FormatFlags &
					                StringFormatFlags.NoWrap) != 0);

					// set the line size to the maximum y
					int lineSizeRemaining = yMax;

					// add the first line
					AddLine(0);

					// set the current position
					int currentPos = 0;

					// add the lines of text
					do
					{
						// measure the line
						MeasureLine
							(ref bounds, ref currentPos, ref text, xMax,
							 graphics, font, vertical, noWrap);

						// add the current line
						AddLine(currentPos);

						// update the remaining line height
						lineSizeRemaining -= fontHeight;
					}
					while(currentPos < text.Length && lineSizeRemaining >= 0 && !noWrap);

					// set the default y offset
					int yOffset = 0;

					// adjust the y offset for the line alignment
					if(format.LineAlignment == StringAlignment.Center)
					{
						yOffset = (lineSizeRemaining / 2);
					}
					else if(format.LineAlignment == StringAlignment.Far)
					{
						yOffset = lineSizeRemaining;
					}

					// set the default x offset
					int xOffset = 0;

					// ??
					for(int i = 0; i < text.Length;)
					{
						// ??
						if(CurrentLine == i)
						{
							// ??
							currentLine++;

							// ??
							xOffset = 0;

							// ??
							if(format.Alignment == StringAlignment.Far)
							{
								// Go back and find the right point of the last visible character

								// ??
								int back = CurrentLine - 1;

								// ??
								if(back > -1)
								{
									// ??
									for(; back >= 0; back--)
									{
										// ??
										if(bounds[back] != Rectangle.Empty)
										{
											break;
										}
									}

									// ??
									xOffset = xMax + 1 - bounds[back].Right;
								}
								else
								{
									// ??
									xOffset = xMax + 1;
								}
							}
							else if(format.Alignment == StringAlignment.Center)
							{
								// ??
								for(int j = i; j < CurrentLine; j++)
								{
									// ??
									xOffset += bounds[j].Width;
								}

								// ??
								xOffset = (xMax + 1 - xOffset)/2;
							}
						}
						else 
						{
							// ??
							if(bounds[i] != Rectangle.Empty)
							{
								// ??
								Rectangle rect = bounds[i];

								// ??
								bounds[i] = new Rectangle
									((rect.Left + xOffset + layout.Left),
									 (rect.Top + ((currentLine - 1) *
									  fontHeight) + layout.Top + 1),
									 rect.Width, rect.Height);
							}

							// ??
							i++;
						}
					}

					// ??
					return bounds;
				}

		private void AddLine(int value)
				{
					// ensure the capacity of the lines array
					if(count == lines.Length)
					{
						// create the new lines array
						int[] newLines = new int[lines.Length * 2];

						// copy the line data to the new array
						Array.Copy(lines, newLines, lines.Length);

						// reset the lines array to the new array
						lines = newLines;
					}

					// add the line
					lines[count++] = value;
				}

		private int CurrentLine
				{
					get
					{
						// return minus one if past the last line
						if(currentLine > count) { return -1; }

						// return the current line
						return lines[currentLine];
					}
				}

		// Measures one full line. Updates the positions of the characters in that line relative to 0,0
		private void MeasureLine
					(ref Rectangle[] bounds, ref int currentPos,
					 ref string text, float maxX, Graphics g, Font f,
					 bool vertical, bool noWrap)
				{
					// set the initial position
					int initialPos = currentPos;

					// set the default x position
					int x;
					if(format.IsTypographic)
					{
						x = 0;
					}
					else
					{
						x = (int)(f.SizeInPoints*g.DpiX/369.7184);
					}

					// set the default y position
					int y = 0;

					// ??
					do
					{
						// get the current character
						char c = text[currentPos];

						// check for the end of the line
						if(c == '\n')
						{
							// move past the end of the line
							currentPos++;

							// we're done
							return;
						}

						// Ignore returns
						if(c != '\r')
						{
							//TODO use Platform specific measure function & take into account kerning

							// get the size of the current character
							g.SelectFont(f);
							int charactersFitted, linesFilled;
							Size s = g.ToolkitGraphics.MeasureString(c.ToString(), null, null, out charactersFitted, out linesFilled, false);
							s.Height = f.Height;

							// ??
							int newX = x;

							// ??
							int newY = y;

							// ??
							if(vertical)
							{
								newX += s.Height;
							}
							else
							{
								newX += s.Width;
							}

							// ??
							if(newX > maxX)
							{
								// ??
								if(noWrap) { return; }

								// Backtrack to wrap the word

								// ??
								for(int i = currentPos; i > initialPos; i--)
								{
									// ??
									if(text[i] == ' ')
									{
										// Swallow the space

										// ??
										bounds[i++] = Rectangle.Empty;

										// ??
										currentPos = i;

										// ??
										return;
									}
								}

								// ??
								return;
							}
							else
							{
								// ??
								if(vertical)
								{
									bounds[currentPos] = new Rectangle
										(y, x, s.Height, s.Width - 1);
								}
								else
								{
									bounds[currentPos] = new Rectangle
										(x, y, s.Width, s.Height - 1);
								}
							}

							// ??
							x = newX;
						}

						// ??
						currentPos++;
					}
					while (currentPos < text.Length);
				}

	}; // class StringMeasurePositionCalculator

	// Measure the character ranges for a string.
	public Region[] MeasureCharacterRanges(String text, Font font, RectangleF layoutRect, StringFormat stringFormat)
			{
				StringMeasurePositionCalculator calculator = new StringMeasurePositionCalculator(this, text, font, Rectangle.Truncate(layoutRect), stringFormat);		
				return calculator.GetRegions();
			}

	// Non Microsoft
	public Rectangle[] MeasureCharacters(String text, Font font, RectangleF layoutRect, StringFormat stringFormat)
			{
				StringMeasurePositionCalculator calculator = new StringMeasurePositionCalculator(this, text, font, Rectangle.Truncate(layoutRect), stringFormat);		
				return calculator.GetCharBounds();
			}

	// Measure the size of a string.
	public SizeF MeasureString(String text, Font font)
			{		
				return MeasureString(text, font, new SizeF(0.0f, 0.0f), null);
			}
	public SizeF MeasureString(String text, Font font, int width)
			{
				return MeasureString
					(text, font, new SizeF(width, 999999.0f), null);
			}
	public SizeF MeasureString(String text, Font font, SizeF layoutArea)
			{
				return MeasureString(text, font, layoutArea, null);
			}
	public SizeF MeasureString(String text, Font font,
							   int width, StringFormat format)
			{
				return MeasureString
					(text, font, new SizeF(width, 999999.0f), format);
			}
	public SizeF MeasureString(String text, Font font,
							   PointF origin, StringFormat format)
			{
				return MeasureString
					(text, font, new SizeF(0.0f, 0.0f), format);
			}
	public SizeF MeasureString(String text, Font font,
							   SizeF layoutArea, StringFormat format)
			{
				int charactersFitted, linesFilled;
				return MeasureString
					(text, font, layoutArea, format, out charactersFitted,
					 out linesFilled);
			}
	public SizeF MeasureString(String text, Font font, SizeF layoutArea,
	                           StringFormat format, out int charactersFitted,
	                           out int linesFilled)
			{
				// bail out now if there's nothing to measure
				if(((Object)text) == null || text.Length == 0)
				{
					charactersFitted = 0;
					linesFilled = 0;
					return new SizeF(0.0f, 0.0f);
				}

				// ensure we have a string format
				if(format == null)
				{
					format = new StringFormat();
				}

				// select the font
				SelectFont(font);

				// measure the string
				Size size = ToolkitGraphics.MeasureString
					(text, null, null, out charactersFitted,
					 out linesFilled, false);

				// determine if the string contains a new line
				bool containsNL =
					(text.IndexOfAny(new char[] { '\r', '\n' }) >= 0);

				// get the layout width
				float width = layoutArea.Width;

				// constant that is added (only for non-typographic formats)
				float inset;
				if(format.IsTypographic)
				{
					inset = 0;
				}
				else
				{
					inset = font.SizeInPoints*DpiX/184.8592F;
				}

				// return the size information based on wrapping behavior
				if((format.FormatFlags & StringFormatFlags.NoWrap) == 0 &&
				   ((size.Width >= width && width != 0.0f) || containsNL))
				{
					// create the layout rectangle
					Rectangle layout = new Rectangle
						(0, 0, (int)width, (int)layoutArea.Height);

					// declare the drawing position calculator
					StringDrawPositionCalculator calculator;

					// create the drawing position calculator
					calculator = new StringDrawPositionCalculator
						(text, this, font, layout , format);

					// calculate the layout of the text
					calculator.LayoutByWords();

					// calculate and return the bounds of the text
					SizeF s = calculator.GetBounds
						(out charactersFitted, out linesFilled);
					s.Width += inset;
					return s;
				}
				else
				{
					// NOTE: we use the font height here, rather than
					//       the height returned by the toolkit, since
					//       the toolkit returns the actual height of
					//       the text but the expected behavior is that
					//       the height be the font height and the width
					//       is all that is actually measured

					// set the number of characters fitted
					charactersFitted = text.Length;

					// set the number of lines filled
					linesFilled = 1;

					// return the size of the text
					return new SizeF(size.Width + inset, font.Height);
				}
			}

	// Multiply the transformation matrix by a specific amount.
	public void MultiplyTransform(Matrix matrix)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Multiply(matrix);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}
	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Multiply(matrix, order);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}

	// Release a HDC that was obtained via a previous call to "GetHdc".
	public void ReleaseHdc(IntPtr hdc)
			{
				lock(this)
				{
					if(graphics != null)
					{
						graphics.ReleaseHdc(hdc);
					}
				}
			}
	public void ReleaseHdcInternal(IntPtr hdc)
			{
				ReleaseHdc(hdc);
			}

	// Reset the clipping region.
	public void ResetClip()
			{
				Clip = new Region();
				UpdateClip();
			}

	// Reset the transformation matrix to identity.
	public void ResetTransform()
			{
				transform = null;
				Clip = clip;
			}

	// Restore to a previous save point.
	public void Restore(GraphicsState gstate)
			{
				if(gstate != null)
				{
					lock(this)
					{
						gstate.Restore(this);
					}
				}
			}

	// Apply a rotation to the transformation matrix.
	public void RotateTransform(float angle)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Rotate(angle);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}
	public void RotateTransform(float angle, MatrixOrder order)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Rotate(angle, order);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}

	// Save the current graphics state.
	public GraphicsState Save()
			{
				lock(this)
				{
					return new GraphicsState(this);
				}
			}

	// Apply a scaling factor to the transformation matrix.
	public void ScaleTransform(float sx, float sy)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Scale(sx, sy);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}
	public void ScaleTransform(float sx, float sy, MatrixOrder order)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Scale(sx, sy, order);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}

	// Set the clipping region of this graphics object.
	public void SetClip(Graphics g)
			{
				if(g == null)
				{
					throw new ArgumentNullException("g");
				}
				clip = g.Clip.Clone();
				UpdateClip();
			}
	public void SetClip(Graphics g, CombineMode combineMode)
			{
				if(g == null)
				{
					throw new ArgumentNullException("g");
				}
				Region other = g.Clip;
				switch(combineMode)
				{
					case CombineMode.Replace:
						clip = other.Clone(); break;

					case CombineMode.Intersect:
					{
						Clip.Intersect(other);
					}
					break;

					case CombineMode.Union:
					{
						Clip.Union(other);
					}
					break;

					case CombineMode.Xor:
					{
						Clip.Xor(other);
					}
					break;

					case CombineMode.Exclude:
					{
						Clip.Exclude(other);
					}
					break;

					case CombineMode.Complement:
					{
						Clip.Complement(other);
					}
					break;

					default: return;
				}
				UpdateClip();
			}
	public void SetClip(GraphicsPath path)
			{
				clip = new Region(path);
				UpdateClip();
			}
	public void SetClip(GraphicsPath path, CombineMode combineMode)
			{
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				switch(combineMode)
				{
					case CombineMode.Replace:
					{
						clip = new Region(path);
					}
					break;

					case CombineMode.Intersect:
					{
						Clip.Intersect(path);
					}
					break;

					case CombineMode.Union:
					{
						Clip.Union(path);
					}
					break;

					case CombineMode.Xor:
					{
						Clip.Xor(path);
					}
					break;

					case CombineMode.Exclude:
					{
						Clip.Exclude(path);
					}
					break;

					case CombineMode.Complement:
					{
						Clip.Complement(path);
					}
					break;

					default: return;
				}
				UpdateClip();
			}
	public void SetClip(Rectangle rect)
			{
				clip = new Region(rect);
				UpdateClip();
			}
	public void SetClip(Rectangle rect, CombineMode combineMode)
			{
				switch(combineMode)
				{
					case CombineMode.Replace:
					{
						clip = new Region(rect);
					}
					break;

					case CombineMode.Intersect:
					{
						Clip.Intersect(rect);
					}
					break;

					case CombineMode.Union:
					{
						Clip.Union(rect);
					}
					break;

					case CombineMode.Xor:
					{
						Clip.Xor(rect);
					}
					break;

					case CombineMode.Exclude:
					{
						Clip.Exclude(rect);
					}
					break;

					case CombineMode.Complement:
					{
						Clip.Complement(rect);
					}
					break;

					default: return;
				}
				UpdateClip();
			}
	public void SetClip(RectangleF rect)
			{
				clip = new Region(rect);
				UpdateClip();
			}
	public void SetClip(RectangleF rect, CombineMode combineMode)
			{
				switch(combineMode)
				{
					case CombineMode.Replace:
					{
						clip = new Region(rect);
					}
					break;

					case CombineMode.Intersect:
					{
						Clip.Intersect(rect);
					}
					break;

					case CombineMode.Union:
					{
						Clip.Union(rect);
					}
					break;

					case CombineMode.Xor:
					{
						Clip.Xor(rect);
					}
					break;

					case CombineMode.Exclude:
					{
						Clip.Exclude(rect);
					}
					break;

					case CombineMode.Complement:
					{
						Clip.Complement(rect);
					}
					break;

					default: return;
				}
				UpdateClip();
			}
	public void SetClip(Region region, CombineMode combineMode)
			{
				if(region == null)
				{
					throw new ArgumentNullException("region");
				}
				switch(combineMode)
				{
					case CombineMode.Replace:
					{
						clip = region.Clone();
					}
					break;

					case CombineMode.Intersect:
					{
						Clip.Intersect(region);
					}
					break;

					case CombineMode.Union:
					{
						Clip.Union(region);
					}
					break;

					case CombineMode.Xor:
					{
						Clip.Xor(region);
					}
					break;

					case CombineMode.Exclude:
					{
						Clip.Exclude(region);
					}
					break;

					case CombineMode.Complement:
					{
						Clip.Complement(region);
					}
					break;

					default: return;
				}
				UpdateClip();
			}

	internal void SetClipInternal(Region region)
	{
		clip = region;
	}

	// Transform points from one co-ordinate space to another.
	public void TransformPoints(CoordinateSpace destSpace,
		CoordinateSpace srcSpace,
		Point[] pts)
			{
				float x;
				float y;
				
				if (srcSpace == CoordinateSpace.Device)
				{
					if (destSpace != CoordinateSpace.Device)
					{
						// Convert from Device to Page.
						for (int i = 0; i < pts.Length; i++)
						{
							if (pageUnit == GraphicsUnit.Pixel)
								continue;
							x = pts[i].X;
							y = pts[i].Y;
							// Apply the page unit to get page co-ordinates.
							switch(pageUnit)
							{
								case GraphicsUnit.Display:
								{
									x /= DpiX / 75.0f;
									y /= DpiY / 75.0f;
								}
									break;

								case GraphicsUnit.Point:
								{
									x /= DpiX / 72.0f;
									y /= DpiY / 72.0f;
								}
									break;

								case GraphicsUnit.Inch:
								{
									x /= DpiX;
									y /= DpiY;
								}
									break;

								case GraphicsUnit.Document:
								{
									x /= DpiX / 300.0f;
									y /= DpiY / 300.0f;
								}
									break;

								case GraphicsUnit.Millimeter:
								{
									x /= DpiX / 25.4f;
									y /= DpiY / 25.4f;
								}
									break;
								default:
									break;
							}

							// Apply the inverse of the page scale factor.
							if(pageScale != 1.0f)
							{
								x /= pageScale;
								y /= pageScale;
							}
							pts[i] = new Point((int)x, (int)y);
						}
					} // destSpace != CoordinateSpace.Device
					srcSpace = CoordinateSpace.Page;
				}
				if (srcSpace == CoordinateSpace.World)
				{
					if (destSpace == CoordinateSpace.World)
						return;
					// Convert from World to Page.
					if (transform != null)
						transform.TransformPoints(pts);

					srcSpace = CoordinateSpace.Page;
				}
				if (srcSpace == CoordinateSpace.Page)
				{
					if (destSpace == CoordinateSpace.World && transform != null)
					{
						// Convert from Page to World.
						// Apply the inverse of the world transform.
						Matrix invert = new Matrix(transform);
						invert.Invert();
						invert.TransformPoints(pts);
					}
					if (destSpace == CoordinateSpace.Device)
					{
						// Convert from Page to Device.
						// Apply the page scale factor.
						for (int i = 0; i < pts.Length; i++)
						{
							x = pts[i].X;
							y = pts[i].Y;
							if(pageScale != 1.0f)
							{
								x *= pageScale;
								y *= pageScale;
							}

							// Apply the page unit to get device co-ordinates.
							switch(pageUnit)
							{
								case GraphicsUnit.Display:
								{
									x *= DpiX / 75.0f;
									y *= DpiY / 75.0f;
								}
									break;

								case GraphicsUnit.Point:
								{
									x *= DpiX / 72.0f;
									y *= DpiY / 72.0f;
								}
									break;

								case GraphicsUnit.Inch:
								{
									x *= DpiX;
									y *= DpiY;
								}
									break;

								case GraphicsUnit.Document:
								{
									x *= DpiX / 300.0f;
									y *= DpiY / 300.0f;
								}
									break;

								case GraphicsUnit.Millimeter:
								{
									x *= DpiX / 25.4f;
									y *= DpiY / 25.4f;
								}
									break;
								case GraphicsUnit.World:
								case GraphicsUnit.Pixel:
								default:
									break;

							}
							pts[i] = new Point((int)x, (int)y);
						}
					}
				}
			}

	public void TransformPoints(CoordinateSpace destSpace,
		CoordinateSpace srcSpace,
		PointF[] pts)
			{
				float x;
				float y;
				
				if (srcSpace == CoordinateSpace.Device)
				{
					if (destSpace != CoordinateSpace.Device)
					{
						// Convert from Device to Page.
						for (int i = 0; i < pts.Length; i++)
						{
							if (pageUnit == GraphicsUnit.Pixel)
								continue;
							x = pts[i].X;
							y = pts[i].Y;
							// Apply the page unit to get page co-ordinates.
							switch(pageUnit)
							{
								case GraphicsUnit.Display:
								{
									x /= DpiX / 75.0f;
									y /= DpiY / 75.0f;
								}
									break;

								case GraphicsUnit.Point:
								{
									x /= DpiX / 72.0f;
									y /= DpiY / 72.0f;
								}
									break;

								case GraphicsUnit.Inch:
								{
									x /= DpiX;
									y /= DpiY;
								}
									break;

								case GraphicsUnit.Document:
								{
									x /= DpiX / 300.0f;
									y /= DpiY / 300.0f;
								}
									break;

								case GraphicsUnit.Millimeter:
								{
									x /= DpiX / 25.4f;
									y /= DpiY / 25.4f;
								}
									break;
								default:
									break;
							}

							// Apply the inverse of the page scale factor.
							if(pageScale != 1.0f)
							{
								x /= pageScale;
								y /= pageScale;
							}
							pts[i] = new PointF(x, y);
						}
					} // destSpace != CoordinateSpace.Device
					srcSpace = CoordinateSpace.Page;
				}
				if (srcSpace == CoordinateSpace.World)
				{
					if (destSpace == CoordinateSpace.World)
						return;
					// Convert from World to Page.
					if (transform != null)
						transform.TransformPoints(pts);

					srcSpace = CoordinateSpace.Page;
				}
				if (srcSpace == CoordinateSpace.Page)
				{
					if (destSpace == CoordinateSpace.World && transform != null)
					{
						// Convert from Page to World.
						// Apply the inverse of the world transform.
						Matrix invert = new Matrix(transform);
						invert.Invert();
						invert.TransformPoints(pts);
					}
					if (destSpace == CoordinateSpace.Device)
					{
						// Convert from Page to Device.
						// Apply the page scale factor.
						for (int i = 0; i < pts.Length; i++)
						{
							x = pts[i].X;
							y = pts[i].Y;
							if(pageScale != 1.0f)
							{
								x *= pageScale;
								y *= pageScale;
							}

							// Apply the page unit to get device co-ordinates.
							switch(pageUnit)
							{
								case GraphicsUnit.Display:
								{
									x *= DpiX / 75.0f;
									y *= DpiY / 75.0f;
								}
									break;

								case GraphicsUnit.Point:
								{
									x *= DpiX / 72.0f;
									y *= DpiY / 72.0f;
								}
									break;

								case GraphicsUnit.Inch:
								{
									x *= DpiX;
									y *= DpiY;
								}
									break;

								case GraphicsUnit.Document:
								{
									x *= DpiX / 300.0f;
									y *= DpiY / 300.0f;
								}
									break;

								case GraphicsUnit.Millimeter:
								{
									x *= DpiX / 25.4f;
									y *= DpiY / 25.4f;
								}
									break;
								case GraphicsUnit.World:
								case GraphicsUnit.Pixel:
								default:
									break;

							}
							pts[i] = new PointF(x, y);
						}
					}
				}
			}

	// Translate the clipping region by a specified amount.
	public void TranslateClip(int dx, int dy)
			{
				Region clip = Clip;
				clip.Translate(dx, dy);
				Clip = clip;
			}
	public void TranslateClip(float dx, float dy)
			{
				Region clip = Clip;
				clip.Translate(dx, dy);
				Clip = clip;
			}

	// Apply a translation to the transformation matrix.
	public void TranslateTransform(float dx, float dy)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Translate(dx, dy);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}
	public void TranslateTransform(float dx, float dy, MatrixOrder order)
			{
                                // Do not use the property Transform directly, because Transform should return a Copy
                                transform = new Matrix(Transform);
				transform.Translate(dx, dy, order);
				if(transform.IsIdentity)
				{
					transform = null;
				}
			}

	// Delegate that is used to handle abort callbacks on "DrawImage".
#if !ECMA_COMPAT
	[Serializable]
	[ComVisible(false)]
#endif
	public delegate bool DrawImageAbort(IntPtr callbackdata);

	// Delegate that is used to enumerate metafile contents.
#if !ECMA_COMPAT
	[Serializable]
	[ComVisible(false)]
#endif
	public delegate bool EnumerateMetafileProc
			(EmfPlusRecordType recordType, int flags, int dataSize,
			 IntPtr data, PlayRecordCallback callbackData);

	private void BaseOffsetPoints(Point[] points)
			{
				for (int i = 0; i < points.Length; i++)
				{
					Point p = points[i];
					p.X += baseWindow.X;
					p.Y += baseWindow.Y;
					points[i] = p;
				}
			}
	
	// Convert a point into device pixels.
	private void ConvertPoint(ref int x, ref int y, GraphicsUnit graphicsUnit)
			{
				float newX, newY;
				float adjustX, adjustY;

				// Apply the world transform first.
				if(transform != null)
				{
					transform.TransformPoint(x, y, out newX, out newY);
				}
				else
				{
					newX = (float)x;
					newY = (float)y;
				}

				// Apply the page scale factor.
				if(pageScale != 1.0f)
				{
					newX *= pageScale;
					newY *= pageScale;
				}

				// Apply the page unit to get device co-ordinates.
				switch(graphicsUnit)
				{
					case GraphicsUnit.World:
					case GraphicsUnit.Pixel:
					default:
					{
						// We are finished - no more adjustments are necessary.
						x = (int)newX;
						y = (int)newY;
						return;
					}
						// Not reached.

					case GraphicsUnit.Display:
					{
						adjustX = DpiX / 75.0f;
						adjustY = DpiY / 75.0f;
					}
						break;

					case GraphicsUnit.Point:
					{
						adjustX = DpiX / 72.0f;
						adjustY = DpiY / 72.0f;
					}
						break;

					case GraphicsUnit.Inch:
					{
						adjustX = DpiX;
						adjustY = DpiY;
					}
						break;

					case GraphicsUnit.Document:
					{
						adjustX = DpiX / 300.0f;
						adjustY = DpiY / 300.0f;
					}
						break;

					case GraphicsUnit.Millimeter:
					{
						adjustX = DpiX / 25.4f;
						adjustY = DpiY / 25.4f;
					}
						break;
				}
				x = (int)(newX * adjustX);
				y = (int)(newY * adjustY);
			}

	private void ConvertPoint(float x, float y, out int dx, out int dy, GraphicsUnit graphicsUnit)
			{
				float newX, newY;
				float adjustX, adjustY;

				// Apply the world transform first.
				if(transform != null)
				{
					transform.TransformPoint(x, y, out newX, out newY);
				}
				else
				{
					newX = x;
					newY = y;
				}

				// Apply the page scale factor.
				if(pageScale != 1.0f)
				{
					newX *= pageScale;
					newY *= pageScale;
				}

				// Apply the page unit to get device co-ordinates.
				switch(graphicsUnit)
				{
					case GraphicsUnit.World:
					case GraphicsUnit.Pixel:
					default:
					{
						// We are finished - no more adjustments are necessary.
						dx = (int)newX;
						dy = (int)newY;
						return;
					}
						// Not reached.

					case GraphicsUnit.Display:
					{
						adjustX = DpiX / 75.0f;
						adjustY = DpiY / 75.0f;
					}
						break;

					case GraphicsUnit.Point:
					{
						adjustX = DpiX / 72.0f;
						adjustY = DpiY / 72.0f;
					}
						break;

					case GraphicsUnit.Inch:
					{
						adjustX = DpiX;
						adjustY = DpiY;
					}
						break;

					case GraphicsUnit.Document:
					{
						adjustX = DpiX / 300.0f;
						adjustY = DpiY / 300.0f;
					}
						break;

					case GraphicsUnit.Millimeter:
					{
						adjustX = DpiX / 25.4f;
						adjustY = DpiY / 25.4f;
					}
						break;
				}
				dx = (int)(newX * adjustX);
				dy = (int)(newY * adjustY);
			}

	// Convert a list of points into device pixels.
	private Point[] ConvertPoints(Point[] points, int minPoints, GraphicsUnit unit)
			{
				// Validate the parameter.
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				if(points.Length < minPoints)
				{
					throw new ArgumentException
						(String.Format
							(S._("Arg_NeedsAtLeastNPoints"), minPoints));
				}
		
				// Convert the "points" array.
				Point[] newPoints = new Point [points.Length];
				int x, y;
				int posn;
				for(posn = 0; posn < points.Length; ++posn)
				{
					x = points[posn].X;
					y = points[posn].Y;
					ConvertPoint(ref x, ref y, unit);
					newPoints[posn] = new Point(x, y);
				}
				return newPoints;
			}
	private Point[] ConvertPoints(PointF[] points, int minPoints, GraphicsUnit unit)
			{
				// Validate the parameter.
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				if(points.Length < minPoints)
				{
					throw new ArgumentException
						(String.Format
							(S._("Arg_NeedsAtLeastNPoints"), minPoints));
				}

				// Convert the "points" array.
				Point[] newPoints = new Point [points.Length];
				int x, y;
				int posn;
				for(posn = 0; posn < points.Length; ++posn)
				{
					ConvertPoint(points[posn].X, points[posn].Y, out x, out y, unit);	
					newPoints[posn] = new Point(x, y);
				}
				return newPoints;
			}

	// Convert a rectangle into a set of 3 device co-ordinates.
	// The result may be a parallelogram, not a rectangle.
	private Point[] ConvertRectangle3(int x, int y, int width, int height, GraphicsUnit unit)
			{
				Point[] points = new Point[3];
				int pt1x, pt1y, pt2x, pt2y, pt3x, pt3y;
				pt1x = x;
				pt1y = y;
				pt2x = x + width;
				pt2y = y;
				pt3x = x;
				pt3y = y + height;
				if(transform != null || pageScale != 1.0f ||
				   (unit != GraphicsUnit.Pixel && unit != GraphicsUnit.World))
				{
					ConvertPoint(ref pt1x, ref pt1y, unit);
					ConvertPoint(ref pt2x, ref pt2y, unit);
					ConvertPoint(ref pt3x, ref pt3y, unit);
				}
				points[0] = new Point(pt1x, pt1y);
				points[1] = new Point(pt2x, pt2y);
				points[2] = new Point(pt3x, pt3y);
				return points;
			}
	private Point[] ConvertRectangle3(float x, float y, float width, float height, GraphicsUnit unit)
			{
				Point[] points = new Point[3];
				int pt1x, pt1y, pt2x, pt2y, pt3x, pt3y;
				ConvertPoint(x, y, out pt1x, out pt1y, unit);
				ConvertPoint(x + width, y, out pt2x, out pt2y, unit);
				ConvertPoint(x, y + height, out pt3x, out pt3y, unit);
				points[0] = new Point(pt1x, pt1y);
				points[1] = new Point(pt2x, pt2y);
				points[2] = new Point(pt3x, pt3y);
				return points;
			}

	// Convert a rectangle into a set of 4 device co-ordinates.
	// The result may be a parallelogram, not a rectangle.
	private Point[] ConvertRectangle(int x, int y,
				int width, int height, GraphicsUnit unit)
			{
				Point[] points = new Point[4];
				points[0] = new Point(x, y);
				points[1] = new Point(x + width, y);
				points[2] = new Point(x + width, y + height);
				points[3] = new Point(x, y + height);
				return ConvertPoints(points, 4, unit);
			}
	private Point[] ConvertRectangle(float x, float y,
									 float width, float height, GraphicsUnit unit)
			{
				PointF[] points = new PointF[4];
				points[0] = new PointF(x, y);
				points[1] = new PointF(x + width, y);
				points[2] = new PointF(x + width, y + height);
				points[3] = new PointF(x, y + height);
				return ConvertPoints(points, 4, unit);
			}

	// Convert a size value from device co-ordinates to graphics units.
	private SizeF ConvertSizeBack(int width, int height)
			{
				float newX, newY, adjustX, adjustY;

				// Bail out early if the context is using pixel units.
				if(IsPixelUnits())
				{
					return new SizeF((float)width, (float)height);
				}

				// Apply the page unit to get page co-ordinates.
				newX = (float)width;
				newY = (float)height;
				switch(pageUnit)
				{
					case GraphicsUnit.World:
					case GraphicsUnit.Pixel:
					default:
					{
						adjustX = 1.0f;
						adjustY = 1.0f;
					}
					break;

					case GraphicsUnit.Display:
					{
						adjustX = DpiX / 75.0f;
						adjustY = DpiY / 75.0f;
					}
					break;

					case GraphicsUnit.Point:
					{
						adjustX = DpiX / 72.0f;
						adjustY = DpiY / 72.0f;
					}
					break;

					case GraphicsUnit.Inch:
					{
						adjustX = DpiX;
						adjustY = DpiY;
					}
					break;

					case GraphicsUnit.Document:
					{
						adjustX = DpiX / 300.0f;
						adjustY = DpiY / 300.0f;
					}
					break;

					case GraphicsUnit.Millimeter:
					{
						adjustX = DpiX / 25.4f;
						adjustY = DpiY / 25.4f;
					}
					break;
				}
				newX /= adjustX;
				newY /= adjustY;

				// Apply the inverse of the page scale factor.
				if(pageScale != 1.0f)
				{
					newX /= pageScale;
					newY /= pageScale;
				}

				// Apply the inverse of the world transform.
				if(transform != null)
				{
					transform.TransformSizeBack
						(newX, newY, out adjustX, out adjustY);
					return new SizeF(adjustX, adjustY);
				}
				else
				{
					return new SizeF(newX, newY);
				}
			}

	// Convert a rectangle into a set of 3 device unit co-ordinates.
	private Point[] ConvertUnits(int x, int y, int width, int height,
	                             GraphicsUnit unit)
			{
				// get the unit adjustment vector
				PointF adjust = ConvertUnitsAdjustVector(unit);

				// calculate the unit adjusted rectangle bounds
				int x0 = (int)((x +      0) * adjust.X);
				int x1 = (int)((x +  width) * adjust.X);
				int y0 = (int)((y +      0) * adjust.Y);
				int y1 = (int)((y + height) * adjust.Y);

				// create the unit adjusted rectangle points
				Point[] points = new Point[]
				{
					new Point(x0, y0),
					new Point(x1, y0),
					new Point(x0, y1)
				};

				// return the unit adjusted rectangle points
				return points;
			}
	private Point[] ConvertUnits(float x, float y, float width, float height,
	                             GraphicsUnit unit)
			{
				// get the unit adjustment vector
				PointF adjust = ConvertUnitsAdjustVector(unit);

				// calculate the unit adjusted rectangle bounds
				int x0 = (int)((x +      0) * adjust.X);
				int x1 = (int)((x +  width) * adjust.X);
				int y0 = (int)((y +      0) * adjust.Y);
				int y1 = (int)((y + height) * adjust.Y);

				// create the unit adjusted rectangle points
				Point[] points = new Point[]
				{
					new Point(x0, y0),
					new Point(x1, y0),
					new Point(x0, y1)
				};

				// return the unit adjusted rectangle points
				return points;
			}

	// Get the unit adjustment vector for the ConvertUnits methods.
	private PointF ConvertUnitsAdjustVector(GraphicsUnit unit)
			{
				// create and return the unit adjustment vector
				switch(unit)
				{
					case GraphicsUnit.World:
					case GraphicsUnit.Pixel:
					default:
						{ return new PointF(1.0f, 1.0f); }

					case GraphicsUnit.Display:
						{ return new PointF((DpiX / 75.0f), (DpiY / 75.0f)); }

					case GraphicsUnit.Point:
						{ return new PointF((DpiX / 72.0f), (DpiY / 72.0f)); }

					case GraphicsUnit.Inch:
						{ return new PointF(DpiX, DpiY); }

					case GraphicsUnit.Document:
						{ return new PointF((DpiX / 300.0f), (DpiY / 300.0f)); }

					case GraphicsUnit.Millimeter:
						{ return new PointF((DpiX / 25.4f), (DpiY / 25.4f)); }
				}
			}


	// Get the toolkit graphics object underlying this object.
	internal IToolkitGraphics ToolkitGraphics
			{
				get
				{
					if(graphics != null)
					{
						return graphics;
					}
					throw new ObjectDisposedException("graphics");
				}
			}

	// Get the default graphics object for the current toolkit.
	internal static Graphics DefaultGraphics
			{
				get
				{
					return defaultGraphics;
				}
			}

	// Select a pen into the toolkit graphics object.
	private void SelectPen(Pen pen)
			{
				if(pen == null)
				{
					throw new ArgumentNullException("pen");
				}
				if(graphics == null)
				{
					throw new ObjectDisposedException("graphics");
				}
				
				Pen penNew = pen;
				if( transform != null ) {
					// calculation new pen size, if a transformation is set
					// using the workaround for Font scaling
					float penWidth = pen.Width;
					float newWidth = transform.TransformFontSize(penWidth);
					if( penWidth != newWidth ) {
						penNew = (Pen) pen.Clone();
						penNew.Width = newWidth;
					}
				}
				
				IToolkitPen tpen = penNew.GetPen(graphics.Toolkit);
				if(penNew.PenType == PenType.SolidColor)
				{
					tpen.Select(graphics);
				}
				else
				{
					IToolkitBrush tbrush = penNew.Brush.GetBrush(graphics.Toolkit);
					tpen.Select(graphics, tbrush);
				}
			}

	// Select a brush into the toolkit graphics object.
	private void SelectBrush(Brush brush)
			{
				if(brush == null)
				{
					throw new ArgumentNullException("brush");
				}
				if(graphics == null)
				{
					throw new ObjectDisposedException("graphics");
				}
				IToolkitBrush tbrush = brush.GetBrush(graphics.Toolkit);
				tbrush.Select(graphics);
			}

	// Select a font into the toolkit graphics object.
	private void SelectFont(Font font)
			{
				if(font == null)
				{
					throw new ArgumentNullException("font");
				}
				if(graphics == null)
				{
					throw new ObjectDisposedException("graphics");
				}
				IToolkitFont tfont = font.GetFont(graphics.Toolkit, DpiY);
				tfont.Select(graphics);
			}

	// Update the clipping region within the IToolkitGraphics object.
	private void UpdateClip()
			{
				RectangleF[] rectsF;
				if (transform == null && pageScale == 1.0f && pageUnit == GraphicsUnit.World)
				{
					rectsF = clip.GetRegionScansIdentity();
				}
				else
				{
					rectsF = clip.GetRegionScans(Transform);
				}
				int left = int.MaxValue;
				int top = int.MaxValue;
				int right = int.MinValue;
				int bottom = int.MinValue;
				Rectangle[] rects = new Rectangle[rectsF.Length];
				for(int i=0;i < rectsF.Length; i++)
				{
					Rectangle r = Rectangle.Truncate(rectsF[i]);
					if (baseWindow != Rectangle.Empty)
					{
						r.Offset(baseWindow.Location);
						r.Intersect(baseWindow);
					}
					rects[i] = r;
					if (left > r.Left)
						left = r.Left;
					if (right < r.Right)
						right = r.Right;
					if (top > r.Top)
						top = r.Top;
					if (bottom < r.Bottom)
						bottom = r.Bottom;
				}
				graphics.SetClipRects(rects);
				deviceClipExtent = Rectangle.FromLTRB(left, top, right, bottom);

			}

	// Determine if this graphics object is using 1-to-1 pixel mappings.
	internal bool IsPixelUnits()
			{
				if((pageUnit == GraphicsUnit.Pixel ||
				    pageUnit == GraphicsUnit.World) &&
				   transform == null && pageScale == 1.0f)
				{
					return true;
				}
				return false;
			}

	// Get the line spacing of a font, in pixels.
	internal int GetLineSpacing(Font font)
			{
				lock(this)
				{
					SelectFont(font);
					return ToolkitGraphics.GetLineSpacing();
				}
			}

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	internal void DrawGlyph(int x, int y,
						    byte[] bits, int bitsWidth, int bitsHeight,
						    Color color)
			{
				// Bail out now if there's nothing to draw.
				if(color.A == 0) { return; }

				int dx, dy;
				ConvertPoint(x, y, out dx, out dy, pageUnit);
				lock(this)
				{
					ToolkitGraphics.DrawGlyph
						(dx + baseWindow.X, dy + baseWindow.Y,
						 bits, bitsWidth, bitsHeight, color);
				}
			}












































#region    /* TextLayoutManager */

	// Each span of characters we are measuring or drawing.
	private sealed class CharSpan
	{
		// Publicly-accessible state.
		public int start;
		public int length;
		public int pixelWidth;
		public bool newline;


		// Constructor.
		public CharSpan()
				{
					this.start = 0;
					this.length = 0;
					this.pixelWidth = -1;
					this.newline = false;
				}


		// Copy the values of this character span to the given span.
		public void CopyTo(CharSpan span)
				{
					span.start = start;
					span.length = length;
					span.pixelWidth = pixelWidth;
					span.newline = newline;
				}

		// Set the values of this character span.
		public void Set(int start, int length, bool newline)
				{
					this.start = start;
					this.length = length;
					this.pixelWidth = -1;
					this.newline = newline;
				}

	}; // class CharSpan



	// Each span of a full line for center and bottom line justified text.
	private struct LineSpan
	{
		// Publicly-accessible state.
		public int start;
		public int length;
		public int pixelWidth;


		// Constructor.
		public LineSpan(int start, int length, int pixelWidth)
				{
					this.start = start;
					this.length = length;
					this.pixelWidth = pixelWidth;
				}

	}; // struct LineSpan



	// Performs text layout for drawing and measuring calculations.
	private sealed class TextLayoutManager
	{
		// Internal state.
		private bool             onlyWholeLines;
		private bool             prevIsNewLine;
		private int              hotkeyIndex;
		private int              lineHeight;
		private int              lineNumber;
		private int              lineSpaceUsedUp;
		private int              nextIndex;
		private Brush            brush;
		private Font             font;
		private Font             underlineFont;
		private Graphics         graphics;
		private IToolkitGraphics toolkitGraphics;
		private Rectangle        layout;
		private String           text;
		private StringFormat     format;

		private static readonly StringFormat SF_DEFAULT = StringFormat.GenericDefault;
		private static readonly Point[] MEASURE_LAYOUT_RECT = new Point[0];


		// Constructor.
		public TextLayoutManager()
				{
					// nothing to do here
				}


		// Calculate whether a word wraps to a new line.
		private void CheckForWrap(CharSpan span)
				{
					// bail out now if there's nothing to do
					if(span.length == 0) { return; }

					// reset the line space usage, if needed
					if(span.newline) { lineSpaceUsedUp = 0; }

					// get the first character of the span
					char c = text[span.start];

					// handle unwrappable span, if needed
					if(c != ' ' &&
					   ((format.FormatFlags & StringFormatFlags.NoWrap) == 0))
					{
						// get the width of the span
						int width = GetSpanWidth(span);

						// handle wrapping of span, as needed
						if((lineSpaceUsedUp + width) > layout.Width)
						{
							// handle unwrappable span trimming, if needed
							if(width > layout.Width)
							{
								// trim the span
								span.length = TrimTextToChar
									(span.start, span.length, layout.Width,
									 out span.pixelWidth);

								// update the text position
								nextIndex = span.start + span.length;

								// set the new line flag, if needed
								if(lineNumber > 0) { span.newline = true; }

								// set the previous span ended in new line flag
								prevIsNewLine = true;
							}

							// reset line space usage
							lineSpaceUsedUp = 0;

							// set the new line flag
							span.newline = true;
						}
					}

					// update line space usage
					lineSpaceUsedUp += GetSpanWidth(span);
				}

		// Calculate and draw the string by drawing each line.
		public void Draw
					(Graphics graphics, String text, Font font,
					 Rectangle drawLayout, StringFormat format, Brush brush)
				{
					// set the current graphics
					this.graphics = graphics;

					// set the current toolkit graphics
					this.toolkitGraphics = graphics.ToolkitGraphics;

					// set the current text
					this.text = text;

					// set the current font
					this.font = font;

					// set the current layout rectangle
					this.layout = drawLayout;

					// set the current brush
					this.brush = brush;

					// ensure we have a string format
					if(format == null) { format = SF_DEFAULT; }

					// set the current string format
					this.format = format;

					// set the default hotkey index
					this.hotkeyIndex = -1;

					// set the current line height
					lineHeight = font.Height;

					// set the only whole lines flag
					onlyWholeLines = (((format.FormatFlags &
					                    StringFormatFlags.LineLimit) != 0) ||
					                  ((format.Trimming &
					                    StringTrimming.None) != 0));

					// set the index of the next character
					nextIndex = 0;

					// set the current line space usage
					lineSpaceUsedUp = 0;

					// set the current line number
					lineNumber = 0;

					// set the previous span ended in new line flag
					prevIsNewLine = false;

					// select the current font into the graphics context
					graphics.SelectFont(font);

					// select the current brush into the graphics context
					graphics.SelectBrush(brush);

					// set the current text start
					int textStart = 0;

					// set the current text length
					int textLength = 0;

					// set the current text width
					int textWidth = 0;

					// get the actual hotkey index, if needed
					if(format.HotkeyPrefix != HotkeyPrefix.None)
					{
						// get the hotkey index
						hotkeyIndex = text.IndexOf('&');

						// handle the hotkey as needed
						if(hotkeyIndex != -1)
						{
							if(hotkeyIndex >= (text.Length - 1) ||
							   Char.IsControl(text[hotkeyIndex + 1]))
							{
								// no need for this anymore
								hotkeyIndex = -1;
							}
							else
							{
								// remove the hotkey character
								text = text.Substring(0, hotkeyIndex) +
								       text.Substring(hotkeyIndex + 1);

								// set the current text
								this.text = text;

								// prepare to show or hide the underline
								if(format.HotkeyPrefix == HotkeyPrefix.Show)
								{
									// get the underline font
									underlineFont = new Font
										(font, font.Style | FontStyle.Underline);
								}
								else
								{
									// no need for this anymore
									hotkeyIndex = -1;
								}
							}
						}
					}

					// draw the text
					try
					{
						// handle drawing based on line alignment
						if(format.LineAlignment == StringAlignment.Near)
						{
							// set the current y position
							int y = layout.Top;

							// get the maximum y position
							int maxY = layout.Bottom;

							// adjust for whole lines, if needed
							if(onlyWholeLines)
							{
								maxY -= ((maxY - y) % lineHeight);
							}

							// get the last line y position
							int lastLineY = maxY - lineHeight;

							// create character spans
							CharSpan span = new CharSpan();
							CharSpan prev = new CharSpan();

							// set the first span flag
							bool firstSpan = true;

							// process the text
							while(nextIndex < text.Length)
							{
								// get the next span of characters
								GetNextSpan(span);

								// draw the pending line, as needed
								if(span.newline && !firstSpan)
								{
									// draw the line, if needed
									if(textWidth > 0)
									{
										// remove trailing spaces, if needed
										if(!firstSpan && text[prev.start] == ' ')
										{
											// update text width
											textWidth -= GetSpanWidth(prev);

											// update text length
											textLength -= prev.length;
										}

										// draw the line
										DrawLine
											(textStart, textLength, textWidth,
											 y, (y > lastLineY));
									}

									// update the y position
									y += lineHeight;

									// update the line number
									++lineNumber;

									// update the text start
									textStart = span.start;

									// reset the text length
									textLength = 0;

									// reset the text width
									textWidth = 0;
								}

								// update the text length
								textLength += span.length;

								// update the text width
								textWidth += GetSpanWidth(span);

								// copy span values to previous span
								span.CopyTo(prev);

								// set the first span flag
								firstSpan = false;

								// break if the y position is out of bounds
								if(y > maxY) { break; }
							}

							// draw the last line, if needed
							if(textWidth > 0 && y <= maxY)
							{
								// draw the last line
								DrawLine
									(textStart, textLength, textWidth, y,
									 (y > lastLineY));
							}
						}
						else
						{
							// set default lines to draw
							int linesToDraw = 0;

							// calculate lines to draw
							if(onlyWholeLines)
							{
								linesToDraw = layout.Height / lineHeight;
							} 
							else
							{
								linesToDraw = (int)Math.Ceiling((double)layout.Height / lineHeight);
							}

							// create line span list
							LineSpan[] lines = new LineSpan[linesToDraw];

							// create character spans
							CharSpan span = new CharSpan();
							CharSpan prev = new CharSpan();

							// set the first span flag
							bool firstSpan = true;

							// set the current line position
							int linePos = 0;

							// populate line span list
							while(linePos < lines.Length &&
							      nextIndex < text.Length)
							{
								// get the next span of characters
								GetNextSpan(span);

								// handle span on new line
								if(span.newline && !firstSpan)
								{
									// remove trailing spaces, if needed
									if(!firstSpan && text[prev.start] == ' ')
									{
										// update text width
										textWidth -= GetSpanWidth(prev);

										// update text length
										textLength -= prev.length;
									}

									// create line span for current line
									LineSpan lineSpan = new LineSpan
										(textStart, textLength, textWidth);

									// add current line span to line span list
									lines[linePos++] = lineSpan;

									// update text start
									textStart = span.start;

									// update text length
									textLength = 0;

									// update text width
									textWidth = 0;
								}

								// update text length
								textLength += span.length;

								// update text width
								textWidth += GetSpanWidth(span);

								// copy span values to previous span
								span.CopyTo(prev);

								// set the first span flag
								firstSpan = false;
							}

							// add the last line to the line span list
							if(linePos < lines.Length)
							{
								// create line span for last line
								LineSpan lineSpan = new LineSpan
									(textStart, textLength, textWidth);

								// add last line span to the line span list
								lines[linePos++] = lineSpan;
							}

							// calculate the top line y
							int y = (layout.Height - (linePos * lineHeight));

							// adjust y for center alignment, if needed
							if(format.LineAlignment == StringAlignment.Center)
							{
								y /= 2;
							}

							// translate y to layout rectangle
							y += layout.Top;

							// adjust line position to last line
							--linePos;

							// draw the lines
							for(int i = 0; i < linePos; ++i)
							{
								// get the current line
								LineSpan line = lines[i];

								// draw the current line
								DrawLine
									(line.start, line.length, line.pixelWidth,
									 y, false);

								// update the y position
								y += lineHeight;
							}

							// draw the last line
							DrawLine
								(lines[linePos].start, lines[linePos].length,
								 lines[linePos].pixelWidth, y, true);
						}
					}
					finally
					{
						// dispose the underline font, if we have one
						if(underlineFont != null)
						{
							// dispose the underline font
							underlineFont.Dispose();

							// reset the underline font
							underlineFont = null;
						}
					}
				}

		// Draw a line.
		private void DrawLine
					(int start, int length, int width, int y, bool lastLine)
				{
					// set default x position
					int x = 0;

					// set truncate line flag
					bool truncateLine = false;

					// update the truncate line flag, as needed
					if((lastLine && ((start + length) < text.Length)) ||
					   ((width > layout.Width) &&
					    ((format.FormatFlags & StringFormatFlags.NoWrap) != 0)))
					{
						truncateLine = true;
					}

					// handle no truncation case
					if(!truncateLine)
					{
						// update x position
						x = GetXPosition(width);

						// draw the line
						if(hotkeyIndex < start ||
						   hotkeyIndex >= (start + length))
						{
							String s = text.Substring(start, length);
							toolkitGraphics.DrawString(s, x, y, format);
						}
						else
						{
							DrawLineWithHotKey(text, start, length, x, y);
						}

						// we're done here
						return;
					}

					// declare out variables
					int cf, lf;

					// set the default ellipsis
					String ellipsis = null;

					// set the maximum width
					int maxWidth = layout.Width;

					// 
					if(format.Trimming == StringTrimming.EllipsisCharacter ||
					   format.Trimming == StringTrimming.EllipsisWord ||
					   format.Trimming == StringTrimming.EllipsisPath)
					{
						// set the ellipsis
						ellipsis = " . . . ";

						// update the maximum width, if needed
						if(format.Trimming != StringTrimming.EllipsisPath)
						{
							// update the maximum width
							maxWidth -= toolkitGraphics.MeasureString
								(ellipsis, MEASURE_LAYOUT_RECT, format,
								 out cf, out lf, false).Width;
						}
					}

					// set the default draw string
					String drawS = null;

					// trim and draw the string
					switch(format.Trimming)
					{
						case StringTrimming.None:
						case StringTrimming.EllipsisCharacter:
						case StringTrimming.Character:
						{
							// update length, if needed
							if(width > maxWidth)
							{
								// update length
								length = TrimTextToChar
									(start, length, maxWidth, out width);
							}

							// set the draw string
							drawS = text.Substring(start, length);

							// update the draw string, if needed
							if(ellipsis != null) { drawS += ellipsis; }

							// update the x position
							x = GetXPosition(width);

							// draw the line
							if(hotkeyIndex < start ||
							   hotkeyIndex >= (start + length))
							{
								// draw the line
								toolkitGraphics.DrawString(drawS, x, y, format);
							}
							else
							{
								// draw the line with hotkey underlining
								DrawLineWithHotKey(drawS, 0, drawS.Length, x, y);
							}
						}
						break;

						case StringTrimming.EllipsisWord:
						case StringTrimming.Word:
						{
							// set the draw string
							drawS = text.Substring
								(start,
									TrimTextToWord
										(start, length, maxWidth, out width));

							// update the draw string, if needed
							if(ellipsis != null) { drawS += ellipsis; }

							// update the x position
							x = GetXPosition(width);

							// draw the line
							if(hotkeyIndex < start ||
							   hotkeyIndex >= (start + length))
							{
								// draw the line
								toolkitGraphics.DrawString(drawS, x, y, format);
							}
							else
							{
								// draw the line with hotkey underlining
								DrawLineWithHotKey(drawS, 0, drawS.Length, x, y);
							}
						}
						break;

						case StringTrimming.EllipsisPath:
						{
							// set the draw string
							drawS = TrimToPath
								(start, (text.Length - start), maxWidth,
								 out width, ellipsis);

							// update the x position
							x = GetXPosition(width);

							// draw the line
							if(hotkeyIndex < start ||
							   hotkeyIndex >= (start + length))
							{
								// draw the line
								toolkitGraphics.DrawString(drawS, x, y, format);
							}
							else
							{
								// draw the line with hotkey underlining
								DrawLineWithHotKey(drawS, 0, drawS.Length, x, y);
							}
						}
						break;
					}
				}

		// Draw a line containing hotkey text.
		private void DrawLineWithHotKey
					(String text, int start, int length, int x, int y)
				{
					// declare the out variables
					int cf, lf;

					// set the default text
					String s = null;

					// draw the pre-hotkey text
					if(hotkeyIndex > start)
					{
						// get the pre-hotkey text
						s = text.Substring(start, (hotkeyIndex - start));

						// draw the pre-hotkey text
						toolkitGraphics.DrawString(s, x, y, format);

						// update the x position
						x += toolkitGraphics.MeasureString
							(s, MEASURE_LAYOUT_RECT, format, out cf, out lf,
							 false).Width;
					}

					// get the hotkey text
					s = text.Substring(hotkeyIndex, 1);

					// select the underline font
					graphics.SelectFont(underlineFont);

					// draw the hotkey text
					toolkitGraphics.DrawString(s, x, y, format);

					// revert to the regular font
					graphics.SelectFont(font);

					// update the x position
					x += toolkitGraphics.MeasureString
						(s, MEASURE_LAYOUT_RECT, format, out cf, out lf,
						 false).Width;

					// draw the post-hotkey text
					if(hotkeyIndex < ((start + length) - 1))
					{
						// get the start index of the post-hotkey text
						int index = (hotkeyIndex + 1);

						// get the length of the post-hotkey text
						length -= (index - start);

						// get the post-hotkey text
						s = text.Substring(index, length);

						// draw the post-hotkey text
						toolkitGraphics.DrawString(s, x, y, format);
					}
				}

		// Calculate text metrics information.
		//
		// Note that this is currently broken. Turn this on at your own risk.
		public Size GetBounds
					(Graphics graphics, String text, Font font,
					 SizeF layoutSize, StringFormat format,
					 out int charactersFitted, out int linesFilled)
				{
					// set the current graphics
					this.graphics = graphics;

					// set the current toolkit graphics
					this.toolkitGraphics = graphics.ToolkitGraphics;

					// set the current text
					this.text = text;

					// set the current font
					this.font = font;

					// ensure we have a string format
					if(format == null) { format = SF_DEFAULT; }

					// set the current string format
					this.format = format;

					// set the current layout rectangle
					this.layout = new Rectangle
						(0, 0, (int)layoutSize.Width, (int)layoutSize.Height);

					// set the current line height
					lineHeight = font.Height;

					// set the only whole lines flag
					onlyWholeLines = (((format.FormatFlags &
					                    StringFormatFlags.LineLimit) != 0) ||
					                  ((format.Trimming &
					                    StringTrimming.None) != 0));

					// set the index of the next character
					nextIndex = 0;

					// set the current line space usage
					lineSpaceUsedUp = 0;

					// set the previous span ended in new line flag
					prevIsNewLine = false;

					// select the current font into the graphics context
					graphics.SelectFont(font);

					// set the text width
					int textWidth = 0;

					// set the maximum width
					int maxWidth = 0;

					// set the default characters fitted
					charactersFitted = 0;

					// set the default lines filled
					linesFilled = 0;

					// remove the hotkey prefix, if needed
					if(format.HotkeyPrefix != HotkeyPrefix.None)
					{
						// get the hotkey index
						hotkeyIndex = text.IndexOf('&');

						// handle the hotkey as needed
						if(hotkeyIndex != -1)
						{
							if(hotkeyIndex < (text.Length - 1) &&
							   !Char.IsControl(text[hotkeyIndex + 1]))
							{
								// remove the hotkey character
								text = text.Substring(0, hotkeyIndex) +
								       text.Substring(hotkeyIndex + 1);

								// set the current text
								this.text = text;

								// update characters fitted
								++charactersFitted;
							}

							// no need for this anymore
							hotkeyIndex = -1;
						}
					}

					// create character spans
					CharSpan span = new CharSpan();
					CharSpan prev = new CharSpan();

					// set the first span flag
					bool firstSpan = true;

					// set the measure trailing spaces flag
					bool mts = ((format.FormatFlags &
					             StringFormatFlags.MeasureTrailingSpaces) != 0);

					// process the text
					while(nextIndex < text.Length)
					{
						// get the next span of characters
						GetNextSpan(span);

						// handle span on new line
						if(span.newline)
						{
							// remove trailing spaces, if needed
							if(!firstSpan && !mts && text[prev.start] == ' ')
							{
								// update the text width
								textWidth -= GetSpanWidth(prev);
							}

							// update the maximum width, if needed
							if(textWidth > maxWidth) { maxWidth = textWidth; }

							// update the text width
							textWidth = 0;

							// update the lines filled
							++linesFilled;
						}

						// update the text width
						textWidth += GetSpanWidth(span);

						// update the characters fitted
						charactersFitted += span.length;

						// copy span values to previous span
						span.CopyTo(prev);
					}

					// update the maximum width, if needed
					if(textWidth > maxWidth) { maxWidth = textWidth; }

					// update the lines filled to account for the first line
					++linesFilled;

					// update the maximum width, if needed
					if(maxWidth > layout.Width) { maxWidth = layout.Width; }

					// calculate the height
					int height = (lineHeight * linesFilled);

					// update the height, if needed
					if(height > layout.Height) { height = layout.Height; }

					// return the size of the text
					return new Size(maxWidth, height);
				}

		// Get the next span of characters.
		private void GetNextSpan(CharSpan span)
				{
					// set new line flag
					bool newline = false;

					// get the start index
					int start = nextIndex;

					// handle whitespace span
					while(nextIndex < text.Length && text[nextIndex] == ' ')
					{
						++nextIndex;
					}

					// handle word span
					if(nextIndex == start)
					{
						// find the end of the word
						while(nextIndex < text.Length)
						{
							// get the current character
							char c = text[nextIndex];

							// find the end of the word
							if(c == ' ' || c == '\n' || c == '\r') { break; }

							// we also split on minus to mimic MS behavior
							if(c == '-')
							{
								nextIndex++;
								break;
							}

							// move to the next character
							++nextIndex;
						}
					}

					// get the length of the span
					int length = nextIndex - start;

					// handle new line characters
					if(nextIndex < text.Length)
					{
						// get the current character
						char c = text[nextIndex];

						// check for new line characters
						if(c == '\r')
						{
							// move past the carriage return
							++nextIndex;

							// move past the line feed, if needed
							if(nextIndex < text.Length &&
							   text[nextIndex] == '\n')
							{
								++nextIndex;
							}

							// set the new line flag
							newline = true;
						}
						else if(c == '\n')
						{
							// move past the line feed
							++nextIndex;

							// set the new line flag
							newline = true;
						}
					}

					// set the span values
					span.Set(start, length, prevIsNewLine);

					// update the previous span ended in new line flag
					prevIsNewLine = newline;

					// handle wrapping
					CheckForWrap(span);
				}

		// Get the width of the span in pixels.
		private int GetSpanWidth(CharSpan span)
				{
					// set the width of the span, if needed
					if(span.pixelWidth == -1)
					{
						// get the text of the span
						String s = text.Substring(span.start, span.length);

						// declare out variables
						int cf, lf;

						// set the width of the span
						span.pixelWidth = toolkitGraphics.MeasureString
							(s, MEASURE_LAYOUT_RECT, format, out cf, out lf,
							 false).Width;
					}

					// return the width of the span
					return span.pixelWidth;
				}

		// Calculate the position of the line based on the formatting and width.
		private int GetXPosition(int width)
				{
					// set the default x position
					int x = layout.X;

					// update the x position based on alignment
					if(format.Alignment != StringAlignment.Near)
					{
						if(format.Alignment == StringAlignment.Far)
						{
							x += (layout.Width - width);
						}
						else
						{
							x += ((layout.Width - width) / 2);
						}
					}

					// return the x position
					return x;
				}

		// Trim to the nearest character.
		//
		// Returns the length of characters from the string once it is trimmed.
		// The "width" variable returns the pixel width of the trimmed string.
		private int TrimTextToChar
					(int start, int length, int maxWidth, out int width)
				{
					// declare out variables
					int cf, lf;

					// set default width
					width = 0;

					// get the current width
					int currWidth = toolkitGraphics.MeasureString
						(text.Substring(start, length), MEASURE_LAYOUT_RECT,
						 format, out cf, out lf, false).Width;

					// handle trivial case first
					if(currWidth <= maxWidth)
					{
						// set the width
						width = currWidth;

						// return the characters fitted
						return length;
					}

					// set the left boundary
					int left = 0;

					// set the right boundary
					int right = (length - 1);

					// set the best fit
					int best = 0;

					// find the maximum number of characters which fit
					while(left <= right)
					{
						// calculate the middle position
						int middle = ((left + right) / 2);

						// get the current width
						currWidth = toolkitGraphics.MeasureString
							(text.Substring(start, middle),
							 MEASURE_LAYOUT_RECT, format,
							 out cf, out lf, false).Width;

						// continue search or return depending on comparison
						if(currWidth > maxWidth)
						{
							// reposition right boundary
							right = (middle - 1);
						}
						else if(currWidth < maxWidth)
						{
							// update the best fit
							best = middle;

							// update the best fit width
							width = currWidth;

							// reposition left boundary
							left = (middle + 1);
						}
						else
						{
							// update the best fit width
							width = currWidth;

							// return the best fit
							return middle;
						}
					}

					// return the best fit
					return best;
				}

		// Trim to the path.
		//
		// Returns the trimmed string. The "width" variable returns the pixel
		// width of the trimmed string. The trimming algorithm tries to place
		// the characters removed in the center of the string but also tries
		// to guarantee that the last path seperator character is shown.
		private String TrimToPath
					(int start, int length, int maxWidth, out int width,
					 String ellipsis)
				{
					// declare out variables
					int cf, lf;

					// set the default return value
					System.Text.StringBuilder retval = new System.Text.StringBuilder(text.Substring(start, length));

					// measure the width of the return value
					width = toolkitGraphics.MeasureString
						(retval.ToString(), MEASURE_LAYOUT_RECT,
						 format, out cf, out lf, false).Width;

					// return the text if it fits
					if(width < maxWidth) { return retval.ToString(); }

					// set the middle position
					int middle = ((start + (length / 2)) + 2);

					// set the separator found flag
					bool seperatorFound = false;

					// set the remove position
					int removePos = ((start + length) - 1);

					// find the optimal remove position
					while(removePos >= start)
					{
						// get the current character
						char c = text[removePos];

						// check for separator
						if(c == '\\' || c == '/') { seperatorFound = true; }

						// break if we've found a separator before the middle
						if(seperatorFound && removePos <= middle) { break; }

						// update the remove position
						--removePos;
					}

					// remove from the middle if no separator was found
					if(!seperatorFound) { removePos = middle; }

					// set the removal start position
					int removeStart = (removePos - 1);

					// find and return the optimal trim pattern
					while(true)
					{
						// attempt to fit the ellipsis
						if(width < maxWidth)
						{
							// set the return value to the pre-removal text
							retval = new System.Text.StringBuilder(text.Substring
								(start, (removeStart - start)));

							// append the ellipsis to the return value
							retval.Append(ellipsis);

							// append the post-removal text to the return value
							retval.Append(text.Substring
								(removePos, ((start + length) - removePos)));

							// measure the width of the return value
							width = toolkitGraphics.MeasureString
								(retval.ToString(), MEASURE_LAYOUT_RECT, format,
								 out cf, out lf, false).Width;

							// return the text if it fits
							if(width < maxWidth) { return retval.ToString(); }
						}

						// set the reduced flag
						bool reduced = false;

						// attempt to reduce
						if(removeStart > start)
						{
							// measure the width of the text
							width -= toolkitGraphics.MeasureString
								(text.Substring(removeStart--, 1),
								 MEASURE_LAYOUT_RECT, format, out cf, out lf,
								 false).Width;

							// continue if no reduction is needed
							if(width < maxWidth) { continue; }

							// set the reduced flag
							reduced = true;
						}

						// attempt to reduce
						if(removePos < (start + length))
						{
							// measure the width of the text
							width -= toolkitGraphics.MeasureString
								(text.Substring(removePos++, 1),
								 MEASURE_LAYOUT_RECT, format, out cf, out lf,
								 false).Width;

							// continue if no reduction is needed
							if(width < maxWidth) { continue; }

							// set the reduced flag
							reduced = true;
						}

						// return the ellipsis, if needed
						if(!reduced)
						{
							// measure the width of the ellipsis
							width = toolkitGraphics.MeasureString
								(ellipsis, MEASURE_LAYOUT_RECT, format,
								 out cf, out lf, false).Width;

							// return the ellipsis
							return ellipsis;
						}
					}
				}

		// Trim to the nearest word or character, as appropriate.
		//
		// Returns the length of characters from the string once it is trimmed.
		// The "width" variable returns the pixel width of the trimmed string.
		// If the string has no words then it is trimmed to the nearest
		// character.
		private int TrimTextToWord
					(int start, int length, int maxWidth, out int width)
				{
					// declare out variables
					int cf, lf;

					// set the default width
					width = 0;

					// set the end position
					int end = start + length;

					// set the start position
					int pos = start;

					// set the previous position
					int prevPos = pos;

					// process the text
					while(pos < end)
					{
						// get the current character
						char c = text[pos];

						// skip over leading spaces
						if(c == ' ')
						{
							// move past space
							++pos;

							// skip over remaining spaces
							while(pos < end && text[pos] == ' ') { ++pos; }
						}

						// skip over word
						while(pos < end && text[pos] != ' ') { ++pos; }

						// get the width of the text
						int stringWidth = toolkitGraphics.MeasureString
							(text.Substring(prevPos, (pos - prevPos)),
							 MEASURE_LAYOUT_RECT, format, out cf, out lf,
							 false).Width;

						// return the characters fitted, if max width exceeded
						if((width + stringWidth) > maxWidth)
						{
							// trim within the word, if needed
							if(width == 0)
							{
								// trim within the word
								return TrimTextToChar
									(start, length, maxWidth, out width);
							}

							// return the characters fitted
							return (prevPos - start);
						}

						// update the current width
						width += stringWidth;

						// update the previous position
						prevPos = pos;
					}

					// return the characters fitted
					return length;
				}

	}; // class TextLayoutManager

#endregion /* TextLayoutManager */

}; // class Graphics

}; // namespace System.Drawing
