/*
 * CHashTable.c - Hash table implementation.
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

#include "CHashTable.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Structure of a hash table layout. */
typedef struct _tagCHashTableLayout CHashTableLayout;
struct _tagCHashTableLayout
{
	CUInt32 lwm;
	CUInt32 hwm;
	CUInt32 hash;
	CUInt32 rehash;
};

/* Structure of a hash table. */
struct _tagCHashTable
{
	const CHashTableLayout  *layout;
	CPredicateBinary        *keysEqual;
	CHashEntry             **entries;
	CUInt32                  count;
};

/*\
|*| NOTE: The hash values are the smallest twin prime pairs
|*|       greater than four times the low watermarks, and
|*|       the high watermarks are three times the low
|*|       watermarks. This should ensure that the tables
|*|       are always somewhere between 25% and 75% full.
\*/

/* List of hash table layouts. */
static const CHashTableLayout CHashTable_Layouts[] =
{
	{        4,        12,        19,        17 },
	{        8,        24,        43,        41 },
	{       16,        48,        73,        71 },
	{       32,        96,       139,       137 },
	{       64,       192,       271,       269 },
	{      128,       384,       523,       521 },
	{      256,       768,      1033,      1031 },
	{      512,      1536,      2083,      2081 },
	{     1024,      3072,      4129,      4127 },
	{     2048,      6144,      8221,      8219 },
	{     4096,     12288,     16453,     16451 },
	{     8192,     24576,     32803,     32801 },
	{    16384,     49152,     65539,     65537 },
	{    32768,     98304,    131113,    131111 },
	{    65536,    196608,    262153,    262151 },
	{   131072,    393216,    524353,    524351 },
	{   262144,    786432,   1048891,   1048889 },
	{   524288,   1572864,   2097259,   2097257 },
	{  1048576,   3145728,   4194583,   4194581 },
	{  2097152,   6291456,   8388619,   8388617 },
	{  4194304,  12582912,  16777291,  16777289 },
	{  8388608,  25165824,  33554503,  33554501 },
	{ 16777216,  50331648,  67109323,  67109321 },
	{ 33554432, 100663296, 134217781, 134217779 },
	{ 67108864, 201326592, 268435579, 268435577 }
};

/* Layout table length. */
#define CHashTable_LayoutsLength \
	(sizeof(CHashTable_Layouts) / sizeof(CHashTableLayout))

/* Handle to last layout. */
#define CHashTable_Layouts_Last \
	((CHashTable_Layouts + CHashTable_LayoutsLength) - 1)

/* Target of dead entry marker. */
static const CHashEntry CHashEntry_DeadEntryObject = { 0 };

/* Null and dead entry handles. */
#define CHashTable_NullEntry ((CHashEntry *)0)
#define CHashTable_DeadEntry ((CHashEntry *)&CHashEntry_DeadEntryObject)

/* Indexed entry status tests. */
#define CHashTable_IsNullEntry(_this, i) \
	(CHashEntry_IsNull((_this)->entries[(i)]))
#define CHashTable_IsDeadEntry(_this, i) \
	(CHashEntry_IsDead((_this)->entries[(i)]))
#define CHashTable_IsLiveEntry(_this, i) \
	(CHashEntry_IsLive((_this)->entries[(i)]))

/* Direct entry status tests. */
#define CHashEntry_IsNull(entry) ((entry) == CHashTable_NullEntry)
#define CHashEntry_IsDead(entry) ((entry) == CHashTable_DeadEntry)
#define CHashEntry_IsLive(entry) \
	(!CHashEntry_IsNull((entry)) && !CHashEntry_IsDead((entry)))

/*\
|*| Allocate a hash entry list.
|*|
|*|   length - the number of entries to accommodate
|*|
|*|  Returns new hash entry list, or null on failure.
\*/
#define CHashTable_AllocEntries(length) \
	((CHashEntry **)CCalloc(1, ((length) * sizeof(CHashEntry *))))

