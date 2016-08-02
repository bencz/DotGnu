/*
 * TestCase.cs - Implementation of the "CSUnit.TestCase" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

namespace CSUnit
{

using System;
using System.Reflection;

public abstract class TestCase : Test
{

	// Internal state.
	private String name;

	// Constructor.
	public TestCase(String name)
			{
				this.name = name;
			}

	// Run the test case, and pass its results to a given object.
	// This also implements the "Test" interface.
	public void Run(TestResult result)
			{
				result.StartTest(this, false);
				Setup();
				try
				{
					RunTest();
				}
				catch(TestAssertFailed assert)
				{
					result.AddFailure(this, assert);
				}
				catch(TestStop)
				{
					// Something wants us to stop testing immediately.
					throw;
				}
				catch(Exception e)
				{
					result.AddException(this, e);
				}
				finally
				{
					Cleanup();
				}
				result.EndTest(this, false);
			}

	// Run a single test case and return its results.
	public TestResult Run()
			{
				TestResult result = CreateResult();
				Run(result);
				return result;
			}

	// Create a result structure for this test case.
	public virtual TestResult CreateResult()
			{
				return new TestResult();
			}

	// Set up the test case.
	protected abstract void Setup();

	// Run the test case.
	protected virtual void RunTest()
			{
				// The default implementation looks for a method within
				// this object's type that is the same as the name of the
				// test case.  Because this uses reflection, it is a good
				// idea to test the reflection system without relying
				// upon this method to work correctly.
				MethodInfo method = GetType().GetMethod
					(name, BindingFlags.Public | BindingFlags.Instance,
					 null, Type.EmptyTypes, null);
				if(method == null)
				{
					throw new TestAssertFailed
						(GetType().FullName +
						 " does not have a zero-parameter method called " +
						 name);
				}
				method.Invoke(this, null);
			}

	// Clean up after running the test case.
	protected abstract void Cleanup();

	// Get the name of this test.  Implements the "Test" interface.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Find a test by name.  Implements the "Test" interface.
	public Test Find(String name)
			{
				if(name == this.name)
				{
					return this;
				}
				else
				{
					return null;
				}
			}

	// List the tests to a TestResult object, but don't run them.
	// Implements the "Test" interface.
	public void List(TestResult result)
			{
				result.ListTest(this, false);
			}

	// Fail the current test with a message.
	public static void Fail()
			{
				throw new TestAssertFailed();
			}
	public static void Fail(String msg)
			{
				if(msg != null)
				{
					throw new TestAssertFailed(msg);
				}
				else
				{
					throw new TestAssertFailed();
				}
			}

	// Test an assertion.
	public static void Assert(bool condition)
			{
				if(!condition)
				{
					Fail(null);
				}
			}
	public static void Assert(String msg, bool condition)
			{
				if(!condition)
				{
					Fail(msg);
				}
			}

	// Report an equality failure.
	private static void EqFail(String msg, Object expected, Object actual)
			{
				String temp;
				if(msg != null)
				{
					temp = msg + " ";
				}
				else
				{
					temp = "";
				}
				if(expected == null)
				{
					expected = "null";
				}
				if(actual == null)
				{
					actual = "null";
				}
				Fail(temp + "expected: <" + expected.ToString() +
					 ">, but was: <" + actual.ToString() + ">");
			}

	// Test for various equality conditions.
	public static void AssertEquals(long expected, long actual)
			{
				if(expected != actual)
				{
					EqFail(null, expected, actual);
				}
			}
	public static void AssertEquals(ulong expected, ulong actual)
			{
				if(expected != actual)
				{
					EqFail(null, expected, actual);
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	public static void AssertEquals(double expected, double actual,
								    double delta)
			{
				AssertEquals(null, expected, actual, delta);
			}
	public static void AssertEquals(float expected, float actual,
								    float delta)
			{
				AssertEquals(null, expected, actual, delta);
			}
#endif
	public static void AssertEquals(String expected, String actual)
			{
				if(expected != actual)
				{
					EqFail(null, expected, actual);
				}
			}
	public static void AssertEquals(Object expected, Object actual)
			{
				if(expected == null && actual == null)
				{
					return;
				}
				else if(expected != null && expected.Equals(actual))
				{
					return;
				}
				else
				{
					EqFail(null, expected, actual);
				}
			}

	// Test for various equality conditions, with messages.
	public static void AssertEquals(String msg, long expected, long actual)
			{
				if(expected != actual)
				{
					EqFail(msg, expected, actual);
				}
			}
	public static void AssertEquals(String msg, ulong expected, ulong actual)
			{
				if(expected != actual)
				{
					EqFail(msg, expected, actual);
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	public static void AssertEquals(String msg, double expected,
									double actual, double delta)
			{
				if(Math.Abs(expected - actual) > delta)
				{
					EqFail(msg, expected, actual);
				}
			}
	public static void AssertEquals(String msg, float expected,
									float actual, float delta)
			{
				if(Math.Abs(expected - actual) > delta)
				{
					EqFail(msg, expected, actual);
				}
			}
#endif
	public static void AssertEquals(String msg, String expected, String actual)
			{
				if(expected != actual)
				{
					EqFail(msg, expected, actual);
				}
			}
	public static void AssertEquals(String msg, Object expected, Object actual)
			{
				if(expected == null && actual == null)
				{
					return;
				}
				else if(expected != null && expected.Equals(actual))
				{
					return;
				}
				else
				{
					EqFail(msg, expected, actual);
				}
			}

	// Assert than an object reference must be null.
	public static void AssertNull(Object value)
			{
				if(value != null)
				{
					EqFail(null, null, value);
				}
			}

	// Assert than an object reference must be null, with messages.
	public static void AssertNull(String msg, Object value)
			{
				if(value != null)
				{
					EqFail(msg, null, value);
				}
			}

	// Assert than an object reference must not be null.
	public static void AssertNotNull(Object value)
			{
				if(value == null)
				{
					EqFail(null, "not null", value);
				}
			}

	// Assert than an object reference must not be null, with messages.
	public static void AssertNotNull(String msg, Object value)
			{
				if(value == null)
				{
					EqFail(msg, "not null", value);
				}
			}

	// Assert that two objects are the same reference.
	public static void AssertSame(Object valuea, Object valueb)
			{
				if(valuea != valueb)
				{
					Fail();
				}
			}

	// Assert that two objects are not the same reference.
	public static void AssertNotSame(Object valuea, Object valueb)
			{
				if(valuea == valueb)
				{
					Fail();
				}
			}

	// Assert that two objects are the same reference, with messages.
	public static void AssertSame(String msg, Object valuea, Object valueb)
			{
				if(valuea != valueb)
				{
					Fail(msg);
				}
			}

	// Assert that two objects are not the same reference, with messages.
	public static void AssertNotSame(String msg, Object valuea, Object valueb)
			{
				if(valuea == valueb)
				{
					Fail(msg);
				}
			}

}; // class TestCase

}; // namespace CSUnit
