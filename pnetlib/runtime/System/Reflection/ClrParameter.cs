/*
 * ClrParameter.cs - Implementation of the
 *			"System.Reflection.ClrParameter" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

using System;
using System.Runtime.CompilerServices;

internal class ClrParameter : ParameterInfo, IClrProgramItem
{
	// Internal state.
	private IntPtr privateData;

	// Implement the IClrProgramItem interface.
	public IntPtr ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Constructor.
	internal ClrParameter(MemberInfo member, IntPtr privateData,
						  int num, Type type)
			: base()
			{
				this.privateData = privateData;
				MemberImpl = member;
				AttrsImpl = ParameterAttributes.None;
				ClassImpl = type;
				if(privateData != IntPtr.Zero)
				{
					// Populate the fields in the parent class.
					AttrsImpl = GetParamAttrs(privateData);
					NameImpl = GetParamName(privateData);
				}
				if(num != 0)
				{
					PositionImpl = num - 1;
				}
				else
				{
					PositionImpl = 0;
					AttrsImpl |= ParameterAttributes.Retval;
				}
			}

	// Get the attributes that are associated with a parameter item.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static ParameterAttributes GetParamAttrs(IntPtr item);

	// Get the name that is associated with a parameter item.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetParamName(IntPtr item);

#if !ECMA_COMPAT

	// Get the custom attributes attached to this parameter.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this parameter.
	public override bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

#else  // ECMA_COMPAT

	// Internal interface used when ICustomAttributeProvider
	// is declared as private in ParameterInfo.
	internal override Object[] ClrGetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	internal override Object[] ClrGetCustomAttributes
					(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}
	internal override bool ClrIsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

#endif // ECMA_COMPAT

#if !ECMA_COMPAT

	// Get the default value that is associated with this parameter.
	public override Object DefaultValue
			{
				get
				{
					if(DefaultValueImpl != null)
					{
						return DefaultValueImpl;
					}
					if(privateData != IntPtr.Zero)
					{
						DefaultValueImpl = GetDefault(privateData);
					}
					return DefaultValueImpl;
				}
			}

	// Get the default value associated with this parameter.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object GetDefault(IntPtr item);

#endif // !ECMA_COMPAT

}; // class ClrParameter

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
