/*
 * NumberObject.cs - The JScript Number object.
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
using System.Globalization;

public class NumberPrototype : NumberObject
{	
	private const String ndigits = "012456789ABCDEFGHIJKLMNOPQRSTUVWXZ";
	
	// internal constructor
	internal NumberPrototype(ScriptObject parent, Object value)
		: base(parent, value)
	{
		EngineInstance inst = EngineInstance.GetEngineInstance(engine);
		
		Put("constructor", constructor);

#if false
		AddBuiltin(inst, "toExponential");
		AddBuiltin(inst, "toFixed");
		AddBuiltin(inst, "toLocaleString");
		AddBuiltin(inst, "toPrecision");
		AddBuiltin(inst, "toString");
#endif 
		AddBuiltin(inst, "valueOf");
	}
	
#if false	
	// return a exponent string representation
	[JSFunctionAttribute(JSFunctionAttributeEnum.HasThisObject,
		JSBuiltin.Number_toExponential)]
	public static String toExponential(Object thisob, Object fractionDigits)
	{
		// TODO
		return valueOf(thisob).ToString();
	}
	
	// returns a fixed-point string representation
	[JSFunctionAttribute(JSFunctionAttributeEnum.HasThisObject,
		JSBuiltin.Number_toFixed)]
	public static String toFixed(Object thisob, Double n)
	{
		// TODO
		return valueOf(thisob).ToString();
	}
	
	// calculate the arc sine of x
	[JSFunctionAttribute(JSFunctionAttributeEnum.HasThisObject,
		JSBuiltin.Number_toLocaleString)]
	public static String toLocaleString(Object thisob)
	{
		// TODO
		return valueOf(thisob).ToString();
	}
	
	// calculate the arc tangent of x
	[JSFunctionAttribute(JSFunctionAttributeEnum.HasThisObject,
		JSBuiltin.Number_toPrecision)]
	public static String toPrecision(Object thisob, Object precision)
	{
		// TODO
		return valueOf(thisob).ToString();
	}
	
	// calculate the arc tangent of two values
	[JSFunctionAttribute(JSFunctionAttributeEnum.HasThisObject,
		JSBuiltin.Number_toString)]
	public static String toString(Object thisob, Object radix)
	{
		// TODO	
		return valueOf(thisob).ToString();
	}
#endif
	
	// calculate the smallest integral value not less than x
	[JSFunctionAttribute(JSFunctionAttributeEnum.HasThisObject,
		JSBuiltin.Number_valueOf)]
	public static Object valueOf(Object thisob)
	{
		NumberObject numbob = thisob as NumberObject;
		if(numbob != null)
		{
			return numbob.value;
		}
		
		switch(Type.GetTypeCode(thisob.GetType()))
		{
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64: 
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
			return thisob;
		default:
			throw new JScriptException(JSError.TypeMismatch);
		}
	}
	
	public static NumberConstructor constructor
	{
		get
		{
			return EngineInstance.Default.GetNumberConstructor();
		}
	}
		
}; // class NumberPrototype

// "Lenient" version of the above class which exports all of the
// prototype's properties to the user level.
public class LenientNumberPrototype : NumberPrototype
{
	// Accessible properties.
	public new Object constructor;
#if false
	public new Object toExponential;
	public new Object toFixed;
	public new Object toLocaleString;
	public new Object toPrecision;
	public new Object toString;
#endif
	public new Object valueOf;

	// Constructor.
	internal LenientNumberPrototype(ScriptObject prototype, Object value)
		: base(prototype, value)
	{
		constructor = Get("constructor");
#if false
		toExponential = Get("toExponential");
		toFixed = Get("toFixed");
		toLocaleString = Get("toLocaleString");
		toPrecision = Get("toPrecision");
		toString = Get("toString");
#endif
		valueOf = Get("valueOf");
	}
	
	internal LenientNumberPrototype(ScriptObject parent)
		: this(parent, 0)
	{
		// nothing to do here
	}

}; // class LenientNumberPrototype

}; // namespace Microsoft.JScript
