/*
 * sys/sysmacros.h - Macros for manipulating device identifiers.
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

#ifndef _SYS_SYSMACROS_H
#define _SYS_SYSMACROS_H

/* These definitions keep Unix-like programs happy, even though
   we don't really have raw devices on this system */

#define major(dev)              ((int)(((dev) >> 8) & 0xFF))
#define minor(dev)              ((int)((dev) & 0xFF))
#define makedev(major,minor)    ((((unsigned int)(major)) << 8) \
                                  ((unsigned int)(minor)))

#endif /* !_SYS_SYSMACROS_H */
