/*
 * CGlyphCache.h - Glyph cache header.
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

#ifndef _C_GLYPHCACHE_H_
#define _C_GLYPHCACHE_H_

#include "CrayonsInternal.h"
#include "CUnicodeHashTable.h"
#include "CHashTable.h"
#include "CCache.h"
#include "CFont.h"
#include <ft2build.h>
#include FT_FREETYPE_H

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCGlyphCache CGlyphCache;


/*\
|*| NOTE: These structures really belong in the font face header
|*|       but putting them here avoids cyclic header inclusions.
\*/

typedef CUInt32 CGlyphFlag;
#define CGlyphFlag_None             0
#define CGlyphFlag_Hinting          1
#define CGlyphFlag_AntiAlias        2
#define CGlyphFlag_AntiAliasHinting 3

typedef struct _tagCGlyphShapeData CGlyphShapeData;
struct _tagCGlyphShapeData
{
	FT_Matrix  matrix;
	FT_F26Dot6 pointsX;
	FT_F26Dot6 pointsY;
};

typedef struct _tagCGlyphShape CGlyphShape;
struct _tagCGlyphShape
{
	CHashEntry      _base;
	CGlyphShapeData data;
	CUInt32         refCount;
};

typedef struct _tagCGlyphKey CGlyphKey;
struct _tagCGlyphKey
{
	CCacheEntry  _base;
	CGlyphIndex  index;
	CGlyphFlag   flags;
	CGlyphShape *shape;
};

typedef struct _tagCGlyphEntry CGlyphEntry;
struct _tagCGlyphEntry
{
	CGlyphKey       _base;
	CTextMetrics    metrics;
	CVectorF        xbearing;
	CVectorF        xadvance;
	pixman_image_t *mask;
};

CINTERNAL CStatus
CGlyphCache_Create(CGlyphCache **_this);
CINTERNAL void
CGlyphCache_Destroy(CGlyphCache **_this);
CINTERNAL void
CGlyphCache_DestroyShape(CGlyphCache  *_this,
                         CGlyphShape **_shape);
CINTERNAL void
CGlyphCache_DisableMemoryManagement(CGlyphCache *_this);
CINTERNAL void
CGlyphCache_EnableMemoryManagement(CGlyphCache *_this);
CINTERNAL CGlyphEntry *
CGlyphCache_GetEntry(CGlyphCache *_this,
                     CGlyphKey   *key);
CINTERNAL CGlyphShape *
CGlyphCache_GetShape(CGlyphCache *_this,
                     CGlyphShape *key);
CINTERNAL CStatus
CGlyphCache_AddEntry(CGlyphCache *_this,
                     CGlyphEntry *entry);
CINTERNAL CStatus
CGlyphCache_AddShape(CGlyphCache *_this,
                     CGlyphShape *shape);
CINTERNAL void
CGlyphCache_RemoveEntry(CGlyphCache *_this,
                        CGlyphEntry *entry);
CINTERNAL void
CGlyphCache_RemoveShape(CGlyphCache *_this,
                        CGlyphShape *shape);

#ifdef __cplusplus
};
#endif

#endif /* _C_GLYPHCACHE_H_ */
