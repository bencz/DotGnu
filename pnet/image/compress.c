/*
 * compress.c - Support routines for compressing metadata items.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#include "il_meta.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Note: this can handle 1, 2, 4, and 5 byte compressed
 * representations, even though Micosoft's spec and examples
 * seem to indicate that only 1, 2, and 4 are supported.  Since
 * all possible 32-bit values cannot be represented in the
 * 4-byte encoding, I am being paranoid and betting that a
 * 5-byte encoding will slip in later.  Better safe than sorry.
 */
int ILMetaCompressData(unsigned char *buf, ILUInt32 data)
{
	if(data < (ILUInt32)0x80)
	{
		buf[0] = (unsigned char)data;
		return 1;
	}
	else if(data < (ILUInt32)(1 << 14))
	{
		buf[0] = (unsigned char)((data >> 8) | 0x80);
		buf[1] = (unsigned char)data;
		return 2;
	}
	else if(data < (ILUInt32)(1L << 29))
	{
		buf[0] = (unsigned char)((data >> 24) | 0xC0);
		buf[1] = (unsigned char)(data >> 16);
		buf[2] = (unsigned char)(data >> 8);
		buf[3] = (unsigned char)data;
		return 4;
	}
	else
	{
		buf[0] = (unsigned char)0xE0;
		buf[1] = (unsigned char)(data >> 24);
		buf[2] = (unsigned char)(data >> 16);
		buf[3] = (unsigned char)(data >> 8);
		buf[4] = (unsigned char)data;
		return 5;
	}
}

int ILMetaCompressToken(unsigned char *buf, ILToken token)
{
	ILUInt32 tokenId = (token & ~IL_META_TOKEN_MASK);
	ILUInt32 tokenClass = (token & IL_META_TOKEN_MASK);
	if(tokenClass == IL_META_TOKEN_TYPE_DEF)
	{
		tokenId = (tokenId << 2) | 0x00;
	}
	else if(tokenClass == IL_META_TOKEN_TYPE_REF)
	{
		tokenId = (tokenId << 2) | 0x01;
	}
	else if(tokenClass == IL_META_TOKEN_TYPE_SPEC)
	{
		tokenId = (tokenId << 2) | 0x02;
	}
	else if(tokenClass == IL_META_TOKEN_BASE_TYPE)
	{
		tokenId = (tokenId << 2) | 0x03;
	}
	else
	{
		return 0;
	}
	return ILMetaCompressData(buf, tokenId);
}

int ILMetaCompressInt(unsigned char *buf, ILInt32 data)
{
	if(data >= 0)
	{
		if(data < (ILInt32)0x40)
		{
			buf[0] = (unsigned char)(data << 1);
			return 1;
		}
		else if(data < (ILInt32)(1 << 13))
		{
			buf[0] = (unsigned char)(((data >> 7) & 0x3F) | 0x80);
			buf[1] = (unsigned char)(data << 1);
			return 2;
		}
		else if(data < (ILInt32)(1 << 28))
		{
			buf[0] = (unsigned char)((data >> 23) | 0xC0);
			buf[1] = (unsigned char)(data >> 15);
			buf[2] = (unsigned char)(data >> 7);
			buf[3] = (unsigned char)(data << 1);
			return 4;
		}
		else
		{
			buf[0] = (unsigned char)0xE0;
			buf[1] = (unsigned char)(data >> 23);
			buf[2] = (unsigned char)(data >> 15);
			buf[3] = (unsigned char)(data >> 7);
			buf[4] = (unsigned char)(data << 1);
			return 5;
		}
	}
	else
	{
		if(data >= ((ILInt32)-0x40))
		{
			buf[0] = ((((unsigned char)(data << 1)) & 0x7E) | 0x01);
			return 1;
		}
		else if(data >= ((ILInt32)-(1 << 13)))
		{
			buf[0] = (unsigned char)(((data >> 7) & 0x3F) | 0x80);
			buf[1] = (unsigned char)((data << 1) | 0x01);
			return 2;
		}
		else if(data >= ((ILInt32)-(1 << 29)))
		{
			buf[0] = (unsigned char)(((data >> 23) & 0x1F) | 0xC0);
			buf[1] = (unsigned char)(data >> 15);
			buf[2] = (unsigned char)(data >> 7);
			buf[3] = (unsigned char)((data << 1) | 0x01);
			return 4;
		}
		else
		{
			buf[0] = (unsigned char)0xE1;
			buf[1] = (unsigned char)(data >> 23);
			buf[2] = (unsigned char)(data >> 15);
			buf[3] = (unsigned char)(data >> 7);
			buf[4] = (unsigned char)((data << 1) | 0x01);
			return 5;
		}
	}
}

#ifdef	__cplusplus
};
#endif
