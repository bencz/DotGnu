/*
 * inttypes.h - Standard integer types.
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

#ifndef _INTTYPES_H
#define _INTTYPES_H

#include <features.h>
#include <stdint.h>

__BEGIN_DECLS

typedef struct
{
    intmax_t    quot;
    intmax_t    rem;

} imaxdiv_t;

extern imaxdiv_t imaxdiv(intmax_t __numer, intmax_t __denom);
extern intmax_t  imaxabs(intmax_t __j);
extern intmax_t  strtoimax(const char * __restrict __nptr,
                           char ** __restrict __endptr, int __base);
extern uintmax_t strtoumax(const char * __restrict __nptr,
                           char ** __restrict __endptr, int __base);
extern intmax_t  wcstoimax(const wchar_t * __restrict __nptr,
                           char ** __restrict __endptr, int __base);
extern uintmax_t wcstoumax(const wchar_t * __restrict __nptr,
                           char ** __restrict __endptr, int __base);

#define PRId8           "d"
#define PRId16          "d"
#define PRId32          "d"
#define PRId64          "lld"
#define PRIdMAX         "lld"

#define	PRIdLEAST8      "d"
#define	PRIdLEAST16     "d"
#define	PRIdLEAST32     "d"
#define	PRIdLEAST64     "lld"

#define	PRIdFAST8       "d"
#define	PRIdFAST16      "d"
#define	PRIdFAST32      "d"
#define	PRIdFAST64      "lld"

#define PRIi8           "i"
#define PRIi16          "i"
#define PRIi32          "i"
#define PRIi64          "lli"
#define PRIiMAX         "lli"

#define	PRIiLEAST8      "i"
#define	PRIiLEAST16     "i"
#define	PRIiLEAST32     "i"
#define	PRIiLEAST64     "lli"

#define	PRIiFAST8       "i"
#define	PRIiFAST16      "i"
#define	PRIiFAST32      "i"
#define	PRIiFAST64      "lli"

#define PRIo8           "o"
#define PRIo16          "o"
#define PRIo32          "o"
#define PRIo64          "llo"
#define PRIoMAX         "llo"

#define	PRIoLEAST8      "o"
#define	PRIoLEAST16     "o"
#define	PRIoLEAST32     "o"
#define	PRIoLEAST64     "llo"

#define	PRIoFAST8       "o"
#define	PRIoFAST16      "o"
#define	PRIoFAST32      "o"
#define	PRIoFAST64      "llo"

#define PRIu8           "u"
#define PRIu16          "u"
#define PRIu32          "u"
#define PRIu64          "llu"
#define PRIuMAX         "llu"

#define	PRIuLEAST8      "u"
#define	PRIuLEAST16     "u"
#define	PRIuLEAST32     "u"
#define	PRIuLEAST64     "llu"

#define	PRIuFAST8       "u"
#define	PRIuFAST16      "u"
#define	PRIuFAST32      "u"
#define	PRIuFAST64      "llu"

#define PRIx8           "x"
#define PRIx16          "x"
#define PRIx32          "x"
#define PRIx64          "llx"
#define PRIxMAX         "llx"

#define	PRIxLEAST8      "x"
#define	PRIxLEAST16     "x"
#define	PRIxLEAST32     "x"
#define	PRIxLEAST64     "llx"

#define	PRIxFAST8       "x"
#define	PRIxFAST16      "x"
#define	PRIxFAST32      "x"
#define	PRIxFAST64      "llx"

#define PRIX8           "X"
#define PRIX16          "X"
#define PRIX32          "X"
#define PRIX64          "llX"
#define PRIXMAX         "llX"

#define	PRIXLEAST8      "X"
#define	PRIXLEAST16     "X"
#define	PRIXLEAST32     "X"
#define	PRIXLEAST64     "llX"

#define	PRIXFAST8       "X"
#define	PRIXFAST16      "X"
#define	PRIXFAST32      "X"
#define	PRIXFAST64      "llX"

#define PRIdPTR         "ld"
#define PRIiPTR         "li"
#define PRIoPTR         "lo"
#define PRIuPTR         "lu"
#define PRIxPTR         "lx"
#define PRIXPTR         "lX"

__END_DECLS

#endif  /* !_INTTYPES_H */
