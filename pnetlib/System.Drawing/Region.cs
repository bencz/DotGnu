/*
 * Region.cs - Implementation of the "System.Drawing.Region" class.
 *
 * Copyright (C) 2003  Neil Cawse.
 * Based on work from the Wine Project
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

/*
 * GDI region objects. Shamelessly ripped out from the X11 distribution
 * Thanks for the nice licence.
 *
 * Copyright 1993, 1994, 1995 Alexandre Julliard
 * Modifications and additions: Copyright 1998 Huw Davies
 *                                                           1999 Alex Korobka
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

/* Copyright (c) 1987, 1988  X Consortium
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *  
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *  
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 * X CONSORTIUM BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
 * AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *  
 * Except as contained in this notice, the name of the X Consortium shall not be
 * used in advertising or otherwise to promote the sale, use or other dealings
 * in this Software without prior written authorization from the X Consortium.
 *  
 * Copyright 1987, 1988 by Digital Equipment Corporation, Maynard, Massachusetts.
 *  
 * All Rights Reserved
 *  
 * Permission to use, copy, modify, and distribute this software and its
 * documentation for any purpose and without fee is hereby granted,
 * provided that the above copyright notice appear in all copies and that
 * both that copyright notice and this permission notice appear in
 * supporting documentation, and that the name of Digital not be
 * used in advertising or publicity pertaining to distribution of the
 * software without specific, written prior permission.
 *  
 * DIGITAL DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE, INCLUDING
 * ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO EVENT SHALL
 * DIGITAL BE LIABLE FOR ANY SPECIAL, INDIRECT OR CONSEQUENTIAL DAMAGES OR
 * ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
 * WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
 * ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS
 * SOFTWARE.
 */

namespace System.Drawing
{

using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class Region : MarshalByRefObject, IDisposable
{
	// Internal byte representation of the region
	private RegionData rgnData;
	// Overall extent of the region
	private RectangleF extent;
	// rectangles that make up the region sorted top to bottom, left to right
	private RectangleF[] rects;

	// Constructors.
	public Region()
	{
		rgnData = new RegionData() ;
		MakeInfinite();
	}

	public Region(GraphicsPath path)
	{
		if(path == null)
		{
			throw new ArgumentNullException("path", "Argument cannot be null");
		}
		rgnData = new RegionData( path );
	}

	public Region(Rectangle rect) : this( (RectangleF)rect)
	{}

	public Region(RectangleF rect)
	{
		rgnData = new RegionData( rect );
		if (rect.Width < 0)
			rect = new RectangleF(rect.Right, rect.Top, -rect.Width, rect.Height);
		if (rect.Height < 0)
			rect = new RectangleF(rect.Left, rect.Bottom, rect.Width, -rect.Height);

		if(rect.Width != 0 || rect.Height != 0)
		{
			extent = rect;
			rects = new RectangleF[1];
			rects[0] = rect;
		}
		else
		{
			// extent is already empty(0,0,0,0)
			rects = new RectangleF[0];
		}
	}

	private Region(int initialRectSize)
	{
		rects = new RectangleF[initialRectSize];
	}

	public Region(RegionData otherRgnData)
	{
		if(otherRgnData == null)
		{
			throw new ArgumentNullException("rgnData", "Argument cannot be null");
		}
		Region r = otherRgnData.ConstructRegion ( otherRgnData ) ;
		this.rects = r.rects ;
		this.extent = r.extent ;
		this.rgnData = r.GetRegionData() ;
	}

	// Helpers, to replace missing "Math" class in some profiles.
	private static int Math_Min(int a, int b)
	{
		if(a < b)
		{
			return a;
		}
		else
		{
			return b;
		}
	}
	private static float Math_Min(float a, float b)
	{
		if(a < b)
		{
			return a;
		}
		else
		{
			return b;
		}
	}
	private static int Math_Max(int a, int b)
	{
		if(a > b)
		{
			return a;
		}
		else
		{
			return b;
		}
	}
	private static float Math_Max(float a, float b)
	{
		if(a > b)
		{
			return a;
		}
		else
		{
			return b;
		}
	}

	// Make an exact copy of this region.
	public Region Clone()
	{
		Region newRegion = new Region();
		newRegion.rgnData = rgnData.Clone();
		newRegion.rects = (RectangleF[])rects.Clone();
		newRegion.extent = extent;
		return newRegion;
	}

	// Form the complement of subtracting this region from another.
	public void Complement(GraphicsPath path)
	{
		Complement ( new Region(path));
	}

	public void Complement(Rectangle rect)
	{
		Complement( new Region(rect));
	}

	public void Complement(RectangleF rect)
	{
		Complement( new Region(rect));
	}

	public void Complement(Region region)
	{
		rgnData.Complement( region ) ;
		Region subtract = Subtract(region, this);
		extent = subtract.extent;
		rects = subtract.rects;
	}

	// Dispose of this region.
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		// Nothing to do in this implementation.
	}

