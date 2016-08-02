/*
 * NUnitEmulation.cs - Emulate enough of the NUnit test interface to
 *                     be able to run tests from the Mono class library.
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

namespace NUnit.Framework
{

using System;

public class TestResult : CSUnit.TestResult
{
	// Constructors.
	public TestResult() : base() {}

}; // class TestResult

public interface ITest : CSUnit.Test
{
}; // interface ITest

public abstract class TestCase : CSUnit.TestCase, ITest
{

	// Constructors.
	public TestCase() : this(String.Empty) {}
	public TestCase(String name) : base(name) {}

	// Create an NUnit-style result object.
	public override CSUnit.TestResult CreateResult()
			{
				return new TestResult();
			}

	// Set up for the test.
	protected virtual void SetUp() {}
	protected override void Setup()
			{
				SetUp();
			}

	// Tear down after the test.
	protected virtual void TearDown() {}
	protected override void Cleanup()
			{
				TearDown();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return Name;
			}

}; // class TestCase

public class TestSuite : CSUnit.TestSuite
{
	// Constructors.
	public TestSuite() : base(String.Empty) {}
	public TestSuite(String name) : base(name) {}
	public TestSuite(Type type, bool suppressWarnings)
			: base(type.Name, type) {}
	public TestSuite(Type type) : base(type.Name, type) {}

	// Add a test to this suite.
	public void AddTest(ITest test)
			{
				base.AddTest(test);
			}

	// Add another test suite to this one.
	public void AddTestSuite(Type type)
			{
				AddTest(new TestSuite(type));
			}

}; // class TestSuite

}; // namespace NUnit.Framework
