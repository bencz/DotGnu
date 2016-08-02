/*
 * TestArrayList.cs - Tests for the "Boolean" class.
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

using CSUnit;
using System;
using System.Collections;

public class TestArrayList : TestCase
{
	// Constructor.
	public TestArrayList(String name)
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

#if !ECMA_COMPAT
	// check the creation of an ArrayList from a multidimensional Array
	// this is not specified in ECMA but fails for MS
	public void TestArrayListCtor1()
			{
				Char[,] charArray = new Char[2,2];
				try
				{
					ArrayList arrayList = new ArrayList(charArray);
					Fail("Should have thrown a RankException");
				}
				catch(RankException e)
				{
				}
			}
#endif // !ECMA_COMPAT

	// Test insertion into an array list.
	public void TestArrayListInsert()
			{
				int posn;
				ArrayList list = new ArrayList();
		
				for(posn = 0; posn < 100; ++posn)
				{
					AssertEquals(list.Count, posn);
					list.Insert(posn / 2, posn.ToString());
					AssertEquals(list.Count, posn + 1);
					AssertEquals(((String)(list[posn / 2])), posn.ToString());
				}
			}

	// Test adding to an array list.
	public void TestArrayListAdd()
			{
				int posn;
				ArrayList list = new ArrayList();
		
				for(posn = 0; posn < 100; ++posn)
				{
					AssertEquals(list.Count, posn);
					list.Add(posn.ToString());
					AssertEquals(list.Count, posn + 1);
					AssertEquals(((String)(list[posn])), posn.ToString());
				}
			}

	// Test clearing an array list.
	public void TestArrayListClear()
			{
				ArrayList list = new ArrayList();
				int posn;

				// Clear an empty list.
				AssertEquals(list.Count, 0);
				list.Clear();
				AssertEquals(list.Count, 0);

				// Clear a list with 1 element.
				list.Add("element");
				AssertEquals(list.Count, 1);
				list.Clear();
				AssertEquals(list.Count, 0);

				// Clear a list with 10 elements.
				for(posn = 0; posn < 10; ++posn)
				{
					list.Add(posn);
				}
				AssertEquals(list.Count, 10);
				list.Clear();
				AssertEquals(list.Count, 0);

				// Attempt to clear a read-only list.
				list = ArrayList.ReadOnly(list);
				try
				{
					list.Clear();

					// We should never get here!
					Fail();
				}
				catch(NotSupportedException)
				{
					// The test was successfull if we get here.
				}
			}

	public void TestArrayListCopyToNull()
			{
				Char[] c1 = new Char[2];
				ArrayList al1 = new ArrayList(c1);
				try
				{
					al1.CopyTo(null, 2);
					Fail("Should have thrown an ArgumentNullException");
				}
				catch (ArgumentNullException)
				{
				}
			}

	public void TestArrayListCopyToWrongArrayType()
			{
				String[] stringArray = {"String", "array"};
				Char[] charArray = new Char[2];
				ArrayList arrayList = new ArrayList(stringArray);
				try
				{
					arrayList.CopyTo(charArray, 0);
					Fail("Should have thrown an InvalidCastException");
				}
				catch(InvalidCastException e)
				{
				}
			}

	public void TestArrayListCopyToWrongRank()
			{
				Char[] charArray1 = new Char[2];
				Char[,] charArray2 = new Char[2,2];
				ArrayList arrayList = new ArrayList(charArray1);
				try
				{
					arrayList.CopyTo(charArray2, 2);
					Fail("Should have thrown an ArgumentException");
				}
				catch(ArgumentException e)
				{
				}
			}

	public void TestArrayListCopyTo()
			{
				Char[] orig = {'a', 'b', 'c', 'd'};
				ArrayList al = new ArrayList(orig);
				Char[] copy = new Char[10];
				Array.Clear(copy, 0, copy.Length);
				al.CopyTo(copy, 3);
				AssertEquals("Wrong CopyTo 0", (char)0, copy[0]);
				AssertEquals("Wrong CopyTo 1", (char)0, copy[1]);
				AssertEquals("Wrong CopyTo 2", (char)0, copy[2]);
				AssertEquals("Wrong CopyTo 3", orig[0], copy[3]);
				AssertEquals("Wrong CopyTo 4", orig[1], copy[4]);
				AssertEquals("Wrong CopyTo 5", orig[2], copy[5]);
				AssertEquals("Wrong CopyTo 6", orig[3], copy[6]);
				AssertEquals("Wrong CopyTo 7", (char)0, copy[7]);
				AssertEquals("Wrong CopyTo 8", (char)0, copy[8]);
				AssertEquals("Wrong CopyTo 9", (char)0, copy[9]);
			}

	public void TestArrayListCopyToArrayIndexOverflow () 
			{
				ArrayList al = new ArrayList ();
				al.Add (this);
				try
				{
					al.CopyTo (0, new byte [2], Int32.MaxValue, 0);
					Fail("Test should have thrown an ArgumentException");
				}
				catch(ArgumentException e)
				{
				}
				catch(Exception e)
				{
					Fail("Test should have thrown an ArgumentException instead of " +
							e.GetType().Name);
				}
			}

	// Test sorting an array list.
	public void TestArrayListSort()
			{
				ArrayList list = new ArrayList();
				list.Add(98);
				list.Add(45);
				list.Sort();
				AssertEquals("Sort (1)", 45, list[0]);
				AssertEquals("Sort (2)", 98, list[1]);

				list = new ArrayList();
				list.Add(98);
				list.Add(0);
				list.Add(45);
				list.Sort();
				AssertEquals("Sort (3)",  0, list[0]);
				AssertEquals("Sort (4)", 45, list[1]);
				AssertEquals("Sort (5)", 98, list[2]);

				list = new ArrayList();
				list.Add(97);
				list.Add(104);
				list.Add(98);
				list.Sort();
				AssertEquals("Sort (6)", 97, list[0]);
				AssertEquals("Sort (7)", 98, list[1]);
				AssertEquals("Sort (8)", 104, list[2]);
			}

	public void TestArrayListSetRangeOverflow () 
			{
				ArrayList arrayList = new ArrayList ();
				arrayList.Add (this);
				try
				{
					arrayList.SetRange (Int32.MaxValue, new ArrayList ());
					Fail("Should have thrown an ArgumentOutOfRangeException");
				}
				catch(ArgumentOutOfRangeException e)
				{
				}
				catch(Exception e)
				{
					Fail("Should have thrown an ArgumentOutOfRangeException instead of " + 
							e.GetType().Name);
				}
			}

}; // class TestArrayList
