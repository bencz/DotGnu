/*
 * grent.h - Walk through the fake group entry list.
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

#ifndef _GRENT_H
#define _GRENT_H

/* Information that is stored on the current thread for
   the "setgrent", "getgrent", and "endgrent" functions */
struct group_info
  {
    int          gi_posn;
    struct group gi_grp;
  };

extern struct group_info __grinfo;
extern int __nextgrent(int posn, struct group *grp);
extern int __persistgr(struct group *grp, char *buffer, size_t buflen);

#endif /* _GRENT_H */
