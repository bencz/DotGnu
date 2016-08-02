/*
 * IActivationObject.cs - Activation object information for the JScript engine.
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
using System.Reflection;

public interface IActivationObject
{
	// Get the default "this" object for this activation.
	Object GetDefaultThisObject();

	// Find the global scope.  Never returns null.
	GlobalScope GetGlobalScope();

	// Look up a local field.
	FieldInfo GetLocalField(String name);

	// Get the value of a specific member in this activation.
	Object GetMemberValue(String name, int lexlevel);

	// Get the field for a specific member in this activation.
	FieldInfo GetField(String name, int lexlevel);

}; // interface IActivationObject

}; // namespace Microsoft.JScript
