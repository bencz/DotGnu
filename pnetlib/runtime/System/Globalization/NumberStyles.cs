/*
 * NumberStyles.cs - Implementation of "System.Globalization.NumberStyles".
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
public enum NumberStyles
{
	None                    = 0x0000,
	AllowLeadingWhite		= 0x0001,
	AllowTrailingWhite      = 0x0002,
	AllowLeadingSign        = 0x0004,
	Integer                 = 0x0007,
	AllowTrailingSign       = 0x0008,
	AllowParentheses        = 0x0010,
	AllowDecimalPoint       = 0x0020,
	AllowThousands          = 0x0040,
	Number                  = 0x006F,
	AllowExponent           = 0x0080,
	Float                   = 0x00A7,
	AllowCurrencySymbol     = 0x0100,
	Any                     = 0x01FF,
	Currency                = 0x017F,
	AllowHexSpecifier       = 0x0200,
	HexNumber               = 0x0203

}; // enum NumberStyles

}; // namespace System.Globalization
