/*
 * monitor.cs - Access to C# monitor operations.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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
using System.Threading;
using System.Runtime.InteropServices;

[GlobalScope]
public class LibCMonitor
{
	// Initialize a monitor.
	public static unsafe void __libc_monitor_init(void **monitor)
			{
				lock(typeof(Monitor))
				{
					if(*monitor == null)
					{
						Object mon = new Object();
						*monitor = (void *)(IntPtr)(GCHandle.Alloc(mon));
					}
				}
			}

	// Destroy a monitor.
	public static unsafe void __libc_monitor_destroy(void **monitor)
			{
				if(*monitor != null)
				{
					GCHandle handle = (GCHandle)(IntPtr)(*monitor);
					handle.Free();
					*monitor = null;
				}
			}

	// Lock a monitor.
	public static unsafe void __libc_monitor_lock(void **monitor)
			{
				__libc_monitor_init(monitor);
				Monitor.Enter(((GCHandle)(IntPtr)(*monitor)).Target);
			}

	// Try to lock a monitor, with a defined timeout in ticks.
	public static unsafe int __libc_monitor_trylock
				(void **monitor, long timeout)
			{
				__libc_monitor_init(monitor);
				if(Monitor.TryEnter(((GCHandle)(IntPtr)(*monitor)).Target,
									new TimeSpan(timeout)))
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

	// Unlock a monitor.
	public static unsafe void __libc_monitor_unlock(void **monitor)
			{
				__libc_monitor_init(monitor);
				Monitor.Exit(((GCHandle)(IntPtr)(*monitor)).Target);
			}

	// Wait for a monitor to be pulsed.
	public static unsafe int __libc_monitor_wait(void **monitor, long timeout)
			{
				__libc_monitor_init(monitor);
				if(Monitor.Wait(((GCHandle)(IntPtr)(*monitor)).Target,
								new TimeSpan(timeout)))
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

	// Pulse a monitor, waking up one waiting thread.
	public static unsafe void __libc_monitor_pulse(void **monitor)
			{
				__libc_monitor_init(monitor);
				Monitor.Pulse(((GCHandle)(IntPtr)(*monitor)).Target);
			}

	// Pulse a monitor, waking up all waiting threads.
	public static unsafe void __libc_monitor_pulseall(void **monitor)
			{
				__libc_monitor_init(monitor);
				Monitor.PulseAll(((GCHandle)(IntPtr)(*monitor)).Target);
			}

} // class LibCMonitor

} // namespace OpenSystem.C
