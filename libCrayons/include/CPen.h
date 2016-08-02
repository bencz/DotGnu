/*
 * CPen.h - Pen header.
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

#ifndef _C_PEN_H_
#define _C_PEN_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCPen
{
	CFloat             dashOffset;
	CFloat             miterLimit;
	CFloat             width;
	CFloat            *compoundArray;
	CFloat            *dashPattern;
	CUInt32            compoundCount;
	CUInt32            dashCount;
	CAffineTransformF  transform;
	CDashCap           dashCap;
	CDashStyle         dashStyle;
	CBrush            *brush;
	CCustomLineCap    *customEndCap;
	CCustomLineCap    *customStartCap;
	CLineCap           endCap;
	CLineCap           startCap;
	CLineJoin          lineJoin;
	CPenAlignment      alignment;
};

CINTERNAL CStatus
CPen_GetPattern(CPen     *_this,
                CPattern *pattern);

#ifdef __cplusplus
};
#endif

#endif /* _C_PEN_H_ */
