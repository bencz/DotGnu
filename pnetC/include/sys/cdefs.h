/*
 * sys/cdefs.h - Common definitions for header file support.
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

#ifndef	_SYS_CDEFS_H
#define	_SYS_CDEFS_H

#ifndef	_FEATURES_H
#include <features.h>
#endif

#ifdef	__cplusplus
#define	__BEGIN_DECLS	extern "C" {
#define	__END_DECLS		}
#else
#define	__BEGIN_DECLS
#define	__END_DECLS
#endif

#define	__THROW

#undef	__P
#undef	__PMT
#define	__P(x)			x
#define	__PMT(x)		x

#define	__inline
#define	__const			__const__
#define	__signed		__signed__
#define	__volatile		__volatile__
#define	__bounded
#define	__unbounded
#define	__ptrvalue
#define	__restrict
#define	__restrict_arr

#define	__CONCAT(x,y)	x ## y
#define	__STRING(x)		#x

#define	__ptr_t			void *
#define	__long_double_t	long double

#define	__flexarr		[0]

#define	__attribute_malloc__
#define	__attribute_pure__
#define	__attribute_format_arg__(x)
#define	__attribute_format_strfmon__(a,b)
#define	__extension__
#define	__printf__		"printf"

#endif	/* !_SYS_CDEFS_H */
