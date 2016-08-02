/*
 * _FieldInfo.cs - Implementation of the
 *			"System.Runtime.InteropServices._FieldInfo" class.
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
[Guid("8A7C1442-A9FB-366B-80D8-4939FFA6DBE0")]
public interface _FieldInfo
{
	bool Equals(Object obj);
	Object[] GetCustomAttributes(Type attributeType, bool inherit);
	Object[] GetCustomAttributes(bool inherit);
	int GetHashCode();
	Type GetType();
	Object GetValue(Object obj);
	Object GetValueDirect(TypedReference obj);
	bool IsDefined(Type attributeType, bool inherit);
	String ToString();
	void SetValue(Object obj, Object value);
	void SetValue(Object obj, Object value, BindingFlags invokeAttr,
				  Binder binder, CultureInfo culture);
	void SetValueDirect(TypedReference obj, Object value);
	FieldAttributes Attributes { get; }
	Type DeclaringType { get; }
	RuntimeFieldHandle FieldHandle { get; }
	bool IsAssembly { get; }
	bool IsFamily { get; }
	bool IsFamilyAndAssembly { get; }
	bool IsFamilyOrAssembly { get; }
	bool IsInitOnly { get; }
	bool IsNotSerialized { get; }
	bool IsPinvokeImpl { get; }
	bool IsPrivate { get; }
	bool IsPublic { get; }
	bool IsSpecialName { get; }
	bool IsStatic { get; }
	MemberTypes MemberType { get; }
	String Name { get; }
	Type ReflectedType { get; }

}; // interface _FieldInfo

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
