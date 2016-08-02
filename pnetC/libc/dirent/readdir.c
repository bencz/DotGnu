/*
 * readdir.c - Read a directory entry.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2003  Free Software Foundation, Inc.
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

#include <errno.h>
#include <stddef.h>
#include <string.h>
#include <dirent.h>
#include "dirent-glue.h"

struct dirent *
__readdir (DIR *dirp)
{
  char *str;
  int err;
  int len;

  if (dirp == NULL || dirp->gc_handle == NULL)
    {
      errno = EBADF;
      return NULL;
    }

  err = 0;
  str = (char *)__syscall_readdir (dirp->gc_handle, &err);

  if (err)
    {
      errno = err;
	  return NULL;
    }
  if (str == NULL)
    {
      return NULL;
    }

  len = sizeof(dirp->current.d_name) - 1;
  strncpy (dirp->current.d_name, str, len);
  dirp->current.d_name[len] = '\0';
  dirp->current.d_ino = 0;
  (void)Marshal::FreeHGlobal((long)str);

  return &(dirp->current);
}
weak_alias (__readdir, readdir)
