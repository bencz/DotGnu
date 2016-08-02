/*
 * HijriCalendar.cs - Implementation of the
 *        "System.Globalization.HijriCalendar" class.
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

using System;
using Microsoft.Win32;

public class HijriCalendar : Calendar
{
	// The Hijri era.
	public static readonly int HijriEra = 1;

	// Useful constants.
	private const int DefaultTwoDigitMax = 1451;
	private const int MaxYear = 9666;
	private const long MinTicks = 196130592000000000L; // 622-07-08

	// Internal state.
	private int adjustment;

	// Number of days in each month of the year.  The last month will
	// have 30 days in a leap year.
	private static readonly int[] daysInMonth =
			{30, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29};

	// Number of days before each month.
	private static readonly int[] daysBeforeMonth =
			{0,
			 30,
			 30 + 29,
			 30 + 29 + 30,
			 30 + 29 + 30 + 29,
			 30 + 29 + 30 + 29 + 30,
			 30 + 29 + 30 + 29 + 30 + 29,
			 30 + 29 + 30 + 29 + 30 + 29 + 30,
			 30 + 29 + 30 + 29 + 30 + 29 + 30 + 29,
			 30 + 29 + 30 + 29 + 30 + 29 + 30 + 29 + 30,
			 30 + 29 + 30 + 29 + 30 + 29 + 30 + 29 + 30 + 29,
			 30 + 29 + 30 + 29 + 30 + 29 + 30 + 29 + 30 + 29 + 30};

	// Constructor.
	public HijriCalendar()
			{
				adjustment = 0x0100;
			}

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [1];
					eras[0] = HijriEra;
					return eras;
				}
			}

	// Get or set the Hijri adjustment value, which shifts the
	// date forward or back by up to two days.
	public int HijriAdjustment
			{
				get
				{
					if(adjustment == 0x0100)
					{
					#if CONFIG_WIN32_SPECIFICS
						// Inspect the registry to get the adjustment value.
						adjustment = 0;
						try
						{
							RegistryKey key = Registry.CurrentUser;
							key = key.OpenSubKey
								("Control Panel\\International", false);
							if(key != null)
							{
								Object value = key.GetValue
									("AddHijriDate", null);
								key.Close();
								String str = null;
								if(value != null)
								{
									str = value.ToString();
								}
								if(str != null &&
								   str.StartsWith("AddHijriDate"))
								{
									str = str.Substring(12);
									if(str.Length > 0)
									{
										int ivalue = Int32.Parse(str);
										if(ivalue >= -2 && ivalue <= 2)
										{
											adjustment = ivalue;
										}
									}
								}
							}
						}
						catch(Exception)
						{
							// Ignore registry access errors.
						}
					#else
						adjustment = 0;
					#endif
					}
					return adjustment;
				}
				set
				{
					if(value < -2 || value > 2)
					{
						throw new ArgumentOutOfRangeException
							("value", _("ArgRange_HijriAdjustment"));
					}
					adjustment = value;
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
					if(value < 100 || value > MaxYear)
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
					return new DateTime(MinTicks);
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

	// Convert a year value into an absolute number of days.
	private long YearToDays(int year)
			{
				int cycle = ((year - 1) / 30) * 30;
				int left = year - cycle - 1;
				long days = ((cycle * 10631L) / 30L) + 227013L;
				while(left > 0)
				{
					days += GetDaysInYear(left, HijriEra);
					--left;
				}
				return days;
			}

	// Pull apart a DateTime value into year, month, and day.
	private void PullDateApart(DateTime time, out int year,
							   out int month, out int day)
			{
				long days;
				long estimate1;
				long estimate2;

				// Validate the time range.
				if(time.Ticks < MinTicks)
				{
					throw new ArgumentOutOfRangeException
						("time", _("ArgRange_HijriDate"));
				}

				// Calculate the absolute date, adjusted as necessary.
				days = (time.Ticks / TimeSpan.TicksPerDay) + 1;
				days += HijriAdjustment;

				// Calculate the Hijri year value.
				year = (int)(((days - 227013) * 30) / 10631) + 1;
				estimate1 = YearToDays(year);
				estimate2 = GetDaysInYear(year, HijriEra);
				if(days < estimate1)
				{
					estimate1 -= estimate2;
					--year;
				}
				else if(days == estimate1)
				{
					--year;
					estimate2 = GetDaysInYear(year, HijriEra);
					estimate1 -= estimate2;
				}
				else if(days > (estimate1 + estimate2))
				{
					estimate1 += estimate2;
					++year;
				}

				// Calculate the Hijri month value.
				month = 1;
				days -= estimate1;
				while(month <= 12 && days > daysBeforeMonth[month - 1])
				{
					++month;
				}
				--month;

				// Calculate the Hijri date value.
				day = (int)(days - daysBeforeMonth[month - 1]);
			}

	// Recombine a DateTime value from its components.
	private DateTime RecombineDate(int year, int month, int day, long ticks)
			{
				int limit = GetDaysInMonth(year, month, HijriEra);
				if(day < 1 || day > limit)
				{
					throw new ArgumentOutOfRangeException
						("day", _("ArgRange_Year"));
				}
				long days;
				days = YearToDays(year) + daysBeforeMonth[month - 1] + day;
				days -= (HijriAdjustment + 1);
				if(days < 0)
				{
					throw new ArgumentOutOfRangeException
						("time", _("ArgRange_HijriDate"));
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
				int limit = GetDaysInMonth(year, month, HijriEra);
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
				if(era != CurrentEra && era != HijriEra)
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
				if(IsLeapYear(year, era))
				{
					return 355;
				}
				else
				{
					return 354;
				}
			}

	// Get the era for a specific DateTime value.
	public override int GetEra(DateTime time)
			{
				return HijriEra;
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				if(year < 1 || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != HijriEra)
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
				if(IsLeapMonth(year, month, era))
				{
					return (day == 30);
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
				if(IsLeapYear(year, era))
				{
					return (month == 12);
				}
				else
				{
					return false;
				}
			}

	// Determine if a particular year is a leap year.
	public override bool IsLeapYear(int year, int era)
			{
				if(year < 1 || year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era != CurrentEra && era != HijriEra)
				{
					throw new ArgumentException(_("Arg_InvalidEra"));
				}
				if((((year * 11) + 14) % 30) < 11)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Convert a particular time into a DateTime value.
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				if(era != CurrentEra && era != HijriEra)
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
				if(year > MaxYear)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				return base.ToFourDigitYear(year);
			}

}; // class HijriCalendar

}; // namespace System.Globalization
