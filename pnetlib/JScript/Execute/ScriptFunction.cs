/*
 * ScriptFunction.cs - Object that represents a script function.
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

public abstract class ScriptFunction : JSObject
{
	// Internal state.
	private String name;
	internal int lengthValue;
	private Object prototypeValue;

	// Constructors.
	protected ScriptFunction(ScriptObject parent, String name)
			: base(parent)
			{
				this.name = name;
				this.lengthValue = length;
				if(parent != null)
				{
					this.prototypeValue =
						new JSPrototypeObject(parent.GetParent(), this);
				}
				else
				{
					this.prototypeValue = Missing.Value;
				}
			}
	internal ScriptFunction(ScriptObject parent, String name, int numParams)
			: base(parent)
			{
				this.name = name;
				this.lengthValue = numParams;
				if(parent != null)
				{
					this.prototypeValue =
						new JSPrototypeObject(parent.GetParent(), this);
				}
				else
				{
					this.prototypeValue = Missing.Value;
				}
			}

	// Initialize the prototype value from a subclass.
	internal void InitPrototype(ScriptObject parent)
			{
				this.prototypeValue =
					new JSPrototypeObject(parent.GetParent(), this);
			}

	// Create an instance of this function.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public Object CreateInstance(params Object[] args)
			{
				return Construct(engine, args);
			}

	// Get the prototype for a constructed function object.
	protected ScriptObject GetPrototypeForConstructedObject()
			{
				return (ScriptObject)prototypeValue;
			}

	// Invoke this script function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasVarArgs)]
	public Object Invoke(Object thisob, params Object[] args)
			{
				return Call(engine, thisob, args);
			}

	// Invoke a member object.
	public override Object InvokeMember
					   (String name, BindingFlags invokeAttr,
						Binder binder, Object target, Object[] args,
						ParameterModifier[] modifiers,
						CultureInfo culture, String[] namedParameters)
			{
				// Bail out if the wrong object was used to invoke.
				if(target != this)
				{
					throw new TargetException();
				}

				// TODO: find the member and invoke it.
				return null;
			}

	// Get or set the length of the script function argument list.
	public virtual int length
			{
				get
				{
					return lengthValue;
				}
				set
				{
					// Nothing to do here.
				}
			}

	// Get or set the prototype that underlies this script function.
	public Object prototype
			{
				get
				{
					return prototypeValue;
				}
				set
				{
					prototypeValue = value;
				}
			}

	// Convert this script function into a string.
	public override String ToString()
			{
				return "function " + name + "() {\n" +
					   "    [native code]\n" +
					   "}";
			}

	// Perform a call on this object.
	internal abstract Object Call
				(VsaEngine engine, Object thisob, Object[] args);

	// Perform a constructor call on this object.
	internal virtual Object Construct(VsaEngine engine, Object[] args)
			{
				JSObject obj = new JSObject(GetPrototypeForConstructedObject());
				Object result = Call(engine, obj, args);
				if(result is ScriptObject)
				{
					return result;
				}
				else
				{
					return obj;
				}
			}

	// Determine if an object is an instance of this class.
	internal virtual bool HasInstance(Object obj)
			{
				// TODO
				return false;
			}

	// Get the internal "[[Class]]" property for this object.
	internal override String Class
			{
				get
				{
					return "Function";
				}
			}

}; // class ScriptFunction

}; // namespace Microsoft.JScript
