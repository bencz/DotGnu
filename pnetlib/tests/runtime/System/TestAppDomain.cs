/*
 * TestAppDomain.cs - Tests for the "AppDomain" class.
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

public class TestAppDomain : TestCase
{
	// Constructor.
	public TestAppDomain(String name) : base(name)
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

	public void TestAppDomainCreate01()
	{
		AppDomain domain = AppDomain.CreateDomain("test");

		AppDomain.Unload(domain);
		
	}

	public void TestAppDomainCreateFriendlyName()
	{
		String friendlyName = "test";
		AppDomain domain = AppDomain.CreateDomain(friendlyName);

		try
		{
			AssertEquals("FriendlyName", friendlyName, domain.FriendlyName);
		}
		finally
		{
			AppDomain.Unload(domain);
		}
	}

	public void TestAppDomainFriendlyNameUnloaded()
	{
		String friendlyName = "test";
		AppDomain domain = AppDomain.CreateDomain(friendlyName);

		AppDomain.Unload(domain);
		try
		{
			AssertEquals("FriendlyName", friendlyName, domain.FriendlyName);
			Fail("Should have thrown an AppDomainUnloadedException !");
		}
		catch(AppDomainUnloadedException)
		{
			// SUCCESS
		}
	}

	public void TestAppDomainCurrentName01()
	{
		AppDomain domain = AppDomain.CurrentDomain;
	
		AssertEquals("Current Appdomain.FriendlyName", "csunit.exe", domain.FriendlyName);
	}

	public void TestAppDomainAssemblies01()
	{
		AppDomain appDomain = AppDomain.CreateDomain("Test1");
		try
		{
			System.Reflection.Assembly[] assemblies = appDomain.GetAssemblies();
			AssertEquals("AppDomainAssemblies 01 Length", 1, assemblies.Length);
			AssertEquals("AppDomainAssemblies 01 Name", "mscorlib", assemblies[0].GetName().Name);
		}
		finally
		{
			AppDomain.Unload(appDomain);
		}
	}

	public void TestAppDomainUnloadDefault()
	{
		AppDomain domain = AppDomain.CurrentDomain;

		try
		{
			AppDomain.Unload(domain);
			Fail("Should have thrown a CannotUnloadAppDomainException !");
		}
		catch(CannotUnloadAppDomainException)
		{
			// SUCCESS
		}
	}

} // class TestAppDomain


