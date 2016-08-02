/*
 * CompareOptions.cs - Implementation of
 *		"System.Globalization.CompareOptions".
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Globalization
{

using System;

[Flags]
public enum CompareOptions
{

	None           = 0x00000000,
	IgnoreCase     = 0x00000001,
	IgnoreNonSpace = 0x00000002,
	IgnoreSymbols  = 0x00000004,
	IgnoreKanaType = 0x00000008,
	IgnoreWidth    = 0x00000010,
	StringSort     = 0x20000000,
	Ordinal        = 0x40000000

}; // enum CompareOptions

}; // namespace System.Globalization
