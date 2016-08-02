/*
 * FieldBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.FieldBuilder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;
using System.Reflection;
using System.Globalization;
using System.Runtime.CompilerServices;

public sealed class FieldBuilder : FieldInfo, IClrProgramItem, IDetachItem
{
	// Internal state.
	internal TypeBuilder type;
	private IntPtr privateData;

	// Constructor.
	internal FieldBuilder(TypeBuilder type, String name, Type fieldType,
						  FieldAttributes attributes)
			{
				// Register this item to be detached later.
				type.module.assembly.AddDetach(this);

				// Create the field.
				lock(typeof(AssemblyBuilder))
				{
					this.type = type;
					this.privateData = ClrFieldCreate
						(((IClrProgramItem)type).ClrHandle, name,
					 	SignatureHelper.CSToILType(type.module, fieldType),
					 	attributes);
				}
			}

	// Get the custom attributes associated with this field.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override Object[] GetCustomAttributes(Type attributeType, 
												 bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get the token code for this field.
	public FieldToken GetToken()
			{
				lock(typeof(AssemblyBuilder))
				{
					return new FieldToken
						(AssemblyBuilder.ClrGetItemToken(privateData));
				}
			}

	// Get the value associated with this field on an object.
	public override Object GetValue(Object obj)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Determine if a custom attribute is defined on this field.
	public override bool IsDefined(Type attributeType, bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Set the constant value on this field.
	public void SetConstant(Object defaultValue)
			{
				try
				{
					type.StartSync();
					ValidateConstant(FieldType, defaultValue);
					lock(typeof(AssemblyBuilder))
					{
						ClrFieldSetConstant(privateData, defaultValue);
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set a custom attribute on this field.
	public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
			{
				try
				{
					type.StartSync();
					type.module.assembly.SetCustomAttribute
						(this, customBuilder);
				}
				finally
				{
					type.EndSync();
				}
			}
	public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
			{
				try
				{
					type.StartSync();
					type.module.assembly.SetCustomAttribute
						(this, con, binaryAttribute);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the marshalling information for this field.
	public void SetMarshal(UnmanagedMarshal unmanagedMarshal)
			{
				try
				{
					type.StartSync();
					if(unmanagedMarshal == null)
					{
						throw new ArgumentNullException("unmanagedMarshal");
					}
					lock(typeof(AssemblyBuilder))
					{
						ClrFieldSetMarshal
							(privateData, unmanagedMarshal.ToBytes());
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the offset of this field in an explicitly laid out class.
	public void SetOffset(int iOffset)
			{
				try
				{
					type.StartSync();
					lock(typeof(AssemblyBuilder))
					{
						ClrFieldSetOffset(privateData, iOffset);
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the value of this field on an object.
	public override void SetValue(Object obj, Object val, 
								  BindingFlags invokeAttr, 
								  Binder binder, 
								  CultureInfo culture)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get the attributes for this field.
	public override FieldAttributes Attributes 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return (FieldAttributes)
							ClrHelpers.GetMemberAttrs(privateData);
					}
				}
			}

	// Get the type that declares this field
	public override Type DeclaringType 
			{
				get
				{
					return type;
				}
			}

	// Get the handle associated with this field.
	public override RuntimeFieldHandle FieldHandle 
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Get the field type.
	public override Type FieldType 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrField.GetFieldType(privateData);
					}
				}
			}

	// Get the name of this field.
	public override String Name 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrHelpers.GetName(this);
					}
				}
			}

	// Get the reflected type for this field.
	public override Type ReflectedType 
			{
				get
				{
					return type;
				}
			}

	// Get the CLR handle for this field.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Validate a constant value against a type.
	internal static void ValidateConstant(Type type, Object value)
			{
				if(value == null)
				{
					// Cannot be null if a value type.
					if(type.IsValueType)
					{
						throw new ArgumentException(_("Emit_InvalidConstant"));
					}
				}
				else if(type.IsEnum)
				{
					// Process enumerated type values.
					if(type != value.GetType() &&
					   type.UnderlyingSystemType != value.GetType())
					{
						throw new ArgumentException(_("Emit_InvalidConstant"));
					}
				}
				else if(value is IConvertible)
				{
					// Determine if it is a simple recognised constant.
					TypeCode code = ((IConvertible)value).GetTypeCode();
					if(code == TypeCode.Empty || code == TypeCode.Object ||
					   code == TypeCode.DBNull || code == TypeCode.DateTime)
					{
						throw new ArgumentException(_("Emit_InvalidConstant"));
					}

					// Check the types.
					if(type != value.GetType())
					{
						throw new ArgumentException(_("Emit_InvalidConstant"));
					}
				}
				else
				{
					// Not a useful constant value.
					throw new ArgumentException(_("Emit_InvalidConstant"));
				}
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

	// Set data on this field within the ".sdata" section of the binary.
	internal void SetData(byte[] data, int size)
			{
				int rva;
				lock(typeof(AssemblyBuilder))
				{
					if(data != null)
					{
						rva = ModuleBuilder.ClrModuleWriteData
							(type.module.privateData, data);
					}
					else
					{
						rva = ModuleBuilder.ClrModuleWriteGap
							(type.module.privateData, size);
					}
					ClrFieldSetRVA(privateData, rva);
				}
			}

	// Create a new field and attach it to a particular class.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrFieldCreate
			(IntPtr classInfo, String name, IntPtr type,
			 FieldAttributes attributes);

	// Internal version of "SetOffset".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrFieldSetOffset(IntPtr item, int offset);

	// Internal version of "SetMarshal" (used by ParameterBuilder also).
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void ClrFieldSetMarshal(IntPtr item, byte[] data);

	// Internal version of "SetConstant" (used by ParameterBuilder and
	// PropertyBuilder also).
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void ClrFieldSetConstant(IntPtr item, Object value);

	// Set the RVA on a field.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrFieldSetRVA(IntPtr item, int rva);

}; // class FieldBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
