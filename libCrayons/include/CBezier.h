/*
 * CBezier.h - Bezier header.
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

#ifndef _C_BEZIER_H_
#define _C_BEZIER_H_

#include "CPointArray.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CBezierX CBezierX_Zero =
{
	{ CFixed_Zero, CFixed_Zero },
	{ CFixed_Zero, CFixed_Zero },
	{ CFixed_Zero, CFixed_Zero },
	{ CFixed_Zero, CFixed_Zero }
};

static const CBezierF CBezierF_Zero =
{
	{ 0.0f, 0.0f },
	{ 0.0f, 0.0f },
	{ 0.0f, 0.0f },
	{ 0.0f, 0.0f }
};

CINTERNAL CBool
CBezierX_Initialize(CBezierX *_this,
                    CPointX  *a,
                    CPointX  *b,
                    CPointX  *c,
                    CPointX  *d);
CINTERNAL CBool
CBezierF_Initialize(CBezierF *_this,
                    CPointF  *a,
                    CPointF  *b,
                    CPointF  *c,
                    CPointF  *d);
CINTERNAL void
CBezierX_Finalize(CBezierX *_this);
CINTERNAL void
CBezierF_Finalize(CBezierF *_this);

CINTERNAL CStatus
CBezierX_Flatten(CBezierX     *_this,
                 CPointArrayX *array,
                 CDouble       tolerance);
CINTERNAL CStatus
CBezierF_Flatten(CBezierF     *_this,
                 CPointArrayF *array,
                 CDouble       tolerance);

#ifdef __cplusplus
};
#endif

#endif /* _C_BEZIER_H_ */
