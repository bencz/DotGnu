/*
 * RuntimeMethodHandle.cs - Implementation of the
 *			"System.RuntimeMethodHandle" class.
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

#if CONFIG_RUNTIME_INFRA

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public struct RuntimeMethodHandle
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Internal state.
	private IntPtr value_;

	// Internal constructor.
	internal RuntimeMethodHandle(IntPtr value)
			{
				value_ = value;
			}

#if !ECMA_COMPAT
	// Get the function pointer associated with this method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public IntPtr GetFunctionPointer();
#endif

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
	internal RuntimeMethodHandle(SerializationInfo info,
								 StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				MethodBase method = (MethodBase)(info.GetValue
					("MethodObj", typeof(ClrMethod)));
				if(method == null)
				{
					// Extension: check for constructors as well.
					method = (MethodBase)(info.GetValue
						("ConstructorObj", typeof(ClrConstructor)));
				}
				if(method == null)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				value_ = method.MethodHandle.value_;
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
				MethodBase method = (MethodBase)
					(MethodBase.GetMethodFromHandle(this));
				if(method is ClrConstructor)
				{
					// Extension: properly serialize constructor handles.
					info.AddValue("MethodObj", null, typeof(ClrMethod));
					info.AddValue("ConstructorObj", method,
								  typeof(ClrConstructor));
				}
				else
				{
					info.AddValue("MethodObj", method, typeof(ClrMethod));
				}
			}

#endif // CONFIG_SERIALIZATION

}; // class RuntimeMethodHandle

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
