/*
 * FunctionDeclaration.cs - Function declaration nodes.
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

// Dummy class for backwards-compatibility.

public sealed class FunctionDeclaration : AST
{
	// Constructor.
	internal FunctionDeclaration() : base() {}

#if false
	// Build a function declaration closure.
	public static Closure JScriptFunctionDeclaration
				(RuntimeTypeHandle handle, String name, String method_name,
				 String[] formal_parameters, JSLocalField[] fields,
				 bool must_save_stack_locals, bool hasArgumentsObject,
				 String text, Object declaringObject, VsaEngine engine)
			{
				// TODO
				return null;
			}
#endif

}; // class FunctionDeclaration

}; // namespace Microsoft.JScript
