/*
 * strtok.c - Split a string into tokens.
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
strtok_r (char *s, const char *delim, char **ptrptr)
{
  size_t len;
  if (!s)
    {
      s = *ptrptr;
    }
  len = strspn (s, delim);
  s += len;
  if (*s == '\0')
    {
      return 0;
    }
  *ptrptr = strpbrk (s, delim);
  if (!(*ptrptr))
    {
      *ptrptr = strchr (s, '\0');
    }
  else
    {
      *((*ptrptr)++) = '\0';
    }
  return s;
}

char *
strtok (char *s, const char *delim)
{
  static char *ptrptr;
  return strtok_r (s, delim, &ptrptr);
}
