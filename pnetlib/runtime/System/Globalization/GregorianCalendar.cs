/*
 * GregorianCalendar.cs - Implementation of the
 *        "System.Globalization.GregorianCalendar" class.
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

public class GregorianCalendar : Calendar
{
	// The A.D. era.
	public const int ADEra = 1;

	// Internal state.
	private GregorianCalendarTypes calendarType;

	// Useful constants.
	private const int DefaultTwoDigitMax = 2029;

	// Constructors.
	public GregorianCalendar()
			{
				calendarType = GregorianCalendarTypes.Localized;
			}
	public GregorianCalendar(GregorianCalendarTypes type)
			{
				calendarType = type;
			}

	// Get or set the Gregorian calendar type.
	public virtual GregorianCalendarTypes CalendarType
			{
				get
				{
					return calendarType;
				}
				set
				{
					calendarType = value;
				}
			}

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [1];
					eras[0] = ADEra;
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
				return time.AddMonths(months);
			}
	public override DateTime AddWeeks(DateTime time, int weeks)
			{
				return base.AddWeeks(time, weeks);
			}
	public override DateTime AddYears(DateTime time, int years)
			{
				return time.AddYears(years);
			}

	// Extract the components from a DateTime value.
	public override int GetDayOfMonth(DateTime time)
			{
				return time.Day;
			}
	public override System.DayOfWeek GetDayOfWeek(DateTime time)
			{
				return time.DayOfWeek;
			}
	public override int GetDayOfYear(DateTime time)
			{
				return time.DayOfYear;
			}
	public override int GetMonth(DateTime time)
			{
				return time.Month;
			}
	public override int GetYear(DateTime time)
			{
				return time.Year;
			}

	// Get the number of days in a particular month.
	public override int GetDaysInMonth(int year, int month, int era)
			{
				if(era != CurrentEra && era != ADEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return DateTime.DaysInMonth(year, month);
			}

	// Get the number of days in a particular year.
	public override int GetDaysInYear(int year, int era)
			{
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != ADEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if(DateTime.IsLeapYear(year))
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
				return ADEra;
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != ADEra)
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
				if(era != CurrentEra && era != ADEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return DateTime.IsLeapYear(year);
			}

	// Convert a particular time into a DateTime value.
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				if(era != CurrentEra && era != ADEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				return new DateTime(year, month, day, hour,
									minute, second, millisecond);
			}

	// Convert a two-digit year value into a four-digit year value.
	public override int ToFourDigitYear(int year)
			{
				return base.ToFourDigitYear(year);
			}

}; // class GregorianCalendar

}; // namespace System.Globalization
