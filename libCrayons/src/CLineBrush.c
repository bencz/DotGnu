/*
 * CLineBrush.c - Linear gradient brush implementation.
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

#include "CLineBrush.h"
#include "CBlend.h"
#include "CMatrix.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* TODO: implement more than a glorified solid brush */

static const CRectangleF CRectangleF_Zero = { 0.0f, 0.0f, 0.0f, 0.0f };

static void
CLineBrush_Initialize(CLineBrush  *_this,
                      CRectangleF  rectangle,
                      CColor       startColor,
                      CColor       endColor,
                      CFloat       angle,
                      CBool        isAngleScalable,
                      CWrapMode    wrapMode)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	{
		_this->transform       = CAffineTransformF_Identity;
		_this->rectangle       = CRectangleF_Zero;
		_this->wrapMode        = wrapMode;
		_this->blend           = CBlend_Zero;
		_this->colorBlend      = CColorBlend_Zero;
		_this->startColor      = startColor;
		_this->endColor        = endColor;
		_this->angle           = angle;
		_this->isAngleScalable = (isAngleScalable != 0);
		_this->gammaCorrection = 0;
	}

	/* initialize the base */
	CBrush_Initialize((CBrush *)_this, &CLineBrush_Class);
}

/* Finalize this line brush. */
static void
CLineBrush_Finalize(CBrush *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize this brush */
	{
		/* declarations */
		CLineBrush *brush;

		/* get this as a line brush */
		brush = (CLineBrush *)_this;

		/* finalize the blend */
		CBlend_Finalize(&(brush->blend));

		/* finalize the color blend */
		CColorBlend_Finalize(&(brush->colorBlend));
	}
}

/* Clone this line brush. */
static CStatus
CLineBrush_Clone(CBrush  *_this,
                 CBrush **_clone)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((_clone != 0));

	/* clone this brush */
	{
		/* declarations */
		CLineBrush *brush;
		CLineBrush *clone;
		CStatus     status;

		/* allocate the clone */
		if(!(*_clone = (CBrush *)CMalloc(sizeof(CLineBrush))))
		{
			return CStatus_OutOfMemory;
		}

		/* get this as a line brush */
		brush = (CLineBrush *)_this;

		/* get the clone brush */
		clone = ((CLineBrush *)(*_clone));

		/* copy the blend */
		status = CBlend_Copy(&(brush->blend), &(clone->blend));

		/* handle blend copy failures */
		if(status != CStatus_OK)
		{
			CFree(*_clone);
			*_clone = 0;
			return status;
		}

		/* copy the color blend */
		status = CColorBlend_Copy(&(brush->colorBlend), &(clone->colorBlend));

		/* handle color blend copy failures */
		if(status != CStatus_OK)
		{
			CBlend_Finalize(&(clone->blend));
			CFree(*_clone);
			*_clone = 0;
			return status;
		}

		/* initialize the remaining clone members */
		clone->transform       = brush->transform;
		clone->rectangle       = brush->rectangle;
		clone->wrapMode        = brush->wrapMode;
		clone->startColor      = brush->startColor;
		clone->endColor        = brush->endColor;
		clone->angle           = brush->angle;
		clone->isAngleScalable = brush->isAngleScalable;
		clone->gammaCorrection = brush->gammaCorrection;
		
	}

	/* return successfully */
	return CStatus_OK;
}

/* Create a pattern for this brush. */
static CStatus
CLineBrush_CreatePattern(CBrush   *_this,
                         CPattern *pattern)
{
	/* declarations */
	CLineBrush *brush;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* TODO: implement this the right way */

	/* get this as a line brush */
	brush = (CLineBrush *)_this;

	/* set the pattern transformation */
	pattern->transform = 0;

	/* create the pattern */
	return CUtils_CreateSolidPattern(&(pattern->image), brush->startColor);
}

/* Create a line brush. */
CStatus
CLineBrush_Create(CLineBrush  **_this,
                  CRectangleF   rectangle,
                  CColor        startColor,
                  CColor        endColor,
                  CFloat        angle,
                  CBool         isAngleScalable,
                  CWrapMode     wrapMode)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the brush */
	if(!(*_this = (CLineBrush *)CMalloc(sizeof(CLineBrush))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the brush */
	CLineBrush_Initialize
		(*_this,
		 rectangle,
		 startColor,
		 endColor,
		 angle,
		 isAngleScalable,
		 wrapMode);

	/* return successfully */
	return CStatus_OK;
}

/* Get the gradient blend. */
CStatus
CLineBrush_GetBlend(CLineBrush *_this,
                    CBlend     *blend)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a blend pointer */
	CStatus_Require((blend != 0), CStatus_ArgumentNull);

	/* ensure we have a blend */
	CStatus_Require((_this->blend.count > 0), CStatus_InvalidOperation);

	/* get the blend */
	return CBlend_Copy(&(_this->blend), blend);
}

