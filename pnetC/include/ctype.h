/*
 * ctype.h - Character type testing.
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

#ifndef _CTYPE_H
#define _CTYPE_H

#include <features.h>
#include <sys/types.h>

__BEGIN_DECLS

extern int isalnum (int __c);
extern int isalpha (int __c);
extern int iscntrl (int __c);
extern int isdigit (int __c);
extern int islower (int __c);
extern int isgraph (int __c);
extern int isprint (int __c);
extern int ispunct (int __c);
extern int isspace (int __c);
extern int isupper (int __c);
extern int isxdigit (int __c);
extern int isblank (int __c);
extern int tolower (int __c);
extern int _tolower (int __c);
extern int toupper (int __c);
extern int _toupper (int __c);
extern int isascii (int __c);
extern int toascii (int __c);

__END_DECLS

#endif  /* !_CTYPE_H */
