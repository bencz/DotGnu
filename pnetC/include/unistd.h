/*
 * unistd.h - POSIX header file for system calls.
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

#ifndef _UNISTD_H
#define _UNISTD_H

#include <features.h>
#include <stddef.h>
#include <sys/types.h>

__BEGIN_DECLS

/* The contents of this file are based on the "Single Unix Specification,
   Version 3", available from "http://www.unix-systems.org/" */

/* Version values */
#define	_POSIX_VERSION		200112L
#define	_POSIX2_VERSION		200112L
#define	_XOPEN_VERSION		600

/* TODO: POSIX capability test macros */

/* Values for "access" */
#define	F_OK			0
#define	X_OK			1
#define	W_OK			2
#define	R_OK			4

/* Values for "lseek" */
#ifndef SEEK_SET
#define SEEK_SET		0
#define	SEEK_CUR		1
#define	SEEK_END		2
#endif

/* Values for "lockf" */
#ifndef F_UNLOCK
#define	F_UNLOCK		0
#define	F_LOCK			1
#define	F_TLOCK			2
#define	F_TEST			3
#endif

/* Standard file descriptors */
#define	STDIN_FILENO		0
#define	STDOUT_FILENO		1
#define	STDERR_FILENO		2

/* Function prototypes */
extern int access(const char *__pathname, int __mode);
extern unsigned int alarm(unsigned int __seconds);
extern int chdir(const char *__path);
extern int chown(const char *__path, uid_t __uid, gid_t __gid);
extern int close(int __fd);
extern size_t confstr(int __name, char *__buf, size_t __len);
extern char *crypt(const char *__key, const char *__salt);
extern char *ctermid(char *__s);
extern int dup(int __fd);
extern int dup2(int __oldfd, int __newfd);
extern void encrypt(char __block[64], int edflag);
extern int execl(const char *__path, const char *__arg, ...);
extern int execle(const char *__path, const char *__arg, ...);
extern int execlp(const char *__path, const char *__arg, ...);
extern int execv(const char *__path, char * const __argv[]);
extern int execve(const char *__path, char * const __argv[],
                  char * const __envp[]);
extern int execvp(const char *__path, char * const __argv[]);
extern void _exit(int __status);
extern int fchown(int __fd, uid_t __uid, gid_t __gid);
extern int fchdir(int __fd);
extern int fdatasync(int __fd);
extern pid_t fork(void);
extern long fpathconf(int __fd, int __name);
extern int fsync(int __fd);
extern int ftruncate(int __fd, off_t __length);
extern char *getcwd(char *__buf, size_t __size);
extern gid_t getegid(void);
extern uid_t geteuid(void);
extern gid_t getgid(void);
extern int getgroups(int __size, gid_t __list[]);
extern long gethostid(void);
extern int gethostname(char *__name, size_t __len);
extern char *getlogin(void);
extern int getlogin_r(char *__name, size_t __name_len);
extern int getopt(int argc, char * const argv[], const char *optstring);
extern pid_t getpgid(pid_t __pid);
extern pid_t getpid(void);
extern pid_t getpgrp(void);
extern pid_t getppid(void);
extern pid_t getsid(pid_t __pid);
extern uid_t getuid(void);
extern char *getwd(char *__buf);
extern int isatty(int __fd);
extern int lchown(const char *__path, uid_t __uid, gid_t __gid);
extern int link(const char *__oldpath, const char *__newpath);
extern int lockf(int __fd, int __cmd, off_t __len);
extern off_t lseek(int __fd, off_t __offset, int __whence);
extern off64_t lseek64(int __fd, off64_t __offset, int __whence);
extern int nice(int __inc);
extern long pathconf(char *__path, int __name);
extern int pause(void);
extern int pipe(int __fd[2]);
extern ssize_t pread(int __fd, void *__buf, size_t __count, off_t __offset);
extern ssize_t pread64(int __fd, void *__buf, size_t __count,
                       off64_t __offset);
extern ssize_t pwrite(int __fd, void *__buf, size_t __count, off_t __offset);
extern ssize_t pwrite64(int __fd, void *__buf, size_t __count,
                        off64_t __offset);
extern ssize_t read(int __fd, void *__buf, size_t __count);
extern ssize_t readlink(const char * __restrict __path,
                        char * __restrict __buf, size_t __bufsiz);
extern int rmdir(const char *__path);
extern int setegid(gid_t __gid);
extern int seteuid(uid_t __uid);
extern int setgid(gid_t __gid);
extern int setpgid(pid_t __pid, pid_t __pgid);
extern pid_t setpgrp(void);
extern int setregid(gid_t __rgid, gid_t __egid);
extern int setreuid(uid_t __ruid, uid_t __euid);
extern pid_t setsid(void);
extern int setuid(uid_t __uid);
extern unsigned sleep(unsigned __seconds);
extern void swab(const void * __restrict __from,
                 void * __restrict __to, ssize_t __n);
extern int symlink(const char *__oldpath, const char *__newpath);
extern void sync(void);
extern pid_t tcgetpgrp(int __fd);
extern int tcsetpgrp(int __fd, pid_t __pgrpid);
extern int truncate(const char *__path, off_t __length);
extern char *ttyname(int __fd);
extern int ttyname_r(int __fd, char *__buf, size_t __buflen);
extern useconds_t ualarm(useconds_t __value, useconds_t __interval);
extern int unlink(const char *__path);
extern int usleep(useconds_t __useconds);
extern pid_t vfork(void);
extern ssize_t write(int __fd, const void *__buf, size_t __count);

/* Global variables */
extern char *optarg;
extern int optind, opterr, optopt;

__END_DECLS

#endif  /* !_UNISTD_H */
