/*
 * atoi.c - Convert a string into an integer value.
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
#include <stdint.h>
#include <ctype.h>
#include <errno.h>

/*
 * Compute "a * base + b" while detecting 64-bit overflow.
 */
static unsigned long long
long_mult_add (unsigned long long a, int base, int b, int *overflow)
{
  unsigned long long result;
  unsigned long long temp;
  if ((base & 1) != 0)
    result = a;
  else
    result = 0;
  base >>= 1;
  while (base != 0)
    {
      temp = a + a;
      if (temp < a)
	{
	  *overflow = 1;
	  return UINT64_MAX;
	}
      a = temp;
      if ((base & 1) != 0)
	{
	  result += a;
	  if (result < a)
	    {
	      *overflow = 1;
	      return UINT64_MAX;
	    }
	}
      base >>= 1;
    }
  a = (unsigned long long)(long long)b;
  result += a;
  if (result < a)
    {
      *overflow = 1;
      return UINT64_MAX;
    }
  return result;
}

/*
 * Inner conversion routine, which returns an unsigned value plus
 * a negative indicator to be processed further by callers.
 */
static unsigned long long
strtoul_inner (const char *nptr, char **endptr, int base,
	       int *isneg, int *overflow)
{
  const char *current;
  unsigned long long value = 0;
  int numdigits, digit;

  /* Clear important return values before we start */
  if (endptr)
    *endptr = (char *)nptr;
  *isneg = 0;
  *overflow = 0;

  /* Bail out if the value is NULL */
  if (!nptr)
    return 0;

  /* Skip white space to find the start of the number */
  current = nptr;
  while (*current != '\0' && isspace (*current & 0xFF))
    ++current;

  /* Extract the sign and then determine the number's base */
  if (*current == '-')
    {
      *isneg = 1;
      ++current;
    }
  else if (*current == '+')
    ++current;
  if ((base == 16 || base == 0) && current[0] == 'x' &&
      (current[1] == 'x' || current[1] == 'X'))
    {
      base = 16;
      current += 2;
    }
  else if (base == 0 && current[0] == '0')
    base = 8;
  else if (base == 0)
    base = 10;
  if (base < 2 || base > 36)
    {
      errno = EINVAL;
      return 0;
    }

  /* Parse the digits */
  numdigits = 0;
  while (*current != '\0')
    {
      if (*current >= '0' && *current <= '9')
        digit = *current - '0';
      else if (*current >= 'A' && *current <= 'Z')
        digit = *current - 'A' + 10;
      else if (*current >= 'a' && *current <= 'z')
        digit = *current - 'a' + 10;
      else
        break;
      if (digit >= base)
        break;
      ++current;
      ++numdigits;
      value = long_mult_add (value, base, digit, overflow);
    }
  if (!numdigits)
    return 0;

  /* Set the end pointer appropriately */
  if (endptr)
    *endptr = (char *)current;
  return value;
}

static long long
strtoll_internal (const char *nptr, char **endptr, int base, int limit_range)
{
  int isneg, overflow;
  unsigned long long value;
  value = strtoul_inner (nptr, endptr, base, &isneg, &overflow);
  if (limit_range)
    {
      /* Range-check a signed 32-bit value */
      if (isneg)
        {
	  if (value > (((unsigned long long)1) << 31))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 31);
	  value = (unsigned long long)(-((long long)value));
	}
      else
        {
	  if (value >= (((unsigned long long)1) << 31))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 31) - 1;
	}
    }
  else
    {
      /* Range-check a signed 64-bit value */
      if (isneg)
        {
	  if (value > (((unsigned long long)1) << 63))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 63);
	  value = (unsigned long long)(-((long long)value));
	}
      else
        {
	  if (value >= (((unsigned long long)1) << 63))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 63) - 1;
	}
    }
  if (overflow)
    errno = ERANGE;
  return (long long)value;
}

static unsigned long long
strtoull_internal (const char *nptr, char **endptr, int base, int limit_range)
{
  int isneg, overflow;
  unsigned long long value;
  value = strtoul_inner (nptr, endptr, base, &isneg, &overflow);
  if (limit_range)
    {
      /* Range-check an unsigned 32-bit value */
      if (isneg)
        {
	  if (value > (((unsigned long long)1) << 31))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 31);
	  value = (unsigned long long)(-((long long)value));
	}
      else
        {
	  if (value > (((unsigned long long)1) << 32))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 32) - 1;
	}
    }
  else
    {
      /* Range-check an unsigned 64-bit value */
      if (isneg)
        {
	  if (value > (((unsigned long long)1) << 63))
	    overflow = 1;
	  if (overflow)
	    value = (((unsigned long long)1) << 63);
	  value = (unsigned long long)(-((long long)value));
	}
      else
        {
	  if (overflow)
	    value = UINT64_MAX;
	}
    }
  if (overflow)
    errno = ERANGE;
  return value;
}

int
atoi (const char *nptr)
{
  return (int)strtol (nptr, (char **)0, 10);
}

long
atol (const char *nptr)
{
  return strtol (nptr, (char **)0, 10);
}

long long
atoll (const char *nptr)
{
  return strtoll (nptr, (char **)0, 10);
}
weak_alias (atoll, atoq)

long
strtol (const char *nptr, char **endptr, int base)
{
  if (sizeof(long) == 8)
    return (long)strtoll_internal (nptr, endptr, base, 0);
  else
    return (long)strtoll_internal (nptr, endptr, base, 1);
}

long long
strtoll (const char *nptr, char **endptr, int base)
{
  return strtoll_internal (nptr, endptr, base, 0);
}
weak_alias (strtoll, strtoq)

unsigned long
strtoul (const char *nptr, char **endptr, int base)
{
  if (sizeof(long) == 8)
    return (unsigned long)strtoull_internal (nptr, endptr, base, 0);
  else
    return (unsigned long)strtoull_internal (nptr, endptr, base, 1);
}

unsigned long long
strtoull (const char *nptr, char **endptr, int base)
{
  return strtoull_internal (nptr, endptr, base, 0);
}
weak_alias (strtoull, strtouq)
