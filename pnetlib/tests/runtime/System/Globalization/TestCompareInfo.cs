/*
 * TestCompareInfo.cs - Tests for the "CompareInfo" class.
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
using System.Globalization;

public class TestCompareInfo : TestCase
{
	private CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;

	// Constructor.
	public TestCompareInfo(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// nothing to do here
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	public void TestStringCompare1()
			{
				AssertEquals("Comparing two null strings", 0,
								compareInfo.Compare(null, null));
			}

	public void TestStringCompare2()
			{
				AssertEquals("Comparing null with String.Empty", -1,
								compareInfo.Compare(null, String.Empty));
			}

	public void TestStringCompare3()
			{
				AssertEquals("Comparing two String.Empty with null", 1,
								compareInfo.Compare(String.Empty, null));
			}

	public void TestStringCompare4()
			{
				String string1 = "abcde";
				String string2 = "ABCDE";

				AssertEquals("Comparing " + string1 + " with " + string2, -1,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare5()
			{
				String string1 = "abcde";
				String string2 = "abcde";

				AssertEquals("Comparing " + string1 + " with " + string2, 0,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare6()
			{
				String string1 = "ABCDE";
				String string2 = "abcde";

				AssertEquals("Comparing " + string1 + " with " + string2, 1,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare7()
			{
				String string1 = "abcde";
				String string2 = "bbcde";

				AssertEquals("Comparing " + string1 + " with " + string2, -1,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare8()
			{
				String string1 = "bbcde";
				String string2 = "abcde";

				AssertEquals("Comparing " + string1 + " with " + string2, 1,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare9()
			{
				String string1 = "abcdef";
				String string2 = "abcde";

				AssertEquals("Comparing " + string1 + " with " + string2, 1,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare10()
			{
				String string1 = "abcde";
				String string2 = "abcdef";

				AssertEquals("Comparing " + string1 + " with " + string2, -1,
								compareInfo.Compare(string1, string2));
			}

	public void TestStringCompare11()
			{
				AssertEquals("Comparing two String.Empty", 0,
								compareInfo.Compare(String.Empty, String.Empty));
			}

	public void TestStringCompareIgnoreCase1()
			{
				String string1 = "ABCDE";
				String string2 = "bbcde";

				AssertEquals("Comparing " + string1 + " with " + string2 + 
								" with IgnoreCase", -1,
								compareInfo.Compare(string1, string2,
								CompareOptions.IgnoreCase));
			}

	public void TestStringCompareIgnoreCase2()
			{
				String string1 = "ABCDE";
				String string2 = "abcde";

				AssertEquals("Comparing " + string1 + " with " + string2 + 
								" with IgnoreCase", 0,
								compareInfo.Compare(string1, string2,
								CompareOptions.IgnoreCase));
			}

	public void TestStringCompareIgnoreCase3()
			{
				String string1 = "BBCDE";
				String string2 = "abcde";

				AssertEquals("Comparing " + string1 + " with " + string2 + 
								" with IgnoreCase", 1,
								compareInfo.Compare(string1, string2,
								CompareOptions.IgnoreCase));
			}

	public void TestStringCompareOffset1()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), 0, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset2()
			{
				String string1 = "aabcde";
				String string2 = "bbcdf";
				int offset1 = 2;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), -1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset3()
			{
				String string1 = "aabcdf";
				String string2 = "bbcde";
				int offset1 = 2;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset4()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 0;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					AssertEquals("Comparing Null from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), -1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset5()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset6()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with Null from offset " +
									offset2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset7()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset8()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = -1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset9()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = -1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffset10()
			{
				String string1 = "abcde";
				String string2 = "BBCDE";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				result = compareInfo.Compare(string1, offset1,
												string2, offset2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() +
								" with " + string2 + " from offset " +
								offset2.ToString(), -1, result);
			}

	public void TestStringCompareOffset11()
			{
				String string1 = "ABCDE";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				result = compareInfo.Compare(string1, offset1,
												string2, offset2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() +
								" with " + string2 + " from offset " +
								offset2.ToString(), 1, result);
			}

	public void TestStringCompareOffset12()
			{
				String string1 = "abcde";
				String string2 = "bbcdef";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				result = compareInfo.Compare(string1, offset1,
												string2, offset2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() +
								" with " + string2 + " from offset " +
								offset2.ToString(), -1, result);
			}

	public void TestStringCompareOffset13()
			{
				String string1 = "abcdef";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				result = compareInfo.Compare(string1, offset1,
												string2, offset2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() +
								" with " + string2 + " from offset " +
								offset2.ToString(), 1, result);
			}

	public void TestStringCompareOffset14()
			{
				int offset1 = 0;
				int offset2 = 0;
				int result;

				result = compareInfo.Compare(String.Empty, offset1,
											 String.Empty, offset2);
				AssertEquals("Comparing String.Empty from " +
								offset1.ToString() +
								" with String.Empty from offset " +
								offset2.ToString(), 0, result);
			}

	public void TestStringCompareOffsetIgnoreCase1()
			{
				String string1 = "abcde";
				String string2 = "BBCDE";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), 0, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase2()
			{
				String string1 = "aabcde";
				String string2 = "BBCDF";
				int offset1 = 2;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), -1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase3()
			{
				String string1 = "aabcdf";
				String string2 = "BBCDE";
				int offset1 = 2;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase4()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 0;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing Null from " +
									offset1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString(), -1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase5()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase6()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() +
									" with Null from offset " +
									offset2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase7()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetIgnoreCase8()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = -1;
				int offset2 = 1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.ToString());
				}
			}

	public void TestStringCompareOffsetIgnoreCase9()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = -1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1,
													string2, offset2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.ToString());
				}
			}

	public void TestStringCompareOffsetIgnoreCase10()
			{
				int offset1 = 0;
				int offset2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(String.Empty, offset1,
												 String.Empty, offset2,
												 CompareOptions.IgnoreCase);
					AssertEquals("Comparing String.Empty from " +
									offset1.ToString() +
									" with String.Empty from offset " +
									offset2.ToString(), 0, result);
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.ToString());
				}
			}

	public void TestStringCompareOffsetLength1()
			{
				String string1 = "aabcde";
				String string2 = "bbcde";
				int offset1 = 2;
				int offset2 = 1;
				int length1 = 2;
				int length2 = 2;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length" +
								length1.ToString() +
								" with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 0, result);
			}

	public void TestStringCompareOffsetLength2()
			{
				String string1 = "aabcdef";
				String string2 = "bbcdff";
				int offset1 = 2;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString() + " and length " +
									length2.ToString() , -1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength3()
			{
				String string1 = "aabcdff";
				String string2 = "bbcdef";
				int offset1 = 2;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString() + " and length " +
									length2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength4()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 0;
				int offset2 = 1;
				int length1 = 0;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					AssertEquals("Comparing Null from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									"with " + string2 + " from offset " +
									offset2.ToString() + " and length " +
									length2.ToString(), -1, result);
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength5()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 0;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength6()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 0;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength7()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength8()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 0;
				int length1 = 4;
				int length2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									" with Null from offset " +
									offset2.ToString() + " and length " +
									length2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength9()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength10()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 0;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength11()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength12()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = -1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength13()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 5;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength14()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = -1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength15()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 5;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength16()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = -1;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength17()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 5;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength18()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = -1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength19()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 5;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLength20()
			{
				String string1 = "abcde";
				String string2 = "BBCDE";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), -1, result);
			}

	public void TestStringCompareOffsetLength21()
			{
				String string1 = "ABCDE";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 1, result);
			}

	public void TestStringCompareOffsetLength22()
			{
				String string1 = "abcde";
				String string2 = "bbcdef";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 5;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), -1, result);
			}

	public void TestStringCompareOffsetLength23()
			{
				String string1 = "abcdef";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 5;
				int length2 = 4;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 1, result);
			}

	public void TestStringCompareOffsetLength24()
			{
				int offset1 = 0;
				int offset2 = 0;
				int length1 = 0;
				int length2 = 0;
				int result;

				result = compareInfo.Compare(String.Empty, offset1, length1,
											 String.Empty, offset2, length2);
				AssertEquals("Comparing String.Empty from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with String.Empty from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 0, result);
			}

	public void TestStringCompareOffsetLength25()
			{
				String string1 = "ab";
				String string2 = "abcd";
				int offset1 = 1;
				int offset2 = 4;
				int length1 = 1;
				int length2 = 0;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 1, result);
			}

	public void TestStringCompareOffsetLength26()
			{
				String string1 = "abcd";
				String string2 = "ab";
				int offset1 = 4;
				int offset2 = 1;
				int length1 = 0;
				int length2 = 1;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), -1, result);
			}

	public void TestStringCompareOffsetLengthIgnoreCase1()
			{
				String string1 = "aabcde";
				String string2 = "bbcde";
				int offset1 = 2;
				int offset2 = 1;
				int length1 = 2;
				int length2 = 2;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2,
												CompareOptions.IgnoreCase);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length" +
								length1.ToString() +
								" with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 0, result);
			}

	public void TestStringCompareOffsetLengthIgnoreCase2()
			{
				String string1 = "aabcdef";
				String string2 = "bbcdff";
				int offset1 = 2;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString() + " and length " +
									length2.ToString() , -1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase3()
			{
				String string1 = "aabcdff";
				String string2 = "bbcdef";
				int offset1 = 2;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									" with " + string2 + " from offset " +
									offset2.ToString() + " and length " +
									length2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase4()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 0;
				int offset2 = 1;
				int length1 = 0;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing Null from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									"with " + string2 + " from offset " +
									offset2.ToString() + " and length " +
									length2.ToString(), -1, result);
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase5()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 0;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase6()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 0;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase7()
			{
				String string1 = null;
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase8()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 0;
				int length1 = 4;
				int length2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					AssertEquals("Comparing " + string1 + " from " +
									offset1.ToString() + " and length " +
									length1.ToString() +
									" with Null from offset " +
									offset2.ToString() + " and length " +
									length2.ToString(), 1, result);
				}
				catch (Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase9()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 0;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase10()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 0;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase11()
			{
				String string1 = "abcde";
				String string2 = null;
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase12()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = -1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase13()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 5;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase14()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = -1;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase15()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 5;
				int length1 = 4;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase16()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = -1;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase17()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 5;
				int length2 = 4;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase18()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = -1;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase19()
			{
				String string1 = "abcde";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 5;
				int result;

				try
				{
					result = compareInfo.Compare(string1, offset1, length1,
													string2, offset2, length2,
													CompareOptions.IgnoreCase);
					Fail("Test should have thrown an ArgumentOutOfRangeException" +
							" but returned " + result.ToString());
				}
				catch(ArgumentOutOfRangeException)
				{
					// success
				}
				catch(Exception e)
				{
					Fail("Test should not throw an " + e.GetType().Name);
				}
			}

	public void TestStringCompareOffsetLengthIgnoreCase20()
			{
				String string1 = "abcde";
				String string2 = "BBCDE";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2,
												CompareOptions.IgnoreCase);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 0, result);
			}

	public void TestStringCompareOffsetLengthIgnoreCase21()
			{
				String string1 = "ABCDE";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 4;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2,
												CompareOptions.IgnoreCase);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 0, result);
			}

	public void TestStringCompareOffsetLengthIgnoreCase22()
			{
				String string1 = "abcde";
				String string2 = "bbcdef";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 4;
				int length2 = 5;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2,
												CompareOptions.IgnoreCase);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), -1, result);
			}

	public void TestStringCompareOffsetLengthIgnoreCase23()
			{
				String string1 = "abcdef";
				String string2 = "bbcde";
				int offset1 = 1;
				int offset2 = 1;
				int length1 = 5;
				int length2 = 4;
				int result;

				result = compareInfo.Compare(string1, offset1, length1,
												string2, offset2, length2,
												CompareOptions.IgnoreCase);
				AssertEquals("Comparing " + string1 + " from " +
								offset1.ToString() + " and length " +
								length1.ToString() +
								"with " + string2 + " from offset " +
								offset2.ToString() + " and length " +
								length2.ToString(), 1, result);
			}

}; // class TestCompareInfo
