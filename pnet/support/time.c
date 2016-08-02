/*
 * time.c - Get the current system time.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004, 2008  Free Software Foundation
 *
 * Contributions from Thong Nguyen (tum@veridicus.com)
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

#include "il_system.h"
#include "il_thread.h"
#if defined(__palmos__)
	#include <PalmTypes.h>
	#include <TimeMgr.h>
#elif TIME_WITH_SYS_TIME
	#include <sys/time.h>
    #include <time.h>
#else
    #if HAVE_SYS_TIME_H
		#include <sys/time.h>
    #else
        #include <time.h>
    #endif
#endif
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifdef IL_WIN32_PLATFORM
#include <windows.h>
#define timezone _timezone
#endif

#if defined(HAVE_SYS_SYSINFO_H) && defined(HAVE_SYSINFO) \
	&& (defined(linux) \
	|| defined(__linux) || defined(__linux__))
	
	#include <sys/sysinfo.h>
#endif

#if defined(HAVE_SYS_SYSCTL_H) && defined(HAVE_SYSCTL) \
	&& defined(__FreeBSD__)
	
	#include <sys/sysctl.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Magic number that converts a time which is relative to
 * Jan 1, 1970 into a value which is relative to Jan 1, 0001.
 */
#define	EPOCH_ADJUST	((ILInt64)62135596800LL)

/*
 * Magic number that converts a time which is relative to
 * Jan 1, 1601 into a value which is relative to Jan 1, 0001.
 */
#define	WIN32_EPOCH_ADJUST	((ILInt64)50491123200LL)

/*
 * Magic number that converts a time which is relative to
 * Jan 1, 1904 into a value which is relative to Jan 1, 0001.
 */
#define	PALM_EPOCH_ADJUST	((ILInt64)60052752000LL)

/*
 * Get the current UTC time relative to Jan 01, 0001
 */
void ILGetCurrTime(ILCurrTime *timeValue)
{
#ifdef HAVE_GETTIMEOFDAY
	/* Try to get the current time, accurate to the microsecond */
	struct timeval tv;
	gettimeofday(&tv, 0);
	timeValue->secs = ((ILInt64)(tv.tv_sec)) + EPOCH_ADJUST;
	timeValue->nsecs = (ILUInt32)(tv.tv_usec * 1000);
#else
#ifdef IL_WIN32_PLATFORM
	/* Get the time using a Win32-specific API */
	FILETIME filetime;
	ILInt64 value;
	GetSystemTimeAsFileTime(&filetime);
	value = (((ILInt64)(filetime.dwHighDateTime)) << 32) +
			 ((ILInt64)(filetime.dwLowDateTime));
	timeValue->secs = (value / (ILInt64)10000000) + WIN32_EPOCH_ADJUST;
	timeValue->nsecs = (ILUInt32)((value % (ILInt64)10000000) * (ILInt64)100);
#elif defined(__palmos__)
	/* Use the PalmOS routine to get the time in seconds */
	timeValue->secs = ((ILInt64)(TimGetSeconds())) + PALM_EPOCH_ADJUST;
	timeValue->nsecs = 0;
#else
	/* Use the ANSI routine to get the time in seconds */
	timeValue->secs = (ILInt64)(time(0)) + EPOCH_ADJUST;
	timeValue->nsecs = 0;
#endif
#endif
}

int ILGetSinceRebootTime(ILCurrTime *timeValue)
{
#ifdef IL_WIN32_PLATFORM
	DWORD tick;
	
	tick = GetTickCount();

	timeValue->secs = tick / 1000;
	timeValue->nsecs = (tick % 1000) * 1000000;

	return 1;
#elif defined(__FreeBSD__)
	size_t len;
	int mib[2];
	struct timeval tv;

	mib[0] = CTL_KERN;
	mib[1] = KERN_BOOTTIME;

	len = sizeof(struct timeval);

	if (sysctl(mib, 2, &tv, &len, 0, 0) != 0)
	{
		return 0;
	}

	timeValue->secs = ((ILInt64)(tv.tv_sec));
	timeValue->nsecs = (ILUInt32)(tv.tv_usec * 1000);

	return 1;

#elif defined(HAVE_CLOCK_GETTIME) && defined(CLOCK_MONOTONIC)

	struct timespec tp;

	if(clock_gettime(CLOCK_MONOTONIC, &tp) != 0)
	{
		return 0;
	}

	timeValue->secs = tp.tv_sec;
	timeValue->nsecs = tp.tv_nsec;

	return 1;

#else
	return 0;
#endif
}

