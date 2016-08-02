/*
 * sockaddr.c - Convert socket addresses to and from C# end points.
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
#include <string.h>

int
__sockaddr_to_endpoint (int fd, EndPoint &ep,
                        const struct sockaddr *addr, socklen_t addrlen)
{
  int family = __syscall_socket_family (fd);
  ep = null;
  if (family < 0)
    {
      errno = -family;
      return -1;
    }
  if (!addr)
    {
      errno = EFAULT;
      return -1;
    }
  if (addrlen < sizeof (struct sockaddr))
    {
      errno = EINVAL;
      return -1;
    }
  else if (addr->sa_family != family)
    {
      errno = EINVAL;
      return -1;
    }
  if (family == AF_INET)
    {
      if (addrlen < sizeof(struct sockaddr_in))
        {
          errno = EINVAL;
          return -1;
	}
      ep = __create_ipv4_endpoint
           (((const struct sockaddr_in *)addr)->sin_addr.s_addr,
	    (int)(ntohs (((const struct sockaddr_in *)addr)->sin_port)));
      return 0;
    }
  else if (family == AF_INET6)
    {
      if (addrlen < sizeof(struct sockaddr_in6))
        {
          errno = EINVAL;
          return -1;
	}
      ep = __create_ipv6_endpoint
           ((long)(((const struct sockaddr_in6 *)addr)->sin6_addr.s6_addr),
            ((const struct sockaddr_in6 *)addr)->sin6_scope_id,
	    (int)(ntohs (((const struct sockaddr_in6 *)addr)->sin6_port)));
      return 0;
    }
  else
    {
      errno = EINVAL;
      return -1;
    }
}

int
__endpoint_to_sockaddr (int fd, EndPoint *ep,
                        struct sockaddr *addr, socklen_t *addrlen)
{
  int family = __syscall_socket_family (fd);
  int port;
  ep = null;
  if (family < 0)
    {
      errno = -family;
      return -1;
    }
  if (!addr || !addrlen || !ep)
    {
      errno = EFAULT;
      return -1;
    }
  if (family == AF_INET)
    {
      if (*addrlen < sizeof(struct sockaddr_in))
        {
          errno = EINVAL;
          return -1;
	}
      memset (addr, 0, sizeof (struct sockaddr_in));
      ((struct sockaddr_in *)addr)->sin_family = AF_INET;
      port = 0;
      if (!__decode_ipv4_endpoint
      	   (ep, ((struct sockaddr_in *)addr)->sin_addr.s_addr, port))
        {
          errno = EINVAL;
          return -1;
	}
      ((struct sockaddr_in *)addr)->sin_port = htons ((uint16_t)port);
      *addrlen = sizeof (struct sockaddr_in);
      return 0;
    }
  else if (family == AF_INET6)
    {
      if (*addrlen < sizeof(struct sockaddr_in6))
        {
          errno = EINVAL;
          return -1;
	}
      memset (addr, 0, sizeof (struct sockaddr_in6));
      ((struct sockaddr_in6 *)addr)->sin6_family = AF_INET6;
      port = 0;
      if (!__decode_ipv6_endpoint
      	   (ep, (long)(((struct sockaddr_in6 *)addr)->sin6_addr.s6_addr),
	    ((struct sockaddr_in6 *)addr)->sin6_scope_id, port))
        {
          errno = EINVAL;
          return -1;
	}
      ((struct sockaddr_in6 *)addr)->sin6_port = htons ((uint16_t)port);
      *addrlen = sizeof (struct sockaddr_in6);
      return 0;
    }
  else
    {
      errno = EINVAL;
      return -1;
    }
}

IPAddress *
__inaddr_to_ipaddress (const void *addr, socklen_t len, int type)
{
  EndPoint *ep;
  if (!addr)
    {
      errno = EFAULT;
      return null;
    }
  if (type == AF_INET)
    {
      if (len < sizeof (struct in_addr))
        {
	  errno = EINVAL;
	  return null;
	}
      ep = __create_ipv4_endpoint
           (((const struct in_addr *)addr)->s_addr, 0);
      return ((IPEndPoint *)ep)->Address;
    }
  else if (type == AF_INET6)
    {
      if (len < sizeof (struct in6_addr))
        {
	  errno = EINVAL;
	  return null;
	}
      ep = __create_ipv6_endpoint
           ((long)(((const struct in6_addr *)addr)->s6_addr), 0, 0);
      return ((IPEndPoint *)ep)->Address;
    }
  else
    {
      errno = EINVAL;
      return null;
    }
}
