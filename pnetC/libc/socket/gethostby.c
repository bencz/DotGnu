/*
 * gethostby.c - Get a host entry by name or address.
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
#include <string.h>
#include <stdlib.h>
#include "socket-glue.h"

/*
 * Global host entry for this thread.
 */
static struct hostent __declspec(thread) h_entry;

/*
 * Free a NULL-terminated list of blocks.
 */
static void
free_list (char **list)
{
  int posn;
  if (!list)
    return;
  posn = 0;
  while (list[posn])
    {
      (void)Marshal::FreeHGlobal ((long)(list[posn]));
      ++posn;
    }
  (void)Marshal::FreeHGlobal ((long)list);
}

/*
 * Free the information in a host entry.  We use the HGlobal routines
 * to avoid problems when "malloc" and "free" are redirected.
 */
static void
free_hostent (struct hostent *ent)
{
  if (ent->h_name)
    (void)Marshal::FreeHGlobal ((long)(ent->h_name));
  free_list (ent->h_aliases);
  free_list (ent->h_addr_list);
}

static struct hostent *
gethostbyaddr_i (struct hostent *ent, const void *addr,
                 socklen_t len, int type)
{
  IPAddress *ipaddr;
  IPHostEntry *entry;

  /* Convert the address into its C# form */
  ipaddr = __inaddr_to_ipaddress (addr, len, type);
  if (!ipaddr)
    {
      h_errno = NETDB_INTERNAL;
      return 0;
    }

  /* Perform the host lookup */
  entry = Dns::GetHostByAddress (ipaddr);
  if (!entry)
    {
      h_errno = HOST_NOT_FOUND;
      return 0;
    }
  h_errno = NETDB_SUCCESS;
  __syscall_convert_hostent (entry, (long)ent);
  return ent;
}

struct hostent *
__gethostbyaddr (const void *addr, socklen_t len, int type)
{
  free_hostent (&h_entry);
  return gethostbyaddr_i (&h_entry, addr, len, type);
}

static struct hostent *
gethostbyname_i (struct hostent *ent, const char *name)
{
  IPHostEntry *entry;

  /* Bail out if the name is invalid */
  if (!name)
    {
      errno = EFAULT;
      h_errno = NETDB_INTERNAL;
      return 0;
    }

  /* Perform the host lookup */
  entry = Dns::GetHostByName (name);
  if (!entry)
    {
      h_errno = HOST_NOT_FOUND;
      return 0;
    }
  h_errno = NETDB_SUCCESS;
  __syscall_convert_hostent (entry, (long)ent);
  return ent;
}

struct hostent *
__gethostbyname (const char *name)
{
  free_hostent (&h_entry);
  return gethostbyname_i (&h_entry, name);
}

struct hostent *
__gethostbyname2 (const char *name, int af)
{
  /* Don't know how to do IPv6 lookup in C# yet */
  if (af == AF_INET)
    return __gethostbyname (name);
  else
    return 0;
}

/*
 * Align a persist buffer on a pointer boundary.
 */
static void
persist_align (char **buf, size_t *buflen)
{
  if (*buf)
    {
      long pad = ((long)*buf) % (long)(sizeof (void *));
      if (pad != 0)
        {
          pad = ((long)(sizeof (void *))) - pad;
	  if (pad <= *buflen)
	    {
	      *buflen -= (size_t)pad;
	      *buf += pad;
	    }
	  else
	    {
	      *buflen = 0;
	    }
	}
    }
}

/*
 * Allocate memory from a persist buffer.
 */
static void *
persist_alloc (char **buf, size_t *buflen, size_t size)
{
  if (*buf && size <= *buflen)
    {
      void *current = (void *)(*buf);
      *buf += size;
      *buflen -= size;
      return current;
    }
  else
    {
      *buflen = 0;
      return 0;
    }
}

/*
 * Allocate a string within a persist buffer.
 */
static char *
persist_string (char **buf, size_t *buflen, const char *str)
{
  void *copy;
  if (!str)
    return 0;
  copy = persist_alloc (buf, buflen, strlen (str) + 1);
  if (copy)
    strcpy (copy, str);
  return copy;
}

/*
 * Copy host entry information to a re-entrant buffer.
 */
static void
copy_hostent_to_buffer (struct hostent *__restrict result_buf,
		        char *__restrict buf, size_t buflen,
			struct hostent *ent)
{
  int len, posn;

  if (!result_buf)
    return;

  /* Copy the primary hostname */
  result_buf->h_name = persist_string (&buf, &buflen, ent->h_name);

