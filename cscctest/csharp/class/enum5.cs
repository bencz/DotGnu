/*
 * enum4.cs - Test forward enumerated type definitions.
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
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

enum Color
{
	Red=0x01,
	Green=Red<<5,
	Blue=Green>>2
}

class UseColor
{
	// "Blue" should be 2, but may be 0 if there are ordering
	// problems with the evaluation of default constant values.
	public static readonly Color c = Color.Blue;
}

