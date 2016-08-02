/*
 * mman-stub.c - Stub out <sys/mman.h> functions.
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

#include <unistd.h>
#include <sys/mman.h>
#include <errno.h>

void *
__mmap (void *addr, size_t len, int prot, int flags, int fd, off_t offset)
{
  errno = ENOSYS;
  return MAP_FAILED;
}

void *
__mmap64 (void *addr, size_t len, int prot, int flags, int fd, off64_t offset)
{
  errno = ENOSYS;
  return MAP_FAILED;
}

int
__munmap (void *addr, size_t len)
{
  errno = ENOSYS;
  return -1;
}

int
__mprotect (void *addr, size_t len, int prot)
{
  errno = ENOSYS;
  return -1;
}

int
msync (void *addr, size_t len, int flags)
{
  errno = ENOSYS;
  return -1;
}

int
madvise (void *addr, size_t len, int advice)
{
  errno = ENOSYS;
  return -1;
}

int
mlock (const void *addr, size_t len)
{
  errno = ENOSYS;
  return -1;
}

int
munlock (const void *addr, size_t len)
{
  errno = ENOSYS;
  return -1;
}

int
mlockall (int flags)
{
  errno = ENOSYS;
  return -1;
}

int
munlockall (void)
{
  errno = ENOSYS;
  return -1;
}

void *
__mremap (void *addr, size_t old_len, size_t new_len, int may_move)
{
  errno = ENOSYS;
  return MAP_FAILED;
}

int
mincore (void *start, size_t len, unsigned char *vec)
{
  errno = ENOSYS;
  return -1;
}

int
shm_open (const char *name, int oflag, mode_t mode)
{
  errno = ENOSYS;
  return -1;
}

int
shm_unlink (const char *name)
{
  errno = ENOSYS;
  return -1;
}

weak_alias (__mmap, mmap)
weak_alias (__mmap64, mmap64)
weak_alias (__munmap, munmap)
weak_alias (__mprotect, mprotect)
weak_alias (__mremap, mremap)
weak_alias (madvise, posix_madvise)
