/*
 * TestCalendar.cs - Tests for the "Calendar" class.
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
using System.Globalization;

public abstract class TestCalendar : TestCase
{
	// Calendar instance to test.
	protected Calendar calendar;

	// Expected value for TwoDigitYearMax.
	protected int twoDigitYearMax;

	// Constructor.
	public TestCalendar(String name)
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

	// Print the type name when running the test, to distinguish
	// the various kinds of calendars.
	protected override void RunTest()
			{
				Console.Write(calendar.GetType().Name);
				Console.Write(" ... ");
			#if !CONFIG_SMALL_CONSOLE
				Console.Out.Flush();
			#endif
				base.RunTest();
			}

	// Test the era information.
	public void TestCalendarEras()
			{
				// There should be one era, of value 1, in each calendar.
				int[] eras = calendar.Eras;
				AssertNotNull("Eras (1)", eras);
				AssertEquals("Eras (2)", 1, eras.Length);
				AssertEquals("Eras (3)", 1, eras[0]);
			}

	// Test 4-digit year conversions.
	public void TestCalendarToFourDigitYear()
			{
				int max = calendar.TwoDigitYearMax;
				AssertEquals("TwoDigitYearMax", twoDigitYearMax, max);
				int year, expected;
				for(year = 0; year < 100; ++year)
				{
					expected = max % 100;
					if(year <= expected)
						expected = max - expected + year;
					else
						expected = max - expected - 100 + year;
					AssertEquals("ToFourDigitYear", expected,
								 calendar.ToFourDigitYear(year));
				}
			}

	// Test the "GetEra" method.
	public void TestCalendarGetEra()
			{
				AssertEquals("GetEra (1)", 1,
							 calendar.GetEra(DateTime.MinValue));
				AssertEquals("GetEra (2)", 1,
							 calendar.GetEra(DateTime.MaxValue));
				AssertEquals("GetEra (3)", 1,
							 calendar.GetEra(DateTime.Now));
			}

	// Test the "AddXXX" methods that are common between all calendars.
	public void TestCalendarAddDays()
			{
				long multiplier = TimeSpan.TicksPerDay;
				DateTime time = DateTime.Now;

				TimeSpan ts = calendar.AddDays(time, -3) - time;
				AssertEquals("AddDays (1)", multiplier * -3, ts.Ticks);

				ts = calendar.AddDays(time, 0) - time;
				AssertEquals("AddDays (2)", multiplier * 0, ts.Ticks);

				ts = calendar.AddDays(time, 145) - time;
				AssertEquals("AddDays (3)", multiplier * 145, ts.Ticks);
			}
	public void TestCalendarAddHours()
			{
				long multiplier = TimeSpan.TicksPerHour;
				DateTime time = DateTime.Now;

				TimeSpan ts = calendar.AddHours(time, -3) - time;
				AssertEquals("AddHours (1)", multiplier * -3, ts.Ticks);

				ts = calendar.AddHours(time, 0) - time;
				AssertEquals("AddHours (2)", multiplier * 0, ts.Ticks);

				ts = calendar.AddHours(time, 145) - time;
				AssertEquals("AddHours (3)", multiplier * 145, ts.Ticks);
			}
#if CONFIG_EXTENDED_NUMERICS
	public void TestCalendarAddMilliseconds()
			{
				long multiplier = TimeSpan.TicksPerMillisecond;
				DateTime time = DateTime.Now;

				TimeSpan ts = calendar.AddMilliseconds(time, -3) - time;
				AssertEquals("AddMilliseconds (1)", multiplier * -3, ts.Ticks);

				ts = calendar.AddMilliseconds(time, 0) - time;
				AssertEquals("AddMilliseconds (2)", multiplier * 0, ts.Ticks);

				ts = calendar.AddMilliseconds(time, 145) - time;
				AssertEquals("AddMilliseconds (3)", multiplier * 145, ts.Ticks);
			}
#endif
	public void TestCalendarAddMinutes()
			{
				long multiplier = TimeSpan.TicksPerMinute;
				DateTime time = DateTime.Now;

				TimeSpan ts = calendar.AddMinutes(time, -3) - time;
				AssertEquals("AddMinutes (1)", multiplier * -3, ts.Ticks);

				ts = calendar.AddMinutes(time, 0) - time;
				AssertEquals("AddMinutes (2)", multiplier * 0, ts.Ticks);

				ts = calendar.AddMinutes(time, 145) - time;
				AssertEquals("AddMinutes (3)", multiplier * 145, ts.Ticks);
			}
	public void TestCalendarAddSeconds()
			{
				long multiplier = TimeSpan.TicksPerSecond;
				DateTime time = DateTime.Now;

				TimeSpan ts = calendar.AddSeconds(time, -3) - time;
				AssertEquals("AddSeconds (1)", multiplier * -3, ts.Ticks);

				ts = calendar.AddSeconds(time, 0) - time;
				AssertEquals("AddSeconds (2)", multiplier * 0, ts.Ticks);

				ts = calendar.AddSeconds(time, 145) - time;
				AssertEquals("AddSeconds (3)", multiplier * 145, ts.Ticks);
			}
	public void TestCalendarAddWeeks()
			{
				long multiplier = TimeSpan.TicksPerDay * 7;
				DateTime time = DateTime.Now;

				TimeSpan ts = calendar.AddWeeks(time, -3) - time;
				AssertEquals("AddWeeks (1)", multiplier * -3, ts.Ticks);

				ts = calendar.AddWeeks(time, 0) - time;
				AssertEquals("AddWeeks (2)", multiplier * 0, ts.Ticks);

				ts = calendar.AddWeeks(time, 145) - time;
				AssertEquals("AddWeeks (3)", multiplier * 145, ts.Ticks);
			}

	// Test the "GetXXX" methods that are common between all calendars.
	public void TestCalendarGetHour()
			{
				DateTime time = DateTime.Now;
				AssertEquals("GetHour (1)", 0,
							 calendar.GetHour(DateTime.MinValue));
				AssertEquals("GetHour (2)", 23,
							 calendar.GetHour(DateTime.MaxValue));
				AssertEquals("GetHour (3)", time.Hour,
							 calendar.GetHour(time));
			}
#if CONFIG_EXTENDED_NUMERICS
	public void TestCalendarGetMilliseconds()
			{
				DateTime time = DateTime.Now;
				AssertEquals("GetMilliseconds (1)", 0.0,
							 calendar.GetMilliseconds(DateTime.MinValue), 0.1);
				AssertEquals("GetMilliseconds (2)", 999.9999,
							 calendar.GetMilliseconds(DateTime.MaxValue), 0.1);
				AssertEquals("GetMilliseconds (3)", time.Millisecond,
							 calendar.GetMilliseconds(time), 1.0);
			}
#endif
	public void TestCalendarGetMinute()
			{
				DateTime time = DateTime.Now;
				AssertEquals("GetMinute (1)", 0,
							 calendar.GetMinute(DateTime.MinValue));
				AssertEquals("GetMinute (2)", 59,
							 calendar.GetMinute(DateTime.MaxValue));
				AssertEquals("GetMinute (3)", time.Minute,
							 calendar.GetMinute(time));
			}
	public void TestCalendarGetSecond()
			{
				DateTime time = DateTime.Now;
				AssertEquals("GetSecond (1)", 0,
							 calendar.GetSecond(DateTime.MinValue));
				AssertEquals("GetSecond (2)", 59,
							 calendar.GetSecond(DateTime.MaxValue));
				AssertEquals("GetSecond (3)", time.Second,
							 calendar.GetSecond(time));
			}

}; // class TestCalendar
