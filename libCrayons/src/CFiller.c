/*
 * CFiller.c - Filler implementation.
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

#include "CFiller.h"
#include "CBezier.h"
#include "CTrapezoids.h"

#ifdef __cplusplus
extern "C" {
#endif

static CStatus
CFiller_Move(CPathInterpreter *_this,
             CFloat            x,
             CFloat            y,
             CPathType         type)
{
	/* declarations */
	CPolygonX *polygon;
	CPointX    point;

	/* assertions */
	CASSERT((_this != 0));

	/* get the polygon */
	polygon = &(((CFiller *)_this)->polygon);

	/* close the polygon */
	CStatus_Check
		(CPolygonX_Close
			(polygon));

	/* get the point */
	CPoint_X(point) = CFloat_ToFixed(x);
	CPoint_Y(point) = CFloat_ToFixed(y);

	/* move to the point */
	CStatus_Check
		(CPolygonX_MoveTo
			(polygon, &point));

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CFiller_Line(CPathInterpreter *_this,
             CFloat            x,
             CFloat            y,
             CPathType         type)
{
	/* declarations */
	CPolygonX *polygon;
	CPointX    point;

	/* assertions */
	CASSERT((_this != 0));

	/* get the polygon */
	polygon = &(((CFiller *)_this)->polygon);

	/* get the point */
	CPoint_X(point) = CFloat_ToFixed(x);
	CPoint_Y(point) = CFloat_ToFixed(y);

	/* line to the point */
	CStatus_Check
		(CPolygonX_LineTo
			(polygon, &point));

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CFiller_Curve(CPathInterpreter *_this,
              CFloat            x1,
              CFloat            y1,
              CFloat            x2,
              CFloat            y2,
              CFloat            x3,
              CFloat            y3,
              CPathType         type)
{
	/* declarations */
	CPointArrayX *array;
	CPolygonX    *polygon;
	CBezierX      bezier;

	/* assertions */
	CASSERT((_this != 0));

	/* perform setup for curve flattening */
	{
		CFiller *filler;
		CPointX  a;
		CPointX  b;
		CPointX  c;
		CPointX  d;

		/* get this as a filler */
		filler = ((CFiller *)_this);

		/* get the polygon */
		polygon = &(filler->polygon);

		/* get the current point */
		a = CPolygonX_GetCurrentPoint(polygon);

		/* get the first point */
		CPoint_X(b) = CFloat_ToFixed(x1);
		CPoint_Y(b) = CFloat_ToFixed(y1);

		/* get the second point */
		CPoint_X(c) = CFloat_ToFixed(x2);
		CPoint_Y(c) = CFloat_ToFixed(y2);

		/* get the third point */
		CPoint_X(d) = CFloat_ToFixed(x3);
		CPoint_Y(d) = CFloat_ToFixed(y3);

		/* initialize bezier and bail out now if curve is degenerate */
		if(CBezierX_Initialize(&bezier, &a, &b, &c, &d))
		{
			return CStatus_OK;
		}

		/* get the point array */
		array = &(filler->array);

		/* reset the count of the point array */
		CPointArray_Count(*array) = 0;
	}

	/* flatten the bezier curve */
	CStatus_Check
		(CBezierX_Flatten
			(&bezier, array, CFiller_TOLERANCE));

	/* add the lines to the polygon */
	{
		/* declarations */
		CPointX *curr;
		CPointX *end;

		/* get the point pointer */
		curr = CPointArray_Points(*array);

		/* get the end pointer */
		end = (curr + CPointArray_Count(*array));

		/* skip the current point */
		++curr;

		/* add the lines */
		while(curr != end)
		{
			/* add the current point */
			CStatus_Check
				(CPolygonX_LineTo
					(polygon, curr));

			/* move to the next point */
			++curr;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CFiller_Close(CPathInterpreter *_this)
{
	/* declarations */
	CFiller   *filler;
	CPolygonX *polygon;

	/* assertions */
	CASSERT((_this != 0));

	/* get this as a filler */
	filler = ((CFiller *)_this);

	/* get the polygon */
	polygon = &(filler->polygon);

	/* close the polygon */
	CStatus_Check(CPolygonX_Close(polygon));

	/* tessellate the polygon, as needed */
	if(filler->trapezoids != 0)
	{
		/* tessellate the polygon */
		CStatus_Check
			(CTrapezoids_TessellatePolygon
				(filler->trapezoids, polygon, filler->fillMode));

		/* reset the polygon */
		CPolygonX_Reset(polygon);
	}

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CFiller_ToPolygon(CFiller       *_this,
                  CPolygonX     *polygon,
                  const CPointF *points,
                  const CByte   *types,
                  CUInt32        count)
{
	/* declarations */
	CPathInterpreter *interpreter;
	CPolygonX         original;
	CStatus           status;

	/* assertions */
	CASSERT((_this    != 0));
	CASSERT((points  != 0));
	CASSERT((types   != 0));
	CASSERT((polygon != 0));

	/* get this as a path interpreter */
	interpreter = ((CPathInterpreter *)_this);

	/* swap the polygons */
	original       = _this->polygon;
	_this->polygon = *polygon;

	/* interpret the path */
	status = CPathInterpreter_Interpret(interpreter, points, types, count);

	/* handle path interpretation failures */
	if(status != CStatus_OK)
	{
		/* swap the polygons */
		*polygon       = _this->polygon;
		_this->polygon = original;

		/* return status */
		return status;
	}

	/* ensure the polygon is closed */
	status = CPolygonX_Close(&(_this->polygon));

	/* swap the polygons */
	*polygon       = _this->polygon;
	_this->polygon = original;

	/* return status */
	return status;
}

CINTERNAL CStatus
CFiller_ToTrapezoids(CFiller       *_this,
                     CTrapezoids   *trapezoids,
                     const CPointF *points,
                     const CByte   *types,
                     CUInt32        count,
                     CFillMode      fillMode)
{
	/* declarations */
	CPathInterpreter *interpreter;
	CStatus           status;

	/* assertions */
	CASSERT((_this       != 0));
	CASSERT((points     != 0));
	CASSERT((types      != 0));
	CASSERT((trapezoids != 0));

	/* get this as a path interpreter */
	interpreter = ((CPathInterpreter *)_this);

	/* set the trapezoids */
	_this->trapezoids = trapezoids;

	/* set the fill mode */
	_this->fillMode = fillMode;

	/* interpret the path */
	status = CPathInterpreter_Interpret(interpreter, points, types, count);

	/* reset the trapezoids */
	_this->trapezoids = 0;

	/* handle path interpretation failures */
	CStatus_Check(status);

	/* finish trapezoidation, as needed */
	if(CPolygonX_HasCurrentPoint(&(_this->polygon)))
	{
		/* ensure the polygon is closed */
		CStatus_Check
			(CPolygonX_Close
				(&(_this->polygon)));

		/* tessellate the polygon */
		return
			CTrapezoids_TessellatePolygon
				(trapezoids, &(_this->polygon), fillMode);
	}

	/* return successfully */
	return CStatus_OK;
}

static const CPathInterpreterClass CFiller_Class =
{
	CFiller_Move,
	CFiller_Line,
	CFiller_Curve,
	CFiller_Close,
	"sentinel"
};

CINTERNAL void
CFiller_Initialize(CFiller *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the base */
	_this->_base._class = &CFiller_Class;

	/* initialize the point array */
	CPointArrayX_Initialize(&(_this->array));

	/* initialize the polygon */
	CPolygonX_Initialize(&(_this->polygon));

	/* set the trapezoids to the default */
	_this->trapezoids = 0;
}

CINTERNAL void
CFiller_Finalize(CFiller *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reset the trapezoids */
	_this->trapezoids = 0;

	/* finalize the point array */
	CPointArrayX_Finalize(&(_this->array));

	/* finalize the polygon */
	CPolygonX_Finalize(&(_this->polygon));

	/* finalize the base */
	_this->_base._class = 0;
}

CINTERNAL void
CFiller_Reset(CFiller *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reset the trapezoids */
	_this->trapezoids = 0;

	/* reset the polygon */
	CPolygonX_Reset(&(_this->polygon));

	/* reset the point array */
	CPointArray_Count(_this->array) = 0;
}

#ifdef __cplusplus
};
#endif
