/*
 * CPolygon.h - Polygon header.
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

#ifndef _C_POLYGON_H_
#define _C_POLYGON_H_

#include "CrayonsInternal.h"

#ifdef __cplusplus
extern "C" {
#endif

struct _tagCPolygonX
{
	CUInt32  capacity;
	CUInt32  count;
	CEdgeX  *edges;
	CPointX  firstPoint;
	CPointX  currentPoint;
	CBool    hasCurrentPoint;
};

CINTERNAL void
CPolygonX_Initialize(CPolygonX *_this);
CINTERNAL void
CPolygonX_Finalize(CPolygonX *_this);
CINTERNAL void
CPolygonX_Reset(CPolygonX *_this);
CINTERNAL CPointX
CPolygonX_GetCurrentPoint(CPolygonX *_this);
CINTERNAL CBool
CPolygonX_HasCurrentPoint(CPolygonX *_this);
CINTERNAL CStatus
CPolygonX_AddEdge(CPolygonX *_this,
                  CPointX   *point1,
                  CPointX   *point2);
CINTERNAL CStatus
CPolygonX_MoveTo(CPolygonX *_this,
                 CPointX   *point);
CINTERNAL CStatus
CPolygonX_LineTo(CPolygonX *_this,
                 CPointX   *point);
CINTERNAL CStatus
CPolygonX_Close(CPolygonX *_this);

#define CPolygon_Edges(polygon)     ((polygon).edges)
#define CPolygon_EdgeCount(polygon) ((polygon).count)

#ifdef __cplusplus
};
#endif

#endif /* _C_POLYGON_H_ */
