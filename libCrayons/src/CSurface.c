/*
 * CSurface.c - Surface implementation.
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

#include "CSurface.h"

#ifdef __cplusplus
extern "C" {
#endif

CStatus
CSurface_Reference(CSurface *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* update the reference count synchronously */
	CSurface_Lock(_this);
	{
		++(_this->refCount);
	}
	CSurface_Unlock(_this);

	/* return successfully */
	return CStatus_OK;
}

CStatus
CSurface_Destroy(CSurface **_this)
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

	/* finalize this surface synchronously */
	CMutex_Lock(lock);
	{
		/* update the reference count */
		--((*_this)->refCount);

		/* finalize, as needed */
		if((*_this)->refCount != 0)
		{
			lockOwner = 0;
		}
		else
		{
			/* set the lock ownership flag */
			lockOwner = 1;

			/* finalize the clip mask, as needed */
			if((*_this)->clip != 0)
			{
				pixman_image_destroy((*_this)->clip);
			}

			/* finalize the compositing mask, as needed */
			if((*_this)->comp != 0)
			{
				pixman_image_destroy((*_this)->comp);
			}

			/* finalize the surface */
			(*_this)->_class->Finalize(*_this);

			/* dispose of the surface */
			CFree(*_this);
		}
	}
	CMutex_Unlock(lock);

	/* destroy the lock, as needed */
	if(lockOwner) { CMutex_Destroy(&lock); }

	/* null the surface pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

CStatus
CSurface_GetBounds(CSurface *_this,
                   CUInt32  *x,
                   CUInt32  *y,
                   CUInt32  *width,
                   CUInt32  *height)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have bounds pointers */
	CStatus_Require((x      != 0), CStatus_ArgumentNull);
	CStatus_Require((y      != 0), CStatus_ArgumentNull);
	CStatus_Require((width  != 0), CStatus_ArgumentNull);
	CStatus_Require((height != 0), CStatus_ArgumentNull);

	/* set the width and height, synchronously */
	CSurface_Lock(_this);
	{
		*x      = _this->x;
		*y      = _this->y;
		*width  = _this->width;
		*height = _this->height;
	}
	CSurface_Unlock(_this);

	/* return successfully */
	return CStatus_OK;
}

CStatus
CSurface_SetBounds(CSurface *_this,
                   CUInt32   x,
                   CUInt32   y,
                   CUInt32   width,
                   CUInt32   height)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the status to the default */
	status = CStatus_OK;

	/* set the bounds, synchronously */
	CSurface_Lock(_this);
	{
		/* bail out now if nothing has changed */
		if(x     == _this->x     && y      == _this->y &&
		   width == _this->width && height == _this->height)
		{
			status = CStatus_OK;
			goto GOTO_Cleanup;
		}

		/* ensure the width is within bounds */
		if(!(width > 0 && (x + width) < 32768))
		{
			status = CStatus_ArgumentOutOfRange;
			goto GOTO_Cleanup;
		}

		/* ensure the height is within bounds */
		if(!(height > 0 && (y + height) < 32768))
		{
			status = CStatus_ArgumentOutOfRange;
			goto GOTO_Cleanup;
		}

		/* set the width and height */
		_this->x      = x;
		_this->y      = y;
		_this->width  = width;
		_this->height = height;

		/* finalize the clip mask, as needed */
		if(_this->clip != 0)
		{
			pixman_image_destroy(_this->clip);
			_this->clip = 0;
		}

		/* finalize the compositing mask, as needed */
		if(_this->comp != 0)
		{
			pixman_image_destroy(_this->comp);
			_this->comp = 0;
		}
	}
GOTO_Cleanup:
	CSurface_Unlock(_this);

	/* return status */
	return status;
}

CINTERNAL void
CSurface_Lock(CSurface *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* lock this surface */
	CMutex_Lock(_this->lock);
}

