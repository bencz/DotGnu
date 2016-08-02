/*
 * TestString.cs - Tests for Basic string operations.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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
using Microsoft.VisualBasic;

public class TestString : TestCase
{
	// Constructor.
	public TestString(String name)
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

	// Test the "Asc" function (we don't test locale-specific encodings
	// because there is no easy way to predict what Encoding.Default is).
	public void TestAsc()
			{
				AssertEquals("Asc (1)", (int)'y', Strings.Asc("y"));
				AssertEquals("Asc (2)", (int)'z', Strings.Asc('z'));
				AssertEquals("Asc (3)", (int)'f', Strings.Asc("foobar"));
				try
				{
					Strings.Asc(null);
					Fail("Asc (4)");
				}
				catch(ArgumentException)
				{
					// OK
				}
				try
				{
					Strings.Asc("");
					Fail("Asc (5)");
				}
				catch(ArgumentException)
				{
					// OK
				}
			}

	// Test the "AscW" function.
	public void TestAscW()
			{
				AssertEquals("AscW (1)", (int)'y', Strings.AscW("y"));
				AssertEquals("AscW (2)", (int)'z', Strings.AscW('z'));
				AssertEquals("AscW (3)", (int)'f', Strings.AscW("foobar"));
				try
				{
					Strings.AscW(null);
					Fail("AscW (4)");
				}
				catch(ArgumentException)
				{
					// OK
				}
				try
				{
					Strings.AscW("");
					Fail("AscW (5)");
				}
				catch(ArgumentException)
				{
					// OK
				}
			}

	// Test the "Chr" function (we don't test locale-specific encodings
	// because there is no easy way to predict what Encoding.Default is).
	public void TestChr()
			{
				AssertEquals("Chr (1)", 'y', Strings.Chr((int)'y'));
				try
				{
					Strings.Chr(65536);
					Fail("Chr (2)");
				}
				catch(ArgumentException)
				{
					// OK
				}
				try
				{
					Strings.Chr(-32769);
					Fail("Chr (3)");
				}
				catch(ArgumentException)
				{
					// OK
				}
			}

	// Test the "ChrW" function.
	public void TestChrW()
			{
				AssertEquals("ChrW (1)", 'y', Strings.ChrW((int)'y'));
				AssertEquals("ChrW (2)", '\u1234', Strings.ChrW(0x1234));
				AssertEquals("ChrW (3)", '\uFFFF', Strings.ChrW(-1));
				try
				{
					Strings.ChrW(65536);
					Fail("ChrW (4)");
				}
				catch(ArgumentException)
				{
					// OK
				}
				try
				{
					Strings.ChrW(-32769);
					Fail("ChrW (4)");
				}
				catch(ArgumentException)
				{
					// OK
				}
			}

}; // class TestString
