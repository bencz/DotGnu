/*
 * CFontCollection.h - Font collection header.
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

#ifndef _C_FONTCOLLECTION_H_
#define _C_FONTCOLLECTION_H_

#include "CrayonsInternal.h"
#include "CTempFileList.h"
#include "CFontFamilyTable.h"
#include "CMutex.h"
#include <fontconfig/fontconfig.h>

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCFontCollection
{
	FcConfig         *config;
	FcFontSet        *fonts;
	CFontFamilyTable  table;
	CMutex           *lock;
	CUInt32           refCount;
	CTempFileList    *tempFiles;
};

CINTERNAL CBool
CFontCollection_Finalize(CFontCollection *_this);
CINTERNAL CStatus
CFontCollection_MatchStyle(CFontCollection  *_this,
                           CFontStyle        style,
                           FcPattern        *family,
                           FcPattern       **match);
CINTERNAL void
CFontCollection_ReferenceFamily(CFontCollection *_this,
                                CFontFamily     *family);
CINTERNAL void
CFontCollection_DereferenceFamily(CFontCollection *_this,
                                  CFontFamily     *family);
CINTERNAL CStatus
CFontCollection_GetNamedFamily(CFontCollection  *_this,
                               FcChar8          *name,
                               CFontFamily     **family);
CINTERNAL CStatus
CFontCollection_GetGenericFamily(CFontCollection     *_this,
                                 CFontFamilyGeneric   generic,
                                 CFontFamily        **family);

#ifdef __cplusplus
};
#endif

#endif /* _C_FONTCOLLECTION_H_ */
