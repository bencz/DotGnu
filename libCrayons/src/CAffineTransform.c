/*
 * CAffineTransform.c - Affine transformation implementation.
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

#include "CAffineTransform.h"
#include "CMath.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Determine if the given transformations are equal. */
CINTERNAL CBool
CAffineTransformF_Equals(const CAffineTransformF *_this,
                         const CAffineTransformF *other)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((other != 0));

	/* determine and return equality */
	return !CMemCmp(_this, other, sizeof(CAffineTransformF));
}

/* Determine if the given transformations are not equal. */
CINTERNAL CBool
CAffineTransformF_NotEquals(const CAffineTransformF *_this,
                            const CAffineTransformF *other)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((other != 0));

	/* determine and return inequality */
	return !(!CMemCmp(_this, other, sizeof(CAffineTransformF)));
}

/* Set this transformation to the identity transformation. */
CINTERNAL void
CAffineTransformF_SetIdentity(CAffineTransformF *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* set the transformation to the identity transformation */
	(*_this) = CAffineTransformF_Identity;
}

/* Set this transformation to warp to a parallelogram. */
CINTERNAL CStatus
CAffineTransformF_SetParallelogram(CAffineTransformF *_this,
                                   CRectangleF        rect,
                                   CPointF            tl,
                                   CPointF            tr,
                                   CPointF            bl)
{
	/* assertions */
	CASSERT((_this != 0));

	/* ensure we have a non-singular rectangle matrix */
	CStatus_Require
		(((CRectangle_Width(rect)  != 0.0f) &&
		  (CRectangle_Height(rect) != 0.0f)),
		 CStatus_InvalidOperation_SingularMatrix);

	/* set the transformation to the parallelogram warp transformation */
	{
		/* calculate values used multiple times */
		const CDouble scaleX = (1.0 / CRectangle_Width(rect));
		const CDouble scaleY = (1.0 / CRectangle_Height(rect));
		const CDouble transX = -CRectangle_X(rect);
		const CDouble transY = -CRectangle_Y(rect);

		/* set the transformation elements */
		_this->m11 = ((CPoint_X(tr) - CPoint_X(tl)) * scaleX);
		_this->m12 = ((CPoint_Y(tr) - CPoint_Y(tl)) * scaleX);
		_this->m21 = ((CPoint_X(bl) - CPoint_X(tl)) * scaleY);
		_this->m22 = ((CPoint_Y(bl) - CPoint_Y(tl)) * scaleY);
		_this->dx  =
			(CPoint_X(tl) + (_this->m11 * transX) + (_this->m21 * transY));
		_this->dy  =
			(CPoint_Y(tl) + (_this->m12 * transX) + (_this->m22 * transY));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the elements of this transformation. */
CINTERNAL void
CAffineTransformF_SetElements(CAffineTransformF *_this,
                              CFloat             m11,
                              CFloat             m12,
                              CFloat             m21,
                              CFloat             m22,
                              CFloat             dx,
                              CFloat             dy)
{
	/* assertions */
	CASSERT((_this != 0));

	/* set the elements of the transformation */
	_this->m11 = m11;
	_this->m12 = m12;
	_this->m21 = m21;
	_this->m22 = m22;
	_this->dx  = dx;
	_this->dy  = dy;
}

/* Get the determinant of this transformation. */
CINTERNAL CFloat
CAffineTransformF_GetDeterminant(const CAffineTransformF *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* calculate and return the determinant */
	return ((_this->m11 * _this->m22) - (_this->m12 * _this->m21));
}

/* Get the inverse of this transformation. */
CINTERNAL CStatus
CAffineTransformF_GetInverse(const CAffineTransformF *_this,
                             CAffineTransformF       *inverse)
{
	/* declarations */
	CFloat determinant;

	/* assertions */
	CASSERT((_this != 0));

	/* get the determinant */
	determinant = CAffineTransformF_GetDeterminant(_this);

	/* ensure the transformation is invertible */
	CStatus_Require
		((determinant != 0.0f), CStatus_InvalidOperation_SingularMatrix);

	/* perform the division once */
	determinant = (1.0 / determinant);

	/* set the inverse transformation */
	inverse->m11 = ( _this->m22 * determinant);
	inverse->m12 = (-_this->m12 * determinant);
	inverse->m21 = (-_this->m21 * determinant);
	inverse->m22 = ( _this->m11 * determinant);
	inverse->dx  =
		(((_this->dy * _this->m21) - (_this->dx * _this->m22)) * determinant);
	inverse->dy  =
		(((_this->dx * _this->m12) - (_this->dy * _this->m11)) * determinant);

	/* return successfully */
	return CStatus_OK;
}

/* Multiply this transformation with another. */
CINTERNAL void
CAffineTransformF_Multiply(CAffineTransformF       *_this,
                           const CAffineTransformF *other,
                           CMatrixOrder             order)
{
	/* declarations */
	CAffineTransformF t1;
	CAffineTransformF t2;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((other != 0));

	/* set up transformations based on order */
	if(order == CMatrixOrder_Prepend)
	{
		t1 = *other;
		t2 = *_this;
	}
	else
	{
		t1 = *_this;
		t2 = *other;
	}

	/* multiply transformations */
	_this->m11 = ((t1.m11 * t2.m11) + (t1.m12 * t2.m21));
	_this->m12 = ((t1.m12 * t2.m22) + (t1.m11 * t2.m12));
	_this->m21 = ((t1.m21 * t2.m11) + (t1.m22 * t2.m21));
	_this->m22 = ((t1.m22 * t2.m22) + (t1.m21 * t2.m12));
	_this->dx  = ((t1.dx  * t2.m11) + (t1.dy  * t2.m21) + (t2.dx));
	_this->dy  = ((t1.dy  * t2.m22) + (t1.dx  * t2.m12) + (t2.dy));
}

/* Inverse multiply this transformation with another. */
CINTERNAL CStatus
CAffineTransformF_MultiplyInverse(CAffineTransformF       *_this,
                                  const CAffineTransformF *other,
                                  CMatrixOrder             order)
{
	/* declarations */
	CAffineTransformF t1;
	CAffineTransformF t2;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((other != 0));

	/* set up transformations based on order */
	if(order == CMatrixOrder_Prepend)
	{
		t1 = *_this;
		CStatus_Check
			(CAffineTransformF_GetInverse
				(other, &t2));
	}
	else
	{
		CStatus_Check
			(CAffineTransformF_GetInverse
				(other, &t1));
		t2 = *_this;
	}

	/* multiply transformations */
	_this->m11 = ((t1.m11 * t2.m11) + (t1.m12 * t2.m21));
	_this->m12 = ((t1.m12 * t2.m22) + (t1.m11 * t2.m12));
	_this->m21 = ((t1.m21 * t2.m11) + (t1.m22 * t2.m21));
	_this->m22 = ((t1.m22 * t2.m22) + (t1.m21 * t2.m12));
	_this->dx  = ((t1.dx  * t2.m11) + (t1.dy  * t2.m21) + (t2.dx));
	_this->dy  = ((t1.dy  * t2.m22) + (t1.dx  * t2.m12) + (t2.dy));

	/* return successfully */
	return CStatus_OK;
}

/* Rotate a transformation. */
CINTERNAL void
CAffineTransformF_Rotate(CAffineTransformF *_this,
                         CFloat             angle,
                         CMatrixOrder       order)
{
	/* declarations */
	CAffineTransformF rotate;
	CDouble           radians;
	CFloat            sin;
	CFloat            cos;

	/* assertions */
	CASSERT((_this != 0));

	/* calculate the radians */
	radians = CMath_ToRadians(angle);

	/* calculate the sine */
	sin = (CFloat)CMath_Sin(radians);

	/* calculate the cosine */
	cos = (CFloat)CMath_Cos(radians);

	/* create the rotation transformation */
	CAffineTransformF_SetElements(&rotate, cos, sin, -sin, cos, 0, 0);

	/* rotate the transformation */
	CAffineTransformF_Multiply(_this, &rotate, order);
}

/* Inverse rotate a transformation. */
CINTERNAL void
CAffineTransformF_RotateInverse(CAffineTransformF *_this,
                                CFloat             angle,
                                CMatrixOrder       order)
{
	/* declarations */
	CAffineTransformF rotate;
	CDouble           radians;
	CFloat            sin;
	CFloat            cos;

	/* assertions */
	CASSERT((_this != 0));

	/* calculate the radians */
	radians = CMath_ToRadians(-angle);

	/* calculate the sine */
	sin = (CFloat)CMath_Sin(radians);

	/* calculate the cosine */
	cos = (CFloat)CMath_Cos(radians);

	/* create the rotation transformation */
	CAffineTransformF_SetElements(&rotate, cos, sin, -sin, cos, 0, 0);

	/* invert the order */
	if(order == CMatrixOrder_Prepend)
	{
		order = CMatrixOrder_Append;
	}
	else
	{
		order = CMatrixOrder_Prepend;
	}

	/* rotate the transformation */
	CAffineTransformF_Multiply(_this, &rotate, order);
}

/* Scale a transformation. */
CINTERNAL void
CAffineTransformF_Scale(CAffineTransformF *_this,
                        CFloat             scaleX,
                        CFloat             scaleY,
                        CMatrixOrder       order)
{
	/*\
	|*| NOTE: technically we could just multiply with a
	|*|       CAffineTransformF(scaleX, 0, 0, scaleY, 0, 0),
	|*|       but this is more efficient
	\*/

	/* assertions */
	CASSERT((_this != 0));

	/* scale the transformation */
	if(order == CMatrixOrder_Prepend)
	{
		_this->m11 *= scaleX;
		_this->m12 *= scaleX;
		_this->m21 *= scaleY;
		_this->m22 *= scaleY;
	}
	else
	{
		_this->m11 *= scaleX;
		_this->m12 *= scaleY;
		_this->m21 *= scaleX;
		_this->m22 *= scaleY;
		_this->dx  *= scaleX;
		_this->dy  *= scaleY;
	}
}

/* Inverse scale a transformation. */
CINTERNAL void
CAffineTransformF_ScaleInverse(CAffineTransformF *_this,
                               CFloat             scaleX,
                               CFloat             scaleY,
                               CMatrixOrder       order)
{
	/* declarations */
	CDouble sx;
	CDouble sy;

	/* assertions */
	CASSERT((_this != 0));

	/* calculate the inverse scale factors */
	sx = (1.0f / (CDouble)scaleX);
	sy = (1.0f / (CDouble)scaleY);

	/* scale the transformation */
	if(order == CMatrixOrder_Prepend)
	{
		_this->m11 *= sx;
		_this->m12 *= sy;
		_this->m21 *= sx;
		_this->m22 *= sy;
		_this->dx  *= sx;
		_this->dy  *= sy;
	}
	else
	{
		_this->m11 *= sx;
		_this->m12 *= sx;
		_this->m21 *= sy;
		_this->m22 *= sy;
	}
}

/* Extract the scaling factors from, then unscale, this transformation. */
CINTERNAL void
CAffineTransformF_ExtractScale(CAffineTransformF *_this,
                               CFloat            *scaleX,
                               CFloat            *scaleY,
                               CMatrixOrder       order)
{
	/* declarations */
	CFloat m11;
	CFloat m12;
	CFloat m21;
	CFloat m22;
	CFloat determinant;
	CFloat shear;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((scaleX != 0));
	CASSERT((scaleY != 0));

	/* get the transformation components */
	m11 = _this->m11;
	m12 = _this->m12;
	m21 = _this->m21;
	m22 = _this->m22;

	/* get the determinant */
	determinant = CAffineTransformF_GetDeterminant(_this);

	/* calculate the horizontal scaling factor */
	*scaleX = CMath_Sqrt((m11 * m11) + (m12 * m12));

	/* unscale the first row */
	m11 *= (1.0f / *scaleX);
	m12 *= (1.0f / *scaleX);

	/* calculate the shear adjustment ((shearX / shearY) * scaleY) */
	shear = ((m11 * m21) + (m12 * m22));

	/* compensate for shear */
	m21 -= (shear * m11);
	m22 -= (shear * m12);

	/* calculate the vertical scaling factor */
	*scaleY = CMath_Sqrt((m21 * m21) + (m22 * m22));

	/* handle reflection, as needed */
	if(determinant < 0.0f)
	{
		*scaleX *= -1;
		*scaleY *= -1;
	}

	/* inverse scale the transformation */
	if(order == CMatrixOrder_Prepend)
	{
		CAffineTransformF_ScaleInverse
			(_this, *scaleX, *scaleY, CMatrixOrder_Append);
	}
	else
	{
		CAffineTransformF_ScaleInverse
			(_this, *scaleX, *scaleY, CMatrixOrder_Prepend);
	}
}

/* Shear a transformation. */
CINTERNAL void
CAffineTransformF_Shear(CAffineTransformF *_this,
                        CFloat             shearX,
                        CFloat             shearY,
                        CMatrixOrder       order)
{
	/*\
	|*| NOTE: technically we could just multiply with a
	|*|       CAffineTransformF(1, shearX, shearY, 1, 0, 0),
	|*|       but this is more efficient
	\*/

	/* assertions */
	CASSERT((_this != 0));

	/* shear the transformation */
	if(order == CMatrixOrder_Prepend)
	{
		/* get the first row of the transformation */
		const CFloat m11 = _this->m11;
		const CFloat m12 = _this->m12;

		/* perform the shear */
		_this->m11 += (_this->m21 * shearY);
		_this->m12 += (_this->m22 * shearY);
		_this->m21 += (       m11 * shearX);
		_this->m22 += (       m12 * shearX);
	}
	else
	{
		/* get the first column of the transformation */
		const CFloat m11 = _this->m11;
		const CFloat m21 = _this->m21;
		const CFloat dx  = _this->dx;

		/* perform the shear */
		_this->m11 += (_this->m12 * shearY);
		_this->m12 += (       m11 * shearX);
		_this->m21 += (_this->m22 * shearY);
		_this->m22 += (       m21 * shearX);
		_this->dx  += (_this->dy  * shearY);
		_this->dy  += (       dx  * shearX);
	}
}

/* Translate a transformation. */
CINTERNAL void
CAffineTransformF_Translate(CAffineTransformF *_this,
                            CFloat             offsetX,
                            CFloat             offsetY,
                            CMatrixOrder       order)
{
	/*\
	|*| NOTE: technically we could just multiply with a
	|*|       CAffineTransformF(1, 0, 0, 1, offsetX, offsetY),
	|*|       but this is more efficient
	\*/

	/* assertions */
	CASSERT((_this != 0));

	/* translate the matrix */
	if(order == CMatrixOrder_Prepend)
	{
		_this->dx += (_this->m11 * offsetX) + (_this->m21 * offsetY);
		_this->dy += (_this->m12 * offsetX) + (_this->m22 * offsetY);
	}
	else
	{
		_this->dx += offsetX;
		_this->dy += offsetY;
	}
}

/* Inverse translate a transformation. */
CINTERNAL void
CAffineTransformF_TranslateInverse(CAffineTransformF *_this,
                                   CFloat             offsetX,
                                   CFloat             offsetY,
                                   CMatrixOrder       order)
{
	/* assertions */
	CASSERT((_this != 0));

	/* inverse translate the matrix */
	if(order == CMatrixOrder_Prepend)
	{
		_this->dx -= offsetX;
		_this->dy -= offsetY;
	}
	else
	{
		_this->dx -= (_this->m11 * offsetX) + (_this->m21 * offsetY);
		_this->dy -= (_this->m12 * offsetX) + (_this->m22 * offsetY);
	}
}

/* Transform a list of points. */
CINTERNAL void
CAffineTransformF_TransformPoints(const CAffineTransformF *_this,
                                  CPointF                 *points,
                                  CUInt32                  count)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));

	/* transform the points */
	{
		/* get the transformation */
		const CAffineTransformF t = *_this;

		/* get the end pointer */
		const CPointF *const end = (points + count);

		/* transform the points */
		while(points != end)
		{
			/* get the x coordinate */
			const CFloat x = CPoint_X(*points);

			/* get the y coordinate */
			const CFloat y = CPoint_Y(*points);

			/* transform the point */
			CPoint_X(*points) = ((x * t.m11) + (y * t.m21) + t.dx);
			CPoint_Y(*points) = ((x * t.m12) + (y * t.m22) + t.dy);

			/* update the points pointer */
			++points;
		}
	}
}

