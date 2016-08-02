/*
 * ClrProperty.cs - Implementation of the
 *		"System.Reflection.ClrProperty" class.
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
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

internal sealed class ClrProperty : PropertyInfo, IClrProgramItem
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Private data used by the runtime engine.
	private IntPtr privateData;

	// Implement the IClrProgramItem interface.
	public IntPtr ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Get the custom attributes attached to this property.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this property.
	public override bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

	// Override inherited properties.
	public override Type DeclaringType
			{
				get
				{
					return ClrHelpers.GetDeclaringType(this);
				}
			}
	public override Type ReflectedType
			{
				get
				{
					return ClrHelpers.GetDeclaringType(this);
				}
			}
	public override String Name
			{
				get
				{
					return ClrHelpers.GetName(this);
				}
			}
	public override PropertyAttributes Attributes
			{
				get
				{
					return (PropertyAttributes)
						ClrHelpers.GetMemberAttrs(privateData);
				}
			}
	public override bool CanRead
			{
				get
				{
					return ClrHelpers.HasSemantics
						(privateData, MethodSemanticsAttributes.Getter, true);
				}
			}
	public override bool CanWrite
			{
				get
				{
					return ClrHelpers.HasSemantics
						(privateData, MethodSemanticsAttributes.Setter, true);
				}
			}
	public override Type PropertyType
			{
				get
				{
					return GetPropertyType(privateData);
				}
			}

	// Get an array of all accessor methods on this property.
	public override MethodInfo[] GetAccessors(bool nonPublic)
			{
				MethodInfo getter =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Getter,
											nonPublic);
				MethodInfo setter =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Setter,
											nonPublic);
				MethodInfo other =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Other,
											nonPublic);
				int size = ((getter != null) ? 1 : 0) +
						   ((setter != null) ? 1 : 0) +
						   ((other != null) ? 1 : 0);
				MethodInfo[] array = new MethodInfo [size];
				int posn = 0;
				if(getter != null)
				{
					array[posn++] = getter;
				}
				if(setter != null)
				{
					array[posn++] = setter;
				}
				if(other != null)
				{
					array[posn++] = other;
				}
				return array;
			}

	// Get the "get" accessor method on this property.
	public override MethodInfo GetGetMethod(bool nonPublic)
			{
				return ClrHelpers.GetSemantics
					(privateData, MethodSemanticsAttributes.Getter, nonPublic);
			}

	// Get the index parameters for this property.
	public override ParameterInfo[] GetIndexParameters()
			{
				MethodInfo method =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Getter,
											true);
				if(method != null)
				{
					return method.GetParameters();
				}
				method =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Setter,
											true);
				if(method != null)
				{
					ParameterInfo[] parameters = method.GetParameters();
					ParameterInfo[] newParams;
					if(parameters != null && parameters.Length >= 1)
					{
						newParams = new ParameterInfo [parameters.Length - 1];
						Array.Copy(parameters, newParams, newParams.Length);
						return newParams;
					}
				}
				throw new MethodAccessException
					(_("Reflection_NoPropertyAccess"));
			}

	// Get the "set" accessor method on this property.
	public override MethodInfo GetSetMethod(bool nonPublic)
			{
				return ClrHelpers.GetSemantics
					(privateData, MethodSemanticsAttributes.Setter, nonPublic);
			}

	// Get the value associated with this property on an object.
	public override Object GetValue(Object obj, BindingFlags invokeAttr,
									Binder binder, Object[] index,
									CultureInfo culture)
			{
				MethodInfo getter =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Getter,
											true);
				if(getter == null)
				{
					throw new ArgumentException
						(_("Reflection_NoPropertyGet"));
				}
				return getter.Invoke(obj, invokeAttr, binder, index, culture);
			}
	public override Object GetValue(Object obj, Object[] index)
			{
				MethodInfo getter =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Getter,
											true);
				if(getter == null)
				{
					throw new ArgumentException
						(_("Reflection_NoPropertyGet"));
				}
				return getter.Invoke(obj, BindingFlags.Default,
									 Type.DefaultBinder, index, null);
			}

	// Set the value associated with this property on an object.
	public override void SetValue(Object obj, Object value,
								  BindingFlags invokeAttr, Binder binder,
								  Object[] index, CultureInfo culture)
			{
				MethodInfo setter =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Setter,
											true);
				if(setter == null)
				{
					throw new ArgumentException
						(_("Reflection_NoPropertySet"));
				}
				Object[] parameters;
				if(index == null)
				{
					parameters = new Object [1];
					parameters[0] = value;
				}
				else
				{
					parameters = new Object [index.Length + 1];
					Array.Copy(index, parameters, index.Length);
					parameters[index.Length] = value;
				}
				setter.Invoke(obj, invokeAttr, binder, parameters, culture);
			}
	public override void SetValue(Object obj, Object value, Object[] index)
			{
				MethodInfo setter =
					ClrHelpers.GetSemantics(privateData,
											MethodSemanticsAttributes.Setter,
											true);
				if(setter == null)
				{
					throw new ArgumentException
						(_("Reflection_NoPropertySet"));
				}
				Object[] parameters;
				if(index == null)
				{
					parameters = new Object [1];
					parameters[0] = value;
				}
				else
				{
					parameters = new Object [index.Length + 1];
					Array.Copy(index, parameters, index.Length);
					parameters[index.Length] = value;
				}
				setter.Invoke(obj, BindingFlags.Default,
							  Type.DefaultBinder, parameters, null);
			}

	// Get the type associated with this property item.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Type GetPropertyType(IntPtr item);

	// Convert the property name into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(PropertyType.ToString());
				builder.Append(' ');
				builder.Append(Name);
				return builder.ToString();
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this property.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				MemberInfoSerializationHolder.Serialize
					(info, MemberTypes.Property, Name, null, ReflectedType);
			}

#endif

}; // class RuntimePropertyInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
