/*
 * CFontCollection.c - Font collection implementation.
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

#include "CFontCollection.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/*\
|*| NOTE: To prevent deadlock, locking must always be performed
|*|       from the top down, starting at the highest element
|*|       being accessed, in the following hierarchy:
|*|
|*|               CFontCollection
|*|               CFontFamily(*)
|*|               CFontFace
|*|
|*|         (*) families have no locks of their own, and so
|*|             mutually exclusive access to them must be
|*|             accomplished using collection locks
|*|
|*|       Unlocking should be performed in the reverse order of
|*|       locking.
\*/

/*\
|*| NOTE: Though font collections hold references to font families,
|*|       this is not counted in the reference counts of the
|*|       families. The families reference the collections as well,
|*|       and this is counted in the reference counts of the
|*|       collections iff the family has a reference count greater
|*|       than zero. When families reach a zero reference count,
|*|       they are placed on a small queue, providing a little bit
|*|       of family caching. The only exception is with the generic
|*|       families of an installed collection; generics are always
|*|       kept around for the life of the installed collection, so
|*|       long as it is known that they are generic. Generics may
|*|       not be known if they are first created by name, as they
|*|       are only discovered when specifically requested.
\*/

/* maintain mutually exclusive access to the installed font collection */
static CMutex CFontCollection_InstalledLock = CMutex_StaticInitializer;

/* declare the installed font collection (created on demand) */
static CFontCollection *CFontCollection_Installed = 0;

/* define the generic serif names */
static const FcChar8 *CFontFamily_GenericSerifNames[] =
	{
		(const FcChar8 *)"Serif",
		(const FcChar8 *)"Times New Roman",
		(const FcChar8 *)"DejaVu Serif",
		(const FcChar8 *)"FreeSerif",
		(const FcChar8 *)"Bitstream Vera Serif"
	};
static const CUInt32 CFontFamily_GenericSerifNamesLength =
	(sizeof(CFontFamily_GenericSerifNames) / sizeof(FcChar8 *));

/* define the generic sans serif names */
static const FcChar8 *CFontFamily_GenericSansSerifNames[] =
	{
		(const FcChar8 *)"Sans",
		(const FcChar8 *)"Sans Serif",
		(const FcChar8 *)"MS Sans Serif",
		(const FcChar8 *)"Arial",
		(const FcChar8 *)"Helvetica",
		(const FcChar8 *)"DejaVu Sans",
		(const FcChar8 *)"FreeSans",
		(const FcChar8 *)"Bitstream Vera Sans"
	};
static const CUInt32 CFontFamily_GenericSansSerifNamesLength =
	(sizeof(CFontFamily_GenericSansSerifNames) / sizeof(FcChar8 *));

/* define the generic monospace names */
static const FcChar8 *CFontFamily_GenericMonospaceNames[] =
	{
		(const FcChar8 *)"Monospace",
		(const FcChar8 *)"Courier New",
		(const FcChar8 *)"DejaVu Sans Mono",
		(const FcChar8 *)"FreeMono",
		(const FcChar8 *)"Bitstream Vera Sans Mono"
	};
static const CUInt32 CFontFamily_GenericMonospaceNamesLength =
	(sizeof(CFontFamily_GenericMonospaceNames) / sizeof(FcChar8 *));

