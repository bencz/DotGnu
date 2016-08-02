/*
 * StringConstructor.cs - Object that represents the "String constructor.
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
using System.Text;
using Microsoft.JScript.Vsa;

public sealed class StringConstructor : ScriptFunction
{
	// Constructor.
	internal StringConstructor(FunctionPrototype parent)
			: base(parent, "String", 1) {}

	// Create a new string instance.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public new StringObject CreateInstance(params Object[] args)
			{
				return (StringObject)Construct(engine, args);
			}

	// Invoke the string constructor.
	public String Invoke(Object arg)
			{
				return Convert.ToString(arg);
			}

	// Build a string from an array of character codes.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs,
				JSBuiltin.String_fromCharCode)]
	public static String fromCharCode(params Object[] args)
			{
				StringBuilder builder = new StringBuilder();
				foreach(Object obj in args)
				{
					// TODO - implement Convert.ToChar
					//builder.Append(Convert.ToChar(obj));
				}
				return builder.ToString();
			}

	// Perform a call on this object.
	internal override Object Call
				(VsaEngine engine, Object thisob, Object[] args)
			{
				if(args.Length == 0)
				{
					return String.Empty;
				}
				else
				{
					return Convert.ToString(args[0]);
				}
			}

	// Perform a constructor call on this object.
	internal override Object Construct(VsaEngine engine, Object[] args)
			{
				if(args.Length == 0)
				{
					return new StringObject
						(EngineInstance.GetEngineInstance(engine)
								.GetStringPrototype(), String.Empty);
				}
				else
				{
					return new StringObject
						(EngineInstance.GetEngineInstance(engine)
								.GetStringPrototype(),
						 Convert.ToString(args[0]));
				}
			}

}; // class StringConstructor

}; // namespace Microsoft.JScript
