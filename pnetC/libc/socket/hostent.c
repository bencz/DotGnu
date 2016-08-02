/*
 * hostent.c - Host database entries.
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

/* The "/etc/hosts" database is not supported in this implementation.
   Use "gethostbyname" or "gethostbyaddr" instead */

void
sethostent (int stay_open)
{
  /* Nothing to do here */
}

void
endhostent (void)
{
  /* Nothing to do here */
}

struct hostent *
gethostent (void)
{
  /* Nothing to do here */
  return 0;
}

int
gethostent_r (struct hostent *__restrict result_buf,
              char *__restrict buf, size_t buflen,
			  struct hostent **__restrict result,
			  int *__restrict h_errnop)
{
  if (result)
    *result = 0;
  if (h_errnop)
    *h_errnop = HOST_NOT_FOUND;
  return -1;
}
