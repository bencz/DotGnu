/*
 * Eval.cs - Evaluate a JScript statement that is provided by string.
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

// Dummy class for backwards-compatibility.

public sealed class Eval : AST
{
	// Constructor.
	internal Eval() : base() {}

	// Evaluate a JScript expression in the context of a specific engine.
	public static Object JScriptEvaluate(Object source, VsaEngine engine)
			{
				Object value = null;

				// Bail out if we weren't supplied a string.
				if(!(source is String))
				{
					return source;
				}

				// Parse the "eval" statement.
				Context context = new Context((String)source);
				context.codebase = new CodeBase("eval code", null);
				JSParser parser = new JSParser(context);
				JNode node = parser.ParseSource(true);

				// Push a scope for use during evaluation.
				engine.PushScriptObject
					(new BlockScope(engine.ScriptObjectStackTop(),
									new JSObject (null, engine)));

				// Evaluate the statement.
				try
				{
					value = node.Eval(engine);
					if(value == Empty.Value)
					{
						value = null;
					}
				}
				catch(JScriptException e)
				{
					// Attach the context information to low-level exceptions.
					if(e.context == null)
					{
						e.context = context;
					}
					throw;
				}
				catch(BreakJumpOut brk)
				{
					// "break" used incorrectly.
					throw new JScriptException(JSError.BadBreak, brk.context);
				}
				catch(ContinueJumpOut cont)
				{
					// "continue" used incorrectly.
					throw new JScriptException(JSError.BadContinue,
											   cont.context);
				}
				catch(ReturnJumpOut ret)
				{
					// "return" used incorrectly.
					throw new JScriptException(JSError.BadReturn, ret.context);
				}
				finally
				{
					// Pop the script scope.
					engine.PopScriptObject();
				}

				// Return the result of the evaluation to the caller.
				return value;
			}

}; // class Eval

}; // namespace Microsoft.JScript
