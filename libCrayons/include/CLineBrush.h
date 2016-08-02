/*
 * CLineBrush.h - Linear gradient brush header.
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

#ifndef _C_LINEBRUSH_H_
#define _C_LINEBRUSH_H_

#include "CBlend.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCLineBrush
{
	CBrush            _base;
	CAffineTransformF transform;
	CRectangleF       rectangle;
	CWrapMode         wrapMode;
	CBlend            blend;
	CColorBlend       colorBlend;
	CColor            startColor;
	CColor            endColor;
	CFloat            angle;
	CBitField         isAngleScalable : 1;
	CBitField         gammaCorrection : 1;
};

static CStatus
CLineBrush_Clone(CBrush  *_this,
                 CBrush **_clone);
static void
CLineBrush_Finalize(CBrush *_this);
static CStatus
CLineBrush_CreatePattern(CBrush   *_this,
                         CPattern *pattern);

static const CBrushClass CLineBrush_Class =
{
	CBrushType_LinearGradient,
	CLineBrush_Clone,
	CLineBrush_Finalize,
	CLineBrush_CreatePattern,
	"sentinel"
};

#ifdef __cplusplus
};
#endif

#endif /* _C_LINEBRUSH_H_ */
