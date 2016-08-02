/*
 * FunctionConstructor.cs - Object that represents the "Function" constructor.
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

public sealed class FunctionConstructor : ScriptFunction
{
	// Constructor.
	internal FunctionConstructor() : base(null, "Function", 1) {}

	// Initialize this object, after all dependencies have been created.
	internal void Init(VsaEngine engine, ScriptObject parent)
			{
				// Set the prototype and engine.
				this.parent = parent;
				this.engine = engine;

				// Set the "prototype" property value.
				ScriptObject prototype =
					EngineInstance.GetEngineInstance(engine)
						.GetObjectPrototype();
				Put("prototype", prototype,
				    PropertyAttributes.ReadOnly |
					PropertyAttributes.DontEnum |
					PropertyAttributes.DontDelete);
			}

	// Construct a new "Function" instance.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public new ScriptFunction CreateInstance(params Object[] args)
			{
				return (ScriptFunction)Construct(engine, args);
			}

	// Invoke this constructor.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public ScriptFunction Invoke(params Object[] args)
			{
				return (ScriptFunction)Construct(engine, args);
			}

	// Perform a call on this object.
	internal override Object Call
				(VsaEngine engine, Object thisob, Object[] args)
			{
				return Construct(engine, args);
			}

	// Perform a constructor call on this object.
	internal override Object Construct(VsaEngine engine, Object[] args)
			{
				String parameters;
				String body;
				String defn;
				int index;
				JSParser parser;
				JFunction func;

				// Collect up the parameters and body.
				if(args.Length == 0)
				{
					parameters = String.Empty;
					body = String.Empty;
				}
				else if(args.Length == 1)
				{
					parameters = String.Empty;
					body = Convert.ToString(args[0]);
				}
				else
				{
					parameters = Convert.ToString(args[0]);
					for(index = 1; index < (args.Length - 1); ++index)
					{
						parameters =
							String.Concat(parameters, ",",
										  Convert.ToString(args[index]));
					}
					body = Convert.ToString(args[args.Length - 1]);
				}

				// Build a complete function definition and parse it.
				defn = "function (" + parameters + ") { " + body + " }";
				parser = new JSParser(new Context(defn));
				func = parser.ParseFunctionSource();

				// Build the function object and return it.
				return new FunctionObject
					(EngineInstance.GetEngineInstance(engine)
						.GetFunctionPrototype(), func,
					 engine.GetMainScope());
			}

}; // class FunctionConstructor

}; // namespace Microsoft.JScript