ILInt32 ILGetTimeZoneAdjust(void)
{
#if !defined(__palmos__)
	int isdst = 0;
	long timezone = 0;

#ifdef IL_WIN32_PLATFORM
	TIME_ZONE_INFORMATION temp;
	DWORD tmz = GetTimeZoneInformation(&temp);
	isdst = (tmz == TIME_ZONE_ID_DAYLIGHT) ? 1 : 0;
	/* we expect the adjustment to be in seconds, not minutes */
	if(isdst)
	{
		timezone = (temp.Bias + temp.DaylightBias) * 60;
	}
	else
	{
		timezone = temp.Bias * 60;
	}
#else
#ifdef HAVE_TM_GMTOFF
	/* Call "localtime", which will set the global "timezone" for us */
	time_t temp = time(0);
	tzset(); /* call this to get timezone changes */
	struct tm *tms = localtime(&temp);
	isdst = tms->tm_isdst;
	timezone = -(tms->tm_gmtoff);
#endif
#endif
	return (ILInt32)timezone;
#else
	/* TODO */
	return 0;
#endif
}

ILInt64 ILCLIToUnixTime(ILInt64 timeValue)
{
	return (timeValue / (ILInt64)10000000) - EPOCH_ADJUST;
}

ILInt64 ILUnixToCLITime(ILInt64 timeValue)
{
	return (timeValue + EPOCH_ADJUST) * (ILInt64)10000000;
}

/*
 * Get the current local time relative to Jan 01, 0001
 */
void ILGetLocalTime(ILCurrTime *timeValue)
{
	ILGetCurrTime(timeValue);

	timeValue->secs -= (ILInt64)(ILGetTimeZoneAdjust());
}

ILBool ILGetPerformanceCounterFrequency(ILInt64 *frequency)
{
	if(!frequency)
	{
		return (ILBool)0;
	}
	else
	{
#ifdef IL_WIN32_PLATFORM
		LARGE_INTEGER freq;

		if(QueryPerformanceFrequency(&freq))
		{
			*frequency = (ILInt64)(freq.QuadPart);
			return (ILBool)1;
		}
#elif defined(HAVE_CLOCK_GETTIME) && \
	  (defined(CLOCK_PROCESS_CPUTIME_ID) || defined(CLOCK_MONOTONIC))
		*frequency = 1000000000L;
		return (ILBool)1;
#endif
	}
	*frequency = 10000000L;
	return (ILBool)0;
}

ILBool ILGetPerformanceCounter(ILInt64 *counter)
{
	if(!counter)
	{
		return (ILBool)0;
	}
	else
	{
#ifdef IL_WIN32_PLATFORM
		LARGE_INTEGER ctr;

		if(QueryPerformanceCounter(&ctr))
		{
			*counter = (ILInt64)(ctr.QuadPart);
			return (ILBool)1;
		}
#elif defined(HAVE_CLOCK_GETTIME) && \
	  (defined(CLOCK_PROCESS_CPUTIME_ID) || defined(CLOCK_MONOTONIC))
		struct timespec tp;
		/*
		 * i'd prefer to use CLOCK_PROCESS_CPUTIME_ID in first place
		 * but the resolution of this timer is too low on some systems.
		 */
#if defined(CLOCK_MONOTONIC)
		if(clock_gettime(CLOCK_MONOTONIC, &tp) != 0)
		{
			return (ILBool)0;
		}
#else
		if(clock_gettime(CLOCK_PROCESS_CPUTIME_ID, &tp) != 0)
		{
			return (ILBool)0;
		}
#endif
		*counter = ((tp.tv_sec * 1000000000L) + (ILInt64)tp.tv_nsec);
		return (ILBool)1;
#endif
	}
	/* TODO: return the local time if we get here */
	return (ILBool)0;
}

#ifdef	__cplusplus
};
#endif
