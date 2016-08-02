/*
 * BinaryTypeTag.cs - Implementation of the
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryTypeTag" class.
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

// Type tags for describing the type of a serialized value.

internal enum BinaryTypeTag
{
	PrimitiveType			= 0,
	String					= 1,
	ObjectType				= 2,
	RuntimeType				= 3,
	GenericType				= 4,
	ArrayOfObject			= 5,
	ArrayOfString			= 6,
	ArrayOfPrimitiveType	= 7,

}; // enum BinaryTypeTag

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization.Formatters.Binary
