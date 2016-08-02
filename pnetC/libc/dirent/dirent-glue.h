/*
 * dirent-glue.h - Definitions that are used internally by dirent.
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

#ifndef _DIRENT_GLUE_H
#define _DIRENT_GLUE_H

#include <dirent.h>

struct __dirstream
  {
    void *gc_handle; /* handle for managed directory information */
    struct dirent current; /* storage for readdir() calls */
  };

__using__ System::String;
__using__ System::Runtime::InteropServices::Marshal;

extern void __syscall_closedir (void *gc_handle, void *err);
extern void *__syscall_opendir (String name, void *err);
extern void *__syscall_readdir(void *gc_handle, void *err);
extern void __syscall_rewinddir(void *gc_handle, void *err);
extern void __syscall_seekdir(void *gc_handle, long long pos);
extern long long __syscall_telldir(void *gc_handle);

#endif /* _DIRENT_GLUE_H */
