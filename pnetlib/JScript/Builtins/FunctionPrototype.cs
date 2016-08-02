/*
 * FunctionPrototype.cs - JScript function prototype objects.
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

public class FunctionPrototype : ScriptFunction
{
	// Constructor.
	internal FunctionPrototype() : base(null, "Function.prototype") {}

	// Initialize this object, after all dependencies have been created.
	internal virtual void Init(VsaEngine engine, ScriptObject parent)
			{
				// Set the prototype and engine.
				this.parent = parent;
				this.engine = engine;
				InitPrototype(parent);

				// Add the builtin "Function" properties to the prototype.
				EngineInstance inst = EngineInstance.GetEngineInstance(engine);
				Put("constructor", inst.GetFunctionConstructor());
				AddBuiltin(inst, "apply");
				AddBuiltin(inst, "call");
				AddBuiltin(inst, "toString");
			}

	// Get the "Function" class constructor.  Don't use this.
	public static FunctionConstructor constructor
			{
				get
				{
					return EngineInstance.Default.GetFunctionConstructor();
				}
			}

	// Implement the builtin "Function.apply" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Function_apply)]
	public static Object apply(Object thisob, Object thisarg, Object argArray)
			{
				// TODO
				return null;
			}

	// Implement the builtin "Function.call" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasVarArgs,
				JSBuiltin.Function_call)]
	public static Object call(Object thisob, Object thisarg,
							  params Object[] args)
			{
				// TODO
				return null;
			}

	// Implement the builtin "Function.toString" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Function_toString)]
	public static String toString(Object thisob)
			{
				if(thisob is ScriptFunction)
				{
					return thisob.ToString();
				}
				else
				{
					throw new JScriptException(JSError.FunctionExpected);
				}
			}

	// Perform a call on this object.
	internal override Object Call
				(VsaEngine engine, Object thisob, Object[] args)
			{
				// Not used on function prototypes.
				return null;
			}

}; // class FunctionPrototype

// "Lenient" version of the above class which exports all of the
// prototype's properties to the user level.
public class LenientFunctionPrototype : FunctionPrototype
{
	// Accessible properties.
	public new Object constructor;
	public new Object apply;
	public new Object call;
	public new Object toString;

	// Constructor.
	internal LenientFunctionPrototype() : base() {}

	// Initialize this object, after all dependencies have been created.
	internal override void Init(VsaEngine engine, ScriptObject parent)
			{
				base.Init(engine, parent);
				constructor = Get("constructor");
				apply = Get("apply");
				call = Get("call");
				toString = Get("toString");
			}

}; // class LenientFunctionPrototype

}; // namespace Microsoft.JScript
