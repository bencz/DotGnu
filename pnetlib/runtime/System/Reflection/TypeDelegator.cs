/*
 * TypeDelegator.cs - Implementation of the
 *			"System.Reflection.TypeDelegator" class.
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
using System.Globalization;

public class TypeDelegator : Type
{
	// Internal state.
	protected Type typeImpl;

	// Constructors.
	protected TypeDelegator() {}
	public TypeDelegator(Type delegatingType)
			{
				if(delegatingType == null)
				{
					throw new ArgumentNullException("delegatingType");
				}
				typeImpl = delegatingType;
			}

	// Get the attribute flags for this type.
	protected override TypeAttributes GetAttributeFlagsImpl()
			{
				return typeImpl.Attributes;
			}

	// Get the element type for this type.
	public override Type GetElementType()
			{
				return typeImpl.GetElementType();
			}

	// Get all interfaces that this type implements.
	public override Type[] GetInterfaces()
			{
				return typeImpl.GetInterfaces();
			}

	// Get an interface mapping for a specific interface type.
	public override InterfaceMapping GetInterfaceMap(Type interfaceType)
			{
				return typeImpl.GetInterfaceMap(interfaceType);
			}

	// Implementation of the "IsArray" property.
	protected override bool IsArrayImpl()
			{
				return typeImpl.IsArray;
			}

	// Implementation of the "IsPointer" property.
	protected override bool IsPointerImpl()
			{
				return typeImpl.IsPointer;
			}

	// Implementation of the "IsPrimitive" property.
	protected override bool IsPrimitiveImpl()
			{
				return typeImpl.IsPrimitive;
			}

	// Implementation of the "IsValueType" property.
	protected override bool IsValueTypeImpl()
			{
				return typeImpl.IsValueType;
			}

	// General properties.
	public override String AssemblyQualifiedName
			{
				get
				{
					return typeImpl.AssemblyQualifiedName;
				}
			}
	public override Type BaseType
			{
				get
				{
					return typeImpl.BaseType;
				}
			}
	public override String FullName
			{
				get
				{
					return typeImpl.FullName;
				}
			}
	public override RuntimeTypeHandle TypeHandle
			{
				get
				{
					return typeImpl.TypeHandle;
				}
			}
	public override System.Reflection.Assembly Assembly
			{
				get
				{
					return typeImpl.Assembly;
				}
			}

	// Get the custom attributes for this type.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return typeImpl.GetCustomAttributes(inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return typeImpl.GetCustomAttributes(type, inherit);
			}

	// Determine if custom attributes are defined for this type.
	public override bool IsDefined(Type type, bool inherit)
			{
				return typeImpl.IsDefined(type, inherit);
			}

	// Get an interface that this type implements.
	public override Type GetInterface(String name, bool ignoreCase)
			{
				return typeImpl.GetInterface(name, ignoreCase);
			}

	// Implementation of "GetConstructor" provided by subclasses.
	protected override ConstructorInfo
					GetConstructorImpl(BindingFlags bindingAttr,
								       Binder binder,
								       CallingConventions callingConventions,
								       Type[] types,
								       ParameterModifier[] modifiers)
			{
				return typeImpl.GetConstructor
					(bindingAttr, binder, callingConventions,
					 types, modifiers);
			}

	// Get all constructors for this type.
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
			{
				return typeImpl.GetConstructors(bindingAttr);
			}

	// Get an event from this type.
	public override EventInfo GetEvent(String name, BindingFlags bindingAttr)
			{
				return typeImpl.GetEvent(name, bindingAttr);
			}

	// Get all events from this type.
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
			{
				return typeImpl.GetEvents(bindingAttr);
			}
	public override EventInfo[] GetEvents()
			{
				return typeImpl.GetEvents();
			}


	// Get a field from this type.
	public override FieldInfo GetField(String name, BindingFlags bindingAttr)
			{
				return typeImpl.GetField(name, bindingAttr);
			}

	// Get all fields from this type.
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
			{
				return typeImpl.GetFields(bindingAttr);
			}

	// Get a member from this type.
	public override MemberInfo[] GetMember
				(String name, MemberTypes type, BindingFlags bindingAttr)
			{
				return typeImpl.GetMember(name, type, bindingAttr);
			}

	// Get all members from this type.
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				return typeImpl.GetMembers(bindingAttr);
			}

	// Implementation of "GetMethod" provided by subclasses.
	protected override MethodInfo
					GetMethodImpl(String name,
								  BindingFlags bindingAttr, Binder binder,
								  CallingConventions callingConventions,
								  Type[] types,
								  ParameterModifier[] modifiers)
			{
				return typeImpl.GetMethod
					(name, bindingAttr, binder, callingConventions,
					 types, modifiers);
			}

	// Get all methods from this type.
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return typeImpl.GetMethods(bindingAttr);
			}

	// Get a nested type from this type.
	public override Type GetNestedType(String name, BindingFlags bindingAttr)
			{
				return typeImpl.GetNestedType(name, bindingAttr);
			}

	// Get all nested types from this type.
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
			{
				return typeImpl.GetNestedTypes(bindingAttr);
			}

	// Implementation of "GetProperty" provided by subclasses.
	protected override PropertyInfo
					GetPropertyImpl(String name,
									BindingFlags bindingAttr, Binder binder,
								    Type returnType, Type[] types,
								    ParameterModifier[] modifiers)
			{
				return typeImpl.GetProperty
					(name, bindingAttr, binder, returnType,
					 types, modifiers);
			}

	// Get all properties from this type.
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
			{
				return typeImpl.GetProperties(bindingAttr);
			}

	// Implementation of the "HasElementType" property.
	protected override bool HasElementTypeImpl()
			{
				return typeImpl.HasElementType;
			}

	// Invoke a member.
	public override Object InvokeMember
						(String name, BindingFlags invokeAttr,
					     Binder binder, Object target, Object[] args,
					     ParameterModifier[] modifiers,
					     CultureInfo culture, String[] namedParameters)
			{
				return typeImpl.InvokeMember
					(name, invokeAttr, binder, target, args,
					 modifiers, culture, namedParameters);
			}

	// Implementation of the "IsByRef" property.
	protected override bool IsByRefImpl()
			{
				return typeImpl.IsByRef;
			}

	// Implementation of the "IsCOMObject" property.
	protected override bool IsCOMObjectImpl()
			{
				return typeImpl.IsCOMObject;
			}

	// Implement overridden properties.
	public override Guid GUID
			{
				get
				{
					return typeImpl.GUID;
				}
			}
	public override System.Reflection.Module Module
			{
				get
				{
					return typeImpl.Module;
				}
			}
	public override String Name
			{
				get
				{
					return typeImpl.Name;
				}
			}
	public override String Namespace
			{
				get
				{
					return typeImpl.Namespace;
				}
			}
	public override Type UnderlyingSystemType
			{
				get
				{
					return typeImpl.UnderlyingSystemType;
				}
			}

	// Internal methods that support generic types.

	protected override bool HasGenericArgumentsImpl()
			{
				return typeImpl.HasGenericArguments;
			}
	protected override bool HasGenericParametersImpl()
			{
				return typeImpl.HasGenericParameters;
			}
	public override Type[] GetGenericArguments()
			{
				return typeImpl.GetGenericArguments();
			}
	public override Type BindGenericParameters(Type[] inst)
			{
				return typeImpl.BindGenericParameters(inst);
			}
	public override Type GetGenericTypeDefinition()
			{
				return typeImpl.GetGenericTypeDefinition();
			}

}; // class TypeDelegator

#endif // CONFIG_REFLECTION && !ECMA_COMPAT

}; // namespace System.Reflection
