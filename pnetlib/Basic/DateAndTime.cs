/*
 * DateAndTime.cs - Implementation of the
 *			"Microsoft.VisualBasic.DateAndTime" class.
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

namespace Microsoft.VisualBasic
{

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class DateAndTime
{
	// This class cannot be instantiated.
	private DateAndTime() {}

	// Convert the string representation of an interval into a "DateInterval".
	private static DateInterval StringToInterval(String Interval)
			{
				if(Interval == null)
				{
					throw new ArgumentException
						(S._("VB_InvalidInterval"), "Interval");
				}
				Interval = Interval.ToLower(CultureInfo.InvariantCulture);
				switch(Interval)
				{
					case "yyyy": 	return DateInterval.Year;
					case "q":		return DateInterval.Quarter;
					case "m":		return DateInterval.Month;
					case "y":		return DateInterval.DayOfYear;
					case "d":		return DateInterval.Day;
					case "ww":		return DateInterval.WeekOfYear;
					case "w":		return DateInterval.Weekday;
					case "h":		return DateInterval.Hour;
					case "n":		return DateInterval.Minute;
					case "s":		return DateInterval.Second;
				}
				throw new ArgumentException
					(S._("VB_InvalidInterval"), "Interval");
			}

	// Convert a number into an integer, rounded for the purposes of DateAdd.
	private static int ToInt(double Number)
			{
				return checked((int)(Math.Round(Conversion.Fix(Number))));
			}

	// Add a time interval to a date value.
	public static DateTime DateAdd
				(String Interval, double Number, Object DateValue)
			{
				return DateAdd(StringToInterval(Interval), Number,
							   DateType.FromObject(DateValue));
			}
	public static DateTime DateAdd
				(DateInterval Interval, double Number, DateTime DateValue)
			{
				Calendar calendar = CultureInfo.CurrentCulture.Calendar;
				switch(Interval)
				{
					case DateInterval.Year:
						return calendar.AddYears(DateValue, ToInt(Number));

					case DateInterval.Quarter:
						return calendar.AddMonths
							(DateValue, ToInt(Number * 3.0));

					case DateInterval.Month:
						return calendar.AddMonths(DateValue, ToInt(Number));

					case DateInterval.DayOfYear:
					case DateInterval.Day:
					case DateInterval.Weekday:
						return DateValue.AddDays(ToInt(Number));

					case DateInterval.WeekOfYear:
						return DateValue.AddDays(ToInt(Number * 7.0));

					case DateInterval.Hour:
						return DateValue.AddHours(Number);

					case DateInterval.Minute:
						return DateValue.AddMinutes(Number);

					case DateInterval.Second:
						return DateValue.AddSeconds(Number);
				}
				throw new ArgumentException
					(S._("VB_InvalidInterval"), "Interval");
			}

	// Adjust a tick difference.
	private static long AdjustDiff(long ticks, long period)
			{
				return ToInt(((double)ticks) / ((double)period));
			}

	// Get the difference between two dates.
	public static long DateDiff
				(DateInterval Interval, DateTime Date1, DateTime Date2,
				 [Optional] [DefaultValue(FirstDayOfWeek.Sunday)]
				 	FirstDayOfWeek DayOfWeek,
				 [Optional] [DefaultValue(FirstWeekOfYear.Jan1)]
				 	FirstWeekOfYear WeekOfYear)
			{
				Calendar calendar = CultureInfo.CurrentCulture.Calendar;
				switch(Interval)
				{
					case DateInterval.Year:
						return calendar.GetYear(Date2) -
							   calendar.GetYear(Date1);

					case DateInterval.Quarter:
						return (calendar.GetYear(Date2) -
							    calendar.GetYear(Date1)) * 4 +
							   ((calendar.GetMonth(Date2) - 1) / 3) -
							   ((calendar.GetMonth(Date1) - 1) / 3);

					case DateInterval.Month:
						return (calendar.GetYear(Date2) -
							    calendar.GetYear(Date1)) * 12 +
							   calendar.GetMonth(Date2) -
							   calendar.GetMonth(Date1);

					case DateInterval.DayOfYear:
					case DateInterval.Day:
						return AdjustDiff(Date2.Ticks - Date1.Ticks,
										  TimeSpan.TicksPerDay);

					case DateInterval.WeekOfYear:
					{
						Date1.AddDays(-Weekday(Date1, DayOfWeek));
						Date2.AddDays(-Weekday(Date2, DayOfWeek));
						return AdjustDiff(Date2.Ticks - Date1.Ticks,
										  TimeSpan.TicksPerDay * 7);
					}
					// Not reached.

					case DateInterval.Weekday:
						return AdjustDiff(Date2.Ticks - Date1.Ticks,
										  TimeSpan.TicksPerDay * 7);

					case DateInterval.Hour:
						return AdjustDiff(Date2.Ticks - Date1.Ticks,
										  TimeSpan.TicksPerHour);

					case DateInterval.Minute:
						return AdjustDiff(Date2.Ticks - Date1.Ticks,
										  TimeSpan.TicksPerMinute);

					case DateInterval.Second:
						return AdjustDiff(Date2.Ticks - Date1.Ticks,
										  TimeSpan.TicksPerSecond);
				}
				throw new ArgumentException
					(S._("VB_InvalidInterval"), "Interval");
			}
	public static long DateDiff
				(String Interval, Object Date1, Object Date2,
				 [Optional] [DefaultValue(FirstDayOfWeek.Sunday)]
				 	FirstDayOfWeek DayOfWeek,
				 [Optional] [DefaultValue(FirstWeekOfYear.Jan1)]
				 	FirstWeekOfYear WeekOfYear)
			{
				return DateDiff(StringToInterval(Interval),
								DateType.FromObject(Date1),
								DateType.FromObject(Date2),
								DayOfWeek, WeekOfYear);
			}

	// Get the system setting for the first day of week.
	private static FirstDayOfWeek SystemFirstDay()
			{
			#if !ECMA_COMPAT
				return (FirstDayOfWeek)
					(((int)CultureInfo.CurrentCulture
							.DateTimeFormat.FirstDayOfWeek) + 1);
			#else
				return FirstDayOfWeek.Sunday;
			#endif
			}

	// Get a particular date part.
	public static int DatePart
				(DateInterval Interval, DateTime DateValue,
				 [Optional] [DefaultValue(FirstDayOfWeek.Sunday)]
				 	FirstDayOfWeek FirstDayOfWeekValue,
				 [Optional] [DefaultValue(FirstWeekOfYear.Jan1)]
				 	FirstWeekOfYear FirstWeekOfYearValue)
			{
				Calendar calendar = CultureInfo.CurrentCulture.Calendar;
				switch(Interval)
				{
					case DateInterval.Year:
						return calendar.GetYear(DateValue);

					case DateInterval.Quarter:
						return ((calendar.GetMonth(DateValue) - 1) % 3) + 1;

					case DateInterval.Month:
						return calendar.GetMonth(DateValue);

					case DateInterval.DayOfYear:
						return calendar.GetDayOfYear(DateValue);

					case DateInterval.Day:
						return calendar.GetDayOfMonth(DateValue);

					case DateInterval.WeekOfYear:
					{
						if(FirstDayOfWeekValue == FirstDayOfWeek.System)
						{
							FirstDayOfWeekValue = SystemFirstDay();
						}
						CalendarWeekRule rule;
						switch(FirstWeekOfYearValue)
						{
							case FirstWeekOfYear.System:
							{
							#if !ECMA_COMPAT
								rule = CultureInfo.CurrentCulture
									.DateTimeFormat.CalendarWeekRule;
							#else
								rule = CalendarWeekRule.FirstDay;
							#endif
							}
							break;

							case FirstWeekOfYear.Jan1:
							{
								rule = CalendarWeekRule.FirstDay;
							}
							break;

							case FirstWeekOfYear.FirstFourDays:
							{
								rule = CalendarWeekRule.FirstFourDayWeek;
							}
							break;

							case FirstWeekOfYear.FirstFullWeek:
							{
								rule = CalendarWeekRule.FirstFullWeek;
							}
							break;

							default:
							{
								throw new ArgumentException
									(S._("VB_InvalidWeekOfYear"),
									 "FirstWeekOfYearValue");
							}
							// Not reached.
						}
						return calendar.GetWeekOfYear
							(DateValue, rule,
							 (DayOfWeek)(((int)FirstDayOfWeekValue) - 1));
					}
					// Not reached.

					case DateInterval.Weekday:
						return Weekday(DateValue, FirstDayOfWeekValue);

					case DateInterval.Hour:
						return DateValue.Hour;

					case DateInterval.Minute:
						return DateValue.Minute;

					case DateInterval.Second:
						return DateValue.Second;
				}
				throw new ArgumentException
					(S._("VB_InvalidInterval"), "Interval");
			}
	public static int DatePart
				(String Interval, Object DateValue,
				 [Optional] [DefaultValue(FirstDayOfWeek.Sunday)]
				 	FirstDayOfWeek FirstDayOfWeekValue,
				 [Optional] [DefaultValue(FirstWeekOfYear.Jan1)]
				 	FirstWeekOfYear FirstWeekOfYearValue)
			{
				return DatePart(StringToInterval(Interval),
								DateType.FromObject(DateValue),
								FirstDayOfWeekValue, FirstWeekOfYearValue);
			}

	// Build a date value.
	public static DateTime DateSerial(int Year, int Month, int Day)
			{
				// Fix up the year value.
				if(Year < 0)
				{
					Year = DateTime.Now.Year + Year;
				}
				else if(Year < 30)
				{
					Year += 2000;
				}
				else if(Year < 100)
				{
					Year += 1900;
				}

				// Fix up the month value.
				while(Month < 1)
				{
					--Year;
					Month += 12;
				}
				while(Month > 12)
				{
					++Year;
					Month -= 12;
				}

				// Fix up the day value.
				int extraDays;
				int daysInMonth = DateTime.DaysInMonth(Year, Month);
				if(Day < 1)
				{
					extraDays = (Day - 1);
				}
				else if(Day > daysInMonth)
				{
					extraDays = (Day - daysInMonth);
				}
				else
				{
					extraDays = 0;
				}

				// Build and return the date.
				DateTime date = new DateTime(Year, Month, Day);

				// Adjust the date for extra days.
				if(extraDays != 0)
				{
					return date.AddDays((double)extraDays);
				}
				else
				{
					return date;
				}
			}

	// Convert a string into a date value.
	public static DateTime DateValue(String StringDate)
			{
				return (DateType.FromString(StringDate)).Date;
			}

	// Get the day from a date value.
	public static int Day(DateTime DateValue)
			{
				return CultureInfo.CurrentCulture.Calendar
							.GetDayOfMonth(DateValue);
			}

	// Get the hour from a date value.
	public static int Hour(DateTime TimeValue)
			{
				return CultureInfo.CurrentCulture.Calendar.GetHour(TimeValue);
			}

	// Get the minute from a date value.
	public static int Minute(DateTime TimeValue)
			{
				return CultureInfo.CurrentCulture.Calendar.GetMinute(TimeValue);
			}

	// Get the month from a date value.
	public static int Month(DateTime DateValue)
			{
				return CultureInfo.CurrentCulture.Calendar.GetMonth(DateValue);
			}

	// Get the name of a specific month.
	public static String MonthName
				(int Month, [Optional] [DefaultValue(false)] bool Abbreviate)
			{
				if(Month < 1 || Month > 13)	// Some calendars have 13 months.
				{
					throw new ArgumentException(S._("VB_MonthRange"), "Month");
				}
				if(Abbreviate)
				{
					return CultureInfo.CurrentCulture.DateTimeFormat
							.GetAbbreviatedMonthName(Month);
				}
				else
				{
					return CultureInfo.CurrentCulture.DateTimeFormat
							.GetMonthName(Month);
				}
			}

	// Get the second from a date value.
	public static int Second(DateTime TimeValue)
			{
				return CultureInfo.CurrentCulture.Calendar.GetSecond(TimeValue);
			}

	// Convert hour, minute, and second values into a time value.
	public static DateTime TimeSerial(int Hour, int Minute, int Second)
			{
				return new DateTime(Hour * TimeSpan.TicksPerHour +
									Minute * TimeSpan.TicksPerMinute +
									Second * TimeSpan.TicksPerSecond);
			}

	// Extract the time portion of a value.
	public static DateTime TimeValue(String StringTime)
			{
				long ticks = (DateType.FromString(StringTime)).Ticks;
				return new DateTime(ticks % TimeSpan.TicksPerDay);
			}

	// Get the weekday from a date value.
	public static int Weekday
				(DateTime DateValue,
				 [Optional] [DefaultValue(FirstDayOfWeek.Sunday)]
				 	FirstDayOfWeek DayOfWeek)
			{
				if(DayOfWeek == FirstDayOfWeek.System)
				{
					DayOfWeek = SystemFirstDay();
				}
				if(((int)DayOfWeek) < 1 || ((int)DayOfWeek) > 7)
				{
					throw new ArgumentException
						(S._("VB_InvalidWeekday"), "DayOfWeek");
				}
				int day = (int)(CultureInfo.CurrentCulture.Calendar
									.GetDayOfWeek(DateValue)) + 1;
				return ((day - (int)DayOfWeek + 7) % 7) + 1;
			}

	// Get the name of a specific weekday.
	public static String WeekdayName
				(int Weekday,
				 [Optional] [DefaultValue(false)] bool Abbreviate,
				 [Optional] [DefaultValue(FirstDayOfWeek.Sunday)]
				 		FirstDayOfWeek FirstDayOfWeekValue)
			{
				if(Weekday < 1 || Weekday > 7)
				{
					throw new ArgumentException
						(S._("VB_InvalidWeekday"), "Weekday");
				}
				if(((int)FirstDayOfWeekValue) < 0 ||
				   ((int)FirstDayOfWeekValue) > 7)
				{
					throw new ArgumentException
						(S._("VB_InvalidWeekday"), "FirstDayOfWeekValue");
				}
				if(FirstDayOfWeekValue == FirstDayOfWeek.System)
				{
					FirstDayOfWeekValue = SystemFirstDay();
				}
				int day = (Weekday + (int)FirstDayOfWeekValue - 2) % 7;
				if(Abbreviate)
				{
					return CultureInfo.CurrentCulture.DateTimeFormat
							.GetAbbreviatedDayName((DayOfWeek)day);
				}
				else
				{
					return CultureInfo.CurrentCulture.DateTimeFormat
							.GetDayName((DayOfWeek)day);
				}
			}

	// Get the year from a date value.
	public static int Year(DateTime DateValue)
			{
				return CultureInfo.CurrentCulture.Calendar.GetYear(DateValue);
			}

	// Get the current date as a string.
	public static String DateString
			{
				get
				{
					return DateTime.Today.ToString
						("MM\\-dd\\-yyyy", CultureInfo.InvariantCulture);
				}
				set
				{
					// Ignored - not used in this implementation.
				}
			}

	// Get the current date and time.
	public static DateTime Now
			{
				get
				{
					return DateTime.Now;
				}
			}

	// Get the current time of day.
	public static DateTime TimeOfDay
			{
				get
				{
					return new DateTime(DateTime.Now.TimeOfDay.Ticks);
				}
				set
				{
					// Ignored - not used in this implementation.
				}
			}

	// Get the current time as a string.
	public static String TimeString
			{
				get
				{
					return (new DateTime(DateTime.Now.TimeOfDay.Ticks))
								.ToString("HH:mm:ss",
										  CultureInfo.InvariantCulture);
				}
				set
				{
					// Ignored - not used in this implementation.
				}
			}

	// Get a timer value from the current time.
	public static double Timer
			{
				get
				{
					return (double)(DateTime.Now.Ticks % TimeSpan.TicksPerDay)
								/ (double)(TimeSpan.TicksPerSecond);
				}
			}

	// Get today's date.
	public static DateTime Today
			{
				get
				{
					return DateTime.Today;
				}
				set
				{
					// Ignored - not used in this implementation.
				}
			}

}; // class DateAndTime

}; // namespace Microsoft.VisualBasic
