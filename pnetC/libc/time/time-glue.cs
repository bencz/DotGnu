/*
 * time-glue.cs - Glue between time and the C# system library.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace OpenSystem.C
{

using System;
using System.Runtime.InteropServices;
using System.Threading;

[GlobalScope]
public class LibCTime
{

	// Get the current system time in UTC.
	public static long __syscall_utc_time()
			{
				return DateTime.UtcNow.Ticks;
			}

	// Get the current system time in local time.
	public static long __syscall_local_time()
			{
				return DateTime.Now.Ticks;
			}

	// Unpack a tick value into a "struct tm" structure.
	public static void __syscall_unpack_time
				(long ticks, IntPtr tm, bool is_local)
			{
				DateTime dt;
				if(is_local)
				{
					long tz = __syscall_utc_time() - __syscall_local_time();
					dt = new DateTime(ticks + tz);
				}
				else
				{
					dt = new DateTime(ticks);
				}
				Marshal.WriteInt32(tm, 0, dt.Second);
				Marshal.WriteInt32(tm, 4, dt.Minute);
				Marshal.WriteInt32(tm, 8, dt.Hour);
				Marshal.WriteInt32(tm, 12, dt.Day);
				Marshal.WriteInt32(tm, 16, dt.Month - 1);
				Marshal.WriteInt32(tm, 20, dt.Year - 1900);
				Marshal.WriteInt32(tm, 24, (int)(dt.DayOfWeek));
				Marshal.WriteInt32(tm, 28, dt.DayOfYear);
				Marshal.WriteInt32(tm, 32, 0);	/* TODO - tm_isdst */
			}

	// Sleep for a number of ticks.
	public static void __syscall_sleep_ticks(long ticks)
			{
				Thread.Sleep(new TimeSpan(ticks));
			}

} // class LibCTime

} // namespace OpenSystem.C
