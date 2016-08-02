/*
 * rewinddir.c - Rewind the DIR position to the beginning of the directory.
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

#include <stddef.h>
#include <dirent.h>
#include "dirent-glue.h"

void
__rewinddir (DIR *dirp)
{
  int err;

  if (dirp == NULL || dirp->gc_handle == NULL)
    {
      return;
    }

  err = 0;
  __syscall_rewinddir (dirp->gc_handle, &err);
}
weak_alias (__rewinddir, rewinddir)
