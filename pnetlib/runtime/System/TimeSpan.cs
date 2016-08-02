/*
 * TimeSpan.cs - Implementation of the "System.TimeSpan" class.
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

namespace System
{

public struct TimeSpan : IComparable
#if CONFIG_FRAMEWORK_2_0
	, IComparable<TimeSpan>, IEquatable<TimeSpan>
#endif
{
	internal long value_;

	// Public constants.
	public static readonly TimeSpan MinValue =
			new TimeSpan(unchecked((long)0x8000000000000000));
	public static readonly TimeSpan MaxValue =
			new TimeSpan(unchecked((long)0x7FFFFFFFFFFFFFFF));
	public static readonly TimeSpan Zero = new TimeSpan(0);
	public const long TicksPerMillisecond = 10000L;
	public const long TicksPerSecond = 10000L * 1000L;
	public const long TicksPerMinute = 10000L * 1000L * 60L;
	public const long TicksPerHour = 10000L * 1000L * 60L * 60L;
	public const long TicksPerDay = 10000L * 1000L * 60L * 60L * 24L;

	// Constructors.
	public TimeSpan(long ticks)
			{
				value_ = ticks;
			}
	public TimeSpan(int hours, int minutes, int seconds)
			: this(0, hours, minutes, seconds, 0) {}
	public TimeSpan(int days, int hours, int minutes, int seconds)
			: this(days, hours, minutes, seconds, 0) {}
	public TimeSpan(int days, int hours, int minutes, int seconds,
					int milliseconds)
			{
				try
				{
					checked
					{
						value_ = ((long)days) * TicksPerDay +
								 ((long)hours) * TicksPerHour +
								 ((long)minutes) * TicksPerMinute +
								 ((long)seconds) * TicksPerSecond +
								 ((long)milliseconds) * TicksPerMillisecond;
					}
				}
				catch(OverflowException)
				{
					throw new ArgumentOutOfRangeException
						(_("ArgRange_TimeSpan"));
				}
			}

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(!(value is TimeSpan))
					{
						throw new ArgumentException(_("Arg_MustBeTimeSpan"));
					}
					long value2 = ((TimeSpan)value).value_;
					if(value_ < value2)
					{
						return -1;
					}
					else if(value_ > value2)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<TinaSpan> interface.
	public int CompareTo(TimeSpan value)
			{
				if(value_ < value.value_)
				{
					return -1;
				}
				else if(value_ > value.value_)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

	// Implementation of the IEquatable<TimeSpan> interface.
	public bool Equals(TimeSpan obj)
			{
				return (value_ == obj.value_);
			}

#endif // CONFIG_FRAMEWORK_2_0

	// Convert an integer value into two digits.
	private static String TwoDigits(int value)
			{
				char[] result = new char[2];
				result[0] = unchecked((char)((value / 10) + (int)'0'));
				result[1] = unchecked((char)((value % 10) + (int)'0'));
				return new String(result);
			}

	// Formatting.
	public override String ToString()
			{
				int days = (int) Math.Abs(Days);
				int hours = (int) Math.Abs(Hours);
				int minutes = (int) Math.Abs(Minutes);
				int seconds = (int) Math.Abs(Seconds);
				int fractional = unchecked((int)(value_ % TicksPerSecond));
				fractional = (int) Math.Abs(fractional);

				String result;
				if(value_ < 0)
				{
					result = "-";
				}
				else
				{
					result = String.Empty;
				}

				if(days != 0)
				{
					result += days.ToString() + ".";
				}

				result = result + TwoDigits(hours) + ":" +
						 TwoDigits(minutes) + ":" + TwoDigits(seconds);
				if(fractional != 0)
				{
					int test = unchecked((int)(TicksPerSecond / 10));
					int digit;
					result = result + ".";
					while(fractional != 0)
					{
						digit = fractional / test;
						result = result + unchecked((char)(digit + (int)'0'));
						fractional %= test;
						test /= 10;
					}
				}
				return result;
			}

	// Add TimeSpan values.
	public TimeSpan Add(TimeSpan ts)
			{
				checked
				{
					return new TimeSpan(value_ + ts.value_);
				}
			}

	// Compare TimeSpan values.
	public static int Compare(TimeSpan t1, TimeSpan t2)
			{
				if(t1.value_ < t2.value_)
				{
					return -1;
				}
				else if(t1.value_ > t2.value_)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

	// Return the absolute duration of a TimeSpan value.
	public TimeSpan Duration()
			{
				if(value_ < 0)
				{
					return new TimeSpan(-value_);
				}
				else
				{
					return this;
				}
			}

	// Determine if this TimeSpan object is equal to another Object.
	public override bool Equals(Object value)
			{
				if(value is TimeSpan)
				{
					return (value_ == ((TimeSpan)value).value_);
				}
				else
				{
					return false;
				}
			}

	// Determine if two TimeSpan values are equal.
	public static bool Equals(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ == t2.value_);
			}

#if CONFIG_EXTENDED_NUMERICS
	// Convert a floating point number of days into a TimeSpan.
	public static TimeSpan FromDays(double value)
			{
				if(Double.IsNaN(value))
				{
					throw new ArgumentException(_("Arg_NotANumber"));
				}
				else if(Double.IsNegativeInfinity(value))
				{
					return MaxValue;
				}
				else if(Double.IsPositiveInfinity(value))
				{
					return MinValue;
				}
				checked
				{
					return new TimeSpan((long)(value * (double)TicksPerDay));
				}
			}

	// Convert a floating point number of hours into a TimeSpan.
	public static TimeSpan FromHours(double value)
			{
				if(Double.IsNaN(value))
				{
					throw new ArgumentException(_("Arg_NotANumber"));
				}
				else if(Double.IsNegativeInfinity(value))
				{
					return MaxValue;
				}
				else if(Double.IsPositiveInfinity(value))
				{
					return MinValue;
				}
				checked
				{
					return new TimeSpan((long)(value * (double)TicksPerHour));
				}
			}

	// Convert a floating point number of milliseconds into a TimeSpan.
	public static TimeSpan FromMilliseconds(double value)
			{
				if(Double.IsNaN(value))
				{
					throw new ArgumentException(_("Arg_NotANumber"));
				}
				else if(Double.IsNegativeInfinity(value))
				{
					return MaxValue;
				}
				else if(Double.IsPositiveInfinity(value))
				{
					return MinValue;
				}
				checked
				{
					return new TimeSpan
						((long)(value * (double)TicksPerMillisecond));
				}
			}

	// Convert a floating point number of minutes into a TimeSpan.
	public static TimeSpan FromMinutes(double value)
			{
				if(Double.IsNaN(value))
				{
					throw new ArgumentException(_("Arg_NotANumber"));
				}
				else if(Double.IsNegativeInfinity(value))
				{
					return MaxValue;
				}
				else if(Double.IsPositiveInfinity(value))
				{
					return MinValue;
				}
				checked
				{
					return new TimeSpan
							((long)(value * (double)TicksPerMinute));
				}
			}

	// Convert a floating point number of seconds into a TimeSpan.
	public static TimeSpan FromSeconds(double value)
			{
				if(Double.IsNaN(value))
				{
					throw new ArgumentException(_("Arg_NotANumber"));
				}
				else if(Double.IsNegativeInfinity(value))
				{
					return MaxValue;
				}
				else if(Double.IsPositiveInfinity(value))
				{
					return MinValue;
				}
				checked
				{
					return new TimeSpan
							((long)(value * (double)TicksPerSecond));
				}
			}
#endif // CONFIG_EXTENDED_NUMERICS

	// Convert a number of ticks into a TimeSpan.
	public static TimeSpan FromTicks(long ticks)
			{
				return new TimeSpan(ticks);
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return unchecked((int)(value_ ^ (value_ >> 16)));
			}

	// Negate a TimeSpan value.
	public TimeSpan Negate()
			{
				if(value_ != Int64.MinValue)
				{
					return new TimeSpan(unchecked(-value_));
				}
				else
				{
					throw new OverflowException
						(_("Overflow_NegateTwosCompNum"));
				}
			}

	// Parse a string into a TimeSpan value.
	public static TimeSpan Parse (String s)
			{
				long numberofticks = 0;
				int days = 0, hours, minutes, seconds;
				long fractions;
				int fractionslength = 0;
				String fractionss = String.Empty;
				String[] tempstringarray;
				bool minus = false;
	
				//Precheck for null reference
				if (s == null)
				{
					throw new ArgumentNullException("s", _("Arg_NotNull"));
				}
	
				try
				{
	
					//Cut of whitespace and check for minus specifier
					s = s.Trim();
					minus = s.StartsWith("-");
	
					//Get days if present
					if ((s.IndexOf(".") < s.IndexOf(":")) && (s.IndexOf(".") != -1))
					{
						days = Int32.Parse(s.Substring(0, s.IndexOf(".")));
						s = s.Substring(s.IndexOf(".") + 1);
					}
	
					//Get fractions if present
					if ((s.IndexOf(".") > s.IndexOf(":")) && (s.IndexOf(".") != -1))
					{
						fractionss = s.Substring(s.IndexOf(".") + 1);
						fractionslength = fractionss.Length;
						s = s.Substring(0, s.IndexOf("."));
					}
	
					//Parse the hh:mm:ss string
					tempstringarray = s.Split(':');
					hours = Int32.Parse(tempstringarray[0]);
					minutes = Int32.Parse(tempstringarray[1]);
					seconds = Int32.Parse(tempstringarray[2]);
	
	
				}
	
				catch
				{
					throw new FormatException(_("Exception_Format"));
				}

				//Check for overflows
				if ( ((hours > 23) || (hours < 0)) ||
					((minutes > 59) || (minutes < 0)) ||
					((seconds > 59) || (seconds < 0)) ||
					((fractionslength > 7) || (fractionslength < 1)) )
				{
					throw new OverflowException(_("Arg_DateTimeRange"));
				}

				//Calculate the fractions expressed in a second
				if(fractionss != String.Empty)
				{
					fractions = Int32.Parse(fractionss) * TicksPerSecond;
					while(fractionslength > 0)
					{
						fractions /= 10;
						--fractionslength;
					}
				}
				else
				{
					fractions = 0;
				}

				//Calculate the numberofticks
				numberofticks += (days * TicksPerDay);
				numberofticks += (hours * TicksPerHour);
				numberofticks += (minutes * TicksPerMinute);
				numberofticks += (seconds * TicksPerSecond);
				numberofticks += fractions;

				//Apply the minus specifier
				if (minus == true) numberofticks = 0 - numberofticks;

				//Last check
				if ((numberofticks < MinValue.Ticks) || (numberofticks > MaxValue.Ticks))
				{
					throw new OverflowException(_("Arg_DateTimeRange"));
				}

				//Return
				return new TimeSpan(numberofticks);
			}

	// Subtract TimeSpan values.
	public TimeSpan Subtract(TimeSpan ts)
			{
				checked
				{
					return new TimeSpan(value_ - ts.value_);
				}
			}

	// Operators.
	public static TimeSpan operator+(TimeSpan t1, TimeSpan t2)
			{
				return t1.Add(t2);
			}
	public static TimeSpan operator-(TimeSpan t1, TimeSpan t2)
			{
				return t1.Subtract(t2);
			}
	public static TimeSpan operator-(TimeSpan ts)
			{
				return ts.Negate();
			}
	public static TimeSpan operator+(TimeSpan ts)
			{
				return ts;
			}
	public static bool operator==(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ == t2.value_);
			}
	public static bool operator!=(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ != t2.value_);
			}
	public static bool operator>(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ > t2.value_);
			}
	public static bool operator>=(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ >= t2.value_);
			}
	public static bool operator<(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ < t2.value_);
			}
	public static bool operator<=(TimeSpan t1, TimeSpan t2)
			{
				return (t1.value_ <= t2.value_);
			}

	// Properties.
	public int Days
			{
				get
				{
					return unchecked((int)(value_ / TicksPerDay));
				}
			}
	public int Hours
			{
				get
				{
					return unchecked((int)((value_ / TicksPerHour) % 24));
				}
			}
	public int Minutes
			{
				get
				{
					return unchecked((int)((value_ / TicksPerMinute) % 60));
				}
			}
	public int Seconds
			{
				get
				{
					return unchecked((int)((value_ / TicksPerSecond) % 60));
				}
			}
	public int Milliseconds
			{
				get
				{
					return unchecked
						((int)((value_ / TicksPerMillisecond) % 1000));
				}
			}
	public long Ticks
			{
				get
				{
					return value_;
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	public double TotalDays
			{
				get
				{
					return ((double)value_) / ((double)TicksPerDay);
				}
			}
	public double TotalHours
			{
				get
				{
					return ((double)value_) / ((double)TicksPerHour);
				}
			}
	public double TotalMinutes
			{
				get
				{
					return ((double)value_) / ((double)TicksPerMinute);
				}
			}
	public double TotalSeconds
			{
				get
				{
					return ((double)value_) / ((double)TicksPerSecond);
				}
			}
	public double TotalMilliseconds
			{
				get
				{
					double temp = ((double)value_) / ((double)TicksPerMillisecond);
					if(temp > 922337203685477.0)
					{
						return 922337203685477.0;
					}
					else if(temp < -922337203685477.0)
					{
						return -922337203685477.0;
					}
					else
					{
						return temp;
					}
				}
			}
#endif // CONFIG_EXTENDED_NUMERICS

}; // class TimeSpan

}; // namespace System
