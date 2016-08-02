/*
 * TestEmit.cs - Test class "System.Reflection" invoke methods.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 * 
 * Authors : Thong Nguyen (tum@veridicus.com)
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

using CSUnit;
using System;
using System.IO;
using System.Reflection;

public class TestInvoke : TestCase
{
	public TestInvoke(String name)
		: base(name)
	{
	}

	// Set up for the tests.
	protected override void Setup()
	{
	}
	
	// Clean up after the tests.
	protected override void Cleanup()
	{
	}
	
	public struct Point
	{
		public int X;
		public int Y;
		
		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
		
		public override string ToString()
		{
			return String.Format("Point({0}, {1})", X, Y);
		}
	}

	public int FooWithInt(int x)
	{
		return x + 1;
	}

	public double FooWithDouble(double x)
	{
		return x + 1;
	}

	public Point FooWithPoint(Point p)
	{
		return new Point(p.X + 1, p.Y + 1);
	}
		
	public string FooWithString(string s)
	{
		return s.ToLower();
	}

	public int FooWithIntByRef(ref int x)
	{
		x++;
		
		return x;
	}
	
	public double FooWithDoubleByRef(ref double x)
	{
		x++;
		
		return x;
	}

	public Point FooWithPointByRef(ref Point p)
	{
		p.X++;
		p.Y++;
		
		return p;
	}
		
	public string FooWithStringByRef(ref string s)
	{
		s = s.ToLower();
		
		return s;
	}

	public void DoTest(string methodName, object arg, object expectedResult)
	{
		object retval;
		MethodInfo method;
		object[] args = new object[1];
		
		method = typeof(TestInvoke).GetMethod(methodName);

		args[0] = arg;		
		retval = method.Invoke(this, args);
		
		AssertEquals(String.Format("{0}=={1}", expectedResult, retval), expectedResult, retval);
	}
	
	public void TestInvokeWithInt()
	{
		DoTest("FooWithInt", 10, 11);
	}

	public void TestInvokeWithDouble()
	{
		DoTest("FooWithDouble", 10.31415926, 11.31415926);
	}

	public void TestInvokeWithPoint()
	{
		DoTest("FooWithPoint", new Point(10, 20), new Point(11, 21));
	}

	public void TestInvokeWithString()
	{
		DoTest("FooWithString", "POKWER", "pokwer");
	}

	public void DoTestByRef(string methodName, object arg, object expectedResult, bool refShouldEqual)
	{
		object retval;
		MethodInfo method;		
		object[] args = new object[1];

		method = typeof(TestInvoke).GetMethod(methodName);
		
		args[0] = arg;
		retval = method.Invoke(this, args);
		
		AssertEquals("Check result", expectedResult, retval);
		
		if (refShouldEqual)
		{
			Assert("Check byref param refs=(1)",
				Object.ReferenceEquals(arg, args[0]));
		}
		else
		{
			Assert("Check byref param refs=(2)",
				!Object.ReferenceEquals(arg, args[0]));
		}
		
		AssertEquals("Check byref param value", expectedResult, args[0]);
	}
	
	public void TestInvokeWithIntByRef()
	{
		DoTestByRef("FooWithIntByRef", 10, 11, true);
	}

	public void TestInvokeWithDoubleByRef()
	{
		DoTestByRef("FooWithDoubleByRef", 10.31415926, 11.31415926, true);
	}

	public void TestInvokeWithPointByRef()
	{
		DoTestByRef("FooWithPointByRef", new Point(10, 20), new Point(11, 21), true);
	}

	public void TestInvokeWithStringByRef()
	{
		DoTestByRef("FooWithStringByRef", "POKWER", "pokwer", false);
	}
	
	public int InvokeValueTypeParamAsNull(int x)
	{
		AssertEquals("x=0", 0, x);
		
		return x + 1;
	}
	
	public void TestInvokeValueTypeParamAsNull()
	{
		object result;
		object[] args = new object[1];
		MethodInfo method = typeof(TestInvoke).GetMethod("InvokeValueTypeParamAsNull");
		
		result = method.Invoke(this, args);
		
		AssertEquals("result=1", 1, result);
	}
	
	public int InvokeValueTypeRefParamAsNull(ref int x)
	{
		x++;
		
		return x;
	}
	
	public void TestInvokeValueTypeRefParamAsNull()
	{
		object result;
		object[] args = new object[1];
		MethodInfo method = typeof(TestInvoke).GetMethod("InvokeValueTypeRefParamAsNull");
		
		result = method.Invoke(this, args);
		
		AssertEquals("result=1", 1, result);
		AssertEquals("args[0]=1", 1, args[0]);
	}
}
