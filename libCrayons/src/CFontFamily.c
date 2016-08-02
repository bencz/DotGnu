/*
 * CFontFamily.c - Font family implementation.
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

#include "CFontFamily.h"
#include "CFontCollection.h"
#include "CFontFace.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/*\
|*| Determine if the given keys are equal.
|*|
|*|   _this - this key equality predicate
|*|      _a - the first key
|*|      _b - the second key
|*|
|*|  Returns true if the keys are equal, false otherwise.
\*/
static CBool
CFontFamilyEquals_Predicate(CPredicateBinary *_this,
                            void             *_a,
                            void             *_b)
{
	/* declarations */
	CFontFamilyEquals *equals;
	CFontFamilyKey    *a;
	CFontFamilyKey    *b;

	/* get this as a font family equality predicate */
	equals = ((CFontFamilyEquals *)_this);

	/* get the keys as font family keys */
	a = ((CFontFamilyKey *)_a);
	b = ((CFontFamilyKey *)_b);

	/* test the equality of the font families */
	return (a == b || FcPatternEqualSubset(a->pattern, b->pattern, equals->os));
}

/*\
|*| Initialize this key equality predicate.
|*|
|*|   _this - this key equality predicate
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontFamilyEquals_Initialize(CFontFamilyEquals *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the base */
	_this->_base.Predicate = CFontFamilyEquals_Predicate;

	/* initialize the object set */
	if(!(_this->os = FcObjectSetBuild(FC_FAMILY, FC_FOUNDRY, 0)))
	{
		return CStatus_OutOfMemory;
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Finalize this key equality predicate.
|*|
|*|   _this - this key equality predicate
\*/
CINTERNAL void
CFontFamilyEquals_Finalize(CFontFamilyEquals *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the base */
	_this->_base.Predicate = 0;

	/* finalize the object set */
	FcObjectSetDestroy(_this->os);

	/* null the object set */
	_this->os = 0;
}

/*\
|*| Initialize this font family key.
|*|
|*|     _this - this font family key
|*|   pattern - the family/foundry pattern
\*/
CINTERNAL void
CFontFamilyKey_Initialize(CFontFamilyKey *_this,
                          FcPattern      *pattern)
{
	/* declarations */
	FcChar8  *str;
	FcChar8   c;
	FcResult  result;
	CUInt32   hash;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* get the font family */
	result = FcPatternGetString(pattern, FC_FAMILY, 0, &str);

	/* ensure we have a family */
	CASSERT((result == FcResultMatch));
	CASSERT((str    != 0));
	CASSERT((*str   != 0));

	/* initialize the hash */
	hash = CUtils_HashFast_Init;

	/* hash the family name */
	while((c = *str++)) { hash = CUtils_HashFast_Oper(hash, c); }

	/* get the font foundry */
	result = FcPatternGetString(pattern, FC_FOUNDRY, 0, &str);

	/* ensure we have a foundry */
	CASSERT((result == FcResultMatch));
	CASSERT((str    != 0));
	CASSERT((*str   != 0));

	/* hash the foundry name */
	while((c = *str++)) { hash = CUtils_HashFast_Oper(hash, c); }

	/* initialize the base */
	_this->_base.hash = hash;

	/* set the pattern */
	_this->pattern = pattern;
}

/*\
|*| Finalize this font family key.
|*|
|*|   _this - this font family key
\*/
CINTERNAL void
CFontFamilyKey_Finalize(CFontFamilyKey *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reset members */
	_this->_base.hash = 0;
	_this->pattern    = 0;
}

/*\
|*| Initialize a font family.
|*|
|*|   _this - this font family
|*|      fc - the font collection
|*|     key - the font family key
\*/
CINTERNAL void
CFontFamily_Initialize(CFontFamily     *_this,
                       CFontCollection *fc,
                       CFontFamilyKey  *key)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((fc    != 0));
	CASSERT((key   != 0));

	/* initialize the face slots */
	_this->faces[0] = 0;
	_this->faces[1] = 0;
	_this->faces[2] = 0;
	_this->faces[3] = 0;

	/* initialize the remaining members */
	_this->_base      = *key;
	_this->collection = fc;
	_this->refCount   = 1;
}

