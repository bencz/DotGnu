/*
 * CUnicodeHashTable.h - Unicode hash table header.
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

#ifndef _C_UNICODEHASHTABLE_H_
#define _C_UNICODEHASHTABLE_H_

#include "CrayonsInternal.h"
#include <fontconfig/fontconfig.h>
#include <ft2build.h>
#include FT_FREETYPE_H

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCUnicodeHashTable CUnicodeHashTable;
typedef CUInt32 CGlyphIndex;

CINTERNAL CStatus
CUnicodeHashTable_Create(CUnicodeHashTable **_this,
                         FcCharSet          *charset,
                         FT_Face             face);
CINTERNAL void
CUnicodeHashTable_Destroy(CUnicodeHashTable **_this);
CINTERNAL CGlyphIndex
CUnicodeHashTable_GetGlyphIndex(CUnicodeHashTable *_this,
                                CChar32            unicode);

#ifdef __cplusplus
};
#endif

#endif /* _C_UNICODEHASHTABLE_H_ */
