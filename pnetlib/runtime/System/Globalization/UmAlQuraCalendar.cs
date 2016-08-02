/*
 * UmAlQuraCalendar.cs - Implementation of the
 *        "System.Globalization.UmAlQuraCalendar" class.
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

namespace System.Globalization
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

using System;

// We wrap up the "HijriCalendar" class and adjust it to match the
// Um Al Qura requirements, since the two calendars are more or
// less identical.  Might as well keep the core of the calculations
// in the same place.

public class UmAlQuraCalendar : Calendar
{
	// The UmAlQura era.
	public static readonly int UmAlQuraEra = 1;

	// Internal state.
	private HijriCalendar hijri;

	// Constructor.
	public UmAlQuraCalendar()
			{
				hijri = new HijriCalendar();
				hijri.HijriAdjustment = 0;
			}

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [1];
					eras[0] = UmAlQuraEra;
					return eras;
				}
			}

	// Set the last year of a 100-year range for 2-digit processing.
	public override int TwoDigitYearMax
			{
				get
				{
					return hijri.TwoDigitYearMax;
				}
				set
				{
					hijri.TwoDigitYearMax = value;
				}
			}

	// Get the minimum DateTime value supported by this calendar.
	public override DateTime MinValue
			{
				get
				{
					return hijri.MinValue;
				}
			}

	// Get the maximum DateTime value supported by this calendar.
	public override DateTime MaxValue
			{
				get
				{
					return hijri.MaxValue;
				}
			}

	// Add a time period to a DateTime value.
	public override DateTime AddMonths(DateTime time, int months)
			{
				return hijri.AddMonths(time, months);
			}
	public override DateTime AddYears(DateTime time, int years)
			{
				return hijri.AddYears(time, years);
			}

	// Extract the components from a DateTime value.
	public override int GetDayOfMonth(DateTime time)
			{
				return hijri.GetDayOfMonth(time);
			}
	public override System.DayOfWeek GetDayOfWeek(DateTime time)
			{
				return hijri.GetDayOfWeek(time);
			}
	public override int GetDayOfYear(DateTime time)
			{
				return hijri.GetDayOfYear(time);
			}
	public override int GetMonth(DateTime time)
			{
				return hijri.GetMonth(time);
			}
	public override int GetYear(DateTime time)
			{
				return hijri.GetYear(time);
			}

	// Get the number of days in a particular month.
	public override int GetDaysInMonth(int year, int month, int era)
			{
				return hijri.GetDaysInMonth(year, month, era);
			}

	// Get the number of days in a particular year.
	public override int GetDaysInYear(int year, int era)
			{
				return hijri.GetDaysInYear(year, era);
			}

	// Get the era for a specific DateTime value.
	public override int GetEra(DateTime time)
			{
				return UmAlQuraEra;
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				return hijri.GetMonthsInYear(year, era);
			}

	// Determine if a particular day is a leap day.
	public override bool IsLeapDay(int year, int month, int day, int era)
			{
				return hijri.IsLeapDay(year, month, day, era);
			}

	// Determine if a particular month is a leap month.
	public override bool IsLeapMonth(int year, int month, int era)
			{
				return hijri.IsLeapMonth(year, month, era);
			}

	// Determine if a particular year is a leap year.
	public override bool IsLeapYear(int year, int era)
			{
				return hijri.IsLeapYear(year, era);
			}

	// Convert a particular time into a DateTime value.
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				return hijri.ToDateTime(year, month, day,
										hour, minute, second,
										millisecond, era);
			}

	// Convert a two-digit year value into a four-digit year value.
	public override int ToFourDigitYear(int year)
			{
				return hijri.ToFourDigitYear(year);
			}

}; // class UmAlQuraCalendar

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Globalization
