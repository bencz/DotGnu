/*
 * opendir.c - Open a directory stream, given its name.
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
#include <stdlib.h>
#include <string.h>
#include <dirent.h>
#include "dirent-glue.h"

DIR *
__opendir (const char *name)
{
  DIR *retval;
  int err;

  if (name == NULL || *name == '\0')
    {
      errno = ENOENT;
      return NULL;
    }

  retval = (DIR *)calloc (1, sizeof(DIR));
  if (retval == NULL)
    {
      errno = ENOMEM;
      return NULL;
    }

  err = 0;
  retval->gc_handle = __syscall_opendir (name, &err);

  if (err)
    {
      free (retval);
      errno = err;
      return NULL;
    }

  return retval;
}
weak_alias (__opendir, opendir)
