/*
 * CAffineTransform.h - Affine transformation header.
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

#ifndef _C_AFFINETRANFSORM_H_
#define _C_AFFINETRANFSORM_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CAffineTransformF CAffineTransformF_Identity =
{
	1.0f, 0.0f,
	0.0f, 1.0f,
	0.0f, 0.0f
};
static const CVectorF CVectorF_Zero = { 0.0f, 0.0f };


CINTERNAL CBool
CAffineTransformF_Equals(const CAffineTransformF *_this,
                         const CAffineTransformF *other);
CINTERNAL CBool
CAffineTransformF_NotEquals(const CAffineTransformF *_this,
                            const CAffineTransformF *other);
CINTERNAL void
CAffineTransformF_SetIdentity(CAffineTransformF *_this);
CINTERNAL CStatus
CAffineTransformF_SetParallelogram(CAffineTransformF *_this,
                                   CRectangleF        rect,
                                   CPointF            tl,
                                   CPointF            tr,
                                   CPointF            bl);
CINTERNAL void
CAffineTransformF_SetElements(CAffineTransformF *_this,
                              CFloat             m11,
                              CFloat             m12,
                              CFloat             m21,
                              CFloat             m22,
                              CFloat             dx,
                              CFloat             dy);
CINTERNAL CFloat
CAffineTransformF_GetDeterminant(const CAffineTransformF *_this);
CINTERNAL CStatus
CAffineTransformF_GetInverse(const CAffineTransformF *_this,
                             CAffineTransformF       *inverse);
CINTERNAL void
CAffineTransformF_Multiply(CAffineTransformF       *_this,
                           const CAffineTransformF *other,
                           CMatrixOrder             order);
CINTERNAL CStatus
CAffineTransformF_MultiplyInverse(CAffineTransformF       *_this,
                                  const CAffineTransformF *other,
                                  CMatrixOrder             order);
CINTERNAL void
CAffineTransformF_Rotate(CAffineTransformF *_this,
                         CFloat             angle,
                         CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_RotateInverse(CAffineTransformF *_this,
                                CFloat             angle,
                                CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_Scale(CAffineTransformF *_this,
                        CFloat             scaleX,
                        CFloat             scaleY,
                        CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_ScaleInverse(CAffineTransformF *_this,
                               CFloat             scaleX,
                               CFloat             scaleY,
                               CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_ExtractScale(CAffineTransformF *_this,
                               CFloat            *scaleX,
                               CFloat            *scaleY,
                               CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_Shear(CAffineTransformF *_this,
                        CFloat             shearX,
                        CFloat             shearY,
                        CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_Translate(CAffineTransformF *_this,
                            CFloat             offsetX,
                            CFloat             offsetY,
                            CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_TranslateInverse(CAffineTransformF *_this,
                                   CFloat             offsetX,
                                   CFloat             offsetY,
                                   CMatrixOrder       order);
CINTERNAL void
CAffineTransformF_TransformPoints(const CAffineTransformF *_this,
                                  CPointF                 *points,
                                  CUInt32                  count);
CINTERNAL void
CAffineTransformF_TransformVectors(const CAffineTransformF *_this,
                                   CPointF                 *points,
                                   CUInt32                  count);

CINTERNAL void
CVectorF_ScalePoints(const CVectorF *_this,
                     CPointF        *points,
                     CUInt32         count);
CINTERNAL void
CVectorF_TranslatePoints(const CVectorF *_this,
                         CPointF        *points,
                         CUInt32         count);

#ifdef __cplusplus
};
#endif

#endif /* _C_AFFINETRANFSORM_H_ */
