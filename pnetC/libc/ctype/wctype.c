/*
 * wctype.c - Wide character type testing.
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

#include <wctype.h>
#include <ctype.h>

/* In this implementation, the wide forms are the same as the normal forms */

int
iswalnum (wint_t c)
{
  return isalnum ((int)c);
}

int
iswalpha (wint_t c)
{
  return isalpha ((int)c);
}

int
iswcntrl (wint_t c)
{
  return iscntrl ((int)c);
}

int
iswdigit (wint_t c)
{
  return isdigit ((int)c);
}

int
iswlower (wint_t c)
{
  return islower ((int)c);
}

int
iswgraph (wint_t c)
{
  return isgraph ((int)c);
}

int
iswprint (wint_t c)
{
  return isprint ((int)c);
}

int
iswpunct (wint_t c)
{
  return ispunct ((int)c);
}

int
iswspace (wint_t c)
{
  return isspace ((int)c);
}

int
iswupper (wint_t c)
{
  return isupper ((int)c);
}

int
iswxdigit (wint_t c)
{
  return isxdigit ((int)c);
}

int
iswblank (wint_t c)
{
  return isblank ((int)c);
}

wint_t
towlower (wint_t c)
{
  return (wint_t)(tolower ((int)c));
}

wint_t
towupper (wint_t c)
{
  return (wint_t)(toupper ((int)c));
}
