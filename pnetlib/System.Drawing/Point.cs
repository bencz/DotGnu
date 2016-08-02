/*
 * Point.cs - Implementation of the "System.Drawing.Point" class.
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

namespace System.Drawing
{

using System.Runtime.InteropServices;
using System.ComponentModel;

#if !ECMA_COMPAT
[Serializable]
[ComVisible(true)]
#endif
#if CONFIG_COMPONENT_MODEL
[TypeConverter("System.Drawing.PointConverter")]
#endif
public struct Point
{
	// Internal state.
	private int x;
	private int y;

	// The empty point.
	public static readonly Point Empty = new Point(0, 0);

	// Constructors.
	public Point(int dw)
			{
				x = (int)(short)dw;
				y = (dw >> 16);
			}
	public Point(Size sz)
			{
				x = sz.Width;
				y = sz.Height;
			}
	public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

	// Determine if this point is empty.
	public bool IsEmpty
			{
				get
				{
					return (x == 0 && y == 0);
				}
			}

	// Get or set the X co-ordinate.
	public int X
			{
				get
				{
					return x;
				}
				set
				{
					x = value;
				}
			}

	// Get or set the Y co-ordinate.
	public int Y
			{
				get
				{
					return y;
				}
				set
				{
					y = value;
				}
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert a PointF object into a Point object using ceiling conversion.
	public static Point Ceiling(PointF value)
			{
				return new Point((int)(Math.Ceiling(value.X)),
								 (int)(Math.Ceiling(value.Y)));
			}

#endif

	// Determine if two points are equal.
	public override bool Equals(Object obj)
			{
				if(obj is Point)
				{
					Point other = (Point)obj;
					return (x == other.x && y == other.y);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return (x ^ y);
			}

	// Offset this point by a specified amount.
	public void Offset(int dx, int dy)
			{
				x += dx;
				y += dy;
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert a PointF object into a Point object using rounding conversion.
	public static Point Round(PointF value)
			{
				return new Point((int)(Math.Round(value.X)),
								 (int)(Math.Round(value.Y)));
			}

#endif

	// Convert this object into a string.
	public override String ToString()
			{
				return "{X=" + x.ToString() + ",Y=" + y.ToString() + "}";
			}

	// Convert a PointF object into a Point object using truncating conversion.
	public static Point Truncate(PointF value)
			{
				return new Point((int)(value.X), (int)(value.Y));
			}

	// Overloaded operators.
	public static Point operator+(Point pt, Size sz)
			{
				return new Point(pt.x + sz.Width, pt.y + sz.Height);
			}
	public static Point operator-(Point pt, Size sz)
			{
				return new Point(pt.x - sz.Width, pt.y - sz.Height);
			}
	public static bool operator==(Point left, Point right)
			{
				return (left.x == right.x && left.y == right.y);
			}
	public static bool operator!=(Point left, Point right)
			{
				return (left.x != right.x || left.y != right.y);
			}
	public static explicit operator Size(Point p)
			{
				return new Size(p.x, p.y);
			}
	public static implicit operator PointF(Point p)
			{
				return new PointF(p.x, p.y);
			}

}; // struct Point
		
}; // namespace System.Drawing
