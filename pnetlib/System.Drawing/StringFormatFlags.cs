/*
 * StringFormatFlags.cs - Implementation of the
 *			"System.Drawing.StringFormatFlags" class.
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

[Flags]
public enum StringFormatFlags
{
	DirectionRightToLeft	= 0x0001,
	DirectionVertical		= 0x0002,
	FitBlackBox				= 0x0004,
	DisplayFormatControl	= 0x0020,
	NoFontFallback			= 0x0400,
	MeasureTrailingSpaces	= 0x0800,
	NoWrap					= 0x1000,
	LineLimit				= 0x2000,
	NoClip					= 0x4000

}; // enum StringFormatFlags
	
}; // namespace System.Drawing
