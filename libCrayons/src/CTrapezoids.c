/*
 * CTrapezoids.c - Trapezoids implementation.
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

#include "CTrapezoids.h"
#include "CFiller.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef CInt64 CFixedL;
typedef CInt64 CFixedL48;

typedef struct _tagCIntersectionInfo CIntersectionInfo;
struct _tagCIntersectionInfo
{
	CFixed intersection;
	CBool  ok;
};

/* Initialize these trapezoids. */
CINTERNAL void
CTrapezoids_Initialize(CTrapezoids *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the trapezoids */
	_this->count      = 0;
	_this->capacity   = 0;
	_this->trapezoids = 0;
}

/* Finalize these trapezoids. */
CINTERNAL void
CTrapezoids_Finalize(CTrapezoids *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the trapezoids */
	{
		/* get the trapezoid list */
		CTrapezoidX *trapezoids = _this->trapezoids;

		/* finalize, as needed */
		if(trapezoids)
		{
			/* reset the members */
			_this->count      = 0;
			_this->capacity   = 0;
			_this->trapezoids = 0;

			/* free the trapezoid list */
			CFree(trapezoids);
		}
	}
}

/* Reset these trapezoids. */
CINTERNAL void
CTrapezoids_Reset(CTrapezoids *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* reset the trapezoids */
	_this->count = 0;
}

/* Add a trapezoid. */
static CStatus
CTrapezoids_AddTrapezoid(CTrapezoids  *_this,
                         CFixed        top,
                         CFixed        bottom,
                         const CLineX *left,
                         const CLineX *right)
{
	/* declarations */
	CUInt32 count;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((left  != 0));
	CASSERT((right != 0));

	/* bail out now if the trapezoid is degenerate */
	CStatus_Require((top != bottom), CStatus_OK);

	/* get the trapezoid count */
	count = _this->count;

	/* ensure the capacity of trapezoid list */
	if(count >= _this->capacity)
	{
		/* declarations */
		CTrapezoidX *tmp;
		CUInt32  newSize;
		CUInt32  newCapacity;

		/* get the capacity */
		const CUInt32 capacity = _this->capacity;

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
		newSize = (newCapacity * sizeof(CTrapezoidX));

		/* create the new trapezoid list */
		if(!(tmp = (CTrapezoidX *)CRealloc(_this->trapezoids, newSize)))
		{
			return CStatus_OutOfMemory;
		}

		/* update the capacity */
		_this->capacity = newCapacity;

		/* set the trapezoid list */
		_this->trapezoids = tmp;
	}

	/* add the new trapezoid */
	{
		CTrapezoidX *trapezoid        = (_this->trapezoids + count);
		CTrapezoid_Top(*trapezoid)    =  top;
		CTrapezoid_Bottom(*trapezoid) =  bottom;
		CTrapezoid_Left(*trapezoid)   = *left;
		CTrapezoid_Right(*trapezoid)  = *right;
	}

	/* update the trapezoid count */
	_this->count = (count + 1);

	/* return successfully */
	return CStatus_OK;
}

/* Compare the tops of two edges. */
static int
CEdgeX_CompareTop(const void *_a,
                  const void *_b)
{
	/* get the edges */
	const CEdgeX *a = (const CEdgeX *)_a;
	const CEdgeX *b = (const CEdgeX *)_b;

	/* return comparison */
	return (CEdge_Y1(*a) - CEdge_Y1(*b));
}

/* Compare the current x positions of two edges. */
static int
CEdgeX_CompareCurrentX(const void *_a,
                       const void *_b)
{
	/* declarations */
	int cmp;

	/* get the edges */
	const CEdgeX *a = _a;
	const CEdgeX *b = _b;

	/* compare on current x position */
	cmp = (int)(CEdge_CurrentX(*a) - CEdge_CurrentX(*b));

	/* compare on slope, if current x positions are equal */
	if(cmp == 0)
	{
		/* calculate the vectors */
		const CFixedL48 aVectorX = (CEdge_X2(*a) - CEdge_X1(*a));
		const CFixedL48 aVectorY = (CEdge_Y2(*a) - CEdge_Y1(*a));
		const CFixedL48 bVectorX = (CEdge_X2(*b) - CEdge_X1(*b));
		const CFixedL48 bVectorY = (CEdge_Y2(*b) - CEdge_Y1(*b));

		/* return the slope comparison (positive is clockwise) */
		return (int)CMath_CrossProduct(bVectorX, bVectorY, aVectorX, aVectorY);
	}

	/* return comparison */
	return cmp;
}

