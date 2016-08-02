/*
 * DrawingXorBrush.cs - Implementation of solid brushes for System.Drawing.Win32.
 * Copyright (C) 2003  Neil Cawse.
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

namespace System.Drawing.Toolkit
{

using System;

internal class DrawingXorBrush : DrawingBrush, IToolkitBrush
{
	IToolkitBrush innerBrush;

	public DrawingXorBrush(IToolkit toolkit, IToolkitBrush innerBrush)
		: base(toolkit, System.Drawing.Color.Black)
	{
		// TODO: create the XOR brush.
		this.innerBrush = innerBrush;
		hBrush = IntPtr.Zero;
	}

	// Select this brush into a graphics object.
	public override void Select(IToolkitGraphics graphics)
	{
		// TODO: need to handle the XOR property.
		innerBrush.Select(graphics);
	}

}; // class DrawingXorBrush

}; // namespace System.Drawing.Toolkit;
