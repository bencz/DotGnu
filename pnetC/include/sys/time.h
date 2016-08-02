/*
 * sys/time.h - Time types.
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

#ifndef _SYS_TIME_H
#define _SYS_TIME_H

#include <time.h>
#include <sys/select.h>

__BEGIN_DECLS

/*
 * Time values.
 */
struct itimerval
  {
    struct timeval it_interval;
    struct timeval it_value;
  };

/*
 * Values for "getitimer" and "setitimer".
 */
enum __itimer_which
  {
    ITIMER_REAL    = 0,
    ITIMER_VIRTUAL = 1,
    ITIMER_PROF    = 2
  };
#define ITIMER_REAL    ITIMER_REAL
#define ITIMER_VIRTUAL ITIMER_VIRTUAL
#define ITIMER_PROF    ITIMER_PROF

/*
 * Timezone information (obsolete and not used in this implementation).
 */
struct timezone
  {
    int tz_minuteswest;
    int tz_dsttime;
  };

/*
 * Function prototypes.
 */
extern int adjtime (const struct timeval *__delta, struct timeval *__olddelta);
extern int getitimer(int __which, struct itimerval *__value);
extern int gettimeofday(struct timeval * __restrict __tv,
                        void * __restrict __tz);
extern int setitimer(int __which, const struct itimerval * __restrict __new,
                     struct itimerval * __restrict __old);
extern int settimeofday(const struct timeval * __restrict __tv,
                        const void * __restrict __tz);
extern int utimes(const char *__file, const struct timeval __tvp[2]);

__END_DECLS

#endif  /* _SYS_TIME_H */
