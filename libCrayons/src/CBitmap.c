/*
 * CBitmap.c - Bitmap implementation.
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

#include "CBitmap.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Initialize this bitmap. */
static CStatus
CBitmap_Initialize(CBitmap      *_this,
                   CUInt32       width,
                   CUInt32       height,
                   CPixelFormat  format)
{
	/* declarations */
	pixman_format_t *fmt;

	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	_this->type        = CImageType_Bitmap;
	_this->format      = CImageFormat_MemoryBMP;
	_this->flags       = CImageFlag_None;
	_this->pixelFormat = format;
	_this->dpiX        = CGraphics_DefaultDpi;
	_this->dpiY        = CGraphics_DefaultDpi;
	_this->width       = width;
	_this->height      = height;
	_this->bitmapDataX = 0;
	_this->bitmapDataY = 0;
	_this->bitmapData  = 0;
	_this->locked      = 0;
	_this->palette     = 0;
	_this->refCount    = 1;

	/* create the mutex */
	CStatus_Check
		(CMutex_Create
			(&(_this->lock)));

	/* create the pixman format */
	if(!(fmt = pixman_format_create(PIXMAN_FORMAT_NAME_ARGB32)))
	{
		CMutex_Destroy(&(_this->lock));
		return CStatus_OutOfMemory;
	}

	/* create the pixman image */
	_this->image = pixman_image_create(fmt, width, height);

	/* free the format */
	pixman_format_destroy(fmt);

	/* handle image creation failures */
	if(_this->image == 0)
	{
		CMutex_Destroy(&(_this->lock));
		return CStatus_OutOfMemory;
	}

	/* initialize the pixman image data */
	{
		/* declarations */
		CByte   *d;
		CUInt32  h;
		CUInt32  s;

		/* get the pixman image information */
		d = (CByte *)pixman_image_get_data(_this->image);
		h = (CUInt32)pixman_image_get_height(_this->image);
		s = (CUInt32)pixman_image_get_stride(_this->image);

		/* initialize the data */
		CMemSet(d, 0x00, (h * s));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Create a bitmap. */
#if 0
CStatus
CBitmap_CreateStream(CBitmap **_this,
                     CStream  *stream,
                     CBool     useICM)
{
	/* TODO */
	return CStatus_NotImplemented;
}
CStatus
CBitmap_CreateFile(CBitmap **_this,
                   CChar16  *filename,
                   CBool     useICM)
{
	/* TODO */
	return CStatus_NotImplemented;
}
#endif

CStatus
CBitmap_Create(CBitmap      **_this,
               CUInt32        width,
               CUInt32        height,
               CPixelFormat   format)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the bitmap */
	if(!(*_this = (CBitmap *)CMalloc(sizeof(CBitmap))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the members */
	status = CBitmap_Initialize(*_this, width, height, format);

	/* handle initialization failures */
	if(status != CStatus_OK)
	{
		CFree(*_this);
		*_this = 0;
		return status;
	}

	/* return successfully */
	return CStatus_OK;
}

CStatus
CBitmap_CreateData(CBitmap      **_this,
                   CByte         *data,
                   CUInt32        width,
                   CUInt32        height,
                   CUInt32        stride,
                   CPixelFormat   format)
{
	/* declarations */
	CStatus status;

	/* ensure we have a data pointer */
	CStatus_Require((data != 0), CStatus_ArgumentNull);

	/* create the bitmap */
	CStatus_Check
		(CBitmap_Create
			(_this, width, height, format));

	/* initialize the pixman image data */
	status =
		CUtils_ToPixmanImage
			(format, data, (*_this)->image, 0, 0, width, height, stride,
			 (*_this)->palette);

	/* handle image data initialization failures */
	if(status != CStatus_OK)
	{
		CBitmap_Destroy(_this);
		return status;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Destroy this bitmap. */
CStatus
CBitmap_Destroy(CBitmap **_this)
{
	return CImage_Destroy(_this);
}

/* Clone this bitmap and transform it into a new pixel format. */
CStatus
CBitmap_Clone(CBitmap       *_this,
              CBitmap      **clone,
              CUInt32        x,
              CUInt32        y,
              CUInt32        width,
              CUInt32        height,
              CPixelFormat   format)
{
	/* declarations */
	CStatus     status;
	CBitmapData bd;

	/* lock the image data */
	CStatus_Check
		(CBitmap_LockBits
			(_this, x, y, width, height,
			 CImageLockMode_ReadOnly, format, &bd));

	/* create the bitmap */
	status =
		CBitmap_CreateData
			(clone, bd.scan0, width, height, bd.stride, format);

	/* unlock the image data */
	CBitmap_UnlockBits(_this, &bd);

	/* return status */
	return status;
}

/* Get the color of a specific pixel. */
CStatus
CBitmap_GetPixel(CBitmap *_this,
                 CUInt32  x,
                 CUInt32  y,
                 CColor  *color)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a color pointer */
	CStatus_Require((color != 0), CStatus_ArgumentNull);

	/* get the pixel synchronously */
	CMutex_Lock(_this->lock);
	{
		/* declarations */
		CStatus  status;
		CColor  *pixel;

		/* ensure the image data isn't locked */
		if(_this->locked)
		{
			CMutex_Unlock(_this->lock);
			return CStatus_InvalidOperation_ImageLocked;
		}

		/* get the pixel pointer */
		status =
			CUtils_GetPixmanPixelPointer
				(_this->image, x, y, &pixel);

		/* ensure we have a pixel pointer */
		if(status != CStatus_OK)
		{
			CMutex_Unlock(_this->lock);
			return status;
		}

		/* get the pixel color */
		{
			/* declarations */
			CByte a, r, g, b;

			/* get the pixel color components */
			CPixmanPixel_ToARGB(*pixel, a, r, g, b);

			/* get the pixel color based on transparency */
			if(a == 0)
			{
				*color = CColor_FromARGB(0, 0, 0, 0);
			}
			else
			{
				/* scale the components */
				b = (((b << 8) - b) / a);
				g = (((g << 8) - g) / a);
				r = (((r << 8) - r) / a);

				/* get the pixel color */
				*color = CColor_FromARGB(a, r, g, b);
			}
		}
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Set a pixel within this bitmap. */
CStatus
CBitmap_SetPixel(CBitmap *_this,
                 CUInt32  x,
                 CUInt32  y,
                 CColor   color)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the pixel synchronously */
	CMutex_Lock(_this->lock);
	{
		/* declarations */
		CStatus  status;
		CColor  *pixel;

		/* ensure the image data isn't locked */
		if(_this->locked)
		{
			CMutex_Unlock(_this->lock);
			return CStatus_InvalidOperation_ImageLocked;
		}

		/* get the pixel pointer */
		status =
			CUtils_GetPixmanPixelPointer
				(_this->image, x, y, &pixel);

		/* ensure we have a pixel pointer */
		if(status != CStatus_OK)
		{
			CMutex_Unlock(_this->lock);
			return status;
		}

		/* get the pixel value */
		{
			/* declarations */
			CByte a, r, g, b;

			/* get the color components */
			a = CColor_A(color);
			r = CColor_R(color);
			g = CColor_G(color);
			b = CColor_B(color);

			/* set the pixel color based on the transparency */
			if(a == 0)
			{
				*pixel = CPixmanPixel_FromARGB(0, 0, 0, 0);
			}
			else
			{
				*pixel =
					CPixmanPixel_FromARGB
						(a, ((r * a) / 255), ((g * a) / 255), ((b * a) / 255));
			}
		}
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Make the bitmap data from the current image data. */
static CStatus
CBitmap_MakeBitmapData(CBitmap        *_this,
                       CUInt32         x,
                       CUInt32         y,
                       CUInt32         width,
                       CUInt32         height,
                       CImageLockMode  lockMode,
                       CPixelFormat    format)
{
	/* declarations */
	CUInt32 stride;

	/* assertions */
	CASSERT((_this             != 0));
	CASSERT((_this->bitmapData == 0));

	/* ensure we have a valid lock mode */
	if(lockMode < CImageLockMode_ReadOnly ||
	   lockMode > CImageLockMode_ReadWrite)
	{
		return CStatus_Argument;
	}

	/* ensure the rectangle is in bounds */
	if(((width) == 0) ||
	   ((height) == 0) ||
	   ((x + width) > _this->width) ||
	   ((y + height) > _this->height))
	{
		return CStatus_ArgumentOutOfRange;
	}

	/* calculate the stride based on the pixel format */
	switch(format)
	{
		case CPixelFormat_1bppIndexed:
		case CPixelFormat_4bppIndexed:
		case CPixelFormat_8bppIndexed:
		case CPixelFormat_48bppRgb:
		case CPixelFormat_64bppArgb:
		case CPixelFormat_64bppPArgb:
			{ return CStatus_NotImplemented; }

		case CPixelFormat_16bppGrayScale:
		case CPixelFormat_16bppRgb555:
		case CPixelFormat_16bppRgb565:
		case CPixelFormat_16bppArgb1555:
		{
			stride = (((width << 1) + 3) & ~3);
		}
		break;

		case CPixelFormat_24bppRgb:
		{
			stride = ((((width << 1) + width) + 3) & ~3);
		}
		break;

		case CPixelFormat_32bppRgb:
		case CPixelFormat_32bppArgb:
		case CPixelFormat_32bppPArgb:
		{
			stride = (width << 2);
		}
		break;

		default: { return CStatus_NotSupported; }
	}

	/* create the bitmap data */
	{
		/* calculate the size */
		const CUInt32 size = (sizeof(CBitmapData) + (height * stride));

		/* allocate the bitmap data */
		if(!(_this->bitmapData = (CBitmapData *)CMalloc(size)))
		{
			return CStatus_OutOfMemory;
		}

		/* set the bitmap data scans */
		_this->bitmapData->scan0 =
			(((CByte *)_this->bitmapData) + sizeof(CBitmapData));

		/* set the remaining bitmap data members */
		_this->bitmapDataX             = x;
		_this->bitmapDataY             = y;
		_this->bitmapData->width       = width;
		_this->bitmapData->height      = height;
		_this->bitmapData->stride      = stride;
		_this->bitmapData->pixelFormat = format;
		_this->bitmapData->reserved    = lockMode;
	}

	/* copy the image data, as needed */
	if(lockMode & CImageLockMode_ReadOnly)
	{
		/* declarations */
		CStatus status;

		/* copy the image data */
		status =
			CUtils_FromPixmanImage
				(_this->bitmapData->pixelFormat, _this->image,
				 _this->bitmapData->scan0,       _this->bitmapDataX,
        		 _this->bitmapDataY,             _this->bitmapData->width,
				 _this->bitmapData->height,      _this->bitmapData->stride,
				 _this->palette);

		/* handle copying failures */
		if(status != CStatus_OK)
		{
			CFree(_this->bitmapData);
			_this->bitmapData = 0;
			return status;
		}
	}
	else
	{
		CMemSet(_this->bitmapData->scan0, 0x00, (height * stride));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Lock a region of this bitmap. */
CStatus
CBitmap_LockBits(CBitmap        *_this,
                 CUInt32         x,
                 CUInt32         y,
                 CUInt32         width,
                 CUInt32         height,
                 CImageLockMode  lockMode,
                 CPixelFormat    format,
                 CBitmapData    *bitmapData)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a bitmap data pointer */
	CStatus_Require((bitmapData != 0), CStatus_ArgumentNull);

	/* lock the bits synchronously */
	CMutex_Lock(_this->lock);
	{
		/* declarations */
		CStatus status;

		/* ensure the image data isn't locked */
		if(_this->locked)
		{
			CMutex_Unlock(_this->lock);
			return CStatus_InvalidOperation_ImageLocked;
		}

		/* create the bitmap data */
		status =
			CBitmap_MakeBitmapData
				(_this, x, y, width, height, lockMode, format);

		/* handle bitmap data creation failures */
		if(status != CStatus_OK)
		{
			CMutex_Unlock(_this->lock);
			return status;
		}

		/* set the locked flag */
		_this->locked = 1;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Set the resolution for this bitmap. */
CStatus
CBitmap_SetResolution(CBitmap *_this,
                      CFloat   dpiX,
                      CFloat   dpiY)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the resolution synchronously */
	CMutex_Lock(_this->lock);
	{
		/* ensure the image data isn't locked */
		if(_this->locked)
		{
			CMutex_Unlock(_this->lock);
			return CStatus_InvalidOperation_ImageLocked;
		}

		/* set the resolution */
		_this->dpiX = dpiX;
		_this->dpiY = dpiY;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Unlock the bits within this bitmap. */
CStatus
CBitmap_UnlockBits(CBitmap     *_this,
                   CBitmapData *bitmapData)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a bitmap data pointer */
	CStatus_Require((bitmapData != 0), CStatus_ArgumentNull);

	/* lock the bits synchronously */
	CMutex_Lock(_this->lock);
	{
		/* declarations */
		CStatus status;

		/* ensure the image data is locked */
		if(!(_this->locked))
		{
			CMutex_Unlock(_this->lock);
			return CStatus_InvalidOperation;
		}

		/* ensure the bitmap data is for this image */
		if(_this->bitmapData->scan0 != bitmapData->scan0)
		{
			CMutex_Unlock(_this->lock);
			return CStatus_Argument;
		}

		/* set the status to the default */
		status = CStatus_OK;

		/* set the image data, as needed */
		if(_this->bitmapData->reserved & CImageLockMode_WriteOnly)
		{
			/* copy the bitmap data to the image */
			status =
				CUtils_ToPixmanImage
					(_this->bitmapData->pixelFormat, _this->bitmapData->scan0,
					 _this->image,                   _this->bitmapDataX,
					 _this->bitmapDataY,             _this->bitmapData->width,
					 _this->bitmapData->height,      _this->bitmapData->stride,
					 _this->palette);
		}

		/* free the bitmap data */
		CFree(_this->bitmapData);

		/* reset the bitmap data */
		_this->bitmapData = 0;
		_this->locked     = 0;
		bitmapData->scan0 = 0;

		/* handle failures */
		if(status != CStatus_OK)
		{
			CMutex_Unlock(_this->lock);
			return status;
		}
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
