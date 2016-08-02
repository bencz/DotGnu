/*
 * _Type.cs - Implementation of the
 *			"System.Runtime.InteropServices._Type" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

using System.Reflection;
using System.Globalization;

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

[CLSCompliant(false)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[Guid("BCA8B44D-AAD6-3A86-8AB7-03349F4F2DA2")]
public interface _Type
{
	bool Equals(Object obj);
	bool Equals(Type o);
	Type[] FindInterfaces(TypeFilter filter, Object filterCriteria);
	MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr,
							 MemberFilter filter, Object filterCriteria);
	int GetArrayRank();
	ConstructorInfo GetConstructor(BindingFlags bindingAttr,
								   Binder binder, Type[] types,
								   ParameterModifier[] modifiers);
	ConstructorInfo GetConstructor(Type[] types);
	ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder,
								   CallingConventions callingConventions,
								   Type[] types, ParameterModifier[] modifiers);
	ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);
	ConstructorInfo[] GetConstructors();
	Object[] GetCustomAttributes(Type attributeType, bool inherit);
	Object[] GetCustomAttributes(bool inherit);
	MemberInfo[] GetDefaultMembers();
	Type GetElementType();
	EventInfo GetEvent(String name, BindingFlags bindingAttr);
	EventInfo GetEvent(String name);
	EventInfo[] GetEvents(BindingFlags bindingAttr);
	EventInfo[] GetEvents();
	FieldInfo GetField(String name, BindingFlags bindingAttr);
	FieldInfo GetField(String name);
	FieldInfo[] GetFields(BindingFlags bindingAttr);
	FieldInfo[] GetFields();
	int GetHashCode();
	Type GetInterface(String name, bool ignoreCase);
	Type GetInterface(String name);
	InterfaceMapping GetInterfaceMap(Type interfaceType);
	Type[] GetInterfaces();
	MemberInfo[] GetMember(String name, BindingFlags bindingAttr);
	MemberInfo[] GetMember(String name);
	MemberInfo[] GetMember(String name, MemberTypes type,
						   BindingFlags bindingAttr);
	MemberInfo[] GetMembers(BindingFlags bindingAttr);
	MemberInfo[] GetMembers();
	MethodInfo GetMethod(String name, BindingFlags bindingAttr);
	MethodInfo GetMethod(String name, BindingFlags bindingAttr,
						 Binder binder, Type[] types,
						 ParameterModifier[] modifiers);
	MethodInfo GetMethod(String name, Type[] types);
	MethodInfo GetMethod(String name, Type[] types,
						 ParameterModifier[] modifiers);
	MethodInfo GetMethod(String name);
	MethodInfo GetMethod(String name, BindingFlags bindingAttr, Binder binder,
						 CallingConventions callingConventions, Type[] types,
						 ParameterModifier[] modifiers);
	MethodInfo[] GetMethods(BindingFlags bindingAttr);
	MethodInfo[] GetMethods();
	Type GetNestedType(String name, BindingFlags bindingAttr);
	Type GetNestedType(String name);
	Type[] GetNestedTypes(BindingFlags bindingAttr);
	Type[] GetNestedTypes();
	PropertyInfo GetProperty(String name);
	PropertyInfo GetProperty(String name, Type returnType);
	PropertyInfo GetProperty(String name, Type[] types);
	PropertyInfo GetProperty(String name, Type returnType, Type[] types);
	PropertyInfo GetProperty(String name, BindingFlags bindingAttr);
	PropertyInfo GetProperty(String name, Type returnType, Type[] types,
							 ParameterModifier[] modifiers);
	PropertyInfo GetProperty(String name, BindingFlags bindingAttr,
							 Binder binder, Type returnType, Type[] types,
							 ParameterModifier[] modifiers);
	PropertyInfo[] GetProperties(BindingFlags bindingAttr);
	PropertyInfo[] GetProperties();
	Type GetType();
	Object InvokeMember
				(String name, BindingFlags invokeAttr,
			     Binder binder, Object target, Object[] args,
			     ParameterModifier[] modifiers,
			     CultureInfo culture, String[] namedParameters);
	Object InvokeMember(String name, BindingFlags invokeAttr,
			     	    Binder binder, Object target, Object[] args);
	Object InvokeMember(String name, BindingFlags invokeAttr,
			     	    Binder binder, Object target, Object[] args,
					    CultureInfo culture);
	bool IsAssignableFrom(Type c);
	bool IsDefined(Type attributeType, bool inherit);
	bool IsInstanceOfType(Object obj);
	bool IsSubclassOf(Type c);
	String ToString();
	Assembly Assembly { get; }
	String AssemblyQualifiedName { get; }
	TypeAttributes Attributes { get; }
	Type BaseType { get; }
	Type DeclaringType { get; }
	String FullName { get; }
	Guid GUID { get; }
	bool HasElementType { get; }
	bool IsAbstract { get; }
	bool IsAnsiClass { get; }
	bool IsArray { get; }
	bool IsAutoClass { get; }
	bool IsAutoLayout { get; }
	bool IsByRef { get; }
	bool IsCOMObject { get; }
	bool IsClass { get; }
	bool IsContextful { get; }
	bool IsEnum { get; }
	bool IsExplicitLayout { get; }
	bool IsImport { get; }
	bool IsInterface { get; }
	bool IsLayoutSequential { get; }
	bool IsMarshalByRef { get; }
	bool IsNestedAssembly { get; }
	bool IsNestedFamANDAssem { get; }
	bool IsNestedFamORAssem { get; }
	bool IsNestedFamily { get; }
	bool IsNestedPrivate { get; }
	bool IsNestedPublic { get; }
	bool IsNotPublic { get; }
	bool IsPointer { get; }
	bool IsPrimitive { get; }
	bool IsSealed { get; }
	bool IsSerializable { get; }
	bool IsSpecialName { get; }
	bool IsUnicodeClass { get; }
	bool IsValueType { get; }
	MemberTypes MemberType { get; }
	Module Module { get; }
	String Name { get; }
	String Namespace { get; }
	Type ReflectedType { get; }
	RuntimeTypeHandle TypeHandle { get; }
	ConstructorInfo TypeInitializer { get; }
	Type UnderlyingSystemType { get; }

}; // interface _Type

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
