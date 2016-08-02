/*
 * AdjustableArrowCap.cs - Implementation of the
 *			"System.Drawing.Drawing2D.AdjustableArrowCap" class.
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

namespace System.Drawing.Drawing2D
{

public sealed class AdjustableArrowCap : CustomLineCap
{
	// Internal state.
	private float width;
	private float height;
	private float middleInset;
	private bool isFilled;

	// Constructors.
	public AdjustableArrowCap(float width, float height)
			: base(null, null)
			{
				this.width = width;
				this.height = height;
				this.isFilled = true;
			}
	public AdjustableArrowCap(float width, float height, bool isFilled)
			: base(null, null)
			{
				this.width = width;
				this.height = height;
				this.isFilled = true;
			}

	// Get or set this object's properties.
	public bool Filled
			{
				get
				{
					return isFilled;
				}
				set
				{
					isFilled = value;
				}
			}
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
	public float MiddleInset
			{
				get
				{
					return middleInset;
				}
				set
				{
					middleInset = value;
				}
			}
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

}; // class AdjustableArrowCap

}; // namespace System.Drawing.Drawing2D
