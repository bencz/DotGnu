/*
 * Stopwatch.cs - Implementation of the
 *			"System.Diagnostics.Stopwatch" class.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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

namespace System.Diagnostics
{

using System;
using System.Runtime.CompilerServices;

#if CONFIG_FRAMEWORK_2_0 && CONFIG_EXTENDED_DIAGNOSTICS

public class Stopwatch
{
	// static members
	public static readonly bool IsHighResolution;
	public static readonly long Frequency;

	// instance members
	private bool isRunning;
	private long elapsed;
	private long start;

	// Get the frequency of the performance counter.
	// If a high resolution timer is supported this function returns true.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool GetPerformanceFrequency(out long frequency);

	// Get the value of the high resolution timer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static long GetPerformanceCounter();
	
	static Stopwatch()
			{
				IsHighResolution = GetPerformanceFrequency(out Frequency);
			}


	public static long GetTimestamp()
			{
				return GetPerformanceCounter();
			}

	public static Stopwatch StartNew()
			{
				Stopwatch stopwatch = new Stopwatch();

				stopwatch.Start();

				return stopwatch;
			}

	public Stopwatch()
			{
				isRunning = false;
				elapsed = 0;
				start = 0;
			}

	public void Reset()
			{
				isRunning = false;
				elapsed = 0;
				start = 0;
			}

	public void Start()
			{
				if(!isRunning)
				{
					start = GetPerformanceCounter();
					isRunning = true;
				}
			}

	public void Stop()
			{
				if(isRunning)
				{
					isRunning = false;
					elapsed += (GetPerformanceCounter() - start);
				}
			}

	public TimeSpan Elapsed
			{
				get
				{
					long ticks = elapsed;

					if(isRunning)
					{
						ticks += (GetPerformanceCounter() - start);
					}

					if(IsHighResolution)
					{
						return new TimeSpan(ticks / (Frequency / 10000000L));
					}
					else
					{
						return new TimeSpan(ticks);
					}
				}
			}

	public long ElapsedMilliseconds
			{
				get
				{
					long ticks = elapsed;

					if(isRunning)
					{
						ticks += (GetPerformanceCounter() - start);
					}

					// We assume Frequency >= 1000
					return ticks / (Frequency / 1000);

				}
			}

	public long ElapsedTicks
			{
				get
				{
					long ticks = elapsed;

					if(isRunning)
					{
						ticks += (GetPerformanceCounter() - start);
					}

					return ticks;
				}

			}

	public bool IsRunning
			{
				get
				{
					return isRunning;
				}
			}

}; // class Stopwatch

#endif  // CONFIG_FRAMEWORK_2_0 && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
