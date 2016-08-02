/*
 * CPointArray.c - Point array implementation.
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

#include "CPointArray.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL void
CPointArrayX_Initialize(CPointArrayX *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	*_this = CPointArrayX_Zero;
}

CINTERNAL void
CPointArrayF_Initialize(CPointArrayF *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	*_this = CPointArrayF_Zero;
}

CINTERNAL void
CPointArrayX_Finalize(CPointArrayX *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the members */
	if(_this->points != 0)
	{
		CFree(_this->points);
		*_this = CPointArrayX_Zero;
	}
}

CINTERNAL void
CPointArrayF_Finalize(CPointArrayF *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the members */
	if(_this->points != 0)
	{
		CFree(_this->points);
		*_this = CPointArrayF_Zero;
	}
}

CINTERNAL CStatus
CPointArrayX_AppendPointNoRepeat(CPointArrayX *_this,
                                 CPointX      *point)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((point != 0));

	/* bail out now if there's nothing to append */
	if(CPointArray_Count(*_this) != 0)
	{
		/* get the last point */
		const CPointX last =
			CPointArray_Point(*_this, CPointArray_Count(*_this) - 1);

		/* bail out if the last point and the current point match */
		if(CPoint_X(last) == CPoint_X(*point) &&
		   CPoint_Y(last) == CPoint_Y(*point))
		{
			return CStatus_OK;
		}
	}

	/* ensure capacity of point list */
	CStatus_Check
		(CPointArrayX_EnsureCapacity
			(_this, (CPointArray_Count(*_this) + 1)));

	/* append the point */
	CPointArray_Point(*_this, CPointArray_Count(*_this)++) = *point;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPointArrayF_AppendPointNoRepeat(CPointArrayF *_this,
                                 CPointF      *point)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((point != 0));

	/* bail out now if there's nothing to append */
	if(CPointArray_Count(*_this) != 0)
	{
		/* get the last point */
		const CPointF last =
			CPointArray_Point(*_this, CPointArray_Count(*_this) - 1);

		/* bail out if the last point and the current point match */
		if(CPoint_X(last) == CPoint_X(*point) &&
		   CPoint_Y(last) == CPoint_Y(*point))
		{
			return CStatus_OK;
		}
	}

	/* ensure capacity of point list */
	CStatus_Check
		(CPointArrayF_EnsureCapacity
			(_this, (CPointArray_Count(*_this) + 1)));

	/* append the point */
	CPointArray_Point(*_this, CPointArray_Count(*_this)++) = *point;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPointArrayX_AppendPoint(CPointArrayX *_this,
                         CPointX      *point)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((point != 0));

	/* ensure capacity of point list */
	CStatus_Check
		(CPointArrayX_EnsureCapacity
			(_this, (CPointArray_Count(*_this) + 1)));

	/* append the point */
	CPointArray_Point(*_this, CPointArray_Count(*_this)++) = *point;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPointArrayF_AppendPoint(CPointArrayF *_this,
                         CPointF      *point)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((point != 0));

	/* ensure capacity of point list */
	CStatus_Check
		(CPointArrayF_EnsureCapacity
			(_this, (CPointArray_Count(*_this) + 1)));

	/* append the point */
	CPointArray_Point(*_this, CPointArray_Count(*_this)++) = *point;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPointArrayX_EnsureCapacity(CPointArrayX *_this,
                            CUInt32       minimum)
{
	/* assertions */
	CASSERT((_this != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require((minimum != 0), CStatus_OK);

	/* reallocate the list, as needed */
	if(minimum > _this->capacity)
	{
		/* declarations */
		CPointX *tmp;
		CUInt32  capacity;

		/* calculate the new capacity */
		capacity = (_this->capacity << 1);

		/* calculate the optimal capacity, as needed */
		if(capacity < minimum || capacity == 0)
		{
			/* calculate the new capacity */
			capacity = (((_this->capacity + minimum) + 31) & ~31);
		}

		/* create the new point list */
		if(!(tmp = (CPointX *)CMalloc(capacity * sizeof(CPointX))))
		{
			return CStatus_OutOfMemory;
		}

		/* copy existing data, as needed */
		if(_this->count != 0)
		{
			/* copy the points */
			CMemCopy(tmp, _this->points, (_this->count * sizeof(CPointX)));
		}

		/* free existing list, as needed */
		if(_this->capacity != 0)
		{
			/* free the point list */
			CFree(_this->points);
		}

		/* update the capacity */
		_this->capacity = capacity;

		/* set the point list */
		_this->points = tmp;
	}

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CPointArrayF_EnsureCapacity(CPointArrayF *_this,
                            CUInt32       minimum)
{
	/* assertions */
	CASSERT((_this != 0));

	/* bail out now if there's nothing to do */
	CStatus_Require((minimum != 0), CStatus_OK);

	/* reallocate the list, as needed */
	if(minimum > _this->capacity)
	{
		/* declarations */
		CPointF *tmp;
		CUInt32  capacity;

		/* calculate the new capacity */
		capacity = (_this->capacity << 1);

		/* calculate the optimal capacity, as needed */
		if(capacity < minimum || capacity == 0)
		{
			/* calculate the new capacity */
			capacity = (((_this->capacity + minimum) + 31) & ~31);
		}

		/* create the new point list */
		if(!(tmp = (CPointF *)CMalloc(capacity * sizeof(CPointF))))
		{
			return CStatus_OutOfMemory;
		}

		/* copy existing data, as needed */
		if(_this->count != 0)
		{
			/* copy the points */
			CMemCopy(tmp, _this->points, (_this->count * sizeof(CPointF)));
		}

		/* free existing list, as needed */
		if(_this->capacity != 0)
		{
			/* free the point list */
			CFree(_this->points);
		}

		/* update the capacity */
		_this->capacity = capacity;

		/* set the point list */
		_this->points = tmp;
	}

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
