/*
 * TestSecurityExceptions.cs - Test various exception classes.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using System.Security;

public class TestSecurityExceptions : TestCase
{
	// Constructor.
	public TestSecurityExceptions(String name)
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

	// Test the SecurityException class.
	public void TestSecurityException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(SecurityException), unchecked((int)0x8013150a));
			}

	// Test the VerificationException class.
	public void TestVerificationException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(VerificationException), unchecked((int)0x8013150d));
			}

#if !ECMA_COMPAT && (CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS)

	// Test the XmlSyntaxException class.
	public void TestXmlSyntaxException()
			{
				// Check the three main exception constructors.
				ExceptionTester.CheckMain
					(typeof(XmlSyntaxException), unchecked((int)0x80131418));
			}

#endif

}; // class TestSecurityExceptions
