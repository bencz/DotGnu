/*
 * CTextureBrush.c - Texture brush implementation.
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

#include "CTextureBrush.h"
#include "CImage.h"
#include "CMatrix.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Clone this texture brush. */
static CStatus
CTextureBrush_Clone(CBrush  *_this,
                    CBrush **_clone)
{
	/* declarations */
	CTextureBrush  *brush;
	CTextureBrush **clone;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((_clone != 0));

	/* get this as a texture brush */
	brush = (CTextureBrush *)_this;

	/* get the clone as a texture brush */
	clone = (CTextureBrush **)_clone;

	/* create the clone */
	CStatus_Check
		(CTextureBrush_Create
			(clone, brush->image, brush->rectangle, brush->wrapMode));

	/* initialize the transformation of the clone */
	(*clone)->transform = brush->transform;

	/* return successfully */
	return CStatus_OK;
}

/* Finalize this texture brush. */
static void
CTextureBrush_Finalize(CBrush *_this)
{
	/* declarations */
	CTextureBrush *brush;

	/* assertions */
	CASSERT((_this != 0));

	/* get this as a texture brush */
	brush = (CTextureBrush *)_this;

	/* finalize the image */
	CImage_Destroy(&(brush->image));
}

/* Create a pattern for this brush. */
static CStatus
CTextureBrush_CreatePattern(CBrush   *_this,
                            CPattern *pattern)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* create the pattern */
	{
		/* declarations */
		CUInt32         x;
		CUInt32         y;
		CUInt32         w;
		CUInt32         h;
		CStatus         status;
		CTextureBrush  *brush;
		pixman_format_t *format;

		/* get this as a texture brush */
		brush = (CTextureBrush *)_this;

		/* set the pattern transformation */
		pattern->transform = &(brush->transform);

		/* get the rectangle dimensions */
		x = (CUInt32)CRectangle_X(brush->rectangle);
		y = (CUInt32)CRectangle_Y(brush->rectangle);
		w = (CUInt32)CRectangle_Width(brush->rectangle);
		h = (CUInt32)CRectangle_Height(brush->rectangle);

		/* TODO: handle different wrapping modes */

		/* create the pixman format */
		format = pixman_format_create(PIXMAN_FORMAT_NAME_ARGB32);

		/* ensure we have a format */
		CStatus_Require((format != 0), CStatus_OutOfMemory);

		/* create the pixman image */
		pattern->image = pixman_image_create(format, w, h);

		/* ensure we have an image */
		CStatus_Require((pattern->image != 0), CStatus_OutOfMemory);

		/* copy the image data */
		status =
			CUtils_PixmanImageRectangle
				(brush->image->image, pattern->image, x, y, w, h);

		/* handle copying failures */
		if(status != CStatus_OK)
		{
			pixman_image_destroy(pattern->image);
			pattern->image = 0;
			return status;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Intialize this texture brush. */
static CStatus
CTextureBrush_Initialize(CTextureBrush *_this,
                         CImage        *image,
                         CRectangleF    rectangle,
                         CWrapMode      wrapMode)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((image != 0));

	/* initialize the members */
	{
		/* clone the image */
		CStatus_Check
			(CImage_Clone
				(image, &(_this->image)));

		/* initialize the base */
		CBrush_Initialize((CBrush *)_this, &CTextureBrush_Class);

		/* initialize the remaining members */
		_this->rectangle = rectangle;
		_this->wrapMode  = wrapMode;
		_this->transform = CAffineTransformF_Identity;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Create a texture brush. */
CStatus
CTextureBrush_Create(CTextureBrush **_this,
                     CImage         *image,
                     CRectangleF     rectangle,
                     CWrapMode       wrapMode)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the brush */
	if(!(*_this = (CTextureBrush *)CMalloc(sizeof(CTextureBrush))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the brush */
	CStatus_Check
		(CTextureBrush_Initialize
			(*_this, image, rectangle, wrapMode));

	/* return successfully */
	return CStatus_OK;
}

/* Get the image of the texture. */
CStatus
CTextureBrush_GetImage(CTextureBrush  *_this,
                       CImage        **image)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an image pointer pointer */
	CStatus_Require((image != 0), CStatus_ArgumentNull);

	/* get the image */
	return CImage_Clone(_this->image, image);
}

/* Get the wrap mode of the texture. */
CStatus
CTextureBrush_GetWrapMode(CTextureBrush *_this,
                          CWrapMode     *wrapMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a wrap mode pointer */
	CStatus_Require((wrapMode != 0), CStatus_ArgumentNull);

	/* get the wrap mode */
	*wrapMode = _this->wrapMode;

	/* return successfully */
	return CStatus_OK;
}

/* Set the wrap mode of the texture. */
CStatus
CTextureBrush_SetWrapMode(CTextureBrush *_this,
                          CWrapMode      wrapMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the wrap mode */
	_this->wrapMode = wrapMode;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the transformation matrix of the texture. */
CStatus
CTextureBrush_GetTransform(CTextureBrush *_this,
                           CMatrix       *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the transformation */
	return CMatrix_SetTransform(matrix, &(_this->transform));
}

/* Multiply the transformation matrix of the texture by another matrix. */
CStatus
CTextureBrush_MultiplyTransform(CTextureBrush *_this,
                                CMatrix       *matrix,
                                CMatrixOrder   order)
{
	/* declarations */
	CAffineTransformF t;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the matrix transformation */
	CStatus_Check
		(CMatrix_GetTransform
			(matrix, &t));

	/* multiply the transformation */
	CAffineTransformF_Multiply(&(_this->transform), &t, order);

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Reset the transformation matrix of the texture. */
CStatus
CTextureBrush_ResetTransform(CTextureBrush *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* reset the transformation */
	_this->transform = CAffineTransformF_Identity;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Rotate the transformation matrix of the texture. */
CStatus
CTextureBrush_RotateTransform(CTextureBrush *_this,
                              CFloat         angle,
                              CMatrixOrder   order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* rotate the transformation */
	CAffineTransformF_Rotate(&(_this->transform), angle, order);

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Scale the transformation matrix of the texture. */
CStatus
CTextureBrush_ScaleTransform(CTextureBrush *_this,
                             CFloat         sx,
                             CFloat         sy,
                             CMatrixOrder   order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* scale the transformation */
	CAffineTransformF_Scale(&(_this->transform), sx, sy, order);

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Set the transformation matrix of the texture. */
CStatus
CTextureBrush_SetTransform(CTextureBrush *_this,
                           CMatrix       *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the transformation */
	CStatus_Check
		(CMatrix_GetTransform
			(matrix, &(_this->transform)));

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Translate the transformation matrix of the texture. */
CStatus
CTextureBrush_TranslateTransform(CTextureBrush *_this,
                                 CFloat         dx,
                                 CFloat         dy,
                                 CMatrixOrder   order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* translate the transformation */
	CAffineTransformF_Translate(&(_this->transform), dx, dy, order);

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

#ifdef __cplusplus
};
#endif