/*\
|*| Update the font set of this collection as needed.
|*|
|*|   _this - this font collection
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_UpdateFonts(CFontCollection *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* update fonts, as needed */
	if(_this->fonts == 0)
	{
		/* declarations */
		FcObjectSet *objects;
		FcPattern   *pattern;
		FcFontSet   *tmp;

		/* create the font config obect set */
		if(!(objects = FcObjectSetBuild(FC_FAMILY, FC_FOUNDRY, 0)))
		{
			return CStatus_OutOfMemory;
		}

		/* create the font config pattern */
		if(!(pattern = FcPatternBuild(0, FC_SCALABLE, FcTypeBool, FcTrue, 0)))
		{
			FcObjectSetDestroy(objects);
			return CStatus_OutOfMemory;
		}

		/* get all the scalable font families */
		tmp = FcFontList(_this->config, pattern, objects);

		/* destroy the font config object set */
		FcObjectSetDestroy(objects);

		/* destroy the font config pattern */
		FcPatternDestroy(pattern);

		/* ensure we have a font set */
		CStatus_Require((tmp != 0), CStatus_OutOfMemory);

		/* set the font set */
		_this->fonts = tmp;
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Create a pattern matching a pattern.
|*|
|*|     _this - this font collection (must be locked)
|*|   pattern - search pattern
|*|     match - matching pattern (created)
|*|
|*|  NOTE: this method modifies the search pattern by
|*|        performing standard pattern substitutions
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_MatchPattern(CFontCollection  *_this,
                             FcPattern        *pattern,
                             FcPattern       **match)
{
	/* declarations */
	FcResult result;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));
	CASSERT((match   != 0));

	/*\
	|*| NOTE: fontconfig doesn't appear to have a result
	|*|       value for out of memory conditions, and to
	|*|       make matters worse it doesn't even appear
	|*|       to consistently set the result value on
	|*|       failures (e.g. if a null configuration is
	|*|       passed to FcFontMatch and FcConfigGetCurrent
	|*|       fails, the result value is left unchanged)
	\*/

	/* set the result to the default (hack around fontconfig) */
	result = FcResultMatch;

	/* set up the pattern for font matching */
	FcConfigSubstitute(_this->config, pattern, FcMatchPattern);
	FcDefaultSubstitute(pattern);

	/* get the family pattern */
	if(!(*match = FcFontMatch(_this->config, pattern, &result)))
	{
		/* return status based on result */
		if(result == FcResultNoMatch)
		{
			return CStatus_Argument_FontFamilyNotFound;
		}
		else
		{
			return CStatus_OutOfMemory;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Create a pattern matching a family name.
|*|
|*|   _this - this font collection (must be locked)
|*|    name - family name
|*|   match - matching pattern (created)
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_MatchFamilyName(CFontCollection  *_this,
                                FcChar8          *name,
                                FcPattern       **match)
{
	/* declarations */
	FcPattern *pattern;
	CStatus    status;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((name  != 0));
	CASSERT((match != 0));

	/* create the font config pattern */
	pattern =
		FcPatternBuild
			(0,
			 FC_FAMILY,   FcTypeString, name,
			 FC_SCALABLE, FcTypeBool,   FcTrue,
			 0);

	/* handle pattern creation failures */
	if(!pattern)
	{
		*match = 0;
		return CStatus_OutOfMemory;
	}

	/* get the matching family pattern */
	status = CFontCollection_MatchPattern(_this, pattern, match);

	/* destroy the pattern */
	FcPatternDestroy(pattern);

	/* return status */
	return status;
}

/*\
|*| Create a pattern matching a family pattern.
|*|
|*|    _this - this font collection (must be locked)
|*|   family - family pattern
|*|    match - matching pattern (created)
|*|
|*|  NOTE: the family pattern should be like those which
|*|        are in the font sets of collections, and
|*|        currently this means that it must contain a
|*|        family and a foundry
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_MatchFamilyPattern(CFontCollection  *_this,
                                   FcPattern        *family,
                                   FcPattern       **match)
{
	/* declarations */
	FcPattern *pattern;
	FcChar8   *name;
	FcChar8   *foundry;
	FcResult   result;
	CStatus    status;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));
	CASSERT((match  != 0));

	/* get the font family */
	result = FcPatternGetString(family, FC_FAMILY, 0, &name);

	/* ensure we have a family */
	CASSERT((result == FcResultMatch));

	/* get the font foundry */
	result = FcPatternGetString(family, FC_FOUNDRY, 0, &foundry);

	/* ensure we have a foundry */
	CASSERT((result == FcResultMatch));

	/* create the font config pattern */
	pattern =
		FcPatternBuild
			(0,
			 FC_FAMILY,   FcTypeString, name,
			 FC_FOUNDRY,  FcTypeString, foundry,
			 FC_SCALABLE, FcTypeBool,   FcTrue,
			 0);

	/* handle pattern creation failures */
	if(!pattern)
	{
		*match = 0;
		return CStatus_OutOfMemory;
	}

	/* get the matching family pattern */
	status = CFontCollection_MatchPattern(_this, pattern, match);

	/* destroy the pattern */
	FcPatternDestroy(pattern);

	/* return status */
	return status;
}

