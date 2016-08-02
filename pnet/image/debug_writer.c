/*
 * debug_writer.c - Write debug informtion to an image.
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

#include "program.h"

#ifdef IL_USE_WRITER

#ifdef	__cplusplus
extern	"C" {
#endif

#define	IL_DEBUG_HEADER_SIZE	24

typedef struct
{
	unsigned long	offset;
	char			name[1];

} ILDebugHashItem;

static unsigned long Debug_ComputeFunc(const void *elem)
{
	return ILHashString(0, ((ILDebugHashItem *)elem)->name, -1);
}

static unsigned long Debug_KeyComputeFunc(const void *key)
{
	return ILHashString(0, (const char *)key, -1);
}

static int Debug_MatchFunc(const void *elem, const void *key)
{
	return !strcmp(((ILDebugHashItem *)elem)->name, (const char *)key);
}

ILUInt32 ILWriterDebugString(ILWriter *writer, const char *str)
{
	ILUInt32 offset;
	ILDebugHashItem *item;

	/* Search the hash table for an identical string */
	if(writer->debugHash)
	{
		item = ILHashFindType(writer->debugHash, str, ILDebugHashItem);
		if(item)
		{
			return item->offset;
		}
	}

	/* Add the string to the end of the "debugStrings" buffer */
	offset = writer->debugStrings.offset;
	if(!_ILWBufferListAdd(&(writer->debugStrings), str, strlen(str) + 1))
	{
		writer->outOfMemory = 1;
		return offset;
	}

	/* Add the string to the hash table */
	if(!(writer->debugHash))
	{
		writer->debugHash = ILHashCreate(0, Debug_ComputeFunc,
										 Debug_KeyComputeFunc,
										 Debug_MatchFunc, ILFree);
		if(!(writer->debugHash))
		{
			writer->outOfMemory = 1;
			return offset;
		}
	}
	if((item = (ILDebugHashItem *)ILMalloc(sizeof(ILDebugHashItem) +
										   strlen(str))) == 0)
	{
		writer->outOfMemory = 1;
		return offset;
	}
	item->offset = offset;
	strcpy(item->name, str);
	if(!ILHashAdd(writer->debugHash, item))
	{
		ILFree(item);
		writer->outOfMemory = 1;
		return offset;
	}

	/* Return the string offset to the caller */
	return offset;
}

static void WriterDebugAdd(ILWriter *writer, ILProgramItem *item,
						   unsigned long pseudo, int type,
					       const void *info, ILUInt32 len)
{
	unsigned char header[IL_META_COMPRESS_MAX_SIZE * 2];
	int posn;
	unsigned long offset;
	ILDebugToken *newTokens;

	/* Write the debug data to the section */
	posn = ILMetaCompressData(header, (ILUInt32)type);
	posn += ILMetaCompressData(header + posn, len);
	offset = writer->debugData.offset + IL_DEBUG_HEADER_SIZE;
	if(!_ILWBufferListAdd(&(writer->debugData), header, posn) ||
	   !_ILWBufferListAdd(&(writer->debugData), info, len))
	{
		writer->outOfMemory = 1;
	}

	/* Create a debug token index record */
	if(writer->numDebugTokens < writer->maxDebugTokens)
	{
		writer->debugTokens[writer->numDebugTokens].item = item;
		writer->debugTokens[writer->numDebugTokens].pseudo = pseudo;
		writer->debugTokens[writer->numDebugTokens].offset = offset;
		++(writer->numDebugTokens);
	}
	else
	{
		newTokens = (ILDebugToken *)ILRealloc
				(writer->debugTokens,
				 (writer->maxDebugTokens + 256) * sizeof(ILDebugToken));
		if(!newTokens)
		{
			writer->outOfMemory = 1;
		}
		else
		{
			writer->debugTokens = newTokens;
			writer->maxDebugTokens += 256;
			newTokens[writer->numDebugTokens].item = item;
			newTokens[writer->numDebugTokens].pseudo = pseudo;
			newTokens[writer->numDebugTokens].offset = offset;
			++(writer->numDebugTokens);
		}
	}
}

void ILWriterDebugAdd(ILWriter *writer, ILProgramItem *item, int type,
					  const void *info, ILUInt32 len)
{
	WriterDebugAdd(writer, item, 0, type, info, len);
}

void ILWriterDebugAddPseudo(ILWriter *writer, unsigned long token, int type,
					  		const void *info, ILUInt32 len)
{
	WriterDebugAdd(writer, 0, token, type, info, len);
}

/*
 * Write the contents of a buffer list to the ".ildebug" section.
 */
static void WriteListToDebug(ILWriter *writer, ILWBufferList *list)
{
	ILWBuffer *buffer = list->firstBuffer;
	while(buffer != 0)
	{
		if(buffer->next)
		{
			ILWriterOtherWrite(writer, ".ildebug", IL_IMAGESECT_DEBUG,
							   buffer->data, IL_WRITE_BUFFER_SIZE);
		}
		else
		{
			ILWriterOtherWrite(writer, ".ildebug", IL_IMAGESECT_DEBUG,
							   buffer->data, list->bytesUsed);
		}
		buffer = buffer->next;
	}
}

#ifdef HAVE_QSORT

