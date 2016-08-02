/*
 * JulianCalendar.cs - Implementation of the
 *        "System.Globalization.JulianCalendar" class.
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

public class JulianCalendar : Calendar
{
	// The Julian era.
	public static readonly int JulianEra = 1;

	// Useful constants.
	private const int DefaultTwoDigitYearMax = 2029;
	private const int DaysPer4Years = (365 * 4 + 1);

	// Jan 1, 0001 AD is Jan 3, 0001 Julian.  This value is used to
	// adjust day numbers to account for the discrepancy.
	private const long EpochAdjust = 2;

	// Constructors.
	public JulianCalendar()
			{
				// Nothing to do here
			}

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [1];
					eras[0] = JulianEra;
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
					else
					{
						// Set the default value.
						base.TwoDigitYearMax = DefaultTwoDigitYearMax;
						return DefaultTwoDigitYearMax;
					}
				}
				set
				{
					if(value < 100 || value > 9999)
					{
						throw new ArgumentOutOfRangeException
							("year", _("ArgRange_Year"));
					}
					base.TwoDigitYearMax = value;
				}
			}

#if CONFIG_FRAMEWORK_1_2

	// Get the minimum DateTime value supported by this calendar.
	public override DateTime MinValue
			{
				get
				{
					return DateTime.MinValue;
				}
			}

	// Get the maximum DateTime value supported by this calendar.
	public override DateTime MaxValue
			{
				get
				{
					return DateTime.MaxValue;
				}
			}

#endif

	// Add a time period to a DateTime value.
	public override DateTime AddMonths(DateTime time, int months)
			{
				// Crack open this DateTime value.
				int year = GetYear(time);
				int month = GetMonth(time);
				int day = GetDayOfMonth(time);
				long ticks = (time.Ticks % TimeSpan.TicksPerDay);

				// Adjust the month and year values and rebuild.
				if(months < 0)
				{
					months = -months;
					year -= months / 12;
					month -= months % 12;
					if(month < 1)
					{
						--year;
						month += 12;
					}
				}
				else
				{
					year += months / 12;
					month += months % 12;
					if(month > 12)
					{
						++year;
						month -= 12;
					}
				}
				return new DateTime
					(ToDateTime(year, month, day, 0, 0, 0, 0).Ticks + ticks);
			}
	public override DateTime AddYears(DateTime time, int years)
			{
				// Crack open this DateTime value.
				int year = GetYear(time);
				int month = GetMonth(time);
				int day = GetDayOfMonth(time);
				long ticks = (time.Ticks % TimeSpan.TicksPerDay);

				// Adjust the year value and rebuild.
				return new DateTime
					(ToDateTime(year + years, month, day,
								0, 0, 0, 0).Ticks + ticks);
			}

	// Extract the components from a DateTime value.
	public override int GetDayOfMonth(DateTime time)
			{
				int day = GetDayOfYear(time) - 1;
				bool isLeap = ((GetYear(time) % 4) == 0);

				// Adjust for Jan and Feb in leap years.
				if(isLeap)
				{
					if(day < 31)
					{
						return day + 1;
					}
					else if(day < (31 + 29))
					{
						return (day - 31 + 1);
					}
					--day;
				}

				// Search for the starting month.
				int month = 1;
				while(month < 12 && day >= DateTime.daysBeforeMonth[month])
				{
					++month;
				}
				return day - DateTime.daysBeforeMonth[month - 1] + 1;
			}
	public override System.DayOfWeek GetDayOfWeek(DateTime time)
			{
				// The Gregorian and Julian calendars match on weekdays.
				return time.DayOfWeek;
			}
	public override int GetDayOfYear(DateTime time)
			{
				// Get the year value.
				int year = GetYear(time);

				// Convert the tick count into a day value.
				long days = time.Ticks / TimeSpan.TicksPerDay;

				// Adjust for the difference in epochs.
				days += EpochAdjust;

				// Return the day number within the year.
				return unchecked((int)((days - YearToDays(year)) + 1));
			}
	public override int GetMonth(DateTime time)
			{
				int day = GetDayOfYear(time) - 1;
				bool isLeap = ((GetYear(time) % 4) == 0);

				// Adjust for Jan and Feb in leap years.
				if(isLeap)
				{
					if(day < 31)
					{
						return 1;
					}
					else if(day < (31 + 29))
					{
						return 2;
					}
					--day;
				}

				// Search for the starting month.
				int month = 1;
				while(month < 12 && day >= DateTime.daysBeforeMonth[month])
				{
					++month;
				}
				return month;
			}
	public override int GetYear(DateTime time)
			{
				// Note: there is probably a tricky mathematical
				// formula for doing this, but this version is a
				// lot easier to understand and debug.

				// Convert the tick count into a day value.
				int days = unchecked((int)(time.Ticks / TimeSpan.TicksPerDay));

				// Adjust for the difference in epochs.
				days += (int)EpochAdjust;

				// Determine the 4-year cycle that contains the date.
				int yearBase = ((days / DaysPer4Years) * 4) + 1;
				int yearOffset = days % DaysPer4Years;

				// Determine the year out of the 4-year cycle.
				if(yearOffset >= 365 * 3)
				{
					return yearBase + 3;
				}
				else if(yearOffset >= 365 * 2)
				{
					return yearBase + 2;
				}
				else if(yearOffset >= 365)
				{
					return yearBase + 1;
				}
				else
				{
					return yearBase;
				}
			}

	// Get the number of days in a particular month.
	public override int GetDaysInMonth(int year, int month, int era)
			{
				if(era != CurrentEra && era != JulianEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
				if(month != 2 || (year % 4) != 0)
				{
					return DateTime.daysForEachMonth[month - 1];
				}
				else
				{
					return 29;
				}
			}

	// Get the number of days in a particular year.
	public override int GetDaysInYear(int year, int era)
			{
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != JulianEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if((year % 4) == 0)
				{
					return 366;
				}
				else
				{
					return 365;
				}
			}

	// Get the era for a specific DateTime value.
	public override int GetEra(DateTime time)
			{
				return JulianEra;
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != JulianEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return 12;
			}

	// Determine if a particular day is a leap day.
	//
	// Note: according to the Calendar FAQ, the leap day is actually
	// the 24th of February, not the 29th!  This comes from the
	// ancient Roman calendar.  However, since most people in the
	// modern world think it is the 29th, Microsoft and others have
	// actually implemented this function "wrong".  We've matched
	// this "wrong" implementation here, for compatibility reasons.
	//
	// See: http://www.tondering.dk/claus/calendar.html
	public override bool IsLeapDay(int year, int month, int day, int era)
			{
				if(day < 1 || day > GetDaysInMonth(year, month, era))
				{
					throw new ArgumentOutOfRangeException
						("day", _("ArgRange_Day"));
				}
				if(DateTime.IsLeapYear(year) && month == 2 && day == 29)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Determine if a particular month is a leap month.
	public override bool IsLeapMonth(int year, int month, int era)
			{
				if(month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
				return (IsLeapYear(year, era) && month == 2);
			}

	// Determine if a particular year is a leap year.
	public override bool IsLeapYear(int year, int era)
			{
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != JulianEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return ((year % 4) == 0);
			}

	// Convert a Julian year into a day number.
	private static long YearToDays(int year)
			{
				--year;
				return (long)(year * 365 + year / 4);
			}

	// Determine if a Julian date is in range (0001/01/03 - 9999-10-19).
	private static bool CheckDateRange(int year, int month, int day)
			{
				if(year == 1 && month == 1)
				{
					return (day >= 3 && day <= 31);
				}
				else if(year == 9999 && month > 10)
				{
					return false;
				}
				else if(year == 9999 && month == 10 && day > 19)
				{
					return false;
				}
				else if(year < 1 || year > 9999 || month < 1 || month > 12)
				{
					return false;
				}
				else if(day < 1)
				{
					return false;
				}
				bool isLeap = ((year % 4) == 0);
				int daysInMonth = DateTime.daysForEachMonth[month - 1];
				if(month == 2 && isLeap)
				{
					++daysInMonth;
				}
				return (day >= 1 && day <= daysInMonth);
			}

	// Convert a particular time into a DateTime value.
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				if(era != CurrentEra && era != JulianEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				int daysInMonth;
				long result;
				bool isLeap;
				if(CheckDateRange(year, month, day))
				{
					isLeap = ((year % 4) == 0);
					daysInMonth = DateTime.daysForEachMonth[month - 1];
					if(month == 2 && isLeap)
					{
						++daysInMonth;
					}
					if(day >= 1 && day <= daysInMonth)
					{
						unchecked
						{
							result = YearToDays(year);
							result +=
								(long)(DateTime.daysBeforeMonth[month - 1]);
							if(month > 2 && isLeap)
							{
								++result;
							}
							result -= EpochAdjust;
							return new DateTime
								  ((result + (long)(day - 1)) *
								   TimeSpan.TicksPerDay +
								   DateTime.TimeToTicks(hour, minute, second) +
								   ((long)millisecond) * 10000L);
						}
					}
				}
				throw new ArgumentOutOfRangeException(_("ArgRange_YMD"));
			}

	// Convert a two-digit year value into a four-digit year value.
	public override int ToFourDigitYear(int year)
			{
				return base.ToFourDigitYear(year);
			}

}; // class JulianCalendar

}; // namespace System.Globalization
