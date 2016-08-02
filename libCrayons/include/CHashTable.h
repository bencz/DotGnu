/*
 * CHashTable.h - Hash table header.
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

#ifndef _C_HASHTABLE_H_
#define _C_HASHTABLE_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCHashTable CHashTable;
typedef struct _tagCHashEntry CHashEntry;
struct _tagCHashEntry
{
	CUInt32 hash;
};

CINTERNAL CStatus
CHashTable_Create(CHashTable       **_this,
                  CPredicateBinary  *keysEqual);
CINTERNAL void
CHashTable_Destroy(CHashTable     **_this,
                   COperatorUnary  *destroyEntry);
CINTERNAL CUInt32
CHashTable_GetCount(CHashTable *_this);
CINTERNAL CHashEntry *
CHashTable_GetEntry(CHashTable *_this,
                    CHashEntry *key);
CINTERNAL CHashEntry *
CHashTable_GetRandomEntry(CHashTable *_this);
CINTERNAL CStatus
CHashTable_AddEntry(CHashTable *_this,
                    CHashEntry *entry);
CINTERNAL CStatus
CHashTable_RemoveEntry(CHashTable *_this,
                       CHashEntry *key);

#ifdef __cplusplus
};
#endif

#endif /* _C_HASHTABLE_H_ */
