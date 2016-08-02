/*
 * MemberInfo.cs - Implementation of the "System.Reflection.MemberInfo" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if CONFIG_REFLECTION

using System.Runtime.InteropServices;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_MemberInfo))]
#endif
#endif
public abstract class MemberInfo : ICustomAttributeProvider
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _MemberInfo
#endif
{

	// Constructor.
	protected MemberInfo() : base() {}

	// Get the type that declares this member.
	public abstract Type DeclaringType { get; }

	// Get the name of this member.
	public abstract String Name { get; }

	// Get the reflected type that was used to locate this member.
	public abstract Type ReflectedType { get; }

	// Note: the following methods are not ECMA-compatible, but so
	// many other ECMA facilities depend upon them that it is a pain
	// to #if these definitions in all of the inheriting classes.
	//
	// If you are writing ECMA-compatible code, you should use
	// "obj is TypeName" instead of "MemberType", and the methods
	// in "System.Attribute" instead of "GetCustomAttributes" and
	// "IsDefined".

	// Get the type of member that this is.
	public abstract MemberTypes MemberType { get; }

	// Get the custom attributes that are associated with this member.
	public abstract Object[] GetCustomAttributes(bool inherit);
	public abstract Object[] GetCustomAttributes(Type type, bool inherit);

	// Determine if custom attributes are defined for this member.
	public abstract bool IsDefined(Type type, bool inherit);

}; // class MemberInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
