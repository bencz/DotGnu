/*
 * CImage.h - Image header.
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

#ifndef _C_IMAGE_H_
#define _C_IMAGE_H_

#include "CrayonsInternal.h"
#include "CMutex.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCImage
{
	CImageType      type;
	CImageFormat    format;
	CImageFlag      flags;
	CPixelFormat    pixelFormat;
	CFloat          dpiX;
	CFloat          dpiY;
	CUInt32         width;
	CUInt32         height;
#if 0
	CPropertyItem  *propertyItems;
	CUInt32         propertyItemCount;
	CFrameInfo     *frameDimensions;
	CUInt32         frameDimensionCount;
	CFrame         *active;
	CFrame         *activeBuffer;
#endif
	CUInt32         bitmapDataX;
	CUInt32         bitmapDataY;
	CBitmapData    *bitmapData;
	CBool           locked;
	CColorPalette  *palette;
	pixman_image_t *image;
	CMutex         *lock;
	CUInt32         refCount;
};

CINTERNAL CImage *
CImage_Reference(CImage *_this);

/*\
|*| NOTE: these declarations should be moved to the public
|*|       header once they're properly implemented
\*/
CStatus
CImage_GetData(CImage   *_this,
               CByte   **data,
               CUInt32  *count);
CStatus
CImage_GetPalette(CImage         *_this,
                  CColorPalette **palette);
CStatus
CImage_SetPalette(CImage        *_this,
                  CColorPalette *palette);
CStatus
CImage_GetPropertyItem(CImage        *_this,
                       CPropertyID    id,
                       CPropertyItem *item);
CStatus
CImage_SetPropertyItem(CImage        *_this,
                       CPropertyItem *item);
CStatus
CImage_RemovePropertyItem(CImage      *_this,
                          CPropertyID  id);
CStatus
CImage_GetPropertyIDs(CImage       *_this,
                      CPropertyID **ids,
                      CUInt32      *count);
CStatus
CImage_GetPropertyItems(CImage         *_this,
                        CPropertyItem **propertyItems,
                        CUInt32        *count);
CStatus
CImage_GetThumbnailImage(CImage               *_this,
                         CUInt32               width,
                         CUInt32               height,
                         CImage              **thumbnail);
CStatus
CImage_RotateFlip(CImage          *_this,
                  CRotateFlipType  rotateFlipType);
CStatus
CImage_GetFrameCount(CImage  *_this,
                     CGuid    dimension,
                     CUInt32 *frameCount);
CStatus
CImage_GetFrameDimensions(CImage   *_this,
                          CGuid   **dimensions,
                          CUInt32  *count);
CStatus
CImage_SelectActiveFrame(CImage  *_this,
                         CGuid    dimension,
                         CUInt32  frameIndex);

#ifdef __cplusplus
};
#endif

#endif /* _C_IMAGE_H_ */
