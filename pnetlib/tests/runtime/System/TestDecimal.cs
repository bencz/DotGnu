/*
 * TestDecimal.cs - Test class for "System.Decimal" 
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2002  FSF.
 * 
 * Authors : Autogenerated using csdoc2test 
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

public class TestDecimal : TestCase
 {
	// Constructor.
	public TestDecimal(String name)	: base(name)
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
#if CONFIG_EXTENDED_NUMERICS

	public void TestDecimalParse()
	{
		AssertEquals("Decimal.Parse(\"1.0\")",
				1.0m, Decimal.Parse("1.0"));
		AssertEquals("Decimal.Parse(\"-1.0\")",
				-1.0m, Decimal.Parse("-1.0"));
		try
		{
			AssertEquals("Decimal.Parse(\"1.0E+2\")",
				100m, Decimal.Parse("1.0E+2"));
			Fail("Decimal.Parse(\"1.0E+2\") should throw a FormatException");
		}
		catch(FormatException)
		{
		}
		try
		{
			AssertEquals("Decimal.Parse(\"1.0E+2\", NumberStyles.Number | NumberStyles.AllowExponent, null)",
				100m, Decimal.Parse("1.0E+2", NumberStyles.Number | NumberStyles.AllowExponent, null));
		}
		catch(FormatException)
		{
			Fail("Decimal.Parse(\"1.0E+2\") must not throw a FormatException");
		}
		AssertEquals("Decimal.Parse(\"0.0\")",
				0.0m, Decimal.Parse("0.0"));
		AssertEquals("Decimal.Parse(\"0.5\")",
				0.5m, Decimal.Parse("0.5"));
		AssertEquals("Decimal.Parse(\"0.500000000000000\")",
				0.5m, Decimal.Parse("0.500000000000000"));
		AssertEquals("Decimal.Parse(\"1.13\")",
				1.13m, Decimal.Parse("1.13"));
		AssertEquals("Decimal.Parse(\"1.130000000000000\")",
				1.13m, Decimal.Parse("1.130000000000000"));
		try
		{
			AssertEquals("Decimal.Parse(\"1e+2\")",
				100m, Decimal.Parse("1e+2"));
			Fail("Decimal.Parse(\"1e+2\") should throw a FormatException");
		}
		catch(FormatException)
		{
		}
		try
		{
			AssertEquals("Decimal.Parse(\"1e+2\", NumberStyles.Number | NumberStyles.AllowExponent, null)",
				100m, Decimal.Parse("1e+2", NumberStyles.Number | NumberStyles.AllowExponent, null));
		}
		catch(FormatException)
		{
			Fail("Decimal.Parse(\"1e+2\", NumberStyles.Number | NumberStyles.AllowExponent) must not throw a FormatException");
		}
		try
		{
			AssertEquals("Decimal.Parse(\"1e-2\")",
				0.01m, Decimal.Parse("1e-2"));
			Fail("Decimal.Parse(\"1e-2\") should throw a FormatException");
		}
		catch(FormatException)
		{
		}
		try
		{
			AssertEquals("Decimal.Parse(\"1e-2\", NumberStyles.Number | NumberStyles.AllowExponent, null)",
				0.01m, Decimal.Parse("1e-2", NumberStyles.Number | NumberStyles.AllowExponent, null));
		}
		catch(FormatException)
		{
			Fail("Decimal.Parse(\"1e-2\", NumberStyles.Number | NumberStyles.AllowExponent) must not throw a FormatException");
		}
		try
		{
			AssertEquals("Decimal.Parse(\"1e2\")",
				100m, Decimal.Parse("1e2"));
			Fail("Decimal.Parse(\"1e2\") should throw a FormatException");
		}
		catch(FormatException)
		{
		}
		try
		{
			AssertEquals("Decimal.Parse(\"1e2\", NumberStyles.Number | NumberStyles.AllowExponent, null)",
				100m, Decimal.Parse("1e2", NumberStyles.Number | NumberStyles.AllowExponent, null));
		}
		catch(FormatException)
		{
			Fail("Decimal.Parse(\"1e2\", NumberStyles.Number | NumberStyles.AllowExponent) must not throw a FormatException");
		}
	}

	public void TestDecimalToString()
	{
		decimal d;

		d = 0.0m;
		AssertEquals("0", "0", d.ToString());

		d = 1.0m;
		AssertEquals("1", "1", d.ToString());

		d = 1.13m;
		AssertEquals("1.13", "1.13", d.ToString());

		d = -1.13m;
		AssertEquals("-1.13", "-1.13", d.ToString());
	}

#endif // CONFIG_EXTENDED_NUMERICS
}
