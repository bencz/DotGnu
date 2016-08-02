/*
 * TestXmlConvert.cs - Tests for the "System.Xml.XmlConvert" class.
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

public class TestXmlConvert : TestCase
{
	// Constructor.
	public TestXmlConvert(String name)
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

	// Test the "XmlConvert.DecodeName" method.
	public void TestXmlConvertDecodeName()
			{
				AssertNull("DecodeName (1)",
						   XmlConvert.DecodeName(null));
				AssertEquals("DecodeName (2)",
							 "foo", XmlConvert.DecodeName("foo"));
				AssertEquals("DecodeName (3)",
							 "Hi", XmlConvert.DecodeName("H_x0069_"));
				AssertEquals("DecodeName (4)",
							 "Hx0069_", XmlConvert.DecodeName("Hx0069_"));
				AssertEquals("DecodeName (5)",
							 "H_x69_", XmlConvert.DecodeName("H_x69_"));
				AssertEquals("DecodeName (6)",
							 "H_x69i", XmlConvert.DecodeName("H_x69_x0069_"));
				AssertEquals("DecodeName (7)",
							 "Hix0069_",
							 XmlConvert.DecodeName("H_x0069_x0069_"));
				AssertEquals("DecodeName (8)",
							 "Hi_i_xxx",
							 XmlConvert.DecodeName("H_x0069___x0069__xxx"));
			}

	// Test the "XmlConvert.EncodeName" method.
	public void TestXmlConvertEncodeName()
			{
				AssertNull("EncodeName (1)",
						   XmlConvert.EncodeName(null));
				AssertEquals("EncodeName (2)",
						     String.Empty, XmlConvert.EncodeName(""));
				AssertEquals("EncodeName (3)",
						     "foo", XmlConvert.EncodeName("foo"));
				AssertEquals("EncodeName (4)",
						     "foo:bar", XmlConvert.EncodeName("foo:bar"));
				AssertEquals("EncodeName (5)",
						     "foo:bar_x002B_",
							 XmlConvert.EncodeName("foo:bar+"));
				AssertEquals("EncodeName (6)",
						     "_x0032_foo", XmlConvert.EncodeName("2foo"));
				AssertEquals("EncodeName (7)",
						     "foo2", XmlConvert.EncodeName("foo2"));
				AssertEquals("EncodeName (8)",
						     "_", XmlConvert.EncodeName("_"));
				AssertEquals("EncodeName (9)",
						     "_x005F_x", XmlConvert.EncodeName("_x"));
			}

	// Test the "XmlConvert.EncodeLocalName" method.
	public void TestXmlConvertEncodeLocalName()
			{
				AssertNull("EncodeLocalName (1)",
						   XmlConvert.EncodeLocalName(null));
				AssertEquals("EncodeLocalName (2)",
						     String.Empty, XmlConvert.EncodeLocalName(""));
				AssertEquals("EncodeLocalName (3)",
						     "foo", XmlConvert.EncodeLocalName("foo"));
				AssertEquals("EncodeLocalName (4)",
						     "foo_x003A_bar",
							 XmlConvert.EncodeLocalName("foo:bar"));
				AssertEquals("EncodeLocalName (5)",
						     "foo_x003A_bar_x002B_",
							 XmlConvert.EncodeLocalName("foo:bar+"));
				AssertEquals("EncodeLocalName (6)",
						     "_x0032_foo", XmlConvert.EncodeLocalName("2foo"));
				AssertEquals("EncodeLocalName (7)",
						     "foo2", XmlConvert.EncodeLocalName("foo2"));
				AssertEquals("EncodeLocalName (8)",
						     "_", XmlConvert.EncodeLocalName("_"));
				AssertEquals("EncodeLocalName (9)",
						     "_x005F_x", XmlConvert.EncodeLocalName("_x"));
			}

	// Test the "XmlConvert.EncodeNmToken" method.
	public void TestXmlConvertEncodeNmToken()
			{
				AssertNull("EncodeNmToken (1)",
						   XmlConvert.EncodeNmToken(null));
				AssertEquals("EncodeNmToken (2)",
						     String.Empty, XmlConvert.EncodeNmToken(""));
				AssertEquals("EncodeNmToken (3)",
						     "foo", XmlConvert.EncodeNmToken("foo"));
				AssertEquals("EncodeNmToken (4)",
						     "foo:bar", XmlConvert.EncodeNmToken("foo:bar"));
				AssertEquals("EncodeNmToken (5)",
						     "foo:bar_x002B_",
							 XmlConvert.EncodeNmToken("foo:bar+"));
				AssertEquals("EncodeNmToken (6)",
						     "2foo", XmlConvert.EncodeNmToken("2foo"));
				AssertEquals("EncodeNmToken (7)",
						     "foo2", XmlConvert.EncodeNmToken("foo2"));
				AssertEquals("EncodeNmToken (8)",
						     "_", XmlConvert.EncodeNmToken("_"));
				AssertEquals("EncodeNmToken (9)",
						     "_x005F_x", XmlConvert.EncodeNmToken("_x"));
			}

	// Test the "XmlConvert.ToBoolean" method.
	public void TestXmlConvertToBoolean()
			{
				try
				{
					XmlConvert.ToBoolean(null);
					Fail("ToBoolean (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}

				Assert("ToBoolean (2)", XmlConvert.ToBoolean("true"));
				Assert("ToBoolean (3)", !XmlConvert.ToBoolean("false"));
				Assert("ToBoolean (4)", XmlConvert.ToBoolean("1"));
				Assert("ToBoolean (5)", !XmlConvert.ToBoolean("0"));
				Assert("ToBoolean (6)", XmlConvert.ToBoolean("  true\t"));

				try
				{
					XmlConvert.ToBoolean("TRUE");
					Fail("ToBoolean (7)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToBoolean("FALSE");
					Fail("ToBoolean (8)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToBoolean("foo");
					Fail("ToBoolean (9)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToBoolean(String.Empty);
					Fail("ToBoolean (10)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToBoolean(" tr ue ");
					Fail("ToBoolean (11)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToByte" method.
	public void TestXmlConvertToByte()
			{
				AssertEquals("ToByte (1)",
							 0, XmlConvert.ToByte("0"));
				AssertEquals("ToByte (2)",
							 42, XmlConvert.ToByte(" 42\t"));

				try
				{
					XmlConvert.ToByte(null);
					Fail("ToByte (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToByte("-1");
					Fail("ToByte (4)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToByte("256");
					Fail("ToByte (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToByte("foo");
					Fail("ToByte (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToChar" method.
	public void TestXmlConvertToChar()
			{
				try
				{
					XmlConvert.ToChar(null);
					Fail("ToChar (1)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				AssertEquals("ToChar (2)", 'A', XmlConvert.ToChar("A"));
				AssertEquals("ToChar (3)", '\u0000', XmlConvert.ToChar("\0"));

				try
				{
					XmlConvert.ToChar("foo");
					Fail("ToChar (4)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToChar(String.Empty);
					Fail("ToChar (5)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToDateTime" method.
	public void TestXmlConvertToDateTime()
			{
				// TODO - waiting for DateTime parsing routines to be finished
			}

#if CONFIG_EXTENDED_NUMERICS

	// Test the "XmlConvert.ToDecimal" method.
	public void TestXmlConvertToDecimal()
			{
#if false	// TODO - some problems with decimal parsing need to be fixed
				AssertEquals("ToDecimal (1)",
							 0.0m, XmlConvert.ToDecimal("0"));
				AssertEquals("ToDecimal (2)",
							 42.0m, XmlConvert.ToDecimal(" 42\t"));
				AssertEquals("ToDecimal (3)",
							 -42.0m, XmlConvert.ToDecimal(" -42\t"));
				AssertEquals("ToDecimal (3)", Decimal.MaxValue,
							 XmlConvert.ToDecimal
							 	("79228162514264337593543950335.0"));

				try
				{
					XmlConvert.ToDecimal(null);
					Fail("ToDecimal (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToDecimal("1e40m");
					Fail("ToDecimal (4)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToDecimal("foo");
					Fail("ToDecimal (5)");
				}
				catch(FormatException)
				{
					// Success
				}
#endif
			}

	// Test the "XmlConvert.ToDouble" method.
	public void TestXmlConvertToDouble()
			{
				AssertEquals("ToDouble (1)",
							 0.0, XmlConvert.ToDouble("0"), 0.0001);
				AssertEquals("ToDouble (2)",
							 0.0, XmlConvert.ToDouble(" -0.0\t"), 0.0001);
				AssertEquals("ToDouble (3)",
							 123.4, XmlConvert.ToDouble("123.4"), 0.0001);
				Assert("ToDouble (4)", Double.IsPositiveInfinity
							(XmlConvert.ToDouble(" INF")));
				Assert("ToDouble (5)", Double.IsNegativeInfinity
							(XmlConvert.ToDouble("-INF")));
			}

#endif // CONFIG_EXTENDED_NUMERICS

	// Test the "XmlConvert.ToInt16" method.
	public void TestXmlConvertToInt16()
			{
				AssertEquals("ToInt16 (1)",
							 0, XmlConvert.ToInt16("0"));
				AssertEquals("ToInt16 (2)",
							 42, XmlConvert.ToInt16(" 42\t"));

				try
				{
					XmlConvert.ToInt16(null);
					Fail("ToInt16 (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt16("-32769");
					Fail("ToInt16 (4)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt16("32768");
					Fail("ToInt16 (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt16("foo");
					Fail("ToInt16 (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToUInt16" method.
	public void TestXmlConvertToUInt16()
			{
				AssertEquals("ToUInt16 (1)",
							 0, XmlConvert.ToUInt16("0"));
				AssertEquals("ToUInt16 (2)",
							 (uint)42, XmlConvert.ToUInt16(" 42\t"));

				try
				{
					XmlConvert.ToUInt16(null);
					Fail("ToUInt16 (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt16("-1");
					Fail("ToUInt16 (4)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt16("65536");
					Fail("ToUInt16 (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt16("foo");
					Fail("ToUInt16 (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToInt32" method.
	public void TestXmlConvertToInt32()
			{
				AssertEquals("ToInt32 (1)",
							 0, XmlConvert.ToInt32("0"));
				AssertEquals("ToInt32 (2)",
							 42, XmlConvert.ToInt32(" 42\t"));

				try
				{
					XmlConvert.ToInt32(null);
					Fail("ToInt32 (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt32("-2147483649");
					Fail("ToInt32 (4)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt32("2147483648");
					Fail("ToInt32 (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt32("foo");
					Fail("ToInt32 (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}


	// Test the "XmlConvert.ToUInt32" method.
	public void TestXmlConvertToUInt32()
			{
				AssertEquals("ToUInt32 (1)",
							 0, XmlConvert.ToUInt32("0"));
				AssertEquals("ToUInt32 (2)",
							 (uint)42, XmlConvert.ToUInt32(" 42\t"));

				try
				{
					XmlConvert.ToUInt32(null);
					Fail("ToUInt32 (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt32("-1");
					Fail("ToUInt32 (4)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt32("4294967296");
					Fail("ToUInt32 (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt32("foo");
					Fail("ToUInt32 (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToInt64" method.
	public void TestXmlConvertToInt64()
			{
				AssertEquals("ToInt64 (1)",
							 0, XmlConvert.ToInt64("0"));
				AssertEquals("ToInt64 (2)",
							 42, XmlConvert.ToInt64(" 42\t"));

				try
				{
					XmlConvert.ToInt64(null);
					Fail("ToInt64 (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt64("-9223372036854775809");
					Fail("ToInt64 (4)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt64("9223372036854775808");
					Fail("ToInt64 (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToInt64("foo");
					Fail("ToInt64 (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}


	// Test the "XmlConvert.ToUInt64" method.
	public void TestXmlConvertToUInt64()
			{
				AssertEquals("ToUInt64 (1)",
							 (ulong)0, XmlConvert.ToUInt64("0"));
				AssertEquals("ToUInt64 (2)",
							 (ulong)42, XmlConvert.ToUInt64(" 42\t"));

				try
				{
					XmlConvert.ToUInt64(null);
					Fail("ToUInt64 (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt64("-1");
					Fail("ToUInt64 (4)");
				}
				catch(FormatException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt64("18446744073709551616");
					Fail("ToUInt64 (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToUInt64("foo");
					Fail("ToUInt64 (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.ToSByte" method.
	public void TestXmlConvertToSByte()
			{
				AssertEquals("ToSByte (1)",
							 0, XmlConvert.ToSByte("0"));
				AssertEquals("ToSByte (2)",
							 42, XmlConvert.ToSByte(" 42\t"));

				try
				{
					XmlConvert.ToSByte(null);
					Fail("ToSByte (3)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToSByte("-129");
					Fail("ToSByte (4)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToSByte("128");
					Fail("ToSByte (5)");
				}
				catch(OverflowException)
				{
					// Success
				}

				try
				{
					XmlConvert.ToSByte("foo");
					Fail("ToSByte (6)");
				}
				catch(FormatException)
				{
					// Success
				}
			}

#if CONFIG_EXTENDED_NUMERICS

	// Test the "XmlConvert.ToSingle" method.
	public void TestXmlConvertToSingle()
			{
				AssertEquals("ToSingle (1)",
							 0.0, XmlConvert.ToSingle("0"), 0.0001);
				AssertEquals("ToSingle (2)",
							 0.0, XmlConvert.ToSingle(" -0.0\t"), 0.0001);
				AssertEquals("ToSingle (3)",
							 123.4, XmlConvert.ToSingle("123.4"), 0.0001);
				Assert("ToSingle (4)", Single.IsPositiveInfinity
							(XmlConvert.ToSingle(" INF")));
				Assert("ToSingle (5)", Single.IsNegativeInfinity
							(XmlConvert.ToSingle("-INF")));
			}

#endif // CONFIG_EXTENDED_NUMERICS

	// Test the "XmlConvert.ToTimeSpan" method.
	public void TestXmlConvertToTimeSpan()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for bool values.
	public void TestXmlConvertBooleanToString()
			{
				AssertEquals("BooleanToString (1)",
							 "true", XmlConvert.ToString(true));
				AssertEquals("BooleanToString (2)",
							 "false", XmlConvert.ToString(false));
			}

	// Test the "XmlConvert.ToString" method for char values.
	public void TestXmlConvertCharToString()
			{
				AssertEquals("CharToString (1)",
							 "A", XmlConvert.ToString('A'));
				AssertEquals("CharToString (2)",
							 "\u0000", XmlConvert.ToString('\0'));
			}

	// Test the "XmlConvert.ToString" method for decimal values.
	public void TestXmlConvertDecimalToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for byte values.
	public void TestXmlConvertByteToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for sbyte values.
	public void TestXmlConvertSByteToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for short values.
	public void TestXmlConvertInt16ToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for ushort values.
	public void TestXmlConvertUInt16ToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for int values.
	public void TestXmlConvertInt32ToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for uint values.
	public void TestXmlConvertUInt32ToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for long values.
	public void TestXmlConvertInt64ToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for ulong values.
	public void TestXmlConvertUInt64ToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for float values.
	public void TestXmlConvertSingleToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for double values.
	public void TestXmlConvertDoubleToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for TimeSpan values.
	public void TestXmlConvertTimeSpanToString()
			{
				// TODO
			}

	// Test the "XmlConvert.ToString" method for DateTime values.
	public void TestXmlConvertDateTimeToString()
			{
				// TODO
			}

	// Helper methods for "TestXmlConvertVerifyName".
	private static void VerifyNameSuccess(String msg, String name)
			{
				try
				{
					XmlConvert.VerifyName(name);
				}
				catch(XmlException)
				{
					Fail(msg);
				}
			}
	private static void VerifyNameFail(String msg, String name)
			{
				try
				{
					XmlConvert.VerifyName(name);
					Fail(msg);
				}
				catch(XmlException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.VerifyName" method.
	public void TestXmlConvertVerifyName()
			{
				try
				{
					XmlConvert.VerifyName(null);
					Fail("VerifyName (1)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.VerifyName(String.Empty);
					Fail("VerifyName (2)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				VerifyNameSuccess("VerifyName (3)", "foo");
				VerifyNameSuccess("VerifyName (4)", "foo:bar");
				VerifyNameSuccess("VerifyName (5)", "foo2");
				VerifyNameFail("VerifyName (6)", "2foo");
				VerifyNameFail("VerifyName (7)", "foo+");
				VerifyNameSuccess("VerifyName (8)", "foo_bar");
			}

	// Helper methods for "TestXmlConvertVerifyNCName".
	private static void VerifyNCNameSuccess(String msg, String name)
			{
				try
				{
					XmlConvert.VerifyNCName(name);
				}
				catch(XmlException)
				{
					Fail(msg);
				}
			}
	private static void VerifyNCNameFail(String msg, String name)
			{
				try
				{
					XmlConvert.VerifyNCName(name);
					Fail(msg);
				}
				catch(XmlException)
				{
					// Success
				}
			}

	// Test the "XmlConvert.VerifyNCName" method.
	public void TestXmlConvertVerifyNCName()
			{
				try
				{
					XmlConvert.VerifyNCName(null);
					Fail("VerifyNCName (1)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				try
				{
					XmlConvert.VerifyNCName(String.Empty);
					Fail("VerifyNCName (2)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				VerifyNCNameSuccess("VerifyNCName (3)", "foo");
				VerifyNCNameFail("VerifyNCName (4)", "foo:bar");
				VerifyNCNameSuccess("VerifyNCName (5)", "foo2");
				VerifyNCNameFail("VerifyNCName (6)", "2foo");
				VerifyNCNameFail("VerifyNCName (7)", "foo+");
				VerifyNCNameSuccess("VerifyNCName (8)", "foo_bar");
			}

}; // class TestXmlConvert
