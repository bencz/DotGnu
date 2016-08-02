/*
 * grent.c - Walk through the fake group entry list.
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

/*
 * This source file implements a fake group file consisting of
 * three entries: "root", "users", and "nobody".  The information
 * is sufficient to fool applications into believing that they have
 * a POSIX-like group file, without allowing the application
 * to circumvent system security and walk the real group file.
 */

#include <grp.h>
#include <pwd.h>
#include <string.h>
#include <unistd.h>
#include <pthread-support.h>
#include "grent.h"
#include "pwent.h"
#include "fake-ids.h"

/*
 * Lists of group members.
 */
static char *root_members[] = {FAKE_ROOT_USER, 0};
static char *users_members[] = {0, 0};
static char *nobody_members[] = {FAKE_NOBODY_USER, 0};

int
__nextgrent (int posn, struct group *grp)
{
  switch (posn)
    {
      case 0:
        {
          /* Return information about "root" */
          grp->gr_name = FAKE_ROOT_GROUP;
          grp->gr_gid  = FAKE_ROOT_GID;
          grp->gr_mem  = root_members;
        }
        return 1;

      case 1:
        {
          /* Return information about "users" */
	  struct passwd currentUser;
	  __nextpwent (1, &currentUser);
	  users_members[0] = currentUser.pw_name;
          grp->gr_name = FAKE_CURRENT_GROUP;
          grp->gr_gid  = FAKE_GID;
          grp->gr_mem  = users_members;
        }
        return 1;

      case 2:
        {
          /* Return information about "nobody" */
          grp->gr_name = FAKE_NOBODY_GROUP;
          grp->gr_gid  = FAKE_NOBODY_GID;
          grp->gr_mem  = nobody_members;
        }
        return 1;
    }
  return 0;
}
