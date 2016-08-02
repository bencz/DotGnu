/*
 * pthread_once.c - Once handling for pthreads.
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

/*
 * Import a spinlock function from "pthread_glue.cs" which
 * does not side-effect "errno" if it fails to acquire a lock.
 */
extern int __pt_spin_trylock (pthread_spinlock_t *lock);

int
pthread_once (pthread_once_t *once_control, void (*init_routine) (void))
{
  if (__pt_spin_trylock (once_control) == 0)
    (*init_routine) ();
  return 0;
}
