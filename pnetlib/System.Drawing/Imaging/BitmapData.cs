/*
 * BitmapData.cs - Implementation of the
 *			"System.Drawing.Imaging.BitmapData" class.
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

namespace System.Drawing.Imaging
{

using System;
using System.Runtime.InteropServices;

public sealed class BitmapData
{
	// Internal state.
	private int height;
	private PixelFormat pixelFormat;
	private int reserved;
	private IntPtr scan0;
	private int stride;
	private int width;
	internal GCHandle dataHandle;

	// Constructor.
	public BitmapData() {}

	// Get or set this object's properties.
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
	public PixelFormat PixelFormat
			{
				get
				{
					return pixelFormat;
				}
				set
				{
					pixelFormat = value;
				}
			}
	public int Reserved
			{
				get
				{
					return reserved;
				}
				set
				{
					reserved = value;
				}
			}
	public IntPtr Scan0
			{
				get
				{
					return scan0;
				}
				set
				{
					scan0 = value;
				}
			}
	public int Stride
			{
				get
				{
					return stride;
				}
				set
				{
					stride = value;
				}
			}
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

}; // class BitmapData

}; // namespace System.Drawing.Imaging
