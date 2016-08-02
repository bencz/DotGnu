/*
 * pthread_barrier.c - Barrier handling for pthreads.
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

int
pthread_barrier_init (pthread_barrier_t *__restrict barrier,
                      const pthread_barrierattr_t *__restrict attr,
                      unsigned int count)
{
  if (count == 0)
    {
      errno = EINVAL;
      return -1;
    }
  if (pthread_mutex_init (&(barrier->__lock), 0) < 0)
    return -1;
  if (pthread_cond_init (&(barrier->__condition), 0) < 0)
    {
      pthread_mutex_destroy (&(barrier->__lock));
      return -1;
    }
  barrier->__desired_count = count;
  barrier->__current_count = 0;
  return 0;
}

int
pthread_barrier_destroy (pthread_barrier_t *barrier)
{
  pthread_cond_destroy (&(barrier->__condition));
  pthread_mutex_destroy (&(barrier->__lock));
  return 0;
}

int
pthread_barrier_wait (pthread_barrier_t *barrier)
{
  pthread_mutex_lock (&(barrier->__lock));
  ++(barrier->__current_count);
  if (barrier->__current_count >= barrier->__desired_count)
    {
      barrier->__current_count = 0;
      pthread_cond_broadcast (&(barrier->__condition));
      pthread_mutex_unlock (&(barrier->__lock));
      return 1;
    }
  else
    {
      pthread_cond_wait (&(barrier->__condition), &(barrier->__lock));
      pthread_mutex_unlock (&(barrier->__lock));
      return 0;
    }
}

int
pthread_barrierattr_init (pthread_barrierattr_t *attr)
{
  attr->__pshared = 0;
  return 0;
}

int
pthread_barrierattr_destroy (pthread_barrierattr_t *attr)
{
  /* Nothing to do here */
  return 0;
}

int
pthread_barrierattr_getpshared (const pthread_barrierattr_t * __restrict attr,
                                int *__restrict pshared)
{
  *pshared = attr->__pshared;
  return 0;
}

int
pthread_barrierattr_setpshared (pthread_barrierattr_t *attr, int pshared)
{
  attr->__pshared = pshared;
  return 0;
}
