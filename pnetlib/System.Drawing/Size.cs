/*
 * Size.cs - Implementation of the "System.Drawing.Size" class.
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
[TypeConverter("System.Drawing.SizeConverter")]
#endif
public struct Size
{
	// Internal state.
	private int width;
	private int height;

	// The empty size.
	public static readonly Size Empty = new Size(0, 0);

	// Constructors.
	public Size(Point pt)
			{
				width = pt.X;
				height = pt.Y;
			}
	public Size(int width, int height)
			{
				this.width = width;
				this.height = height;
			}

	// Determine if this size is empty.
	public bool IsEmpty
			{
				get
				{
					return (width == 0 && height == 0);
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

#if CONFIG_EXTENDED_NUMERICS

	// Convert a SizeF object into a Size object using ceiling conversion.
	public static Size Ceiling(SizeF value)
			{
				return new Size((int)(Math.Ceiling(value.Width)),
								(int)(Math.Ceiling(value.Height)));
			}

#endif

	// Determine if two sizes are equal.
	public override bool Equals(Object obj)
			{
				if(obj is Size)
				{
					Size other = (Size)obj;
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
				return (width ^ height);
			}

#if CONFIG_EXTENDED_NUMERICS

	// Convert a SizeF object into a Size object using rounding conversion.
	public static Size Round(SizeF value)
			{
				return new Size((int)(Math.Round(value.Width)),
								(int)(Math.Round(value.Height)));
			}

#endif

	// Convert this object into a string.
	public override String ToString()
			{
				return "{Width=" + width.ToString() +
					   ", Height=" + height.ToString() + "}";
			}

	// Convert a SizeF object into a Size object using truncating conversion.
	public static Size Truncate(SizeF value)
			{
				return new Size((int)(value.Width), (int)(value.Height));
			}

	// Overloaded operators.
	public static Size operator+(Size sz1, Size sz2)
			{
				return new Size(sz1.width + sz2.width, sz1.height + sz2.height);
			}
	public static Size operator-(Size sz1, Size sz2)
			{
				return new Size(sz1.width - sz2.width, sz1.height - sz2.height);
			}
	public static bool operator==(Size left, Size right)
			{
				return (left.width == right.width &&
						left.height == right.height);
			}
	public static bool operator!=(Size left, Size right)
			{
				return (left.width != right.width ||
						left.height != right.height);
			}
	public static explicit operator Point(Size size)
			{
				return new Point(size.width, size.height);
			}
	public static implicit operator SizeF(Size size)
			{
				return new SizeF(size.width, size.height);
			}

}; // struct Size
		
}; // namespace System.Drawing