CINTERNAL void
CSurface_Unlock(CSurface *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* unlock this surface */
	CMutex_Unlock(_this->lock);
}

CINTERNAL CRectangleF
CSurface_GetBoundsF(CSurface *_this)
{
	/* declarations */
	CRectangleF bounds;

	/* assertions */
	CASSERT((_this != 0));

	/* set the width and height */
	CRectangle_X(bounds)      = _this->x;
	CRectangle_Y(bounds)      = _this->y;
	CRectangle_Width(bounds)  = _this->width;
	CRectangle_Height(bounds) = _this->height;

	/* return the bounds */
	return bounds;
}

CINTERNAL CStatus
CSurface_GetClipMask(CSurface        *_this,
                     pixman_image_t **mask,
                     CBool            gray)
{
	/* declarations */
	pixman_format_t *format;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((mask  != 0));

	/* bail out now if there's nothing to do */
	if(_this->clip != 0 && !(_this->maskFlags & CSurface_ClipMask8) == !gray)
	{
		*mask = _this->clip;
		return CStatus_OK;
	}

	/* dispose of the clip mask, as needed */
	if(_this->clip != 0)
	{
		pixman_image_destroy(_this->clip);
		_this->clip = 0;
	}

	/* create the pixman format */
	if(gray)
	{
		format = pixman_format_create(PIXMAN_FORMAT_NAME_A8);
		_this->maskFlags |= CSurface_ClipMask8;
	}
	else
	{
		format = pixman_format_create(PIXMAN_FORMAT_NAME_A1);
		_this->maskFlags &= ~CSurface_ClipMask8;
	}

	/* ensure we have a format */
	CStatus_Require((format != 0), CStatus_OutOfMemory);

	/* create the pixman image */
	*mask = pixman_image_create(format, _this->width, _this->height);

	/* dispose of the format */
	pixman_format_destroy(format);

	/* ensure we have an image */
	CStatus_Require((*mask != 0), CStatus_OutOfMemory);

	/* set the clip mask */
	_this->clip = *mask;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_GetCompositingMask(CSurface        *_this,
                            pixman_image_t **mask,
                            CBool            gray)
{
	/* declarations */
	pixman_format_t *format;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((mask  != 0));

	/* bail out now if there's nothing to do */
	if(_this->comp != 0 && !(_this->maskFlags & CSurface_CompMask8) == !gray)
	{
		*mask = _this->comp;
		return CStatus_OK;
	}

	/* dispose of the compositing mask, as needed */
	if(_this->comp != 0)
	{
		pixman_image_destroy(_this->comp);
		_this->comp = 0;
	}

	/* create the pixman format */
	if(gray)
	{
		format = pixman_format_create(PIXMAN_FORMAT_NAME_A8);
		_this->maskFlags |= CSurface_CompMask8;
	}
	else
	{
		format = pixman_format_create(PIXMAN_FORMAT_NAME_A1);
		_this->maskFlags &= ~CSurface_CompMask8;
	}

	/* ensure we have a format */
	CStatus_Require((format != 0), CStatus_OutOfMemory);

	/* create the pixman image */
	*mask = pixman_image_create(format, _this->width, _this->height);

	/* dispose of the format */
	pixman_format_destroy(format);

	/* ensure we have an image */
	CStatus_Require((*mask != 0), CStatus_OutOfMemory);

	/* set the compositing mask */
	_this->comp = *mask;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_Composite(CSurface           *_this,
                   CInt32              x,
                   CInt32              y,
                   CUInt32             width,
                   CUInt32             height,
                   pixman_image_t     *src,
                   pixman_image_t     *mask,
                   CInterpolationMode  interpolationMode,
                   CCompositingMode    compositingMode)
{
	/* declarations */
	pixman_operator_t op;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a source pointer */
	CStatus_Require((src != 0), CStatus_ArgumentNull);

	/* ensure we have a mask pointer */
	CStatus_Require((mask != 0), CStatus_ArgumentNull);

	/* adjust bounds of composite */
	{
		/* get the bounds */
		CUInt32 x1 = _this->x;
		CUInt32 y1 = _this->y;
		CUInt32 x2 = _this->width  + x1;
		CUInt32 y2 = _this->height + x2;

		/* calculate the new bounds */
		if(x            > x1) { x1 = x; }
		if(y            > y1) { y1 = y; }
		if((x + width)  < x2) { x2 = (x + width); }
		if((y + height) < y2) { y2 = (y + height); }

		/* bail out now if there's nothing to do */
		CStatus_Require((x1 < x2 && y1 < y2), CStatus_OK);

		/* set the bounds */
		x      = x1;
		y      = y1;
		width  = (x2 - x1);
		height = (y2 - y1);
	}

	/* set the filter */
	switch(interpolationMode)
	{
		case CInterpolationMode_Bicubic:
		case CInterpolationMode_HighQuality:
		case CInterpolationMode_HighQualityBilinear:
		case CInterpolationMode_HighQualityBicubic:
			{ pixman_image_set_filter(src, PIXMAN_FILTER_BEST); } break;

		case CInterpolationMode_NearestNeighbor:
			{ pixman_image_set_filter(src, PIXMAN_FILTER_NEAREST); } break;

		case CInterpolationMode_Bilinear:
			{ pixman_image_set_filter(src, PIXMAN_FILTER_BILINEAR); } break;

		case CInterpolationMode_LowQuality:
			{ pixman_image_set_filter(src, PIXMAN_FILTER_FAST); } break;

		case CInterpolationMode_Default:
		default:
			{ pixman_image_set_filter(src, PIXMAN_FILTER_GOOD); } break;
	}

	/* set the operator */
	switch(compositingMode)
	{
		case CCompositingMode_SourceCopy:
			{ op = PIXMAN_OPERATOR_SRC; } break;

		case CCompositingMode_Xor:
			{ op = PIXMAN_OPERATOR_XOR; } break;

		case CCompositingMode_SourceOver:
		default:
			{ op = PIXMAN_OPERATOR_OVER; } break;
	}

	/* perform the compositing operation */
	CStatus_Check
		(_this->_class->Composite
			(_this, x, y, width, height, src, mask, op));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_Clear(CSurface *_this,
               CColor    color)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* clear the surface */
	CStatus_Check
		(_this->_class->Clear
			(_this, color));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_Flush(CSurface        *_this,
               CFlushIntention  intention)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* flush the surface */
	CStatus_Check
		(_this->_class->Flush
			(_this, intention));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_GetDpiX(CSurface *_this,
                 CFloat   *dpiX)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a horizontal resolution pointer */
	CStatus_Require((dpiX != 0), CStatus_ArgumentNull);

	/* get the horizontal resolution */
	CStatus_Check
		(_this->_class->GetDpiX
			(_this, dpiX));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_GetDpiY(CSurface *_this,
                 CFloat   *dpiY)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a vertical resolution pointer */
	CStatus_Require((dpiY != 0), CStatus_ArgumentNull);

	/* get the vertical resolution */
	CStatus_Check
		(_this->_class->GetDpiY
			(_this, dpiY));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CSurface_Initialize(CSurface            *_this,
                    const CSurfaceClass *_class,
                    CUInt32              x,
                    CUInt32              y,
                    CUInt32              width,
                    CUInt32              height)
{
	/* assertions */
	CASSERT((_this != 0));

	/* set the bounds */
	_this->x      = x;
	_this->y      = y;
	_this->width  = width;
	_this->height = height;

	/* set the class */
	_this->_class = _class;

	/* set the reference count */
	_this->refCount = 1;

	/* set the masks and mask flags */
	_this->clip      = 0;
	_this->comp      = 0;
	_this->maskFlags = 0;

	/* create the lock */
	return CMutex_Create(&(_this->lock));
}

#ifdef __cplusplus
};
#endif
