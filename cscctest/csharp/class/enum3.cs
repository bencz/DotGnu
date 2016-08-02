/*
 * enum3.cs - Test enumerated type definitions that include self-references.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

public enum Attributes : uint
{
	// Traditional attribute names.
	A_NORMAL			= 0x00000000,
	A_ATTRIBUTES		= 0xFFFFFF00,
	A_CHARTEXT			= 0x000000FF,
	A_COLOR				= 0x0000FF00,
	A_STANDOUT			= 0x00010000,
	A_UNDERLINE			= 0x00020000,
	A_REVERSE			= 0x00040000,
	A_BLINK				= 0x00080000,
	A_DIM				= 0x00100000,
	A_BOLD				= 0x00200000,
	A_ALTCHARSET		= 0x00400000,
	A_INVIS				= 0x00800000,
	A_PROTECT			= 0x01000000,
	A_HORIZONTAL		= 0x02000000,
	A_LEFT				= 0x04000000,
	A_LOW				= 0x08000000,
	A_RIGHT				= 0x10000000,
	A_TOP				= 0x20000000,
	A_VERTICAL			= 0x40000000,

	// XSI standard attribute names.
	WA_ATTRIBUTES		= A_ATTRIBUTES,
	WA_NORMAL			= A_NORMAL,
	WA_STANDOUT			= A_STANDOUT,
	WA_UNDERLINE		= A_UNDERLINE,
	WA_REVERSE			= A_REVERSE,
	WA_BLINK			= A_BLINK,
	WA_DIM				= A_DIM,
	WA_BOLD				= A_BOLD,
	WA_ALTCHARSET		= A_ALTCHARSET,
	WA_INVIS			= A_INVIS,
	WA_PROTECT			= A_PROTECT,
	WA_HORIZONTAL		= A_HORIZONTAL,
	WA_LEFT				= A_LEFT,
	WA_LOW				= A_LOW,
	WA_RIGHT			= A_RIGHT,
	WA_TOP				= A_TOP,
	WA_VERTICAL			= A_VERTICAL,

} // enum Attributes
