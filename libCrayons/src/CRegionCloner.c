/*
 * CRegionCloner.c - Region cloner implementation.
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

#include "CRegionCloner.h"

#ifdef __cplusplus
extern "C" {
#endif

static CStatus
CRegionCloner_Op(CRegionInterpreter  *_this,
                 CRegionOp           *op,
                 void                *left,
                 void                *right,
                 void               **data)
{
	/* declarations */
	CRegionOp *ro;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((op    != 0));
	CASSERT((left  != 0));
	CASSERT((right != 0));
	CASSERT((data  != 0));

	/* create the operation node */
	if(!(CRegionOp_Alloc(ro)))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the operation node */
	ro->_base = op->_base;
	ro->left  = (CRegionNode *)left;
	ro->right = (CRegionNode *)right;

	/* set the clone */
	*data = (void *)ro;

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionCloner_Data(CRegionInterpreter  *_this,
                   CRegionNode         *node,
                   void               **data)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((node  != 0));
	CASSERT((data  != 0));

	/* set the data to the default */
	*data = 0;

	/* clone the data node */
	if(node->type == CRegionType_Path)
	{
		/* declarations */
		CRegionPath *rp;

		/* get the count and sizes */
		const CUInt32 count = ((CRegionPath *)node)->count;
		const CUInt32 sizeP = (count * sizeof(CPointF));
		const CUInt32 sizeT = (count * sizeof(CByte));

		/* create the path node */
		if(!(CRegionPath_Alloc(rp)))
		{
			return CStatus_OutOfMemory;
		}

		/* initialize the path node */
		rp->_base    = *node;
		rp->count    = count;
		rp->fillMode = ((CRegionPath *)node)->fillMode;

		/* allocate the point list */
		if(!(rp->points = (CPointF *)CMalloc(sizeP)))
		{
			CFree(rp);
			return CStatus_OutOfMemory;
		}

		/* allocate the type list */
		if(!(rp->types = (CByte *)CMalloc(sizeT)))
		{
			CFree(rp->points);
			CFree(rp);
			return CStatus_OutOfMemory;
		}

		/* copy the points */
		CMemCopy(rp->points, ((CRegionPath *)node)->points, sizeP);

		/* copy the types */
		CMemCopy(rp->types, ((CRegionPath *)node)->types, sizeT);

		/* set the clone */
		*data = (void *)rp;
	}
	else if(node->type == CRegionType_Rectangle)
	{
		/* declarations */
		CRegionRect *rr;

		/* create the rectangle node */
		if(!(CRegionRect_Alloc(rr)))
		{
			return CStatus_OutOfMemory;
		}

		/* initialize the rectangle node */
		rr->_base     = *node;
		rr->rectangle = ((CRegionRect *)node)->rectangle;

		/* set the clone */
		*data = (void *)rr;
	}
	else
	{
		/* declarations */
		CRegionNode *rn;

		/* create the node */
		if(!(CRegionNode_Alloc(rn)))
		{
			return CStatus_OutOfMemory;
		}

		/* initialize the node */
		*rn = *node;

		/* set the clone */
		*data = (void *)rn;
	}

	/* return successfully */
	return CStatus_OK;
}

static void
CRegionCloner_FreeData(void *data)
{
	/* assertions */
	CASSERT((data != 0));

	/* free the node members, as needed */
	if(CRegionNode_IsOp(((CRegionNode *)data)))
	{
		/* get the operation node */
		CRegionOp *op = ((CRegionOp *)data);

		/* free the left operand node, as needed */
		if(op->left) { CRegionCloner_FreeData(op->left); }

		/* free the right operand node, as needed */
		if(op->right) { CRegionCloner_FreeData(op->right); }
	}
	else if(((CRegionNode *)data)->type == CRegionType_Path)
	{
		/* get the path node */
		CRegionPath *rp = ((CRegionPath *)data);

		/* free the point list */
		CFree(rp->points);

		/* free the type list */
		CFree(rp->types);
	}

	/* free the node */
	CFree(data);
}

static const CRegionInterpreterClass CRegionCloner_Class =
{
	CRegionCloner_Data,
	CRegionCloner_Op,
	CRegionCloner_FreeData
};

CINTERNAL void
CRegionCloner_Initialize(CRegionCloner *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the base */
	CRegionInterpreter_Initialize
		((CRegionInterpreter *)_this, &CRegionCloner_Class);
}

CINTERNAL void
CRegionCloner_Finalize(CRegionCloner *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the base */
	CRegionInterpreter_Finalize((CRegionInterpreter *)_this);
}


#ifdef __cplusplus
};
#endif
