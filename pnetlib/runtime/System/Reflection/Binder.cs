/*
 * Binder.cs - Implementation of the "System.Reflection.Binder" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System;
using System.Globalization;
using System.Runtime.InteropServices;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#endif
public abstract class Binder
{

	// Constructor.
	protected Binder() {}

	// Bind a value to a field.
	public abstract FieldInfo BindToField(BindingFlags bindingAttr,
										  FieldInfo[] match,
										  Object value,
										  CultureInfo culture);

	// Bind a set of arguments to a method.
	public abstract MethodBase BindToMethod(BindingFlags bindingAttr,
											MethodBase[] match,
											ref Object[] args,
											ParameterModifier[] modifiers,
											CultureInfo culture,
											String[] names,
											ref Object state);

	// Convert an object from one type into another.
	public abstract Object ChangeType(Object value, Type type,
									  CultureInfo culture);

	// Re-order the argument array for a method call.
	public abstract void ReorderArgumentArray(ref Object[] args,
											  Object state);

	// Select a method based on argument types.
	public abstract MethodBase SelectMethod(BindingFlags bindingAttr,
											MethodBase[] match,
											Type[] types,
											ParameterModifier[] modifiers);

	// Select a property based on specified type criteria.
	public abstract PropertyInfo SelectProperty(BindingFlags bindingAttr,
												PropertyInfo[] match,
												Type returnType,
												Type[] indexes,
												ParameterModifier[] modifiers);

}; // class Binder

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
