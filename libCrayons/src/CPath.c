/*
 * CPath.c - Path implementation.
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

#include "CPath.h"
#include "CFiller.h"
#include "CFlattener.h"
#include "CMatrix.h"
#include "CStroker.h"
#include "CTrapezoids.h"

#ifdef __cplusplus
extern "C" {
#endif

#define _TYPE_LINE_CLOSE   (CPathType_Line   | CPathType_CloseSubpath)
#define _TYPE_BEZIER_CLOSE (CPathType_Bezier | CPathType_CloseSubpath)

#define _SetPoint(currP, currT, x, y, type)                                    \
		*(currT) = (type);                                                     \
		CPoint_X(*(currP)) = (x);                                              \
		CPoint_Y(*(currP)) = (y)
#define _NextPoint(currP, currT)                                               \
	++currP; ++currT
#define _SetPointAdvance(currP, currT, x, y, type)                             \
	_SetPoint((currP), (currT), (x), (y), (type));                             \
	_NextPoint((currP), (currT))
#define _MoveTo(currP, currT, x, y)                                            \
	do {                                                                       \
		_SetPointAdvance((currP), (currT), (x), (y), CPathType_Start);         \
	} while(0)
#define _LineTo(currP, currT, x, y)                                            \
	do {                                                                       \
		_SetPointAdvance((currP), (currT), (x), (y), CPathType_Line);          \
	} while(0)
#define _LineToClose(currP, currT, x, y)                                       \
	do {                                                                       \
		_SetPointAdvance((currP), (currT), (x), (y), _TYPE_LINE_CLOSE);        \
	} while(0)
#define _CurveTo(currP, currT, x1, y1, x2, y2, x3, y3)                         \
	do {                                                                       \
		_SetPointAdvance((currP), (currT), (x1), (y1), CPathType_Bezier);      \
		_SetPointAdvance((currP), (currT), (x2), (y2), CPathType_Bezier);      \
		_SetPointAdvance((currP), (currT), (x3), (y3), CPathType_Bezier);      \
	} while(0)
#define _CurveToClose(currP, currT, x1, y1, x2, y2, x3, y3)                    \
	do {                                                                       \
		_SetPointAdvance((currP), (currT), (x1), (y1), CPathType_Bezier);      \
		_SetPointAdvance((currP), (currT), (x2), (y2), CPathType_Bezier);      \
		_SetPointAdvance((currP), (currT), (x3), (y3), _TYPE_BEZIER_CLOSE);    \
	} while(0)
#define _BeginAdd(path, n, plist, tlist, x1, y1)                               \
	do {                                                                       \
		/* get the current count */                                            \
		const CUInt32 _cc_ = (path)->count;                                    \
		                                                                       \
		/* ensure the capacity of the point and type lists */                  \
		CStatus_Check(CPath_EnsureCapacity((path), (_cc_ + (n))));             \
		                                                                       \
		/* update the count */                                                 \
		(path)->count = (_cc_ + (n));                                          \
		                                                                       \
		/* get the point and type list pointers */                             \
		(plist) = ((path)->points + _cc_);                                     \
		(tlist) = ((path)->types  + _cc_);                                     \
		                                                                       \
		/* set the start point */                                              \
		if((path)->newFigure)                                                  \
		{                                                                      \
			/* start the new figure */                                         \
			_MoveTo((plist), (tlist), (x1), (y1));                             \
			                                                                   \
			/* reset the new figure flag */                                    \
			(path)->newFigure = 0;                                             \
		}                                                                      \
		else                                                                   \
		{                                                                      \
			/* set a line to the start point */                                \
			_LineTo((plist), (tlist), (x1), (y1));                             \
		}                                                                      \
	} while(0)
#define _BeginNew(path, n, plist, tlist, x1, y1)                               \
	do {                                                                       \
		/* get the current count */                                            \
		const CUInt32 _cc_ = (path)->count;                                    \
		                                                                       \
		/* ensure the capacity of the point and type lists */                  \
		CStatus_Check(CPath_EnsureCapacity((path), (_cc_ + (n))));             \
		                                                                       \
		/* update the count */                                                 \
		(path)->count = (_cc_ + (n));                                          \
		                                                                       \
		/* get the point and type list pointers */                             \
		(plist) = ((path)->points + _cc_);                                     \
		(tlist) = ((path)->types  + _cc_);                                     \
		                                                                       \
		/* start the new figure */                                             \
		_MoveTo((plist), (tlist), (x1), (y1));                                 \
			                                                                   \
		/* set the new figure flag */                                          \
		(path)->newFigure = 1;                                                 \
	} while(0)

