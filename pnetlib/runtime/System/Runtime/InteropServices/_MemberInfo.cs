/*
 * _MemberInfo.cs - Implementation of the
 *			"System.Runtime.InteropServices._MemberInfo" class.
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

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

[CLSCompliant(false)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[Guid("f7102fa9-cabb-3a74-a6da-b4567ef1b079")]
public interface _MemberInfo
{
	bool Equals(Object obj);
	Object[] GetCustomAttributes(Type attributeType, bool inherit);
	Object[] GetCustomAttributes(bool inherit);
	int GetHashCode();
	Type GetType();
	bool IsDefined(Type attributeType, bool inherit);
	String ToString();
	Type DeclaringType { get; }
	MemberTypes MemberType { get; }
	String Name { get; }
	Type ReflectedType { get; }

}; // interface _MemberInfo

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
