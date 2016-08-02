/*
 * pid_stub.c - Stub out pid functions, which are not secure.
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

#include <unistd.h>
#include <errno.h>
#include "fake-ids.h"

/* See "fake-ids.h" for a description of why these values are fake */

pid_t
__getpgid(pid_t pid)
{
  if (pid == ((pid_t)0))
    {
      return FAKE_PID;
    }
  else
    {
      return pid;
    }
}

pid_t
__getpid(void)
{
  return FAKE_PID;
}

pid_t
__getppid(void)
{
  return FAKE_PPID;
}

pid_t
__getsid(pid_t pid)
{
  if (pid == ((pid_t)0))
    {
      return FAKE_PID;
    }
  else
    {
      return pid;
    }
}

int
__setpgid(pid_t pid, pid_t pgid)
{
  if ((pid == ((pid_t)0) || pid == FAKE_PID) &&
      (pgid == ((pid_t)0) || pgid == FAKE_PID))
    {
      return 0;
    }
  else
    {
      errno = EPERM;
      return -1;
    }
}

int
__setpgrp(void)
{
  return 0;
}

pid_t
__setsid(void)
{
  return FAKE_PID;
}

extern int __isatty(int fd);

pid_t
__getpgrp(void)
{
  return FAKE_PID;
}

pid_t
__tcgetpgrp(int fd)
{
  if (fd < 0)
    {
      errno = EBADF;
      return (pid_t)(-1);
    }
  else if(!__isatty(fd))
    {
      errno = EPERM;
      return (pid_t)(-1);
    }
  else
    {
      return FAKE_PID;
    }
}

int
__tcsetpgrp(int fd, pid_t pgrpid)
{
  if (fd < 0)
    {
      errno = EBADF;
      return (pid_t)(-1);
    }
  else if(!__isatty(fd) ||
          (pgrpid != ((pid_t)0) && pgrpid != FAKE_PID))
    {
      errno = EPERM;
      return (pid_t)(-1);
    }
  else
    {
      return 0;
    }
}

weak_alias(__getpgid, getpgid)
weak_alias(__getpid, getpid)
weak_alias(__getpgrp, getpgrp)
weak_alias(__getppid, getppid)
weak_alias(__getsid, getsid)
weak_alias(__setpgid, setpgid)
weak_alias(__setpgrp, setpgrp)
weak_alias(__setsid, setsid)
weak_alias(__tcgetpgrp, tcgetpgrp)
weak_alias(__tcsetpgrp, tcsetpgrp)
