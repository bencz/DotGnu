/*
 * MathObject.cs - The JScript Math object.
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

public class MathObject : JSObject
{
	// e, the base of natural algorithms
	public const double E		= 2.7182818284590452354;
	// the natural logarithm of 10
	public const double LN10	= 2.302585092994046;
	// the natural logarithm of 2
	public const double LN2		= 0.6931471805599453;
	// the base-2 logarithm of e
	public const double LOG2E	= 1.4426950408889634;
	// the base-10 logarithm of e
	public const double LOG10E	= 0.4342944819032518;
	// pi - the ratio of the circumference of a circle to its diameter
	public const double PI		= 3.14159265358979323846;
	// the square root of 1/2
	public const double SQRT1_2	= 0.7071067811865476;
	// the square root of 2
	public const double SQRT2	= 1.4142135623730951;
	
	// internal state
	private static Random rnd;
	
	// static constructor
	static MathObject()
	{
		rnd = new Random();
	}
	
	// internal constructor
	internal MathObject(ScriptObject parent)
		: base(parent)
	{
		EngineInstance inst = EngineInstance.GetEngineInstance(engine);
		
		Put("E", E,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("LN10", LN10,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("LN2", LN2,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("LOG2E", LOG2E,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("LOG10E", LOG10E,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		Put("PI", PI,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
			
		Put("SQRT1_2", SQRT1_2,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
			
		Put("SQRT2", SQRT2,
			PropertyAttributes.DontEnum|
			PropertyAttributes.DontDelete|
			PropertyAttributes.ReadOnly);
		
		AddBuiltin(inst, "abs");
		AddBuiltin(inst, "acos");
		AddBuiltin(inst, "asin");
		AddBuiltin(inst, "atan");
		AddBuiltin(inst, "atan2");
		AddBuiltin(inst, "ceil");
		AddBuiltin(inst, "exp");
		AddBuiltin(inst, "floor");
		AddBuiltin(inst, "log");
		AddBuiltin(inst, "max");
		AddBuiltin(inst, "min");
		AddBuiltin(inst, "pow");
		AddBuiltin(inst, "random");
		AddBuiltin(inst, "round");
		AddBuiltin(inst, "sin");
		AddBuiltin(inst, "sqrt");
		AddBuiltin(inst, "tan");
	}
	
	// compute the absolute value of an integer
	[JSFunctionAttribute(0, JSBuiltin.Math_abs)]
	public static Double abs(Double d)
	{
		return Math.Abs(d);
	}
	
	// calculate the arc cosine of x
	[JSFunctionAttribute(0, JSBuiltin.Math_acos)]
	public static Double acos(Double x)
	{
		return Math.Acos(x);
	}
	
	// calculate the arc sine of x
	[JSFunctionAttribute(0, JSBuiltin.Math_asin)]
	public static Double asin(Double x)
	{
		return Math.Asin(x);
	}
	
	// calculate the arc tangent of x
	[JSFunctionAttribute(0, JSBuiltin.Math_atan)]
	public static Double atan(Double x)
	{
		return Math.Atan(x);
	}
	
	// calculate the arc tangent of two values
	[JSFunctionAttribute(0, JSBuiltin.Math_atan2)]
	public static Double atan2(Double dy, Double dx)
	{
		return Math.Atan2(dy, dx);
	}
	
	// calculate the smallest integral value not less than x
	[JSFunctionAttribute(0, JSBuiltin.Math_ceil)]
	public static Double ceil(Double x)
	{
		return Math.Ceiling(x);
	}
	
	// calculate the cosin value of x
	[JSFunctionAttribute(0, JSBuiltin.Math_cos)]
	public static Double cos(Double x)
	{
		return Math.Cos(x);
	}
	
	// returns the value of E raised to the power of x
	[JSFunctionAttribute(0, JSBuiltin.Math_exp)]
	public static Double exp(Double x)
	{
		return Math.Exp(x);
	}
	
	// return the greatest integral value not greater than x
	[JSFunctionAttribute(0, JSBuiltin.Math_floor)]
	public static Double floor(Double x)
	{
		return Math.Floor(x);
	}
	
	// return the natural logarithm of x
	[JSFunctionAttribute(0, JSBuiltin.Math_log)]
	public static Double log(Double x)
	{
		return Math.Log(x);
	}
	
	// returns the greater value of two, or more, numbers
	[JSFunctionAttribute
		(JSFunctionAttributeEnum.HasVarArgs, JSBuiltin.Math_max)]
	public static Double max(params Object[] args)
	{
		Double dx, dy;
		
		if(args == null || args.Length < 1)
		{
			return Double.NegativeInfinity;
		}
		else
		{
			dx = args[0] is Double ? 
				(Double)args[0] : Convert.ToNumber(args[0]);
		}
		if(args.Length < 2)
		{
			return dx;
		}
		else
		{
			dy = dx = args[1] is Double ? 
				(Double)args[1] : Convert.ToNumber(args[1]);
		}
		
		Double max = dx.CompareTo(dy) < 0 ? dy : dx;
		
		if(args.Length > 2)
		{
			foreach(Object o in args)
			{
				max = MathObject.max(max, o);
			}
		}
		
		return max;
	}
	
	// returns the smaller value of two, or more, numbers
	[JSFunctionAttribute
		(JSFunctionAttributeEnum.HasVarArgs, JSBuiltin.Math_max)]
	public static Double min(params Object[] args)
	{
		Double dx, dy;
		
		if(args == null || args.Length < 1)
		{
			return Double.NegativeInfinity;
		}
		else
		{
			dx = args[0] is Double ? 
				(Double)args[0] : Convert.ToNumber(args[0]);
		}
		if(args.Length < 2)
		{
			return dx;
		}
		else
		{
			dy = dx = args[1] is Double ? 
				(Double)args[1] : Convert.ToNumber(args[1]);
		}
		
		Double min = dx.CompareTo(dy) < 0 ? dx : dy;
		
		if(args.Length > 2)
		{
			foreach(Object o in args)
			{
				min = MathObject.min(min, o);
			}
		}
		
		return min;
	}
	
	// returns the value of x raised to the power of y
	[JSFunctionAttribute(0, JSBuiltin.Math_pow)]
	public static Double pow(Double x, Double y)
	{
		return Math.Pow(x, y);
	}
	
	// returns a random number
	public static Double random()
	{	
		// paranoid
		if(rnd == null)
		{
			rnd = new Random();
		}
			
		return (Double)rnd.Next();
	}
	
	// round to nearest integer, away from zero
	[JSFunctionAttribute(0, JSBuiltin.Math_round)]
	public static Double round(Double d)
	{
		return Math.Round(d);
	}
	
	// returns the sine of x
	[JSFunctionAttribute(0, JSBuiltin.Math_sin)]
	public static Double sin(Double x)
	{
		return Math.Sin(x);
	}
	
	// returns the non-negative square root of x
	[JSFunctionAttribute(0, JSBuiltin.Math_sqrt)]
	public static Double sqrt(Double x)
	{
		return Math.Sqrt(x);
	}
	
	// returns the tangent of x
	[JSFunctionAttribute(0, JSBuiltin.Math_tan)]
	public static Double tan(Double x)
	{
		return Math.Tan(x);
	}
		
}; // class MathObject

public sealed class LenientMathObject : MathObject
{
	public new System.Object abs;
	public new System.Object acos;
	public new System.Object asin;
	public new System.Object atan;
	public new System.Object atan2;
	public new System.Object ceil;
	public new System.Object cos;
	public new System.Object exp;
	public new System.Object floor;
	public new System.Object log;
	public new System.Object max;
	public new System.Object min;
	public new System.Object pow;
	public new System.Object random;
	public new System.Object round;
	public new System.Object sin;
	public new System.Object sqrt;
	public new System.Object tan;
	
	internal LenientMathObject(ScriptObject parent, FunctionPrototype prototype)
		: base(parent)
	{
		EngineInstance inst = EngineInstance.GetEngineInstance(engine);
		
		abs = Get("abs");
		acos = Get("acos");
		asin = Get("asin");
		atan = Get("atan");
		atan2 = Get("atan2");
		ceil = Get("ceil");
		cos = Get("cos");
		exp = Get("exp");
		floor = Get("floor");
		log = Get("log");
		max = Get("max");
		min = Get("min");
		pow = Get("pow");
		random = Get("random");
		round = Get("round");
		sin = Get("sin");
		sqrt = Get("sqrt");
		tan = Get("tan");
	}

}; // class LenientMathObject

}; // namespace Microsoft.JScript
