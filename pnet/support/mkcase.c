/*
 * mkcase.c - Make the Unicode case conversion table from UnicodeData.txt.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

#include <stdio.h>
#include <stdlib.h>
#include "il_system.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(__palmos__)

int main(int argc, char *argv[])
{
	return 0;
}

#else

/*
 * Range limits for compressing the table size.
 */
#define	RANGE1_LOWER		0x0000
#define	RANGE1_UPPER		0x05FF
#define	RANGE2_LOWER		0x1E00
#define	RANGE2_UPPER		0x24FF
#define	RANGE3_LOWER		0xFF00
#define	RANGE3_UPPER		0xFFFF

/*
 * Lower, upper, and title case conversions.
 */
static unsigned short lower[65536];
static unsigned short upper[65536];
static unsigned short title[65536];

/*
 * Parse a hex character value.
 */
static unsigned long parseHex(char **_ptr, unsigned long defValue)
{
	char *ptr = *_ptr;
	unsigned long ch;
	if(*ptr == '\0' || *ptr == ';' || *ptr == '\r' || *ptr == '\n')
	{
		return defValue;
	}
	ch = 0;
	while(*ptr != '\0' && *ptr != ';')
	{
		if(*ptr >= '0' && *ptr <= '9')
		{
			ch = (ch * 16) + (*ptr - '0');
		}
		else if(*ptr >= 'A' && *ptr <= 'F')
		{
			ch = (ch * 16) + (*ptr - 'A' + 10);
		}
		else if(*ptr >= 'a' && *ptr <= 'f')
		{
			ch = (ch * 16) + (*ptr - 'a' + 10);
		}
		else if(*ptr == '\r' || *ptr == '\n')
		{
			/* Skip end of line characters */
		}
		else
		{
			fprintf(stderr, "Bad character value in input: '%c'\n", *ptr);
			exit(1);
		}
		++ptr;
	}
	*_ptr = ptr;
	return ch;
}

/*
 * Write a case conversion table.
 */
static void writeTable(const char *name, unsigned short *table)
{
	int outIfDef = 0;
	unsigned long ch;
	printf("static unsigned short const %s[] = {\n", name);
	for(ch = 0; ch < 65536; ++ch)
	{
		/* Filter out characters that we know will map to themselves */
		if(ch > RANGE1_UPPER && ch < RANGE2_LOWER)
		{
			continue;
		}
		if(ch > RANGE2_UPPER && ch < RANGE3_LOWER)
		{
			continue;
		}

		/* Print a #ifdef to be used in latin1 mode to reduce the table size */
		if(!outIfDef && ch >= 0x100)
		{
			printf("#ifndef SMALL_UNICODE_TABLE\n");
			outIfDef = 1;
		}

		/* Print the character mapping information */
		printf("0x%04X, ", (int)(table[ch]));
		if((ch & 7) == 7)
		{
			putc('\n', stdout);
		}
	}
	printf("#endif\n");
	printf("};\n");
}

int main(int argc, char *argv[])
{
	char buffer[BUFSIZ];
	unsigned long startch;
	unsigned long ch;
	char *ptr;
	char *catName;
	unsigned long upperch;
	unsigned long lowerch;
	unsigned long titlech;

	/* Initialize the value tables to straight-through mappings */
	for(ch = 0; ch < 65536; ++ch)
	{
		lower[ch] = (unsigned short)ch;
		upper[ch] = (unsigned short)ch;
		title[ch] = (unsigned short)ch;
	}

	/* Process UnicodeData.txt to get the case conversion information */
	startch = 0;
	while(fgets(buffer, sizeof(buffer), stdin))
	{
		ptr = buffer;

		/* Parse the character value */
		ch = parseHex(&ptr, 0);
		if(*ptr != ';')
		{
			continue;
		}

		/* Skip the character if not within the 16-bit range */
		if(ch >= 0x10000)
		{
			startch = ch + 1;
			continue;
		}

		/* Find the category name */
		++ptr;
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		catName = ptr;
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}

		/* If the character name ended in ", Last>", then ignore
		   the character range */
		if((catName - buffer) > 8 && !strncmp(catName - 8, ", Last>", 7))
		{
			continue;
		}

		/* Find the case conversion fields */
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}
		while(*ptr != '\0' && *ptr != ';')
		{
			++ptr;
		}
		if(*ptr == ';')
		{
			++ptr;
		}

		/* Extract the upper, lower, and title mappings */
		upperch = parseHex(&ptr, ch);
		if(*ptr != ';')
		{
			continue;
		}
		++ptr;
		lowerch = parseHex(&ptr, ch);
		if(*ptr != ';')
		{
			continue;
		}
		++ptr;
		titlech = parseHex(&ptr, ch);

		/* Update the case mapping tables */
		upper[ch] = (unsigned short)upperch;
		lower[ch] = (unsigned short)lowerch;
		title[ch] = (unsigned short)titlech;
	}

	/* Write out the mapping tables */
	printf("/* This file is automatically generated - do not edit */\n");
	printf("#define	UNICASE_RANGE1_LOWER  0x%04X\n", RANGE1_LOWER);
	printf("#define	UNICASE_RANGE1_UPPER  0x%04X\n", RANGE1_UPPER);
	printf("#define	UNICASE_RANGE1_OFFSET 0x%04X\n", 0);
	printf("#define	UNICASE_RANGE2_LOWER  0x%04X\n", RANGE2_LOWER);
	printf("#define	UNICASE_RANGE2_UPPER  0x%04X\n", RANGE2_UPPER);
	printf("#define	UNICASE_RANGE2_OFFSET 0x%04X\n", RANGE1_UPPER + 1);
	printf("#define	UNICASE_RANGE3_LOWER  0x%04X\n", RANGE3_LOWER);
	printf("#define	UNICASE_RANGE3_UPPER  0x%04X\n", RANGE3_UPPER);
	printf("#define	UNICASE_RANGE3_OFFSET 0x%04X\n",
		   (RANGE1_UPPER - RANGE1_LOWER + 1) +
		   (RANGE2_UPPER - RANGE2_LOWER + 1));
	writeTable("unicodeToUpper", upper);
	writeTable("unicodeToLower", lower);
#if 0
	writeTable("unicodeToTitle", title);
#endif

	/* Done */
	return 0;
}

#endif

#ifdef	__cplusplus
};
#endif
