/*
 * IVariableAccess.cs - Short-cut for variable declaration access.
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

internal interface IVariableAccess
{
	// Determine if this scope has a specific variable.
	bool HasVariable(String name);

	// Declare a specific variable in this scope and get its value.
	Object GetVariable(String name);

	// Declare a specific variable in this scope and set its value.
	void SetVariable(String name, Object value);

	// Declare a specific variable in this scope if it isn't already present.
	// If it is present, then its current value is left unmodified.
	void DeclareVariable(String name);

	// Get the parent variable scope.
	IVariableAccess GetParentScope();

}; // interface IVariableAccess

}; // namespace Microsoft.JScript
