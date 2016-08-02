/*
 * pwent.c - Walk through the fake password entry list.
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

/*
 * This source file implements a fake password file consisting of
 * three entries: "root", "user", and "nobody".  The information
 * is sufficient to fool applications into believing that they have
 * a POSIX-like password file, without allowing the application
 * to circumvent system security and walk the real password file.
 */

#include <pwd.h>
#include <string.h>
#include <unistd.h>
#include <pthread-support.h>
#include "pwent.h"
#include "fake-ids.h"

__using__ System::String;
__using__ System::Runtime::InteropServices::Marshal;
__using__ System::Environment;

static __libc_monitor_t currentUserMutex = __LIBC_MONITOR_INITIALIZER;
static char *currentUser, *currentReal, *currentHome, *currentShell;

int
__nextpwent (int posn, struct passwd *pwd)
{
  switch (posn)
    {
      case 0:
        {
          /* Return information about "root" */
          pwd->pw_name   = FAKE_ROOT_USER;
          pwd->pw_passwd = "*";
          pwd->pw_uid    = FAKE_ROOT_UID;
          pwd->pw_gid    = FAKE_ROOT_GID;
          pwd->pw_gecos  = FAKE_ROOT_REAL;
          pwd->pw_dir    = FAKE_ROOT_HOME;
          pwd->pw_shell  = FAKE_SHELL;
        }
        return 1;

      case 1:
        {
          /* Return information about "user" */
	  __libc_monitor_lock(&currentUserMutex);
	  if(!currentUser)
	    {
	      currentUser = getlogin();
	      if(!strcmp(currentUser, "nobody"))
	        {
		  /* The runtime engine will not return user information */
		  currentUser = FAKE_CURRENT_USER;
		  currentReal = FAKE_CURRENT_REAL;
		  currentHome = FAKE_CURRENT_HOME;
		  currentShell = FAKE_SHELL;
		}
	      else
	        {
		  /* Fetch the rest of the user information */
		  String str;
		  currentReal = currentUser;
		  str = Environment::GetEnvironmentVariable("HOME");
		  currentHome = (char *)Marshal::StringToHGlobalAnsi(str);
		  if(!currentHome)
		    currentHome = FAKE_CURRENT_HOME;
		  str = Environment::GetEnvironmentVariable("SHELL");
		  currentShell = (char *)Marshal::StringToHGlobalAnsi(str);
		  if(!currentShell)
		    currentShell = FAKE_SHELL;
		}
	    }
          pwd->pw_name   = currentUser;
          pwd->pw_passwd = "*";
          pwd->pw_uid    = FAKE_UID;
          pwd->pw_gid    = FAKE_GID;
          pwd->pw_gecos  = currentReal;
          pwd->pw_dir    = currentHome;
          pwd->pw_shell  = currentShell;
	  __libc_monitor_unlock(&currentUserMutex);
        }
        return 1;

      case 2:
        {
          /* Return information about "nobody" */
          pwd->pw_name   = FAKE_NOBODY_USER;
          pwd->pw_passwd = "*";
          pwd->pw_uid    = FAKE_NOBODY_UID;
          pwd->pw_gid    = FAKE_NOBODY_GID;
          pwd->pw_gecos  = FAKE_NOBODY_REAL;
          pwd->pw_dir    = FAKE_NOBODY_HOME;
          pwd->pw_shell  = FAKE_SHELL;
        }
        return 1;
    }
  return 0;
}
