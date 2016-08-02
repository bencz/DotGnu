/*
 * TestNameTable.cs - Tests for the "System.Xml.NameTable" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Xml;

public class TestNameTable : TestCase
{
	// Constructor.
	public TestNameTable(String name)
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

	// Test adding strings and checking that object equality
	// works as string equality on the results.
	public void TestNameTableAdd()
			{
				NameTable table = new NameTable();
				String value, value2, result;

				// Add an initial string, which should be added directly.
				value = "Hello";
				result = table.Add(value);
				if(!ReferenceEquals(value, result))
				{
					Fail("initial add");
				}

				// Create a string that has the same contents as "value",
				// but which will not have the same object reference.
				value2 = String.Concat("Hel", "lo");
				if(ReferenceEquals(value, value2))
				{
					Fail("concat construction failed - runtime engine error");
				}

				// Look up the initial string and validate it.
				if(!ReferenceEquals(value, table.Get(value)))
				{
					Fail("lookup initial did not give the initial string");
				}
				if(!ReferenceEquals(value, table.Get(value2)))
				{
					Fail("lookup initial on same contents gave wrong result");
				}

				// Add another string and validate against the first.
				value2 = table.Add("Goodbye");
				if(ReferenceEquals(value2, value))
				{
					Fail("Hello == Goodbye!");
				}
				if(!ReferenceEquals(value, table.Get(value)))
				{
					Fail("initial string changed after adding another string");
				}
				if(!ReferenceEquals(value2, table.Get("Goodbye")))
				{
					Fail("second string could not be found on lookup");
				}

				// Check that the empty string is added as "String.Empty".
				if(!ReferenceEquals(String.Empty, table.Add("")))
				{
					Fail("empty string not added as String.Empty");
				}

				// Add and get strings using an array.
				char[] array = new char [10];
				array[3] = 'H';
				array[4] = 'i';
				value2 = table.Add(array, 3, 2);
				if(!ReferenceEquals(value2, table.Get("Hi")))
				{
					Fail("array add on Hi failed");
				}
				if(!ReferenceEquals(value2, table.Get(array, 3, 2)))
				{
					Fail("array get on Hi failed");
				}
				array[3] = 'H';
				array[4] = 'e';
				array[5] = 'l';
				array[6] = 'l';
				array[7] = 'o';
				value2 = table.Add(array, 3, 5);
				if(!ReferenceEquals(value, value2))
				{
					Fail("array add on Hello gave incorrect value");
				}
				if(!ReferenceEquals(value, table.Get(array, 3, 5)))
				{
					Fail("array add on Hello gave incorrect value");
				}
				if(!ReferenceEquals(String.Empty, table.Add(array, 10, 0)))
				{
					Fail("array add on \"\" gave incorrect value");
				}
			}

	// Test the exceptions that may be thrown by various methods.
	public void TestNameTableExceptions()
			{
				NameTable table = new NameTable();
				char[] array = new char [10];

				try
				{
					table.Add(null);
					Fail("Add(null) should throw an exception");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					table.Get(null);
					Fail("Get(null) should throw an exception");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					table.Add(null, 0, 0);
					Fail("Add(null, 0, 0) should throw an exception");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					table.Get(null, 0, 0);
					Fail("Get(null, 0, 0) should throw an exception");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					table.Add(array, 0, -1);
					Fail("Add(array, 0, -1) should throw an exception");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success
				}

				try
				{
					table.Get(array, 0, -1);
					Fail("Get(array, 0, -1) should throw an exception");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success
				}

				try
				{
					table.Add(array, -1, 3);
					Fail("Add(array, -1, 3) should throw an exception");
				}
				catch(IndexOutOfRangeException)
				{
					// Success
				}

				try
				{
					table.Get(array, -1, 3);
					Fail("Get(array, -1, 3) should throw an exception");
				}
				catch(IndexOutOfRangeException)
				{
					// Success
				}

				try
				{
					table.Add(array, 0, 11);
					Fail("Add(array, 0, 11) should throw an exception");
				}
				catch(IndexOutOfRangeException)
				{
					// Success
				}

				try
				{
					table.Get(array, 0, 11);
					Fail("Get(array, 0, 11) should throw an exception");
				}
				catch(IndexOutOfRangeException)
				{
					// Success
				}
			}

}; // class TestNameTable
