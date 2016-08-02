/*
 * unicode.c - Routines for manipulating Unicode characters and strings.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include "il_system.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Use small unicode tables if we are configured as Latin1 only.
 */
#ifdef IL_CONFIG_LATIN1
#ifndef	SMALL_UNICODE_TABLE
#define	SMALL_UNICODE_TABLE 1
#endif
#endif

/*
 * Include the Unicode category table from "unicat.c".
 * The table is automatically generated from "UnicodeData.txt"
 * using the "mkcategory" program.  The latest version of
 * the file can be found at the following URL:
 *
 *		ftp://ftp.unicode.org/Public/UNIDATA/UnicodeData.txt
 *
 * If the SMALL_UNICODE_TABLE macro is defined, then a smaller
 * table restricted to the Latin-1 subset is used instead of
 * the full table.  This is for memory-limited devices that
 * are likely to only be used within a Latin-1 locale.
 */
#include "unicat.c"

ILUnicodeCategory ILGetUnicodeCategory(unsigned ch)
{
#ifndef SMALL_UNICODE_TABLE
	/* The full Unicode category table is available */
	if(ch < (unsigned)0x10000)
	{
		return (ILUnicodeCategory)
					((charCategories[ch / 6] >> (5 * (ch % 6))) & 0x1F);
	}
	else
	{
		return ILUnicode_OtherNotAssigned;
	}
#else
	/* The restricted Latin-1 subset table is being used */
	if(ch < (unsigned)0x0100)
	{
		return (ILUnicodeCategory)
					((charCategories[ch / 6] >> (5 * (ch % 6))) & 0x1F);
	}

	/* Recognize some extra currency symbols, including the Euro,
	   which may be needed in some Latin-1 locales in Europe */
	if(ch >= (unsigned)0x20A0 && ch <= (unsigned)0x20AF)
	{
		return ILUnicode_CurrencySymbol;
	}

	/* Check for surrogates */
	if(ch >= (unsigned)0xD800 && ch <= (unsigned)0xDFFF)
	{
		return ILUnicode_Surrogate;
	}

	/* We have no idea what this is, so return OtherNotAssigned */
	return ILUnicode_OtherNotAssigned;
#endif
}

/*
 * Include the Unicode numeric value table from "uninum.c".
 * The table is automatically generated from "UnicodeData.txt"
 * using the "mknumber" program.
 */
#include "uninum.c"

double ILGetUnicodeValue(unsigned ch)
{
	int left, right, middle;
	left = 0;
	right = (sizeof(charValues) / sizeof(struct ILUniNumInfo)) - 1;
	while(left <= right)
	{
		middle = (left + right) / 2;
		if(charValues[middle].ch == ch)
		{
			return charValues[middle].value;
		}
		else if(charValues[middle].ch < ch)
		{
			left = middle + 1;
		}
		else
		{
			right = middle - 1;
		}
	}
	return -1.0;
}

int ILIsWhitespaceUnicode(unsigned ch)
{
#ifndef SMALL_UNICODE_TABLE
	if(ch < 0x0100)
	{
#endif
		/* Check for simple Latin-1 whitespace characters */
		return (ch == 0x0020 || ch == 0x0009 ||
				ch == 0x000A || ch == 0x000B ||
				ch == 0x000C || ch == 0x000D ||
				ch == 0x0085 || ch == 0x00A0);
#ifndef SMALL_UNICODE_TABLE
	}
	else
	{
		/* Check for general Unicode whitespace characters */
		return (ILGetUnicodeCategory(ch) == ILUnicode_SpaceSeparator);
	}
#endif
}

/*
 * Include the Unicode case conversion value table from "unicase.c".
 * The table is automatically generated from "UnicodeData.txt"
 * using the "mkcase" program.
 */
#include "unicase.c"

