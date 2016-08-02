/*
 * TypeCode.cs - Implementation of the "System.TypeCode" class.
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

namespace System
{

#if ECMA_COMPAT

internal enum TypeCode
{
	Empty		= 0x00,
	Object		= 0x01,
	DBNull		= 0x02,
	Boolean		= 0x03,
	Char		= 0x04,
	SByte		= 0x05,
	Byte		= 0x06,
	Int16		= 0x07,
	UInt16		= 0x08,
	Int32		= 0x09,
	UInt32		= 0x0A,
	Int64		= 0x0B,
	UInt64		= 0x0C,
	Single		= 0x0D,
	Double		= 0x0E,
	Decimal		= 0x0F,
	DateTime	= 0x10,
	String		= 0x12

}; // enum TypeCode

#endif // ECMA_COMPAT

}; // namespace System
