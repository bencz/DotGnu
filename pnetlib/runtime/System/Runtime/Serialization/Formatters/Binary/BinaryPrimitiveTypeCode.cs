/*
 * BinaryPrimitiveTypeCode.cs - Implementation of
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryPrimitiveTypeCode".
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

namespace System.Runtime.Serialization.Formatters.Binary
{

#if CONFIG_SERIALIZATION

// Type codes for primitive types.

internal enum BinaryPrimitiveTypeCode
{
	Boolean		= 1,
	Byte		= 2,
	Char		= 3,
	Decimal		= 5,
	Double		= 6,
	Int16		= 7,
	Int32		= 8,
	Int64		= 9,
	SByte		= 10,
	Single		= 11,
	TimeSpan	= 12,
	DateTime	= 13,
	UInt16		= 14,
	UInt32		= 15,
	UInt64		= 16,
	String		= 18,

}; // enum BinaryPrimitiveTypeCode

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization.Formatters.Binary
