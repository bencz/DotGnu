/*
 * GlobalField.cs - JScript global variable, as a field.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Globalization;
using System.Reflection;

internal sealed class GlobalField : JSVariableField
{
	// Constructor.
	public GlobalField(FieldAttributes attributes, String name,
					   ScriptObject obj, Object value)
			: base(attributes, name, obj, value)
			{
				// Nothing to do here.
			}

	// Get the value of this field.
	public override Object GetValue(Object obj)
			{
				return value;
			}

	// Set the value of this field.
	public override void SetValue(Object obj, Object value,
								  BindingFlags invokeAttr, Binder binder,
								  CultureInfo culture)
			{
				bool isLiteral = ((Attributes & FieldAttributes.Literal) != 0);
				bool isInitOnly =
					((Attributes & FieldAttributes.InitOnly) != 0);
				if((isLiteral || isInitOnly) && !(this.value is Missing))
				{
					throw new JScriptException
						(JSError.AssignmentToReadOnly);
				}
				// TODO: typed fields
				this.value = value;
			}

}; // class GlobalField

}; // namespace Microsoft.JScript
