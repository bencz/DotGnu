/*
 * TestResult.cs - Implementation of the "CSUnit.TestResult" class.
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

public class TestResult
{

	// Internal state.
	protected int numTestsRun;
	protected TestArray failures;
	protected TestArray errors;

	// Constructor.
	public TestResult()
			{
				numTestsRun = 0;
				failures = new TestArray();
				errors = new TestArray();
			}

	// Start a new test.
	public virtual void StartTest(Test test, bool isSuite)
			{
				if(!isSuite)
				{
					++numTestsRun;
				}
			}

	// End the current test.
	public virtual void EndTest(Test test, bool isSuite)
			{
				// Nothing to do here.
			}

	// List a test.
	public virtual void ListTest(Test test, bool isSuite)
			{
				// Nothing to do here.
			}

	// Record an assertion failure.
	public virtual void AddFailure(Test test, Exception e)
			{
				failures.Add(new TestFailure(test, e));
			}

	// Record an uncaught exception.
	public virtual void AddException(Test test, Exception e)
			{
				errors.Add(new TestFailure(test, e));
			}

	// Report result summary information.
	public virtual void ReportSummary()
			{
				// Nothing to do here.
			}

	// Determine if we had failures.
	public virtual bool HadFailures
			{
				get
				{
					return false;
				}
			}

}; // class TestResult

}; // namespace CSUnit
