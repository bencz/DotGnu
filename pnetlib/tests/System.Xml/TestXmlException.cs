/*
 * TestString.cs - Tests for the "System.String" class.
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 *
 * Authors : Stephen Compall, Gopal.V, & Richard Baumann
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
using System.Reflection;
using System.Resources;
using System.Xml;

public class TestXmlException : TestCase
{
	// Constructor.
	public TestXmlException(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// Set up for the tests.
	protected override void Setup()
			{
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}
	
	//Methods
	public void TestXmlExceptionEmptyConstructor()
	{
		XmlException x = new XmlException();

		AssertNotNull(x.Message);
	}

	public void TestXmlExceptionStringConstructor()
	{
		XmlException x = new XmlException("foo_for_the_masses");
		AssertEquals(x.Message,"foo_for_the_masses");
	}
}; // class TestString
