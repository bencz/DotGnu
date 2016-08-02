/*
 * Math.cs - Implementation of the "System.Math" class for when
 *           the base class library does not have it.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

#if !CONFIG_EXTENDED_NUMERICS

internal sealed class Math
{

	// Get the maximum of two values.
	[CLSCompliant(false)]
	public static sbyte Max(sbyte val1, sbyte val2)
			{
				return (sbyte)((val1 > val2) ? val1 : val2);
			}
	public static byte Max(byte val1, byte val2)
			{
				return (byte)((val1 > val2) ? val1 : val2);
			}
	public static short Max(short val1, short val2)
			{
				return (short)((val1 > val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ushort Max(ushort val1, ushort val2)
			{
				return (ushort)((val1 > val2) ? val1 : val2);
			}
	public static int Max(int val1, int val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static uint Max(uint val1, uint val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	public static long Max(long val1, long val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ulong Max(ulong val1, ulong val2)
			{
				return ((val1 > val2) ? val1 : val2);
			}

	// Get the minimum of two values.
	[CLSCompliant(false)]
	public static sbyte Min(sbyte val1, sbyte val2)
			{
				return (sbyte)((val1 < val2) ? val1 : val2);
			}
	public static byte Min(byte val1, byte val2)
			{
				return (byte)((val1 < val2) ? val1 : val2);
			}
	public static short Min(short val1, short val2)
			{
				return (short)((val1 < val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ushort Min(ushort val1, ushort val2)
			{
				return (ushort)((val1 < val2) ? val1 : val2);
			}
	public static int Min(int val1, int val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static uint Min(uint val1, uint val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	public static long Min(long val1, long val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}
	[CLSCompliant(false)]
	public static ulong Min(ulong val1, ulong val2)
			{
				return ((val1 < val2) ? val1 : val2);
			}

}; // class Math

#endif // !CONFIG_EXTENDED_NUMERICS

}; // namespace System
