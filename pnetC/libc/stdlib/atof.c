/*
 * atof.c - Convert a string into a floating-point value.
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

#include <stdlib.h>
#include <ctype.h>
#include <errno.h>

__using__ System::Single;
__using__ System::Double;
__using__ System::Runtime::InteropServices::Marshal;

static double
strtod_internal (const char *nptr, char **endptr, int report_errors)
{
  const char *current;
  const char *start;
  double value;

  /* Send the end pointer in case we need to bail out early */
  if (endptr)
    *endptr = (char *)nptr;

  /* Bail out if the value is NULL */
  if (!nptr)
    return 0.0;

  /* Skip white space to find the start of the number */
  current = nptr;
  while (*current != '\0' && isspace (*current & 0xFF))
    ++current;
  start = current;

  /* Verify that the number has a valid form */
  if (*current == '-' || *current == '+')
    ++current;
  if (*current >= '0' && *current <= '9')
    {
      while (*current >= '0' && *current <= '9')
        ++current;
    }
  else if (*current != '.')
    {
      return 0.0;
    }
  if (*current == '.')
    ++current;
  while (*current >= '0' && *current <= '9')
    ++current;
  if (*current == 'e' || *current == 'E')
    {
      ++current;
      if (*current == '-' || *current == '+')
        ++current;
      if (*current < '0' || *current > '9')
        return 0.0;
      while (*current >= '0' && *current <= '9')
        ++current;
    }

  /* Set the end pointer appropriately */
  if (endptr)
    *endptr = (char *)current;

  /* Use the C# library to convert the value into a double */
  value = Double::Parse
    (Marshal::PtrToStringAnsi ((long)start, (int)(current - start)));
  if (report_errors && Double::IsInfinity (value))
    errno = ERANGE;
  return value;
}

double
atof (const char *nptr)
{
  return strtod_internal (nptr, (char **)0, 0);
}

double
strtod (const char *nptr, char **endptr)
{
  return strtod_internal (nptr, endptr, 1);
}

float
strtof (const char *nptr, char **endptr)
{
  float value = (float)strtod_internal (nptr, endptr, 1);
  if (Single::IsInfinity (value))
    errno = ERANGE;
  return value;
}

long double
strtold (const char *nptr, char **endptr)
{
  return (long double)strtod_internal (nptr, endptr, 1);
}
