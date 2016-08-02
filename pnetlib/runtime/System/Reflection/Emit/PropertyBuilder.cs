/*
 * PropertyBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.PropertyBuilder" class.
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

public sealed class PropertyBuilder
	: PropertyInfo, IClrProgramItem, IDetachItem
{
	// Internal state.
	private TypeBuilder type;
	private Type returnType;
	private MethodInfo getMethod;
	private MethodInfo setMethod;
	private IntPtr privateData;

	// Constructor.
	internal PropertyBuilder(TypeBuilder type, String name,
							 PropertyAttributes attributes,
				 			 Type returnType, Type[] parameterTypes)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(returnType == null)
				{
					throw new ArgumentNullException("returnType");
				}

				// Initialize this object's internal state.
				this.type = type;
				this.returnType = returnType;
				this.getMethod = null;
				this.setMethod = null;

				// Register this item to be detached later.
				type.module.assembly.AddDetach(this);

				// Create the property signature.
				SignatureHelper helper =
					SignatureHelper.GetPropertySigHelper
						(type.module, returnType, parameterTypes);

				// Create the property.
				lock(typeof(AssemblyBuilder))
				{
					this.privateData = ClrPropertyCreate
						(((IClrProgramItem)type).ClrHandle, name,
						 attributes, helper.sig);
				}
			}

	// Add an "other" method to this property.
	public void AddOtherMethod(MethodBuilder mdBuilder)
			{
				try
				{
					type.StartSync();
					if(mdBuilder == null)
					{
						throw new ArgumentNullException("mdBuilder");
					}
					lock(typeof(AssemblyBuilder))
					{
						ClrPropertyAddSemantics
							(privateData, MethodSemanticsAttributes.Other,
						 	type.module.GetMethodToken(mdBuilder));
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Get the accessor methods for this property.
	public override MethodInfo[] GetAccessors(bool nonPublic)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get custom attributes form this property.
	public override Object[] GetCustomAttributes(bool inherit)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override Object[] GetCustomAttributes(Type attributeType, 
												 bool inherit)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get the "get" method for this property.
	public override MethodInfo GetGetMethod(bool nonPublic)
			{
				if(getMethod == null || nonPublic)
				{
					return getMethod;
				}
				else if((getMethod.Attributes &
							MethodAttributes.MemberAccessMask) ==
						MethodAttributes.Public)
				{
					return getMethod;
				}
				else
				{
					return null;
				}
			}

	// Get the index parameters for this property.
	public override ParameterInfo[] GetIndexParameters()
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get the "set" method for this property.
	public override MethodInfo GetSetMethod(bool nonPublic)
			{
				if(setMethod == null || nonPublic)
				{
					return setMethod;
				}
				else if((setMethod.Attributes &
							MethodAttributes.MemberAccessMask) ==
						MethodAttributes.Public)
				{
					return setMethod;
				}
				else
				{
					return null;
				}
			}

	// Get the value of this property on an object.
	public override Object GetValue(Object obj, Object[] index)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override Object GetValue(Object obj, BindingFlags invokeAttr, 
									Binder binder, Object[] index, 
									CultureInfo culture)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Determine if a particular custom attribute is defined on this property.
	public override bool IsDefined(Type attributeType, bool inherit)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Set the constant value on this property.
	public void SetConstant(Object defaultValue)
			{
				try
				{
					type.StartSync();
					FieldBuilder.ValidateConstant(returnType, defaultValue);
					lock(typeof(AssemblyBuilder))
					{
						FieldBuilder.ClrFieldSetConstant
							(privateData, defaultValue);
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set a custom attribute on this property.
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

	// Set the "get" method on this property.
	public void SetGetMethod(MethodBuilder mdBuilder)
			{
				try
				{
					type.StartSync();
					if(mdBuilder == null)
					{
						throw new ArgumentNullException("mdBuilder");
					}
					else if(getMethod != null)
					{
						throw new ArgumentException
							(_("Emit_GetAlreadyDefined"));
					}
					lock(typeof(AssemblyBuilder))
					{
						ClrPropertyAddSemantics
							(privateData, MethodSemanticsAttributes.Getter,
						 	type.module.GetMethodToken(mdBuilder));
					}
					getMethod = mdBuilder;
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the "set" method on this property.
	public void SetSetMethod(MethodBuilder mdBuilder)
			{
				try
				{
					type.StartSync();
					if(mdBuilder == null)
					{
						throw new ArgumentNullException("mdBuilder");
					}
					else if(setMethod != null)
					{
						throw new ArgumentException
							(_("Emit_SetAlreadyDefined"));
					}
					lock(typeof(AssemblyBuilder))
					{
						ClrPropertyAddSemantics
							(privateData, MethodSemanticsAttributes.Setter,
						 	type.module.GetMethodToken(mdBuilder));
					}
					setMethod = mdBuilder;
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the value of this property on an object.
	public override void SetValue(Object obj, Object value, Object[] index)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override void SetValue(Object obj, Object value, 
								  BindingFlags invokeAttr, 
								  Binder binder, Object[] index, 
								  CultureInfo culture)
			{
		 		throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get the attributes for this property.
	public override PropertyAttributes Attributes 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return (PropertyAttributes)
							ClrHelpers.GetMemberAttrs(privateData);
					}
				}
			}

	// Determine if we can read from this property.
	public override bool CanRead 
			{
				get
				{
					return (getMethod != null);
				}
			}

	// Determine if we can write to this property.
	public override bool CanWrite 
			{
				get
				{
					return (setMethod != null);
				}
			}

	// Get the type that this property is declared within.
	public override Type DeclaringType 
			{
				get
				{
					return type;
				}
			}

	// Get the name of this property.
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

	// Get the token associated with this property.
	public PropertyToken PropertyToken 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return new PropertyToken
							(AssemblyBuilder.ClrGetItemToken(privateData));
					}
				}
			}

	// Get the type associated with this property.
	public override Type PropertyType 
			{
				get
				{
					return returnType;
				}
			}

	// Get the reflected type that this property exists within.
	public override Type ReflectedType 
			{
				get
				{
					return type;
				}
			}

	// Get the CLR handle for this property.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

	// Create a new property and attach it to a particular class.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrPropertyCreate
			(IntPtr classInfo, String name,
			 PropertyAttributes attributes, IntPtr signature);

	// Add semantic information to this property.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrPropertyAddSemantics
			(IntPtr item, MethodSemanticsAttributes attr,
			 MethodToken token);

}; // class PropertyBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
