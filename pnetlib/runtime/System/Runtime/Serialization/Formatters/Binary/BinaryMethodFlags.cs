/*
 * BinaryMethodFlags.cs - Implementation of
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryMethodCallFlags".
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

#if CONFIG_REMOTING

// Flags that are used on method calls and responses.

[Flags]
internal enum BinaryMethodFlags
{
	NoArguments					= 0x0001,
	PrimitiveArguments			= 0x0002,
	ArgumentsInSimpleArray		= 0x0004,
	ArgumentsInMultiArray		= 0x0008,
	ExcludeLogicalCallContext	= 0x0010,
	IncludesLogicalCallContext	= 0x0040,
	IncludesSignature			= 0x0080,

}; // enum BinaryMethodFlags

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Serialization.Formatters.Binary
