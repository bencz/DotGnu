/*
 * sigset.c - Manage signal mask sets.
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
#include <errno.h>

int
sigaddset (sigset_t *set, int signo)
{
  if (!set || signo < 1 || signo >= NSIG)
    {
      errno = EINVAL;
      return -1;
    }
  *set |= (1 << signo);
  return 0;
}

int
sigandset (sigset_t *set, const sigset_t *left, const sigset_t *right)
{
  if (!set || !left || !right)
    {
      errno = EINVAL;
      return -1;
    }
  *set = *left & *right;
  return 0;
}

int
sigdelset (sigset_t *set, int signo)
{
  if (!set || signo < 1 || signo >= NSIG)
    {
      errno = EINVAL;
      return -1;
    }
  *set &= ~(1 << signo);
  return 0;
}

int
sigemptyset (sigset_t *set)
{
  if (!set)
    {
      errno = EINVAL;
      return -1;
    }
  *set = 0;
  return 0;
}

int
sigfillset (sigset_t *set)
{
  if (!set)
    {
      errno = EINVAL;
      return -1;
    }
  *set = ~0;
  return 0;
}

int
sigisemptyset (const sigset_t *set)
{
  if (!set)
    {
      errno = EINVAL;
      return -1;
    }
  return (*set == 0);
}

int
sigismember (const sigset_t *set, int signo)
{
  if (!set || signo < 1 || signo >= NSIG)
    {
      errno = EINVAL;
      return -1;
    }
  if ((*set & (1 << signo)) != 0)
    return 1;
  else
    return 0;
}

int
sigorset (sigset_t *set, const sigset_t *left, const sigset_t *right)
{
  if (!set || !left || !right)
    {
      errno = EINVAL;
      return -1;
    }
  *set = *left | *right;
  return 0;
}
