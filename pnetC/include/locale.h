/*
 * locale.h - Locale information routines.
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

#ifndef	_LOCALE_H
#define	_LOCALE_H

#include <features.h>
#include <stddef.h>

__BEGIN_DECLS

/*
 * Locale categories.
 */
enum
  {
    __LC_CTYPE		= 0,
    __LC_NUMERIC	= 1,
    __LC_TIME		= 2,
    __LC_COLLATE	= 3,
    __LC_MONETARY	= 4,
    __LC_MESSAGES	= 5,
    __LC_ALL		= 6,
    __LC_PAPER		= 7,
    __LC_NAME		= 8,
    __LC_ADDRESS	= 9,
    __LC_TELEPHONE	= 10,
    __LC_MEASUREMENT	= 11,
    __LC_IDENTIFICATION	= 12
  };
#define LC_CTYPE          __LC_CTYPE
#define LC_NUMERIC        __LC_NUMERIC
#define LC_TIME           __LC_TIME
#define LC_COLLATE        __LC_COLLATE
#define LC_MONETARY       __LC_MONETARY
#define LC_MESSAGES       __LC_MESSAGES
#define	LC_ALL		  __LC_ALL
#define LC_PAPER	  __LC_PAPER
#define LC_NAME		  __LC_NAME
#define LC_ADDRESS	  __LC_ADDRESS
#define LC_TELEPHONE	  __LC_TELEPHONE
#define LC_MEASUREMENT	  __LC_MEASUREMENT
#define LC_IDENTIFICATION __LC_IDENTIFICATION

/*
 * Locale conversion information.
 */
struct lconv
  {
    char *decimal_point;
    char *thousands_sep;
    char *grouping;
    char *int_curr_symbol;
    char *currency_symbol;
    char *mon_decimal_point;
    char *mon_thousands_sep;
    char *mon_grouping;
    char *positive_sign;
    char *negative_sign;
    char int_frac_digits;
    char frac_digits;
    char p_cs_precedes;
    char p_sep_by_space;
    char n_cs_precedes;
    char n_sep_by_space;
    char p_sign_posn;
    char n_sign_posn;
    char int_p_cs_precedes;
    char int_p_sep_by_space;
    char int_n_cs_precedes;
    char int_n_sep_by_space;
    char int_p_sign_posn;
    char int_n_sign_posn;
  };

/*
 * Function prototypes.
 */
extern char *setlocale (int __category, const char *__locale);
extern struct lconv *localeconv (void);

__END_DECLS

#endif /* _LOCALE_H */