/*\
|*| Finalize a font family.
|*|
|*|   _this - this font family
\*/
CINTERNAL void
CFontFamily_Finalize(CFontFamily *_this)
{
	/* assertions */
	CASSERT((_this           != 0));
	CASSERT((_this->refCount == 0));

	/* finalize the faces */
	{
		/* declarations */
		CFontFace **curr;
		CFontFace **end;

		/* get the current face pointer */
		curr = _this->faces;

		/* get the end pointer */
		end = (curr + CFontFamily_MaxFaces);

		/* finalize the faces */
		while(curr != end)
		{
			/* finalize the current face, as needed */
			if(*curr != 0 && *curr != CFontFace_Unavailable)
			{
				CFontFace_Destroy(curr);
			}

			/* move on to the next face slot */
			++curr;
		}
	}

	/*\
	|*| NOTE: key finalization doesn't assume ownership
	|*|       of the pattern, but families always have
	|*|       ownership of their patterns, so explicit
	|*|       pattern destruction is necessary here
	\*/

	/* destroy the pattern */
	FcPatternDestroy(_this->_base.pattern);

	/* finalize the base */
	CFontFamilyKey_Finalize(&(_this->_base));

	/* null the collection */
	_this->collection = 0;
}

/*\
|*| Reference a font family.
|*|
|*|   _this - this font family
|*|
|*|  NOTE: the collection must NOT be locked
\*/
CINTERNAL void
CFontFamily_Reference(CFontFamily *_this)
{
	/* declarations */
	CFontCollection *fc;

	/* assertions */
	CASSERT((_this != 0));

	/* get the font collection */
	fc = _this->collection;

	/* reference this family, synchronously */
	CMutex_Lock(fc->lock);
	{
		CFontCollection_ReferenceFamily(fc, _this);
	}
	CMutex_Unlock(fc->lock);
}

/*\
|*| Get a font face.
|*|
|*|   _this - this font family
|*|   style - style of face
|*|    face - face (returned)
|*|
|*|  NOTE: the collection must NOT be locked
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontFamily_GetFace(CFontFamily  *_this,
                    CFontStyle    style,
                    CFontFace   **face)
{
	/* declarations */
	CFontFace **slot;
	CStatus     status;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((face  != 0));

	/* get the face slot for the given style */
	slot = (_this->faces + (style & 3));

	/* set the status to the default */
	status = CStatus_OK;

	/* get or create the face, synchronously */
	CMutex_Lock(_this->collection->lock);
	{
		/* create the face, as needed */
		if(*slot == 0)
		{
			status = CFontFace_Create(slot, _this, (style & 3));
		}
	}
	CMutex_Unlock(_this->collection->lock);

	/* get the face */
	*face = *slot;

	/* return status */
	return status;
}

/* Create a font family. */
CStatus
CFontFamily_CreateName(CFontFamily     **_this,
                       const CChar16    *name,
                       CFontCollection  *collection)
{
	/* declarations */
	FcChar8         *fname;
	CFontCollection *fc;
	CStatus          status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a name pointer */
	CStatus_Require((name != 0), CStatus_ArgumentNull);

	/* ensure we have a font collection */
	if(collection == 0)
	{
		/* create an installed font collection */
		CStatus_Check(CFontCollection_CreateInstalled(&fc));
	}
	else
	{
		/* set the font collection */
		fc = collection;
	}

	/* get the font name in utf8 */
	CStatus_CheckGOTO
		(CUtils_Str16ToStr8(name, (CChar8 **)&fname), status, GOTO_Cleanup);

	/* create or reference the font family, synchronously */
	CMutex_Lock(fc->lock);
	{
		status = CFontCollection_GetNamedFamily(fc, fname, _this);
	}
	CMutex_Unlock(fc->lock);

GOTO_Cleanup:
	/* free the utf8 font name */
	CFree(fname);

	/*\
	|*| NOTE: the family getter of the collection updates
	|*|       the reference count of the collection as
	|*|       appropriate, so any references made to it
	|*|       here need to be destroyed in order to end
	|*|       up with the correct reference count
	\*/

	/* destroy the font collection */
	if(collection == 0) { CFontCollection_Destroy(&fc); }

	/* return status */
	return status;
}