/*\
|*| Create a pattern matching a generic family.
|*|
|*|     _this - this font collection (must be locked)
|*|   generic - generic family type
|*|     match - matching pattern (created)
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_MatchGeneric(CFontCollection     *_this,
                             CFontFamilyGeneric   generic,
                             FcPattern          **match)
{
	/* declarations */
	const FcChar8 **names;
	const FcChar8 **end;
	FcPattern      *pattern;
	CStatus         status;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((match != 0));

	/* set the matching pattern to the default */
	*match = 0;

	/* get the names list */
	switch(generic)
	{
		case CFontFamilyGeneric_Serif:
		{
			names = CFontFamily_GenericSerifNames;
			end   = (names + CFontFamily_GenericSerifNamesLength);
		}
		break;
		case CFontFamilyGeneric_SansSerif:
		{
			names = CFontFamily_GenericSansSerifNames;
			end   = (names + CFontFamily_GenericSansSerifNamesLength);
		}
		break;
		case CFontFamilyGeneric_Monospace:
		default:
		{
			names = CFontFamily_GenericMonospaceNames;
			end   = (names + CFontFamily_GenericMonospaceNamesLength);
		}
		break;
	}

	/* create the font config pattern */
	if(!(pattern = FcPatternBuild(0, FC_SCALABLE, FcTypeBool, FcTrue, 0)))
	{
		CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_CleanupA);
	}

	/* add the family names */
	while(names != end)
	{
		/* add the current family name */
		if(!FcPatternAddString(pattern, FC_FAMILY, *names))
		{
			CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_CleanupB);
		}

		/* update the current family name pointer */
		++names;
	}

	/* get the matching family pattern */
	status = CFontCollection_MatchPattern(_this, pattern, match);

GOTO_CleanupB:
	/* destroy the pattern */
	FcPatternDestroy(pattern);

GOTO_CleanupA:
	/* return status */
	return status;
}

/*\
|*| Get a matching family from the family table.
|*|
|*|    _this - this font collection (must be locked)
|*|    match - matched family pattern pointer
|*|   family - returned family (created or referenced)
|*|
|*|  NOTE: this method assumes ownership of the matched
|*|        pattern, regardless of success, and nulls
|*|        the reference before returning, so a clone
|*|        of the pattern should be made if still needed
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_GetMatchingFamily(CFontCollection  *_this,
                                  FcPattern       **match,
                                  CFontFamily     **family)
{
	/* declarations */
	CStatus        status;
	CFontFamilyKey key;

	/* initialize the key */
	CFontFamilyKey_Initialize(&key, *match);

	/* get the family from the table, if available */
	*family = CFontFamilyTable_GetEntry(&(_this->table), &key);

	/* create or reference the family */
	if(*family != 0)
	{
		/* destroy the matching pattern */
		FcPatternDestroy(*match);

		/* finalize the key */
		CFontFamilyKey_Finalize(&key);

		/* reference the family */
		CFontCollection_ReferenceFamily(_this, *family);
	}
	else
	{
		/* allocate the font family */
		if(!(*family = (CFontFamily *)CMalloc(sizeof(CFontFamily))))
		{
			CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailA);
		}

		/* initialize the font family */
		CFontFamily_Initialize(*family, _this, &key);

		/* add the font family to the table */
		CStatus_CheckGOTO
			(CFontFamilyTable_AddEntry(&(_this->table), *family), status,
			 GOTO_FailB);

		/* reference this collection */
		++(_this->refCount);
	}

	/* null the matched pattern */
	*match = 0;

	/* return successfully */
	return CStatus_OK;

	/* handle failures */
	{
	GOTO_FailB:
		/* finalize the family */
		CFontFamily_Finalize(*family);

		/* null the matching pattern (destroyed by family finalization) */
		*match = 0;

		/* free the family */
		CFree(*family);

		/* null the family */
		*family = 0;

	GOTO_FailA:
		/* finalize the matching pattern and key, as needed */
		if(*match != 0)
		{
			CFontFamilyKey_Finalize(&key);
			FcPatternDestroy(*match);
			*match = 0;
		}

		/* return status */
		return status;
	}
}

