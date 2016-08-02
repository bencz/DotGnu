/*
 * time.h - Time manipulation functions.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#ifndef _TIME_H
#define _TIME_H

#include <features.h>
#include <sys/types.h>
#include <stddef.h>

__BEGIN_DECLS

/*
 * Time values.
 */
struct timeval
  {
    time_t      tv_sec;
    suseconds_t tv_usec;
  };

/*
 * Time specifications.
 */
struct timespec
  {
    time_t tv_sec;
    long   tv_nsec;
  };
struct itimerspec
  {
    struct timespec it_interval;
    struct timespec it_value;
  };

/*
 * Split out time information.
 */
struct tm
  {
    int tm_sec;
    int tm_min;
    int tm_hour;
    int tm_mday;
    int tm_mon;
    int tm_year;
    int tm_wday;
    int tm_yday;
    int tm_isdst;
  };

/*
 * Useful constants.
 */
#define CLOCKS_PER_SEC			1000000L
#define CLOCK_REALTIME			0
#define CLOCK_PROCESS_CPUTIME_ID	2
#define CLOCK_THREAD_CPUTIME_ID		3
#define	CLOCK_MONOTONIC			4
#define TIMER_ABSTIME			1

/*
 * Functions.
 */
extern char *asctime (const struct tm *__tp);
extern char *asctime_r (const struct tm *__tp, char * __restrict __buf);
extern clock_t clock (void);
extern int clock_getcpuclockid (pid_t __pid, clockid_t *__clock_id);
extern int clock_getres (clockid_t __clock_id, struct timespec *__res);
extern int clock_gettime (clockid_t __clock_id, struct timespec *__tp);
extern int clock_nanosleep (clockid_t __clock_id, int __flags,
                            const struct timespec *__req,
			    struct timespec *__rem);
extern int clock_settime (clockid_t __clock_id, const struct timespec *__tp);
extern char *ctime (const time_t *__timer);
extern char *ctime_r (const time_t * __restrict __timer,
                      char * __restrict __buf);
extern double difftime (time_t __time1, time_t __time2);
extern struct tm *getdate (const char *__string);
extern int getdate_r (const char * __restrict __string,
                      struct tm * __restrict __resbufp);
extern struct tm *gmtime (const time_t *__timer);
extern struct tm *gmtime_r (const time_t * __restrict __timer,
                            struct tm * __restrict __tp);
extern struct tm *localtime (const time_t *__timer);
extern struct tm *localtime_r (const time_t * __restrict __timer,
                               struct tm * __restrict __tp);
extern time_t mktime (struct tm *__tp);
extern int nanosleep (const struct timespec *__requested_time,
                      struct timespec *__remaining);
extern size_t strftime (char * __restrict __s, size_t __maxsize,
                        const char * __restrict __format,
                        const struct tm * __restrict __tp);
extern char *strptime (const char * __restrict __s,
                       const char * __restrict __fmt,
                       struct tm * __restrict __tp);
extern time_t time (time_t *__timer);
#if 0
extern int timer_create (clockid_t __clock_id,
                         struct sigevent * __restrict __evp,
			 timer_t * __restrict __timerid);
extern int timer_delete (timer_t __timerid);
extern int timer_gettime (timer_t __timerid, struct itimerspec *__value);
extern int timer_getoverrun (timer_t __timerid);
extern int timer_settime (timer_t __timerid, int __flags,
                          const struct itimerspec * __restrict __value,
			  struct itimerspec * __restrict __ovalue);
#endif
extern void tzset (void);

/*
 * Variables.
 */
extern int daylight;
extern long timezone;
extern char *tzname[2];

__END_DECLS

#include <time.h>

#endif  /* !_TIME_H */
