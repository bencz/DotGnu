/*
 * semaphore.h - Semaphore routines.
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

#ifndef _SEMAPHORE_H
#define _SEMAPHORE_H	1

#include <features.h>
#include <sys/types.h>
#include <time.h>

__BEGIN_DECLS

/*
 * Structure of a semaphore.
 */
typedef struct __sem_s
  {
    void *__sem_monitor;
    int __sem_value;
  } sem_t;

/*
 * Value returned if "sem_open" failed.
 */
#define SEM_FAILED          ((sem_t *)0)

/*
 * Maximum value the semaphore can have.
 */
#define SEM_VALUE_MAX       ((int)(((unsigned int)(~0)) >> 1))

/*
 * Semaphore functions.
 */
extern int sem_close (sem_t *__sem);
extern int sem_destroy (sem_t *__sem);
extern int sem_getvalue (sem_t *__restrict __sem, int *__restrict __sval);
extern int sem_init (sem_t *__sem, int __pshared, unsigned __value);
extern sem_t *sem_open (__const char *__name, int __oflag, ...);
extern int sem_post (sem_t *__sem);
extern int sem_timedwait (sem_t *__restrict __sem,
                          __const struct timespec *__restrict __abstime);
extern int sem_trywait (sem_t *__sem);
extern int sem_unlink (__const char *__name);
extern int sem_wait (sem_t *__sem);

__END_DECLS

#endif  /* !_SEMAPHORE_H */
