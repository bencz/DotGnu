/*
 * uncompress.c - Support routines for uncompressing metadata items.
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

#include "il_values.h"
#include "il_meta.h"

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
ILUInt32 ILMetaUncompressData(ILMetaDataRead *meta)
{
	unsigned char ch;
	unsigned char ch2;
	unsigned char ch3;
	unsigned char ch4;

	if(meta->len > 0)
	{
		ch = *((meta->data)++);
		--(meta->len);
		if(ch < (unsigned char)0x80)
		{
			/* Single-byte form of the item */
			return (ILUInt32)ch;
		}
		else if((ch & 0xC0) == 0x80)
		{
			/* Two-byte form of the item */
			if(meta->len > 0)
			{
				--(meta->len);
				return (((ILUInt32)(ch & 0x3F)) << 8) |
					    ((ILUInt32)(*((meta->data)++)));
			}
			else
			{
				meta->error = 1;
				return 0;
			}
		}
		else if((ch & 0xE0) == 0xC0)
		{
			/* Four-byte form of the item */
			if(meta->len >= 3)
			{
				ch2 = meta->data[0];
				ch3 = meta->data[1];
				ch4 = meta->data[2];
				meta->len -= 3;
				meta->data += 3;
				return (((ILUInt32)(ch & 0x1F)) << 24) |
					   (((ILUInt32)ch2) << 16) |
					   (((ILUInt32)ch3) << 8) |
					    ((ILUInt32)ch4);
			}
			else
			{
				meta->len = 0;
				meta->error = 1;
				return 0;
			}
		}
		else if((ch & 0xF0) == 0xE0)
		{
			/* Five-byte form of the item */
			if(meta->len >= 4)
			{
				ch  = meta->data[0];
				ch2 = meta->data[1];
				ch3 = meta->data[2];
				ch4 = meta->data[3];
				meta->len -= 4;
				meta->data += 4;
				return (((ILUInt32)ch) << 24) |
					   (((ILUInt32)ch2) << 16) |
					   (((ILUInt32)ch3) << 8) |
					    ((ILUInt32)ch4);
			}
			else
			{
				meta->len = 0;
				meta->error = 1;
				return 0;
			}
		}
		else
		{
			/* Unknown encoding */
			meta->len = 0;
			meta->error = 1;
			return 0;
		}
	}
	else
	{
		meta->error = 1;
		return 0;
	}
}

ILToken ILMetaUncompressToken(ILMetaDataRead *meta)
{
	ILUInt32 item;

	item = ILMetaUncompressData(meta);
	if(!(meta->error))
	{
		ILUInt32 type = (item & 0x03);

		switch(type)
		{
			case 0x00:
			{
				return (ILToken)((item >> 2) | IL_META_TOKEN_TYPE_DEF);
			}
			break;

			case 0x01:
			{
				return (ILToken)((item >> 2) | IL_META_TOKEN_TYPE_REF);
			}
			break;

			case 0x02:
			{
				return (ILToken)((item >> 2) | IL_META_TOKEN_TYPE_SPEC);
			}
			break;

			default:
			{
				return (ILToken)((item >> 2) | IL_META_TOKEN_BASE_TYPE);
			}
		}
	}
	else
	{
		return 0;
	}
}

ILInt32 ILMetaUncompressInt(ILMetaDataRead *meta)
{
	if(meta->len > 0)
	{
		unsigned char ch;
		unsigned char ch2;
		unsigned char ch3;
		unsigned char ch4;
		ILUInt32 value;

		ch = *((meta->data)++);
		--(meta->len);
		if((ch & 0x80) == 0x00)
		{
			/* One-byte form of the item */
			if((ch & 0x01) == 0x00)
				return (ILInt32)(ch >> 1);
			else
				return (ILInt32)(signed char)((ch >> 1) | 0xC0);
		}
		else if((ch & 0xC0) == 0x80)
		{
			/* Two-byte form of the item */
			if(meta->len > 0)
			{
				--(meta->len);
				value = (((ILUInt32)(ch & 0x3F)) << 8) |
					     ((ILUInt32)(*((meta->data)++)));
				if((value & 0x01) == 0x00)
					return (ILInt32)(value >> 1);
				else
					return (ILInt32)((value >> 1) | 0xFFFFE000);
			}
			else
			{
				meta->error = 1;
				return 0;
			}
		}
		else if((ch & 0xE0) == 0xC0)
		{
			/* Four-byte form of the item */
			if(meta->len >= 3)
			{
				ch2 = meta->data[0];
				ch3 = meta->data[1];
				ch4 = meta->data[2];
				meta->len -= 3;
				meta->data += 3;
				value = (((ILUInt32)(ch & 0x1F)) << 24) |
					    (((ILUInt32)ch2) << 16) |
					    (((ILUInt32)ch3) << 8) |
					     ((ILUInt32)ch4);
				if((value & 0x01) == 0x00)
					return (ILInt32)(value >> 1);
				else
					return (ILInt32)((value >> 1) | 0xF0000000);
			}
			else
			{
				meta->len = 0;
				meta->error = 1;
				return 0;
			}
		}
		else
		{
			/* Five-byte form of the item */
			if(meta->len >= 4)
			{
				ch  = meta->data[0];
				ch2 = meta->data[1];
				ch3 = meta->data[2];
				ch4 = meta->data[3];
				meta->len -= 4;
				meta->data += 4;
				value = (((ILUInt32)ch) << 24) |
					    (((ILUInt32)ch2) << 16) |
					    (((ILUInt32)ch3) << 8) |
					     ((ILUInt32)ch4);
				return (ILInt32)value;
			}
			else
			{
				meta->len = 0;
				meta->error = 1;
				return 0;
			}
		}
	}
	else
	{
		meta->error = 1;
		return 0;
	}
}

#ifdef	__cplusplus
};
#endif
