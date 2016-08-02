/*
 * CPathBrush.c - Path gradient brush implementation.
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

#include "CPathBrush.h"
#include "CBlend.h"
#include "CMatrix.h"
#include "CPath.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* TODO: implement more than a glorified solid brush */

static const CPointF CPointF_Zero = { 0.0f, 0.0f };

/* Initialize this path brush. */
static CStatus
CPathBrush_Initialize(CPathBrush *_this,
                      CPath      *path)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	{
		/* initialize the path */
		CStatus_Check
			(CPath_Clone
				(path, &(_this->path)));

		/* find the center point and bounds */
		{
			/* declarations */
			CFloat   maxX,  maxY;
			CFloat   minX,  minY;
			CFloat   sumX,  sumY;
			CPointF *curr, *end;

			/*\
			|*| NOTE: directly accessing the path internals isn't exactly a
			|*|       clean way to do this, but it avoids making yet another
			|*|       copy of the data... this should probably be made into
			|*|       an internal access path method... this also only
			|*|       takes into account the polygonal bounds of curves,
			|*|       which may not be what we need
			\*/

			/* get the point pointer */
			curr = _this->path->points;

			/* get the end pointer */
			end = (curr + _this->path->count);

			/* ensure we have at least one point (for now) */
			if(curr == end)
			{
				CPath_Destroy(&(_this->path));
				return CStatus_Argument;
			}

			/* initialize the bounds and sums */
			sumX = maxX = minX = CPoint_X(*curr);
			sumY = maxY = minY = CPoint_Y(*curr);

			/* move to the next point position */
			++curr;

			/* calculate the bounds and sums */
			while(curr != end)
			{
				/* declarations */
				CFloat x, y;

				/* get the current coordinates */
				x = CPoint_X(*curr);
				y = CPoint_Y(*curr);

				/* update the sums */
				sumX += x;
				sumY += y;

				/* update the bounds */
				maxX = ((x > maxX) ? x : maxX);
				maxY = ((y > maxY) ? y : maxY);
				minX = ((x < minX) ? x : minX);
				minY = ((y < minY) ? y : minY);
			}

			/* initialize the center point */
			CPoint_X(_this->centerPoint) = (sumX / _this->path->count);
			CPoint_Y(_this->centerPoint) = (sumY / _this->path->count);

			/* initialize the bounding rectangle */
			CRectangle_X(_this->rectangle)      = (minX);
			CRectangle_Y(_this->rectangle)      = (minY);
			CRectangle_Width(_this->rectangle)  = (maxX - minX);
			CRectangle_Height(_this->rectangle) = (maxY - minY);
		}

		/* allocate the surrounding colors */
		if(!(_this->surroundColors = (CColor *)CMalloc(sizeof(CColor))))
		{
			CPath_Destroy(&(_this->path));
			return CStatus_OutOfMemory;
		}

		/* initialize the remaining members */
		_this->transform         = CAffineTransformF_Identity;
		_this->wrapMode          = CWrapMode_Clamp;
		_this->blend             = CBlend_Zero;
		_this->colorBlend        = CColorBlend_Zero;
		_this->focusPoint        = CPointF_Zero;
		_this->centerColor       = CColor_White;
		_this->surroundColors[0] = CColor_White;
		_this->surroundCount     = 1;
	}

	/* initialize the base */
	CBrush_Initialize((CBrush *)_this, &CPathBrush_Class);

	/* return successfully */
	return CStatus_OK;
}

/* Finalize this path brush. */
static void
CPathBrush_Finalize(CBrush *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize this brush */
	{
		/* declarations */
		CPathBrush *brush;

		/* get this as a path brush */
		brush = (CPathBrush *)_this;

		/* finalize the blend */
		CBlend_Finalize(&(brush->blend));

		/* finalize the color blend */
		CColorBlend_Finalize(&(brush->colorBlend));

		/* finalize the surrounding colors, as needed */
		if(brush->surroundColors != 0)
		{
			CFree(brush->surroundColors);
			brush->surroundColors = 0;
			brush->surroundCount  = 0;
		}

		/* finalize the path, as needed */
		if(brush->path != 0)
		{
			CPath_Destroy(&(brush->path));
		}
	}
}

