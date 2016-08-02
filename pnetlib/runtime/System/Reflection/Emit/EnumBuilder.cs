/*
 * EnumBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.EnumBuilder" class.
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

public sealed class EnumBuilder : Type
{
	// Internal state.
	internal TypeBuilder builder;
	private Type underlyingType;
	private FieldBuilder underlyingField;

	// Constructor.
	internal EnumBuilder(ModuleBuilder module, String name, String nspace,
						 TypeAttributes visibility, Type underlyingType)
			{
				// Only allowed to specify the visibility.
				if((visibility & ~TypeAttributes.VisibilityMask) != 0)
				{
					throw new ArgumentException(_("Emit_InvalidTypeAttrs"));
				}

				// Create a type builder behind the scenes.
				builder = new TypeBuilder
					(module, name, nspace,
					 visibility | TypeAttributes.Sealed,
					 typeof(System.Enum), null, PackingSize.Unspecified,
					 0, null);

				// Define the "value__" field for the enumeration.
				this.underlyingType = underlyingType;
				this.underlyingField = builder.DefineField
					("value__", underlyingType,
					 FieldAttributes.Private | FieldAttributes.SpecialName);
			}

	// Create the final type for this enumeration.
	public Type CreateType()
			{
				return builder.CreateType();
			}

	// Define a literal within this enumeration.
	public FieldBuilder DefineLiteral(String literalName, Object literalValue)
			{
				FieldBuilder field;
				field = builder.DefineField
					(literalName, builder,	// Note: use correct enum type.
					 FieldAttributes.Public | FieldAttributes.Static |
					 FieldAttributes.Literal);
				field.SetConstant(literalValue);
				return field;
			}

	// Invoke a specific type member.
	public override Object InvokeMember
				(String name, BindingFlags invokeAttr, Binder binder,
				 Object target, Object[] args, ParameterModifier[] modifiers,
				 CultureInfo culture, String[] namedParameters)
			{
				return builder.InvokeMember(name, invokeAttr, binder,
										    target, args, modifiers,
										    culture, namedParameters);
			}

	// Implementation of "GetConstructor" provided by subclasses.
	protected override ConstructorInfo
					GetConstructorImpl(BindingFlags bindingAttr,
								       Binder binder,
								       CallingConventions callingConventions,
								       Type[] types,
								       ParameterModifier[] modifiers)
			{
				return builder.GetConstructor
						(bindingAttr, binder, callingConventions,
					     types, modifiers);
			}

	// Get all constructors for this type.
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
			{
				return builder.GetConstructors(bindingAttr);
			}

	// Get the custom attributes that are associated with this member.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return builder.GetCustomAttributes(inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return builder.GetCustomAttributes(type, inherit);
			}

	// Determine if custom attributes are defined for this member.
	public override bool IsDefined(Type type, bool inherit)
			{
				return builder.IsDefined(type, inherit);
			}

	// Get the element type.
	public override Type GetElementType()
			{
				return builder.GetElementType();
			}

	// Get an event from this type.
	public override EventInfo GetEvent(String name, BindingFlags bindingAttr)
			{
				return builder.GetEvent(name, bindingAttr);
			}

	// Get the list of all events within this type.
	public override EventInfo[] GetEvents()
			{
				return builder.GetEvents();
			}
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
			{
				return builder.GetEvents(bindingAttr);
			}

	// Get a field from this type.
	public override FieldInfo GetField(String name, BindingFlags bindingAttr)
			{
				return builder.GetField(name, bindingAttr);
			}

	// Get the list of all fields within this type.
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
			{
				return builder.GetFields(bindingAttr);
			}

	// Get an interface from within this type.
	public override Type GetInterface(String name, bool ignoreCase)
			{
				return builder.GetInterface(name, ignoreCase);
			}

	// Get an interface mapping for this type.
	public override InterfaceMapping GetInterfaceMap(Type interfaceType)
			{
				return builder.GetInterfaceMap(interfaceType);
			}

	// Get the list of all interfaces that are implemented by this type.
	public override Type[] GetInterfaces()
			{
				return builder.GetInterfaces();
			}

	// Get a list of members that have a specific name.
	public override MemberInfo[] GetMember
				(String name, MemberTypes type, BindingFlags bindingAttr)
			{
				return builder.GetMember(name, type, bindingAttr);
			}

	// Get a list of all members in this type.
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				return builder.GetMembers(bindingAttr);
			}

	// Implementation of "GetMethod".
	protected override MethodInfo GetMethodImpl
				(String name, BindingFlags bindingAttr,
				 Binder binder, CallingConventions callConvention,
				 Type[] types, ParameterModifier[] modifiers)
			{
				return builder.GetMethod(name, bindingAttr, binder,
									     callConvention, types, modifiers);
			}

	// Get a list of all methods in this type.
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return builder.GetMethods(bindingAttr);
			}

	// Get a nested type that is contained within this type.
	public override Type GetNestedType(String name, BindingFlags bindingAttr)
			{
				return builder.GetNestedType(name, bindingAttr);
			}

	// Get a list of all nested types in this type.
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
			{
				return builder.GetNestedTypes(bindingAttr);
			}

	// Get a list of all properites in this type.
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
			{
				return builder.GetProperties(bindingAttr);
			}

	// Get a specific property from within this type.
	protected override PropertyInfo GetPropertyImpl
				(String name, BindingFlags bindingAttr, Binder binder,
				 Type returnType, Type[] types, ParameterModifier[] modifiers)
			{
				return builder.GetProperty(name, bindingAttr, binder,
										   returnType, types, modifiers);
			}

	// Get the attribute flags for this type.
	protected override TypeAttributes GetAttributeFlagsImpl()
			{
				return builder.attr;
			}

	// Determine if this type has an element type.
	protected override bool HasElementTypeImpl()
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Determine if this type is an array.
	protected override bool IsArrayImpl()
			{
				return false;
			}

	// Determine if this type is a "by reference" type.
	protected override bool IsByRefImpl()
			{
				return false;
			}
	
	// Determine if this type imports a COM type.
	protected override bool IsCOMObjectImpl()
			{
				return false;
			}

	// Determine if this is a pointer type.
	protected override bool IsPointerImpl()
			{
				return false;
			}

	// Determine if this is a primitive type.
	protected override bool IsPrimitiveImpl()
			{
				return false;
			}

	// Determine if this is a value type.
	protected override bool IsValueTypeImpl()
			{
				return true;
			}

	// Set a custom attribute on this enum builder.
	public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
			{
				builder.SetCustomAttribute(customBuilder);
			}
	public void SetCustomAttribute(ConstructorInfo con,
								   byte[] binaryAttribute)
			{
				builder.SetCustomAttribute(con, binaryAttribute);
			}

	// Get the assembly associated with this type.
	public override Assembly Assembly
			{
				get
				{
					return builder.Assembly;
				}
			}

	// Get the full assembly-qualified name of this type.
	public override String AssemblyQualifiedName
			{
				get
				{
					return builder.AssemblyQualifiedName;
				}
			}

	// Get the declaring type.
	public override Type DeclaringType
			{
				get
				{
					return builder.DeclaringType;
				}
			}

	// Get the full name of this type.
	public override String FullName
			{
				get
				{
					return builder.FullName;
				}
			}

	// Get the base type of this type.
	public override Type BaseType
			{
				get
				{
					return builder.BaseType;
				}
			}

	// Get the GUID of this type.
	public override Guid GUID
			{
				get
				{
					return builder.GUID;
				}
			}

	// Get the module associated with this type.
	public override Module Module
			{
				get
				{
					return builder.Module;
				}
			}

	// Get the name of this type.
	public override String Name
			{
				get
				{
					return builder.Name;
				}
			}

	// Get the namespace of this type.
	public override String Namespace
			{
				get
				{
					return builder.Namespace;
				}
			}

	// Get the reflected type.
	public override Type ReflectedType
			{
				get
				{
					return builder.ReflectedType;
				}
			}

	// Get the type handle for this enumerated type.
	public override RuntimeTypeHandle TypeHandle
			{
				get
				{
					return builder.TypeHandle;
				}
			}

	// Get the token for this enumerated type.
	public TypeToken TypeToken
			{
				get
				{
					return builder.TypeToken;
				}
			}

	// Get the underlying field.
	public FieldBuilder UnderlyingField
			{
				get
				{
					return underlyingField;
				}
			}

	// Get the underlying type for this enumeration.
	public override Type UnderlyingSystemType
			{
				get
				{
					return underlyingType;
				}
			}

}; // class EnumBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
