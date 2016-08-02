/*
 * CStroker.h - Stroker header.
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

#ifndef _C_STROKER_H_
#define _C_STROKER_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCStrokeJoiner CStrokeJoiner;
typedef struct _tagCStrokeCapper CStrokeCapper;

struct _tagCStrokeJoiner
{
	CLineJoin type;
	union
	{
		struct
		{
			CPointF *points;
			CUInt32  count;
			CUInt32  size;
		} round;
		struct
		{
			CAffineTransformF  *transform;
			CVectorF            scale;
			CDouble             limitSquared;
		} other;
	} u;

	CStatus (*Join)(CStrokeJoiner *_this,
	                CPath         *path,
	                CFloat         centerX,
	                CFloat         centerY,
	                CFloat         prevC,
	                CFloat         prevS,
	                CFloat         currC,
	                CFloat         currS);
};

struct _tagCStrokeCapper
{
	CLineCap type;
	union
	{
		struct
		{
			CPointF *points;
			CUInt32  count;
			CUInt32  size;
		} round;
		struct
		{
			CFloat radius;
			union
			{
				CAffineTransformF  *transform;
				CVectorF           *scale;
			} u;
		} other;
	} u;

	CStatus (*Cap)(CStrokeCapper *_this,
	               CPath         *path,
	               CFloat        *centerX,
	               CFloat        *centerY,
	               CFloat         slopeX,
	               CFloat         slopeY);
};

struct _tagCStroker
{
	CPointArrayF       array;
	CStrokeCapper      startCapper;
	CStrokeCapper      endCapper;
	CStrokeJoiner      joiner;
	CAffineTransformF  dev;
	CAffineTransformF  pen;
	CVectorF           scale;
	CFloat             radius;

	CStatus (*Stroke)(CStroker *_this,
	                  CPath    *path,
	                  CPointF  *points,
	                  CUInt32   count);
};

CINTERNAL CStatus
CStroker_Initialize(CStroker          *_this,
                    CPen              *pen,
                    CAffineTransformF *deviceTransform);
CINTERNAL void
CStroker_Finalize(CStroker *_this);
CINTERNAL CStatus
CStroker_Stroke(CStroker *_this,
                CPath    *path,
                CPointF  *points,
                CByte    *types,
                CUInt32   count,
                CBool     hasCurves);

#ifdef __cplusplus
};
#endif

#endif /* _C_STROKER_H_ */