static void
CPath_Initialize(CPath *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the path */
	_this->capacity  = 0;
	_this->count     = 0;
	_this->points    = 0;
	_this->types     = 0;
	_this->winding   = 0;
	_this->newFigure = 1;
	_this->hasCurves = 0;
}

static void
CPath_Finalize(CPath *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the path */
	{
		/* get the point list */
		CPointF *points = _this->points;

		/* get the type list */
		CByte *types = _this->types;

		/* finalize, as needed */
		if(points != 0)
		{
			/* reset the members */
			_this->capacity  = 0;
			_this->count     = 0;
			_this->points    = 0;
			_this->types     = 0;
			_this->winding   = 0;
			_this->newFigure = 1;
			_this->hasCurves = 0;

			/* free the point list */
			CFree(points);

			/* free the type list */
			CFree(types);
		}
	}
}

/*\
|*| Ensure the capacity of point and type lists of this path.
|*|
|*|   _this - this path
|*|   count - the total minimum capacity required
|*|
|*|  Returns status code.
\*/
static CStatus
CPath_EnsureCapacity(CPath   *_this,
                     CUInt32  minimum)
{
	/* assertions */
	CASSERT((_this != 0));

	/* ensure capacity */
	_EnsurePathCapacity(_this, minimum);

	/* return successfully */
	return CStatus_OK;
}

/* Sanity check the given type list. */
static CStatus
_SanityCheckTypes(CByte   *types,
                  CUInt32  count)
{
	/* TODO: assume all is ok for now */
	return CStatus_OK;
}


