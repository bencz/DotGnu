/*
 * XorBrush.cs - Implementation of the "System.Drawing.XorBrush" class.
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

internal sealed class XorBrush : Brush
{
	// Internal state.
	private Brush brush;

	// Constructors.
	public XorBrush(Brush brush)
			{
				if(brush == null)
				{
					throw new ArgumentNullException("brush");
				}
				this.brush = brush;
			}

	// Get the brush underlying this XOR brush.
	public Brush Brush
			{
				get
				{
					return brush;
				}
			}

	// Clone this brush.
	public override Object Clone()
			{
				lock(this)
				{
					XorBrush brush = (XorBrush)(MemberwiseClone());
					brush.toolkit = null;
					brush.toolkitBrush = null;
					return brush;
				}
			}

	// Create this brush for a specific toolkit.  Inner part of "GetBrush()".
	internal override IToolkitBrush CreateBrush(IToolkit toolkit)
			{
				IToolkitBrush innerBrush = brush.GetBrush(toolkit);
				return toolkit.CreateXorBrush(innerBrush);
			}

}; // class XorBrush

}; // namespace System.Drawing
