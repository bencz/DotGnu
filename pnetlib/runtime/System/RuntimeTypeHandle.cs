/*
 * RuntimeTypeHandle.cs - Implementation of "System.RuntimeTypeHandle".
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System.Reflection;
using System.Runtime.Serialization;

public struct RuntimeTypeHandle
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Internal state.
	private IntPtr value_;

	// Constructor.
	internal RuntimeTypeHandle(IntPtr value)
			{
				value_ = value;
			}

	// Properties.
	public IntPtr Value
			{
				get
				{
					return value_;
				}
			}

#if CONFIG_SERIALIZATION

	// De-serialize this object.
	internal RuntimeTypeHandle(SerializationInfo info,
							   StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				Type t = (Type)(info.GetValue("TypeObj", typeof(ClrType)));
				if(t == null)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				value_ = t.TypeHandle.value_;
			}

	// Get the serialization data for this object.
	public void GetObjectData(SerializationInfo info,
							  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				if(value_ == IntPtr.Zero)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				ClrType type = (ClrType)(Type.GetTypeFromHandle(this));
				info.AddValue("TypeObj", type, typeof(ClrType));
			}

#endif // CONFIG_SERIALIZATION

}; // class RuntimeTypeHandle

}; // namespace System
