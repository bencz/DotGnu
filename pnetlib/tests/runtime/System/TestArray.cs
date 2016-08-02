/*
 * TestArray.cs - Tests for the "Array" class.
 *
 * Copyright (C) 2003, 2009  Southern Storm Software, Pty Ltd.
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

public class TestArray : TestCase
{
	// Constructor.
	public TestArray(String name)
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

	// Test binary searches through an array.
	public void TestArrayBinarySearch()
			{
				// Test parameter checks.
				try
				{
					Array.BinarySearch(null, 42);
					Fail("BinarySearch (1)");
				}
				catch(ArgumentNullException)
				{
					// Test succeeded.
				}
				try
				{
					Array.BinarySearch(new int [3, 3], 42);
					Fail("BinarySearch (2)");
				}
				catch(RankException)
				{
					// Test succeeded.
				}
				int[] array = new int [] {1, 2, 3, 5, 6};
				try
				{
					Array.BinarySearch(array, -1, 1, 42);
					Fail("BinarySearch (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Test succeeded.
				}
				try
				{
					Array.BinarySearch(array, 0, -1, 42);
					Fail("BinarySearch (4)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Test succeeded.
				}
				try
				{
					Array.BinarySearch(array, 6, 0, 42);
					Fail("BinarySearch (5)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					Array.BinarySearch(array, 5, 1, 42);
					Fail("BinarySearch (6)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					Array.BinarySearch(array, 4, 5, 42);
					Fail("BinarySearch (7)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					// Upper bound is Int32.MaxValue, which is not allowed.
					Array.BinarySearch
						(Array.CreateInstance(typeof(int),
											  new int [] {16},
											  new int [] {0x7FFFFFF0}),
						 0x7FFFFFF0, 16, 42);
					Fail("BinarySearch (8)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					// Don't know how to compare the array elements.
					Array.BinarySearch(new Object []
										{new Object(), new Object()},
									   0, 2, new Object());
					Fail("BinarySearch (9)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}

				// Search an empty list.
				AssertEquals("BinarySearch (10)",
							 ~0, Array.BinarySearch(new int [0], 42));
				AssertEquals("BinarySearch (11)",
							 ~0, Array.BinarySearch(new int [0], 0, 0, 42));

				// Search a specific list for all values.
				int posn;
				for(posn = 0; posn < array.Length; ++posn)
				{
					AssertEquals("BinarySearch (12)", posn,
								 Array.BinarySearch(array, array[posn]));
				}

				// Search for missing values.
				AssertEquals("BinarySearch (13)", ~0,
							 Array.BinarySearch(array, -1));
				AssertEquals("BinarySearch (14)", ~5,
							 Array.BinarySearch(array, 42));
				AssertEquals("BinarySearch (15)", ~3,
							 Array.BinarySearch(array, 4));
			}

	// Test clearing the elements of an array.
	public void TestArrayClear()
			{
				// Test parameter checks.
				try
				{
					Array.Clear(null, 0, 0);
					Fail("Clear (1)");
				}
				catch(ArgumentNullException)
				{
					// Test succeeded.
				}
				Array.Clear(new int[0], 0, 0);
				Array.Clear(new int [3, 3], 0, 9);	// Indices are relative.
				int[] array = new int [] {1, 2, 3, 5, 6};
				try
				{
					Array.Clear(array, -1, 1);
					Fail("Clear (2)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Clear(array, 0, -1);
					Fail("Clear (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Clear(array, 6, 0);
					Fail("Clear (4)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Clear(array, 5, 1);
					Fail("Clear (5)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Clear(array, 4, 5);
					Fail("Clear (6)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}

				// "array" should still have its original values.
				AssertEquals("Clear (7a)", 1, array[0]);
				AssertEquals("Clear (7b)", 2, array[1]);
				AssertEquals("Clear (7c)", 3, array[2]);
				AssertEquals("Clear (7d)", 5, array[3]);
				AssertEquals("Clear (7e)", 6, array[4]);

				// Clear a subset of the array.
				Array.Clear(array, 2, 2);

				// Check that the clear was successful.
				AssertEquals("Clear (8a)", 1, array[0]);
				AssertEquals("Clear (8b)", 2, array[1]);
				AssertEquals("Clear (8c)", 0, array[2]);
				AssertEquals("Clear (8d)", 0, array[3]);
				AssertEquals("Clear (8e)", 6, array[4]);
			}

	// Determine if two arrays have the same contents.
	private static bool SameContents(Array a, Array b)
			{
				int index = a.Length;
				while(index > 0)
				{
					--index;
					if(!a.GetValue(index).Equals(b.GetValue(index)))
					{
						return false;
					}
				}
				return true;
			}

	// Helper to clone an array via the ICloneable interface
	static private Object CloneObject(ICloneable o)
			{
				return o.Clone();
			}

	// Test Cloning Arrays
	public void TestArrayClone()
			{
				int[] a;
				int[] b;

				a = new int [] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
				b = (int[])a.Clone();
				Assert("Clone (1)", SameContents(a, b));

				b = (int[])CloneObject(a);
				Assert("Clone (2)", SameContents(a, b));
			}

	// Test copying arrays.
	public void TestArrayCopy()
			{
				int[] a = new int [10];
				int[] b = new int [10];
				int[,] c = new int [3, 3];
				byte[] d = new byte [10];

				// Parameter validity checks.
				try
				{
					Array.Copy(null, b, 10);
					Fail("Copy (1)");
				}
				catch(ArgumentNullException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Copy(a, null, 10);
					Fail("Copy (2)");
				}
				catch(ArgumentNullException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Copy(a, b, -1);
					Fail("Copy (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Copy(a, b, 11);
					Fail("Copy (4)");
				}
				catch(ArgumentException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Copy(a, c, 3);
					Fail("Copy (5)");
				}
				catch(RankException)
				{
					// Test succeeded.
				}
				try
				{
					Array.Copy(a, d, 3);
					Fail("Copy (6)");
				}
				catch(ArrayTypeMismatchException)
				{
					// Test succeeded.
				}

				// Test copies within the same array.
				a = new int [] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
				Array.Copy(a, a, 10);
				Assert("Copy (7)", SameContents
					(a, new int [] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}));
				Array.Copy(a, 8, a, 0, 2);
				Assert("Copy (8)", SameContents
					(a, new int [] {9, 10, 3, 4, 5, 6, 7, 8, 9, 10}));
				Array.Copy(a, 1, a, 2, 2);
				Assert("Copy (9)", SameContents
					(a, new int [] {9, 10, 10, 3, 5, 6, 7, 8, 9, 10}));

				// Test copies between arrays.
				a = new int [] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
				b = new int [] {10, 9, 8, 7, 6, 5, 4, 3, 2, 1};
				Array.Copy(a, 3, b, 7, 3);
				Assert("Copy (10)", SameContents
					(a, new int [] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}));
				Assert("Copy (11)", SameContents
					(b, new int [] {10, 9, 8, 7, 6, 5, 4, 4, 5, 6}));

				// Test copies with cast conversions.
				Array.Copy(d, a, 10);
				ValueType[] e = new ValueType [10];
				Array.Copy(a, e, 10);
			}

	// Test sorting an array.
	public void TestArraySort()
			{
				int[] list = new int [] {98, 45};
				Array.Sort(list);
				AssertEquals("Sort (1)", 45, list[0]);
				AssertEquals("Sort (2)", 98, list[1]);

				list = new int[] {98, 0, 45};
				Array.Sort(list);
				AssertEquals("Sort (3)",  0, list[0]);
				AssertEquals("Sort (4)", 45, list[1]);
				AssertEquals("Sort (5)", 98, list[2]);

				list = new int[] {97, 104, 98};
				Array.Sort(list);
				AssertEquals("Sort (6)", 97, list[0]);
				AssertEquals("Sort (7)", 98, list[1]);
				AssertEquals("Sort (8)", 104, list[2]);
			}

}; // class TestArray
