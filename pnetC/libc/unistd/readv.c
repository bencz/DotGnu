/*
 * readv.c - Read vectors of data from a file descriptor.
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
#include <sys/uio.h>
#include <errno.h>

extern int __syscall_read (int fd, long buf, unsigned int count);

ssize_t
__readv (int fd, const struct iovec *vector, int count)
{
  int posn;
  int result;
  int len;

  /* Validate the parameters */
  if (!vector)
    {
      errno = EFAULT;
      return -1;
    }
  else if (count <= 0)
    {
      errno = EINVAL;
      return -1;
    }
  for (posn = 0; posn < count; ++posn)
    {
      if (!(vector[posn].iov_base))
        {
          errno = EFAULT;
	  return -1;
        }
    }

  /* Read data into the supplied buffers */
  result = 0;
  for (posn = 0; posn < count; ++posn)
    {
      if (!(vector[posn].iov_len))
        continue;
      len = __syscall_read (fd, (long)(vector[posn].iov_base),
      			    vector[posn].iov_len);
      if (len > 0)
        {
	  result += len;
	  if (((size_t)len) < vector[posn].iov_len)
	    break;
	}
      else if(len < 0 && posn == 0)
        {
	  errno = -len;
	  return -1;
	}
      else
        {
	  break;
	}
    }
  return (ssize_t)result;
}

weak_alias(__readv, readv)