	~Region()
	{
		Dispose(false);
	}

	// Determine if two regions are equal after applying a transformation.
	// TODO transformation
	public bool Equals(Region region, Graphics g)
	{
		if(region == null)
		{
			throw new ArgumentNullException("region", "Argument cannot be null");
		}
		if(g == null)
		{
			throw new ArgumentNullException("g", "Argument cannot be null");
		}

		if (rects.Length == 0)
			return true;
		if (region.rects.Length != rects.Length | region.extent != extent)
			return false;
		for (int i = 0; i < rects.Length; i++)
		{
			if (rects[i] != region.rects[i])
				return false;
		}
		return true;
	}

	// Subtract another region from this one.
	public void Exclude(GraphicsPath path)
	{
		Exclude( new Region(path));
	}
	
	public void Exclude(Rectangle rect)
	{
		Exclude( new Region(rect));
	}

	public void Exclude(RectangleF rect)
	{
		Exclude( new Region(rect));
	}

	public void Exclude(Region region)
	{
		rgnData.Exclude( region ) ;
		Region subtract = Subtract(this, region);
		extent = subtract.extent;
		rects = subtract.rects;
	}

	// Create a region object from a HRGN.
	[TODO]
	public static Region FromHrgn(IntPtr hrgn)
	{
		return new Region();
	}

	// Get the bounds of this region on a particular graphics object.
	public RectangleF GetBounds(Graphics graphics)
	{
		return extent;
	}

	// Get a HRGN object for this region.
	public IntPtr GetHrgn(Graphics g)
	{
		// Not used in this implementation
		return IntPtr.Zero;
	}

	// Get the raw region data for this region.
	public RegionData GetRegionData()
	{
		return rgnData;
	}

	// Get an array of rectangles that represents this region.
	[TODO]
	public RectangleF[] GetRegionScans(Matrix matrix)
	{
		//HACK: this is temporary and will not work on transforms with shear.
		RectangleF[] newRects = new RectangleF[rects.Length];
		for(int i = 0; i < newRects.Length; i++)
		{
			RectangleF r = rects[i];
			float ox, oy, or, ob;
			matrix.TransformPoint(r.X, r.Y, out ox, out oy);
			matrix.TransformPoint(r.Right, r.Bottom, out or, out ob);
			newRects[i] = RectangleF.FromLTRB(ox, oy, or, ob);
		}
		return newRects;
	}

	internal RectangleF[] GetRegionScansIdentity()
	{
		return rects;
	}

	// Form the intersection of this region and another.
	public void Intersect(GraphicsPath path)
	{
		Intersect(new Region(path));
	}
	
	public void Intersect(Rectangle rect)
	{
		Intersect(new Region(rect));
	}
	
	public void Intersect(RectangleF rect)
	{
		Intersect(new Region(rect));
	}
	
	public void Intersect(Region region)
	{
		rgnData.Intersect(region);
		Intersect(this, region);
	}

	// Determine if this region is empty on a particular graphics object.
	public bool IsEmpty(Graphics g)
	{
		// TODO Transformation
		return extent.IsEmpty;
	}

	// Determine if this region is infinite on a particular graphics object.
	[TODO]
	public bool IsInfinite(Graphics g)
	{
		// TODO
		return false;
	}

	// Determine if a point is contained within this region.
	public bool IsVisible(Point point)
	{
		return IsVisible((PointF)point);
	}
	
	public bool IsVisible(PointF point)
	{
		if (extent.Contains( point ))
			for (int i = 0; i < rects.Length; i++)
				if (rects[i].Contains( point))
					return true;
		return false;
	}

	public bool IsVisible(Rectangle rect)
	{
		return IsVisible((RectangleF)rect);
	}
	
	public bool IsVisible(RectangleF rect)
	{
		if (rects.Length > 0 && extent.IntersectsWith( rect))
		{
			for (int i = 0; i < rects.Length; i++)
			{
				RectangleF currentRect = rects[i];
				// Not far enough down yet
				if (currentRect.Bottom <= rect.Top)
					continue;
				// Too far down
				if (currentRect.Top >= rect.Bottom)
					break;
				// Not far over enough yet
				if (currentRect.Right <= rect.Left)
					continue;
				if (currentRect.Left >= rect.Right) 
					continue;
				return true;
			}
		}
		return false;
	}

	public bool IsVisible(Point point, Graphics g)
	{
		return IsVisible((PointF)point, g);
	}
	
