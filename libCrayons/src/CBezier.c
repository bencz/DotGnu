/*
 * CBezier.c - Bezier implementation.
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

#include "CBezier.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL CBool
CBezierX_Initialize(CBezierX *_this,
                    CPointX  *a,
                    CPointX  *b,
                    CPointX  *c,
                    CPointX  *d)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((a     != 0));
	CASSERT((b     != 0));
	CASSERT((c     != 0));
	CASSERT((d     != 0));

	/* set the control points of the curve */
    _this->a = *a;
    _this->b = *b;
    _this->c = *c;
    _this->d = *d;

	/* bail out now if the curve is degenerate */
	if(CPoint_X(*a) == CPoint_X(*d) && CPoint_Y(*a) == CPoint_Y(*d))
	{
		/* return with degenerate flag */
		return 1;
	}

	/* return with non-degenerate flag */
	return 0;
}

CINTERNAL CBool
CBezierF_Initialize(CBezierF *_this,
                    CPointF  *a,
                    CPointF  *b,
                    CPointF  *c,
                    CPointF  *d)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((a     != 0));
	CASSERT((b     != 0));
	CASSERT((c     != 0));
	CASSERT((d     != 0));

	/* set the control points of the curve */
    _this->a = *a;
    _this->b = *b;
    _this->c = *c;
    _this->d = *d;

	/* bail out now if the curve is degenerate */
	if(CPoint_X(*a) == CPoint_X(*d) && CPoint_Y(*a) == CPoint_Y(*d))
	{
		/* return with degenerate flag */
		return 1;
	}

	/* return with non-degenerate flag */
	return 0;
}


CINTERNAL void
CBezierX_Finalize(CBezierX *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reset the members */
	*_this = CBezierX_Zero;
}

CINTERNAL void
CBezierF_Finalize(CBezierF *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reset the members */
	*_this = CBezierF_Zero;
}

static CMATH CPointX
CBezierX_Midpoint(CPointX start,
                  CPointX end)
{
	/* initialize the midpoint to the start point */
	CPointX middle = start;

	/* calculate and set the midpoint */
	CPoint_X(middle) += ((CPoint_X(end) - CPoint_X(start)) >> 1);
	CPoint_Y(middle) += ((CPoint_Y(end) - CPoint_Y(start)) >> 1);

	/* return the midpoint */
	return middle;
}

static CMATH CPointF
CBezierF_Midpoint(CPointF start,
                  CPointF end)
{
	/* initialize the midpoint to the start point */
	CPointF middle = start;

	/* calculate and set the midpoint */
	CPoint_X(middle) += ((CPoint_X(end) - CPoint_X(start)) * 0.5f);
	CPoint_Y(middle) += ((CPoint_Y(end) - CPoint_Y(start)) * 0.5f);

	/* return the midpoint */
	return middle;
}

static void
CBezierX_DeCasteljau(CBezierX *_this,
                     CBezierX *start,
                     CBezierX *end)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((start != 0));
	CASSERT((end   != 0));

	/* initialize the results */
	*start = CBezierX_Zero;
	*end   = CBezierX_Zero;

	/* calculate the results */
	{
		/* get the curve control points */
		const CPointX a = _this->a;
		const CPointX b = _this->b;
		const CPointX c = _this->c;
		const CPointX d = _this->d;

		/* calculate the midpoints */
		const CPointX ab    = CBezierX_Midpoint(a, b);
		const CPointX bc    = CBezierX_Midpoint(b, c);
		const CPointX cd    = CBezierX_Midpoint(c, d);
		const CPointX abbc  = CBezierX_Midpoint(ab, bc);
		const CPointX bccd  = CBezierX_Midpoint(bc, cd);
		const CPointX ab_cd = CBezierX_Midpoint(abbc, bccd);

		/* set the starting curve control points */
		start->a = a;
		start->b = ab;
		start->c = abbc;
		start->d = ab_cd;

		/* set the ending curve control points */
		end->a = ab_cd;
		end->b = bccd;
		end->c = cd;
		end->d = d;
	}
}

static void
CBezierF_DeCasteljau(CBezierF *_this,
                     CBezierF *start,
                     CBezierF *end)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((start != 0));
	CASSERT((end   != 0));

	/* initialize the results */
	*start = CBezierF_Zero;
	*end   = CBezierF_Zero;

	/* calculate the results */
	{
		/* get the curve control points */
		const CPointF a = _this->a;
		const CPointF b = _this->b;
		const CPointF c = _this->c;
		const CPointF d = _this->d;

		/* calculate the midpoints */
		const CPointF ab    = CBezierF_Midpoint(a, b);
		const CPointF bc    = CBezierF_Midpoint(b, c);
		const CPointF cd    = CBezierF_Midpoint(c, d);
		const CPointF abbc  = CBezierF_Midpoint(ab, bc);
		const CPointF bccd  = CBezierF_Midpoint(bc, cd);
		const CPointF ab_cd = CBezierF_Midpoint(abbc, bccd);

		/* set the starting curve control points */
		start->a = a;
		start->b = ab;
		start->c = abbc;
		start->d = ab_cd;

		/* set the ending curve control points */
		end->a = ab_cd;
		end->b = bccd;
		end->c = cd;
		end->d = d;
	}
}

