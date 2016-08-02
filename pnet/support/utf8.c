/*
 * utf8.c - Utility functions for managing UTF-8 sequences.
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

#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

unsigned long ILUTF8ReadChar(const void *_str, int len, int *posn)
{
	const char *str = (const char *)_str;
	char ch = str[*posn];
	unsigned long result;
	if((ch & 0x80) == 0)
	{
		/* Single-byte UTF-8 encoding */
		++(*posn);
		return (unsigned long)ch;
	}
	else if((ch & (char)0xE0) == (char)0xC0 && (*posn + 2) <= len)
	{
		/* Two-byte UTF-8 encoding */
		result = ((((unsigned long)(ch & 0x1F)) << 6) |
		           ((unsigned long)(str[(*posn) + 1] & 0x3F)));
		(*posn) += 2;
		return result;
	}
	else if((ch & (char)0xF0) == (char)0xE0 && (*posn + 3) <= len)
	{
		/* Three-byte UTF-8 encoding */
		result = ((((unsigned long)(ch & 0x0F)) << 12) |
		          (((unsigned long)(str[(*posn) + 1] & 0x3F)) << 6) |
		           ((unsigned long)(str[(*posn) + 2] & 0x3F)));
		(*posn) += 3;
		return result;
	}
	else if((ch & (char)0xF8) == (char)0xF0 && (*posn + 4) <= len)
	{
		/* Four-byte UTF-8 encoding */
		result = ((((unsigned long)(ch & 0x07)) << 18) |
		          (((unsigned long)(str[(*posn) + 1] & 0x3F)) << 12) |
		          (((unsigned long)(str[(*posn) + 2] & 0x3F)) << 6) |
		           ((unsigned long)(str[(*posn) + 3] & 0x3F)));
		(*posn) += 4;
		return result;
	}
	else if((ch & (char)0xFC) == (char)0xF8 && (*posn + 5) <= len)
	{
		/* Five-byte UTF-8 encoding */
		result = ((((unsigned long)(ch & 0x03)) << 24) |
		          (((unsigned long)(str[(*posn) + 1] & 0x3F)) << 18) |
		          (((unsigned long)(str[(*posn) + 2] & 0x3F)) << 12) |
		          (((unsigned long)(str[(*posn) + 3] & 0x3F)) << 6) |
		           ((unsigned long)(str[(*posn) + 4] & 0x3F)));
		(*posn) += 5;
		return result;
	}
	else if((ch & (char)0xFC) == (char)0xFC && (*posn + 6) <= len)
	{
		/* Six-byte UTF-8 encoding */
		result = ((((unsigned long)(ch & 0x03)) << 30) |
		          (((unsigned long)(str[(*posn) + 1] & 0x3F)) << 24) |
		          (((unsigned long)(str[(*posn) + 2] & 0x3F)) << 18) |
		          (((unsigned long)(str[(*posn) + 3] & 0x3F)) << 12) |
		          (((unsigned long)(str[(*posn) + 4] & 0x3F)) << 6) |
		           ((unsigned long)(str[(*posn) + 5] & 0x3F)));
		(*posn) += 6;
		return result;
	}
	else
	{
		/* Invalid UTF-8 encoding: treat as an 8-bit Latin-1 character */
		++(*posn);
		return (((unsigned long)ch) & 0xFF);
	}
}

int ILUTF8WriteChar(char *str, unsigned long ch)
{
	if(str)
	{
		/* Write the character to the buffer */
		if(!ch)
		{
			/* Encode embedded NUL's as 0xC0 0x80 so that code
			   that uses C-style strings doesn't get confused */
			str[0] = (char)0xC0;
			str[1] = (char)0x80;
			return 2;
		}
		else if(ch < (unsigned long)0x80)
		{
			str[0] = (char)ch;
			return 1;
		}
		else if(ch < (((unsigned long)1) << 11))
		{
			str[0] = (char)(0xC0 | (ch >> 6));
			str[1] = (char)(0x80 | (ch & 0x3F));
			return 2;
		}
		else if(ch < (((unsigned long)1) << 16))
		{
			str[0] = (char)(0xE0 | (ch >> 12));
			str[1] = (char)(0x80 | ((ch >> 6) & 0x3F));
			str[2] = (char)(0x80 | (ch & 0x3F));
			return 3;
		}
		else if(ch < (((unsigned long)1) << 21))
		{
			str[0] = (char)(0xF0 | (ch >> 18));
			str[1] = (char)(0x80 | ((ch >> 12) & 0x3F));
			str[2] = (char)(0x80 | ((ch >> 6) & 0x3F));
			str[3] = (char)(0x80 | (ch & 0x3F));
			return 4;
		}
		else if(ch < (((unsigned long)1) << 26))
		{
			str[0] = (char)(0xF8 | (ch >> 24));
			str[1] = (char)(0x80 | ((ch >> 18) & 0x3F));
			str[2] = (char)(0x80 | ((ch >> 12) & 0x3F));
			str[3] = (char)(0x80 | ((ch >> 6) & 0x3F));
			str[4] = (char)(0x80 | (ch & 0x3F));
			return 5;
		}
		else
		{
			str[0] = (char)(0xFC | (ch >> 30));
			str[1] = (char)(0x80 | ((ch >> 24) & 0x3F));
			str[2] = (char)(0x80 | ((ch >> 18) & 0x3F));
			str[3] = (char)(0x80 | ((ch >> 12) & 0x3F));
			str[4] = (char)(0x80 | ((ch >> 6) & 0x3F));
			str[5] = (char)(0x80 | (ch & 0x3F));
			return 6;
		}
	}
	else
	{
		/* Determine the length of the character */
		if(!ch)
		{
			return 2;
		}
		else if(ch < (unsigned long)0x80)
		{
			return 1;
		}
		else if(ch < (((unsigned long)1) << 11))
		{
			return 2;
		}
		else if(ch < (((unsigned long)1) << 16))
		{
			return 3;
		}
		else if(ch < (((unsigned long)1) << 21))
		{
			return 4;
		}
		else if(ch < (((unsigned long)1) << 26))
		{
			return 5;
		}
		else
		{
			return 6;
		}
	}
}

#ifdef	__cplusplus
};
#endif
