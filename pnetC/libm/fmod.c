/*
 * fmod.c - Compute the modulus of two floating-point values.
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

/* We need to do this with CIL assembly code, because there is
   no other way to access the runtime engine's "fmod" behaviour */

float
fmodf(float value1, float value2)
{
  __asm__ __volatile__ (
    "\tldarg.0\n"
	"\tldarg.1\n"
	"\trem\n"
	"\tconv.r4\n"
	"\tret\n"
  ::);
}

double
fmod(double value1, double value2)
{
  __asm__ __volatile__ (
    "\tldarg.0\n"
	"\tldarg.1\n"
	"\trem\n"
	"\tconv.r8\n"
	"\tret\n"
  ::);
}

long double
fmodl(long double value1, long double value2)
{
  __asm__ __volatile__ (
    "\tldarg.0\n"
	"\tldarg.1\n"
	"\trem\n"
	"\tret\n"
  ::);
}
