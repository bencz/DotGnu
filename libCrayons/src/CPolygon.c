/*
 * CPolygon.c - Polygon implementation.
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

#include "CPolygon.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL void
CPolygonX_Initialize(CPolygonX *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the path */
	_this->capacity        = 0;
	_this->count           = 0;
	_this->edges           = 0;
	_this->hasCurrentPoint = 0;
}

CINTERNAL void
CPolygonX_Finalize(CPolygonX *_this)
{
	/* assertions */
	CASSERT((_this  != 0));

	/* finalize the polygon */
	{
		/* get the edge list */
		CEdgeX *edges = _this->edges;

		/* finalize, as needed */
		if(edges != 0)
		{
			/* reset the members */
			_this->capacity  = 0;
			_this->count     = 0;
			_this->edges     = 0;

			/* free the edge list */
			CFree(edges);
		}

		/* reset the current point flag */
		_this->hasCurrentPoint = 0;
	}
}

CINTERNAL void
CPolygonX_Reset(CPolygonX *_this)
{
	/* assertions */
	CASSERT((_this  != 0));

	/* reset the members */
	_this->count           = 0;
	_this->hasCurrentPoint = 0;
}

CINTERNAL CPointX
CPolygonX_GetCurrentPoint(CPolygonX *_this)
{
	/* assertions */
	CASSERT((_this  != 0));

	/* return the current point */
	return _this->currentPoint;
}

CINTERNAL CBool
CPolygonX_HasCurrentPoint(CPolygonX *_this)
{
	/* assertions */
	CASSERT((_this  != 0));

	/* return the has current point flag */
	return _this->hasCurrentPoint;
}

CINTERNAL CStatus
CPolygonX_AddEdge(CPolygonX *_this,
                  CPointX   *point1,
                  CPointX   *point2)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((point1 != 0));
	CASSERT((point2 != 0));

	/* add the edge, if it isn't horizontal */
	if(CPoint_Y(*point1) != CPoint_Y(*point2))
	{
		/* get the edge count */
		const CUInt32 count = _this->count;

		/* get the capacity */
		const CUInt32 capacity = _this->capacity;

		/* ensure the capacity of edge list */
		if(count >= capacity)
		{
			/* declarations */
			CEdgeX  *tmp;
			CUInt32  newSize;
			CUInt32  newCapacity;

			/* calculate the new capacity */
			newCapacity = ((capacity + 32) & ~31);

			/* calculate the optimal capacity, as needed */
			if(capacity != 0)
			{
				/* calculate a new capacity candidate */
				const CUInt32 newCapacity2 = (capacity << 1);

				/* use the larger candidate capacity */
				if(newCapacity < newCapacity2)
				{
					newCapacity = newCapacity2;
				}
			}

			/* calculate the new points size */
			newSize = (newCapacity  * sizeof(CEdgeX));

			/* create the new edge list */
			if(!(tmp = (CEdgeX *)CRealloc(_this->edges, newSize)))
			{
				return CStatus_OutOfMemory;
			}

			/* update the capacity */
			_this->capacity = newCapacity;

			/* set the edge list */
			_this->edges = tmp;
		}

		/* add the new edge */
		{
			/* get the current edge */
			CEdgeX *edge = (_this->edges + count);

			/* set the edge properties based on the point order */
			if(CPoint_Y(*point1) < CPoint_Y(*point2))
			{
				CEdge_Point1(*edge)    = *point1;
				CEdge_Point2(*edge)    = *point2;
				CEdge_Clockwise(*edge) =  1;
			}
			else
			{
				CEdge_Point1(*edge)    = *point2;
				CEdge_Point2(*edge)    = *point1;
				CEdge_Clockwise(*edge) =  0;
			}
		}

		/* update the edge count */
		_this->count = (count + 1);
	}

	/* update the current point */
	CPolygonX_MoveTo(_this, point2);

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPolygonX_MoveTo(CPolygonX *_this,
                 CPointX   *point)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((point != 0));

	/* set the first point, as needed */
	if(!(_this->hasCurrentPoint))
	{
		_this->firstPoint = *point;
	}

	/* set the current point */
	_this->currentPoint    = *point;
	_this->hasCurrentPoint = 1;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPolygonX_LineTo(CPolygonX *_this,
                 CPointX   *point)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((point != 0));

	/* add an edge, if possible */
	if(_this->hasCurrentPoint)
	{
		return CPolygonX_AddEdge(_this, &(_this->currentPoint), point);
	}

	/* move to the point */
	CPolygonX_MoveTo(_this, point);

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPolygonX_Close(CPolygonX *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* close the polygon, as needed */
	if(_this->hasCurrentPoint)
	{
		/* add an edge from the last point to the first */
		CStatus_Check
			(CPolygonX_AddEdge
				(_this, &(_this->currentPoint), &(_this->firstPoint)));

		/* reset current point flag */
		_this->hasCurrentPoint = 0;
	}

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