unsigned ILUnicodeCharToUpper(unsigned ch)
{
#ifdef SMALL_UNICODE_TABLE
	if(ch < 0x0100)
	{
		return unicodeToUpper[ch];
	}
	else
	{
		return ch;
	}
#else
	if(ch <= UNICASE_RANGE1_UPPER)
	{
		return unicodeToUpper[ch];
	}
	else if(ch >= UNICASE_RANGE2_LOWER && ch <= UNICASE_RANGE2_UPPER)
	{
		return unicodeToUpper
			[ch - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
	}
	else if(ch >= UNICASE_RANGE3_LOWER && ch <= UNICASE_RANGE3_UPPER)
	{
		return unicodeToUpper
			[ch - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
	}
	else
	{
		return ch;
	}
#endif
}

unsigned ILUnicodeCharToLower(unsigned ch)
{
#ifdef SMALL_UNICODE_TABLE
	if(ch < 0x0100)
	{
		return unicodeToLower[ch];
	}
	else
	{
		return ch;
	}
#else
	if(ch <= UNICASE_RANGE1_UPPER)
	{
		return unicodeToLower[ch];
	}
	else if(ch >= UNICASE_RANGE2_LOWER && ch <= UNICASE_RANGE2_UPPER)
	{
		return unicodeToLower
			[ch - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
	}
	else if(ch >= UNICASE_RANGE3_LOWER && ch <= UNICASE_RANGE3_UPPER)
	{
		return unicodeToLower
			[ch - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
	}
	else
	{
		return ch;
	}
#endif
}

void ILUnicodeStringToUpper(unsigned short *dest, const unsigned short *src,
					 		unsigned long len)
{
	unsigned ch;
	while(len > 0)
	{
		ch = *src++;
#ifdef SMALL_UNICODE_TABLE
		if(ch < 0x0100)
		{
			ch = unicodeToUpper[ch];
		}
#else
		if(ch <= UNICASE_RANGE1_UPPER)
		{
			ch = unicodeToUpper[ch];
		}
		else if(ch >= UNICASE_RANGE2_LOWER && ch <= UNICASE_RANGE2_UPPER)
		{
			ch = unicodeToUpper
				[ch - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
		}
		else if(ch >= UNICASE_RANGE3_LOWER && ch <= UNICASE_RANGE3_UPPER)
		{
			ch = unicodeToUpper
				[ch - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
		}
#endif
		*dest++ = (ILUInt16)ch;
		--len;
	}
}

void ILUnicodeStringToLower(unsigned short *dest, const unsigned short *src,
					 		unsigned long len)
{
	unsigned ch;
	while(len > 0)
	{
		ch = *src++;
#ifdef SMALL_UNICODE_TABLE
		if(ch < 0x0100)
		{
			ch = unicodeToLower[ch];
		}
#else
		if(ch <= UNICASE_RANGE1_UPPER)
		{
			ch = unicodeToLower[ch];
		}
		else if(ch >= UNICASE_RANGE2_LOWER && ch <= UNICASE_RANGE2_UPPER)
		{
			ch = unicodeToLower
				[ch - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
		}
		else if(ch >= UNICASE_RANGE3_LOWER && ch <= UNICASE_RANGE3_UPPER)
		{
			ch = unicodeToLower
				[ch - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
		}
#endif
		*dest++ = (ILUInt16)ch;
		--len;
	}
}

int ILUnicodeStringCompareIgnoreCase(const unsigned short *str1,
									 const unsigned short *str2,
									 unsigned long len)
{
	unsigned ch1;
	unsigned ch2;
	while(len > 0)
	{
		ch1 = *str1++;
#ifdef SMALL_UNICODE_TABLE
		if(ch1 < 0x0100)
		{
			ch1 = unicodeToLower[ch1];
		}
#else
		if(ch1 <= UNICASE_RANGE1_UPPER)
		{
			ch1 = unicodeToLower[ch1];
		}
		else if(ch1 >= UNICASE_RANGE2_LOWER && ch1 <= UNICASE_RANGE2_UPPER)
		{
			ch1 = unicodeToLower
				[ch1 - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
		}
		else if(ch1 >= UNICASE_RANGE3_LOWER && ch1 <= UNICASE_RANGE3_UPPER)
		{
			ch1 = unicodeToLower
				[ch1 - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
		}
#endif
		ch2 = *str2++;
#ifdef SMALL_UNICODE_TABLE
		if(ch2 < 0x0100)
		{
			ch2 = unicodeToLower[ch2];
		}
#else
		if(ch2 <= UNICASE_RANGE1_UPPER)
		{
			ch2 = unicodeToLower[ch2];
		}
		else if(ch2 >= UNICASE_RANGE2_LOWER && ch2 <= UNICASE_RANGE2_UPPER)
		{
			ch2 = unicodeToLower
				[ch2 - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
		}
		else if(ch2 >= UNICASE_RANGE3_LOWER && ch2 <= UNICASE_RANGE3_UPPER)
		{
			ch2 = unicodeToLower
				[ch2 - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
		}
#endif
		if(ch1 < ch2)
		{
			return -1;
		}
		else if(ch1 > ch2)
		{
			return 1;
		}
		--len;
	}
	return 0;
}

int ILUnicodeStringCompareNoIgnoreCase(const unsigned short *str1,
									   const unsigned short *str2,
									   unsigned long len)
{
	unsigned ch1;
	unsigned ch2;
	int uc1;
	int uc2;
	unsigned tc;
	while(len > 0)
	{
		ch1 = *str1++;
		uc1 = 0;
		ch2 = *str2++;
		uc2 = 0;
		if(ch1 != ch2)
		{
#ifdef SMALL_UNICODE_TABLE
			if(ch1 < 0x0100)
			{
				if(ch1 != unicodeToLower[ch1])
				{
					ch1 = unicodeToLower[ch1];
					uc1 = 1;
				}
			}
#else
			if(ch1 <= UNICASE_RANGE1_UPPER)
			{
				unsigned tc = unicodeToLower[ch1];
				if(tc != ch1)
				{
					ch1 = tc;
					uc1 = 1;
				}
			}
			else if(ch1 >= UNICASE_RANGE2_LOWER && ch1 <= UNICASE_RANGE2_UPPER)
			{
				unsigned tc = unicodeToLower
					[ch1 - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
				if(tc != ch1)
				{
					ch1 = tc;
					uc1 = 1;
				}
			}
			else if(ch1 >= UNICASE_RANGE3_LOWER && ch1 <= UNICASE_RANGE3_UPPER)
			{
				unsigned tc = unicodeToLower
					[ch1 - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
				if(tc != ch1)
				{
					ch1 = tc;
					uc1 = 1;
				}
			}
#endif
#ifdef SMALL_UNICODE_TABLE
			if(ch2 < 0x0100)
			{
				if(ch2 != unicodeToLower[ch2])
				{
					ch2 = unicodeToLower[ch2];
					uc2 = 1;
				}
			}
#else
			if(ch2 <= UNICASE_RANGE1_UPPER)
			{
				unsigned tc = unicodeToLower[ch2];
				if(tc != ch2)
				{
					ch2 = tc;
					uc2 = 1;
				}
			}
			else if(ch2 >= UNICASE_RANGE2_LOWER && ch2 <= UNICASE_RANGE2_UPPER)
			{
				unsigned tc = unicodeToLower
					[ch2 - UNICASE_RANGE2_LOWER + UNICASE_RANGE2_OFFSET];
				if(tc != ch2)
				{
					ch2 = tc;
					uc2 = 1;
				}
			}
			else if(ch2 >= UNICASE_RANGE3_LOWER && ch2 <= UNICASE_RANGE3_UPPER)
			{
				unsigned tc = unicodeToLower
					[ch2 - UNICASE_RANGE3_LOWER + UNICASE_RANGE3_OFFSET];
				if(tc != ch2)
				{
					ch2 = tc;
					uc2 = 1;
				}
			}
#endif
			if(ch1 < ch2)
			{
				return -1;
			}
			else if(ch1 > ch2)
			{
				return 1;
			}
			else
			{
				if(uc1 < uc2)
				{
					return -1;
				}
				else if(uc1 > uc2)
				{
					return 1;
				}
			}
		}
		--len;
	}
	return 0;
}


#ifdef	__cplusplus
};
#endif
