/*
 * getlogin.c - Get the user's login name.
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

#include <unistd.h>
#include <string.h>
#include <pthread-support.h>

__using__ System::String;
__using__ System::Runtime::InteropServices::Marshal;
__using__ System::Environment;

static __libc_monitor_t loginMutex = __LIBC_MONITOR_INITIALIZER;
static char *loginName;

char *
getlogin (void)
{
  char *login;
  __libc_monitor_lock(&loginMutex);
  if (!loginName)
    {
       String str = Environment::get_UserName ();
       loginName = (char *)Marshal::StringToHGlobalAnsi (str);
       if (!loginName)
         {
	   loginName = strdup("nobody");
	 }
    }
  login = loginName;
  __libc_monitor_unlock(&loginMutex);
  return login;
}

int
getlogin_r (char *name, size_t name_len)
{
  char *login = getlogin();
  strncpy (name, login, name_len);
  return strlen (login);
}
