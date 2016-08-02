/*
 * TestArgIterator.cs - Tests for the "ArgIterator" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

public class TestArgIterator : TestCase
{
	// Constructor.
	public TestArgIterator(String name)
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

#if false
#if !ECMA_COMPAT

	// Helper method for "TestArgIteratorGetType".
	private void TestTypes(String testNum, Type[] types, __arglist)
			{
				ArgIterator iter = new ArgIterator(__arglist);
				int count = iter.GetRemainingCount();
				AssertEquals("Length " + testNum, types.Length, count);
				while(count > 0)
				{
					Type type = Type.GetTypeFromHandle
						(iter.GetNextArgType());
					AssertEquals("TypeCheck " + testNum,
								 types[types.Length - count], type);
					AssertEquals("Remaining " + testNum,
								 count, iter.GetRemainingCount());
					iter.GetNextArg();
					--count;
				}
				try
				{
					iter.GetNextArgType();
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());
			}

	// Test getting the type of the arguments to a vararg method.
	public void TestArgIteratorGetType()
			{
				TestTypes("(0)", new Type [0], __arglist());
				TestTypes("(1)", new Type [] {typeof(int)},
						  __arglist(1));
				TestTypes("(2)", new Type [] {typeof(int), typeof(String)},
						  __arglist(1, "hello"));
				TestTypes("(3)", new Type [] {typeof(int), typeof(String),
											  typeof(Object)},
						  __arglist(1, "hello", null));
				TestTypes("(4)", new Type [] {typeof(int), typeof(String),
											  typeof(Object), typeof(TypeCode)},
						  __arglist(1, "hello", null, TypeCode.DBNull));
			}

	// Helper method for "TestArgIteratorGetValue".
	private void TestValues(String testNum, Object[] values, __arglist)
			{
				Object expected, actual;
				ArgIterator iter = new ArgIterator(__arglist);
				int count = iter.GetRemainingCount();
				AssertEquals("Length " + testNum, values.Length, count);
				while(count > 0)
				{
					expected = values[values.Length - count];
					actual = TypedReference.ToObject(iter.GetNextArg());
					if(expected == null)
					{
						AssertNull("ValueCheck " + testNum, actual);
					}
					else
					{
						Assert("ValueCheck " + testNum,
							   expected.Equals(actual));
					}
					--count;
					AssertEquals("Remaining " + testNum,
								 count, iter.GetRemainingCount());
				}
				try
				{
					iter.GetNextArg();
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());

				// Restart and run the test again to make sure that
				// the first iteration did not modify the vararg values.
				iter = new ArgIterator(__arglist);
				count = iter.GetRemainingCount();
				AssertEquals("Length " + testNum, values.Length, count);
				while(count > 0)
				{
					expected = values[values.Length - count];
					actual = TypedReference.ToObject(iter.GetNextArg());
					if(expected == null)
					{
						AssertNull("ValueCheck " + testNum, actual);
					}
					else
					{
						Assert("ValueCheck " + testNum,
							   expected.Equals(actual));
					}
					--count;
					AssertEquals("Remaining " + testNum,
								 count, iter.GetRemainingCount());
				}
				try
				{
					iter.GetNextArg();
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());
			}

	// Test getting the values of the arguments to a vararg method.
	public void TestArgIteratorGetValue()
			{
				TestValues("(0)", new Object [0], __arglist());
				TestValues("(1)", new Object [] {1}, __arglist(1));
				TestValues("(2)", new Object [] {1, "hello"},
						  __arglist(1, "hello"));
				TestValues("(3)", new Object [] {1, "hello", null},
						  __arglist(1, "hello", null));
				TestValues("(4)", new Object []
								{1, "hello", null, TypeCode.DBNull},
						  __arglist(1, "hello", null, TypeCode.DBNull));
			}

	// Helper method for "TestArgIteratorGetValueStatic".
	private static void TestValuesStatic
				(String testNum, Object[] values, __arglist)
			{
				Object expected, actual;
				ArgIterator iter = new ArgIterator(__arglist);
				int count = iter.GetRemainingCount();
				AssertEquals("Length " + testNum, values.Length, count);
				while(count > 0)
				{
					expected = values[values.Length - count];
					actual = TypedReference.ToObject(iter.GetNextArg());
					if(expected == null)
					{
						AssertNull("ValueCheck " + testNum, actual);
					}
					else
					{
						Assert("ValueCheck " + testNum,
							   expected.Equals(actual));
					}
					--count;
					AssertEquals("Remaining " + testNum,
								 count, iter.GetRemainingCount());
				}
				try
				{
					iter.GetNextArg();
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());

				// Restart and run the test again to make sure that
				// the first iteration did not modify the vararg values.
				iter = new ArgIterator(__arglist);
				count = iter.GetRemainingCount();
				AssertEquals("Length " + testNum, values.Length, count);
				while(count > 0)
				{
					expected = values[values.Length - count];
					actual = TypedReference.ToObject(iter.GetNextArg());
					if(expected == null)
					{
						AssertNull("ValueCheck " + testNum, actual);
					}
					else
					{
						Assert("ValueCheck " + testNum,
							   expected.Equals(actual));
					}
					--count;
					AssertEquals("Remaining " + testNum,
								 count, iter.GetRemainingCount());
				}
				try
				{
					iter.GetNextArg();
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());
			}

	// Test getting the values of the arguments to a vararg method.
	public void TestArgIteratorGetValueStatic()
			{
				TestValuesStatic("(0)", new Object [0], __arglist());
				TestValuesStatic("(1)", new Object [] {1}, __arglist(1));
				TestValuesStatic("(2)", new Object [] {1, "hello"},
						  __arglist(1, "hello"));
				TestValuesStatic("(3)", new Object [] {1, "hello", null},
						  __arglist(1, "hello", null));
				TestValuesStatic("(4)", new Object []
								{1, "hello", null, TypeCode.DBNull},
						  __arglist(1, "hello", null, TypeCode.DBNull));
			}

	// Helper method for "TestArgIteratorGetValueByType".
	private void TestByType(String testNum, Type[] types, __arglist)
			{
				ArgIterator iter = new ArgIterator(__arglist);
				int count = iter.GetRemainingCount();
				AssertEquals("Length " + testNum, types.Length, count);
				while(count > 0)
				{
					Type type = types[types.Length - count];
					Object value = TypedReference.ToObject
						(iter.GetNextArg(type.TypeHandle));
					Type valueType = value.GetType();
					AssertEquals("TypeCheck " + testNum, type, valueType);
					--count;
					AssertEquals("Remaining " + testNum,
								 count, iter.GetRemainingCount());
				}
				try
				{
					iter.GetNextArg(typeof(double).TypeHandle);
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());

				// Reset the list and check for the wrong type.
				iter = new ArgIterator(__arglist);
				try
				{
					iter.GetNextArg(typeof(long).TypeHandle);
					Fail("TypeCheck 2 " + testNum);
				}
				catch(InvalidOperationException)
				{
					// This is what we expected.
				}
			}

	// Test getting the values of the arguments to a vararg method,
	// using the type to extract the values.  Also check for a little
	// "fuzziness" in the type, which is allowed by "GetNextArg(type)".
	public void TestArgIteratorGetValueByType()
			{
				TestByType("(0)", new Type [0], __arglist());
				TestByType("(1)", new Type [] {typeof(int)},
						   __arglist(1));
				TestByType("(2)", new Type [] {typeof(int), typeof(char)},
						   __arglist(1, 'x'));
				TestByType("(3)", new Type [] {typeof(int), typeof(char),
											  typeof(IntPtr)},
						   __arglist(1, 'x', IntPtr.Zero));
				TestByType("(4)", new Type [] {typeof(int), typeof(char),
											  typeof(UIntPtr), typeof(int)},
						   __arglist(1, 'x', IntPtr.Zero, TypeCode.DBNull));
			}

	// Helper method for "TestArgIteratorEnd".
	private void TestEnd(String testNum, __arglist)
			{
				ArgIterator iter = new ArgIterator(__arglist);
				iter.End();
				AssertEquals("Remaining " + testNum, 0,
							 iter.GetRemainingCount());
				try
				{
					iter.GetNextArg();
					Fail("EndCheck " + testNum);
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
			}

	// Test moving to the end of a vararg list.
	public void TestArgIteratorEnd()
			{
				TestEnd("(0)", __arglist());
				TestEnd("(1)", __arglist(1));
				TestEnd("(2)", __arglist(1, "hello"));
				TestEnd("(3)", __arglist(1, "hello", null));
				TestEnd("(4)", __arglist(1, "hello", null, TypeCode.DBNull));
			}

	// Check that we cannot do anything with an uninitialized ArgIterator.
	private ArgIterator emptyIterator;
	public void TestArgIteratorUninitialized()
			{
				AssertEquals("Remaining", 0,
							 emptyIterator.GetRemainingCount());
				try
				{
					emptyIterator.GetNextArg();
					Fail("EndCheck");
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				try
				{
					emptyIterator.GetNextArgType();
					Fail("EndCheck 2");
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
				try
				{
					emptyIterator.GetNextArg(typeof(int).TypeHandle);
					Fail("EndCheck 3");
				}
				catch(InvalidOperationException)
				{
					// We expect this exception at the end of the list.
				}
			}

#endif // !ECMA_COMPAT
#endif

}; // class TestArgIterator
