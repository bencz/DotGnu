/*
 * JalaaliCalendar.cs - Implementation of the
 *        "System.Globalization.JalaaliCalendar" class.
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

// This Jalaali calendar implementation is based on the algorithm used by
// "http://www.funaba.org/en/calendar.html".

public class JalaaliCalendar : Calendar
{
	// The Jalaali era.
	public const int JalaaliEra = 1;

	// Useful constants.
	private const int DefaultTwoDigitMax = 1408;	// guessed value - TODO
	private const int MinYear = 1;				// Minimum Jalaali year.
	private const int MaxYear = 9377;			// Maximum Jalaali year.
	private const long JalaaliYearOne = 226894;	// Day number of 1/1/0001 J.
	private static readonly DateTime MinDate =
			new DateTime(622, 3, 21);			// Gregorian for 1/1/0001 J.

	// Number of days in each month of the year.  The last month will
	// have 30 days in a leap year.
	private static readonly int[] daysInMonth =
			{31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29};

	// Number of days before each month.
	private static readonly int[] daysBeforeMonth =
			{0,
			 31,
			 31 + 31,
			 31 + 31 + 31,
			 31 + 31 + 31 + 31,
			 31 + 31 + 31 + 31 + 31,
			 31 + 31 + 31 + 31 + 31 + 31,
			 31 + 31 + 31 + 31 + 31 + 31 + 30,
			 31 + 31 + 31 + 31 + 31 + 31 + 30 + 30,
			 31 + 31 + 31 + 31 + 31 + 31 + 30 + 30 + 30,
			 31 + 31 + 31 + 31 + 31 + 31 + 30 + 30 + 30 + 30,
			 31 + 31 + 31 + 31 + 31 + 31 + 30 + 30 + 30 + 30 + 30};

	// Constructors.
	public JalaaliCalendar() {}

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [1];
					eras[0] = JalaaliEra;
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
						base.TwoDigitYearMax = DefaultTwoDigitMax;
						return DefaultTwoDigitMax;
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

	// Get the minimum DateTime value supported by this calendar.
	public override DateTime MinValue
			{
				get
				{
					return MinDate;
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

	// Convert a Jalaali year value into an offset in days since 1/1/0001 AD.
	private static long YearToDays(int year)
			{
				return 365 * (year - 1) +
				       ((21 + (8 * year)) / 33) +
					   JalaaliYearOne;
			}

	// Pull apart a DateTime value into year, month, and day.
	private static void PullDateApart(DateTime time, out int year,
							   		  out int month, out int day)
			{
				// Validate the time range.
				if(time < MinDate)
				{
					throw new ArgumentOutOfRangeException
						("time", _("Arg_DateTimeRange"));
				}

				// Get the absolute day number for the date.
				long absolute = (time.Ticks / TimeSpan.TicksPerDay) + 1;

				// Extract the year value.
				int approx = (int)((absolute - JalaaliYearOne) / 366);
				int temp, y;
				temp = 0;
				y = approx;
				while(absolute >= (YearToDays(y + 1) + 1))
				{
					++temp;
					++y;
				}
				year = y;

				// Extract the year component from the absolute date.
				absolute -= YearToDays(year);

				// Determine the month and day values.
				month = 1;
				while(month < 12 && absolute > daysInMonth[month - 1])
				{
					absolute -= daysInMonth[month - 1];
					++month;
				}
				day = (int)absolute;
			}

	// Recombine a DateTime value from its components.
	private DateTime RecombineDate(int year, int month, int day, long ticks)
			{
				int limit = GetDaysInMonth(year, month, JalaaliEra);
				if(day < 1 || day > limit)
				{
					throw new ArgumentOutOfRangeException
						("day", _("ArgRange_Year"));
				}
				long days;
				days = YearToDays(year) + daysBeforeMonth[month - 1] + day - 1;
				if(days < 0)
				{
					throw new ArgumentOutOfRangeException
						("time", _("Arg_DateTimeRange"));
				}
				return new DateTime(days * TimeSpan.TicksPerDay + ticks);
			}

	// Add a time period to a DateTime value.
	public override DateTime AddMonths(DateTime time, int months)
			{
				int year, month, day;
				PullDateApart(time, out year, out month, out day);
				if(months > 0)
				{
					year += months / 12;
					month += months % 12;
					if(month > 12)
					{
						++year;
						month -= 12;
					}
				}
				else if(months < 0)
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
				int limit = GetDaysInMonth(year, month, JalaaliEra);
				if(day > limit)
				{
					day = limit;
				}
				return RecombineDate(year, month, day,
									 time.Ticks % TimeSpan.TicksPerDay);
			}
	public override DateTime AddYears(DateTime time, int years)
			{
				return AddMonths(time, years * 12);
			}

	// Extract the components from a DateTime value.
	public override int GetDayOfMonth(DateTime time)
			{
				int year, month, day;
				PullDateApart(time, out year, out month, out day);
				return day;
			}
	public override System.DayOfWeek GetDayOfWeek(DateTime time)
			{
				return time.DayOfWeek;
			}
	public override int GetDayOfYear(DateTime time)
			{
				int year, month, day;
				PullDateApart(time, out year, out month, out day);
				return daysBeforeMonth[month - 1] + day;
			}
	public override int GetMonth(DateTime time)
			{
				int year, month, day;
				PullDateApart(time, out year, out month, out day);
				return month;
			}
	public override int GetYear(DateTime time)
			{
				int year, month, day;
				PullDateApart(time, out year, out month, out day);
				return year;
			}

	// Get the number of days in a particular month.
	public override int GetDaysInMonth(int year, int month, int era)
			{
				if(era != CurrentEra && era != JalaaliEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(year < 1 || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
				if(month < 12)
				{
					return daysInMonth[month - 1];
				}
				else if(IsLeapYear(year, era))
				{
					return 30;
				}
				else
				{
					return 29;
				}
			}

	// Get the number of days in a particular year.
	public override int GetDaysInYear(int year, int era)
			{
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != JalaaliEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(IsLeapYear(year))
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
				return JalaaliEra;
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != JalaaliEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return 12;
			}

	// Determine if a particular day is a leap day.
	public override bool IsLeapDay(int year, int month, int day, int era)
			{
				if(day < 1 || day > GetDaysInMonth(year, month, era))
				{
					throw new ArgumentOutOfRangeException
						("day", _("ArgRange_Day"));
				}
				if(DateTime.IsLeapYear(year) && month == 12 && day == 30)
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
				return (IsLeapYear(year, era) && month == 12);
			}

	// Determine if a particular year is a leap year.
	public override bool IsLeapYear(int year, int era)
			{
				if(year < MinYear || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != JalaaliEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
			    return (((29 + (8 * year)) % 33) < 8);
			}

	// Convert a particular time into a DateTime value.
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				if(era != CurrentEra && era != JalaaliEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return RecombineDate(year, month, day,
								     (new TimeSpan(hour, minute, second,
									 			   millisecond)).Ticks);
			}

	// Convert a two-digit year value into a four-digit year value.
	public override int ToFourDigitYear(int year)
			{
				return base.ToFourDigitYear(year);
			}

}; // class JalaaliCalendar

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Globalization
