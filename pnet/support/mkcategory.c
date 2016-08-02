/*
 * mkcategory.c - Make the Unicode category table from UnicodeData.txt.
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

#include <stdio.h>
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
 * Full category table.
 */
static unsigned char category[65536];

int main(int argc, char *argv[])
{
	char buffer[BUFSIZ];
	unsigned long startch;
	unsigned long ch;
	char *ptr;
	char *catName;
	ILUnicodeCategory catValue;
	unsigned long mask;
	int wordsPerLine;

	/* Initialize the category table to all unassigned */
	for(ch = 0; ch < 65536; ++ch)
	{
		category[ch] = (unsigned char)ILUnicode_OtherNotAssigned;
	}

	/* Process UnicodeData.txt to get the category information */
	startch = 0;
	while(fgets(buffer, sizeof(buffer), stdin))
	{
		ptr = buffer;

		/* Parse the character value */
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
			else
			{
				fprintf(stderr, "Bad character value in input\n");
				return 1;
			}
			++ptr;
		}
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
		*ptr = '\0';

		/* Convert the category name into its numeric equivalent */
		if(!strcmp(catName, "Lu"))
		{
			catValue = ILUnicode_UppercaseLetter;
		}
		else if(!strcmp(catName, "Ll"))
		{
			catValue = ILUnicode_LowercaseLetter;
		}
		else if(!strcmp(catName, "Lt"))
		{
			catValue = ILUnicode_TitlecaseLetter;
		}
		else if(!strcmp(catName, "Mn"))
		{
			catValue = ILUnicode_NonSpacingMark;
		}
		else if(!strcmp(catName, "Mc"))
		{
			catValue = ILUnicode_SpaceCombiningMark;
		}
		else if(!strcmp(catName, "Me"))
		{
			catValue = ILUnicode_EnclosingMark;
		}
		else if(!strcmp(catName, "Nd"))
		{
			catValue = ILUnicode_DecimalDigitNumber;
		}
		else if(!strcmp(catName, "Nl"))
		{
			catValue = ILUnicode_LetterNumber;
		}
		else if(!strcmp(catName, "No"))
		{
			catValue = ILUnicode_OtherNumber;
		}
		else if(!strcmp(catName, "Zs"))
		{
			catValue = ILUnicode_SpaceSeparator;
		}
		else if(!strcmp(catName, "Zl"))
		{
			catValue = ILUnicode_LineSeparator;
		}
		else if(!strcmp(catName, "Zp"))
		{
			catValue = ILUnicode_ParagraphSeparator;
		}
		else if(!strcmp(catName, "Cc"))
		{
			catValue = ILUnicode_Control;
		}
		else if(!strcmp(catName, "Cf"))
		{
			catValue = ILUnicode_Format;
		}
		else if(!strcmp(catName, "Cs"))
		{
			catValue = ILUnicode_Surrogate;
		}
		else if(!strcmp(catName, "Co"))
		{
			catValue = ILUnicode_PrivateUse;
		}
		else if(!strcmp(catName, "Cn"))
		{
			catValue = ILUnicode_OtherNotAssigned;
		}
		else if(!strcmp(catName, "Lm"))
		{
			catValue = ILUnicode_ModifierLetter;
		}
		else if(!strcmp(catName, "Lo"))
		{
			catValue = ILUnicode_OtherLetter;
		}
		else if(!strcmp(catName, "Pc"))
		{
			catValue = ILUnicode_ConnectorPunctuation;
		}
		else if(!strcmp(catName, "Pd"))
		{
			catValue = ILUnicode_DashPunctuation;
		}
		else if(!strcmp(catName, "Ps"))
		{
			catValue = ILUnicode_OpenPunctuation;
		}
		else if(!strcmp(catName, "Pe"))
		{
			catValue = ILUnicode_ClosePunctuation;
		}
		else if(!strcmp(catName, "Pi"))
		{
			catValue = ILUnicode_InitialQuotePunctuation;
		}
		else if(!strcmp(catName, "Pf"))
		{
			catValue = ILUnicode_FinalQuotePunctuation;
		}
		else if(!strcmp(catName, "Po"))
		{
			catValue = ILUnicode_OtherPunctuation;
		}
		else if(!strcmp(catName, "Sm"))
		{
			catValue = ILUnicode_MathSymbol;
		}
		else if(!strcmp(catName, "Sc"))
		{
			catValue = ILUnicode_CurrencySymbol;
		}
		else if(!strcmp(catName, "Sk"))
		{
			catValue = ILUnicode_ModifierSymbol;
		}
		else if(!strcmp(catName, "So"))
		{
			catValue = ILUnicode_OtherSymbol;
		}
		else
		{
			catValue = ILUnicode_OtherNotAssigned;
		}

		/* If the character name ended in ", Last>", then we need
		   to set a range of characters.  Otherwise a single character */
		if((catName - buffer) > 8 && !strncmp(catName - 8, ", Last>", 7))
		{
			while(startch <= ch)
			{
				category[startch] = (unsigned char)catValue;
				++startch;
			}
		}
		else
		{
			category[ch] = (unsigned char)catValue;
		}
		startch = ch + 1;
	}

	/* Write out the category details */
	printf("/* This file is automatically generated - do not edit */\n");
	printf("static ILUInt32 const charCategories[] = {\n");
	mask = 0;
	wordsPerLine = 0;
	for(ch = 0; ch < 65536; ++ch)
	{
		mask |= (((unsigned long)(category[ch])) << (5 * (ch % 6)));
		if((ch % 6) == 5)
		{
			printf("0x%08lX, ", mask);
			mask = 0;
			++wordsPerLine;
			if(ch >= 0x00FF && (ch - 6) < 0x00FF)
			{
				printf("\n#ifndef SMALL_UNICODE_TABLE\n");
				wordsPerLine = 0;
			}
			if(wordsPerLine >= 6)
			{
				printf("\n");
				wordsPerLine = 0;
			}
		}
	}
	printf("0x%08lX\n", mask);
	printf("#endif\n");
	printf("};\n");

	/* Done */
	return 0;
}

#endif

#ifdef	__cplusplus
};
#endif
