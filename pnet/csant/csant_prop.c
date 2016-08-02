/*
 * csant_prop.c - Manage the list of properties in the system.
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

#include "csant_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Hash table that contains the properties and their current definitions.
 */
static ILHashTable *properties = 0;

/*
 * Property hash entry.
 */
typedef struct
{
	int		fromCmdLine;
	int		nameLen;
	char	name[1];

} PropHashEntry;

/*
 * Property hash key.
 */
typedef struct
{
	const char *name;
	int         nameLen;

} PropHashKey;

/*
 * Compute the hash value for an entry.
 */
static unsigned long Prop_Compute(const PropHashEntry *entry)
{
	return ILHashString(0, entry->name, entry->nameLen);
}

/*
 * Compute the hash value for a key.
 */
static unsigned long Prop_KeyCompute(const PropHashKey *key)
{
	return ILHashString(0, key->name, key->nameLen);
}

/*
 * Match a hash entry against a key.
 */
static int Prop_Match(const PropHashEntry *entry, const PropHashKey *key)
{
	if(entry->nameLen == key->nameLen &&
	   !ILMemCmp(entry->name, key->name, key->nameLen))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Free a hash entry.
 */
static void Prop_Free(PropHashEntry *entry)
{
	ILFree(entry);
}

void CSAntDefineProperty(const char *name, int nameLen,
						 const char *value, int fromCmdLine)
{
	PropHashKey key;
	PropHashEntry *entry;

	/* Initialize the hash table, if necessary */
	if(!properties)
	{
		properties = ILHashCreate(0, (ILHashComputeFunc)Prop_Compute,
								  (ILHashKeyComputeFunc)Prop_KeyCompute,
								  (ILHashMatchFunc)Prop_Match,
								  (ILHashFreeFunc)Prop_Free);
		if(!properties)
		{
			CSAntOutOfMemory();
		}
	}

	/* See if we already have a property with this name */
	key.name = name;
	if(nameLen >= 0)
	{
		key.nameLen = nameLen;
	}
	else
	{
		key.nameLen = strlen(name);
	}
	entry = ILHashFindType(properties, &key, PropHashEntry);
	if(entry)
	{
		/* If the existing entry is from the command-line,
		   then we cannot replace it with a new value */
		if(entry->fromCmdLine)
		{
			return;
		}

		/* Remove the existing entry */
		ILHashRemove(properties, entry, 1);
	}

	/* Create a new property hash entry */
	entry = (PropHashEntry *)ILMalloc(sizeof(PropHashEntry) + key.nameLen +
									  strlen(value));
	if(!entry)
	{
		CSAntOutOfMemory();
	}
	entry->fromCmdLine = fromCmdLine;
	entry->nameLen = key.nameLen;
	ILMemCpy(entry->name, name, key.nameLen);
	strcpy(entry->name + key.nameLen, value);

	/* Add the entry to the hash table */
	if(!ILHashAdd(properties, entry))
	{
		CSAntOutOfMemory();
	}
}

const char *CSAntGetProperty(const char *name, int nameLen)
{
	PropHashKey key;
	PropHashEntry *entry;
	key.name = name;
	if(nameLen >= 0)
	{
		key.nameLen = nameLen;
	}
	else
	{
		key.nameLen = strlen(name);
	}
	entry = ILHashFindType(properties, &key, PropHashEntry);
	if(entry)
	{
		return entry->name + entry->nameLen;
	}
	else
	{
		return 0;
	}
}

#ifdef	__cplusplus
};
#endif
