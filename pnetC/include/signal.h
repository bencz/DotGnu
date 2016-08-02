/*
 * signal.h - Signal handling.
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

#ifndef _SIGNAL_H
#define _SIGNAL_H

#include <time.h>

__BEGIN_DECLS

/*
 * Type of a signal handler.
 */
typedef void (*sighandler_t)(int __sig);
typedef sighandler_t __sighandler_t;

/*
 * Special signal functions.
 */
#define SIG_ERR      ((sighandler_t)(-1))
#define SIG_DFL      ((sighandler_t)(0))
#define SIG_IGN      ((sighandler_t)(1))
#define SIG_HOLD     ((sighandler_t)(2))

/*
 * Signals values.
 */
#define	SIGHUP		1
#define	SIGINT		2
#define	SIGQUIT		3
#define	SIGILL		4
#define	SIGTRAP		5
#define	SIGABRT		6
#define	SIGIOT		SIGABRT
#define	SIGBUS		7
#define	SIGFPE		8
#define	SIGKILL		9
#define	SIGUSR1		10
#define	SIGSEGV		11
#define	SIGUSR2		12
#define	SIGPIPE		13
#define	SIGALRM		14
#define	SIGTERM		15
#define	SIGSTKFLT	16
#define	SIGCHLD		17
#define	SIGCLD		SIGCHLD
#define	SIGCONT		18
#define	SIGSTOP		19
#define	SIGTSTP		20
#define	SIGTTIN		21
#define	SIGTTOU		22
#define	SIGURG		23
#define	SIGXCPU		24
#define	SIGXFSZ		25
#define	SIGVTALRM	26
#define	SIGPROF		27
#define	SIGWINCH	28
#define	SIGIO		29
#define	SIGPOLL		SIGIO
#define	SIGPWR		30
#define SIGSYS		31
#define SIGUNUSED	31
#define	_NSIG		32
#define	NSIG		_NSIG

/*
 * Signal set type.
 */
typedef unsigned int sigset_t;

/*
 * Signal information structure.
 */
struct siginfo
  {
    int si_signo;
    int si_errno;
    int si_code;
  };
typedef struct siginfo siginfo_t;

/*
 * Signal action control structure.
 */
struct sigaction
  {
    sighandler_t sa_handler;
    void (*sa_sigaction) (int, siginfo_t *, void *);
    sigset_t sa_mask;
    int sa_flags;
    void (*sa_restorer) (void);
  };

/*
 * Value parameter for "sigqueue".
 */
union sigval
  {
    int sival_int;
    void *sival_ptr;
  };
typedef union sigval sigval_t;

/*
 * Flags within "struct sigaction".
 */
#define	SA_NOCLDSTOP  1
#define SA_NOCLDWAIT  2
#define SA_SIGINFO    4
#define SA_ONSTACK    0x08000000
#define SA_RESTART    0x10000000
#define SA_NODEFER    0x40000000
#define SA_RESETHAND  0x80000000
#define SA_INTERRUPT  0x20000000
#define SA_NOMASK     SA_NODEFER
#define SA_ONESHOT    SA_RESETHAND
#define SA_STACK      SA_ONSTACK

/*
 * Values for the "how" argument to "sigprocmask".
 */
#define	SIG_BLOCK     0
#define	SIG_UNBLOCK   1
#define	SIG_SETMASK   2

/*
 * List of signal messages.
 */
extern __const char *__const _sys_siglist[_NSIG];
#define	sys_siglist _sys_siglist

/*
 * Function prototypes.
 */
extern sighandler_t bsd_signal (int __sig, sighandler_t __handler);
extern int kill (pid_t __pid, int __sig);
extern int killpg (pid_t __pgrp, int __sig);
extern void psignal (int __sig, __const char *__s);
extern int pthread_kill (long long __thread, int __sig);
extern int raise (int __sig);
extern int sigaddset (sigset_t *__set, int __signo);
extern int sigandset (sigset_t *__set, const sigset_t *__left,
                      const sigset_t *__right);
extern int sigdelset (sigset_t *__set, int __signo);
extern int sigemptyset (sigset_t *__set);
extern int sigfillset (sigset_t *__set);
extern int sigisemptyset (const sigset_t *__set);
extern int sigismember (const sigset_t *__set, int __signo);
extern int sigorset (sigset_t *__set, const sigset_t *__left,
                     const sigset_t *__right);
extern sighandler_t signal (int __sig, sighandler_t __handler);
extern int sigprocmask (int __how, const sigset_t * __restrict __set,
                        sigset_t * __restrict __oset);
extern int pthread_sigmask (int __how, const sigset_t * __restrict __set,
                            sigset_t * __restrict __oset);
extern int sissuspend (sigset_t *__sigmask);
extern int sispending (sigset_t *__sigmask);
extern int sigaction (int __sig, const struct sigaction * __restrict __act,
                      struct sigaction * __restrict __oact);
extern int sigqueue (pid_t __pid, int __sig, const sigval_t __value);
extern int sighold (int __sig);
extern int sigignore (int __sig);
extern int sigpause (int __sig);
extern int sigrelse (int __sig);
extern sighandler_t sigset (int __sig, sighandler_t __handler);
extern int sigwait (__const sigset_t *__restrict __set, int *__restrict __sig);

__END_DECLS

#endif  /* !_SIGNAL_H */
