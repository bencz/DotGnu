/*
 * ctype.c - Character type testing.
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

#include <ctype.h>

/* Use the C# class library to do character type testing */
__using__ System::Char;

/* This definition must match "System.Globalization.UnicodeCategory" */
typedef enum
{
  UppercaseLetter = 0,
  LowercaseLetter = 1,
  TitlecaseLetter = 2,
  ModifierLetter = 3,
  OtherLetter = 4,
  NonSpacingMark = 5,
  SpacingCombiningMark = 6,
  EnclosingMark = 7,
  DecimalDigitNumber = 8,
  LetterNumber = 9,
  OtherNumber = 10,
  SpaceSeparator = 11,
  LineSeparator = 12,
  ParagraphSeparator = 13,
  Control = 14,
  Format = 15,
  Surrogate = 16,
  PrivateUse = 17,
  ConnectorPunctuation = 18,
  DashPunctuation = 19,
  OpenPunctuation = 20,
  ClosePunctuation = 21,
  InitialQuotePunctuation = 22,
  FinalQuotePunctuation = 23,
  OtherPunctuation = 24,
  MathSymbol = 25,
  CurrencySymbol = 26,
  ModifierSymbol = 27,
  OtherSymbol = 28,
  OtherNotAssigned = 29

} UnicodeCategory;

/* Get the raw Unicode category from the C# class library */
static UnicodeCategory
get_category (int c)
{
  if (c >= 0 && c < 65536)
    {
      return (UnicodeCategory)(Char::GetUnicodeCategory ((__wchar__)c));
    }
  else
    {
      return OtherNotAssigned;
    }
}

int
isalnum (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == UppercaseLetter ||
          category == LowercaseLetter ||
          category == TitlecaseLetter ||
          category == ModifierLetter ||
          category == OtherLetter ||
          category == DecimalDigitNumber ||
          category == LetterNumber ||
          category == OtherNumber);
}

int
isalpha (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == UppercaseLetter ||
          category == LowercaseLetter ||
          category == TitlecaseLetter ||
          category == ModifierLetter ||
          category == OtherLetter);
}

int
iscntrl (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == Control);
}

int
isdigit (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == DecimalDigitNumber);
}

int
islower (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == LowercaseLetter);
}

int
isgraph (int c)
{
  UnicodeCategory category = get_category (c);
  return (category != Control && c != ' ');
}

int
isprint (int c)
{
  UnicodeCategory category = get_category (c);
  return (category != Control);
}

int
ispunct (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == ConnectorPunctuation ||
          category == DashPunctuation ||
          category == OpenPunctuation ||
          category == ClosePunctuation ||
          category == InitialQuotePunctuation ||
          category == FinalQuotePunctuation ||
          category == OtherPunctuation);
}

int
isspace (int c)
{
  if (c == 0x0009 || c == 0x000a || c == 0x000b ||
      c == 0x000c || c == 0x000d || c == 0x0085 ||
      c == 0x2028 || c == 0x2029)
  {
    return 1;
  }
  return (get_category (c) == SpaceSeparator);
}

int
isupper (int c)
{
  UnicodeCategory category = get_category (c);
  return (category == UppercaseLetter);
}

int
isxdigit (int c)
{
  if (c >= '0' && c <= '9')
    {
      return 1;
    }
  if (c >= 'A' && c <= 'F')
    {
      return 1;
    }
  if (c >= 'a' && c <= 'f')
    {
      return 1;
    }
  return 0;
}

int
isblank (int c)
{
  return (c == ' ' || c == '\t');
}

int
tolower (int c)
{
  if (c >= 0 && c < 65535)
    {
      return (int)(Char::ToLower ((__wchar__)c));
    }
  else
    {
      return c;
    }
}

int
_tolower (int c)
{
  return tolower (c);
}

int
toupper (int c)
{
  if (c >= 0 && c < 65535)
    {
      return (int)(Char::ToUpper ((__wchar__)c));
    }
  else
    {
      return c;
    }
}

int
_toupper (int c)
{
  return toupper (c);
}

int
isascii (int c)
{
  return ((c & ~0x7F) == 0);
}

int
toascii (int c)
{
  return (c & 0x7F);
}
