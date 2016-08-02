/*
 * TimeZone.cs - Implementation of the "System.TimeZone" class.
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

namespace System
{

#if !ECMA_COMPAT

using System.Globalization;
using Platform;

[Serializable]
public abstract class TimeZone
{
	// Internal state.
	private static TimeZone current;

	// Constructor.
	protected TimeZone() {}

	// Get the current timezone.
	public static TimeZone CurrentTimeZone
			{
				get
				{
					lock(typeof(TimeZone))
					{
						if(current == null)
						{
							current = new LocalTimeZone();
						}
						return current;
					}
				}
			}

	// Get the daylight savings name for the timezone.
	public abstract String DaylightName { get; }

	// Get the standard name for the timezone.
	public abstract String StandardName { get; }

	// Get the offset from UTC of this timezone at a given time.
	public abstract TimeSpan GetUtcOffset(DateTime time);

	// Convert a UTC time value into a local time value.
	public virtual DateTime ToLocalTime(DateTime time)
			{
				return time - GetUtcOffset(time);
			}

	// Convert a local time value into a UTC time value.
	public virtual DateTime ToUniversalTime(DateTime time)
			{
				return time + GetUtcOffset(time);
			}

	// Get the daylight saving time rules for this timezone in a given year.
	public abstract DaylightTime GetDaylightChanges(int year);

	// Determine if a specified time is within the daylight savings period.
	public static bool IsDaylightSavingTime
				(DateTime time, DaylightTime daylightTimes)
			{
				// If there are no daylight savings rules, then bail out.
				if(daylightTimes == null)
				{
					return false;
				}

				// The period needs to be calculated differently depending
				// upon whether the delta is positive or negative.
				DateTime start, end;
				if(daylightTimes.Delta.Ticks > 0)
				{
					start = daylightTimes.Start + daylightTimes.Delta;
					end = daylightTimes.End;
				}
				else
				{
					start = daylightTimes.Start;
					end = daylightTimes.End - daylightTimes.Delta;
				}

				// Detect which hemisphere the information is for.
				if(start > end)
				{
					// Southern hemisphere with summer at year's end.
					return (time < start || time >= end);
				}
				else
				{
					// Northern hemisphere with summer in mid-year.
					return (time >= start && time < end);
				}
			}
	public virtual bool IsDaylightSavingTime(DateTime time)
			{
				return IsDaylightSavingTime
					(time, GetDaylightChanges(time.Year));
			}

	// Information about the local time zone.
	private sealed class LocalTimeZone : TimeZone
	{
		// Cached daylight information for a particular year.
		private int cachedYear;
		private DaylightTime cachedChanges;

		// Constructor.
		public LocalTimeZone()
				{
					cachedYear = -1;
				}

		// Get the daylight savings name for the timezone.
		public override String DaylightName
				{
					get
					{
						return TimeMethods.GetDaylightName();
					}
				}

		// Get the standard name for the timezone.
		public override String StandardName
				{
					get
					{
						return TimeMethods.GetStandardName();
					}
				}

		// Get the offset from UTC of this timezone at a given time.
		public override TimeSpan GetUtcOffset(DateTime time)
				{
					int secs = TimeMethods.GetTimeZoneAdjust(time.Ticks);
					return new TimeSpan(-secs * TimeSpan.TicksPerSecond);
				}

		// Get the daylight saving time rules for this timezone in a given year.
		public override DaylightTime GetDaylightChanges(int year)
				{
					long start, end, delta;
					if(year < 1 || year > 9999)
					{
						throw new ArgumentOutOfRangeException
							(_("ArgRange_Year"));
					}
					lock(this)
					{
						if(cachedYear == year)
						{
							return cachedChanges;
						}
						if(TimeMethods.GetDaylightRules
								(year, out start, out end, out delta))
						{
							cachedChanges = new DaylightTime
								(new DateTime(start),
								 new DateTime(end),
								 new TimeSpan(delta));
						}
						else
						{
							cachedChanges = new DaylightTime
								(DateTime.MinValue, DateTime.MaxValue,
								 TimeSpan.Zero);
						}
						cachedYear = year;
						return cachedChanges;
					}
				}

	}; // class LocalTimeZone

}; // class TimeZone

#endif // !ECMA_COMPAT

}; // namespace System
