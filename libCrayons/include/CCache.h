/*
 * CCache.h - Cache header.
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

#ifndef _C_CACHE_H_
#define _C_CACHE_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCCache CCache;
typedef struct _tagCCacheEntry CCacheEntry;
struct _tagCCacheEntry
{
	CUInt32 hash;
	CUInt32 memory;
};

CINTERNAL CStatus
CCache_Create(CCache           **_this,
              CPredicateBinary  *keysEqual,
              COperatorUnary    *destroyEntry,
              CUInt32            maxMemory);
CINTERNAL void
CCache_Destroy(CCache **_this);
CINTERNAL void
CCache_Disable(CCache *_this);
CINTERNAL void
CCache_Enable(CCache *_this);
CINTERNAL CCacheEntry *
CCache_GetEntry(CCache      *_this,
                CCacheEntry *key);
CINTERNAL CStatus
CCache_AddEntry(CCache      *_this,
                CCacheEntry *entry);
CINTERNAL void
CCache_RemoveEntry(CCache      *_this,
                   CCacheEntry *entry);


#ifdef __cplusplus
};
#endif

#endif /* _C_CACHE_H_ */