/*\
|*| Create a hash table.
|*|
|*|       _this - this hash table (created)
|*|   keysEqual - key equality predicate
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CHashTable_Create(CHashTable       **_this,
                  CPredicateBinary  *keysEqual)
{
	/* declarations */
	CHashTable *table;

	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((keysEqual != 0));

	/* allocate the table */
	if(!(*_this = CMalloc(sizeof(CHashTable))))
	{
		return CStatus_OutOfMemory;
	}

	/* get the table pointer */
	table = *_this;

	/* initialize the table layout */
	table->layout = CHashTable_Layouts;

	/* allocate the entry list */
	if(!(table->entries = CHashTable_AllocEntries(table->layout->hash)))
	{
		CFree(*_this);
		*_this = 0;
		return CStatus_OutOfMemory;
	}

	/* initialize the key equality operator */
	table->keysEqual = keysEqual;

	/* initialize the live entry count */
	table->count = 0;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Destroy a hash table.
|*|
|*|          _this - this hash table
|*|   destroyEntry - entry destruction operator (optional)
\*/
CINTERNAL void
CHashTable_Destroy(CHashTable     **_this,
                   COperatorUnary  *destroyEntry)
{
	/* declarations */
	CHashTable *table;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* get the table pointer */
	table = *_this;

	/* empty the table as needed */
	if(destroyEntry && table->count != 0)
	{
		/* declarations */
		CHashEntry **curr;
		CHashEntry **end;
		CUInt32      count;

		/* get the current entry pointer */
		curr = table->entries;

		/* get the end pointer */
		end = (curr + table->layout->hash);

		/* get the live entry count */
		count = table->count;

		/* remove all the live entries */
		while(curr != end)
		{
			/* remove the current entry, if it's live */
			if(CHashEntry_IsLive(*curr))
			{
				/* destroy the entry */
				COperator_Unary(destroyEntry, *curr);

				/* mark the entry as dead, as needed */
				*curr = CHashTable_DeadEntry;

				/* update the live entry count */
				--count;

				/* stop processing live entries, if there are none left */
				if(count == 0) { break; }
			}

			/* update the current entry pointer */
			++curr;
		}
	}

	/* free the entry list */
	CFree(table->entries);

	/* free the hash table */
	CFree(table);

	/* null the hash table */
	*_this = 0;
}

/*\
|*| Determine if a key matches an entry.
|*|
|*|   _this - this hash table
|*|     key - the search key
|*|   entry - the entry
|*|
|*|  Return true on match, false otherwise.
\*/
#define CHashTable_IsMatch(_this, key, entry)                                  \
	((key)->hash == (entry)->hash &&                                           \
     CHashTable_KeysEqual((_this), (key), (entry)))

/*\
|*| Determine if keys are equal.
|*|
|*|   _this - this hash table
|*|       a - the first key
|*|       b - the second key
|*|
|*|  Returns true if equal, false otherwise.
\*/
#define CHashTable_KeysEqual(_this, a, b) \
	(CPredicate_Binary((_this)->keysEqual, (a), (b)))

/*\
|*| Search a hash table.
|*|
|*|   _this - this hash table
|*|     key - the search key
|*|   found - the result of the search
|*|   match - use matching flag
\*/
#define CHashTable_Search(_this, key, found, match)                            \
	do {                                                                       \
		/* declarations */                                                     \
		CHashEntry **entry;                                                    \
		CHashEntry **end;                                                      \
		CHashEntry **start;                                                    \
		CUInt32      offset;                                                   \
		CUInt32      index;                                                    \
		CUInt32      length;                                                   \
		                                                                       \
		/* get the table length */                                             \
		length = (_this)->layout->hash;                                        \
		                                                                       \
		/* calculate the entry reference */                                    \
		entry = ((_this)->entries + ((key)->hash % length));                   \
		                                                                       \
		/* get the end pointer */                                              \
		end = ((_this)->entries + length);                                     \
		                                                                       \
		/* calculate the offset */                                             \
		offset = ((key)->hash % (_this)->layout->rehash);                      \
		                                                                       \
		/* ensure the offset is non-zero */                                    \
		if(offset == 0) { offset = 1; }                                        \
		                                                                       \
		/* set the start of chain entry to the default */                      \
		start = 0;                                                             \
		                                                                       \
		/* find the entry */                                                   \
		for(index = 0; index < length; ++index)                                \
		{                                                                      \
			/* process the entry */                                            \
			if(match)                                                          \
			{                                                                  \
				/* process the entry based on its state */                     \
				if(CHashEntry_IsNull(*entry))                                  \
				{                                                              \
					start = ((start != 0) ? start : entry);                    \
					break;                                                     \
				}                                                              \
				else if(CHashEntry_IsDead(*entry))                             \
				{                                                              \
					if(start == 0) { start = entry; }                          \
				}                                                              \
				else if(CHashTable_IsMatch((_this), (key), *entry))            \
				{                                                              \
					start = entry;                                             \
					break;                                                     \
				}                                                              \
			}                                                                  \
			else                                                               \
			{                                                                  \
				/* return the entry if it's empty */                           \
				if(!CHashEntry_IsLive(*entry))                                 \
				{                                                              \
					start = entry;                                             \
					break;                                                     \
				}                                                              \
			}                                                                  \
			                                                                   \
			/* update the entry reference */                                   \
			entry += offset;                                                   \
			                                                                   \
			/* wrap around the table as needed */                              \
			if(entry >= end) { entry -= length; }                              \
		}                                                                      \
		                                                                       \
		/* return the start of the chain */                                    \
		(found) = start;                                                       \
	} while(0)

