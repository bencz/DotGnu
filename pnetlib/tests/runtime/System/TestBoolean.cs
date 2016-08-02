/*
 * TestBoolean.cs - Tests for the "Boolean" class.
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

public class TestBoolean : TestCase
{
	// Constructor.
	public TestBoolean(String name)
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

	// Test the constants.
	public void TestBooleanConstants()
			{
				AssertEquals("Boolean.True", "True", Boolean.TrueString);
				AssertEquals("Boolean.False", "False", Boolean.FalseString);
			}

	// Test the CompareTo method.
	public void TestBooleanCompareTo()
			{
				bool b1 = false;
				bool b2 = true;

				AssertEquals("false.CompareTo(true)", b1.CompareTo(true), -1);
				AssertEquals("false.CompareTo(false)", b1.CompareTo(false), 0);
				AssertEquals("false.CompareTo(null)", b1.CompareTo(null), 1);
				try
				{
					b1.CompareTo(3);
					Fail("false.CompareTo(3) should not return a value");
				}
				catch(ArgumentException)
				{
					// Success
				}

				AssertEquals("true.CompareTo(true)", b2.CompareTo(true), 0);
				AssertEquals("true.CompareTo(false)", b2.CompareTo(false), 1);
				AssertEquals("true.CompareTo(null)", b2.CompareTo(null), 1);
				try
				{
					b2.CompareTo(3);
					Fail("true.CompareTo(3) should not return a value");
				}
				catch(ArgumentException)
				{
					// Success
				}
			}

	// Test the Equals method.
	public void TestBooleanEquals()
			{
				bool b1 = false;
				bool b2 = true;

				Assert("!false.Equals(true)", !b1.Equals(true));
				Assert("false.Equals(false)", b1.Equals(false));
				Assert("!false.Equals(null)", !b1.Equals(null));
				Assert("!false.Equals(3)", !b1.Equals(3));

				Assert("true.Equals(true)", b2.Equals(true));
				Assert("!true.Equals(false)", !b2.Equals(false));
				Assert("!true.Equals(null)", !b2.Equals(null));
				Assert("!true.Equals(3)", !b2.Equals(3));
			}

	// Test the GetHashCode method.
	public void TestBooleanGetHashCode()
			{
				// We don't really care what the values are.  We test
				// that no exceptions are thrown and that the values
				// are different for the two boolean values.
				false.GetHashCode();
				true.GetHashCode();
				Assert("false.GetHashCode() != true.GetHashCode()",
					   false.GetHashCode() != true.GetHashCode());
			}

#if !ECMA_COMPAT
	// Test the GetTypeCode method
	public void TestBooleanGetTypeCode ()
			{
				Boolean boolean = true;
				AssertEquals("GetTypeCode failed", TypeCode.Boolean, boolean.GetTypeCode());
			}
#endif // !ECMA_COMPAT
	
	// Test AND operator
	public void TestANDOperator()
			{
				bool b1 = true;
				bool b2 = false;

				Assert("false.Equals(false & false)", b2.Equals(b2 & b2));
				Assert("false.Equals(true & false)", b2.Equals(b1 & b2));
				Assert("false.Equals(false & true)", b2.Equals(b2 & b1));
				Assert("true.Equals(true & true)", b1.Equals(b1 & b1));
			}

	// Test OR operator
	public void TestOROperator()
			{
				bool b1 = true;
				bool b2 = false;

				Assert("false.Equals(false | false)", b2.Equals(b2 | b2));
				Assert("true.Equals(true | false)", b1.Equals(b1 | b2));
				Assert("true.Equals(false | true)", b1.Equals(b2 | b1));
				Assert("true.Equals(true | true)", b1.Equals(b1 | b1));
			}

	// Test XOR operators
	public void TestXOROperator()
			{
				bool b1 = true;
				bool b2 = false;

				Assert("false.Equals(false ^ false)", b2.Equals(b2 ^ b2));
				Assert("true.Equals(true ^ false)", b1.Equals(b1 ^ b2));
				Assert("true.Equals(false ^ true)", b1.Equals(b2 ^ b1));
				Assert("false.Equals(true ^ true)", b2.Equals(b1 ^ b1));
			}

	// Test the Parse method.
	public void TestBooleanParse()
			{
				// Test the null argument case.
				try
				{
					Boolean.Parse(null);
					Fail("Boolean.Parse(null) should not succeed");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				// Test parses that are expected to succeed.
				Assert("Boolean.Parse(\"True\")", Boolean.Parse("True"));
				Assert("!Boolean.Parse(\"False\")", !Boolean.Parse("False"));
				Assert("Boolean.Parse(\"true\")", Boolean.Parse("true"));
				Assert("!Boolean.Parse(\"false\")", !Boolean.Parse("false"));
				Assert("Boolean.Parse(\" true\")", Boolean.Parse(" true"));
				Assert("!Boolean.Parse(\" false\")", !Boolean.Parse(" false"));
				Assert("Boolean.Parse(\"true \")", Boolean.Parse("true "));
				Assert("!Boolean.Parse(\"false \")", !Boolean.Parse("false "));

				// Test parses that are expected to fail.
				try
				{
					Boolean.Parse("TrueBlue");
					Fail("Boolean.Parse(\"TrueBlue\") should " +
						 "not return a value");
				}
				catch(FormatException)
				{
					// Success
				}
				try
				{
					Boolean.Parse("");
					Fail("Boolean.Parse(\"\") should not return a value");
				}
				catch(FormatException)
				{
					// Success
				}
				try
				{
					Boolean.Parse("x");
					Fail("Boolean.Parse(\"x\") should not return a value");
				}
				catch(FormatException)
				{
					// Success
				}
				try
				{
					Boolean.Parse("   ");
					Fail("Boolean.Parse(\"   \") should not return a value");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the ToString method.
	public void TestBooleanToString()
			{
				AssertEquals("true.ToString()", "True", true.ToString());
				AssertEquals("false.ToString()", "False", false.ToString());
				AssertEquals("true.ToString(null)",
							 "True", true.ToString(null));
				AssertEquals("false.ToString(null)",
							 "False", false.ToString(null));
			}

#if !ECMA_COMPAT

	// Test the IConvertible interface.
	public void TestBooleanIConvertible()
			{
//				AssertEquals("false.GetTypeCode()", TypeCode.Boolean,
//							 false.GetTypeCode());

				Assert("true.ToBoolean(null)",
					   ((IConvertible)true).ToBoolean(null));
				Assert("!false.ToBoolean(null)",
					   !((IConvertible)false).ToBoolean(null));

				AssertEquals("true.ToByte(null)", 1,
						     ((IConvertible)true).ToByte(null));
				AssertEquals("false.ToByte(null)", 0,
							 ((IConvertible)false).ToByte(null));

				AssertEquals("true.ToSByte(null)", 1,
							 ((IConvertible)true).ToSByte(null));
				AssertEquals("false.ToSByte(null)", 0,
							 ((IConvertible)false).ToSByte(null));

				AssertEquals("true.ToInt16(null)", 1,
							 ((IConvertible)true).ToInt16(null));
				AssertEquals("false.ToInt16(null)", 0,
							 ((IConvertible)false).ToInt16(null));

				AssertEquals("true.ToUInt16(null)", 1,
							 ((IConvertible)true).ToUInt16(null));
				AssertEquals("false.ToUInt16(null)", 0,
							 ((IConvertible)false).ToUInt16(null));

				AssertEquals("true.ToInt32(null)", 1,
							 ((IConvertible)true).ToInt32(null));
				AssertEquals("false.ToInt32(null)", 0,
							 ((IConvertible)false).ToInt32(null));

				AssertEquals("true.ToUInt32(null)", 1,
							 ((IConvertible)true).ToUInt32(null));
				AssertEquals("false.ToUInt32(null)", 0,
							 ((IConvertible)false).ToUInt32(null));

				AssertEquals("true.ToInt64(null)", 1,
							 ((IConvertible)true).ToInt64(null));
				AssertEquals("false.ToInt64(null)", 0,
							 ((IConvertible)false).ToInt64(null));

				AssertEquals("true.ToUInt64(null)", 1,
							 ((IConvertible)true).ToUInt64(null));
				AssertEquals("false.ToUInt64(null)", 0,
							 ((IConvertible)false).ToUInt64(null));

				AssertEquals("true.ToSingle(null)", 1.0f,
							 ((IConvertible)true).ToSingle(null), 0.01f);
				AssertEquals("false.ToSingle(null)", 0.0f,
							 ((IConvertible)false).ToSingle(null), 0.01f);

				AssertEquals("true.ToDouble(null)", 1.0d,
							 ((IConvertible)true).ToDouble(null), 0.01d);
				AssertEquals("false.ToDouble(null)", 0.0d,
							 ((IConvertible)false).ToDouble(null), 0.01d);

//				AssertEquals("true.ToDecimal(null)", 1.0m,
//							 ((IConvertible)true).ToDecimal(null));
//				AssertEquals("false.ToDecimal(null)", 0.0m,
//							 ((IConvertible)false).ToDecimal(null));

				try
				{
					((IConvertible)true).ToDateTime(null);
					Fail("true.ToDateTime(null) should not return a value");
				}
				catch(InvalidCastException)
				{
					// Success
				}

				try
				{
					((IConvertible)false).ToDateTime(null);
					Fail("false.ToDateTime(null) should not return a value");
				}
				catch(InvalidCastException)
				{
					// Success
				}

//				AssertEquals("true.ToType(typeof(int), null)", 1,
//							 ((IConvertible)true).ToType(typeof(int), null));
//				AssertEquals("false.ToType(typeof(int), null)", 0,
//							 ((IConvertible)false).ToType(typeof(int), null));
			}

#if CONFIG_FRAMEWORK_2_0
	// Test the TryParse method.
	public void TestBooleanTryParse()
			{
				// Test the null argument case.
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse(null, out result);
					Assert("Returnvalue Boolean.Parse(null, out result)", !rc);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(null, out result) should not throw any exception");
				}

				// Test parses that are expected to succeed.
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse("True", out result);
					Assert("Returnvalue Boolean.Parse(\"True\", out result)", rc);
					Assert("result Boolean.Parse(\"True\", out result)", result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"True\", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse("False", out result);
					Assert("Returnvalue Boolean.Parse(\"False\", out result)", rc);
					Assert("result Boolean.Parse(\"False\", out result)", !result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"False\", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse("true", out result);
					Assert("Returnvalue Boolean.Parse(\"true\", out result)", rc);
					Assert("result Boolean.Parse(\"true\", out result)", result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"true\", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse("false", out result);
					Assert("Returnvalue Boolean.Parse(\"false\", out result)", rc);
					Assert("result Boolean.Parse(\"false\", out result)", !result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"false\", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse(" true", out result);
					Assert("Returnvalue Boolean.Parse(\" true\", out result)", rc);
					Assert("result Boolean.Parse(\" true\", out result)", result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\" true\", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse(" false", out result);
					Assert("Returnvalue Boolean.Parse(\" false\", out result)", rc);
					Assert("result Boolean.Parse(\" false\", out result)", !result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\" false\", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse("true ", out result);
					Assert("Returnvalue Boolean.Parse(\"true \", out result)", rc);
					Assert("result Boolean.Parse(\"true \", out result)", result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"true \", out result) should not throw any exception");
				}
				try
				{
					bool result;
					bool rc;

					rc = Boolean.TryParse("false ", out result);
					Assert("Returnvalue Boolean.Parse(\"false \", out result)", rc);
					Assert("result Boolean.Parse(\"false \", out result)", !result);
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"false \", out result) should not throw any exception");
				}

				// Test parses that are expected to fail.
				try
				{
					bool result;

					Assert("false = Boolean.TryParse(\"TrueBlue\")", !Boolean.TryParse("TrueBlue", out result));
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"TrueBlue\", out result) should not throw any exception");
				}
				try
				{
					bool result;

					Assert("false = Boolean.TryParse(\"\")", !Boolean.TryParse("", out result));
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"\", out result) should not throw any exception");
				}
				try
				{
					bool result;

					Assert("false = Boolean.TryParse(\"x\")", !Boolean.TryParse("x", out result));
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"x\", out result) should not throw any exception");
				}
				try
				{
					bool result;

					Assert("false = Boolean.TryParse(\"   \")", !Boolean.TryParse("   ", out result));
				}
				catch
				{
					// Failure
					Fail("Boolean.TryParse(\"   \", out result) should not throw any exception");
				}
			}
#endif // CONFIG_FRAMEWORK_2_0
#endif // !ECMA_COMPAT

}; // class TestBoolean
