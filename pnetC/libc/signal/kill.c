/*
 * kill.c - Send signals to processes and handle process/thread masks.
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
#include <pthread-support.h>
#include <stdlib.h>

/*
 * Import the glue functions from "signal-glue.cs".
 */
extern void __syscall_siginit(long long thread);
extern void __syscall_sigdeliver(long long thread, int sig);
extern int  __syscall_sigsuspend(long long thread, unsigned int blocked);
extern int  __syscall_signext(long long thread, unsigned int blocked);
extern unsigned int __syscall_sigpending(long long thread);

/*
 * The signal masks for the overall process and the current thread.
 */
static sigset_t volatile process_mask;
static sigset_t __declspec(thread) thread_mask;

/*
 * Import the signal dispatcher from "sigaction.c".
 */
extern void __sigdispatch (int sig);

/*
 * Process pending signals for the current thread.
 */
static void
process_pending_signals (void)
{
  int sig;
  sigset_t set;
  for (;;)
    {
      sig = __syscall_signext (__pthread_self (), process_mask | thread_mask);
      if (sig == -1)
      	break;
      __sigdispatch (sig);
    }
}

int
__kill (pid_t pid, int sig)
{
  sighandler_t handler;

  /* Validate the signal number */
  if (sig < 1 || sig >= NSIG || sig == SIGKILL || sig == SIGCONT)
    {
      errno = EINVAL;
      return -1;
    }

  /* We can only send signals to ourselves, never to any other process */
  if (pid != 0 && pid != getpid() && pid != -(getpgrp()))
    {
      errno = EPERM;
      return -1;
    }

  /* Make sure that signal handling is enabled on the current thread */
  __syscall_siginit (__pthread_self ());

  /* Deliver the signal to any thread that hasn't got it blocked */
  __syscall_sigdeliver (-1, sig);

  /* Process pending signals for this thread */
  process_pending_signals ();
  return 0;
}

int
__killpg (pid_t pgrp, int sig)
{
  if (pgrp < 0)
    {
      errno = EINVAL;
      return -1;
    }
  return kill (-pgrp, sig);
}

int
__sigqueue (pid_t pid, int sig, const sigval_t value)
{
  return kill (pid, sig);
}

int
__pthread_kill (__pthread_t thread, int sig)
{
  /* Validate the signal number */
  if (sig < 0 || sig >= NSIG || sig == SIGKILL || sig == SIGSTOP)
    {
      errno = EINVAL;
      return -1;
    }

  /* Deliver the signal to the thread */
  __syscall_sigdeliver (thread, sig);

  /* Handle the signal if it was actually delivered to this thread */
  if (thread == __pthread_self ())
    process_pending_signals ();

  /* The signal has been delivered and/or handled */
  return 0;
}

int
__sigprocmask (int how, const sigset_t * __restrict set,
               sigset_t * __restrict oset)
{
  if (set)
    {
      if (how == SIG_BLOCK)
        process_mask |= *set;
      else if (how == SIG_UNBLOCK)
        process_mask &= ~*set;
      else if (how == SIG_SETMASK)
        process_mask = *set;
      else
       {
         errno = EINVAL;
         return -1;
       }
    }
  if (oset)
    *oset = process_mask;
  return 0;
}

int
__pthread_sigmask (int how, const sigset_t * __restrict set,
                   sigset_t * __restrict oset)
{
  if (set)
    {
      if (how == SIG_BLOCK)
        thread_mask |= *set;
      else if (how == SIG_UNBLOCK)
        thread_mask &= ~*set;
      else if (how == SIG_SETMASK)
        thread_mask = *set;
      else
       {
         errno = EINVAL;
         return -1;
       }
    }
  if (oset)
    *oset = thread_mask;
  return 0;
}

int
__sigsuspend (sigset_t *sigmask)
{
  int sig;
  sigset_t old;

  /* Block the signals in the mask and then wait for something to arrive */
  old = thread_mask;
  thread_mask = *sigmask;
  sig = __syscall_sigsuspend (__pthread_self (), process_mask | thread_mask);
  thread_mask = old;

  /* Dispatch the signal that we saw */
  __sigdispatch (sig);

  /* Indicate to the caller that we were interrupted by a signal */
  errno = EINTR;
  return -1;
}

int
sigpending (sigset_t *sigmask)
{
  /* Process signals that aren't blocked but which were delivered to us */
  process_pending_signals ();

  /* Get the set of pending signals that are still blocked */
  *sigmask = __syscall_sigpending (__pthread_self ())
  			& (process_mask | thread_mask);
  return 0;
}

int
__sigwait (const sigset_t *__restrict set, int *__restrict sig)
{
  sigset_t old;

  /* Validate the signal set */
  if (!set)
    {
      errno = EINVAL;
      return -1;
    }

  /* Block the signals in the mask and then wait for something to arrive */
  old = thread_mask;
  thread_mask = *set;
  *sig = __syscall_sigsuspend (__pthread_self (), process_mask | thread_mask);
  thread_mask = old;

  /* Dispatch the signal that we saw */
  __sigdispatch (*sig);
  return 0;
}

weak_alias (__kill, kill)
weak_alias (__killpg, killpg)
weak_alias (__sigqueue, sigqueue)
weak_alias (__sigprocmask, sigprocmask)
weak_alias (__sigsuspend, sigsuspend)
weak_alias (__sigwait, sigwait)
