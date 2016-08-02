/*
 * CRegionTranslator.c - Region translator implementation.
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

#include "CRegionTranslator.h"
#include "CAffineTransform.h"

#ifdef __cplusplus
extern "C" {
#endif

static CStatus
CRegionTranslator_Data(CRegionInterpreter  *_this,
                       CRegionNode         *node,
                       void               **data)
{
	/* declarations */
	CRegionTranslator *rt;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((node  != 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* get this as a region translator */
	rt = ((CRegionTranslator *)_this);

	/* translate the node */
	if(node->type == CRegionType_Rectangle)
	{
		/* get the rectangle node */
		CRegionRect *rr = ((CRegionRect *)node);

		/* apply the translation */
		CRectangle_X(rr->rectangle) += CVector_X(rt->offset);
		CRectangle_Y(rr->rectangle) += CVector_Y(rt->offset);
	}
	else if(node->type == CRegionType_Path)
	{
		/* get the path node */
		CRegionPath *rp = ((CRegionPath *)node);

		/* apply the translation */
		CVectorF_TranslatePoints
			(&(rt->offset), rp->points, rp->count);
	}

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionTranslator_Op(CRegionInterpreter  *_this,
                     CRegionOp           *op,
                     void                *left,
                     void                *right,
                     void               **data)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((op    != 0));
	CASSERT((left  == 0));
	CASSERT((right == 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* return successfully */
	return CStatus_OK;
}

static const CRegionInterpreterClass CRegionTranslator_Class =
{
	CRegionTranslator_Data,
	CRegionTranslator_Op,
	0
};

CINTERNAL void
CRegionTranslator_Initialize(CRegionTranslator *_this,
                             CFloat             dx,
                             CFloat             dy)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the offset */
	CVector_X(_this->offset) = dx;
	CVector_Y(_this->offset) = dy;

	/* initialize the base */
	CRegionInterpreter_Initialize
		((CRegionInterpreter *)_this, &CRegionTranslator_Class);
}

CINTERNAL void
CRegionTranslator_Finalize(CRegionTranslator *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the members */
	{
		/* finalize the base */
		CRegionInterpreter_Finalize((CRegionInterpreter *)_this);

		/* finalize the offset */
		_this->offset = CVectorF_Zero;
	}
}

#ifdef __cplusplus
};
#endif
