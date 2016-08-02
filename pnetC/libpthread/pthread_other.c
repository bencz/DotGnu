/*
 * pthread_other.c - Other pthread functions.
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

#include <pthread.h>
#include <errno.h>

static int concurrency = 0;

int
pthread_getconcurrency (void)
{
  return concurrency;
}

int
pthread_setconcurrency (int new_level)
{
  if (new_level < 0)
    {
      errno = EINVAL;
      return -1;
    }
  concurrency = new_level;
  return 0;
}

int
pthread_getcpuclockid (pthread_t thread, clockid_t *clock_id)
{
  *clock_id = CLOCK_THREAD_CPUTIME_ID;
  return 0;
}

int
pthread_atfork (void (*prepare) (void), void (*parent) (void),
                void (*child) (void))
{
  /* Nothing to do here */
  return 0;
}

void
pthread_kill_other_threads_np (void)
{
  /* Nothing to do here */
}
