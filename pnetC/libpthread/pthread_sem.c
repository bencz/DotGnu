/*
 * pthread_sem.c - Semaphore handling for pthreads.
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

#include <semaphore.h>
#include <pthread.h>
#include <pthread-support.h>
#include <errno.h>
#include <stdarg.h>
#include <string.h>
#include <stdlib.h>
#include <fcntl.h>

/*
 * Convert absolute times into tick timeouts.
 */
extern long long __pt_convert_time (const struct timespec *abstime);

/*
 * Named semaphore pool.  Named semaphores are local to the process
 * and never shared with other processes.  This is for security reasons.
 */
typedef struct sem_named_t
  {
    char *name;
    sem_t sem;
    int refcount;
    struct sem_named_t *next;
  } sem_named_t;
static pthread_mutex_t sem_lock = PTHREAD_MUTEX_INITIALIZER;
static sem_named_t *semaphores = 0;

int
sem_init (sem_t *sem, int pshared, unsigned value)
{
  if (value > SEM_VALUE_MAX)
    {
      errno = EINVAL;
      return -1;
    }
  __libc_monitor_init (&(sem->__sem_monitor));
  sem->__sem_value = (int)value;
  return 0;
}

int
sem_destroy (sem_t *sem)
{
  __libc_monitor_destroy (&(sem->__sem_monitor));
  return 0;
}

int
sem_getvalue (sem_t *__restrict sem, int *__restrict sval)
{
  if (!sem)
    {
      errno = EINVAL;
      return -1;
    }
  __libc_monitor_lock (&(sem->__sem_monitor));
  *sval = sem->__sem_value;
  __libc_monitor_unlock (&(sem->__sem_monitor));
  return 0;
}

int
sem_post (sem_t *sem)
{
  if (!sem)
    {
      errno = EINVAL;
      return -1;
    }
  __libc_monitor_lock (&(sem->__sem_monitor));
  if ((sem->__sem_value)++ == 0)
    __libc_monitor_pulse (&(sem->__sem_monitor));
  __libc_monitor_unlock (&(sem->__sem_monitor));
  return 0;
}

int
sem_timedwait (sem_t *__restrict sem,
               const struct timespec *__restrict abstime)
{
  long long timeout = __pt_convert_time (abstime);
  int result = 0;
  if (!sem)
    {
      errno = EINVAL;
      return -1;
    }
  __libc_monitor_lock (&(sem->__sem_monitor));
  if (sem->__sem_value == 0)
    {
      if (__libc_monitor_wait (&(sem->__sem_monitor), timeout))
        --(sem->__sem_value);
      else
        result = -1;
    }
  else
    {
      --(sem->__sem_value);
    }
  __libc_monitor_unlock (&(sem->__sem_monitor));
  if (result == -1)
    {
      errno = ETIMEDOUT;
      return -1;
    }
  return result;
}

int
sem_trywait (sem_t *sem)
{
  int result;
  if (!sem)
    {
      errno = EINVAL;
      return -1;
    }
  __libc_monitor_lock (&(sem->__sem_monitor));
  if (sem->__sem_value == 0)
    {
      result = -1;
    }
  else
    {
      --(sem->__sem_value);
      result = 0;
    }
  __libc_monitor_unlock (&(sem->__sem_monitor));
  if (result == -1)
    {
      errno = EAGAIN;
    }
  return result;
}

int
sem_wait (sem_t *sem)
{
  if (!sem)
    {
      errno = EINVAL;
      return -1;
    }
  __libc_monitor_lock (&(sem->__sem_monitor));
  if (sem->__sem_value == 0)
    {
      __libc_monitor_wait (&(sem->__sem_monitor), -1);
    }
  --(sem->__sem_value);
  __libc_monitor_unlock (&(sem->__sem_monitor));
  return 0;
}

sem_t *
sem_open (const char *name, int oflag, ...)
{
  sem_named_t *named;
  mode_t mode = 0;
  unsigned value = 0;
  if (!name)
    {
      errno = EINVAL;
      return SEM_FAILED;
    }
  if ((oflag & O_CREAT) != 0)
    {
      va_list va;
      va_start (va, oflag);
      mode = va_arg (va, mode_t);
      value = va_arg (va, unsigned);
      va_end (va);
      if (value > SEM_VALUE_MAX)
        {
	  errno = EINVAL;
	  return SEM_FAILED;
	}
    }
  pthread_mutex_lock (&sem_lock);
  named = semaphores;
  while (named != 0)
    {
      if (named->name && !strcmp (named->name, name))
        break;
      named = named->next;
    }
  if (named)
    {
      /* The name already exists: bail out if O_EXCL is set */
      if ((oflag & O_EXCL) != 0)
        {
          pthread_mutex_unlock (&sem_lock);
	  errno = EEXIST;
	  return SEM_FAILED;
	}
      ++(named->refcount);
    }
  else if ((oflag & O_CREAT) == 0)
    {
      /* The name does not exist and we should not create it */
      pthread_mutex_unlock (&sem_lock);
      errno = ENOENT;
      return SEM_FAILED;
    }
  else
    {
      /* Create a new semaphore called "name" */
      named = (sem_named_t *)malloc (sizeof (sem_named_t));
      if (!named)
        {
          pthread_mutex_unlock (&sem_lock);
	  errno = ENOMEM;
	  return SEM_FAILED;
	}
      named->name = strdup (name);
      if (!(named->name))
        {
	  free (named);
          pthread_mutex_unlock (&sem_lock);
	  errno = ENOMEM;
	  return SEM_FAILED;
	}
      sem_init (&(named->sem), 0, value);
      named->refcount = 1;
      named->next = semaphores;
      semaphores = named;
    }
  pthread_mutex_unlock (&sem_lock);
  return &(named->sem);
}

int
sem_close (sem_t *sem)
{
  sem_named_t *prev;
  sem_named_t *named;
  if (!sem)
    {
      errno = EINVAL;
      return -1;
    }
  pthread_mutex_lock (&sem_lock);
  prev = 0;
  named = semaphores;
  while (named != 0)
    {
      if (&(named->sem) == sem)
	break;
      prev = named;
      named = named->next;
    }
  if (!named || named->refcount == 0)
    {
      pthread_mutex_unlock (&sem_lock);
      errno = EINVAL;
      return -1;
    }
  --(named->refcount);
  if (named->refcount == 0 && !(named->name))
    {
      /* Last reference to an unlink'ed semaphore */
      if (prev)
        prev->next = named->next;
      else
        semaphores = named->next;
      free (named);
    }
  pthread_mutex_unlock (&sem_lock);
  return 0;
}

int
sem_unlink (const char *name)
{
  sem_named_t *prev;
  sem_named_t *named;
  if (!name)
    {
      errno = ENOENT;
      return -1;
    }
  pthread_mutex_lock (&sem_lock);
  prev = 0;
  named = semaphores;
  while (named != 0)
    {
      if (named->name && !strcmp (named->name, name))
	break;
      prev = named;
      named = named->next;
    }
  if (!named)
    {
      pthread_mutex_unlock (&sem_lock);
      errno = ENOENT;
      return -1;
    }
  if (named->refcount == 0)
    {
      /* Unlinked and there are no references left to be closed */
      if (prev)
        prev->next = named->next;
      else
        semaphores = named->next;
      free (named->name);
      free (named);
    }
  else
    {
      /* Free the name, but leave the semaphore active for now */
      free (named->name);
      named->name = 0;
    }
  pthread_mutex_unlock (&sem_lock);
  return 0;
}
