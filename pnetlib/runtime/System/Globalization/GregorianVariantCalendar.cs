/*
 * GregorianVariantCalendar.cs - Implementation of the
 *        "System.Globalization.GregorianVariantCalendar" class.
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

// This class is used to implement the Japanese, Korean, Taiwan,
// and ThaiBuddhist calendars, which are all variants of the
// Gregorian calendar, but which differ only in how eras and
// years are calculated.

internal sealed class GregorianVariantCalendar : GregorianCalendar
{
	// Internal state.
	private int twoDigitYearMax;
	private EraRule[] rules;
	private int currentEra;

	// Era rule information.
	public sealed class EraRule
	{
		public int		era;
		public DateTime	startEra;
		public DateTime	endEra;
		public int		startYear;
		public int		endYear;

		public EraRule(int era, DateTime startEra,
					   DateTime endEra, int startYear)
				{
					this.era = era;
					this.startEra = startEra;
					if(endEra != DateTime.MaxValue)
					{
						this.endEra = endEra - new TimeSpan(1);
					}
					else
					{
						this.endEra = endEra;
					}
					this.startYear = startYear;
					this.endYear = startYear + (endEra.Year - startEra.Year);
				}

	}; // class EraRule

	// Constructor.
	public GregorianVariantCalendar
				(int twoDigitYearMax, EraRule[] rules, int currentEra)
			{
				this.twoDigitYearMax = twoDigitYearMax;
				this.rules = rules;
				this.currentEra = currentEra;
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
						base.TwoDigitYearMax = twoDigitYearMax;
						return twoDigitYearMax;
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

	// Get a list of eras for the calendar.
	public override int[] Eras
			{
				get
				{
					int[] eras = new int [rules.Length];
					int posn;
					for(posn = 0; posn < rules.Length; ++posn)
					{
						eras[posn] = rules[posn].era;
					}
					return eras;
				}
			}

	// Get the year from a DateTime value.
	public override int GetYear(DateTime time)
			{
				int posn;
				for(posn = 0; posn < rules.Length; ++posn)
				{
					if(time >= rules[posn].startEra &&
					   time <= rules[posn].endEra)
					{
						return time.Year - rules[posn].startEra.Year +
							   rules[posn].startYear;
					}
				}
				throw new ArgumentException(_("Arg_NoEraYear"));
			}

	// Map a localized year to a Gregorian year.
	private int ToGregorianYear(int year, int era)
			{
				if(year < 1 || year > 9999)
				{
					throw new ArgumentOutOfRangeException
						("year", _("ArgRange_Year"));
				}
				if(era == CurrentEra)
				{
					era = currentEra;
				}
				int posn;
				for(posn = 0; posn < rules.Length; ++posn)
				{
					if(rules[posn].era == era &&
					   year >= rules[posn].startYear &&
					   year <= rules[posn].endYear)
					{
						return year - rules[posn].startYear +
							   rules[posn].startEra.Year;
					}
				}
				throw new ArgumentException(_("Arg_InvalidEra"));
			}

	// Get the number of days in a particular year.
	public override int GetDaysInYear(int year, int era)
			{
				return base.GetDaysInYear(ToGregorianYear(year, era), ADEra);
			}

	// Get the era for a specific DateTime value.
	public override int GetEra(DateTime time)
			{
				int posn;
				for(posn = 0; posn < rules.Length; ++posn)
				{
					if(time >= rules[posn].startEra &&
					   time <= rules[posn].endEra)
					{
						return rules[posn].era;
					}
				}
				throw new ArgumentException(_("Arg_NoEraYear"));
			}

	// Get the number of months in a specific year.
	public override int GetMonthsInYear(int year, int era)
			{
				return base.GetMonthsInYear
					(ToGregorianYear(year, era), ADEra);
			}

	// Determine if a particular day is a leap day.
	public override bool IsLeapDay(int year, int month, int day, int era)
			{
				return base.IsLeapDay(ToGregorianYear(year, era),
									  month, day, ADEra);
			}

	// Determine if a particular month is a leap month.
	public override bool IsLeapMonth(int year, int month, int era)
			{
				return base.IsLeapMonth(ToGregorianYear(year, era),
										month, ADEra);
			}

	// Determine if a particular year is a leap year.
	public override bool IsLeapYear(int year, int era)
			{
				return base.IsLeapYear(ToGregorianYear(year, era), ADEra);
			}

	// Convert a particular time into a DateTime value.
	public override DateTime ToDateTime(int year, int month, int day,
										int hour, int minute, int second,
										int millisecond, int era)
			{
				return base.ToDateTime(ToGregorianYear(year, era),
									   month, day, hour, minute,
									   second, millisecond, ADEra);
			}

}; // class GregorianVariantCalendar

}; // namespace System.Globalization