/* Calculate the intersection of the given line segments. */
static CMATH CIntersectionInfo
CLineX_CalculateIntersection(CLineX a,
                             CLineX b)
{
	/* declarations */
	CIntersectionInfo info;

	/* calculate the inverse slopes */
	const CDouble sA =
		(CFixed_ToDouble(CLine_X2(a) - CLine_X1(a)) /
		 CFixed_ToDouble(CLine_Y2(a) - CLine_Y1(a)));
	const CDouble sB =
		(CFixed_ToDouble(CLine_X2(b) - CLine_X1(b)) /
		 CFixed_ToDouble(CLine_Y2(b) - CLine_Y1(b)));

	/* calculate the intersection, if possible */
	if(sA == sB)
	{
		info.intersection = 0;
		info.ok           = 0;
	}
	else
	{
		/* calculate the intercepts */
		const CDouble iA =
			(CFixed_ToDouble(CLine_X1(a)) -
			 (sA * CFixed_ToDouble(CLine_Y1(a))));
		const CDouble iB =
			(CFixed_ToDouble(CLine_X1(b)) -
			 (sB * CFixed_ToDouble(CLine_Y1(b))));

		/* calculate the intersection */
		info.intersection = CDouble_ToFixed((iB - iA) / (sA - sB));

		/* flag that we have an intersection */
		info.ok = 1;
	}

	/* return the intersection information */
	return info;
}

/* Calculate the x position of the line at the given y position. */
static CMATH CFixed
CLineX_CalculateCurrentX(CLineX line,
                         CFixed y)
{
	/* declarations */
	CFixed dx;

	/* calculate the x vector */
	const CFixed vectorX = (CLine_X2(line) - CLine_X1(line));

	/* calculate the y vector */
	const CFixed vectorY = (CLine_Y2(line) - CLine_Y1(line));

	/* calculate the y delta */
	y -= CLine_Y1(line);

	/* calculate the x delta */
	dx = (((CFixedL48)vectorX * (CFixedL48)(y)) / vectorY);

	/* calculate and return the current x position */
	return (CLine_X1(line) + dx);
}

