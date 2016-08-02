/*
 * TestVersion.cs - Tests for the "Version" class.
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

public class TestVersion : TestCase
{
	// Constructor.
	public TestVersion(String name) : base(name)
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

	public void TestVersionConstructor01()
	{
		Version version = new Version();
		AssertEquals("Major 1:", 0, version.Major);
		AssertEquals("Minor 1:", 0, version.Minor);
		AssertEquals("Build 1:", -1, version.Build);
		AssertEquals("Revision 1:", -1, version.Revision);
	}

	public void TestVersionConstructor02()
	{
		Version version = new Version(1, 2);
		AssertEquals("Major 2:", 1, version.Major);
		AssertEquals("Minor 2:", 2, version.Minor);
		AssertEquals("Build 2:", -1, version.Build);
		AssertEquals("Revision 2:", -1, version.Revision);
	}

	public void TestVersionConstructor03()
	{
		Version version = new Version(1, 2, 3);
		AssertEquals("Major 3:", 1, version.Major);
		AssertEquals("Minor 3:", 2, version.Minor);
		AssertEquals("Build 3:", 3, version.Build);
		AssertEquals("Revision 3:", -1, version.Revision);
	}

	public void TestVersionConstructor04()
	{
		Version version = new Version(1, 2, 3, 4);
		AssertEquals("Major 4:", 1, version.Major);
		AssertEquals("Minor 4:", 2, version.Minor);
		AssertEquals("Build 4:", 3, version.Build);
		AssertEquals("Revision 4:", 4, version.Revision);
	}

	public void TestVersionConstructor05()
	{
		Version version = new Version("1.2.3.4");
		AssertEquals("Major 5:", 1, version.Major);
		AssertEquals("Minor 5:", 2, version.Minor);
		AssertEquals("Build 5:", 3, version.Build);
		AssertEquals("Revision 5:", 4, version.Revision);
	}

	public void TestVersionConstructor06()
	{
		try
		{
			Version version = new Version("A.2.3.4");
			Fail("Test should have thrown a FormatException");
		}
		catch (FormatException)
		{
			// SUCCESS
		}
	}

	public void TestVersionConstructor07()
	{
		try
		{
			Version version = new Version(null);
			Fail("Test should have thrown am ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// SUCCESS
		}
	}

	public void TestVersionConstructor08()
	{
		try
		{
			Version version = new Version("1");
			Fail("Test should have thrown an ArgumentException");
		}
		catch (ArgumentException)
		{
			// SUCCESS
		}
	}

	public void TestVersionConstructor09()
	{
		Version version = new Version("1.2");
		AssertEquals("Major 9:", 1, version.Major);
		AssertEquals("Minor 9:", 2, version.Minor);
		AssertEquals("Build 9:", -1, version.Build);
		AssertEquals("Revision 9:", -1, version.Revision);
	}

	public void TestVersionConstructor10()
	{
		Version version = new Version("1.2.3");
		AssertEquals("Major 10:", 1, version.Major);
		AssertEquals("Minor 10:", 2, version.Minor);
		AssertEquals("Build 10:", 3, version.Build);
		AssertEquals("Revision 10:", -1, version.Revision);
	}

	public void TestVersionConstructor11()
	{
		Version version = new Version("1.2.3.4");
		AssertEquals("Major 11:", 1, version.Major);
		AssertEquals("Minor 11:", 2, version.Minor);
		AssertEquals("Build 11:", 3, version.Build);
		AssertEquals("Revision 11:", 4, version.Revision);
	}

	public void TestVersionConstructor12()
	{
		try
		{
			Version version = new Version("1.-1.3.4");
			Fail("Test should have thrown an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// SUCCESS
		}
	}

	public void TestVersionToString01()
	{
		Version version = new Version("1.2");
		AssertEquals("ToString 1:", "1.2", version.ToString());
	}

	public void TestVersionToString02()
	{
		Version version = new Version("1.2.3");
		AssertEquals("ToString 2:", "1.2.3", version.ToString());
	}

	public void TestVersionToString03()
	{
		Version version = new Version("1.2.3.4");
		AssertEquals("ToString 3:", "1.2.3.4", version.ToString());
	}

} // class TestVersion


