/*
 * pthread-support.h - Support routines in "libc" for Pthreads.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifndef _PTHREAD_SUPPORT_H
#define _PTHREAD_SUPPORT_H	1

#include <features.h>
#include <signal.h>

__BEGIN_DECLS

typedef long long __pthread_t;
typedef void *__libc_monitor_t;
__using__ System::Threading::Thread;
__using__ System::Object;

/*
 * Low-level counterparts to pthread functions.
 */
extern __pthread_t __pthread_self (void);
extern int __pthread_kill (__pthread_t thread, int sig);
extern int __pthread_sigmask (int how, const sigset_t * __restrict set,
                              sigset_t * __restrict oset);
extern __pthread_t __libc_thread_register
          (Thread thread, Object state);
extern __pthread_t __libc_thread_register_foreign (Thread thread);
extern void __libc_thread_unregister (__pthread_t id);
extern void __libc_thread_set_self (__pthread_t id);
extern Thread __libc_thread_object (__pthread_t id);
extern Object __libc_thread_state (__pthread_t id);

/*
 * Access to the C# monitor routines via "libc".  These versions
 * ensure that the monitor can be stored anywhere in main memory,
 * even in non-GC'ed memory.
 */
#define	__LIBC_MONITOR_INITIALIZER	((__libc_monitor_t *)0)
extern void __libc_monitor_init (__libc_monitor_t *monitor);
extern void __libc_monitor_destroy (__libc_monitor_t *monitor);
extern void __libc_monitor_lock (__libc_monitor_t *monitor);
extern int __libc_monitor_trylock (__libc_monitor_t *monitor,
                                   long long timeout);
extern void __libc_monitor_unlock (__libc_monitor_t *monitor);
extern int __libc_monitor_wait (__libc_monitor_t *monitor, long long timeout);
extern void __libc_monitor_pulse (__libc_monitor_t *monitor);
extern void __libc_monitor_pulseall (__libc_monitor_t *monitor);

__END_DECLS

#endif  /* !_PTHREAD_SUPPORT_H */
