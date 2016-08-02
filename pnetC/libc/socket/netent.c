/*
 * netent.c - Network database entries.
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

/* The "/etc/networks" database is not supported in this implementation.
   The file is normally empty on most people's systems and few modern
   applications make use of the database anyway */

void
setnetent (int stay_open)
{
  /* Nothing to do here */
}

void endnetent (void)
{
  /* Nothing to do here */
}

struct netent *
getnetent (void)
{
  /* Nothing to do here */
  return 0;
}

struct netent *
getnetbyaddr (uint32_t net, int type)
{
  /* Nothing to do here */
  return 0;
}

struct netent *
getnetbyname (const char *name)
{
  /* Nothing to do here */
  return 0;
}

int getnetent_r (struct netent *__restrict result_buf,
			     char *__restrict buf, size_t buflen,
			     struct netent **__restrict result,
			     int *__restrict h_errnop)
{
  if (result)
    *result = 0;
  if (h_errnop)
    *h_errnop = HOST_NOT_FOUND;
  return -1;
}

int getnetbyaddr_r (uint32_t net, int type,
			        struct netent *__restrict result_buf,
			        char *__restrict buf, size_t buflen,
			        struct netent **__restrict result,
			        int *__restrict h_errnop)
{
  if (result)
    *result = 0;
  if (h_errnop)
    *h_errnop = HOST_NOT_FOUND;
  return -1;
}

int getnetbyname_r (const char *__restrict name,
			        struct netent *__restrict result_buf,
			        char *__restrict buf, size_t buflen,
			        struct netent **__restrict result,
			        int *__restrict h_errnop)
{
  if (result)
    *result = 0;
  if (h_errnop)
    *h_errnop = HOST_NOT_FOUND;
  return -1;
}
