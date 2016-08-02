/*
 * strcmp.c - Compare two strings.
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

int
strcmp (const char *s1, const char *s2)
{
  int c1, c2;
  do
    {
      /* Use "unsigned char" to make the implementation 8-bit clean */
      c1 = *((unsigned char *)(s1++));
      c2 = *((unsigned char *)(s2++));
      if (c1 != c2)
        {
          return (c1 - c2);
        }
    }
  while (c1 != 0);
  return 0;
}
