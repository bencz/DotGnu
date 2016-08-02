/*
 * CStroker.c - Stroker implementation.
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

#include "CStroker.h"
#include "CAffineTransform.h"
#include "CBezier.h"
#include "CFlattener.h"
#include "CMath.h"
#include "CPen.h"
#include "CPath.h"
#include "CPointArray.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CStrokeJoiner CStrokeJoiner_Zero;
static const CStrokeCapper CStrokeCapper_Zero;

#define CStrokeJoiner_Join(j, p, cX, cY, pC, pS, cC, cS) \
	((j)->Join((j), (p), (cX), (cY), (pC), (pS), (cC), (cS)))
#define CStrokeCapper_Cap(c, p, cX, cY, sX, sY) \
	((c)->Cap((c), (p), (cX), (cY), (sX), (sY)))

#define CStroker_CirclePoints(array, radius, transform, scale)                 \
	do {                                                                       \
		/* declarations */                                                     \
		CBezierF bezier;                                                       \
		CPointF  points[12];                                                   \
		                                                                       \
		/* calculate the width and radius */                                   \
		const CDouble r = (radius);                                            \
		const CDouble w = (r * 2.0f);                                          \
		                                                                       \
		/* calculate the distance along the tangents */                        \
		const CDouble d = (r * CMath_Arc90Fraction);                           \
		                                                                       \
		/* calculate the tangential control points */                          \
		const CFloat p = (r + d);                                              \
		const CFloat m = (r - d);                                              \
		                                                                       \
		/* initialize the points */                                            \
		CPoint_X(points[0])  = w; CPoint_Y(points[0])  = r;                    \
		CPoint_X(points[1])  = w; CPoint_Y(points[1])  = m;                    \
		CPoint_X(points[2])  = p; CPoint_Y(points[2])  = 0;                    \
		CPoint_X(points[3])  = r; CPoint_Y(points[3])  = 0;                    \
		CPoint_X(points[4])  = m; CPoint_Y(points[4])  = 0;                    \
		CPoint_X(points[5])  = 0; CPoint_Y(points[5])  = m;                    \
		CPoint_X(points[6])  = 0; CPoint_Y(points[6])  = r;                    \
		CPoint_X(points[7])  = 0; CPoint_Y(points[7])  = p;                    \
		CPoint_X(points[8])  = m; CPoint_Y(points[8])  = w;                    \
		CPoint_X(points[9])  = r; CPoint_Y(points[9])  = w;                    \
		CPoint_X(points[10]) = p; CPoint_Y(points[10]) = w;                    \
		CPoint_X(points[11]) = w; CPoint_Y(points[11]) = p;                    \
		                                                                       \
		/* transform or scale the points */                                    \
		if((transform) != 0)                                                   \
		{                                                                      \
			CAffineTransformF_TransformPoints((transform), points, 12);        \
		}                                                                      \
		else                                                                   \
		{                                                                      \
			CVectorF_ScalePoints((scale), points, 12);                         \
		}                                                                      \
		                                                                       \
		/* reset the count */                                                  \
		CPointArray_Count(*(array)) = 0;                                       \
		                                                                       \
		/* initialize the first quadrant */                                    \
		CBezierF_Initialize                                                    \
			(&bezier, &points[0], &points[1], &points[2], &points[3]);         \
		                                                                       \
		/* flatten the first quadrant */                                       \
		CStatus_Check                                                          \
			(CBezierF_Flatten                                                  \
				(&bezier, (array), CFiller_TOLERANCE));                        \
		                                                                       \
		/* initialize the second quadrant */                                   \
		CBezierF_Initialize                                                    \
			(&bezier, &points[3], &points[4], &points[5], &points[6]);         \
		                                                                       \
		/* flatten the second quadrant */                                      \
		CStatus_Check                                                          \
			(CBezierF_Flatten                                                  \
				(&bezier, (array), CFiller_TOLERANCE));                        \
		                                                                       \
		/* initialize the third quadrant */                                    \
		CBezierF_Initialize                                                    \
			(&bezier, &points[6], &points[7], &points[8], &points[9]);         \
		                                                                       \
		/* flatten the third quadrant */                                       \
		CStatus_Check                                                          \
			(CBezierF_Flatten                                                  \
				(&bezier, (array), CFiller_TOLERANCE));                        \
		                                                                       \
		/* initialize the fourth quadrant */                                   \
		CBezierF_Initialize                                                    \
			(&bezier, &points[9], &points[10], &points[11], &points[0]);       \
		                                                                       \
		/* flatten the fourth quadrant */                                      \
		CStatus_Check                                                          \
			(CBezierF_Flatten                                                  \
				(&bezier, (array), CFiller_TOLERANCE));                        \
	} while(0)

#define CStroker_TempSpacePoints(array, points, count, size)                   \
	do {                                                                       \
		/* declarations */                                                     \
		CPointF *tmp;                                                          \
		                                                                       \
		/* get the points */                                                   \
		tmp = CPointArray_Points(*(array));                                    \
		                                                                       \
		/* get the count */                                                    \
		(count) = CPointArray_Count(*(array));                                 \
		                                                                       \
		/* calculate the size */                                               \
		(size) = ((count) * sizeof(CPointF));                                  \
		                                                                       \
		/* allocate the points */                                              \
		if(!((points) = (CPointF *)CMalloc((size) << 1)))                      \
		{                                                                      \
			return CStatus_OutOfMemory;                                        \
		}                                                                      \
		                                                                       \
		/* copy the points */                                                  \
		CMemCopy((points), tmp, (size));                                       \
	} while(0)