/* Set the gradient blend. */
CStatus
CLineBrush_SetBlend(CLineBrush *_this,
                    CBlend      blend)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have factors */
	CStatus_Require((blend.factors != 0), CStatus_ArgumentNull);

	/* ensure we have positions */
	CStatus_Require((blend.positions != 0), CStatus_ArgumentNull);

	/* ensure the count is in range */
	CStatus_Require((blend.count >= 2), CStatus_ArgumentOutOfRange);

	/* set the blend */
	{
		/* declarations */
		CBlend tmp;

		/* copy the blend */
		CStatus_Check
			(CBlend_Copy
				(&(blend), &(tmp)));

		/* dispose of the current blend as needed */
		if(_this->blend.count != 0)
		{
			CBlend_Finalize(&(_this->blend));
		}
		else
		{
			CColorBlend_Finalize(&(_this->colorBlend));
		}

		/* set the blend */
		_this->blend = tmp;
	}

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the start and end colors of the gradient. */
CStatus
CLineBrush_GetColors(CLineBrush *_this,
                     CColor     *startColor,
                     CColor     *endColor)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a start color pointer */
	CStatus_Require((startColor != 0), CStatus_ArgumentNull);

	/* ensure we have an end color pointer */
	CStatus_Require((endColor != 0), CStatus_ArgumentNull);

	/* get the color */
	*startColor = _this->startColor;
	*endColor   = _this->endColor;

	/* return successfully */
	return CStatus_OK;
}

/* Set the start and end colors of the gradient. */
CStatus
CLineBrush_SetColor(CLineBrush *_this,
                    CColor      startColor,
                    CColor      endColor)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the colors */
	_this->startColor = startColor;
	_this->endColor   = endColor;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the color blend of the gradient. */
CStatus
CLineBrush_GetColorBlend(CLineBrush  *_this,
                         CColorBlend *colorBlend)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a color blend pointer */
	CStatus_Require((colorBlend != 0), CStatus_ArgumentNull);

	/* ensure we have a color blend */
	CStatus_Require((_this->colorBlend.count > 0), CStatus_InvalidOperation);

	/* get the color blend */
	return CColorBlend_Copy(&(_this->colorBlend), colorBlend);
}

/* Set the color blend of the gradient. */
CStatus
CLineBrush_SetColorBlend(CLineBrush  *_this,
                         CColorBlend  colorBlend)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have colors */
	CStatus_Require((colorBlend.colors != 0), CStatus_ArgumentNull);

	/* ensure we have positions */
	CStatus_Require((colorBlend.positions != 0), CStatus_ArgumentNull);

	/* ensure the count is in range */
	CStatus_Require((colorBlend.count >= 2), CStatus_ArgumentOutOfRange);

	/* set the color blend */
	{
		/* declarations */
		CColorBlend tmp;

		/* copy the color blend */
		CStatus_Check
			(CColorBlend_Copy
				(&(colorBlend), &(tmp)));

		/* dispose of the current blend as needed */
		if(_this->colorBlend.count != 0)
		{
			CColorBlend_Finalize(&(_this->colorBlend));
		}
		else
		{
			CBlend_Finalize(&(_this->blend));
		}

		/* set the color blend */
		_this->colorBlend = tmp;
	}

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the gamma correction flag of the gradient. */
CStatus
CLineBrush_GetGammaCorrection(CLineBrush *_this,
                              CBool      *gammaCorrection)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a gamma correction flag pointer */
	CStatus_Require((gammaCorrection != 0), CStatus_ArgumentNull);

	/* get the gamma correction flag */
	*gammaCorrection = _this->gammaCorrection;

	/* return successfully */
	return CStatus_OK;
}

/* Set the gamma correction flag of the gradient. */
CStatus
CLineBrush_SetGammaCorrection(CLineBrush *_this,
                              CBool       gammaCorrection)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the gamma correction flag */
	_this->gammaCorrection = gammaCorrection;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the bounding rectangle of the gradient. */
