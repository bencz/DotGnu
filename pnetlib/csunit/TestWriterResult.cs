/*
 * TestWriterResult.cs - Implementation of the "CSUnit.TestWriterResult" class.
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
using System.IO;

public class TestWriterResult : TestResult
{
	// Internal state.
	private TextWriter writer;
	private bool stopAtFail;
	private bool showOnlyFailed;
	private bool sawFailure;
	private int numFailures;

	// Constructor.
	public TestWriterResult(TextWriter writer,
							bool stopAtFail,
							bool showOnlyFailed)
			: base()
			{
				this.writer = writer;
				this.stopAtFail = stopAtFail;
				this.showOnlyFailed = showOnlyFailed;
				sawFailure = false;
				numFailures = 0;
			}

	// Print the name of a test suite.
	private void PrintSuiteName(String name)
			{
				if(name != null)
				{
					writer.WriteLine();
					writer.Write("Suite: ");
					writer.WriteLine(name);
					writer.WriteLine();
				}
			}

	// Start a new test.
	public override void StartTest(Test test, bool isSuite)
			{
				base.StartTest(test, isSuite);
				if(isSuite)
				{
					if(!showOnlyFailed)
					{
						PrintSuiteName(test.Name);
					}
				}
				else if(!showOnlyFailed)
				{
					writer.Write(test.Name);
					writer.Write(" ... ");
					writer.Flush();
				}
				sawFailure = false;
			}

	// End the current test.
	public override void EndTest(Test test, bool isSuite)
			{
				if(sawFailure)
				{
					if(stopAtFail)
					{
						// Stop testing immediately.
						throw new TestStop();
					}
				}
				else if(!isSuite && !showOnlyFailed)
				{
					writer.WriteLine("ok");
				}
				sawFailure = false;
			}

	// List a test.
	public override void ListTest(Test test, bool isSuite)
			{
				if(isSuite)
				{
					PrintSuiteName(test.Name);
				}
				else
				{
					writer.WriteLine(test.Name);
				}
			}

	// Record an assertion failure.
	public override void AddFailure(Test test, Exception e)
			{
				base.AddFailure(test, e);
				++numFailures;
				sawFailure = true;
				if(showOnlyFailed)
				{
					writer.Write(test.Name);
					writer.Write(" ... ");
				}
				writer.Write("failed: ");
				writer.WriteLine(e.ToString());
			}

	// Record an uncaught exception.
	public override void AddException(Test test, Exception e)
			{
				base.AddException(test, e);
				++numFailures;
				sawFailure = true;
				if(showOnlyFailed)
				{
					writer.Write(test.Name);
					writer.Write(" ... ");
				}
				writer.Write("threw exception: ");
				writer.WriteLine(e.ToString());
			}

	// Report result summary information.
	public override void ReportSummary()
			{
				if(!showOnlyFailed || numFailures > 0)
				{
					writer.WriteLine();
					writer.WriteLine(numTestsRun + " tests run, " +
									 numFailures + " tests failed");
				}
			}

	// Determine if we had failures.
	public override bool HadFailures
			{
				get
				{
					return (numFailures > 0);
				}
			}

}; // class TestWriterResult

}; // namespace CSUnit
