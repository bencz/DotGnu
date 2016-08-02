/*
 * Interlocked.cs - Implementation of the "System.Threading.Interlocked" class.
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

namespace System.Threading
{

using System.Runtime.CompilerServices;

public sealed class Interlocked
{
	// This class cannot be instantiated.
	private Interlocked() {}

	// Compare two values and exchange if equal.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int CompareExchange(ref int location1,
										     int value, int comparand);

#if CONFIG_EXTENDED_NUMERICS
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static float CompareExchange(ref float location1,
											   float value, float comparand);
#endif

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Object CompareExchange(ref Object location1,
											    Object value,
												Object comparand);

	// Perform an atomic decrement on a location.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Decrement(ref int location);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long Decrement(ref long location);

	// Exchange the contents of a location and a value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Exchange(ref int location, int value);

#if CONFIG_EXTENDED_NUMERICS
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static float Exchange(ref float location, float value);
#endif

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Object Exchange(ref Object location, Object value);

	// Perform an atomic increment on a location.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Increment(ref int location);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long Increment(ref long location);

}; // class Interlocked

}; // namespace System.Threading