/*\
|*| Get a family from the family table.
|*|
|*|     _this - this font collection (must be locked)
|*|   pattern - family search pattern
|*|    family - returned family (created or referenced)
|*|
|*|  Returns status code.
\*/
static CStatus
CFontCollection_GetFamily(CFontCollection  *_this,
                          FcPattern        *pattern,
                          CFontFamily     **family)
{
	/* declarations */
	CFontFamilyKey key;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));
	CASSERT((family  != 0));

	/* initialize the key */
	CFontFamilyKey_Initialize(&key, pattern);

	/* get the family from the table, if available */
	*family = CFontFamilyTable_GetEntry(&(_this->table), &key);

	/* finalize the key */
	CFontFamilyKey_Finalize(&key);

	/* create or reference the family */
	if(family != 0)
	{
		/* reference the family */
		CFontCollection_ReferenceFamily(_this, *family);
	}
	else
	{
		/* declarations */
		CStatus    status;
		FcPattern *match;

		/* get the matching pattern */
		status = CFontCollection_MatchFamilyPattern(_this, pattern, &match);

		/* handle pattern creation failures */
		if(status != CStatus_OK)
		{
			*family = 0;
			return status;
		}

		/* get or create the matching family */
		CStatus_Check
			(CFontCollection_GetMatchingFamily
				(_this, &match, family));
	}

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Finalize this font collection.
|*|
|*|   _this - this font collection (must be locked)
|*|
|*|  NOTE: this method may be called from the collection
|*|        destructor, if no externally referenced families
|*|        remain, and there is only the one remaining
|*|        external reference to this collection, or this
|*|        method may be called from the family destructor,
|*|        if there are no external references to this
|*|        collection, and the last remaining external
|*|        reference to a family of this collection is
|*|        destroyed... in either case the collection
|*|        should be locked, and that lock should be
|*|        destroyed, or not, based solely on the value
|*|        returned by this method
|*|
|*|  Returns true if the lock should be destroyed, false otherwise.
\*/
CINTERNAL CBool
CFontCollection_Finalize(CFontCollection *_this)
{
	/* declarations */
	CBool lockOwner;

	/* assertions */
	CASSERT((_this           != 0));
	CASSERT((_this->refCount == 0));

	/* set the lock ownership flag to the default */
	lockOwner = 1;

	/* destroy the font set, as needed */
	if(_this->fonts != 0)
	{
		FcFontSetDestroy(_this->fonts);
	}

	/* destroy the font configuration, as needed */
	if(_this->config != 0)
	{
		FcConfigDestroy(_this->config);
	}

	/* finalize the family table */
	CFontFamilyTable_Finalize(&(_this->table));

	/*\
	|*| NOTE: destruction of the temporary file list must take
	|*|       place after the above, to ensure that any open
	|*|       handles to the temporary files are closed before
	|*|       their deletion, so that we avoid trouble on
	|*|       retarded operating systems
	\*/

	/* destroy the temporary file list, as needed */
	if(_this->tempFiles != 0)
	{
		CTempFileList_Destroy(&(_this->tempFiles));
	}

	/* null any global references and determine lock ownership */
	if(_this == CFontCollection_Installed)
	{
		CFontCollection_Installed = 0;
		lockOwner = 0;
	}

	/* return lock ownership flag */
	return lockOwner;
}

