/*
 * fcntl.h - File control options.
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

#ifndef _FCNTL_H
#define _FCNTL_H

#include <features.h>
#include <sys/types.h>
#ifdef __USE_XOPEN
/*#include <sys/stat.h> */ /* TODO */
#include <unistd.h>
#endif

__BEGIN_DECLS

/* File open modes */
#define	O_ACCMODE		000003
#define	O_RDONLY		000000
#define	O_WRONLY		000001
#define	O_RDWR			000002
#define O_CREAT			000100
#define O_EXCL			000200
#define O_NOCTTY		000400
#define	O_TRUNC			001000
#define	O_APPEND		002000
#define	O_NONBLOCK		004000
#define	O_NDELAY		O_NONBLOCK
#define	O_SYNC			010000
#define	O_FSYNC			O_SYNC
#define	O_DSYNC			O_SYNC
#define	O_RSYNC			O_SYNC
#define	O_ASYNC			020000

/* File control commands */
#define	F_DUPFD			0
#define	F_GETFD			1
#define	F_SETFD			2
#define	F_GETFL			3
#define	F_SETFL			4
#define	F_GETLK			5
#define	F_SETLK			6
#define	F_SETLKW		7
#define	F_GETLK64		F_GETLK
#define	F_SETLK64		F_SETLK
#define	F_SETLKW64		F_SETLKW
#define	F_GETOWN		8
#define	F_SETOWN		9

/* Special values for use with fcntl */
#define	FD_CLOEXEC		1
#define	F_RDLCK			0
#define	F_WRLCK			1
#define	F_UNLCK			2

/* Lock control structures */
struct flock
  {
    short l_type;
    short l_whence;
    off_t l_start;
    off_t l_len;
    pid_t l_pid;
  };
struct flock64
  {
    short l_type;
    short l_whence;
    off64_t l_start;
    off64_t l_len;
    pid_t l_pid;
  };

/* Function prototypes */
extern int creat(const char *__path, mode_t __mode);
extern int creat64(const char *__path, mode_t __mode);
extern int fcntl(int __fd, int __cmd, ...);
extern int open(const char *__path, int __flags, ...);
extern int open64(const char *__path, int __flags, ...);

__END_DECLS

#endif /* _FCNTL_H */
