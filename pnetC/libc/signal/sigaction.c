/*
 * sigaction.c - Manage signal handlers.
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

/*
 * Table of signal actions for the current process.
 */
static struct sigaction sig_actions[NSIG];

/*
 * Perform the default action for a signal.
 */
static void
default_action (int sig)
{
  switch (sig)
    {
      case SIGHUP:
      case SIGINT:
      case SIGKILL:
      case SIGPIPE:
      case SIGALRM:
      case SIGTERM:
      case SIGUSR1:
      case SIGUSR2:
      case SIGPOLL:
      case SIGPROF:
      case SIGVTALRM:
      case SIGSTKFLT:
      case SIGPWR:
        /* Signals that terminate the process */
        _exit (1);
        break;

      case SIGQUIT:
      case SIGILL:
      case SIGABRT:
      case SIGFPE:
      case SIGSEGV:
      case SIGBUS:
      case SIGSYS:
      case SIGTRAP:
      case SIGXCPU:
      case SIGXFSZ:
        /* We can't dump core, but at least indicate something different */
        _exit (3);
        break;

      case SIGCHLD:
      case SIGCONT:
      case SIGSTOP:
      case SIGTSTP:
      case SIGTTIN:
      case SIGTTOU:
      case SIGURG:
      case SIGWINCH:
        /* Ignored signals */
        break;
    }
}

/*
 * Dispatch a signal within the current thread.
 */
void
__sigdispatch (int sig)
{
  struct sigaction action;
  sigset_t set;
  sighandler_t handler;
  siginfo_t info;

  /* Get the action to be invoked */
  action = sig_actions[sig];

  /* Reset the handler to SIG_DFL if SA_RESETHAND is set */
  if ((action.sa_flags & SA_RESETHAND) != 0)
    {
      sig_actions[sig].sa_handler = SIG_DFL;
      sig_actions[sig].sa_flags &= ~SA_SIGINFO;
    }

  /* Block the signal at the process level if necessary */
  if ((action.sa_flags & (SA_RESETHAND | SA_NODEFER)) == 0 ||
      sigismember (&(action.sa_mask), sig))
    {
      sigemptyset (&set);
      sigaddset (&set, sig);
      sigprocmask (SIG_BLOCK, &set, 0);
    }

  /* Dispatch the signal using the handler */
  if (sig == SIGKILL || sig == SIGSTOP)
    default_action (sig);
  else if ((sig_actions[sig].sa_flags & SA_SIGINFO) == 0)
    {
      handler = action.sa_handler;
      if (handler == SIG_DFL)
        default_action (sig);
      else if(handler != SIG_IGN && handler != SIG_ERR && handler != SIG_HOLD)
        {
          (*handler) (sig);
        }
    }
  else if (action.sa_sigaction != 0)
    {
      info.si_signo = sig;
      info.si_errno = 0;
      info.si_code = 0;
      (*(action.sa_sigaction)) (sig, &info, 0);
    }
  else
    {
      default_action (sig);
    }

  /* Unblock the signal if necessary */
  if ((action.sa_flags & (SA_RESETHAND | SA_NODEFER)) == 0 ||
      sigismember (&(action.sa_mask), sig))
    {
      sigemptyset (&set);
      sigaddset (&set, sig);
      sigprocmask (SIG_UNBLOCK, &set, 0);
    }
}

int
__libc_sigaction (int sig, const struct sigaction * __restrict act,
           		  struct sigaction * __restrict oact)
{
  /* Validate the signal identifier */
  if (sig < 1 || sig >= NSIG)
    {
      errno = EINVAL;
      return -1;
    }

  /* Fetch the original signal action */
  if (oact)
    *oact = sig_actions[sig];

  /* Install the new action handler and return */
  if (act)
    sig_actions[sig] = *act;
  return 0;
}

weak_alias (__libc_sigaction, sigaction)
weak_alias (__libc_sigaction, __sigaction)
