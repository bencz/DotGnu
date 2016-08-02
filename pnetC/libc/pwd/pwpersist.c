/*
 * pwpersist.c - Persist a password entry within a re-entrant return buffer.
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

#include <pwd.h>
#include <grp.h>
#include <string.h>
#include "pwent.h"
#include "grent.h"

static char *
persistpw (char *value, char **buffer, size_t *buflen)
{
  size_t len = strlen (value);
  char *result;
  if (len >= *buflen)
    {
      return 0;
    }
  strcpy (*buffer, value);
  result = *buffer;
  *buffer += len + 1;
  *buflen -= len + 1;
  return result;
}

int
__persistpw (struct passwd *pwd, char *buffer, size_t buflen)
{
  if ((pwd->pw_name = persistpw (pwd->pw_name, &buffer, &buflen)) == 0)
    {
      return 0;
    }
  if ((pwd->pw_passwd = persistpw (pwd->pw_passwd, &buffer, &buflen)) == 0)
    {
      return 0;
    }
  if ((pwd->pw_gecos = persistpw (pwd->pw_gecos, &buffer, &buflen)) == 0)
    {
      return 0;
    }
  if ((pwd->pw_dir = persistpw (pwd->pw_dir, &buffer, &buflen)) == 0)
    {
      return 0;
    }
  if ((pwd->pw_shell = persistpw (pwd->pw_shell, &buffer, &buflen)) == 0)
    {
      return 0;
    }
  return 1;
}

int
__persistgr (struct group *grp, char *buffer, size_t buflen)
{
  char **members;
  int len;
  size_t round;

  /* Persist the group name */
  if ((grp->gr_name = persistpw (grp->gr_name, &buffer, &buflen)) == 0)
    {
      return 0;
    }

  /* Determine the length of the group member list */
  len = 0;
  while (grp->gr_mem[len] != 0)
    ++len;
  ++len;

  /* Allocate space for the array of group member pointers */
  round = (size_t)(((long)buffer) % sizeof (char *));
  if (round != 0)
    {
      round = sizeof (char *) - round;
      if (buflen < round)
        {
	  return 0;
	}
      buffer += round;
      buflen -= round;
    }
  members = (char **)buffer;
  round = len * sizeof (char *);
  if (buflen < round)
    {
      return 0;
    }
  buffer += round;
  buflen -= round;

  /* Persist the group members */
  len = 0;
  while (grp->gr_mem[len] != 0)
    {
      members[len] = persistpw (grp->gr_mem[len], &buffer, &buflen);
      if (!(members[len]))
        return 0;
      ++len;
    }
  members[len] = 0;
  grp->gr_mem = members;
  return 1;
}