static CMATH CDouble
CBezierX_DistanceToLine(CPointX p,
                        CPointX start,
                        CPointX end)
{
	/* declarations */
	CDouble u;

	/*\
	|*|
	|*| L - the line
	|*| S - the start of the line
	|*| E - the end of the line
	|*| P - the point whose distance from the line is to be calculated
	|*| I - intersection of P and L
	|*| m - magnitude squared
	|*|
	|*| . - dot product operator
	|*| x - x accessor
	|*| y - y accessor
	|*|
	|*|   PS = (P - S)
	|*|   ES = (E - S)
	|*|
	|*|   L  = S + (u * ES)
	|*|   I  = (P - L) . (ES)
	|*|      = (PS - (u * ES)) . (ES)
	|*|   ---------------------------------------------
	|*|   m  = ((x(ES) * x(ES)) + (y(ES) * y(ES)))
	|*|   u  = ((x(PS) * x(ES)) + (y(PS) * y(ES))) / m
	|*|
	\*/

	/* calculate the component vectors */
	const CDouble esX = CFixed_ToFloat(CPoint_X(end) - CPoint_X(start));
	const CDouble esY = CFixed_ToFloat(CPoint_Y(end) - CPoint_Y(start));
	const CDouble psX = CFixed_ToFloat(CPoint_X(p)   - CPoint_X(start));
	const CDouble psY = CFixed_ToFloat(CPoint_Y(p)   - CPoint_Y(start));

	/* handle degenerate case (distance to start point) */
	if(esX == 0.0f && esY == 0.0f)
	{
		/* calculate and return the distance (squared) */
		return CMath_DotProduct(psX, psY, psX, psY);
	}

	/* calculate the line coefficient */
	u = (CMath_DotProduct(psX, psY, esX, esY) /
	     CMath_DotProduct(esX, esY, esX, esY));

	/* handle degenerate case (distance to start point) */
	if(u <= 0.0f)
	{
		/* calculate and return the distance (squared) */
		return CMath_DotProduct(psX, psY, psX, psY);
	}

	/* handle degenerate case (distance to end point) */
	if(u >= 1.0f)
	{
		/* calculate the component vectors */
		const CDouble peX = CFixed_ToFloat(CPoint_X(p) - CPoint_X(end));
		const CDouble peY = CFixed_ToFloat(CPoint_Y(p) - CPoint_Y(end));

		/* calculate and return the distance (squared) */
		return CMath_DotProduct(peX, peY, peX, peY);
	}

	/* handle non-degenerate case (distance to line segment) */
	{
		/* calculate the component vectors */
		const CDouble piX = psX - (u * esX);
		const CDouble piY = psY - (u * esY);

		/* calculate and return the distance (squared) */
		return CMath_DotProduct(piX, piY, piX, piY);
	}
}

static CMATH CDouble
CBezierF_DistanceToLine(CPointF p,
                        CPointF start,
                        CPointF end)
{
	/* declarations */
	CDouble u;

	/*\
	|*|
	|*| L - the line
	|*| S - the start of the line
	|*| E - the end of the line
	|*| P - the point whose distance from the line is to be calculated
	|*| I - intersection of P and L
	|*| m - magnitude squared
	|*|
	|*| . - dot product operator
	|*| x - x accessor
	|*| y - y accessor
	|*|
	|*|   PS = (P - S)
	|*|   ES = (E - S)
	|*|
	|*|   L  = S + (u * ES)
	|*|   I  = (P - L) . (ES)
	|*|      = (PS - (u * ES)) . (ES)
	|*|   ---------------------------------------------
	|*|   m  = ((x(ES) * x(ES)) + (y(ES) * y(ES)))
	|*|   u  = ((x(PS) * x(ES)) + (y(PS) * y(ES))) / m
	|*|
	\*/

	/* calculate the component vectors */
	const CDouble esX = (CPoint_X(end) - CPoint_X(start));
	const CDouble esY = (CPoint_Y(end) - CPoint_Y(start));
	const CDouble psX = (CPoint_X(p)   - CPoint_X(start));
	const CDouble psY = (CPoint_Y(p)   - CPoint_Y(start));

	/* handle degenerate case (distance to start point) */
	if(esX == 0.0f && esY == 0.0f)
	{
		/* calculate and return the distance (squared) */
		return CMath_DotProduct(psX, psY, psX, psY);
	}

	/* calculate the line coefficient */
	u = (CMath_DotProduct(psX, psY, esX, esY) /
	     CMath_DotProduct(esX, esY, esX, esY));

	/* handle degenerate case (distance to start point) */
	if(u <= 0.0f)
	{
		/* calculate and return the distance (squared) */
		return CMath_DotProduct(psX, psY, psX, psY);
	}

	/* handle degenerate case (distance to end point) */
	if(u >= 1.0f)
	{
		/* calculate the component vectors */
		const CDouble peX = (CPoint_X(p) - CPoint_X(end));
		const CDouble peY = (CPoint_Y(p) - CPoint_Y(end));

		/* calculate and return the distance (squared) */
		return CMath_DotProduct(peX, peY, peX, peY);
	}

	/* handle non-degenerate case (distance to line segment) */
	{
		/* calculate the component vectors */
		const CDouble piX = psX - (u * esX);
		const CDouble piY = psY - (u * esY);

		/* calculate and return the distance (squared) */
		return CMath_DotProduct(piX, piY, piX, piY);
	}
}