/*\
|*| Create a pattern matching a family and style.
|*|
|*|    _this - this font collection (must be locked)
|*|    style - font style
|*|   family - family pattern
|*|    match - matching pattern (created)
|*|
|*|  NOTE: the family pattern should be like those which
|*|        are in the font sets of collections, and
|*|        currently this means that it must contain a
|*|        family and a foundry
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontCollection_MatchStyle(CFontCollection  *_this,
                           CFontStyle        style,
                           FcPattern        *family,
                           FcPattern       **match)
{
	/* declarations */
	FcPattern *pattern;
	FcChar8   *name;
	FcChar8   *foundry;
	FcResult   result;
	int        slant;
	int        weight;
	CStatus    status;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));
	CASSERT((match  != 0));

	/* get the font family */
	result = FcPatternGetString(family, FC_FAMILY, 0, &name);

	/* ensure we have a family */
	CASSERT((result == FcResultMatch));

	/* get the font foundry */
	result = FcPatternGetString(family, FC_FOUNDRY, 0, &foundry);

	/* ensure we have a foundry */
	CASSERT((result == FcResultMatch));

	/* get the font weight */
	if((style & CFontStyle_Bold) == CFontStyle_Bold)
	{
		weight = FC_WEIGHT_BOLD;
	}
	else
	{
		weight = FC_WEIGHT_MEDIUM;
	}

	/* get the font slant */
	if((style & CFontStyle_Italic) == CFontStyle_Italic)
	{
		slant = FC_SLANT_ITALIC;
	}
	else
	{
		slant = FC_SLANT_ROMAN;
	}

	/* create the font config pattern */
	pattern =
		FcPatternBuild
			(0,
			 FC_FAMILY,   FcTypeString,  name,
			 FC_FOUNDRY,  FcTypeString,  foundry,
			 FC_SLANT,    FcTypeInteger, slant,
			 FC_WEIGHT,   FcTypeInteger, weight,
			 FC_SCALABLE, FcTypeBool,    FcTrue,
			 0);

	/* handle pattern creation failures */
	if(!pattern)
	{
		*match = 0;
		return CStatus_OutOfMemory;
	}

	/* get the matching family pattern */
	status = CFontCollection_MatchPattern(_this, pattern, match);

	/* destroy the pattern */
	FcPatternDestroy(pattern);

	/* return status */
	return status;
}

/*\
|*| Reference a family, uncaching as necessary.
|*|
|*|    _this - this font collection (must be locked)
|*|   family - font family to reference
\*/
CINTERNAL void
CFontCollection_ReferenceFamily(CFontCollection *_this,
                                CFontFamily     *family)
{
	/* assertions */
	CASSERT((_this            != 0));
	CASSERT((family           != 0));
	CASSERT((family->refCount != ((CUInt32)-1)));

	/* unsave the family, as needed */
	if(family->refCount == 0)
	{
		/* unsave the family */
		CFontFamilyTable_Unsave(&(_this->table), family);

		/* reference this collection */
		++(_this->refCount);
	}

	/* reference the family */
	++(family->refCount);
}

/*\
|*| Dereference a family, caching as necessary.
|*|
|*|    _this - this font collection (must be locked)
|*|   family - font family to dereference
\*/
CINTERNAL void
CFontCollection_DereferenceFamily(CFontCollection *_this,
                                  CFontFamily     *family)
{
	/* assertions */
	CASSERT((_this            != 0));
	CASSERT((family           != 0));
	CASSERT((family->refCount != 0));

	/* dereference the family */
	--(family->refCount);

	/* save the family, as needed */
	if(family->refCount == 0)
	{
		/* save the family */
		CFontFamilyTable_Save(&(_this->table), family);

		/* dereference this collection */
		--(_this->refCount);
	}
}

