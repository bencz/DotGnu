/*
 * ObjectPrototype.cs - Common base for JScript prototype objects.
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

public class ObjectPrototype : JSObject
{
	// Constructor.
	internal ObjectPrototype() : base(null) {}

	// Initialize this object, after all dependencies have been created.
	internal virtual void Init(VsaEngine engine)
			{
				// Set the engine.
				this.engine = engine;

				// Add the builtin "Object" properties to the prototype.
				EngineInstance inst = EngineInstance.GetEngineInstance(engine);
				Put("constructor", inst.GetObjectConstructor());
				AddBuiltin(inst, "toString");
				AddBuiltin(inst, "toLocaleString");
				AddBuiltin(inst, "valueOf");
				AddBuiltin(inst, "hasOwnProperty");
				AddBuiltin(inst, "isPrototypeOf");
				AddBuiltin(inst, "propertyIsEnumerable");
			}

	// Get the "Object" class constructor.  Don't use this.
	public static ObjectConstructor constructor
			{
				get
				{
					return EngineInstance.Default.GetObjectConstructor();
				}
			}

	// Implement the builtin "Object.hasOwnProperty" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Object_hasOwnProperty)]
	public static bool hasOwnProperty(Object thisob, Object name)
			{
				String cname = Convert.ToString(name);
				if(thisob is ScriptObject)
				{
					return ((ScriptObject)thisob).HasOwnProperty(cname);
				}
				return false;
			}

	// Implement the builtin "Object.isPrototypeOf" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Object_isPrototypeOf)]
	public static bool isPrototypeOf(Object thisob, Object ob)
			{
				if(thisob is ScriptObject && ob is ScriptObject)
				{
					while(ob != null)
					{
						if(ob == thisob)
						{
							return true;
						}
						ob = ((ScriptObject)ob).GetParent();
					}
				}
				return false;
			}

	// Implement the builtin "Object.propertyIsEnumerable" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Object_propertyIsEnumerable)]
	public static bool propertyIsEnumerable(Object thisob, Object name)
			{
				String cname = Convert.ToString(name);
				if(thisob is JSObject)
				{
					return ((JSObject)thisob).IsEnumerable(cname);
				}
				return false;
			}

	// Implement the builtin "Object.toLocaleString" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Object_toLocaleString)]
	public static String toLocaleString(Object thisob)
			{
				// Let "JSObject.ToString" take care of redirecting
				// to the JScript "toString" method on the object.
				return thisob.ToString();
			}

	// Implement the builtin "Object.toString" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Object_toString)]
	public static String toString(Object thisob)
			{
				String className;
				if(thisob is JSObject)
				{
					className = ((JSObject)thisob).Class;
				}
				else
				{
					className = thisob.GetType().Name;
				}
				return "[object " + className + "]";
			}

	// Implement the builtin "Object.valueOf" function.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Object_valueOf)]
	public static Object valueOf(Object thisob)
			{
				return thisob;
			}

}; // class ObjectPrototype

// "Lenient" version of the above class which exports all of the
// prototype's properties to the user level.
public class LenientObjectPrototype : ObjectPrototype
{
	// Accessible properties.
	public new Object constructor;
	public new Object hasOwnProperty;
	public new Object isPrototypeOf;
	public new Object propertyIsEnumerable;
	public new Object toLocaleString;
	public new Object toString;
	public new Object valueOf;

	// Constructor.
	internal LenientObjectPrototype() : base() {}

	// Initialize this object, after all dependencies have been created.
	internal override void Init(VsaEngine engine)
			{
				base.Init(engine);
				constructor = Get("constructor");
				hasOwnProperty = Get("hasOwnProperty");
				isPrototypeOf = Get("isPrototypeOf");
				propertyIsEnumerable = Get("propertyIsEnumerable");
				toLocaleString = Get("toLocaleString");
				toString = Get("toString");
				valueOf = Get("valueOf");
			}

}; // class LenientObjectPrototype

}; // namespace Microsoft.JScript
