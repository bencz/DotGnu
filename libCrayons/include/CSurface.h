/*
 * CSurface.h - Surface header.
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

#ifndef _C_SURFACE_H_
#define _C_SURFACE_H_

#include "CrayonsInternal.h"
#include "CMutex.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCSurfaceClass CSurfaceClass;

struct _tagCSurface
{
	const CSurfaceClass *_class;
	CMutex              *lock;
	CUInt32              refCount;
	CUInt32              x;
	CUInt32              y;
	CUInt32              width;
	CUInt32              height;
	pixman_image_t      *clip;
	pixman_image_t      *comp;
	CUInt32              maskFlags;
};

struct _tagCSurfaceClass
{
	/*\
	|*| Composite the image onto the surface.
	|*|
	|*|    _this - this surface
	|*|        x - destination x coordinate
	|*|        y - destination y coordinate
	|*|    width - width of composition
	|*|   height - height of composition
	|*|      src - source image
	|*|     mask - mask image
	|*|       op - compositing operator
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Composite)(CSurface          *_this,
	                     CUInt32            x,
	                     CUInt32            y,
	                     CUInt32            width,
	                     CUInt32            height,
	                     pixman_image_t    *src,
	                     pixman_image_t    *mask,
	                     pixman_operator_t  op);

	/*\
	|*| Clear the surface.
	|*|
	|*|   _this - this surface
	|*|   color - background color
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Clear)(CSurface *_this,
	                 CColor    color);

	/*\
	|*| Flush the surface.
	|*|
	|*|       _this - this surface
	|*|   intention - flush operation
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*Flush)(CSurface        *_this,
	                 CFlushIntention  intention);

	/*\
	|*| Get the horizontal resolution of the surface.
	|*|
	|*|   _this - this surface
	|*|    dpiX - horizontal resolution
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*GetDpiX)(CSurface *_this,
	                   CFloat   *dpiX);

	/*\
	|*| Get the vertical resolution of the surface.
	|*|
	|*|   _this - this surface
	|*|    dpiY - vertical resolution
	|*|
	|*|  Returns status code.
	\*/
	CStatus (*GetDpiY)(CSurface *_this,
	                   CFloat   *dpiY);

	/*\
	|*| Finalize the surface.
	|*|
	|*|   _this - this surface
	\*/
	void (*Finalize)(CSurface *_this);

	/*\
	|*| Sentinel string to catch missing methods in class tables.
	\*/
	const char *sentinel;
};

CINTERNAL void
CSurface_Lock(CSurface *_this);
CINTERNAL void
CSurface_Unlock(CSurface *_this);
CINTERNAL CRectangleF
CSurface_GetBoundsF(CSurface *_this);
CINTERNAL CStatus
CSurface_GetClipMask(CSurface        *_this,
                     pixman_image_t **mask,
                     CBool            gray);
CINTERNAL CStatus
CSurface_GetCompositingMask(CSurface        *_this,
                            pixman_image_t **mask,
                            CBool            gray);
CINTERNAL CStatus
CSurface_Composite(CSurface           *_this,
                   CInt32              x,
                   CInt32              y,
                   CUInt32             width,
                   CUInt32             height,
                   pixman_image_t     *src,
                   pixman_image_t     *mask,
                   CInterpolationMode  interpolationMode,
                   CCompositingMode    compositingMode);
CINTERNAL CStatus
CSurface_Clear(CSurface *_this,
               CColor    color);
CINTERNAL CStatus
CSurface_Flush(CSurface        *_this,
               CFlushIntention  intention);
CINTERNAL CStatus
CSurface_GetDpiX(CSurface *_this,
                 CFloat   *dpiX);
CINTERNAL CStatus
CSurface_GetDpiY(CSurface *_this,
                 CFloat   *dpiY);
CINTERNAL CStatus
CSurface_Initialize(CSurface            *_this,
                    const CSurfaceClass *_class,
                    CUInt32              x,
                    CUInt32              y,
                    CUInt32              width,
                    CUInt32              height);

#define CSurface_ClipMask8 1
#define CSurface_CompMask8 2

#ifdef __cplusplus
};
#endif

#endif /* _C_SURFACE_H_ */
