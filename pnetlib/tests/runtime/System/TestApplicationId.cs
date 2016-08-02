/*
 * TestApplicationId.cs - Tests for the "ApplicationId" class.
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

#if CONFIG_FRAMEWORK_2_0

public class TestApplicationId : TestCase
{
	// Constructor.
	public TestApplicationId(String name) : base(name)
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

	public void TestApplicationIdConstructor01()
	{
		try
		{
			ApplicationId id = new ApplicationId(null, null, null, null, null);
			Fail("Test should have thrown an ArgumentNullException");			
		}
		catch (ArgumentNullException)
		{
		}
	}

	public void TestApplicationIdConstructor02()
	{
		try
		{
			ApplicationId id = new ApplicationId(null, "Test", null, null, null);
			Fail("Test should have thrown an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
		}
	}

	public void TestApplicationIdConstructor03()
	{
		try
		{
			ApplicationId id = new ApplicationId(null, "Test", new Version(), null, null);
			Fail("Test should have thrown an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
		}
	}

	public void TestApplicationIdConstructor04()
	{
		ApplicationId id = new ApplicationId(new Byte[0], "Test", new Version(), null, null);
	}

	public void TestApplicationIdConstructor05()
	{
		ApplicationId id = new ApplicationId(new Byte[0], "Test", new Version(), "Test", null);
	}

	public void TestApplicationIdConstructor06()
	{
		ApplicationId id = new ApplicationId(new Byte[0], "Test", new Version(), null, "Test");
	}

	public void TestApplicationIdPublicKeyToken01()
	{
		Byte[] publicKeyToken = {0, 1, 2, 3, 4};
		ApplicationId id = new ApplicationId(publicKeyToken, "Test", new Version(), null, null);
		AssertEquals("Index 0:", publicKeyToken[0], id.PublicKeyToken[0]);
		AssertEquals("Index 1:", publicKeyToken[1], id.PublicKeyToken[1]);
		AssertEquals("Index 2:", publicKeyToken[2], id.PublicKeyToken[2]);
		AssertEquals("Index 3:", publicKeyToken[3], id.PublicKeyToken[3]);
		AssertEquals("Index 4:", publicKeyToken[4], id.PublicKeyToken[4]);
	}

	public void TestApplicationIdPublicKeyToken02()
	{
		Byte[] publicKeyToken = {0, 1, 2, 3, 4};
		ApplicationId id = new ApplicationId(publicKeyToken, "Test", new Version(), null, null);
		publicKeyToken[0] = (Byte)10;
		AssertEquals("Index 0:", 0, id.PublicKeyToken[0]);
	}

	public void TestApplicationIdPublicKeyToken03()
	{
		Byte[] publicKeyToken = {0, 1, 2, 3, 4};
		ApplicationId id = new ApplicationId(publicKeyToken, "Test", new Version(), null, null);
		id.PublicKeyToken[0] = (Byte)10;
		AssertEquals("Index 0:", 0, id.PublicKeyToken[0]);
	}

	public void TestApplicationIdToString01()
	{
		Byte[] publicKeyToken = {0, 1, 2, 3, 4};
		ApplicationId id = new ApplicationId(publicKeyToken, "Test", new Version(), null, null);
		AssertEquals("ToString 01:", "Test, version=\"0.0\", publicKeyToken=\"0001020304\"", id.ToString());
	}

	public void TestApplicationIdToString02()
	{
		Byte[] publicKeyToken = {0, 1, 2, 3, 4};
		ApplicationId id = new ApplicationId(publicKeyToken, "Test", new Version(), "Invalid", null);
		AssertEquals("ToString 01:", "Test, version=\"0.0\", publicKeyToken=\"0001020304\", processorArchitecture =\"Invalid\"", id.ToString());
	}

	public void TestApplicationIdToString03()
	{
		Byte[] publicKeyToken = {0, 1, 2, 3, 4};
		ApplicationId id = new ApplicationId(publicKeyToken, "Test", new Version(), null, "Invalid");
		AssertEquals("ToString 01:", "Test, culture=\"Invalid\", version=\"0.0\", publicKeyToken=\"0001020304\"", id.ToString());
	}
} // class TestApplicationIdentity

#endif // CONFIG_FRAMEWORK_2_0
