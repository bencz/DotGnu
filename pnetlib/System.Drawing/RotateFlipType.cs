/*
 * RotateFlipType.cs - Implementation of the
 *			"System.Drawing.RotateFlipType" class.
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

public enum RotateFlipType
{
	RotateNoneFlipNone	= 0,
	Rotate90FlipNone	= 1,
	Rotate180FlipNone	= 2,
	Rotate270FlipNone	= 3,
	RotateNoneFlipX		= 4,
	Rotate90FlipX		= 5,
	Rotate180FlipX		= 6,
	Rotate270FlipX		= 7,

	Rotate180FlipXY		= RotateNoneFlipNone,
	Rotate270FlipXY		= Rotate90FlipNone,
	RotateNoneFlipXY	= Rotate180FlipNone,
	Rotate90FlipXY		= Rotate270FlipNone,
	Rotate180FlipY		= RotateNoneFlipX,
	Rotate270FlipY		= Rotate90FlipX,
	RotateNoneFlipY		= Rotate180FlipX,
	Rotate90FlipY		= Rotate270FlipX

}; // enum RotateFlipType
	
}; // namespace System.Drawing
