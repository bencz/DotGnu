/*
 * CMatrix.c - Matrix implementation.
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

#include "CMatrix.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Create an identity matrix. */
CStatus
CMatrix_Create(CMatrix **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the matrix */
	if(!(*_this = (CMatrix *)CMalloc(sizeof(CMatrix))))
	{
		return CStatus_OutOfMemory;
	}

	/* set the transformation to the identity transformation */
	(*_this)->transform = CAffineTransformF_Identity;

	/* return successfully */
	return CStatus_OK;
}

/* Create a parallelogram warp matrix. */
CStatus
CMatrix_CreateParallelogram(CMatrix     **_this,
                            CRectangleF   rect,
                            CPointF       tl,
                            CPointF       tr,
                            CPointF       bl)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the matrix */
	if(!(*_this = (CMatrix *)CMalloc(sizeof(CMatrix))))
	{
		return CStatus_OutOfMemory;
	}

	/* set the transformation to the parallelogram warp transformation */
	status =
		CAffineTransformF_SetParallelogram
			(&((*_this)->transform), rect, tl, tr, bl);

	/* handle status */
	if(status != CStatus_OK)
	{
		CFree(*_this);
		*_this = 0;
	}

	/* return status */
	return status;
}

/* Create a matrix with the given elements. */
CStatus
CMatrix_CreateElements(CMatrix **_this,
                       CFloat    m11,
                       CFloat    m12,
                       CFloat    m21,
                       CFloat    m22,
                       CFloat    dx,
                       CFloat    dy)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the matrix */
	if(!(*_this = (CMatrix *)CMalloc(sizeof(CMatrix))))
	{
		return CStatus_OutOfMemory;
	}

	/* set the elements of the transformation */
	CAffineTransformF_SetElements
		(&((*_this)->transform), m11, m12, m21, m22, dx, dy);

	/* return successfully */
	return CStatus_OK;
}

/* Destroy this matrix. */
CStatus
CMatrix_Destroy(CMatrix **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require((*_this != 0), CStatus_ArgumentNull);

	/* dispose of the matrix */
	CFree(*_this);

	/* null the this pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Get the determinant of this matrix. */
CStatus
CMatrix_GetDeterminant(CMatrix *_this,
                       CFloat  *determinant)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a determinant pointer */
	CStatus_Require((determinant != 0), CStatus_ArgumentNull);

	/* get the determinant */
	*determinant = CAffineTransformF_GetDeterminant(&(_this->transform));

	/* return successfully */
	return CStatus_OK;
}

/* Get the inverse of this matrix. */
CStatus
CMatrix_GetInverse(CMatrix *_this,
                   CMatrix *inverse)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an inverse pointer */
	CStatus_Require((inverse != 0), CStatus_ArgumentNull);

	/* get the inverse */
	return
		CAffineTransformF_GetInverse
			(&(_this->transform), &(inverse->transform));
}

/* Get the transformation of this matrix. */
CINTERNAL CStatus
CMatrix_GetTransform(CMatrix           *_this,
                     CAffineTransformF *transform)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a transformation pointer */
	CStatus_Require((transform != 0), CStatus_ArgumentNull);

	/* get the transformation */
	*transform = _this->transform;

	/* return successfully */
	return CStatus_OK;
}

/* Set the transformation of this matrix. */
CINTERNAL CStatus
CMatrix_SetTransform(CMatrix           *_this,
                     CAffineTransformF *transform)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a transformation pointer */
	CStatus_Require((transform != 0), CStatus_ArgumentNull);

	/* set the transformation */
	_this->transform = *transform;

	/* return successfully */
	return CStatus_OK;
}

/* Multiply this matrix with another. */
CStatus
CMatrix_Multiply(CMatrix      *_this,
                 CMatrix      *other,
                 CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an other pointer */
	CStatus_Require((other != 0), CStatus_ArgumentNull);

	/* perform the multiplication */
	CAffineTransformF_Multiply
		(&(_this->transform), &(other->transform), order);

	/* return successfully */
	return CStatus_OK;
}

/* Determine if the given matrices are equal. */
CStatus
CMatrix_Equals(CMatrix *_this,
               CMatrix *other,
               CBool   *eq)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an other pointer */
	CStatus_Require((other != 0), CStatus_ArgumentNull);

	/* ensure we have an equality pointer */
	CStatus_Require((eq != 0), CStatus_ArgumentNull);

	/* determine equality */
	*eq = CAffineTransformF_Equals(&(_this->transform), &(other->transform));

	/* return successfully */
	return CStatus_OK;
}

/* Determine if the given matrices are not equal. */
CStatus
CMatrix_NotEquals(CMatrix *_this,
                  CMatrix *other,
                  CBool   *ne)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an other pointer */
	CStatus_Require((other != 0), CStatus_ArgumentNull);

	/* ensure we have an inequality pointer */
	CStatus_Require((ne != 0), CStatus_ArgumentNull);

	/* determine equality */
	*ne = CAffineTransformF_NotEquals(&(_this->transform), &(other->transform));

	/* return successfully */
	return CStatus_OK;
}

/* Rotate this matrix. */
CStatus
CMatrix_Rotate(CMatrix      *_this,
               CFloat        angle,
               CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* perform the rotation */
	CAffineTransformF_Rotate(&(_this->transform), angle, order);

	/* return successfully */
	return CStatus_OK;
}

/* Scale this matrix. */
CStatus
CMatrix_Scale(CMatrix      *_this,
              CFloat        scaleX,
              CFloat        scaleY,
              CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* perform the scale */
	CAffineTransformF_Scale(&(_this->transform), scaleX, scaleY, order);

	/* return successfully */
	return CStatus_OK;
}

/* Shear this matrix. */
CStatus
CMatrix_Shear(CMatrix      *_this,
              CFloat        shearX,
              CFloat        shearY,
              CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* perform the shear */
	CAffineTransformF_Shear(&(_this->transform), shearX, shearY, order);

	/* return successfully */
	return CStatus_OK;
}

/* Translate this matrix. */
CStatus
CMatrix_Translate(CMatrix      *_this,
                  CFloat        offsetX,
                  CFloat        offsetY,
                  CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* perform the translation */
	CAffineTransformF_Translate(&(_this->transform), offsetX, offsetY, order);

	/* return successfully */
	return CStatus_OK;
}

/* Transform a list of points. */
CStatus
CMatrix_TransformPoints(CMatrix *_this,
                        CPointF *points,
                        CUInt32  count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a points pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the point transformations */
	CAffineTransformF_TransformPoints(&(_this->transform), points, count);

	/* return successfully */
	return CStatus_OK;
}

/* Transform a list of vectors. */
CStatus
CMatrix_TransformVectors(CMatrix *_this,
                         CPointF *points,
                         CUInt32  count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a points pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* perform the vector transformations */
	CAffineTransformF_TransformVectors(&(_this->transform), points, count);

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
