/*
 * LineCap.cs - Implementation of the
 *			"System.Drawing.Drawing2D.LineCap" class.
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

namespace System.Drawing.Drawing2D
{

public enum LineCap
{
	Flat			= 0x0000,
	Square			= 0x0001,
	Round			= 0x0002,
	Triangle		= 0x0003,
	NoAnchor		= 0x0010,
	SquareAnchor	= 0x0011,
	RoundAnchor		= 0x0012,
	DiamondAnchor	= 0x0013,
	ArrowAnchor		= 0x0014,
	AnchorMask		= 0x00F0,
	Custom			= 0x00FF

}; // enum LineCap

}; // namespace System.Drawing.Drawing2D
