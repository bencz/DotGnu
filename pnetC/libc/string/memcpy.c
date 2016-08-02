/*
 * memcpy.c - Copy memory area.
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

#include <string.h>

/* We make both "memcpy" and "memmove" handle overlapping memory
   regions so that they have the same behaviour on all platforms */

void *
memcpy (void * __restrict dest, const void * __restrict src, size_t n)
{
  char *d = (char *)dest;
  if (d < (char *)src)
    {
      if ((d + n) <= (char *)src)
        {
          /* There is no overlap, so it is safe to use "cpblk" */
          __asm__ __volatile__ (
            "\tldarg.0\n"
            "\tldarg.1\n"
            "\tldarg.2\n"
            "\tunaligned. 1\n"
            "\tcpblk\n"
          ::);
        }
      else
        {
          while (n > 0)
            {
              *d++ = *((const char *)src);
              ++src;
              --n;
            }
        }
    }
  else
    {
      if ((((char *)src) + n) <= d)
        {
          /* There is no overlap, so it is safe to use "cpblk" */
          __asm__ __volatile__ (
            "\tldarg.0\n"
            "\tldarg.1\n"
            "\tldarg.2\n"
            "\tunaligned. 1\n"
            "\tcpblk\n"
          ::);
        }
      else
        {
          d += n;
          src += n;
          while (n > 0)
            {
              --src;
              *(--d) = *((const char *)src);
              --n;
            }
        }
    }
  return dest;
}

strong_alias(memcpy, memmove)

void
bcopy (const void *src, void *dest, size_t n)
{
  memcpy (dest, src, n);
}
