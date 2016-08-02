/*
 * cpuclock.c - Support for real-time CPU clocks.
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

#include <time.h>
#include <errno.h>
#include "time-defs.h"
#include "../unistd/fake-ids.h"

int
clock_getcpuclockid (pid_t pid, clockid_t *clock_id)
{
  /* We can only get the clock ID for the current process */
  if (pid != FAKE_PID)
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      *clock_id = CLOCK_PROCESS_CPUTIME_ID;
      return 0;
    }
}

int
clock_getres (clockid_t clock_id, struct timespec *res)
{
  if (res)
    {
      /* We hard-write a resolution of 1 microsecond */
      res->tv_sec = 0;
      res->tv_nsec = 1000;
    }
  return 0;
}

int
clock_gettime (clockid_t clock_id, struct timespec *tp)
{
  if (tp)
    {
      /* We don't really have a realtime clock, so just return
         the current time of day to the best resolution possible */
      long long t = __syscall_utc_time ();
      tp->tv_sec = (time_t)((t / TICKS_PER_SEC) - EPOCH_ADJUST);
      tp->tv_nsec = (long)((t % TICKS_PER_SEC) * NSECS_PER_TICK);
    }
  return 0;
}

int
clock_nanosleep (clockid_t clock_id, int flags,
                 const struct timespec *req,
                 struct timespec *rem)
{
  return nanosleep (req, rem);
}

int
clock_settime (clockid_t clock_id, const struct timespec *tp)
{
  errno = EPERM;
  return -1;
}
