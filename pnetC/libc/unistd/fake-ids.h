/*
 * fake-ids.h - Fake uid/gid/pid values.
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

#ifndef _FAKE_IDS_H
#define _FAKE_IDS_H

/*
 * Functions like "getuid", "setgid", "getpid", etc, are not
 * safe to use in a secure execution environment, because they
 * may reveal too much about the user or the user's system.
 *
 * To alleviate the privacy threat, we use fake identifiers in
 * situations where the application expects to use a real user,
 * group, or process identifier.
 */

#define FAKE_UID                100
#define FAKE_GID                100
#define FAKE_CURRENT_USER       "user"
#define FAKE_CURRENT_GROUP      "users"
#define FAKE_CURRENT_REAL       "J. Random User"
#define FAKE_CURRENT_HOME       "/home/user"

#define FAKE_PPID               1000
#define FAKE_PID                1001
#define FAKE_FIRST_CHILD_PID    1002

#define FAKE_NOBODY_UID         32766
#define FAKE_NOBODY_GID         32766
#define FAKE_NOBODY_USER        "nobody"
#define FAKE_NOBODY_GROUP       "nobody"
#define FAKE_NOBODY_REAL        "Nobody"
#define FAKE_NOBODY_HOME        "/"

#define FAKE_ROOT_UID           0
#define FAKE_ROOT_GID           0
#define FAKE_ROOT_USER          "root"
#define FAKE_ROOT_GROUP         "root"
#define FAKE_ROOT_REAL          "root"
#define FAKE_ROOT_HOME          "/root"

#define FAKE_SHELL              "/bin/sh"

#endif /* _FAKE_IDS_H */
