/*
 * TestStopwatch.cs - Tests for the "Stopwatch" class.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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
using System.Diagnostics;

#if CONFIG_FRAMEWORK_2_0 && CONFIG_EXTENDED_DIAGNOSTICS

public class TestStopwatch : TestCase
{
	// Constructor.
	public TestStopwatch(String name)
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

	// test static constructor
	public void TestStopwatchCCtor ()
			{
				bool isHighResolution = Stopwatch.IsHighResolution;
				long timestamp;

				if(isHighResolution)
				{
					Assert("Frequency != 0", Stopwatch.Frequency != 0);
				}
				else
				{
					AssertEquals( "Frequency", 10000000, Stopwatch.Frequency );
				}

				timestamp = Stopwatch.GetTimestamp();
				Assert("Timestamp increases", Stopwatch.GetTimestamp() > timestamp);

			}

	// test constructor
	public void TestStopwatchCtor ()
			{
				Stopwatch stopwatch;

				stopwatch = new Stopwatch();

				Assert("Not Running", !stopwatch.IsRunning);
				AssertEquals( "ElapsedTicks == 0", 0, stopwatch.ElapsedTicks );
				AssertEquals( "ElapsedMilliseconds == 0", 0, stopwatch.ElapsedMilliseconds );
				AssertEquals( "Elapsed == 0", new TimeSpan(0), stopwatch.Elapsed );

			}

	// test creating a running stopwatch
	public void TestStopwatchStartNew ()
			{
				Stopwatch stopwatch;
				long ticks;

				stopwatch = Stopwatch.StartNew();

				Assert("Running", stopwatch.IsRunning);

				stopwatch.Stop();

				Assert("Not Running", !stopwatch.IsRunning);

				ticks = stopwatch.ElapsedTicks;
				Assert( "ElapsedTicks > 0", ticks > 0 );
				// On fast systems less than one millisecond is elapsed
				if(ticks / (Stopwatch.Frequency / 1000) > 0)
				{
					Assert( "ElapsedMilliseconds > 0", stopwatch.ElapsedMilliseconds > 0 );
				}
				else
				{
					Assert( "ElapsedMilliseconds == 0", stopwatch.ElapsedMilliseconds == 0 );
				}
				Assert( "Elapsed > 0", stopwatch.Elapsed.Ticks > 0 );

			}

	public void TestStopwatch ()
			{
				Stopwatch stopwatch;
				long ticks;
				long ticks1;

				stopwatch = Stopwatch.StartNew();

				ticks = stopwatch.ElapsedTicks;
				Assert( "ElapsedTicks > ticks", stopwatch.ElapsedTicks > ticks );

				stopwatch.Stop();
				ticks = stopwatch.ElapsedTicks;
				AssertEquals( "ElapsedTicks == ticks", ticks, stopwatch.ElapsedTicks );

				stopwatch.Start();
				Assert( "ElapsedTicks > ticks after restart", stopwatch.ElapsedTicks > ticks );

				stopwatch.Reset();
				Assert("Not Running after Reset", !stopwatch.IsRunning);
				AssertEquals( "ElapsedTicks == 0 after Reset", 0, stopwatch.ElapsedTicks );
				AssertEquals( "ElapsedMilliseconds == 0 after Reset", 0, stopwatch.ElapsedMilliseconds );
				AssertEquals( "Elapsed == 0 after Reset", new TimeSpan(0), stopwatch.Elapsed );
			}

}; // class TestStopwatch

#endif // CONFIG_FRAMEWORK_2_0 && CONFIG_EXTENDED_DIAGNOSTICS