/* Clone this path brush. */
static CStatus
CPathBrush_Clone(CBrush  *_this,
                 CBrush **_clone)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((_clone != 0));

	/* clone this brush */
	{
		/* declarations */
		CPathBrush *brush;
		CPathBrush *clone;
		CStatus     status;

		/* get this as a path brush */
		brush = (CPathBrush *)_this;

		/* create the clone */
		CStatus_Check
			(CPathBrush_Create
				(((CPathBrush **)_clone), brush->path));

		/* get the clone brush */
		clone = ((CPathBrush *)(*_clone));

		/* copy the blend */
		status = CBlend_Copy(&(brush->blend), &(clone->blend));

		/* handle blend copy failures */
		if(status != CStatus_OK)
		{
			CPathBrush_Finalize(*_clone);
			CFree(*_clone);
			*_clone = 0;
			return status;
		}

		/* copy the color blend */
		status = CColorBlend_Copy(&(brush->colorBlend), &(clone->colorBlend));

		/* handle color blend copy failures */
		if(status != CStatus_OK)
		{
			CPathBrush_Finalize(*_clone);
			CFree(*_clone);
			*_clone = 0;
			return status;
		}

		/* copy the surrounding colors */
		status =
			CPathBrush_SetSurroundColors
				(clone, brush->surroundColors, brush->surroundCount);

		/* handle surrounding colors copy failures */
		if(status != CStatus_OK)
		{
			CPathBrush_Finalize(*_clone);
			CFree(*_clone);
			*_clone = 0;
			return status;
		}

		/* initialize the remaining clone members */
		clone->transform   = brush->transform;
		clone->wrapMode    = brush->wrapMode;
		clone->focusPoint  = brush->focusPoint;
		clone->centerColor = brush->centerColor;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Create a pattern for this brush. */
static CStatus
CPathBrush_CreatePattern(CBrush   *_this,
                         CPattern *pattern)
{
	/* declarations */
	CPathBrush *brush;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* TODO: implement this the right way */

	/* get this as a path brush */
	brush = (CPathBrush *)_this;

	/* set the pattern transformation */
	pattern->transform = 0;

	/* create the pattern */
	return CUtils_CreateSolidPattern(&(pattern->image), brush->centerColor);
}

/* Create a path brush. */
CStatus
CPathBrush_Create(CPathBrush **_this,
                  CPath       *path)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a path */
	CStatus_Require((path != 0), CStatus_ArgumentNull);

	/* allocate the brush */
	if(!(*_this = (CPathBrush *)CMalloc(sizeof(CPathBrush))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the brush */
	{
		/* declarations */
		CStatus status;

		/* initialize the brush */
		if((status = CPathBrush_Initialize(*_this, path)) != CStatus_OK)
		{
			CFree(*_this);
			*_this = 0;
			return status;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the gradient blend. */
CStatus
CPathBrush_GetBlend(CPathBrush *_this,
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
CPathBrush_SetBlend(CPathBrush *_this,
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

/* Get the center color of the gradient. */
CStatus
CPathBrush_GetCenterColor(CPathBrush *_this,
                          CColor     *centerColor)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a center color pointer */
	CStatus_Require((centerColor != 0), CStatus_ArgumentNull);

	/* get the center color */
	*centerColor = _this->centerColor;

	/* return successfully */
	return CStatus_OK;
}

/* Set the center color of the gradient. */
CStatus
CPathBrush_SetCenterColor(CPathBrush *_this,
                          CColor      centerColor)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the center color */
	_this->centerColor = centerColor;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the center point of the gradient. */
CStatus
CPathBrush_GetCenterPoint(CPathBrush *_this,
                          CPointF    *centerPoint)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a center point pointer */
	CStatus_Require((centerPoint != 0), CStatus_ArgumentNull);

	/* get the center point */
	*centerPoint = _this->centerPoint;

	/* return successfully */
	return CStatus_OK;
}

/* Set the center point of the gradient. */
CStatus
CPathBrush_SetCenterPoint(CPathBrush *_this,
                          CPointF     centerPoint)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the center point */
	_this->centerPoint = centerPoint;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the color blend of the gradient. */
CStatus
CPathBrush_GetColorBlend(CPathBrush  *_this,
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
CPathBrush_SetColorBlend(CPathBrush  *_this,
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

/* Get the focus point of the gradient. */
CStatus
CPathBrush_GetFocusPoint(CPathBrush *_this,
                         CPointF    *focusPoint)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a focus point pointer */
	CStatus_Require((focusPoint != 0), CStatus_ArgumentNull);

	/* get the focus point */
	*focusPoint = _this->focusPoint;

	/* return successfully */
	return CStatus_OK;
}

/* Set the focus point of the gradient. */
CStatus
CPathBrush_SetFocusPoint(CPathBrush *_this,
                         CPointF     focusPoint)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the focus point */
	_this->focusPoint = focusPoint;

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the bounding rectangle of the gradient. */
CStatus
CPathBrush_GetRectangle(CPathBrush  *_this,
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

/* Get the surrounding colors of the gradient. */
CStatus
CPathBrush_GetSurroundColors(CPathBrush  *_this,
                             CColor     **surroundColors,
                             CUInt32     *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a surrounding colors pointer pointer */
	CStatus_Require((surroundColors != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the surrounding colors */
	{
		/* get the count */
		*count = _this->surroundCount;

		/* allocate the surrounding colors */
		*surroundColors = (CColor *)CMalloc(*count * sizeof(CColor));

		/* ensure we have surrounding colors */
		CStatus_Require((*surroundColors != 0), CStatus_OutOfMemory);

		/* get the surrounding colors */
		CMemCopy(*surroundColors, _this->surroundColors, *count);
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the colors of the gradient path points. */
CStatus
CPathBrush_SetSurroundColors(CPathBrush *_this,
                             CColor     *surroundColors,
                             CUInt32     count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a surrounding colors pointer */
	CStatus_Require((surroundColors != 0), CStatus_ArgumentNull);

	/* set the surrounding colors */
	{
		/* declarations */
		CUInt32  pc;
		CColor  *tmp;

		/* get the path count */
		CStatus_Check
			(CPath_GetCount
				(_this->path, &pc));

		/* ensure the count is in range */
		CStatus_Require
			((count != 0 && count < pc), CStatus_ArgumentOutOfRange);

		/* allocate the surrounding colors */
		tmp = (CColor *)CMalloc(count * sizeof(CColor));

		/* ensure we have surrounding colors */
		CStatus_Require((tmp != 0), CStatus_OutOfMemory);

		/* copy the surrounding colors */
		CMemCopy(tmp, surroundColors, count);

		/* dispose of the current surrounding colors, as needed */
		if(_this->surroundCount != 0)
		{
			CFree(_this->surroundColors);
		}

		/* set the surrounding colors */
		_this->surroundColors = tmp;

		/* set the surrounding colors count */
		_this->surroundCount = count;
	}

	/* send change signal to base */
	CBrush_OnChange((CBrush *)_this);

	/* return successfully */
	return CStatus_OK;
}

/* Get the wrap mode of the gradient. */
CStatus
CPathBrush_GetWrapMode(CPathBrush *_this,
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
CPathBrush_SetWrapMode(CPathBrush *_this,
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
CPathBrush_GetTransform(CPathBrush *_this,
                        CMatrix    *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the transformation */
	return CMatrix_SetTransform(matrix, &(_this->transform));
}

/* Multiply the transformation matrix of the gradient by another matrix. */
CStatus
CPathBrush_MultiplyTransform(CPathBrush   *_this,
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
CPathBrush_ResetTransform(CPathBrush *_this)
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
CPathBrush_RotateTransform(CPathBrush   *_this,
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
CPathBrush_ScaleTransform(CPathBrush   *_this,
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
CPathBrush_SetTriangularShape(CPathBrush *_this,
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
CPathBrush_SetSigmaBellShape(CPathBrush *_this,
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
CPathBrush_SetTransform(CPathBrush *_this,
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
CPathBrush_TranslateTransform(CPathBrush   *_this,
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
