/*
 * pthread_cond.c - Condition variable handling for pthreads.
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
#include <pthread-support.h>
#include <errno.h>

extern long long __pt_convert_time (const struct timespec *abstime);

int
pthread_cond_init (pthread_cond_t *__restrict cond,
                   const pthread_condattr_t *__restrict cond_attr)
{
  cond->__lock = 0;
  cond->__wait_mutex = 0;
  return 0;
}

int
pthread_cond_destroy (pthread_cond_t *cond)
{
  /* Nothing to do here */
  return 0;
}

int
pthread_cond_signal (pthread_cond_t *cond)
{
  pthread_mutex_t *mutex;
  pthread_spin_lock (&(cond->__lock));
  mutex = cond->__wait_mutex;
  pthread_spin_unlock (&(cond->__lock));
  if (mutex)
    {
      __libc_monitor_pulse (&(mutex->__m_monitor));
    }
  return 0;
}

int
pthread_cond_broadcast (pthread_cond_t *cond)
{
  pthread_mutex_t *mutex;
  pthread_spin_lock (&(cond->__lock));
  mutex = cond->__wait_mutex;
  pthread_spin_unlock (&(cond->__lock));
  if (mutex)
    {
      __libc_monitor_pulseall (&(mutex->__m_monitor));
    }
  return 0;
}

int
pthread_cond_wait (pthread_cond_t *__restrict cond,
                   pthread_mutex_t *__restrict mutex)
{
  /* See if the current thread should be cancelled */
  pthread_testcancel();

  /* Record the mutex we are waiting on in the condition variable */
  pthread_spin_lock (&(cond->__lock));
  cond->__wait_mutex = mutex;
  pthread_spin_unlock (&(cond->__lock));

  /* Wait on the mutex's monitor */
  __libc_monitor_wait (&(mutex->__m_monitor), -1);

  /* Check for cancellation again */
  pthread_testcancel();
  return 0;
}

int
pthread_cond_timedwait (pthread_cond_t *__restrict cond,
                        pthread_mutex_t *__restrict mutex,
                        const struct timespec *__restrict abstime)
{
  long long timeout;

  /* See if the current thread should be cancelled */
  pthread_testcancel();

  /* Record the mutex we are waiting on in the condition variable */
  pthread_spin_lock (&(cond->__lock));
  cond->__wait_mutex = mutex;
  pthread_spin_unlock (&(cond->__lock));

  /* Wait until the timeout expires */
  timeout = __pt_convert_time (abstime);
  if (__libc_monitor_wait (&(mutex->__m_monitor), timeout))
    {
      pthread_testcancel();
      return 0;
    }
  pthread_testcancel();
  errno = ETIMEDOUT;
  return -1;
}

int
pthread_condattr_init (pthread_condattr_t *attr)
{
  attr->__pshared = 0;
  attr->__clock_id = 0;
  return 0;
}

int
pthread_condattr_destroy (pthread_condattr_t *attr)
{
  /* Nothing to do here */
  return 0;
}

int
pthread_condattr_getpshared (const pthread_condattr_t * __restrict attr,
                             int *__restrict pshared)
{
  *pshared = attr->__pshared;
  return 0;
}

int
pthread_condattr_setpshared (pthread_condattr_t *attr, int pshared)
{
  attr->__pshared = pshared;
  return 0;
}

int
pthread_condattr_getclock (const pthread_condattr_t * __restrict attr,
                           clockid_t *__restrict clock_id)
{
  *clock_id = attr->__clock_id;
  return 0;
}

int
pthread_condattr_setclock (pthread_condattr_t *attr, clockid_t clock_id)
{
  attr->__clock_id = clock_id;
  return 0;
}
