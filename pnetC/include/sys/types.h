/*
 * sys/types.h - Primitive system types.
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

#ifndef _SYS_TYPES_H
#define _SYS_TYPES_H

#include <features.h>
#include <stdint.h>

__BEGIN_DECLS

/* Define lots of useful types from a number of system variants */
typedef unsigned char       __u_char, u_char;
typedef unsigned short      __u_short, u_short, ushort;
typedef unsigned int        __u_int, u_int, uint;
typedef unsigned long       __u_long, u_long, ulong;
typedef uint64_t            __u_quad_t, u_quad_t;
typedef int64_t             __quad_t, quad_t;
typedef int8_t              __int8_t;
typedef uint8_t             __uint8_t, u_int8_t;
typedef int16_t             __int16_t;
typedef uint16_t            __uint16_t, u_int16_t;
typedef int32_t             __int32_t;
typedef uint32_t            __uint32_t, u_int32_t;
typedef int64_t             __int64_t;
typedef uint64_t            __uint64_t, u_int64_t;
typedef quad_t             *__qaddr_t;
typedef char               *__caddr_t, *caddr_t;
typedef const char         *__c_caddr_t, *c_caddr_t;
typedef volatile char      *__v_caddr_t, *v_caddr_t;
typedef int32_t             __daddr_t, daddr_t;
typedef uint32_t            __u_daddr_t, u_daddr_t;
typedef int64_t             __blkcnt_t, blkcnt_t, __blkcnt64_t, blkcnt64_t;
typedef long                __clock_t;
typedef int                 __clockid_t;
typedef uint32_t            __dev_t, dev_t, udev_t;
typedef uint64_t            __fsblkcnt_t, fsblkcnt_t;
typedef uint64_t            __fsblkcnt64_t, fsblkcnt64_t;
typedef uint64_t            __fsfilcnt_t, fsfilcnt_t;
typedef uint64_t            __fsfilcnt64_t, fsfilcnt64_t;
typedef uint32_t            __gid_t, gid_t;
typedef uint32_t            __id_t, id_t;
typedef uint32_t            __ino_t, ino_t;
typedef uint64_t            __ino64_t, ino64_t;
typedef uint16_t            __ipc_pid_t;
typedef long                __key_t, key_t;
typedef uint16_t            __mode_t, mode_t;
typedef uint16_t            __nlink_t, nlink_t;
typedef quad_t              __off_t, off_t, __off64_t, off64_t,
                            __loff_t, loff_t;
typedef int                 __pid_t, pid_t;
typedef int                 register_t;
typedef quad_t              __rlim_t, rlim_t, __rlim64_t;
typedef quad_t              __segsz_t, segsz_t;
typedef unsigned int        __size_t;
typedef unsigned int		__socklen_t;
typedef int                 __ssize_t;
typedef long                __swblk_t, swblk_t;
typedef long                __time_t;
typedef long                __t_scalar_t;
typedef unsigned long       __t_uscalar_t;
typedef int32_t             __ufs_daddr_t, ufs_daddr_t;
typedef uint32_t            __uid_t, uid_t;
typedef unsigned int        __useconds_t, useconds_t;
typedef long                __suseconds_t, suseconds_t;
#define __BIT_TYPES_DEFINED__   1

#if 0 /* TODO */
/* Definitions for file descriptor sets */
typedef unsigned long       __fd_mask, fd_mask;
#define NBBY                8
#ifndef FD_SETSIZE
#define FD_SETSIZE          1024
#endif
#define NFDBITS             (NBBY * sizeof(__fd_mask))
#define __FDELT(d)          ((d) / NFDBITS)
#define __FDMASK(d)         ((fd_mask)1 << ((d) % NFDBITS))
#ifndef howmany
#define howmany(a,b)        (((a) + (b) - 1) / (b))
#endif
typedef struct fd_set
{
    fd_mask fds_bits[howmany(FD_SETSIZE, NFDBITS)];

} __fd_set, fd_set;
#define __fds_bits          fds_bits
#define FD_ZERO(set)    \
        do { \
            unsigned int __posn; \
            fd_set *__set = (set); \
            for(__posn = 0; __posn < howmany(FD_SETSIZE, NFDBITS); ++__posn) \
                __set->fds_bits[__posn] = 0; \
        } while (0)
#define FD_SET(fd,set)      ((set)->fds_bits[__FDELT(fd)] |= __FDMASK(fd))
#define FD_CLR(fd,set)      ((set)->fds_bits[__FDELT(fd)] &= ~__FDMASK(fd))
#define FD_ISSET(fd,set)    ((set)->fds_bits[__FDELT(fd)] & __FDMASK(fd))
#define FD_COPY(from,to)    (*(to) = *(from))
#endif

/* Define specific types */
#ifndef ssize_t
typedef __ssize_t   ssize_t;
#define ssize_t     ssize_t
#endif
#ifndef size_t
typedef __size_t    size_t;
#define size_t      size_t
#endif
#ifndef time_t
typedef __time_t    time_t;
#define time_t      time_t
#endif
#ifndef clock_t
typedef __clock_t   clock_t;
#define clock_t     clock_t
#endif
#ifndef clockid_t
typedef __clockid_t clockid_t;
#define clockid_t   clockid_t
#endif

#include <sys/sysmacros.h>

__END_DECLS

#endif  /* _SYS_TYPES_H */
