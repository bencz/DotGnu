/*
 * _EventInfo.cs - Implementation of the
 *			"System.Runtime.InteropServices._EventInfo" class.
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
[Guid("9DE59C64-D889-35A1-B897-587D74469E5B")]
public interface _EventInfo
{
	void AddEventHandler(Object target, Delegate handler);
	bool Equals(Object obj);
	MethodInfo GetAddMethod();
	MethodInfo GetAddMethod(bool nonPublic);
	Object[] GetCustomAttributes(Type attributeType, bool inherit);
	Object[] GetCustomAttributes(bool inherit);
	int GetHashCode();
	MethodInfo GetRaiseMethod();
	MethodInfo GetRaiseMethod(bool nonPublic);
	MethodInfo GetRemoveMethod();
	MethodInfo GetRemoveMethod(bool nonPublic);
	Type GetType();
	bool IsDefined(Type attributeType, bool inherit);
	void RemoveEventHandler(Object target, Delegate handler);
	String ToString();
	EventAttributes Attributes { get; }
	Type DeclaringType { get; }
	Type EventHandlerType { get; }
	bool IsMulticast { get; }
	bool IsSpecialName { get; }
	MemberTypes MemberType { get; }
	String Name { get; }
	Type ReflectedType { get; }

}; // interface _EventInfo

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
