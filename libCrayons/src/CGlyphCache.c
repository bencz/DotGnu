/*
 * CGlyphCache.c - Glyph cache implementation.
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

#include "CGlyphCache.h"
#include "CFontFace.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Structure of a glyph cache operator. */
typedef struct _tagCGlyphCacheOp CGlyphCacheOp;
struct _tagCGlyphCacheOp
{
	COperatorUnary  _base;
	CGlyphCache    *cache;
};

/* Structure of a glyph cache. */
struct _tagCGlyphCache
{
	CHashTable    *shapes;
	CCache        *glyphs;
	CGlyphCacheOp  destroyEntry;
};

/* Shape equality predicate. */
static CBool
CGlyphCache_ShapeEquals_Predicate(CPredicateBinary *pred,
                                  void             *_a,
                                  void             *_b)
{
	/* declarations */
	CGlyphShapeData *a;
	CGlyphShapeData *b;

	/* get the glyph shape data */
	a = &(((CGlyphShape *)_a)->data);
	b = &(((CGlyphShape *)_b)->data);

	/* compare the glyph shape data */
	if(CMemCmp(a, b, (sizeof(CGlyphShapeData))) != 0) { return 0; }

	/* return equals flag */
	return 1;
}

/* Entry equality predicate. */
static CBool
CGlyphCache_EntryEquals_Predicate(CPredicateBinary *pred,
                                  void             *_a,
                                  void             *_b)
{
	/* declarations */
	CGlyphKey *a;
	CGlyphKey *b;

	/* get the glyph keys */
	a = (CGlyphKey *)_a;
	b = (CGlyphKey *)_b;

	/* compare the indices */
	if(a->index != b->index) { return 0; }

	/* compare the flags */
	if(a->flags != b->flags) { return 0; }

	/* compare the shapes */
	if(a->shape != b->shape) { return 0; }

	/* return equals flag */
	return 1;
}

/* Entry destruction operator. */
static void
CGlyphCache_EntryDestroy_Operator(COperatorUnary *oper,
                                  void           *_entry)
{
	/* declarations */
	CGlyphCacheOp *op;
	CGlyphEntry   *entry;

	/* get the operation */
	op = (CGlyphCacheOp *)oper;

	/* get the entry */
	entry = (CGlyphEntry *)_entry;

	/* destroy the shape */
	CGlyphCache_DestroyShape(op->cache, &(entry->_base.shape));

	/* destroy the entry */
	CGlyphEntry_Destroy(&entry);
}

/* Glyph cache predicates and operators. */
static struct
{
	CPredicateBinary shapeEquals;
	CPredicateBinary entryEquals;
	COperatorUnary   entryDestroy;
} CGlyphCache_PredOps =
{
	{ CGlyphCache_ShapeEquals_Predicate },
	{ CGlyphCache_EntryEquals_Predicate },
	{ CGlyphCache_EntryDestroy_Operator }
};

