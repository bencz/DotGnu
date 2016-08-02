/*
 * nanosleep.c - Sleep for a period of time.
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

int
nanosleep (const struct timespec *requested_time,
           struct timespec *remaining)
{
  if (requested_time->tv_nsec < 0 || requested_time->tv_nsec > 1000000000LL)
    {
      errno = EINVAL;
      return -1;
    }
  __syscall_sleep_ticks (requested_time->tv_sec * TICKS_PER_SEC +
  			 requested_time->tv_nsec / NSECS_PER_TICK);
  if (remaining)
    {
      remaining->tv_sec = 0;
      remaining->tv_nsec = 0;
    }
  return 0;
}
