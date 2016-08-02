/*
 * FunctionObject.cs - Class that represents "Function" objects.
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
using System.Globalization;
using Microsoft.JScript.Vsa;

public sealed class FunctionObject : ScriptFunction
{
	// Internal state.
	private JFunction defn;
	private ScriptObject declaringScope;

	// Constructor.
	internal FunctionObject(FunctionPrototype parent, JFunction defn,
							ScriptObject declaringScope)
			: base(parent, defn.name, Support.ExprListLength(defn.fparams))
			{
				this.defn = defn;
				this.declaringScope = declaringScope;
			}

	// Convert this function object into a string.
	public override String ToString()
			{
				return defn.context.GetCode();
			}

	// Perform a call on this object.
	internal override Object Call
				(VsaEngine engine, Object thisob, Object[] args)
			{
				// Create a new scope object and initialize the parameters.
				ScriptObject scope;
				if(thisob is JSObject)
				{
					scope = new FunctionScope
						((JSObject)thisob, defn.fparams, thisob, args);
				}
				else
				{
					scope = new FunctionScope
						(declaringScope, defn.fparams, thisob, args);
				}

				// Push the scope onto the stack.
				engine.PushScriptObjectChecked(scope);

				// Call the function and pop the scope afterwards.
				Object result = Empty.Value;
				try
				{
					if(defn.body != null)
					{
						defn.body.Eval(engine);
					}
				}
				catch(ReturnJumpOut r)
				{
					return r.value;
				}
				finally
				{
					engine.PopScriptObject();
				}

				// Return the function result to the caller.
				return result;
			}

}; // class FunctionObject

}; // namespace Microsoft.JScript
