/*
 * time-defs.h - Useful definitions for handling C# <-> C time conversions.
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

#ifndef _TIME_DEFS_H
#define	_TIME_DEFS_H

extern long long __syscall_utc_time (void);
extern long long __syscall_local_time (void);
extern void __syscall_unpack_time (long long ticks, long tm, _Bool is_local);
extern void __syscall_sleep_ticks (long long ticks);

#define	EPOCH_ADJUST        62135596800LL
#define	TICKS_PER_SEC       10000000LL
#define	TICKS_PER_USEC      10LL
#define TICKS_PER_CLOCKS    10LL
#define NSECS_PER_TICK      100LL

#define	TIME_TO_TICKS(t)	(((t) + EPOCH_ADJUST) * TICKS_PER_SEC)

extern int __tz_is_set;
extern long long __startup_time;

#endif /* _TIME_DEFS_H */
