/*
 * ieee754.h - Floating point bit field definitions.
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

#ifndef _IEEE754_H
#define _IEEE754_H

#include <features.h>

__BEGIN_DECLS

/* We don't define the actual bit fields because they aren't portable */

union ieee754_float
  {
    float f;
  };

union ieee754_double
  {
    double d;
  };

union ieee854_long_double
  {
    long double d;
  };

__END_DECLS

#endif  /* !_IEEE754_H */
