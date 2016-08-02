/*
 * pthread_rwlock.c - Read-write lock handling for pthreads.
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

int
pthread_rwlock_init (pthread_rwlock_t *__restrict rwlock,
                     const pthread_rwlockattr_t * __restrict attr)
{
  if (attr)
    return pthread_mutex_init (&(rwlock->__lock), &(attr->__attr));
  else
    return pthread_mutex_init (&(rwlock->__lock), 0);
}

int
pthread_rwlock_destroy (pthread_rwlock_t *rwlock)
{
  return pthread_mutex_destroy (&(rwlock->__lock));
}

int
pthread_rwlock_rdlock (pthread_rwlock_t *rwlock)
{
  return pthread_mutex_lock (&(rwlock->__lock));
}

int
pthread_rwlock_tryrdlock (pthread_rwlock_t *rwlock)
{
  return pthread_mutex_trylock (&(rwlock->__lock));
}

int
pthread_rwlock_timedrdlock (pthread_rwlock_t *__restrict rwlock,
                            const struct timespec *__restrict abstime)
{
  return pthread_mutex_timedlock (&(rwlock->__lock), abstime);
}

int
pthread_rwlock_wrlock (pthread_rwlock_t *rwlock)
{
  return pthread_mutex_lock (&(rwlock->__lock));
}

int
pthread_rwlock_trywrlock (pthread_rwlock_t *rwlock)
{
  return pthread_mutex_trylock (&(rwlock->__lock));
}

int
pthread_rwlock_timedwrlock (pthread_rwlock_t *__restrict rwlock,
                            const struct timespec *__restrict abstime)
{
  return pthread_mutex_timedlock (&(rwlock->__lock), abstime);
}

int
pthread_rwlock_unlock (pthread_rwlock_t *rwlock)
{
  return pthread_mutex_unlock (&(rwlock->__lock));
}

int
pthread_rwlockattr_init (pthread_rwlockattr_t *attr)
{
  return pthread_mutexattr_init (&(attr->__attr));
}

int
pthread_rwlockattr_destroy (pthread_rwlockattr_t *attr)
{
  return pthread_mutexattr_destroy (&(attr->__attr));
}

int
pthread_rwlockattr_getpshared (const pthread_rwlockattr_t * __restrict attr,
			       int *__restrict pshared)
{
  return pthread_mutexattr_getpshared (&(attr->__attr), pshared);
}

int
pthread_rwlockattr_setpshared (pthread_rwlockattr_t *attr, int pshared)
{
  return pthread_mutexattr_setpshared (&(attr->__attr), pshared);
}

int
pthread_rwlockattr_getkind_np (const pthread_rwlockattr_t *attr, int *pref)
{
  return pthread_mutexattr_gettype (&(attr->__attr), pref);
}

int
pthread_rwlockattr_setkind_np (pthread_rwlockattr_t *attr, int pref)
{
  return pthread_mutexattr_settype (&(attr->__attr), pref);
}