	[TODO]
	public bool IsVisible(PointF point, Graphics g)
	{
		// TODO
		return false;
	}

	public bool IsVisible(Rectangle rect, Graphics g)
	{
		return IsVisible((RectangleF)rect, g);
	}
	
	[TODO]
	public bool IsVisible(RectangleF rect, Graphics g)
	{
		// TODO
		return false;
	}

	public bool IsVisible(float x, float y)
	{
		return IsVisible(new PointF(x, y));
	}

	public bool IsVisible(int x, int y, Graphics g)
	{
		return IsVisible(new PointF(x, y), g);
	}

	public bool IsVisible(float x, float y, Graphics g)
	{
		return IsVisible(new PointF(x, y), g);
	}
	
	public bool IsVisible(int x, int y, int width, int height)
	{
		return IsVisible(new RectangleF(x, y, width, height));
	}

	public bool IsVisible(float x, float y, float width, float height)
	{
		return IsVisible(new RectangleF(x, y, width, height));
	}

	public bool IsVisible(int x, int y, int width, int height, Graphics g)
	{
		return IsVisible(new RectangleF(x, y, width, height), g);
	}

	public bool IsVisible(float x, float y, float width,
		float height, Graphics g)
	{
		return IsVisible(new RectangleF(x, y, width, height), g);
	}

	// Make this region empty.
	public void MakeEmpty()
	{
		rgnData.MakeEmpty() ;
		rects = new RectangleF[0];
		extent = Rectangle.Empty;
	}

	// Make this region infinite.
	public void MakeInfinite()
	{
		rgnData.MakeInfinite() ;
		const float maxCoord = 4194304; //Math.Pow(2, 22)
		extent = new RectangleF(-maxCoord, -maxCoord, 2 * maxCoord, 2 * maxCoord);
		rects = new RectangleF[1] { extent };
	}

	// Transform this region using a specified matrix.
	[TODO]
	public void Transform(Matrix matrix)
	{
		rgnData.Transform( matrix ) ;
		// TODO: other state variables
	}

	// Translate this matrix by a specific amount.
	public void Translate(int dx, int dy)
	{
		Translate((float)dx, (float)dy);
	}
	
	public void Translate(float dx, float dy)
	{
		rgnData.Translate( dx, dy ) ;
		for( int i = 0; i < rects.Length; i++)
			rects[i].Offset(dx, dy);
		if (rects.Length>0)
			extent.Offset(dx, dy);
	}

	// Form the union of this region and another.
	public void Union(GraphicsPath path)
	{
		Union( new Region(path));
	}

	public void Union(Rectangle rect)
	{
		Union( new Region(rect));
	}

	public void Union(RectangleF rect)
	{
		Union( new Region(rect));
	}
	
	public void Union(Region region)
	{
		rgnData.Union( region );
		Union (this, region);
	}

	// Union of reg1 and reg2
	public static void Union ( Region reg1, Region reg2)
	{
		if ( reg1 == reg2 | reg2.rects.Length == 0 )
			return;
		if ( reg1.rects.Length == 0 )
		{
			reg1.rects = (RectangleF[])reg2.rects.Clone();
			reg1.extent = reg2.extent;
			return;
		}
		// Region 1 completely subsumes region 2
		RectangleF reg1Extent = reg1.extent;
		RectangleF reg2Extent = reg2.extent;
		if ( reg1.rects.Length == 1 && reg1Extent.Left <= reg2Extent.Left && reg1Extent.Top <= reg2Extent.Top && reg1Extent.Right >= reg2Extent.Right && reg1Extent.Bottom >= reg2Extent.Bottom )
			return;

		// Region 2 completely subsumes region 1
		if ( reg2.rects.Length == 1 && (reg2Extent.Left <= reg1Extent.Left) && reg2Extent.Top <= reg1Extent.Top && reg2Extent.Right >= reg1Extent.Right && reg2Extent.Bottom >= reg1Extent.Bottom )
		{
			reg1.rects = (RectangleF[])reg2.rects.Clone();
			reg1.extent = reg2.extent;
			return;
		}

		Region union = RegionOperation( reg1, reg2, RegionOperationType.Union);
		reg1.rects = union.rects;
		reg1.extent = RectangleF.Union( reg1.extent, reg2.extent);
	}

	// Subtract reg2 from reg1
	private static Region Subtract ( Region reg1, Region reg2)
	{
		RectangleF extent1 = reg1.extent;
		if (reg1.rects.Length==0 || reg2.rects.Length == 0 || !extent1.IntersectsWith(reg2.extent))
		{
			Region region = new Region();
			region.extent = reg1.extent;
			region.rects = (RectangleF[])reg1.rects.Clone();
			return region;
		}
		else
		{
			Region subtract = RegionOperation( reg1, reg2, RegionOperationType.Subtract);
			CalculateExtents(subtract);
			return subtract;
		}
	}

