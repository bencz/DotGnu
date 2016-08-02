/*
 * CRegionRasterizer.c - Region rasterizer implementation.
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

#include "CRegionRasterizer.h"
#include "CAffineTransform.h"
#include "CTrapezoids.h"
#include "CPointArray.h"

#ifdef __cplusplus
extern "C" {
#endif

static const pixman_operator_t CRegionType_PixmanOperator[] =
{
	PIXMAN_OPERATOR_SRC,
	PIXMAN_OPERATOR_IN,
	PIXMAN_OPERATOR_ADD,
	PIXMAN_OPERATOR_XOR,
	PIXMAN_OPERATOR_OUT,
	PIXMAN_OPERATOR_OUT_REVERSE
};

static CStatus
CRegionRasterizer_CreateMaskSimple(CRegionRasterizer  *_this,
                                   CByte               value,
                                   pixman_image_t    **mask)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((mask  != 0));

	/* create the mask */
	*mask =
		pixman_image_create
			(_this->format, _this->width, _this->height);

	/* ensure we have a mask */
	CStatus_Require((*mask != 0), CStatus_OutOfMemory);

	/* ensure we have the size */
	if(_this->size == -1)
	{
		_this->size = (_this->height * pixman_image_get_stride(*mask));
	}

	/* initialize the mask */
	CMemSet(pixman_image_get_data(*mask), value, _this->size);

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionRasterizer_CreateMaskPath(CRegionRasterizer  *_this,
                                 const CPointF      *points,
                                 const CByte        *types,
                                 CUInt32             count,
                                 CFillMode           fillMode,
                                 pixman_image_t    **mask)
{
	/* declarations */
	CTrapezoidX *trapezoids;
	CUInt32      tcount;

	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((points != 0));
	CASSERT((types  != 0));
	CASSERT((mask   != 0));

	/* reset filler and trapezoids */
	CTrapezoids_Reset(&(_this->trapezoids));
	CFiller_Reset(&(_this->filler));

	/* perform path trapezoidation */
	CStatus_Check
		(CFiller_ToTrapezoids
			(&(_this->filler), &(_this->trapezoids),
			 points, types, count, fillMode));

	/* create the mask */
	CStatus_Check
		(CRegionRasterizer_CreateMaskSimple
			(_this, 0x00, mask));

	/* get the trapezoid list and the count */
	tcount     = CTrapezoids_Count(_this->trapezoids);
	trapezoids = CTrapezoids_Trapezoids(_this->trapezoids);

	/* add the trapezoids to the mask */
	pixman_add_trapezoids
		(*mask, 0, 0, ((pixman_trapezoid_t *)trapezoids), tcount);

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionRasterizer_Data(CRegionInterpreter  *_this,
                       CRegionNode         *node,
                       void               **data)
{
	/* declarations */
	CRegionRasterizer *rast;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((node  != 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* get this as a rasterizer */
	rast = ((CRegionRasterizer *)_this);

	/* rasterize based on type */
	if(node->type == CRegionType_Rectangle)
	{
		/* declarations */
		CPointF      points[4];
		CRegionRect *rr;

		/* get the region rectangle */
		rr = ((CRegionRect *)node);

		/* set the points */
		CRegionRect_RectToPath(points, rr->rectangle);

		/* transform the points, as needed */
		if(rast->transform != 0)
		{
			CAffineTransformF_TransformPoints(rast->transform, points, 4);
		}

		/* rasterize the rectangle */
		{
			/* declarations */
			pixman_image_t *mask;

			/* create the mask */
			CStatus_Check
				(CRegionRasterizer_CreateMaskPath
					(rast, points, CRegionRect_PathTypes, 4,
					 CFillMode_Alternate, &mask));

			/* set the data */
			*data = mask;
		}
	}
	else if(node->type == CRegionType_Path)
	{
		/* declarations */
		CPointArrayF *array;
		CRegionPath  *rp;

		/* get the region path */
		rp = ((CRegionPath *)node);

		/* get the array */
		array = &(rast->array);

		/* ensure the capacity of the array */
		CStatus_Check
			(CPointArrayF_EnsureCapacity
				(array, rp->count));

		/* copy the points */
		CMemCopy(CPointArray_Points(*array), rp->points, rp->count);

		/* transform the points, as needed */
		if(rast->transform != 0)
		{
			CAffineTransformF_TransformPoints
				(rast->transform, CPointArray_Points(*array), rp->count);
		}

		/* rasterize the path */
		{
			/* declarations */
			pixman_image_t *mask;

			/* create the mask */
			CStatus_Check
				(CRegionRasterizer_CreateMaskPath
					(rast, CPointArray_Points(*array), rp->types, rp->count,
					 rp->fillMode, &mask));

			/* set the data */
			*data = mask;
		}
	}
	else if(node->type == CRegionType_Infinite)
	{
		/* declarations */
		pixman_image_t *mask;

		/* create the mask */
		CStatus_Check
			(CRegionRasterizer_CreateMaskSimple
				(rast, 0xFF, &mask));

		/* set the data */
		*data = mask;
	}
	else
	{
		/* declarations */
		pixman_image_t *mask;

		/* assertions */
		CASSERT((node->type == CRegionType_Empty));

		/* create the mask */
		CStatus_Check
			(CRegionRasterizer_CreateMaskSimple
				(rast, 0x00, &mask));

		/* set the data */
		*data = mask;
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionRasterizer_Op(CRegionInterpreter  *_this,
                     CRegionOp           *op,
                     void                *left,
                     void                *right,
                     void               **data)
{
	/* declarations */
	CRegionRasterizer *rast;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((op    != 0));
	CASSERT((left  != 0));
	CASSERT((right != 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* get this as a rasterizer */
	rast = ((CRegionRasterizer *)_this);

	/* perform the operation */
	{
		/* declarations */
		pixman_image_t *li;
		pixman_image_t *ri;

		/* get the images */
		li = ((pixman_image_t *)left);
		ri = ((pixman_image_t *)right);

		/* composite the images */
		pixman_composite
			(CRegionType_PixmanOperator[op->_base.type], li, 0, ri,
			 0, 0, 0, 0, 0, 0, rast->width, rast->height);

		/* free the left operand */
		pixman_image_destroy(li);

		/* set the data */
		*data = ri;
	}

	/* return successfully */
	return CStatus_OK;
}

static void
CRegionRasterizer_FreeData(void *data)
{
	/* assertions */
	CASSERT((data != 0));

	/* free the data */
	pixman_image_destroy((pixman_image_t *)data);
}

static const CRegionInterpreterClass CRegionRasterizer_Class =
{
	CRegionRasterizer_Data,
	CRegionRasterizer_Op,
	CRegionRasterizer_FreeData
};

CINTERNAL CStatus
CRegionRasterizer_Initialize(CRegionRasterizer *_this,
                             CAffineTransformF *transform,
                             CFloat             width,
                             CFloat             height,
                             CBool              gray)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	{
		/* initialize the format */
		if(gray)
		{
			_this->format = pixman_format_create(PIXMAN_FORMAT_NAME_A8);
		}
		else
		{
			_this->format = pixman_format_create(PIXMAN_FORMAT_NAME_A1);
		}

		/* ensure we have a format */
		CStatus_Require((_this->format != 0), CStatus_OutOfMemory);

		/* initialize the trapezoids */
		CTrapezoids_Initialize(&(_this->trapezoids));

		/* initialize the filler */
		CFiller_Initialize(&(_this->filler));

		/* initialize the array */
		CPointArrayF_Initialize(&(_this->array));

		/* initialize the remaining members */
		_this->transform = transform;
		_this->size      = -1;
		_this->width     = width;
		_this->height    = height;
	}

	/* initialize the base */
	CRegionInterpreter_Initialize
		((CRegionInterpreter *)_this, &CRegionRasterizer_Class);

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL void
CRegionRasterizer_Finalize(CRegionRasterizer *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the members */
	{
		/* finalize the base */
		CRegionInterpreter_Finalize((CRegionInterpreter *)_this);

		/* finalize the trapezoids */
		CTrapezoids_Finalize(&(_this->trapezoids));

		/* finalize the filler */
		CFiller_Finalize(&(_this->filler));

		/* finalize the array */
		CPointArrayF_Finalize(&(_this->array));

		/* free the format */
		pixman_format_destroy(_this->format);

		/* finalize the transform */
		_this->transform = 0;
	}
}


#ifdef __cplusplus
};
#endif
