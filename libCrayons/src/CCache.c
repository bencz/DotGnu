/*
 * CCache.c - Cache implementation.
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

#include "CCache.h"
#include "CHashTable.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Structure of cache memory manager. */
typedef struct _tagCCacheMemory CCacheMemory;
struct _tagCCacheMemory
{
	CUInt32 maximum;
	CUInt32 current;
	CUInt32 disabled;
};

/* Structure of cache. */
struct _tagCCache
{
	CHashTable     *table;
	COperatorUnary *destroyEntry;
	CCacheMemory    memory;
};

/* Forward declare memory management method. */
static void
CCache_ManageMemory(CCache *_this,
                    CInt32  delta);

/* Cache memory management macros. */
#define CCacheMemory_Initialize(cm, max)                                       \
	do {                                                                       \
		(cm).maximum  = (max);                                                 \
		(cm).current  = 0;                                                     \
		(cm).disabled = 0;                                                     \
	} while(0)
#define CCacheMemory_Finalize(cm)           CCacheMemory_Initialize((cm), 0)
#define CCacheMemory_UpdateUsage(cm, delta) ((cm).current += (delta))
#define CCacheMemory_ExceedsMaximum(cm)     ((cm).current > (cm).maximum)
#define CCacheMemory_IsDisabled(cm)         ((cm).disabled > 0)
#define CCacheMemory_IsDisableAbused(cm)    ((cm).disabled == ((CUInt32)-1))
#define CCacheMemory_Disable(cm)            (++((cm).disabled))
#define CCacheMemory_Enable(cm)             (--((cm).disabled))

/*\
|*| Create a cache.
|*|
|*|          _this - this cache (created)
|*|      keysEqual - key equality predicate
|*|   destroyEntry - entry destruction operator
|*|      maxMemory - maximum memory usage
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CCache_Create(CCache           **_this,
              CPredicateBinary  *keysEqual,
              COperatorUnary    *destroyEntry,
              CUInt32            maxMemory)
{
	/* assertions */
	CASSERT((_this        != 0));
	CASSERT((keysEqual    != 0));
	CASSERT((destroyEntry != 0));

	/* allocate the cache */
	if(!(*_this = CMalloc(sizeof(CCache)))) { return CStatus_OutOfMemory; }

	/* initialize the cache */
	{
		/* declarations */
		CCache  *cache;
		CStatus  status;

		/* get the cache pointer */
		cache = *_this;

		/* create the hash table */
		status = CHashTable_Create(&(cache->table), keysEqual);

		/* handle table creation failures */
		if(status != CStatus_OK)
		{
			CFree(*_this);
			*_this = 0;
			return status;
		}

		/* initialize the cache memory information */
		CCacheMemory_Initialize(cache->memory, maxMemory);

		/* initialize the entry destruction operator */
		cache->destroyEntry = destroyEntry;
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Destroy a cache.
|*|
|*|   _this - this cache
\*/
CINTERNAL void
CCache_Destroy(CCache **_this)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* finalize the cache */
	{
		/* declarations */
		CCache *cache;

		/* get the cache pointer */
		cache = *_this;

		/* destroy the table */
		CHashTable_Destroy(&(cache->table), cache->destroyEntry);

		/* reset the cache memory information */
		CCacheMemory_Finalize(cache->memory);
	}

	/* free the cache */
	CFree(*_this);

	/* null the cache */
	*_this = 0;
}

/*\
|*| Disable cache memory management.
|*|
|*|   _this - this cache
\*/
CINTERNAL void
CCache_Disable(CCache *_this)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT(!(CCacheMemory_IsDisableAbused(_this->memory)));

	/* disable cache memory management */
	CCacheMemory_Disable(_this->memory);
}

/*\
|*| Reenable cache memory management.
|*|
|*|   _this - this cache
\*/
CINTERNAL void
CCache_Enable(CCache *_this)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((CCacheMemory_IsDisabled(_this->memory)));

	/* enable cache memory management */
	if(CCacheMemory_Enable(_this->memory))
	{
		/* perform memory management */
		CCache_ManageMemory(_this, 0);
	}
}

/*\
|*| Get a matching entry from a cache.
|*|
|*|   _this - this cache
|*|     key - the search key
|*|
|*|  Returns the entry if found, null otherwise.
\*/
CINTERNAL CCacheEntry *
CCache_GetEntry(CCache      *_this,
                CCacheEntry *key)
{
	/* assertions */
	CASSERT((_this != 0));

	/* return the entry */
	return (CCacheEntry *)CHashTable_GetEntry(_this->table, ((CHashEntry *)key));
}

/*\
|*| Manage the memory of a cache.
|*|
|*|   _this - this cache
|*|   delta - proposed change in memory usage
|*|
|*|  NOTE: this method doesn't update the cache memory
|*|        usage, it only ejects entries from the
|*|        cache to bring usage to acceptable limits
\*/
static void
CCache_ManageMemory(CCache *_this,
                    CInt32  delta)
{
	/* assertions */
	CASSERT((_this != 0));

	/* bail out now if cache memory management is disabled */
	if(CCacheMemory_IsDisabled(_this->memory)) { return; }

	/* adjust the current memory usage */
	CCacheMemory_UpdateUsage(_this->memory, delta);

	/* remove random entries until memory usage is under the limit */
	while(CCacheMemory_ExceedsMaximum(_this->memory))
	{
		/* declarations */
		CCacheEntry *entry;

		/* get a random entry from the table */
		entry = (CCacheEntry *)CHashTable_GetRandomEntry(_this->table);

		/* bail out now if the cache is empty */
		if(entry == 0) { break; }

		/* update the current memory usage */
		CCacheMemory_UpdateUsage(_this->memory, -(entry->memory));

		/* remove the entry from the table */
		CHashTable_RemoveEntry(_this->table, ((CHashEntry *)entry));

		/* destroy the entry */
		COperator_Unary(_this->destroyEntry, entry);
	}

	/* readjust the current memory usage */
	CCacheMemory_UpdateUsage(_this->memory, -delta);
}

/*\
|*| Add an entry to a cache.
|*|
|*|   _this - this cache
|*|   entry - the entry to add
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CCache_AddEntry(CCache      *_this,
                CCacheEntry *entry)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((entry != 0));

	/* ensure memory usage stays under the limit */
	CCache_ManageMemory(_this, entry->memory);

	/* add the entry to the table */
	CStatus_Check
		(CHashTable_AddEntry
			(_this->table, ((CHashEntry *)entry)));

	/* update the current memory usage */
	CCacheMemory_UpdateUsage(_this->memory, entry->memory);

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Remove an entry from a cache.
|*|
|*|   _this - this cache
|*|   entry - the entry to remove
\*/
CINTERNAL void
CCache_RemoveEntry(CCache      *_this,
                   CCacheEntry *entry)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((entry != 0));

	/* remove the entry from the table */
	CHashTable_RemoveEntry(_this->table, ((CHashEntry *)entry));

	/* update the memory usage */
	CCacheMemory_UpdateUsage(_this->memory, -entry->memory);
}


#ifdef __cplusplus
};
#endif