/*\
|*| Get a named family from the family table.
|*|
|*|    _this - this font collection (must be locked)
|*|     name - family name
|*|   family - returned family (created or referenced)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontCollection_GetNamedFamily(CFontCollection  *_this,
                               FcChar8          *name,
                               CFontFamily     **family)
{
	/* declarations */
	FcPattern *match;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((name   != 0));
	CASSERT((family != 0));

	/* get the matching family pattern */
	CStatus_Check
		(CFontCollection_MatchFamilyName
			(_this, name, &match));

	/* get or create the matching family */
	CStatus_Check
		(CFontCollection_GetMatchingFamily
			(_this, &match, family));

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Get a generic family from the family table.
|*|
|*|     _this - this font collection (must be locked)
|*|   generic - generic family type
|*|    family - returned family (created or referenced)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CFontCollection_GetGenericFamily(CFontCollection     *_this,
                                 CFontFamilyGeneric   generic,
                                 CFontFamily        **family)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((family != 0));

	/* create or reference the generic */
	if((*family = CFontFamilyTable_GetGeneric(&(_this->table), generic)) != 0)
	{
		CFontCollection_ReferenceFamily(_this, *family);
	}
	else
	{
		/* declarations */
		FcPattern *match;

		/* get the matching pattern */
		CStatus_Check
			(CFontCollection_MatchGeneric
				(_this, generic, &match));

		/* get or create the matching family */
		CStatus_Check
			(CFontCollection_GetMatchingFamily
				(_this, &match, family));

		/* set the generic family in the table */
		CFontFamilyTable_SetGeneric(&(_this->table), generic, *family);
	}

	/* return successfully */
	return CStatus_OK;
}

/* Create an installed font collection. */
CStatus
CFontCollection_CreateInstalled(CFontCollection **_this)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* create or reference the installed collection, synchronously */
	CMutex_Lock(&CFontCollection_InstalledLock);
	{
		/* create or reference the installed collection */
		if(CFontCollection_Installed != 0)
		{
			/* get the installed collection */
			*_this = CFontCollection_Installed;

			/* update the reference count */
			++(CFontCollection_Installed->refCount);
		}
		else
		{
			/* declarations */
			CFontCollection *fc;

			/* allocate the font collection */
			if(!(*_this = (CFontCollection *)CMalloc(sizeof(CFontCollection))))
			{
				CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailA);
			}

			/* get this font collection */
			fc = *_this;

			/* initialize the family table */
			CStatus_CheckGOTO
				(CFontFamilyTable_Initialize(&(fc->table)), status,
				 GOTO_FailB);

			/* initialize the members to the defaults */
			fc->config    = 0;
			fc->fonts     = 0;
			fc->lock      = &CFontCollection_InstalledLock;
			fc->refCount  = 1;
			fc->tempFiles = 0;

			/* update the font set */
			CStatus_CheckGOTO
				(CFontCollection_UpdateFonts(fc), status, GOTO_FailC);

			/* set the installed collection */
			CFontCollection_Installed = *_this;
		}
	}
	CMutex_Unlock(&CFontCollection_InstalledLock);

	/* return successfully */
	return CStatus_OK;

	/* handle failures */
	{
	GOTO_FailC:
		/* finalize the family table */
		CFontFamilyTable_Finalize(&((*_this)->table));

	GOTO_FailB:
		/* free the collection */
		CFree(*_this);

		/* null the collection */
		*_this = 0;

	GOTO_FailA:
		/* unlock the collection */
		CMutex_Unlock(&CFontCollection_InstalledLock);

		/* return status */
		return status;
	}
}

