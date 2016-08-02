/*
 * ansi.c - Handle the system-defined "ANSI" character encoding.
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
#if HAVE_LIMITS_H
#include <limits.h>
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
    !(defined(HAVE_MBTOWC) || defined(HAVE_MBRTOWC)) || \
	!(defined(HAVE_WCHAR_H))
#ifndef IL_CONFIG_LATIN1
#define	IL_CONFIG_LATIN1	1
#endif
#endif

#if !defined(HAVE_MBSTATE_T) && defined(HAVE_WCHAR_H)
#ifdef __BEOS__
typedef struct 
{
	int 	__count;
	wint_t	__value;
}mbstate_t;
#endif
#endif

#ifndef	MB_LEN_MAX
#define	MB_LEN_MAX	6
#endif

unsigned long ILAnsiGetByteCount(const unsigned short *chars,
								 unsigned long count)
{
#ifdef IL_CONFIG_LATIN1
	return count;
#else
#if HAVE_WCRTOMB
	/* Use the re-entrant function to perform the conversion */
	mbstate_t state;
	char buf[MB_LEN_MAX+1];
	size_t chlen;
	unsigned short ch;
	unsigned long len = 0;
	ILMemZero(&state, sizeof(state));
	wcrtomb(buf, 0, &state);
	while(count > 0)
	{
		ch = *chars++;
		if(ch != 0)
		{
			chlen = wcrtomb(buf, (wchar_t)ch, &state);
			if(chlen != (size_t)(-1))
			{
				len += (unsigned long)chlen;
			}
		}
		else
		{
			++len;
		}
		--count;
	}
	return len;
#else
	/* Use the non re-entrant function to perform the conversion
	   and just hope that the underlying libc takes care of the
	   thread-safety issues for us */
	char buf[MB_LEN_MAX+1];
	int chlen;
	unsigned long len = 0;
	wctomb((char *)0, 0);
	while(count > 0)
	{
		chlen = wctomb(buf, (wchar_t)(*chars));
		if(chlen > 0)
		{
			len += (unsigned long)chlen;
		}
		++chars;
		--count;
	}
	return len;
#endif
#endif
}

long ILAnsiGetBytes(const unsigned short *chars, unsigned long charCount,
                   	unsigned char *bytes, unsigned long byteCount)
{
#ifdef IL_CONFIG_LATIN1
	unsigned long len;
	unsigned short ch;

	/* Check for enough space in the output buffer */
	if(charCount > byteCount)
	{
		return -1;
	}

	/* Convert the characters */
	len = charCount;
	while(len > 0)
	{
		ch = *chars++;
		if(ch < (unsigned short)0x0100)
		{
			*bytes++ = (unsigned char)ch;
		}
		else
		{
			*bytes++ = (unsigned char)'?';
		}
		--len;
	}
	return charCount;
#else
#if HAVE_WCRTOMB
	/* Use the re-entrant function to perform the conversion */
	mbstate_t state;
	char buf[MB_LEN_MAX+1];
	size_t chlen;
	unsigned short ch;
	unsigned long len = 0;
	ILMemZero(&state, sizeof(state));
	wcrtomb(buf, 0, &state);
	while(charCount > 0)
	{
		ch = *chars++;
		if(ch != 0)
		{
			chlen = wcrtomb(buf, (wchar_t)ch, &state);
			if(chlen != (size_t)(-1))
			{
				if(((unsigned long)chlen) > byteCount)
				{
					return -1;
				}
				ILMemCpy(bytes, buf, chlen);
				bytes += chlen;
				byteCount -= (unsigned long)chlen;
				len += (unsigned long)chlen;
			}
		}
		else if(byteCount > 0)
		{
			*bytes++ = 0;
			--byteCount;
			++len;
		}
		else
		{
			return -1;
		}
		--charCount;
	}
	return len;
#else
	/* Use the non re-entrant function to perform the conversion
	   and just hope that the underlying libc takes care of the
	   thread-safety issues for us */
	char buf[MB_LEN_MAX+1];
	int chlen;
	unsigned long len = 0;
	wctomb((char *)0, 0);
	while(charCount > 0)
	{
		chlen = wctomb(buf, (wchar_t)(*chars));
		if(chlen > 0)
		{
			if(((unsigned long)chlen) > byteCount)
			{
				return -1;
			}
			ILMemCpy(bytes, buf, chlen);
			bytes += chlen;
			byteCount -= (unsigned long)chlen;
			len += (unsigned long)chlen;
		}
		++chars;
		--charCount;
	}
	return len;
#endif
#endif
}

