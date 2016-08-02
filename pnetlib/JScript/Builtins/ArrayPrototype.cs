/*
 * ArrayPrototype.cs - Prototype object for "Array" objects.
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

public class ArrayPrototype : ArrayObject
{
	// Constructor.
	internal ArrayPrototype(ObjectPrototype parent, EngineInstance inst)
			: base(parent)
			{
				// Add the builtin "Array" properties to the prototype.
				Put("constructor", inst.GetArrayConstructor());
				AddBuiltin(inst, "concat");
				AddBuiltin(inst, "join");
				AddBuiltin(inst, "pop");
				AddBuiltin(inst, "push");
				AddBuiltin(inst, "reverse");
				AddBuiltin(inst, "shift");
				AddBuiltin(inst, "slice");
				AddBuiltin(inst, "sort");
				AddBuiltin(inst, "splice");
				AddBuiltin(inst, "toLocaleString");
				AddBuiltin(inst, "toString");
				AddBuiltin(inst, "unshift");
			}

	// Get the "Array" class constructor.  Don't use this.
	public static ArrayConstructor constructor
			{
				get
				{
					return EngineInstance.Default.GetArrayConstructor();
				}
			}

	// Concatenate arrays together.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasVarArgs |
				JSFunctionAttributeEnum.HasEngine, JSBuiltin.Array_concat)]
	public static ArrayObject concat(Object thisob, VsaEngine engine,
									 params Object[] args)
			{
				// TODO
				return null;
			}

	// Join the elements together into a string.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject, JSBuiltin.Array_join)]
	public static String join(Object thisob, Object separator)
			{
				String sep;
				StringBuilder builder;
				if(separator is Missing)
				{
					sep = ",";
				}
				else
				{
					sep = Convert.ToString(separator);
				}
				builder = new StringBuilder();
				// TODO
				return builder.ToString();
			}

	// Pop an element from the end of an array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject, JSBuiltin.Array_pop)]
	public static Object pop(Object thisob)
			{
				// TODO
				return thisob;
			}

	// Push elements onto the end of an array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasVarArgs, JSBuiltin.Array_push)]
	public static long push(Object thisob, params Object[] args)
			{
				// TODO
				return 0;
			}

	// Reverse the elements in an array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Array_reverse)]
	public static Object reverse(Object thisob)
			{
				// TODO
				return thisob;
			}

	// Shift the elements in an array down one, returning the first.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject, JSBuiltin.Array_shift)]
	public static Object shift(Object thisob)
			{
				// TODO
				return null;
			}

	// Slice a sub-array out of an array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasEngine, JSBuiltin.Array_slice)]
	public static ArrayObject slice(Object thisob, VsaEngine engine,
								    double start, Object end)
			{
				// TODO
				return null;
			}

	// Sort the contents of an array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject, JSBuiltin.Array_sort)]
	public static Object sort(Object thisob, Object function)
			{
				// TODO
				return thisob;
			}

	// Split an array into the middle of another array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasVarArgs |
				JSFunctionAttributeEnum.HasEngine, JSBuiltin.Array_splice)]
	public static ArrayObject splice(Object thisob, VsaEngine engine,
									 double start, double deleteCnt,
									 params Object[] args)
			{
				// TODO
				return null;
			}

	// Convert the contents of an array into a locale-based string.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Array_toLocaleString)]
	public static String toLocaleString(Object thisob)
			{
				// TODO
				return null;
			}

	// Convert the contents of an array into an invariant string.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.Array_toString)]
	public static String toString(Object thisob)
			{
				if(thisob is ArrayObject)
				{
					return join(thisob, ",");
				}
				else
				{
					throw new JScriptException(JSError.NeedArrayObject);
				}
			}

	// Unshift elements back into an array.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasEngine, JSBuiltin.Array_unshift)]
	public static Object unshift(Object thisob, params Object[] args)
			{
				// TODO
				return null;
			}

}; // class ArrayPrototype

// "Lenient" version of the above class which exports all of the
// prototype's properties to the user level.
public class LenientArrayPrototype : ArrayPrototype
{
	// Accessible properties.
	public new Object constructor;
	public new Object concat;
	public new Object join;
	public new Object pop;
	public new Object push;
	public new Object reverse;
	public new Object shift;
	public new Object slice;
	public new Object sort;
	public new Object splice;
	public new Object toLocaleString;
	public new Object toString;
	public new Object unshift;

	// Constructor.
	internal LenientArrayPrototype(ObjectPrototype parent, EngineInstance inst)
			: base(parent, inst)
			{
				constructor = inst.GetArrayConstructor();
				concat = Get("concat");
				join = Get("join");
				pop = Get("pop");
				push = Get("push");
				reverse = Get("reverse");
				shift = Get("shift");
				slice = Get("slice");
				sort = Get("sort");
				splice = Get("splice");
				toLocaleString = Get("toLocaleString");
				toString = Get("toString");
				unshift = Get("unshift");
			}

}; // class LenientArrayPrototype

}; // namespace Microsoft.JScript
