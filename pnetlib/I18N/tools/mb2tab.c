/*
 * mb2tab.c - Convert multi-byte data files into CJK conversion tables.
 *
 * Copyright (c) 2003  Southern Storm Software, Pty Ltd
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

/*

Usage: mb2tab input output

Example:

	mb2tab [-y] ksc5601.txt ksc.table

where "-y" indicates that we need to add the Yen and overline mappings
for JIS-based locales.

Get the files from ftp://ftp.unicode.org/Public/MAPPINGS/OBSOLETE/EASTASIA
                or ftp://ftp.unicode.org/Public/VENDORS/MICSFT

*/

#include <stdio.h>
#include <string.h>
#include <stdlib.h>

/*
 * Forward declarations.
 */
static void initTables(int yen);
static void convertLine(char *buf);
static int createTables(const char *filename);

int main(int argc, char *argv[])
{
	FILE *file;
	char buffer[BUFSIZ];
	int error;
	int yen = 0;

	/* Validate the parameters */
	if(argc > 1 && !strcmp(argv[1], "-y"))
	{
		yen = 1;
		++argv;
		--argc;
	}
	if(argc != 3)
	{
		fprintf(stderr, "Usage: mb2tab input output\n");
		return 1;
	}

	/* Initialize the tables */
	initTables(yen);

	/* Load the relevant contents from the input file */
	if((file = fopen(argv[1], "r")) == NULL)
	{
		perror(argv[1]);
		return 1;
	}
	while(fgets(buffer, sizeof(buffer), file))
	{
		if(buffer[0] == '0' && buffer[1] == 'x')
		{
			convertLine(buffer);
		}
	}
	fclose(file);

	/* Create the output tables */
	error = createTables(argv[2]);

	/* Clean up and exit */
	return error;
}

/*
 * Parse a hexadecimal value.  Returns the length
 * of the value that was parsed.
 */
static int parseHex(const char *buf, unsigned long *value)
{
	int len = 0;
	char ch;
	*value = 0;
	while((ch = buf[len]) != '\0')
	{
		if(ch >= '0' && ch <= '9')
		{
			*value = *value * 16 + (unsigned long)(ch - '0');
		}
		else if(ch >= 'A' && ch <= 'F')
		{
			*value = *value * 16 + (unsigned long)(ch - 'A' + 10);
		}
		else if(ch >= 'a' && ch <= 'f')
		{
			*value = *value * 16 + (unsigned long)(ch - 'a' + 10);
		}
		else
		{
			break;
		}
		++len;
	}
	return len;
}

/*
 * Tables.
 */
static unsigned short dbyteToUnicode[65535];
static int lowFirst, highFirst, lowSecond, highSecond;
static unsigned short unicodeToDByte[65536];
static int lowRangeUpper, midRangeLower, midRangeUpper, highRangeLower;

/*
 * Initialize the tables.
 */
static void initTables(int yen)
{
	int posn;
	lowFirst = 256;
	highFirst = -1;
	lowSecond = 256;
	highSecond = -1;
	lowRangeUpper = 127;
	midRangeLower = 0xF000;
	midRangeUpper = 0x1000;
	highRangeLower = 0x10000;
	for(posn = 0; posn < 128; ++posn)
	{
		unicodeToDByte[posn] = (unsigned short)posn;
	}
	if(yen)
	{
		unicodeToDByte[0xA5] = 0x5C;
		unicodeToDByte[0xA6] = 0x7C;
		lowRangeUpper = 0xA6;
	}
}

/*
 * Convert an input line into table entries.
 */
static void convertLine(char *buf)
{
	unsigned long dbyte;
	unsigned long code;
	int first, second;

	/* Parse the hex name of the dbyte character */
	buf += 2;
	buf += parseHex(buf, &dbyte);

	/* Skip to the Unicode character name */
	while(*buf != '\0' && *buf != '0' && *buf != '#')
	{
		++buf;
	}
	if(*buf != '0')
	{
		return;
	}
	if(dbyte >= 0x10000)
	{
		/* Cannot handle three-byte forms yet */
		return;
	}

	/* Parse the hex name of the Unicode character */
	buf += 2;
	buf += parseHex(buf, &code);
	if(code >= 0x10000)
	{
		/* Cannot handle surrogate-based CJK characters yet */
		return;
	}

	/* Update the mapping tables */
	if(dbyte >= 0x2000 && dbyte < 0x8000)
	{
		/* Convert JIS form into EUC form */
		dbyte += 0x8080;
	}
	if(dbyte <= 0x0100)
	{
		/* Single-byte character encoding */
		if(unicodeToDByte[code] == 0)
		{
			unicodeToDByte[code] = dbyte;
		}
		unicodeToDByte[code] = dbyte;
	}
	else
	{
		/* Double-byte character encoding */
		dbyteToUnicode[dbyte] = code;
		if(unicodeToDByte[code] == 0)
		{
			unicodeToDByte[code] = dbyte;
		}
		first = (int)(dbyte / 256);
		second = (int)(dbyte % 256);
		if(first < lowFirst)
		{
			lowFirst = first;
		}
		if(first > highFirst)
		{
			highFirst = first;
		}
		if(second < lowSecond)
		{
			lowSecond = second;
		}
		if(second > highSecond)
		{
			highSecond = second;
		}
	}

	/* Update the extent of the code ranges */
	if(code < 0x1000)
	{
		/* Latin, Greek, Cyrillic, or symbol character range */
		if(code > lowRangeUpper)
		{
			lowRangeUpper = code;
		}
	}
	else if(code < 0xF000)
	{
		/* CJK charcter range */
		if(code < midRangeLower)
		{
			midRangeLower = code;
		}
		if(code > midRangeUpper)
		{
			midRangeUpper = code;
		}
	}
	else
	{
		/* Latin/katakana high character range */
		if(code < highRangeLower)
		{
			highRangeLower = code;
		}
	}
}

