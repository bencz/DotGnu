/*
 * PerformanceCounterType.cs - Implementation of the
 *			"System.Diagnostics.PerformanceCounterType" class.
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

using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
[TypeConverter(typeof(AlphabeticalEnumConverter))]
#endif
public enum PerformanceCounterType
{

	NumberOfItemsHEX32				= 0x00000000,
	NumberOfItemsHEX64				= 0x00000100,
	NumberOfItems32					= 0x00010000,
	NumberOfItems64					= 0x00010100,
	CounterDelta32					= 0x00400400,
	CounterDelta64					= 0x00400500,
	SampleCounter					= 0x00410400,
	CountPerTimeInterval32			= 0x00450400,
	CountPerTimeInterval64			= 0x00450500,
	RateOfCountsPerSecond32			= 0x10410400,
	RateOfCountsPerSecond64			= 0x10410500,
	RawFraction						= 0x20020400,
	CounterTimer					= 0x20410500,
	Timer100Ns						= 0x20510500,
	SampleFraction					= 0x20C20400,
	CounterTimerInverse				= 0x21410500,
	Timer100NsInverse				= 0x21510500,
	CounterMultiTimer				= 0x22410500,
	CounterMultiTimerInverse		= 0x23410500,
	CounterMultiTimer100Ns			= 0x22510500,
	CounterMultiTimer100NsInverse	= 0x23510500,
	AverageTimer32					= 0x30020400,
	ElapsedTime						= 0x30240500,
	AverageCount64					= 0x40020500,
	SampleBase						= 0x40030401,
	AverageBase						= 0x40030402,
	RawBase							= 0x40030403,
	CounterMultiBase				= 0x42030500

}; // enum PerformanceCounterType

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
