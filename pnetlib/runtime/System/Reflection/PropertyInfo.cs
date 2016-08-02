/*
 * PropertyInfo.cs - Implementation of the
 *		"System.Reflection.PropertyInfo" class.
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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_PropertyInfo))]
#endif
#endif
public abstract class PropertyInfo : MemberInfo
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _PropertyInfo
#endif
{

	// Constructor.
	protected PropertyInfo() : base() {}

	// Get the member type for this item.
	public override MemberTypes MemberType
			{
				get
				{
					return MemberTypes.Property;
				}
			}

	// Get an array of all accessor methods on this property.
	public abstract MethodInfo[] GetAccessors(bool nonPublic);
	public MethodInfo[] GetAccessors()
			{
				return GetAccessors(false);
			}

	// Get the "get" accessor method on this property.
	public abstract MethodInfo GetGetMethod(bool nonPublic);
	public MethodInfo GetGetMethod()
			{
				return GetGetMethod(false);
			}

	// Get the index parameters for this property.
	public abstract ParameterInfo[] GetIndexParameters();

	// Get the "set" accessor method on this property.
	public abstract MethodInfo GetSetMethod(bool nonPublic);
	public MethodInfo GetSetMethod()
			{
				return GetSetMethod(false);
			}

	// Get the value associated with this property on an object.
	public abstract Object GetValue(Object obj, BindingFlags invokeAttr,
									Binder binder, Object[] index,
									CultureInfo culture);
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public virtual Object GetValue(Object obj, Object[] index)
			{
				return GetValue(obj, BindingFlags.Default,
								Type.DefaultBinder, index, null);
			}

	// Set the value associated with this property on an object.
	public abstract void SetValue(Object obj, Object value,
								  BindingFlags invokeAttr, Binder binder,
								  Object[] index, CultureInfo culture);
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public virtual void SetValue(Object obj, Object value, Object[] index)
			{
				SetValue(obj, value, BindingFlags.Default,
						 Type.DefaultBinder, index, null);
			}

	// Get the attributes associated with this property.
	public abstract PropertyAttributes Attributes { get; }

	// Determine if it is possible to read from this property.
	public abstract bool CanRead { get; }

	// Determine if it is possible to write to this property.
	public abstract bool CanWrite { get; }

	// Get the type of this property.
	public abstract Type PropertyType { get; }

#if !ECMA_COMPAT

	// Determine if this property has a special name.
	public bool IsSpecialName
			{
				get
				{
					return ((Attributes & PropertyAttributes.SpecialName) != 0);
				}
			}

#endif // !ECMA_COMPAT

}; // class PropertyInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
