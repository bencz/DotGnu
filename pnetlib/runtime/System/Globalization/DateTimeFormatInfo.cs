/*
 * DateTimeFormatInfo.cs - Implementation of the
 *		"System.Globalization.DateTimeFormatInfo" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

public sealed class DateTimeFormatInfo : ICloneable, IFormatProvider
{

	// Internal state.
	private static DateTimeFormatInfo invariantInfo;
	private bool readOnly;
	private String amDesignator;
	private String pmDesignator;
	private String[] abbreviatedDayNames;
	private String[] dayNames;
	private String[] abbreviatedMonthNames;
	private String[] monthNames;
	private String[] eraNames;
	private String[] abbrevEraNames;
	private String dateSeparator;
	private String timeSeparator;
	private String fullDateTimePattern;
	private String longDatePattern;
	private String longTimePattern;
	private String monthDayPattern;
	private String shortDatePattern;
	private String shortTimePattern;
	private String yearMonthPattern;
	private Calendar calendar;
#if !ECMA_COMPAT
	private CalendarWeekRule calendarWeekRule;
	private DayOfWeek firstDayOfWeek;
#endif // !ECMA_COMPAT
	private String[] dateTimePatterns;

	// Invariant abbreviated day names.
	private static readonly String[] invAbbrevDayNames =
		{"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};

	// Invariant day names.
	private static readonly String[] invDayNames =
		{"Sunday", "Monday", "Tuesday", "Wednesday",
		 "Thursday", "Friday", "Saturday"};

	// Invariant abbreviated month names.
	private static readonly String[] invAbbrevMonthNames =
		{"Jan", "Feb", "Mar", "Apr", "May", "Jun",
		 "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", ""};

	// Invariant month names.
	private static readonly String[] invMonthNames =
		{"January", "February", "March", "April", "May", "June",
		 "July", "August", "September", "October", "November",
		 "December", ""};

	// Invariant era names.
	private static readonly String[] invEraNames = {"A.D."};
	private static readonly String[] invAbbrevEraNames = {"AD"};

	// Invariant date time pattern list.  Each string begins
	// with a format character followed by a colon.
	private static readonly String[] invDateTimePatterns =
			{"d:MM/dd/yyyy",
			 "D:dddd, dd MMMM yyyy",
			 "f:dddd, dd MMMM yyyy HH:mm",
			 "f:dddd, dd MMMM yyyy hh:mm tt",
			 "f:dddd, dd MMMM yyyy H:mm",
			 "f:dddd, dd MMMM yyyy h:mm tt",
			 "F:dddd, dd MMMM yyyy HH:mm:ss",
			 "g:MM/dd/yyyy HH:mm",
			 "g:MM/dd/yyyy hh:mm tt",
			 "g:MM/dd/yyyy H:mm",
			 "g:MM/dd/yyyy h:mm tt",
			 "G:MM/dd/yyyy HH:mm:ss",
			 "m:MMMM dd",
			 "M:MMMM dd",
			 "r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
			 "R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
			 "s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
			 "t:HH:mm",
			 "t:hh:mm tt",
			 "t:H:mm",
			 "t:h:mm tt",
			 "T:HH:mm:ss",
			 "u:yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
			 "U:dddd, dd MMMM yyyy HH:mm:ss",
			 "y:yyyy MMMM",
			 "Y:yyyy MMMM"};

	// Constructor.
	public DateTimeFormatInfo()
			{
				readOnly = false;
				amDesignator = "AM";
				pmDesignator = "PM";
				abbreviatedDayNames = invAbbrevDayNames;
				dayNames = invDayNames;
				abbreviatedMonthNames = invAbbrevMonthNames;
				monthNames = invMonthNames;
				eraNames = invEraNames;
				abbrevEraNames = invAbbrevEraNames;
				dateSeparator = "/";
				timeSeparator = ":";
				fullDateTimePattern = "dddd, dd MMMM yyyy HH:mm:ss";
				longDatePattern = "dddd, dd MMMM yyyy";
				longTimePattern = "HH:mm:ss";
				monthDayPattern = "MMMM dd";
				shortDatePattern = "MM/dd/yyyy";
				shortTimePattern = "HH:mm";
				yearMonthPattern = "yyyy MMMM";
				calendar = new GregorianCalendar();
				dateTimePatterns = invDateTimePatterns;
			#if !ECMA_COMPAT
				calendarWeekRule = CalendarWeekRule.FirstDay;
				firstDayOfWeek = DayOfWeek.Sunday;
			#endif // !ECMA_COMPAT
			}

	// Get the invariant date time format information.
	public static DateTimeFormatInfo InvariantInfo
			{
				get
				{
					lock(typeof(DateTimeFormatInfo))
					{
						if(invariantInfo == null)
						{
							invariantInfo = new DateTimeFormatInfo();
							invariantInfo.readOnly = true;
						}
						return invariantInfo;
					}
				}
			}

	// Get the date time format information for the current thread's culture.
	public static DateTimeFormatInfo CurrentInfo
			{
				get
				{
					return CultureInfo.CurrentCulture.DateTimeFormat;
				}
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
			#if !ECMA_COMPAT
				DateTimeFormatInfo dateTimeFormat = (DateTimeFormatInfo)MemberwiseClone();
				dateTimeFormat.readOnly = false;
				return dateTimeFormat;
			#else
				return MemberwiseClone();
			#endif
			}

	// Get the abbreviated name for a month.
	public String GetAbbreviatedMonthName(int month)
			{
				if(month >= 1 && month <= 13)
				{
					return abbreviatedMonthNames[month - 1];
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
			}

	// Search for an era name within an array.
	private static int SearchForEra(String[] names, String name)
			{
				int posn;
				if(names == null)
				{
					return -1;
				}
				for(posn = 0; posn < names.Length; ++posn)
				{
					if(String.Compare(names[posn], name, true) == 0)
					{
						return posn + 1;
					}
				}
				return -1;
			}

	// Get a value that represents an era name.
	public int GetEra(String eraName)
			{
				int era;
				if(eraName == null)
				{
					throw new ArgumentNullException("eraName");
				}
				era = SearchForEra(eraNames, eraName);
				if(era == -1)
				{
					era = SearchForEra(abbrevEraNames, eraName);
				}
				if(era == -1)
				{
					era = SearchForEra(invEraNames, eraName);
				}
				if(era == -1)
				{
					era = SearchForEra(invAbbrevEraNames, eraName);
				}
				return era;
			}

	// Get the name of a particular era.
	public String GetEraName(int era)
			{
				if(era == System.Globalization.Calendar.CurrentEra)
				{
					era = Calendar.GetEra(DateTime.Now);
				}
				if(era >= 1 && era <= eraNames.Length)
				{
					return eraNames[era - 1];
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("era", _("Arg_InvalidEra"));
				}
			}

	// Implement the IFormatProvider interface.
	public Object GetFormat(Type formatType)
			{
				if(formatType == typeof(DateTimeFormatInfo))
				{
					return this;
				}
				else
				{
					return CurrentInfo;
				}
			}

	// Get the date time format information associated with "provider".
#if ECMA_COMPAT
	internal
#else
	public
#endif
	static DateTimeFormatInfo GetInstance(IFormatProvider provider)
			{
				if(provider != null)
				{
					Object obj = provider.GetFormat(typeof(DateTimeFormatInfo));
					if(obj != null)
					{
						return (DateTimeFormatInfo)obj;
					}
				}
				return CurrentInfo;
			}

	// Get the full name of a specific month.
	public String GetMonthName(int month)
			{
				if(month >= 1 && month <= 13)
				{
					return monthNames[month - 1];
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("month", _("ArgRange_Month"));
				}
			}

	// Create a read-only copy of a DateTimeFormatInfo object.
	public static DateTimeFormatInfo ReadOnly(DateTimeFormatInfo dtfi)
			{
				if(dtfi == null)
				{
					throw new ArgumentNullException("dtfi");
				}
				else if(dtfi.IsReadOnly)
				{
					return dtfi;
				}
				else
				{
					DateTimeFormatInfo newDtfi;
					newDtfi = (DateTimeFormatInfo)(dtfi.Clone());
					newDtfi.readOnly = true;
					return newDtfi;
				}
			}

	// Get the abbreviated name of a week day.
	public String GetAbbreviatedDayName(DayOfWeek dayOfWeek)
			{
				if(dayOfWeek >= DayOfWeek.Sunday &&
				   dayOfWeek <= DayOfWeek.Saturday)
				{
					return abbreviatedDayNames[(int)dayOfWeek];
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("dayOfWeek", _("Arg_DayOfWeek"));
				}
			}

	// Get the full name of a week day.
	public String GetDayName(DayOfWeek dayOfWeek)
			{
				if(dayOfWeek >= DayOfWeek.Sunday &&
				   dayOfWeek <= DayOfWeek.Saturday)
				{
					return dayNames[(int)dayOfWeek];
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("dayOfWeek", _("Arg_DayOfWeek"));
				}
			}

#if !ECMA_COMPAT

	// Get the abbreviated name of an era.
	public String GetAbbreviatedEraName(int era)
			{
				if(abbrevEraNames == null)
				{
					// Use the full name if there are no abbreviated names.
					return GetEraName(era);
				}
				if(era == System.Globalization.Calendar.CurrentEra)
				{
					era = Calendar.GetEra(DateTime.Now);
				}
				if(era >= 1 && era <= abbrevEraNames.Length)
				{
					return abbrevEraNames[era - 1];
				}
				else
				{
					throw new ArgumentOutOfRangeException
						("era", _("Arg_InvalidEra"));
				}
			}

#endif // !ECMA_COMPAT

	// Get all date time patterns.
	public String[] GetAllDateTimePatterns()
			{
				String[] patterns = new String [dateTimePatterns.Length];
				int posn;
				for(posn = 0; posn < dateTimePatterns.Length; ++posn)
				{
					patterns[posn] = dateTimePatterns[posn].Substring(2);
				}
				return patterns;
			}
	public String[] GetAllDateTimePatterns(char format)
			{
				String[] patterns;
				int posn, len;
				len = 0;
				for(posn = 0; posn < dateTimePatterns.Length; ++posn)
				{
					if(dateTimePatterns[posn][0] == format)
					{
						++len;
					}
				}
				if(len == 0)
				{
					throw new ArgumentException(_("Arg_DateTimeFormatChar"));
				}
				patterns = new String [len];
				len = 0;
				for(posn = 0; posn < dateTimePatterns.Length; ++posn)
				{
					if(dateTimePatterns[posn][0] == format)
					{
						patterns[len++] = dateTimePatterns[posn].Substring(2);
					}
				}
				return patterns;
			}

	// Properties.
	public String AMDesignator
			{
				get
				{
					return amDesignator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					amDesignator = value;
				}
			}
	public String PMDesignator
			{
				get
				{
					return pmDesignator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					pmDesignator = value;
				}
			}
	public String[] AbbreviatedDayNames
			{
				get
				{
					return abbreviatedDayNames;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(value.Length != 7)
					{
						throw new ArgumentException(_("Arg_Array7Elems"));
					}
					CheckForNulls(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					abbreviatedDayNames = value;
				}
			}
	public String[] DayNames
			{
				get
				{
					return dayNames;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(value.Length != 7)
					{
						throw new ArgumentException(_("Arg_Array7Elems"));
					}
					CheckForNulls(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					dayNames = value;
				}
			}
	public String[] AbbreviatedMonthNames
			{
				get
				{
					return abbreviatedMonthNames;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(value.Length != 13)
					{
						throw new ArgumentException(_("Arg_Array13Elems"));
					}
					CheckForNulls(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					abbreviatedMonthNames = value;
				}
			}
	public String[] MonthNames
			{
				get
				{
					return monthNames;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(value.Length != 13)
					{
						throw new ArgumentException(_("Arg_Array13Elems"));
					}
					CheckForNulls(value);
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					monthNames = value;
				}
			}
	public String DateSeparator
			{
				get
				{
					return dateSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					dateSeparator = value;
				}
			}
	public String TimeSeparator
			{
				get
				{
					return timeSeparator;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					timeSeparator = value;
				}
			}
	public bool IsReadOnly
			{
				get
				{
					return readOnly;
				}
			}
	public String FullDateTimePattern
			{
				get
				{
					return fullDateTimePattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					fullDateTimePattern = value;
				}
			}
	public String LongDatePattern
			{
				get
				{
					return longDatePattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					longDatePattern = value;
				}
			}
	public String LongTimePattern
			{
				get
				{
					return longTimePattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					longTimePattern = value;
				}
			}
	public String MonthDayPattern
			{
				get
				{
					return monthDayPattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					monthDayPattern = value;
				}
			}
	public String ShortDatePattern
			{
				get
				{
					return shortDatePattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					shortDatePattern = value;
				}
			}
	public String ShortTimePattern
			{
				get
				{
					return shortTimePattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					shortTimePattern = value;
				}
			}
	public String YearMonthPattern
			{
				get
				{
					return yearMonthPattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					yearMonthPattern = value;
				}
			}

	// Get or set the calendar in use by this date/time formatting object.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	Calendar Calendar
			{
				get
				{
					return calendar;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}

					// Validate the calendar against the current thread.
					if(value is GregorianCalendar)
					{
						// We can always use the Gregorian calendar.
						calendar = value;
						return;
					}
					Calendar temp = CultureInfo.CurrentCulture.Calendar;
					if(temp != null &&
					   temp.GetType().IsAssignableFrom(value.GetType()))
					{
						calendar = value;
						return;
					}
					Calendar[] opt =
						CultureInfo.CurrentCulture.OptionalCalendars;
					if(opt != null)
					{
						int posn;
						for(posn = 0; posn < opt.Length; ++posn)
						{
							temp = opt[posn];
							if(temp != null &&
							   temp.GetType().IsAssignableFrom(value.GetType()))
							{
								calendar = value;
								return;
							}
						}
					}

					// The calendar is invalid for the current culture.
					throw new ArgumentException(_("Arg_InvalidCalendar"));
				}
			}

#if !ECMA_COMPAT

	// Non-ECMA properties.
	public CalendarWeekRule CalendarWeekRule
			{
				get
				{
					return calendarWeekRule;
				}
				set
				{
					if(value != CalendarWeekRule.FirstDay &&
					   value != CalendarWeekRule.FirstFullWeek &&
					   value != CalendarWeekRule.FirstFourDayWeek)
					{
						throw new ArgumentOutOfRangeException
							(_("Arg_CalendarWeekRule"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					calendarWeekRule = value;
				}
			}
	public DayOfWeek FirstDayOfWeek
			{
				get
				{
					return firstDayOfWeek;
				}
				set
				{
					if(value < DayOfWeek.Sunday || value > DayOfWeek.Saturday)
					{
						throw new ArgumentOutOfRangeException
							(_("Arg_DayOfWeek"));
					}
					if(readOnly)
					{
						throw new InvalidOperationException
							(_("Invalid_ReadOnly"));
					}
					firstDayOfWeek = value;
				}
			}
	public String RFC1123Pattern
			{
				get
				{
					return "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
				}
			}
	public String SortableDateTimePattern
			{
				get
				{
					return "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
				}
			}
	public String UniversalSortableDateTimePattern
			{
				get
				{
					return "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
				}
			}

#endif // !ECMA_COMPAT

	// Check for null strings in an array.
	private static void CheckForNulls(String[] value)
			{
				int index;
				for(index = 0; index < value.Length; ++index)
				{
					if(value[index] == null)
					{
						throw new ArgumentNullException
							("value[" + index.ToString() + "]");
					}
				}
			}

	// Set the era name lists - this should not be used by applications.
	// It exists to support I18N plugins, which have no other way to
	// set this information through the published API's.
	public void I18NSetEraNames(String[] names, String[] abbrevNames)
			{
				if(names == null)
				{
					throw new ArgumentNullException("names");
				}
				CheckForNulls(names);
				if(abbrevNames != null)
				{
					CheckForNulls(abbrevNames);
				}
				eraNames = names;
				abbrevEraNames = abbrevNames;
			}

	// Set the date/time pattern list - this should not be used by
	// applications.  It exists to support I18N plugins.
	public void I18NSetDateTimePatterns(String[] patterns)
			{
				if(patterns == null)
				{
					throw new ArgumentNullException("patterns");
				}
				CheckForNulls(patterns);
				dateTimePatterns = patterns;
			}

}; // class DateTimeFormatInfo

}; // namespace System.Globalization