/*\
|*| Find an empty entry.
|*|
|*|   _this - this hash table
|*|     key - the search key
|*|
|*|  NOTE: this is only safe to use when resizing
|*|
|*|  Returns an empty entry.
\*/
static CHashEntry **
CHashTable_FindEntry(CHashTable *_this,
                     CHashEntry *key)
{
	/* declarations */
	CHashEntry **e;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* search for the entry */
	CHashTable_Search(_this, key, e, 0);

	/* return the entry reference */
	return e;
}

/*\
|*| Find a matching entry.
|*|
|*|   _this - this hash table
|*|     key - the search key
|*|
|*|  Returns entry if found, null otherwise.
\*/
static CHashEntry **
CHashTable_FindMatch(CHashTable *_this,
                     CHashEntry *key)
{
	/* declarations */
	CHashEntry **e;

	/* search for the entry */
	CHashTable_Search(_this, key, e, 1);

	/* return the entry reference */
	return e;
}

/*\
|*| Find an appropriate layout for a table.
|*|
|*|   _this - this hash table
|*|
|*|  Returns a layout, or null when out of memory.
\*/
static const CHashTableLayout *
CHashTable_FindLayout(CHashTable *_this)
{
	/* declarations */
	const CHashTableLayout *layout;
	CUInt32                 count;

	/* get the current layout */
	layout = _this->layout;

	/* get the current count */
	count = _this->count;

	/* bail out now if there's nothing to do */
	if(count >= layout->lwm && count <= layout->hwm) { return layout; }

	/* find the appropriate layout */
	if(count < layout->lwm)
	{
		/* bail out now if there's nothing to do */
		if(layout == CHashTable_Layouts) { return layout; }

		/* return the next smaller layout */
		return (layout - 1);
	}
	else
	{
		/* bail out now if there's no appropriate layout */
		if(layout == CHashTable_Layouts_Last) { return 0; }

		/* return the next larger layout */
		return (layout + 1);
	}
}

