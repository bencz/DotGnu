/*
 * Monitor.cs - Implementation of the "System.Threading.Monitor" class.
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

namespace System.Threading
{

using System.Runtime.CompilerServices;

public sealed class Monitor
{
	// This class cannot be instantiated.
	private Monitor() {}

	// Enter a monitor on an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Enter(Object obj);

	// Try to enter a monitor on an object.
	public static bool TryEnter(Object obj)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				return InternalTryEnter(obj, 0);
			}

	// Try to enter a monitor on an object within a specified timeout.
	public static bool TryEnter(Object obj, int millisecondsTimeout)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				return InternalTryEnter(obj, millisecondsTimeout);
			}
	public static bool TryEnter(Object obj, TimeSpan timeout)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				return InternalTryEnter(obj, TimeSpanToMS(timeout));
			}

	// Internal version of "TryEnter".  A timeout of -1 indicates
	// infinite, and zero indicates "test and return immediately".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool InternalTryEnter(Object obj, int timeout);

	// Leave a monitor on an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Exit(Object obj);

	// Wait on an object's monitor.
	public static bool Wait(Object obj)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				return InternalWait(obj, -1);
			}

	// Wait on an object's monitor until a specified timeout.
	public static bool Wait(Object obj, int millisecondsTimeout)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				return InternalWait(obj, millisecondsTimeout);
			}
	public static bool Wait(Object obj, TimeSpan timeout)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				return InternalWait(obj, TimeSpanToMS(timeout));
			}
#if !ECMA_COMPAT
	public static bool Wait(Object obj, int millisecondsTimeout,
							bool exitContext)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				return InternalWait(obj, millisecondsTimeout);
			}
	public static bool Wait(Object obj, TimeSpan timeout, bool exitContext)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				return InternalWait(obj, TimeSpanToMS(timeout));
			}
#endif

	// Internal version of "Wait".  A timeout of -1 indicates
	// infinite, and zero indicates "test and return immediately".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool InternalWait(Object obj, int timeout);

	// Pulse a thread that is waiting on an object's monitor.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Pulse(Object obj);

	// Pulse all threads that are waiting on an object's monitor.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void PulseAll(Object obj);

	// Convert a TimeSpan timeout into a number of milliseconds.
	internal static int TimeSpanToMS(TimeSpan timeout)
			{
				long ms = timeout.Ticks;
				if(ms == -1L)
				{
					return -1;
				}
				ms /= TimeSpan.TicksPerMillisecond;
				if(ms < -1L || ms > (long)Int32.MaxValue)
				{
					throw new ArgumentOutOfRangeException
						("timeout", _("ArgRange_NonNegOrNegOne"));
				}
				return unchecked((int)ms);
			}

}; // class Monitor

}; // namespace System.Threading