	// Creates a new array and copies when we run out of space. Doubles the size.

	// Intersection of reg1 and reg2
	private static void Intersect ( Region reg1, Region reg2)
	{
		// Trivial case
		RectangleF reg1Extent = reg1.extent;
		if ( reg1.rects.Length == 0 || reg2.rects.Length == 0  || !reg1Extent.IntersectsWith(reg2.extent))
		{
			reg1.MakeEmpty();
			return;
		}
		Region intersect = RegionOperation( reg1, reg2, RegionOperationType.Intersect);
		CalculateExtents(intersect);
		reg1.rects = intersect.rects;
		reg1.extent = intersect.extent;
	}

	private static void AllocateSpace( Region reg, int nextRectangle)
	{
		// If we have run out of space in the array, create a new copy with double the size
		if (reg.rects.Length == nextRectangle)
		{
			RectangleF[] newRects = new RectangleF[reg.rects.Length * 2];
			Array.Copy(reg.rects, newRects, reg.rects.Length);
			reg.rects = newRects;
		}
	}

	// Re-calculate the extents of a region
	private static void CalculateExtents (Region reg)
	{
		if (reg.rects.Length == 0)
		{
			reg.extent = RectangleF.Empty;
			return;
		}

		/* Since rectStart is the first rectangle in the region, it must have the
		 * smallest top and since rectEnd is the last rectangle in the region,
		 * it must have the largest bottom, because of banding.
		 */
		RectangleF rectStart = reg.rects[0];
		float left = rectStart.Left;
		float top = rectStart.Top;
		RectangleF rectEnd = reg.rects[reg.rects.Length-1];
		float right = rectEnd.Right;
		float bottom = rectEnd.Bottom;

		for (int i = 0; i < reg.rects.Length; i++)
		{
			RectangleF rect = reg.rects[i];
			if (rect.Left < left)
				left = rect.Left;
			if (rect.Right > right)
				right = rect.Right;
		}
		reg.extent = RectangleF.FromLTRB(left, top, right, bottom);
		
	}

	/* Handle a non-overlapping band for the union operation. Just
	 * Adds the rectangles into the region. Doesn't have to check for
	 * subsumption or anything.
	 * nextRectangle is incremented and the final rectangles overwritten
	 * with the rectangles we're passed.
	 */
	private static void UnionNonOverlapBands (Region regNew, ref int nextRectangle, Region reg, int rect, int rectEnd, float top, float bottom)
	{
	 
		int nextRect = nextRectangle;

		while (rect != rectEnd)
		{
			AllocateSpace( regNew, nextRectangle);
			RectangleF curRect = reg.rects[rect];
			regNew.rects[nextRect] = RectangleF.FromLTRB(curRect.Left, top, curRect.Right, bottom);
			nextRectangle += 1;
			nextRect++;
			rect++;
		}
	}

	/* Handle an overlapping band for the union operation. Picks the
	 * left-most rectangle each time and merges it into the region.
	 * Rectangles are overwritten in reg.rects and nextRectangle will
	 * be changed.
	 */
	private static void UnionOverlapBands (Region regNew, ref int nextRectangle, Region reg1, int r1, int r1End, Region reg2, int r2, int r2End, float top, float bottom)
	{
		while (r1 != r1End && r2 != r2End)
			if (reg1.rects[r1].Left < reg2.rects[r2].Left)
				UnionOverlapBandsMerge(reg1, ref r1, regNew, ref nextRectangle, top, bottom);
			else
				UnionOverlapBandsMerge(reg2, ref r2, regNew, ref nextRectangle, top, bottom);

		if (r1 != r1End)
			while (r1 != r1End)
				UnionOverlapBandsMerge(reg1, ref r1, regNew, ref nextRectangle, top, bottom);
		else
			while (r2 != r2End)
				 UnionOverlapBandsMerge(reg2, ref r2, regNew, ref nextRectangle, top, bottom);
	}

	private static void UnionOverlapBandsMerge(Region reg, ref int r, Region regNew, ref int nextRectangle, float top, float bottom)
	{
		bool addNew = false;
		
		RectangleF rRect = reg.rects[r];
		if (nextRectangle == 0)
			addNew = true;
		else
		{
			RectangleF rect = regNew.rects[nextRectangle - 1];
			if (nextRectangle != 0 && rect.Top == top && rect.Bottom == bottom && rect.Right >= rRect.Left)
			{
				if (rect.Right < rRect.Right)
					regNew.rects[nextRectangle - 1]= RectangleF.FromLTRB( rect.Left, rect.Top, rRect.Right, rect.Bottom);
			}
			else
				addNew = true;
		}
		if (addNew)
		{
			AllocateSpace( regNew, nextRectangle);
			regNew.rects[nextRectangle] = RectangleF.FromLTRB(rRect.Left, top, rRect.Right, bottom);
			nextRectangle++;
		}
		r++;
	}