/*\
|*| Create a glyph cache.
|*|
|*|   _this - this glyph cache (created)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CGlyphCache_Create(CGlyphCache **_this)
{
	/* declarations */
	CStatus      status;
	CGlyphCache *gc;

	/* assertions */
	CASSERT((_this != 0));

	/* allocate the glyph cache */
	if(!(*_this = (CGlyphCache *)CMalloc(sizeof(CGlyphCache))))
	{
		return CStatus_OutOfMemory;
	}

	/* get the glyph cache */
	gc = *_this;

	/* create the shape table */
	status =
		CHashTable_Create
			(&(gc->shapes),
			 &(CGlyphCache_PredOps.shapeEquals));

	/* handle table creation failures */
	if(status != CStatus_OK)
	{
		CFree(*_this);
		*_this = 0;
		return status;
	}

	/* initialize the entry destructor */
	gc->destroyEntry._base = CGlyphCache_PredOps.entryDestroy;
	gc->destroyEntry.cache = *_this;

	/* create the glyph entry cache */
	status =
		CCache_Create
			(&(gc->glyphs),
			 &(CGlyphCache_PredOps.entryEquals),
			 &(gc->destroyEntry._base),
			 CFontFace_MaxCacheMemory);

	/* handle cache creation failures */
	if(status != CStatus_OK)
	{
		CHashTable_Destroy(&(gc->shapes), 0);
		CFree(*_this);
		*_this = 0;
		return status;
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Destroy a glyph cache.
|*|
|*|   _this - this glyph cache
\*/
CINTERNAL void
CGlyphCache_Destroy(CGlyphCache **_this)
{
	/* declarations */
	CGlyphCache *gc;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* get the glyph cache */
	gc = *_this;

	/* destroy the glyph entry cache */
	CCache_Destroy(&(gc->glyphs));

	/* ensure the table is empty */
	CASSERT((CHashTable_GetCount(gc->shapes) == 0));

	/* destroy the shape table */
	CHashTable_Destroy(&(gc->shapes), 0);

	/* free the glyph cache */
	CFree(*_this);

	/* null the glyph cache */
	*_this = 0;
}

/*\
|*| Destroy a shape in the cache.
|*|
|*|   _this - this glyph cache
|*|   shape - shape to be destroyed
|*|
|*|  NOTE: this method removes the shape from the cache
|*|        if there are no remaining references to it
\*/
CINTERNAL void
CGlyphCache_DestroyShape(CGlyphCache  *_this,
                         CGlyphShape **_shape)
{
	/* declarations */
	CGlyphShape *shape;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((_shape  != 0));
	CASSERT((*_shape != 0));

	/* get the shape */
	shape = *_shape;

	/* remove the shape, as needed */
	if(shape->refCount == 1)
	{
		CHashTable_RemoveEntry(_this->shapes, (CHashEntry *)shape);
	}

	/* destroy the shape */
	CGlyphShape_Destroy(_shape);
}

/*\
|*| Disable cache memory management.
|*|
|*|   _this - this glyph cache
|*|
|*|  NOTE: this method disables the removal of glyph
|*|        entries due to memory pressure
\*/
CINTERNAL void
CGlyphCache_DisableMemoryManagement(CGlyphCache *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* disable cache memory management */
	CCache_Disable(_this->glyphs);
}

/*\
|*| Reenable cache memory management.
|*|
|*|   _this - this glyph cache
|*|
|*|  NOTE: this method reenables the removal of glyph
|*|        entries due to memory pressure
\*/
CINTERNAL void
CGlyphCache_EnableMemoryManagement(CGlyphCache *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reenable cache memory management */
	CCache_Enable(_this->glyphs);
}

/*\
|*| Get a glyph entry from a glyph cache.
|*|
|*|   _this - this glyph cache
|*|     key - the search key
|*|
|*|  Returns glyph entry if found, null otherwise.
\*/
CINTERNAL CGlyphEntry *
CGlyphCache_GetEntry(CGlyphCache *_this,
                     CGlyphKey   *key)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* get and return the entry */
	return (CGlyphEntry *)CCache_GetEntry(_this->glyphs, (CCacheEntry *)key);
}

/*\
|*| Get a glyph shape from a glyph cache.
|*|
|*|   _this - this glyph cache
|*|     key - the search key
|*|
|*|  Returns glyph shape if found, null otherwise.
\*/
CINTERNAL CGlyphShape *
CGlyphCache_GetShape(CGlyphCache *_this,
                     CGlyphShape *key)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((key   != 0));

	/* get and return the shape */
	return (CGlyphShape *)CHashTable_GetEntry(_this->shapes, (CHashEntry *)key);
}

/*\
|*| Add a glyph entry to a glyph cache.
|*|
|*|   _this - this glyph cache
|*|   entry - the glyph entry
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CGlyphCache_AddEntry(CGlyphCache *_this,
                     CGlyphEntry *entry)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((entry != 0));

	/* add the entry */
	return CCache_AddEntry(_this->glyphs, (CCacheEntry *)entry);
}

/*\
|*| Add a glyph shape to a glyph cache.
|*|
|*|   _this - this glyph cache
|*|   shape - the glyph shape
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CGlyphCache_AddShape(CGlyphCache *_this,
                     CGlyphShape *shape)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((shape != 0));

	/* add the shape */
	return CHashTable_AddEntry(_this->shapes, (CHashEntry *)shape);
}

/*\
|*| Remove a glyph entry from a glyph cache.
|*|
|*|   _this - this glyph cache
|*|   entry - the glyph entry
\*/
CINTERNAL void
CGlyphCache_RemoveEntry(CGlyphCache *_this,
                        CGlyphEntry *entry)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((entry != 0));

	/* remove the entry */
	CCache_RemoveEntry(_this->glyphs, (CCacheEntry *)entry);
}

/*\
|*| Remove a glyph shape from a glyph cache.
|*|
|*|   _this - this glyph cache
|*|   shape - the glyph shape
\*/
CINTERNAL void
CGlyphCache_RemoveShape(CGlyphCache *_this,
                        CGlyphShape *shape)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((shape != 0));

	/* remove the shape */
	CHashTable_RemoveEntry(_this->shapes, (CHashEntry *)shape);
}


#ifdef __cplusplus
};
#endif
