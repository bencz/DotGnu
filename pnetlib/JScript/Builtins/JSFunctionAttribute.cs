/*
 * JSFunctionAttribute.cs - Mark a method with JScript-specific attributes.
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

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public class JSFunctionAttribute : Attribute
{
	// Internal state.
	private JSFunctionAttributeEnum value;
	private JSBuiltin builtinFunction;

	// Constructors.
	public JSFunctionAttribute(JSFunctionAttributeEnum value)
			{
				this.value = value;
				this.builtinFunction = (JSBuiltin)0;
			}
	public JSFunctionAttribute
				(JSFunctionAttributeEnum value, JSBuiltin builtinFunction)
			{
				this.value = value;
				this.builtinFunction = builtinFunction;
			}

	// Get the attribute's value.
	public JSFunctionAttributeEnum GetAttributeValue()
			{
				return value;
			}

}; // class Expando

}; // namespace Microsoft.JScript
