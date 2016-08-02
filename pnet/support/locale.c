/*
 * locale.c - Routines for processing locale information.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
#if HAVE_WCHAR_H
#include <wchar.h>
#endif
#if HAVE_STDLIB_H
#include <stdlib.h>
#endif
#if HAVE_LOCALE_H
#include <locale.h>
#endif
#if HAVE_LANGINFO_H
#include <langinfo.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * See if we have sufficient functions to do locale-based conversion.
 * The "IL_CONFIG_LATIN1" macro is defined to set the encoding to Latin1
 * if we have no idea how to convert to and from the system encoding,
 * or if the profile says to always use Latin1.
 */
#if !(defined(HAVE_WCTOMB) || defined(HAVE_WCRTOMB)) || \
    !(defined(HAVE_MBTOWC) || defined(HAVE_MBRTOWC))
#ifndef IL_CONFIG_LATIN1
#define	IL_CONFIG_LATIN1	1
#endif
#endif

/*
 * Some interesting Windows code page numbers.
 */
#define	CP_8859_1			28591
#define	CP_8859_2			28592
#define	CP_8859_3			28593
#define	CP_8859_4			28594
#define	CP_8859_5			28595
#define	CP_8859_6			28596
#define	CP_8859_7			28597
#define	CP_8859_8			28598
#define	CP_8859_9			28599
#define	CP_8859_15			28605
#define	CP_SHIFT_JIS		932
#define	CP_KOI8_R			20866
#define	CP_KOI8_U			21866
#define	CP_UTF7				65000
#define	CP_UTF8				65001
#define	CP_UCS2_LE			1200
#define	CP_UCS2_BE			1201
#define	CP_UCS2_DETECT		1202		/* Fake value for auto-detection */

void ILInitLocale(void)
{
#if HAVE_SETLOCALE
	setlocale(LC_ALL, "");
#endif
}

#if defined(HAVE_NL_LANGINFO) && defined(CODESET) && !defined(IL_CONFIG_LATIN1)

/*
 * Compare two codeset names.
 */
static int CompareCodesets(const char *name1, const char *name2)
{
	char n1;
	while(*name1 != '\0' && *name2 != '\0')
	{
		/* Normalize the incoming character */
		n1 = *name1;
		if(n1 >= 'A' && n1 <= 'Z')
		{
			n1 = n1 - 'A' + 'a';
		}
		else if(n1 == '_')
		{
			n1 = '-';
		}

		/* Do we have a match? */
		if(n1 != *name2)
		{
			return 0;
		}
		++name1;
		++name2;
	}
	if(*name2 == '\0')
	{
		/* We ignore extraneous data after a colon, because it is
		   usually year information that we aren't interested in */
		return (*name1 == '\0' || *name1 == ':');
	}
	return 0;
}

#endif

