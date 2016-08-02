/*
 * CImage.c - Image implementation.
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

#include "CImage.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/******************************************************************************/
/* Define the frame dimensions. */
const CGuid CFrameDimension_Page =
{
	0x7462dc86, 0x6180, 0x4c7e, 0x8e, 0x3f, 0xee, 0x73, 0x33, 0xa7, 0xa4, 0x83
};
const CGuid CFrameDimension_Resolution =
{
	0x84236f7b, 0x3bd3, 0x428f, 0x8d, 0xab, 0x4e, 0xa1, 0x43, 0x9c, 0xa3, 0x15
};
const CGuid CFrameDimension_Time =
{
	0x6aedbd6d, 0x3fb5, 0x418a, 0x83, 0xa6, 0x7f, 0x45, 0x22, 0x9d, 0xc8, 0x72
};



/* Define the image formats. */
const CGuid CImageFormat_MemoryBMP =
{
	0xb96b3caa, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_BMP =
{
	0xb96b3cab, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_EMF =
{
	0xb96b3cac, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_WMF =
{
	0xb96b3cad, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_JPG =
{
	0xb96b3cae, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_PNG =
{
	0xb96b3caf, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_GIF =
{
	0xb96b3cb0, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_TIFF =
{
	0xb96b3cb1, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
const CGuid CImageFormat_EXIF =
{
	0xb96b3cb2, 0x0728, 0x11d3, 0x9d, 0x7b, 0x00, 0x00, 0xf8, 0x1e, 0xf3, 0x2e
};
/******************************************************************************/





















/******************************************************************************/
/* Create an image. */
#if 0
CStatus
CImage_CreateData(CImage  **_this,
                  CByte    *data,
                  CUInt32  count)
{
	/* TODO */
	return CStatus_NotImplemented;
}
CStatus
CImage_CreateFile(CImage  **_this,
                  CChar16  *filename,
                  CBool     useICM)
{
	/* TODO */
	return CStatus_NotImplemented;
}
CStatus
CImage_CreateStream(CImage  **_this,
                    CStream  *stream,
                    CBool     useICM)
{
	/* TODO */
	return CStatus_NotImplemented;
}
#endif

/* Reference this image. */
CINTERNAL CImage *
CImage_Reference(CImage *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reference this image synchronously */
	CMutex_Lock(_this->lock);
	{
		++(_this->refCount);
	}
	CMutex_Unlock(_this->lock);

	/* return this image */
	return _this;
}

/* Destroy this image. */
CStatus
CImage_Destroy(CImage **_this)
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

	/* finalize and dispose of the image */
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
			/* finalize the bitmap data */
			if((*_this)->locked)
			{
				CFree((*_this)->bitmapData);
				(*_this)->bitmapData = 0;
				(*_this)->locked = 0;
			}

			/* set the lock ownership flag */
			lockOwner = 1;

			/* finalize the palette */
			if((*_this)->palette != 0)
			{
				CColorPalette_Destroy(&((*_this)->palette));
			}

			/* finalize the pixman image */
			pixman_image_destroy((*_this)->image);

			/* dispose of the image */
			CFree(*_this);
		}
	}
	CMutex_Unlock(lock);

	/* destroy the lock, as needed */
	if(lockOwner) { CMutex_Destroy(&lock); }

	/* null the image pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Get the flags of this image. */
CStatus
CImage_GetFlags(CImage     *_this,
                CImageFlag *flags)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a flags pointer */
	CStatus_Require((flags != 0), CStatus_ArgumentNull);

	/* get the flags synchronously */
	CMutex_Lock(_this->lock);
	{
		*flags = _this->flags;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Get the height of this image. */
CStatus
CImage_GetHeight(CImage  *_this,
                 CUInt32 *height)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a height pointer */
	CStatus_Require((height != 0), CStatus_ArgumentNull);

	/* get the height synchronously */
	CMutex_Lock(_this->lock);
	{
		*height = _this->height;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Get the horizontal resolution of this image. */
CStatus
CImage_GetHorizontalResolution(CImage *_this,
                               CFloat *dpiX)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a horizontal resolution pointer */
	CStatus_Require((dpiX != 0), CStatus_ArgumentNull);

	/* get the horizontal resolution synchronously */
	CMutex_Lock(_this->lock);
	{
		*dpiX = _this->dpiX;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Get the type of this image. */
CStatus
CImage_GetImageType(CImage     *_this,
                    CImageType *type)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a type pointer */
	CStatus_Require((type != 0), CStatus_ArgumentNull);

	/* get the type */
	*type = _this->type;

	/* return successfully */
	return CStatus_OK;
}

/* Get the size of this image. */
CStatus
CImage_GetPhysicalDimension(CImage *_this,
                            CSizeF *size)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a size pointer */
	CStatus_Require((size != 0), CStatus_ArgumentNull);

	/* get the size synchronously */
	CMutex_Lock(_this->lock);
	{
		CSize_Width(*size)  = _this->width;
		CSize_Height(*size) = _this->height;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Get the pixel format of this image. */
CStatus
CImage_GetPixelFormat(CImage       *_this,
                      CPixelFormat *pixelFormat)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a pixel format pointer */
	CStatus_Require((pixelFormat != 0), CStatus_ArgumentNull);

	/* get the pixel format synchronously */
	CMutex_Lock(_this->lock);
	{
		*pixelFormat = _this->pixelFormat;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Get the raw format of this image. */
CStatus
CImage_GetRawFormat(CImage *_this,
                    CGuid  *format)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a raw format pointer */
	CStatus_Require((format != 0), CStatus_ArgumentNull);

	/* get the raw format */
	*format = _this->format;

	/* return successfully */
	return CStatus_OK;
}

/* Get the vertical resolution of this image. */
CStatus
CImage_GetVerticalResolution(CImage *_this,
                             CFloat *dpiY)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a vertical resolution pointer */
	CStatus_Require((dpiY != 0), CStatus_ArgumentNull);

	/* get the vertical resolution synchronously */
	CMutex_Lock(_this->lock);
	{
		*dpiY = _this->dpiY;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/* Get the width of this image. */
CStatus
CImage_GetWidth(CImage  *_this,
                CUInt32 *width)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a width pointer */
	CStatus_Require((width != 0), CStatus_ArgumentNull);

	/* get the width synchronously */
	CMutex_Lock(_this->lock);
	{
		*width = _this->width;
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}
/******************************************************************************/










/******************************************************************************/
/* Clone this image. */
CStatus
CImage_Clone(CImage  *_this,
             CImage **clone)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* create the clone synchronously */
	CMutex_Lock(_this->lock);
	{
		/* declarations */
		CStatus status;

		/* create the image clone */
		status =
			CBitmap_Create
				((CBitmap **)clone, _this->width, _this->height,
				 _this->pixelFormat);

		/* handle clone creation failures */
		if(status != CStatus_OK)
		{
			CMutex_Unlock(_this->lock);
			return status;
		}

		/* initialize the pixman image data */
		{
			/* declarations */
			CByte   *src;
			CByte   *dst;
			CUInt32  height;
			CUInt32  stride;

			/* get the pixman image data */
			src = (CByte *)pixman_image_get_data(_this->image);
			dst = (CByte *)pixman_image_get_data((*clone)->image);

			/* get the height */
			height = pixman_image_get_height(_this->image);

			/* get the stride */
			stride = pixman_image_get_stride(_this->image);

			/* copy the image data */
			CMemCopy(dst, src, (height * stride));
		}
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

/******************************************************************************/
















/******************************************************************************/
/* Get a bounding rectangle for this image. */
CStatus
CImage_GetBounds(CImage        *_this,
                 CGraphicsUnit  pageUnit,
                 CRectangleF   *bounds)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a bounds pointer */
	CStatus_Require((bounds != 0), CStatus_ArgumentNull);

	/* get the bounds synchronously */
	CMutex_Lock(_this->lock);
	{
		/* set the offsets */
		CRectangle_X(*bounds) = 0;
		CRectangle_Y(*bounds) = 0;

		/* get the width */
		CRectangle_Width(*bounds) =
			CUtils_ConvertUnitsDPI
				(CGraphicsUnit_Pixel, pageUnit,
				 _this->width, _this->dpiY, _this->dpiY);

		/* get the height */
		CRectangle_Height(*bounds) =
			CUtils_ConvertUnitsDPI
				(CGraphicsUnit_Pixel, pageUnit,
				 _this->height, _this->dpiX, _this->dpiX);
	}
	CMutex_Unlock(_this->lock);

	/* return successfully */
	return CStatus_OK;
}

#if 0
/* Get parameter information for a specific encoder. */
CStatus
CImage_GetEncoderParameters(CImage             *_this,
                            CGuid               encoder,
                            CEncoderParameter **parameters,
                            CUInt32            *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}
#endif

/* Get the raw image data of this image. */
CStatus
CImage_GetData(CImage   *_this,
               CByte   **data,
               CUInt32  *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/******************************************************************************/






/******************************************************************************/
/* Get the palette of this image. */
CStatus
CImage_GetPalette(CImage         *_this,
                  CColorPalette **palette)
{
#if 0
	/* declarations */
	CColorPalette *tmp;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a palette pointer pointer */
	CStatus_Require((palette != 0), CStatus_ArgumentNull);

	/* TODO: locking */

	/* get the palette */
	tmp = _this->palette;

	/* clone the palette, as needed */
	if(tmp == 0)
	{
		*palette = 0;
	}
	else
	{
		CStatus_Check
			(CColorPalette_Clone
				(tmp, palette));
	}

	/* return successfully */
	return CStatus_OK;
#else
	return CStatus_NotImplemented;
#endif
}

/* Set the palette of this image. */
CStatus
CImage_SetPalette(CImage        *_this,
                  CColorPalette *palette)
{
#if 0
	/* declarations */
	CPixelFormat pixelFormat;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a palette pointer */
	CStatus_Require((palette != 0), CStatus_ArgumentNull);

	/* TODO: locking */

	/* get the pixel format */
	pixelFormat = _this->pixelFormat;

	/* ensure we have an indexed frame */
	CStatus_Require
		((CPixelFormat_IsIndexed(pixelFormat)), CStatus_InvalidOperation);

	/* ensure we have a correctly sized palette */
	CStatus_Require
		((CColorPalette_CheckFormat(pixelFormat)), CStatus_Argument);

	/* set the palette */
	{
		/* declarations */
		CColorPalette *tmp, *old;

		/* get the old palette */
		old = _this->palette;

		/* clone the new palette */
		CStatus_Check
			(CColorPalette_Clone
				(palette, &tmp));

		/* dispose of the old palette, as needed */
		if(old != 0)
		{
			CColorPalette_Destroy(&(old));
		}

		/* set the palette */
		_this->palette = tmp;
	}

	/* return successfully */
	return CStatus_OK;
#else
	return CStatus_NotImplemented;
#endif
}

/******************************************************************************/









/******************************************************************************/
/* Get a specific property item. */
CStatus
CImage_GetPropertyItem(CImage        *_this,
                       CPropertyID    id,
                       CPropertyItem *item)
{
#if 0
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a property item pointer */
	CStatus_Require((item != 0), CStatus_ArgumentNull);

	/* get the property item */
	{
		/* declarations */
		CPropertyItem *curr;
		CPropertyItem *end;

		/* get the count */
		const CUInt32 count = _this->propertyItemCount;

		/* get the property item pointer */
		curr = _this->propertyItems;

		/* get the end pointer */
		end = (curr + count);

		/* get the property item */
		while(curr != end)
		{
			/* get the item if it matches */
			if(curr->id == id)
			{
				/* initialize the item */
				CStatus_Check
					(CPropertyItem_Initialize
						(item, id, curr->length, curr->type, curr->value));

				/* return successfully */
				return CStatus_OK;
			}

			/* move to the next property item */
			++curr;
		}
	}

	/* TODO: test GDI+ to see what it does here */

	/* return unsuccessfully */
	return CStatus_Argument;
#else
	return CStatus_NotImplemented;
#endif
}

/* Set a property on this image. */
CStatus
CImage_SetPropertyItem(CImage        *_this,
                       CPropertyItem *item)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Remove a specific property item. */
CStatus
CImage_RemovePropertyItem(CImage      *_this,
                          CPropertyID  id)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get a list of the property ids of this image. */
CStatus
CImage_GetPropertyIDs(CImage       *_this,
                      CPropertyID **ids,
                      CUInt32      *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get a list of the property items of this image. */
CStatus
CImage_GetPropertyItems(CImage         *_this,
                        CPropertyItem **propertyItems,
                        CUInt32        *count)
{
	/* TODO */
	return CStatus_NotImplemented;
}
/******************************************************************************/








/******************************************************************************/
/* Get a thumbnail version of this image. */
CStatus
CImage_GetThumbnailImage(CImage               *_this,
                         CUInt32               width,
                         CUInt32               height,
                         CImage              **thumbnail)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Rotate and/or flip this image. */
CStatus
CImage_RotateFlip(CImage          *_this,
                  CRotateFlipType  rotateFlipType)
{
	/* TODO */
	return CStatus_NotImplemented;
}
/******************************************************************************/







/******************************************************************************/
#if 0
/* Save this image. */
CStatus
CImage_Save(CImage            *_this,
            CStream           *stream,
            CGuid              encoder,
            CEncoderParameter *parameters,
            CUInt32            count)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Add a frame to the previously saved image file. */
CStatus
CImage_SaveAdd(CImage            *_this,
               CEncoderParameter *parameters,
               CUInt32            count)
{
	/* TODO */
	return CStatus_NotImplemented;
}
CStatus
CImage_SaveAddImage(CImage            *_this,
                    CImage            *image,
                    CEncoderParameter *parameters,
                    CUInt32            count)
{
	/* TODO */
	return CStatus_NotImplemented;
}
#endif
/******************************************************************************/










/******************************************************************************/
/* Get the number of frames in a specific dimension. */
CStatus
CImage_GetFrameCount(CImage  *_this,
                     CGuid    dimension,
                     CUInt32 *frameCount)
{
#if 0
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a frame count pointer */
	CStatus_Require((frameCount != 0), CStatus_ArgumentNull);

	/* get the frame count */
	{
		/* declarations */
		CFrameInfo *curr;
		CFrameInfo *end;

		/* get the frame information pointer */
		curr = _this->frameDimensions;

		/* get the end pointer */
		end = (curr + (_this->frameDimensionCount));

		/* get the frame count */
		while(curr != end)
		{
			/* get the frame count, if the dimension matches */
			if(CMemCmp(&(curr->guid), &dimension, sizeof(CGuid)) == 0)
			{
				/* set the frame count */
				*frameCount = curr->count;

				/* return successfully */
				return CStatus_OK;
			}

			/* move to the next frame information position */
			++curr;
		}
	}

	/* set the frame count */
	*frameCount = 0;

	/* return successfully */
	return CStatus_OK;
#else
	return CStatus_NotImplemented;
#endif
}

/* Get a list of the frame dimensions of this image. */
CStatus
CImage_GetFrameDimensions(CImage   *_this,
                          CGuid   **dimensions,
                          CUInt32  *count)
{
#if 0
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a frame dimension list pointer */
	CStatus_Require((dimensions != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the frame dimensions */
	{
		/* declarations */
		CFrameInfo *curr;
		CFrameInfo *end;
		CGuid      *dim;

		/* get the count */
		*count = _this->frameDimensionCount;

		/* allocate the frame dimension list */
		if(!(*dimensions = (CGuid *)CMalloc((*count) * sizeof(CGuid))))
		{
			return CStatus_OutOfMemory;
		}

		/* get the input pointer */
		curr = _this->frameDimensions;

		/* get the end of input pointer */
		end = (curr + (*count));

		/* get the output pointer */
		dim = *dimensions;

		/* get the frame dimensions */
		while(curr != end)
		{
			/* get the current guid */
			*dim = curr->guid;

			/* move to the next input position */
			++curr;

			/* move to the next output position */
			++dim;
		}
	}

	/* return successfully */
	return CStatus_OK;
#else
	return CStatus_NotImplemented;
#endif
}

/* Select a new frame and make it the active one. */
CStatus
CImage_SelectActiveFrame(CImage  *_this,
                         CGuid    dimension,
                         CUInt32  frameIndex)
{
#if 0
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we aren't locked (CBitmap_LockBits) */
	CStatus_Require
		((_this->lockMode == CImageLockMode_None), CStatus_InvalidOperation);

	/* select the frame */
	{
		/* declarations */
		CFrameInfo *curr;
		CFrameInfo *end;

		/* get the input pointer */
		curr = _this->frameDimensions;

		/* get the end of input pointer */
		end = (curr + (_this->frameDimensionCount));

		/* select the frame */
		while(curr != end)
		{
			/* select the frame from the matching dimension */
			if(CMemCmp(&(curr->guid), &dimension, sizeof(CGuid)) == 0)
			{
				/* declarations */
				CFrame *tmp;

				/* ensure we have a valid frame index */
				CStatus_Require
					((index < (curr->count)), CStatus_ArgumentOutOfRange);

				/* get the frame */
				CStatus_Check
					(CFrame_Clone
						(curr->frames[index], &tmp, CPixelFormat_32bppPArgb));

				/* dispose of the current active buffer frame */
				CFrame_Destroy(&(_this->activeBuffer));

				/* set the active frame */
				_this->active = &(curr->frames[index]);

				/* set the active buffer frame */
				_this->activeBuffer = tmp;

				/* return successfully */
				return CStatus_OK;
			}

			/* move to the next frame information position */
			++curr;
		}
	}

	/* return unsuccessfully */
	return CStatus_Argument;
#else
	return CStatus_NotImplemented;
#endif
}
/******************************************************************************/

#ifdef __cplusplus
};
#endif
