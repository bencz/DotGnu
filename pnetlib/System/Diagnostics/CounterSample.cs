/*
 * CounterSample.cs - Implementation of the
 *			"System.Diagnostics.CounterSample" class.
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

namespace System.Diagnostics
{

#if CONFIG_EXTENDED_DIAGNOSTICS

public struct CounterSample
{
	// Internal state.
	private long baseValue;
	private long counterFrequency;
	private long counterTimeStamp;
	private PerformanceCounterType counterType;
	private long rawValue;
	private long systemFrequency;
	private long timeStamp;
	private long timeStamp100nSec;

	// The "empty" sample.
	public static readonly CounterSample Empty =
			new CounterSample(0, 0, 0, 0, 0, 0,
							  PerformanceCounterType.NumberOfItems32);

	// Constructors.
	public CounterSample(long rawValue, long baseValue,
						 long counterFrequency, long systemFrequency,
						 long timeStamp, long timeStamp100nSec,
						 PerformanceCounterType counterType)
			{
				this.rawValue = rawValue;
				this.baseValue = baseValue;
				this.counterFrequency = counterFrequency;
				this.systemFrequency = systemFrequency;
				this.timeStamp = timeStamp;
				this.timeStamp100nSec = timeStamp100nSec;
				this.counterType = counterType;
				this.counterTimeStamp = 0;
			}
	public CounterSample(long rawValue, long baseValue,
						 long counterFrequency, long systemFrequency,
						 long timeStamp, long timeStamp100nSec,
						 PerformanceCounterType counterType,
						 long counterTimeStamp)
			{
				this.rawValue = rawValue;
				this.baseValue = baseValue;
				this.counterFrequency = counterFrequency;
				this.systemFrequency = systemFrequency;
				this.timeStamp = timeStamp;
				this.timeStamp100nSec = timeStamp100nSec;
				this.counterType = counterType;
				this.counterTimeStamp = counterTimeStamp;
			}

	// Access the members of this structure.
	public long BaseValue
			{
				get
				{
					return baseValue;
				}
			}
	public long CounterFrequency
			{
				get
				{
					return counterFrequency;
				}
			}
	public long CounterTimeStamp
			{
				get
				{
					return counterTimeStamp;
				}
			}
	public PerformanceCounterType CounterType
			{
				get
				{
					return counterType;
				}
			}
	public long RawValue
			{
				get
				{
					return rawValue;
				}
			}
	public long SystemFrequency
			{
				get
				{
					return systemFrequency;
				}
			}
	public long TimeStamp
			{
				get
				{
					return timeStamp;
				}
			}
	public long TimeStamp100nSec
			{
				get
				{
					return timeStamp100nSec;
				}
			}

	// Calculate performance values.
	public static float Calculate(CounterSample counterSample)
			{
				return CounterSampleCalculator.ComputeCounterValue
					(counterSample);
			}
	public static float Calculate(CounterSample counterSample,
								  CounterSample nextCounterSample)
			{
				return CounterSampleCalculator.ComputeCounterValue
					(counterSample, nextCounterSample);
			}

}; // struct CounterSample

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
