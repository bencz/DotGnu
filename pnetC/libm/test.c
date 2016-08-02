/*
 * test.c - Value testing functions.
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

#undef isfinite
#undef isinf
#undef isnan

__using__ System::Single;
__using__ System::Double;

#define	LD_POS_INF	((long double)(1.0 / 0.0))
#define	LD_NEG_INF	((long double)(-1.0 / 0.0))

int
__isfinitef(float value)
{
  return !Single::IsNaN(value) &&
         !Single::IsInfinity(value);
}

int
__isfinite(double value)
{
  return !Double::IsNaN(value) &&
         !Double::IsInfinity(value);
}

int
__isfinitel(long double value)
{
  return (value == value && value != LD_POS_INF && value != LD_NEG_INF);
}

int
__isinff(float value)
{
  return Single::IsInfinity(value);
}

int
__isinf(double value)
{
  return Double::IsInfinity(value);
}

int
__isinfl(long double value)
{
  return (value == LD_POS_INF || value == LD_NEG_INF);
}

int
__isnanf(float value)
{
  return Single::IsNaN(value);
}

int
__isnan(double value)
{
  return Double::IsNaN(value);
}

int
__isnanl(long double value)
{
  return (value != value);
}

strong_alias(__isfinite, isfinite)
strong_alias(__isinf, isinf)
strong_alias(__isnan, isnan)
