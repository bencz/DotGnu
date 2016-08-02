/*
 * Pointer.cs - Implementation of the "System.Reflection.Pointer" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION && !ECMA_COMPAT

using System;
using System.Runtime.Serialization;

[CLSCompliant(false)]
public unsafe sealed class Pointer
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Internal state.
	private void *ptr;
	private Type type;

	// Constructor.
	private Pointer(void *ptr, Type type)
			{
				this.ptr = ptr;
				this.type = type;
			}

	// Box a pointer value.
	public static Object Box(void *ptr, Type type)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				else if(!(type.IsPointer))
				{
					throw new ArgumentException(_("Arg_PointerType"));
				}
				else
				{
					return new Pointer(ptr, type);
				}
			}

	// Unbox a pointer value.
	public static void *Unbox(Object ptr)
			{
				if(!(ptr is Pointer))
				{
					throw new ArgumentException(_("Arg_PointerValue"));
				}
				return ((Pointer)ptr).ptr;
			}

#if CONFIG_SERIALIZATION

	// Implement the ISerializable interface.
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
			{
				throw new NotSupportedException
					(_("NotSupp_SerializePointer"));
			}

#endif

}; // class Pointer

#endif // CONFIG_REFLECTION && !ECMA_COMPAT

}; // namespace System.Reflection
