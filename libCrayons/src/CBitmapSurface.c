/*
 * CBitmapSurface.c - Bitmap surface implementation.
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

#include "CBitmapSurface.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

CStatus
CBitmapSurface_Create(CBitmapSurface **_this,
                      CBitmap         *image)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an image pointer */
	CStatus_Require((image != 0), CStatus_ArgumentNull);

	/* allocate the bitmap surface */
	if(!(*_this = (CBitmapSurface *)CMalloc(sizeof(CBitmapSurface))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the bitmap surface */
	if(!((*_this)->image = (CBitmap *)CImage_Reference((CImage *)image)))
	{
		CFree(*_this);
		*_this = 0;
		return CStatus_OutOfMemory;
	}

	/* initialize the base */
	{
		/* declarations */
		CStatus   status;
		CSurface *surface;

		/* get this as a surface */
		surface = (CSurface *)*_this;

		/* initialize the base */
		status =
			CSurface_Initialize
				(surface, &CBitmapSurface_Class, 0, 0, image->width,
				 image->height);

		/* handle base initialization failures */
		if(status != CStatus_OK)
		{
			CBitmapSurface_Finalize(surface);
			CFree(*_this);
			*_this = 0;
			return status;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CBitmapSurface_Composite(CSurface          *_this,
                         CUInt32            x,
                         CUInt32            y,
                         CUInt32            width,
                         CUInt32            height,
                         pixman_image_t    *src,
                         pixman_image_t    *mask,
                         pixman_operator_t  op)
{
	/* declarations */
	CBitmap *image;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((src   != 0));
	CASSERT((mask  != 0));

	/* get the image */
	image = ((CBitmapSurface *)_this)->image;

	/* perform the composite synchronously */
	CMutex_Lock(image->lock);
	{
		/* ensure the image data isn't locked */
		if(image->locked)
		{
			CMutex_Unlock(image->lock);
			return CStatus_InvalidOperation_ImageLocked;
		}

		/* perform the composite */
		pixman_composite
			(op, src, mask, image->image, 0, 0, 0, 0, x, y, width, height);
	}
	CMutex_Unlock(image->lock);

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CBitmapSurface_Clear(CSurface *_this,
                     CColor    color)
{
	/* declarations */
	CBitmap        *image;
	pixman_color_t  pixel;

	/* assertions */
	CASSERT((_this != 0));

	/* create the pixel */
	pixel = CUtils_ToPixmanColor(color);

	/* get the image */
	image = ((CBitmapSurface *)_this)->image;

	/* perform the clear synchronously */
	CMutex_Lock(image->lock);
	{
		/* ensure the image data isn't locked */
		if(image->locked)
		{
			CMutex_Unlock(image->lock);
			return CStatus_InvalidOperation_ImageLocked;
		}

		/* clear the image */
		pixman_fill_rectangle
			(PIXMAN_OPERATOR_SRC, image->image, &pixel, _this->x, _this->y,
			 _this->width, _this->height);
	}
	CMutex_Unlock(image->lock);

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CBitmapSurface_Flush(CSurface        *_this,
                     CFlushIntention  intention)
{
	/* assertions */
	CASSERT((_this != 0));

	/* nothing to do here */

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CBitmapSurface_GetDpiX(CSurface *_this,
                       CFloat   *dpiX)
{
	/* declarations */
	CBitmap *image;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((dpiX  != 0));

	/* get the image */
	image = ((CBitmapSurface *)_this)->image;

	/* get the horizontal resolution, synchronously */
	CMutex_Lock(image->lock);
	{
		*dpiX = image->dpiX;
	}
	CMutex_Unlock(image->lock);

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CBitmapSurface_GetDpiY(CSurface *_this,
                       CFloat   *dpiY)
{
	/* declarations */
	CBitmap *image;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((dpiY  != 0));

	/* get the image */
	image = ((CBitmapSurface *)_this)->image;

	/* get the vertical resolution, synchronously */
	CMutex_Lock(image->lock);
	{
		*dpiY = image->dpiY;
	}
	CMutex_Unlock(image->lock);

	/* return successfully */
	return CStatus_OK;
}

static void
CBitmapSurface_Finalize(CSurface *_this)
{
	/* declarations */
	CBitmapSurface *surface;

	/* assertions */
	CASSERT((_this != 0));

	/* get this as a bitmap surface */
	surface = (CBitmapSurface *)_this;

	/* finalize the bitmap surface */
	CImage_Destroy((CImage **)&(surface->image));
}


#ifdef __cplusplus
};
#endif
