/*
 * BuiltinFunction.cs - Wrapper for builtin functions.
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

internal sealed class BuiltinFunction : ScriptFunction
{
	// Internal state.
	private MethodInfo method;
	private JSFunctionAttributeEnum flags;
	private int requiredParameters;

	// Constructor.
	public BuiltinFunction(ScriptObject prototype, String name,
						   MethodInfo method)
			: base(prototype, name)
			{
				this.method = method;
				Object[] attrs = method.GetCustomAttributes
						(typeof(JSFunctionAttribute), false);
				if(attrs == null || attrs.Length == 0)
				{
					this.flags = (JSFunctionAttributeEnum)0;
				}
				else
				{
					this.flags = ((JSFunctionAttribute)(attrs[0]))
							.GetAttributeValue();
				}
				requiredParameters = method.GetParameters().Length;
				lengthValue = requiredParameters;
				if((flags & JSFunctionAttributeEnum.HasThisObject) != 0)
				{
					--lengthValue;
				}
				if((flags & JSFunctionAttributeEnum.HasEngine) != 0)
				{
					--lengthValue;
				}
				if((flags & JSFunctionAttributeEnum.HasVarArgs) != 0)
				{
					--lengthValue;
				}
			}

	// Perform a call on this object.
	internal override Object Call
					(VsaEngine engine, Object thisob, Object[] args)
			{
				// Invoke the builtin method using reflection.
				if((flags & (JSFunctionAttributeEnum.HasThisObject |
							 JSFunctionAttributeEnum.HasVarArgs |
							 JSFunctionAttributeEnum.HasEngine)) ==
						(JSFunctionAttributeEnum)0 &&
				   requiredParameters == args.Length)
				{
					return method.Invoke(null, args);
				}
				else
				{
					Object[] tempArgs = new Object [requiredParameters];
					int posn = 0;
					int req = requiredParameters;
					Object[] rest;
					if((flags & JSFunctionAttributeEnum.HasThisObject) != 0)
					{
						tempArgs[posn++] = thisob;
						--req;
					}
					if((flags & JSFunctionAttributeEnum.HasEngine) != 0)
					{
						tempArgs[posn++] = engine;
						--req;
					}
					if((flags & JSFunctionAttributeEnum.HasVarArgs) != 0)
					{
						if(req <= 1)
						{
							tempArgs[posn] = args;
						}
						else
						{
							--req;
							if(args.Length <= req)
							{
								Array.Copy(args, 0, tempArgs, posn,
										   args.Length);
								rest = new Object [0];
							}
							else
							{
								Array.Copy(args, 0, tempArgs, posn, req);
								rest = new Object [args.Length - req];
								Array.Copy(args, req, rest, 0, rest.Length);
							}
							tempArgs[tempArgs.Length - 1] = rest;
						}
					}
					else if(args.Length <= req)
					{
						Array.Copy(args, 0, tempArgs, posn, args.Length);
					}
					else
					{
						Array.Copy(args, 0, tempArgs, posn, req);
					}
					return method.Invoke(null, tempArgs);
				}
			}

}; // class BuiltinFunction

}; // namespace Microsoft.JScript
