/*
 * CFlattener.c - Flattener implementation.
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

#include "CFlattener.h"
#include "CPath.h"

#ifdef __cplusplus
extern "C" {
#endif

static CStatus
CFlattener_EnsureCapacity(CFlattener *_this,
                          CUInt32     minimum)
{
	/* assertions */
	CASSERT((_this != 0));

	/* ensure capacity */
	_EnsurePathCapacity(_this, minimum);

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL void
CFlattener_Initialize(CFlattener *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the point array */
	CPointArrayF_Initialize(&(_this->array));

	/* initialize the remaining members */
	_this->points    = 0;
	_this->types     = 0;
	_this->count     = 0;
	_this->capacity  = 0;
}

CINTERNAL void
CFlattener_Finalize(CFlattener *_this,
                    CPointF    **points,
                    CByte      **types,
                    CUInt32     *count,
                    CUInt32     *capacity)
{
	/* assertions */
	CASSERT((_this  != 0));

	/* finalize the point array */
	CPointArrayF_Finalize(&(_this->array));

	/* get or finalize the path information */
	if(points != 0)
	{
		/* assertions */
		CASSERT((types    != 0));
		CASSERT((count    != 0));
		CASSERT((capacity != 0));

		/* get the path information */
		*points   = _this->points;
		*types    = _this->types;
		*count    = _this->count;
		*capacity = _this->capacity;
	}
	else
	{
		/* assertions */
		CASSERT((types    == 0));
		CASSERT((count    == 0));
		CASSERT((capacity == 0));

		/* finalize the path information */
		CFree(_this->points);
		CFree(_this->types);
	}

	/* reset the path information */
	_this->points   = 0;
	_this->types    = 0;
	_this->count    = 0;
	_this->capacity = 0;
}

CINTERNAL CStatus
CFlattener_Flatten(CFlattener *_this,
                   CPointF    *points,
                   CByte      *types,
                   CUInt32     count,
                   CFloat      tolerance)
{
	/* declarations */
	CByte    *srcT;
	CByte    *dstT;
	CPointF  *srcP;
	CPointF  *dstP;
	CPointF  *end;
	CUInt32   srcN;
	CUInt32   dstN;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));
	CASSERT((types  != 0));

	/* get the type input pointer */
	srcT = types;

	/* get the point input pointer */
	srcP = points;

	/* get the end of input pointer */
	end  = (srcP + count);

	/* set the counts to the default */
	srcN = 0;
	dstN = 0;

	/*\
	|*| NOTE: to avoid excessive method calls, we ensure the capacity
	|*|       assuming all lines, then ensure again only when we hit
	|*|       a curve, so at any point we can add a line cheaply
	\*/

	/* ensure capacity */
	CStatus_Check(CFlattener_EnsureCapacity(_this, count));

	/* get the type output pointer */
	dstT = _this->types;

	/* get the point output pointer */
	dstP = _this->points;

	/* flatten the path */
	while(srcP != end)
	{
		/* process point based on type */
		if((*srcT & CPathType_TypeMask) != CPathType_Bezier)
		{
			/* line to point */
			*dstP = *srcP;
			*dstT = *srcT;

			/* move to the next destination position */
			++dstP; ++dstT; ++dstN;
		}
		else
		{
			/* declarations */
			CByte         type;
			CPointArrayF *array;
			CPointF      *a;
			CPointF      *b;
			CPointF      *c;
			CPointF      *d;
			CBezierF      bezier;

			/* assertions */
			CASSERT((srcP       != points));
			CASSERT(((srcP + 2) != end));

			/* get the current point */
			a = (srcP - 1);

			/* save the type */
			type = *srcT;

			/* get the first point */
			b = srcP;

			/* advance to the second point */
			++srcP; ++srcT; ++srcN;

			/* assertions */
			CASSERT(((*srcT & CPathType_TypeMask) == CPathType_Bezier));

			/* get the second point */
			c = srcP;

			/* advance to the third point */
			++srcP; ++srcT; ++srcN;

			/* assertions */
			CASSERT(((*srcT & CPathType_TypeMask) == CPathType_Bezier));

			/* get the third point */
			d = srcP;

			/* initialize the bezier and handle degenerates */
			if(CBezierF_Initialize(&bezier, a, b, c, d))
			{
				/* move to the next source position */
				++srcP; ++srcT; ++srcN;

				/* TODO: copy d's extra type information into a? */

				/* handle next point */
				continue;
			}

			/* get the point array */
			array = &(_this->array);

			/* reset the count of the point array */
			CPointArray_Count(*array) = 0;

			/* flatten the bezier curve */
			CStatus_Check
				(CBezierF_Flatten
					(&bezier, array, tolerance));

			/* add the lines */
			{
				/* declarations */
				CPointF *arrP;
				CPointF *arrE;
				CUInt32  arrN;

				/* get the count */
				arrN = CPointArray_Count(*array);

				/* set the count */
				_this->count = dstN;

				/* ensure capacity */
				CStatus_Check
					(CFlattener_EnsureCapacity
						(_this, (((dstN + arrN) - 1) + (count - srcN))));

				/* get the destination type pointer */
				dstT = (_this->types + dstN);

				/* get the destination point pointer */
				dstP = (_this->points + dstN);

				/* update the destination count */
				dstN += (arrN - 1);

				/* get the array point pointer */
				arrP = CPointArray_Points(*array);

				/* get the end pointer */
				arrE = (arrP + arrN);

				/* skip the current point */
				++arrP;

				/* add the lines */
				while(arrP != arrE)
				{
					/* set the point */
					*dstP++ = *arrP++;

					/* set the type */
					*dstT++ = CPathType_Line;
				}

				/* add the extra type information to the last point */
				*(dstT - 1) |= (type & ~CPathType_TypeMask);
			}
		}

		/* move to the next source position */
		++srcP; ++srcT; ++srcN;
	}

	/* set the count */
	_this->count = dstN;

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
