/*
 * lseek.c - Seek to a new position on a file descriptor.
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

extern long long __syscall_lseek (int fd, long long offset, int whence);

off_t
__libc_lseek (int fd, off_t offset, int whence)
{
  long result = __syscall_lseek (fd, offset, whence);
  if (result != -1L)
    {
      return (off_t)result;
    }
  else
    {
      errno = -((int)result);
      return -1;
    }
}

weak_alias(__libc_lseek, __lseek)
weak_alias(__libc_lseek, lseek)
weak_alias(__libc_lseek, __lseek64)
weak_alias(__libc_lseek, lseek64)
strong_alias(__libc_lseek, __libc_lseek64)
