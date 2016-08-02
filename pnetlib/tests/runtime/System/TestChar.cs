/*
 * TestChar.cs - Tests for the "System.Char" class.
 *
 * Copyright (C) 2009  Free Software Foundation Inc.
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

using System;
using CSUnit;

public class TestChar : TestCase
{
	// Constructor.
	public TestChar(String name)
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
	
	public void TestCharParseString0()
	{
		char c = Char.Parse("a");

		AssertEquals(c, 'a');
	}

	public void TestCharParseString2()
	{
		try
		{
			char c = Char.Parse("ab");
			Fail("Expected a FormatException");
		}
		catch(FormatException e)
		{
			// success
		}

	}

	public void TestCharParseStringNull()
	{
		try
		{
			char c = Char.Parse(null);
			Fail("Expected an ArgumentNullException");
		}
		catch(ArgumentNullException e)
		{
			// success
		}
	}

#if CONFIG_FRAMEWORK_2_0

	public void TestCharCompare()
	{
		Assert("'a'.CompareTo('a')", 'a'.CompareTo('a') == 0);
		Assert("'b'.CompareTo('a')", 'b'.CompareTo('a') > 0);
		Assert("'a'.CompareTo('b')", 'a'.CompareTo('b') < 0);
	}

	public void TestCharEauals()
	{
		Assert("'a'.Equals('a')", 'a'.Equals('a'));
		Assert("!('a'.Equals('b'))", !('a'.Equals('b')));
	}

#if !ECMA_COMPAT

	public void TestCharHighSurrogate()
	{
		char cLow = '\uDC00';
		char cHigh = '\uD800';

		Assert("!IsHighSurrogate('\uDC00')", !Char.IsHighSurrogate(cLow));
		Assert("IsHighSurrogate('\uD800')", Char.IsHighSurrogate(cHigh));
		Assert("!IsHighSurrogate('a')", !Char.IsHighSurrogate('a'));
	}

	public void TestCharLowSurrogate()
	{
		char cLow = '\uDC00';
		char cHigh = '\uD800';

		Assert("IsLowSurrogate('\uDC00')", Char.IsLowSurrogate(cLow));
		Assert("!IsLowSurrogate('\uD800')", !Char.IsLowSurrogate(cHigh));
		Assert("!IsLowSurrogate('a')", !Char.IsLowSurrogate('a'));
	}

#endif // !ECMA_COMPAT
#endif // CONFIG_FRAMEWORK_2_0

#if CONFIG_FRAMEWORK_2_0

	
#endif // CONFIG_FRAMEWORK_2_0

}; // class TestChar
