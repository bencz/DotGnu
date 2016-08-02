/*
 * _PropertyInfo.cs - Implementation of the
 *			"System.Runtime.InteropServices._PropertyInfo" class.
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
[Guid("F59ED4E4-E68F-3218-BD77-061AA82824BF")]
public interface _PropertyInfo
{
	bool Equals(Object obj);
	MethodInfo[] GetAccessors();
	MethodInfo[] GetAccessors(bool nonPublic);
	Object[] GetCustomAttributes(Type attributeType, bool inherit);
	Object[] GetCustomAttributes(bool inherit);
	MethodInfo GetGetMethod();
	MethodInfo GetGetMethod(bool nonPublic);
	int GetHashCode();
	ParameterInfo[] GetIndexParameters();
	MethodInfo GetSetMethod();
	MethodInfo GetSetMethod(bool nonPublic);
	Type GetType();
	Object GetValue(Object obj, Object[] index);
	Object GetValue(Object obj, BindingFlags invokeAttr,
					Binder binder, Object[] index, CultureInfo culture);
	bool IsDefined(Type attributeType, bool inherit);
	void SetValue(Object obj, Object value, Object[] index);
	void SetValue(Object obj, Object value, BindingFlags invokeAttr,
				  Binder binder, Object[] index, CultureInfo culture);
	String ToString();
	PropertyAttributes Attributes { get; }
	bool CanRead { get; }
	bool CanWrite { get; }
	Type DeclaringType { get; }
	bool IsSpecialName { get; }
	MemberTypes MemberType { get; }
	String Name { get; }
	Type PropertyType { get; }
	Type ReflectedType { get; }

}; // interface _PropertyInfo

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
