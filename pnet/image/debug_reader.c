/*
 * debug_reader.c - Read debug symbol information from within an IL binary.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

#include "image.h"
#include "il_debug.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Internal structure of debug contexts.
 */
struct _tagILDebugContext
{
	ILImage				   *image;
	const unsigned char    *section;
	ILUInt32				sectionLen;
	const unsigned char    *index;
	unsigned long			indexSize;
	const unsigned char    *strings;
	unsigned long			stringsLen;

};

int ILDebugPresent(ILImage *image)
{
	return (image->debugRVA != 0);
}

ILDebugContext *ILDebugCreate(ILImage *image)
{
	ILDebugContext *dbg;
	unsigned long offset;
	unsigned long len;

	/* Create the debug context */
	if((dbg = (ILDebugContext *)ILMalloc(sizeof(ILDebugContext))) == 0)
	{
		return 0;
	}

	/* Initialize the context fields */
	dbg->image = image;
	dbg->index = 0;
	dbg->indexSize = 0;
	dbg->strings = 0;
	dbg->stringsLen = 0;

	/* Find the debug section */
	if(!ILImageGetSection(image, IL_SECTION_DEBUG,
						  (void **)&(dbg->section), &(dbg->sectionLen)))
	{
		dbg->section = 0;
		dbg->sectionLen = 0;
	}

	/* Decode and validate the debug section header */
	if(dbg->sectionLen < 24 ||						/* Header length */
	   ILMemCmp(dbg->section, "ILDB", 4) != 0 ||	/* Magic */
	   IL_READ_UINT32(dbg->section + 4) != 1)		/* Version */
	{
		dbg->section = 0;
		dbg->sectionLen = 0;
	}
	else
	{
		/* Validate the offset and length of the index table */
		offset = IL_READ_UINT32(dbg->section + 8);
		if(offset < dbg->sectionLen)
		{
			len = IL_READ_UINT32(dbg->section + 12);
			if(len < (unsigned long)0x20000000 &&
			   (dbg->sectionLen - offset) >= len * 8)
			{
				dbg->index = dbg->section + offset;
				dbg->indexSize = len;
			}
		}

		/* Validate the offset and length of the string table */
		offset = IL_READ_UINT32(dbg->section + 16);
		if(offset < dbg->sectionLen)
		{
			len = IL_READ_UINT32(dbg->section + 20);
			if(len > 0 &&
			   (dbg->sectionLen - offset) >= len &&
			   dbg->section[offset + len - 1] == (unsigned char)'\0')
			{
				dbg->strings = dbg->section + offset;
				dbg->stringsLen = len;
			}
		}
	}

	/* Ready to go */
	return dbg;
}

void ILDebugDestroy(ILDebugContext *dbg)
{
	ILFree(dbg);
}

ILImage *ILDebugToImage(ILDebugContext *dbg)
{
	return dbg->image;
}

ILToken ILDebugGetPseudo(const char *name)
{
	ILUInt32 token = 0;
	int shift = 0;
	while(*name != '\0')
	{
		token |= (((ILUInt32)(*name++)) << shift);
		shift += 8;
	}
	return (token | (ILUInt32)0x80000000);
}

const char *ILDebugGetString(ILDebugContext *dbg, ILUInt32 offset)
{
	if(offset < dbg->stringsLen)
	{
		return (const char *)dbg->strings + offset;
	}
	else
	{
		return 0;
	}
}

void ILDebugIterInit(ILDebugIter *iter, ILDebugContext *dbg, ILToken token)
{
	long left;
	long right;
	long middle;
	ILToken testToken;

	/* Find an entry for the token using binary search */
	left = 0;
	right = ((long)(dbg->indexSize)) - 1;
	middle = 0;
	while(left <= right)
	{
		middle = (left + right) / 2;
		testToken = IL_READ_UINT32(dbg->index + middle * 8);
		if(testToken == token)
		{
			break;
		}
		else if(testToken < token)
		{
			left = middle + 1;
		}
		else
		{
			right = middle - 1;
		}
	}
	if(left > right)
	{
		/* There is no debug information for the token */
		iter->dbg = dbg;
		iter->reserved1 = 0;
		iter->reserved2 = 0;
		return;
	}

	/* Find the first and last entries for the token using linear search */
	left = middle;
	while(left > 0)
	{
		testToken = IL_READ_UINT32(dbg->index + left * 8 - 8);
		if(testToken != token)
		{
			break;
		}
		--left;
	}
	right = middle;
	while(right < (dbg->indexSize - 1))
	{
		testToken = IL_READ_UINT32(dbg->index + right * 8 + 8);
		if(testToken != token)
		{
			break;
		}
		++right;
	}

	/* Set up the iterator and return */
	iter->dbg = dbg;
	iter->reserved1 = (unsigned long)left;
	iter->reserved2 = (unsigned long)(right + 1);
}

int ILDebugIterNext(ILDebugIter *iter)
{
	unsigned long offset;
	ILMetaDataRead reader;

	/* Find the next token information block that is formatted correctly */
	for(;iter->reserved1 < iter->reserved2; ++(iter->reserved1))
	{
		/* Fetch the data offset and validate it */
		offset = IL_READ_UINT32(iter->dbg->index + iter->reserved1 * 8 + 4);
		if(offset >= iter->dbg->sectionLen)
		{
			continue;
		}

		/* Read the token information block */
		reader.data = iter->dbg->section + offset;
		reader.len = iter->dbg->sectionLen - offset;
		reader.error = 0;
		iter->type = ILMetaUncompressData(&reader);
		iter->length = ILMetaUncompressData(&reader);
		iter->data = (const void *)(reader.data);
		if(reader.error || iter->length > reader.len)
		{
			continue;
		}

		/* Advance to the next block and return */
		++(iter->reserved1);
		return 1;
	}

	/* If we get here, then we don't have any more blocks for this token */
	return 0;
}

