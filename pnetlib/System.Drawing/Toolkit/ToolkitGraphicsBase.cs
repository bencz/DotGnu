/*
 * ToolkitGraphicsBase.cs - Implementation of the
 *			"System.Drawing.Toolkit.ToolkitGraphicsBase" class.
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
using DotGNU.Images;

// This base class provides some common functionality which should
// help to make it easier to write "IToolkitGraphics" handlers.

[NonStandardExtra]
public abstract class ToolkitGraphicsBase : IToolkitGraphics
{
	// Dirty bit flags for changed values.
	[Flags]
	public enum DirtyFlags
	{
		CompositingMode		= (1 << 1),
		CompositingQuality	= (1 << 2),
		InterpolationMode	= (1 << 3),
		PixelOffsetMode		= (1 << 4),
		RenderingOrigin		= (1 << 5),
		SmoothingMode		= (1 << 6),
		TextContrast		= (1 << 7),
		TextRenderingHint	= (1 << 8),
		All					= -1

	}; // enum DirtyFlags

	// Internal state.
	protected IToolkit toolkit;
	protected Region clip;
	protected CompositingMode compositingMode;
	protected CompositingQuality compositingQuality;
	protected InterpolationMode interpolationMode;
	protected PixelOffsetMode pixelOffsetMode;
	protected Point renderingOrigin;
	protected SmoothingMode smoothingMode;
	protected int textContrast;
	protected TextRenderingHint textRenderingHint;
	protected DirtyFlags dirtyFlags;
	protected IToolkitPen pen;
	protected IToolkitBrush brush;
	protected IToolkitFont font;

	// Constructor.
	protected ToolkitGraphicsBase(IToolkit toolkit)
			{
				this.toolkit = toolkit;
				clip = null;
				compositingMode = CompositingMode.SourceOver;
				compositingQuality = CompositingQuality.Default;
				interpolationMode = InterpolationMode.Default;
				pixelOffsetMode = PixelOffsetMode.Default;
				renderingOrigin = new Point(0, 0);
				smoothingMode = SmoothingMode.Default;
				textContrast = 4;
				textRenderingHint = TextRenderingHint.SystemDefault;
				dirtyFlags = DirtyFlags.All;
			}

	// Get or set the graphics object's properties.
	public IToolkit Toolkit
			{
				get
				{
					return toolkit;
				}
			}
	public virtual CompositingMode CompositingMode
			{
				get
				{
					return compositingMode;
				}
				set
				{
					if(compositingMode != value)
					{
						compositingMode = value;
						dirtyFlags |= DirtyFlags.CompositingMode;
					}
				}
			}
	public virtual CompositingQuality CompositingQuality
			{
				get
				{
					return compositingQuality;
				}
				set
				{
					if(compositingQuality != value)
					{
						compositingQuality = value;
						dirtyFlags |= DirtyFlags.CompositingQuality;
					}
				}
			}
	public virtual float DpiX
			{
				get
				{
					return Graphics.DefaultScreenDpi;
				}
			}
	public virtual float DpiY
			{
				get
				{
					return Graphics.DefaultScreenDpi;
				}
			}
	public virtual InterpolationMode InterpolationMode
			{
				get
				{
					return interpolationMode;
				}
				set
				{
					if(interpolationMode != value)
					{
						interpolationMode = value;
						dirtyFlags |= DirtyFlags.InterpolationMode;
					}
				}
			}
	public virtual PixelOffsetMode PixelOffsetMode
			{
				get
				{
					return pixelOffsetMode;
				}
				set
				{
					if(pixelOffsetMode != value)
					{
						pixelOffsetMode = value;
						dirtyFlags |= DirtyFlags.PixelOffsetMode;
					}
				}
			}
	public virtual Point RenderingOrigin
			{
				get
				{
					return renderingOrigin;
				}
				set
				{
					if(renderingOrigin != value)
					{
						renderingOrigin = value;
						dirtyFlags |= DirtyFlags.RenderingOrigin;
					}
				}
			}
	public virtual SmoothingMode SmoothingMode
			{
				get
				{
					return smoothingMode;
				}
				set
				{
					if(smoothingMode != value)
					{
						smoothingMode = value;
						dirtyFlags |= DirtyFlags.SmoothingMode;
					}
				}
			}
	public virtual int TextContrast
			{
				get
				{
					return textContrast;
				}
				set
				{
					if(textContrast != value)
					{
						textContrast = value;
						dirtyFlags |= DirtyFlags.TextContrast;
					}
				}
			}
	public virtual TextRenderingHint TextRenderingHint
			{
				get
				{
					return textRenderingHint;
				}
				set
				{
					if(textRenderingHint != value)
					{
						textRenderingHint = value;
						dirtyFlags |= DirtyFlags.TextRenderingHint;
					}
				}
			}

	// Determine if a particular section of the dirty flags are set.
	protected bool IsDirty(DirtyFlags flags)
			{
				return ((dirtyFlags & flags) != 0);
			}

	// Check if dirty flags are set and also clear them.
	protected bool CheckDirty(DirtyFlags flags)
			{
				bool result = ((dirtyFlags & flags) != 0);
				dirtyFlags &= ~flags;
				return result;
			}

	// Clear specific dirty flags.
	protected void ClearDirty(DirtyFlags flags)
			{
				dirtyFlags &= ~flags;
			}

	// Dispose of this object.
	public virtual void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	protected abstract void Dispose(bool disposing);

	~ToolkitGraphicsBase()
			{
				Dispose(false);
			}

	// Clear the entire drawing surface.
	public abstract void Clear(Color color);

	// Draw a line between two points using the current pen.
	public abstract void DrawLine(int x1, int y1, int x2, int y2);

	// Draw a polygon using the current pen.
	public abstract void DrawPolygon(Point[] points);

	// Fill a polygon using the current brush.
	public abstract void FillPolygon(Point[] points, FillMode fillMode);

	// Draw a set of connected line segments using the current pen.
	public virtual void DrawLines(Point[] points)
			{
				for ( int i = 1;i < points.Length; i++ )
				{
					if (points[i-1] != points[i])
						DrawLine( points[i-1].X,points[i-1].Y,points[i].X,points[i].Y );
				}
			}

	// Select a font
	public IToolkitFont Font
			{
				set
				{
					font = value;
				}
				get
				{
					return font;
				}
			}

	// Select a pen
	public IToolkitPen Pen
			{
				set
				{
					pen = value;
				}
				get
				{
					return pen;
				}
			}

	// Select a brush
	public IToolkitBrush Brush
			{
				set
				{
					brush = value;
				}
				get
				{
					return brush;
				}
			}

	// Draw the source parallelogram of an image into the parallelogram
	// defined by dest. Point[] has 3 Points, Top-Left, Top-Right and Bottom-Left.
	// The remaining point is inferred.
	public virtual void DrawImage(IToolkitImage image, Point[] src, Point[] dest)
			{
				int originX = dest[0].X;
				int originY = dest[0].Y;
				for (int i = 1; i < 3; i++)
				{
					if (originX > dest[i].X)
						originX = dest[i].X;
					if (originY > dest[i].Y)
						originY = dest[i].Y;
				}

				DotGNU.Images.Image gnuImage = (image as ToolkitImageBase).image;
				// Currently we only draw the first frame.
				Frame frame = gnuImage.GetFrame(0);
				frame = frame.AdjustImage(src[0].X, src[0].Y, src[1].X, src[1].Y,
					src[2].X, src[2].Y, dest[0].X - originX, dest[0].Y - originY,
					dest[1].X - originX, dest[1].Y - originY, dest[2].X - originX,
					dest[2].Y - originY);
				// Setup the new image and draw it.
				using (DotGNU.Images.Image newGnuImage = new DotGNU.Images.Image(gnuImage.Width,
					gnuImage.Height, gnuImage.PixelFormat))
				{
					newGnuImage.AddFrame(frame);
					IToolkitImage newImage = Toolkit.CreateImage(newGnuImage, 0);
					DrawImage( newImage, originX, originY);
				}
			}

#if CONFIG_EXTENDED_NUMERICS

	private static Point ComputePoint(float u,
										int x1, int y1,
										int x2, int y2,
										int x3, int y3,
										int x4, int y4)
			{
				float px=0;
				float py=0;
				float blend;
				float u2=(1.0f-u);

				// Point 0
				blend = u2*u2*u2;
				px += blend * x1;
				py += blend * y1;

				// Point 1
				blend = 3 * u * u2 * u2;
				px += blend * x2;
				py += blend * y2;

				// Point 2
				blend = 3 * u * u * u2;
				px += blend * x3;
				py += blend * y3;

				// Point 3
				blend = u * u * u;
				px += blend * x4;
				py += blend * y4;

				return new Point((int)Math.Round(px),(int)Math.Round(py));
			}

	private int ComputeSteps(int x1, int y1, int x2, int y2, int x3,
								int y3, int x4, int y4)
			{
				double length=0L;
				
				length+=Math.Sqrt((x1-x2)*(x1-x2) + (y1-y2) * (y1-y2));
				length+=Math.Sqrt((x2-x3)*(x2-x3) + (y2-y3) * (y2-y3));
				length+=Math.Sqrt((x3-x4)*(x3-x4) + (y3-y4) * (y3-y4));
				
        return (int)Math.Ceiling(length);	
			}

	private Point[] ComputeBezier(int x1, int y1, int x2, int y2,		
						   int x3, int y3, int x4, int y4)
			{
				int steps=ComputeSteps(x1,y1,x2,y2,x3,y3,x4,y4);
				Point [] points=new Point[steps];
				for(int i=0;i<steps;i++)
				{
					float coeff=((float)i+1)/steps;
					points[i]=ComputePoint(coeff ,x1,y1,x2,y2,x3,y3,x4,y4);
				}                	
				return points;
			}

	private Point[] ComputeSplineSegment(Point p0,Point p1,Point T1, Point T2 )
			{                           
				int steps=ComputeSteps(p0.X,p0.Y,T1.X,T1.Y,T2.X,T2.Y,p1.X,p1.Y);			
							Point[] points = new Point[steps+1];
				for(int i=0;i<=steps;i++)
				{
					float s=((float)i)/steps;
					float s2=s*s;
					float s3=s2*s;
	  				float h1 =  2*s3 - 3*s2 + 1;
  					float h2 = -2*s3 + 3*s2;
  					float h3 =    s3 - 2*s2 + s;
  					float h4 =    s3 -   s2;   
					points[i].X=(int)(h1*p0.X+h2*p1.X+h3*T1.X+h4*T2.X);
					points[i].Y=(int)(h1*p0.Y+h2*p1.Y+h3*T1.Y+h4*T2.Y);
				}                	
				return points;
			}
               
	private Point[] ComputeTangent(Point[] points, float tension, bool closed, int numberOfSegments)
			{
				Point p0,p1;
				if (numberOfSegments<3) return null;
				Point[] tan=new Point[numberOfSegments];	
				for(int i=0;i<numberOfSegments;i++)
				{                       
					if(i==0) 
					{
										if(closed)
							p0 = points[numberOfSegments-1];
						else
							p0 = points[0];
					} else p0 = points[i-1];
					if(i==numberOfSegments-1)
					{
						if(closed)
							p1 = points[0];
						else
							p1 = points[i];
					} else p1 = points[i+1];

					tan[i].X = (int)(tension*(p1.X - p0.X));
					tan[i].Y = (int)(tension*(p1.Y - p0.Y));			
				}			
				return tan;
			}

	// Draw a bezier curve using the current pen.
	public virtual void DrawBezier(int x1, int y1, int x2, int y2,
								   int x3, int y3, int x4, int y4)
			{
				// TODO: Optimize this to plot points without 
				// involving line-drawing operations
				Point [] points = ComputeBezier(x1,y1,x2,y2,x3,y3,x4,y4);
				if (points.Length > 2)
				{
					DrawLines(points);
				}
			}

// Fill a bezier curve using the current pen.
	public virtual void FillBezier(int x1, int y1, int x2, int y2,
																		 int x3, int y3, int x4, int y4, FillMode fillMode )
	{
		// TODO: Optimize this to plot points without 
		// involving line-drawing operations
		Point [] points = ComputeBezier(x1,y1,x2,y2,x3,y3,x4,y4);
		if (points.Length > 2)
		{
			FillPolygon(points,fillMode);
		}
	}
			
	// Draw a closed cardinal curve using the current pen.
	public virtual void DrawClosedCurve(Point[] points, float tension)
			{
				if(points.Length == 0)
					return;
			
				Point [] tangent=ComputeTangent(points,tension,true,points.Length);
				if (tangent == null)
				{
					DrawLines(points);
					return;
				}
				for(int i=0;i<points.Length-1;i++)
					DrawLines(ComputeSplineSegment(points[i],points[i+1],tangent[i],tangent[i+1]));
				DrawLines(ComputeSplineSegment(points[points.Length-1],points[0],tangent[points.Length-1],tangent[0]));
			}


	// Fill a closed cardinal curve using the current brush.
	public virtual void FillClosedCurve
				(Point[] points, float tension, FillMode fillMode)
			{
				if(points.Length == 0)
					return;
							
				Point [] tangent=ComputeTangent(points,tension,true,points.Length);
				if (tangent == null)
 				{
					DrawLines(points);
					return;
 				}
				Point[][] fpoints = new Point[points.Length][];
				int size=0;
				for(int i=0;i<points.Length-1;i++)
				{
					fpoints[i]=ComputeSplineSegment(points[i],points[i+1],tangent[i],tangent[i+1]);
					DrawLines(fpoints[i]);
					size+=fpoints[i].Length;				
				}

				fpoints[points.Length-1]= 
                        		ComputeSplineSegment(points[points.Length-1],points[0],tangent[points.Length-1],tangent[0]);
				size+=fpoints[points.Length-1].Length;

				Point[] poly= new Point[size];
				int z=0;
				for(int i=0;i<fpoints.Length;i++) 			
					for(int j=0;j<fpoints[i].Length;j++)
						poly[z++]=fpoints[i][j];
				FillPolygon(poly,fillMode);
			}


	// Draw a cardinal curve using the current pen.
	public virtual void DrawCurve(Point[] points, int offset,
				   				  int numberOfSegments, float tension)
			{
				Point [] tangent=ComputeTangent(points,tension,false,numberOfSegments+1);
				if (tangent == null)
				{
					DrawLines(points);
					return;
				}
				for(int i=0;i<numberOfSegments;i++)
					DrawLines(ComputeSplineSegment(points[i],points[i+1],tangent[i],tangent[i+1]));
			}

#else // !CONFIG_EXTENDED_NUMERICS

	// Stub out spline operations if we don't have floating-point.

	// Draw a bezier curve using the current pen.
	public virtual void DrawBezier(int x1, int y1, int x2, int y2,
								   int x3, int y3, int x4, int y4)
			{
				// Nothing to do here.
			}

	// Draw a closed cardinal curve using the current pen.
	public virtual void DrawClosedCurve(Point[] points, float tension)
			{			
				// Nothing to do here.
			}

	// Fill a closed cardinal curve using the current brush.
	public virtual void FillClosedCurve
				(Point[] points, float tension, FillMode fillMode)
			{
				// Nothing to do here.
			}

	// Draw a cardinal curve using the current pen.
	public virtual void DrawCurve(Point[] points, int offset,
				   				  int numberOfSegments, float tension)
			{
				// Nothing to do here.
			}

#endif // !CONFIG_EXTENDED_NUMERICS

	// Draw an arc within a rectangle defined by four points.
	public abstract void DrawArc
			(Point[] rect, float startAngle, float sweepAngle);

	// Draw a pie slice within a rectangle defined by four points.
	public abstract void DrawPie
			(Point[] rect, float startAngle, float sweepAngle);

	// Fill a pie slice within a rectangle defined by four points.
	public abstract void FillPie
			(Point[] rect, float startAngle, float sweepAngle);

	// Draw a string using the current font and brush.
	public abstract void DrawString
				(String s, int x, int y, StringFormat format);

	// Draw a string using the current font and brush within a
	// layout rectangle that is defined by four points.
	public virtual void DrawString
				(String s, Point[] layoutRectangle, StringFormat format)
			{
				// TODO: implement a default string draw, laying out
				// the text within the specified rectangle.
			}

	// Measure a string using the current font and a given layout rectangle.
	public abstract Size MeasureString
				(String s, Point[] layoutRectangle,
				 StringFormat format, out int charactersFitted,
				 out int linesFilled, bool ascentOnly);

	// Flush the graphics subsystem
	public virtual void Flush(FlushIntention intention)
			{
				// Nothing to do in the base class.
			}

	// Get the nearest color to a specified one.
	public virtual Color GetNearestColor(Color color)
			{
				// By default, we assume that the display is true color.
				return color;
			}

	// Add a metafile comment.
	public virtual void AddMetafileComment(byte[] data)
			{
				// Nothing to do in the base class.
			}

	// Get the HDC associated with this graphics object.
	public virtual IntPtr GetHdc()
			{
				// Nothing to do in the base class.
				return IntPtr.Zero;
			}

	// Release a HDC that was obtained using "GetHdc()".
	public virtual void ReleaseHdc(IntPtr hdc)
			{
				// Nothing to do in the base class.
			}

	// Set the clipping region to empty.
	public abstract void SetClipEmpty();

	// Set the clipping region to infinite (i.e. disable clipping).
	public abstract void SetClipInfinite();

	// Set the clipping region to a single rectangle.
	public abstract void SetClipRect(int x, int y, int width, int height);

	// Set the clipping region to a list of rectangles.
	public abstract void SetClipRects(Rectangle[] rects);

	// Set the clipping region to a complex mask.
	public abstract void SetClipMask(Object mask, int topx, int topy);

	// Get the line spacing for the font selected into this graphics object.
	public abstract int GetLineSpacing();

	// Draw an image at the coordinates
	public abstract void DrawImage(IToolkitImage image, int x, int y);

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	public abstract void DrawGlyph(int x, int y,
				   				   byte[] bits, int bitsWidth, int bitsHeight,
				   				   Color color);

}; // class ToolkitGraphicsBase

}; // namespace System.Drawing.Toolkit