	/* Overlapping band subtraction. x1 is the left-most point not yet checked
	 */
	private static void SubtractOverlapBands(Region regNew, ref int nextRectangle, Region reg1, int r1, int r1End, Region reg2, int r2, int r2End, float top, float bottom)
	{
		float left = reg1.rects[r1].Left;
		RectangleF rect1 = Rectangle.Empty;
		RectangleF rect2 = Rectangle.Empty;
		if (r1 != r1End)
			rect1 = reg1.rects[r1];
		if (r2 != r2End)
			rect2 = reg2.rects[r2];
			
		while (r1 != r1End && r2 != r2End)
		{
			if (rect2.Right <= left)
			{
				// Subtrahend missed the boat: go to next subtrahend.
				r2++;
				if (r2 != r2End)
					rect2 = reg2.rects[r2];
			}
			else if (rect2.Left <= left)
			{
				 // Subtrahend preceeds minuend: nuke left edge of minuend.
				left = rect2.Right;
				if (left >= rect1.Right)
				{
					/* Minuend completely covered: advance to next minuend and
					 * reset left fence to edge of new minuend.
					 */
					r1++;
					if (r1 != r1End)
					{
						rect1 = reg1.rects[r1];
						left = rect1.Left;
					}				
				}
				else
				{
					/* Subtrahend now used up since it doesn't extend beyond
					 * minuend
					 */
					r2++;
					if (r2 != r2End)
						rect2 = reg2.rects[r2];
				}
			}
			else if (rect2.Left < rect1.Right)
			{
				/* Left part of subtrahend covers part of minuend: add uncovered
				 * part of minuend to region and skip to next subtrahend.
				 */
				AllocateSpace(regNew, nextRectangle);
				regNew.rects[nextRectangle++] = RectangleF.FromLTRB(left, top, rect2.Left, bottom);
				left = rect2.Right;
				if (left >= rect1.Right)
				{
					// Minuend used up: advance to new...
					r1++;
					if (r1 != r1End)
					{
						rect1 = reg1.rects[r1];
						left = rect1.Left;
					}
				}
				else
				{
					// Subtrahend used up
					r2++;
					if (r2 != r2End)
						rect2 = reg2.rects[r2];
				}
			}
			else
			{
				// Minuend used up: add any remaining piece before advancing.
				if (rect1.Right > left)
				{
					AllocateSpace(regNew, nextRectangle);
					regNew.rects[nextRectangle++] = RectangleF.FromLTRB(left, top, rect1.Right, bottom);
				}
				r1++;
				if (r1 != r1End)
				{
					rect1 = reg1.rects[r1];
					left = rect1.Left;
				}
			}
		}

		// Add remaining minuend rectangles to region.
		while (r1 != r1End)
		{
			AllocateSpace(regNew, nextRectangle);
			regNew.rects[nextRectangle++] = RectangleF.FromLTRB(left, top, rect1.Right, bottom);
			r1++;
			if (r1 != r1End)
			{
				rect1 = reg1.rects[r1];
				left = rect1.Left;
			}
		}
	}

	/* Deal with non-overlapping band for subtraction. Any parts from
	 * region 2 we discard. Anything from region 1 we add to the region.
	 */
	private static void SubtractNonOverlapBands(Region regNew, ref int nextRectangle, Region reg, int r, int rEnd, float top, float bottom)
	{
		while (r != rEnd)
		{
			AllocateSpace(regNew, nextRectangle);
			RectangleF rect = reg.rects[r];
			regNew.rects[nextRectangle++] = RectangleF.FromLTRB(rect.Left, top, rect.Right, bottom);
			r++;
		}
	}