/* Transform a list of vectors. */
CINTERNAL void
CAffineTransformF_TransformVectors(const CAffineTransformF *_this,
                                   CPointF                 *points,
                                   CUInt32                  count)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));

	/* transform the vectors */
	{
		/* get the transformation */
		const CAffineTransformF t = *_this;

		/* get the end pointer */
		const CPointF *const end = (points + count);

		/* transform the vectors */
		while(points != end)
		{
			/* get the horizontal weight */
			const CFloat x = CPoint_X(*points);

			/* get the vertical weight */
			const CFloat y = CPoint_Y(*points);

			/* transform the vector */
			CPoint_X(*points) = ((x * t.m11) + (y * t.m21));
			CPoint_Y(*points) = ((x * t.m12) + (y * t.m22));

			/* update the vector pointer */
			++points;
		}
	}
}

CINTERNAL void
CVectorF_ScalePoints(const CVectorF *_this,
                     CPointF        *points,
                     CUInt32         count)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));

	/* scale the points */
	{
		/* get the scaling factors */
		const CFloat scaleX = CVector_X(*_this);
		const CFloat scaleY = CVector_Y(*_this);

		/* get the end pointer */
		const CPointF *const end = (points + count);

		/* scale the points */
		while(points != end)
		{
			/* scale the point */
			CPoint_X(*points) *= scaleX;
			CPoint_Y(*points) *= scaleY;

			/* update the points pointer */
			++points;
		}
	}
}

CINTERNAL void
CVectorF_TranslatePoints(const CVectorF *_this,
                         CPointF        *points,
                         CUInt32         count)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));

	/* translate the points */
	{
		/* get the translation factors */
		const CFloat transX = CVector_X(*_this);
		const CFloat transY = CVector_Y(*_this);

		/* get the end pointer */
		const CPointF *const end = (points + count);

		/* translate the points */
		while(points != end)
		{
			/* translate the point */
			CPoint_X(*points) += transX;
			CPoint_Y(*points) += transY;

			/* update the points pointer */
			++points;
		}
	}
}


#ifdef __cplusplus
};
#endif