/* Create a private font collection. */
CStatus
CFontCollection_CreatePrivate(CFontCollection **_this)
{
	/* declarations */
	CFontCollection *fc;
	CStatus          status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the font collection */
	if(!(*_this = (CFontCollection *)CMalloc(sizeof(CFontCollection))))
	{
		CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailA);
	}

	/* get this font collection */
	fc = ((CFontCollection *)(*_this));

	/* initialize the font collection */
	{
		/* create the font configuration */
		if(!(fc->config = FcConfigCreate()))
		{
			CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailB);
		}

		/* initialize the family table */
		CStatus_CheckGOTO
			(CFontFamilyTable_Initialize(&(fc->table)), status, GOTO_FailC);

		/* create the temporary file list */
		CStatus_CheckGOTO
			(CTempFileList_Create(&(fc->tempFiles)), status, GOTO_FailD);

		/* create the mutex */
		CStatus_CheckGOTO
			(CMutex_Create(&(fc->lock)), status, GOTO_FailE);

		/* initialize the remaining members */
		fc->fonts    = 0;
		fc->refCount = 1;
	}

	/* return successfully */
	return CStatus_OK;

	/* handle failures */
	{
	GOTO_FailE:
		/* destroy the temporary file list */
		CTempFileList_Destroy(&(fc->tempFiles));

	GOTO_FailD:
		/* finalize the family table */
		CFontFamilyTable_Finalize(&(fc->table));

	GOTO_FailC:
		/* destroy the font configuration */
		FcConfigDestroy(fc->config);

	GOTO_FailB:
		/* free the font collection */
		CFree(*_this);

		/* null the font collection */
		*_this = 0;

	GOTO_FailA:
		/* return status */
		return status;
	}
}

/* Destroy a font collection. */
CStatus
CFontCollection_Destroy(CFontCollection **_this)
{
	/* declarations */
	CMutex *lock;
	CBool   lockOwner;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require((*_this != 0), CStatus_ArgumentNull);

	/* get the lock */
	lock = (*_this)->lock;

	/* finalize and dispose of the font collection, synchronously */
	CMutex_Lock(lock);
	{
		/* declarations */
		CFontCollection *fc;

		/* get the font collection */
		fc = *_this;

		/* update the reference count */
		--(fc->refCount);

		/* finalize, as needed */
		if(fc->refCount == 0)
		{
			/* finalize the font collection */
			lockOwner = CFontCollection_Finalize(*_this);

			/* free the font collection */
			CFree(*_this);
		}
		else
		{
			/* the remaining references own the lock */
			lockOwner = 0;
		}
	}
	CMutex_Unlock(lock);

	/* destroy the lock, as needed */
	if(lockOwner) { CMutex_Destroy(&lock); }

	/* null the font family pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Add a font file to this private collection. */
CStatus
CFontCollection_AddFontFile(CFontCollection *_this,
                            const CChar16   *filename)
{
	/* declarations */
	CChar8  *fname;
	CStatus  status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a file name pointer */
	CStatus_Require((filename != 0), CStatus_ArgumentNull);

	/* ensure this is a private collection */
	CStatus_Require((_this->config != 0), CStatus_InvalidOperation);

	/* convert the file name to utf8 */
	CStatus_Check(CUtils_Str16ToStr8(filename, &fname));

	/* determine if file exists */
	if(!CUtils_FileExists(fname))
	{
		CFree(fname);
		return CStatus_IOError_FileNotFound;
	}

	/* add the font file, synchronously */
	CMutex_Lock(_this->lock);
	{
		/* add the font file */
		if(!FcConfigAppFontAddFile(_this->config, (FcChar8 *)fname))
		{
			status = CStatus_OutOfMemory;
		}
		else
		{
			/* set the status to success */
			status = CStatus_OK;

			/* dispose of the old font set, as needed */
			if(_this->fonts != 0) { FcFontSetDestroy(_this->fonts); }

			/* null the font set */
			_this->fonts = 0;
		}
	}
	CMutex_Unlock(_this->lock);

	/* dispose of the utf8 file name */
	CFree(fname);

	/* return status */
	return status;
}

/* Add a memory font to this private collection. */
CStatus
CFontCollection_AddFontMemory(CFontCollection *_this,
                              const CByte     *memory,
                              CUInt32          length)
{
	/* declarations */
	CStatus  status;
	CChar8  *filename;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a memory pointer */
	CStatus_Require((memory != 0), CStatus_ArgumentNull);

	/* ensure we have memory */
	CStatus_Require((length != 0), CStatus_Argument);

	/* ensure this is a private collection */
	CStatus_Require((_this->config != 0), CStatus_InvalidOperation);

	/* add the memory font */
	{
		/* declarations */
		CIOHandle handle;
		CUInt32   written;

		/* create a temporary file */
		CStatus_Check
			(CUtils_CreateTemporaryFile
				(&filename, &handle));

		/* write the data to the file */
		status = CUtils_WriteIOHandle(handle, memory, length, &written);

		/* close the file */
		CUtils_CloseIOHandle(handle);

		/* handle write failures */
		if(status != CStatus_OK) { goto GOTO_FailA; }

		/* add the font, synchronously */
		CMutex_Lock(_this->lock);
		{
			/* add the file to the temporary file list */
			CStatus_CheckGOTO
				(CTempFileList_AddFile(_this->tempFiles, filename), status,
				 GOTO_FailB);

			/* add the font to the collection */
			if(!FcConfigAppFontAddFile(_this->config, filename))
			{
				CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailB);
			}

			/* dispose of the old font set, as needed */
			if(_this->fonts != 0) { FcFontSetDestroy(_this->fonts); }

			/* null the font set */
			_this->fonts = 0;
		}
		CMutex_Unlock(_this->lock);
	}

	/* return successfully */
	return CStatus_OK;

	/* handle failures */
	{
	GOTO_FailB:
		/* unlock this collection */
		CMutex_Unlock(_this->lock);

	GOTO_FailA:
		/* delete the temporary file */
		CUtils_DeleteFile(filename);

		/* free the filename */
		CFree(filename);

		/* return status */
		return status;
	}
}

