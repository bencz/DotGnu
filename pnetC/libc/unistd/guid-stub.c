/*
 * guid_stub.c - Stub out uid/gid functions, which are not secure.
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

gid_t
__getegid(void)
{
  return FAKE_GID;
}

uid_t
__geteuid(void)
{
  return FAKE_UID;
}

gid_t
__getgid(void)
{
  return FAKE_GID;
}

int
__getgroups(int size, gid_t list[])
{
  if (size == 0)
    {
      return 1;
    }
  else if (size >= 1)
    {
      if (list == NULL)
        {
          errno = EFAULT;
          return -1;
        }
      else
        {
          list[0] = FAKE_GID;
          return 1;
        }
    }
  else
    {
      errno = EINVAL;
      return -1;
    }
}

uid_t
__getuid(void)
{
  return FAKE_UID;
}

int
__setegid(gid_t gid)
{
  if (gid != FAKE_GID)
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      return 0;
    }
}

int
__seteuid(uid_t uid)
{
  if (uid != FAKE_UID)
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      return 0;
    }
}

int
__setgid(gid_t gid)
{
  if (gid != FAKE_GID)
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      return 0;
    }
}

int
__setregid(gid_t rgid, gid_t egid)
{
  if ((rgid != ((gid_t)(-1)) && rgid != FAKE_GID) ||
      (egid != ((gid_t)(-1)) && egid != FAKE_GID))
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      return 0;
    }
}

int
__setreuid(uid_t ruid, uid_t euid)
{
  if ((ruid != ((uid_t)(-1)) && ruid != FAKE_UID) ||
      (euid != ((uid_t)(-1)) && euid != FAKE_UID))
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      return 0;
    }
}

int
__setuid(uid_t uid)
{
  if (uid != FAKE_UID)
    {
      errno = EPERM;
      return -1;
    }
  else
    {
      return 0;
    }
}

weak_alias(__getegid, getegid)
weak_alias(__geteuid, geteuid)
weak_alias(__getgid, getgid)
weak_alias(__getgroups, getgroups)
weak_alias(__getuid, getuid)
weak_alias(__setegid, setegid)
weak_alias(__seteuid, seteuid)
weak_alias(__setgid, setgid)
weak_alias(__setregid, setregid)
weak_alias(__setreuid, setreuid)
weak_alias(__setuid, setuid)
