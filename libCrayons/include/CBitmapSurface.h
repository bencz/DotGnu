/*
 * CBitmapSurface.h - Bitmap surface header.
 *
 * Copyright (C) 2005  Free Software Foundation, Inc.
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

#ifndef _C_BITMAPSURFACE_H_
#define _C_BITMAPSURFACE_H_

#include "CBitmap.h"
#include "CSurface.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCBitmapSurface
{
	CBitmap *image;
};

static CStatus
CBitmapSurface_Composite(CSurface          *_this,
                         CUInt32            x,
                         CUInt32            y,
                         CUInt32            width,
                         CUInt32            height,
                         pixman_image_t    *src,
                         pixman_image_t    *mask,
                         pixman_operator_t  op);
static CStatus
CBitmapSurface_Clear(CSurface *_this,
                     CColor    color);
static CStatus
CBitmapSurface_Flush(CSurface        *_this,
                     CFlushIntention  intention);
static CStatus
CBitmapSurface_GetDpiX(CSurface *_this,
                       CFloat   *dpiX);
static CStatus
CBitmapSurface_GetDpiY(CSurface *_this,
                       CFloat   *dpiY);
static void
CBitmapSurface_Finalize(CSurface *_this);


static const CSurfaceClass CBitmapSurface_Class =
{
	CBitmapSurface_Composite,
	CBitmapSurface_Clear,
	CBitmapSurface_Flush,
	CBitmapSurface_GetDpiX,
	CBitmapSurface_GetDpiY,
	CBitmapSurface_Finalize,
	"sentinel"
};

#ifdef __cplusplus
};
#endif

#endif /* _C_BITMAPSURFACE_H_ */
