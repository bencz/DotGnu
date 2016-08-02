/*
 * readdir_r.c - Read a directory entry into the given entry storage.
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

int
__readdir_r (DIR *dirp, struct dirent *entry, struct dirent **result)
{
  char *str;
  int err;
  int len;

  if (dirp == NULL || dirp->gc_handle == NULL)
    {
      errno = EBADF;
      return EBADF;
    }

  err = 0;
  str = (char *)__syscall_readdir (dirp->gc_handle, &err);

  if (err)
    {
      errno = err;
	  *result = NULL;
	  return NULL;
    }
  if (str == NULL)
    {
      *result = NULL;
      return err;
    }

  len = sizeof(entry->d_name) - 1;
  strncpy (entry->d_name, str, len);
  entry->d_name[len] = '\0';
  entry->d_ino = 0;
  (void)Marshal::FreeHGlobal((long)str);
  *result = entry;

  return err;
}
weak_alias (__readdir_r, readdir_r)