	/* Handle an overlapping band for REGION_Intersect.
	 * Rectangles may be added to the region.
	 */
	private static void IntersectOverlapBands(Region regNew, ref int nextRectangle, Region reg1, int r1, int r1End, Region reg2, int r2, int r2End, float top, float bottom)
	{
		float left, right;

		while (r1 != r1End && r2 != r2End)
		{
			RectangleF rect1 = reg1.rects[r1];
			RectangleF rect2 = reg2.rects[r2];
			left = Math_Max(rect1.Left, rect2.Left);
			right =	Math_Min(rect1.Right, rect2.Right);

			/*
			 * If there's any overlap between the two rectangles, add that
			 * overlap to the new region.
			 * There's no need to check for subsumption because the only way
			 * such a need could arise is if some region has two rectangles
			 * right next to each other. Since that should never happen...
			 */
			if (left < right)
			{
				AllocateSpace(regNew, nextRectangle);
				regNew.rects[nextRectangle++] = RectangleF.FromLTRB(left, top, right, bottom);
			}

			/*
			 * Need to advance the pointers. Shift the one that extends
			 * to the right the least, since the other still has a chance to
			 * overlap with that region's next rectangle, if you see what I mean.
			 */
			if (rect1.Right < rect2.Right)
				r1++;
			else if (rect2.Right < rect1.Right)
				r2++;
			else
			{
				r1++;
				r2++;
			}
		}
	}

	/* Attempt to merge the rects in the current band with those in the
	 * previous one. Used only by RegionOperation.
	 * Results: The new index for the previous band.
	 *
	 * If coalescing takes place:
	 * - rectangles in the previous band will have their bottom fields
	 * altered.
	 * - nextRectangle will be decreased.
	 */
	private static int CoalesceRegion ( Region reg, ref int nextRectangle,
		int prevStart,  /* Index of start of previous band */
		int curStart    /* Index of start of current band */
		)
	{
	
		int curNumRects; /* Number of rectangles in current band */
		int regEnd = nextRectangle; /* End of region */
		int prevRect = prevStart; /* Current rect in previous band */
		int prevNumRects = curStart - prevStart; /* Number of rectangles in previous band */
		int curRect = curStart; /* Current rect in current band */
		float bandtop = reg.rects[curRect].Top; /* top coordinate for current band */
    
		/* Figure out how many rectangles are in the current band. Have to do
		 * this because multiple bands could have been added in REGION_RegionOp
		 * at the end when one region has been exhausted.
		 */
		for (curNumRects = 0; curRect != regEnd && reg.rects[curRect].Top == bandtop; curNumRects++)
			curRect++;

		if (curRect != regEnd)
		{
			/*
			 * If more than one band was added, we have to find the start
			 * of the last band added so the next coalescing job can start
			 * at the right place... (given when multiple bands are added,
			 * this may be pointless -- see above).
			 */
			regEnd--;
			while (reg.rects[regEnd -1].Top == reg.rects[regEnd].Top)
				regEnd--;
			curStart = regEnd;
			regEnd = nextRectangle;
		}

		if (curNumRects == prevNumRects && curNumRects != 0)
		{
			curRect -= curNumRects;
			/*
			 * The bands may only be coalesced if the bottom of the previous
			 * matches the top scanline of the current.
			 */
			if (reg.rects[prevRect].Bottom == reg.rects[curRect].Top)
			{
				/*
				 * Make sure the bands have rects in the same places. This
				 * assumes that rects have been added in such a way that they
				 * cover the most area possible. I.e. two rects in a band must
				 * have some horizontal space between them.
				 */
				do
				{
					if (reg.rects[prevRect].Left != reg.rects[curRect].Left ||
						reg.rects[prevRect].Right != reg.rects[curRect].Right)
					{
						/*
						 * The bands don't line up so they can't be coalesced.
						 */
						return curStart;
					}
					prevRect++;
					curRect++;
					prevNumRects -= 1;
				}
				while (prevNumRects != 0);

				nextRectangle -= curNumRects;
				curRect -= curNumRects;
				prevRect -= curNumRects;

				/*
				 * The bands may be merged, so set the bottom of each rect
				 * in the previous band to that of the corresponding rect in
				 * the current band.
				 */
				do
				{
					RectangleF previousRectangle = reg.rects[prevRect];
					reg.rects[prevRect] = RectangleF.FromLTRB(previousRectangle.Left, previousRectangle.Top, previousRectangle.Right, reg.rects[curRect].Bottom);
					prevRect++;
					curRect++;
					curNumRects -= 1;
				}
				while (curNumRects != 0);

				/*
				 * If only one band was added to the region, we have to backup
				 * curStart to the start of the previous band.
				 *
				 * If more than one band was added to the region, copy the
				 * other bands down. The assumption here is that the other bands
				 * came from the same region as the current one and no further
				 * coalescing can be done on them since it's all been done
				 * already... curStart is already in the right place.
				 */

				if (curRect == regEnd)
					curStart = prevStart;
				else
				{
					do
					{
						reg.rects[prevRect++] = reg.rects[curRect++];
					}
					while (curRect != regEnd);
				}

			}
		}
		return curStart;
	}

	private enum RegionOperationType
	{
		Intersect,
		Union,
		Subtract
	}
	
