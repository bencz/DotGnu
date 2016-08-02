/*
 * fcntl.c - Perform control operations on a file descriptor.
 *
 * This file is part of the Portable.NET C library.
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

#include <unistd.h>
#include <fcntl.h>
#include <stdarg.h>
#include <errno.h>

__using__ OpenSystem::C::FileTable;

extern int  __syscall_is_nonblocking (int fd);
extern void __syscall_set_nonblocking (int fd, int value);

int
fcntl (int fd, int cmd, ...)
{
  va_list va;
  int newfd;

  /* Validate the file descriptor */
  if (!FileTable::GetStream (fd))
    {
      errno = EBADF;
      return -1;
    }

  /* Execute the specified command */
  va_start (va);
  switch (cmd)
    {
      case F_DUPFD:
        newfd = FileTable::DupAfter (fd, va_arg (va, int));
	if (newfd >= 0)
	  return newfd;
	errno = (newfd == -1 ? EMFILE : EBADF);
	return -1;

      case F_GETFD:
        /* Ignored in this implementation */
        return 0;

      case F_SETFD:
        /* Ignored in this implementation */
        return 0;

      case F_GETFL:
        if (__syscall_is_nonblocking (fd))
	  return O_NONBLOCK;
	else
	  return 0;

      case F_SETFL:
        __try__
	{
	  int flags = va_arg (va, int);
	  __syscall_set_nonblocking (fd, (flags & O_NONBLOCK) != 0);
	  return 0;
	}
	__catch__
	{
	  errno = EINVAL;
	  return -1;
	}
    }

  /* The command was not understood */
  errno = EINVAL;
  return -1;
}
