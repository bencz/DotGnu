/*
 * SoapDuration.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapDuration" class.
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

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{

#if CONFIG_SERIALIZATION

using System.Text;

// This class processes durations in ISO/XSD notation:
//
//			[-]PxxxxYxxMxxDTxxHxxMxxS
//			[-]PxxxxYxxMxxDTxxHxxMxx.xxxxxxxS

public sealed class SoapDuration
{
	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "duration";
				}
			}

	// Parse a value into a TimeSpan instance.
	public static TimeSpan Parse(String value)
			{
				int sign;
				int posn;

				// Bail out early if no value supplied.
				if(value == null)
				{
					return TimeSpan.Zero;
				}
				else if(value == String.Empty)
				{
					return TimeSpan.Zero;
				}

				// Process the sign.
				if(value[0] == '-')
				{
					sign = -1;
					posn = 1;
				}
				else
				{
					sign = 0;
					posn = 0;
				}

				// Initialize the field values.
				int years = 0;
				int months = 0;
				int days = 0;
				int hours = 0;
				int minutes = 0;
				int seconds = 0;
				int fractions = 0;

				// Process the fields within the value.
				char ch;
				int val = 0;
				int numDigits = 0;
				bool withinTime = false;
				bool withinFractions = false;
				while(posn < value.Length)
				{
					ch = value[posn++];
					if(ch >= '0' && ch <= '9')
					{
						val = val * 10 + (int)(ch - '0');
						++numDigits;
						if(numDigits >= 9)
						{
							throw new RemotingException
								(_("Arg_InvalidSoapValue"));
						}
					}
					else
					{
						switch(ch)
						{
							case 'P': case 'Z':		break;
							case 'T':				withinTime = true; break;
							case 'Y':				years = val; break;

							case 'M':
							{
								if(withinTime)
								{
									minutes = val;
								}
								else
								{
									months = val;
								}
							}
							break;

							case 'D':				days = val; break;
							case 'H':				hours = val; break;

							case '.':
							{
								seconds = val;
								withinFractions = true;
							}
							break;

							case 'S':
							{
								if(withinFractions)
								{
									while(numDigits < 7)
									{
										val *= 10;
									}
									while(numDigits > 7)
									{
										val /= 10;
									}
									fractions = val;
								}
								else
								{
									seconds = val;
								}
							}
							break;

							default:
							{
								throw new RemotingException
									(_("Arg_InvalidSoapValue"));
							}
							// Not reached.
						}
						val = 0;
						numDigits = 0;
					}
				}

				// Build the final value and return.
				long ticks = years * (360 * TimeSpan.TicksPerDay) +
							 months * (30 * TimeSpan.TicksPerDay) +
							 days * TimeSpan.TicksPerDay +
							 hours * TimeSpan.TicksPerHour +
							 minutes * TimeSpan.TicksPerHour +
							 seconds * TimeSpan.TicksPerSecond +
							 fractions;
				return new TimeSpan(sign * ticks);
			}

	// Convert a time span into a string.
	public static String ToString(TimeSpan value)
			{
				StringBuilder builder = new StringBuilder(32);

				// Process the duration's sign.
				long ticks = value.Ticks;
				if(ticks < 0)
				{
					builder.Append('-');
					ticks = -ticks;
				}

				// Calculate the years and months, using a standard
				// month length of 30 and a year length of 360.
				long days = ticks / TimeSpan.TicksPerDay;
				int years = (int)(days / 360);
				days -= years * 360L;
				int months = (int)(days / 30);
				days -= months * 30L;

				// Output the date portion.
				builder.Append('P');
				builder.Append(years);
				builder.Append('Y');
				builder.Append(months);
				builder.Append('M');
				builder.Append(days);
				builder.Append('D');

				// Output the time portion.
				ticks %= TimeSpan.TicksPerDay;
				builder.Append('T');
				builder.Append(ticks / TimeSpan.TicksPerHour);
				builder.Append('H');
				ticks %= TimeSpan.TicksPerHour;
				builder.Append(ticks / TimeSpan.TicksPerMinute);
				builder.Append('M');
				ticks %= TimeSpan.TicksPerMinute;
				builder.Append(ticks / TimeSpan.TicksPerSecond);
				ticks %= TimeSpan.TicksPerSecond;
				if(ticks != 0)
				{
					builder.Append('.');
					builder.Append(String.Format("{0:D7}", ticks));
				}
				builder.Append('S');

				// Return the final string.
				return builder.ToString();
			}

}; // class SoapDuration

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