	/*      Apply an operation to two regions.
	*
	*      The idea behind this function is to view the two regions as sets.
	*      Together they cover a rectangle of area that this function divides
	*      into horizontal bands where points are covered only by one region
	*      or by both. For the first case, the nonOverlapFunc is called with
	*      each the band and the band's upper and lower extents. For the
	*      second, the overlapFunc is called to process the entire band. It
	*      is responsible for clipping the rectangles in the band, though
	*      this function provides the boundaries.
	*      At the end of each band, the new region is coalesced, if possible,
	*      to reduce the number of rectangles in the region.
	*/

	private static Region RegionOperation( Region reg1, Region reg2, RegionOperationType regionOperationType)
	{
		float ybot; /* Bottom of intersection */
		float ytop; /* Top of intersection */
		int prevBand; /* Index of start of previous band in newReg */
		int curBand; /* Index of start of current band in newReg */
		int r1BandEnd; /* End of current band in r1 */
		int r2BandEnd; /* End of current band in r2 */
		float top; /* Top of non-overlapping band */
		float bot; /* Bottom of non-overlapping band */

		/* Initialization:
		 *  set r1, r2, r1End and r2End appropriately
		 */
		int r1 = 0;
		int r1End = reg1.rects.Length; /* End of 1st region */
		RectangleF reg1Extent = reg1.extent;
		int r2 = 0;
		int r2End = reg2.rects.Length; /* End of 2nd region */
		RectangleF reg2Extent = reg2.extent;

		RectangleF rect1;
		RectangleF rect2;

		/*
		 * Allocate a reasonable number of rectangles for the new region. The idea
		 * is to allocate enough so the individual functions don't need to
		 * reallocate and copy the array, which is time consuming, yet we don't
		 * want to use too much memory.
		 */
		Region newReg = new Region(Math_Max(reg1.rects.Length, reg2.rects.Length) * 2);
		// The total number of rectangles we have written to the array
		int nextRectangle = 0;
    
		/*
		 * Initialize ybot and ytop.
		 * In the upcoming loop, ybot and ytop serve different functions depending
		 * on whether the band being handled is an overlapping or non-overlapping
		 * band.
		 * In the case of a non-overlapping band (only one of the regions
		 * has points in the band), ybot is the bottom of the most recent
		 * intersection and thus clips the top of the rectangles in that band.
		 * ytop is the top of the next intersection between the two regions and
		 * serves to clip the bottom of the rectangles in the current band.
		 *  For an overlapping band (where the two regions intersect), ytop clips
		 * the top of the rectangles of both regions and ybot clips the bottoms.
		 */

		if (reg1Extent.Top < reg2Extent.Top)
			ybot = reg1Extent.Top;
		else
			ybot = reg2Extent.Top;

		/*
		 * prevBand serves to mark the start of the previous band so rectangles
		 * can be coalesced into larger rectangles. qv. miCoalesce, above.
		 * In the beginning, there is no previous band, so prevBand == curBand
		 * (curBand is set later on, of course, but the first band will always
		 * start at index 0). prevBand and curBand must be indices because of
		 * the possible expansion, and resultant moving, of the new region's
		 * array of rectangles.
		 */
		prevBand = 0;

		do
		{
			rect1 = reg1.rects[r1];
			rect2 = reg2.rects[r2];
			curBand = nextRectangle;

			/*
			 * This algorithm proceeds one source-band (as opposed to a
			 * destination band, which is determined by where the two regions
			 * intersect) at a time. r1BandEnd and r2BandEnd serve to mark the
			 * rectangle after the last one in the current band for their
			 * respective regions.
			 */
			r1BandEnd = r1;
			while (r1BandEnd != r1End && reg1.rects[r1BandEnd].Top == rect1.Top)
				r1BandEnd++;

				r2BandEnd = r2;
			while (r2BandEnd != r2End && reg2.rects[r2BandEnd].Top == rect2.Top)
				r2BandEnd++;

			/*
			 * First handle the band that doesn't intersect, if any.
			 *
			 * Note that attention is restricted to one band in the
			 * non-intersecting region at once, so if a region has n
			 * bands between the current position and the next place it overlaps
			 * the other, this entire loop will be passed through n times.
			 */
			if (rect1.Top < rect2.Top)
			{
				top = Math_Max(rect1.Top, ybot);
				bot = Math_Min(rect1.Bottom, rect2.Top);

				if ((top != bot))
				{
					if (regionOperationType == RegionOperationType.Subtract)
						SubtractNonOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, top, bot);
					else if (regionOperationType == RegionOperationType.Union)
						UnionNonOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, top, bot);
				}

				ytop = rect2.Top;
			}
			else if (rect2.Top < rect1.Top)
			{
				top = Math_Max(rect2.Top, ybot);
				bot = Math_Min(rect2.Bottom, rect1.Top);

				if (top != bot)
				{
					if (regionOperationType == RegionOperationType.Union)
						UnionNonOverlapBands(newReg, ref nextRectangle, reg2, r2, r2BandEnd, top, bot);
				}

				ytop = rect1.Top;
			}
			else
				ytop = rect1.Top;

