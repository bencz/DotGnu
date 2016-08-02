/*
 * TestSuite.cs - Implementation of the "CSUnit.TestSuite" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

public class TestSuite : Test
{
	// Type list for locating constructors.
	private static Type[] ctorParamTypes = {typeof(System.String)};

	// Internal state.
	private String    name;
	private TestArray tests;

	// Simple constructor.
	public TestSuite(String name)
			{
				this.name = name;
				tests = new TestArray();
			}

	// A constructor that builds a list of tests from a type's interface.
	public TestSuite(String name, Type type)
			{
				this.name = name;
				tests = new TestArray();
				AddTests(type);
			}

	// Add a test to this suite.
	public void AddTest(Test test)
			{
				tests.Add(test);
			}

	// Add a group of tests to this suite that are defined by a type.
	// Note: this makes heavy use of reflection, so it is probably a
	// good idea to construct the reflection tests "by hand" so that we
	// know reflection works before relying upon it to work.
	public void AddTests(Type type)
			{
				// Make sure that the type inherits from "TestCase".
				if(!type.IsSubclassOf(typeof(TestCase)))
				{
					throw new ArgumentException
						("`type' does not inherit from `CSUnit.TestCase'");
				}

				// Find the 1-argument constructor that takes a string.
				ConstructorInfo ctor = type.GetConstructor(ctorParamTypes);
				if(ctor == null)
				{
					throw new ArgumentException
						("`type' does not have a (string) constructor");
				}

				// Scan the type for all public zero-parameter methods
				// that begin with the word "Test" or "test".  Create a
				// new test case object for each one.
				MethodInfo[] methods = type.GetMethods(BindingFlags.Public |
													   BindingFlags.Instance);
				String name;
				Object[] parameters = new Object [1];
				foreach(MethodInfo method in methods)
				{
					name = method.Name;
					if(name.StartsWith("Test") || name.StartsWith("test"))
					{
						parameters[0] = name;
						AddTest((Test)(ctor.Invoke(parameters)));
					}
				}
			}

	// Implement the "Test" interface.
	public void Run(TestResult result)
			{
				int posn;
				result.StartTest(this, true);
				for(posn = 0; posn < tests.Length; ++posn)
				{
					((Test)(tests[posn])).Run(result);
				}
				result.EndTest(this, true);
			}

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
					int posn;
					Test test;
					for(posn = 0; posn < tests.Length; ++posn)
					{
						test = ((Test)(tests[posn])).Find(name);
						if(test != null)
						{
							return test;
						}
					}
					return null;
				}
			}

	// List the tests to a TestResult object, but don't run them.
	// Implements the "Test" interface.
	public void List(TestResult result)
			{
				int posn;
				result.ListTest(this, true);
				for(posn = 0; posn < tests.Length; ++posn)
				{
					((Test)(tests[posn])).List(result);
				}
			}

}; // class TestSuite

}; // namespace CSUnit