/* Get a list of the font families in this collection. */
CStatus
CFontCollection_GetFamilyList(CFontCollection   *_this,
                              CFontFamily     ***families,
                              CUInt32           *count)
{
	/* declarations */
	FcPattern    **currP;
	CFontFamily ***currF;
	CFontFamily ***end;
	CStatus        status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a font family list pointer */
	CStatus_Require((families != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the font families, synchronously */
	CMutex_Lock(_this->lock);
	{
		/* declarations */
		CUInt32 size;

		/* update the font set */
		CStatus_CheckGOTO
			(CFontCollection_UpdateFonts(_this), status, GOTO_FailA);

		/* get the count */
		if(!(*count = _this->fonts->nfont))
		{
			CStatus_CheckGOTO(CStatus_OK, status, GOTO_FailA);
		}

		/* calculate the list size */
		size = ((*count) * sizeof(CFontFamily *));

		/* allocate the font family list */
		if(!(*families = (CFontFamily **)CMalloc(size)))
		{
			CStatus_CheckGOTO(CStatus_OutOfMemory, status, GOTO_FailA);
		}

		/* get the pattern pointer */
		currP = _this->fonts->fonts;

		/* get the font family pointer */
		currF = families;

		/* get the end pointer */
		end = (currF + (*count));

		/* create the font families */
		while(currF != end)
		{
			/* get or create the family */
			CStatus_CheckGOTO
				(CFontCollection_GetFamily(_this, *currP, *currF),
				 status,
				 GOTO_FailB);

			/* move to the next position */
			++currP; ++currF;
		}
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;

	/* handle failures and cleanup */
	{
	GOTO_FailB:
		/* get the end pointer */
		end = families;

		/* dereference all the previously retrieved families */
		while(currF != end)
		{
			--currF;
			CFontCollection_DereferenceFamily(_this, **currF);
		}

		/* free the family list */
		CFree(*families);

	GOTO_FailA:
		/* unlock this collection */
		CMutex_Unlock(_this->lock);

		/* null the family list */
		*families = 0;

		/* set the count to zero */
		*count = 0;

		/* return status */
		return status;
	}
}


#ifdef __cplusplus
};
#endif
