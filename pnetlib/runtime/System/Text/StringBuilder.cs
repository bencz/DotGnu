/*
 * StringBuilder.cs - Implementation of the "System.Text.StringBuilder" class.
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

namespace System.Text
{

using System;
using System.Runtime.CompilerServices;

public sealed class StringBuilder
{
	// Internal state.
	private String buildString;
	private int    maxCapacity;
	private bool   needsCopy;

	// Minimum recommended capacity.
	private const int MinCapacity = 16;

	// Value to use to round up length resizes.
	// Must be a power of 2 minus 1.
	private const int ResizeRound = 31;

	// Value to round up long strings (longer than 2047)
	private const int ResizeRoundBig = 1023;

	// Constructors.
	public StringBuilder() : this(MinCapacity) {}
	public StringBuilder(int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_StrCapacity"));
				}
				if(capacity < MinCapacity)
				{
					capacity = MinCapacity;
				}
				buildString = String.NewBuilder(null, capacity);
				maxCapacity = Int32.MaxValue;
				needsCopy = false;
			}
	public StringBuilder(String value)
			{
				if(value == null)
				{
					buildString = String.NewBuilder(null, MinCapacity);
				}
				else
				{
					int capacity = value.Length;
					if(capacity < MinCapacity)
					{
						capacity = MinCapacity;
					}
					buildString = String.NewBuilder(value, capacity);
				}
				maxCapacity = Int32.MaxValue;
				needsCopy = false;
			}
#if !ECMA_COMPAT
	public StringBuilder(int capacity, int maxCapacity)
			{
				if(maxCapacity < 1)
				{
					throw new ArgumentOutOfRangeException
						("maxCapacity", _("ArgRange_MaxStrCapacity"));
				}
				else if(capacity < 0 || capacity > maxCapacity)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_StrCapacity"));
				}
				if(capacity < MinCapacity && maxCapacity >= MinCapacity)
				{
					capacity = MinCapacity;
				}
				buildString = String.NewBuilder(null, capacity);
				this.maxCapacity = maxCapacity;
				if(buildString.capacity > maxCapacity)
				{
					buildString.capacity = maxCapacity;
				}
				needsCopy = false;
			}
	public StringBuilder(String value, int capacity)
			{
				if (capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_StrCapacity"));
				}
				if(value != null)
				{
					if(capacity < value.Length)
					{
						buildString = String.NewBuilder(value, -1);
					}
					else
					{
						buildString = String.NewBuilder(value, capacity);
					}
				}
				else
				{
					if(capacity < MinCapacity)
					{
						capacity = MinCapacity;
					}
					buildString = String.NewBuilder(null, capacity);
				}
				maxCapacity = Int32.MaxValue;
				needsCopy = false;
			}
	public StringBuilder(String value, int startIndex,
						 int length, int capacity)
			{
				if(capacity < 0)
				{
					throw new ArgumentOutOfRangeException
						("capacity", _("ArgRange_StrCapacity"));
				}
				if(startIndex < 0)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				if(length < 0)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				// Shrink the substring to the legitimate part of the string.
				if(value != null)
				{
					if((value.Length - startIndex) < length)
					{
						throw new ArgumentOutOfRangeException
							("length", _("ArgRange_Array"));
					}
				}
				else
				{
					if (startIndex != 0 || length != 0)
					{
						throw new ArgumentOutOfRangeException
							("length", _("ArgRange_Array"));
					}
				}

				// Validate the capacity.
				if(capacity < length)
				{
					capacity = length;
				}
				if(capacity < MinCapacity)
				{
					capacity = MinCapacity;
				}

				// Allocate a new string builder and fill it.
				buildString = String.NewBuilder(null, capacity);
				if(length > 0)
				{
					String.Copy(buildString, 0, value, startIndex, length);
					buildString.length = length;
				}
				maxCapacity = Int32.MaxValue;
				needsCopy = false;
			}
#endif // !ECMA_COMPAT

	// Create a new builder and clamp the maximum capacity.
	private String NewBuilder(String value, int capacity)
			{
				String builder = String.NewBuilder(value, capacity);
				if(builder.capacity > maxCapacity)
				{
					builder.capacity = maxCapacity;
				}
				return builder;
			}

	// Append a string to this builder.
	//
	// Note: the ECMA spec is unclear as to what happens when the
	// maximum capacity overflows.  We have chosen to append as
	// much as possible and truncate the rest.
	[MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Append(String value);

	// Append other types of values to this builder.
	public StringBuilder Append(bool value)
			{
				return Append(value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Append(sbyte value)
			{
				return Append(value.ToString());
			}
	public StringBuilder Append(byte value)
			{
				return Append(value.ToString());
			}
	public StringBuilder Append(short value)
			{
				return Append(value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Append(ushort value)
			{
				return Append(value.ToString());
			}
    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Append(char value);

    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Append(char value, int repeatCount);

	public StringBuilder Append(int value)
			{
				return Append(value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Append(uint value)
			{
				return Append(value.ToString());
			}
	public StringBuilder Append(long value)
			{
				return Append(value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Append(ulong value)
			{
				return Append(value.ToString());
			}
#if CONFIG_EXTENDED_NUMERICS
	public StringBuilder Append(float value)
			{
				return Append(value.ToString());
			}
	public StringBuilder Append(double value)
			{
				return Append(value.ToString());
			}
	public StringBuilder Append(Decimal value)
			{
				return Append(value.ToString());
			}
#endif
	public StringBuilder Append(Object value)
			{
				if(value != null)
				{
					return Append(value.ToString());
				}
				else
				{
					return this;
				}
			}

    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Append(String value, int startIndex, int length);

    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Append(char[] value);

	[MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Append(char[] value, int startIndex, int length);

	// Appended formatted values to this string builder.
	public StringBuilder AppendFormat(String format, Object arg0)
			{
				return Append(String.Format(format, arg0));
			}
	public StringBuilder AppendFormat(String format, Object arg0, Object arg1)
			{
				return Append(String.Format(format, arg0, arg1));
			}
	public StringBuilder AppendFormat(String format, Object arg0, Object arg1,
									  Object arg2)
			{
				return Append(String.Format(format, arg0, arg1, arg2));
			}
	public StringBuilder AppendFormat(String format, params Object[] args)
			{
				return Append(String.Format(format, args));
			}
	public StringBuilder AppendFormat(IFormatProvider provider,
									  String format, params Object[] args)
			{
				return Append(String.Format(provider, format, args));
			}

	// Ensure that this string builder has a specific capacity available.
    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public int EnsureCapacity(int capacity);

	// Determine if two StringBuilders are equal.
	public bool Equals(StringBuilder sb)
			{
				if(sb != null)
				{
					return String.Equals(buildString, sb.buildString);
				}
				else
				{
					return false;
				}
			}

	// Insert a string into this builder.
	//
	// Note: the ECMA spec is unclear as to what happens when the
	// maximum capacity overflows.  We have chosen to insert as
	// much as possible and truncate the rest.
    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Insert(int index, String value);

	// Insert other types of values into this builder.
	public StringBuilder Insert(int index, bool value)
			{
				return Insert(index, value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Insert(int index, sbyte value)
			{
				return Insert(index, value.ToString());
			}
	public StringBuilder Insert(int index, byte value)
			{
				return Insert(index, value.ToString());
			}
	public StringBuilder Insert(int index, short value)
			{
				return Insert(index, value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Insert(int index, ushort value)
			{
				return Insert(index, value.ToString());
			}
    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Insert(int index, char value);

	public StringBuilder Insert(int index, int value)
			{
				return Insert(index, value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Insert(int index, uint value)
			{
				return Insert(index, value.ToString());
			}
	public StringBuilder Insert(int index, long value)
			{
				return Insert(index, value.ToString());
			}
	[CLSCompliant(false)]
	public StringBuilder Insert(int index, ulong value)
			{
				return Insert(index, value.ToString());
			}
#if CONFIG_EXTENDED_NUMERICS
	public StringBuilder Insert(int index, float value)
			{
				return Insert(index, value.ToString());
			}
	public StringBuilder Insert(int index, double value)
			{
				return Insert(index, value.ToString());
			}
	public StringBuilder Insert(int index, Decimal value)
			{
				return Insert(index, value.ToString());
			}
#endif
	public StringBuilder Insert(int index, Object value)
			{
				if(value != null)
				{
					return Insert(index, value.ToString());
				}
				else
				{
					return this;
				}
			}
    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Insert(int index, char[] value);

    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Insert(int index, char[] value,
								int startIndex, int length);

    [MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Insert(int index, String value, int count);

	// Remove a range of characters from this string builder.
	public StringBuilder Remove(int startIndex, int length)
			{
				if(startIndex < 0 || startIndex > buildString.length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_StringIndex"));
				}
				else if((buildString.length - startIndex) < length)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_StringRange"));
				}
				else if(needsCopy)
				{
					buildString = NewBuilder
						(buildString, buildString.capacity);
					needsCopy = false;
				}
				String.RemoveSpace(buildString, startIndex, length);
				return this;
			}

	// Replace all occurrences of a specific old string with a new string.
	[MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Replace(char oldChar, char newChar);

	public StringBuilder Replace(String oldValue, String newValue)
			{
				return Replace(oldValue, newValue, 0, buildString.length);
			}
	public StringBuilder Replace(String oldValue, String newValue,
								 int startIndex, int count)
			{
				if(oldValue == null)
				{
					throw new ArgumentNullException("oldValue");
				}
				else if(oldValue.Length == 0)
				{
					throw new ArgumentException
						(_("ArgRange_StringNonEmpty"));
				}
				else if(startIndex < 0 || startIndex > buildString.length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_StringIndex"));
				}
				else if((buildString.length - startIndex) < count)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_StringRange"));
				}
				if(count > 0)
				{
					String temp = buildString;
					buildString = NewBuilder(null, buildString.capacity);
					needsCopy = false;
					if(startIndex > 0)
					{
						Append(temp, 0, startIndex);
					}
					Append(temp.Substring(startIndex, count)
							   .Replace(oldValue, newValue));
					if((startIndex + count) < temp.length)
					{
						Append(temp, startIndex + count,
							   temp.length - (startIndex + count));
					}
				}
				return this;
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
 	extern public StringBuilder Replace(char oldChar, char newChar,
								 int startIndex, int count);

	// Convert this string builder into a string.
	public override String ToString()
			{
				if(needsCopy)
				{
					// Already in its real string form.
					return buildString;
				}
				else if(buildString.Length > (buildString.capacity / 2))
				{
					// Return the builder itself if more than half full.
					needsCopy = true;
					return buildString;
				}
				else
				{
					// Create a new string with the contents.
					return String.Copy(buildString);
				}
			}

	// Convert a sub-range of this string builder into a string.
	public String ToString(int startIndex, int length)
			{
				// Let the String class do the work, including validation.
				return buildString.Substring(startIndex, length);
			}

	// Get or set the capacity of this string builder.
	public int Capacity
			{
				get
				{
					return buildString.capacity;
				}
				set
				{
					if(value < buildString.length)
					{
						throw new ArgumentException
							(_("ArgRange_StrCapacity"));
					}
					EnsureCapacity(value);
				}
			}

	// Get or set a particular character within this string builder.
	// The "IndexerName" attribute ensures that the get
	// accessor method is named according to the ECMA spec.
	[IndexerName("Chars")]
	public char this[int index]
			{
				get
				{
					return buildString.GetChar(index);
				}
				set
				{
					buildString.SetChar(index, value);
				}
			}

	// Get or set the length of this string builder.
	public int Length
			{
				get
				{
					return buildString.length;
				}
				set
				{
					int buildLen;
					if(value < 0 || value > maxCapacity)
					{
						throw new ArgumentOutOfRangeException
							("value", _("ArgRange_StrBuilderLength"));
					}
					buildLen = buildString.length;
					if(value < buildLen)
					{
						Remove(value, buildLen - value);
					}
					else if(value > buildLen)
					{
						Append(' ', value - buildLen);
					}
				}
			}

#if !ECMA_COMPAT
	// Get the maximum capacity for this string builder.
	public int MaxCapacity
			{
				get
				{
					return maxCapacity;
				}
			}
#endif // !ECMA_COMPAT

}; // class StringBuilder

}; // namespace System.Text
