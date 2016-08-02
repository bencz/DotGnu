/*
 * NumberConstructor.cs - Object that represents the "Number constructor".
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

public sealed class NumberConstructor : ScriptFunction
{
	public Object MAX_VALUE;
	public Object MIN_VALUE;
	public Object NaN;
	public Object NEGATIVE_INFINITY;
	public Object POSITIVE_INFINITY;
	
	// Constructor.
	internal NumberConstructor(FunctionPrototype parent)
		: base(parent, "Number", 1)
	{
		Put("MAX_VALUE", Double.MaxValue,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("MIN_VALUE", Double.MinValue,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("NaN", Double.NaN,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("NEGATIVE_INFINITY", Double.NegativeInfinity,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("POSITIVE_INFINITY", Double.PositiveInfinity,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
			
		MAX_VALUE = Get("MAX_VALUE");
		MIN_VALUE = Get("MIN_VALUE");
		NaN = Get("NaN");
		NEGATIVE_INFINITY = Get("NEGATIVE_INFINITY");
		POSITIVE_INFINITY = Get("POSITIVE_INFINITY");
	}

	// Create a new string instance.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public new NumberObject CreateInstance(params Object[] args)
	{
		return (NumberObject)Construct(engine, args);
	}

	// Invoke the string constructor.
	public Double Invoke(Object arg)
	{
		return Convert.ToNumber(arg);
	}

	// Perform a call on this object.
	internal override Object Call
		(VsaEngine engine, Object thisob, Object[] args)
	{
		if(args.Length == 0)
		{
			return Double.NaN;
		}
		else
		{
			return Convert.ToNumber(args[0]);
		}
	}

	// Perform a constructor call on this object.
	internal override Object Construct(VsaEngine engine, Object[] args)
	{
		if(args.Length == 0)
		{
			return new NumberObject
				(EngineInstance.GetEngineInstance(engine)
						.GetNumberPrototype(), 0);
		}
		else
		{
			return new NumberObject
				(EngineInstance.GetEngineInstance(engine)
						.GetNumberPrototype(), args[0]);
		}
	}

}; // class NumberConstructor

}; // namespace Microsoft.JScript
