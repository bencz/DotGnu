/*
 * Globals.cs - Global context for the JScript engine.
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
using Microsoft.JScript.Vsa;

public sealed class Globals
{

	// Current engine context.
	public static VsaEngine contextEngine = null;

	// Construct an array object.  Don't use - not re-entrant safe.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public static ArrayObject ConstructArray(params Object[] args)
			{
				return (ArrayObject)
					(ArrayPrototype.constructor.Construct
						(GetContextEngine(), args));
			}

	// Construct an array literal.  Don't use - not re-entrant safe.
	public static ArrayObject ConstructArrayLiteral(Object[] args)
			{
				return (ArrayObject)
					(ArrayPrototype.constructor.ConstructArray(args));
			}

	// Get the global context engine.  Use this in preference
	// to directly accessing the "contextEngine" variable.
	internal static VsaEngine GetContextEngine()
			{
				// Lock on "VsaEngine" to synchronize with "CreateEngine()".
				lock(typeof(VsaEngine))
				{
					if(contextEngine == null)
					{
						contextEngine = VsaEngine.MakeNewEngine();
					}
					return contextEngine;
				}
			}

	// Set the global context engine.
	internal static void SetContextEngine(VsaEngine engine)
			{
				// Lock on "VsaEngine" to synchronize with "CreateEngine()".
				lock(typeof(VsaEngine))
				{
					if(contextEngine == null)
					{
						contextEngine = engine;
					}
				}
			}

}; // class Globals

}; // namespace Microsoft.JScript
