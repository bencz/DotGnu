/*
 * DefaultBinder.cs - Implementation of the
 *		"System.Private.DefaultBinder" class.
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

namespace System.Private
{

#if CONFIG_REFLECTION

using System;
using System.Reflection;
using System.Globalization;

internal class DefaultBinder : Binder
{

	// Constructor.
	public DefaultBinder() : base() {}

	// Bind a value to a field.
	[TODO]
	public override FieldInfo BindToField(BindingFlags bindingAttr,
										  FieldInfo[] match,
										  Object value,
										  CultureInfo culture)
			{
				// TODO
				return null;
			}

	// Bind a set of arguments to a method.
	[TODO]
	public override MethodBase BindToMethod(BindingFlags bindingAttr,
											MethodBase[] match,
											ref Object[] args,
											ParameterModifier[] modifiers,
											CultureInfo culture,
											String[] names,
											ref Object state)
			{
				// TODO
				return null;
			}

	// Convert an object from one type into another.
	[TODO]
	public override Object ChangeType(Object value, Type type,
									  CultureInfo culture)
			{
				// TODO
				return null;
			}

	// Re-order the argument array for a method call.
	[TODO]
	public override void ReorderArgumentArray(ref Object[] args,
											  Object state)
			{
				// TODO
			}

	// Select a method based on argument types.
	[TODO]
	public override MethodBase SelectMethod(BindingFlags bindingAttr,
											MethodBase[] match,
											Type[] types,
											ParameterModifier[] modifiers)
			{
				// TODO
				return null;
			}

	// Select a property based on specified type criteria.
	[TODO]
	public override PropertyInfo SelectProperty(BindingFlags bindingAttr,
												PropertyInfo[] match,
												Type returnType,
												Type[] indexes,
												ParameterModifier[] modifiers)
			{
				// TODO
				return null;
			}

}; // class DefaultBinder

#endif // CONFIG_REFLECTION

}; // namespace System.Private
