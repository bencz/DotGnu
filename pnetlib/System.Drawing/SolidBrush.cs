/*
 * SolidBrush.cs - Implementation of the "System.Drawing.SolidBrush" class.
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

using System.Drawing.Toolkit;

public sealed class SolidBrush : Brush
{
	// Internal state.
	private Color color;

	// Constructor.
	public SolidBrush(Color color)
			{
				this.color = color;
			}

	// Get or set the color of this brush.
	public Color Color
			{
				get
				{
					return color;
				}
				set
				{
					if(color != value)
					{
						color = value;
						Modified();
					}
				}
			}

	// Clone this brush.
	public override Object Clone()
			{
				return new SolidBrush(color);
			}

	// Create this brush for a specific toolkit.  Inner part of "GetBrush()".
	internal override IToolkitBrush CreateBrush(IToolkit toolkit)
			{
				return toolkit.CreateSolidBrush(color);
			}

}; // class SolidBrush

}; // namespace System.Drawing
