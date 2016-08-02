/*
 * isfdtype.c - Determine if an fd is a socket or something else.
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

#include "socket-glue.h"
#include <sys/stat.h>

int
__isfdtype (int fd, int fdtype)
{
  int error;
  Socket *socket;
#ifdef S_IFSOCK
  if (fdtype == S_IFSOCK || fdtype == S_IFIFO)
#else
  if (fdtype == S_IFIFO)
#endif
    {
      socket = __syscall_get_socket (fd, &error);
      if (socket)
        return 1;
      else
        return 0;
    }
  else if (fdtype == S_IFREG)
    {
      socket = __syscall_get_socket (fd, &error);
      if (socket)
        return 0;
      else
        return (error == ENOTSOCK);
    }
  else
    {
      return 0;
    }
}

weak_alias (__isfdtype, isfdtype)