/*
 * Write a section header.
 */
static void writeSection(FILE *file, unsigned long num, unsigned long size)
{
	putc((int)(num & 0xFF), file);
	putc((int)((num >> 8) & 0xFF), file);
	putc((int)((num >> 16) & 0xFF), file);
	putc((int)((num >> 24) & 0xFF), file);
	putc((int)(size & 0xFF), file);
	putc((int)((size >> 8) & 0xFF), file);
	putc((int)((size >> 16) & 0xFF), file);
	putc((int)((size >> 24) & 0xFF), file);
}

/*
 * Write an array of 16-bit data values.
 */
static void writeData(FILE *file, unsigned short *data, unsigned long size)
{
	while(size > 0)
	{
		putc((int)(*data & 0xFF), file);
		putc((int)((*data >> 8) & 0xFF), file);
		++data;
		--size;
	}
}

/*
 * Section numbers for the conversion tables.
 */
#define	Info_Block				1
#define	DByte_To_Unicode		2
#define	Low_Unicode_To_DByte	3
#define	Mid_Unicode_To_DByte	4
#define	High_Unicode_To_DByte	5

/*
 * Write the conversion table file.
 */
static void writeTables(FILE *file)
{
	unsigned long size;
	int first, second;

	/* Range check values */
	if(lowFirst > highFirst)
	{
		lowFirst = highFirst = 0;
	}
	if(lowSecond > highSecond)
	{
		lowSecond = highSecond = 0;
	}
	if(midRangeLower > midRangeUpper)
	{
		midRangeLower = midRangeUpper = 0;
	}

	/* Write the information block */
	writeSection(file, Info_Block, 12);
	putc((lowFirst & 0xFF), file);
	putc((highFirst & 0xFF), file);
	putc((lowSecond & 0xFF), file);
	putc((highSecond & 0xFF), file);
	putc((lowRangeUpper & 0xFF), file);
	putc(((lowRangeUpper >> 8) & 0xFF), file);
	putc((midRangeLower & 0xFF), file);
	putc(((midRangeLower >> 8) & 0xFF), file);
	putc((midRangeUpper & 0xFF), file);
	putc(((midRangeUpper >> 8) & 0xFF), file);
	putc((highRangeLower & 0xFF), file);
	putc(((highRangeLower >> 8) & 0xFF), file);

	/* Write the double byte to unicode conversion table */
	size = (highFirst - lowFirst + 1) * (highSecond - lowSecond + 1) * 2;
	writeSection(file, DByte_To_Unicode, size);
	for(first = lowFirst; first <= highFirst; ++first)
	{
		for(second = lowSecond; second <= highSecond; ++second)
		{
			size = dbyteToUnicode[first * 256 + second];
			putc((size & 0xFF), file);
			putc(((size >> 8) & 0xFF), file);
		}
	}

	/* Write the low unicode to dbyte conversion table */
	size = (lowRangeUpper + 1);
	writeSection(file, Low_Unicode_To_DByte, size * 2);
	writeData(file, unicodeToDByte, size);

	/* Write the middle unicode to dbyte conversion table */
	size = (midRangeUpper - midRangeLower + 1);
	writeSection(file, Mid_Unicode_To_DByte, size * 2);
	writeData(file, unicodeToDByte + midRangeLower, size);

	/* Write the high unicode to dbyte conversion table */
	size = (0x10000 - highRangeLower);
	writeSection(file, High_Unicode_To_DByte, size * 2);
	writeData(file, unicodeToDByte + highRangeLower, size);
}

/*
 * Create all of the tables that we need based on the input file.
 */
static int createTables(const char *filename)
{
	FILE *file;

	/* Create the JIS conversion table */
	if((file = fopen(filename, "wb")) == NULL)
	{
		if((file = fopen(filename, "wb")) == NULL)
		{
			perror(filename);
			return 1;
		}
	}
	writeTables(file);
	fclose(file);

	/* Done */
	return 0;
}
