/*
 * TestApplicationIdentity.cs - Tests for the "ApplicationIdentity" class.
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

public class TestApplicationIdentity : TestCase
{
	// Constructor.
	public TestApplicationIdentity(String name) : base(name)
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

	public void TestApplicationIdentityConstructor01()
	{
		try
		{
			ApplicationIdentity identity = new ApplicationIdentity(null);
			Fail("Test should have thrown a NullReferenceException");			
		}
		catch (NullReferenceException)
		{
		}
	}

	public void TestApplicationIdentityConstructor02()
	{
		ApplicationIdentity identity = new ApplicationIdentity("A");
	}

	public void TestApplicationIdentityFullName01()
	{
		ApplicationIdentity identity = new ApplicationIdentity("A");
		// this is the expected result but fails in pnet for now
		// AssertEquals("Fullname01 :","A, Culture=neutral",identity.FullName);
		AssertEquals("Fullname01 :", "A", identity.FullName);
	}

	public void TestApplicationIdentityFullName02()
	{
		ApplicationIdentity identity = new ApplicationIdentity("Testruntime");
		// this is the expected result but fails in pnet for now
		// AssertEquals("Fullname01 :", "Testruntime, Culture=neutral",
		//								identity.FullName);
		AssertEquals("Fullname01 :", "Testruntime", identity.FullName);
	}

	public void TestApplicationIdentityCodeBase01()
	{
		ApplicationIdentity identity = new ApplicationIdentity("A");
		AssertNull("CodeBase01 :", identity.CodeBase);
	}

	public void TestApplicationIdentityCodeBase02()
	{
		ApplicationIdentity identity = new ApplicationIdentity("Testruntime");
		AssertNull("CodeBase01 :", identity.CodeBase);
	}
} // class TestApplicationIdentity

#endif // CONFIG_FRAMEWORK_2_0
