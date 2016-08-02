/*
 * TestDelegate.cs - Tests for the "Delegate" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * Contributors:  Thong Nguyen (tum@veridicus.com)
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
using System.Threading;

public class TestDelegate : TestCase
{
	// Constructor.
	public TestDelegate(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// Nothing to do here.
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	/*
	 * Test async calls with objects.
	 */
	private delegate object ObjectMethod(object x, object y);
	
	private object ObjectAdd(object x, object y)
	{
		return (int)x + (int)y;
	}
	
	public void TestAsyncCallWithObjects()
	{
		int result;
		IAsyncResult ar;
		ObjectMethod m = new ObjectMethod(ObjectAdd);
		
		ar = m.BeginInvoke(10, 20, null, null);
		
		result = (int)m.EndInvoke(ar);
		
		AssertEquals("result==30", 30, result);
	}

	/*
	 * Test async calls with objects byref.
	 */
	private delegate object ObjectByRefMethod(ref object x, ref object y);
	
	private object ObjectByRefAdd(ref object x, ref object y)
	{
		object tmp;
		
		tmp = x;
		x = y;
		y = tmp;
		
		return (int)x + (int)y;
	}
	
	public void TestAsyncCallWithObjectsByRef()
	{
		int result;
		object x, y, x2, y2;
		IAsyncResult ar;
		ObjectByRefMethod m = new ObjectByRefMethod(ObjectByRefAdd);
		
		x = 10;
		y = 20;
		x2 = x;
		y2 = y;
		
		ar = m.BeginInvoke(ref x, ref y, null, null);
		
		result = (int)m.EndInvoke(ref x, ref y, ar);
		
		Assert("x==y2", Object.ReferenceEquals(x, y2));
		Assert("y==x2", Object.ReferenceEquals(y, x2));
		AssertEquals("result==30", 30, result);
	}

	/*
	 * Test async calls with ints.
	 */
	private delegate int IntMethod(int x, int y);
	
	private int IntAdd(int x, int y)
	{
		return x + y;
	}

	public void TestAsyncCallWithInts()
	{
		int result;
		IAsyncResult ar;
		IntMethod m = new IntMethod(IntAdd);
		
		ar = m.BeginInvoke(10, 20, null, null);
		
		result = m.EndInvoke(ar);
		
		AssertEquals("result==30", 30, result);
	}

	/*
	 * Test async calls with ints byref.
	 */
	private delegate int IntByRefMethod(ref int x, ref int y);
	
	private int IntByRefAdd(ref int x, ref int y)
	{
		int tmp;
		
		tmp = x;
		x = y;
		y = tmp;
		
		return x + y;
	}

	public void TestAsyncCallWithIntsByRef()
	{
		int x, y, result;
		IAsyncResult ar;
		IntByRefMethod m = new IntByRefMethod(IntByRefAdd);
		
		x = 10;
		y = 20;
		ar = m.BeginInvoke(ref x, ref y, null, null);
		
		result = m.EndInvoke(ref x, ref y, ar);
		
		AssertEquals("x==20", 20, x);
		AssertEquals("y==10", 10, y);
		AssertEquals("result==30", 30, result);
	}
		
	/*
	 * Test async calls with double results.
	 */
	private delegate double DoubleMethod(double x, double y);
	
	private double DoubleAdd(double x, double y)
	{
		return x + y;
	}

	public void TestAsyncCallWithDoubles()
	{
		double result;
		IAsyncResult ar;
		DoubleMethod m = new DoubleMethod(DoubleAdd);
		
		ar = m.BeginInvoke(10, 20, null, null);
		
		result = m.EndInvoke(ar);
		
		AssertEquals("result==30", (double)30, result);
	}	

	/*
	 * Test async calls with doubles byref.
	 */
	private delegate double DoubleByRefMethod(ref double x, ref double y);
	
	private double DoubleByRefAdd(ref double x, ref double y)
	{
		double tmp;
		
		tmp = x;
		x = y;
		y = tmp;
		
		return x + y;
	}

	public void TestAsyncCallWithDoublesByRef()
	{
		double x, y, result;
		IAsyncResult ar;
		DoubleByRefMethod m = new DoubleByRefMethod(DoubleByRefAdd);
		
		x = 10;
		y = 20;
		
		ar = m.BeginInvoke(ref x, ref y, null, null);
		
		result = m.EndInvoke(ref x, ref y, ar);
		
		AssertEquals("x==20", (double)20, x);
		AssertEquals("y==10", (double)10, y);
		AssertEquals("result==30", (double)30, result);
	}	

	/*
	 * Test async calls with custom valuetype results.
	 */
	private delegate Point PointMethod(Point x, Point y);
	
	public struct Point
	{
		public int X;
		public int Y;
		
		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
		
	}
	
	private Point PointAdd(Point x, Point y)
	{
		return new Point(x.X + y.X, x.Y + y.Y);
	}

	public void TestAsyncCallWithValueTypes()
	{
		Point result;
		IAsyncResult ar;
		PointMethod m = new PointMethod(PointAdd);
		
		ar = m.BeginInvoke(new Point(10, 20), new Point(30, 40), null, null);
		
		result = m.EndInvoke(ar);
		
		AssertEquals("result==Point(40, 60)", new Point(40, 60), result);
	}	

	/*
	 * Test async calls with custom valuetypes byref.
	 */
	private delegate Point PointByRefMethod(ref Point x, ref Point y);
	
	private Point PointByRefAdd(ref Point x, ref Point y)
	{
		Point tmp;
		
		tmp = x;
		x = y;
		y = tmp;
		
		return new Point(x.X + y.X, x.Y + y.Y);
	}

	public void TestAsyncCallWithValueTypesByRef()
	{
		Point x, y, result;
		IAsyncResult ar;
		PointByRefMethod m = new PointByRefMethod(PointByRefAdd);
		
		x = new Point(10, 20);
		y = new Point(30, 40);
		
		ar = m.BeginInvoke(ref x, ref y, null, null);
		
		result = m.EndInvoke(ref x, ref y, ar);
		
		AssertEquals("x=(30,40)", new Point(30, 40), x);
		AssertEquals("y=(10,20)", new Point(10, 20), y);
		AssertEquals("result==Point(40, 60)", new Point(40, 60), result);
	}	
}; // class TestArray
