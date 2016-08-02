/*
 * DrawingBrush.cs - Implementation of abstract brush for System.Drawing.Win32.
 * Copyright (C) 2003  Neil Cawse.
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
	
	abstract class DrawingBrush : ToolkitBrushBase
	{
		// Internal state.
		protected internal IntPtr hBrush;
		protected IToolkit toolkit;

		public DrawingBrush(IToolkit toolkit, Color color) : base (color)
		{
			this.toolkit = toolkit;
		}

		// Select this brush into a graphics object.
		public override void Select(IToolkitGraphics graphics)
		{
			(graphics as ToolkitGraphicsBase).Brush = this;
		}

		// Dispose of this object.
		protected override void Dispose(bool disposing)
		{
			Win32.Api.DeleteObject(hBrush);
			hBrush = IntPtr.Zero;
		}
	}
}; // namespace System.Drawing.Toolkit;
