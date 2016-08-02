/*
 * read.c - Read from a file descriptor.
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

extern int __syscall_read (int fd, long buf, unsigned int count);

ssize_t
__libc_read (int fd, void *buf, size_t count)
{
  int result = __syscall_read (fd, (long)buf, count);
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

weak_alias(__libc_read, __read)
weak_alias(__libc_read, read)
