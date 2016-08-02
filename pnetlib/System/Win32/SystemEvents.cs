/*
 * SystemEvents.cs - Implementation of the
 *		Microsoft.Win32.SystemEvents class.
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

namespace Microsoft.Win32
{

#if CONFIG_WIN32_SPECIFICS

using System;
using System.Collections;
using System.Threading;

// System events are highly Windows specific, impossible to emulate
// elsewhere, and arguably a potentional security risk.
//
// We let callers register for events, and then simply never deliver them.
// This should fool the application into believing that the event hasn't
// occurred yet and that it should wait longer for it to occur.
//
// The one exception to the above is timers, which we emulate using
// the "System.Threading.Timer" class.

public sealed class SystemEvents
{

	// This class cannot be instantiated.
	private SystemEvents() {}

	// Execute a delegate on the event listening thread.
	public static void InvokeOnEventsThread(Delegate method)
			{
				// There is no event thread, so execute the delegate here.
				if(method != null)
				{
					method.DynamicInvoke(new Object [0]);
				}
			}

	// System event handlers.
	public static event EventHandler DisplaySettingsChanged;
	public static event EventHandler EventsThreadShutdown;
	public static event EventHandler InstalledFontsChanged;
	public static event EventHandler LowMemory;
	public static event EventHandler PaletteChanged;
	public static event PowerModeChangedEventHandler PowerModeChanged;
	public static event SessionEndedEventHandler SessionEnded;
	public static event SessionEndingEventHandler SessionEnding;
	public static event EventHandler TimeChanged;
	public static event TimerElapsedEventHandler TimerElapsed;
	public static event UserPreferenceChangedEventHandler
			UserPreferenceChanged;
	public static event UserPreferenceChangingEventHandler
			UserPreferenceChanging;

	// List of currently active timers.
	private static ArrayList timers;

	// Timer information block.
	private sealed class TimerInfo
	{
		public int timerId;
		public Timer timer;

	}; // class TimerInfo

	// Handle a timeout.
	private static void Timeout(Object state)
			{
				TimerInfo info = (TimerInfo)state;
				lock(typeof(SystemEvents))
				{
					if(timers[info.timerId] == info)
					{
						timers[info.timerId] = null;
					}
					if(info.timer != null)
					{
						info.timer.Dispose();
						info.timer = null;
					}
				}
				if(TimerElapsed != null)
				{
					TimerElapsed(null,
						new TimerElapsedEventArgs
							(new IntPtr(info.timerId + 1)));
				}
			}

	// Create a timer.
	public static IntPtr CreateTimer(int interval)
			{
				lock(typeof(SystemEvents))
				{
					// Create the timer information block.
					TimerInfo info = new TimerInfo();

					// Allocate a new timer identifier.
					if(timers == null)
					{
						timers = new ArrayList();
					}
					int timerId = 0;
					while(timerId < timers.Count &&
						  timers[timerId] != null)
					{
						++timerId;
					}
					if(timerId >= timers.Count)
					{
						timers.Add(info);
					}
					else
					{
						timers[timerId] = info;
					}

					// Initialize the timer information block.
					info.timerId = timerId;
					info.timer = new Timer(new TimerCallback(Timeout),
										   info, interval, -1);

					// Return the timer identifier, and make sure
					// that it will never be zero.
					return new IntPtr(timerId + 1);
				}
			}

	// Kill a previously installed timer.
	public static void KillTimer(IntPtr timerId)
			{
				int timId = (unchecked((int)(timerId.ToInt64())) - 1);
				lock(typeof(SystemEvents))
				{
					if(timers != null && timId >= 0 && timId < timers.Count)
					{
						TimerInfo info = (TimerInfo)(timers[timId]);
						if(info != null && info.timerId == timId)
						{
							timers[timId] = null;
							if(info.timer != null)
							{
								info.timer.Dispose();
								info.timer = null;
							}
						}
					}
				}
			}

}; // class SystemEvents

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
