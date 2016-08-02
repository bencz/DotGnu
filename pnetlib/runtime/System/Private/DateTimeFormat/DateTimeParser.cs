/*
 * DateTimeParser.cs - Implementation of the
 *		"System.Private.DateTimeFormat.DateTimeParser" class.
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

namespace System.Private.DateTimeFormat
{

using System.Globalization;
using System.Text;

internal sealed class DateTimeParser
{
	// Skip white space in a string.
	private static void SkipWhite(String s, ref int posn)
			{
				while(posn < s.Length && Char.IsWhiteSpace(s[posn]))
				{
					++posn;
				}
			}

	// Parse a decimal number from a string.
	private static int ParseNumber(String s, ref int posn, int maxDigits)
			{
				int value;

				// The first character must be a decimal digit.
				if(s[posn] < '0' || s[posn] > '9')
				{
					throw new FormatException();
				}
				value = (int)(s[posn++] - '0');

				// Recognize additional digits.
				while(maxDigits > 1 && posn < s.Length && s[posn] >= '0'
				      && s[posn] <= '9')
				{
					value = (value * 10) + (int)(s[posn++] - '0');
					--maxDigits;
				}

				// Return the value to the caller.
				return value;
			}

	// Parse a particular string.
	private static void ParseString(String s, ref int posn, String value)
			{
				if((s.Length - posn) < value.Length)
				{
					throw new FormatException();
				}
				if(String.Compare(s, posn, value, 0, value.Length,
								  false, CultureInfo.CurrentCulture) != 0)
				{
					throw new FormatException();
				}
				posn += value.Length;
			}

	// Parse one of the strings in an array.
	private static int ParseOneOf(String s, ref int posn, String[] values)
			{
				int maxlen = 0;
				int found = 0;
				int index;
				CultureInfo culture = CultureInfo.CurrentCulture;
				for(index = 0; index < values.Length; ++index)
				{
					if((s.Length - posn) < values[index].Length)
					{
						continue;
					}
					else if(values[index].Length == 0)
					{
						continue;
					}
					if(String.Compare(s, posn, values[index], 0,
									  values[index].Length, false,
									  culture) == 0)
					{
						if(found == 0 || values[index].Length > maxlen)
						{
							found = index + 1;
							maxlen = values[index].Length;
						}
					}
				}
				if(found == 0)
				{
					throw new FormatException();
				}
				posn += maxlen;
				return found;
			}

	// Parse an era name.
	private static int ParseEra(String s, ref int posn,
								Calendar calendar, DateTimeFormatInfo info)
			{
				// Get the list of eras from the calendar.
				int[] eras = calendar.Eras;

				// Convert the eras into era names.
				String[] eraNames = new String [eras.Length];
				int index;
				for(index = 0; index < eras.Length; ++index)
				{
					eraNames[index] = info.GetEraName(eras[index]);
				}

				// Parse the era value using the strings we just got.
				return ParseOneOf(s, ref posn, eraNames);
			}

	// Parse a timezone designator.
	private static int ParseTimeZone(String s, ref int posn)
			{
				int sign = 1;
				int hour, minute;

				// Process the sign of the timezone;
				if(posn < s.Length)
				{
					if(s[posn] == '-')
					{
						sign = -1;
						++posn;
					}
					else if(s[posn] == '+')
					{
						++posn;
					}
				}

				// Parse the hour portion of the timezone.
				hour = ParseNumber(s, ref posn, 2);

				// Parse the minute portion if there is a ':' separator.
				if(posn < s.Length && s[posn] == ':')
				{
					++posn;
					minute = ParseNumber(s, ref posn, 2);
				}
				else
				{
					minute = 0;
				}

				// Return the final timezone value.
				return sign * (hour * 60 + minute);
			}

	// Try parsing a DateTime value with a specific format.
	// Throws "FormatException" if the parse failed.
	private static DateTime TryParse(String s, String format,
						      		 DateTimeFormatInfo info,
						      		 DateTimeStyles style)
			{
				int posn, sposn;
				char ch;
				int count;
				Calendar calendar = CultureInfo.CurrentCulture.Calendar;

				// Clear the parse information.
				int year = 0;
				int month = 0;
				int day = 0;
				int hour = 0;
				int minute = 0;
				int second = 0;
				int fractions = 0;
				int era = Calendar.CurrentEra;
				int ampm = -1;
				int timezone = 0;

				// Parse the contents of the string.
				posn = 0;
				sposn = 0;
				while(posn < format.Length && sposn < s.Length)
				{
					// Extract the next format character plus its count.
					ch = format[posn++];
					if(ch == ' ')
					{
						// White space is required in this position.
						if(!Char.IsWhiteSpace(s[sposn]))
						{
							throw new FormatException();
						}
						++sposn;
						SkipWhite(s, ref sposn);
						if(sposn >= s.Length)
						{
							break;
						}
						continue;
					}
					else if((style & DateTimeStyles.AllowInnerWhite) != 0)
					{
						// White space is allowed between all components.
						SkipWhite(s, ref sposn);
						if(sposn >= s.Length)
						{
							break;
						}
					}
					count = 1;
					switch(ch)
					{
						case 'd': case 'm': case 'M': case 'y':
						case 'g': case 'h': case 'H': case 's':
						case 'f': case 't': case 'z':
						{
							while(posn < format.Length &&
								  format[posn] == ch)
							{
								++posn;
								++count;
							}
						}
						break;

						case ':':
						{
							// Looking for the time separator.
							ParseString(s, ref sposn, info.TimeSeparator);
							continue;
						}
						// Not reached.

						case '/':
						{
							// Looking for the date separator.
							ParseString(s, ref sposn, info.DateSeparator);
							continue;
						}
						// Not reached.

						case '%':
						{
							// Used to escape custom patterns that would
							// otherwise look like single-letter formats.
							continue;
						}
						// Not reached.

						case '\\':
						{
							// Escape the next character.
							if(posn < format.Length)
							{
								ch = format[posn++];
								if(s[sposn++] != ch)
								{
									throw new FormatException();
								}
							}
							else
							{
								throw new FormatException();
							}
							continue;
						}
						// Not reached.

						case '\'':
						{
							// Quoted text.
							while(posn < format.Length)
							{
								ch = format[posn++];
								if(ch == '\'')
								{
									break;
								}
								if(s[sposn++] != ch)
								{
									throw new FormatException();
								}
							}
							continue;
						}
						// Not reached.

						default:
						{
							// Literal character.
							if(s[sposn++] != ch)
							{
								throw new FormatException();
							}
							continue;
						}
						// Not reached.
					}

					// Process the format character.
					switch(ch)
					{
						case 'd':
						{
							// Parse the day or weekday.  We discard weekdays
							// because they don't add anything to the value.
							if(count == 1 || count == 2)
							{
								day = ParseNumber(s, ref sposn, 2);
							}
							else if(count == 3)
							{
								ParseOneOf(s, ref sposn,
										   info.AbbreviatedDayNames);
							}
							else
							{
								ParseOneOf(s, ref sposn, info.DayNames);
							}
						}
						break;

						case 'M':
						{
							// Parse the month.
							if(count == 1 || count == 2)
							{
								month = ParseNumber(s, ref sposn, 2);
							}
							else if(count == 3)
							{
								month = ParseOneOf(s, ref sposn,
										   info.AbbreviatedMonthNames);
							}
							else
							{
								month = ParseOneOf(s, ref sposn,
										   		   info.MonthNames);
							}
						}
						break;

						case 'y':
						{
							// Parse the year.
							if(count == 1 || count == 2)
							{
								year = calendar.ToFourDigitYear
									(ParseNumber(s, ref sposn, 2));
							}
							else
							{
								year = ParseNumber(s, ref sposn, 4);
							}
						}
						break;

						case 'g':
						{
							// Parse an era name.
							era = ParseEra(s, ref sposn, calendar, info);
						}
						break;

						case 'h':
						{
							// Parse the hour in 12-hour format.
							hour = ParseNumber(s, ref sposn, 2);
						}
						break;

						case 'H':
						{
							// Parse the hour in 24-hour format.
							hour = ParseNumber(s, ref sposn, 2);
						}
						break;

						case 'm':
						{
							// Parse the minute.
							minute = ParseNumber(s, ref sposn, 2);
						}
						break;

						case 's':
						{
							// Parse the second.
							second = ParseNumber(s, ref sposn, 2);
						}
						break;

						case 'f':
						{
							// Parse fractions of a second.
							fractions = ParseNumber(s, ref sposn,
													Int32.MaxValue);
							while(count < 7)
							{
								fractions *= 10;
								++count;
							}
						}
						break;

						case 't':
						{
							// Parse an AM/PM designator.
							if(count == 1)
							{
								ampm = ParseOneOf
									(s, ref sposn, new String[]
										{info.AMDesignator[0].ToString(),
										 info.PMDesignator[0].ToString()});
							}
							else
							{
								ampm = ParseOneOf
									(s, ref sposn, new String[]
										{info.AMDesignator,
										 info.PMDesignator});
							}
						}
						break;

						case 'z':
						{
							// Parse the timezone indicator.
							timezone = ParseTimeZone(s, ref sposn);
						}
						break;
					}
				}

				// At this point, we need to be at the end of both
				// the format string and the parse string.  If we
				// aren't then the parse failed.
				if(posn < format.Length || sposn < s.Length)
				{
					throw new FormatException();
				}

				// Build the final DateTime value and return it.
				if(ampm == 1)
				{
					if(hour == 12)
					{
						hour = 0;
					}
				}
				else if(ampm == 2)
				{
					if(hour < 12)
					{
						hour += 12;
					}
				}
				if(year == 0 && month == 0 && day == 0)
				{
					if((style & DateTimeStyles.NoCurrentDateDefault) == 0)
					{
						DateTime now = DateTime.Now;
						year = calendar.GetYear(now);
						month = calendar.GetMonth(now);
						day = calendar.GetDayOfMonth(now);
					}
					else
					{
						// Use Gregorian 01/01/0001 as the date portion.
						return new DateTime
							(1, 1, 1, hour, minute, second, fractions / 10000);
					}
				}
				else if(day == 0)
				{
					// Probably a "year month" format.
					day = 1;
				}
				DateTime value;
				try
				{
					if(month!=0 || day!=0 || year!=0)
					{
						if(year==0)
						{
							year=DateTime.Now.Year;
						}
						if(month==0)
						{
							month=1;
						}
						if(day==0)
						{
							day=1;
						}
					}
					else
					{
						year=DateTime.Now.Year;
						month=DateTime.Now.Month;
						day=DateTime.Now.Day;
					}
					value = calendar.ToDateTime
						(year, month, day, hour, minute, second,
						 fractions / 10000, era);
				}
				catch(Exception)
				{
					// The calendar didn't like the date/time value,
					// so assume that it could not be parsed correctly.
					throw new FormatException();
				}
				if((style & DateTimeStyles.AdjustToUniversal) != 0)
				{
					value = value.ToUniversalTime();
				}
				return value;
			}

	// Parse a DateTime value, using exact format information.
	public static DateTime ParseExact(String s, String[] formats,
									  IFormatProvider provider,
									  DateTimeStyles style)
			{
				DateTimeFormatInfo info;
				int posn;

				// Validate the parameters.
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(formats == null)
				{
					throw new ArgumentNullException("formats");
				}
				for(posn = 0; posn < formats.Length; ++posn)
				{
					if(formats[posn] == null)
					{
						throw new ArgumentNullException
							("formats[" + posn.ToString() + "]");
					}
					else if(formats[posn] == String.Empty)
					{
						throw new FormatException(_("Format_Empty"));
					}
				}

				// Get the date time format information from the provider.
				info = DateTimeFormatInfo.GetInstance(provider);

				// Strip white space from the incoming string.
				if((style & DateTimeStyles.AllowLeadingWhite) != 0)
				{
					s = s.TrimStart(null);
				}
				if((style & DateTimeStyles.AllowTrailingWhite) != 0)
				{
					s = s.TrimEnd(null);
				}
				if(s == String.Empty)
				{
					throw new FormatException
						(_("ArgRange_StringNonEmpty"));
				}

				// Process each of the formats in turn.  We may need
				// to recursively re-enter ParseExact if single-letter
				// format characters are used within the list.
				for(posn = 0; posn < formats.Length; ++posn)
				{
					try
					{
						if(formats[posn].Length == 1)
						{
							return ParseExact
								(s, info.GetAllDateTimePatterns
										(formats[posn][0]), info, style);
						}
						else
						{
							return TryParse
								(s, formats[posn], info, style);
						}
					}
					catch(FormatException)
					{
						// Didn't match this format.  Try the next one.
					}
				}

				// If we get here, then we were unable to parse the string.
				throw new FormatException(_("Format_DateTime"));
			}

	// Parse a DateTime value, using any format type.
	public static DateTime Parse(String s, IFormatProvider provider,
								 DateTimeStyles style)
			{
				DateTimeFormatInfo info;
				info = DateTimeFormatInfo.GetInstance(provider);
				return ParseExact
					(s, info.GetAllDateTimePatterns(), info, style);
			}

}; // class DateTimeParser

}; // namespace System.Private.DateTimeFormat
