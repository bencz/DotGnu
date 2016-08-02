/*
 * vb_date.c - Date utilities for the VB compiler.
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

#include <cscc/vb/vb_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Useful constants.
 */
#define	TicksPerSecond		((ILInt64)10000000)
#define	TicksPerDay			(TicksPerSecond * 3600 * 24)

/*
 * Number of days in each month of the year.
 */
static int const daysForEachMonth[] =
	{31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

/*
 * Number of days before each month of the year (for non-leap years).
 */
static int const daysBeforeMonth[] =
	{0,
	 31,
	 31 + 28,
	 31 + 28 + 31,
	 31 + 28 + 31 + 30,
	 31 + 28 + 31 + 30 + 31,
	 31 + 28 + 31 + 30 + 31 + 30,
	 31 + 28 + 31 + 30 + 31 + 30 + 31,
	 31 + 28 + 31 + 30 + 31 + 30 + 31 + 31,
	 31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30,
	 31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31,
	 31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31 + 30};

/*
 * Determine if a year is a leap year.
 */
static int IsLeapYear(ILInt64 year)
{
	return ((year % 4) == 0 &&
			((year % 100) != 0 || (year % 400) == 0));
}

/*
 * Determine the number of days in a specific month.
 */
static int DaysInMonth(ILInt64 year, ILInt64 month)
{
	if(IsLeapYear(year) && month == 2)
	{
		return 29;
	}
	else
	{
		return daysForEachMonth[(int)(month - 1)];
	}
}

/*
 * Get the number of days to reach the start of a particular year.
 */
static ILInt64 YearToDays(ILInt64 year)
{
	--year;
	return year * 365 + year / 4 - year / 100 + year / 400;
}

ILInt64 VBDate(ILInt64 month, ILInt64 day, ILInt64 year, int yearDigits)
{
	ILInt64 result;

	/* Validate the parameters */
	if(year >= 0 && year < 100 && yearDigits <= 2)
	{
		if(year < 30)
		{
			year += 2000;
		}
		else
		{
			year += 1900;
		}
	}
	if(year < 1 || year > 9999)
	{
		CCError(_("invalid year value"));
		year = 1;
	}
	if(month < 1 || month > 12)
	{
		CCError(_("invalid month value"));
		month = 1;
	}
	if(day < 1 || day > DaysInMonth(year, month))
	{
		CCError(_("invalid day value"));
		day = 1;
	}

	/* Build the final tick value */
	result = YearToDays(year);
	result += daysBeforeMonth[(int)(month - 1)];
	if(month > 2 && IsLeapYear(year))
	{
		++result;
	}
	return (result + (day - 1)) * TicksPerDay;
}

ILInt64 VBTime(ILInt64 hour, ILInt64 minute, ILInt64 second, int ampm)
{
	/* Validate the parameters */
	if(ampm == VB_TIME_UNSPEC)
	{
		if(hour < 0 || hour > 23)
		{
			CCError(_("invalid hour value"));
			hour = 0;
		}
	}
	else if(ampm == VB_TIME_AM)
	{
		if(hour < 1 || hour > 12)
		{
			CCError(_("invalid hour value"));
			hour = 0;
		}
		else if(hour == 12)
		{
			hour = 0;
		}
	}
	else
	{
		if(hour < 1 || hour > 12)
		{
			CCError(_("invalid hour value"));
			hour = 12;
		}
		else if(hour != 12)
		{
			hour += 12;
		}
	}
	if(minute < 0 || minute > 59)
	{
		CCError(_("invalid minute value"));
		minute = 0;
	}
	if(second < 0 || second > 59)
	{
		CCError(_("invalid second value"));
		second = 0;
	}

	/* Build the final tick value */
	return (hour * 3600 + minute * 60 + second) * TicksPerSecond;
}

#ifdef	__cplusplus
};
#endif
