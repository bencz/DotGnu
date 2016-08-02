/*
 * PixelFormat.cs - Implementation of the
 *			"System.Drawing.Imaging.PixelFormat" class.
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

public enum PixelFormat
{
	Undefined				= 0x00000000,
	DontCare				= 0x00000000,
	Max						= 0x0000000F,
	Indexed					= 0x00010000,
	Gdi						= 0x00020000,
	Format16bppRgb555		= 0x00021005,
	Format16bppRgb565		= 0x00021006,
	Format24bppRgb			= 0x00021808,
	Format32bppRgb			= 0x00022009,
	Format1bppIndexed		= 0x00030101,
	Format4bppIndexed		= 0x00030402,
	Format8bppIndexed		= 0x00030803,
	Alpha					= 0x00040000,
	Format16bppArgb1555		= 0x00061007,
	PAlpha					= 0x00080000,
	Format32bppPArgb		= 0x000E200B,
	Extended				= 0x00100000,
	Format16bppGrayScale	= 0x00101004,
	Format48bppRgb			= 0x0010300C,
	Format64bppPArgb		= 0x001C400E,
	Canonical				= 0x00200000,
	Format32bppArgb			= 0x0026200A,
	Format64bppArgb			= 0x0034400D

}; // enum PixelFormat

}; // namespace System.Drawing.Imaging
