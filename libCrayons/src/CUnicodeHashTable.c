/*
 * CUnicodeHashTable.c - Unicode hash table implementation.
 *
 * Copyright (C) 2006  Free Software Foundation, Inc.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#include "CUnicodeHashTable.h"
#include "CUtils.h"
#include <fontconfig/fcfreetype.h>

#ifdef __cplusplus
extern "C" {
#endif

/* Structure of a unicode hash entry. */
typedef struct _tagCUnicodeHashEntry CUnicodeHashEntry;
struct _tagCUnicodeHashEntry
{
	CChar32     unicode;
	CGlyphIndex glyph;
};

/* Structure of a unicode hash table. */
struct _tagCUnicodeHashTable
{
	CUnicodeHashEntry *entries;
	CUInt32            hash;
	CUInt32            rehash;
	FcCharSet         *charset;
	FT_Face            face;
};

/* Missing entry marker. */
#define CUnicodeHashEntry_MissingUnicode ((CChar32)-1)

/*\
|*| Create a unicode hash table.
|*|
|*|     _this - this unicode hash table (created)
|*|   charset - fontconfig character set
|*|      face - freetype face
|*|
|*|  NOTE: the table takes ownership of the character set
|*|        if construction is successful, but does NOT
|*|        assume ownership of the face, and requires
|*|        that the face continues to be valid for the
|*|        life of the table
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CUnicodeHashTable_Create(CUnicodeHashTable **_this,
                         FcCharSet          *charset,
                         FT_Face             face)
{
	/* declarations */
	CUnicodeHashTable *ht;
	CUInt32            size;
	CUInt32            rehash;
	CUInt32            hash;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((charset != 0));

	/* calculate the hash and rehash value */
	rehash = CUtils_NextTwinPrime(FcCharSetCount(charset) + 1);
	hash   = (rehash + 2);

	/*\
	|*| NOTE: the maximum character set count should be 2^20
	|*|       plus 2^16, for full unicode coverage, and so
	|*|       1200359 is the largest twin prime which should
	|*|       be returned by the twin prime search
	\*/

	/* ensure we have a reasonably sized character set */
	CASSERT((rehash != 0));
	CASSERT((rehash <= 1200359));

	/* calculate the size of the table */
	size = (sizeof(CUnicodeHashTable) + (hash * sizeof(CUnicodeHashEntry)));

	/* allocate the table */
	if(!(*_this = (CUnicodeHashTable *)CMalloc(size)))
	{
		return CStatus_OutOfMemory;
	}

	/* get the hash table */
	ht = *_this;

	/* initialize the entry list */
	{
		/* declarations */
		CUnicodeHashEntry *curr;
		CUnicodeHashEntry *end;

		/* get the entry list */
		ht->entries = ((CUnicodeHashEntry *)(_this + 1));

		/* get the current entry pointer */
		curr = ht->entries;

		/* get the end pointer */
		end = (curr + hash);

		/* initialize the entries */
		while(curr != end)
		{
			/* initialize the current entry */
			curr->unicode = CUnicodeHashEntry_MissingUnicode;
			curr->glyph   = 0;

			/* move to the next entry */
			++curr;
		}
	}

	/* initialize the remaining members */
	ht->hash    = hash;
	ht->rehash  = rehash;
	ht->charset = charset;
	ht->face    = face;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Destroy a unicode hash table.
|*|
|*|   _this - this unicode hash table
\*/
CINTERNAL void
CUnicodeHashTable_Destroy(CUnicodeHashTable **_this)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* destroy the character set */
	FcCharSetDestroy((*_this)->charset);

	/* free the table */
	CFree(*_this);

	/* null the table */
	*_this = 0;
}

/*\
|*| Get a glyph index for a unicode character.
|*|
|*|     _this - this unicode hash table
|*|   unicode - unicode character (ucs4 encoding)
|*|
|*|  Returns glyph index.
\*/
CINTERNAL CGlyphIndex
CUnicodeHashTable_GetGlyphIndex(CUnicodeHashTable *_this,
                                CChar32            unicode)
{
	/* declarations */
	CUnicodeHashEntry *entry;
	CUnicodeHashEntry *end;
	CUInt32            offset;
	CUInt32            index;
	CUInt32            length;

	/* assertions */
	CASSERT((_this != 0));

	/* get the table length */
	length = _this->hash;

	/* calculate the entry reference */
	entry = (_this->entries + (unicode % length));

	/* handle the trivial case */
	if(entry->unicode == unicode) { return entry->glyph; }

	/* get the end pointer */
	end = (_this->entries + length);

	/* calculate the offset */
	offset = (unicode % _this->rehash);

	/* ensure the offset is non-zero */
	if(offset == 0) { offset = 1; }

	/* find or add the entry */
	for(index = 0; index < length; ++index)
	{
		/* bail out now if we have a match */
		if(entry->unicode == unicode) { return entry->glyph; }

		/* add a new entry to the table as needed */
		if(entry->unicode == CUnicodeHashEntry_MissingUnicode)
		{
			/* add the entry if possible */
			if(FcCharSetHasChar(_this->charset, unicode))
			{
				/* set the character of the entry */
				entry->unicode = unicode;

				/* set the glyph index of the entry */
				entry->glyph = FcFreeTypeCharIndex(_this->face, unicode);
			}

			/* we're done here */
			break;
		}

		/* update the entry reference */
		entry += offset;

		/* wrap around the table as needed */
		if(entry >= end) { entry -= length; }
	}

	/* return the glyph index */
	return entry->glyph;
}


#ifdef __cplusplus
};
#endif
