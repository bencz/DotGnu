/*
 * pthread_kill.c - Signal handling for pthreads.
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

int
pthread_kill (pthread_t thread, int sig)
{
  return __pthread_kill (thread, sig);
}

int
raise (int sig)
{
  return __pthread_kill (pthread_self (), sig);
}

int
pthread_sigmask (int how, const sigset_t * __restrict set,
                 sigset_t * __restrict oset)
{
  return __pthread_sigmask (how, set, oset);
}

extern int __sigwait (__const sigset_t *__restrict __set,
                      int *__restrict __sig);

int
sigwait (const sigset_t *__restrict set, int *__restrict sig)
{
  /* Overrides the "sigwait" in "libc" to provide cancellation semantics */
  int result;
  pthread_testcancel ();
  result = __sigwait (set, sig);
  pthread_testcancel ();
  return result;
}

weak_alias (raise, gsignal)
