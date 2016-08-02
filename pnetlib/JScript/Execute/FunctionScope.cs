/*
 * FunctionScope.cs - Structure of an activation object for local
 *					  variables within a function activation.
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

internal sealed class FunctionScope : ActivationObject
{
	// Internal state.
	private Object thisObject;

	// Constructor.
	internal FunctionScope(ScriptObject parent, JFormalParams fparams,
						   Object thisObject, Object[] args)
			: base(parent, new JSObject())
			{
				this.thisObject = thisObject;
				if(fparams != null)
				{
					JExprListElem param = fparams.first;
					int posn = 0;
					while(param != null && posn < args.Length)
					{
						((IVariableAccess)this).SetVariable
							(Convert.ToString(param.name), args[posn++]);
						param = param.next;
					}
					while(param != null)
					{
						((IVariableAccess)this).SetVariable
							(Convert.ToString(param.name), null);
						param = param.next;
					}
				}
			}

	// Get the "this" object for this function scope.
	public override Object GetDefaultThisObject()
			{
				return thisObject;
			}

}; // class FunctionScope

}; // namespace Microsoft.JScript
