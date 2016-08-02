/*
 * DateTimeFormatter.cs - Implementation of the
 *		"System.Private.DateTimeFormat.DateTimeFormatter" class.
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

internal sealed class DateTimeFormatter
{
	// Format a date value as a string using a particular pattern format.
	public static String Format(String format, DateTime date,
								DateTimeFormatInfo info)
			{
				// Format the date/time value.
				StringBuilder builder = new StringBuilder();
				int posn = 0;
				char ch;
				int count, value;
				while(posn < format.Length)
				{
					// Extract the next format character plus its count.
					ch = format[posn++];
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
							builder.Append(info.TimeSeparator);
							continue;
						}
						// Not reached.

						case '/':
						{
							builder.Append(info.DateSeparator);
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
								builder.Append(format[posn++]);
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
								builder.Append(ch);
							}
							continue;
						}
						// Not reached.

						default:
						{
							// Literal character.
							builder.Append(ch);
							continue;
						}
						// Not reached.
					}

					// Process the format character.
					switch(ch)
					{
						case 'd':
						{
							// Output the day or weekday.
							if(count == 1)
							{
								value = date.Day;
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else if(count == 2)
							{
								value = date.Day;
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
							else if(count == 3)
							{
								builder.Append
									(info.AbbreviatedDayNames
										[(int)(date.DayOfWeek)]);
							}
							else
							{
								builder.Append
									(info.DayNames[(int)(date.DayOfWeek)]);
							}
						}
						break;

						case 'M':
						{
							// Output the month.
							value = date.Month;
							if(count == 1)
							{
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else if(count == 2)
							{
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
							else if(count == 3)
							{
								builder.Append
									(info.AbbreviatedMonthNames[value - 1]);
							}
							else
							{
								builder.Append(info.MonthNames[value - 1]);
							}
						}
						break;

						case 'y':
						{
							// Output the year.
							value = date.Year;
							if(count == 1)
							{
								value %= 100;
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else if(count == 2)
							{
								value %= 100;
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
							else
							{
								builder.Append((char)('0' + (value / 1000)));
								builder.Append
									((char)('0' + ((value / 100 % 10))));
								builder.Append
									((char)('0' + ((value / 10 % 10))));
								builder.Append((char)('0' + (value % 10)));
							}
						}
						break;

						case 'g':
						{
							// Output the era name.
							try
							{
								int era = info.Calendar.GetEra(date);
								builder.Append(info.GetEraName(era));
							}
							catch(ArgumentException)
							{
								// The date does not have an era.
							}
						}
						break;

						case 'h':
						{
							// Output the hour in 12-hour format.
							value = date.Hour;
							if(value == 0)
							{
								value = 12;
							}
							else if(value > 12)
							{
								value -= 12;
							}
							if(count == 1)
							{
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else
							{
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
						}
						break;

						case 'H':
						{
							// Output the hour in 24-hour format.
							value = date.Hour;
							if(count == 1)
							{
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else
							{
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
						}
						break;

						case 'm':
						{
							// Output the minute.
							value = date.Minute;
							if(count == 1)
							{
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else
							{
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
						}
						break;

						case 's':
						{
							// Output the second.
							value = date.Second;
							if(count == 1)
							{
								if(value < 10)
								{
									builder.Append((char)('0' + value));
								}
								else
								{
									builder.Append((char)('0' + (value / 10)));
									builder.Append((char)('0' + (value % 10)));
								}
							}
							else
							{
								builder.Append((char)('0' + (value / 10)));
								builder.Append((char)('0' + (value % 10)));
							}
						}
						break;

						case 'f':
						{
							// Output fractions of a second.
							if(count > 7)
							{
								count = 7;
							}
							long frac = date.Ticks;
							long divisor = TimeSpan.TicksPerSecond;
							while(count > 0)
							{
								divisor /= 10;
								value = (int)((frac / divisor) % 10);
								builder.Append((char)('0' + value));
								frac %= divisor;
								--count;
							}
						}
						break;

						case 't':
						{
							value = date.Hour;
							if(count == 1)
							{
								if(value < 12)
								{
									builder.Append(info.AMDesignator[0]);
								}
								else
								{
									builder.Append(info.PMDesignator[0]);
								}
							}
							else
							{
								if(value < 12)
								{
									builder.Append(info.AMDesignator);
								}
								else
								{
									builder.Append(info.PMDesignator);
								}
							}
						}
						break;

					#if !ECMA_COMPAT
						case 'z':
						{
							long offset =
								TimeZone.CurrentTimeZone
									.GetUtcOffset(date).Ticks;
							int hour, min;
							if(offset >= 0)
							{
								builder.Append('+');
							}
							else
							{
								builder.Append('-');
								offset = -offset;
							}
							hour = (int)(offset / TimeSpan.TicksPerHour);
							offset %= TimeSpan.TicksPerHour;
							min = (int)(offset / TimeSpan.TicksPerMinute);
							if(count == 1)
							{
								if(hour < 10)
								{
									builder.Append((char)('0' + hour));
								}
								else
								{
									builder.Append((char)('0' + (hour / 10)));
									builder.Append((char)('0' + (hour % 10)));
								}
							}
							else if(count == 2)
							{
								builder.Append((char)('0' + (hour / 10)));
								builder.Append((char)('0' + (hour % 10)));
							}
							else
							{
								builder.Append((char)('0' + (hour / 10)));
								builder.Append((char)('0' + (hour % 10)));
								builder.Append(':');
								builder.Append((char)('0' + (min / 10)));
								builder.Append((char)('0' + (min % 10)));
							}
						}
						break;
					#endif
					}
				}

				// Return the formatted string to the caller.
				return builder.ToString();
			}

	// Format a date value as a string.
	public static String Format(DateTime date, String format,
							    IFormatProvider provider)
			{
				DateTimeFormatInfo info;

				// Get the date/time formatting information to use.
				info = DateTimeFormatInfo.GetInstance(provider);

				// Validate the format string.
				if(format == null || format == String.Empty)
				{
					format = "G";
				}
				if(format.Length == 1)
				{
					// Resolve the format code to a custom format string.
					switch(format)
					{
						case "d":	format = info.ShortDatePattern; break;
						case "D":	format = info.LongDatePattern; break;
						case "f":
							format = info.LongDatePattern + " " +
									 info.ShortTimePattern;
							break;
						case "F":	format = info.FullDateTimePattern; break;
						case "g":
							format = info.ShortDatePattern + " " +
									 info.ShortTimePattern;
							break;
						case "G":
							format = info.ShortDatePattern + " " +
									 info.LongTimePattern;
							break;
						case "m": case "M":
							format = info.MonthDayPattern; break;
						#if !ECMA_COMPAT
						case "r": case "R":
							format = info.RFC1123Pattern; break;
						case "s":
							format = info.SortableDateTimePattern; break;
						case "u":
							format = info.UniversalSortableDateTimePattern;
							break;
						#else
						case "r": case "R":
							format = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
							break;
						case "s":
							format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
							break;
						case "u":
							format = "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";
							break;
						#endif
						case "t":
							format = info.ShortTimePattern; break;
						case "T":
							format = info.LongTimePattern; break;
						case "U":
							date = date.ToUniversalTime();
							format = info.FullDateTimePattern;
							break;
						case "y": case "Y":
							format = info.YearMonthPattern; break;

						default:
						{
							throw new FormatException
								(_("Format_FormatString"));
						}
						// Not reached.
					}
				}
				return Format(format, date, info);
			}

}; // class DateTimeFormatter

}; // namespace System.Private.DateTimeFormat
