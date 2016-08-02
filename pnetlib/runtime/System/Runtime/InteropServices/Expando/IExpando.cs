/*
 * IExpando.cs - Implementation of the
 *			"System.Runtime.InteropServices.Expando.IExpando" class.
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

namespace System.Runtime.InteropServices.Expando
{

using System.Reflection;

#if !ECMA_COMPAT

[Guid("AFBF15E6-C37C-11d2-B88E-00A0C9B471B8")]
public interface IExpando : IReflect
{

	// Add a new field.
	FieldInfo AddField(String name);

	// Add a new method.
	MethodInfo AddMethod(String name, Delegate method);

	// Add a new property.
	PropertyInfo AddProperty(String name);

	// Remove a member.
	void RemoveMember(MemberInfo m);

}; // interface IExpando

#endif // !ECMA_COMPAT

}; // namespace System.Runtime.InteropServices.Expando
