/*
 * HebrewCalendar.cs - Implementation of the
 *        "System.Globalization.HebrewCalendar" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System;

public class HebrewCalendar : Calendar
{

	// Hebrew era value.
	public static readonly int HebrewEra = 1;

	// Useful internal constants.
	private const int DefaultTwoDigitYearMax = 5790;
	private const int MinYear = 5343;
	private const int MaxYear = 6000;

	// Gregorian 0001/01/01 is Hebrew 3761/10/18.  This constant
	// indicates the number of days to use to offset a Gregorian
	// day number to turn it into a Hebrew day number.
	private const long Year1ADDays = 1373428;

	// There are 1080 "parts" per hour.
	private const int PartsPerHour = 1080;

	// Number of "parts" in a day.
	private const int PartsPerDay = 24 * PartsPerHour;

	// Length of a lunar month in days and parts.
	private const int DaysPerMonth = 29;
	private const int DaysPerMonthFraction = 12 * PartsPerHour + 793;
	private const int DaysPerMonthParts =
			DaysPerMonth * PartsPerDay + DaysPerMonthFraction;

	// The time of the new moon in parts on the first day in the
	// Hebrew calendar (1 Tishri, year 1 which is approx 6 Oct 3761 BC).
	private const int FirstNewMoon = 11 * PartsPerHour + 204;

	// Number of days in each month for deficient, regular,
	// and complete years in both normal and leap variants.
	private static readonly int[] daysPerMonthDeficient =
			{30, 29, 29, 29, 30, 29, 30, 29, 30, 29, 30, 29};
	private static readonly int[] daysPerMonthDeficientLeap =
			{30, 29, 29, 29, 30, 30, 29, 30, 29, 30, 29, 30, 29};
	private static readonly int[] daysPerMonthRegular =
			{30, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29};
	private static readonly int[] daysPerMonthRegularLeap =
			{30, 29, 30, 29, 30, 30, 29, 30, 29, 30, 29, 30, 29};
	private static readonly int[] daysPerMonthComplete =
			{30, 30, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29};
	private static readonly int[] daysPerMonthCompleteLeap =
			{30, 30, 30, 29, 30, 30, 29, 30, 29, 30, 29, 30, 29};

	// Constructor.
	public HebrewCalendar()
			{
				// Nothing to do here.
			}

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [1];
					eras[0] = HebrewEra;
					return eras;
				}
			}

	// Set the last year of a 100-year range for 2-digit processing.
	public override int TwoDigitYearMax
			{
				get
				{
					int value = base.TwoDigitYearMax;
					if(value != -1)
					{
						return value;
					}
					base.TwoDigitYearMax = DefaultTwoDigitYearMax;
					return DefaultTwoDigitYearMax;
				}
				set
				{
					base.TwoDigitYearMax = value;
				}
			}

#if CONFIG_FRAMEWORK_1_2

	// Get the minimum DateTime value supported by this calendar.
	public override DateTime MinValue
			{
				get
				{
					return ToDateTime(MinYear, 1, 1, 0, HebrewEra);
				}
			}

	// Get the maximum DateTime value supported by this calendar.
	public override DateTime MaxValue
			{
				get
				{
					return GetMaxValue(MaxYear);
				}
			}

#endif

	// Determine if a year value is a Hebrew leap year.
	private static bool IsHebrewLeapYear(int year)
			{
				switch(year % 19)
				{
					case 0: case 3: case 6: case 8:
					case 11: case 14: case 17:
						return true;

					default:
						return false;
				}
			}

	// Get the absolute day number that starts a particular year.
	// Based on the algorithm used by "http://www.funaba.org/en/calendar.html".
	private static long StartOfYear(int year)
			{
				// Get the number of months before the year.
				int months = (235 * year - 234) / 19;

				// Calculate the day number in whole and fractional parts.
				long fraction = ((long)months) * ((long)DaysPerMonthFraction) +
								((long)FirstNewMoon);
				long day = ((long)months) * 29 + (fraction / PartsPerDay);
				fraction %= PartsPerDay;

				// Adjust the start of the year for the day of the week.
				// The value "weekday == 0" indicates Monday.
				int weekday = (int)(day % 7);
				if(weekday == 2 || weekday == 4 || weekday == 6)
				{
					// Cannot start on a Sunday, Wednesday, or Friday.
					++day;
					weekday = (int)(day % 7);
				}
				if(weekday == 1 &&
				   fraction > (15 * PartsPerHour + 204) &&
				   !IsHebrewLeapYear(year))
				{
					// If the new moon falls after "15 hours, 204 parts"
					// from the previous noon on a Tuesday and it is not
					// a leap year, postpone by 2 days.
					day += 2;
				}
				else if(weekday == 0 &&
				 		fraction > (21 * PartsPerHour + 589) &&
						IsHebrewLeapYear(year - 1))
				{
					// This happens for years that are 382 days in length,
					// which is not a legal year length.
					++day;
				}

				// Return the day number to the caller.
				return day;
			}

	// Get the month table for a specific year.
	private static int[] GetMonthTable(int year)
			{
				switch((int)(StartOfYear(year + 1) - StartOfYear(year)))
				{
					case 353: return daysPerMonthDeficient;
					case 383: return daysPerMonthDeficientLeap;

					case 354: return daysPerMonthRegular;
					case 384: return daysPerMonthRegularLeap;

					case 355: return daysPerMonthComplete;
					case 385: return daysPerMonthCompleteLeap;

					// Shouldn't happen.
					default: throw new ArgumentOutOfRangeException();
				}
			}

	// Add a time period to a DateTime value.
	public override DateTime AddMonths(DateTime time, int months)
			{
				// Pull the time value apart.
				int year = GetYear(time);
				int month = GetMonth(time);
				int day = GetDayOfMonth(time);

				// Increment or decrement the month and year values.
				int monthsInYear;
				if(months > 0)
				{
					while(months > 0)
					{
						monthsInYear = GetMonthsInYear(year, CurrentEra);
						if(months <= monthsInYear)
						{
							month += months;
							months = 0;
						}
						else
						{
							month += monthsInYear;
							months -= monthsInYear;
						}
						if(month > monthsInYear)
						{
							++year;
							month -= monthsInYear;
						}
					}
				}
				else if(months < 0)
				{
					months = -months;
					while(months > 0)
					{
						if(month > months)
						{
							month -= months;
							months = 0;
						}
						else
						{
							months -= month;
							--year;
							month = GetMonthsInYear(year, CurrentEra);
						}
					}
				}

				// Adjust the day down if it is beyond the end of the month.
				int daysInMonth = GetDaysInMonth(year, month, CurrentEra);
				if(day > daysInMonth)
				{
					day = daysInMonth;
				}

				// Build and return the new DateTime value.
				return ToDateTime(year, month, day,
								  time.Ticks % TimeSpan.TicksPerDay,
								  CurrentEra);
			}
	public override DateTime AddYears(DateTime time, int years)
			{
				// Pull the time value apart and increment it.
				int year = GetYear(time) + years;
				int month = GetMonth(time);
				int day = GetDayOfMonth(time);

				// Range-check the month and day values.
				int monthsInYear = GetMonthsInYear(year, CurrentEra);
				if(month > monthsInYear)
				{
					month = monthsInYear;
				}
				int daysInMonth = GetDaysInMonth(year, month, CurrentEra);
				if(day > daysInMonth)
				{
					day = daysInMonth;
				}

				// Build and return the new DateTime value.
				return ToDateTime(year, month, day,
								  time.Ticks % TimeSpan.TicksPerDay,
								  CurrentEra);
			}

	// Extract the components from a DateTime value.
	public override int GetDayOfMonth(DateTime time)
			{
				// Get the day of the year.
				int year = GetYear(time);
				long yearDays = StartOfYear(year) - Year1ADDays;
				int day = (int)((time.Ticks / TimeSpan.TicksPerDay) - yearDays);

				// Get the month table for this year.
				int[] table = GetMonthTable(year);

				// Scan forward until we find the right month.
				int posn = 0;
				while(posn < 12 && day >= table[posn])
				{
					day -= table[posn];
					++posn;
				}
				return day + 1;
			}
	public override System.DayOfWeek GetDayOfWeek(DateTime time)
			{
				// The Gregorian and Hebrew weekdays are identical.
				return time.DayOfWeek;
			}
	public override int GetDayOfYear(DateTime time)
			{
				int year = GetYear(time);
				long yearDays = StartOfYear(year) - Year1ADDays;
				return (int)(((time.Ticks /
								TimeSpan.TicksPerDay) - yearDays) + 1);
			}
	public override int GetMonth(DateTime time)
			{
				// Get the day of the year.
				int year = GetYear(time);
				long yearDays = StartOfYear(year) - Year1ADDays;
				int day = (int)((time.Ticks / TimeSpan.TicksPerDay) - yearDays);

				// Get the month table for this year.
				int[] table = GetMonthTable(year);

				// Scan forward until we find the right month.
				int posn = 0;
				while(posn < 12 && day >= table[posn])
				{
					day -= table[posn];
					++posn;
				}
				return posn + 1;
			}
	public override int GetYear(DateTime time)
			{
				// Get the absolute day number for "time".
				long day = time.Ticks / TimeSpan.TicksPerDay;
				day += Year1ADDays;

				// Perform a range check on MinYear and MaxYear.
				if(day < StartOfYear(MinYear) ||
				   day >= StartOfYear(MaxYear + 1))
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}

				// Perform a binary search for the year.  There is probably
				// a smarter way to do this algorithmically, but this version
				// is easier to understand and debug.  The maximum number
				// of search steps will be ceil(log2(MaxYear - MinYear)) = 10.
				int left, right, middle;
				long start;
				left = MinYear;
				right = MaxYear;
				while(left <= right)
				{
					middle = (left + right) / 2;
					start = StartOfYear(middle);
					if(day < start)
					{
						right = middle - 1;
					}
					else if(day >= start && day < StartOfYear(middle + 1))
					{
						return middle;
					}
					else
					{
						left = middle + 1;
					}
				}
				return left;
			}

	// Get the number of days in a particular month.
	public override int GetDaysInMonth(int year, int month, int era)
			{
				// Get the number of months in the year, which will
				// also validate "year" and "era".
				int monthsInYear = GetMonthsInYear(year, era);

				// Validate the month value.
				if(month < 1 || month > monthsInYear)
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}

				// Get the days per month table for this year.
				int[] table = GetMonthTable(year);

				// Return the number of days in the month.
				return table[month - 1];
			}

	// Get the number of days in a particular year.
	public override int GetDaysInYear(int year, int era)
			{
				if(era != CurrentEra && era != HebrewEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				return (int)(StartOfYear(year + 1) - StartOfYear(year));
			}

	// Get the era for a specific DateTime value.
	public override int GetEra(DateTime time)
			{
				return HebrewEra;
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				if(era != CurrentEra && era != HebrewEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(IsHebrewLeapYear(year))
				{
					return 13;
				}
				else
				{
					return 12;
				}
			}

	// Determine if a particular day is a leap day.
	public override bool IsLeapDay(int year, int month, int day, int era)
			{
				// Validate the day value.  "GetDaysInMonth" will
				// take care of validating the year, month, and era.
				if(day > GetDaysInMonth(year, month, era) || day < 1)
				{
					throw new ArgumentOutOfRangeException
						("day", _("ArgRange_Day"));
				}

				// Every day in a leap month is a leap year.
				if(IsLeapMonth(year, month, era))
				{
					return true;
				}

				// Is this a leap year?
				if(IsHebrewLeapYear(year))
				{
					// The 30th day of the 6th month is a leap day.
					if(month == 6 && day == 30)
					{
						return true;
					}
				}

				// All other days are regular days.
				return false;
			}

	// Determine if a particular month is a leap month.
	public override bool IsLeapMonth(int year, int month, int era)
			{
				if(era != CurrentEra && era != HebrewEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(month < 1 || month > GetMonthsInYear(year, era))
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
				if(!IsHebrewLeapYear(year))
				{
					return false;
				}
				return (month == 7);
			}

	// Determine if a particular year is a leap year.
	public override bool IsLeapYear(int year, int era)
			{
				if(era != CurrentEra && era != HebrewEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				return IsHebrewLeapYear(year);
			}

	// Convert a particular time into a DateTime value.
	private DateTime ToDateTime(int year, int month, int day,
							    long tickOffset, int era)
			{
				// Validate the parameters.
				if(era != CurrentEra && era != HebrewEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				int monthsInYear = GetMonthsInYear(year, era);
				if(month < 1 || month > monthsInYear)
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
				int daysInMonth = GetDaysInMonth(year, month, era);
				if(day < 1 || day > daysInMonth)
				{
					throw new ArgumentOutOfRangeException
						("day", _("ArgRange_Day"));
				}

				// Convert the Hebrew date into a Gregorian date.
				// We do this by calculating the number of days since
				// 1 January 0001 AD, which is Hebrew 01/01/3760.
				long days = StartOfYear(year) - Year1ADDays + day - 1;
				int[] table = GetMonthTable(year);
				int posn;
				for(posn = 1; posn < month; ++posn)
				{
					days += table[posn - 1];
				}

				// Build the DateTime value and return it.
				return new DateTime(days * TimeSpan.TicksPerDay + tickOffset);
			}
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				long ticks;
				ticks = DateTime.TimeToTicks(hour, minute, second);
				ticks += ((long)millisecond) * TimeSpan.TicksPerMillisecond;
				return ToDateTime(year, month, day, ticks, era);
			}

	// Convert a two-digit year value into a four-digit year value.
	public override int ToFourDigitYear(int year)
			{
				return base.ToFourDigitYear(year);
			}

}; // class HebrewCalendar

}; // namespace System.Globalization
