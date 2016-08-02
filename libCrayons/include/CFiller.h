/*
 * CFiller.h - Filler header.
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

#ifndef _C_FILLER_H_
#define _C_FILLER_H_

#include "CPathInterpreter.h"
#include "CPolygon.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCFiller
{
	CPathInterpreter  _base;
	CPointArrayX      array;
	CPolygonX         polygon;
	CTrapezoids      *trapezoids;
	CFillMode         fillMode;
};

CINTERNAL void
CFiller_Initialize(CFiller *_this);
CINTERNAL void
CFiller_Finalize(CFiller *_this);
CINTERNAL void
CFiller_Reset(CFiller *_this);
CINTERNAL CStatus
CFiller_ToPolygon(CFiller       *_this,
                  CPolygonX     *polygon,
                  const CPointF *points,
                  const CByte   *types,
                  CUInt32        count);
CINTERNAL CStatus
CFiller_ToTrapezoids(CFiller       *_this,
                     CTrapezoids   *trapezoids,
                     const CPointF *points,
                     const CByte   *types,
                     CUInt32        count,
                     CFillMode      fillMode);

#ifdef __cplusplus
};
#endif

#endif /* _C_FILLER_H_ */