/* Create a path. */
CStatus
CPath_Create(CPath **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the path */
	if(!(*_this = (CPath *)CMalloc(sizeof(CPath))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the path */
	CPath_Initialize(*_this);

	/* return successfully */
	return CStatus_OK;
}

/* Destroy a path. */
CStatus
CPath_Destroy(CPath **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require(((*_this) != 0), CStatus_ArgumentNull);

	/* finalize the path */
	CPath_Finalize(*_this);

	/* free the path */
	CFree(*_this);

	/* null the this pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Get the fill mode of this path. */
CStatus
CPath_GetFillMode(CPath     *_this,
                  CFillMode *fillMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a fill mode pointer */
	CStatus_Require((fillMode != 0), CStatus_ArgumentNull);

	/* get the fill mode */
	*fillMode = (_this->winding ? CFillMode_Winding : CFillMode_Alternate);

	/* return successfully */
	return CStatus_OK;
}

/* Set the fill mode of this path. */
CStatus
CPath_SetFillMode(CPath     *_this,
                  CFillMode  fillMode)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the fill mode */
	_this->winding = ((fillMode == CFillMode_Winding) ?  1 : 0);

	/* return successfully */
	return CStatus_OK;
}

/* Get the points in this path. */
CStatus
CPath_GetPoints(CPath    *_this,
                CPointF **points,
                CUInt32  *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the points */
	{
		/* get the count and size */
		const CUInt32 cnt  = _this->count;
		const CUInt32 size = (cnt * sizeof(CPointF));

		/* allocate the point list */
		if(!(*points = (CPointF *)CMalloc(size)))
		{
			*count = 0;
			return CStatus_OutOfMemory;
		}

		/* get the points */
		CMemCopy(*points, _this->points, size);

		/* get the count */
		*count = cnt;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the types of the points in this path. */
CStatus
CPath_GetTypes(CPath    *_this,
               CByte   **types,
               CUInt32  *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a type list pointer */
	CStatus_Require((types != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the types */
	{
		/* get the count and size */
		const CUInt32 cnt  = _this->count;
		const CUInt32 size = (cnt * sizeof(CByte));

		/* allocate the type list */
		if(!(*types = (CByte *)CMalloc(size)))
		{
			*count = 0;
			return CStatus_OutOfMemory;
		}

		/* get the types */
		CMemCopy(*types, _this->types, size);

		/* get the count */
		*count = cnt;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the points and types in this path. */
CStatus
CPath_GetPathData(CPath    *_this,
                  CPointF **points,
                  CByte   **types,
                  CUInt32  *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* ensure we have a type list pointer */
	CStatus_Require((types != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the path data */
	{
		/* get the count and sizes */
		const CUInt32 cnt   = _this->count;
		const CUInt32 sizeP = (cnt * sizeof(CPointF));
		const CUInt32 sizeT = (cnt * sizeof(CByte));

		/* allocate the point list */
		if(!(*points = (CPointF *)CMalloc(sizeP)))
		{
			*types = 0;
			*count = 0;
			return CStatus_OutOfMemory;
		}

		/* allocate the type list */
		if(!(*types = (CByte *)CMalloc(sizeT)))
		{
			CFree(*points);
			*points = 0;
			*count = 0;
			return CStatus_OutOfMemory;
		}

		/* get the points */
		CMemCopy(*points, _this->points, sizeP);

		/* get the types */
		CMemCopy(*types, _this->types, sizeT);

		/* get the count */
		*count = cnt;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the points and types in this path. */
CStatus
CPath_SetPathData(CPath   *_this,
                  CPointF *points,
                  CByte   *types,
                  CUInt32  count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* ensure we have a type list */
	CStatus_Require((types != 0), CStatus_ArgumentNull);

	/* sanity check the types */
	CStatus_Check(_SanityCheckTypes(types, count));

	/* set the points and types */
	{
		/* ensure the capacity of the point and type lists */
		CStatus_Check(CPath_EnsureCapacity(_this, count));

		/* update the count */
		_this->count = count;

		/* copy the points to the point list */
		CMemCopy(_this->points, points, (count * sizeof(CPointF)));

		/* copy the types to the type list */
		CMemCopy(_this->types, types, (count * sizeof(CByte)));

		/* set the new figure flag */
		_this->newFigure =
			((count == 0) ||
			 ((types[count - 1] & CPathType_CloseSubpath) != 0));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add an arc to the current figure. */
CStatus
CPath_AddArc(CPath  *_this,
             CFloat  x,
             CFloat  y,
             CFloat  width,
             CFloat  height,
             CFloat  startAngle,
             CFloat  sweepAngle)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Add a bezier curve to the current figure. */
CStatus
CPath_AddBezier(CPath  *_this,
                CFloat  x1,
                CFloat  y1,
                CFloat  x2,
                CFloat  y2,
                CFloat  x3,
                CFloat  y3,
                CFloat  x4,
                CFloat  y4)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* add the bezier curve */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* perform standard startup procedures for path segment additions */
		_BeginAdd(_this, 4, p, t, x1, y1);

		/* complete the curve */
		_CurveTo(p, t, x2, y2, x3, y3, x4, y4);

		/* flag that we have curves */
		_this->hasCurves = 1;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a sequence of connected bezier curves to the current figure. */
CStatus
CPath_AddBeziers(CPath   *_this,
                 CPointF *points,
                 CUInt32  count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* ensure we have the required minimum number of points */
	CStatus_Require((count >= 4), CStatus_Argument_NeedAtLeast4Points);

	/* ensure we have a valid number of additional points */
	CStatus_Require
		((((count - 4) % 3) == 0), CStatus_Argument_InvalidPointCount);

	/* add the bezier curves */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* get the end of input pointer */
		const CPointF *const end = (points + count);

		/* perform standard startup procedures for path segment additions */
		_BeginAdd
			(_this,             count,
			 p,                 t,
			 CPoint_X(*points), CPoint_Y(*points++));

		/* add the curve segments */
		while(points != end)
		{
			_CurveTo
				(p,                 t,
				 CPoint_X(*points), CPoint_Y(*points++),
				 CPoint_X(*points), CPoint_Y(*points++),
				 CPoint_X(*points), CPoint_Y(*points++));
		}

		/* flag that we have curves */
		_this->hasCurves = 1;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a closed curve to this path. */
CStatus
CPath_AddClosedCardinalCurve(CPath   *_this,
                             CPointF *points,
                             CUInt32  count,
                             CFloat   tension)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Add a curve to the current figure. */
CStatus
CPath_AddCardinalCurve(CPath   *_this,
                       CPointF *points,
                       CUInt32  count,
                       CUInt32  offset,
                       CUInt32  numberOfSegments,
                       CFloat   tension)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Add an ellipse to this path. */
CStatus
CPath_AddEllipse(CPath  *_this,
                 CFloat  x,
                 CFloat  y,
                 CFloat  width,
                 CFloat  height)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* add the ellipse */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* calculate the radii */
		const CDouble rX = width / 2;
		const CDouble rY = height / 2;

		/* calculate the distances along the tangents */
		const CDouble dX = rX * CMath_Arc90Fraction;
		const CDouble dY = rY * CMath_Arc90Fraction;

		/* calculate the center point */
		const CFloat cX = x + rX;
		const CFloat cY = y + rY;

		/* calculate the tangential control points */
		const CFloat pX = cX + dX;
		const CFloat pY = cY + dY;
		const CFloat mX = cX - dX;
		const CFloat mY = cY - dY;

		/* calculate the edge points */
		const CFloat right  = x + width;
		const CFloat bottom = y + height;

		/* perform standard startup procedures for new figure additions */
		_BeginNew(_this, 13, p, t, right, cY);

		/* curve counter-clockwise, starting from the top-right quadrant */
		_CurveTo     (p, t, right, mY,     pX,    y,      cX,    y);
		_CurveTo     (p, t, mX,    y,      x,     mY,     x,     cY);
		_CurveTo     (p, t, x,     pY,     mX,    bottom, cX,    bottom);
		_CurveToClose(p, t, pX,    bottom, right, pY,     right, cY);

		/* flag that we have curves */
		_this->hasCurves = 1;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a line to the current figure. */
CStatus
CPath_AddLine(CPath  *_this,
              CFloat  x1,
              CFloat  y1,
              CFloat  x2,
              CFloat  y2)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* add the line */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* perform standard startup procedures for path segment additions */
		_BeginAdd(_this, 2, p, t, x1, y1);

		/* complete the line */
		_LineTo(p,  t, x2, y2);
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a sequence of connected line segments to the current figure. */
CStatus
CPath_AddLines(CPath   *_this,
               CPointF *points,
               CUInt32  count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* ensure we have the required minimum number of points */
	CStatus_Require((count >= 2), CStatus_Argument_NeedAtLeast2Points);

	/* add the lines */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* get the end of input pointer */
		const CPointF *const end = (points + count);

		/* perform standard startup procedures for path segment additions */
		_BeginAdd
			(_this,             count,
			 p,                 t,
			 CPoint_X(*points), CPoint_Y(*points++));

		/* add the line segments */
		while(points != end)
		{
			_LineTo
				(p,                 t,
				 CPoint_X(*points), CPoint_Y(*points++));
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add the contents of another path to this path. */
CStatus
CPath_AddPath(CPath *_this,
              CPath *path,
              CBool  connect)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a path pointer */
	CStatus_Require((path != 0), CStatus_ArgumentNull);

	/* bail out now if there's nothing to do */
	CStatus_Require((path->count != 0), CStatus_OK);

	/* add the path */
	{
		/* ensure the path capacity */
		CStatus_Check
			(CPath_EnsureCapacity
				(_this, (_this->count + path->count)));

		/* copy the points */
		CMemCopy
			((_this->points + _this->count),
			 path->points, (path->count * sizeof(CPointF)));

		/* copy the types */
		CMemCopy
			((_this->types + _this->count),
			 path->types, (path->count * sizeof(CByte)));

		/* connect the path as needed */
		if(connect && !(_this->newFigure))
		{
			_this->types[_this->count] = CPathType_Line;
		}

		/* update the flags */
		_this->hasCurves |= path->hasCurves;
		_this->newFigure  = path->newFigure;

		/* update the count */
		_this->count += path->count;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a pie section to this path. */
CStatus
CPath_AddPie(CPath  *_this,
             CFloat  x,
             CFloat  y,
             CFloat  width,
             CFloat  height,
             CFloat  startAngle,
             CFloat  sweepAngle)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Add a polygon to this path. */
CStatus
CPath_AddPolygon(CPath   *_this,
                 CPointF *points,
                 CUInt32  count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a point list pointer */
	CStatus_Require((points != 0), CStatus_ArgumentNull);

	/* ensure we have the required minimum number of points */
	CStatus_Require((count >= 3), CStatus_Argument_NeedAtLeast3Points);

	/* add the polygon */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* get the last point pointer */
		const CPointF *last = (points + count - 1);

		/* ignore redundant last points */
		if(CPoint_X(*points) == CPoint_X(*last) &&
		   CPoint_Y(*points) == CPoint_Y(*last))
		{
			--last;
			--count;
		}

		/* perform standard startup procedures for new figure additions */
		_BeginNew
			(_this,             count,
			 p,                 t,
			 CPoint_X(*points), CPoint_Y(*points++));

		/* add the polygon edges */
		while(points != last)
		{
			_LineTo
				(p,                 t,
				 CPoint_X(*points), CPoint_Y(*points++));
		}

		/* complete the polygon */
		_LineToClose
			(p,                 t,
			 CPoint_X(*points), CPoint_Y(*points));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a rectangle to this path. */
CStatus
CPath_AddRectangle(CPath  *_this,
                   CFloat  x,
                   CFloat  y,
                   CFloat  width,
                   CFloat  height)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* add the rectangle */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;

		/* calculate edges */
		const CFloat right  = (x + width);
		const CFloat bottom = (y + height);

		/* perform standard startup procedures for new figure additions */
		_BeginNew(_this, 4, p, t, x, y);

		/* add the remaining rectangle sides */
		_LineTo(p, t, right, y);
		_LineTo(p, t, right, bottom);
		_LineToClose(p, t, x, bottom);
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a sequence of rectangles to this path. */
CStatus
CPath_AddRectangles(CPath       *_this,
                    CRectangleF *rects,
                    CUInt32      count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a rectangle list pointer */
	CStatus_Require((rects != 0), CStatus_ArgumentNull);

	/* bail out now if there's nothing to add */
	CStatus_Require((count != 0), CStatus_OK);

	/* add the rectangles */
	{
		/* declarations */
		CPointF *p;
		CByte   *t;
		CFloat   left;
		CFloat   top;
		CFloat   right;
		CFloat   bottom;

		/* get the end of input pointer */
		const CRectangleF *const end = (rects + count);

		/* get the rectangle edges */
		left   = CRectangle_X(*rects);
		top    = CRectangle_Y(*rects);
		right  = left + CRectangle_Width(*rects);
		bottom = top + CRectangle_Height(*rects++);

		/* perform standard startup procedures for new figure additions */
		_BeginNew
			(_this, (count * 4),
			 p,     t,
			 left,  top);

		/* complete the first rectangle */
		_LineTo(p, t, right, top);
		_LineTo(p, t, right, bottom);
		_LineToClose(p, t, left, bottom);

		/* add the remaining rectangles */
		while(rects != end)
		{
			/* get the rectangle edges */
			left   = CRectangle_X(*rects);
			top    = CRectangle_Y(*rects);
			right  = left + CRectangle_Width(*rects);
			bottom = top + CRectangle_Height(*rects++);

			/* add the rectangle */
			_MoveTo(p, t, left, top);
			_LineTo(p, t, right, top);
			_LineTo(p, t, right, bottom);
			_LineToClose(p, t, left, bottom);
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Add a string to this path. */
CStatus
CPath_AddString(CPath         *_this,
                CChar16       *s,
                CUInt32        length,
                CFontFamily   *family,
                CFontStyle     style,
                CFloat         emSize,
                CRectangleF    layoutRect,
                CStringFormat *format)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Clear all markers from this path. */
CStatus
CPath_ClearMarkers(CPath *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* clear markers */
	{
		/* declarations */
		CByte *curr;

		/* get the end pointer */
		const CByte *const end = (_this->types + _this->count);

		/* clear the markers */
		for(curr = _this->types; curr != end; ++curr)
		{
			*curr &= ~CPathType_PathMarker;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Clone this path. */
CStatus
CPath_Clone(CPath  *_this,
            CPath **clone)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a clone pointer pointer */
	CStatus_Require((clone != 0), CStatus_ArgumentNull);

	/* create the clone path */
	CStatus_Check(CPath_Create(clone));

	/* clone the members */
	{
		/* declarations */
		CPointF *tmpP;
		CByte   *tmpT;

		/* get the count and capacity */
		const CUInt32 count    = _this->count;
		const CUInt32 capacity = ((count + 31) & ~31);

		/* allocate the clone points list */
		if(!(tmpP = (CPointF *)CMalloc(capacity * sizeof(CPointF))))
		{
			CPath_Destroy(clone);
			return CStatus_OutOfMemory;
		}

		/* allocate the clone types list */
		if(!(tmpT = (CByte *)CMalloc(capacity * sizeof(CByte))))
		{
			CFree(tmpP);
			CPath_Destroy(clone);
			return CStatus_OutOfMemory;
		}

		/* deep copy the points into the clone */
		CMemCopy(tmpP, _this->points, (count * sizeof(CPointF)));

		/* deep copy the types into the clone */
		CMemCopy(tmpT, _this->types, (count * sizeof(CByte)));

		/* set the clone members */
		(*clone)->capacity  = capacity;
		(*clone)->count     = count;
		(*clone)->points    = tmpP;
		(*clone)->types     = tmpT;
		(*clone)->winding   = _this->winding;
		(*clone)->newFigure = _this->newFigure;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Close all open figures in this path and start a new one. */
CStatus
CPath_CloseAllFigures(CPath *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* close the figures */
	{
		/* declarations */
		CByte *curr, *prev;

		/* get the end pointer */
		const CByte *const end = (_this->types + _this->count);

		/* bail out now if there's nothing to close */
		CStatus_Require((_this->count <= 1), CStatus_OK);

		/* get the current pointer */
		curr = (_this->types + 2);

		/* get the previous pointer */
		prev = (_this->types + 1);

		/* close the figures */
		while(curr != end)
		{
			/* close the previous figure if we're on a new figure */
			if(*curr == CPathType_Start)
			{
				*prev |= CPathType_CloseSubpath;
			}

			/* advance to next position */
			prev = curr++;
		}

		/* close the current figure */
		*prev |= CPathType_CloseSubpath;

		/* start a new figure */
		_this->newFigure = 1;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Close the current figure in this path and start a new one. */
CStatus
CPath_CloseFigure(CPath *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* close the current figure */
	{
		/* bail out now if there's nothing to close */
		CStatus_Require((_this->count <= 1), CStatus_OK);

		/* close the current figure */
		_this->types[_this->count - 1] |= CPathType_CloseSubpath;

		/* start a new figure */
		_this->newFigure = 1;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Flatten curves in this path into sequences of connected line segments. */
CStatus
CPath_Flatten(CPath   *_this,
              CMatrix *matrix,
              CFloat   flatness)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* bail out now if there's nothing to flatten */
	CStatus_Require((_this->count == 0), CStatus_OK);

	/* transform the path, as needed */
	if(matrix != 0)
	{
		/* transform the points */
		CStatus_Check
			(CMatrix_TransformPoints
				(matrix, _this->points, _this->count));
	}

	/* bail out now if there's nothing more to do */
	CStatus_Require((_this->hasCurves != 0), CStatus_OK);

	/* flatten the path */
	{
		/* declarations */
		CFlattener  flattener;
		CPointF    *points;
		CByte      *types;
		CUInt32     count;
		CUInt32     capacity;
		CStatus     status;

		/* initialize the flattener */
		CFlattener_Initialize(&flattener);

		/* flatten the path */
		status =
			CFlattener_Flatten
				(&flattener, _this->points, _this->types, _this->count,
				 flatness);

		/* handle flattening failures */
		if(status != CStatus_OK)
		{
			/* finalize the flattener */
			CFlattener_Finalize(&flattener, 0, 0, 0, 0);

			/* return status */
			return status;
		}

		/* finalize the flattener */
		CFlattener_Finalize(&flattener, &points, &types, &count, &capacity);

		/* finalize the current path members */
		CFree(_this->points);
		CFree(_this->types);

		/* initialize the members */
		_this->points   = points;
		_this->types    = types;
		_this->count    = count;
		_this->capacity = capacity;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the point count of this path. */
CStatus
CPath_GetCount(CPath   *_this,
               CUInt32 *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the count */
	*count = _this->count;

	/* return successfully */
	return CStatus_OK;
}

/* Get the bounds of this path. */
CStatus
CPath_GetBounds(CPath       *_this,
                CMatrix     *matrix,
                CPen        *pen,
                CRectangleF *bounds)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Get the last point in this path. */
CStatus
CPath_GetLastPoint(CPath   *_this,
                   CPointF *point)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the last point */
	{
		/* ensure we have a point */
		CStatus_Require((_this->count > 0), CStatus_Argument);

		/* get the last point */
		*point = _this->points[_this->count - 1];
	}

	/* return successfully */
	return CStatus_OK;
}

/* Determine if a point is visible within an outline of this path. */
CStatus
CPath_IsOutlineVisible(CPath     *_this,
                       CFloat     x,
                       CFloat     y,
                       CPen      *pen,
                       CGraphics *graphics,
                       CBool     *visible)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Determine if a point is visible within this path. */
CStatus
CPath_IsVisible(CPath     *_this,
                CFloat     x,
                CFloat     y,
                CGraphics *graphics,
                CBool     *visible)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Reset this path. */
CStatus
CPath_Reset(CPath *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* reset this path */
	{
		/* reset the members */
		_this->count     = 0;
		_this->winding   = 0;
		_this->newFigure = 1;
		_this->hasCurves = 0;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Reverse the order of the points in this path. */
CStatus
CPath_Reverse(CPath *_this)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Set a marker at the current position in this path. */
CStatus
CPath_SetMarker(CPath *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the marker */
	{
		/* ensure we have a point at which to set the marker */
		CStatus_Require((_this->count > 0), CStatus_Argument);

		/* set the marker */
		_this->types[_this->count - 1] |= CPathType_PathMarker;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Start a new figure in this path without closing the current one. */
CStatus
CPath_StartFigure(CPath *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* start the new figure */
	_this->newFigure = 1;

	/* return successfully */
	return CStatus_OK;
}

/* Transform the points of this path by a matrix. */
CStatus
CPath_Transform(CPath   *_this,
                CMatrix *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a matrix pointer */
	CStatus_Require((matrix != 0), CStatus_ArgumentNull);

	/* transform the points */
	{
		/* bail out now if there's nothing to transform */
		CStatus_Require((_this->count == 0), CStatus_OK);

		/* transform the points */
		CStatus_Check
			(CMatrix_TransformPoints
				(matrix, _this->points, _this->count));
	}

	/* return successfully */
	return CStatus_OK;
}

/* Warp the points of this path. */
CStatus
CPath_Warp(CPath     *_this,
           CMatrix   *matrix,
           CPointF   *dstPoints,
           CUInt32    dstLength,
           CFloat     srcX,
           CFloat     srcY,
           CFloat     srcWidth,
           CFloat     srcHeight,
           CWarpMode  warpMode,
           CFloat     flatness)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Widen this path. */
CStatus
CPath_Widen(CPath   *_this,
            CPen    *pen,
            CMatrix *matrix,
            CFloat   flatness)
{
	/* TODO */
	return CStatus_NotImplemented;
}

/* Transform the points of this path by an affine transformation. */
CINTERNAL void
CPath_TransformAffine(CPath             *_this,
                      CAffineTransformF *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* transform the points, as needed */
	if(_this->count != 0)
	{
		CAffineTransformF_TransformPoints
			(transform, _this->points, _this->count);
	}
}

/* Stroke this path to another path. */
CINTERNAL CStatus
CPath_Stroke(CPath    *_this,
             CPath    *stroke,
             CStroker *stroker)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((stroke  != 0));
	CASSERT((stroker != 0));

	/* stroke the path */
	CStatus_Check
		(CStroker_Stroke
			(stroker, stroke, _this->points, _this->types, _this->count,
			 _this->hasCurves));

	/* return successfully */
	return CStatus_OK;
}

/* Fill this path to trapezoids. */
CINTERNAL CStatus
CPath_Fill(CPath       *_this,
           CTrapezoids *trapezoids)
{
	/* declarations */
	CFillMode fillMode;

	/* assertions */
	CASSERT((_this      != 0));
	CASSERT((trapezoids != 0));

	/* get the fill mode */
	fillMode = (_this->winding ? CFillMode_Winding : CFillMode_Alternate);

	/* fill the path */
	CStatus_Check
		(CTrapezoids_Fill
			(trapezoids, _this->points, _this->types, _this->count, fillMode));

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
