/*
 * CFontFamily.h - Font family header.
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

#ifndef _C_FONTFAMILY_H_
#define _C_FONTFAMILY_H_

#include "CrayonsInternal.h"
#include "CFontFace.h"
#include "CHashTable.h"
#include <fontconfig/fontconfig.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCFontFamilyKey CFontFamilyKey;
struct _tagCFontFamilyKey
{
	CHashEntry  _base;
	FcPattern  *pattern;
};

typedef struct _tagCFontFamilyEquals CFontFamilyEquals;
struct _tagCFontFamilyEquals
{
	CPredicateBinary  _base;
	FcObjectSet      *os;
};

#define CFontFamily_MaxFaces 4
struct _tagCFontFamily
{
	CFontFamilyKey   _base;
	CFontCollection *collection;
	CFontFace       *faces[CFontFamily_MaxFaces];
	CUInt32          refCount;
};


CINTERNAL void
CFontFamilyKey_Initialize(CFontFamilyKey *_this,
                          FcPattern      *pattern);
CINTERNAL void
CFontFamilyKey_Finalize(CFontFamilyKey *_this);


CINTERNAL CStatus
CFontFamilyEquals_Initialize(CFontFamilyEquals *_this);
CINTERNAL void
CFontFamilyEquals_Finalize(CFontFamilyEquals *_this);


CINTERNAL void
CFontFamily_Initialize(CFontFamily     *_this,
                       CFontCollection *fc,
                       CFontFamilyKey  *key);
CINTERNAL void
CFontFamily_Finalize(CFontFamily *_this);
CINTERNAL void
CFontFamily_Reference(CFontFamily *_this);
CINTERNAL CStatus
CFontFamily_GetFace(CFontFamily  *_this,
                    CFontStyle    style,
                    CFontFace   **face);

#ifdef __cplusplus
};
#endif

#endif /* _C_FONTFAMILY_H_ */