CStatus
CLineBrush_GetRectangle(CLineBrush  *_this,
                        CRectangleF *rectangle)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a bounding rectangle pointer */
	CStatus_Require((rectangle != 0), CStatus_ArgumentNull);

	/* get the bounding rectangle */
	*rectangle = _this->rectangle;

	/* return successfully */
	return CStatus_OK;
}

/* Get the wrap mode of the gradient. */
CStatus
CLineBrush_GetWrapMode(CLineBrush *_this,
                       CWrapMode  *wrapMode)
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

/* Set the wrap mode of the gradient. */
CStatus
CLineBrush_SetWrapMode(CLineBrush *_this,
                       CWrapMode   wrapMode)
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

/* Get the transformation matrix of the gradient. */
CStatus
CLineBrush_GetTransform(CLineBrush *_this,
                        CMatrix    *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the transformation */
	return CMatrix_SetTransform(matrix, &(_this->transform));
}

/* Multiply the transformation matrix of the gradient by another matrix. */
CStatus
CLineBrush_MultiplyTransform(CLineBrush   *_this,
                             CMatrix      *matrix,
                             CMatrixOrder  order)
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

/* Reset the transformation matrix of the gradient. */
CStatus
CLineBrush_ResetTransform(CLineBrush *_this)
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

/* Rotate the transformation matrix of the gradient. */
CStatus
CLineBrush_RotateTransform(CLineBrush   *_this,
                           CFloat        angle,
                           CMatrixOrder  order)
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

/* Scale the transformation matrix of the gradient. */
CStatus
CLineBrush_ScaleTransform(CLineBrush   *_this,
                          CFloat        sx,
                          CFloat        sy,
                          CMatrixOrder  order)
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

/* Set the shape of the gradient to a triangle. */
CStatus
CLineBrush_SetTriangularShape(CLineBrush *_this,
                              CFloat      focus,
                              CFloat      scale)
{
	/* declarations */
	CUInt32 count;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* clamp the focus and scale */
	CCLAMP(focus, 0.0f, 1.0f);
	CCLAMP(scale, 0.0f, 1.0f);

	/* set the count */
	if(focus == 0.0f || focus == 1.0f)
	{
		count = CBlend_TriangularHalfCount;
	}
	else
	{
		count = CBlend_TriangularFullCount;
	}

	/* allocate and dispose, as needed */
	if(_this->blend.count != count)
	{
		/* declarations */
		CBlend tmp;

		/* initialize the blend */
		CBlend_Initialize(&(tmp), count);

		/* dispose of the current blend as needed */
		if(_this->blend.count != 0)
		{
			CBlend_Finalize(&(_this->blend));
		}
		else
		{
			CColorBlend_Finalize(&(_this->colorBlend));
		}

		/* set the blend */
		_this->blend = tmp;
	}

	/* set the blend to the triangular shape */
	CBlend_SetTriangularShape(&(_this->blend), focus, scale);

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Set the shape of the gradient to a sigma bell. */
CStatus
CLineBrush_SetSigmaBellShape(CLineBrush *_this,
                             CFloat      focus,
                             CFloat      scale)
{
	/* declarations */
	CUInt32 count;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* clamp the focus and scale */
	CCLAMP(focus, 0.0f, 1.0f);
	CCLAMP(scale, 0.0f, 1.0f);

	/* set the count */
	if(focus == 0.0f || focus == 1.0f)
	{
		count = CBlend_SigmaBellHalfCount;
	}
	else
	{
		count = CBlend_SigmaBellFullCount;
	}

	/* allocate and dispose, as needed */
	if(_this->blend.count != count)
	{
		/* declarations */
		CBlend tmp;

		/* initialize the blend */
		CBlend_Initialize(&(tmp), count);

		/* dispose of the current blend as needed */
		if(_this->blend.count != 0)
		{
			CBlend_Finalize(&(_this->blend));
		}
		else
		{
			CColorBlend_Finalize(&(_this->colorBlend));
		}

		/* set the blend */
		_this->blend = tmp;
	}

	/* set the blend to the sigma bell shape */
	CBlend_SetSigmaBellShape(&(_this->blend), focus, scale);

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Set the transformation matrix of the gradient. */
CStatus
CLineBrush_SetTransform(CLineBrush *_this,
                        CMatrix    *matrix)
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

/* Translate the transformation matrix of the gradient. */
CStatus
CLineBrush_TranslateTransform(CLineBrush   *_this,
                              CFloat        dx,
                              CFloat        dy,
                              CMatrixOrder  order)
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
