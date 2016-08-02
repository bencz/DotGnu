/*
 * BinaryElementType.cs - Implementation of the
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryElementType" class.
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

// Type codes for individual elements within a binary stream.

internal enum BinaryElementType
{
	Header 						= 0,
	RefTypeObject				= 1,
	RuntimeObject				= 4,
	ExternalObject				= 5,
	String						= 6,
	GenericArray				= 7,
	BoxedPrimitiveTypeValue		= 8,
	ObjectReference				= 9,
	NullValue					= 10,
	End							= 11,
	Assembly					= 12,
	ArrayFiller8b				= 13,
	ArrayFiller32b				= 14,
	ArrayOfPrimitiveType		= 15,
	ArrayOfObject				= 16,
	ArrayOfString				= 17,
	MethodCall					= 21,
	MethodResponse				= 22,

}; // enum BinaryElementType

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization.Formatters.Binary
