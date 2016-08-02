/*
 * CFontFamilyTable.c - Font family table implementation.
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

#include "CFontFamilyTable.h"
#include "CFontFamily.h"

#ifdef __cplusplus
extern "C" {
#endif

/*\
|*| Destroy a family entry.
|*|
|*|    oper - destruction operator
|*|   entry - entry to be destroyed
\*/
static void
CFontFamilyTable_DestroyOperator(COperatorUnary *oper,
                                 void           *entry)
{
	/* finalize the family */
	CFontFamily_Finalize((CFontFamily *)entry);

	/* free the family */
	CFree(entry);
}

/*\
|*| Initialize this family table.
|*|
|*|   _this - this family table
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontFamilyTable_Initialize(CFontFamilyTable *_this)
{
	/* declarations */
	CFontFamily **curr;
	CFontFamily **end;
	CStatus       status;

	/* assertions */
	CASSERT((_this != 0));

	/* initialize the font family equality predicate */
	CStatus_Check
		(CFontFamilyEquals_Initialize
			(&(_this->keysEqual)));

	/* initialize the family table */
	status =
		CHashTable_Create
			(&(_this->families), (CPredicateBinary *)&(_this->keysEqual));

	/* handle table initialization failures */
	if(status != CStatus_OK)
	{
		CFontFamilyEquals_Finalize(&(_this->keysEqual));
		return status;
	}

	/* initialize the generic families list */
	{
		/* get the current generic pointer */
		curr = _this->generics;

		/* get the end pointer */
		end = (curr + CFontFamilyTable_MaxGenerics);

		/* null the generic family slots */
		while(curr != end) { *curr = 0; ++curr; }
	}

	/* initialize the saved families queue */
	{
		/* get the current save pointer */
		curr = _this->saves;

		/* get the end pointer */
		end = (curr + CFontFamilyTable_MaxSaves);

		/* null the saved family slots */
		while(curr != end) { *curr = 0; ++curr; }
	}

	/* initialize the saved families count */
	_this->count = 0;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Finalize this family table.
|*|
|*|   _this - this family table
\*/
CINTERNAL void
CFontFamilyTable_Finalize(CFontFamilyTable *_this)
{
	/* declarations */
	COperatorUnary destroyEntry;

	/* assertions */
	CASSERT((_this != 0));

	/* finalize the font family equality predicate */
	CFontFamilyEquals_Finalize(&(_this->keysEqual));

	/* initialize the entry destructor */
	destroyEntry.Operator = CFontFamilyTable_DestroyOperator;

	/* finalize the family table */
	CHashTable_Destroy(&(_this->families), &destroyEntry);
}

/*\
|*| Get a generic family.
|*|
|*|     _this - this family table
|*|   generic - generic type of family
|*|
|*|  Returns generic family if set, null otherwise.
\*/
CINTERNAL CFontFamily *
CFontFamilyTable_GetGeneric(CFontFamilyTable   *_this,
                            CFontFamilyGeneric  generic)
{
	/* assertions */
	CASSERT((_this != 0));

	/* return the family from the generics list */
	return _this->generics[generic];
}

/*\
|*| Set a family as a generic family.
|*|
|*|     _this - this family table
|*|   generic - generic type of family
|*|    family - family to be set as generic
\*/
CINTERNAL void
CFontFamilyTable_SetGeneric(CFontFamilyTable   *_this,
                            CFontFamilyGeneric  generic,
                            CFontFamily        *family)
{
	/* assertions */
	CASSERT((_this                   != 0));
	CASSERT((family                  != 0));
	CASSERT(_this->generics[generic] == 0);

	/* remove from the family from the saved families queue, as needed */
	if(family->refCount == 0) { CFontFamilyTable_Unsave(_this, family); }

	/* add the family to the generics list */
	_this->generics[generic] = family;
}

