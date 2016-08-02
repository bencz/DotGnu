/*
 * strstr.c - Search for one string within another.
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

char *
strstr (const char *haystack, const char *needle)
{
  size_t hlen = strlen (haystack);
  size_t nlen = strlen (needle);
  size_t posn;
  if (!nlen)
    {
      return (char *)haystack;
    }
  while (hlen >= nlen)
    {
      for (posn = 0; posn < nlen; ++posn)
        {
          if (haystack[posn] != needle[posn])
            {
              break;
            }
        }
      if (posn >= nlen)
        {
          return (char *)haystack;
        }
      ++haystack;
      --hlen;
    }
  return NULL;
}
