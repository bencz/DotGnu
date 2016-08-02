/*
 * VbStrConv.cs - Implementation of the
 *			"Microsoft.VisualBasic.VbStrConv" class.
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

namespace Microsoft.VisualBasic
{

using System;

[Flags]
public enum VbStrConv
{
	None				= 0x0000,
	UpperCase			= 0x0001,
	LowerCase			= 0x0002,
	ProperCase			= 0x0003,
	Wide				= 0x0004,
	Narrow				= 0x0008,
	Katakana			= 0x0010,
	Hiragana			= 0x0020,
	SimplifiedChinese	= 0x0100,
	TraditionalChinese	= 0x0200,
	LinguisticCasing	= 0x0400

}; // enum VbStrConv

}; // namespace Microsoft.VisualBasic