const char *ILDebugGetLineInfo(ILDebugContext *dbg, ILToken token,
						       ILUInt32 offset, ILUInt32 *line,
						       ILUInt32 *column)
{
	ILDebugIter iter;
	ILMetaDataRead reader;
	ILUInt32 lowOffset = 0;
	const char *lowFilename = 0;
	ILUInt32 lowLine = 0;
	ILUInt32 lowColumn = 0;
	ILUInt32 tempOffset;
	const char *tempFilename;
	ILUInt32 tempLine;
	ILUInt32 tempColumn;

	/* Find the highest block that is lower than the specified offset.
	   If there are multiple lines for the same offset then return
	   the highest line number.  Multiple lines can happen when nested
	   blocks in the original source all begin on the same offset */
	ILDebugIterInit(&iter, dbg, token);
	while(ILDebugIterNext(&iter))
	{
		/* Skip unknown debug types */
		if(iter.type != IL_DEBUGTYPE_LINE_COL &&
		   iter.type != IL_DEBUGTYPE_LINE_OFFSETS &&
		   iter.type != IL_DEBUGTYPE_LINE_COL_OFFSETS)
		{
			continue;
		}

		/* Read the filename string */
		reader.data = iter.data;
		reader.len = iter.length;
		reader.error = 0;
		tempFilename = ILDebugGetString(dbg, ILMetaUncompressData(&reader));
		if(!tempFilename || reader.error)
		{
			continue;
		}

		/* Determine how to decode the rest of the block */
		if(iter.type == IL_DEBUGTYPE_LINE_COL)
		{
			/* Line and column information */
			while(reader.len > 0)
			{
				tempLine = ILMetaUncompressData(&reader);
				tempColumn = ILMetaUncompressData(&reader);
				tempOffset = 0;
				if(reader.error)
				{
					break;
				}
				if(tempOffset <= offset && tempOffset >= lowOffset)
				{
					if(!lowFilename || tempLine > lowLine)
					{
						lowFilename = tempFilename;
						lowOffset = tempOffset;
						lowLine = tempLine;
						lowColumn = tempColumn;
					}
				}
			}
		}
		else if(iter.type == IL_DEBUGTYPE_LINE_OFFSETS)
		{
			/* Line and offset information */
			while(reader.len > 0)
			{
				tempLine = ILMetaUncompressData(&reader);
				tempColumn = 0;
				tempOffset = ILMetaUncompressData(&reader);
				if(reader.error)
				{
					break;
				}
				if(tempOffset <= offset && tempOffset >= lowOffset)
				{
					if(!lowFilename || tempLine > lowLine)
					{
						lowFilename = tempFilename;
						lowOffset = tempOffset;
						lowLine = tempLine;
						lowColumn = tempColumn;
					}
				}
			}
		}
		else
		{
			/* Line, column, and offset information */
			while(reader.len > 0)
			{
				tempLine = ILMetaUncompressData(&reader);
				tempColumn = ILMetaUncompressData(&reader);
				tempOffset = ILMetaUncompressData(&reader);
				if(reader.error)
				{
					break;
				}
				if(tempOffset <= offset && tempOffset >= lowOffset)
				{
					if(!lowFilename || tempLine > lowLine)
					{
						lowFilename = tempFilename;
						lowOffset = tempOffset;
						lowLine = tempLine;
						lowColumn = tempColumn;
					}
				}
			}
		}
	}

	/* Return the final information to the caller */
	*line = lowLine;
	*column = lowColumn;
	return lowFilename;
}

const char *ILDebugGetVarName(ILDebugContext *dbg, ILToken token,
							  ILUInt32 offset, ILUInt32 varNum)
{
	ILDebugIter iter;
	ILMetaDataRead reader;
	const char *name = 0;
	ILUInt32 scopeStart = 0;
	ILUInt32 scopeEnd = IL_MAX_UINT32;
	ILUInt32 tempStart;
	ILUInt32 tempEnd;
	ILUInt32 tempName;
	ILUInt32 tempIndex;

	/* Find the innermost scope block that contains the variable */
	ILDebugIterInit(&iter, dbg, token);
	while(ILDebugIterNext(&iter))
	{
		/* Skip unknown debug types */
		if(iter.type != IL_DEBUGTYPE_VARS &&
		   iter.type != IL_DEBUGTYPE_VARS_OFFSETS)
		{
			continue;
		}

		/* Get the scope information for this block */
		reader.data = iter.data;
		reader.len = iter.length;
		reader.error = 0;
		if(iter.type == IL_DEBUGTYPE_VARS_OFFSETS)
		{
			tempStart = ILMetaUncompressData(&reader);
			tempEnd = ILMetaUncompressData(&reader);
			if(reader.error)
			{
				continue;
			}
			if(tempStart > tempEnd)
			{
				tempEnd = tempStart;
			}
		}
		else
		{
			tempStart = 0;
			tempEnd = IL_MAX_UINT32;
		}

		/* Read the name and variable index information */
		while(reader.len > 0)
		{
			tempName = ILMetaUncompressData(&reader);
			tempIndex = ILMetaUncompressData(&reader);
			if(reader.error)
			{
				break;
			}
			if(tempIndex != varNum)
			{
				continue;
			}
			if(tempStart >= scopeStart && tempEnd <= scopeEnd)
			{
				scopeStart = tempStart;
				scopeEnd = tempEnd;
				if(offset >= scopeStart && offset < scopeEnd)
				{
					name = ILDebugGetString(dbg, tempName);
				}
			}
		}
	}

	/* Return the innermost name that we found */
	return name;
}

#ifdef	__cplusplus
};
#endif
