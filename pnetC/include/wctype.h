/*
 * wctype.h - Wide character type testing.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#ifndef _WCTYPE_H
#define _WCTYPE_H

#include <features.h>
#include <sys/types.h>
#include <stddef.h>

__BEGIN_DECLS

#ifndef WEOF
#define WEOF ((wint_t)0xFFFFFFFF)
#endif

typedef unsigned long wctype_t;
typedef __const int *wctrans_t;

extern int iswalnum (wint_t __c);
extern int iswalpha (wint_t __c);
extern int iswcntrl (wint_t __c);
extern int iswdigit (wint_t __c);
extern int iswlower (wint_t __c);
extern int iswgraph (wint_t __c);
extern int iswprint (wint_t __c);
extern int iswpunct (wint_t __c);
extern int iswspace (wint_t __c);
extern int iswupper (wint_t __c);
extern int iswxdigit (wint_t __c);
extern int iswblank (wint_t __c);
extern wint_t towlower (wint_t __c);
extern wint_t towupper (wint_t __c);

extern wctype_t wctype (__const char *__property);
extern int iswctype (wint_t __wc, wctype_t __desc);

extern wctrans_t wctrans (__const char *__property);
extern wint_t towctrans (wint_t __wc, wctrans_t __desc);

__END_DECLS

#endif  /* !_WCTYPE_H */