static CStatus
CStroker_StrokeSubpaths(CStroker *_this,
                        CPath    *path,
                        CPointF  *points,
                        CByte    *types,
                        CUInt32   count)
{
	/* declarations */
	CByte   *type;
	CPointF *curr;
	CPointF *end;

	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((path      != 0));
	CASSERT((points    != 0));
	CASSERT((types     != 0));

	/* get the current type pointer */
	type = types;

	/* get the current point pointer */
	curr = points;

	/* get the end pointer */
	end = (curr + count);

	/* reset the count of the array */
	CPointArray_Count(_this->array) = 0;

	/* stroke the subpaths */
	while(curr != end)
	{
		/* declarations */
		CPointF *first;

		/* get the first point */
		first = curr;

		/* get the current subpath */
		while(curr != end && ((*type & CPathType_Start) == 0))
		{
			/* add the current point */
			CStatus_Check
				(CPointArrayF_AppendPointNoRepeat
					(&(_this->array), curr));

			/* close, as needed */
			if((*type & CPathType_CloseSubpath) != 0)
			{
				CStatus_Check
					(CPointArrayF_AppendPointNoRepeat
						(&(_this->array), first));
			}

			/* move to the next position */
			++type; ++curr;
		}

		/* stroke the subpath, as needed */
		if(CPointArray_Count(_this->array) != 0)
		{
			/* stroke the subpath */
			CStatus_Check
				(_this->Stroke
					(_this, path, CPointArray_Points(_this->array),
					 CPointArray_Count(_this->array)));

			/* reset for next subpath */
			CPointArray_Count(_this->array) = 0;
		}
	}

	/* apply device transformation */
	CPath_TransformAffine(path, &(_this->dev));

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStroker_FullStroke(CStroker *_this,
                    CPath    *path,
                    CPointF  *points,
                    CUInt32   count)
{
	/* declarations */
	CAffineTransformF *transform;
	CPointF           *end;
	CPointF           *curr;
	CPointF           *next;
	CPointF           *last;
	CBool              needCap;
	CFloat             prevC;
	CFloat             prevS;
	CFloat             radius;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((path   != 0));
	CASSERT((points != 0));

	/* get the end of points pointer */
	end  = (points + count);

	/* get the current point pointer */
	curr = points;

	/* get the next point pointer */
	next = (curr + 1);

	/* get the last point pointer */
	last = (end - 1);

	/* flag that we need a start cap */
	needCap = 1;

	/* get the pen transformation */
	transform = &(_this->pen);

	/* get the pen radius */
	radius = _this->radius;

	/* set the previous cosine and sine to the default */
	prevC = 0;
	prevS = 0;

	/* generate the stroke */
	while(next != end)
	{
		/* declarations */
		CFloat  currX;
		CFloat  currY;
		CFloat  nextX;
		CFloat  nextY;
		CFloat  slopeX;
		CFloat  slopeY;
		CFloat  length;
		CPointF delta[4];

		/* get the current and next coordinates */
		currX = CPoint_X(*curr);
		currY = CPoint_Y(*curr);
		nextX = CPoint_X(*next);
		nextY = CPoint_Y(*next);

		/* calculate the line slope */
		slopeX = (nextX - currX);
		slopeY = (nextY - currY);

		/* calculate the line length */
		length =
			(CFloat)CMath_Sqrt
				((slopeX * slopeX) + (slopeY * slopeY));

		/* stroke non-degenerate lines */
		if(length != 0.0f)
		{
			/* get the current sine and cosine */
			slopeX = slopeX / length;
			slopeY = slopeY / length;

			/* add a join or start cap, as needed */
			if(needCap)
			{
				/* add the start cap, as needed */
				if((_this->startCapper.Cap) != 0)
				{
					CStatus_Check
						(CStrokeCapper_Cap
							(&(_this->startCapper), path, &currX, &currY,
							 slopeX, slopeY));
				}

				/* reset the need start cap flag */
				needCap = 0;
			}
			else
			{
				/* add the join */
				CStatus_Check
					(CStrokeJoiner_Join
						(&(_this->joiner), path, currX, currY,
						 prevC, prevS, slopeX, slopeY));
			}

			/* add the end cap, as needed */
			if(next == last && ((_this->endCapper.Cap) != 0))
			{
				CStatus_Check
					(CStrokeCapper_Cap
						(&(_this->endCapper), path, &nextX, &nextY,
						 -slopeX, -slopeY));
			}

			/* calculate the stroke bounds */
			CPoint_X(delta[0]) =  slopeY * radius;
			CPoint_Y(delta[0]) = -slopeX * radius;
			CPoint_X(delta[1]) = -slopeY * radius;
			CPoint_Y(delta[1]) =  slopeX * radius;
			CPoint_X(delta[2]) =  CPoint_X(delta[1]);
			CPoint_Y(delta[2]) =  CPoint_Y(delta[1]);
			CPoint_X(delta[3]) =  CPoint_X(delta[0]);
			CPoint_Y(delta[3]) =  CPoint_Y(delta[0]);

			/* transform by the pen transformation */
			CAffineTransformF_TransformPoints(transform, delta, 4);

			/* translate the stroke into place */
			CPoint_X(delta[0]) += currX;
			CPoint_Y(delta[0]) += currY;
			CPoint_X(delta[1]) += currX;
			CPoint_Y(delta[1]) += currY;
			CPoint_X(delta[2]) += nextX;
			CPoint_Y(delta[2]) += nextY;
			CPoint_X(delta[3]) += nextX;
			CPoint_Y(delta[3]) += nextY;

			/* add the stroke to the path */
			CStatus_Check
				(CPath_AddPolygon
					(path, delta, 4));

			/* update the points */
			curr = next++;

			/* update the previous slope */
			prevC = slopeX;
			prevS = slopeY;
		}
	}

	/* handle degenerate subpaths */
	if(needCap)
	{
		/* add single point caps */
		{
			/* add a start cap, as needed */
			if((_this->startCapper.Cap) != 0)
			{
				/* get the current x coordinate */
				CFloat x = CPoint_X(*curr);

				/* get the current y coordinate */
				CFloat y = CPoint_Y(*curr);

				/* add the start cap */
				CStatus_Check
					(CStrokeCapper_Cap
						(&(_this->startCapper), path, &x, &y, 1.0f, 0.0f));
			}

			/* add an end cap, as needed */
			if((_this->endCapper.Cap) != 0)
			{
				/* get the current x coordinate */
				CFloat x = CPoint_X(*curr);

				/* get the current y coordinate */
				CFloat y = CPoint_Y(*curr);

				/* add the end cap */
				CStatus_Check
					(CStrokeCapper_Cap
						(&(_this->endCapper), path, &x, &y, -1.0f, 0.0f));
			}
		}
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStroker_FastStroke(CStroker *_this,
                    CPath    *path,
                    CPointF  *points,
                    CUInt32   count)
{
	/* declarations */
	CPointF           *end;
	CPointF           *curr;
	CPointF           *next;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((path   != 0));
	CASSERT((points != 0));

	/* get the end of points pointer */
	end  = (points + count);

	/* get the current point pointer */
	curr = points;

	/* get the next point pointer */
	next = (curr + 1);

	/* generate the stroke */
	while(next != end)
	{
		/* declarations */
		CFloat  currX;
		CFloat  currY;
		CFloat  nextX;
		CFloat  nextY;
		CFloat  slopeX;
		CFloat  slopeY;
		CFloat  length;
		CPointF delta[4];

		/* get the current and next coordinates */
		currX = CPoint_X(*curr);
		currY = CPoint_Y(*curr);
		nextX = CPoint_X(*next);
		nextY = CPoint_Y(*next);

		/* calculate the line slope */
		slopeX = (nextX - currX);
		slopeY = (nextY - currY);

		/* calculate the line length */
		length =
			(CFloat)CMath_Sqrt
				((slopeX * slopeX) + (slopeY * slopeY));

		/* stroke non-degenerate lines */
		if(length != 0.0f)
		{
			/* get the current sine and cosine */
			slopeX = slopeX / length;
			slopeY = slopeY / length;

			/* calculate the stroke bounds */
			CPoint_X(delta[0]) =  slopeY * 0.5f;
			CPoint_Y(delta[0]) = -slopeX * 0.5f;
			CPoint_X(delta[1]) = -slopeY * 0.5f;
			CPoint_Y(delta[1]) =  slopeX * 0.5f;
			CPoint_X(delta[2]) =  CPoint_X(delta[1]);
			CPoint_Y(delta[2]) =  CPoint_Y(delta[1]);
			CPoint_X(delta[3]) =  CPoint_X(delta[0]);
			CPoint_Y(delta[3]) =  CPoint_Y(delta[0]);

			/* translate the stroke into place */
			CPoint_X(delta[0]) += currX;
			CPoint_Y(delta[0]) += currY;
			CPoint_X(delta[1]) += currX;
			CPoint_Y(delta[1]) += currY;
			CPoint_X(delta[2]) += nextX;
			CPoint_Y(delta[2]) += nextY;
			CPoint_X(delta[3]) += nextX;
			CPoint_Y(delta[3]) += nextY;

			/* add the stroke to the path */
			CStatus_Check
				(CPath_AddPolygon
					(path, delta, 4));

			/* update the points */
			curr = next++;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

static CMATH CPointF
CStrokeJoiner_MiterIntersect(CPointF a,
                             CPointF b,
                             CFloat  prevC,
                             CFloat  prevS,
                             CFloat  currC,
                             CFloat  currS,
                             CDouble cross)
{
	/* declarations */
	CPointF intersect;
	CDouble iX;
	CDouble iY;

	/* get the point components */
	const CFloat aX = CPoint_X(a);
	const CFloat aY = CPoint_Y(a);
	const CFloat bX = CPoint_X(b);
	const CFloat bY = CPoint_Y(b);

	/* calculate the product of the previous sine and current cosine */
	const CDouble pScC = (prevS * currC);

	/* calculate the product of the previous cosine and current sine */
	const CDouble pCcS = (prevC * currS);

	/* calculate the product of the previous sine and current sine */
	const CDouble pScS = (prevS * currS);

	/* calculate the vertical component of the intersection vector */
	iY  = ((((bX - aX) * pScS) + (aY * pCcS) - (bY * pScC)) / cross);

	/* calculate the horizontal component of the intersection vector */
	iX = (CMath_Abs(prevS) >= CMath_Abs(currS)) ?
	     ((((iY - aY) * prevC) / prevS) + aX) :
	     ((((iY - bY) * currC) / currS) + bX);

	/* set the components of the intersection vector */
	CPoint_X(intersect) = (CFloat)iX;
	CPoint_Y(intersect) = (CFloat)iY;

	/* return the intersection vector */
	return intersect;
}

static CStatus
CStrokeJoiner_AddMiter(CStrokeJoiner *_this,
                       CPath         *path,
                       CFloat         centerX,
                       CFloat         centerY,
                       CFloat         prevC,
                       CFloat         prevS,
                       CFloat         currC,
                       CFloat         currS)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((path  != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require(((currC != prevC) || (currS != prevS)), CStatus_OK);

	/* add the miter */
	{
		/* declarations */
		CVectorF center;

		/* calculate the dot and cross products */
		const CDouble dot   = CMath_DotProduct(prevC, prevS, currC, currS);
		const CDouble cross = CMath_CrossProduct(prevC, prevS, currC, currS);

		/* get the center point */
		CVector_X(center) = centerX;
		CVector_Y(center) = centerY;

		/* add the join based on the limit and angle */
		if((_this->u.other.limitSquared * (1 + dot)) >= 2)
		{
			/* declarations */
			CPointF delta[4];

			/* set the first point */
			CPoint_X(delta[0]) = 0.0f;
			CPoint_Y(delta[0]) = 0.0f;

			/* set the second and fourth points based on angle */
			if(cross < 0.0f)
			{
				CPoint_X(delta[1]) =  prevS;
				CPoint_Y(delta[1]) = -prevC;
				CPoint_X(delta[3]) =  currS;
				CPoint_Y(delta[3]) = -currC;
			}
			else
			{
				CPoint_X(delta[1]) = -prevS;
				CPoint_Y(delta[1]) =  prevC;
				CPoint_X(delta[3]) = -currS;
				CPoint_Y(delta[3]) =  currC;
			}

			/* calculate the intersection */
			delta[2] =
				CStrokeJoiner_MiterIntersect
					(delta[1], delta[3], prevC, prevS, currC, currS, cross);

			/* scale the join to the stroke size */
			CVectorF_ScalePoints(&(_this->u.other.scale), delta, 4);

			/* transform the join */
			CAffineTransformF_TransformPoints
				(_this->u.other.transform, delta, 4);

			/* translate the join into place */
			CVectorF_TranslatePoints(&center, delta, 4);

			/* add the join to the path */
			CStatus_Check
				(CPath_AddPolygon
					(path, delta, 4));
		}
		else
		{
			/* declarations */
			CPointF delta[3];

			/* set the first point */
			CPoint_X(delta[0]) = 0.0f;
			CPoint_Y(delta[0]) = 0.0f;

			/* set the second and third points based on angle */
			if(cross < 0.0f)
			{
				CPoint_X(delta[1]) =  prevS;
				CPoint_Y(delta[1]) = -prevC;
				CPoint_X(delta[2]) =  currS;
				CPoint_Y(delta[2]) = -currC;
			}
			else
			{
				CPoint_X(delta[1]) = -prevS;
				CPoint_Y(delta[1]) =  prevC;
				CPoint_X(delta[2]) = -currS;
				CPoint_Y(delta[2]) =  currC;
			}

			/* scale the join to the stroke size */
			CVectorF_ScalePoints(&(_this->u.other.scale), delta, 3);

			/* transform the join */
			CAffineTransformF_TransformPoints
				(_this->u.other.transform, delta, 3);

			/* translate the join into place */
			CVectorF_TranslatePoints(&center, delta, 3);

			/* add the join to the path */
			CStatus_Check
				(CPath_AddPolygon
					(path, delta, 3));
		}
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeJoiner_AddRound(CStrokeJoiner *_this,
                       CPath         *path,
                       CFloat         centerX,
                       CFloat         centerY,
                       CFloat         prevC,
                       CFloat         prevS,
                       CFloat         currC,
                       CFloat         currS)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((path  != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require(((currC != prevC) || (currS != prevS)), CStatus_OK);

	/* add the circle */
	{
		/* declarations */
		CPointF  *points;
		CPointF  *tmp;
		CVectorF  center;

		/* get the center point */
		CVector_X(center) = centerX;
		CVector_X(center) = centerY;

		/* get the points */
		points = _this->u.round.points;

		/* get the temporary space */
		tmp = (points + _this->u.round.count);

		/* copy the points into temporary space */
		CMemCopy(tmp, points, _this->u.round.size);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, tmp, _this->u.round.count);

		/* add the cap to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, tmp, _this->u.round.count));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeJoiner_AddBevel(CStrokeJoiner *_this,
                       CPath         *path,
                       CFloat         centerX,
                       CFloat         centerY,
                       CFloat         prevC,
                       CFloat         prevS,
                       CFloat         currC,
                       CFloat         currS)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((path  != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require(((currC != prevC) || (currS != prevS)), CStatus_OK);

	/* add the bevel */
	{
		/* declarations */
		CVectorF center;
		CPointF  delta[3];

		/* calculate the cross product */
		const CDouble cross = CMath_CrossProduct(prevC, prevS, currC, currS);

		/* get the center point */
		CVector_X(center) = centerX;
		CVector_Y(center) = centerY;

		/* set the first point */
		CPoint_X(delta[0]) = 0.0f;
		CPoint_Y(delta[0]) = 0.0f;

		/* set the second and third points based on angle */
		if(cross < 0.0f)
		{
			CPoint_X(delta[1]) =  prevS;
			CPoint_Y(delta[1]) = -prevC;
			CPoint_X(delta[2]) =  currS;
			CPoint_Y(delta[2]) = -currC;
		}
		else
		{
			CPoint_X(delta[1]) = -prevS;
			CPoint_Y(delta[1]) =  prevC;
			CPoint_X(delta[2]) = -currS;
			CPoint_Y(delta[2]) =  currC;
		}

		/* scale the join to the stroke size */
		CVectorF_ScalePoints(&(_this->u.other.scale), delta, 3);

		/* transform the join */
		CAffineTransformF_TransformPoints
			(_this->u.other.transform, delta, 3);

		/* translate the join into place */
		CVectorF_TranslatePoints(&center, delta, 3);

		/* add the join to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, delta, 3));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeCapper_AddSquare(CStrokeCapper *_this,
                        CPath         *path,
                        CFloat        *centerX,
                        CFloat        *centerY,
                        CFloat         slopeX,
                        CFloat         slopeY)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((centerX != 0));
	CASSERT((centerY != 0));

	/* add the square */
	{
		/* declarations */
		CPointF  delta[4];
		CVectorF center;
		CFloat   dX;
		CFloat   dY;

		/* get the center point */
		CVector_X(center) = *centerX;
		CVector_Y(center) = *centerY;

		/* calculate the bounds components */
		dX = (slopeX * _this->u.other.radius);
		dY = (slopeY * _this->u.other.radius);

		/* calculate the cap bounds */
		CPoint_X(delta[0]) =  (dY + dX);
		CPoint_Y(delta[0]) =  (dY - dX);
		CPoint_X(delta[1]) =  (dY);
		CPoint_Y(delta[1]) = -(dX);
		CPoint_X(delta[2]) = -(dY);
		CPoint_Y(delta[2]) =  (dX);
		CPoint_X(delta[3]) = -(CPoint_Y(delta[0]));
		CPoint_Y(delta[3]) =  (CPoint_X(delta[0]));

		/* transform the cap */
		CAffineTransformF_TransformPoints
			(_this->u.other.u.transform, delta, 4);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, delta, 4);

		/* add the stroke to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, delta, 4));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeCapper_AddSquareAnchor(CStrokeCapper *_this,
                              CPath         *path,
                              CFloat        *centerX,
                              CFloat        *centerY,
                              CFloat         slopeX,
                              CFloat         slopeY)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((centerX != 0));
	CASSERT((centerY != 0));

	/* add the square */
	{
		/* declarations */
		CPointF  delta[4];
		CVectorF center;
		CFloat   dX;
		CFloat   dY;

		/* get the center point */
		CVector_X(center) = *centerX;
		CVector_Y(center) = *centerY;

		/* calculate the bounds components */
		dX = (slopeX * _this->u.other.radius);
		dY = (slopeY * _this->u.other.radius);

		/* calculate the cap bounds */
		CPoint_X(delta[0]) =  (dY + dX);
		CPoint_Y(delta[0]) =  (dY - dX);
		CPoint_X(delta[1]) =  CPoint_Y(delta[0]);
		CPoint_Y(delta[1]) = -CPoint_X(delta[0]);
		CPoint_X(delta[2]) = -CPoint_X(delta[0]);
		CPoint_Y(delta[2]) = -CPoint_Y(delta[0]);
		CPoint_X(delta[3]) = -CPoint_Y(delta[0]);
		CPoint_Y(delta[3]) =  CPoint_X(delta[0]);

		/* transform the cap */
		CVectorF_ScalePoints(_this->u.other.u.scale, delta, 4);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, delta, 4);

		/* add the stroke to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, delta, 4));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeCapper_AddTriangle(CStrokeCapper *_this,
                          CPath         *path,
                          CFloat        *centerX,
                          CFloat        *centerY,
                          CFloat         slopeX,
                          CFloat         slopeY)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((centerX != 0));
	CASSERT((centerY != 0));

	/* add the triangle */
	{
		/* declarations */
		CPointF  delta[3];
		CVectorF center;
		CFloat   dX;
		CFloat   dY;

		/* get the center point */
		CVector_X(center) = *centerX;
		CVector_Y(center) = *centerY;

		/* calculate the bounds components */
		dX = (slopeX * _this->u.other.radius);
		dY = (slopeY * _this->u.other.radius);

		/* calculate the cap bounds */
		CPoint_X(delta[0]) =  dY;
		CPoint_Y(delta[0]) = -dX;
		CPoint_X(delta[1]) =  dX;
		CPoint_Y(delta[1]) =  dY;
		CPoint_X(delta[2]) = -dY;
		CPoint_Y(delta[2]) =  dX;

		/* transform the cap */
		CAffineTransformF_TransformPoints
			(_this->u.other.u.transform, delta, 3);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, delta, 3);

		/* add the cap to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, delta, 3));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeCapper_AddDiamondAnchor(CStrokeCapper *_this,
                               CPath         *path,
                               CFloat        *centerX,
                               CFloat        *centerY,
                               CFloat         slopeX,
                               CFloat         slopeY)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((centerX != 0));
	CASSERT((centerY != 0));

	/* add the diamond */
	{
		/* declarations */
		CPointF  delta[4];
		CVectorF center;
		CFloat   dX;
		CFloat   dY;

		/* get the center point */
		CVector_X(center) = *centerX;
		CVector_Y(center) = *centerY;

		/* calculate the bounds components */
		dX = (slopeX * _this->u.other.radius);
		dY = (slopeY * _this->u.other.radius);

		/* calculate the cap bounds */
		CPoint_X(delta[0]) =  dY;
		CPoint_Y(delta[0]) = -dX;
		CPoint_X(delta[1]) =  dX;
		CPoint_Y(delta[1]) =  dY;
		CPoint_X(delta[2]) = -dY;
		CPoint_Y(delta[2]) =  dX;
		CPoint_X(delta[3]) = -dX;
		CPoint_Y(delta[3]) = -dY;

		/* scale the cap */
		CVectorF_ScalePoints(_this->u.other.u.scale, delta, 4);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, delta, 4);

		/* add the cap to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, delta, 4));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeCapper_AddRound(CStrokeCapper *_this,
                       CPath         *path,
                       CFloat        *centerX,
                       CFloat        *centerY,
                       CFloat         slopeX,
                       CFloat         slopeY)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((centerX != 0));
	CASSERT((centerY != 0));

	/* add the circle */
	{
		/* declarations */
		CPointF  *points;
		CPointF  *tmp;
		CVectorF  center;

		/* get the center point */
		CVector_X(center) = *centerX;
		CVector_X(center) = *centerY;

		/* get the points */
		points = _this->u.round.points;

		/* get the temporary space */
		tmp = (points + _this->u.round.count);

		/* copy the points into temporary space */
		CMemCopy(tmp, points, _this->u.round.size);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, tmp, _this->u.round.count);

		/* add the cap to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, tmp, _this->u.round.count));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeCapper_AddArrowAnchor(CStrokeCapper *_this,
                             CPath         *path,
                             CFloat        *centerX,
                             CFloat        *centerY,
                             CFloat         slopeX,
                             CFloat         slopeY)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((path    != 0));
	CASSERT((centerX != 0));
	CASSERT((centerY != 0));

	/* add the arrow */
	{
		/* declarations */
		CPointF  delta[3];
		CVectorF center;
		CFloat   dX;
		CFloat   dY;
		CFloat   oX;
		CFloat   oY;

		/*\
		|*| NOTE: the arrow cap is essentially two 30-60 right triangles, with
		|*|       the visible outer angled edges being at 30/210 degrees from
		|*|       the center line, so, if 'offset' is the length from the
		|*|       given center point to the back of the arrow head:
		|*|
		|*|         given that:
		|*|           sin(30)    = 0.5
		|*|           sin(t)     = opposite / hypotenuse
		|*|           cos(t)     = adjacent / hypotenuse
		|*|
		|*|         and substituting:
		|*|           opposite   = radius
		|*|
		|*|         we get:
		|*|           hypotenuse = 2 * radius
		|*|           adjacent   = cos(30) * (2 * radius)
		|*|
		|*|         and therefore:
		|*|           offset     = -(slope * radius * cos(30) * 2)
		|*|
		\*/

		/* get the center point */
		CVector_X(center) = *centerX;
		CVector_Y(center) = *centerY;

		/* calculate the bounds components */
		dX = (slopeX * _this->u.other.radius);
		dY = (slopeY * _this->u.other.radius);

		/* define the constant for offset calculation */
		#undef _COS30NEG2
		#define _COS30NEG2 (-1.732050807568877f)

		/* calculate the offset */
		oX = (dX * _COS30NEG2);
		oY = (dY * _COS30NEG2);

		/* undefine the constant for offset calculation */
		#undef _COS30NEG2

		/* calculate the cap bounds */
		CPoint_X(delta[0]) = 0.0f;
		CPoint_Y(delta[0]) = 0.0f;
		CPoint_X(delta[1]) = (oX + dY);
		CPoint_Y(delta[1]) = (oY - dX);
		CPoint_X(delta[2]) = (oX - dY);
		CPoint_Y(delta[2]) = (oY + dX);

		/* transform the cap */
		CVectorF_ScalePoints(_this->u.other.u.scale, delta, 3);

		/* translate the cap into place */
		CVectorF_TranslatePoints(&center, delta, 3);

		/* add the cap to the path */
		CStatus_Check
			(CPath_AddPolygon
				(path, delta, 3));

		/* update the center point */
		*centerX = (oX + CVector_X(center));
		*centerY = (oY + CVector_Y(center));
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CStrokeJoiner_Initialize(CStrokeJoiner     *_this,
                         CPointArrayF      *array,
                         CAffineTransformF *transform,
                         CLineJoin          join,
                         CFloat             radius,
                         CFloat             limit)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((array     != 0));
	CASSERT((transform != 0));

	/* set the type to the default */
	_this->type = 0;

	/* initialize the joiner */
	switch(join)
	{
		case CLineJoin_Miter:
		case CLineJoin_MiterClipped:
		{
			/*\
			|*| TODO: the fallback for Miter should be a partial miter, out to
			|*|       a distance of the limit times the width, instead of a
			|*|       bevel fallback, as is the case for MiterClipped
			\*/
			_this->u.other.transform        = transform;
			CVector_X(_this->u.other.scale) = radius;
			CVector_Y(_this->u.other.scale) = radius;
			_this->u.other.limitSquared     = (limit * limit);
			_this->Join                     = CStrokeJoiner_AddMiter;
		}
		break;
		case CLineJoin_Round:
		{
			/* get the circle points */
			CStroker_CirclePoints(array, radius, transform, 0);

			/* set the points with temporary spacing */
			CStroker_TempSpacePoints
				(array,
				 _this->u.round.points,
				 _this->u.round.count,
				 _this->u.round.size);

			/* set the join method */
			_this->Join = CStrokeJoiner_AddRound;
		}
		break;
		case CLineJoin_Bevel:
		default:
		{
			_this->u.other.transform        = transform;
			CVector_X(_this->u.other.scale) = radius;
			CVector_Y(_this->u.other.scale) = radius;
			_this->Join                     = CStrokeJoiner_AddBevel;
		}
		break;
	}

	/* set the type */
	_this->type = join;

	/* return successfully */
	return CStatus_OK;
}

static void
CStrokeJoiner_Finalize(CStrokeJoiner *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the capper */
	if(_this->type == CLineJoin_Round)
	{
		CFree(_this->u.round.points);
		_this->type = 0;
	}
}

static CStatus
CStrokeCapper_Initialize(CStrokeCapper     *_this,
                         CPointArrayF      *array,
                         CAffineTransformF *transform,
                         CVectorF          *scale,
                         CLineCap           cap,
                         CFloat             radius)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((array     != 0));
	CASSERT((transform != 0));
	CASSERT((scale     != 0));

	/* set the type to the default */
	_this->type = 0;

	/* initialize the capper */
	switch(cap)
	{
		case CLineCap_Square:
		{
			_this->u.other.radius      = radius;
			_this->u.other.u.transform = transform;
			_this->Cap                 = CStrokeCapper_AddSquare;
		}
		break;
		case CLineCap_Round:
		case CLineCap_RoundAnchor:
		{
			/* get the circle points */
			if(cap == CLineCap_Round)
			{
				CStroker_CirclePoints(array, radius, transform, 0);
			}
			else
			{
				CStroker_CirclePoints(array, (radius * 2.0f), 0, scale);
			}

			/* set the points with temporary spacing */
			CStroker_TempSpacePoints
				(array,
				 _this->u.round.points,
				 _this->u.round.count,
				 _this->u.round.size);

			/* set the cap method */
			_this->Cap = CStrokeCapper_AddRound;
		}
		break;
		case CLineCap_Triangle:
		{
			_this->u.other.radius      = radius;
			_this->u.other.u.transform = transform;
			_this->Cap                 = CStrokeCapper_AddTriangle;
		}
		break;
		case CLineCap_SquareAnchor:
		{
			_this->u.other.radius  = (radius * 1.5f);
			_this->u.other.u.scale = scale;
			_this->Cap             = CStrokeCapper_AddSquareAnchor;
		}
		break;
		case CLineCap_DiamondAnchor:
		{
			_this->u.other.radius  = (radius * 2.0f);
			_this->u.other.u.scale = scale;
			_this->Cap             = CStrokeCapper_AddDiamondAnchor;
		}
		break;
		case CLineCap_ArrowAnchor:
		{
			_this->u.other.radius  = (radius * 2.0f);
			_this->u.other.u.scale = scale;
			_this->Cap             = CStrokeCapper_AddArrowAnchor;
		}
		break;
		case CLineCap_Flat:
		case CLineCap_NoAnchor:
		case CLineCap_Custom: /* TODO: add custom cap */
		default:
		{
			_this->Cap  = 0;
		}
		break;
	}

	/* set the type */
	_this->type = cap;

	/* return successfully */
	return CStatus_OK;
}

static void
CStrokeCapper_Finalize(CStrokeCapper *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the capper */
	if((_this->type & ~CLineCap_AnchorMask) == CLineCap_Round)
	{
		CFree(_this->u.round.points);
		_this->type = 0;
	}
}

CINTERNAL CStatus
CStroker_Initialize(CStroker          *_this,
                    CPen              *pen,
                    CAffineTransformF *deviceTransform)
{
	/* assertions */
	CASSERT((_this           != 0));
	CASSERT((pen             != 0));
	CASSERT((deviceTransform != 0));

	/* initialize the stroker */
	{
		/* declarations */
		CLineCap  startCap;
		CLineCap  endCap;
		CLineJoin lineJoin;
		CFloat    scaleX;
		CFloat    scaleY;
		CFloat    width;
		CFloat    miterLimit;

		/* get the cap and join information from the pen */
		startCap = pen->startCap;
		endCap   = pen->endCap;
		lineJoin = pen->lineJoin;

		/* get the pen width and miter limit */
		width      = pen->width;
		miterLimit = pen->miterLimit;

		/* calculate the pen radius */
		_this->radius = (width * 0.5f);

		/* copy the transformations */
		_this->dev = *deviceTransform;
		_this->pen =  pen->transform;

		/* extract the scale from the device transformation */
		CAffineTransformF_ExtractScale
			(&(_this->dev), &scaleX, &scaleY, CMatrixOrder_Prepend);

		/* set the scale */
		CVector_X(_this->scale) = scaleX;
		CVector_Y(_this->scale) = scaleY;

		/* scale the pen transformation */
		CAffineTransformF_Scale
			(&(_this->pen), scaleX, scaleY, CMatrixOrder_Append);

		/* initialize the temporary array */
		CPointArrayF_Initialize(&(_this->array));

		/* TODO: compound and dashed strokes */

		/* prepare to stroke the path */
		if(width <= 0 || ((width * scaleX) == 1  && (width * scaleY) == 1))
		{
			/* set up for fast stroke */
			_this->startCapper = CStrokeCapper_Zero;
			_this->endCapper   = CStrokeCapper_Zero;
			_this->joiner      = CStrokeJoiner_Zero;
			_this->Stroke      = CStroker_FastStroke;
			_this->dev         = *deviceTransform;
			_this->pen         = CAffineTransformF_Identity;
		}
		else
		{
			/* initialize the start capper */
			CStatus_Check
				(CStrokeCapper_Initialize
					(&(_this->startCapper), &(_this->array), &(_this->pen),
					 &(_this->scale), startCap, _this->radius));

			/* initialize the end capper */
			CStatus_Check
				(CStrokeCapper_Initialize
					(&(_this->endCapper), &(_this->array), &(_this->pen),
					 &(_this->scale), endCap, _this->radius));

			/* initialize the joiner */
			CStatus_Check
				(CStrokeJoiner_Initialize
					(&(_this->joiner), &(_this->array), &(_this->pen),
					 lineJoin, _this->radius, miterLimit));

			/* set the stroke method */
			_this->Stroke = CStroker_FullStroke;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL void
CStroker_Finalize(CStroker *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the stroker */
	CPointArrayF_Finalize(&(_this->array));
	CStrokeCapper_Finalize(&(_this->startCapper));
	CStrokeCapper_Finalize(&(_this->endCapper));
	CStrokeJoiner_Finalize(&(_this->joiner));
	_this->Stroke = 0;
}

CINTERNAL CStatus
CStroker_Stroke(CStroker *_this,
                CPath    *path,
                CPointF  *points,
                CByte    *types,
                CUInt32   count,
                CBool     hasCurves)
{
	/* declarations */
	CStatus  status;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((path   != 0));
	CASSERT((points != 0));
	CASSERT((types  != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require((count != 0), CStatus_OK);

	/* stroke path */
	{
		/* assertions */
		CASSERT((points != 0));
		CASSERT((types  != 0));

		/* scale the path, as needed */
		if(_this->Stroke == CStroker_FullStroke)
		{
			CVectorF_ScalePoints(&(_this->scale), points, count);
		}

		/* stroke the path */
		if(!hasCurves)
		{
			/* stroke the subpaths */
			status =
				CStroker_StrokeSubpaths
					(_this, path, points, types, count);
		}
		else
		{
			/* declarations */
			CPointF *newPoints;
			CByte   *newTypes;
			CUInt32  newCount;

			/* flatten the path */
			{
				/* declarations */
				CFlattener flattener;
				CUInt32    capacity;

				/* initialize the flattener */
				CFlattener_Initialize(&flattener);

				/* flatten the path */
				status =
					CFlattener_Flatten
						(&flattener, points, types, count, CFiller_TOLERANCE);

				/* handle flattening failures */
				if(status != CStatus_OK)
				{
					/* finalize the flattener */
					CFlattener_Finalize(&flattener, 0, 0, 0, 0);
	
					/* return status */
					return status;
				}

				/* finalize the flattener */
				CFlattener_Finalize
					(&flattener, &newPoints, &newTypes, &newCount, &capacity);
			}

			/* stroke the subpaths */
			status =
				CStroker_StrokeSubpaths
					(_this, path, newPoints, newTypes, newCount);

			/* free the flattened path */
			CFree(newPoints);
			CFree(newTypes);
		}
	}

	/* return status */
	return status;
}


#ifdef __cplusplus
};
#endif