static CBool
CBezierX_IsInTolerance(CBezierX *_this,
                       CDouble   tSquared)
{
	/* assertions */
	CASSERT((_this != 0));

	/* perform a tolerance check on the first control point */
	if(CBezierX_DistanceToLine(_this->b, _this->a, _this->d) >= tSquared)
	{
		/* return the 'is not inside tolerance' flag */
		return 0;
	}

	/* perform a tolerance check on the second control point */
	if(CBezierX_DistanceToLine(_this->c, _this->a, _this->d) >= tSquared)
	{
		/* return the 'is not inside tolerance' flag */
		return 0;
	}

	/* return the 'is inside tolerance' flag */
	return 1;
}

static CBool
CBezierF_IsInTolerance(CBezierF *_this,
                       CDouble   tSquared)
{
	/* assertions */
	CASSERT((_this != 0));

	/* perform a tolerance check on the first control point */
	if(CBezierF_DistanceToLine(_this->b, _this->a, _this->d) >= tSquared)
	{
		/* return the 'is not inside tolerance' flag */
		return 0;
	}

	/* perform a tolerance check on the second control point */
	if(CBezierF_DistanceToLine(_this->c, _this->a, _this->d) >= tSquared)
	{
		/* return the 'is not inside tolerance' flag */
		return 0;
	}

	/* return the 'is inside tolerance' flag */
	return 1;
}

static CStatus
CBezierX_FlattenR(CBezierX     *_this,
                  CPointArrayX *array,
                  CDouble       tSquared)
{
	/* declarations */
	CBezierX w;
	CBezierX z;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((array != 0));

	/* handle the base case */
	if(CBezierX_IsInTolerance(_this, tSquared))
	{
		return CPointArrayX_AppendPoint(array, &(_this->a));
	}

	/* split the curve */
	CBezierX_DeCasteljau(_this, &w, &z);

	/* recurse on the first half of the curve */
	CStatus_Check
		(CBezierX_FlattenR
			(&w, array, tSquared));

	/* recurse on the second half of the curve */
	CStatus_Check
		(CBezierX_FlattenR
			(&z, array, tSquared));

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CBezierF_FlattenR(CBezierF     *_this,
                  CPointArrayF *array,
                  CDouble       tSquared)
{
	/* declarations */
	CBezierF w;
	CBezierF z;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((array != 0));

	/* handle the base case */
	if(CBezierF_IsInTolerance(_this, tSquared))
	{
		return CPointArrayF_AppendPoint(array, &(_this->a));
	}

	/* split the curve */
	CBezierF_DeCasteljau(_this, &w, &z);

	/* recurse on the first half of the curve */
	CStatus_Check
		(CBezierF_FlattenR
			(&w, array, tSquared));

	/* recurse on the second half of the curve */
	CStatus_Check
		(CBezierF_FlattenR
			(&z, array, tSquared));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CBezierX_Flatten(CBezierX     *_this,
                 CPointArrayX *array,
                 CDouble       tolerance)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((array != 0));

	/* calculate the tolerance squared */
	tolerance = (tolerance * tolerance);

	/* ensure the tolerance is within bounds */
	if(tolerance < 0.000001f) { tolerance = 0.000001f; }

	/* recursively flatten the curve */
	CStatus_Check
		(CBezierX_FlattenR
			(_this, array, tolerance));

	/* add the final point */
	CStatus_Check
		(CPointArrayX_AppendPoint
			(array, &(_this->d)));

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CBezierF_Flatten(CBezierF     *_this,
                 CPointArrayF *array,
                 CDouble       tolerance)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((array != 0));

	/* calculate the tolerance squared */
	tolerance = (tolerance * tolerance);

	/* ensure the tolerance is within bounds */
	if(tolerance < 0.000001f) { tolerance = 0.000001f; }

	/* recursively flatten the curve */
	CStatus_Check
		(CBezierF_FlattenR
			(_this, array, tolerance));

	/* add the final point */
	CStatus_Check
		(CPointArrayF_AppendPoint
			(array, &(_this->d)));

	/* return successfully */
	return CStatus_OK;
}

#ifdef __cplusplus
};
#endif