			/*
			 * If any rectangles got added to the region, try and coalesce them
			 * with rectangles from the previous band. Note we could just do
			 * this test in miCoalesce, but some machines incur a not
			 * inconsiderable cost for function calls, so...
			 */
			if (nextRectangle != curBand)
			{
				prevBand = CoalesceRegion(newReg, ref nextRectangle, prevBand, curBand);
			}

			/*
			 * Now see if we've hit an intersecting band. The two bands only
			 * intersect if ybot > ytop
			 */
			ybot = Math_Min(rect1.Bottom, rect2.Bottom);
			curBand = nextRectangle;
			if (ybot > ytop)
			{
				if (regionOperationType == RegionOperationType.Subtract)
					SubtractOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, reg2, r2, r2BandEnd, ytop, ybot);
				else if (regionOperationType == RegionOperationType.Union)
					UnionOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, reg2, r2, r2BandEnd, ytop, ybot);
				else if (regionOperationType == RegionOperationType.Intersect)
					IntersectOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, reg2, r2, r2BandEnd, ytop, ybot);
			}

			if (nextRectangle != curBand)
				prevBand = CoalesceRegion (newReg, ref nextRectangle, prevBand, curBand);

			/*
			 * If we've finished with a band (bottom == ybot) we skip forward
			 * in the region to the next band.
			 */
			if (rect1.Bottom == ybot)
				r1 = r1BandEnd;
			if (rect2.Bottom == ybot)
				r2 = r2BandEnd;
		}
		while (r1 != r1End && r2 != r2End);

		/*
		 * Deal with whichever region still has rectangles left.
		 */
		curBand = nextRectangle;
		if (r1 != r1End)
		{
			if (regionOperationType == RegionOperationType.Subtract || regionOperationType == RegionOperationType.Union)
			{
				do
				{
					rect1 = reg1.rects[r1];
					r1BandEnd = r1;
					while (r1BandEnd < r1End && reg1.rects[r1BandEnd].Top == rect1.Top)
						r1BandEnd++;
					if (regionOperationType == RegionOperationType.Subtract)
						SubtractNonOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, Math_Max(rect1.Top, ybot), rect1.Bottom);
					else if (regionOperationType == RegionOperationType.Union)
						UnionNonOverlapBands(newReg, ref nextRectangle, reg1, r1, r1BandEnd, Math_Max(rect1.Top, ybot), rect1.Bottom);
					r1 = r1BandEnd;
				}
				while (r1 != r1End);
			}
		}
		else if (r2 != r2End && regionOperationType == RegionOperationType.Union)
		{
			do
			{
				rect2 = reg2.rects[r2];
				r2BandEnd = r2;
				while (r2BandEnd < r2End && reg2.rects[r2BandEnd].Top == rect2.Top)
					r2BandEnd++;
				UnionNonOverlapBands(newReg, ref nextRectangle, reg2, r2, r2BandEnd, Math_Max(rect2.Top, ybot), rect2.Bottom);
				r2 = r2BandEnd;
			} while (r2 != r2End);
		}

		if (nextRectangle != curBand)
			CoalesceRegion (newReg, ref nextRectangle, prevBand, curBand);

		/*
		 * A bit of cleanup. To keep regions from growing without bound,
		 * we shrink the array of rectangles to match the new number of
		 * rectangles in the region. This never goes to 0, however...
		 *
		 * Only do this stuff if the number of rectangles allocated is more than
		 * twice the number of rectangles in the region (a simple optimization...).
		 */
		if (nextRectangle < newReg.rects.Length)
		{
			RectangleF[] newRects = new RectangleF[nextRectangle];
			Array.Copy(newReg.rects, newRects, nextRectangle);
			newReg.rects = newRects;
		}
		return newReg;
	}


	// Form the XOR of this region and another.
	public void Xor(GraphicsPath path)
	{
		Xor(new Region(path));
	}
	
	public void Xor(Rectangle rect)
	{
		Xor(new Region(rect));
	}
	
	public void Xor(RectangleF rect)
	{
		Xor(new Region(rect));
	}
	
	public void Xor(Region region)
	{
		rgnData.Xor( region ) ;
		Region reg1 = Subtract(this, region);
		Union(reg1, Subtract(region, this));
		extent = reg1.extent;
		rects = reg1.rects;
	}

}; // class Region

}; // namespace System.Drawing