unsigned ILGetCodePage(void)
{
#if defined(HAVE_NL_LANGINFO) && defined(CODESET) && !defined(IL_CONFIG_LATIN1)
	char *set;
	int index;

	/* Table of common code sets and their code page equivalents.
	   The names are normalised to lower case with '_' replaced
	   with '-'.  This table will no doubt need to be extended */
	static struct codeset
	{
		const char *name;
		unsigned page;

	} const codesets[] = {
		{"iso8859-1",			CP_8859_1},
		{"iso-8859-1",			CP_8859_1},
		{"iso8859-2",			CP_8859_2},
		{"iso-8859-2",			CP_8859_2},
		{"iso8859-3",			CP_8859_3},
		{"iso-8859-3",			CP_8859_3},
		{"iso8859-4",			CP_8859_4},
		{"iso-8859-4",			CP_8859_4},
		{"iso8859-5",			CP_8859_5},
		{"iso-8859-5",			CP_8859_5},
		{"iso8859-6",			CP_8859_6},
		{"iso-8859-6",			CP_8859_6},
		{"iso8859-7",			CP_8859_7},
		{"iso-8859-7",			CP_8859_7},
		{"iso8859-8",			CP_8859_8},
		{"iso-8859-8",			CP_8859_8},
		{"iso8859-9",			CP_8859_9},
		{"iso-8859-9",			CP_8859_9},
		{"iso8859-15",			CP_8859_15},
		{"iso-8859-15",			CP_8859_15},

		{"windows-1250",		1250},
		{"windows-1251",		1251},
		{"windows-1252",		1252},
		{"windows-1253",		1253},
		{"windows-1254",		1254},
		{"windows-1255",		1255},
		{"windows-1256",		1256},
		{"windows-1257",		1257},
		{"windows-1258",		1258},

		{"shift-jis",			CP_SHIFT_JIS},
		{"sjis",				CP_SHIFT_JIS},
		{"koi8-r",				CP_KOI8_R},
		{"koi8-u",				CP_KOI8_U},

		{"utf-7",				CP_UTF7},
		{"utf-8",				CP_UTF8},
		{"ucs-2",				CP_UCS2_DETECT},
		{"ucs-2le",				CP_UCS2_LE},
		{"ucs-2be",				CP_UCS2_BE},
		{"utf-16",				CP_UCS2_DETECT},
		{"utf-16le",			CP_UCS2_LE},
		{"utf-16be",			CP_UCS2_BE},

		/* Map ASCII codesets to Latin1 just in case the underlying
		   locale system is misconfigured */
		{"us-ascii",			CP_8859_1},
		{"ansi-3.4-1968",		CP_8859_1},
		{"ansi-3.4-1986",		CP_8859_1},
		{"ansi-3.4",			CP_8859_1},
	};

	/* Get the code set for the current locale */
	set = nl_langinfo(CODESET);
	if(!set)
	{
		return 0;
	}

	/* Look in the codeset table for the corresponding code page */
	for(index = 0; index < (sizeof(codesets) / sizeof(codesets[0])); ++index)
	{
		if(CompareCodesets(set, codesets[index].name))
		{
			if(codesets[index].page != CP_UCS2_DETECT)
			{
				return codesets[index].page;
			}
			else
			{
				/* Detect the default UCS-2 encoding for this platform */
				union { short x; unsigned char y[2]; } convert;
				convert.x = 0x0102;
				if(convert.y[0] == 0x01)
				{
					return CP_UCS2_BE;
				}
				else
				{
					return CP_UCS2_LE;
				}
			}
		}
	}
	return 0;
#else
#ifdef IL_CONFIG_LATIN1
	/* Force the code page to always be Latin1 */
	return CP_8859_1;
#else
	/* We have no idea how to obtain the code page information */
	return 0;
#endif
#endif
}

unsigned ILGetCultureID(void)
{
	/* We prefer to return culture information by name */
	return 0;
}

/*
 * Locale-safe letter manipulation macros.
 */
#define	IsAlpha(ch)	(((ch) >= 'A' && (ch) <= 'Z') || \
					 ((ch) >= 'a' && (ch) <= 'z'))
#define	ToLower(ch)	(((ch) >= 'A' && (ch) <= 'Z') ? (ch) - 'A' + 'a' : (ch))
#define	ToUpper(ch)	(((ch) >= 'a' && (ch) <= 'z') ? (ch) - 'a' + 'A' : (ch))

char *ILGetCultureName(void)
{
#if defined(__palmos__)
	return 0;
#else
	char *env;
	char name[8];

	/* Get the culture information from LC_ALL or LANG environment variable */
	env = getenv("LC_ALL");
	if(!env || *env == '\0')
	{
		env = getenv("LANG");
		if(!env || *env == '\0')
		{
			return 0;
		}
	}

	/* Convert the LANG value into an ECMA culture identifier */
	if(!IsAlpha(env[0]) || !IsAlpha(env[1]))
	{
		return 0;
	}
	name[0] = ToLower(env[0]);
	name[1] = ToLower(env[1]);
	if(env[2] == '\0')
	{
		name[2] = '\0';
		return ILDupString(name);
	}
	if(env[2] != '-' && env[2] != '_')
	{
		return 0;
	}
	if(!IsAlpha(env[3]) || !IsAlpha(env[4]))
	{
		return 0;
	}
	name[2] = '-';
	name[3] = ToUpper(env[3]);
	name[4] = ToUpper(env[4]);
	name[5] = '\0';
	return ILDupString(name);
#endif
}

#ifdef	__cplusplus
};
#endif
