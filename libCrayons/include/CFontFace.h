/*
 * CFontFace.h - Font face header.
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

#ifndef _C_FONTFACE_H_
#define _C_FONTFACE_H_

#include "CrayonsInternal.h"
#include "CUnicodeHashTable.h"
#include "CGlyphCache.h"
#include "CMutex.h"
#include <ft2build.h>
#include FT_FREETYPE_H

#ifdef __cplusplus
extern "C" {
#endif

/* maximum cache memory per face */
#define CFontFace_MaxCacheMemory 65536

/* 26.6 fixed point conversions */
#define CFloat_ToF26Dot6(f)  ((FT_F26Dot6)(((CFloat) (f)) * 64))
#define CDouble_ToF26Dot6(f) ((FT_F26Dot6)(((CDouble)(f)) * 64))
#define CF26Dot6_ToFloat(f)  ((CFloat)    (((CFloat) (f)) / 64))
#define CF26Dot6_ToDouble(f) ((CDouble)   (((CDouble)(f)) / 64))
#define CF26Dot6_Floor(f)    ((FT_F26Dot6)(((f) +  0)  & ~63))
#define CF26Dot6_Ceil(f)     ((FT_F26Dot6)(((f) + 63)  & ~63))
#define CF26Dot6_Round(f)    ((FT_F26Dot6)(((f) + 32)  & ~63))
#define CF26Dot6_Trunc(f)    ((CInt32)    ((f) >> 6))
#define CF26Dot6_Zero        ((FT_F26Dot6) 0)
#define CF26Dot6_One         ((FT_F26Dot6)64)

typedef struct _tagCFontFace CFontFace;
struct _tagCFontFace
{
	FT_Face            face;
	FT_Library         library;
	CFontFamily       *family;
	CUnicodeHashTable *unicode;
	CGlyphCache       *cache;
	CGlyphShape       *current;
	CMutex            *lock;
};

extern CUInt32 CFontFace_UnavailableObject;
#define CFontFace_Unavailable \
	((CFontFace *)((void *)&CFontFace_UnavailableObject))

extern CUInt32 CGlyphEntry_NullMaskObject;
#define CGlyphEntry_NullMask \
	((pixman_image_t *)((void *)&CGlyphEntry_NullMaskObject))

CINTERNAL CStatus
CFontFace_Create(CFontFace   **_this,
                 CFontFamily  *family,
                 CFontStyle    style);
CINTERNAL void
CFontFace_Destroy(CFontFace **_this);
CINTERNAL CStatus
CFontFace_GetGlyphEntry(CFontFace    *_this,
                        CChar32       unicode,
                        CGlyphFlag    flags,
                        CBool         render,
                        CGlyphEntry **entry);
CINTERNAL CStatus
CFontFace_SetShape(CFontFace                *_this,
                   const CAffineTransformF  *device,
                   CFloat                    points,
                   CGlyphShape             **shape,
                   CFloat                   *scaleX,
                   CFloat                   *scaleY);
CINTERNAL void
CFontFace_DisableMemoryManagement(CFontFace *_this);
CINTERNAL void
CFontFace_EnableMemoryManagement(CFontFace *_this);
CINTERNAL void
CGlyphEntry_Destroy(CGlyphEntry **_this);
CINTERNAL void
CGlyphShape_Destroy(CGlyphShape **_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_FONTFACE_H_ */