unsigned long ILAnsiGetCharCount(const unsigned char *bytes,
								 unsigned long count)
{
#ifdef IL_CONFIG_LATIN1
	return count;
#else
#if HAVE_MBRTOWC
	/* Use the re-entrant function to perform the conversion */
	mbstate_t state;
	size_t chlen;
	unsigned long len = 0;
	wchar_t ch;
	ILMemZero(&state, sizeof(state));
	mbrtowc((wchar_t *)0, (char *)0, 0, &state);
	while(count > 0)
	{
		chlen = mbrtowc(&ch, (char *)bytes, (size_t)count, &state);
		if(chlen == (size_t)(-1) || chlen == (size_t)(-2))
		{
			/* Invalid character */
			++bytes;
			--count;
		}
		else if(chlen != 0)
		{
			/* Ordinary character */
			len += ILUTF16WriteChar((unsigned short *)0, (unsigned long)ch);
			bytes += chlen;
			count -= (unsigned long)chlen;
		}
		else
		{
			/* Embedded NUL character */
			++len;
			++bytes;
			--count;
		}
	}
	return len;
#else
	/* Use the non re-entrant function to perform the conversion
	   and just hope that the underlying libc takes care of the
	   thread-safety issues for us */
	int chlen;
	unsigned long len = 0;
	wchar_t ch;
	mbtowc((wchar_t *)0, (char *)0, 0);
	while(count > 0)
	{
		chlen = mbtowc(&ch, (char *)bytes, (size_t)count);
		if(chlen > 0)
		{
			/* Ordinary character */
			len += ILUTF16WriteChar((unsigned short *)0, (unsigned long)ch);
			bytes += chlen;
			count -= (unsigned long)chlen;
		}
		else if(!chlen)
		{
			/* Embedded NUL character */
			++len;
			++bytes;
			--count;
		}
		else
		{
			/* Invalid character */
			++bytes;
			--count;
		}
	}
	return len;
#endif
#endif
}

long ILAnsiGetChars(const unsigned char *bytes, unsigned long byteCount,
					unsigned short *chars, unsigned long charCount)
{
#ifdef IL_CONFIG_LATIN1
	unsigned long len;

	/* Check for enough space in the output buffer */
	if(byteCount > charCount)
	{
		return -1;
	}

	/* Convert the bytes */
	len = byteCount;
	while(len > 0)
	{
		*chars++ = (unsigned short)(*bytes++);
		--len;
	}
	return (long)byteCount;
#else
#if HAVE_MBRTOWC
	/* Use the re-entrant function to perform the conversion */
	mbstate_t state;
	size_t chlen;
	unsigned long len = 0;
	wchar_t ch;
	int wrlen;
	ILMemZero(&state, sizeof(state));
	mbrtowc((wchar_t *)0, (char *)0, 0, &state);
	while(byteCount > 0)
	{
		chlen = mbrtowc(&ch, (char *)bytes, (size_t)byteCount, &state);
		if(chlen == (size_t)(-1) || chlen == (size_t)(-2))
		{
			/* Invalid character */
			++bytes;
			--byteCount;
		}
		else if(chlen != 0)
		{
			/* Ordinary character */
			wrlen = ILUTF16WriteChar((unsigned short *)0, (unsigned long)ch);
			if(charCount < (unsigned long)wrlen)
			{
				return -1;
			}
			ILUTF16WriteChar(chars, (unsigned long)ch);
			chars += wrlen;
			len += wrlen;
			bytes += chlen;
			byteCount -= (unsigned long)chlen;
		}
		else
		{
			/* Embedded NUL character */
			if(charCount <= 0)
			{
				return -1;
			}
			*chars++ = '\0';
			++len;
			++bytes;
			--byteCount;
		}
	}
	return (long)len;
#else
	/* Use the non re-entrant function to perform the conversion
	   and just hope that the underlying libc takes care of the
	   thread-safety issues for us */
	int chlen;
	unsigned long len = 0;
	wchar_t ch;
	int wrlen;
	mbtowc((wchar_t *)0, (char *)0, 0);
	while(byteCount > 0)
	{
		chlen = mbtowc(&ch, (char *)bytes, (size_t)byteCount);
		if(chlen > 0)
		{
			/* Ordinary character */
			wrlen = ILUTF16WriteChar((unsigned short *)0, (unsigned long)ch);
			if(charCount < (unsigned long)wrlen)
			{
				return -1;
			}
			ILUTF16WriteChar(chars, (unsigned long)ch);
			chars += wrlen;
			len += wrlen;
			bytes += chlen;
			byteCount -= (unsigned long)chlen;
		}
		else if(!chlen)
		{
			/* Embedded NUL character */
			if(charCount <= 0)
			{
				return -1;
			}
			*chars++ = '\0';
			++len;
			++bytes;
			--byteCount;
		}
		else
		{
			/* Invalid character */
			++bytes;
			--byteCount;
		}
	}
	return (long)len;
#endif
#endif
}

unsigned long ILAnsiGetMaxByteCount(unsigned long charCount)
{
#ifdef IL_CONFIG_LATIN1
	return charCount;
#else
	return charCount * MB_LEN_MAX;
#endif
}

unsigned long ILAnsiGetMaxCharCount(unsigned long byteCount)
{
	return byteCount;
}

#ifdef	__cplusplus
};
#endif
