/*
 * Region.cs - Region management for X applications.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp
{

using System;
using Xsharp.Types;

/// <summary>
/// <para>The <see cref="T:Xsharp.Region"/> class manages a
/// region structure, constructed from a list of rectangles.</para>
///
/// <para>Regions are used in X to manage non-rectangular clip regions,
/// window shapes, and expose areas.</para>
/// </summary>
public sealed class Region : IDisposable, ICloneable
{
	// Pointer to the raw X11 region structure.
	private IntPtr region;

	/// <summary>
	/// <para>Construct a new <see cref="T:Xsharp.Region"/>
	/// instance that is initially set to the empty region.</para>
	/// </summary>
	public Region()
			{
				lock(typeof(Region))
				{
					region = Xlib.XCreateRegion();
					if(region == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
				}
			}

	/// <summary>
	/// <para>Construct a new <see cref="T:Xsharp.Region"/>
	/// instance that is initially set to the same area as another
	/// region object.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region object to copy.  If <paramref name="r"/>
	/// is <see langword="null"/> or has been disposed, the new region
	/// will be set to the empty region.</para>
	/// </param>
	public Region(Region r)
			{
				lock(typeof(Region))
				{
					region = Xlib.XCreateRegion();
					if(region == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
					Union(r);
				}
			}

	/// <summary>
	/// <para>Construct a new <see cref="T:Xsharp.Region"/>
	/// instance that is initially set to the same area as
	/// a rectangle.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to set the region to initially.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	public Region(Rectangle rect)
			{
				lock(typeof(Region))
				{
					region = Xlib.XCreateRegion();
					if(region == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
					Union(rect.x, rect.y, rect.width, rect.height);
				}
			}

	/// <summary>
	/// <para>Construct a new <see cref="T:Xsharp.Region"/>
	/// instance that is initially set to the same area as
	/// an explicitly-specified rectangle.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	public Region(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					region = Xlib.XCreateRegion();
					if(region == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
					Union(x, y, width, height);
				}
			}

	/// <summary>
	/// <para>Construct a new <see cref="T:Xsharp.Region"/>
	/// instance that is initially set to a polygon.</para>
	/// </summary>
	///
	/// <param name="points">
	/// <para>An array of points that defines the polygon.</para>
	/// </param>
	///
	/// <param name="fillRule">
	/// <para>The area fill rule to use for the polygon.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="points"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	/// <para>Raised if <paramref name="points"/> has less than 3
	/// elements.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if any of the elements in <paramref name="points"/>
	/// has co-ordinates that are out of range, or if
	/// <paramref name="fillRule"/> is invalid.</para>
	/// </exception>
	public Region(Point[] points, FillRule fillRule)
			{
				// Validate the parameters.
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				else if(points.Length < 3)
				{
					throw new ArgumentOutOfRangeException
						("points", S._("X_PolygonNeeds3Pts"));
				}

				// Convert "points" into an "XPoint[]" array.
				XPoint[] pts = new XPoint [points.Length];
				try
				{
					checked
					{
						for(int index = 0; index < points.Length; ++index)
						{
							pts[index].x = (short)(points[index].x);
							pts[index].y = (short)(points[index].y);
						}
					}
				}
				catch(OverflowException)
				{
					throw new XException(S._("X_PointCoordRange"));
				}

				// Validate the fill rule.
				if(fillRule != FillRule.EvenOddRule &&
				   fillRule != FillRule.WindingRule)
				{
					throw new XException
						(String.Format(S._("X_FillRule"), (int)fillRule));
				}

				// Create the polygon region.
				lock(typeof(Region))
				{
					region = Xlib.XPolygonRegion(pts, pts.Length,
												 (int)fillRule);
					if(region == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
				}
			}

	/// <summary>
	/// <para>Destroy an instance of <see cref="T:Xsharp.Region"/>.</para>
	/// </summary>
	~Region()
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						Xlib.XDestroyRegion(region);
						region = IntPtr.Zero;
					}
				}
			}

	/// <summary>
	/// <para>Dispose an instance of <see cref="T:Xsharp.Region"/>.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This method implements the <see cref="T:System.IDisposeable"/>
	/// interface.</para>
	/// </remarks>
	public void Dispose()
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						Xlib.XDestroyRegion(region);
						region = IntPtr.Zero;
					}
				}
			}

	/// <summary>
	/// <para>Clone an instance of <see cref="T:Xsharp.Region"/>.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This method implements the <see cref="T:System.ICloneable"/>
	/// interface.</para>
	/// </remarks>
	public Object Clone()
			{
				Region r = new Region();
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						Xlib.XUnionRegion(region, r.region, r.region);
					}
				}
				return r;
			}

	/// <summary>
	/// <para>Determine if this region is empty.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if this region is empty
	/// or disposed; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool IsEmpty()
			{
				lock(typeof(Region))
				{
					return (region == IntPtr.Zero ||
					        Xlib.XEmptyRegion(region) != 0);
				}
			}

	/// <summary>
	/// <para>Determine if two regions are equal.</para>
	/// </summary>
	///
	/// <param name="obj">
	/// <para>The region object to compare against.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the two regions are equal;
	/// <see langword="false"/> otherwise.  For the purposes of this
	/// method, disposed regions are considered the same as empty
	/// regions.</para>
	/// </returns>
	public override bool Equals(Object obj)
			{
				lock(typeof(Region))
				{
					Region other = (obj as Region);
					if(other != null)
					{
						if(this == other)
						{
							return true;
						}
						else if(IsEmpty())
						{
							return other.IsEmpty();
						}
						else if(other.IsEmpty())
						{
							return false;
						}
						else
						{
							return (Xlib.XEqualRegion
								(region, other.region) != 0);
						}
					}
					else
					{
						return false;
					}
				}
			}

	/// <summary>
	/// <para>Get the hash code for a region.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns the hash code.</para>
	/// </returns>
	public override int GetHashCode()
			{
				lock(typeof(Region))
				{
					// TODO
					return 0;
				}
			}

	/// <summary>
	/// <para>Union a rectangle with this region.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to union with this region.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to <paramref name="rect"/>.</para>
	/// </remarks>
	public void Union(Rectangle rect)
			{
				lock(typeof(Region))
				{
					XRectangle xrect = new XRectangle
						(rect.x, rect.y, rect.width, rect.height);
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					Xlib.XUnionRectWithRegion(ref xrect, region, region);
				}
			}

	/// <summary>
	/// <para>Union an explicitly-specified rectangle with this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to the rectangle.</para>
	/// </remarks>
	public void Union(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					XRectangle xrect = new XRectangle(x, y, width, height);
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					Xlib.XUnionRectWithRegion(ref xrect, region, region);
				}
			}

	/// <summary>
	/// <para>Union another region with this one.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region to union with this one.  If <paramref name="r"/>
	/// is <see langword="null"/>, the same as <see langword="this"/>, or
	/// disposed, then this method will do nothing.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to a copy of <paramref name="r"/>.</para>
	/// </remarks>
	public void Union(Region r)
			{
				lock(typeof(Region))
				{
					if(r != null && r != this && r.region != IntPtr.Zero)
					{
						if(region == IntPtr.Zero)
						{
							region = Xlib.XCreateRegion();
							if(region == IntPtr.Zero)
							{
								Display.OutOfMemory();
							}
						}
						Xlib.XUnionRegion(region, r.region, region);
					}
				}
			}

	/// <summary>
	/// <para>Intersect a rectangle with this region.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to intersect with this region.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to empty.</para>
	/// </remarks>
	public void Intersect(Rectangle rect)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						IntPtr reg = Xlib.XCreateRegion();
						if(reg == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
						XRectangle xrect = new XRectangle
							(rect.x, rect.y, rect.width, rect.height);
						Xlib.XUnionRectWithRegion(ref xrect, reg, reg);
						Xlib.XIntersectRegion(reg, region, region);
						Xlib.XDestroyRegion(reg);
					}
				}
			}

	/// <summary>
	/// <para>Intersect an explicitly-specified rectangle with
	/// this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to empty.</para>
	/// </remarks>
	public void Intersect(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						IntPtr reg = Xlib.XCreateRegion();
						if(reg == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
						XRectangle xrect = new XRectangle(x, y, width, height);
						Xlib.XUnionRectWithRegion(ref xrect, reg, reg);
						Xlib.XIntersectRegion(reg, region, region);
						Xlib.XDestroyRegion(reg);
					}
				}
			}

	/// <summary>
	/// <para>Intersect another region with this one.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region to intersect with this one.  If
	/// <paramref name="r"/> is <see langword="null"/> or disposed,
	/// the method operates as an intersection with the empty region.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to empty.</para>
	/// </remarks>
	public void Intersect(Region r)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else if(r == null || r.region == IntPtr.Zero)
					{
						Xlib.XDestroyRegion(region);
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else if(r != this)
					{
						Xlib.XIntersectRegion(r.region, region, region);
					}
				}
			}

	/// <summary>
	/// <para>Subtract a rectangle from this region.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to subtract from this region.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to empty.</para>
	/// </remarks>
	public void Subtract(Rectangle rect)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						XRectangle xrect = new XRectangle
							(rect.x, rect.y, rect.width, rect.height);
						IntPtr reg = Xlib.XCreateRegion();
						if(reg == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
						Xlib.XUnionRectWithRegion(ref xrect, reg, reg);
						Xlib.XSubtractRegion(region, reg, region);
						Xlib.XDestroyRegion(reg);
					}
				}
			}

	/// <summary>
	/// <para>Subtract an explicitly-specified rectangle from
	/// this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to empty.</para>
	/// </remarks>
	public void Subtract(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						XRectangle xrect = new XRectangle(x, y, width, height);
						IntPtr reg = Xlib.XCreateRegion();
						if(reg == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
						Xlib.XUnionRectWithRegion(ref xrect, reg, reg);
						Xlib.XSubtractRegion(region, reg, region);
						Xlib.XDestroyRegion(reg);
					}
				}
			}

	/// <summary>
	/// <para>Subtract another region from this one.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region to subtract from this one.  If
	/// <paramref name="r"/> is <see langword="null"/> or disposed,
	/// the method does nothing.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be re-created
	/// with its initial contents set to empty.</para>
	/// </remarks>
	public void Subtract(Region r)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else if(r == null || r.region == IntPtr.Zero)
					{
						// Nothing to do here: subtracting an empty region.
					}
					else if(r == this)
					{
						// Subtract the region from itself: result is empty.
						Xlib.XDestroyRegion(region);
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						Xlib.XSubtractRegion(region, r.region, region);
					}
				}
			}

	/// <summary>
	/// <para>Xor a rectangle with this region.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to xor with this region.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be treated
	/// as empty prior to the xor operation.</para>
	/// </remarks>
	public void Xor(Rectangle rect)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					XRectangle xrect = new XRectangle
						(rect.x, rect.y, rect.width, rect.height);
					IntPtr reg = Xlib.XCreateRegion();
					if(reg == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
					Xlib.XUnionRectWithRegion(ref xrect, reg, reg);
					Xlib.XXorRegion(region, reg, region);
					Xlib.XDestroyRegion(reg);
				}
			}

	/// <summary>
	/// <para>Xor an explicitly-specified rectangle with
	/// this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if the rectangle co-ordinates are out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be treated
	/// as empty prior to the xor operation.</para>
	/// </remarks>
	public void Xor(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					XRectangle xrect = new XRectangle(x, y, width, height);
					IntPtr reg = Xlib.XCreateRegion();
					if(reg == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
					Xlib.XUnionRectWithRegion(ref xrect, reg, reg);
					Xlib.XXorRegion(region, reg, region);
					Xlib.XDestroyRegion(reg);
				}
			}

	/// <summary>
	/// <para>Xor another region with this one.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region to xor with this one.  If
	/// <paramref name="r"/> is <see langword="null"/> or disposed,
	/// then it will be treated as the empty region.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>If this region has been disposed, then it will be treated
	/// as empty prior to the xor operation.</para>
	/// </remarks>
	public void Xor(Region r)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					if(r == null || r.region == IntPtr.Zero)
					{
						// Xor of an empty and a non-empty region gives
						// the non-empty region as the result.
					}
					else if(r == this)
					{
						// Xor the region with itself: result is empty.
						Xlib.XDestroyRegion(region);
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						Xlib.XXorRegion(region, r.region, region);
					}
				}
			}

	/// <summary>
	/// <para>Set this region to empty.</para>
	/// </summary>
	public void SetEmpty()
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						Xlib.XDestroyRegion(region);
					}
					region = Xlib.XCreateRegion();
					if(region == IntPtr.Zero)
					{
						Display.OutOfMemory();
					}
				}
			}

	/// <summary>
	/// <para>Offset this region by a specified delta.</para>
	/// </summary>
	///
	/// <param name="dx">
	/// <para>The X delta adjustment to apply to the region.</para>
	/// </param>
	///
	/// <param name="dy">
	/// <para>The Y delta adjustment to apply to the region.</para>
	/// </param>
	public void Offset(int dx, int dy)
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						Xlib.XOffsetRegion(region, dx, dy);
					}
				}
			}

	/// <summary>
	/// <para>Shrink this region by a specified delta.</para>
	/// </summary>
	///
	/// <param name="dx">
	/// <para>The X delta adjustment to apply to the region.</para>
	/// </param>
	///
	/// <param name="dy">
	/// <para>The Y delta adjustment to apply to the region.</para>
	/// </param>
	public void Shrink(int dx, int dy)
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						Xlib.XShrinkRegion(region, dx, dy);
					}
				}
			}

	/// <summary>
	/// <para>Determine if a point is contained within this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the point.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the point is contained
	/// within this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Contains(int x, int y)
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						return (Xlib.XPointInRegion(region, x, y) != 0);
					}
					else
					{
						return false;
					}
				}
			}

	/// <summary>
	/// <para>Determine if a point is contained within this region.</para>
	/// </summary>
	///
	/// <param name="point">
	/// <para>The point to test.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the point is contained
	/// within this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Contains(Point point)
			{
				lock(typeof(Region))
				{
					if(region != IntPtr.Zero)
					{
						return (Xlib.XPointInRegion
									(region, point.x, point.y) != 0);
					}
					else
					{
						return false;
					}
				}
			}

	/// <summary>
	/// <para>Determine if a rectangle is completely contained
	/// in this region.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to test against this region.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the rectangle is completely
	/// contained within this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Contains(Rectangle rect)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return false;
					}
					else
					{
						return (Xlib.XRectInRegion(region, rect.x, rect.y,
												   (uint)(rect.width),
												   (uint)(rect.height))
										== 1);	// RectangleIn
					}
				}
			}

	/// <summary>
	/// <para>Determine if an explicitly-specified rectangle is completely
	/// contained in this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the rectangle is completely
	/// contained within this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Contains(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return false;
					}
					else
					{
						return (Xlib.XRectInRegion(region, x, y,
												   (uint)(width),
												   (uint)(height))
										== 1);	// RectangleIn
					}
				}
			}

	/// <summary>
	/// <para>Determine if another region is completely contained
	/// in this region.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region to test against this region.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if <paramref name="r"/> is
	/// completely contained within this region; <see langword="false"/>
	/// otherwise.</para>
	/// </returns>
	public bool Contains(Region r)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return false;
					}
					else if(r == null || r.region == IntPtr.Zero)
					{
						return false;
					}
					else if(r == this)
					{
						return true;
					}
					else
					{
						IntPtr reg = Xlib.XCreateRegion();
						if(reg == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
						Xlib.XIntersectRegion(region, r.region, reg);
						bool result = (Xlib.XEqualRegion(reg, r.region) != 0);
						Xlib.XDestroyRegion(reg);
						return result;
					}
				}
			}

	/// <summary>
	/// <para>Determine if a rectangle is overlaps with this region.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to test against this region.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the rectangle overlaps
	/// with this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Overlaps(Rectangle rect)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return false;
					}
					else
					{
						return (Xlib.XRectInRegion(region, rect.x, rect.y,
												   (uint)(rect.width),
												   (uint)(rect.height))
										!= 0);	// RectangleOut
					}
				}
			}

	/// <summary>
	/// <para>Determine if an explicitly-specified rectangle overlaps
	/// with this region.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the rectangle overlaps
	/// with this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Overlaps(int x, int y, int width, int height)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return false;
					}
					else
					{
						return (Xlib.XRectInRegion(region, x, y,
												   (uint)(width),
												   (uint)(height))
										!= 0);	// RectangleOut
					}
				}
			}

	/// <summary>
	/// <para>Determine if another region overlaps with this region.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The other region to test against this region.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if <paramref name="r"/> overlaps
	/// with this region; <see langword="false"/> otherwise.</para>
	/// </returns>
	public bool Overlaps(Region r)
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return false;
					}
					else if(r == null || r.region == IntPtr.Zero)
					{
						return false;
					}
					else if(r == this)
					{
						return true;
					}
					else
					{
						IntPtr reg = Xlib.XCreateRegion();
						if(reg == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
						Xlib.XIntersectRegion(region, r.region, reg);
						bool result = (Xlib.XEmptyRegion(reg) == 0);
						Xlib.XDestroyRegion(reg);
						return result;
					}
				}
			}

	/// <summary>
	/// <para>Get the smallest rectangle that completely contains
	/// this region.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>A <see cref="T:Xsharp.Rectangle"/> instance corresponding
	/// to the smallest rectangle that contains the region.</para>
	/// </returns>
	public Rectangle ClipBox()
			{
				lock(typeof(Region))
				{
					Rectangle rect;
					if(region == IntPtr.Zero)
					{
						rect = new Rectangle(0, 0, 0, 0);
					}
					else
					{
						XRectangle xrect;
						Xlib.XClipBox(region, out xrect);
						rect = new Rectangle(xrect.x, xrect.y,
											 xrect.width, xrect.height);
					}
					return rect;
				}
			}

	/// <summary>
	/// <para>Get the list of rectangles that defines this region.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>An array of <see cref="T:Xsharp.Rectangle"/> instances
	/// corresponding to the rectangles that make up the region.
	/// Returns a zero-length array if the region is empty.</para>
	/// </returns>
	public Rectangle[] GetRectangles()
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						return new Rectangle [0];
					}
					else
					{
						Rectangle[] rects;
						XRectangle xrect;
						int size, index;
						size = Xlib.XSharpGetRegionSize(region);
						rects = new Rectangle [size];
						for(index = 0; index < size; ++index)
						{
							Xlib.XSharpGetRegionRect(region, index, out xrect);
							rects[index].x = xrect.x;
							rects[index].y = xrect.y;
							rects[index].width = xrect.width;
							rects[index].height = xrect.height;
						}
						return rects;
					}
				}
			}

	// Get the Xlib region structure, and make sure it is non-NULL.
	internal IntPtr GetRegion()
			{
				lock(typeof(Region))
				{
					if(region == IntPtr.Zero)
					{
						region = Xlib.XCreateRegion();
						if(region == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					return region;
				}
			}

} // class Region

} // namespace Xsharp
