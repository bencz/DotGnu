/*
 * pthread_mutex.c - Mutex handling for pthreads.
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

#include <signal.h>
#include <pthread.h>
#include <pthread-support.h>
#include <errno.h>
#include <time.h>

int
pthread_mutex_init (pthread_mutex_t *__restrict mutex,
                    const pthread_mutexattr_t *__restrict mutex_attr)
{
  /* We don't use mutex attributes - all mutexes are recursive
     because it isn't possible to create a non-recursive monitor */
  __libc_monitor_init (&(mutex->__m_monitor));
  return 0;
}

int
pthread_mutex_destroy (pthread_mutex_t *mutex)
{
  __libc_monitor_destroy (&(mutex->__m_monitor));
  return 0;
}

int
pthread_mutex_trylock (pthread_mutex_t *mutex)
{
  if (__libc_monitor_trylock (&(mutex->__m_monitor), -1))
    return 0;
  errno = EBUSY;
  return -1;
}

int
pthread_mutex_lock (pthread_mutex_t *mutex)
{
  __libc_monitor_lock (&(mutex->__m_monitor));
  return 0;
}

long long
__pt_convert_time (const struct timespec *abstime)
{
  long long timeout;
  if (!abstime)
    timeout = 0;
  else
    {
      struct timespec current;
      clock_gettime (CLOCK_REALTIME, &current);
      if (current.tv_sec > abstime->tv_sec ||
          (current.tv_sec == abstime->tv_sec &&
	   current.tv_nsec >= abstime->tv_nsec))
        {
	  timeout = 0;
	}
      else if (current.tv_sec == abstime->tv_sec)
        {
	  timeout = (abstime->tv_nsec - current.tv_nsec) / 100LL;
	}
      else
        {
	  timeout = (1000000000LL - current.tv_nsec) / 100LL;
	  timeout += abstime->tv_nsec / 100LL;
	  timeout += (abstime->tv_sec - current.tv_sec - 1) * 10000000LL;
	}
    }
  return timeout;
}

int
pthread_mutex_timedlock (pthread_mutex_t *__restrict mutex,
			 const struct timespec *__restrict abstime)
{
  long long timeout = __pt_convert_time (abstime);
  if (__libc_monitor_trylock (&(mutex->__m_monitor), timeout))
    return 0;
  errno = ETIMEDOUT;
  return -1;
}

int
pthread_mutex_unlock (pthread_mutex_t *mutex)
{
  __libc_monitor_unlock (&(mutex->__m_monitor));
  return 0;
}


int
pthread_mutexattr_init (pthread_mutexattr_t *attr)
{
  attr->__mutex_kind = 0;
  attr->__mutex_pshared = 0;
  return 0;
}

int
pthread_mutexattr_destroy (pthread_mutexattr_t *attr)
{
  /* Nothing to do here */
  return 0;
}

int
pthread_mutexattr_getpshared (const pthread_mutexattr_t * __restrict attr,
                              int *__restrict pshared)
{
  *pshared = attr->__mutex_pshared;
  return 0;
}

int
pthread_mutexattr_setpshared (pthread_mutexattr_t *attr, int pshared)
{
  attr->__mutex_pshared = pshared;
  return 0;
}

int
pthread_mutexattr_settype (pthread_mutexattr_t *attr, int kind)
{
  attr->__mutex_kind = kind;
  return 0;
}

int
pthread_mutexattr_gettype (const pthread_mutexattr_t *__restrict attr,
                           int *__restrict kind)
{
  *kind = attr->__mutex_kind;
  return 0;
}
