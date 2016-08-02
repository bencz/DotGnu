/*
 * Test.cs - Implementation of the "CSUnit.Test" interface.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace CSUnit
{

using System;

public interface Test
{

	// Run this test and send its results to a specified object.
	void Run(TestResult result);

	// Find a test by name.  If this object is a test case,
	// returns "this" if the name matches, or null otherwise.
	// If this object is a test suite, and the name does not
	// match the suite name, then recursively search for
	// a match within the suite's tests.
	Test Find(String name);

	// List this test to a TestResult object, but don't run it.
	void List(TestResult result);

	// Get the name of this test.
	String Name { get; }

}; // class Test

}; // namespace CSUnit