  /* Copy the alias list */
  persist_align (&buf, &buflen);
  len = 0;
  while (ent->h_aliases && ent->h_aliases[len])
    ++len;
  result_buf->h_aliases =
  	persist_alloc (&buf, &buflen, sizeof(char *) * (len + 1));
  if (result_buf->h_aliases)
    {
      posn = 0;
      while (posn < len)
        {
          result_buf->h_aliases[posn] =
	  	persist_string (&buf, &buflen, ent->h_aliases[posn]);
	  ++posn;
	}
      result_buf->h_aliases[posn] = 0;
    }

  /* Copy the address type and length */
  result_buf->h_addrtype = ent->h_addrtype;
  result_buf->h_length = ent->h_length;

  /* Copy the address list */
  persist_align (&buf, &buflen);
  len = 0;
  while (ent->h_addr_list && ent->h_addr_list[len])
    ++len;
  result_buf->h_addr_list =
  	persist_alloc (&buf, &buflen, sizeof(char *) * (len + 1));
  if (result_buf->h_addr_list)
    {
      posn = 0;
      while (posn < len)
        {
  	  persist_align (&buf, &buflen);
          result_buf->h_addr_list[posn] =
	  	persist_alloc (&buf, &buflen, ent->h_length);
	  if (result_buf->h_addr_list[posn])
	    {
	      memcpy (result_buf->h_addr_list[posn], ent->h_addr_list[posn],
	              ent->h_length);
	    }
	  ++posn;
	}
      result_buf->h_addr_list[posn] = 0;
    }
}

int
gethostbyaddr_r (const void *__restrict addr, socklen_t len, int type,
		 struct hostent *__restrict result_buf,
		 char *__restrict buf, size_t buflen,
		 struct hostent **__restrict result,
		 int *__restrict h_errnop)
{
  int saveErrno = errno;
  int saveHerrno = h_errno;
  int error, resultValue;
  struct hostent tempent;
  struct hostent *ent;
  ent = gethostbyaddr_i (&tempent, addr, len, type);
  if (ent || h_errno != NETDB_INTERNAL)
    resultValue = 0;
  else
    resultValue = errno;
  error = h_errno;
  errno = saveErrno;
  h_errno = saveHerrno;
  if (!ent)
    {
      if (result)
        *result = 0;
      if (h_errnop)
        *h_errnop = error;
      return resultValue;
    }
  copy_hostent_to_buffer (result_buf, buf, buflen, ent);
  free_hostent (&tempent);
  if (result)
    *result = result_buf;
  if (h_errnop)
    *h_errnop = error;
  return resultValue;
}

int
gethostbyname_r (const char *__restrict name,
                 struct hostent *__restrict result_buf,
                 char *__restrict buf, size_t buflen,
                 struct hostent **__restrict result,
                 int *__restrict h_errnop)
{
  int saveErrno = errno;
  int saveHerrno = h_errno;
  int error, resultValue;
  struct hostent tempent;
  struct hostent *ent;
  ent = gethostbyname_i (&tempent, name);
  if (ent || h_errno != NETDB_INTERNAL)
    resultValue = 0;
  else
    resultValue = errno;
  error = h_errno;
  errno = saveErrno;
  h_errno = saveHerrno;
  if (!ent)
    {
      if (result)
        *result = 0;
      if (h_errnop)
        *h_errnop = error;
      return resultValue;
    }
  copy_hostent_to_buffer (result_buf, buf, buflen, ent);
  free_hostent (&tempent);
  if (result)
    *result = result_buf;
  if (h_errnop)
    *h_errnop = error;
  return resultValue;
}

int
gethostbyname2_r (const char *__restrict name, int af,
                  struct hostent *__restrict result_buf,
                  char *__restrict buf, size_t buflen,
                  struct hostent **__restrict result,
                  int *__restrict h_errnop)
{
  int saveErrno = errno;
  int saveHerrno = h_errno;
  int error, resultValue;
  struct hostent tempent;
  struct hostent *ent;
  if (af == AF_INET)
    ent = gethostbyname_i (&tempent, name);
  else
    {
      memset (&tempent, 0, sizeof (tempent));
      h_errno = HOST_NOT_FOUND;
      ent = 0;
    }
  if (ent || h_errno != NETDB_INTERNAL)
    resultValue = 0;
  else
    resultValue = errno;
  error = h_errno;
  errno = saveErrno;
  h_errno = saveHerrno;
  if (!ent)
    {
      if (result)
        *result = 0;
      if (h_errnop)
        *h_errnop = error;
      return resultValue;
    }
  copy_hostent_to_buffer (result_buf, buf, buflen, ent);
  free_hostent (&tempent);
  if (result)
    *result = result_buf;
  if (h_errnop)
    *h_errnop = error;
  return resultValue;
}

weak_alias (__gethostbyaddr, gethostbyaddr)
weak_alias (__gethostbyname, gethostbyname)
weak_alias (__gethostbyname2, gethostbyname2)
