/*
 * ImageFlags.cs - Implementation of the
 *			"System.Drawing.Imaging.ImageFlags" class.
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

[Flags]
public enum ImageFlags
{
	Scalable			= 0x00000001,
	None				= 0x00000000,
	HasAlpha			= 0x00000002,
	HasTranslucent		= 0x00000004,
	PartiallyScalable	= 0x00000008,
	ColorSpaceRgb		= 0x00000010,
	ColorSpaceCmyk		= 0x00000020,
	ColorSpaceGray		= 0x00000040,
	ColorSpaceYcbcr		= 0x00000080,
	ColorSpaceYcck		= 0x00000100,
	HasRealDpi			= 0x00001000,
	HasRealPixelSize	= 0x00002000,
	ReadOnly			= 0x00010000,
	Caching				= 0x00020000

}; // enum ImageFlags

}; // namespace System.Drawing.Imaging
