/*
 * CRegionDisposer.c - Region disposer implementation.
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

#include "CRegionDisposer.h"

#ifdef __cplusplus
extern "C" {
#endif

static CStatus
CRegionDisposer_Op(CRegionInterpreter  *_this,
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

	/* dispose of the operation node */
	CFree(op);

	/* return successfully */
	return CStatus_OK;
}

static CStatus
CRegionDisposer_Data(CRegionInterpreter  *_this,
                     CRegionNode         *node,
                     void               **data)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((node  != 0));
	CASSERT((data  != 0));

	/* dispose of the data node */
	CRegionData_Free(node);

	/* return successfully */
	return CStatus_OK;
}

static void
CRegionDisposer_FreeData(void *data)
{
	/* assertions */
	CASSERT((data != 0));

	/* free the node members, as needed */
	if(CRegionNode_IsOp(((CRegionNode *)data)))
	{
		/* get the operation node */
		CRegionOp *op = ((CRegionOp *)data);

		/* free the left operand node, as needed */
		if(op->left) { CRegionDisposer_FreeData(op->left); }

		/* free the right operand node, as needed */
		if(op->right) { CRegionDisposer_FreeData(op->right); }
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

static const CRegionInterpreterClass CRegionDisposer_Class =
{
	CRegionDisposer_Data,
	CRegionDisposer_Op,
	CRegionDisposer_FreeData
};

CINTERNAL void
CRegionDisposer_Initialize(CRegionDisposer *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the base */
	CRegionInterpreter_Initialize
		((CRegionInterpreter *)_this, &CRegionDisposer_Class);
}

CINTERNAL void
CRegionDisposer_Finalize(CRegionDisposer *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the base */
	CRegionInterpreter_Finalize((CRegionInterpreter *)_this);
}


#ifdef __cplusplus
};
#endif
