/*
 * hashtab.c - Hash table functions.
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

#include "il_system.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Structure of a single hash entry.
 */
struct _tagILHashEntry
{
	void	       *elem;
	ILHashEntry	   *overflow;

};

/*
 * Structure of an overflow table block.
 */
typedef struct _tagILHashOverflow ILHashOverflow;
struct _tagILHashOverflow
{
	ILHashOverflow *next;
	ILHashEntry		entries[1];

};

/*
 * Internal structure of a hash table.
 */
struct _tagILHashTable
{
	int						size;
	ILHashComputeFunc		computeFunc;
	ILHashKeyComputeFunc	keyComputeFunc;
	ILHashMatchFunc			matchFunc;
	ILHashFreeFunc			freeFunc;
	ILHashOverflow         *overflow;
	int						overflowPosn;
	ILHashEntry				table[1];

};

ILHashTable *ILHashCreate(int size, ILHashComputeFunc computeFunc,
						  ILHashKeyComputeFunc keyComputeFunc,
						  ILHashMatchFunc matchFunc,
						  ILHashFreeFunc freeFunc)
{
	ILHashTable *hashtab;

	/* Allocate space for the hash table */
	if(!size)
	{
		size = 509;
	}
	if((hashtab = (ILHashTable *)ILCalloc(1, sizeof(ILHashTable) +
											 sizeof(ILHashEntry) * size)) == 0)
	{
		return 0;
	}

	/* Initialize the hash table */
	hashtab->size = size;
	hashtab->computeFunc = computeFunc;
	hashtab->keyComputeFunc = keyComputeFunc;
	hashtab->matchFunc = matchFunc;
	hashtab->freeFunc = freeFunc;
	hashtab->overflow = 0;
	hashtab->overflowPosn = size;

	/* Ready to go */
	return hashtab;
}

void ILHashDestroy(ILHashTable *hashtab)
{
	ILHashOverflow *overflow, *nextOverflow;
	int hash;
	ILHashEntry *entry;

	/* Free all elements within the hash table */
	if(hashtab->freeFunc)
	{
		for(hash = 0; hash < hashtab->size; ++hash)
		{
			entry = &(hashtab->table[hash]);
			while(entry != 0)
			{
				if(entry->elem != 0)
				{
					(*(hashtab->freeFunc))(entry->elem);
				}
				entry = entry->overflow;
			}
		}
	}

	/* Free the overflow table blocks */
	overflow = hashtab->overflow;
	while(overflow != 0)
	{
		nextOverflow = overflow->next;
		ILFree(overflow);
		overflow = nextOverflow;
	}

	/* Free the hash table object itself */
	ILFree(hashtab);
}

int ILHashAdd(ILHashTable *hashtab, void *elem)
{
	int hash;
	ILHashEntry *entry;
	ILHashOverflow *overflow;

	/* Compute the hash table index */
	hash = (int)((*(hashtab->computeFunc))(elem) %
				 (unsigned long)(hashtab->size));

	/* Add the element to the main part of the hash table if possible */
	if(hashtab->table[hash].elem == 0)
	{
		hashtab->table[hash].elem = elem;
		return 1;
	}

	/* Allocate a new overflow hash entry */
	if(hashtab->overflowPosn < hashtab->size)
	{
		entry = &(hashtab->overflow->entries[(hashtab->overflowPosn)++]);
	}
	else if((overflow = (ILHashOverflow *)ILMalloc
				(sizeof(ILHashOverflow) +
				 (hashtab->size - 1) * sizeof(ILHashEntry))) == 0)
	{
		return 0;
	}
	else
	{
		overflow->next = hashtab->overflow;
		hashtab->overflow = overflow;
		entry = &(overflow->entries[0]);
		hashtab->overflowPosn = 1;
	}

	/* Fill in the overflow entry and link it to the main hash table */
	entry->elem = elem;
	entry->overflow = hashtab->table[hash].overflow;
	hashtab->table[hash].overflow = entry;
	return 1;
}

void *ILHashFind(ILHashTable *hashtab, const void *key)
{
	int hash;
	ILHashEntry *entry;

	/* Compute the hash table index */
	hash = (int)((*(hashtab->keyComputeFunc))(key) %
				 (unsigned long)(hashtab->size));

	/* Search for the requested entry */
	entry = &(hashtab->table[hash]);
	while(entry != 0)
	{
		if(entry->elem != 0 &&
		   (*(hashtab->matchFunc))(entry->elem, key))
		{
			return entry->elem;
		}
		entry = entry->overflow;
	}

	/* The requested entry does not exist in the hash table */
	return 0;
}

void *ILHashFindAlt(ILHashTable *hashtab, const void *key,
					ILHashKeyComputeFunc keyComputeFunc,
					ILHashMatchFunc matchFunc)
{
	int hash;
	ILHashEntry *entry;

	/* Compute the hash table index */
	hash = (int)((*keyComputeFunc)(key) % (unsigned long)(hashtab->size));

	/* Search for the requested entry */
	entry = &(hashtab->table[hash]);
	while(entry != 0)
	{
		if(entry->elem != 0 && (*matchFunc)(entry->elem, key))
		{
			return entry->elem;
		}
		entry = entry->overflow;
	}

	/* The requested entry does not exist in the hash table */
	return 0;
}

void ILHashRemove(ILHashTable *hashtab, void *elem, int freeElem)
{
	int hash;
	ILHashEntry *entry;

	/* Compute the hash table index */
	hash = (int)((*(hashtab->computeFunc))(elem) %
				 (unsigned long)(hashtab->size));

	/* Search for the requested entry */
	entry = &(hashtab->table[hash]);
	while(entry != 0)
	{
		if(entry->elem == elem)
		{
			/* Remove the entry from the hash table and exit */
			if(freeElem && hashtab->freeFunc)
			{
				(*(hashtab->freeFunc))(entry->elem);
			}
			entry->elem = 0;
			return;
		}
		entry = entry->overflow;
	}
}

void ILHashRemoveSubset(ILHashTable *hashtab, ILHashMatchFunc matchFunc,
						const void *key, int freeElem)
{
	int hash;
	ILHashEntry *entry;
	for(hash = 0; hash < hashtab->size; ++hash)
	{
		entry = &(hashtab->table[hash]);
		while(entry != 0)
		{
			if(entry->elem != 0 && (*matchFunc)(entry->elem, key))
			{
				if(freeElem && hashtab->freeFunc)
				{
					(*(hashtab->freeFunc))(entry->elem);
				}
				entry->elem = 0;
			}
			entry = entry->overflow;
		}
	}
}

void ILHashIterInit(ILHashIter *iter, ILHashTable *hashtab)
{
	iter->hashtab = hashtab;
	iter->index = -1;
	iter->entry = 0;
}

void *ILHashIterNext(ILHashIter *iter)
{
	ILHashEntry *temp;
	for(;;)
	{
		if(iter->entry == 0)
		{
			++(iter->index);
			if(iter->index >= iter->hashtab->size)
			{
				break;
			}
			iter->entry = &(iter->hashtab->table[iter->index]);
		}
		while(iter->entry != 0 && iter->entry->elem == 0)
		{
			iter->entry = iter->entry->overflow;
		}
		if((temp = iter->entry) != 0)
		{
			iter->entry = temp->overflow;
			return temp->elem;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
