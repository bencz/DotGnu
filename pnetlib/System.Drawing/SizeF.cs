/*
 * SizeF.cs - Implementation of the "System.Drawing.SizeF" class.
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
public struct SizeF
{
	// Internal state.
	private float width;
	private float height;

	// The empty size.
	public static readonly SizeF Empty = new SizeF(0.0f, 0.0f);

	// Constructors.
	public SizeF(PointF pt)
			{
				width = pt.X;
				height = pt.Y;
			}
	public SizeF(SizeF size)
			{
				width = size.width;
				height = size.height;
			}
	public SizeF(float width, float height)
			{
				this.width = width;
				this.height = height;
			}

	// Determine if this size is empty.
	public bool IsEmpty
			{
				get
				{
					return (width == 0.0f && height == 0.0f);
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

	// Determine if two sizes are equal.
	public override bool Equals(Object obj)
			{
				if(obj is SizeF)
				{
					SizeF other = (SizeF)obj;
					return (width == other.width && height == other.height);
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

	// Convert this size into a point.
	public PointF ToPointF()
			{
				return new PointF(width, height);
			}

	// Convert this size into its integer form.
	public Size ToSize()
			{
				return new Size((int)width, (int)height);
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert this object into a string.
	public override String ToString()
			{
				return "{Width=" + width.ToString() +
					   ", Height=" + height.ToString() + "}";
			}

#endif

	// Overloaded operators.
	public static SizeF operator+(SizeF sz1, SizeF sz2)
			{
				return new SizeF(sz1.width + sz2.width,
								 sz1.height + sz2.height);
			}
	public static SizeF operator-(SizeF sz1, SizeF sz2)
			{
				return new SizeF(sz1.width - sz2.width,
								 sz1.height - sz2.height);
			}
	public static bool operator==(SizeF left, SizeF right)
			{
				return (left.width == right.width &&
						left.height == right.height);
			}
	public static bool operator!=(SizeF left, SizeF right)
			{
				return (left.width != right.width ||
						left.height != right.height);
			}
	public static explicit operator PointF(SizeF size)
			{
				return new PointF(size.width, size.height);
			}

}; // struct SizeF
		
}; // namespace System.Drawing
