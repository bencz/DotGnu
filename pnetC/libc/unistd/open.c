/*
 * open.c - Open a file descriptor.
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
#include <fcntl.h>
#include <errno.h>

/* System.IO.FileMode values */
#define	FileMode_CreateNew	1
#define FileMode_Create		2
#define	FileMode_Open		3
#define	FileMode_OpenOrCreate	4
#define	FileMode_Truncate	5
#define	FileMode_Append		6

/* System.IO.FileAccess values */
#define	FileAccess_Read		1
#define	FileAccess_Write	2
#define	FileAccess_ReadWrite	3

extern int __syscall_open (long path, int mode, int access);

int
__libc_open (const char *path, int flags, ...)
{
  int mode;
  int access;
  int result;

  /* Bail out if "path" is obviously invalid */
  if (!path)
    {
      errno = EFAULT;
      return -1;
    }

  /* Convert the flags into System.IO mode and access values */
  mode = (flags & (O_CREAT | O_EXCL | O_TRUNC | O_APPEND));
  if (mode == (O_CREAT | O_EXCL))
    {
      mode = FileMode_CreateNew;
    }
  else if (mode == (O_CREAT | O_TRUNC))
    {
      mode = FileMode_Create;
    }
  else if (mode == 0)
    {
      mode = FileMode_Open;
    }
  else if (mode == O_CREAT)
    {
      mode = FileMode_OpenOrCreate;
    }
  else if (mode == O_TRUNC)
    {
      mode = FileMode_Truncate;
    }
  else if (mode == (O_CREAT | O_APPEND))
    {
      mode = FileMode_Append;
    }
  else
    {
      errno = EACCES;
      return -1;
    }
  if ((flags & O_ACCMODE) == O_RDONLY)
    {
      access = FileAccess_Read;
    }
  else if ((flags & O_ACCMODE) == O_WRONLY)
    {
      access = FileAccess_Write;
    }
  else if ((flags & O_ACCMODE) == O_RDWR)
    {
      access = FileAccess_ReadWrite;
    }
  else
    {
      errno = EACCES;
      return -1;
    }

  /* Perform the open operation */
  result = __syscall_open ((long)path, mode, access);
  if (result >= 0)
    {
      return result;
    }
  else
    {
      errno = -result;
      return -1;
    }
}

int
__creat (const char *path, mode_t mode)
{
  int result;
  if (!path)
    {
      errno = EFAULT;
      return -1;
    }
  result = __syscall_open ((long)path, FileMode_Create, FileAccess_Write);
  if (result >= 0)
    {
      return result;
    }
  else
    {
      errno = -result;
      return -1;
    }
}

/* Weak aliases are impossible to implement for vararg methods within
   this environment, so we have to use strong aliases for "open" instead */
strong_alias(__libc_open, __open)
strong_alias(__libc_open, open)
strong_alias(__libc_open, __open64)
strong_alias(__libc_open, open64)
strong_alias(__libc_open, __libc_open64)
weak_alias(__creat, creat)
weak_alias(__creat, __creat64)
weak_alias(__creat, creat64)
