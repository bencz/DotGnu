/*
 * ImageCodecFlags.cs - Implementation of the
 *			"System.Drawing.Imaging.ImageCodecFlags" class.
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
public enum ImageCodecFlags
{
	Encoder			= 0x00000001,
	Decoder			= 0x00000002,
	SupportBitmap	= 0x00000004,
	SupportVector	= 0x00000008,
	SeekableEncode	= 0x00000010,
	BlockingDecoder	= 0x00000020,
	Builtin			= 0x00010000,
	System			= 0x00020000,
	User			= 0x00040000

}; // enum ImageCodecFlags

}; // namespace System.Drawing.Imaging
