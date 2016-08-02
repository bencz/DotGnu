/*
 * BlockScope.cs - Structure of an activation object for a simple block.
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
using System.Collections;
using System.Reflection;

public class BlockScope : ActivationObject
{
	// Constructor.
	internal BlockScope(ScriptObject parent, ScriptObject storage)
			: base(parent, storage)
			{
				// Nothing to do here.
			}

	// Create a new field within this scope.
	protected override JSVariableField CreateField
				(String name, FieldAttributes attributes, Object value)
			{
				return base.CreateField(name, attributes, value);
			}

}; // class BlockScope

}; // namespace Microsoft.JScript
