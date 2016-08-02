/*
 * ClrField.cs - Implementation of the
 *		"System.Reflection.ClrField" class.
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

internal sealed class ClrField : FieldInfo, IClrProgramItem
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

	// Get the custom attributes attached to this field.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this field.
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
	public override FieldAttributes Attributes
			{
				get
				{
					return (FieldAttributes)
						ClrHelpers.GetMemberAttrs(privateData);
				}
			}
	public override Type FieldType
			{
				get
				{
					return GetFieldType(privateData);
				}
			}

	// Get the value associated with this field on an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal Object GetValueInternal(Object obj);

	// Get the value associated with this field on an object.
	public override Object GetValue(Object obj)
			{
				if(obj == null)
				{
					// Make sure that the class constructor has been
					// executed before we access a static field.
					RuntimeHelpers.RunClassConstructor
						(DeclaringType.TypeHandle);
				}
				return GetValueInternal(obj);
			}

	// Set the value associated with this field on an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal void SetValueInternal
				(Object obj, Object value,
				 BindingFlags invokeAttr, Binder binder,
				 CultureInfo culture);

	// Set the value associated with this field on an object.
	public override void SetValue(Object obj, Object value,
						  		  BindingFlags invokeAttr,
						  		  Binder binder, CultureInfo culture)
			{
				if(obj == null)
				{
					// Make sure that the class constructor has been
					// executed before we access a static field.
					RuntimeHelpers.RunClassConstructor
						(DeclaringType.TypeHandle);
				}
				SetValueInternal(obj, value, invokeAttr, binder, culture);
			}

	// Get the type of this field item.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static Type GetFieldType(IntPtr item);

#if !ECMA_COMPAT

	// Get the handle that is associated with this field.
	public override RuntimeFieldHandle FieldHandle
			{
				get
				{
					return new RuntimeFieldHandle(privateData);
				}
			}

	// Get the value directly from a typed reference.
	[CLSCompliant(false)]
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override object GetValueDirect(TypedReference obj);

	// Set the value directly to a typed reference.
	[CLSCompliant(false)]
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override void SetValueDirect
			(TypedReference obj, Object value);

#endif // !ECMA_COMPAT

	// Convert the field name into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(FieldType.ToString());
				builder.Append(' ');
				builder.Append(Name);
				return builder.ToString();
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this field.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				MemberInfoSerializationHolder.Serialize
					(info, MemberTypes.Field, Name, ToString(), ReflectedType);
			}

#endif

}; // class ClrField

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
