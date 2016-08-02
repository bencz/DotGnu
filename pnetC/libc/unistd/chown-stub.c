/*
 * chown-stub.c - Stub out "chown" functions, which are not secure.
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

int
__chown (const char *path, uid_t uid, gid_t gid)
{
  if(path == NULL)
    {
      errno = EINVAL;
      return -1;
    }
  else
    {
      errno = ENOSYS;
      return -1;
    }
}

int
__lchown (const char *path, uid_t uid, gid_t gid)
{
  if(path == NULL)
    {
      errno = EINVAL;
      return -1;
    }
  else
    {
      errno = ENOSYS;
      return -1;
    }
}

int
__fchown (int fd, uid_t uid, gid_t gid)
{
  if(fd < 0)
    {
      errno = EINVAL;
      return -1;
    }
  else
    {
      errno = ENOSYS;
      return -1;
    }
}

weak_alias(__chown, chown)
weak_alias(__lchown, lchown)
weak_alias(__fchown, fchown)