/* Create a generic font family. */
CStatus
CFontFamily_CreateGeneric(CFontFamily        **_this,
                          CFontFamilyGeneric   generic)
{
	/* declarations */
	CFontCollection *fc;
	CStatus          status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure the generic family type is in range */
	CFontFamilyGeneric_Default(generic);

	/* create or reference an installed font collection */
	CStatus_Check(CFontCollection_CreateInstalled(&fc));

	/* create or reference the font family, synchronously */
	CMutex_Lock(fc->lock);
	{
		status = CFontCollection_GetGenericFamily(fc, generic, _this);
	}
	CMutex_Unlock(fc->lock);

	/*\
	|*| NOTE: the family getter of the collection updates
	|*|       the reference count of the collection as
	|*|       appropriate, so any references made to it
	|*|       here need to be destroyed in order to end
	|*|       up with the correct reference count
	\*/

	/* destroy the font collection */
	CFontCollection_Destroy(&fc);

	/* return successfully */
	return CStatus_OK;
}

/* Destroy a font family. */
CStatus
CFontFamily_Destroy(CFontFamily **_this)
{
	/* declarations */
	CFontCollection *fc;
	CMutex          *lock;
	CBool            lockOwner;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require((*_this != 0), CStatus_ArgumentNull);

	/* get the font collection */
	fc = (*_this)->collection;

	/* get the font collection lock */
	lock = fc->lock;

	/* set the lock ownership flag to the default */
	lockOwner = 0;

	/* dereference this family, synchronously */
	CMutex_Lock(lock);
	{
		/* dereference this family */
		CFontCollection_DereferenceFamily(fc, *_this);

		/* finalize the font collection, as needed */
		if(fc->refCount == 0)
		{
			/* finalize the font collection */
			lockOwner = CFontCollection_Finalize(fc);

			/* free the font collection */
			CFree(fc);
		}
	}
	CMutex_Unlock(lock);

	/* destroy the lock, as needed */
	if(lockOwner) { CMutex_Destroy(&lock); }

	/* null the family */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Get the metrics of a font family and style. */
CStatus
CFontFamily_GetMetrics(CFontFamily  *_this,
                       CFontStyle    style,
                       CFontMetrics *metrics)
{
	/* declarations */
	CFontFace *face;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a metrics pointer */
	CStatus_Require((metrics != 0), CStatus_ArgumentNull);

	/* get the face for the given style */
	CStatus_Check(CFontFamily_GetFace(_this, style, &face));

	/* handle the unavailable case */
	if(face == CFontFace_Unavailable)
	{
		return CStatus_Argument_StyleNotAvailable;
	}

	/* get the metrics information */
	metrics->emHeight    =  (face->face->units_per_EM);
	metrics->cellAscent  =  (face->face->ascender);
	metrics->cellDescent = -(face->face->descender);
	metrics->lineSpacing =  (face->face->height);

	/* return successfully */
	return CStatus_OK;
}

/* Get the name of a font family. */
CStatus
CFontFamily_GetName(CFontFamily  *_this,
                    CChar16     **name)
{
	/* declarations */
	FcResult  result;
	FcChar8  *s;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a name pointer */
	CStatus_Require((name != 0), CStatus_ArgumentNull);

	/* get the family name from the pattern */
	result = FcPatternGetString(_this->_base.pattern, FC_FAMILY, 0, &s);

	/* ensure we have a family name */
	CASSERT((result == FcResultMatch));

	/* convert the name to utf16 */
	CStatus_Check(CUtils_Str8ToStr16((CChar8 *)s, name));

	/* return successfully */
	return CStatus_OK;
}

/* Determine if a style is available for a font family. */
CStatus
CFontFamily_IsStyleAvailable(CFontFamily *_this,
                             CFontStyle   style,
                             CBool       *available)
{
	/* declarations */
	CFontFace *face;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a style available flag pointer */
	CStatus_Require((available != 0), CStatus_ArgumentNull);

	/* get the face for the given style */
	CStatus_Check(CFontFamily_GetFace(_this, style, &face));

	/* get the available flag */
	*available = (face != CFontFace_Unavailable);

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
