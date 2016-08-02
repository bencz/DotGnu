/*
 * CFontFamilyTable.h - Font family table header.
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

#ifndef _C_FONTFAMILYTABLE_H_
#define _C_FONTFAMILYTABLE_H_

#include "CFontFamily.h"
#include "CHashTable.h"

#ifdef __cplusplus
extern "C" {
#endif

#define CFontFamilyTable_MaxSaves    8
#define CFontFamilyTable_MaxGenerics 3
typedef struct _tagCFontFamilyTable CFontFamilyTable;
struct _tagCFontFamilyTable
{
	CHashTable         *families;
	CFontFamily        *generics[CFontFamilyTable_MaxGenerics];
	CFontFamily        *saves[CFontFamilyTable_MaxSaves];
	CUInt32             count;
	CFontFamilyEquals   keysEqual;
};

CINTERNAL CStatus
CFontFamilyTable_Initialize(CFontFamilyTable *_this);
CINTERNAL void
CFontFamilyTable_Finalize(CFontFamilyTable *_this);
CINTERNAL CFontFamily *
CFontFamilyTable_GetGeneric(CFontFamilyTable   *_this,
                            CFontFamilyGeneric  generic);
CINTERNAL void
CFontFamilyTable_SetGeneric(CFontFamilyTable   *_this,
                            CFontFamilyGeneric  generic,
                            CFontFamily        *family);
CINTERNAL void
CFontFamilyTable_Save(CFontFamilyTable *_this,
                      CFontFamily      *family);
CINTERNAL void
CFontFamilyTable_Unsave(CFontFamilyTable *_this,
                        CFontFamily      *family);
CINTERNAL CStatus
CFontFamilyTable_AddEntry(CFontFamilyTable *_this,
                          CFontFamily      *family);
CINTERNAL CFontFamily *
CFontFamilyTable_GetEntry(CFontFamilyTable *_this,
                          CFontFamilyKey   *key);

#ifdef __cplusplus
};
#endif

#endif /* _C_FONTFAMILYTABLE_H_ */
