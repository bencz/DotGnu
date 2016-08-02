/*
 * realloc.c - Re-allocate memory from the C heap.
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

#include <malloc.h>
#include <stdint.h>
#include <errno.h>

__using__ System::Runtime::InteropServices::Marshal;

void *
__realloc(void *ptr, size_t size)
{
  /* Validate the parameters and determine the kind of realloc to perform */
  if(ptr == 0)
    {
      /* Perform a "malloc" operation */
      if(size == 0)
        {
          size = 1;
        }
      else if(size > (size_t)INT32_MAX)
        {
          errno = ENOMEM;
          return 0;
        }
      ptr = (void *)Marshal::AllocHGlobal((long)size);
    }
  else if(size == 0)
    {
      /* Perform a "free" operation */
      (void)Marshal::FreeHGlobal((long)ptr);
      return 0;
    }
  else if(size > (size_t)INT32_MAX)
    {
      errno = ENOMEM;
      return 0;
    }
  else
    {
      ptr = (void *)Marshal::ReAllocHGlobal((long)ptr, (long)size);
    }
  if(ptr != 0)
    {
      return ptr;
    }
  errno = ENOMEM;
  return ptr;
}

weak_alias(__realloc, realloc)
