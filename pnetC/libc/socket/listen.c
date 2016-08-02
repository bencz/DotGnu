/*
 * listen.c - Listen for connections on a socket.
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

int
__listen (int fd, int n)
{
  Socket *socket;
  int result;

  /* Validate the parameter */
  if (n <= 0)
    {
      errno = EINVAL;
      return -1;
    }

  /* Get the socket object associated with the descriptor */
  socket = syscall_get_socket (fd);
  if (!socket)
    return -1;
  if (__syscall_is_listening (fd))
    {
      errno = EOPNOTSUPP;
      return -1;
    }

  /* Listen for connections */
  try
    {
      socket->Listen (n);
    }
  catch (SocketException)
    {
      errno = EOPNOTSUPP;
      return -1;
    }
  __syscall_set_listening (fd, 1);
  return 0;
}

weak_alias (__listen, listen)
