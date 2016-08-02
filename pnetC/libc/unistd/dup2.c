/*
 * dup2.c - Duplicate a file descriptor and overwrite another.
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

__using__ OpenSystem::C::FileTable;

int
__dup2 (int oldfd, int newfd)
{
  int result = FileTable::Dup2 (oldfd, newfd);
  if (result >= 0)
    {
      return result;
    }
  else
    {
      errno = EBADF;
      return -1;
    }
}

weak_alias(__dup2, dup2)