/*\
|*| Find an appropriate layout for a hash table.
|*|
|*|   _this - this hash table
|*|
|*|  Returns a layout, or null when out of memory.
\*/
static CStatus
CHashTable_Resize(CHashTable *_this)
{
	/* declarations */
	CHashTable table;
	CUInt32    length;

	/* get the table */
	table = *_this;

	/* find the appropriate layout */
	table.layout = CHashTable_FindLayout(&table);

	/* bail out now if there's nothing to do */
	CStatus_Require((table.layout != _this->layout), CStatus_OK);

	/* ensure we have a layout */
	CStatus_Require((table.layout != 0), CStatus_OutOfMemory);

	/* get the new length */
	length = table.layout->hash;

	/* allocate the new entry list */
	if(!(table.entries = CHashTable_AllocEntries(length)))
	{
		return CStatus_OutOfMemory;
	}

	/* add the live entries to the new entry list */
	{
		/* declarations */
		CHashEntry **curr;
		CHashEntry **end;

		/* get the current entry pointer */
		curr = _this->entries;

		/* get the end pointer */
		end = (curr + _this->layout->hash);

		/* copy the entries to the new entry list */
		while(curr != end)
		{
			/* copy the current entry to the new entry list, as needed */
			if(CHashEntry_IsLive(*curr))
			{
				/* declarations */
				CHashEntry **entry;

				/* find a suitable position for the current entry */
				entry = CHashTable_FindEntry(&table, *curr);

				/* add the current entry to the new entry list */
				*entry = *curr;
			}

			/* move to the next entry */
			++curr;
		}
	}

	/* free the old entry list */
	CFree(_this->entries);

	/* update the layout */
	_this->layout = table.layout;

	/* update the entry list */
	_this->entries = table.entries;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Get the entry count of a hash table.
|*|
|*|   _this - this hash table
|*|
|*|  Returns the entry count.
\*/
CINTERNAL CUInt32
CHashTable_GetCount(CHashTable *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* return the count */
	return _this->count;
}

/*\
|*| Get a matching entry from a hash table.
|*|
|*|   _this - this hash table
|*|     key - the search key
|*|
|*|  Returns the entry if found, null otherwise.
\*/
CINTERNAL CHashEntry *
CHashTable_GetEntry(CHashTable *_this,
                    CHashEntry *key)
{
	/* declarations */
	CHashEntry **entry;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* find the matching entry */
	entry = CHashTable_FindMatch(_this, key);

	/* null the entry as needed */
	if(!CHashEntry_IsLive(*entry)) { *entry = 0; }

	/* return the entry */
	return *entry;
}

/*\
|*| Get a random entry from a hash table.
|*|
|*|   _this - this hash table
|*|
|*|  Returns a random entry if found, null otherwise.
\*/
CINTERNAL CHashEntry *
CHashTable_GetRandomEntry(CHashTable *_this)
{
	/* declarations */
	CHashEntry **entry;
	CHashEntry **end;
	CUInt32      offset;
	CUInt32      index;
	CUInt32      length;
	CUInt32      hash;

	/* assertions */
	CASSERT((_this != 0));

	/* get the table length */
	length = _this->layout->hash;

	/* generate a random hash value */
	hash = rand();

	/* calculate the entry reference */
	entry = (_this->entries + (hash % length));

	/* get the end pointer */
	end = (_this->entries + length);

	/* calculate the offset */
	offset = (hash % _this->layout->rehash);

	/* ensure the offset is non-zero */
	if(offset == 0) { offset = 1; }

	/* find the entry */
	for(index = 0; index < length; ++index)
	{
		/* return the entry if it is live */
		if(CHashEntry_IsLive(*entry)) { return *entry; }

		/* update the entry reference */
		entry += offset;

		/* wrap around the table as needed */
		if(entry >= end) { entry -= length; }
	}

	/* return a null reference */
	return 0;
}

/*\
|*| Add an entry to a hash table.
|*|
|*|   _this - this hash table
|*|   entry - the entry to add
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CHashTable_AddEntry(CHashTable *_this,
                    CHashEntry *entry)
{
	/* declarations */
	CHashEntry **e;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((entry  != 0));

	/* find a suitable entry reference */
	e = CHashTable_FindMatch(_this, entry);

	/* ensure the entry doesn't already exist */
	CASSERT((!CHashEntry_IsLive(*e)));

	/* set the entry */
	*e = entry;

	/* update the live entry count */
	++(_this->count);

	/* resize and return */
	return CHashTable_Resize(_this);
}

/*\
|*| Remove an entry from a hash table.
|*|
|*|   _this - this hash table
|*|     key - the search key
|*|
|*|  NOTE: this method does nothing, returning successfully,
|*|        if a matching entry is not found in the table
|*|
|*|  Returns status code (safe to ignore).
\*/
CINTERNAL CStatus
CHashTable_RemoveEntry(CHashTable *_this,
                       CHashEntry *key)
{
	/* declarations */
	CHashEntry **entry;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* find a matching entry reference */
	entry = CHashTable_FindMatch(_this, key);

	/* bail out now if there's nothing to do */
	CStatus_Require((CHashEntry_IsLive(*entry)), CStatus_OK);

	/* mark the entry as dead */
	*entry = CHashTable_DeadEntry;

	/* update the live entry count */
	--(_this->count);

	/* resize and return */
	return CHashTable_Resize(_this);
}


#ifdef __cplusplus
};
#endif
