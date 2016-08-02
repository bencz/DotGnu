/*
 * _MethodInfo.cs - Implementation of the
 *			"System.Runtime.InteropServices._MethodInfo" class.
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
[Guid("FFCC1B5D-ECB8-38DD-9B01-3DC8ABC2AA5F")]
public interface _MethodInfo
{
	bool Equals(Object obj);
	MethodInfo GetBaseDefinition();
	Object[] GetCustomAttributes(Type attributeType, bool inherit);
	Object[] GetCustomAttributes(bool inherit);
	int GetHashCode();
	MethodImplAttributes GetMethodImplementationFlags();
	ParameterInfo[] GetParameters();
	Type GetType();
	Object Invoke(Object obj, BindingFlags invokeAttr, Binder binder,
				  Object[] parameters, CultureInfo culture);
	Object Invoke(Object obj, Object[] parameters);
	bool IsDefined(Type attributeType, bool inherit);
	String ToString();
	MethodAttributes Attributes { get; }
	CallingConventions CallingConvention { get; }
	Type DeclaringType { get; }
	bool IsAbstract { get; }
	bool IsAssembly { get; }
	bool IsConstructor { get; }
	bool IsFamily { get; }
	bool IsFamilyAndAssembly { get; }
	bool IsFamilyOrAssembly { get; }
	bool IsFinal { get; }
	bool IsHideBySig { get; }
	bool IsPrivate { get; }
	bool IsPublic { get; }
	bool IsSpecialName { get; }
	bool IsStatic { get; }
	bool IsVirtual { get; }
	MemberTypes MemberType { get; }
	RuntimeMethodHandle MethodHandle { get; }
	String Name { get; }
	Type ReflectedType { get; }
	Type ReturnType { get; }
	ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

}; // interface _MethodInfo

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
