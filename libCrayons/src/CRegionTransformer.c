/*
 * CRegionTransformer.c - Region transformer implementation.
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

#include "CRegionTransformer.h"
#include "CAffineTransform.h"

#ifdef __cplusplus
extern "C" {
#endif

static CStatus
CRegionTransformer_Op(CRegionInterpreter  *_this,
                      CRegionOp           *op,
                      void                *left,
                      void                *right,
                      void               **data)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((op    != 0));
	CASSERT((data  != 0));

	/* set data to the default */
	*data = 0;

	/* handle the left operand */
	if(left != 0)
	{
		/* get the left node */
		CRegionNode *node = ((CRegionNode *)left);

		/* assertions */
		CASSERT((op->left->type == CRegionType_Rectangle));
		CASSERT((node->type     == CRegionType_Path));

		/* free the current left node */
		CRegionData_Free(op->left);

		/* set the left node */
		op->left = node;
	}

	/* handle the right operand */
	if(right != 0)
	{
		/* get the right node */
		CRegionNode *node = ((CRegionNode *)right);

		/* assertions */
		CASSERT((op->right->type == CRegionType_Rectangle));
		CASSERT((node->type      == CRegionType_Path));

		/* free the current right node */
		CRegionData_Free(op->right);

		/* set the right node */
		op->right = node;
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionTransformer_Data(CRegionInterpreter  *_this,
                        CRegionNode         *node,
                        void               **data)
{
	/* declarations */
	CRegionTransformer *t;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((node  != 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* get this as a region transformer */
	t = ((CRegionTransformer *)_this);

	/* transform the node */
	if(node->type == CRegionType_Rectangle)
	{
		/* declarations */
		CRegionPath *rp;
		CRegionRect *rr;

		/* get the rectangle node */
		rr = ((CRegionRect *)node);

		/* allocate the path node */
		if(!CRegionPath_Alloc(rp))
		{
			return CStatus_OutOfMemory;
		}

		/* allocate the point list */
		if(!(rp->points = (CPointF *)CMalloc(4 * sizeof(CPointF))))
		{
			CFree(rp);
			return CStatus_OutOfMemory;
		}

		/* allocate the type list */
		if(!(rp->types = (CByte *)CMalloc(4 * sizeof(CByte))))
		{
			CFree(rp->points);
			CFree(rp);
			return CStatus_OutOfMemory;
		}

		/* set the base */
		rp->_base = CRegionNode_Path;

		/* set the count */
		rp->count = 4;

		/* set the fill mode */
		rp->fillMode = CFillMode_Alternate;

		/* set the points */
		CRegionRect_RectToPath(rp->points, rr->rectangle);

		/* set the types */
		CMemCopy(rp->types, CRegionRect_PathTypes, 4);

		/* transform the points */
		CAffineTransformF_TransformPoints(t->transform, rp->points, 4);

		/* set the data */
		*data = (void *)rp;
	}
	else if(node->type == CRegionType_Path)
	{
		/* get the path node */
		CRegionPath *rp = ((CRegionPath *)node);

		/* transform the points */
		CAffineTransformF_TransformPoints
			(t->transform, rp->points, rp->count);
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionTransformer_DataSimple(CRegionInterpreter  *_this,
                              CRegionNode         *node,
                              void               **data)
{
	/* declarations */
	CRegionTransformer *t;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((node  != 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* get this as a region transformer */
	t = ((CRegionTransformer *)_this);

	/* transform the node */
	if(node->type == CRegionType_Rectangle)
	{
		/* declarations */
		CRegionRect *rr;

		/* get the transformation properties */
		const CFloat xx = CAffineTransform_XX(*(t->transform));
		const CFloat yy = CAffineTransform_YY(*(t->transform));
		const CFloat dx = CAffineTransform_DX(*(t->transform));
		const CFloat dy = CAffineTransform_DY(*(t->transform));

		/* get the rectangle node */
		rr = ((CRegionRect *)node);

		/* perform the transformation */
		{
			/* get the rectangle properties */
			const CFloat x = CRectangle_X(rr->rectangle);
			const CFloat y = CRectangle_Y(rr->rectangle);
			const CFloat w = CRectangle_Width(rr->rectangle);
			const CFloat h = CRectangle_Height(rr->rectangle);

			/* transform x and width */
			if(xx < 0.0f)
			{
				CRectangle_X(rr->rectangle)     = (xx * (x + w)) + dx;
				CRectangle_Width(rr->rectangle) = (-xx * w);
			}
			else
			{
				CRectangle_X(rr->rectangle)     = (xx * x) + dx;
				CRectangle_Width(rr->rectangle) = (xx * w);
			}

			/* transform y and height */
			if(yy < 0.0f)
			{
				CRectangle_Y(rr->rectangle)      = (yy * (y - h)) + dy;
				CRectangle_Height(rr->rectangle) = (-yy * h);
			}
			else
			{
				CRectangle_Y(rr->rectangle)      = (yy * y) + dy;
				CRectangle_Height(rr->rectangle) = (yy * h);
			}
		}
	}
	else if(node->type == CRegionType_Path)
	{
		/* get the path node */
		CRegionPath *rp = ((CRegionPath *)node);

		/* transform the points */
		CAffineTransformF_TransformPoints
			(t->transform, rp->points, rp->count);
	}

	/* return successfully */
	return CStatus_OK;
}

static void
CRegionTransformer_FreeData(void *data)
{
	/* declarations */
	CRegionPath *rp;

	/* assertions */
	CASSERT((data != 0));

	/* get the path node */
	rp = ((CRegionPath *)data);

	/* assertions */
	CASSERT((rp->_base.type == CRegionType_Path));

	/* free the point list */
	CFree(rp->points);

	/* free the type list */
	CFree(rp->types);

	/* free the node */
	CFree(rp);
}

static const CRegionInterpreterClass CRegionTransformer_Class =
{
	CRegionTransformer_Data,
	CRegionTransformer_Op,
	CRegionTransformer_FreeData
};
static const CRegionInterpreterClass CRegionTransformer_ClassSimple =
{
	CRegionTransformer_DataSimple,
	CRegionTransformer_Op,
	0
};

CINTERNAL void
CRegionTransformer_Initialize(CRegionTransformer *_this,
                              CAffineTransformF  *transform)
{
	/* assertions */
	CASSERT((_this     != 0));
	CASSERT((transform != 0));

	/* initialize the transformation */
	_this->transform = transform;

	/* initialize the base */
	if((CAffineTransform_XY(*transform) == 0.0f) &&
	   (CAffineTransform_YX(*transform) == 0.0f))
	{
		CRegionInterpreter_Initialize
			((CRegionInterpreter *)_this, &CRegionTransformer_ClassSimple);
	}
	else
	{
		CRegionInterpreter_Initialize
			((CRegionInterpreter *)_this, &CRegionTransformer_Class);
	}
}

CINTERNAL void
CRegionTransformer_Finalize(CRegionTransformer *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the members */
	{
		/* finalize the base */
		CRegionInterpreter_Finalize((CRegionInterpreter *)_this);

		/* finalize the transform */
		_this->transform = 0;
	}
}


#ifdef __cplusplus
};
#endif
