/*
 * Margins.cs - Implementation of the
 *			"System.Drawing.Printing.Margins" class.
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

namespace System.Drawing.Printing
{

using System.ComponentModel;
using System.Text;

#if CONFIG_COMPONENT_MODEL
[TypeConverter(typeof(MarginsConverter))]
#endif
public class Margins : ICloneable
{
	// Internal state.
	private int left;
	private int right;
	private int top;
	private int bottom;

	// Constructors.
	public Margins()
			{
				left = 100;
				right = 100;
				top = 100;
				bottom = 100;
			}
	public Margins(int left, int right, int top, int bottom)
			{
				if(left < 0)
				{
					throw new ArgumentException
						(S._("ArgRange_NonNegative"), "left");
				}
				if(right < 0)
				{
					throw new ArgumentException
						(S._("ArgRange_NonNegative"), "right");
				}
				if(top < 0)
				{
					throw new ArgumentException
						(S._("ArgRange_NonNegative"), "top");
				}
				if(bottom < 0)
				{
					throw new ArgumentException
						(S._("ArgRange_NonNegative"), "bottom");
				}
				this.left = left;
				this.right = right;
				this.top = top;
				this.bottom = bottom;
			}

	// Get or set this object's properties.
	public int Bottom
			{
				get
				{
					return bottom;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"), "value");
					}
					bottom = value;
				}
			}
	public int Left
			{
				get
				{
					return left;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"), "value");
					}
					left = value;
				}
			}
	public int Right
			{
				get
				{
					return right;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"), "value");
					}
					right = value;
				}
			}
	public int Top
			{
				get
				{
					return top;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"), "value");
					}
					top = value;
				}
			}

	// Clone this object.
	public Object Clone()
			{
				return MemberwiseClone();
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				Margins other = (obj as Margins);
				if(other != null)
				{
					return (other.left == left && other.right == right &&
							other.top == top && other.bottom == bottom);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return (left + right + top + bottom);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[Margins Left=");
				builder.Append(left.ToString());
				builder.Append(" Right=");
				builder.Append(right.ToString());
				builder.Append(" Top=");
				builder.Append(top.ToString());
				builder.Append(" Bottom=");
				builder.Append(bottom.ToString());
				builder.Append(']');
				return builder.ToString();
			}

}; // class Margins

}; // namespace System.Drawing.Printing
