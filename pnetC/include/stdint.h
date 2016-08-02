/*
 * stdint.h - Standard integer types.
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

#ifndef _STDINT_H
#define _STDINT_H

#include <features.h>
#define __need_wchar_t
#include <stddef.h>

typedef signed char         int8_t;
typedef short               int16_t;
typedef int                 int32_t;
typedef long long           int64_t;
typedef unsigned char       uint8_t;
typedef unsigned short      uint16_t;
typedef unsigned int        uint32_t;
typedef unsigned long long  uint64_t;

typedef signed char         int_least8_t;
typedef short               int_least16_t;
typedef int                 int_least32_t;
typedef long long           int_least64_t;
typedef unsigned char       uint_least8_t;
typedef unsigned short      uint_least16_t;
typedef unsigned int        uint_least32_t;
typedef unsigned long long  uint_least64_t;

typedef signed char         int_fast8_t;
typedef int                 int_fast16_t;
typedef int                 int_fast32_t;
typedef long long           int_fast64_t;
typedef unsigned char       uint_fast8_t;
typedef unsigned int        uint_fast16_t;
typedef unsigned int        uint_fast32_t;
typedef unsigned long long  uint_fast64_t;

typedef long 				intptr_t;
typedef unsigned long 		uintptr_t;

typedef long long           intmax_t;
typedef unsigned long long  uintmax_t;

#if !defined(__cplusplus) || defined(__STDC_LIMIT_MACROS)

#define INT8_MIN            (-128)
#define INT8_MAX            (127)
#define INT16_MIN           (-32768)
#define INT16_MAX           (32767)
#define INT32_MIN           (-2147483647-1)
#define INT32_MAX           (2147483647)
#define INT64_MIN           (-9223372036854775807LL-1)
#define INT64_MAX           (9223372036854775807LL)
#define UINT8_MAX           (255U)
#define UINT16_MAX          (65536U)
#define UINT32_MAX          (4294967295U)
#define UINT64_MAX          (18446744073709551615ULL)

#define INT_LEAST8_MIN      (-128)
#define INT_LEAST8_MAX      (127)
#define INT_LEAST16_MIN     (-32768)
#define INT_LEAST16_MAX     (32767)
#define INT_LEAST32_MIN     (-2147483647-1)
#define INT_LEAST32_MAX     (2147483647)
#define INT_LEAST64_MIN     (-9223372036854775807LL-1)
#define INT_LEAST64_MAX     (9223372036854775807LL)
#define UINT_LEAST8_MAX     (255U)
#define UINT_LEAST16_MAX    (65536U)
#define UINT_LEAST32_MAX    (4294967295U)
#define UINT_LEAST64_MAX    (18446744073709551615ULL)

#define INT_FAST8_MIN       (-128)
#define INT_FAST8_MAX       (127)
#define INT_FAST16_MIN      (-2147483647-1)
#define INT_FAST16_MAX      (2147483647)
#define INT_FAST32_MIN      (-2147483647-1)
#define INT_FAST32_MAX      (2147483647)
#define INT_FAST64_MIN      (-9223372036854775807LL-1)
#define INT_FAST64_MAX      (9223372036854775807LL)
#define UINT_FAST8_MAX      (255U)
#define UINT_FAST16_MAX     (4294967295U)
#define UINT_FAST32_MAX     (4294967295U)
#define UINT_FAST64_MAX     (18446744073709551615ULL)

#define	INTPTR_MAX			((long)((~((unsigned long)0)) >> 1))
#define	INTPTR_MIN			(-INTPTR_MAX - (long)1)
#define	UINTPTR_MAX			(~((unsigned long)0))

#define INTMAX_MIN          (-9223372036854775807LL-1)
#define INTMAX_MAX          (9223372036854775807LL)
#define UINTMAX_MAX         (18446744073709551615ULL)

#define PTRDIFF_MIN         INTPTR_MIN
#define PTRDIFF_MAX         INTPTR_MAX

#define SIG_ATOMIC_MIN      (-2147483647-1)
#define SIG_ATOMIC_MAX      (2147483647)

#define SIZE_MAX            (4294967295U)

#ifndef WCHAR_MIN
#define WCHAR_MIN           ((__wchar__)0)
#define WCHAR_MAX           ((__wchar__)65535)
#endif

#define WINT_MIN            (0U)
#define WINT_MAX            (4294967295U)

#endif

#if !defined(__cplusplus) || defined(__STDC_CONSTANT_MACROS)

#define INT8_C(con)         con
#define UINT8_C(con)        con##U
#define INT16_C(con)        con
#define UINT16_C(con)       con##U
#define INT32_C(con)        con
#define UINT32_C(con)       con##U
#define INT64_C(con)        con##LL
#define UINT64_C(con)       con##ULL
#define INTMAX_C(con)       con##LL
#define UINTMAX_C(con)      con##ULL

#endif

#endif  /* !_STDINT_H */
