/*
 * EncoderValue.cs - Implementation of the
 *			"System.Drawing.Imaging.EncoderValue" class.
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

public enum EncoderValue
{
	ColorTypeCMYK				= 0,
	ColorTypeYCCK				= 1,
	CompressionLZW				= 2,
	CompressionCCITT3			= 3,
	CompressionCCITT4			= 4,
	CompressionRle				= 5,
	CompressionNone				= 6,
	ScanMethodInterlaced		= 7,
	ScanMethodNonInterlaced		= 8,
	VersionGif87				= 9,
	VersionGif89				= 10,
	RenderProgressive			= 11,
	RenderNonProgressive		= 12,
	TransformRotate90			= 13,
	TransformRotate180			= 14,
	TransformRotate270			= 15,
	TransformFlipHorizontal		= 16,
	TransformFlipVertical		= 17,
	MultiFrame					= 18,
	LastFrame					= 19,
	Flush						= 20,
	FrameDimensionTime			= 21,
	FrameDimensionResolution	= 22,
	FrameDimensionPage			= 23

}; // enum EncoderValue

}; // namespace System.Drawing.Imaging
