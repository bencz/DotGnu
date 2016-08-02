/*
 * RectangleF.cs - Implementation of the "System.Drawing.RectangleF" class.
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

#if !ECMA_COMPAT
[Serializable]
#endif
public struct RectangleF
{
	// Internal state.
	private float x;
	private float y;
	private float width;
	private float height;

	// The empty rectangle.
	public static readonly RectangleF Empty
				= new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

	// Constructors.
	public RectangleF(PointF location, SizeF size)
			{
				x = location.X;
				y = location.Y;
				width = size.Width;
				height = size.Height;
			}
	public RectangleF(float x, float y, float width, float height)
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
					return (x == 0.0f && y == 0.0f &&
							width == 0.0f && height == 0.0f);
				}
			}

	// Get or set the X co-ordinate.
	public float X
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
	public float Y
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
	public float Width
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
	public float Height
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
	public float Bottom
			{
				get
				{
					return y + height;
				}
			}

	// Get the left edge of the rectangle.
	public float Left
			{
				get
				{
					return x;
				}
			}

	// Get the right edge of the rectangle.
	public float Right
			{
				get
				{
					return x + width;
				}
			}

	// Get the top edge of the rectangle.
	public float Top
			{
				get
				{
					return y;
				}
			}

	// Get or set the location of the top-left corner.
	public PointF Location
			{
				get
				{
					return new PointF(x, y);
				}
				set
				{
					x = value.X;
					y = value.Y;
				}
			}

	// Get or set the size of the rectangle.
	public SizeF Size
			{
				get
				{
					return new SizeF(width, height);
				}
				set
				{
					width = value.Width;
					height = value.Height;
				}
			}

	// Determine if a rectangle contains a point.
	public bool Contains(float x, float y)
			{
				return (x >= this.x && x < (this.x + this.width) &&
				        y >= this.y && y < (this.y + this.height));
			}
	public bool Contains(PointF pt)
			{
				return Contains(pt.X, pt.Y);
			}

	// Determine if one rectangle contains another.
	public bool Contains(RectangleF rect)
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
				if(obj is RectangleF)
				{
					RectangleF other = (RectangleF)obj;
					return (x == other.x && y == other.y &&
					        width == other.width && height == other.height);
				}
				else
				{
					return false;
				}
			}

	// Convert left, top, right, bottom values into a rectangle.
	public static RectangleF FromLTRB
				(float left, float top, float right, float bottom)
			{
				return new RectangleF(left, top, right - left, bottom - top);
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

	// Inflate this rectangle.
	public void Inflate(float width, float height)
			{
				this.x -= width;
				this.y -= height;
				this.width += width * 2;
				this.height += height * 2;
			}
	public void Inflate(SizeF size)
			{
				Inflate(size.Width, size.Height);
			}

	// Inflate a specific rectangle without modifying it.
	public static RectangleF Inflate(RectangleF rect, float x, float y)
			{
				return new RectangleF(rect.x - x, rect.y - y,
									  rect.width + x * 2, rect.height + y * 2);
			}

	// Form the intersection of another rectangle with this one.
	public void Intersect(RectangleF rect)
			{
				float left, top, right, bottom;
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
					x = 0.0f;
					y = 0.0f;
					width = 0.0f;
					height = 0.0f;
				}
			}

	// Form the intersection of two rectangles.
	public static RectangleF Intersect(RectangleF a, RectangleF b)
			{
				a.Intersect(b);
				return a;
			}

	// Determine if this rectangle intersects with another.
	public bool IntersectsWith(RectangleF rect)
			{
				return (rect.x < (x + width) &&
				        (rect.x + rect.width) >= x &&
				        rect.y < (y + height) &&
				        (rect.y + rect.height) >= y);
			}

	// Offset this rectangle by a point.
	public void Offset(float x, float y)
			{
				this.x += x;
				this.y += y;
			}
	public void Offset(PointF pos)
			{
				this.x += pos.X;
				this.y += pos.Y;
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert this object into a string.
	public override String ToString()
			{
				return "{X=" + x.ToString() +
				       ",Y=" + y.ToString() +
					   ",Width=" + width.ToString() +
					   ",Height=" + height.ToString() + "}";
			}

#endif

	// Get the union of two rectangles.
	public static RectangleF Union(RectangleF a, RectangleF b)
			{
				float left, top, right, bottom;
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
				return new RectangleF(left, top, right - left, bottom - top);
			}

	// Overloaded operators.
	public static bool operator==(RectangleF left, RectangleF right)
			{
				return (left.x == right.x &&
						left.y == right.y &&
						left.width == right.width &&
						left.height == right.height);
			}
	public static bool operator!=(RectangleF left, RectangleF right)
			{
				return (left.x != right.x ||
						left.y != right.y ||
						left.width != right.width ||
						left.height != right.height);
			}
	public static implicit operator RectangleF(Rectangle r)
			{
				return new RectangleF(r.X, r.Y, r.Width, r.Height);
			}

}; // struct RectangleF
		
}; // namespace System.Drawing
