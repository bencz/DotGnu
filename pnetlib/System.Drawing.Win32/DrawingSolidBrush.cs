/*
 * DrawingSolidBrush.cs - Implementation of solid brushes for System.Drawing.Win32.
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

internal class DrawingSolidBrush : DrawingBrush, IToolkitBrush
{
	public DrawingSolidBrush(IToolkit toolkit, System.Drawing.Color color) : base(toolkit, color)
	{
		hBrush = CreateSolidBrush(color);
	}

	public static IntPtr CreateSolidBrush(Color color)
	{
		Win32.Api.LOGBRUSH lb; 
		lb.lbStyle = (Win32.Api.LogBrushStyles)Win32.Api.LogBrushStyles.BS_SOLID;
		lb.lbColor = DrawingGraphics.ColorToWin32(color); 
		lb.lbHatch = 0;
		return Win32.Api.CreateBrushIndirect(ref lb);
	}

}; // class DrawingSolidBrush

}; // namespace System.Drawing.Toolkit;