/*\
|*| Add a family to the saved families queue.
|*|
|*|    _this - this family table
|*|   family - family to be placed on the queue
|*|
|*|  NOTE: families on the queue will be finalized to make
|*|        room for new saves, but family reference count
|*|        management is not handled by the family table
\*/
CINTERNAL void
CFontFamilyTable_Save(CFontFamilyTable *_this,
                      CFontFamily      *family)
{
	/* declarations */
	CFontFamily **curr;
	CFontFamily **end;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));

	/*\
	|*| NOTE: generic families are always saved to the generics list,
	|*|       and last as long as the collection; they are only placed
	|*|       on the saved families queue if they are, at the time,
	|*|       not known to be generic, in which case they are removed
	|*|       from the saved families queue as soon as they are added
	|*|       to the generics list
	\*/

	/* get the current generic family pointer */
	curr = _this->generics;

	/* get the end of generics pointer */
	end = (curr + CFontFamilyTable_MaxGenerics);

	/* find the matching family */
	while(curr != end && *curr != family) { ++curr; }

	/* add the family to the saved families queue, as appropriate */
	if(curr == end)
	{
		/* declarations */
		CFontFamily **head;

		/* get the head pointer */
		head = _this->saves;

		/* remove the head of the saved families queue, as needed */
		if(_this->count == CFontFamilyTable_MaxSaves)
		{
			/* remove the family from the family table */
			CHashTable_RemoveEntry(_this->families, ((CHashEntry *)*head));

			/* finalize the family */
			CFontFamily_Finalize(*head);

			/* free the family */
			CFree(*head);

			/* update the save count */
			--(_this->count);

			/* remove the family from the saved families queue */
			CMemMove(head, (head + 1), (_this->count * sizeof(CFontFamily *)));
		}

		/* add the family to the saved families queue */
		head[_this->count] = family;

		/* update the save count */
		++(_this->count);
	}
}

/*\
|*| Remove a family from the saved families queue.
|*|
|*|    _this - this family table
|*|   family - family to be removed from the queue
\*/
CINTERNAL void
CFontFamilyTable_Unsave(CFontFamilyTable *_this,
                        CFontFamily      *family)
{
	/* declarations */
	CFontFamily **curr;
	CFontFamily **end;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));

	/* get the current saved family pointer */
	curr = _this->saves;

	/* get the end of saves pointer */
	end = (curr + _this->count);

	/* find the matching saved family */
	while(curr != end && *curr != family) { ++curr; }

	/* remove matching family, or check for generic, as appropriate */
	if(curr != end)
	{
		/* remove the family from the saved families queue */
		CMemMove(curr, (curr + 1), ((end - curr) * sizeof(CFontFamily *)));

		/* update the save count */
		--(_this->count);
	}
	else
	{
		/* get the current generic family pointer */
		curr = _this->generics;

		/* get the end of generics pointer */
		end = (curr + CFontFamilyTable_MaxGenerics);

		/* find the matching family */
		while(curr != end && *curr != family) { ++curr; }

		/* ensure the family was saved */
		CASSERT((curr != end));
	}
}

/*\
|*| Add a family to this family table.
|*|
|*|    _this - this family table
|*|   family - family to add
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontFamilyTable_AddEntry(CFontFamilyTable *_this,
                          CFontFamily      *family)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));

	/* add the entry */
	return CHashTable_AddEntry(_this->families, (CHashEntry *)family);
}

/*\
|*| Get a matching family from this family table.
|*|
|*|   _this - this family table
|*|     key - search key
|*|
|*|  Returns matching family if available, null otherwise.
\*/
CINTERNAL CFontFamily *
CFontFamilyTable_GetEntry(CFontFamilyTable *_this,
                          CFontFamilyKey   *key)
{
	/* declarations */
	CHashEntry *entry;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* get the entry */
	entry = CHashTable_GetEntry(_this->families, (CHashEntry *)key);

	/* return the entry */
	return (CFontFamily *)entry;
}


#ifdef __cplusplus
};
#endif
