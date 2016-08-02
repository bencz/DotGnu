/*
 * PointF.cs - Implementation of the "System.Drawing.PointF" class.
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

#if !ECMA_COMPAT
[Serializable]
[ComVisible(true)]
#endif
public struct PointF
{
	// Internal state.
	private float x;
	private float y;

	// The empty point.
	public static readonly PointF Empty = new PointF(0.0f, 0.0f);

	// Constructor.
	public PointF(float x, float y)
			{
				this.x = x;
				this.y = y;
			}

	// Determine if this point is empty.
	public bool IsEmpty
			{
				get
				{
					return (x == 0.0f && y == 0.0f);
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

	// Determine if two points are equal.
	public override bool Equals(Object obj)
			{
				if(obj is PointF)
				{
					PointF other = (PointF)obj;
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
				return base.GetHashCode();
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert this object into a string.
	public override String ToString()
			{
				return "{X=" + x.ToString() + ",Y=" + y.ToString() + "}";
			}

#endif

	// Overloaded operators.
	public static PointF operator+(PointF pt, Size sz)
			{
				return new PointF(pt.x + sz.Width, pt.y + sz.Height);
			}
	public static PointF operator-(PointF pt, Size sz)
			{
				return new PointF(pt.x - sz.Width, pt.y - sz.Height);
			}
	public static bool operator==(PointF left, PointF right)
			{
				return (left.x == right.x && left.y == right.y);
			}
	public static bool operator!=(PointF left, PointF right)
			{
				return (left.x != right.x || left.y != right.y);
			}

}; // struct PointF
		
}; // namespace System.Drawing
