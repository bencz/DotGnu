/*
 * utf16.c - Utility functions for managing UTF-8 sequences.
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

#include "il_utils.h"
#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

unsigned long ILUTF16ReadChar(const unsigned short *str, int len, int *posn)
{
	unsigned long ch = (unsigned long)(str[*posn]);
	unsigned long low;
	if(ch < (unsigned long)0xD800 || ch > (unsigned long)0xDBFF)
	{
		/* Regular 16-bit character, or a low surrogate in the
		   wrong position for UTF-16 */
		++(*posn);
		return ch;
	}
	else if((*posn + 2) <= len &&
		    (low = (unsigned long)(str[*posn + 1])) >= (unsigned long)0xDC00 &&
			low <= (unsigned long)0xDFFF)
	{
		/* Surrogate character */
		*posn += 2;
		return ((ch - (unsigned long)0xD800) << 10) + (low & 0x03FF) +
			   (unsigned long)0x10000;
	}
	else
	{
		/* High surrogate without a low surrogate following it */
		++(*posn);
		return ch;
	}
}

unsigned long ILUTF16ReadCharAsBytes(const void *_str, int len, int *posn)
{
	const unsigned char *str = (const unsigned char *)_str;
	unsigned long ch;
	unsigned long low;
	if((*posn + 2) > len)
	{
		/* We have a character left over, which is an error.
		   But we have to do something, so return it as-is */
		ch = (unsigned long)(str[*posn]);
		++(*posn);
		return ch;
	}
	ch = (unsigned long)(IL_READ_UINT16(str + *posn));
	*posn += 2;
	if(ch < (unsigned long)0xD800 || ch > (unsigned long)0xDBFF)
	{
		/* Regular 16-bit character, or a low surrogate in the
		   wrong position for UTF-16 */
		return ch;
	}
	if((*posn + 2) > len)
	{
		/* Not enough bytes: return the high surrogate as-is */
		return ch;
	}
	low = (unsigned long)(IL_READ_UINT16(str + *posn));
	if(low < (unsigned long)0xDC00 || low > (unsigned long)0xDFFF)
	{
		/* High surrogate without a low surrogate following it */
		return ch;
	}
	*posn += 2;
	return ((ch - (unsigned long)0xD800) << 10) + (low & 0x03FF) +
		   (unsigned long)0x10000;
}

int ILUTF16WriteChar(unsigned short *buf, unsigned long ch)
{
	if(buf)
	{
		if(ch < (unsigned long)0x10000)
		{
			*buf = (unsigned short)ch;
			return 1;
		}
		else if(ch < (unsigned long)0x110000)
		{
			ch -= 0x10000;
			buf[0] = (unsigned short)((ch >> 10) + 0xD800);
			buf[1] = (unsigned short)((ch & 0x03FF) + 0xDC00);
			return 2;
		}
		else
		{
			return 0;
		}
	}
	else
	{
		if(ch < (unsigned long)0x10000)
		{
			return 1;
		}
		else if(ch < (unsigned long)0x110000)
		{
			return 2;
		}
		else
		{
			return 0;
		}
	}
}

int ILUTF16WriteCharAsBytes(void *_buf, unsigned long ch)
{
	if(_buf)
	{
		unsigned char *buf = (unsigned char *)_buf;
		if(ch < (unsigned long)0x10000)
		{
			buf[0] = (unsigned char)ch;
			buf[1] = (unsigned char)(ch >> 8);
			return 2;
		}
		else if(ch < (unsigned long)0x110000)
		{
			unsigned tempch;
			ch -= 0x10000;
			tempch = (unsigned)((ch >> 10) + 0xD800);
			buf[0] = (unsigned char)tempch;
			buf[1] = (unsigned char)(tempch >> 8);
			tempch = (unsigned)((ch & 0x03FF) + 0xDC00);
			buf[2] = (unsigned char)tempch;
			buf[3] = (unsigned char)(tempch >> 8);
			return 4;
		}
		else
		{
			return 0;
		}
	}
	else
	{
		if(ch < (unsigned long)0x10000)
		{
			return 2;
		}
		else if(ch < (unsigned long)0x110000)
		{
			return 4;
		}
		else
		{
			return 0;
		}
	}
}

#ifdef	__cplusplus
};
#endif
