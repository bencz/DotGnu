/*
 * BinaryArrayType.cs - Implementation of the
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryArrayType" class.
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

// Type codes for array layout.

internal enum BinaryArrayType
{
	Single								= 0,
	Jagged								= 1,
	Multidimensional					= 2,
	SingleWithLowerBounds				= 3,
	JaggedWithLowerBounds				= 4,
	MultidimensionalWithLowerBounds		= 5

}; // enum BinaryArrayType

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization.Formatters.Binary
