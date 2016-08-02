/*
 * TestGuid.cs - Tests for the "ActivationArguments" class.
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

public class TestActivationArguments : TestCase
{
	// Constructor.
	public TestActivationArguments(String name) : base(name)
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

	public void TestActivationArgumentsConstructor01()
	{
		try
		{
			ActivationArguments activationArguments = 
									new ActivationArguments(null);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor02()
	{
		try
		{
			ActivationArguments activationArguments =
									new ActivationArguments(null, null);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor03()
	{
		try
		{
			ActivationArguments activationArguments =
									new ActivationArguments(null, null, null);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor04()
	{
		try
		{
			ActivationArguments activationArguments =
									new ActivationArguments("Test");
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor05()
	{
		try
		{
			ActivationArguments activationArguments =
									new ActivationArguments("Test", null);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor06()
	{
		try
		{
			ActivationArguments activationArguments =
						new ActivationArguments("Test", new String[] {null});
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor07()
	{
		try
		{
			ActivationArguments activationArguments =
				new ActivationArguments("Test", new String[] {null, null});
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor08()
	{
		try
		{
			ActivationArguments activationArguments =
			new ActivationArguments("Test", new String[] {null, null, null});
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor09()
	{
		try
		{
			ActivationArguments activationArguments =
				new ActivationArguments("Test", new String[] {"A", "B", "C"});
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor10()
	{
		try
		{
			ActivationArguments activationArguments =
							new ActivationArguments("Test",
										new String[] {"A", "B", "C"}, null);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsConstructor11()
	{
		try
		{
			ActivationArguments activationArguments =
							new ActivationArguments("Test",
											new String[] { "A", "B", "C" },
											new String[] { null });
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name);
		}
	}

	public void TestActivationArgumentsFullName01()
	{
		ActivationArguments activationArguments;
		try
		{
			activationArguments = new ActivationArguments("Test");
			AssertEquals("Fullname 1:",
							activationArguments.ApplicationFullName, "Test");
			activationArguments.ApplicationFullName = "Test1";
			AssertEquals("Fullname 2:",
							activationArguments.ApplicationFullName, "Test1");
			AssertNull("Fullname 3:",
						activationArguments.ApplicationManifestPaths);
			AssertNull("Fullname 4:", activationArguments.ActivationData);
			try
			{
				activationArguments.ApplicationFullName = null;
				Fail("Test Fullname 3 should have thrown an ArgumentNullException");
			}
			catch (ArgumentNullException)
			{
				// success
			}
			activationArguments.ApplicationFullName = String.Empty;
			AssertEquals("Fullname 2:",
							activationArguments.ApplicationFullName,
							String.Empty);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name +
					" at\n" + e.StackTrace);			
		}
	}

	public void TestActivationArgumentsManifestPaths01()
	{
		ActivationArguments activationArguments;
		try
		{
			activationArguments = new ActivationArguments("Test", null);
			AssertNull("ApplicationManifestPaths 1:",
						activationArguments.ApplicationManifestPaths);
			activationArguments.ApplicationManifestPaths = null;
			AssertNull("ApplicationManifestPaths 2:",
						activationArguments.ApplicationManifestPaths);
			// MS docs say this should be an Array with two items but it
			// does not throw an exception in this case
			activationArguments.ApplicationManifestPaths =
													new String[] { null };
			AssertNotNull("ApplicationManifestPaths 3:",
							activationArguments.ApplicationManifestPaths);
			AssertEquals("ApplicationManifestPaths 4:",
						activationArguments.ApplicationManifestPaths.Length,
						1); 
			AssertNull("ApplicationManifestPaths 5:",
							activationArguments.ApplicationManifestPaths[0]);
			activationArguments.ApplicationManifestPaths =
												new String[] { null, null };
			AssertNotNull("ApplicationManifestPaths 6:",
								activationArguments.ApplicationManifestPaths);
			AssertEquals("ApplicationManifestPaths 7:",
						activationArguments.ApplicationManifestPaths.Length,
						2); 
			AssertNull("ApplicationManifestPaths 8:",
						activationArguments.ApplicationManifestPaths[0]);
			AssertNull("ApplicationManifestPaths 9:",
						activationArguments.ApplicationManifestPaths[1]);
			// MS docs say this should be an Array with two items but
			// it does not throw an exception in this case
			activationArguments.ApplicationManifestPaths =
										new String[] { null, null, null };
			AssertNotNull("ApplicationManifestPaths 10:",
								activationArguments.ApplicationManifestPaths);
			AssertEquals("ApplicationManifestPaths 11:",
						activationArguments.ApplicationManifestPaths.Length,
						3); 
			AssertNull("ApplicationManifestPaths 12:",
						activationArguments.ApplicationManifestPaths[0]);
			AssertNull("ApplicationManifestPaths 13:",
						activationArguments.ApplicationManifestPaths[1]);
			AssertNull("ApplicationManifestPaths 13:",
						activationArguments.ApplicationManifestPaths[2]);
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name +
											" at\n" + e.StackTrace);
		}
	}

	public void TestActivationArgumentsManifestPaths02()
	{
		ActivationArguments activationArguments;
		String[] strings = { "A", "B" };

		try
		{
			activationArguments = new ActivationArguments("Test", strings);
			AssertEquals("activationArguments 1:",
					activationArguments.ApplicationManifestPaths.Length, 2); 
			AssertEquals("activationArguments 2:",
						activationArguments.ApplicationManifestPaths[0],
						strings[0]); 
			AssertEquals("activationArguments 3:",
						activationArguments.ApplicationManifestPaths[1],
						strings[1]); 
			strings[0] = "C";
			AssertEquals("activationArguments 3:",
						activationArguments.ApplicationManifestPaths[0],
						"C"); 
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name +
												" at\n" + e.StackTrace);
		}
	}

	public void TestActivationArgumentsActivationData01()
	{
		ActivationArguments activationArguments;
		try
		{
			activationArguments = new ActivationArguments("Test", null, null);
			AssertNull("ActivationData 1:",
						activationArguments.ActivationData);
			activationArguments.ActivationData = null;
			AssertNull("ActivationData 2:",
						activationArguments.ActivationData);
			activationArguments.ActivationData = new String[] { "A" };
			AssertNotNull("ActivationData 3:",
							activationArguments.ActivationData);
			AssertEquals("ActivationData 4:",
							activationArguments.ActivationData.Length, 1); 
			AssertEquals("ActivationData 5:",
							activationArguments.ActivationData[0], "A"); 
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name +
											" at\n" + e.StackTrace);
		}
	}

	public void TestActivationArgumentsActivationData02()
	{
		ActivationArguments activationArguments;
		String[] strings = { "A", "B" };

		try
		{
			activationArguments =
						new ActivationArguments("Test", null, strings);
			AssertEquals("ActivationData 1:",
							activationArguments.ActivationData.Length, 2); 
			AssertEquals("ActivationData 2:",
							activationArguments.ActivationData[0], strings[0]); 
			AssertEquals("ActivationData 3:",
							activationArguments.ActivationData[1], strings[1]); 
			strings[0] = "C";
			AssertEquals("ActivationData 4:",
							activationArguments.ActivationData[0], "C"); 
		}
		catch (Exception e)
		{
			Fail("Test should not have thrown an " + e.GetType().Name +
												" at\n" + e.StackTrace);
		}
	}

} // class TestActivationArguments

#endif // CONFIG_FRAMEWORK_2_0
