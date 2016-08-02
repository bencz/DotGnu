/*
 * signal.c - Install signal handlers.
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
#include <unistd.h>
#include <errno.h>

sighandler_t
__bsd_signal (int sig, sighandler_t handler)
{
  struct sigaction act, oact;

  /* Validate the parameters */
  if (handler == SIG_ERR || sig < 1 || sig >= NSIG)
    {
      errno = EINVAL;
      return SIG_ERR;
    }

  /* Install a signal handler using "sigaction" and BSD-style semantics */
  act.sa_handler = handler;
  act.sa_flags = SA_RESTART;
  sigemptyset (&act.sa_mask);
  sigaddset (&act.sa_mask, sig);
  act.sa_sigaction = 0;
  act.sa_restorer = 0;
  if (sigaction (sig, &act, &oact) == -1)
    return SIG_ERR;
  return oact.sa_handler;
}

weak_alias (__bsd_signal, bsd_signal)
weak_alias (__bsd_signal, signal)
weak_alias (__bsd_signal, ssignal)
weak_alias (__bsd_signal, sigset)
weak_alias (__bsd_signal, sysv_signal)
weak_alias (__bsd_signal, __sysv_signal)
