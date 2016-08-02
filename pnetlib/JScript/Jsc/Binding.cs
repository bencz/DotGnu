/*
 * Binding.cs - Handle binding operations.
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
using System.Reflection.Emit;

// Dummy class for backwards-compatibility.

public abstract class Binding : AST
{
	// Constructor.
	internal Binding() : base() {}

	// Determine if a value is "Missing".
	public static bool IsMissing(Object value)
			{
				return (value is Missing);
			}

	// Get the object for this binding.
	protected abstract Object GetObject();

	// Handle "no such member" errors.
	protected abstract void HandleNoSuchMemberError();

	// Resolve the right-hand part of a binding.
	protected void ResolveRHValue()
			{
				// Never used.
			}

#if CONFIG_REFLECTION_EMIT

	// Translate this binding object into IL.
	protected abstract void TranslateToILObject
			(ILGenerator il, Type obtype, bool noValue);
	protected abstract void TranslateToILWithDupOfThisOb(ILGenerator il);

#endif // CONFIG_REFLECTION_EMIT

}; // class Binding

}; // namespace Microsoft.JScript
