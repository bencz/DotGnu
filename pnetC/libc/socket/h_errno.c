/*
 * h_errno.c - Declaration of "h_errno" and friends.
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

#include <netdb.h>
#include <stdio.h>

#undef h_errno
int __declspec(thread) h_errno;

int *
__h_errno_location (void)
{
  return &h_errno;
}

void
herror (const char *str)
{
  if (str)
    {
      fputs (str, stderr);
      fputs (": ", stderr);
    }
  fputs (hstrerror (h_errno), stderr);
  putc ('\n', stderr);
}

static const char * const h_errors [] = {
        "Resolver Error 0 (no error)",
        "Unknown host",
        "Host name lookup failure",
        "Unknown server error",
        "No address associated with name",
        "Unknown resolver error"
};

const char *
hstrerror (int err_num)
{
  if (err_num < 0 || err_num > 5)
    err_num = 5;
  return h_errors[err_num];
}
