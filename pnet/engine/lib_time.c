/*
 * lib_time.c - Internalcall methods for the "Platform.TimeMethods" classes.
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

#include "engine.h"
#include "lib_defs.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static long GetCurrentTime();
 *
 * This returns the time since 12:00 Jan 1, 0001 in tenths of a microsecond.
 */
ILInt64 _IL_TimeMethods_GetCurrentTime(ILExecThread *thread)
{
	ILCurrTime timeValue;
	ILGetCurrTime(&timeValue);
	timeValue.secs -= (ILInt64)(ILGetTimeZoneAdjust());
	return (timeValue.secs * (ILInt64)10000000) +
				(ILInt64)(timeValue.nsecs / (ILUInt32)100);
}

/*
 * public static long GetCurrentUtcTime();
 *
 * This returns UTC time since 12:00 Jan 1, 0001 in tenths of a microsecond.
 */
ILInt64 _IL_TimeMethods_GetCurrentUtcTime(ILExecThread *thread)
{
	ILCurrTime timeValue;
	ILGetCurrTime(&timeValue);
	return (timeValue.secs * (ILInt64)10000000) +
				(ILInt64)(timeValue.nsecs / (ILUInt32)100);
}

/*
 * public static int GetTimeZoneAdjust(long time);
 *
 * This returns the number of seconds West of GMT for the local timezone.
 */
ILInt32 _IL_TimeMethods_GetTimeZoneAdjust(ILExecThread *thread, ILInt64 time)
{
	/* TODO: change timezone value based on the time value */
	return ILGetTimeZoneAdjust();
}

/*
 * public static int GetUpTime();
 *
 * Note: this is supposed to return the number of milliseconds
 * since the system was rebooted.  Reboot times aren't usually
 * available on Unix-style systems, so we may have to use the
 * number of milliseconds since the engine was started instead.
 */
ILInt32 _IL_TimeMethods_GetUpTime(ILExecThread *thread)
{
	ILCurrTime timeValue;
	if(!ILGetSinceRebootTime(&timeValue))
	{
		ILGetCurrTime(&timeValue);
		
		if(timeValue.nsecs < thread->process->startTime.nsecs)
		{
			timeValue.nsecs =
				timeValue.nsecs - thread->process->startTime.nsecs 
				+ 1000000000;
			timeValue.secs =
				timeValue.secs - thread->process->startTime.secs;
		}
		else
		{
			timeValue.nsecs =
				timeValue.nsecs - thread->process->startTime.nsecs;			
			timeValue.secs =
				timeValue.secs - thread->process->startTime.secs;
		}
	}
	return (ILInt32)(((timeValue.secs * (ILInt64)1000) +
				(ILInt64)(timeValue.nsecs / (ILUInt32)1000000)) % IL_MAX_INT32);
}

/*
 * public static String GetDaylightName();
 */
ILString *_IL_TimeMethods_GetDaylightName(ILExecThread *_thread)
{
	/* TODO */
	return 0;
}

/*
 * public static String GetStandardName();
 */
extern ILString * _IL_TimeMethods_GetStandardName(ILExecThread *_thread)
{
	/* TODO */
	return 0;
}

/*
 * public static bool GetDaylightRules(int year, out long start,
 *									   out long end, out long delta);
 */
ILBool _IL_TimeMethods_GetDaylightRules(ILExecThread *_thread,
										ILInt32 year,
										ILInt64 *start,
										ILInt64 *end,
										ILInt64 *delta)
{
	/* TODO */
	return 0;
}

/*
 * private static bool GetPerformanceFrequency(out long frequency)
 */
ILBool _IL_Stopwatch_GetPerformanceFrequency(ILExecThread *_thread,
											   ILInt64 *frequency)
{
	return ILGetPerformanceCounterFrequency(frequency);
}

/*
 * private static long GetPerformanceCounter()
 */
ILInt64 _IL_Stopwatch_GetPerformanceCounter(ILExecThread *_thread)
{
	ILInt64 counter;

	if(ILGetPerformanceCounter(&counter))
	{
		return counter;
	}
	return _IL_TimeMethods_GetCurrentTime(_thread);
}

#ifdef	__cplusplus
};
#endif
