/*
 * redirect.c - Redirect math functions to the C# library, where possible.
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

#include <math.h>

__using__ System::Math;

/* Helper macros for redirecting C functions to the C# library */
#define	__MATHREDIR1(name,csname) \
float name##f(float value) \
{ \
  return (float)(Math::csname((double)value)); \
} \
double name(double value) \
{ \
  return Math::csname(value); \
} \
long double name##l(long double value) \
{ \
  return (long double)(Math::csname((double)value)); \
}
#define	__MATHREDIR2(name,csname) \
float name##f(float value1, float value2) \
{ \
  return (float)(Math::csname((double)value1, (double)value2)); \
} \
double name(double value1, double value2) \
{ \
  return Math::csname(value1, value2); \
} \
long double name##l(long double value1, long double value2) \
{ \
  return (long double)(Math::csname((double)value1, \
                                              (double)value2)); \
}

/* Redirect the C functions */
__MATHREDIR1(acos, Acos)
__MATHREDIR1(asin, Asin)
__MATHREDIR1(atan, Atan)
__MATHREDIR2(atan2, Atan2)
__MATHREDIR1(ceil, Ceiling)
__MATHREDIR1(cos, Cos)
__MATHREDIR1(cosh, Cosh)
__MATHREDIR1(exp, Exp)
__MATHREDIR1(floor, Floor)
__MATHREDIR1(log, Log)
__MATHREDIR1(log10, Log10)
__MATHREDIR2(pow, Pow)
__MATHREDIR2(remainder, IEEERemainder)
__MATHREDIR1(round, Round)
__MATHREDIR1(sin, Sin)
__MATHREDIR1(sinh, Sinh)
__MATHREDIR1(sqrt, Sqrt)
__MATHREDIR1(tan, Tan)
__MATHREDIR1(tanh, Tanh)