/*
 * Compare two debug token index entries.
 */
static int DebugIndexCompare(const void *e1, const void *e2)
{
	unsigned long token1;
	unsigned long token2;
	if(((ILDebugToken *)e1)->item)
	{
		token1 = ((ILDebugToken *)e1)->item->token;
	}
	else
	{
		token1 = ((ILDebugToken *)e1)->pseudo;
	}
	if(((ILDebugToken *)e2)->item)
	{
		token2 = ((ILDebugToken *)e2)->item->token;
	}
	else
	{
		token2 = ((ILDebugToken *)e2)->pseudo;
	}
	if(token1 < token2)
	{
		return -1;
	}
	else if(token1 > token2)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

#endif

void _ILWriteDebug(ILWriter *writer)
{
	unsigned char header[IL_DEBUG_HEADER_SIZE];
	unsigned long posn;
	unsigned long value;
	unsigned long adjust;

	/* The string table must not be empty */
	if(!(writer->debugStrings.offset))
	{
		header[0] = (unsigned char)0x00;
		_ILWBufferListAdd(&(writer->debugStrings), header, 1);
	}

	/* Construct the debug section header */
	header[0] = 'I';						/* Magic number */
	header[1] = 'L';
	header[2] = 'D';
	header[3] = 'B';
	IL_WRITE_UINT32(header + 4, 1);			/* Version */
	value = IL_DEBUG_HEADER_SIZE + writer->debugData.offset +
			writer->debugStrings.offset;
	if((value & 3) != 0)
	{
		adjust = 4 - (value & 3);
		value += adjust;
	}
	else
	{
		adjust = 0;
	}
	IL_WRITE_UINT32(header + 8, value);		/* Offset of token index */
	value = writer->numDebugTokens;
	IL_WRITE_UINT32(header + 12, value);	/* Number of index entries */
	value = IL_DEBUG_HEADER_SIZE + writer->debugData.offset;
	IL_WRITE_UINT32(header + 16, value);	/* Offset of string table */
	value = writer->debugStrings.offset;
	IL_WRITE_UINT32(header + 20, value);	/* Length of string table */

	/* Write the debug section header to the ".ildebug" section */
	ILWriterOtherWrite(writer, ".ildebug", IL_IMAGESECT_DEBUG,
					   header, IL_DEBUG_HEADER_SIZE);

	/* Write the debug data to the ".ildebug" section */
	WriteListToDebug(writer, &(writer->debugData));

	/* Write the string table to the ".ildebug" section */
	WriteListToDebug(writer, &(writer->debugStrings));

	/* Align the ".ildebug" section on a 4-byte boundary */
	if(adjust > 0)
	{
		ILMemZero(header, sizeof(header));
		ILWriterOtherWrite(writer, ".ildebug", IL_IMAGESECT_DEBUG,
						   header, adjust);
	}

	/* Sort the token index */
#ifdef HAVE_QSORT
	qsort(writer->debugTokens, writer->numDebugTokens,
		  sizeof(ILDebugToken), DebugIndexCompare);
#else
	{
		/* We don't have "qsort", so use a simple sorting algorithm */
		unsigned long posn2;
		unsigned long temp;
		unsigned long token1;
		unsigned long token2;
		ILProgramItem *tempItem;
		for(posn = 0; posn < (writer->numDebugTokens - 1); ++posn)
		{
			for(posn2 = (posn + 1); posn2 < writer->numDebugTokens; ++posn2)
			{
				if(writer->debugTokens[posn].item)
				{
					token1 = writer->debugTokens[posn].item->token;
				}
				else
				{
					token1 = writer->debugTokens[posn].pseudo;
				}
				if(writer->debugTokens[posn2].item)
				{
					token2 = writer->debugTokens[posn2].item->token;
				}
				else
				{
					token2 = writer->debugTokens[posn2].pseudo;
				}
				if(token1 > token2)
				{
					tempItem = writer->debugTokens[posn].item;
					writer->debugTokens[posn].item =
						writer->debugTokens[posn2].item;
					writer->debugTokens[posn2].item = tempItem;
					temp = writer->debugTokens[posn].pseudo;
					writer->debugTokens[posn].pseudo =
						writer->debugTokens[posn2].pseudo;
					writer->debugTokens[posn2].pseudo = temp;
					temp = writer->debugTokens[posn].offset;
					writer->debugTokens[posn].offset =
						writer->debugTokens[posn2].offset;
					writer->debugTokens[posn2].offset = temp;
				}
			}
		}
	}
#endif

	/* Write the token index to the ".ildebug" section */
	for(posn = 0; posn < writer->numDebugTokens; ++posn)
	{
		if(writer->debugTokens[posn].item)
		{
			value = writer->debugTokens[posn].item->token;
		}
		else
		{
			value = writer->debugTokens[posn].pseudo;
		}
		IL_WRITE_UINT32(header, value);
		value = writer->debugTokens[posn].offset;
		IL_WRITE_UINT32(header + 4, value);
		ILWriterOtherWrite(writer, ".ildebug", IL_IMAGESECT_DEBUG, header, 8);
	}
}

#ifdef	__cplusplus
};
#endif

#endif /* IL_USE_WRITER */
