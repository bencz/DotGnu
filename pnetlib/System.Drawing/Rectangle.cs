/*
 * Rectangle.cs - Implementation of the "System.Drawing.Rectangle" class.
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
[TypeConverter("System.Drawing.RectangleConverter")]
#endif
public struct Rectangle
{
	// Internal state.
	private int x;
	private int y;
	private int width;
	private int height;

	// The empty rectangle.
	public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);

	// Constructors.
	public Rectangle(Point location, Size size)
			{
				x = location.X;
				y = location.Y;
				width = size.Width;
				height = size.Height;
			}
	public Rectangle(int x, int y, int width, int height)
			{
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;
			}

	// Determine if this rectangle is empty.
	public bool IsEmpty
			{
				get
				{
					return (x == 0 && y == 0 && width == 0 && height == 0);
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

	// Get or set the width.
	public int Width
			{
				get
				{
					return width;
				}
				set
				{
					width = value;
				}
			}

	// Get or set the height.
	public int Height
			{
				get
				{
					return height;
				}
				set
				{
					height = value;
				}
			}

	// Get the bottom edge of the rectangle.
	public int Bottom
			{
				get
				{
					return y + height;
				}
			}

	// Get the left edge of the rectangle.
	public int Left
			{
				get
				{
					return x;
				}
			}

	// Get the right edge of the rectangle.
	public int Right
			{
				get
				{
					return x + width;
				}
			}

	// Get the top edge of the rectangle.
	public int Top
			{
				get
				{
					return y;
				}
			}

	// Get or set the location of the top-left corner.
	public Point Location
			{
				get
				{
					return new Point(x, y);
				}
				set
				{
					x = value.X;
					y = value.Y;
				}
			}

	// Get or set the size of the rectangle.
	public Size Size
			{
				get
				{
					return new Size(width, height);
				}
				set
				{
					width = value.Width;
					height = value.Height;
				}
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert a RectangleF object into a Rectangle by ceiling conversion.
	public static Rectangle Ceiling(RectangleF value)
			{
				return new Rectangle((int)(Math.Ceiling(value.X)),
								     (int)(Math.Ceiling(value.Y)),
								     (int)(Math.Ceiling(value.Width)),
								     (int)(Math.Ceiling(value.Height)));
			}

#endif

	// Determine if a rectangle contains a point.
	public bool Contains(int x, int y)
			{
				return (x >= this.x && x < (this.x + this.width) &&
				        y >= this.y && y < (this.y + this.height));
			}
	public bool Contains(Point pt)
			{
				return Contains(pt.X, pt.Y);
			}

	// Determine if one rectangle contains another.
	public bool Contains(Rectangle rect)
			{
				if(rect.x >= this.x &&
				   (rect.x + rect.width) <= (this.x + this.width) &&
				   rect.y >= this.y &&
				   (rect.y + rect.height) <= (this.y + this.height))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Determine if two rectangles are equal.
	public override bool Equals(Object obj)
			{
				if(obj is Rectangle)
				{
					Rectangle other = (Rectangle)obj;
					return (x == other.x && y == other.y &&
					        width == other.width && height == other.height);
				}
				else
				{
					return false;
				}
			}

	// Convert left, top, right, bottom values into a rectangle.
	public static Rectangle FromLTRB(int left, int top, int right, int bottom)
			{
				return new Rectangle(left, top, right - left, bottom - top);
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return (x ^ y ^ width ^ height);
			}

	// Inflate this rectangle.
	public void Inflate(int width, int height)
			{
				this.x -= width;
				this.y -= height;
				this.width += width * 2;
				this.height += height * 2;
			}
	public void Inflate(Size size)
			{
				Inflate(size.Width, size.Height);
			}

	// Inflate a specific rectangle without modifying it.
	public static Rectangle Inflate(Rectangle rect, int x, int y)
			{
				return new Rectangle(rect.x - x, rect.y - y,
									 rect.width + x * 2, rect.height + y * 2);
			}

	// Form the intersection of another rectangle with this one.
	public void Intersect(Rectangle rect)
			{
				int left, top, right, bottom;
				left = x;
				if(left < rect.x)
				{
					left = rect.x;
				}
				top = y;
				if(top < rect.y)
				{
					top = rect.y;
				}
				right = x + width;
				if(right > (rect.x + rect.width))
				{
					right = (rect.x + rect.width);
				}
				bottom = y + height;
				if(bottom > (rect.y + rect.height))
				{
					bottom = (rect.y + rect.height);
				}
				if(left < right && top < bottom)
				{
					x = left;
					y = top;
					width = right - left;
					height = bottom - top;
				}
				else
				{
					x = 0;
					y = 0;
					width = 0;
					height = 0;
				}
			}

	// Form the intersection of two rectangles.
	public static Rectangle Intersect(Rectangle a, Rectangle b)
			{
				a.Intersect(b);
				return a;
			}

	// Determine if this rectangle intersects with another.
	public bool IntersectsWith(Rectangle rect)
			{
				return (rect.x < (x + width) &&
				        (rect.x + rect.width) >= x &&
				        rect.y < (y + height) &&
				        (rect.y + rect.height) >= y);
			}

	// Offset this rectangle by a point.
	public void Offset(int x, int y)
			{
				this.x += x;
				this.y += y;
			}
	public void Offset(Point pos)
			{
				this.x += pos.X;
				this.y += pos.Y;
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert a RectangleF object into a Rectangle by rounding conversion.
	public static Rectangle Round(RectangleF value)
			{
				return new Rectangle((int)(Math.Round(value.X)),
								     (int)(Math.Round(value.Y)),
								     (int)(Math.Round(value.Width)),
								     (int)(Math.Round(value.Height)));
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return "{X=" + x.ToString() +
				       ",Y=" + y.ToString() +
					   ",Width=" + width.ToString() +
					   ",Height=" + height.ToString() + "}";
			}

#endif

	// Convert a RectangleF object into a Rectangle by truncating conversion.
	public static Rectangle Truncate(RectangleF value)
			{
				return new Rectangle((int)(value.X), (int)(value.Y),
								     (int)(value.Width), (int)(value.Height));
			}

	// Get the union of two rectangles.
	public static Rectangle Union(Rectangle a, Rectangle b)
			{
				int left, top, right, bottom;
				left = a.x;
				if(left > b.x)
				{
					left = b.x;
				}
				top = a.y;
				if(top > b.y)
				{
					top = b.y;
				}
				right = a.x + a.width;
				if(right < (b.x + b.width))
				{
					right = b.x + b.width;
				}
				bottom = a.y + a.height;
				if(bottom < (b.y + b.height))
				{
					bottom = b.y + b.height;
				}
				return new Rectangle(left, top, right - left, bottom - top);
			}

	// Overloaded operators.
	public static bool operator==(Rectangle left, Rectangle right)
			{
				return (left.x == right.x &&
						left.y == right.y &&
						left.width == right.width &&
						left.height == right.height);
			}
	public static bool operator!=(Rectangle left, Rectangle right)
			{
				return (left.x != right.x ||
						left.y != right.y ||
						left.width != right.width ||
						left.height != right.height);
			}

}; // struct Rectangle
		
}; // namespace System.Drawing
