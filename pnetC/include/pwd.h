/*
 * pwd.h - Password file emulation.
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

#ifndef _PWD_H
#define _PWD_H

#include <features.h>
#include <sys/types.h>

__BEGIN_DECLS

/* Password file entry information block */
struct passwd
  {
    char *pw_name;
    char *pw_passwd;
    uid_t pw_uid;
    gid_t pw_gid;
    char *pw_gecos;
    char *pw_dir;
    char *pw_shell;
  };

/* Function prototypes */
extern void setpwent(void);
extern struct passwd *getpwent(void);
extern void endpwent(void);
extern struct passwd *getpwnam(const char *__name);
extern struct passwd *getpwuid(uid_t __uid);
extern int getpwnam_r(const char *__name,
                      struct passwd * __restrict __resultbuf,
                      char * __restrict __buffer, size_t __buflen,
                      struct passwd ** __restrict __result);
extern int getpwuid_r(uid_t __uid,
                      struct passwd * __restrict __resultbuf,
                      char * __restrict __buffer, size_t __buflen,
                      struct passwd ** __restrict __result);

__END_DECLS

#endif  /* !_PWD_H */