/* Tessellate the polygon into trapezoids (corrupts polygon). */
CINTERNAL CStatus
CTrapezoids_TessellatePolygon(CTrapezoids *_this,
                              CPolygonX   *polygon,
                              CFillMode    fillMode)
{
	/* declarations */
	CFixed   top;
	CUInt32  edgeCount;
	CEdgeX  *edges;
	CEdgeX  *active;
	CEdgeX  *inactive;
	CEdgeX  *end;

	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((polygon != 0));

	/* get the edge count */
	edgeCount = CPolygon_EdgeCount(*polygon);

	/* bail out now if there's nothing to do */
	CStatus_Require((edgeCount != 0), CStatus_OK);

	/* get the edges */
	edges = CPolygon_Edges(*polygon);

	/* sort the edges by top */
	qsort(edges, edgeCount, sizeof(CEdgeX), CEdgeX_CompareTop);

	/* get the active edge pointer */
	active = edges;

	/* get the inactive edge pointer */
	inactive = (edges + 1);

	/* get the end of input pointer */
	end = (edges + edgeCount);

	/* get the starting y position */
	top = CEdge_Y1(*edges);

	/* process all the edges */
	while(active != end)
	{
		/* declarations */
		CEdgeX *curr;
		CFixed  bottom;

		/* find inactive edges */
		while(inactive != end && CEdge_Y1(*inactive) <= top) { ++inactive; }

		/* calculate the current x positions of the active edges */
		for(curr = active; curr != inactive; ++curr)
		{
			CEdge_CurrentX(*curr) =
				CLineX_CalculateCurrentX
					(CEdge_Line(*curr), top);
		}

		/* sort the edges by their current x position */
		qsort
			(active, (inactive - active), sizeof(CEdgeX),
			 CEdgeX_CompareCurrentX);

		/* set the bottom position to the default */
		bottom = CEdge_Y2(*active);

		/* search for the bottom position among active edges */
		{
			/* get the last active edge */
			const CEdgeX *last = (inactive - 1);

			/* check the last active edge for the bottom position */
			if(CEdge_Y2(*last) < bottom) { bottom = CEdge_Y2(*last); }

			/* search the active edges */
			for(curr = active; curr != last; ++curr)
			{
				/* get the next active edge */
				const CEdgeX *next = (curr + 1);

				/* set the bottom position, based on vertex */
				if(CEdge_Y2(*curr) < bottom) { bottom = CEdge_Y2(*curr); }

				/* set the bottom position, based on intersection */
				if(CEdge_CurrentX(*curr) != CEdge_CurrentX(*next))
				{
					/* calculate the intersection ceiling */
					const CIntersectionInfo info =
						CLineX_CalculateIntersection
							(CEdge_Line(*curr), CEdge_Line(*next));

					/* set the bottom position, as needed */
					if(info.ok &&
					   info.intersection > top &&
					   info.intersection < bottom)
					{
						bottom = info.intersection;
					}
				}
			}
		}

		/* set the bottom position, based on next inactive vertex */
		if(inactive != end && CEdge_Y1(*inactive) < bottom)
		{
			bottom = CEdge_Y1(*inactive);
		}

		/* generate trapezoids from the active edges */
		if(fillMode == CFillMode_Alternate)
		{
			/* declarations */
			CInt32 inside;

			/* get the last active edge */
			const CEdgeX *last = (inactive - 1);

			/* set the insideness to the default */
			inside = 0;

			/* generate trapezoids */
			for(curr = active; curr != last; ++curr)
			{
				/* get the next active edge */
				const CEdgeX *next = (curr + 1);

				/* calculate the insideness of the current edge */
				inside = ((inside + 1) & 1);

				/* add the trapezoid, if it's inside */
				if(inside != 0)
				{
					/* add the trapezoid */
					CStatus_Check
						(CTrapezoids_AddTrapezoid
							(_this,
							 top, bottom,
							 &CEdge_Line(*curr),
							 &CEdge_Line(*next)));
				}
			}
		}
		else
		{
			/* declarations */
			CInt32 inside;

			/* get the last active edge */
			const CEdgeX *last = (inactive - 1);

			/* set the insideness to the default */
			inside = 0;

			/* generate trapezoids */
			for(curr = active; curr != last; ++curr)
			{
				/* get the next active edge */
				const CEdgeX *next = (curr + 1);

				/* calculate the insideness of the current edge */
				inside = (inside + (CEdge_Clockwise(*curr) ? 1 : -1));

				/* add the trapezoid, if it's inside */
				if(inside != 0)
				{
					/* add the trapezoid */
					CStatus_Check
						(CTrapezoids_AddTrapezoid
							(_this,
							 top, bottom,
							 &CEdge_Line(*curr),
							 &CEdge_Line(*next)));
				}
			}
		}

		/* remove passed edges */
		for(curr = active; curr != inactive; ++curr)
		{
			/* remove the current edge, as needed */
			if(CEdge_Y2(*curr) <= bottom)
			{
				/* calculate the number of bytes to move */
				const CUInt32 n = ((curr - active) * sizeof(CEdgeX));

				/* remove the current edge from the edge list */
				CMemMove((active + 1), active, n);

				/* update active edge pointer */
				++active;
			}
		}

		/* move to next y position */
		top = bottom;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Fill the path to these trapezoids. */
CINTERNAL CStatus
CTrapezoids_Fill(CTrapezoids *_this,
                 CPointF     *points,
                 CByte       *types,
                 CUInt32      count,
                 CFillMode    fillMode)
{
	/* declarations */
	CStatus status;
	CFiller filler;

	/* bail out now if there's nothing to do */
	CStatus_Require((count != 0), CStatus_OK);

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));
	CASSERT((types  != 0));

	/* initialize the filler */
	CFiller_Initialize(&filler);

	/* fill to the trapezoids */
	status =
		CFiller_ToTrapezoids
			(&filler, _this, points, types, count, fillMode);

	/* finalize the filler */
	CFiller_Finalize(&filler);

	/* return status */
	return status;
}


#ifdef __cplusplus
};
#endif
