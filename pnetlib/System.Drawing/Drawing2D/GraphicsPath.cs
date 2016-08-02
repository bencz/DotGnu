/*
 * GraphicsPath.cs - Implementation of the
 *			"System.Drawing.Drawing2D.GraphicsPath" class.
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

using System.Collections;

namespace System.Drawing.Drawing2D
{

public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
{
	// Internal state.
	private ArrayList  pathFigures  = new ArrayList(50);
	private ArrayList  stringObjs   = new ArrayList(50);
	private PointF     []pathPoints = new PointF[0];
	private PathFigure actualFigure = null; 

	private bool needPenBrush;
	private FillMode fillMode;

	// Convert an integer point array into a float point array.
	private static PointF[] Convert(Point[] pts)
			{
				if(pts == null)
				{
					throw new ArgumentNullException("pts");
				}
				PointF[] fpts = new PointF [pts.Length];
				int posn;
				for(posn = 0; posn < pts.Length; ++posn)
				{
					fpts[posn] = pts[posn];
				}
				return fpts;
			}

	// Convert an integer rectangle array into a float rectangle array.
	private static RectangleF[] Convert(Rectangle[] rects)
			{
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				RectangleF[] frects = new RectangleF [rects.Length];
				int posn;
				for(posn = 0; posn < rects.Length; ++posn)
				{
					frects[posn] = rects[posn];
				}
				return frects;
			}

	// Constructors.
	public GraphicsPath()
			{
				this.fillMode = FillMode.Alternate;
			}
	public GraphicsPath(FillMode fillMode)
			{
				this.fillMode = fillMode;
			}
	public GraphicsPath(Point[] pts, byte[] types)
			: this(pts, types, FillMode.Alternate) {}
	public GraphicsPath(PointF[] pts, byte[] types)
			: this(pts, types, FillMode.Alternate) {}
	[TODO]
	public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
			{
				if(pts == null)
				{
					throw new ArgumentNullException("pts");
				}
				if(types == null)
				{
					throw new ArgumentNullException("types");
				}
				this.fillMode = fillMode;
				// TODO: convert the pts and types arrays
			}
	[TODO]
	public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
			{
				if(pts == null)
				{
					throw new ArgumentNullException("pts");
				}
				if(types == null)
				{
					throw new ArgumentNullException("types");
				}
				this.fillMode = fillMode;
				// TODO: convert the pts and types arrays
			}

	// Destructor.
	~GraphicsPath()
			{
				Dispose(false);
			}

	// Get or set this object's properties.
	public FillMode FillMode
			{
				get
				{
					return fillMode;
				}
				set
				{
					fillMode = value;
				}
			}
	[TODO]
	public PathData PathData
			{
				get
				{
					PathData data = new PathData();
					// TODO
					data.Points = this.pathPoints;
					data.Types = this.PathTypes;
					return data;
				}
			}
	[TODO]
	public PointF[] PathPoints
			{
				get
				{
					return this.pathPoints;
				}
			}
	[TODO]
	public byte[] PathTypes
			{
				get
				{
					// TODO
					//return types;
					return new byte [this.pathPoints.Length];
				}
			}

	// get the number of elements in PathPoints or the PathTypes Array
	[TODO]
	public int PointCount
			{
				get
				{
					return this.pathPoints.Length;
				}
			}
			
	private void AddPathPoints( PointF [] pts ) 
		{
			if( null == pts || pts.Length == 0 ) return;
			PointF [] pNew = new PointF[this.pathPoints.Length+pts.Length];
			Array.Copy( this.pathPoints, 0, pNew, 0, this.pathPoints.Length );
			Array.Copy( pts, 0, pNew, this.pathPoints.Length, pts.Length );
			this.pathPoints = pNew;
		}

	// Add an object to this path.
	private void AddPathObject(PathObject obj)
			{
				if( null == actualFigure ) {
					actualFigure = new PathFigure();
					pathFigures.Add( actualFigure );
				}
				actualFigure.AddPathObject( obj );
				this.AddPathPoints( obj.GetPathPoints() );
			}

	// Add an arc to the current figure.
	public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
			{
				AddArc((float)(rect.X), (float)(rect.Y),
					   (float)(rect.Width), (float)(rect.Height),
					   startAngle, sweepAngle);
			}
	public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
			{
				AddArc(rect.X, rect.Y, rect.Width, rect.Height,
					   startAngle, sweepAngle);
			}
	public void AddArc(int x, int y, int width, int height,
					   float startAngle, float sweepAngle)
			{
				AddArc((float)x, (float)y, (float)width, (float)height,
					   startAngle, sweepAngle);
			}
	public void AddArc(float x, float y, float width, float height,
					   float startAngle, float sweepAngle)
			{
				AddPathObject(new ArcPathObject(x, y, width, height,
												startAngle, sweepAngle));
				needPenBrush = true;
			}

	// Add a bezier curve to the current path.
	public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
			{
				AddBezier((float)(pt1.X), (float)(pt1.Y),
						  (float)(pt2.X), (float)(pt2.Y),
						  (float)(pt3.X), (float)(pt3.Y),
						  (float)(pt4.X), (float)(pt4.Y));
			}
	public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
			{
				AddBezier(pt1.X, pt1.Y, pt2.X, pt2.Y,
						  pt3.X, pt3.Y, pt4.X, pt4.Y);
			}
	public void AddBezier(int x1, int y1, int x2, int y2,
						  int x3, int y3, int x4, int y4)
			{
				AddBezier((float)x1, (float)y1, (float)x2, (float)y2,
						  (float)x3, (float)y3, (float)x4, (float)y4);
			}
	public void AddBezier(float x1, float y1, float x2, float y2,
						  float x3, float y3, float x4, float y4)
			{
				AddPathObject(new BezierPathObject
						(x1, y1, x2, y2, x3, y3, x4, y4));
				needPenBrush = true;
			}

	// Add a set of beziers to the current path.
	public void AddBeziers(Point[] pts)
			{
				AddBeziers(Convert(pts));
			}
	public void AddBeziers(PointF[] pts)
			{
				if(pts == null)
				{
					throw new ArgumentNullException("pts");
				}
				if(pts.Length < 4)
				{
					throw new ArgumentException
						(String.Format
							(S._("Arg_NeedsAtLeastNPoints"), 4));
				}
				int posn = 0;
				while((posn + 4) <= pts.Length)
				{
					AddBezier(pts[posn], pts[posn + 1],
							  pts[posn + 2], pts[posn + 3]);
					posn += 3;
				}
			}

	// Add a closed curve to the current path.
	public void AddClosedCurve(Point[] points)
			{
				AddClosedCurve(Convert(points), 0.5f);
			}
	public void AddClosedCurve(PointF[] points)
			{
				AddClosedCurve(points, 0.5f);
			}
	public void AddClosedCurve(Point[] points, float tension)
			{
				AddClosedCurve(Convert(points), tension);
			}
	public void AddClosedCurve(PointF[] points, float tension)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				if(points.Length < 4)
				{
					throw new ArgumentException
						(String.Format
							(S._("Arg_NeedsAtLeastNPoints"), 4));
				}
				this.actualFigure = null; // Close Figure before adding a closed curve
				AddPathObject(new ClosedCurvePathObject(points, tension));
				this.actualFigure = null; // Close Figure after adding a closed curve
			}

	// Add a curve to the current path.
	public void AddCurve(Point[] points)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				AddCurve(Convert(points), 0, points.Length - 1, 0.5f);
			}
	public void AddCurve(PointF[] points)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				AddCurve(points, 0, points.Length - 1, 0.5f);
			}
	public void AddCurve(Point[] points, float tension)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				AddCurve(Convert(points), 0, points.Length - 1, tension);
			}
	public void AddCurve(PointF[] points, float tension)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				AddCurve(points, 0, points.Length - 1, tension);
			}
	public void AddCurve(Point[] points, int offset,
						 int numberOfSegments, float tension)
			{
				AddCurve(Convert(points), offset, numberOfSegments, tension);
			}
	public void AddCurve(PointF[] points, int offset,
						 int numberOfSegments, float tension)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
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
				AddPathObject(new CurvePathObject
					(points, offset, numberOfSegments, tension));
				needPenBrush = true;
			}

	// Add an ellipse to the current figure.
	public void AddEllipse(Rectangle rect)
			{
				AddEllipse((float)(rect.X), (float)(rect.Y),
					       (float)(rect.Width), (float)(rect.Height));
			}
	public void AddEllipse(RectangleF rect)
			{
				AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);
			}
	public void AddEllipse(int x, int y, int width, int height)
			{
				AddEllipse((float)x, (float)y, (float)width, (float)height);
			}
	public void AddEllipse(float x, float y, float width, float height)
			{
				this.actualFigure = null;	// Ellipse closes figure
				AddPathObject(new ArcPathObject
					(x, y, width, height, -5f, 365f));
				this.actualFigure = null;	// Ellipse closes figure
			}

	// Add a line to the current figure.
	public void AddLine(Point pt1, Point pt2)
			{
				AddLine((float)(pt1.X), (float)(pt1.Y),
					    (float)(pt2.X), (float)(pt2.Y));
			}
	public void AddLine(PointF pt1, PointF pt2)
			{
				AddLine(pt1.X, pt1.Y, pt2.X, pt2.Y);
			}
	public void AddLine(int x1, int y1, int x2, int y2)
			{
				AddLine((float)x1, (float)y1, (float)x2, (float)y2);
			}
	public void AddLine(float x1, float y1, float x2, float y2)
			{
				AddPathObject(new LinePathObject(x1, y1, x2, y2));
				needPenBrush = true;
			}

	// Add a list of lines to the current figure.
	public void AddLines(Point[] points)
			{
				AddLines(Convert(points));
			}
	public void AddLines(PointF[] points)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				if(points.Length < 2)
				{
					throw new ArgumentException
						(String.Format
							(S._("Arg_NeedsAtLeastNPoints"), 2));
				}
				AddPathObject(new LinesPathObject(points));
				needPenBrush = true;
			}

	// Append another path to this one.  "connect" is intended for figures,
	public void AddPath(GraphicsPath addingPath, bool connect)
			{
				if(addingPath == null)
				{
					throw new ArgumentNullException("addingPath");
				}
				
				if( connect ) {
					foreach( PathFigure fig in addingPath.pathFigures ) {
						foreach( PathObject obj in fig.pathObjs ) {
							this.AddPathObject( obj.Clone() );
						}
					}
				}
				else {
					foreach( PathFigure fig in addingPath.pathFigures ) {
						this.pathFigures.Add( fig.Clone() );
					}
					actualFigure = null;
				}
				
				if(addingPath.needPenBrush)
				{
					needPenBrush = true;
				}
			}

	// Add a pie section to the current figure.
	public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
			{
				AddPie((float)(rect.X), (float)(rect.Y),
					   (float)(rect.Width), (float)(rect.Height),
					   startAngle, sweepAngle);
			}
	public void AddPie(int x, int y, int width, int height,
					   float startAngle, float sweepAngle)
			{
				AddPie((float)x, (float)y, (float)width, (float)height,
					   startAngle, sweepAngle);
			}
	public void AddPie(float x, float y, float width, float height,
					   float startAngle, float sweepAngle)
			{
				this.actualFigure = null; // Adding Pie closes a figure
				AddPathObject(new PiePathObject(x, y, width, height,
												startAngle, sweepAngle));
				this.actualFigure = null;// Adding Pie closes a figure
			}

	// Add a polygon to this path.
	public void AddPolygon(Point[] points)
			{
				AddPolygon(Convert(points));
			}
	public void AddPolygon(PointF[] points)
			{
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				if(points.Length < 2)
				{
					throw new ArgumentException
						(String.Format
							(S._("Arg_NeedsAtLeastNPoints"), 2));
				}
				this.actualFigure = null; // Adding Plygon closes a figure
				AddPathObject(new PolygonPathObject(points));
				this.actualFigure = null; // Adding Plygon closes a figure
			}

	// Add a rectangle to this path.
	public void AddRectangle(Rectangle rect)
			{
				AddRectangle((RectangleF)rect);
			}
	public void AddRectangle(RectangleF rect)
			{
				this.actualFigure = null; // Adding Rectangle closes a figure
				AddPathObject(new RectanglePathObject
					(rect.X, rect.Y, rect.Width, rect.Height));
				this.actualFigure = null; // Adding Rectangle closes a figure
			}

	// Add a list of rectangles to this path.
	public void AddRectangles(Rectangle[] rects)
			{
				AddRectangles(Convert(rects));
			}
	public void AddRectangles(RectangleF[] rects)
			{
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				int posn;
				for(posn = 0; posn < rects.Length; ++posn)
				{
					AddRectangle(rects[posn]);
				}
			}

	// Add a string to this path.
	public void AddString(String s, FontFamily family,
						  int style, float emSize,
						  Point origin, StringFormat format)
			{
				AddString(s, family, style, emSize, (PointF)origin, format);
			}
	[TODO]
	public void AddString(String s, FontFamily family,
						  int style, float emSize,
						  PointF origin, StringFormat format)
			{
				// String Paths are added drawn
				this.stringObjs.Add( new StringPathObject( s, family, style, emSize, origin, format ) );
			}
	public void AddString(String s, FontFamily family,
						  int style, float emSize,
						  Rectangle layoutRect, StringFormat format)
			{
				AddString(s, family, style, emSize,
						  (RectangleF)layoutRect, format);
			}
	[TODO]
	public void AddString(String s, FontFamily family,
						  int style, float emSize,
						  RectangleF layoutRect, StringFormat format)
			{
				this.actualFigure = null; // Close Figure before adding a Text
				AddPathObject( new StringPathObject( s, family, style, emSize, layoutRect, format ) );
				this.actualFigure = null; // Close Figure after adding a Text
			}

	// Clean all markers from this path.
	public void ClearMarkers()
			{
				// We don't do anything special with markers here.
			}

	// Clone this object.
	public Object Clone()
			{
				GraphicsPath path = new GraphicsPath(fillMode);
				path.needPenBrush = needPenBrush;
				
				foreach( PathFigure figure in pathFigures ) {
					path.pathFigures.Add( figure.Clone() );
				}
				return path;
			}

	// Close all figures within the path.
	public void CloseAllFigures()
			{
				actualFigure = null;
			}

	// Close the current figure and start a new one.
	public void CloseFigure()
			{
				if( null != actualFigure ) actualFigure.CloseFigure();
				actualFigure = null;
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	private void Dispose(bool disposing)
			{
				// Nothing to do here, because there is no unmanaged state.
			}

	// Flatten this curve into a set of line segments.
	public void Flatten()
			{
				Flatten(null, 0.25f);
			}
	public void Flatten(Matrix matrix)
			{
				Flatten(matrix, 0.25f);
			}
	[TODO]
	public void Flatten(Matrix matrix, float flatness)
			{
				// TODO
			}

	// Get a rectangle that bounds this path.
	public RectangleF GetBounds()
			{
				return GetBounds(null, null);
			}
	public RectangleF GetBounds(Matrix matrix)
			{
				return GetBounds(matrix, null);
			}
	[TODO]
	public RectangleF GetBounds(Matrix matrix, Pen pen)
			{
				// TODO
				return RectangleF.Empty;
			}

	// Get the last point in this path.
	public PointF GetLastPoint()
			{
				PointF[] points = PathPoints;
				if( null == PathPoints ) return new PointF(0,0);
				return points[points.Length - 1];
			}

	// Determine if a point is visible when drawing an outline with a pen.
	public bool IsOutlineVisible(Point point, Pen pen)
			{
				return IsOutlineVisible((float)(point.X), (float)(point.Y),
										pen, null);
			}
	public bool IsOutlineVisible(PointF point, Pen pen)
			{
				return IsOutlineVisible(point.X, point.Y, pen, null);
			}
	public bool IsOutlineVisible(int x, int y, Pen pen)
			{
				return IsOutlineVisible((float)x, (float)y, pen, null);
			}
	public bool IsOutlineVisible(float x, float y, Pen pen)
			{
				return IsOutlineVisible(x, y, pen, null);
			}
	public bool IsOutlineVisible(Point point, Pen pen, Graphics graphics)
			{
				return IsOutlineVisible((float)(point.X), (float)(point.Y),
										pen, graphics);
			}
	public bool IsOutlineVisible(PointF point, Pen pen, Graphics graphics)
			{
				return IsOutlineVisible(point.X, point.Y, pen, graphics);
			}
	public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
			{
				return IsOutlineVisible((float)x, (float)y, pen, graphics);
			}
	[TODO]
	public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
			{
				// TODO
				return false;
			}

	// Determine if a point is visible.
	public bool IsVisible(Point point)
			{
				return IsVisible((float)(point.X), (float)(point.Y), null);
			}
	public bool IsVisible(PointF point)
			{
				return IsVisible(point.X, point.Y, null);
			}
	public bool IsVisible(int x, int y)
			{
				return IsVisible((float)x, (float)y, null);
			}
	public bool IsVisible(float x, float y)
			{
				return IsVisible(x, y, null);
			}
	public bool IsVisible(Point point, Graphics graphics)
			{
				return IsVisible((float)(point.X), (float)(point.Y), graphics);
			}
	public bool IsVisible(PointF point, Graphics graphics)
			{
				return IsVisible(point.X, point.Y, graphics);
			}
	public bool IsVisible(int x, int y, Graphics graphics)
			{
				return IsVisible((float)x, (float)y, graphics);
			}
	[TODO]
	public bool IsVisible(float x, float y, Graphics graphics)
			{
				// TODO
				return false;
			}

	// Reset this path.
	public void Reset()
			{
				pathPoints = new PointF[0]; // reset path points
				actualFigure = null;
				pathFigures.Clear();
				needPenBrush = false;
				fillMode = FillMode.Alternate;
			}

	// Reverse the order of the path.
	public void Reverse()
			{
				foreach( PathFigure fig in pathFigures ) {
					fig.Reverse();
				}
				pathFigures.Reverse();
			}

	// Set a marker.
	public void SetMarkers()
			{
				// We don't do anything special with markers here.
			}

	// Start a new figure without closing the old one.
	public void StartFigure()
			{
				// We don't do anything special with figures here.
				actualFigure = null;
			}

	// Apply a transformation to this path.
	[TODO]
	public void Transform(Matrix matrix)
			{
				foreach( PathFigure fig in pathFigures ) {
					fig.Transform(matrix);
				}
				foreach( StringPathObject obj in stringObjs ) {
					obj.Transform(matrix);
				}
			}

	// Apply a warp transformation to this path.
	public void Warp(PointF[] destPoints, RectangleF srcRect)
			{
				Warp(destPoints, srcRect, null, WarpMode.Perspective, 0.25f);
			}
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
			{
				Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0.25f);
			}
	public void Warp(PointF[] destPoints, RectangleF srcRect,
					 Matrix matrix, WarpMode warpMode)
			{
				Warp(destPoints, srcRect, matrix, warpMode, 0.25f);
			}
	[TODO]
	public void Warp(PointF[] destPoints, RectangleF srcRect,
					 Matrix matrix, WarpMode warpMode, float flatness)
			{
				// TODO
			}

	// Widen the path outline.
	public void Widen(Pen pen)
			{
				Widen(pen, null, 0.25f);
			}
	public void Widen(Pen pen, Matrix matrix)
			{
				Widen(pen, matrix, 0.25f);
			}
	[TODO]
	public void Widen(Pen pen, Matrix matrix, float flatness)
			{
				// TODO
			}

	// Draw this graphics path.
	internal void Draw(Graphics graphics, Pen pen)
			{	
				foreach( PathFigure fig in pathFigures ) {
					fig.Draw( graphics, pen );
				}
				foreach( StringPathObject obj in stringObjs ) {
					obj.Draw( graphics, pen );
				}
			}

	// Fill this graphics path.
	internal void Fill(Graphics graphics, Brush brush)
			{
				Pen pen;
				if(needPenBrush)
				{
					pen = new Pen(brush);
				}
				else
				{
					pen = null;
				}
				
				// XorBrush xorbrush = new XorBrush( brush );
				
				/* TODO:
					Fill graphics Path xor, does not do yet :(
				*/
				
				foreach( PathFigure fig in pathFigures ) {
					fig.Fill( graphics, brush, pen, fillMode );
				}
				foreach( StringPathObject obj in stringObjs ) {
					obj.Fill( graphics, brush, pen, fillMode );
				}
				
				/*
				XorBrush xorbrush = new XorBrush( brush );
				foreach( PathFigure fig in pathFigures ) {
					fig.Fill( graphics, xorbrush, pen, fillMode );
				}
				foreach( StringPathObject obj in stringObjs ) {
					obj.Fill( graphics, xorbrush, pen, fillMode );
				}
				*/
				
				if(needPenBrush)
				{
					pen.Dispose();
				}
			}
			
	private class PathFigure : ICloneable
	{
		bool closedFigure = false;
		public ArrayList pathObjs;
		
		public PathFigure() 
		{
			pathObjs = new ArrayList();
		}
		
		public object Clone() {
			PathFigure cpy = new PathFigure();
			foreach( PathObject o in pathObjs ) {
				cpy.AddPathObject( o.Clone() );
			}
			return cpy;
		}
		
		public void CloseFigure() {
			this.closedFigure = true;
		}
		
		public PointF[] GetPathPoints() {
			PointF[][] fpoints = new PointF[pathObjs.Count][];
			int size=0;
			int i= 0;
			foreach( PathObject obj in pathObjs ) 
			{
				fpoints[i] = obj.GetPathPoints();
				if( null != fpoints[i] ) size += fpoints[i].Length;
				i++;
			}
			
			if( this.closedFigure ) size += 1; 
				
			PointF[] poly= new PointF[size];
			int z=0;
			for(i=0;i<fpoints.Length;i++) {
				if( null != fpoints[i] ) {
					for(int j=0;j<fpoints[i].Length;j++) {
						poly[z++]=fpoints[i][j];
					}
				}
			}
			if( this.closedFigure ) poly[z++] = poly[0];
				
			return poly;
		}
		
		public void AddPathObject(PathObject obj)
		{
			pathObjs.Add( obj );
		}
		
		public void Reverse() {
			pathObjs.Reverse();
		} 
		
		public void Transform(Matrix matrix)
		{
			foreach( PathObject obj in pathObjs ) {
				obj.Transform( matrix );
			}
		}
		
		// Draw the graphics path
		public void Draw(Graphics graphics, Pen pen)
		{
			/*
			foreach( PathObject o in pathObjs ) {
				o.Draw( graphics, pen );
			}
			*/
			
			PointF [] poly = GetPathPoints();
				
			if( poly.Length > 1 ) {
				
				graphics.DrawLines(pen, poly);
				
			}
			foreach( PathObject o in pathObjs ) {
				if( !o.HasPathPoints ) o.Draw( graphics, pen );
			}
		}

		// Fill this graphics path.
		public void Fill(Graphics graphics, Brush brush, Pen pen, FillMode fillMode)
		{
			/*
			foreach( PathObject o in pathObjs ) {
				o.Fill( graphics, brush, pen, fillMode );
			}
			*/
			PointF [] poly = GetPathPoints();
				
			if( poly.Length > 2 ) {
				
				graphics.FillPolygon(brush, poly, fillMode );
				
			}
			foreach( PathObject o in pathObjs ) {
				if( !o.HasPathPoints ) o.Fill(graphics, brush, pen, fillMode);
			}
		}
	}

	// Base class for path objects.
	private abstract class PathObject
	{
		protected Matrix mMatrix;
		// Constructor.
		protected PathObject() {}

		// Draw this path object.
		public abstract void Draw(Graphics graphics, Pen pen);

		// Fill this path object.
		public abstract void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode);

		// Clone this path object.
		public abstract PathObject Clone();
		
		public void Transform(Matrix matrix)
		{
			if( null == mMatrix ) {
				mMatrix = new Matrix();
			}
			mMatrix.Multiply( matrix );
		}
		
		public PointF[] GetPathPoints() {
			PointF[] data = DoGetPathPoints();
			if( null == data ) return null;
			data = (PointF[])data.Clone();
			if( null != mMatrix ) {
				mMatrix.TransformPoints( data );
			}
			return data;
		}
		protected abstract PointF [] DoGetPathPoints();
		public abstract bool HasPathPoints { get; }

		protected static PointF ComputePoint(double u,
														 double x1, double y1,
														 double x2, double y2,
														 double x3, double y3,
														 double x4, double y4)
		{
			double px=0;
			double py=0;
			double blend;
			double u2=(1.0-u);

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

			return new PointF((float)Math.Round(px),(float)Math.Round(py));
		}

		protected int ComputeSteps(double x1, double y1, double x2, double y2, double x3,
											double y3, double x4, double y4)
		{
			double length=0L;
				
			length+=Math.Sqrt((x1-x2)*(x1-x2) + (y1-y2) * (y1-y2));
			length+=Math.Sqrt((x2-x3)*(x2-x3) + (y2-y3) * (y2-y3));
			length+=Math.Sqrt((x3-x4)*(x3-x4) + (y3-y4) * (y3-y4));
				
			return (int)Math.Ceiling(length);	
		}

		protected PointF[] ComputeBezier(double x1, double y1, double x2, double y2,		
													double x3, double y3, double x4, double y4)
		{
			int steps=ComputeSteps(x1,y1,x2,y2,x3,y3,x4,y4);
			PointF [] points=new PointF[steps];
			for(int i=0;i<steps;i++)
			{
				double coeff=((double)i+1)/steps;
				points[i]=ComputePoint(coeff ,x1,y1,x2,y2,x3,y3,x4,y4);
			}                	
			return points;
		}

		protected PointF[] ComputeSplineSegment(PointF p0,PointF p1,PointF T1, PointF T2 )
		{                           
			int steps=ComputeSteps(p0.X,p0.Y,T1.X,T1.Y,T2.X,T2.Y,p1.X,p1.Y);			
			PointF[] points = new PointF[steps+1];
			for(int i=0;i<=steps;i++)
			{
				double s=((double)i)/steps;
				double s2=s*s;
				double s3=s2*s;
				double h1 =  2*s3 - 3*s2 + 1;
				double h2 = -2*s3 + 3*s2;
				double h3 =    s3 - 2*s2 + s;
				double h4 =    s3 -   s2;   
				points[i].X=(float)(h1*p0.X+h2*p1.X+h3*T1.X+h4*T2.X);
				points[i].Y=(float)(h1*p0.Y+h2*p1.Y+h3*T1.Y+h4*T2.Y);
			}                	
			return points;
		}
               
		protected PointF[] ComputeTangent(PointF[] points, float tension, bool closed, int numberOfSegments)
		{
			PointF p0,p1;
			if (numberOfSegments<3) return null;
			PointF[] tan=new PointF[numberOfSegments];	
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
		
		protected double DegToRad( double val ) {
			return Math.PI*val/180.0;
		}
		protected double DegCosinus( double val ) {
			return Math.Cos( DegToRad( val ) );
		}
		protected double DegSinus( double val ) {
			return Math.Sin( DegToRad( val ) );
		}
	
	}; // class PathObject

	
	// Arc path objects.
	private sealed class ArcPathObject : PathObject
	{
		// Internal state.
		private float x, y, width, height;
		private float startAngle, sweepAngle;
		PointF [] pathPoints;
		PointF startPoint, endPoint; 

		// Constructor.
		public ArcPathObject(float x, float y, float width, float height,
					   		 float startAngle, float sweepAngle)
				{
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					this.startAngle = startAngle;
					this.sweepAngle = sweepAngle;
					this.pathPoints = CalculatePathPoints();
				}
				
		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}
		
		PointF [] CalculatePathPoints() {
			int    iCount = 360;
			PointF [] points = new PointF[iCount];
			
			double dDelta = this.sweepAngle;
			dDelta /= iCount;
			double dStart = this.startAngle;
			double dEnd   = dStart +  this.sweepAngle;
			double px,py;
			double rx = this.width/2;
			double ry = this.height/2;
			for( int i = 0; i < iCount-1; i++ ) {
				px = rx*( 1 + DegCosinus( dStart ) ) + this.x;
				py = ry*( 1 + DegSinus  ( dStart ) ) + this.y;
				points[i] = new PointF( (float)px,(float)py );
				if( i == 0 ) startPoint = points[i];
				dStart += dDelta;
			}
			px = rx*( 1 + DegCosinus( dEnd ) ) + this.x;
			py = ry*( 1 + DegSinus  ( dEnd ) ) + this.y;
			points[iCount-1] = new PointF( (float)px,(float)py );
			endPoint = points[iCount-1];
			return points;
		}

		protected override PointF [] DoGetPathPoints() {
			return this.pathPoints;
		}
				
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
					//graphics.DrawLine(pen, this.startPoint, this.endPoint);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.DrawArc(penBrush, x, y, width, height, startAngle, sweepAngle);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new ArcPathObject(x, y, width, height,
											 startAngle, sweepAngle);
				}

	}; // class ArcPathObject

	// Bezier path objects.
	private sealed class BezierPathObject : PathObject
	{
		// Internal state.
		private float x1, y1, x2, y2, x3, y3, x4, y4;
		PointF [] points;

		// Constructor.
		public BezierPathObject(float x1, float y1, float x2, float y2,
								float x3, float y3, float x4, float y4)
				{
					this.x1 = x1;
					this.y1 = y1;
					this.x2 = x2;
					this.y2 = y2;
					this.x3 = x3;
					this.y3 = y3;
					this.x4 = x4;
					this.y4 = y4;
					points = ComputeBezier( x1, y1, x2, y2, x3, y3, x4, y4 );
				}

		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}
		
		protected override PointF [] DoGetPathPoints() {
			 return points;
		}
		
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawBezier(pen, x1, y1, x2, y2, x3, y3, x4, y4);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.FillBezier(brush, x1, y1, x2, y2, x3, y3, x4, y4, fillMode );
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new BezierPathObject
						(x1, y1, x2, y2, x3, y3, x4, y4);
				}

	}; // class BezierPathObject

	// Closed curve path objects.
	private sealed class ClosedCurvePathObject : PathObject
	{
		// Internal state.
		private PointF[] points;
		private float tension;

		// Constructor.
		public ClosedCurvePathObject(PointF[] points, float tension)
				{
					this.points = (PointF[])(points.Clone());
					this.tension = tension;
				}
				
				
		public override bool HasPathPoints { 
			get {
				return false;
			} 
		}
		
		protected override PointF [] DoGetPathPoints() {
			return null;
		}

		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					if( null != mMatrix ) {
						Matrix save = graphics.Transform;
						graphics.Transform = mMatrix;
						graphics.DrawClosedCurve(pen, points, tension, FillMode.Alternate);
						graphics.Transform = save;
					}
					else {
						graphics.DrawClosedCurve(pen, points, tension, FillMode.Alternate);
					}
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					if( null != mMatrix ) {
						Matrix save = graphics.Transform;
						graphics.Transform = mMatrix;
						graphics.FillClosedCurve(brush, points, fillMode, tension);
						graphics.Transform = save;
					}
					else {
						graphics.FillClosedCurve(brush, points, fillMode, tension);
					}
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new ClosedCurvePathObject(points, tension);
				}

	}; // class ClosedCurvePathObject

	// Curve path objects.
	private sealed class CurvePathObject : PathObject
	{
		// Internal state.
		private PointF[] points;
		private int offset, numberOfSegments;
		private float tension;
		private PointF [] pathPoints;

		// Constructor.
		public CurvePathObject(PointF[] points, int offset,
							   int numberOfSegments, float tension)
				{
					this.points = (PointF[])(points.Clone());
					this.offset = offset;
					this.numberOfSegments = numberOfSegments;
					this.tension = tension;
					this.pathPoints = ComputePathPoints();
				}
				
		private PointF [] ComputePathPoints() {
			PointF [] tangent=ComputeTangent(points,tension,false,numberOfSegments+1);
			if (tangent == null)
			{
				return points;
			}
			PointF[][] fpoints = new PointF[numberOfSegments][];
			int size=0;
			for(int i=0;i<numberOfSegments;i++)
			{
				fpoints[i]=ComputeSplineSegment(points[i],points[i+1],tangent[i],tangent[i+1]);
				size+=fpoints[i].Length;				
			}

			PointF[] poly= new PointF[size];
			int z=0;
			for(int i=0;i<numberOfSegments;i++) 			
				for(int j=0;j<fpoints[i].Length;j++)
					poly[z++]=fpoints[i][j];
			return poly;
		}
		
		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}

		protected override PointF [] DoGetPathPoints() {
			return this.pathPoints;
		}
		
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawCurve(pen, points, offset, numberOfSegments, tension);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.DrawCurve(penBrush, points, offset, numberOfSegments, tension);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new CurvePathObject
						(points, offset, numberOfSegments, tension);
				}

	}; // class CurvePathObject

	// Line path objects.
	private sealed class LinePathObject : PathObject
	{
		// Internal state.
		private float x1, y1, x2, y2;
		PointF [] pathPoints;

		// Constructor.
		public LinePathObject(float x1, float y1, float x2, float y2)
				{
					this.x1 = x1;
					this.y1 = y1;
					this.x2 = x2;
					this.y2 = y2;
					pathPoints = new PointF[2];
					pathPoints[0] = new PointF( x1,y1 );
					pathPoints[1] = new PointF( x2,y2 );
				}
				
		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}

		protected override PointF [] DoGetPathPoints() {
			return pathPoints;
		}

		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawLine(pen, x1, y1, x2, y2);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.DrawLine(penBrush, x1, y1, x2, y2);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new LinePathObject(x1, y1, x2, y2);
				}

	}; // class LinePathObject

	// Multiple lines path objects.
	private sealed class LinesPathObject : PathObject
	{
		// Internal state.
		private PointF[] points;

		// Constructor.
		public LinesPathObject(PointF[] points)
				{
					this.points = (PointF[])(points.Clone());
				}

		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}

		protected override PointF [] DoGetPathPoints() {
			return points;
		}
		
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawLines(pen, points);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.DrawLines(penBrush, points);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new LinesPathObject(points);
				}

	}; // class LinesPathObject

	// Pie path objects.
	private sealed class PiePathObject : PathObject
	{
		// Internal state.
		private float x, y, width, height;
		private float startAngle, sweepAngle;
		PointF [] pathPoints;

		// Constructor.
		public PiePathObject(float x, float y, float width, float height,
					   		 float startAngle, float sweepAngle)
				{
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					this.startAngle = startAngle;
					this.sweepAngle = sweepAngle;
					this.pathPoints = CalculatePathPoints();
				}
				
		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}

		protected override PointF [] DoGetPathPoints() {
			return this.pathPoints;
		}

		PointF [] CalculatePathPoints() {
			int    iCount = 360;
			PointF [] points = new PointF[iCount+2];
			
			double dDelta = this.sweepAngle;
			dDelta /= iCount;
			double dStart = this.startAngle;
			double dEnd   = dStart+this.sweepAngle;
			double xP,yP;
			double rx = this.width/2;
			double ry = this.height/2;
			points[0] = new PointF( (float)(this.x+rx),(float)(this.y+ry) );
			for( int i = 1; i < iCount; i++ ) {
				xP = rx*( 1 + DegCosinus( dStart ) ) + this.x;
				yP = ry*( 1 + DegSinus  ( dStart ) ) + this.y;
				points[i] = new PointF( (float)xP,(float)yP );
				dStart += dDelta;
			}
			xP = rx*( 1 + DegCosinus( dEnd ) ) + this.x;
			yP = ry*( 1 + DegSinus  ( dEnd ) ) + this.y;
			points[iCount]   = new PointF( (float)xP,(float)yP );
			points[iCount+1] = points[0];
			return points;
		}

				
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					// TODO brubbel: calculate points and draw lines
					graphics.DrawPie(pen, x, y, width, height,
									 startAngle, sweepAngle);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					// TODO brubbel: calculate points and fill polygon
					graphics.FillPie(brush, x, y, width, height,
									 startAngle, sweepAngle);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new ArcPathObject(x, y, width, height,
											 startAngle, sweepAngle);
				}

	}; // class PiePathObject

	// Polygon path objects.
	private sealed class PolygonPathObject : PathObject
	{
		// Internal state.
		private PointF[] points;
		private PointF[] pathPoints;
		
		// Constructor.
		public PolygonPathObject(PointF[] points)
				{
					this.points = (PointF[])(points.Clone());
					int iCount = this.points.Length;
					this.pathPoints = new PointF[iCount+1];
					this.points.CopyTo( this.pathPoints, 0 );
					this.pathPoints[iCount] = this.pathPoints[0];
				}

		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}

		protected override PointF [] DoGetPathPoints() {
			return this.pathPoints;
		}
				
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawPolygon(pen, points);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.FillPolygon(brush, points);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new PolygonPathObject(points);
				}

	}; // class PolygonPathObject

	// Rectangle path objects.
	private sealed class RectanglePathObject : PathObject
	{
		// Internal state.
		private float x, y, width, height;
		private PointF [] pathPoints;

		// Constructor.
		public RectanglePathObject(float x, float y, float width, float height)
				{
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					pathPoints = new PointF[5];
					pathPoints[0] = new PointF( x, y );
					pathPoints[1] = new PointF( x+width, y );
					pathPoints[2] = new PointF( x+width, y+height );
					pathPoints[3] = new PointF( x      , y+height );
					pathPoints[4] = new PointF( x      , y );
					
				}
		
		public override bool HasPathPoints { 
			get {
				return true;
			} 
		}

		protected override PointF [] DoGetPathPoints() {
			return pathPoints;
		}

		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
				{
					graphics.DrawRectangle(pen, x, y, width, height);
				}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
								  Pen penBrush, FillMode fillMode)
				{
					graphics.FillRectangle(brush, x, y, width, height);
				}

		// Clone this path object.
		public override PathObject Clone()
				{
					return new RectanglePathObject(x, y, width, height);
				}

	}; // class RectanglePathObject

	// String path objects.
	private sealed class StringPathObject : PathObject
	{
		// Internal state.
		String 			mString;
		RectangleF		mRect;
		PointF			mPoint;
		StringFormat 	mFormat;
		Font				mFont;

		// Constructor.
		public StringPathObject( String s, FontFamily family, int style, float emSize, PointF origin, StringFormat format ) :
				this( s, family, style, emSize, origin, RectangleF.Empty, format )
		{
		}

		// Constructor.
		public StringPathObject( String s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format ) :
				this( s, family, style, emSize, PointF.Empty, layoutRect, format )
		{
		}
		
		private StringPathObject( String s, FontFamily family, int style, float emSize, PointF origin, RectangleF layoutRect, StringFormat format ) :
			this( s, new Font( family, emSize, (FontStyle)style ), origin, layoutRect, format )
		{
		}
		
		private StringPathObject( String s, Font font, PointF origin, RectangleF layoutRect, StringFormat format )
		{
			mString		= s;
			mFont			= font;
			mPoint		= origin;
			mRect			= layoutRect;
			mFormat		= format;
			mFormat.FormatFlags |= StringFormatFlags.NoClip;
		}
		
		public override bool HasPathPoints { 
			get {
				return false;
			} 
		}

						
		protected override PointF [] DoGetPathPoints() {
			// How should I ????
			return null;
		}

		
		RectangleF GetRect(Graphics graphics ) {
			RectangleF ret;
			if( RectangleF.Empty != mRect ) {
				ret = mRect;
			}
			else {
				SizeF sz = graphics.MeasureString( mString, mFont, mPoint, mFormat );
				ret = new RectangleF( mPoint, sz );
			}
			return ret;
		}
		
		// Draw this path object.
		public override void Draw(Graphics graphics, Pen pen)
		{
			using( SolidBrush b = new SolidBrush(pen.Color) ) {
				this.Fill( graphics, b, pen, FillMode.Alternate );
			}
		}

		// Fill this path object.
		public override void Fill(Graphics graphics, Brush brush,
										  Pen penBrush, FillMode fillMode)
		{
			RectangleF r = GetRect(graphics);
			if( null != mMatrix ) {
				Matrix oldMatrix = graphics.Transform;
				graphics.Transform = mMatrix;
				graphics.DrawString(mString, mFont, brush, r, mFormat);
				graphics.Transform = oldMatrix;
			}
			else {
				graphics.DrawString(mString, mFont, brush, r, mFormat);
			}
		}

		// Clone this path object.
		public override PathObject Clone()
		{
			return (PathObject) this.MemberwiseClone();
		}

	}; // class RectanglePathObject

}; // class GraphicsPath

}; // namespace System.Drawing.Drawing2D
